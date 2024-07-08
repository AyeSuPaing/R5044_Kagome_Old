/*
=========================================================================================================
  Module      : 送受信データログ用クラス(SendDataWriter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2011 All Rights Reserved.
  MEMO        : 送受信データログへの処理を司るクラス。
=========================================================================================================
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;
using System.Text;


namespace w2.Plugin.P0011_Intercom.CooperationWebService
{
	/// <summary>
	/// 送受信データログ用クラス
	/// </summary>
	class DataSetLogWriter : FileWriterBase 
	{
		const string FileName = "CooperationWebServiceLog.txt";
		string m_fileDir = "";
		
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="logFileDirPath">ログ出力フォルダパス</param>
		public DataSetLogWriter(string logFileDirPath)
		{
			m_fileDir = logFileDirPath + @"\";
		}

		/// <summary>
		/// ログ出力
		/// </summary>
		/// <param name="writeStr">ログ出力する文字列</param>
		public void Write(string writeStr)
		{
			string writewilepath = m_fileDir + DateTime.Now.ToString("yyyyMM") + "_" + FileName;
			base.WriteFile(writewilepath, writeStr);

		}
		/// <summary>
		/// ログ出力
		/// </summary>
		/// <param name="ds">出力するデータセット</param>
		/// <param name="procType">処理区分</param>
		/// <param name="way">送受信方向</param>
		public void Write(DataSet ds, string procType, string way)
		{
			string writewilepath = m_fileDir + DateTime.Now.ToString("yyyyMM") + "_" + FileName;
			StringBuilder sBuilder = new StringBuilder();

			sBuilder.Append(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "  " + "処理区分：" + procType + "   " +  way + "データ：");

			try
			{
				foreach (DataTable dt in ds.Tables)
				{
					sBuilder.Append("テーブル名[" + dt.TableName + "]");
					//データ行カウンタ
					int rCnt = 1;

					foreach (DataRow dr in dt.Rows)
					{
						sBuilder.Append(rCnt.ToString() + "行目<<");
						foreach (DataColumn dc in dt.Columns)
						{
							//項目毎に出力
							sBuilder.Append(dc.ColumnName + ":" + ConvNullToEmpty(dr[dc.ColumnName]) + ";");
						}
						sBuilder.Append(">>" + rCnt.ToString() + "行目End");
						rCnt++;
					}

				}

				WriteFile(writewilepath,sBuilder.ToString());

			}
			catch
			{
				//エラー時何もしない
			}
			finally
			{
				sBuilder.Clear();
				sBuilder = null;
			}
						
		}

		#region NullまたはDBNullかどうかをチェック

		/// <summary>
		/// NullまたはDBNullの場合にはEmptyに
		/// </summary>
		/// <param name="targetValue"></param>
		/// <returns></returns>
		private string ConvNullToEmpty(object targetValue)
		{
			if (targetValue == null)
			{
				return "";
			}

			if (targetValue == DBNull.Value)
			{
				return "";
			}

			return targetValue.ToString();

		}

		#endregion
	}
}
