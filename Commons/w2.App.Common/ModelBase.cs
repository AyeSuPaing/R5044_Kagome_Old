/*
=========================================================================================================
  Module      : モデル基底クラス(ModelBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace w2.App.Common
{
	/// <summary>
	/// モデル抽象クラス
	/// </summary>
	[Serializable]
	public abstract class ModelBase<TSelf> : ICloneable
	{
		#region プロパティ
		/// <summary>ソース</summary>
		public Hashtable DataSource { get; set; }
		#endregion

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		protected ModelBase()
		{
			this.DataSource = new Hashtable();
		}
		#endregion

		#region +Clone 複製
		/// <summary>
		/// 複製
		/// </summary>
		/// <returns>複製されたオブジェクト</returns>
		public object Clone()
		{
			Hashtable clonedHashtable = (Hashtable)this.DataSource.Clone();

			// コンストラクタの情報を取得
			Type type = typeof(TSelf);
			ConstructorInfo constructor = type.GetConstructor(new Type[] { typeof(Hashtable) });

			if (constructor == null)
				throw new NotSupportedException("コンストラクタが定義されていません。");

			// リフレクションでインスタンスを生成
			return (TSelf)constructor.Invoke(new object[] { clonedHashtable });
		}
		#endregion

		#region +DeepClone ディープクローン
		/// <summary>
		/// ディープクローン
		/// </summary>
		/// <typeparam name="T">対象オブジェクトタイプ</typeparam>
		/// <param name="obj">対象オブジェクト</param>
		/// <returns>コピーしたオブジェクト</returns>
		public static TSelf DeepClone(TSelf obj)
		{
			using (var ms = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(ms, obj);
				ms.Position = 0;
				var result = (TSelf)formatter.Deserialize(ms);
				return result;
			}
		}
		#endregion

		#region +Tostring 文字列化
		/// <summary>
		/// 文字列化
		/// </summary>
		/// <returns>検証用に利用する、各プロパティを文字列化して結合したもの</returns>
		public string Tostring()
		{
			List<object> list = this.DataSource.Keys.Cast<object>().ToList();

			return list.OrderBy((item) => item.ToString()).Aggregate("", (current, Key) => current + this.DataSource[Key].ToString());

		}
		#endregion
	}
}
