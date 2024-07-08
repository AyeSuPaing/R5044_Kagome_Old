/*
=========================================================================================================
  Module      : アドレス帳確認画面処理(UserShippingConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;
using w2.App.Common.Global;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.UserShipping;
using w2.App.Common.SendMail;

public partial class Form_User_UserShippingConfirm : ProductPage
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
		//------------------------------------------------------
		// HTTPS通信チェック（HTTPのとき、アドレス帳一覧画面へ）
		//------------------------------------------------------
		CheckHttps(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_SHIPPING_LIST);

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// 表示制御
			//------------------------------------------------------
			// 更新の場合
			if (this.UserShipping.ShippingNo != "0")
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
		var userShippingModel = this.UserShipping.CreateModel();

		if (userShippingModel.ShippingNo == 0)
		{
			new UserShippingService().Insert(
				userShippingModel,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.Insert);
		}
		else
		{
			new UserShippingService().Update(
				userShippingModel,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.Insert);
		}

		// メール送信
		SendMailCommon.SendModifyUserShippingMail(this.LoginUserId, userShippingModel);

		//------------------------------------------------------
		// アドレス帳一覧画面へ
		//------------------------------------------------------
		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_SHIPPING_LIST);
	}

	/// <summary>
	/// 戻るリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbBack_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// ターゲットページ設定
		//------------------------------------------------------
		this.SessionParamTargetPage = Constants.PAGE_FRONT_USER_SHIPPING_INPUT;

		//------------------------------------------------------
		// アドレス帳登録ページへ遷移
		//------------------------------------------------------
		// URLに配送番号をセットしてリダイレクト
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_USER_SHIPPING_INPUT);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_SHIPPING_NO).Append("=").Append(this.UserShipping.ShippingNo);
		Response.Redirect(sbUrl.ToString());
	}

	/// <summary>ユーザー配送先入力</summary>
	protected UserShippingInput UserShipping
	{
		get { return (UserShippingInput)Session[Constants.SESSION_KEY_PARAM]; }
	}
	/// <summary>ユーザーの配送先住所が日本か</summary>
	public bool IsShippingAddrJp
	{
		get { return GlobalAddressUtil.IsCountryJp(this.ShippingAddrCountryIsoCode); }
	}
	/// <summary>ユーザーの配送先住所国ISOコード</summary>
	public string ShippingAddrCountryIsoCode
	{
		get { return this.UserShipping.ShippingCountryIsoCode; }
	}
}
