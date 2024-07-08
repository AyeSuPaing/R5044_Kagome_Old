/*
=========================================================================================================
  Module      : ベースページモジュール(BasePage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using w2.App.Common.Option;
using w2.App.Common.Global.Config;
using w2.App.Common.Manager.Menu;
using w2.Common.Web;
using w2.Domain.ShopOperator;

/// <summary>
/// BasePage の概要の説明です
/// </summary>
public class BasePage : w2.App.Common.Web.Page.CommonPage
{
	/// <summary>
	/// ページ初期化
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void Page_PreInit(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// HTTPS通信チェック（HTTPの場合、HTTPSで再読込）
			//------------------------------------------------------
			CheckHttps(Constants.PROTOCOL_HTTPS + Request.Url.Authority + this.RawUrl);
		}

		if (Page.Master != null)
		{
			// ポップアップマスタページを設定
			if (Request[Constants.REQUEST_KEY_WINDOW_KBN] == Constants.KBN_WINDOW_POPUP)
			{
				Page.MasterPageFile = "~/Form/Common/PopupPage.master";
			}
		}
	}

	/// <summary>
	/// HTTPS通信チェック
	/// </summary>
	public virtual void CheckHttps()
	{
		// HTTPS通信チェック（ＮＧの場合トップページへ遷移）
		CheckHttps(Constants.PATH_ROOT);
	}
	/// <summary>
	/// HTTPS通信チェック
	/// </summary>
	/// <param name="strRedirectUrl">ＮＧ時リダイレクトURL</param>
	public void CheckHttps(string strRedirectUrl)
	{
		WebUtility.CheckHttps(Request, Response, strRedirectUrl);
	}

	/// <summary>
	/// 処理区分チェック
	/// </summary>
	/// <param name="strActionStatus">処理区分</param>
	/// <remarks>
	/// 不正な処理区分の場合、エラー画面へ遷移
	/// </remarks>
	protected void CheckActionStatus(string strActionStatus)
	{
		// アクションステータスがマッチしていない場合
		if ((string)Request[Constants.REQUEST_KEY_ACTION_STATUS] != strActionStatus)
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// CSVファイル情報出力
	/// </summary>
	/// <param name="strFileName">ファイル名</param>
	/// <param name="strOutPutInfo">出力内容</param>
	protected void OutPutFileCsv(string strFileName, string strOutPutInfo)
	{
		Response.ContentEncoding = Encoding.GetEncoding("Shift_JIS");
		Response.ContentType = "application/csv";
		Response.AppendHeader("Content-Disposition", "attachment; filename=" + strFileName + ".csv");
		Response.Write(strOutPutInfo);
		Response.End();
	}

	/// <summary>
	/// CSVレコード作成
	/// </summary>
	/// <param name="List<string>">値リスト</param>
	protected string CreateRecordCsv(List<string> lParams)
	{
		StringBuilder sbRecord = new StringBuilder();
		bool firstFlg = true;

		foreach (string str in lParams)
		{
			sbRecord.Append((firstFlg ? "" : ",")).Append(StringUtility.EscapeCsvColumn(str.Replace(",", "")));
			firstFlg = false;
		}
		sbRecord.Append("\r\n");

		return sbRecord.ToString();
	}

	/// <summary>
	/// 文字列を省略する
	/// </summary>
	/// <param name="strValue">指定文字列</param>
	/// <param name="iLength">文字列数</param>
	/// <returns>省略文字列</returns>
	protected string AbbreviateString(string strValue, int iLength)
	{
		string strResult = StringUtility.ToEmpty(strValue);

		// 指定文字列より大きい場合
		if (strResult.Length > iLength)
		{
			strResult = strResult.Substring(0, iLength) + "...";
		}

		return strResult;
	}

	/// <summary>
	/// サイト名取得
	/// </summary>
	/// <param name="strMallId">モールID</param>
	/// <param name="strMallName">モール名</param>
	/// <returns>サイト名（モール名）</returns>
	public static string CreateSiteNameOnly(string strMallId, string strMallName)
	{
		return (strMallId != Constants.FLG_USER_MALL_ID_OWN_SITE) ? strMallName : ValueText.GetValueText("SiteName", "OwnSiteName", Constants.FLG_USER_MALL_ID_OWN_SITE);
	}

	/// <summary>
	/// リスト表示用サイト名取得
	/// </summary>
	/// <param name="strMallId">モールID</param>
	/// <param name="strMallName">モール名</param>
	/// <returns>サイト名（モール名＋モールID）</returns>
	public static string CreateSiteNameForList(string strMallId, string strMallName)
	{
		StringBuilder sbSiteName = new StringBuilder();
		if (strMallId != Constants.FLG_USER_MALL_ID_OWN_SITE)
		{
			sbSiteName.Append(strMallName);
			sbSiteName.Append(" (").Append(strMallId).Append(")");
		}
		else
		{
			sbSiteName.Append(ValueText.GetValueText("SiteName", "OwnSiteName", Constants.FLG_USER_MALL_ID_OWN_SITE));
		}
		return sbSiteName.ToString();
	}

	/// <summary>
	/// 詳細表示用サイト名取得
	/// </summary>
	/// <param name="strMallId">モールID</param>
	/// <param name="strMallName">モール名</param>
	/// <returns>サイト名（モール名＋モールID）</returns>
	public static string CreateSiteNameForDetail(string strMallId, string strMallName)
	{
		StringBuilder sbSiteName = new StringBuilder();
		if (strMallId != Constants.FLG_USER_MALL_ID_OWN_SITE)
		{
			sbSiteName.Append(strMallName);
			sbSiteName.Append(" (モールID：").Append(strMallId).Append(")");
		}
		else
		{
			sbSiteName.Append(ValueText.GetValueText("SiteName", "OwnSiteName", Constants.FLG_USER_MALL_ID_OWN_SITE));
		}
		return sbSiteName.ToString();
	}

	/// <summary>
	/// データ取り出しメソッド
	/// </summary>
	/// <param name="objSrc"></param>
	/// <param name="strKey"></param>
	/// <returns></returns>
	public static object GetKeyValue(object objSrc, string strKey)
	{
		if (objSrc is DataRowView)
		{
			return ((DataRowView)objSrc).Row.Table.Columns.Contains(strKey) ? ((DataRowView)objSrc)[strKey] : null;
		}
		else if (objSrc is IDictionary)
		{
			return ((IDictionary)objSrc)[strKey];
		}

		return null;
	}

	/// <summary>
	/// ブラウザーキャッシュ削除
	/// </summary>
	protected void ClearBrowserCache()
	{
		Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
		Response.Cache.SetCacheability(HttpCacheability.NoCache);
		Response.Cache.SetNoStore();
	}

	#region +CreateUserDetailUrl ユーザ詳細URL作成
	/// <summary>
	/// ユーザ詳細URL作成
	/// </summary>
	/// <param name="userId">ユーザID</param>
	/// <param name="isEditable">編集可能か</param>
	/// <param name="isPopup">メニュー表示するか</param>
	/// <returns>ユーザ詳細URL</returns>
	public string CreateUserDetailUrl(string userId, bool isEditable = true, bool isPopup = true)
	{
		StringBuilder url = new StringBuilder();
		url.Append(Constants.PATH_ROOT_EC).Append(isEditable ? Constants.PAGE_MANAGER_USER_CONFIRM : Constants.PAGE_MANAGER_USER_CONFIRM_POPUP);
		url.Append("?").Append(Constants.REQUEST_KEY_USERID).Append("=").Append(HttpUtility.UrlEncode(userId));
		url.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_DETAIL);
		url.Append("&").Append(Constants.REQUEST_KEY_WINDOW_KBN).Append("=").Append(isPopup ? Constants.KBN_WINDOW_POPUP : Constants.KBN_WINDOW_DEFAULT);
		return url.ToString();
	}
	#endregion

	/// <summary>
	/// Create the target list URL.
	/// </summary>
	/// <param name="parameters">The parameters.</param>
	/// <returns>The target list URL</returns>
	protected string CreateTargetListUrl(Hashtable parameters)
	{
		var queryString = HttpUtility.ParseQueryString(string.Empty);

		foreach (DictionaryEntry param in parameters)
		{
			queryString[StringUtility.ToEmpty(param.Key)] = StringUtility.ToEmpty(param.Value);
		}

		return string.Format("{0}{1}?{2}", Constants.PATH_ROOT, Constants.PAGE_W2MP_MANAGER_TARGETLIST_LIST, queryString);
	}

	/// <summary>
	/// 媒体掲載期間取得
	/// </summary>
	/// <param name="dateFrom">開始日</param>
	/// <param name="dateTo">終了日</param>
	/// <returns>媒体掲載期間</returns>
	public string GetPublicationDateString(DateTime? dateFrom, DateTime? dateTo)
	{
		if (dateFrom.HasValue || dateTo.HasValue)
		{
			var result = string.Format(
				"{0}～{1}",
				DateTimeUtility.ToStringForManager(dateFrom, DateTimeUtility.FormatType.ShortDate2Letter),
				DateTimeUtility.ToStringForManager(dateTo, DateTimeUtility.FormatType.ShortDate2Letter));
			return result;
		}

		return string.Empty;
	}

	/// <summary>
	/// ゼローで２桁の揃える処理
	/// </summary>
	/// <param name="value">値</param>
	/// <returns>ゼローで２桁の揃える文字列</returns>
	public string PadLeftZero2Letter(string value)
	{
		// 空の場合はそのまま返す
		if (string.IsNullOrEmpty(value)) return value;
		var result = value.PadLeft(2, '0');
		return result;
	}

	/// <summary>
	/// 受注詳細詳細URL作成
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <param name="isPopUpAction">ポップアップさせるかどうか</param>
	/// <param name="reloadsParent">表示時に親ウィンドウをリロードするかどうか</param>
	/// <param name="popupParantName">ポップアップ元ページ（遷移元のリロード判定に利用）</param>
	/// <returns>注文情報詳細URL</returns>
	public static string CreateOrderDetailUrl(string orderId, bool isPopUpAction, bool reloadsParent, string popupParantName)
	{
		var url = new UrlCreator(Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_ORDER_CONFIRM)
			.AddParam(Constants.REQUEST_KEY_ORDER_ID, orderId).AddParam(
				Constants.REQUEST_KEY_ACTION_STATUS,
				Constants.ACTION_STATUS_DETAIL);

		if (isPopUpAction)
		{
			url.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP);
		}

		if (reloadsParent)
		{
			url.AddParam(Constants.REQUEST_KEY_RELOAD_PARENT_WINDOW, Constants.KBN_RELOAD_PARENT_WINDOW_ON);
		}

		if (string.IsNullOrEmpty(popupParantName) == false)
		{
			url.AddParam(Constants.REQUEST_KEY_MANAGER_POPUP_PARENT_NAME, popupParantName);
		}

		return url.CreateUrl();
	}

	/// <summary>
	/// レコメンドリストURL作成
	/// </summary>
	/// <returns>レコメンドリストURL</returns>
	protected static string CreateRecommendListUrl()
	{
		var url = new UrlCreator(Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_RECOMMEND_LIST)
			.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP)
			.CreateUrl();
		return url;
	}

	#region プロパティ
	/// <summary>RawUrl（IISのバージョンによる機能の違いを吸収）</summary>
	public string RawUrl
	{
		get { return w2.Common.Web.WebUtility.GetRawUrl(Request); }
	}
	/// <summary>ログイン店舗オペレータ</summary>
	protected ShopOperatorModel LoginShopOperator
	{
		get { return (ShopOperatorModel)Session[Constants.SESSION_KEY_LOGIN_SHOP_OPERTOR] ?? new ShopOperatorModel(); }
	}
	/// <summary>ログインオペレータ店舗ID</summary>
	protected string LoginOperatorShopId
	{
		get { return this.LoginShopOperator.ShopId; }
	}
	/// <summary>ログインオペレータ識別ID</summary>
	protected string LoginOperatorDeptId
	{
		get { return this.LoginShopOperator.ShopId; }
	}
	/// <summary>ログインオペレータID</summary>
	protected string LoginOperatorId
	{
		get { return this.LoginShopOperator.OperatorId; }
	}
	/// <summary>ログインオペレータ名</summary>
	protected string LoginOperatorName
	{
		get { return this.LoginShopOperator.Name; }
	}
	/// <summary>ログインオペレータメニュー</summary>
	protected List<MenuLarge> LoginOperatorMenu
	{
		get { return (List<MenuLarge>)Session[Constants.SESSION_KEY_LOGIN_OPERTOR_MENU]; }
	}
	/// <summary>ログインオペレータメニュー権限</summary>
	protected string LoginOperatorMenuAccessLevel
	{
		get { return Session[Constants.SESSION_KEY_LOGIN_OPERTOR_MENU_ACCESS_LEVEL].ToString(); }
	}
	/// <summary>アクションステータス</summary>
	protected string ActionStatus
	{
		get { return Request[Constants.REQUEST_KEY_ACTION_STATUS]; }
	}
	/// <summary>サイトのCSSクラス名</summary>
	protected string SiteCssClassName
	{
		get
		{
			if (this.IsW2Mp) return "";
			if (this.IsRepeatPlus) return "repeatplus";
			if (this.IsRepeatFood) return "repeatfood";
			return "hanyou";
		}
	}
	/// <summary>w2MPか</summary>
	protected bool IsW2Mp
	{
		get { return (this.ManagerDesingSettingDirName == Constants.KBN_MANAGER_DESIGN_SETTING_W2); }
	}
	/// <summary>RepeatPlusか</summary>
	protected bool IsRepeatPlus
	{
		get { return (this.ManagerDesingSettingDirName == Constants.KBN_MANAGER_DESIGN_SETTING_REPEATPLUS); }
	}
	/// <summary>RepeatFoodか</summary>
	protected bool IsRepeatFood
	{
		get { return (this.ManagerDesingSettingDirName == Constants.KBN_MANAGER_DESIGN_SETTING_REPEATFOOD); }
	}
	/// <summary>w2製品か</summary>
	protected bool IsW2Product
	{
		get { return (this.IsW2Mp || this.IsRepeatPlus || this.IsRepeatFood); }
	}
	/// <summary>管理画面デザイン管理ディレクトリ名</summary>
	protected string ManagerDesingSettingDirName
	{
		get { return Constants.MANAGER_DESIGN_SETTING; }
	}
		/// <summary>Display NotSearch Default</summary>
	protected bool IsNotSearchDefault
	{
		get { return ((Request.QueryString.AllKeys.Length == 0) && (Constants.DISPLAY_NOT_SEARCH_DEFAULT)); }
	}
	/// <summary>商品税込み表示フラグ</summary>
	public bool ProductIncludedTaxFlg
	{
		get { return Constants.MANAGEMENT_INCLUDED_TAX_FLAG; }
	}
	/// <summary>商品価格区分表示文言</summary>
	public string ProductPriceTextPrefix
	{
		get { return TaxCalculationUtility.GetTaxTypeText(); }
	}
	/// <summary> 日付形式タイプ </summary>
	protected string DateFormatType
	{
		get
		{
			if (Constants.GLOBAL_OPTION_ENABLE == false) return string.Empty;
			return GlobalConfigUtil.GetDateTimeFormatText(
					Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE,
					DateTimeUtility.FormatType.General)
				.ToLower();
		}
	}
	/// <summary> 「MDY」日付形式タイプか </summary>
	public bool IsMdyFormat { get { return (this.DateFormatType == "mdy"); } }
	/// <summary> 「DMY」日付形式タイプか </summary>
	public bool IsDmyFormat { get { return (this.DateFormatType == "dmy"); } }
	#endregion
}
