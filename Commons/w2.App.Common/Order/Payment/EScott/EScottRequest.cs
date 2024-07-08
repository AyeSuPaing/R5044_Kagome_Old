
/*
=========================================================================================================
  Module      : ソニーペイメントe-SCOTTE リクエスト送信(EScottRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using w2.App.Common.Order.Payment.EScott.Helper;

namespace w2.App.Common.Order.Payment.EScott
{
	/// <summary>
	/// リクエスト送信クラス
	/// </summary>
	internal class EScottRequest
	{
		/// <summary>送信パラメータ</summary>
		private readonly Dictionary<string, string> m_parameter;
		/// <summary>リクエストURL</summary>
		private readonly string m_requestUrl;
		/// <summary>エンコーディング</summary>
		private readonly Encoding m_encodingPost = Encoding.GetEncoding("UTF-8");

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="requestUrl">リクエストURL</param>
		public EScottRequest(string requestUrl)
		{
			m_requestUrl = requestUrl;
			m_parameter = new Dictionary<string, string>();
		}

		/// <summary>
		/// リクエストパラメータ追加
		/// </summary>
		/// <param name="key">キー</param>
		/// <param name="value">値</param>
		public void AddRequestParameter(string key, string value)
		{
			if (string.IsNullOrEmpty(value) == false) m_parameter.Add(key, value);
		}

		/// <summary>
		/// POST送信
		/// </summary>
		/// <returns>戻り文字列</returns>
		public Dictionary<string, string> PostHttpRequestWithResponseSplitting()
		{
			var responseText = PostHttp();

			var result = EScottHelper.SplitResponse(responseText);
			return result;
		}

		/// <summary>
		/// POST送信(レスポンスを分割しない)
		/// </summary>
		/// <returns>戻り文字列</returns>
		public string PostHttpRequestWithoutResponseSplitting()
		{
			var responseText = PostHttp();
			return responseText;
		}

		/// <summary>
		/// POST送信
		/// </summary>
		/// <returns></returns>
		private string PostHttp()
		{
			var requestString = GenerateRequestParamter();


			var postData = m_encodingPost.GetBytes(requestString);

			// POST送信設定
			var webRequest = (HttpWebRequest)WebRequest.Create(this.m_requestUrl);
			webRequest.Method = WebRequestMethods.Http.Post;
			webRequest.ContentType = "application/x-www-form-urlencoded";
			webRequest.ContentLength = postData.Length;

			// ログ書き込み
			WritePaymentRequestLog(requestString);

			// 送信データの書き込み
			var stPostStream = webRequest.GetRequestStream();
			stPostStream.Write(postData, 0, postData.Length);	// 送信するデータを書き込む
			stPostStream.Close();

			// レスポンス取得
			string responseText = null;
			using (Stream responseStream = webRequest.GetResponse().GetResponseStream())
			using (StreamReader sr = new StreamReader(responseStream, m_encodingPost))
			{
				responseText = sr.ReadToEnd();
			}

			// ログ書き込み
			WritePaymentResponseLog(responseText);

			return responseText;
		}

		/// <summary>
		/// リクエストパラメータ電文生成
		/// </summary>
		/// <returns>リクエストパラメータ電文</returns>
		private string GenerateRequestParamter()
		{
			var rseult = string.Join("&", m_parameter.Select(kvp => kvp.Key + "=" + kvp.Value));
			return rseult;
		}

		/// <summary>
		/// パラメータ取得（keyが存在しない場合は空文字を返す）
		/// </summary>
		/// <param name="key">キー</param>
		/// <returns>パラメータ</returns>
		public string GetParameter(string key)
		{
			var result = m_parameter.ContainsKey(key)
				? m_parameter[key]
				: string.Empty;
			return result;
		}

		/// <summary>
		/// リクエストのログ書き込み
		/// </summary>
		/// <param name="parameter">リクエストのパラメータ</param>
		private void WritePaymentRequestLog(string parameter)
		{
			PaymentFileLogger.WritePaymentLog(
				null,
				Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				PaymentFileLogger.PaymentType.EScott,
				PaymentFileLogger.PaymentProcessingType.BeforeApiRequest,
				parameter);
		}

		/// <summary>
		/// レスポンスのログ書き込み
		/// </summary>
		/// <param name="parameter">レスポンスのパラメータ</param>
		private void WritePaymentResponseLog(string parameter)
		{
			PaymentFileLogger.WritePaymentLog(
				null,
				Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				PaymentFileLogger.PaymentType.EScott,
				PaymentFileLogger.PaymentProcessingType.AfterApiRequest,
				parameter);
		}
	}
}
