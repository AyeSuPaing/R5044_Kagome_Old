
/*
=========================================================================================================
  Module      : ユーザクレジットカード確認画面処理(UserCreditCardConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;
using System.Web;
using System.Web.UI;
using w2.Domain.UpdateHistory.Helper;
using w2.App.Common.Input.Order;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.Zeus;
using w2.App.Common.User;
using w2.Common.Web;
using w2.Domain.Order;
using w2.Domain.UserCreditCard;
using w2.App.Common.Order.Payment.PayTg;
using System.Collections;

public partial class Form_User_UserCreditCardConfirm : UserCreditCardPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// HTTPS通信チェック
		//------------------------------------------------------
		CheckHttps(Constants.PATH_ROOT + Constants.PAGE_MANAGER_USER_CREDITCARD_CONFIRM);

		// 利用可能チェック
		CheckUsable(this.ActionStatus);

		// クレジットカード有効期限チェック
		CheckTokenExpiredAndRedirectPage();

		if (!IsPostBack)
		{
			// カード名変更の場合
			if (Request[Constants.REQUEST_KEY_CREDITCARD_NO] != null)
			{
				var updateCreditCard = (UserCreditCardInput)Session[Constants.SESSION_KEY_PARAM_FOR_USER_CREDITCARD_INPUT];
				lCardDispName.Text = updateCreditCard.CardDispName;

				// 表示内容
				trCreditCardNumber.Visible = trlExpiration.Visible 
					= trCreditAuthorName.Visible = false;
				btnUpdate.Visible = true;
			}
			else
			{
				// コンポーネント初期化
				InitComponents();
				btnSend.Visible = true;
				if (this.ProcessMode == ProcessModeType.OrderCreditCardAuth)
				{
					btnSend_Click(sender, e);
				}
			}
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitComponents()
	{
		var userCreditCard = this.OrderCreditCardInput;
		lCreditCardCompanyName.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_ORDER, OrderCommon.CreditCompanyValueTextFieldName, userCreditCard.CompanyCode));
		lDispCreditCardNo.Text =
			WebSanitizer.HtmlEncode(
				(this.CreditTokenizedPanUse)
					? userCreditCard.CardNo
					: "************" + UserCreditCardHelper.CreateCreditCardNoDispString(userCreditCard.CardNo));
		lExpirationMonth.Text = WebSanitizer.HtmlEncode(userCreditCard.ExpireMonth);
		lExpirationYear.Text = WebSanitizer.HtmlEncode(userCreditCard.ExpireYear);
		lAutherName.Text = WebSanitizer.HtmlEncode(userCreditCard.AuthorName);
		lCardDispName.Text = WebSanitizer.HtmlEncode(userCreditCard.RegisterCardName);
	}

	/// <summary>
	/// 送信リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSend_Click(object sender, EventArgs e)
	{
		// ゼウス以外の仮クレジットカード登録実行
		if (this.NeedsRegisterProvisionalCreditCardCardKbnExceptZeus)
		{
			new ProvisionalCreditCardProcessor().RegisterUnregisterdCreditCard(
				this.UserId,
				this.OrderCreditCardInput.RegisterCardName,
				Constants.FLG_USERCREDITCARD_REGISTER_ACTION_KBN_USERCREDITCARD_REGISTER,
				"",
				"",
				this.LoginOperatorName,
				UpdateHistoryAction.Insert);

			SetOrpnerDispUserDetailAndCloseScript();
		}
		// 通常の登録実行
		else if (this.ProcessMode == ProcessModeType.UserCreditCartdRegister)
		{
			ExecUserCreditcardRegister();
		}
		// ゼウス向け注文与信実行
		else if (this.ProcessMode == ProcessModeType.OrderCreditCardAuth)
		{
			// 注文クレジットカード与信実行
			var errorMessage = ExecOrderCreditcardAuthForZeus(this.OrderId, this.LoginOperatorName);
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
		}
		Session[Constants.SESSION_KEY_PARAM_FOR_USER_CREDITCARD_INPUT] = null;
	}

	/// <summary>
	/// ユーザークレジットカード登録実行
	/// </summary>
	private void ExecUserCreditcardRegister()
	{
		Result resutl;
		var userCreditCardInput = this.OrderCreditCardInput.CeateUserCreditCardInput(this.UserId);
		if (this.IsUserPayTg && (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten))
		{
			userCreditCardInput.CreditToken = CartPayment.CreditTokenInfoBase.CreateCreditTokenInfo(this.CreditTokenbyPayTg);
			userCreditCardInput.CompanyCode = this.OrderCreditCardInput.CompanyCode;
			resutl = new UserCreditCardRegister().ExecForPayTg(
				userCreditCardInput,
				SiteKbn.Pc,
				true,
				this.LoginOperatorName,
				UpdateHistoryAction.Insert);
		}
		else
		{
			// クレジットカード情報登録（更新履歴とともに）
			resutl = new UserCreditCardRegister().Exec(
				userCreditCardInput,
				SiteKbn.Pc,
				true,
				this.LoginOperatorName,
				UpdateHistoryAction.Insert);
		}

		if (resutl.Success)
		{
			if (SessionManager.UsePaymentTabletZeus)
			{
				var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_USER_CONFIRM)
					.AddParam(Constants.REQUEST_KEY_USER_ID, this.UserId)
					.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_DETAIL).CreateUrl();
				Response.Redirect(url);
			}
	
			var clientScript = string.Empty;
			// ポップアップ画面（注文返品交換画面から？）
			if (StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_WINDOW_KBN]) == Constants.KBN_WINDOW_POPUP)
			{
				// クレジット情報セット
				clientScript = "window.opener.setCreditInfo();window.close();";
				ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "reloadParent", clientScript, true);
			}
			// その他（ユーザー詳細から？）
			else
			{
				// クレジット一覧画面へ
				SetOrpnerDispUserDetailAndCloseScript();
			}
		}
		else
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CREDITCARD_AUTH_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR + "?" + Constants.REQUEST_KEY_WINDOW_KBN + "=" + Constants.KBN_WINDOW_POPUP);
		}
	}

	/// <summary>
	/// クレジットカードの更新
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		var updateCreditCard = (UserCreditCardInput)Session[Constants.SESSION_KEY_PARAM_FOR_USER_CREDITCARD_INPUT];

		var userCredtiModel = new UserCreditCardModel
		{
			UserId = this.UserId,
			BranchNo = int.Parse(Request[Constants.REQUEST_KEY_CREDITCARD_NO]),
			CardDispName = updateCreditCard.CardDispName,
			LastChanged = this.LoginOperatorName,
		};

		new UserCreditCardService().UpdateCardDisplayName(userCredtiModel, this.LoginOperatorName, UpdateHistoryAction.Insert); 
		
		// 画面遷移
		var url = new UrlCreator(Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_USER_CONFIRM)
			.AddParam(Constants.REQUEST_KEY_USER_ID, Request[Constants.REQUEST_KEY_USER_ID])
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_DETAIL)
			.CreateUrl();

		var clientScript = string.Format("window.opener.location.href=decodeURIComponent(\"{0}\");window.close();", HttpUtility.UrlEncode(url));
		ScriptManager.RegisterClientScriptBlock(this, GetType(), "reloadParent", clientScript, true);
	}

	/// <summary>
	/// 戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBuckHistoryBackTop_OnClick(object sender, EventArgs e)
	{
		var url = new UrlCreator(Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_USER_CREDITCARD_INPUT)
		.AddParam(Constants.REQUEST_KEY_USER_ID, this.UserId)
		.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, this.ActionStatus);
		Response.Redirect(url.CreateUrl());
	}

	/// <summary>
	/// オープン元をユーザー詳細遷移URLに設定して閉じるスクリプトセット
	/// </summary>
	/// <returns></returns>
	private void SetOrpnerDispUserDetailAndCloseScript()
	{
		var clientScript = string.Format("window.opener.location.href=\"{0}\";window.close();", CreateUserDetailUrl());
		ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "reloadParent", clientScript, true);
	}

	/// <summary>
	/// ユーザー詳細URL作成
	/// </summary>
	/// <returns></returns>
	private string CreateUserDetailUrl()
	{
		var url = new UrlCreator(Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_USER_CONFIRM)
			.AddParam(Constants.REQUEST_KEY_USER_ID, Request[Constants.REQUEST_KEY_USER_ID])
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_DETAIL)
			.AddParam("xxx", DateTime.Now.ToString("yyyyMMddHHmmssfff")).CreateUrl() + "#for_disp_creditcard";
		return url;
	}

	/// <summary>
	/// ZEUS向け注文クレジットカード与信実行
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <param name="lastChanged">最終更新者</param>
	/// <returns>エラーメッセージ</returns>
	private string ExecOrderCreditcardAuthForZeus(
		string orderId,
		string lastChanged)
	{
		// ZEUSクレジットカード登録
		var order = new OrderService().Get(orderId);
		var userCreditCardOld = new UserCreditCardService().Get(order.UserId, order.CreditBranchNo.Value);
		var cooperationInfo = new UserCreditCard.UserCardCooperationInfo(userCreditCardOld);
		var result = new ZeusSecureLinkApi().ExecForRegisterCreditCard(
			this.OrderCreditCardInput.CreditToken.Token,
			cooperationInfo.ZeusTelNo,
			order.Owner.OwnerMailAddr,
			cooperationInfo.ZeusSendId);
		if (result.Success == false)
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CREDITCARD_AUTH_ERROR);
		}
		// クレジットカード情報更新（登録済みへ）
		new ProvisionalCreditCardProcessor().UpdateProvisionalCreditCardRegisterdForZeus(
			userCreditCardOld,
			UserCreditCardHelper.CreateCreditCardNoDispString(this.OrderCreditCardInput.CardNo),
			this.OrderCreditCardInput.ExpireMonth,
			this.OrderCreditCardInput.ExpireYear,
			this.OrderCreditCardInput.AuthorName,
			this.OrderCreditCardInput.DoRegister,
			this.OrderCreditCardInput.RegisterCardName,
			lastChanged,
			UpdateHistoryAction.DoNotInsert);

		// 与信＆注文確定実行
		var messages = new ProvisionalCreditCardProcessor().AuthAndUpdateOrderStatus(
			orderId,
			order.CreditBranchNo.Value,
			this.OrderCreditCardInput.CompanyCode,
			this.OrderCreditCardInput.InstallmentsCode,
			this.OrderCreditCardInput.InstallmentsName,
			lastChanged,
			UpdateHistoryAction.Insert);
		if (string.IsNullOrEmpty(messages) == false)
		{
			this.OrderCreditCardInput.CreditToken.SetTokneExpired(); // 失敗したらトークン無効にする
			return messages;
		}

		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_CONFIRM)
			.AddParam(Constants.REQUEST_KEY_ORDER_ID, orderId)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_DETAIL).CreateUrl();
		Response.Redirect(url);
		return "";
	}

	/// <summary>
	/// クレジットカード表示文字列（「************1234」）取得
	/// </summary>
	/// <returns>クレジットカード表示文字列</returns>
	public string GetCreditCardDispString(string cardNo)
	{
		string cardNoTmp = "    " + cardNo;
		return cardNoTmp.Substring(cardNoTmp.Length - 4, 4);
	}

	/// <summary>
	/// トークン有効期限切れをチェックし、切れていればエラーページへ遷移する、そもそもトークンがない場合は入力ページへ遷移する
	/// </summary>
	private void CheckTokenExpiredAndRedirectPage()
	{
		if (this.NeedsRegisterProvisionalCreditCardCardKbnExceptZeus) return;
		if (OrderCommon.CreditTokenUse == false) return;

		if (this.OrderCreditCardInput.CreditToken == null)
		{
			HttpContext.Current.Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_USER_CREDITCARD_INPUT);
		}
		if (this.OrderCreditCardInput.IsTokenExpired())
		{
			HttpContext.Current.Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_CREDIT_TOKEN_EXPIRED);
			HttpContext.Current.Response.Redirect(
				Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR +
					"?" + Constants.REQUEST_KEY_WINDOW_KBN + "=" + HttpUtility.UrlEncode(Constants.KBN_WINDOW_POPUP));
		}
	}

	/// <summary>
	/// オーダークレジットカード入力（共用する）
	/// </summary>
	protected OrderCreditCardInput OrderCreditCardInput
	{
		get { return (OrderCreditCardInput)Session[Constants.SESSION_KEY_PARAM_FOR_USER_CREDITCARD_INPUT]; }
	}
	/// <summary>PayTgクレジットトークン</summary>
	protected string CreditTokenbyPayTg
	{
		get { return (string)this.Session[PayTgConstants.PARAM_TOKEN]; }
		set { this.Session[PayTgConstants.PARAM_TOKEN] = value; }
	}
}
