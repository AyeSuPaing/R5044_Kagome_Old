/*
=========================================================================================================
  Module      : ユーザアドレス登録ページ処理(UserShippingInput.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using w2.Common.Web;
using w2.Domain.UserShipping;
using w2.App.Common;
using w2.Domain.CountryLocation;
using w2.App.Common.Util;

public partial class Form_User_UserShippingInput : ProductPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{

		if (!IsPostBack)
		{
			InitComponents(this.ActionStatus);

			// セット用ユーザー配送先情報取得
			var userShipping = GetUserShippingForSet();
			Session[Constants.SESSION_KEY_PARAM_FOR_USER_SHIPPING_INPUT] = null;
			Session[Constants.SESSION_KEY_PARAM_FOR_BACK] = null;

			// ユーザー配送先情報項目セット
			if (userShipping != null)
			{
				tbName.Text = userShipping.Name;
				tbShippingName1.Text = userShipping.ShippingName1;
				tbShippingName2.Text = userShipping.ShippingName2;
				tbShippingNameKana1.Text = userShipping.ShippingNameKana1;
				tbShippingNameKana2.Text = userShipping.ShippingNameKana2;
				tbShippingZip1.Text = userShipping.ShippingZip1;
				tbShippingZip2.Text = userShipping.ShippingZip2;
				ddlShippingAddr1.SelectedValue = userShipping.ShippingAddr1;
				tbShippingAddr2.Text = userShipping.ShippingAddr2;
				tbShippingAddr3.Text = userShipping.ShippingAddr3;
				tbShippingAddr4.Text = userShipping.ShippingAddr4;
				tbShippingCompanyName.Text = userShipping.ShippingCompanyName;
				tbShippingCompanyPostName.Text = userShipping.ShippingCompanyPostName;
				tbShippingTel1_1.Text = userShipping.ShippingTel1_1;
				tbShippingTel1_2.Text = userShipping.ShippingTel1_2;
				tbShippingTel1_3.Text = userShipping.ShippingTel1_3;

				if (Constants.GLOBAL_OPTION_ENABLE)
				{
					ddlShippingCountry.SelectedValue = userShipping.ShippingCountryIsoCode;
					if (this.IsShippingAddrUs)
					{
						ddlShippingAddr5.SelectedValue = userShipping.ShippingAddr5;
					}
					else
					{
						tbShippingAddr5.Text = userShipping.ShippingAddr5;
					}
					tbShippingZipGlobal.Text = userShipping.ShippingZip;
					tbShippingTel1Global.Text = userShipping.ShippingTel1;
				}
			}
		}
	}

	/// <summary>
	/// 画面セット用ユーザー配送先取得
	/// </summary>
	private UserShippingModel GetUserShippingForSet()
	{
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_UPDATE:
				// ユーザー配送先情報取得
				var sessionUserShipping = (UserShippingModel)this.Session[Constants.SESSION_KEY_PARAM_FOR_USER_SHIPPING_INPUT];
				if ((sessionUserShipping == null) || (this.Session[Constants.SESSION_KEY_PARAM_FOR_BACK] == null)
					|| (sessionUserShipping.ShippingNo.ToString() != StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_SHIPPING_NO])))
				{
					var userShipping = new UserShippingService().Get(this.UserId, this.ShippingNo);
					return userShipping;
				}
				else
				{
					return sessionUserShipping;
				}

			case Constants.ACTION_STATUS_INSERT:
				if ((this.Session[Constants.SESSION_KEY_PARAM_FOR_USER_SHIPPING_INPUT] != null) && (this.Session[Constants.SESSION_KEY_PARAM_FOR_BACK] != null))
				{
					var userShipping = (UserShippingModel)this.Session[Constants.SESSION_KEY_PARAM_FOR_USER_SHIPPING_INPUT];
					return userShipping;
				}
				break;
		}
		return null;
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	/// <param name="actionStatus">アクションステータス</param>
	private void InitComponents(string actionStatus)
	{
		if (actionStatus == Constants.ACTION_STATUS_INSERT)
		{
			pRegistInfo.Visible = true;
		}
		else if (actionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			pModifyInfo.Visible = true;
		}

		// 都道府県ドロップダウン作成
		this.ddlShippingAddr1.Items.Add(new ListItem("", ""));
		foreach (string strPrefecture in Constants.STR_PREFECTURES_LIST)
		{
			this.ddlShippingAddr1.Items.Add(new ListItem(strPrefecture));
		}

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			// 国ドロップダウン作成
			var shippingAvailableCountries = new CountryLocationService().GetShippingAvailableCountry();
			ddlShippingCountry.Items.AddRange(shippingAvailableCountries.Select(c => new ListItem(c.CountryName, c.CountryIsoCode)).ToArray());
			ddlShippingCountry.SelectedValue = Constants.OPERATIONAL_BASE_ISO_CODE;

			// 州ドロップダウン作成
			ddlShippingAddr5.Items.AddRange(Constants.US_STATES_LIST.Select(state => new ListItem(state)).ToArray());
		}
	}

	/// <summary>
	/// 郵便番号検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnZipSearch_Click(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// 郵便番号入力チェック
		//------------------------------------------------------
		string zip = StringUtility.ToHankaku(this.tbShippingZip1.Text + this.tbShippingZip2.Text);

		// 入力チェック
		var errorMessages = new StringBuilder();
		var zipNoText = ReplaceTag("@@User.zip.name@@");
		errorMessages.Append(Validator.CheckNecessaryError(zipNoText, zip));
		errorMessages.Append(Validator.CheckHalfwidthNumberError(zipNoText, zip));
		errorMessages.Append(Validator.CheckLengthError(zipNoText, zip, 7));

		//------------------------------------------------------
		// 入力チェックOKの場合、郵便番号検索実行＆セット
		//------------------------------------------------------
		if (errorMessages.Length == 0)
		{
			errorMessages.Append(
				SearchAddrFromZip(this.tbShippingZip1, this.tbShippingZip2, this.ddlShippingAddr1, this.tbShippingAddr2));
		}

		//------------------------------------------------------
		// エラーメッセージ表示
		//------------------------------------------------------
		this.ZipInputErrorMessage = errorMessages.ToString();
		if (this.ZipInputErrorMessage != "")
		{
			// 郵便番号検索ボックス選択
			this.tbShippingZip1.Focus();
		}
	}

	/// <summary>
	/// 郵便番号検索（テキストボックス）
	/// </summary>
	/// <param name="tbZip1">郵便番号テキストボックス1</param>
	/// <param name="tbZip2">郵便番号テキストボックス2</param>
	/// <param name="ddlAddr1">住所1ドロップダウン</param>
	/// <param name="tbAddr2">住所2テキストボックス</param>
	/// <returns>エラーメッセージ</returns>
	protected string SearchAddrFromZip(TextBox tbZip1, TextBox tbZip2, DropDownList ddlAddr1, TextBox tbAddr2)
	{
		// 郵便番号検索
		var zipcodeUtil = new ZipcodeSearchUtility(StringUtility.ToHankaku(tbZip1.Text + tbZip2.Text));
		if (zipcodeUtil.Success)
		{
			foreach (ListItem li in ddlAddr1.Items)
			{
				li.Selected = (li.Value == zipcodeUtil.PrefectureName);
			}
			tbAddr2.Text = zipcodeUtil.CityName + zipcodeUtil.TownName;
			tbAddr2.Focus();
		}
		else
		{
			// 郵便番号1へフォーカス
			tbZip1.Focus();

			// 郵便番号検索該当データなしエラー
			return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ZIPCODE_NO_ADDR);
		}

		return "";
	}

	/// <summary>
	/// 入力データをチェックし、確認ページに移動します
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnComfirm_Click(object sender, EventArgs e)
	{
		// 入力値格納
		var userShippingInput = new UserShippingInput
		{
			UserId = this.UserId,
			ShippingNo = this.ShippingNo.ToString(),
			Name = this.tbName.Text,
			ShippingName = DataInputUtility.ConvertToFullWidthBySetting(this.tbShippingName1.Text + this.tbShippingName2.Text, this.IsShippingAddrJp),
			ShippingName1 = DataInputUtility.ConvertToFullWidthBySetting(this.tbShippingName1.Text, this.IsShippingAddrJp),
			ShippingName2 = DataInputUtility.ConvertToFullWidthBySetting(this.tbShippingName2.Text, this.IsShippingAddrJp),
			ShippingNameKana = StringUtility.ToZenkaku(this.tbShippingNameKana1.Text + this.tbShippingNameKana2.Text),
			ShippingNameKana1 = StringUtility.ToZenkaku(this.tbShippingNameKana1.Text),
			ShippingNameKana2 = StringUtility.ToZenkaku(this.tbShippingNameKana2.Text),
			ShippingCountryIsoCode = Constants.GLOBAL_OPTION_ENABLE ? ddlShippingCountry.SelectedValue : string.Empty,
			ShippingCountryName = Constants.GLOBAL_OPTION_ENABLE ? ddlShippingCountry.SelectedItem.Text : string.Empty,
			ShippingZip = StringUtility.ToHankaku(tbShippingZipGlobal.Text),
			ShippingZip1 = this.IsShippingAddrJp ? StringUtility.ToHankaku(this.tbShippingZip1.Text) : string.Empty,
			ShippingZip2 = this.IsShippingAddrJp ? StringUtility.ToHankaku(this.tbShippingZip2.Text) : string.Empty,
			ShippingAddr1 = this.ddlShippingAddr1.SelectedValue,
			ShippingAddr2 = DataInputUtility.ConvertToFullWidthBySetting(this.tbShippingAddr2.Text, this.IsShippingAddrJp).Trim(),
			ShippingAddr3 = DataInputUtility.ConvertToFullWidthBySetting(this.tbShippingAddr3.Text, this.IsShippingAddrJp).Trim(),
			ShippingAddr4 = DataInputUtility.ConvertToFullWidthBySetting(this.tbShippingAddr4.Text, this.IsShippingAddrJp).Trim(),
			ShippingAddr5 = string.Empty,
			ShippingCompanyName = this.tbShippingCompanyName.Text,
			ShippingCompanyPostName = this.tbShippingCompanyPostName.Text,
			ShippingTel1 = Constants.GLOBAL_OPTION_ENABLE ? StringUtility.ToHankaku(tbShippingTel1Global.Text) : string.Empty,
			ShippingTel1_1 = this.IsShippingAddrJp ? StringUtility.ToHankaku(this.tbShippingTel1_1.Text) : string.Empty,
			ShippingTel1_2 = this.IsShippingAddrJp ? StringUtility.ToHankaku(this.tbShippingTel1_2.Text) : string.Empty,
			ShippingTel1_3 = this.IsShippingAddrJp ? StringUtility.ToHankaku(this.tbShippingTel1_3.Text) : string.Empty,
			ShippingReceivingStoreFlg = Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF
		};

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			userShippingInput.ShippingAddr5 = this.IsShippingAddrUs
				? ddlShippingAddr5.SelectedValue
				: DataInputUtility.ConvertToFullWidthBySetting(tbShippingAddr5.Text, this.IsShippingAddrJp).Trim();
		}

		// 入力チェック
		string errorMessages = userShippingInput.Validate();
		if (errorMessages != "")
		{
			lbErrorMessage.Text = errorMessages;
			divErrorMessage.Visible = true;
			return;
		}

		var model = userShippingInput.CreateModel();
		Session[Constants.SESSION_KEY_PARAM_FOR_USER_SHIPPING_INPUT] = model;

		// アドレス帳確認画面遷移
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_USER_SHIPPING_CONFIRM)
			.AddParam(Constants.REQUEST_KEY_USER_ID, this.UserId)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, this.ActionStatus).CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// データをクリア
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnReset_Click(object sender, EventArgs e)
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_USER_SHIPPING_INPUT);
		urlCreator = urlCreator.AddParam(Constants.REQUEST_KEY_SHIPPING_NO, this.ShippingNo.ToString());
		if (this.UserId != null) urlCreator.AddParam(Constants.REQUEST_KEY_USER_ID, this.UserId);
		if (this.ActionStatus != null) urlCreator.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, this.ActionStatus);
		var url = urlCreator.CreateUrl();

		if (this.ShippingNo != 0) Session[Constants.SESSION_KEY_PARAM_FOR_USER_SHIPPING_INPUT] = null;

		Response.Redirect(url);
	}

	/// <summary>
	/// Linkbutton search address from zipcode global click event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSearchAddrFromZipGlobal_Click(object sender, EventArgs e)
	{
		BindingAddressByGlobalZipcode(
			this.ShippingAddrCountryIsoCode,
			StringUtility.ToHankaku(tbShippingZipGlobal.Text.Trim()),
			tbShippingAddr2,
			tbShippingAddr3,
			tbShippingAddr4,
			tbShippingAddr5,
			ddlShippingAddr5);
	}

	/// <summary>（リクエストから取得できる）ユーザーID</summary>
	protected string UserId
	{
		get { return Request[Constants.REQUEST_KEY_USER_ID]; }
	}
	/// <summary>（リクエストから取得できる）配送先枝番（取得できない場合は0）</summary>
	protected int ShippingNo
	{
		get { return int.Parse(Request[Constants.REQUEST_KEY_SHIPPING_NO] ?? "0"); }
	}
	/// <summary>郵便番号入力チェックエラー文言</summary>
	protected string ZipInputErrorMessage
	{
		get { return StringUtility.ToEmpty(ViewState["ZipInputErrorMessage"]); }
		set { ViewState["ZipInputErrorMessage"] = value; }
	}
	/// <summary>配送先の住所が日本か</summary>
	protected bool IsShippingAddrJp
	{
		get { return IsCountryJp(this.ShippingAddrCountryIsoCode); }
	}
	/// <summary>配送先の住所がアメリカか</summary>
	protected bool IsShippingAddrUs
	{
		get { return IsCountryUs(this.ShippingAddrCountryIsoCode); }
	}
	/// <summary>配送先の住所郵便番号が必須か</summary>
	protected bool IsShippingAddrZipNecessary
	{
		get { return IsAddrZipcodeNecessary(this.ShippingAddrCountryIsoCode); ; }
	}
	/// <summary>配送先の住所国ISOコード</summary>
	protected string ShippingAddrCountryIsoCode
	{
		get { return ddlShippingCountry.SelectedValue; }
	}
}