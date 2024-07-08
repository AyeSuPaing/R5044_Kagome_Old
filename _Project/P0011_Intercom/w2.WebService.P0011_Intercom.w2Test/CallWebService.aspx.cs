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
	public partial class CallWebService : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void Button1_Click(object sender, EventArgs e)
		{
			DataSet ds = new DataSet();

			ds.Tables.Add(CreateProcTable(DropDownList1.SelectedValue.ToString()));

			switch (DropDownList1.SelectedValue.ToString())
			{
				case "UserRegist":
					ds.Tables.Add(CreateUserData());
					break;

				case "UserModify":
					ds.Tables.Add(CreateUserData());
					break;
				case "UserDelete":
					ds.Tables.Add(CreateUserData());
					break;
			}

			DataSet rtnds = null;

			//会員情報連携
			using (localhost.w2Service sc = new localhost.w2Service())
			{
				sc.Credentials = System.Net.CredentialCache.DefaultCredentials;
				sc.Credentials = new System.Net.NetworkCredential("websrv", "websrv");
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

			Label22.Text = sb.ToString();
		}

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
		/// テスト用のユーザーデータ作成
		/// </summary>
		/// <returns></returns>
		private DataTable CreateUserData()
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

			dr["user_id"] = txw2id.Text;
			dr["user_kbn"] = "PC_USER";
			dr["mall_id"] = "OWN_SITE";
			dr["name1"] = txSei.Text;
			dr["name2"] = txMei.Text;
			dr["name_kana1"] = txSeiKana.Text;
			dr["name_kana2"] = txMeiKana.Text;
			dr["mail_addr"] = txMail.Text;
			dr["zip1"] = txzip1.Text;
			dr["zip2"] = txzip2.Text;
			dr["addr1"] = txadr1.Text;
			dr["addr2"] = txadr2.Text;
			dr["addr3"] = txadr3.Text;
			dr["addr4"] = txadr4.Text;
			dr["tel1_1"] = txtell1.Text;
			dr["tel1_2"] = txtell2.Text;
			dr["tel1_3"] = txtell3.Text;
			dr["sex"] = dpSex.SelectedValue;
			dr["birth_year"] = txNen.Text;
			dr["birth_month"] = txTuki.Text;
			dr["birth_day"] = txHi.Text;
			dr["company_name"] = txkigyo.Text;
			dr["company_post_name"] = txbusyo.Text;
			dr["attribute1"] = txicid.Text;
			dr["login_id"] = txLogin.Text;
			dr["password"] = txPass.Text;
			dr["mail_flg"] = dpMailFlg.SelectedValue;
			dr["del_flg"] = dpDelFlg.Text;
			dr["last_changed"] = txkousin.Text;
		
			dt.Rows.Add(dr);

			return dt;

		}


		/// <summary>
		/// テスト地
		/// </summary>
		/// <returns></returns>
		private void SetTestVal()
		{
			
			txw2id.Text  = "";
			txSei.Text  = "永木";
			txMei.Text  = "文康";
			txSeiKana.Text  = "うぇぶ";
			txMeiKana.Text  = "てすと";
			txMail.Text   = "f.nagaki@w2solution.co.jp";
			txzip1.Text  = "105";
			txzip2.Text  = "0004";
			txadr1.Text  = "東京都";
			txadr2.Text = "港区新橋";
			txadr3.Text = "１－８－３";
			txadr4.Text = "住友新橋ビル７Ｆ";
			txtell1.Text  = "1111";
			txtell2.Text = "11";
			txtell3.Text = "1111";
			dpSex.SelectedValue  = "MALE";
			txNen.Text  = "1980";
			txTuki.Text  = "2";
			txHi.Text  = "17";
			txkigyo.Text  = "企業テスト1";
			txbusyo.Text  = "企業部署1";
			txicid.Text   = "ic0001";
			txLogin.Text  = "testlogin";
			txPass.Text  = "pass123456";
			dpMailFlg.SelectedValue  = "ON";
			dpDelFlg.SelectedValue  = "0";
			txkousin.Text  = "webservice";
			
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

		protected void Button2_Click(object sender, EventArgs e)
		{
			SetTestVal();
		}

		#endregion

		protected void Button3_Click(object sender, EventArgs e)
		{
			DataSet ds = new DataSet();

			ds.Tables.Add(CreateProcTable(DropDownList1.SelectedValue.ToString()));

			switch (DropDownList1.SelectedValue.ToString())
			{
				case "UserRegist":
					ds.Tables.Add(CreateUserData());
					break;

				case "UserModify":
					ds.Tables.Add(CreateUserData());
					break;
				case "UserDelete":
					ds.Tables.Add(CreateUserData());
					break;
			}

			DataSet rtnds = null;

			//会員情報連携
			using (com.w2solution.tt4.w2Service sc = new com.w2solution.tt4.w2Service())
			{
				sc.Credentials = System.Net.CredentialCache.DefaultCredentials;
				sc.Credentials = new System.Net.NetworkCredential("w2User", "e9QAi=a4h1", "com.w2solution.tt4");

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

			Label22.Text = sb.ToString();
		}

		protected void Button4_Click(object sender, EventArgs e)
		{
			DataSet ds = new DataSet();

			ds.Tables.Add(CreateProcTable(DropDownList1.SelectedValue.ToString()));

			switch (DropDownList1.SelectedValue.ToString())
			{
				case "UserRegist":
					ds.Tables.Add(CreateUserData());
					break;

				case "UserModify":
					ds.Tables.Add(CreateUserData());
					break;
				case "UserDelete":
					ds.Tables.Add(CreateUserData());
					break;
			}

			DataSet rtnds = null;

			//会員情報連携
			using (jp.co.intercom.reddragon_UserSync.w2iUserSyncService  sc = new jp.co.intercom.reddragon_UserSync.w2iUserSyncService())
			{
				//sc.Credentials = System.Net.CredentialCache.DefaultCredentials;
				//sc.Credentials = new System.Net.NetworkCredential("w2User", "e9QAi=a4h1", "com.w2solution.tt4");

				rtnds = sc.UserSync(ds);
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

			Label22.Text = sb.ToString();
		}
	}
}