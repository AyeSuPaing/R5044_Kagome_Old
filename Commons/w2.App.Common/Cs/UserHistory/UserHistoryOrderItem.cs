/*
=========================================================================================================
  Module      : ユーザー履歴（注文アイテム）クラス(UserHistoryOrderItem.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using w2.Common.Util;
using w2.Common.Extensions;

namespace w2.App.Common.Cs.UserHistory
{
	/// <summary>
	/// ユーザー履歴（注文アイテム）クラス
	/// </summary>
	[Serializable]
	public class UserHistoryOrderItem : ModelBase<UserHistoryOrderItem>
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		private UserHistoryOrderItem()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="info">情報</param>
		public UserHistoryOrderItem(DataRowView info)
			: this(info.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="info">情報</param>
		public UserHistoryOrderItem(Hashtable info)
			: this()
		{
			this.DataSource = info;
		}

		/// <summary>注文ID</summary>
		public string OrderId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_ORDER_ORDER_ID]); }
		}
		/// <summary>アイテム商品ID</summary>
		public string ItemProductId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_ID]); }
		}
		/// <summary>アイテム商品名</summary>
		public string ItemProductName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_NAME]); }
		}
		/// <summary>アイテム注文数</summary>
		public int ItemQuantity
		{
			get { return (int)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_QUANTITY]; }
		}
	}
}