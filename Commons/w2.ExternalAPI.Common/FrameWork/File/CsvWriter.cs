/*
=========================================================================================================
  Module      : Csv書き込みクラス(CsvWriter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.IO;
using System.Linq;

namespace w2.ExternalAPI.Common.FrameWork.File
{
	/// <summary>
	///	Csvファイル書き込みクラス
	/// </summary>
	/// <remarks>
	/// Csvファイルの書き込み制御を行う
	/// </remarks>
	public class CsvWriter : IDisposable
	{
		#region メンバ変数
		private readonly CsvSetting m_csvSetting;
		#endregion

		#region プロパティ
		/// <summary> 書き込みストリーム </summary>
		protected TextWriter TextWriter { get; set; }
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="textWriter"></param>
		/// <param name="csvSetting"> </param>
		public CsvWriter(TextWriter textWriter, CsvSetting csvSetting)
		{
			TextWriter = textWriter;
			m_csvSetting = csvSetting;
		}
		#endregion

		#region +WriteLine 1行書き込み
		/// <summary>1行書き込み</summary>
		public void WriteLine(object[] objects)
		{
			string[] enclosed = objects.Select<object, string>(obj => m_csvSetting.Enclosure + obj.ToString() + m_csvSetting.Enclosure).ToArray();
			string line = string.Join(m_csvSetting.Delimiter, enclosed);

			if(m_csvSetting.DoesEscapeLineDelimiter)
			{
				line = line.Replace("\r\n", m_csvSetting.LineDelimiterEscapeString);
			}
			TextWriter.Write(line + m_csvSetting.LineDelimiter);
		}
		#endregion

		#region +Flush ストリーム更新
		/// <summary>
		/// ストリーム更新
		/// </summary>
		public void Flush()
		{
			TextWriter.Flush();
		}
		#endregion

		#region +Dispose
		/// <summary>Dispose</summary>
		public void Dispose()
		{
			if (this.TextWriter != null)
			{
				this.TextWriter.Close();
				this.TextWriter.Dispose();
			}

			this.TextWriter = null;
		}
		#endregion
	}
}
