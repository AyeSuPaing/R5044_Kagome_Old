/*
=========================================================================================================
  Module      : 後払いキャンセル応答 (AtobaraicomCancelResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Xml.Linq;
using w2.App.Common.Order.Payment.Atobaraicom;

namespace w2.App.Common.Order
{
	/// <summary>
	/// 後払いキャンセル応答
	/// </summary>
	public class AtobaraicomCancelResponse
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
		/// <returns>エラーメッセージ</returns>
		public void HandleMessages(string responseText)
		{
			if (string.IsNullOrWhiteSpace(responseText) == false)
			{
				this.ParserElements(XDocument.Parse(responseText));
			}
		}

		/// <summary>
		/// 結果
		/// </summary>
		public class Result
		{
			/// <summary>注文ID</summary>
			public string OrderId { get; set; }
			/// <summary>理由</summary>
			public string Reason { get; set; }
			/// <summary>ステータスをキャンセル</summary>
			public string CancelStatus { get; set; }
			/// <summary>エラー</summary>
			public string Error { get; set; }
			/// <summary>エラーコード</summary>
			public string ErrorCode { get; set; }
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

						case "reason":
							item.Reason = elementChild.Value;
							break;

						case "cancelStatus":
							this.CancelStatus = item.CancelStatus = elementChild.Value;
							break;

						case "error":
							this.ErrorCode = item.Error = elementChild.Value;
							this.ErrorMessage = item.ErrorCode = AtobaraicomUtil.GetMessages(elementChild, "error");
							break;
					}
				}
				lstResult.Add(item);
			}
			return lstResult;
		}

		/// <summary>レスポンス文字列</summary>
		public string ResponseString { get; set; }
		/// <summary>レスポンスXML</summary>
		public XDocument ResponseXml { get; set; }
		/// <summary>状態</summary>
		public string Status { get; set; }
		/// <summary>結果</summary>
		public IEnumerable<Result> Results { get; set; }
		/// <summary>ステータスをキャンセル</summary>
		public string CancelStatus { get; set; }
		/// <summary>メッセージ</summary>
		public string Messages { get; set; }
		/// <summary>エラーコード</summary>
		public string ErrorCode { get; set; }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; set; }
		/// <summary>支払いID</summary>
		public string PaymentId { get; set; }
		/// <summary>Is cancel satus error</summary>
		public bool IsCancelStatusError
		{
			get
			{
				return (this.CancelStatus == AtobaraicomConstants.ATOBARAICOM_API_RESULT_STATUS_ERROR);
			}
		}
	}
}
