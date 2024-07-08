/*
=========================================================================================================
  Module      : コーディネートマスタモデル (CoordinateModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using w2.Domain.ContentsLog.Helper;
using w2.Domain.ContentsTag;
using w2.Domain.CoordinateCategory;
using w2.Domain.Product;

namespace w2.Domain.Coordinate
{
	/// <summary>
	/// コーディネートマスタモデル
	/// </summary>
	public partial class CoordinateModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>カテゴリリスト</summary>
		public List<CoordinateCategoryModel> CategoryList { get; set; }
		/// <summary>タグリスト</summary>
		public List<ContentsTagModel> TagList { get; set; }
		/// <summary>商品リスト</summary>
		public List<ProductModel> ProductList { get; set; }
		/// <summary>スタッフイメージパス</summary>
		public string StaffImagePath { get; set; }
		/// <summary>スタッフ名</summary>
		public string StaffName
		{
			get { return (string)this.DataSource[Constants.FIELD_STAFF_STAFF_NAME]; }
			set { this.DataSource[Constants.FIELD_STAFF_STAFF_NAME] = value; }
		}
		/// <summary>身長</summary>
		public int StaffHeight
		{
			get { return (int)this.DataSource[Constants.FIELD_STAFF_STAFF_HEIGHT]; }
			set { this.DataSource[Constants.FIELD_STAFF_STAFF_HEIGHT] = value; }
		}
		/// <summary>プロフィール</summary>
		public string StaffProfile
		{
			get { return (string)this.DataSource[Constants.FIELD_STAFF_STAFF_PROFILE]; }
			set { this.DataSource[Constants.FIELD_STAFF_STAFF_PROFILE] = value; }
		}
		/// <summary>インスタグラムID</summary>
		public string StaffInstagramId
		{
			get { return (string)this.DataSource[Constants.FIELD_STAFF_STAFF_INSTAGRAM_ID]; }
			set { this.DataSource[Constants.FIELD_STAFF_STAFF_INSTAGRAM_ID] = value; }
		}
		/// <summary>リアル店舗名</summary>
		public string RealShopName
		{
			get { return (string)this.DataSource[Constants.FLG_COORDINATE_REAL_SHOP_NAME]; }
			set { this.DataSource[Constants.FLG_COORDINATE_REAL_SHOP_NAME] = value; }
		}
		/// <summary>アイテムNO</summary>
		public int ItemNo
		{
			get { return (int)this.DataSource[Constants.FIELD_COORDINATEITEM_ITEM_NO]; }
			set { this.DataSource[Constants.FIELD_COORDINATEITEM_ITEM_NO] = value; }
		}
		/// <summary>アイテム区分</summary>
		public string ItemKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATEITEM_ITEM_KBN]; }
			set { this.DataSource[Constants.FIELD_COORDINATEITEM_ITEM_KBN] = value; }
		}
		/// <summary>アイテムID</summary>
		public string ItemId
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATEITEM_ITEM_ID]; }
			set { this.DataSource[Constants.FIELD_COORDINATEITEM_ITEM_ID] = value; }
		}
		/// <summary>アイテムID2</summary>
		public string ItemId2
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATEITEM_ITEM_ID2]; }
			set { this.DataSource[Constants.FIELD_COORDINATEITEM_ITEM_ID2] = value; }
		}
		/// <summary>アイテム名</summary>
		public string ItemName
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATEITEM_ITEM_NAME]; }
			set { this.DataSource[Constants.FIELD_COORDINATEITEM_ITEM_NAME] = value; }
		}
		/// <summary>いいね数</summary>
		public int LikeCount
		{
			get { return (int)this.DataSource["like_count"]; }
			set { this.DataSource["like_count"] = value; }
		}
		/// <summary>日時概要</summary>
		public ContentsSummaryData ContentsSummaryData
		{
			get { return (ContentsSummaryData)this.DataSource["contents_summary_data"]; }
			set { this.DataSource["contents_summary_data"] = value; }
		}
		/// <summary>ランキング件数</summary>
		public long RankingCount
		{
			get
			{
				if (this.DataSource["ranking_count"] is int)
				{
					return unchecked((int)this.DataSource["ranking_count"]);
				}

				return (long)this.DataSource["ranking_count"];
			}
			set { this.DataSource["ranking_count"] = value; }
		}
		/// <summary>スタッフが有効か</summary>
		public string StaffValidFlg
		{
			get { return (string)this.DataSource["staff_valid_flg"]; }
			set { this.DataSource["staff_valid_flg"] = value; }
		}
		#endregion
	}
}
