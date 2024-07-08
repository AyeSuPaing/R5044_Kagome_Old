/*
=========================================================================================================
  Module      : ユーザクレジットカード確認画面処理(UserCreditCardConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Text;
using System.Web;
using w2.Domain.UpdateHistory.Helper;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.YamatoKwc;
using w2.App.Common.SendMail;
using w2.App.Common.User;
using w2.App.Common.Util;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain.UserCreditCard;

public partial class Form_User_UserCreditCardConfirm : BasePage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return true; } }	// ログイン必須
	/// <summary>マイページメニュー表示判定</summary>
	public override bool DispMyPageMenu { get { return true; } }	// マイページメニュー表示

	#region ラップ済コントロール宣言
	protected WrappedHtmlGenericControl WdivRegisterDisplay { get { return GetWrappedControl<WrappedHtmlGenericControl>("divRegisterDisplay"); } }
	protected WrappedHtmlGenericControl WdivUpdateDisplay { get { return GetWrappedControl<WrappedHtmlGenericControl>("divUpdateDisplay"); } }
	protected WrappedHtmlGenericControl WdivCreditCardNoToken { get { return GetWrappedControl<WrappedHtmlGenericControl>("divCreditCardNoToken"); } }
	protected WrappedLiteral WlCardDispName { get { return GetWrappedControl<WrappedLiteral>("lCardDispName"); } }
	protected WrappedLiteral WlCardCompanyName { get { return GetWrappedControl<WrappedLiteral>("lCardCompanyName"); } }
	protected WrappedLiteral WlLastFourDigit { get { return GetWrappedControl<WrappedLiteral>("lLastFourDigit"); } }
	protected WrappedLiteral WlExpirationMonth { get { return GetWrappedControl<WrappedLiteral>("lExpirationMonth"); } }
	protected WrappedLiteral WlExpirationYear { get { return GetWrappedControl<WrappedLiteral>("lExpirationYear"); } }
	protected WrappedLiteral WlAuthorName { get { return GetWrappedControl<WrappedLiteral>("lAuthorName"); } }
	protected WrappedLinkButton WlbSend { get { return GetWrappedControl<WrappedLinkButton>("lbSend"); } }
	protected WrappedLinkButton WlbUpdate { get { return GetWrappedControl<WrappedLinkButton>("lbUpdate"); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// HTTPS通信チェック（HTTPのとき、クレジットカード一覧画面へ）
		//------------------------------------------------------
		CheckHttps(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_CREDITCARD_LIST);

		//------------------------------------------------------
		// 画面遷移確認
		//------------------------------------------------------
		// 正しい画面遷移以外で確認画面に来た場合、クレジットカード一覧画面へ
		if (this.SessionParamTargetPage != Constants.PAGE_FRONT_USER_CREDITCARD_CONFIRM)
		{
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_CREDITCARD_LIST);
		}

		// クレジットカード入力情報取得
		var userCreditCardInput = ((UserCreditCardInput)Session[Constants.SESSION_KEY_PARAM]);

		// トークンがあった場合
		if (userCreditCardInput.CreditToken != null)
		{
			// トークン有効期限切れチェック
			CheckTokenExpiredAndRedirectPage(userCreditCardInput);
			this.WdivUpdateDisplay.Visible = this.WlbUpdate.Visible = false;
		}
		// 決済区分がZcom、カード枝番が新規登録（null）の場合
		else if ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Zcom)
				&& (userCreditCardInput.BranchNo == null))
		{
			this.WdivUpdateDisplay.Visible = this.WlbUpdate.Visible = false;
		}
		else
		{
			this.WdivCreditCardNoToken.Visible = this.WdivRegisterDisplay.Visible = this.WlbSend.Visible = false;
		}

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// 入力情報設定
			//------------------------------------------------------
			WlCardDispName.Text = WebSanitizer.HtmlEncode(userCreditCardInput.CardDispName);
			WlCardCompanyName.Text = WebSanitizer.HtmlEncode(OrderCommon.GetCreditCardCompanyName(userCreditCardInput.CompanyCode));
			WlLastFourDigit.Text = GetCreditCardDispString(userCreditCardInput.CardNo);
			WlExpirationMonth.Text = WebSanitizer.HtmlEncode(userCreditCardInput.ExpirationMonth);
			WlExpirationYear.Text = WebSanitizer.HtmlEncode(userCreditCardInput.ExpirationYear);
			WlAuthorName.Text = WebSanitizer.HtmlEncode(userCreditCardInput.AuthorName);

			// ※（デザインマージを少なくするために残す）
			this.UserCreditCard = new Hashtable
			{
				{CartPayment.FIELD_REGIST_CREDIT_CARD_NAME, WlCardDispName.Text},
				{CartPayment.FIELD_CREDIT_EXPIRE_MONTH, WlExpirationMonth.Text},
				{CartPayment.FIELD_CREDIT_EXPIRE_YEAR, WlExpirationYear.Text},
				{CartPayment.FIELD_CREDIT_AUTHOR_NAME, userCreditCardInput.AuthorName},
				{CartPayment.FIELD_CREDIT_COMPANY, WlCardCompanyName.Text},
			};
			this.DataBind();
		}
	}

	/// <summary>
	/// トークン有効期限切れをチェックし、切れていればエラーページへ遷移する、そもそもトークンがない場合は入力ページへ遷移する
	/// </summary>
	/// <param name="userCreditCardInput">ユーザークレジットカード入力クラス</param>
	private void CheckTokenExpiredAndRedirectPage(UserCreditCardInput userCreditCardInput)
	{
		if (OrderCommon.CreditTokenUse == false) return;

		if (userCreditCardInput.CreditToken == null)
		{
			HttpContext.Current.Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_CREDITCARD_INPUT);
		}
		if (userCreditCardInput.IsTokenExpired())
		{
			HttpContext.Current.Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_CREDIT_TOKEN_EXPIRED);
			HttpContext.Current.Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}
	}

	/// <summary>
	/// クレジットカード表示文字列（「XXXXXXXXXXXX1234」）取得 ※（デザインマージを少なくするために残す）
	/// </summary>
	/// <returns>クレジットカード表示文字列</returns>
	public string GetCreditCardDispString()
	{
		var result = GetCreditCardDispString((string)this.UserCreditCard[CartPayment.FIELD_CREDIT_CARD_NO_1] + (string)this.UserCreditCard[CartPayment.FIELD_CREDIT_CARD_NO_2] + (string)this.UserCreditCard[CartPayment.FIELD_CREDIT_CARD_NO_3] + (string)this.UserCreditCard[CartPayment.FIELD_CREDIT_CARD_NO_4]);
		return result;
	}
	/// <summary>
	/// クレジットカード表示文字列（「XXXXXXXXXXXX1234」）取得
	/// </summary>
	/// <param name="cardNo">カード番号</param>
	/// <returns>クレジットカード表示文字列</returns>
	public string GetCreditCardDispString(string cardNo)
	{
		string strCardNoTmp = "    " + cardNo;

		return strCardNoTmp.Substring(strCardNoTmp.Length - 4, 4);
	}

	/// <summary>
	/// 送信リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSend_Click(object sender, EventArgs e)
	{
		// クレジット与信ロックされていればエラーページへ
		if (CreditAuthAttackBlocker.Instance.IsLocked(this.LoginUserId, Request.UserHostAddress))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CREDIT_AUTH_LOCK);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		// クレジットカード登録（更新履歴とともに）
		var userCreditCardInput = (UserCreditCardInput)Session[Constants.SESSION_KEY_PARAM];
		var result = new UserCreditCardRegister().Exec(
			userCreditCardInput,
			this.IsSmartPhone ? SiteKbn.SmartPhone : SiteKbn.Pc,
			true,
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.Insert);

		if (result.Success)
		{
			// 登録完了メール送信
			SendMailCommon.SendModifyCreditCardMail(this.LoginUserId);

			// ターゲットページ設定を消去
			this.SessionParamTargetPage = null;

			// クレジット一覧画面へ
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_CREDITCARD_LIST);
		}
		else
		{
			// トークン削除
			userCreditCardInput.CreditToken = null;

			// クレジット与信試行カウント-1
			CreditAuthAttackBlocker.Instance.DecreasePossibleTrialCount(this.LoginUserId, Request.UserHostAddress);

			AppLogger.WriteInfo(result.ErrorMessage);

			// カード情報入力画面で更新画面が表示されないように枝番を破棄する
			userCreditCardInput.BranchNo = null;

			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CREDITCARD_AUTH_ERROR);

			// エラー画面から戻る際、再入力しやすいように入力画面に遷移させる
			var errorPageUrl = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			errorPageUrl.AddParam(Constants.REQUEST_KEY_BACK_URL, Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_CREDITCARD_INPUT);

			SessionManager.NextPageForCheck = Constants.PAGE_FRONT_USER_CREDITCARD_INPUT;
			this.SessionParamTargetPage = Constants.PAGE_FRONT_USER_CREDITCARD_INPUT;

			Response.Redirect(errorPageUrl.CreateUrl());
		}
	}

	/// <summary>
	/// 更新クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbUpdate_Click(object sender, EventArgs e)
	{
		var updateCreditCard = ((UserCreditCardInput)Session[Constants.SESSION_KEY_PARAM]);

		var userCreditCard = new UserCreditCardModel()
		{
			UserId = updateCreditCard.UserId,
			BranchNo = int.Parse(updateCreditCard.BranchNo),
			CardDispName = updateCreditCard.CardDispName
		};

		new UserCreditCardService().UpdateCardDisplayName(
			userCreditCard,
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.Insert);
		
		// クレジット一覧画面へ
		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_CREDITCARD_LIST);
	}

	/// <summary>
	/// 戻るリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbBack_Click(object sender, EventArgs e)
	{
		// クレジットカード入力情報取得
		var userCreditCardInput = ((UserCreditCardInput)Session[Constants.SESSION_KEY_PARAM]);

		//------------------------------------------------------
		// ターゲットページ設定
		//------------------------------------------------------
		this.SessionParamTargetPage = Constants.PAGE_FRONT_USER_CREDITCARD_INPUT;

		if (userCreditCardInput.CreditToken != null)
		{
			//------------------------------------------------------
			// クレジットカード登録ページへ遷移
			//------------------------------------------------------
			StringBuilder sbUrl = new StringBuilder();
			sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_USER_CREDITCARD_INPUT);
			Response.Redirect(sbUrl.ToString());
		}

		// クレジットカード入力画面へ
		var url = new UrlCreator(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_CREDITCARD_INPUT)
			.AddParam(Constants.REQUEST_KEY_CREDITCARD_NO, userCreditCardInput.BranchNo)
			.CreateUrl();

		Response.Redirect(url);
	}

	/// <summary>ユーザクレジットカード（デザインマージを少なくするために残す）</summary>
	protected Hashtable UserCreditCard { get; private set; }
}