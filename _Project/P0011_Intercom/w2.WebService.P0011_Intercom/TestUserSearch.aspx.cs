using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Text;
using w2.Plugin.P0011_Intercom.WebService.Util;

namespace w2.Plugin.P0011_Intercom.WebService
{
	/// <summary>
	/// テスト用の会員情報検索画面
	/// </summary>
	public partial class TestUserSearch : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void Button1_Click(object sender, EventArgs e)
		{
			lblref.Text = "";

			string connStr = WebConfigurationManager.ConnectionStrings["w2Database"].ToString();

			string loginid = TextBox1.Text;

			if (loginid == "")
			{
				lblref.Text = "検索条件となるログインIDを入力してください。";
				return;
			}

			string sqlStr = " SELECT user_id as w2ユーザID,attribute1 as intercomユーザID,login_id as ログインID,password as パスワード " +
						" ,company_name as 会社名,company_post_name as 部署名,name1 as 氏名_姓,name2 as 氏名_名,name_kana1 as 氏名かな_姓 " +
						" ,name_kana2 as 氏名かな_名,zip1 as 郵便番号1,zip2 as 郵便番号2,addr1 as 住所1,addr2 as 住所2,addr3 as 住所3 " +
						" ,addr4 as 住所4,tel1_1 as  電話番号1,tel1_2 as 電話番号2,tel1_3 as 電話番号3,mail_addr as メールアドレス " +
						" ,birth_year as 生年月日_年,birth_month as 生年月日_月,birth_day as 生年月日_日,sex as 性別,del_flg as 削除フラグ " +
						" ,mail_flg as メール配信フラグ,last_changed as 最終更新者 " +
						" FROM w2_User " +
						" where login_id = @login_id ";

			DataTable dt = new DataTable();

			using (SqlConnection conn = new SqlConnection(connStr))
			{
				//SqlCommand
				SqlCommand selCmd = new SqlCommand(sqlStr, conn);

				//SettingParam
				selCmd.Parameters.AddWithValue("@login_id", loginid);

				SqlDataAdapter adp = new SqlDataAdapter(selCmd);


				adp.Fill(dt);

			}

			if (dt.Rows.Count > 0)
			{
				StringBuilder sb = new StringBuilder();

				foreach (DataRow dr in dt.Rows)
				{
					foreach (DataColumn dc in dt.Columns)
					{
						//項目毎に出力
						if (dc.ColumnName == "パスワード")
						{
							byte[] key = Convert.FromBase64String(WebConfigurationManager.AppSettings["key"].ToString());
							byte[] iv = Convert.FromBase64String(WebConfigurationManager.AppSettings["iv"].ToString());

							RijndaelCrypto cry = new RijndaelCrypto(key, iv);

							sb.AppendLine(dc.ColumnName + ":" + cry.Decrypt(ConvNullToEmpty(dr[dc.ColumnName])) + "<BR>");
						}
						else
						{
							sb.AppendLine(dc.ColumnName + ":" + ConvNullToEmpty(dr[dc.ColumnName]) + "<BR>");
						}
					}

					sb.AppendLine("<br>");
				}

				lblref.Text = sb.ToString();
			}
			else
			{
				lblref.Text = "該当する情報はありません。";
			}

		}

		protected void btnw2_Click(object sender, EventArgs e)
		{
			lblref.Text = "";

			string connStr = WebConfigurationManager.ConnectionStrings["w2Database"].ToString();

			string user_id = txtw2.Text;

			if (user_id == "")
			{
				lblref.Text = "検索条件となるw2ユーザIDを入力してください。";
				return;
			}

			string sqlStr = " SELECT user_id as w2ユーザID,attribute1 as intercomユーザID,login_id as ログインID,password as パスワード " +
						" ,company_name as 会社名,company_post_name as 部署名,name1 as 氏名_姓,name2 as 氏名_名,name_kana1 as 氏名かな_姓 " +
						" ,name_kana2 as 氏名かな_名,zip1 as 郵便番号1,zip2 as 郵便番号2,addr1 as 住所1,addr2 as 住所2,addr3 as 住所3 " +
						" ,addr4 as 住所4,tel1_1 as  電話番号1,tel1_2 as 電話番号2,tel1_3 as 電話番号3,mail_addr as メールアドレス " +
						" ,birth_year as 生年月日_年,birth_month as 生年月日_月,birth_day as 生年月日_日,sex as 性別,del_flg as 削除フラグ " +
						" ,mail_flg as メール配信フラグ,last_changed as 最終更新者 " +
						" FROM w2_User " +
						" where user_id = @user_id ";

			DataTable dt = new DataTable();

			using (SqlConnection conn = new SqlConnection(connStr))
			{
				//SqlCommand
				SqlCommand selCmd = new SqlCommand(sqlStr, conn);

				//SettingParam
				selCmd.Parameters.AddWithValue("@user_id", user_id);

				SqlDataAdapter adp = new SqlDataAdapter(selCmd);


				adp.Fill(dt);

			}

			if (dt.Rows.Count > 0)
			{
				StringBuilder sb = new StringBuilder();

				foreach (DataRow dr in dt.Rows)
				{
					foreach (DataColumn dc in dt.Columns)
					{
						//項目毎に出力
						if (dc.ColumnName == "パスワード")
						{
							byte[] key = Convert.FromBase64String(WebConfigurationManager.AppSettings["key"].ToString());
							byte[] iv = Convert.FromBase64String(WebConfigurationManager.AppSettings["iv"].ToString());

							RijndaelCrypto cry = new RijndaelCrypto(key, iv);

							sb.AppendLine(dc.ColumnName + ":" + cry.Decrypt(ConvNullToEmpty(dr[dc.ColumnName])) + "<BR>");
						}
						else
						{
							sb.AppendLine(dc.ColumnName + ":" + ConvNullToEmpty(dr[dc.ColumnName]) + "<BR>");
						}
					}

					sb.AppendLine("<br>");
				}

				lblref.Text = sb.ToString();
			}
			else
			{
				lblref.Text = "該当する情報はありません。";
			}
		}

		protected void btnic_Click(object sender, EventArgs e)
		{
			lblref.Text = "";

			string connStr = WebConfigurationManager.ConnectionStrings["w2Database"].ToString();

			string attribute1 = txtic.Text;

			if (attribute1 == "")
			{
				lblref.Text = "検索条件となるw2ユーザIDを入力してください。";
				return;
			}

			string sqlStr = " SELECT user_id as w2ユーザID,attribute1 as intercomユーザID,login_id as ログインID,password as パスワード " +
						" ,company_name as 会社名,company_post_name as 部署名,name1 as 氏名_姓,name2 as 氏名_名,name_kana1 as 氏名かな_姓 " +
						" ,name_kana2 as 氏名かな_名,zip1 as 郵便番号1,zip2 as 郵便番号2,addr1 as 住所1,addr2 as 住所2,addr3 as 住所3 " +
						" ,addr4 as 住所4,tel1_1 as  電話番号1,tel1_2 as 電話番号2,tel1_3 as 電話番号3,mail_addr as メールアドレス " +
						" ,birth_year as 生年月日_年,birth_month as 生年月日_月,birth_day as 生年月日_日,sex as 性別,del_flg as 削除フラグ " +
						" ,mail_flg as メール配信フラグ,last_changed as 最終更新者 " +
						" FROM w2_User " +
						" where attribute1 = @attribute1 ";

			DataTable dt = new DataTable();

			using (SqlConnection conn = new SqlConnection(connStr))
			{
				//SqlCommand
				SqlCommand selCmd = new SqlCommand(sqlStr, conn);

				//SettingParam
				selCmd.Parameters.AddWithValue("@attribute1", attribute1);

				SqlDataAdapter adp = new SqlDataAdapter(selCmd);


				adp.Fill(dt);

			}

			if (dt.Rows.Count > 0)
			{
				StringBuilder sb = new StringBuilder();

				foreach (DataRow dr in dt.Rows)
				{
					foreach (DataColumn dc in dt.Columns)
					{

						//項目毎に出力
						if (dc.ColumnName == "パスワード")
						{
							byte[] key = Convert.FromBase64String(WebConfigurationManager.AppSettings["key"].ToString());
							byte[] iv = Convert.FromBase64String(WebConfigurationManager.AppSettings["iv"].ToString());

							RijndaelCrypto cry = new RijndaelCrypto(key, iv);

							sb.AppendLine(dc.ColumnName + ":" + cry.Decrypt(ConvNullToEmpty(dr[dc.ColumnName])) + "<BR>");
						}
						else
						{
							sb.AppendLine(dc.ColumnName + ":" + ConvNullToEmpty(dr[dc.ColumnName]) + "<BR>");
						}
					}

					sb.AppendLine("<br>");
				}

				lblref.Text = sb.ToString();
			}
			else
			{
				lblref.Text = "該当する情報はありません。";
			}

		}

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
	}

}