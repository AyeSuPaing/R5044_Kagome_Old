/*
=========================================================================================================
  Module      : メール配信文章メールクリック設定ページ処理(MailDistTextMailclick.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.Text;
using w2.App.Common.MailDist;

public partial class Form_MailDistText_MailDistTextMailclick : DecomeBasePage
{
	string m_strMailTextId = null;
	Random m_random = new Random();	// メソッド呼び出しのたびにこのインスタンスを生成すると、乱数シードが同じ値で更新され出力結果がランダムにならないため、外で宣言する。

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// リクエスト取得＆ビューステート格納
			//------------------------------------------------------
			m_strMailTextId = Request[Constants.REQUEST_KEY_MAILTEXT_ID];
			ViewState[Constants.REQUEST_KEY_MAILTEXT_ID] = m_strMailTextId;

			//------------------------------------------------------
			// 画面制御
			//------------------------------------------------------
			//InitializeComponents(m_strActionStatus);

			//------------------------------------------------------
			// 画面設定処理
			//------------------------------------------------------
			DataView dvMailDistText = null;
			DataView dvMailClickPC = null;
			DataView dvMailClickMobile = null;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				using (SqlStatement sqlStatement = new SqlStatement("MailDistText", "GetMailDistText"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_MAILDISTTEXT_DEPT_ID, this.LoginOperatorDeptId);
					htInput.Add(Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID, m_strMailTextId);

					dvMailDistText = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
				}
				using (SqlStatement sqlStatement = new SqlStatement("MailClick", "GetMailClick"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_MAILCLICK_DEPT_ID, this.LoginOperatorDeptId);
					htInput.Add(Constants.FIELD_MAILCLICK_MAILTEXT_ID, m_strMailTextId);
					htInput.Add(Constants.FIELD_MAILCLICK_PCMOBILE_KBN, Constants.FLG_MAILCLICK_PCMOBILE_KBN_PC);

					dvMailClickPC = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
				}
				using (SqlStatement sqlStatement = new SqlStatement("MailClick", "GetMailClick"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_MAILCLICK_DEPT_ID, this.LoginOperatorDeptId);
					htInput.Add(Constants.FIELD_MAILCLICK_MAILTEXT_ID, m_strMailTextId);
					htInput.Add(Constants.FIELD_MAILCLICK_PCMOBILE_KBN, Constants.FLG_MAILCLICK_PCMOBILE_KBN_MOBILE);

					dvMailClickMobile = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
				}
			}

			// 該当データが有りの場合
			if (dvMailDistText.Count != 0)
			{
				DataRowView drvMailDistText = dvMailDistText[0];
				lMailtextId.Text = WebSanitizer.HtmlEncode(drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID]);
				lMailtextName.Text = WebSanitizer.HtmlEncode(drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_NAME]);
				lMailFromName.Text = WebSanitizer.HtmlEncode(drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAIL_FROM_NAME]);
				lMailFrom.Text = WebSanitizer.HtmlEncode(drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAIL_FROM]);
				lMailtextSubject.Text = WebSanitizer.HtmlEncode(drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_SUBJECT]);
				lMailtextSubjectMobile.Text = ReplaceEmojiTagToHtml((string)drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_SUBJECT_MOBILE]);
				dvMailtextSubjectMobile.Visible = (string.IsNullOrEmpty(lMailtextSubjectMobile.Text) == false);
				//lMailtextText.Text = StringUtility.ChangeToBrTag(WebSanitizer.HtmlEncode(drvMailDistText[Constants.FIELD_MAILDISTTEXT_mailtext_body]));
				//lMailtextHtml.Text = WebSanitizer.HtmlEncode(drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_HTML]);
				//lMailtextMobile.Text = StringUtility.ChangeToBrTag(WebSanitizer.HtmlEncode(drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_BODY_MOBILE]));
				lMailtextDateCreated.Text = WebSanitizer.HtmlEncode(drvMailDistText[Constants.FIELD_MAILDISTTEXT_DATE_CREATED]);
				lMailtextDateChanged.Text = WebSanitizer.HtmlEncode(drvMailDistText[Constants.FIELD_MAILDISTTEXT_DATE_CHANGED]);
				lMailtextLastChanged.Text = WebSanitizer.HtmlEncode(drvMailDistText[Constants.FIELD_MAILDISTTEXT_LAST_CHANGED]);

				//------------------------------------------------------
				// 本文・URL部分設定
				//------------------------------------------------------
				SetUrlText((string)drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_BODY], dvMailClickPC, rMailTextBody, false);
				SetUrlText((string)drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_BODY_MOBILE], dvMailClickMobile, rMailTextBodyMobile, false);
				SetUrlText((string)drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_HTML], dvMailClickPC, rMailTextHtml, true);

				// モバイルメールが設定されていなければプレビューの枠を消す
				if (string.IsNullOrEmpty((string)drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_BODY_MOBILE]))
				{
					dvMailtextBodyMobile.Visible = false;
				}

				// 絵文字タグを画像に変換
				foreach (RepeaterItem repeaterItem in rMailTextBodyMobile.Items)
				{
					((Literal)repeaterItem.FindControl("lMailTextLine")).Text = ReplaceEmojiTagToHtml(((Literal)repeaterItem.FindControl("lMailTextLine")).Text);
				}

				List<string> lMaixTextUrls = new List<string>();
				foreach (Match mUrl in Regex.Matches((string)drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_HTML], @"https?:\/\/[-_.!~*'()a-zA-Z0-9;/?:@&=+$,%#]+"))
				{
					lMaixTextUrls.Add(mUrl.Value);
				}
			}
			else
			{
				// 該当データ無しの場合、エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
			}
		}
		else
		{
			m_strMailTextId = (string)ViewState[Constants.REQUEST_KEY_MAILTEXT_ID];
		}
	}
	
	/// <summary>
	/// 本文データ画面セット（メールクリック設定用チェックボックス設定）
	/// </summary>
	/// <param name="strMailText"></param>
	/// <param name="dvMailClick"></param>
	/// <param name="rTarget"></param>
	private void SetUrlText(string strMailText, DataView dvMailClick, Repeater rTarget, bool blIsHtml)
	{
		string strUrlPattern = MailDistTextUtility.GetPatturnUrl(blIsHtml);
		string strSeparatePattern = MailDistTextUtility.GetSeparatePattern(blIsHtml);

		rTarget.DataSource = MailDistTextUtility.CreateMailTextLines(strMailText, strSeparatePattern);
		rTarget.DataBind();

		foreach (RepeaterItem ri in rTarget.Items)
		{
			string strMailTextLine = ((Literal)ri.FindControl("lMailTextLine")).Text;

			Match mAnchorUrl = Regex.Match(strMailTextLine, strUrlPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
			if (mAnchorUrl.Success)
			{
				string strAnchorUrl = mAnchorUrl.Value;
				string strUrl = strAnchorUrl;
				if (blIsHtml)
				{
					Match m = Regex.Match(strUrl, MailDistTextUtility.PATTURN_URL_TEXT);
					if (m.Success)
					{
						strUrl = m.Value;
					}
				}

				foreach (DataRowView drvMailClick in dvMailClick)
				{
					if (strUrl == (string)drvMailClick[Constants.FIELD_MAILCLICK_MAILCLICK_URL])
					{
						((Literal)ri.FindControl("lMailTextLine")).Text = strMailTextLine.Replace(strAnchorUrl, "<span style='color:Blue; font-weight:bold'>[</span>" + strAnchorUrl + "<span style='color:Blue; font-weight:bold'>]</span>");
						((CheckBox)ri.FindControl("cbMailClickUrl")).Checked = true;
						break;
					}
				}
				((CheckBox)ri.FindControl("cbMailClickUrl")).Visible = true;
			}
		}
	}

	/// <summary>
	/// 通常テキストのメールクリック有効のリンクをリストで取得
	/// </summary>
	/// <param name="rText"></param>
	/// <param name="strMailClickKbn"></param>
	/// <returns></returns>
	protected List<KeyValuePair<string, string>> GetCheckedUrlsOfText(Repeater rText, string strMailClickKbn)
	{
		List<KeyValuePair<string, string>> lCheckedUrls = new List<KeyValuePair<string, string>>();

		foreach (RepeaterItem ri in rText.Items)
		{
			CheckBox cbMailClickUrl = (CheckBox)ri.FindControl("cbMailClickUrl");
			if (cbMailClickUrl.Visible)
			{
				string strMailTextLine = ((Literal)ri.FindControl("lMailTextLine")).Text;
				MatchCollection mcUrl = Regex.Matches(strMailTextLine, MailDistTextUtility.PATTURN_URL_TEXT);
				if ((mcUrl.Count != 0) && (cbMailClickUrl.Checked))
				{
					lCheckedUrls.Add(new KeyValuePair<string, string>(mcUrl[0].Value, strMailClickKbn));
				}
			}
		}

		return lCheckedUrls;
	}

	/// <summary>
	/// HTMLのメールクリック有効のリンクをリスト取得
	/// </summary>
	/// <param name="rText">HTML文書リピーターコントロール</param>
	/// <param name="mailClickKbn">メールクリック区分</param>
	/// <returns>HTMLのメールクリック有効のリンクリスト</returns>
	protected List<KeyValuePair<string, string>> GetCheckedUrlsOfHtml(Repeater rText, string mailClickKbn)
	{
		var checkedUrls = new List<KeyValuePair<string, string>>();

		foreach (RepeaterItem ri in rText.Items)
		{
			var cbMailClickUrl = (CheckBox)ri.FindControl("cbMailClickUrl");
			if (cbMailClickUrl.Visible && cbMailClickUrl.Checked)
			{
				var mailTextLine = ((Literal)ri.FindControl("lMailTextLine")).Text;
				var matchUrl = Regex.Matches(mailTextLine, MailDistTextUtility.PATTURN_URL_HTML, RegexOptions.Singleline | RegexOptions.IgnoreCase);
				if ((matchUrl.Count != 0) && (cbMailClickUrl.Checked))
				{
					var matchUrlText = Regex.Matches(matchUrl[0].Value, MailDistTextUtility.PATTURN_URL_TEXT, RegexOptions.Singleline | RegexOptions.IgnoreCase);
					if ((matchUrlText.Count != 0) && (cbMailClickUrl.Checked))
					{
						checkedUrls.Add(new KeyValuePair<string, string>(matchUrlText[0].Value, mailClickKbn));
					}
				}
			}
		}

		return checkedUrls;
	}

	/// <summary>
	/// 更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// チェックされたクリックURL取得
		//------------------------------------------------------
		List<KeyValuePair<string, string>> lCheckedUrls = new List<KeyValuePair<string, string>>();
		// PC URL
		lCheckedUrls.AddRange(GetCheckedUrlsOfText(rMailTextBody, Constants.FLG_MAILCLICK_PCMOBILE_KBN_PC));

		// PC HTML URL
		lCheckedUrls.AddRange(GetCheckedUrlsOfHtml(rMailTextHtml, Constants.FLG_MAILCLICK_PCMOBILE_KBN_PC));

		// MOBILE URL
		lCheckedUrls.AddRange(GetCheckedUrlsOfText(rMailTextBodyMobile, Constants.FLG_MAILCLICK_PCMOBILE_KBN_MOBILE));

		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			// トランザクション開始 //
			sqlAccessor.OpenConnection();
			sqlAccessor.BeginTransaction();

			try
			{
				//------------------------------------------------------
				// メールクリックいったんすべて無効にする(UPDATE)
				//------------------------------------------------------
				using (SqlStatement sqlStatement = new SqlStatement("MailClick", "UpdateMailClickUnvalidAll"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_MAILCLICK_DEPT_ID, this.LoginOperatorDeptId);
					htInput.Add(Constants.FIELD_MAILCLICK_MAILTEXT_ID, m_strMailTextId);
					htInput.Add(Constants.FIELD_MAILCLICK_LAST_CHANGED, this.LoginOperatorName);

					sqlStatement.ExecStatement(sqlAccessor, htInput);
				}

				//------------------------------------------------------
				// メールクリック登録
				//------------------------------------------------------
				foreach (KeyValuePair<string,string> kvpUrl in lCheckedUrls)
				{
					//------------------------------------------------------
					// メールクリック取得（存在確認）
					//------------------------------------------------------
					DataView dvMailClick = null;
					using (SqlStatement sqlStatement = new SqlStatement("MailClick", "GetMailClickUrl"))
					{
						Hashtable htInput = new Hashtable();
						htInput.Add(Constants.FIELD_MAILCLICK_DEPT_ID, this.LoginOperatorDeptId);
						htInput.Add(Constants.FIELD_MAILCLICK_MAILTEXT_ID, m_strMailTextId);
						htInput.Add(Constants.FIELD_MAILCLICK_MAILCLICK_URL, kvpUrl.Key);
						htInput.Add(Constants.FIELD_MAILCLICK_PCMOBILE_KBN, kvpUrl.Value);

						dvMailClick = sqlStatement.SelectSingleStatement(sqlAccessor, htInput);
					}

					// 存在しなければ登録
					if (dvMailClick.Count == 0)
					{
						//------------------------------------------------------
						// メールクリック登録（キーが重複しなくなるまで繰り返す）
						//------------------------------------------------------
						using (SqlStatement sqlStatement = new SqlStatement("MailClick", "InsertMailClick"))
						{
							for (int iLoop = 0; iLoop < 20; iLoop++)
							{
								Hashtable htInput = new Hashtable();
								htInput.Add(Constants.FIELD_MAILCLICK_DEPT_ID, this.LoginOperatorDeptId);
								htInput.Add(Constants.FIELD_MAILCLICK_MAILTEXT_ID, m_strMailTextId);
								htInput.Add(Constants.FIELD_MAILCLICK_MAILDIST_ID, "");
								htInput.Add(Constants.FIELD_MAILCLICK_ACTION_NO, 0);
								//htInput.Add(Constants.FIELD_MAILCLICK_MAILCLICK_ID, ""); 自動作成
								htInput.Add(Constants.FIELD_MAILCLICK_MAILCLICK_URL, kvpUrl.Key);
								htInput.Add(Constants.FIELD_MAILCLICK_MAILCLICK_KEY, CreateRandomKey(10));
								htInput.Add(Constants.FIELD_MAILCLICK_PCMOBILE_KBN, kvpUrl.Value);
								htInput.Add(Constants.FIELD_MAILCLICK_LAST_CHANGED, this.LoginOperatorName);

								int iInsert = sqlStatement.ExecStatement(sqlAccessor, htInput);

								if (iInsert > 0)
								{
									break;
								}
							}
						}
					}
					// 存在する場合
					else
					{
						// 無効だったら有効にする
						if ((string)dvMailClick[0][Constants.FIELD_MAILCLICK_VALID_FLG] == Constants.FLG_MAILCLICK_VALID_FLG_INVALID)
						{
							using (SqlStatement sqlStatement = new SqlStatement("MailClick", "UpdateMailClickValid"))
							{
								Hashtable htInput = new Hashtable();
								htInput.Add(Constants.FIELD_MAILCLICK_DEPT_ID, this.LoginOperatorDeptId);
								htInput.Add(Constants.FIELD_MAILCLICK_MAILTEXT_ID, m_strMailTextId);
								htInput.Add(Constants.FIELD_MAILCLICK_MAILCLICK_URL, kvpUrl.Key);
								htInput.Add(Constants.FIELD_MAILCLICK_PCMOBILE_KBN, kvpUrl.Value);
								htInput.Add(Constants.FIELD_MAILCLICK_VALID_FLG, Constants.FLG_MAILCLICK_VALID_FLG_VALID);
								htInput.Add(Constants.FIELD_MAILCLICK_LAST_CHANGED, this.LoginOperatorName);

								sqlStatement.ExecStatement(sqlAccessor, htInput);
							}
						}
					}
				}

				// コミット //
				sqlAccessor.CommitTransaction();
			}
			catch (Exception ex)
			{
				// トランザクションロールバック //
				sqlAccessor.RollbackTransaction();
				throw ex;
			}
		}
	}

	/// <summary>
	/// ランダムキー作成
	/// </summary>
	/// <param name="iLength"></param>
	/// <returns></returns>
	private string CreateRandomKey(int iLength)
	{
		StringBuilder sbRandomKey = new StringBuilder();

		char[] chRandomChars = { 
			'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
			'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

		for (int iLoop = 0; iLoop < iLength; iLoop++)
		{
			int iCharIndex = m_random.Next(chRandomChars.Length);
			sbRandomKey.Append(chRandomChars[iCharIndex]);
		}

		return sbRandomKey.ToString();
	}
}
