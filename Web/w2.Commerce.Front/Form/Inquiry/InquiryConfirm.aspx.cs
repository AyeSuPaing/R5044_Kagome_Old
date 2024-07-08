/*
=========================================================================================================
  Module      : 問合せ確認画面処理(InquiryConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Text;
using w2.App.Common;
using w2.App.Common.Global.Region;

public partial class Form_Inquiry_InquiryConfirm : BasePage
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
		// HTTPS通信チェック（HTTPの場合、HTTPSで再読込）
		//------------------------------------------------------
		CheckHttps(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_INQUIRY_CONFIRM);

		//------------------------------------------------------
		// URLセッションチェック
		//------------------------------------------------------
		CheckUrlSessionForUserRegistModify();

		if (!IsPostBack)
		{
			// apsx側プロパティセットしているため、バインドを行う
			if (this.Captcha != null)
			{
				this.Captcha.DataBind();
			}
		}
	}

	/// <summary>
	/// キャプチャ認証チェック
	/// </summary>
	/// <returns>成功：true、失敗：false（※キャプチャ認証利用なしの場合もtrueを返す）</returns>
	protected bool CheckCaptcha()
	{
		// キャプチャ認証サイトキー指定なしの場合は何もしない
		if (string.IsNullOrEmpty(Constants.RECAPTCHA_SITE_KEY)) return true;

		// キャプチャ認証コントロールがなしの場合は何もしない
		if (this.Captcha == null) return true;

		// キャプチャ認証成功？
		if (this.Captcha.IsSuccess) return true;

		return false;
	}

	/// <summary>
	/// 送信するリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSend_Click(object sender, EventArgs e)
	{
		// キャプチャ認証失敗時は処理終了
		if (CheckCaptcha() == false) return;

		//------------------------------------------------------
		// メール送信(管理者向け)
		//------------------------------------------------------
		using (MailSendUtility msMailSend = new MailSendUtility(Constants.CONST_DEFAULT_SHOP_ID, Constants.CONST_MAIL_ID_INQUIRY_INPUT_FOR_OPERATOR, "", InquiryInput, true, Constants.MailSendMethod.Auto))
		{
			// メール送信元に「@@mail_addr@@」が登録されている場合、From変換
			// ※メール送信元にスペースを含む登録が行えないため
			if (msMailSend.TmpFrom.Contains("@@" + Constants.FIELD_USER_MAIL_ADDR + "@@"))
			{
				msMailSend.SetFrom((string)InquiryInput[Constants.FIELD_USER_MAIL_ADDR], msMailSend.Message.From.DisplayName);
			}
			else
			{
				// ユーザーのメールアドレスは Reply-Toに設定する
				msMailSend.SetReplyTo((string)InquiryInput[Constants.FIELD_USER_MAIL_ADDR]);
			}

			if (msMailSend.SendMail() == false)
			{
				AppLogger.WriteError(this.GetType().BaseType.ToString() + " : " + msMailSend.MailSendException.ToString());
			}
		}

		//------------------------------------------------------
		// メール送信(ユーザ向け)
		//------------------------------------------------------
		using (MailSendUtility msMailSend = new MailSendUtility(
			Constants.CONST_DEFAULT_SHOP_ID,
			Constants.CONST_MAIL_ID_INQUIRY_INPUT_FOR＿USER,
			this.LoginUserId,
			InquiryInput,
			true,
			Constants.MailSendMethod.Auto,
			RegionManager.GetInstance().Region.LanguageCode,
			RegionManager.GetInstance().Region.LanguageLocaleId,
			(string)InquiryInput[Constants.FIELD_USER_MAIL_ADDR]))
		{
			// 送信先に入力メールアドレスを設定
			msMailSend.AddTo((string)InquiryInput[Constants.FIELD_USER_MAIL_ADDR]);
			if (msMailSend.SendMail() == false)
			{
				AppLogger.WriteError(this.GetType().BaseType.ToString() + " : " + msMailSend.MailSendException.ToString());
			}
		}

		//------------------------------------------------------
		// 問合せ完了ページへ遷移
		//------------------------------------------------------
		// 画面遷移制御を削除
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = null;

		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_INQUIRY_COMPLETE);
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
		this.SessionParamTargetPage = Constants.PAGE_FRONT_INQUIRY_INPUT;

		//------------------------------------------------------
		// 問い合わせ入力ページへ遷移
		//------------------------------------------------------
		StringBuilder queryParams = new StringBuilder();
		queryParams.Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_INQUIRY_INPUT);
		if(((string)InquiryInput[Constants.FIELD_PRODUCT_SHOP_ID] != "")
			&& ((string)InquiryInput[Constants.FIELD_PRODUCT_PRODUCT_ID] != ""))
		{
			queryParams.Append("?").Append(Constants.REQUEST_KEY_SHOP_ID).Append("=").Append(InquiryInput[Constants.FIELD_PRODUCT_SHOP_ID]);
			queryParams.Append("&").Append(Constants.REQUEST_KEY_PRODUCT_ID).Append("=").Append(InquiryInput[Constants.FIELD_PRODUCT_PRODUCT_ID]);
			queryParams.Append("&").Append(Constants.REQUEST_KEY_VARIATION_ID).Append("=").Append(InquiryInput[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]);
		}
		Response.Redirect(queryParams.ToString());
	}

	#region プロパティ
	/// <summary>問い合わせ情報</summary>
	protected Hashtable InquiryInput { get { return (Hashtable)Session[Constants.SESSION_KEY_PARAM]; } }
	/// <summary>キャプチャ認証コントロール</summary>
	private CaptchaControl Captcha
	{
		get
		{
			var captcha = GetDefaultMasterContentPlaceHolder().FindControl("ucCaptcha");
			return (captcha != null) ? (CaptchaControl)captcha : null;
		}
	} 
	#endregion
}
