/*
=========================================================================================================
  Module      : 基底ページプロセス(BasePageProcess.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using w2.App.Common.Affiliate;
using w2.App.Common.Amazon;
using w2.App.Common.Auth.RakutenIDConnect;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.App.Common.Input;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.ECPay;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.Product;
using w2.App.Common.SMS;
using w2.App.Common.User;
using w2.App.Common.User.SocialLogin.Helper;
using w2.App.Common.User.SocialLogin.Util;
using w2.App.Common.Util;
using w2.App.Common.Web.Process;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.GlobalZipcode;
using w2.Domain.MemberRank;
using w2.Domain.ShopShipping;
using w2.Domain.TargetList;
using w2.Domain.TempDatas;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.Zipcode;
using w2.Domain.SubscriptionBox;

/// <summary>
/// OrderLpInputs の概要の説明です
/// </summary>
public class BasePageProcess : CommonPageProcess
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="caller">呼び出し元</param>
	/// <param name="viewState">ビューステート</param>
	/// <param name="context">コンテキスト</param>
	public BasePageProcess(object caller, StateBag viewState, HttpContext context)
		: base(caller, viewState)
	{
		this.Context = context;
	}

	/// <summary>
	/// ページ初期化
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void Page_Init(object sender, EventArgs e)
	{
		// Check if Test EcPay then go to localhost
		if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED)
		{
			CheckSiteDomainAndRedirectWithPostData();
		}

		// ログインチェック
		if (this.NeedsLogin && (this.IsPreview == false)) CheckLoggedIn();

		// リピートプラスONEオプションチェック
		if (this.RepeatPlusOneNeedsRedirect && (this.IsPreview == false)) CheckRepeatPlusOneOptionEnabled();

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// ユーザーにユニークなクッキーを発行するようにする
			//------------------------------------------------------
			if (UserCookieManager.UniqueUserId == "")
			{
				UserCookieManager.UniqueUserId = DateTime.Now.ToString("yyyyMMdd") + "_" + Guid.NewGuid().ToString();
			}

			//------------------------------------------------------
			// サイト変更リクエストがあればクッキーに設定（スマートフォン向け）
			//	※BeginRequest内ではクッキーが消えなかった（セッションクッキーだから？）
			//------------------------------------------------------
			if (Constants.SMARTPHONE_OPTION_ENABLED)
			{
				switch (Request[Constants.REQUEST_KEY_CHANGESITE])
				{
					case Constants.KBN_REQUEST_CHANGESITE_PC:
						CookieManager.SetCookie(
							Constants.COOKIE_KEY_SMARTPHONE_SITE,
							Constants.KBN_REQUEST_CHANGESITE_PC,
							Constants.PATH_ROOT);
						break;

					case Constants.KBN_REQUEST_CHANGESITE_SMARTPHONE:
						CookieManager.SetCookie(
							Constants.COOKIE_KEY_SMARTPHONE_SITE,
							Constants.KBN_REQUEST_CHANGESITE_SMARTPHONE,
							Constants.PATH_ROOT);
						break;
				}
			}

			// Set session referral code
			if (Constants.INTRODUCTION_COUPON_OPTION_ENABLED)
			{
				var referralCode = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_REFERRAL_CODE]);
				if (string.IsNullOrEmpty(referralCode) == false) SessionManager.ReferralCode = referralCode;
			}

			// LPカート遷移のセッション管理
			InitializeSessionForLpCartConfirm();
		}

		//------------------------------------------------------
		// 認証キー系処理（httpsかつセッション切替中でないときに認証キー処理を行う）
		//------------------------------------------------------
		if (Request.IsSecureConnection)
		{
			// 認証キー未発行であれば発行
			SessionSecurityManager.PublishAuthKeyIfUnpublished(Context);

			// 認証キー判定。NGであれば新しいセッション発行してリダイレクト
			CheckAuthKey();
		}

		//------------------------------------------------------
		// ログイン中は重要なCookieをセキュアにする
		//------------------------------------------------------
		if (Constants.SESSIONCOOKIE_SECURE_ENABLED)
		{
			// ※ ログイン後のRestoreSession.aspxではまだIsLoggedInがfalseで返るため、ログイン用フラグを立てて処理を通す必要がある
			if (Request.IsSecureConnection
				&& (SessionSecurityManager.HasCriticalInformation(Context) || (Request[Constants.REQUEST_KEY_LOGIN_FLG] == "1")))
			{
				// セッションクッキーをセキュアに設定
				var hcSessionId = CookieManager.Get(Constants.SESSION_COOKIE_NAME);
				if (hcSessionId != null)
				{
					CookieManager.SetCookie(
						Constants.SESSION_COOKIE_NAME,
						Session.SessionID,	// セッション切替のタイミングでValueが取得できないことがあるので明示的にセット
						"/",	// セッションクッキーは「/」
						hcSessionId.Expires,
						true);
				}
			}
		}

		//------------------------------------------------------
		// 1クリック攻撃対策（無理矢理実行させられたくないポストバックを抑止）
		//	→セッションが切れた場合でも正常に動作する場所でなければならない
		//------------------------------------------------------
		if (!IsPostBack)
		{
			// 過去 background="#000000"（正しくはbgcolor） のような指定があり
			// 同じページのPageLoadが２回動いてしまっていたのが原因でポストバック時に検証エラーとなっていた。
			// (２回目のthis.ViewStateUserKeyがHTMLのViewstateに反映されないが、Sessionは更新されるのため。）
			// 対策としてはhtmlとしてアクセスされたもののみ this.ViewStateUserKey セットの対象とする。
			// ※非同期ポストバックはtext/htmlで来ないので判定は初期表示のみとする。
			// ※Androidは2、3ともに画像もtext/htmlが指定されるため完全にカバーできない。
			if ((Request.AcceptTypes != null)
				&& Array.Exists(Request.AcceptTypes, s => (s.StartsWith("text/html") || (s.StartsWith("application/xhtml+xml")))))
			{
				var viewStateUserKey = Session.SessionID;
				this.ViewStateUserKey = viewStateUserKey;
				Session["ViewStateUserKey"] = viewStateUserKey;
			}
		}
		else
		{
			this.ViewStateUserKey = (string)Session["ViewStateUserKey"];
		}

		//------------------------------------------------------
		// 広告コードログ登録（ Main() でオートログインするため、ユーザ情報更新のために事前に広告コード取得）
		//------------------------------------------------------
		if (!IsPostBack)
		{
			if ((Constants.W2MP_AFFILIATE_OPTION_ENABLED) && (Request[Constants.REQUEST_KEY_ADVCODE] != null))
			{
				string strAdvCode = (string)Request[Constants.REQUEST_KEY_ADVCODE];
				if (AdvCodeLogRegister.IsValid(Constants.W2MP_DEPT_ID, strAdvCode))
				{
					// 広告コードセッション格納
					Session[Constants.SESSION_KEY_ADVCODE_NOW] = strAdvCode;

					// 広告コード登録
					AdvCodeLogRegister.Regist(Constants.W2MP_DEPT_ID, strAdvCode, "00pc", UserCookieManager.UniqueUserId);
				}
			}
		}

		//------------------------------------------------------
		// 広告パラメータCookie保存
		//------------------------------------------------------
		if (!IsPostBack)
		{
			// クエリストリング内の有効な広告パラメータを取得
			var validAdvParamName = AffiliateCookieManager.GetFirstValidAdvParamName(Request);
			var advValues = (string.IsNullOrEmpty(validAdvParamName) == false) ? Request.QueryString.GetValues(validAdvParamName) : new[] { string.Empty };
			var validAdvValue = (advValues != null)
				? advValues.FirstOrDefault(value => (string.IsNullOrEmpty(value) == false)) ?? string.Empty
				: string.Empty;

			if ((string.IsNullOrEmpty(validAdvValue) == false)
				&& (string.IsNullOrEmpty((string)Session[Constants.SESSION_KEY_ADVCODE_NOW]) == false))
			{
				// Cookie作成
				AffiliateCookieManager.CreateCookie(validAdvParamName, validAdvValue);
				// 有効な広告パラメータキー格納
				Session[Constants.SESSION_KEY_ADV_PARAMETER_NAME] = validAdvParamName;
			}
		}

		//------------------------------------------------------
		// ブランド情報設定
		//------------------------------------------------------
		SetBrandInfo();

		//グローバル対応:IP範囲による国リージョン振り分け
		RegionManager.GetInstance().CountryAllcationByIpAddressIpv4(this.LoginUserId, false);
		UrlRegionParamManager.ChangeRegionByUrlParams(this.LoginUserId);
	}

	/// <summary>
	/// 認証キーチェック（NGであれば新しいセッションを発行し、同じページにリダイレクト）
	/// </summary>
	public void CheckAuthKey()
	{
		if (SessionSecurityManager.HasCorrectAuthKey(Context) == false)
		{
			FileLogger.Write(
				"security",
				string.Format(
					"認証キー不一致[{0}][{1}] [{2}<>{3}]",
					Request.UserHostAddress,
					Request.UserAgent,
					Session[Constants.SESSION_KEY_AUTH_KEY_FOR_SECURE_SESSION],
					CookieManager.GetResponseValue(Constants.COOKIE_KEY_AUTH_KEY)
					?? CookieManager.GetValue(Constants.COOKIE_KEY_AUTH_KEY) ?? ""),
				true);

			// 元のユーザーに影響を与えたくないのでクッキーのセッションID破棄（クッキー削除）
			CookieManager.RemoveCookie(Constants.SESSION_COOKIE_NAME, "/");

			// 上記だけではまだセッションが参照できてしまうのでリダイレクト
			Response.Redirect(Request.Url.PathAndQuery);
		}
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
	/// リピートプラスONEオプションがTRUEの場合、リダイレクト先に遷移する
	/// </summary>
	public void CheckRepeatPlusOneOptionEnabled()
	{
		if (Constants.REPEATPLUSONE_OPTION_ENABLED == false) return;

		var url = this.SecurePageProtocolAndHost
			+ Constants.PATH_ROOT
			+ Constants.REPEATPLUSONE_REDIRECT_PAGE;

		Response.Redirect(url);
	}

	/// <summary>
	/// パラメタに格納されたNextUrl取得
	/// </summary>
	/// <returns>NextUrlの値（パラメタ無しの場合はnull）</returns>
	public virtual string GetNextUrlForCheck()
	{
		if (Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] != null)
		{
			return (string)Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK];
		}

		return null;
	}

	#region 住所系
	/// <summary>
	/// 国が日本かどうか
	/// </summary>
	/// <param name="countryIsoCode">国ISOコード</param>
	/// <returns>日本か</returns>
	public bool IsCountryJp(string countryIsoCode)
	{
		return GlobalAddressUtil.IsCountryJp(countryIsoCode);
	}

	/// <summary>
	/// 国がアメリカかどうか
	/// </summary>
	/// <param name="countryIsoCode">国ISOコード</param>
	/// <returns>アメリカか</returns>
	public bool IsCountryUs(string countryIsoCode)
	{
		return GlobalAddressUtil.IsCountryUs(countryIsoCode);
	}

	/// <summary>
	/// 国が台湾かどうか
	/// </summary>
	/// <param name="countryIsoCode">国ISOコード</param>
	/// <returns>台湾か</returns>
	public bool IsCountryTw(string countryIsoCode)
	{
		return GlobalAddressUtil.IsCountryTw(countryIsoCode);
	}

	/// <summary>
	/// Is not country JP
	/// </summary>
	/// <param name="countryIsoCode">Country ISO code</param>
	/// <returns>True: is not country Jp, otherwise: false</returns>
	public bool IsNotCountryJp(string countryIsoCode)
	{
		return (IsCountryJp(countryIsoCode) == false);
	}

	/// <summary>
	/// 郵便番号が必須かどうか
	/// </summary>
	/// <param name="countryIsoCode">国ISOコード</param>
	/// <returns>必須か</returns>
	public bool IsAddrZipcodeNecessary(string countryIsoCode)
	{
		return GlobalAddressUtil.IsAddrZipcodeNecessary(countryIsoCode);
	}

	/// <summary>
	/// 国情報をコントロールのCSSクラス情報に付与
	/// </summary>
	/// <param name="inputControls">入力対象コントロールリスト</param>
	/// <param name="countryIsoCode">国ISOコード</param>
	public void AddCountryInfoToControlForChangeCountry(WrappedWebControl[] inputControls, string countryIsoCode)
	{
		foreach (var inputControl in inputControls)
		{
			inputControl.CssClass = Regex.Replace(inputControl.CssClass, string.Format(" {0}:.*? ", Constants.CLIENTVALIDATE_CSS_HEAD_LANGUAGE_LOCALE_ID), " ");
			inputControl.CssClass = Regex.Replace(inputControl.CssClass, string.Format(" {0}:.*? ", Constants.CLIENTVALIDATE_CSS_HEAD_COUNTRY_ISO_CODE), " ");
			inputControl.CssClass += string.Format(" {0}:{1} ", Constants.CLIENTVALIDATE_CSS_HEAD_LANGUAGE_LOCALE_ID, Constants.GLOBAL_OPTION_ENABLE ? RegionManager.GetInstance().Region.LanguageLocaleId : "");
			inputControl.CssClass += string.Format(" {0}:{1} ", Constants.CLIENTVALIDATE_CSS_HEAD_COUNTRY_ISO_CODE, countryIsoCode);
		}
	}

	/// <summary>
	/// 国情報によりCustomValidatorのValidationGroupをセット
	/// </summary>
	/// <param name="targetControls">対象コントロールリスト</param>
	/// <param name="validationGroup">設定するバリデーショングループ</param>
	public void ChangeValidationGroupForChangeCountry(
		WrappedControl[] targetControls,
		string validationGroup)
	{
		foreach (var targetControl in targetControls)
		{
			if (targetControl is WrappedCustomValidator)
			{
				((WrappedCustomValidator)targetControl).ValidationGroup = validationGroup;
			}
			else if (targetControl is WrappedLinkButton)
			{
				((WrappedLinkButton)targetControl).ValidationGroup = validationGroup;
			}
		}
	}

	/// <summary>
	/// 郵便番号検索
	/// </summary>
	/// <param name="senderControl">郵便番号検索の発生元コントロール（郵便番号テキストボックスor検索ボタン）</param>
	/// <param name="unavailableShippingZip">配送不可エリア郵便番号</param>
	/// <param name="isFocus">住所3テキストボックスにフォーカスするか</param>
	/// <returns>エラーメッセージ</returns>
	public string SearchZipCode(object senderControl, string unavailableShippingZip = "", bool isFocus = true)
	{
		// 郵便番号検索対象のコントロールを取得する
		WrappedTextBox wtbZip;
		WrappedTextBox wtbZip1;
		WrappedTextBox wtbZip2;
		WrappedDropDownList wddlAddr1;
		WrappedTextBox wtbAddr2;
		WrappedTextBox wtbAddr3;
		GetWrappedZipCodeControlName(
			senderControl,
			out wtbZip,
			out wtbZip1,
			out wtbZip2,
			out wddlAddr1,
			out wtbAddr2,
			out wtbAddr3
		);

		// 郵便番号入力チェック
		var errorMessages = wtbZip2.HasInnerControl
			? Validator.CheckZipCode(wtbZip1.Text.Trim(), wtbZip2.Text.Trim())
			: Validator.CheckZipCode(wtbZip.Text.Trim());

		if (errorMessages.Any()) return errorMessages;

		if (isFocus)
		{
			// 郵便番号検索の発生元が検索ボタンまたはテキストボックスの場合、住所3テキストボックスにフォーカスする
			if ((senderControl is LinkButton) || (senderControl is TextBox)) wtbAddr3.Focus();
		}

		// 入力チェックOKの場合、郵便番号検索実行&セット
		errorMessages += SearchAddrFromZip(wtbZip1, wtbZip2, wddlAddr1, wtbAddr2, wtbZip);

		if (string.IsNullOrEmpty(unavailableShippingZip)) return errorMessages;

		var inputZip = wtbZip2.HasInnerControl
			? wtbZip1.Text.Trim() + wtbZip2.Text.Trim()
			: wtbZip.Text.Trim().Replace("-", "");

		// 配送不可エリアかチェックしてエラーメッセージをセット
		if (OrderCommon.CheckUnavailableShippingArea(unavailableShippingZip, inputZip))
		{
			errorMessages += WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ZIPCODE_UNAVAILABLE_SHIPPING_AREA);
		}

		return errorMessages;
	}

	/// <summary>
	/// 郵便番号検索対象のコントロールプロパティを設定する
	/// </summary>
	/// <param name="senderControl">郵便番号検索の発生元コントロール</param>
	/// <param name="wtbZip">Wrapped textbox zip</param>
	/// <param name="wtbZip1">ラップ済みコントロール（郵便番号1）</param>
	/// <param name="wtbZip2">ラップ済みコントロール（郵便番号2）</param>
	/// <param name="wddlAddr1">ラップ済みコントロール（住所1）</param>
	/// <param name="wtbAddr2">ラップ済みコントロール（住所2）</param>
	private void GetWrappedZipCodeControlName(
		object senderControl,
		out WrappedTextBox wtbZip,
		out WrappedTextBox wtbZip1,
		out WrappedTextBox wtbZip2,
		out WrappedDropDownList wddlAddr1,
		out WrappedTextBox wtbAddr2,
		out WrappedTextBox wtbAddr3
	)
	{
		var control = (Control)senderControl;
		wtbZip = GetWrappedControl<WrappedTextBox>(control.Parent, "tbUserZip");
		wtbZip1 = GetWrappedControl<WrappedTextBox>(control.Parent, "tbUserZip1");
		wtbZip2 = GetWrappedControl<WrappedTextBox>(control.Parent, "tbUserZip2");
		wddlAddr1 = GetWrappedControl<WrappedDropDownList>(control.Parent, "ddlUserAddr1");
		wtbAddr2 = GetWrappedControl<WrappedTextBox>(control.Parent, "tbUserAddr2");
		wtbAddr3 = GetWrappedControl<WrappedTextBox>(control.Parent, "tbUserAddr3");
		if ((control.ID == "tbShippingZip1")
			|| (control.ID == "tbShippingZip2")
			|| (control.ID == "lbSearchShippingAddr")
			|| (control.ID == "tbShippingZip"))
		{
			wtbZip = GetWrappedControl<WrappedTextBox>(control.Parent, "tbShippingZip");
			wtbZip1 = GetWrappedControl<WrappedTextBox>(control.Parent, "tbShippingZip1");
			wtbZip2 = GetWrappedControl<WrappedTextBox>(control.Parent, "tbShippingZip2");
			wddlAddr1 = GetWrappedControl<WrappedDropDownList>(control.Parent, "ddlShippingAddr1");
			wtbAddr2 = GetWrappedControl<WrappedTextBox>(control.Parent, "tbShippingAddr2");
			wtbAddr3 = GetWrappedControl<WrappedTextBox>(control.Parent, "tbShippingAddr3");
		}
		else if ((control.ID == "tbOwnerZip1")
			|| (control.ID == "tbOwnerZip2")
			|| (control.ID == "lbSearchOwnergAddr")
			|| (control.ID == "tbOwnerZip"))
		{
			wtbZip = GetWrappedControl<WrappedTextBox>(control.Parent, "tbOwnerZip");
			wtbZip1 = GetWrappedControl<WrappedTextBox>(control.Parent, "tbOwnerZip1");
			wtbZip2 = GetWrappedControl<WrappedTextBox>(control.Parent, "tbOwnerZip2");
			wddlAddr1 = GetWrappedControl<WrappedDropDownList>(control.Parent, "ddlOwnerAddr1");
			wtbAddr2 = GetWrappedControl<WrappedTextBox>(control.Parent, "tbOwnerAddr2");
			wtbAddr3 = GetWrappedControl<WrappedTextBox>(control.Parent, "tbOwnerAddr3");
		}
		else if ((control.ID == "tbSenderZip1")
			|| (control.ID == "tbSenderZip2")
			|| (control.ID == "lbSearchSenderAddr")
			|| (control.ID == "tbSenderZip"))
		{
			wtbZip = GetWrappedControl<WrappedTextBox>(control.Parent, "tbSenderZip");
			wtbZip1 = GetWrappedControl<WrappedTextBox>(control.Parent, "tbSenderZip1");
			wtbZip2 = GetWrappedControl<WrappedTextBox>(control.Parent, "tbSenderZip2");
			wddlAddr1 = GetWrappedControl<WrappedDropDownList>(control.Parent, "ddlSenderAddr1");
			wtbAddr2 = GetWrappedControl<WrappedTextBox>(control.Parent, "tbSenderAddr2");
			wtbAddr3 = GetWrappedControl<WrappedTextBox>(control.Parent, "tbSenderAddr3");
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
	public string SearchAddrFromZip(
		WrappedTextBox wtbZip1,
		WrappedTextBox wtbZip2,
		WrappedDropDownList wddlAddr1,
		WrappedTextBox wtbAddr2,
		WrappedTextBox wtbZip)
	{
		var zipcode = wtbZip1.HasInnerControl
			? StringUtility.ToHankaku(wtbZip1.Text.Trim() + wtbZip2.Text.Trim())
			: StringUtility.ToHankaku(wtbZip.Text.Trim());
		var addrs = new ZipcodeService().GetByZipcode(StringUtility.ReplaceDelimiter(zipcode));

		if (addrs.Length == 0)
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ZIPCODE_NO_ADDR);
		}
		else if (addrs.Length == 1)
		{
			wddlAddr1.Text = addrs[0].Prefecture;
			wtbAddr2.Text = addrs[0].City + addrs[0].Town;
		}
		// 郵便番号によって複数の選択肢がある場合、画面側で選択しているため、改めて設定しない
		return "";
	}

	/// <summary>
	/// 郵便番号検索のエラーメッセージを削除する
	/// </summary>
	/// <param name="giftOrderOptionEnabled">ギフト購入オプション可否フラグ</param>
	/// <param name="repeaterCartList">リピーターカートリスト</param>
	/// <param name="lpPageFlg">LPページであるかの判定</param>
	public void ResetAddressSearchResultErrorMessage(
		bool giftOrderOptionEnabled,
		WrappedRepeater repeaterCartList,
		bool lpPageFlg = false)
	{
		ResetAddressSearchResultErrorMessage(giftOrderOptionEnabled, repeaterCartList.InnerControl, lpPageFlg);
	}
	/// <summary>
	/// 郵便番号検索のエラーメッセージを削除する
	/// </summary>
	/// <param name="giftOrderOptionEnabled">ギフト購入オプション可否フラグ</param>
	/// <param name="repeaterCartList">リピーターカートリスト</param>
	/// <param name="lpPageFlg">LPページであるかの判定</param>
	public void ResetAddressSearchResultErrorMessage(bool giftOrderOptionEnabled, Repeater repeaterCartList, bool lpPageFlg = false)
	{
		if (giftOrderOptionEnabled && (lpPageFlg == false))
		{
			foreach (RepeaterItem ri in repeaterCartList.Items)
			{
				var repeaterCartShipping = ((Repeater)ri.FindControl("rCartShippings"));
				if (repeaterCartShipping != null)
				{
					foreach (RepeaterItem riShipping in repeaterCartShipping.Items)
					{
						var senderZipErrorControl = riShipping.FindControl("sSenderZipError");
						var shippingZipErrorControl = riShipping.FindControl("sShippingZipError");
						if (senderZipErrorControl != null)
						{
							GetWrappedControl<WrappedHtmlGenericControl>(senderZipErrorControl.Parent, "sSenderZipError").InnerText = "";
						}
						if (shippingZipErrorControl != null)
						{
							GetWrappedControl<WrappedHtmlGenericControl>(shippingZipErrorControl.Parent, "sShippingZipError").InnerText = "";
						}
					}
				}
			}
		}
		else
		{
			foreach (RepeaterItem ri in repeaterCartList.Items)
			{
				var ownerZipErrorControl = ri.FindControl("sOwnerZipError");
				var shippingZipErrorControl = ri.FindControl("sShippingZipError");
				if (ownerZipErrorControl != null)
				{
					GetWrappedControl<WrappedHtmlGenericControl>(ownerZipErrorControl.Parent, "sOwnerZipError").InnerText = "";
				}
				if (shippingZipErrorControl != null)
				{
					GetWrappedControl<WrappedHtmlGenericControl>(shippingZipErrorControl.Parent, "sShippingZipError").InnerText = "";
				}
			}
		}
	}

	/// <summary>
	/// Get the text box wrapped from the repeater
	/// </summary>
	/// <param name="item">Repeater item</param>
	/// <param name="idTextBox">The id of the text box</param>
	/// <returns>Wrapped text box by id</returns>
	public WrappedTextBox GetWrappedTextBoxFromRepeater(RepeaterItem item, string idTextBox)
	{
		var wTextBox = GetWrappedControl<WrappedTextBox>(item, idTextBox);
		return wTextBox;
	}

	/// <summary>
	/// Get the link button wrapped from the repeater
	/// </summary>
	/// <param name="item">Repeater item</param>
	/// <param name="idLinkButton">The id of the link button</param>
	/// <returns>Wrapped link button by id</returns>
	public WrappedLinkButton GetWrappedLinkButtonFromRepeater(RepeaterItem item, string idLinkButton)
	{
		var wLinkButton = GetWrappedControl<WrappedLinkButton>(item, idLinkButton);
		return wLinkButton;
	}

	/// <summary>
	/// Set Zip Code Textbox
	/// </summary>
	/// <param name="tbZip">Textbox zip</param>
	/// <param name="tbZip1">Textbox zip 1</param>
	/// <param name="tbZip2">Textbox zip 2</param>
	/// <param name="value">Value of zip</param>
	public void SetZipCodeTextbox(
		WrappedTextBox tbZip,
		WrappedTextBox tbZip1,
		WrappedTextBox tbZip2,
		string value)
	{
		var zipCode = new ZipCode(StringUtility.ToHankaku(value));
		if (tbZip1.HasInnerControl)
		{
			tbZip1.Text = zipCode.Zip1;
			tbZip2.Text = zipCode.Zip2;
			return;
		}

		tbZip.Text = zipCode.Zip;
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
		var tel = new Tel(value);
		if (tbTel1.HasInnerControl)
		{
			tbTel1.Text = tel.Tel1;
			tbTel2.Text = tel.Tel2;
			tbTel3.Text = tel.Tel3;
			return;
		}

		tbTel.Text = tel.TelNo;
	}
	#endregion

	/// <summary>
	/// 入力フォームからトークン情報削除（再入力ボタンで利用）
	/// </summary>
	/// <param name="riPayment">決済リピータアイテム</param>
	public void ResetCreditTokenInfoFromForm(RepeaterItem riPayment)
	{
		ResetCreditTokenInfoFromForm(riPayment, Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING);
	}

	/// <summary>
	/// 該当ユーザーは該当商品に定期購入制限されるかをチェック
	/// </summary>
	/// <param name="shopId">ショップId</param>
	/// <param name="productId">商品Id</param>
	/// <returns>制限されているかどうか</returns>
	public bool CheckFixedPurchaseLimitedUserLevel(string shopId, string productId)
	{
		if (string.IsNullOrEmpty(this.UserManagementLevelId)) return false;

		var fixedPurchaseLimitedUserLevels = ProductCommon.GetProductInfoUnuseMemberRankPrice(shopId, productId);

		var fixedPurchaseAbleUserLevel = fixedPurchaseLimitedUserLevels.Cast<DataRowView>().Select(drv => drv[Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_USER_LEVEL_IDS].ToString().Split(',')).FirstOrDefault();
		var result = (fixedPurchaseAbleUserLevel == null) ? false : fixedPurchaseAbleUserLevel.Contains(this.UserManagementLevelId);

		return result;
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
		return DataCacheControllerFacade.GetSubscriptionBoxCacheController().GetSubscriptionBoxesByProductId(shopId, productId, variationId);
	}

	/// <summary>
	/// 楽天IDConnect認証ページへ遷移
	/// </summary>
	/// <param name="type">アクション種別</param>
	public void RedirectRakutenIdConnect(ActionType type)
	{
		RedirectRakutenIdConnect(
			type,
			SessionManager.RakutenIdConnectActionInfo.BeforeUrl,
			SessionManager.RakutenIdConnectActionInfo.NextUrl,
			SessionManager.RakutenIdConnectActionInfo.IsLandingCart);
	}
	/// <summary>
	/// 楽天IDConnect認証ページへ遷移
	/// </summary>
	/// <param name="type">アクション種別</param>
	/// <param name="beforeUrl">遷移元Url</param>
	/// <param name="nextUrl">遷移先Url</param>
	/// <param name="isLandingCart">ランディングカートか</param>
	public void RedirectRakutenIdConnect(ActionType type, string beforeUrl, string nextUrl, bool isLandingCart = false)
	{
		SessionManager.RakutenIdConnectActionInfo =
			new RakutenIDConnectActionInfo(type, beforeUrl, nextUrl, isLandingCart);

		var url = RakutenIDConnectUrlCreator.CreateUrlCreatorForAuth(
			type,
			SessionManager.RakutenIdConnectActionInfo.State,
			SessionManager.RakutenIdConnectActionInfo.Nonce).CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// ペイパル認証情報をセッションにセット
	/// </summary>
	/// <param name="wucPaypalScriptsForm">WrappedPayPalPayScriptsFormControl</param>
	public void SetPaypalInfoToSession(WrappedPayPalPayScriptsFormControl wucPaypalScriptsForm)
	{
		SessionManager.PayPalLoginResult = new PayPalLoginResult(
			wucPaypalScriptsForm.PayPalPayerId,
			wucPaypalScriptsForm.PayPalNonce,
			wucPaypalScriptsForm.PayPalDeviceData,
			wucPaypalScriptsForm.PayPalShippingAddress);
		SessionManager.PayPalCooperationInfo = new PayPalCooperationInfo(SessionManager.PayPalLoginResult);
	}

	/// <summary>
	/// ログイン成功処理実行＆次の画面へ遷移（会員登録向け）
	/// </summary>
	/// <param name="user">ユーザー</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	public void ExecLoginSuccessProcessAndGoNextForUserRegister(
		UserModel user,
		UpdateHistoryAction updateHistoryAction)
	{
		// 通常は会員登録完了ページ
		// かんたん会員登録 & 遷移先URLが3分岐の場合は3分岐へ遷移（その後に注文配送先に遷移）
		var nextUrl =
			StringUtility.ToEmpty(Session[Constants.SESSION_KEY_NEXT_URL])
				.Contains(Constants.PAGE_FRONT_ORDER_OWNER_DECISION)
				? Constants.PAGE_FRONT_ORDER_OWNER_DECISION
				: Constants.PAGE_FRONT_USER_REGIST_COMPLETE;
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = nextUrl;

		// ログインIDをCookieから削除 ※クッキーのログインIDが他者の可能性があるため
		UserCookieManager.CreateCookieForLoginId("", false);

		// ログイン成功アクション実行
		ExecLoginSuccessActionAndGoNextInner(user, Constants.PATH_ROOT + nextUrl, updateHistoryAction);
	}

	/// <summary>
	/// ログイン成功処理実行＆次の画面へ遷移（ログイン向け）
	/// </summary>
	/// <param name="user">ユーザー</param>
	/// <param name="defaultNextUrl">デフォルト遷移先URL</param>
	/// <param name="saveAutoCompleteLoginId">オートコンプリートのログインIDを保存するか</param>
	/// <param name="loginType">ログインタイプ</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	public void ExecLoginSuccessProcessAndGoNextForLogin(
		UserModel user,
		string defaultNextUrl,
		bool saveAutoCompleteLoginId,
		BasePage.LoginType loginType,
		UpdateHistoryAction updateHistoryAction)
	{
		// 楽天IDConnect会員登録ユーザーの場合は自社ログインは利用できないためエラーとする
		if (Constants.RAKUTEN_LOGIN_ENABLED)
		{
			if ((loginType != BasePage.LoginType.RakutenConnect)
				&& (user.UserExtend.UserExtendDataValue[Constants.RAKUTEN_ID_CONNECT_REGISTER_USER]
					== Constants.FLG_USER_RAKUTEN_ID_CONNECT_REGISTER_USER_ON))
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(
					Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED
						? WebMessages.ERRMSG_FRONT_USER_LOGIN_IN_MAILADDR_ERROR
						: WebMessages.ERRMSG_FRONT_USER_LOGIN_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			SessionManager.IsRakutenIdConnectLoggedIn = (loginType == BasePage.LoginType.RakutenConnect);
		}

		// ログインIDのSecureCookie作成に関する処理
		UserCookieManager.CreateCookieForLoginId(user.LoginId, saveAutoCompleteLoginId);

		// ログイン成功アクション
		ExecLoginSuccessActionAndGoNextInner(user, defaultNextUrl, updateHistoryAction);
	}

	/// <summary>
	/// ログイン成功処理実行＆次の画面へ遷移（ログイン向け）
	/// </summary>
	/// <param name="user">ユーザー</param>
	/// <param name="socialLogin">ソーシャルログイン情報</param>
	/// <param name="defaultNextUrl">デフォルト遷移先URL</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	public void ExecLoginSuccessProcessAndGoNextForSocialPlusLogin(
		UserModel user,
		SocialLoginModel socialLogin,
		string defaultNextUrl,
		UpdateHistoryAction updateHistoryAction)
	{
		// ログインのたびに同期をとる
		SocialLoginUtil.SyncSocialProviderInfo(socialLogin.SPlusUserId, socialLogin.W2UserId, user.UserExtend);

		ExecLoginSuccessActionAndGoNextInner(user, defaultNextUrl, updateHistoryAction);
	}

	/// <summary>
	/// ログイン成功アクション実行
	/// </summary>
	/// <param name="user">ユーザー</param>
	/// <param name="nextUrl">遷移先URL</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	public void ExecLoginSuccessActionAndGoNextInner(UserModel user, string nextUrl, UpdateHistoryAction updateHistoryAction)
	{
		// プラグイン：ユーザーログインイベント
		if (Constants.USER_COOPERATION_ENABLED)
		{
			var userCooperationPlugin = new UserCooperationPlugin(Constants.FLG_LASTCHANGED_USER);
			userCooperationPlugin.Login(user);

			var viewStateNextUrl = StringUtility.ToEmpty(this.ViewState["NextUrl"]);
			if (string.IsNullOrEmpty(viewStateNextUrl) == false) { nextUrl = viewStateNextUrl; }
		}

		// ログイン情報をセッションに格納
		SetLoginUserData(user, UpdateHistoryAction.DoNotInsert);
		if (Constants.RAKUTEN_LOGIN_ENABLED)
		{
			var openId = user.UserExtend.UserExtendDataValue[Constants.RAKUTEN_ID_CONNECT_OPEN_ID];
			SessionManager.LoginUserRakutenOpenId = StringUtility.ToEmpty(openId);

			var registerFlg = user.UserExtend.UserExtendDataValue[Constants.RAKUTEN_ID_CONNECT_REGISTER_USER];
			SessionManager.IsRakutenIdConnectRegisterUser =
				(registerFlg == Constants.FLG_USER_RAKUTEN_ID_CONNECT_REGISTER_USER_ON);
		}

		// アクセスログ用にログイン時ステータスを格納
		// AccessLogTrackerScript.ascx.cs内でステータスを参照し、ログイン時用のスクリプトを出力している
		// スクリプト出力と同時にSession[Constants.SESSION_KEY_W2MP_ACCESSLOG_STATUS]にnullを格納
		Session[Constants.SESSION_KEY_W2MP_ACCESSLOG_STATUS] = Constants.KBN_W2MP_ACCESSLOG_STATUS_LOGIN;

		var errorPointApi = string.Empty;
		PointOptionUtility.GiveEachLoginPoint(
			this.LoginUserId,
			Constants.FLG_LASTCHANGED_USER,
			ref errorPointApi,
			UpdateHistoryAction.DoNotInsert);

		if (Constants.CROSS_POINT_OPTION_ENABLED)
		{
			// Get info and update user
			UserUtility.AdjustPointAndMemberRankByCrossPointApi(user);
		}

		if (string.IsNullOrEmpty(errorPointApi) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorPointApi;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		// ログイン日時更新（履歴は落とさない）
		var userService = new UserService();
		userService.UpdateLoginDate(user.UserId, Constants.FLG_LASTCHANGED_USER, UpdateHistoryAction.DoNotInsert);

		// ログイン時のレコメンド用処理（履歴は落とさない）
		RecommendWithLogin(user, UpdateHistoryAction.DoNotInsert);

		// ユーザ広告コード更新＆取得（アフィリエイトOP）
		if (Constants.W2MP_AFFILIATE_OPTION_ENABLED)
		{
			var advCodeNow = (string)Session[Constants.SESSION_KEY_ADVCODE_NOW];

			if (string.IsNullOrEmpty(user.AdvcodeFirst) && (string.IsNullOrEmpty(advCodeNow) == false))
			{
				userService.UpdateAdvCodeFirst(
					user.UserId,
					(string)Session[Constants.SESSION_KEY_ADVCODE_NOW],
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.DoNotInsert);
			}

			// セッションに格納
			Session[Constants.SESSION_KEY_ADVCODE_FIRST] = GetAdvCodeFirst(advCodeNow, user.AdvcodeFirst);
		}

		// ユーザ情報を最新の状態に更新
		this.LoginUser = userService.Get(user.UserId);

		this.LoginUserPoint = PointOptionUtility.GetUserPoint(this.LoginUserId);

		// カート切り替え処理（切り替えを行う場合、遷移先URLはカート選択画面になる)
		var changedNextUrl = CartKirikae(nextUrl, user.MemberRankId);

		// ノベルティOP有効?
		if (Constants.NOVELTY_OPTION_ENABLED)
		{
			// 遷移先がカート選択ではないかつ、
			// 3分岐画面からのログインか、遷移先が3分岐画面の場合、
			// ログイン済 AND 3分岐画面の場合、配送先入力画面にリダイレクトされる
			// そのため、付与できるノベルティが存在する場合、カート画面へ遷移
			var orderOwnerDecisionUrl = (Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_OWNER_DECISION);
			var cartSelect = Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_SELECT;
			if ((changedNextUrl.Split('?')[0] != cartSelect)
				&& ((nextUrl.Split('?')[0] == orderOwnerDecisionUrl)
				|| (changedNextUrl.Split('?')[0] == orderOwnerDecisionUrl)))
			{
				// カートリスト取得
				var cartList = this.GetCartObjectList();
				// カートノベルティリスト作成
				var cartNoveltyList = new CartNoveltyList(cartList);
				// カートに追加された付与アイテムを含むカートノベルティを削除
				cartNoveltyList.RemoveCartNovelty(cartList);
				// 付与アイテムが存在する場合、遷移先をカートリストへ
				if (cartNoveltyList.ExistsCartNoveltyGrantItem())
				{
					changedNextUrl = Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST;
				}
			}
		}

		LPCartKirikae();

		// 通常カートでAmazonアカウントで新規登録している場合、注文者情報をnullに置換、
		// 再度カート画面にアクセスした際に再構築
		if (SessionManager.IsAmazonPayRegisterForOrder) SessionManager.CartList.Owner = null;

		// CookieからカートID削除 ※カート切り替え処理後なのでCookie保存は不要
		CartCookieManager.RemoveCartCookie();

		// Userのグローバル情報を更新
		RegionManager.GetInstance().UpdateUserRegion(new RegionModel(), this.LoginUserId);

		// 更新履歴登録
		if (updateHistoryAction == UpdateHistoryAction.Insert)
		{
			new UpdateHistoryService().InsertForUser(this.LoginUserId, Constants.FLG_LASTCHANGED_USER);
		}

		GetNewSessionAndGoNextUrl(changedNextUrl);
	}

	/// <summary>
	/// ログイン拒否エラー文言取得
	/// </summary>
	/// <param name="loginId">ログインID</param>
	/// <param name="password">パスワード</param>
	/// <param name="isCooperate">アカウント連携時のチェックか</param>
	/// <returns>エラー文言</returns>
	public string GetLoginDeniedErrorMessage(string loginId, string password, bool isCooperate = false)
	{
		// ログイン試行回数を更新
		LoginAccountLockManager.GetInstance().UpdateLockPossibleTrialLoginCount(Request.UserHostAddress, loginId, password);

		// ログイン試行可能回数を超えていた場合はログイン試行可能回数エラーのメッセージを設定
		if (LoginAccountLockManager.GetInstance().IsAccountLocked(Request.UserHostAddress, loginId, password))
		{
			return WebMessages.GetMessages(
				isCooperate == false
					? WebMessages.ERRMSG_FRONT_W2_EXISTING_USER_COOPERATION_ACCOUNT_LOCK
					: WebMessages.ERRMSG_FRONT_USER_LOGIN_ACCOUNT_LOCK);
		}

		// アカウント連携時は専用メッセージを設定
		if (isCooperate)
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_W2_EXISTING_USER_COOPERATE_NOT_FOUND);
		}

		// アカウント連携時以外はログイン失敗のメッセージを設定
		return WebMessages.GetMessages(
			Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED
				? WebMessages.ERRMSG_FRONT_USER_LOGIN_IN_MAILADDR_ERROR
				: WebMessages.ERRMSG_FRONT_USER_LOGIN_ERROR);
	}

	/// <summary>
	/// ブランド情報の設定
	/// </summary>
	public void SetBrandInfo()
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

	#region コントロール検索系
	/// <summary>
	/// 外部コントロール取得（再帰メソッド）
	/// </summary>
	/// <param name="control">検索の基準になるコントロール</param>
	/// <param name="type">検索するコントロールの型</param>
	/// <returns></returns>
	public dynamic GetOuterControl(Control control, Type type)
	{
		if (control.Parent == null) return null;

		if (control.Parent.GetType() == type)
		{
			return control.Parent;
		}
		else
		{
			return GetOuterControl(control.Parent, type);
		}
	}

	/// <summary>
	/// 親リピーターアイテムを取得
	/// </summary>
	/// <param name="control">検索の基準になるコントロール</param>
	/// <param name="repeaterControlId">検索するリピーターのID</param>
	/// <returns>検索するリピーターアイテム</returns>
	public RepeaterItem GetParentRepeaterItem(Control control, string repeaterControlId)
	{
		if (control.Parent == null) return null;

		if ((control is RepeaterItem) && (control.Parent is Repeater) && (control.Parent.ID == repeaterControlId))
		{
			return (RepeaterItem)control;
		}
		else
		{
			return GetParentRepeaterItem(control.Parent, repeaterControlId);
		}
	}
	#endregion

	/// <summary>
	/// ListItemCollectionの中に値があればそれを返す（無ければnullを返す）　※DataBindなどで利用
	/// </summary>
	/// <param name="collection">ListItemCollecyion</param>
	/// <param name="value">値</param>
	/// <returns>ListItemCollectionの該当の値</returns>
	public string GetListItemValue(ListItemCollection collection, string value)
	{
		return collection.Cast<ListItem>().Any(li => li.Value == value) ? value : null;
	}

	/// <summary>
	/// カートオブジェクト取得
	/// </summary>
	/// <returns></returns>
	public CartObjectList GetCartObjectList()
	{
		return SessionSecurityManager.GetCartObjectList(Context, this.IsPc ? Constants.FLG_ORDER_ORDER_KBN_PC : Constants.FLG_ORDER_ORDER_KBN_SMARTPHONE);
	}

	/// <summary>
	/// 初回広告コード取得
	/// </summary>
	/// <param name="advCodeNow">現在の広告コード</param>
	/// <param name="advCodeFirst">初回広告コード</param>
	/// <returns>初回広告コード</returns>
	protected string GetAdvCodeFirst(string advCodeNow, string advCodeFirst)
	{
		// 初回広告コードがあれば
		if (string.IsNullOrEmpty(advCodeFirst) == false) return advCodeFirst;

		return StringUtility.ToEmpty(advCodeNow);
	}

	/// <summary>
	/// カート切り替え処理
	/// 切り替え時にメンバーランクIDが必要
	/// </summary>
	/// <param name="nextUrl">次画面URL</param>
	/// <returns>次画面URL</returns>
	private string CartKirikae(string nextUrl, string memberRankId)
	{
		//------------------------------------------------------
		// カート切替処理
		//------------------------------------------------------
		// ユーザカートリスト情報取得
		CartObjectList userCartList = CartObjectList.GetUserCartList(this.LoginUserId, this.IsPc ? Constants.FLG_ORDER_ORDER_KBN_PC : Constants.FLG_ORDER_ORDER_KBN_SMARTPHONE);
		userCartList.SetFixedPurchaseMemberFlgForCartObject(this.LoginUserFixedPurchaseMemberFlg);
		userCartList.CartListShippingMethodUserUnSelected();
		CartObjectList guestCartList = GetCartObjectList();

		// ゲストカート情報の有無をチェック
		if (IsExistCart(guestCartList))
		{
			// ユーザカートリスト情報あり
			if (IsExistCart(userCartList))
			{
				// ゲストカート、ユーザカートがある場合
				// そしてそれぞれの商品の一覧画面を出し、
				// どの商品を最終的にカートへ入れるかチェックボックスで選択させる
				// （ゲストカート商品にはデフォルトチェックあり）

				// ユーザカートのユーザID紐付けを外す
				userCartList.UpdateUserIdToCartDb("");

				// ゲストカートにユーザID,メンバーランクIDを紐づける
				guestCartList.UpdateUserIdToCartDb(this.LoginUserId);
				userCartList.MemberRankId = memberRankId;

				// 遷移先URL、紐付けがはずれたカート情報をパラメタに設定
				Hashtable htParam = new Hashtable();
				htParam.Add("cart", userCartList);
				Session[Constants.SESSION_KEY_PARAM] = htParam;

				// カート選択ページへ遷移
				Session[Constants.SESSION_KEY_NEXT_URL] = this.NextUrlValidation(nextUrl);	// 元々の次URLをセッションへ保存
				nextUrl = Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_SELECT;	// 次URLはカート選択画面
			}
			else
			{
				// ゲストカートリストありでユーザカートがない場合、ユーザIDを紐付け
				guestCartList.UpdateUserIdToCartDb(this.LoginUserId);
				guestCartList.SetFixedPurchaseMemberFlgForCartObject(this.LoginUserFixedPurchaseMemberFlg);
				guestCartList.MemberRankId = memberRankId;
			}
		}
		else
		{
			// ゲストカートリストがない場合、ユーザカートをセッションにセット
			// （存在しない場合は空のカートリスト作成）
			Session[Constants.SESSION_KEY_CART_LIST] = userCartList;
		}
		return nextUrl;
	}

	/// <summary>
	/// ゲストカート情報存在チェック
	/// </summary>
	/// <returns>存在フラグ</returns>
	protected bool IsExistCart(CartObjectList cartList)
	{
		if (cartList == null) return false;

		if (cartList.Items.Count == 0) return false;

		return true;
	}

	/// <summary>
	/// LPカート切り替え処理
	/// </summary>
	private void LPCartKirikae()
	{
		//------------------------------------------------------
		// LP用カート切り替え処理
		//------------------------------------------------------
		List<string> lLandingCartSessionKeys = new List<string>();

		// セッションキーからLP用カートのものを全て抜き出す
		for (int iLoop = 0; iLoop < Session.Count; iLoop++)
		{
			if (Session.Keys[iLoop].StartsWith(Constants.SESSION_KEY_CART_LIST_LANDING))
			{
				lLandingCartSessionKeys.Add(Session.Keys[iLoop]);
			}
		}

		this.IsLandingPage = lLandingCartSessionKeys.Any();

		// LP用カートの更新を行う
		foreach (string strLandingCartSessionKey in lLandingCartSessionKeys)
		{
			// セッションからカートオブジェクトリストを取得
			var col = (CartObjectList)Session[strLandingCartSessionKey];
			// カートオブジェクが存在しない場合はスキップ
			if (col == null) break;

			if (col != null)
			{
				// 注文者情報をnullに置換
				// LPカートの注文者情報は再度LPカート画面にアクセスした際に再構築
				if (this.RegisterUser == null) col.Owner = null;

				col.Owner = this.RegisterUser == null ? null : col.Owner;
				// 会員ランクIDを更新
				col.MemberRankId = this.MemberRankId;

				Session[strLandingCartSessionKey] = col;
			}
		}
	}

	/// <summary>
	/// セッション張り直し後、次画面遷移（セッションハイジャック対策）
	/// </summary>
	/// <param name="nextUrl">次画面のURL</param>
	private void GetNewSessionAndGoNextUrl(string nextUrl)
	{
		// セッション張り直しのためのデータ格納
		SessionSecurityManager.SaveSesstionContetnsToDatabaseForChangeSessionId(Request, Response, Session);

		// 次画面へ遷移（遷移先をパラメータで渡す）
		nextUrl = nextUrl.Replace("#CartList", string.Empty);
		var url = new UrlCreator(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_RESTORE_SESSION)
				.AddParam(Constants.REQUEST_KEY_NEXT_URL, nextUrl)
				.AddParam(Constants.REQUEST_KEY_LOGIN_FLG, "1")
				.CreateUrl();
		url = this.IsLandingPage ? string.Format("{0}#CartList", url) : url;
		this.IsLandingPage = false;
		this.Response.Redirect(url);
	}

	/// <summary>
	///  ログイン時のレコメンド処理
	/// </summary>
	/// <param name="user">ユーザー情報</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	private void RecommendWithLogin(UserModel user, UpdateHistoryAction updateHistoryAction)
	{
		string recommendUserId = user.RecommendUid;

		// CookieのユーザID上書き or CookieのユーザIDをDBに登録
		// DBにPCユーザ識別IDが登録されている場合
		if (StringUtility.ToEmpty(recommendUserId) != "")
		{
			UserCookieManager.UniqueUserId = recommendUserId;
		}
		// DBにPCユーザ識別IDが登録されていない場合
		else
		{
			var userService = new UserService();
			var updateUser = userService.Get(this.LoginUserId);
			if (updateUser != null)
			{
				updateUser.RecommendUid = UserCookieManager.UniqueUserId;
				updateUser.LastChanged = Constants.FLG_LASTCHANGED_USER;

				userService.UpdateWithUserExtend(updateUser, updateHistoryAction);
			}
		}
	}

	/// <summary>
	/// ログインユーザデータを格納
	/// </summary>
	/// <param name="user">ユーザ情報</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	public void SetLoginUserData(UserModel user, UpdateHistoryAction updateHistoryAction)
	{
		this.LoginUser = user;
		this.LoginUserId = user.UserId;
		// ライン直接連携されているかつデータが登録されていない場合は、自動ログイン時のユーザ名を格納する
		this.LoginUserName = (w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED
			&& (string.IsNullOrEmpty(user.UserExtend.UserExtendDataValue[Constants.SOCIAL_PROVIDER_ID_LINE]) == false)
			&& string.IsNullOrEmpty(user.MailAddr)
			&& string.IsNullOrEmpty(user.MailAddr2))
			? w2.App.Common.Line.Constants.LINE_DIRECT_AUTO_LOGIN_USER_NAME
			: user.ComplementUserName;
		this.LoginUserNickName = user.NickName;
		this.LoginUserMail = user.MailAddr;
		this.LoginUserMail2 = user.MailAddr2;
		this.LoginUserBirth = DateTimeUtility.ToStringFromRegion(user.Birth, DateTimeUtility.FormatType.ShortDate2Letter);
		this.LoginUserPoint = PointOptionUtility.GetUserPoint(user.UserId);
		this.LoginUserMemberRankId = MemberRankOptionUtility.GetMemberRankId(user.UserId);
		this.LastLoggedinDate = (user.DateLastLoggedin != null) ? user.DateLastLoggedin.ToString() : null;
		this.LoginUserEasyRegisterFlg = user.EasyRegisterFlg;
		this.LoginUserCpmClusterName = "";
		this.LoginUserFixedPurchaseMemberFlg = user.FixedPurchaseMemberFlg;
		this.LoginMemberRankInfo = (LoginUserMemberRankId != null)
			? MemberRankOptionUtility.GetMemberRankList().FirstOrDefault(memberRank => memberRank.MemberRankId == LoginUserMemberRankId) : null;
		this.UserManagementLevelId = new UserService().Get(user.UserId).UserManagementLevelId;
		this.LoginUserHitTargetListIds = new TargetListService().GetHitTargetListId(
			Constants.W2MP_DEPT_ID,
			user.UserId,
			Constants.FLG_TARGETLISTDATA_TARGET_KBN_TARGETLIST);
		// CPM有効のとき
		if (Constants.CPM_OPTION_ENABLED)
		{
			var attr = new UserService().GetUserAttribute(user.UserId);
			if ((attr == null) || (string.IsNullOrEmpty(attr.CpmClusterId)))
			{
				this.LoginUserCpmClusterName = "";
			}
			else
			{
				this.LoginUserCpmClusterName = attr.GetCpmClusterName(Constants.CPM_CLUSTER_SETTINGS);
				if (string.IsNullOrEmpty(this.LoginUserCpmClusterName))
				{
					AppLogger.WriteError("CPMクラスタが定義されていません：" + StringUtility.ToEmpty(attr.CpmClusterId));
				}
			}
		}
		// PayPal情報格納（格納されていなければ）
		if (Constants.PAYPAL_LOGINPAYMENT_ENABLED)
		{
			// ユーザー拡張項目にPayPal情報がある場合はその情報をセッションに格納
			if (string.IsNullOrEmpty(user.UserExtend.UserExtendDataValue[Constants.PAYPAL_USEREXTEND_COLUMNNAME_CUSTOMER_ID]) == false)
			{
				SessionManager.PayPalCooperationInfo = new PayPalCooperationInfo(user.UserExtend);
				SessionManager.PayPalLoginResult = null;
			}
			// ゲストでPayPal連携 -> PayPal連携したユーザー以外でログインした時にPayPal情報が引き継がれてしまうため、PayPal情報を初期化する(Bug#4681)
			else if (SessionManager.PayPalLoginResult != null)
			{
				SessionManager.PayPalLoginResult = null;
				SessionManager.PayPalCooperationInfo = null;
			}
		}
		// 楽天コネクト情報格納
		if (Constants.RAKUTEN_LOGIN_ENABLED)
		{
			SessionManager.LoginUserRakutenOpenId = StringUtility.ToEmpty(user.UserExtend.UserExtendDataValue[Constants.RAKUTEN_ID_CONNECT_OPEN_ID]);
			SessionManager.IsRakutenIdConnectRegisterUser = (StringUtility.ToEmpty(user.UserExtend.UserExtendDataValue[Constants.RAKUTEN_ID_CONNECT_REGISTER_USER]) == Constants.FLG_USER_RAKUTEN_ID_CONNECT_REGISTER_USER_ON);
		}
	}

	/// <summary>
	/// 遷移先URLの正当性チェック（外部サイトにジャンプしようとしていた場合TOPページに書き換え）
	/// </summary>
	/// <param name="nextUrl">遷移先URL</param>
	/// <returns>次URL</returns>
	public string NextUrlValidation(string nextUrl)
	{
		return NextUrlValidation(this.Request, nextUrl);
	}
	/// <summary>
	/// 遷移先URLの正当性チェック（外部サイトにジャンプしようとしていた場合TOPページに書き換え）
	/// </summary>
	/// <param name="request">HTTPリクエスト</param>
	/// <param name="nextUrl">遷移先URL</param>
	/// <returns>次URL</returns>
	public string NextUrlValidation(HttpRequest request, string nextUrl)
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

	#region BasePage共通処理
	/// <summary>ポストバックか</summary>
	public bool IsPostBack { get { return this.Page.IsPostBack; } }

	/// <summary>
	/// エラー向けコントロール表示リセット処理
	/// </summary>
	/// <param name="cTarget">カスタムバリデータ取得元コントロール</param>
	/// <param name="wcControls">対象コントロール</param>
	public void ResetControlViewsForError(Control cTarget, params WrappedControl[] wcControls)
	{
		// カスタムバリデータ取得
		var customValidators = new List<CustomValidator>();
		CreateCustomValidators(cTarget, customValidators);

		if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten) return;

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
	/// カスタムバリデータ一覧作成
	/// </summary>
	/// <param name="cTarget">カスタムバリデータを探す対象コントロール</param>
	/// <param name="lCustomValidators">カスタムバリデータ一覧（再帰してここに作成されていく）</param>
	public void CreateCustomValidators(Control cTarget, List<CustomValidator> lCustomValidators)
	{
		foreach (Control ccInner in cTarget.Controls)
		{
			// 中にコントロールがあればその中を探しに行く（再帰）
			if (ccInner.Controls.Count != 0)
			{
				CreateCustomValidators(ccInner, lCustomValidators);
			}
			// カスタムバリデータであれば追加
			else if (ccInner is CustomValidator)
			{
				lCustomValidators.Add((CustomValidator)ccInner);
			}
		}
	}

	/// <summary>
	/// 数値表示
	/// </summary>
	/// <param name="objNum">数量</param>
	/// <returns>数量</returns>
	public string GetNumeric(object objNum)
	{
		var dec = 0m;
		if (decimal.TryParse(StringUtility.ToEmpty(objNum), out dec))
		{
			return StringUtility.ToNumeric(DecimalUtility.DecimalRound(dec, DecimalUtility.Format.RoundDown));
		}
		else
		{
			return string.Empty;
		}
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
		// ValidatorXML取得＆エラー内容をカスタムバリデータへセット（フォーカス考慮してValidator Xmlの下から処理をする）
		XmlDocument validatorXml = Validator.GetValidateXmlDocument(validatorCheckKbn);
		XmlNodeList columnNodes = validatorXml.SelectNodes(validatorCheckKbn + "/Column");
		for (int i = columnNodes.Count - 1; i >= 0; i--)
		{
			string fieldName = columnNodes[i].Attributes["name"].InnerText;
			if (errorMessages.ContainsKey(fieldName))
			{
				if (columnNodes[i].Attributes["control"] != null)
				{
					string controlName = columnNodes[i].Attributes["control"].InnerText;
					foreach (var target in customValidators.FindAll(cv => cv.ControlToValidate == controlName))
					{
						// 対象コントロールが存在する？
						WebControl control = (WebControl)target.Parent.FindControl(controlName);
						if (control != null)
						{
							// 初期化
							target.IsValid = true;
							target.ErrorMessage = "";
							control.CssClass = control.CssClass.Replace(Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING, " ");
							// 対象コントロールの表示変更
							ChangeControlLooksForValidator(
								errorMessages,
								fieldName,
								target,
								control);
						}

						var controlNameClientId = target.Parent.FindControl(controlName).ClientID;
						var lpValidateErrorElementClientId = (SessionManager.LpValidateErrorElementClientId != null)
							? SessionManager.LpValidateErrorElementClientId.Split('_').Last()
							: "";
						// LPページ複数カート時の存在しないClientIdを指定しない
						if (controlNameClientId.Split('_').Last()
							!= lpValidateErrorElementClientId.Split('_').Last())
						{
							SessionManager.LpValidateErrorElementClientId = controlNameClientId;
						}
					}
				}
			}
		}
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
		// カスタムバリデータが存在しない場合は背景色などセットしない
		if (wcvCustomValidator.InnerControl == null)
		{
			return (dicErrorMessages.ContainsKey(strTargetKey));
		}

		// コントロール変更
		return ChangeControlLooksForValidator(
			dicErrorMessages,
			strTargetKey,
			wcvCustomValidator.InnerControl,
			wcControls.Select(wc => (WebControl)wc.InnerControl).Where(wc => wc != null).ToArray());
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
		if (dicErrorMessages.ContainsKey(strTargetKey))
		{
			// エラーメッセージ取得
			cvCustomValidator.ErrorMessage = StringUtility.ChangeToBrTag(dicErrorMessages[strTargetKey]);
			cvCustomValidator.IsValid = false;

			foreach (var wcWebControl in wcWebControls)
			{
				// ラジオボタンリストの場合、エラーの背景色をセットしない
				if (wcWebControl.GetType() == typeof(RadioButtonList)) continue;

				// コントロールに背景色をセットする
				if (wcWebControl.CssClass.Contains(Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING) == false)
				{
					wcWebControl.CssClass += Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING;
				}
			}
		}
		else
		{
			cvCustomValidator.ErrorMessage = "";
			cvCustomValidator.IsValid = true;
			foreach (var wcWebControl in wcWebControls)
			{
				wcWebControl.CssClass = wcWebControl.CssClass.Replace(Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING, " ");
			}
		}

		return cvCustomValidator.IsValid;
	}

	/// <summary>
	/// Set disable and hide custom validator control information list
	/// </summary>
	/// <param name="customValidators">Wrapped custom validators</param>
	public void SetDisableAndHideCustomValidatorControlInformationList(
		IEnumerable<WrappedCustomValidator> customValidators)
	{
		var customValidatorControlDisabledInformation = new Dictionary<string, string>();
		foreach (var customValidator in customValidators)
		{
			customValidator.DisableAndHideCustomValidator();
			if (string.IsNullOrEmpty(customValidator.ControlToValidate)) continue;

			var wcTarget = (WebControl)customValidator.Parent.FindControl(customValidator.ControlToValidate);
			if (wcTarget == null) continue;

			if (customValidatorControlDisabledInformation.ContainsKey(wcTarget.ClientID))
			{
				var oldValue = customValidatorControlDisabledInformation[wcTarget.ClientID];
				if (oldValue.Split('+').Any(item => item == customValidator.ClientID)) continue;

				customValidatorControlDisabledInformation[wcTarget.ClientID] =
					string.Format(
						"{0}+{1}",
						oldValue,
						customValidator.ClientID);
			}
			else
			{
				customValidatorControlDisabledInformation.Add(wcTarget.ClientID, customValidator.ClientID);
			}
		}
		var customValidatorControlDisabledInformationList = customValidatorControlDisabledInformation.Select(item =>
			string.Format("{0} {1}", item.Key, item.Value));
		this.CustomValidatorControlDisabledInformationList = customValidatorControlDisabledInformationList.ToArray();
	}

	/// <summary>
	/// Set custom validator control information list
	/// </summary>
	/// <param name="target">カスタムバリデータ取得元コントロール</param>
	public void SetCustomValidatorControlInformationList(Control target)
	{
		// カスタムバリデータ取得
		var customValidators = new List<CustomValidator>();
		CreateCustomValidators(target, customValidators);

		var customValidatorControlInformation = new Dictionary<string, string>();
		foreach (var customValidator in customValidators)
		{
			if (string.IsNullOrEmpty(customValidator.ControlToValidate)) continue;

			var wcTarget = (WebControl)customValidator.Parent.FindControl(customValidator.ControlToValidate);
			if (wcTarget == null) continue;

			if (customValidatorControlInformation.ContainsKey(wcTarget.ClientID))
			{
				var oldValue = customValidatorControlInformation[wcTarget.ClientID];
				if (oldValue.Split('+').Any(item => item == customValidator.ClientID)) continue;

				customValidatorControlInformation[wcTarget.ClientID] =
					string.Format(
						"{0}+{1}",
						oldValue,
						customValidator.ClientID);
			}
			else
			{
				customValidatorControlInformation.Add(wcTarget.ClientID, customValidator.ClientID);
			}
		}
		var customValidatorControlInformationList = customValidatorControlInformation.Select(item =>
			string.Format("{0} {1}", item.Key, item.Value));
		this.CustomValidatorControlInformationList = customValidatorControlInformationList.ToArray();
	}
	#endregion

	/// <summary>
	/// サイト変更URL作成
	/// </summary>
	/// <remarks>
	/// ・フレンドリURLの場合でも無駄なパラメタを付与しない
	/// ・changesiteパラメタが重複してふよされないようにする
	/// </remarks>
	/// <param name="changeSiteKbn">サイト変更区分</param>
	/// <returns>サイト変更URL</returns>
	private string CreateChangeSiteUrl(string changeSiteKbn)
	{
		var rawUrl = WebUtility.GetRawUrl(this.Context.Request);
		var splitUrl = rawUrl.Split('?');
		var urlCreator = new UrlCreator(splitUrl[0]);

		if (splitUrl.Length >= 2)
		{
			foreach (var requestKey in splitUrl[1].Split('&'))
			{
				var keyval = requestKey.Split('=');
				if ((keyval.Length >= 2)
					&& (keyval[0] != Constants.REQUEST_KEY_CHANGESITE))
				{
					urlCreator.AddParam(keyval[0], keyval[1]);
				}
			}
		}
		urlCreator.AddParam(Constants.REQUEST_KEY_CHANGESITE, changeSiteKbn);

		var url = urlCreator.CreateUrl();
		return url;
	}

	/// <summary>
	/// モバイルか判定
	/// </summary>
	/// <param name="request">リクエスト</param>
	/// <returns>モバイルか</returns>
	private bool CheckIsMobile(HttpRequest request)
	{
		// モバイル遷移先設定あり？
		if (Constants.PATH_MOBILESITE.Length != 0)
		{
			var userAgent = StringUtility.ToEmpty(request.UserAgent);

			// モバイル判定
			if (Regex.IsMatch(userAgent, @"^DoCoMo/")
				|| Regex.IsMatch(userAgent, @"^((KDDI-)?[A-Z]+[A-Z0-9]+\s+)?UP\.Browser/")
				|| Regex.IsMatch(userAgent, @"^((KDDI-)?[A-Z]+[A-Z0-9]+\s+)?UP\.Browser/")
				|| Regex.IsMatch(userAgent, @"^(J-PHONE|Vodafone|SoftBank)/")
				|| (request.ServerVariables["HTTP_X_JPHONE_MSNAME"] != null)
#if DEBUG
				|| Regex.IsMatch(userAgent, @"^(J-EMULATOR|Vemulator)/")
#endif
)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// リンク式決済か
	/// </summary>
	/// <returns>リンク式決済か</returns>
	public bool IsCreditCardLinkPayment()
	{
		return ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Zeus) && Constants.PAYMENT_SETTING_ZEUS_USE_LINKPOINT_ENABLED);
	}

	/// <summary>
	/// 置換タグの置換後の値(MaxLength)を取得
	/// </summary>
	/// <param name="strReplaceTag">置換タグ</param>
	/// <returns>置換後の値</returns>
	public int GetMaxLength(string strReplaceTag)
	{
		// 設定が見つからない場合やintに出来ない場合、文字数制限なし（MaxLength = 0）。
		int iMaxLength;
		if (int.TryParse(Constants.TAG_REPLACER_DATA_SCHEMA.GetValue(strReplaceTag), out iMaxLength) == false)
		{
			iMaxLength = 0;
		}

		return iMaxLength;
	}

	/// <summary>
	/// アフィリエイトタグ データバインド
	/// </summary>
	/// <remarks>
	/// LPページのようにカートを変更している場合、
	/// カートの内容が確定後にデータバインドを行ってください
	/// </remarks>
	public void AffiliateTagDataBind()
	{
		var affiliateTagHead = GetDefaultMaster().FindControl("AffiliateTagHead");
		if (affiliateTagHead != null) affiliateTagHead.DataBind();

		var affiliateTagBodyTop = GetDefaultMaster().FindControl("AffiliateTagBodyTop");
		if (affiliateTagBodyTop != null) affiliateTagBodyTop.DataBind();

		var affiliateTagBodyBottom = GetDefaultMaster().FindControl("AffiliateTagBodyBottom");
		if (affiliateTagBodyBottom != null) affiliateTagBodyBottom.DataBind();
	}

	/// <summary>
	/// ブランド名翻訳名取得
	/// </summary>
	/// <returns>ブランド名翻訳名</returns>
	private string GetBrandTranslationName()
	{
		return NameTranslationCommon.GetTranslationName(
			this.BrandId,
			Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTBRAND,
			Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTBRAND_BRAND_NAME,
			this.m_brandName);
	}

	/// <summary>
	/// Check Site Domain And Redirect With Post Data
	/// </summary>
	public void CheckSiteDomainAndRedirectWithPostData()
	{
		if ((this.PostParams.Count == 0)
			|| this.SecurePageProtocolAndHost.ToLower().Contains(Constants.SITE_DOMAIN.ToLower()))
		{
			return;
		}

		var url = string.Format("{0}{1}{2}",
			Constants.PROTOCOL_HTTPS,
			Constants.SITE_DOMAIN,
			this.RawUrl);

		var html = new StringBuilder();
		html.Append("<html>")
			.Append("<body onload='document.forms[0].submit()'>")
			.AppendFormat("<form action='{0}' method='post'>", url);
		foreach (var parameter in this.PostParams)
		{
			html.AppendFormat(
				"<input type='hidden' name='{0}' value='{1}'>",
				parameter.Key,
				parameter.Value);
		}
		html.Append("</form>")
			.Append("</body>")
			.Append("</html>");

		Response.Clear();
		Response.Write(html.ToString());
		Response.End();
	}

	/// <summary>
	/// Set Information Receiving Store
	/// </summary>
	/// <param name="cartList">Cart List</param>
	public void SetInformationReceivingStore(CartObjectList cartList)
	{
		if ((cartList == null)
			|| (cartList.Items.Count == 0)
			|| (this.CartIndexWhenCallBackFromEcPay == -1)
			|| (this.PostParams.Count == 0))
		{
			return;
		}

		var cart = cartList.Items[this.CartIndexWhenCallBackFromEcPay];
		var cartShipping = cart.Shippings[0];
		cartShipping.UpdateConvenienceStoreAddr(
			CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE,
			this.PostParams[ECPayConstants.PARAM_CVSSTOREID],
			this.PostParams[ECPayConstants.PARAM_CVSSTORENAME],
			this.PostParams[ECPayConstants.PARAM_CVSADDRESS],
			this.PostParams[ECPayConstants.PARAM_CVSTELEPHONE],
			cartShipping.ShippingReceivingStoreType);
	}

	/// <summary>
	/// 新LPカートの画面遷移制御用セッションを初期化する
	/// </summary>
	private void InitializeSessionForLpCartConfirm()
	{
		// 画像の取得などのリクエストの場合、何もしない
		if ((this.Request.AcceptTypes != null)
			&& ((this.Request.AcceptTypes.Any(ap => ap.Contains("html"))) == false))
		{
			return;
		}

		// LPカート遷移のURLだったら何もしない.
		var path = this.Request[Constants.REQUEST_KEY_RETURN_URL];
		var targetUrl = string.IsNullOrEmpty(path) ? this.Request.Url.AbsolutePath : path;
		var notChangeUrls = new string[]
		{
			(Constants.PATH_ROOT_FRONT_PC + Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM),
			(Constants.PATH_ROOT_FRONT_PC + Constants.PAGE_FRONT_CART_LIST_LP),
		};
		if (notChangeUrls.Any(url => url == targetUrl)) return;

		// LPカート制御用セッションを初期化する
		var lpContorolPath = Constants.PATH_ROOT_FRONT_PC + Constants.PAGE_FRONT_CART_LIST_LP;
		var lpCartConfirmSessionKey = Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK + lpContorolPath;
		if (this.Session[lpCartConfirmSessionKey] == null) return;

		this.Session[lpCartConfirmSessionKey] = string.Empty;
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
		GlobalAddressUtil.BindingAddressByGlobalZipcode(
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
	/// Create authentication code
	/// </summary>
	/// <returns>Authentication code</returns>
	private string CreateAuthenticationCode()
	{
		var random = new Random();
		var authenticationCode = string.Join(
			string.Empty,
			Enumerable.Range(0, Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_AUTH_CODE_DIGITS)
				.Select(item => random.Next(0, 9)));

		return authenticationCode;
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
		wlbAuthenticationStatus.Text = string.Empty;
		wtbAuthenticationCode.Text = string.Empty;
		wtbAuthenticationCode.Enabled = true;

		var phoneNumber = SMSHelper.GetSmsToPhoneNumber(userTel, userAddrCountryIsoCode);
		if (string.IsNullOrEmpty(phoneNumber)) return;

		var tempKey = string.Format(
			"{0}_{1}",
			Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_AUTH_METHOD,
			phoneNumber);

		// Check if is valid to send sms
		var authCodeSendCount = 0;
		var sendFailCount = 0;
		var errorMessage = SMSHelper.CheckBeforeSendAuthenticationCode(
			tempKey,
			Request.UserHostAddress,
			ref sendFailCount,
			ref authCodeSendCount);

		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			StopTimeCount();
			wlbAuthenticationStatus.Visible = true;
			wlbAuthenticationStatus.InnerControl.Style["color"] = "#FF0000";
			wlbAuthenticationStatus.Text = errorMessage;
			wtbAuthenticationCode.Enabled = false;
			return;
		}

		// Send sms
		var tempDatasService = new TempDatasService();
		var authenticationCode = CreateAuthenticationCode();
		var isSendSmsSucccess = SMSHelper.SendSmsAuthenticationCode(authenticationCode, phoneNumber);

		if (isSendSmsSucccess == false)
		{
			// Update send sms failed count
			tempDatasService.Save(
				TempDatasService.TempType.AuthCodeTrySendTel,
				tempKey,
				(sendFailCount + 1));
			return;
		}

		// Save ip address send authentication code
		tempDatasService.Save(
			TempDatasService.TempType.AuthCodeSendIpAddress,
			Request.UserHostAddress,
			(authCodeSendCount + 1));

		// Save authentication code
		tempDatasService.Save(
			TempDatasService.TempType.AuthCode,
			tempKey,
			authenticationCode);

		wlbAuthenticationStatus.InnerControl.Style["color"] = "#000000";
		wtbAuthenticationCode.Attributes["onchange"] = "checkAuthenticationCodeInput()";

		ScriptManager.RegisterStartupScript(
			this.Page,
			this.Page.GetType(),
			"setAuthenticationMessage",
			"setAuthenticationMessage()",
			true);
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
		var errorMessages = new Dictionary<string, string>();
		var phoneSMSNumber = SMSHelper.GetSmsToPhoneNumber(userTel, userAddrCountryIsoCode);
		var tempKey = string.Format(
			"{0}_{1}",
			Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_AUTH_METHOD,
			phoneSMSNumber);
		var errorMessage = string.Empty;
		var checkAuthenticationCode = SMSHelper.CheckAuthenticationCode(
			wtbAuthenticationCode.Text,
			tempKey,
			Request.UserHostAddress,
			ref errorMessage);

		switch (checkAuthenticationCode)
		{
			case Constants.FLG_AUTHENTICATION_RESULT_STOP_PROCESS:
				StopTimeCount();
				errorMessages.Add(
					Constants.CONST_KEY_USER_AUTHENTICATION_CODE,
					errorMessage);
				return errorMessages;

			case Constants.FLG_AUTHENTICATION_RESULT_VERIFICATION_CODE_IS_INCORRECT:
				wlbAuthenticationMessage.Text = string.Empty;
				errorMessages.Add(
					Constants.CONST_KEY_USER_AUTHENTICATION_CODE,
					errorMessage);
				return errorMessages;

			case Constants.FLG_AUTHENTICATION_RESULT_VERIFICATION_CODE_HAS_EXPIRED:
				errorMessages.Add(
					Constants.CONST_KEY_USER_AUTHENTICATION_CODE,
					errorMessage);
				return errorMessages;

			case Constants.FLG_AUTHENTICATION_RESULT_SUCCSESS:
				wlbGetAuthenticationCode.Enabled
					= wtbAuthenticationCode.Enabled
					= false;
				StopTimeCount();
				return errorMessages;
		}

		return errorMessages;
	}

	/// <summary>
	/// Stop time count
	/// </summary>
	public void StopTimeCount()
	{
		ScriptManager.RegisterStartupScript(
			this.Page,
			this.Page.GetType(),
			"clearInterval",
			"clearInterval(setIntervalId)",
			true);
	}

	/// <summary>
	/// Get verification code note
	/// </summary>
	/// <param name="countryIsoCode">Country iso code</param>
	/// <returns>Verification_code note</returns>
	public string GetVerificationCodeNote(string countryIsoCode)
	{
		var note = string.Format(
			ReplaceTag("@@User.message_enter_verification_code.name@@", countryIsoCode),
			Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_AUTH_CODE_DIGITS);
		return note;
	}

	/// <summary>
	/// Remove error input class
	/// </summary>
	/// <param name="wTextBox">Wrapped text box</param>
	public void RemoveErrorInputClass(WrappedTextBox wTextBox)
	{
		wTextBox.CssClass = wTextBox.CssClass.Replace(
			Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING,
			string.Empty);
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
		wtbAuthenticationCode.Text = authenticationCode;
		wlbGetAuthenticationCode.Enabled = (this.HasAuthenticationCode == false);
		wtbAuthenticationCode.Enabled = (this.HasAuthenticationCode == false)
			&& (string.IsNullOrEmpty(authenticationCode) == false);

		if (wlbAuthenticationStatus == null) return;

		wlbAuthenticationStatus.Visible = true;
		wlbAuthenticationStatus.InnerControl.Style["color"] = "#FF0000";
		wlbAuthenticationStatus.Text =
			WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_REACQUIRE_THE_AUTHENTICATION_CODE);
	}

	/// <summary>
	/// Get value for telephone
	/// </summary>
	/// <param name="wtbTel1_1">Wrapped text box telephone1 1</param>
	/// <param name="wtbTel1_2">Wrapped text box telephone2 2</param>
	/// <param name="wtbTel1_3">Wrapped text box telephone3 3</param>
	/// <param name="wtbTel1">Wrapped text box telephone1</param>
	/// <param name="wtbTel1Global">Wrapped text box telephone global</param>
	/// <param name="userAddrCountryIsoCode">User address country iso code</param>
	/// <returns>Telephone number</returns>
	public string GetValueForTelephone(
		WrappedTextBox wtbTel1_1,
		WrappedTextBox wtbTel1_2,
		WrappedTextBox wtbTel1_3,
		WrappedTextBox wtbTel1,
		WrappedTextBox wtbTel1Global,
		string userAddrCountryIsoCode)
	{
		var inputTel = StringUtility.ToHankaku(wtbTel1_1.HasInnerControl
			? wtbTel1_1.Text.Trim()
			: wtbTel1.Text.Trim());

		if (wtbTel1_2.HasInnerControl)
		{
			inputTel = UserService.CreatePhoneNo(
				inputTel,
				StringUtility.ToHankaku(wtbTel1_2.Text.Trim()),
				StringUtility.ToHankaku(wtbTel1_3.Text.Trim()));
		}

		var tel1 = new Tel(inputTel);
		var telephone = (string.IsNullOrEmpty(tel1.TelNo) == false)
			? tel1.TelNo
			: inputTel;

		if (IsCountryJp(userAddrCountryIsoCode) == false) telephone = StringUtility.ToHankaku(wtbTel1Global.Text.Trim());

		return telephone;
	}

	/// <summary>
	/// Get wrapped control of text box authentication code
	/// </summary>
	/// <param name="isCountryJp">Is country Jp</param>
	/// <param name="control">Control</param>
	/// <returns>Wrapped text box</returns>
	public WrappedTextBox GetWrappedTextBoxAuthenticationCode(
		bool isCountryJp,
		Control control)
	{
		if (control == null)
		{
			return isCountryJp
				? GetWrappedControl<WrappedTextBox>("tbAuthenticationCode")
				: GetWrappedControl<WrappedTextBox>("tbAuthenticationCodeGlobal");
		}

		return isCountryJp
			? GetWrappedControl<WrappedTextBox>(control, "tbAuthenticationCode")
			: GetWrappedControl<WrappedTextBox>(control, "tbAuthenticationCodeGlobal");
	}

	/// <summary>
	/// Get wrapped control of label authentication status
	/// </summary>
	/// <param name="isCountryJp">Is country Jp</param>
	/// <param name="control">Control</param>
	/// <returns>Wrapped label</returns>
	public WrappedLabel GetWrappedControlOfLabelAuthenticationStatus(
		bool isCountryJp,
		Control control)
	{
		if (control == null)
		{
			return isCountryJp
				? GetWrappedControl<WrappedLabel>("lbAuthenticationStatus")
				: GetWrappedControl<WrappedLabel>("lbAuthenticationStatusGlobal");
		}

		return isCountryJp
			? GetWrappedControl<WrappedLabel>(control, "lbAuthenticationStatus")
			: GetWrappedControl<WrappedLabel>(control, "lbAuthenticationStatusGlobal");
	}

	/// <summary>
	/// Get wrapped control of label authentication message
	/// </summary>
	/// <param name="isCountryJp">Is country Jp</param>
	/// <param name="control">Control</param>
	/// <returns>Wrapped label</returns>
	public WrappedLabel GetWrappedControlOfLabelAuthenticationMessage(
		bool isCountryJp,
		Control control)
	{
		if (control == null)
		{
			return isCountryJp
				? GetWrappedControl<WrappedLabel>("lbAuthenticationMessage")
				: GetWrappedControl<WrappedLabel>("lbAuthenticationMessageGlobal");
		}

		return isCountryJp
			? GetWrappedControl<WrappedLabel>(control, "lbAuthenticationMessage")
			: GetWrappedControl<WrappedLabel>(control, "lbAuthenticationMessageGlobal");
	}

	/// <summary>
	/// Get wrapped control of link button authentication code
	/// </summary>
	/// <param name="isCountryJp">Is country Jp</param>
	/// <param name="control">Control</param>
	/// <returns>Wrapped link button</returns>
	public WrappedLinkButton GetWrappedControlOfLinkButtonAuthenticationCode(
		bool isCountryJp,
		Control control)
	{
		if (control == null)
		{
			return isCountryJp
				? GetWrappedControl<WrappedLinkButton>("lbGetAuthenticationCode")
				: GetWrappedControl<WrappedLinkButton>("lbGetAuthenticationCodeGlobal");
		}

		return isCountryJp
			? GetWrappedControl<WrappedLinkButton>(control, "lbGetAuthenticationCode")
			: GetWrappedControl<WrappedLinkButton>(control, "lbGetAuthenticationCodeGlobal");
	}

	/// <summary>
	/// 紹介コードチェック
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	public void CheckReferralCode(string userId)
	{
		// 紹介コードない場合はチェックしない
		if (string.IsNullOrEmpty(SessionManager.ReferralCode)) return;

		// ゲストの場合チェックしない
		if (this.IsLoggedIn == false) return;

		var user = DomainFacade.Instance.UserService.GetUserByReferralCode(SessionManager.ReferralCode, userId);
		if (this.LoginUser.UserId == (user ?? new UserModel()).UserId)
		{
			SessionManager.ReferralCode = null;
		}
	}

	/// <summary>
	/// 請求先住所取得オプションの対象か判定
	/// </summary>
	/// <returns>判定結果（TRUE：対象、FALSE：対象外）</returns>
	public bool IsTargetToExtendedAmazonAddressManagerOption()
	{
		if (Constants.AMAZONPAYMENTCV2_USEBILLINGASOWNER_ENABLED == false) return false;
		if (this.IsLoggedIn) return false;
		if (this.IsAmazonLogin == false) return false;
		return true;
	}

	/// <summary>
	/// AmazonAddressManagerオプションの対象に対してカスタムバリデータコントロールを設定
	/// </summary>
	/// <param name="wrCartList">カートリスト</param>
	public void SetCustomValidatorControlsForExtendedAmazonAddressManagerOption(WrappedRepeater wrCartList)
	{
		var searchTag = new List<string>
		{
			"cvOwnerName1",
			"cvOwnerName2",
			"cvOwnerName1",
			"cvOwnerName2",
			"cvOwnerNameKana1",
			"cvOwnerNameKana2",
			"cvOwnerBirth",
			"cvOwnerSex",
			"cvOwnerMailAddr",
			"cvOwnerMailAddrForCheck",
			"cvOwnerMailAddrConf",
			"cvOwnerMailAddr2",
			"cvOwnerMailAddr2Conf",
			"cvOwnerZip1",
			"cvOwnerZip2",
			"cvOwnerAddr1",
			"cvOwnerAddr2",
			"cvOwnerAddr3",
			"cvOwnerTel1_1",
			"cvOwnerTel1_2",
			"cvOwnerTel1_3",
			"cvOwnerTel2_1",
			"cvOwnerTel2_2",
			"cvOwnerTel2_3",
		};

		var repeaterItem = wrCartList.Items.Cast<RepeaterItem>().First();
		var customValidatorControls = searchTag
			.Select(target => GetWrappedControl<WrappedCustomValidator>(repeaterItem, target))
			.ToList();

		foreach (var customValidatorControl in customValidatorControls)
		{
			customValidatorControl.ValidationGroup = "OrderOwnerForAmazonPayGuest";
		}
	}

	/// <summary>ブランドID</summary>
	public string BrandId
	{
		get { return StringUtility.ToEmpty(this.Session[Constants.SESSION_KEY_BRAND_ID]); }
		set { this.Session[Constants.SESSION_KEY_BRAND_ID] = value; }
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
		get
		{
			if (Constants.GLOBAL_OPTION_ENABLE == false) return this.m_brandName;
			return GetBrandTranslationName();
		}
		set { this.m_brandName = value; }
	}
	private string m_brandName = null;
	/// <summary>ブランドタイトル</summary>
	public string BrandTitle { get; set; }
	/// <summary>ブランドSEOキーワード</summary>
	public string BrandSeoKeyword { get; set; }
	/// <summary>ブランド追加タグ情報</summary>
	public string BrandAdditionalDsignTag { get; set; }

	/// <summary>セキュアページプロトコル取得</summary>
	public string SecurePageProtocolAndHost
	{
		get { return SessionSecurityManager.GetSecurePageProtocolAndHost(); }
	}
	/// <summary>非セキュアページプロトコル取得</summary>
	public string UnsecurePageProtocolAndHost
	{
		get { return SessionSecurityManager.GetUnsecurePageProtocolAndHost(this.Context); }
	}
	/// <summary>RawUrl（IISのバージョンによる機能の違いを吸収）</summary>
	public string RawUrl
	{
		get { return WebUtility.GetRawUrl(this.Request); }
	}
	/// <summary>プレビューか</summary>
	public bool IsPreview { get { return Preview.IsPreview(this.Request); } }
	/// <summary>PCサイト変更URL</summary>
	public string ChangeToPcSiteUrl
	{
		get { return CreateChangeSiteUrl(Constants.KBN_REQUEST_CHANGESITE_PC); }
	}
	/// <summary>スマートフォンサイト変更URL</summary>
	public string ChangeToSmartPhoneSiteUrl
	{
		get { return CreateChangeSiteUrl(Constants.KBN_REQUEST_CHANGESITE_SMARTPHONE); }
	}

	/// <summary>ログインユーザー</summary>
	public UserModel LoginUser
	{
		get { return (UserModel)Session[Constants.SESSION_KEY_LOGIN_USER]; }
		set { Session[Constants.SESSION_KEY_LOGIN_USER] = value; }
	}
	/// <summary>ログインユーザーID</summary>
	public string LoginUserId
	{
		get { return (string)Session[Constants.SESSION_KEY_LOGIN_USER_ID]; }
		set { Session[Constants.SESSION_KEY_LOGIN_USER_ID] = value; }
	}
	/// <summary>ログインユーザー名</summary>
	public string LoginUserName
	{
		get { return (string)Session[Constants.SESSION_KEY_LOGIN_USER_NAME]; }
		set { Session[Constants.SESSION_KEY_LOGIN_USER_NAME] = value; }
	}
	/// <summary>ログインユーザーニックネーム</summary>
	public string LoginUserNickName
	{
		get { return (string)Session[Constants.SESSION_KEY_LOGIN_USER_NICK_NAME]; }
		set { Session[Constants.SESSION_KEY_LOGIN_USER_NICK_NAME] = value; }
	}
	/// <summary>ログインユーザーメールアドレス</summary>
	public string LoginUserMail
	{
		get { return (string)Session[Constants.SESSION_KEY_LOGIN_USER_MAIL]; }
		set { Session[Constants.SESSION_KEY_LOGIN_USER_MAIL] = value; }
	}
	/// <summary>ログインユーザーメールアドレス2</summary>
	public string LoginUserMail2
	{
		get { return (string)Session[Constants.SESSION_KEY_LOGIN_USER_MAIL2]; }
		set { Session[Constants.SESSION_KEY_LOGIN_USER_MAIL2] = value; }
	}
	/// <summary>ログインユーザー生年月日</summary>
	public string LoginUserBirth
	{
		get { return (string)Session[Constants.SESSION_KEY_LOGIN_USER_BIRTH]; }
		set { Session[Constants.SESSION_KEY_LOGIN_USER_BIRTH] = value; }
	}
	/// <summary>会員ランクID</summary>
	public string MemberRankId
	{
		// あまりいいやり方でないけれど、ログインしてるかどうかに関わらず使ってる箇所が多すぎるので。。
		get
		{
			if (this.IsLoggedIn) return this.LoginUserMemberRankId;

			// 会員ランクOPOFFの場合はnothingを返す必要がある
			return Constants.MEMBER_RANK_OPTION_ENABLED ? "" : "nothing";
		}
	}
	/// <summary>ログインユーザー会員ランクID</summary>
	public string LoginUserMemberRankId
	{
		get { return (string)Session[Constants.SESSION_KEY_LOGIN_USER_MEMBER_RANK_ID]; }
		set { Session[Constants.SESSION_KEY_LOGIN_USER_MEMBER_RANK_ID] = value; }
	}
	/// <summary>会員ランク情報</summary>
	public MemberRankModel LoginMemberRankInfo
	{
		get { return (MemberRankModel)Session[Constants.SESSION_KEY_LOGIN_USER_MEMBER_RANK]; }
		set { Session[Constants.SESSION_KEY_LOGIN_USER_MEMBER_RANK] = value; }
	}
	/// <summary>会員ランク名</summary>
	public string MemberRankName
	{
		get
		{
			var memberRankName = this.LoginMemberRankInfo.MemberRankName;

			// 翻訳名取得
			var memberRankTranslationName = NameTranslationCommon.GetTranslationName(
				this.LoginUserMemberRankId,
				Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_MEMBERRANK,
				Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_MEMBERRANK_MENBER_RANK_NAME,
				memberRankName);
			return memberRankTranslationName;
		}
	}
	/// <summary>User Fixed Purchase Member Flg</summary>
	public string UserFixedPurchaseMemberFlg
	{
		get
		{
			return (this.IsLoggedIn ? (this.LoginUserFixedPurchaseMemberFlg ?? string.Empty) : string.Empty);
		}
	}
	/// <summary>ログインユーザー定期会員フラグ</summary>
	public string LoginUserFixedPurchaseMemberFlg
	{
		get { return (string)Session[Constants.SESSION_KEY_LOGIN_USER_FIXED_PURCHASE_MEMBER_FLG]; }
		set { Session[Constants.SESSION_KEY_LOGIN_USER_FIXED_PURCHASE_MEMBER_FLG] = value; }
	}
	/// <summary>ログインユーザーCPMクラスタ名</summary>
	public string LoginUserCpmClusterName
	{
		get { return StringUtility.ToEmpty(Session[Constants.SESSION_KEY_LOGIN_USER_CPM_CLUSTER_NAME]); }
		set { Session[Constants.SESSION_KEY_LOGIN_USER_CPM_CLUSTER_NAME] = value; }
	}
	/// <summary>前回ログイン日時</summary>
	public string LastLoggedinDate
	{
		get { return (string)Session[Constants.SESSION_KEY_LOGIN_USER_LAST_LOGGEDIN_DATE]; }
		set { Session[Constants.SESSION_KEY_LOGIN_USER_LAST_LOGGEDIN_DATE] = value; }
	}
	/// <summary>ログインユーザかんたん会員フラグ</summary>
	public string LoginUserEasyRegisterFlg
	{
		get { return (string)Session[Constants.SESSION_KEY_LOGIN_USER_EASY_REGISTER_FLG]; }
		set { Session[Constants.SESSION_KEY_LOGIN_USER_EASY_REGISTER_FLG] = value; }
	}
	/// <summary>Amazonログイン状態</summary>
	public bool IsAmazonLoggedIn
	{
		get { return (Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL] != null); }
	}
	/// <summary>会員登録入力情報</summary>
	public UserModel RegisterUser
	{
		get { return (UserModel)Session[Constants.SESSION_KEY_REGISTER_USER_INPUT]; }
		set { Session[Constants.SESSION_KEY_REGISTER_USER_INPUT] = value; }
	}
	/// <summary>ユーザー拡張項目入力情報</summary>
	public UserExtendInput UserExtendInput
	{
		get { return (UserExtendInput)Session[Constants.SESSION_KEY_USER_EXTEND_INPUT]; }
		set { Session[Constants.SESSION_KEY_USER_EXTEND_INPUT] = value; }
	}
	/// <summary>楽天IDConnect会員登録か</summary>
	public bool IsRakutenIdConnectUserRegister
	{
		get
		{
			var result = (Constants.RAKUTEN_LOGIN_ENABLED
				&& (SessionManager.RakutenIdConnectActionInfo != null)
				&& (SessionManager.RakutenIdConnectActionInfo.RakutenIdConnectUserInfoResponseData != null));
			return result;
		}
	}

	/// <summary>ログイン状態</summary>
	public bool IsLoggedIn
	{
		get { return (this.LoginUserId != null); }
	}
	/// <summary>かんたん会員かどうか</summary>
	public bool IsEasyUser
	{
		get { return (SessionSecurityManager.IsEasyUser(this.Context)); }
	}
	/// <summary>本ポイント合計</summary>
	public decimal LoginUserPointUsable
	{
		get { return (this.LoginUserPoint != null) ? this.LoginUserPoint.PointUsable : 0; }
	}
	/// <summary>仮ポイント合計</summary>
	public decimal LoginUserPointTemp
	{
		get { return (this.LoginUserPoint != null) ? this.LoginUserPoint.PointTemp : 0; }
	}
	/// <summary>通常ポイント有効期限</summary>
	public DateTime? LoginUserPointExpiry
	{
		get { return ((this.LoginUserPointUsable != 0) ? this.LoginUserPoint.BasicPoint.PointCompExpiryDate : null); }
	}
	/// <summary>通常本ポイント数</summary>
	public decimal LoginUserBasePoint
	{
		get { return (this.LoginUserPoint != null) ? this.LoginUserPoint.BasicPoint.PointComp : 0; }
	}
	/// <summary>定期購入に利用できるポイント</summary>
	public decimal LoginUserPointUsableForFixedPurchase
	{
		get { return this.LoginUserBasePoint; }
	}
	/// <summary>期間限定ポイント所有しているか</summary>
	public bool HasLimitedTermPoint
	{
		get { return (this.LoginUserLimitedTermPointTotal > 0); }
	}
	/// <summary>利用可能期間限定ポイント合計</summary>
	public decimal LoginUserLimitedTermPointUsableTotal
	{
		get
		{
			return (this.LoginUserPoint != null)
				? this.LoginUserPoint.LimitedTermPoint
					.Where(x => x.IsUsableForOrder)
					.Sum(x => x.Point)
				: 0;
		}
	}
	/// <summary>利用可能期間前期間限定ポイント合計（仮ポイントは除く）</summary>
	public decimal LoginUserLimitedTermPointUnusableTotal
	{
		get
		{
			return (this.LoginUserPoint != null)
				? this.LoginUserPoint.LimitedTermPoint
					.Where(x => (x.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP)
						&& (DateTime.Now <= x.EffectiveDate))
					.Sum(x => x.Point)
				: 0;
		}
	}
	/// <summary>期間限定ポイント合計</summary>
	public decimal LoginUserLimitedTermPointTotal
	{
		get
		{
			return (this.LoginUserPoint != null)
				? this.LoginUserPoint.LimitedTermPoint
					.Where(x => (x.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP))
					.Sum(x => x.Point)
				: 0;
		}
	}

	/// <summary>ログインユーザに有効なターゲットリスト群</summary>
	public string[] LoginUserHitTargetListIds
	{
		get { return (string[])Session[Constants.SESSION_KEY_LOGIN_USER_HIT_TARGET_LIST_IDS] ?? new string[0]; }
		set { Session[Constants.SESSION_KEY_LOGIN_USER_HIT_TARGET_LIST_IDS] = value; }
	}
	/// <summary>ユーザーポイント</summary>
	public w2.App.Common.Option.UserPointObject LoginUserPoint
	{
		get { return (w2.App.Common.Option.UserPointObject)Session[Constants.SESSION_KEY_LOGIN_USER_POINT]; }
		set { Session[Constants.SESSION_KEY_LOGIN_USER_POINT] = value; }
	}
	/// <summary>ログインユーザー管理レベル</summary>
	public string UserManagementLevelId
	{
		get { return (string)Session[Constants.SESSION_KEY_LOGIN_USER_MANAGEMENT_LEVEL_ID]; }
		set { Session[Constants.SESSION_KEY_LOGIN_USER_MANAGEMENT_LEVEL_ID] = value; }
	}

	/// <summary>ユーザー台湾地域情報</summary>
	public Dictionary<string, Dictionary<string, string>> UserTwDistrictDict
	{
		get { return Constants.TW_DISTRICT_DICT; }
	}

	/// <summary>ターゲットページ</summary>
	/// <summary>ターゲットページ</summary>
	public string SessionParamTargetPage
	{
		get
		{
			if (Session[Constants.SESSION_KEY_PARAM] == null) return null;
			if ((Session[Constants.SESSION_KEY_PARAM] is Hashtable)
				&& (((Hashtable)Session[Constants.SESSION_KEY_PARAM]).Contains(Constants.SESSION_KEY_TARGET_PAGE)))
			{
				return (string)((Hashtable)Session[Constants.SESSION_KEY_PARAM])[Constants.SESSION_KEY_TARGET_PAGE];
			}
			else if (Session[Constants.SESSION_KEY_PARAM] is IInput)
			{
				return (string)((IInput)Session[Constants.SESSION_KEY_PARAM]).DataSource[Constants.SESSION_KEY_TARGET_PAGE];
			}
			return null;
		}
		set
		{
			if (Session[Constants.SESSION_KEY_PARAM] == null) return;
			if (Session[Constants.SESSION_KEY_PARAM] is Hashtable)
			{
				((Hashtable)Session[Constants.SESSION_KEY_PARAM])[Constants.SESSION_KEY_TARGET_PAGE] = value;
			}
			else if (Session[Constants.SESSION_KEY_PARAM] is IInput)
			{
				((IInput)Session[Constants.SESSION_KEY_PARAM]).DataSource[Constants.SESSION_KEY_TARGET_PAGE] = value;
			}
		}
	}

	#region プロパティ：クライアント判断系
	/// <summary>PCかどうか</summary>
	public bool IsPc { get { return (this.IsSmartPhone == false); } }
	/// <summary>スマートフォンかどうか</summary>
	public bool IsSmartPhone
	{
		get { return (Constants.SMARTPHONE_OPTION_ENABLED) && (SmartPhoneUtility.CheckSmartPhone(this.Request.UserAgent)); }
	}
	/// <summary>モバイルかどうか</summary>
	public bool IsMobile { get { return CheckIsMobile(this.Request); } }
	#endregion

	#region プロパティ：オプション系
	/// <summary>EFOオプション有効か</summary>
	public bool IsEfoOptionEnabled
	{
		get { return ((string.IsNullOrEmpty(Constants.EFO_OPTION_PROJECT_NO) == false) && (Constants.GLOBAL_OPTION_ENABLE == false)); }
	}
	#endregion

	/// <summary>店舗ID</summary>
	public string ShopId
	{
		get { return m_shopId; }
		set { m_shopId = value; }
	}
	private string m_shopId = Constants.CONST_DEFAULT_SHOP_ID;

	/// <summary>ページ参照日</summary>
	public DateTime ReferenceDateTime
	{
		get { return (DateTime?)Session[Constants.SESSION_KEY_REFERENCE_DATETIME] ?? DateTime.Now; }
		set { Session[Constants.SESSION_KEY_REFERENCE_DATETIME] = value; }
	}
	/// <summary>ページ参照会員ランク</summary>
	public MemberRankModel ReferenceMemgbeRankModel
	{
		get { return (MemberRankModel)Session[Constants.SESSION_KEY_REFERENCE_MEMBER_RANK]; }
		set { Session[Constants.SESSION_KEY_REFERENCE_MEMBER_RANK] = value; }
	}
	/// <summary>ページ参照ターゲットリスト</summary>
	public string[] ReferenceTargetList
	{
		get { return (string[])Session[Constants.SESSION_KEY_REFERENCE_TARGET_LIST] ?? new string[0]; }
		set { Session[Constants.SESSION_KEY_REFERENCE_TARGET_LIST] = value; }
	}

	/// <summary>ログイン必須判定</summary>
	public bool NeedsLogin { get; set; }
	/// <summary>リピートプラスONEリダイレクト必須判定</summary>
	public bool RepeatPlusOneNeedsRedirect { get; set; }
	protected string ViewStateUserKey
	{
		get { return this.Page.ViewStateUserKey; }
		set { this.Page.ViewStateUserKey = value; }
	}
	/// <summary>Post Params</summary>
	public Dictionary<string, string> PostParams
	{
		get
		{
			var result = Request.Form.AllKeys.Contains(ECPayConstants.PARAM_MERCHANT_ID)
				? this.Request.Form.AllKeys.ToDictionary(key => key, key => Request.Form[key])
				: new Dictionary<string, string>();
			return result;
		}
	}
	/// <summary>Cart index when call back from EcPay</summary>
	public int CartIndexWhenCallBackFromEcPay
	{
		get
		{
			var cartIndex = 0;
			var result = int.TryParse(Request[Constants.REQUEST_KEY_CART_INDEX], out cartIndex)
				? cartIndex
				: -1;
			return result;
		}
	}
	/// <summary>Amazon連携かどうか</summary>
	public bool IsAmazonLogin
	{
		get
		{
			return ((Constants.AMAZON_LOGIN_OPTION_ENABLED || Constants.AMAZON_PAYMENT_OPTION_ENABLED)
					&& (Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL] != null));
		}
	}
	/// <summary>AmazonPay(CV2)かつゲスト購入か</summary>
	public bool IsAmazonCv2Guest
	{
		get
		{
			return (this.IsAmazonLogin
				&& Constants.AMAZON_PAYMENT_CV2_ENABLED
				&& (this.IsLoggedIn == false));
		}
	}

	/// <summary>リクエスト</summary>
	protected HttpRequest Request { get { return this.Context.Request; } }
	/// <summary>レスポンス</summary>
	protected HttpResponse Response { get { return this.Context.Response; } }
	/// <summary>セッション</summary>
	protected HttpSessionState Session { get { return this.Context.Session; } }
	/// <summary>HTTPコンテキスト</summary>
	protected HttpContext Context { get; set; }
	/// <summary>ランディングページかどうか</summary>
	public bool IsLandingPage { get; set; }
	/// <summary>Has authentication code</summary>
	public bool HasAuthenticationCode
	{
		get { return (bool?)Session[Constants.SESSION_KEY_HAS_AUTHENTICATION_CODE] ?? false; }
		set { Session[Constants.SESSION_KEY_HAS_AUTHENTICATION_CODE] = value; }
	}
	/// <summary>Authentication code</summary>
	public string AuthenticationCode
	{
		get { return StringUtility.ToEmpty(Session[Constants.SESSION_KEY_AUTHENTICATION_CODE]); }
		set { Session[Constants.SESSION_KEY_AUTHENTICATION_CODE] = value; }
	}
	/// <summary>Is social login</summary>
	public bool IsSocialLogin
	{
		get
		{
			var result = (this.IsAmazonLoggedIn
				|| ((SocialLoginModel)Session[Constants.SESSION_KEY_SOCIAL_LOGIN_MODEL] != null)
				|| (SessionManager.PayPalLoginResult != null)
				|| (string.IsNullOrEmpty(SessionManager.LineProviderUserId) == false));

			return result;
		}
	}
	/// <summary>Custom validator control disabled information list</summary>
	public string[] CustomValidatorControlDisabledInformationList
	{
		get { return (string[])ViewState["custom_validator_control_disabled_information_list"] ?? new string[0]; }
		set { ViewState["custom_validator_control_disabled_information_list"] = value; }
	}
	/// <summary>Custom validator control information list</summary>
	public string[] CustomValidatorControlInformationList
	{
		get { return (string[])ViewState["custom_validator_control_information_list"] ?? new string[0]; }
		set { ViewState["custom_validator_control_information_list"] = value; }
	}
}
