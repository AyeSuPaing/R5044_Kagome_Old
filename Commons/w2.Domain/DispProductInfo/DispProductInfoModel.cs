/*
=========================================================================================================
  Module      : 商品表示情報マスタモデル (DispProductInfoModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.DispProductInfo
{
	/// <summary>
	/// 商品表示情報マスタモデル
	/// </summary>
	[Serializable]
	public partial class DispProductInfoModel : ModelBase<DispProductInfoModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public DispProductInfoModel()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public DispProductInfoModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public DispProductInfoModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_DISPPRODUCTINFO_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_DISPPRODUCTINFO_SHOP_ID] = value; }
		}
		/// <summary>データ区分</summary>
		public string DataKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_DISPPRODUCTINFO_DATA_KBN]; }
			set { this.DataSource[Constants.FIELD_DISPPRODUCTINFO_DATA_KBN] = value; }
		}
		/// <summary>表示順</summary>
		public int DisplayOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_DISPPRODUCTINFO_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_DISPPRODUCTINFO_DISPLAY_ORDER] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_DISPPRODUCTINFO_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_DISPPRODUCTINFO_PRODUCT_ID] = value; }
		}
		/// <summary>区分1</summary>
		public string Kbn1
		{
			get { return (string)this.DataSource[Constants.FIELD_DISPPRODUCTINFO_KBN1]; }
			set { this.DataSource[Constants.FIELD_DISPPRODUCTINFO_KBN1] = value; }
		}
		/// <summary>区分2</summary>
		public string Kbn2
		{
			get { return (string)this.DataSource[Constants.FIELD_DISPPRODUCTINFO_KBN2]; }
			set { this.DataSource[Constants.FIELD_DISPPRODUCTINFO_KBN2] = value; }
		}
		/// <summary>区分3</summary>
		public string Kbn3
		{
			get { return (string)this.DataSource[Constants.FIELD_DISPPRODUCTINFO_KBN3]; }
			set { this.DataSource[Constants.FIELD_DISPPRODUCTINFO_KBN3] = value; }
		}
		/// <summary>区分4</summary>
		public string Kbn4
		{
			get { return (string)this.DataSource[Constants.FIELD_DISPPRODUCTINFO_KBN4]; }
			set { this.DataSource[Constants.FIELD_DISPPRODUCTINFO_KBN4] = value; }
		}
		/// <summary>区分5</summary>
		public string Kbn5
		{
			get { return (string)this.DataSource[Constants.FIELD_DISPPRODUCTINFO_KBN5]; }
			set { this.DataSource[Constants.FIELD_DISPPRODUCTINFO_KBN5] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_DISPPRODUCTINFO_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_DISPPRODUCTINFO_DATE_CREATED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_DISPPRODUCTINFO_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_DISPPRODUCTINFO_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
