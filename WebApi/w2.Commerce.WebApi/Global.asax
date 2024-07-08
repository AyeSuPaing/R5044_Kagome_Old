<%@ Application Language="C#" Inherits="System.Web.HttpApplication" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Web.Configuration" %>
<%@ Import Namespace="w2.Common.Logger" %>
<%@ Import Namespace="w2.Common.Util" %>

<script RunAt="server">

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
		FileUpdateObserver.GetInstance().AddObservationWithSubDir(
			WebConfigurationManager.AppSettings["ConfigFileDirPath"] + @"AppConfig\",
			"*.*",
			InitializePackage);

		// アプリケーション開始をログ出力
		FileLogger.WriteInfo("アプリケーション開始");
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
	/// アプリケーション実行中にエラーが発生した場合に実行
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	void Application_Error(object sender, EventArgs e)
	{
		// ハンドルされていないエラーが発生したときに実行するコードです

		// 最後に発生したエラー原因情報をExceptionオブジェクトとして取得
		Exception objErr = Server.GetLastError();

		// 制御文字入力エラー
		if (objErr.GetBaseException() is System.Web.HttpRequestValidationException)
		{
			return;
		}
		else if (objErr is HttpException)
		{

			int iHttpCode = ((HttpException)objErr).GetHttpCode();

			// 404エラーの場合
			if (iHttpCode == 404)
			{
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

				// ログファイル記述
				AppLogger.WriteError(message.ToString(), objErr);
			}
		}
		else
		{
			FileLogger.WriteError(objErr);
		}
	}

	/// <summary>
	/// セッション開始時に実行
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
						w2.App.Common.ConfigurationSetting.ReadKbn.C300_Pc,
						w2.App.Common.ConfigurationSetting.ReadKbn.C300_ComerceManager);

			}
			catch (Exception ex)
			{
				HttpRuntime.UnloadAppDomain();
				throw new ApplicationException("Configファイルの読み込みに失敗しました。", ex);
			}
		}
	}
</script>
