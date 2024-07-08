/*
=========================================================================================================
  Module      : 基底ユーザコントロール(BaseUserControl.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.DataCacheController;
using w2.App.Common.Option;
using w2.App.Common.Product;
using w2.App.Common.Web.Page;
using w2.App.Common.Web.Process;
using w2.Domain.MemberRank;
using w2.Domain.User;

///*********************************************************************************************
/// <summary>
/// 基底ユーザコントロール
/// </summary>
///*********************************************************************************************
public class BaseUserControl : CommonUserControl
{
	/// <summary>
	/// ページ初期化
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Init(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// ブランド情報取得
		//------------------------------------------------------
		GetBrandInfo();

		// カスタムパーツ取得
		var fileName = StringUtility.ToEmpty(Path.GetFileName(this.AppRelativeVirtualPath)).Replace(Preview.PREVIEW_PARTS_EXTENSION, "");
		var model = DataCacheControllerFacade.GetPartsDesignCacheController().CacheData
			.FirstOrDefault(
				m => (m.PartsType == Constants.FLG_PARTSDESIGN_PARTS_TYPE_CUSTOM)
					&& String.Equals(m.FileName, fileName, StringComparison.CurrentCultureIgnoreCase));

		if (model == null) return;

		// アクセスユーザ情報取得
		var accessUser = new ReleaseRangeAccessUser
		{
			Now = this.ReferenceDateTime,
			MemberRankInfo = this.LoginMemberRankInfo,
			IsLoggedIn = this.IsLoggedIn,
			HitTargetListId = this.LoginUserHitTargetListIds
		};

		// 公開範囲条件の確認
		var result = new ReleaseRangePartsDesign(model).Check(accessUser);

		if ((result.Publish == ReleaseRangeResult.RangeResult.Out)
			|| (result.PublishDate == ReleaseRangeResult.RangeResult.Out)
			|| (result.MemberRank == ReleaseRangeResult.RangeResult.Out)
			|| (result.TargetList == ReleaseRangeResult.RangeResult.Out))
		{
			this.Visible = false;
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
		return BasePage.GetKeyValue(objSrc, strKey);
	}

	/// <summary>
	/// データ取り出しメソッド
	/// </summary>
	/// <param name="src">ソースオブジェクト</param>
	/// <param name="key">キー</param>
	/// <returns>データ</returns>
	public static object GetKeyValueToNull(object src, string key)
	{
		return BasePage.GetKeyValueToNull(src, key);
	}

	/// <summary>
	/// ブランド情報の取得
	/// </summary>
	public void GetBrandInfo()
	{
		if (Constants.PRODUCT_BRAND_ENABLED)
		{
			var bid = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_BRAND_ID]);

			// 最後に表示していたBrand_Idを更新
			if (string.IsNullOrEmpty(bid) == false)
			{
				this.LastDisplayedBrandId = bid;
			}

			if (Constants.BRAND_SESSION_ENABLED == false)
			{
				this.BrandId = bid;
			}
			else
			{
				// クエリストリング > sessionのbid > デフォルトブランド > 有効なブランドの順でセット
				// クエリストリングのブランドがあればそれをセット
				if (string.IsNullOrEmpty(bid) == false)
				{
					this.BrandId = bid;
				}

				if (string.IsNullOrEmpty(this.BrandId))
				{
					var defBrandDv = ProductBrandUtility.GetDefaultBrand();
					if ((defBrandDv != null) && (defBrandDv.Count > 0))
					{
						// デフォルトブランドセット
						this.BrandId = StringUtility.ToEmpty(defBrandDv[0][Constants.FIELD_PRODUCTBRAND_BRAND_ID]);
					}
					else
					{
						// デフォルトさえなければ先頭のブランド
						var brands = ProductBrandUtility.GetProductBrandList();
						if ((brands != null) && (brands.Count > 0))
						{
							this.BrandId = StringUtility.ToEmpty(brands[0][Constants.FIELD_PRODUCTBRAND_BRAND_ID]);
						}
					}
				}
			}

			if (this.BrandId != "")
			{
				List<DataRowView> lBrand = ProductBrandUtility.GetBrandDataFromCache(this.BrandId);
				if (lBrand.Count != 0)
				{
					this.BrandName = (string)lBrand[0][Constants.FIELD_PRODUCTBRAND_BRAND_NAME];
					this.BrandTitle = (string)lBrand[0][Constants.FIELD_PRODUCTBRAND_BRAND_TITLE];
					this.BrandSeoKeyword = (string)lBrand[0][Constants.FIELD_PRODUCTBRAND_SEO_KEYWORD];
					this.BrandAdditionalDsignTag = (string)lBrand[0][Constants.FIELD_PRODUCTBRAND_ADDITIONAL_DESIGN_TAG];
				}
			}
		}
		else
		{
			this.BrandId = "";
		}
	}

	/// <summary>
	/// 遷移先URLの正当性チェック（外部サイトにジャンプしようとしていた場合TOPページに書き換え）
	/// </summary>
	/// <param name="nextUrl">遷移先URL</param>
	/// <returns></returns>
	protected string NextUrlValidation(string nextUrl)
	{
		var urlHttps = Uri.UriSchemeHttps + Uri.SchemeDelimiter + Constants.SITE_DOMAIN;
		var CompletedNextUrl = nextUrl;
		if (nextUrl.StartsWith("/")) CompletedNextUrl = string.Format("{0}{1}", urlHttps, nextUrl);
		if (nextUrl.StartsWith("//")) CompletedNextUrl = string.Format("{0}{1}{2}", urlHttps, Constants.PATH_ROOT, nextUrl);

		// 他サイトへ飛ぼうとしていたらURLをルートへ書き換える（踏み台対策）
		if ((CompletedNextUrl.StartsWith(Uri.UriSchemeHttp + Uri.SchemeDelimiter + Constants.SITE_DOMAIN + "/") == false)
			&& (CompletedNextUrl.StartsWith(urlHttps + "/") == false))
		{
			return Constants.PATH_ROOT;
		}
		return CompletedNextUrl;
	}

	/// <summary>
	/// カートオブジェクト取得
	/// </summary>
	/// <returns></returns>
	protected w2.App.Common.Order.CartObjectList GetCartObjectList()
	{
		return SessionSecurityManager.GetCartObjectList(Context, this.IsPc ? Constants.FLG_ORDER_ORDER_KBN_PC : Constants.FLG_ORDER_ORDER_KBN_SMARTPHONE);
	}

	/// <summary>
	/// 置換タグの置換後の値(MaxLength)を取得
	/// </summary>
	/// <param name="strReplaceTag">置換タグ</param>
	/// <returns>置換後の値</returns>
	public int GetMaxLength(string strReplaceTag)
	{
		return this.Process.GetMaxLength(strReplaceTag);
	}

	/// <summary>
	/// ログインユーザーのCPMクラスタを含んでいるか（部分一致）
	/// </summary>
	/// <param name="cpmClusterNames">CPMクラスタ名リスト</param>
	/// <returns>含んでいるか</returns>
	public bool ContainsCpmClusterName(params string[] cpmClusterNames)
	{
		return cpmClusterNames.Contains(this.LoginUserCpmClusterName);
	}

	#region "各ペースページモジュール共通"
	/// <summary>
	/// 数値表示
	/// </summary>
	/// <param name="objNum">数量</param>
	/// <returns>数量</returns>
	public string GetNumeric(object objNum)
	{
		return this.Process.GetNumeric(objNum);
	}

	/// <summary>
	/// 外部コントロール取得（再帰メソッド）
	/// </summary>
	/// <param name="control">検索の基準になるコントロール</param>
	/// <param name="type">検索するコントロールの型</param>
	/// <returns>外部コントロール</returns>
	protected dynamic GetOuterControl(Control control, Type type)
	{
		return this.Process.GetOuterControl(control, type);
	}

	/// <summary>
	/// 親リピーターアイテムを取得
	/// </summary>
	/// <param name="control">検索の基準になるコントロール</param>
	/// <param name="repeaterControlId">検索するリピーターのID</param>
	/// <returns>検索するリピーターアイテム</returns>
	public RepeaterItem GetParentRepeaterItem(Control control, string repeaterControlId)
	{
		return this.Process.GetParentRepeaterItem(control, repeaterControlId);
	}

	/// <summary>
	/// カスタムバリデータ一覧作成
	/// </summary>
	/// <param name="cTarget">カスタムバリデータを探す対象コントロール</param>
	/// <param name="lCustomValidators">カスタムバリデータ一覧（再帰してここに作成されていく）</param>
	public void CreateCustomValidators(Control cTarget, List<CustomValidator> lCustomValidators)
	{
		this.Process.CreateCustomValidators(cTarget, lCustomValidators);
	}

	/// <summary>
	/// エラー向けコントロール表示変更処理
	/// </summary>
	/// <param name="validatorCheckKbn">バリデーターチェック区分</param>
	/// <param name="errorMessages">エラーメッセージ</param>
	/// <param name="customValidators">カスタムバリデータリスト</param>
	public void SetControlViewsForError(
		string validatorCheckKbn,
		Dictionary<string, string> errorMessages,
		List<CustomValidator> customValidators)
	{
		this.Process.SetControlViewsForError(validatorCheckKbn, errorMessages, customValidators);
	}

	/// <summary>
	/// ListItemCollectionの中に値があればそれを返す（無ければnullを返す）　※DataBindなどで利用
	/// </summary>
	/// <param name="collection">ListItemCollecyion</param>
	/// <param name="value">値</param>
	/// <returns>ListItemCollectionの該当の値</returns>
	protected string GetListItemValue(ListItemCollection collection, string value)
	{
		return this.Process.GetListItemValue(collection, value);
	}

	/// <summary>
	/// 国が日本かどうか
	/// </summary>
	/// <param name="countryIsoCode">国ISOコード</param>
	/// <returns>日本か</returns>
	protected bool IsCountryJp(string countryIsoCode)
	{
		return this.Process.IsCountryJp(countryIsoCode);
	}

	/// <summary>
	/// 国がアメリカかどうか
	/// </summary>
	/// <param name="countryIsoCode">国ISOコード</param>
	/// <returns>アメリカか</returns>
	protected bool IsCountryUs(string countryIsoCode)
	{
		return this.Process.IsCountryUs(countryIsoCode);
	}

	/// <summary>
	/// 国が台湾かどうか
	/// </summary>
	/// <param name="countryIsoCode">国ISOコード</param>
	/// <returns>台湾か</returns>
	protected bool IsCountryTw(string countryIsoCode)
	{
		return this.Process.IsCountryTw(countryIsoCode);
	}

	/// <summary>
	/// 郵便番号が必須かどうか
	/// </summary>
	/// <param name="countryIsoCode">国ISOコード</param>
	/// <returns>必須か</returns>
	protected bool IsAddrZipcodeNecessary(string countryIsoCode)
	{
		return this.Process.IsAddrZipcodeNecessary(countryIsoCode);
	}

	/// <summary>
	/// リンク式決済か
	/// </summary>
	/// <returns>リンク式決済か</returns>
	protected bool IsCreditCardLinkPayment()
	{
		return this.Process.IsCreditCardLinkPayment();
	}

	/// <summary>プロセス</summary>
	protected new BasePageProcess Process
	{
		get { return (BasePageProcess)this.ProcessTemp; }
	}
	/// <summary>プロセステンポラリ</summary>
	protected override IPageProcess ProcessTemp
	{
		get
		{
			if (m_processTmp == null) m_processTmp = new BasePageProcess(this, this.ViewState, this.Context);
			return m_processTmp;
		}
	}
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
	public UserModel LoginUser { get { return this.Process.LoginUser; } }
	/// <summary>ログインユーザーID</summary>
	public string LoginUserId { get { return this.Process.LoginUserId; } }
	/// <summary>ログインユーザー名</summary>
	public string LoginUserName { get { return this.Process.LoginUserName; } }
	/// <summary>ログインユーザーニックネーム</summary>
	public string LoginUserNickName { get { return this.Process.LoginUserNickName; } }
	/// <summary>ログインユーザーメールアドレス</summary>
	public string LoginUserMail { get { return this.Process.LoginUserMail; } }
	/// <summary>ログインユーザーメールアドレス2</summary>
	public string LoginUserMail2 { get { return this.Process.LoginUserMail2; } }
	/// <summary>ログインユーザー生年月日</summary>
	public string LoginUserBirth { get { return this.Process.LoginUserBirth; } }
	/// <summary>会員ランクID</summary>
	public string MemberRankId { get { return StringUtility.ToEmpty(this.Process.MemberRankId); } }
	/// <summary>ログインユーザー会員ランクID</summary>
	public string LoginUserMemberRankId { get { return this.Process.LoginUserMemberRankId; } }
	/// <summary>会員ランク情報</summary>
	public MemberRankModel LoginMemberRankInfo { get { return this.Process.LoginMemberRankInfo; } }
	/// <summary>会員ランク名</summary>
	public string MemberRankName { get { return this.Process.MemberRankName; } }
	/// <summary>User Fixed Purchase Member Flg</summary>
	public string UserFixedPurchaseMemberFlg { get { return StringUtility.ToEmpty(this.Process.UserFixedPurchaseMemberFlg); } }
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
	public string[] LoginUserHitTargetListIds { get { return this.Process.LoginUserHitTargetListIds; } }
	/// <summary>ターゲットページ</summary>
	public string SessionParamTargetPage { get { return this.Process.SessionParamTargetPage; } }
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
	#endregion

	/// <summary>ブランドID</summary>
	public string BrandId
	{
		get { return StringUtility.ToEmpty(this.Session[Constants.SESSION_KEY_BRAND_ID]); }
		set { this.Session[Constants.SESSION_KEY_BRAND_ID] = value; }
	}
	/// <summary>ブランド名</summary>
	public string BrandName { get; set; }
	/// <summary>最後に表示していたブランドID</summary>
	public string LastDisplayedBrandId
	{
		get { return StringUtility.ToEmpty(this.Session[Constants.SESSION_KEY_LAST_DISPLAYED_BRAND_ID]); }
		set { this.Session[Constants.SESSION_KEY_LAST_DISPLAYED_BRAND_ID] = value; }
	}
	/// <summary>ブランドタイトル</summary>
	public string BrandTitle { get; set; }
	/// <summary>ブランドSEOキーワード</summary>
	public string BrandSeoKeyword { get; set; }
	/// <summary>ブランド追加タグ情報</summary>
	public string BrandAdditionalDsignTag { get; set; }

	/// <summary>非同期表示？</summary>
	public bool IsAsync
	{
		get { return m_blIsAsync; }
		set { m_blIsAsync = value; }
	}
	bool m_blIsAsync = true;

	/// <summary>TOP＆商品ページ判定</summary>
	protected bool IsTopAndProductPage
	{
		get
		{
			return (this.IsTopPage || this.IsProductPage);
		}
	}
	/// <summary>TOPページ判定</summary>
	protected bool IsTopPage
	{
		get
		{
			return ((this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_DEFAULT))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_DEFAULT_BRAND_TOP)));
		}
	}
	/// <summary>商品ページ判定</summary>
	protected bool IsProductPage
	{
		get
		{
			return ((this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_PRODUCT_LIST))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_PRODUCT_LIST_PREVIEW))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_PREVIEW_PRODUCT_DETAIL))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_PREVIEW_PRODUCT_DETAIL_PREVIEW))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_PRODUCTSTOCK_LIST))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_PRODUCTSTOCK_LIST_PREVIEW))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_PRODUCTSET_LIST))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_PRODUCTSET_LIST_PREVIEW))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_PRODUCTVARIATION_LIST))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_PRODUCTVARIATION_LIST_PREVIEW))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_REALSHOPPRODUCTSTOCK_LIST))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_REALSHOPPRODUCTSTOCK_LIST_PREVIEW)));
		}
	}
	/// <summary>カート一覧＆注文ページ判定</summary>
	protected bool IsCartListAndOrderPage
	{
		get
		{
			return ((Request.Url.AbsolutePath.EndsWith(Constants.PAGE_FRONT_CART_LIST, true, null)) || this.IsOrderPage);
		}
	}
	/// <summary>注文ページ判定（カート一覧は含まない）</summary>
	protected bool IsOrderPage
	{
		get
		{
			return ((this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_ORDER_SHIPPING))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_ORDER_SHIPPING_PREVIEW))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_ORDER_SHIPPING_SELECT_SHIPPING))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_ORDER_SHIPPING_SELECT_SHIPPING_PREVIEW))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_ORDER_SHIPPING_SELECT_PRODUCT))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_ORDER_SHIPPING_SELECT_PRODUCT_PREVIEW))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_ORDER_PAYMENT))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_ORDER_PAYMENT_PREVIEW))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_ORDER_CONFIRM))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_ORDER_CONFIRM_PREVIEW))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_ORDER_SETTLEMENT))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_ORDER_SETTLEMENT_PREVIEW))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_ORDER_COMPLETE))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_ORDER_COMPLETE_PREVIEW))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_AMAZON_PAYMENT_INPUT))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_FRONT_ORDER_COMPLETE))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_ORDER_SHIPPING_SELECT))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_ORDER_SHIPPING_SELECT_PREVIEW)));
		}
	}
	/// <summary>ランディングカートページ判定</summary>
	protected bool IsLandingCartPage
	{
		get
		{
			if (string.IsNullOrEmpty(this.LandingCartInputAbsolutePath)) return false;

			return ((this.Parent.Page.GetType().Name.Contains(this.LandingCartInputAbsolutePath.Remove(0, Constants.PATH_ROOT.Length)))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_LANDING_LANDING_CART_CONFIRM))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_LP_PREVIEW))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_PAYMENT_ATONE_ATONE_EXEC_ORDER_FOR_LANDINGCART))
				|| (this.Parent.Page.GetType().Name.Contains(Constants.PAGE_TYPE_FRONT_PAYMENT_AFTEE_AFTEE_EXEC_ORDER_FOR_LANDINGCART)));
		}
	}
	/// <summary>新LPカートリストページから遷移されるLP確認ページかどうか</summary>
	protected bool IsLandingConfirmFromCartListLp
	{
		get
		{
			var returnUrl = Request[Constants.REQUEST_KEY_RETURN_URL] ?? string.Empty;
			return (Constants.CART_LIST_LP_OPTION
				&& returnUrl.ToUpper().Contains(Constants.CART_LIST_LP_PAGE_NAME));
		}
	}
	/// <summary>新LPカートリストページかどうか</summary>
	protected bool IsCartListLp
	{
		get
		{
			if (string.IsNullOrEmpty(this.LandingCartInputAbsolutePath)) return false;
			var result = (Constants.CART_LIST_LP_OPTION
				&& this.LandingCartInputAbsolutePath.ToUpper().Contains(Constants.CART_LIST_LP_PAGE_NAME));
			return result;
		}
	}
	/// <summary>ランディングカート入力画面絶対パス</summary>
	public string LandingCartInputAbsolutePath
	{
		get { return (string)Session["landing_cart_input_absolutePath"]; }
		set { Session["landing_cart_input_absolutePath"] = value; }
	}
	/// <summary>ランディング入力ページ最後に書き込み時刻</summary>
	public string LandingCartInputLastWriteTime
	{
		get { return (string)this.Session[Constants.SESSION_KEY_LANDING_CART_INPUT_LAST_WRITE_TIME]; }
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
	/// <summary>商品税込みフラグ</summary>
	public bool ProductIncludedTaxFlg
	{
		get { return Constants.MANAGEMENT_INCLUDED_TAX_FLAG; }
	}
	/// <summary>商品価格区分表示文言</summary>
	public string ProductPriceTextPrefix
	{
		get { return TaxCalculationUtility.GetTaxTypeText(); }
	}
}
