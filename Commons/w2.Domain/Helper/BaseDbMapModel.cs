/*
=========================================================================================================
  Module      : Dbマップ基底クラス (BaseDbMapModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using System.Linq;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.Helper
{
	/// <summary>
	/// 検索条件の基底クラス
	/// </summary>
	public abstract class BaseDbMapModel
	{
		/* 
		 * ↓ Please implementation ↓ 
		 * Property to be a condition
		 * 
		 */

		/// <summary>
		/// パラメタハッシュテーブル生成
		/// </summary>
		/// <returns></returns>
		public Hashtable CreateHashtableParams()
		{
			// Dbマップ属性を利用してプロパティを元に検索条件用のHashTableを生成
			// ここで作成したHashTableが、SqlStatement経由でクエリのバインド変数に割当たる。
			var properties = this.GetType().GetProperties();
			var propertiesTmp = properties.Where(p => System.Attribute.GetCustomAttribute(p, typeof(DbMapName)) != null)
				.ToArray();
			var dic = propertiesTmp.ToDictionary(
				p => ((DbMapName)System.Attribute.GetCustomAttribute(p, typeof(DbMapName))).MapName,
				p => p.GetValue(this, null));
			return new Hashtable(dic);
		}
	}
}