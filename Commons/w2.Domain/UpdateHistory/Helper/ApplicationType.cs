/*
=========================================================================================================
  Module      : 更新アプリケーション種別 (ApplicationType.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace w2.Domain.UpdateHistory.Helper
{
	/// <summary>更新アプリケーション種別</summary>
	public enum ApplicationType
	{
		/// <summary>フロント</summary>
		Front,
		/// <summary>管理</summary>
		Manager,
		/// <summary>バッチ</summary>
		Batch,
		/// <summary>未定義</summary>
		Undefined,
	}
}
