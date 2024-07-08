/*
=========================================================================================================
  Module      : 定期購入同梱ユーティリティクラス(FixedPurchaseCombineUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.DataCacheController;
using w2.App.Common.Order.FixedPurchase;
using w2.App.Common.Order.Register;
using w2.Common.Sql;
using w2.Domain.Coupon;
using w2.Domain.Coupon.Helper;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchase.Helper;
using w2.App.Common.Product;
using w2.Domain.Order;
using w2.Domain.ShopShipping;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Common.Util;
using w2.Domain.DeliveryCompany;

namespace w2.App.Common.Order.FixedPurchaseCombine
{
	/// <summary>
	/// 定期購入同梱ユーティリティクラス
	/// </summary>
	public class FixedPurchaseCombineUtility
	{
		/// <summary>
		/// 定期台帳の同梱処理
		/// </summary>
		/// <param name="parentFixedPurchaseId">親定期台帳ID</param>
		/// <param name="childFixedPurchaseIds">子定期台帳ID配列</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>成功したか</returns>
		public static bool CombineFixedPurchase(
			string parentFixedPurchaseId,
			string[] childFixedPurchaseIds,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var fixedPurchaseService = new FixedPurchaseService();
				var parentFp = fixedPurchaseService.Get(parentFixedPurchaseId, accessor);
				var mergedFpItems = parentFp.Shippings[0].Items;
				var result = false;
				var count = 0;
				var existExpressInChildFps = false;
				var hasGetNextShippingUseCouponFromChild = string.IsNullOrEmpty(parentFp.NextShippingUseCouponId);
				UserCouponDetailInfo useCoupon = null;
				string apiError;

				foreach (var childFixedPurchaseId in childFixedPurchaseIds)
				{
					// 子定期台帳の商品を追加
					var childFp = fixedPurchaseService.Get(childFixedPurchaseId, accessor);
					mergedFpItems = MergeFpItemsIntoParentFpItems(mergedFpItems, childFp.Shippings[0].Items);

					// 親定期台帳の利用クーポンがない場合、子定期台帳の利用クーポンがあったら、設定したクーポン情報を取得
					if (hasGetNextShippingUseCouponFromChild && (childFp.NextShippingUseCouponDetail != null))
					{
						useCoupon = childFp.NextShippingUseCouponDetail;
						hasGetNextShippingUseCouponFromChild = false;
					}

					var fixedPurchaseChild = fixedPurchaseService.GetContainer(
						childFixedPurchaseId,
						accessor: accessor);
					existExpressInChildFps |= (fixedPurchaseChild.Shippings[0].ShippingMethod == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS);

					// 子定期台帳のキャンセル（次回利用ポイント／クーポンも戻す）
					result = fixedPurchaseService.CancelFixedPurchase(
						fixedPurchaseChild,
						lastChanged,
						Constants.CONST_DEFAULT_DEPT_ID,
						Constants.W2MP_POINT_OPTION_ENABLED,
						UpdateHistoryAction.DoNotInsert,
						accessor);

					result = result && FixedPurchaseHelper.CancelPaymentContinuous(
						fixedPurchaseChild.FixedPurchaseId,
						fixedPurchaseChild.OrderPaymentKbn,
						fixedPurchaseChild.ExternalPaymentAgreementId,
						lastChanged,
						out apiError,
						accessor);
					if (result == false)
					{
						accessor.RollbackTransaction();
						return false;
					}

					// 子定期台帳の拡張ステータス：注文同梱キャンセルフラグを更新（更新履歴とともに）
					count = UpdateManagerFixedPurchaseCombineCancelFlg(
						childFixedPurchaseId,
						true,
						lastChanged,
						UpdateHistoryAction.DoNotInsert,
						accessor);
					if (count == 0)
					{
						accessor.RollbackTransaction();
						return false;
					}

					//注文メモのマージ
					if (string.IsNullOrEmpty(childFp.Memo) == false)
					{
						var newOrderMemo = JoinMemo(parentFp.Memo, childFp.Memo);
						fixedPurchaseService.UpdateFixedPurchaseMemo(
							parentFixedPurchaseId,
							newOrderMemo,
							lastChanged,
							UpdateHistoryAction.DoNotInsert);
					}

					//管理メモのマージ
					if (string.IsNullOrEmpty(childFp.FixedPurchaseManagementMemo) == false)
					{
						var newManagementMemo = JoinMemo(parentFp.FixedPurchaseManagementMemo, childFp.FixedPurchaseManagementMemo);
						fixedPurchaseService.UpdateFixedPurchaseManagementMemo(
							parentFixedPurchaseId,
							newManagementMemo,
							lastChanged,
							UpdateHistoryAction.DoNotInsert);
					}

					//配送メモのマージ
					if(string.IsNullOrEmpty(childFp.ShippingMemo) == false)
					{
						var newShippingMemo = JoinMemo(parentFp.ShippingMemo, childFp.ShippingMemo);
						fixedPurchaseService.UpdateShippingMemo(
							parentFixedPurchaseId,
							newShippingMemo,
							lastChanged,
							UpdateHistoryAction.DoNotInsert);
					}

					// 更新履歴登録
					if (updateHistoryAction == UpdateHistoryAction.Insert)
					{
						new UpdateHistoryService().InsertForFixedPurchase(childFixedPurchaseId, lastChanged, accessor);
						new UpdateHistoryService().InsertForUser(fixedPurchaseChild.UserId, lastChanged, accessor);
					}
				}

				// 親定期台帳の配送方法がメール便の場合で、子定期台帳の配送方法に宅配便が含まれるもしくは同梱後メール便での配送ができない場合、配送方法を宅配便に変更する
				var shippingMethodChangeToExpress = false;
				if (parentFp.Shippings[0].ShippingMethod == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL)
				{
					shippingMethodChangeToExpress = (existExpressInChildFps || (OrderCommon.IsAvailableShippingKbnMail(mergedFpItems) == false));

					// 親の配送サービスが宅配便を使用不可の場合、親のデフォルト配送サービスへ変更
					var shippingTypeId = CheckFixdPurchaseCompanyId(parentFp);
					if (shippingMethodChangeToExpress && (string.IsNullOrEmpty(shippingTypeId) == false))
					{
						var defaultShopShipping = DataCacheControllerFacade.GetShopShippingCacheController().Get(shippingTypeId);
						var defaultExpressDeliveryCompany = new DeliveryCompanyService().Get(
							defaultShopShipping.GetDefaultDeliveryCompany(true).DeliveryCompanyId);

						parentFp.Shippings[0].DeliveryCompanyId =
							defaultExpressDeliveryCompany.DeliveryCompanyId;

						fixedPurchaseService.UpdateShipping(
							parentFp.Shippings[0],
							lastChanged,
							UpdateHistoryAction.DoNotInsert,
							accessor);
					}
				}

				// 親定期台帳の商品情報を更新
				result = fixedPurchaseService.UpdateItemsForOrderCombine(
					mergedFpItems,
					string.Join(",", childFixedPurchaseIds),
					shippingMethodChangeToExpress,
					lastChanged,
					UpdateHistoryAction.DoNotInsert,
					accessor);

				// 次回購入の利用クーポンの同梱処理
				if (result
					&& Constants.W2MP_COUPON_OPTION_ENABLED
					&& string.IsNullOrEmpty(parentFp.NextShippingUseCouponId)
					&& (useCoupon != null))
				{
					// 定期台帳の利用クーポン更新
					result = fixedPurchaseService.UpdateNextShippingUseCoupon(
						parentFp.FixedPurchaseId,
						useCoupon.CouponId,
						useCoupon.CouponNo,
						lastChanged,
						updateHistoryAction,
						accessor);

					// 次回購入の利用クーポンを適用
					if (result)
					{
						var user = new UserService().Get(parentFp.UserId, accessor);
						result = new CouponService().ApplyNextShippingUseCouponToFixedPurchase(
							useCoupon,
							parentFp.UserId,
							parentFp.FixedPurchaseId,
							(Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE == Constants.FLG_COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE_MAIL_ADDRESS)
								? ((user != null) ? user.MailAddr : string.Empty)
								: parentFp.UserId,
							lastChanged,
							updateHistoryAction,
							accessor);
					}
				}

				if (result == false)
				{
					accessor.RollbackTransaction();
					return false;
				}

				count = UpdateManagerFixedPurchaseCombineFlg(
					parentFixedPurchaseId,
					true,
					DateTime.Now.Date, 
					lastChanged,
					updateHistoryAction,
					accessor);
				if (count == 0)
				{
					accessor.RollbackTransaction();
					return false;
				}

				accessor.CommitTransaction();
				return true;
			}
		}

		/// <summary>
		/// 定期購入同梱 管理者同梱フラグ(拡張ステータス)更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="flg">フラグ</param>
		/// <param name="updateDate">拡張ステータス更新日</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		private static int UpdateManagerFixedPurchaseCombineFlg(
			string fixedPurchaseId,
			bool flg,
			DateTime updateDate,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var count = new FixedPurchaseService().UpdateExtendStatus(
				fixedPurchaseId,
				35,
				(flg) ? Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_ON : Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF,
				updateDate,
				lastChanged,
				updateHistoryAction,
				accessor);
			return count;
		}

		/// <summary>
		/// 定期購入設定文言を取得
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <returns>定期購入設定文言</returns>
		public static string GetFixedPachasePatternSettingMessage(string fixedPurchaseId)
		{
			if (string.IsNullOrEmpty(fixedPurchaseId)) { return "―"; }

			var parentFp = new FixedPurchaseService().Get(fixedPurchaseId);
			if (parentFp == null) { return "―"; }

			var fpSettingMessage = OrderCommon.CreateFixedPurchaseSettingMessage(parentFp.FixedPurchaseKbn, parentFp.FixedPurchaseSetting1);
			return fpSettingMessage;
		}

		/// <summary>
		/// 定期購入同梱管理者同梱キャンセルフラグ(拡張ステータス)更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="flg">フラグ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		private static int UpdateManagerFixedPurchaseCombineCancelFlg(
			string fixedPurchaseId,
			bool flg,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var count = new FixedPurchaseService().UpdateExtendStatus(
				fixedPurchaseId,
				36,
				(flg) ? Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_ON : Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF,
				DateTime.Now.Date,
				lastChanged,
				updateHistoryAction,
				accessor);
			return count;
		}

		/// <summary>
		/// 定期購入アイテム作成
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="fixedPurchaseShippingNo">定期購入配送先枝番</param>
		/// <param name="itemNo">アイテムNO</param>
		/// <param name="cartProduct">カート商品</param>
		/// <returns>定期購入アイテム</returns>
		private static FixedPurchaseItemModel CreateFixedPurchaseItem(string fixedPurchaseId, int fixedPurchaseShippingNo, int itemNo, CartProduct cartProduct)
		{
			var item = new FixedPurchaseItemModel
			{
				FixedPurchaseId = fixedPurchaseId,
				FixedPurchaseItemNo = itemNo,
				FixedPurchaseShippingNo = fixedPurchaseShippingNo,
				ShopId = cartProduct.ShopId,
				ProductId = cartProduct.ProductId,
				VariationId = cartProduct.VariationId,
				SupplierId = cartProduct.SupplierId,
				ItemQuantity = cartProduct.Count,
				ItemQuantitySingle = cartProduct.CountSingle,
				ProductOptionTexts =
					(cartProduct.ProductOptionSettingList != null)
						? ProductOptionSettingHelper.GetSelectedOptionSettingForFixedPurchaseItem(cartProduct.ProductOptionSettingList)
						: string.Empty
			};
			return item;
		}

		/// <summary>
		/// 注文同梱済みカートの配送情報更新(定期購入用)
		/// </summary>
		/// <param name="combinedCart">注文同梱済みカート</param>
		/// <param name="fixedPurchaseKbn">定期購入区分</param>
		/// <param name="fixedPurchaseSetting">定期購入設定</param>
		/// <param name="nextShippingDate">次回配送予定日</param>
		public static void UpdateCombinedCartFixedPurchaseDeliveryInfo(
			CartObject combinedCart,
			string fixedPurchaseKbn,
			string fixedPurchaseSetting,
			DateTime? nextShippingDate = null)
		{
			var combinedCartShip = combinedCart.Shippings[0];
			var shopShip = DataCacheControllerFacade.GetShopShippingCacheController().Get(combinedCart.ShippingType);

			// 次回・次々回配送日計算
			var fixedPurchaseService = new FixedPurchaseService();
			var calculateMode = fixedPurchaseService.GetCalculationMode(
				fixedPurchaseKbn,
				Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE);
			var nextShipDate = nextShippingDate ?? fixedPurchaseService.CalculateFollowingShippingDate(
				fixedPurchaseKbn,
				fixedPurchaseSetting,
				combinedCart.Shippings[0].ShippingDate.HasValue ? combinedCart.Shippings[0].ShippingDate.Value : DateTime.Now,
				shopShip.FixedPurchaseMinimumShippingSpan,
				calculateMode);

			var nextNextShipDate = fixedPurchaseService.CalculateFollowingShippingDate(
				fixedPurchaseKbn,
				fixedPurchaseSetting,
				nextShipDate,
				shopShip.FixedPurchaseMinimumShippingSpan,
				calculateMode);

			// 注文同梱用配送情報作成
			var cartShip = new CartShipping(combinedCart);
			cartShip.UpdateShippingAddr(combinedCartShip);
			cartShip.UpdateFixedPurchaseSetting(
				fixedPurchaseKbn,
				fixedPurchaseSetting,
				shopShip.FixedPurchaseShippingDaysRequired,
				shopShip.FixedPurchaseMinimumShippingSpan);
			cartShip.ShippingDate = combinedCart.Shippings[0].ShippingDate;
			cartShip.ShippingTime = combinedCart.Shippings[0].ShippingTime;
			cartShip.ScheduledShippingDate = combinedCart.Shippings[0].ScheduledShippingDate;
			cartShip.UpdateNextShippingDates(nextShipDate, nextNextShipDate);
			cartShip.ShippingMethod = combinedCart.Shippings[0].ShippingMethod;
			cartShip.DeliveryCompanyId = combinedCart.Shippings[0].DeliveryCompanyId;

			// 配送先情報更新
			combinedCart.SetShippingAddressAndShippingDateTime(cartShip);
			combinedCart.GetShipping().UpdateShippingDateTime(
				shopShip.IsValidShippingDateSetFlg,
				(combinedCart.Shippings[0].ShippingTime != null),
				combinedCart.Shippings[0].ShippingDate,
				combinedCart.Shippings[0].ShippingTime,
				combinedCart.Shippings[0].ShippingTimeMessage);
		}

		/// <summary>
		/// 注文同梱用 カート情報から定期商品作成
		/// </summary>
		/// <param name="parentFpId">注文同梱親注文 定期購入ID</param>
		/// <param name="childCart">カート情報</param>
		/// <returns>定期商品情報</returns>
		private static FixedPurchaseItemModel[] CreateFpItemsFromCartForOrderCombine(string parentFpId, CartObject childCart)
		{
			var fpItems = childCart.Items
				.Where(item => item.IsFixedPurchase)
				.Select(item => new FixedPurchaseItemModel
				{
					FixedPurchaseId = parentFpId,
					FixedPurchaseItemNo = 0,
					FixedPurchaseShippingNo = 1,
					ShopId = childCart.ShopId,
					ProductId = item.ProductId,
					VariationId = item.VariationId,
					SupplierId = item.SupplierId,
					ItemQuantity = item.Count,
					ItemQuantitySingle = item.CountSingle,
					ProductOptionTexts = (item.ProductOptionSettingList != null)
						? item.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues()
						: string.Empty,
					ItemOrderCount = 1
				})
				.ToArray();

			return fpItems;
		}

		/// <summary>
		/// 注文同梱用 定期商品情報マージ
		/// </summary>
		/// <param name="parentFpItems">注文同梱 親注文定期商品</param>
		/// <param name="childFpItems">注文同梱 子注文定期商品</param>
		/// <returns>定期商品情報</returns>
		public static FixedPurchaseItemModel[] MergeFpItemsIntoParentFpItems(FixedPurchaseItemModel[] parentFpItems, FixedPurchaseItemModel[] childFpItems)
		{
			var mergeItems = parentFpItems.ToList();
			foreach (var childItem in childFpItems)
			{
				var findMergeItem = mergeItems.FirstOrDefault(
					mItem => (mItem.ShopId == childItem.ShopId)
						&& (mItem.ProductId == childItem.ProductId)
						&& (mItem.VariationId == childItem.VariationId)
						&& (mItem.ProductOptionTexts == childItem.ProductOptionTexts));

				if (findMergeItem != null)
				{
					findMergeItem.ItemQuantity += childItem.ItemQuantity;
					findMergeItem.ItemQuantitySingle += childItem.ItemQuantitySingle;
					findMergeItem.ItemOrderCount += childItem.ItemOrderCount;
				}
				else
				{
					mergeItems.Add(childItem);
				}
			}

			var itemNo = 1;
			foreach (var item in mergeItems)
			{
				item.FixedPurchaseId = mergeItems.First().FixedPurchaseId;
				item.FixedPurchaseItemNo = itemNo;
				itemNo++;
			}

			return mergeItems.ToArray();
		}

		/// <summary>
		/// 親注文定期台帳へのカート定期商品の追加
		/// </summary>
		/// <param name="parentId">親注文ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="childCart">子カート</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>成功したか</returns>
		public static bool AddFixedPurchaseItemsForOrderCombine(
			string parentId,
			string userId,
			string lastChanged,
			CartObject childCart,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var parentOrder = new OrderService().Get(parentId, accessor);
			var parentFp = new FixedPurchaseService().Get(parentOrder.FixedPurchaseId, accessor);
			if (parentFp.Shippings[0].IsMail)
			{
				var shopShipping = new ShopShippingService().Get(childCart.ShopId, childCart.ShippingType);
				parentFp.Shippings[0].DeliveryCompanyId = shopShipping.CompanyListExpress.FirstOrDefault(item => item.IsDefault).DeliveryCompanyId;
			}
			var childFpItems = CreateFpItemsFromCartForOrderCombine(parentFp.FixedPurchaseId, childCart);
			var mergedFpItems = MergeFpItemsIntoParentFpItems(parentFp.Shippings[0].Items, childFpItems);
			var shippingMethodChangeToExpress = ((OrderCommon.IsAvailableShippingKbnMail(mergedFpItems) == false)
				&& parentFp.Shippings[0].IsMail);

			// カートとの同梱で子定期購入がないため、子定期購入IDはブランクとする
			var result = new FixedPurchaseService().UpdateItemsForOrderCombine(
				mergedFpItems,
				"",
				shippingMethodChangeToExpress,
				lastChanged,
				updateHistoryAction,
				accessor);

			return result;
		}

		/// <summary>
		/// 定期購入同梱可能な定期購入情報取得
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="nextShipDateFrom">次回配送日From</param>
		/// <param name="nextShipDateTo">次回配送日To</param>
		/// <returns>定期購入情報</returns>
		public static FixedPurchaseModel[] GetCombinableChildFixedPurchases(
			string fixedPurchaseId,
			string userId,
			DateTime nextShipDateFrom,
			DateTime nextShipDateTo)
		{
			var fp = new FixedPurchaseService().Get(fixedPurchaseId);
			var dv = ProductCommon.GetProductInfo(fp.ShopId, fp.Shippings[0].Items[0].ProductId, "", string.Empty);
			if (dv.Count == 0) return null;

			var shippingType = (string)dv[0][Constants.FIELD_PRODUCT_SHIPPING_TYPE];
			
			var models = new FixedPurchaseService().GetCombinableFixedPurchase(
				fp.ShopId,
				Constants.FIXEDPURCHASECOMBINE_ALLOW_FIXEDPURCHASE_STATUS,
				Constants.FIXEDPURCHASECOMBINE_ALLOW_PAYMENT_STATUS,
				userId,
				shippingType,
				nextShipDateFrom,
				nextShipDateTo,
				fp.OrderPaymentKbn);

			var orderedFixedPurchases = models
				.Where(m => (m.FixedPurchaseId != fixedPurchaseId)
					&& (OrderCommon.IsOnlyCancelablePaymentContinuousByManual(m.OrderPaymentKbn) == false)
					&& (m.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU))
				.OrderBy(m => m.NextShippingDate)
				.ToArray();

			return orderedFixedPurchases;
		}

		/// <summary>
		/// 定期購入同梱可能な親定期購入情報取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="userName">ユーザー氏名</param>
		/// <param name="nextShipDateFrom">次回配送日From</param>
		/// <param name="nextShipDateTo">次回配送日To</param>
		/// <param name="startRowNum">取得開始行番号</param>
		/// <param name="endRowNum">取得終了行番号</param>
		/// <returns>モデル</returns>
		public static FixedPurchaseModel[] GetCombinableParentFixedPurchaseWithCondition(
			string userId,
			string userName,
			DateTime nextShipDateFrom,
			DateTime nextShipDateTo,
			int startRowNum,
			int endRowNum)
		{
			var models = new FixedPurchaseService().GetCombinableParentFixedPurchaseWithCondition(
				Constants.FIXEDPURCHASECOMBINE_ALLOW_FIXEDPURCHASE_STATUS,
				Constants.FIXEDPURCHASECOMBINE_ALLOW_PAYMENT_STATUS,
				userId,
				userName,
				nextShipDateFrom,
				nextShipDateTo,
				startRowNum,
				endRowNum);

			var orderedFixedPurchases = models
				.Where(fixedPurchase => (fixedPurchase.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU))
				.ToArray();

			return orderedFixedPurchases;
		}

		/// <summary>
		/// 定期購入同梱可能な親定期購入件数取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="userName">ユーザー氏名</param>
		/// <param name="nextShipDateFrom">次回配送日From</param>
		/// <param name="nextShipDateTo">次回配送日To</param>
		/// <returns>件数</returns>
		public static int GetCombinableParentFixedPurchaseWithConditionCount(
			string userId,
			string userName,
			DateTime nextShipDateFrom,
			DateTime nextShipDateTo)
		{
			var count = new FixedPurchaseService().GetCombinableParentFixedPurchaseWithConditionCount(
				Constants.FIXEDPURCHASECOMBINE_ALLOW_FIXEDPURCHASE_STATUS,
				Constants.FIXEDPURCHASECOMBINE_ALLOW_PAYMENT_STATUS,
				userId,
				userName,
				nextShipDateFrom,
				nextShipDateTo);

			return count;
		}

		/// <summary>
		/// 注文同梱に伴う親定期台帳更新(商品追加含む)
		/// </summary>
		/// <param name="parentOrderId">注文同梱 親注文ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="childCart">注文同梱 子注文カート</param>
		/// <param name="combinedOrderId">注文同梱で作成された注文ID</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>定期台帳更新結果</returns>
		public static OrderRegisterBase.ResultTypes UpdateFixedPurchaseAndAddItemForOrderCombine(
			string parentOrderId,
			string userId,
			string lastChanged,
			CartObject childCart,
			string combinedOrderId,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();
				var orderService = new OrderService();
				var fixedPurchaseService = new FixedPurchaseService();
				var parentOrder = orderService.Get(parentOrderId, accessor);
				var parentFp = fixedPurchaseService.Get(parentOrder.FixedPurchaseId, accessor);

				// 定期台帳に注文同梱履歴追加
				fixedPurchaseService.RegistHistoryForOrderCombine(
					parentOrder.FixedPurchaseId,
					combinedOrderId,
					userId,
					lastChanged,
					UpdateHistoryAction.DoNotInsert,
					accessor,
					true);

				// 定期台帳の子注文の商品を追加
				var addResult = AddFixedPurchaseItemsForOrderCombine(
					parentOrderId,
					userId,
					lastChanged,
					childCart,
					UpdateHistoryAction.DoNotInsert,
					accessor);

				if (addResult)
				{
					// 注文に定期台帳の情報を追加(定期の購入回数は注文同梱元親注文の回数を引き継ぐ)
					orderService.UpdateFixedPurchaseIdAndFixedPurchaseOrderCount(
						combinedOrderId,
						parentOrder.FixedPurchaseId,
						parentOrder.FixedPurchaseOrderCount ?? 1,
						lastChanged,
						UpdateHistoryAction.DoNotInsert,
						accessor);

					// 定期商品購入回数（注文時点）更新（注文同梱元注文から引き継ぐ）
					orderService.UpdateFixedPerchaseItemOrderCountWhenOrderCombine(
						combinedOrderId,
						parentOrder.OrderId,
						lastChanged,
						UpdateHistoryAction.DoNotInsert,
						accessor);

					// 更新履歴登録
					if (updateHistoryAction == UpdateHistoryAction.Insert)
					{
						new UpdateHistoryService().InsertForFixedPurchase(parentOrder.FixedPurchaseId, lastChanged, accessor);
						new UpdateHistoryService().InsertForOrder(combinedOrderId, lastChanged, accessor);
					}

					accessor.CommitTransaction();
					return OrderRegisterBase.ResultTypes.Success;
				}
				else
				{
					accessor.RollbackTransaction();
					return OrderRegisterBase.ResultTypes.Fail;
				}
			}
		}

		/// <summary>
		/// 注文同梱に伴う親定期台帳更新
		/// </summary>
		/// <param name="parentOrderId">注文同梱 親注文ID</param>
		/// <param name="combinedOrgOrderIds">注文同梱 注文同梱元注文ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="combinedOrderId">注文同梱で作成された注文ID</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>定期台帳更新結果</returns>
		public static OrderRegisterBase.ResultTypes UpdateFixedPurchaseForOrderCombine(
			string parentOrderId,
			string combinedOrgOrderIds,
			string userId,
			string lastChanged,
			string combinedOrderId,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 定期台帳に注文同梱履歴追加
				var parentOrder = new OrderService().Get(parentOrderId, accessor);
				var orderService = new OrderService();

				new FixedPurchaseService().RegistHistoryForOrderCombine(
					parentOrder.FixedPurchaseId,
					combinedOrderId,
					userId,
					lastChanged,
					UpdateHistoryAction.DoNotInsert,
					accessor,
					true);

				// 注文に定期台帳の情報を追加(定期の購入回数は注文同梱元親注文の回数を引き継ぐ)
				orderService.UpdateFixedPurchaseIdAndFixedPurchaseOrderCount(
					combinedOrderId,
					parentOrder.FixedPurchaseId,
					parentOrder.FixedPurchaseOrderCount ?? 1,
					lastChanged,
					UpdateHistoryAction.DoNotInsert,
					accessor);

				// 定期商品購入回数（注文時点）更新（注文同梱元注文から引き継ぐ）
				orderService.UpdateFixedPerchaseItemOrderCountWhenOrderCombine(
					combinedOrderId,
					combinedOrgOrderIds,
					lastChanged,
					UpdateHistoryAction.DoNotInsert,
					accessor);

				// 更新履歴登録
				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertForFixedPurchase(parentOrder.FixedPurchaseId, lastChanged, accessor);
					new UpdateHistoryService().InsertForOrder(combinedOrderId, lastChanged, accessor);
				}

				accessor.CommitTransaction();
				return OrderRegisterBase.ResultTypes.Success;
			}
		}

		/// <summary>
		/// メモのマージ
		/// </summary>
		/// <param name="memo1">メモ1</param>
		/// <param name="memo2">メモ2</param>
		/// <returns>マージ後のメモ</returns>
		private static string JoinMemo(string memo1, string memo2)
		{
			var separater = (string.IsNullOrEmpty(memo1) || string.IsNullOrEmpty(memo2)) ? "" : "\r\n";
			var mergedMemo = StringUtility.ToEmpty(memo1) + separater + StringUtility.ToEmpty(memo2);

			return mergedMemo;
		}

		/// <summary>
		/// 親配送サービスが、配送サービス（宅配）リストのいずれかと合致するかチェック
		/// </summary>
		/// <param name="fixedPurchase">定期情報</param>
		/// <param name="user">ユーザー</param>
		/// <returns>配送料種別</returns>
		private static string CheckFixdPurchaseCompanyId(FixedPurchaseModel fixedPurchase)
		{
			// 商品情報セット
			var user = new UserService().Get(fixedPurchase.UserId);
			var fixedPurchaseShipping = fixedPurchase.Shippings[0];
			var deliveryCompanyId = fixedPurchaseShipping.DeliveryCompanyId;

			var product = ProductCommon.GetProductVariationInfo(
				fixedPurchaseShipping.Items[0].ShopId,
				fixedPurchaseShipping.Items[0].ProductId,
				fixedPurchaseShipping.Items[0].VariationId,
				user.MemberRankId);

			// 配送種別取得
			var shopShipping = DataCacheControllerFacade.GetShopShippingCacheController().Get((string)product[0][Constants.FIELD_PRODUCT_SHIPPING_TYPE]);

			// 配送種別の配送サービス（宅配）リスト取得
			var deliveryCompanyList = shopShipping.CompanyListExpress;

			// 親配送サービスが、配送サービス（宅配）リストのいずれかと合致するか
			if (deliveryCompanyList.Any(list => list.DeliveryCompanyId == deliveryCompanyId) == false) return (string)product[0][Constants.FIELD_PRODUCT_SHIPPING_TYPE];
			return string.Empty;
		}
	}
}
