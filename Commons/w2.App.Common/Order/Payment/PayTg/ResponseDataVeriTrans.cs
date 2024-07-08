/*
=========================================================================================================
  Module      : PayTgのAPI受信データ(ResponseDataVeriTrans.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Runtime.Serialization;

namespace w2.App.Common.Order.Payment.PayTg
{
	/// <summary>
	/// PayTgのAPI受信データ
	/// </summary>
	[DataContract]
	public class ResponseDataVeriTrans
	{
		/// <summary>PayTg端末処理結果</summary>
		[DataMember(Name = PayTgConstants.PARAM_COMRESULT)]
		public string ComResult { get; set; }
		/// <summary>処理結果ステータス</summary>
		[DataMember(Name = PayTgConstants.PARAM_MSTATUS)]
		public string Mstatus { get; set; }
		/// <summary>詳細結果コード</summary>
		[DataMember(Name = PayTgConstants.PARAM_VRESULTCODE)]
		public string VResultCode { get; set; }
		/// <summary>決済エラーメッセージ</summary>
		[DataMember(Name = PayTgConstants.PARAM_ERRORMSG)]
		public string ErrorMsg { get; set; }
		/// <summary>電文受信日時</summary>
		[DataMember(Name = PayTgConstants.PARAM_REQUESTDATE)]
		public string RequestDate { get; set; }
		/// <summary>電文送信日時</summary>
		[DataMember(Name = PayTgConstants.PARAM_RESPONSEDATE)]
		public string ResponseDate { get; set; }
		/// <summary>取引ID</summary>
		[DataMember(Name = PayTgConstants.PARAM_ORDERID)]
		public string OrderId { get; set; }
		/// <summary>顧客ID</summary>
		[DataMember(Name = PayTgConstants.PARAM_CUSTOMERID)]
		public string CustomerId { get; set; }
		/// <summary>カード番号</summary>
		[DataMember(Name = PayTgConstants.PARAM_CARD_NUMBER)]
		public string CardNumber { get; set; }
		/// <summary>カード有効期限</summary>
		[DataMember(Name = PayTgConstants.PARAM_CARD_EXPIRE)]
		public string CardExpire { get; set; }
	}
}
