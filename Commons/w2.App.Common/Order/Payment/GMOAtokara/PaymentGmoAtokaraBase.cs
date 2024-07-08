/*
=========================================================================================================
  Module      : GMOアトカラ基底クラス(PaymentGmoAtokaraBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using w2.App.Common.Order.Payment.GMOAtokara.Utils;

namespace w2.App.Common.Order.Payment.GMOAtokara
{
	/// <summary>
	/// GMOアトカラ基底クラス
	/// </summary>
	public abstract class PaymentGmoAtokaraBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">設定</param>
		public PaymentGmoAtokaraBase(
			PaymentGmoAtokaraSetting settings)
		{
			this.Settings = settings;
		}

		/// <summary>
		/// ログ出力
		/// </summary>
		/// <param name="isSuccess">成功ならtrue、失敗ならfalse</param>
		/// <param name="processingContent">処理内容</param>
		/// <param name="apiName">API名</param>
		/// <param name="message">メッセージ</param>
		/// <param name="messageDetail">メッセージ詳細</param>
		/// <param name="idKeyAndValueDictionary">ログに入れるID名と値のdictionary</param>
		protected void WriteLog(
			bool? isSuccess,
			PaymentFileLogger.PaymentProcessingType processingContent,
			string apiName,
			string message,
			string messageDetail = "",
			Dictionary<string, string> idKeyAndValueDictionary = null)
		{
			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				isSuccess,
				Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA,
				PaymentFileLogger.PaymentType.GmoAtokara,
				processingContent,
				apiName + "\t" + message + ((string.IsNullOrEmpty(messageDetail) == false) ? "\t" + messageDetail : ""),
				idKeyAndValueDictionary);
		}
		/// <summary>
		/// ログ出力
		/// </summary>
		/// <param name="isSuccess">成功ならtrue、失敗ならfalse</param>
		/// <param name="processingContent">処理内容</param>
		/// <param name="apiName">API名</param>
		/// <param name="requestXml">リクエストXML</param>
		/// <param name="responseXml">レスポンスXML</param>
		protected void WriteLog(
			bool? isSuccess,
			PaymentFileLogger.PaymentProcessingType processingContent,
			string apiName,
			XDocument requestXml,
			XDocument responseXml = null)
		{
			// メッセージ作成
			var logMessage = new StringBuilder()
				.AppendLine($"request_type:{apiName}")
				.AppendLine($"request_value:{requestXml}");
			if (responseXml != null) logMessage.AppendLine($"response:{responseXml}");

			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				isSuccess,
				Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA,
				PaymentFileLogger.PaymentType.GmoAtokara,
				processingContent,
				logMessage.ToString());
		}

		/// <summary>設定</summary>
		protected PaymentGmoAtokaraSetting Settings { get; private set; }

		/// <summary>
		/// 商品アイテム
		/// </summary>
		public struct ProductItem
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="id">ID</param>
			/// <param name="name">商品名</param>
			/// <param name="count">商品数</param>
			/// <param name="priceSubtotal">商品小計</param>
			public ProductItem(string id, string name, int count, decimal priceSubtotal)
			{
				this.Id = id;
				this.Name = name;
				this.Count = count;
				this.PriceSubtotal = priceSubtotal;
			}
			/// <summary>ID</summary>
			public string Id;
			/// <summary>商品名</summary>
			public string Name;
			/// <summary>商品数</summary>
			public int Count;
			/// <summary>商品小計</summary>
			public decimal PriceSubtotal;
		}
	}
}
