/*
=========================================================================================================
  Module      : 注文同梱設定インターフェース(IOrderCombineSettings.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace w2.Domain.Order.Helper
{
	/// <summary>
	/// 注文同梱設定インターフェース
	/// </summary>
	public interface IOrderCombineSettings
	{
		/// <summary>注文同梱許可 決済種別</summary>
		string[] AllowPaymentKbn { get; } 
		/// <summary>注文同梱許可 注文ステータス</summary>
		string[] AllowOrderStatus { get; }
		/// <summary>注文同梱許可 注文決済ステータス</summary>
		string[] AllowOrderPaymentStatus { get; }
		/// <summary>注文同梱許可 注文日からの経過日数</summary>
		int AllowOrderDayPassed { get; }
		/// <summary>注文同梱許可 配送希望日までの日数</summary>
		int AllowShippingDayBefore { get; }
		/// <summary>注文同梱許可しない 配送種別</summary>
		string[] DenyShippingIds { get; }
		/// <summary>注文同梱許可しない 配送方法</summary>
		string[] DenyShippingMethods { get; }
	}
}
