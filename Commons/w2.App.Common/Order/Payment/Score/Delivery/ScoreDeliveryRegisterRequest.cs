/*
=========================================================================================================
  Module      : スコア後払い発送情報登録リクエスト値(ScoreDeliveryRegisterRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;
using w2.App.Common.Order.Payment.Score.ObjectsElement;

namespace w2.App.Common.Order.Payment.Score.Delivery
{
	/// <summary>
	/// 発送情報登録リクエスト
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false, Namespace = "")]
	public class ScoreDeliveryRegisterRequest : BaseScoreRequest
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ScoreDeliveryRegisterRequest()
		{
			this.Transaction = new TransactionElement();
			this.PdRequest = new PdRequestElement();
		}

		/// <summary>取引情報</summary>
		[XmlElement("transaction")]
		public TransactionElement Transaction { get; set; }
		// <summary>発送情報</summary>
		[XmlElement("PdRequest")]
		public PdRequestElement PdRequest { get; set; }
	}
}
