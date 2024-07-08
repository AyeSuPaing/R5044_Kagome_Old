/*
=========================================================================================================
  Module      : デザインページパーツページ(DesignPagePartsPage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using w2.Common.Util.Archiver;

/// <summary>
/// DesignPagePartsPage の概要の説明です
/// </summary>
public abstract class DesignPagePartsPage : DesignBasePage
{
	private const string ERROR_CAPTION_DETAIL_COMPILER_OUTPUT = "詳しいコンパイラ出力を表示:";	// 読み取り終了文字
	private const string ERROR_CAPTION_TAG = "<h2> <i>";					// エラー情報読み取り開始タグ
	private const string ERROR_CODE_START_TAG = "<code>";				// コード開始タグ
	private const string ERROR_CODE_END_TAG = "</code>";					// コード終了タグ
	private const string ERROR_PREFORMATTED_TEXT_START_TAG = "<pre>";	// 整形済みテキスト開始タグ
	private const string ERROR_PREFORMATTED_TEXT_END_TAG = "</pre>";		// 整形済みテキスト終了タグ

	/// <summary>
	/// コンポーネントリフレッシュ
	/// </summary>
	protected abstract void RefreshComponent();

	/// <summary>
	/// カスタムリストリロード
	/// </summary>
	protected abstract void ReloadCustomListBoxes();

	/// <summary>
	/// 編集可能エリア取得
	/// </summary>
	/// <param name="strFileTextAll"></param>
	/// <returns></returns>
	protected List<KeyValuePair<string, StringBuilder>> GetEditableAreaForEdit(string strFileTextAll)
	{
		// 編集領域可能エリアの取得
		List<KeyValuePair<string, StringBuilder>> lEditableText = GetAreas(strFileTextAll, m_strEditableTagBgns, m_strEditableTagEnd);
		foreach (KeyValuePair<string, StringBuilder> ctsEditableText in lEditableText)
		{
			// パーツ置換
			foreach (string strKey in m_dicUserControlTags.Keys)
			{
				// カスタムパーツのみタグ置換を行う
				if (((string)m_dicUserControlTags[strKey]).StartsWith("<uc:Parts"))
				{
					ctsEditableText.Value.Replace(m_dicUserControlTags[strKey], TAG_PARTS_BGN + strKey + TAG_PARTS_END);
				}
			}
		}

		return lEditableText;
	}

	/// <summary>
	/// ＮＧワードチェック
	/// </summary>
	/// <param name="lEditableText"></param>
	/// <returns></returns>
	protected string CheckNGWord(List<KeyValuePair<string, StringBuilder>> lEditableText)
	{
		// ＮＧワードチェック
		StringBuilder sbSystemCodeHtmlEncoded = new StringBuilder();
		foreach (KeyValuePair<string, StringBuilder> ctsEditableText in lEditableText)
		{
			// 変換漏れチェック
			List<string> lSystemCode = new List<string>();
			foreach (Match match in Regex.Matches(ctsEditableText.Value.ToString(), "unat=\"server\""))
			{
				lSystemCode.Add(match.Value);
			}
			foreach (Match match in Regex.Matches(ctsEditableText.Value.ToString(), @"<uc\:.*"))
			{
				lSystemCode.Add(match.Value);
			}
			foreach (Match match in Regex.Matches(ctsEditableText.Value.ToString(), @"<w2c\:.*"))
			{
				lSystemCode.Add(match.Value);
			}
			foreach (Match match in Regex.Matches(ctsEditableText.Value.ToString(), @"<\%.*"))
			{
				lSystemCode.Add(match.Value);
			}
			foreach (string str in lSystemCode)
			{
				if (sbSystemCodeHtmlEncoded.Length != 0)
				{
					sbSystemCodeHtmlEncoded.Append("<br /><hr />");
				}
				sbSystemCodeHtmlEncoded.Append(WebSanitizer.HtmlEncode(str));
			}
		}

		return sbSystemCodeHtmlEncoded.ToString();
	}

	/// <summary>
	/// 各種エリア取得
	/// </summary>
	/// <param name="strHtml"></param>
	/// <param name="strTagBgns"></param>
	/// <param name="strTagEnd"></param>
	/// <returns></returns>
	protected List<KeyValuePair<string, StringBuilder>> GetAreas(string strHtml, string[] strTagBgns, string strTagEnd)
	{
		List<KeyValuePair<string, StringBuilder>> lEditableText = new List<KeyValuePair<string, StringBuilder>>();
		foreach (Match match in Regex.Matches(strHtml, strTagBgns[0] + ".*?" + strTagBgns[1] + "\r\n" + ".*?" + strTagEnd, RegexOptions.Singleline))
		{
			string strHeader = match.Value.Substring(0, match.Value.IndexOf("\r\n"));

			lEditableText.Add(new KeyValuePair<string, StringBuilder>(
				strHeader.Replace(strTagBgns[0], "").Replace(strTagBgns[1], ""),
				new StringBuilder(match.Value.Replace(strHeader + "\r\n", "").Replace("\r\n" + strTagEnd, "").Replace(strTagEnd, "")))); // 中身が空の時とそうでない時とでEndタグは２度置換
		}
		return lEditableText;
	}
	/// <summary>
	/// 各種エリア取得
	/// </summary>
	/// <param name="strHtml"></param>
	/// <param name="strTagBgn"></param>
	/// <param name="strTagEnd"></param>
	/// <returns></returns>
	protected string GetArea(string strHtml, string strTagBgn, string strTagEnd)
	{
		foreach (Match match in Regex.Matches(strHtml, strTagBgn + ".*?" + strTagEnd, RegexOptions.Singleline))
		{
			string strHeader = match.Value.Substring(0, match.Value.IndexOf("\r\n"));

			return match.Value.Replace(strHeader + "\r\n", "").Replace(strTagEnd, "").Trim();
		}
		return "";
	}

	/// <summary>
	/// テキストエリアの高さ取得
	/// </summary>
	/// <param name="strText"></param>
	/// <param name="iMin"></param>
	/// <param name="iMax"></param>
	/// <returns></returns>
	protected int GetAextAreaHeight(string strText, int iMin, int iMax)
	{
		int iRealHeight = (strText.Split('\n').Length + 1) * 16 + 1;

		if (iRealHeight < iMin)
		{
			return iMin;
		}
		else if (iRealHeight > iMax)
		{
			return iMax;
		}

		return iRealHeight;
	}

	/// <summary>
	/// ファイルタイトル置換
	/// </summary>
	/// <param name="sbFileTextAllSrc"></param>
	/// <param name="strTitle"></param>
	protected void ReplaceTitle(StringBuilder sbFileTextAllSrc, string strTitle)
	{
		foreach (Match mPageTag in Regex.Matches(sbFileTextAllSrc.ToString(), "<%@ Page " + ".*?" + "%>"))
		{
			string strReplacedPageTag = null;
			foreach (Match mTitle in Regex.Matches(mPageTag.Value, " Title=\".*?\" "))
			{
				strReplacedPageTag = mPageTag.Value.Replace(
					mTitle.Value,
					" Title=\"" + WebSanitizer.HtmlEncode(strTitle) + "\" ");
				break;
			}
			if (strReplacedPageTag != null)
			{
				sbFileTextAllSrc.Replace(mPageTag.Value, strReplacedPageTag);
			}
			break;
		}
	}

	/// <summary>
	/// ファイル削除処理
	/// </summary>
	/// <param name="lbStandardObjectListBox"></param>
	/// <param name="lbCustomObjectListBox"></param>
	protected void DeleteFile(ListBox lbStandardObjectListBox, ListBox lbCustomObjectListBox)
	{
		//------------------------------------------------------
		// ファイル削除
		//------------------------------------------------------
		if (lbCustomObjectListBox.SelectedIndex != -1)
		{
			try
			{
				File.Delete(this.PhysicaldirpathTargetSite + lbCustomObjectListBox.SelectedValue);
			}
			catch (Exception ex)
			{
				AppLogger.WriteError("ファイルが削除できませんでした:" + lbCustomObjectListBox.SelectedValue, ex);

				Session[Constants.SESSION_KEY_ERROR_MSG] = "ファイルが削除できませんでした";
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
		}

		//------------------------------------------------------
		// 画面更新
		//------------------------------------------------------
		// 選択コンテンツの１つ前を取得（画面更新後選択状態にしたい）
		int iCustomPartsListSelectedIndex = -1;
		if ((lbCustomObjectListBox.SelectedIndex != -1)
			&& (lbCustomObjectListBox.Items.Count > 1))
		{
			iCustomPartsListSelectedIndex = lbCustomObjectListBox.SelectedIndex - 1;
			iCustomPartsListSelectedIndex = (iCustomPartsListSelectedIndex < 0) ? 0 : iCustomPartsListSelectedIndex;
		}

		// カスタムリストボックスリロード
		ReloadCustomListBoxes();

		// 選択状態セット
		if (iCustomPartsListSelectedIndex != -1)
		{
			SelectListBox(lbCustomObjectListBox, iCustomPartsListSelectedIndex);
		}
		else
		{
			SelectListBox(lbStandardObjectListBox, 0);
		}
	}

	/// <summary>
	/// ユーザコントロール宣言取得
	/// </summary>
	/// <param name="sbFileTextAll">Content</param>
	/// <param name="currentDefinations">Current Part Defination</param>
	protected string GetUserControlDeclarations(StringBuilder sbFileTextAll, string currentDefinations)
	{
		StringBuilder sbResult = new StringBuilder();
		string part = string.Empty;
		string currentPart = string.Empty;

		foreach (string strKey in m_dicUserControlTags.Keys)
		{
			// ユーザコントロールを利用している？
			if (sbFileTextAll.ToString().Contains(m_dicUserControlTags[strKey]))
			{
				part = m_dicUserControlDeclarations[strKey];

				// Get defination from currents
				currentPart = GetCurrentDefination(currentDefinations, part);
				if ((string.IsNullOrEmpty(currentPart) == false)
					&& (sbResult.ToString().Contains(currentPart) == false))
				{
					sbResult.Append(currentPart).Append("\r\n");
				}
				// Get new defination
				else if (sbResult.ToString().Contains(part) == false)
				{
					// ユーザコントロール宣言追加
					sbResult.Append(part.Replace("~/", "~/" + this.TargetSitePathRoot.Substring(Constants.PATH_ROOT_FRONT_PC.Length))).Append("\r\n");
				}
			}
		}

		return sbResult.ToString();
	}

	/// <summary>
	/// Get defination form current definations
	/// </summary>
	/// <param name="currentDefination">Current definations</param>
	/// <param name="part">Part name</param>
	/// <returns></returns>
	private string GetCurrentDefination(string currentDefination, string part)
	{
		string tagName = "";

		foreach (Match match in Regex.Matches(part, "TagName=\".*?\"", RegexOptions.Singleline))
		{
			tagName = match.Value;
		}

		if (string.IsNullOrEmpty(tagName)) return string.Empty;

		return currentDefination.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None).FirstOrDefault(x => x.Contains(tagName));
	}

	/// <summary>
	/// 更新用コンテンツタグ置換
	/// </summary>
	/// <param name="sbFileTextAll"></param>
	/// <param name="rEditArea"></param>
	protected void ReplaceContentsTagForUpdate(StringBuilder sbFileTextAll, Repeater rEditArea)
	{
		//------------------------------------------------------
		// 領域数チェック
		//------------------------------------------------------
		List<KeyValuePair<string, StringBuilder>> lEditableText = GetAreas(sbFileTextAll.ToString(), m_strEditableTagBgns, m_strEditableTagEnd);

		if (rEditArea.Items.Count != lEditableText.Count)
		{
			// TODO:エラー処理
			throw new Exception("数が合いません");
		}

		//------------------------------------------------------
		// テキスト置換
		//------------------------------------------------------
		for (int iAreaIndex = 0; iAreaIndex < lEditableText.Count; iAreaIndex++)
		{
			// タグの内部取得
			string newText = m_strEditableTagBgns[0] + lEditableText[iAreaIndex].Key + m_strEditableTagBgns[1] + "\r\n" + ((TextBox)rEditArea.Items[iAreaIndex].FindControl("tbEdit")).Text.Trim() + "\r\n" + m_strEditableTagEnd;
			sbFileTextAll.Replace(Regex.Match(sbFileTextAll.ToString(), m_strEditableTagBgns[0] + lEditableText[iAreaIndex].Key + m_strEditableTagBgns[1] + ".*?" + m_strEditableTagEnd, RegexOptions.Singleline | RegexOptions.IgnoreCase).Value, newText);
		}

		//------------------------------------------------------
		// パーツ置換
		//------------------------------------------------------
		foreach (string strKey in m_dicUserControlTags.Keys)
		{
			sbFileTextAll.Replace(TAG_PARTS_BGN + strKey + TAG_PARTS_END, m_dicUserControlTags[strKey]);
		}

		// ユーザーコントロール宣言リプレース
		string strUserControlDeclarationsBefore = GetArea(sbFileTextAll.ToString(), m_strUserControlDeclarationBgn, m_strUserControlDeclarationEnd);
		sbFileTextAll.Replace(
			m_strUserControlDeclarationBgn + "\r\n" + strUserControlDeclarationsBefore + ((strUserControlDeclarationsBefore != "") ? "\r\n" : "") + m_strUserControlDeclarationEnd,
			m_strUserControlDeclarationBgn + "\r\n" + GetUserControlDeclarations(sbFileTextAll, strUserControlDeclarationsBefore) + m_strUserControlDeclarationEnd);
	}

	/// <summary>
	/// 更新用レイアウト置換
	/// </summary>
	/// <param name="sbFileTextAll"></param>
	/// <param name="iptLayoutTop"></param>
	/// <param name="iptLayoutRight"></param>
	/// <param name="iptLayoutBottom"></param>
	/// <param name="iptLayoutLeft"></param>
	protected void ReplaceLayoutForUpdate(
		StringBuilder sbFileTextAll,
		System.Web.UI.HtmlControls.HtmlInputHidden iptLayoutTop,
		System.Web.UI.HtmlControls.HtmlInputHidden iptLayoutRight,
		System.Web.UI.HtmlControls.HtmlInputHidden iptLayoutBottom,
		System.Web.UI.HtmlControls.HtmlInputHidden iptLayoutLeft)
	{
		string strUserControlDeclarationsBefore = GetArea(sbFileTextAll.ToString(), m_strUserControlDeclarationBgn, m_strUserControlDeclarationEnd);

		List<KeyValuePair<string, StringBuilder>> lLayoutText = GetAreas(sbFileTextAll.ToString(), m_strLayoutTagBgns, m_strLayoutTagEnd);
		foreach (KeyValuePair<string, StringBuilder> kvpLayout in lLayoutText)
		{
			string[] strControlNames = null;
			switch (kvpLayout.Key)
			{
				case "トップエリア":
					strControlNames = iptLayoutTop.Value.Split(',');
					break;

				case "ライトエリア":
					strControlNames = iptLayoutRight.Value.Split(',');
					break;

				case "ボトムエリア":
					strControlNames = iptLayoutBottom.Value.Split(',');
					break;

				case "レフトエリア":
					strControlNames = iptLayoutLeft.Value.Split(',');
					break;
			}

			StringBuilder sbLayoutInner = new StringBuilder();
			foreach (string strControlName in strControlNames)
			{
				if (strControlName != "")
				{
					// レイアウト追加
					if (m_dicUserControlTags.ContainsKey(strControlName))
					{
						sbLayoutInner.Append(m_dicUserControlTags[strControlName]).Append("\r\n");
					}
				}
			}
			// レイアウトリプレース
			sbFileTextAll.Replace(
				m_strLayoutTagBgns[0] + kvpLayout.Key + m_strLayoutTagBgns[1] + "\r\n" + kvpLayout.Value.ToString() + ((kvpLayout.Value.Length != 0) ? "\r\n" : "") + m_strLayoutTagEnd,
				m_strLayoutTagBgns[0] + kvpLayout.Key + m_strLayoutTagBgns[1] + "\r\n" + sbLayoutInner.ToString() + m_strLayoutTagEnd);
		}

		// ユーザーコントロール宣言リプレース
		sbFileTextAll.Replace(
			m_strUserControlDeclarationBgn + "\r\n" + strUserControlDeclarationsBefore + ((strUserControlDeclarationsBefore != "") ? "\r\n" : "") + m_strUserControlDeclarationEnd,
			m_strUserControlDeclarationBgn + "\r\n" + GetUserControlDeclarations(sbFileTextAll, strUserControlDeclarationsBefore) + m_strUserControlDeclarationEnd);
	}

	/// <summary>
	/// 最終更新者置換
	/// </summary>
	/// <param name="sbFileTextAllSrc"></param>
	protected void ReplaceLastChanged(StringBuilder sbFileTextAllSrc)
	{
		foreach (Match mPageTag in Regex.Matches(sbFileTextAllSrc.ToString(), TAG_FILEINFO_LASTCHANGED_BGN + ".*?" + TAG_FILEINFO_LASTCHANGED_END))
		{
			sbFileTextAllSrc.Replace(mPageTag.Value,
				TAG_FILEINFO_LASTCHANGED_BGN + WebSanitizer.HtmlEncode(this.LoginOperatorName) + TAG_FILEINFO_LASTCHANGED_END);
			break;
		}
	}

	/// <summary>
	/// プレビューファイル削除
	/// </summary>
	protected void DeletePreviewFile()
	{
		foreach (string strPreviewFile in Directory.GetFiles(this.PhysicaldirpathTargetSite, "*.Preview.*"))
		{
			File.Delete(strPreviewFile);
		}
		foreach (string strPreviewFile in Directory.GetFiles(this.PhysicaldirpathTargetSite + "Form/", "*.Preview.*"))
		{
			File.Delete(strPreviewFile);
		}
		foreach (string strPreviewFile in Directory.GetFiles(this.PhysicaldirpathTargetSite + "Form/Common/", "*.Preview.*", SearchOption.AllDirectories))
		{
			File.Delete(strPreviewFile);
		}
		foreach (string strPreviewFile in Directory.GetFiles(this.PhysicaldirpathTargetSite + "Form/Product/", "*.Preview.*"))
		{
			File.Delete(strPreviewFile);
		}
		foreach (string strPreviewFile in Directory.GetFiles(this.PhysicaldirpathTargetSite + "Page/", "*.Preview.*", SearchOption.AllDirectories))
		{
			File.Delete(strPreviewFile);
		}
	}

	/// <summary>
	/// ファイル更新
	/// </summary>
	/// <param name="strTargetFilePath"></param>
	/// <param name="strFileTextAll"></param>
	protected void UpdateFile(string strTargetFilePath, string strFileTextAll)
	{
		//------------------------------------------------------
		// ファイル書き込み処理
		//------------------------------------------------------
		if ((File.Exists(strTargetFilePath) == false)
			|| ((File.GetAttributes(strTargetFilePath) & FileAttributes.ReadOnly) != 0) == false)
		{
			using (StreamWriter sw = new StreamWriter(strTargetFilePath, false, Encoding.UTF8))
			{
				sw.Write(strFileTextAll);
			}
		}
		else
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FILE_READONLY_ERROR).Replace("@@ 1 @@", Path.GetFileName(strTargetFilePath));
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// 最終更新者更新
	/// </summary>
	/// <param name="strTargetFilePath"></param>
	protected void UpdateLastChanged(string strTargetFilePath)
	{
		StringBuilder sbFileTextAll = new StringBuilder(GetFileTextAll(strTargetFilePath));
		ReplaceLastChanged(sbFileTextAll);
		UpdateFile(strTargetFilePath, sbFileTextAll.ToString());
	}

	/// <summary>
	/// Webリクエスト送信
	/// </summary>
	/// <param name="strTargetUrl">ターゲットURL</param>
	protected void SendWebRequest(string strTargetUrl)
	{
		SendWebRequest(
			strTargetUrl,
			"Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; MDDS; InfoPath.2; OfficeLiveConnector.1.5; OfficeLivePatch.1.3; .NET4.0C; .NET4.0E)");
	}
	/// <summary>
	/// Webリクエスト送信
	/// </summary>
	/// <param name="strTargetUrl">ターゲットURL</param>
	/// <param name="strUserAgent">UserAgent</param>
	protected void SendWebRequest(string strTargetUrl, string strUserAgent)
	{
		// WEBアクセスしてコンパイル
		HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(strTargetUrl);

		// 文字エンコードの指定
		Encoding eAuthorizationEncoding = Encoding.GetEncoding("UTF-8");
		// 要求の事前認証を行うよう設定
		webRequest.PreAuthenticate = true;
		// ユーザーエージェントの設定
		webRequest.UserAgent = strUserAgent;
		// basic認証情報をBASE64でエンコードして設定
		webRequest.Headers.Add("Authorization: Basic " + Convert.ToBase64String(eAuthorizationEncoding.GetBytes(Constants.BASIC_AUTHENTICATION_USER_ACCOUNT)));

		using (Stream responseStream = webRequest.GetResponse().GetResponseStream())
		using (StreamReader sr = new StreamReader(responseStream, Encoding.UTF8))
		{
			string str = sr.ReadToEnd();
		}
	}

	/// <summary>
	/// ページのエラーチェック
	/// </summary>
	/// <param name="strTargetUrl">ターゲットURL</param>
	protected void CheckPageCompileError(string strTargetPageUrl)
	{
		CheckPageCompileError(
			strTargetPageUrl,
			"Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; MDDS; InfoPath.2; OfficeLiveConnector.1.5; OfficeLivePatch.1.3; .NET4.0C; .NET4.0E)");
	}
	/// <summary>
	/// ページのエラーチェック
	/// </summary>
	/// <param name="targetUrl">ターゲットURL</param>
	/// <param name="userAgent">UserAgent</param>
	protected void CheckPageCompileError(string targetUrl, string userAgent)
	{
		var webRequest = (HttpWebRequest)WebRequest.Create(targetUrl);

		// 文字エンコードの指定
		var authorizationEncoding = Encoding.GetEncoding("UTF-8");
		// 要求の事前認証を行うよう設定
		webRequest.PreAuthenticate = true;
		// ユーザーエージェントの設定
		webRequest.UserAgent = userAgent;
		// basic認証情報をBASE64でエンコードして設定
		webRequest.Headers.Add("Authorization: Basic " + Convert.ToBase64String(authorizationEncoding.GetBytes(Constants.BASIC_AUTHENTICATION_USER_ACCOUNT)));

		try
		{
			// WEBアクセスしてコンパイルさせる
			using (var webResponse = (HttpWebResponse)webRequest.GetResponse())
			{
				// なにもしない //
			}
		}
		// リクエストで例外発生時は、エラー画面に遷移しレスポンスの内容を出力する
		catch (WebException we)
		{
			string errorMessage = null;
			if (we.Response != null)
			{
				errorMessage = GetAccessErrorMessage(we);

				// Write log when have error config from IIS. Not write log when design content update error
				if (string.IsNullOrEmpty(errorMessage) || errorMessage.Contains("[ConfigurationErrorsException]"))
				{
					AppLogger.WriteError(string.Format("URL（{0}）へのアクセスでエラーが発生しました。", targetUrl), we);
				}

				// 認証エラーの場合はエラー画面に遷移し、認証エラーである旨のメッセージを表示
				if (((HttpWebResponse)we.Response).StatusCode == HttpStatusCode.Unauthorized)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CMS_FRONT_SITE_UNAUTHORIZED_ERROR);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}
			}
			else
			{
				errorMessage = "ページのチェックでエラーが発生しました。<br />"
					+ "SSL証明書が正しくない等の理由が考えられますため、一度管理者にお問い合わせください。";

				AppLogger.WriteError("URL（" + targetUrl + "）へのアクセスでエラーが発生しました。", we);
			}
			Session[Constants.SESSION_KEY_ERROR_MSG] = (errorMessage.Length > 0) ? errorMessage : "";
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// アクセスエラーメッセージ取得
	/// </summary>
	/// <param name="we">WebException</param>
	/// <returns>エラーメッセージ</returns>
	private string GetAccessErrorMessage(WebException we)
	{
		var errorMessagesTmp = new StringBuilder();

		using (var responseStream = we.Response.GetResponseStream())
		using (var sr = new StreamReader(responseStream, Encoding.UTF8))
		{
			var lineFeedFlg = false;
			while (sr.EndOfStream == false)
			{
				var readedLine = sr.ReadLine();

				// 「<h2> <i>」タグから読み取り開始
				if ((readedLine.Contains(ERROR_CAPTION_TAG) == false) && (errorMessagesTmp.Length == 0))
				{
					continue;
				}
				// 「詳しいコンパイラ出力を表示：～」があれば読み取り終了
				if (readedLine.Contains(ERROR_CAPTION_DETAIL_COMPILER_OUTPUT)) break;

				errorMessagesTmp.Append(readedLine);

				// </code>タグがあれば改行フラグをオフにする
				if (readedLine.Contains(ERROR_CODE_END_TAG))
				{
					lineFeedFlg = false;

					// 読み取り行が</pre>,</code>タグのみ含むかどうかで改行を調整
					if ((readedLine.StartsWith(ERROR_PREFORMATTED_TEXT_END_TAG)) || (readedLine.StartsWith(ERROR_CODE_END_TAG)))
					{
						errorMessagesTmp.Replace(ERROR_CODE_END_TAG, ERROR_CODE_END_TAG + "<br />");
					}
					else
					{
						errorMessagesTmp.Replace(ERROR_CODE_END_TAG, "<br />" + ERROR_CODE_END_TAG + "<br />");
					}
				}

				// <code>～</code>タグ間は改行タグを付与する
				if (lineFeedFlg) errorMessagesTmp.Append("<br />");

				// <code>タグがあれば改行フラグをオンにする
				if (readedLine.Contains(ERROR_CODE_START_TAG)) lineFeedFlg = true;
			}
		}

		// IEでテーブル内で文字列が折り返されないため<pre>～</pre>タグを削除
		errorMessagesTmp.Replace(ERROR_PREFORMATTED_TEXT_START_TAG, "");
		errorMessagesTmp.Replace(ERROR_PREFORMATTED_TEXT_END_TAG, "");

		// エラー情報のルート物理パスを削除
		var errorMessages = errorMessagesTmp.ToString();
		errorMessages = Regex.Replace(errorMessages, this.PhysicaldirpathTargetSite.Replace(@"\", @"\\"), "", RegexOptions.IgnoreCase);
		errorMessages = Regex.Replace(errorMessages, @Constants.PATH_ROOT_FRONT_PC, "", RegexOptions.IgnoreCase);

		return errorMessages;
	}

	/// <summary>
	/// 最終更新者取得
	/// </summary>
	/// <param name="sbFileTextAllSrc"></param>
	protected string GetLastChanged(string sbFileTextAllSrc)
	{
		foreach (Match mPageTag in Regex.Matches(sbFileTextAllSrc.ToString(), TAG_FILEINFO_LASTCHANGED_BGN + ".*?" + TAG_FILEINFO_LASTCHANGED_END))
		{
			return mPageTag.Value.Replace(TAG_FILEINFO_LASTCHANGED_BGN, "").Replace(TAG_FILEINFO_LASTCHANGED_END, "");
		}

		return "";
	}

	/// <summary>
	/// リストボックス選択変更（その他のリストボックスを未選択に）
	/// </summary>
	/// <param name="lbTarget"></param>
	/// <param name="iSelectIndex"></param>
	protected abstract void SelectListBox(ListBox lbTarget, int iSelectIndex);
	/// <summary>
	/// リストボックス選択変更（その他のリストボックスを未選択に）
	/// </summary>
	/// <param name="lbTarget"></param>
	/// <param name="strSelectValue"></param>
	protected abstract void SelectListBox(ListBox lbTarget, string strSelectValue);
	/// <summary>ターゲットファイルパス取得</summary>
	protected abstract string TargetFilePath { get; }
	/// <summary>現在の編集モード</summary>
	protected abstract EditMode CurrentEditMode { get; set; }
	/// <summary>現在の編集ターゲット（既定のもの）</summary>
	protected abstract string CurrentTarget { get; }
}
