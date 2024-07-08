<%--
=========================================================================================================
  Module      : Global.asax
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Application Language="C#" Inherits="System.Web.HttpApplication" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Web.Configuration" %>
<%@ Import Namespace="w2.Common.Logger" %>
<%@ Import Namespace="w2.Common.Util" %>
<%@ Import Namespace="w2.Common.Web" %>
<%@ Import Namespace="w2.Domain.ShopOperator" %>

<script runat="server">

	/// <summary>
	/// クライアントからのリクエスト受付開始前処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Application_BeginRequest(Object sender, EventArgs e)
	{
		// 設定XMLは外部から読み込めないようにする
		if (Request.Url.AbsolutePath.ToLower().EndsWith(".xml"))
		{
			if (Request.Url.AbsolutePath.ToLower().StartsWith((Constants.PATH_ROOT + Constants.PATH_SETTING_XML).ToLower()))
			{
				Response.Redirect(Constants.PATH_ROOT);
			}
		}
		
		// URLルートパスを設定する（ＰＣ、モバイル）
		if (Constants.PATH_ROOT_FRONT_PC.StartsWith("/"))
		{
			Constants.URL_FRONT_PC = Constants.PROTOCOL_HTTP + Request.Url.Authority + Constants.PATH_ROOT_FRONT_PC;
			Constants.URL_FRONT_PC_SECURE = Constants.PROTOCOL_HTTPS + Request.Url.Authority + Constants.PATH_ROOT_FRONT_PC;
		}
		else
		{
			Constants.URL_FRONT_PC = Constants.PATH_ROOT_FRONT_PC;
			Constants.URL_FRONT_PC_SECURE = Constants.PATH_ROOT_FRONT_PC;
		}

		if (Constants.PATH_ROOT_FRONT_MOBILE.StartsWith("/"))
		{
			Constants.URL_FRONT_MOBILE = Constants.PROTOCOL_HTTP + Request.Url.Authority + Constants.PATH_ROOT_FRONT_MOBILE;
		}
		else
		{
			Constants.URL_FRONT_MOBILE = Constants.PATH_ROOT_FRONT_MOBILE;
		}
	}

	/// <summary>
	///  リクエストに関連した状態（セッション等）取得時処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Application_AcquireRequestState(Object sender, EventArgs e)
	{
		// 一括ログインチェック
		HttpContext hc = HttpContext.Current;
		if (hc.Session != null)	// TreeView等でSessionなしのリクエストがある
		{
			if ((string.Compare(hc.Request.FilePath, Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR, true) != 0)
				&& (string.Compare(hc.Request.FilePath, Constants.PATH_ROOT + Constants.PAGE_MANAGER_LOGIN, true) != 0)
				&& (string.Compare(hc.Request.FilePath, Constants.PATH_ROOT + Constants.PAGE_MANAGER_OPERATOR_PASSWORD_CHANGE_COMPLETE, true) != 0)
				&& (string.Compare(hc.Request.FilePath, Constants.PATH_ROOT + Constants.PAGE_MANAGER_MASTERIMPORT_OUTPUTLOG, true) != 0))
			{
				if (Constants.CS_OPTION_ENABLED == false)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages
						.GetMessages(WebMessages.ERRMSG_MANAGER_CS_OPTION_DISABLED);
					var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR).CreateUrl();
					Response.Redirect(url);
				}

				if (hc.Session[Constants.SESSION_KEY_LOGIN_SHOP_OPERTOR] == null)
				{
					if (string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_CUSTOMER_TELNO]) == false)
					{
						var loginPageUrl = new StringBuilder();
						loginPageUrl.Append(Constants.PATH_ROOT + Constants.PAGE_MANAGER_LOGIN);
						loginPageUrl.Append("?");
						loginPageUrl.Append(Constants.REQUEST_KEY_MANAGER_LOGIN_ID).Append("=").Append(Request[Constants.REQUEST_KEY_MANAGER_LOGIN_ID]);
						loginPageUrl.Append("&");
						loginPageUrl.Append(Constants.REQUEST_KEY_CUSTOMER_TELNO).Append("=").Append(Request[Constants.REQUEST_KEY_CUSTOMER_TELNO]);

						hc.Response.Redirect(loginPageUrl.ToString());
					}

					// Create next url
					var nextUrl = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_LOGIN)
						.AddParam(Constants.REQUEST_KEY_MANAGER_LOGIN_EXPIRED_FLG, Constants.REQUEST_KEY_MANAGER_LOGIN_EXPIRED_FLG_VALID)
						.AddParam(Constants.REQUEST_KEY_MANAGER_NEXTURL, WebUtility.GetRawUrl(hc.Request))
						.CreateUrl();
					hc.Response.Redirect(nextUrl);
				}
			}
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

		// コンフィグ監視起動（パッケージ初期化処理セット）
		// AppConfigフォルダ配下の全てのファイルを対象とする
		FileUpdateObserver.GetInstance().AddObservationWithSubDir(
			WebConfigurationManager.AppSettings["ConfigFileDirPath"] + @"AppConfig\",
			"*.*",
			InitializePackage);
		FileUpdateObserver.GetInstance().AddObservationWithSubDir(
			WebConfigurationManager.AppSettings["ConfigFileDirPath"] + @"AppConfig\",
			"*.*",
			ManagerMenuCache.Instance.RefreshMenuList);

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
	/// ASP.NET がレスポンスを返す直前に実行するイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Application_OnPreSendRequestHeaders(Object sender, EventArgs e)
	{
		// ServerがWindowsVista以上のOSの場合、レスポンスヘッダのサーバー項目を削除（それ以下のOSの場合はツールを使用して削除）
		if (Environment.OSVersion.Version.Major > 5)
		{
			Response.Headers.Remove("Server");
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
		// ハンドルされていないエラーが発生したときに実行するコードです

		// 最後に発生したエラー原因情報をExceptionオブジェクトとして取得
		Exception objErr = Server.GetLastError();

		// 制御文字入力エラー
		if (objErr.GetBaseException() is System.Web.HttpRequestValidationException)
		{
			// エラーをクリア（しないとIIS7以降でエラー画面へ遷移できない）
			Context.ClearError();
			
			// エラーページへ
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR + "?" + Constants.REQUEST_KEY_ERRORPAGE_MANAGER_ERRORKBN + "=" + HttpUtility.UrlEncode(WebMessages.ERRMSG_SYSTEM_VALIDATION_ERROR));
			return;
		}
		else if (objErr is HttpException)
		{
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
				int iHttpCode = ((HttpException)objErr).GetHttpCode();

				// 404エラーの場合、エラーページへ遷移
				if (iHttpCode == 404)
				{
					Context.ClearError(); // エラーをクリア（しないとIIS7以降でエラー画面へ遷移できない）

					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR + "?" + Constants.REQUEST_KEY_ERRORPAGE_MANAGER_ERRORKBN + "=" + HttpUtility.UrlEncode(WebMessages.ERRMSG_404_ERROR));
					return;
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
					string operatorId;
					try
					{
						operatorId = ((ShopOperatorModel)Session[Constants.SESSION_KEY_LOGIN_SHOP_OPERTOR]) == null ? "(未ログイン)" : ((ShopOperatorModel)Session[Constants.SESSION_KEY_LOGIN_SHOP_OPERTOR]).OperatorId;
					}
					catch
					{
						operatorId = "(取得失敗)";
					}
					message.AppendLine("  ユーザー情報: " + operatorId);

					// ログファイル記述
					AppLogger.WriteError(message.ToString(), objErr);
				}
			}

			// エラーページで例外が発生した場合はエラーページへ飛ばない
			if (Request.Path.ToLower().StartsWith(Constants.PATH_ROOT.ToLower() + Constants.PAGE_MANAGER_ERROR.ToLower())) return;

#if !DEBUG
			// システムエラーを表示
			RedirectErrorPage();
			return;
#endif
		}
		else
		{
			FileLogger.WriteError(objErr);

#if !DEBUG
			// システムエラーを表示
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
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_ERROR);
		if (isDisplaySystemError)
		{
			urlCreator.AddParam(
				Constants.REQUEST_KEY_ERRORPAGE_MANAGER_ERRORKBN,
				HttpUtility.UrlEncode(WebMessages.ERRMSG_SYSTEM_ERROR));
		}
		var url = urlCreator.CreateUrl();

		// エラーページへ
		this.Response.Redirect(url);
	}

	/// <summary>
	/// パッケージ初期化
	/// </summary>
	private void InitializePackage()
	{
		lock (Application)
		{
			try
			{
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
						w2.App.Common.ConfigurationSetting.ReadKbn.C200_CommonManager,
						w2.App.Common.ConfigurationSetting.ReadKbn.C300_CustomerSupport,
						w2.App.Common.ConfigurationSetting.ReadKbn.C300_MarketingPlanner);	// 注意！メール履歴の対応で入ったらしい・・・

				//------------------------------------------------------
				// アプリケーション固有の設定
				//------------------------------------------------------
				// ルートパス設定			
				Constants.PATH_ROOT = Constants.PATH_ROOT_CS;
				// フロントサイト物理パス設定
				Constants.PHYSICALDIRPATH_FRONT_PC = Constants.PHYSICALDIRPATH_CONTENTS_ROOT;
				// CS機能
				Constants.CONST_DISP_CONTENTS_CSTOP_LIST = csSetting.GetAppIntSetting("Const_DispListContentsCount_CsTop");
				Constants.MESSAGEREQUEST_APPROVAL_TYPE_DEFAULT = (csSetting.GetAppStringSetting("MessageRequest_Approval_Type_Default") == "Consultation" ? Constants.FLG_CSMESSAGEREQUEST_APPROVAL_TYPE_CONSULTATION : Constants.FLG_CSMESSAGEREQUEST_APPROVAL_TYPE_PARALLEL);
				Constants.SETTING_MAILINPUT_MAIL_SIGNATURE_INSERT_MODE = (Constants.MailSignatureInsertModeType)csSetting.GetAppSetting("MessageInput_MailSinatureInsertMode", typeof(Constants.MailSignatureInsertModeType));
				Constants.MAX_SUMMARY_SETTING_COUNT = csSetting.GetAppIntSetting("Setting_MaxSummarySettingCount");
				Constants.DMSHIPPINGHISTORY_DISPLAY_METHOD = csSetting.GetAppStringSetting("DmShippingHistory_DisplayMethod");
				Constants.ERROR_MAILADDRESS = csSetting.GetAppStringSetting("CsTop_ErrorMailAddress");
				Constants.DISPLAY_ERROR_POINT = csSetting.GetAppIntSetting("CsTop_DisplayErrorPoint");
				Constants.SEARCH_PAGE_FIRSTVIEW_TIMESPAN_SETTING = csSetting.GetAppStringSetting("SearchPage_FirstView_TimespanSetting");
				Constants.MAIL_TRANSMISSION_DISABLED_STRINGS = csSetting.GetAppStringSetting("Mail_Transmission_Disabled_Strings")
					.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				
				//------------------------------------------------------
				// 各種物理ディレクトリ設定
				//------------------------------------------------------
				// メニューXMLファイル
				Constants.PHYSICALDIRPATH_MENU_XML = AppDomain.CurrentDomain.BaseDirectory + Constants.FILE_XML_MENU;
				Constants.PHYSICALDIRPATH_PAGE_INDEX_LIST_XML = AppDomain.CurrentDomain.BaseDirectory + Constants.FILE_XML_PAGE_INDEX_LIST;
				// CSメール取込バッチEXE
				Constants.PHYSICALDIRPATH_CSMAILRECEIVER_EXE = csSetting.GetAppStringSetting("Program_CsMailReceiver");
				// 絵文字画像ファイル
				Constants.EMOJI_IMAGE_DIRPATH = csSetting.GetAppStringSetting("Mobile_EmojiImageDirPath");
				Constants.EMOJI_IMAGE_URL = csSetting.GetAppStringSetting("Mobile_EmojiImageUrl");

				// メニュー権限
				Constants.STRING_SUPERUSER_NAME = ValueText.GetValueText(
					Constants.TABLE_MENUAUTHORITY,
					Constants.FIELD_MENUAUTHORITY_MENU_AUTHORITY_NAME,
					Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_SUPERUSER);
				Constants.STRING_UNACCESSABLEUSER_NAME = ValueText.GetValueText(
					Constants.TABLE_MENUAUTHORITY,
					Constants.FIELD_MENUAUTHORITY_MENU_AUTHORITY_NAME,
					Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_UNACCESSABLEUSER);
			}
			catch (Exception ex)
			{
				HttpRuntime.UnloadAppDomain();
				throw new ApplicationException("Configファイルの読み込みに失敗しました。", ex);
			}
		}
	}
	  
</script>
