/*
=========================================================================================================
  Module      : 基底マスタページ(BaseMasterPage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Text;
using w2.App.Common.Option;
using w2.App.Common.Web.Process;
using w2.Domain.MemberRank;
using w2.Domain.User;

///*********************************************************************************************
/// <summary>
/// 基底マスタページ
/// </summary>
///*********************************************************************************************
public class BaseMasterPage : System.Web.UI.MasterPage
{
	/// <summary>
	/// ページ初期化
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected virtual void Page_Init(object sender, EventArgs e)
	{
	}

	/// <summary>
	/// UpdatePanel内でのエラーハンドラ
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void smScriptManager_AsyncPostBackError(object sender, System.Web.UI.AsyncPostBackErrorEventArgs e)
	{
		AppLogger.WriteError(e.Exception);

		// Set description for check Invalid ViewState on Application_Error
		Response.StatusDescription = Constants.ASYNC_POSTBACK_ERROR;

		// デバッグモードである時に詳細なエラー内容をエラー画面に表示のため、セッションにエラー内容を保持する
		// リリース時はメッセージそのまま
#if DEBUG
		Session[Constants.SESSION_KEY_ERROR_MSG] = AppLogger.CreateExceptionMessage(e.Exception);
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
#endif
	}

	/// <summary>
	/// ログインしていなければログインページにとばす
	/// </summary>
	/// <remarks>遷移後は元のページに戻る</remarks>
	public void CheckLoggedIn()
	{
		CheckLoggedIn(this.RawUrl);
	}
	/// <summary>
	/// ログインしていなければ指定ページにとばす
	/// </summary>
	/// <param name="strPagePath">ログイン後の遷移先</param>
	public void CheckLoggedIn(string strPagePath)
	{
		if (this.IsLoggedIn == false)
		{
			// ログインページへ（エラーメッセージ、ログイン後遷移URLをわたす）
			StringBuilder sbUrl = new StringBuilder();
			sbUrl.Append(this.SecurePageProtocolAndHost).Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_LOGIN);
			sbUrl.Append("?").Append(Constants.REQUEST_KEY_NEXT_URL).Append("=").Append(HttpUtility.UrlEncode(strPagePath));
			sbUrl.Append("&").Append(Constants.REQUEST_KEY_ERRORPAGE_FRONT_ERRORKBN).Append("=").Append(WebMessages.ERRMSG_FRONT_NO_USER_SESSION);

			Response.Redirect(sbUrl.ToString());
		}
	}

	/// <summary>
	/// CSSリンク設定
	/// </summary>
	/// <param name="strControlId"></param>
	/// <param name="strCssPath"></param>
	protected void SetCssLink(string strControlId, string strCssPath)
	{
		HtmlLink hlCssLink = (HtmlLink)this.Page.Form.FindControl(strControlId);
		if (hlCssLink != null)
		{
			hlCssLink.Href = strCssPath;
		}
	}

	/// <summary>
	/// サイト変更URL作成
	/// </summary>
	/// <param name="Request">HTTPリクエスト</param>
	/// <param name="strChangeSiteKbn">サイト変更区分</param>
	/// <returns>サイト変更URL</returns>
	public static string CreateChangeSiteUrl(HttpRequest Request, string strChangeSiteKbn)
	{
		bool blAddParam = false;

		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(w2.Common.Web.WebUtility.GetRawUrl(Request).Split('?')[0]);
		foreach (string strKey in Request.QueryString.AllKeys)
		{
			if (strKey != Constants.REQUEST_KEY_CHANGESITE)
			{
				sbUrl.Append(blAddParam ? "&" : "?");
				sbUrl.Append(strKey).Append("=").Append(HttpUtility.UrlEncode(Request[strKey]));

				blAddParam = true;
			}
		}
		sbUrl.Append(blAddParam ? "&" : "?");
		sbUrl.Append(Constants.REQUEST_KEY_CHANGESITE).Append("=").Append(strChangeSiteKbn);

		return sbUrl.ToString();
	}

	/// <summary>
	/// カートオブジェクトをかえす
	/// </summary>
	/// <returns></returns>
	protected w2.App.Common.Order.CartObjectList GetCartObjectList()
	{
		return SessionSecurityManager.GetCartObjectList(Context, this.IsPc ? Constants.FLG_ORDER_ORDER_KBN_PC : Constants.FLG_ORDER_ORDER_KBN_SMARTPHONE);
	}

	/// <summary>
	/// SEO設定とOGPタグ設定を設定
	/// </summary>
	public void SetSeoAndOgpTagSetting()
	{
		if (Constants.SEOTAGDISPSETTING_OPTION_ENABLED == false) return;

		var className = this.Page.GetType().BaseType.FullName;

		// 商品詳細でSEOタグとOgpタグを利用しない場合
		if((className == Constants.CLASS_NAME_PRODUCT_DETAIL) && (Constants.SEOTAG_AND_OGPTAG_IN_PRODUCTDETAIL_ENABLED == false)) return;

		if ((className == Constants.CLASS_NAME_PRODUCT_LIST) && (Constants.SEOTAG_IN_PRODUCTLIST_ENABLED == false)) return;

		var utility = new SeoUtility(
			this.Request,
			this.Page,
			this.Session);

		this.Seo = utility.SeoData;
		utility.OgpData.ImageUrl = EncodeFileName(utility.OgpData.ImageUrl);
		this.Ogp = utility.OgpData;

		this.Page.Title = this.Seo.HtmlTitle;
	}

	/// <summary>
	/// ファイル名をエンコードする
	/// </summary>
	/// <param name="url">Url</param>
	/// <returns>ファイル名がエンコードされたUrl</returns>
	private static string EncodeFileName(string url)
	{
		if (string.IsNullOrEmpty(url)) return url;
		var path = Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC + Constants.PATH_OGP_TAG_IMAGE;
		var fileName = HttpUtility.UrlEncode(Path.GetFileName(url));

		return path + fileName;
	}

	/// <summary>SEO設定</summary>
	public SeoUtility.Seo Seo { get; set; }
	/// <summary>OGP設定</summary>
	public SeoUtility.Ogp Ogp { get; set; }
	/// <summary>SEOタグを表示するか</summary>
	public bool IsDisplayedSeoTag
	{
		get
		{
			if (Constants.SEOTAGDISPSETTING_OPTION_ENABLED == false) return false;
			var className = this.Page.GetType().BaseType.FullName;
			if ((className == Constants.CLASS_NAME_PRODUCT_DETAIL) && (Constants.SEOTAG_AND_OGPTAG_IN_PRODUCTDETAIL_ENABLED == false)) return false;
			if ((className == Constants.CLASS_NAME_PRODUCT_LIST) && (Constants.SEOTAG_IN_PRODUCTLIST_ENABLED == false)) return false;

			return true;
		}
	}
	/// <summary>OGPタグを表示するか</summary>
	public bool IsDisplayedOgpTag
	{
		get
		{
			if (Constants.SEOTAGDISPSETTING_OPTION_ENABLED == false) return false;
			var className = this.Page.GetType().BaseType.FullName;
			if ((className == Constants.CLASS_NAME_PRODUCT_DETAIL) && (Constants.SEOTAG_AND_OGPTAG_IN_PRODUCTDETAIL_ENABLED == false))
			{
				return false;
			}

			return true;
		}
	}
	/// <summary>マイページ内判定</summary>
	[Obsolete("[V5.5] 使用しないのであれば削除します")]
	protected bool IsMyPage { get { return this.DispMyPageMenu; } }
	/// <summary>マイページメニュー表示判定</summary>
	protected bool DispMyPageMenu
	{
		get { return (this.Page is BasePage) ? ((BasePage)this.Page).DispMyPageMenu : false; }
	}
	/// <summary>ページアクセスタイプ</summary>
	protected BasePage.PageAccessTypes PageAccessType
	{
		get { return (this.Page is BasePage) ? ((BasePage)this.Page).PageAccessType : BasePage.PageAccessTypes.NoAction; }
	}
	/// <summary>ログイン必須判定</summary>
	protected bool NeedsLogin
	{
		get { return (this.Page is BasePage) ? ((BasePage)this.Page).NeedsLogin : false; }
	}
	/// <summary>プロセス</summary>
	protected BasePageProcess Process
	{
		get { return (BasePageProcess)this.ProcessTemp; }
	}
	/// <summary>プロセステンポラリ</summary>
	protected IPageProcess ProcessTemp
	{
		get
		{
			if (m_processTmp == null) m_processTmp = new BasePageProcess(this, this.ViewState, this.Context);
			return m_processTmp;
		}
	}
	protected IPageProcess m_processTmp = null;
	#region "各ペースページモジュール共通"
	/// <summary>セキュアページプロトコル取得</summary>
	public string SecurePageProtocolAndHost { get { return this.Process.SecurePageProtocolAndHost; } }
	/// <summary>非セキュアページプロトコル取得</summary>
	public string UnsecurePageProtocolAndHost { get { return this.Process.UnsecurePageProtocolAndHost; } }
	/// <summary>RawUrl（IISのバージョンによる機能の違いを吸収）</summary>
	public string RawUrl { get { return this.Process.RawUrl; } }

	/// <summary>PCかどうか</summary>
	protected bool IsPc { get { return this.Process.IsPc; } }
	/// <summary>スマートフォンかどうか</summary>
	protected bool IsSmartPhone { get { return this.Process.IsSmartPhone; } }
	/// <summary>モバイルかどうか</summary>
	protected bool IsMobile { get { return this.Process.IsMobile; } }
	/// <summary>PCサイト変更URL</summary>
	protected string ChangeToPcSiteUrl { get { return this.Process.ChangeToPcSiteUrl; } }
	/// <summary>スマートフォンサイト変更URL</summary>
	protected string ChangeToSmartPhoneSiteUrl { get { return this.Process.ChangeToSmartPhoneSiteUrl; } }
	/// <summary>プレビューか?</summary>
	public bool IsPreview { get { return this.Process.IsPreview; } }

	/// <summary>ログイン状態</summary>
	public bool IsLoggedIn { get { return this.Process.IsLoggedIn; } }
	/// <summary>ログインユーザー</summary>
	public UserModel LoginUser
	{
		get { return this.Process.LoginUser; }
		set { this.Process.LoginUser = value; }
	}
	/// <summary>ログインユーザーID</summary>
	public string LoginUserId
	{
		get { return this.Process.LoginUserId; }
		set { this.Process.LoginUserId = value; }
	}
	/// <summary>ログインユーザー名</summary>
	public string LoginUserName
	{
		get { return this.Process.LoginUserName; }
		set { this.Process.LoginUserName = value; }
	}
	/// <summary>ログインユーザーニックネーム</summary>
	public string LoginUserNickName { get { return this.Process.LoginUserNickName; } }
	/// <summary>ログインユーザーメールアドレス</summary>
	public string LoginUserMail { get { return this.Process.LoginUserMail; } }
	/// <summary>ログインユーザーメールアドレス2</summary>
	public string LoginUserMail2 { get { return this.Process.LoginUserMail2; } }
	/// <summary>ログインユーザー生年月日</summary>
	public string LoginUserBirth { get { return this.Process.LoginUserBirth; } }
	/// <summary>会員ランクID</summary>
	public string MemberRankId { get { return this.Process.MemberRankId; } }
	/// <summary>ログインユーザー会員ランクID</summary>
	public string LoginUserMemberRankId { get { return this.Process.LoginUserMemberRankId; } }
	/// <summary>会員ランク情報</summary>
	public MemberRankModel LoginMemberRankInfo
	{
		get { return this.Process.LoginMemberRankInfo; }
		set { this.Process.LoginMemberRankInfo = value; }
	}
	/// <summary>ページ参照日</summary>
	public DateTime ReferenceDateTime
	{
		get { return this.Process.ReferenceDateTime; }
		set { this.Process.ReferenceDateTime = value; }
	}
	/// <summary>ページ参照会員ランク</summary>
	public MemberRankModel ReferenceMemgbeRankModel
	{
		get { return this.Process.ReferenceMemgbeRankModel; }
		set { this.Process.ReferenceMemgbeRankModel = value; }
	}
	/// <summary>ページ参照ターゲットリスト</summary>
	public string[] ReferenceTargetList
	{
		get { return this.Process.ReferenceTargetList; }
		set { this.Process.ReferenceTargetList = value; }
	}
	/// <summary>EFOオプション有効か</summary>
	public bool IsEfoOptionEnabled
	{
		get { return this.Process.IsEfoOptionEnabled; }
	}
	/// <summary>会員ランク名</summary>
	public string MemberRankName { get { return this.Process.MemberRankName; } }
	/// <summary>User Fixed Purchase Member Flg</summary>
	public string UserFixedPurchaseMemberFlg { get { return this.Process.UserFixedPurchaseMemberFlg; } }
	/// <summary>ログインユーザー定期会員フラグ</summary>
	public string LoginUserFixedPurchaseMemberFlg { get { return this.Process.LoginUserFixedPurchaseMemberFlg; } }
	/// <summary>ログインユーザーCPMクラスタ名</summary>
	public string LoginUserCpmClusterName { get { return this.Process.LoginUserCpmClusterName; } }
	/// <summary>前回ログイン日時</summary>
	public string LastLoggedinDate { get { return this.Process.LastLoggedinDate; } }
	/// <summary>ログインユーザかんたん会員フラグ</summary>
	public string LoginUserEasyRegisterFlg { get { return this.Process.LoginUserEasyRegisterFlg; } }
	/// <summary>かんたん会員かどうか</summary>
	public bool IsEasyUser { get { return this.Process.IsEasyUser; } }
	/// <summary>Amazonログイン状態</summary>
	public bool IsAmazonLoggedIn { get { return this.Process.IsAmazonLoggedIn; } }
	/// <summary>楽天IDConnect会員登録か</summary>
	protected bool IsRakutenIdConnectUserRegister { get { return this.Process.IsRakutenIdConnectUserRegister; } }

	/// <summary>利用可能ポイント</summary>
	public decimal LoginUserPointUsable { get { return this.Process.LoginUserPointUsable; } }
	/// <summary>仮ポイント合計</summary>
	public decimal LoginUserPointTemp { get { return this.Process.LoginUserPointTemp; } }
	/// <summary>通常本ポイント有効期限</summary>
	public DateTime? LoginUserPointExpiry { get { return this.Process.LoginUserPointExpiry; } }
	/// <summary>通常本ポイント数</summary>
	public decimal LoginUserBasePoint { get { return this.Process.LoginUserBasePoint; } }
	/// <summary>期間限定ポイント所有しているか</summary>
	public bool HasLimitedTermPoint { get { return this.Process.HasLimitedTermPoint; } }
	/// <summary>利用可能期間限定ポイント合計</summary>
	public decimal LoginUserLimitedTermPointUsableTotal { get { return this.Process.LoginUserLimitedTermPointUsableTotal; } }
	/// <summary>利用可能期間前期間限定ポイント合計（仮ポイントは除く）</summary>
	public decimal LoginUserLimitedTermPointUnusableTotal { get { return this.Process.LoginUserLimitedTermPointUnusableTotal; } }
	/// <summary>期間限定ポイント合計</summary>
	public decimal LoginUserLimitedTermPointTotal { get { return this.Process.LoginUserLimitedTermPointTotal; } }
	/// <summary>定期購入に利用できるポイント</summary>
	public decimal LoginUserPointUsableForFixedPurchase { get { return this.Process.LoginUserPointUsableForFixedPurchase; } }
	/// <summary>ユーザーポイント</summary>
	public UserPointObject LoginUserPoint { get { return this.Process.LoginUserPoint; } }
	/// <summary>ログインユーザに有効なターゲットリスト群</summary>
	public string[] LoginUserHitTargetListIds
	{
		get { return this.Process.LoginUserHitTargetListIds; }
		set { this.Process.LoginUserHitTargetListIds = value; }
	}

	/// <summary>ターゲットページ</summary>
	public string SessionParamTargetPage { get { return this.Process.SessionParamTargetPage; } }
	#endregion

	/// <summary>OnLoadイベント</summary>
	public string OnLoadEvents
	{
		get { return (string)ViewState["OnLoadEvents"]; }
		set { ViewState["OnLoadEvents"] = value; }
	}
	/// <summary>カートリストランディングページかどうか</summary>
	protected bool IsCartListLp
	{
		get
		{
			var path = StringUtility.ToEmpty(Request.Url.AbsolutePath);
			var beforePath = ((Request.UrlReferrer != null) ? Request.UrlReferrer.AbsolutePath : string.Empty);
			var returnUrl = Request[Constants.REQUEST_KEY_RETURN_URL] ?? string.Empty;
			if (Constants.CART_LIST_LP_OPTION
				&& (path.ToUpper().Contains(Constants.CART_LIST_LP_PAGE_NAME)
					|| beforePath.ToUpper().Contains(Constants.CART_LIST_LP_PAGE_NAME)
					|| returnUrl.ToUpper().Contains(Constants.CART_LIST_LP_PAGE_NAME)))
			{
				return true;
			}
			return false;
		}
	}
}
