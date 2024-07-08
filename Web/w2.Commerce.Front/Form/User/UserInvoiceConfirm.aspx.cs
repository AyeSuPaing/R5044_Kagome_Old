/*
=========================================================================================================
  Module      : User Invoice Confirmation Screen Processing (UserInvoiceConfirm.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;
using w2.Domain.TwUserInvoice;
using w2.Domain.UpdateHistory.Helper;

public partial class Form_User_UserInvoiceConfirm : ProductPage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return true; } }	// ログイン必須
	/// <summary>マイページメニュー表示判定</summary>
	public override bool DispMyPageMenu { get { return true; } }	// マイページメニュー表示

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// HTTPS通信チェック（HTTPのとき、アドレス帳一覧画面へ）
		CheckHttps(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_INVOICE_LIST);

		if (!IsPostBack)
		{
			// 表示制御
			// 更新の場合
			if (this.TwUserInvoice.TwInvoiceNo != "0")
			{
				lbRegist.Visible = false;
				lbModify.Visible = true;
				
			}
			// 登録の場合
			else
			{
				lbRegist.Visible = true;
				lbModify.Visible = false;
			}
		}
	}

	/// <summary>
	/// 送信リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSend_Click(object sender, EventArgs e)
	{
		var twUserInvoiceModel = this.TwUserInvoice.CreateModel();
		var twUserInvoiceService = new TwUserInvoiceService();

		if (twUserInvoiceModel.TwInvoiceNo == 0)
		{
			twUserInvoiceService.Insert(twUserInvoiceModel,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.Insert);
		}
		else
		{
			twUserInvoiceService.Update(twUserInvoiceModel,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.Insert);
		}

		// アドレス帳一覧画面へ
		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_INVOICE_LIST);
	}

	/// <summary>
	/// 戻るリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbBack_Click(object sender, EventArgs e)
	{
		// ターゲットページ設定
		this.SessionParamTargetPage = Constants.PAGE_FRONT_USER_INVOICE_INPUT;

		// URLに配送番号をセットしてリダイレクト
		StringBuilder url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_USER_INVOICE_INPUT)
			.Append("?").Append(Constants.REQUEST_KEY_INVOICE_NO).Append("=").Append(this.TwUserInvoice.TwInvoiceNo);
		Response.Redirect(url.ToString());
	}

	/// <summary>Tw User Invoice Input</summary>
	protected TwUserInvoiceInput TwUserInvoice
	{
		get { return (TwUserInvoiceInput)Session[Constants.SESSION_KEY_PARAM]; }
	}
}
