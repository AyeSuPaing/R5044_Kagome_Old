/*
=========================================================================================================
  Module      : 後払いの注文変更応答 (AtobaraicomModifyOrderResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Text;
using System.Xml.Linq;

namespace w2.App.Common.Order
{
	/// <summary>
	/// 後払いの注文変更応答
	/// </summary>
	public class AtobaraicomModifyOrderResponse
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

		/// <summary>状態</summary>
		public string Status { get; protected set; }
		/// <summary>注文ID</summary>
		public string OrderId { get; protected set; }
		/// <summary>メッセージ</summary>
		public string Messages { get; set; }
	}
}
