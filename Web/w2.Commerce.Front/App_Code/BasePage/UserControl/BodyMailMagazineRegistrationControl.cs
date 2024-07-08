/*
=========================================================================================================
  Module      : メールマガジン登録画面(BodyMailMagazineRegistrationControl.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Web.Process;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.User;

public class BodyMailMagazineRegistrationControl : BaseUserControl
{
	#region ラップ済コントロール宣言
	protected WrappedHtmlGenericControl WdvMailMagazineRegistInput { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvMailMagazineRegistInput"); } }
	protected WrappedHtmlGenericControl WdvUserModifyConfirm { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvUserModifyConfirm"); } }
	protected WrappedTextBox WtbUserMailAddr { get { return GetWrappedControl<WrappedTextBox>("tbUserMailAddr"); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// メルマガ入力項目と登録結果の表示を設定
			this.WdvMailMagazineRegistInput.Visible = true;
			this.WdvUserModifyConfirm.Visible = false;

			if (this.IsLoggedIn)
			{
				// ユーザーIDからユーザー情報の取得し、入力欄にセットする
				var user = new UserService().Get(this.LoginUserId);

				if (user != null) this.WtbUserMailAddr.Text = user.MailAddr;
			}
		}
	}

	/// <summary>
	/// 登録するリンクをクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbConfirm_Click(object sender, EventArgs e)
	{
		var userInput = new UserInput(new UserModel())
		{
			MailAddr = StringUtility.ToHankaku(this.WtbUserMailAddr.Text.Trim())
		};
		this.Process.ExecRegistUserMailMagazine(userInput);

		// メルマガ入力項目と登録結果の表示を設定
		this.WdvMailMagazineRegistInput.Visible = false;
		this.WdvUserModifyConfirm.Visible = true;
	}

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