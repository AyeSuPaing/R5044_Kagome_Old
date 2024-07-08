/*
=========================================================================================================
  Module      : 商品情報設定例外エラークラス(ProductSettingException.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Commerce.MallBatch.Mkadv.Common
{
	///**************************************************************************************
	/// <summary>
	/// 商品情報設例外エラークラス
	/// </summary>
	///**************************************************************************************
	public class ProductSettingException : Exception
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="message">メッセージ</param>
		public ProductSettingException(string Message) : base(Message) 
		{
			// 基底クラスのコンストラクタをコールするのみ
		}
	}
}
