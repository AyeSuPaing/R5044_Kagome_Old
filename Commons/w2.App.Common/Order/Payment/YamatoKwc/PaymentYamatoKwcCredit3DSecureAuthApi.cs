/*
=========================================================================================================
  Module      : ヤマトKWC トークン決済登録３Ｄセキュア結果用(PaymentYamatoKwcCredit3DSecureAuthApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Specialized;
using w2.App.Common.Order.Payment.YamatoKwc.Helper;

namespace w2.App.Common.Order.Payment.YamatoKwc
{
	public class PaymentYamatoKwcCredit3DSecureAuthApi : PaymentYamatoKwcApiBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reserve1">予備項目設定値(テスト用）</param>
		public PaymentYamatoKwcCredit3DSecureAuthApi(string reserve1 = "")
			: base(PaymentYamatoKwcFunctionDiv.A09, reserve1)
		{
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="returnParam">3Dセキュアからの戻り値</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="threeDToken">3Dトークン</param>
		/// <returns>結果</returns>
		public PaymentYamatoKwcCreditAuthResponseData Exec(
			NameValueCollection returnParam,
			string paymentOrderId,
			string threeDToken)
		{
			var param = CreateParam(returnParam, paymentOrderId, threeDToken);
			var resultString = PostHttpRequest(param);
			var responseData = new PaymentYamatoKwcCreditAuthResponseData(resultString);
			return responseData;
		}

		/// <summary>
		/// パラメタ作成(A09:トークン決済登録３Ｄセキュア結果用)
		/// </summary>
		/// <param name="returnParam">3Dセキュアからの戻り値</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="threeDToken">3Dトークン</param>
		/// <returns>パラメタ</returns>
		private string[][] CreateParam(NameValueCollection returnParam, string paymentOrderId, string threeDToken)
		{
			var param = new[]
			{
				new[] { PaymentYamatoKwcConstants.REQUEST_PARAM_A09_FUNCTION_DIV, this.FunctionDiv.ToString() },
				new[] { PaymentYamatoKwcConstants.REQUEST_PARAM_A09_TRADER_CODE, Constants.PAYMENT_SETTING_YAMATO_KWC_TRADER_CODE },
				new[] { PaymentYamatoKwcConstants.REQUEST_PARAM_A09_ORDER_NO, paymentOrderId },
				new[] { PaymentYamatoKwcConstants.REQUEST_PARAM_A09_COMP_CD, returnParam[PaymentYamatoKwcConstants.RESPONSE_PARAM_3D_SECURE_COMP_CD] },
				new[] { PaymentYamatoKwcConstants.REQUEST_PARAM_A09_TOKEN, returnParam[PaymentYamatoKwcConstants.RESPONSE_PARAM_3D_SECURE_TOKEN] },
				new[] { PaymentYamatoKwcConstants.REQUEST_PARAM_A09_CARD_EXP, returnParam[PaymentYamatoKwcConstants.RESPONSE_PARAM_3D_SECURE_CARD_EXP] },
				new[] { PaymentYamatoKwcConstants.REQUEST_PARAM_A09_ITEM_PRICE, returnParam[PaymentYamatoKwcConstants.RESPONSE_PARAM_3D_SECURE_ITEM_PRICE] },
				new[] { PaymentYamatoKwcConstants.REQUEST_PARAM_A09_ITEM_TAX, returnParam[PaymentYamatoKwcConstants.RESPONSE_PARAM_3D_SECURE_ITEM_TAX] },
				new[] { PaymentYamatoKwcConstants.REQUEST_PARAM_A09_CUST_CD, returnParam[PaymentYamatoKwcConstants.RESPONSE_PARAM_3D_SECURE_CUST_CD] },
				new[] { PaymentYamatoKwcConstants.REQUEST_PARAM_A09_SHOP_ID, returnParam[PaymentYamatoKwcConstants.RESPONSE_PARAM_3D_SECURE_SHOP_ID] },
				new[] { PaymentYamatoKwcConstants.REQUEST_PARAM_A09_TERM_CD, returnParam[PaymentYamatoKwcConstants.RESPONSE_PARAM_3D_SECURE_TERM_CD] },
				new[] { PaymentYamatoKwcConstants.REQUEST_PARAM_A09_RES_CD, returnParam[PaymentYamatoKwcConstants.RESPONSE_PARAM_3D_SECURE_CRD_RES_CD] },
				new[] { PaymentYamatoKwcConstants.REQUEST_PARAM_A09_RES_VE, returnParam[PaymentYamatoKwcConstants.RESPONSE_PARAM_3D_SECURE_RES_VE] },
				new[] { PaymentYamatoKwcConstants.REQUEST_PARAM_A09_RES_PA, returnParam[PaymentYamatoKwcConstants.RESPONSE_PARAM_3D_SECURE_RES_PA] },
				new[] { PaymentYamatoKwcConstants.REQUEST_PARAM_A09_RES_CODE, returnParam[PaymentYamatoKwcConstants.RESPONSE_PARAM_3D_SECURE_RES_CODE] },
				new[] { PaymentYamatoKwcConstants.REQUEST_PARAM_A09_THREE_D_INF, returnParam[PaymentYamatoKwcConstants.RESPONSE_PARAM_3D_SECURE_3D_INF] },
				new[] { PaymentYamatoKwcConstants.REQUEST_PARAM_A09_THREE_D_TRAN_ID, returnParam[PaymentYamatoKwcConstants.RESPONSE_PARAM_3D_SECURE_3D_TRAN_ID] },
				new[] { PaymentYamatoKwcConstants.REQUEST_PARAM_A09_SEND_DT, returnParam[PaymentYamatoKwcConstants.RESPONSE_PARAM_3D_SECURE_SEND_DT] },
				new[] { PaymentYamatoKwcConstants.REQUEST_PARAM_A09_HASH_VALUE, returnParam[PaymentYamatoKwcConstants.RESPONSE_PARAM_3D_SECURE_HASH_VALUE] },
				new[] { PaymentYamatoKwcConstants.REQUEST_PARAM_A09_THREE_D_TOKEN, threeDToken },
			};
			return param;
		}
	}
}
