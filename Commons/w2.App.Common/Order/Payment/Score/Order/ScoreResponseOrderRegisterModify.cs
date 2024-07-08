/*
=========================================================================================================
  Module      : スコア後払い取引登録のレスポンス値(ScoreResponseOrderRegisterModify.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.Score.Order
{
	/// <summary>
	/// 取引登録レスポンス値
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public class ScoreResponseOrderRegisterModify : BaseScoreResponse
	{
	}
}
