/*
=========================================================================================================
  Module      : ヤマト決済(後払い) API基底クラス(PaymentYamatoKaBaseApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
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
	/// ヤマト決済(後払い) API基底クラス
	/// </summary>
	public abstract class PaymentYamatoKaBaseApi
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="targetApi">接続先API</param>
		public PaymentYamatoKaBaseApi(string targetApi)
		{
			this.ApiUrl = Constants.PAYMENT_SETTING_YAMATO_KA_API_URL + targetApi + (Constants.PAYMENT_SETTING_YAMATO_KA_DEVELOP ? ".ashx" : ".action");
			this.YcfStrCode = Constants.PAYMENT_SETTING_YAMATO_KA_TRADER_CODE;
			this.Password = Constants.PAYMENT_SETTING_YAMATO_KA_TRADER_PASSWORD;
			this.CartCode = Constants.PAYMENT_SETTING_YAMATO_KA_CART_CODE;
		}

		/// <summary>
		/// POST送信
		/// </summary>
		/// <param name="param">リクエスト文字列</param>
		/// <returns>戻り文字列</returns>
		protected string PostHttpRequest(string[][] param)
		{
			var paramString = string.Join("&",
				param.Select(p => string.Format("{0}={1}", p[0], HttpUtility.UrlEncode(p[1], Encoding.UTF8))));
			var postData = Encoding.UTF8.GetBytes(paramString);

			// POST送信設定
			var webRequest = (HttpWebRequest)WebRequest.Create(this.ApiUrl);
			webRequest.Method = WebRequestMethods.Http.Post;
			webRequest.ContentType = @"application/x-www-form-urlencoded; charset=UTF-8";
			webRequest.ContentLength = postData.Length;

			// 送信データの書き込み
			var postStream = webRequest.GetRequestStream();
			postStream.Write(postData, 0, postData.Length);	// 送信するデータを書き込む
			postStream.Close();

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
		/// エラーログ出力
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <param name="messageDetail">詳細メッセージ</param>
		/// <param name="paymentDetailType">決済種別</param>
		/// <param name="processingContent">処理内容</param>
		protected void WriteErrorLog(string message, string messageDetail, string paymentDetailType, PaymentFileLogger.PaymentProcessingType processingContent)
		{
			WriteLog(message, false, messageDetail, paymentDetailType, processingContent);
		}

		/// <summary>
		/// 成功ログ出力
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <param name="messageDetail">詳細メッセージ</param>
		/// <param name="paymentDetailType">決済種別</param>
		/// <param name="processingContent">処理内容</param>
		protected void WriteSuccessLog(string message, string messageDetail, string paymentDetailType, PaymentFileLogger.PaymentProcessingType processingContent)
		{
			WriteLog(message, true, messageDetail, paymentDetailType, processingContent);
		}

		/// <summary>
		/// ログ出力 
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <param name="success">成功だったらtrue、失敗だったらfalse、その他であればnull</param>
		/// <param name="messageDetail">詳細メッセージ</param>
		/// <param name="paymentDetailType">決済種別</param>
		/// <param name="processingContent">処理内容</param>
		protected void WriteLog(string message, bool? success, string messageDetail, string paymentDetailType, PaymentFileLogger.PaymentProcessingType processingContent)
		{
			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				success,
				paymentDetailType,
				PaymentFileLogger.PaymentType.Yamatoka,
				processingContent,
				message + "\t" + messageDetail);
		}

		/// <summary>API URL</summary>
		protected string ApiUrl { get; private set; }
		/// <summary>加盟店コード</summary>
		protected string YcfStrCode { get; private set; }
		/// <summary>加盟店パスワード</summary>
		protected string Password { get; private set; }
		/// <summary>パートナー社識別コード</summary>
		protected string CartCode { get; private set; }
		/// <summary>レスポンスデータ</summary>
		protected PaymentYamatoKaBaseResponseData ResponseDataInner { get; set; }
	}
}
