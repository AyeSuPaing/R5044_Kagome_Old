/*
=========================================================================================================
  Module      : Cross Point API Point Change Reason Setting Response (PointChangeReasonSettingResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.CrossPoint.Point
{
	/// <summary>
	/// Cross point api point change reason setting response
	/// </summary>
	[XmlRoot("GetPointChangeReasonSetting")]
	public class PointChangeReasonSettingResponse : ResponseBase<PointApiResult>
	{
	}
}
