using System;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using w2.Plugin.P0011_Intercom.ReSync.Util;
using System.Configuration;

namespace w2.Plugin.P0011_Intercom.ReSync
{
	/// <summary>
	/// インターコム再同期ツール
	/// </summary>
	class Program
	{
		static void Main(string[] args)
		{
			// 接続文字列
			string connStr = ConfigurationManager.ConnectionStrings["w2Database"].ToString();
			// エラーログファイルパス
			string errLogFilePath = ConfigurationManager.AppSettings.Get("errLogFilePath");
			// 連携データ格納用ディレクトリパス
			string targetDirPath = ConfigurationManager.AppSettings.Get("targetDirPath");
			// 連携成功データ格納用ディレクトリパス
			string successDirPath = ConfigurationManager.AppSettings.Get("successDirPath");

			// ファイル制御ユーティリティ
			FileUtil fileUtil = new FileUtil();
			// エラーログライター
			ErrorLogWriter errorLogWriter = new ErrorLogWriter(errLogFilePath);
			
			// 送信用データセット
			DataSet sendDataSet;
			// 戻り結果データセット
			DataSet rtnds;

			// 連携データ格納ディレクトリ内の対象ファイル分ループ
			foreach (string csvFilePath in Directory.GetFiles(targetDirPath, "*CallWebServiceErrorDataOnRegistered.csv"))
			{
				sendDataSet = new DataSet();

				try
				{
					// ログファイルのロード
					DataTable loadt = fileUtil.LoadUploadCSV(csvFilePath);

					// 連携用同期データの生成
					sendDataSet.Tables.Add(CreateProcTable("UserRegist"));
					sendDataSet.Tables.Add(CreateSendSyncData(loadt));

					//会員情報連携実行
					using (Intercom_User.w2iUserSyncService sc = new Intercom_User.w2iUserSyncService())
					{
						rtnds = sc.UserSync(sendDataSet);
					}

					//処理結果データテーブル判断
					if (rtnds == null)
					{
						throw new Exception("Webサービスからデータセットが返却されませんでした。");
					}

					if (rtnds.Tables.Count == 0)
					{
						throw new Exception("Webサービスからデータテーブルが一つも返却されませんでした。");
					}

					if (rtnds.Tables.Contains("ResultTable") == false)
					{
						throw new Exception("Webサービスから処理結果データテーブルが一つも返却されませんでした。");
					}

					if (rtnds.Tables["ResultTable"].Rows.Count == 0)
					{
						throw new Exception("Webサービスから処理結果行が一件も返却されませんでした。");
					}

					if (rtnds.Tables["ResultTable"].Columns.Contains("ProcResult") == false)
					{
						throw new Exception("Webサービスから処理結果区分が返却されませんでした。");
					}

					if (rtnds.Tables["ResultTable"].Columns.Contains("ProcMessage") == false)
					{
						throw new Exception("Webサービスから処理メッセージが返却されませんでした。");
					}

					string result = (string)rtnds.Tables["ResultTable"].Rows[0]["ProcResult"];
					string procResulMsgt = (string)rtnds.Tables["ResultTable"].Rows[0]["ProcMessage"];

					// 連携結果チェック
					if (result != "Success")
					{
						throw new Exception("Webサービスから処理区分がFailedで返却されました。：" + procResulMsgt);
					}

					// 連携結果取り出し
					string intercomId = (string)rtnds.Tables["UserIDTable"].Rows[0]["UserID"];
					
					// 連携結果反映
					UpdateIntercomID(connStr, loadt.Rows[0]["user_id"].ToString(), intercomId);

					// うまく連携できたcsvファイルを移動
					File.Move(csvFilePath, successDirPath + @"\" + new FileInfo(csvFilePath).Name);

				}
				catch (Exception ex)
				{
					// エラーログ出力
					errorLogWriter.write("対象ファイルパス：[" + csvFilePath + "]" + ex.Message);
				}
			}

		}

		#region -CreateProcTable 処理区分用データテーブル作成
		/// <summary>
		/// 処理区分用データテーブル作成
		/// </summary>
		/// <param name="procType">設定する処理区分の値</param>
		/// <returns>作成した処理区分用データテーブル</returns>
		private static DataTable CreateProcTable(string procType)
		{
			DataTable dt = new DataTable("ProcTable");
			dt.Columns.Add("ProcType");

			DataRow dr = dt.NewRow();
			dr[0] = procType;

			dt.Rows.Add(dr);

			return dt;
		}
		#endregion

		#region -UpdateIntercomID インターコムユーザID更新
		/// <summary>
		/// インターコムユーザID更新
		/// </summary>
		/// <param name="connStr"></param>
		/// <param name="w2id"></param>
		/// <param name="intercomid"></param>
		private static void UpdateIntercomID(string connStr, string w2id, string intercomid)
		{
			string constr = connStr;

			using (SqlConnection conn = new SqlConnection(constr))
			{
				conn.Open();

				string sqlstr = " UPDATE w2_User "
					+ " SET ATTRIBUTE1 = @ic_UserID "
					+ " where user_id = @user_id ";

				SqlCommand cmd = new SqlCommand(sqlstr, conn);

				cmd.Parameters.AddWithValue("@user_id", w2id);
				cmd.Parameters.AddWithValue("@ic_UserID", intercomid);
				cmd.ExecuteNonQuery();

				conn.Close();

			}

		}

		#endregion

		#region -CreateSendSyncData 連携用同期データ生成
		/// <summary>
		/// 連携用同期データ生成
		/// </summary>
		/// <param name="loadData">データ生成の元となるデータ</param>
		/// <returns>連携処理に投げる同期用のデータ</returns>
		private static DataTable CreateSendSyncData(DataTable loadData)
		{
			DataTable dt = new DataTable("UserTable");

			dt.Columns.Add("user_id");
			dt.Columns.Add("user_kbn");
			dt.Columns.Add("mall_id");
			dt.Columns.Add("name");
			dt.Columns.Add("name1");
			dt.Columns.Add("name2");
			dt.Columns.Add("name_kana");
			dt.Columns.Add("name_kana1");
			dt.Columns.Add("name_kana2");
			dt.Columns.Add("mail_addr");
			dt.Columns.Add("zip1");
			dt.Columns.Add("zip2");
			dt.Columns.Add("addr1");
			dt.Columns.Add("addr2");
			dt.Columns.Add("addr3");
			dt.Columns.Add("addr4");
			dt.Columns.Add("tel1_1");
			dt.Columns.Add("tel1_2");
			dt.Columns.Add("tel1_3");
			dt.Columns.Add("sex");
			dt.Columns.Add("birth_year");
			dt.Columns.Add("birth_month");
			dt.Columns.Add("birth_day");
			dt.Columns.Add("company_name");
			dt.Columns.Add("company_post_name");
			dt.Columns.Add("attribute1");
			dt.Columns.Add("login_id");
			dt.Columns.Add("password");
			dt.Columns.Add("mail_flg");
			dt.Columns.Add("del_flg");
			dt.Columns.Add("last_changed");

			DataRow dr = dt.NewRow();

			dr["user_id"] = loadData.Rows[0]["user_id"];
			dr["user_kbn"] = "PC_USER";
			dr["mall_id"] = "OWN_SITE";
			dr["name1"] = loadData.Rows[0]["name1"];
			dr["name2"] = loadData.Rows[0]["name2"];
			dr["name_kana1"] = loadData.Rows[0]["name_kana1"];
			dr["name_kana2"] = loadData.Rows[0]["name_kana2"];
			dr["mail_addr"] = loadData.Rows[0]["mail_addr"];
			dr["zip1"] = loadData.Rows[0]["zip1"];
			dr["zip2"] = loadData.Rows[0]["zip2"];
			dr["addr1"] = loadData.Rows[0]["addr1"];
			dr["addr2"] = loadData.Rows[0]["addr2"];
			dr["addr3"] = loadData.Rows[0]["addr3"];
			dr["addr4"] = loadData.Rows[0]["addr4"];
			dr["tel1_1"] = loadData.Rows[0]["tel1_1"];
			dr["tel1_2"] = loadData.Rows[0]["tel1_2"];
			dr["tel1_3"] = loadData.Rows[0]["tel1_3"];
			dr["sex"] = loadData.Rows[0]["sex"];
			dr["birth_year"] = loadData.Rows[0]["birth_year"];
			dr["birth_month"] = loadData.Rows[0]["birth_month"];
			dr["birth_day"] = loadData.Rows[0]["birth_day"];
			dr["company_name"] = loadData.Rows[0]["company_name"];
			dr["company_post_name"] = loadData.Rows[0]["company_post_name"];
			dr["attribute1"] = loadData.Rows[0]["attribute1"];
			dr["login_id"] = loadData.Rows[0]["login_id"];
			dr["password"] = loadData.Rows[0]["password"];
			dr["mail_flg"] = loadData.Rows[0]["mail_flg"];
			dr["del_flg"] = loadData.Rows[0]["del_flg"];
			dr["last_changed"] = loadData.Rows[0]["last_changed"];

			dt.Rows.Add(dr);

			return dt;
		}
		#endregion

	}

}
