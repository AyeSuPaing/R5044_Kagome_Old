/*
=========================================================================================================
  Module      : スコア後払い発送情報登録レスポンス値(ScoreDeliveryRegisterResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.Score.Delivery
{
	/// <summary>
	/// 発送情報登録レスポンス
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public class ScoreDeliveryRegisterResponse : BaseScoreResponse
	{
	}
}
