/*
=========================================================================================================
  Module      : スコア後払いキャンセルレスポンス値(ScoreCancelResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;
using w2.App.Common.Order.Payment.Score.ObjectsElement;

namespace w2.App.Common.Order.Payment.Score.Cancel
{
	/// <summary>
	/// キャンセルレスポンス値
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public class ScoreCancelResponse : BaseScoreResponse
	{
		/// <summary>取引情報</summary>
		[XmlElement("transactionInfo")]
		public TransactionElement TransactionCancelResult { get; set; }
	}
}
