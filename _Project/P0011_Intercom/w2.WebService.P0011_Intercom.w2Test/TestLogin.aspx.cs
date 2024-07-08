using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace w2.Plugin.P0011_Intercom.Webservice.w2Test
{
	public partial class TestLogin : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			DataSet ds = new DataSet();
			ds.Tables.Add(CreateUserIDTable("w2solnagaki", "1072671"));

			DataSet rtnds = null;

			//会員情報連携
			using (com.w2solution.tt4.w2Service sc = new com.w2solution.tt4.w2Service())
			{
				sc.Credentials = System.Net.CredentialCache.DefaultCredentials;
				sc.Credentials = new System.Net.NetworkCredential("w2User", "e9QAi=a4h1", "com.w2solution.tt4");

				rtnds = sc.CreateOnetimePassword(ds);
			}

			//戻りデータセットからワンタイムパスワード取り出し
			string onetimepass = rtnds.Tables["OnetimePasswordTable"].Rows[0]["OnetimePassword"].ToString();


			//暗号化して
			w2.Crypto.w2Crypt cry = new Crypto.w2Crypt();

			string encw2id = cry.Encrypt("N14", "u.%CR@iFw8xG/-y3#7u9pAl+NLdYvhUCCW1eSpxG!VSnT?_hi#ONnDlqEy.HMAFFyI81a%311t*0AMYzZ#4s!tv=AsU%G891u#Ho-YNBRQO=.cqPVvgxod1ntfF$iq$K");
			string encicid = cry.Encrypt("test0001", "u.%CR@iFw8xG/-y3#7u9pAl+NLdYvhUCCW1eSpxG!VSnT?_hi#ONnDlqEy.HMAFFyI81a%311t*0AMYzZ#4s!tv=AsU%G891u#Ho-YNBRQO=.cqPVvgxod1ntfF$iq$K");
			string encpass = cry.Encrypt(onetimepass, "u.%CR@iFw8xG/-y3#7u9pAl+NLdYvhUCCW1eSpxG!VSnT?_hi#ONnDlqEy.HMAFFyI81a%311t*0AMYzZ#4s!tv=AsU%G891u#Ho-YNBRQO=.cqPVvgxod1ntfF$iq$K");
			string encnurl = cry.Encrypt("http://www.google.co.jp/", "u.%CR@iFw8xG/-y3#7u9pAl+NLdYvhUCCW1eSpxG!VSnT?_hi#ONnDlqEy.HMAFFyI81a%311t*0AMYzZ#4s!tv=AsU%G891u#Ho-YNBRQO=.cqPVvgxod1ntfF$iq$K");
			string encproc = cry.Encrypt("0", "u.%CR@iFw8xG/-y3#7u9pAl+NLdYvhUCCW1eSpxG!VSnT?_hi#ONnDlqEy.HMAFFyI81a%311t*0AMYzZ#4s!tv=AsU%G891u#Ho-YNBRQO=.cqPVvgxod1ntfF$iq$K");

			TextBox1.Text = "https://tt4.w2solution.com/Test/P0011_Intercom/Form/Entrance.aspx?proc=" 
				+ HttpUtility.UrlEncode(encproc) +
				"&w2id=" + HttpUtility.UrlEncode(encw2id) +
				"&icid=" + HttpUtility.UrlEncode(encicid) +
				"&pass=" + HttpUtility.UrlEncode(encpass) +
				"&nurl=" + HttpUtility.UrlEncode(encnurl);

		}

		protected void Button1_Click(object sender, EventArgs e)
		{
			Response.Redirect(TextBox1.Text);
		}

		private DataTable CreateUserIDTable(string w2id, string icid)
		{

			DataTable userIDTable = new DataTable("UserTable");

			userIDTable.Columns.Add("UserID");
			userIDTable.Columns.Add("LinkedUserID");

			DataRow dr1 = userIDTable.NewRow();
			dr1["UserID"] = w2id;
			dr1["LinkedUserID"] = icid;

			userIDTable.Rows.Add(dr1);

			return userIDTable;
		}

		protected void Button2_Click(object sender, EventArgs e)
		{

			//暗号化して
			w2.Crypto.w2Crypt cry = new Crypto.w2Crypt();

			string encnurl = cry.Encrypt("http://www.google.co.jp/", "u.%CR@iFw8xG/-y3#7u9pAl+NLdYvhUCCW1eSpxG!VSnT?_hi#ONnDlqEy.HMAFFyI81a%311t*0AMYzZ#4s!tv=AsU%G891u#Ho-YNBRQO=.cqPVvgxod1ntfF$iq$K");
			string encproc = cry.Encrypt("2", "u.%CR@iFw8xG/-y3#7u9pAl+NLdYvhUCCW1eSpxG!VSnT?_hi#ONnDlqEy.HMAFFyI81a%311t*0AMYzZ#4s!tv=AsU%G891u#Ho-YNBRQO=.cqPVvgxod1ntfF$iq$K");

			TextBox2.Text = "https://tt4.w2solution.com/Test/P0011_Intercom/Form/Entrance.aspx?proc="
				+ HttpUtility.UrlEncode(encproc) +
					"&nurl=" + HttpUtility.UrlEncode(encnurl);

		}
	}
}