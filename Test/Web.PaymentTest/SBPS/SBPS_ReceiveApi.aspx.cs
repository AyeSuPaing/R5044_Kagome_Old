using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

public partial class SBPS_SBPS_ReceiveCreditAuthApi : System.Web.UI.Page
{

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
		if (Request.InputStream.Length == 0)
		{
			Response.Write("ストリームが取得出来ませんでした。（直アクセス？）");
			return;
		}

		try
		{
			ISBPSReceiver receiver = SBPSReceiverFactory.Create(Request);
			if (receiver != null)
			{
				string responseString = receiver.Receive();

				Response.Write(responseString);
			}
		}
		catch (Exception ex)
		{
			Response.Write("リクエストデータの解析に失敗しました。：" + ex.ToString());
		}
    }
}