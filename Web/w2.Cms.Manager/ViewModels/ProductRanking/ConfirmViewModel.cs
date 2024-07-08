/*
=========================================================================================================
  Module      : 商品ランキング詳細画面ビューモデル(ConfirmViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.Util;
using w2.Cms.Manager.Codes;
using w2.Domain.ProductRanking;

namespace w2.Cms.Manager.ViewModels.ProductRanking
{
	/// <summary>
	/// 商品ランキング詳細ビューモデル
	/// </summary>
	[Serializable]
	public class ConfirmViewModel : ViewModelBase
	{
		/// <summary>ランキング表示件数</summary>
		protected const int RANKING_DISPLAY_COUNT = 10;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="productRanking">商品ランキング</param>
		/// <param name="categoryIds">カテゴリID</param>
		/// <param name="excludeCategoryIds">含まないカテゴリID</param>
		public ConfirmViewModel(
			ActionStatus actionStatus,
			ProductRankingModel productRanking = null,
			string categoryIds = "",
			string excludeCategoryIds = "")
		{
			this.ActionStatus = actionStatus;
			SetRanking(actionStatus, productRanking, categoryIds, excludeCategoryIds);
		}

		/// <summary>
		/// 商品ランキング情報セット
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="productRanking">商品ランキング</param>
		/// <param name="categoryIds">カテゴリID</param>
		/// <param name="excludeCategoryIds">含まないカテゴリID</param>
		private void SetRanking(
			ActionStatus actionStatus,
			ProductRankingModel productRanking,
			string categoryIds,
			string excludeCategoryIds)
		{
			this.ProductRanking = new SearchResultListViewModel(
				productRanking,
				categoryIds,
				excludeCategoryIds);

			this.ProductRankingItems = Enumerable.Range(0, RANKING_DISPLAY_COUNT).ToList()
				.Select(
					emptyRank => new ProductRankingItem()
					{
						RankingId = productRanking.RankingId,
						ProductId = "",
						Rank = emptyRank + 1,
						FixationFlg = "",
						Name = "",
						DisplayPrice = "",
						DisplaySpecialPrice = ""
					}).ToArray();
			this.ProductRankingItems
				.Where(item => productRanking.Items.Any(rankingItem => (rankingItem.Rank == item.Rank))).ToList()
				.ForEach(
					rankingItem =>
					{
						var updateRanking =
							productRanking.Items.Where(item => (item.Rank == rankingItem.Rank)).ToArray()[0];
						rankingItem.ProductId = updateRanking.ProductId;
						rankingItem.Rank = updateRanking.Rank;
						rankingItem.Name = updateRanking.Name;
						rankingItem.DisplayPrice = updateRanking.DisplayPrice.ToString();
						rankingItem.DisplaySpecialPrice = updateRanking.DisplaySpecialPrice.ToString();
						rankingItem.FixationFlg = updateRanking.FixationFlg;
					});
			this.Desc = productRanking.Desc1;

			if (actionStatus != ActionStatus.Detail) return;
			this.DateCreated = DateTimeUtility.ToStringForManager(
				productRanking.DateCreated,
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter);
			this.DateChanged = DateTimeUtility.ToStringForManager(
				productRanking.DateChanged,
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter);
			this.LastChanged = productRanking.LastChanged;
		}

		/// <summary>商品ランキング情報</summary>
		public SearchResultListViewModel ProductRanking { get; set; }
		/// <summary>商品ランキング商品情報</summary>
		public ProductRankingItem[] ProductRankingItems { get; set; }
		/// <summary>説明文</summary>
		public string Desc { get; set; }
		/// <summary>作成日</summary>
		public string DateCreated { get; set; }
		/// <summary>更新日</summary>
		public string DateChanged { get; set; }
		/// <summary>最終更新者</summary>
		public string LastChanged { get; set; }
	}

	/// <summary>
	/// ランキング商品アイテムクラス
	/// </summary>
	[Serializable]
	public class ProductRankingItem
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProductRankingItem()
		{
			this.RankingId = "";
			this.ProductId = "";
			this.Rank = 0;
			this.FixationFlg = "";
			this.Name = "";
			this.DisplayPrice = "";
			this.DisplaySpecialPrice = "";
		}

		/// <summary>商品ランキングID</summary>
		public string RankingId { get; set; }
		/// <summary>商品ID</summary>
		public string ProductId { get; set; }
		/// <summary>ランク</summary>
		public int Rank { get; set; }
		/// <summary>固定フラグ</summary>
		public string FixationFlg { get; set; }
		/// <summary>商品名</summary>
		public string Name { get; set; }
		/// <summary>商品表示価格</summary>
		public string DisplayPrice { get; set; }
		/// <summary>商品表示特別価格</summary>
		public string DisplaySpecialPrice { get; set; }
	}
}