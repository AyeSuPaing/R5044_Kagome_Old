/*
=========================================================================================================
  Module      : リアル店舗情報登録ページ処理(RealShopRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global;
using w2.App.Common.RealShop;
using w2.Common.Extensions;
using w2.Domain.CountryLocation;
using w2.Domain.ProductBrand;

public partial class Form_RealShop_RealShopRegister : RealShopPage
{
	private string REQUEST_KEY_SUCCESS = "success";

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
			// コンポーネント初期化
			//------------------------------------------------------
			InitializeComponents(this.ActionStatus);
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	/// <param name="actionStatus">処理ステータス</param>
	private void InitializeComponents(string actionStatus)
	{
		// 説明
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_REALSHOP, Constants.FIELD_REALSHOP_DESC1_KBN_PC))
		{
			rblDesc1KbnPC.Items.Add(li);
			if (li.Value == Constants.FLG_REALSHOP_DESC_TEXT) li.Selected = true;
		}
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_REALSHOP, Constants.FIELD_REALSHOP_DESC2_KBN_PC))
		{
			rblDesc2KbnPC.Items.Add(li);
			if (li.Value == Constants.FLG_REALSHOP_DESC_TEXT) li.Selected = true;
		}
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_REALSHOP, Constants.FIELD_REALSHOP_DESC1_KBN_SP))
		{
			rblDesc1KbnSP.Items.Add(li);
			if (li.Value == Constants.FLG_REALSHOP_DESC_TEXT) li.Selected = true;
		}
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_REALSHOP, Constants.FIELD_REALSHOP_DESC2_KBN_SP))
		{
			rblDesc2KbnSP.Items.Add(li);
			if (li.Value == Constants.FLG_REALSHOP_DESC_TEXT) li.Selected = true;
		}

		// 都道府県
		ddlAddr1.Items.Add(new ListItem(string.Empty, string.Empty));
		foreach (string strPrefecture in Constants.STR_PREFECTURES_LIST)
		{
			ddlAddr1.Items.Add(new ListItem(strPrefecture));
		}

		// 国
		var countries = new CountryLocationService().GetCountryNames();
		ddlCountry.Items.AddRange(countries.Select(c => new ListItem(c.CountryName, c.CountryIsoCode)).ToArray());
		ddlCountry.SelectedValue = Constants.OPERATIONAL_BASE_ISO_CODE;

		// 州
		ddlAddr5.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlAddr5.Items.AddRange(Constants.US_STATES_LIST.Select(state => new ListItem(state)).ToArray());

		// Set area item
		var realShopAreaList = DataCacheControllerFacade
			.GetRealShopAreaCacheController()
			.GetRealShopAreaList();

		ddlSettingArea.Items.Add(new ListItem(string.Empty, string.Empty));

		if (realShopAreaList != null)
		{
			var realShopAreaListItem = realShopAreaList
				.Select(realShopArea => new ListItem(realShopArea.AreaName, realShopArea.AreaId))
				.ToArray();

			ddlSettingArea.Items.AddRange(realShopAreaListItem);
		}

		// Set brand item
		var brandList = new ProductBrandService()
			.GetValidBrandList()
			.Select(brand => new ListItem(brand.BrandName, brand.BrandId))
			.ToArray();

		ddlSettingBrand.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlSettingBrand.Items.AddRange(brandList);

		// 新規登録？
		if (actionStatus == Constants.ACTION_STATUS_INSERT)
		{
			trInputRealShopId.Visible = true;
			trRegistTitle.Visible = true;
			btnUpdateTop.Visible
				= btnUpdateBottom.Visible
				= btnCopyInsertTop.Visible
				= btnCopyInsertBottom.Visible
				= btnDeleteTop.Visible
				= btnDeleteBottom.Visible = false;
		}
		else if (actionStatus == Constants.ACTION_STATUS_COPY_INSERT)
		{
			trInputRealShopId.Visible = true;
			trRegistTitle.Visible = true;
			btnUpdateTop.Visible
				= btnUpdateBottom.Visible
				= btnCopyInsertTop.Visible
				= btnCopyInsertBottom.Visible
				= btnDeleteTop.Visible
				= btnDeleteBottom.Visible = false;
		}
		else if (actionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			trDispRealShopId.Visible = true;
			trEditTitle.Visible = true;
			btnInsertTop.Visible = btnInsertBottom.Visible = false;
			divComp.Visible = (StringUtility.ToEmpty(Request[REQUEST_KEY_SUCCESS]) == "1");
		}
		if (actionStatus != Constants.ACTION_STATUS_INSERT)
		{
			SetRealShopInfoToControl(Request[Constants.REQUEST_KEY_REALSHOP_REAL_SHOP_ID]);
		}
	}

	/// <summary>
	/// 一覧へ戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBackList_Click(object sender, EventArgs e)
	{
		int pageNo;
		var parameters = (Hashtable)Session[Constants.SESSIONPARAM_KEY_REALSHOP_SEARCH_INFO];
		int.TryParse(StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_PAGE_NO]), out pageNo);
		Response.Redirect(CreateRealShopListUrl(parameters, (pageNo == 0) ? 1 : pageNo));
	}

	/// <summary>
	/// 住所検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnZipSearch_Click(object sender, EventArgs e)
	{
		var zipCodeUtil = new ZipcodeSearchUtility(tbZip1.Text + tbZip2.Text);
		if (!zipCodeUtil.Success) return;
		foreach (ListItem li in ddlAddr1.Items)
		{
			li.Selected = (li.Value == zipCodeUtil.PrefectureName);
		}
		tbAddr2.Text = zipCodeUtil.CityName + zipCodeUtil.TownName;
	}

	/// <summary>
	/// データチェック
	/// </summary>
	/// <returns>リアル店舗データ</returns>
	private Hashtable ValidateInputData()
	{
		string validator = null;
		//------------------------------------------------------
		// 処理ステータスの割り振り
		//------------------------------------------------------
		// 新規・コピー新規
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:
				validator = this.IsShopAddrJp ? "RealShopRegist" : "RealShopRegistGlobal";
				break;

			case Constants.ACTION_STATUS_UPDATE:
				validator = this.IsShopAddrJp ? "RealShopModify" : "RealShopModifyGlobal";
				break;
		}

		//------------------------------------------------------
		// パラメタ格納
		//------------------------------------------------------
		var input = new Hashtable();
		input.Add(Constants.FIELD_REALSHOP_REAL_SHOP_ID, tbRealShopId.Text.Trim());
		input.Add(Constants.FIELD_REALSHOP_NAME, tbRealShopName.Text.Trim());
		input.Add(Constants.FIELD_REALSHOP_NAME_KANA, tbRealShopNameKana.Text.Trim());

		input.Add(Constants.FIELD_REALSHOP_DESC1_KBN_PC, rblDesc1KbnPC.SelectedValue);
		input.Add(Constants.FIELD_REALSHOP_DESC1_PC, tblDesc1Pc.Text.Trim());
		input.Add(Constants.FIELD_REALSHOP_DESC2_KBN_PC, rblDesc2KbnPC.SelectedValue);
		input.Add(Constants.FIELD_REALSHOP_DESC2_PC, tblDesc2Pc.Text.Trim());

		input.Add(Constants.FIELD_REALSHOP_DESC1_KBN_SP, Constants.SMARTPHONE_OPTION_ENABLED ? rblDesc1KbnSP.SelectedValue : Constants.FLG_REALSHOP_DESC_TEXT);
		input.Add(Constants.FIELD_REALSHOP_DESC1_SP, tblDesc1Sp.Text.Trim());
		input.Add(Constants.FIELD_REALSHOP_DESC2_KBN_SP, Constants.SMARTPHONE_OPTION_ENABLED ? rblDesc2KbnSP.SelectedValue : Constants.FLG_REALSHOP_DESC_TEXT);
		input.Add(Constants.FIELD_REALSHOP_DESC2_SP, tblDesc2Sp.Text.Trim());

		input.Add(Constants.FIELD_USER_ZIP1, StringUtility.ToHankaku(tbZip1.Text.Trim()));
		input.Add(Constants.FIELD_USER_ZIP2, StringUtility.ToHankaku(tbZip2.Text.Trim()));
		if ((tbZip1.Text.Trim().Length != 0) || (tbZip2.Text.Trim().Length != 0))
		{
			input.Add(Constants.FIELD_REALSHOP_ZIP, StringUtility.ToHankaku(tbZip1.Text.Trim()) + "-" + StringUtility.ToHankaku(tbZip2.Text.Trim()));
		}
		else
		{
			input.Add(Constants.FIELD_REALSHOP_ZIP, string.Empty);
		}

		input.Add(Constants.FIELD_REALSHOP_ADDR1, ddlAddr1.SelectedValue);
		input.Add(Constants.FIELD_REALSHOP_ADDR2, this.IsShopAddrJp ? StringUtility.ToZenkaku(tbAddr2.Text.Trim()) : tbAddr2.Text.Trim());
		input.Add(Constants.FIELD_REALSHOP_ADDR3, this.IsShopAddrJp ? StringUtility.ToZenkaku(tbAddr3.Text.Trim()) : tbAddr3.Text.Trim());
		input.Add(Constants.FIELD_REALSHOP_ADDR4, this.IsShopAddrJp ? StringUtility.ToZenkaku(tbAddr4.Text.Trim()) : tbAddr4.Text.Trim());
		input.Add(Constants.FIELD_REALSHOP_TEL_1, StringUtility.ToHankaku(tbTel1_1.Text.Trim()));
		input.Add(Constants.FIELD_REALSHOP_TEL_2, StringUtility.ToHankaku(tbTel1_2.Text.Trim()));
		input.Add(Constants.FIELD_REALSHOP_TEL_3, StringUtility.ToHankaku(tbTel1_3.Text.Trim()));
		if ((tbTel1_1.Text.Trim().Length != 0) || (tbTel1_2.Text.Trim().Length != 0) || (tbTel1_3.Text.Trim().Length != 0))
		{
			input.Add(Constants.FIELD_REALSHOP_TEL, string.Format("{0}-{1}-{2}", StringUtility.ToHankaku(tbTel1_1.Text.Trim()), StringUtility.ToHankaku(tbTel1_2.Text.Trim()), StringUtility.ToHankaku(tbTel1_3.Text.Trim())));
		}
		else
		{
			input.Add(Constants.FIELD_REALSHOP_TEL, string.Empty);
		}

		input.Add(Constants.FIELD_REALSHOP_FAX_1, StringUtility.ToHankaku(tbFax1_1.Text.Trim()));
		input.Add(Constants.FIELD_REALSHOP_FAX_2, StringUtility.ToHankaku(tbFax1_2.Text.Trim()));
		input.Add(Constants.FIELD_REALSHOP_FAX_3, StringUtility.ToHankaku(tbFax1_3.Text.Trim()));
		if ((tbFax1_1.Text.Trim().Length != 0) || (tbFax1_2.Text.Trim().Length != 0) || (tbFax1_3.Text.Trim().Length != 0))
		{
			input.Add(Constants.FIELD_REALSHOP_FAX, string.Format("{0}-{1}-{2}", StringUtility.ToHankaku(tbFax1_1.Text.Trim()), StringUtility.ToHankaku(tbFax1_2.Text.Trim()), StringUtility.ToHankaku(tbFax1_3.Text.Trim())));
		}
		else
		{
			input.Add(Constants.FIELD_REALSHOP_FAX, string.Empty);
		}
		input.Add(Constants.FIELD_REALSHOP_URL, StringUtility.ToHankaku(tbUrl.Text.Trim()));
		input.Add(Constants.FIELD_REALSHOP_MAIL_ADDR, StringUtility.ToHankaku(tbMailAddr.Text.Trim()));
		input.Add(Constants.FIELD_REALSHOP_OPENING_HOURS, StringUtility.ToHankaku(tbOpeningHours.Text.Trim()));
		input.Add(Constants.FIELD_REALSHOP_DISPLAY_ORDER, StringUtility.ToHankaku(tbDisplayOrder.Text.Trim()));
		input.Add(Constants.FIELD_REALSHOP_VALID_FLG, cbValidFlg.Checked ? Constants.FLG_REALSHOP_VALID_FLG_VALID : Constants.FLG_REALSHOP_VALID_FLG_INVALID);
		
		input.Add(Constants.FIELD_REALSHOP_COUNTRY_ISO_CODE, string.Empty);
		input.Add(Constants.FIELD_REALSHOP_COUNTRY_NAME, string.Empty);
		input.Add(Constants.FIELD_REALSHOP_ADDR5, string.Empty);
		
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			input[Constants.FIELD_REALSHOP_COUNTRY_ISO_CODE] = ddlCountry.SelectedValue;
			input[Constants.FIELD_REALSHOP_COUNTRY_NAME] = ddlCountry.SelectedItem.Text;
			var addr5 = GlobalAddressUtil.IsCountryUs(ddlCountry.SelectedValue)
				? ddlAddr5.SelectedValue
				: tbAddr5.Text.Trim();
			input[Constants.FIELD_REALSHOP_ADDR5] = addr5;

			if (this.IsShopAddrJp == false)
			{
				input[Constants.FIELD_REALSHOP_ZIP] = StringUtility.ToHankaku(tbZipGlobal.Text.Trim()); ;
				input[Constants.FIELD_REALSHOP_ZIP1] = string.Empty;
				input[Constants.FIELD_REALSHOP_ZIP2] = string.Empty;
				input[Constants.FIELD_REALSHOP_TEL] = StringUtility.ToHankaku(tbTel1Global.Text.Trim());
				input[Constants.FIELD_REALSHOP_TEL_1] = string.Empty;
				input[Constants.FIELD_REALSHOP_TEL_2] = string.Empty;
				input[Constants.FIELD_REALSHOP_TEL_3] = string.Empty;
				input[Constants.FIELD_REALSHOP_FAX] = StringUtility.ToHankaku(tbFax1Global.Text.Trim()); ;
				input[Constants.FIELD_REALSHOP_FAX_1] = string.Empty;
				input[Constants.FIELD_REALSHOP_FAX_2] = string.Empty;
				input[Constants.FIELD_REALSHOP_FAX_3] = string.Empty;
			}
		}

		var address = ConcatenateAddress(input);
		input.Add(Constants.FIELD_REALSHOP_ADDR, address);
		input[Constants.FIELD_REALSHOP_AREA_ID] = ddlSettingArea.SelectedValue;
		input[Constants.FIELD_REALSHOP_BRAND_ID] = ddlSettingBrand.SelectedValue;
		input[Constants.FIELD_REALSHOP_LATITUDE] = StringUtility.ToNull(tbLatitude.Text);
		input[Constants.FIELD_REALSHOP_LONGITUDE] = StringUtility.ToNull(tbLongitude.Text);

		if (Constants.STORE_PICKUP_OPTION_ENABLED)
		{
			input.Add(Constants.FLG_MAIL_ADDR_STORE_PICK_UP_OPTION_VALID, StringUtility.ToHankaku(tbMailAddr.Text.Trim()));
		}

		//--------------------------------------------
		// 入力チェック＆重複チェック
		//--------------------------------------------
		string errorMessage = Validator.Validate(validator, input);
		if (errorMessage.Length != 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
		return input;
	}

	/// <summary>
	/// 住所項目結合
	/// </summary>
	/// <param name="input">入力値ハッシュテーブル</param>
	/// <returns>結合した住所</returns>
	private string ConcatenateAddress(Hashtable input)
	{
		string address = string.Empty;
		if (this.IsShopAddrJp)
		{
			address = (string)input[Constants.FIELD_REALSHOP_ADDR1]
				+ (string)input[Constants.FIELD_REALSHOP_ADDR2]
				+ (string)input[Constants.FIELD_REALSHOP_ADDR3]
				+ (string)input[Constants.FIELD_REALSHOP_ADDR4]
				+ (string)input[Constants.FIELD_REALSHOP_COUNTRY_NAME];
		}
		else
		{
			address = (string)input[Constants.FIELD_REALSHOP_ADDR2]
				+ ((string.IsNullOrEmpty((string)input[Constants.FIELD_REALSHOP_ADDR3]) == false) ? " " : "")
				+ (string)input[Constants.FIELD_REALSHOP_ADDR3]
				+ " "
				+ (string)input[Constants.FIELD_REALSHOP_ADDR4]
				+ ((string.IsNullOrEmpty((string)input[Constants.FIELD_REALSHOP_ADDR5]) == false) ? " " : "")
				+ (string)input[Constants.FIELD_REALSHOP_ADDR5]
				+ " "
				+ (string)input[Constants.FIELD_REALSHOP_COUNTRY_NAME];
		}
		return address;
	}

	/// <summary>
	/// リアル店舗情報再表示
	/// </summary>
	/// <param name="realShopId">リアル店舗ID</param>
	private void SetRealShopInfoToControl(string realShopId)
	{
		var realShopDetail = new Hashtable();
		if (this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT
				|| this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			var realShopInfo = RealShop.GetRealShopDetail(realShopId);
			if (realShopInfo.Count > 0)
			{
				realShopDetail = realShopInfo[0].ToHashtable();
			}
			else
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
		}

		if (realShopDetail.Count == 0) return;
		tbRealShopId.Text = lRealShopId.Text = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_REAL_SHOP_ID]);
		tbRealShopName.Text = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_NAME]);
		tbRealShopNameKana.Text = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_NAME_KANA]);
		tbZip1.Text = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_ZIP1]);
		tbZip2.Text = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_ZIP2]);
		ddlAddr1.SelectedValue = realShopDetail[Constants.FIELD_REALSHOP_ADDR1].ToString();

		tbAddr2.Text = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_ADDR2]);
		tbAddr3.Text = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_ADDR3]);
		tbAddr4.Text = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_ADDR4]);

		tbTel1_1.Text = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_TEL_1]);
		tbTel1_2.Text = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_TEL_2]);
		tbTel1_3.Text = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_TEL_3]);

		tbFax1_1.Text = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_FAX_1]);
		tbFax1_2.Text = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_FAX_2]);
		tbFax1_3.Text = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_FAX_3]);

		tbMailAddr.Text = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_MAIL_ADDR]);
		tbUrl.Text = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_URL]);
		tbOpeningHours.Text = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_OPENING_HOURS]);
		tbDisplayOrder.Text = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_DISPLAY_ORDER]);
		cbValidFlg.Checked = realShopDetail[Constants.FIELD_REALSHOP_VALID_FLG].ToString() == Constants.FLG_REALSHOP_VALID_FLG_VALID;

		rblDesc1KbnPC.SelectedValue = realShopDetail[Constants.FIELD_REALSHOP_DESC1_KBN_PC].ToString();
		rblDesc2KbnPC.SelectedValue = realShopDetail[Constants.FIELD_REALSHOP_DESC2_KBN_PC].ToString();
		rblDesc1KbnSP.SelectedValue = realShopDetail[Constants.FIELD_REALSHOP_DESC1_KBN_SP].ToString();
		rblDesc2KbnSP.SelectedValue = realShopDetail[Constants.FIELD_REALSHOP_DESC2_KBN_SP].ToString();

		tblDesc1Pc.Text = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_DESC1_PC]);
		tblDesc2Pc.Text = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_DESC2_PC]);
		tblDesc1Sp.Text = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_DESC1_SP]);
		tblDesc2Sp.Text = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_DESC2_SP]);

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			ddlCountry.SelectedValue = realShopDetail[Constants.FIELD_REALSHOP_COUNTRY_ISO_CODE].ToString();
			tbZipGlobal.Text = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_ZIP]);
			tbTel1Global.Text = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_TEL]);
			tbFax1Global.Text = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_FAX]);

			if (GlobalAddressUtil.IsCountryUs(realShopDetail[Constants.FIELD_REALSHOP_COUNTRY_ISO_CODE].ToString()))
			{
				ddlAddr5.SelectedValue = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_ADDR5]);
			}
			else
			{
				tbAddr5.Text = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_ADDR5]);
			}
		}

		ddlSettingArea.SelectedValue = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_AREA_ID]);
		ddlSettingBrand.SelectedValue = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_BRAND_ID]);
		tbLatitude.Text = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_LATITUDE]);
		tbLongitude.Text = StringUtility.ToEmpty(realShopDetail[Constants.FIELD_REALSHOP_LONGITUDE]);
	}

	/// <summary>
	/// 登録する/更新するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsertUpdate_Click(object sender, EventArgs e)
	{
		var realShopInfo = ValidateInputData();

		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:
				RealShop.InsertRealShop(realShopInfo);
				break;

			case Constants.ACTION_STATUS_UPDATE:
				realShopInfo[Constants.FIELD_REALSHOP_LAST_CHANGED] = this.LoginOperatorName;
				RealShop.UpdateRealShop(realShopInfo);
				break;
		}
		Response.Redirect(CreateRealShopRegistUrl(tbRealShopId.Text.Trim(), Constants.ACTION_STATUS_UPDATE) + "&" + REQUEST_KEY_SUCCESS + "=1");
	}

	/// <summary>
	/// コピー新規登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, EventArgs e)
	{
		// 登録画面へ
		Response.Redirect(CreateRealShopRegistUrl(Request[Constants.REQUEST_KEY_REALSHOP_REAL_SHOP_ID], Constants.ACTION_STATUS_COPY_INSERT));
	}

	/// <summary>
	/// 削除するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 削除用Hashtable作成
		//------------------------------------------------------
		var input = new Hashtable();
		input.Add(Constants.FIELD_REALSHOP_REAL_SHOP_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_REALSHOP_REAL_SHOP_ID]));
		//------------------------------------------------------
		// DB削除
		//------------------------------------------------------
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			sqlAccessor.OpenConnection();
			// リアル店舗情報削除
			using (SqlStatement sqlStatement = new SqlStatement("RealShop", "DeleteRealShop"))
			{
				sqlStatement.ExecStatement(sqlAccessor, input);
			}
			// リアル店舗在庫情報削除
			using (SqlStatement sqlStatement = new SqlStatement("RealShopProductStock", "DeleteRealShopProductStockAll"))
			{
				sqlStatement.ExecStatement(sqlAccessor, input);
			}
		}
		//------------------------------------------------------
		// 一覧画面へ戻る
		//------------------------------------------------------
		Response.Redirect(CreateRealShopListUrl((Hashtable)Session[Constants.SESSIONPARAM_KEY_REALSHOP_SEARCH_INFO], 1));
	}

	/// <summary>
	/// Linkbutton search address from zipcode global click event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSearchAddrFromZipGlobal_Click(object sender, EventArgs e)
	{
		BindingAddressByGlobalZipcode(
			this.ShopAddrCountryIsoCode,
			StringUtility.ToHankaku(tbZipGlobal.Text.Trim()),
			tbAddr2,
			tbAddr3,
			tbAddr4,
			tbAddr5,
			ddlAddr5);
	}

	/// <summary>店舗の住所が日本か</summary>
	protected bool IsShopAddrJp
	{
		get { return IsCountryJp(this.ShopAddrCountryIsoCode); }
	}
	/// <summary>店舗の住所がアメリカか</summary>
	protected bool IsShopAddrUs
	{
		get { return IsCountryUs(this.ShopAddrCountryIsoCode); }
	}
	/// <summary>店舗の住所国ISOコード</summary>
	protected string ShopAddrCountryIsoCode
	{
		get { return ddlCountry.SelectedValue; }
	}
}