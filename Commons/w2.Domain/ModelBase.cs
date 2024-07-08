/*
=========================================================================================================
  Module      : モデル基底クラス(ModelBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using w2.Domain.Helper.Attribute;
using w2.Domain.UpdateHistory.Helper.UpdateData;

namespace w2.Domain
{
	/// <summary>
	/// モデル抽象クラス
	/// </summary>
	[Serializable]
	public abstract class ModelBase<TSelf> : IModel
		where TSelf : ModelBase<TSelf>
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
		public TSelf Clone()
		{
			var clonedHashtable = (Hashtable)this.DataSource.Clone();

			// コンストラクタの情報を取得
			var type = typeof(TSelf);
			var constructor = type.GetConstructor(new [] { typeof(Hashtable) });

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

		#region +ToString 文字列化
		/// <summary>
		/// 文字列化
		/// </summary>
		/// <returns>検証用に利用する、各プロパティを文字列化して結合したもの</returns>
		public new string ToString()
		{
			var list = this.DataSource.Keys.Cast<object>().ToList();
			return list.OrderBy((item) => item.ToString()).Aggregate("", (current, Key) => current + this.DataSource[Key].ToString());
		}
		#endregion

		/// <summary>
		/// 更新データキー値リスト作成
		/// </summary>
		/// <returns>更新データキー値リスト</returns>
		public UpdateDataKeyValue[] CreateUpdateDataList()
		{
			return GetType().GetProperties()
				.Where(p => Attribute.GetCustomAttribute(p, typeof(UpdateDataAttribute)) != null)
				.OrderBy(p => ((UpdateDataAttribute)Attribute.GetCustomAttribute(p, typeof(UpdateDataAttribute))).No)
				.Select(p =>
					new UpdateDataKeyValue
						(
							((UpdateDataAttribute)Attribute.GetCustomAttribute(p, typeof(UpdateDataAttribute))).Key,
							p.GetValue(this, null)
						)
					).ToArray();
		}

		/// <summary>
		/// To view model
		/// </summary>
		/// <typeparam name="TViewModel">Generic</typeparam>
		/// <returns>View model</returns>
		public TViewModel ToViewModel<TViewModel>()
		{
			var serialize = JsonConvert.SerializeObject(this);
			var viewModel = JsonConvert.DeserializeObject<TViewModel>(serialize);
			return viewModel;
		}
	}
}
