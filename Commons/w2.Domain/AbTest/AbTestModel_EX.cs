/*
=========================================================================================================
  Module      : ABテストモデル (AbTestModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.AbTest
{
	/// <summary>
	/// ABテストモデル
	/// </summary>
	public partial class AbTestModel
	{
		#region プロパティ
		/// <summary>アイテムリスト</summary>
		public AbTestItemModel[] Items { get; set; }
		#endregion
	}
}