/*
=========================================================================================================
  Module      : カート例外クラス(CartException.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace w2.App.Common.Order
{
	///*********************************************************************************************
	/// <summary>
	/// カート例外クラス
	/// </summary>
	///*********************************************************************************************
	public class CartException : ApplicationException
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strMessage"></param>
		public CartException(string strMessage)
			: base(strMessage)
		{
		}
	}
}
