/*
=========================================================================================================
  Module      : 商品ランキングRegisterビューモデル(RegisterViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using w2.App.Common;
using w2.App.Common.Product;
using w2.Cms.Manager.Codes.Util;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ViewModels.Shared;
using w2.Common.Util;
using w2.Domain.ProductRanking;

namespace w2.Cms.Manager.ViewModels.ProductRanking
{
	/// <summary>
	/// 商品ランキングRegisterビューモデル
	/// </summary>
	[Serializable]
	public class RegisterViewModel : ViewModelBase
	{
		/// <summary>ランキング表示件数</summary>
		public const int RANKING_DISPLAY_COUNT = 10;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		public RegisterViewModel(string shopId)
		{
			this.Input = new ProductRankingInput
			{
				ShopId = shopId,
			};
			this.ProductRanking = new SearchResultListViewModel();
			this.Input.ProductRankingItems = new ProductRankingItemInput[0];
			this.TotalTypes = ValueTextForCms.GetValueSelectListItems(Constants.TABLE_PRODUCTRANKING, Constants.FIELD_PRODUCTRANKING_TOTAL_TYPE);
			this.StockKbns = ValueTextForCms.GetValueSelectListItems(Constants.TABLE_PRODUCTRANKING, Constants.FIELD_PRODUCTRANKING_STOCK_KBN);
			this.Brands = ProductBrandUtility.GetProductBrandList().Cast<DataRowView>().Select(
				data => new SelectListItem()
				{
					Text = (string)data[Constants.FIELD_PRODUCTBRAND_BRAND_NAME],
					Value = (string)data[Constants.FIELD_PRODUCTBRAND_BRAND_ID],
				}).ToArray();

			this.Input.ProductRankingItems = Enumerable.Range(0, RANKING_DISPLAY_COUNT).ToList()
				.Select(
					emptyRank => new ProductRankingItemInput()
					{
						RankingId = this.ProductRanking.RankingId,
						ProductId = "",
						Rank = emptyRank + 1,
						IsFixation = false,
						Name = "",
					}).ToArray();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="model">商品ランキング</param>
		/// <param name="categoryIds">カテゴリID（表示用）</param>
		/// <param name="excludeCategoryIds">含まないカテゴリID（表示用）</param>
		public RegisterViewModel(string shopId, ProductRankingModel model, string categoryIds, string excludeCategoryIds) : this(shopId)
		{
			this.ProductRanking = new SearchResultListViewModel(model, categoryIds, excludeCategoryIds);

			this.Input.DataSource = model.DataSource;
			model.Items.ToList().ForEach(
				item =>
				{
					var target = this.Input.ProductRankingItems.First(itemInput => itemInput.Rank == item.Rank);
					target.DataSource = item.DataSource;
					target.IsFixation = (item.FixationFlg == Constants.FLG_PRODUCTRANKINGITEM_FIXATION_FLG_ON);
				});

			this.Input.TotalFrom = new DateTimeInputModel("Input.TotalFrom", model.TotalFrom);
			this.Input.TotalTo = new DateTimeInputModel("Input.TotalTo", model.TotalTo);
		}

		/// <summary>店舗ID</summary>
		public String ShopId { get; set; }
		/// <summary>登録情報</summary>
		public ProductRankingInput Input { get; set; }
		/// <summary>選択肢群 集計タイプ</summary>
		public SelectListItem[] TotalTypes { get; private set; }
		/// <summary>選択肢群 在庫切れ商品</summary>
		public SelectListItem[] StockKbns { get; private set; }
		/// <summary>選択肢群 ブランド</summary>
		public SelectListItem[] Brands { get; private set; }
		/// <summary>商品ランキング情報</summary>
		public SearchResultListViewModel ProductRanking { get; set; }
	}
}