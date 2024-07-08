using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common;
using w2.App.Common.Order.Payment.YamatoKwc;
using w2.App.Common.Order.Payment.YamatoKwc.Helper;

namespace YamatoTest
{
	class Program
	{
		static void Main(string[] args)
		{
			// ★本番★
			Constants.PAYMENT_SETTING_YAMATO_KWC_TRADER_CODE = "888889974";
			Constants.PAYMENT_SETTING_YAMATO_KWC_ACCESS_KEY = "1111111";
			Constants.PAYMENT_SETTING_YAMATO_KWC_API_URL_TYPE = PaymentYamatoKwcApiUrlSetting.UrlType.Test;	// 本番
			Constants.PAYMENT_SETTING_YAMATO_KWC_GOODS_NAME = "お買い物商品";

			var sampleErrorCode = "A012060001";
			var userId = "USR23456789";
			var userCardKey = "USR001";
			var orderId = "ODR" + DateTime.Now.ToString("yyyyMMddHHmmss");
			var slipNo = "123456789013";
			var cardCodeApi = "1";
			// 本番用
			//var cardNo = "5250731583824004";
			//var securityCode = "022";
			// テスト用
			var cardNo = "0000000000000001";
			var securityCode = "022";
			var cardOwner = "WTWO TARO";
			var cardExp = "1017";
			var authKey = "authkey";

			object result;

			{
				// カード情報参照
				var resultInfo = new PaymentYamatoKwcCreditInfoGetApi("").Exec(
					userId,
					authKey);

				//var optionParam = new PaymentYamatoKwcCreditOptionServiceParamNormal(cardCodeApi, cardNo, securityCode, cardOwner, cardExp);
				//var optionParam = new PaymentYamatoKwcCreditOptionServiceParamOptionReg(cardCodeApi, cardNo, securityCode, cardOwner, cardExp, userId, authKey);
				var optionParam = new PaymentYamatoKwcCreditOptionServiceParamOptionRep(userId, authKey, resultInfo.CardDatas[0], securityCode);

				// 通常カード決済
				result = new PaymentYamatoKwcCreditAuthApi("").Exec(
					PaymentYamatoKwcDeviceDiv.Pc,
					orderId,
					Constants.PAYMENT_SETTING_YAMATO_KWC_GOODS_NAME,
					1,
					"ｗ２てすと",
					"090-7248-1616",
					"ochiai@w2solution.co.jp",
					1,
					optionParam);
			}

			{
				// カード金額変更
				var resultInfo = new PaymentYamatoKwcCreditChangePriceApi().Exec(
					orderId,
					100);
			}

			


			//{
			//	// カード情報削除
			//	result = new PaymentYamatoKwcCreditInfoDeleteApi("").Exec(
			//		userCardKey,
			//		authKey,
			//		userCardKey,
			//		DateTime.Now);
			//}

			//{
			//	// カード決済キャンセル
			//	result = new PaymentYamatoKwcCreditCancelApi("").Exec(orderId);
			//}

			//{
			//	// コンビニセブン
			//	result = new PaymentYamatoKwcCvs1AuthApi("").Exec(
			//		PaymentYamatoKwcDeviceDiv.Pc,
			//		orderId,
			//		Constants.PAYMENT_SETTING_YAMATO_KWC_GOODS_NAME,
			//		5600,
			//		"ｗ２太郎",
			//		"ochiai@w2solution.co.jp");
			//}

			//{
			//	// コンビニファミマ
			//	result = new PaymentYamatoKwcCvs2AuthApi("").Exec(
			//		PaymentYamatoKwcDeviceDiv.Pc,
			//		orderId,
			//		Constants.PAYMENT_SETTING_YAMATO_KWC_GOODS_NAME,
			//		5600,
			//		"ｗ２太郎",
			//		"ダブルツダロウ",
			//		"090-7248-1616",
			//		"ochiai@w2solution.co.jp");
			//}

			//{
			//	// コンビニローソン
			//	result = new PaymentYamatoKwcCvs3AuthApi(YamatoKwcFunctionDivCvs.B03, "").Exec(
			//		PaymentYamatoKwcDeviceDiv.Pc,
			//		orderId,
			//		Constants.PAYMENT_SETTING_YAMATO_KWC_GOODS_NAME,
			//		5600,
			//		"ｗ２太郎",
			//		"090-1414-1919",
			//		"ochiai@w2solution.co.jp");
			//}

			//{
			//	// 出荷情報登録
			//	result = new PaymentYamatoKwcShipmentEntryApi("").Exec(orderId, slipNo);
			//}

			//{
			//	// 出荷情報キャンセル
			//	result = new PaymentYamatoKwcShipmentCancelApi("").Exec(orderId, slipNo);
			//}

			{
				// 出荷情報キャンセル
				result = new PaymentYamatoKwcTradeInfoApi().Exec(orderId);
			}


			result = null;
		}
	}
}
