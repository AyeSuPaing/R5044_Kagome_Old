/*
=========================================================================================================
  Module      : Cross Point API Update Point Response (UpdatePointResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.CrossPoint.Point
{
	/// <summary>
	/// Update point response
	/// </summary>
	[XmlRoot("UpdPoint")]
	public class UpdatePointResponse : ResponseBase<PointApiResult>
	{
	}
}
