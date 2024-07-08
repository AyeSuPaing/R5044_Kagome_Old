/*
=========================================================================================================
  Module      : 郵便番号のヘルパクラス (ZipcodeHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.Domain.Zipcode;

namespace w2.Domain.Zipcode.Helper
{
	/// <summary>
	/// 郵便番号のヘルパクラス
	/// </summary>
	internal class ZipcodeHelper
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ZipcodeHelper()
		{
		}

		/// <summary>
		/// 住所の不要部分を削除する
		/// </summary>
		/// <param name="model">住所モデル</param>
		/// <returns>不要部分削除後の住所モデル</returns>
		public static ZipcodeModel RemoveInvalidAddressPart(ZipcodeModel model)
		{
			model.Town = model.Town.Replace("（次のビルを除く）", "").Replace("以下に掲載がない場合", "");
			return model;
		}
	}
}
