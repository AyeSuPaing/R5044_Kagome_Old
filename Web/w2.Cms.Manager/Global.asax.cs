/*
=========================================================================================================
  Module      : Global.asax(Global.asax)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using w2.App.Common;
using w2.App.Common.OperationLog;
using w2.App.Common.Web.Process;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;
using w2.Cms.Manager.Codes.Menu;
using w2.Cms.Manager.Properties;
using w2.Common.Logger;
using w2.Common.Util;
using Constants = w2.Cms.Manager.Codes.Constants;
using WebUtility = w2.Common.Web.WebUtility;

namespace w2.Cms.Manager
{
	/// <summary>
	/// MVCアプリケーション
	/// </summary>
	public class MvcApplication : System.Web.HttpApplication
	{
		/// <summary>
		/// アプリケーションスタート
		/// </summary>
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			BinderConfig.RegisterBinders(ModelBinders.Binders);

			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);

			ModelBinders.Binders.Add(typeof(bool), new BooleanModelBinder());

			// 仮のログ出力先を設定
			Constants.PHYSICALDIRPATH_LOGFILE = Path.Combine(
				Settings.Default.ConfigFileDirPath,
				@"Logs",
				Settings.Default.Application_Name);
			// 初期化
			InitializePackage();

			// コンフィグ監視起動（パッケージ初期化処理セット）
			// AppConfigフォルダ配下の全てのファイルを対象とする
			FileUpdateObserver.GetInstance().AddObservationWithSubDir(
				Path.Combine(Settings.Default.ConfigFileDirPath, @"AppConfig\"),
				"*.*",
				InitializePackage);
		}

		/// <summary>
		/// パッケージ初期化
		/// </summary>
		private void InitializePackage()
		{
			lock (this.Application)
			{
				try
				{
					// アプリケーション名
					Constants.APPLICATION_NAME = Settings.Default.Application_Name;
					// アプリケーション共通の設定
					var cs = new ConfigurationSetting(
						Settings.Default.ConfigFileDirPath,
						ConfigurationSetting.ReadKbn.C000_AppCommon,
						ConfigurationSetting.ReadKbn.C100_SiteCommon,
						ConfigurationSetting.ReadKbn.C200_CommonManager,
						ConfigurationSetting.ReadKbn.C300_Cms);

					// ルートパス設定
					Constants.PATH_ROOT = Constants.PATH_ROOT_CMS;

					//商品画像エンコードクオリティ
					Constants.PRODUCTIMAGE_ENCODE_QUALITY = cs.GetAppIntSetting("ImgConv_ProductImage_Encode_Quality");

					// CMS商品画像リサイズ設定
					Constants.PRODUCTIMAGE_SIZE_S = cs.GetAppStringSetting("ImgConv_ProductImage_SizeS");
					Constants.PRODUCTIMAGE_SIZE_M = cs.GetAppStringSetting("ImgConv_ProductImage_SizeM");
					Constants.PRODUCTIMAGE_SIZE_L = cs.GetAppStringSetting("ImgConv_ProductImage_SizeL");
					Constants.PRODUCTIMAGE_SIZE_LL = cs.GetAppStringSetting("ImgConv_ProductImage_SizeLL");
					// CMS商品サブ画像リサイズ設定
					Constants.PRODUCTSUBIMAGE_SIZE_M = cs.GetAppStringSetting("ImgConv_ProductSubImage_SizeM");
					Constants.PRODUCTSUBIMAGE_SIZE_L = cs.GetAppStringSetting("ImgConv_ProductSubImage_SizeL");
					Constants.PRODUCTSUBIMAGE_SIZE_LL = cs.GetAppStringSetting("ImgConv_ProductSubImage_SizeLL");
					// FrontPageバックアップパス
					Constants.NUMBER_OF_GENERATIONS_HOLODING_FRONT_BACKUP = cs.GetAppIntSetting("Number_Of_Generations_Holding_Front_Backup");
					// コンテンツショートカット設定
					Constants.CONTENTSMANAGER_CONTENTS_SHORTCUT_LIST = cs.GetAppStringSetting("ContentsShortCut").Split(';')
						.Where(s => string.IsNullOrEmpty(s) == false)
						.Select(setting =>
						{
							var splited = setting.Split(',');
							return new KeyValuePair<string, string>(CommonPageProcess.ReplaceTag(splited[0]), splited[1]);
						}).ToList();

					// マスタファイルアップロード実行ＥＸＥ
					Constants.PHYSICALDIRPATH_MASTERUPLOAD_EXE = cs.GetAppStringSetting("Program_MasterFileImport");

					// 表示商品作成バッチ実行ＥＸＥ
					Constants.PHYSICALDIRPATH_CREATEDISPPRODUCT_EXE = cs.GetAppStringSetting("Program_CreateDispProduct");

					// サイトマップ設定XML
					Constants.FILE_SITEMAPSETTING_PC = cs.GetAppStringSetting("File_SitemapSetting_Pc");

					//商品ブランド
					Constants.PRODUCT_BRAND_ENABLED = cs.GetAppBoolSetting("ProductBrand_Enabled");

					Constants.PAGE_PARTS_MAX_VIEW_CONTENT_COUNT = cs.GetAppIntSetting("Page_Parts_Max_View_Content_Count");

					Constants.PARTS_IN_PAGEDESIGN_MAX_VIEW_CONTENT_COUNT = cs.GetAppIntSetting("Parts_In_PageDesign_Max_View_Content_Count");

					Constants.PHYSICALDIRPATH_PAGEDESIGN_CONSISTENCY_EXE = cs.GetAppStringSetting("Program_PageDesignConsistency");

					Constants.CONCURRENT_EDIT_EXCLUSION_LOGIN_OPERATOR_ID_LIST = cs.GetAppStringSettingList("ConcurrentEditExclusionLoginOperatorIdList");

					Constants.TREEVIEW_MAX_VIEW_CONTENT_COUNT = cs.GetAppIntSetting("TreeView_Max_View_Content_Count");

					Constants.PREVIEW_URL = cs.GetAppStringSetting("Preview_Url");
					Constants.PREVIEW_BASIC_AUTHENTICATION_USER_ACCOUNT = cs.GetAppStringSetting("Preview_BasicAuthentication_UserAccount");
					// メニュー権限リフレッシュ
					ManagerMenuCache.Instance.RefreshMenuList();
				}
				catch (Exception ex)
				{
					HttpRuntime.UnloadAppDomain();
					throw new Exception("Configファイルの読み込みに失敗しました。", ex);
				}
			}
		}

		/// <summary>
		///  リクエストに関連した状態（セッション等）取得時処理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Application_AcquireRequestState(Object sender, EventArgs e)
		{
			// HTTPS通信チェック（HTTPの場合、HTTPSで再読込）
			CheckHttps(Constants.PROTOCOL_HTTPS + Request.Url.Authority + WebUtility.GetRawUrl(this.Request));

			HttpContext hc = HttpContext.Current;
			if (hc.Session == null) return;
			if ((string.Compare(
					hc.Request.FilePath,
					Constants.PATH_ROOT + Constants.CONTROLLER_W2CMS_MANAGER_ERROR,
					StringComparison.OrdinalIgnoreCase) != 0)
				&& (string.Compare(
					hc.Request.FilePath,
					Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR,
					StringComparison.OrdinalIgnoreCase) != 0)
				&& (string.Compare(
					hc.Request.FilePath,
					Constants.PATH_ROOT + Constants.CONTROLLER_W2CMS_MANAGER_LOGIN,
					StringComparison.OrdinalIgnoreCase) != 0)
				&& (string.Compare(
					hc.Request.FilePath,
					Constants.PATH_ROOT + Constants.CONTROLLER_W2CMS_MANAGER_SINGLE_SIGN_ON,
					StringComparison.OrdinalIgnoreCase) != 0))
			{
				if (Constants.CMS_OPTION_ENABLED == false)
				{
					var url = UrlUtil.CreateUnCmsOptionEnabledErrorUrl();
					this.Response.Redirect(url);
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
		}

		/// <summary>
		/// HTTPS通信チェック
		/// </summary>
		/// <param name="redirectUrl">ＮＧ時リダイレクトURL</param>
		public void CheckHttps(string redirectUrl)
		{
			WebUtility.CheckHttps(this.Request, this.Response, redirectUrl);
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
		/// ページ実行開始直前の処理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void Application_PreRequestHandlerExecute(object sender, EventArgs e)
		{
			var sessionWrapper = new SessionWrapper();
			var notLoggedIn = "(未ログイン)";
			var operatorId = (sessionWrapper.Session != null)
				? (sessionWrapper.LoginOperator != null)
					? sessionWrapper.LoginOperator.OperatorId
					: notLoggedIn
				: "(取得失敗)";

			if (operatorId.Equals(notLoggedIn))
			// ログイン時のログ出力
			{
				OperationLogWriter.WriteOperationLog(
					operatorId,
					Request.UserHostAddress,
					(HttpContext.Current.Session == null) ? string.Empty : HttpContext.Current.Session.SessionID,
					Request.RawUrl);
			}
			else
			{
				//LogデータをJsonファイルに記録
				OperationLogWriter.WriteOperationLogAndEs(
					operatorId,
					sessionWrapper.LoginOperatorName,
					Request.UserHostAddress,
					(HttpContext.Current.Session == null) ? string.Empty : HttpContext.Current.Session.SessionID,
					Request.RawUrl,
					Request.RawUrl.Split('?')[0],
					Request.Url.Query);
			}
		}

		/// <summary>
		/// 集約エラーハンドラ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Application_Error(Object sender, EventArgs e)
		{
			// 最後に発生したエラー原因情報をExceptionオブジェクトとして取得
			var ex = Server.GetLastError();

			var writeLog = false;

			// 制御文字入力エラー
			if (ex.GetBaseException() is HttpRequestValidationException)
			{
				var url = UrlUtil.CreateErrorPageUrl(Constants.REQUEST_KBN_ERROR_KBN_SYTEM_VALIDATION_ERROR);
				Response.Redirect(url);
				return;
			}
			else if (ex is HttpException)
			{
				// HttpException -> HttpParseException の継承関係なので、HttpParseExceptionを先に捕まえる
				if (ex is HttpParseException)
				{
					// アクセスされたページから参照されているファイル(.ascx)が存在しない場合は、
					// HttpParseException で HttpCode 404になる。（つまりここに含まれる）
					// この場合は、ページ404として扱ってはいけないし、ショートURL処理を行ってもいけない。
					// そしてエラーログを出さなければならない。
					writeLog = true;
				}
				else
				{
					// 404エラーの場合、エラーページへ遷移
					var httpCode = ((HttpException)ex).GetHttpCode();
					var isNotAjaxRequest = (new HttpRequestWrapper(Request).IsAjaxRequest() == false);
					if ((httpCode == 404) && isNotAjaxRequest)
					{
						var url = UrlUtil.CreateErrorPageUrl(Constants.REQUEST_KBN_ERROR_KBN_404_ERROR);
						Response.Redirect(url);
						return;
					}

					// 403,404エラーでなければエラーログ出力
					if ((httpCode != 404) && (httpCode != 403)) writeLog = true;
				}

				if (writeLog)
				{
					var message = CreateErrorMessageForLog(ex);
					FileLogger.WriteError(message, ex);
					return;
				}

#if !DEBUG
				// エラーページでのエラーではなく、AJAXでない場合はエラーページへ遷移
				if ((Request.Path.StartsWith(System.IO.Path.Combine(Constants.PATH_ROOT, Constants.PAGE_MANAGER_ERROR), StringComparison.OrdinalIgnoreCase) == false)
					&& (new HttpRequestWrapper(Request).IsAjaxRequest() == false))
				{	
					RedirectErrorPage();
					return;
				}
#endif
			}
			else
			{
				FileLogger.WriteError(ex);
#if !DEBUG
				RedirectErrorPage();
				return;
#endif
			}
		}

		/// <summary>
		/// ログ向けエラーメッセージ作成
		/// </summary>
		/// <param name="ex">例外</param>
		/// <returns>エラーメッセージ</returns>
		private string CreateErrorMessageForLog(Exception ex)
		{
			var message = new StringBuilder();
			if (ex is HttpParseException)
			{
				message.AppendFormat("HTTPパーサーエラー: {0}", ((HttpParseException)ex).Message).AppendLine();
			}
			else
			{
				message.AppendFormat("HTTPエラー: {0}", ex.Message).AppendLine();
			}
			message.AppendFormat("  要求URL: {0}", Request.RawUrl ?? "(null)").AppendLine();
			message.AppendFormat("  実行URL: {0}", Request.Url.PathAndQuery ?? "(null)").AppendLine();
			if (ex is HttpParseException)
			{
				message.AppendFormat("  発生URL: {0}", ((HttpParseException)ex).VirtualPath ?? "(null)").AppendLine();
				message.AppendFormat("  発生行数: {0}", ((HttpParseException)ex).Line).AppendLine();
			}
			message.AppendFormat("  IPアドレス: {0}", Request.UserHostAddress ?? "").AppendLine();
			message.AppendFormat("  User-Agent: {0}", Request.UserAgent ?? "").AppendLine();

			var sessionWrapper = new SessionWrapper();
			var operatorId = (sessionWrapper.Session != null)
				? (sessionWrapper.LoginOperator != null)
					? sessionWrapper.LoginOperator.OperatorId
					: "(未ログイン)"
				: "(取得失敗)";
			message.AppendFormat("  オペレータ情報: {0}", operatorId).AppendLine();

			return message.ToString();
		}

		/// <summary>
		/// エラーページへの遷移
		/// </summary>
		private void RedirectErrorPage()
		{
			this.Context.ClearError();

			var url = UrlUtil.CreateErrorPageUrl(Constants.REQUEST_KBN_ERROR_KBN_SYSTEM_ERROR);
			this.Response.Redirect(url);
		}
	}
}
