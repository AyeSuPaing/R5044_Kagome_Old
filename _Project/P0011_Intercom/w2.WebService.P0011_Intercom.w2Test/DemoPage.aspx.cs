using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;

namespace w2.Plugin.P0011_Intercom.Webservice.w2Test
{
	public partial class DemoPage : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			//ページロード
		}

		protected void Button1_Click(object sender, EventArgs e)
		{
			DataSet ds = new DataSet();

			ds.Tables.Add(CreateProcTable(DropDownList1.SelectedValue.ToString()));

			switch (DropDownList1.SelectedValue.ToString())
			{
				case "UserRegist" :
					ds.Tables.Add(CreateInsertUserData(txtUserID.Text));
					break;

				case "UserModify" :
					ds.Tables.Add(CreateUpdateUserData(txtUserID.Text));
					break;
				case "UserDelete" :
					ds.Tables.Add(CreateUpdateUserData(txtUserID.Text));
					break;
			}

			DataSet rtnds = null;
			
			//会員情報連携
			using (websrv.Iw2ServiceClient sc = new websrv.Iw2ServiceClient())
			{
				rtnds = sc.UserSyncExecute(ds);
			}

			//戻り結果をラベルに
			StringBuilder sb = new StringBuilder();

			foreach (DataTable dt in rtnds.Tables)
			{
				sb.AppendLine("テーブル名[" + dt.TableName + "]");
				//データ行カウンタ
				int rCnt = 1;

				foreach (DataRow dr in dt.Rows)
				{
					sb.AppendLine(rCnt.ToString() + "行目__");
					foreach (DataColumn dc in dt.Columns)
					{
						//項目毎に出力
						sb.AppendLine(dc.ColumnName + ":" + ConvNullToEmpty(dr[dc.ColumnName]) + ";");
					}
					sb.AppendLine("__" + rCnt.ToString() + "行目End");
					rCnt++;
				}

			}

			lblRtDs.Text = sb.ToString();
		}

		protected void Button2_Click(object sender, EventArgs e)
		{

			DataSet ds = new DataSet();
			ds.Tables.Add(CreateUserIDTable());
			
			DataSet rtnds = null;

			//ワンタイムパス発行
			using (websrv.Iw2ServiceClient sc = new websrv.Iw2ServiceClient())
			{
				sc.ClientCredentials.UserName.UserName = "";
				sc.ClientCredentials.UserName.Password = "";

				//sc.Credentials = new System.Net.NetworkCredential("websrv", "test");
				rtnds = sc.CreateOnetimePassword(ds);
			}

			this.write(rtnds.Tables[0].Rows[0][0].ToString());

		}

		private void write(string target)
		{
			Response.Write(target + "<BR>");
		}

		//########################################################################################################################

		#region ヘルパ

		private DataTable CreateUserIDTable()
		{
			return CreateUserIDTable(DateTime.Now.ToString("yyyyMMddHHmmssfff"));
		}

		private DataTable CreateUserIDTable(string target)
		{

			DataTable userIDTable = new DataTable("UserIDTable");

			userIDTable.Columns.Add("UserID");

			DataRow dr1 = userIDTable.NewRow();
			dr1["UserID"] = target;

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

		/// <summary>
		/// テスト用の登録用ユーザーデータ作成
		/// </summary>
		/// <returns></returns>
		private DataTable CreateInsertUserData(string loginid)
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
			dt.Columns.Add("nick_name");
			dt.Columns.Add("mail_addr");
			dt.Columns.Add("mail_addr2");
			dt.Columns.Add("zip");
			dt.Columns.Add("zip1");
			dt.Columns.Add("zip2");
			dt.Columns.Add("addr");
			dt.Columns.Add("addr1");
			dt.Columns.Add("addr2");
			dt.Columns.Add("addr3");
			dt.Columns.Add("addr4");
			dt.Columns.Add("tel1");
			dt.Columns.Add("tel1_1");
			dt.Columns.Add("tel1_2");
			dt.Columns.Add("tel1_3");
			dt.Columns.Add("sex");
			dt.Columns.Add("birth");
			dt.Columns.Add("birth_year");
			dt.Columns.Add("birth_month");
			dt.Columns.Add("birth_day");
			dt.Columns.Add("company_name");
			dt.Columns.Add("company_post_name");
			dt.Columns.Add("advcode_first");
			dt.Columns.Add("attribute1");
			dt.Columns.Add("login_id");
			dt.Columns.Add("password");
			dt.Columns.Add("question");
			dt.Columns.Add("answer");
			dt.Columns.Add("career_id");
			dt.Columns.Add("mobile_uid");
			dt.Columns.Add("remote_addr");
			dt.Columns.Add("mail_flg");
			dt.Columns.Add("user_memo");
			dt.Columns.Add("del_flg");
			dt.Columns.Add("last_changed");
			dt.Columns.Add("member_rank_id");

			DataRow dr = dt.NewRow();

			dr["user_id"] = "";
			dr["user_kbn"] = "PC_USER";
			dr["mall_id"] = "OWN_SITE";
			dr["name"] = "うぇぶてすと";
			dr["name1"] = "うぇぶ";
			dr["name2"] = "てすと";
			dr["name_kana"] = "うぇぶてすと";
			dr["name_kana1"] = "うぇぶ";
			dr["name_kana2"] = "てすと";
			dr["nick_name"] = "うぇぶてす";
			dr["mail_addr"] = "f.nagaki@w2solution.co.jp";
			dr["mail_addr2"] = "";
			dr["zip"] = "105-0004";
			dr["zip1"] = "105";
			dr["zip2"] = "0004";
			dr["addr"] = "東京都港区新橋１－８－３住友新橋ビル７Ｆ";
			dr["addr1"] = "東京都";
			dr["addr2"] = "港区新橋";
			dr["addr3"] = "１－８－３";
			dr["addr4"] = "住友新橋ビル７Ｆ";
			dr["tel1"] = "0000-00-0000";
			dr["tel1_1"] = "0000";
			dr["tel1_2"] = "00";
			dr["tel1_3"] = "0000";
			dr["sex"] = "MALE";
			dr["birth"] = Convert.ToDateTime("1980/02/17");
			dr["birth_year"] = "1980";
			dr["birth_month"] = "2";
			dr["birth_day"] = "17";
			dr["company_name"] = "企業テスト1";
			dr["company_post_name"] = "企業部署1";
			dr["advcode_first"] = "";
			dr["attribute1"] = "ic0001";
			dr["login_id"] = loginid;
			dr["password"] = "pass123456";
			dr["question"] = "質問1";
			dr["answer"] = "答え1";
			dr["career_id"] = "";
			dr["mobile_uid"] = "";
			dr["remote_addr"] = "127.0.0.1";
			dr["mail_flg"] = "ON";
			dr["user_memo"] = "特記事項";
			dr["del_flg"] = "0";
			dr["last_changed"] = "webservice";
			dr["member_rank_id"] = "";

			dt.Rows.Add(dr);

			return dt;

		}


		/// <summary>
		/// テスト用の更新用ユーザーデータ作成
		/// </summary>
		/// <returns></returns>
		private DataTable CreateUpdateUserData(string userID)
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
			dt.Columns.Add("nick_name");
			dt.Columns.Add("mail_addr");
			dt.Columns.Add("mail_addr2");
			dt.Columns.Add("zip");
			dt.Columns.Add("zip1");
			dt.Columns.Add("zip2");
			dt.Columns.Add("addr");
			dt.Columns.Add("addr1");
			dt.Columns.Add("addr2");
			dt.Columns.Add("addr3");
			dt.Columns.Add("addr4");
			dt.Columns.Add("tel1");
			dt.Columns.Add("tel1_1");
			dt.Columns.Add("tel1_2");
			dt.Columns.Add("tel1_3");
			dt.Columns.Add("sex");
			dt.Columns.Add("birth");
			dt.Columns.Add("birth_year");
			dt.Columns.Add("birth_month");
			dt.Columns.Add("birth_day");
			dt.Columns.Add("company_name");
			dt.Columns.Add("company_post_name");
			dt.Columns.Add("advcode_first");
			dt.Columns.Add("attribute1");
			dt.Columns.Add("login_id");
			dt.Columns.Add("password");
			dt.Columns.Add("question");
			dt.Columns.Add("answer");
			dt.Columns.Add("career_id");
			dt.Columns.Add("mobile_uid");
			dt.Columns.Add("remote_addr");
			dt.Columns.Add("mail_flg");
			dt.Columns.Add("user_memo");
			dt.Columns.Add("del_flg");
			dt.Columns.Add("last_changed");
			dt.Columns.Add("member_rank_id");

			DataRow dr = dt.NewRow();

			dr["user_id"] = userID;
			dr["user_kbn"] = "PC_USER";
			dr["mall_id"] = "OWN_SITE";
			dr["name"] = "うぶてと";
			dr["name1"] = "うぶ";
			dr["name2"] = "てと";
			dr["name_kana"] = "うぶてと";
			dr["name_kana1"] = "うぶ";
			dr["name_kana2"] = "てと";
			dr["nick_name"] = "うぶす";
			dr["mail_addr"] = "f.nagaki@w2solution.co.jp";
			dr["mail_addr2"] = "";
			dr["zip"] = "105-0004";
			dr["zip1"] = "105";
			dr["zip2"] = "0004";
			dr["addr"] = "東京都港区新橋１－８－３住友新橋ビル７Ｆ";
			dr["addr1"] = "東京都";
			dr["addr2"] = "港区新橋";
			dr["addr3"] = "１－８－３";
			dr["addr4"] = "住友新橋ビル７Ｆ";
			dr["tel1"] = "1111-11-1111";
			dr["tel1_1"] = "1111";
			dr["tel1_2"] = "11";
			dr["tel1_3"] = "1111";
			dr["sex"] = "MALE";
			dr["birth"] = Convert.ToDateTime("1980/02/17");
			dr["birth_year"] = "1980";
			dr["birth_month"] = "2";
			dr["birth_day"] = "17";
			dr["company_name"] = "企業テスト1";
			dr["company_post_name"] = "企業部署1";
			dr["advcode_first"] = "";
			dr["attribute1"] = "ic0001";
			dr["login_id"] = "testlogin";
			dr["password"] = "pass123456";
			dr["question"] = "質問1";
			dr["answer"] = "答え1";
			dr["career_id"] = "";
			dr["mobile_uid"] = "";
			dr["remote_addr"] = "127.0.0.1";
			dr["mail_flg"] = "ON";
			dr["user_memo"] = "特記事項編集したよ";
			dr["del_flg"] = "0";
			dr["last_changed"] = "webservice";
			dr["member_rank_id"] = "";

			dt.Rows.Add(dr);

			return dt;

		}

		/// <summary>
		/// 処理区分用データテーブル作成
		/// </summary>
		/// <param name="procType">設定する処理区分の値</param>
		/// <returns>作成した処理区分用データテーブル</returns>
		private DataTable CreateProcTable(string procType)
		{
			DataTable dt = new DataTable("ProcTable");
			dt.Columns.Add("ProcType");

			DataRow dr = dt.NewRow();
			dr[0] = procType;

			dt.Rows.Add(dr);

			return dt;
		}

		#endregion

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

		protected void Button3_Click(object sender, EventArgs e)
		{
			DataSet ds = new DataSet();

			ds.Tables.Add(CreateProcTable(DropDownList1.SelectedValue.ToString()));

			switch (DropDownList1.SelectedValue.ToString())
			{
				case "UserRegist":
					//ds.Tables.Add(CreateInsertUserData(txtUserID.Text));
					break;

				case "UserModify":
					//ds.Tables.Add(CreateUpdateUserData(txtUserID.Text));
					break;
				case "UserDelete":
					//ds.Tables.Add(CreateUpdateUserData(txtUserID.Text));
					break;
			}

			DataSet rtnds = null;

			//会員情報連携
			using (websrv.Iw2ServiceClient sc = new websrv.Iw2ServiceClient())
			{
				rtnds = sc.UserSyncExecute(ds);
			}

			//戻り結果をラベルに
			StringBuilder sb = new StringBuilder();

			foreach (DataTable dt in rtnds.Tables)
			{
				sb.AppendLine("テーブル名[" + dt.TableName + "]");
				//データ行カウンタ
				int rCnt = 1;

				foreach (DataRow dr in dt.Rows)
				{
					sb.AppendLine(rCnt.ToString() + "行目__");
					foreach (DataColumn dc in dt.Columns)
					{
						//項目毎に出力
						sb.AppendLine(dc.ColumnName + ":" + ConvNullToEmpty(dr[dc.ColumnName]) + ";");
					}
					sb.AppendLine("__" + rCnt.ToString() + "行目End");
					rCnt++;
				}

			}

			lblRtDs.Text = sb.ToString();
		}

		protected void Button4_Click(object sender, EventArgs e)
		{
			DataSet ds = new DataSet();

			ds.Tables.Add(CreateProcTable(DropDownList1.SelectedValue.ToString()));

			switch (DropDownList1.SelectedValue.ToString())
			{
				case "UserRegist":
					ds.Tables.Add(CreateInsertUserData(txtUserID.Text));
					break;

				case "UserModify":
					ds.Tables.Add(CreateUpdateUserData(txtUserID.Text));
					break;
				case "UserDelete":
					ds.Tables.Add(CreateUpdateUserData(txtUserID.Text));
					break;
			}

			DataSet rtnds = null;

			//会員情報連携
			using (Test_W2_WebSrv.Iw2ServiceClient sc = new Test_W2_WebSrv.Iw2ServiceClient())
			{
				rtnds = sc.UserSyncExecute(ds);
			}

			//戻り結果をラベルに
			StringBuilder sb = new StringBuilder();

			foreach (DataTable dt in rtnds.Tables)
			{
				sb.AppendLine("テーブル名[" + dt.TableName + "]");
				//データ行カウンタ
				int rCnt = 1;

				foreach (DataRow dr in dt.Rows)
				{
					sb.AppendLine(rCnt.ToString() + "行目__");
					foreach (DataColumn dc in dt.Columns)
					{
						//項目毎に出力
						sb.AppendLine(dc.ColumnName + ":" + ConvNullToEmpty(dr[dc.ColumnName]) + ";");
					}
					sb.AppendLine("__" + rCnt.ToString() + "行目End");
					rCnt++;
				}

			}

			lblRtDs.Text = sb.ToString();
		}
	}
}