/*
=========================================================================================================
  Module      : ヤマトKWC 取引情報照会APIレスポンスデータ(PaymentYamatoKwcTradeInfoResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace w2.App.Common.Order.Payment.YamatoKwc
{
	/// <summary>
	/// ヤマトKWC 取引情報照会APIレスポンスデータ
	/// </summary>
	public class PaymentYamatoKwcTradeInfoResponseData : PaymentYamatoKwcResponseDataBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="responseString">レスポンス文字列</param>
		public PaymentYamatoKwcTradeInfoResponseData(string responseString)
			: base(responseString)
		{
		}

		/// <summary>
		/// レスポンスをプロパティへ格納
		/// </summary>
		/// <param name="responseXml">レスポンスXML</param>
		public override void SetPropertyFromXml(XDocument responseXml)
		{
			// 基底クラスのメソッド呼び出し
			base.SetPropertyFromXml(responseXml);

			// 固有の値をセット
			foreach (var element in responseXml.Root.Elements())
			{
				switch (element.Name.ToString())
				{
					case "resultCount":
						this.ResultCount = int.Parse(element.Value);
						break;

					case "resultData":
						this.ResultDatas.Add(new ResultData(element));
						break;
				}
			}
		}

		/// <summary>結果件数</summary>
		public int ResultCount { get; private set; }
		/// <summary>データ</summary>
		public List<ResultData> ResultDatas
		{
			get { return m_resultDatas; }
		}
		private readonly List<ResultData> m_resultDatas = new List<ResultData>();

		/// <summary>
		/// 結果データ
		/// </summary>
		public class ResultData
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="cardDataElement"></param>
			public ResultData(XElement cardDataElement)
			{
				// 固有の値をセット
				foreach (var element in cardDataElement.Elements())
				{
					switch (element.Name.ToString())
					{
						case "orderNo":
							this.OrderNo = element.Value;
							break;

						case "deviceDiv":
							this.DeviceDiv = element.Value;
							break;

						case "settleDate":
							this.SettleDate = string.IsNullOrEmpty(element.Value) ? (DateTime?)null : DateTime.ParseExact(element.Value, "yyyyMMddHHmmss", null);
							
							break;

						case "orderDataSeq":	
							this.OrderDataSeq = element.Value;
							break;

						case "settlePrice":
							this.SettlePrice = element.Value;
							break;

						case "settleMethodDiv":
							this.SettleMethodDiv = element.Value;
							break;

						case "settleMethod":
							this.SettleMethod = element.Value;
							break;

						case "statusInfo":
							this.StatusInfo = element.Value;
							break;

						case "memberId":
							this.MemberId = element.Value;
							break;

						case "payWay":
							this.PayWay = element.Value;
							break;

						case "crdCResCd":
							this.CrdCResCd = element.Value;
							break;

						case "crdCResDate":
							this.CrdCResDate = element.Value;
							break;

						case "threeDCode":
							this.ThreeDCode = element.Value;
							break;

						case "voteNumber":
							this.VoteNumber = element.Value;
							break;

						case "expiredDays":
							this.ExpiredDays = string.IsNullOrEmpty(element.Value) ? (DateTime?)null : DateTime.ParseExact(element.Value, "yyyyMMddHHmmss", null);
							break;

						case "emoneyExpiredDays":
							this.EmoneyExpiredDays = string.IsNullOrEmpty(element.Value) ? (DateTime?)null : DateTime.ParseExact(element.Value, "yyyyMMddHHmmss", null);
							break;

						case "packingInfo":
							this.PackingInfo = element.Value;
							break;

						case "slipNo":
							this.SlipNo = element.Value;
							break;

						case "extraSlipNo":
							this.ExtraSlipNo = element.Value;
							break;

						case "deliveryServiceCode":
							this.DeliveryServiceCode = element.Value;
							break;

						case "allDeliveryDate":
							this.AllDeliveryDate = string.IsNullOrEmpty(element.Value) ? (DateTime?)null : DateTime.ParseExact(element.Value, "yyyyMMdd", null);
							break;

						case "finishDate":
							this.FinishDate = string.IsNullOrEmpty(element.Value) ? (DateTime?)null : DateTime.ParseExact(element.Value, "yyyyMMdd", null);
							break;

						case "slipDate":
							this.SlipDate = string.IsNullOrEmpty(element.Value) ? (DateTime?)null : DateTime.ParseExact(element.Value, "yyyyMMddHHmmss", null);
							break;

						case "scheduledShippingDate":
							this.ScheduledShippingDate = string.IsNullOrEmpty(element.Value) ? (DateTime?)null : DateTime.ParseExact(element.Value, "yyyyMMdd", null);
							break;

						case "warningInfo":
							this.WarningInfo = element.Value;
							break;

						case "cancelDate":
							this.CancelDate = string.IsNullOrEmpty(element.Value) ? (DateTime?)null : DateTime.ParseExact(element.Value, "yyyyMMddHHmmss", null);
							break;

						case "renewalDate":
							this.RenewalDate = string.IsNullOrEmpty(element.Value) ? (DateTime?)null : DateTime.ParseExact(element.Value, "yyyyMMddHHmmss", null);
							break;

					}
				}
			}

			/// <summary>受付番号</summary>
			public string OrderNo { get; set; }
			/// <summary>端末区分</summary>
			public string DeviceDiv { get; set; }
			/// <summary>決済日時</summary>
			/// <remarks>注文完了後、エンドユーザーからの支払いがない場合 <settleDate/>形式で返すため、nullとなります。</remarks>
			public DateTime? SettleDate { get; set; }
			/// <summary>お問い合わせ番号</summary>
			public string OrderDataSeq { get; set; }
			/// <summary>決済金額</summary>
			public string SettlePrice { get; set; }
			/// <summary>決済方法</summary>
			public string SettleMethodDiv { get; set; }
			/// <summary>決済手段</summary>
			public string SettleMethod { get; set; }
			/// <summary>取引状況</summary>
			public string StatusInfo { get; set; }
			/// <summary>カード保有者を特定するID</summary>
			public string MemberId { get; set; }
			/// <summary>支払回数</summary>
			public string PayWay { get; set; }
			/// <summary>与信承認番号</summary>
			public string CrdCResCd { get; set; }
			/// <summary>与信承認日</summary>
			public string CrdCResDate { get; set; }
			/// <summary>３Ｄ認証結果</summary>
			public string ThreeDCode { get; set; }
			/// <summary>コンビニ注文番号</summary>
			public string VoteNumber { get; set; }
			/// <summary>コンビニ支払期限</summary>
			public DateTime? ExpiredDays { get; set; }
			/// <summary>電子マネー支払期限</summary>
			public DateTime? EmoneyExpiredDays { get; set; }
			/// <summary>同梱情報受付番号</summary>
			public string PackingInfo { get; set; }
			/// <summary>出荷送り状番号</summary>
			public string SlipNo { get; set; }
			/// <summary>送り状番号（その他）</summary>
			public string ExtraSlipNo { get; set; }
			/// <summary>配送サービスコード</summary>
			public string DeliveryServiceCode { get; set; }
			/// <summary>全量出荷日</summary>
			public DateTime? AllDeliveryDate { get; set; }
			/// <summary>全量配完日</summary>
			public DateTime? FinishDate { get; set; }
			/// <summary>出荷情報登録日</summary>
			public DateTime? SlipDate { get; set; }
			/// <summary>出荷予定日</summary>
			public DateTime? ScheduledShippingDate { get; set; }
			/// <summary>警報情報</summary>
			public string WarningInfo { get; set; }
			/// <summary>取消日</summary>
			public DateTime? CancelDate { get; set; }
			/// <summary>認証結果</summary>
			public DateTime? RenewalDate { get; set; }
		}
	}
}
