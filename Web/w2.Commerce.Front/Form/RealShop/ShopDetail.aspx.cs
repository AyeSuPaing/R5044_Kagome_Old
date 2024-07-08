/*
=========================================================================================================
  Module      : Shop Detail (ShopDetail.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Global.Region;
using w2.Domain.RealShop;

/// <summary>
/// Shop Detail
/// </summary>
public partial class Form_RealShop_ShopDetail : BasePage
{
	/// <summary>Google map url</summary>
	private const string GOOGLE_MAP_URL = "https://www.google.com/maps";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!this.IsPostBack)
		{
			this.RealShop = new RealShopService().Get(Request[Constants.REQUEST_KEY_REAL_SHOP_ID]);

			if ((Constants.REALSHOP_OPTION_ENABLED == false)
				|| (this.RealShop == null))
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = Constants.REALSHOP_OPTION_ENABLED
					? WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_IRREGULAR_PARAMETER_ERROR)
					: WebMessages.GetMessages(WebMessages.ERRMSG_404_ERROR);

				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			// データバインド
			this.DataBind();
		}
	}

	/// <summary>
	/// Create google map url
	/// </summary>
	/// <returns>Google map url</returns>
	protected string CreateGoogleMapUrl()
	{
		var result = string.Format(
			"{0}?q=@{1},{2}&output=embed&hl={3}",
			GOOGLE_MAP_URL,
			this.RealShop.Latitude,
			this.RealShop.Longitude,
			RegionManager.GetInstance().Region.LanguageLocaleId);
		return result;
	}

	/// <summary>Real shop</summary>
	protected RealShopModel RealShop { get; private set; }
	/// <summary>Has setting google map</summary>
	protected bool HasSettingGoogleMap
	{
		get
		{
			return (this.RealShop.Latitude != null)
				&& (this.RealShop.Longitude != null);
		}
	}

	/// <summary>
	/// リアル店舗説明取得(Text,Html判定）
	/// </summary>
	/// <param name="field">フィールド名</param>
	/// <returns>リアル店舗説明</returns>
	protected string GetRealShopDataHtml(string field)
	{
		string kbnField = null;
		switch (field)
		{
			case Constants.FIELD_REALSHOP_DESC1_PC:
				kbnField = Constants.FIELD_REALSHOP_DESC1_KBN_PC;
				break;
			case Constants.FIELD_REALSHOP_DESC2_PC:
				kbnField = Constants.FIELD_REALSHOP_DESC2_KBN_PC;
				break;
			case Constants.FIELD_REALSHOP_DESC1_SP:
				kbnField = Constants.FIELD_REALSHOP_DESC1_KBN_SP;
				break;
			case Constants.FIELD_REALSHOP_DESC2_SP:
				kbnField = Constants.FIELD_REALSHOP_DESC2_KBN_SP;
				break;
		}

		var realShopDataHtml = ProductPage.GetProductDescHtml(
			(string)GetKeyValue(this.RealShop, kbnField),
			(string)GetKeyValue(this.RealShop, field));
		return realShopDataHtml;
	}
	
}
