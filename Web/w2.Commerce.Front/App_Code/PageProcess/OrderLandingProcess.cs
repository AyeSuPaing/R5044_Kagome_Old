/*
=========================================================================================================
  Module      : 注文LPプロセス(OrderLandingProcess.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using w2.App.Common.Order;

/// <summary>
/// OrderLandingInputProcess の概要の説明です
/// </summary>
public class OrderLandingProcess : OrderFlowProcess
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="caller">呼び出し元</param>
	/// <param name="viewState">ビューステート</param>
	/// <param name="context">コンテキスト</param>
	public OrderLandingProcess(object caller, StateBag viewState, HttpContext context)
		: base(caller, viewState, context)
	{
	}

	/// <summary>
	/// ページ初期化
	/// </summary>
	public new void Page_Init(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 基底ページ初期処理呼び出し
		//------------------------------------------------------
		base.Page_Init(sender, e);

		LoadLadingCartSession();
	}

	/// <summary>
	/// ------------------------------------------------------
	/// ランディングカートオブジェクトリストロード
	/// ------------------------------------------------------
	/// カートが無い場合は作成（この時点ではレコードは作成されないし、採番もされない）
	///	セッションキーにファイルパス追加（複数ランディングページを使用した場合、意図しないカート情報を取得する可能性がある為）
	///	また、購入完了まではユーザーとLPカートを紐づけさせたくない為、ユーザーIDは必ず空で作成
	///	（購入時にログインしているユーザーはthis.LoginUserIdからユーザーIDを取得して注文とユーザーを紐づけしているので問題ない）
	/// </summary>
	protected void LoadLadingCartSession()
	{
		if (this.Session[this.LadingCartSessionKey] == null)
		{
			this.Session[this.LadingCartSessionKey] = new CartObjectList(
				"",
				this.IsPc ? Constants.FLG_ORDER_ORDER_KBN_PC : Constants.FLG_ORDER_ORDER_KBN_SMARTPHONE,
				true,
				"",
				(this.LoginMemberRankInfo != null) ? this.LoginMemberRankInfo.MemberRankId : "");
		}
		else if (Constants.CART_LIST_LP_OPTION
			&& this.LadingCartSessionKey.ToUpper().Contains(Constants.CART_LIST_LP_PAGE_NAME))
		{
			var lpCartList = (this.IsPreview == false) ? GetCartObjectList() : Preview.GetDummyCart(this.ShopId);
			if (lpCartList.Items.Count != 0)
			{
				this.Session[this.LadingCartSessionKey] = lpCartList;
			}
		}

		this.CartList = (CartObjectList)this.Session[this.LadingCartSessionKey];
		if (this.CartList.Items.Count == 0)
		{
			this.CartList.CheckProductDeleted();
		}

		if (Constants.CART_LIST_LP_OPTION
			&& this.LadingCartSessionKey.ToUpper().Contains(Constants.CART_LIST_LP_PAGE_NAME))
		{
			SessionManager.CartListLp = (CartObjectList)this.Session[this.LadingCartSessionKey];
		}
	}

	/// <summary>
	/// 遷移元チェック
	/// </summary>
	public new void CheckOrderUrlSession()
	{
		// 遷移元で格納されたURLが存在しない場合はトップ画面へ遷移（イレギュラー）
		var nextUrl = GetNextUrlForCheck();
		if ((nextUrl == null))
		{
			Response.Redirect(Constants.PATH_ROOT);
		}

		// 遷移元で格納されたURLと一致しない場合は遷移元で格納されたURLへ遷移
		if (this.RawUrl.IndexOf(nextUrl) == -1)
		{
			Response.Redirect(Constants.PATH_ROOT + nextUrl);
		}
	}

	/// <summary>
	/// パラメタに格納されたNextUrl取得
	/// </summary>
	/// <returns>NextUrlの値（パラメタ無しの場合はnull）</returns>
	public override string GetNextUrlForCheck()
	{
		if (Session[this.LadingCartNextPageForCheck] != null)
		{
			return (string)Session[this.LadingCartNextPageForCheck];
		}
		return null;
	}

	/// <summary>
	/// セッションよりバックアップを復元
	/// </summary>
	public void RestoreCartFromSession()
	{
		this.CartList = (CartObjectList)Session[this.BackupLadingCartSessionKey];
		Session[this.LadingCartSessionKey] = Session[this.BackupLadingCartSessionKey];
		Session[this.BackupLadingCartSessionKey] = null;

		// セッション書き換えてもDB内は変わらないので同期させる
		this.CartList.SyncCartDb();
	}

	/// <summary>ランディングカート入力画面絶対パス</summary>
	public string LandingCartInputAbsolutePath
	{
		get
		{
			var path = this.Request[Constants.REQUEST_KEY_RETURN_URL];
			if (string.IsNullOrEmpty(path)) path = (string)this.Session["landing_cart_input_absolutePath"];
			return path;
		}
		set
		{
			this.Session["landing_cart_input_absolutePath"] = value;
		}
	}
	/// <summary>ランディングカート入力画面URL</summary>
	public string LandingCartInputUrl
	{
		get { return this.SecurePageProtocolAndHost + this.LandingCartInputAbsolutePath; }
	}
	/// <summary>ランディング入力ページ最後に書き込み時刻</summary>
	public string LandingCartInputLastWriteTime
	{
		get { return (string)this.Session[Constants.SESSION_KEY_LANDING_CART_INPUT_LAST_WRITE_TIME]; }
		set { this.Session[Constants.SESSION_KEY_LANDING_CART_INPUT_LAST_WRITE_TIME] = value; }
	}
	/// <summary>ランディングカート保持用セッションキー</summary>
	public string LadingCartSessionKey
	{
		get
		{
			return string.Format(
				"{0}{1}{2}",
				Constants.SESSION_KEY_CART_LIST_LANDING,
				this.LandingCartInputAbsolutePath,
				this.IsCartListLp ? "" : this.LandingCartInputLastWriteTime);
		}
	}
	/// <summary>ランディングカート商品リスト保持用セッションキー</summary>
	public string LandingCartProductListSessionKey
	{
		get { return Constants.SESSION_KEY_PRODUCT_LIST_LANDING + this.LandingCartInputAbsolutePath; }
	}
	/// <summary>ランディングカート保持用画面遷移正当性チェック用セッションキー</summary>
	public string LadingCartNextPageForCheck
	{
		get { return Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK + this.LandingCartInputAbsolutePath; } // ここの定義変えるときはSessionManagerも要変更
	}
	/// <summary>CMSランディングページ用 確認画面スキップフラグ用セッションキー</summary>
	public string LadingCartConfirmSkipFlgSessionKey
	{
		get { return Constants.SESSION_KEY_CMS_LANDING_CART_CONFIRM_SKIP_FLG + this.LandingCartInputAbsolutePath; }
	}
	/// <summary>CMSランディングページ用 商品セット選択用セッションキー</summary>
	public string LadingCartProductSetSelectSessionKey
	{
		get { return Constants.SESSION_KEY_CMS_LANDING_PRODUCT_SET_SELECT + this.LandingCartInputAbsolutePath; }
	}
	/// <summary>CMSランディングページ用 定期購入判定用セッションキー</summary>
	public string LandingCartIsFixedPurchaseSessionKey
	{
		get { return Constants.SESSION_KEY_CMS_LANDING_IS_FIXEDPURCHASE + this.LandingCartInputAbsolutePath; }
	}
	/// <summary>カートリストランディングページかどうか</summary>
	public new bool IsCartListLp
	{
		get
		{
			var path = StringUtility.ToEmpty(Request.Url.AbsolutePath);
			var beforePath = ((Request.UrlReferrer != null) ? Request.UrlReferrer.AbsolutePath : string.Empty);
			if (Constants.CART_LIST_LP_OPTION
				&& (path.ToUpper().Contains(Constants.CART_LIST_LP_PAGE_NAME)
					|| beforePath.ToUpper().Contains(Constants.CART_LIST_LP_PAGE_NAME)
					|| this.LandingCartInputAbsolutePath.ToUpper().Contains(Constants.CART_LIST_LP_PAGE_NAME)))
			{
				return true;
			}
			return false;
		}
	}
	/// /// <summary>ランディングカート保持用セッションキー（バックアップ）</summary>
	public string BackupLadingCartSessionKey
	{
		get { return "Backup_" + this.LadingCartSessionKey; }
	}
	/// <summary>バックアップがあるか？</summary>
	public bool IsBackupLandingCartSession
	{
		get { return (Session[this.BackupLadingCartSessionKey] != null); }
	}
	/// <summary>フォーカスを当てるコントロール名</summary>
	protected string[] ControlFocusOn
	{
		get
		{
			var controlIds = (this.Request[Constants.REQUEST_KEY_FOCUS_ON] ?? string.Empty)
				.Split(',')
				.Where(s => string.IsNullOrWhiteSpace(s) == false)
				.Take(3)
				.ToArray();
			return controlIds;
		}
	}
}