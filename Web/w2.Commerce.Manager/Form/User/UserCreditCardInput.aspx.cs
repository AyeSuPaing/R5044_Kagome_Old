/*
=========================================================================================================
  Module      : ユーザクレジットカード入力画面処理(UserCreditCardInput.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.Input.Order;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.PayTg;
using w2.App.Common.Order.Payment.Veritrans;
using w2.App.Common.User;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.Order;
using w2.Domain.User;
using w2.Domain.UserCreditCard;

public partial class Form_User_UserCreditCardInput : UserCreditCardPage
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
		CheckHttps(Constants.PATH_ROOT + Constants.PAGE_MANAGER_USER_CREDITCARD_INPUT);

		// 利用可能チェック
		CheckUsable(this.ActionStatus);

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// コンポーネント初期化
			//------------------------------------------------------
			if (Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_UPDATE)
			{
				var userCreditCard = new UserCreditCardService().Get(Request[Constants.FIELD_USER_USER_ID], 
				int.Parse(this.BranchNo));
				
				tbUserCreditCardName.Text = userCreditCard.CardDispName;
				trCreditExpire.Visible = false;
				tbCreditAuthorName.Visible = false;
				tdCreditNumber.Visible = true;
				trInstallments.Visible = false;
				trSecurityCode.Visible = false;
				tdGetCardInfo.Visible = false;
				phCreditCardNotRakuten.Visible = false;
			}
			else
			{
				InitComponents();
			}

			// 画面表示
			switch (this.ProcessMode)
			{
				case ProcessModeType.UserCreditCartdRegister:
					trRegistCreditCard.Visible = false;
					trCreditCardName.Visible = true;
					break;

				case ProcessModeType.OrderCreditCardAuth:
					var order = DisplayOrderInfo();
					this.CreditBranchNo = order.CreditBranchNo;
					if ((order.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_TEMP)
						|| (order.OrderPaymentKbn != Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID))
					{
						Response.Redirect(OrderPage.CreateOrderDetailUrl(this.OrderId, false, false, ""));
					}
					var user = new UserService().Get(order.UserId);
					trRegistCreditCard.Visible = (user.IsMember && (Constants.MAX_NUM_REGIST_CREDITCARD > user.UserCreditCards.Length));
					trCreditCardName.Visible = cbRegistCreditCard.Checked;
					break;
			}

			if (this.IsUserPayTg
				&& (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.VeriTrans)
				&& Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_INSERT)
			{
				// PayTg端末状態取得
				GetPayTgDeviceStatus();
			}
		}
		else
		{
			// トークンが入力されていたら入力画面を切り替える
			SwitchDisplayForCreditTokenInput();
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitComponents()
	{
		// カード会社リスト
		if (Constants.PAYMENT_CARD_KBN == w2.App.Common.Constants.PaymentCard.YamatoKwc)
		{
			ddlCreditCardCompany.Items.AddRange(
				ValueText.GetValueItemList(Constants.TABLE_ORDER, OrderCommon.CreditCompanyValueTextFieldName).Cast<ListItem>().ToArray());
		}

		// 有効期限(月)ドロップダウン作成
		ddlCreditExpireMonth.Items.AddRange(this.CreditExpirationMonthListItems);

		// 有効期限(年)ドロップダウン作成
		ddlCreditExpireYear.Items.AddRange(this.CreditExpirationYearListItems);

		// 分割払い（注文与信用）
		trInstallments.Visible = (this.ProcessMode == ProcessModeType.OrderCreditCardAuth) && OrderCommon.CreditInstallmentsSelectable;
		if (OrderCommon.CreditInstallmentsSelectable)
		{
			dllCreditInstallments.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDER, OrderCommon.CreditInstallmentsValueTextFieldName));
		}

		try
		{
			Hashtable fillBackData;
			fillBackData = (Session[Constants.SESSION_KEY_PARAM_FOR_USER_CREDITCARD_INPUT] != null) && (Session[Constants.SESSION_KEY_PARAM_FOR_USER_CREDITCARD_INPUT].GetType() == typeof(Hashtable))
				? (Hashtable)Session[Constants.SESSION_KEY_PARAM_FOR_USER_CREDITCARD_INPUT]
				: new Hashtable();

			tbCreditCardNo1.Text = StringUtility.ToEmpty(fillBackData[CartPayment.FIELD_CREDIT_CARD_NO_1]);
			ddlCreditExpireMonth.Text = StringUtility.ToEmpty(fillBackData[CartPayment.FIELD_CREDIT_EXPIRE_MONTH]);
			ddlCreditExpireYear.Text = StringUtility.ToEmpty(fillBackData[CartPayment.FIELD_CREDIT_EXPIRE_YEAR]);
			tbCreditAuthorName.Text = StringUtility.ToEmpty(fillBackData[CartPayment.FIELD_CREDIT_AUTHOR_NAME]);
			tbUserCreditCardName.Text = StringUtility.ToEmpty(fillBackData[CartPayment.FIELD_REGIST_CREDIT_CARD_NAME]);
		}
		catch (Exception)
		{ }

		trSecurityCode.Visible = OrderCommon.CreditSecurityCodeEnable;

		// 楽天連携かつPayTg非利用の場合、クレカ入力領域を非表示にする
		this.phCreditCardNotRakuten.Visible = (this.IsNotRakutenAgency || this.IsUserPayTg);

		if (this.IsUserPayTg && (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans))
		{
			if (string.IsNullOrEmpty(tbCreditCardNo1.Text)) tbCreditCardNo1.Text = Constants.PAYMENT_SETTING_PAYTG_DEFAULT_CARD_NUMBER;
			ddlCreditExpireMonth.SelectedValue = Constants.PAYMENT_SETTING_PAYTG_DEFAULT_EXPIRATION_MONTH;
			ddlCreditExpireYear.SelectedValue = Constants.PAYMENT_SETTING_PAYTG_DEFAULT_EXPIRATION_YEAR;
		}

		tdCreditNumber.Visible = Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_UPDATE
			? true
			: (this.IsUserPayTg == false);

		trCreditExpire.Visible = (this.IsUserPayTg == false);
		trSecurityCode.Visible = (this.IsUserPayTg == false);
		tdGetCardInfo.Visible = this.IsUserPayTg;
	}

	/// <summary>
	/// カード情報取得ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnGetCardInfo_Click(object sender, EventArgs e)
	{
		// PayTg WebApiで利用するため決済注文IDを採番
		var paymentOrderId = OrderCommon.CreatePaymentOrderId(this.LoginOperatorShopId);
		hfPayTgSendId.Value = paymentOrderId;

		// PayTG連携APIのポストデータ作成
		hfPayTgPostData.Value = Constants.PAYMENT_SETTING_PAYTG_MOCK_ENABLED
			? string.Empty
			: new PayTgApi(paymentOrderId).CreatePostData();

		var apiUrl = Constants.PAYMENT_SETTING_PAYTG_MOCK_ENABLED
			? CreateRegisterCardMockUrl()
			: Constants.PAYMENT_SETTING_PAYTG_BASEURL + PayTgConstants.PspShortName.RAKUTEN + PayTgConstants.DealingTypes.URL_CHECKCARD;

		// PayTG連動でカード登録実行
		ScriptManager.RegisterStartupScript(
			this,
			GetType(),
			"execRegistration",
			string.Format(
				"execCardRegistration('{0}', '{1}');",
				apiUrl,
				hfPayTgPostData.Value),
			true);
	}

	/// <summary>
	/// PayTG連携のレスポンス処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnProcessPayTgResponse_Click(object sender, EventArgs e)
	{
		var payTg = new PayTgApi(hfPayTgSendId.Value);
		payTg.ParseResponse(hfPayTgResponse.Value);

		this.IsSuccessfulCardRegistration = false;

		if (payTg.Result.IsSuccess)
		{
			var cardExpireMonth = payTg.Result.Response.McAcntNo1;
			var cardExpireYear = payTg.Result.Response.Expire;
			var lastFourDigit = payTg.Result.Response.Last4;
			var cardNumber = "XXXXXXXXXXXX" + lastFourDigit;
			var companyCode = payTg.Result.Response.AcqName;
			var token = payTg.Result.Response.Token;
			var vResultCode = payTg.Result.Response.VResultCode;
			var errorMsg = payTg.Result.Response.ErrorMsg;

			var payTgResponse = new Hashtable
				{
					{ PayTgConstants.PAYTG_CARD_EXPIRE_MONTH, cardExpireMonth },
					{ PayTgConstants.PAYTG_CARD_EXPIRE_YEAR, cardExpireYear },
					{ PayTgConstants.PARAM_LAST4, lastFourDigit },
					{ PayTgConstants.PAYTG_CARD_NUMBER, cardNumber },
					{ PayTgConstants.PARAM_ACQNAME, companyCode },
					{ PayTgConstants.PARAM_TOKEN, token },
					{ PayTgConstants.PAYTG_RESPONSE_ERROR, string.Empty },
				};
			Session[PayTgConstants.PARAM_TOKEN] = hfPayTgSendId.Value = payTg.Result.Response.Token;
			Session[PayTgConstants.PARAM_ACQNAME] = companyCode;
			this.PayTgResponse = payTgResponse;

			ddlCreditExpireMonth.SelectedValue = cardExpireMonth;
			ddlCreditExpireYear.SelectedValue = cardExpireYear.Substring(cardExpireYear.Length - 2);
			tbCreditCardNo1.Text = cardNumber;

			this.IsSuccessfulCardRegistration = true;
			divErrorMessage.Visible = false;
		}
		else
		{
			// PCサイト向けに優先したいクレジットカードメッセージ
			var cardErrorMessageForPc = string.Empty;

			this.IsSuccessfulCardRegistration = false;
			var resultCode = payTg.Result.Response.VResultCode;

			if (string.IsNullOrEmpty(resultCode))
			{
				// PayTg端末のエラーの場合はエラーメッセージを統一
				resultCode = PayTgConstants.ERRMSG_PAYTG_UNAVAILABLE;
			}
			var creditError = new CreditErrorMessage();
			creditError.SetCreditErrorMessages(Constants.FILE_XML_RAKUTEN_CREDIT_ERROR_MESSAGE);
			var errorList = creditError.GetValueItemArray();
			cardErrorMessageForPc = (errorList.Any(s => s.Value == resultCode))
				? errorList.First(s => (s.Value == resultCode)).Text
				: string.Empty;

			var payTgResponse = new Hashtable { { PayTgConstants.PAYTG_RESPONSE_ERROR, cardErrorMessageForPc } };
			this.PayTgResponse = payTgResponse;
			divErrorMessage.Visible = true;
			lbErrorMessage.Text = cardErrorMessageForPc;
		}
		DisplayCreditInputForm();
	}

	/// <summary>
	/// 確認するリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, EventArgs e)
	{
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
				ConrimInset();
				break;

			case Constants.ACTION_STATUS_UPDATE:
				ConfirmUpdate();
				break;
		}
	}

	/// <summary>
	/// 登録用確認
	/// </summary>
	protected void ConrimInset()
	{
		var userId = this.UserId ?? new OrderService().Get(this.OrderId).UserId;

		var orderCreditCardInput = GetOrderCreditCardInputForUserCreditCardPage(userId, this.ProcessMode);
		if (this.NeedsRegisterProvisionalCreditCardCardKbnExceptZeus)
		{
			orderCreditCardInput = new OrderCreditCardInput
			{
				CreditBranchNo = CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW,
				CompanyCode = orderCreditCardInput.CompanyCode,
				RegisterCardName = orderCreditCardInput.RegisterCardName,
				DoRegister = cbRegistCreditCard.Checked,
			};
		}
		else if (this.IsUserPayTg
			&& (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten))
		{
			orderCreditCardInput.ExpireMonth = ddlCreditExpireMonth.SelectedValue;
			orderCreditCardInput.ExpireYear = ddlCreditExpireYear.SelectedValue;
			orderCreditCardInput.CompanyCode = this.CreditCardCompanyCodebyPayTg;
			orderCreditCardInput.CreditToken = CartPayment.CreditTokenInfoBase.CreateCreditTokenInfo(this.CreditTokenbyPayTg);
		}

		// トークンが取得できていないときはエラーとして扱う(バグ#3554対策)
		if (OrderCommon.CreditTokenUse
			&& string.IsNullOrEmpty(hfCreditToken.Value)
			&& (this.NeedsRegisterProvisionalCreditCardCardKbnExceptZeus == false))
		{
			spanErrorMessageForCreditCard.InnerHtml =
				WebSanitizer.HtmlEncodeChangeToBr(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CARDAUTH_ERROR));
			spanErrorMessageForCreditCard.Style["display"] = "block";
			return;
		}

		// 入力チェック
		var errorMessages = orderCreditCardInput.Validate();
		if (string.IsNullOrEmpty(errorMessages) == false)
		{
			lbErrorMessage.Text = errorMessages;
			divErrorMessage.Visible = true;
			return;
		}

		// 画面遷移
		Session[Constants.SESSION_KEY_PARAM_FOR_USER_CREDITCARD_INPUT] = orderCreditCardInput;
		var url = new UrlCreator(Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_USER_CREDITCARD_CONFIRM)
			.AddParam(Constants.REQUEST_KEY_USER_ID, Request[Constants.REQUEST_KEY_USER_ID])
			.AddParam(Constants.REQUEST_KEY_ORDER_ID, Request[Constants.REQUEST_KEY_ORDER_ID])
			.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Request[Constants.REQUEST_KEY_WINDOW_KBN])
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, this.ActionStatus).CreateUrl();

		var clientScript = string.Format("location.href=decodeURIComponent(\"{0}\")", HttpUtility.UrlEncode(url));
		ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CreditCardConfirm", clientScript, true);
	}

	/// <summary>
	/// 更新用確認
	/// </summary>
	protected void ConfirmUpdate()
	{
		var userCreditCardInput = new UserCreditCardInput
		{
			CardDispName = tbUserCreditCardName.Text,
		};

		var errorMessages = userCreditCardInput.Validate();
		if (errorMessages != "")
		{
			lbErrorMessage.Text = errorMessages;
			divErrorMessage.Visible = true;
			return;
		}

		Session[Constants.SESSION_KEY_PARAM_FOR_USER_CREDITCARD_INPUT] = userCreditCardInput;

		// 画面遷移
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_USER_CREDITCARD_CONFIRM)
			.AddParam(Constants.REQUEST_KEY_USER_ID, this.UserId)
			.AddParam(Constants.REQUEST_KEY_CREDITCARD_NO,this.BranchNo)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, this.ActionStatus).CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// （トークン決済向け）カード情報編集リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbEditCreditCardNoForToken_Click(object sender, System.EventArgs e)
	{
		// トークンなどクレジットカード情報削除
		ResetCreditTokenInfoFromForm(null, "DUMMY");

		SwitchDisplayForCreditTokenInput();
	}

	/// <summary>
	/// クレジットトークン向け  カード情報取得JSスクリプト作成（カート内ではない）
	/// </summary>
	/// <returns>
	/// カード情報取得スクリプト
	/// 文字列を返すのでevalで動的実行すれば連想配列でカード情報がとれます
	/// </returns>
	protected string CreateGetCardInfoJsScriptForCreditToken()
	{
		var script = base.CreateGetCardInfoJsScriptForCreditTokenInner();
		return script;
	}

	/// <summary>
	/// クレジットトークン取得＆フォームセットJSスクリプト作成
	/// </summary>
	/// <returns>JSスクリプト</returns>
	protected string CreateGetCreditTokenAndSetToFormJsScript()
	{
		var script = CreateGetCreditTokenAndSetToFormJsScriptInner();
		return script;
	}

	/// <summary>
	/// クレジットトークン向け フォームマスキングJSスクリプト作成
	/// </summary>
	/// <returns>JSスクリプト</returns>
	protected string CreateMaskFormsForCreditTokenJsScript()
	{
		var maskingScripts = CreateMaskFormsForCreditTokenJsScriptInner();
		return maskingScripts;
	}

	/// <summary>
	/// クレジットカード登録チェックボックスチェック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbRegistCreditCard_CheckedChanged(object sender, EventArgs e)
	{
		DisplayCreditInputForm();

		trCreditCardName.Visible = cbRegistCreditCard.Checked;
	}

	/// <summary>
	/// クレジット入力フォーム表示切り替え
	/// </summary>
	private void DisplayCreditInputForm()
	{
		if (this.IsUserPayTg)
		{
			btnGetCreditCardInfo.Enabled = (this.IsSuccessfulCardRegistration == false);
			ddlCreditExpireMonth.Enabled = (this.IsSuccessfulCardRegistration == false);
			ddlCreditExpireYear.Enabled = (this.IsSuccessfulCardRegistration == false);
			tbCreditCardNo1.Enabled = (this.IsSuccessfulCardRegistration == false);
			trCreditExpire.Visible = this.IsSuccessfulCardRegistration;
			tdCreditNumber.Visible = this.IsSuccessfulCardRegistration;
			tdGetCardInfo.Visible = (this.IsSuccessfulCardRegistration == false);
		}
	}

	/// <summary>
	/// PayTg端末状態取得
	/// </summary>
	private void GetPayTgDeviceStatus()
	{
		var apiUrl = Constants.PAYMENT_SETTING_PAYTG_MOCK_ENABLED
			? Constants.PATH_ROOT + Constants.PAGE_MANAGER_CHECK_DEVICE_STATUS_MOCK
			: Constants.PAYMENT_SETTING_PAYTG_DEVICE_STATUS_CHECK_URL;

		// PayTG連動でカード登録実行
		ScriptManager.RegisterStartupScript(
			this,
			GetType(),
			"execGetDeviceStatus",
			string.Format("execGetPayTgDeviceStatus('{0}');", apiUrl),
			true);
	}

	/// <summary>クレジットカード枝番（与信時）</summary>
	protected int? CreditBranchNo
	{
		get { return (int?)ViewState["CreditBranchNo"]; }
		set { ViewState["CreditBranchNo"] = value;  }
	}
	/// <summary>有効期限(月)</summary>
	protected ListItemCollection CreditExpireMonth { get; private set; }
	/// <summary>有効期限(年)</summary>
	protected ListItemCollection CreditExpireYear { get; private set; }
	/// <summary>カード枝番(パラメータ)</summary>
	public string BranchNo
	{
		get { return Request.QueryString[Constants.REQUEST_KEY_CREDITCARD_NO]; }
	}
	/// <summary>楽天連携以外か</summary>
	protected bool IsNotRakutenAgency
	{
		get { return (Constants.PAYMENT_CARD_KBN != w2.App.Common.Constants.PaymentCard.Rakuten); }
	}
	/// <summary>PayTgクレジットトークン</summary>
	protected string CreditTokenbyPayTg
	{
		get { return (string)this.Session[PayTgConstants.PARAM_TOKEN]; }
		set { this.Session[PayTgConstants.PARAM_TOKEN] = value; }
	}
	/// <summary>PayTgクレジット会社コード</summary>
	protected string CreditCardCompanyCodebyPayTg
	{
		get { return (string)this.Session[PayTgConstants.PARAM_ACQNAME]; }
		set { this.Session[PayTgConstants.PARAM_ACQNAME] = value; }
	}
}
