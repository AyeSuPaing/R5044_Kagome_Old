/*
=========================================================================================================
  Module      : LINEミニアプリプロセス(LineMiniAppProcess.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Web;
using System.Web.UI;
using w2.Domain.LineTemporaryUser;
using w2.Domain.User;

/// <summary>
/// LINEミニアプリプロセス
/// </summary>
public class LineMiniAppProcess : CrossPointProcess
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="caller">呼び出し元</param>
	/// <param name="viewState">ビューステート</param>
	/// <param name="context">コンテキスト</param>
	public LineMiniAppProcess(object caller, StateBag viewState, HttpContext context)
		: base(caller, viewState, context)
	{
	}

	/// <summary>
	/// ページ初期化
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public new void Page_Init(object sender, EventArgs e)
	{
		base.Page_Init(sender, e);

		// LINEミニアプリが使用可能な設定でなければトップページに遷移
		if (CheckMiniAppUsable() == false)
		{
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT);
		}

		// LINEミニアプリ内でLINE情報を保持していない場合は未ログイン扱いとする
		if ((this.HasLineTempUser == false)
			&& (this.Request.Path != Constants.PATH_ROOT + Constants.PAGE_LINE_MINIAPP_SINGLE_SIGN_ON))
		{
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_LINE_MINIAPP_SINGLE_SIGN_ON);
		}
	}

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public new void Page_Load(object sender, EventArgs e)
	{
		if ((this.IsPreview == false) && this.IsLoggedIn)
		{
			this.LoginUser.UserExtend = new UserService().GetUserExtend(this.LoginUserId);
		}
	}

	/// <summary>
	/// LINEミニアプリが使用可能か
	/// </summary>
	/// <returns>LINEミニアプリが使用可能か</returns>
	private bool CheckMiniAppUsable()
	{
		var isUsable = w2.App.Common.Line.Constants.LINE_MINIAPP_OPTION_ENABLED
			&& w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED;
		return isUsable;
	}

	/// <summary>ログインしているのが仮ユーザーか</summary>
	public bool IsTempLoggedIn
	{
		get { return this.LineTempUser.IsRegularUser == false; }
	}
	/// <summary>ログインユーザーLINE-ID</summary>
	public string LoginLineUserId
	{
		get { return this.IsTempLoggedIn ? this.LineTempUser.LineUserId : string.Empty; }
	}
	/// <summary>仮登録ユーザーを保持しているか</summary>
	public bool HasLineTempUser
	{
		get{ return this.LineTempUser != null; }
	}
	/// <summary>LINEミニアプリ：仮登録ユーザー</summary>
	public LineTemporaryUserModel LineTempUser
	{
		get { return (LineTemporaryUserModel)this.Session["LineTempUser"]; }
		set { this.Session["LineTempUser"] = value; }
	}
}