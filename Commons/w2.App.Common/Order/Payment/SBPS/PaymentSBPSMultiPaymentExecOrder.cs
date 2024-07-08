/*
=========================================================================================================
  Module      : ソフトバンクペイメント マルチペイメント「購入要求API」クラス(PaymentSBPSMultiPaymentExecOrder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;
using w2.App.Common.Extensions.Currency;
using w2.Common.Web;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ソフトバンクペイメント マルチペイメント「購入要求API」クラス
	/// </summary>
	public class PaymentSBPSMultiPaymentExecOrder : PaymentSBPSBaseLink
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaymentSBPSMultiPaymentExecOrder()
			: this(PaymentSBPSSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		public PaymentSBPSMultiPaymentExecOrder(
			PaymentSBPSSetting settings)
			: base(settings, Encoding.UTF8)
		{
		}

		/// <summary>
		/// 注文要求フォーム作成
		/// </summary>
		/// <param name="payMethod">支払い方法</param>
		/// <param name="custCode">顧客ID（決済情報保管時の紐付けキー）</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="itemId">商品検索ID</param>
		/// <param name="itemName">商品表示名</param>
		/// <param name="productItems">注文アイテム</param>
		/// <param name="amount">注文金額合計</param>
		/// <param name="isKetai">ケータイか（スマートフォンはfalse）</param>
		/// <param name="orderCompleUrl">注文完了URL</param>
		/// <param name="orderCancelUrl">注文キャンセルURL</param>
		/// <param name="errorUrl">エラーURL</param>
		/// <param name="paymentNoticeUrl">決済通知用CGI</param>
		/// <param name="freeCsv">「フリー項目」クラス</param>
		/// <param name="isRecurringCharge">継続課金（定期・従量）か</param>
		/// <returns>フォーム文字列</returns>
		public string CreateOrderFromInputs(
			PaymentSBPSTypes.PayMethodTypes? payMethod,
			string custCode,
			string orderId,
			string itemId,
			string itemName,
			List<ProductItem> productItems,
			decimal amount,
			bool isKetai,
			string orderCompleUrl,
			string orderCancelUrl,
			string errorUrl,
			string paymentNoticeUrl,
			IPaymentSBPSFreeCSV freeCsv = null,
			bool isRecurringCharge = false)
		{
			List<KeyValuePair<string, string>> datas = new List<KeyValuePair<string, string>>();
			if (payMethod.HasValue)
			{
				datas.Add(new KeyValuePair<string, string>("pay_method", this.HashCalculator.Add(payMethod.ToString())));
			}
			datas.Add(new KeyValuePair<string, string>("merchant_id", this.HashCalculator.Add(this.Settings.MerchantId)));
			datas.Add(new KeyValuePair<string, string>("service_id", this.HashCalculator.Add(this.Settings.ServiceId)));
			datas.Add(new KeyValuePair<string, string>("cust_code", this.HashCalculator.Add(custCode)));
			//datas.Add(new KeyValuePair<string, string>("sps_cust_no", this.HashCalculator.Add("")));		// 当連携モデルの場合は未設定（空文字）
			//datas.Add(new KeyValuePair<string, string>("sps_payment_no", this.HashCalculator.Add("")));	// 当連携モデルの場合は未設定（空文字）
			datas.Add(new KeyValuePair<string, string>("order_id", this.HashCalculator.Add(orderId)));
			datas.Add(new KeyValuePair<string, string>("item_id", this.HashCalculator.Add(itemId)));
			//datas.Add(new KeyValuePair<string, string>("pay_item_id", this.HashCalculator.Add("")));		// 当連携モデルの場合は未設定（空文字）
			datas.Add(new KeyValuePair<string, string>("item_name", this.HashCalculator.Add(itemName)));
			//datas.Add(new KeyValuePair<string, string>("tax", this.HashCalculator.Add("")));
			datas.Add(new KeyValuePair<string, string>("amount", this.HashCalculator.Add(amount.ToPriceString())));
			// 購入タイプ（0: 都度購入 ／ 1:継続課金（簡易）／ 2：継続課金（定期・従量）
			datas.Add(new KeyValuePair<string, string>("pay_type", this.HashCalculator.Add(isRecurringCharge ? "2" : "0")));
			if (isRecurringCharge)
			{
				// 自動課金タイプ（0:自動課金しない／ 1:自動課金する）
				datas.Add(new KeyValuePair<string, string>("auto_charge_type", this.HashCalculator.Add("0")));
				// 決済区分（0：前払い(継続課金のみ)）
				datas.Add(new KeyValuePair<string, string>("div_settele", this.HashCalculator.Add("0")));
			}
			datas.Add(new KeyValuePair<string, string>("service_type", this.HashCalculator.Add("0")));		// 0:売上（購入）／ 1:取消（月額課金解約）
			//datas.Add(new KeyValuePair<string, string>("last_charge_month", this.HashCalculator.Add("")));	// 継続課金（簡易）のみ
			//datas.Add(new KeyValuePair<string, string>("camp_type", this.HashCalculator.Add("")));			// 継続課金（簡易）のみ
			//datas.Add(new KeyValuePair<string, string>("tracking_id", this.HashCalculator.Add("")));		// 取消（月額課金解約）のみ
			datas.Add(new KeyValuePair<string, string>("terminal_type", this.HashCalculator.Add(isKetai ? "1" : "0")));
			datas.Add(new KeyValuePair<string, string>("success_url", this.HashCalculator.Add(orderCompleUrl)));
			datas.Add(new KeyValuePair<string, string>("cancel_url", this.HashCalculator.Add(orderCancelUrl)));
			datas.Add(new KeyValuePair<string, string>("error_url", this.HashCalculator.Add(errorUrl)));
			datas.Add(new KeyValuePair<string, string>("pagecon_url", this.HashCalculator.Add(paymentNoticeUrl)));
			datas.Add(new KeyValuePair<string, string>("free1", this.HashCalculator.Add("")));
			datas.Add(new KeyValuePair<string, string>("free2", this.HashCalculator.Add("")));
			datas.Add(new KeyValuePair<string, string>("free3", this.HashCalculator.Add("")));
			if (freeCsv != null)
			{
				datas.Add(new KeyValuePair<string, string>("free_csv", this.HashCalculator.Add(freeCsv.GetFreeCsvString())));
			}
			/*
			int index = 1;
			foreach (var items in productItems)
			{
				datas.Add(new KeyValuePair<string, string>("dtl_rowno",  this.HashCalculator.Add((index++).ToString())));
				datas.Add(new KeyValuePair<string, string>("dtl_item_id",  this.HashCalculator.Add(items.Id)));
				datas.Add(new KeyValuePair<string, string>("dtl_item_name",  this.HashCalculator.Add(items.Name)));
				datas.Add(new KeyValuePair<string, string>("dtl_item_count",  this.HashCalculator.Add(items.Count.ToString())));
				//datas.Add(new KeyValuePair<string, string>("dtl_tax", this.HashCalculator.Add("")));
				datas.Add(new KeyValuePair<string, string>("dtl_amount",  this.HashCalculator.Add(items.PriceSubtotal.ToString())));
				//datas.Add(new KeyValuePair<string, string>("dtl_free1", this.HashCalculator.Add("")));
				//datas.Add(new KeyValuePair<string, string>("dtl_free2", this.HashCalculator.Add("")));
				//datas.Add(new KeyValuePair<string, string>("dtl_free3", this.HashCalculator.Add("")));
			}*/
			datas.Add(new KeyValuePair<string, string>("request_date", this.HashCalculator.Add(DateTime.Now.ToString("yyyyMMddHHmmss"))));
			datas.Add(new KeyValuePair<string, string>("limit_second", this.HashCalculator.Add("600")));

			datas.Add(new KeyValuePair<string, string>("sps_hashcode", this.HashCalculator.ComputeHashSHA1AndClearBuffer()));

			StringBuilder form = new StringBuilder();
			foreach (var kv in datas)
			{
				form.Append("<input type=\"hidden\" name=\"").Append(kv.Key).Append("\" value=\"").Append(HtmlSanitizer.HtmlEncode(this.HashCalculator.Add(kv.Value))).Append("\">\r\n");
			}

			return form.ToString();
		}
	}
}
