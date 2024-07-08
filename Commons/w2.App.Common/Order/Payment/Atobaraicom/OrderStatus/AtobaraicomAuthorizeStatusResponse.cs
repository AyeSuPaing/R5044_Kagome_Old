/*
=========================================================================================================
  Module      : Atobaraicom authorize status response (AtobaraicomAuthorizeStatusResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Xml.Linq;

namespace w2.App.Common.Order.Payment.Atobaraicom.OrderStatus
{
	/// <summary>
	/// Atobaraicom authorize status response
	/// </summary>
	public class AtobaraicomAuthorizeStatusResponse
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public AtobaraicomAuthorizeStatusResponse()
		{
			this.Status = string.Empty;
			this.Messages = string.Empty;
			this.PaymentId = string.Empty;
			this.Results = new Result[0];
		}

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

					case "messages":
						this.Messages = AtobaraicomUtil.GetMessages(element, "message");
						break;

					case "results":
						this.Results = this.GetResults(element);
						break;
				}
			}
		}

		/// <summary>
		/// エラーメッセージ取得
		/// </summary>
		/// <param name="responseText">エラーコード</param>
		public void HandleMessages(string responseText)
		{
			if (string.IsNullOrWhiteSpace(responseText) == false)
			{
				this.ParserElements(XDocument.Parse(responseText));
			}
		}

		/// <summary>
		/// 結果を取得する
		/// </summary>
		/// <param name="messages">メッセージ</param>
		/// <returns>リスト結果</returns>
		private List<Result> GetResults(XElement messages)
		{
			var lstResult = new List<Result>();
			foreach (var element in messages.Elements("result"))
			{
				var item = new Result();
				foreach (var elementChild in element.Elements())
				{
					switch (elementChild.Name.ToString())
					{
						case "orderId":
							this.PaymentId = item.OrderId = elementChild.Value;
							break;

						case "orderStatus":
							item.OrderStatusCdCode = elementChild.Attribute("cd").Value;
							break;

						case "entOrderId":
							item.EntOrderId = elementChild.Value;
							break;
					}
				}
				lstResult.Add(item);
			}
			return lstResult;
		}

		/// <summary>状態</summary>
		public string Status { get; set; }
		/// <summary>結果</summary>
		public IEnumerable<Result> Results { get; set; }
		/// <summary>メッセージ</summary>
		public string Messages { get; set; }
		/// <summary>支払いID</summary>
		public string PaymentId { get; set; }
		/// <summary>Is success</summary>
		public bool IsSuccess
		{
			get { return (this.Status == AtobaraicomConstants.ATOBARAICOM_API_RESULT_STATUS_SUCCESS); }
		}
		/// <summary>Order status cd code</summary>
		public string OrderStatusCdCode { get; set; }

		/// <summary>
		/// 結果
		/// </summary>
		public class Result
		{
			/// <summary>注文ID</summary>
			public string OrderId { get; set; }
			/// <summary>任意注文番号ID</summary>
			public string EntOrderId { get; set; }
			/// <summary>Order status cd code</summary>
			public string OrderStatusCdCode { get; set; }
		}
	}
}
