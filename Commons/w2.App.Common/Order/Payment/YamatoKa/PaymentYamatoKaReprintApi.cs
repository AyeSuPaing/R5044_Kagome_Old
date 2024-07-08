/*
=========================================================================================================
  Module      : ヤマト決済(後払い) 請求書再発行APIクラス(PaymentYamatoKaReprintApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.YamatoKa.Utils;
using w2.Common.Helper.Attribute;
using w2.Common.Util;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ヤマト決済(後払い) 請求書再発行APIクラス
	/// </summary>
	public class PaymentYamatoKaReprintApi : PaymentYamatoKaBaseApi
	{
		/// <summary>不備事由</summary>
		public enum ReissueReason
		{
			/// <summary>宛先不備</summary>
			[EnumTextName("宛先不備")]
			WrongDestination = 1,
			/// <summary>紛失</summary>
			[EnumTextName("紛失")]
			Lost = 2,
			/// <summary>破損・汚損</summary>
			[EnumTextName("破損・汚損")]
			Corruption = 3,
			/// <summary>転居</summary>
			[EnumTextName("転居")]
			Moving = 4,
			/// <summary>入力間違い</summary>
			[EnumTextName("入力間違い")]
			InputMistake = 5,
			/// <summary>その他</summary>
			[EnumTextName("その他")]
			Others = 6,
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaymentYamatoKaReprintApi()
			: base(@"KAARR0010APIAction_execute")
		{
		}

		/// <summary>
		/// 実行 ヤマト決済(後払い) 請求書再発行
		/// </summary>
		/// <param name="orderNo">受注番号</param>
		/// <param name="requestContents">ご依頼内容</param>
		/// <param name="reasonReissue">不備事由</param>
		/// <param name="reasonReissueEtc">不備事由その他</param>
		/// <param name="shipYmd">出荷予定日</param>
		/// <param name="sendDiv">送り先区分</param>
		/// <param name="postCode">郵便番号</param>
		/// <param name="address">住所</param>
		/// <param name="productItems">商品アイテム</param>
		/// <param name="billPostCode">フリー項目</param>
		/// <returns>実行結果</returns>
		public bool Exec(
			string orderNo,
			string requestContents,
			ReissueReason reasonReissue,
			string reasonReissueEtc,
			string shipYmd,
			PaymentYamatoKaSendDiv2 sendDiv,
			string postCode,
			PaymentYamatoKaAddress address,
			PaymentYamatoKaProductItem[] productItems,
			string billPostCode)
		{
			// リクエストパラメータ作成
			var requestParam = CreateParam(
				orderNo,
				requestContents,
				reasonReissue,
				reasonReissueEtc,
				shipYmd,
				sendDiv,
				postCode,
				address.Address1,
				address.Address2,
				productItems,
				billPostCode);

			// 接続・レスポンス取得
			var responseString = this.PostHttpRequest(requestParam);
			this.ResponseDataInner = new PaymentYamatoKaReprintResponseData(responseString);

			// 成功判定
			var result = this.ResponseDataInner.IsSuccess;

			WriteLog(
				result
					? LogCreator.CreateMessageWithRequestData(
						StringUtility.ToEmpty(this.ResponseDataInner.RequestDate),
						StringUtility.ToEmpty(this.ResponseDataInner.OrderNo)) + "\t" + LogCreator.CreateMessage(orderNo, "")
					: LogCreator.CreateErrorMessageWithRequestData(
						StringUtility.ToEmpty(this.ResponseDataInner.RequestDate),
						StringUtility.ToEmpty(this.ResponseDataInner.ErrorCode),
						StringUtility.ToEmpty(this.ResponseDataInner.ErrorMessages)) + "\t" + LogCreator.CreateMessage(orderNo, ""),
				result,
				"",
				Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
				PaymentFileLogger.PaymentProcessingType.InvoiceReissue);

			return result;
		}

		/// <summary>
		/// パラメータ作成 ヤマト決済(後払い) 請求書再発行
		/// </summary>
		/// <param name="orderNo">受注番号</param>
		/// <param name="requestContents">ご依頼内容</param>
		/// <param name="reasonReissue">不備事由</param>
		/// <param name="reasonReissueEtc">不備事由その他</param>
		/// <param name="shipYmd">出荷予定日</param>
		/// <param name="sendDiv">送り先区分</param>
		/// <param name="postCode">郵便番号</param>
		/// <param name="address1">住所１</param>
		/// <param name="address2">住所２</param>
		/// <param name="productItems">商品アイテム</param>
		/// <param name="billPostCode">フリー項目</param>
		/// <returns>パラメータ</returns>
		private string[][] CreateParam(
			string orderNo,
			string requestContents,
			ReissueReason reasonReissue,
			string reasonReissueEtc,
			string shipYmd,
			PaymentYamatoKaSendDiv2 sendDiv,
			string postCode,
			string address1,
			string address2,
			PaymentYamatoKaProductItem[] productItems,
			string billPostCode)
		{
			var param = new[]
				{
					new[] {"ycfStrCode", this.YcfStrCode}, // 基本情報
					new[] {"orderNo", orderNo},
					new[] {"password", this.Password},
					new[] {"requestContents", requestContents},
					new[] {"reasonReissue", ((int)reasonReissue).ToString()},
					new[] {"reasonReissueEtc", reasonReissueEtc},
					new[] {"shipYmd", shipYmd},
					new[] {"sendDiv", ((int)sendDiv).ToString()},
					new[] {"postCode", postCode},
					new[] {"address1", address1},
					new[] {"address2", address2},
				}
				.Concat(PaymentYamatoKaUtility.CreateProductItemList(productItems)) // 商品情報
				.Concat(
					new[]
					{
						new[] {"billPostCode", billPostCode},
					});
			return param.ToArray();
		}

		/// <summary>レスポンスデータ</summary>
		public PaymentYamatoKaReprintResponseData ResponseData { get { return (PaymentYamatoKaReprintResponseData)this.ResponseDataInner; } }
	}
}
