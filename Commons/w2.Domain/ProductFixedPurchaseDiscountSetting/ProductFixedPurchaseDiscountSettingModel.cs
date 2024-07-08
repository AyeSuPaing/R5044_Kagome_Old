/*
=========================================================================================================
  Module      : 商品定期購入割引設定モデル (ProductFixedPurchaseDiscountSettingModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ProductFixedPurchaseDiscountSetting
{
	/// <summary>
	/// 商品定期購入割引設定モデル
	/// </summary>
	[Serializable]
	public partial class ProductFixedPurchaseDiscountSettingModel : ModelBase<ProductFixedPurchaseDiscountSettingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductFixedPurchaseDiscountSettingModel()
		{
			this.OrderCountMoreThanFlg = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_ORDER_COUNT_MORE_THAN_FLG_INVALID;
			this.DiscountValue = null;
			this.DiscountType = null;
			this.PointValue = null;
			this.PointType = null;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductFixedPurchaseDiscountSettingModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductFixedPurchaseDiscountSettingModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_SHOP_ID] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_PRODUCT_ID] = value; }
		}
		/// <summary>購入回数</summary>
		public int OrderCount
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_ORDER_COUNT]; }
			set { this.DataSource[Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_ORDER_COUNT] = value; }
		}
		/// <summary>購入回数以降フラグ</summary>
		public string OrderCountMoreThanFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_ORDER_COUNT_MORE_THAN_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_ORDER_COUNT_MORE_THAN_FLG] = value; }
		}
		/// <summary>商品個数</summary>
		public int ProductCount
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_PRODUCT_COUNT]; }
			set { this.DataSource[Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_PRODUCT_COUNT] = value; }
		}
		/// <summary>値引</summary>
		public decimal? DiscountValue
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_VALUE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_VALUE];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_VALUE] = value; }
		}
		/// <summary>値引タイプ</summary>
		public string DiscountType
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE] = value; }
		}
		/// <summary>付与ポイント</summary>
		public decimal? PointValue
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_POINT_VALUE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_POINT_VALUE];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_POINT_VALUE] = value; }
		}
		/// <summary>付与ポイントタイプ</summary>
		public string PointType
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_POINT_TYPE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_POINT_TYPE];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_POINT_TYPE] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
