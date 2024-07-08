/*
=========================================================================================================
  Module      : 基底ページ(BasePage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using w2.App.Common.Global;
using w2.App.Common.Global.Region;
using w2.App.Common.Line.Util;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.Aftee;
using w2.App.Common.Order.Payment.Atone;
using w2.App.Common.Preview;
using w2.App.Common.Product;
using w2.App.Common.User.SocialLogin.Helper;
using w2.App.Common.Util;
using w2.App.Common.Web.Page;
using w2.App.Common.Web.Process;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Logger;
using w2.Common.Web;
using w2.Domain.LandingPage;
using w2.Domain.MemberRank;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.SubscriptionBox;

///*********************************************************************************************
/// <summary>
/// 基底ページ
/// </summary>
///*********************************************************************************************
public abstract class BasePage : CommonPage
{
	/// <summary>ページアクセスタイプ</summary>
	public enum PageAccessTypes
	{
		/// <summary>とくに何もしない</summary>
		NoAction,
		/// <summary>リダイレクトしてHttpアクセスさせる（「ログイン後http許可フラグ設定がONのとき）</summary>
		Http,
		/// <summary>リダイレクトしてHttpsアクセスさせる</summary>
		Https,
	}
	/// <summary>ページアクセスタイプ</summary>
	/// <remarks>
	/// それぞれのページでオーバーライドしてページアクセス時の振る舞いを設定します。
	/// 各ページクラスの先頭に設置してください。
	/// </remarks>
	public virtual PageAccessTypes PageAccessType { get { return PageAccessTypes.NoAction; } }

	/// <summary>ログイン必須判定</summary>
	public virtual bool NeedsLogin { get { return this.Process.NeedsLogin; } }

	/// <summary>リピートプラスONEリダイレクト必須判定</summary>
	public virtual bool RepeatPlusOneNeedsRedirect { get { return this.Process.RepeatPlusOneNeedsRedirect; } }

	/// <summary>マイページメニュー表示判定</summary>
	public virtual bool DispMyPageMenu { get { return false; } }

	/// <summary>
	/// ログイン実施タイプ
	/// </summary>
	public enum LoginType
	{
		/// <summary>通常ログイン</summary>
		Normal,
		/// <summary>楽天コネクト</summary>
		RakutenConnect,
		/// <summary>ペイパル</summary>
		PayPal,
		/// <summary>LINE</summary>
		Line,
	}

	/// <summary>
	/// ページ初期化開始時
	/// </summary>
	protected void Page_PreInit(object sender, EventArgs e)
	{
		// オーバーライドされた項目をプロセスにセット
		this.Process.NeedsLogin = this.NeedsLogin;
		this.Process.RepeatPlusOneNeedsRedirect = this.RepeatPlusOneNeedsRedirect;

		// Safariブラウザの場合、downlevel（IE4.0以前）と認識されるケースがあるため、uplevel（IE4.0以後）に更新
		// ※downlevelのままだと.Net FrameworkのJSコードが出力されないため
		if ((Request.UserAgent != null) && (Request.UserAgent.IndexOf("AppleWebKit", StringComparison.CurrentCultureIgnoreCase) > -1))
		{
			this.ClientTarget = "uplevel";
		}

		PreviewInit();
	}

	/// <summary>
	/// ページ初期化（各BasePageでnewされるので共通処理を書かないこと）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Init(object sender, EventArgs e)
	{
		this.Process.Page_Init(sender, e);
	}

	/// <summary>
	/// ページ描画前
	/// </summary>
	/// <remarks>
	/// 呼び出し順序：Page_PreInit -> Page_Init -> Page_Load -> 各種クリックとかのイベント -> Page_PreRender
	/// </remarks>
	protected void Page_PreRender(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// ゲスト状態のときに、セッションにカートオブジェクトリストが存在すればCookie保存、なければ復元
		//------------------------------------------------------
		// ※Page_Initに記述すると、カート投入でページ遷移しない設定時（商品一覧や詳細）にCookie保存されないため、Page_PreRenderへ移動
		if ((this.IsLoggedIn == false) && (this.IsPreview == false))
		{
			CartCookieManager.RefreshCookieCartId(GetCartObjectList());
		}
	}

	/// <summary>
	/// ページ表示終了時
	/// </summary>
	protected void Page_LoadComplete(object sender, EventArgs e)
	{
		PreviewEnd();
	}

	/// <summary>
	/// 認証キーチェック（NGであれば新しいセッションを発行し、同じページにリダイレクト）
	/// </summary>
	protected void CheckAuthKey()
	{
		this.Process.CheckAuthKey();
	}

	/// <summary>
	/// HTTPS通信チェック
	/// </summary>
	public virtual void CheckHttps()
	{
		// HTTPS通信チェック（ＮＧの場合トップページへ遷移）
		this.Process.CheckHttps(Constants.PATH_ROOT);
	}
	/// <summary>
	/// HTTPS通信チェック
	/// </summary>
	/// <param name="strRedirectUrl">ＮＧ時リダイレクトURL</param>
	public void CheckHttps(string strRedirectUrl)
	{
		this.Process.CheckHttps(strRedirectUrl);
	}

	/// <summary>
	/// ログインしていなければログインページにとばす
	/// </summary>
	/// <remarks>遷移後は元のページに戻る</remarks>
	public void CheckLoggedIn()
	{
		this.Process.CheckLoggedIn();
	}
	/// <summary>
	/// ログインしていなければ指定ページにとばす
	/// </summary>
	/// <param name="strPagePath">ログイン後の遷移先</param>
	public void CheckLoggedIn(string strPagePath)
	{
		this.Process.CheckLoggedIn(strPagePath);
	}

	/// <summary>
	/// パラメタに格納されたNextUrl取得
	/// </summary>
	/// <returns>NextUrlの値（パラメタ無しの場合はnull）</returns>
	public string GetNextUrlForCheck()
	{
		return this.Process.GetNextUrlForCheck();
	}

	/// <summary>
	/// 確認画面用正常遷移チェック処理
	/// </summary>
	/// <remarks>
	/// 画面遷移の正当性をチェックしたいときに使用する。
	/// 
	/// １．遷移元で画面遷移する際、セッションパラメタに
	///		 > htParam.Add(Constants.HASH_KEY_NEXT_PAGE_FOR_CHECK, Constants.PAGE_FRONT_USER_MODIFY_CONFIRM);
	///		 > Session[Constants.SESSION_KEY_PARAM] = htParam;
	///		 のように次ページのURLを格納する。
	///		 
	/// ２．遷移先のページでこのメソッドをコールすることにより、遷移元のから遷移であるかを確認する。
	///		> SessionManager.CheckUrlSession(Session, Response, this.RawUrl);
	/// 
	/// ※主に、入力データを確認画面で正しく受け取るための制御に使用する。
	/// </remarks>
	public void CheckUrlSessionForUserRegistModify()
	{
		string strNextUrl = GetNextUrlForCheck();

		if ((strNextUrl == null) || (this.RawUrl.IndexOf(strNextUrl) == -1))
		{
			// エラーページへ（トップページからやりなおす）
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_INCORRECT_NEXTURL_SESSION);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR + "?" + Constants.REQUEST_KEY_ERRORPAGE_KBN + "=" + Constants.KBN_REQUEST_ERRORPAGE_GOTOP);
		}
	}

	/// <summary>
	/// モバイルチェック
	/// </summary>
	/// <remarks>
	/// モバイル遷移先設定がしてあるとき、モバイルアクセスしたらリダイレクト
	/// </remarks>
	protected void MobileCheck()
	{
		// モバイルオプションがOFFの場合は何もしない
		if (Constants.MOBILEOPTION_ENABLED == false) return;

		// モバイル遷移先設定あり？
		if (IsMobile)
		{
			//------------------------------------------------------
			// 一部のSoftbank機種がAUと判断されてしまうためリダイレクト前にContentsTypeを固定で指定
			//------------------------------------------------------
			Response.ContentType = "text/html";

			//------------------------------------------------------
			// リダイレクト先URL作成（絶対パスとする）
			//------------------------------------------------------
			string strRedirectUrl = null;
			if (Constants.PATH_MOBILESITE.StartsWith("/"))
			{
				strRedirectUrl = Uri.UriSchemeHttp + Uri.SchemeDelimiter + Request.Url.Authority + Constants.PATH_MOBILESITE;
			}
			else
			{
				strRedirectUrl = Constants.PATH_MOBILESITE;
			}

			//------------------------------------------------------
			// リダイレクト
			//------------------------------------------------------
			if (Regex.IsMatch(StringUtility.ToEmpty(Request.UserAgent), @"^DoCoMo/"))
			{
				if (Constants.PATH_MOBILESITE.IndexOf('?') == -1)
				{
					Response.Redirect(strRedirectUrl + "?uid=NULLGWDOCOMO");
				}
				else
				{
					Response.Redirect(strRedirectUrl + "&uid=NULLGWDOCOMO");
				}
			}
			else
			{
				Response.Redirect(strRedirectUrl);
			}
			Response.End();
		}
	}

	/// <summary>
	/// 価格表示
	/// </summary>
	/// <param name="objNum">数量</param>
	/// <param name="strPriceUnitString">通貨単位文字列</param>
	/// <returns>数量</returns>
	public static string GetPrice(object objNum, string strPriceUnitString)
	{
		decimal dPrice;
		if (decimal.TryParse(StringUtility.ToEmpty(objNum), out dPrice) == false)
		{
			return strPriceUnitString;
		}

		if (dPrice < 0)
		{
			return "-" + strPriceUnitString + StringUtility.ToEmpty(dPrice * -1);
		}

		return strPriceUnitString + StringUtility.ToEmpty(dPrice);
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
	/// 検証によるコントロール表示制御
	/// </summary>
	/// <param name="dicErrorMessages">エラーメッセージ</param>
	/// <param name="strTargetKey">対象キー</param>
	/// <param name="wcvCustomValidator">ラップ済カスタムバリデータ</param>
	/// <param name="wcControls">対象コントロール</param>
	/// <returns>検証成功/失敗</returns>
	public bool ChangeControlLooksForValidator(
		Dictionary<string, string> dicErrorMessages,
		string strTargetKey,
		WrappedCustomValidator wcvCustomValidator,
		params WrappedControl[] wcControls)
	{
		return this.Process.ChangeControlLooksForValidator(dicErrorMessages, strTargetKey, wcvCustomValidator, wcControls);
		}
	/// <summary>
	/// 検証によるコントロール表示制御
	/// </summary>
	/// <param name="dicErrorMessages">エラーメッセージ</param>
	/// <param name="strTargetKey">対象キー</param>
	/// <param name="cvCustomValidator">カスタムバリデータ</param>
	/// <param name="wcWebControls">対象WEBコントロール</param>
	/// <returns>検証成功/失敗</returns>
	public bool ChangeControlLooksForValidator(
		Dictionary<string, string> dicErrorMessages,
		string strTargetKey,
		CustomValidator cvCustomValidator,
		params WebControl[] wcWebControls)
	{
		return this.Process.ChangeControlLooksForValidator(
			dicErrorMessages,
			strTargetKey,
			cvCustomValidator,
			wcWebControls);
	}

	/// <summary>
	/// Set disable and hide custom validator control information list
	/// </summary>
	/// <param name="customValidators">Wrapped custom validators</param>
	protected void SetDisableAndHideCustomValidatorControlInformationList(
		IEnumerable<WrappedCustomValidator> customValidators)
	{
		this.Process.SetDisableAndHideCustomValidatorControlInformationList(customValidators);
	}

	/// <summary>
	/// Set custom validator control information list
	/// </summary>
	/// <param name="target">カスタムバリデータ取得元コントロール</param>
	protected void SetCustomValidatorControlInformationList(Control target)
	{
		this.Process.SetCustomValidatorControlInformationList(target);
	}

	/// <summary>
	/// エラー向けコントロール表示リセット処理
	/// </summary>
	/// <param name="cTarget">カスタムバリデータ取得元コントロール</param>
	/// <param name="wcControls">対象コントロール</param>
	protected void ResetControlViewsForError(Control cTarget, params WrappedControl[] wcControls)
	{
		// カスタムバリデータ取得
		var customValidators = new List<CustomValidator>();
		CreateCustomValidators(cTarget, customValidators);

		// 対象コントロールに対して繰り返す
		foreach (var wrappedControl in wcControls.Where(wc => wc.InnerControl != null))
		{
			var innerControl = (WebControl)wrappedControl.InnerControl;
			innerControl.CssClass = innerControl.CssClass.Replace(Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING, " ");
			foreach (var customValidator in customValidators)
			{
				customValidator.ErrorMessage = "";
				customValidator.IsValid = true;
			}
		}
	}

	/// <summary>
	/// 郵便番号検索（ラップ済テキストボックス）
	/// </summary>
	/// <param name="wtbZip1">郵便番号テキストボックス1</param>
	/// <param name="wtbZip2">郵便番号テキストボックス2</param>
	/// <param name="wddlAddr1">住所1ドロップダウン</param>
	/// <param name="wtbAddr2">住所2テキストボックス</param>
	/// <param name="wtbZip">Textbox zip</param>
	/// <returns>エラーメッセージ</returns>
	protected string SearchAddrFromZip(
		WrappedTextBox wtbZip1,
		WrappedTextBox wtbZip2,
		WrappedDropDownList wddlAddr1,
		WrappedTextBox wtbAddr2,
		WrappedTextBox wtbZip)
	{
		return this.Process.SearchAddrFromZip(
			wtbZip1,
			wtbZip2,
			wddlAddr1,
			wtbAddr2,
			wtbZip);
	}

	/// <summary>
	/// ログイン成功処理実行＆次の画面へ遷移（会員登録向け）
	/// </summary>
	/// <param name="user">ユーザー</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	protected void ExecLoginSuccessProcessAndGoNextForUserRegister(
		UserModel user,
		UpdateHistoryAction updateHistoryAction)
	{
		this.Process.ExecLoginSuccessProcessAndGoNextForUserRegister(user, updateHistoryAction);
	}

	/// <summary>
	/// ログイン成功処理実行＆次の画面へ遷移（ログイン向け）
	/// </summary>
	/// <param name="user">ユーザー</param>
	/// <param name="defaultNextUrl">デフォルト遷移先URL</param>
	/// <param name="saveAutoCompleteLoginId">オートコンプリートのログインIDを保存するか</param>
	/// <param name="loginType">ログインタイプ</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	protected void ExecLoginSuccessProcessAndGoNextForLogin(
		UserModel user,
		string defaultNextUrl,
		bool saveAutoCompleteLoginId,
		LoginType loginType,
		UpdateHistoryAction updateHistoryAction)
	{
		this.Process.ExecLoginSuccessProcessAndGoNextForLogin(
			user,
			defaultNextUrl,
			saveAutoCompleteLoginId,
			loginType,
			updateHistoryAction);
			}

	/// <summary>
	/// ログイン成功処理実行＆次の画面へ遷移（ログイン向け）
	/// </summary>
	/// <param name="user">ユーザー</param>
	/// <param name="socialLogin">ソーシャルログイン情報</param>
	/// <param name="defaultNextUrl">デフォルト遷移先URL</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	protected void ExecLoginSuccessProcessAndGoNextForSocialPlusLogin(
		UserModel user,
		SocialLoginModel socialLogin,
		string defaultNextUrl,
		UpdateHistoryAction updateHistoryAction)
	{
		this.Process.ExecLoginSuccessProcessAndGoNextForSocialPlusLogin(
			user,
			socialLogin,
			defaultNextUrl,
			updateHistoryAction);
	}

	/// <summary>
	/// ログイン成功アクション実行
	/// </summary>
	/// <param name="user">ユーザー</param>
	/// <param name="nextUrl">遷移先URL</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	protected void ExecLoginSuccessActionAndGoNextInner(UserModel user, string nextUrl, UpdateHistoryAction updateHistoryAction)
	{
		this.Process.ExecLoginSuccessActionAndGoNextInner(user, nextUrl, updateHistoryAction);
		}

	/// <summary>
	/// ログインユーザデータを格納
	/// </summary>
	/// <param name="user">ユーザ情報</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	protected void SetLoginUserData(UserModel user, UpdateHistoryAction updateHistoryAction)
	{
		this.Process.SetLoginUserData(user, updateHistoryAction);
			}

	/// <summary>
	/// プレビュー初期処理
	/// </summary>
	/// <remarks>
	/// セッションのバックアップ
	/// セッションにプレビュー用ダミーデータを配置
	/// </remarks>
	private void PreviewInit()
	{
		if (this.IsPreview == false) return;

		// プレビュー時はキャッシュを残さない
		Preview.SetNonCache(this.Response);

		// セッションのバックアップ
		this.SessionBackup = new List<KeyValuePair<string, object>>();
		foreach (var sessionName in this.SessionNameList
			.Where(s => this.SessionBackupExcludedNameList.Any(ns => ns == s) == false))
		{
			var value = new KeyValuePair<string, object>(sessionName, this.Session[sessionName]);
			this.SessionBackup.Add(value);
			this.Session.Remove(sessionName);
		}

		// セッションにダミーデータを配置
		this.LoginUser = Preview.GetDummyUserModel();
		this.LoginUserId = this.LoginUser.UserId;
		this.LoginUserName = "Preview";

		if (Constants.MEMBER_RANK_OPTION_ENABLED) this.LoginMemberRankInfo = this.ReferenceMemgbeRankModel;

		this.LoginUserHitTargetListIds = this.ReferenceTargetList;
		}

	/// <summary>
	/// プレビュー終了時処理
	/// </summary>
	/// <remarks>
	/// セッションの復元
	/// </remarks>
	private void PreviewEnd()
	{
		if (this.IsPreview == false) return;

		// バックアップからセッションを復元
		foreach (var sessionName in this.SessionNameList
			.Where(s => this.SessionBackupExcludedNameList.Any(ns => ns == s) == false))
		{
			this.Session.Remove(sessionName);
		}
		this.SessionBackup.ForEach(s => { this.Session.Add(s.Key, s.Value); });
	}

	/// <summary>
	/// ログイン拒否エラー文言取得
	/// </summary>
	/// <param name="loginId">ログインID</param>
	/// <param name="password">パスワード</param>
	/// <returns>エラー文言</returns>
	protected string GetLoginDeniedErrorMessage(string loginId, string password)
	{
		return this.Process.GetLoginDeniedErrorMessage(loginId, password);
		}

	/// <summary>
	/// 不正なブランドチェック
	/// <param name="productId">商品ID（デフォルトNULL）</param>
	/// </summary>
	public void BrandCheck(string productId = null)
	{
		//------------------------------------------------------
		// ブランド使用確認
		//------------------------------------------------------
		if (Constants.PRODUCT_BRAND_ENABLED == false)
		{
			return;
		}

		//------------------------------------------------------
		// 指定のブランドIDがマスタにあるか確認
		//------------------------------------------------------
		var brands = ProductBrandUtility.GetBrandDataFromCache(this.BrandId);

		//------------------------------------------------------
		// 指定のブランドIDが商品情報のブランドID1～5にあるか確認
		//------------------------------------------------------
		var isUseProductBrandId = true;
		if ((brands.Count > 0) && (productId != null))
		{
			var dvProduct = this.IsPreviewProductMode
				? ProductPreview.GetProductDetailPreview(Constants.CONST_DEFAULT_SHOP_ID, productId)
				: ProductCommon.GetProductInfo(Constants.CONST_DEFAULT_SHOP_ID, productId, this.MemberRankId, this.UserFixedPurchaseMemberFlg);
			if (dvProduct.Count != 0)
			{
				if ((this.BrandId != (string)dvProduct[0][Constants.FIELD_PRODUCT_BRAND_ID1])
					&& (this.BrandId != (string)dvProduct[0][Constants.FIELD_PRODUCT_BRAND_ID2])
					&& (this.BrandId != (string)dvProduct[0][Constants.FIELD_PRODUCT_BRAND_ID3])
					&& (this.BrandId != (string)dvProduct[0][Constants.FIELD_PRODUCT_BRAND_ID4])
					&& (this.BrandId != (string)dvProduct[0][Constants.FIELD_PRODUCT_BRAND_ID5]))
				{
					isUseProductBrandId = false;
				}
			}
		}

		// （不正なブランドIDの場合）又は（存在するブランドID かつ 商品情報に対象ブランドが未設定の場合）
		if ((brands.Count == 0) || (isUseProductBrandId == false))
		{
			//------------------------------------------------------
			// 商品IDがある場合
			//------------------------------------------------------
			if (productId != null)
			{
				var dvProduct = this.IsPreviewProductMode
					? ProductPreview.GetProductDetailPreview(Constants.CONST_DEFAULT_SHOP_ID, productId)
					: ProductCommon.GetProductInfoUnuseMemberRankPrice(Constants.CONST_DEFAULT_SHOP_ID, productId);

				if (dvProduct.Count != 0)
				{
					//------------------------------------------------------
					// 商品のブランドIDを設定してリダイレクト
					//------------------------------------------------------
					string strBrandId = (string)dvProduct[0][Constants.FIELD_PRODUCT_BRAND_ID1];
					string strBrandName = ProductBrandUtility.GetProductBrandName(strBrandId);
					string strProductName = (string)dvProduct[0][Constants.FIELD_PRODUCT_NAME];

					List<DataRowView> lProductBrand = ProductBrandUtility.GetBrandDataFromCache(strBrandId);
					if (lProductBrand.Count == 0)
					{
						// エラーページへ
						Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_UNDISP);
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR + "?" + Constants.REQUEST_KEY_ERRORPAGE_KBN + "=" + Constants.KBN_REQUEST_ERRORPAGE_GOTOP);
					}

					string strUrl = ProductCommon.CreateProductDetailUrl(
						Request[Constants.REQUEST_KEY_SHOP_ID],
						Request[Constants.REQUEST_KEY_CATEGORY_ID],
						strBrandId,
						Request[Constants.REQUEST_KEY_SEARCH_WORD],
						Request[Constants.REQUEST_KEY_PRODUCT_ID],
						Request[Constants.REQUEST_KEY_VARIATION_ID],
						strProductName,
						strBrandName);

					// ★TODO ブランド機能ONの場合、ページデザイン設定及びパーツデザイン設定機能のバグの為、
					// 暫定的に対応。V5.3実装時に正式に修正する予定
					PreviewThrowghRedirectForBrandCheck(strUrl);
				}
			}

			//------------------------------------------------------
			// デフォルトブランド処理
			//------------------------------------------------------
			DataView dvDefaultBrand = ProductBrandUtility.GetDefaultBrand();

			// デフォルトブランドあり かつ　デフォルトブランドトップに遷移するON あるいは遷移先がトップ
			if ((dvDefaultBrand.Count != 0)
				&& (Constants.REDIRECT_TO_DEFAULT_BRAND_TOP
					|| Request.Url.ToString().Contains(Constants.PAGE_FRONT_DEFAULT.ToLower())))
			{
				// デフォルトブランドIDを付与してリダイレクト
				string strBrandId = (string)dvDefaultBrand[0][Constants.FIELD_PRODUCTBRAND_BRAND_ID];
				string strBrandName = (string)dvDefaultBrand[0][Constants.FIELD_PRODUCTBRAND_BRAND_NAME];
				string strBrandTop = ProductCommon.CreateBrandTopPageUrl(strBrandId, strBrandName);

				// ★TODO ブランド機能ONの場合、ページデザイン設定及びパーツデザイン設定機能のバグの為、
				// 暫定的に対応。V5.3実装時に正式に修正する予定
				PreviewThrowghRedirectForBrandCheck(strBrandTop);
			}
			// デフォルトブランドなし　あるいは　デフォルトブランドトップに遷移しない
			else
			{
				// 商品検索が行われた場合、デフォルトの商品一覧ページを表示する
				if ((StringUtility.ToEmpty(this.BrandId) == "") && Request.Url.ToString().Contains(Constants.PAGE_FRONT_PRODUCT_LIST))
				{
					return;
				}

				// すでにルートでなければルートへ遷移
				// （大文字、小文字の違いは無視する）
				if (Request.Url.AbsolutePath.EndsWith(Constants.PAGE_FRONT_DEFAULT, StringComparison.OrdinalIgnoreCase) == false)
				{
					// ★TODO ブランド機能ONの場合、ページデザイン設定及びパーツデザイン設定機能のバグの為、
					// 暫定的に対応。V5.3実装時に正式に修正する予定
					PreviewThrowghRedirectForBrandCheck(Constants.PATH_ROOT);
				}
			}
		}
	}

	/// <summary>
	/// ブランド情報の設定
	/// </summary>
	public void SetBrandInfo()
	{
		this.Process.SetBrandInfo();
			}

	/// <summary>
	/// 最大文字数を設定
	/// </summary>
	/// <param name="wtbTarget">対象テキストボックス</param>
	/// <param name="strReplaceTag">置換タグ</param>
	public void SetMaxLength(WrappedTextBox wtbTarget, string strReplaceTag)
	{
		// MaxLengthが直接設定されていない場合のみMaxLengthを設定
		if (wtbTarget.MaxLength == 0)
		{
			wtbTarget.MaxLength = GetMaxLength(strReplaceTag);
		}
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
	/// aspx.Preview.aspxへのリダイレクトをスキップする処理
	/// プレビュー表示の場合は遷移しない
	/// </summary>
	/// <param name="redirectUrl">遷移先</param>
	protected void PreviewThrowghRedirectForBrandCheck(string redirectUrl)
	{
		// ★TODO ブランド機能ONの場合、ページデザイン設定及びパーツデザイン設定機能のバグの為、
		// 暫定的に対応。V5.3実装時に正式に修正する予定
		if ((Request.Url.ToString().Contains("aspx.Preview.aspx") == false)
			&& string.IsNullOrEmpty((string)Request[Constants.REQUEST_KEY_PREVIEW_HASH]))
		{
			Response.Redirect(redirectUrl);
		}
	}

	/// <summary>
	/// エラー画面遷移
	/// </summary>
	/// <param name="errorMessage">エラーメッセージ</param>
	/// <remarks>デフォルトで前の画面に戻る</remarks>
	protected void RedirectErrorPage(string errorMessage = "")
	{
		if (string.IsNullOrEmpty(errorMessage))
		{
			errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_SYSTEM_ERROR);
		}

		Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
	}

	/// <summary>
	/// カートオブジェクト取得
	/// </summary>
	/// <returns></returns>
	protected CartObjectList GetCartObjectList()
	{
		return this.Process.GetCartObjectList();
	}

	/// <summary>
	/// 遷移先URLの正当性チェック（外部サイトにジャンプしようとしていた場合TOPページに書き換え）
	/// </summary>
	/// <param name="nextUrl">遷移先URL</param>
	/// <returns>次URL</returns>
	public string NextUrlValidation(string nextUrl)
	{
		return this.Process.NextUrlValidation(nextUrl);
	}
	/// <summary>
	/// 遷移先URLの正当性チェック（外部サイトにジャンプしようとしていた場合TOPページに書き換え）
	/// </summary>
	/// <param name="request">HTTPリクエスト</param>
	/// <param name="nextUrl">遷移先URL</param>
	/// <returns>次URL</returns>
	public string NextUrlValidation(HttpRequest request, string nextUrl)
	{
		return this.Process.NextUrlValidation(request, nextUrl);
		}

	/// <summary>
	/// ログインユーザーのCPMクラスタを含んでいるか（部分一致）
	/// </summary>
	/// <param name="cpmClusterNames">CPMクラスタ名リスト</param>
	/// <returns>含んでいるか</returns>
	public bool ContainsCpmCluster(params string[] cpmClusterNames)
	{
		if (this.LoginUserCpmClusterName == null)
		{
			return cpmClusterNames.Contains(null);
		}
		return cpmClusterNames.Any(name => this.LoginUserCpmClusterName.Contains(name));
	}

	/// <summary>
	/// 郵便番号検索
	/// </summary>
	/// <param name="senderControl">郵便番号検索の発生元コントロール（郵便番号テキストボックスor検索ボタン）</param>
	/// <param name="unavailableShippingZip">配送不可郵便番号</param>
	/// <returns>エラーメッセージ</returns>
	protected string SearchZipCode(object senderControl, string unavailableShippingZip = "")
	{
		return this.Process.SearchZipCode(senderControl, unavailableShippingZip);
	}

	/// <summary>
	/// 郵便番号検索のエラーメッセージを削除する
	/// </summary>
	/// <param name="giftOrderOptionEnabled">ギフト購入オプション可否フラグ</param>
	/// <param name="repeaterCartList">リピーターカートリスト</param>
	/// <param name="lpPageFlg">LPページであるかの判定</param>
	protected void ResetAddressSearchResultErrorMessage(bool giftOrderOptionEnabled, Repeater repeaterCartList, bool lpPageFlg = false)
	{
		this.Process.ResetAddressSearchResultErrorMessage(giftOrderOptionEnabled, repeaterCartList, lpPageFlg);
	}

	/// <summary>
	/// 会員ランク情報の存在確認
	/// </summary>
	/// <returns>存在判定結果</returns>
	public bool HasLoginMemberRank()
	{
		if (Constants.MEMBER_RANK_OPTION_ENABLED == false) return false;
		return (this.LoginMemberRankInfo != null);
	}

	/// <summary>
	/// 購入会員ランク・販売期間・在庫有無チェック
	/// </summary>
	/// <param name="productData">商品情報</param>
	/// <param name="stockFlg">在庫フラグ</param>
	/// <returns>エラーメッセージ</returns>
	/// <remarks>購入会員ランクチェック、販売期間チェック、在庫有無チェックの順に優先度が高い</remarks>
	public string CheckBuyableMemberRank(DataRowView productData, bool stockFlg)
		{
		// 購入可能会員ランク
		string buyableMemberRank = (string)productData[Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK];

		// 購入可能な会員ランク？
		if (MemberRankOptionUtility.CheckMemberRankPermission(this.MemberRankId, buyableMemberRank) == false)
		{
			return OrderCommon.GetErrorMessage(
				OrderErrorcode.SellMemberRankError,
				productData[Constants.FIELD_PRODUCT_NAME].ToString(),
				MemberRankOptionUtility.GetMemberRankName(buyableMemberRank));
		}
		
		// 商品販売前？
		if (ProductCommon.IsSellBefore(productData))
		{
			return OrderCommon.GetErrorMessage(
				OrderErrorcode.ProductBeforeSellTerm,
				productData[Constants.FIELD_PRODUCT_NAME].ToString()
			);
		}

		// 商品販売後？
		if (ProductCommon.IsSellAfter(productData))
		{
			return OrderCommon.GetErrorMessage(
				OrderErrorcode.ProductOutOfSellTerm,
				productData[Constants.FIELD_PRODUCT_NAME].ToString()
			);
		}

		// 商品販売期間外？
		if (ProductCommon.IsSellTerm(productData) == false)
		{
			return OrderCommon.GetErrorMessage(
				OrderErrorcode.ProductInvalid,
				productData[Constants.FIELD_PRODUCT_NAME].ToString()
			);
		}
		// 在庫なし？
		if (stockFlg == false)
		{
			return OrderCommon.GetErrorMessage(
				OrderErrorcode.ProductNoStockBeforeCart,
				productData[Constants.FIELD_PRODUCT_NAME].ToString()
			);
		}

		return "";
	}

	/// <summary>
	/// バリエーション変更向けデータバインド
	/// </summary>
	/// <param name="target">コントロールを探すターゲット</param>
	/// <param name="className">クラス名</param>
	public static void DataBindByClass(Control target, string className)
	{
		foreach (Control cInner in target.Controls)
		{
			if ((cInner is System.Web.UI.HtmlControls.HtmlGenericControl)
				&& ((System.Web.UI.HtmlControls.HtmlGenericControl)cInner).Attributes["class"] == className)
			{
				cInner.DataBind();
			}
			else if (cInner.Controls.Count != 0)
			{
				DataBindByClass(cInner, className);
			}
		}
	}

	/// <summary>
	/// 公開範囲設定 判定結果よりページのリダイレクト
	/// </summary>
	/// <param name="result">公開設定 判定結果</param>
	protected void ReleaseRangeRedirect(ReleaseRangeResult result)
	{
		if (this.Request.UserHostAddress == Constants.ALLOWED_IP_ADDRESS_FOR_WEBCAPTURE) return;

		// 判定にログインが必要な場合はログイン画面に遷移してログイン後に本ページに戻す
		if (result.RequiredLoginStatus == ReleaseRangeResult.RequiredLogin.Required)
		{
			CheckLoggedIn(this.RawUrl);
		}

		// １つでも範囲外が存在する場合は404ページを表示
		if ((this.IsPreview == false)
			&& ((result.Publish == ReleaseRangeResult.RangeResult.Out)
			|| (result.PublishDate == ReleaseRangeResult.RangeResult.Out)
			|| (result.MemberRank == ReleaseRangeResult.RangeResult.Out)
			|| (result.TargetList == ReleaseRangeResult.RangeResult.Out)))
		{
			this.Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_404_ERROR);
			var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			urlCreator.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOTOP);
			this.Response.Redirect(urlCreator.CreateUrl());
		}
	}

	/// <summary>
	/// 公開範囲設定 アクセスユーザ
	/// </summary>
	/// <returns>アクセスユーザ</returns>
	protected ReleaseRangeAccessUser GetReleaseRangeAccessUser()
		{
		var accessUser = new ReleaseRangeAccessUser
			{
			Now = DateTime.Now,
			MemberRankInfo = this.LoginMemberRankInfo,
			IsLoggedIn = this.IsLoggedIn,
			HitTargetListId = this.LoginUserHitTargetListIds
		};

		return accessUser;
	}

	/// <summary>
	/// リンク式決済か
	/// </summary>
	/// <returns>リンク式決済か</returns>
	protected bool IsCreditCardLinkPayment()
				{
		return this.Process.IsCreditCardLinkPayment();
						}

	/// <summary>
	/// 商品税抜きのメッセージ表示する判定
	/// </summary>
	/// <param name="orderInfo">注文情報</param>
	/// <returns>メッセージ表示か</returns>
	public bool IsDisplayProductTaxExcludedMessage(object orderInfo)
					{
		// 海外配送時の税込請求、またはグローバル対応なしもしくは注文情報がない場合、
		// メッセージを表示しない
		if (this.ProductIncludedTaxFlg
			|| (Constants.GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG && (this.ProductIncludedTaxFlg == false))
			|| (Constants.GLOBAL_OPTION_ENABLE == false)
			|| (orderInfo == null)) return false;

		// 配送先の国と州をまとめる
		var addressInfo = new List<KeyValuePair<string, string>>();
		if (orderInfo is DataView)
						{
			addressInfo = ((DataView)orderInfo).Cast<DataRowView>()
				.Select(item => new KeyValuePair<string, string>(
					(string)item[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE],
					(string)item[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5]))
				.ToList();
						}
		if (orderInfo is CartObject)
						{
			addressInfo = ((CartObject)orderInfo).Shippings
				.Select(item => new KeyValuePair<string, string>(item.ShippingCountryIsoCode, item.Addr5))
				.ToList();
						}

		// 配送先情報がなければ、メッセージを表示しない
		if (addressInfo.Count == 0) return false;

		// 運用地はアメリカであれば、配送先の国と州を考慮する
		if (Constants.OPERATIONAL_BASE_ISO_CODE == Constants.COUNTRY_ISO_CODE_US)
		{
			return addressInfo.Any(
				item => ((item.Key != Constants.OPERATIONAL_BASE_ISO_CODE)
					|| (item.Value != Constants.OPERATIONAL_BASE_PROVINCE)));
					}

		return addressInfo.Any(item => item.Key != Constants.OPERATIONAL_BASE_ISO_CODE);
				}

	/// <summary>
	/// Write Log Success Aftee
	/// </summary>
	/// <param name="response">Response</param>
	[WebMethod]
	public static void WriteLogSuccessAftee(string response)
	{
		var afteeResponse = JsonConvert.DeserializeObject<AfteeResponse>(response);
		AfteePaymentApiFacade.WriteLogResult(afteeResponse);
	}

	/// <summary>
	/// Write Log Error Aftee
	/// </summary>
	/// <param name="response">Response</param>
	[WebMethod]
	public static void WriteLogErrorAftee(string response)
	{
		var afteeResponse = JsonConvert.DeserializeObject<AfteeResponse>(response);

		var errorMessageForLog = new StringBuilder();
		errorMessageForLog.Append("Aftee Authority Error:").AppendLine(afteeResponse.Message);

		var errorMessagesForDisplay = new StringBuilder();
		errorMessagesForDisplay.AppendLine("aftee翌月払い決済に失敗しました。")
			.AppendLine("カード情報入力内容に誤りが無いかご確認下さい。");

		if (afteeResponse.Errors != null)
		{
			foreach (var error in afteeResponse.Errors)
			{
				errorMessageForLog.AppendLine(error.Code + ":");
				errorMessageForLog.AppendLine(string.Join(Environment.NewLine, error.Messages));
				errorMessageForLog.AppendLine(string.Join(Environment.NewLine, error.Params));

				errorMessagesForDisplay.AppendLine(string.Format("{0}:{1}",
					error.Code,
					string.Join(Environment.NewLine, error.Messages)));
			}
		}
		else
		{
			errorMessagesForDisplay.AppendLine(afteeResponse.Message);
		}

		FileLogger.WriteError(errorMessageForLog.ToString());
	}

	/// <summary>
	/// Write Log Success Atone
	/// </summary>
	/// <param name="response">Response</param>
	[WebMethod]
	public static void WriteLogSuccessAtone(string response)
	{
		var atoneResponse = JsonConvert.DeserializeObject<AtoneResponse>(response);
		AtonePaymentApiFacade.WriteLogResult(atoneResponse);
	}

	/// <summary>
	/// Write Log Error Atone
	/// </summary>
	/// <param name="response">Response</param>
	[WebMethod]
	public static void WriteLogErrorAtone(string response)
			{
		var atoneResponse = JsonConvert.DeserializeObject<AtoneResponse>(response);

		var errorMessagesForLog = new StringBuilder();
		errorMessagesForLog.Append("Atone Authority Error:").AppendLine(atoneResponse.Message);

		var errorMessagesForDisplay = new StringBuilder();
		errorMessagesForDisplay.AppendLine("atone翌月払い決済に失敗しました。")
			.AppendLine("カード情報入力内容に誤りが無いかご確認下さい。");

		if (atoneResponse.Errors != null)
				{
			foreach (var error in atoneResponse.Errors)
			{
				errorMessagesForLog.AppendLine(error.Code + ":");
				errorMessagesForLog.AppendLine(string.Join(Environment.NewLine, error.Messages));
				errorMessagesForLog.AppendLine(string.Join(Environment.NewLine, error.Params));

				errorMessagesForDisplay.AppendLine(string.Format("{0}:{1}",
					error.Code,
					string.Join(Environment.NewLine, error.Messages)));
				}
		}

		FileLogger.WriteError(errorMessagesForLog.ToString());
	}

	/// <summary>
	/// JAFシングルサインオン用の遷移先URLを作成
	/// </summary>
	/// <param name="url">遷移先URL</param>
	/// <param name="productId">商品Id</param>
	/// <param name="variationId">バリエーションId</param>
	/// <param name="isFixedPurchase">定期購入であるか</param>
	/// <returns>遷移先URL(コールバックURL付)</returns>
	protected string CreateUrlForJafSingleSignOn(string url, string productId, string variationId, bool isFixedPurchase = false)
			{
		var nextUrl = CreateAddCartUrlForJaf(productId, variationId, isFixedPurchase);
		var qUrl = 
			new UrlCreator(Constants.PROTOCOL_HTTPS
				+ Constants.SITE_DOMAIN
				+ Constants.PATH_ROOT_FRONT_PC
				+ Constants.PAGE_FRONT_SINGLE_SIGN_ON)
			.AddParam(Constants.JAF_REQUEST_KEY_USER_ID, (this.LoginUserId ?? string.Empty))
			.AddParam(Constants.REQUEST_KEY_FRONT_NEXT_URL, nextUrl)
			.CreateUrl();

		var nurl = new UrlCreator(url)
			.AddParam(Constants.REQUEST_KEY_SINGLE_SIGN_ON_SERVICEID, Constants.JAF_SERVICE_ID)
			.AddParam(Constants.REQUEST_KEY_SINGLE_SIGN_ON_URL, qUrl)
			.CreateUrl();

		return nurl;
	}

	/// <summary>
	/// JAF用のカート投入URLを作成
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	/// <param name="isFixedPurchase">定期購入であるか</param>
	/// <param name="prdcnt">カート商品追加数</param>
	/// <returns>JAF用のカート投入URL</returns>
	protected string CreateAddCartUrlForJaf(string productId, string variationId = "", bool isFixedPurchase = false, int prdcnt = 1)
	{
		const string SEQUENTIAL_ORDER = "1";

		var urlCreator =
			new UrlCreator(Constants.JAF_RETURN_URL_FROM_SSO)
				.AddParam(Constants.REQUEST_KEY_CART_ACTION, SEQUENTIAL_ORDER)
				.AddParam(Constants.REQUEST_KEY_PRODUCT_ID + SEQUENTIAL_ORDER, productId)
				.AddParam(Constants.REQUEST_KEY_VARIATION_ID + SEQUENTIAL_ORDER, variationId)
				.AddParam(Constants.REQUEST_KEY_PRODUCT_COUNT + SEQUENTIAL_ORDER, prdcnt.ToString());

		if (isFixedPurchase)
			urlCreator.AddParam(Constants.REQUEST_KEY_FIXED_PURCHASE + SEQUENTIAL_ORDER, "1");

		var url = urlCreator.CreateUrl();
		return url;
	}

	/// <summary>
	/// 新LPカートリストが利用可能か
	/// </summary>
	/// <returns>利用可能：TRUE、利用不可：FALSE</returns>
	public bool CanUseCartListLp()
	{
		var result = false;
		if (Constants.CART_LIST_LP_OPTION)
		{
			var landingPageDesignModel = new LandingPageService().GetPageByFileName(Constants.CARTLIST_LP_FILE_NAME.Replace(".aspx", string.Empty));
			result = (landingPageDesignModel.Length > 0) && (landingPageDesignModel[0].IsPublished);
		}

		return result;
	}
	
	/// <summary>
	/// Check Site Domain And Redirect With Post Data
	/// </summary>
	public void CheckSiteDomainAndRedirectWithPostData()
	{
		this.Process.CheckSiteDomainAndRedirectWithPostData();
	}

	/// <summary>
	/// Set Information Receiving Store
	/// </summary>
	/// <param name="cartList">Cart List</param>
	public void SetInformationReceivingStore(CartObjectList cartList)
	{
		this.Process.SetInformationReceivingStore(cartList);
	}

	/// <summary>
	/// キャプチャ認証チェック
	/// </summary>
	/// <returns>成功：true、失敗：false（※キャプチャ認証利用なしの場合もtrueを返す）</returns>
	protected bool CheckCaptcha()
	{
		// キャプチャ認証サイトキー指定なしの場合は何もしない
		if (string.IsNullOrEmpty(Constants.RECAPTCHA_SITE_KEY)) return true;

		// キャプチャ認証コントロールがなしの場合は何もしない
		if (this.Captcha == null) return true;

		// キャプチャ認証成功？
		if (this.Captcha.IsSuccess) return true;

		return false;
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
	/// <returns></returns>
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
	/// ペイパル認証情報をセッションにセット
	/// </summary>
	/// <param name="wucPaypalScriptsForm">WrappedPayPalPayScriptsFormControl</param>
	protected void SetPaypalInfoToSession(WrappedPayPalPayScriptsFormControl wucPaypalScriptsForm)
	{
		this.Process.SetPaypalInfoToSession(wucPaypalScriptsForm);
	}

	/// <summary>
	/// 国情報をコントロールのCSSクラス情報に付与
	/// </summary>
	/// <param name="inputControls">入力対象コントロールリスト</param>
	/// <param name="countryIsoCode">国ISOコード</param>
	protected void AddCountryInfoToControlForChangeCountry(WrappedWebControl[] inputControls, string countryIsoCode)
	{
		this.Process.AddCountryInfoToControlForChangeCountry(inputControls, countryIsoCode);
	}

	/// <summary>
	/// 国情報によりCustomValidatorのValidationGroupをセット
	/// </summary>
	/// <param name="targetControls">対象コントロールリスト</param>
	/// <param name="validationGroup">設定するバリデーショングループ</param>
	protected void ChangeValidationGroupForChangeCountry(
		WrappedControl[] targetControls,
		string validationGroup)
	{
		this.Process.ChangeValidationGroupForChangeCountry(targetControls, validationGroup);
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
	/// Get the text box wrapped from the repeater
	/// </summary>
	/// <param name="item">Repeater item</param>
	/// <param name="idTextBox">The id of the text box</param>
	/// <returns>Wrapped text box by id</returns>
	public WrappedTextBox GetWrappedTextBoxFromRepeater(RepeaterItem item, string idTextBox)
	{
		return this.Process.GetWrappedTextBoxFromRepeater(item, idTextBox);
	}

	/// <summary>
	/// Get the link button wrapped from the repeater
	/// </summary>
	/// <param name="item">Repeater item</param>
	/// <param name="idLinkButton">The id of the link button</param>
	/// <returns>Wrapped link button by id</returns>
	public WrappedLinkButton GetWrappedLinkButtonFromRepeater(RepeaterItem item, string idLinkButton)
	{
		return this.Process.GetWrappedLinkButtonFromRepeater(item, idLinkButton);
	}

	/// <summary>
	/// LINE連携
	/// </summary>
	/// <param name="nextUrl">NextURL</param>
	/// <param name="socialPlusCallbackPath">コールバックパス(ソーシャルプラス用)</param>
	/// <param name="socialPlusErrorCallbackPath">エラー時コールバックパス(ソーシャルプラス用)</param>
	/// <param name="socialPlusProfile">取得範囲(ソーシャルプラス用)</param>
	/// <param name="uriAuthority">URLAuthority</param>
	/// <param name="returnPath">戻り先パス</param> 
	/// <returns>リクエストURL</returns>
	public string LineConnect(
		string nextUrl,
		string socialPlusCallbackPath,
		string socialPlusErrorCallbackPath,
		bool socialPlusProfile,
		string uriAuthority,
		string returnPath = null)
	{
		var requestUrl = string.Empty;
		if (Constants.SOCIAL_LOGIN_ENABLED)
		{
			if (string.IsNullOrEmpty(returnPath))
			{
				requestUrl = 
					w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
						SocialLoginApiProviderType.Line,
						socialPlusCallbackPath,
						socialPlusErrorCallbackPath,
						socialPlusProfile,
						uriAuthority);
			}
			else
			{
				requestUrl = 
					w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
						SocialLoginApiProviderType.Line,
						socialPlusCallbackPath,
						socialPlusErrorCallbackPath,
						socialPlusProfile,
						uriAuthority,
						returnPath);
			}
			
		}
		else if ((Constants.SOCIAL_LOGIN_ENABLED == false) && w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED)
		{
			Session[Constants.SESSION_KEY_NEXT_URL] = (Constants.PATH_ROOT + nextUrl);
			requestUrl = LineUtil.CreateConnectLineUrl(Session.SessionID);
		}
		return requestUrl;
	}

	/// <summary>
	/// Set Zip Code Textbox
	/// </summary>
	/// <param name="tbZip">Textbox zip</param>
	/// <param name="tbZip1">Textbox zip 1</param>
	/// <param name="tbZip2">Textbox zip 2</param>
	/// <param name="zip">Value of zip</param>
	public void SetZipCodeTextbox(
		WrappedTextBox tbZip,
		WrappedTextBox tbZip1,
		WrappedTextBox tbZip2,
		string value)
	{
		this.Process.SetZipCodeTextbox(
			tbZip,
			tbZip1,
			tbZip2,
			value);
	}

	/// <summary>
	/// Set Tel Textbox
	/// </summary>
	/// <param name="tbTel">Textbox tel</param>
	/// <param name="tbTel1">Textbox tel 1</param>
	/// <param name="tbTel2">Textbox tel 2</param>
	/// <param name="tbTel3">Textbox tel 3</param>
	/// <param name="value">Value of telephone</param>
	public void SetTelTextbox(
		WrappedTextBox tbTel,
		WrappedTextBox tbTel1,
		WrappedTextBox tbTel2,
		WrappedTextBox tbTel3,
		string value)
	{
		this.Process.SetTelTextbox(
			tbTel,
			tbTel1,
			tbTel2,
			tbTel3,
			value);
	}

	/// <summary>
	/// Binding address by global zipcode
	/// </summary>
	/// <param name="countryIsoCode">Country ISO Code</param>
	/// <param name="globalZipCode">Global zipcode</param>
	/// <param name="wtbAddr2">Wrapped textbox address 2</param>
	/// <param name="wtbAddr4">Wrapped textbox address 4</param>
	/// <param name="wtbAddr5">Wrapped textbox address 5</param>
	/// <param name="wddlShippingAddr2">Wrapped dropdownlist Taiwan prefecture/city</param>
	/// <param name="wddlShippingAddr3">Wrapped dropdownlist Taiwan town</param>
	/// <param name="wddlShippingAddr5">Wrapped dropdownlist US state</param>
	public void BindingAddressByGlobalZipcode(
		string countryIsoCode,
		string globalZipCode,
		WrappedTextBox wtbAddr2,
		WrappedTextBox wtbAddr4,
		WrappedTextBox wtbAddr5,
		WrappedDropDownList wddlShippingAddr2,
		WrappedDropDownList wddlShippingAddr3,
		WrappedDropDownList wddlShippingAddr5)
	{
		this.Process.BindingAddressByGlobalZipcode(
			countryIsoCode,
			globalZipCode,
			wtbAddr2,
			wtbAddr4,
			wtbAddr5,
			wddlShippingAddr2,
			wddlShippingAddr3,
			wddlShippingAddr5);
	}

	/// <summary>
	/// Send authentication code
	/// </summary>
	/// <param name="wtbAuthenticationCode">Wrapped textbox authentication code</param>
	/// <param name="wlbAuthenticationStatus">Wrapped label authentication status</param>
	/// <param name="userTel">User tel</param>
	/// <param name="userAddrCountryIsoCode">User address country iso code</param>
	public void SendAuthenticationCode(
		WrappedTextBox wtbAuthenticationCode,
		WrappedLabel wlbAuthenticationStatus,
		string userTel,
		string userAddrCountryIsoCode)
	{
		this.Process.SendAuthenticationCode(
			wtbAuthenticationCode,
			wlbAuthenticationStatus,
			userTel,
			userAddrCountryIsoCode);
	}

	/// <summary>
	/// Exec check authentication code
	/// </summary>
	/// <param name="wlbGetAuthenticationCode">Wrapped linkbutton get authentication code</param>
	/// <param name="wtbAuthenticationCode">Wrapped textbox authentication code</param>
	/// <param name="wlbAuthenticationMessage">Wrapped label authentication message</param>
	/// <param name="wlbAuthenticationStatus">Wrapped label authentication status</param>
	/// <param name="userTel">User tel</param>
	/// <param name="userAddrCountryIsoCode">User address country iso code</param>
	/// <returns>Error messages</returns>
	public Dictionary<string, string> ExecCheckAuthenticationCode(
		WrappedLinkButton wlbGetAuthenticationCode,
		WrappedTextBox wtbAuthenticationCode,
		WrappedLabel wlbAuthenticationMessage,
		WrappedLabel wlbAuthenticationStatus,
		string userTel,
		string userAddrCountryIsoCode)
	{
		return this.Process.ExecCheckAuthenticationCode(
			wlbGetAuthenticationCode,
			wtbAuthenticationCode,
			wlbAuthenticationMessage,
			wlbAuthenticationStatus,
			userTel,
			userAddrCountryIsoCode);
	}

	/// <summary>
	/// Get verification code note
	/// </summary>
	/// <param name="countryIsoCode">Country iso code</param>
	/// <returns>Verification_code note</returns>
	public string GetVerificationCodeNote(string countryIsoCode = "")
	{
		return this.Process.GetVerificationCodeNote(countryIsoCode);
	}

	/// <summary>
	/// Remove error input class
	/// </summary>
	/// <param name="wTextBox">Wrapped text box</param>
	public void RemoveErrorInputClass(WrappedTextBox wTextBox)
	{
		this.Process.RemoveErrorInputClass(wTextBox);
	}

	/// <summary>
	/// Display authentication code
	/// </summary>
	/// <param name="wlbGetAuthenticationCode">Wrapped link button get authentication code</param>
	/// <param name="wtbAuthenticationCode">Wrapped text box authentication code</param>
	/// <param name="wlbAuthenticationStatus">Wrapped label authentication status</param>
	/// <param name="authenticationCode">Authentication code</param>
	public void DisplayAuthenticationCode(
		WrappedLinkButton wlbGetAuthenticationCode,
		WrappedTextBox wtbAuthenticationCode,
		WrappedLabel wlbAuthenticationStatus = null,
		string authenticationCode = "")
	{
		this.Process.DisplayAuthenticationCode(
			wlbGetAuthenticationCode,
			wtbAuthenticationCode,
			wlbAuthenticationStatus,
			authenticationCode);
	}

	/// <summary>
	/// Get value for telephone
	/// </summary>
	/// <param name="wtbTel1_1">Wrapped text box telephone1 1</param>
	/// <param name="wtbTel1_2">Wrapped text box telephone1 2</param>
	/// <param name="wtbTel1_3">Wrapped text box telephone1 3</param>
	/// <param name="wtbTel1">Wrapped text box telephone1</param>
	/// <param name="wtbTel1Global">Wrapped text box telephone global</param>
	/// <param name="userAddrCountryIsoCode">User addr country iso code</param>
	/// <returns>Telephone number</returns>
	public string GetValueForTelephone(
		WrappedTextBox wtbTel1_1,
		WrappedTextBox wtbTel1_2,
		WrappedTextBox wtbTel1_3,
		WrappedTextBox wtbTel1,
		WrappedTextBox wtbTel1Global,
		string userAddrCountryIsoCode)
	{
		return this.Process.GetValueForTelephone(
			wtbTel1_1,
			wtbTel1_2,
			wtbTel1_3,
			wtbTel1,
			wtbTel1Global,
			userAddrCountryIsoCode);
	}

	/// <summary>
	/// Stop time count
	/// </summary>
	public void StopTimeCount()
	{
		this.Process.StopTimeCount();
	}

	/// <summary>
	/// Get wrapped control of text box authentication code
	/// </summary>
	/// <param name="isCountryJp">Is country Jp</param>
	/// <param name="control">Control</param>
	/// <returns>Wrapped text box</returns>
	public WrappedTextBox GetWrappedTextBoxAuthenticationCode(
		bool isCountryJp,
		Control control = null)
	{
		return this.Process.GetWrappedTextBoxAuthenticationCode(isCountryJp, control);
	}

	/// <summary>
	/// Get wrapped control of label authentication status
	/// </summary>
	/// <param name="isCountryJp">Is country Jp</param>
	/// <param name="control">Control</param>
	/// <returns>Wrapped label</returns>
	public WrappedLabel GetWrappedControlOfLabelAuthenticationStatus(
		bool isCountryJp,
		Control control = null)
	{
		return this.Process.GetWrappedControlOfLabelAuthenticationStatus(isCountryJp, control);
	}

	/// <summary>
	/// Get wrapped control of label authentication message
	/// </summary>
	/// <param name="isCountryJp">Is country Jp</param>
	/// <param name="control">Control</param>
	/// <returns>Wrapped label</returns>
	public WrappedLabel GetWrappedControlOfLabelAuthenticationMessage(
		bool isCountryJp,
		Control control = null)
	{
		return this.Process.GetWrappedControlOfLabelAuthenticationMessage(isCountryJp, control);
	}

	/// <summary>
	/// Get wrapped control of link button authentication code
	/// </summary>
	/// <param name="isCountryJp">Is country Jp</param>
	/// <param name="control">Control</param>
	/// <returns>Wrapped link button</returns>
	public WrappedLinkButton GetWrappedControlOfLinkButtonAuthenticationCode(
		bool isCountryJp,
		Control control = null)
	{
		return this.Process.GetWrappedControlOfLinkButtonAuthenticationCode(isCountryJp, control);
	}

	
	/// <summary>
	/// CSSリンク設定
	/// </summary>
	/// <param name="strControlId">コントロールID</param>
	/// <param name="strCssPath">CSSファイルパス</param>
	protected void SetCssLink(string strControlId, string strCssPath)
	{
		HtmlLink hlCssLink = (HtmlLink)this.Page.Form.FindControl(strControlId);
		if (hlCssLink != null)
		{
			hlCssLink.Href = strCssPath;
		}
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
	public string LoginUserNickName
	{
		get { return this.Process.LoginUserNickName; }
		set { this.Process.LoginUserNickName = value; }
	}
	/// <summary>ログインユーザーメールアドレス</summary>
	public string LoginUserMail
	{
		get { return this.Process.LoginUserMail; }
		set { this.Process.LoginUserMail = value; }
	}
	/// <summary>ログインユーザーメールアドレス2</summary>
	public string LoginUserMail2
	{
		get { return this.Process.LoginUserMail2; }
		set { this.Process.LoginUserMail2 = value; }
	}
	/// <summary>ログインユーザー生年月日</summary>
	public string LoginUserBirth
	{
		get { return this.Process.LoginUserBirth; }
		set { this.Process.LoginUserBirth = value; }
	}
	/// <summary>前回ログイン日時</summary>
	public string LastLoggedinDate
			{
		get { return this.Process.LastLoggedinDate; }
		set { this.Process.LastLoggedinDate = value; }
			}
	/// <summary>ログインユーザかんたん会員フラグ</summary>
	public string LoginUserEasyRegisterFlg
			{
		get { return this.Process.LoginUserEasyRegisterFlg; }
		set { this.Process.LoginUserEasyRegisterFlg = value; }
		}
	/// <summary>ログインユーザーCPMクラスタ名</summary>
	public string LoginUserCpmClusterName
	{
		get { return this.Process.LoginUserCpmClusterName; }
		set { this.Process.LoginUserCpmClusterName = value; }
	}
	/// <summary>ログインユーザー定期会員フラグ</summary>
	public string LoginUserFixedPurchaseMemberFlg
	{
		get { return this.Process.LoginUserFixedPurchaseMemberFlg; }
		set { this.Process.LoginUserFixedPurchaseMemberFlg = value; }
	}
	/// <summary>会員ランク情報</summary>
	public MemberRankModel LoginMemberRankInfo
	{
		get { return this.Process.LoginMemberRankInfo; }
		set { this.Process.LoginMemberRankInfo = value; }
	}
	/// <summary>会員ランクID</summary>
	public string MemberRankId { get { return this.Process.MemberRankId ?? string.Empty; } }
	/// <summary>会員ランク名</summary>
	public string MemberRankName
	{
		get { return this.Process.MemberRankName; }
	}
	/// <summary>ログインユーザー会員ランクID</summary>
	public string LoginUserMemberRankId
	{
		get { return this.Process.LoginUserMemberRankId; }
		set { this.Process.LoginUserMemberRankId = value; }
	}
	/// <summary>ユーザー管理レベル</summary>
	public string UserManagementLevelId
	{
		get { return this.Process.UserManagementLevelId; }
		set { this.Process.UserManagementLevelId = value; }
	}
	/// <summary>ログインユーザに有効なターゲットリスト群</summary>
	public string[] LoginUserHitTargetListIds
	{
		get { return this.Process.LoginUserHitTargetListIds; }
		set { this.Process.LoginUserHitTargetListIds = value; }
	}

	/// <summary>User Fixed Purchase Member Flg</summary>
	public string UserFixedPurchaseMemberFlg
	{
		get { return this.Process.UserFixedPurchaseMemberFlg; }
	}
	/// <summary>かんたん会員かどうか</summary>
	public bool IsEasyUser
	{
		get { return this.Process.IsEasyUser; }
	}
	/// <summary>Amazonログイン状態</summary>
	public bool IsAmazonLoggedIn
	{
		get { return this.Process.IsAmazonLoggedIn; }
	}
	/// <summary>会員登録入力情報</summary>
	public UserModel RegisterUser
	{
		get { return this.Process.RegisterUser; }
		set { this.Process.RegisterUser = value; }
	}
	/// <summary>楽天IDConnect会員登録か</summary>
	protected bool IsRakutenIdConnectUserRegister
	{
		get { return this.Process.IsRakutenIdConnectUserRegister; }
	}
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
	public UserPointObject LoginUserPoint
	{
		get { return this.Process.LoginUserPoint; }
		set { this.Process.LoginUserPoint = value; }
	}

	/// <summary>ターゲットページ</summary>
	public string SessionParamTargetPage
	{
		get { return this.Process.SessionParamTargetPage; }
		set { this.Process.SessionParamTargetPage = value; }
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
	/// Is not country JP
	/// </summary>
	/// <param name="countryIsoCode">Country ISO code</param>
	/// <returns>True: is not country Jp, otherwise: false</returns>
	protected bool IsNotCountryJp(string countryIsoCode)
	{
		return this.Process.IsNotCountryJp(countryIsoCode);
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
	/// 住所３が必須かどうか
	/// </summary>
	/// <param name="countryIsoCode">国ISOコード</param>
	/// <returns>必須か</returns>
	protected static bool IsAddress3Necessary(string countryIsoCode)
	{
		return GlobalAddressUtil.IsAddress3Necessary(countryIsoCode);
	}

	/// <summary>
	/// 該当ユーザーは該当商品に定期購入制限されるかをチェック
	/// </summary>
	/// <param name="shopId">ショップId</param>
	/// <param name="productId">商品Id</param>
	/// <returns>制限されているかどうか</returns>
	public bool CheckFixedPurchaseLimitedUserLevel(string shopId, string productId)
	{
		return this.Process.CheckFixedPurchaseLimitedUserLevel(shopId, productId);
	}

	/// <summary>
	/// 商品IDから有効な頒布会コースを取得
	/// </summary>
	/// <param name="shopId">ショップid</param>
	/// <param name="productId">商品id</param>
	/// <param name="variationId">バリエーションid</param>
	/// <returns>頒布会モデル配列</returns>
	public SubscriptionBoxModel[] GetAvailableSubscriptionBoxesByProductId(string shopId, string productId, string variationId)
	{
		return this.Process.GetAvailableSubscriptionBoxesByProductId(shopId, productId, variationId);
	}

	/// <summary>
	/// 請求先住所取得オプションの対象か判定
	/// </summary>
	/// <returns>判定結果（TRUE：対象、FALSE：）</returns>
	protected bool IsTargetToExtendedAmazonAddressManagerOption()
	{
		return this.Process.IsTargetToExtendedAmazonAddressManagerOption();
	}

	/// <summary>ブランドID</summary>
	public string BrandId
	{
		get { return this.Process.BrandId; }
		set { this.Process.BrandId = value; }
	}
	/// <summary>最後に表示していたブランドID</summary>
	public string LastDisplayedBrandId
	{
		get { return StringUtility.ToEmpty(this.Session[Constants.SESSION_KEY_LAST_DISPLAYED_BRAND_ID]); }
		set { this.Session[Constants.SESSION_KEY_LAST_DISPLAYED_BRAND_ID] = value; }
	}
	/// <summary>ブランド名</summary>
	public string BrandName
		{
		get { return this.Process.BrandName; }
		set { this.Process.BrandName = value; }
		}
	/// <summary>ブランドタイトル</summary>
	public string BrandTitle
		{
		get { return this.Process.BrandTitle; }
		set { this.Process.BrandTitle = value; }
		}
	/// <summary>ブランドSEOキーワード</summary>
	public string BrandSeoKeyword
		{
		get { return this.Process.BrandSeoKeyword; }
		set { this.Process.BrandSeoKeyword = value; }
		}
	/// <summary>ブランド追加タグ情報</summary>
	public string BrandAdditionalDsignTag
		{
		get { return this.Process.BrandAdditionalDsignTag; }
		set { this.Process.BrandAdditionalDsignTag = value; }
	}
	/// <summary>EFOオプション有効か</summary>
	public bool IsEfoOptionEnabled
	{
		get { return this.Process.IsEfoOptionEnabled; }
	}
	/// <summary>ユーザー台湾地域情報</summary>
	protected Dictionary<string, Dictionary<string, string>> UserTwDistrictDict
	{
		get { return this.Process.UserTwDistrictDict; }
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
	/// <summary>Post Params</summary>
	public Dictionary<string, string> PostParams
	{
		get { return this.Process.PostParams; }
	}
	#endregion

	/// <summary>Httpsで表示したいページか</summary>
	public bool NeedsHttps { get { return (this.PageAccessType == PageAccessTypes.Https); } }
	/// <summary>HTTPで表示したいページか</summary>
	public bool NeedsHttp { get { return (this.PageAccessType == PageAccessTypes.Http); } }
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
	/// <summary>日本語か</summary>
	public bool IsJapanese
	{
		get { return (Constants.GLOBAL_OPTION_ENABLE == false || RegionManager.GetInstance().Region.LanguageCode == Constants.LANGUAGE_CODE_JAPANESE); }
	}
	/// <summary>通常の配送先に設定するがチェックされているか</summary>
	public bool? IsDefaultShippingChecked
	{
		get { return (bool?)Session[Constants.SESSION_KEY_DEFAULT_SHIPPING_CHECK]; }
		set { Session[Constants.SESSION_KEY_DEFAULT_SHIPPING_CHECK] = value; }
	}
	/// <summary>通常の支払方法に設定するがチェックされているか</summary>
	public bool? IsDefaultPaymentChecked
	{
		get { return (bool?)Session[Constants.SESSION_KEY_DEFAULT_PAYMENT_CHECK]; }
		set { Session[Constants.SESSION_KEY_DEFAULT_PAYMENT_CHECK] = value; }
	}
	/// <summary>台湾住所都市情報</summary>
	protected ListItem[] AddrTwCityList
	{
		get
		{
			return Constants.GLOBAL_OPTION_ENABLE
				? Constants.TW_CITIES_LIST.Select(state => new ListItem(state)).ToArray()
				: new[] { new ListItem(string.Empty) };
		}
	}
	/// <summary>セッション名一覧</summary>
	private string[] SessionNameList
	{
		get { return this.Session.Cast<object>().Select(sessionName => sessionName.ToString()).ToArray(); }
	}
	/// <summary>プレビュー時のバックアップセッション</summary>
	private List<KeyValuePair<string, object>> SessionBackup { get; set; }
	/// <summary>プレビュー時のバックアップ・削除・復元の対象外セッション</summary>
	private string[] SessionBackupExcludedNameList
	{
		get
		{
			return new[]
			{
				Constants.SESSION_KEY_REFERENCE_DATETIME,
				Constants.SESSION_KEY_REFERENCE_MEMBER_RANK,
				Constants.SESSION_KEY_REFERENCE_TARGET_LIST,
				"ViewStateUserKey",
				Constants.SESSION_KEY_ERROR_MSG
			};
		}
	}
	/// <summary>Is Default Invoice Checked</summary>
	public bool? IsDefaultInvoiceChecked
	{
		get { return (bool?)Session[Constants.SESSION_KEY_DEFAULT_INVOICE_CHECK]; }
		set { Session[Constants.SESSION_KEY_DEFAULT_INVOICE_CHECK] = value; }
	}
	/// <summary>JAF購入可能か</summary>
	protected bool CanPurchaseJaf 
	{
		get { return (this.LoginUserMemberRankId != Constants.JAF_RANK_ID); }
	}
	/// <summary>Amazon連携かどうか</summary>
	protected bool IsAmazonLogin
	{
		get { return this.Process.IsAmazonLogin; }
	}
	/// <summary>AmazonPay(CV2)かつゲスト購入か</summary>
	protected bool IsAmazonCv2Guest
	{
		get { return this.Process.IsAmazonCv2Guest; }
	}
	/// <summary>LINEユーザーID</summary>
	public string LineUserId { get; set; }
	/// <summary>Has authentication code</summary>
	public bool HasAuthenticationCode
	{
		get { return this.Process.HasAuthenticationCode; }
		set { this.Process.HasAuthenticationCode = value; }
	}
	/// <summary>Has authentication code</summary>
	public string AuthenticationCode
	{
		get { return this.Process.AuthenticationCode; }
		set { this.Process.AuthenticationCode = value; }
	}
	/// <summary>Is social login</summary>
	public bool IsSocialLogin
	{
		get { return this.Process.IsSocialLogin; }
	}
	/// <summary>Is preview product mode</summary>
	protected bool IsPreviewProductMode
	{
		get { return ((string)Request[Constants.REQUEST_KEY_PREVIEW_HASH] == ProductPreview.CreateProductDetailHash()); }
	}
	/// <summary>Custom validator control disabled information list</summary>
	protected string[] CustomValidatorControlDisabledInformationList
	{
		get { return this.Process.CustomValidatorControlDisabledInformationList; }
		set { this.Process.CustomValidatorControlDisabledInformationList = value; }
	}
	/// <summary>Custom validator control information list</summary>
	protected string[] CustomValidatorControlInformationList
	{
		get { return this.Process.CustomValidatorControlInformationList; }
		set { this.Process.CustomValidatorControlInformationList = value; }
	}
	/// <summary>キャプチャ認証コントロール</summary>
	protected CaptchaControl Captcha
	{
		get
		{
			var captcha = GetDefaultMasterContentPlaceHolder().FindControl("ucCaptcha");
			return (captcha != null) ? (CaptchaControl)captcha : null;
		}
	}
}
