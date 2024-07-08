/*
=========================================================================================================
  Module      : 商品レビューモデル (ProductReviewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ProductReview
{
	/// <summary>
	/// 商品レビューモデル
	/// </summary>
	public partial class ProductReviewModel : ModelBase<ProductReviewModel>
	{

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductReviewModel()
		{
			this.ShopId = string.Empty;
			this.ProductId = string.Empty;
			this.ReivewNo = 0;
			this.UserId = string.Empty;
			this.NickName = string.Empty;
			this.ReviewRating = string.Empty;
			this.ReviewTitle = string.Empty;
			this.ReviewComment = string.Empty;
			this.OpenFlg = Constants.FLG_PRODUCTREVIEW_OPEN_FLG_INVALID;
			this.CheckedFlg = Constants.FLG_PRODUCTREVIEW_CHECK_FLG_INVALID;
			this.DateOpened = null;
			this.DateChecked = null;
			this.DateCreated = DateTime.Now;
			this.DateChanged = DateTime.Now;
			this.LastChanged = string.Empty;
			this.ReviewRewardPointFlg = Constants.FLG_PRODUCTREVIEW_REVIEW_REWARD_POINT_FLG_INVALID;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductReviewModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductReviewModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTREVIEW_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTREVIEW_SHOP_ID] = value; }
		}
		/// <summary>商品ID </summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTREVIEW_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTREVIEW_PRODUCT_ID] = value; }
		}
		/// <summary>レビュー番号</summary>
		public int ReivewNo
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTREVIEW_REVIEW_NO]; }
			set { this.DataSource[Constants.FIELD_PRODUCTREVIEW_REVIEW_NO] = value; }
		}
		/// <summary>ユーザーID</summary>
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTREVIEW_USER_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTREVIEW_USER_ID] = value; }
		}
		/// <summary>ニックネーム</summary>
		public string NickName
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTREVIEW_NICK_NAME]; }
			set { this.DataSource[Constants.FIELD_PRODUCTREVIEW_NICK_NAME] = value; }
		}
		/// <summary>評価</summary>
		public string ReviewRating
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTREVIEW_REVIEW_RATING]; }
			set { this.DataSource[Constants.FIELD_PRODUCTREVIEW_REVIEW_RATING] = value; }
		}
		/// <summary>タイトル</summary>
		public string ReviewTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTREVIEW_REVIEW_TITLE]; }
			set { this.DataSource[Constants.FIELD_PRODUCTREVIEW_REVIEW_TITLE] = value; }
		}
		/// <summary>コメント</summary>
		public string ReviewComment
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTREVIEW_REVIEW_COMMENT]; }
			set { this.DataSource[Constants.FIELD_PRODUCTREVIEW_REVIEW_COMMENT] = value; }
		}
		/// <summary>公開フラグ</summary>
		public string OpenFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTREVIEW_OPEN_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTREVIEW_OPEN_FLG] = value; }
		}
		/// <summary>管理者チェックフラグ</summary>
		public string CheckedFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTREVIEW_CHECKED_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTREVIEW_CHECKED_FLG] = value; }
		}
		/// <summary>公開日</summary>
		public DateTime? DateOpened
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTREVIEW_DATE_OPENED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTREVIEW_DATE_OPENED] = value; }
		}
		/// <summary>管理者チェック日</summary>
		public object DateChecked
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTREVIEW_DATE_CHECKED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTREVIEW_DATE_CHECKED] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTREVIEW_DATE_CHECKED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTREVIEW_DATE_CHECKED] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTREVIEW_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTREVIEW_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTREVIEW_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTREVIEW_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTREVIEW_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTREVIEW_LAST_CHANGED] = value; }
		}
		/// <summary>レビュー投稿ポイント付与フラグ</summary>
		public string ReviewRewardPointFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTREVIEW_REVIEW_REWARD_POINT_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTREVIEW_REVIEW_REWARD_POINT_FLG] = value; }
		}
		#endregion
	}
}
