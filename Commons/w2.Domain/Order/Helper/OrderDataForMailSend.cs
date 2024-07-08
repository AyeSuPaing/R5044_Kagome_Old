/*
=========================================================================================================
  Module      : メールテンプレートでの注文情報出力のためのヘルパクラス (OrderDataForSendMail.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.Common.Extensions;

namespace w2.Domain.Order.Helper
{
	/// <summary>
	/// メールテンプレート用注文情報クラス
	/// </summary>
	public class OrderDataForMailSend : ModelBase<OrderDataForMailSend>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OrderDataForMailSend()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderDataForMailSend(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderDataForMailSend(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		#endregion
	}
}
