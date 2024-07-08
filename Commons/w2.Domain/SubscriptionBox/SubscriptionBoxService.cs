/*
=========================================================================================================
  Module      : 頒布会サービス (SubscriptionBoxService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using System.Data;
using w2.Common.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using w2.Common.Util;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.Product;
using w2.Common.Logger;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Domain.SubscriptionBox
{
	/// <summary>
	/// 頒布会サービス
	/// </summary>
	public class SubscriptionBoxService : ServiceBase, ISubscriptionBoxService
	{
		/// <summary>
		/// 頒布会コースID重複チェック
		/// </summary>
		/// <param name="courseId">頒布会コースID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>頒布会リスト</returns>
		public bool CheckDuplicationCourseId(string courseId, SqlAccessor accessor = null)
		{
			using (var repository = new SubscriptionBoxRepository(accessor))
			{
				return repository.CheckDuplicationCourseId(courseId);
			}
		}

		/// <summary>
		/// すべて取得
		/// </summary>
		/// <param name="accessor">アクセサ</param>
		/// <returns>頒布会リスト</returns>
		public SubscriptionBoxModel[] GetAll(SqlAccessor accessor = null)
		{
			using (var repository = new SubscriptionBoxRepository(accessor))
			{
				var models = repository.GetAll();
				return models;
			}
		}

		/// <summary>
		/// 有効なものをすべて取得（子アイテム郡すべて含む）
		/// </summary>
		/// <remarks>
		/// I/O負荷高いのでキャッシュリフレッシュ専用
		/// </remarks>
		/// <returns>頒布会</returns>
		public SubscriptionBoxModel[] GetValidOnesWithChildren()
		{
			using (var repository = new SubscriptionBoxRepository())
			{
				var models = repository.GetValidAll();
				var items = repository.GetAllValidItems();
				var defualtItems = repository.GetAllValidDefaultItems();

				foreach (var model in models)
				{
					model.DefaultOrderProducts = defualtItems
						.Where(di => (di.SubscriptionBoxCourseId == model.CourseId))
						.ToArray();

					model.SelectableProducts = items
						.Where(i => (i.SubscriptionBoxCourseId == model.CourseId))
						.ToArray();
				}

				return models;
			}
		}

		/// <summary>
		/// コースIDで頒布会を取得
		/// </summary>
		/// <param name="courseId">コースID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>頒布会</returns>
		public SubscriptionBoxModel GetByCourseId(string courseId, SqlAccessor accessor = null)
		{
			using (var repository = new SubscriptionBoxRepository(accessor))
			{
				var model = repository.Get(courseId);
				return model;
			}
		}

		#region +GetByTaxCategoryId 商品税率カテゴリと紐づけられた頒布会を取得
		/// <summary>
		/// 商品税率カテゴリと紐づけられた頒布会を取得
		/// </summary>
		/// <param name="taxCategoryId">商品税率カテゴリID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>頒布会情報</returns>
		public SubscriptionBoxModel GetByTaxCategoryId(string taxCategoryId, SqlAccessor accessor = null)
		{
			using (var repository = new SubscriptionBoxRepository(accessor))
			{
				var model = repository.GetByTaxCategoryId(taxCategoryId);
				return model;
			}
		}
		#endregion

		#region +GetCourseList
		/// <summary>
		/// 頒布会コースリストを取得
		/// </summary>
		/// <param name="displayQuantity">表示件数</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>頒布会</returns>
		public SubscriptionBoxModel[] GetCourseList(int displayQuantity, SqlAccessor accessor = null)
		{
			using (var repository = new SubscriptionBoxRepository(accessor))
			{
				var model = repository.GetCourseList(displayQuantity);
				return model;
			}
		}
		#endregion

		/// <summary>
		/// 頒布会コース更新
		/// </summary>
		/// <param name="model">頒布会モデル</param>
		/// <returns>成否</returns>
		public bool Update(SubscriptionBoxModel model)
		{
			using (var accessor = new SqlAccessor())
			using (var repository = new SubscriptionBoxRepository(accessor))
			{
				// トランザクション開始
				accessor.OpenConnection();
				accessor.BeginTransaction();

				repository.Update(model);
				repository.DeleteItems(model.CourseId);
				repository.DeleteDefaultItems(model.CourseId);

				foreach (var item in model.SelectableProducts)
				{
					repository.Insert(item);
				}

				foreach (var item in model.DefaultOrderProducts)
				{
					item.CorrectTerm();
					repository.Insert(item);
				}

				accessor.CommitTransaction();
				return true;
			}
		}

		/// <summary>
		/// 頒布会コースを削除
		/// </summary>
		/// <param name="courseId">コースID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>影響件数</returns>
		public int Delete(string courseId, SqlAccessor accessor = null)
		{
			using (var repository = new SubscriptionBoxRepository(accessor))
			{
				var rowCount = repository.Delete(courseId);
				return rowCount;
			}
		}

		/// <summary>
		/// インサート
		/// </summary>
		/// <param name="model">頒布会モデル</param>
		public bool Insert(SubscriptionBoxModel model)
		{
			using (var accessor = new SqlAccessor())
			using (var repository = new SubscriptionBoxRepository(accessor))
			{
				// トランザクション開始
				accessor.OpenConnection();
				accessor.BeginTransaction();
				repository.Insert(model);

				// TODO: Delete all items.

				foreach (var item in model.SelectableProducts)
				{
					repository.Insert(item);
				}

				foreach (var item in model.DefaultOrderProducts)
				{
					item.CorrectTerm();
					repository.Insert(item);
				}

				accessor.CommitTransaction();
				return true;
			}
		}

		/// <summary>
		/// Get Count
		/// </summary>
		/// <param name="htSqlParam">SQL Parameter</param>
		/// <returns></returns>
		public int? GetCount(Hashtable htSqlParam)
		{
			using (var repository = new SubscriptionBoxRepository())
			{
				var models = repository.GetCount(htSqlParam);
				return models;
			}
		}

		/// <summary>
		/// Get Valid Subscription Box
		/// </summary>
		/// <param name="accessor">sql accessor</param>
		/// <returns>models</returns>
		public SubscriptionBoxModel[] GetValidSubscriptionBox(SqlAccessor accessor = null)
		{
			using (var repository = new SubscriptionBoxRepository(accessor))
			{
				var models = repository.GetValidSubscriptionBox();
				return models;
			}
		}

		/// <summary>
		/// 頒布会検索（DataView）
		/// </summary>
		/// <param name="htSqlParam">検索パラメタ</param>
		/// <returns>検索結果</returns>
		public DataView SearchSubscriptionBoxesAtDataView(Hashtable htSqlParam)
		{
			// HACK: Without knowing why is this method returns DataView...
			using (var repository = new SubscriptionBoxRepository())
			{
				var models = repository.SearchSubscriptionBoxesAtDataView(htSqlParam);
				return models;
			}
		}

		/// <summary>
		/// 頒布会注文（あるいは頒布会定期）チェック
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>True：頒布会注文（あるいは頒布会定期）、False：頒布会注文（あるいは頒布会定期）以外</returns>
		public bool IsSubscriptionBoxOrderOrFixedPurchase(Hashtable condition)
		{
			using (var repository = new SubscriptionBoxRepository())
			{
				var result = repository.IsSubscriptionBoxOrderOrFixedPurchase(condition);
				return result;
			}
		}

		/// <summary>
		/// Get Subscription Box Course Id Of Order Or FixedPurchase
		/// </summary>
		/// <param name="htSqlParam">SQL Parameter</param>
		/// <returns></returns>
		public string GetSubscriptionBoxCourseIdOfOrderOrFixedPurchase(Hashtable htSqlParam)
		{
			using (var repository = new SubscriptionBoxRepository())
			{
				var models = repository.GetSubscriptionBoxCourseIdOfOrderOrFixedPurchase(htSqlParam);
				return models;
			}
		}

		/// <summary>
		/// Get Product Belong To subscription box Ids
		/// </summary>
		/// <param name="productId"></param>
		/// <param name="variationId"></param>
		/// <param name="shopId"></param>
		/// <returns></returns>
		public SubscriptionBoxModel[] GetAvailableSubscriptionBoxesByProductId(string productId, string variationId, string shopId)
		{
			using (var repository = new SubscriptionBoxRepository())
			{
				var models = repository.GetProductBelongToHanpukaiIDs(shopId, productId, variationId);
				return models;
			}
		}

		/// <summary>
		/// Get NameDisplay 
		/// </summary>
		/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
		/// <returns>頒布会表示名</returns>
		public string GetDisplayName(string subscriptionBoxCourseId)
		{
			using (var repository = new SubscriptionBoxRepository())
			{
				var models = repository.GetNameDisplay(subscriptionBoxCourseId);
				return models;
			}
		}

		/// <summary>
		/// 頒布会選択可能商品を取得
		/// </summary>
		/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
		/// <param name="date">日付</param>
		/// <returns>頒布会選択可能商品</returns>
		public SubscriptionBoxItemModel[] GetSubscriptionBoxItemAvailable(string subscriptionBoxCourseId, string date)
		{
			using (var repository = new SubscriptionBoxRepository())
			{
				var models = repository.GetSubscriptionBoxItemAvailable(subscriptionBoxCourseId, date);
				return models;
			}
		}

		/// <summary>
		/// Get Number of display
		/// </summary>
		/// <param name="htSqlParam"></param>
		/// <returns></returns>
		public int? GetNumberOfDisplay(Hashtable htSqlParam)
		{
			using (var repository = new SubscriptionBoxRepository())
			{
				var models = repository.GetNumberOfDisplay(htSqlParam);
				return models;
			}
		}

		/// <summary>
		/// Get List Products For Count Total Money
		/// </summary>
		/// <param name="htSqlParam">SQL Parameter</param>
		/// <returns></returns>
		public DataView GetListProduct(Hashtable htSqlParam)
		{
			using (var repository = new SubscriptionBoxRepository())
			{
				var models = repository.GetListProduct(htSqlParam);
				return models;
			}
		}

		// HACK: I was trying to write properly xml document but I couldn't understand how this method works.
		/// <summary>
		/// get next date
		/// </summary>
		/// <param name="htSqlParam">SQL Parameter</param>
		/// <returns>result</returns>
		public DateTime? GetNextDate(Hashtable htSqlParam)
		{
			using (var repository = new SubscriptionBoxRepository())
			{
				var models = repository.GetNextDate(htSqlParam);
				return models;
			}
		}

		#region +GetDisplayItems
		/// <summary>
		/// 頒布会次回配送商品の取得
		/// </summary>
		/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
		/// <returns>表示内容</returns>
		public SubscriptionBoxDefaultItemModel[] GetDisplayItems(string subscriptionBoxCourseId)
		{
			var subscriptionBox = GetByCourseId(subscriptionBoxCourseId);

			if ((subscriptionBox == null) || (subscriptionBox.IsValid == false))
			{
				return null;
			}

			using (var repository = new SubscriptionBoxRepository())
			{
				var defaultItem = repository.GetDisplayItems(subscriptionBoxCourseId);
				return defaultItem;
			}
		}
		#endregion

		#region GetFixedPurchaseNextProduct
		/// <summary>
		/// 頒布会次回配送商品の取得
		/// </summary>
		/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
		/// <param name="fixedPurchaseId">定期台帳ID</param>
		/// <param name="memberRankId">メンバーランクID</param>
		/// <param name="dateDelivery">次回配送日</param>
		/// <param name="subscriptionBoxOrderCount">回数</param>
		/// <param name="shipping">定期購入配送先情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>頒布会商品更新結果クラス</returns>
		public SubscriptionBoxGetNextProductsResult GetFixedPurchaseNextProduct(
			string subscriptionBoxCourseId,
			string fixedPurchaseId,
			string memberRankId,
			DateTime dateDelivery,
			int subscriptionBoxOrderCount,
			FixedPurchaseShippingContainer shipping,
			SqlAccessor accessor = null)
		{
			var subscriptionBox = GetByCourseId(subscriptionBoxCourseId, accessor);
			var result = new SubscriptionBoxGetNextProductsResult();

			if ((subscriptionBox == null) || (subscriptionBox.IsValid == false))
			{
				return result;
			}

			var itemNo = 1;
			var productService = new ProductService();
			var nextProducts = new List<FixedPurchaseItemModel>();
			var isPartialFailure = false;
			switch (subscriptionBox.OrderItemDeterminationType)
			{
				case Constants.FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_NUMBER_TIME:
					var maxCount = subscriptionBox.DefaultOrderProducts.Max(defaultItem => defaultItem.Count).Value;
					if ((subscriptionBoxOrderCount > maxCount)
						&& (subscriptionBox.IsAutoRenewal == false)
						&& (subscriptionBox.IsIndefinitePeriod == false))
					{
						result.Result = SubscriptionBoxGetNextProductsResult.ResultTypes.Cancel;
						return result;
					}

					var currentCount = ((subscriptionBoxOrderCount > maxCount)
						&& (subscriptionBox.IsAutoRenewal))
						? 1
						: subscriptionBoxOrderCount;

					// 規定回数完了かつ、最終商品の無限繰り返しが有効か
					if ((subscriptionBoxOrderCount > maxCount) && subscriptionBox.IsIndefinitePeriod)
					{
						currentCount = subscriptionBoxOrderCount - 1;
					}

					var defaultOrderProducts = subscriptionBox.DefaultOrderProducts
						.Where(defaultItem => defaultItem.Count == currentCount).ToArray();

					// 前回の商品を引き継ぐ判定
					if ((defaultOrderProducts.Length > 0) && (string.IsNullOrEmpty(defaultOrderProducts.First().ProductId) == false))
					{
						#region 引き継ぎなし
						result.HasNecessaryFlg = HasNecessaryProducts(defaultOrderProducts);
						if (result.HasNecessaryFlg
							&& (subscriptionBox.FirstSelectableFlg == Constants.FLG_SUBSCRIPTIONBOX_FIRST_SELECTABLE_PAGE_FALSE))
						{
							defaultOrderProducts = defaultOrderProducts
								.Where(defaultOrderProduct => defaultOrderProduct.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_VALID)
								.ToArray();
						}

						foreach (var defaultProduct in defaultOrderProducts)
						{
							var productItem = subscriptionBox.SelectableProducts.First(
									product => ((product.ProductId == defaultProduct.ProductId)
										&& (product.VariationId == defaultProduct.VariationId)));
							var since = productItem.SelectableSince ?? dateDelivery;
							var until = productItem.SelectableUntil ?? dateDelivery;

							if ((dateDelivery < since) || (dateDelivery > until)) continue;
							var subscriptionProduct = productService.GetProductVariation(
								defaultProduct.ShopId,
								defaultProduct.ProductId,
								defaultProduct.VariationId,
								memberRankId,
								accessor);
							if (subscriptionProduct == null) continue;
							nextProducts.Add(
								new FixedPurchaseItemModel()
								{
									FixedPurchaseId = fixedPurchaseId,
									FixedPurchaseItemNo = itemNo,
									FixedPurchaseShippingNo = 1,
									ShopId = Constants.CONST_DEFAULT_SHOP_ID,
									ProductId = subscriptionProduct.ProductId,
									VariationId = subscriptionProduct.VariationId,
									SupplierId = subscriptionProduct.SupplierId,
									ItemQuantity = defaultProduct.ItemQuantity,
									ItemQuantitySingle = defaultProduct.ItemQuantity,
									DateCreated = DateTime.Now,
									DateChanged = DateTime.Now,
									ProductSetNo = null,
									ProductSetId = string.Empty,
									ProductSetCount = null,
									ProductOptionTexts = string.Empty
								});
							itemNo++;
						}

						isPartialFailure = (defaultOrderProducts.Length != nextProducts.Count);

						result.NextCount = currentCount;
						break;
						#endregion
					}

					#region 引き継ぎ有り
					if (shipping == null)
					{
						result.Result = SubscriptionBoxGetNextProductsResult.ResultTypes.Cancel;
						return result;
					}
					// 引き継ぎ対象の商品があるかチェック
					for (var beforeSubscriptionCount = subscriptionBoxOrderCount - 1; beforeSubscriptionCount > 0; beforeSubscriptionCount--)
					{
						defaultOrderProducts = subscriptionBox.DefaultOrderProducts
								.Where(defaultItem => defaultItem.Count == beforeSubscriptionCount)
								.ToArray();
						var canTakeOver = defaultOrderProducts
							.Any(defaultOrderProduct => string.IsNullOrEmpty(defaultOrderProduct.ProductId));
						if (canTakeOver == false) break;
					}

					result.HasNecessaryFlg = HasNecessaryProducts(defaultOrderProducts);

					if (result.HasNecessaryFlg
						&& (subscriptionBox.FirstSelectableFlg == Constants.FLG_SUBSCRIPTIONBOX_FIRST_SELECTABLE_PAGE_FALSE))
					{
						defaultOrderProducts = defaultOrderProducts
							.Where(defaultOrderProduct => defaultOrderProduct.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_VALID)
							.ToArray();

						foreach (var takeOverBeforeProduct in defaultOrderProducts)
						{
							var productItem = subscriptionBox.SelectableProducts.First(
								product => ((product.ProductId == takeOverBeforeProduct.ProductId)
									&& (product.VariationId == takeOverBeforeProduct.VariationId)));
							var since = productItem.SelectableSince ?? dateDelivery;
							var until = productItem.SelectableUntil ?? dateDelivery;

							if ((dateDelivery < since) || (dateDelivery > until)) continue;
							var supplierId = new ProductService().GetProductVariation(
								takeOverBeforeProduct.ShopId,
								takeOverBeforeProduct.ProductId,
								takeOverBeforeProduct.VariationId,
								memberRankId).SupplierId;
							nextProducts.Add(
								new FixedPurchaseItemModel()
								{
									FixedPurchaseId = fixedPurchaseId,
									FixedPurchaseItemNo = itemNo,
									FixedPurchaseShippingNo = 1,
									ShopId = Constants.CONST_DEFAULT_SHOP_ID,
									ProductId = takeOverBeforeProduct.ProductId,
									VariationId = takeOverBeforeProduct.VariationId,
									SupplierId = supplierId,
									ItemQuantity = takeOverBeforeProduct.ItemQuantity,
									ItemQuantitySingle = takeOverBeforeProduct.ItemQuantity,
									DateCreated = DateTime.Now,
									DateChanged = DateTime.Now,
									ProductSetNo = null,
									ProductSetId = string.Empty,
									ProductSetCount = null,
									ProductOptionTexts = string.Empty
								});

							itemNo++;
						}
						isPartialFailure = (defaultOrderProducts.Length != nextProducts.Count);
						result.NextCount = currentCount;
						break;
					}

					foreach (var takeOverBeforeProduct in shipping.Items)
					{
						var productItem = subscriptionBox.SelectableProducts
							.First(product => ((product.ProductId == takeOverBeforeProduct.ProductId)
								&& (product.VariationId == takeOverBeforeProduct.VariationId)));
						var since = productItem.SelectableSince ?? dateDelivery;
						var until = productItem.SelectableUntil ?? dateDelivery;

						if ((dateDelivery < since) || (dateDelivery > until)) continue;

						nextProducts.Add(
							new FixedPurchaseItemModel()
							{
								FixedPurchaseId = fixedPurchaseId,
								FixedPurchaseItemNo = itemNo,
								FixedPurchaseShippingNo = 1,
								ShopId = Constants.CONST_DEFAULT_SHOP_ID,
								ProductId = takeOverBeforeProduct.ProductId,
								VariationId = takeOverBeforeProduct.VariationId,
								SupplierId = takeOverBeforeProduct.SupplierId,
								ItemQuantity = takeOverBeforeProduct.ItemQuantity,
								ItemQuantitySingle = takeOverBeforeProduct.ItemQuantity,
								DateCreated = DateTime.Now,
								DateChanged = DateTime.Now,
								ProductSetNo = null,
								ProductSetId = string.Empty,
								ProductSetCount = null,
								ProductOptionTexts = string.Empty
							});
						itemNo++;
					}
					#endregion

					result.NextCount = currentCount;
					break;

				case Constants.FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_PERIOD:
					var lastDate = subscriptionBox.DefaultOrderProducts.Max(dp => dp.TermUntil);
					if ((lastDate < dateDelivery))
					{
						result.Result = SubscriptionBoxGetNextProductsResult.ResultTypes.Cancel;
						return result;
					}

					defaultOrderProducts = subscriptionBox.DefaultOrderProducts
						.Where(defaultItem => (defaultItem.TermSince <= dateDelivery) && (dateDelivery <= defaultItem.TermUntil))
						.ToArray();
					if (defaultOrderProducts.Any() == false) return result;

					result.HasNecessaryFlg = HasNecessaryProducts(defaultOrderProducts);
					if (result.HasNecessaryFlg)
					{
						defaultOrderProducts = defaultOrderProducts
							.Where(defaultOrderProduct => defaultOrderProduct.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_VALID)
							.ToArray();
					}

					foreach (var defaultProduct in defaultOrderProducts)
					{
						// 前回の商品を引き継ぐ判定
						#region 引き継ぎ無し
						if (string.IsNullOrEmpty(defaultProduct.ProductId) == false)
						{
							var subscriptionProduct = productService.GetProductVariation(
								defaultProduct.ShopId,
								defaultProduct.ProductId,
								defaultProduct.VariationId,
								memberRankId,
								accessor);
							if (subscriptionProduct == null) continue;

							result.HasNecessaryFlg = HasNecessaryProducts(defaultOrderProducts);
							if (result.HasNecessaryFlg)
							{
								defaultOrderProducts = defaultOrderProducts
									.Where(defaultOrderProduct => defaultOrderProduct.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_VALID)
									.ToArray();
							}

							var itemOrderCount = 0;
							var itemShippingCount = 0;
							if ((shipping != null)
								&& (shipping.Items
									.Any(shippingItem => (shippingItem.ShopId == subscriptionProduct.ShopId)
										&& (shippingItem.ProductId == subscriptionProduct.ProductId)
										&& (shippingItem.VariationId == subscriptionProduct.VariationId))))
							{
								var item = shipping.Items
									.First(shippingItem => (shippingItem.ShopId == subscriptionProduct.ShopId)
								&& (shippingItem.ProductId == subscriptionProduct.ProductId)
								&& (shippingItem.VariationId == subscriptionProduct.VariationId));
								itemOrderCount = item.ItemOrderCount;
								itemShippingCount = item.ItemShippedCount;
							}

							nextProducts.Add(
									new FixedPurchaseItemModel()
									{
										FixedPurchaseId = fixedPurchaseId,
										FixedPurchaseItemNo = itemNo,
										FixedPurchaseShippingNo = 1,
										ShopId = Constants.CONST_DEFAULT_SHOP_ID,
										ProductId = subscriptionProduct.ProductId,
										VariationId = subscriptionProduct.VariationId,
										SupplierId = subscriptionProduct.SupplierId,
										ItemQuantity = defaultProduct.ItemQuantity,
										ItemQuantitySingle = defaultProduct.ItemQuantity,
										DateCreated = DateTime.Now,
										DateChanged = DateTime.Now,
										ProductSetNo = null,
										ProductSetId = string.Empty,
										ProductSetCount = null,
										ProductOptionTexts = string.Empty,
										ItemOrderCount = itemOrderCount,
										ItemShippedCount = itemShippingCount
									});

							itemNo++;
							isPartialFailure = (defaultOrderProducts.Length != nextProducts.Count);
							continue;
						}
						#endregion

						#region 引き継ぎ有りの場合
						if (shipping == null)
						{
							result.Result = SubscriptionBoxGetNextProductsResult.ResultTypes.Cancel;
							return result;
						}

						// 引き継ぎ対象の商品があるかチェック
						var fixedPurchase = new FixedPurchaseService().Get(fixedPurchaseId);
						var beforeSubscriptionCount = 0;
						for (beforeSubscriptionCount = fixedPurchase.Shippings.Length - 1; beforeSubscriptionCount >= 0; beforeSubscriptionCount--)
						{
							var shippingDate = fixedPurchase.Shippings[beforeSubscriptionCount].DateCreated;
							defaultOrderProducts = subscriptionBox.DefaultOrderProducts
									.Where(defaultItem => (defaultItem.TermSince <= shippingDate)
										&& (shippingDate <= defaultItem.TermUntil)).ToArray();
							var canTakeOver = defaultOrderProducts
								.Any(defaultOrderProduct => string.IsNullOrEmpty(defaultOrderProduct.ProductId));
							if (canTakeOver == false) break;
						}

						// 引継ぎ対象がなかった場合初回購入時の商品を入れる
						if (beforeSubscriptionCount == 1)
						{
							defaultOrderProducts = subscriptionBox.DefaultOrderProducts
								.Where(defaultItem => (defaultItem.TermSince <= fixedPurchase.DateCreated)
									&& (fixedPurchase.DateCreated <= defaultItem.TermUntil)).ToArray();
						}

						result.HasNecessaryFlg = HasNecessaryProducts(defaultOrderProducts);

						if (result.HasNecessaryFlg)
						{
							defaultOrderProducts = defaultOrderProducts
								.Where(defaultOrderProduct => defaultOrderProduct.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_VALID)
								.ToArray();

							foreach (var takeOverBeforeProduct in defaultOrderProducts)
							{
								var productItem = subscriptionBox.SelectableProducts.First(
									product => ((product.ProductId == takeOverBeforeProduct.ProductId)
										&& (product.VariationId == takeOverBeforeProduct.VariationId)));
								var since = productItem.SelectableSince ?? dateDelivery;
								var until = productItem.SelectableUntil ?? dateDelivery;

								if ((dateDelivery < since) || (dateDelivery > until)) continue;

								var supplierId = new ProductService().GetProductVariation(
									takeOverBeforeProduct.ShopId,
									takeOverBeforeProduct.ProductId,
									takeOverBeforeProduct.VariationId,
									memberRankId).SupplierId;
								var itemOrderCount = 0;
								itemOrderCount = shipping.Items
									.FirstOrDefault(p => p.VariationId == takeOverBeforeProduct.VariationId)
									.ItemOrderCount;
								var itemShipedCount = 0;
								itemShipedCount = shipping.Items
									.FirstOrDefault(p => p.VariationId == takeOverBeforeProduct.VariationId)
									.ItemShippedCount;

								nextProducts.Add(
									new FixedPurchaseItemModel()
									{
										FixedPurchaseId = fixedPurchaseId,
										FixedPurchaseItemNo = itemNo,
										FixedPurchaseShippingNo = 1,
										ShopId = Constants.CONST_DEFAULT_SHOP_ID,
										ProductId = takeOverBeforeProduct.ProductId,
										VariationId = takeOverBeforeProduct.VariationId,
										SupplierId = supplierId,
										ItemQuantity = takeOverBeforeProduct.ItemQuantity,
										ItemQuantitySingle = takeOverBeforeProduct.ItemQuantity,
										DateCreated = DateTime.Now,
										DateChanged = DateTime.Now,
										ProductSetNo = null,
										ProductSetId = string.Empty,
										ProductSetCount = null,
										ProductOptionTexts = string.Empty,
										ItemOrderCount = itemOrderCount,
										ItemShippedCount = itemShipedCount
									});

								itemNo++;
							}
							break;
						}

						foreach (var takeOverBeforeProduct in shipping.Items)
						{
							var productItem = subscriptionBox.SelectableProducts.First(
								product => ((product.ProductId == takeOverBeforeProduct.ProductId)
									&& (product.VariationId == takeOverBeforeProduct.VariationId)));
							var since = productItem.SelectableSince ?? dateDelivery;
							var until = productItem.SelectableUntil ?? dateDelivery;

							if ((dateDelivery < since) || (dateDelivery > until)) continue;

							nextProducts.Add(
								new FixedPurchaseItemModel()
								{
									FixedPurchaseId = fixedPurchaseId,
									FixedPurchaseItemNo = itemNo,
									FixedPurchaseShippingNo = 1,
									ShopId = Constants.CONST_DEFAULT_SHOP_ID,
									ProductId = takeOverBeforeProduct.ProductId,
									VariationId = takeOverBeforeProduct.VariationId,
									SupplierId = takeOverBeforeProduct.SupplierId,
									ItemQuantity = takeOverBeforeProduct.ItemQuantity,
									ItemQuantitySingle = takeOverBeforeProduct.ItemQuantity,
									DateCreated = DateTime.Now,
									DateChanged = DateTime.Now,
									ProductSetNo = null,
									ProductSetId = string.Empty,
									ProductSetCount = null,
									ProductOptionTexts = string.Empty,
									ItemOrderCount = takeOverBeforeProduct.ItemOrderCount,
									ItemShippedCount = takeOverBeforeProduct.ItemShippedCount
								});

							itemNo++;
						}
						isPartialFailure = (defaultOrderProducts.Length + shipping.Items.Length != nextProducts.Count);
						#endregion
					}
					result.NextCount = subscriptionBoxOrderCount;
					break;
			}

			if (nextProducts.Count == 0) return result;

			result.NextProducts = nextProducts.ToArray();
			result.Result = isPartialFailure
				? SubscriptionBoxGetNextProductsResult.ResultTypes.PartialFailure
				: SubscriptionBoxGetNextProductsResult.ResultTypes.Success;
			return result;
		}

		/// <summary>
		/// 必須商品保持判定
		/// </summary>
		/// <param name="defaultOrderProducts">デフォルト商品</param>
		/// <returns>結果</returns>
		private bool HasNecessaryProducts(SubscriptionBoxDefaultItemModel[] defaultOrderProducts)
		{
			var result = defaultOrderProducts
				.Any(defaultOrderProduct => defaultOrderProduct.NecessaryProductFlg == Constants.FLG_ON);
			return result;
		}
		#endregion

		#region +GetSubscriptionBoxItemList
		/// <summary>
		/// 頒布会商品データー一覧取得
		/// </summary>
		/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
		/// <param name="date">次回配送日付</param>
		/// <returns>散布会商品データー一覧</returns>
		public SubscriptionBoxItemModel[] GetSubscriptionBoxItemList(string shopId, string subscriptionBoxCourseId, DateTime date)
		{
			using (var repository = new SubscriptionBoxRepository())
			{
				var models = repository.GetSubscriptionBoxItemList(shopId, subscriptionBoxCourseId, date);
				return models;
			}
		}
		#endregion

		#region GetSubscriptionBoxProductOfNumberErrorType
		/// <summary>
		/// 頒布会数量エラー取得
		/// </summary>
		/// <param name="subsctiptionBoxCourseId">頒布会コースID</param>
		/// <param name="numberOfProducts">商品種類数</param>
		/// <param name="isFront">フロントか</param>
		/// <returns>頒布会商品種類数エラー判定クラス</returns>
		public SubscriptionBoxProductOfNumberError GetSubscriptionBoxProductOfNumberErrorType(
			string subsctiptionBoxCourseId,
			int numberOfProducts,
			bool isFront = false)
		{
			var subscriptionBoxConfig = new SubscriptionBoxService().GetByCourseId(subsctiptionBoxCourseId);
			var minimumNumberOfProducts = subscriptionBoxConfig.MinimumNumberOfProducts;
			var maximumNumberOfProducts = subscriptionBoxConfig.MaximumNumberOfProducts;
			var displayName = (isFront) ? subscriptionBoxConfig.DisplayName : subscriptionBoxConfig.ManagementName;
			var result = GetSubscriptionBoxProductOfNumberErrorType(
				displayName,
				numberOfProducts,
				minimumNumberOfProducts,
				maximumNumberOfProducts);

			return result;
		}
		
		/// <summary>
		/// 頒布会数量エラー取得
		/// </summary>
		/// <param name="displayName">表示名</param>
		/// <param name="numberOfProducts">商品種類数</param>
		/// <param name="minimumNumberOfProducts">最低種類数</param>
		/// <param name="maximumNumberOfProducts">最大種類数</param>
		/// <returns>頒布会商品種類数エラー判定クラス</returns>
		public SubscriptionBoxProductOfNumberError GetSubscriptionBoxProductOfNumberErrorType(
			string displayName,
			int numberOfProducts,
			int? minimumNumberOfProducts,
			int? maximumNumberOfProducts)
		{
			var result = new SubscriptionBoxProductOfNumberError(
				displayName,
				StringUtility.ToEmpty(minimumNumberOfProducts),
				StringUtility.ToEmpty(maximumNumberOfProducts));

			if (minimumNumberOfProducts.HasValue && (numberOfProducts < minimumNumberOfProducts))
			{
				result.ErrorType = SubscriptionBoxProductOfNumberError.ErrorTypes.MinLimit;
				return result;
			}

			if (maximumNumberOfProducts.HasValue && (numberOfProducts > maximumNumberOfProducts))
			{
				result.ErrorType = SubscriptionBoxProductOfNumberError.ErrorTypes.MaxLimit;
				return result;
			}

			return result;
		}
		#endregion

		#region GetValidSubscriptionBoxByVariationId
		/// <summary>
		/// バリエーションIDで頒布会を取得
		/// </summary>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>頒布会</returns>
		public SubscriptionBoxModel[] GetValidSubscriptionBoxByVariationId(
			string variationId,
			SqlAccessor accessor = null)
		{
			using (var repository = new SubscriptionBoxRepository(accessor))
			{
				var model = repository.GetValidSubscriptionBoxByVariationId(variationId);
				return model;
			}
		}
		#endregion

		#region GetSubscriptionItemByProductId
		/// <summary>
		/// 商品IDに紐づく頒布会コースを取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>頒布会コースID</returns>
		public SubscriptionBoxItemModel[] GetSubscriptionItemByProductId(
			string shopId,
			string productId,
			SqlAccessor accessor = null)
		{
			using (var repository = new SubscriptionBoxRepository(accessor))
			{
				var model = repository.GetSubscriptionItemByProductId(shopId, productId);
				return model;
			}
		}
		#endregion

		#region GetSubscriptionItemByProductId
		/// <summary>
		/// 商品IDに紐づく頒布会コースを取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>頒布会コースID</returns>
		public SubscriptionBoxItemModel[] GetSubscriptionItemByProductVariationId(
			string shopId,
			string productId,
			string variationId,
			SqlAccessor accessor = null)
		{
			using (var repository = new SubscriptionBoxRepository(accessor))
			{
				var model = repository.GetSubscriptionItemByProductVariationId(shopId, productId, variationId);
				return model;
			}
		}
		#endregion

		/// <summary>
		/// 頒布会選択可能商品を取得<br />
		/// （商品名等も含む）
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="subscriptionBoxCourseId">コースID</param>
		/// <returns>選択可能商品</returns>
		public SubscriptionBoxItemContainerList GetSubscriptionItemsWithProductInfo(
			string fixedPurchaseId,
			string subscriptionBoxCourseId)
		{
			using (var repository = new SubscriptionBoxRepository())
			{
				var items = repository.GetSubscriptionItemsWithProductInfo(
					fixedPurchaseId,
					subscriptionBoxCourseId);

				return items;
			}
		}

		/// <summary>
		/// 定期台帳に設定されている商品が全て、紐づいている頒布会コース内の商品に設定されているかの確認
		/// </summary>
		/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor"></param>
		/// <returns>定期台帳に設定されている商品が全て、紐づいている頒布会コース内の商品に設定されているか</returns>
		public bool CheckAllItemInSubscriptionBoxItem(
			string subscriptionBoxCourseId,
			string fixedPurchaseId,
			string lastChanged,
			SqlAccessor accessor = null)
		{
			// そもそも定期台帳に頒布会が紐づいていない場合
			if (string.IsNullOrEmpty(subscriptionBoxCourseId)) return true;

			var subscriptionBox = new SubscriptionBoxService().GetByCourseId(subscriptionBoxCourseId);

			//　定期台帳に紐づく頒布会がない場合はfalseを返す。
			if (subscriptionBox == null) return false;

			var fixedPurchaseItems = new FixedPurchaseService().GetAllItem(fixedPurchaseId);
			// 定期台帳に設定されている商品が全て、紐づいている頒布会コース内の商品に設定されている場合はtrueを返す。

			if(IsItemsInSubscrioptionBoxItem(fixedPurchaseItems)) return true;
			return false;

			/// <summary>
			/// <param name="Items">定期台帳に設定されている商品</param>
			/// <returns>定期台帳内の全ての商品が頒布会コースの商品に設定されているか</returns>
			/// </summary>
			bool IsItemsInSubscrioptionBoxItem(FixedPurchaseItemModel[] Items)
			{
				foreach (var fixedPurchaseItem in Items)
				{
					var productItem = subscriptionBox.SelectableProducts
						.Any(product => ((product.ProductId == fixedPurchaseItem.ProductId)
						&& (product.VariationId == fixedPurchaseItem.VariationId)));
					if (productItem == false)
					{
						FileLogger.WriteError(string.Format(
						"定期購入ID：{0}は頒布会コースに設定されていない商品（商品ID：{1}　バリエーションID：{2}）が設定されています。",
						fixedPurchaseId, fixedPurchaseItem.ProductId, fixedPurchaseItem.VariationId));

						// 定期台帳に紐づく頒布会がない場合は定期購入ステータスをその他エラーにする。
						new FixedPurchaseService().UpdateForFailedOrder(
								fixedPurchaseId,
								lastChanged,
								UpdateHistoryAction.Insert,
								accessor);
						return false;
					}
				}
				return true;
			}
		}

		#region +GetFirstSelection 頒布会初回選択商品取得
		/// <summary>
		/// 頒布会初回選択可能商品取得
		/// </summary>
		/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
		/// <returns>頒布会表示名</returns>
		public DataView GetFirstSelectionItems(string subscriptionBoxCourseId)
		{
			using (var repository = new SubscriptionBoxRepository())
			{
				var result = repository.GetFirstSelectableItems(subscriptionBoxCourseId);
				return result;
			}
		}
		#endregion

		#region +GetFirstDefaultItem
		/// <summary>
		/// 一回目のデフォルト商品取得
		/// </summary>
		/// <param name="subscriptionBoxCourseId"></param>
		/// <returns>一回目に登録されているデフォルト商品</returns>
		public SubscriptionBoxDefaultItemModel[] GetFirstDefaultItem(string subscriptionBoxCourseId)
		{
			using (var repository = new SubscriptionBoxRepository())
			{
				var models = repository.GetFirstDefaultItem(subscriptionBoxCourseId);
				return models;
			}
		}
		#endregion

		#region +GetDefaultItemDetails
		/// <summary>
		/// デフォルト商品情報取得（日別予測レポートバッチ用）
		/// </summary>
		/// <param name="subscriptionBoxCourseId"></param>
		/// <returns>デフォルト商品情報</returns>
		public SubscriptionBoxDefaultItemModel[] GetDefaultItemDetails(string subscriptionBoxCourseId)
		{
			using (var repository = new SubscriptionBoxRepository())
			{
				var models = repository.GetDefaultItemDetails(subscriptionBoxCourseId);
				return models;
			}
		}
		#endregion
	}
}
