/*
=========================================================================================================
  Module      : AmazonApiメッセージ管理クラス(AmazonApiMessageManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Xml.XPath;
using System.Xml.Linq;

namespace w2.App.Common.Amazon.Util
{
	/// <summary>
	/// AmazonApiメッセージ管理クラス
	/// </summary>
	public class AmazonApiMessageManager
	{
		/// <summary>メッセージ(XML)</summary>
		private static readonly XDocument m_doc = XDocument.Parse(Properties.Resources.AmazonApiMessages);

		/// <summary>
		/// メッセージ種別
		/// </summary>
		private enum MessageType
		{
			/// <summary>エラー</summary>
			Error,
			/// <summary>OrderReference制約</summary>
			OrderReferenceConstraint,
			/// <summary>BillingAgreement制約</summary>
			BillingAgreementConstraint,
		}

		/// <summary>
		/// エラーメッセージ取得
		/// </summary>
		/// <param name="code">エラーコード</param>
		/// <param name="args">メッセージ引数</param>
		/// <returns>エラーメッセージ</returns>
		public static string GetErrorMessage(string code, params string[] args)
		{
			return GetMessage(MessageType.Error, code, args);
		}

		/// <summary>
		/// OrderReference制約メッセージ取得
		/// </summary>
		/// <param name="code">OrderReference制約コード</param>
		/// <param name="args">メッセージ引数</param>
		/// <returns>OrderReference制約メッセージ</returns>
		public static string GetOrderReferenceConstraintMessage(string code, params string[] args)
		{
			return GetMessage(MessageType.OrderReferenceConstraint, code, args);
		}

		/// <summary>
		/// BillingAgreement制約メッセージ取得
		/// </summary>
		/// <param name="code">BillingAgreement制約コード</param>
		/// <param name="args">メッセージ引数</param>
		/// <returns>BillingAgreement制約メッセージ</returns>
		public static string GetBillingAgreementConstraintMessage(string code, params string[] args)
		{
			return GetMessage(MessageType.BillingAgreementConstraint, code, args);
		}

		/// <summary>
		/// メッセージ取得
		/// </summary>
		/// <param name="type">メッセージ種別</param>
		/// <param name="code">メッセージコード</param>
		/// <param name="args">メッセージ引数</param>
		/// <returns>メッセージ</returns>
		private static string GetMessage(MessageType type, string code, params string[] args)
		{
			var element = m_doc
				.XPathSelectElements("AmazonApiMessages/Messages")
				.FirstOrDefault(d => (d.Attribute("type").Value == type.ToString()))
				.XPathSelectElements("Message")
				.FirstOrDefault(d => (d.Attribute("code").Value == code));
			
			if (element == null) return string.Empty;

			var message = string.Format(element.Value, args);
			return message;
		}
	}
}