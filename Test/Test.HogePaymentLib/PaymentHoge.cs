using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Test.HogePaymentLib.Auth;
using Test.HogePaymentLib.Cancel;
using Test.HogePaymentLib.Sale;

namespace Test.HogePaymentLib
{
	/// <summary>
	/// 設定クラス
	/// </summary>
	public class HogeSetting
	{
		private HogeSetting()
		{
		}

		public HogeSetting(string authUrl, string saleUrl, string cancelUrl)
		{
			this.AuthUrl = authUrl;
			this.SaleUrl = saleUrl;
			this.CancelUrl = cancelUrl;
		}

		public string AuthUrl { get; set; }
		public string SaleUrl { get; set; }
		public string CancelUrl { get; set; }
	}

	public class PaymentHoge
	{
		private PaymentHoge()
		{
		}

		public PaymentHoge(HogeSetting setting)
		{
			this.Setting = setting;

		}

		public SaleResponse Sale(SaleRequest request)
		{
			var xml = HogeHelper.Serialize(request);
			var res = CallApi(this.Setting.SaleUrl, xml);
			if (string.IsNullOrEmpty(res)) { return new SaleResponse() { ResultCode = "NG" }; }
			return HogeHelper.Deserialize<SaleResponse>(res);
		}

		public AuthResponse Auth(AuthRequest request)
		{
			var xml = HogeHelper.Serialize(request);
			var res = CallApi(this.Setting.AuthUrl, xml);
			if (string.IsNullOrEmpty(res)) { return new AuthResponse() { ResultCode = "NG", TransactionID = "", TransactionKey = "" }; }
			return HogeHelper.Deserialize<AuthResponse>(res);
		}

		public CancelResponse Cancel(CancelRequest request)
		{
			var xml = HogeHelper.Serialize(request);
			var res = CallApi(this.Setting.CancelUrl, xml);
			if (string.IsNullOrEmpty(res)) { return new CancelResponse() { ResultCode = "NG" }; }
			return HogeHelper.Deserialize<CancelResponse>(res);
		}

		private string CallApi(string apiUrl, string requestData)
		{
			string responseString = "";

			try
			{
				var postData = Encoding.UTF8.GetBytes(requestData);
				HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
				webRequest.Method = "POST";
				webRequest.ContentType = "application/xml";
				webRequest.ContentLength = postData.Length; // POST送信するデータの長さを指定
				webRequest.Proxy = null; // プロクシは通さない

				using (Stream stream = webRequest.GetRequestStream())
				{
					stream.Write(postData, 0, postData.Length);
				}

				using (WebResponse response = webRequest.GetResponse())
				using (Stream stream = response.GetResponseStream())
				using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
				{
					responseString = reader.ReadToEnd();
				}
			}
			catch
			{
			}

			return responseString;
		}

		private HogeSetting Setting { get; set; }
	}


}
