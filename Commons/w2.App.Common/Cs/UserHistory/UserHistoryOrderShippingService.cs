/*
=========================================================================================================
  Module      : ユーザー注文履歴配送先サービスクラス(UserHistoryOrderShipping.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Util;
using w2.Common.Extensions;

namespace w2.App.Common.Cs.UserHistory
{
	/// <summary>
	/// ユーザー注文履歴配送先サービスクラス
	/// </summary>
	[Serializable]
	public class UserHistoryOrderShipping : ModelBase<UserHistoryOrderShipping>
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		private UserHistoryOrderShipping()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="info">情報</param>
		public UserHistoryOrderShipping(DataRowView info)
			: this(info.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="info">情報</param>
		public UserHistoryOrderShipping(Hashtable info)
			: this()
		{
			this.DataSource = info;
		}

		/// <summary>配送先枝番</summary>
		public string OrderShippingNo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO]); }
		}
		/// <summary>出荷予定日</summary>
		public DateTime? OrderShippingScheduleShippingDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE];
			}
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE] = value; }
		}
	}
}