/*
=========================================================================================================
  Module      : モデルインターフェース(IModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;

namespace w2.Domain
{
	/// <summary>
	/// モデルインターフェース
	/// </summary>
	public interface IModel
	{
		#region プロパティ
		/// <summary>ソース</summary>
		Hashtable DataSource { get; set; }
		#endregion
	}
}
