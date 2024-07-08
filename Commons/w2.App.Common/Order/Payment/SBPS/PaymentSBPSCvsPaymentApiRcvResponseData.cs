/*
=========================================================================================================
  Module      : ソフトバンクペイメント WEBコンビニ「入金通知処理」受取API レスポンスデータ(PaymentSBPSCvsPaymentApiRcvResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ソフトバンクペイメント WEBコンビニ「入金通知処理」受取API レスポンスデータ
	/// </summary>
	public class PaymentSBPSCvsPaymentApiRcvResponseData : PaymentSBPSBaseResponseData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		internal PaymentSBPSCvsPaymentApiRcvResponseData(PaymentSBPSSetting settings)
			: base(settings)
		{
			this.HashCalculator = new PaymentSBPSHashCalculator(settings.HashKey, Encoding.GetEncoding("Shift_JIS"));
		}

		/// <summary>
		/// レスポンスをプロパティへ格納(クレジット用にオーバーライド）
		/// </summary>
		/// <param name="responseXml">レスポンスXML</param>
		public override void LoadXml(XDocument responseXml)
		{
			// コンビニ入金通知固有の値をセット（ハッシュのバッファに格納）
			foreach (XElement element in responseXml.Root.Elements())
			{
				switch (element.Name.ToString())
				{
					case "merchant_id":
						this.MerchantId = this.HashCalculator.Add(element.Value);
						break;

					case "service_id":
						this.ServiceId = this.HashCalculator.Add(element.Value);
						break;

					case "sps_transaction_id":
						this.SpsTransactionId = this.HashCalculator.Add(element.Value);
						break;

					case "tracking_id":
						this.TrackingId = this.HashCalculator.Add(element.Value);
						break;

					case "request_date":
						this.RequestDate = 
							DateTime.ParseExact(
								this.HashCalculator.Add(element.Value),
								"yyyyMMddHHmmss",
								System.Globalization.DateTimeFormatInfo.InvariantInfo,
								System.Globalization.DateTimeStyles.None);
						break;

					case "order_id":
						this.OrderId = this.HashCalculator.Add(element.Value);
						break;

					case "rec_datetime":
						this.RecDatetime = DateTime.ParseExact(this.HashCalculator.Add(element.Value), "yyyyMMddHHmmss", null);
						break;

					case "pay_method_info":
						foreach (XElement paymentElement in element.Elements())
						{
							switch (paymentElement.Name.ToString())
							{
								case "rec_type":
									this.RecType = (RecTypes)Enum.Parse(typeof(RecTypes), this.HashCalculator.Add(GetDecryptedData(paymentElement.Value)));
									break;

								case "rec_amount":
									this.RecAmount = decimal.Parse(this.HashCalculator.Add(GetDecryptedData(paymentElement.Value)));
									break;

								case "rec_amount_total":
									this.RecAmountTotal = decimal.Parse(this.HashCalculator.Add(GetDecryptedData(paymentElement.Value)));
									break;

								case "res_mail":
									this.ResMail = this.HashCalculator.Add(GetDecryptedData(paymentElement.Value));
									break;

								case "rec_extra":
									this.RecExtra = this.HashCalculator.Add(GetDecryptedData(paymentElement.Value));
									break;
							}
						}
						break;

					case "sps_hashcode":
						this.SpsHashcode = element.Value;
						break;
				}
			}

			// ハッシュチェック
			string hashCode = this.HashCalculator.ComputeHashSHA1AndClearBuffer();
			if (hashCode != this.SpsHashcode)
			{
				throw new Exception(string.Format("チェックサムが一致しませんでした。{0} <> {1}",  hashCode, this.SpsHashcode));
			}
		}

		/// <summary>マーチャントID</summary>
		public string MerchantId { get; private set; }
		/// <summary>サービスID</summary>
		public string ServiceId { get; private set; }
		/// <summary>トランザクションID</summary>
		public string SpsTransactionId { get; private set; }
		/// <summary>トラッキングID</summary>
		public string TrackingId { get; private set; }
		/// <summary>リクエスト日時</summary>
		public DateTime RequestDate { get; private set; }
		/// <summary>購入ID（購入ID返却オプション利用）</summary>
		public string OrderId { get; private set; }
		/// <summary>処理日時</summary>
		public DateTime RecDatetime { get; private set; }
		/// <summary>入金種別</summary>
		public RecTypes RecType { get; private set; }
		/// <summary>入金金額</summary>
		public decimal RecAmount { get; private set; }
		/// <summary>入金累計金額</summary>
		public decimal RecAmountTotal { get; private set; }
		/// <summary>入金社メールアドレス</summary>
		public string ResMail { get; private set; }
		/// <summary>備考</summary>
		public string RecExtra { get; private set; }
		/// <summary>チェックサム</summary>
		public string SpsHashcode { get; private set; }

		/// <summary>SBPSハッシュ計算クラス</summary>
		protected PaymentSBPSHashCalculator HashCalculator { get; private set; }

		/// <summary>入金種別</summary>
		public enum RecTypes
		{
			/// <summary>速報</summary>
			PrompReport = 1,
			/// <summary>速報取消</summary>
			PrompReportCansel = 2,
			/// <summary>確報</summary>
			FixReport = 3,
			/// <summary>確報取消</summary>
			FixReportCansel = 4
		}
	}
}
