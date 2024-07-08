/*
=========================================================================================================
  Module      : Real Shop Area (RealShopArea.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.RealShop
{
	/// <summary>
	/// Real shop area
	/// </summary>
	[Serializable]
	public class RealShopArea
	{
		/// <summary>Area id</summary>
		public string AreaId { get; set; }
		/// <summary>Area name</summary>
		public string AreaName { get; set; }
	}
}
