/*
=========================================================================================================
  Module      : ノベルティ付与条件モデル (NoveltyGrantConditionsModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.Novelty
{
	/// <summary>
	/// ノベルティ付与条件モデル
	/// </summary>
	public partial class NoveltyGrantConditionsModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>
		/// ノベルティ付与アイテム
		/// </summary>
		public NoveltyGrantItemModel[] GrantItemList
		{
			get { return (NoveltyGrantItemModel[])this.DataSource["GrantItemList"]; }
			set { this.DataSource["GrantItemList"] = value; }
		}
		#endregion
	}
}
