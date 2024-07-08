/*
=========================================================================================================
  Module      : メールテンプレート用のシリアルキー付き注文情報のヘルパクラス (OrderSerialKeyForOrderMail.cs)
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
	/// メールテンプレート用シリアルキー付き注文情報クラス
	/// </summary>
	public class OrderSerialKeyForMailSend : ModelBase<OrderSerialKeyForMailSend>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OrderSerialKeyForMailSend()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderSerialKeyForMailSend(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderSerialKeyForMailSend(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		#endregion
	}
}
