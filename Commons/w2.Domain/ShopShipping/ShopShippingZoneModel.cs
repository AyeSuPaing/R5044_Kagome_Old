/*
=========================================================================================================
  Module      : 店舗配送料地帯マスタモデル (ShopShippingZoneModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ShopShipping
{
	/// <summary>
	/// 店舗配送料地帯マスタモデル
	/// </summary>
	[Serializable]
	public partial class ShopShippingZoneModel : ModelBase<ShopShippingZoneModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ShopShippingZoneModel()
		{
			this.ShippingZoneNo = 0;
			this.SizeXxsShippingPrice = 0;
			this.SizeXsShippingPrice = 0;
			this.SizeSShippingPrice = 0;
			this.SizeMShippingPrice = 0;
			this.SizeLShippingPrice = 0;
			this.SizeXlShippingPrice = 0;
			this.SizeXxlShippingPrice = 0;
			this.ConditionalShippingPriceThreshold = null;
			this.ConditionalShippingPrice = null;
			this.UnavailableShippingAreaFlg
				= Constants.FLG_SHOPSHIPPINGZONE_UNAVAILABLE_SHIPPING_AREA_INVALID;
			this.DelFlg = "0";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ShopShippingZoneModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ShopShippingZoneModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SHOP_ID] = value; }
		}
		/// <summary>配送料設定ID</summary>
		public string ShippingId
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ID]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ID] = value; }
		}
		/// <summary>配送サービス（配送会社）ID</summary>
		public string DeliveryCompanyId
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_DELIVERY_COMPANY_ID]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_DELIVERY_COMPANY_ID] = value; }
		}
		/// <summary>配送料地帯区分</summary>
		public int ShippingZoneNo
		{
			get { return (int)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NO]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NO] = value; }
		}
		/// <summary>地帯名</summary>
		public string ShippingZoneName
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NAME]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NAME] = value; }
		}
		/// <summary>郵便番号</summary>
		public string Zip
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_ZIP]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_ZIP] = value; }
		}
		/// <summary>XXSサイズ商品配送料</summary>
		public decimal SizeXxsShippingPrice
		{
			get { return (decimal)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_XXS_SHIPPING_PRICE]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_XXS_SHIPPING_PRICE] = value; }
		}
		/// <summary>XSサイズ商品配送料</summary>
		public decimal SizeXsShippingPrice
		{
			get { return (decimal)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_XS_SHIPPING_PRICE]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_XS_SHIPPING_PRICE] = value; }
		}
		/// <summary>Sサイズ商品配送料</summary>
		public decimal SizeSShippingPrice
		{
			get { return (decimal)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_S_SHIPPING_PRICE]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_S_SHIPPING_PRICE] = value; }
		}
		/// <summary>Mサイズ商品配送料</summary>
		public decimal SizeMShippingPrice
		{
			get { return (decimal)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_M_SHIPPING_PRICE]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_M_SHIPPING_PRICE] = value; }
		}
		/// <summary>Lサイズ商品配送料</summary>
		public decimal SizeLShippingPrice
		{
			get { return (decimal)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_L_SHIPPING_PRICE]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_L_SHIPPING_PRICE] = value; }
		}
		/// <summary>XLサイズ商品配送料</summary>
		public decimal SizeXlShippingPrice
		{
			get { return (decimal)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_XL_SHIPPING_PRICE]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_XL_SHIPPING_PRICE] = value; }
		}
		/// <summary>XXLサイズ商品配送料</summary>
		public decimal SizeXxlShippingPrice
		{
			get { return (decimal)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_XXL_SHIPPING_PRICE]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_XXL_SHIPPING_PRICE] = value; }
		}
		/// <summary>メール便サイズ商品配送料</summary>
		public decimal SizeMailShippingPrice
		{
			get { return (decimal)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_MAIL_SHIPPING_PRICE]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_MAIL_SHIPPING_PRICE] = value; }
		}
		/// <summary>条件付き配送料閾値</summary>
		public decimal? ConditionalShippingPriceThreshold
		{
			get
			{
				return (this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_CONDITIONAL_SHIPPING_PRICE_THRESHOLD] == DBNull.Value)
					? null
					: (decimal?)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_CONDITIONAL_SHIPPING_PRICE_THRESHOLD];
			}
			set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_CONDITIONAL_SHIPPING_PRICE_THRESHOLD] = value; }
		}
		/// <summary>条件付き配送料</summary>
		public decimal? ConditionalShippingPrice
		{
			get
			{
				return (this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_CONDITIONAL_SHIPPING_PRICE] == DBNull.Value)
					? null
					: (decimal?)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_CONDITIONAL_SHIPPING_PRICE];
			}
			set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_CONDITIONAL_SHIPPING_PRICE] = value; }
		}
		/// <summary>配送不可エリアフラグ</summary>
		public string UnavailableShippingAreaFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_UNAVAILABLE_SHIPPING_AREA_FLG]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_UNAVAILABLE_SHIPPING_AREA_FLG] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
