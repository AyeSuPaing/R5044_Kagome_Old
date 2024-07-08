﻿/*
=========================================================================================================
  Module      : ヤマトKWC ファミリーマート決済API(PaymentYamatoKwcCvs2AuthApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Order.Payment.YamatoKwc.Helper;

namespace w2.App.Common.Order.Payment.YamatoKwc
{
	/// <summary>
	/// ヤマトKWC ファミリーマート決済API
	/// </summary>
	public class PaymentYamatoKwcCvs2AuthApi : PaymentYamatoKwcApiBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reserve1">予備項目設定値(テスト用）</param>
		public PaymentYamatoKwcCvs2AuthApi(string reserve1 = "")
			: base(PaymentYamatoKwcFunctionDiv.B02, reserve1)
		{
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="deviceDiv">端末区分</param>
		/// <param name="orderNo">注文番号</param>
		/// <param name="goodsName">商品名</param>
		/// <param name="settlePrice">決済金額</param>
		/// <param name="buyerNameKanji">購入者名（漢字）</param>
		/// <param name="buyerNameKana">購入者名（フリガナ）</param>
		/// <param name="buyerTel">購入者TEL</param>
		/// <param name="buyerEmail">購入者E-Mail</param>
		/// <returns>実行結果</returns>
		public PaymentYamatoKwcCvs2AuthResponseData Exec(
			PaymentYamatoKwcDeviceDiv deviceDiv,
			string orderNo,
			string goodsName,
			decimal settlePrice,
			string buyerNameKanji,
			string buyerNameKana,
			string buyerTel,
			string buyerEmail)
		{
			var param = CreateParam(
				deviceDiv,
				orderNo,
				goodsName,
				settlePrice,
				buyerNameKanji,
				buyerNameKana,
				buyerTel,
				buyerEmail);

			var resultString = PostHttpRequest(param);
			var responseData = new PaymentYamatoKwcCvs2AuthResponseData(resultString);
			WriteLog(
				Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE,
				PaymentFileLogger.PaymentProcessingType.PaymentByCvs2,
				responseData.Success,
				new KeyValuePair<string, string>("orderNo", orderNo));
			return responseData;
		}

		/// <summary>
		/// パラメタ作成
		/// </summary>
		/// <param name="deviceDiv">端末区分</param>
		/// <param name="orderNo">注文番号</param>
		/// <param name="goodsName">商品名</param>
		/// <param name="settlePrice">決済金額</param>
		/// <param name="buyerNameKanji">購入者名（漢字）</param>
		/// <param name="buyerNameKana">購入者名（フリガナ）</param>
		/// <param name="buyerTel">購入者TEL</param>
		/// <param name="buyerEmail">購入者E-Mail</param>
		/// <returns>パラメタ</returns>
		private string[][] CreateParam(
			PaymentYamatoKwcDeviceDiv deviceDiv,
			string orderNo,
			string goodsName,
			decimal settlePrice,
			string buyerNameKanji,
			string buyerNameKana,
			string buyerTel,
			string buyerEmail)
		{
			var param = new[]
			{
				new[] {"function_div", this.FunctionDiv.ToString()},
				new[] {"trader_code", Constants.PAYMENT_SETTING_YAMATO_KWC_TRADER_CODE},
				new[] {"device_div", ((int)deviceDiv).ToString()},
				new[] {"order_no", orderNo},
				new[] {"goods_name", goodsName},
				new[] {"settle_price", settlePrice.ToPriceString()},
				new[] {"buyer_name_kanji", buyerNameKanji},
				new[] {"buyer_name_kana", buyerNameKana},
				new[] {"buyer_tel", buyerTel},
				new[] {"buyer_email", buyerEmail},
				new[] {"reserve_1", this.Reserve1},
			};
			return param;
		}
	}
}
