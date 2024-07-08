/*
=========================================================================================================
  Module      : 更新履歴データ用属性クラス (UpdateDataAttribute.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace w2.Domain.Helper.Attribute
{
	/// <summary>
	/// 更新履歴データ属性クラス
	/// </summary>
	public class UpdateDataAttribute : System.Attribute
	{
		/// <summary>No</summary>
		public int No { private set; get; }
		/// <summary>キー</summary>
		public string Key { private set; get; }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="no">No</param>
		/// <param name="key">キー</param>
		public UpdateDataAttribute(int no, string key)
		{
			this.No = no;
			this.Key = key;
		}
	}
}
