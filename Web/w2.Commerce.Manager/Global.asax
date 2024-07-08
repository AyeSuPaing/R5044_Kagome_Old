<%--
=========================================================================================================
  Module      : Global.asax
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Application Language="C#" Inherits="System.Web.HttpApplication" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Web.Configuration" %>
<%@ Import Namespace="Common" %>
<%@ Import Namespace="w2.App.Common.Order.Cart" %>
<%@ Import Namespace="w2.Common.Logger" %>
<%@ Import Namespace="w2.Common.Util" %>
<%@ Import Namespace="w2.Common.Web" %>
<%@ Import Namespace="w2.Domain.Order.Setting" %>
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
		//------------------------------------------------------
		// 一括ログインチェック
		//------------------------------------------------------
		HttpContext hc = HttpContext.Current;
		if (hc.Session != null) // TreeView等でSessionなしのリクエストがある
		{
			if ((string.Compare(hc.Request.FilePath, Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR, true) != 0)
				&& (string.Compare(hc.Request.FilePath, Constants.PATH_ROOT + Constants.PAGE_MANAGER_LOGIN, true) != 0)
				&& (string.Compare(hc.Request.FilePath, Constants.PATH_ROOT + Constants.PAGE_MANAGER_OPERATOR_PASSWORD_CHANGE_COMPLETE, true) != 0)
				&& (string.Compare(hc.Request.FilePath, Constants.PATH_ROOT + Constants.PAGE_MANAGER_MASTERIMPORT_OUTPUTLOG, true) != 0)
				&& (string.Compare(hc.Request.FilePath, Constants.PATH_ROOT + Constants.PAGE_MANAGER_DATA_MIGRATION_OUTPUT_LOG, true) != 0))
			{
				if (Constants.EC_OPTION_ENABLED == false)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages
						.GetMessages(WebMessages.ERRMSG_MANAGER_EC_OPTION_DISABLED);
					var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR).CreateUrl();
					Response.Redirect(url);
				}

				if (hc.Session[Constants.SESSION_KEY_LOGIN_SHOP_OPERTOR] == null)
				{
					// Create next url
					var nextUrl = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_LOGIN)
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
		FileUpdateObserver.GetInstance().AddObservationWithSubDir(
			WebConfigurationManager.AppSettings["ConfigFileDirPath"] + @"AppConfig\",
			"*.*",
			InitializePackage);
		FileUpdateObserver.GetInstance().AddObservationWithSubDir(
			WebConfigurationManager.AppSettings["ConfigFileDirPath"] + @"AppConfig\",
			"*.*",
			ManagerMenuCache.Instance.RefreshMenuList);

		// 配送先マッチング設定監視
		FileUpdateObserver.GetInstance().AddObservationWithSubDir(
			Path.Combine(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE, @"Settings"),
			@"OrderShippingMatchingSetting.xml",
			OrderShippingMatchingSetting.UpdateSetting);

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
	/// Application post map request handler
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Application_PostMapRequestHandler(object sender, EventArgs e)
	{
		// Get current context
		var context = HttpContext.Current;
		// When the context handler is ajax request from the page, add filter page method exception logger to catch response error
		if ((context.Handler is Page) && (string.IsNullOrEmpty(context.Request.PathInfo) == false))
		{
			var contentType = context.Request.ContentType.Split(';')[0];
			if (contentType.Equals("application/json", StringComparison.OrdinalIgnoreCase))
			{
				context.Response.Filter = new PageMethodExceptionLogger(context.Response);
			}
		}
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

				// 500エラーの場合、システムエラーでエラーページへ遷移
				if (iHttpCode == 500)
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

					RedirectErrorPage();
				}

				// 403,404,500エラーでなければエラーログ出力
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

			// 海外送料エラーの場合、特別なメッセージを出すようここでまとめて制御
			if (objErr.GetBaseException() is GlobalShippingPriceCalcException)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CALC_SHIPPING_ERROR).Replace("@@ 1 @@", objErr.GetBaseException().Message);
				// エラーページに遷移する
				RedirectErrorPage(false);
				return;
			}
#if !DEBUG
			// システムエラーを表示する
			RedirectErrorPage();
			return;
#endif
		}
		else
		{
			FileLogger.WriteError(objErr);

#if !DEBUG
			// システムエラーを表示する
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
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
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
				var csSetting
					= w2.App.Common.ConfigurationSetting.CreateInstanceByReadKbn(
						WebConfigurationManager.AppSettings["ConfigFileDirPath"],
						w2.App.Common.ConfigurationSetting.ReadKbn.C300_ComerceManager);

				//------------------------------------------------------
				// アプリケーション固有の設定
				//------------------------------------------------------
				// ルートパス設定			
				Constants.PATH_ROOT = csSetting.GetAppStringSetting("Site_RootPath_w2cManager");
				Constants.OPTIONAPPEAL_PROJECT_PLAN = csSetting.GetAppStringSetting("Option_Appeal_Project_Plan");
				Constants.OPTIONAPPEAL_PROJECT_USED_OPTIONS = csSetting.GetAppStringSetting("Option_Appeal_Project_Used_Options");
				Constants.OPTIONAPPEAL_PROJECT_PLAN = csSetting.GetAppStringSetting("Option_Appeal_Project_Plan");
				Constants.OPTIONAPPEAL_PROJECT_OPTION_VERSION = csSetting.GetAppStringSetting("Option_Appeal_Project_Option_Version");
				Constants.OPTIONAPPEAL_PROJECT_NO = csSetting.GetAppStringSetting("Option_Appeal_Project_No");
				Constants.OPTIONAPPEAL_ENABLED = csSetting.GetAppBoolSetting("Option_Appeal_Enabled");
				Constants.OPTIONAPPEAL_INQUIRY_MAIL_FROM = csSetting.GetAppStringSetting("Option_Appeal_Inquiry_MailAddress_From");
				Constants.OPTIONAPPEAL_INQUIRY_MAIL_TO = csSetting.GetAppStringSetting("Option_Appeal_Inquiry_MailAddress_To");
				// フロントサイト物理パス設定
				Constants.PHYSICALDIRPATH_FRONT_PC = Constants.PHYSICALDIRPATH_CONTENTS_ROOT;
				// 一覧表示系の設定
				Constants.CONST_COUNT_PDF_DIRECTDOUNLOAD = csSetting.GetAppIntSetting("Const_DirectDounload_Pdf_OrderCountLimit");
				Constants.CONST_DISP_BUNDLE_CONTENTS_OVER_HIT_LIST = csSetting.GetAppIntSetting("Const_HitsLimit_BundleList");
				Constants.CONST_DISP_CONTENTS_PAYMENT_LIST = csSetting.GetAppIntSetting("Const_DispListContentsCount_PaymentList");
				Constants.CONST_DISP_CONTENTS_PRODUCTREVIEW_LIST = csSetting.GetAppIntSetting("Const_DispListContentsCount_ProductReviewList");
				Constants.CONST_DISP_CONTENTS_ORDERWORKFLOWSETTING_LIST = csSetting.GetAppIntSetting("Const_DispListContentsCount_OrderWorkFlowSettingList");
				Constants.KBN_SORT_FIXEDPURCHASE_DATE_DEFAULT = csSetting.GetAppStringSetting("SortKbn_FixedpurchaseList_Default");
				Constants.KBN_SORT_MAILTEMPLATE_LIST_DEFAULT = csSetting.GetAppStringSetting("SortKbn_MailTemplateList_Default");
				Constants.KBN_SORT_MALLEXHIBITSCONFIG_LIST_DEFAULT = csSetting.GetAppStringSetting("SortKbn_MallexhibitsConfigList_Default");
				Constants.KBN_SORT_MOBILEGROUP_LIST_DEFAULT = csSetting.GetAppStringSetting("SortKbn_MobileGroupList_Default");
				Constants.KBN_SORT_MOBILEORIGINALTAG_LIST_DEFAULT = csSetting.GetAppStringSetting("SortKbn_MobileRriginalTag_List_Default");
				Constants.KBN_SORT_MOBILEPAGE_LIST_DEFAULT = csSetting.GetAppStringSetting("SortKbn_MobilePageList_Default");
				Constants.KBN_SORT_MOBILEPICTORIALSYMBOL_LIST_DEFAULT = csSetting.GetAppStringSetting("SortKbn_MobilePictorialSymbol_List_Default");
				Constants.KBN_SORT_NEWS_LIST_DEFAULT = csSetting.GetAppStringSetting("SortKbn_NewsList_Default");
				Constants.KBN_SORT_ORDER_DATE_DEFAULT = csSetting.GetAppStringSetting("SortKbn_OrderList_Default");
				Constants.KBN_SORT_PRODUCT_LIST_DEFAULT = csSetting.GetAppStringSetting("SortKbn_ProductList_Default");
				Constants.KBN_SORT_PRODUCTREVIEW_LIST_DEFAULT = csSetting.GetAppStringSetting("SortKbn_ProductReviewList_Default");
				Constants.KBN_SORT_PRODUCTSALE_DEFAULT = csSetting.GetAppStringSetting("SortKbn_ProductSaleList_Default");
				Constants.KBN_SORT_PRODUCTSTOCK_LIST_DEFAULT = csSetting.GetAppStringSetting("SortKbn_ProductStockList_Default");
				Constants.KBN_SORT_USER_LIST_DEFAULT = csSetting.GetAppStringSetting("SortKbn_UserList_Default");
				Constants.PRODUCTTAGSETTING_MAXCOUNT = csSetting.GetAppIntSetting("ProductTagSetting_MaxCount");
				Constants.USEREXTENDSETTING_MAXCOUNT = csSetting.GetAppIntSetting("UserExtendSetting_MaxCount");
				Constants.ORDERWORKFLOWSETTING_MAXCOUNT = csSetting.GetAppIntOrNullSetting("OrderWorkflowSetting_MaxCount");
				Constants.ORDERLIST_FIRSTVIEW_ORDERSTATUS = csSetting.GetAppStringSetting("OrderList_FirstView_OrderStatus");
				Constants.ORDERLIST_FIRSTVIEW_ABSTIMESPAN = csSetting.GetAppStringSetting("OrderList_FirstView_AbsoluteTimespan");
				// 受注編集：自動計算適用のデフォルト（TRUE：ON、FALSE：OFF）
				Constants.ORDER_APPLYAUTOCALCULATION_DEFAULT = csSetting.GetAppBoolSetting("Order_ApplyAutoCalculation_Default");

				// 商品画像エンコードクオリティ
				Constants.PRODUCTIMAGE_ENCODE_QUALITY = csSetting.GetAppIntSetting("ImgConv_ProductImage_Encode_Quality");

				// CMS商品画像リサイズ設定
				Constants.PRODUCTIMAGE_SIZE_S = csSetting.GetAppStringSetting("ImgConv_ProductImage_SizeS");
				Constants.PRODUCTIMAGE_SIZE_M = csSetting.GetAppStringSetting("ImgConv_ProductImage_SizeM");
				Constants.PRODUCTIMAGE_SIZE_L = csSetting.GetAppStringSetting("ImgConv_ProductImage_SizeL");
				Constants.PRODUCTIMAGE_SIZE_LL = csSetting.GetAppStringSetting("ImgConv_ProductImage_SizeLL");

				// CMS商品サブ画像リサイズ設定
				Constants.PRODUCTSUBIMAGE_SIZE_M = csSetting.GetAppStringSetting("ImgConv_ProductSubImage_SizeM");
				Constants.PRODUCTSUBIMAGE_SIZE_L = csSetting.GetAppStringSetting("ImgConv_ProductSubImage_SizeL");
				Constants.PRODUCTSUBIMAGE_SIZE_LL = csSetting.GetAppStringSetting("ImgConv_ProductSubImage_SizeLL");

				// 注文登録系
				// 注文登録時デフォルト注文区分
				Constants.ORDER_DEFALUT_ORDER_KBN = csSetting.GetAppStringSetting("Order_DefaultOrderKbn");
				// 注文登録時デフォルト注文者区分
				Constants.ORDER_DEFALUT_OWNER_KBN = csSetting.GetAppStringSetting("Order_DefaultOwnerKbn");
				// 注文情報登録完了後の遷移先画面
				Constants.ORDER_REGISTER_COMPLETE_PAGE =
					(Constants.OrderRegisterCompletePageType)csSetting.GetAppSetting("OrderRegisterCompletePage", typeof(Constants.OrderRegisterCompletePageType));
				// 新規注文登録会員ランクメモ表示
				Constants.ORDER_MEMBERRANK_MEMO_DISPLAY = csSetting.GetAppBoolSetting("Order_MemberRank_Memo_Display");
				// ユーザー検索サジェスト最大表示件数（最大100件）
				var maxCountForDisplay = csSetting.GetAppIntSetting("Auto_Suggest_Max_Count_For_Display");
				Constants.AUTO_SUGGEST_MAX_COUNT_FOR_DISPLAY = (maxCountForDisplay <= 100)
					? maxCountForDisplay
					: 100;
				// 新規受注登録内項目 サジェスト用クエリタイムアウト時間（秒）
				Constants.ORDERREGISTINPUT_SUGGEST_QUERY_TIMEOUT = csSetting.GetAppIntSetting("OrderRegistInput_Suggest_Query_Timeout");

				// システム管理の許可IPアドレス
				Constants.ALLOWED_IP_ADDRESS_FOR_SYSTEMSETTINGS = csSetting.GetAppStringSettingList("Allowed_IP_Addresses_For_SystemSettings")
					.Where(ip => string.IsNullOrWhiteSpace(ip) == false)
					.ToList();

				// タスクスケジューラ認証情報：バッチ管理＞ターゲットサーバー
				Constants.BATCH_MANAGER_TASKSCHEDULER_TARGET_SERVER = csSetting.GetAppStringSetting("Batch_Manager_TaskScheduler_Target_Server");
				// タスクスケジューラ認証情報：バッチ管理＞ユーザーネーム
				Constants.BATCH_MANAGER_TASKSCHEDULER_USER_NAME = csSetting.GetAppStringSetting("Batch_Manager_TaskScheduler_User_Name");
				// タスクスケジューラ認証情報：バッチ管理＞アカウントドメイン
				Constants.BATCH_MANAGER_TASKSCHEDULER_ACCOUNT_DOMAIN = csSetting.GetAppStringSetting("Batch_Manager_TaskScheduler_Account_Domain");
				// タスクスケジューラ認証情報：バッチ管理＞パスワード
				Constants.BATCH_MANAGER_TASKSCHEDULER_PASSWORD = csSetting.GetAppStringSetting("Batch_Manager_TaskScheduler_Password");

				//------------------------------------------------------
				// 決済系
				//------------------------------------------------------
				// クレジット 実売上連動設定
				Constants.PAYMENT_CARD_REALSALES_ENABLED = csSetting.GetAppBoolSetting("Payment_Credit_RealSales");
				// クレジット キャンセル連動設定
				Constants.PAYMENT_CARD_CANCEL_ENABLED = csSetting.GetAppBoolSetting("Payment_Credit_Cancel_Enabled");
				// AmazonPay 連携設定
				Constants.PAYMENT_AMAZONPAY_REALSALES_ENABLED = csSetting.GetAppBoolSetting("Payment_AmazonPay_RealSales");
				Constants.PAYMENT_AMAZONPAY_CANCEL_ENABLED = csSetting.GetAppBoolSetting("Payment_AmazonPay_Cancel_Enabled");
				// ドコモケータイ払い 連携設定
				Constants.PAYMENT_SETTING_DOCOMOKETAI_SERVER_URL_DECISION = csSetting.GetAppStringSetting("Payment_Docomo_RealSales_ServerUrl");
				Constants.PAYMENT_SETTING_DOCOMOKETAI_REALSALES_ENABLED = csSetting.GetAppBoolSetting("Payment_Docomo_RealSales_Enabled");
				// S!まとめて支払い 連携設定
				Constants.PAYMENT_SETTING_SMATOMETE_SERVER_URL_DECISION = csSetting.GetAppStringSetting("Payment_Softbank_ServerUrl_Decision");
				Constants.PAYMENT_SETTING_SMATOMETE_REALSALES_ENABLED = csSetting.GetAppBoolSetting("Payment_Softbank_Payment");
				// SBPSキャリア決済売上連動
				Constants.PAYMENT_SETTING_SBPS_SOFTBANKKETAI_REALSALES_ENABLED = csSetting.GetAppBoolSetting("Payment_SoftbankKetai_RealSales_Enabled");
				Constants.PAYMENT_SETTING_SBPS_DOCOMOKETAI_REALSALES_ENABLED = csSetting.GetAppBoolSetting("Payment_DocomoKetai_RealSales_Enabled");
				Constants.PAYMENT_SETTING_SBPS_AUKANTAN_REALSALES_ENABLED = csSetting.GetAppBoolSetting("Payment_AuKantan_RealSales_Enabled");
				Constants.PAYMENT_SETTING_SBPS_RECRUIT_REALSALES_ENABLED = csSetting.GetAppBoolSetting("Payment_Recruit_RealSales_Enabled");
				Constants.PAYMENT_SETTING_SBPS_RAKUTEN_ID_REALSALES_ENABLED = csSetting.GetAppBoolSetting("Payment_Rakuten_Id_RealSales_Enabled");
				// SBPSキャリア決済キャンセル連動
				Constants.PAYMENT_SETTING_SBPS_SOFTBANKKETAI_CANCEL_ENABLED = csSetting.GetAppBoolSetting("Payment_SoftbankKetai_Cancel_Enabled");
				Constants.PAYMENT_SETTING_SBPS_DOCOMOKETAI_CANCEL_ENABLED = csSetting.GetAppBoolSetting("Payment_DocomoKetaiCancel_Enabled");
				Constants.PAYMENT_SETTING_SBPS_AUKANTAN_CANCEL_ENABLED = csSetting.GetAppBoolSetting("Payment_AuKantan_Cancel_Enabled");
				Constants.PAYMENT_SETTING_SBPS_RECRUIT_CANCEL_ENABLED = csSetting.GetAppBoolSetting("Payment_Recruit_Cancel_Enabled");
				Constants.PAYMENT_SETTING_SBPS_PAYPAL_CANCEL_ENABLED = csSetting.GetAppBoolSetting("Payment_Paypal_Cancel_Enabled");
				Constants.PAYMENT_SETTING_SBPS_RAKUTEN_ID_CANCEL_ENABLED = csSetting.GetAppBoolSetting("Payment_Rakuten_Id_Cancel_Enabled");

				//------------------------------------------------------
				// カスタムページ/パーツ/Cssファイルツリーリスト枠のサイズ
				//------------------------------------------------------
				// カスタムページ枠の最大サイズ
				Constants.CUSTOM_PAGE_FRAME_MAX_HEIGHT = csSetting.GetAppIntSetting("CustomPageFrameMaxHeight");
				// カスタムページ枠の最小サイズ
				Constants.CUSTOM_PAGE_FRAME_MIN_HEIGHT = csSetting.GetAppIntSetting("CustomPageFrameMinHeight");
				// カスタムページ枠用アップダウンサイズをリサイズ
				Constants.CUSTOM_PAGE_FRAME_UPDOWN_HEIGHT = csSetting.GetAppIntSetting("CustomPageFrameUpDownHeight");
				// カスタムパーツ枠の最大サイズ
				Constants.CUSTOM_PART_FRAME_MAX_HEIGHT = csSetting.GetAppIntSetting("CustomPartFrameMaxHeight");
				// カスタムパーツ枠の最小サイズ
				Constants.CUSTOM_PART_FRAME_MIN_HEIGHT = csSetting.GetAppIntSetting("CustomPartFrameMinHeight");
				// カスタムパーツ枠用アップダウンサイズをリサイズ
				Constants.CUSTOM_PART_FRAME_UPDOWN_HEIGHT = csSetting.GetAppIntSetting("CustomPartFrameUpDownHeight");
				// カスタムCssファイルツリーリストの最大サイズ
				Constants.CSS_LIST_FRAME_MAX_HEIGHT = csSetting.GetAppIntSetting("CssListFrameMaxHeight");
				// カスタムCssファイルツリーリストの最小サイズ
				Constants.CSS_LIST_FRAME_MIN_HEIGHT = csSetting.GetAppIntSetting("CssListFrameMinHeight");
				// カスタムCssファイルツリーリスト用アップダウンサイズをリサイズ
				Constants.CSS_LIST_FRAME_UPDOWN_HEIGHT = csSetting.GetAppIntSetting("CssListFrameUpDownHeight");
				// カスタムパーツの制限件数
				Constants.CUSTOM_PARTS_MAX_COUNT = csSetting.GetAppIntSetting("CustomParts_MaxCount");
				// ワークフロー設定画面のテキストボックスを表示/非表示
				Constants.ORDERWORKFLOWSETTING_MEMO_TEXTBOX_ENABLED = csSetting.GetAppBoolSetting("WorkflowSettingMemoTextbox_Enabled");
				// 検索条件/ワークフロー設定に「配送先：都道府県」を表示/非表示
				Constants.SEARCHCONDITION_SHIPPINGADDR1_ENABLED = csSetting.GetAppBoolSetting("SearchConditionShippingAddr1_Enabled");

				//------------------------------------------------------
				// 定期購入系設定
				//------------------------------------------------------
				// 定期注文登録時にユーザーに注文メール＆定期購入エラーメール送信するか
				Constants.SEND_FIXEDPURCHASE_MAIL_TO_USER = csSetting.GetAppBoolSetting("SendFixedPurchaseMailToUser");

				//------------------------------------------------------
				// 各種オプション利用有無系
				//------------------------------------------------------
				// 注文登録利用有無
				Constants.ORDERREGIST_OPTION_ENABLED = csSetting.GetAppBoolSetting("OrderRegistOption_Enabled");
				// 実在庫利用設定有無
				Constants.REALSTOCK_OPTION_ENABLED = csSetting.GetAppBoolSetting("RealStockOption_Enabled");
				// 商品セール利用有無
				Constants.PRODUCT_SALE_OPTION_ENABLED = csSetting.GetAppBoolSetting("ProductSaleOption_Enabled");
				// 送り状発行CSV出力機能
				Constants.INVOICECSV_ENABLED = csSetting.GetAppBoolSetting("OrderInvoiceCSV_Enabled");
				// ユーザーデータ初期移行
				Constants.MASTERUPLOAD_USER_ENABLED = csSetting.GetAppBoolSetting("MasterUpload_UserUpload_Enabled");
				// 注文本ポイント自動付与（本ポイント）
				Constants.GRANT_ORDER_POINT_AUTOMATICALLY = csSetting.GetAppBoolSetting("Point_GrantOrderPointWithShipment");
				// 受注管理の在庫連動可否
				Constants.ORDERMANAGEMENT_STOCKCOOPERATION_ENABLED = csSetting.GetAppBoolSetting("OrderManagementStockCooperation_Enabled");
				// 受注明細書出力機能
				Constants.PDF_OUTPUT_ORDERSTATEMENT_ENABLED = csSetting.GetAppBoolSetting("PdfOutputOrderstatement_Enabled");
				// ピッキングリスト出力機能
				Constants.PDF_OUTPUT_PICKINGLIST_ENABLED = csSetting.GetAppBoolSetting("PdfOutputPickingList_Enabled");
				// エラーメールポイント更新
				Constants.UPDATE_POINT_ERROR_MAIL_OPTION_ENABLED = csSetting.GetAppBoolSetting("UpdatePointErrorMailOption_Enabled");
				// メールオプション：メールオプション利用有無
				Constants.MARKETINGPLANNER_MAIL_OPTION_ENABLE = csSetting.GetAppBoolSetting("MailDistributeOption_Enabled");

				//------------------------------------------------------
				// 商品コンバータ設定
				//------------------------------------------------------
				Constants.PHYSICALDIRPATH_KICKUPDATE_SERVICE_EXE = csSetting.GetAppStringSetting("Program_KickUpdateService");

				//------------------------------------------------------
				// 各種物理ディレクトリ設定
				//------------------------------------------------------
				// マスタファイルアップロード実行ＥＸＥ
				Constants.PHYSICALDIRPATH_MASTERUPLOAD_EXE = csSetting.GetAppStringSetting("Program_MasterFileImport");
				// 外部ファイルアップロードディレクトリ
				Constants.PHYSICALDIRPATH_EXTERNAL_DIR = Constants.PHYSICALDIRPATH_EXTERNALFILEUPLOAD;
				// 注文PDFクリエータ実行ＥＸＥ
				Constants.PHYSICALDIRPATH_ORDER_PDF_CREATER_EXE = csSetting.GetAppStringSetting("Program_OrderPdfCreater");
				// Cssルート
				Constants.PHYSICALDIRPATH_CSS_ROOT = Constants.PHYSICALDIRPATH_FRONT_PC + @"Css\";
				// メニューXMLファイル
				Constants.PHYSICALDIRPATH_MENU_XML = AppDomain.CurrentDomain.BaseDirectory + Constants.FILE_XML_MENU;
				// 機能一覧XMLファイル
				Constants.PHYSICALDIRPATH_PAGE_INDEX_LIST_XML = AppDomain.CurrentDomain.BaseDirectory + Constants.FILE_XML_PAGE_INDEX_LIST;
				// pdfテンプレートファイル
				Constants.PHYSICALDIRPATH_PDF_TEMPLATE = AppDomain.CurrentDomain.BaseDirectory + "Pdf/";
				// 表示商品作成バッチ実行ＥＸＥ
				Constants.PHYSICALDIRPATH_CREATEDISPPRODUCT_EXE = csSetting.GetAppStringSetting("Program_CreateDispProduct");
				// 入荷通知メール送信EXE
				Constants.PHYSICALDIRPATH_ARRIVALMAILSEND_EXE = csSetting.GetAppStringSetting("Program_ArrivalMailSend");
				// The number of item history first display
				Constants.ITEMS_HISTORY_FIRST_DISPLAY = csSetting.GetAppIntSetting("Items_History_First_Display");

				// 注文同梱
				Constants.CONST_DISP_LIST_CONTENTS_COUNT_ORDERCOMBINE = csSetting.GetAppIntSetting("Const_DispListContentsCount_OrderCombine");
				Constants.CONST_DISP_LIST_CONTENTS_COUNT_FIXEDPURCHASECOMBINE = csSetting.GetAppIntSetting("Const_DispListContentsCount_FixedPurchaseCombine");
				Constants.CONST_DISP_FIXEDPURCHASECOMBINE_INITIAL_NEXT_SHIPPING_DATE_INTERVAL = csSetting.GetAppIntSetting("Const_Disp_FixedPurchaseCombine_Initial_next_Shipping_Date_Interval");

				// 注文関連ファイル取込の同期/非同期
				Constants.ORDER_FILE_IMPORT_ASYNC = csSetting.GetAppBoolSetting("Order_File_Import_Async");
				// 注文関連ファイル取込バッチ実行ＥＸＥ
				Constants.PHYSICALDIRPATH_ORDERFILEIMPORT_EXE = csSetting.GetAppStringSetting("Program_ImportOrderFile");
				// 注文関連ファイルのUpload先
				Constants.DIRECTORY_IMPORTORDERFILE_UPLOAD_PATH = csSetting.GetAppStringSetting("Directory_ImportOrderFile_Upload_Path");
				// FLAPS連携バッチ実行EXEファイルパス
				Constants.PHYSICALDIRPATH_FLAPS_INTEGRATION_EXE = csSetting.GetAppStringSetting("Program_FlapsIntegration");

				// WFでの出荷報告時の外部決済ST
				Constants.EXTERNAL_PAYMENT_STATUS_SHIPCOMP_ORDERWORKFLOW_EXTERNALSHIPMENTACTION
					= csSetting.GetAppBoolSetting("External_Payment_Status_ShipComp_OrderWorkflow_ExternalShipmentAction");

				// メニュー権限
				Constants.STRING_SUPERUSER_NAME = ValueText.GetValueText(
					Constants.TABLE_MENUAUTHORITY,
					Constants.FIELD_MENUAUTHORITY_MENU_AUTHORITY_NAME,
					Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_SUPERUSER);
				Constants.STRING_UNACCESSABLEUSER_NAME = ValueText.GetValueText(
					Constants.TABLE_MENUAUTHORITY,
					Constants.FIELD_MENUAUTHORITY_MENU_AUTHORITY_NAME,
					Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_UNACCESSABLEUSER);

				// 受注詳細画面でアラート表示にする注文ステータス（カンマ区切り。空白で非利用となる。）
				Constants.ALERT_FOR_ORDER_STATUS_IN_ORDER_CONFIRM = csSetting.GetAppStringSetting("AlertForOrderStatusInOrderConfirm");

				// Exchange rate execution path
				Constants.PHYSICALDIRPATH_GET_EXCHANGERATE_EXE = csSetting.GetAppStringSetting("Program_GetExchangeRate");

				// 定期購入時に初回配送日を注文日の翌月以降から選択する機能
				Constants.FIXED_PURCHASE_FIRST_SHIPPING_DATE_NEXT_MONTH_ENABLED = csSetting.GetAppBoolSetting("FixedPurchase_FirstShippingDate_NextMonth_Enabled");
			}
			catch (Exception ex)
			{
				HttpRuntime.UnloadAppDomain();
				throw new ApplicationException("Configファイルの読み込みに失敗しました。", ex);
			}
		}
	}

</script>
