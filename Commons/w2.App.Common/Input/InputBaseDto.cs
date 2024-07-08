/*
=========================================================================================================
  Module      : Dto用入力基底クラス(InputBaseDto.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;

namespace w2.App.Common.Input
{
	/// <summary>
	/// Dto用入力基底クラス
	/// </summary>
	[Serializable]
	public abstract class InputBaseDto : IInput
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		protected InputBaseDto()
		{
			this.DataSource = new Hashtable();
		}
		#endregion

		#region プロパティ
		/// <summary>ソース</summary>
		public Hashtable DataSource { get; set; }
		#endregion
	}
}