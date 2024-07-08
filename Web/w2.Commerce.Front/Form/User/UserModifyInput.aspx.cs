/*
=========================================================================================================
  Module      : 会員登録変更入力画面処理(UserModifyInput.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.Amazon.Util;
using w2.App.Common.Global;
using w2.App.Common.Line.Util;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.User;
using w2.App.Common.User.SocialLogin.Util;
using w2.App.Common.Util;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain.User;
using w2.Domain.User.Helper;

public partial class Form_User_UserModifyInput : UserPage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return true; } }	// ログイン必須
	/// <summary>マイページメニュー表示判定</summary>
	public override bool DispMyPageMenu { get { return true; } }	// マイページメニュー表示

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
	protected WrappedDropDownList WddlUserCountry { get { return GetWrappedControl<WrappedDropDownList>("ddlUserCountry"); } }
	protected WrappedTextBox WtbUserZipGlobal { get { return GetWrappedControl<WrappedTextBox>("tbUserZipGlobal"); } }
	protected WrappedTextBox WtbUserZip1 { get { return GetWrappedControl<WrappedTextBox>("tbUserZip1"); } }
	protected WrappedTextBox WtbUserZip2 { get { return GetWrappedControl<WrappedTextBox>("tbUserZip2"); } }
	protected WrappedDropDownList WddlUserAddr1 { get { return GetWrappedControl<WrappedDropDownList>("ddlUserAddr1"); } }
	protected WrappedTextBox WtbUserAddr2 { get { return GetWrappedControl<WrappedTextBox>("tbUserAddr2"); } }
	protected WrappedTextBox WtbUserAddr3 { get { return GetWrappedControl<WrappedTextBox>("tbUserAddr3"); } }
	protected WrappedTextBox WtbUserAddr4 { get { return GetWrappedControl<WrappedTextBox>("tbUserAddr4"); } }
	protected WrappedTextBox WtbUserAddr5 { get { return GetWrappedControl<WrappedTextBox>("tbUserAddr5"); } }
	protected WrappedDropDownList WddlUserAddr5 { get { return GetWrappedControl<WrappedDropDownList>("ddlUserAddr5"); } }
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
	protected WrappedTextBox WtbUserLoginId
	{
		get
		{
			if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED)
			{
				return GetWrappedControl<WrappedTextBox>(this.IsPcSiteOrOfflineUser ? "tbUserMailAddr" : "tbUserMailAddr2");
			}
			else
			{
				return GetWrappedControl<WrappedTextBox>("tbUserLoginId");
			}
		}
	}
	protected WrappedTextBox WtbUserPasswordBefore { get { return GetWrappedControl<WrappedTextBox>("tbUserPasswordBefore"); } }
	protected WrappedTextBox WtbUserPassword { get { return GetWrappedControl<WrappedTextBox>("tbUserPassword"); } }
	protected WrappedTextBox WtbUserPasswordConf { get { return GetWrappedControl<WrappedTextBox>("tbUserPasswordConf"); } }
	protected WrappedCustomValidator WcvUserBirthYear { get { return GetWrappedControl<WrappedCustomValidator>("cvUserBirthYear"); } }
	protected WrappedCustomValidator WcvUserBirthMonth { get { return GetWrappedControl<WrappedCustomValidator>("cvUserBirthMonth"); } }
	protected WrappedCustomValidator WcvUserBirthDay { get { return GetWrappedControl<WrappedCustomValidator>("cvUserBirthDay"); } }
	protected WrappedCustomValidator WcvUserSex { get { return GetWrappedControl<WrappedCustomValidator>("cvUserSex"); } }
	protected WrappedCustomValidator WcvUserMailAddr { get { return GetWrappedControl<WrappedCustomValidator>("cvUserMailAddr"); } }
	protected WrappedCustomValidator WcvUserMailAddrConf { get { return GetWrappedControl<WrappedCustomValidator>("cvUserMailAddrConf"); } }
	protected WrappedCustomValidator WcvUserMailAddrForCheck { get { return GetWrappedControl<WrappedCustomValidator>("cvUserMailAddrForCheck"); } }
	protected WrappedCustomValidator WcvUserMailAddr2 { get { return GetWrappedControl<WrappedCustomValidator>("cvUserMailAddr2"); } }
	protected WrappedCustomValidator WcvUserMailAddr2Conf { get { return GetWrappedControl<WrappedCustomValidator>("cvUserMailAddr2Conf"); } }
	protected WrappedCustomValidator WcvUserMailAddr2ForCheck { get { return GetWrappedControl<WrappedCustomValidator>("cvUserMailAddr2ForCheck"); } }
	protected WrappedCustomValidator WcvUserAddr2 { get { return GetWrappedControl<WrappedCustomValidator>("cvUserAddr2"); } }
	protected WrappedCustomValidator WcvUserAddr3 { get { return GetWrappedControl<WrappedCustomValidator>("cvUserAddr3"); } }
	protected WrappedCustomValidator WcvUserAddr4 { get { return GetWrappedControl<WrappedCustomValidator>("cvUserAddr4"); } }
	protected WrappedCustomValidator WcvUserLoginId
	{
		get
		{
			if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED)
			{
				return GetWrappedControl<WrappedCustomValidator>(this.IsPcSiteOrOfflineUser ? "cvUserMailAddr" : "cvUserMailAddr2");
			}
			else
			{
				return GetWrappedControl<WrappedCustomValidator>("cvUserLoginId");
			}
		}
	}
	protected WrappedCustomValidator WcvUserPasswordBefore { get { return GetWrappedControl<WrappedCustomValidator>("cvUserPasswordBefore"); } }
	protected WrappedCustomValidator WcvUserPassword { get { return GetWrappedControl<WrappedCustomValidator>("cvUserPassword"); } }
	protected WrappedCustomValidator WcvUserPasswordConf { get { return GetWrappedControl<WrappedCustomValidator>("cvUserPasswordConf"); } }
	protected WrappedHtmlGenericControl WhgcCountryAlertMessage { get { return GetWrappedControl<WrappedHtmlGenericControl>("countryAlertMessage"); } }
	protected WrappedLinkButton WlbConfirm { get { return GetWrappedControl<WrappedLinkButton>("lbConfirm"); } }
	protected WrappedDropDownList WddlUserAddr2 { get { return GetWrappedControl<WrappedDropDownList>("ddlUserAddr2"); } }
	protected WrappedDropDownList WddlUserAddr3 { get { return GetWrappedControl<WrappedDropDownList>("ddlUserAddr3"); } }
	protected WrappedLinkButton WlbSearchAddr { get { return GetWrappedControl<WrappedLinkButton>("lbSearchAddr"); } }
	protected WrappedTextBox WtbUserZip { get { return GetWrappedControl<WrappedTextBox>("tbUserZip"); } }
	protected WrappedTextBox WtbUserTel1_1 { get { return GetWrappedControl<WrappedTextBox>("tbUserTel1_1"); } }
	protected WrappedTextBox WtbUserTel1_2 { get { return GetWrappedControl<WrappedTextBox>("tbUserTel1_2"); } }
	protected WrappedLinkButton WlbSearchAddrFromZipGlobal { get { return GetWrappedControl<WrappedLinkButton>("lbSearchAddrFromZipGlobal"); } }

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

	/// <summary>Wrapped custom validator authentication code</summary>
	protected WrappedCustomValidator WcvAuthenticationCode { get { return GetWrappedControl<WrappedCustomValidator>("cvAuthenticationCode"); } }
	/// <summary>Wrapped custom validator authentication code global</summary>
	protected WrappedCustomValidator WcvAuthenticationCodeGlobal { get { return GetWrappedControl<WrappedCustomValidator>("cvAuthenticationCodeGlobal"); } }
	/// <summary>Wrapped hidden field reset authentication Code</summary>
	protected WrappedHiddenField WhfResetAuthenticationCode { get { return GetWrappedControl<WrappedHiddenField>("hfResetAuthenticationCode"); } }
	/// <summary>Wrapped custom validator user tel 1 global</summary>
	protected WrappedCustomValidator WcvUserTel1Global { get { return GetWrappedControl<WrappedCustomValidator>("cvUserTel1Global"); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// ログインチェック（ログイン後は顧客変更入力ページから）
		//------------------------------------------------------
		CheckLoggedIn(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_MODIFY_INPUT);

		//------------------------------------------------------
		// HTTPS通信チェック（HTTPのとき、トップ画面へ）
		//------------------------------------------------------
		CheckHttps();

		if (!IsPostBack)
		{
			//ユーザーが退会済みでないか確認
			if (string.IsNullOrEmpty(this.LoginUserId) == false)
			{
				var user = new UserService().Get(this.LoginUserId);
				if ((user != null) && user.IsDeleted)
				{
					Session.Contents.RemoveAll();
					CookieManager.RemoveCookie(Constants.COOKIE_KEY_AUTH_KEY, Constants.PATH_ROOT);
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CART_SESSION_VANISHED);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}
			}

			//------------------------------------------------------
			// コンポーネント初期化
			//------------------------------------------------------
			InitComponents();

			if (this.SessionParamTargetPage == Constants.PAGE_FRONT_USER_MODIFY_INPUT)
			{
				// セッション情報から会員情報を入力欄にセットする
				UserInput userInput = (UserInput)Session[Constants.SESSION_KEY_PARAM];
				var user = userInput.CreateModel();
				this.DisplayUserInfo(user);
				this.WtbUserPasswordBefore.Text = userInput.PasswordBefore;
				this.WtbUserPassword.Text = user.PasswordDecrypted;
				this.WtbUserPasswordConf.Text = user.PasswordDecrypted;

				if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED
					&& userInput.HasAuthenticationCode)
				{
					this.HasAuthenticationCode = userInput.HasAuthenticationCode;

					DisplayAuthenticationCode(
						this.IsUserAddrJp ? this.WlbGetAuthenticationCode : this.WlbGetAuthenticationCodeGlobal,
						this.IsUserAddrJp ? this.WtbAuthenticationCode : this.WtbAuthenticationCodeGlobal,
						authenticationCode : userInput.AuthenticationCode);
				}

#if !DEBUG
				// ターゲットページ情報をクリアする
				this.SessionParamTargetPage = null;
#endif
			}
			else
			{
				if (Constants.CROSS_POINT_OPTION_ENABLED)
				{
					UserUtility.AdjustPointAndMemberRankByCrossPointApi(this.LoginUser);
				}

				// ユーザ情報取得
				var user = new UserService().Get(this.LoginUserId);
				if (user != null) DisplayUserInfo(user);

				if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED)
				{
					this.HasAuthenticationCode = true;
				}
			}

			// 国切替初期化
			ddlUserAddrCountry_SelectedIndexChanged(sender, e);
		}
	}

	/// <summary>
	/// 画面にユーザー情報を表示する
	/// </summary>
	/// <param name="user">ユーザー情報</param>
	private void DisplayUserInfo(UserModel user)
	{
		this.UserKbn = user.UserKbn;
		this.WtbUserName1.Text = user.Name1;
		this.WtbUserName2.Text = user.Name2;
		this.WtbUserNameKana1.Text = user.NameKana1;
		this.WtbUserNameKana2.Text = user.NameKana2;
		this.WtbUserNickName.Text = user.NickName;
		this.WddlUserBirthYear.SelectItemByValue(user.BirthYear);
		this.WddlUserBirthMonth.SelectItemByValue(user.BirthMonth.TrimStart('0'));
		this.WddlUserBirthDay.SelectItemByValue(user.BirthDay.TrimStart('0'));
		this.WrblUserSex.SelectItemByValue(user.Sex);
		this.WtbUserMailAddr.Text = user.MailAddr;
		this.WtbUserMailAddrConf.Text = user.MailAddr;
		this.WtbUserMailAddr2.Text = user.MailAddr2;
		this.WtbUserMailAddr2Conf.Text = user.MailAddr2;
		this.WddlUserCountry.SelectItemByValue(user.AddrCountryIsoCode);
		this.WtbUserZipGlobal.Text = user.Zip;

		// Set value for zip code
		SetZipCodeTextbox(
			this.WtbUserZip,
			this.WtbUserZip1,
			this.WtbUserZip2,
			user.Zip);

		this.WddlUserAddr1.SelectItemByText(user.Addr1);
		this.WtbUserAddr2.Text = user.Addr2;
		this.WtbUserAddr3.Text = user.Addr3;
		this.WtbUserAddr4.Text = user.Addr4;

		if (IsCountryUs(user.AddrCountryIsoCode))
		{
			this.WddlUserAddr5.SelectItemByText(user.Addr5);
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
			this.WddlUserAddr3.DataSource = this.UserTwDistrictDict[this.WddlUserAddr2.SelectedItem.ToString()];
			this.WddlUserAddr3.DataBind();
			this.WddlUserAddr3.ForceSelectItemByText(user.Addr3);
			this.WtbUserZipGlobal.Text = user.Zip;
		}

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

		this.WtbUserTel1Global.Text = user.Tel1;
		this.WtbUserTel2Global.Text = user.Tel2;
		this.WcbUserMailFlg.Checked = (user.MailFlg == Constants.FLG_USER_MAILFLG_OK);
		if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED == false) this.WtbUserLoginId.Text = user.LoginId;
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitComponents()
	{
		var settings = new UserService().GetUserEasyRegisterSettingList();

		foreach (var setting in settings)
		{
			var itemId = setting.ItemId;
			var visible = (setting.DisplayFlg == Constants.FLG_USER_EASY_REGISTER_FLG_EASY);

			switch (itemId)
			{
				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_ADDR1:
					this.IsUserAddr1Necessary = ((this.IsEasyUser == false) || (visible && (Constants.GLOBAL_OPTION_ENABLE == false)));
					break;
				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_ADDR2:
					this.IsUserAddr2Necessary = ((this.IsEasyUser == false) || (visible && (Constants.GLOBAL_OPTION_ENABLE == false)));
					break;
				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_ADDR3:
					this.IsUserAddr3Necessary = ((this.IsEasyUser == false) || (visible && (Constants.GLOBAL_OPTION_ENABLE == false)));
					break;
				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_NAME:
					this.IsUserNameNecessary = ((this.IsEasyUser == false) || visible);
					break;
				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_NAME_KANA:
					this.IsUserNameKanaNecessary = ((this.IsEasyUser == false) || visible);
					break;
				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_BIRTH:
					this.IsUserBirthNecessary = ((this.IsEasyUser == false) || visible);
					break;
				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_SEX:
					this.IsUserSexNecessary = ((this.IsEasyUser == false) || visible);
					break;
				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_TEL1:
					this.IsUserTel1Necessary = ((this.IsEasyUser == false) || visible);
					break;
				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_ZIP:
					this.IsUserZipNecessary = ((this.IsEasyUser == false) || (visible && (Constants.GLOBAL_OPTION_ENABLE == false)));
					break;
				case Constants.FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_COUNTRY:
					this.IsUserCountryNecessary = ((this.IsEasyUser == false) || (visible && Constants.GLOBAL_OPTION_ENABLE));
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

		// 生年月日ドロップダウン作成
		this.WddlUserBirthYear.Items.Add("");
		this.WddlUserBirthYear.AddItems(DateTimeUtility.GetBirthYearListItem());
		this.WddlUserBirthMonth.Items.Add("");
		this.WddlUserBirthMonth.AddItems(DateTimeUtility.GetMonthListItem());
		this.WddlUserBirthDay.Items.Add("");
		this.WddlUserBirthDay.AddItems(DateTimeUtility.GetDayListItem());

		// 都道府県ドロップダウン作成
		this.WddlUserAddr1.AddItem(new ListItem("", ""));
		foreach (string strPrefecture in Constants.STR_PREFECTURES_LIST)
		{
			this.WddlUserAddr1.AddItem(new ListItem(strPrefecture));
		}

		// 国ドロップダウン作成
		var countries = GlobalAddressUtil.GetCountriesAll();
		this.WddlUserCountry.Items.Add(new ListItem("", ""));
		this.WddlUserCountry.Items.AddRange(countries.Select(c => new ListItem(c.CountryName, c.CountryIsoCode)).ToArray());
		// 州ドロップダウン作成
		this.WddlUserAddr5.Items.Add(new ListItem("", ""));
		this.WddlUserAddr5.Items.AddRange(Constants.US_STATES_LIST.Select(state => new ListItem(state)).ToArray());

		// ソーシャルログイン連携されている場合は、パスワード非表示とする
		var isSocialLogin = (Constants.SOCIAL_LOGIN_ENABLED && (SocialLoginUtil.GetProviders(null, this.LoginUserId).Any()));
		var isAmazonLogin = (Constants.AMAZON_LOGIN_OPTION_ENABLED && Constants.AMAZON_PAYMENT_OPTION_ENABLED) && (string.IsNullOrEmpty(AmazonUtil.GetAmazonUserIdByUseId(this.LoginUserId)) == false);
		var isPayPalLogin = (Constants.PAYPAL_LOGINPAYMENT_ENABLED
			&& (SessionManager.PayPalCooperationInfo != null)
			&& (PayPalUtility.Account.GetUserByPayPalCustomerId(SessionManager.PayPalCooperationInfo.CustomerId) != null));
		// ライン直接連携は、パスワードが存在しない場合のみ、パスワードを非表示にする
		var isLineDirectLogin = w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED
			&& (Constants.SOCIAL_LOGIN_ENABLED == false)
			&& (string.IsNullOrEmpty(LineUtil.GetLineUserIdByLoginUserId(this.LoginUserId)) == false)
			&& string.IsNullOrEmpty(this.LoginUser.Password);
		this.IsVisible_UserPassword = (isSocialLogin == false)
			&& (isAmazonLogin == false)
			&& (isPayPalLogin == false)
			&& (isLineDirectLogin == false);
		if (isAmazonLogin)
		{
			if (this.WddlUserCountry.InnerControl != null) this.WddlUserCountry.InnerControl.Enabled = false;
			this.WhgcCountryAlertMessage.Visible = true;
		}

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
		//ユーザーが退会済みでないか確認
		var user = new UserService().Get(this.LoginUserId);
		if (user.IsDeleted)
		{
			Session.Contents.RemoveAll();
			CookieManager.RemoveCookie(Constants.COOKIE_KEY_AUTH_KEY, Constants.PATH_ROOT);
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_NO_USER_SESSION);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		// Set has authentication code
		if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED)
		{
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

			if (user.Tel1 == telephone) this.HasAuthenticationCode = true;
		}

		// 入力情報取得
		var input = CreateInputData();
		var excludeList = new List<string>();
		if (this.WtbUserTel1_1.HasInnerControl == false)
		{
			excludeList.Add(Constants.FIELD_USER_TEL1);
		}
		if (this.WtbUserTel1_2.HasInnerControl == false)
		{
			excludeList.Add(Constants.FIELD_USER_TEL2);
		}
		var userInputValidate = this.IsEasyUser ? CreateInputDataEasyUser() : input;
		var errorMessages = (this.IsUserAddrJp)
			? userInputValidate.Validate(UserInput.EnumUserInputValidationKbn.UserModify)
			: userInputValidate.Validate(UserInput.EnumUserInputValidationKbn.UserModifyGlobal);

		if (errorMessages.Count == 0)
		{
			// ユーザー拡張項目情報検証
			errorMessages = userInputValidate.UserExtendInput.Validate();
		}

		if (Constants.CROSS_POINT_OPTION_ENABLED
			&& (input.UserExtendInput.UserExtendDataValue.ContainsKey(Constants.CROSS_POINT_USREX_SHOP_CARD_NO))
			&& (input.UserExtendInput.UserExtendDataValue.ContainsKey(Constants.CROSS_POINT_USREX_SHOP_CARD_PIN)))
		{
			var beforeShopCardNo = user.UserExtend.UserExtendDataValue[Constants.CROSS_POINT_USREX_SHOP_CARD_NO];
			var beforeShopCardPin = user.UserExtend.UserExtendDataValue[Constants.CROSS_POINT_USREX_SHOP_CARD_PIN];
			var afterShopCardNo = input.UserExtendInput.UserExtendDataValue[Constants.CROSS_POINT_USREX_SHOP_CARD_NO];
			var afterShopCardPin = input.UserExtendInput.UserExtendDataValue[Constants.CROSS_POINT_USREX_SHOP_CARD_PIN];
			SessionManager.UpdatedShopCardNoAndPinFlg = true;

			if ((string.IsNullOrEmpty(afterShopCardNo)
					&& (string.IsNullOrEmpty(afterShopCardPin) == false))
				|| ((string.IsNullOrEmpty(afterShopCardNo) == false)
					&& string.IsNullOrEmpty(afterShopCardPin)))
			{
				var shopCardNoAndPinCodeErrorMessage =
					w2.App.Common.CommerceMessages.ERRMSG_FRONT_POINT_CARD_REGISTER_ERROR;

				if (errorMessages.ContainsKey(Constants.CROSS_POINT_USREX_SHOP_CARD_NO))
				{
					errorMessages[Constants.CROSS_POINT_USREX_SHOP_CARD_NO] =
						WebMessages.GetMessages(shopCardNoAndPinCodeErrorMessage);
				}
				else
				{
					errorMessages.Add(
						Constants.CROSS_POINT_USREX_SHOP_CARD_NO,
						WebMessages.GetMessages(shopCardNoAndPinCodeErrorMessage));
				}
			}
			else if (string.IsNullOrEmpty(afterShopCardNo)
					&& string.IsNullOrEmpty(afterShopCardPin))
			{
				SessionManager.MemberIdForCrossPoint = beforeShopCardNo;
				SessionManager.PinCodeForCrossPoint = beforeShopCardPin;
				SessionManager.UpdatedShopCardNoAndPinFlg = false;
			}
		}

		if (errorMessages.Count == 0)
		{
			if (this.IsEasyUser)
			{
				input.EasyRegisterFlg = Constants.FLG_USER_EASY_REGISTER_FLG_EASY;
			}

			// 問題なければ次画面に遷移する
			GoNext(input);
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

			// カスタムバリデータにメッセージを表示させる
			// ユーザ拡張項目のバリデータチェックはUC側実施
			GetErrMsgAndFocusToCV(errorMessages);
		}
	}

	/// <summary>
	/// 次画面へ遷移
	/// </summary>
	private void GoNext(UserInput input)
	{
		// 問題なければ次画面に遷移する
		Session[Constants.SESSION_KEY_PARAM] = input;
		// 画面遷移の正当性チェックのため遷移先ページURLを設定
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_USER_MODIFY_CONFIRM;

		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_MODIFY_CONFIRM);
	}

	/// <summary>
	///  エラーメッセージをカスタムバリデータにセットしてフォーカス
	/// </summary>
	private void GetErrMsgAndFocusToCV(Dictionary<string, string> errorMessages)
	{
		// パスワード確認用必須チェックエラーメッセージを変更後パスワードのエラーメッセージに統合する
		// （変更後パスワードのエラーが、パスワード確認用テキストボックスの下に表示されるため）
		if (errorMessages.ContainsKey(Constants.FIELD_USER_PASSWORD + Constants.FIELD_COMMON_CONF + "_ncsry"))
		{
			if (errorMessages.ContainsKey(Constants.FIELD_USER_PASSWORD + Constants.FIELD_COMMON_CONF))
			{
				errorMessages[Constants.FIELD_USER_PASSWORD + Constants.FIELD_COMMON_CONF] += "\r\n" + errorMessages[Constants.FIELD_USER_PASSWORD + Constants.FIELD_COMMON_CONF + "_ncsry"];
			}
			else
			{
				errorMessages.Add(Constants.FIELD_USER_PASSWORD + Constants.FIELD_COMMON_CONF, errorMessages[Constants.FIELD_USER_PASSWORD + Constants.FIELD_COMMON_CONF + "_ncsry"]);
			}
		}

		if (errorMessages.Count != 0)
		{
			// カスタムバリデータ取得
			List<CustomValidator> lCustomValidators = new List<CustomValidator>();
			CreateCustomValidators(this, lCustomValidators);

			// エラーをカスタムバリデータへ
			SetControlViewsForError("UserModify", errorMessages, lCustomValidators);
			SetControlViewsForError("UserModifyGlobal", errorMessages, lCustomValidators);

			// ログインIDチェック（メールアドレスをログインIDにするかどうかでメッセージ出力する場所が異なるのでここで指定）
			// ChangeControlLooksForValidatorを読んで WcvUserLoginId.IsValid の値を書き換えるため呼び出し毎にIF文記述
			if (this.WcvUserLoginId.IsValid)
			{
				ChangeControlLooksForValidator(
					errorMessages,
					Constants.FIELD_USER_LOGIN_ID,
					this.WcvUserLoginId,
					this.WtbUserLoginId);
			}
			if (this.WcvUserLoginId.IsValid)
			{
				ChangeControlLooksForValidator(
					errorMessages,
					Constants.FIELD_USER_LOGIN_ID + "_input_check",
					this.WcvUserLoginId,
					this.WtbUserLoginId);
			}
			if ((this.WcvUserLoginId.IsValid) && (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED))
			{
				ChangeControlLooksForValidator(
					errorMessages,
					Constants.FIELD_USER_LOGIN_ID + "_mail_check",
					this.WcvUserLoginId,
					this.WtbUserLoginId);
			}

			ChangeControlLooksForValidator(
				errorMessages,
				Constants.FIELD_USER_MAIL_ADDR + "_for_check",
				this.WcvUserMailAddrForCheck,
				this.WtbUserMailAddr);

			ChangeControlLooksForValidator(
				errorMessages,
				Constants.FIELD_USER_MAIL_ADDR2 + "_for_check",
				this.WcvUserMailAddr2ForCheck,
				this.WtbUserMailAddr2);

			// エラーフォーカス
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
						//cvCustomValidator.ErrorMessage = "";
						//wcTarget.CssClass = wcTarget.CssClass.Replace(Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING, "");
					}
				}
			}
		}
	}

	/// <summary>
	///  入力値を取得してバリューオブジェクトに格納
	/// </summary>
	/// <returns>入力値が格納されたバリューオブジェクト</returns>
	private UserInput CreateInputData()
	{
		UserInput input = new UserInput(new UserModel());
		input.UserId = this.LoginUserId;
		input.UserKbn = this.UserKbn;
		input.Name1 = DataInputUtility.ConvertToFullWidthBySetting(this.WtbUserName1.Text, this.IsUserAddrJp);
		input.Name2 = DataInputUtility.ConvertToFullWidthBySetting(this.WtbUserName2.Text, this.IsUserAddrJp);
		input.Name = input.Name1 + input.Name2;
		input.NameKana1 = StringUtility.ToZenkaku(this.WtbUserNameKana1.Text);
		input.NameKana2 = StringUtility.ToZenkaku(this.WtbUserNameKana2.Text);
		input.NameKana = input.NameKana1 + input.NameKana2;
		input.NickName = this.WtbUserNickName.Text;
		input.BirthYear = this.WddlUserBirthYear.SelectedValue;
		input.BirthMonth = this.WddlUserBirthMonth.SelectedValue;
		input.BirthDay = this.WddlUserBirthDay.SelectedValue;
		// どれか未入力の時は日付整合性チェックは行わない
		if ((this.WddlUserBirthYear.SelectedValue + this.WddlUserBirthMonth.SelectedValue + this.WddlUserBirthDay.SelectedValue).Length != 0)
		{
			input.Birth = this.WddlUserBirthYear.SelectedValue + "/" + this.WddlUserBirthMonth.SelectedValue + "/" + this.WddlUserBirthDay.SelectedValue;
		}
		else
		{
			input.Birth = null;
		}
		input.Sex = this.WrblUserSex.SelectedValue;
		input.MailAddr = StringUtility.ToHankaku(this.WtbUserMailAddr.Text);
		input.MailAddrConf = StringUtility.ToHankaku(this.WtbUserMailAddrConf.Text);
		input.MailAddr2 = StringUtility.ToHankaku(this.WtbUserMailAddr2.Text);
		input.MailAddr2Conf = StringUtility.ToHankaku(this.WtbUserMailAddr2Conf.Text);

		// Set value for zip code
		var inputZipCode = (this.WtbUserZip1.HasInnerControl)
			? StringUtility.ToHankaku(this.WtbUserZip1.Text.Trim())
			: StringUtility.ToHankaku(this.WtbUserZip.Text.Trim());
		if (this.WtbUserZip1.HasInnerControl) inputZipCode += ("-" + StringUtility.ToHankaku(this.WtbUserZip2.Text.Trim()));
		var zipCode = new ZipCode(inputZipCode);
		input.Zip1 = zipCode.Zip1;
		input.Zip2 = zipCode.Zip2;
		input.Zip = (string.IsNullOrEmpty(zipCode.Zip) == false)
			? zipCode.Zip
			: inputZipCode;

		input.Addr1 = this.WddlUserAddr1.SelectedValue;
		input.Addr2 = DataInputUtility.ConvertToFullWidthBySetting(this.WtbUserAddr2.Text, this.IsUserAddrJp).Trim();
		input.Addr3 = DataInputUtility.ConvertToFullWidthBySetting(this.WtbUserAddr3.Text, this.IsUserAddrJp).Trim();
		input.Addr4 = DataInputUtility.ConvertToFullWidthBySetting(this.WtbUserAddr4.Text, this.IsUserAddrJp).Trim();
		input.CompanyName = this.WtbUserCompanyName.Text;
		input.CompanyPostName = this.WtbUserCompanyPostName.Text;

		// Set value for telephone 1
		var inputTel1 = (this.WtbUserTel1.HasInnerControl)
			? StringUtility.ToHankaku(WtbUserTel1.Text.Trim())
			: StringUtility.ToHankaku(WtbUserTel1_1.Text.Trim());
		if (this.WtbUserTel1.HasInnerControl)
		{
			inputTel1 = UserService.CreatePhoneNo(
				inputTel1,
				StringUtility.ToHankaku(WtbUserTel2.Text.Trim()),
				StringUtility.ToHankaku(WtbUserTel3.Text.Trim()));
		}
		var tel1 = new Tel(inputTel1);
		input.Tel1_1 = tel1.Tel1;
		input.Tel1_2 = tel1.Tel2;
		input.Tel1_3 = tel1.Tel3;
		input.Tel1 = (string.IsNullOrEmpty(tel1.TelNo) == false)
			? tel1.TelNo
			: inputTel1;

		// Set value for telephone 2
		var inputTel2 = (this.WtbUserTel2_1.HasInnerControl)
			? StringUtility.ToHankaku(this.WtbUserTel2_1.Text.Trim())
			: StringUtility.ToHankaku(this.WtbUserTel1_2.Text.Trim());
		if (this.WtbUserTel2_1.HasInnerControl)
		{
			inputTel2 = UserService.CreatePhoneNo(
				inputTel2,
				StringUtility.ToHankaku(this.WtbUserTel2_2.Text.Trim()),
				StringUtility.ToHankaku(this.WtbUserTel2_3.Text.Trim()));
		}
		var tel2 = new Tel(inputTel2);
		input.Tel2_1 = tel2.Tel1;
		input.Tel2_2 = tel2.Tel2;
		input.Tel2_3 = tel2.Tel3;
		input.Tel2 = (string.IsNullOrEmpty(tel2.TelNo) == false)
			? tel2.TelNo
			: inputTel2;

		input.MailFlg = this.WcbUserMailFlg.Checked ? Constants.FLG_USER_MAILFLG_OK : Constants.FLG_USER_MAILFLG_NG;

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			input.AddrCountryIsoCode = this.WddlUserCountry.SelectedValue;
			input.Addr5 = IsCountryUs(this.WddlUserCountry.SelectedValue)
				? this.WddlUserAddr5.SelectedText
				: DataInputUtility.ConvertToFullWidthBySetting(this.WtbUserAddr5.Text, this.IsUserAddrJp).Trim();
			input.AddrCountryName = this.WddlUserCountry.SelectedText;

			if (IsCountryTw(this.UserAddrCountryIsoCode)
				&& this.WddlUserAddr2.HasInnerControl
				&& this.WddlUserAddr3.HasInnerControl)
			{
				input.Addr2 = this.WddlUserAddr2.SelectedText;
				input.Addr3 = this.WddlUserAddr3.SelectedText;
			}

			if (this.IsUserAddrJp == false)
			{
				input.Zip = StringUtility.ToHankaku(this.WtbUserZipGlobal.Text);
				input.Zip1 = string.Empty;
				input.Zip2 = string.Empty;
				input.Tel1 = StringUtility.ToHankaku(this.WtbUserTel1Global.Text);
				input.Tel1_1 = string.Empty;
				input.Tel1_2 = string.Empty;
				input.Tel1_3 = string.Empty;
				input.Tel2 = StringUtility.ToHankaku(this.WtbUserTel2Global.Text);
				input.Tel2_1 = string.Empty;
				input.Tel2_2 = string.Empty;
				input.Tel2_3 = string.Empty;
				input.NameKana1 = string.Empty;
				input.NameKana2 = string.Empty;
				input.NameKana = string.Empty;
			}
		}
		input.Addr = input.ConcatenateAddress();

		// メールアドレスチェック
		// PC・モバイルメール片方入力形式でOK？（ログインIDにメルアドは利用しな前提の仕様）
		if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED)
		{
			input.MailAddrForCheck = StringUtility.ToHankaku(this.WtbUserMailAddr.Text.Trim() + this.WtbUserMailAddr2.Text.Trim());
		}
		// 顧客区分に応じたメールアドレス必須チェック
		else
		{
			if (this.IsPcSiteOrOfflineUser)
			{
				input.MailAddrForCheck = StringUtility.ToHankaku(this.WtbUserMailAddr.Text);
			}
			else
			{
				input.MailAddr2ForCheck = StringUtility.ToHankaku(this.WtbUserMailAddr2.Text);
			}
		}

		// ログインIDにメルアド利用する？
		if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED)
		{
			// 顧客区分に応じた必須入力チェック（他入力形式はUserMailAddr* で実施）
			input.LoginIdMailCheck = StringUtility.ToHankaku(this.IsPcSiteOrOfflineUser ? this.WtbUserMailAddr.Text : this.WtbUserMailAddr2.Text);
		}

		// ログインIDチェック （メルアドログインID設定や顧客区分によって適切な値格納済み）
		input.LoginId = StringUtility.ToHankaku(this.WtbUserLoginId.Text);
		// パスワード変更の場合は入力チェック対象とする
		if ((this.WtbUserPasswordBefore.Text != "")
			|| (this.WtbUserPassword.Text != "")
			|| (this.WtbUserPasswordConf.Text != ""))
		{
			input.PasswordBefore = this.IsVisible_UserPassword ? StringUtility.ToHankaku(this.WtbUserPasswordBefore.Text) : null;
			input.Password = StringUtility.ToHankaku(this.WtbUserPassword.Text);
			input.PasswordConf = StringUtility.ToHankaku(this.WtbUserPasswordConf.Text);

			// 必須チェック用
			input.PasswordNecessary = StringUtility.ToHankaku(this.WtbUserPassword.Text);
			input.PasswordConfNecessary = StringUtility.ToHankaku(this.WtbUserPasswordConf.Text);
		}

		input.RemoteAddr = Request.ServerVariables["REMOTE_ADDR"];
		input.LastChanged = Constants.FLG_LASTCHANGED_USER;
		input.EasyRegisterFlg = Constants.FLG_USER_EASY_REGISTER_FLG_NORMAL;

		// ユーザ拡張項目のユーザコントロールがあれば情報取得
		input.UserExtendInput = (this.UserExtendUserControl != null) ? ((UserExtendUserControl)this.UserExtendUserControl).UserExtend : new UserExtendInput();
		input.UserExtendSettingList = (this.UserExtendUserControl != null) ? ((UserExtendUserControl)this.UserExtendUserControl).UserExtendSettingList : new UserExtendSettingList();

		if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED)
		{
			input.HasAuthenticationCode = this.HasAuthenticationCode;
			input.AuthenticationCode = this.IsUserAddrJp
				? this.WtbAuthenticationCode.Text
				: this.WtbAuthenticationCodeGlobal.Text;
		}

		return input;
	}

	/// <summary>
	/// 入力値を取得してUserInputに格納
	/// </summary>
	private UserInput CreateInputDataEasyUser()
	{
		UserInput userInput = CreateInputData();
		var userEasySettingList = new UserService().GetUserEasyRegisterSettingList();

		// かんたん会員登録の項目以外
		foreach (var setting in userEasySettingList.Where(item => item.DisplayFlg == Constants.FLG_USER_EASY_REGISTER_FLG_NORMAL))
		{
			// ブランクをnullに変更
			foreach (var itemName in new UserEasyRegisterHelper().GetValidaterItemList(setting.ItemId))
			{
				if (StringUtility.ToEmpty(userInput.DataSource[itemName]) == string.Empty) userInput.DataSource[itemName] = null;
			}
		}

		return userInput;
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

			if ((this.IsUserAddrJp) == false) this.WtbUserZip2.Text = string.Empty;

			// Display Zip Global
			if (IsCountryTw(this.UserAddrCountryIsoCode) && this.WddlUserAddr3.HasInnerControl)
			{
				BindingDdlUserAddr3();
			}
		}

		if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED && IsPostBack)
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
		RemoveErrorInputClass(this.WtbUserTel1Global);

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

		// Reset authentication code
		if (string.IsNullOrEmpty(this.WhfResetAuthenticationCode.Value) == false)
		{
			// Set authenticated if phone number not change
			this.HasAuthenticationCode = (this.LoginUser.Tel1 == telephone);

			this.WlbAuthenticationStatus.Text
				= this.WlbAuthenticationStatusGlobal.Text
				= string.Empty;

			var wlbAuthenticationStatus = (this.LoginUser.Tel1 != telephone)
				? this.IsUserAddrJp
					? this.WlbAuthenticationStatus
					: this.WlbAuthenticationStatusGlobal
				: null;

			DisplayAuthenticationCode(
				this.IsUserAddrJp ? this.WlbGetAuthenticationCode : this.WlbGetAuthenticationCodeGlobal,
				this.IsUserAddrJp ? this.WtbAuthenticationCode : this.WtbAuthenticationCodeGlobal,
				wlbAuthenticationStatus);

			return;
		}

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
		RemoveErrorInputClass(this.IsUserAddrJp
			? this.WtbAuthenticationCode
			: this.WtbAuthenticationCodeGlobal);
	}

	/// <summary>郵便番号入力チェックエラー文言</summary>
	protected string ZipInputErrorMessage
	{
		get { return StringUtility.ToEmpty(ViewState["ZipInputErrorMessage"]); }
		set { ViewState["ZipInputErrorMessage"] = value; }
	}
	/// <summary>ユーザー区分</summary>
	protected string UserKbn
	{
		get { return (string)ViewState["UserKbn"]; }
		set { ViewState["UserKbn"] = value; }
	}
	/// <summary>パスワード表示フラグ</summary>
	protected bool IsVisible_UserPassword
	{
		get { return (bool)ViewState["UserPassword"]; }
		set { ViewState["UserPassword"] = value; }
	}
	/// <summary>氏名が必須かどうか</summary>
	protected bool IsUserNameNecessary
	{
		get { return (bool)ViewState["UserName"]; }
		set { ViewState["UserName"] = value; }
	}
	/// <summary>氏名(かな)が必須かどうか</summary>
	protected bool IsUserNameKanaNecessary
	{
		get { return (bool)ViewState["UserNameKana"]; }
		set { ViewState["UserNameKana"] = value; }
	}
	/// <summary>生年月日が必須かどうか</summary>
	protected bool IsUserBirthNecessary
	{
		get { return (bool)ViewState["UserBirth"]; }
		set { ViewState["UserBirth"] = value; }
	}
	/// <summary>性別が必須かどうか</summary>
	protected bool IsUserSexNecessary
	{
		get { return (bool)ViewState["UserSex"]; }
		set { ViewState["UserSex"] = value; }
	}
	/// <summary>国が必須かどうか</summary>
	protected bool IsUserCountryNecessary
	{
		get { return (bool)ViewState["UserCountry"]; }
		set { ViewState["UserCountry"] = value; }
	}
	/// <summary>郵便番号が必須かどうか</summary>
	protected bool IsUserZipNecessary
	{
		get { return (bool)ViewState["UserZip"]; }
		set { ViewState["UserZip"] = value; }
	}
	/// <summary>都道府県が必須かどうか</summary>
	protected bool IsUserAddr1Necessary
	{
		get { return (bool)ViewState["UserAddr1"]; }
		set { ViewState["UserAddr1"] = value; }
	}
	/// <summary>市区町村が必須かどうか</summary>
	protected bool IsUserAddr2Necessary
	{
		get { return (bool)ViewState["UserAddr2"]; }
		set { ViewState["UserAddr2"] = value; }
	}
	/// <summary>番地が必須かどうか</summary>
	protected bool IsUserAddr3Necessary
	{
		get { return (bool)ViewState["UserAddr3"]; }
		set { ViewState["UserAddr3"] = value; }
	}
	/// <summary>電話番号が必須かどうか</summary>
	protected bool IsUserTel1Necessary
	{
		get { return (bool)ViewState["UserTel1"]; }
		set { ViewState["UserTel1"] = value; }
	}
	#region 利用していないが下位互換のため残しておく
	/// <summary>ユーザー区分がPCユーザーか判定</summary>
	[Obsolete("使用しないのであれば削除します", false)]
	protected bool IsUserKbnPCUser
	{
		get { return (this.UserKbn == Constants.FLG_USER_USER_KBN_PC_USER); }
	}
	/// <summary>ユーザー区分がモバイルユーザーか判定</summary>
	[Obsolete("使用しないのであれば削除します", false)]
	protected bool IsUserKbnMobileUser
	{
		get { return (this.UserKbn == Constants.FLG_USER_USER_KBN_MOBILE_USER); }
	}
	/// <summary>ユーザー区分がスマートフォンユーザーか判定</summary>
	[Obsolete("使用しないのであれば削除します", false)]
	protected bool IsUserKbnSmartPhoneUser
	{
		get { return (this.UserKbn == Constants.FLG_USER_USER_KBN_SMARTPHONE_USER); }
	}
	#endregion
	/// <summary>PCサイトユーザーか判定（利用メールアドレス判定用）</summary>
	protected bool IsPcSiteOrOfflineUser
	{
		get { return UserService.IsPcSiteOrOfflineUser(this.UserKbn); }
	}
	/// <summary>ユーザ拡張項目のユーザコントロール</summary>
	private Control UserExtendUserControl { get { return GetDefaultMasterContentPlaceHolder().FindControl("ucBodyUserExtendModify"); } }
	/// <summary> 表示するユーザ拡張項目があるか</summary>
	protected bool ExistsUserExtend
	{
		get { return (((UserExtendUserControl)this.UserExtendUserControl).UserExtendSettingList.Items.Count > 0); }
	}
	/// <summary>ユーザーの住所国ISOコード</summary>
	public string UserAddrCountryIsoCode
	{
		get { return this.WddlUserCountry.SelectedValue; }
	}
	/// <summary>ユーザーの住所が日本か</summary>
	public bool IsUserAddrJp
	{
		get { return GlobalAddressUtil.IsCountryJp(this.UserAddrCountryIsoCode); }
	}
	/// <summary>ユーザーの住所がUSか</summary>
	public bool IsUserAddrUs
	{
		get { return GlobalAddressUtil.IsCountryUs(this.UserAddrCountryIsoCode); }
	}
	/// <summary>ユーザーの住所がUSか</summary>
	public bool IsUserAddrZipNecessary
	{
		get { return GlobalAddressUtil.IsAddrZipcodeNecessary(this.UserAddrCountryIsoCode); }
	}
	/// <summary>バリデーショングループ</summary>
	public string ValidationGroup
	{
		get { return this.IsUserAddrJp ? "UserModify" : "UserModifyGlobal"; }
	}
}
