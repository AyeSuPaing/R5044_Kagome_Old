/*
=========================================================================================================
  Module      : CrossPointオプション共通処理クラス(CrossPointUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using w2.App.Common.CrossPoint.Point;
using w2.App.Common.CrossPoint.PointHistory;
using w2.App.Common.Order;
using w2.App.Common.Web;
using w2.Common;
using w2.Common.Extensions;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.Point;
using w2.Domain.User;

namespace w2.App.Common.Option.CrossPoint
{
	/// <summary>
	/// CrossPointオプションユーティリティ
	/// </summary>
	public class CrossPointUtility
	{
		/// <summary>要素</summary>
		private const string FIELD_VALUE = "Value";
		/// <summary>属性値：ポイント加算区分</summary>
		private const string ATTRIBUTE_POINT_INC_KBN = "PointIncKbn";
		/// <summary>属性値：理由ID</summary>
		private const string ATTRIBUTE_VALUE = "Value";
		/// <summary>テーブル名</summary>
		private const string TABLE_NAME = "CrossPointOptionSetting";
		/// <summary>設定ファイルディレクトリ</summary>
		private const string DIRNAME_PARAMETER_SETTING = @"Settings\";
		/// <summary>設定ファイル名</summary>
		private const string FILENAME_BLOWFISH_SETTING = "CrossPointOptionSetting.xml";
		/// <summary>設定ファイルパス</summary>
		private static readonly string s_xmlFilePath = Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE + DIRNAME_PARAMETER_SETTING + FILENAME_BLOWFISH_SETTING;
		/// <summary>CrossPointオプション設定XML</summary>
		private static XmlDocument s_crossPointSettingCache;
		/// <summary>スレッドセーフ保つためのロックオブジェクト</summary>
		private static readonly object _lockObject = new object();
		/// <summary>定期購入判定（クロスポイント用）</summary>
		private static bool s_isFixedPurchaseOrderForCrossPoint;

		/// <summary>
		/// スタティックコンストラクタ
		/// </summary>
		static CrossPointUtility()
		{
			// 新着情報更新処理セット
			FileUpdateObserver.GetInstance().AddObservation(
				(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE + DIRNAME_PARAMETER_SETTING),
				FILENAME_BLOWFISH_SETTING,
				UpdateXmlCacheData);
		}

		/// <summary>
		/// CrossPointオプション設定XMLを読み込む
		/// </summary>
		/// <returns>CrossPointオプション設定XML</returns>
		private static XmlDocument LoadCrossPointSetting()
		{
			var docment = XDocument.Load(s_xmlFilePath).ToXmlDocument();
			return docment;
		}

		/// <summary>
		/// XMLキャッシュデータ更新
		/// </summary>
		private static void UpdateXmlCacheData()
		{
			lock (_lockObject)
			{
				s_crossPointSettingCache = LoadCrossPointSetting();
			}
		}

		/// <summary>
		/// フィールド値の表示文字列取得
		/// </summary>
		/// <param name="fieldName">フィールド名</param>
		/// <param name="value">値</param>
		/// <returns>値の名称</returns>
		public static string GetValue(string fieldName, object value)
		{
			return GetValue(fieldName, StringUtility.ToEmpty(value));
		}

		/// <summary>
		/// フィールド値の表示文字列取得
		/// </summary>
		/// <param name="fieldName">フィールド名</param>
		/// <param name="attributeKey">要素キー</param>
		/// <returns>値の名称</returns>
		private static string GetValue(string fieldName, string attributeKey)
		{
			var kvps = GetValueKvpArray(fieldName);
			foreach (var kvp in kvps)
			{
				if (kvp.Key == attributeKey) return kvp.Value;
			}
			return string.Empty;
		}

		/// <summary>
		/// フィールド値の表示文字列KeyValuePair配列取得
		/// </summary>
		/// <param name="fieldName">フィールド名</param>
		/// <returns>KeyValuePair配列</returns>
		private static KeyValuePair<string, string>[] GetValueKvpArray(string fieldName)
		{
			try
			{
				var array = CrossPointSettingData.SelectSingleNode("CrossPointOptionSetting/" + fieldName).ChildNodes.Cast<XmlNode>()
					.Where(fv => fv.Name == FIELD_VALUE).Select(
						fv => new KeyValuePair<string, string>(
							fv.Attributes[ATTRIBUTE_POINT_INC_KBN].Value,
							fv.Attributes[ATTRIBUTE_VALUE].Value)).ToArray();
				return array;
			}
			catch (Exception ex)
			{
				throw new w2Exception("CrossPointOptionSetting:" + fieldName + "で例外が発生しました。", ex);
			}
		}

		/// <summary>クロスポイント設定データ</summary>
		private static XmlDocument CrossPointSettingData
		{
			get
			{
				if (s_crossPointSettingCache == null)
				{
					lock (_lockObject)
					{
						if (s_crossPointSettingCache == null)
						{
							UpdateXmlCacheData();
						}
					}
				}

				return s_crossPointSettingCache;
			}
		}

		/// <summary>
		/// Adjust point by Cross Point
		/// </summary>
		/// <param name="userId">User id</param>
		/// <param name="addPoint">Add point</param>
		/// <param name="addInvalidDate">Add invalid date</param>
		/// <param name="lastChanged">Last changed</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of cases affected</returns>
		public static int AdjustPointByCrossPoint(
			string userId,
			decimal addPoint,
			string addInvalidDate,
			string lastChanged,
			SqlAccessor accessor = null)
		{
			var model = new UserPointModel
			{
				UserId = userId,
				DeptId = Constants.W2MP_DEPT_ID,
				Point = addPoint,
				PointExp = string.IsNullOrEmpty(addInvalidDate)
					? (DateTime?)null
					// addInvalidDateには日付までの値しか存在しないので、時間の最大値をセットする。
					: DateTime.Parse($"{addInvalidDate} 23:59:59.997"),
				LastChanged = lastChanged,
			};
			var result = DomainFacade.Instance.PointService.AdjustPointByCrossPoint(model, accessor);
			return result;
		}

		/// <summary>
		/// 【※標準では使用しない】購入時発行ポイント取得
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>購入時発行ポイント</returns>
		public static PointApiResult GetOrderPointAdd(OrderModel order)
		{
			// ゲスト購入時スキップ
			if (string.IsNullOrEmpty(order.UserId)) return null;

			var discount = order.MemberRankDiscountPrice
				+ order.OrderCouponUse
				+ order.SetpromotionProductDiscountAmount
				+ order.FixedPurchaseDiscountPrice
				+ order.FixedPurchaseMemberDiscountAmount
				- order.OrderPriceRegulation;
			var getInput = new PointApiInput
			{
				MemberId = order.UserId,
				PriceTotalInTax = (TaxCalculationUtility.GetPriceTaxIncluded(order.OrderPriceSubtotal, order.OrderPriceSubtotalTax) - discount),
				PriceTotalNoTax = (TaxCalculationUtility.GetPriceTaxExcluded(order.OrderPriceSubtotal, order.OrderPriceSubtotalTax) - discount),
				UsePoint = order.OrderPointUse,
				Items = CartObject.GetOrderDetails(order),
				IsNoPointId = true,
			};

			var point = new CrossPointPointApiService().Get(getInput.GetParam(PointApiInput.RequestType.Get));
			return point;
		}
		/// <summary>
		/// 【※標準では使用しない】購入時発行ポイント取得
		/// </summary>
		/// <param name="cart">カート情報</param>
		/// <returns>購入時発行ポイント</returns>
		public static PointApiResult GetOrderPointAdd(CartObject cart)
		{
			// ゲスト購入時スキップ
			if (string.IsNullOrEmpty(cart.OrderUserId)) return null;

			var getInput = new PointApiInput
			{
				MemberId = cart.OrderUserId,
				PriceTotalInTax = (TaxCalculationUtility.GetPriceTaxIncluded(cart.PriceSubtotal, cart.PriceSubtotalTax) - cart.TotalPriceDiscount),
				PriceTotalNoTax = (TaxCalculationUtility.GetPriceTaxExcluded(cart.PriceSubtotal, cart.PriceSubtotalTax) - cart.TotalPriceDiscount),
				UsePoint = cart.UsePoint,
				Items = CartObject.GetOrderDetails(cart),
				IsNoPointId = true,
			};
			var point = new CrossPointPointApiService().Get(getInput.GetParam(PointApiInput.RequestType.Get));
			return point;
		}

		/// <summary>
		/// 【※標準では使用しない】購入時発行ポイント取得
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>購入時発行ポイント</returns>
		public static PointApiResult GetOrderPointAddForOrder(OrderModel order)
		{
			// ゲスト購入時スキップ
			if (string.IsNullOrEmpty(order.UserId)) return null;

			var discount = order.MemberRankDiscountPrice
				+ order.OrderCouponUse
				+ order.SetpromotionProductDiscountAmount
				+ order.FixedPurchaseDiscountPrice
				+ order.FixedPurchaseMemberDiscountAmount
				- order.OrderPriceRegulation;
			var getInput = new PointApiInput
			{
				MemberId = order.UserId,
				PriceTotalInTax = (TaxCalculationUtility.GetPriceTaxIncluded(order.OrderPriceSubtotal, order.OrderPriceSubtotalTax) - discount),
				PriceTotalNoTax = (TaxCalculationUtility.GetPriceTaxExcluded(order.OrderPriceSubtotal, order.OrderPriceSubtotalTax) - discount),
				UsePoint = order.OrderPointUse,
				Items = CartObject.GetOrderDetails(order),
			};

			var point = new CrossPointPointApiService().Get(getInput.GetParam(PointApiInput.RequestType.Get));
			return point;
		}
		/// <summary>
		/// 【※標準では使用しない】購入時発行ポイント取得
		/// </summary>
		/// <param name="cart">カート情報</param>
		/// <returns>購入時発行ポイント</returns>
		public static PointApiResult GetOrderPointAddForOrder(CartObject cart)
		{
			if (string.IsNullOrEmpty(cart.OrderUserId)) return null;

			var getInput = new PointApiInput
			{
				MemberId = cart.OrderUserId,
				PriceTotalInTax = (TaxCalculationUtility.GetPriceTaxIncluded(cart.PriceSubtotal, cart.PriceSubtotalTax)),
				PriceTotalNoTax = (TaxCalculationUtility.GetPriceTaxExcluded(cart.PriceSubtotal, cart.PriceSubtotalTax)),
				UsePoint = cart.UsePoint,
				Items = CartObject.GetOrderDetails(cart),
			};
			var point = new CrossPointPointApiService().Get(getInput.GetParam(PointApiInput.RequestType.Get));
			return point;
		}

		/// <summary>
		/// 割引額・各種手数料明細取得
		/// </summary>
		/// <param name="cart">カート情報</param>
		/// <returns>割引額・各種手数料明細</returns>
		public static OrderDetail[] GetSellingCosts(CartObject cart)
		{
			// 割引額合計を事前計算（マイナス値）
			var discount = 0m
				- cart.MemberRankDiscount
				- cart.UseCouponPrice
				- cart.UsePoint
				- cart.SetPromotions.ProductDiscountAmount
				- cart.FixedPurchaseDiscount
				- cart.FixedPurchaseMemberDiscountAmount
				- cart.SetPromotions.PaymentChargeDiscountAmount;

			var costs = new[]
			{
				new OrderDetail
				{
					JanCode = "9999",
					ProductName = "各種金額調整（値引き・調整金額）",
					ProductId = "9999",
					Price = discount,
					SalesPrice = discount,
					Quantity = 1,
					Tax = 0m,
					ItemSalesKbn = Constants.CROSS_POINT_FLG_ITEM_KBN_NOT_PRODUCT,
				},
				new OrderDetail
				{
					JanCode = "9999",
					ProductName = "送料",
					ProductId = "9999",
					Price = cart.ShippingPriceForCalculationDiscountAndTax,
					SalesPrice = cart.ShippingPriceForCalculationDiscountAndTax,
					Quantity = 1,
					Tax = 0m,
					ItemSalesKbn = Constants.CROSS_POINT_FLG_ITEM_KBN_NOT_PRODUCT,
				},
				new OrderDetail
				{
					JanCode = "9999",
					ProductName = "決済手数料",
					ProductId = "9999",
					Price = cart.PaymentPriceForCalculationDiscountAndTax,
					SalesPrice = cart.PaymentPriceForCalculationDiscountAndTax,
					Quantity = 1,
					Tax = 0m,
					ItemSalesKbn = Constants.CROSS_POINT_FLG_ITEM_KBN_NOT_PRODUCT,
				},
			};

			return costs.Where(detail => (detail.SalesPrice != 0m)).ToArray();
		}

		/// <summary>
		/// 割引額・各種手数料明細取得
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>割引額・各種手数料明細</returns>
		public static OrderDetail[] GetSellingCosts(OrderModel order)
		{
			// 割引額合計を事前計算（マイナス値）
			var discount = 0m
				- order.MemberRankDiscountPrice
				- order.OrderCouponUse
				- order.SetpromotionProductDiscountAmount
				- order.FixedPurchaseDiscountPrice
				- order.FixedPurchaseMemberDiscountAmount
				+ order.OrderPriceRegulation
				- order.OrderPointUse
				+ order.ReturnPriceCorrection;

			var costs = new[]
			{
				new OrderDetail
				{
					JanCode = "9999",
					ProductName = "各種金額調整（値引き・調整金額）",
					ProductId = "9999",
					Price = discount,
					SalesPrice = discount,
					Quantity = 1,
					Tax = 0m,
					ItemSalesKbn = Constants.CROSS_POINT_FLG_ITEM_KBN_NOT_PRODUCT,
				},
				new OrderDetail
				{
					JanCode = "9999",
					ProductName = "送料",
					ProductId = "9999",
					Price = order.OrderPriceShipping,
					SalesPrice = order.OrderPriceShipping,
					Quantity = 1,
					Tax = 0m,
					ItemSalesKbn = Constants.CROSS_POINT_FLG_ITEM_KBN_NOT_PRODUCT,
				},
				new OrderDetail
				{
					JanCode = "9999",
					ProductName = "決済手数料",
					ProductId = "9999",
					Price = order.OrderPriceExchange,
					SalesPrice = order.OrderPriceExchange,
					Quantity = 1,
					Tax = 0m,
					ItemSalesKbn = Constants.CROSS_POINT_FLG_ITEM_KBN_NOT_PRODUCT,
				},
			};

			return costs.Where(detail => (detail.SalesPrice != 0m)).ToArray();
		}

		/// <summary>
		/// CrossPoint 利用ポイント・仮付与ポイント情報を登録
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="error">エラーメッセージ</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>実行結果</returns>
		public static bool RegisterCrossPoint(
			Hashtable order,
			CartObject cart,
			out string error,
			SqlAccessor accessor = null)
		{
			// 利用ポイントが利用可能ポイントを上回っている場合エラー
			var userPoint = PointOptionUtility.GetUserPoint(
				cart.OrderUserId,
				Constants.FLG_USERPOINT_POINT_KBN_BASE,
				cart.CartId,
				accessor);

			if ((s_isFixedPurchaseOrderForCrossPoint == false)
				&& (userPoint.PointUsable < cart.UsePoint))
			{
				var usablePoint = StringUtility.ToNumeric(userPoint.PointUsable);
				error = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_POINT_USE_MAX_ERROR)
					.Replace("@@ 1 @@", usablePoint + Constants.CONST_UNIT_POINT_PT);
				return false;
			}

			// 利用ポイント・付与ポイント登録
			var registerInput = new PointApiInput
			{
				MemberId = cart.OrderUserId,
				OrderDate = (DateTime)order[Constants.FIELD_ORDER_ORDER_DATE],
				PosNo = Constants.CROSS_POINT_POS_NO,
				OrderId = (string)order[Constants.FIELD_ORDER_ORDER_ID],
				BaseGrantPoint = (order.ContainsKey(Constants.FIELD_ORDER_ORDER_POINT_ADD))
					? (decimal)order[Constants.FIELD_ORDER_ORDER_POINT_ADD]
					: cart.BuyPoint + cart.FirstBuyPoint,
				PriceTotalInTax = (TaxCalculationUtility.GetPriceTaxIncluded(cart.PriceSubtotal, cart.PriceSubtotalTax)),
				PriceTotalNoTax = (TaxCalculationUtility.GetPriceTaxExcluded(cart.PriceSubtotal, cart.PriceSubtotalTax)),
				UsePoint = cart.UsePoint,
				Items = CartObject.GetOrderDetails(cart),
			};
			var result = new CrossPointPointApiService().Register(registerInput.GetParam(PointApiInput.RequestType.Register));

			error = (result.IsSuccess == false)
				? MessageManager.GetMessages(Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR)
				: string.Empty;

			return result.IsSuccess;
		}

		/// <summary>
		/// CROSSPOINTのポイント更新（Front表示用エラーメッセージ取得）
		/// </summary>
		/// <param name="user">User model</param>
		/// <param name="point">Point</param>
		/// <param name="pointKbn">Point kbn</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Error messages</returns>
		public static string UpdateCrossPointApiWithWebErrorMessage(
			UserModel user,
			decimal point,
			string pointKbn,
			SqlAccessor accessor = null)
		{
			var errorMessages = UpdateCrossPointApi(user, point, pointKbn, accessor);
			if (string.IsNullOrEmpty(errorMessages) == false)
			{
				errorMessages = MessageManager.GetMessages(Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR);
			}

			return errorMessages;
		}

		/// <summary>
		/// CROSSPOINTのポイント更新API
		/// </summary>
		/// <param name="user">User model</param>
		/// <param name="point">Point</param>
		/// <param name="pointKbn">Point kbn</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>CROSSPOINT APIエラーメッセージ</returns>
		public static string UpdateCrossPointApi(
			UserModel user,
			decimal point,
			string pointKbn,
			SqlAccessor accessor = null)
		{
			var input = new Dictionary<string, string>
			{
				{ Constants.CROSS_POINT_PARAM_POINT_MEMBER_ID, user.UserId },
				{ Constants.CROSS_POINT_PARAM_POINT_POINT, point.ToString() },
				{ Constants.CROSS_POINT_PARAM_POINT_REASON, pointKbn },
			};
			var result = new CrossPointPointApiService().UpdatePoint(input);
			return (result.IsSuccess) ? string.Empty : result.ErrorMessage;
		}

		/// <summary>
		/// Get order point add by Cross Point
		/// </summary>
		/// <param name="cart">Cart object</param>
		/// <param name="pointIncKbn">Point inc kbn</param>
		/// <param name="fixedPurchase">Fixed purchase model</param>
		/// <returns>Order grant points</returns>
		public static decimal GetOrderPointAddByCrossPoint(
			CartObject cart,
			string pointIncKbn,
			FixedPurchaseModel fixedPurchase)
		{
			if (pointIncKbn != Constants.FLG_POINTRULE_POINT_INC_KBN_BUY) return 0m;

			var grantPoint = GetOrderPointAdd(cart);
			var addPoint = (grantPoint == null)
				? 0
				: (grantPoint.BaseGrantPoint + grantPoint.SpecialGrantPoint);

			return addPoint;
		}

		/// <summary>
		/// 購買ポイントキャンセル
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>購買ポイントが取り消しできたか</returns>
		public static bool DeletePurchasePoint(OrderModel order)
		{
			var registerInput = new PointApiInput
			{
				MemberId = order.UserId,
				OrderDate = order.OrderDate,
				PosNo = Constants.CROSS_POINT_POS_NO,
				OrderId = order.OrderId,
			};

			var result = new CrossPointPointApiService().Delete(registerInput.GetParam(PointApiInput.RequestType.Delete));
			return result.IsSuccess;
		}

		/// <summary>
		/// 実店舗側を含んだユーザーポイント履歴一覧取得
		/// </summary>
		/// /// <param name="userId">ユーザーID</param>
		/// <param name="realShopPointHistoryArray">ポイント履歴一覧</param>
		/// <returns>ユーザーポイント履歴一覧</returns>
		public static DataView GetUserPointHistoryList(string userId, PointHistoryItem[] realShopPointHistoryArray)
		{
			var dt = new DataTable();
			dt.Columns.Add(Constants.FIELD_USERPOINTHISTORY_USER_ID);
			dt.Columns.Add(Constants.FIELD_USERPOINTHISTORY_POINT_KBN);
			dt.Columns.Add(Constants.FIELD_USERPOINTHISTORY_POINT_TYPE);
			dt.Columns.Add(Constants.FIELD_USERPOINTHISTORY_POINT_INC_KBN);
			dt.Columns.Add(Constants.FIELD_USERPOINTHISTORY_RESTORED_FLG);
			dt.Columns.Add("point_date_created");
			dt.Columns.Add(Constants.FIELD_USERPOINT_POINT);
			dt.Columns.Add(Constants.FIELD_USERPOINTHISTORY_HISTORY_NO);
			dt.Columns.Add("return_order_count");
			dt.Columns.Add("exchange_order_count");
			dt.Columns.Add(Constants.CROSS_POINT_SETTING_SHOP_NAME);
			dt.Columns.Add("row_count");
			dt.Columns.Add("order_id");
			dt.Columns.Add("point_fixed_purchase_id");
			dt.Columns.Add(Constants.CROSS_POINT_SETTING_POINT_CHANGE_REASON);

			var pointHistoryList = DomainFacade.Instance.PointService.GetUserPointHistoriesOnFront(userId).ToArray();

			foreach (var item in pointHistoryList)
			{
				dt.Rows.Add(
					userId,
					item.PointKbn,
					item.PointType,
					item.PointIncKbn,
					item.RestoredFlg,
					item.PointCreateDate,
					item.Point,
					item.HistoryNo,
					item.ReturnOrderCount,
					item.ExchangeOrderCount,
					Constants.CROSS_POINT_EC_SHOP_NAME,
					pointHistoryList.Length + realShopPointHistoryArray.Length,
					item.PointOrderId,
					item.PointFixedPurchaseId,
					// ECは変動理由なし
					string.Empty
				);
			}

			var historyNo = dt.Rows.Count + 1;
			foreach (var item in realShopPointHistoryArray)
			{
				dt.Rows.Add(
					userId,
					(item.IsEffectivePoint)
						? Constants.FLG_USERPOINTHISTORY_POINT_TYPE_COMP
						: Constants.FLG_USERPOINTHISTORY_POINT_TYPE_TEMP,
					Constants.FLG_USERPOINTHISTORY_POINT_KBN_BASE,
					// CrossPointからはIDなし
					string.Empty,
					Constants.FLG_USERPOINTHISTORY_POINT_RESTORED_FLG_NOT_RESTORED,
					DateTime.Parse(item.PointGrantDate),
					item.Point,
					historyNo,
					0,
					0,
					item.ShopName,
					pointHistoryList.Length + realShopPointHistoryArray.Length,
					string.Empty,
					string.Empty,
					item.PointChangeReason
				);
				historyNo++;
			}
			return dt.AsDataView();
		}

		/// <summary>
		/// 定期購入判定セット（クロスポイント用）
		/// </summary>
		public static void SetFixedPurchaseOrderForCrossPoint()
		{
			s_isFixedPurchaseOrderForCrossPoint = true;
		}
	}
}
