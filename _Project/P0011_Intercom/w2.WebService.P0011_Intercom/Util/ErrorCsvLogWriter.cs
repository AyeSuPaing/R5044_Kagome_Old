/*
=========================================================================================================
  Module      : エラーCSVログ用クラス(ErrorCsvLogWriter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2011 All Rights Reserved.
  MEMO        : エラーCSVログへの処理を司るクラス。
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
	public class ErrorCsvLogWriter : FileWriterBase 
	{
		const string FileName = "WebSeviceErrorData";
		const string Extension = ".csv";
		string m_fileDir = "";

		public ErrorCsvLogWriter()
		{
			m_fileDir = WebConfigurationManager.AppSettings[WebServiceConst.APP_SETTINGS_KEY_ERROR_CSV_LOG_DIR].ToString() + @"\";
		}
				
		public void write(DataTable dt,string proc)
		{
			StringBuilder sBuilder = new StringBuilder();
			try
			{
				string writewilepath = m_fileDir + DateTime.Now.ToString("yyyyMMddhhmmss") + "_" + FileName + proc + Extension;
				if (dt == null)
				{
					WriteFile(writewilepath, "データがとれませんでした");
					return;
				}

				//データ行カウンタ
				int i = 1;

				foreach (DataRow dr in dt.Rows)
				{
					if (i == 1)
					{
						//列カウンタ
						int j = 1;
						//列ヘッダ
						string headStr = "";
						//ヘッダ出力
						foreach (DataColumn dc in dt.Columns)
						{
							//区切り文字のカンマ追加
							if (j > 1) { headStr = headStr + ","; }
							//項目毎に出力
							headStr = headStr + dc.ColumnName;
							//列カウンタインクリメント
							j++;
						}

						sBuilder.AppendLine(headStr);
					}

					//データ出力
					//列カウンタ
					int k = 1;
					string valStr = "";
					foreach (DataColumn dc in dt.Columns)
					{
						//区切り文字のカンマ追加
						if (k > 1)	{ valStr = valStr + ","; }
						//項目毎に出力
						valStr = valStr + dr[dc.ColumnName];
						//列カウンタインクリメント
						k++;
					}

					sBuilder.AppendLine(valStr);

					//データ行カウンタインクリメント
					i++;

				}
				writewilepath = m_fileDir + DateTime.Now.ToString("yyyyMMddhhmmss") + "_" + FileName + proc + Extension;
				WriteFile(writewilepath, sBuilder.ToString());

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
	}
}