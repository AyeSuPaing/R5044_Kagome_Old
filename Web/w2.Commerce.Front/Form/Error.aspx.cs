/*
=========================================================================================================
  Module      : エラー画面処理(Error.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using w2.App.Common;
using w2.Common.Logger;
using w2.Domain.ShortUrl.Helper;
using w2.App.Common.DataCacheController;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;

public partial class Form_Error : BasePage
{
	#region ラップ済みコントロール宣言

	WrappedHtmlGenericControl WdvErrorContents { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvErrorContents"); } }
	WrappedHtmlGenericControl WspGoBack { get { return GetWrappedControl<WrappedHtmlGenericControl>("spGoBack"); } }
	WrappedHtmlAnchor WaGoBack { get { return GetWrappedControl<WrappedHtmlAnchor>("aGoBack"); } }
	WrappedHtmlGenericControl WspGoTop { get { return GetWrappedControl<WrappedHtmlGenericControl>("spGoTop"); } }
	WrappedHtmlGenericControl WhErrorTitle { get { return GetWrappedControl<WrappedHtmlGenericControl>("hErrorTitle"); } }
	WrappedHtmlGenericControl WhErrorContent { get { return GetWrappedControl<WrappedHtmlGenericControl>("hErrorContent"); } }

	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		// ショートURLからロングURL取得
		var longUrl = GetLongUrl();
		if (longUrl != null)
		{
			// SEO対策として、302ではなく、301転送を利用する
			Response.Clear();
			Response.Status = "301 Moved Permanently";
			Response.AddHeader("Location", longUrl);
			Response.End();
		}

		if (!IsPostBack)
		{
			// エラー文言設定
			var errorMessage = GetErrorMessage();
			this.WdvErrorContents.InnerHtml = (errorMessage.Length != 0) ? errorMessage.Replace("\r\n", "<br />") : WebMessages.GetMessages(WebMessages.ERRMSG_SYSTEM_ERROR);

			// Display Title Custom
			DisplayTitleCustom();

			// エラー遷移先切替
			SwitchingTransition();

			// エラーページ用のログを出力
			WriteMessageForErrorPage(errorMessage);

			//カート情報セッションに同梱商品が含まれていたら同梱商品を削除
			DeleteBundledProduct();

		}
	}

	/// <summary>
	/// トップページへ戻る
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbGoTop_Click(object sender, System.EventArgs e)
	{
		Response.Redirect(Constants.PATH_ROOT);
	}

	/// <summary>
	/// ロングURL取得
	/// </summary>
	/// <returns>ロングURL</returns>
	private string GetLongUrl()
	{
		// ショートURL取得
		string shortUrl = null;
		string shortUrlParam = null;
		// IISの404ページ経由の404エラーの場合
		if (this.Is404Error)
		{
			// IISで捕捉された404はここに来る //
			// [例]
			// ・http://xxxxx/s1
			//   → Request.RawUrl は "/RedSurl.aspx?404;http://xxxxx/s1"  ※IIS 6.0は「404;http://xxxxx:80/s1」
			//   → Request.QueryString[0] は "404;http://xxxxx/s1"
			//
			// ・http://xxxxx/s1
			//   → Request.RawUrl は "/RedSurl.aspx?404;http://xxxxx/s1?a=1"
			//   → Request.QueryString.Keys[0]	は "404;http://localhost/w2.Commerce/Front/xxx?a"
			//   → Request.QueryString[0] は "1"

			// IIS6.0の場合のポート番号「:80」等を削除
			string[] shortUrlRaws = Request.Url.AbsoluteUri.Split(';')[1].Split('?');
			shortUrlRaws[0] = Regex.Replace(shortUrlRaws[0], @":\d+/", @"/");

			// 拒否拡張子付きのURLの場合は、404エラーページを表示する
			string[] shortUrlParams = shortUrlRaws[0].Split('/');
			if (shortUrlParams[shortUrlParams.Length - 1].Contains("."))
			{
				string[] splitedFileNames = shortUrlParams[shortUrlParams.Length - 1].Split('.');
				if (Constants.SHORTURL_DENY_EXTENSIONS.Contains((splitedFileNames[splitedFileNames.Length - 1]).ToLower()))
				{
					Response.Clear();
					Response.StatusCode = 404;
					Response.End();
				}
			}

			// 検索対象ショートURL取得
			shortUrl = shortUrlRaws[0];
			if (shortUrlRaws.Length > 1) shortUrlParam = shortUrlRaws[1];
		}
		else
		{
			// ASP.NETで捕捉された404（ショートURLの拡張子がaspxのとき）はここに来る //
			shortUrl = Uri.UriSchemeHttp + Uri.SchemeDelimiter + Request.Url.Authority + Request.Url.AbsolutePath;
			shortUrlParam = (Request.QueryString.ToString() == "") ? null : Request.QueryString.ToString();
		}

		// ショートURL検索用補正
		shortUrl = Regex.Replace(shortUrl, @"^https://", @"http://");
		shortUrl = Regex.Replace(shortUrl, @"\/+$", @"");
		shortUrl = UrlUtility.RemoveProtocolAndDomain(shortUrl);

		// ロングURL取得
		var longUrl = DataCacheControllerFacade.GetShortUrlCacheController().GetLongUrl(shortUrl);
		if (longUrl != null)
		{
			var url = new StringBuilder();

			// 設定されているロングURLへリダイレクト
			longUrl = UrlUtility.AddProtocolAndDomain(longUrl);
			if (longUrl.IndexOf(Uri.UriSchemeHttp) != 0)
			{
				url.Append(Request.Url.Scheme).Append(Uri.SchemeDelimiter).Append(Constants.SITE_DOMAIN).Append(Constants.PATH_ROOT);
			}
			url.Append(longUrl);

			if (shortUrlParam != null)
			{
				url.Append((longUrl.IndexOf('?') == -1) ? "?" : "&").Append(shortUrlParam);
			}

			return url.ToString();
		}

		return null;
	}

	/// <summary>
	/// エラーメッセージ取得
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	private string GetErrorMessage()
	{
		var errorMessage = new StringBuilder();
		switch (this.RequestErrorKbn)
		{
			// 制御文字入力エラー
			case WebMessages.ERRMSG_SYSTEM_VALIDATION_ERROR:
				errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_SYSTEM_VALIDATION_ERROR));
				break;

			// システムエラー（集約エラーハンドラ内ではセッションが使えないこともあるので）
			case WebMessages.ERRMSG_SYSTEM_ERROR:
				errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_SYSTEM_ERROR));
				// UNDONE:IISでStatusCodeが補足されWeb側のエラーページが出力されてしまうのでコメント化（WindowsServer2008 IE7.5で再現）
				// Response.StatusCode = 500;
				break;

			// その他のエラー
			default:
				// 以下の場合404コードを返す
				// ・IISの404ページ経由の404エラー
				// ・アプリケーション経由の404エラー
				// ・商品詳細表示エラー
				if (this.Is404Error
					|| ((Session[Constants.SESSION_KEY_ERROR_MSG] is string)
						&& StringUtility.ToEmpty(Session[Constants.SESSION_KEY_ERROR_MSG]) == WebMessages.GetMessages(WebMessages.ERRMSG_404_ERROR))
					|| ((Session[Constants.SESSION_KEY_ERROR_MSG] is string)
						&& StringUtility.ToEmpty(Session[Constants.SESSION_KEY_ERROR_MSG]) == WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_UNDISP)))
				{
					errorMessage.Append(this.Is404Error ? WebMessages.GetMessages(WebMessages.ERRMSG_404_ERROR) : StringUtility.ToEmpty(Session[Constants.SESSION_KEY_ERROR_MSG]));
					Response.StatusCode = 404;
					Response.TrySkipIisCustomErrors = true;
					break;
				}
				// YamatoKWCのクレジット情報が400日を超えている場合表示
				else if(Session[CommerceMessages.ERRMSG_FRONT_YAMATO_KWC_CREDIT_EXPIRED] != null)
				{
					this.WspGoBack.Visible = false;
					errorMessage.Append((string)Session[CommerceMessages.ERRMSG_FRONT_YAMATO_KWC_CREDIT_EXPIRED]);

					Session.Remove(CommerceMessages.ERRMSG_FRONT_YAMATO_KWC_CREDIT_EXPIRED);
					break;
				}
				// エラーメッセージが存在しない場合はシステムエラー
				else if (Session[Constants.SESSION_KEY_ERROR_MSG] == null)
				{
					errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_SYSTEM_ERROR));
					// UNDONE:IISでStatusCodeが補足されWeb側のエラーページが出力されてしまうのでコメント化（WindowsServer2008 IE7.5で再現）
					// Response.StatusCode = 500;
					break;
				}

				// メッセージが存在する場合はメッセージ表示
				if (Session[Constants.SESSION_KEY_ERROR_MSG] is List<string>)
				{
					foreach (string errorMessageTmp in (List<string>)Session[Constants.SESSION_KEY_ERROR_MSG])
					{
						errorMessage.Append((errorMessage.Length != 0) ? "\r\n" : "");
						errorMessage.Append(errorMessageTmp);
					}
				}
				else
				{
					errorMessage.Append(StringUtility.ToEmpty(Session[Constants.SESSION_KEY_ERROR_MSG]));
				}
				break;
		}
		return errorMessage.ToString();
	}

	/// <summary>
	/// エラー遷移先切替
	/// </summary>
	private void SwitchingTransition()
	{
		switch (this.RequestErrorPageKbn)
		{
			// トップページへ
			case Constants.KBN_REQUEST_ERRORPAGE_GOTOP:
				this.WspGoTop.Visible = true;
				this.WspGoBack.Visible = false;
				break;

			// 戻る
			default:
				// 戻り用URLが付与されている場合、<a>タグのherf属性値を書き換える
				var backUrl = this.RequestBackUrl;
				if (backUrl != "")
				{
					this.WaGoBack.HRef = HttpUtility.HtmlEncode(backUrl);
				}
				break;
		}
	}

	/// <summary>
	/// エラーページ用のログを出力
	/// </summary>
	/// <param name="errorMessage">エラーメッセージ</param>
	/// <remarks>AppLoggerだとEventLoggerも動作するためFileLoggerを使用</remarks>
	private void WriteMessageForErrorPage(string errorMessage)
	{
		var message = new StringBuilder();
		message.Append(this.IsSmartPhone ? "SP" : "PC").Append(" ");
		message.Append(TransformationLog(errorMessage)).Append(" ");
		message.Append((errorMessage != "") ? TransformationLog(this.RequestErrorKbn) : WebMessages.ERRMSG_SYSTEM_ERROR).Append(" ");
		message.Append(TransformationLog(StringUtility.ToEmpty(Request.ServerVariables["REMOTE_ADDR"]))).Append(" ");
		message.Append(TransformationLog(StringUtility.ToEmpty(Request.UserAgent))).Append(" ");
		message.Append(TransformationLog(StringUtility.ToEmpty(Request.UrlReferrer)).ToString().ToLower());

		FileLogger.Write(Constants.INITIALS_ERROR_LOG, message.ToString(), true);
	}

	/// <summary>
	/// ログ用に変換
	/// </summary>
	/// <param name="log">ログ内容</param>
	/// <returns>加工したログ内容</returns>
	private string TransformationLog(string log)
	{
		return (log.Trim() != "") ? log.Replace("\r\n", "").Replace("　", "++").Replace(" ", "+") : "-";
	}

	/// <summary>
	/// Display Title Custom
	/// </summary>
	private void DisplayTitleCustom()
	{
		switch (this.RequestDisptitileKbn)
		{
			case Constants.KBN_DISPTITILE_NONE:
				this.WhErrorTitle.Visible = false;
				this.WhErrorContent.Visible = false;
				break;

			default:
				this.WhErrorTitle.Visible = true;
				this.WhErrorContent.Visible = true;
				break;
		}
	}

	/// <summary>
	/// カート情報セッションの同梱商品を削除
	/// </summary>
	private void DeleteBundledProduct()
	{
		if ((SessionManager.CartList != null)
			&& SessionManager.CartList.Items.SelectMany(item => item.Items).Any(item => item.IsBundle))
		{
			var cartList = SessionManager.CartList.Items.Select(item => item.Items);
			foreach (var cartItem in cartList.Select((value, index) => new { value, index }))
			{
				SessionManager.CartList.Items.ElementAt(cartItem.index).Items.RemoveAll(x => x.IsBundle);
			}
		}
	}

	#region プロパティ
	/// <summary>エラーページ区分</summary>
	private string RequestErrorPageKbn { get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ERRORPAGE_KBN]); } }
	/// <summary>戻りURL</summary>
	private string RequestBackUrl
	{
		get
		{
			var url = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_BACK_URL]);
			// セキュリティ対策のため、自サイトかチェックをする
			// 外部サイトやJSなどのスクリプトだった場合はURLを空にする
			if (UrlUtility.IsSiteDomain(url)
				|| url.StartsWith(Constants.PATH_ROOT))
			{
				return url;
			}

			return string.Empty;
		}
	}
	/// <summary>エラー区分</summary>
	private string RequestErrorKbn { get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ERRORPAGE_FRONT_ERRORKBN]); } }
	/// <summary>IISの404ページ経由の404エラーか？</summary>
	private bool Is404Error
	{
		get
		{
			return ((Request.QueryString.Count != 0)
					&& ((Request.QueryString[0].IndexOf("404;") == 0)
						|| ((Request.QueryString.Keys[0] != null) && (Request.QueryString.Keys[0].IndexOf("404;") == 0))));
		}
	}
	/// <summary>Request Disptitile Type</summary>
	private string RequestDisptitileKbn { get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_DISPTITILE_KBN]); } }
	#endregion
}
