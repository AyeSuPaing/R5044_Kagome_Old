/*
=========================================================================================================
  Module      : ターゲットリスト条件のDQL作成クラス(CreateTargetListSql.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using w2.Common.Util;

namespace w2.App.Common.TargetList
{
	/// <summary>
	/// ターゲットリスト条件SQL作成クラス
	/// </summary>
	[Serializable]
	class CreateTargetListSql
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CreateTargetListSql()
		{
			this.OrderExtractCondition = new List<TargetListCondition>();
			this.OrderExceptCondition = new List<TargetListCondition>();
			this.CartCondition = new List<TargetListCondition>();
			this.FixedPurchaseCondition = new List<TargetListCondition>();
			this.CouponCondition = new List<TargetListCondition>();
			this.MailClickCondition = new List<TargetListCondition>();
			this.FixedPurchaseOrderExceptCondition = new List<TargetListCondition>();
			this.FixedPurchaseOrderExtractCondition = new List<TargetListCondition>();
			this.AggregatedUserPointCondition = new List<TargetListCondition>();
			this.BasePointCondition = new List<TargetListCondition>();
			this.LimitedTermPointExtractCondition = new List<TargetListCondition>();
			this.LimitedTermPointExceptCondition = new List<TargetListCondition>();
			this.DmShippingHistoryExtractCondition = new List<TargetListCondition>();
			this.DmShippingHistoryExceptCondition = new List<TargetListCondition>();
			this.SqlCondition = new List<TargetListCondition>();
			this.OrderCountCondition = new List<TargetListCondition>();
			this.OrderPriceSumCondition = new List<TargetListCondition>();
		}

		/// <summary>
		/// 手動設定抽出条件SQL作成
		/// </summary>
		/// <returns>抽出条件SQL</returns>
		public string CreateManualSql(ITargetListCondition condition)
		{
			var result = new StringBuilder();

			//------------------------------------------------------
			// 条件を振り分け
			//------------------------------------------------------
			string conditionType = null;
			var conditionDataKbnList = new List<string>();

			if (condition is TargetListConditionGroup)
			{
				// グループ化されていた場合グループコンディションタイプ格納
				conditionType = ((TargetListConditionGroup)condition).TargetGroup[0].GroupConditionType;

				foreach (var tlc in ((TargetListConditionGroup)condition).TargetGroup)
				{
					SortCondition(tlc);
					conditionDataKbnList.Add(tlc.DataKbn);
				}
			}
			else
			{
				var tlc = (TargetListCondition)condition;
				conditionType = tlc.ConditionType;
				conditionDataKbnList.Add(tlc.DataKbn);
				SortCondition(tlc);
			}

			//------------------------------------------------------
			// 抽出条件追加
			//------------------------------------------------------

			// 最初の条件にAND、OR条件はいらないのでnull格納、２番目からはコンディションタイプ格納
			string appendConditionType = null;

			// ANDなら全部合わせる、ORなら別々にSQL作成
			if (conditionType == TargetListCondition.CONDITION_TYPE_AND)
			{
				if ((this.OrderExtractCondition.Count != 0)
					|| (this.FixedPurchaseOrderExtractCondition.Count != 0)
					|| (this.CartCondition.Count != 0)
					|| (this.FixedPurchaseCondition.Count != 0)
					|| (this.CouponCondition.Count != 0)
					|| (this.MailClickCondition.Count != 0)
					|| (this.BasePointCondition.Count != 0)
					|| (this.LimitedTermPointExtractCondition.Count != 0)
					|| (this.DmShippingHistoryExtractCondition.Count != 0)
					|| (this.AggregatedUserPointCondition.Count != 0)
					|| (this.OrderCountCondition.Count > 0)
					|| (this.OrderPriceSumCondition.Count > 0))
				{
					AppendExtractBaseCondition(result, conditionDataKbnList);

					// 条件作成：カート情報
					// ※ユーザ1レコードに対し、カート情報はNレコード存在する。
					// 　そのため、外部結合を行うと、ユーザ情報のレコード件数 X 注文情報のレコード件数 X カート情報の件数が取得されてしまう。（=クロス結合）
					// 　パフォーマンスが悪化する原因になるため、内部結合で抽出を行う。
					result.Append(CreateManualSqlWhereForCart(this.CartCondition, conditionType));

					// 条件作成：定期購入情報
					// ※ユーザ1レコードに対し、定期購入情報はNレコード存在する。
					// 　そのため、外部結合を行うと、ユーザ情報のレコード件数 X 注文情報のレコード件数 X 定期購入報の件数が取得されてしまう。（=クロス結合）
					// 　パフォーマンスが悪化する原因になるため、内部結合で抽出を行う。
					result.Append(CreateManualSqlWhereForFixedPurchase(this.FixedPurchaseCondition, conditionType, true));

					// 条件作成：クーポン情報
					// ※ユーザ1レコードに対し、クーポン情報はNレコード存在する。
					// 　そのため、外部結合を行うと、ユーザ情報のレコード件数 X 注文情報のレコード件数 X クーポン情報の件数が取得されてしまう。
					// 　パフォーマンスが悪化する原因になるため、内部結合で抽出を行う。
					result.Append(CreateManualSqlWhereForCoupon(this.CouponCondition, conditionType));

					// 条件作成：メールクリック結果
					// ※ユーザ1レコードに対し、メールクリック結果はNレコード存在する。
					// 　そのため、外部結合を行うと、ユーザ情報のレコード件数 X 注文情報のレコード件数 X クーポン情報の件数が取得されてしまう。
					// 　パフォーマンスが悪化する原因になるため、内部結合で抽出を行う。
					result.Append(CreateManualSqlWhereForMailClick(this.MailClickCondition, conditionType));

					// 条件作成：ユーザーポイント情報（合算）
					result.Append(CreateManualSqlWhereForAggregatedUserPoint(this.AggregatedUserPointCondition, conditionType));

					// 条件作成：ユーザーポイント情報（通常）
					result.Append(CreateManualSqlWhereForUserBasePoint(this.BasePointCondition, conditionType));

					// 条件作成：ユーザーポイント情報（期間限定ポイント）
					result.Append(CreateManualSqlWhereForLimitedTermPoint(this.LimitedTermPointExtractCondition, conditionType, true));

					// 条件作成：定期注文情報
					result.Append(CreateManualSqlWhereForFixedPurchase(this.FixedPurchaseOrderExtractCondition, conditionType, true));

					// 条件作成：DM発送履歴情報
					result.Append(CreateManualSqlWhereForDmShippingHistory(this.DmShippingHistoryExtractCondition, conditionType, true));

					// 条件作成：ユーザー、ユーザ拡張、ユーザ属性、ポイント、注文情報、お気に入り商品情報
					var sqlWhere = CreateManualSqlWhereForUserAndOrder(this.OrderExtractCondition, conditionType)
						+ CreateManualSqlWhereForUserAndOrderAdditional(this.OrderExtractCondition, conditionType);
					if (string.IsNullOrEmpty(sqlWhere) == false)
					{
						result.Append("	 WHERE													\n");
						result.Append(sqlWhere);
					}

					// 回数と金額合計SQL作成
					result.Append(CreateSqlForOrderCountAndProductSum(
						"w2_User.user_id",
						this.OrderCountCondition,
						this.OrderPriceSumCondition));

					result.Append(")").Append("\n");
					appendConditionType = conditionType;
				}

				//------------------------------------------------------
				// 除外条件追加（除外条件はそれぞれに w2_User.user_id NOT IN SQLを作成する）
				//------------------------------------------------------
				AppendExceptCondition(result, ref appendConditionType, conditionType);

				//------------------------------------------------------
				// 直SQL条件追加
				//------------------------------------------------------
				AppendSqlCondition(result, appendConditionType, conditionType);
			}
			else
			{
				if (this.OrderExtractCondition.Count != 0)
				{
					AppendExtractBaseCondition(result, conditionDataKbnList);
					// 条件作成：ユーザー、ユーザ拡張、ユーザ属性、ポイント、注文情報、お気に入り商品情報
					var sqlWhere = CreateManualSqlWhereForUserAndOrder(this.OrderExtractCondition, conditionType)
						+ CreateManualSqlWhereForUserAndOrderAdditional(this.OrderExtractCondition, conditionType);
					if (string.IsNullOrEmpty(sqlWhere) == false)
					{
						result.Append("	 WHERE													\n");
						result.Append(sqlWhere);
					}
					result.Append(")").Append("\n");
					appendConditionType = conditionType;
				}

				if (this.CartCondition.Count != 0)
				{
					result.Append("			" + appendConditionType + "\n");
					AppendExtractBaseCondition(result, conditionDataKbnList);
					result.Append(CreateManualSqlWhereForCart(this.CartCondition, conditionType));
					result.Append(")").Append("\n");
					appendConditionType = conditionType;
				}

				if (this.FixedPurchaseCondition.Count != 0)
				{
					result.Append("			" + appendConditionType + "\n");
					AppendExtractBaseCondition(result, conditionDataKbnList);
					result.Append(CreateManualSqlWhereForFixedPurchase(this.FixedPurchaseCondition, conditionType, true));
					result.Append(")").Append("\n");
					appendConditionType = conditionType;
				}

				if (this.FixedPurchaseOrderExtractCondition.Count != 0)
				{
					result.Append("			" + appendConditionType + "\n");
					AppendExtractBaseCondition(result, conditionDataKbnList);
					result.Append(CreateManualSqlWhereForFixedPurchase(this.FixedPurchaseOrderExtractCondition, conditionType, true))
						.Append(")")
						.Append("\n");
					appendConditionType = conditionType;
				}

				if (this.CouponCondition.Count != 0)
				{
					result.Append("			" + appendConditionType + "\n");
					AppendExtractBaseCondition(result, conditionDataKbnList);
					result.Append(CreateManualSqlWhereForCoupon(this.CouponCondition, conditionType));
					result.Append(")").Append("\n");
					appendConditionType = conditionType;
				}

				if (this.MailClickCondition.Count != 0)
				{
					result.Append("			" + appendConditionType + "\n");
					AppendExtractBaseCondition(result, conditionDataKbnList);
					result.Append(CreateManualSqlWhereForMailClick(this.MailClickCondition, conditionType));
					result.Append(")").Append("\n");
					appendConditionType = conditionType;
				}

				if (this.BasePointCondition.Count != 0)
				{
					result.Append("			" + appendConditionType + "\n");
					AppendExtractBaseCondition(result, conditionDataKbnList);
					result.Append(CreateManualSqlWhereForUserBasePoint(this.BasePointCondition, conditionType));
					result.Append(")").Append("\n");
					appendConditionType = conditionType;
				}

				if (this.LimitedTermPointExtractCondition.Count != 0)
				{
					result.Append("			" + appendConditionType + "\n");
					AppendExtractBaseCondition(result, conditionDataKbnList);
					result.Append(CreateManualSqlWhereForLimitedTermPoint(this.LimitedTermPointExtractCondition, conditionType, true))
						.Append(")")
						.Append("\n");
					appendConditionType = conditionType;
				}

				if (this.AggregatedUserPointCondition.Count != 0)
				{
					result.Append("			" + appendConditionType + "\n");
					AppendExtractBaseCondition(result, conditionDataKbnList);
					result.Append(CreateManualSqlWhereForAggregatedUserPoint(this.AggregatedUserPointCondition, conditionType));
					result.Append(")").Append("\n");
					appendConditionType = conditionType;
				}

				// 注文回数
				if (this.OrderCountCondition.Count > 0)
				{
					result.Append("			" + appendConditionType + "\n");
					AppendExtractBaseCondition(result, conditionDataKbnList);
					result.Append(CreateSqlForOrderCountAndProductSum(
						"w2_User.user_id",
						this.OrderCountCondition,
						new List<TargetListCondition>()));
					result.Append(")").Append("\n");
					appendConditionType = conditionType;
				}

				// 金額合計
				if (this.OrderPriceSumCondition.Count > 0)
				{
					result.Append("			" + appendConditionType + "\n");
					AppendExtractBaseCondition(result, conditionDataKbnList);
					result.Append(CreateSqlForOrderCountAndProductSum(
						"w2_User.user_id",
						new List<TargetListCondition>(),
						this.OrderPriceSumCondition));
					result.Append(")").Append("\n");
					appendConditionType = conditionType;
				}

				if (this.DmShippingHistoryExtractCondition.Count != 0)
				{
					result.Append("			" + appendConditionType + "\n");
					AppendExtractBaseCondition(result, conditionDataKbnList);
					result.Append(CreateManualSqlWhereForDmShippingHistory(this.DmShippingHistoryExtractCondition, conditionType, true))
						.Append(")")
						.Append("\n");
					appendConditionType = conditionType;
				}

				//------------------------------------------------------
				// 除外条件追加（除外条件はそれぞれに w2_User.user_id NOT IN SQLを作成する）
				//------------------------------------------------------
				AppendExceptCondition(result, ref appendConditionType, conditionType);

				//------------------------------------------------------
				// 直SQL条件追加
				//------------------------------------------------------
				AppendSqlCondition(result, appendConditionType, conditionType);
			}

			return result.ToString();
		}

		/// <summary>
		/// 除外条件追加（除外条件はそれぞれに w2_User.user_id NOT IN SQLを作成する）
		/// </summary>
		/// <param name="sqlString">SQL文字列</param>
		/// <param name="appendConditionType">追加条件</param>
		/// <param name="conditionType">AND、OR条件</param>
		private void AppendExceptCondition(
			StringBuilder sqlString,
			ref string appendConditionType,
			string conditionType)
		{
			if (this.OrderExceptCondition.Count != 0)
			{
				AppendExceptOrderCondition(sqlString, appendConditionType, conditionType);
				appendConditionType = conditionType;
			}

			if (this.FixedPurchaseOrderExceptCondition.Count != 0)
			{
				AppendExceptFixedPurchaseOrderCondition(sqlString, appendConditionType, conditionType);
				appendConditionType = conditionType;
			}

			if (this.LimitedTermPointExceptCondition.Count != 0)
			{
				AppendExceptLimitedTermPointCondition(sqlString, appendConditionType, conditionType);
				appendConditionType = conditionType;
			}

			if (this.DmShippingHistoryExceptCondition.Count != 0)
			{
				AppendExceptDmShippingHistoryCondition(sqlString, appendConditionType, conditionType);
				appendConditionType = conditionType;
			}
		}

		/// <summary>
		/// 条件をKbnで分ける
		/// </summary>
		/// <param name="tlc">条件</param>
		private void SortCondition(TargetListCondition tlc)
		{
			switch (tlc.DataKbn)
			{
				// ユーザー、ユーザ拡張、ユーザ属性、ポイント、カート、定期購入、保有クーポン、メールクリック結果、DM発送履歴
				case TargetListCondition.DATAKBN_USER_INFO:
				case TargetListCondition.DATAKBN_USER_EXTEND_INFO:
				case TargetListCondition.DATAKBN_USER_ATTRIBUTE_INFO:
				case TargetListCondition.DATAKBN_ORDER_EXTEND_INFO:
					this.OrderExtractCondition.Add(tlc);
					break;

				case TargetListCondition.DATAKBN_CART_INFO:
					this.CartCondition.Add(tlc);
					break;

				case TargetListCondition.DATAKBN_FIXEDPURCHASE_ORDER_EXTEND_INFO:
					this.FixedPurchaseOrderExtractCondition.Add(tlc);
					break;

				case TargetListCondition.DATAKBN_FIXEDPURCHASE_INFO:
					if (tlc.FixedPurchaseOrderExist == TargetListCondition.FIXEDPURCHASEORDEREXIST_NOTEXIST)
					{
						this.FixedPurchaseOrderExceptCondition.Add(tlc);
					}
					else
					{
						this.FixedPurchaseOrderExtractCondition.Add(tlc);
					}
					break;

				case TargetListCondition.DATAKBN_COUPON_INFO:
					this.CouponCondition.Add(tlc);
					break;

				case TargetListCondition.DATAKBN_MAIL_CLICK_INFO:
					this.MailClickCondition.Add(tlc);
					break;

				case TargetListCondition.DATAKBN_POINT_INFO:
					this.BasePointCondition.Add(tlc);
					break;

				case TargetListCondition.DATAKBN_AGGREGATED_POINT_INFO:
					this.AggregatedUserPointCondition.Add(tlc);
					break;

				// 注文情報
				case TargetListCondition.DATAKBN_ORDER_INFO:
					if (tlc.OrderExist == TargetListCondition.ORDEREXIST_NOTEXIST)
					{
						this.OrderExceptCondition.Add(tlc);
					}
					else
					{
						this.OrderExtractCondition.Add(tlc);
					}
					break;

				// 受注情報（集計）
				case TargetListCondition.DATAKBN_ORDER_AGGREGATE:
					// 購入回数
					if (tlc.DataType == TargetListCondition.DATATYPE_COUNT)
					{
						this.OrderCountCondition.Add(tlc);
					}
					// 金額合計
					else if (tlc.DataType == TargetListCondition.DATATYPE_SUM)
					{
						this.OrderPriceSumCondition.Add(tlc);
					}
					break;

				// 期間限定ポイント情報
				case TargetListCondition.DATAKBN_LIMITED_TERM_POINT_INFO:
					if (tlc.PointExist == TargetListCondition.POINTEXIST_NOTEXIST)
					{
						this.LimitedTermPointExceptCondition.Add(tlc);
					}
					else
					{
						this.LimitedTermPointExtractCondition.Add(tlc);
					}
					break;

				case TargetListCondition.DATAKBN_DM_SHIPPING_HISTORY_INFO:
					if (tlc.DmShippingHistoryExist == TargetListCondition.DMSHIPPINGHISTORYINFO_NOTEXIST)
					{
						this.DmShippingHistoryExceptCondition.Add(tlc);
					}
					else
					{
						this.DmShippingHistoryExtractCondition.Add(tlc);
					}
					break;

				// 直SQL条件
				case TargetListCondition.DATAKBN_SQL_CONDITION:
					this.SqlCondition.Add(tlc);
					break;

				case TargetListCondition.DATAKBN_FAVORITE_PRODUCT_INFO:
					if (tlc.FavoriteExist == TargetListCondition.FAVORITEEXIST_EXIST)
					{
						// お気に入り商品情報もオーダーイクストラクト
						this.OrderExtractCondition.Add(tlc);
					}
					else
					{
						this.OrderExceptCondition.Add(tlc);
					}
					break;
			}
		}

		/// <summary>
		/// Extractの条件の基本のSQL文
		/// </summary>
		/// <param name="result">全体のSQL文</param>
		/// <param name="conditionDataKbnList">Condition data kbn list</param>
		private void AppendExtractBaseCondition(StringBuilder result, List<string> conditionDataKbnList)
		{
			result.Append("		w2_User.user_id IN (								\n");
			result.Append("	SELECT	w2_User.user_id									\n");
			result.Append("	  FROM	w2_User											\n");

			if (conditionDataKbnList.Contains(TargetListCondition.DATAKBN_USER_EXTEND_INFO))
			{
				result.Append("			LEFT JOIN w2_UserExtend ON						\n");
				result.Append("			(												\n");
				result.Append("				w2_User.user_id = w2_UserExtend.user_id		\n");
				result.Append("			)												\n");
			}
			if (conditionDataKbnList.Contains(TargetListCondition.DATAKBN_USER_ATTRIBUTE_INFO))
			{
				result.Append("			LEFT JOIN w2_UserAttribute ON					\n");
				result.Append("			(												\n");
				result.Append("				w2_User.user_id = w2_UserAttribute.user_id	\n");
				result.Append("			)												\n");
			}
			if (conditionDataKbnList.Contains(TargetListCondition.DATAKBN_ORDER_INFO)
				|| conditionDataKbnList.Contains(TargetListCondition.DATAKBN_ORDER_AGGREGATE)
				|| conditionDataKbnList.Contains(TargetListCondition.DATAKBN_FIXEDPURCHASE_INFO)
				|| conditionDataKbnList.Contains(TargetListCondition.DATAKBN_ORDER_EXTEND_INFO)
				|| conditionDataKbnList.Contains(TargetListCondition.DATAKBN_FIXEDPURCHASE_ORDER_EXTEND_INFO))
			{
				result.Append("			LEFT JOIN w2_Order ON							\n");
				result.Append("			(												\n");
				result.Append("				w2_User.user_id = w2_Order.user_id			\n");
				result.Append("			)												\n");
				result.Append("			LEFT JOIN w2_OrderItem ON 						\n");
				result.Append("			(												\n");
				result.Append("				w2_Order.order_id = w2_OrderItem.order_id	\n");
				result.Append("			)												\n");
				result.Append("			LEFT JOIN w2_OrderSetPromotion ON				\n");
				result.Append("			(												\n");
				result.Append("				w2_Order.order_id = w2_OrderSetPromotion.order_id	\n");
				result.Append("			)												\n");
				result.Append("			LEFT JOIN w2_OrderShipping ON					\n");
				result.Append("			(												\n");
				result.Append("				w2_Order.order_id = w2_OrderShipping.order_id	\n");
				result.Append("			)												\n");
				result.Append("			LEFT JOIN w2_OrderOwner ON						\n");
				result.Append("			(												\n");
				result.Append("				w2_Order.order_id = w2_OrderOwner.order_id	\n");
				result.Append("			)												\n");
				result.Append("			LEFT JOIN w2_OrderCoupon ON						\n");
				result.Append("			(												\n");
				result.Append("				w2_Order.order_id = w2_OrderCoupon.order_id	\n");
				result.Append("			)												\n");
				result.Append("			LEFT JOIN w2_Product ON							\n");
				result.Append("			(												\n");
				result.Append("				w2_Product.shop_id = w2_OrderItem.shop_id	\n");
				result.Append("				AND											\n");
				result.Append("				w2_Product.product_id = w2_OrderItem.product_id	\n");
				result.Append("			)												\n");
			}
			if (conditionDataKbnList.Contains(TargetListCondition.DATAKBN_FAVORITE_PRODUCT_INFO))
			{
				result.Append("			LEFT JOIN w2_Favorite ON						\n");
				result.Append("			(												\n");
				result.Append("				w2_Favorite.user_id = w2_User.user_id		\n");
				result.Append("			)												\n");
			}
			result.Append(CreateJoinSqlExceptOrder());
			result.Append(CreateJoinSqlForProductSum());
		}

		/// <summary>
		/// Except条件の注文SQL文作成
		/// </summary>
		/// <param name="result">全体のSQL文</param>
		/// <param name="appendConditionType">追加条件</param>
		/// <param name="conditionType">AND、OR条件</param>
		private void AppendExceptOrderCondition(
			StringBuilder result,
			string appendConditionType,
			string conditionType)
		{
			result.Append("			" + appendConditionType + "\n");
			result.Append("		w2_User.user_id NOT IN (							\n");
			result.Append("	SELECT	w2_Order.user_id								\n");
			result.Append("	  FROM	w2_Order										\n");
			result.Append("			LEFT JOIN w2_OrderItem ON 						\n");
			result.Append("			(												\n");
			result.Append("				w2_Order.order_id = w2_OrderItem.order_id	\n");
			result.Append("			)												\n");
			result.Append("			LEFT JOIN w2_OrderSetPromotion ON				\n");
			result.Append("			(												\n");
			result.Append("				w2_Order.order_id = w2_OrderSetPromotion.order_id	\n");
			result.Append("			)												\n");
			result.Append("			LEFT JOIN w2_OrderShipping ON					\n");
			result.Append("			(												\n");
			result.Append("				w2_Order.order_id = w2_OrderShipping.order_id	\n");
			result.Append("			)												\n");
			result.Append("			LEFT JOIN w2_OrderOwner ON						\n");
			result.Append("			(												\n");
			result.Append("				w2_Order.order_id = w2_OrderOwner.order_id	\n");
			result.Append("			)												\n");
			result.Append("			LEFT JOIN w2_OrderCoupon ON						\n");
			result.Append("			(												\n");
			result.Append("				w2_Order.order_id = w2_OrderCoupon.order_id	\n");
			result.Append("			)												\n");
			result.Append("			LEFT JOIN w2_Product ON							\n");
			result.Append("			(												\n");
			result.Append("				w2_Product.shop_id = w2_OrderItem.shop_id	\n");
			result.Append("				AND											\n");
			result.Append("				w2_Product.product_id = w2_OrderItem.product_id	\n");
			result.Append("			)												\n");
			result.Append("			LEFT JOIN w2_Favorite ON						\n");
			result.Append("			(												\n");
			result.Append("				w2_Favorite.user_id = w2_User.user_id		\n");
			result.Append("			)												\n");

			// NOTINなので今のAND、ORとは逆の条件でSQL作成
			var oppositeConditionType = (conditionType == TargetListCondition.CONDITION_TYPE_AND)
				? TargetListCondition.CONDITION_TYPE_OR
				: TargetListCondition.CONDITION_TYPE_AND;
			var sqlWhereTemp = new StringBuilder();
			var sqlWhereForUserAndOrder = CreateManualSqlWhereForUserAndOrder(this.OrderExceptCondition, oppositeConditionType);
			if (sqlWhereForUserAndOrder.Length > 0)
			{
				sqlWhereTemp.Append("	 WHERE													\n");
				sqlWhereTemp.Append(sqlWhereForUserAndOrder);
			}

			if (sqlWhereTemp.Length > 0)
			{
				result.Append(sqlWhereTemp);
			}

			result.Append(")\n"); // w2_User.user_id NOT IN(～ を閉じる
		}

		/// <summary>
		/// Except条件の定期購入注文SQL文作成
		/// </summary>
		/// <param name="result">全体のSQL文</param>
		/// <param name="appendConditionType">追加条件</param>
		/// <param name="conditionType">AND、OR条件</param>
		private void AppendExceptFixedPurchaseOrderCondition(
			StringBuilder result,
			string appendConditionType,
			string conditionType)
		{
			result.Append("			" + appendConditionType + "\n");
			result.Append("		w2_User.user_id NOT IN (							\n");
			result.Append(
				CreateManualSqlWhereForFixedPurchase(this.FixedPurchaseOrderExceptCondition, conditionType, false));
			result.Append(")\n"); // w2_User.user_id NOT IN(～ を閉じる
		}

		/// <summary>
		/// Except条件の期間限定ポイントSQL文作成
		/// </summary>
		/// <param name="result">全体のSQL文</param>
		/// <param name="appendConditionType">追加条件</param>
		/// <param name="conditionType">AND、OR条件</param>
		private void AppendExceptLimitedTermPointCondition(
			StringBuilder result,
			string appendConditionType,
			string conditionType)
		{
			result.Append("			" + appendConditionType + "\n");
			result.Append("		w2_User.user_id NOT IN (							\n");
			result.Append(
				CreateManualSqlWhereForLimitedTermPoint(this.LimitedTermPointExceptCondition, conditionType, false));
			result.Append(")\n"); // w2_User.user_id NOT IN(～ を閉じる
		}

		/// <summary>
		/// Except条件のDM送信履歴SQL文作成
		/// </summary>
		/// <param name="result">全体のSQL文</param>
		/// <param name="appendConditionType">追加条件</param>
		/// <param name="conditionType">AND、OR条件</param>
		private void AppendExceptDmShippingHistoryCondition(
			StringBuilder result,
			string appendConditionType,
			string conditionType)
		{
			result.Append("			" + appendConditionType + "\n");
			result.Append("		w2_User.user_id NOT IN (							\n");
			result.Append(
				CreateManualSqlWhereForDmShippingHistory(this.DmShippingHistoryExceptCondition, conditionType, false));
			result.Append(")\n"); // w2_User.user_id NOT IN(～ を閉じる
		}

		/// <summary>
		/// 直SQLの条件でSQL作成
		/// </summary>
		/// <param name="result">全体のSQL文</param>
		/// <param name="conditionType">AND、OR条件</param>
		private void AppendSqlCondition(StringBuilder result, string appendConditionType, string conditionType)
		{
			if (this.SqlCondition.Count != 0)
			{
				result.Append("			" + appendConditionType + "\n");

				var index = 0;
				foreach (TargetListCondition tlc in this.SqlCondition)
				{
					if (index != 0) result.Append("			" + conditionType + "\n");
					result.Append("		w2_User.user_id IN ( \n");
					result.Append(tlc.Values[0].Value);
					result.Append(")").Append("\n");
					index++;
				}
			}
		}

		/// <summary>
		/// ユーザ・注文抽出条件SQL作成
		/// </summary>
		/// <param name="conditionList">条件リスト</param>
		/// <param name="logicalOperation">論理演算（AND or OR）</param>
		/// <returns>抽出条件SQL</returns>
		private string CreateManualSqlWhereForUserAndOrder(
			List<TargetListCondition> conditionList,
			string logicalOperation)
		{
			// 条件作成：ユーザー、ユーザ拡張、ユーザ属性、注文情報、DM発送履歴
			var targetConditionList = conditionList.Where(
				c => ((c.DataKbn == TargetListCondition.DATAKBN_USER_INFO)
					|| (c.DataKbn == TargetListCondition.DATAKBN_USER_EXTEND_INFO)
					|| (c.DataKbn == TargetListCondition.DATAKBN_USER_ATTRIBUTE_INFO)
					|| (c.DataKbn == TargetListCondition.DATAKBN_ORDER_INFO)
					|| (c.DataKbn == TargetListCondition.DATAKBN_ORDER_EXTEND_INFO)
					|| (c.DataKbn == TargetListCondition.DATAKBN_DM_SHIPPING_HISTORY_INFO)
					|| (c.DataKbn == TargetListCondition.DATAKBN_FAVORITE_PRODUCT_INFO))).ToList();

			if (targetConditionList.Count == 0) return string.Empty;

			var sqlList = new List<string>();
			targetConditionList.ForEach(c =>
				sqlList.Add(CreateSqlWhereForOrderAndFixedPurchase(c) + CreateNotNullStatementlWhere(c.DataKbn)));
			return string.Join("			" + logicalOperation + "\n", sqlList);
		}

		/// <summary>
		/// ユーザ・注文抽出追加条件SQL作成
		/// </summary>
		/// <param name="conditionList">条件リスト</param>
		/// <param name="logicalOperation">論理演算（AND or OR）</param>
		/// <returns>抽出条件SQL</returns>
		private string CreateManualSqlWhereForUserAndOrderAdditional(
			List<TargetListCondition> conditionList,
			string logicalOperation)
		{
			var result = new StringBuilder();
			foreach (var condition in conditionList.Where(c => (c.DataKbn == TargetListCondition.DATAKBN_POINT_INFO)))
			{
				switch (condition.DataField)
				{
					case "w2_UserPoint.point_exp":
						result.Append("			" + logicalOperation + "\n").Append(" w2_UserPoint.point > 0");
						break;
				}

				break;
			}

			return result.ToString();
		}

		/// <summary>
		/// カート抽出条件SQL作成
		/// </summary>
		/// <param name="conditionList">条件リスト</param>
		/// <param name="logicalOperation">AND、OR条件</param>
		/// <returns>抽出条件SQL</returns>
		private string CreateManualSqlWhereForCart(List<TargetListCondition> conditionList, string logicalOperation)
		{
			var result = new StringBuilder();

			// 条件作成：カート
			var targetConditionList = conditionList.Where(c => (c.DataKbn == TargetListCondition.DATAKBN_CART_INFO));
			// 条件あり?
			if (targetConditionList.Any())
			{
				result.Append("			INNER JOIN (\n");
				result.Append("				SELECT DISTINCT w2_Cart.user_id").Append("\n");
				result.Append("				  FROM w2_Cart").Append("\n");
				result.Append("				 WHERE").Append("\n");
				result.Append(
					string.Join("			" + logicalOperation + "\n", targetConditionList.Select(c => CreateManualSqlWhere(c)).ToArray()));
				result.Append("					   ) AS cartTemp").Append("\n");
				result.Append("					ON (\n");
				result.Append("						w2_User.user_id = cartTemp.user_id\n");
				result.Append("					     )\n");
			}

			return result.ToString();
		}

		/// <summary>
		/// 定期購入情報抽出条件SQL作成
		/// </summary>
		/// <param name="conditionList">条件リスト</param>
		/// <param name="logicalOperation">AND、OR条件</param>
		/// <param name="createJoin">JOIN句作成（「・・がある」条件はJOIN結合するが、「・・がない」条件は別SQLにする</param>
		/// <returns>抽出条件SQL</returns>
		private string CreateManualSqlWhereForFixedPurchase(
			List<TargetListCondition> conditionList,
			string logicalOperation,
			bool createJoin)
		{
			var result = new StringBuilder();

			// 条件作成：定期購入情報
			var targetConditionList = conditionList.Where(
				c => (c.DataKbn == TargetListCondition.DATAKBN_FIXEDPURCHASE_INFO)
					|| (c.DataKbn == TargetListCondition.DATAKBN_FIXEDPURCHASE_ORDER_EXTEND_INFO)).ToList();
			// 条件あり?
			if (targetConditionList.Any())
			{
				if (createJoin) result.Append("			INNER JOIN (\n");
				result.Append("				SELECT DISTINCT w2_FixedPurchase.user_id").Append("\n");
				result.Append("				  FROM w2_FixedPurchase").Append("\n");
				result.Append("				   INNER JOIN w2_FixedPurchaseShipping ON").Append("\n");
				result.Append("				   (").Append("\n");
				result.Append("						w2_FixedPurchase.fixed_purchase_id = w2_FixedPurchaseShipping.fixed_purchase_id")
					.Append("\n");
				result.Append("				   )").Append("\n");
				result.Append("				   INNER JOIN w2_FixedPurchaseItem ON").Append("\n");
				result.Append("				   (").Append("\n");
				result.Append(
						"						w2_FixedPurchaseShipping.fixed_purchase_id = w2_FixedPurchaseItem.fixed_purchase_id")
					.Append("	\n");
				result.Append("						AND").Append("\n");
				result.Append(
						"						w2_FixedPurchaseShipping.fixed_purchase_shipping_no = w2_FixedPurchaseItem.fixed_purchase_shipping_no")
					.Append("\n");
				result.Append("				   )").Append("\n");
				result.Append("				   INNER JOIN w2_Product ON").Append("\n");
				result.Append("				   (").Append("\n");
				result.Append("						w2_Product.shop_id = w2_FixedPurchaseItem.shop_id").Append("    \n");
				result.Append("						AND").Append("\n");
				result.Append("						w2_Product.product_id = w2_FixedPurchaseItem.product_id").Append("\n");
				result.Append("				   )").Append("\n");
				result.Append("				 WHERE").Append("\n");
				result.Append(
					string.Join("			" + logicalOperation + "\n",
						targetConditionList.Select(CreateSqlWhereForOrderAndFixedPurchase)));
				if (createJoin)
				{
					result.Append("					   ) AS fixedPurchaseTemp").Append("\n");
					result.Append("					ON (\n");
					result.Append("						w2_User.user_id = fixedPurchaseTemp.user_id\n");
					result.Append("					     )\n");
				}
			}

			return result.ToString();
		}

		/// <summary>
		/// DM発送歴情報抽出条件SQL作成
		/// </summary>
		/// <param name="conditionList">条件リスト</param>
		/// <param name="logicalOperation">AND、OR条件</param>
		/// <param name="createJoin">JOIN句作成（「・・がある」条件はJOIN結合するが、「・・がない」条件は別SQLにする</param>
		/// <returns>抽出条件SQL</returns>
		private string CreateManualSqlWhereForDmShippingHistory(
			List<TargetListCondition> conditionList,
			string logicalOperation,
			bool createJoin)
		{
			var result = new StringBuilder();

			// 条件作成：定期購入情報
			var targetConditionList = conditionList.Where(
				c => (c.DataKbn == TargetListCondition.DATAKBN_DM_SHIPPING_HISTORY_INFO)
					|| (c.DataKbn == TargetListCondition.DATAKBN_DM_SHIPPING_HISTORY_INFO)).ToList();
			// 条件あり?
			if (targetConditionList.Any())
			{
				if (createJoin) result.Append("			INNER JOIN (\n");
				result.Append("				SELECT DISTINCT w2_DmShippingHistory.user_id").Append("\n");
				result.Append("				  FROM w2_DmShippingHistory").Append("\n");
				result.Append("				 WHERE").Append("\n");

				var sqlWhere = new List<string>();
				targetConditionList.ForEach(c => sqlWhere.Add(CreateManualSqlWhere(c)));
				result.Append(string.Join("			" + logicalOperation + "\n", sqlWhere));
				if (createJoin)
				{
					result.Append("					   ) AS dmShippingHistory").Append("\n");
					result.Append("					ON (\n");
					result.Append("						w2_User.user_id = dmShippingHistory.user_id\n");
					result.Append("					     )\n");
				}
			}

			return result.ToString();
		}

		/// <summary>
		/// クーポン情報抽出条件SQL作成
		/// </summary>
		/// <param name="conditionList">条件リスト</param>
		/// <param name="logicalOperation">AND、OR条件</param>
		/// <returns>抽出条件SQL</returns>
		private string CreateManualSqlWhereForCoupon(List<TargetListCondition> conditionList, string logicalOperation)
		{
			var result = new StringBuilder();

			// 条件作成：クーポン情報
			var targetConditionList = conditionList.Where(c => (c.DataKbn == TargetListCondition.DATAKBN_COUPON_INFO));
			// 条件あり?
			if (targetConditionList.Any())
			{
				result.Append("			INNER JOIN (").Append("\n");
				result.Append("				SELECT DISTINCT tempUserCouponUseFlgCount.user_id").Append("\n");
				result.Append("				  FROM (").Append("\n");
				result.Append("					SELECT w2_User.user_id,").Append("\n");
				result.Append("						   ISNULL(UserCouponUsedCount.used_count, 0) AS used_count,").Append("\n");
				result.Append("						   ISNULL(UserCouponUnusedCount.unused_count, 0) AS unused_count").Append("\n");
				result.Append("					FROM w2_user").Append("\n");
				result.Append("					 LEFT JOIN (").Append("\n");
				result.Append("						SELECT w2_UserCoupon.user_id, COUNT(*) AS used_count").Append("\n");
				result.Append("						  FROM w2_UserCoupon").Append("\n");
				result.Append("						 WHERE w2_UserCoupon.use_flg = '1'").Append("\n");
				result.Append("						GROUP BY w2_UserCoupon.user_id").Append("\n");
				result.Append("					 ) AS UserCouponUsedCount ON").Append("\n");
				result.Append("						w2_user.user_id = UserCouponUsedCount.user_id").Append("\n");
				result.Append("					 LEFT JOIN (").Append("\n");
				result.Append("						SELECT w2_UserCoupon.user_id, COUNT(*) AS unused_count").Append("\n");
				result.Append("						  FROM w2_UserCoupon").Append("\n");
				result.Append("						 WHERE w2_UserCoupon.use_flg = '0'").Append("\n");
				result.Append("						 GROUP BY w2_UserCoupon.user_id").Append("\n");
				result.Append("					 ) AS UserCouponUnusedCount ON").Append("\n");
				result.Append("						w2_user.user_id = UserCouponUnusedCount.user_id").Append("\n");
				result.Append("				) AS tempUserCouponUseFlgCount").Append("\n");
				result.Append("				INNER JOIN w2_UserCoupon ON").Append("\n");
				result.Append("				(").Append("\n");
				result.Append("					tempUserCouponUseFlgCount.user_id = w2_UserCoupon.user_id").Append("\n");
				result.Append("				)").Append("\n");
				result.Append("				INNER JOIN w2_Coupon ON").Append("\n");
				result.Append("				 (").Append("\n");
				result.Append("					w2_UserCoupon.dept_id = w2_Coupon.dept_id").Append("\n");
				result.Append("					AND").Append("\n");
				result.Append("					w2_UserCoupon.coupon_id = w2_Coupon.coupon_id").Append("\n");
				result.Append("				 )").Append("\n");
				// 条件指定でテーブル名：w2_UserCoupon、w2_Couponを使用できるようにするため、有効期限は別で計算したものを内部結合
				result.Append("				INNER JOIN (").Append("\n");
				result.Append("					SELECT w2_UserCoupon.user_id,").Append("\n");
				result.Append("						w2_UserCoupon.dept_id,").Append("\n");
				result.Append("						w2_UserCoupon.coupon_id,").Append("\n");
				result.Append("						w2_UserCoupon.coupon_no,").Append("\n");
				result.Append("						ISNULL(").Append("\n");
				result.Append("							w2_Coupon.expire_date_end,").Append("\n");
				result.Append("							DATEADD(DAY, w2_Coupon.expire_day, w2_UserCoupon.date_created)").Append("\n");
				result.Append("						) AS coupon_expire_date").Append("\n");
				result.Append("					  FROM w2_UserCoupon").Append("\n");
				result.Append("					   INNER JOIN w2_Coupon ON").Append("\n");
				result.Append("						(").Append("\n");
				result.Append("							w2_UserCoupon.dept_id = w2_Coupon.dept_id").Append("\n");
				result.Append("							AND").Append("\n");
				result.Append("							w2_UserCoupon.coupon_id = w2_Coupon.coupon_id").Append("\n");
				result.Append("						)").Append("\n");
				result.Append("					) AS tempUserCouponExpireDate ON").Append("\n");
				result.Append("					 (").Append("\n");
				result.Append("						w2_UserCoupon.user_id = tempUserCouponExpireDate.user_id").Append("\n");
				result.Append("						AND").Append("\n");
				result.Append("						w2_UserCoupon.dept_id = tempUserCouponExpireDate.dept_id").Append("\n");
				result.Append("						AND").Append("\n");
				result.Append("						w2_UserCoupon.coupon_id = tempUserCouponExpireDate.coupon_id").Append("\n");
				result.Append("						AND").Append("\n");
				result.Append("						w2_UserCoupon.coupon_no = tempUserCouponExpireDate.coupon_no").Append("\n");
				result.Append("					)").Append("\n");
				result.Append("				 WHERE").Append("\n");
				result.Append(
					string.Join("			" + logicalOperation + "\n", targetConditionList.Select(c => CreateManualSqlWhere(c)).ToArray()));
				result.Append("					   ) AS couponTemp").Append("\n");
				result.Append("					ON (").Append("\n");
				result.Append("						w2_User.user_id = couponTemp.user_id").Append("\n");
				result.Append("						)").Append("\n");
			}

			return result.ToString();
		}

		/// <summary>
		/// メールクリック結果抽出条件SQL作成
		/// </summary>
		/// <param name="conditionList">条件リスト</param>
		/// <param name="logicalOperation">AND、OR条件</param>
		/// <returns>抽出条件SQL</returns>
		private string CreateManualSqlWhereForMailClick(List<TargetListCondition> conditionList, string logicalOperation)
		{
			// 条件作成：メールクリック結果
			var targetConditionList = conditionList
				.Where(c => (c.DataKbn == TargetListCondition.DATAKBN_MAIL_CLICK_INFO)).ToArray();
			var targetConditionWhereString = string.Join(
				"			" + logicalOperation + "\n",
				targetConditionList.Select(CreateManualSqlWhere).ToArray());

			// 条件あり?
			if (targetConditionList.Any())
			{
				var result = "			INNER JOIN (\n" + "				SELECT DISTINCT w2_MailClickLog.user_id\n"
					+ "				  FROM w2_MailClickLog\n" + "				   INNER JOIN w2_MailClick ON\n" + "				   (\n"
					+ "						w2_MailClickLog.dept_id = w2_MailClick.dept_id\n"
					+ "						AND w2_MailClickLog.mailtext_id = w2_MailClick.mailtext_id\n"
					+ "						AND w2_MailClickLog.maildist_id = w2_MailClick.maildist_id\n"
					+ "						AND w2_MailClickLog.action_no = w2_MailClick.action_no\n"
					+ "						AND w2_MailClickLog.mailclick_id = w2_MailClick.mailclick_id\n" + "				   )\n"
					+ "				 WHERE\n" + "					" + targetConditionWhereString + "					   ) AS mailClickTemp\n"
					+ "					ON (\n" + "						w2_User.user_id = mailClickTemp.user_id\n" + "					     )\n";
				return result;
			}

			return "";
		}

		/// <summary>
		/// 合算ユーザーポイント抽出条件SQL作成
		/// </summary>
		/// <param name="conditionList">条件リスト</param>
		/// <param name="logicalOperation">AND、OR条件</param>
		/// <returns>抽出条件SQL</returns>
		private string CreateManualSqlWhereForAggregatedUserPoint(List<TargetListCondition> conditionList, string logicalOperation)
		{
			// デバッグ時にSQL文のインデントがぶっ壊れていないようにしたい
			const string tabs = "\t\t\t\t";
			var targetConditionList = conditionList.Where(c => (c.DataKbn == TargetListCondition.DATAKBN_AGGREGATED_POINT_INFO)).ToArray();
			var targetConditionWhereString = string.Join((logicalOperation + "\n"), targetConditionList.Select(CreateManualSqlWhere).ToArray());

			if (targetConditionList.Any() == false) return string.Empty;

			var result
				= tabs + "  INNER JOIN ( \n"
				+ tabs + "    SELECT  DISTINCT AggregatedUserPoint.user_id \n"
				+ tabs + "      FROM  ( \n"
				+ tabs + "                SELECT  w2_UserPoint.user_Id AS user_id, \n"
				+ tabs + "                        SUM(w2_UserPoint.point) AS point \n"
				+ tabs + "                  FROM  w2_UserPoint \n"
				+ tabs + "                 WHERE  w2_UserPoint.point_type = '01' \n"
				+ tabs + "                   AND  w2_UserPoint.point_kbn IN ('01', '02') \n"
				+ tabs + "                 GROUP  BY w2_UserPoint.user_id"
				+ tabs + "            ) AS AggregatedUserPoint \n"
				+ tabs + "     WHERE  "
				+ tabs + targetConditionWhereString
				+ tabs + "  ) AS aggregatedPointTemp \n"
				+ tabs + "  ON \n"
				+ tabs + "  ( \n"
				+ tabs + "    w2_User.user_id = aggregatedPointTemp.user_id \n"
				+ tabs + "  ) \n";
			return result;
		}

		/// <summary>
		/// 通常ユーザーポイント抽出条件SQL作成
		/// </summary>
		/// <param name="conditionList">条件リスト</param>
		/// <param name="logicalOperation">AND、OR条件</param>
		/// <returns>抽出条件SQL</returns>
		private string CreateManualSqlWhereForUserBasePoint(List<TargetListCondition> conditionList, string logicalOperation)
		{
			// デバッグ時にSQL文のインデントがぶっ壊れていないようにしたい
			const string tabs = "\t\t\t\t";
			var targetConditionList = conditionList.Where(c => (c.DataKbn == TargetListCondition.DATAKBN_POINT_INFO)).ToArray();
			var targetConditionWhereString = string.Join((logicalOperation + "\n"), targetConditionList.Select(CreateManualSqlWhere).ToArray());

			if (targetConditionList.Any() == false) return string.Empty;

			var result
				= tabs + "  INNER JOIN ( \n"
				+ tabs + "    SELECT  w2_UserPoint.user_id, \n"
				+ tabs + "            w2_UserPoint.point_exp \n"
				+ tabs + "      FROM  w2_UserPoint"
				+ tabs + "     WHERE  point_kbn = '01' "
				+ tabs + "       AND  point_type = '01' "
				+ tabs + "       AND  " + targetConditionWhereString
				+ tabs + "  ) AS w2_UserPoint \n"
				+ tabs + "  ON \n"
				+ tabs + "  ( \n"
				+ tabs + "    w2_User.user_id = w2_UserPoint.user_id \n"
				+ tabs + "  ) \n";
			return result;
		}

		/// <summary>
		/// 期間限定ポイント抽出条件SQL作成
		/// </summary>
		/// <param name="conditionList">条件リスト</param>
		/// <param name="logicalOperation">AND、OR条件</param>
		/// <param name="createJoin">JOIN句作成（「・・がある」条件はJOIN結合するが、「・・がない」条件は別SQLにする</param>
		/// <returns>抽出条件SQL</returns>
		private string CreateManualSqlWhereForLimitedTermPoint(
			List<TargetListCondition> conditionList,
			string logicalOperation,
			bool createJoin)
		{
			// デバッグ時にSQL文のインデントがぶっ壊れていないようにしたい
			const string tabs = "\t\t\t\t";
			var targetConditionList = conditionList.Where(c => (c.DataKbn == TargetListCondition.DATAKBN_LIMITED_TERM_POINT_INFO)).ToArray();
			var targetConditionWhereString = string.Join((logicalOperation + "\n"), targetConditionList.Select(CreateManualSqlWhere).ToArray());

			if (targetConditionList.Any() == false) return string.Empty;

			var result = "";
			if (createJoin)
			{
				result += tabs + "  INNER JOIN ( \n";
			}
			result += tabs + "    SELECT  DISTINCT LimitedTermPoint.user_id \n";
			result += tabs + "      FROM  ( \n";
			result += tabs + "                SELECT  w2_UserPoint.user_Id AS user_id, \n";
			result += tabs + "                        w2_UserPoint.point AS point, \n";
			result += tabs + "                        w2_UserPoint.point_exp AS point_exp, \n";
			result += tabs + "                        w2_UserPoint.effective_date AS effective_date \n";
			result += tabs + "                  FROM  w2_UserPoint \n";
			result += tabs + "                 WHERE  w2_UserPoint.point_type = '01' \n";
			result += tabs + "                   AND  w2_UserPoint.point_kbn = '02' \n";
			result += tabs + "            ) AS LimitedTermPoint \n";
			result += tabs + "     WHERE  " + tabs + targetConditionWhereString;
			if (createJoin)
			{
				result += tabs + "  ) AS LimitedTermPointTemp \n";
				result += tabs + "  ON \n";
				result += tabs + "  ( \n";
				result += tabs + "    w2_User.user_id = LimitedTermPointTemp.user_id \n";
				result += tabs + "  ) \n";
			}
			return result;
		}

		/// <summary>
		/// 手動設定抽出条件WhereSQL作成
		/// </summary>
		/// <param name="targetListCondition">ターゲットリスト抽出条件</param>
		/// <returns>手動設定抽出条件WhereSQL</returns>
		private string CreateManualSqlWhere(TargetListCondition targetListCondition)
		{
			DateTime tmpDate; // switch内で宣言がかぶるのでここで宣言
			string tmpString; // switch内で宣言がかぶるのでここで宣言
			var fixedPurchaseSetting = string.Format("{0}.{1}", Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1);

			var result = new StringBuilder();
			result.Append("			(").Append("\n");
			switch (targetListCondition.EqualSign)
			{
				// 文字・等しい
				case TargetListCondition.EQUALSIGN_SELECT_EQUAL:
				case TargetListCondition.EQUALSIGN_STRING_EQUAL:
					foreach (TargetListCondition.Data dataValue in targetListCondition.Values)
					{
						if (targetListCondition.Values.IndexOf(dataValue) != 0)
						{
							result.Append("OR \n");
						}

						tmpString = CreateStringValue(dataValue.Value);

						// 頒布会コースID検索の場合は別でSQLを作成
						if (targetListCondition.IsDataFieldSubscriptionBoxCourseId())
						{
							var subscriptionBoxSql = CreateJoinSqlForSubscriptionBoxCourseId(
								tmpString,
								targetListCondition.EqualSign);
							result.Append(subscriptionBoxSql);
							continue;
						}

						var sql = targetListCondition.DataField == fixedPurchaseSetting
							? CreateJoinSqlForFixedPurchaseSetting(tmpString, targetListCondition, true)
							: new StringBuilder(targetListCondition.DataField).Append(" = '").Append(tmpString).Append("'\n");
						result.Append(sql);

						if (targetListCondition.DataField == "w2_UserCoupon.use_flg")
						{
							// 利用可能回数（user_coupon_count）は利用回数無制限の時などにnullになるので、0ではないものとして扱う
							result.Append((tmpString == "0")
								? "AND ISNULL(w2_UserCoupon.user_coupon_count, '1') <> 0"
								: "OR ISNULL(w2_UserCoupon.user_coupon_count, '1') = 0");
						}
					}

					break;

				// 文字・等しくない
				case TargetListCondition.EQUALSIGN_SELECT_NOT_EQUAL:
				case TargetListCondition.EQUALSIGN_STRING_NOT_EQUAL:
					foreach (TargetListCondition.Data dataValue in targetListCondition.Values)
					{
						if (targetListCondition.Values.IndexOf(dataValue) != 0)
						{
							result.Append("OR \n");
						}

						tmpString = CreateStringValue(dataValue.Value);

						// 頒布会コースID検索の場合は別でSQLを作成
						if (targetListCondition.IsDataFieldSubscriptionBoxCourseId())
						{
							var subscriptionBoxSql = CreateJoinSqlForSubscriptionBoxCourseId(
								tmpString,
								targetListCondition.EqualSign);
							result.Append(subscriptionBoxSql);
							continue;
						}

						var sql = targetListCondition.DataField == fixedPurchaseSetting
							? CreateJoinSqlForFixedPurchaseSetting(tmpString, targetListCondition, false)
							: new StringBuilder(targetListCondition.DataField).Append(" <> '").Append(tmpString).Append("'\n");
						result.Append(sql);

						if (targetListCondition.DataField == "w2_UserCoupon.use_flg")
						{
							// 利用可能回数（user_coupon_count）は利用回数無制限の時などにnullになるので、0ではないものとして扱う
							result.Append((tmpString == "0")
								? "OR ISNULL(w2_UserCoupon.user_coupon_count, '1') = 0"
								: "AND ISNULL(w2_UserCoupon.user_coupon_count, '1') <> 0");
						}
					}

					break;

				// 文字・選択している (カンマ区切りで値が入っているフィールドに対応)
				case TargetListCondition.EQUALSIGN_CHECK_EQUAL:
					foreach (TargetListCondition.Data dataValue in targetListCondition.Values)
					{
						if (targetListCondition.Values.IndexOf(dataValue) != 0)
						{
							result.Append("OR \n");
						}

						// 比較元・比較先のどちらも両端にカンマをつけて比較する。カンマ区切りのフィールド以外で利用してもOK
						result.Append("',' + ").Append(targetListCondition.DataField)
							.Append(" + ',' LIKE '%,").Append(dataValue.Value.Replace("'", "''")).Append(",%'\n");
					}

					break;

				// 文字・選択していない (カンマ区切りで値が入っているフィールドに対応)
				case TargetListCondition.EQUALSIGN_CHECK_NOT_EQUAL:
					foreach (TargetListCondition.Data dataValue in targetListCondition.Values)
					{
						if (targetListCondition.Values.IndexOf(dataValue) != 0)
						{
							result.Append("OR \n");
						}

						// 比較元・比較先のどちらも両端にカンマをつけて比較する。カンマ区切りのフィールド以外で利用してもOK
						result.Append("',' + ").Append(targetListCondition.DataField)
							.Append(" + ',' NOT LIKE '%,").Append(dataValue.Value.Replace("'", "''")).Append(",%'\n");
					}

					break;

				// 文字・含む
				case TargetListCondition.EQUALSIGN_STRING_CONTAIN:
					foreach (TargetListCondition.Data dataValue in targetListCondition.Values)
					{
						if (targetListCondition.Values.IndexOf(dataValue) != 0)
						{
							result.Append("OR \n");
						}

						tmpString = CreateStringValue(dataValue.Value);

						// 頒布会コースID検索の場合は別でSQLを作成
						if (targetListCondition.IsDataFieldSubscriptionBoxCourseId())
						{
							var subscriptionBoxSql = CreateJoinSqlForSubscriptionBoxCourseId(
								tmpString,
								targetListCondition.EqualSign);
							result.Append(subscriptionBoxSql);
							continue;
						}

						result.Append(targetListCondition.DataField)
							.Append(" LIKE '%").Append(tmpString).Append("%'\n");
					}

					break;

				// 文字・含まない
				case TargetListCondition.EQUALSIGN_STRING_NOT_CONTAIN:
					foreach (TargetListCondition.Data dataValue in targetListCondition.Values)
					{
						if (targetListCondition.Values.IndexOf(dataValue) != 0)
						{
							result.Append("OR \n");
						}

						tmpString = CreateStringValue(dataValue.Value);

						// 頒布会コースID検索の場合は別でSQLを作成
						if (targetListCondition.IsDataFieldSubscriptionBoxCourseId())
						{
							var subscriptionBoxSql = CreateJoinSqlForSubscriptionBoxCourseId(
								tmpString,
								targetListCondition.EqualSign);
							result.Append(subscriptionBoxSql);
							continue;
						}

						result.Append(targetListCondition.DataField)
							.Append(" NOT LIKE '%").Append(tmpString).Append("%'\n");
					}

					break;

				// 文字・～からはじまる
				case TargetListCondition.EQUALSIGN_STRING_BEGIN_WITH:
					foreach (TargetListCondition.Data dataValue in targetListCondition.Values)
					{
						if (targetListCondition.Values.IndexOf(dataValue) != 0)
						{
							result.Append("OR \n");
						}

						tmpString = CreateStringValue(dataValue.Value);

						// 頒布会コースID検索の場合は別でSQLを作成
						if (targetListCondition.IsDataFieldSubscriptionBoxCourseId())
						{
							var subscriptionBoxSql = CreateJoinSqlForSubscriptionBoxCourseId(
								tmpString,
								targetListCondition.EqualSign);
							result.Append(subscriptionBoxSql);
							continue;
						}

						result.Append(targetListCondition.DataField)
							.Append(" LIKE '").Append(tmpString).Append("%'\n");
					}

					break;

				// 文字・～でおわる
				case TargetListCondition.EQUALSIGN_STRING_END_WITH:
					foreach (TargetListCondition.Data dataValue in targetListCondition.Values)
					{
						if (targetListCondition.Values.IndexOf(dataValue) != 0)
						{
							result.Append("OR \n");
						}

						tmpString = CreateStringValue(dataValue.Value);

						// 頒布会コースID検索の場合は別でSQLを作成
						if (targetListCondition.IsDataFieldSubscriptionBoxCourseId())
						{
							var subscriptionBoxSql = CreateJoinSqlForSubscriptionBoxCourseId(
								tmpString,
								targetListCondition.EqualSign);
							result.Append(subscriptionBoxSql);
							continue;
						}

						result.Append(targetListCondition.DataField)
							.Append(" LIKE '%").Append(tmpString).Append("'\n");
					}

					break;

				// 数値・等しい
				case TargetListCondition.EQUALSIGN_NUMBER_EQUAL:
					foreach (TargetListCondition.Data dataValue in targetListCondition.Values)
					{
						if (targetListCondition.Values.IndexOf(dataValue) != 0)
						{
							result.Append("OR \n");
						}

						result.Append(targetListCondition.DataField).Append(" = ").Append(dataValue.Value).Append("\n");
					}

					break;

				// 数値・～より大きい
				case TargetListCondition.EQUALSIGN_NUMBER_BIGGER_THAN:
					result.Append(targetListCondition.DataField).Append(" > ")
						.Append(targetListCondition.Values[0].Value).Append("\n");
					break;

				// 数値・～より小さい
				case TargetListCondition.EQUALSIGN_NUMBER_SMALLER_THAN:
					result.Append(targetListCondition.DataField).Append(" < ")
						.Append(targetListCondition.Values[0].Value).Append("\n");
					break;

				// 日付・日にちが等しい
				case TargetListCondition.EQUALSIGN_DATE_EQUAL:
					foreach (TargetListCondition.Data dataValue in targetListCondition.Values)
					{
						if (targetListCondition.Values.IndexOf(dataValue) != 0)
						{
							result.Append("OR \n");
						}

						tmpDate = CreateDateTimeValue(dataValue.Value);
						result.Append(CreateEqualDate(targetListCondition.DataField, tmpDate));
					}

					break;

				// 日付・～より未来
				case TargetListCondition.EQUALSIGN_DATE_AFTER_THAN:
					tmpDate = CreateDateTimeValue(targetListCondition.Values[0].Value);
					result.Append(targetListCondition.DataField).Append(" > '")
						.Append(tmpDate.ToString()).Append("'\n");
					break;

				// 日付・～より過去
				case TargetListCondition.EQUALSIGN_DATE_BEFORE_THAN:
					tmpDate = CreateDateTimeValue(targetListCondition.Values[0].Value);
					result.Append(targetListCondition.DataField).Append(" < '")
						.Append(tmpDate.ToString()).Append("'\n");
					break;

				// 日付・～日前から過去
				case TargetListCondition.EQUALSIGN_DATE_MORE_THAN_DAY:
					tmpDate = DateTime.Now.Date.AddDays(-1 * int.Parse(targetListCondition.Values[0].Value));
					result.Append("'").Append(tmpDate.AddDays(1).ToString()).Append("' > ")
						.Append(targetListCondition.DataField).Append(" \n");
					break;

				// 日付・～日以内
				case TargetListCondition.EQUALSIGN_DATE_LESS_THAN_DAY:
					tmpDate = DateTime.Now.Date.AddDays(-1 * int.Parse(targetListCondition.Values[0].Value));
					result.Append("'").Append(tmpDate.ToString()).Append("' <= ")
						.Append(targetListCondition.DataField).Append(" \n");
					break;

				// 日付・～週間以内
				case TargetListCondition.EQUALSIGN_DATE_LESS_THAN_WEEK:
					tmpDate = DateTime.Now.Date.AddDays(-7 * int.Parse(targetListCondition.Values[0].Value));
					result.Append("'").Append(tmpDate.ToString()).Append("' <= ")
						.Append(targetListCondition.DataField).Append(" \n");
					break;

				// 日付・～ヶ月以内
				case TargetListCondition.EQUALSIGN_DATE_LESS_THAN_MONTH:
					tmpDate = DateTime.Now.Date.AddMonths(-1 * int.Parse(targetListCondition.Values[0].Value));
					result.Append("'").Append(tmpDate.ToString()).Append("' <= ")
						.Append(targetListCondition.DataField).Append(" \n");
					break;

				// 日付・～年以内
				case TargetListCondition.EQUALSIGN_DATE_LESS_THAN_YEAR:
					tmpDate = DateTime.Now.Date.AddYears(-1 * int.Parse(targetListCondition.Values[0].Value));
					result.Append("'").Append(tmpDate.ToString()).Append("' <= ")
						.Append(targetListCondition.DataField).Append(" \n");
					break;

				// 経過日数・～日と等しい
				case TargetListCondition.EQUALSIGN_DAYAFTER_EQUAL:
					tmpDate = DateTime.Now.Date.AddDays(-1 * int.Parse(targetListCondition.Values[0].Value));
					result.Append(CreateEqualDate(targetListCondition.DataField.Replace("_dayafter", ""), tmpDate));
					break;

				// 経過日数・～日を超える
				case TargetListCondition.EQUALSIGN_DAYAFTER_BIGGER_THAN:
					tmpDate = DateTime.Now.Date.AddDays(-1 * int.Parse(targetListCondition.Values[0].Value));
					result.Append(targetListCondition.DataField.Replace("_dayafter", "")).Append(" < ")
						.Append("'").Append(tmpDate.ToString()).Append("' \n");
					break;

				// 経過日数・～日未満
				case TargetListCondition.EQUALSIGN_DAYAFTER_SMALLER_THAN:
					tmpDate = DateTime.Now.Date.AddDays(-1 * int.Parse(targetListCondition.Values[0].Value) + 1);
					result.Append(targetListCondition.DataField.Replace("_dayafter", "")).Append(" >= ")
						.Append("'").Append(tmpDate.ToString()).Append("' \n");
					break;

				// 日付・～日前
				case TargetListCondition.EQUALSIGN_DATE_DAY_BEFORE:
					tmpDate = DateTime.Now.Date.AddDays(-1 * int.Parse(targetListCondition.Values[0].Value));
					result.Append(CreateEqualDate(targetListCondition.DataField, tmpDate));
					break;

				// 日付・～日後
				case TargetListCondition.EQUALSIGN_DATE_DAY_AFTER:
					tmpDate = DateTime.Now.Date.AddDays(1 * int.Parse(targetListCondition.Values[0].Value));
					result.Append(CreateEqualDate(targetListCondition.DataField, tmpDate));
					break;

				// 日付・～日後から未来
				case TargetListCondition.EQUALSIGN_DATE_DAY_BEFORE_FUTURE:
					tmpDate = DateTime.Now.Date.AddDays(1 * int.Parse(targetListCondition.Values[0].Value));
					result.Append("'").Append(tmpDate.ToString()).Append("' <= ")
						.Append(targetListCondition.DataField).Append(" \n");
					break;

				// 日付・～日後から過去
				case TargetListCondition.EQUALSIGN_DATE_DAY_AFTER_PAST:
					tmpDate = DateTime.Now.Date.AddDays(1 * int.Parse(targetListCondition.Values[0].Value));
					result.Append("'").Append(tmpDate.AddDays(1).ToString()).Append("' > ")
						.Append(targetListCondition.DataField).Append(" \n");
					break;

				// 日付・データなし(NULL)
				case TargetListCondition.EQUALSIGN_DATE_DAY_NULL:
					result.Append(targetListCondition.DataField + " IS NULL \n");
					break;
			}

			result.Append("			)").Append("\n");

			return result.ToString();
		}

		/// <summary>
		/// 文字列取得
		/// </summary>
		/// <param name="value">値</param>
		/// <returns>文字列</returns>
		private string CreateStringValue(string value)
		{
			string result = "";
			var tmpValue = value.ToLower();

			if (tmpValue.Contains("today"))
			{
				var tmpDate = DateTime.Now.Date;

				var reg1 = new Regex("\\(.*\\)");
				if (reg1.IsMatch(value))
				{
					// 抜き出した部分だけで日付取得
					tmpDate = CreateDateTimeValue(reg1.Match(value).Value.Replace("(", "").Replace(")", ""));

					// 抜き出した部分をtodayで置換
					tmpValue = reg1.Replace(tmpValue, "today");
				}

				var reg = new Regex("((today.)|(year|month|day)|([+-][0-9]{1,}))");
				var matches = reg.Matches(tmpValue);
				var ymd = matches[1].Value;
				int add = 0;
				if (matches.Count > 2)
				{
					int.TryParse(matches[2].Value, out add);
				}

				switch (ymd)
				{
					case "year":
						result = tmpDate.AddYears(add).Year.ToString();
						break;

					case "month":
						result = tmpDate.AddMonths(add).Month.ToString();
						break;

					case "day":
						result = tmpDate.AddDays(add).Day.ToString();
						break;
				}
			}
			else
			{
				result = value;
			}

			return result.Replace("'", "''");
		}

		/// <summary>
		/// 日付取得
		/// </summary>
		/// <param name="value">値</param>
		/// <returns>日付</returns>
		private DateTime CreateDateTimeValue(string value)
		{
			DateTime result;
			var tmpValue = value.ToLower();

			if (tmpValue.Contains("today"))
			{
				result = DateTime.Now.Date;

				var reg = new Regex("([+-][0-9]{1,}[ymd])");
				var matches = reg.Matches(tmpValue);
				foreach (Match match in matches)
				{
					var ymd = match.Value.Substring(match.Value.Length - 1);
					int add;
					int.TryParse(match.Value.Substring(0, match.Value.Length - 1), out add);
					switch (ymd)
					{
						case "y":
							result = result.AddYears(add);
							break;

						case "m":
							result = result.AddMonths(add);
							break;

						case "d":
							result = result.AddDays(add);
							break;
					}
				}
			}
			else
			{
				DateTime.TryParseExact(
					tmpValue,
					"yyyy/M/d",
					null,
					System.Globalization.DateTimeStyles.None,
					out result);
			}

			return result;
		}

		/// <summary>
		/// 日付条件取得
		/// </summary>
		/// <param name="field">項目</param>
		/// <param name="value">値</param>
		/// <returns>SQL条件</returns>
		private string CreateEqualDate(string field, DateTime value)
		{
			var result = new StringBuilder();

			// 時間が入っていることを考慮して「field >= value AND field < value+1日」の形にする。
			result.Append("(");
			result.Append(field).Append(" >= '").Append(value.ToString()).Append("'");
			result.Append(" AND ");
			result.Append(field).Append(" < '").Append(value.AddDays(1).ToString()).Append("'");
			result.Append(")");

			return result.ToString();
		}

		/// <summary>
		/// Create the not null statement where
		/// </summary>
		/// <param name="dataKbn">The data KBN</param>
		/// <returns>The not null statement string</returns>
		private string CreateNotNullStatementlWhere(string dataKbn)
		{
			var notNullStatement = string.Empty;
			switch (dataKbn)
			{
				case TargetListCondition.DATAKBN_USER_INFO:
					notNullStatement = string.Format("{0}.{1}", Constants.TABLE_USER, Constants.FIELD_USER_USER_ID);
					break;

				case TargetListCondition.DATAKBN_POINT_INFO:
					notNullStatement = string.Format(
						"{0}.{1}",
						Constants.TABLE_USERPOINT,
						Constants.FIELD_USERPOINT_USER_ID);
					break;

				case TargetListCondition.DATAKBN_ORDER_INFO:
					notNullStatement = string.Format("{0}.{1}", Constants.TABLE_ORDER, Constants.FIELD_ORDER_USER_ID);
					break;

				case TargetListCondition.DATAKBN_FAVORITE_PRODUCT_INFO:
					notNullStatement = string.Format("{0}.{1}", Constants.TABLE_FAVORITE, Constants.FIELD_FAVORITE_PRODUCT_ID);
					break;
			}

			return ((string.IsNullOrEmpty(notNullStatement) == false)
				? string.Format(" AND {0} IS NOT NULL \n", notNullStatement)
				: string.Empty);
		}

		/// <summary>
		/// 抽出条件WhereSQL作成(受注、定期用)
		/// </summary>
		/// <param name="targetListCondition">ターゲットリスト抽出条件</param>
		/// <returns>抽出条件WhereSQL</returns>
		private string CreateSqlWhereForOrderAndFixedPurchase(TargetListCondition targetListCondition)
		{
			return targetListCondition.DataField.Contains("category_id")
				? CreateCategoryManualSqlWhere(targetListCondition)
				: CreateManualSqlWhere(targetListCondition);
		}

		/// <summary>
		/// カテゴリ抽出条件SQL作成
		/// </summary>
		/// <param name="condition">ターゲットリスト抽出条件</param>
		/// <returns>SQL</returns>
		private string CreateCategoryManualSqlWhere(TargetListCondition condition)
		{
			var sql = CreateManualSqlWhere(condition);
			var sqlList = new List<string>();
			var count = 0;
			while (count < 5)
			{
				count++;
				sqlList.Add(sql.Replace("category_id", ("category_id" + count)));
			}

			var logicalOperation = ((condition.EqualSign == TargetListCondition.EQUALSIGN_STRING_NOT_EQUAL)
				|| (condition.EqualSign == TargetListCondition.EQUALSIGN_STRING_NOT_CONTAIN))
					? " AND "
					: " OR ";
			return "(" + string.Join(logicalOperation, sqlList) + ")";
		}

		/// <summary>
		/// 購入回数と注文商品合計金額（全体注文）用のSQL作成
		/// </summary>
		/// <param name="orderCountCondition">購入回数条件リスト</param>
		/// <param name="orderPriceSumCondition">金額合計条件リスト</param>
		/// <param name="summaryColumn">集計用カラム</param>
		/// <returns>抽出条件SQL</returns>
		private string CreateSqlForOrderCountAndProductSum(
			string summaryColumn,
			List<TargetListCondition> orderCountCondition,
			List<TargetListCondition> orderPriceSumCondition)
		{
			if ((orderCountCondition.Count == 0) && (orderPriceSumCondition.Count == 0))
				return string.Empty;

			// 購入回数
			var sqlListForCount = new List<string>();
			orderCountCondition.ForEach(c =>
			{
				c.DataField = "COUNT(DISTINCT(OrderItemWithoutExceptOrder.order_id))";
				sqlListForCount.Add(CreateManualSqlWhere(c));
			});

			// 注文商品合計金額（全体注文）
			var sqlListForPriceSum = new List<string>();
			orderPriceSumCondition.ForEach(c =>
			{
				c.DataField = "SUM(OrderItemForPriceSum.item_price_sum)";
				sqlListForPriceSum.Add(CreateManualSqlWhere(c));
			});

			sqlListForCount.AddRange(sqlListForPriceSum);

			var sql = new StringBuilder();
			sql.Append("GROUP BY " + summaryColumn + "	\n")
				.Append("HAVING " + string.Join("AND	\n", sqlListForCount) + "	\n");
			return sql.ToString();
		}

		/// <summary>
		/// 対象外注文を除外するJoin文作成
		/// キャンセル、仮注文、仮注文キャンセル、返品交換注文を対象外にする
		/// 受注情報（集計）用のSQL文
		/// </summary>
		/// <returns>SQL文</returns>
		private StringBuilder CreateJoinSqlExceptOrder()
		{
			var sql = new StringBuilder();
			if ((this.OrderCountCondition.Count == 0) && (this.OrderPriceSumCondition.Count == 0))
				return sql;

			sql.Append("LEFT JOIN (	\n")
				.Append("  SELECT  OrderItem.order_id,	\n")
				.Append("          OrderItem.product_id,	\n")
				.Append("          OrderItem.variation_id,	\n")
				.Append("          OrderItem.item_quantity_sum + ISNULL(OrderItemReturn.item_quantity_sum, 0) AS ordered_quantity	\n")
				.Append("    FROM	\n")
				.Append("    (	\n")
				.Append("      SELECT  w2_OrderItem.order_id,	\n")
				.Append("              w2_OrderItem.product_id,	\n")
				.Append("              w2_OrderItem.variation_id,	\n")
				.Append("              SUM(w2_OrderItem.item_quantity) AS item_quantity_sum	\n")
				.Append("        FROM  w2_OrderItem    \n")
				.Append("        INNER JOIN w2_Order ON w2_Order.order_id = w2_OrderItem.order_id	\n")
				.Append("       WHERE  w2_Order.order_status NOT IN ('TMP', 'ODR_CNSL', 'TMP_CNSL')	\n")
				.Append("      GROUP BY w2_OrderItem.order_id, w2_OrderItem.product_id, w2_OrderItem.variation_id	\n")
				.Append("    ) AS OrderItem	\n")
				.Append("    LEFT JOIN (	\n")
				.Append("      SELECT  w2_Order.order_id_org,	\n")
				.Append("              w2_OrderItem.product_id,	\n")
				.Append("              w2_OrderItem.variation_id,	\n")
				.Append("              SUM(w2_OrderItem.item_quantity) AS item_quantity_sum	\n")
				.Append("        FROM  w2_OrderItem	\n")
				.Append("       INNER JOIN w2_Order ON	\n")
				.Append("       (	\n")
				.Append("         w2_Order.order_id = w2_OrderItem.order_id	\n")
				.Append("       )	\n")
				.Append("       WHERE  w2_Order.order_id_org <> ''	\n")
				.Append("      GROUP BY w2_Order.order_id_org, w2_OrderItem.product_id, w2_OrderItem.variation_id	\n")
				.Append("    ) AS OrderItemReturn ON	\n")
				.Append("    (	\n")
				.Append("      OrderItem.order_id = OrderItemReturn.order_id_org	\n")
				.Append("      AND	\n")
				.Append("      OrderItem.product_id = OrderItemReturn.product_id	\n")
				.Append("      AND	\n")
				.Append("      OrderItem.variation_id = OrderItemReturn.variation_id	\n")
				.Append("    )	\n")
				.Append(") AS OrderItemWithoutExceptOrder ON	\n")
				.Append("(	\n")
				.Append("  OrderItemWithoutExceptOrder.order_id = w2_Order.order_id	\n")
				.Append("  AND	\n")
				.Append("  OrderItemWithoutExceptOrder.product_id = w2_OrderItem.product_id	\n")
				.Append("  AND	\n")
				.Append("  OrderItemWithoutExceptOrder.variation_id = w2_OrderItem.variation_id	\n")
				.Append("  AND	\n")
				.Append("  OrderItemWithoutExceptOrder.ordered_quantity > 0	\n")
				.Append(")	\n");
			return sql;
		}

		/// <summary>
		/// 注文商品合計金額（全体注文）用のJoin文作成
		/// </summary>
		/// <returns>SQL文</returns>
		private StringBuilder CreateJoinSqlForProductSum()
		{
			var sql = new StringBuilder();
			if (this.OrderPriceSumCondition.Count == 0) return sql;

			sql.Append("LEFT JOIN (	\n")
				.Append("  SELECT  w2_OrderItem.order_id,	\n")
				.Append("          w2_OrderItem.product_id,	\n")
				.Append("          w2_OrderItem.variation_id,	\n")
				.Append("          SUM(w2_OrderItem.item_price) AS item_price_sum	\n")
				.Append("    FROM  w2_OrderItem	\n")
				.Append("  GROUP BY w2_OrderItem.order_id,w2_OrderItem.product_id,w2_OrderItem.variation_id	\n")
				.Append(") AS OrderItemForPriceSum ON	\n")
				.Append("(	\n")
				.Append("  OrderItemForPriceSum.order_id = w2_Order.order_id	\n")
				.Append("  AND	\n")
				.Append("  OrderItemForPriceSum.product_id = OrderItemWithoutExceptOrder.product_id	\n")
				.Append("  AND	\n")
				.Append("  OrderItemForPriceSum.variation_id = OrderItemWithoutExceptOrder.variation_id	\n")
				.Append(")	\n");
			return sql;
		}

		/// <summary>
		/// 定期配送パターンSQL作成
		/// </summary>
		/// <param name="fixedPurchasePattern">定期配送パターンカンマ区切り文字列</param>
		/// <param name="targetListCondition">ターゲットリスト抽出条件</param>
		/// <param name="equalFlag">等号・不等号判定</param>
		/// <returns>SQL文字列</returns>
		protected StringBuilder CreateJoinSqlForFixedPurchaseSetting(string fixedPurchasePattern, TargetListCondition targetListCondition, bool equalFlag)
		{
			var sql = new StringBuilder();
			var calculationSymbol = (equalFlag) ? " LIKE '" : " NOT LIKE '";
			var equalSign = (equalFlag) ? " = '" : " <> '";

			if (fixedPurchasePattern.EndsWith(",,"))
			{
				sql.Append(targetListCondition.DataField).Append(calculationSymbol)
					.Append((fixedPurchasePattern).TrimEnd(',')).Append("%").Append("'\n");
			}
			else if (fixedPurchasePattern.EndsWith(","))
			{
				sql.Append(targetListCondition.DataField).Append(calculationSymbol)
					.Append(fixedPurchasePattern).Append("%").Append("'\n");
			}
			else if (fixedPurchasePattern.StartsWith(","))
			{
				sql.Append(targetListCondition.DataField).Append(calculationSymbol)
					.Append("%").Append(fixedPurchasePattern).Append("'\n");
			}
			else
			{
				sql.Append(targetListCondition.DataField).Append(equalSign)
					.Append(fixedPurchasePattern).Append("'\n");
			}

			if (targetListCondition.FixedPurchaseKbn.Length > 0)
			{
				sql.Append("AND \n").Append(Constants.TABLE_FIXEDPURCHASE).Append(".").Append(Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN).Append(equalSign)
					.Append(targetListCondition.FixedPurchaseKbn).Append("'\n");
			}

			return sql;
		}

		/// <summary>
		/// 頒布会コースID検索用SQL作成
		/// </summary>
		/// <param name="searchValue">検索値</param>
		/// <param name="equalSignType">等号タイプ</param>
		/// <remarks>同梱注文の場合、w2_OrderItem.subscription_box_course_id の値を正として扱うためそちらを見る</remarks>
		/// <returns>頒布会コースID検索用SQL</returns>
		private StringBuilder CreateJoinSqlForSubscriptionBoxCourseId(string searchValue, string equalSignType)
		{
			var sqlWithEqualSign = string.Empty;
			// LIKE検索か
			var isLikeSearch = true;
			// 否定式か
			var isNegativeSign = false;
			switch (equalSignType)
			{
				case TargetListCondition.EQUALSIGN_STRING_EQUAL:
					sqlWithEqualSign = " = '{0}'";
					isLikeSearch = false;
					break;

				case TargetListCondition.EQUALSIGN_STRING_NOT_EQUAL:
					sqlWithEqualSign = " = '{0}'";
					isLikeSearch = false;
					isNegativeSign = true;
					break;

				case TargetListCondition.EQUALSIGN_STRING_CONTAIN:
					sqlWithEqualSign = " LIKE '%{0}%' ESCAPE '#'";
					break;

				case TargetListCondition.EQUALSIGN_STRING_NOT_CONTAIN:
					sqlWithEqualSign = " LIKE '%{0}%' ESCAPE '#'";
					isNegativeSign = true;
					break;

				case TargetListCondition.EQUALSIGN_STRING_BEGIN_WITH:
					sqlWithEqualSign = " LIKE '{0}%' ESCAPE '#'";
					break;

				case TargetListCondition.EQUALSIGN_STRING_END_WITH:
					sqlWithEqualSign = " LIKE '%{0}' ESCAPE '#'";
					break;

				default:
					throw new ArgumentOutOfRangeException(
						nameof(equalSignType),
						equalSignType,
						"文字列の等式タイプである必要があります。");
			}

			var escapedSearchValue = isLikeSearch
				? StringUtility.SqlLikeStringSharpEscape(searchValue).Replace("'", "''")
				: StringUtility.EscapeSqlString(searchValue);

			var sql = new StringBuilder();
			sql.AppendLine($" {(isNegativeSign ? "NOT" : string.Empty)} EXISTS (");
			sql.AppendLine("   SELECT  w2_Order.order_id");
			sql.AppendLine("     FROM  w2_OrderItem");
			sql.AppendLine("    WHERE  w2_OrderItem.order_id = w2_Order.order_id");
			sql.AppendLine("      AND  (");
			sql.AppendLine("             CASE w2_Order.combined_org_order_ids WHEN '' THEN w2_Order.subscription_box_course_id");
			sql.AppendLine("             ELSE w2_OrderItem.subscription_box_course_id");
			sql.AppendLine("             END");
			sql.AppendLine("           )");
			sql.AppendLine($"          {string.Format(sqlWithEqualSign, escapedSearchValue)}");
			sql.AppendLine(" )");

			return sql;
		}

		/// <summary>注文条件リスト</summary>
		private List<TargetListCondition> OrderExtractCondition { set; get; }
		/// <summary>注文除外条件リスト</summary>
		private List<TargetListCondition> OrderExceptCondition { set; get; }
		/// <summary>カート条件リスト</summary>
		private List<TargetListCondition> CartCondition { set; get; }
		/// <summary>定期注文条件リスト</summary>
		private List<TargetListCondition> FixedPurchaseCondition { set; get; }
		/// <summary>クーポン条件リスト</summary>
		private List<TargetListCondition> CouponCondition { set; get; }
		/// <summary>メールクリック条件リスト</summary>
		private List<TargetListCondition> MailClickCondition { set; get; }
		/// <summary>通常ポイント条件リスト</summary>
		private List<TargetListCondition> BasePointCondition { set; get; }
		/// <summary>期間限定ポイント条件リスト</summary>
		private List<TargetListCondition> LimitedTermPointExtractCondition { set; get; }
		/// <summary>期間限定ポイント除外リスト</summary>
		private List<TargetListCondition> LimitedTermPointExceptCondition { set; get; }
		/// <summary>DM発送履歴条件リスト</summary>
		private List<TargetListCondition> DmShippingHistoryExtractCondition { set; get; }
		/// <summary>DM発送履歴除外リスト</summary>
		private List<TargetListCondition> DmShippingHistoryExceptCondition { set; get; }
		/// <summary>合算ポイント条件リスト</summary>
		private List<TargetListCondition> AggregatedUserPointCondition { set; get; }
		/// <summary>定期注文含む条件リスト</summary>
		private List<TargetListCondition> FixedPurchaseOrderExtractCondition { set; get; }
		/// <summary>定期注文除外条件リスト</summary>
		private List<TargetListCondition> FixedPurchaseOrderExceptCondition { set; get; }
		/// <summary>直SQLリスト</summary>
		private List<TargetListCondition> SqlCondition { set; get; }
		/// <summary>購入回数条件リスト</summary>
		private List<TargetListCondition> OrderCountCondition { set; get; }
		/// <summary>金額合計条件リスト</summary>
		private List<TargetListCondition> OrderPriceSumCondition { set; get; }
	}
}
