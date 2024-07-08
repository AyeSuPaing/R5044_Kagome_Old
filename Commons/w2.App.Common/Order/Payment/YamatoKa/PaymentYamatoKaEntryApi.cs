/*
=========================================================================================================
  Module      : ヤマト決済(後払い) 決済依頼APIクラス(PaymentYamatoKaEntryApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.YamatoKa.Utils;
using w2.Common.Util;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ヤマト決済(後払い) 決済依頼APIクラス
	/// </summary>
	public class PaymentYamatoKaEntryApi : PaymentYamatoKaBaseApi
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaymentYamatoKaEntryApi()
			: base(@"KAARA0010APIAction_execute")
		{
		}

		/// <summary>
		/// 実行 ヤマト決済(後払い) 決済依頼
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="orderYmd">受注日</param>
		/// <param name="shipYmd">出荷予定日</param>
		/// <param name="name">氏名</param>
		/// <param name="nameKana">氏名カナ</param>
		/// <param name="postCode">郵便番号</param>
		/// <param name="address">住所</param>
		/// <param name="telNum">電話番号</param>
		/// <param name="email">メールアドレス</param>
		/// <param name="totalAmount">決済金額総計</param>
		/// <param name="sendDiv">送り先区分</param>
		/// <param name="productItems">商品アイテム</param>
		/// <param name="sendName">送り先名称</param>
		/// <param name="sendPostCode">送り先郵便番号</param>
		/// <param name="sendAddress">送り先住所</param>
		/// <param name="sendTelNum">送り先電話番号</param>
		/// <returns>実行結果</returns>
		public bool Exec(
			string paymentOrderId,
			string orderYmd,
			string shipYmd,
			string name,
			string nameKana,
			string postCode,
			PaymentYamatoKaAddress address,
			string telNum,
			string email,
			decimal totalAmount,
			PaymentYamatoKaSendDiv sendDiv,
			PaymentYamatoKaProductItem[] productItems,
			string sendName,
			string sendPostCode,
			PaymentYamatoKaAddress sendAddress,
			string sendTelNum)
		{
			// リクエストパラメータ作成
			var requestParam = CreateParam(
				paymentOrderId,
				orderYmd,
				shipYmd,
				name,
				nameKana,
				postCode,
				address.Address1,
				address.Address2,
				telNum,
				email,
				totalAmount,
				sendDiv,
				productItems,
				sendName,
				sendPostCode,
				sendAddress.Address1,
				sendAddress.Address2,
				sendTelNum);

			// 接続・レスポンス取得
			var responseString = this.PostHttpRequest(requestParam);
			this.ResponseDataInner = new PaymentYamatoKaEntryResponseData(responseString);

			// 成功判定
			var result = CheckSuccess(sendDiv);

			// ログ格納処理
			WriteLog(
				result
					? string.Format(
						"送信日時:{0} 受注番号{1}",
						StringUtility.ToEmpty(this.ResponseDataInner.RequestDate),
						StringUtility.ToEmpty(this.ResponseDataInner.OrderNo))
					: string.Format(
						"送信日時:{0} エラーメッセージ:{1} エラーコード:({2}) (審査結果:{3})",
						StringUtility.ToEmpty(this.ResponseDataInner.RequestDate),
						StringUtility.ToEmpty(this.ResponseDataInner.ErrorMessages),
						StringUtility.ToEmpty(this.ResponseDataInner.ErrorCode),
						StringUtility.ToEmpty(this.ResponseData.ResultDescription)) + "\n" + LogCreator.CreateMessage(this.ResponseDataInner.OrderNo, ""),
				result,
				"",
				Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
				PaymentFileLogger.PaymentProcessingType.YamatoPaymentForPostpaidSettlementRequest);

			return result;
		}

		/// <summary>
		/// パラメータ作成 ヤマト決済(後払い) 決済依頼
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="orderYmd">受注日</param>
		/// <param name="shipYmd">出荷予定日</param>
		/// <param name="name">氏名</param>
		/// <param name="nameKana">氏名カナ</param>
		/// <param name="postCode">郵便番号</param>
		/// <param name="address1">住所１</param>
		/// <param name="address2">住所２</param>
		/// <param name="telNum">電話番号</param>
		/// <param name="email">メールアドレス</param>
		/// <param name="totalAmount">決済金額総計</param>
		/// <param name="sendDiv">送り先区分</param>
		/// <param name="productItems">商品アイテム</param>
		/// <param name="sendName">送り先名称</param>
		/// <param name="sendPostCode">送り先郵便番号</param>
		/// <param name="sendAddress1">送り先住所１</param>
		/// <param name="sendAddress2">送り先住所２</param>
		/// <param name="sendTelNum">送り先電話番号</param>
		/// <returns>パラメータ</returns>
		private string[][] CreateParam(
			string paymentOrderId,
			string orderYmd,
			string shipYmd,
			string name,
			string nameKana,
			string postCode,
			string address1,
			string address2,
			string telNum,
			string email,
			decimal totalAmount,
			PaymentYamatoKaSendDiv sendDiv,
			PaymentYamatoKaProductItem[] productItems,
			string sendName,
			string sendPostCode,
			string sendAddress1,
			string sendAddress2,
			string sendTelNum)
		{
			var param = new[]
				{
					new[] {"ycfStrCode", this.YcfStrCode}, // 基本情報
					new[] {"orderNo", paymentOrderId},
					new[] {"orderYmd", orderYmd},
					new[] {"shipYmd", shipYmd},
					new[] {"name", name},
					new[] {"nameKana", nameKana},
					new[] {"postCode", postCode},
					new[] {"address1", address1},
					new[] {"address2", address2},
					new[] {"telNum", telNum},
					new[] {"email", email},
					new[] {"totalAmount", totalAmount.ToPriceString()},
					new[] {"sendDiv", ((int)sendDiv).ToString()},
					new[] {"cartCode", this.CartCode},
				}
				.Concat(CreateProductItemList(productItems)) // 商品情報
				.Concat(
					new[]
					{
						new[] {"sendName", sendName}, // 送り先情報
						new[] {"sendPostCode", sendPostCode},
						new[] {"sendAddress1", sendAddress1},
						new[] {"sendAddress2", sendAddress2},
						new[] {"sendTelNum", sendTelNum},
						new[] {"requestDate", DateTime.Now.ToString("yyyyMMddHHmmss")}, // 共通情報
						new[] {"password", this.Password},
					});
			return param.ToArray();
		}

		/// <summary>
		/// 商品アイテムリスト作成
		/// </summary>
		/// <param name="productItems">商品アイテム</param>
		/// <returns>商品アイテムリスト</returns>
		private string[][] CreateProductItemList(PaymentYamatoKaProductItem[] productItems)
		{
			if (productItems.Length == 0) return null;

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
		/// 成功判定
		/// </summary>
		/// <param name="sendDiv">送り先区分</param>
		/// <returns>成功したか</returns>
		private bool CheckSuccess(PaymentYamatoKaSendDiv sendDiv)
		{
			PaymentYamatoKaEntryResponseData.ResultValue value;
			var isConvertionSuccess = Enum.TryParse(this.ResponseData.Result, out value);

			var result = (isConvertionSuccess
				&& this.ResponseData.IsSuccess
				&& ((sendDiv == PaymentYamatoKaSendDiv.SmsAuth)
					&& (value == PaymentYamatoKaEntryResponseData.ResultValue.UnderExamination)
					|| (value == PaymentYamatoKaEntryResponseData.ResultValue.Available)));

			return result;
		}

		/// <summary>レスポンスデータ</summary>
		public PaymentYamatoKaEntryResponseData ResponseData { get { return (PaymentYamatoKaEntryResponseData)this.ResponseDataInner; } }
	}
}
