/*
=========================================================================================================
  Module      : ユーザー連携プラグインクラス(UserCooperationPlugin.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using w2.App.Common.Order;
using w2.Domain.User;

namespace w2.App.Common.User
{
	/// <summary>
	/// イベントハンドラ
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void UserEventHandler(object sender, UserEventArgs e);

	/// <summary>
	/// ユーザー連携プラグインクラス
	/// </summary>
	public class UserCooperationPlugin
	{
		#region イベント

		/// <summary>登録処理イベント</summary>
		public event UserEventHandler RegisterEvent;
		/// <summary>更新処理イベント</summary>
		public event UserEventHandler ModifyEvent;
		/// <summary>退会処理イベント</summary>
		public event UserEventHandler WithdrawalEvent;
		/// <summary>ログインイベント</summary>
		public event UserEventHandler LoggedInEvent;

		protected virtual void OnRegistEvent(object sender, UserEventArgs e)
		{
			if (RegisterEvent != null)
			{
				RegisterEvent(this, e);
			}
		}

		protected virtual void OnModifyEvent(object sender, UserEventArgs e)
		{
			if (ModifyEvent != null)
			{
				ModifyEvent(this, e);
			}
		}

		protected virtual void OnWithdrawalEvent(object sender, UserEventArgs e)
		{
			if (WithdrawalEvent != null)
			{
				WithdrawalEvent(this, e);
			}
		}

		protected virtual void OnLoggedInEvent(object sender, UserEventArgs e)
		{
			if (LoggedInEvent != null)
			{
				LoggedInEvent(this, e);
			}
		}
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="lastChanged">最終更新者</param>
		public UserCooperationPlugin(string lastChanged)
		{
			this.EventBinder = new UserEventBinder(lastChanged);

			// イベントハンドラ設定
			this.RegisterEvent += this.EventBinder.OnRegistered;
			this.ModifyEvent += this.EventBinder.OnModified;
			this.WithdrawalEvent += this.EventBinder.OnWithdrawed;
			this.LoggedInEvent += this.EventBinder.OnLoggedIn;
		}

		/// <summary>
		/// ユーザー登録イベント
		/// </summary>
		/// <param name="user">ユーザー情報</param>
		public void Regist(UserModel user)
		{
			UserEventArgs e = new UserEventArgs(user);
			OnRegistEvent(this, e);
		}

		/// <summary>
		///  ユーザー情報の編集イベント
		/// </summary>
		/// <param name="user">ユーザー情報</param>
		public void Update(UserModel user)
		{
			UserEventArgs e = new UserEventArgs(user);
			OnModifyEvent(this, e);
		}

		/// <summary>
		///  退会イベント
		/// </summary>
		/// <param name="user">ユーザー</param>
		public void Withdrawal(UserModel user)
		{
			UserEventArgs e = new UserEventArgs(user);
			OnWithdrawalEvent(this, e);
		}

		/// <summary>
		/// ログインイベント
		/// </summary>
		/// <param name="user">ユーザー情報</param>
		public void Login(UserModel user)
		{
			UserEventArgs e = new UserEventArgs(user);
			OnLoggedInEvent(this, e);
		}
		
		/// <summary>ユーザ処理系イベントバインダ（テストコード用に公開しているだけです。使用禁止）</summary>
		public UserEventBinder EventBinder { get; set; } //HACK: テストコード用に公開している。要検討
	}
}
