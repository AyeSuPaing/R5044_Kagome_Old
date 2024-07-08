/*
=========================================================================================================
  Module      : ターゲットリスト条件リストクラス(TargetListConditionList.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;

namespace w2.App.Common.TargetList
{
	/// <summary>
	/// ターゲットリスト条件リストクラス
	/// </summary>
	[Serializable]
	public class TargetListConditionList
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public TargetListConditionList()
		{
			this.TargetConditionList = new List<ITargetListCondition>();
		}

		/// <summary>条件格納用リストプロパティ</summary>
		public List<ITargetListCondition> TargetConditionList { set; get; }
	}
}
