/*
=========================================================================================================
  Module      : 商品モデル(ProductModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Domain;

namespace w2.Commerce.Batch.ExportCriteoDatas.Proc
{
	/// <summary>
	/// 商品モデル
	/// </summary>
	[Serializable]
	public partial class ProductModel : ModelBase<ProductModel>
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductModel()
			: base()
		{}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">注文商品情報</param>
		public ProductModel(DataRowView source)
			: this(source.ToHashtable())
		{}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">注文商品情報</param>
		public ProductModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}

		#region プロパティ

		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_PRODUCT_ID] = value; }
		}
		/// <summary>商品名</summary>
		public string Name
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_NAME]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_NAME] = value; }
		}
		/// <summary>商品概要HTML区分</summary>
		public string OutlineKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_OUTLINE_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_OUTLINE_KBN] = value; }
		}
		/// <summary>商品概要</summary>
		public string Outline
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_OUTLINE]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_OUTLINE] = value; }
		}
		/// <summary>表示価格</summary>
		public decimal DisplayPrice
		{
			get { return (decimal)this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_PRICE]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_PRICE] = value; }
		}
		/// <summary>表示価格</summary>
		public decimal? DisplaySpecialPrice
		{
			get { return (decimal?)this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE] = value; }
		}
		/// <summary>表示期間(FROM)</summary>
		public DateTime? DisplayFrom
		{
			get { return (this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_FROM] == DBNull.Value) ? (DateTime?)null : (DateTime)this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_FROM]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_FROM] = value; }
		}
		/// <summary>表示期間(TO)</summary>
		public DateTime? DisplayTo
		{
			get { return (this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_TO] == DBNull.Value) ? (DateTime?)null : (DateTime)this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_TO]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_TO] = value; }
		}
		/// <summary>販売期間(FROM)</summary>
		public DateTime? SellFrom
		{
			get { return (this.DataSource[Constants.FIELD_PRODUCT_SELL_FROM] == DBNull.Value) ? (DateTime?)null : (DateTime)this.DataSource[Constants.FIELD_PRODUCT_SELL_FROM]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_SELL_FROM] = value; }
		}
		/// <summary>販売期間(TO)</summary>
		public DateTime? SellTo
		{
			get { return (this.DataSource[Constants.FIELD_PRODUCT_SELL_TO] == DBNull.Value) ? (DateTime?)null : (DateTime)this.DataSource[Constants.FIELD_PRODUCT_SELL_TO]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_SELL_TO] = value; }
		}
		/// <summary>在庫管理方法</summary>
		public string StockManagementKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN] = value; }
		}
		/// <summary>検索時表示区分</summary>
		public string DisplayKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_KBN] = value; }
		}
		/// <summary>カテゴリID1</summary>
		public string CategoryId1
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID1]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID1] = value; }
		}
		/// <summary>カテゴリID2</summary>
		public string CategoryId2
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID2]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID2] = value; }
		}
		/// <summary>カテゴリID3</summary>
		public string CategoryId3
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID3]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID3] = value; }
		}
		/// <summary>カテゴリID4</summary>
		public string CategoryId4
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID4]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID4] = value; }
		}
		/// <summary>カテゴリID5</summary>
		public string CategoryId5
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID5]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID5] = value; }
		}
		/// <summary>ブランドID1</summary>
		public string BrandId1
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_BRAND_ID1]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_BRAND_ID1] = value; }
		}
		/// <summary>商品画像名ヘッダ</summary>
		public string ImageHead
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_IMAGE_HEAD]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_IMAGE_HEAD] = value; }
		}
		// Marks案件用
		/// <summary>キャッチコピー</summary>
		public string Catchcopy
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_CATCHCOPY]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_CATCHCOPY] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_VALID_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime? DateCreated
		{
			get { return (this.DataSource[Constants.FIELD_PRODUCT_DATE_CREATED] == DBNull.Value) ? (DateTime?)null : (DateTime)this.DataSource[Constants.FIELD_PRODUCT_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime? DateChanged
		{
			get { return (this.DataSource[Constants.FIELD_PRODUCT_DATE_CHANGED] == DBNull.Value) ? (DateTime?)null : (DateTime)this.DataSource[Constants.FIELD_PRODUCT_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
