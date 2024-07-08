/*
=========================================================================================================
  Module      : エラーログ用クラス(ErrorLogWriter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2011 All Rights Reserved.
  MEMO        : エラーログへの処理を司るクラス。
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
	/// エラーログ用クラス
	/// </summary>
	class ErrorLogWriter : FileWriterBase
	{
		const string FileName = "Err_CooperationWebServiceLog.txt";
		string m_fileDir = "";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="logFileDirPath">ログ出力先フォルダパス</param>
		public ErrorLogWriter(string logFileDirPath)
		{
			m_fileDir = logFileDirPath + @"\";
		}

		/// <summary>
		/// ログ出力
		/// </summary>
		/// <param name="writeStr">出力する文字列</param>
		public void Write(string writeStr)
		{
			try
			{
				string writewilepath = m_fileDir + DateTime.Now.ToString("yyyyMM") + "_" + FileName;
				base.WriteFile(writewilepath, DateTime.Now.ToString("yyyy/MM/dd hh:mi:ss") + " " + writeStr);
			}
			catch
			{
				//エラー時は何もしない
			}

		}
		#region -ConvNullToEmpty NullまたはDBNullかどうかをチェック

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
