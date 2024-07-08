/*
=========================================================================================================
  Module      : メールマガジン登録確認画面処理(MailMagazineRegistConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Web.Process;

/// <summary>
/// Mail Magazine Regist Confirm
/// </summary>
public partial class Form_User_MailMagazineRegistConfirm : BasePage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
    {
		//------------------------------------------------------
		// HTTPS通信チェック（HTTPのとき、規約画面へ）
		//------------------------------------------------------
		CheckHttps();

		//------------------------------------------------------
		// URLセッションチェック
		//------------------------------------------------------
		CheckUrlSessionForUserRegistModify();
	}

	/// <summary>
	/// 送信リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSend_Click(object sender, EventArgs e)
	{
		this.Process.ExecRegistUserMailMagazine(this.UserInput);

		// メールアドレスをセッションへ格納（完了ページ表示用）
		Session[Constants.SESSION_KEY_PARAM] = this.UserInput;

		//------------------------------------------------------
		// 完了画面へ
		//------------------------------------------------------
		// メールマガジン登録完了画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_MAILMAGAZINE_REGIST_COMPLETE);

	}

	/// <summary>
	/// 戻るリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbBack_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// ターゲットページ設定
		//------------------------------------------------------
		this.SessionParamTargetPage = Constants.PAGE_FRONT_MAILMAGAZINE_REGIST_INPUT;

		//------------------------------------------------------
		// 会員登録ページへ遷移
		//------------------------------------------------------
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_MAILMAGAZINE_REGIST_INPUT);
	}

	/// <summary>ユーザーバリューオブジェクト</summary>
	protected UserInput UserInput { get { return (UserInput)Session[Constants.SESSION_KEY_PARAM]; } }
	/// <summary>プロセス</summary>
	protected new MailMagazineRegistrationProcess Process
	{
		get { return (MailMagazineRegistrationProcess)this.ProcessTemp; }
	}
	/// <summary>プロセステンポラリ</summary>
	protected override IPageProcess ProcessTemp
	{
		get
		{
			if (m_processTmp == null) m_processTmp = new MailMagazineRegistrationProcess(this, this.ViewState, this.Context);
			return m_processTmp;
		}
	}
}