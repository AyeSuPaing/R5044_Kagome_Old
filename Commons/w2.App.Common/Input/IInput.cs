/*
=========================================================================================================
  Module      : 入力インターフェース(IInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;

namespace w2.App.Common.Input
{
	/// <summary>
	/// 入力インターフェース
	/// </summary>
	public interface IInput
	{
		/// <summary>データソース</summary>
		Hashtable DataSource { get; set; }
	}
}