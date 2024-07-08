/*
=========================================================================================================
  Module      : SBPS リンク型決済 ｸﾚｼﾞｯﾄｶｰﾄﾞ登録要求ページ処理(PaymentSBPSCreditCardRegisterOrder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;
using w2.Common.Web;

namespace w2.App.Common.Order
{
	/// <summary>
	/// SBPS リンク型決済 ｸﾚｼﾞｯﾄｶｰﾄﾞ登録要求ページ
	/// </summary>
	public class PaymentSBPSCreditCardRegisterOrder : PaymentSBPSBaseLink
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaymentSBPSCreditCardRegisterOrder()
			: this(PaymentSBPSSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		public PaymentSBPSCreditCardRegisterOrder(
			PaymentSBPSSetting settings)
			: base(settings, Encoding.UTF8)
		{
		}

		/// <summary>
		/// 注文要求フォーム作成
		/// </summary>
		/// <param name="payMethod">支払い方法</param>
		/// <param name="custCode">顧客ID（決済情報保管時の紐付けキー）</param>
		/// <param name="isKetai">ケータイか（スマートフォンはfalse）</param>
		/// <param name="orderCompleUrl">注文完了URL</param>
		/// <param name="orderCancelUrl">注文キャンセルURL</param>
		/// <param name="errorUrl">エラーURL</param>
		/// <param name="noticeUrl">決済通知用CGI</param>
		/// <param name="freeCsv">「フリー項目」クラス</param>
		/// <returns>フォーム文字列</returns>
		public string CreateOrderFromInputs(
			PaymentSBPSTypes.PayMethodTypes? payMethod,
			string custCode,
			bool isKetai,
			string orderCompleUrl,
			string orderCancelUrl,
			string errorUrl,
			string noticeUrl,
			IPaymentSBPSFreeCSV freeCsv = null)
		{
			var requestParameters = new List<KeyValuePair<string, string>>();
			if (payMethod.HasValue)
			{
				requestParameters.Add(new KeyValuePair<string, string>("pay_method", this.HashCalculator.Add(payMethod.ToString())));
			}
			requestParameters.Add(new KeyValuePair<string, string>("merchant_id", this.HashCalculator.Add(this.Settings.MerchantId)));
			requestParameters.Add(new KeyValuePair<string, string>("service_id", this.HashCalculator.Add(this.Settings.ServiceId)));
			requestParameters.Add(new KeyValuePair<string, string>("cust_code", this.HashCalculator.Add(custCode)));
			//datas.Add(new KeyValuePair<string, string>("sps_cust_no", this.HashCalculator.Add("")));		// 当連携モデルの場合は未設定（空文字）
			//datas.Add(new KeyValuePair<string, string>("sps_payment_no", this.HashCalculator.Add("")));	// 当連携モデルの場合は未設定（空文字）
			requestParameters.Add(new KeyValuePair<string, string>("terminal_type", this.HashCalculator.Add(isKetai ? "1" : "0")));
			requestParameters.Add(new KeyValuePair<string, string>("success_url",  this.HashCalculator.Add(orderCompleUrl)));
			requestParameters.Add(new KeyValuePair<string, string>("cancel_url",  this.HashCalculator.Add(orderCancelUrl)));
			requestParameters.Add(new KeyValuePair<string, string>("error_url",  this.HashCalculator.Add(errorUrl)));
			requestParameters.Add(new KeyValuePair<string, string>("pagecon_url",  this.HashCalculator.Add(noticeUrl)));
			requestParameters.Add(new KeyValuePair<string, string>("free1", this.HashCalculator.Add("")));
			requestParameters.Add(new KeyValuePair<string, string>("free2", this.HashCalculator.Add("")));
			requestParameters.Add(new KeyValuePair<string, string>("free3", this.HashCalculator.Add("")));

			requestParameters.Add(new KeyValuePair<string, string>("request_date",  this.HashCalculator.Add(DateTime.Now.ToString("yyyyMMddHHmmss"))));
			requestParameters.Add(new KeyValuePair<string, string>("limit_second", this.HashCalculator.Add("600")));

			requestParameters.Add(new KeyValuePair<string, string>("sps_hashcode", this.HashCalculator.ComputeHashSHA1AndClearBuffer()));
			if (freeCsv != null)
			{
				requestParameters.Add(new KeyValuePair<string, string>("free_csv", this.HashCalculator.Add(freeCsv.GetFreeCsvString())));
			}

			var form = new StringBuilder();
			foreach (var kv in requestParameters)
			{
				form.Append("<input type=\"hidden\" name=\"")
					.Append(kv.Key)
					.Append("\" value=\"")
					.Append(HtmlSanitizer.HtmlEncode(this.HashCalculator.Add(kv.Value)))
					.Append("\">\r\n");
			}

			return form.ToString();
		}
	}
}
