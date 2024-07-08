/*
=========================================================================================================
  Module      : ScoringSaleProductモデル (ScoringSaleProductModel_EX.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.ScoringSale
{
	/// <summary>
	/// ScoringSaleProductモデル
	/// </summary>
	public partial class ScoringSaleProductModel
	{
		#region プロパティ
		/// <summary>Scoring sale result condition</summary>
		public ScoringSaleResultConditionModel[] ScoringSaleResultConditions { get; set; }
		/// <summary>Products recommend</summary>
		public string ProductImage { get; set; }
		/// <summary>Product name</summary>
		public string ProductName { get; set; }
		/// <summary>Fixed purchase flag</summary>
		public bool FixedPurchaseFlg { get; set; }
		#endregion
	}
}
