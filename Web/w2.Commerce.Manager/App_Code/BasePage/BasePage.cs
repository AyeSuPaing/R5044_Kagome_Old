/*
=========================================================================================================
  Module      : 基底ページ(BasePage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.Global;
using w2.App.Common.Global.Config;
using w2.App.Common.Manager;
using w2.App.Common.Manager.Menu;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.Common.Web;
using w2.Domain.FieldMemoSetting;
using w2.Domain.GlobalZipcode;
using w2.Domain.ShopOperator;
using w2.Domain.User;
using w2.Domain.User.Helper;

/// <summary>
/// BasePage の概要の説明です
/// </summary>
public partial class BasePage : w2.App.Common.Web.Page.CommonPage
{
	// ビューステート共通キー
	protected const string VIEWSTATE_KEY = "vskey";

	/// <summary>
	/// ページ初期化
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks>Page.MasterPageFile の書き換えは Page_PreInitでないといけない</remarks>
	public void Page_PreInit(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// HTTPS通信チェック（HTTPの場合、HTTPSで再読込）
			//------------------------------------------------------
			CheckHttps(Constants.PROTOCOL_HTTPS + Request.Url.Authority + this.RawUrl);

			//------------------------------------------------------
			// ポップアップ画面リダイレクト処理
			// （リファラにポップアップフラグが含まれていたら、パラメタ付加してリダイレクト）
			//------------------------------------------------------
			if ((this.IsPopUp == false)
				&& (Request.UrlReferrer != null))
			{
				if (HttpUtility.ParseQueryString(Request.UrlReferrer.Query)[Constants.REQUEST_KEY_WINDOW_KBN] == Constants.KBN_WINDOW_POPUP)
				{
					StringBuilder sbRedirectUrl = new StringBuilder(Request.Url.PathAndQuery);
					sbRedirectUrl.Append((Request.Url.Query == "") ? "?" : "&").Append(Constants.REQUEST_KEY_WINDOW_KBN).Append("=").Append(Constants.KBN_WINDOW_POPUP);

					Response.Redirect(sbRedirectUrl.ToString());
				}
			}
		}

		//------------------------------------------------------
		// ポップアップフラグが立っていたらポップアップマスタを読み込み（ポストバック時も呼び出す必要あり）
		//------------------------------------------------------
		if (Page.Master != null) // ログインページ対策
		{
			if (this.IsPopUp)
			{
				Page.MasterPageFile = "~/Form/Common/PopupPage.master";
			}
			// ZEUSタブレットの場合はデフォルトページ補正を行う
			else if (SessionManager.UsePaymentTabletZeus && this.IsNotPopUp)
			{
				Page.MasterPageFile = "~/Form/Common/DefaultPage.master";
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
	/// Clear Cache of Browser
	/// </summary>
	protected void ClearBrowserCache()
	{
		Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
		Response.Cache.SetCacheability(HttpCacheability.NoCache);
		Response.Cache.SetNoStore();
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
		if (this.ActionStatus != strActionStatus)
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// ユーザーシンボル取得
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	/// <returns>ユーザーシンボル</returns>
	public static string GetUserSymbol(string userId)
	{
		return GetUserSymbols(userId).First().Value;
	}

	/// <summary>
	/// ユーザーシンボル取得
	/// </summary>
	/// <param name="userIds">ユーザーIDリスト</param>
	/// <returns>ユーザーシンボル</returns>
	protected static KeyValuePair<string, string>[] GetUserSymbols(params string[] userIds)
	{
		var useList = new UserService().GetUserSymbols(userIds);
		return (from userId in userIds
				let user = useList.FirstOrDefault(u => u.UserId == userId)
				select new KeyValuePair<string, string>(userId, ConvertToUserSymbols(user))).ToArray();
	}

	/// <summary>
	/// ユーザーシンボルへ変換
	/// </summary>
	/// <param name="userInfo">ユーザーシンボル向けユーザー情報</param>
	/// <returns>ユーザーシンボル</returns>
	private static string ConvertToUserSymbols(UserSymbols user)
	{
		if (user == null) return "";
		return string.Join("", ConvertToUserSymbols(user.OrderCount, StringUtility.ToEmpty(user.UserMemo)));
	}
	/// <summary>
	/// ユーザーシンボルへ変換
	/// </summary>
	/// <param name="orderCount">注文件数</param>
	/// <param name="userMemo">ユーザ特記欄</param>
	/// <returns>ユーザーシンボル</returns>
	private static IEnumerable<string> ConvertToUserSymbols(int orderCount, string userMemo)
	{
		// 注文件数が2件以上であればリピーターユーザーシンボル追加
		yield return (orderCount >= 2) ? Constants.USERSYMBOL_REPEATER : "";

		// ユーザ特記欄に入力があれば特記ユーザーシンボル追加
		yield return (string.IsNullOrEmpty(userMemo) == false) ? Constants.USERSYMBOL_HAS_NOTE : "";
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
	/// プレビュー用URL作成
	/// </summary>
	/// <param name="strUrl">URL</param>
	/// <returns>プレビュー用URL</returns>
	public static string CreatePreviewUrl(string strUrl)
	{
		StringBuilder sbCreateUrl = new StringBuilder();

		if (strUrl != "")
		{
			if ((strUrl.ToLower().IndexOf(Constants.PROTOCOL_HTTP) == 0)
				|| strUrl.ToLower().IndexOf(Constants.PROTOCOL_HTTPS) == 0)
			{
				sbCreateUrl.Append(strUrl);
			}
			else
			{
				sbCreateUrl.Append(Constants.URL_FRONT_PC).Append(strUrl);
			}
		}

		return sbCreateUrl.ToString();
	}

	/// <summary>
	/// サイト名取得
	/// </summary>
	/// <param name="mallId">モールID</param>
	/// <param name="mallName">モール名</param>
	/// <returns>サイト名（モール名）</returns>
	public static string CreateSiteNameOnly(string mallId, string mallName)
	{
		var result = ((mallId != Constants.FLG_USER_MALL_ID_OWN_SITE)
			&& (Constants.FLG_USER_MALL_ID_EXTERNAL_ORDER_SITES.Any(site => site == mallId) == false))
				? mallId
				: ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_SITENAME,
					Constants.VALUETEXT_PARAM_OWNSITENAME,
					Constants.FLG_USER_MALL_ID_EXTERNAL_ORDER_SITES.Any(site => site == mallId)
						? mallId
						: Constants.FLG_USER_MALL_ID_OWN_SITE);
		return result;
	}

	/// <summary>
	/// リスト表示用サイト名取得
	/// </summary>
	/// <param name="mallId">モールID</param>
	/// <param name="mallName">モール名</param>
	/// <returns>サイト名（モール名＋モールID）</returns>
	public static string CreateSiteNameForList(string mallId, string mallName)
	{
		var siteName = new StringBuilder();
		if ((mallId != Constants.FLG_USER_MALL_ID_OWN_SITE)
			&& (Constants.FLG_USER_MALL_ID_EXTERNAL_ORDER_SITES.Any(site => site == mallId) == false))
		{
			siteName.Append(mallName);
			siteName.AppendFormat(" ({0})", mallId);
		}
		else
		{
			siteName.Append(
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_SITENAME,
					Constants.VALUETEXT_PARAM_OWNSITENAME,
					Constants.FLG_USER_MALL_ID_EXTERNAL_ORDER_SITES.Any(site => site == mallId)
						? mallId
						: Constants.FLG_USER_MALL_ID_OWN_SITE));
		}
		return siteName.ToString();
	}

	/// <summary>
	/// 詳細表示用サイト名取得
	/// </summary>
	/// <param name="mallId">モールID</param>
	/// <param name="mallName">モール名</param>
	/// <returns>サイト名（モール名＋モールID）</returns>
	public static string CreateSiteNameForDetail(string mallId, string mallName)
	{
		return OrderCommon.CreateSiteNameForDetail(mallId, mallName);
	}

	/// <summary>
	/// 決済種別情報取得取得
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <returns>決済種別情報取得報データビュー</returns>
	protected static DataView GetPaymentValidList(string shopId)
	{
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("Payment", "GetPaymentValidList"))
		{
			Hashtable sqlParam = new Hashtable();
			sqlParam.Add(Constants.FIELD_PAYMENT_SHOP_ID, shopId);

			var dv = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, sqlParam);

			if (Constants.AMAZON_PAYMENT_OPTION_ENABLED == false)
			{
				dv.RowFilter = string.Format("{0} <> '{1}' ", Constants.FIELD_PAYMENT_PAYMENT_ID, Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT);
				dv.RowFilter = string.Format("{0} <> '{1}' ", Constants.FIELD_PAYMENT_PAYMENT_ID, Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2);
			}

			if (Constants.PAYMENT_LINEPAY_OPTION_ENABLED == false)
			{
				dv.RowFilter = string.Format("{0} <> '{1}' ", Constants.FIELD_PAYMENT_PAYMENT_ID, Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY);
			}

			if (Constants.PAYMENT_GMO_POST_ENABLED == false)
			{
				dv.RowFilter = string.Format("{0} <> '{1}' AND {0} <> '{2}'", Constants.FIELD_PAYMENT_PAYMENT_ID, Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO, Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE);
			}

			if (Constants.PAYMENT_GMOATOKARA_ENABLED == false)
			{
				dv.RowFilter = string.Format("{0} <> '{1}'", Constants.FIELD_PAYMENT_PAYMENT_ID, Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA);
			}

			return dv;
		}
	}

	/// <summary>
	/// 項目メモ一覧取得処理
	/// </summary>
	/// <param name="tableName">テーブル名</param>
	/// <returns>項目メモ一覧データ</returns>
	public static FieldMemoSettingModel[] GetFieldMemoSettingList(string tableName)
	{
		var memoList = new FieldMemoSettingService().GetFieldMemoSettingListClip(tableName);
		return memoList;
	}

	/// <summary>
	/// モール連携設定一覧情報取得
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <returns>モール連携設定一覧情報</returns>
	protected DataView GetMallCooperationSettingList(string shopId)
	{
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MallLiaise", "GetMallCooperationSettingListFromShopId"))
		{
			Hashtable sqlParam = new Hashtable();
			sqlParam.Add(Constants.FIELD_MALLCOOPERATIONSETTING_SHOP_ID, this.LoginOperatorShopId);

			return sqlStatement.SelectSingleStatement(sqlAccessor, sqlParam);
		}
	}

	/// <summary>
	/// データ取り出しメソッド
	/// </summary>
	/// <param name="objSrc"></param>
	/// <param name="strKey"></param>
	/// <returns></returns>
	public static object GetKeyValue(object objSrc, string strKey)
	{
		return ProductCommon.GetKeyValue(objSrc, strKey);
	}

	/// <summary>
	/// データ取り出しメソッド（DBNullをnull扱いにする）
	/// </summary>
	/// <param name="src">ソース</param>
	/// <param name="key">キー</param>
	/// <returns>データ</returns>
	public static object GetKeyValueToNull(object src, string key)
	{
		return ProductCommon.GetKeyValueToNull(src, key);
	}

	/// <summary>
	/// チェックボックスリストのチェック設定を行う
	/// </summary>
	/// <param name="cblTarget">対象チェックボックスリスト</param>
	/// <param name="strValues">値配列</param>
	public void SetSearchCheckBoxValue(CheckBoxList cblTarget, string[] strValues)
	{
		foreach (ListItem liItem in cblTarget.Items)
		{
			liItem.Selected = false;
			foreach (string strValue in strValues)
			{
				if (liItem.Value == strValue)
				{
					liItem.Selected = true;
					break;
				}
			}
		}
	}
	/// <summary>
	/// 検索条件設定入力抽出データ取得
	/// </summary>
	/// <param name="licCollection"></param>
	/// <param name="isOrderWorkFlowPaymnetAdd">受注ワークフローでの実行かどうか</param>
	/// <returns></returns>
	public string CreateSearchStringParts(ListItemCollection licCollection, bool isOrderWorkFlowPaymnetAdd = false)
	{
		StringBuilder searchSettingString = new StringBuilder();
		foreach (ListItem liItem in licCollection)
		{
			if (liItem.Selected)
			{
				searchSettingString.Append(searchSettingString.Length != 0 ? "," : "").Append(liItem.Value);

				if (isOrderWorkFlowPaymnetAdd
					&& (liItem.Value == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2))
				{
					searchSettingString
						.Append(searchSettingString.Length != 0 ? "," : "")
						.Append(Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT);
				}
			}
		}
		return searchSettingString.ToString();
	}

	/// <summary>
	/// ハッシュからクエリストリングを生成しURLに付加する
	/// </summary>
	/// <param name="strUrl">対象URL</param>
	/// <param name="dictParam">リクエストパラメーター</param>
	/// <param name="blPageNumberAdd">ページ番号を付加するか</param>
	/// <returns></returns>
	public string CreateQueryStringUrl(string strUrl, IDictionary dictParam, bool blPageNumberAdd)
	{
		if (blPageNumberAdd)
		{
			return CreateQueryStringUrl(strUrl, dictParam) + "&" + Constants.REQUEST_KEY_PAGE_NO + "=" + StringUtility.ToEmpty(dictParam[Constants.REQUEST_KEY_PAGE_NO]);
		}
		else
		{
			return CreateQueryStringUrl(strUrl, dictParam);
		}
	}
	/// <summary>
	/// ハッシュからクエリストリングを生成しURLに付加する
	/// </summary>
	/// <param name="strUrl">対象URL</param>
	/// <param name="dictParam">リクエストパラメーター</param>
	/// <returns></returns>
	public string CreateQueryStringUrl(string strUrl, IDictionary dictParam)
	{
		if (dictParam == null) return strUrl;

		var vals = dictParam.Cast<DictionaryEntry>()
			.Where(item => item.Value is string && StringUtility.ToEmpty(item.Key) != Constants.REQUEST_KEY_PAGE_NO)
			.OrderBy(item => item.Key)
			.ToArray();

		if (vals.Any() == false) return strUrl;

		return string.Format("{0}?{1}", strUrl, string.Join("&", vals.Select(item => string.Format("{0}={1}", item.Key, StringUtility.ToEmpty(item.Value)))));
	}

	/// <summary>
	/// 注文拡張ステータス設定一覧取得
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <returns>注文拡張ステータス設定一覧</returns>
	public static DataView GetOrderExtendStatusSettingList(string shopId)
	{
		var input = new Hashtable();
		input.Add(Constants.FIELD_ORDEREXTENDSTATUSSETTING_SHOP_ID, shopId);
		input.Add("extend_status_max", Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX);

		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("OrderExtendStatusSetting", "GetOrderExtendStatusSettingList"))
		{
			return statement.SelectSingleStatementWithOC(accessor, input);
		}
	}

	/// <summary>
	/// 日付値取得
	/// </summary>
	/// <param name="master">マスタ</param>
	/// <param name="key">キー</param>
	/// <param name="defaultValue">デフォルト値</param>
	/// <param name="format">フォーマット</param>
	/// <returns>日付</returns>
	public DateTime? GetDate(object master, string key, DateTime? defaultValue = null, string format = "yyyy/MM/dd HH:mm:ss")
	{
		var date = StringUtility.ToDateString(GetKeyValue(master, key), format);
		return string.IsNullOrEmpty(date) ? defaultValue : DateTime.Parse(date);
	}

	/// <summary>
	/// 国が日本かどうか
	/// </summary>
	/// <param name="countryIsoCode">国ISOコード</param>
	/// <returns>日本か</returns>
	protected static bool IsCountryJp(string countryIsoCode)
	{
		return GlobalAddressUtil.IsCountryJp(countryIsoCode);
	}

	/// <summary>
	/// 国がアメリカかどうか
	/// </summary>
	/// <param name="countryIsoCode">国ISOコード</param>
	/// <returns>アメリカか</returns>
	protected static bool IsCountryUs(string countryIsoCode)
	{
		return GlobalAddressUtil.IsCountryUs(countryIsoCode);
	}

	/// <summary>
	/// 国が台湾かどうか
	/// </summary>
	/// <param name="countryIsoCode">国ISOコード</param>
	/// <returns>台湾か</returns>
	protected static bool IsCountryTw(string countryIsoCode)
	{
		return GlobalAddressUtil.IsCountryTw(countryIsoCode);
	}

	/// <summary>
	/// 郵便番号が必須かどうか
	/// </summary>
	/// <param name="countryIsoCode">国ISOコード</param>
	/// <returns>必須か</returns>
	protected static bool IsAddrZipcodeNecessary(string countryIsoCode)
	{
		return GlobalAddressUtil.IsAddrZipcodeNecessary(countryIsoCode);
	}

	/// <summary>
	/// エラーページへ遷移
	/// <param name="message">メッセージ</param>
	/// </summary>
	protected void RedirectToErrorPage(string message)
	{
		Session[Constants.SESSION_KEY_ERROR_MSG] = message;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
	}

	/// <summary>
	/// Create report json string
	/// </summary>
	/// <param name="data">Data</param>
	/// <returns>Json string</returns>
	public static string CreateReportJsonString(object data)
	{
		return BasePageHelper.ConvertObjectToJsonString(data);
	}

	/// <summary>
	/// Binding address by global zipcode
	/// </summary>
	/// <param name="countryIsoCode">Country ISO Code</param>
	/// <param name="globalZipCode">Global zipcode</param>
	/// <param name="tbAddr2">Textbox address 2</param>
	/// <param name="tbAddr3">Textbox address 3</param>
	/// <param name="tbAddr4">Textbox address 4</param>
	/// <param name="tbAddr5">Textbox address 5</param>
	/// <param name="ddlUsState">Dropdownlist US state</param>
	public static void BindingAddressByGlobalZipcode(
		string countryIsoCode,
		string globalZipCode,
		TextBox tbAddr2,
		TextBox tbAddr3,
		TextBox tbAddr4,
		TextBox tbAddr5,
		DropDownList ddlUsState)
	{
		if (globalZipCode.Length < 3) return;

		var result = new GlobalZipcodeService().Get(
			countryIsoCode,
			StringUtility.ReplaceDelimiter(globalZipCode));
		if (result == null) return;

		// Is country Taiwan
		if (IsCountryTw(countryIsoCode))
		{
			// Set Taiwan address information
			tbAddr2.Text = result.City;
			tbAddr3.Text = result.Address;
			tbAddr4.Text = result.Province;
			return;
		}

		// Set address information
		tbAddr2.Text = result.Address;
		tbAddr4.Text = result.City;
		tbAddr5.Text = result.Province;

		// Is country US
		if (IsCountryUs(countryIsoCode))
		{
			var selectedState = ddlUsState.Items.FindByText(result.Province);
			if (selectedState == null) return;

			// Unselect all items
			foreach (ListItem item in ddlUsState.Items)
			{
				item.Selected = false;
			}

			// Selected state by zip code
			selectedState.Selected = true;
		}
	}

	/// <summary>
	/// Get encoded html display message
	/// </summary>
	/// <param name="message">The message</param>
	/// <returns>Display message has encoded html</returns>
	public string GetEncodedHtmlDisplayMessage(string message)
	{
		if (string.IsNullOrEmpty(message)) return string.Empty;

		if (message.Contains("<br />") || message.Contains("<br/>"))
		{
			message = message.Replace("<br />", Environment.NewLine)
				.Replace("<br/>", Environment.NewLine);
		}

		return WebSanitizer.HtmlEncodeChangeToBr(message);
	}

	/// <summary>
	/// When move to list page, if no data exists redirect to last page that have data
	/// </summary>
	/// <param name="currentCount">Current count</param>
	/// <param name="totalCount">Total count</param>
	/// <param name="pageUrl">Page url</param>
	/// <remarks>If only one item of data is displayed on the page being paging,
	/// deleting that one item of data will result in no information to be displayed on the page.
	/// At this point, there is no more information to display and we need to redirect to the previous page.
	/// This method redirects to the previous page if the page has no information to display.</remarks>
	public void CheckRedirectToLastPage(int currentCount, int totalCount, string pageUrl)
	{
		if ((currentCount == 0)
			&& (totalCount > 0))
		{
			var lastPathNo = ((totalCount - 1) / Constants.CONST_DISP_CONTENTS_DEFAULT_LIST) + 1;
			var result = new StringBuilder(pageUrl);
			if (result.Length != 0)
			{
				result.AppendFormat("&{0}={1}", Constants.REQUEST_KEY_PAGE_NO, lastPathNo);
				Response.Redirect(result.ToString());
			}
		}
	}

	/// <summary>
	/// 許可されたIPアドレスからのアクセスか
	/// </summary>
	/// <param name="allowedIPs">許可IPリスト</param>
	/// <returns>許可されたIPアドレスか</returns>
	public bool IsAllowedIpAddress(List<string> allowedIPs)
	{
		// 開発環境の場合、ローカルのIPアドレスを取得
		var externalIp = new ShopOperatorLoginManager().GetIpAddress();

		var result = ((allowedIPs.Any() == false) || allowedIPs.Contains(externalIp));

		return result;
	}

	/// <summary>
	/// 登録・編集ユーザー情報保存用ユニークキー生成
	/// </summary>
	/// <param name="actionStatus">アクションステータス</param>
	/// <param name="targetUserId">編集対象ユーザーID</param>
	/// <returns></returns>
	protected string CreateUniqueKeyForSaveUserInput(string actionStatus, string targetUserId)
	{
		var result = actionStatus == Constants.ACTION_STATUS_UPDATE
			? targetUserId
			: DateTime.Now.ToString("yyyyMMddHHmmss");
		return result;
	}

	/// <summary>
	/// 登録・編集定期商品変更設定情報保存用ユニークキー生成
	/// </summary>
	/// <param name="actionStatus">アクションステータス</param>
	/// <param name="fixedPurchaseProductChangeId">編集対象定期商品変更ID</param>
	/// <returns></returns>
	protected string CreateUniqueKeyForSaveFixedPurchaseProductChangeSettingInput(string actionStatus, string fixedPurchaseProductChangeId)
	{
		var result = actionStatus == Constants.ACTION_STATUS_UPDATE
			? fixedPurchaseProductChangeId
			: DateTime.Now.ToString("yyyyMMddHHmmss");
		return result;
	}

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
			if (this.IsW2C) return "";
			if (this.IsRepeatPlus) return "repeatplus";
			if (this.IsRepeatFood) return "repeatfood";
			return "hanyou";
		}
	}
	/// <summary>w2MCか</summary>
	protected bool IsW2C
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
	/// <summary>W2製品か</summary>
	protected bool IsW2Product
	{
		get { return (this.IsW2C || this.IsRepeatPlus || this.IsRepeatFood); }
	}
	/// <summary>管理画面デザイン管理ディレクトリ名</summary>
	protected string ManagerDesingSettingDirName
	{
		get { return Constants.MANAGER_DESIGN_SETTING; }
	}
	/// <summary>項目メモ一覧</summary>
	protected FieldMemoSettingModel[] FieldMemoSettingList { get; set; }
	/// <summary>Previous Url</summary>
	protected string PrevUrl
	{
		get
		{
			var prevUrl = CookieManager.GetValue(Constants.COOKIE_KEY_PREV_URL);
			return HttpUtility.UrlDecode(prevUrl ?? "");
		}
	}
	/// <summary>Is Back From Error Page</summary>
	protected bool IsBackFromErrorPage
	{
		get
		{
			Uri prevUri;
			return Uri.TryCreate(this.PrevUrl, UriKind.RelativeOrAbsolute, out prevUri) && prevUri.PathAndQuery.StartsWith(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}
	/// <summary>Display NotSearch Default</summary>
	protected virtual bool IsNotSearchDefault
	{
		get { return ((Request.QueryString.AllKeys.Length == 0) && (Constants.DISPLAY_NOT_SEARCH_DEFAULT)); }
	}
	/// <summary>ポップアップか</summary>
	protected bool IsPopUp
	{
		get { return (Request[Constants.REQUEST_KEY_WINDOW_KBN] == Constants.KBN_WINDOW_POPUP); }
	}
	/// <summary>ポップアップ指定なしか</summary>
	protected bool IsNotPopUp
	{
		get { return (Request[Constants.REQUEST_KEY_WINDOW_KBN] == Constants.KBN_WINDOW_DEFAULT); }
	}
	/// <summary>クレジットカード番号フォーム入力可能か（トークン決済or永久トークン決済）</summary>
	protected bool CanUseCreditCardNoForm
	{
		get { return BasePageHelper.CanUseCreditCardNoForm; }
	}
	/// <summary>クレジットカード仮登録が必要なカード区分か(すべて) ※仮クレカを利用するかはこの条件に加え自社サイト注文かつ新規クレカか等の条件が必要</summary>
	public bool NeedsRegisterProvisionalCreditCardCardKbn
	{
		get { return OrderCommon.NeedsRegisterProvisionalCreditCardCardKbn; }
	}
	/// <summary>クレジットカード仮登録が必要なカード区分か(ZEUS) ※仮クレカを利用するかはこの条件に加え自社サイト注文かつ新規クレカか等の条件が必要</summary>
	public bool NeedsRegisterProvisionalCreditCardCardKbnZeus
	{
		get { return OrderCommon.NeedsRegisterProvisionalCreditCardCardKbnZeus; }
	}
	/// <summary>クレジットカード仮登録が必要なカード区分か(ZEUS以外) ※仮クレカを利用するかはこの条件に加え自社サイト注文かつ新規クレカか等の条件が必要</summary>
	public bool NeedsRegisterProvisionalCreditCardCardKbnExceptZeus
	{
		get { return OrderCommon.NeedsRegisterProvisionalCreditCardCardKbnExceptZeus; }
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
	/// <summary>プレビュー対象HTMLリスト</summary>
	protected List<string> HtmlForPreviewList
	{
		get
		{
			return (Session[Constants.SESSION_KEY_HTML_FOR_PREVIEW_LIST] != null)
				? (List<string>)Session[Constants.SESSION_KEY_HTML_FOR_PREVIEW_LIST]
				: null;
		}
		set { Session[Constants.SESSION_KEY_HTML_FOR_PREVIEW_LIST] = value; }
	}
	/// <summary>リードタイム設定利用可否</summary>
	public bool UseLeadTime
	{
		get { return GlobalConfigUtil.UseLeadTime(); }
	}

	/// <summary>運用地は日本であるか</summary>
	protected bool IsOperationalCountryJp
	{
		get
		{
			return ((Constants.GLOBAL_OPTION_ENABLE == false) || (Constants.OPERATIONAL_BASE_ISO_CODE == Constants.COUNTRY_ISO_CODE_JP));
		}
	}
	/// <summary>日本に配送可能か</summary>
	protected bool IsShippingCountryAvailableJp
	{
		get
		{
			return ((Constants.GLOBAL_OPTION_ENABLE == false)
				|| (ShippingCountryUtil.GetShippingCountryAvailableListAndCheck(Constants.COUNTRY_ISO_CODE_JP)));
		}
	}
	/// <summary>Is Shipping Country Available Taiwan</summary>
	protected bool IsShippingCountryAvailableTw
	{
		get
		{
			return (Constants.TW_COUNTRY_SHIPPING_ENABLE
				&& ShippingCountryUtil.GetShippingCountryAvailableListAndCheck(Constants.COUNTRY_ISO_CODE_TW));
		}
	}
	/// <summary>Prefectures List</summary>
	protected string[] PrefecturesList
	{
		get
		{
			var prefecturesList = Constants.TW_COUNTRY_SHIPPING_ENABLE
				? Constants.TW_CITIES_LIST
				: Constants.STR_PREFECTURES_LIST;
			return prefecturesList;
		}
	}
	/// <summary>利用可能ポイントの定期継続利用が利用可能か</summary>
	protected bool CanUseAllPointFlg
	{
		get
		{
			return (Constants.W2MP_POINT_OPTION_ENABLED
				&& Constants.FIXEDPURCHASE_OPTION_ENABLED
				&& Constants.FIXEDPURCHASE_NEXT_SHIPPING_USE_POINT_ALL_OPTION_ENABLE);
		}
	}
	/// <summary>コンフィグ設定情報</summary>
	public List<SettingNode> AllConfigSettings
	{
		get { return (List<SettingNode>)Session[Constants.SESSIONPARAM_KEY_ALL_CONFIGRATION + this.GuidString]; }
		set { Session[Constants.SESSIONPARAM_KEY_ALL_CONFIGRATION + this.GuidString] = value; }
	}
	/// <summary>各コンフィグの最終更新時間</summary>
	public Dictionary<string, DateTime> ConfigLastUpdatedTimes
	{
		get { return (Dictionary<string, DateTime>)Session[Constants.SESSIONPARAM_KEY_ALL_CONFIGRATION_LAST_UPDATED_TIMES + this.GuidString]; }
		set { Session[Constants.SESSIONPARAM_KEY_ALL_CONFIGRATION_LAST_UPDATED_TIMES + this.GuidString] = value; }
	}
	/// <summary>グローバル一意識別子</summary>
	protected string GuidString
	{
		get
		{
			var newGuid = string.Format("{0:yyyyMMdd}_{1}", DateTime.Now, Guid.NewGuid());
			var currentGuid = Request[Constants.PARAM_GUID_STRING];

			var result = (string.IsNullOrEmpty(currentGuid))
				? newGuid
				: currentGuid;
			return result;
		}
	}
}
