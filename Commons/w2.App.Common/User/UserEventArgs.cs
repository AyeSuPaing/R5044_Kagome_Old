/*
=========================================================================================================
  Module      : ユーザイベント引数クラス(UserEventArgs.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Domain.User;

namespace w2.App.Common.User
{
	public class UserEventArgs : EventArgs
	{
		/// <summary>
		/// ユーザーイベント引数
		/// </summary>
		/// <param name="user">ユーザ連携情報</param>
		public UserEventArgs(UserModel user)
		{
			this.UserData = user;
		}

		/// <summary>ユーザ連携情報</summary>
		public UserModel UserData { get; private set; }
	}
}
