/*
=========================================================================================================
  Module      : 配送料マスタモデル (ShippingDeliveryPostageModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ShopShipping
{
	/// <summary>
	/// 配送料マスタモデル
	/// </summary>
	[Serializable]
	public partial class ShippingDeliveryPostageModel : ModelBase<ShippingDeliveryPostageModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ShippingDeliveryPostageModel()
		{
			this.ShopId = "";
			this.ShippingId = "";
			this.DeliveryCompanyId = "";
			this.ShippingPriceKbn = Constants.FLG_SHIPPING_PRICE_KBN_NONE;
			this.ShippingFreePriceFlg = Constants.FLG_SHOPSHIPPING_SHIPPING_FREE_PRICE_FLG_INVALID;
			this.ShippingFreePrice = null;
			this.AnnounceFreeShippingFlg = Constants.FLG_SHOPSHIPPING_ANNOUNCE_FREE_SHIPPING_FLG_INVALID;
			this.CalculationPluralKbn = Constants.FLG_SHOPSHIPPING_CALCULATION_PLURAL_KBN_SUM_OF_PRODUCT_SHIPPING_PRICE;
			this.PluralShippingPrice = null;
			this.LastChanged = "";
			this.StorePickupFreePriceFlg = Constants.FLG_OFF;
			this.FreeShippingFee = null;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ShippingDeliveryPostageModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ShippingDeliveryPostageModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_SHOP_ID] = value; }
		}
		/// <summary>配送種別ID</summary>
		public string ShippingId
		{
			get { return (string)this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_SHIPPING_ID]; }
			set { this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_SHIPPING_ID] = value; }
		}
		/// <summary>配送会社ID</summary>
		public string DeliveryCompanyId
		{
			get { return (string)this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_DELIVERY_COMPANY_ID]; }
			set { this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_DELIVERY_COMPANY_ID] = value; }
		}
		/// <summary>配送料設定区分</summary>
		public string ShippingPriceKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_SHIPPING_PRICE_KBN]; }
			set { this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_SHIPPING_PRICE_KBN] = value; }
		}
		/// <summary>配送料無料購入金額設定フラグ</summary>
		public string ShippingFreePriceFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_SHIPPING_FREE_PRICE_FLG]; }
			set { this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_SHIPPING_FREE_PRICE_FLG] = value; }
		}
		/// <summary>配送料無料購入金額設定</summary>
		public decimal? ShippingFreePrice
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_SHIPPING_FREE_PRICE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_SHIPPING_FREE_PRICE];
			}
			set { this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_SHIPPING_FREE_PRICE] = value; }
		}
		/// <summary>配送料無料案内表示フラグ</summary>
		public string AnnounceFreeShippingFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_ANNOUNCE_FREE_SHIPPING_FLG]; }
			set { this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_ANNOUNCE_FREE_SHIPPING_FLG] = value; }
		}
		/// <summary>複数商品計算区分</summary>
		public string CalculationPluralKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_CALCULATION_PLURAL_KBN]; }
			set { this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_CALCULATION_PLURAL_KBN] = value; }
		}
		/// <summary>複数商品配送料</summary>
		public decimal? PluralShippingPrice
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_PLURAL_SHIPPING_PRICE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_PLURAL_SHIPPING_PRICE];
			}
			set { this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_PLURAL_SHIPPING_PRICE] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_LAST_CHANGED] = value; }
		}
		/// <summary>店舗受取時配送料無料フラグ</summary>
		public string StorePickupFreePriceFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_STOREPICKUP_FREE_PRICE_FLG]; }
			set { this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_STOREPICKUP_FREE_PRICE_FLG] = value; }
		}
		/// <summary>配送料無料時の請求料金</summary>
		public decimal? FreeShippingFee
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_FREE_SHIPPING_FEE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_FREE_SHIPPING_FEE];
			}
			set { this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_FREE_SHIPPING_FEE] = value; }
		}
		#endregion
	}
}
