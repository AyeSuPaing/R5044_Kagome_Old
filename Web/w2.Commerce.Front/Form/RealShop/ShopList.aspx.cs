/*
=========================================================================================================
  Module      : Shop List (ShopList.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.DataCacheController;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain.ProductBrand;
using w2.Domain.RealShop;

/// <summary>
/// Shop list
/// </summary>
public partial class Form_RealShop_ShopList : BasePage
{
	#region Wrapped control declaration
	/// <summary>Wrapped repeater area tags</summary>
	protected WrappedRepeater WrAreaTags { get { return GetWrappedControl<WrappedRepeater>("rAreaTags"); } }
	/// <summary>Wrapped repeater brand list</summary>
	protected WrappedRepeater WrBrandList { get { return GetWrappedControl<WrappedRepeater>("rBrandList"); } }
	#endregion

	/// <summary>
	/// Page load
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			if (Constants.REALSHOP_OPTION_ENABLED == false)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_404_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			// コンポーネント初期化
			InitComponents();

			// データバインド
			this.DataBind();
		}
	}

	/// <summary>
	/// Init components
	/// </summary>
	private void InitComponents()
	{
		this.AreaId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_AREA_ID]);
		this.BrandId = Constants.PRODUCT_BRAND_ENABLED
			? StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_BRAND_ID])
			: string.Empty;

		var brandIdsHasRealShop = new RealShopService()
			.GetRealShops(this.AreaId, this.BrandId)
			.Select(brand => brand.BrandId);

		var brandList = new ProductBrandService()
			.GetValidBrandList()
			.Where(brand => brandIdsHasRealShop.Contains(brand.BrandId));

		var realShopAreaList = DataCacheControllerFacade
			.GetRealShopAreaCacheController()
			.GetRealShopAreaList();

		// Set value to repeater
		this.WrBrandList.DataSource = brandList;
		this.WrAreaTags.DataSource = realShopAreaList;
	}

	/// <summary>
	/// Get real shops
	/// </summary>
	/// <param name="brandId">Brand id</param>
	/// <returns>Array of real shop model</returns>
	public RealShopModel[] GetRealShops(string brandId)
	{
		var result = new RealShopService().GetRealShops(this.AreaId, brandId);
		return result;
	}

	/// <summary>
	/// Create area link url
	/// </summary>
	/// <param name="areaId">Area id</param>
	/// <returns>Area link url</returns>
	public string CreateAreaLinkUrl(string areaId)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_SHOP_LIST)
			.AddParam(Constants.REQUEST_KEY_BRAND_ID, this.BrandId)
			.AddParam(Constants.REQUEST_KEY_AREA_ID, areaId)
			.CreateUrl();
		return url;
	}

	/// <summary>
	/// Create shop detail link url
	/// </summary>
	/// <param name="realShopId">Real shop id</param>
	/// <returns>Shop detail link url</returns>
	public string CreateShopDetailLinkUrl(string realShopId)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_SHOP_DETAIL)
			.AddParam(Constants.REQUEST_KEY_REAL_SHOP_ID, realShopId)
			.CreateUrl();
		return url;
	}

	/// <summary>Area id</summary>
	protected string AreaId { set; get; }
}
