/*
=========================================================================================================
  Module      : お気に入り商品マスタモデル (FavoriteModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.Favorite
{
	/// <summary>
	/// お気に入り商品マスタモデル
	/// </summary>
	[Serializable]
	public class FavoriteModel : ModelBase<FavoriteModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public FavoriteModel()
		{
			this.UserId = string.Empty;
			this.ShopId = Constants.CONST_DEFAULT_SHOP_ID;
			this.ProductId = string.Empty;
			this.DelFlg = Constants.FIELD_FAVORITE_DELETE_FLG;
			this.VariationId = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FavoriteModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FavoriteModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ユーザID</summary>
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_FAVORITE_USER_ID]; }
			set { this.DataSource[Constants.FIELD_FAVORITE_USER_ID] = value; }
		}
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_FAVORITE_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_FAVORITE_SHOP_ID] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_FAVORITE_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_FAVORITE_PRODUCT_ID] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_FAVORITE_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_FAVORITE_DEL_FLG] = value; }
		}
		/// <summary>バリエーションID</summary>
		public string VariationId
		{
			get { return (string)this.DataSource[Constants.FIELD_FAVORITE_VARIATION_ID]; }
			set { this.DataSource[Constants.FIELD_FAVORITE_VARIATION_ID] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FAVORITE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_FAVORITE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FAVORITE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_FAVORITE_DATE_CHANGED] = value; }
		}
		#endregion
	}
}
