/*
=========================================================================================================
  Module      : 注文例外クラス(OrderException.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.App.Common.Order
{
	///*********************************************************************************************
	/// <summary>
	/// 注文例外クラス
	/// </summary>
	///*********************************************************************************************
	public class OrderException : Exception
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OrderException()
			: base()
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strErrorMessage">エラーメッセージ</param>
		public OrderException(string strErrorMessage)
			: base(strErrorMessage)
		{
		}
	}
}
