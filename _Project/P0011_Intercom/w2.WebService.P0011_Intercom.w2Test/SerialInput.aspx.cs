using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace w2.Plugin.P0011_Intercom.Webservice.w2Test
{
	public partial class SerialInput : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Request.QueryString != null)
			{
				foreach (string key in Request.QueryString.AllKeys)
				{
					if (key == "pid")
					{
						lblpid.Text = Request.QueryString[key];
					}

					if (key == "vaid")
					{
						lblva.Text = Request.QueryString[key];
					}
					
					if (key == "ko")
					{
						lblko.Text = Request.QueryString[key];
					}
				}
			}
		}

		protected void Button1_Click(object sender, EventArgs e)
		{
			string sel1 = txtSeri1.Text;
			string sel2 = txtSeri2.Text;

			string qval = txtSeri1.Text + ":" + txtSeri2.Text;
						
			Response.Redirect("https://localhost/P0011_Intercom/Web/w2.Commerce.Front/Form/Entrance.aspx?proc=1" +
				"&pid=" + lblpid.Text +
				"&vaid=" + lblva.Text +
				"&ko=" + lblko.Text +
				"&sel=" + qval);


		}
	}
}