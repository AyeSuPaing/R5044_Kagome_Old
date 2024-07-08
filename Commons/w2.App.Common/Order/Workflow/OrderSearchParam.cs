/*
=========================================================================================================
  Module      : 受注検索パラメータ(OrderSearchParam.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using w2.App.Common.OrderExtend;
using w2.App.Common.Util;
using w2.Common.Util;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.Holiday.Helper;
using w2.Domain.Order.Helper;
using Validator = w2.Common.Util.Validator;

namespace w2.App.Common.Order.Workflow
{
	/// <summary>
	/// 受注検索パラメータ
	/// </summary>
	public class OrderSearchParam
	{
		/// <summary>条件文</summary>
		public const string WORKFLOWSETTING_WHERE = "@@ where @@";
		/// <summary>The order workflow setting: Sort kbn</summary>
		public const string ORDERWORKFLOWSETTING_SORT_KBN = "sort_kbn";
		/// <summary> 検索キー：広告コード</summary>
		public const string KEY_ORDER_ADVCODE = "advcode";
		/// <summary> 検索キー：広告コードワークフロー</summary>
		public const string KEY_ORDER_ADVCODE_WORKFLOW = "advcode_workflow";


		/// <summary>
		/// 注文ワークフロー設定から検索情報取得
		/// </summary>
		/// <param name="workflowKbn">ワークフロー区分</param>
		/// <param name="workflowNo">ワークフローNO</param>
		/// <param name="masterKbn">マスタ出力の時のマスタ区分</param>
		/// <param name="condition">コンディション</param>
		/// <param name="workflowSetting">ワークフロー設定</param>
		/// <param name="useLeadTime">仕様するリードタイム</param>
		/// <param name="workflowType">ワークフロータイプ</param>
		/// <param name="shopId">ショップID</param>
		/// <returns>注文ワークフロー設定から検索情報</returns>
		public Hashtable GetSearchParamOrder(
			string workflowType,
			string shopId,
			string workflowKbn,
			string workflowNo,
			OrderListConditionForWorkflow condition,
			WorkflowSetting workflowSetting,
			bool useLeadTime,
			string masterKbn = null)
		{
			// 注文ワークフロー設定情報取得(存在しなければ空の情報を返す）
			var searchSettingsFieldValue = string.Empty;
			var displayCount = 1;
			var workflow = workflowSetting.GetOrderWorkflowSetting(shopId, workflowKbn, workflowNo, workflowType);
			if (workflow.Count != 0)
			{
				searchSettingsFieldValue = (string)workflow[0][Constants.FIELD_ORDERWORKFLOWSETTING_SEARCH_SETTING];
				displayCount = (int)workflow[0][Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_COUNT];
			}

			// パラメタ作成
			var sqlParam = new Hashtable
			{
				{ Constants.FIELD_ORDERWORKFLOWSETTING_SHOP_ID, shopId },
				{ Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_COUNT, displayCount },
				{ Constants.FIELD_ORDER_MEMO, StringUtility.ToEmpty(condition.MemoFlg) },
				{ Constants.FIELD_ORDER_MEMO + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(condition.Memo) },
				{ Constants.FIELD_ORDER_PAYMENT_MEMO, StringUtility.ToEmpty(condition.PaymentMemoFlg) },
				{ Constants.FIELD_ORDER_PAYMENT_MEMO + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(condition.PaymentMemo) },
				{ Constants.FIELD_ORDER_MANAGEMENT_MEMO, StringUtility.ToEmpty(condition.ManagementMemoFlg) },
				{ Constants.FIELD_ORDER_MANAGEMENT_MEMO + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(condition.ManagementMemo) },
				{ Constants.FIELD_ORDER_SHIPPING_MEMO, StringUtility.ToEmpty(condition.ShippingMemoFlg) },
				{ Constants.FIELD_ORDER_SHIPPING_MEMO + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(condition.ShippingMemo) },
				{ Constants.FIELD_ORDER_RELATION_MEMO, StringUtility.ToEmpty(condition.RelationMemoFlg) },
				{ Constants.FIELD_ORDER_RELATION_MEMO + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(condition.RelationMemo) },
				{ Constants.FIELD_USER_USER_MEMO, StringUtility.ToEmpty(condition.UserMemoFlg) },
				{ Constants.FIELD_USER_USER_MEMO + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(condition.UserMemo) },
				{ Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS, StringUtility.ToEmpty(condition.ProductOptionFlg) },
				{ Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(condition.ProductOptionTexts) },
				{ Constants.FIELD_ORDER_RECEIPT_FLG, StringUtility.ToEmpty(condition.ReceiptFlg) },
				{ Constants.SEARCH_FIELD_ORDER_EXTEND_NAME, condition.OrderExtendName },
				{ Constants.SEARCH_FIELD_ORDER_EXTEND_FLG, condition.OrderExtendFlg },
				{ Constants.SEARCH_FIELD_ORDER_EXTEND_TYPE, condition.OrderExtendType },
				{ Constants.SEARCH_FIELD_ORDER_EXTEND_LIKE_ESCAPED, condition.OrderExtendLikeEscaped },
				{ ORDERWORKFLOWSETTING_SORT_KBN, string.Empty },
				{ Constants.FIELD_ORDER_STOREPICKUP_STATUS, condition.StorePickupStatus }
			};

			// WHERE文作成
			// 追加検索用パラメタ取得
			var productId = StringUtility.ToEmpty(condition.ProductId);
			var productName = StringUtility.ToEmpty(condition.ProductName);
			var orderId = StringUtility.ToEmpty(condition.OrderId);
			var userId = StringUtility.ToEmpty(condition.UserId);
			var setPromotionId = StringUtility.ToEmpty(condition.SetpromotionId);
			var noveltyId = StringUtility.ToEmpty(condition.NoveltyId);
			var recommendId = StringUtility.ToEmpty(condition.RecommendId);
			var twInvoiceStatus = StringUtility.ToEmpty(condition.TwInvoiceStatus);
			var anotherShippingFlag = StringUtility.ToEmpty(condition.AnotherShippingFlag);
			var shippingStatus = StringUtility.ToEmpty(condition.ShippingStatus);
			var orderUpdateDateStatus = StringUtility.ToEmpty(condition.OrderUpdateDateStatus);
			var orderUpdateDateFrom = StringUtility.ToEmpty(condition.OrderUpdateDateFrom);
			var orderUpdateDateTo = StringUtility.ToEmpty(condition.OrderUpdateDateTo);
			var paymentOrderId = StringUtility.ToEmpty(condition.PaymentOrderId);
			var cardTranId = StringUtility.ToEmpty(condition.CardTranId);
			var updateDateExtendStatus = StringUtility.ToEmpty(condition.UpdateDateExtendStatus);
			var extendStatusNo = StringUtility.ToEmpty(condition.ExtendStatusNo);
			var extendStatus = StringUtility.ToEmpty(condition.ExtendStatus);
			var extendStatusDateFrom = StringUtility.ToEmpty(condition.ExtendStatusDateFrom);
			var extendStatusDateTo = StringUtility.ToEmpty(condition.ExtendStatusDateTo);
			var shippingPrefectures = StringUtility.ToEmpty(condition.ShippingPrefectures);
			var shippingCity = StringUtility.ToEmpty(condition.ShippingCity);
			var shippingStatusCode = StringUtility.ToEmpty(condition.ShippingStatusCode);
			var shippingCurrentStatus = StringUtility.ToEmpty(condition.ShippingCurrentStatus);
			var subscriptionBoxCourseId = StringUtility.ToEmpty(condition.SubscriptionBoxCourseId);
			var subscriptionBoxOrderCountFrom = StringUtility.ToEmpty(condition.SubscriptionBoxOrderCountFrom);
			var subscriptionBoxOrderCountTo = StringUtility.ToEmpty(condition.SubscriptionBoxOrderCountTo);
			// デフォルト設定（リスト未選択の場合はありえない条件を付加。csv出力でも使われるためここで条件設定）
			var whereString = new StringBuilder();
			if (!condition.IsSelectedByWorkflow)
			{
				sqlParam.Add(WORKFLOWSETTING_WHERE, " 1 = 0 ");

				return sqlParam;	// 未選択の時は検索しない条件で返す
			}

			whereString.Append(" 1 = 1 ");
			whereString.Append("	AND ");

			// 注文ステータス（返品交換ワークかどうかで切り替え）
			whereString.Append(Constants.TABLE_ORDER).Append(".").Append(Constants.FIELD_ORDER_ORDER_STATUS);
			whereString.Append((workflowKbn == Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_RETURN_EXCHANGE) ? " = " : " != ");
			whereString.Append("'").Append(Constants.FLG_ORDER_ORDER_STATUS_UNKNOWN).Append("' ");

			if (workflowSetting.IsAdditionalSearchFlgOn
				&& (workflowSetting.IsDetailKbnNormal || workflowSetting.IsDetailKbnReturn))
			{
				// 商品IDを条件に追加（前方一致）
				if (string.IsNullOrEmpty(productId) == false)
				{
					whereString.Append((whereString.Length == 0) ? "" : " AND ");
					whereString.Append("EXISTS ( ");
					whereString.Append("    SELECT	* ");
					whereString.Append("      FROM  w2_OrderItem ");
					whereString.Append("     WHERE  w2_OrderItem.order_id = w2_Order.order_id");
					whereString.Append("       AND  w2_OrderItem.variation_id LIKE '")
						.Append(productId.Replace("'", "''")).Append("%') ");
				}
				// 商品名を条件に追加（部分一致）
				if (string.IsNullOrEmpty(productName) == false)
				{
					whereString.Append((whereString.Length == 0) ? "" : " AND ");
					whereString.Append("EXISTS ( ");
					whereString.Append("    SELECT  * ");
					whereString.Append("      FROM  w2_OrderItem ");
					whereString.Append("     WHERE  w2_OrderItem.order_id = w2_Order.order_id");
					whereString.Append("       AND  w2_OrderItem.product_name LIKE '%")
						.Append(productName.Replace("'", "''")).Append("%') ");
				}
				// 注文IDを条件に追加（前方一致）
				if (string.IsNullOrEmpty(orderId) == false)
				{
					whereString.Append((whereString.Length == 0) ? "" : " AND ")
						.Append("(")
						.Append("w2_Order.order_id LIKE '")
						.Append(orderId.Replace("'", "''"))
						.Append("%' ");
					foreach (var splitOrderId in orderId.Split(','))
					{
						whereString.Append(" OR ")
							.Append("w2_Order.order_id = '")
							.Append(splitOrderId.Replace("'", "''"))
							.Append("'");
					}
					whereString.Append(")");
				}
				// 注文ステータス
				if (string.IsNullOrEmpty(orderUpdateDateStatus) == false)
				{
					if (Validator.IsDate(orderUpdateDateFrom)
						|| Validator.IsDate(orderUpdateDateTo))
					{
						var tempWhere = new StringBuilder();
						// ステータス更新日(From)
						if (Validator.IsDate(orderUpdateDateFrom))
						{
							tempWhere.Append("w2_Order." + orderUpdateDateStatus + " >= '")
								.Append(orderUpdateDateFrom)
								.Append("'");
						}

						// ステータス更新日(To)
						if (Validator.IsDate(orderUpdateDateTo))
						{
							tempWhere.Append(tempWhere.Length != 0 ? " AND " : " ")
								.Append("w2_Order." + orderUpdateDateStatus + " < ")
								.Append("'" + DateTime.Parse(orderUpdateDateTo).AddDays(1) + "'");
						}
						whereString.Append((whereString.Length == 0) ? "( " : " AND ( ");
						whereString.Append(tempWhere);
						whereString.Append(" )");
					}
				}

				// 決済注文IDを条件に追加（前方一致）
				if (string.IsNullOrEmpty(paymentOrderId) == false)
				{
					whereString.Append((whereString.Length == 0) ? "(" : " AND (")
						.Append("w2_Order.payment_order_id LIKE '")
						.Append(paymentOrderId.Replace("'", "''"))
						.Append("%' ")
						.Append(" OR ")
						.Append("w2_Order.payment_order_id IN ('")
						.Append(paymentOrderId.Replace("'", "''").Replace(",", "','"))
						.Append("'))");
				}

				// 決済取引IDを条件に追加（前方一致）
				if (string.IsNullOrEmpty(cardTranId) == false)
				{
					whereString.Append((whereString.Length == 0) ? "(" : " AND (")
						.Append("w2_Order.card_tran_id LIKE '")
						.Append(cardTranId.Replace("'", "''"))
						.Append("%' ")
						.Append(" OR ")
						.Append("w2_Order.card_tran_id IN ('")
						.Append(cardTranId.Replace("'", "''").Replace(",", "','"))
						.Append("'))");
				}

				// 外部決済ステータス
				if (string.IsNullOrEmpty(condition.ExternalPaymentStatus) == false)
				{
					whereString.Append((whereString.Length == 0) ? "" : " AND ");
					whereString.Append("w2_Order.external_payment_status = '")
						.Append(condition.ExternalPaymentStatus).Append("'");
				}

				// 拡張ステータス
				if ((string.IsNullOrEmpty(extendStatusNo) == false)
					&& (string.IsNullOrEmpty(extendStatus) == false))
				{
					whereString.Append((whereString.Length == 0) ? "" : " AND ")
						.Append("w2_Order.extend_status" + extendStatusNo + " = '")
						.Append(extendStatus.Replace("'", "''"))
						.Append("'");
				}

				// 拡張ステータス更新日
				if (string.IsNullOrEmpty(updateDateExtendStatus) == false)
				{
					if (Validator.IsDate(extendStatusDateFrom)
						|| Validator.IsDate(extendStatusDateTo))
					{
						var tempWhere = new StringBuilder();
						// 拡張ステータス更新日(From)
						if (Validator.IsDate(extendStatusDateFrom))
						{
							tempWhere.Append("w2_Order.extend_status_date" + updateDateExtendStatus + " >= '")
								.Append(extendStatusDateFrom)
								.Append("'");
						}

						// 拡張ステータス更新日(To)
						if (Validator.IsDate(extendStatusDateTo))
						{
							tempWhere.Append(tempWhere.Length != 0 ? " AND " : " ")
								.Append("w2_Order.extend_status_date" + updateDateExtendStatus + " < ")
								.Append("'" + DateTime.Parse(extendStatusDateTo).AddDays(1) + "'");
						}
						whereString.Append((whereString.Length == 0) ? "( " : " AND ( ");
						whereString.Append(tempWhere);
						whereString.Append(" )");
					}
				}

				// 最終与信日指定
				if (Validator.IsDate(condition.ExternalPaymentAuthDateFrom) || Validator.IsDate(condition.ExternalPaymentAuthDateTo))
				{
					var tempWhere = new StringBuilder();
					if (Validator.IsDate(condition.ExternalPaymentAuthDateFrom))
					{
						tempWhere.Append((tempWhere.Length == 0) ? " AND (" : " AND ");
						tempWhere.Append("w2_Order.external_payment_auth_date >= '")
							.Append(condition.ExternalPaymentAuthDateFrom).Append("'");
					}
					if (Validator.IsDate(condition.ExternalPaymentAuthDateTo))
					{
						tempWhere.Append((tempWhere.Length == 0) ? " AND (" : " AND ");
						tempWhere.Append("w2_Order.external_payment_auth_date < DATEADD(day, 1, '")
							.Append(condition.ExternalPaymentAuthDateTo).Append("') ");
					}
					if (condition.IsExternalPaymentAuthDateNone)
					{
						tempWhere.Append((tempWhere.Length == 0) ? " AND (" : " OR ");
						tempWhere.Append("w2_Order.external_payment_auth_date IS NULL");
					}

					tempWhere.Append(")");
					whereString.Append(tempWhere);
				}

				// 配送希望日
				if (Validator.IsDate(condition.ShippingDateFrom) || Validator.IsDate(condition.ShippingDateTo))
				{
					var tempWhere = new StringBuilder();
					if (Validator.IsDate(condition.ShippingDateFrom))
					{
						tempWhere.Append("(").Append("w2_OrderShipping.shipping_date >= '")
							.Append(condition.ShippingDateFrom).Append("'");
					}

					// HACK: 有効な日付じゃない時、単純に条件から除外するのではなく、
					// 受注一覧の検索のように補正した日付で検索する親切設計になると便利
					if (Validator.IsDate(condition.ShippingDateTo))
					{
						tempWhere.Append(tempWhere.Length != 0 ? " AND " : "(");
						tempWhere.Append("w2_OrderShipping.shipping_date < '")
							.Append(DateTime.Parse(condition.ShippingDateTo).AddDays(1)).Append("'");
					}

					tempWhere.Append(")");
					if (condition.IsShippingDate)
					{
						tempWhere.Append(" OR w2_OrderShipping.shipping_date IS NULL");
					}

					whereString.Append((whereString.Length == 0) ? "" : " AND ");
					whereString.Append("EXISTS (");
					whereString.Append("    SELECT  *");
					whereString.Append("      FROM  w2_OrderShipping");
					whereString.Append("     WHERE  w2_OrderShipping.order_id = w2_Order.order_id");
					whereString.Append("       AND  (");
					whereString.Append(tempWhere);
					whereString.Append("            ))");
				}

				// ユーザーIDを条件に追加（前方一致）
				if (string.IsNullOrEmpty(userId) == false)
				{
					whereString.Append((whereString.Length == 0) ? "" : " AND ");
					whereString.Append("w2_Order.user_id LIKE '").Append(userId.Replace("'", "''")).Append("%' ");
				}

				// セットプロモーションIDを条件に追加（前方一致）
				if (string.IsNullOrEmpty(setPromotionId) == false)
				{
					whereString.Append((whereString.Length == 0) ? "" : " AND ");
					whereString.Append("EXISTS (");
					whereString.Append("    SELECT  *");
					whereString.Append("      FROM  w2_OrderSetPromotion");
					whereString.Append("     WHERE  w2_OrderSetPromotion.order_id = w2_Order.order_id");
					whereString.Append("       AND  w2_OrderSetPromotion.setpromotion_id LIKE '")
						.Append(setPromotionId.Replace("'", "''")).Append("%') ");
				}

				// ノベルティIDを条件に追加（前方一致）
				if (string.IsNullOrEmpty(noveltyId) == false)
				{
					whereString.Append((whereString.Length == 0) ? "" : " AND ");
					whereString.Append("EXISTS ( ");
					whereString.Append("    SELECT	* ");
					whereString.Append("      FROM  w2_OrderItem ");
					whereString.Append("     WHERE  w2_OrderItem.order_id = w2_Order.order_id");
					whereString.Append("       AND  w2_OrderItem.novelty_id LIKE '")
						.Append(noveltyId.Replace("'", "''")).Append("%') ");
				}

				// レコメンドIDを条件に追加（前方一致）
				if (string.IsNullOrEmpty(recommendId) == false)
				{
					whereString.Append((whereString.Length == 0) ? "" : " AND ");
					whereString.Append("EXISTS ( ");
					whereString.Append("    SELECT	* ");
					whereString.Append("      FROM  w2_OrderItem ");
					whereString.Append("     WHERE  w2_OrderItem.order_id = w2_Order.order_id");
					whereString.Append("       AND  w2_OrderItem.recommend_id LIKE '")
						.Append(recommendId.Replace("'", "''")).Append("%') ");
				}

				// 出荷予定日
				if (Validator.IsDate(condition.ScheduledShippingDateFrom) || Validator.IsDate(condition.ScheduledShippingDateTo))
				{
					var tempWhere = new StringBuilder();
					if (Validator.IsDate(condition.ScheduledShippingDateFrom))
					{
						tempWhere.Append("(").Append("w2_OrderShipping.scheduled_shipping_date >= '")
							.Append(condition.ScheduledShippingDateFrom).Append("'");
					}

					// HACK: 有効な日付じゃない時、単純に条件から除外するのではなく、
					// 受注一覧の検索のように補正した日付で検索する親切設計になると便利
					if (Validator.IsDate(condition.ScheduledShippingDateTo))
					{
						tempWhere.Append(tempWhere.Length != 0 ? " AND " : "(");
						tempWhere.Append("w2_OrderShipping.scheduled_shipping_date < '")
							.Append(DateTime.Parse(condition.ScheduledShippingDateTo).AddDays(1)).Append("'");
					}

					tempWhere.Append(")");
					if (condition.IsScheduledShippingDate)
					{
						tempWhere.Append(" OR w2_OrderShipping.scheduled_shipping_date IS NULL");
					}

					whereString.Append((whereString.Length == 0) ? string.Empty : " AND ");
					whereString.Append("EXISTS (");
					whereString.Append("    SELECT  *");
					whereString.Append("      FROM  w2_OrderShipping");
					whereString.Append("     WHERE  w2_OrderShipping.order_id = w2_Order.order_id");
					whereString.Append("       AND  (");
					whereString.Append(tempWhere);
					whereString.Append("            ))");
				}

				// 請求書同梱フラグ
				if (string.IsNullOrEmpty(condition.InvoiceBundleFlg) == false)
				{
					whereString.Append((whereString.Length == 0) ? "" : " AND ");
					whereString.Append("w2_Order.invoice_bundle_flg = '").Append(condition.InvoiceBundleFlg).Append("'");
				}

				if (workflowSetting.IsAdditionalSearchFlgOn
					&& OrderCommon.DisplayTwInvoiceInfo()
					&& (string.IsNullOrEmpty(twInvoiceStatus) == false))
				{
					whereString.Append((whereString.Length == 0) ? string.Empty : " AND ");
					whereString.Append("EXISTS ( ");
					whereString.Append("    SELECT  w2_TwOrderInvoice.order_id ");
					whereString.Append("      FROM  w2_TwOrderInvoice ");
					whereString.Append("     WHERE  w2_TwOrderInvoice.order_id = w2_Order.order_id");
					whereString.Append("       AND  w2_TwOrderInvoice.tw_invoice_status = '").Append(twInvoiceStatus).Append("')");
				}

				// 別出荷フラグ
				if (string.IsNullOrEmpty(anotherShippingFlag) == false)
				{
					whereString.Append((whereString.Length == 0) ? string.Empty : " AND ")
						.Append("EXISTS (")
						.Append("         SELECT  w2_OrderShipping.order_id")
						.Append("           FROM  w2_OrderShipping")
						.Append("          WHERE  w2_OrderShipping.order_id = w2_Order.order_id")
						.Append("            AND  ( ")
						.Append("                   ( ")
						.Append("                      '").Append(anotherShippingFlag).Append("'= 2")
						.Append("                      AND ")
						.Append("                      w2_OrderShipping.shipping_receiving_store_flg = 1 ")
						.Append("                   ) ")
						.Append("                   OR ")
						.Append("                   ( ")
						.Append("                      '").Append(anotherShippingFlag).Append("'= 3")
						.Append("                      AND w2_OrderShipping.another_shipping_flg IN ('0','1')")
						.Append("                   ) ")
						.Append("                   OR ")
						.Append("                   ( ")
						.Append("                      '").Append(anotherShippingFlag).Append("'= 4")
						.Append("                      AND w2_OrderShipping.another_shipping_flg = 4 ")
						.Append("                   ) ")
						.Append("                   OR ")
						.Append("                   ( ")
						.Append("                      '").Append(anotherShippingFlag).Append("'<> 2")
						.Append("                      AND ")
						.Append("                      '").Append(anotherShippingFlag).Append("'<> 3")
						.Append("                      AND ")
						.Append("                      '").Append(anotherShippingFlag).Append("'<> 4")
						.Append("                      AND ")
						.Append("                      w2_OrderShipping.another_shipping_flg = '").Append(anotherShippingFlag).Append("'")
						.Append("                   ) ")
						.Append("                 ) ")
						.Append(") ");
				}

				// 配送状態
				if (string.IsNullOrEmpty(shippingStatus) == false)
				{
					whereString.Append((whereString.Length == 0) ? string.Empty : " AND ")
						.Append("EXISTS (")
						.Append("         SELECT  w2_OrderShipping.*")
						.Append("           FROM  w2_OrderShipping")
						.Append("          WHERE  w2_OrderShipping.order_id = w2_Order.order_id")
						.Append("            AND  w2_OrderShipping.shipping_status = '").Append(shippingStatus).Append("'")
						.Append(") ");
				}

				// 配送先 都道府県、市区町村
				if (string.IsNullOrEmpty(shippingPrefectures) == false)
				{
					var prefectures = PrefectureUtility.GetPrefectures(shippingPrefectures);
					if (prefectures.Length > 0)
					{
						whereString.Append((whereString.Length == 0) ? string.Empty : " AND ")
							.Append("EXISTS (")
							.Append("         SELECT  w2_OrderShipping.*")
							.Append("           FROM  w2_OrderShipping")
							.Append("          WHERE  w2_OrderShipping.order_id = w2_Order.order_id")
							.Append("            AND  w2_OrderShipping.shipping_addr1 IN ('")
							.Append(string.Join("','", prefectures))
							.Append("')");

						if (string.IsNullOrEmpty(shippingCity) == false)
						{
							whereString.Append("            AND  w2_OrderShipping.shipping_addr2 LIKE '")
								.Append(shippingCity)
								.Append("%'");
						}
						whereString.Append(")");
					}
				}
				else if (string.IsNullOrEmpty(shippingCity) == false)
				{
					whereString.Append((whereString.Length == 0) ? string.Empty : " AND ")
						.Append("EXISTS (")
						.Append("         SELECT  w2_OrderShipping.*")
						.Append("           FROM  w2_OrderShipping")
						.Append("          WHERE  w2_OrderShipping.order_id = w2_Order.order_id")
						.Append("            AND  w2_OrderShipping.shipping_addr2 LIKE '")
						.Append(shippingCity)
						.Append("%'")
							.Append(")");
				}

				// 完了状態コード
				if (string.IsNullOrEmpty(shippingStatusCode) == false)
				{
					whereString.Append((whereString.Length == 0) ? string.Empty : " AND ")
						.Append("EXISTS (")
						.Append("         SELECT  w2_OrderShipping.*")
						.Append("           FROM  w2_OrderShipping")
						.Append("          WHERE  w2_OrderShipping.order_id = w2_Order.order_id")
						.Append("            AND  w2_OrderShipping.shipping_status_code = '").Append(StringUtility.EscapeSqlString(shippingStatusCode)).Append("'")
						.Append(") ");
				}

				// 現在の状態
				if (string.IsNullOrEmpty(shippingCurrentStatus) == false)
				{
					whereString.Append((whereString.Length == 0) ? string.Empty : " AND ")
						.Append("EXISTS (")
						.Append("         SELECT  w2_OrderShipping.*")
						.Append("           FROM  w2_OrderShipping")
						.Append("          WHERE  w2_OrderShipping.order_id = w2_Order.order_id")
						.Append("            AND  w2_OrderShipping.shipping_current_status = '").Append(StringUtility.EscapeSqlString(shippingCurrentStatus)).Append("'")
						.Append(") ");
				}

				// 頒布会コースID
				if (string.IsNullOrEmpty(subscriptionBoxCourseId) == false)
				{
					whereString.Append(GetSqlAndCondition(whereString))
						.Append("EXISTS (")
						.Append("  SELECT  w2_Order.order_id")
						.Append("    FROM  w2_OrderItem")
						.Append("   WHERE  w2_OrderItem.order_id = w2_Order.order_id")
						.Append("     AND (")
						.Append("           CASE w2_Order.combined_org_order_ids")
						.Append("             WHEN  '' THEN w2_Order.subscription_box_course_id")
						.Append("             ELSE  w2_OrderItem.subscription_box_course_id")
						.Append("           END")
						.Append($"        ) LIKE '{StringUtility.SqlLikeStringSharpEscape(subscriptionBoxCourseId).Replace("'", "''")}%' ESCAPE '#'")
						.Append(")");
				}

				// 購入回数(注文基準from)
				if (string.IsNullOrEmpty(subscriptionBoxOrderCountFrom) == false)
				{
					whereString.Append(GetSqlAndCondition(whereString))
						.Append("w2_Order.order_subscription_box_order_count >= ")
						.Append(subscriptionBoxOrderCountFrom);
				}

				// 購入回数(注文基準To)
				if (string.IsNullOrEmpty(subscriptionBoxOrderCountTo) == false)
				{
					whereString.Append(GetSqlAndCondition(whereString))
						.Append("w2_Order.order_subscription_box_order_count <= ")
						.Append(subscriptionBoxOrderCountTo);
				}
			}

			// 抽出検索条件が存在する場合
			if (string.IsNullOrEmpty(searchSettingsFieldValue) == false)
			{
				var updateStatus = string.Empty;
				var statusDateTo = 0;
				var currentDate = DateTime.Now.ToString("yyyy/MM/dd");

				var searchSettingsFieldValues = searchSettingsFieldValue.Split('&');
				foreach (var searchSetting in searchSettingsFieldValues)
				{
					var searchSettingFieldValue = searchSetting.Split('=');	// パラメタと値に分割
					var searchField = searchSettingFieldValue[0];				// 値(カンマ区切りの連続した値)
					var searchValue = searchSettingFieldValue[1];				// 値(カンマ区切りの連続した値)

					var whereInner = new StringBuilder();

					// 通常注文の場合(返品交換ワーク以外)
					if (workflowKbn != Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_RETURN_EXCHANGE)
					{
						switch (searchField)
						{
							// 注文区分
							case Constants.FIELD_ORDER_ORDER_KBN:
							// 注文ステータス
							case Constants.FIELD_ORDER_ORDER_STATUS:
							// 入金ステータス
							case Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS:
							// 督促ステータス
							case Constants.FIELD_ORDER_DEMAND_STATUS:
							// 在庫引当ステータス
							case Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS:
							// 出荷ステータス
							case Constants.FIELD_ORDER_ORDER_SHIPPED_STATUS:
							// 注文拡張ステータス1～50
							case Constants.FIELD_ORDER_EXTEND_STATUS1:
							case Constants.FIELD_ORDER_EXTEND_STATUS2:
							case Constants.FIELD_ORDER_EXTEND_STATUS3:
							case Constants.FIELD_ORDER_EXTEND_STATUS4:
							case Constants.FIELD_ORDER_EXTEND_STATUS5:
							case Constants.FIELD_ORDER_EXTEND_STATUS6:
							case Constants.FIELD_ORDER_EXTEND_STATUS7:
							case Constants.FIELD_ORDER_EXTEND_STATUS8:
							case Constants.FIELD_ORDER_EXTEND_STATUS9:
							case Constants.FIELD_ORDER_EXTEND_STATUS10:
							case Constants.FIELD_ORDER_EXTEND_STATUS11:
							case Constants.FIELD_ORDER_EXTEND_STATUS12:
							case Constants.FIELD_ORDER_EXTEND_STATUS13:
							case Constants.FIELD_ORDER_EXTEND_STATUS14:
							case Constants.FIELD_ORDER_EXTEND_STATUS15:
							case Constants.FIELD_ORDER_EXTEND_STATUS16:
							case Constants.FIELD_ORDER_EXTEND_STATUS17:
							case Constants.FIELD_ORDER_EXTEND_STATUS18:
							case Constants.FIELD_ORDER_EXTEND_STATUS19:
							case Constants.FIELD_ORDER_EXTEND_STATUS20:
							case Constants.FIELD_ORDER_EXTEND_STATUS21:
							case Constants.FIELD_ORDER_EXTEND_STATUS22:
							case Constants.FIELD_ORDER_EXTEND_STATUS23:
							case Constants.FIELD_ORDER_EXTEND_STATUS24:
							case Constants.FIELD_ORDER_EXTEND_STATUS25:
							case Constants.FIELD_ORDER_EXTEND_STATUS26:
							case Constants.FIELD_ORDER_EXTEND_STATUS27:
							case Constants.FIELD_ORDER_EXTEND_STATUS28:
							case Constants.FIELD_ORDER_EXTEND_STATUS29:
							case Constants.FIELD_ORDER_EXTEND_STATUS30:
							case Constants.FIELD_ORDER_EXTEND_STATUS31:
							case Constants.FIELD_ORDER_EXTEND_STATUS32:
							case Constants.FIELD_ORDER_EXTEND_STATUS33:
							case Constants.FIELD_ORDER_EXTEND_STATUS34:
							case Constants.FIELD_ORDER_EXTEND_STATUS35:
							case Constants.FIELD_ORDER_EXTEND_STATUS36:
							case Constants.FIELD_ORDER_EXTEND_STATUS37:
							case Constants.FIELD_ORDER_EXTEND_STATUS38:
							case Constants.FIELD_ORDER_EXTEND_STATUS39:
							case Constants.FIELD_ORDER_EXTEND_STATUS40:
							case Constants.FIELD_ORDER_EXTEND_STATUS41:
							case Constants.FIELD_ORDER_EXTEND_STATUS42:
							case Constants.FIELD_ORDER_EXTEND_STATUS43:
							case Constants.FIELD_ORDER_EXTEND_STATUS44:
							case Constants.FIELD_ORDER_EXTEND_STATUS45:
							case Constants.FIELD_ORDER_EXTEND_STATUS46:
							case Constants.FIELD_ORDER_EXTEND_STATUS47:
							case Constants.FIELD_ORDER_EXTEND_STATUS48:
							case Constants.FIELD_ORDER_EXTEND_STATUS49:
							case Constants.FIELD_ORDER_EXTEND_STATUS50:
							case Constants.FIELD_ORDER_ORDER_PAYMENT_KBN: // 決済種別
							case Constants.FIELD_ORDER_SHIPPING_ID: // 配送種別
							case Constants.FIELD_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG: // 配送料の別途見積もりフラグ
							case Constants.FIELD_ORDER_MALL_ID: // サイト
							case Constants.FIELD_ORDER_GIFT_FLG: // ギフトフラグ
							case Constants.FIELD_ORDER_DIGITAL_CONTENTS_FLG: // デジタルコンテンツ商品フラグ
							case Constants.FIELD_ORDER_SHIPPED_CHANGED_KBN: // 出荷後変更区分
							case Constants.FIELD_ORDER_EXTERNAL_IMPORT_STATUS: // 外部連携ステータス
							case Constants.FIELD_ORDER_MALL_LINK_STATUS: // モール連携ステータス
							case Constants.FIELD_ORDER_RECEIPT_FLG: // 領収書希望フラグ
							case Constants.FIELD_ORDER_RECEIPT_OUTPUT_FLG: // 領収書出力フラグ
							case Constants.FIELD_ORDER_INVOICE_BUNDLE_FLG: // 請求書同梱フラグ
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									whereInner.Append("(").Append(Constants.TABLE_ORDER).Append(".")
										.Append(searchField)
										.Append(" IN ('")
										.Append(searchValue.Replace(",", "','"))
										.Append("'))");
								}
								break;

							// 注文者区分
							case Constants.FIELD_ORDEROWNER_OWNER_KBN:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									whereInner.Append("(")
										.Append(Constants.TABLE_ORDEROWNER)
										.Append(".")
										.Append(searchField)
										.Append(" IN ('")
										.Append(searchValue
											.Replace(",", "','"))
										.Append("'))");
								}
								break;

							// 配送伝票番号
							case Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									whereInner.Append("EXISTS (");
									whereInner.Append("         SELECT  w2_OrderShipping.order_id");
									whereInner.Append("           FROM  w2_OrderShipping");
									whereInner.Append("          WHERE  w2_OrderShipping.order_id = w2_Order.order_id");
									whereInner.Append("       GROUP BY  w2_OrderShipping.order_id");
									whereInner.Append("         HAVING  (");
									StringBuilder whereHaving = new StringBuilder();
									foreach (string searchStatus in searchValue.Split(','))
									{
										switch (searchStatus)
										{
											// strSearchValue = "0" : 登録なし
											case "0":
												whereHaving.Append(((whereHaving.Length != 0) ? " OR " : ""))
													.Append("(COUNT(CASE WHEN w2_OrderShipping.shipping_check_no != '' THEN w2_OrderShipping.shipping_check_no END) = 0)");
												break;

											// strSearchValue = "1" : 登録あり
											case "1":
												whereHaving.Append(((whereHaving.Length != 0) ? " OR " : ""))
													.Append("(COUNT(*) = COUNT(CASE WHEN w2_OrderShipping.shipping_check_no != '' THEN w2_OrderShipping.shipping_check_no END))");
												break;

											// strSearchValue = "2" : 一部登録あり
											case "2":
												whereHaving.Append(((whereHaving.Length != 0) ? " OR " : ""))
													.Append("(COUNT(*) != COUNT(CASE WHEN w2_OrderShipping.shipping_check_no != '' THEN w2_OrderShipping.shipping_check_no END)");
												whereHaving.Append(" AND  COUNT(CASE WHEN w2_OrderShipping.shipping_check_no != '' THEN w2_OrderShipping.shipping_check_no END) > 0)");
												break;
										}
									}
									whereInner.Append(whereHaving);
									whereInner.Append("  )");
									whereInner.Append(")");
								}
								break;

							// 別出荷フラグ
							case Constants.FIELD_ORDERSHIPPING_ANOTHER_SHIPPING_FLG:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									whereInner.Append("EXISTS (");
									whereInner.Append("         SELECT  w2_OrderShipping.order_id");
									whereInner.Append("           FROM  w2_OrderShipping");
									whereInner.Append("          WHERE  w2_OrderShipping.order_id = w2_Order.order_id");
									whereInner.Append("            AND");
									whereInner.Append("            (");
									whereInner.Append("              (");
									whereInner.Append("                '").Append(searchValue).Append("' = 2");
									whereInner.Append("                AND");
									whereInner.Append("                w2_OrderShipping.shipping_receiving_store_flg = 1");
									whereInner.Append("              )");
									whereInner.Append("              OR");
									whereInner.Append("              (");
									whereInner.Append("                '").Append(searchValue).Append("' = 3");
									whereInner.Append("                AND w2_OrderShipping.another_shipping_flg IN ('0','1')");
									whereInner.Append("              )");
									whereInner.Append("              OR");
									whereInner.Append("              (");
									whereInner.Append("                '").Append(searchValue).Append("' = 4");
									whereInner.Append("                AND w2_OrderShipping.another_shipping_flg = 4");
									whereInner.Append("              )");
									whereInner.Append("              OR");
									whereInner.Append("              (");
									whereInner.Append("                '").Append(searchValue).Append("' <> 2");
									whereInner.Append("                AND");
									whereInner.Append("                '").Append(searchValue).Append("' <> 4");
									whereInner.Append("                AND");
									whereInner.Append("                w2_OrderShipping.another_shipping_flg = '").Append(searchValue).Append("'");
									whereInner.Append("              )");
									whereInner.Append("            )");
									whereInner.Append(")");
								}
								break;

							// 配送方法
							case Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									whereInner.Append("EXISTS (");
									whereInner.Append("         SELECT  w2_OrderShipping.order_id");
									whereInner.Append("           FROM  w2_OrderShipping");
									whereInner.Append("          WHERE  w2_OrderShipping.order_id = w2_Order.order_id");
									whereInner.Append("                 AND w2_OrderShipping.shipping_method IN ('")
										.Append(searchValue.Replace(",", "','")).Append("')");
									whereInner.Append(")");
								}
								break;

							// 配送会社
							case Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									whereInner.Append("EXISTS (");
									whereInner.Append("         SELECT  w2_OrderShipping.order_id");
									whereInner.Append("           FROM  w2_OrderShipping");
									whereInner.Append("          WHERE  w2_OrderShipping.order_id = w2_Order.order_id");
									whereInner.Append("                 AND w2_OrderShipping.delivery_company_id IN ('")
										.Append(searchValue.Replace(",", "','")).Append("')");
									whereInner.Append(")");
								}
								break;

							// 配送先：国
							case Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									whereInner.Append("EXISTS (");
									whereInner.Append("         SELECT  w2_OrderShipping.order_id");
									whereInner.Append("           FROM  w2_OrderShipping");
									whereInner.Append("          WHERE  w2_OrderShipping.order_id = w2_Order.order_id");
									whereInner.Append("                 AND w2_OrderShipping.shipping_country_iso_code IN ('")
										.Append(searchValue.Replace(",", "','"))
										.Append("')");
									whereInner.Append(")");
								}
								break;

							// 配送先 都道府県
							case Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_SHIPPING_PREFECTURES:
								var prefectures = PrefectureUtility.GetPrefectures(searchValue);
								if (prefectures.Length > 0)
								{
									whereInner.Append("EXISTS (");
									whereInner.Append("         SELECT  w2_OrderShipping.order_id");
									whereInner.Append("           FROM  w2_OrderShipping");
									whereInner.Append("          WHERE  w2_OrderShipping.order_id = w2_Order.order_id");
									whereInner.Append("            AND  w2_OrderShipping.shipping_addr1 IN ('")
										.Append(string.Join("','", prefectures))
										.Append("')");
									whereInner.Append(")");
								}
								break;

							case Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_SHIPPING_CITY:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									whereInner.Append("EXISTS (");
									whereInner.Append("         SELECT  w2_OrderShipping.order_id");
									whereInner.Append("           FROM  w2_OrderShipping");
									whereInner.Append("          WHERE  w2_OrderShipping.order_id = w2_Order.order_id");
									whereInner.Append("            AND  w2_OrderShipping.shipping_addr2 LIKE '")
										.Append(searchValue)
										.Append("%'");
									whereInner.Append(")");
								}
								break;

							// 並び順
							case Constants.REQUEST_KEY_SORT_KBN:
								sqlParam[ORDERWORKFLOWSETTING_SORT_KBN] = searchValue;
								break;

							// ステータス更新日(ステータス)
							case WorkflowSetting.m_FIELD_UPDATE_STATUS:
								updateStatus = searchValue;
								break;

							// ステータス更新日(指定方法)
							case WorkflowSetting.m_FIELD_UPDATE_STATUS_DAY:
								if ((string.IsNullOrEmpty(searchValue) == false)
									&& (searchValue != WorkflowSetting.m_SEARCH_STATUS_DATE_FROMTO))
								{
									KeyValuePair<string, string> kvpUpdateStatusDateFromTo =
										GetUpdateStatusDateFromTo(WorkflowSetting.m_FIELD_UPDATE_STATUS, searchValue);
									whereInner.Append("(").Append(updateStatus).Append(" >= '")
										.Append(kvpUpdateStatusDateFromTo.Key).Append("')");
									whereInner.Append(" AND (").Append(updateStatus).Append(" < DateAdd(day,1,'")
										.Append(kvpUpdateStatusDateFromTo.Value).Append("'))");
								}
								break;

							// ステータス更新日(期間From)
							case WorkflowSetting.m_FIELD_UPDATE_STATUS_FROM:
								statusDateTo = -(int.Parse(searchValue));
								whereInner.Append("(").Append(updateStatus).Append(" < DateAdd(day,")
									.Append((statusDateTo + 1).ToString()).Append(",'").Append(currentDate).Append("'))");
								break;

							// ステータス更新日(期間To)
							case WorkflowSetting.m_FIELD_UPDATE_STATUS_TO:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									statusDateTo += -(int.Parse(searchValue));	// 日付を加算
									whereInner.Append("(").Append(updateStatus).Append(" >= DateAdd(day,")
										.Append(statusDateTo.ToString())
										.Append(",'")
										.Append(currentDate)
										.Append(" ")
										.Append(GetUpdateStatusTimeTo(
											searchSettingsFieldValues,
											WorkflowSetting.m_FIELD_UPDATE_STATUS_TIME))
										.Append("'))");
								}
								break;

							// 外部決済ステータス：クレジットカード
							case WorkflowSetting.m_ORDER_EXTERNAL_PAYMENT_STATUS_CARD:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									var inValue = string.Join(",", searchValue.Split(',').Select(x => string.Format("'{0}'", x)));

									whereString.Append(
										string.Format(
											" AND ( (w2_Order.order_payment_kbn <> '{0}') OR( w2_Order.order_payment_kbn = '{0}' AND w2_Order.external_payment_status IN ({1}) ) )  ",
											Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
											inValue));
								}
								break;

							// 外部決済ステータス：コンビニ(後払い)
							case WorkflowSetting.m_ORDER_EXTERNAL_PAYMENT_STATUS_CVS:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									var inValue = string.Join(",", searchValue.Split(',').Select(x => string.Format("'{0}'", x)));

									whereString.Append(
										string.Format(
											" AND ( (w2_Order.order_payment_kbn <> '{0}') OR( w2_Order.order_payment_kbn = '{0}' AND w2_Order.external_payment_status IN ({1}) ) )  ",
											Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
											inValue));
								}
								break;

							// 外部決済ステータス：台湾後払い
							case WorkflowSetting.m_ORDER_EXTERNAL_PAYMENT_STATUS_TRYLINK_AFTERPAY:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									var inValue = string.Join(",", searchValue.Split(',').Select(x => string.Format("'{0}'", x)));

									whereString.Append(
										string.Format(
											" AND ( (w2_Order.order_payment_kbn <> '{0}') OR( w2_Order.order_payment_kbn = '{0}' AND w2_Order.external_payment_status IN ({1}) ) )  ",
											Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY,
											inValue));
								}
								break;

							// Order external payment status for all payment
							case Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									var inValue = string.Join(",", searchValue.Split(',').Select(item => string.Format("'{0}'", item)));

									whereString.Append(
										string.Format("AND (w2_Order.external_payment_status IN ({0}))", inValue));
								}
								break;

							// EcPay
							case WorkflowSetting.m_ORDER_EXTERNAL_PAYMENT_STATUS_ECPAY:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									var result = string.Join(
										",",
										searchValue.Split(',').Select(value => string.Format("'{0}'", value)));
									var query = "AND ( (w2_Order.order_payment_kbn = '{0}')"
										+ " AND w2_Order.external_payment_type IN ({1}) )  ";
									whereString.Append(
										string.Format(
											query,
											Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY,
											result));
								}
								break;

							// 最終与信日指定
							case Constants.FIELD_ORDER_EXTERNAL_PAYMENT_AUTH_DATE:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									var tempWhere = new StringBuilder();
									var hasNumberOfDays = false;
									foreach (var value in searchValue.Split(','))
									{
										switch (value)
										{
											case WorkflowSetting.m_LAST_AUTH_DATE_SPECIFIED:
												if (hasNumberOfDays) continue;
												tempWhere.Append((tempWhere.Length != 0) ? " OR " : "")
													.Append("w2_Order.external_payment_auth_date IS NOT NULL");
												break;

											case WorkflowSetting.m_LAST_AUTH_DATE_UNSPECIFIED:
												tempWhere.Append((tempWhere.Length != 0) ? " OR " : "")
													.Append("w2_Order.external_payment_auth_date IS NULL");
												break;

											default:
												// 最終与信日指定有りの場合に検索範囲を指定する
												var valueDays = value.Split(WorkflowSetting.m_LAST_AUTH_DATE_SEPARATOR_CHARACTER);
												if (valueDays.Length > 1)
												{
													int dayFrom, dayTo;
													var hasDayFrom = int.TryParse(valueDays[0], out dayFrom);
													var hasDayTo = int.TryParse(valueDays[1], out dayTo);

													hasNumberOfDays = (hasDayFrom || hasDayTo);

													if (hasDayFrom)
													{
														tempWhere.Append(
															string.Format(
																"w2_Order.external_payment_auth_date >= '{0}'",
																(dayFrom == 0)
																	? DateTime.Now.Date
																	: DateTime.Now.AddDays(-dayFrom).Date));
													}

													if (hasDayTo)
													{
														if (tempWhere.Length > 0) tempWhere.Append(" AND ");
														tempWhere.Append(
															string.Format("w2_Order.external_payment_auth_date < DATEADD(day, 1, '{0}')",
															(dayTo == 0) ? DateTime.Now.Date : DateTime.Now.AddDays(-dayTo).Date));
													}
													else
													{
														if (tempWhere.Length > 0) tempWhere.Append(" AND ");
														tempWhere.Append(
															string.Format(
																"w2_Order.external_payment_auth_date < DATEADD(day, 1, '{0}')",
																DateTime.Now.Date));
													}
												}
												break;
										}
									}

									if (tempWhere.Length != 0)
									{
										whereString.Append((whereString.Length == 0) ? "" : " AND (");
										whereString.Append(tempWhere).Append(")");
									}
								}
								break;

							// 配送希望日（配送希望日指定有無もあわせて）
							case Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									var tempWhere = new StringBuilder();
									var hasNumberOfDays = false;
									foreach (var value in searchValue.Split(','))
									{
										switch (value)
										{
											case WorkflowSetting.m_SHIPPINGDATE_SPECIFIED:
												if (hasNumberOfDays) continue;
												tempWhere.Append((tempWhere.Length != 0) ? " OR " : "")
													.Append("w2_OrderShipping.shipping_date IS NOT NULL");
												break;

											case WorkflowSetting.m_SHIPPINGDATE_UNSPECIFIED:
												tempWhere.Append((tempWhere.Length != 0) ? " OR " : "")
													.Append("w2_OrderShipping.shipping_date IS NULL");
												break;

											default:
												var valueDays = value.Split(WorkflowSetting.m_SHIPPINGDATE_SEPARATOR_CHARACTER);

												if (valueDays.Length > 1)
												{
													// 配送希望日の日数指定有りと判断
													int dayFrom, dayTo;
													var hasDayFrom = int.TryParse(valueDays[0], out dayFrom);
													var hasDayTo = int.TryParse(valueDays[1], out dayTo);
													var presentDay = DateTime.Now.Date;

													hasNumberOfDays = (hasDayFrom || hasDayTo);

													if (hasDayFrom)
													{
														tempWhere.Append(
															string.Format(
																"w2_OrderShipping.shipping_date >= '{0}'",
																(dayFrom == 0) ? presentDay : (presentDay.AddDays(dayFrom))));
													}

													if (hasDayTo)
													{
														if (tempWhere.Length > 0) tempWhere.Append(" AND ");
														tempWhere.Append(
															string.Format("w2_OrderShipping.shipping_date <= '{0}'",
																(dayTo == 0) ? presentDay : (presentDay.AddDays(dayTo))));
													}
												}
												break;
										}
									}

									if (tempWhere.Length != 0)
									{
										whereString.Append((whereString.Length == 0) ? "" : " AND ");
										whereString.Append("EXISTS ( ");
										whereString.Append("                      SELECT  * ");
										whereString.Append("                        FROM  w2_OrderShipping ");
										whereString.Append("                       WHERE  w2_OrderShipping.order_id = w2_Order.order_id");
										whereString.Append("                         AND  (");
										whereString.Append(tempWhere);
										whereString.Append("))");
									}
								}
								break;

							// 楽天ポイント利用方法(外部連携メモ)
							case WorkflowSetting.m_FIELD_RAKUTEN_POINT_USE_TYPE:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									foreach (var searchWord in searchValue.Split(','))
									{
										whereInner.Append(((whereInner.Length == 0) ? "((" : "OR ("))
											.Append(Constants.TABLE_ORDER)
											.Append(".")
											.Append(Constants.FIELD_ORDER_RELATION_MEMO)
											.Append(" LIKE '%")
											.Append(searchWord.Replace("[", "[ [ ]"))
											.Append("%' )"); // XMLの予約語をエスケープ
									}
									whereInner.Append((whereInner.Length != 0) ? ")" : "");
								}
								break;

							// 定期購買注文
							case Constants.FIELD_ORDER_FIXED_PURCHASE_ID:
								if ((string.IsNullOrEmpty(searchValue) == false)
									&& ((searchValue.Contains("1")
										&& searchValue.Contains("0")) == false))
								{
									if (searchValue.Contains("1"))
									{
										whereInner.Append("(LEN(")
											.Append(Constants.TABLE_ORDER)
											.Append(".")
											.Append(Constants.FIELD_ORDER_FIXED_PURCHASE_ID)
											.Append(") <> 0)");
									}
									if (searchValue.Contains("0"))
									{
										whereInner.Append("(LEN(").Append(Constants.TABLE_ORDER).Append(".")
											.Append(Constants.FIELD_ORDER_FIXED_PURCHASE_ID).Append(") = 0)");
									}
								}
								break;

							// 定期購入回数(注文時点)
							case Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT:
								int orderCountFrom;
								int orderCountTo;
								var orderCountFromValid = int.TryParse(searchValue.Split(',')[0], out orderCountFrom);
								if (orderCountFromValid)
								{
									whereInner.Append("(")
										.Append(Constants.TABLE_ORDER)
										.Append(".")
										.Append(Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT)
										.Append(" >= ")
										.Append(orderCountFrom)
										.Append(")");
								}
								if (int.TryParse(searchValue.Split(',')[1], out orderCountTo))
								{
									if (orderCountFromValid) whereInner.Append(" AND ");
									whereInner.Append("(")
										.Append(Constants.TABLE_ORDER)
										.Append(".")
										.Append(Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT)
										.Append(" <= ")
										.Append(orderCountTo)
										.Append(")");
								}
								break;

							// 定期購入回数(出荷時点)
							case Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT:
								int shippedCountFrom;
								int shippedCountTo;
								var shippedCountFromValid = int.TryParse(searchValue.Split(',')[0], out shippedCountFrom);
								if (shippedCountFromValid)
								{
									whereInner.Append("(")
										.Append(Constants.TABLE_ORDER)
										.Append(".")
										.Append(Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT)
										.Append(" >= ")
										.Append(shippedCountFrom)
										.Append(")");
								}
								if (int.TryParse(searchValue.Split(',')[1], out shippedCountTo))
								{
									if (shippedCountFromValid) whereInner.Append(" AND ");
									whereInner.Append("(")
										.Append(Constants.TABLE_ORDER)
										.Append(".")
										.Append(Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT)
										.Append(" <= ")
										.Append(shippedCountTo)
										.Append(")");
								}
								break;

							// 各メモ(注文メモ、管理メモ、配送メモ、決済連携メモ、外部連携メモ)
							case Constants.FIELD_ORDER_MEMO:
							case Constants.FIELD_ORDER_MANAGEMENT_MEMO:
							case Constants.FIELD_ORDER_SHIPPING_MEMO:
							case Constants.FIELD_ORDER_PAYMENT_MEMO:
							case Constants.FIELD_ORDER_RELATION_MEMO:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									whereInner.Append("(");
									foreach (var value in searchValue.Split(','))
									{
										whereInner.Append(((whereInner.Length < 2) ? string.Empty : " OR "));
										switch (value.Trim())
										{
											case "1":
												whereInner.Append("(");
												whereInner.Append(Constants.TABLE_ORDER).Append(".").Append(searchField).Append(" != ''");
												whereInner.Append(" AND ");
												whereInner.Append(
													"w2_Order."
													+ searchField
													+ " LIKE '%' + '"
													+ StringUtility.SqlLikeStringSharpEscape(
														GetSearchSettingValue(
															searchSettingsFieldValues,
															searchField + "_search_text"))
														.Replace("'", "''")
													+ "' + '%' ESCAPE '#' ");
												whereInner.Append(")");
												break;

											case "0":
												whereInner.Append(Constants.TABLE_ORDER)
													.Append(".")
													.Append(searchField)
													.Append(" = ''");
												break;
										}
									}
									whereInner.Append(")");
								}
								break;

							// User Memo
							case Constants.FIELD_USER_USER_MEMO:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									whereInner.Append("(");
									foreach (var value in searchValue.Split(','))
									{
										whereInner.Append(((whereInner.Length < 2) ? string.Empty : " OR "));
										switch (value.Trim())
										{
											case "1":
												whereInner.Append(Constants.TABLE_USER)
													.Append(".").Append(searchField).Append(" <> ''");
												break;

											case "0":
												whereInner.Append(Constants.TABLE_USER)
													.Append(".").Append(searchField).Append(" = ''");
												break;
										}
									}
									whereInner.Append(")");
								}
								break;

							// 商品付帯情報
							case Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									whereInner.Append("(");
									foreach (var value in searchValue.Split(','))
									{
										whereInner.Append(((whereInner.Length < 2) ? string.Empty : " OR "));
										switch (value.Trim())
										{
											case "1":
												whereInner.Append(" EXISTS (SELECT * FROM ").Append(Constants.TABLE_ORDERITEM);
												whereInner.Append(" WHERE ")
													.Append(Constants.TABLE_ORDERITEM)
													.Append(".order_id = ")
													.Append(Constants.TABLE_ORDER)
													.Append(".order_id AND (");
												whereInner.Append(Constants.TABLE_ORDERITEM)
													.Append(".")
													.Append(searchField)
													.Append(" <> ''))");
												break;

											case "0":
												whereInner.Append(" NOT EXISTS (SELECT * FROM ").Append(Constants.TABLE_ORDERITEM);
												whereInner.Append(" WHERE ")
													.Append(Constants.TABLE_ORDERITEM)
													.Append(".order_id = ")
													.Append(Constants.TABLE_ORDER)
													.Append(".order_id AND (");
												whereInner.Append(Constants.TABLE_ORDERITEM)
													.Append(".")
													.Append(searchField)
													.Append(" <> ''))");
												break;
										}
									}
									whereInner.Append(")");
								}
								break;

							// 合計金額
							case Constants.FIELD_ORDER_ORDER_PRICE_TOTAL:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									whereInner.Append("(")
										.Append(Constants.TABLE_ORDER)
										.Append(".")
										.Append(searchField)
										.Append(">=")
										.Append(searchValue)
										.Append(")");
								}
								break;

							// 商品ID
							case Constants.FIELD_ORDERITEM_PRODUCT_ID:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									whereString.Append((whereString.Length == 0) ? "" : " AND ");
									whereString.Append("EXISTS ( ");
									whereString.Append("SELECT * ");
									whereString.Append("FROM w2_OrderItem ");
									whereString.Append("WHERE w2_OrderItem.order_id = w2_Order.order_id");
									whereString.Append("  AND w2_OrderItem.product_id IN ('")
										.Append(searchValue.Replace(",", "','"))
										.Append("'))");
								}
								break;

							// セットプロモーションID
							case Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_ID:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									whereString.Append((whereString.Length == 0) ? "" : " AND ");
									whereString.Append("EXISTS (");
									whereString.Append("    SELECT  *");
									whereString.Append("      FROM  w2_OrderSetPromotion");
									whereString.Append("     WHERE  w2_OrderSetPromotion.order_id = w2_Order.order_id");
									whereString.Append("       AND  w2_OrderSetPromotion.setpromotion_id = '")
										.Append(searchValue.Replace("'", "''"))
										.Append("') ");
								}
								break;

							// ノベルティID
							case Constants.FIELD_ORDERITEM_NOVELTY_ID:
								// ノベルティOPが有効の場合
								if ((Constants.NOVELTY_OPTION_ENABLED) && (string.IsNullOrEmpty(searchValue) == false))
								{
									var isSpecified = false;
									var isUnspecified = false;
									var noveltyIds = new List<string>();
									searchValue.Split(',').ToList()
										.ForEach(value =>
										{
											switch (value)
											{
												case WorkflowSetting.m_NOVELTY_ID_SPECIFIED:
													isSpecified = true;
													break;
												case WorkflowSetting.m_NOVELTY_ID_UNSPECIFIED:
													isUnspecified = true;
													break;
												default:
													noveltyIds.Add(value);
													break;
											}
										});
									// あり + なし指定の場合は、処理を抜ける
									if (isSpecified && isUnspecified) break;
									// あり?
									if (isSpecified)
									{
										whereString.Append((whereString.Length == 0) ? "" : " AND ");
										whereString.Append("EXISTS ( ");
										whereString.Append("                      SELECT  * ");
										whereString.Append("                        FROM  w2_OrderItem ");
										whereString.Append("                       WHERE  w2_OrderItem.order_id = w2_Order.order_id");
										whereString.Append("                         AND  w2_OrderItem.novelty_id <> ''");
										whereString.Append(")");
									}
									// なし?
									if (isUnspecified)
									{
										whereString.Append((whereString.Length == 0) ? "" : " AND ");
										whereString.Append("EXISTS ( ");
										whereString.Append("                      SELECT  * ");
										whereString.Append("                        FROM  w2_OrderItem ");
										whereString.Append("                       WHERE  w2_OrderItem.order_id = w2_Order.order_id");
										whereString.Append("                         AND  w2_OrderItem.novelty_id = ''");
										whereString.Append(")");
									}
									var tempWhere = new StringBuilder();
									noveltyIds.ForEach(value =>
									{
										tempWhere.Append((tempWhere.Length != 0) ? " OR " : "")
											.Append(string.Format("w2_OrderItem.novelty_id = '{0}'", value.Replace("'", "''")));
									});
									if (tempWhere.Length != 0)
									{
										whereString.Append((whereString.Length == 0) ? "" : " AND ");
										whereString.Append("EXISTS ( ");
										whereString.Append("                      SELECT  * ");
										whereString.Append("                        FROM  w2_OrderItem ");
										whereString.Append("                       WHERE  w2_OrderItem.order_id = w2_Order.order_id");
										whereString.Append("                         AND  ");
										whereString.Append(tempWhere);
										whereString.Append(")");
									}
								}
								break;

							// レコメンドID
							case Constants.FIELD_ORDERITEM_RECOMMEND_ID:
								// レコメンドOPが有効の場合
								if ((Constants.RECOMMEND_OPTION_ENABLED) && (string.IsNullOrEmpty(searchValue) == false))
								{
									var isSpecified = false;
									var isUnspecified = false;
									var recommendIds = new List<string>();
									searchValue.Split(',').ToList()
										.ForEach(value =>
										{
											switch (value)
											{
												case WorkflowSetting.m_RECOMMEND_ID_SPECIFIED:
													isSpecified = true;
													break;
												case WorkflowSetting.m_RECOMMEND_ID_UNSPECIFIED:
													isUnspecified = true;
													break;
												default:
													recommendIds.Add(value);
													break;
											}
										});
									// あり + なし指定の場合は、処理を抜ける
									if (isSpecified && isUnspecified) break;
									// あり?
									if (isSpecified)
									{
										whereString.Append((whereString.Length == 0) ? "" : " AND ");
										whereString.Append("EXISTS ( ");
										whereString.Append("                      SELECT  * ");
										whereString.Append("                        FROM  w2_OrderItem ");
										whereString.Append("                       WHERE  w2_OrderItem.order_id = w2_Order.order_id");
										whereString.Append("                         AND  w2_OrderItem.recommend_id <> ''");
										whereString.Append(")");
									}
									// なし?
									if (isUnspecified)
									{
										whereString.Append((whereString.Length == 0) ? "" : " AND ");
										whereString.Append("EXISTS ( ");
										whereString.Append("                      SELECT * ");
										whereString.Append("                        FROM  w2_OrderItem ");
										whereString.Append("                       WHERE  w2_OrderItem.order_id = w2_Order.order_id");
										whereString.Append("                         AND  w2_OrderItem.recommend_id = ''");
										whereString.Append(")");
									}
									var tempWhere = new StringBuilder();
									recommendIds.ForEach(value =>
									{
										tempWhere.Append((tempWhere.Length != 0) ? " OR " : "")
											.Append(string.Format("w2_OrderItem.recommend_id = '{0}'", value.Replace("'", "''")));
									});
									if (tempWhere.Length != 0)
									{
										whereString.Append((whereString.Length == 0) ? "" : " AND ");
										whereString.Append("EXISTS ( ");
										whereString.Append("                      SELECT  * ");
										whereString.Append("                        FROM  w2_OrderItem ");
										whereString.Append("                       WHERE  w2_OrderItem.order_id = w2_Order.order_id");
										whereString.Append("                         AND  ");
										whereString.Append(tempWhere);
										whereString.Append(")");
									}
								}
								break;

							// ユーザー管理レベル
							case Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									whereInner.Append("EXISTS (");
									whereInner.Append("         SELECT  w2_User.user_id");
									whereInner.Append("           FROM  w2_User");
									whereInner.Append("          WHERE  w2_User.user_id = w2_Order.user_id");
									whereInner.Append("            AND  w2_User.user_management_level_id IN ('")
										.Append(searchValue.Replace(",", "','")).Append("'))");
								}
								break;

							// ADVCODE
							case KEY_ORDER_ADVCODE_WORKFLOW:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									whereInner.Append("(");
									var advcodeSearchText = GetSearchSettingValue(
										searchSettingsFieldValues,
										KEY_ORDER_ADVCODE
											+ "_search_text")
										.Trim();
									foreach (var value in searchValue.Split(','))
									{
										whereInner.Append(((whereInner.Length < 2) ? string.Empty : " OR "));
										switch (value.Trim())
										{
											case "1":
												whereInner.Append(Constants.TABLE_ORDER)
													.Append(".")
													.Append(Constants.FIELD_ORDER_ADVCODE_FIRST)
													.Append(" <> ''");
												whereInner.Append(" AND ");
												whereInner.Append(
													"w2_Order."
													+ Constants.FIELD_ORDER_ADVCODE_FIRST
													+ " LIKE '%' + '"
													+ StringUtility.SqlLikeStringSharpEscape(advcodeSearchText).Replace("'", "''")
													+ "' + '%' ESCAPE '#'");
												break;

											case "2":
												whereInner.Append(Constants.TABLE_ORDER)
													.Append(".")
													.Append(Constants.FIELD_ORDER_ADVCODE_NEW)
													.Append(" <> ''");
												whereInner.Append(" AND ");
												whereInner.Append(
													"w2_Order."
													+ Constants.FIELD_ORDER_ADVCODE_NEW
													+ " LIKE '%' + '"
													+ StringUtility.SqlLikeStringSharpEscape(advcodeSearchText)
														.Replace("'", "''") + "' + '%' ESCAPE '#'");
												break;
										}
									}
									whereInner.Append(")");
								}
								break;

							// 出荷予定日（出荷予定日指定有無もあわせて）
							case Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE:
								// 出荷予定日オプションが有効の場合
								if ((string.IsNullOrEmpty(searchValue) == false) && useLeadTime)
								{
									var tempWhere = new StringBuilder();
									var hasNumberOfDays = false;
									foreach (var value in searchValue.Split(','))
									{
										switch (value)
										{
											case WorkflowSetting.m_SCHEDULED_SHIPPINGDATE_SPECIFIED:
												if (hasNumberOfDays) continue;
												tempWhere.Append((tempWhere.Length != 0) ? " OR " : string.Empty)
													.Append("w2_OrderShipping.scheduled_shipping_date IS NOT NULL");
												break;

											case WorkflowSetting.m_SCHEDULED_SHIPPINGDATE_UNSPECIFIED:
												tempWhere.Append((tempWhere.Length != 0) ? " OR " : string.Empty)
													.Append("w2_OrderShipping.scheduled_shipping_date IS NULL");
												break;

											default:
												var valueDays =
													value.Split(WorkflowSetting.m_SCHEDULED_SHIPPINGDATE_SEPARATOR_CHARACTER);
												if (valueDays.Length > 1)
												{
													// 出荷予定日の日数指定有りと判断
													int dayFrom, dayTo;
													var hasDayFrom = int.TryParse(valueDays[0], out dayFrom);
													var hasDayTo = int.TryParse(valueDays[1], out dayTo);
													var presentDay = DateTime.Now.Date;
													hasNumberOfDays = (hasDayFrom || hasDayTo);

													if (hasDayFrom)
													{
														if (dayFrom == 0)
														{
															if (tempWhere.Length > 0) tempWhere.Append(" AND ");
															tempWhere.Append(
																string.Format("w2_OrderShipping.scheduled_shipping_date >= '{0}'",
																	presentDay));
														}
														else
														{
															var numberOfAddDay = 1;
															var scheduledShippingDateFrom = presentDay;
															while (numberOfAddDay <= dayFrom)
															{
																scheduledShippingDateFrom = scheduledShippingDateFrom.AddDays(1);
																if (HolidayUtil.IsHoliday(scheduledShippingDateFrom) == false) numberOfAddDay++;
															}

															if (tempWhere.Length > 0) tempWhere.Append(" AND ");
															tempWhere.Append(string.Format(
																"w2_OrderShipping.scheduled_shipping_date >= '{0}'",
																scheduledShippingDateFrom));
														}
													}

													if (hasDayTo)
													{
														if (dayTo == 0)
														{
															if (tempWhere.Length > 0) tempWhere.Append(" AND ");
															tempWhere.Append(
																string.Format("w2_OrderShipping.scheduled_shipping_date <= '{0}'",
																	presentDay));
														}
														else
														{
															var numberOfAddDay = 1;
															var scheduledShippingDateTo = presentDay;
															while (numberOfAddDay <= dayTo)
															{
																scheduledShippingDateTo = scheduledShippingDateTo.AddDays(1);
																if (HolidayUtil.IsHoliday(scheduledShippingDateTo) == false) numberOfAddDay++;
															}

															if (tempWhere.Length > 0) tempWhere.Append(" AND ");
															tempWhere.Append(
																string.Format(
																	"w2_OrderShipping.scheduled_shipping_date <= '{0}'",
																	scheduledShippingDateTo));
														}
													}
												}
												break;
										}
									}

									if (tempWhere.Length != 0)
									{
										whereString.Append((whereString.Length == 0) ? string.Empty : " AND ");
										whereString.Append("EXISTS ( ");
										whereString.Append("                      SELECT  * ");
										whereString.Append("                        FROM  w2_OrderShipping ");
										whereString.Append("                       WHERE  w2_OrderShipping.order_id = w2_Order.order_id");
										whereString.Append("                         AND  (");
										whereString.Append(tempWhere);
										whereString.Append("))");
									}
								}
								break;
							// 注文種別
							case WorkflowSetting.m_TARGET_ORDER_TYPE:
								switch (searchValue)
								{
									case Constants.FLG_ORDERWORKFLOWSETTING_TARGET_ORDER_TYPE_NORMAL:
										whereInner.Append("w2_Order.fixed_purchase_id = ''");
										break;

									case Constants.FLG_ORDERWORKFLOWSETTING_TARGET_ORDER_TYPE_FIXED_PURCHASE:
										whereInner.Append("w2_Order.fixed_purchase_id != ''");
										break;
								}
								break;

							// 配送状態
							case Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									var listSearchValue = searchValue.Split(',');
									searchValue = string.Join("','", listSearchValue);
									whereInner.Append("EXISTS (");
									whereInner.Append("         SELECT  w2_OrderShipping.order_id");
									whereInner.Append("           FROM  w2_OrderShipping");
									whereInner.Append("          WHERE  w2_OrderShipping.order_id = w2_Order.order_id");
									whereInner.Append("            AND  w2_OrderShipping.shipping_status IN ('").Append(searchValue).Append("')");
									whereInner.Append(")");
								}
								break;

							// 発票ステータス
							case Constants.FIELD_TWORDERINVOICE_TW_INVOICE_STATUS:
								if (OrderCommon.DisplayTwInvoiceInfo()
									&& (string.IsNullOrEmpty(searchValue) == false))
								{
									whereInner.Append("EXISTS (")
										.Append("         SELECT  w2_TwOrderInvoice.order_id")
										.Append("           FROM  w2_TwOrderInvoice")
										.Append("          WHERE  w2_TwOrderInvoice.order_id = w2_Order.order_id")
										.Append("                 AND w2_TwOrderInvoice.tw_invoice_status IN ('")
										.Append(searchValue.Replace(",", "','")).Append("'))");
								}
								break;

							case Constants.FIELD_ORDER_ATTRIBUTE1:
							case Constants.FIELD_ORDER_ATTRIBUTE2:
							case Constants.FIELD_ORDER_ATTRIBUTE3:
							case Constants.FIELD_ORDER_ATTRIBUTE4:
							case Constants.FIELD_ORDER_ATTRIBUTE5:
							case Constants.FIELD_ORDER_ATTRIBUTE6:
							case Constants.FIELD_ORDER_ATTRIBUTE7:
							case Constants.FIELD_ORDER_ATTRIBUTE8:
							case Constants.FIELD_ORDER_ATTRIBUTE9:
							case Constants.FIELD_ORDER_ATTRIBUTE10:
								var orderExtendQuery = OrderExtendCommon.CreateWhereQuerryOrderWorkflow(
									searchValue,
									searchField,
									Constants.TABLE_ORDER);
								whereInner.Append(orderExtendQuery);
								break;

							// 完了状態コード
							case Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_CODE:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									var listSearchValue = searchValue.Split(',');
									searchValue = string.Join("','", listSearchValue);
									whereInner.Append("EXISTS (");
									whereInner.Append("         SELECT  w2_OrderShipping.order_id");
									whereInner.Append("           FROM  w2_OrderShipping");
									whereInner.Append("          WHERE  w2_OrderShipping.order_id = w2_Order.order_id");
									whereInner.Append("            AND  w2_OrderShipping.shipping_status_code IN ('").Append(StringUtility.EscapeSqlString(searchValue)).Append("')");
									whereInner.Append(")");
								}
								break;

							// 現在の状態
							case Constants.FIELD_ORDERSHIPPING_SHIPPING_CURRENT_STATUS:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									var listSearchValue = searchValue.Split(',');
									searchValue = string.Join("','", listSearchValue);
									whereInner.Append("EXISTS (");
									whereInner.Append("         SELECT  w2_OrderShipping.order_id");
									whereInner.Append("           FROM  w2_OrderShipping");
									whereInner.Append("          WHERE  w2_OrderShipping.order_id = w2_Order.order_id");
									whereInner.Append("            AND  w2_OrderShipping.shipping_current_status IN ('").Append(StringUtility.EscapeSqlString(searchValue)).Append("')");
									whereInner.Append(")");
								}
								break;

							// キャンセル可能時間帯注文
							case Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_TARGET_CANCELABLE_TIME_ORDERS_FLG:
								if (searchValue == Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_TARGET_CANCELABLE_TIME_ORDERS_FLG_ON)
								{
									whereString.Append((whereString.Length == 0) ? "" : " AND ");
									whereString.Append("(w2_Order.order_date < DATEADD(MINUTE, -")
										.Append(Constants.ORDER_HISTORY_DETAIL_ORDER_CANCEL_TIME)
										.Append(" , GETDATE()))");
								}
								break;

							// 頒布会コースID
							case Constants.FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									whereString.Append(GetSqlAndCondition(whereString))
										.Append("EXISTS (")
										.Append("  SELECT  w2_Order.order_id")
										.Append("    FROM  w2_OrderItem")
										.Append("   WHERE  w2_OrderItem.order_id = w2_Order.order_id")
										.Append("     AND  ((")
										.Append("            CASE w2_Order.combined_org_order_ids")
										.Append("              WHEN  '' THEN w2_Order.subscription_box_course_id")
										.Append("              ELSE  w2_OrderItem.subscription_box_course_id")
										.Append("            END")
										.Append($"         ) LIKE '{StringUtility.SqlLikeStringSharpEscape(searchValue).Replace("'", "''")}%' ESCAPE '#'")
										.Append("          OR (")
										.Append("               CASE w2_Order.combined_org_order_ids")
										.Append("                 WHEN  '' THEN w2_Order.subscription_box_course_id")
										.Append("                 ELSE  w2_OrderItem.subscription_box_course_id")
										.Append("               END")
										.Append($"            ) IN ('{StringUtility.EscapeSqlString(searchValue).Replace(",", "','")}'))")
										.Append(")");
								}
								break;

							// 頒布会購入回数FROM
							case WorkflowSetting.SUBSCRIPTIONBOX_ORDER_COUNT_FROM:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									whereString.Append(GetSqlAndCondition(whereString))
										.Append("w2_Order.order_subscription_box_order_count >= ")
										.Append(searchValue);
								}
								break;

							// 頒布会購入回数TO
							case WorkflowSetting.SUBSCRIPTIONBOX_ORDER_COUNT_TO:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									whereString.Append(GetSqlAndCondition(whereString))
										.Append("w2_Order.order_subscription_box_order_count <= ")
										.Append(searchValue);
								}
								break;

							// 店舗受取ステータス
							case Constants.FIELD_ORDER_STOREPICKUP_STATUS:
								if ((string.IsNullOrEmpty(searchValue) == false)
									&& Constants.STORE_PICKUP_OPTION_ENABLED)
								{
									whereInner.AppendFormat(
										"({0}.{1} IN ('{2}'))",
										Constants.TABLE_ORDER,
										searchField,
										searchValue.Replace(",", "','"));
								}
								break;

							// リアル店舗ID
							case Constants.FIELD_REALSHOP_REAL_SHOP_ID:
								if ((string.IsNullOrEmpty(searchValue) == false)
									&& Constants.STORE_PICKUP_OPTION_ENABLED)
								{
									whereInner.Append("EXISTS (")
										.Append("         SELECT  w2_OrderShipping.*")
										.Append("           FROM  w2_OrderShipping")
										.Append("          WHERE  w2_OrderShipping.order_id = w2_Order.order_id")
										.Append("            AND  w2_OrderShipping.storepickup_real_shop_id = '")
										.Append(searchValue)
										.Append("')");
								}
								break;
						}
					}
					// 返品交換ワークの場合
					else
					{
						switch (searchField)
						{
							// 返品交換区分
							case Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN:
							// 返品交換都合区分
							case Constants.FIELD_ORDER_RETURN_EXCHANGE_REASON_KBN:
							// 返品交換ステータス
							case Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS:
							// 返金ステータス
							case Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									whereInner
										.Append("(")
										.Append(searchField)
										.Append(" IN ('")
										.Append(searchValue.Replace(",", "','"))
										.Append("'))");
								}
								break;

							// 並び順
							case Constants.REQUEST_KEY_RETURN_EXCHANGE_SORT_KBN:
								sqlParam[ORDERWORKFLOWSETTING_SORT_KBN] = searchValue;
								break;

							// 返品交換返金更新日(ステータス)
							case WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS:
								updateStatus = searchValue;
								break;

							// 返品交換返金更新日(指定方法)
							case WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_DAY:
								// 指定がある&期間指定ではない場合
								if ((string.IsNullOrEmpty(searchValue) == false)
									&& (searchValue != WorkflowSetting.m_SEARCH_STATUS_DATE_FROMTO))
								{
									KeyValuePair<string, string> kvpUpdateStatusDateFromTo =
										GetUpdateStatusDateFromTo(
											WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS,
											searchValue);
									whereInner.Append("(")
										.Append(updateStatus)
										.Append(" >= '")
										.Append(kvpUpdateStatusDateFromTo.Key)
										.Append("')");
									whereInner.Append(" AND (")
										.Append(updateStatus)
										.Append(" < DateAdd(day,1,'")
										.Append(kvpUpdateStatusDateFromTo.Value)
										.Append("'))");
								}
								break;


							// 返品交換返金更新日(期間From)
							case WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_FROM:
								statusDateTo = -(int.Parse(searchValue));
								whereInner.Append("(")
									.Append(updateStatus)
									.Append(" < DateAdd(day,")
									.Append((statusDateTo + 1).ToString())
									.Append(",'")
									.Append(currentDate)
									.Append("'))");
								break;

							// 返品交換返金更新日(期間To)
							case WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_TO:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									statusDateTo += -(int.Parse(searchValue));	// 日付を加算
									whereInner.Append("(")
										.Append(updateStatus)
										.Append(" >= DateAdd(day,")
										.Append(statusDateTo.ToString())
										.Append(",'")
										.Append(currentDate)
										.Append(" ")
										.Append(GetUpdateStatusTimeTo(
											searchSettingsFieldValues,
											WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_TIME))
										.Append("'))");
								}
								break;

							// 発票ステータス
							case Constants.FIELD_TWORDERINVOICE_TW_INVOICE_STATUS:
								if (OrderCommon.DisplayTwInvoiceInfo()
									&& (string.IsNullOrEmpty(searchValue) == false))
								{
									whereInner.Append("EXISTS (")
										.Append("         SELECT  w2_TwOrderInvoice.order_id")
										.Append("           FROM  w2_TwOrderInvoice")
										.Append("          WHERE  w2_TwOrderInvoice.order_id = w2_Order.order_id")
										.Append("                 AND w2_TwOrderInvoice.tw_invoice_status IN ('")
										.Append(searchValue.Replace(",", "','")).Append("'))");
								}
								break;

							// 返品交換 決済種別
							case WorkflowSetting.m_ORDER_RETURN_PAYMENT_KBN:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									var returnPaymentKbn = string.Join("','", searchValue.Split(','));
									whereInner.Append("w2_Order.order_payment_kbn IN('" + returnPaymentKbn + "')");
								}
								break;

							// 外部決済ステータス：クレジットカード
							case WorkflowSetting.m_ORDER_RETURN_EXTERNAL_PAYMENT_STATUS_CARD:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									var inValue = string.Join(",", searchValue.Split(',').Select(x => string.Format("'{0}'", x)));

									whereString.Append(
										string.Format(" AND ( (w2_Order.order_payment_kbn <> '{0}') OR( w2_Order.order_payment_kbn = '{0}' AND w2_Order.external_payment_status IN ({1}) ) )  ",
											Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT, inValue));
								}
								break;

							// 外部決済ステータス：コンビニ(後払い)
							case WorkflowSetting.m_ORDER_RETURN_EXTERNAL_PAYMENT_STATUS_CVS:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									var inValue = string.Join(",", searchValue.Split(',').Select(x => string.Format("'{0}'", x)));

									whereString.Append(
										string.Format(
											" AND ( (w2_Order.order_payment_kbn <> '{0}') OR( w2_Order.order_payment_kbn = '{0}' AND w2_Order.external_payment_status IN ({1}) ) )  ",
											Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
											inValue));
								}
								break;

							// 外部決済ステータス：台湾後払い
							case WorkflowSetting.m_ORDER_RETURN_EXTERNAL_PAYMENT_STATUS_TRYLINK_AFTERPAY:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									var inValue = string.Join(",", searchValue.Split(',').Select(x => string.Format("'{0}'", x)));

									whereString.Append(
										string.Format(
											" AND ( (w2_Order.order_payment_kbn <> '{0}') OR( w2_Order.order_payment_kbn = '{0}' AND w2_Order.external_payment_status IN ({1}) ) )  ",
											Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY,
											inValue));
								}
								break;

							// Order external payment status for all payment
							case Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									var inValue = string.Join(",", searchValue.Split(',').Select(item => string.Format("'{0}'", item)));

									whereString.Append(
										string.Format("AND (w2_Order.external_payment_status IN ({0}))", inValue));
								}
								break;

							// 頒布会コースID
							case Constants.FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									whereString.Append(GetSqlAndCondition(whereString))
										.Append("EXISTS (")
										.Append("  SELECT  w2_Order.order_id")
										.Append("    FROM  w2_OrderItem")
										.Append("   WHERE  w2_OrderItem.order_id = w2_Order.order_id")
										.Append("     AND  (")
										.Append($"           w2_OrderItem.subscription_box_course_id LIKE '{StringUtility.SqlLikeStringSharpEscape(searchValue).Replace("'", "''")}%' ESCAPE '#'")
										.Append("            OR")
										.Append($"           w2_OrderItem.subscription_box_course_id IN ('{StringUtility.EscapeSqlString(searchValue).Replace(",", "','")}') )")
										.Append("          )")
										.Append(")");
								}
								break;

							// 頒布会購入回数FROM
							case WorkflowSetting.SUBSCRIPTIONBOX_ORDER_COUNT_FROM:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									whereString.Append(GetSqlAndCondition(whereString))
										.Append("w2_Order.order_subscription_box_order_count >= ")
										.Append(searchValue);
								}
								break;

							// 頒布会購入回数TO
							case WorkflowSetting.SUBSCRIPTIONBOX_ORDER_COUNT_TO:
								if (string.IsNullOrEmpty(searchValue) == false)
								{
									whereString.Append(GetSqlAndCondition(whereString))
										.Append("w2_Order.order_subscription_box_order_count <= ")
										.Append(searchValue);
								}
								break;
						}
					}

					// 内部の条件文が存在する場合は結合
					if (whereInner.Length != 0)
					{
						if (string.IsNullOrEmpty(whereString.ToString()) == false)
						{
							whereString.Append(" AND ");
						}
						whereString.Append(whereInner);
					}
				}
			}

			// 条件文が存在する場合に格納
			if (whereString.Length != 0)
			{
				sqlParam[WORKFLOWSETTING_WHERE] = whereString.ToString();
			}

			if (!string.IsNullOrEmpty(masterKbn)) sqlParam[Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN] = masterKbn;

			return sqlParam;
		}

		/// <summary>
		/// Get Search Param Fixed Purchase
		/// </summary>
		/// <param name="workflowType">Workflow type</param>
		/// <param name="shopId">Shop id</param>
		/// <param name="workflowKbn">Workflow kbn</param>
		/// <param name="workflowNo">Workflow no</param>
		/// <param name="condition">Condition</param>
		/// <param name="workflowSetting">Workflow setting</param>
		/// <param name="useLeadTime">Use lead time</param>
		/// <param name="masterKbn">Master kbn</param>
		/// <returns>Param data</returns>
		public Hashtable GetSearchParamFixedPurchase(
			string workflowType,
			string shopId,
			string workflowKbn,
			string workflowNo,
			FixedPurchaseConditionForWorkflow condition,
			WorkflowSetting workflowSetting,
			bool useLeadTime,
			string masterKbn = null)
		{
			// 定期ワークフロー設定情報取得(存在しなければ空の情報を返す）
			var searchSettingsFieldValue = string.Empty;
			var displayCount = 1;
			var workflow = workflowSetting.GetOrderWorkflowSetting(
				shopId,
				workflowKbn,
				workflowNo,
				workflowType);

			if (workflow.Count != 0)
			{
				searchSettingsFieldValue = StringUtility.ToEmpty(workflow[0][Constants.FIELD_ORDERWORKFLOWSETTING_SEARCH_SETTING]);
				displayCount = (int)workflow[0][Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_COUNT];
			}

			// パラメタ作成
			var sqlParam = new Hashtable
			{
				{ Constants.FIELD_ORDERWORKFLOWSETTING_SHOP_ID, shopId },
				{ Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_COUNT, displayCount },
				{ Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_MANAGEMENT_MEMO, StringUtility.ToEmpty(condition.MemoFlg) },
				{ Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_MANAGEMENT_MEMO + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(condition.Memo) },
				{ Constants.FIELD_FIXEDPURCHASE_SHIPPING_MEMO, StringUtility.ToEmpty(condition.ShippingMemoFlg) },
				{ Constants.FIELD_FIXEDPURCHASE_SHIPPING_MEMO + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(condition.ShippingMemo) },
				{ Constants.FIELD_FIXEDPURCHASE_RECEIPT_FLG, StringUtility.ToEmpty(condition.ReceiptFlg) },
				{ Constants.SEARCH_FIELD_ORDER_EXTEND_NAME,  StringUtility.ToEmpty(condition.OrderExtendName) },
				{ Constants.SEARCH_FIELD_ORDER_EXTEND_FLG,  StringUtility.ToEmpty(condition.OrderExtendFlg) },
				{ Constants.SEARCH_FIELD_ORDER_EXTEND_TYPE,  StringUtility.ToEmpty(condition.OrderExtendType) },
				{ Constants.SEARCH_FIELD_ORDER_EXTEND_LIKE_ESCAPED,  StringUtility.ToEmpty(condition.OrderExtendLikeEscaped) },
				{ ORDERWORKFLOWSETTING_SORT_KBN, string.Empty }
			};

			// WHERE文作成
			// 追加検索用パラメタ取得
			var fixedPurchaseId = StringUtility.ToEmpty(condition.FixedPurchaseId);
			var productId = StringUtility.ToEmpty(condition.ProductId);
			var productName = StringUtility.ToEmpty(condition.ProductName);
			var userId = StringUtility.ToEmpty(condition.UserId);
			var fixedPurchaseStatus = StringUtility.ToEmpty(condition.FixedPurchaseStatus);
			var fixedPurchasePaymentStatus = StringUtility.ToEmpty(condition.FixedPurchasePaymentStatus);
			var orderedCountFrom = StringUtility.ToEmpty(condition.OrderedCountFrom);
			var orderedCountTo = StringUtility.ToEmpty(condition.OrderedCountTo);
			var shippedCountFrom = StringUtility.ToEmpty(condition.ShippedCountFrom);
			var shippedCountTo = StringUtility.ToEmpty(condition.ShippedCountTo);
			var dateCreatedFrom = StringUtility.ToEmpty(condition.DateCreatedFrom);
			var dateCreatedTo = StringUtility.ToEmpty(condition.DateCreatedTo);
			var dateChangedFrom = StringUtility.ToEmpty(condition.DateChangedFrom);
			var dateChangedTo = StringUtility.ToEmpty(condition.DateChangedTo);
			var dateLastOrderedFrom = StringUtility.ToEmpty(condition.DateLastOrderedFrom);
			var dateLastOrderedTo = StringUtility.ToEmpty(condition.DateLastOrderedTo);
			var dateBgnFrom = StringUtility.ToEmpty(condition.DateBgnFrom);
			var dateBgnTo = StringUtility.ToEmpty(condition.DateBgnTo);
			var dateNextShippingFrom = StringUtility.ToEmpty(condition.DateNextShippingFrom);
			var dateNextShippingTo = StringUtility.ToEmpty(condition.DateNextShippingTo);
			var dateNextNextShippingFrom = StringUtility.ToEmpty(condition.DateNextNextShippingFrom);
			var dateNextNextShippingTo = StringUtility.ToEmpty(condition.DateNextNextShippingTo);
			var subscriptionBoxCourseId = StringUtility.ToEmpty(condition.SubscriptionBoxCourseId);
			var subscriptionBoxOrderCountFrom = StringUtility.ToEmpty(condition.SubscriptionBoxOrderCountFrom);
			var subscriptionBoxOrderCountTo = StringUtility.ToEmpty(condition.SubscriptionBoxOrderCountTo);

			// デフォルト設定（リスト未選択の場合はありえない条件を付加。csv出力でも使われるためここで条件設定）
			var whereString = new StringBuilder();
			if (!condition.IsSelectedByWorkflow)
			{
				// 未選択の時は検索しない条件で返す
				sqlParam.Add(OrderSearchParam.WORKFLOWSETTING_WHERE, " 1 = 0 ");
				return sqlParam;
			}

			whereString.Append(" 1 = 1 ");

			if (workflowSetting.IsDetailKbnNormal
				&& workflowSetting.IsAdditionalSearchFlgOn)
			{
				// 商品IDを条件に追加（前方一致）
				if (string.IsNullOrEmpty(productId) == false)
				{
					whereString.Append(GetSqlAndCondition(whereString))
						.Append("w2_FixedPurchase.fixed_purchase_id IN ( ")
						.Append("    SELECT  DISTINCT w2_FixedPurchaseItem.fixed_purchase_id ")
						.Append("      FROM  w2_FixedPurchaseItem ")
						.Append("     WHERE  w2_FixedPurchaseItem.variation_id LIKE '")
						.Append(productId.Replace("'", "''")).Append("%') ");
				}

				// 商品名を条件に追加（部分一致）
				if (string.IsNullOrEmpty(productName) == false)
				{
					whereString.Append(GetSqlAndCondition(whereString))
						.Append("w2_FixedPurchase.fixed_purchase_id IN ( ")
						.Append("    SELECT  DISTINCT w2_FixedPurchaseItem.fixed_purchase_id ")
						.Append("      FROM  w2_FixedPurchaseItem ")
						.Append("     INNER JOIN w2_ProductView ON")
						.Append("     (")
						.Append("       w2_FixedPurchaseItem.shop_id = w2_ProductView.shop_id")
						.Append("       AND w2_FixedPurchaseItem.product_id = w2_ProductView.product_id")
						.Append("       AND w2_FixedPurchaseItem.variation_id = w2_ProductView.variation_id")
						.Append("     )")
						.Append("     WHERE w2_ProductView.name LIKE '%")
						.Append(productName.Replace("'", "''")).Append("%') ");
				}

				// 注文IDを条件に追加（前方一致）
				if (string.IsNullOrEmpty(fixedPurchaseId) == false)
				{
					whereString.Append(GetSqlAndCondition(whereString))
						.Append("w2_FixedPurchase.fixed_purchase_id LIKE '")
						.Append(fixedPurchaseId.Replace("'", "''")).Append("%' ");
				}

				// ユーザーIDを条件に追加（前方一致）
				if (string.IsNullOrEmpty(userId) == false)
				{
					whereString.Append(GetSqlAndCondition(whereString))
						.Append("w2_FixedPurchase.user_id LIKE '")
						.Append(userId.Replace("'", "''")).Append("%' ");
				}

				// 定期ステータス
				if (string.IsNullOrEmpty(fixedPurchaseStatus) == false)
				{
					whereString.Append(GetSqlAndCondition(whereString))
						.Append("w2_FixedPurchase.fixed_purchase_status = '")
						.Append(fixedPurchaseStatus.Replace("'", "''")).Append("'");
				}

				// 定期決済ステータス
				if (string.IsNullOrEmpty(fixedPurchasePaymentStatus) == false)
				{
					whereString.Append(GetSqlAndCondition(whereString))
						.Append("w2_FixedPurchase.payment_status = '")
						.Append(fixedPurchasePaymentStatus.Replace("'", "''")).Append("'");
				}

				// 購入回数(注文基準)(From)
				if (string.IsNullOrEmpty(orderedCountFrom) == false)
				{
					whereString.Append(GetSqlAndCondition(whereString))
						.Append("w2_FixedPurchase.order_count >= '")
						.Append(orderedCountFrom.Replace("'", "''")).Append("' ");
				}

				// 購入回数(注文基準)(To)
				if (string.IsNullOrEmpty(orderedCountTo) == false)
				{
					whereString.Append(GetSqlAndCondition(whereString))
						.Append("w2_FixedPurchase.order_count <= '")
						.Append(orderedCountTo.Replace("'", "''"))
						.Append("' ");
				}

				// 購入回数(出荷基準)(From)
				if (string.IsNullOrEmpty(shippedCountFrom) == false)
				{
					whereString.Append(GetSqlAndCondition(whereString))
						.Append("w2_FixedPurchase.shipped_count >= '")
						.Append(shippedCountFrom.Replace("'", "''"))
						.Append("' ");
				}

				// 購入回数(出荷基準)(To)
				if (string.IsNullOrEmpty(shippedCountTo) == false)
				{
					whereString.Append(GetSqlAndCondition(whereString))
						.Append("w2_FixedPurchase.shipped_count <= '")
						.Append(shippedCountTo.Replace("'", "''"))
						.Append("' ");
				}

				// 拡張ステータス
				if ((string.IsNullOrEmpty(condition.FixedPurchaseExtendStatusNo) == false)
					&& (string.IsNullOrEmpty(condition.FixedPurchaseExtendStatus) == false))
				{
					whereString.Append(GetSqlAndCondition(whereString));
					whereString.Append(
						"w2_FixedPurchase.extend_status" + condition.FixedPurchaseExtendStatusNo
						+ " = '").Append(condition.FixedPurchaseExtendStatus.Replace("'", "''")).Append("'");
				}

				// 拡張ステータス更新日
				if (string.IsNullOrEmpty(condition.FixedPurchaseExtendStatusNoUpdateDate) == false)
				{
					// 拡張ステータス更新日(From)
					var extendStatusUpdateDateFrom = condition.FixedPurchaseExtendStatusForm;
					if (Validator.IsDate(extendStatusUpdateDateFrom))
					{
						whereString.Append(GetSqlAndCondition(whereString)).Append(
								"w2_FixedPurchase.extend_status_date"
								+ condition.FixedPurchaseExtendStatusNoUpdateDate + " >= '")
							.Append(extendStatusUpdateDateFrom.Replace("'", "''")).Append("' ");
					}

					// 拡張ステータス更新日(To)
					var extendStatusUpdateDateTo = condition.FixedPurchaseExtendStatusTo;
					if (Validator.IsDate(extendStatusUpdateDateTo))
					{
						whereString.Append(GetSqlAndCondition(whereString)).Append(
							"w2_FixedPurchase.extend_status_date"
							+ condition.FixedPurchaseExtendStatusNoUpdateDate
							+ " < DATEADD(day, 1, '").Append(extendStatusUpdateDateTo.Replace("'", "''")).Append("') ");
					}
				}

				// 作成日(From)
				if (Validator.IsDate(dateCreatedFrom))
				{
					whereString.Append(GetSqlAndCondition(whereString))
						.Append("w2_FixedPurchase.date_created >= '")
						.Append(dateCreatedFrom.Replace("'", "''"))
						.Append("' ");
				}

				// 作成日(To)
				if (Validator.IsDate(dateCreatedTo))
				{
					whereString.Append(GetSqlAndCondition(whereString))
						.Append("w2_FixedPurchase.date_created < DATEADD(day, 1, '")
						.Append(dateCreatedTo.Replace("'", "''"))
						.Append("') ");
				}

				// 更新日(From)
				if (Validator.IsDate(dateChangedFrom))
				{
					whereString.Append(GetSqlAndCondition(whereString))
						.Append("w2_FixedPurchase.date_changed >= '")
						.Append(dateChangedFrom.Replace("'", "''"))
						.Append("' ");
				}

				// 更新日(To)
				if (Validator.IsDate(dateChangedTo))
				{
					whereString.Append(GetSqlAndCondition(whereString))
						.Append("w2_FixedPurchase.date_changed < DATEADD(day, 1, '")
						.Append(dateChangedTo.Replace("'", "''"))
						.Append("') ");
				}

				// 最終購入日(From)
				if (Validator.IsDate(dateLastOrderedFrom))
				{
					whereString.Append(GetSqlAndCondition(whereString))
						.Append("w2_FixedPurchase.last_order_date >= '")
						.Append(dateLastOrderedFrom.Replace("'", "''"))
						.Append("' ");
				}

				// 最終購入日(To)
				if (Validator.IsDate(dateLastOrderedTo))
				{
					whereString.Append(GetSqlAndCondition(whereString))
						.Append("w2_FixedPurchase.last_order_date < DATEADD(day, 1, '")
						.Append(dateLastOrderedTo.Replace("'", "''"))
						.Append("') ");
				}

				// 購入開始日(From)
				if (Validator.IsDate(dateBgnFrom))
				{
					whereString.Append(GetSqlAndCondition(whereString))
						.Append("w2_FixedPurchase.fixed_purchase_date_bgn >= '")
						.Append(dateBgnFrom.Replace("'", "''"))
						.Append("' ");
				}

				// 購入開始日(To)
				if (Validator.IsDate(dateBgnTo))
				{
					whereString.Append(GetSqlAndCondition(whereString))
						.Append("w2_FixedPurchase.fixed_purchase_date_bgn < DATEADD(day, 1, '")
						.Append(dateBgnTo.Replace("'", "''"))
						.Append("') ");
				}

				// 次回配送日(From)
				if (Validator.IsDate(dateNextShippingFrom))
				{
					whereString.Append(GetSqlAndCondition(whereString))
						.Append("w2_FixedPurchase.next_shipping_date >= '")
						.Append(dateNextShippingFrom.Replace("'", "''"))
						.Append("' ");
				}

				// 次回配送日(To)
				if (Validator.IsDate(dateNextShippingTo))
				{
					whereString.Append(GetSqlAndCondition(whereString))
						.Append("w2_FixedPurchase.next_shipping_date < DATEADD(day, 1, '")
						.Append(dateNextShippingTo.Replace("'", "''"))
						.Append("') ");
				}

				// 次々回配送日(From)
				if (Validator.IsDate(dateNextNextShippingFrom))
				{
					whereString.Append(GetSqlAndCondition(whereString))
						.Append("w2_FixedPurchase.next_next_shipping_date >= '")
						.Append(dateNextNextShippingFrom.Replace("'", "''"))
						.Append("' ");
				}

				// 次々回配送日(To)
				if (Validator.IsDate(dateNextNextShippingTo))
				{
					whereString.Append(GetSqlAndCondition(whereString))
						.Append("w2_FixedPurchase.next_next_shipping_date < DATEADD(day, 1, '")
						.Append(dateNextNextShippingTo.Replace("'", "''"))
						.Append("') ");
				}

				// 頒布会コースID
				if (string.IsNullOrEmpty(subscriptionBoxCourseId) == false)
				{
					whereString.Append(GetSqlAndCondition(whereString))
						.Append("w2_FixedPurchase.subscription_box_course_id LIKE '")
						.Append(subscriptionBoxCourseId.Replace("'", "''"))
						.Append("%' ");
				}

				// 購入回数(注文基準from)
				if (string.IsNullOrEmpty(subscriptionBoxOrderCountFrom) == false)
				{
					whereString.Append(GetSqlAndCondition(whereString))
						.Append("w2_FixedPurchase.subscription_box_order_count >= ")
						.Append(subscriptionBoxOrderCountFrom);
				}

				// 購入回数(注文基準To)
				if (string.IsNullOrEmpty(subscriptionBoxOrderCountTo) == false)
				{
					whereString.Append(GetSqlAndCondition(whereString))
						.Append("w2_FixedPurchase.subscription_box_order_count <= ")
						.Append(subscriptionBoxOrderCountTo);
				}
			}

			// 抽出検索条件が存在する場合
			if (string.IsNullOrEmpty(searchSettingsFieldValue) == false)
			{
				var searchSettingsFieldValues = searchSettingsFieldValue.Split('&');
				foreach (var searchSetting in searchSettingsFieldValues)
				{
					var searchSettingFieldValue = searchSetting.Split('=');
					var searchField = searchSettingFieldValue[0];
					var searchValue = searchSettingFieldValue[1];
					var whereInner = new StringBuilder();

					switch (searchField)
					{
						// 注文区分
						case Constants.FIELD_FIXEDPURCHASE_ORDER_KBN:
						// 定期購入ステータス
						case Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS:
						// 定期区分
						case Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN:
						// 決済種別区分
						case Constants.FIELD_FIXEDPURCHASE_ORDER_PAYMENT_KBN:
						// 注文拡張ステータス1～50
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS1:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS2:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS3:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS4:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS5:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS6:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS7:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS8:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS9:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS10:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS11:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS12:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS13:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS14:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS15:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS16:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS17:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS18:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS19:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS20:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS21:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS22:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS23:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS24:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS25:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS26:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS27:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS28:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS29:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS30:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS31:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS32:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS33:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS34:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS35:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS36:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS37:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS38:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS39:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS40:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS41:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS42:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS43:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS44:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS45:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS46:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS47:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS48:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS49:
						case Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS50:
						case Constants.FIELD_FIXEDPURCHASE_RECEIPT_FLG:
							if (string.IsNullOrEmpty(searchValue) == false)
							{
								whereInner.AppendFormat(
									"({0}.{1} IN ('{2}'))",
									Constants.TABLE_FIXEDPURCHASE,
									searchField,
									searchValue.Replace("'", "''").Replace(",", "','"));
							}
							break;

						// 各メモ(注文メモ、管理メモ、配送メモ、決済連携メモ、外部連携メモ)
						case Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_MANAGEMENT_MEMO:
						case Constants.FIELD_FIXEDPURCHASE_SHIPPING_MEMO:
							if (string.IsNullOrEmpty(searchValue) == false)
							{
								whereInner.Append("(");
								foreach (var value in searchValue.Split(','))
								{
									whereInner.Append(((whereInner.Length < 2) ? string.Empty : " OR "));
									switch (value.Trim())
									{
										case "1":
											whereInner.AppendFormat(
												"({0}.{1} != '' AND {0}.{1} LIKE '%' + '{2}' + '%' ESCAPE '#' )",
												Constants.TABLE_FIXEDPURCHASE,
												searchField,
												StringUtility.SqlLikeStringSharpEscape(
													GetSearchSettingValue(
														searchSettingsFieldValues,
														searchField + "_search_text")).Replace("'", "''"));
											break;

										case "0":
											whereInner.AppendFormat("{0}.{1} = ''", Constants.TABLE_FIXEDPURCHASE, searchField);
											break;
									}
								}
								whereInner.Append(")");
							}
							break;

						// 商品ID
						case Constants.FIELD_FIXEDPURCHASEITEM_PRODUCT_ID:
							if (string.IsNullOrEmpty(searchValue) == false)
							{
								whereString.Append(GetSqlAndCondition(whereString))
									.Append("w2_FixedPurchase.fixed_purchase_id IN ( ")
									.Append("SELECT DISTINCT w2_FixedPurchaseItem.fixed_purchase_id ")
									.Append("FROM w2_FixedPurchaseItem ")
									.Append("WHERE w2_FixedPurchaseItem.product_id IN ('")
									.Append(searchValue.Replace("'", "''").Replace(",", "','"))
									.Append("'))");
							}
							break;

						// 購入回数(注文基準from)
						case WorkflowSetting.m_FIXEDPURCHASE_ORDER_COUNT_FROM:
							if (string.IsNullOrEmpty(searchValue) == false)
							{
								whereString.Append(GetSqlAndCondition(whereString))
									.Append(" w2_FixedPurchase.order_count >= ")
									.Append(searchValue);
							}
							break;

						// 購入回数(注文基準To)
						case WorkflowSetting.m_FIXEDPURCHASE_ORDER_COUNT_TO:
							if (string.IsNullOrEmpty(searchValue) == false)
							{
								whereString.Append(GetSqlAndCondition(whereString))
									.Append(" w2_FixedPurchase.order_count <= ")
									.Append(searchValue);
							}
							break;

						// 購入回数(出荷基準from)
						case WorkflowSetting.m_FIXEDPURCHASE_SHIPPED_COUNT_FROM:
							if (string.IsNullOrEmpty(searchValue) == false)
							{
								whereString.Append(GetSqlAndCondition(whereString))
									.Append(" w2_FixedPurchase.shipped_count >= ")
									.Append(searchValue);
							}
							break;

						// 購入回数(出荷基準To)
						case WorkflowSetting.m_FIXEDPURCHASE_SHIPPED_COUNT_TO:
							if (string.IsNullOrEmpty(searchValue) == false)
							{
								whereString.Append(GetSqlAndCondition(whereString))
									.Append(" w2_FixedPurchase.shipped_count <= ")
									.Append(searchValue);
							}
							break;

						// 配送種別
						case Constants.FIELD_PRODUCT_SHIPPING_TYPE:
							if (string.IsNullOrEmpty(searchValue) == false)
							{
								whereString.Append(GetSqlAndCondition(whereString));
								whereString.Append("    EXISTS  (");
								whereString.Append("    SELECT  1");
								whereString.Append("      FROM  w2_FixedPurchaseItem");
								whereString.Append("      INNER JOIN  w2_ProductView");
								whereString.Append("      ON ");
								whereString.Append("      w2_FixedPurchaseItem.shop_id = w2_ProductView.shop_id");
								whereString.Append("      AND");
								whereString.Append("      w2_FixedPurchaseItem.product_id = w2_ProductView.product_id");
								whereString.Append("      AND");
								whereString.Append("      w2_FixedPurchaseItem.variation_id = w2_ProductView.variation_id");
								whereString.Append("      WHERE w2_FixedPurchaseItem.fixed_purchase_id = w2_FixedPurchase.fixed_purchase_id");
								whereString.Append("      AND w2_ProductView.shipping_type IN('")
									.Append(searchValue.Replace("'", "''").Replace(",", "','")).Append("'))");
							}
							break;

						// 作成日指定
						case Constants.FIELD_FIXEDPURCHASE_DATE_CREATED:
							if (string.IsNullOrEmpty(searchValue) == false)
							{
								var tempWhere = new StringBuilder();
								int dayTo;
								if (int.TryParse(searchValue, out dayTo))
								{
									if (tempWhere.Length > 0) tempWhere.Append(" AND ");
									tempWhere.AppendFormat(
										"w2_FixedPurchase.date_created < '{0}'",
										((dayTo == 0)
											? DateTime.Now.Date
											: DateTime.Now.AddDays(-dayTo + 1).Date).ToString().Replace("'", "''"));
								}

								if (tempWhere.Length != 0)
								{
									whereString.Append((whereString.Length == 0) ? string.Empty : " AND (")
										.Append(tempWhere.ToString()).Append(")");
								}
							}
							break;

						// 更新日指定
						case Constants.FIELD_FIXEDPURCHASE_DATE_CHANGED:
							if (string.IsNullOrEmpty(searchValue) == false)
							{
								var tempWhere = new StringBuilder();
								int dayTo;
								if (int.TryParse(searchValue, out dayTo))
								{
									if (tempWhere.Length > 0) tempWhere.Append(" AND ");
									tempWhere.AppendFormat(
										"w2_FixedPurchase.date_changed < '{0}'",
										((dayTo == 0)
											? DateTime.Now.Date
											: DateTime.Now.AddDays(-dayTo + 1).Date).ToString().Replace("'", "''"));
								}

								if (tempWhere.Length != 0)
								{
									whereString.Append((whereString.Length == 0) ? string.Empty : " AND (")
										.Append(tempWhere.ToString()).Append(")");
								}
							}
							break;

						// 最終購入日指定
						case Constants.FIELD_FIXEDPURCHASE_LAST_ORDER_DATE:
							if (string.IsNullOrEmpty(searchValue) == false)
							{
								var tempWhere = new StringBuilder();
								int dayTo;
								if (int.TryParse(searchValue, out dayTo))
								{
									if (tempWhere.Length > 0) tempWhere.Append(" AND ");
									tempWhere.AppendFormat(
										"w2_FixedPurchase.last_order_date < '{0}'",
										((dayTo == 0)
											? DateTime.Now.Date
											: DateTime.Now.AddDays(-dayTo + 1).Date).ToString().Replace("'", "''"));
								}

								if (tempWhere.Length != 0)
								{
									whereString.Append((whereString.Length == 0) ? string.Empty : " AND (")
										.Append(tempWhere.ToString()).Append(")");
								}
							}
							break;

						// 購入開始日指定
						case Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_DATE_BGN:
							if (string.IsNullOrEmpty(searchValue) == false)
							{
								var tempWhere = new StringBuilder();
								int dayTo;
								if (int.TryParse(searchValue, out dayTo))
								{
									if (tempWhere.Length > 0) tempWhere.Append(" AND ");
									tempWhere.AppendFormat(
										"w2_FixedPurchase.fixed_purchase_date_bgn < '{0}'",
										((dayTo == 0)
											? DateTime.Now.Date
											: DateTime.Now.AddDays(-dayTo + 1).Date).ToString().Replace("'", "''"));
								}

								if (tempWhere.Length != 0)
								{
									whereString.Append((whereString.Length == 0) ? string.Empty : " AND (")
										.Append(tempWhere.ToString()).Append(")");
								}
							}
							break;

						// 次回配送日指定
						case Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE:
							if (string.IsNullOrEmpty(searchValue) == false)
							{
								var tempWhere = new StringBuilder();
								var valueDays = searchValue.Split(WorkflowSetting.m_LAST_AUTH_DATE_SEPARATOR_CHARACTER);
								if (valueDays.Length > 1)
								{
									int dayFrom, dayTo;

									if (int.TryParse(valueDays[0], out dayFrom))
									{
										tempWhere.AppendFormat(
											"w2_FixedPurchase.next_shipping_date >= '{0}'",
											((dayFrom == 0)
												? DateTime.Now.Date
												: DateTime.Now.AddDays(dayFrom).Date).ToString().Replace("'", "''"));
									}

									if (int.TryParse(valueDays[1], out dayTo))
									{
										if (tempWhere.Length > 0) tempWhere.Append(" AND ");

										tempWhere.AppendFormat(
											"w2_FixedPurchase.next_shipping_date < '{0}'",
											((dayTo == 0)
												? DateTime.Now.Date
												: DateTime.Now.AddDays(dayTo + 1).Date).ToString().Replace("'", "''"));
									}
								}

								if (tempWhere.Length != 0)
								{
									whereString.Append((whereString.Length == 0) ? string.Empty : " AND (")
										.Append(tempWhere.ToString()).Append(")");
								}
							}
							break;

						// 次々回配送日指定
						case Constants.FIELD_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE:
							if (string.IsNullOrEmpty(searchValue) == false)
							{
								var tempWhere = new StringBuilder();
								var valueDays = searchValue.Split(WorkflowSetting.m_LAST_AUTH_DATE_SEPARATOR_CHARACTER);
								if (valueDays.Length > 1)
								{
									int dayFrom, dayTo;

									if (int.TryParse(valueDays[0], out dayFrom))
									{
										tempWhere.AppendFormat(
											"w2_FixedPurchase.next_next_shipping_date >= '{0}'",
											((dayFrom == 0)
												? DateTime.Now.Date
												: DateTime.Now.AddDays(dayFrom).Date).ToString().Replace("'", "''"));
									}

									if (int.TryParse(valueDays[1], out dayTo))
									{
										if (tempWhere.Length > 0) tempWhere.Append(" AND ");

										tempWhere.AppendFormat(
											"w2_FixedPurchase.next_next_shipping_date < '{0}'",
											((dayTo == 0)
												? DateTime.Now.Date
												: DateTime.Now.AddDays(dayTo + 1).Date).ToString().Replace("'", "''"));
									}
								}

								if (tempWhere.Length != 0)
								{
									whereString.Append((whereString.Length == 0) ? string.Empty : " AND (")
										.Append(tempWhere.ToString()).Append(")");
								}
							}
							break;

						// 定期再開予定日指定
						case Constants.FIELD_FIXEDPURCHASE_RESUME_DATE:
							if (string.IsNullOrEmpty(searchValue) == false)
							{
								var resumeDateWhere = GetFixedPurchaseResumeDateWhere(searchValue);
								if (resumeDateWhere.Length != 0)
								{
									whereString.AppendFormat(
										"{0}{1})",
										((whereString.Length == 0) ? string.Empty : " AND ("),
										resumeDateWhere);
								}
							}
							break;

						case Constants.FIELD_FIXEDPURCHASE_ATTRIBUTE1:
						case Constants.FIELD_FIXEDPURCHASE_ATTRIBUTE2:
						case Constants.FIELD_FIXEDPURCHASE_ATTRIBUTE3:
						case Constants.FIELD_FIXEDPURCHASE_ATTRIBUTE4:
						case Constants.FIELD_FIXEDPURCHASE_ATTRIBUTE5:
						case Constants.FIELD_FIXEDPURCHASE_ATTRIBUTE6:
						case Constants.FIELD_FIXEDPURCHASE_ATTRIBUTE7:
						case Constants.FIELD_FIXEDPURCHASE_ATTRIBUTE8:
						case Constants.FIELD_FIXEDPURCHASE_ATTRIBUTE9:
						case Constants.FIELD_FIXEDPURCHASE_ATTRIBUTE10:
							var orderExtendQuery = OrderExtendCommon.CreateWhereQuerryOrderWorkflow(
								searchValue,
								searchField,
								Constants.TABLE_FIXEDPURCHASE);
							whereInner.Append(orderExtendQuery);
							break;

						// 頒布会コースID
						case Constants.FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID:
							if (string.IsNullOrEmpty(searchValue) == false)
							{
								whereString.Append(" AND (").Append("w2_FixedPurchase.subscription_box_course_id LIKE '")
									.Append(searchValue.Replace("'", "''")).Append("%' ");
								whereString.Append(" OR ").Append("w2_FixedPurchase.subscription_box_course_id IN ('")
									.Append(searchValue.Replace("'", "''").Replace(",", "','")).Append("'))");
							}
							break;

						// 頒布会購入回数FROM
						case WorkflowSetting.SUBSCRIPTIONBOX_ORDER_COUNT_FROM:
							if (string.IsNullOrEmpty(searchValue) == false)
							{
								whereString.Append(GetSqlAndCondition(whereString))
									.Append("w2_FixedPurchase.subscription_box_order_count >= ")
									.Append(searchValue);
							}
							break;

						// 頒布会購入回数TO
						case WorkflowSetting.SUBSCRIPTIONBOX_ORDER_COUNT_TO:
							if (string.IsNullOrEmpty(searchValue) == false)
							{
								whereString.Append(GetSqlAndCondition(whereString))
									.Append("w2_FixedPurchase.subscription_box_order_count <= ")
									.Append(searchValue);
							}
							break;

						// 並び順
						case Constants.REQUEST_KEY_SORT_KBN:
							sqlParam[ORDERWORKFLOWSETTING_SORT_KBN] = searchValue;
							break;
					}

					// 内部の条件文が存在する場合は結合
					if (whereInner.Length != 0)
					{
						whereString.Append(GetSqlAndCondition(whereString))
							.Append(whereInner.ToString());
					}
				}
			}

			if (string.IsNullOrEmpty(masterKbn) == false)
			{
				sqlParam[Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN] = masterKbn;
			}

			// 条件文が存在する場合に格納
			if (whereString.Length != 0)
			{
				sqlParam[OrderSearchParam.WORKFLOWSETTING_WHERE] = whereString.ToString();
			}
			return sqlParam;
		}

		/// <summary>
		/// Get SQL and condtion
		/// </summary>
		/// <param name="whereString">SQL where string</param>
		/// <returns>An SQL and condition as string or empty if SQL where string is empty</returns>
		private string GetSqlAndCondition(StringBuilder whereString)
		{
			var andString = (whereString.Length != 0)
				? " AND "
				: string.Empty;
			return andString;
		}

		/// <summary>
		/// 定期再開予定日のWhere句取得
		/// </summary>
		/// <param name="searchValue">抽出検索条件の値</param>
		/// <returns>Where句</returns>
		private StringBuilder GetFixedPurchaseResumeDateWhere(string searchValue)
		{
			var tempWhere = new StringBuilder();
			var hasNumberOfDays = false;
			foreach (var value in searchValue.Split(','))
			{
				switch (value)
				{
					case WorkflowSetting.m_FIXEDPURCHASE_RESUME_DATE_SPECIFIED:
						if (hasNumberOfDays) continue;

						tempWhere.Append(string.Format(
							"{0}{1}",
							((tempWhere.Length != 0) ? " OR " : string.Empty),
							"w2_FixedPurchase.resume_date IS NOT NULL"));
						break;

					case WorkflowSetting.m_FIXEDPURCHASE_RESUME_DATE_UNSPECIFIED:
						tempWhere.Append(string.Format(
							"{0}{1}",
							((tempWhere.Length != 0) ? " OR " : string.Empty),
							"w2_FixedPurchase.resume_date IS NULL"));
						break;

					default:
						var valueDays = value.Split(WorkflowSetting.m_FIXEDPURCHASE_RESUME_DATE_CHARACTER);
						if (valueDays.Length > 1)
						{
							// 定期再開予定日の日数指定有りと判断
							int dayFrom, dayTo;
							var hasDayFrom = int.TryParse(valueDays[0], out dayFrom);
							var hasDayTo = int.TryParse(valueDays[1], out dayTo);
							var presentDay = DateTime.Now.Date;
							hasNumberOfDays = (hasDayFrom || hasDayTo);

							if (hasDayFrom)
							{
								if (dayFrom == 0)
								{
									if (tempWhere.Length > 0) tempWhere.Append(" AND ");

									tempWhere.AppendFormat(
										"w2_FixedPurchase.resume_date >= '{0}'",
										(presentDay.ToString().Replace("'", "''")));
								}
								else
								{
									var numberOfAddDay = 1;
									var scheduledShippingDateFrom = presentDay;
									while (numberOfAddDay <= dayFrom)
									{
										scheduledShippingDateFrom = scheduledShippingDateFrom.AddDays(1);

										if (HolidayUtil.IsHoliday(scheduledShippingDateFrom) == false) numberOfAddDay++;
									}

									if (tempWhere.Length > 0) tempWhere.Append(" AND ");

									tempWhere.AppendFormat(
										"w2_FixedPurchase.resume_date >= '{0}'",
										scheduledShippingDateFrom.ToString().Replace("'", "''"));
								}
							}

							if (hasDayTo)
							{
								if (dayTo == 0)
								{
									tempWhere.AppendFormat(
										"w2_FixedPurchase.resume_date <= '{0}'",
										presentDay.ToString().Replace("'", "''"));
								}
								else
								{
									var numberOfAddDay = 1;
									var scheduledShippingDateTo = presentDay;
									while (numberOfAddDay <= dayTo)
									{
										scheduledShippingDateTo = scheduledShippingDateTo.AddDays(1);

										if (HolidayUtil.IsHoliday(scheduledShippingDateTo) == false) numberOfAddDay++;
									}

									if (tempWhere.Length > 0) tempWhere.Append(" AND ");
									tempWhere.AppendFormat(
										"w2_FixedPurchase.resume_date <= '{0}'",
										scheduledShippingDateTo.ToString().Replace("'", "''"));
								}
							}
						}
						break;
				}
			}
			return tempWhere;
		}

		/// <summary>
		/// ステータス更新日検索条件用日付取得
		/// </summary>
		/// <param name="strField">フィールド名</param>
		/// <param name="strStatusDate">抽出指定</param>
		/// <returns>FromTo期間</returns>
		public KeyValuePair<string, string> GetUpdateStatusDateFromTo(string strField, string strStatusDate)
		{
			var fromDatetime = DateTime.MinValue;
			var toDateTime = DateTime.MinValue;
			switch (strStatusDate)
			{
				// 当日
				case WorkflowSetting.m_SEARCH_STATUS_DATE_TODAY:
					fromDatetime = DateTime.Now;
					toDateTime = DateTime.Now;
					break;

				// 今週分
				case WorkflowSetting.m_SEARCH_STATUS_DATE_THISWEEK:
					fromDatetime = GetFirstDayOfThisWeek(WorkflowSetting.m_FIRST_DAY, DateTime.Now);
					toDateTime = fromDatetime.AddDays(6);
					break;

				// 前週分
				case WorkflowSetting.m_SEARCH_STATUS_DATE_LASTWEEK:
					fromDatetime = GetFirstDayOfThisWeek(WorkflowSetting.m_FIRST_DAY, DateTime.Now.AddDays(-7));
					toDateTime = fromDatetime.AddDays(6);
					break;

				// 今月分
				case WorkflowSetting.m_SEARCH_STATUS_DATE_THISMONTH:
					fromDatetime = DateTime.Parse(DateTime.Now.Year + "/" + DateTime.Now.Month + "/01");
					toDateTime = fromDatetime.AddMonths(1).AddDays(-1);
					break;

				// 前月分
				case WorkflowSetting.m_SEARCH_STATUS_DATE_LASTMONTH:
					fromDatetime = DateTime.Parse(DateTime.Now.Year + "/" + DateTime.Now.Month + "/01").AddMonths(-1);
					toDateTime = fromDatetime.AddMonths(1).AddDays(-1);
					break;
			}

			return new KeyValuePair<string, string>(fromDatetime.ToString("yyyy/MM/dd"), toDateTime.ToString("yyyy/MM/dd"));
		}

		/// <summary>
		/// 週の始まりの日付を取得
		/// </summary>
		/// <param name="week">週始まり曜日</param>
		/// <param name="dayDate">日付</param>
		/// <returns>週の始まりの日付</returns>
		public DateTime GetFirstDayOfThisWeek(DayOfWeek week, DateTime dayDate)
		{
			var startDateOfWeek = week - dayDate.DayOfWeek;
			if (startDateOfWeek > 0)
			{
				startDateOfWeek -= 7;
			}
			return dayDate.AddDays(startDateOfWeek);
		}

		/// <summary>
		/// ステータス更新日検索条件用時間取得
		/// </summary>
		/// <param name="fieldValues">項目配列</param>
		/// <param name="targetFieldName">ターゲットフィールド名</param>
		/// <returns>ステータス更新時間</returns>
		public string GetUpdateStatusTimeTo(string[] fieldValues, string targetFieldName)
		{
			foreach (var fieldValue in fieldValues)
			{
				var searchFields = fieldValue.Split('=');
				var searchValue = searchFields[1];
				if (searchFields[0] == targetFieldName)
				{
					return searchValue;
				}
			}

			return "0:00:00";
		}

		/// <summary>
		/// 指定したキーの抽出条件の値を取得
		/// </summary>
		/// <param name="searchSettingsFieldValues">抽出条件の配列</param>
		/// <param name="keySearch">抽出条件のキー</param>
		/// <returns>抽出条件の値</returns>
		public string GetSearchSettingValue(IEnumerable<string> searchSettingsFieldValues, string keySearch)
		{
			var result = string.Empty;
			foreach (var item in searchSettingsFieldValues.Where(item => item.StartsWith(keySearch + "=")))
			{
				result = item.Split('=')[1];
			}
			return HttpUtility.UrlDecode(result);
		}
	}
}
