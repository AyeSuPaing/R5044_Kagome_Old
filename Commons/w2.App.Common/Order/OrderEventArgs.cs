/*
=========================================================================================================
  Module      : 注文イベント引数クラス(OrderEventArgs.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.App.Common.Order
{
	public class OrderEventArgs : EventArgs
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="orderData">注文情報</param>
		public OrderEventArgs(Hashtable orderData)
		{
			this.ReturnMessage = "";
			this.IsSuccess = true;
			this.OrderData = orderData;
		}

		/// <summary>注文情報</summary>
		public Hashtable OrderData { get; set; }
		/// <summary>成功フラグ（デフォルトtrue）</summary>
		public bool IsSuccess { get; set; }
		/// <summary>イベント先からのメッセージ</summary>
		public string ReturnMessage { get; set; }
	}
}
