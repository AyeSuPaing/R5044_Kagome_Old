/*
=========================================================================================================
  Module      : LINEミニアプリ基底ユーザーコントロール(LinieMiniAppControl.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Web.Process;
using w2.Domain.LineTemporaryUser;

/// <summary>
/// LINEミニアプリ基底ユーザーコントロール
/// </summary>
public class LineMiniAppControl : BaseUserControl
{
	/// <summary>ミニアプリOPが有効か</summary>
	protected bool IsLineMiniAppOptionEnabled
	{
		get { return w2.App.Common.Line.Constants.LINE_MINIAPP_OPTION_ENABLED; }
	}
	/// <summary>LINE LIFFアプリID</summary>
	protected string LineLiffId
	{
		get { return w2.App.Common.Line.Constants.LINE_MINIAPP_LIFF_ID; }
	}
	/// <summary>LINE チャネルID</summary>
	protected string LineClientId
	{
		get { return w2.App.Common.Line.Constants.LINE_DIRECT_CONNECT_CLIENT_ID; }
	}
	/// <summary>仮ユーザーログインか</summary>
	protected bool IsTempLoggedIn
	{
		get { return this.Process.IsTempLoggedIn; }
	}
	/// <summary>ログインLINEユーザーID</summary>
	protected string LoginLineUserId
	{
		get { return this.Process.LoginLineUserId; }
	}
	/// <summary>ログイン中のLINE仮会員ユーザー情報</summary>
	protected LineTemporaryUserModel LineTempUser
	{
		get { return this.Process.LineTempUser; }
		set { this.Process.LineTempUser = value; }
	}
	/// <summary>仮ユーザーIDを保持しているか</summary>
	protected bool HasTemporaryUserId
	{
		get { return SessionManager.HasTemporaryUserId; }
	}
	/// <summary>プロセス</summary>
	protected new LineMiniAppProcess Process
	{
		get { return (LineMiniAppProcess)this.ProcessTemp; }
	}
	/// <summary>プロセステンポラリ</summary>
	protected override IPageProcess ProcessTemp
	{
		get
		{
			if (m_processTmp == null) m_processTmp = new LineMiniAppProcess(this, this.ViewState, this.Context);
			return m_processTmp;
		}
	}
}