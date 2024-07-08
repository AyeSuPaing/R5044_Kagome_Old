using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace w2.Plugin.P0011_Intercom.Webservice.w2Test
{
	public partial class IntercomLogin : System.Web.UI.Page
	{
		//暗号化パスワード
		private const string ENC_PASS = "eWP%DX@BUEwoKun%b=cF.1$OEevWHphNUdt6MHnSA7MDw@V*y9x5!KRhD-+f?a.K3S$ka$7P3ls.a+h49bUlv5dk2ZwVWzmNw.4A0mCw5XnNVd7NHi=DgNXkg/d_VD?f";
		//NGの場合
		//private const string ENC_PASS = "eWP%DX@BUEwoKun%b=cF.1$OEevWHphNUdt6MHnSA7MDw@V*y9x5!KRhD-+f?a.K3S$ka$7P3ls.a+h49bUlv5dk2ZwVWzmNw.4A0mCw5XnNVd7NHi=DgNXkg/d_VD?";

		protected void Page_Load(object sender, EventArgs e)
		{
			txtw2id.Text = "N200003526";
			txticid.Text = "20110824131653282";
		}

		protected void Button1_Click(object sender, EventArgs e)
		{
			string w2id = txtw2id.Text;
			string icid = txticid.Text;

			DataSet ds = new DataSet();
			ds.Tables.Add(CreateUserIDTable(w2id, icid));

			DataSet rtnds = null;
			//会員情報連携
			using (localhost.w2Service sc = new localhost.w2Service())
			{
				sc.Credentials = System.Net.CredentialCache.DefaultCredentials;
				sc.Credentials = new System.Net.NetworkCredential("websrv", "websrv");
				rtnds = sc.CreateOnetimePassword(ds);
			}

			//戻りデータセットからワンタイムパスワード取り出し
			string onetimepass = rtnds.Tables["OnetimePasswordTable"].Rows[0]["OnetimePassword"].ToString();

			//暗号化して
			w2.Crypto.w2Crypt cry = new Crypto.w2Crypt();

			string encw2id = cry.Encrypt(w2id, ENC_PASS);
			string encicid = cry.Encrypt(icid, ENC_PASS);
			string encpass = cry.Encrypt(onetimepass, ENC_PASS);
			string encnurl = cry.Encrypt("http://localhost/Solutions/w2.Plugin.P0011_Intercom.Webservice.w2Test/DemoPage.aspx", ENC_PASS);
			string encproc = cry.Encrypt("0", ENC_PASS);
			

			//DEC情報を出力
			string decw2id = cry.Decrypt(encw2id, ENC_PASS);
			string decicid = cry.Decrypt(encicid, ENC_PASS);
			string decpassid = cry.Decrypt(encpass, ENC_PASS);
			string decnurlid = cry.Decrypt(encnurl, ENC_PASS);

			Response.Write(decw2id + "<br>");
			Response.Write(decicid + "<br>");
			Response.Write(decpassid + "<br>");
			Response.Write(decnurlid + "<br>");


			//Response.Redirect("https://localhost/Solutions/P0011_Intercomv5.3/Web/w2.Commerce.Front/Form/Entrance.aspx?proc=0" +
			//    "&w2id=" + encw2id +
			//    "&icid=" + encicid +
			//    "&pass=" + encpass  +
			//    "&nurl=" + encnurl
			//    );

			TextBox1.Text = "https://localhost/Solutions/P0011_Intercomv5.3/Web/w2.Commerce.Front/Form/Entrance.aspx?proc=" + encproc +
				"&w2id=" + HttpUtility.UrlEncode(encw2id) +
				"&icid=" + HttpUtility.UrlEncode(encicid) +
				"&pass=" + HttpUtility.UrlEncode(encpass) +
				"&nurl=" + HttpUtility.UrlEncode(encnurl);
				
		}

		private DataTable CreateUserIDTable(string w2id,string icid)
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
			Response.Redirect(TextBox1.Text);
		}
	}
}