/*
=========================================================================================================
  Module      : アドレス帳入力画面処理(UserShippingInput.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Global;
using w2.App.Common.Web.WrappedContols;
using w2.App.Common.Util;
using w2.App.Common.User;
using w2.Common.Web;
using w2.Domain.UserShipping;
using w2.Domain.CountryLocation;
using w2.Domain.User;

public partial class Form_User_UserShippingInput : ProductPage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return true; } }	// ログイン必須
	/// <summary>マイページメニュー表示判定</summary>
	public override bool DispMyPageMenu { get { return true; } }	// マイページメニュー表示

	# region ラップ済みコントロール宣言
	protected WrappedHtmlGenericControl WpRegistInfo { get { return GetWrappedControl<WrappedHtmlGenericControl>("pRegistInfo"); } }
	protected WrappedHtmlGenericControl WpModifyInfo { get { return GetWrappedControl<WrappedHtmlGenericControl>("pModifyInfo"); } }
	protected WrappedTextBox WtbName { get { return GetWrappedControl<WrappedTextBox>("tbName"); } }
	protected WrappedTextBox WtbShippingName1 { get { return GetWrappedControl<WrappedTextBox>("tbShippingName1"); } }
	protected WrappedTextBox WtbShippingName2 { get { return GetWrappedControl<WrappedTextBox>("tbShippingName2"); } }
	protected WrappedTextBox WtbShippingNameKana1 { get { return GetWrappedControl<WrappedTextBox>("tbShippingNameKana1"); } }
	protected WrappedTextBox WtbShippingNameKana2 { get { return GetWrappedControl<WrappedTextBox>("tbShippingNameKana2"); } }
	protected WrappedDropDownList WddlShippingCountry { get { return GetWrappedControl<WrappedDropDownList>("ddlShippingCountry"); } }
	protected WrappedDropDownList WddlShippingAddr1 { get { return GetWrappedControl<WrappedDropDownList>("ddlShippingAddr1"); } }
	protected WrappedTextBox WtbShippingZip1 { get { return GetWrappedControl<WrappedTextBox>("tbShippingZip1"); } }
	protected WrappedTextBox WtbShippingZip2 { get { return GetWrappedControl<WrappedTextBox>("tbShippingZip2"); } }
	protected WrappedTextBox WtbShippingZipGlobal { get { return GetWrappedControl<WrappedTextBox>("tbShippingZipGlobal"); } }
	protected WrappedTextBox WtbShippingAddr2 { get { return GetWrappedControl<WrappedTextBox>("tbShippingAddr2"); } }
	protected WrappedTextBox WtbShippingAddr3 { get { return GetWrappedControl<WrappedTextBox>("tbShippingAddr3"); } }
	protected WrappedTextBox WtbShippingAddr4 { get { return GetWrappedControl<WrappedTextBox>("tbShippingAddr4"); } }
	protected WrappedDropDownList WddlShippingAddr5 { get { return GetWrappedControl<WrappedDropDownList>("ddlShippingAddr5"); } }
	protected WrappedTextBox WtbShippingAddr5 { get { return GetWrappedControl<WrappedTextBox>("tbShippingAddr5"); } }
	protected WrappedTextBox WtbShippingCompanyName { get { return GetWrappedControl<WrappedTextBox>("tbShippingCompanyName"); } }
	protected WrappedTextBox WtbShippingCompanyPostName { get { return GetWrappedControl<WrappedTextBox>("tbShippingCompanyPostName"); } }
	protected WrappedTextBox WtbShippingTel1_1 { get { return GetWrappedControl<WrappedTextBox>("tbShippingTel1_1"); } }
	protected WrappedTextBox WtbShippingTel1_2 { get { return GetWrappedControl<WrappedTextBox>("tbShippingTel1_2"); } }
	protected WrappedTextBox WtbShippingTel1_3 { get { return GetWrappedControl<WrappedTextBox>("tbShippingTel1_3"); } }
	protected WrappedTextBox WtbShippingTel1Global { get { return GetWrappedControl<WrappedTextBox>("tbShippingTel1Global"); } }
	protected WrappedCustomValidator WcvShippingAddr2 { get { return GetWrappedControl<WrappedCustomValidator>("cvShippingAddr2"); } }
	protected WrappedCustomValidator WcvShippingAddr3 { get { return GetWrappedControl<WrappedCustomValidator>("cvShippingAddr3"); } }
	protected WrappedCustomValidator WcvShippingAddr4 { get { return GetWrappedControl<WrappedCustomValidator>("cvShippingAddr4"); } }
	protected WrappedLinkButton WlbConfirm { get { return GetWrappedControl<WrappedLinkButton>("lbConfirm"); } }
	protected WrappedDropDownList WddlShippingAddr2 { get { return GetWrappedControl<WrappedDropDownList>("ddlShippingAddr2"); } }
	protected WrappedDropDownList WddlShippingAddr3 { get { return GetWrappedControl<WrappedDropDownList>("ddlShippingAddr3"); } }
	protected WrappedLinkButton WlbSearchShippingAddr { get { return GetWrappedControl<WrappedLinkButton>("lbSearchShippingAddr"); } }
	protected WrappedTextBox WtbShippingZip { get { return GetWrappedControl<WrappedTextBox>("tbShippingZip"); } }
	protected WrappedTextBox WtbShippingTel1 { get { return GetWrappedControl<WrappedTextBox>("tbShippingTel1"); } }
	protected WrappedLinkButton WlbSearchAddrFromShippingZipGlobal { get { return GetWrappedControl<WrappedLinkButton>("lbSearchAddrFromShippingZipGlobal"); } }
	# endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// HTTPS通信チェック（HTTPのとき、アドレス帳一覧画面へ）
		CheckHttps(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_SHIPPING_LIST);

		if (!IsPostBack)
		{
			// コンポーネント初期化
			InitComponents();

			// ユーザー配送先情報項目セット
			var userShipping = GetUserShippingForDisplay();
			if (userShipping != null)
			{
				DisplayUserShipping(userShipping);

				this.WpRegistInfo.Visible = (this.ShippingNo == 0);
				this.WpModifyInfo.Visible = (this.ShippingNo != 0);
			}
			else
			{
				this.WpRegistInfo.Visible = true;
			}

			// 国切替初期化
			ddlShippingCountry_SelectedIndexChanged(sender, e);
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitComponents()
	{
		// 都道府県ドロップダウン作成
		this.WddlShippingAddr1.AddItem(new ListItem("", ""));
		foreach (string strPrefecture in Constants.STR_PREFECTURES_LIST)
		{
			this.WddlShippingAddr1.AddItem(new ListItem(strPrefecture));
		}

		// 国ドロップダウン作成
		var shippingAvailableCountries = new CountryLocationService().GetShippingAvailableCountry();
		this.WddlShippingCountry.Items.Add(new ListItem("", ""));
		this.WddlShippingCountry.Items.AddRange(shippingAvailableCountries.Select(c => new ListItem(c.CountryName, c.CountryIsoCode)).ToArray());
		var user = new UserService().Get(this.LoginUser.UserId);
		if (shippingAvailableCountries.Any(s => s.CountryIsoCode == user.AddrCountryIsoCode)) this.WddlShippingCountry.SelectedValue = user.AddrCountryIsoCode;

		// 州ドロップダウンリスト作成
		this.WddlShippingAddr5.Items.Add(new ListItem("", ""));
		this.WddlShippingAddr5.Items.AddRange(Constants.US_STATES_LIST.Select(state => new ListItem(state)).ToArray());

		this.WddlShippingAddr2.Items.AddRange(this.AddrTwCityList);
	}

	/// <summary>
	/// 表示用ユーザー配送先情報取得
	/// </summary>
	/// <returns>ユーザー配送先情報</returns>
	private UserShippingInput GetUserShippingForDisplay()
	{
		// アドレス帳編集画面の戻るボタンで来た場合、ターゲットページ情報がセットされている
		if (this.SessionParamTargetPage == Constants.PAGE_FRONT_USER_SHIPPING_INPUT)
		{
			// ターゲットページ情報がある場合は、セッションに配送先情報が存在している
			var userShipping = this.UserShipping;

			// ターゲットページ情報をクリア
			this.SessionParamTargetPage = null;

			return userShipping;
		}
		
		// セッション情報が存在しない場合は、パラメータの値でDBから値を取得
		//（アドレス帳リストから編集ボタンで来た場合、パラメータに配送先番号が含まれている）
		if (this.ShippingNo != 0)
		{
			var userShipping = new UserShippingService().Get(this.LoginUserId, this.ShippingNo);
			return new UserShippingInput(userShipping);
		}
		return null;
	}

	/// <summary>
	/// ユーザー配送先表示
	/// </summary>
	/// <param name="userShipping">ユーザー配送先</param>
	private void DisplayUserShipping(UserShippingInput userShipping)
	{
		this.WtbName.Text = userShipping.Name;
		this.WtbShippingName1.Text = userShipping.ShippingName1;
		this.WtbShippingName2.Text = userShipping.ShippingName2;
		this.WtbShippingNameKana1.Text = userShipping.ShippingNameKana1;
		this.WtbShippingNameKana2.Text = userShipping.ShippingNameKana2;
		this.WddlShippingCountry.SelectItemByValue(userShipping.ShippingCountryIsoCode);

		// Set value for zip code
		SetZipCodeTextbox(
			this.WtbShippingZip,
			this.WtbShippingZip1,
			this.WtbShippingZip2,
			userShipping.ShippingZip);

		this.WtbShippingZipGlobal.Text = userShipping.ShippingZip;
		this.WddlShippingAddr1.SelectItemByText(userShipping.ShippingAddr1);
		this.WtbShippingAddr2.Text = userShipping.ShippingAddr2;
		this.WtbShippingAddr3.Text = userShipping.ShippingAddr3;
		this.WtbShippingAddr4.Text = userShipping.ShippingAddr4;

		if (IsCountryUs(userShipping.ShippingCountryIsoCode))
		{
			this.WddlShippingAddr5.SelectItemByText(userShipping.ShippingAddr5);
		}
		else
		{
			this.WtbShippingAddr5.Text = userShipping.ShippingAddr5;
		}

		if (IsCountryTw(userShipping.ShippingCountryIsoCode)
			&& this.WddlShippingAddr2.HasInnerControl
			&& this.WddlShippingAddr3.HasInnerControl)
		{
			this.WddlShippingAddr2.SelectItemByValue(userShipping.ShippingAddr2);
			BindingDdlShippingAddr3();

			this.WddlShippingAddr3.ForceSelectItemByText(userShipping.ShippingAddr3);
		}

		this.WtbShippingCompanyName.Text = userShipping.ShippingCompanyName;
		this.WtbShippingCompanyPostName.Text = userShipping.ShippingCompanyPostName;

		// Set value for telephone
		SetTelTextbox(
			this.WtbShippingTel1,
			this.WtbShippingTel1_1,
			this.WtbShippingTel1_2,
			this.WtbShippingTel1_3,
			userShipping.ShippingTel1);

		this.WtbShippingTel1Global.Text = userShipping.ShippingTel1;
	}

	/// <summary>
	/// アドレス帳入力画面URL作成
	/// </summary>
	/// <returns></returns>
	/// <remarks>
	/// リセット時に利用
	/// </remarks>
	protected string CreateUserInputUrl()
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_SHIPPING_INPUT)
			.AddParam(Constants.REQUEST_KEY_SHIPPING_NO, this.ShippingNo.ToString()).CreateUrl();
		return url;
	}

	/// <summary>
	/// 「住所検索」ボタン押下イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSearchShippingAddr_Click(object sender, System.EventArgs e)
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
		// 入力値格納
		var userShippingInput = GetUserShippingInput();

		// 入力チェック
		var errorMessages = userShippingInput.Validate();

		if (this.WtbShippingZip1.HasInnerControl)
		{
			errorMessages.Remove(Constants.FIELD_USERSHIPPING_SHIPPING_ZIP);
		}

		if (this.WtbShippingTel1_1.HasInnerControl)
		{
			errorMessages.Remove(Constants.FIELD_USERSHIPPING_SHIPPING_TEL1);
		}

		if (errorMessages.Count != 0)
		{
			if (errorMessages.ContainsKey(Constants.FIELD_USERSHIPPING_SHIPPING_ZIP))
			{
				this.ZipInputErrorMessage = string.Empty;
			}

			var customValidators = new List<CustomValidator>();
			CreateCustomValidators(this, customValidators);

			SetControlViewsForError(this.ValidationGroup, errorMessages, customValidators);
			return;
		}

		this.UserShipping = userShippingInput;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_SHIPPING_CONFIRM);
	}

	/// <summary>
	/// ユーザー配送先情報取得
	/// </summary>
	/// <returns>ユーザー配送先情報</returns>
	private UserShippingInput GetUserShippingInput()
	{
		var shippingAddr2 = this.WtbShippingAddr2.Text.Trim();
		var shippingAddr3 = this.WtbShippingAddr3.Text.Trim();
		if (Constants.GLOBAL_OPTION_ENABLE
			&& this.IsShippingAddrTw
			&& this.WddlShippingAddr2.HasInnerControl
			&& this.WddlShippingAddr3.HasInnerControl)
		{
			shippingAddr2 = this.WddlShippingAddr2.SelectedValue;
			shippingAddr3 = this.WddlShippingAddr3.SelectedText;
		}

		// Set value for zip code
		var inputZipCode = (this.WtbShippingZip1.HasInnerControl)
			? StringUtility.ToHankaku(this.WtbShippingZip1.Text.Trim())
			: StringUtility.ToHankaku(this.WtbShippingZip.Text.Trim());
		if (this.WtbShippingZip1.HasInnerControl) inputZipCode += ("-" + StringUtility.ToHankaku(this.WtbShippingZip2.Text.Trim()));
		var zipCode = new ZipCode(inputZipCode);
		var shippingZip = (this.IsShippingAddrJp
				&& this.WtbShippingZip.HasInnerControl)
			? (string.IsNullOrEmpty(zipCode.Zip) == false)
				? zipCode.Zip
				: inputZipCode
			: StringUtility.ToHankaku(this.WtbShippingZipGlobal.Text);

		// Set value for telephone 1
		var inputTel1 = (this.WtbShippingTel1_1.HasInnerControl)
			? StringUtility.ToHankaku(this.WtbShippingTel1_1.Text.Trim())
			: StringUtility.ToHankaku(this.WtbShippingTel1.Text.Trim());
		if (this.WtbShippingTel1_1.HasInnerControl)
		{
			inputTel1 = UserService.CreatePhoneNo(
				inputTel1,
				StringUtility.ToHankaku(this.WtbShippingTel1_2.Text.Trim()),
				StringUtility.ToHankaku(this.WtbShippingTel1_3.Text.Trim()));
		}
		var tel1 = new Tel(inputTel1);
		var shippingTel1 = (this.IsShippingAddrJp
				&& this.WtbShippingTel1.HasInnerControl)
			? (string.IsNullOrEmpty(tel1.TelNo) == false)
				? tel1.TelNo
				: inputTel1
			: this.WtbShippingTel1Global.Text;

		return new UserShippingInput
		{
			UserId = this.LoginUserId,
			ShippingNo = this.ShippingNo.ToString(),
			Name = this.WtbName.Text,
			ShippingName = DataInputUtility.ConvertToFullWidthBySetting(this.WtbShippingName1.Text + this.WtbShippingName2.Text, this.IsShippingAddrJp),
			ShippingName1 = DataInputUtility.ConvertToFullWidthBySetting(this.WtbShippingName1.Text, this.IsShippingAddrJp),
			ShippingName2 = DataInputUtility.ConvertToFullWidthBySetting(this.WtbShippingName2.Text, this.IsShippingAddrJp),
			ShippingNameKana = (this.IsShippingAddrJp)
				? StringUtility.ToZenkaku(this.WtbShippingNameKana1.Text + this.WtbShippingNameKana2.Text)
				: string.Empty,
			ShippingNameKana1 = (this.IsShippingAddrJp)
				? StringUtility.ToZenkaku(this.WtbShippingNameKana1.Text)
				: string.Empty,
			ShippingNameKana2 = (this.IsShippingAddrJp)
				? StringUtility.ToZenkaku(this.WtbShippingNameKana2.Text)
				: string.Empty,
			ShippingCountryName = (Constants.GLOBAL_OPTION_ENABLE) ? this.WddlShippingCountry.SelectedText : string.Empty,
			ShippingCountryIsoCode = (Constants.GLOBAL_OPTION_ENABLE) ? this.WddlShippingCountry.SelectedValue : string.Empty,
			ShippingZip = shippingZip,
			ShippingZip1 = this.IsShippingAddrJp ? zipCode.Zip1 : string.Empty,
			ShippingZip2 = this.IsShippingAddrJp ? zipCode.Zip2 : string.Empty,
			ShippingAddr1 = this.WddlShippingAddr1.SelectedValue,
			ShippingAddr2 = DataInputUtility.ConvertToFullWidthBySetting(shippingAddr2, this.IsShippingAddrJp),
			ShippingAddr3 = DataInputUtility.ConvertToFullWidthBySetting(shippingAddr3, this.IsShippingAddrJp),
			ShippingAddr4 = DataInputUtility.ConvertToFullWidthBySetting(this.WtbShippingAddr4.Text, this.IsShippingAddrJp).Trim(),
			ShippingAddr5 = IsCountryUs(this.WddlShippingCountry.SelectedValue)
				? this.WddlShippingAddr5.SelectedText
				: DataInputUtility.ConvertToFullWidthBySetting(this.WtbShippingAddr5.Text, this.IsShippingAddrJp).Trim(),
			ShippingCompanyName = this.WtbShippingCompanyName.Text,
			ShippingCompanyPostName = this.WtbShippingCompanyPostName.Text,
			ShippingTel1_1 = tel1.Tel1,
			ShippingTel1_2 = tel1.Tel2,
			ShippingTel1_3 = tel1.Tel3,
			ShippingTel1 = shippingTel1,
			ShippingReceivingStoreFlg = Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF,
			ShippingReceivingStoreId = string.Empty,
		};
	}

	#region #ddlShippingCountry_SelectedIndexChanged 配送先住所国ドロップダウンリスト変更時イベント
	/// <summary>
	/// 配送先住所国ドロップダウンリスト変更時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlShippingCountry_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			// 国変更向けグローバル国設定CSS付与（クライアント検証用）
			AddCountryInfoToControlForChangeCountry(
				new WrappedWebControl[]
				{
					this.WtbShippingName1,
					this.WtbShippingName2,
					this.WtbShippingNameKana1,
					this.WtbShippingNameKana2,
					this.WtbShippingZip1,
					this.WtbShippingZip2,
					this.WtbShippingAddr2,
					this.WtbShippingAddr2,
					this.WtbShippingAddr2,
					this.WtbShippingAddr3,
					this.WtbShippingAddr4,
					this.WtbShippingTel1_1,
					this.WtbShippingTel1_2,
					this.WtbShippingTel1_3,
					this.WtbShippingZipGlobal,
					this.WtbShippingTel1Global,
				},
				this.ShippingAddrCountryIsoCode);
			// 国変更向けValidationGroup変更処理
			ChangeValidationGroupForChangeCountry(
				new WrappedControl[] { this.WcvShippingAddr2, this.WcvShippingAddr3, this.WcvShippingAddr4, this.WlbConfirm, },
				this.ValidationGroup);

			if (this.IsShippingAddrJp == false) this.WtbShippingZip2.Text = string.Empty;

			// Display Zip Global
			if (IsCountryTw(this.ShippingAddrCountryIsoCode) && this.WddlShippingAddr3.HasInnerControl)
			{
				BindingDdlShippingAddr3();
			}
		}
	}
	#endregion

	/// <summary>
	/// 台湾都市ドロップダウンリスト選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlShippingAddr2_SelectedIndexChanged(object sender, EventArgs e)
	{
		BindingDdlShippingAddr3();
	}

	/// <summary>
	/// 台湾地域ドロップダウンリスト生成
	/// </summary>
	protected void BindingDdlShippingAddr3()
	{
		GlobalAddressUtil.BindingDdlUserAddr3(
			WddlShippingAddr3,
			WtbShippingZipGlobal,
			this.UserTwDistrictDict[this.WddlShippingAddr2.SelectedItem.ToString()]);
	}

	/// <summary>
	/// Linkbutton search address from shipping zipcode global click event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSearchAddrFromShippingZipGlobal_Click(object sender, EventArgs e)
	{
		if (IsNotCountryJp(this.ShippingAddrCountryIsoCode) == false) return;

		BindingAddressByGlobalZipcode(
			this.ShippingAddrCountryIsoCode,
			StringUtility.ToHankaku(this.WtbShippingZipGlobal.Text.Trim()),
			this.WtbShippingAddr2,
			this.WtbShippingAddr4,
			this.WtbShippingAddr5,
			this.WddlShippingAddr2,
			this.WddlShippingAddr3,
			this.WddlShippingAddr5);
	}

	/// <summary>配送先枝番</summary>
	private int ShippingNo
	{
		get
		{
			int shippingNo;
			return int.TryParse(Request[Constants.REQUEST_KEY_SHIPPING_NO], out shippingNo) ? shippingNo : 0;
		}
	}
	/// <summary>郵便番号入力チェックエラー文言</summary>
	protected string ZipInputErrorMessage
	{
		get { return StringUtility.ToEmpty(ViewState["ZipInputErrorMessage"]); }
		set { ViewState["ZipInputErrorMessage"] = value; }
	}
	/// <summary>ユーザー配送先入力</summary>
	protected UserShippingInput UserShipping
	{
		get { return (UserShippingInput)Session[Constants.SESSION_KEY_PARAM]; }
		set { Session[Constants.SESSION_KEY_PARAM] = value; }
	}
	/// <summary>配送先の住所国ISOコード</summary>
	public string ShippingAddrCountryIsoCode
	{
		get { return this.WddlShippingCountry.SelectedValue; }
	}
	/// <summary>配送先の住所が日本か</summary>
	public bool IsShippingAddrJp
	{
		get { return GlobalAddressUtil.IsCountryJp(this.ShippingAddrCountryIsoCode); }
	}
	/// <summary>配送先の住所がUSか</summary>
	public bool IsShippingAddrUs
	{
		get { return GlobalAddressUtil.IsCountryUs(this.ShippingAddrCountryIsoCode); }
	}
	/// <summary>配送先の住所がTWか</summary>
	public bool IsShippingAddrTw
	{
		get { return GlobalAddressUtil.IsCountryTw(this.ShippingAddrCountryIsoCode); }
	}
	/// <summary>配送先の住所郵便番号が必須か</summary>
	public bool IsShippingAddrZipNecessary
	{
		get
		{
			var necessary = GlobalAddressUtil.IsAddrZipcodeNecessary(
				this.ShippingAddrCountryIsoCode);
			return necessary;
		}
	}
	/// <summary>バリデーショングループ</summary>
	public string ValidationGroup
	{
		get { return this.IsShippingAddrJp ? "UserShippingRegist" : "UserShippingRegistGlobal"; }
	}
}