/*
=========================================================================================================
  Module      : 入力基底クラス(InputBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using w2.Domain;

namespace w2.App.Common.Input
{
	/// <summary>
	/// 入力基底クラス
	/// </summary>
	[Serializable]
	public abstract class InputBase<T> : IInput
		where T : IModel
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		protected InputBase()
		{
			this.DataSource = new Hashtable();
		}
		#endregion

		#region プロパティ
		/// <summary>ソース</summary>
		public Hashtable DataSource { get; set; }
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public abstract T CreateModel();
		#endregion
	}
}