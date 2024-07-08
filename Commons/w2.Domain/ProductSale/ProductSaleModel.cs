/*
=========================================================================================================
  Module      : 商品セールマスタモデル (ProductSaleModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ProductSale
{
	/// <summary>
	/// 商品セールマスタモデル
	/// </summary>
	[Serializable]
	public partial class ProductSaleModel : ModelBase<ProductSaleModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductSaleModel()
		{
			this.ProductsaleKbn = Constants.FLG_PRODUCTSALE_PRODUCTSALE_KBN_TIMESALES;
			this.ValidFlg = Constants.FLG_PRODUCTSALE_VALIDITY_VALID;
			this.DelFlg = Constants.FLG_PRODUCTSALE_DELFLG_DELETED;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductSaleModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductSaleModel(Hashtable source = null)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSALE_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSALE_SHOP_ID] = value; }
		}
		/// <summary>商品セールID</summary>
		public string ProductsaleId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID] = value; }
		}
		/// <summary>商品セール区分</summary>
		public string ProductsaleKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSALE_PRODUCTSALE_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSALE_PRODUCTSALE_KBN] = value; }
		}
		/// <summary>商品セール名</summary>
		public string ProductsaleName
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSALE_PRODUCTSALE_NAME]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSALE_PRODUCTSALE_NAME] = value; }
		}
		/// <summary>やみ市パスワード</summary>
		public string ClosedmarketPassword
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSALE_CLOSEDMARKET_PASSWORD]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSALE_CLOSEDMARKET_PASSWORD] = value; }
		}
		/// <summary>開始日時</summary>
		public DateTime DateBgn
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTSALE_DATE_BGN]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSALE_DATE_BGN] = value; }
		}
		/// <summary>終了日時</summary>
		public DateTime DateEnd
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTSALE_DATE_END]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSALE_DATE_END] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSALE_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSALE_VALID_FLG] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSALE_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSALE_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTSALE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSALE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTSALE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSALE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSALE_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSALE_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
