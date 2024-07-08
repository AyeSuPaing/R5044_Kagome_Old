/*
=========================================================================================================
  Module      : 商品ランキングInputクラス(ProductRankingInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Text;
using w2.App.Common.Input;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.ViewModels.Shared;
using w2.Common.Util;
using w2.Domain.Product;
using w2.Domain.ProductCategory;
using w2.Domain.ProductRanking;
using Validator = w2.Cms.Manager.Codes.Common.Validator;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	///  商品ランキングInputクラス
	/// </summary>
	public class ProductRankingInput: InputBase<ProductRankingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductRankingInput()
		{
			this.TotalFrom = new DateTimeInputModel("Input.TotalFrom", DateTime.Now.Date);
			this.TotalTo = new DateTimeInputModel("Input.TotalTo", DateTime.Now.Date.AddYears(1));
			this.ProductRankingItems = new ProductRankingItemInput[0];
			this.DateCreated = string.Empty;
			this.DateChanged = string.Empty;
			this.LastChanged = string.Empty;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override ProductRankingModel CreateModel()
		{
			return null;
		}
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル</returns>
		public ProductRankingModel CreateModel(string shopId)
		{
			var model = new ProductRankingModel
			{
				ShopId = shopId,
				RankingId = this.RankingId,
				TotalType = this.TotalType,
				Desc1 = this.Desc1,
				CategoryId = this.CategoryId,
				ExcludeCategoryIds = this.ExcludeCategoryIds,
				BrandId = this.BrandId ?? "",
				BrandKbn = this.BrandKbn ?? Constants.FLG_PRODUCTRANKING_BRAND_KBN_INVALID,
				StockKbn = this.StockKbn ?? Constants.FLG_PRODUCTRANKING_STOCK_KBN_INVALID,
				TotalKbn = this.TotalKbn ?? Constants.FLG_PRODUCTRANKING_TOTAL_KBN_PERIOD,
				TotalFrom = this.TotalFrom.ToDateTime,
				TotalTo = this.TotalTo.ToDateTime,
				ValidFlg = bool.Parse(this.ValidFlg)
					? Constants.FLG_PRODUCTRANKING_VALID_FLG_VALID
					: Constants.FLG_PRODUCTRANKING_VALID_FLG_INVALID,
				TotalDays = this.TotalDays,
				DateCreated = DateTime.Now,
				DateChanged = DateTime.Now,
				LastChanged = this.LastChanged,
				Items = this.ProductRankingItems.Select(item => item.CreateModel(shopId)).ToArray(),
			};
			return model;
		}

		/// <summary>
		/// 情報不足の部分を初期化する
		/// </summary>
		private void SetData()
		{
			if ((this.TotalType == Constants.FLG_PRODUCTRANKING_TOTAL_TYPE_MANUAL)
				&& string.IsNullOrEmpty(this.StockKbn))
			{
				this.StockKbn = Constants.FLG_PRODUCTRANKING_STOCK_KBN_INVALID;
			}

			if (string.IsNullOrEmpty(this.TotalDays)) this.TotalDays = "1";
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <returns>エラーメッセージ</returns>
		public string Validate(ActionStatus actionStatus)
		{
			this.SetData();

			var errorMessage = new StringBuilder();
			errorMessage.Append(CheckInputProductRankingData(actionStatus));
			errorMessage.Append(CheckInputProductRankingItemData());
			if (this.BrandKbn == Constants.FLG_PRODUCTRANKING_BRAND_KBN_VALID)
			{
				errorMessage.Append(Validator.CheckNecessaryError("ブランドID", this.BrandId));
			}

			// 集計タイプが自動の場合、集計期間もチェックする
			if (this.TotalType == Constants.FLG_PRODUCTRANKING_TOTAL_TYPE_AUTO)
			{

				// Check date error for aggregation period (from)
				var totalFromError = this.TotalFrom.Validate("集計期間(FROM)");
				errorMessage.Append(
					(string.IsNullOrEmpty(totalFromError) == false)
						? string.Format("{0}<br />", totalFromError)
						: string.Empty);

				// Check date error for aggregation period (to)
				var totalToError = this.TotalTo.Validate("集計期間(TO)");
				errorMessage.Append(
					(string.IsNullOrEmpty(totalToError) == false)
						? string.Format("{0}<br />", totalToError)
						: string.Empty);

				// Check date range for aggregation period (from - to)
				if (string.IsNullOrEmpty(totalFromError) && string.IsNullOrEmpty(totalToError))
				{
					if ((this.TotalTo != null) && Validator.CheckDateRange(
						this.TotalFrom.DateTimeString,
						this.TotalTo.DateTimeString) == false)
					{
						errorMessage.Append(WebMessages.InputCheckDateRange.Replace("@@ 1 @@", "表示日付"));
					}
				}
			}

			return errorMessage.ToString();
		}

		/// <summary>
		/// 商品カテゴリ存在チェック
		/// </summary>
		/// <param name="categoryIds">カテゴリID</param>
		/// <returns>エラーメッセージ</returns>
		private string CheckExsistProductCategory(string categoryIds)
		{
			var categoryIdList = categoryIds.Replace("\r\n", "\n").Split('\n').Where(c => string.IsNullOrEmpty(c) == false).ToArray();
			var categoryList = new ProductCategoryService().GetAll();

			var errorMessage = string.Join(
				"",
				categoryIdList
					.Where(categoryId => (categoryList.Any(c => c.CategoryId == categoryId) == false))
					.Select(categoryId => WebMessages.ProductRankingProductCategoryUnFound.Replace("@@ 1 @@", categoryId)));

			return errorMessage.ToString();
		}

		/// <summary>
		///  商品ランキング情報の入力チェック
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <returns>エラーメッセージ</returns>
		private string CheckInputProductRankingData(ActionStatus actionStatus)
		{
			var errorMessage = Validator.Validate(
				(actionStatus == ActionStatus.Insert) ? "ProductRankingRegist" :
				(actionStatus == ActionStatus.Update) ? "ProductRankingModity" : "",
				this.DataSource);

			if (string.IsNullOrEmpty(errorMessage))
			{
				errorMessage += CheckExsistProductCategory(this.CategoryId);
				errorMessage += CheckExsistProductCategory(this.ExcludeCategoryIds);
				errorMessage += CheckProductCategoryIdConflict(this.CategoryId, this.ExcludeCategoryIds);
			}

			return errorMessage;
		}

		/// <summary>
		/// カテゴリID指定とカテゴリID除外指定に同一のカテゴリIDが指定されていないかチェック
		/// </summary>
		/// <param name="categoryId">指定カテゴリID</param>
		/// <param name="excludedCategoryIds">除外カテゴリID</param>
		/// <returns>エラーメッセージ</returns>
		private string CheckProductCategoryIdConflict(string categoryId, string excludedCategoryIds)
		{
			var categoryIds = excludedCategoryIds.Replace("\r\n", "\n").Split('\n')
				.Where(c => string.IsNullOrEmpty(c) == false)
				.ToArray();

			var errorMessage = (categoryIds.All(ci => (ci.ToString() != categoryId)))
				? ""
				: WebMessages.ProductRankingProductCategoryConflictError;
			return errorMessage;
		}

		/// <summary>
		/// 商品ランキングアイテムの入力チェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		private string CheckInputProductRankingItemData()
		{
			// 空値行を除いた商品ランキングアイテムを取得
			var productRankingItemTmps = this.ProductRankingItems
				.Where(item => (string.IsNullOrEmpty(item.ProductId) == false)).ToArray();

			var errorMessages = new StringBuilder();

			foreach (var productRankingItem in productRankingItemTmps)
			{
				// 入力チェック
				var errors = Validator.Validate("ProductRankingItemRegist", productRankingItem.DataSource);
				errorMessages.Append(
					string.Join(
						"\r\n",
						errors.Replace("@@ 1 @@", GetProductRankingErrorPrefix(productRankingItem.Rank))));

				// 同一商品有無チェック
				foreach (var productRankingItemTmp in productRankingItemTmps)
				{
					// 同じ商品IDの場合はNG
					if ((productRankingItem != productRankingItemTmp)
						&& (productRankingItem.ProductId).ToLower() == (productRankingItemTmp.ProductId).ToLower())
					{
						errorMessages
							.Append(GetProductRankingErrorPrefix(productRankingItem.Rank))
							.Append(WebMessages.ProductRankingProductIdDuplicationError)
							.Replace("@@ 1 @@", productRankingItem.ProductId);
					}
				}

				// 商品存在または有効性チェック
				var products = new ProductService().Get(this.ShopId, productRankingItem.ProductId);
				if (products == null)
				{
					errorMessages
						.Append(GetProductRankingErrorPrefix(productRankingItem.Rank))
						.Append(WebMessages.ProductRankingProductIdUnfind)
						.Replace("@@ 1 @@", productRankingItem.ProductId);
				}
				else if (products.ValidFlg == Constants.FLG_PRODUCT_VALID_FLG_INVALID)
				{
					errorMessages
						.Append(GetProductRankingErrorPrefix(productRankingItem.Rank))
						.Append(WebMessages.ProductInvalid)
						.Replace("@@ 1 @@", productRankingItem.ProductId);
				}
				else
				{
					productRankingItem.DisplayPrice = products.DisplayPrice;
					productRankingItem.DisplaySpecialPrice = products.DisplaySpecialPrice;
					productRankingItem.Name = products.Name;
				}
			}
			return errorMessages.ToString();
		}

		/// <summary>
		/// ランキングのエラー接頭辞取得
		/// </summary>
		/// <param name="rank">ランク</param>
		/// <returns>ランキングのエラー接頭辞</returns>
		protected string GetProductRankingErrorPrefix(int rank)
		{
			return WebMessages.ProductRankingErrorPrefix.Replace("@@ 1 @@", rank.ToString());
		}

		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKING_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_SHOP_ID] = value; }
		}
		/// <summary>商品ランキングID</summary>
		public string RankingId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKING_RANKING_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_RANKING_ID] = value; }
		}
		/// <summary>集計タイプ</summary>
		public string TotalType
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKING_TOTAL_TYPE]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_TOTAL_TYPE] = value; }
		}
		/// <summary>集計期間区分</summary>
		public string TotalKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKING_TOTAL_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_TOTAL_KBN] = value; }
		}
		/// <summary>集計日数</summary>
		public string TotalDays
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKING_TOTAL_DAYS]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_TOTAL_DAYS] = value; }
		}
		/// <summary>カテゴリID</summary>
		public string CategoryId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKING_CATEGORY_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_CATEGORY_ID] = value; }
		}
		/// <summary>説明</summary>
		public string Desc1
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKING_DESC1]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_DESC1] = value; }
		}
		/// <summary>在庫切れ商品</summary>
		public string StockKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKING_STOCK_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_STOCK_KBN] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKING_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_VALID_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public string DateCreated
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKING_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public string DateChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_LAST_CHANGED] = value; }
		}
		/// <summary>ブランド指定</summary>
		public string BrandKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKING_BRAND_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_BRAND_KBN] = value; }
		}
		/// <summary>ブランドID</summary>
		public string BrandId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKING_BRAND_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_BRAND_ID] = value; }
		}
		/// <summary>カテゴリ除外ID</summary>
		public string ExcludeCategoryIds
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKING_EXCLUDE_CATEGORY_IDS]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_EXCLUDE_CATEGORY_IDS] = value; }
		}
		/// <summary>有効か</summary>
		public bool IsValid
		{
			get { return (this.ValidFlg == Constants.FLG_PRODUCTRANKING_VALID_FLG_VALID); }
		}
		/// <summary>ブランド指定するか</summary>
		public bool IsBrand
		{
			get { return (this.BrandKbn == Constants.FLG_PRODUCTRANKING_BRAND_KBN_VALID); }
		}
		/// <summary>集計期間指定か</summary>
		public bool IsTotalKbnPeriod
		{
			get
			{
				return (this.TotalKbn == Constants.FLG_PRODUCTRANKING_TOTAL_KBN_PERIOD)
					|| string.IsNullOrEmpty(this.TotalKbn);
			}
		}
		/// <summary>期間指定（From）</summary>
		public DateTimeInputModel TotalFrom { get; set; }
		/// <summary>期間指定（To）</summary>
		public DateTimeInputModel TotalTo { get; set; }
		/// <summary>商品ランキング設定商品</summary>
		public ProductRankingItemInput[] ProductRankingItems { get; set; }
		#endregion
	}

	/// <summary>
	///  商品ランキングItemInputクラス
	/// </summary>
	public class ProductRankingItemInput
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProductRankingItemInput()
		{
			this.DataSource = new Hashtable();
			this.IsFixation = false;
			this.DisplayPrice = 0;
			this.Rank = 0;
			this.DateCreated = DateTime.Now;
			this.DateChanged = DateTime.Now;
		}
			
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル</returns>
		public ProductRankingItemModel CreateModel(string shopId)
		{
			var model = new ProductRankingItemModel
			{
				ShopId = shopId,
				RankingId = this.RankingId,
				ProductId = this.ProductId,
				Rank = this.Rank,
				Name = this.Name,
				FixationFlg =
					this.IsFixation
						? Constants.FLG_PRODUCTRANKINGITEM_FIXATION_FLG_ON
						: Constants.FLG_PRODUCTRANKINGITEM_FIXATION_FLG_OFF,
				DisplayPrice = this.DisplayPrice,
				DisplaySpecialPrice = this.DisplaySpecialPrice,
				DateCreated = DateTime.Now,
				DateChanged = DateTime.Now,
				LastChanged = this.LastChanged,
			};
			return model;
		}

		/// <summary>商品ランキングID</summary>
		public string RankingId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKINGITEM_RANKING_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKINGITEM_RANKING_ID] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKINGITEM_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKINGITEM_PRODUCT_ID] = value; }
		}
		/// <summary>ランク</summary>
		public int Rank
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTRANKINGITEM_RANK]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKINGITEM_RANK] = value; }
		}
		/// <summary>商品名</summary>
		public string Name
		{
			get { return (string)this.DataSource["name"]; }
			set { this.DataSource["name"] = value; }
		}
		/// <summary>商品表示価格</summary>
		public decimal DisplayPrice
		{
			get { return (decimal)this.DataSource["display_price"]; }
			set { this.DataSource["display_price"] = value; }
		}
		/// <summary>商品表示特別価格</summary>
		public decimal? DisplaySpecialPrice
		{
			get
			{
				return this.DataSource["display_special_price"] == DBNull.Value
					? null
					: (decimal?)this.DataSource["display_special_price"];
			}
			set { this.DataSource["display_special_price"] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTRANKINGITEM_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKINGITEM_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTRANKINGITEM_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKINGITEM_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKINGITEM_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKINGITEM_LAST_CHANGED] = value; }
		}
		/// <summary>ソース</summary>
		public Hashtable DataSource { get; set; }
		/// <summary>固定か</summary>
		public bool IsFixation { get; set; }
	}
}