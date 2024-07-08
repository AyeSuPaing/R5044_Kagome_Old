/*
=========================================================================================================
  Module      : 後払い変更注文すべてのアイテムの応答 (AtobaraicomModifyOrderAllItemResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Text;
using System.Xml.Linq;
using w2.App.Common.Order.Payment.Atobaraicom;

namespace w2.App.Common.Order
{
	/// <summary>
	/// 後払い変更注文すべてのアイテムの応答
	/// </summary>
	public class AtobaraicomModifyOrderAllItemResponse
	{
		/// <summary>
		/// レスポンスをプロパティへ格納
		/// </summary>
		/// <param name="responseXml">レスポンスXML</param>
		public virtual void ParserElements(XDocument responseXml)
		{
			foreach (var element in responseXml.Root.Elements())
			{
				switch (element.Name.ToString())
				{
					case "status":
						this.Status = element.Value;
						break;

					case "orderId":
						this.OrderId = element.Value;
						break;

					case "systemOrderId":
						this.SystemOrderId = element.Value;
						break;

					case "orderStatus":
						this.OrderStatus = element.Value;
						break;

					case "messages":
						this.Messages = AtobaraicomUtil.GetMessages(element, "message");
						break;
				}
			}
		}

		/// <summary>
		/// エラーメッセージ取得
		/// </summary>
		/// <param name="responseText">エラーコード</param>
		/// <returns>エラーメッセージ</returns>
		public void HandleMessages(string responseText)
		{
			if (string.IsNullOrWhiteSpace(responseText) == false)
			{
				this.ParserElements(XDocument.Parse(responseText));
			}
		}

		/// <summary>レスポンス文字列</summary>
		public string ResponseString { get; protected set; }
		/// <summary>レスポンスXML</summary>
		public XDocument ResponseXml { get; protected set; }
		/// <summary>状態</summary>
		public string Status { get; protected set; }
		/// <summary>注文ID</summary>
		public string OrderId { get; protected set; }
		/// <summary>システム注文ID</summary>
		public string SystemOrderId { get; protected set; }
		/// <summary>注文の状況</summary>
		public string OrderStatus { get; protected set; }
		/// <summary>メッセージ</summary>
		public string Messages { get; set; }
		/// <summary>Is status error</summary>
		public bool IsError
		{
			get
			{
				return (this.Status == AtobaraicomConstants.ATOBARAICOM_API_RESULT_STATUS_ERROR);
			}
		}
		/// <summary>Is authorize OK</summary>
		public bool IsAuthorizeOK
		{
			get
			{
				return (this.OrderStatus == AtobaraicomConstants.ATOBARAICOM_API_AUTH_RESULT_STATUS_OK);
			}
		}
		/// <summary>Is authorize NG</summary>
		public bool IsAuthorizeNG
		{
			get
			{
				return (this.OrderStatus == AtobaraicomConstants.ATOBARAICOM_API_AUTH_RESULT_STATUS_NG);
			}
		}
		/// <summary>Is during authorize</summary>
		public bool IsAuthorizeHold
		{
			get
			{
				return (this.OrderStatus == AtobaraicomConstants.ATOBARAICOM_API_AUTH_RESULT_STATUS_HOLD);
			}
		}
	}
}
