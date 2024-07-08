/*
=========================================================================================================
  Module      : ユーザー情報登録ページ処理(UserRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Collections;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.DefaultSetting;
using w2.App.Common.User;
using w2.App.Common.Util;
using w2.Domain.User;
using w2.Domain.User.Helper;
using w2.App.Common.Global.Config;
using w2.Common.Web;
using w2.Domain.CountryLocation;
using w2.Domain.UserManagementLevel;
using w2.Domain.UserBusinessOwner;
using System.Web.UI;

public partial class Form_User_UserRegister : BasePage
{
	string m_strUserId = null;
	string m_strActionStatus = null;
	private string _uniqueKey;

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// パラメタ取得
			//------------------------------------------------------
			// ユーザID
			m_strUserId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USER_ID]);
			ViewState[Constants.REQUEST_KEY_USER_ID] = m_strUserId;

			// アクションステータス
			m_strActionStatus = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ACTION_STATUS]);
			ViewState[Constants.REQUEST_KEY_ACTION_STATUS] = m_strActionStatus;

			if (this.ActionStatus == Constants.ACTION_STATUS_DEFAULTSETTING)
			{
				CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);
			}

			// ユニークキー
			_uniqueKey = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_UNIQUE_KEY]);
			ViewState[Constants.REQUEST_KEY_UNIQUE_KEY] = _uniqueKey;

			//------------------------------------------------------
			// コンポーネント初期化
			//------------------------------------------------------
			InitializeComponent();

			switch (m_strActionStatus)
			{
				// 新規登録
				case Constants.ACTION_STATUS_INSERT:
				case Constants.ACTION_STATUS_DEFAULTSETTING:
					lSiteName.Text = WebSanitizer.HtmlEncode(CreateSiteNameForDetail(Constants.FLG_USER_MALL_ID_OWN_SITE, ""));

					ucUserBirth.Year = "";
					ucUserBirth.Month = "";
					ucUserBirth.Day = "";

					// Get data default setting for user
					if ((this.UserMaster == null)
						&& this.DefaultSettingInfo.DefaultSettingTables.ContainsKey(Constants.TABLE_USER))
					{
						this.UserMaster = DefaultSettingPage.GetDefaultSettingValue(this.DefaultSettingInfo, Constants.TABLE_USER);

						// Back from error page
						if (this.IsBackFromErrorPage)
						{
							var userDatas = Session[Constants.SESSION_KEY_PARAM_FOR_USER_DEFAULT_SETTING];
							if (userDatas != null)
							{
								this.UserMaster = userDatas;
								Session[Constants.SESSION_KEY_PARAM_FOR_USER_DEFAULT_SETTING] = null;
							}
						}
					}
					// Get data default setting for user
					if ((this.userBusinessOwnerMaster == null)
						&& this.DefaultSettingInfo.DefaultSettingTables.ContainsKey(Constants.TABLE_USER_BUSINESS_OWNER))
					{
						this.userBusinessOwnerMaster = DefaultSettingPage.GetDefaultSettingValue(this.DefaultSettingInfo, Constants.TABLE_USER_BUSINESS_OWNER);
					}
					SetDefaultValue();
					DataBind();
					break;

				// 更新
				case Constants.ACTION_STATUS_UPDATE:
					// ユーザデータ取得
					var user = new UserService().Get(m_strUserId);

					if (Constants.CROSS_POINT_OPTION_ENABLED)
					{
						// Adjust point and member rank by Cross Point api
						UserUtility.AdjustPointAndMemberRankByCrossPointApi(user);
					}

					var userBusinessOwner = new UserBusinessOwnerService().GetByUserId(m_strUserId);
					// 画面セット
					if ((user != null)
						&& (this.IsBackFromConfirm == false))
					{
						SetUserFromUserDatas(user, userBusinessOwner);
					}
					else
					{
						// エラー
					}

					if ((this.UserMaster == null)
						&& this.DefaultSettingInfo.DefaultSettingTables.ContainsKey(Constants.TABLE_USER))
					{
						// セッションでパスワード復号化
						if (this.HavePasswordDisplayPower) user.Password = user.PasswordDecrypted;

						this.UserMaster = user;
					}
					if ((this.userBusinessOwnerMaster == null)
						&& this.DefaultSettingInfo.DefaultSettingTables.ContainsKey(Constants.TABLE_USER_BUSINESS_OWNER))
					{
						this.userBusinessOwnerMaster = userBusinessOwner;
					}
					DataBind();
					break;

			}

			// 確認画面から戻ってきた場合
			if (this.IsBackFromConfirm)
			{
				var userDatas = this.UserInputForBack;
				SetUserFromUserDatas(userDatas.CreateModel(), userDatas.BusinessOwner != null ? userDatas.BusinessOwner.CreateModel() : null);
			}
			this.UserInputForBack = null;
			Session[Constants.SESSION_KEY_PARAM_FOR_USER_BUSINESS_OWNER_INPUT] = null;
		}
		else
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = ""; //エラーメッセージを念のため初期化しておく
			m_strUserId = (string)ViewState[Constants.REQUEST_KEY_USER_ID];
			m_strActionStatus = (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS];
			_uniqueKey = (string)ViewState[Constants.REQUEST_KEY_UNIQUE_KEY];
		}

		DisplayDefaultSettingOfInputAddress();
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponent()
	{
		if (m_strActionStatus == Constants.ACTION_STATUS_INSERT)
		{
			// 新規登録
			trRegister.Visible = true;
			trInputUserId.Visible = false; // ユーザーIDは自動登録
			btnUpdateDefaultSettingBottom.Visible = false;
			btnUpdateDefaultSettingTop.Visible = false;
		}
		else if (m_strActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			// ユーザー情報の更新
			trEdit.Visible = true;
			btnUpdateDefaultSettingBottom.Visible = false;
			btnUpdateDefaultSettingTop.Visible = false;
			tblMemo.Visible = false;
		}
		else if (m_strActionStatus == Constants.ACTION_STATUS_DEFAULTSETTING)
		{
			trRegister.Visible = false;
			trEdit.Visible = false;
			trInputUserId.Visible = false;
			btnConfirmBottom.Visible = false;
			btnConfirmTop.Visible = false;
			trSiteName.Visible = true;
			trInputUserId.Visible = true;
		}

		// ユーザ区分
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_USER, Constants.FIELD_USER_USER_KBN))
		{
			if ((Constants.CS_OPTION_ENABLED == false) && (li.Value == Constants.FLG_USER_USER_KBN_CS)) continue;	// オプションOFF時はCS区分を追加しない
			// モバイルデータの表示と非表示OFF時はMB_USERとMB_GEST区分を追加しない
			if ((Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED == false)
				&& ((li.Value == Constants.FLG_USER_USER_KBN_MOBILE_USER)
					|| (li.Value == Constants.FLG_USER_USER_KBN_MOBILE_GUEST))) continue;
			// パラメータ顧客区分があれば、デフォルト値としてセット
			if (Request[Constants.REQUEST_KEY_USER_KBN] == li.Value)
			{
				li.Selected = true;
			}
			ddlUserKbn.Items.Add(li);
		}


		// 性別
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_USER, Constants.FIELD_USER_SEX))
		{
			rblUserSex.Items.Add(li);
		}

		// 都道府県
		ddlUserAddr1.Items.Add("");
		foreach (string strAddr1 in Constants.STR_PREFECTURES_LIST)
		{
			ddlUserAddr1.Items.Add(strAddr1);
		}

		// メール配信希望
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_FLG))
		{
			rblUserMailFlg.Items.Add(li);
		}
		rblUserMailFlg.SelectedValue = Constants.FLG_USER_MAILFLG_NG;

		// かんたん会員登録
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_USER, Constants.FIELD_USER_EASY_REGISTER_FLG))
		{
			rblUserEasyRegisterFlg.Items.Add(li);
		}
		rblUserEasyRegisterFlg.SelectedValue = Constants.FLG_USER_EASY_REGISTER_FLG_NORMAL;

		// 会員ランク
		DataView dvMemberRank = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatetment = new SqlStatement("MemberRank", "GetMemberRankList"))
		{
			dvMemberRank = sqlStatetment.SelectSingleStatementWithOC(sqlAccessor);
		}
		ddlMemberRank.Items.Add("");
		foreach (DataRowView drv in dvMemberRank)
		{
			ddlMemberRank.Items.Add(new ListItem((string)drv[Constants.FIELD_MEMBERRANK_MEMBER_RANK_NAME], (string)drv[Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID]));
		}

		var models = new UserManagementLevelService().GetAllList();
		ddlUserManagementLevel.Items.AddRange(
			models.Select(m => new ListItem(m.UserManagementLevelName, m.UserManagementLevelId)).ToArray());

		//グローバル対応 項目作成
		ddlAccessCountryIsoCode.Items.Add(new ListItem("", ""));
		ddlDispCurrencyLocaleId.Items.Add(new ListItem("", ""));
		ddlDispLanguageLocaleId.Items.Add(new ListItem("", ""));
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			ddlAccessCountryIsoCode.Items.AddRange(Constants.GLOBAL_CONFIGS.GlobalSettings.CountryIsoCodes.Select(cic => new ListItem(cic, cic)).ToArray());
			ddlDispLanguageLocaleId.Items.AddRange(Constants.GLOBAL_CONFIGS.GlobalSettings.Languages.Select(l => new ListItem(GlobalConfigUtil.LanguageLocaleIdDisplayFormat(l.LocaleId), l.LocaleId)).ToArray());
			ddlDispCurrencyLocaleId.Items.AddRange(Constants.GLOBAL_CONFIGS.GlobalSettings.Currencies.SelectMany(cu => cu.CurrencyLocales.Select(cul => new ListItem(GlobalConfigUtil.CurrencyLocaleIdDisplayFormat(cul.LocaleId), cul.LocaleId))).ToArray());

			var countries = new CountryLocationService().GetCountryNames();
			ddlUserCountry.Items.AddRange(countries.Select(c => new ListItem(c.CountryName, c.CountryIsoCode)).ToArray());
			ddlUserCountry.SelectedValue = Constants.OPERATIONAL_BASE_ISO_CODE;
			ddlUserAddr5.Items.AddRange(Constants.US_STATES_LIST.Select(state => new ListItem(state)).ToArray());
		}
	}

	/// <summary>
	/// ユーザ情報を画面項目に設定
	/// </summary>
	/// <param name="userDatas">ユーザー情報</param>
	/// <param name="userBusinessOwner">ビジネスオーナーのモデル</param>
	private void SetUserFromUserDatas(UserModel userDatas,UserBusinessOwnerModel userBusinessOwner)
	{
		lUserId.Text = WebSanitizer.HtmlEncode(userDatas.UserId);
		foreach (ListItem li in ddlUserKbn.Items)
		{
			li.Selected = (userDatas.UserKbn == li.Value);
		}
		lSiteName.Text = WebSanitizer.HtmlEncode(CreateSiteNameForDetail(userDatas.MallId, userDatas.GetMallName()));
		hfMallId.Value = userDatas.MallId;
		hfMallName.Value = userDatas.GetMallName();
		tbUserName1.Text = userDatas.Name1;
		tbUserName2.Text = userDatas.Name2;
		tbUserNameKana1.Text = userDatas.NameKana1;
		tbUserNameKana2.Text = userDatas.NameKana2;
		tbUserNickName.Text = userDatas.NickName;
		//GMO
		if (userBusinessOwner != null)
		{
			isBusinessOwner.Checked = !string.IsNullOrWhiteSpace(userBusinessOwner.OwnerName1);
			tbOwnerName1.Text = userBusinessOwner.OwnerName1;
			tbOwnerName2.Text = userBusinessOwner.OwnerName2;
			tbOwnerNameKana1.Text = userBusinessOwner.OwnerNameKana1;
			tbOwnerNameKana2.Text = userBusinessOwner.OwnerNameKana2;
			if (userBusinessOwner.Birth != null)
			{
				var birth = DateTime.Parse(userBusinessOwner.Birth.ToString());
				ucOwnerBirth.Year = birth.Year.ToString();
				ucOwnerBirth.Month = birth.Month.ToString();
				ucOwnerBirth.Day = birth.Day.ToString();
			}
			
			tbRequestBudget.Text = userBusinessOwner.RequestBudget.ToString();
			if (this.userBusinessOwnerMaster == null)
			{
				this.userBusinessOwnerMaster = userBusinessOwner;
			}
		}
		else
		{
			isBusinessOwner.Checked = false;
		}
		
		foreach (ListItem li in rblUserSex.Items)
		{
			li.Selected = (userDatas.Sex == li.Value);
		}
		ucUserBirth.Year = userDatas.BirthYear;
		ucUserBirth.Month = userDatas.BirthMonth;
		ucUserBirth.Day = userDatas.BirthDay;
		tbUserMailAddr1.Text = userDatas.MailAddr;
		tbUserMailAddr2.Text = userDatas.MailAddr2;
		tbUserZip1.Text = userDatas.Zip1;
		tbUserZip2.Text = userDatas.Zip2;
		foreach (ListItem li in ddlUserAddr1.Items)
		{
			li.Selected = (userDatas.Addr1 == li.Value);
		}
		tbUserAddr2.Text = userDatas.Addr2;
		tbUserAddr3.Text = userDatas.Addr3;
		tbUserAddr4.Text = userDatas.Addr4;
		tbUserCompanyName.Text = userDatas.CompanyName;
		tbUserCompanyPostName.Text = userDatas.CompanyPostName;
		tbUserTel1_1.Text = userDatas.Tel1_1;
		tbUserTel1_2.Text = userDatas.Tel1_2;
		tbUserTel1_3.Text = userDatas.Tel1_3;
		tbUserTel2_1.Text = userDatas.Tel2_1;
		tbUserTel2_2.Text = userDatas.Tel2_2;
		tbUserTel2_3.Text = userDatas.Tel2_3;
		foreach (ListItem li in rblUserMailFlg.Items)
		{
			li.Selected = (userDatas.MailFlg == li.Value);
		}
		foreach (ListItem li in rblUserEasyRegisterFlg.Items)
		{
			li.Selected = (userDatas.EasyRegisterFlg == li.Value);
		}
		tbUserLoginId.Text = userDatas.LoginId;

		// セッションでパスワード復号化
		if (this.HavePasswordDisplayPower) tbPassword.Text = userDatas.PasswordDecrypted;

		tbUserMemo.Text = userDatas.UserMemo;

		// 会員ランク
		foreach (ListItem li in ddlMemberRank.Items)
		{
			li.Selected = (userDatas.MemberRankId == li.Value);
		}

		// ユーザー管理レベル
		ListItem userManagementLevelItem = ddlUserManagementLevel.Items.FindByValue(userDatas.UserManagementLevelId);
		if (userManagementLevelItem != null) userManagementLevelItem.Selected = true;

		// 最終誕生日ポイント付与年
		tbLastBirthdayPointAddYear.Text = userDatas.LastBirthdayPointAddYear;
		// 最終誕生日クーポン付与年
		tbLastBirthdayCouponPublishYear.Text = userDatas.LastBirthdayCouponPublishYear;

		// リモートIPアドレス格納
		ViewState[Constants.FIELD_USER_REMOTE_ADDR] = userDatas.RemoteAddr;

		// 広告コード格納（ＯＰがオフでも更新のため格納）
		tbUserAdvCode.Text = userDatas.AdvcodeFirst;

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			ddlAccessCountryIsoCode.SelectedValue = userDatas.AccessCountryIsoCode;
			lDispLanguageCode.Text = userDatas.DispLanguageCode;
			ddlDispLanguageLocaleId.SelectedValue = userDatas.DispLanguageLocaleId;
			lDispCurrencyCode.Text = userDatas.DispCurrencyCode;
			ddlDispCurrencyLocaleId.SelectedValue = userDatas.DispCurrencyLocaleId;

			ddlUserCountry.SelectedValue = userDatas.AddrCountryIsoCode;

			if (this.IsUserAddrUs)
			{
				ddlUserAddr5.SelectedValue = userDatas.Addr5;
			}
			else
			{
				tbUserAddr5.Text = userDatas.Addr5;
			}

			if (this.IsUserAddrJp == false)
			{
				tbUserZipGlobal.Text = userDatas.Zip;
				tbUserTel1Global.Text = userDatas.Tel1;
				tbUserTel2Global.Text = userDatas.Tel2;
			}
		}
	}

	/// <summary>
	/// 確認するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 処理ステータスの割り振り
		//------------------------------------------------------
		// パラメタ格納
		UserInput userInput = CreateInputData();
		UserInput userInputValidate = (userInput.EasyRegisterFlg == Constants.FLG_USER_EASY_REGISTER_FLG_EASY) ? CreateInputDataEasyUser() : userInput;
		// 入力チェック＆重複チェック
		var errorMessage = new StringBuilder();
		switch (m_strActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:
				errorMessage.Append(userInputValidate.Validate(
					this.IsUserAddrJp
						? UserInput.EnumUserInputValidationKbn.UserRegist
						: UserInput.EnumUserInputValidationKbn.UserRegistGlobal));
				break;

			case Constants.ACTION_STATUS_UPDATE:
				errorMessage.Append(userInputValidate.Validate(
					this.IsUserAddrJp
						? UserInput.EnumUserInputValidationKbn.UserModify
						: UserInput.EnumUserInputValidationKbn.UserModifyGlobal));
				break;
		}

		if (userInput.BusinessOwner != null)
		{
			errorMessage.Append(userInput.BusinessOwner.Validate(UserBusinessOwnerInput.EnumUserInputValidationKbn.Modify));
		}

		if (errorMessage.Length != 0)
		{
			// エラー発生時のみ画面上部へ
			this.MaintainScrollPositionOnPostBack = false;
			lbErrorMessages.Text = StringUtility.ToEmpty(errorMessage);
			tbdyErrorMessages.Visible = true;
			return;
		}

		if ((m_strActionStatus == Constants.ACTION_STATUS_UPDATE)
			&& Constants.CROSS_POINT_OPTION_ENABLED
			&& (userInput.UserExtendInput.UserExtendDataValue.ContainsKey(Constants.CROSS_POINT_USREX_SHOP_CARD_NO))
			&& (userInput.UserExtendInput.UserExtendDataValue.ContainsKey(Constants.CROSS_POINT_USREX_SHOP_CARD_PIN)))
		{
			// ユーザ情報取得
			var user = new UserService().Get(userInput.UserId);

			var beforeShopCardNo = user.UserExtend.UserExtendDataValue[Constants.CROSS_POINT_USREX_SHOP_CARD_NO];
			var beforeShopCardPin = user.UserExtend.UserExtendDataValue[Constants.CROSS_POINT_USREX_SHOP_CARD_PIN];
			var afterShopCardNo = userInput.UserExtendInput.UserExtendDataValue[Constants.CROSS_POINT_USREX_SHOP_CARD_NO];
			var afterShopCardPin = userInput.UserExtendInput.UserExtendDataValue[Constants.CROSS_POINT_USREX_SHOP_CARD_PIN];
			SessionManager.UpdatedShopCardNoAndPinFlg = true;

			if ((string.IsNullOrEmpty(afterShopCardNo)
					&& (string.IsNullOrEmpty(afterShopCardPin) == false))
				|| ((string.IsNullOrEmpty(afterShopCardNo) == false)
					&& string.IsNullOrEmpty(afterShopCardPin)))
			{
				errorMessage.Append(WebMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_POINT_CARD_REGISTER_ERROR));

				// エラー発生時のみ画面上部へ
				this.MaintainScrollPositionOnPostBack = false;
				lbErrorMessages.Text = StringUtility.ToEmpty(errorMessage);
				tbdyErrorMessages.Visible = true;
				return;
			}

			if (string.IsNullOrEmpty(afterShopCardNo)
				&& string.IsNullOrEmpty(afterShopCardPin))
			{
				SessionManager.MemberIdForCrossPoint = beforeShopCardNo;
				SessionManager.PinCodeForCrossPoint = beforeShopCardPin;
				SessionManager.UpdatedShopCardNoAndPinFlg = false;
			}
		}

		// 画面遷移
		var uniqueKey = CreateUniqueKeyForSaveUserInput(m_strActionStatus, m_strUserId);
		Session[Constants.SESSION_KEY_PARAM_FOR_USER_INPUT + uniqueKey] = userInput;
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_USER_CONFIRM);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_USER_ID).Append("=").Append(m_strUserId);
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(m_strActionStatus);
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_UNIQUE_KEY).Append("=").Append(uniqueKey);
		Response.Redirect(sbUrl.ToString());
	}

	/// <summary>
	/// 戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnHistoryBackTop_OnClick(object sender, EventArgs e)
	{
		Session[Constants.SESSION_KEY_PARAM] = null;
		if ((this.ActionStatus == Constants.ACTION_STATUS_INSERT)
			|| (this.ActionStatus == Constants.ACTION_STATUS_DEFAULTSETTING))
		{
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_USER_LIST);
		}
		else
		{
			var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_USER_CONFIRM)
				.AddParam(Constants.REQUEST_KEY_USER_ID, m_strUserId)
				.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_DETAIL)
				.CreateUrl();
			Response.Redirect(url);
		}
	}

	/// <summary>
	/// Update Default Setting
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateDefaultSetting_Click(object sender, EventArgs e)
	{
		var userInput = CreateInputData();

		var defaultSetting = new DefaultSetting();
		defaultSetting.DefaultSettingTables[Constants.TABLE_USER] = CreateDefaultSettingTable(
			userInput.DataSource,
			Constants.TABLE_USER);

		var fieldDefaultSettingValues = defaultSetting.DefaultSettingTables[Constants.TABLE_USER].GetFieldDefaultSettingValues();
		var errorMessage = Validator.Validate(
			this.IsUserAddrJp
				? "UserRegist"
				: "UserRegistGlobal",
			fieldDefaultSettingValues,
			this.UserAddrCountryIsoCode);
		//table userBusinessOwnerImfo
		defaultSetting.DefaultSettingTables[Constants.TABLE_USER_BUSINESS_OWNER] = CreateDefaultSettingTable(
					userInput.BusinessOwner.DataSource,
					Constants.TABLE_USER_BUSINESS_OWNER);
		var fieldDefaultSettingValuesGmo = defaultSetting.DefaultSettingTables[Constants.TABLE_USER_BUSINESS_OWNER].GetFieldDefaultSettingValues();

		errorMessage += Validator.Validate(
			this.IsUserAddrJp
				? "UserRegist"
				: "UserRegistGlobal",
			fieldDefaultSettingValuesGmo,
			this.UserAddrCountryIsoCode);

		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			Session[Constants.SESSION_KEY_PARAM_FOR_USER_DEFAULT_SETTING] = fieldDefaultSettingValues;
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		defaultSetting.UpdateDefaultSetting(
			Constants.CONST_DEFAULT_SHOP_ID,
			Constants.TABLE_USER,
			this.LoginOperatorName);
		divComp.Visible = true;
	}

	/// <summary>
	/// 入力値を取得してUserInputに格納
	/// </summary>
	private UserInput CreateInputData()
	{
		UserInput userInput = new UserInput(new UserModel());
		userInput.UserId = m_strUserId;
		userInput.UserKbn = ddlUserKbn.SelectedValue;
		userInput.MallId = hfMallId.Value;
		userInput.MallName = hfMallName.Value;
		userInput.Name1 = DataInputUtility.ConvertToFullWidthBySetting(tbUserName1.Text, this.IsUserAddrJp);
		userInput.Name2 = DataInputUtility.ConvertToFullWidthBySetting(tbUserName2.Text, this.IsUserAddrJp);
		userInput.Name = userInput.Name1 + userInput.Name2;
		userInput.NameKana1 = StringUtility.ToZenkaku(this.tbUserNameKana1.Text);
		userInput.NameKana2 = StringUtility.ToZenkaku(this.tbUserNameKana2.Text);
		userInput.NameKana = userInput.NameKana1 + userInput.NameKana2;
		userInput.NickName = tbUserNickName.Text;
		userInput.Sex = rblUserSex.SelectedValue;
		userInput.BirthYear = ucUserBirth.Year;
		userInput.BirthMonth = ucUserBirth.Month;
		userInput.BirthDay = ucUserBirth.Day;

		//GMO
		if (isBusinessOwner.Checked)
		{
			userInput.BusinessOwner.OwnerName1 = DataInputUtility.ConvertToFullWidthBySetting(this.tbOwnerName1.Text); ;
			userInput.BusinessOwner.OwnerName2 = DataInputUtility.ConvertToFullWidthBySetting(this.tbOwnerName2.Text);
			userInput.BusinessOwner.OwnerNameKana1 = StringUtility.ToZenkaku(this.tbOwnerNameKana1.Text);
			userInput.BusinessOwner.OwnerNameKana2 = StringUtility.ToZenkaku(this.tbOwnerNameKana2.Text);

			userInput.BusinessOwner.RequestBudget = this.tbRequestBudget.Text;
			userInput.BusinessOwner.Birth = ucOwnerBirth.DateString;
		}
		else
		{
			userInput.BusinessOwner = null;
		}
		// どれか未入力の時は日付整合性チェックは行わない
		if ((ucUserBirth.Year + ucUserBirth.Month + ucUserBirth.Day).Length != 0)
		{
			userInput.Birth = ucUserBirth.DateString;
		}
		else
		{
			userInput.Birth = null;
		}
		userInput.MailAddr = StringUtility.ToHankaku(tbUserMailAddr1.Text);
		userInput.MailAddr2 = StringUtility.ToHankaku(tbUserMailAddr2.Text);
		userInput.Zip1 = StringUtility.ToHankaku(tbUserZip1.Text);
		userInput.Zip2 = StringUtility.ToHankaku(tbUserZip2.Text);
		if ((tbUserZip1.Text.Length != 0) || (tbUserZip2.Text.Length != 0))
		{
			userInput.Zip = StringUtility.ToHankaku(tbUserZip1.Text) + "-" + StringUtility.ToHankaku(tbUserZip2.Text);
		}
		else
		{
			userInput.Zip = string.Empty;
		}
		userInput.Addr1 = ddlUserAddr1.SelectedValue;
		userInput.Addr2 = DataInputUtility.ConvertToFullWidthBySetting(tbUserAddr2.Text, this.IsUserAddrJp).Trim();
		userInput.Addr3 = DataInputUtility.ConvertToFullWidthBySetting(tbUserAddr3.Text, this.IsUserAddrJp).Trim();
		userInput.Addr4 = DataInputUtility.ConvertToFullWidthBySetting(tbUserAddr4.Text, this.IsUserAddrJp).Trim();
		userInput.CompanyName = tbUserCompanyName.Text;
		userInput.CompanyPostName = tbUserCompanyPostName.Text;
		userInput.Tel1_1 = StringUtility.ToHankaku(tbUserTel1_1.Text);
		userInput.Tel1_2 = StringUtility.ToHankaku(tbUserTel1_2.Text);
		userInput.Tel1_3 = StringUtility.ToHankaku(tbUserTel1_3.Text);
		userInput.Tel1 = UserService.CreatePhoneNo(tbUserTel1_1.Text, tbUserTel1_2.Text, tbUserTel1_3.Text);
		userInput.Tel2_1 = StringUtility.ToHankaku(tbUserTel2_1.Text);
		userInput.Tel2_2 = StringUtility.ToHankaku(tbUserTel2_2.Text);
		userInput.Tel2_3 = StringUtility.ToHankaku(tbUserTel2_3.Text);
		userInput.Tel2 = UserService.CreatePhoneNo(tbUserTel2_1.Text, tbUserTel2_2.Text, tbUserTel2_3.Text);
		userInput.MailFlg = rblUserMailFlg.SelectedValue;
		userInput.EasyRegisterFlg = rblUserEasyRegisterFlg.SelectedValue;
		userInput.LoginId = StringUtility.ToHankaku(tbUserLoginId.Text.Trim());
		//権限により入力欄のパスワードあるいは元のパスワードで更新
		if (m_strActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			var user = new UserService().Get(m_strUserId);
			userInput.Password = (this.HavePasswordDisplayPower) ? StringUtility.ToHankaku(tbPassword.Text) : user.PasswordDecrypted;
			// モバイルデータの表示と非表示OFF時
			if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED == false)
			{
				userInput.MailAddr2 = user.MailAddr2;
			}
			userInput.CareerId = user.CareerId;
			userInput.MobileUid = user.MobileUid;
			userInput.FixedPurchaseMemberFlg = user.FixedPurchaseMemberFlg;
		}
		else
		{
			userInput.Password = StringUtility.ToHankaku(tbPassword.Text);
		}
		userInput.RemoteAddr = StringUtility.ToEmpty(ViewState[Constants.FIELD_USER_REMOTE_ADDR]);
		userInput.UserMemo = StringUtility.RemoveUnavailableControlCode(tbUserMemo.Text);
		userInput.MemberRankId = ddlMemberRank.SelectedValue;
		userInput.UserManagementLevelId = ddlUserManagementLevel.SelectedValue;
		userInput.AdvcodeFirst = tbUserAdvCode.Text.Trim();	// 広告コード格納（ＯＰがオフでも更新のため格納）
		userInput.LastBirthdayPointAddYear = tbLastBirthdayPointAddYear.Text;
		userInput.LastBirthdayCouponPublishYear = tbLastBirthdayCouponPublishYear.Text;

		// ユーザー拡張項目情報のセット
		var wucBodyUserExtendRegist = FindControlRecursive(this, "ucBodyUserExtendRegist") as Form_Common_User_BodyUserExtendRegist;
		if (wucBodyUserExtendRegist != null) userInput.UserExtendInput = wucBodyUserExtendRegist.CreateUserExtendFromInputData();

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			userInput.AccessCountryIsoCode = ddlAccessCountryIsoCode.SelectedValue;
			userInput.DispLanguageCode = lDispLanguageCode.Text;
			userInput.DispLanguageLocaleId = ddlDispLanguageLocaleId.SelectedValue;
			userInput.DispCurrencyCode = lDispCurrencyCode.Text;
			userInput.DispCurrencyLocaleId = ddlDispCurrencyLocaleId.SelectedValue;

			userInput.AddrCountryIsoCode = ddlUserCountry.SelectedValue;
			userInput.AddrCountryName = ddlUserCountry.SelectedItem.Text;
			userInput.Addr5 = this.IsUserAddrUs
				? ddlUserAddr5.SelectedItem.Text
				: DataInputUtility.ConvertToFullWidthBySetting(tbUserAddr5.Text.Trim(), this.IsUserAddrJp);

			if (this.IsUserAddrJp == false)
			{
				userInput.Tel1 = StringUtility.ToHankaku(tbUserTel1Global.Text);
				userInput.Tel1_1 = string.Empty;
				userInput.Tel1_2 = string.Empty;
				userInput.Tel1_3 = string.Empty;
				userInput.Tel2 = StringUtility.ToHankaku(tbUserTel2Global.Text);
				userInput.Tel2_1 = string.Empty;
				userInput.Tel2_2 = string.Empty;
				userInput.Tel2_3 = string.Empty;
				userInput.Zip = StringUtility.ToHankaku(tbUserZipGlobal.Text);
				userInput.Zip1 = string.Empty;
				userInput.Zip2 = string.Empty;
				userInput.NameKana1 = string.Empty;
				userInput.NameKana2 = string.Empty;
				userInput.NameKana = string.Empty;
			}
		}
		userInput.Addr = userInput.ConcatenateAddress();

		return userInput;
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

	/// <summary>
	/// 郵便番号検索
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnOwnerZipSearch_Click(object sender, EventArgs e)
	{
		var zipcodeUtil = new ZipcodeSearchUtility(tbUserZip1.Text + tbUserZip2.Text);
		if (zipcodeUtil.Success)
		{
			foreach (ListItem li in ddlUserAddr1.Items)
			{
				li.Selected = (li.Value == zipcodeUtil.PrefectureName);
			}
			tbUserAddr2.Text = zipcodeUtil.CityName + zipcodeUtil.TownName;
		}
	}

	/// <summary>
	/// 通貨ロケールIDドロップダウンリスト変更時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlDispCurrencyLocaleId_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(ddlDispCurrencyLocaleId.SelectedValue))
		{
			lDispCurrencyCode.Text = string.Empty;
			return;
		}
		lDispCurrencyCode.Text = GlobalConfigUtil.GetCurrencyByLocaleId(ddlDispCurrencyLocaleId.SelectedValue).Code;
	}

	/// <summary>
	/// 言語ロケールID ドロップダウンリスト変更時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlDispLanguageLocaleId_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(ddlDispLanguageLocaleId.SelectedValue))
		{
			lDispLanguageCode.Text = string.Empty;
			return;
		}
		lDispLanguageCode.Text = GlobalConfigUtil.GetLanguageByLocaleId(ddlDispLanguageLocaleId.SelectedValue).Code;
	}

	/// <summary>
	/// Create default setting table
	/// </summary>
	/// <param name="parameters">Parameters</param>
	/// <param name="tableName">Table name</param>
	/// <returns>Default setting table</returns>
	private DefaultSettingTable CreateDefaultSettingTable(Hashtable parameters, string tableName)
	{
		var defaultSettingTable = new DefaultSettingTable(tableName);
		if (tableName == Constants.TABLE_USER_BUSINESS_OWNER)
		{
			//GMO
			// Add user president Name Family
			defaultSettingTable.Add(
				Constants.FIELD_USER_BUSINESS_OWNER_NAME1,
				(cbOwnerName1HasDefault.Checked)
					? parameters[Constants.FIELD_USER_BUSINESS_OWNER_NAME1]
					: null,
				tbOwnerName1Default.Text,
				cbOwnerName1Default.Checked);

			// Add user president Name
			defaultSettingTable.Add(
				Constants.FIELD_USER_BUSINESS_OWNER_NAME2,
				(cbOwnerName2HasDefault.Checked)
					? parameters[Constants.FIELD_USER_BUSINESS_OWNER_NAME2]
					: null,
				tbOwnerName2Default.Text,
				cbOwnerName2Default.Checked);

			// Add user president Name Family Kana
			defaultSettingTable.Add(
				Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA1,
				(cbOwnerNameKana1HasDefault.Checked)
					? parameters[Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA1]
					: null,
				tbOwnerNameKana1Default.Text,
				cbOwnerNameKana1Default.Checked);

			// Add user president Name kana
			defaultSettingTable.Add(
				Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA2,
				(cbOwnerNameKana2HasDefault.Checked)
					? parameters[Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA2]
					: null,
				tbOwnerNameKana2Default.Text,
				cbOwnerNameKana2Default.Checked);
			// Add user gmo birth
			defaultSettingTable.Add(
				Constants.FIELD_USER_BUSINESS_OWNER_BIRTH,
				(cbOwnerBirthHasDefault.Checked)
					? parameters[Constants.FIELD_USER_BUSINESS_OWNER_BIRTH]
					: null,
				tbOwnerBirthDefault.Text,
				cbOwnerBirthDefault.Checked);

			// Add user req upper Limit
			defaultSettingTable.Add(
				Constants.FIELD_USER_BUSINESS_OWNER_REQUEST_BUDGET,
				(cbRequestBudgetHasDefault.Checked)
					? parameters[Constants.FIELD_USER_BUSINESS_OWNER_REQUEST_BUDGET]
					: null,
				tbRequestBudgetDefault.Text,
				cbRequestBudgetDefault.Checked);
			return defaultSettingTable;
		}
		// Add user id
		defaultSettingTable.Add(
			Constants.FIELD_USER_USER_ID,
			null,
			tbUserIdDefault.Text,
			cbUserDefault.Checked);
		// Add mall id
		defaultSettingTable.Add(
			Constants.FIELD_USER_MALL_ID,
			(cbSiteHasDefault.Checked)
				? parameters[Constants.FIELD_USER_MALL_ID]
				: null,
			tbSiteDefault.Text,
			cbSiteDefault.Checked);
		// Add user kbn
		defaultSettingTable.Add(
			Constants.FIELD_USER_USER_KBN,
			(cbUserKbnHasDefault.Checked)
				? parameters[Constants.FIELD_USER_USER_KBN]
				: null,
			tbUserKbnDefault.Text,
			cbUserKbnDefault.Checked);
		// Add user name 1
		defaultSettingTable.Add(
			Constants.FIELD_USER_NAME1,
			(cbUserNameHasDefault.Checked)
				? parameters[Constants.FIELD_USER_NAME1]
				: null,
			tbUserNameDefault.Text,
			cbUserNameDefault.Checked);
		// Add user name 2
		defaultSettingTable.Add(
			Constants.FIELD_USER_NAME2,
			(cbUserNameHasDefault.Checked)
				? parameters[Constants.FIELD_USER_NAME2]
				: null,
			string.Empty,
			cbUserNameDefault.Checked);
		// Add user name kana 1
		defaultSettingTable.Add(
			Constants.FIELD_USER_NAME_KANA1,
			(cbUserNameKanaHasDefault.Checked)
				? parameters[Constants.FIELD_USER_NAME_KANA1]
				: null,
			tbUserNameKanaDefault.Text,
			cbUserNameKanaDefault.Checked);
		// Add user name kana 2
		defaultSettingTable.Add(
			Constants.FIELD_USER_NAME_KANA2,
			(cbUserNameKanaHasDefault.Checked)
				? parameters[Constants.FIELD_USER_NAME_KANA2]
				: null,
			string.Empty,
			cbUserNameKanaDefault.Checked);
		// Add user nick name
		defaultSettingTable.Add(
			Constants.FIELD_USER_NICK_NAME,
			(cbUserNickNameHasDefault.Checked)
				? parameters[Constants.FIELD_USER_NICK_NAME]
				: null,
			tbUserNickNameDefault.Text,
			cbUserNickNameDefault.Checked);
		// Add user user sex
		defaultSettingTable.Add(
			Constants.FIELD_USER_SEX,
			(cbUserSexHasDefault.Checked)
				? parameters[Constants.FIELD_USER_SEX]
				: null,
			tbUserSexDefault.Text,
			cbUserSexDefault.Checked);
		// Add user birth
		defaultSettingTable.Add(
			Constants.FIELD_USER_BIRTH,
			(cbBirthHasDefault.Checked)
				? parameters[Constants.FIELD_USER_BIRTH]
				: null,
			tbBirthDefault.Text,
			cbBirthDefault.Checked);
		// Add user mail addr
		defaultSettingTable.Add(
			Constants.FIELD_USER_MAIL_ADDR,
			(cbUserMailAddr1HasDefault.Checked)
				? parameters[Constants.FIELD_USER_MAIL_ADDR]
				: null,
			tbUserMailAddr1Default.Text,
			cbUserMailAddr1Default.Checked);
		// Add user mail addr 2
		defaultSettingTable.Add(
			Constants.FIELD_USER_MAIL_ADDR2,
			(cbUserMailAddr2HasDefault.Checked)
				? parameters[Constants.FIELD_USER_MAIL_ADDR2]
				: null,
			tbUserMailAddr2Default.Text,
			cbUserMailAddr2Default.Checked);
		// Add user mail addr country ISO code
		defaultSettingTable.Add(
			Constants.FIELD_USER_ADDR_COUNTRY_ISO_CODE,
			(cbUserCountryHasDefault.Checked)
				? parameters[Constants.FIELD_USER_ADDR_COUNTRY_ISO_CODE]
				: null,
			tbUserCountryDefault.Text,
			cbUserCountryDefault.Checked);
		// Add user zip 1
		defaultSettingTable.Add(
			Constants.FIELD_USER_ZIP1,
			(cbUserZipHasDefault.Checked)
				? parameters[Constants.FIELD_USER_ZIP1]
				: null,
			tbUserZipDefault.Text,
			cbUserZipDefault.Checked);
		// Add user zip 2
		defaultSettingTable.Add(
			Constants.FIELD_USER_ZIP2,
			(cbUserZipHasDefault.Checked)
				? parameters[Constants.FIELD_USER_ZIP2]
				: null,
			string.Empty,
			cbUserZipDefault.Checked);
		// Add user zip
		defaultSettingTable.Add(
			Constants.FIELD_USER_ZIP,
			(cbUserZipGlobalHasDefault.Checked)
				? parameters[Constants.FIELD_USER_ZIP]
				: null,
			tbUserZipGlobalDefault.Text,
			cbUserZipGlobalDefault.Checked);
		// Add user addr 1
		defaultSettingTable.Add(
			Constants.FIELD_USER_ADDR1,
			(cbUserAddr1HasDefault.Checked)
				? parameters[Constants.FIELD_USER_ADDR1]
				: null,
			tbUserAddr1Default.Text,
			cbUserAddr1Default.Checked);
		// Add user addr 2
		defaultSettingTable.Add(
			Constants.FIELD_USER_ADDR2,
			(cbUserAddr2HasDefault.Checked)
				? parameters[Constants.FIELD_USER_ADDR2]
				: null,
			tbUserAddr2Default.Text,
			cbUserAddr2Default.Checked);
		// Add user addr 3
		defaultSettingTable.Add(
			Constants.FIELD_USER_ADDR3,
			(cbUserAddr3HasDefault.Checked)
				? parameters[Constants.FIELD_USER_ADDR3]
				: null,
			tbUserAddr3Default.Text,
			cbUserAddr3Default.Checked);
		// Add user addr 4
		defaultSettingTable.Add(
			Constants.FIELD_USER_ADDR4,
			(cbUserAddr4HasDefault.Checked)
				? parameters[Constants.FIELD_USER_ADDR4]
				: null,
			tbUserAddr4Default.Text,
			cbUserAddr4Default.Checked);
		// Add user addr 5
		defaultSettingTable.Add(
			Constants.FIELD_USER_ADDR5,
			(cbUserAddr5HasDefault.Checked)
				? parameters[Constants.FIELD_USER_ADDR5]
				: null,
			tbUserAddr5Default.Text,
			cbUserAddr5Default.Checked);
		// Add user tel1
		defaultSettingTable.Add(
			Constants.FIELD_USER_TEL1,
			(cbUserTel1GlobalHasDefault.Checked)
				? parameters[Constants.FIELD_USER_TEL1]
				: null,
			tbUserTel1GlobalDefault.Text,
			cbUserTel1GlobalDefault.Checked);
		// Add user tel1 1
		defaultSettingTable.Add(
			Constants.FIELD_USER_TEL1_1,
			(cbUserTelHasDefault.Checked)
				? parameters[Constants.FIELD_USER_TEL1_1]
				: null,
			tbUserTelDefault.Text,
			cbUserTelDefault.Checked);
		// Add user tel1 2
		defaultSettingTable.Add(
			Constants.FIELD_USER_TEL1_2,
			(cbUserTelHasDefault.Checked)
				? parameters[Constants.FIELD_USER_TEL1_2]
				: null,
			string.Empty,
			cbUserTelDefault.Checked);
		// Add user tel1 3
		defaultSettingTable.Add(
			Constants.FIELD_USER_TEL1_3,
			(cbUserTelHasDefault.Checked)
				? parameters[Constants.FIELD_USER_TEL1_3]
				: null,
			string.Empty,
			cbUserTelDefault.Checked);
		// Add user tel2
		defaultSettingTable.Add(
			Constants.FIELD_USER_TEL2,
			(cbUserTel2GlobalHasDefault.Checked)
				? parameters[Constants.FIELD_USER_TEL2]
				: null,
			tbUserTel2GlobalDefault.Text,
			cbUserTel2GlobalDefault.Checked);
		// Add user tel2 1
		defaultSettingTable.Add(
			Constants.FIELD_USER_TEL2_1,
			(cbUserTel2HasDefault.Checked)
				? parameters[Constants.FIELD_USER_TEL2_1]
				: null,
			tbUserTel2Default.Text,
			cbUserTel2Default.Checked);
		// Add user tel2 2
		defaultSettingTable.Add(
			Constants.FIELD_USER_TEL2_2,
			(cbUserTel2HasDefault.Checked)
				? parameters[Constants.FIELD_USER_TEL2_2]
				: null,
			string.Empty,
			cbUserTel2Default.Checked);
		// Add user tel2 3
		defaultSettingTable.Add(
			Constants.FIELD_USER_TEL2_3,
			(cbUserTel2HasDefault.Checked)
				? parameters[Constants.FIELD_USER_TEL2_3]
				: null,
			string.Empty,
			cbUserTel2Default.Checked);
		// Add user mail flag
		defaultSettingTable.Add(
			Constants.FIELD_USER_MAIL_FLG,
			(cbUserMailFlgHasDefault.Checked)
				? parameters[Constants.FIELD_USER_MAIL_FLG]
				: null,
			tbUserMailFlgDefault.Text,
			cbUserMailFlgDefault.Checked);
		// Add user easy register flag
		defaultSettingTable.Add(
			Constants.FIELD_USER_EASY_REGISTER_FLG,
			(cbUserEasyRegisterFlgHasDefault.Checked)
				? parameters[Constants.FIELD_USER_EASY_REGISTER_FLG]
				: null,
			tbUserEasyRegisterFlgDefault.Text,
			cbUserEasyRegisterFlgDefault.Checked);
		// Add user login id
		defaultSettingTable.Add(
			Constants.FIELD_USER_LOGIN_ID,
			(cbUserLoginIdHasDefault.Checked)
				? parameters[Constants.FIELD_USER_LOGIN_ID]
				: null,
			tbUserLoginIdDefault.Text,
			cbUserLoginIdDefault.Checked);
		// Add user password
		defaultSettingTable.Add(
			Constants.FIELD_USER_PASSWORD,
			(cbUserPasswordHasDefault.Checked)
				? parameters[Constants.FIELD_USER_PASSWORD]
				: null,
			tbUserPasswordDefault.Text,
			cbUserPasswordDefault.Checked);
		// Add user advcode first
		defaultSettingTable.Add(
			Constants.FIELD_USER_ADVCODE_FIRST,
			(cbUserAdvCodeHasDefault.Checked)
				? parameters[Constants.FIELD_USER_ADVCODE_FIRST]
				: null,
			tbUserAdvCodeDefault.Text,
			cbUserAdvCodeDefault.Checked);
		// Add user memo
		defaultSettingTable.Add(
			Constants.FIELD_USER_USER_MEMO,
			(cbUserMemoHasDefault.Checked)
				? parameters[Constants.FIELD_USER_USER_MEMO]
				: null,
			tbUserMemoDefault.Text,
			cbUserMemoDefault.Checked);
		// Add user member rank id
		defaultSettingTable.Add(
			Constants.FIELD_USER_MEMBER_RANK_ID,
			(cbMemberRankHasDefault.Checked)
				? parameters[Constants.FIELD_USER_MEMBER_RANK_ID]
				: null,
			tbMemberRankDefault.Text,
			cbMemberRankDefault.Checked);
		// Add user management level id
		defaultSettingTable.Add(
			Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID,
			(cbUserManagementLevelHasDefault.Checked)
				? parameters[Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID]
				: null,
			tbUserManagementLevelDefault.Text,
			cbUserManagementLevelDefault.Checked);
		// Add user access country ISO code
		defaultSettingTable.Add(
			Constants.FIELD_USER_ACCESS_COUNTRY_ISO_CODE,
			(cbAccessCountryIsoCodeHasDefault.Checked)
				? parameters[Constants.FIELD_USER_ACCESS_COUNTRY_ISO_CODE]
				: null,
			tbAccessCountryIsoCodeDefault.Text,
			cbAccessCountryIsoCodeDefault.Checked);
		// Add user disp language locale id
		defaultSettingTable.Add(
			Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID,
			(cbDispLanguageLocaleIdHasDefault.Checked)
				? parameters[Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID]
				: null,
			tbDispLanguageLocaleIdDefault.Text,
			cbDispLanguageLocaleIdDefault.Checked);
		// Add user disp currency locale id
		defaultSettingTable.Add(
			Constants.FIELD_USER_DISP_CURRENCY_LOCALE_ID,
			(cbDispCurrencyLocaleIdHasDefault.Checked)
				? parameters[Constants.FIELD_USER_DISP_CURRENCY_LOCALE_ID]
				: null,
			tbDispCurrencyLocaleIdDefault.Text,
			cbDispCurrencyLocaleIdDefault.Checked);
		// Add user last birthday point add year
		defaultSettingTable.Add(
			Constants.FIELD_USER_LAST_BIRTHDAY_POINT_ADD_YEAR,
			(cbLastBirthdayPointAddYearHasDefault.Checked)
				? parameters[Constants.FIELD_USER_LAST_BIRTHDAY_POINT_ADD_YEAR]
				: null,
			tbLastBirthdayPointAddYearDefault.Text,
			cbLastBirthdayPointAddYearDefault.Checked);
		// Add user last birthday coupon publish year
		defaultSettingTable.Add(
			Constants.FIELD_USER_LAST_BIRTHDAY_COUPON_PUBLISH_YEAR,
			(cbLastBirthdayCouponPublishYearHasDefault.Checked)
				? parameters[Constants.FIELD_USER_LAST_BIRTHDAY_COUPON_PUBLISH_YEAR]
				: null,
			tbLastBirthdayCouponPublishYearDefault.Text,
			cbLastBirthdayCouponPublishYearDefault.Checked);
		// Add user company name
		defaultSettingTable.Add(
			Constants.FIELD_USER_COMPANY_NAME,
			(cbUserCompanyNameHasDefault.Checked)
				? parameters[Constants.FIELD_USER_COMPANY_NAME]
				: null,
			tbUserCompanyNameDefault.Text,
			cbUserCompanyNameDefault.Checked);
		// Add user company post name
		defaultSettingTable.Add(
			Constants.FIELD_USER_COMPANY_POST_NAME,
			(cbUserCompanyPostNameHasDefalut.Checked)
				? parameters[Constants.FIELD_USER_COMPANY_POST_NAME]
				: null,
			tbUserCompanyPostNameDefalutSetting.Text,
			cbUserCompanyPostNameDefault.Checked);

		return defaultSettingTable;
	}

	/// <summary>
	/// Set default value
	/// </summary>
	private void SetDefaultValue()
	{
		if (this.IsBackFromConfirm) return;

		// Dropdownlist user kbn
		var userKbnValueDefault = StringUtility.ToEmpty(
			GetKeyValue(this.UserMaster, Constants.FIELD_USER_USER_KBN));
		if (ddlUserKbn.Items.FindByValue(userKbnValueDefault) != null)
		{
			ddlUserKbn.SelectedValue = userKbnValueDefault;
		}

		// Dropdownlist member rank
		var memberRankValueDefault = StringUtility.ToEmpty(
			GetKeyValue(this.UserMaster, Constants.FIELD_USER_MEMBER_RANK_ID));
		if (ddlMemberRank.Items.FindByValue(memberRankValueDefault) != null)
		{
			ddlMemberRank.SelectedValue = memberRankValueDefault;
		}

		// Dropdownlist user country
		var userCountryValueDefault = StringUtility.ToEmpty(
			GetKeyValue(this.UserMaster, Constants.FIELD_USER_ADDR_COUNTRY_ISO_CODE));
		if (ddlUserCountry.Items.FindByValue(userCountryValueDefault) != null)
		{
			ddlUserCountry.SelectedValue = userCountryValueDefault;
		}

		// Dropdownlist user management level
		var userManagementLevelValueDefault = StringUtility.ToEmpty(
			GetKeyValue(this.UserMaster, Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID));
		if (ddlUserManagementLevel.Items.FindByValue(userManagementLevelValueDefault) != null)
		{
			ddlUserManagementLevel.SelectedValue = userManagementLevelValueDefault;
		}

		// Dropdownlist access country ISO code
		var accessCountryIsoCodeValueDefault = StringUtility.ToEmpty(
			GetKeyValue(this.UserMaster, Constants.FIELD_USER_ACCESS_COUNTRY_ISO_CODE));
		if (ddlAccessCountryIsoCode.Items.FindByValue(accessCountryIsoCodeValueDefault) != null)
		{
			ddlAccessCountryIsoCode.SelectedValue = accessCountryIsoCodeValueDefault;
		}

		// Dropdownlist disp language locale id
		var dispLanguageLocaleIdValueDefault = StringUtility.ToEmpty(
			GetKeyValue(this.UserMaster, Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID));
		if (ddlDispLanguageLocaleId.Items.FindByValue(dispLanguageLocaleIdValueDefault) != null)
		{
			ddlDispLanguageLocaleId.SelectedValue = dispLanguageLocaleIdValueDefault;
		}

		// Dropdownlist disp currency locale id
		var dispCurrencyLocaleIdValueDefault = StringUtility.ToEmpty(
			GetKeyValue(this.UserMaster, Constants.FIELD_USER_DISP_CURRENCY_LOCALE_ID));
		if (ddlDispCurrencyLocaleId.Items.FindByValue(dispCurrencyLocaleIdValueDefault) != null)
		{
			ddlDispCurrencyLocaleId.SelectedValue = dispCurrencyLocaleIdValueDefault;
		}

		// Dropdownlist address 1
		var address1Default = StringUtility.ToEmpty(
			GetKeyValue(this.UserMaster, Constants.FIELD_USER_ADDR1));
		if (ddlUserAddr1.Items.FindByValue(address1Default) != null)
		{
			ddlUserAddr1.SelectedValue = address1Default;
		}

		// Dropdownlist address 5
		var address5Default = StringUtility.ToEmpty(
			GetKeyValue(this.UserMaster, Constants.FIELD_USER_ADDR5));
		if (ddlUserAddr5.Items.FindByValue(address5Default) != null)
		{
			ddlUserAddr5.SelectedValue = address5Default;
		}
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
			Session[Constants.SESSION_KEY_PARAM_FOR_USER_BUSINESS_OWNER_INPUT] = userBusinessOwnerInput;
		}
	}
	
	/// <summary>
	/// Is default setting has default
	/// </summary>
	/// <param name="tableName">Table name</param>
	/// <param name="fieldName">Field name</param>
	/// <returns>Default setting has default</returns>
	protected bool IsDefaultSettingHasDefault(string tableName, string fieldName)
	{
		var hasDefault = (this.IsDefaultSettingPage
			&& this.DefaultSettingInfo.DefaultSettingTables.ContainsKey(tableName)
			&& (this.DefaultSettingInfo.DefaultSettingTables[tableName].GetDefaultSettingValue(fieldName) != null));
		return hasDefault;
	}
	/// <summary>
	/// Get default setting comment
	/// </summary>
	/// <param name="tableName">Table name</param>
	/// <param name="fieldName">Field name</param>
	/// <returns>Comment</returns>
	protected string GetDefaultSettingComment(string tableName, string fieldName)
	{
		string comment = DefaultSettingPage.GetDefaultSettingComment(
			this.DefaultSettingInfo,
			tableName,
			fieldName);
		return comment;
	}
	/// <summary>
	/// Get default setting comment for display
	/// </summary>
	/// <param name="tableName">Table name</param>
	/// <param name="fieldName">Field name</param>
	/// <returns>Comment</returns>
	protected string GetDefaultSettingCommentForDisplay(string tableName, string fieldName)
	{
		var comment = DefaultSettingPage.GetDefaultSettingComment(
			this.DefaultSettingInfo,
			tableName,
			fieldName);

		if (string.IsNullOrEmpty(comment)) return string.Empty;

		var result = string.Format(
			"[{0}]",
			this.DefaultSettingInfo.DefaultSettingTables[tableName].GetDefaultSettingCommentField(fieldName));
		return result;
	}

	/// <summary>
	/// Is default setting display field
	/// </summary>
	/// <param name="tableName">Table name</param>
	/// <param name="fieldName">Field name</param>
	/// <returns>TRUE: display / False: not display</returns>
	protected bool IsDefaultSettingDisplayField(string tableName, string fieldName)
	{
		var result = DefaultSettingPage.IsDefaultSettingDisplayField(
			this.DefaultSettingInfo,
			tableName,
			fieldName);
		return result;
	}

	/// <summary>
	/// Display default setting of input address
	/// </summary>
	private void DisplayDefaultSettingOfInputAddress()
	{
		if (m_strActionStatus != Constants.ACTION_STATUS_DEFAULTSETTING) return;

		// Checkbox user address 3
		cbUserAddr3Default.Enabled = true;
		if (CheckNecessaryAddress(Constants.FIELD_USER_ADDR3))
		{
			cbUserAddr3Default.Checked = true;
			cbUserAddr3Default.Enabled = false;
		}

		// Checkbox user address 4
		cbUserAddr4Default.Enabled = true;
		if (CheckNecessaryAddress(Constants.FIELD_USER_ADDR4))
		{
			cbUserAddr4Default.Checked = true;
			cbUserAddr4Default.Enabled = false;
		}

		// Checkbox user address 5
		cbUserAddr5Default.Enabled = true;
		if ((CheckNecessaryAddress(Constants.FIELD_USER_ADDR5))
			|| this.IsUserAddrUs)
		{
			cbUserAddr5Default.Checked = true;
			cbUserAddr5Default.Enabled = false;
		}

		// Checkbox zip global
		cbUserZipGlobalDefault.Enabled = true;
		if (CheckNecessaryAddress(Constants.FIELD_USER_ZIP))
		{
			cbUserZipGlobalDefault.Checked = true;
			cbUserZipGlobalDefault.Enabled = false;
		}
	}

	/// <summary>
	/// Check necessary address
	/// </summary>
	/// <param name="key">Key</param>
	/// <returns>True: necessary, otherwise: false</returns>
	public bool CheckNecessaryAddress(string key)
	{
		if (this.IsUserAddrJp)
		{
			var necessary = ReplaceTag(
				string.Format("@@User.{0}.necessary@@", key),
				this.UserAddrCountryIsoCode);
			return (necessary == "1");
		}

		// Other countries Japan
		var globalNecessary = ReplaceTag(
			string.Format("@@User.{0}.globalAddress.necessary@@", key),
			this.UserAddrCountryIsoCode);
		return (globalNecessary == "1");
	}

	/// <summary>
	/// Linkbutton search address from zipcode global click event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSearchAddrFromZipGlobal_Click(object sender, EventArgs e)
	{
		BindingAddressByGlobalZipcode(
			this.UserAddrCountryIsoCode,
			StringUtility.ToHankaku(tbUserZipGlobal.Text.Trim()),
			tbUserAddr2,
			tbUserAddr3,
			tbUserAddr4,
			tbUserAddr5,
			ddlUserAddr5);
	}

	/// <summary>
	/// コントロールツリーを再帰的に探索してコントロールを見つけるメソッド
	/// </summary>
	/// <param name="root">ルート</param>
	/// <param name="controlId">コントロールID</param>
	/// <returns>コントロール</returns>
	private Control FindControlRecursive(Control root, string controlId)
	{
		if (root.ID == controlId)
		{
			return root;
		}

		foreach (Control control in root.Controls)
		{
			Control foundControl = FindControlRecursive(control, controlId);
			if (foundControl != null)
			{
				return foundControl;
			}
		}

		return null;
	}

	/// <summary>ユーザーの住所が日本か</summary>
	protected bool IsUserAddrJp
	{
		get { return (IsCountryJp(this.UserAddrCountryIsoCode) && this.IsShippingCountryAvailableJp); }
	}
	/// <summary>ユーザーの住所がアメリカか</summary>
	protected bool IsUserAddrUs
	{
		get { return IsCountryUs(this.UserAddrCountryIsoCode); }
	}
	/// <summary>ユーザーの住所郵便番号が必須か</summary>
	protected bool IsUserAddrZipNecessary
	{
		get { return IsAddrZipcodeNecessary(this.UserAddrCountryIsoCode); ; }
	}
	/// <summary>ユーザーの住所国ISOコード</summary>
	protected string UserAddrCountryIsoCode
	{
		get { return ddlUserCountry.SelectedValue; }
	}
	/// <summary> 「パスワード欄表示」権限の判定 </summary>
	protected bool HavePasswordDisplayPower
	{
		get { return (MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_USER_PASSWORD_DISPLAY)); }
	}
	/// <summary>確認画面から戻って来た時、画面表示用ユーザー情報</summary>
	private UserInput UserInputForBack
	{
		get
		{
			if (IsBackFromConfirm == false) return null;

			var result = (UserInput)Session[Constants.SESSION_KEY_PARAM_FOR_BACK + _uniqueKey];
			return result;
		}
		set
		{
			Session[Constants.SESSION_KEY_PARAM_FOR_BACK + _uniqueKey] = value;
		}
	}
	/// <summary>確認画面から戻ってきたか</summary>
	private bool IsBackFromConfirm
	{
		get { return (Session[Constants.SESSION_KEY_PARAM_FOR_BACK + _uniqueKey] != null); }
	}
	/// <summary>Is default setting page</summary>
	protected bool IsDefaultSettingPage
	{
		get { return (this.ActionStatus == Constants.ACTION_STATUS_DEFAULTSETTING); }
	}
	/// <summary>Default setting</summary>
	private DefaultSetting m_defaultSetting = null;
	/// <summary>Default setting info</summary>
	protected DefaultSetting DefaultSettingInfo
	{
		get
		{
			if (m_defaultSetting == null)
			{
				m_defaultSetting = new DefaultSetting();
				m_defaultSetting.LoadDefaultSetting(this.LoginOperatorShopId, Constants.TABLE_USER);
			}
			return m_defaultSetting;
		}
	}
	/// <summary>User master</summary>
	protected object UserMaster { get; set; }
	/// <summary>ビジネスオーナーの入力データ</summary>
	protected object userBusinessOwnerMaster { get; set; }
	/// <summary>Is user addr tw</summary>
	protected bool IsUserAddrTw
	{
		get { return (IsCountryTw(this.UserAddrCountryIsoCode) && this.IsShippingCountryAvailableTw); }
	}
}
