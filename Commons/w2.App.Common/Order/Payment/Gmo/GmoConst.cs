/*
=========================================================================================================
  Module      : GMO定義(GmoConst.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.GMO
{
	#region 列挙体

	/// <summary>処理結果</summary>
	public enum ResultCode
	{
		/// <summary>成功</summary>
		[XmlEnum(Name = "OK")]
		OK,
		/// <summary>失敗</summary>
		[XmlEnum(Name = "NG")]
		NG
	}

	/// <summary>取引更新種別</summary>
	public enum UpdateKindType
	{
		/// <summary>取引修正</summary>
		[XmlEnum(Name = "1")]
		OrderModify,
		/// <summary>キャンセル</summary>
		[XmlEnum(Name = "2")]
		OrderCancel
	}

	/// <summary>更新種別情報</summary>
	public enum KindInfoType
	{
		/// <summary>出荷報告修正</summary>
		[XmlEnum(Name = "1")]
		ShipmentModify,
		/// <summary>出荷報告キャンセル</summary>
		[XmlEnum(Name = "2")]
		ShipmentCancel
	}

	/// <summary>決済種別</summary>
	public enum GmoPaymentType
	{
		/// <summary>別送サービス</summary>
		[XmlEnum(Name = "2")]
		SeparateService,
		/// <summary>同梱サービス</summary>
		[XmlEnum(Name = "3")]
		IncludeService
	}

	/// <summary>入金状態コード</summary>
	public enum PaymentStatusCode
	{
		/// <summary>未入金</summary>
		[XmlEnum(Name = "0")]
		NOT,
		/// <summary>入金済み(速報)</summary>
		[XmlEnum(Name = "1")]
		PROMPT,
		/// <summary>入金済み（確報）</summary>
		[XmlEnum(Name = "2")]
		DEFINITE,
		/// <summary>イレギュラー</summary>
		[XmlEnum(Name = "3")]
		IRREGULAR
	}

	/// <summary>Destination Kind Type</summary>
	public enum DestinationKindType
	{
		/// <summary>Current billing address</summary>
		[XmlEnum(Name = "1")]
		ALSO,
		/// <summary>New billing address</summary>
		[XmlEnum(Name = "2")]
		NEW
	}

	/// <summary>Reissue Requested Reason Code</summary>
	public enum ReissueRequestedReasonCode
	{
		/// <summary>Lost invoice</summary>
		[XmlEnum(Name = "01")]
		LOST,
		/// <summary>Request not reached</summary>
		[XmlEnum(Name = "02")]
		NOT_RECEIVED,
		/// <summary>Moving</summary>
		[XmlEnum(Name = "03")]
		MOVING,
		/// <summary>Other</summary>
		[XmlEnum(Name = "04")]
		OTHER
	}
	#endregion
}
