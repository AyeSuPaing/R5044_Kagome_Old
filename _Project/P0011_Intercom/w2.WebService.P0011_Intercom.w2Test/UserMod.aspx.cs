using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace w2.Plugin.P0011_Intercom.Webservice.w2Test
{
	public partial class UserMod : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			//foreach (KeyValuePair<string, string> que in Request.QueryString)
			//{
			//    Response.Write(que.Key + ":" + que.Value + "<br>");
			//}

			if (Request.QueryString != null)
			{
				foreach (string key in Request.QueryString.AllKeys)
				{
					Response.Write(key + ":" + Request.QueryString[key] + "<br>");
				}
			}
		}
	}
}