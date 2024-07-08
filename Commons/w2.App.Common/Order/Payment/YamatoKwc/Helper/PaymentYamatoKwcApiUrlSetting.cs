/*
=========================================================================================================
  Module      : ヤマトKWC API URL設定(PaymentYamatoKwcApiUrlSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;

namespace w2.App.Common.Order.Payment.YamatoKwc.Helper
{
	/// <summary>
	/// ヤマトKWCAPI URL設定
	/// </summary>
	public class PaymentYamatoKwcApiUrlSetting
	{
		/// <summary>
		/// URLタイプ
		/// </summary>
		public enum UrlType
		{
			/// <summary>ヤマトKWC本番環境</summary>
			Product,
			/// <summary>ヤマトKWC検証環境</summary>
			Test,
			/// <summary>w2社内開発環境</summary>
			Develop,
			/// <summary>w2社外開発環境</summary>
			W2Test
		}

		/// <summary>
		/// URL
		/// </summary>
		private readonly Dictionary<UrlType, Dictionary<PaymentYamatoKwcFunctionDiv, string>> m_url =
			new Dictionary<UrlType, Dictionary<PaymentYamatoKwcFunctionDiv, string>>()
			{
				{
					UrlType.Product,
					new Dictionary<PaymentYamatoKwcFunctionDiv, string>
					{
						{PaymentYamatoKwcFunctionDiv.A01, @"https://api.kuronekoyamato.co.jp/api/credit"},
						{PaymentYamatoKwcFunctionDiv.A02, @"https://api.kuronekoyamato.co.jp/api/credit3D"},
						{PaymentYamatoKwcFunctionDiv.A03, @"https://api.kuronekoyamato.co.jp/api/creditInfoGet"},
						{PaymentYamatoKwcFunctionDiv.A04, @"https://api.kuronekoyamato.co.jp/api/creditInfoUpdate"},
						{PaymentYamatoKwcFunctionDiv.A05, @"https://api.kuronekoyamato.co.jp/api/creditInfoDelete"},
						{PaymentYamatoKwcFunctionDiv.A06, @"https://api.kuronekoyamato.co.jp/api/creditCancel"},
						{PaymentYamatoKwcFunctionDiv.A07, @"https://api.kuronekoyamato.co.jp/api/creditChangePrice"},
						{PaymentYamatoKwcFunctionDiv.A08, @"https://api.kuronekoyamato.co.jp/api/creditToken"},
						{PaymentYamatoKwcFunctionDiv.A09, @"https://api.kuronekoyamato.co.jp/api/creditToken3D"},
						{PaymentYamatoKwcFunctionDiv.B01, @"https://api.kuronekoyamato.co.jp/api/cvs1"},
						{PaymentYamatoKwcFunctionDiv.B02, @"https://api.kuronekoyamato.co.jp/api/cvs2"},
						{PaymentYamatoKwcFunctionDiv.B03, @"https://api.kuronekoyamato.co.jp/api/cvs3"},
						{PaymentYamatoKwcFunctionDiv.B04, @"https://api.kuronekoyamato.co.jp/api/cvs3"},
						{PaymentYamatoKwcFunctionDiv.B05, @"https://api.kuronekoyamato.co.jp/api/cvs3"},
						{PaymentYamatoKwcFunctionDiv.B06, @"https://api.kuronekoyamato.co.jp/api/cvs3"},
						{PaymentYamatoKwcFunctionDiv.E01, @"https://api.kuronekoyamato.co.jp/api/shipmentEntry"},
						{PaymentYamatoKwcFunctionDiv.E02, @"https://api.kuronekoyamato.co.jp/api/shipmentCancel"},
						{PaymentYamatoKwcFunctionDiv.E04, @"https://api.kuronekoyamato.co.jp/api/tradeInfo"},
					}
				},
				{
					UrlType.Test,
					new Dictionary<PaymentYamatoKwcFunctionDiv, string>
					{
						{PaymentYamatoKwcFunctionDiv.A01, @"https://ptwebcollect.jp/test_gateway/credit.api"},
						{PaymentYamatoKwcFunctionDiv.A02, @"https://ptwebcollect.jp/test_gateway/credit3D.api"},
						{PaymentYamatoKwcFunctionDiv.A03, @"https://ptwebcollect.jp/test_gateway/creditInfoGet.api"},
						{PaymentYamatoKwcFunctionDiv.A04, @"https://ptwebcollect.jp/test_gateway/creditInfoUpdate.api"},
						{PaymentYamatoKwcFunctionDiv.A05, @"https://ptwebcollect.jp/test_gateway/creditInfoDelete.api"},
						{PaymentYamatoKwcFunctionDiv.A06, @"https://ptwebcollect.jp/test_gateway/creditCancel.api"},
						{PaymentYamatoKwcFunctionDiv.A07, @"https://ptwebcollect.jp/test_gateway/creditChangePrice.api"},
						{PaymentYamatoKwcFunctionDiv.A08, @"https://ptwebcollect.jp/test_gateway/creditToken.api"},
						{PaymentYamatoKwcFunctionDiv.A09, @"https://ptwebcollect.jp/test_gateway/creditToken3D.api"},
						{PaymentYamatoKwcFunctionDiv.B01, @"https://ptwebcollect.jp/test_gateway/cvs1.api"},
						{PaymentYamatoKwcFunctionDiv.B02, @"https://ptwebcollect.jp/test_gateway/cvs2.api"},
						{PaymentYamatoKwcFunctionDiv.B03, @"https://ptwebcollect.jp/test_gateway/cvs3.api"},
						{PaymentYamatoKwcFunctionDiv.B04, @"https://ptwebcollect.jp/test_gateway/cvs3.api"},
						{PaymentYamatoKwcFunctionDiv.B05, @"https://ptwebcollect.jp/test_gateway/cvs3.api"},
						{PaymentYamatoKwcFunctionDiv.B06, @"https://ptwebcollect.jp/test_gateway/cvs3.api"},
						{PaymentYamatoKwcFunctionDiv.E01, @"https://ptwebcollect.jp/test_gateway/shipmentEntry.api"},
						{PaymentYamatoKwcFunctionDiv.E02, @"https://ptwebcollect.jp/test_gateway/shipmentCancel.api"},
						{PaymentYamatoKwcFunctionDiv.E04, @"https://ptwebcollect.jp/test_gateway/tradeInfo.api"},
					}
				},
				{
					UrlType.Develop,
					new Dictionary<PaymentYamatoKwcFunctionDiv, string>
					{
						{PaymentYamatoKwcFunctionDiv.A01, @"http://localhost/R5044_Kagome.Develop/Test/Web.PaymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.A02, @"http://localhost/R5044_Kagome.Develop/Test/Web.PaymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.A03, @"http://localhost/R5044_Kagome.Develop/Test/Web.PaymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.A04, @"http://localhost/R5044_Kagome.Develop/Test/Web.PaymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.A05, @"http://localhost/R5044_Kagome.Develop/Test/Web.PaymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.A06, @"http://localhost/R5044_Kagome.Develop/Test/Web.PaymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.A07, @"http://localhost/R5044_Kagome.Develop/Test/Web.PaymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.A08, @"http://localhost/R5044_Kagome.Develop/Test/Web.PaymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.A09, @"http://localhost/R5044_Kagome.Develop/Test/Web.PaymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.B01, @"http://localhost/R5044_Kagome.Develop/Test/Web.PaymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.B02, @"http://localhost/R5044_Kagome.Develop/Test/Web.PaymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.B03, @"http://localhost/R5044_Kagome.Develop/Test/Web.PaymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.B04, @"http://localhost/R5044_Kagome.Develop/Test/Web.PaymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.B05, @"http://localhost/R5044_Kagome.Develop/Test/Web.PaymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.B06, @"http://localhost/R5044_Kagome.Develop/Test/Web.PaymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.E01, @"http://localhost/R5044_Kagome.Develop/Test/Web.PaymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.E02, @"http://localhost/R5044_Kagome.Develop/Test/Web.PaymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.E04, @"http://localhost/R5044_Kagome.Develop/Test/Web.PaymentTest/YamatoKwc/Api.ashx"},
					}
				},
				{
					UrlType.W2Test,
					new Dictionary<PaymentYamatoKwcFunctionDiv, string>
					{
						{PaymentYamatoKwcFunctionDiv.A01, @"http://tryout.w2solution.com/paymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.A02, @"http://tryout.w2solution.com/paymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.A03, @"http://tryout.w2solution.com/paymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.A04, @"http://tryout.w2solution.com/paymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.A05, @"http://tryout.w2solution.com/paymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.A06, @"http://tryout.w2solution.com/paymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.A07, @"http://tryout.w2solution.com/paymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.A08, @"http://tryout.w2solution.com/paymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.A09, @"http://tryout.w2solution.com/paymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.B01, @"http://tryout.w2solution.com/paymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.B02, @"http://tryout.w2solution.com/paymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.B03, @"http://tryout.w2solution.com/paymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.B04, @"http://tryout.w2solution.com/paymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.B05, @"http://tryout.w2solution.com/paymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.B06, @"http://tryout.w2solution.com/paymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.E01, @"http://tryout.w2solution.com/paymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.E02, @"http://tryout.w2solution.com/paymentTest/YamatoKwc/Api.ashx"},
						{PaymentYamatoKwcFunctionDiv.E04, @"http://tryout.w2solution.com/paymentTest/YamatoKwc/Api.ashx"},
					}
				},
			};

		/// <summary>
		/// URL取得
		/// </summary>
		/// <param name="functionDiv"></param>
		/// <returns></returns>
		public string GetUrl(PaymentYamatoKwcFunctionDiv functionDiv)
		{
			return m_url[Constants.PAYMENT_SETTING_YAMATO_KWC_API_URL_TYPE][functionDiv];
		}
	}
}
