/*
=========================================================================================================
  Module      : 更新データインターフェイス (IUpdateData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.Domain.UpdateHistory.Helper.UpdateData
{
	/// <summary>
	/// 更新データインターフェイス
	/// </summary>
	public interface IUpdateData
	{
		/// <summary>
		/// 更新データXML変換（モデル⇒バイナリ）
		/// </summary>
		/// <returns>シリアライズ化された更新履歴、ハッシュ文字列</returns>
		Tuple<byte[], string> SerializeAndCreateHashString();
	}
}