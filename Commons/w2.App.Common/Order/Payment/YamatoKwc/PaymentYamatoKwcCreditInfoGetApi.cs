/*
=========================================================================================================
  Module      : ヤマトKWC クレジット情報取得API(PaymentYamatoKwcCreditInfoGetApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using w2.App.Common.Order.Payment.YamatoKwc.Helper;

namespace w2.App.Common.Order.Payment.YamatoKwc
{
	/// <summary>
	/// ヤマトKWC クレジット情報取得API
	/// </summary>
	public class PaymentYamatoKwcCreditInfoGetApi : PaymentYamatoKwcApiBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reserve1">予備項目設定値(テスト用）</param>
		public PaymentYamatoKwcCreditInfoGetApi(string reserve1 = "")
			: base(PaymentYamatoKwcFunctionDiv.A03, reserve1)
		{
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="memberId">会員ID</param>
		/// <param name="authenticationKey">認証キー</param>
		/// <returns>結果</returns>
		public PaymentYamatoKwcCreditInfoGetResponseData Exec(string memberId, string authenticationKey)
		{
			var param = CreateParam(memberId, authenticationKey);

			var resultString = PostHttpRequest(param);
			var responseData = new PaymentYamatoKwcCreditInfoGetResponseData(resultString);
			WriteLog(
				Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				PaymentFileLogger.PaymentProcessingType.GetCreditInfo,
				responseData.Success,
				new KeyValuePair<string, string>("memberId", memberId),
				new KeyValuePair<string, string>("authenticationKey", authenticationKey),
				new KeyValuePair<string, string>("CardUnit", responseData.CardUnit.ToString()));
			return responseData;
		}

		/// <summary>
		/// パラメタ作成
		/// </summary>
		/// <param name="memberId">会員ID</param>
		/// <param name="authenticationKey">認証キー</param>
		/// <returns>パラメタ</returns>
		private string[][] CreateParam(string memberId, string authenticationKey)
		{
			var param = new[]
			{
				new[] {"function_div", this.FunctionDiv.ToString()},
				new[] {"trader_code", Constants.PAYMENT_SETTING_YAMATO_KWC_TRADER_CODE},
				new[] {"member_id", memberId},
				new[] {"authentication_key", authenticationKey},
				new[] {"check_sum", PaymentYamatoKwcCheckSumCreater.CreateForAuth(memberId, authenticationKey) },
				new[] {"reserve_1", this.Reserve1},
			};
			return param;
		}
	}
}
