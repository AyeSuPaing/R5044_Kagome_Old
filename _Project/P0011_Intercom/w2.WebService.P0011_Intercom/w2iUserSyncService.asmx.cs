using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;

namespace w2.Plugin.P0011_Intercom.WebService
{
	/// <summary>
	/// w2iUserSyncService の概要の説明です
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	// この Web サービスを、スクリプトから ASP.NET AJAX を使用して呼び出せるようにするには、次の行のコメントを解除します。 
	// [System.Web.Script.Services.ScriptService]
	public class w2iUserSyncService : System.Web.Services.WebService
	{

		
		[WebMethod]
		public DataSet SyncUserData(DataSet ds)
		{
			//InterCom用会員情報連携webサービス
			//テスト用コード

			DataSet rtnDS = new DataSet();

			//引数データ分解
			try
			{
				string proc = "";

				if (IsDBNullAndNullCheck(ds.Tables["ProcTable"].Rows[0]["ProcType"]) == false)
				{
					proc = ds.Tables["ProcTable"].Rows[0]["ProcType"].ToString();
				}

				switch (proc)
				{
					case "UserRegist":
						//会員登録
						rtnDS.Tables.Add(CreateResultDataTable("Success", "正常終了"));
						//採番番号
						rtnDS.Tables.Add(CreateUserIDTable());

						break;

					case "UserDelete":
						//会員退会
						rtnDS.Tables.Add(CreateResultDataTable("Success", "正常終了"));

						break;

					default:
						//上記以外
						//エラー情報セット
						rtnDS.Tables.Add(CreateResultDataTable("Failed", "処理区分が不正です。指定処理区分→" + proc));

						break;
				}


			}
			catch (Exception ex)
			{
				if (rtnDS.Tables.Contains("ResultData"))
				{
					//すでに処理結果テーブルが生成されていた場合
					rtnDS.Tables["ResultData"].Rows[0]["ProcResult"] = "Failed";
					rtnDS.Tables["ResultData"].Rows[0]["ProcMessage"] = "エラーが発生しました。エラーメッセージ→" + ex.Message;
				}
				else
				{
					rtnDS.Tables.Add(CreateResultDataTable("Failed", "エラーが発生しました。エラーメッセージ→" + ex.Message));
				}

			}
			finally
			{
				//特になし

			}

			return rtnDS;

		}

		[WebMethod]
		public DataSet UserSync(DataSet ds)
		{
			//InterCom用会員情報連携webサービス
			//テスト用コード

			DataSet rtnDS = new DataSet();

			//引数データ分解
			try
			{
				string proc = "";

				if (IsDBNullAndNullCheck(ds.Tables["ProcTable"].Rows[0]["ProcType"]) == false)
				{
					proc = ds.Tables["ProcTable"].Rows[0]["ProcType"].ToString();
				}

				switch (proc)
				{
					case "UserRegist":
						//会員登録
						rtnDS.Tables.Add(CreateResultDataTable("Success", "正常終了"));
						//採番番号
						rtnDS.Tables.Add(CreateUserIDTable());

						break;

					case "UserDelete":
						//会員退会
						rtnDS.Tables.Add(CreateResultDataTable("Success", "正常終了"));

						break;

					default:
						//上記以外
						//エラー情報セット
						rtnDS.Tables.Add(CreateResultDataTable("Failed", "処理区分が不正です。指定処理区分→" + proc));

						break;
				}


			}
			catch (Exception ex)
			{
				if (rtnDS.Tables.Contains("ResultData"))
				{
					//すでに処理結果テーブルが生成されていた場合
					rtnDS.Tables["ResultData"].Rows[0]["ProcResult"] = "Failed";
					rtnDS.Tables["ResultData"].Rows[0]["ProcMessage"] = "エラーが発生しました。エラーメッセージ→" + ex.Message;
				}
				else
				{
					rtnDS.Tables.Add(CreateResultDataTable("Failed", "エラーが発生しました。エラーメッセージ→" + ex.Message));
				}

			}
			finally
			{
				//特になし

			}

			return rtnDS;

		}

		#region ヘルパ

		/// <summary>
		/// テスト用イベントIDテーブル生成
		/// </summary>
		/// <returns></returns>
		private DataTable CreateEventIDTable()
		{

			//テスト用イベントリスト
			//好きに変更してね
			List<string> evelst = new List<string> { "0001", "0002", "0003" };

			DataTable eventIDTable = new DataTable("EventTable");

			eventIDTable.Columns.Add("EventID");
			eventIDTable.Columns.Add("SerialFlag");
			eventIDTable.Columns.Add("DisplayOrder");

			int i = 0;

			foreach (string eve in evelst)
			{
				DataRow dr1 = eventIDTable.NewRow();
				dr1["EventID"] = eve;
				dr1["DisplayOrder"] = i;

				if (eve == "0003")
				{
					//このイベントの場合はシリアル対象とする
					dr1["SerialFlag"] = "1";

				}
				else
				{
					dr1["SerialFlag"] = "0";
				}

				eventIDTable.Rows.Add(dr1);

				i++;
			}

			return eventIDTable;
		}


		/// <summary>
		/// テスト用商品IDテーブル生成
		/// </summary>
		/// <returns></returns>
		private DataTable CreateRecommendTargetTable(string eventid)
		{

			//テスト用レコメンド対象商品リスト
			//好きに変更してね
			List<string> evelst = new List<string> { "0001", "0002", "0003" };

			DataTable targetIDTable = new DataTable("RecommendProductTable");

			targetIDTable.Columns.Add("EventID");
			targetIDTable.Columns.Add("ProductID");
			targetIDTable.Columns.Add("VariationID");

			Dictionary<string, string> targetDic = new Dictionary<string, string>();

			switch (eventid)
			{
				case "0001":
					targetDic.Add("0000282", "0000282");
					targetDic.Add("0000296", "0000296");
					break;
				case "0002":
					targetDic.Add("0868286Y", "0868286Y");
					break;
				case "0003":
					targetDic.Add("0780003SER", "0780003SER");
					targetDic.Add("0868263SER", "0868263SER");
					targetDic.Add("0868288Y", "0868288Y-blu");
					break;
				default:
					targetDic.Add("0215913", "0215913");
					targetDic.Add("0215925", "0215925");
					break;
			}

			foreach (string key in targetDic.Keys)
			{
				DataRow dr1 = targetIDTable.NewRow();
				dr1["EventID"] = eventid;
				dr1["ProductID"] = key;
				dr1["VariationID"] = targetDic[key];
				targetIDTable.Rows.Add(dr1);
			}

			return targetIDTable;
		}

		private DataTable CreateResultDataTable(string procResult, string procMessage)
		{

			DataTable resultDataTable = new DataTable("ResultTable");

			resultDataTable.Columns.Add("ProcResult");
			resultDataTable.Columns.Add("ProcMessage");

			DataRow dr1 = resultDataTable.NewRow();
			dr1["ProcResult"] = procResult;
			dr1["ProcMessage"] = procMessage;

			resultDataTable.Rows.Add(dr1);

			return resultDataTable;
		}

		private DataTable CreateUserIDTable()
		{

			DataTable userIDTable = new DataTable("UserIDTable");

			userIDTable.Columns.Add("UserID");

			DataRow dr1 = userIDTable.NewRow();
			dr1["UserID"] = DateTime.Now.ToString("yyyyMMddHHmmssfff");

			userIDTable.Rows.Add(dr1);

			return userIDTable;
		}

		private bool IsDBNullAndNullCheck(object targetValue)
		{
			if (targetValue == null)
			{
				return true;
			}

			if (targetValue == DBNull.Value)
			{
				return true;
			}

			return false;

		}

		#endregion
	}
}
