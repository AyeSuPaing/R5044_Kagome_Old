/*
=========================================================================================================
  Module      : セッションラッパー(SessionWrapper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Web;
using w2.App.Common.Manager.Menu;
using w2.Cms.Manager.ViewModels.FeatureImage;
using w2.Domain.FeatureImage;
using w2.Domain.ShopOperator;

namespace w2.Cms.Manager.Codes
{
	/// <summary>
	/// セッションラッパー
	/// </summary>
	public class SessionWrapper
	{
		/// <summary>
		/// コンストラクタ（テスト実施の際はいったんnull格納）
		/// </summary>
		public SessionWrapper() : this(
			((HttpContext.Current != null) && (HttpContext.Current.Session != null))
				? new HttpSessionStateWrapper(HttpContext.Current.Session)
				: null)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="session">セッション</param>
		public SessionWrapper(HttpSessionStateBase session)
		{
			this.Session = session;
		}

		/// <summary>
		/// ファイルを開いた時間を取得
		/// </summary>
		/// <param name="lpPageFilePath">LPページファイルパス</param>
		/// <returns>取得された時間</returns>
		public virtual DateTime? GetLpPageOpenDateTime(string lpPageFilePath)
		{
			return (DateTime?)HttpContext.Current.Session["FileOpenTime_" + lpPageFilePath];
		}

		/// <summary>
		/// ファイルを開いた時間を更新
		/// </summary>
		/// <param name="lpPageFilePath">LPページファイルパス</param>
		public void UpdateFileOpenTime(string lpPageFilePath)
		{
			HttpContext.Current.Session["FileOpenTime_" + lpPageFilePath] = DateTime.Now;
		}

		/// <summary>ログインしているか</summary>
		public virtual bool IsLoggedIn
		{
			get { return (this.LoginOperator != null); }
		}
		/// <summary>ログイン店舗ID</summary>
		public virtual string LoginShopId
		{
			get { return (this.LoginOperator != null) ? this.LoginOperator.ShopId : ""; }
		}
		/// <summary>ログインオペレータ名</summary>
		public virtual string LoginOperatorName
		{
			get { return (this.LoginOperator != null) ? this.LoginOperator.Name : ""; }
		}
		/// <summary>ログインオペレータメニュー</summary>
		public virtual MenuLarge[] LoginOperatorMenus
		{
			get
			{
				if (this.LoginMenuAccessInfo != null) return this.LoginMenuAccessInfo.LargeMenus;
				return new MenuLarge[0];
			}
		}
		/// <summary>ログインオペレータ情報</summary>
		public virtual ShopOperatorModel LoginOperator
		{
			get { return (ShopOperatorModel)this.Session["LoginOperator"]; }
			set { this.Session["LoginOperator"] = value; }
		}
		/// <summary>ログインメニューアクセス情報</summary>
		public virtual MenuAccessInfo LoginMenuAccessInfo
		{
			get { return (MenuAccessInfo)this.Session["LoginMenuAccessInfo"]; }
			set { this.Session["LoginMenuAccessInfo"] = value; }
		}
		/// <summary>スーパーユーザーか</summary>
		public virtual bool IsSuperUser
		{
			get
			{
				if (this.LoginMenuAccessInfo == null) return false;
				return (this.LoginMenuAccessInfo.MenuAccessLevel.HasValue
					&& (this.LoginMenuAccessInfo.MenuAccessLevel == OperatorMenuManager.KBN_OPERATOR_LEVEL_SUPERUSER));
			}
		}
		/// <summary>アクションステータス</summary>
		public virtual string ActionStatus
		{
			get { return (string)this.Session["ActionStatus"]; }
			set { this.Session["ActionStatus"] = value; }
		}
		/// <summary>ログインエラーカウント</summary>
		public virtual Dictionary<string, int> LoginErrorCounts
		{
			get
			{
				if (this.Session[Constants.SESSION_KEY_LOGIN_ERROR_INFO] == null)
				{
					this.Session[Constants.SESSION_KEY_LOGIN_ERROR_INFO] = new Dictionary<string, int>();
				}
				var loginErrorCounts = (Dictionary<string, int>)this.Session[Constants.SESSION_KEY_LOGIN_ERROR_INFO];
				return loginErrorCounts;
			}
			set { this.Session[Constants.SESSION_KEY_LOGIN_ERROR_INFO] = value; }
		}
		/// <summary>名称翻訳設定検索条件</summary>
		public string[] TranslationSearchCondition
		{
			get { return (string[])this.Session[Constants.SESSION_KEY_PARAM]; }
			set { this.Session[Constants.SESSION_KEY_PARAM] = value; }
		}
		/// <summary>名称翻訳設定出力対象データ区分</summary>
		public string TranslationExportTargetDataKbn
		{
			get { return (string)this.Session[Constants.SESSION_KEY_NAMETRANSLATIONSETTING_EXPORT_TARGET_DATAKBN]; }
			set { this.Session[Constants.SESSION_KEY_NAMETRANSLATIONSETTING_EXPORT_TARGET_DATAKBN] = value; }
		}
		/// <summary>サイト基本情報ステータス</summary>
		public virtual string SiteInformationStatus
		{
			get { return (string)this.Session[Constants.SESSION_KEY_SITE_INFORMATION_STATUS]; }
			set { this.Session[Constants.SESSION_KEY_SITE_INFORMATION_STATUS] = value; }
		}
		/// <summary>ランディングページID</summary>
		public string LandingPageId
		{
			get { return (string)this.Session["LandingPageId"]; }
			set { this.Session["LandingPageId"] = value; }
		}
		/// <summary>コーディネートID</summary>
		public string CoordinateId
		{
			get { return (string)this.Session["CoordinatePageId"]; }
			set { this.Session["CoordinatePageId"] = value; }
		}
		/// <summary>コンテンツマネージャー：選択されたディレクトリ</summary>
		public string ContentsMnagerClickCurrent
		{
			get { return (string)this.Session["ContentsMnagerClickCurrent"]; }
			set { this.Session["ContentsMnagerClickCurrent"] = value; }
		}
		/// <summary>コンテンツマネージャー：作業用ディレクトリパス</summary>
		public string ContentsMnagerTempDirPath
		{
			get { return (string)this.Session["ContentsMnagerTempDirPath"]; }
			set { this.Session["ContentsMnagerTempDirPath"] = value; }
		}
		/// <summary>CSSデザインマネージャー：選択されたディレクトリ</summary>
		public string CssDesignMnagerClickCurrent
		{
			get { return (string)this.Session["CssDesignMnagerClickCurrent"]; }
			set { this.Session["CssDesignMnagerClickCurrent"] = value; }
		}
		/// <summary>CSSデザインマネージャー：選択中のパス</summary>
		public string CssDesignMnagerSelectPath
		{
			get { return (string)this.Session["CssDesignMnagerSelectPath"]; }
			set { this.Session["CssDesignMnagerSelectPath"] = value; }
		}
		/// <summary>CSSデザインマネージャー：現在のフロントアプリケーションのパスルート</summary>
		public string CssDesignMnagerPathRootFront
		{
			get { return (string)this.Session["CssDesignMnagerPathRootFront"]; }
			set { this.Session["CssDesignMnagerPathRootFront"] = value; }
		}
		/// <summary>JavaScriptデザインマネージャー：選択されたディレクトリ</summary>
		public string JavascriptDesingMnagerClickCurrent
		{
			get { return (string)this.Session["JavascriptDesingMnagerClickCurrent"]; }
			set { this.Session["JavascriptDesingMnagerClickCurrent"] = value; }
		}
		/// <summary>JavaScriptデザインマネージャー：選択中のパス</summary>
		public string JavascriptDesingMnagerSelectPath
		{
			get { return (string)this.Session["JavascriptDesingMnagerSelectPath"]; }
			set { this.Session["JavascriptDesingMnagerSelectPath"] = value; }
		}
		/// <summary>Javascriptデザインマネージャー：現在のフロントアプリケーションのパスルート</summary>
		public string JavascriptDesignMnagerPathRootFront
		{
			get { return (string)this.Session["JavascriptDesignMnagerPathRootFront"]; }
			set { this.Session["JavascriptDesignMnagerPathRootFront"] = value; }
		}
		/// <summary>Scoring sale Id</summary>
		public string ScoringSaleId
		{
			get { return (string)this.Session["ScoringSaleId"]; }
			set { this.Session["ScoringSaleId"] = value; }
		}
		/// <summary>ページID</summary>
		public long? PageId
		{
			get { return (long?)this.Session["PageId"]; }
			set { this.Session["PageId"] = value; }
		}
		/// <summary>特集画像グループリスト</summary>
		public FeatureImageGroupModel[] FeatureImageGroupList
		{
			get { return (FeatureImageGroupModel[])this.Session[Constants.SESSION_KEY_FEATURE_IMAGE_GROUP_LIST]; }
			set { this.Session[Constants.SESSION_KEY_FEATURE_IMAGE_GROUP_LIST] = value; }
		}
		/// <summary>特集画像リスト</summary>
		public List<ImageViewModel> FeatureImageList
		{
			get { return (List<ImageViewModel>)this.Session[Constants.SESSION_KEY_FEATURE_IMAGE_LIST]; }
			set { this.Session[Constants.SESSION_KEY_FEATURE_IMAGE_LIST] = value; }
		}

		/// <summary>セッション</summary>
		public HttpSessionStateBase Session { get; private set; }
	}
}
