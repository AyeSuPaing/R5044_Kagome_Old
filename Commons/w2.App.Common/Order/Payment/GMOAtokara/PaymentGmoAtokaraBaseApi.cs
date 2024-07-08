/*
=========================================================================================================
  Module      : GMOアトカラ API基底クラス(PaymentGmoAtokaraBaseApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;
using w2.App.Common.Order.Payment.GMOAtokara.Utils;

namespace w2.App.Common.Order.Payment.GMOAtokara
{
	/// <summary>
	/// GMOアトカラ API基底クラス
	/// </summary>
	public abstract class PaymentGmoAtokaraBaseApi : PaymentGmoAtokaraBase
	{
		/// <summary>エンコーディング</summary>
		protected Encoding _encodingPost = Encoding.GetEncoding("UTF-8");

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="apiUrl">APIURL</param>
		/// <param name="settings">設定</param>
		/// <param name="responseData">レスポンスデータ</param>
		public PaymentGmoAtokaraBaseApi(
			string apiUrl,
			PaymentGmoAtokaraSetting settings,
			PaymentGmoAtokaraBaseResponseData responseData)
			: base(settings)
		{
			this.TargetUrl = apiUrl;
			this.ResponseDataInner = responseData;
		}

		/// <summary>
		/// 認証情報ノード作成
		/// </summary>
		/// <returns>認証情報ノード</returns>
		protected XElement CreateShopInfoNode()
		{
			return new XElement("shopInfo",
				new XElement("authenticationId", this.Settings.AuthenticationId),
				new XElement("shopCode", this.Settings.ShopCode),
				new XElement("connectPassword", this.Settings.ConnectPassword)
			);
		}

		/// <summary>
		/// XML POST送信
		/// </summary>
		/// <param name="requestXml">リクエストXML</param>
		/// <returns>実行結果</returns>
		protected virtual bool Post(XDocument requestXml)
		{
			var apiName = this.GetType().Name;
			WriteLog(
				true,
				PaymentFileLogger.PaymentProcessingType.ApiRequest,
				apiName,
				requestXml);

			// 接続・レスポンス取得
			var responseXml = PostHttpRequest(requestXml);
			this.ResponseDataInner.LoadXml(responseXml);

			// 成功判定
			var result = this.ResponseDataInner.Result == PaymentGmoAtokaraConstants.RESULT_OK;
			WriteLog(
				result,
				PaymentFileLogger.PaymentProcessingType.ApiRequestEnd,
				apiName,
				requestXml,
				responseXml);

			return result;
		}

		/// <summary>
		/// POST送信
		/// </summary>
		/// <param name="document">リクエストXML</param>
		/// <returns>戻りXML</returns>
		protected XDocument PostHttpRequest(XDocument document)
		{
			var responseText = PostHttpRequest("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" + document.ToString());

			return XDocument.Parse(responseText);
		}
		/// <summary>
		/// POST送信
		/// </summary>
		/// <param name="requestString">リクエスト文字列</param>
		/// <returns>戻り文字列</returns>
		private string PostHttpRequest(string requestString)
		{
			var postData = _encodingPost.GetBytes(requestString);

			// POST送信設定
			var webRequest = (HttpWebRequest)WebRequest.Create(this.TargetUrl);
			webRequest.Method = WebRequestMethods.Http.Post;
			webRequest.ContentType = "text/xml";
			webRequest.ContentLength = postData.Length;

			// Basic認証ID/PW設定
			webRequest.Headers["Authorization"] = "Basic " +
			Convert.ToBase64String(Encoding.ASCII.GetBytes(this.Settings.AuthenticationId + ":" + this.Settings.ConnectPassword));

			// 送信データの書き込み
			var postStream = webRequest.GetRequestStream();
			postStream.Write(postData, 0, postData.Length); // 送信するデータを書き込む
			postStream.Close();

			// レスポンス取得
			string responseText = null;
			using (var responseStream = webRequest.GetResponse().GetResponseStream())
			using (var reader = new StreamReader(responseStream, _encodingPost))
			{
				responseText = reader.ReadToEnd();
			}
			return responseText;
		}

		/// <summary>
		/// エラーメッセージ取得
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string GetErrorMessage()
		{
			var result = ((this.ResponseDataInner != null) && (this.ResponseDataInner.Errors != null))
				? this.ResponseDataInner.Errors.GetErrorMessage()
				: "";
			return result;
		}
		/// <summary>接続先URL</summary>
		protected string TargetUrl { get; set; }
		/// <summary>レスポンスデータ</summary>
		protected PaymentGmoAtokaraBaseResponseData ResponseDataInner { get; set; }
	}
}
