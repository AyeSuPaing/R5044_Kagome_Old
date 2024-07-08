/*
=========================================================================================================
  Module      : Csvファイル読み込みクラス(CsvReader.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace w2.ExternalAPI.Common.FrameWork.File
{
	/// <summary>
	///	Csvファイル読み込みクラス
	/// </summary>
	/// <remarks>
	/// Csvファイルの読み込み制御を行う
	/// </remarks>
	public class CsvReader : IDisposable 
	{
		/// <summary> 読み込みソース </summary>
		private TextReader CurrentReader { get; set; }
		/// <summary> CSV情報構造体 </summary>
		private CsvSetting m_csvSetting;
	
		#region コンストラクタ

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reader">テキストリーダー</param>
		/// <param name="csvSetting">Csv設定</param>
		public CsvReader(TextReader reader,CsvSetting csvSetting)
		{
			this.CurrentReader = reader;
			m_csvSetting = csvSetting;
		}
		#endregion


		#region #ReadLine #CSV行分割
		/// <summary>CSV行を読んで分割、ストリームを先に進める</summary>
		/// <param name="strCsvLine">CSV列</param>
		/// <returns>分割後の文字配列</returns>
		public DataRow ReadLine()
		{
			if (m_csvSetting.Enclosure == m_csvSetting.Delimiter) throw new ArgumentException("不正なCSV設定です。包み文字と区切り文字が等しくなっています。");

			List<string> result = new List<string>();
			StringBuilder quotedText = new StringBuilder();
			bool isEnclosed = false;
			bool isEscaped = false;
			bool isWaitEnclosure = true;
			string restOfLine = "";
			var isEnclosedCsv2 = false;

			do
			{
				string csvLine = this.CurrentReader.ReadLine();
				if (csvLine == null)
				{
					if ((!string.IsNullOrEmpty(m_csvSetting.Enclosure)) && (isEnclosed))
					{
						throw new Exception("区切り文字で正しく閉じられていません。");
					}
					else
					{
						break;
					}
				}
				csvLine = (string.IsNullOrEmpty(restOfLine) ? "" : restOfLine) + csvLine;

				// 正規表現では、厳密なCSVの読み取りは難しい。StateMachineを用いる必用がある。
				for (int charIndex = 0; charIndex < csvLine.Length; charIndex++)	// 1文字ずつ調査する
				{
					char curChar = csvLine[charIndex];
					char postChar = charIndex + 1 < csvLine.Length ? csvLine[charIndex + 1] : ' ';

					// 包み文字中
					if (isEnclosed)
					{
						// エスケープされてるので、そのまま溜める
						if (isEscaped)
						{
							isEscaped = false;
							quotedText.Append(curChar);
						}
						// エスケープされていない
						else
						{
							// 包み文字
							if ((curChar.ToString() == m_csvSetting.Enclosure)
								// もしくは、包み文字が空白の場合、区切り文字
								|| ((m_csvSetting.Enclosure == "") && (curChar.ToString() == m_csvSetting.Delimiter)))
							{
								result.Add(quotedText.ToString());
								quotedText = new StringBuilder();
								isEnclosed = false;	// 区切り文字の外へ
							}
							// エスケープされた包み文字
							else if (curChar == '\\')
							{
								if (postChar.ToString() == "\\" || postChar.ToString() == m_csvSetting.Enclosure)
								{
									isEscaped = true;
								}
								else
								{
									quotedText.Append(curChar);
								}
							}
							else if (curChar.ToString() != "\"")
							{
								quotedText.Append(curChar);
							}
						}
					}
					// 包み文字外。包み文字に遭遇すれば、包み文字中とする
					else
					{
						// 包み文字探し
						if (isWaitEnclosure)
						{
							if ((string.IsNullOrEmpty(m_csvSetting.Enclosure)) || (curChar.ToString() == m_csvSetting.Enclosure))
							{
								if (string.IsNullOrEmpty(m_csvSetting.Enclosure) && (curChar.ToString() != "\"")) charIndex--;	// 包み文字が空白なら今回の文字を読まなかったことにする
								isEnclosedCsv2 = 
									((string.IsNullOrEmpty(m_csvSetting.Enclosure))
										&& (curChar.ToString() == "\"")
										&& (isEnclosedCsv2 == false));

								isEnclosed = true;

								if (!string.IsNullOrEmpty(m_csvSetting.Enclosure)) isWaitEnclosure = false;
							}
						}
						else
						{
							// 要素区切り記号
							if (curChar.ToString() == m_csvSetting.Delimiter)
							{
								isWaitEnclosure = true;
							}
							// 行区切り記号
							else if (m_csvSetting.LineDelimiter.StartsWith(curChar.ToString()))
							{
								string lineDelimiter = csvLine.Substring(charIndex, m_csvSetting.LineDelimiter.Length);
								// 改行
								if (m_csvSetting.LineDelimiter.StartsWith(lineDelimiter))
								{
									// 処理されなかった文字列を溜めておく
									restOfLine = csvLine.Substring(charIndex + m_csvSetting.LineDelimiter.Length);

									break;
								}
							}
						}
					}
				}

				// 包み文字が空白の場合、今あるものを最後の要素として、パースを終了
				if (string.IsNullOrEmpty(m_csvSetting.Enclosure) && (isEnclosedCsv2 != true))
				{
					isEnclosed = false;
					result.Add(quotedText.ToString());
				}
				// 包み文字が空白でなければ、改行を取得して次の行をパース 
				else
				{
					restOfLine = "\r\n";
					
				}
			} while ((isEnclosed) || m_csvSetting.LineDelimiter != "\r\n");

			// データ列にして返却
			return ConvertToDataRow(result.ToArray());
		}
		#endregion

		#region ConvertToDataRow
		/// <summary>
		/// オブジェクト配列をDataRowに変換
		/// </summary>
		/// <param name="results"></param>
		/// <returns></returns>
		private DataRow ConvertToDataRow(object[] results)
		{
			if (results.Length == 0) return null;

			DataTable table = new DataTable();
			foreach(var item in results)
			{
				DataColumn column = new DataColumn();
				column.DataType = typeof(string);
				column.ReadOnly = true;
				table.Columns.Add(column);
			}
			
			DataRow row = table.NewRow();
			row.ItemArray = results;
			return row;
		}
		#endregion

		#region Dispose
		public void Dispose()
		{
			if(this.CurrentReader != null)
			{	
				this.CurrentReader.Close();
				this.CurrentReader.Dispose();
			}

			this.CurrentReader = null;
		}
		#endregion
	}
}
