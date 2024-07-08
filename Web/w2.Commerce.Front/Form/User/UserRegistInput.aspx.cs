/*
=========================================================================================================
  Module      : 会員登録入力画面処理(UserRegistInput.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using w2.App.Common.Amazon;
using w2.App.Common.Amazon.Helper;
using w2.App.Common.AmazonCv2;
using w2.App.Common.Auth.RakutenIDConnect;
using w2.App.Common.Auth.RakutenIDConnect.Helper;
using w2.App.Common.Global;
using w2.App.Common.Global.Region;
using w2.App.Common.Option;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.User;
using w2.App.Common.User.SocialLogin.Helper;
using w2.App.Common.Util;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.UserBusinessOwner;

public partial class Form_User_UserRegistInput : UserPage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス

	#region ラップ済コントロール宣言
	protected WrappedTextBox WtbUserName1 { get { return GetWrappedControl<WrappedTextBox>("tbUserName1"); } }
	protected WrappedTextBox WtbUserName2 { get { return GetWrappedControl<WrappedTextBox>("tbUserName2"); } }
	protected WrappedTextBox WtbUserNameKana1 { get { return GetWrappedControl<WrappedTextBox>("tbUserNameKana1"); } }
	protected WrappedTextBox WtbUserNameKana2 { get { return GetWrappedControl<WrappedTextBox>("tbUserNameKana2"); } }
	protected WrappedTextBox WtbUserNickName { get { return GetWrappedControl<WrappedTextBox>("tbUserNickName"); } }
	protected WrappedDropDownList WddlUserBirthYear { get { return GetWrappedControl<WrappedDropDownList>("ddlUserBirthYear"); } }
	protected WrappedDropDownList WddlUserBirthMonth { get { return GetWrappedControl<WrappedDropDownList>("ddlUserBirthMonth"); } }
	protected WrappedDropDownList WddlUserBirthDay { get { return GetWrappedControl<WrappedDropDownList>("ddlUserBirthDay"); } }
	protected WrappedRadioButtonList WrblUserSex { get { return GetWrappedControl<WrappedRadioButtonList>("rblUserSex", Constants.FLG_USER_SEX_UNKNOWN); } }
	protected WrappedTextBox WtbUserMailAddr { get { return GetWrappedControl<WrappedTextBox>("tbUserMailAddr"); } }
	protected WrappedTextBox WtbUserMailAddrConf { get { return GetWrappedControl<WrappedTextBox>("tbUserMailAddrConf"); } }
	protected WrappedTextBox WtbUserMailAddr2 { get { return GetWrappedControl<WrappedTextBox>("tbUserMailAddr2"); } }
	protected WrappedTextBox WtbUserMailAddr2Conf { get { return GetWrappedControl<WrappedTextBox>("tbUserMailAddr2Conf"); } }
	protected WrappedTextBox WtbUserZipGlobal { get { return GetWrappedControl<WrappedTextBox>("tbUserZipGlobal"); } }
	protected WrappedTextBox WtbUserZip1 { get { return GetWrappedControl<WrappedTextBox>("tbUserZip1"); } }
	protected WrappedTextBox WtbUserZip2 { get { return GetWrappedControl<WrappedTextBox>("tbUserZip2"); } }
	protected WrappedDropDownList WddlUserCountry { get { return GetWrappedControl<WrappedDropDownList>("ddlUserCountry"); } }
	protected WrappedDropDownList WddlUserAddr1 { get { return GetWrappedControl<WrappedDropDownList>("ddlUserAddr1"); } }
	protected WrappedTextBox WtbUserAddr2 { get { return GetWrappedControl<WrappedTextBox>("tbUserAddr2"); } }
	protected WrappedTextBox WtbUserAddr3 { get { return GetWrappedControl<WrappedTextBox>("tbUserAddr3"); } }
	protected WrappedTextBox WtbUserAddr4 { get { return GetWrappedControl<WrappedTextBox>("tbUserAddr4"); } }
	protected WrappedDropDownList WddlUserAddr5 { get { return GetWrappedControl<WrappedDropDownList>("ddlUserAddr5"); } }
	protected WrappedTextBox WtbUserAddr5 { get { return GetWrappedControl<WrappedTextBox>("tbUserAddr5"); } }
	protected WrappedTextBox WtbUserCompanyName { get { return GetWrappedControl<WrappedTextBox>("tbUserCompanyName"); } }
	protected WrappedTextBox WtbUserCompanyPostName { get { return GetWrappedControl<WrappedTextBox>("tbUserCompanyPostName"); } }
	protected WrappedTextBox WtbUserTel1 { get { return GetWrappedControl<WrappedTextBox>("tbUserTel1"); } }
	protected WrappedTextBox WtbUserTel2 { get { return GetWrappedControl<WrappedTextBox>("tbUserTel2"); } }
	protected WrappedTextBox WtbUserTel3 { get { return GetWrappedControl<WrappedTextBox>("tbUserTel3"); } }
	protected WrappedTextBox WtbUserTel1Global { get { return GetWrappedControl<WrappedTextBox>("tbUserTel1Global"); } }
	protected WrappedTextBox WtbUserTel2_1 { get { return GetWrappedControl<WrappedTextBox>("tbUserTel2_1"); } }
	protected WrappedTextBox WtbUserTel2_2 { get { return GetWrappedControl<WrappedTextBox>("tbUserTel2_2"); } }
	protected WrappedTextBox WtbUserTel2_3 { get { return GetWrappedControl<WrappedTextBox>("tbUserTel2_3"); } }
	protected WrappedTextBox WtbUserTel2Global { get { return GetWrappedControl<WrappedTextBox>("tbUserTel2Global"); } }
	protected WrappedCheckBox WcbUserMailFlg { get { return GetWrappedControl<WrappedCheckBox>("cbUserMailFlg", false); } }
	protected WrappedTextBox WtbUserLoginId { get { return GetWrappedControl<WrappedTextBox>(Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED ? "tbUserMailAddr" : "tbUserLoginId"); } }
	protected WrappedTextBox WtbUserPassword { get { return GetWrappedControl<WrappedTextBox>("tbUserPassword"); } }
	protected WrappedTextBox WtbUserPasswordConf { get { return GetWrappedControl<WrappedTextBox>("tbUserPasswordConf"); } }

	protected WrappedCustomValidator WcvUserName1 { get { return GetWrappedControl<WrappedCustomValidator>("cvUserName1"); } }
	protected WrappedCustomValidator WcvUserName2 { get { return GetWrappedControl<WrappedCustomValidator>("cvUserName2"); } }
	protected WrappedCustomValidator WcvUserNameKana1 { get { return GetWrappedControl<WrappedCustomValidator>("cvUserNameKana1"); } }
	protected WrappedCustomValidator WcvUserNameKana2 { get { return GetWrappedControl<WrappedCustomValidator>("cvUserNameKana2"); } }
	protected WrappedCustomValidator WcvUserBirthYear { get { return GetWrappedControl<WrappedCustomValidator>("cvUserBirthYear"); } }
	protected WrappedCustomValidator WcvUserBirthMonth { get { return GetWrappedControl<WrappedCustomValidator>("cvUserBirthMonth"); } }
	protected WrappedCustomValidator WcvUserBirthDay { get { return GetWrappedControl<WrappedCustomValidator>("cvUserBirthDay"); } }
	protected WrappedCustomValidator WcvUserSex { get { return GetWrappedControl<WrappedCustomValidator>("cvUserSex"); } }
	protected WrappedCustomValidator WcvUserMailAddrForCheck { get { return GetWrappedControl<WrappedCustomValidator>("cvUserMailAddrForCheck"); } }
	protected WrappedCustomValidator WcvUserMailAddr { get { return GetWrappedControl<WrappedCustomValidator>("cvUserMailAddr"); } }
	protected WrappedCustomValidator WcvUserMailAddrConf { get { return GetWrappedControl<WrappedCustomValidator>("cvUserMailAddrConf"); } }
	protected WrappedCustomValidator WcvUserMailAddr2 { get { return GetWrappedControl<WrappedCustomValidator>("cvUserMailAddr2"); } }
	protected WrappedCustomValidator WcvUserMailAddr2Conf { get { return GetWrappedControl<WrappedCustomValidator>("cvUserMailAddr2Conf"); } }
	protected WrappedCustomValidator WcvUserZip1 { get { return GetWrappedControl<WrappedCustomValidator>("cvUserZip1"); } }
	protected WrappedCustomValidator WcvUserZip2 { get { return GetWrappedControl<WrappedCustomValidator>("cvUserZip2"); } }
	protected WrappedCustomValidator WcvUserAddr1 { get { return GetWrappedControl<WrappedCustomValidator>("cvUserAddr1"); } }
	protected WrappedCustomValidator WcvUserAddr2 { get { return GetWrappedControl<WrappedCustomValidator>("cvUserAddr2"); } }
	protected WrappedCustomValidator WcvUserAddr3 { get { return GetWrappedControl<WrappedCustomValidator>("cvUserAddr3"); } }
	protected WrappedCustomValidator WcvUserAddr4 { get { return GetWrappedControl<WrappedCustomValidator>("cvUserAddr4"); } }
	protected WrappedCustomValidator WcvUserTel1 { get { return GetWrappedControl<WrappedCustomValidator>("cvUserTel1"); } }
	protected WrappedCustomValidator WcvUserTel2 { get { return GetWrappedControl<WrappedCustomValidator>("cvUserTel2"); } }
	protected WrappedCustomValidator WcvUserTel3 { get { return GetWrappedControl<WrappedCustomValidator>("cvUserTel3"); } }
	protected WrappedCustomValidator WcvUserTel2_1 { get { return GetWrappedControl<WrappedCustomValidator>("cvUserTel2_1"); } }
	protected WrappedCustomValidator WcvUserTel2_2 { get { return GetWrappedControl<WrappedCustomValidator>("cvUserTel2_2"); } }
	protected WrappedCustomValidator WcvUserTel2_3 { get { return GetWrappedControl<WrappedCustomValidator>("cvUserTel2_3"); } }
	protected WrappedCustomValidator WcvUserLoginId { get { return GetWrappedControl<WrappedCustomValidator>(Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED ? "cvUserMailAddr" : "cvUserLoginId"); } }
	protected WrappedCustomValidator WcvUserPassword { get { return GetWrappedControl<WrappedCustomValidator>("cvUserPassword"); } }
	protected WrappedCustomValidator WcvUserPasswordConf { get { return GetWrappedControl<WrappedCustomValidator>("cvUserPasswordConf"); } }
	protected WrappedCustomValidator WcvAuthenticationCode { get { return GetWrappedControl<WrappedCustomValidator>("cvAuthenticationCode"); } }
	protected WrappedCustomValidator WcvAuthenticationCodeGlobal { get { return GetWrappedControl<WrappedCustomValidator>("cvAuthenticationCodeGlobal"); } }

	protected WrappedHiddenField WhfSocialLoginJson { get { return GetWrappedControl<WrappedHiddenField>("hfSocialLoginJson"); } }
	protected WrappedHiddenField WhfAmazonOrderRefID { get { return GetWrappedControl<WrappedHiddenField>("hfAmazonOrderRefID"); } }
	protected WrappedHtmlGenericControl WhgcAddressBookWidgetDiv { get { return GetWrappedControl<WrappedHtmlGenericControl>("addressBookWidgetDiv"); } }
	protected WrappedHtmlGenericControl WhgcAddressBookErrorMessage { get { return GetWrappedControl<WrappedHtmlGenericControl>("addressBookErrorMessage"); } }
	protected WrappedHtmlGenericControl WhgcCountryAlertMessage { get { return GetWrappedControl<WrappedHtmlGenericControl>("countryAlertMessage"); } }
	protected WrappedLinkButton WlbConfirm { get { return GetWrappedControl<WrappedLinkButton>("lbConfirm"); } }
	protected WrappedDropDownList WddlUserAddr2 { get { return GetWrappedControl<WrappedDropDownList>("ddlUserAddr2"); } }
	protected WrappedDropDownList WddlUserAddr3 { get { return GetWrappedControl<WrappedDropDownList>("ddlUserAddr3"); } }
	
	protected WrappedLinkButton WlbSearchAddr { get { return GetWrappedControl<WrappedLinkButton>("lbSearchAddr"); } }
	protected WrappedTextBox WtbUserZip { get { return GetWrappedControl<WrappedTextBox>("tbUserZip"); } }
	protected WrappedTextBox WtbUserTel1_1 { get { return GetWrappedControl<WrappedTextBox>("tbUserTel1_1"); } }
	protected WrappedTextBox WtbUserTel1_2 { get { return GetWrappedControl<WrappedTextBox>("tbUserTel1_2"); } }
	protected WrappedButton WdbtnUserPasswordReType { get { return GetWrappedControl<WrappedButton>("btnUserPasswordReType"); } }
	protected WrappedLiteral WdlUserPassword { get { return GetWrappedControl<WrappedLiteral>("lUserPassword"); } }
	protected WrappedLiteral WdlUserPasswordConf { get { return GetWrappedControl<WrappedLiteral>("lUserPasswordConf"); } }
	protected WrappedHtmlGenericControl WhgcReTypeHiddenDiv1 { get { return GetWrappedControl<WrappedHtmlGenericControl>("reTypeHiddenDiv1"); } }
	protected WrappedHtmlGenericControl WhgcReTypeHiddenDiv2 { get { return GetWrappedControl<WrappedHtmlGenericControl>("reTypeHiddenDiv2"); } }

	protected WrappedLinkButton WlbSearchAddrFromZipGlobal { get { return GetWrappedControl<WrappedLinkButton>("lbSearchAddrFromZipGlobal"); } }

	/// <summary>Wrapped text box authentication code</summary>
	protected WrappedTextBox WtbAuthenticationCode { get { return GetWrappedControl<WrappedTextBox>("tbAuthenticationCode"); } }
	/// <summary>Wrapped link button get authentication code</summary>
	protected WrappedLinkButton WlbGetAuthenticationCode { get { return GetWrappedControl<WrappedLinkButton>("lbGetAuthenticationCode"); } }
	/// <summary>Wrapped link button check authentication code</summary>
	protected WrappedLinkButton WlbCheckAuthenticationCode { get { return GetWrappedControl<WrappedLinkButton>("lbCheckAuthenticationCode"); } }
	/// <summary>Wrapped label authentication status</summary>
	protected WrappedLabel WlbAuthenticationStatus { get { return GetWrappedControl<WrappedLabel>("lbAuthenticationStatus"); } }
	/// <summary>Wrapped label has authentication</summary>
	protected WrappedLabel WlbHasAuthentication { get { return GetWrappedControl<WrappedLabel>("lbHasAuthentication"); } }
	/// <summary>Wrapped label authentication message</summary>
	protected WrappedLabel WlbAuthenticationMessage { get { return GetWrappedControl<WrappedLabel>("lbAuthenticationMessage"); } }
	/// <summary>Wrapped text box authentication code global</summary>
	protected WrappedTextBox WtbAuthenticationCodeGlobal { get { return GetWrappedControl<WrappedTextBox>("tbAuthenticationCodeGlobal"); } }
	/// <summary>Wrapped link button get authentication code global</summary>
	protected WrappedLinkButton WlbGetAuthenticationCodeGlobal { get { return GetWrappedControl<WrappedLinkButton>("lbGetAuthenticationCodeGlobal"); } }
	/// <summary>Wrapped label authentication status global</summary>
	protected WrappedLabel WlbAuthenticationStatusGlobal { get { return GetWrappedControl<WrappedLabel>("lbAuthenticationStatusGlobal"); } }
	/// <summary>Wrapped label has authentication global</summary>
	protected WrappedLabel WlbHasAuthenticationGlobal { get { return GetWrappedControl<WrappedLabel>("lbHasAuthenticationGlobal"); } }
	/// <summary>Wrapped label authentication message global</summary>
	protected WrappedLabel WlbAuthenticationMessageGlobal { get { return GetWrappedControl<WrappedLabel>("lbAuthenticationMessageGlobal"); } }
	/// <summary>Wrapped hidden field reset authentication code</summary>
	protected WrappedHiddenField WhfResetAuthenticationCode { get { return GetWrappedControl<WrappedHiddenField>("hfResetAuthenticationCode"); } }
	/// <summary>Wrapped customvalidator user tel 1 global</summary>
	protected WrappedCustomValidator WcvUserTel1Global { get { return GetWrappedControl<WrappedCustomValidator>("cvUserTel1Global"); } }

	//user business owner
	protected WrappedCheckBox WcbIsBusinessOwner { get { return GetWrappedControl<WrappedCheckBox>("IsBusinessOwner", false); } }
	protected WrappedTextBox wtbOwnerName1 { get { return GetWrappedControl<WrappedTextBox>("tbOwnerName1"); } }
	protected WrappedTextBox wtbOwnerName2 { get { return GetWrappedControl<WrappedTextBox>("tbOwnerName2"); } }
	protected WrappedTextBox wtbOwnerNameKana1 { get { return GetWrappedControl<WrappedTextBox>("tbOwnerNameKana1"); } }
	protected WrappedTextBox wtbOwnerNameKana2 { get { return GetWrappedControl<WrappedTextBox>("tbOwnerNameKana2"); } }
	protected WrappedDropDownList WddlOwnerBirthYear { get { return GetWrappedControl<WrappedDropDownList>("ddlOwnerBirthYear"); } }
	protected WrappedDropDownList WddlOwnerBirthMonth { get { return GetWrappedControl<WrappedDropDownList>("ddlOwnerBirthMonth"); } }
	protected WrappedDropDownList WddlOwnerBirthDay { get { return GetWrappedControl<WrappedDropDownList>("ddlOwnerBirthDay"); } }
	protected WrappedTextBox WtbRequestBudget { get { return GetWrappedControl<WrappedTextBox>("tbRequestBudget"); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		// HTTPS通信チェック（HTTPのとき、規約画面へ）
		CheckHttps(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_REGIST_REGULATION);

		// 楽天IDConnectで新規会員登録ボタン以外でこのページに来たらセッションから楽天の情報を消す
		if (Constants.RAKUTEN_LOGIN_ENABLED
			&& (Request[Constants.REQUEST_KEY_RAKUTEN_REGIST] != Constants.FLG_RAKUTEN_USER_REGIST))
		{
			SessionManager.RakutenIdConnectActionInfo = null;
		}

		// ログインチェック（ログイン済みの場合、トップ画面へ）
		if (this.IsLoggedIn && (SessionManager.HasTemporaryUserId == false))
		{
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT);
		}

		//カスタムバリデータ属性値更新
		UpdateAttributeValueForCustomValidator();

		if (!IsPostBack)
		{
			// コンポーネント初期化
			InitComponents();

			// デフォルト値セット
			SetDefaultToForm();

			// Amazonログインオプションが有効でない場合はセッションのAmazonアカウント情報を破棄
			if (Constants.AMAZON_LOGIN_OPTION_ENABLED == false)
			{
				Session.Remove(AmazonConstants.SESSION_KEY_AMAZON_MODEL);
			}

			if (this.SessionParamTargetPage == Constants.PAGE_FRONT_USER_REGIST_INPUT)
			{
				// セッション情報から会員情報を入力欄にセットする
				var userInput = (UserInput)Session[Constants.SESSION_KEY_PARAM];

				var userExted = userInput.UserExtendInput;
				userExted.UserExtendDataValue[w2.App.Common.Constants.SOCIAL_PROVIDER_ID_LINE] = SessionManager.LineProviderUserId;
				userInput.UserExtendInput = userExted;
				var user = userInput.CreateModel();
				this.DisplayUserInfo(user);

				//GMO
				if (userInput.BusinessOwner != null)
				{
					this.WcbIsBusinessOwner.Checked = true;
					this.wtbOwnerName1.Text = userInput.BusinessOwner.OwnerName1;
					this.wtbOwnerName2.Text = userInput.BusinessOwner.OwnerName2;
					this.wtbOwnerNameKana1.Text = userInput.BusinessOwner.OwnerNameKana1;
					this.wtbOwnerNameKana2.Text = userInput.BusinessOwner.OwnerNameKana2;
					this.WtbRequestBudget.Text = userInput.BusinessOwner.RequestBudget;
					this.WddlOwnerBirthYear.SelectedValue = userInput.BusinessOwner.BirthYear;
					this.WddlOwnerBirthMonth.SelectedValue = userInput.BusinessOwner.BirthMonth;
					this.WddlOwnerBirthDay.SelectedValue = userInput.BusinessOwner.BirthDay;
				}
				else
				{
					this.WcbIsBusinessOwner.Checked = false;
				}

				this.WtbUserLoginId.Text = user.LoginId;

				//パスワード入力部分の表示非表示の切り替え
				if ((string.IsNullOrEmpty(user.PasswordDecrypted) == false)
					&& WdbtnUserPasswordReType.HasInnerControl)
				{
					this.WtbUserPassword.Visible = false;
					this.WtbUserPasswordConf.Visible = false;

					var hiddenPassword = StringUtility.ChangeToAster(user.PasswordDecrypted);

					this.WdlUserPassword.Text = hiddenPassword;
					this.WdlUserPasswordConf.Text = hiddenPassword;

					WdbtnUserPasswordReType.Visible = true;
					WhgcReTypeHiddenDiv1.Visible = false;
					WhgcReTypeHiddenDiv2.Visible = false;
				}
				this.WtbUserPassword.Text = user.PasswordDecrypted;
				this.WtbUserPasswordConf.Text = user.PasswordDecrypted;

				if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED)
				{
					this.HasAuthenticationCode = userInput.HasAuthenticationCode;

					if (this.HasAuthenticationCode)
					{
						DisplayAuthenticationCode(
							this.IsUserAddrJp ? this.WlbGetAuthenticationCode : this.WlbGetAuthenticationCodeGlobal,
							this.IsUserAddrJp ? this.WtbAuthenticationCode : this.WtbAuthenticationCodeGlobal,
							authenticationCode : userInput.AuthenticationCode);
					}
				}

				// ターゲットページ設定
				this.SessionParamTargetPage = null;
			}
			// 注文完了後、会員登録の場合はユーザー情報を復元
			else if (Session[Constants.SESSION_KEY_USER_REGIST_AFTER_ORDER_USER_ID] != null)
			{
				// ユーザーIDからユーザー情報の取得し、入力欄にセットする
				var user = new UserService().Get((string)Session[Constants.SESSION_KEY_USER_REGIST_AFTER_ORDER_USER_ID]);
				if (user != null)
				{
					this.DisplayUserInfo(user);

					//GMO
					var userBusinessOwner = new UserBusinessOwnerService().GetByUserId((string)Session[Constants.SESSION_KEY_USER_REGIST_AFTER_ORDER_USER_ID]);
					if (userBusinessOwner != null)
					{
						this.WcbIsBusinessOwner.Checked = true;
						this.wtbOwnerName1.Text = userBusinessOwner.OwnerName1;
						this.wtbOwnerName2.Text = userBusinessOwner.OwnerName2;
						this.wtbOwnerNameKana1.Text = userBusinessOwner.OwnerNameKana1;
						this.wtbOwnerNameKana2.Text = userBusinessOwner.OwnerNameKana2;
						this.WtbRequestBudget.Text = userBusinessOwner.RequestBudget.ToString();
						var birth = DateTime.Parse(userBusinessOwner.Birth.ToString());
						this.WddlOwnerBirthYear.SelectedValue = birth.Year.ToString();
						this.WddlOwnerBirthMonth.SelectedValue = birth.Month.ToString();
						this.WddlOwnerBirthDay.SelectedValue = birth.Day.ToString();
					}
					else
					{
						this.WcbIsBusinessOwner.Checked = false;
					}
					// Set authentication for user register after order
					if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED)
					{
						this.HasAuthenticationCode = false;
					}
				}
			}
			else if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED)
			{
				this.HasAuthenticationCode = false;
			}

			// 国切替初期化
			ddlUserAddrCountry_SelectedIndexChanged(sender, e);
			Session[Constants.SESSION_KEY_PARAM_FOR_USER_BUSINESS_OWNER_INPUT] = null;

			// 確認画面から遷移時の処理
			if ((Request.UrlReferrer != null) && Request.UrlReferrer.AbsolutePath.Contains(Constants.PAGE_FRONT_USER_REGIST_CONFIRM))
			{
				// ユーザー情報取得
				var userInfo = (UserInput)Session[Constants.SESSION_KEY_PARAM];
				var errorMessages = ValidateUserInfo(userInfo);

				GetErrMsgAndFocusToCV(errorMessages);
			}
		}

		// Amazonログインボタンペイロード取得
		if (Constants.AMAZON_LOGIN_OPTION_ENABLED && Constants.AMAZON_PAYMENT_CV2_ENABLED)
		{
			var callbackPath = new UrlCreator(Constants.PAGE_FRONT_AMAZON_LOGIN_CALLBACK)
				.AddParam(
					AmazonConstants.REQUEST_KEY_AMAZON_STATE,
					Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_REGIST_INPUT)
				.CreateUrl();
			this.AmazonRequest = AmazonCv2Redirect.SignPayloadForSignIn(callbackPath);
		}
		// apsx側プロパティセットしているため、バインドを行う
		if (this.Captcha != null)
		{
			this.Captcha.DataBind();
		}
	}

	/// <summary>
	/// デフォルト値セット
	/// </summary>
	private void SetDefaultToForm()
	{
		// 年月日をデフォルトにセットする
		var defaultBirthday = DateTimeUtility.GetDefaultSettingBirthday();
		this.WddlUserBirthYear.SelectedValue = (defaultBirthday != null) ? defaultBirthday.Value.Year.ToString() : "";
		this.WddlUserBirthMonth.SelectedValue = (defaultBirthday != null) ? defaultBirthday.Value.Month.ToString() : "";
		this.WddlUserBirthDay.SelectedValue = (defaultBirthday != null) ? defaultBirthday.Value.Day.ToString() : "";

		// リプレースタグよりデフォルト値指定
		this.WrblUserSex.SelectedValue = Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.sex.default@@");
		this.WcbUserMailFlg.Checked = (Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.mail_flg.default@@") == Constants.FLG_USER_MAILFLG_OK);

		this.IsVisible_UserPassword = true;

		this.WddlUserCountry.SelectedValue = RegionManager.GetInstance().Region.CountryIsoCode;

		// ソーシャルログイン情報補完
		if (Constants.SOCIAL_LOGIN_ENABLED)
		{
			// 会員規約画面or確認画面からの遷移、会員登録入力画面からソーシャルログインボタンを押下して遷移した場合のみソーシャルログインセッションを保持
			var socialLogin = (SocialLoginModel)Session[Constants.SESSION_KEY_SOCIAL_LOGIN_MODEL];
			if ((socialLogin != null)
				&& ((socialLogin.TransitionSourcePath.EndsWith(Constants.PAGE_FRONT_USER_REGIST_REGULATION))
					|| (socialLogin.TransitionSourcePath.EndsWith(Constants.PAGE_FRONT_USER_REGIST_CONFIRM))
					|| (socialLogin.TransitionSourcePath.EndsWith(Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_REGIST_INPUT_CALLBACK))
					|| (socialLogin.TransitionSourcePath.EndsWith(Constants.PAGE_FRONT_USER_REGIST_INPUT))))
			{
				this.WhfSocialLoginJson.Value = socialLogin.RawResponse;
				this.IsVisible_UserPassword = false;
				socialLogin.TransitionSourcePath = this.AppRelativeVirtualPath;
			}
			else
			{
				// セッション情報のクリア
				 Session.Remove(Constants.SESSION_KEY_SOCIAL_LOGIN_MODEL);
			}
		}
		// Amazonログイン情報補完
		if (this.IsAmazonLoggedIn && Constants.AMAZON_LOGIN_OPTION_ENABLED)
		{
			var amazonModel = (AmazonModel)Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL];
			this.WtbUserName1.Text = amazonModel.GetName1();
			this.WtbUserName2.Text = amazonModel.GetName2();
			this.WtbUserMailAddr.Text = amazonModel.Email;
			this.WtbUserMailAddrConf.Text = amazonModel.Email;
			this.IsVisible_UserPassword = false;

			if (Constants.AMAZON_PAYMENT_CV2_ENABLED)
			{
				var buyer = new AmazonCv2ApiFacade().GetBuyer(amazonModel.Token);
				if (buyer.BillingAddress != null)
				{
					this.WtbUserZip1.Text = buyer.BillingAddress.PostalCode.Substring(0, 3);
					this.WtbUserZip2.Text = buyer.BillingAddress.PostalCode.Substring(4, 4);
					this.WddlUserAddr1.SelectItemByValue(buyer.BillingAddress.StateOrRegion);
					this.WtbUserAddr2.Text = buyer.BillingAddress.AddressLine1;
					this.WtbUserAddr3.Text = buyer.BillingAddress.AddressLine2;
					this.WtbUserAddr4.Text = buyer.BillingAddress.AddressLine3;
					var phoneNumber = buyer.BillingAddress.PhoneNumber;
					this.WtbUserTel1.Text = phoneNumber.Substring(0, phoneNumber.Length - 8);
					this.WtbUserTel2.Text = phoneNumber.Substring(phoneNumber.Length - 8, 4);
					this.WtbUserTel3.Text = phoneNumber.Substring(phoneNumber.Length - 4, 4);
				}
			}

			// 国をJapan固定にする
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				this.WddlUserCountry.SelectItemByValue(Constants.COUNTRY_ISO_CODE_JP);
				this.WddlUserCountry.InnerControl.Enabled = false;
				this.WhgcCountryAlertMessage.Visible = true;
			}
		}
		// ペイパル情報補完（パスワードは非表示に）
		if (SessionManager.PayPalLoginResult != null)
		{
			this.WtbUserName1.Text = SessionManager.PayPalLoginResult.AddressInfo.Name1;
			this.WtbUserName2.Text = SessionManager.PayPalLoginResult.AddressInfo.Name2;
			this.WtbUserMailAddr.Text = this.WtbUserMailAddrConf.Text = SessionManager.PayPalLoginResult.AddressInfo.MailAddr;
			this.WddlUserCountry.Items.Cast<ListItem>().ToList().ForEach(li => li.Selected = (li.Value == SessionManager.PayPalLoginResult.AddressInfo.ContryIsoCode));
			this.WtbUserZip1.Text = SessionManager.PayPalLoginResult.AddressInfo.Zip1;
			this.WtbUserZip2.Text = SessionManager.PayPalLoginResult.AddressInfo.Zip2;
			this.WddlUserAddr1.Items.Cast<ListItem>().ToList().ForEach(li => li.Selected = (li.Value == SessionManager.PayPalLoginResult.AddressInfo.Addr1));
			this.WtbUserAddr2.Text = SessionManager.PayPalLoginResult.AddressInfo.Addr2;
			this.WtbUserAddr3.Text = SessionManager.PayPalLoginResult.AddressInfo.Addr3;
			this.WtbUserAddr4.Text = SessionManager.PayPalLoginResult.AddressInfo.Addr4;
			this.WtbUserAddr5.Text = SessionManager.PayPalLoginResult.AddressInfo.Addr5;
			this.WtbUserTel1.Text = SessionManager.PayPalLoginResult.AddressInfo.Tel_1;
			this.WtbUserTel2.Text = SessionManager.PayPalLoginResult.AddressInfo.Tel_2;
			this.WtbUserTel3.Text = SessionManager.PayPalLoginResult.AddressInfo.Tel_3;
			this.IsVisible_UserPassword = false;
		}
		//楽天IDConnect情報補完
		if (this.IsRakutenIdConnectUserRegister)
		{
			this.IsVisible_UserPassword = false;

			var user = new RakutenIDConnectUser(SessionManager.RakutenIdConnectActionInfo);
			this.WtbUserName1.Text = user.FamilyName;
			this.WtbUserName2.Text = user.GivenName;
			this.WtbUserNameKana1.Text = user.FamilyNameKana;
			this.WtbUserNameKana2.Text = user.GivenNameKana;
			this.WtbUserNickName.Text = user.NickName;
			this.WddlUserBirthYear.SelectedValue = user.BirthYear;
			this.WddlUserBirthMonth.SelectedValue = user.BirthMonth;
			this.WddlUserBirthDay.SelectedValue = user.BirthDay;
			this.WrblUserSex.SelectedValue = user.Gender;
			this.WtbUserMailAddr.Text = user.Email;
			this.WtbUserMailAddrConf.Text = user.Email;
			this.WddlUserCountry.SelectedIndex = this.WddlUserCountry.Items.IndexOf(
				this.WddlUserCountry.Items.FindByText(user.Country));
			this.WtbUserZip1.Text = user.PostalCode1;
			this.WtbUserZip2.Text = user.PostalCode2;
			this.WddlUserAddr1.SelectedValue = user.Address1;
			this.WtbUserAddr2.Text = user.Address2;
			this.WtbUserAddr3.Text = user.Address3;
			this.WtbUserTel1.Text = user.PhoneNumber1;
			this.WtbUserTel2.Text = user.PhoneNumber2;
			this.WtbUserTel3.Text = user.PhoneNumber3;
			this.WtbUserTel1Global.Text = user.PhoneNumber;
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitComponents()
	{
		//------------------------------------------------------
		// コンポーネント初期化
		//------------------------------------------------------
		// 性別ラジオボタン設定
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_USER, Constants.FIELD_USER_SEX))
		{
			this.WrblUserSex.AddItem(li);
		}

		// お知らせメール希望
		this.WcbUserMailFlg.Checked = false;

		// 生年月日ドロップダウン作成
		this.WddlUserBirthYear.Items.Add("");
		this.WddlUserBirthYear.AddItems(DateTimeUtility.GetBirthYearListItem());
		this.WddlUserBirthYear.SelectedValue = (this.WddlUserBirthYear.InnerControl != null) ? "1970" : "";
		this.WddlUserBirthMonth.Items.Add("");
		this.WddlUserBirthMonth.AddItems(DateTimeUtility.GetMonthListItem());
		this.WddlUserBirthDay.Items.Add("");
		this.WddlUserBirthDay.AddItems(DateTimeUtility.GetDayListItem());

		// 生年月日ドロップダウン作成 GMO
		this.WddlOwnerBirthYear.Items.Add(string.Empty);
		this.WddlOwnerBirthYear.AddItems(DateTimeUtility.GetBirthYearListItem());
		this.WddlOwnerBirthYear.SelectedValue = (this.WddlUserBirthYear.InnerControl != null) ? "1970" : "";
		this.WddlOwnerBirthMonth.Items.Add(string.Empty);
		this.WddlOwnerBirthMonth.AddItems(DateTimeUtility.GetMonthListItem());
		this.WddlOwnerBirthDay.Items.Add(string.Empty);
		this.WddlOwnerBirthDay.AddItems(DateTimeUtility.GetDayListItem());

		// 都道府県ドロップダウン作成
		this.WddlUserAddr1.AddItem(new ListItem("", ""));
		foreach (string strPrefecture in Constants.STR_PREFECTURES_LIST)
		{
			this.WddlUserAddr1.AddItem(new ListItem(strPrefecture));
		}

		if (Constants.GLOBAL_OPTION_ENABLE == false) return;

		// 国ドロップダウン作成
		var countries = GlobalAddressUtil.GetCountriesAll();
		this.WddlUserCountry.Items.Add(new ListItem("", ""));
		this.WddlUserCountry.Items.AddRange(countries.Select(c => new ListItem(c.CountryName, c.CountryIsoCode)).ToArray());
		this.WddlUserCountry.SelectedValue = RegionManager.GetInstance().Region.CountryIsoCode;

		// 州ドロップダウン作成
		this.WddlUserAddr5.Items.Add(new ListItem("", ""));
		this.WddlUserAddr5.Items.AddRange(Constants.US_STATES_LIST.Select(state => new ListItem(state)).ToArray());

		this.WddlUserAddr2.Items.AddRange(this.AddrTwCityList);
	}

	/// <summary>
	/// 「住所検索」ボタン押下イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSearchAddr_Click(object sender, System.EventArgs e)
	{
		this.ZipInputErrorMessage = SearchZipCode(sender);
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

		if (this.IsAmazonLoggedIn && (Constants.AMAZON_PAYMENT_CV2_ENABLED == false))
		{
			if ((string)Session[AmazonConstants.SESSION_KEY_AMAZON_ADDRESS_ERROR_MSG] != null) return;
		}

		// ユーザー情報取得
		UserInput userInfo = CreateInputData();
		var errorMessages = ValidateUserInfo(userInfo);

		// エラーが無い場合
		if (errorMessages.Count == 0)
		{
			// 問題なければ次画面に遷移する
			GoNext(userInfo);
		}
		else
		{
			if (errorMessages.ContainsKey(Constants.FIELD_USER_ZIP))
			{
				this.ZipInputErrorMessage = string.Empty;
			}

			// バリデータチェックのエラー内容をバインド
			if (this.UserExtendUserControl != null)
			{
				((UserExtendUserControl)this.UserExtendUserControl).ErrorMsg = errorMessages;
				((UserExtendUserControl)this.UserExtendUserControl).SetErrMessage();
			}

			if (this.IsSocialLogin
				&& errorMessages.ContainsKey(Constants.CONST_KEY_USER_AUTHENTICATION_CODE))
			{
				this.HasAuthenticationCode = true;
				errorMessages.Remove(Constants.CONST_KEY_USER_AUTHENTICATION_CODE);
			}

			// カスタムバリデータにメッセージを表示させる
			// ユーザ拡張項目のバリデータチェックはUC側実施
			GetErrMsgAndFocusToCV(errorMessages);
		}
	}

	/// <summary>
	/// 次画面へ遷移
	/// </summary>
	private void GoNext(UserInput userInfo)
	{
		// 問題なければ次画面に遷移する
		Session[Constants.SESSION_KEY_PARAM] = userInfo;
		// 画面遷移の正当性チェックのため遷移先ページURLを設定
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_USER_REGIST_CONFIRM;

		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_REGIST_CONFIRM);
	}

	/// <summary>
	///  エラーメッセージをカスタムバリデータにセットしてフォーカス
	/// </summary>
	/// <param name="errorMsg">エラーメッセージ一覧</param>
	private void GetErrMsgAndFocusToCV(Dictionary<string, string> errorMsg)
	{
		// カスタムバリデータ取得
		List<CustomValidator> lCustomValidators = new List<CustomValidator>();
		CreateCustomValidators(this, lCustomValidators);

		// エラーをカスタムバリデータへ
		SetControlViewsForError("UserRegist", errorMsg, lCustomValidators);
		// エラーをカスタムバリデータへ
		SetControlViewsForError("UserRegistGlobal", errorMsg, lCustomValidators);

		// ログインIDチェック（メールアドレスをログインIDにするかどうかでメッセージ出力する場所が異なるのでここで指定）
		if (this.WcvUserLoginId.IsValid)
		{
			ChangeControlLooksForValidator(
				errorMsg,
				Constants.FIELD_USER_LOGIN_ID,
				this.WcvUserLoginId,
				this.WtbUserLoginId);
		}
		if (this.WcvUserLoginId.IsValid)
		{
			ChangeControlLooksForValidator(
				errorMsg,
				Constants.FIELD_USER_LOGIN_ID + "_input_check",
				this.WcvUserLoginId,
				this.WtbUserLoginId);
		}

		// メールアドレス必須チェック
		ChangeControlLooksForValidator(
			errorMsg,
			Constants.FIELD_USER_MAIL_ADDR + "_for_check",
			this.WcvUserMailAddrForCheck,
			this.WtbUserMailAddr);

		// エラーフォーカス
		// HACK:JSでおｋ（その方がお客さんも幸せでは？）
		foreach (CustomValidator cvCustomValidator in lCustomValidators)
		{
			WebControl wcTarget = (WebControl)cvCustomValidator.Parent.FindControl(cvCustomValidator.ControlToValidate);
			if (wcTarget != null)
			{
				if (cvCustomValidator.IsValid == false)
				{
					switch (wcTarget.ID)
					{
						// 下の方にあるので確認ボタンフォーカス
						case "tbUserLoginId":
						case "tbUserPassword":
						case "tbUserPasswordConf":
							lbConfirm.Focus();
							break;

						// ターゲットにフォーカス
						default:
							wcTarget.Focus();
							break;
					}
					break;
				}
				else
				{
					cvCustomValidator.ErrorMessage = "";
					wcTarget.CssClass = wcTarget.CssClass.Replace(Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING, " ");
				}
			}
		}
	}

	/// <summary>
	/// 入力値を取得してUserInputに格納
	/// </summary>
	private UserInput CreateInputData()
	{
		UserInput userInput = new UserInput(new UserModel());
		userInput.UserKbn = this.IsSmartPhone ? Constants.FLG_USER_USER_KBN_SMARTPHONE_USER : Constants.FLG_USER_USER_KBN_PC_USER;
		userInput.Name1 = DataInputUtility.ConvertToFullWidthBySetting(this.WtbUserName1.Text, this.IsUserAddrJp);
		userInput.Name2 = DataInputUtility.ConvertToFullWidthBySetting(this.WtbUserName2.Text, this.IsUserAddrJp);
		userInput.Name = userInput.Name1 + userInput.Name2;
		userInput.NameKana1 = StringUtility.ToZenkaku(this.WtbUserNameKana1.Text);
		userInput.NameKana2 = StringUtility.ToZenkaku(this.WtbUserNameKana2.Text);
		userInput.NameKana = userInput.NameKana1 + userInput.NameKana2;
		userInput.NickName = this.WtbUserNickName.Text;
		userInput.BirthYear = this.WddlUserBirthYear.SelectedValue;
		userInput.BirthMonth = this.WddlUserBirthMonth.SelectedValue;
		userInput.BirthDay = this.WddlUserBirthDay.SelectedValue;

		//GMO
		if (this.WcbIsBusinessOwner.Checked)
		{
			userInput.BusinessOwner.OwnerName1 = DataInputUtility.ConvertToFullWidthBySetting(this.wtbOwnerName1.Text, this.IsUserAddrJp);
			userInput.BusinessOwner.OwnerName2 = DataInputUtility.ConvertToFullWidthBySetting(this.wtbOwnerName2.Text, this.IsUserAddrJp);
			userInput.BusinessOwner.OwnerNameKana1 = StringUtility.ToZenkaku(this.wtbOwnerNameKana1.Text);
			userInput.BusinessOwner.OwnerNameKana2 = StringUtility.ToZenkaku(this.wtbOwnerNameKana2.Text);
			userInput.BusinessOwner.RequestBudget = (string.IsNullOrWhiteSpace(this.WtbRequestBudget.Text) ? "0" : this.WtbRequestBudget.Text);
			userInput.BusinessOwner.BirthYear = this.WddlOwnerBirthYear.SelectedValue;
			userInput.BusinessOwner.BirthMonth = this.WddlOwnerBirthMonth.SelectedValue;
			userInput.BusinessOwner.BirthDay = this.WddlOwnerBirthDay.SelectedValue;
			if ((string.IsNullOrWhiteSpace(WddlOwnerBirthYear.SelectedValue) == false) || 
				(string.IsNullOrWhiteSpace(WddlOwnerBirthMonth.SelectedValue) == false) || 
				(string.IsNullOrWhiteSpace(WddlOwnerBirthDay.SelectedValue) == false))
			{
				userInput.BusinessOwner.Birth = string.Format("{0}/{1}/{2}", this.WddlOwnerBirthYear.SelectedValue, this.WddlOwnerBirthMonth.SelectedValue, this.WddlOwnerBirthDay.SelectedValue);
			}
			else
			{
				userInput.BusinessOwner.Birth = null;
			}
		}
		else
		{
			userInput.BusinessOwner = null;
		}

		// どれか未入力の時は日付整合性チェックは行わない
		if ((WddlUserBirthYear.SelectedValue + WddlUserBirthMonth.SelectedValue + WddlUserBirthDay.SelectedValue).Length != 0)
		{
			userInput.Birth = this.WddlUserBirthYear.SelectedValue + "/" + this.WddlUserBirthMonth.SelectedValue + "/" + this.WddlUserBirthDay.SelectedValue;
		}
		else
		{
			userInput.Birth = null;
		}
		userInput.Sex = this.WrblUserSex.SelectedValue;
		userInput.MailAddr = StringUtility.ToHankaku(this.WtbUserMailAddr.Text);
		userInput.MailAddrConf = StringUtility.ToHankaku(this.WtbUserMailAddrConf.Text);
		userInput.MailAddr2 = StringUtility.ToHankaku(this.WtbUserMailAddr2.Text);
		userInput.MailAddr2Conf = StringUtility.ToHankaku(this.WtbUserMailAddr2Conf.Text);
		if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED)
		{
			// どちらか一方が入力されていること
			userInput.MailAddrForCheck = StringUtility.ToHankaku(this.WtbUserMailAddr.Text.Trim() + this.WtbUserMailAddr2.Text.Trim());
		}
		else
		{
			// PCアドレス必須
			userInput.MailAddrForCheck = StringUtility.ToHankaku(this.WtbUserMailAddr.Text.Trim());
		}

		// Set value for zip code
		var inputZipCode = (this.WtbUserZip1.HasInnerControl)
			? StringUtility.ToHankaku(this.WtbUserZip1.Text.Trim())
			: StringUtility.ToHankaku(this.WtbUserZip.Text.Trim());
		if (this.WtbUserZip1.HasInnerControl) inputZipCode += ("-" + StringUtility.ToHankaku(this.WtbUserZip2.Text.Trim()));
		var zipCode = new ZipCode(inputZipCode);
		userInput.Zip1 = zipCode.Zip1;
		userInput.Zip2 = zipCode.Zip2;
		userInput.Zip = (string.IsNullOrEmpty(zipCode.Zip) == false)
			? zipCode.Zip
			: inputZipCode;

		userInput.Addr1 = this.WddlUserAddr1.SelectedValue;
		userInput.Addr2 = DataInputUtility.ConvertToFullWidthBySetting(this.WtbUserAddr2.Text, this.IsUserAddrJp).Trim();
		userInput.Addr3 = DataInputUtility.ConvertToFullWidthBySetting(this.WtbUserAddr3.Text, this.IsUserAddrJp).Trim();
		userInput.Addr4 = DataInputUtility.ConvertToFullWidthBySetting(this.WtbUserAddr4.Text, this.IsUserAddrJp).Trim();
		userInput.CompanyName = this.WtbUserCompanyName.Text;
		userInput.CompanyPostName = this.WtbUserCompanyPostName.Text;

		// Set value for telephone 1
		var inputTel1 = (this.WtbUserTel1.HasInnerControl)
			? StringUtility.ToHankaku(this.WtbUserTel1.Text.Trim())
			: StringUtility.ToHankaku(this.WtbUserTel1_1.Text.Trim());
		if (this.WtbUserTel2.HasInnerControl)
		{
			inputTel1 = UserService.CreatePhoneNo(
				inputTel1,
				StringUtility.ToHankaku(this.WtbUserTel2.Text.Trim()),
				StringUtility.ToHankaku(this.WtbUserTel3.Text.Trim()));
		}
		var tel1 = new Tel(inputTel1);
		userInput.Tel1_1 = tel1.Tel1;
		userInput.Tel1_2 = tel1.Tel2;
		userInput.Tel1_3 = tel1.Tel3;
		userInput.Tel1 = (string.IsNullOrEmpty(tel1.TelNo) == false)
			? tel1.TelNo
			: inputTel1;

		// Set value for telephone 2
		var inputTel2 = (this.WtbUserTel2_1.HasInnerControl)
			? StringUtility.ToHankaku(this.WtbUserTel2_1.Text.Trim())
			: StringUtility.ToHankaku(this.WtbUserTel1_2.Text.Trim());
		if (this.WtbUserTel2_2.HasInnerControl)
		{
			inputTel2 = UserService.CreatePhoneNo(
				inputTel2,
				StringUtility.ToHankaku(this.WtbUserTel2_2.Text.Trim()),
				StringUtility.ToHankaku(this.WtbUserTel2_3.Text.Trim()));
		}
		var tel2 = new Tel(inputTel2);
		userInput.Tel2_1 = tel2.Tel1;
		userInput.Tel2_2 = tel2.Tel2;
		userInput.Tel2_3 = tel2.Tel3;
		userInput.Tel2 = (string.IsNullOrEmpty(tel2.TelNo) == false)
			? tel2.TelNo
			: inputTel2;

		userInput.MailFlg = this.WcbUserMailFlg.Checked ? Constants.FLG_USER_MAILFLG_OK : Constants.FLG_USER_MAILFLG_NG;
		userInput.LoginId = StringUtility.ToHankaku(this.WtbUserLoginId.Text);
		userInput.Password = this.IsVisible_UserPassword ? StringUtility.ToHankaku(this.WtbUserPassword.Text) : null;
		userInput.PasswordConf = this.IsVisible_UserPassword ? StringUtility.ToHankaku(this.WtbUserPasswordConf.Text) : null;
		userInput.AdvcodeFirst = StringUtility.ToEmpty(Session[Constants.SESSION_KEY_ADVCODE_NOW]);	// 広告コード格納（新規登録時のみ）
		userInput.RemoteAddr = this.Page.Request.ServerVariables["REMOTE_ADDR"];
		userInput.RecommendUid = UserCookieManager.UniqueUserId;
		userInput.MemberRankId = MemberRankOptionUtility.GetDefaultMemberRank(); // デフォルト会員ランクの設定
		userInput.LastChanged = Constants.FLG_LASTCHANGED_USER;
		userInput.AddrCountryIsoCode = string.Empty;
		userInput.Addr5 = string.Empty;
		userInput.AddrCountryName = string.Empty;

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			userInput.AddrCountryIsoCode = this.WddlUserCountry.SelectedValue;
			userInput.Addr5 = IsCountryUs(userInput.AddrCountryIsoCode)
				? this.WddlUserAddr5.SelectedValue
				: DataInputUtility.ConvertToFullWidthBySetting(this.WtbUserAddr5.Text, this.IsUserAddrJp).Trim();
			userInput.AddrCountryName = this.WddlUserCountry.SelectedText;

			if (IsCountryTw(this.UserAddrCountryIsoCode)
				&& this.WddlUserAddr2.HasInnerControl
				&& this.WddlUserAddr3.HasInnerControl)
			{
				userInput.Addr2 = this.WddlUserAddr2.SelectedText;
				userInput.Addr3 = this.WddlUserAddr3.SelectedText;
			}

			if (this.IsUserAddrJp == false)
			{
				userInput.Zip = StringUtility.ToHankaku(this.WtbUserZipGlobal.Text);
				userInput.Zip1 = string.Empty;
				userInput.Zip2 = string.Empty;
				userInput.Tel1 = StringUtility.ToHankaku(this.WtbUserTel1Global.Text);
				userInput.Tel1_1 = string.Empty;
				userInput.Tel1_2 = string.Empty;
				userInput.Tel1_3 = string.Empty;
				userInput.Tel2 = StringUtility.ToHankaku(this.WtbUserTel2Global.Text);
				userInput.Tel2_1 = string.Empty;
				userInput.Tel2_2 = string.Empty;
				userInput.Tel2_3 = string.Empty;
				userInput.NameKana1 = string.Empty;
				userInput.NameKana2 = string.Empty;
				userInput.NameKana = string.Empty;
			}
		}
		userInput.Addr = userInput.ConcatenateAddress();

		// ユーザ拡張項目のユーザコントロールがあれば情報取得
		userInput.UserExtendInput = (this.UserExtendUserControl != null) ? ((UserExtendUserControl)this.UserExtendUserControl).UserExtend : new UserExtendInput();

		// Amazonログインの場合の処理
		if (this.IsAmazonLoggedIn && (Constants.AMAZON_PAYMENT_CV2_ENABLED == false))
		{
			var amazonModel = (AmazonModel)Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL];
			var res = AmazonApiFacade.GetOrderReferenceDetails(this.WhfAmazonOrderRefID.Value, amazonModel.Token);
			var amazonAddressInput = new AmazonAddressInput(res);
			var amazonAddressModel = AmazonAddressParser.Parse(amazonAddressInput);

			userInput.Zip1 = amazonAddressModel.Zip1;
			userInput.Zip2 = amazonAddressModel.Zip2;
			userInput.Zip = userInput.Zip1 + "-" + userInput.Zip2;
			userInput.Addr1 = amazonAddressModel.Addr1;
			userInput.Addr2 = StringUtility.ToZenkaku(amazonAddressModel.Addr2);
			// Addr3,Addr4は空文字になる可能性があるので、空文字の場合nullにすることで必須チェック回避
			userInput.Addr3 = (amazonAddressModel.Addr3 == string.Empty) ? null : StringUtility.ToZenkaku(amazonAddressModel.Addr3);
			userInput.Addr4 = (amazonAddressModel.Addr4 == string.Empty) ? null : StringUtility.ToZenkaku(amazonAddressModel.Addr4);
			userInput.Addr = userInput.ConcatenateAddressWithoutCountryName();
		}

		if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED)
		{
			userInput.HasAuthenticationCode = this.HasAuthenticationCode;
			userInput.AuthenticationCode = this.IsUserAddrJp
				? this.WtbAuthenticationCode.Text
				: this.WtbAuthenticationCodeGlobal.Text;

			if (this.IsDisplayPersonalAuthentication == false) userInput.HasAuthenticationCode = true;
		}

		return userInput;
	}

	/// <summary>
	/// ユーザー情報を画面に表示する
	/// </summary>
	/// <param name="user">ユーザー情報</param>
	private void DisplayUserInfo(UserModel user)
	{
		this.WtbUserName1.Text = user.Name1;
		this.WtbUserName2.Text = user.Name2;
		this.WtbUserNickName.Text = user.NickName;
		this.WtbUserMailAddr.Text = user.MailAddr;
		this.WtbUserMailAddrConf.Text = user.MailAddr;
		this.WtbUserMailAddr2.Text = user.MailAddr2;
		this.WtbUserMailAddr2Conf.Text = user.MailAddr2;

		// AmazonPay利用時カナ情報は設定値が入るため、上書きをしない
		this.WtbUserNameKana1.Text = (user.NameKana1 != Constants.PAYMENT_AMAZON_NAMEKANA1)
			? user.NameKana1
			: this.WtbUserNameKana1.Text;
		this.WtbUserNameKana2.Text = (user.NameKana2 != Constants.PAYMENT_AMAZON_NAMEKANA2)
			? user.NameKana2
			: this.WtbUserNameKana2.Text;
		
		// AmazonPayCV2では住所と電話番号はユーザー格納されないので
		// その場合は上書きしないようにする
		if (string.IsNullOrEmpty(
			user.Zip1 + user.Zip2 + user.Addr1 + user.Addr2 + user.Addr3 + user.Addr4
			+ user.Tel1_1 + user.Tel1_2 + user.Tel1_3))
		{
			return;
		}

		this.WddlUserBirthYear.SelectedValue = user.BirthYear;
		this.WddlUserBirthMonth.SelectedValue = user.BirthMonth;
		this.WddlUserBirthDay.SelectedValue = user.BirthDay;
		this.WrblUserSex.SelectItemByValue(user.Sex);

		// Set value for zip
		SetZipCodeTextbox(
			this.WtbUserZip,
			this.WtbUserZip1,
			this.WtbUserZip2,
			user.Zip);

		this.WddlUserAddr1.SelectedValue = user.Addr1;
		this.WtbUserAddr2.Text = user.Addr2;
		this.WtbUserAddr3.Text = user.Addr3;
		this.WtbUserAddr4.Text = user.Addr4;
		this.WtbUserCompanyName.Text = user.CompanyName;
		this.WtbUserCompanyPostName.Text = user.CompanyPostName;

		// Set value for telephone 1
		SetTelTextbox(
			this.WtbUserTel1_1,
			this.WtbUserTel1,
			this.WtbUserTel2,
			this.WtbUserTel3,
			user.Tel1);

		// Set value for telephone 2
		SetTelTextbox(
			this.WtbUserTel1_2,
			this.WtbUserTel2_1,
			this.WtbUserTel2_2,
			this.WtbUserTel2_3,
			user.Tel2);

		this.WcbUserMailFlg.Checked = (user.MailFlg == Constants.FLG_USER_MAILFLG_OK);

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			this.WddlUserCountry.SelectedValue = user.AddrCountryIsoCode;
			this.WtbUserZipGlobal.Text = user.Zip;
			this.WtbUserTel1Global.Text = user.Tel1;
			this.WtbUserTel2Global.Text = user.Tel2;

			if (IsCountryUs(user.AddrCountryIsoCode))
			{
				this.WddlUserAddr5.SelectedText = user.Addr5;
			}
			else
			{
				this.WtbUserAddr5.Text = user.Addr5;
			}

			if (IsCountryTw(user.AddrCountryIsoCode)
				&& this.WddlUserAddr2.HasInnerControl
				&& this.WddlUserAddr3.HasInnerControl)
			{
				this.WddlUserAddr2.SelectItemByValue(user.Addr2);
				BindingDdlUserAddr3();

				this.WddlUserAddr3.ForceSelectItemByText(user.Addr3);
			}
		}
	}

	/// <summary>
	/// 楽天IDConnectリクエストクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbRakutenIdConnectRequestAuth_OnClick(object sender, EventArgs e)
	{
		var url = Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_REGIST_INPUT;
		this.Process.RedirectRakutenIdConnect(ActionType.UserRegister, url, url);
	}

	/// <summary>
	/// ペイパル認証完了
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbPayPalAuthComplete_Click(object sender, EventArgs e)
	{
		// ペイパル認証情報をセッションにセット
		var payPalScriptsForm = GetWrappedControl<WrappedPayPalPayScriptsFormControl>("ucPaypalScriptsForm");
		SetPaypalInfoToSession(payPalScriptsForm);

		// ユーザーが見つかればログインさせ、見つからなければ入力情報補完させる
		var user = PayPalUtility.Account.GetUserByPayPalCustomerId(SessionManager.PayPalLoginResult.CustomerId);
		if (user != null)
		{
			// ログイン成功処理＆次の画面へ遷移
			ExecLoginSuccessProcessAndGoNextForLogin(user, Constants.PATH_ROOT, false, LoginType.Normal, UpdateHistoryAction.Insert);
		}

		// ユーザーが見つからない場合デフォルトセット
		SetDefaultToForm();
	}

	/// <summary>
	/// Amazonアドレス帳ウィジェットウィジェット情報取得
	/// </summary>
	/// <param name="amazonOrderReferenceId">Amazon注文リファレンスID</param>
	/// <returns>エラーメッセージ</returns>
	[WebMethod]
	public static string GetAmazonAddress(string amazonOrderReferenceId)
	{
		// トークン取得
		var session = HttpContext.Current.Session;
		var amazonModel = (AmazonModel)session[AmazonConstants.SESSION_KEY_AMAZON_MODEL];
		var token = amazonModel.Token;

		// ウィジェットから住所情報取得
		var res = AmazonApiFacade.GetOrderReferenceDetails(amazonOrderReferenceId, token);
		var input = new AmazonAddressInput(res);

		// 入力チェック
		var errorMessage = input.Validate();
		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			session[AmazonConstants.SESSION_KEY_AMAZON_ADDRESS_ERROR_MSG] = errorMessage;
			return JsonConvert.SerializeObject(new { Error = errorMessage, Input = input });
		}

		session.Remove(AmazonConstants.SESSION_KEY_AMAZON_ADDRESS_ERROR_MSG);
		return JsonConvert.SerializeObject(new { Error = errorMessage, Input = input });
	}

	#region #ddlUserCountry_SelectedIndexChanged ユーザー住所国ドロップダウンリスト変更時イベント
	/// <summary>
	/// ユーザー住所国ドロップダウンリスト変更時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlUserAddrCountry_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			// 国変更向けグローバル国設定CSS付与（クライアント検証用）
			AddCountryInfoToControlForChangeCountry(
				new WrappedWebControl[]
				{
					this.WtbUserName1,
					this.WtbUserName2,
					this.WtbUserNameKana1,
					this.WtbUserNameKana2,
					this.WtbUserMailAddr,
					this.WtbUserMailAddr2,
					this.WddlUserBirthYear,
					this.WddlUserBirthMonth,
					this.WddlUserBirthDay,
					this.WrblUserSex,
					this.WtbUserZip1,
					this.WtbUserZip2,
					this.WtbUserAddr2,
					this.WtbUserAddr2,
					this.WtbUserAddr2,
					this.WtbUserAddr3,
					this.WtbUserAddr4,
					this.WtbUserTel1,
					this.WtbUserTel2,
					this.WtbUserTel3,
					this.WtbUserTel2_1,
					this.WtbUserTel2_2,
					this.WtbUserTel2_3,
					this.WtbUserZipGlobal,
					this.WtbUserTel1Global,
					this.WtbUserTel2Global,
				},
				this.UserAddrCountryIsoCode);
			// 国変更向けValidationGroup変更処理
			ChangeValidationGroupForChangeCountry(
				new WrappedControl[] { this.WcvUserAddr2, this.WcvUserAddr3, this.WcvUserAddr4, this.WlbConfirm, },
				this.ValidationGroup);

			if (this.IsUserAddrJp == false) this.WtbUserZip2.Text = string.Empty;

			// Display Zip Global
			if (IsCountryTw(this.UserAddrCountryIsoCode) && this.WddlUserAddr3.HasInnerControl)
			{
				BindingDdlUserAddr3();
			}
		}

		if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED
			&& IsPostBack)
		{
			var wlbGetAuthenticationCode = this.IsUserAddrJp
				? this.WlbGetAuthenticationCode
				: this.WlbGetAuthenticationCodeGlobal;
			var wtbAuthenticationCode = this.IsUserAddrJp
				? this.WtbAuthenticationCode
				: this.WtbAuthenticationCodeGlobal;

			if (this.IsUserAddrJp)
			{
				if (this.WtbUserTel1.HasInnerControl)
				{
					this.WtbUserTel1.Text
						= this.WtbUserTel2.Text
						= this.WtbUserTel3.Text
						= string.Empty;
				}
				else
				{
					this.WtbUserTel1_1.Text = string.Empty;
				}
			}
			else
			{
				this.WtbUserTel1Global.Text = string.Empty;
			}

			// Stop process validation when changing country
			StopTimeCount();

			this.HasAuthenticationCode = false;
			this.WlbAuthenticationStatus.Text
				= this.WlbAuthenticationStatusGlobal.Text
				= string.Empty;

			DisplayAuthenticationCode(
				wlbGetAuthenticationCode,
				wtbAuthenticationCode);
		}
	}
	#endregion

	/// <summary>
	/// カスタムバリデータの属性値を変更する（EFOオプションONのとき、カスタムバリデータを無効化する）
	/// </summary>
	public void UpdateAttributeValueForCustomValidator()
	{
		// EFOオプションチェック（有効な場合、カスタムバリデータを無効化）
		if (this.IsEfoOptionEnabled == false)
		{
			SetCustomValidatorControlInformationList(this);
			return;
		}

		var customValidatorControls = new []
		{
			this.WcvUserName1,
			this.WcvUserName2,
			this.WcvUserNameKana1,
			this.WcvUserNameKana2,
			this.WcvUserBirthYear,
			this.WcvUserBirthMonth,
			this.WcvUserBirthDay,
			this.WcvUserSex,
			this.WcvUserMailAddrForCheck,
			this.WcvUserMailAddr,
			this.WcvUserMailAddrConf,
			this.WcvUserMailAddr2,
			this.WcvUserMailAddr2Conf,
			this.WcvUserZip1,
			this.WcvUserZip2,
			this.WcvUserAddr1,
			this.WcvUserAddr2,
			this.WcvUserAddr3,
			this.WcvUserTel1,
			this.WcvUserTel2,
			this.WcvUserTel3,
			this.WcvUserTel2_1,
			this.WcvUserTel2_2,
			this.WcvUserTel2_3,
			this.WcvUserLoginId,
			this.WcvUserPassword,
			this.WcvUserPasswordConf,
		};
		SetDisableAndHideCustomValidatorControlInformationList(customValidatorControls);
	}

	/// <summary>
	/// 台湾都市ドロップダウンリスト選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlUserAddr2_SelectedIndexChanged(object sender, EventArgs e)
	{
		BindingDdlUserAddr3();
	}

	/// <summary>
	/// 台湾地域ドロップダウンリスト生成
	/// </summary>
	protected void BindingDdlUserAddr3()
	{
		GlobalAddressUtil.BindingDdlUserAddr3(
			WddlUserAddr3,
			WtbUserZipGlobal,
			this.UserTwDistrictDict[this.WddlUserAddr2.SelectedItem.ToString()]);
	}

	/// <summary>
	/// 再入力ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUserPasswordReType_Click(object sender, EventArgs e)
	{
		UserInput userInput = (UserInput)Session[Constants.SESSION_KEY_PARAM];
		userInput.Password = string.Empty;
		Session[Constants.SESSION_KEY_PARAM] = userInput;

		this.SessionParamTargetPage = Constants.PAGE_FRONT_USER_REGIST_INPUT;
		var Url = new UrlCreator(Request.RawUrl).WithUrlFragment("logininfo").CreateUrl();
		Response.Redirect(Url);
	}

	/// <summary>
	/// Linkbutton search address from zipcode global click event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSearchAddrFromZipGlobal_Click(object sender, EventArgs e)
	{
		if (IsNotCountryJp(this.UserAddrCountryIsoCode) == false) return;

		BindingAddressByGlobalZipcode(
			this.UserAddrCountryIsoCode,
			StringUtility.ToHankaku(this.WtbUserZipGlobal.Text.Trim()),
			this.WtbUserAddr2,
			this.WtbUserAddr4,
			this.WtbUserAddr5,
			this.WddlUserAddr2,
			this.WddlUserAddr3,
			this.WddlUserAddr5);
	}

	/// <summary>
	/// Link button get authentication code click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbGetAuthenticationCode_Click(object sender, EventArgs e)
	{
		this.HasAuthenticationCode = false;
		RemoveErrorInputClass(WtbUserTel1Global);

		ChangeControlLooksForValidator(
			new Dictionary<string, string> { { string.Empty, string.Empty } },
			Constants.CONST_KEY_USER_AUTHENTICATION_CODE,
			this.IsUserAddrJp ? this.WcvAuthenticationCode : this.WcvAuthenticationCodeGlobal,
			this.IsUserAddrJp ? this.WtbAuthenticationCode : this.WtbAuthenticationCodeGlobal);

		ChangeControlLooksForValidator(
			new Dictionary<string, string> { { string.Empty, string.Empty } },
			Constants.FIELD_USER_TEL1,
			this.WcvUserTel1Global,
			this.WtbUserTel1Global);

		var userAddrCountryIsoCode = Constants.GLOBAL_OPTION_ENABLE
			? this.UserAddrCountryIsoCode
			: Constants.COUNTRY_ISO_CODE_JP;

		var telephone = GetValueForTelephone(
			this.WtbUserTel1,
			this.WtbUserTel2,
			this.WtbUserTel3,
			this.WtbUserTel1_1,
			this.WtbUserTel1Global,
			userAddrCountryIsoCode);

		SendAuthenticationCode(
			this.IsUserAddrJp ? this.WtbAuthenticationCode : this.WtbAuthenticationCodeGlobal,
			this.IsUserAddrJp ? this.WlbAuthenticationStatus : this.WlbAuthenticationStatusGlobal,
			telephone,
			userAddrCountryIsoCode);
	}

	/// <summary>
	/// Link button check authentication code click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCheckAuthenticationCode_Click(object sender, EventArgs e)
	{
		ChangeControlLooksForValidator(
			new Dictionary<string, string> { { string.Empty, string.Empty } },
			Constants.CONST_KEY_USER_AUTHENTICATION_CODE,
			this.IsUserAddrJp ? this.WcvAuthenticationCode : this.WcvAuthenticationCodeGlobal,
			this.IsUserAddrJp ? this.WtbAuthenticationCode : this.WtbAuthenticationCodeGlobal);

		// Reset authentication code
		if (string.IsNullOrEmpty(this.WhfResetAuthenticationCode.Value) == false)
		{
			this.HasAuthenticationCode = false;

			DisplayAuthenticationCode(
				this.IsUserAddrJp ? this.WlbGetAuthenticationCode : this.WlbGetAuthenticationCodeGlobal,
				this.IsUserAddrJp ? this.WtbAuthenticationCode : this.WtbAuthenticationCodeGlobal,
				this.IsUserAddrJp ? this.WlbAuthenticationStatus : this.WlbAuthenticationStatusGlobal);

			return;
		}

		var userAddrCountryIsoCode = Constants.GLOBAL_OPTION_ENABLE
			? this.UserAddrCountryIsoCode
			: Constants.COUNTRY_ISO_CODE_JP;

		var telephone = GetValueForTelephone(
			this.WtbUserTel1,
			this.WtbUserTel2,
			this.WtbUserTel3,
			this.WtbUserTel1_1,
			this.WtbUserTel1Global,
			userAddrCountryIsoCode);

		// Exec check authentication code
		var errorMessages = this.IsUserAddrJp
			? ExecCheckAuthenticationCode(
				this.WlbGetAuthenticationCode,
				this.WtbAuthenticationCode,
				this.WlbAuthenticationMessage,
				this.WlbAuthenticationStatus,
				telephone,
				userAddrCountryIsoCode)
			: ExecCheckAuthenticationCode(
				this.WlbGetAuthenticationCodeGlobal,
				this.WtbAuthenticationCodeGlobal,
				this.WlbAuthenticationMessageGlobal,
				this.WlbAuthenticationStatusGlobal,
				telephone,
				userAddrCountryIsoCode);

		if (errorMessages.Count > 0)
		{
			this.HasAuthenticationCode = false;
			ChangeControlLooksForValidator(
				errorMessages,
				Constants.CONST_KEY_USER_AUTHENTICATION_CODE,
				this.IsUserAddrJp ? this.WcvAuthenticationCode : this.WcvAuthenticationCodeGlobal,
				this.IsUserAddrJp ? this.WtbAuthenticationCode : this.WtbAuthenticationCodeGlobal);
			return;
		}

		this.HasAuthenticationCode = true;
		this.WhfResetAuthenticationCode.Value = string.Empty;

		RemoveErrorInputClass(this.IsUserAddrJp
			? this.WtbAuthenticationCode
			: this.WtbAuthenticationCodeGlobal);
	}

	/// <summary>
	/// GMOチェックボックスのクリック時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void checkBusinessOwnerChangedEvent(object sender, EventArgs e)
	{
		CheckBox cb = (CheckBox)sender;
		if (cb.Checked)
		{
			UserBusinessOwnerInput userBusinessOwnerInput = (UserBusinessOwnerInput)Session[Constants.SESSION_KEY_PARAM_FOR_USER_BUSINESS_OWNER_INPUT];
			if (userBusinessOwnerInput != null)
			{
				tbOwnerName1.Text = userBusinessOwnerInput.OwnerName1;
				tbOwnerName2.Text = userBusinessOwnerInput.OwnerName2;
				tbOwnerNameKana1.Text = userBusinessOwnerInput.OwnerNameKana1;
				tbOwnerNameKana2.Text = userBusinessOwnerInput.OwnerNameKana2;
				tbRequestBudget.Text = userBusinessOwnerInput.RequestBudget.ToString();
				WddlOwnerBirthYear.SelectedValue = userBusinessOwnerInput.BirthYear;
				WddlOwnerBirthMonth.SelectedValue = userBusinessOwnerInput.BirthMonth;
				WddlOwnerBirthDay.SelectedValue = userBusinessOwnerInput.BirthDay;
			}
		}
		else
		{
			UserBusinessOwnerInput userBusinessOwnerInput = new UserBusinessOwnerInput();
			userBusinessOwnerInput.OwnerName1 = this.tbOwnerName1.Text;
			userBusinessOwnerInput.OwnerName2 = this.tbOwnerName2.Text;
			userBusinessOwnerInput.OwnerNameKana1 = this.tbOwnerNameKana1.Text;
			userBusinessOwnerInput.OwnerNameKana2 = this.tbOwnerNameKana2.Text;
			userBusinessOwnerInput.RequestBudget = this.tbRequestBudget.Text;
			userBusinessOwnerInput.BirthYear = this.WddlOwnerBirthYear.SelectedValue;
			userBusinessOwnerInput.BirthMonth = this.WddlOwnerBirthMonth.SelectedValue;
			userBusinessOwnerInput.BirthDay = this.WddlOwnerBirthDay.SelectedValue;
			Session[Constants.SESSION_KEY_PARAM_FOR_USER_BUSINESS_OWNER_INPUT] = userBusinessOwnerInput;
		}
	}

	/// <summary>
	/// ユーザー情報のバリデート処理
	/// </summary>
	/// <param name="userInfo">ユーザー情報</param>
	/// <returns>エラーメッセージ</returns>
	private Dictionary<string, string> ValidateUserInfo(UserInput userInfo)
	{
		// 確認画面の「戻る」ボタンで入力画面に戻ったが、パスワード再入力しない場合、セッションからパスワード情報を取得する
		if (string.IsNullOrEmpty(userInfo.Password)
			&& string.IsNullOrEmpty(userInfo.PasswordConf)
			&& this.IsVisible_UserPassword
			&& (Session[Constants.SESSION_KEY_PARAM] != null))
		{
			var sessionUserInput = (UserInput)Session[Constants.SESSION_KEY_PARAM];
			userInfo.Password = sessionUserInput.Password;
			userInfo.PasswordConf = sessionUserInput.PasswordConf;
		}

		// ユーザー情報検証
		var excludeList = new List<string>();
		if (this.WtbUserTel1_1.HasInnerControl == false)
		{
			excludeList.Add(Constants.FIELD_USER_TEL1);
		}
		if (this.WtbUserTel1_2.HasInnerControl == false)
		{
			excludeList.Add(Constants.FIELD_USER_TEL2);
		}
		var errorMessages = userInfo.Validate(
			this.IsUserAddrJp
				? UserInput.EnumUserInputValidationKbn.UserRegist
				: UserInput.EnumUserInputValidationKbn.UserRegistGlobal
			, excludeList);

		if (userInfo.BusinessOwner != null)
		{
			var errorMessagesGmo = userInfo.BusinessOwner.Validate(UserBusinessOwnerInput.EnumUserInputValidationKbn.Regist, excludeList);
			foreach (var item in errorMessagesGmo)
			{
				errorMessages.Add(item.Key, item.Value);
			}
		}

		if (errorMessages.Count == 0)
		{
			// ユーザー拡張項目情報検証
			errorMessages = userInfo.UserExtendInput.Validate();
		}

		return errorMessages;
	}

	/// <summary>郵便番号入力チェックエラー文言</summary>
	protected string ZipInputErrorMessage
	{
		get { return StringUtility.ToEmpty(ViewState["ZipInputErrorMessage"]); }
		set { ViewState["ZipInputErrorMessage"] = value; }
	}
	/// <summary>パスワード表示フラグ</summary>
	protected bool IsVisible_UserPassword
	{
		get { return (bool)ViewState["UserPassword"]; }
		set { ViewState["UserPassword"] = value; }
	}
	/// <summary>ユーザ拡張項目のユーザコントロール</summary>
	private Control UserExtendUserControl { get { return GetDefaultMasterContentPlaceHolder().FindControl("ucBodyUserExtendRegist"); } }
	/// <summary>ユーザーの住所が日本か</summary>
	protected bool IsUserAddrJp
	{
		get { return GlobalAddressUtil.IsCountryJp(this.UserAddrCountryIsoCode); }
	}
	/// <summary>ユーザーの住所がUSか</summary>
	protected bool IsUserAddrUs
	{
		get { return GlobalAddressUtil.IsCountryUs(this.UserAddrCountryIsoCode); }
	}
	/// <summary>ユーザーの住所郵便番号が必須か</summary>
	protected bool IsUserAddrZipNecessary
	{
		get
		{
			var necessary = GlobalAddressUtil.IsAddrZipcodeNecessary(
				this.UserAddrCountryIsoCode);
			return necessary;
		}
	}
	/// <summary>ユーザーの住所国ISOコード</summary>
	protected string UserAddrCountryIsoCode
	{
		get { return this.WddlUserCountry.SelectedValue; }
	}
	/// <summary>バリデーショングループ</summary>
	protected string ValidationGroup
	{
		get { return this.IsUserAddrJp ? "UserRegist" : "UserRegistGlobal"; }
	}
	/// <summary>注文完了後会員登録か</summary>
	protected bool IsAfterOrder
	{
		get
		{
			var result = (Session[Constants.SESSION_KEY_USER_REGIST_AFTER_ORDER_USER_ID] != null);
			return result;
		}
	}
	/// <summary>アマゾンリクエスト</summary>
	protected AmazonCv2Redirect AmazonRequest { get; set; }
	/// <summary>Is display personal authentication</summary>
	public bool IsDisplayPersonalAuthentication
	{
		get
		{
			if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED == false) return false;

			return (this.IsSocialLogin == false);
		}
	}
	/// <summary>クロスポイントアプリか</summary>
	private bool IsAppCrossPoint
	{
		get
		{
			var result = (Constants.CROSS_POINT_OPTION_ENABLED
				&& (string.IsNullOrEmpty(SessionManager.AppKeyForCrossPoint) == false));
			return result;
		}
	}
}
