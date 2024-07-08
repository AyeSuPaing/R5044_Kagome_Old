using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace w2.Plugin.P0011_Intercom.Webservice.w2Test
{
	public partial class Redirect1 : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void Button1_Click(object sender, EventArgs e)
		{
			Context.Response.Redirect("Redirect2.aspx");
		}

		protected void Button2_Click(object sender, EventArgs e)
		{
			//シングルサインオン
			Response.Redirect("https://localhost/R5044_Kagome.Develop/Web/w2.Commerce.Front/Form/Entrance.aspx?proc=0");
		}
	}
}