using Newtonsoft.Json;
/*
=========================================================================================================
  Module      : かんたん会員登録入力画面処理(UserEasyRegistInput.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;
using w2.App.Common.CrossPoint.User;
using w2.Domain;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.App.Common.Amazon;
using w2.App.Common.Amazon.Util;
using w2.App.Common.Amazon.Helper;
using w2.App.Common.AmazonCv2;
using w2.App.Common.Global;
using w2.App.Common.Global.Region;
using w2.App.Common.Line.Util;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.Option;
using w2.App.Common.User;
using w2.App.Common.User.SocialLogin;
using w2.App.Common.User.SocialLogin.Helper;
using w2.App.Common.User.SocialLogin.Util;
using w2.App.Common.Util;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.AdvCode;
using w2.Common.Util;

public partial class Form_User_UserEasyRegistInput : UserPage
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
	protected WrappedTextBox WtbUserZip1 { get { return GetWrappedControl<WrappedTextBox>("tbUserZip1"); } }
	protected WrappedTextBox WtbUserZip2 { get { return GetWrappedControl<WrappedTextBox>("tbUserZip2"); } }
	protected WrappedDropDownList WddlUserCountry { get { return GetWrappedControl<WrappedDropDownList>("ddlUserCountry"); } }
	protected WrappedDropDownList WddlUserAddr1 { get { return GetWrappedControl<WrappedDropDownList>("ddlUserAddr1"); } }
	protected WrappedTextBox WtbUserAddr2 { get { return GetWrappedControl<WrappedTextBox>("tbUserAddr2"); } }
	protected WrappedTextBox WtbUserAddr3 { get { return GetWrappedControl<WrappedTextBox>("tbUserAddr3"); } }
	protected WrappedTextBox WtbUserAddr4 { get { return GetWrappedControl<WrappedTextBox>("tbUserAddr4"); } }
	protected WrappedTextBox WtbUserAddr5 { get { return GetWrappedControl<WrappedTextBox>("tbUserAddr5"); } }
	protected WrappedTextBox WtbUserCompanyName { get { return GetWrappedControl<WrappedTextBox>("tbUserCompanyName"); } }
	protected WrappedTextBox WtbUserCompanyPostName { get { return GetWrappedControl<WrappedTextBox>("tbUserCompanyPostName"); } }
	protected WrappedTextBox WtbUserTel1 { get { return GetWrappedControl<WrappedTextBox>("tbUserTel1"); } }
	protected WrappedTextBox WtbUserTel2 { get { return GetWrappedControl<WrappedTextBox>("tbUserTel2"); } }
	protected WrappedTextBox WtbUserTel3 { get { return GetWrappedControl<WrappedTextBox>("tbUserTel3"); } }
	protected WrappedTextBox WtbUserTel2_1 { get { return GetWrappedControl<WrappedTextBox>("tbUserTel2_1"); } }
	protected WrappedTextBox WtbUserTel2_2 { get { return GetWrappedControl<WrappedTextBox>("tbUserTel2_2"); } }
	protected WrappedTextBox WtbUserTel2_3 { get { return GetWrappedControl<WrappedTextBox>("tbUserTel2_3"); } }
	protected WrappedCheckBox WcbUserMailFlg { get { return GetWrappedControl<WrappedCheckBox>("cbUserMailFlg", false); } }
	protected WrappedTextBox WtbUserLoginId { get { return GetWrappedControl<WrappedTextBox>(Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED ? "tbUserMailAddr" : "tbUserLoginId"); } }
	protected WrappedTextBox WtbUserPassword { get { return GetWrappedControl<WrappedTextBox>("tbUserPassword"); } }
	protected WrappedTextBox WtbUserPasswordConf { get { return GetWrappedControl<WrappedTextBox>("tbUserPasswordConf"); } }
	protected WrappedCheckBox WcbUserAcceptedRegulation { get { return GetWrappedControl<WrappedCheckBox>("cbUserAcceptedRegulation"); } }

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

	protected WrappedHiddenField WhfSocialLoginJson { get { return GetWrappedControl<WrappedHiddenField>("hfSocialLoginJson"); } }
	protected WrappedHiddenField WhfAmazonOrderRefID { get { return GetWrappedControl<WrappedHiddenField>("hfAmazonOrderRefID"); } }
	protected WrappedHtmlGenericControl WhgcAddressBookWidgetDiv { get { return GetWrappedControl<WrappedHtmlGenericControl>("addressBookWidgetDiv"); } }
	protected WrappedHtmlGenericControl WhgcAddressBookErrorMessage { get { return GetWrappedControl<WrappedHtmlGenericControl>("addressBookErrorMessage"); } }
	protected WrappedHtmlGenericControl WhgcCountryAlertMessage { get { return GetWrappedControl<WrappedHtmlGenericControl>("countryAlertMessage"); } }

	protected WrappedLinkButton WlbSearchAddr { get { return GetWrappedControl<WrappedLinkButton>("lbSearchAddr"); } }
	protected WrappedTextBox WtbUserZip { get { return GetWrappedControl<WrappedTextBox>("tbUserZip"); } }
	protected WrappedTextBox WtbUserTel1_1 { get { return GetWrappedControl<WrappedTextBox>("tbUserTel1_1"); } }
	protected WrappedTextBox WtbUserTel1_2 { get { return GetWrappedControl<WrappedTextBox>("tbUserTel1_2"); } }

	/// <summary>Wrapped text box authentication code</summary>
	protected WrappedTextBox WtbAuthenticationCode { get { return GetWrappedControl<WrappedTextBox>("tbAuthenticationCode"); } }
	/// <summary>Wrapped link button get authentication code</summary>
	protected WrappedLinkButton WlbGetAuthenticationCode { get { return GetWrappedControl<WrappedLinkButton>("lbGetAuthenticationCode"); } }
	/// <summary>Wrapped label authentication status</summary>
	protected WrappedLabel WlbAuthenticationStatus { get { return GetWrappedControl<WrappedLabel>("lbAuthenticationStatus"); } }
	/// <summary>Wrapped label has authentication</summary>
	protected WrappedLabel WlbHasAuthentication { get { return GetWrappedControl<WrappedLabel>("lbHasAuthentication"); } }
	/// <summary>Wrapped label authentication message</summary>
	protected WrappedLabel WlbAuthenticationMessage { get { return GetWrappedControl<WrappedLabel>("lbAuthenticationMessage"); } }
	/// <summary>Wrapped hidden field reset authentication Code</summary>
	protected WrappedHiddenField WhfResetAuthenticationCode { get { return GetWrappedControl<WrappedHiddenField>("hfResetAuthenticationCode"); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// HTTPS通信チェック（HTTPのとき、HTTPSにリダイレクト）
		//------------------------------------------------------
		CheckHttps(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_EASY_REGIST_INPUT);

		//------------------------------------------------------
		// ログインチェック（ログイン済みの場合、トップ画面へ）
		//------------------------------------------------------
		if (this.IsLoggedIn)
		{
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT);
		}

		// かんたん会員登録オプションOFFの場合、かんたん会員登録ページアクセス時404エラーページ遷移
		if ((Constants.USEREAZYREGISTERSETTING_OPTION_ENABLED == false))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_404_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		//カスタムバリデータ属性値更新
		UpdateAttributeValueForCustomValidator();

		if (!IsPostBack)
		{
			// コンポーネント初期化
			InitComponents();

			// デフォルト値をフォームにセット
			SetDefaultToForm();

			// リクエスト情報にNextUrlが存在した場合、セッションNextUrlを格納
			if (Request[Constants.REQUEST_KEY_NEXT_URL] != null)
			{
				Session[Constants.SESSION_KEY_NEXT_URL] = NextUrlValidation(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_NEXT_URL]));
			}

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				this.WddlUserCountry.SelectItemByValue(RegionManager.GetInstance().Region.CountryIsoCode);
			}

			// 注文完了後、会員登録の場合はの通常の会員登録フローに遷移させる
			if (Session[Constants.SESSION_KEY_USER_REGIST_AFTER_ORDER_USER_ID] != null)
			{
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_REGIST_REGULATION);
			}

			if (SessionManager.TemporaryStoreSocialLogin != null)
			{
				SessionManager.SocialLogin = SessionManager.TemporaryStoreSocialLogin;
			}

			if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED) this.HasAuthenticationCode = false;
		}

		if (Constants.AMAZON_LOGIN_OPTION_ENABLED && Constants.AMAZON_PAYMENT_CV2_ENABLED)
		{
			this.AmazonRequest = AmazonCv2Redirect.SignPayloadForSignIn(Constants.PAGE_FRONT_AMAZON_USER_EASY_REGISTER_CALLBACK);
		}
		// apsx側プロパティセットしているため、バインドを行う
		if (this.Captcha != null)
		{
			this.Captcha.DataBind();
		}
	}

	/// <summary>
	/// デフォルト値をフォームにセット
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
		this.WcbUserMailFlg.Checked = (Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.mail_flg.default@@")
			== Constants.FLG_USER_MAILFLG_OK);
		this.IsVisible_UserPassword = true;
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			this.WddlUserCountry.SelectedValue = RegionManager.GetInstance().Region.CountryIsoCode;
		}

		// ペイパル情報補完
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
		// ソーシャルログイン情報補完
		else if (Constants.SOCIAL_LOGIN_ENABLED)
		{
			var socialLogin = (SocialLoginModel)this.Session[Constants.SESSION_KEY_SOCIAL_LOGIN_MODEL];
			// 遷移元ページがコールバックページの場合は連携したとみなす
			if (((socialLogin != null) && (socialLogin.TransitionSourcePath == ("~/" + Constants.PAGE_FRONT_SOCIAL_LOGIN_USER_EASY_REGISTER_CALLBACK))))
			{
				this.WhfSocialLoginJson.Value = socialLogin.RawResponse;
				this.IsVisible_UserPassword = false;
				// 遷移元情報を上書き
				socialLogin.TransitionSourcePath = this.AppRelativeVirtualPath;
			}
			else
			{
				// セッション情報のクリア
				this.Session.Remove(Constants.SESSION_KEY_SOCIAL_LOGIN_MODEL);
			}
		}

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
				if (this.WddlUserCountry.InnerControl != null) this.WddlUserCountry.InnerControl.Enabled = false;
				this.WhgcCountryAlertMessage.Visible = true;
			}
		}
	}

	/// <summary>
	/// 登録
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbRegister_Click(object sender, EventArgs e)
	{
		// キャプチャ認証失敗時は処理終了
		if (CheckCaptcha() == false) return;

		// ユーザー情報取得
		var userInfo = CreateInputData();

		// ユーザー情報検証
		var excludeList = new List<string>();
		if (this.WtbUserZip2.HasInnerControl == false)
		{
			excludeList.Add(Constants.FIELD_USER_ZIP1);
			excludeList.Add(Constants.FIELD_USER_ZIP2);
		}
		if (this.WtbUserTel1_1.HasInnerControl == false)
		{
			excludeList.Add(Constants.FIELD_USER_TEL1);
		}
		if (this.WtbUserTel1_2.HasInnerControl == false)
		{
			excludeList.Add(Constants.FIELD_USER_TEL2);
		}
		var errorMessages = userInfo.Validate(UserInput.EnumUserInputValidationKbn.UserEasyRegist, excludeList);

		if (Constants.CROSS_POINT_OPTION_ENABLED
			&& errorMessages.ContainsKey(Constants.FIELD_USER_LOGIN_ID) == false)
		{
			// CrossPoint メールアドレス重複チェック
			var duplicationMailErrorMessage = GetErrorMessageDuplicationCrossPointMailAddress(userInfo);

			if (string.IsNullOrEmpty(duplicationMailErrorMessage) == false)
			{
				errorMessages.Add(Constants.FIELD_USER_LOGIN_ID, duplicationMailErrorMessage);
			}
		}

		if (errorMessages.Count != 0)
		{
			if (errorMessages.ContainsKey(Constants.FIELD_USER_ZIP))
			{
				this.ZipInputErrorMessage = string.Empty;
			}

			if (this.IsSocialLogin
				&& errorMessages.ContainsKey(Constants.CONST_KEY_USER_AUTHENTICATION_CODE))
			{
				this.HasAuthenticationCode = true;
				errorMessages.Remove(Constants.CONST_KEY_USER_AUTHENTICATION_CODE);
			}

			GetErrMsgAndFocusToCV(errorMessages);
			return;
		}

		// 登録
		var user = userInfo.CreateModel();
		var userId = UserService.CreateNewUserId(
			Constants.CONST_DEFAULT_SHOP_ID,
			Constants.NUMBER_KEY_USER_ID,
			Constants.CONST_USER_ID_HEADER,
			Constants.CONST_USER_ID_LENGTH);

		user.UserId = userId;
		user.Name1 = user.Name1 ?? string.Empty;
		user.Name2 = user.Name2 ?? string.Empty;
		user.NameKana1 = user.NameKana1 ?? string.Empty;
		user.NameKana2 = user.NameKana2 ?? string.Empty;
		user.NickName = user.NickName ?? string.Empty;
		user.Zip = user.Zip ?? string.Empty;
		user.Zip1 = user.Zip1 ?? string.Empty;
		user.Zip2 = user.Zip2 ?? string.Empty;
		user.Addr = user.Addr ?? string.Empty;
		user.Addr1 = user.Addr1 ?? string.Empty;
		user.Addr2 = user.Addr2 ?? string.Empty;
		user.Addr3 = user.Addr3 ?? string.Empty;
		user.Addr4 = user.Addr4 ?? string.Empty;
		user.Addr5 = user.Addr5 ?? string.Empty;
		user.Tel1 = user.Tel1 ?? string.Empty;
		user.Tel1_1 = user.Tel1_1 ?? string.Empty;
		user.Tel1_2 = user.Tel1_2 ?? string.Empty;
		user.Tel1_3 = user.Tel1_3 ?? string.Empty;
		user.Tel2 = user.Tel2 ?? string.Empty;
		user.Tel2_1 = user.Tel2_1 ?? string.Empty;
		user.Tel2_2 = user.Tel2_2 ?? string.Empty;
		user.Tel2_3 = user.Tel2_3 ?? string.Empty;
		user.Tel3 = user.Tel3 ?? string.Empty;
		user.Tel3_1 = user.Tel3_1 ?? string.Empty;
		user.Tel3_2 = user.Tel3_2 ?? string.Empty;
		user.Tel3_3 = user.Tel3_3 ?? string.Empty;
		user.Fax = user.Fax ?? string.Empty;
		user.Fax_1 = user.Fax_1 ?? string.Empty;
		user.Fax_2 = user.Fax_2 ?? string.Empty;
		user.Fax_3 = user.Fax_3 ?? string.Empty;
		user.Sex = user.Sex ?? string.Empty;
		user.BirthYear = user.BirthYear ?? string.Empty;
		user.BirthMonth = user.BirthMonth ?? string.Empty;
		user.BirthDay = user.BirthDay ?? string.Empty;
		user.CompanyName = user.CompanyName ?? string.Empty;
		user.CompanyPostName = user.CompanyPostName ?? string.Empty;
		user.CompanyExectiveName = user.CompanyExectiveName ?? string.Empty;
		user.AdvcodeFirst = user.AdvcodeFirst ?? string.Empty;
		user.MailFlg = user.MailFlg ?? string.Empty;
		user.EasyRegisterFlg = Constants.FLG_USER_EASY_REGISTER_FLG_EASY;
		user.LastChanged = Constants.FLG_LASTCHANGED_USER;
		user.AddrCountryIsoCode = user.AddrCountryIsoCode ?? string.Empty;
		user.AddrCountryName = user.AddrCountryName ?? string.Empty;

		// 広告コードより補正
		UserUtility.CorrectUserByAdvCode(user);

		var userService = new UserService();
		user.UserExtend = new UserExtendInput().CreateModel();
		userService.InsertWithUserExtend(user, Constants.FLG_LASTCHANGED_USER, UpdateHistoryAction.DoNotInsert);

		if (Constants.CROSS_POINT_OPTION_ENABLED)
		{
			// Cooperation cross point api
			CooperationCrossPointApi(user);
		}

		// DBから最新情報を取得
		var registedUser = userService.Get(userId);

		// ユーザー登録時イベント
		ExecProcessOnUserRegisted(registedUser, UpdateHistoryAction.DoNotInsert);

		// IPとアカウント、IPとパスワードでそれぞれアカウントロックがかかっている可能性がある。
		// そのため、登録時にアカウントロックキャンセルを行う
		LoginAccountLockManager.GetInstance().CancelAccountLock(
			this.Page.Request.UserHostAddress,
			user.LoginId,
			user.PasswordDecrypted);

		// プラグイン連携ユーザー登録イベント
		if (Constants.USER_COOPERATION_ENABLED)
		{
			var userCooperationPlugin = new UserCooperationPlugin(Constants.FLG_LASTCHANGED_USER);
			userCooperationPlugin.Regist(user);
		}

		//LINEユーザーIDの紐づけ
		if ((string.IsNullOrEmpty(Constants.SOCIAL_PROVIDER_ID_LINE) == false)
			&& (Constants.SOCIAL_LOGIN_ENABLED == false)
			&& string.IsNullOrEmpty(SessionManager.LineProviderUserId) == false)
		{
			LineUtil.UpdateUserExtendForLineUserId(user.UserId, SessionManager.LineProviderUserId, UpdateHistoryAction.DoNotInsert);

			Session.Remove(Constants.SESSION_KEY_LOGIN_USER_LINE_ID);
		}

		// Update referred user id
		var referredUser = DomainFacade.Instance.UserService.GetUserByReferralCode(SessionManager.ReferralCode, null);
		var referredUserId = (referredUser == null) ? userId : referredUser.UserId;

		DomainFacade.Instance.UserService.UpdateReferredUserId(
			userId,
			referredUserId,
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.Insert);

		// 更新履歴登録
		new UpdateHistoryService().InsertForUser(userId, Constants.FLG_LASTCHANGED_USER);

		// Remove session user introduce code
		Session.Remove(Constants.SESSION_KEY_REFERRAL_CODE);

		var loggedUser = this.IsVisible_UserPassword
			? userService.TryLogin(user.LoginId, user.PasswordDecrypted, Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED)
			: userService.Get(user.UserId);
		if (loggedUser == null)
		{
			// ログイン失敗時はエラー画面に遷移
			Session[Constants.SESSION_KEY_ERROR_MSG] = GetLoginDeniedErrorMessage(userId, user.PasswordDecrypted);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		// セッションにユーザー入力情報格納
		Session[Constants.SESSION_KEY_PARAM] = userInfo;

		// ログイン成功処理＆次の画面へ遷移
		ExecLoginSuccessProcessAndGoNextForUserRegister(
			loggedUser,
					UpdateHistoryAction.Insert);
	}

	/// <summary>
	/// ユーザ登録時処理実行
	/// </summary>
	/// <param name="user">ユーザ情報</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	private void ExecProcessOnUserRegisted(UserModel user, UpdateHistoryAction updateHistoryAction)
	{
		var mailData = (Hashtable)user.DataSource.Clone();

		mailData[Constants.FIELD_USER_NAME] = UserModel.CreateComplementUserName(
			(string)mailData[Constants.FIELD_USER_NAME],
			(string)mailData[Constants.FIELD_USER_MAIL_ADDR],
			(string)mailData[Constants.FIELD_USER_MAIL_ADDR2]);

		mailData[Constants.FIELD_USER_NAME1] =
			(string.IsNullOrEmpty((string)mailData[Constants.FIELD_USER_NAME1])
			&& string.IsNullOrEmpty((string)mailData[Constants.FIELD_USER_NAME2]))
			? UserModel.CreateComplementUserName(
				(string)mailData[Constants.FIELD_USER_NAME1],
				(string)mailData[Constants.FIELD_USER_MAIL_ADDR],
				(string)mailData[Constants.FIELD_USER_MAIL_ADDR2])
			: mailData[Constants.FIELD_USER_NAME1];

		// 複合化されたパスワードに書き換え
		mailData[Constants.FIELD_USER_PASSWORD] = user.PasswordDecrypted;

		// ポイントOPがONならポイント発行
		if (Constants.W2MP_POINT_OPTION_ENABLED)
		{
			var publishdPoint = PointAtRegist(user, UpdateHistoryAction.DoNotInsert);
			mailData.Add(Constants.FIELD_USERPOINT_POINT, publishdPoint);
		}

		// クーポンOPがONならポイント発行
		if (Constants.W2MP_COUPON_OPTION_ENABLED)
		{
			// 紹介した人のデータ取得
			var introducer = DomainFacade.Instance.UserService.GetUserByReferralCode(SessionManager.ReferralCode, null);

			var publishCoupon = PublishCouponAtRegist(user, UpdateHistoryAction.DoNotInsert, (introducer != null) ? introducer.UserId : "", true);
			mailData.Add("publish_coupons", publishCoupon);


			if (Constants.INTRODUCTION_COUPON_OPTION_ENABLED
				&& (introducer != null))
			{
				// Publish coupon register for introducer
				var publishCouponRegisterForIntroducer = PublishCouponAtRegist(
					user,
					UpdateHistoryAction.DoNotInsert,
					introducer.UserId);
			}
		}

		// 生年月日の時分秒削除
		mailData[Constants.FIELD_USER_BIRTH]
			= DateTimeUtility.ToStringFromRegion(mailData[Constants.FIELD_USER_BIRTH], DateTimeUtility.FormatType.ShortDate2Letter);

		// ソーシャルログインID紐付け
		if (Constants.SOCIAL_LOGIN_ENABLED)
		{
			var socialLogin = (SocialLoginModel)Session[Constants.SESSION_KEY_SOCIAL_LOGIN_MODEL];
			if (socialLogin != null)
			{
				var authUser = new SocialLoginMap();
				socialLogin.W2UserId = user.UserId;
				authUser.Exec(Constants.SOCIAL_LOGIN_API_KEY, socialLogin.SPlusUserId, socialLogin.W2UserId, false);

				// ソーシャルプロバイダID保存
				SocialLoginUtil.SetSocialProviderInfo(socialLogin.SPlusUserId, user.UserId, user.UserExtend);
				new UserService().UpdateUserExtend(
					user.UserExtend,
					user.UserId,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.DoNotInsert);

				// セッション情報のクリア
				Session.Remove(Constants.SESSION_KEY_SOCIAL_LOGIN_MODEL);
			}
		}

		// AmazonユーザーID紐づけ
		if (this.IsAmazonLoggedIn)
		{
			// ユーザー拡張項目にAmazonユーザーIDをセット
			var amazonModel = (AmazonModel)Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL];
			AmazonUtil.SetAmazonUserIdForUserExtend(user.UserExtend, user.UserId, amazonModel.UserId);
		}
		// PayPalユーザー紐づけ
		if (SessionManager.PayPalLoginResult != null)
		{
			PayPalUtility.Account.UpdateUserExtendForPayPal(
				user.UserId,
				SessionManager.PayPalLoginResult,
				UpdateHistoryAction.DoNotInsert);
		}

		SendMail(
			Constants.CONST_MAIL_ID_USER_EASY_REGIST,
			user.UserId,
			mailData,
			RegionManager.GetInstance().Region.LanguageCode,
			RegionManager.GetInstance().Region.LanguageLocaleId);
		// 更新履歴登録
		if (updateHistoryAction == UpdateHistoryAction.Insert)
		{
			new UpdateHistoryService().InsertForUser(user.UserId, Constants.FLG_LASTCHANGED_USER);
		}
	}

	/// <summary>
	///  会員登録時のポイント処理
	/// </summary>
	/// <param name="user">ユーザー情報</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <returns>発行済みポイント</returns>
	private string PointAtRegist(UserModel user, UpdateHistoryAction updateHistoryAction)
	{
		var point = PublishPointAtRegist(user, updateHistoryAction);
		return point;
	}

	/// <summary>
	///  エラーメッセージをカスタムバリデータにセットしてフォーカス
	/// </summary>
	/// <param name="errorMsg">エラーメッセージ一覧</param>
	private void GetErrMsgAndFocusToCV(Dictionary<string, string> errorMsg)
	{
		// カスタムバリデータ取得
		var lCustomValidators = new List<CustomValidator>();
		CreateCustomValidators(this, lCustomValidators);

		// エラーをカスタムバリデータへ
		SetControlViewsForError("UserEasyRegist", errorMsg, lCustomValidators);

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
						// 下の方にあるので登録ボタンフォーカス
						case "tbUserLoginId":
						case "tbUserPassword":
						case "tbUserPasswordConf":
							lbRegister.Focus();
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
		var isUserAddrJp = ((this.IsVisible_UserCountry == false) || IsCountryJp(this.WddlUserCountry.SelectedValue));
		UserInput userInput = new UserInput(new UserModel());
		userInput.UserKbn = this.IsSmartPhone ? Constants.FLG_USER_USER_KBN_SMARTPHONE_USER : Constants.FLG_USER_USER_KBN_PC_USER;
		userInput.Name1 = this.IsVisible_UserName ? DataInputUtility.ConvertToFullWidthBySetting(this.WtbUserName1.Text, isUserAddrJp) : null;
		userInput.Name2 = this.IsVisible_UserName ? DataInputUtility.ConvertToFullWidthBySetting(this.WtbUserName2.Text, isUserAddrJp) : null;
		userInput.Name = userInput.Name1 + userInput.Name2;
		userInput.NameKana1 = this.IsVisible_UserNameKana ? StringUtility.ToZenkaku(this.WtbUserNameKana1.Text) : null;
		userInput.NameKana2 = this.IsVisible_UserNameKana ? StringUtility.ToZenkaku(this.WtbUserNameKana2.Text) : null;
		userInput.NameKana = userInput.NameKana1 + userInput.NameKana2;
		userInput.NickName = this.IsVisible_UserNickName ? this.WtbUserNickName.Text : null;
		userInput.BirthYear = this.IsVisible_UserBirth ? this.WddlUserBirthYear.SelectedValue : null;
		userInput.BirthMonth = this.IsVisible_UserBirth ? this.WddlUserBirthMonth.SelectedValue : null;
		userInput.BirthDay = this.IsVisible_UserBirth ? this.WddlUserBirthDay.SelectedValue : null;

		// どれか未入力の時は日付整合性チェックは行わない
		userInput.Birth = (this.IsVisible_UserBirth
			&& ((WddlUserBirthYear.SelectedValue + WddlUserBirthMonth.SelectedValue + WddlUserBirthDay.SelectedValue).Length != 0))
			? this.WddlUserBirthYear.SelectedValue + "/" + this.WddlUserBirthMonth.SelectedValue + "/" + this.WddlUserBirthDay.SelectedValue
			: null;
		userInput.Sex = this.IsVisible_UserSex ? this.WrblUserSex.SelectedValue : null;
		userInput.MailAddr = StringUtility.ToHankaku(this.WtbUserMailAddr.Text);
		userInput.MailAddrConf = StringUtility.ToHankaku(this.WtbUserMailAddrConf.Text);
		userInput.MailAddr2 = this.IsVisible_UserMailAddr2 ? StringUtility.ToHankaku(this.WtbUserMailAddr2.Text) : string.Empty;
		userInput.MailAddr2Conf = this.IsVisible_UserMailAddr2Conf ? StringUtility.ToHankaku(this.WtbUserMailAddr2Conf.Text) : string.Empty;
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
		var inputZipCode = (this.WtbUserZip2.HasInnerControl)
			? StringUtility.ToHankaku(this.WtbUserZip1.Text.Trim())
			: StringUtility.ToHankaku(this.WtbUserZip.Text.Trim());
		if (this.WtbUserZip2.HasInnerControl) inputZipCode += ("-" + StringUtility.ToHankaku(this.WtbUserZip2.Text.Trim()));
		var zipCode = new ZipCode(inputZipCode);
		userInput.Zip1 = this.IsVisible_UserZip ? zipCode.Zip1 : null;
		userInput.Zip2 = this.IsVisible_UserZip ? zipCode.Zip2 : null;
		userInput.Zip = this.IsVisible_UserZip
			? (string.IsNullOrEmpty(zipCode.Zip) == false)
				? zipCode.Zip
				: inputZipCode
			: null;
		userInput.Addr1 = this.IsVisible_UserAddr1 ? this.WddlUserAddr1.SelectedValue : null;
		userInput.Addr2 = this.IsVisible_UserAddr2 ? DataInputUtility.ConvertToFullWidthBySetting(this.WtbUserAddr2.Text, isUserAddrJp).Trim() : null;
		userInput.Addr3 = this.IsVisible_UserAddr3 ? DataInputUtility.ConvertToFullWidthBySetting(this.WtbUserAddr3.Text, isUserAddrJp).Trim() : null;
		userInput.Addr4 = this.IsVisible_UserAddr4 ? DataInputUtility.ConvertToFullWidthBySetting(this.WtbUserAddr4.Text, isUserAddrJp).Trim() : null;
		userInput.Addr = (this.IsVisible_UserAddr1 || this.IsVisible_UserAddr2 || this.IsVisible_UserAddr3 || this.IsVisible_UserAddr4)
			? userInput.ConcatenateAddressWithoutCountryName()
			: null;
		userInput.CompanyName = this.IsVisible_UserCompanyName ? this.WtbUserCompanyName.Text : null;
		userInput.CompanyPostName = this.IsVisible_UserCompanyPostName ? this.WtbUserCompanyPostName.Text : null;

		// Set value for telephone 1
		var inputTel1 = (this.WtbUserTel1.HasInnerControl)
			? StringUtility.ToHankaku(this.WtbUserTel1.Text.Trim())
			: StringUtility.ToHankaku(this.WtbUserTel1_1.Text.Trim());
		if (this.IsVisible_UserTel1
			&& this.WtbUserTel1.HasInnerControl)
		{
			inputTel1 = UserService.CreatePhoneNo(
				inputTel1,
				StringUtility.ToHankaku(this.WtbUserTel2.Text.Trim()),
				StringUtility.ToHankaku(this.WtbUserTel3.Text.Trim()));
		}
		var tel1 = new Tel(inputTel1);
		userInput.Tel1_1 = this.IsVisible_UserTel1 ? tel1.Tel1 : null;
		userInput.Tel1_2 = this.IsVisible_UserTel1 ? tel1.Tel2 : null;
		userInput.Tel1_3 = this.IsVisible_UserTel1 ? tel1.Tel3 : null;
		userInput.Tel1 = this.IsVisible_UserTel1
			? (string.IsNullOrEmpty(tel1.TelNo) == false)
				? tel1.TelNo
				: inputTel1
			: null;

		// Set value for telephone 2
		var inputTel2 = (this.WtbUserTel2_2.HasInnerControl)
			? StringUtility.ToHankaku(this.WtbUserTel2_1.Text.Trim())
			: StringUtility.ToHankaku(this.WtbUserTel1_2.Text.Trim());
		if (this.IsVisible_UserTel2
			&& this.WtbUserTel2_2.HasInnerControl)
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

		userInput.MailFlg = this.IsVisible_UserMailFlg
			? (this.WcbUserMailFlg.Checked ? Constants.FLG_USER_MAILFLG_OK : Constants.FLG_USER_MAILFLG_NG)
			: null;
		userInput.LoginId = StringUtility.ToHankaku(this.WtbUserLoginId.Text);
		userInput.Password = this.IsVisible_UserPassword ? StringUtility.ToHankaku(this.WtbUserPassword.Text) : null;
		userInput.PasswordConf = this.IsVisible_UserPassword ? StringUtility.ToHankaku(this.WtbUserPasswordConf.Text) : null;
		userInput.AdvcodeFirst = StringUtility.ToEmpty(Session[Constants.SESSION_KEY_ADVCODE_NOW]);	// 広告コード格納（新規登録時のみ）
		userInput.RemoteAddr = this.Page.Request.ServerVariables["REMOTE_ADDR"];
		userInput.RecommendUid = UserCookieManager.UniqueUserId;
		userInput.MemberRankId = MemberRankOptionUtility.GetDefaultMemberRank(); // デフォルト会員ランクの設定
		userInput.LastChanged = Constants.FLG_LASTCHANGED_USER;
		userInput.AddrCountryIsoCode = this.IsVisible_UserCountry ? this.WddlUserCountry.SelectedValue : null;
		userInput.AddrCountryName = this.IsVisible_UserCountry ? this.WddlUserCountry.SelectedText : null;

		// Amazonアドレス帳ウィジェットを表示している場合の処理
		if (this.IsVisible_AmazonAddressWidget)
		{
			// OrderReference取得
			var amazonModel = (AmazonModel)Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL];
			var res = AmazonApiFacade.GetOrderReferenceDetails(this.WhfAmazonOrderRefID.Value, amazonModel.Token);
			var amazonAddressInput = new AmazonAddressInput(res);
			var amazonAddress = AmazonAddressParser.Parse(amazonAddressInput);

			userInput.Zip1 = amazonAddress.Zip1;
			userInput.Zip2 = amazonAddress.Zip2;
			userInput.Zip = userInput.Zip1 + "-" + userInput.Zip2;
			userInput.Addr1 = amazonAddress.Addr1;
			userInput.Addr2 = StringUtility.ToZenkaku(amazonAddress.Addr2).Trim();
			// Addr3,Addr4は空文字になる可能性があるので、nullにすることで必須チェック回避
			userInput.Addr3 = string.IsNullOrEmpty(amazonAddress.Addr3) ? null : StringUtility.ToZenkaku(amazonAddress.Addr3).Trim();
			userInput.Addr4 = string.IsNullOrEmpty(amazonAddress.Addr4) ? null : StringUtility.ToZenkaku(amazonAddress.Addr4).Trim();
			userInput.Addr = userInput.Addr1 + userInput.Addr2 + userInput.Addr3 + " " + userInput.Addr4;
			if (IsVisible_UserTel1 == false)
			{
				userInput.Tel1 = amazonAddress.Tel;
				userInput.Tel1_1 = amazonAddress.Tel1;
				userInput.Tel1_2 = amazonAddress.Tel2;
				userInput.Tel1_3 = amazonAddress.Tel3;
			}
		}

		if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED)
		{
			userInput.AuthenticationCode = this.WtbAuthenticationCode.Text;
			userInput.HasAuthenticationCode = this.HasAuthenticationCode;

			if (this.IsVisible_UserTel1 == false) userInput.HasAuthenticationCode = true;
		}

		return userInput;
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitComponents()
	{
		//------------------------------------------------------
		// コンポーネント初期化
		//------------------------------------------------------
		var settings = new UserService().GetUserEasyRegisterSettingList();

		foreach (var setting in settings)
		{
			var itemId = setting.ItemId;
			var visible = (setting.DisplayFlg == Constants.FLG_USER_EASY_REGISTER_FLG_EASY);

			switch (itemId)
			{
				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_ADDR1:
					this.IsVisible_UserAddr1 = (visible && (Constants.GLOBAL_OPTION_ENABLE == false));
					break;
				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_ADDR2:
					this.IsVisible_UserAddr2 = (visible && (Constants.GLOBAL_OPTION_ENABLE == false));
					break;
				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_ADDR3:
					this.IsVisible_UserAddr3 = (visible && (Constants.GLOBAL_OPTION_ENABLE == false));
					break;
				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_ADDR4:
					this.IsVisible_UserAddr4 = (visible && (Constants.GLOBAL_OPTION_ENABLE == false));
					break;
				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_BIRTH:
					this.IsVisible_UserBirth = visible;
					break;
				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_COMPANY_NAME:
					this.IsVisible_UserCompanyName = visible;
					break;
				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_COMPANY_POST_NAME:
					this.IsVisible_UserCompanyPostName = visible;
					break;
				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_MAIL_ADDR2:
					this.IsVisible_UserMailAddr2 = visible;
					this.IsVisible_UserMailAddr2Conf = visible;
					break;
				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_MAIL_FLG:
					this.IsVisible_UserMailFlg = visible;
					break;
				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_NAME:
					this.IsVisible_UserName = visible;
					break;
				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_NAME_KANA:
					this.IsVisible_UserNameKana = visible;
					break;
				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_NICK_NAME:
					this.IsVisible_UserNickName = visible;
					break;
				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_SEX:
					this.IsVisible_UserSex = visible;
					break;
				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_TEL1:
					this.IsVisible_UserTel1 = visible;
					break;
				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_TEL2:
					this.IsVisible_UserTel2 = visible;
					break;
				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_ZIP:
					this.IsVisible_UserZip = (visible && (Constants.GLOBAL_OPTION_ENABLE == false));
					break;
				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_COUNTRY:
					this.IsVisible_UserCountry = (visible && Constants.GLOBAL_OPTION_ENABLE);
					break;
				default:
					break;
			}
		}

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

		// 都道府県ドロップダウン作成
		this.WddlUserAddr1.AddItem(new ListItem("", ""));
		foreach (var strPrefecture in Constants.STR_PREFECTURES_LIST)
		{
			this.WddlUserAddr1.AddItem(new ListItem(strPrefecture));
		}

		// 国ドロップダウン作成
		var countries = GlobalAddressUtil.GetCountriesAll();
		this.WddlUserCountry.AddItems(countries.Select(c => new ListItem(c.CountryName, c.CountryIsoCode)).ToArray());
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
	/// ペイパル認証完了
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbPayPalAuthComplete_Click(object sender, EventArgs e)
	{
		// ペイパル認証情報をセッションにセット
		var payPalScriptsForm = GetWrappedControl<WrappedPayPalPayScriptsFormControl>("ucPaypalScriptsForm");
		SetPaypalInfoToSession(payPalScriptsForm);

		// ユーザーが見つかればログインさせ、見つからなければ入力補完
		var user = PayPalUtility.Account.GetUserByPayPalCustomerId(SessionManager.PayPalLoginResult.CustomerId);
		if (user != null)
		{
			// ログイン成功処理＆次の画面へ遷移
			ExecLoginSuccessProcessAndGoNextForLogin(
				user,
				Constants.PATH_ROOT,
				false,
				LoginType.PayPal,
				UpdateHistoryAction.Insert);
		}

		SetDefaultToForm();
	}

	/// <summary>
	/// Amazonアドレス帳ウィジェット情報取得
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

		// アドレス帳ウィジェットから住所情報取得
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
		return JsonConvert.SerializeObject(new { Error = string.Empty, Input = input });
	}

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

		var customValidatorControls = new[]
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
	/// DropDownList user addr country selected index changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlUserAddrCountry_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED
			&& IsPostBack)
		{
			// Stop process validation when changing country
			StopTimeCount();

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

			this.HasAuthenticationCode = false;
			this.WlbAuthenticationStatus.Text = string.Empty;

			DisplayAuthenticationCode(
				this.WlbGetAuthenticationCode,
				this.WtbAuthenticationCode);
		}
	}

	/// <summary>
	/// Link button get authentication code click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbGetAuthenticationCode_Click(object sender, EventArgs e)
	{
		this.HasAuthenticationCode = false;
		ChangeControlLooksForValidator(
			new Dictionary<string, string> { { string.Empty, string.Empty } },
			Constants.CONST_KEY_USER_AUTHENTICATION_CODE,
			this.WcvAuthenticationCode,
			this.WtbAuthenticationCode);

		var telephone = GetValueForTelephone(
			this.WtbUserTel1,
			this.WtbUserTel2,
			this.WtbUserTel3,
			this.WtbUserTel1_1,
			this.WtbUserTel1_1,
			this.UserAddrCountryIsoCode);

		SendAuthenticationCode(
			this.WtbAuthenticationCode,
			this.WlbAuthenticationStatus,
			telephone,
			this.UserAddrCountryIsoCode);
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
			this.WcvAuthenticationCode,
			this.WtbAuthenticationCode);

		// Reset authentication code
		if (string.IsNullOrEmpty(this.WhfResetAuthenticationCode.Value) == false)
		{
			this.HasAuthenticationCode = false;
			this.WlbAuthenticationStatus.Visible = true;

			DisplayAuthenticationCode(
				this.WlbGetAuthenticationCode,
				this.WtbAuthenticationCode,
				this.WlbAuthenticationStatus);
			return;
		}

		var telephone = GetValueForTelephone(
			this.WtbUserTel1,
			this.WtbUserTel2,
			this.WtbUserTel3,
			this.WtbUserTel1_1,
			this.WtbUserTel1_1,
			this.UserAddrCountryIsoCode);

		// Exec check authentication code
		var errorMessages = ExecCheckAuthenticationCode(
			this.WlbGetAuthenticationCode,
			this.WtbAuthenticationCode,
			this.WlbAuthenticationMessage,
			this.WlbAuthenticationStatus,
			telephone,
			this.UserAddrCountryIsoCode);

		if (errorMessages.Count > 0)
		{
			this.HasAuthenticationCode = false;
			GetErrMsgAndFocusToCV(errorMessages);
			return;
		}

		this.HasAuthenticationCode = true;
		RemoveErrorInputClass(this.WtbAuthenticationCode);
	}

	/// <summary>
	/// Cooperation cross point api
	/// </summary>
	/// <param name="user">User model</param>
	private void CooperationCrossPointApi(UserModel user)
	{
		var result = new CrossPointUserApiService().Insert(user);
		if (result.IsSuccess == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = result.ErrorCodeList.Contains(
					w2.App.Common.Constants.CROSS_POINT_RESULT_DUPLICATE_MEMBER_ID_ERROR_CODE)
				? result.ErrorMessage
				: MessageManager.GetMessages(w2.App.Common.Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR);

			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		var userApiResult = new CrossPointUserApiService().Get(user.UserId);
		if (userApiResult == null)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = MessageManager.GetMessages(
				w2.App.Common.Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR);

			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		user.UserExtend.UserExtendDataValue.CrossPointShopCardNo = userApiResult.RealShopCardNo;
		user.UserExtend.UserExtendDataValue.CrossPointShopCardPin = userApiResult.PinCode;
	}

	#region プロパティ
	/// <summary>郵便番号入力チェックエラー文言</summary>
	protected string ZipInputErrorMessage
	{
		get { return StringUtility.ToEmpty(ViewState["ZipInputErrorMessage"]); }
		set { ViewState["ZipInputErrorMessage"] = value; }
	}
	// TODO：プロパティの命名規則が規約違反のため、あとで修正する
	/// <summary>「氏名」入力欄を表示するか</summary>
	protected bool IsVisible_UserName
	{
		get { return (bool)ViewState["UserName"]; }
		set { ViewState["UserName"] = value; }
	}
	/// <summary>「氏名(かな)」入力欄を表示するか</summary>
	protected bool IsVisible_UserNameKana
	{
		get { return (bool)ViewState["UserNameKana"]; }
		set { ViewState["UserNameKana"] = value; }
	}
	/// <summary>「ニックネーム」入力欄を表示するか</summary>
	protected bool IsVisible_UserNickName
	{
		get { return (bool)ViewState["UserNickName"]; }
		set { ViewState["UserNickName"] = value; }
	}
	/// <summary>「生年月日」入力欄を表示するか</summary>
	protected bool IsVisible_UserBirth
	{
		get { return (bool)ViewState["UserBirth"]; }
		set { ViewState["UserBirth"] = value; }
	}
	/// <summary>「性別」入力欄を表示するか</summary>
	protected bool IsVisible_UserSex
	{
		get { return (bool)ViewState["UserSex"]; }
		set { ViewState["UserSex"] = value; }
	}
	/// <summary>「モバイルメールアドレス」入力欄を表示するか</summary>
	protected bool IsVisible_UserMailAddr2
	{
		get { return (bool)ViewState["UserMailAddr2"]; }
		set { ViewState["UserMailAddr2"] = value; }
	}
	/// <summary>「モバイルメールアドレス(確認用)」入力欄を表示するか</summary>
	protected bool IsVisible_UserMailAddr2Conf
	{
		get { return (bool)ViewState["UserMailAddr2Conf"]; }
		set { ViewState["UserMailAddr2Conf"] = value; }
	}
	/// <summary>「郵便番号」入力欄を表示するか</summary>
	protected bool IsVisible_UserZip
	{
		get { return (bool)ViewState["UserZip"]; }
		set { ViewState["UserZip"] = value; }
	}
	/// <summary>「都道府県」入力欄を表示するか</summary>
	protected bool IsVisible_UserAddr1
	{
		get { return (bool)ViewState["UserAddr1"]; }
		set { ViewState["UserAddr1"] = value; }
	}
	/// <summary>「市区町村」入力欄を表示するか</summary>
	protected bool IsVisible_UserAddr2
	{
		get { return (bool)ViewState["UserAddr2"]; }
		set { ViewState["UserAddr2"] = value; }
	}
	/// <summary>「番地」入力欄を表示するか</summary>
	protected bool IsVisible_UserAddr3
	{
		get { return (bool)ViewState["UserAddr3"]; }
		set { ViewState["UserAddr3"] = value; }
	}
	/// <summary>「ビル・マンション名」入力欄を表示するか</summary>
	protected bool IsVisible_UserAddr4
	{
		get { return (bool)ViewState["UserAddr4"]; }
		set { ViewState["UserAddr4"] = value; }
	}
	/// <summary>「企業名」入力欄を表示するか</summary>
	protected bool IsVisible_UserCompanyName
	{
		get { return (bool)ViewState["UserCompanyName"]; }
		set { ViewState["UserCompanyName"] = value; }
	}
	/// <summary>「部署名」入力欄を表示するか</summary>
	protected bool IsVisible_UserCompanyPostName
	{
		get { return (bool)ViewState["UserCompanyPostName"]; }
		set { ViewState["UserCompanyPostName"] = value; }
	}
	/// <summary>「電話番号」入力欄を表示するか</summary>
	protected bool IsVisible_UserTel1
	{
		get { return (bool)ViewState["UserTel1"]; }
		set { ViewState["UserTel1"] = value; }
	}
	/// <summary>「電話番号(予備)」入力欄を表示するか</summary>
	protected bool IsVisible_UserTel2
	{
		get { return (bool)ViewState["UserTel2"]; }
		set { ViewState["UserTel2"] = value; }
	}
	/// <summary>「お知らせメールの配信希望」入力欄を表示するか</summary>
	protected bool IsVisible_UserMailFlg
	{
		get { return (bool)ViewState["UserMailFlg"]; }
		set { ViewState["UserMailFlg"] = value; }
	}
	/// <summary>「パスワード」入力欄を表示するか</summary>
	protected bool IsVisible_UserPassword
	{
		get { return (bool)(ViewState["UserPassword"] ?? true); }
		set { ViewState["UserPassword"] = value; }
	}
	/// <summary>「国」入力欄を表示するか</summary>
	protected bool IsVisible_UserCountry
	{
		get { return (bool)ViewState["UserCountry"]; }
		set { ViewState["UserCountry"] = value; }
	}
	/// <summary>Amazonウィジェットを表示するかどうか</summary>
	protected bool IsVisible_AmazonAddressWidget
	{
		get
		{
			return this.IsAmazonLoggedIn
				&& Constants.AMAZON_LOGIN_OPTION_ENABLED
				&& (Constants.AMAZON_PAYMENT_CV2_ENABLED == false)
				&& (this.IsVisible_UserZip
					|| this.IsVisible_UserAddr1
					|| this.IsVisible_UserAddr2
					|| this.IsVisible_UserAddr3
					|| (this.IsVisible_UserAddr4 && Constants.DISPLAY_ADDR4_ENABLED));
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
	/// <summary>User addr country iso code</summary>
	public string UserAddrCountryIsoCode
	{
		get
		{
			if (Constants.GLOBAL_OPTION_ENABLE == false) return Constants.COUNTRY_ISO_CODE_JP;

			var result = this.IsVisible_UserCountry
				? this.WddlUserCountry.SelectedValue
				: Constants.COUNTRY_ISO_CODE_JP;
			return result;
		}
	}
	#endregion
}
