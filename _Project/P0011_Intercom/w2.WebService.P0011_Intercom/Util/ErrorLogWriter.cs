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
using System.Web.Configuration;
using System.Data;
using System.IO;
using System.Text;


namespace w2.Plugin.P0011_Intercom.WebService.Util
{
	public class ErrorLogWriter : FileWriterBase
	{
		const string FileName = "ErrorLog.txt";
		string m_fileDir = "";

		public ErrorLogWriter()
		{
			m_fileDir = WebConfigurationManager.AppSettings[WebServiceConst.APP_SETTINGS_KEY_RECEIVE_DATA_LOG_DIR].ToString() + @"\";
		}

		public void write(string writeStr)
		{
			string writewilepath = m_fileDir + DateTime.Now.ToString("yyyyMMdd") + "_" + FileName;
			base.WriteFile(writewilepath, writeStr);

		}

		public void write(DataSet ds,Exception ex)
		{
			string writewilepath = m_fileDir + DateTime.Now.ToString("yyyyMMdd") + "_" + FileName;
			StringBuilder sBuilder = new StringBuilder();

			sBuilder.Append(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "   " + "受領データ：");

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

				sBuilder.Append("エラーメッセージ：" + ex.Message);
				sBuilder.Append("スタックトレース：" + ex.StackTrace);

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