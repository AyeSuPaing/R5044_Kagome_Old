/*
=========================================================================================================
  Module      : ソフトバンクペイメント API基底クラス(PaymentSBPSBaseApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Order.Payment;
using w2.Common.Util;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ソフトバンクペイメント API基底クラス
	/// </summary>
	public abstract class PaymentSBPSBaseApi : PaymentSBPSBase
	{
		/// <summary>エンコーディング</summary>
		protected Encoding m_encodingPost = Encoding.GetEncoding("Shift_JIS");
		/// <summary>3DESオブジェクト</summary>
		protected PaymentSBPSTripleDESCrypto m_tripleDES = null;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		/// <param name="responseData">レスポンスデータ</param>
		public PaymentSBPSBaseApi(
			PaymentSBPSSetting settings,
			PaymentSBPSBaseResponseData responseData)
			: base(settings, Encoding.GetEncoding("Shift_JIS"))
		{
			this.TargetUrl = settings.ApiUrl;
			if (settings.TripleDESKeyAndIV.HasValue)
			{
				m_tripleDES = new PaymentSBPSTripleDESCrypto(
					settings.TripleDESKeyAndIV.Value.Key,
					settings.TripleDESKeyAndIV.Value.Value);
			}
			this.ResponseDataInner = responseData;
		}

		/// <summary>
		/// 商品アイテムノード作成
		/// </summary>
		/// <param name="productItems">商品アイテム</param>
		/// <returns>商品アイテムノード</returns>
		protected XElement CreateProductItemNode(List<ProductItem> productItems)
		{
			if (productItems.Count == 0) return null;

			int iItemIndexNo = 0;

			return new XElement("dtls",
				productItems.Select(product =>
					new XElement("dtl",
						new XElement("dtl_rowno", this.HashCalculator.Add((++iItemIndexNo).ToString())),
						new XElement("dtl_item_id", this.HashCalculator.Add(product.Id)),
						new XElement("dtl_item_name", EncodeMultibyteToBase64(this.HashCalculator.Add(product.Name))),
						new XElement("dtl_item_count", this.HashCalculator.Add(product.Count.ToString())),
						//new XElement("dtl_tax", this.HashCalculator.Add(product.Tax.ToString())),
						new XElement("dtl_amount", this.HashCalculator.Add(product.PriceSubtotal.ToPriceString()))
					)
				).ToArray<XElement>()
			);
		}

		/// <summary>
		/// XML POST送信
		/// </summary>
		/// <param name="requestXml">リクエストXML</param>
		/// <returns>実行結果</returns>
		protected virtual bool Post(XDocument requestXml)
		{
			// 接続・レスポンス取得
			XDocument responseXml = PostHttpRequest(requestXml);
			this.ResponseDataInner.LoadXml(responseXml);

			// 成功判定
			bool result = false;
			if (this.ResponseDataInner.ResResult == "OK")
			{
				result = true;

				WriteLog(
					true,
					"",
					PaymentFileLogger.PaymentProcessingType.ApiRequest,
					LogCreator.CreateMessageWithTransactionId(
						StringUtility.ToEmpty(this.ResponseDataInner.ResSpsTransactionId),
						this.ResponseDataInner.ResTrackingId));
			}
			else
			{
				result = false;

				WriteLog(
					false,
					"",
					PaymentFileLogger.PaymentProcessingType.ApiRequest,
					LogCreator.CreateErrorMessageWithTransactionId(
						this.ResponseDataInner.ResSpsTransactionId,
						this.ResponseDataInner.ResErrCode,
						this.ResponseDataInner.ResErrMessages));
			}

			return result;
		}

		/// <summary>
		/// POST送信
		/// </summary>
		/// <param name="document">リクエストXML</param>
		/// <returns>戻りXML</returns>
		protected XDocument PostHttpRequest(XDocument document)
		{
			string responseText = PostHttpRequest("<?xml version=\"1.0\" encoding=\"Shift_JIS\"?>\r\n" + document.ToString());

			return XDocument.Parse(responseText);
		}
		/// <summary>
		/// POST送信
		/// </summary>
		/// <param name="requestString">リクエスト文字列</param>
		/// <returns>戻り文字列</returns>
		private string PostHttpRequest(string requestString)
		{
			byte[] postData = m_encodingPost.GetBytes(requestString);

			// POST送信設定
			HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(this.TargetUrl);
			webRequest.Method = WebRequestMethods.Http.Post;
			webRequest.ContentType = "text/xml";
			webRequest.ContentLength = postData.Length;

			// Basic認証ID/PW設定
			webRequest.Headers["Authorization"] = "Basic " +
			Convert.ToBase64String(Encoding.ASCII.GetBytes(this.Settings.BasicAuthenticationId + ":" + this.Settings.BasicAuthenticationPassword));

			// 送信データの書き込み
			Stream stPostStream = webRequest.GetRequestStream();
			stPostStream.Write(postData, 0, postData.Length);	// 送信するデータを書き込む
			stPostStream.Close();

			// レスポンス取得
			string responseText = null;
			using (Stream responseStream = webRequest.GetResponse().GetResponseStream())
			using (StreamReader sr = new StreamReader(responseStream, m_encodingPost))
			{
				responseText = sr.ReadToEnd();
			}
			return responseText;
		}

		/// <summary>
		/// マルチバイト向けbase64エンコード
		/// </summary>
		/// <param name="source">対象</param>
		/// <returns>暗号化データ</returns>
		protected string EncodeMultibyteToBase64(string source)
		{
			return Convert.ToBase64String(m_encodingPost.GetBytes(source));
		}

		/// <summary>
		/// 暗号化データ取得
		/// </summary>
		/// <param name="source">対象</param>
		/// <returns>暗号化データ</returns>
		protected string GetEncryptedData(string source)
		{
			if (m_tripleDES == null) return source;

			return m_tripleDES.GetEncryptedData(source);
		}

		/// <summary>接続先URL</summary>
		protected string TargetUrl { get; set; }
		/// <summary>レスポンスデータ</summary>
		protected PaymentSBPSBaseResponseData ResponseDataInner { get; set; }
	}
}
