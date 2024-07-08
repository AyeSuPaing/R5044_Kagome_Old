/*
=========================================================================================================
  Module      : Facebook External Data(FacebookExternalData.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using w2.App.Common.Facebook;
using w2.Common.Util;
using w2.Domain;

namespace w2.Commerce.Batch.LiaiseFacebookMall.Settings
{
	/// <summary>
	/// Facebook External Data
	/// </summary>
	public class FacebookExternalData
	{
		#region Constructor
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">Data source</param>
		public FacebookExternalData(Hashtable source)
		{
			this.DataSource = source;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Create sub image urls
		/// </summary>
		/// <returns>A List of sub image url</returns>
		public List<string> CreateSubImageUrls()
		{
			var result = new List<string>();
			if (string.IsNullOrEmpty(this.AdditionalImageUrl)) return result;

			var productSubImageSettings = DomainFacade.Instance.ProductSubImageSettingService.GetProductSubImageSettings(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.PRODUCTSUBIMAGE_MAXCOUNT);
			foreach (var productSubImageSetting in productSubImageSettings)
			{
				var imageUrl = string.Format(
					@"{0}{1}{2}{3}{4}/{5}{6}{7}{8}",
					Constants.PROTOCOL_HTTPS,
					string.IsNullOrEmpty(Constants.WEBHOOK_DOMAIN)
						? Constants.SITE_DOMAIN
						: Constants.WEBHOOK_DOMAIN,
					Constants.PATH_ROOT_FRONT_PC,
					Constants.PATH_PRODUCTSUBIMAGES,
					Constants.CONST_DEFAULT_SHOP_ID,
					this.AdditionalImageUrl,
					Constants.PRODUCTSUBIMAGE_FOOTER,
					productSubImageSetting.ProductSubImageNo,
					Constants.PRODUCTIMAGE_FOOTER_LL);
				result.Add(imageUrl);
			}

			return result;
		}

		/// <summary>
		/// Create main image url
		/// </summary>
		/// <returns>A url of main image</returns>
		public string CreateMainImageUrl()
		{
			var domain = string.IsNullOrEmpty(Constants.WEBHOOK_DOMAIN)
				? Constants.SITE_DOMAIN
				: Constants.WEBHOOK_DOMAIN;
			var result = (string.IsNullOrEmpty(this.ImageUrl) == false)
				? string.Format(
					@"{0}{1}{2}{3}{4}/{5}{6}",
					Constants.PROTOCOL_HTTPS,
					domain,
					Constants.PATH_ROOT_FRONT_PC,
					Constants.PATH_PRODUCTIMAGES,
					Constants.CONST_DEFAULT_SHOP_ID,
					this.ImageUrl,
					Constants.PRODUCTIMAGE_FOOTER_LL)
				: string.Format(
					"{0}{1}{2}{3}{4}",
					Constants.PROTOCOL_HTTPS,
					domain,
					Constants.PATH_ROOT_FRONT_PC,
					Constants.PATH_PRODUCTIMAGES,
					Constants.PRODUCTIMAGE_NOIMAGE_PC_LL);
			return result;
		}

		/// <summary>
		/// Get data request fields
		/// </summary>
		/// <returns>A facebook catalog request field api</returns>
		public FacebookCatalogRequestFieldApi GetDataRequestFields()
		{
			var result = new FacebookCatalogRequestFieldApi
			{
				Method = Constants.FACEBOOK_CATALOG_API_METHOD,
				RetailerId = this.VariationId,
				Data = new FacebookCatalogDataRequestApi
				{
					AdditionalImageUrls = this.CreateSubImageUrls(),
					Color = this.Color,
					Condition = this.Condition,
					Currency = this.Currency,
					CustomLabel0 = this.CustomLabel0,
					CustomLabel1 = this.CustomLabel1,
					CustomLabel2 = this.CustomLabel2,
					CustomLabel3 = this.CustomLabel3,
					CustomLabel4 = this.CustomLabel4,
					Description = this.Description,
					Gender = this.Gender,
					Gtin = this.Gtin,
					Brand = this.Brand,
					ImageUrl = this.CreateMainImageUrl(),
					Inventory = this.Inventory,
					Name = this.Name,
					Pattern = this.Pattern,
					Price = decimal.ToInt32(this.Price),
					ProductType = this.ProductType,
					RetailerProductGroupId = this.RetailerProductGroupId,
					SalePrice = (this.SalePrice.HasValue)
						? decimal.ToInt32(this.SalePrice.Value)
						: (int?)null,
					SalePriceEndDate = this.SalePriceEndDate,
					SalePriceStartDate = this.SalePriceStartDate,
					Size = this.Size,
					Url = this.Url,
					VendorId = this.VendorId
				}
			};

			return result;
		}
		#endregion

		#region Properties
		/// <summary>Variation id</summary>
		public string VariationId
		{
			get { return StringUtility.ToEmpty(this.DataSource["variation_id"]); }
			set { this.DataSource["variation_id"] = value; }
		}
		/// <summary>Additional image url</summary>
		public string AdditionalImageUrl
		{
			get { return StringUtility.ToEmpty(this.DataSource["additional_image_urls"]); }
			set { this.DataSource["additional_image_urls"] = value; }
		}
		/// <summary>Color</summary>
		public string Color
		{
			get { return StringUtility.ToEmpty(this.DataSource["color"]); }
			set { this.DataSource["color"] = value; }
		}
		/// <summary>Condition</summary>
		public string Condition
		{
			get { return StringUtility.ToEmpty(this.DataSource["condition"]); }
			set { this.DataSource["condition"] = value; }
		}
		/// <summary>Currency</summary>
		public string Currency
		{
			get { return StringUtility.ToEmpty(this.DataSource["currency"]); }
			set { this.DataSource["currency"] = value; }
		}
		/// <summary>Custom label 0</summary>
		public string CustomLabel0
		{
			get { return StringUtility.ToEmpty(this.DataSource["custom_label_0"]); }
			set { this.DataSource["custom_label_0"] = value; }
		}
		/// <summary>Custom label 1</summary>
		public string CustomLabel1
		{
			get { return StringUtility.ToEmpty(this.DataSource["custom_label_1"]); }
			set { this.DataSource["custom_label_1"] = value; }
		}
		/// <summary>Custom label 2</summary>
		public string CustomLabel2
		{
			get { return StringUtility.ToEmpty(this.DataSource["custom_label_2"]); }
			set { this.DataSource["custom_label_2"] = value; }
		}
		/// <summary>Custom label 3</summary>
		public string CustomLabel3
		{
			get { return StringUtility.ToEmpty(this.DataSource["custom_label_3"]); }
			set { this.DataSource["custom_label_3"] = value; }
		}
		/// <summary>Custom label 4</summary>
		public string CustomLabel4
		{
			get { return StringUtility.ToEmpty(this.DataSource["custom_label_4"]); }
			set { this.DataSource["custom_label_4"] = value; }
		}
		/// <summary>Gender</summary>
		public string Gender
		{
			get { return StringUtility.ToEmpty(this.DataSource["gender"]); }
			set { this.DataSource["gender"] = value; }
		}
		/// <summary>Gtin</summary>
		public string Gtin
		{
			get { return StringUtility.ToEmpty(this.DataSource["gtin"]); }
			set { this.DataSource["gtin"] = value; }
		}
		/// <summary>Brand</summary>
		public string Brand
		{
			get { return StringUtility.ToEmpty(this.DataSource["brand"]); }
			set { this.DataSource["brand"] = value; }
		}
		/// <summary>Image url</summary>
		public string ImageUrl
		{
			get { return StringUtility.ToEmpty(this.DataSource["image_url"]); }
			set { this.DataSource["image_url"] = value; }
		}
		/// <summary>Inventory</summary>
		public int? Inventory
		{
			get { return ObjectUtility.TryParseInt(StringUtility.ToEmpty(this.DataSource["inventory"])); }
			set { this.DataSource["inventory"] = value; }
		}
		/// <summary>Name</summary>
		public string Name
		{
			get { return StringUtility.ToEmpty(this.DataSource["name"]); }
			set { this.DataSource["name"] = value; }
		}
		/// <summary>Pattern</summary>
		public string Pattern
		{
			get { return StringUtility.ToEmpty(this.DataSource["pattern"]); }
			set { this.DataSource["pattern"] = value; }
		}
		/// <summary>Price</summary>
		public decimal Price
		{
			get { return ObjectUtility.TryParseDecimal(StringUtility.ToEmpty(this.DataSource["price"]), 0); }
			set { this.DataSource["price"] = value; }
		}
		/// <summary>Product type</summary>
		public string ProductType
		{
			get { return StringUtility.ToEmpty(this.DataSource["product_type"]); }
			set { this.DataSource["product_type"] = value; }
		}
		/// <summary>Retailer product group id</summary>
		public string RetailerProductGroupId
		{
			get { return StringUtility.ToEmpty(this.DataSource["retailer_product_group_id"]); }
			set { this.DataSource["retailer_product_group_id"] = value; }
		}
		/// <summary>Sale price</summary>
		public decimal? SalePrice
		{
			get { return ObjectUtility.TryParseDecimal(StringUtility.ToEmpty(this.DataSource["sale_price"])); }
			set { this.DataSource["sale_price"] = value; }
		}
		/// <summary>Sale price start date</summary>
		public string SalePriceStartDate
		{
			get
			{
				var date = StringUtility.ToNull(this.DataSource["sale_price_start_date"]);
				return (string.IsNullOrEmpty(date) == false)
					? StringUtility.ToDateString(this.DataSource["sale_price_start_date"], Constants.FACEBOOK_CATALOG_API_DATETIME_FORMAT)
					: date;
			}
			set { this.DataSource["sale_price_start_date"] = value; }
		}
		/// <summary>Sale price end date</summary>
		public string SalePriceEndDate
		{
			get
			{
				var date = StringUtility.ToNull(this.DataSource["sale_price_end_date"]);
				return (string.IsNullOrEmpty(date) == false)
					? StringUtility.ToDateString(this.DataSource["sale_price_end_date"], Constants.FACEBOOK_CATALOG_API_DATETIME_FORMAT)
					: date;
			}
			set { this.DataSource["sale_price_end_date"] = value; }
		}
		/// <summary>Size</summary>
		public string Size
		{
			get { return StringUtility.ToEmpty(this.DataSource["size"]); }
			set { this.DataSource["size"] = value; }
		}
		/// <summary>Url</summary>
		public string Url
		{
			get { return StringUtility.ToEmpty(this.DataSource["url"]); }
			set { this.DataSource["url"] = value; }
		}
		/// <summary>Vendor id</summary>
		public string VendorId
		{
			get { return StringUtility.ToEmpty(this.DataSource["vendor_id"]); }
			set { this.DataSource["vendor_id"] = value; }
		}
		/// <summary>Description</summary>
		public string Description
		{
			get { return StringUtility.ToEmpty(this.DataSource["description"]); }
			set { this.DataSource["description"] = value; }
		}
		/// <summary>ソース</summary>
		private Hashtable DataSource { get; set; }
		#endregion
	}
}