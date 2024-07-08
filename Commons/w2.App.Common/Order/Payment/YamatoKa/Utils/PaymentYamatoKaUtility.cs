/*
=========================================================================================================
  Module      : ヤマト決済(後払い) ユーティリティークラス(PaymentYamatoKaUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using w2.App.Common.Extensions.Currency;
using w2.Domain.Order;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ヤマト決済(後払い) ユーティリティークラス
	/// </summary>
	public static class PaymentYamatoKaUtility
	{
		/// <summary>
		/// 商品アイテムリスト作成
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート情報</param>
		/// <returns>商品アイテムリスト</returns>
		public static PaymentYamatoKaProductItem[] CreateProductItemList(Hashtable order, CartObject cart)
		{
			var result = CreateProductItemList(cart.PriceTotal);
			return result;
		}
		/// <summary>
		/// 商品アイテムリスト作成
		/// </summary>
		/// <param name="priceTotal">金額</param>
		/// <returns>商品アイテムリスト</returns>
		public static PaymentYamatoKaProductItem[] CreateProductItemList(decimal priceTotal)
		{
			var result = new[]
				{
					new PaymentYamatoKaProductItem(Constants.PAYMENT_SETTING_YAMATO_KA_ITEM_NAME, null, null, priceTotal)
				};
			return result;
		}

		/// <summary>
		/// 商品アイテムリスト作成
		/// </summary>
		/// <param name="productItems">商品アイテム</param>
		/// <returns>商品アイテムリスト</returns>
		public static string[][] CreateProductItemList(PaymentYamatoKaProductItem[] productItems)
		{
			var result = new List<string[]>();
			int indexNo = 0;

			foreach (var productItem in productItems)
			{
				indexNo++;
				result.AddRange(new[]
					{
						new[] { "itemName" + indexNo.ToString(), productItem.ItemName },
						new[] { "itemCount" + indexNo.ToString(), productItem.ItemCount.ToString() },
						new[] { "unitPrice" + indexNo.ToString(), productItem.UnitPrice.ToPriceString() },
						new[] { "subTotal" + indexNo.ToString(), productItem.SubTotal.ToPriceString() },
					});
			}
			return result.ToArray();
		}

		/// <summary>
		/// 送り先区分作成
		/// </summary>
		/// <param name="isSms">SMS決済か</param>
		/// <param name="anothierShippingFlg">別送フラグ</param>
		/// <param name="isRegisteredFromFixedPurchase">定期注文から登録される注文か</param>
		/// <param name="isExecutedByExternalOrderImportBatch">外部受注取り込みバッチから実行されているか</param>
		/// <returns>送り先区分</returns>
		public static PaymentYamatoKaSendDiv CreateSendDiv(
			bool isSms,
			string anothierShippingFlg,
			bool isRegisteredFromFixedPurchase = false,
			bool isExecutedByExternalOrderImportBatch = false)
		{
			if (isSms
				&& (isExecutedByExternalOrderImportBatch == false))
			{
				return isRegisteredFromFixedPurchase ? PaymentYamatoKaSendDiv.SmsAvailable : PaymentYamatoKaSendDiv.SmsAuth;
			}

			if (Constants.PAYMENT_SETTING_YAMATO_KA_INVOICE_BUNDLE)
			{
				// 同梱
				return PaymentYamatoKaSendDiv.Bundle;
			}
			else if (anothierShippingFlg == "0")
			{
				// 別送：本人送り
				return PaymentYamatoKaSendDiv.Myself;
			}
			else
			{
				// 別送：本人以外送り
				return PaymentYamatoKaSendDiv.AnotherShipping;
			}
		}

		/// <summary>
		/// 処理区分作成
		/// </summary>
		/// <param name="paymentNo">送り状番号</param>
		/// <param name="beforePaymentNo">元送り状番号</param>
		/// <param name="deliveryCompanyType">出荷連携配送会社</param>
		/// <returns>処理区分</returns>
		public static PaymentYamatoKaProcessDiv CreateProcessDiv(string paymentNo, string beforePaymentNo, string deliveryCompanyType)
		{
			if ((paymentNo != "") && (deliveryCompanyType != Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_YAMATO))
			{
				// 他社配送分の新規登録又は変更登録
				return PaymentYamatoKaProcessDiv.InsertUpdate;
			}
			else if ((beforePaymentNo == "") && (paymentNo != ""))
			{
				// 新規
				return PaymentYamatoKaProcessDiv.Entry;
			}
			else if ((beforePaymentNo != "") && (paymentNo != ""))
			{
				// 変更
				return PaymentYamatoKaProcessDiv.Update;
			}
			else
			{
				// 取消
				return PaymentYamatoKaProcessDiv.Cancel;
			}
		}

		/// <summary>
		/// 出荷予定日作成
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>出荷予定日</returns>
		public static string CreateYamatoKaShipYmd(OrderModel order)
		{
			return CreateYamatoKaShipYmd(order.Shippings[0].ShippingDate);
		}
		/// <summary>
		/// 出荷予定日作成
		/// </summary>
		/// <param name="cart">カート情報</param>
		/// <returns>出荷予定日</returns>
		public static string CreateYamatoKaShipYmd(CartObject cart)
		{
			return CreateYamatoKaShipYmd(cart.Shippings[0].ShippingDate);
		}
		/// <summary>
		/// 出荷予定日作成
		/// </summary>
		/// <param name="shippingDate">配送希望日</param>
		/// <returns>出荷予定日</returns>
		public static string CreateYamatoKaShipYmd(DateTime? shippingDate = null)
		{
			// 配送希望日の指定がある場合：配送希望日 - 出荷予定期間
			// ない場合：現在日 + 出荷予定期間
			DateTime yamatoKaShipYmd = shippingDate.HasValue
				? shippingDate.Value.AddDays(Constants.PAYMENT_SETTING_YAMATO_KA_SHIPPING_TERM_PLAN * -1)
				: DateTime.Now.AddDays(Constants.PAYMENT_SETTING_YAMATO_KA_SHIPPING_TERM_PLAN);
			return yamatoKaShipYmd.ToString("yyyyMMdd");
		}
	}
}
