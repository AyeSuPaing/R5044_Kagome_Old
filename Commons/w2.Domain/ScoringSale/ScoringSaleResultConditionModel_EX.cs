/*
=========================================================================================================
  Module      : ScoringSaleResultConditionモデル (ScoringSaleResultConditionModel_EX.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.ScoringSale
{
	/// <summary>
	/// ScoringSaleResultConditionモデル
	/// </summary>
	public partial class ScoringSaleResultConditionModel
	{
		#region プロパティ
		/// <summary>Is group</summary>
		public bool IsGroup { get; set; }
		/// <summary>Group brand no</summary>
		public int GroupBrandNo { get; set; }
		#endregion
	}
}
