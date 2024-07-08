/*
=========================================================================================================
  Module      : エラーポイント蓄積アイコン処理(ErrorPointIcon.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Cs.ErrorPoint;
using w2.Common.Net.Mail;

public partial class Form_Top_ErrorPointIcon : System.Web.UI.UserControl
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// 何もしない
	}

	/// <summary>
	/// メールアドレス（「XXX &lt;a@b.c&gt;」形式でも可）を設定する。
	/// </summary>
	/// <param name="mailAddr">メールアドレス</param>
	public void SetMailAddr(string mailAddr)
	{
		string address = MailAddress.GetInstance(mailAddr).Address;
		if (string.IsNullOrEmpty(address))
		{
			this.ErrorPoint = 0;
			return;
		}
		CsMailErrorAddrService service = new CsMailErrorAddrService(new CsMailErrorAddrRepository());
		CsMailErrorAddrModel model = service.Get(address);
		
		this.ErrorPoint = (model == null) ? 0 : model.ErrorPoint;
	}

	/// <summary>エラーポイント</summary>
	protected int ErrorPoint
	{
		get { return (int?)ViewState["ErrorPoint"] ?? 0; }
		private set { ViewState["ErrorPoint"] = value; }
	}
}
