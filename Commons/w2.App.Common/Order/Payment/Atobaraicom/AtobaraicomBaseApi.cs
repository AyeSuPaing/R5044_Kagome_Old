/*
=========================================================================================================
  Module      : 後払い基本API (AtobaraicomBaseApi.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using w2.App.Common.Order.Payment;

namespace w2.App.Common.Order
{
	/// <summary>
	/// 後払い基本API
	/// </summary>
	public abstract class AtobaraicomBaseApi
	{
		/// <summary>Encoding post</summary>
		protected Encoding m_encodingPost = Encoding.GetEncoding("Shift_JIS");

		/// <summary>
		/// POST送信
		/// </summary>
		/// <param name="param">Param</param>
		/// <param name="url">URL</param>
		public string PostHttpRequest(string[][] param, string url)
		{
			var responseText = string.Empty;
			var paramString = string.Join(
				"&",
				param.Select(p => string.Format("{0}={1}", p[0], HttpUtility.UrlEncode(p[1], Encoding.UTF8))));
			var postData = Encoding.UTF8.GetBytes(paramString);

			// POST送信設定
			var webRequest = (HttpWebRequest)WebRequest.Create(url);
			webRequest.Method = WebRequestMethods.Http.Post;
			webRequest.ContentType = @"application/x-www-form-urlencoded; charset=UTF-8";
			webRequest.ContentLength = postData.Length;
			webRequest.Timeout = (Constants.PAYMENT_ATOBARAICOM_WEB_REQUEST_TIME_OUT_SECOND * 1000);

			// 送信データの書き込み
			var postStream = webRequest.GetRequestStream();
			postStream.Write(postData, 0, postData.Length); // 送信するデータを書き込む
			postStream.Close();

			// レスポンス取得
			using (var responseStream = webRequest.GetResponse().GetResponseStream())
			using (var sr = new StreamReader(responseStream, Encoding.UTF8))
			{
				responseText = sr.ReadToEnd();
			}
			return responseText;
		}

		/// <summary>
		/// ログを書き込む
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <param name="success">成功</param>
		/// <param name="messageDetail">メッセージ詳細</param>
		/// <param name="paymentDetailType">支払詳細タイプ</param>
		/// <param name="processingContent">コンテンツの処理</param>
		public void WriteLog(
			string message,
			bool? success,
			string messageDetail,
			string paymentDetailType,
			PaymentFileLogger.PaymentProcessingType processingContent)
		{
			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				success,
				paymentDetailType,
				PaymentFileLogger.PaymentType.Atobaraicom,
				processingContent,
				message + "\t" + messageDetail);
		}
	}
}
