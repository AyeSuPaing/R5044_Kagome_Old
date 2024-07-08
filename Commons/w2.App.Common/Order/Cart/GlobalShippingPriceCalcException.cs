/*
=========================================================================================================
  Module      : 海外送料計算Exception(GlobalShippingPriceCalcException.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace w2.App.Common.Order.Cart
{
	/// <summary>
	/// 海外送料計算Exception
	/// </summary>
	public class GlobalShippingPriceCalcException : Exception
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GlobalShippingPriceCalcException() : base() { }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="msg">エラーメッセージ</param>
		public GlobalShippingPriceCalcException(string msg) : base(msg) { }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="msg">エラーメッセージ</param>
		/// <param name="innerException">内部例外</param>
		public GlobalShippingPriceCalcException(string msg, Exception innerException) : base(msg, innerException) { }
		/// <summary>
		/// フロント表示用のエラーメッセージ
		/// </summary>
		public string FrontErrorMessage { get; set; }
		/// <summary>
		/// 管理画面表示用のエラーメッセージ
		/// </summary>
		public string ManagerErrorMessage { get; set; }
	}
}
