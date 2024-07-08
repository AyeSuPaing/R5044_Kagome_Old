/*
=========================================================================================================
  Module      : ユーザクレジットカード入力画面処理(UserCreditCardInput.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.Rakuten;
using w2.App.Common.User;
using w2.App.Common.Web.Process;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain.UserCreditCard;

public partial class Form_User_UserCreditCardInput : BasePage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return true; } }	// ログイン必須
	/// <summary>マイページメニュー表示判定</summary>
	public override bool DispMyPageMenu { get { return true; } }	// マイページメニュー表示
	/// <summary>クレジットカード情報</summary>
	private UserCreditCardModel _creditCard;

	# region ラップ済みコントロール宣言
	private CommonPageProcess.WrappedCreditCardInputs WciCardInputs
	{
		get { return m_wciCardInputs ?? (m_wciCardInputs = new CommonPageProcess.WrappedCreditCardInputs(this.Process, this.Page)); }
	}
	private CommonPageProcess.WrappedCreditCardInputs m_wciCardInputs;
	private WrappedHtmlGenericControl WtrSecurityCode { get { return GetWrappedControl<WrappedHtmlGenericControl>("trSecurityCode"); } }
	private WrappedHtmlGenericControl WdivCreditCardNoToken { get { return GetWrappedControl<WrappedHtmlGenericControl>("divCreditCardNoToken"); } }
	private WrappedHtmlGenericControl WdivCreditCardForTokenAcquired { get { return GetWrappedControl<WrappedHtmlGenericControl>("divCreditCardForTokenAcquired"); } }
	private WrappedHtmlGenericControl WdivRakutenCredit { get { return GetWrappedControl<WrappedHtmlGenericControl>("divRakutenCredit"); } }
	#endregion

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

		//------------------------------------------------------
		// クレジットカード登録可能チェック
		//------------------------------------------------------
		// クレジットカード登録不可の場合、クレジットカード一覧画面へ遷移する
		if ((OrderCommon.GetCreditCardRegistable(this.IsLoggedIn, this.LoginUserId) == false) && (this.BranchNo == 0))
		{
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_CREDITCARD_LIST);
		}

		// 枝番正常系のときは クレカ情報取得
		_creditCard = (this.BranchNo != 0) ? new UserCreditCardService().Get(this.LoginUserId, this.BranchNo) : null;
		// 取得できても表示フラグOFFか、異常系の際には新規登録へ
		if (((_creditCard != null) && (_creditCard.IsDisp == false))
			|| ((_creditCard == null)) && (Request.QueryString[Constants.REQUEST_KEY_CREDITCARD_NO] != null))
		{
			var url = this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_CREDITCARD_INPUT;
			Response.Redirect(url);
		}

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// コンポーネント初期化
			//------------------------------------------------------
			InitComponents();

			//------------------------------------------------------
			// ユーザークレジット情報取得
			//------------------------------------------------------
			var isBackFromConfirm = false;
			if (this.SessionParamTargetPage == Constants.PAGE_FRONT_USER_CREDITCARD_INPUT)
			{
				// ターゲットページ情報をクリア
				this.SessionParamTargetPage = null;

				isBackFromConfirm = (this.CreditCardSession != null);
			}

			//------------------------------------------------------
			// ユーザークレジット情報項目セット
			//------------------------------------------------------

			// 変更処理時、確認画面から戻った場合
			if (isBackFromConfirm && (this.BranchNo != 0))
			{
				this.WciCardInputs.WtbUserCreditCardName.Text = this.CreditCardSession.CardDispName;
				this.WdivRakutenCredit.Visible = false;
				this.WdivCreditCardNoToken.Visible = false;
				this.WdivCreditCardForTokenAcquired.Visible = false;
			}
			// 新規登録時、確認画面から戻った場合
			else if (isBackFromConfirm)
			{
				this.WciCardInputs.WtbUserCreditCardName.Text = this.CreditCardSession.CardDispName;

				foreach (ListItem li in this.WciCardInputs.WddlCardCompany.Items)
				{
					li.Selected = (li.Value == this.CreditCardSession.CompanyCode);
				}

				// クレジットカード番号再表示の際は、上位４桁と下位４桁を補完
				var cardNo1 = this.CreditCardSession.CardNo1.Replace(Constants.CHAR_MASKING_FOR_TOKEN, "");
				var cardNo2 = this.CreditCardSession.CardNo2.Replace(Constants.CHAR_MASKING_FOR_TOKEN, "");
				var cardNo3 = this.CreditCardSession.CardNo3.Replace(Constants.CHAR_MASKING_FOR_TOKEN, "");
				var cardNo4 = this.CreditCardSession.CardNo4.Replace(Constants.CHAR_MASKING_FOR_TOKEN, "");

				if (cardNo2 + cardNo3 + cardNo4 == "")
				{
					this.WciCardInputs.WtbCard1.Text = "";
				}
				if (cardNo4 != "")
				{
					this.WciCardInputs.WtbCard1.Text = cardNo1;
					this.WciCardInputs.WtbCard4.Text = cardNo4;
				}
				else
				{
					this.WciCardInputs.WtbCard1.Text = cardNo1;
					this.WciCardInputs.WtbCard3.Text = cardNo3;
				}

				foreach (ListItem li in this.WciCardInputs.WddlExpireMonth.Items)
				{
					li.Selected = (li.Value == this.CreditCardSession.ExpirationMonth);
				}
				foreach (ListItem li in this.WciCardInputs.WddlExpireYear.Items)
				{
					li.Selected = (li.Value == this.CreditCardSession.ExpirationYear);
				}
				this.WciCardInputs.WtbAuthorName.Text = this.CreditCardSession.AuthorName;
				// セキュリティコードは補完しない
				this.WciCardInputs.WtbSecurityCode.Text = "";
				// トークン補完
				if (OrderCommon.CreditTokenUse && (string.IsNullOrEmpty(this.WciCardInputs.WhfCreditToken.Value) == false))
				{
					this.WciCardInputs.WhfCreditToken.Value = this.CreditCardSession.CreditToken.CreateTokenHiddenValue();
					if (this.CreditCardSession.CreditCvvToken != null)
					{
						this.WciCardInputs.WhfMyCvvMount.Value = this.CreditCardSession.CreditCvvToken.CreateTokenHiddenValue();
					}

					SwitchDisplayForCreditTokenInput();
				}
			}
			DisplayCreditCardInfo(isBackFromConfirm);

			//------------------------------------------------------
			// データバインド
			//------------------------------------------------------
			this.WciCardInputs.WddlExpireMonth.DataBind();
			this.WciCardInputs.WddlExpireYear.DataBind();

			//------------------------------------------------------
			// 表示制御
			//------------------------------------------------------
			if (this.BranchNo == 0)
			{
				this.WtrSecurityCode.Visible = OrderCommon.CreditSecurityCodeEnable;
			}

			// トークン決済の場合はクライアント検証をオフ
			DisableCreditInputCustomValidatorForGetCreditToken();

			ucRakutenCreditCard.DataBind();
		}
		else
		{
			// トークンが入力されていたら入力画面を切り替える
			SwitchDisplayForCreditTokenInput();
		}

		if (this.BranchNo == 0) RefreshCreditForm();

		if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten)
		{
			this.WciCardInputs.WcvUserCreditCardName.Visible = false;
		}

		// apsx側プロパティセットしているため、バインドを行う
		if (this.Captcha != null) this.Captcha.DataBind();
	}

	/// <summary>
	/// 画面にクレジットカード情報を載せる
	/// </summary>
	/// <returns>確認ページからの遷移か</returns>
	private void DisplayCreditCardInfo(bool isBackFromConfirm)
	{
		// 確認画面からの遷移か、カード情報がない際は 表示しない
		if (isBackFromConfirm || _creditCard == null) return;
		// カード登録名のみ表示
		this.WciCardInputs.WtbUserCreditCardName.Text = _creditCard.CardDispName;
		this.WdivRakutenCredit.Visible = false;
		this.WdivCreditCardNoToken.Visible = false;
		this.WdivCreditCardForTokenAcquired.Visible = false;
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitComponents()
	{
		// カード会社リスト
		if (Constants.PAYMENT_CARD_KBN == w2.App.Common.Constants.PaymentCard.YamatoKwc)
		{
			this.WciCardInputs.WddlCardCompany.Items.AddRange(
				ValueText.GetValueItemList(Constants.TABLE_ORDER, OrderCommon.CreditCompanyValueTextFieldName).Cast<ListItem>().ToArray());
		}
		// 有効期限(月)ドロップダウン作成
		this.WciCardInputs.WddlExpireMonth.AddItems(DateTimeUtility.GetCreditMonthListItem());
		// 有効期限(年)ドロップダウン作成
		this.WciCardInputs.WddlExpireYear.AddItems(DateTimeUtility.GetCreditYearListItem());
	}

	/// <summary>
	/// キャプチャ認証チェック
	/// </summary>
	/// <returns>成功：true、失敗：false（※キャプチャ認証利用なしの場合もtrueを返す）</returns>
	protected bool CheckCaptcha()
	{
		// キャプチャ認証サイトキー指定なしの場合は何もしない
		if (string.IsNullOrEmpty(Constants.RECAPTCHA_SITE_KEY)) return true;

		// キャプチャ認証コントロールがなしの場合は何もしない
		if (this.Captcha == null) return true;

		// キャプチャ認証が成功か
		if (this.Captcha.IsSuccess) return true;

		return false;
	}

	/// <summary>
	/// 確認するリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbConfirm_Click(object sender, EventArgs e)
	{
		// キャプチャ認証失敗時は処理終了
		if (CheckCaptcha() == false) return;
		if (this.BranchNo != 0)
		{
			Update();
		}
		else
		{
			Insert();
		}
	}

	/// <summary>
	/// 登録の場合
	/// </summary>
	private void Insert()
	{
		var companyCode = OrderCommon.CreditCompanySelectable ? this.WciCardInputs.WddlCardCompany.SelectedValue : "";
		if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten)
		{
			this.WciCardInputs.WhfMyCardMount.Value = GetWrappedControl<WrappedHiddenField>(ucRakutenCreditCard, "hfMyCardMount").Value;
			this.WciCardInputs.WhfMyCvvMount.Value = GetWrappedControl<WrappedHiddenField>(ucRakutenCreditCard, "hfMyCvvMount").Value;
			var year = GetWrappedControl<WrappedHiddenField>(ucRakutenCreditCard, "hfMyExpirationYearMount").Value;
			this.WciCardInputs.WhfMyExpirationYearMount.Value = year.Substring(year.Length - 2, 2);
			this.WciCardInputs.WhfMyExpirationMonthMount.Value = GetWrappedControl<WrappedHiddenField>(ucRakutenCreditCard, "hfMyExpirationMonthMount").Value;
			this.WciCardInputs.WhfAuthorCardName.Text = GetWrappedControl<WrappedHiddenField>(ucRakutenCreditCard, "hfAuthorNameCard").Value;
			this.WciCardInputs.WhfCreditCardCompany.Value = GetWrappedControl<WrappedHiddenField>(ucRakutenCreditCard, "hfCreditCardCompany").Value;
			companyCode = RakutenApiFacade.ConvertCompanyCode(this.WciCardInputs.WhfCreditCardCompany.Value);
		}
		// パラメタ格納
		var creditCardInput = new UserCreditCardInput
		{
			UserId = this.LoginUserId,
			CardDispName = this.WciCardInputs.WtbUserCreditCardName.Text.Trim(),
			CompanyCode = companyCode,
			CardNo = Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten ? this.WciCardInputs.WhfMyCardMount.Value : this.WciCardInputs.WtbCard1.Text.Trim() + this.WciCardInputs.WtbCard2.Text.Trim() + this.WciCardInputs.WtbCard3.Text.Trim() + this.WciCardInputs.WtbCard4.Text.Trim(),
			CardNo1 = Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten ? this.WciCardInputs.WhfMyCardMount.Value : this.WciCardInputs.WtbCard1.Text.Trim(),
			CardNo2 = Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten ? this.WciCardInputs.WhfMyCardMount.Value : this.WciCardInputs.WtbCard2.Text.Trim(),
			CardNo3 = Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten ? this.WciCardInputs.WhfMyCardMount.Value : this.WciCardInputs.WtbCard3.Text.Trim(),
			CardNo4 = Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten ? this.WciCardInputs.WhfMyCardMount.Value : this.WciCardInputs.WtbCard4.Text.Trim(),
			ExpirationMonth = Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten ? this.WciCardInputs.WhfMyExpirationMonthMount.Value : this.WciCardInputs.WddlExpireMonth.Text,
			ExpirationYear = Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten ? this.WciCardInputs.WhfMyExpirationYearMount.Value : this.WciCardInputs.WddlExpireYear.Text,
			AuthorName = Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten ? this.WciCardInputs.WhfAuthorCardName.Text.Trim() : this.WciCardInputs.WtbAuthorName.Text.Trim(),
			SecurityCode = (OrderCommon.CreditSecurityCodeEnable && (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Rakuten) ? this.WciCardInputs.WtbSecurityCode.Text : null),
			CreditToken = CartPayment.CreditTokenInfoBase.CreateCreditTokenInfo(this.WciCardInputs.WhfCreditToken.Value),
			CreditCvvToken = OrderCommon.CreditSecurityCodeEnable && Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten
				? CartPayment.CreditTokenInfoBase.CreateCreditTokenInfo(this.WciCardInputs.WhfMyCvvMount.Value)
				: null,
		};

		var dicErrorMessages = creditCardInput.ValidateForFrontUserCreditCardRegist();
		if (dicErrorMessages.Count != 0)
		{
			// トークン決済の場合はクライアント検証
			EnableCreditInputCustomValidatorForGetCreditToken();

			// カスタムバリデータ取得
			List<CustomValidator> lCustomValidators = new List<CustomValidator>();
			CreateCustomValidators(this, lCustomValidators);

			// エラーをカスタムバリデータへ
			SetControlViewsForError("UserCreditCardRegist", dicErrorMessages, lCustomValidators);

			// カード番号桁数エラーをカスタムバリデータへ
			if (this.WciCardInputs.WcvCreditCardNo1.IsValid)
			{
				ChangeControlLooksForValidator(
					dicErrorMessages,
					CartPayment.FIELD_CREDIT_CARD_NO + "_length",
					this.WciCardInputs.WcvCreditCardNo1,
					this.WciCardInputs.WtbCard1,
					this.WciCardInputs.WtbCard2,
					this.WciCardInputs.WtbCard3,
					this.WciCardInputs.WtbCard4);
			}
			return;
		}

		// 入力チェック
		// トークンが取得できていないときはエラーとして扱う(バグ#3554対策)
		if (OrderCommon.CreditTokenUse && string.IsNullOrEmpty(this.WciCardInputs.WhfCreditToken.Value) && (this.IsCreditCardLinkPayment() == false))
		{
			if (this.WciCardInputs.WspanErrorMessageForCreditCard.HasInnerControl)
			{
				this.WciCardInputs.WspanErrorMessageForCreditCard.InnerHtml = WebSanitizer.HtmlEncodeChangeToBr(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CARDAUTH_ERROR));
				this.WciCardInputs.WspanErrorMessageForCreditCard.InnerControl.Style["display"] = "block";
			}
			return;
		}

		// 画面遷移
		if (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Rakuten)
		{
			Session[Constants.SESSION_KEY_PARAM] = creditCardInput;
		}
		else if ((this.CreditCardSession == null)
				|| (this.CreditCardSession.CreditToken == null)
				|| (this.CreditCardSession.CreditToken.Token != creditCardInput.CreditToken.Token))
		{
			Session[Constants.SESSION_KEY_PARAM] = creditCardInput;
		}

		if (IsCreditCardLinkPayment())
		{
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_PAYMENT_ZEUS_LINK_POINT_GET_NOTICE;

			var url = new StringBuilder();
			url.Append(this.SecurePageProtocolAndHost).Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_PAYMENT_ZEUS_LINK_POINT_POST);
			url.Append("?").Append(Constants.REQUEST_KEY_PAYMENTCREDITCARD_ACTION_TYPE).Append("=").Append(Constants.ActionTypes.RegisterUserCreditCard);
			Response.Redirect(url.ToString());
		}
		else
		{
			// ターゲットページ設定
			this.SessionParamTargetPage = Constants.PAGE_FRONT_USER_CREDITCARD_CONFIRM;

			// クレジット登録確認画面遷移
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_CREDITCARD_CONFIRM);
		}
	}

	/// <summary>
	/// 更新の場合
	/// </summary>
	private void Update()
	{
		var creditCard = new UserCreditCardService().Get(this.LoginUserId, this.BranchNo);

		// パラメタ格納
		var creditCardInput = new UserCreditCardInput
		{
			UserId = this.LoginUserId,
			BranchNo = creditCard.BranchNo.ToString(),
			CardDispName = this.WciCardInputs.WtbUserCreditCardName.Text.Trim(),
		};

		// 入力チェック
		var errorMessages = creditCardInput.ValidateForFrontUserCreditCardRegist();
		if (errorMessages.Count != 0)
		{
			// カスタムバリデータ取得
			List<CustomValidator> lCustomValidators = new List<CustomValidator>();
			CreateCustomValidators(this, lCustomValidators);

			// エラーをカスタムバリデータへ
			SetControlViewsForError("UserCreditCardRegist", errorMessages, lCustomValidators);

			// クレジットカード登録名のみ表示
			this.WdivRakutenCredit.Visible = false;
			this.WdivCreditCardNoToken.Visible = false;
			this.WdivCreditCardForTokenAcquired.Visible = false;

			return;
		}

		// 画面遷移
		Session[Constants.SESSION_KEY_PARAM] = creditCardInput;
		// ターゲットページ設定
		this.SessionParamTargetPage = Constants.PAGE_FRONT_USER_CREDITCARD_CONFIRM;

		// クレジット登録確認画面遷移
		var sbNextUrl = new UrlCreator(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_CREDITCARD_CONFIRM)
			.AddParam(Constants.REQUEST_KEY_CREDITCARD_NO, creditCardInput.BranchNo)
			.CreateUrl();

		Response.Redirect(sbNextUrl);
	}

	/// <summary>
	/// （トークン決済向け）カード情報編集リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbEditCreditCardNoForToken_Click(object sender, EventArgs e)
	{
		// トークンなどクレジットカード情報削除
		ResetCreditTokenInfoFromForm(null, Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING);
		// セッションも削除
		Session[Constants.SESSION_KEY_PARAM] = null;

		RefreshCreditForm();
	}

	/// <summary>
	/// クレジットフォームをリフレッシュ
	/// </summary>
	private void RefreshCreditForm()
	{
		var isRakutenCredit = Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten;
		this.WdivRakutenCredit.Visible = isRakutenCredit;
		this.WdivCreditCardNoToken.Visible = (isRakutenCredit == false) && (HasCreditToken() == false);
		this.WdivCreditCardForTokenAcquired.Visible = (isRakutenCredit == false) && HasCreditToken();
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
		var script = CreateGetCardInfoJsScriptForCreditTokenInner();
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

	/// <summary>カード枝番(新規登録/異常なパラメタのときは0)</summary>
	public int BranchNo
	{
		get
		{
			var branchNo = 0;
			var isNumber = int.TryParse(Request.QueryString[Constants.REQUEST_KEY_CREDITCARD_NO], out branchNo);
			return ((branchNo > 0 && isNumber) ? branchNo : 0);
		}
	}
	/// <summary>セッションのクレジットカード情報</summary>
	private UserCreditCardInput CreditCardSession
	{
		get
		{
			var result
				= (Session[Constants.SESSION_KEY_PARAM] is UserCreditCardInput)
					? (UserCreditCardInput)Session[Constants.SESSION_KEY_PARAM]
					: null;
			return result;
		}
	}
	/// <summary>キャプチャ認証コントロール</summary>
	private CaptchaControl Captcha
	{
		get
		{
			var captcha = this.Master.FindControl("ContentPlaceHolder1").FindControl("ucCaptcha");
			return ((captcha != null) && (this.BranchNo == 0)) ? (CaptchaControl)captcha : null;
		}
	}
	/// <summary>支払回数</summary>
	protected ListItemCollection CreditInstallmentsList
	{
		get
		{
			var installments = new ListItemCollection();
			installments.AddRange(ValueText.GetValueItemArray(
				Constants.TABLE_ORDER,
				OrderCommon.CreditInstallmentsValueTextFieldName));
			return installments;
		}
	}
}
