/*
=========================================================================================================
  Module      : Zcom Check Auth Response (ZcomCheckAuthResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Xml;
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.GMO.Zcom.CheckAuth
{
	/// <summary>
	/// Zcom check order response
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "GlobalPayment_result", IsNullable = false, Namespace = "")]
	public class ZcomCheckAuthResponse : BaseZcomResponse
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ZcomCheckAuthResponse()
			: base()
		{
		}

		/// <summary>Response object</summary>
		[XmlElement("result")]
		public Response[] ResponseObject { get; set; }

		/// <summary>
		/// Response
		/// </summary>
		[Serializable]
		public class Response
		{
			/// <summary>契約コード</summary>
			[XmlAttribute("contract_code")]
			public string ContractCode { get; set; }
			/// <summary>バージョン</summary>
			[XmlAttribute("version")]
			public string Version { get; set; }
			/// <summary>文字コード</summary>
			[XmlAttribute("character_code")]
			public string CharacterCode { get; set; }
			/// <summary>処理区分</summary>
			[XmlAttribute("process_code")]
			public string ProcessCode { get; set; }
			/// <summary>決済状態</summary>
			[XmlAttribute("state")]
			public string State { get; set; }
			/// <summary>ユーザーID</summary>
			[XmlAttribute("user_id")]
			public string UserId { get; set; }
			/// <summary>ユーザー氏名</summary>
			[XmlAttribute("user_name")]
			public string UserName { get; set; }
			/// <summary>メールアドレス</summary>
			[XmlAttribute("user_mail_add")]
			public string UserMailAdd { get; set; }
			/// <summary>利用言語</summary>
			[XmlAttribute("lang_id")]
			public string LangId { get; set; }
			/// <summary>商品コード</summary>
			[XmlAttribute("item_code")]
			public string ItemCode { get; set; }
			/// <summary>商品名</summary>
			[XmlAttribute("item_name")]
			public string ItemName { get; set; }
			/// <summary>オーダー番号</summary>
			[XmlAttribute("order_number")]
			public string OrderNumber { get; set; }
			/// <summary>トランザクションコード</summary>
			[XmlAttribute("trans_code")]
			public string TransCode { get; set; }
			/// <summary>決済区分</summary>
			[XmlAttribute("st_code")]
			public string StCode { get; set; }
			/// <summary>課金区分</summary>
			[XmlAttribute("mission_code")]
			public string MissionCode { get; set; }
			/// <summary>通貨コード</summary>
			[XmlAttribute("currency_id")]
			public string CurrencyId { get; set; }
			/// <summary>価格</summary>
			[XmlAttribute("item_price")]
			public string ItemPrice { get; set; }
			/// <summary>予備1</summary>
			[XmlAttribute("memo1")]
			public string Memo1 { get; set; }
			/// <summary>予備2</summary>
			[XmlAttribute("memo2")]
			public string Memo2 { get; set; }
			/// <summary>注文日時</summary>
			[XmlAttribute("receipt_date")]
			public string ReceiptDate { get; set; }
			/// <summary>決済方法</summary>
			[XmlAttribute("payment_code")]
			public string PaymentCode { get; set; }
			/// <summary>課金日時</summary>
			[XmlAttribute("charge_date")]
			public string ChargeDate { get; set; }
		}

		/// <summary>
		/// Get response
		/// </summary>
		/// <returns>Response</returns>
		public Response GetResponse()
		{
			if (this.ResponseObject == null) return null;

			var response = new Response();
			foreach (var item in this.ResponseObject)
			{
				var properties = item.GetType().GetProperties();
				foreach (var property in properties)
				{
					if (property.GetValue(item) == null) continue;

					property.SetValue(response, property.GetValue(item));
				}
			}
			return response;
		}
	}
}
