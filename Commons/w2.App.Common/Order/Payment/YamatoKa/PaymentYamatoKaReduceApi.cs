/*
=========================================================================================================
  Module      : ヤマト決済(後払い) 請求金額変更（減額）依頼APIクラス(PaymentYamatoKaReduceApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.Order.Payment.YamatoKa.Utils;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Order.Payment;
using w2.Common.Util;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ヤマト決済(後払い) 請求金額変更（減額）依頼APIクラス
	/// </summary>
	public class PaymentYamatoKaReduceApi : PaymentYamatoKaBaseApi
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaymentYamatoKaReduceApi()
			: base(@"KAAKK0010APIAction_execute")
		{
		}

		/// <summary>
		/// 実行 ヤマト決済(後払い) 請求金額変更（減額）依頼
		/// </summary>
		/// <param name="orderNo">受注番号</param>
		/// <param name="orderYmd">受注日</param>
		/// <param name="shipYmd">出荷予定日</param>
		/// <param name="postCode">郵便番号</param>
		/// <param name="address">住所</param>
		/// <param name="productItems">商品アイテム</param>
		/// <param name="totalAmount">決済金額総計</param>
		/// <param name="sendDiv">送り先区分</param>
		/// <param name="billPostCode">フリー項目</param>
		/// <returns>実行結果</returns>
		public bool Exec(
			string orderNo,
			string orderYmd,
			string shipYmd,
			string postCode,
			PaymentYamatoKaAddress address,
			PaymentYamatoKaProductItem[] productItems,
			decimal totalAmount,
			PaymentYamatoKaSendDiv2 sendDiv,
			string billPostCode)
		{
			// リクエストパラメータ作成
			var requestParam = CreateParam(
				orderNo,
				orderYmd,
				shipYmd,
				postCode,
				address.Address1,
				address.Address2,
				productItems,
				totalAmount,
				sendDiv,
				billPostCode);

			// 接続・レスポンス取得
			var responseString = this.PostHttpRequest(requestParam);
			this.ResponseDataInner = new PaymentYamatoKaReduceResponseData(responseString);

			// 成功判定
			var result = this.ResponseDataInner.IsSuccess;

			WriteLog(
				result
					? LogCreator.CreateMessageWithRequestData(
						StringUtility.ToEmpty(this.ResponseDataInner.RequestDate),
						StringUtility.ToEmpty(this.ResponseDataInner.OrderNo))
					: LogCreator.CreateErrorMessageWithRequestData(
						StringUtility.ToEmpty(this.ResponseDataInner.RequestDate),
						StringUtility.ToEmpty(this.ResponseDataInner.ErrorCode),
						StringUtility.ToEmpty(this.ResponseDataInner.ErrorMessages) + "\t" + LogCreator.CreateMessage(orderNo, "")),
				result,
				"",
				Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
				PaymentFileLogger.PaymentProcessingType.ChargeChangeForReductionRequest);

			return result;
		}

		/// <summary>
		/// パラメータ作成 ヤマト決済(後払い) 請求金額変更（減額）依頼
		/// </summary>
		/// <param name="orderNo">受注番号</param>
		/// <param name="orderYmd">受注日</param>
		/// <param name="shipYmd">出荷予定日</param>
		/// <param name="postCode">郵便番号</param>
		/// <param name="address1">住所１</param>
		/// <param name="address2">住所２</param>
		/// <param name="productItems">商品アイテム</param>
		/// <param name="totalAmount">決済金額総計</param>
		/// <param name="sendDiv">送り先区分</param>
		/// <param name="billPostCode">フリー項目</param>
		/// <returns>パラメータ</returns>
		private string[][] CreateParam(
			string orderNo,
			string orderYmd,
			string shipYmd,
			string postCode,
			string address1,
			string address2,
			PaymentYamatoKaProductItem[] productItems,
			decimal totalAmount,
			PaymentYamatoKaSendDiv2 sendDiv,
			string billPostCode)
		{
			var param = new[]
				{
					new[] {"ycfStrCode", this.YcfStrCode}, // 基本情報
					new[] {"orderNo", orderNo},
					new[] {"orderYmd", orderYmd},
					new[] {"shipYmd", shipYmd},
					new[] {"postCode", postCode},
					new[] {"address1", address1},
					new[] {"address2", address2},
				}
				.Concat(PaymentYamatoKaUtility.CreateProductItemList(productItems)) // 商品情報
				.Concat(
					new[]
					{
						new[] {"totalAmount", totalAmount.ToPriceString()},
						new[] {"sendDiv", ((int)sendDiv).ToString()},
						new[] {"billPostCode", billPostCode},
						new[] {"password", this.Password},
						new[] {"requestDate", DateTime.Now.ToString("yyyyMMddHHmmss")},
					});
			return param.ToArray();
		}

		/// <summary>レスポンスデータ</summary>
		public PaymentYamatoKaReduceResponseData ResponseData { get { return (PaymentYamatoKaReduceResponseData)this.ResponseDataInner; } }
	}
}
