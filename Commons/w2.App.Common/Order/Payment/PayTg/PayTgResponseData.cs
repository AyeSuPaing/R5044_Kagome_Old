/*
=========================================================================================================
  Module      : PayTgのAPI受信データ(ResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System.Runtime.Serialization;

namespace w2.App.Common.Order.Payment.PayTg
{
	/// <summary>
	/// PayTgのAPI受信データ
	/// </summary>
	[DataContract]
	public class ResponseData
	{
		/// <summary>PayTg端末連動結果</summary>
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
		/// <summary>決済 GW エラーコード</summary>
		[DataMember(Name = PayTgConstants.PARAM_GWERRORCODE)]
		public string GwErrCd { get; set; }
		/// <summary>決済 GW エラーメッセージ</summary>
		[DataMember(Name = PayTgConstants.PARAM_GWERRORMSG)]
		public string GwErrMsg { get; set; }
		/// <summary>電文受信日時</summary>
		[DataMember(Name = PayTgConstants.PARAM_REQUESTDATE)]
		public string RequestDate { get; set; }
		/// <summary>電文送信日時</summary>
		[DataMember(Name = PayTgConstants.PARAM_RESPONSEDATE)]
		public string ResponseDate { get; set; }
		/// <summary>処理時間（トークン化）</summary>
		[DataMember(Name = PayTgConstants.PARAM_ORDERDATE)]
		public string OrderDate { get; set; }
		/// <summary>デジタル署名</summary>
		[DataMember(Name = PayTgConstants.PARAM_LINE1)]
		public string Line1 { get; set; }
		/// <summary>キーバージョン</summary>
		[DataMember(Name = PayTgConstants.PARAM_MCSECCD)]
		public string McSecCd { get; set; }
		/// <summary>カードトークン</summary>
		[DataMember(Name = PayTgConstants.PARAM_TOKEN)]
		public string Token { get; set; }
		/// <summary>発行者識別番号</summary>
		[DataMember(Name = PayTgConstants.PARAM_TOP6)]
		public string Top6 { get; set; }
		/// <summary>カード番号下4桁</summary>
		[DataMember(Name = PayTgConstants.PARAM_LAST4)]
		public string Last4 { get; set; }
		/// <summary>有効期限月</summary>
		[DataMember(Name = PayTgConstants.PARAM_MCACNTNO1)]
		public string McAcntNo1 { get; set; }
		/// <summary>有効期限年</summary>
		[DataMember(Name = PayTgConstants.PARAM_EXPIRE)]
		public string Expire { get; set; }
		/// <summary>国際ブランド</summary>
		[DataMember(Name = PayTgConstants.PARAM_BRAND)]
		public string Brand { get; set; }
		/// <summary>イシュアコード</summary>
		[DataMember(Name = PayTgConstants.PARAM_ISSUERNAME)]
		public string IssurName { get; set; }
		/// <summary>カードタイプ</summary>
		[DataMember(Name = PayTgConstants.PARAM_KANACARDNAME)]
		public string KanaCardName { get; set; }
		/// <summary>セキュリティコードトークン</summary>
		[DataMember(Name = PayTgConstants.PARAM_PROCESSPASS)]
		public string ProcessPass { get; set; }
		/// <summary>サービスID</summary>
		[DataMember(Name = PayTgConstants.PARAM_PROCESSID)]
		public string ProcessId { get; set; }
		/// <summary>顧客ID</summary>
		[DataMember(Name = PayTgConstants.PARAM_CUSTOMERID)]
		public string CustomerId { get; set; }
		/// <summary>取引ID</summary>
		[DataMember(Name = PayTgConstants.PARAM_ORDERID)]
		public string OrderId { get; set; }
		/// <summary>会社コード</summary>
		[DataMember(Name = PayTgConstants.PARAM_ACQNAME)]
		public string AcqName { get; set; }
		/// <summary>リクエストID</summary>
		[DataMember(Name = PayTgConstants.PARAM_RESAUTHCODE)]
		public string ResAuthCode { get; set; }
		/// <summary>処理時間（決済リクエスト）</summary>
		[DataMember(Name = PayTgConstants.PARAM_TRANDATE)]
		public string TranDate { get; set; }
		/// <summary>リファレンス</summary>
		[DataMember(Name = PayTgConstants.PARAM_LINE2)]
		public string Line2 { get; set; }
		/// <summary>カード分割払い</summary>
		[DataMember(Name = PayTgConstants.PARAM_PAYTIMES)]
		public string PayTimes { get; set; }
		/// <summary>ボーナス払い</summary>
		[DataMember(Name = PayTgConstants.PARAM_NAMEKANJI)]
		public string NameKanji { get; set; }
		/// <summary>リボ払い</summary>
		[DataMember(Name = PayTgConstants.PARAM_NAMEKANA)]
		public string NameKana { get; set; }
		/// <summary>楽天カード判定</summary>
		[DataMember(Name = PayTgConstants.PARAM_FORWARD)]
		public string Forward { get; set; }
	}
}
