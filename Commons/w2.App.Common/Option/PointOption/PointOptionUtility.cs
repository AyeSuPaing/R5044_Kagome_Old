/*
=========================================================================================================
  Module      : ポイントオプション共通処理クラス(PointOptionUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.CrossPoint.Point;
using w2.App.Common.DataCacheController;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Option.CrossPoint;
using w2.App.Common.Order;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.Cart;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.Point;
using w2.Domain.Point.Helper;
using w2.Domain.ProductReview;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

namespace w2.App.Common.Option
{
	/// <summary>
	/// ポイントオプションユーティリティ
	/// </summary>
	public class PointOptionUtility
	{
		#region "ポイント発行関連"

		/// <summary>
		/// ユーザーポイント情報取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="pointKbn">ポイント区分</param>
		/// <param name="cartId">カートID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザーポイント情報</returns>
		/// <remarks>ポイント区分省略時は全部もってくる</remarks>
		public static UserPointObject GetUserPoint(
			string userId,
			string pointKbn = "",
			string cartId = "",
			SqlAccessor accessor = null)
		{
			// ポイントOPが有効では無い場合Nullを返す
			if (Constants.W2MP_POINT_OPTION_ENABLED == false) return null;

			// ユーザーポイント取得 ポイント区分で絞る
			var userPoints = DomainFacade.Instance.PointService.GetUserPoint(
					userId,
					cartId: cartId,
					accessor: accessor)
				.Where(upm => (string.IsNullOrEmpty(pointKbn) || (upm.PointKbn == pointKbn)))
				.ToArray();
			return new UserPointObject(userPoints);
		}

		/// <summary>
		/// ログイン時ユーザーポイント情報のINSERT処理
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="pointRuleId">ポイントルールID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>処理件数</returns>
		public static int InsertLoginUserPoint(string userId, string pointRuleId, string lastChanged, UpdateHistoryAction updateHistoryAction)
		{
			var sv = DomainFacade.Instance.PointService;
			var rule = sv.GetPointRule(Constants.W2MP_DEPT_ID, pointRuleId);
			return sv.IssuePointByRule(rule, userId, string.Empty, rule.IncNum, lastChanged, updateHistoryAction);
		}

		/// <summary>
		/// ログイン毎ポイント付与
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="errorMessages">Error messages</param>
		public static void GiveEachLoginPoint(
			string userId,
			string lastChanged,
			ref string errorMessages,
			UpdateHistoryAction updateHistoryAction)
		{
			if (Constants.W2MP_POINT_OPTION_ENABLED == false) return;

			var pointRules = GetPointRulePriorityHigh(Constants.FLG_POINTRULE_POINT_INC_KBN_LOGIN);
			foreach (var pointRule in pointRules)
			{
				if (Constants.CROSS_POINT_OPTION_ENABLED == false)
				{
					InsertLoginUserPoint(userId, pointRule.PointRuleId, lastChanged, updateHistoryAction);
				}

				if (Constants.CROSS_POINT_OPTION_ENABLED
					&& Constants.CROSS_POINT_LOGIN_POINT_ENABLED)
				{
					var updatedUserCount = InsertLoginUserPoint(userId, pointRule.PointRuleId, lastChanged, updateHistoryAction);

					if (updatedUserCount == 0) return;

					var user = DomainFacade.Instance.UserService.Get(userId);

					// Update point api
					var result = CrossPointUtility.UpdateCrossPointApiWithWebErrorMessage(
						user,
						pointRule.IncNum,
						CrossPointUtility.GetValue(
							Constants.CROSS_POINT_SETTING_ELEMENT_REASON_ID,
							Constants.CROSS_POINT_REASON_KBN_LOGIN));

					if (result.Length > 0) errorMessages = result;
				}
			}
		}

		/// <summary>
		/// 新規登録時ユーザーポイント情報のINSERT処理
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="pointRuleId">ポイントルールID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>処理件数</returns>
		public int InsertUserRegisterUserPoint(
			string userId,
			string pointRuleId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			var sv = new PointService();
			var model = sv.GetPointRule(Constants.W2MP_DEPT_ID, pointRuleId);
			return sv.IssuePointByRule(model, userId, string.Empty, model.IncNum, lastChanged, updateHistoryAction);
		}
		/// <summary>
		/// 新規登録時ユーザーポイント情報のINSERT処理
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="pointRuleId">ポイントルールID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>処理件数</returns>
		public int InsertUserRegisterUserPoint(
			string userId,
			string pointRuleId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var sv = new PointService();
			var model = sv.GetPointRule(Constants.W2MP_DEPT_ID, pointRuleId);
			return sv.IssuePointByRule(model, userId, string.Empty, model.IncNum, lastChanged, updateHistoryAction, accessor);
		}

		/// <summary>
		/// ユーザーポイント情報のINSERT処理
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="pointRuleId">ポイントルールID</param>
		/// <param name="pointAdd">ポイント加算数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>処理件数</returns>
		public static int InsertUserPoint(
			string userId,
			string orderId,
			string pointRuleId,
			decimal pointAdd,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor)
		{
			var sv = DomainFacade.Instance.PointService;
			var model = sv.GetPointRule(Constants.W2MP_DEPT_ID, pointRuleId);
			return sv.IssuePointByRule(model, userId, orderId, pointAdd, lastChanged, updateHistoryAction, sqlAccessor);
		}

		/// <summary>
		/// ユーザーポイント情報のUPDATE処理(ポイント利用更新)
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderPointUse">利用ポイント</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="cartId">カートID</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>処理件数</returns>
		public static int UpdateOrderPointUse(
			string userId,
			string orderId,
			decimal orderPointUse,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			string cartId,
			SqlAccessor sqlAccessor)
		{
			var sv = DomainFacade.Instance.PointService;
			var updatedCount = sv.UsePointForBuy(
				Constants.W2MP_DEPT_ID,
				userId,
				orderId,
				orderPointUse,
				lastChanged,
				updateHistoryAction,
				cartId,
				sqlAccessor);

			return updatedCount;
		}

		/// <summary>
		/// 初回購入チェック
		/// </summary>
		/// <remarks>
		/// 注文同梱の元注文に初回購入ポイントを含むものがあるかどうか
		/// </remarks>
		/// <param name="userId">ユーザーID</param>
		/// <param name="combinedOrderIds">元注文ID</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>True:初回購入ポイントを含む False:初回購入ポイントを含まない</returns>
		public static bool CheckOrderFirstBuyForOrderCombine(string userId, string[] combinedOrderIds, SqlAccessor sqlAccessor = null)
		{
			var result = false;
			foreach (var orderId in combinedOrderIds)
			{
				var pointHistoryList
					= DomainFacade.Instance.PointService.GetUserPointHistoryByOrderId(userId, orderId, sqlAccessor);
				result = pointHistoryList
					.Any(x => x.PointIncKbn == Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_FIRST_BUY);
				if (result) break;
			}
			return result;
		}

		/// <summary>
		/// 商品付与ポイント取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="pointKbn">付与ポイント区分</param>
		/// <param name="productPoint">商品ポイント</param>
		/// <param name="productPrice">Product price</param>
		/// <param name="memberRankId">会員ランクID</param>
		/// <returns>商品加算ポイント</returns>
		/// <remarks>商品が持つポイントを利用するときのみ表示</remarks>
		public static string GetProductAddPointString(string shopId, string pointKbn, decimal productPoint, decimal productPrice, string memberRankId)
		{
			if (HasProductAddPoint(shopId, pointKbn))
			{
				decimal memberRankPointAddRate = MemberRankOptionUtility.GetMemberRankPointAdd(memberRankId);

				// 加算数の場合
				if (pointKbn == Constants.FLG_PRODUCT_POINT_KBN1_NUM)
				{
					if (memberRankPointAddRate == 0)	// 会員ランクごとに「加算しない(=0)」場合は加算せず計算
					{
						return StringUtility.ToNumeric(productPoint) + Constants.CONST_UNIT_POINT_PT;
					}
					else
					{
						var memberRankPointAdd = decimal.Floor(productPrice * (memberRankPointAddRate / (decimal)100));
						return StringUtility.ToNumeric(productPoint + memberRankPointAdd) + Constants.CONST_UNIT_POINT_PT;
					}
				}
				// 加算率の場合
				else if (pointKbn == Constants.FLG_PRODUCT_POINT_KBN1_RATE)
				{
					return StringUtility.ToNumeric(productPoint + memberRankPointAddRate) + "%";
				}
			}

			return "";
		}

		/// <summary>
		/// 商品が付与ポイントを持つか
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="pointKbn">付与ポイント区分</param>
		/// <returns></returns>
		public static bool HasProductAddPoint(string shopId, string pointKbn)
		{
			if (Constants.W2MP_POINT_OPTION_ENABLED == false) return false;

			// 商品マスタのポイント数を使用の場合はポイント表示
			if (IsIncType(Constants.FLG_POINTRULE_POINT_INC_KBN_BUY, Constants.FLG_POINTRULE_INC_TYPE_PRODUCT))
			{
				if (pointKbn != Constants.FLG_PRODUCT_POINT_KBN1_INVALID)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// 注文付与ポイント取得
		/// </summary>
		/// <param name="coCart">カート情報</param>
		/// <param name="strPointIncKbn">ポイント加算区分</param>
		/// <param name="fixedPurchase">定期購入情報</param>
		/// <returns>注文付与ポイント</returns>
		public static decimal GetOrderPointAdd(CartObject coCart, string strPointIncKbn, FixedPurchaseModel fixedPurchase)
		{
			string strPointRuelIncType = null;
			return GetOrderPointAdd(coCart, strPointIncKbn, out strPointRuelIncType, fixedPurchase);
		}
		/// <summary>
		/// 注文付与ポイント取得
		/// </summary>
		/// <param name="coCart">カート情報</param>
		/// <param name="strPointIncKbn">ポイント加算区分</param>
		/// <param name="strPointRuleIncType">ポイント加算方法</param>
		/// <param name="fixedPurchase">定期購入情報</param>
		/// <param name="pointRuleForIssue">ポイントルール</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>注文付与ポイント</returns>
		/// <remarks>
		/// ポイントルールを省略した場合は適用可能なポイントルール全ての合計を返します。
		/// </remarks>
		public static decimal GetOrderPointAdd(
			CartObject coCart,
			string strPointIncKbn,
			out string strPointRuleIncType,
			FixedPurchaseModel fixedPurchase = null,
			PointRuleModel pointRuleForIssue = null,
			SqlAccessor accessor = null)
		{
			//------------------------------------------------------
			// 優先度の高いポイントルール取得
			//------------------------------------------------------
			strPointRuleIncType = "";

			var pointRules = (pointRuleForIssue == null)
				? GetPointRulePriorityHigh(strPointIncKbn)
				: new[] { pointRuleForIssue };

			//------------------------------------------------------
			// ポイント数計算
			//------------------------------------------------------
			var dOrderPointAdd = 0m;
			foreach (var pointRule in pointRules)
			{
				// ポイント付与区分を設定
				strPointRuleIncType = (coCart.HasFixedPurchase && pointRule.FixedPurchasePointValid)
					? pointRule.IncFixedPurchaseType
					: pointRule.IncType;

				// 会員ランク加算率取得
				decimal dbMemberRankPointAdd = MemberRankOptionUtility.GetMemberRankPointAdd(string.IsNullOrEmpty(coCart.MemberRankId) 
						? MemberRankOptionUtility.GetDefaultMemberRank() 
						: coCart.MemberRankId);

				// 税額の再計算のためにカートのコピーを作成し、税額を再計算
				var pointCalculateCart = coCart.Copy();
				pointCalculateCart.CalculateTaxPrice(Constants.POINT_OPTION_TAXEXCLUDED_FRACTION_ROUNDING);
				// 税率毎の購入金額を算出する
				var groupedItem = pointCalculateCart.Items.GroupBy(item => item.TaxRate);
				var priceByTaxRate = groupedItem.Select(
					item => new Hashtable
					{
						{ "tax_rate" , item.Key },
						{ "price" , item.Sum(itemKey => itemKey.PriceSubtotalAfterDistribution) },
					});
				var priceAllItemSubtotalAfterDistribution = pointCalculateCart.Items.Sum(
					item => item.PriceSubtotalAfterDistribution);
				var priceAllItemSubtotalAfterDistributionTax = priceByTaxRate.Sum(
					item => TaxCalculationUtility.GetTaxPrice(
						(decimal)item["price"],
						(decimal)item["tax_rate"],
						Constants.POINT_OPTION_TAXEXCLUDED_FRACTION_ROUNDING,
						true));
				// 税別の購入金額取得
				decimal dbTargetPriceSubTotal = GetPriceSubtotal(priceAllItemSubtotalAfterDistribution, priceAllItemSubtotalAfterDistributionTax, true);

				//------------------------------------------------------
				// 購入金額毎：加算数の場合
				//------------------------------------------------------
				if (strPointRuleIncType == Constants.FLG_POINTRULE_INC_TYPE_NUM)
				{
					dOrderPointAdd += (coCart.HasFixedPurchase && pointRule.FixedPurchasePointValid) ? (decimal)pointRule.IncFixedPurchaseNum : pointRule.IncNum;

					// 購入金額＊(会員ランク加算率)付与
					decimal dbPercentage = dbMemberRankPointAdd / (decimal)100;
					dOrderPointAdd += decimal.Floor(dbTargetPriceSubTotal * dbPercentage);
				}
				//------------------------------------------------------
				// 購入金額毎：加算率の場合
				//------------------------------------------------------
				else if (strPointRuleIncType == Constants.FLG_POINTRULE_INC_TYPE_RATE)
				{
					decimal dbPercentage =
						(dbMemberRankPointAdd + ((coCart.HasFixedPurchase && pointRule.FixedPurchasePointValid)
							? (decimal)pointRule.IncFixedPurchaseRate
							: pointRule.IncRate)) / (decimal)100;

					dOrderPointAdd += decimal.Floor(dbTargetPriceSubTotal * dbPercentage);
				}
				//------------------------------------------------------
				// 商品毎（商品マスタポイント）の場合
				//------------------------------------------------------
				else if (strPointRuleIncType == Constants.FLG_POINTRULE_INC_TYPE_PRODUCT)
				{
					dOrderPointAdd += CalculatePointTypeProductRule(coCart, dbMemberRankPointAdd);
				}

				// 購入時ポイント発行の場合のみ、定期購入ポイント数計算
				if ((strPointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_BUY)
					&& (coCart.HasFixedPurchase))
				{
					// 定期購入回数 このタイミングでは定期台帳の回数が前回回数から更新されていないため+1する
					var fixedPurchaseOrderCount = (fixedPurchase == null) ? 1 : fixedPurchase.OrderCount + 1;
					var service = DomainFacade.Instance.ProductFixedPurchaseDiscountSettingService;

					foreach (var cp in coCart.Items.Where(p => p.IsFixedPurchase))
					{
						// 定期購入割引設定取得
						var discountSetting = service.GetApplyFixedPurchasePointSetting(
							cp.ShopId,
							cp.ProductId,
							(Constants.FIXEDPURCHASE_ORDER_DISCOUNT_METHOD == Constants.FLG_FIXEDPURCHASE_COUNT)
								? fixedPurchaseOrderCount
								: coCart.GetFixedPurchaseItemOrderCount(cp, accessor),
							cp.Count);
						if (discountSetting == null)
						{
							continue;
						}

						// 定期購入ポイント適用
						var fixedPointValue = discountSetting.PointValue ?? 0;
						if ((string.IsNullOrEmpty(discountSetting.PointType) == false) && (fixedPointValue > 0))
						{
							// 税抜きオプション:TRUEの場合、税抜き価格に調整する
							var amountAfterTaxAdjustment = Constants.POINT_OPTION_USE_TAX_EXCLUDED_POINT
								? Math.Floor(cp.PriceSubtotalAfterDistribution / ((100 + cp.TaxRate) / 100))
								: cp.PriceSubtotalAfterDistribution;

							dOrderPointAdd += (discountSetting.PointType == Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_POINT_TYPE_POINT)
								? fixedPointValue * cp.Count
								: Math.Floor((amountAfterTaxAdjustment * fixedPointValue) / 100);
						}
					}
				}
			}

			return dOrderPointAdd;
		}
		/// <summary>
		/// ポイント履歴情報から付与ポイントの取得
		/// </summary>
		/// <param name="userPointHistoryModels">ユーザポイント履歴</param>
		/// <param name="pointKbn">ポイント区分</param>
		/// <param name="orderId">注文ID</param>
		/// <returns>付与ポイント合計数</returns>
		public static decimal GetOrderPointAdd(UserPointHistoryModel[] userPointHistoryModels, string pointKbn, string orderId)
		{
			var result = userPointHistoryModels
				.Where(
					x =>
						(x.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP)
						&& ((x.PointIncKbn == Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_BUY)
							|| (x.PointIncKbn == Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_FIRST_BUY)
							|| (x.PointIncKbn
								== Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_RETURNEXCHANGE_ADJUST_ORDER_POINT_ADD))
						&& (x.PointKbn == pointKbn)
						&& (x.Kbn1 == orderId))
				.Sum(x => x.PointInc);
			return result;
		}

		/// <summary>
		/// 注文情報から注文付与ポイント取得
		/// </summary>
		/// <param name="orderModel">注文情報</param>
		/// <param name="pointIncKbn">ポイント加算区分</param>
		/// <param name="fixedPurchase">定期購入情報</param>
		/// <param name="pointRuleForIssue">ポイントルール</param>
		/// <returns>注文付与ポイント</returns>
		/// <remarks>
		/// ポイントルールを省略した場合は適用可能なポイントルール全ての合計を返します。
		/// </remarks>
		public static decimal GetOrderPointAddForOrder(
			OrderModel orderModel,
			string pointIncKbn,
			FixedPurchaseModel fixedPurchase = null,
			PointRuleModel pointRuleForIssue = null)
		{
			// 優先度の高いポイントルール取得
			var pointRules = (pointRuleForIssue == null)
				? GetPointRulePriorityHigh(pointIncKbn)
				: new[] { pointRuleForIssue };

			// ポイント数計算
			var orderPointAdd = 0m;
			foreach (var pointRule in pointRules)
			{
				// ポイント付与区分を設定
				var pointRuleIncType = pointRule.IncType;

				// 税別の購入金額取得
				// 会員ランク加算率取得
				var memberRankPointAdd = MemberRankOptionUtility.GetMemberRankPointAdd(orderModel.MemberRankId);

				// 税額の再計算のためにカートのコピーを作成し、税額を再計算
				var pointCalculateCart = CartObject.CreateCartByOrder(orderModel);
				pointCalculateCart.CalculateTaxPrice(Constants.POINT_OPTION_TAXEXCLUDED_FRACTION_ROUNDING);
				// 税率毎の購入金額を算出する
				var groupedItem = pointCalculateCart.Items.GroupBy(item => item.TaxRate);
				var priceByTaxRate = groupedItem.Select(
					item => new Hashtable
					{
						{ "tax_rate" , item.Key },
						{ "price" , item.Sum(itemKey => itemKey.PriceSubtotalAfterDistribution) },
					});
				var priceAllItemSubtotalAfterDistribution = pointCalculateCart.Items.Sum(
					item => item.PriceSubtotalAfterDistribution);
				var priceAllItemSubtotalAfterDistributionTax = priceByTaxRate.Sum(
					item => TaxCalculationUtility.GetTaxPrice(
						(decimal)item["price"],
						(decimal)item["tax_rate"],
						Constants.POINT_OPTION_TAXEXCLUDED_FRACTION_ROUNDING,
						true));
				// 購入金額取得
				var targetPriceSubTotal = GetPriceSubtotal(priceAllItemSubtotalAfterDistribution, priceAllItemSubtotalAfterDistributionTax, true);

				// 購入金額毎：加算数の場合
				if (pointRuleIncType == Constants.FLG_POINTRULE_INC_TYPE_NUM)
				{
					// 購入金額毎加算数付与
					orderPointAdd += pointRule.IncNum;

					// 購入金額＊(会員ランク加算率)付与
					var percentage = memberRankPointAdd / (decimal)100;
					orderPointAdd += decimal.Floor(targetPriceSubTotal * percentage);
				}
				// 購入金額毎：加算率の場合
				else if (pointRuleIncType == Constants.FLG_POINTRULE_INC_TYPE_RATE)
				{
					// 購入金額＊(会員ランク加算率＋ポイント加算率)付与
					var percentage = (memberRankPointAdd + pointRule.IncRate) / (decimal)100;
					orderPointAdd += decimal.Floor(targetPriceSubTotal * percentage);
				}
				// 商品毎（商品マスタポイント）の場合
				else if (pointRuleIncType == Constants.FLG_POINTRULE_INC_TYPE_PRODUCT)
				{
					orderPointAdd += CalculatePointTypeProductRule(pointCalculateCart, memberRankPointAdd);
				}

				// 購入時ポイント発行の場合のみ、定期購入ポイント数計算
				if ((pointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_BUY)
					&& (string.IsNullOrEmpty(orderModel.FixedPurchaseId) == false))
				{
					var service = DomainFacade.Instance.ProductFixedPurchaseDiscountSettingService;
					foreach (var shipping in orderModel.Shippings)
					{
						foreach (var item in shipping.Items.Where(p => p.FixedPurchaseProductFlg == Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON))
						{
							// 定期購入割引設定取得
							var discountSetting = service.GetApplyFixedPurchasePointSetting(
								item.ShopId,
								item.ProductId,
								(Constants.FIXEDPURCHASE_ORDER_DISCOUNT_METHOD == Constants.FLG_FIXEDPURCHASE_COUNT)
									? orderModel.FixedPurchaseOrderCount.Value
									: item.FixedPurchaseItemOrderCount.GetValueOrDefault(0),
								item.ItemQuantity);
							if (discountSetting == null)
							{
								continue;
							}

							// 定期購入ポイント適用
							var fixedPointValue = discountSetting.PointValue ?? 0;
							if ((string.IsNullOrEmpty(discountSetting.PointType) == false) && (fixedPointValue > 0))
							{
								orderPointAdd += (discountSetting.PointType == Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_POINT_TYPE_POINT)
									? fixedPointValue * item.ItemQuantity
									: Math.Floor((item.ItemPrice * fixedPointValue) / 100);
							}
						}
					}
				}
			}

			return orderPointAdd;
		}

		/// <summary>
		/// 使用ポイント按分適用後商品小計
		/// </summary>
		/// <param name="priceSubtotalDistribution">対象となる商品小計</param>
		/// <param name="priceSubtotalDistributionAll">対象となる商品合計</param>
		/// <param name="usePointPrice">使用ポイント</param>
		/// <returns>按分適用後商品小計</returns>
		public static decimal GetPriceTotalUsePoint(decimal priceSubtotalDistribution, decimal priceSubtotalDistributionAll, decimal usePointPrice)
		{
			// 商品小計(ポイント按分適用後)：商品小計 - INT((商品小計 / 商品合計) * ポイント利用額)
			return priceSubtotalDistribution - (((priceSubtotalDistribution / ((priceSubtotalDistributionAll != 0) ? priceSubtotalDistributionAll : 1)) * usePointPrice)).ToPriceDecimal(DecimalUtility.Format.RoundDown).Value;
		}

		/// <summary>
		/// ポイント付与数
		/// </summary>
		/// <param name="priceSubTotalPoint">ポイント按分後商品小計</param>
		/// <param name="addRate">加算率</param>
		/// <returns>ポイント付与数</returns>
		public static decimal GetAddPointDistribution(decimal priceSubTotalPoint, decimal addRate)
		{
			// INT(商品小計(ポイント按分適用後) * (ポイント加算率))
			return Math.Floor(priceSubTotalPoint * (addRate / (decimal)100));
		}

		/// <summary>
		/// ポイントを計算(基本ルール：商品ごとに計算)
		/// </summary>
		/// <param name="cart">カートオブジェクト</param>
		/// <param name="memberRankPointAdd">会員ランク加算率</param>
		/// <returns>ポイント付与数</returns>
		public static decimal CalculatePointTypeProductRule(CartObject cart, decimal memberRankPointAdd)
		{
			var point = 0m;
			if (cart.IsGift)
			{
				foreach (var shipping in cart.Shippings)
				{
					foreach (var product in shipping.ProductCounts.Where(pc => pc.Count > 0))
					{
						var taxPrice = TaxCalculationUtility.GetTaxPrice(
							(product.PriceSubtotalAfterDistribution / product.Count).ToPriceDecimal(DecimalUtility.Format.RoundDown).Value,
							product.Product.TaxRate,
							shipping.ShippingCountryIsoCode,
							shipping.Addr5,
							Constants.POINT_OPTION_TAXEXCLUDED_FRACTION_ROUNDING,
							true);
						var priceTax = taxPrice * product.Count;
						var priceSubTotalPoint = GetPriceSubtotal(
							product.PriceSubtotalAfterDistribution,
							priceTax,
							true);
						point += GetPointByProduct(
							priceSubTotalPoint,
							product.Product,
							product.Count,
							memberRankPointAdd);
					}
				}
			}
			else
			{
				foreach (var product in cart.Items.Where(cp => cp.Count > 0))
				{
					var taxPrice = TaxCalculationUtility.GetTaxPrice(
						(product.PriceSubtotalAfterDistribution / product.Count).ToPriceDecimal(DecimalUtility.Format.RoundDown).Value,
						product.TaxRate,
						cart.Shippings[0].ShippingCountryIsoCode,
						cart.Shippings[0].Addr5,
						Constants.POINT_OPTION_TAXEXCLUDED_FRACTION_ROUNDING,
						true);
					var priceTax = taxPrice * product.Count;
					var priceSubTotalPoint = GetPriceSubtotal(
						product.PriceSubtotalAfterDistribution,
						priceTax,
						true);
					point += GetPointByProduct(
						priceSubTotalPoint,
						product,
						product.Count,
						memberRankPointAdd);
				}
			}
			return point;
		}

		/// <summary>
		/// 商品情報から付与ポイントを計算
		/// </summary>
		/// <param name="product">商品情報</param>
		/// <param name="priceSubtotal">商品購入合計額</param>
		/// <param name="count">商品購入数</param>
		/// <param name="memberRankPointAdd">会員ランク加算率</param>
		/// <returns>ポイント付与数</returns>
		public static decimal GetPointByProduct(decimal priceSubtotal, CartProduct product, int count, decimal memberRankPointAdd)
		{
			var resultPoint = 0m;
			var productPointKbn = (product.IsFixedPurchase && (product.PointKbn2 != Constants.FLG_PRODUCT_POINT_KBN1_INVALID)) ? product.PointKbn2 : product.PointKbn1;
			var productPoint = (product.IsFixedPurchase && (product.PointKbn2 != Constants.FLG_PRODUCT_POINT_KBN1_INVALID)) ? product.Point2 : product.Point1;
			switch (productPointKbn)
			{
				// 設定なし
				case Constants.FLG_PRODUCT_POINT_KBN1_INVALID:
					break;

				// 値
				case Constants.FLG_PRODUCT_POINT_KBN1_NUM:
					// 商品毎ポイント加算数付与
					resultPoint += productPoint * count;
					// 商品小計＊(会員ランク加算率)付与
					if (product.MemberRankPointExcludeFlg != Constants.FLG_PRODUCT_CHECK_MEMBER_RANK_POINT_EXCLUDE_FLG_VALID)
					{
						resultPoint += GetAddPointDistribution(priceSubtotal, memberRankPointAdd);
					}
					break;

				// 率（%) 按分計算を行う
				case Constants.FLG_PRODUCT_POINT_KBN1_RATE:
					// 商品小計＊(会員ランク加算率＋商品毎ポイント加算率)付与
					if (product.MemberRankPointExcludeFlg != Constants.FLG_PRODUCT_CHECK_MEMBER_RANK_POINT_EXCLUDE_FLG_VALID)
					{
						resultPoint += GetAddPointDistribution(priceSubtotal, memberRankPointAdd + productPoint);
						break;
					}
					resultPoint += GetAddPointDistribution(priceSubtotal, productPoint);
					break;

				default:
					break;
			}
			
			return resultPoint;
		}

		/// <summary>
		/// 購入時ポイントの取得
		/// </summary>
		/// <param name="lCart">カート情報リスト</param>
		/// <returns></returns>
		public static decimal GetAddPoint(List<CartObject> lCart)
		{
			decimal dAddPoint = 0;

			if (lCart != null)
			{
				//------------------------------------------------------
				// 付与ポイント取得（成功したカートから）
				//------------------------------------------------------
				bool blIsAddFirstBuyPoint = false;
				foreach (CartObject co in lCart)
				{
					// 購入時ポイント追加
					dAddPoint += co.BuyPoint;

					// 初回購入ポイント追加（加算数の場合は最初の一つ、加算率の場合は全て付与）
					if ((blIsAddFirstBuyPoint == false) || (co.FirstBuyPointKbn == Constants.FLG_POINTRULE_INC_TYPE_RATE))
					{
						dAddPoint += co.FirstBuyPoint;
						blIsAddFirstBuyPoint = true;
					}
				}
			}

			return dAddPoint;
		}

		/// <summary>
		/// 税処理後の金額取得
		/// </summary>
		/// <param name="price">計算元金額</param>
		/// <param name="taxRate">税率</param>
		/// <returns>税抜金額</returns>
		public static decimal GetTaxCaluculatedPrice(decimal price, decimal taxRate)
		{
			var priceTax = TaxCalculationUtility.GetTaxPrice(price,	taxRate, Constants.POINT_OPTION_TAXEXCLUDED_FRACTION_ROUNDING);
			return GetPriceSubtotal(price, priceTax);
		}

		/// <summary>
		/// 金額と税額から商品小計を取得
		/// </summary>
		/// <param name="price">金額</param>
		/// <param name="priceTax">税額</param>
		/// <param name="taxIncludedFlag">税込みフラグ(true：税込 false：システム管理の税区分)</param>
		/// <returns>ポイント付与数</returns>
		public static decimal GetPriceSubtotal(decimal price, decimal priceTax, bool? taxIncludedFlag = null)
			{
			var isPriceTaxIncluded = taxIncludedFlag ?? Constants.MANAGEMENT_INCLUDED_TAX_FLAG;
			return Constants.POINT_OPTION_USE_TAX_EXCLUDED_POINT
				? isPriceTaxIncluded
				? TaxCalculationUtility.GetPriceTaxExcluded(price, priceTax, isPriceTaxIncluded)
					: price
				: isPriceTaxIncluded
					? price
				: TaxCalculationUtility.GetPriceTaxIncluded(price, priceTax, isPriceTaxIncluded);
		}

		/// <summary>
		/// レビュー投稿ポイント付与
		/// </summary>
		/// <param name="model">商品レビューモデル</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセス</param>
		/// <returns>エラーメッセージ</returns>
		public static string AddReviewPoint(ProductReviewModel model, string lastChanged, SqlAccessor accessor)
		{
			var pointRules = GetPointRulePriorityHigh(Constants.FLG_POINTRULE_POINT_INC_KBN_REWARD_POINT);

			if (CanAddReviewPoint(pointRules, model.UserId, model.ProductId, accessor) == false) return string.Empty;

			var sv = new PointService();
			foreach (var pointRule in pointRules)
			{
				 sv.IssuePointByRule(
					pointRule,
					model.UserId,
					string.Empty,
					pointRule.IncNum,
					lastChanged,
					UpdateHistoryAction.Insert,
					accessor,
					model.ProductId);

				// クロスポイント連携
				if (Constants.CROSS_POINT_OPTION_ENABLED)
				{
					var result = CrossPointUtility.UpdateCrossPointApiWithWebErrorMessage(
						new UserService().Get(model.UserId, accessor),
						pointRule.IncNum,
						CrossPointUtility.GetValue(
							Constants.CROSS_POINT_SETTING_ELEMENT_REASON_ID,
							Constants.CROSS_POINT_REASON_KBN_REVIEW));
					if(string.IsNullOrEmpty(result) == false) return result;
				}
			}

			// ポイント付与が成功したら、レビュー投稿のフラグをONにする
			model.ReviewRewardPointFlg = Constants.FLG_PRODUCTREVIEW_REVIEW_REWARD_POINT_FLG_VALID;
			new ProductReviewService().Update(model, accessor);

			return string.Empty;
		}

		/// <summary>
		/// レビュー投稿時のポイント付与判定
		/// </summary>
		/// <param name="pointRules">ポイントルール</param>
		/// <param name="userId">商品ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="accessor">SQLアクセス</param>
		/// <returns>true:ポイント付与する ,false:ポイント付与しない</returns>
		private static bool CanAddReviewPoint(PointRuleModel[] pointRules, string userId, string productId, SqlAccessor accessor)
		{
			if ((pointRules.Length == 0) || string.IsNullOrEmpty(userId)) return false;
			if (string.IsNullOrEmpty(Constants.REVIEW_REWARD_POINT_GRANT_LIMIT)) return true;

			var result = false;

			var userPointHistoryModels = new PointService().GetUserHistoriesByPointIncKbn(
				Constants.FLG_POINTRULE_POINT_INC_KBN_REWARD_POINT,
				userId,
				accessor);
			switch (Constants.REVIEW_REWARD_POINT_GRANT_LIMIT)
			{
				// ポイント履歴から商品IDがない場合はtrueを返す。
				case Constants.FLG_REWARD_POINT_GRANT_LIMIT_KBN_PRODUCT:
					result = (userPointHistoryModels.Any(model => model.ProductId == productId) == false);
					break;

				// ポイント履歴から「ポイント加算区分:レビューポイント発行」がない場合はtrueを返す。
				case Constants.FLG_REWARD_POINT_GRANT_LIMIT_KBN_USER:
					result = (userPointHistoryModels.Length == 0);
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(result),false,null);
			}

			return result;
		}
		/// <summary>
		/// 【EC用】レビュー投稿時のポイント付与判定
		/// </summary>
		/// <param name="model">商品レビューモデル</param>>
		/// <returns>true:ポイント付与する,false:ポイント付与しない</returns>
		public static bool CanAddReviewPoint(ProductReviewModel model)
		{
			var result = Constants.W2MP_POINT_OPTION_ENABLED
				&& Constants.REVIEW_REWARD_POINT_ENABLED
				&& (Constants.PRODUCTREVIEW_AUTOOPEN_ENABLED == false)
				&& (string.IsNullOrEmpty(model.UserId) == false)
				&& (model.IsRewardedReviewPoint == false)
				&& model.IsOpen;
			return result;
		}
		#endregion

		#region "共通"

		/// <summary>
		/// 適用可能なポイントルール取得
		/// </summary>
		/// <param name="pointIncKbn">ポイント加算区分</param>
		/// <returns>適用可能なポイントルール</returns>
		public static PointRuleModel[] GetPointRulePriorityHigh(string pointIncKbn)
		{
			// １日以上経ってたら更新（m_pointRulesを利用しているのはここだけなのでチェックもここだけ）
			if (DateTime.Now.Date > DataCacheControllerFacade.GetPointRuleCacheController().CacheData.CacheUpdateTime)
			{
				RefreshPointRuleCacheData();
			}

			var basicRule = DataCacheControllerFacade.GetPointRuleCacheController().CacheData.BasicRule.FirstOrDefault(i => (i.PointIncKbn == pointIncKbn));
			var campaignRule = DataCacheControllerFacade.GetPointRuleCacheController().CacheData.HightPriorityCampaignRule.FirstOrDefault(i => (i.PointIncKbn == pointIncKbn));

			var rules = new List<PointRuleModel>()
			{
				basicRule,
				campaignRule
			};

			if ((campaignRule != null)
				&& (campaignRule.AllowDuplicateApplyFlg == Constants.FLG_POINTRULE_DUPLICATE_APPLY_DISALLOW))
			{
				rules.Remove(basicRule);
			}

			rules.RemoveAll(rule => (rule == null));
			return rules.ToArray();
		}

		/// <summary>
		/// 優先度の高いポイントルールのポイント加算方法が「引数strIncType」かチェック
		/// </summary>
		/// <param name="strPointIncKbn">ポイント加算区分</param>
		/// <param name="strIncType">ポイント加算方法</param>
		/// <returns></returns>
		public static bool IsIncType(string strPointIncKbn, string strIncType)
		{
			// 優先度の高いポイントルール情報取得
			var pointRules = GetPointRulePriorityHigh(strPointIncKbn);
			return pointRules.Any(rule => (rule.IncType == strIncType));
		}

		/// <summary>
		/// ポイントマスタ情報データビュー取得
		/// </summary>
		/// <param name="strPointKbn">ポイント区分</param>
		/// <returns>ポイントマスタ情報データビュー</returns>
		public static PointModel[] GetPointMaster(string strPointKbn)
		{
			var master = DomainFacade.Instance.PointService.GetPointMaster().Where(pointModel => (pointModel.PointKbn == strPointKbn));
			return master.ToArray();
		}

		/// <summary>
		/// ポイントルールキャッシュの更新
		/// </summary>
		public static void RefreshPointRuleCacheData()
		{
			DataCacheControllerFacade.GetPointRuleCacheController().RefreshCacheData();
		}

		/// <summary>
		/// 注文ポイント利用額取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="orderPointUse">利用ポイント</param>
		/// <returns>注文ポイント利用額（ToPriceDecimalせずそのまま利用可能）</returns>
		public static decimal GetOrderPointUsePriceDecimal(string deptId, decimal orderPointUse)
		{
			var price = OrderPointUseHelper.GetOrderPointUsePrice(deptId, orderPointUse)
				.ToPriceDecimal(DecimalUtility.Format.RoundDown).Value;
			return price;
		}
		/// <summary>
		/// 注文ポイント利用額取得
		/// </summary>
		/// <param name="orderPointUse">利用ポイント</param>
		/// <param name="pointMaster">ポイントモデル</param>
		/// <returns>注文ポイント利用額（ToPriceDecimalせずそのまま利用可能）</returns>
		public static decimal GetOrderPointUsePriceDecimal(decimal orderPointUse, PointModel pointMaster)
		{
			var price = OrderPointUseHelper.GetOrderPointUsePrice(orderPointUse, pointMaster)
				.ToPriceDecimal(DecimalUtility.Format.RoundDown).Value;
			return price;
		}
		#endregion
	}
}
