using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace w2.Plugin.P0011_Intercom.Webservice.w2Test
{
	public partial class IntercomLogOff : System.Web.UI.Page
	{

		//暗号化パスワード
		private const string ENC_PASS = "eWP%DX@BUEwoKun%b=cF.1$OEevWHphNUdt6MHnSA7MDw@V*y9x5!KRhD-+f?a.K3S$ka$7P3ls.a+h49bUlv5dk2ZwVWzmNw.4A0mCw5XnNVd7NHi=DgNXkg/d_VD?f";
		//NGの場合
		//private const string ENC_PASS = "eWP%DX@BUEwoKun%b=cF.1$OEevWHphNUdt6MHnSA7MDw@V*y9x5!KRhD-+f?a.K3S$ka$7P3ls.a+h49bUlv5dk2ZwVWzmNw.4A0mCw5XnNVd7NHi=DgNXkg/d_VD?";

		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void Button1_Click(object sender, EventArgs e)
		{
			//暗号化して
			w2.Crypto.w2Crypt cry = new Crypto.w2Crypt();

			string encnurl = cry.Encrypt("http://localhost/Solutions/w2.Plugin.P0011_Intercom.Webservice.w2Test/IntercomLogin.aspx", ENC_PASS);
			string encproc = cry.Encrypt("2", ENC_PASS);

			Response.Redirect("https://localhost/Solutions/P0011_Intercomv5.3/Web/w2.Commerce.Front/Form/Entrance.aspx?proc=" + encproc
				+"&nurl=" + HttpUtility.UrlEncode(encnurl));
		}
	}
}