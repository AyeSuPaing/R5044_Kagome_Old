/*
=========================================================================================================
  Module      : 配送種別配送会社マスタモデル (ShopShippingCompanyModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ShopShipping
{
	/// <summary>
	/// 配送種別配送会社マスタモデル
	/// </summary>
	[Serializable]
	public partial class ShopShippingCompanyModel : ModelBase<ShopShippingCompanyModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ShopShippingCompanyModel()
		{
			// TODO:定数を利用するよう書き換えてください。
			this.ShippingKbn = Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS;
			this.DefaultDeliveryCompany = Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_DEFAULT_DELIVERY_COMPANY_INVALID;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ShopShippingCompanyModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ShopShippingCompanyModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>配送種別ID</summary>
		public string ShippingId
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGCOMPANY_SHIPPING_ID]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPINGCOMPANY_SHIPPING_ID] = value; }
		}
		/// <summary>配送区分</summary>
		public string ShippingKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGCOMPANY_SHIPPING_KBN]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPINGCOMPANY_SHIPPING_KBN] = value; }
		}
		/// <summary>配送会社ID</summary>
		public string DeliveryCompanyId
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGCOMPANY_DELIVERY_COMPANY_ID]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPINGCOMPANY_DELIVERY_COMPANY_ID] = value; }
		}
		/// <summary>初期配送会社</summary>
		public string DefaultDeliveryCompany
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGCOMPANY_DEFAULT_DELIVERY_COMPANY]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPINGCOMPANY_DEFAULT_DELIVERY_COMPANY] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SHOPSHIPPINGCOMPANY_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPINGCOMPANY_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SHOPSHIPPINGCOMPANY_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPINGCOMPANY_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGCOMPANY_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPINGCOMPANY_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
