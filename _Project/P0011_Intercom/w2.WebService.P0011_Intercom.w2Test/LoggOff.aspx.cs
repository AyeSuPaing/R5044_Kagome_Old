using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.Crypto;

namespace w2.Plugin.P0011_Intercom.Webservice.w2Test
{
	public partial class LoggOff : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Request.QueryString != null)
			{
				w2Crypt cry = new w2Crypt();

				foreach (string key in Request.QueryString.AllKeys)
				{
					Response.Write(key + ":" 
						+ Request.QueryString[key] + ":"
						+ cry.Decrypt(Request.QueryString[key], "u.%CR@iFw8xG/-y3#7u9pAl+NLdYvhUCCW1eSpxG!VSnT?_hi#ONnDlqEy.HMAFFyI81a%311t*0AMYzZ#4s!tv=AsU%G891u#Ho-YNBRQO=.cqPVvgxod1ntfF$iq$K") + "<br>");
				}
			}
		}

		protected void Button1_Click(object sender, EventArgs e)
		{
			w2Crypt cry = new w2Crypt();

			Response.Redirect(cry.Decrypt(Request.QueryString["bu"], "u.%CR@iFw8xG/-y3#7u9pAl+NLdYvhUCCW1eSpxG!VSnT?_hi#ONnDlqEy.HMAFFyI81a%311t*0AMYzZ#4s!tv=AsU%G891u#Ho-YNBRQO=.cqPVvgxod1ntfF$iq$K"));
		}
	}
}