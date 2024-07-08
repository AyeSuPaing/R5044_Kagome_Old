using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace w2.Plugin.P0011_Intercom.Webservice.w2Test
{
	public partial class Redirect2 : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Context.Response.Redirect("Redirect3.aspx");
		}
	}
}