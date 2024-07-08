/*
=========================================================================================================
  Module      : 商品レビューモデル (ProductReviewModel_Ex.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.ProductReview
{
	/// <summary>
	/// 商品レビューモデル
	/// </summary>
	public partial class ProductReviewModel
	{

		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>公開が有効か</summary>
		public bool IsOpen
		{
			get { return (this.OpenFlg == Constants.FLG_PRODUCTREVIEW_OPEN_FLG_VALID); }
		}
		/// <summary>管理者チェック済み判定</summary>
		public bool IsChecked
		{
			get { return (this.CheckedFlg == Constants.FLG_PRODUCTREVIEW_CHECK_FLG_INVALID); }
		}
		/// <summary>レビュー投稿ポイント付与済み判定</summary>
		public bool IsRewardedReviewPoint
		{
			get { return (this.ReviewRewardPointFlg == Constants.FLG_PRODUCTREVIEW_REVIEW_REWARD_POINT_FLG_VALID); }
		}
		#endregion
	}
}
