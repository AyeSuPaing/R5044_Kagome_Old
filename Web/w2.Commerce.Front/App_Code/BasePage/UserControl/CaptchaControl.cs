/*
=========================================================================================================
  Module      : キャプチャ認証コントローラ処理(CaptchaControl.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using w2.App.Common.Api;

/// <summary>
/// キャプチャ認証コントローラ処理
/// </summary>
public class CaptchaControl : BaseUserControl
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			this.IsSuccess = false;
		}
	}

	/// <summary>
	/// キャプチャ認証状態保存イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSave_Click(object sender, EventArgs e)
	{
		// キャプチャ認証結果取得
		var auth = new CaptchaApi(Constants.RECAPTCHA_SECRET_KEY, Request["g-recaptcha-response"]);
		auth.Auth();
		this.IsSuccess = auth.IsSuccess;
	}

	#region プロパティ
	/// <summary>キャプチャ認証成功？</summary>
	public bool IsSuccess
	{
		get { return (bool)ViewState["IsSuccess"]; }
		set { ViewState["IsSuccess"] = value; }
	}
	/// <summary>キャプチャ認証成功時に有効にするコントールのID</summary>
	public string EnabledControlClientID { get; set; }
	#endregion
}