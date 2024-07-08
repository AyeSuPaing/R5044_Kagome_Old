<%--
=========================================================================================================
  Module      : Global.asax
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Application Language="C#" Inherits="System.Web.HttpApplication" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Threading" %>
<%@ Import Namespace="System.Web.Configuration" %>
<%@ Import Namespace="w2.App.Common.DataCacheController" %>
<%@ Import Namespace="w2.App.Common.Util" %>
<%@ Import Namespace="w2.App.Common.Global.Translation" %>
<%@ Import Namespace="w2.App.Common.Order.Cart" %>
<%@ Import Namespace="w2.App.Common.RefreshFileManager" %>
<%@ Import Namespace="w2.Common.Logger" %>
<%@ Import Namespace="w2.Common.Util" %>
<%@ Import Namespace="w2.Common.Web" %>
<%@ Import Namespace="w2.Domain.Order.Setting" %>
<%@ Import Namespace="w2.Domain.FeaturePage" %>

<script RunAt="server">

	/// <summary>
	/// クライアントからのリクエスト受付開始前処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Application_BeginRequest(Object sender, EventArgs e)
	{
		// GETポストバック制限（「__EVENTVALIDATION」がQueryStringに含まれていたらエラーとする）
		if (Request.QueryString["__EVENTVALIDATION"] != null)
		{
			Response.Redirect(Constants.PATH_ROOT);
		}

		// URLリライト処理
		string strRealUrl = null;	// "~/～"形式で格納される

		// フレンドリーURLからリアルURL取得（IIS6系と7系とで挙動が違うので注意）
		if (Constants.FRIENDLY_URL_ENABLED)
		{
			if (Request.Url.AbsoluteUri.Contains("?404;"))	// 必ず404で来るはず
			{
				string strRealUrlTmp = FriendlyUrlUtility.GetRealUrl(Request);
				if (strRealUrlTmp != null)
				{
					strRealUrl = strRealUrlTmp;
				}
			}
		}

		// スマートフォンURL取得
		if (Constants.SMARTPHONE_OPTION_ENABLED)
		{
			if ((Request.Url.AbsoluteUri.EndsWith(".css") == false)	// *.cssアクセスは除外
				&& (Request.Url.AbsoluteUri.Contains(".axd?") == false)	// *.axdアクセスは除外
				&& (Request.Url.AbsoluteUri.Contains(Constants.PAGE_FRONT_VALIDATE_SCRIPT) == false))	// ValidateScriptAspxは除外
			{
				if ((Request.QueryString[Constants.REQUEST_KEY_CHANGESITE] != Constants.KBN_REQUEST_CHANGESITE_PC)
					&& ((Request.QueryString[Constants.REQUEST_KEY_CHANGESITE] == Constants.KBN_REQUEST_CHANGESITE_SMARTPHONE)
						|| (CookieManager.Get(Constants.COOKIE_KEY_SMARTPHONE_SITE) == null)	// PCサイトクッキーを持ってたら除外
						|| (CookieManager.GetValue(Constants.COOKIE_KEY_SMARTPHONE_SITE) != Constants.KBN_REQUEST_CHANGESITE_PC)))	// スマートフォン指定あれば対象
				{
					string strRealUrlTmp = SmartPhoneUtility.GetSmartPhoneUrl(
						strRealUrl ?? Request.AppRelativeCurrentExecutionFilePath,
						Request.UserAgent,
						HttpContext.Current);	// 管理者なら可能とする

					if (strRealUrlTmp != null)
					{
						strRealUrl = strRealUrlTmp;
					}
				}
			}

			// カスタムページ PC・SP表示制御
			var customPageRx = new Regex(string.Format(@"^~\/{0}\/(?!.*\/).*\.aspx$", Constants.PATH_CONTENTS_PAGE.Replace("/", "")), RegexOptions.Compiled);
			var isCustomPage = customPageRx.IsMatch(Request.AppRelativeCurrentExecutionFilePath);
			if (isCustomPage)
			{
				var fileName = StringUtility.ToEmpty(Path.GetFileName(Request.AppRelativeCurrentExecutionFilePath));
				var model = DataCacheControllerFacade.GetPageDesignCacheController().CacheData
					.FirstOrDefault(m => (m.PageType == Constants.FLG_PAGEDESIGN_PAGE_TYPE_CUSTOM)
						&& (String.Equals(m.FileName, fileName, StringComparison.CurrentCultureIgnoreCase)));

				if (model != null)
				{
					strRealUrl = DevicePageChange(
						(model.UseType == Constants.FLG_PAGEDESIGN_USE_TYPE_PC),
						(model.UseType == Constants.FLG_PAGEDESIGN_USE_TYPE_SP),
						strRealUrl);
				}
			}

			// 特集ページ PC・SP表示制御
			var featurePageRx = new Regex(
				string.Format(
					@"^~\/{0}\/(?!.*\/).*\.aspx$",
					Constants.PATH_FEATURE_PAGE),
					RegexOptions.Compiled);

			var isFeaturePage = featurePageRx.IsMatch(this.Request.AppRelativeCurrentExecutionFilePath);
			if (isFeaturePage)
			{
				var fileName = StringUtility.ToEmpty(Path.GetFileName(this.Request.AppRelativeCurrentExecutionFilePath));
				var model = DataCacheControllerFacade
					.GetFeaturePageCacheController()
					.CacheData
					.FirstOrDefault(item =>
						String.Equals(
							item.FileName, fileName,
							StringComparison.CurrentCultureIgnoreCase));

				if (model != null)
				{
					strRealUrl = DevicePageChange(
						(model.UseType == Constants.FLG_PAGEDESIGN_USE_TYPE_PC),
						(model.UseType == Constants.FLG_PAGEDESIGN_USE_TYPE_SP),
						strRealUrl);
				}
			}
		}

		// URLルートパスを設定する
		Constants.URL_FRONT_PC =
			(Constants.PATH_ROOT_FRONT_PC.StartsWith("/") ? (Constants.PROTOCOL_HTTP + Request.Url.Authority) : "")
			+ Constants.PATH_ROOT_FRONT_PC;

		Constants.URL_FRONT_PC_SECURE =
			(Constants.PATH_ROOT_FRONT_PC.StartsWith("/") ? (Constants.PROTOCOL_HTTPS + Request.Url.Authority) : "")
				+ Constants.PATH_ROOT_FRONT_PC;

		// URL書き換え
		if (strRealUrl != null)
		{
			Context.RewritePath(strRealUrl);
		}

		// GETポストバック制限（「__EVENTVALIDATION」がQueryStringに含まれていたらエラーとする）
		if (Request.QueryString["__EVENTVALIDATION"] != null)
		{
			Response.Redirect(Constants.PATH_ROOT);
		}
	}

	/// <summary>
	/// アプリケーション起動時の処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	void Application_Start(object sender, EventArgs e)
	{
		// 仮のログ出力先を設定
		Constants.PHYSICALDIRPATH_LOGFILE = Path.Combine(
			WebConfigurationManager.AppSettings["ConfigFileDirPath"],
			@"Logs",
			WebConfigurationManager.AppSettings["Application_Name"]);

		// パッケージ初期化
		InitializePackage();

		FileLogger.WriteInfo("設定読み込み完了");

		// コンフィグ監視起動（パッケージ初期化処理セット）
		FileUpdateObserver.GetInstance().AddObservationWithSubDir(
			WebConfigurationManager.AppSettings["ConfigFileDirPath"] + @"AppConfig\",
			"*.*",
			InitializePackage);

		FileLogger.WriteInfo("Config監視起動完了");

		// スマートフォンXML監視セット＆UPDATE
		if (Constants.SMARTPHONE_OPTION_ENABLED)
		{
			// 監視セット
			FileUpdateObserver.GetInstance().AddObservation(
				Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE + SmartPhoneUtility.DIRPATH_SMARTPHONE_SITE_SETTING,
				SmartPhoneUtility.FILENAME_SMARTPHONE_SITE_SETTING,
				SmartPhoneUtility.UpdateSetting);

			// UPDATE
			SmartPhoneUtility.UpdateSetting();

			FileLogger.WriteInfo("SP監視起動完了");
		}

		// デバッグ対象ページ設定XML＆UPDATE（監視セット）
		FileUpdateObserver.GetInstance().AddObservation(
			Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE + DebugLogProvider.DIRPATH_DEBUGLOG_SETTING,
			DebugLogProvider.FILENAME_DEBUGLOG_TARGET_PAGES,
			DebugLogProvider.UpdateSetting);
		DebugLogProvider.UpdateSetting();

		FileLogger.WriteInfo("デバッグ監視起動完了");

		// 配送先マッチング設定監視
		FileUpdateObserver.GetInstance().AddObservationWithSubDir(
			Path.Combine(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE, @"Settings"),
			@"OrderShippingMatchingSetting.xml",
			OrderShippingMatchingSetting.UpdateSetting);

		FileLogger.WriteInfo("配送先マッチング設定監視起動完了");

		// メンテナンスリフレッシュファイル監視
		RefreshFileManagerProvider.GetInstance(RefreshFileType.Maintenance).AddObservation(HttpRuntime.UnloadAppDomain);

		FileLogger.WriteInfo("メンテナンスリフレッシュファイル監視起動完了");

		// 監視スレッド生成
		ObservationModule.CreateObserverThread();

		FileLogger.WriteInfo("監視スレッド生成完了");

		// 自動翻訳API利用時、利用状況監視スレッドを起動
		TranslationManager.WorkerRun();

		FileLogger.WriteInfo("翻訳監視起動完了");

		// アプリケーション開始をログ出力
		FileLogger.WriteInfo("アプリケーション開始");
	}

	/// <summary>
	/// アプリケーション終了時の処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	void Application_End(object sender, EventArgs e)
	{
	}

	/// <summary>
	/// セッション開始時の処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	void Session_Start(object sender, EventArgs e)
	{
		// セッションにダミー値を格納
		// （セッションに何か格納しないとセッションクッキーが発行されず、Session.SessionIDが定まらない為）
		Session["__DummyValueToFixSessionID__"] = string.Empty;

		// 特定UAの場合はSameSiteをLaxに変更(chrome version80対応)
		if ((string.IsNullOrEmpty(Request.UserAgent) == false) && Regex.IsMatch(Request.UserAgent, Constants.DISALLOW_SAMESITE_NONE_USERAGENTSPATTERN))
		{
			CookieManager.SetCookieToLax(Constants.SESSION_COOKIE_NAME);
		}
	}

	/// <summary>
	/// セッション終了時の処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks>
	/// Web.config ファイル内で sessionstate モードが InProc に設定されているときのみSession_End イベントが発生します。
	/// session モードが StateServer か、または SQLServer に設定されている場合イベントは発生しません。
	/// </remarks>
	void Session_End(object sender, EventArgs e)
	{
	}

	/// <summary>
	/// ASP.NET がイベント ハンドラ実行開始
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Application_PreRequestHandlerExecute(Object sender, EventArgs e)
	{
		// デバッグログ出力
		DebugLogProvider.Write(HttpContext.Current,
			new string[] { Constants.SESSION_KEY_LOGIN_USER_ID },
			DebugLogProvider.Site.Pc);

		// ログイン中かつhttp通信の場合、httpsでリダイレクトする
		//   *.axdファイルアクセスの場合はSessionが空
		//   DownloadLink.aspxの場合はリダイレクトしない（大文字小文字を区別しない）
		if (HttpContext.Current.Session != null && (Request.Url.AbsolutePath.ToLower().Contains("downloadlink.aspx") == false))
		{
			// SSLを利用する時のみhttpsでリダイレクトする
			if (Constants.PROTOCOL_HTTP != Constants.PROTOCOL_HTTPS)
			{
				BasePage basePage = (Context.Handler is BasePage) ? (BasePage)Context.Handler : null;

				// http かつ 保護が必要な状態であれば セッション張り直してhttps遷移
				if ((Request.IsSecureConnection == false)
					&& (((Constants.ALLOW_HTTP_AFTER_LOGGEDIN == false) && SessionSecurityManager.HasCriticalInformation(Context))
						|| (Constants.ALLOW_HTTP_AFTER_LOGGEDIN && (basePage != null) && basePage.NeedsHttps)))
				{
					// セッション張り直しのためのデータ格納（セッションハイジャック対策）
					SessionSecurityManager.SaveSesstionContetnsToDatabaseForChangeSessionId(Request, Response, Session);

					// セッション復元ページへリダイレクト（遷移先をパラメータで渡す）
					StringBuilder sbUrl = new StringBuilder();
					sbUrl.Append(SessionSecurityManager.GetSecurePageProtocolAndHost()).Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_RESTORE_SESSION);
					sbUrl.Append("?").Append(Constants.REQUEST_KEY_NEXT_URL).Append("=").Append(Server.UrlEncode(Request.Url.AbsoluteUri.Replace(Constants.PROTOCOL_HTTP, Constants.PROTOCOL_HTTPS)));
					sbUrl.Append("&").Append(Constants.REQUEST_KEY_LOGIN_FLG).Append("=1");

					Response.Redirect(sbUrl.ToString());
				}
				// https かつ ログイン後Http許可 かつ http表示ページ であればhttpへ遷移
				else if ((Request.IsSecureConnection
					&& (Constants.ALLOW_HTTP_AFTER_LOGGEDIN)
					&& ((basePage != null) && basePage.NeedsHttp)))
				{
					Response.Redirect(Constants.PROTOCOL_HTTP + Constants.SITE_DOMAIN + Request.RawUrl.Replace("#", HttpUtility.UrlEncode("#", Encoding.UTF8)));
				}
			}
		}
	}

	/// <summary>
	/// ASP.NET がレスポンスを返す直前に実行するイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Application_OnPreSendRequestHeaders(Object sender, EventArgs e)
	{
		// ServerがWindowsVista以上のOSの場合、レスポンスヘッダのサーバー項目を削除（それ以下のOSの場合はツールを使用して削除）
		if (Environment.OSVersion.Version.Major > 5)
		{
			Response.Headers.Remove("Server");	// この操作を実行するには、IIS統合パイプライン モードが必要です
		}
	}

	/// <summary>
	/// ページの実行を開始する直前に発生
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Application_OnPreRequestHandlerExecute(Object sender, EventArgs e)
	{
		if (Constants.LOGGING_PERFORMANCE_REQUEST_ENABLED == false) return;
		if (Request.Url.AbsolutePath.EndsWith(Constants.PAGE_FRONT_VALIDATE_SCRIPT)) return;
		if (HttpContext.Current == null) return;
		if (HttpContext.Current.Session == null) return;
		HttpContext.Current.Session[Constants.SESSION_KEY_ON_PRE_REQUEST_HANDLER_EXECUTE_DATETIME] = DateTime.Now;
	}

	/// <summary>
	/// ページの実行を完了した直後に発生
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Application_OnPostRequestHandlerExecute(Object sender, EventArgs e)
	{
		if (Constants.LOGGING_PERFORMANCE_REQUEST_ENABLED == false) return;
		if (Request.Url.AbsolutePath.EndsWith(Constants.PAGE_FRONT_VALIDATE_SCRIPT)) return;
		if (HttpContext.Current == null) return;
		if (HttpContext.Current.Session == null) return;
		if (HttpContext.Current.Session[Constants.SESSION_KEY_ON_PRE_REQUEST_HANDLER_EXECUTE_DATETIME] == null) return;
		var begin = (DateTime)HttpContext.Current.Session[Constants.SESSION_KEY_ON_PRE_REQUEST_HANDLER_EXECUTE_DATETIME];
		PerformanceLogger.WriteForRequest(begin, DateTime.Now, Request.Url.PathAndQuery);
	}

	/// <summary>
	/// 集約エラーハンドラ
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Application_Error(Object sender, EventArgs e)
	{
		// 最後に発生したエラー原因情報をExceptionオブジェクトとして取得
		Exception objErr = Server.GetLastError();
		if (objErr == null) return;

		// 制御文字入力エラー（このエラーの場合だけValidateModuleの場合は文字列を出力する）
		if (objErr.GetBaseException() is System.Web.HttpRequestValidationException)
		{
			// エラーをクリア（しないとIIS7以降でエラー画面へ遷移できない）
			this.Server.ClearError();

			if (Request.Url.AbsolutePath.Contains(Constants.PATH_ROOT + Constants.PAGE_FRONT_VALIDATE_MODULE))
			{
				Response.Write(WebSanitizer.HtmlEncode(WebMessages.GetMessages(WebMessages.ERRMSG_SYSTEM_VALIDATION_ERROR)));
			}
			else
			{
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR + "?" + Constants.REQUEST_KEY_ERRORPAGE_FRONT_ERRORKBN + "=" + HttpUtility.UrlEncode(WebMessages.ERRMSG_SYSTEM_VALIDATION_ERROR));
			}
			return;
		}
		else if (objErr is HttpException)
		{
			// Exception invalid viewstate
			var httpException = (HttpException)objErr;
			if ((objErr.GetBaseException() is System.Web.UI.ViewStateException)
				|| (httpException.WebEventCode == System.Web.Management.WebEventCodes.AuditInvalidViewStateFailure)
				|| (httpException.WebEventCode == System.Web.Management.WebEventCodes.InvalidViewState)
				|| (httpException.WebEventCode == System.Web.Management.WebEventCodes.InvalidViewStateMac)
				|| (httpException.WebEventCode == System.Web.Management.WebEventCodes.RuntimeErrorViewStateFailure))
			{
				if (Response.StatusDescription != Constants.ASYNC_POSTBACK_ERROR) FileLogger.WriteError(objErr);
				this.Server.ClearError();

				Response.Redirect(HttpContext.Current.Request.Url.AbsoluteUri);
				return;
			}

			// HttpException -> HttpParseException の継承関係なので、HttpParseExceptionを先に捕まえる
			if (objErr is HttpParseException)
			{
				// アクセスされたページから参照されているファイル(.ascx)が存在しない場合は、
				// HttpParseException で HttpCode 404になる。（つまりここに含まれる）
				// この場合は、ページ404として扱ってはいけないし、ショートURL処理を行ってもいけない。
				// そしてエラーログを出さなければならない。

				var errHttpParse = objErr as HttpParseException;

				var message = new StringBuilder();
				message.AppendFormat("HTTPパーサーエラー: {0}", errHttpParse.Message).AppendLine();

				// 要求URL（クライアントがアクセスしたパス）
				message.AppendFormat("  要求URL: {0}", Request.RawUrl ?? "(null)").AppendLine();
				// 実行URL（実際に実行されたパス）
				message.AppendFormat("  実行URL: {0}", Request.Url.PathAndQuery ?? "(null)").AppendLine();
				// 発生URL（エラーが起きたパス）
				message.AppendFormat("  発生URL: {0}", errHttpParse.VirtualPath ?? "(null)").AppendLine();
				// 発生行数（エラーが起きた行）
				message.AppendFormat("  発生行数: {0}", errHttpParse.Line).AppendLine();
				message.AppendFormat("  IPアドレス: {0}", Request.UserHostAddress ?? "").AppendLine();
				message.AppendFormat("  User-Agent: {0}", Request.UserAgent ?? "").AppendLine();
				FileLogger.WriteError(message.ToString(), objErr);
			}
			else
			{
				// 404ならショートURLリダイレクト試行
				int iHttpCode = ((HttpException)objErr).GetHttpCode();
				if (iHttpCode == 404)
				{
					// エラーをクリア（しないとIIS7以降でaspxファイルのショートURL機能が使えない）
					Context.ClearError();

					Response.StatusCode = 404;
					if (HttpRuntime.UsingIntegratedPipeline) Response.SubStatusCode = 0;

					Response.End();
				}

				// 403,404エラーでなければエラーログ出力
				if ((iHttpCode != 404) && (iHttpCode != 403))
				{
					var message = new StringBuilder();
					message.AppendLine("HTTPエラー: " + objErr.Message);
					message.AppendLine("  要求URL: " + (w2.Common.Web.WebUtility.GetRawUrl(Request) ?? "(null)"));
					message.AppendLine("  実行URL: " + (Request.Url.PathAndQuery ?? "(null)"));
					message.AppendLine("  IPアドレス: " + (Request.UserHostAddress ?? "(null)"));
					message.AppendLine("  User-Agent: " + (Request.UserAgent ?? "(null)"));

					// ログインユーザー情報
					string userId;
					try
					{
						userId = (string)Session[Constants.SESSION_KEY_LOGIN_USER_ID] ?? "(未ログイン)";
					}
					catch
					{
						userId = "(取得失敗)";
					}
					message.AppendLine("  ユーザー情報: " + userId);

					// ログファイル記述
					AppLogger.WriteError(message.ToString(), objErr);
				}
			}

			// プレビューページの判定
			var isPreviewPage = Preview.IsPreview(Request);

			// プレビュー許可IPアドレスかつプレビューページの場合は、詳細なエラー内容を送る
			if (IsPreviewAddr(Request) && isPreviewPage)
			{
				// ステータスは500で返すがIISのエラーページは使用しない
				Context.ClearError();
				Response.StatusCode = 500;
				Response.TrySkipIisCustomErrors = true;

				Response.ContentType = "text/html;charset=utf-8";

				// キャッシュ無効
				Response.Cache.SetCacheability(HttpCacheability.NoCache);
				Response.DisableKernelCache();

				// ランタイムエラーのHTMLを送る（但し本物のランタイムエラーと区別するため背景色を変える）
				var errHttp = objErr as HttpException;
				Response.Write(errHttp.GetHtmlErrorMessage().Replace("#ffffcc", "#ffcccc"));
				return;
			}

			// エラーページまたはプレビューページで例外が発生した場合はエラーページへ飛ばない
			var isErrorPage = Request.Path.ToLower().StartsWith(Constants.PATH_ROOT.ToLower() + Constants.PAGE_FRONT_ERROR.ToLower());
			if (isErrorPage || isPreviewPage) return;

			// 海外送料エラーの場合、特別なメッセージを出すようここでまとめて制御
			if (objErr.GetBaseException() is GlobalShippingPriceCalcException)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CALC_SHIPPING_ERROR).Replace("@@ 1 @@", objErr.GetBaseException().Message);
				// エラーページへの遷移
				RedirectErrorPage(false);
				return;
			}

#if !DEBUG
			// エラーページへの遷移
			RedirectErrorPage();
			return;
#endif
		}
		else
		{
			FileLogger.WriteError(objErr);

#if !DEBUG
			// エラーページへの遷移
			RedirectErrorPage();
			return;
#endif
		}
	}

	/// <summary>
	/// エラーページへの遷移
	/// </summary>
	/// <param name="isDisplaySystemError">システムエラー表示のか</param>
	private void RedirectErrorPage(bool isDisplaySystemError = true)
	{
		// エラーをクリア（しないとIIS7以降でエラー画面へ遷移できない）
		this.Context.ClearError();

		// エラーページのURL作成
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		if (isDisplaySystemError)
		{
			urlCreator.AddParam(
				Constants.REQUEST_KEY_ERRORPAGE_FRONT_ERRORKBN,
				HttpUtility.UrlEncode(WebMessages.ERRMSG_SYSTEM_ERROR));
		}
		var url = urlCreator.CreateUrl();

		// エラーページへ
		this.Response.Redirect(url);
	}

	/// <summary>
	/// プレビュー許可IPアドレスか
	/// </summary>
	/// <param name="request">HttpRequest</param>
	/// <returns>プレビュー許可IPアドレスの場合はtrue</returns>
	private bool IsPreviewAddr(HttpRequest request)
	{
		// 既定のローカル判定ロジック
		if (request.IsLocal) return true;
		// 上記で漏れるアドレス及びカスタムアドレス分を判定
		var filePath = Server.MapPath("~/PreviewAddr.conf");
		if (System.IO.File.Exists(filePath))
		{
			var content = System.IO.File.ReadAllText(filePath);
			var allowAddrs = Regex.Split(content ?? "", @"\s+");
			return allowAddrs.Contains(request.UserHostAddress);
		}
		return false;
	}

	/// <summary>
	/// 利用端末による表示制限によりページのURL切り替え
	/// </summary>
	/// <param name="isPcOnly">PCのみ表示ページか?</param>
	/// <param name="isSpOnly">SPのみ表示ページか?</param>
	/// <param name="accessUrl">アクセスURL</param>
	/// <returns>表示するページURL</returns>
	private string DevicePageChange(bool isPcOnly, bool isSpOnly, string accessUrl)
	{
		var resultUrl = (accessUrl ?? Request.AppRelativeCurrentExecutionFilePath);
		var isSpAccess = SmartPhoneUtility.SmartPhoneSiteSettings.Any(s => (string.IsNullOrEmpty(s.RootPath) == false) && resultUrl.Contains(s.RootPath));
		// PCのみ表示でSPからのアクセスの場合 PC画面表示
		if (isPcOnly && isSpAccess)
		{
			foreach (var spSetting in SmartPhoneUtility.SmartPhoneSiteSettings)
			{
				if ((string.IsNullOrEmpty(spSetting.RootPath) == false) && resultUrl.Contains(spSetting.RootPath) && spSetting.SmartPhonePageEnabled)
				{
					resultUrl = resultUrl.Replace(spSetting.RootPath, "~/");
					break;
				}
			}
		}

		// SPのみ表示でPCからのアクセスの場合 SP画面表示
		if (isSpOnly && (isSpAccess == false))
		{
			var targetSetting = SmartPhoneUtility.SmartPhoneSiteSettings.FirstOrDefault();
			if (targetSetting != null)
			{
				resultUrl = resultUrl.Replace("~/", targetSetting.RootPath);
			}
		}

		return resultUrl;
	}

	/// <summary>
	/// アプリケーション初期化
	/// </summary>
	private void InitializePackage()
	{
		lock (Application)
		{
			try
			{
				//------------------------------------------------------
				// セッション設定取得
				//------------------------------------------------------
				SessionStateSection sssSessionConfig = (SessionStateSection)WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath).GetSection("system.web/sessionState");
				Constants.SESSION_COOKIE_NAME = sssSessionConfig.CookieName;

				//------------------------------------------------------
				// アプリケーション設定読み込み
				//------------------------------------------------------
				// アプリケーション名
				Constants.APPLICATION_NAME = WebConfigurationManager.AppSettings["Application_Name"];
				
				// アプリケーション共通の設定			
				w2.App.Common.ConfigurationSetting csSetting
					= new w2.App.Common.ConfigurationSetting(
						WebConfigurationManager.AppSettings["ConfigFileDirPath"],
						w2.App.Common.ConfigurationSetting.ReadKbn.C000_AppCommon,
						w2.App.Common.ConfigurationSetting.ReadKbn.C100_SiteCommon,
						w2.App.Common.ConfigurationSetting.ReadKbn.C200_CommonFront,
						w2.App.Common.ConfigurationSetting.ReadKbn.C300_Pc,
						w2.App.Common.ConfigurationSetting.ReadKbn.C200_OrderPointBatch);

				//------------------------------------------------------
				// アプリケーション固有の設定
				//------------------------------------------------------
				Constants.PATH_ROOT = Constants.PATH_ROOT_FRONT_PC;
				Constants.PATH_MOBILESITE = Constants.PATH_ROOT_FRONT_MOBILE;
				Constants.ALLOW_HTTP_AFTER_LOGGEDIN = csSetting.GetAppBoolSetting("Site_AllowHttpAfterLoggedin_Enabled");
				Constants.SESSIONCOOKIE_SECURE_ENABLED = csSetting.GetAppBoolSetting("Site_SessionCookie_Secure_Enabled");
				// アフィリエイト
				Constants.AFFILIATE_LINKSHARE_COOKIE_LIMIT_DAYS = csSetting.GetAppIntSetting("Affiliate_LinkShare_Cookie_LimitDays");
				Constants.AFFILIATE_LINKSHARE_VALID = csSetting.GetAppBoolSetting("Affiliate_LinkShare_Valid");

				// ユーザー情報連携OP
				Constants.USER_COOPERATION_ENABLED = csSetting.GetAppBoolSetting("User_Cooperation_Enabled");

				// 一覧表示系の設定
				Constants.CONST_DISP_CONTENTS_DEFAULT_LIST = csSetting.GetAppIntSetting("Const_DispListContentsCount_Default");
				Constants.CONST_DISP_CONTENTS_PRODUCT_LIST_IMG_ON = csSetting.GetAppIntSetting("Const_DispListContentsCount_ProductList_ImgOn");
				Constants.CONST_DISP_CONTENTS_PRODUCT_LIST_WINDOWSHOPPING = csSetting.GetAppIntSetting("Const_DispListContentsCount_ProductList_WindowShopping");
				Constants.CONST_DISP_CONTENTS_PRODUCTREVIEW_LIST = csSetting.GetAppIntSetting("Const_DispListContentsCount_ProductReviewList");
				Constants.CONST_DISP_CONTENTS_PRODUCTVARIATION_LIST = csSetting.GetAppIntSetting("Const_DispListContentsCount_ProductVariationList");
				Constants.CONST_PRODUCTHISTORY_HOLD_COUNT = csSetting.GetAppIntSetting("Const_ProductDispHistory_HoldCount");
				Constants.NUMBER_DISPLAY_LINKS_PRODUCT_LIST = csSetting.GetAppIntSettingList("NumberDisplayLinks_ProductList");
				Constants.LAYER_DISPLAY_VARIATION_IMAGES_ENABLED = csSetting.GetAppBoolSetting("Layer_Display_Variation_Images_Enabled");
				Constants.LAYER_DISPLAY_NOVARIATION_UNDISPLAY_ENABLED = csSetting.GetAppBoolSetting("Layer_Display_NoVariation_UnDisplay_Enabled");
				Constants.LAYER_DISPLAY_VARIATION_GROUP_NAME = csSetting.GetAppStringSetting("Layer_Display_Variation_Group_Name");
				Constants.PHYSICALDIRPATH_CONTENTS = AppDomain.CurrentDomain.BaseDirectory + @"Contents\";

				// 商品詳細系の設定
				Constants.PRODUCTDETAIL_VARIATION_SINGLE_SELECTED = csSetting.GetAppBoolSetting("ProductDetail_Variation_Single_Selected");

				//商品バリエーション取得設定
				Constants.PRODUCTLIST_VARIATION_DISPLAY_ENABLED = csSetting.GetAppBoolSetting("ProductList_Variation_Display_Enabled");
				Constants.PRODUCT_RECOMMEND_VARIATION_DISPLAY_ENABLED = csSetting.GetAppBoolSetting("Product_Recommend_Variation_Display_Enabled");

				// パフォーマンス：商品一覧キャッシング時間（分。「0」はキャッシングしない）
				Constants.PRODUCTLIST_CACHE_EXPIRE_MINUTES = csSetting.GetAppIntSetting("ProductList_CacheExpireMinutes");
				// パフォーマンス：コーディネート一覧キャッシング時間（分。「0」はキャッシングしない）
				Constants.COORDINATELIST_CACHE_EXPIRE_MINUTES = csSetting.GetAppIntSetting("CoordinateList_CacheExpireMinutes");

				// パフォーマンス：商品ランキングキャッシング時間（分。「0」はキャッシングしない）
				Constants.PRODUCT_RANKING_CACHE_EXPIRE_MINUTES = csSetting.GetAppIntSetting("ProductRanking_CacheExpireMinutes");

				// パフォーマンス：おすすめ商品キャッシング時間（分。「0」はキャッシングしない）
				Constants.PRODUCT_RECOMMEND_CACHE_EXPIRE_MINUTES = csSetting.GetAppIntSetting("ProductRecommend_CacheExpireMinutes");

				// パフォーマンス：おすすめ商品(詳細)キャッシング時間（分。「0」はキャッシングしない））
				Constants.PRODUCT_RECOMMEND_ADVANCED_CACHE_EXPIRE_MINUTES = csSetting.GetAppIntSetting("ProductRecommendAdvanced_CacheExpireMinutes");

				// マイページに受信メール履歴の表示設定
				Constants.MYPAGE_RECIEVEMAIL_HISTORY_DISPLAY = csSetting.GetAppBoolSetting("MyPage_RecieveMail_History_Display");

				// マイページ：会員ランクアップの表示設定
				Constants.MYPAGE_MEMBERRANKUP_DISPLAY = csSetting.GetAppBoolSetting("MyPage_MemberRankUp_Display");
				// マイページ：スケジュール実行が１回のみの会員ランクアップの表示設定
				Constants.MYPAGE_SCHEDULE_KBN_ONCE_MEMBERRANKUP_DISPLAY = csSetting.GetAppBoolSetting("MyPage_Schedule_Kbn_Once_MemberRankUp_Display");

				// アクセスログ
				Constants.W2MP_ACCESSLOG_ACCOUNT_ID = csSetting.GetAppStringSetting("AccessLog_AccountId");
				Constants.W2MP_ACCESSLOG_GETLOG_PATH = csSetting.GetAppStringSetting("AccessLog_GetlogPath");
				Constants.W2MP_ACCESSLOG_TARGET_DOMAIN = csSetting.GetAppStringSetting("AccessLog_TargetDomain");
				Constants.W2MP_ACCESSLOG_TRACKER_PATH = csSetting.GetAppStringSetting("AccessLog_TrackerPath");

				//------------------------------------------------------
				// ポイント
				//------------------------------------------------------
				Constants.ORDER_POINT_BATCH_CHANGE_TEMP_TO_COMP_ENABLED = csSetting.GetAppBoolSetting("OrderPointBatch_ChangeTempToComp_Enabled");
				Constants.ORDER_POINT_BATCH_POINT_TEMP_TO_COMP_DAYS = csSetting.GetAppIntSetting("OrderPointBatch_PointTempToCompDays");
				Constants.ORDER_POINT_BATCH_POINT_TEMP_TO_COMP_LIMITED_TERM_POINT_DAYS
					= csSetting.GetAppIntSetting("OrderPointBatch_PointTempToCompLimitedTermPointDays");

				// Webキャプチャー用許可IPアドレス
				Constants.ALLOWED_IP_ADDRESS_FOR_WEBCAPTURE = csSetting.GetAppStringSetting("Allowed_IP_Address_For_Webcapture");
			}
			catch (Exception ex)
			{
				HttpRuntime.UnloadAppDomain();
				throw new System.ApplicationException("Configファイルの読み込みに失敗しました。", ex);
			}
		}
	}
</script>
