/*
=========================================================================================================
  Module      : ヤマトKWC API基底クラス(PaymentYamatoKwcApiBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using w2.App.Common.Order.Payment.YamatoKwc.Helper;

namespace w2.App.Common.Order.Payment.YamatoKwc
{
	/// <summary>
	/// ヤマトKWC API基底クラス
	/// </summary>
	public class PaymentYamatoKwcApiBase
	{
		/// <summary>URL設定</summary>
		private static readonly PaymentYamatoKwcApiUrlSetting m_urlSetting = null;

		/// <summary>
		/// スタティックコンストラクタ
		/// </summary>
		static PaymentYamatoKwcApiBase()
		{
			m_urlSetting = new PaymentYamatoKwcApiUrlSetting();
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="functionDiv">機能区分</param>
		/// <param name="reserve1">予備項目設定値(テスト用）</param>
		public PaymentYamatoKwcApiBase(PaymentYamatoKwcFunctionDiv functionDiv, string reserve1)
		{
			this.FunctionDiv = functionDiv;
			this.Reserve1 = reserve1;
			this.Url = m_urlSetting.GetUrl(functionDiv);
		}

		/// <summary>
		/// POST送信
		/// </summary>
		/// <param name="param">リクエスト文字列</param>
		/// <returns>戻り文字列</returns>
		protected string PostHttpRequest(string[][] param)
		{
			var paramString = string.Join("&",
				param.Where(p => p[1] != null)
				.Select(p => string.Format("{0}={1}", p[0], HttpUtility.UrlEncode(p[1], Encoding.UTF8))));
			var postData = Encoding.UTF8.GetBytes(paramString);

			var webRequest = (HttpWebRequest)WebRequest.Create(this.Url);
			webRequest.Method = WebRequestMethods.Http.Post;
			webRequest.ContentType = @"application/x-www-form-urlencoded; charset=UTF-8";
			webRequest.ContentLength = postData.Length;

			// 送信データの書き込み
			var stPostStream = webRequest.GetRequestStream();
			stPostStream.Write(postData, 0, postData.Length);	// 送信するデータを書き込む
			stPostStream.Close();

			// レスポンス取得
			string responseText = null;
			using (var responseStream = webRequest.GetResponse().GetResponseStream())
			using (var sr = new StreamReader(responseStream, Encoding.UTF8))
			{
				responseText = sr.ReadToEnd();
			}
			return responseText;
		}

		/// <summary>
		/// ログ出力
		/// </summary>
		/// <param name="kbn">区分（決済の種類）</param>
		/// <param name="processingContent">処理内容</param>
		/// <param name="result">結果（なければnull）</param>
		/// <param name="infos">情報</param>
		public void WriteLog(string kbn, PaymentFileLogger.PaymentProcessingType processingContent, bool? result, params KeyValuePair<string, string>[] infos)
		{
			PaymentYamatoKwcLogger.WriteLog(kbn, processingContent, result, infos);
		}

		/// <summary>機能区分</summary>
		public PaymentYamatoKwcFunctionDiv FunctionDiv { get; set; }
		/// <summary>予備1</summary>
		public string Reserve1 { get; set; }
		/// <summary>URL</summary>
		public string Url { get; set; }

	}
}
