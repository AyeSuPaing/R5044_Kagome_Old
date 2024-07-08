/*
=========================================================================================================
  Module      : 必須チェック基底クラス(NecessaryCheckBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace w2.App.Common.Order.Import.OrderImport.NecessaryCheck
{
	/// <summary>
	/// 必須チェック基底
	/// </summary>
	public abstract class NecessaryCheckBase
	{
		/// <summary>必須項目リスト</summary>
		protected List<string> m_FieldsNecessary = new List<string>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public NecessaryCheckBase()
		{
			// リスト作成
			CreateListNecessary();
		}

		/// <summary>
		/// 必須項目リスト作成
		/// </summary>
		internal abstract void CreateListNecessary();

		/// <summary>
		/// 必須チェック
		/// </summary>
		/// <param name="items">項目リスト</param>
		/// <returns>必須項目が全部あるかどうか</returns>
		public bool Check(string[] items)
		{
			var notFound = this.m_FieldsNecessary.Where(field => items.Contains(field) == false).ToArray();

			this.NotFoundField = string.Join(",", notFound);
			return (notFound.Length == 0);
		}

		/// <summary>足りない項目</summary>
		public string NotFoundField { get; set; }
	}
}
