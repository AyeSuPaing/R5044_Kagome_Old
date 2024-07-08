/*
=========================================================================================================
  Module      : スコア後払い与信結果取得のレスポンス値(ScoreGetAuthResultResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.Score.GetAuth
{
	/// <summary>
	/// 与信結果取得のレスポンス値
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public class ScoreGetAuthResultResponse : BaseScoreResponse
	{
	}
}
