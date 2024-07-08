/*
=========================================================================================================
  Module      : YAHOOモール注文 (YahooMallOrder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using w2.App.Common.Mall.Yahoo.Dto;
using w2.App.Common.Mall.Yahoo.Foundation;
using w2.Common.Helper;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Domain.User;

namespace w2.App.Common.Mall.Yahoo.YahooMallOrders
{
	/// <summary>
	/// YAHOOモール注文
	/// </summary>
	public class YahooMallOrder
	{
		/// <summary>DBのNTEXT型の最大サイズ</summary>
		private const int DB_NTEXT_TYPE_MAX_BYTES = 1073741823;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="statusCode">HTTPステータスコード</param>
		/// <param name="reasonPhrase">理由語句</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="dto">データトランスファーオブジェクト</param>
		/// <param name="mapper">Yahooモール注文決済マッピングクラス</param>
		/// <param name="xSwsAuthorizeStatusHeader">リクエストヘッダー "X-SWS-Authorize-Status" (公開鍵認証の結果)</param>
		public YahooMallOrder(
			HttpStatusCode statusCode,
			string reasonPhrase,
			string orderId,
			YahooApiOrderInfoResponseDto dto,
			YahooMallOrderPaymentMapper mapper,
			string xSwsAuthorizeStatusHeader)
		{
			if (dto == null)
			{
				throw new ArgumentException();
			}

			this.StatusCode = statusCode;
			this.ReasonPhrase = reasonPhrase;

			// 注文ID
			this.OrderId = orderId;

			// 注文日時
			var order = dto.Result.OrderInfo;
			if (DateTime.TryParse(order.OrderTime, out var orderTime) == false)
			{
				throw new ArgumentException("DateTime型にパースできませんでした。", nameof(order.OrderTime));
			}
			this.OrderDate = orderTime;

			// 注文ステータス
			if (EnumHelper.TryParseToEnum<OrderStatus>(order.OrderStatus, out var orderKbn) == false)
			{
				FileLogger.WriteWarn($"想定外の値です。OrderStatus={order.OrderStatus}");
			}
			switch (orderKbn)
			{
				case YahooMallOrders.OrderStatus.Reserved:
				case YahooMallOrders.OrderStatus.Processing:
				case YahooMallOrders.OrderStatus.Completed:
				default:
					this.OrderStatus = Constants.FLG_ORDER_ORDER_STATUS_ORDERED;
					break;

				case YahooMallOrders.OrderStatus.Suspended:
				case YahooMallOrders.OrderStatus.Canceled:
					throw new YahooMallOrderException($"注文ステータスがキャンセルか保留となっています。取り込みを行いません。OrderStatus={orderKbn}");
			}

			// 注文メモ
			if (Encoding.UTF8.GetByteCount(order.BuyerComments) > DB_NTEXT_TYPE_MAX_BYTES)
			{
				throw new ArgumentException(
					$"想定のサイズを超えています。BuyerComments={order.BuyerComments}",
					nameof(order.BuyerComments));
			}
			var orderMemo = string.IsNullOrEmpty(order.BuyerComments) == false
				? $"\r\n－－コメント－－\r\n{order.BuyerComments}"
				: "";
			if (Encoding.UTF8.GetByteCount(order.Ship.GiftWrapMessage) > DB_NTEXT_TYPE_MAX_BYTES)
			{
				throw new ArgumentException(
					$"想定のサイズを超えています。GiftWrapMessage={order.Ship.GiftWrapMessage}",
					nameof(order.Ship.GiftWrapMessage));
			}
			orderMemo += string.IsNullOrEmpty(order.Ship.GiftWrapMessage) == false
				? $"\r\n－－ギフトメッセージ－－\r\n{order.Ship.GiftWrapMessage}"
				: "";

			// 注文メモ (カスタムフィールド)
			var properties = new Dictionary<string, string>();
			foreach (var cf in mapper.MappingConfig.CustomFields)
			{
				var orderInfoProperty = YahooApiOrderInfoResponseDto.GetOrderInfoProperties().SingleOrDefault(p => p.Name == cf);
				if (orderInfoProperty != null)
				{
					if (properties.ContainsKey(orderInfoProperty.Name)) continue;
					properties.Add(orderInfoProperty.Name, StringUtility.ToEmpty(orderInfoProperty.GetValue(order)));
					continue;
				}
				var payProperty = YahooApiOrderInfoResponseDto.GetPayProperties().SingleOrDefault(p => p.Name == cf);
				if (payProperty != null)
				{
					if (properties.ContainsKey(payProperty.Name)) continue;
					properties.Add(payProperty.Name, StringUtility.ToEmpty(payProperty.GetValue(order.Pay)));
					continue;
				}
				var shipProperty = YahooApiOrderInfoResponseDto.GetShipProperties().SingleOrDefault(p => p.Name == cf);
				if (shipProperty != null)
				{
					if (properties.ContainsKey(shipProperty.Name)) continue;
					properties.Add(shipProperty.Name, StringUtility.ToEmpty(shipProperty.GetValue(order.Ship)));
					continue;
				}
				var detailProperty = YahooApiOrderInfoResponseDto.GetDetailProperties().SingleOrDefault(p => p.Name == cf);
				if (detailProperty != null)
				{
					if (properties.ContainsKey(detailProperty.Name)) continue;
					properties.Add(detailProperty.Name, StringUtility.ToEmpty(detailProperty.GetValue(order.Detail)));
					continue;
				}
				var sellerProperty = YahooApiOrderInfoResponseDto.GetSellerProperties().SingleOrDefault(p => p.Name == cf);
				if (sellerProperty != null)
				{
					if (properties.ContainsKey(sellerProperty.Name)) continue;
					properties.Add(sellerProperty.Name, StringUtility.ToEmpty(sellerProperty.GetValue(order.Seller)));
					continue;
				}
				var buyerProperty = YahooApiOrderInfoResponseDto.GetBuyerProperties().SingleOrDefault(p => p.Name == cf);
				if (buyerProperty != null)
				{
					if (properties.ContainsKey(buyerProperty.Name)) continue;
					properties.Add(buyerProperty.Name, StringUtility.ToEmpty(buyerProperty.GetValue(order.Buyer)));
				}
			}
			orderMemo += properties.Any()
				? "\r\n■カスタムフィールド■"
				: "";
			foreach (var property in properties)
			{
				orderMemo += string.IsNullOrEmpty(property.Key) == false
					? $"\r\n－－{property.Key}－－\r\n{property.Value}"
					: "";
			}
			this.OrderMemo = orderMemo;

			// 外部連携メモ
			if (Encoding.UTF8.GetByteCount(order.Ship.ShipMethodName) > DB_NTEXT_TYPE_MAX_BYTES)
			{
				throw new ArgumentException(
					$"想定のサイズを超えています。ShipMethodName={order.Ship.ShipMethodName}",
					nameof(order.Ship.ShipMethodName));
			}
			this.RelationMemo = string.IsNullOrEmpty(order.Ship.ShipMethodName) == false
				? $"\r\n－－お届け方法－－\r\n{order.Ship.ShipMethodName}"
				: "";

			// 決済カード支払回数文言
			var cardPayCount = order.Pay.CardPayCount;
			if (cardPayCount.Length > 30)
			{
				FileLogger.WriteError($"想定のサイズを超えています。CardPayCount={order.Pay.CardPayCount}");
				cardPayCount = cardPayCount.Substring(0, 30);
			}
			this.CardInstruments = cardPayCount;

			// 注文者
			var orderOwner = new YahooMallOrderOwner(order);
			this.Owner = orderOwner;
			this.User = orderOwner.GenerateUser(order);

			// 配送先
			this.Shipping = new YahooMallOrderShipping(order);

			// 決済
			this.Payment = new YahooMallOrderPayment(
				order.Pay.PayMethod,
				order.Pay.CombinedPayMethod,
				order.Detail.PayMethodAmount,
				order.Detail.CombinedPayMethodAmount,
				order.Detail.TotalPrice,
				order.Detail.UsePoint,
				order.Detail.TotalMallCouponDiscount,
				order.Detail.GiftWrapCharge,
				mapper);

			// 注文者の住所と配送先の住所が異なるかどうか
			this.DeliversToPlaceOtherThanOrderOwnerAddress = (this.Owner.OwnerName != this.Shipping.ShippingName)
				|| (this.Owner.OwnerZip.Zip != this.Shipping.ShippingZip.Zip)
				|| (this.Owner.OwnerAddr != this.Shipping.ShippingAddr)
				|| (this.Owner.OwnerTel != this.Shipping.ShippingTel);

			if (EnumHelper.TryParseToEnum<YahooApiPublicKeyAuthResponseStatus>(xSwsAuthorizeStatusHeader, out var publicKeyAuthResultCode) == false)
			{
				FileLogger.WriteWarn($"想定外の値です。PublicKeyAuthResult={xSwsAuthorizeStatusHeader}");
			}
			this.PublicKeyAuthResultCode = publicKeyAuthResultCode;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="statusCode">HTTPステータスコード</param>
		/// <param name="reasonPhrase">理由語句</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="dto">データトランスファーオブジェクト</param>
		/// <param name="xSwsAuthorizeStatusHeader">リクエストヘッダー "X-SWS-Authorize-Status" (公開鍵認証の結果)</param>
		public YahooMallOrder(
			HttpStatusCode statusCode,
			string reasonPhrase,
			string orderId,
			YahooApiOrderInfoErrorResponseDto dto,
			string xSwsAuthorizeStatusHeader)
		{
			this.StatusCode = statusCode;
			this.ReasonPhrase = reasonPhrase;

			// 注文ID
			this.OrderId = orderId;

			// エラー
			this.ErrorCode = dto.Code;
			this.ErrorMessage = dto.Message;

			if (EnumHelper.TryParseToEnum<YahooApiPublicKeyAuthResponseStatus>(xSwsAuthorizeStatusHeader, out var publicKeyAuthResultCode) == false)
			{
				FileLogger.WriteWarn($"想定外の値です。PublicKeyAuthResult={xSwsAuthorizeStatusHeader}");
			}
			this.PublicKeyAuthResultCode = publicKeyAuthResultCode;
		}

		/// <summary>
		/// 注文を更新するためのパラメータセット生成
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">ｗ２モール注文ID</param>
		/// <returns>パラメータセット</returns>
		public Hashtable GenerateParamSetToUpdateOrder(string userId, string orderId)
		{
			var result = new Hashtable
			{
				{ Constants.FIELD_ORDER_SHOP_ID, Constants.CONST_DEFAULT_SHOP_ID },
				{ Constants.FIELD_ORDER_ORDER_ID, orderId },
				{ Constants.FIELD_ORDER_RELATION_MEMO, this.RelationMemo },
				{ Constants.FIELD_ORDER_ORDER_PAYMENT_KBN, this.Payment.PaymentKbn },
				{ Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, this.Payment.PaymentStatus },
				{ Constants.FIELD_ORDER_MEMO, this.OrderMemo },
				{ Constants.FIELD_ORDER_CARD_INSTRUMENTS, this.CardInstruments },
				{ Constants.FIELD_ORDER_ORDER_STATUS, this.OrderStatus },
				{ Constants.FIELD_ORDER_USER_ID, userId },
				{ Constants.FIELD_ORDER_REGULATION_MEMO, this.Payment.RegulationMemo },
				{ Constants.FIELD_ORDER_ORDER_PRICE_REGULATION, this.Payment.OrderPriceRegulation },
				{ Constants.FIELD_ORDER_ORDER_PRICE_TOTAL, this.Payment.TotalPrice },
				{ Constants.FIELD_ORDER_LAST_BILLED_AMOUNT, this.Payment.TotalPrice },
				{ Constants.FIELD_ORDER_SETTLEMENT_AMOUNT, this.Payment.TotalPrice },
				{ "is_paid_with_paypay", this.Payment.IsPaidWithPayPay ? "1" : "" },
			};
			return result;
		}

		/// <summary>
		/// 注文者を更新するためのパラメータセット生成
		/// </summary>
		/// <param name="orderId">ｗ２モール注文ID</param>
		/// <returns>パラメータセット</returns>
		public Hashtable GenerateParamSetToUpdateOrderOwner(string orderId)
		{
			var result = new Hashtable
			{
				{ Constants.FIELD_ORDER_ORDER_ID, orderId },
				{ Constants.FIELD_ORDEROWNER_OWNER_KBN, this.Owner.OwnerKbn },
				{ Constants.FIELD_ORDEROWNER_OWNER_NAME, this.Owner.OwnerName },
				{ Constants.FIELD_ORDEROWNER_OWNER_NAME1, this.Owner.OwnerName1 },
				{ Constants.FIELD_ORDEROWNER_OWNER_NAME2, this.Owner.OwnerName2 },
				{ Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR, this.Owner.OwnerMailAddr },
				{ Constants.FIELD_ORDEROWNER_OWNER_ADDR1, this.Owner.OwnerAddr1 },
				{ Constants.FIELD_ORDEROWNER_OWNER_ADDR2, this.Owner.OwnerAddr2 },
				{ Constants.FIELD_ORDEROWNER_OWNER_ADDR3, this.Owner.OwnerAddr3 },
				{ Constants.FIELD_ORDEROWNER_OWNER_ADDR4, this.Owner.OwnerAddr4 },
				{ Constants.FIELD_ORDEROWNER_OWNER_ZIP, this.Owner.OwnerZip.Zip },
				{ Constants.FIELD_ORDEROWNER_OWNER_TEL1, this.Owner.OwnerTel.Tel },
			};
			return result;
		}

		/// <summary>
		/// 注文配送を更新するためのパラメータセット生成
		/// </summary>
		/// <param name="shippingTime">配送希望時間</param>
		/// <param name="orderId">ｗ２モール注文ID</param>
		/// <returns>パラメータセット</returns>
		public Hashtable GenerateParamSetToUpdateOrderShipping(string shippingTime, string orderId)
		{
			var result = new Hashtable
			{
				{ Constants.FIELD_ORDERSHIPPING_ORDER_ID, orderId },
				{ Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME, this.Shipping.ShippingName },
				{ Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1, this.Shipping.ShippingName1 },
				{ Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2, this.Shipping.ShippingName2 },
				{ Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP, this.Shipping.ShippingZip.Zip },
				{ Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1, this.Shipping.ShippingAddr1 },
				{ Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2, this.Shipping.ShippingAddr2 },
				{ Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3, this.Shipping.ShippingAddr3 },
				{ Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4, this.Shipping.ShippingAddr4 },
				{ Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1, this.Shipping.ShippingTel.Tel },
				{ Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE, this.Shipping.ShippingDate },
				{ Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME, shippingTime },
				{
					Constants.FIELD_ORDERSHIPPING_ANOTHER_SHIPPING_FLG,
					this.DeliversToPlaceOtherThanOrderOwnerAddress
						? Constants.FLG_ORDERSHIPPING_ANOTHER_SHIPPING_FLG_VALID
						: Constants.FLG_ORDERSHIPPING_ANOTHER_SHIPPING_FLG_INVALID
				},
			};
			return result;
		}

		/// <summary>
		/// ユーザーを更新するためのパラメータセット生成
		/// </summary>
		/// <param name="user">ユーザー</param>
		/// <returns>パラメータセット</returns>
		public UserModel GenerateParamSetToUpdateUser(UserModel user)
		{
			var newUser = user.Clone();
			newUser.UserKbn = this.User.UserKbn;
			newUser.MailAddr = this.User.UserMailAddr;
			newUser.MailAddr2 = this.User.UserMailAddr2;
			newUser.Name = this.User.UserName;
			newUser.Name1 = this.User.UserName1;
			newUser.Name2 = this.User.UserName2;
			newUser.NameKana = this.User.UserNameKana;
			newUser.NameKana1 = this.User.UserNameKana1;
			newUser.NameKana2 = this.User.UserNameKana2;
			newUser.Zip = this.User.UserZip.Zip;
			newUser.Zip1 = this.User.UserZip.Zip1;
			newUser.Zip2 = this.User.UserZip.Zip2;
			newUser.Addr = this.User.UserAddr;
			newUser.Addr1 = this.User.UserAddr1;
			newUser.Addr2 = this.User.UserAddr2;
			newUser.Addr3 = this.User.UserAddr3;
			newUser.Addr4 = this.User.UserAddr4;
			newUser.Tel1 = this.User.UserTel.Tel;
			newUser.Tel1_1 = this.User.UserTel.Tel1;
			newUser.Tel1_2 = this.User.UserTel.Tel2;
			newUser.Tel1_3 = this.User.UserTel.Tel3;
			newUser.LastChanged = Constants.FLG_LASTCHANGED_BATCH;
			return newUser;
		}

		/// <summary>
		/// 成功したかどうか
		/// </summary>
		/// <returns>成功したかどうか</returns>
		public bool IsSuccessful() => string.IsNullOrEmpty(this.ErrorCode) && string.IsNullOrEmpty(this.ErrorMessage);

		/// <summary>HTTPステータスコード</summary>
		public HttpStatusCode StatusCode { get; }
		/// <summary>理由語句</summary>
		public string ReasonPhrase { get; } = "";
		/// <summary>注文ID</summary>
		public string OrderId { get; } = "";
		/// <summary>注文日時</summary>
		public DateTime OrderDate { get; }
		/// <summary>注文ステータス</summary>
		public string OrderStatus { get; } = "";
		/// <summary>バイヤーコメント</summary>
		public string OrderMemo { get; } = "";
		/// <summary>支払い</summary>
		public YahooMallOrderPayment Payment { get; }
		/// <summary>決済カード支払回数文言</summary>
		public string CardInstruments { get; } = "";
		/// <summary>外部連携メモ</summary>
		public string RelationMemo { get; } = "";
		/// <summary>ユーザー</summary>
		public YahooMallOrderUser User { get; }
		/// <summary>注文者</summary>
		public YahooMallOrderOwner Owner { get; }
		/// <summary>注文配送</summary>
		public YahooMallOrderShipping Shipping { get; }
		/// <summary>注文者の住所と配送先の住所が異なるかどうか</summary>
		public bool DeliversToPlaceOtherThanOrderOwnerAddress { get; }
		/// <summary>エラーコード</summary>
		public string ErrorCode { get; } = "";
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; } = "";
		/// <summary>公開鍵認証結果コード</summary>
		public YahooApiPublicKeyAuthResponseStatus PublicKeyAuthResultCode { get; }
	}
}
