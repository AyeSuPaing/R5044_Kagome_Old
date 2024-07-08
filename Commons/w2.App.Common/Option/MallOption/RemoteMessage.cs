/*
=========================================================================================================
  Module      : リモートメッセージモジュール(RemoteMessage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace w2.App.Common.Option
{
	///*********************************************************************************************
	/// <summary>
	/// リモートメッセージクラス
	/// </summary>
	///*********************************************************************************************
	public class RemoteMessage : MarshalByRefObject
	{
		public delegate void CallHandler(string str);
		public event CallHandler eventCall;

		/// <summary>
		/// イベントコール
		/// </summary>
		/// <param name="str"></param>
		public void Call(string str)
		{
			eventCall(str);
		}
	}
}