/*
=========================================================================================================
  Module      : 注文LP入力フロープロセス(OrderLandingInputProcess.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using Amazon.Pay.API.WebStore.CheckoutSession;
using Extensions;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.ABTest.Util;
using w2.App.Common.Amazon;
using w2.App.Common.Amazon.Helper;
using w2.App.Common.Amazon.Util;
using w2.App.Common.AmazonCv2;
using w2.App.Common.Auth.RakutenIDConnect;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.ECPay;
using w2.App.Common.Order.Payment.Paygent;
using w2.App.Common.Order.Payment.TriLinkAfterPay.Helper;
using w2.App.Common.Product;
using w2.App.Common.User;
using w2.App.Common.User.SocialLogin.Helper;
using w2.App.Common.Util;
using w2.App.Common.Web.WebCustomControl;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Extensions;
using w2.Common.Helper;
using w2.Common.Web;
using w2.Domain.ContentsLog;
using w2.Domain.LandingPage;
using w2.Domain.MemberRank;
using w2.Domain.Payment;
using w2.Domain.Product;
using w2.Domain.SubscriptionBox;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

/// <summary>
/// OrderLandingInputProcess の概要の説明です
/// </summary>
public class OrderLandingInputProcess : OrderLandingProcess
{
	#region ラップ済コントロール宣言
	private WrappedLinkButton WlbNext { get { return GetWrappedControl<WrappedLinkButton>("lbNext"); } }
	public WrappedButton WbtnCheckCartAndProductOptionValueSetting { get { return GetWrappedControl<WrappedButton>("btnCheckCartAndProductOptionValueSetting"); } }
	public WrappedHtmlGenericControl WhgcConstraintErrorMessage { get { return GetWrappedControl<WrappedHtmlGenericControl>(this.FirstRpeaterItem, "constraintErrorMessage"); } }
	public WrappedHtmlGenericControl WhgcShippingAddressBookErrorMessage { get { return GetWrappedControl<WrappedHtmlGenericControl>(this.FirstRpeaterItem, "shippingAddressBookErrorMessage"); } }
	public WrappedHtmlGenericControl WhgcOwnerAddressBookErrorMessage { get { return GetWrappedControl<WrappedHtmlGenericControl>(this.FirstRpeaterItem, "ownerAddressBookErrorMessage"); } }
	public WrappedCheckBox WcbOwnerMailFlg { get { return GetWrappedControl<WrappedCheckBox>(this.FirstRpeaterItem, "cbOwnerMailFlg"); } }
	public WrappedCheckBox WcbShipToOwnerAddress { get { return GetWrappedControl<WrappedCheckBox>(this.FirstRpeaterItem, "cbShipToOwnerAddress"); } }
	public WrappedUpdatePanel WupCartListUpdatePanel { get { return GetWrappedControl<WrappedUpdatePanel>("upCartListUpdatePanel"); } }
	public WrappedUpdatePanel WupPasswordUpdatePanel { get { return GetWrappedControl<WrappedUpdatePanel>("upPasswordUpdatePanel"); } }

	// ログイン関係
	/// <summary>ログインメールアドレス</summary>
	public WrappedTextBox WtbLoginIdInMailAddr
	{
		get
		{
			var wrappedControl = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbLoginIdInMailAddr");
			return (wrappedControl.InnerControl != null) ? wrappedControl : GetWrappedControl<WrappedTextBox>("tbLoginIdInMailAddr");
		}
	}
	/// <summary>ログインID</summary>
	public WrappedTextBox WtbLoginId
	{
		get
		{
			var wrappedControl = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbLoginId");
			return (wrappedControl.InnerControl != null) ? wrappedControl : GetWrappedControl<WrappedTextBox>("tbLoginId");
		}
	}
	/// <summary>パスワード</summary>
	public WrappedTextBox WtbPassword
	{
		get
		{
			var wrappedControl = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbPassword");
			return (wrappedControl.InnerControl != null) ? wrappedControl : GetWrappedControl<WrappedTextBox>("tbPassword");
		}
	}
	/// <summary>ログインIDを保存</summary>
	protected WrappedCheckBox WcbAutoCompleteLoginIdFlg
	{
		get
		{
			var wrappedControl = GetWrappedControl<WrappedCheckBox>(this.FirstRpeaterItem, "cbAutoCompleteLoginIdFlg");
			return (wrappedControl.InnerControl != null) ? wrappedControl : GetWrappedControl<WrappedCheckBox>("cbAutoCompleteLoginIdFlg", false);
		}
	}
	/// <summary>ログインエラーメッセージ</summary>
	public WrappedHtmlGenericControl WdLoginErrorMessage { get { return GetWrappedControl<WrappedHtmlGenericControl>(this.FirstRpeaterItem, "dLoginErrorMessage"); } }
	/// <summary>ログインボタン</summary>
	public WrappedLinkButton WlbLogin { get { return GetWrappedControl<WrappedLinkButton>(this.FirstRpeaterItem, "lbLogin"); } }

	public WrappedTextBox WtbOwnerMailAddr { get { return GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerMailAddr"); } }
	public WrappedTextBox WtbOwnerMailAddrConf { get { return GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerMailAddrConf"); } }
	public WrappedHiddenField WhfSocialLoginJson { get { return GetWrappedControl<WrappedHiddenField>("hfSocialLoginJson"); } }
	public WrappedCheckBox WcbUserRegister { get { return GetWrappedControl<WrappedCheckBox>(this.FirstRpeaterItem, "cbUserRegister", false); } }
	public WrappedHtmlGenericControl WdvUserPassword { get { return GetWrappedControl<WrappedHtmlGenericControl>(this.FirstRpeaterItem, "dvUserPassword"); } }

	public WrappedTextBox WtbOwnerName1 { get { return GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerName1"); } }
	public WrappedTextBox WtbOwnerName2 { get { return GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerName2"); } }
	public WrappedTextBox WtbOwnerNameKana1 { get { return GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerNameKana1"); } }
	public WrappedTextBox WtbOwnerNameKana2 { get { return GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerNameKana2"); } }
	public WrappedDropDownList WddlOwnerBirthYear { get { return GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlOwnerBirthYear"); } }
	public WrappedDropDownList WddlOwnerBirthMonth { get { return GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlOwnerBirthMonth"); } }
	public WrappedDropDownList WddlOwnerBirthDay { get { return GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlOwnerBirthDay"); } }
	public WrappedRadioButtonList WrblOwnerSex { get { return GetWrappedControl<WrappedRadioButtonList>(this.FirstRpeaterItem, "rblOwnerSex", Constants.FLG_USER_SEX_UNKNOWN); } }
	public WrappedTextBox WtbOwnerZip1 { get { return GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerZip1"); } }
	public WrappedTextBox WtbOwnerZip2 { get { return GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerZip2"); } }
	public WrappedDropDownList WddlOwnerAddr1 { get { return GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlOwnerAddr1"); } }
	public WrappedTextBox WtbOwnerAddr2 { get { return GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerAddr2"); } }
	public WrappedTextBox WtbOwnerAddr3 { get { return GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerAddr3"); } }
	public WrappedTextBox WtbOwnerTel1_1 { get { return GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel1_1"); } }
	public WrappedTextBox WtbOwnerTel1_2 { get { return GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel1_2"); } }
	public WrappedTextBox WtbOwnerTel1_3 { get { return GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel1_3"); } }
	public WrappedTextBox WtbOwnerZip { get { return GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerZip"); } }
	public WrappedTextBox WtbOwnerTel1 { get { return GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel1"); } }
	public WrappedDropDownList WddlOwnerCountry { get { return GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlOwnerCountry"); } }
	public WrappedTextBox WtbOwnerTel1Global { get { return GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel1Global"); } }
	public WrappedHtmlGenericControl WsOwnerZipError { get { return GetWrappedControl<WrappedHtmlGenericControl>(this.FirstRpeaterItem, "sOwnerZipError"); } }
	public WrappedHtmlGenericControl WsShippingZipError { get { return GetWrappedControl<WrappedHtmlGenericControl>(this.FirstRpeaterItem, "sShippingZipError"); } }

	public WrappedRepeater WrProductList { get { return GetWrappedControl<WrappedRepeater>("rProductList"); } }
	public WrappedDropDownList WddlProductList { get { return GetWrappedControl<WrappedDropDownList>("ddlProductList"); } }

	public WrappedDropDownList WddlProductSet { get { return GetWrappedControl<WrappedDropDownList>("ddlProductSet"); } }

	/// <summary>Paidy token hidden field control</summary>
	public WrappedHiddenField WhfPaidyTokenId { get { return GetWrappedControl<WrappedHiddenField>(this.FirstRpeaterItem, "hfPaidyTokenId"); } }
	/// <summary>Paidy pay seleced hidden field control</summary>
	public WrappedHiddenField WhfPaidyPaySelected { get { return GetWrappedControl<WrappedHiddenField>(this.FirstRpeaterItem, "hfPaidyPaySelected"); } }

	public WrappedButton WdbtnUserPasswordReType { get { return GetWrappedControl<WrappedButton>(this.FirstRpeaterItem, "btnUserPasswordReType"); } }
	public WrappedLiteral WdlUserPassword { get { return GetWrappedControl<WrappedLiteral>(this.FirstRpeaterItem, "lUserPassword"); } }
	public WrappedLiteral WdlUserPasswordConf { get { return GetWrappedControl<WrappedLiteral>(this.FirstRpeaterItem, "lUserPasswordConf"); } }
	public WrappedTextBox WtbUserPassword { get { return GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbUserPassword"); } }
	public WrappedTextBox WtbUserPasswordConf { get { return GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbUserPasswordConf"); } }

	public WrappedLabel WlbAuthenticationStatus { get { return GetWrappedControl<WrappedLabel>(this.FirstRpeaterItem, "lbAuthenticationStatus"); } }
	public WrappedLabel WlbAuthenticationStatusGlobal { get { return GetWrappedControl<WrappedLabel>(this.FirstRpeaterItem, "lbAuthenticationStatusGlobal"); } }
	public WrappedLabel WlbAuthenticationMessage { get { return GetWrappedControl<WrappedLabel>(this.FirstRpeaterItem, "lbAuthenticationMessage"); } }
	public WrappedLabel WlbAuthenticationMessageGlobal { get { return GetWrappedControl<WrappedLabel>(this.FirstRpeaterItem, "lbAuthenticationMessageGlobal"); } }
	#endregion

	/// <summary>ページアクセスタイプ</summary>
	public enum ChooseProductType
	{
		/// <summary>商品選択しない</summary>
		DoNotChoose,
		/// <summary>チェックボックス</summary>
		CheckBox,
		/// <summary>ドロップダウンリスト</summary>
		DropDownList,
	}

	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="caller">呼び出し元</param>
	/// <param name="viewState">ビューステート</param>
	/// <param name="context">コンテキスト</param>
	public OrderLandingInputProcess(object caller, StateBag viewState, HttpContext context)
		: base(caller, viewState, context)
	{
	}

	/// <summary>
	/// ページ初期化
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public new void Page_Init(object sender, EventArgs e)
	{
		// ランディング入力ページ絶対パス取得
		this.LandingCartInputAbsolutePath = Request.Url.AbsolutePath;
		// ランディング入力ページ最後に書き込み時刻取得
		this.LandingCartInputLastWriteTime = new FileInfo(Request.PhysicalPath).LastWriteTime.ToString();

		if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED)
		{
			CheckSiteDomainAndRedirectWithPostData();
		}

		// HTTPS通信チェック（HTTP通信の場合、HTTPS通信へ変更）
		CheckHttps(this.LandingCartInputUrl + Request.Url.Query);

		// AmazonPayでの注文完了後は、セッション情報が残っている可能性があるためセッションクリア
		if ((string)Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] == Constants.PAGE_FRONT_ORDER_COMPLETE)
		{
			Session.Remove(Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK);
			Session.Remove(this.LadingCartNextPageForCheck);
			Session.Remove(AmazonConstants.SESSION_KEY_AMAZON_MODEL);
			Session.Remove(AmazonConstants.SESSION_KEY_AMAZON_ADDRESS);
		}

		// 確認画面からのブラウザバック制御
		if (GetNextUrlForCheck() == Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM)
		{
			var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM)
				.AddParam(Constants.REQUEST_KEY_RETURN_URL, Request.Url.AbsolutePath)
				.CreateUrl();
			Response.Redirect(url);
		}

		// AmazonPay 自動連携解除アラート
		if (this.ShouldDisplayAmazonPayAutoUnlinkAlert)
		{
			this.ErrorMessages.Add(-1, -1, WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AMAZON_PAY_AUTO_UNLINK));
		}

		LoadLadingCartSession();
	}

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void Page_Load(object sender, EventArgs e)
	{
		this.IsLandingPage = true;
		this.ErrorMessage = "";
		SessionManager.IsCartListLp = false;

		//------------------------------------------------------
		// コンポーネント初期化
		//------------------------------------------------------
		InitComponents();

		//------------------------------------------------------
		// トークン有効期限チェック
		//-----------------------------------------------------
		var creditTokenCheck = this.CartList.CheckCreditTokenExpired(true);
		if (creditTokenCheck == false)
		{
			this.ErrorMessage += WebMessages.GetMessages(WebMessages.ERRMSG_CREDIT_TOKEN_EXPIRED);
		}

		// 検索結果レイヤーから住所を確定後、ポストバック発生で住所検索のエラーメッセージが再表示されてしまうためPostBack時に再度消す
		ResetAddressSearchResultErrorMessage(Constants.GIFTORDER_OPTION_ENABLED, this.WrCartList, true);

		if (!IsPostBack)
		{
			if (this.IsCmsLandingPage)
			{
				DdlProductSet();

				if ((Constants.CART_LIST_LP_OPTION == false)
					|| (this.LandingPageDesignModel.IsCartListLp == false))
				{
					// パラメータからの直指定している場合
					if (Request[Constants.REQUEST_KEY_LANDING_PAGE_BRANCH_NO] != null)
					{
						this.CartList = new CartObjectList(
							"",
							this.IsPc ? Constants.FLG_ORDER_ORDER_KBN_PC : Constants.FLG_ORDER_ORDER_KBN_SMARTPHONE,
							true,
							memberRankId: this.MemberRankId);
						var productSet = this.ValidLandingPageProductSetModels
							.FirstOrDefault(ps => ps.BranchNo.ToString() == Request[Constants.REQUEST_KEY_LANDING_PAGE_BRANCH_NO].ToString());
						ChangeCartBySelectProductSet(
							(productSet != null)
								? productSet.BranchNo.ToString()
								: this.ValidLandingPageProductSetModels.First().BranchNo.ToString());
					}
					// セッション内に選択した商品セットが存在する場合
					else if (Session[this.LadingCartProductSetSelectSessionKey] != null)
					{
						if (CheckCartCompareProductSet(Session[this.LadingCartProductSetSelectSessionKey].ToString()) && (Request["reset"] == null))
						{
							if (this.WddlProductSet.InnerControl != null)
							{
								this.WddlProductSet.SelectedValue = Session[this.LadingCartProductSetSelectSessionKey].ToString();
							}
							ChangeCartBySelectProductSet(Session[this.LadingCartProductSetSelectSessionKey].ToString());
						}
						else
						{
							// カート内容と選択されている商品セットの内容に差異がある場合、またはリセット処理の場合はカートの再生成
							this.CartList = new CartObjectList(
								"",
								this.IsPc ? Constants.FLG_ORDER_ORDER_KBN_PC : Constants.FLG_ORDER_ORDER_KBN_SMARTPHONE,
								true,
								memberRankId: this.MemberRankId);
							ChangeCartBySelectProductSet(Session[this.LadingCartProductSetSelectSessionKey].ToString());
						}
					}
					// 選択している商品セットが存在しない場合は先頭を指定
					else if (Session[this.LadingCartProductSetSelectSessionKey] == null)
					{
						this.CartList = new CartObjectList(
							"",
							this.IsPc ? Constants.FLG_ORDER_ORDER_KBN_PC : Constants.FLG_ORDER_ORDER_KBN_SMARTPHONE,
							true,
							memberRankId: this.MemberRankId);
						ChangeCartBySelectProductSet(this.ValidLandingPageProductSetModels.First().BranchNo.ToString());
					}
				}
				else
				{
					SessionManager.IsCartListLp = true;

					if (this.Session[this.LadingCartSessionKey] != null)
					{
						this.CartList = (CartObjectList)this.Session[this.LadingCartSessionKey];
					}

					// Only add the shopping cart landing page product set the first time
					if (SessionManager.IsOnlyAddProductSetCartLpFirstTime)
					{
						ChangeCartBySelectProductSet(this.ValidLandingPageProductSetModels.First().BranchNo.ToString());
						SessionManager.IsOnlyAddProductSetCartLpFirstTime = false;
					}

					AddProductToCartFromRequest();

					// カート投入URLアクション系でリダイレクトしてきた場合
					if (string.IsNullOrEmpty(Constants.SESSION_KEY_ERROR_FOR_ADD_CART) == false)
					{
						this.ErrorMessages.Add(StringUtility.ToEmpty(Session[Constants.SESSION_KEY_ERROR_FOR_ADD_CART]));
						Session[Constants.SESSION_KEY_ERROR_FOR_ADD_CART] = null;
					}

					// カート投入URLアクション
					if (Request[Constants.REQUEST_KEY_CART_ACTION] != null)
					{
						// POST・GETに合わせてリクエスト情報取得
						var addCartHttpRequest = new AddCartHttpRequest(Request, true);
						// リクエスト情報からカートに商品投入
						AddProductToCartFromRequest(addCartHttpRequest);

						// カート投入URLの指定方法に誤りがある場合メッセージ追加
						this.ErrorMessages.Add(addCartHttpRequest.ErrorMessages);

						// カートへ遷移する
						// カートから商品削除後、ブラウザの戻るボタンで再カート投入されないよう回避するためと、一部商品エラーでもパラメタ残さないようにするため。
						Session[Constants.SESSION_KEY_ERROR_FOR_ADD_CART] = this.ErrorMessages.Get();
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST_LP);
					}
					else if (Request[Constants.REQUEST_KEY_CART_SUBSCRIPTION_BOX_COURSE_ID] != null)
					{
						AddProductsToCartListForSubscriptionBox(
							Request[Constants.REQUEST_KEY_CART_SUBSCRIPTION_BOX_COURSE_ID]);
						Session[Constants.SESSION_KEY_ERROR_FOR_ADD_CART] = this.ErrorMessages.Get();
					}

				// Update shipping to the user's default order setting if the page is CartListLp
				if ((this.CartList.Items.Count > 0)
						&& this.CartList.CanSetUserDefaultOrderSettingShipping)
					{
						var cartShipping = this.CartList.Items[0].Shippings[0];
						cartShipping.ShippingAddrKbn = string.Empty;
						this.CartList.Items[0].Shippings[0].UpdateShippingAddr(cartShipping);
					}

					SessionManager.CartListLp = this.CartList;
				}

				// ログイン済み、かつ確認画面以外より遷移する場合
				// 配送先区分のデフォルト値をログインユーザーのデフォルト配送先に設定する
				if (this.IsLoggedIn && (GetNextUrlForCheck() == null))
				{
					var userShippingKbn = this.UserDefaultOrderSettingShipping;
					this.CartList.UserId = this.LoginUserId;
					foreach (var cart in this.CartList.Items)
					{
						cart.Shippings[0].ShippingAddrKbn = userShippingKbn;
						this.CartList.SetDefaultOrderShippingForUserDefaultOrder(userShippingKbn);
					}
				}
			}
			//旧LP画面でのログイン時、商品リスト・カートリストを再構築する
			else if ((GetNextUrlForCheck() != Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM)
				&& (Request["reset"] == null)
				&& (this.CartList.Items.Count > 0)
				&& (this.IsCartListLp == false))
			{
				// 商品リスト作成
				CreateProductList();

				// When changing item quantity on the display, the value will set to cart product in the cart list.
				// So update product quantity for product list when changing item quantity on the display.
				UpdateProductListFromCartList();

				// カートリスト作成
				CreateCartList();
			}

			var hasRecommendProduct = false;
			// レコメンド追加時になにか不具合があり、バックアップがある場合復元する
			if (this.IsAddRecmendItem && this.IsBackupLandingCartSession)
			{
				RestoreCartFromSession();
			}
			else if (this.IsAddRecmendItem && (this.IsCartListLp == false))
			{
				hasRecommendProduct = true;
			}

			// カートに商品がない場合のみ商品を追加する
			//	例外としてリセット処理の場合はカートオブジェクトリストを再構築をする
			if ((GetNextUrlForCheck() != Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM)
				&& ((Request["reset"] != null)
					|| (this.CartList.Items.Count == 0)
					|| hasRecommendProduct
					|| this.IsCartListLp))
			{
				//------------------------------------------------------
				// リセット処理（サイト管理者向け）
				//------------------------------------------------------
				if ((Request["reset"] != null)
					|| hasRecommendProduct)
				{
					this.CartList = new CartObjectList(
						"",
						this.IsPc ? Constants.FLG_ORDER_ORDER_KBN_PC : Constants.FLG_ORDER_ORDER_KBN_SMARTPHONE,
						true,
						memberRankId: this.MemberRankId);
					Session[this.LadingCartSessionKey] = this.CartList;
				}

				// 商品リスト作成
				CreateProductList();

				// カートリスト作成
				CreateCartList();
			}

			// 商品購入制限チェック
			ProductOrderLimitItemDelete();

			if ((this.CartList.Items.Count == 0)
				&& (this.IsCartListLp == false))
			{
				RedirectErrorPage(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_LANDING_VALID_PRODUCT_NOTINCART_ERROR));
			}

			if ((this.ProductList.Length == 0) && (this.IsCmsLandingPage == false))
			{
				// 一つでも商品が投入出来な場合、ランディングページ用カート保持用セッションを削除しエラー画面へ遷移する
				// （ランディングページ用カート保持用セッションを削除するのは再度同じページを開いた際にカート投入出来た商品だけが表示される可能性を防ぐため）
				Session.Remove(Constants.SESSION_KEY_CART_LIST_LANDING);
				RedirectErrorPage(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_LANDING_NOT_VALID_PRODUCT_ERROR));
			}

			if (this.IsSelectFromList)
			{
				// 商品リスト設定（Repeater）
				SetProductList();
			}
			else if (this.IsDropDownList)
			{
				// 商品リストDDL設定（DropDownList）
				SetProductListDropDown();
			}

			// 定期会員フラグ設定
			SetFixedPurchaseMemberFlgForCartObject(this.CartList.Items);

			if (this.IsLoggedIn)
			{
				// ユーザー管理レベルにより制限されているかどうかチェック
				foreach (var productid in this.ProductList)
				{
					CheckUserLevelIsLimited(productid.ProductId, productid.ProductJointName);
				}

				// ログインユーザのランキング設定
				this.CartList.Items.ForEach(item => item.UpdateMemberRank(this.LoginUserId));
			}

			if (Constants.RAKUTEN_LOGIN_ENABLED)
			{
				// 楽天IDConnectで新規会員登録ボタンから遷移ならユーザー情報書き換え
				if (Request[Constants.REQUEST_KEY_RAKUTEN_REGIST] == Constants.FLG_RAKUTEN_USER_REGIST)
				{
					this.CartList.Owner = null;
				}
			}

			// Set delivery to the user's default order setting if the page is CartListLp
			if (this.IsCartListLp)
			{
				// ゲスト会員で注文完了後の遷移で注文者情報の初期化
				if ((this.IsLoggedIn == false) && (SessionManager.IsRedirectFromLandingCartConfirm == false))
				{
					this.CartList.Owner = null;
				}

				var notHasShippingAddrKbn = this.CartList.Items.Any(cart =>
					cart.Shippings.Any(shipping => string.IsNullOrEmpty(shipping.ShippingAddrKbn)));

				if (notHasShippingAddrKbn
					&& this.CartList.Items.Count > 0)
				{
					this.CartList.Items[0].Shippings[0].ShippingAddrKbn = this.UserDefaultOrderSettingShipping;
				}
			}

			//------------------------------------------------------
			// 配送情報入力系処理
			//------------------------------------------------------
			// 注文者情報作成
			CreateOrderOwner();

			// ユーザー区分（＝注文者区分）セット
			this.UserKbn = this.CartList.Owner.OwnerKbn;

			// 配送先情報作成
			CreateOrderShipping();

			if (this.IsCartListLp)
			{
				// NOTE:カートリストランディングページの場合、
				// 定期注文の次回配送日計算で、配送先情報の利用が必要なので、
				// 配送先情報のチェックと設定を行う
				InitializeCartOrderShippingToCartOwnerShippingIfNoZip();
			}

			// カート注文メモ作成
			CreateOrderMemo();

			CreateOrderExtend();

			// 配送情報入力画面初期処理（共通）
			InitComponentsDispOrderShipping(e);

			//------------------------------------------------------
			// 支払い情報入力系処理
			//------------------------------------------------------
			// カート決済情報作成（デフォルトで配送希望は希望するようにする）
			CreateCartPayment();

			//  クレジットカード番号再表示の際は、上位４桁と下位４桁以外を空文字設定、セキュリティコードを空文字設定
			AdjustCreditCardNo();

			// Amazon Pay利用不可場合セッションのAmazonアカウント情報破棄
			if (this.CanUseAmazonPayment() == false)
			{
				this.AmazonPaySessionAmazonModel = null;
			}
			else if (Request[AmazonCv2Constants.REQUEST_KEY_AMAZON_CHECKOUT_SESSION_ID] != null)
			{
				this.AmazonCheckoutSessionId = Request[AmazonCv2Constants.REQUEST_KEY_AMAZON_CHECKOUT_SESSION_ID].Split('#')[0];
				this.AmazonCheckoutSession = new AmazonCv2ApiFacade().GetCheckoutSession(this.AmazonCheckoutSessionId);
				if (this.AmazonCheckoutSession.Success && (this.AmazonCheckoutSession.Buyer != null))
				{
					var amazonInput = new AmazonAddressInput(this.AmazonCheckoutSession.ShippingAddress, this.AmazonCheckoutSession.Buyer.Email);
					var billingInput = new AmazonAddressInput(this.AmazonCheckoutSession.BillingAddress, this.AmazonCheckoutSession.Buyer.Email);

					var bilingsAddress = AmazonAddressParser.Parse(billingInput);
					this.AmazonPaySessionShippingAddress = AmazonAddressParser.Parse(amazonInput);

					var amazonModel = new AmazonModel
					{
						Email = this.AmazonCheckoutSession.Buyer.Email,
						Name = this.AmazonCheckoutSession.Buyer.Name,
						UserId = this.AmazonCheckoutSession.Buyer.BuyerId
					};

					var amazonOwner = new AmazonAddressModel
					{
						Name = amazonModel.Name,
						Name1 = string.IsNullOrEmpty(amazonModel.GetName1() + amazonModel.GetName2())
							? amazonModel.Name
							: amazonModel.GetName1(),
						Name2 = amazonModel.GetName2(),
						NameKana = Constants.PAYMENT_AMAZON_NAMEKANA1 + Constants.PAYMENT_AMAZON_NAMEKANA2,
						NameKana1 = Constants.PAYMENT_AMAZON_NAMEKANA1,
						NameKana2 = Constants.PAYMENT_AMAZON_NAMEKANA2,
						MailAddr = amazonModel.Email,
					};

					this.AmazonPaySessionAmazonModel = amazonModel;
					this.AmazonPaySessionOwnerAddress = amazonOwner;
					this.AmazonPaySessionPaymentDescriptor = this.AmazonCheckoutSession.PaymentPreferences[0].PaymentDescriptor;

					// 「請求先住所取得オプションがTRUE」 かつ 「未ログイン(ゲストユーザ)」 かつ「注文確認画面へ遷移したことない」場合に注文者情報を設定
					if (Constants.AMAZONPAYMENTCV2_USEBILLINGASOWNER_ENABLED
						&& (this.IsLoggedIn == false)
						&& (SessionManager.IsMovedOnOrderConfirm == false))
					{
						var owner = new CartOwner
						{
							Name = bilingsAddress.Name,
							Name1 = bilingsAddress.Name1,
							Name2 = bilingsAddress.Name2,
							Zip1 = bilingsAddress.Zip1,
							Zip2 = bilingsAddress.Zip2,
							Addr1 = bilingsAddress.Addr1,
							Addr2 = bilingsAddress.Addr2,
							Addr3 = bilingsAddress.Addr3,
							Addr4 = bilingsAddress.Addr4,
							AddrCountryIsoCode = Constants.GLOBAL_OPTION_ENABLE
								? Constants.COUNTRY_ISO_CODE_JP
								: string.Empty,
							Tel1_1 = bilingsAddress.Tel1,
							Tel1_2 = bilingsAddress.Tel2,
							Tel1_3 = bilingsAddress.Tel3,
							MailAddr = amazonModel.Email,
							MailFlg = (Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.mail_flg.default@@") == Constants.FLG_USER_MAILFLG_OK),
						};
						this.CartList.SetOwner(owner);
					}
				}
			}
			else if (Constants.AMAZON_PAYMENT_CV2_ENABLED
				&& ((this.AmazonPaySessionAmazonModel == null)
					|| (this.AmazonPaySessionOwnerAddress == null)
					|| (this.AmazonPaySessionPaymentDescriptor == null)
					|| (this.AmazonPaySessionShippingAddress == null)))
			{
				Session.Remove(AmazonConstants.SESSION_KEY_AMAZON_MODEL);
				Session.Remove(AmazonConstants.SESSION_KEY_AMAZON_ADDRESS);
			}

			// Update LoginUserPoint
			if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsLoggedIn)
			{
				this.LoginUserPoint = PointOptionUtility.GetUserPoint(this.LoginUserId);
			}

			if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED)
			{
				SetInformationReceivingStore(this.CartList);
			}

			if (string.IsNullOrEmpty(Request.QueryString[Constants.REQUEST_KEY_AB_TEST_ID]) && (SessionManager.IsRedirectFromLandingCartConfirm == false))
			{
				Session[Constants.REQUEST_KEY_AB_TEST_ID] = Session[Constants.SESSION_KEY_IS_REDIRECT_FROM_ABTEST_ALLOTOMENT] = string.Empty;
			}

			var fileName = Path.GetFileNameWithoutExtension(this.Request.Path);

			if (Constants.AB_TEST_OPTION_ENABLED && ((string.IsNullOrEmpty(Request.QueryString[Constants.REQUEST_KEY_AB_TEST_ID])) == false))
			{
				var abTestId = Request.QueryString[Constants.REQUEST_KEY_AB_TEST_ID];
				Session[Constants.REQUEST_KEY_AB_TEST_ID] = abTestId;

				//ABテストクエリパラメータの有効性検証
				if (string.IsNullOrEmpty(abTestId) == false)
				{
					var lp = new LandingPageService().GetPageByFileName(fileName);

					var errorNo = AbTestUtil.ValidateAbTest(abTestId, lp[0].PageId);
					if (errorNo != AbTestUtil.ValidateStatus.NoError) Response.Redirect(this.Request.Path);

					var abTestUrl = (string)Session[Constants.SESSION_KEY_IS_REDIRECT_FROM_ABTEST_ALLOTOMENT];
					if (string.IsNullOrEmpty(abTestUrl) == false)
					{
						Session[Constants.SESSION_KEY_IS_REDIRECT_FROM_ABTEST_ALLOTOMENT] = string.Empty;

						var landingPageModel = new LandingPageService().GetPageByFileName(fileName);
						CookieManager.SetCookie(abTestId, landingPageModel[0].PageId, Constants.PATH_ROOT_FRONT_PC);

						var contentsLog = new ContentsLogModel
						{
							AccessKbn = this.IsPc
								? Constants.FLG_CONTENTSLOG_ACCESS_KBN_PC
								: Constants.FLG_CONTENTSLOG_ACCESS_KBN_SP,
							ReportType = Constants.FLG_CONTENTSLOG_REPORT_TYPE_PV,
							ContentsId = abTestId + "-" + lp[0].PageId,
							ContentsType = Constants.FLG_CONTENTSLOG_CONTENTS_TYPE_ABTEST,
							Date = DateTime.Now,
						};

						new ContentsLogService().Insert(contentsLog);
					}
				}
			}

			// 画面表示
			Reload(e);

			// 入力フォームにログインIDに設定
			SetLoginIdToInputForm(UserCookieManager.GetLoginIdFromCookie());

			AffiliateTagDataBind();

			// 確認画面から戻ってきた場合は、会員登録チェックボックスの選択状態を復元する
			if (GetNextUrlForCheck() != null)
			{
				WdvUserPassword.Visible = WcbUserRegister.Checked = (this.RegisterUser != null);
			}

			FocusOnSpecifiedControls();
		}
		else
		{
			// トークンが入力されていたら入力画面を切り替える
			SwitchDisplayForCreditTokenInput(this.WrCartList);

			if (this.IsCmsLandingPage)
			{
				// ランディングページ商品セットモデルから商品を設定
				var branchNo = StringUtility.ToEmpty(this.Session[this.LadingCartProductSetSelectSessionKey]);
				var productSet =
					this.ValidLandingPageProductSetModels.FirstOrDefault(ps => (ps.BranchNo.ToString() == branchNo))
					?? this.ValidLandingPageProductSetModels.First();
				ProductSetItemSetting(productSet);
			}
		}

		// アマゾンログイン時、配送情報更新
		var amazonAddress = (AmazonAddressModel)Session[AmazonConstants.SESSION_KEY_AMAZON_ADDRESS];
		if ((Session[AmazonConstants.SESSION_KEY_AMAZON_ADDRESS] != null)
			&& (this.CartList.Items.Count > 0)
			&& ((this.CartList.Items[0].Shippings[0].Zip1 != amazonAddress.Zip1)
				|| (this.CartList.Items[0].Shippings[0].Zip2 != amazonAddress.Zip2)
				|| (this.CartList.Items[0].Shippings[0].Addr1 != amazonAddress.Addr1)
				|| (this.CartList.Items[0].Shippings[0].Addr2 != amazonAddress.Addr2)
				|| (this.CartList.Items[0].Shippings[0].Addr3 != amazonAddress.Addr3)
				|| (this.CartList.Items[0].Shippings[0].Addr4 != amazonAddress.Addr4)))
		{
			this.CartList.Items[0].Shippings[0].Zip1 = amazonAddress.Zip1;
			this.CartList.Items[0].Shippings[0].Zip2 = amazonAddress.Zip2;
			this.CartList.Items[0].Shippings[0].Addr1 = amazonAddress.Addr1;
			this.CartList.Items[0].Shippings[0].Addr2 = amazonAddress.Addr2;
			this.CartList.Items[0].Shippings[0].Addr3 = amazonAddress.Addr3;
			this.CartList.Items[0].Shippings[0].Addr4 = amazonAddress.Addr4;
			this.CartList.Items[0].Shippings[0].ShippingCountryIsoCode = amazonAddress.CountryCode;
			Reload(e);
		}

		if (this.ChooseProduct == ChooseProductType.CheckBox)
		{
			foreach (RepeaterItem item in this.WrProductList.Items)
			{
				var index = item.ItemIndex;
				var wcbPurchase = GetWrappedControl<WrappedCheckBox>(item, "cbPurchase", true);
				var landingCartProduct = this.ProductList[index];

				if (wcbPurchase.InnerControl != null)
				{
					landingCartProduct.Selected = wcbPurchase.Checked;
				}
			}
		}

		if (this.ChooseProduct == ChooseProductType.DropDownList)
		{
			var selectedIndex = int.Parse(this.WddlProductList.SelectedValue);
			for (var index = 0; index < this.ProductList.Length; index++)
			{
				var landingCartProduct = this.ProductList[index];
				landingCartProduct.Selected = (index == selectedIndex);
			}
		}

		// ユーザー管理レベルにより制限されているかどうかチェック
		if (this.IsLoggedIn)
		{
			foreach (var productid in this.ProductList)
			{
				CheckUserLevelIsLimited(productid.ProductId, productid.ProductJointName);
			}
		}

		// Display Shop Data
		if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED) DisplayShopData();

		// パスワード入力部分の表示切替
		var userSessionPassword = (string)Session[Constants.SESSION_KEY_LP_PASSWORD];
		var userSessionPasswordConf = (string)Session[Constants.SESSION_KEY_LP_PASSWORDCONF];
		DisplaySwitchPasswordInputField(userSessionPassword, userSessionPasswordConf);

		// Display Checkbox Regist CreditCard
		DisplayCheckboxRegistCreditCard();

		// Display control authentication
		DisplayControlAuthentication(this.LandingPageDesignModel);

		// カスタムバリデータ属性値更新
		UpdateAttributeValueForCustomValidator();
	}

	/// <summary>
	/// パスワード部分の表示切り替え
	/// </summary>
	/// <param name="password">パスワード</param>
	/// <param name="passwordConf">パスワード確認用</param>
	public void DisplaySwitchPasswordInputField(string password, string passwordConf)
	{
		if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordConf)) return;
		if (password != passwordConf) return;
		this.WtbUserPassword.Visible = false;
		this.WtbUserPasswordConf.Visible = false;

		var hiddenPassword = StringUtility.ChangeToAster(password);

		this.WdlUserPassword.Text = hiddenPassword;
		this.WdlUserPasswordConf.Text = hiddenPassword;

		this.WdbtnUserPasswordReType.Visible = true;

		this.WupPasswordUpdatePanel.DataBind();
		this.WupPasswordUpdatePanel.Update();
	}

	/// <summary>
	/// ランディングページ商品セットモデルから商品を設定
	/// </summary>
	/// <param name="model">ランディングページ商品セットモデル</param>
	public void ProductSetItemSetting(LandingPageProductSetModel model)
	{
		this.ProductId1 = (model.Products.Length > 0) ? string.Format("{0},{1}", model.Products[0].ProductId, model.Products[0].VariationId) : "";
		this.ProductId2 = (model.Products.Length > 1) ? string.Format("{0},{1}", model.Products[1].ProductId, model.Products[1].VariationId) : "";
		this.ProductId3 = (model.Products.Length > 2) ? string.Format("{0},{1}", model.Products[2].ProductId, model.Products[2].VariationId) : "";
		this.ProductId4 = (model.Products.Length > 3) ? string.Format("{0},{1}", model.Products[3].ProductId, model.Products[3].VariationId) : "";
		this.ProductId5 = (model.Products.Length > 4) ? string.Format("{0},{1}", model.Products[4].ProductId, model.Products[4].VariationId) : "";
		this.ProductId6 = (model.Products.Length > 5) ? string.Format("{0},{1}", model.Products[5].ProductId, model.Products[5].VariationId) : "";
		this.ProductId7 = (model.Products.Length > 6) ? string.Format("{0},{1}", model.Products[6].ProductId, model.Products[6].VariationId) : "";
		this.ProductId8 = (model.Products.Length > 7) ? string.Format("{0},{1}", model.Products[7].ProductId, model.Products[7].VariationId) : "";
		this.ProductId9 = (model.Products.Length > 8) ? string.Format("{0},{1}", model.Products[8].ProductId, model.Products[8].VariationId) : "";
		this.ProductId10 = (model.Products.Length > 9) ? string.Format("{0},{1}", model.Products[9].ProductId, model.Products[9].VariationId) : "";

		this.ProductId1Quantity = (model.Products.Length > 0) ? model.Products[0].Quantity : 1;
		this.ProductId2Quantity = (model.Products.Length > 1) ? model.Products[1].Quantity : 1;
		this.ProductId3Quantity = (model.Products.Length > 2) ? model.Products[2].Quantity : 1;
		this.ProductId4Quantity = (model.Products.Length > 3) ? model.Products[3].Quantity : 1;
		this.ProductId5Quantity = (model.Products.Length > 4) ? model.Products[4].Quantity : 1;
		this.ProductId6Quantity = (model.Products.Length > 5) ? model.Products[5].Quantity : 1;
		this.ProductId7Quantity = (model.Products.Length > 6) ? model.Products[6].Quantity : 1;
		this.ProductId8Quantity = (model.Products.Length > 7) ? model.Products[7].Quantity : 1;
		this.ProductId9Quantity = (model.Products.Length > 8) ? model.Products[8].Quantity : 1;
		this.ProductId10Quantity = (model.Products.Length > 9) ? model.Products[9].Quantity : 1;

		this.Product1BuyType = (model.Products.Length > 0)
			? model.Products[0].BuyType
			: LandingPageConst.BUY_TYPE_NORMAL;
		this.Product2BuyType = (model.Products.Length > 1)
			? model.Products[1].BuyType
			: LandingPageConst.BUY_TYPE_NORMAL;
		this.Product3BuyType = (model.Products.Length > 2)
			? model.Products[2].BuyType
			: LandingPageConst.BUY_TYPE_NORMAL;
		this.Product4BuyType = (model.Products.Length > 3)
			? model.Products[3].BuyType
			: LandingPageConst.BUY_TYPE_NORMAL;
		this.Product5BuyType = (model.Products.Length > 4)
			? model.Products[4].BuyType
			: LandingPageConst.BUY_TYPE_NORMAL;
		this.Product6BuyType = (model.Products.Length > 5)
			? model.Products[5].BuyType
			: LandingPageConst.BUY_TYPE_NORMAL;
		this.Product7BuyType = (model.Products.Length > 6)
			? model.Products[6].BuyType
			: LandingPageConst.BUY_TYPE_NORMAL;
		this.Product8BuyType = (model.Products.Length > 7)
			? model.Products[7].BuyType
			: LandingPageConst.BUY_TYPE_NORMAL;
		this.Product9BuyType = (model.Products.Length > 8)
			? model.Products[8].BuyType
			: LandingPageConst.BUY_TYPE_NORMAL;
		this.Product10BuyType = (model.Products.Length > 9)
			? model.Products[9].BuyType
			: LandingPageConst.BUY_TYPE_NORMAL;

		this.AddCartKbn = (string.IsNullOrEmpty(model.SubscriptionBoxCourseId) == false)
			? Constants.AddCartKbn.SubscriptionBox
			: model.Products.Any(p => (p.BuyType == LandingPageConst.BUY_TYPE_FIXEDPURCHASE))
			? Constants.AddCartKbn.FixedPurchase
			: Constants.AddCartKbn.Normal;
		this.SubscriptionBoxCourseId = model.SubscriptionBoxCourseId;
	}

	/// <summary>
	/// 商品リスト設定（Repeater）
	/// </summary>
	protected void SetProductList()
	{
		this.WrProductList.DataSource = this.ProductList;
		this.WrProductList.DataBind();
	}

	/// <summary>有効なCMSランディングページ商品セット選択肢モデル</summary>
	public LandingPageProductSetModel[] ValidLandingPageProductSetModels
	{
		get
		{
			var models = this.LandingPageDesignModel.ProductSets
				.Where(ps => ps.ValidFlg == LandingPageConst.PRODUCT_SET_VALID_FLG_VALID)
				.ToArray();
			return models;
		}
	}
	/// <summary>CMSランディングページモデル</summary>
	public LandingPageDesignModel LandingPageDesignModel { get; set; }

	/// <summary>
	/// 商品リスト作成
	/// </summary>
	private void CreateProductList()
	{
		// 画面で設定されている商品ID取得
		string[][] productIds
			= {
				this.ProductId1.Split(','),
				this.ProductId2.Split(','),
				this.ProductId3.Split(','),
				this.ProductId4.Split(','),
				this.ProductId5.Split(','),
				this.ProductId6.Split(','),
				this.ProductId7.Split(','),
				this.ProductId8.Split(','),
				this.ProductId9.Split(','),
				this.ProductId10.Split(',')
			};
		bool[] defaultChecked
			= {
				this.DefaultChecked1,
				this.DefaultChecked2,
				this.DefaultChecked3,
				this.DefaultChecked4,
				this.DefaultChecked5,
				this.DefaultChecked6,
				this.DefaultChecked7,
				this.DefaultChecked8,
				this.DefaultChecked9,
				this.DefaultChecked10
			};
		int[] addQuantitys
			={
				 this.ProductId1Quantity,
				 this.ProductId2Quantity,
				 this.ProductId3Quantity,
				 this.ProductId4Quantity,
				 this.ProductId5Quantity,
				 this.ProductId6Quantity,
				 this.ProductId7Quantity,
				 this.ProductId8Quantity,
				 this.ProductId9Quantity,
				 this.ProductId10Quantity,
			 };
		var productBuyTypes = new[]{
			this.Product1BuyType,
			this.Product2BuyType,
			this.Product3BuyType,
			this.Product4BuyType,
			this.Product5BuyType,
			this.Product6BuyType,
			this.Product7BuyType,
			this.Product8BuyType,
			this.Product9BuyType,
			this.Product10BuyType,
		};
		var productList = new List<LandingCartProduct>();
		string shippingId = null;
		var isSelected = false;
		for (var index = 0; index < 10; index++)
		{
			var productId = productIds[index];

			// 商品IDが未設定
			if (productId[0] == "") continue;

			// カートに投入するか
			// 商品選択しない		：全部選択
			// チェックボックス形式	：設定どおり、ただし一つも選択されていない場合は、1つ目を選択状態にする
			// 一つだけ選択する形式	：複数選択される設定の場合は先勝ち、ただし一つも選択されていない場合は、1つ目を選択状態にする
			var addCart = ((this.ChooseProduct == ChooseProductType.DoNotChoose)
				|| (this.IsCheckBox && defaultChecked[index])
				|| (this.IsChooseOnlyOne && defaultChecked[index] && (isSelected == false))
				|| ((defaultChecked.Any(set => set) == false) && (index == 0)));

			isSelected = (isSelected || addCart);

			var addQuantity = addQuantitys[index];

			var isForFixedpurchase = this.IsCmsLandingPage
				? (productBuyTypes[index] == LandingPageConst.BUY_TYPE_FIXEDPURCHASE)
				: (this.AddCartKbn == Constants.AddCartKbn.FixedPurchase);

			// ランディングカート用商品情報取得
			var landingCartProduct = GetLandingCartProduct(
				index,
				this.ShopId,
				productId[0],
				(productId.Length > 1) ? productId[1] : productId[0],
				addCart,
				addQuantity,
				isForFixedpurchase);

			// 取得できなかった
			if (landingCartProduct == null) continue;

			// ユーザー管理レベルにより制限されているかどうかチェック
			CheckUserLevelIsLimited(productId[0], landingCartProduct.ProductJointName);

			// 配送種別チェック
			if (shippingId == null)
			{
				shippingId = (string)landingCartProduct.Product[Constants.FIELD_PRODUCT_SHIPPING_TYPE];
			}
			else if ((this.ChooseProduct != ChooseProductType.DoNotChoose)
				&& (shippingId != (string)landingCartProduct.Product[Constants.FIELD_PRODUCT_SHIPPING_TYPE]))
			{
				// 選択式のLPカートで配送種別が別の場合
				// リストに追加しない
				continue;
			}
			productList.Add(landingCartProduct);
		}

		if (string.IsNullOrEmpty(this.SubscriptionBoxCourseId) == false && this.IsCheckBox)
		{
			var subscriptionBox = new SubscriptionBoxService().GetByCourseId(this.SubscriptionBoxCourseId);

			var subscriptionBoxFirstDefaultItem = (subscriptionBox.IsNumberTime)
				? subscriptionBox.DefaultOrderProducts
					.Where(item => item.Count == 1).ToArray()
				: subscriptionBox.DefaultOrderProducts
					.Where(item => (item.TermSince <= DateTime.Now)
						&& (item.TermUntil >= DateTime.Now))
					.ToArray();

			if (subscriptionBoxFirstDefaultItem
				.Any(item => item.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_VALID))
			{
				foreach (var product in productList)
				{
					product.Selected = true;
				}
			}
		}

		this.ProductList = productList.ToArray();
	}

	/// <summary>
	/// ランディングカート用商品情報取得
	/// </summary>
	/// <param name="index">インデックス</param>
	/// <param name="shopId">店舗ID</param>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	/// <param name="addCart">カート投入区分</param>
	/// <param name="addQuantity">投入数</param>
	/// <param name="isForFixedpurchase">定期購入用かどうか</param>
	/// <returns></returns>
	private LandingCartProduct GetLandingCartProduct(
		int index,
		string shopId,
		string productId,
		string variationId,
		bool addCart,
		int addQuantity,
		bool isForFixedpurchase)
	{
		// 商品情報取得
		var product = GetProduct(shopId, productId, variationId);
		if (product.Count == 0)
		{
			return null;
		}

		// ランディングカート用商品情報作成
		var landingCartProduct = new LandingCartProduct
		{
			Index = index,
			Product = product[0].ToHashtable(),
			ProductCount = (addQuantity == 0) ? 1 : addQuantity,
			Selected = addCart,
			IsFixedPurchase = isForFixedpurchase,
		};

		return landingCartProduct;
	}

	/// <summary>
	/// 商品セット選択によるカートの内容変更
	/// </summary>
	/// <param name="branchNo">商品セット選択肢の枝番</param>
	/// <param name="isChangeProductSet">True: changed productset else false</param>
	private void ChangeCartBySelectProductSet(string branchNo, bool isChangeProductSet = false)
	{
		var productSet = this.ValidLandingPageProductSetModels
			.FirstOrDefault(ps => ps.BranchNo.ToString() == branchNo);

		var selectBranchNo = "";
		if (productSet != null)
		{
			ProductSetItemSetting(productSet);
			CreateProductList();

			// When changing item quantity on the display, the value will set to cart product in the cart list.
			// So update product quantity for product list when changing item quantity on the display.
			UpdateProductListFromCartList();

			CreateCartList();
			selectBranchNo = productSet.BranchNo.ToString();
			if (isChangeProductSet
				&& (string.IsNullOrEmpty(productSet.SubscriptionBoxCourseId) == false)
				&& (productSet.Products.Length == 0))
			{
				Session.Remove(Constants.SESSION_KEY_CART_LIST_LANDING);
				RedirectErrorPage(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_LANDING_VALID_OUT_OF_PERIOD_ERROR));
			}
		}
		else
		{
			selectBranchNo = this.ValidLandingPageProductSetModels.First().BranchNo.ToString();
		}

		Session[this.LadingCartProductSetSelectSessionKey] = selectBranchNo;
		Session[this.LadingCartSessionKey] = this.CartList;

		if (this.WddlProductSet.InnerControl != null)
		{
			this.WddlProductSet.SelectedValue = selectBranchNo;
		}
	}

	/// <summary>
	/// カートリスト作成
	/// </summary>
	private void CreateCartList()
	{
		// HACK: this.CartList.HasFixedPurchase はカートリストLP時のみ利用。通常LP時は商品セット変更時に正しく判定できないので利用しない。
		var hasFixedPurchase = this.IsCartListLp
			? this.CartList.HasFixedPurchase
			: this.ProductList.Any(p => p.IsFixedPurchase && p.Selected);
		var hasFixedPurchaseBeforeRecreate = (bool)(Session[this.LandingCartIsFixedPurchaseSessionKey] ?? hasFixedPurchase);

		foreach (var pl in this.ProductList)
		{
			foreach (CartObject cart in this.CartList)
			{
				var cp = cart.GetProductEither(pl.ShopId, pl.ProductId, pl.VariationId);
				if (cp == null) continue;

				var addCartKbn = (this.AddCartKbn == Constants.AddCartKbn.FixedPurchase)
					? (pl.IsFixedPurchase)
						? Constants.AddCartKbn.FixedPurchase
						: Constants.AddCartKbn.Normal
					: this.AddCartKbn;

				cart.RemoveProduct(
					pl.ShopId,
					pl.ProductId,
					pl.VariationId,
					addCartKbn,
					cp.ProductSaleId,
					cp.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues());
			}
		}

		ContentsLogModel contentsLog;
		var lp = new LandingPageService().GetPageByFileName(Path.GetFileNameWithoutExtension(this.Request.Path));
		if (lp.Any())
		{
			// コンテンツログ作成
			contentsLog = new ContentsLogModel
			{
				ContentsType = Constants.FLG_CONTENTSLOG_CONTENTS_TYPE_LANDINGCART,
				ContentsId = lp[0].PageId,
			};
		}
		else
		{
			contentsLog = null;
		}

		// 選択された商品をカートに投入する
		foreach (var product in this.ProductList.Where(i => i.Selected))
		{
			// エラー内容に重複がなければ追加する
			var addCartKbn = (this.AddCartKbn == Constants.AddCartKbn.FixedPurchase)
				? (product.IsFixedPurchase)
				? Constants.AddCartKbn.FixedPurchase
				: Constants.AddCartKbn.Normal
				: this.AddCartKbn;
			var errorMessage = AddProductToLandingCart(
				product.ShopId,
				product.ProductId,
				product.VariationId,
				addCartKbn,
				product.ProductCount,
				new List<string>(),
				contentsLog,
				this.SubscriptionBoxCourseId,
				(this.IsLoggedIn ? this.LoginUserId : string.Empty));

			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				var productErrorMessage = string.Format("({0}){1}", product.ProductJointName, errorMessage);
				if (this.ErrorMessages.Get(-3, -3).Contains(productErrorMessage) == false)
				{
					this.ErrorMessages.Add(-3, -3, productErrorMessage);
				}
			}

			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				((OrderCartPageLanding)this.Page).ExecAddProductError(product, errorMessage);
			}
		}

		var noneProductCart = new List<CartObject>();
		noneProductCart.AddRange(this.CartList.Cast<CartObject>().Where(cart => cart.Items.Count == 0));

		if (this.CartList.Items.Count > 1)
		{
			foreach (var cart in noneProductCart)
			{
				this.CartList.DeleteCartVurtual(cart);
			}
		}

		// チェックがすべて外れた状態からチェックを入れた時カート追加
		foreach (var cartitem in this.CartList.Items)
		{
			if ((cartitem.Items.Count > 0) && (cartitem.Payment == null))
			{
				SetCartInformation();
			}
		}

		// 空カートが存在する場合、新しくカート追加されたと判断
		if (noneProductCart.Count > 0)
		{
			SetCartInformation();
		}

		this.CartList.IsLandingCart = true;
		this.CartList.LandingCartInputAbsolutePath = this.LandingCartInputAbsolutePath;

		if (this.CartList.Items.All(item => (string.IsNullOrEmpty(item.SubscriptionBoxCourseId) == false))
			&& (this.SubscriputionBoxProductListModify != null))
		{
			foreach (var product in this.SubscriputionBoxProductListModify)
			{
				AddProductToLandingCart(
					product.ShopId,
					product.ProductId,
					product.VariationId,
					Constants.AddCartKbn.SubscriptionBox,
					product.ItemQuantity,
					new List<string>(),
					null,
					this.CartList.Items[0].SubscriptionBoxCourseId);
			}
		}

		Session[this.LandingCartIsFixedPurchaseSessionKey] = hasFixedPurchase;

		var path = new UrlCreator(Constants.PAGE_FRONT_AMAZON_LANDING_PAGE_CALLBACK)
			.AddParam(AmazonConstants.REQUEST_KEY_AMAZON_STATE, Request.Url.AbsolutePath)
			.CreateUrl();
		var callbackPath = string.Format("{0}#CartList", path);
		this.AmazonRequest = hasFixedPurchase
			? AmazonCv2Redirect.SignPayloadForReccuring(this.CartList.PriceCartListTotal, callbackPath: callbackPath)
			: AmazonCv2Redirect.SignPayloadForOneTime(callbackPath);

		this.WhfAmazonCv2Payload.DataBind();
		this.WhfAmazonCv2Signature.DataBind();

		// 継続課金の要否が変更された場合、AmazonPay連携は解除してアラートをだす。
		// （このまま注文してもチェックアウトセッションと注文内容が食い違ってしまい決済エラーとなる。）
		if (this.IsAmazonLoggedIn && (hasFixedPurchase != hasFixedPurchaseBeforeRecreate))
		{
			// 商品セットの選択肢をセッションに保持しておく
			Session[this.LadingCartProductSetSelectSessionKey] = this.WddlProductSet.SelectedValue;

			UnlinkAmazonPaySession();

			// パラメータ追加してリロード
			var next = new UrlCreator(this.Request.Url.AbsolutePath, true)
				.ReplaceParam(Constants.REQUEST_KEY_AMAZON_PAY_AUTO_UNLINK_ALERT, "1")
				.WithUrlFragment("#CartList")
				.CreateUrl();
			Response.Redirect(next);
		}
	}

	/// <summary>
	/// 商品リストDDL設定（DropDownList）
	/// </summary>
	private void SetProductListDropDown()
	{
		if (this.WddlProductList.InnerControl == null) return;
		this.WddlProductList.DataSource = this.ProductListCollection;
		this.WddlProductList.DataBind();

		// 商品選択
		this.WddlProductList.SelectedValue = this.ProductList.First(item => item.Selected).Index.ToString();
	}

	/// <summary>
	/// コンポーネント初期処理
	/// </summary>
	private void InitComponents()
	{
		this.IsVisible_UserPassword = true;
		// ソーシャルログイン情報補完
		if (Constants.SOCIAL_LOGIN_ENABLED)
		{
			var socialLogin = (SocialLoginModel)Session[Constants.SESSION_KEY_SOCIAL_LOGIN_MODEL];
			if ((socialLogin != null))
			{
				this.WhfSocialLoginJson.Value = socialLogin.RawResponse;
				this.IsVisible_UserPassword = false;
			}
			else
			{
				// セッション情報のクリア
				Session.Remove(Constants.SESSION_KEY_SOCIAL_LOGIN_MODEL);
			}
		}

		if (Constants.RAKUTEN_LOGIN_ENABLED)
		{
			var rakutenLogin = (RakutenIDConnectActionInfo)Session[Constants.SESSION_KEY_RAKUTEN_ID_CONNECT_ACTION_INFO];
			if (rakutenLogin != null)
			{
				// ActionTypeがLoginでもログイン済みではない可能性があるので（ログインに失敗した・ログイン画面から戻ってきたなど）
				// ログインが完了していなければ（=Userがnullであれば）ログインボタンを表示する
				var isRakutenLoggedIn = ((rakutenLogin.Type == ActionType.Login) && (rakutenLogin.User != null));
				this.IsVisible_UserPassword = (isRakutenLoggedIn == false);
			}
		}

		// 配送情報入力系初期処理
		InitComponentsOrderShipping();

		// 支払情報入力系初期処理
		InitComponentsOrderPayment();
	}

	/// <summary>
	/// 商品セット選択肢リストDDL設定（DropDownList）
	/// </summary>
	private void DdlProductSet()
	{
		if (this.WddlProductSet.InnerControl == null) return;

		switch (this.ValidLandingPageProductSetModels.Length)
		{
			case 0:
				this.WddlProductSet.Visible = false;
				break;

			case 1:
				this.WddlProductSet.Visible = false;
				this.Session[this.LadingCartProductSetSelectSessionKey] = this.ValidLandingPageProductSetModels.First().BranchNo.ToString();
				break;

			default:
				this.WddlProductSet.DataSource = this.ProductSetCollection;
				this.WddlProductSet.DataBind();
				break;
		}
	}

	/// <summary>
	/// 現在のカート内容と最新の有効な商品セット選択肢と比較
	/// </summary>
	/// <param name="branchNo">商品セット選択肢の枝番</param>
	/// <returns>一致:true 不一致:false</returns>
	private bool CheckCartCompareProductSet(string branchNo)
	{
		var productSet = this.ValidLandingPageProductSetModels
			.FirstOrDefault(ps => ps.BranchNo.ToString() == branchNo);

		if (productSet == null) return false;

		if ((this.CartList.HasFixedPurchase && (productSet.HasAnyProductForFixedPurchase == false)) ||
			((this.CartList.HasFixedPurchase == false) && productSet.HasAnyProductForFixedPurchase)) return false;

		bool productsComparison;
		if (this.IsNotChoose == false)
		{
			productsComparison = productSet.Products
				.Select(p => this.CartList.Items.SelectMany(co => co.Items)
					.Where(cp => (cp.IsNovelty == false))
					.Any(cp => ((cp.ProductId == p.ProductId) && (cp.VariationId == p.VariationId))))
				.All(result => result);
		}
		else
		{
			productsComparison = this.CartList.Items.SelectMany(co => co.Items)
				.Where(cp => ((cp.IsBundle == false) && (cp.IsNovelty == false)))
				.Select(cp => productSet.Products.Any(p => ((p.ProductId == cp.ProductId) && (p.VariationId == cp.VariationId))))
				.All(result => result);
		}

		return productsComparison;
	}

	/// <summary>
	/// エラーページへ遷移
	/// </summary>
	/// <param name="strErrorMessage">エラーメッセージ</param>
	private void RedirectErrorPage(string strErrorMessage)
	{
		Session[Constants.SESSION_KEY_ERROR_MSG] = strErrorMessage;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
	}

	/// <summary>
	/// 入力フォームにログインIDを設定
	/// </summary>
	/// <param name="loginId">Cookieから取得したログインID</param>
	private void SetLoginIdToInputForm(string loginId)
	{
		if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED)
		{
			this.WtbLoginIdInMailAddr.Text = loginId;
		}
		else
		{
			this.WtbLoginId.Text = loginId;
		}
		this.WcbAutoCompleteLoginIdFlg.Checked = (loginId != "");
	}

	/// <summary>
	/// 商品セット選択ドロップダウンリスト 変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ddlProductSet_OnSelectedIndexChanged(object sender, System.EventArgs e)
	{
		this.CartList = new CartObjectList(
			"",
			this.IsPc ? Constants.FLG_ORDER_ORDER_KBN_PC : Constants.FLG_ORDER_ORDER_KBN_SMARTPHONE,
			true,
			memberRankId: this.MemberRankId);

		CartObjectList oldCartList = (CartObjectList)Session[this.LadingCartSessionKey];
		DeleteW2CartOldProduct(this.WddlProductSet.SelectedValue, oldCartList);

		Session[this.LadingCartSessionKey] = this.CartList;

		ChangeCartBySelectProductSet(this.WddlProductSet.SelectedValue, true);

		if (this.IsSelectFromList)
		{
			// 商品リスト設定（Repeater）
			SetProductList();
		}
		else if (this.IsDropDownList)
		{
			// 商品リストDDL設定（DropDownList）
			SetProductListDropDown();
		}

		if (this.CartList.Items.Count == 0)
		{
			this.WrCartList.Dispose();
			this.WrCartList.DataBind();
			return;
		}

		//------------------------------------------------------
		// 配送情報入力系処理
		//------------------------------------------------------
		// 注文者情報作成
		CreateOrderOwner();

		// ユーザー区分（＝注文者区分）セット
		this.UserKbn = this.CartList.Owner.OwnerKbn;

		// 配送先情報作成
		CreateOrderShipping();

		// カート注文メモ作成
		CreateOrderMemo();

		CreateOrderExtend();

		//------------------------------------------------------
		// 支払い情報入力系処理
		//------------------------------------------------------
		// カート決済情報作成（デフォルトで配送希望は希望するようにする）
		CreateCartPayment();

		//  クレジットカード番号再表示の際は、上位４桁と下位４桁以外を空文字設定、セキュリティコードを空文字設定
		AdjustCreditCardNo();

		// Amazon Pay利用不可場合セッションのAmazonアカウント情報破棄
		if (this.CanUseAmazonPayment() == false)
		{
			Session.Remove(AmazonConstants.SESSION_KEY_AMAZON_MODEL);
		}

		// 画面更新
		Reload(e, true);

		//カスタムバリデータ属性値更新
		UpdateAttributeValueForCustomValidator();
	}

	/// <summary>
	/// 商品セット選択による買い物かごマスタ変更前の商品内容削除
	/// </summary>
	/// <param name="branchNo">商品セット選択肢の枝番</param>
	/// <param name="oldCartList">カートリスト</param>
	private void DeleteW2CartOldProduct(string branchNo, CartObjectList oldCartList)
	{
		var productSet = this.ValidLandingPageProductSetModels
			.FirstOrDefault(ps => ps.BranchNo.ToString() == branchNo);

		if (productSet == null) return;

		foreach (CartObject cart in oldCartList.Items)
		{
			for (var i = cart.Items.Count - 1; i >= 0; i--)
			{
				CartProduct carItem = cart.Items[i];
				var cp = cart.GetProductEither(carItem.ShopId, carItem.ProductId, carItem.VariationId);
				if (cp == null) continue;

				var addCartKbn = (this.AddCartKbn == Constants.AddCartKbn.FixedPurchase)
					? (carItem.IsFixedPurchase)
						? Constants.AddCartKbn.FixedPurchase
						: Constants.AddCartKbn.Normal
					: this.AddCartKbn;

				cart.RemoveProduct(
					carItem.ShopId,
					carItem.ProductId,
					carItem.VariationId,
					addCartKbn,
					cp.ProductSaleId,
					cp.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues(),
					true);

				if (cart.Items.Count == 0) break;
			}
		}
	}

	/// <summary>
	/// カート再作成リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void lbCreateCart_Click(object sender, System.EventArgs e)
	{
		foreach (RepeaterItem item in this.WrProductList.Items)
		{
			var index = item.ItemIndex;
			var wtbProductCount = GetWrappedControl<WrappedTextBox>(item, "tbProductCount", "1");
			var wcbPurchase = GetWrappedControl<WrappedCheckBox>(item, "cbPurchase", true);

			var productList = this.ProductList[index];
			var productCountString = StringUtility.ToHankaku(wtbProductCount.Text);

			// 入力値エラー
			if (CheckInputQuantity(productCountString) != "")
			{
				wtbProductCount.Text = productList.ProductCount.ToString();
				continue;
			}

			var productCount = int.Parse(StringUtility.ToHankaku(wtbProductCount.Text));

			// 上限チェック
			if (productCount > productList.MaxSellQuantity)
			{
				wtbProductCount.Text = productList.ProductCount.ToString();
				continue;
			}

			productList.ProductCount = productCount;
			if (wcbPurchase.InnerControl != null)
			{
				productList.Selected = wcbPurchase.Checked;
			}
		}

		// カート再作成
		CreateCartList();

		// 画面更新
		Reload(e);
	}

	/// <summary>
	/// 再計算リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public override void lbRecalculate_Click(object sender, System.EventArgs e)
	{
		// バインドデータ更新
		lbNext_Click_OrderShipping_Owner(sender, e);
		lbNext_Click_OrderShipping_Shipping(sender, e);
		lbNext_Click_OrderShipping_Others(sender, e);

		//------------------------------------------------------
		// 入力中カートINDEXセット
		//------------------------------------------------------
		this.CurrentCartIndex = int.Parse(((LinkButton)sender).CommandArgument);

		//------------------------------------------------------
		// 入力チェックし、エラーがなければその他チェック・更新
		//------------------------------------------------------
		CheckAndSetInputData(e);

		//------------------------------------------------------
		// カートチェック（入力チェックNGの場合はカートチェックまではしない）
		//------------------------------------------------------
		if (this.ErrorMessages.Count == 0)
		{
			CheckCartData();
		}

		// 領収書情報取得
		if (Constants.RECEIPT_OPTION_ENABLED) SetReceipt();

		//------------------------------------------------------
		// 画面更新
		//------------------------------------------------------
		Reload(e);
	}

	/// <summary>
	/// 入力チェック＆オブジェクトへセット
	/// </summary>
	protected void CheckAndSetInputData(System.EventArgs e)
	{
		//------------------------------------------------------
		// 入力チェック
		//------------------------------------------------------
		// 商品選択方式がチェックボックス形式であるとき、購入商品が１つ以上選択されているかチェックする
		CheckExistPurchaseProductForCheckBox();

		// 商品/セット商品数入力項目チェック  ※セット購入制限チェックも行う
		CheckInputDataForCartList();

		// ポイント入力チェック
		if (Constants.W2MP_POINT_OPTION_ENABLED)
		{
			CheckInputDataForPoint();
		}

		//------------------------------------------------------
		// エラーがなければ入力値セット（商品数はDB反映する）
		//------------------------------------------------------
		this.IsSubscriptionBoxError =
			((this.CartList.Items
					.Where(cartList => string.IsNullOrEmpty(cartList.SubscriptionBoxErrorMsg) == false))
				.Any());
		if (this.ErrorMessages.Count == 0)
		{
			// 商品注文数設定
			SetInputDataForCartList();

			// 利用クーポンセット
			if (Constants.W2MP_COUPON_OPTION_ENABLED)
			{
				SetUseCouponData(this.WrCartList);
			}

			// 利用ポイント数セット
			if (Constants.W2MP_POINT_OPTION_ENABLED)
			{
				SetUsePointData(this.WrCartList);
			}

			// 再計算
			foreach (CartObject coCart in this.CartList.Items)
			{
				coCart.CalculateWithCartShipping();
			}
		}
	}

	/// <summary>
	/// 商品選択方式がチェックボックス形式であるとき、購入商品が１つ以上選択されているかチェックする
	/// </summary>
	private void CheckExistPurchaseProductForCheckBox()
	{
		// 商品選択方式がチェックボックスでないときは何もしない
		if (this.IsCheckBox == false) return;

		// 購入商品が１つも選択されていなければ、エラーメッセージを追記する
		var isChecked = this.WrProductList.Items.Cast<RepeaterItem>()
			.Any(product => GetWrappedControl<WrappedCheckBox>(product, "cbPurchase", true).Checked);

		if (isChecked == false)
		{
			this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_EXIST_PURCHASE_PRODUCT_FOR_LANDING_CART_INPUT));
		}
	}

	/// <summary>
	/// レジへ進むリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public override void lbNext_Click(object sender, System.EventArgs e)
	{
		this.RegisterUser = null;

		var userInfo = CreateInputData();

		if (WdbtnUserPasswordReType.HasInnerControl)
		{
			Session[Constants.SESSION_KEY_LP_PASSWORD] = userInfo.Password;
			Session[Constants.SESSION_KEY_LP_PASSWORDCONF] = userInfo.PasswordConf;
		}

		if ((string.IsNullOrEmpty(userInfo.Password)) == false)
		{

		}

		this.CartList.Items.ForEach(cart => cart.DeviceInfo = this.Request[Constants.REQUEST_GMO_DEFERRED_DEVICE_INFO]);

		// 別ブラウザや別タブでログアウト処理がされていた場合
		if (this.CartList.Owner == null) RedirectErrorPage(WebMessages.GetMessages(WebMessages.ERRMSG_LANDINGCARTINPUT_INPUT_SESSION_VANISHED));

		if (this.IsCmsLandingPage)
		{
			// 選択されている商品セットが存在しない、もしくはセッションに保持した選択内容とページ内の選択内容が異なる場合。注文には進まない。
			if ((Session[this.LadingCartProductSetSelectSessionKey] == null)
				|| ((Session[this.LadingCartProductSetSelectSessionKey] != null)
					&& (Session[this.LadingCartProductSetSelectSessionKey].ToString() != this.WddlProductSet.SelectedValue)
					&& (this.ValidLandingPageProductSetModels.Length > 1)))
			{
				SetEvent();
				RegisterStartupScript();

				return;
			}

			if ((this.LandingPageDesignModel.MailAddressConfirmFormUseFlg == LandingPageConst.MAIL_ADDRESS_CONFIRM_FORM_USE_FLG_OFF)
				&& (this.WtbOwnerMailAddrConf.InnerControl != null))
			{
				this.WtbOwnerMailAddrConf.Text = this.WtbOwnerMailAddr.Text;
			}
		}

		//------------------------------------------------------
		// 注文配送情報チェック及びオブジェクトへセット
		//------------------------------------------------------
		// Amazon Pay(CV1)の場合
		if (this.IsAmazonLoggedIn
			&& Constants.AMAZON_PAYMENT_OPTION_ENABLED
			&& (Constants.AMAZON_PAYMENT_CV2_ENABLED == false))
		{
			// バインドデータ更新
			var validationOk = lbNext_Click_OrderShipping_AmazonOwner(sender, e);
			validationOk &= lbNext_Click_OrderShipping_AmazonShipping(sender, e);
			validationOk &= lbNext_Click_OrderShipping_AmazonOthers(sender, e);
			if (validationOk == false)
			{
				SetEvent();
				return;
			}

			this.CartList.Items[0].AmazonOrderReferenceId = this.WhfAmazonOrderRefID.Value;
			this.CartList.Items[0].ExternalPaymentAgreementId = this.WhfAmazonBillingAgreementId.Value;

			this.CartList.Items[0].Payment.UpdateCartPayment(
			Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT,
			new PaymentService().GetPaymentName(this.ShopId, Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT),
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			null,
			false,
			string.Empty);
			this.CartList.Items[0].CalculateWithCartShipping();
		}
		else if (this.IsAmazonLoggedIn && Constants.AMAZON_PAYMENT_OPTION_ENABLED && Constants.AMAZON_PAYMENT_CV2_ENABLED)
		{
			// バインドデータ更新
			var validationOk = lbNext_Click_OrderShipping_AmazonOwner(sender, e);
			validationOk &= lbNext_Click_OrderShipping_AmazonShipping(sender, e);
			validationOk &= lbNext_Click_OrderShipping_AmazonOthers(sender, e);
			if (validationOk == false)
			{
				SetEvent();
				return;
			}

			this.CartList.Items[0].ExternalPaymentAgreementId = Request[AmazonCv2Constants.REQUEST_KEY_AMAZON_CHECKOUT_SESSION_ID];

			this.CartList.Items[0].Payment.UpdateCartPayment(
			Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2,
			new PaymentService().GetPaymentName(this.ShopId, Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2),
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			null,
			false,
			string.Empty);
			this.CartList.Items[0].CalculateWithCartShipping();
		}
		// 共通処理失敗時はこれ以降の処理を行わない
		else
		{
			var validationOk = lbNext_Click_OrderShipping_Owner(sender, e);
			validationOk &= lbNext_Click_OrderShipping_Shipping(sender, e);
			validationOk &= lbNext_Click_OrderShipping_Others(sender, e);
			if (validationOk == false)
			{
				SetEvent();
				RegisterStartupScript();
				DisplaySwitchPasswordInputField((string)Session[Constants.SESSION_KEY_LP_PASSWORD], (string)Session[Constants.SESSION_KEY_LP_PASSWORDCONF]);

				return;
			}
		}

		// Display shipping date error message if exising
		if (DisplayShippingDateErrorMessage())
		{
			SetEvent();
			RegisterStartupScript();

			return;
		}

		//商品チェックがすべて外されているときエラー
		foreach (var cartitem in this.CartList.Items)
		{
			if (cartitem.Items.Count == 0)
			{
				CheckExistPurchaseProductForCheckBox();
				Reload(e);
				return;
			}
		}

		//------------------------------------------------------
		// カートチェック
		//------------------------------------------------------
		CheckCart(e);

		//------------------------------------------------------
		// 商品付帯情報チェック
		//------------------------------------------------------
		CheckProductOptionValueSetting();

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			// 後付款(TriLink後払い)を利用できるかチェックする
			CheckPaymentForTriLinkAfterPay();
		}

		if ((this.ErrorMessages.Count > 0) || IsSubscriptionBoxError)
		{
			// エラーメッセージ表示のため画面リロード
			Reload(e);
			return;
		}

		//------------------------------------------------------
		// お支払い情報をカート情報へセット
		//------------------------------------------------------
		bool blHasError = false;
		if (this.IsAmazonLoggedIn == false)
		{
			blHasError = SetPayment(true);
		}

		// 領収書情報をカートにセット
		if (Constants.RECEIPT_OPTION_ENABLED && SetReceipt())
		{
			SetEvent();
			return;
		}

		if (blHasError) return;

		// Check Shipping Country Iso Code Can Order With Paidy Pay
		CheckShippingCountryIsoCodeCanOrderWithPaidyPay(this.CartList);

		// Check Country Iso Code Can Order With NP After Pay
		CheckCountryIsoCodeCanOrderWithNPAfterPay(this.CartList);

		// 配送不可エリアエラーの場合、配送不可エリアエラーメッセージを表示する
		if (OrderCommon.CheckUnavailableShippingArea(
			this.UnavailableShippingZip,
			this.CartList.Items[0].Shippings[0].HyphenlessZip))
		{
			OrderCommon.ShowUnavailableShippingErrorMessage(
			this.CartList,
			WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ZIPCODE_UNAVAILABLE_SHIPPING_AREA),
			this.WsOwnerZipError,
			this.WsShippingZipError);

			return;
		}

		//------------------------------------------------------
		// 画面遷移
		//------------------------------------------------------
		if ((blHasError == false)
			&& (this.ErrorMessages.Count == 0))
		{
			if (this.IsCmsLandingPage
				&& (this.LandingPageDesignModel.OrderConfirmPageSkipFlg == LandingPageConst.ORDER_CONFIRM_PAGE_SKIP_FLG_ON))
			{
				// 確認画面スキップフラグをセッション内に保持
				Session[this.LadingCartConfirmSkipFlgSessionKey] = this.LandingPageDesignModel.OrderConfirmPageSkipFlg;
			}

			if (this.WcbUserRegister.Checked && (this.IsAmazonLoggedIn == false))
			{
				if (Constants.LANDING_CART_USER_REGISTER_WHEN_ORDER_COMPLETE == false)
				{
					UserRegisterAndNextUrl();
					return;
				}
				this.RegisterUser = GetRegisterUser(CreateInputData());
				this.UserExtendInput = CreateUserExtendInput();
			}

			if (this.IsAmazonLoggedIn
				&& (this.IsUserRegistedForAmazon == false)
				&& (this.WcbUserRegisterForExternalSettlement.Checked
					|| (this.WcbUserRegister.Visible && this.WcbUserRegister.Checked))
				&& (ExistsUserWithSameAmazonEmailAddress == false))
			{
				this.IsVisible_UserPassword = false;
				if (Constants.LANDING_CART_USER_REGISTER_WHEN_ORDER_COMPLETE == false)
				{
					var loggedUser = Constants.AMAZONPAYMENTCV2_USEBILLINGASOWNER_ENABLED == false
						? UserRegisterAndNextUrlForAmazonPay()
						: UserRegisterForAmazonPayByOrderOwner();

					// LPカート確認画面へ遷移
					var nextUrl = this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM;

					// 画面遷移の正当性チェックのため遷移先ページURLを設定
					Session[this.LadingCartNextPageForCheck] = Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM;

					// ログインIDをCookieから削除 ※クッキーのログインIDが他者の可能性があるため
					UserCookieManager.CreateCookieForLoginId("", false);

					// ログイン成功アクション実行
					ExecLoginSuccessActionAndGoNextInner(loggedUser, nextUrl, UpdateHistoryAction.Insert);
					return;
				}
				var user = (AmazonModel)Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL];
				this.RegisterUser = CreateUserInputForAmazonPay(user).CreateModel();
			}

			// Process Check Paidy Token Id Exist
			if ((Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Direct)
				&& CheckPaidyTokenIdExist())
			{
				var errorMessage = WebMessages.GetMessages(
					WebMessages.ERRMSG_FRONT_PAIDY_TOKEN_ID_EXISTED_ERROR)
					.Replace("@@ 1 @@", this.WhfPaidyTokenId.Value);
				Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			// 画面遷移の正当性チェックのため遷移先ページURLを設定
			Session[this.LadingCartNextPageForCheck] = Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM;

			var url = new UrlCreator(
					this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM)
				.AddParam(Constants.REQUEST_KEY_RETURN_URL, Request.Url.AbsolutePath)
				.CreateUrl();
			Response.Redirect(url);
		}
		else
		{
			Reload(e);	// Reloadしないとエラーが画面へ表示されない
		}
	}

	/// <summary>
	/// 画面のイベント設定
	/// </summary>
	private void SetEvent()
	{
		var wlbCreateCart = GetWrappedControl<WrappedLinkButton>("lbCreateCart");

		foreach (RepeaterItem product in this.WrProductList.Items)
		{
			// テキストボックス向けイベント作成
			var createCartEventCreater = new TextBoxEventScriptCreator(wlbCreateCart);

			var wtbProductCount = GetWrappedControl<WrappedTextBox>(product, "tbProductCount", "1");
			var wcbPurchase = GetWrappedControl<WrappedCheckBox>(product, "cbPurchase");

			createCartEventCreater.AddScriptToControl(wtbProductCount);
			createCartEventCreater.AddScriptToControl(wcbPurchase);
		}

		//------------------------------------------------------
		// 各種表示初期化
		//------------------------------------------------------
		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			// ラップ済みコントロール宣言
			string usePoint = this.CartList.Items[riCart.ItemIndex].UsePoint.ToString();
			string couponCode = (this.CartList.Items[riCart.ItemIndex].Coupon != null) ? this.CartList.Items[riCart.ItemIndex].Coupon.CouponCode : "";
			var wrCart = GetWrappedControl<WrappedRepeater>(riCart, "rCart");
			var wrCartSetPromotion = GetWrappedControl<WrappedRepeater>(riCart, "rCartSetPromotion");
			var wtbOrderPointUse = GetWrappedControl<WrappedTextBox>(riCart, "tbOrderPointUse", usePoint);
			var wtbCouponCode = GetWrappedControl<WrappedTextBox>(riCart, "tbCouponCode", couponCode);
			var wcbUseSamePaymentAddrAsCart1 = GetWrappedControl<WrappedCheckBox>(riCart, "cbUseSamePaymentAddrAsCart1", false);
			var wrPayment = GetWrappedControl<WrappedRepeater>(riCart, "rPayment");
			var wlbRecalculateCart = GetWrappedControl<WrappedLinkButton>(riCart, "lbRecalculateCart");
			var wddlShippingMethod = CreateWrappedDropDownListShippingMethod(riCart);

			// テキストボックス向けイベント作成
			TextBoxEventScriptCreator eventCreator = new TextBoxEventScriptCreator(wlbRecalculateCart);
			eventCreator.RegisterInitializeScript(this.Page);

			foreach (RepeaterItem riProduct in wrCart.Items)
			{
				// 注文数入力ボックス設定
				var wtbProductCount = GetWrappedControl<WrappedTextBox>(riProduct, "tbProductCount", "1");
				var wrProductSet = GetWrappedControl<WrappedRepeater>(riProduct, "rProductSet");

				eventCreator.AddScriptToControl(wtbProductCount);

				// セット商品も
				if (wrProductSet.DataSource != null)
				{
					foreach (RepeaterItem riProductSet in wrProductSet.Items)
					{
						var wtbProductSetCount = GetWrappedControl<WrappedTextBox>(riProductSet, "tbProductSetCount", "0");
						eventCreator.AddScriptToControl(wtbProductSetCount);
						break;
					}
				}
			}

			// セットプロモーション商品の注文数入力テキストボックス設定
			foreach (RepeaterItem riSetPromotion in wrCartSetPromotion.Items)
			{
				var wrCartSetPromotionItem = GetWrappedControl<WrappedRepeater>(riSetPromotion, "rCartSetPromotionItem");

				foreach (RepeaterItem riSetPromotionItem in wrCartSetPromotionItem.Items)
				{
					var wtbSetPromotionItemCount = GetWrappedControl<WrappedTextBox>(riSetPromotionItem, "tbSetPromotionItemCount", "0");
					eventCreator.AddScriptToControl(wtbSetPromotionItemCount);
				}
			}

			// ポイント入力ボックス設定
			eventCreator.AddScriptToControl(wtbOrderPointUse);

			// クーポン入力ボックス設定
			eventCreator.AddScriptToControl(wtbCouponCode);

			// 「カート番号「１」同じお支払いを指定する」チェックイベント
			if (wcbUseSamePaymentAddrAsCart1.InnerControl != null)
			{
				cbUseSamePaymentAddrAsCart1_OnCheckedChanged(wcbUseSamePaymentAddrAsCart1, EventArgs.Empty);
			}

			// 決済種別が未選択の場合、一番上の決済種別を選択（ラジオボタン）
			Constants.PAYMENT_CHOOSE_TYPE_LP = (Constants.PAYMENT_CHOOSE_TYPE_LP_OPTION && this.IsCmsLandingPage)
				? string.IsNullOrEmpty(this.LandingPageDesignModel.PaymentChooseType)
					? Constants.PAYMENT_CHOOSE_TYPE
					: this.LandingPageDesignModel.PaymentChooseType
				: Constants.PAYMENT_CHOOSE_TYPE;
			if (Constants.PAYMENT_CHOOSE_TYPE_LP == Constants.PAYMENT_CHOOSE_TYPE_RB)
			{
				bool blChecked = false;
				foreach (RepeaterItem riPayment in wrPayment.Items)
				{
					var wrbgPayment = GetWrappedControl<WrappedRadioButtonGroup>(riPayment, "rbgPayment");
					blChecked |= wrbgPayment.Checked;
				}
				if ((blChecked == false) && (wrPayment.Items.Count != 0))
				{
					var wrbgPayment = GetWrappedControl<WrappedRadioButtonGroup>(wrPayment.Items[0], "rbgPayment");
					wrbgPayment.Checked = true;
				}
			}

			// 決済種別ラジオボタンクリックイベント
			foreach (RepeaterItem riPayment in wrPayment.Items)
			{
				if (Constants.PAYMENT_CHOOSE_TYPE_LP == Constants.PAYMENT_CHOOSE_TYPE_RB)
				{
					var wrbgPayment = GetWrappedControl<WrappedRadioButtonGroup>(riPayment, "rbgPayment");
					if (wrbgPayment.InnerControl != null)
					{
						base.rbgPayment_OnCheckedChanged(wrbgPayment, EventArgs.Empty);
					}
				}
				else if (Constants.PAYMENT_CHOOSE_TYPE_LP == Constants.PAYMENT_CHOOSE_TYPE_DDL)
				{
					var wddlPayment = GetWrappedControl<WrappedDropDownList>(riPayment.Parent.Parent, "ddlPayment");
					if (wddlPayment.InnerControl != null)
					{
						base.rbgPayment_OnCheckedChanged(wddlPayment, EventArgs.Empty);
					}
				}

				var wddlUserCreditCard = GetWrappedControl<WrappedDropDownList>(riPayment, "ddlUserCreditCard");
				if (wddlUserCreditCard.InnerControl != null)
				{
					ddlUserCreditCard_OnSelectedIndexChanged(wddlUserCreditCard, EventArgs.Empty);
				}

				var wcbRegistCreditCard = GetWrappedControl<WrappedCheckBox>(riPayment, "cbRegistCreditCard");
				if (wcbRegistCreditCard.InnerControl != null)
				{
					cbRegistCreditCard_OnCheckedChanged(wcbRegistCreditCard, EventArgs.Empty);
				}
			}
			// 配送方法設定
			wddlShippingMethod.SelectedValue = this.SelectedShippingMethod[riCart.ItemIndex];

			// 領収書情報
			if (Constants.RECEIPT_OPTION_ENABLED)
			{
				// 領収書希望の選択肢変更イベント
				var wddlReceiptFlg = GetWrappedControl<WrappedDropDownList>(riCart, "ddlReceiptFlg");
				if (wddlReceiptFlg.HasInnerControl)
				{
					ddlReceiptFlg_OnSelectedIndexChanged(wddlReceiptFlg, EventArgs.Empty);
				}

				// AmazonPay指定、かつAmazonPayは領収書開発しない決済区分となる場合、領収書情報入力の表示制御
				if (this.IsAmazonLoggedIn
					&& Constants.NOT_OUTPUT_RECEIPT_PAYMENT_KBN.Contains(Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT))
				{
					var wdivReceiptInfoInputForm =
						GetWrappedControl<WrappedHtmlGenericControl>(riCart, "divReceiptInfoInputForm");
					var wdivDisplayCanNotInputMessage =
						GetWrappedControl<WrappedHtmlGenericControl>(riCart, "divDisplayCanNotInputMessage");
					wdivDisplayCanNotInputMessage.Visible = true;
					wdivReceiptInfoInputForm.Visible = false;
				}
			}

			InitializeCouponComponents(riCart);
		}
	}

	/// <summary>
	/// カートチェック
	/// </summary>
	/// <param name="e"></param>
	private void CheckCart(EventArgs e)
	{
		//------------------------------------------------------
		// カートチェック
		//------------------------------------------------------
		if (this.CartList.Items.Count != 0)
		{
			// 入力チェック
			CheckAndSetInputData(e);

			// カートチェック
			CheckCartData();

			// エラーの場合はエラーメッセージ表示
			if ((String.IsNullOrEmpty(this.ErrorMessages.Get()) == false) || (this.ErrorMessages.Count != 0))
			{
				if (this.CartList.Items.Count != this.WrCartList.Items.Count)
				{
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}
			}
		}
	}

	/// <summary>
	/// 商品選択ドロップダウンリスト
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ddlProductList_OnSelectedIndexChanged(object sender, System.EventArgs e)
	{
		var selectedIndex = int.Parse(this.WddlProductList.SelectedValue);
		for (var index = 0; index < this.ProductList.Length; index++)
		{
			var productList = this.ProductList[index];
			productList.Selected = (index == selectedIndex);
		}

		// カート再作成
		CreateCartList();

		// 画面更新
		Reload(e);
	}

	/// <summary>
	/// リピータイベント
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	public override void rCartList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		#region ラップ済みコントロール宣言
		var whfShopId = GetWrappedControl<WrappedHiddenField>(e.Item, "hfShopId", "");
		var whfProductId = GetWrappedControl<WrappedHiddenField>(e.Item, "hfProductId", "");
		var whfVariationId = GetWrappedControl<WrappedHiddenField>(e.Item, "hfVariationId", "");
		var whfIsFixedPurchase = GetWrappedControl<WrappedHiddenField>(e.Item, "hfIsFixedPurchase", "false");	// 旧バージョンはこちらを見る
		var whfhfAddCartKbn = GetWrappedControl<WrappedHiddenField>(e.Item, "hfAddCartKbn", "Normal");
		var whfProductSaleId = GetWrappedControl<WrappedHiddenField>(e.Item, "hfProductSaleId", "");
		var whfProductOptionValue = GetWrappedControl<WrappedHiddenField>(e.Item, "hfProductOptionValue", "");
		var whfUnallocatedQuantity = GetWrappedControl<WrappedHiddenField>(e.Item, "hfUnallocatedQuantity", "0");
		var whfAllocatedQuantity = GetWrappedControl<WrappedHiddenField>(e.Item, "hfAllocatedQuantity", "0");
		var whfProductSetId = GetWrappedControl<WrappedHiddenField>(e.Item.Parent.Parent, "hfProductSetId", "");
		var whfProductSetNo = GetWrappedControl<WrappedHiddenField>(e.Item.Parent.Parent, "hfProductSetNo", "");
		#endregion

		var hfCartId = (HiddenField)BasePageHelper.GetParentRepeaterItemControl((Repeater)source, "hfCartId");
		var cartId = (hfCartId != null) ? hfCartId.Value : "";
		var cartCount = this.CartList.Items.Count;

		//------------------------------------------------------
		// 商品情報削除
		//------------------------------------------------------
		if ((e.CommandName == "DeleteProduct") || (e.CommandName == "DeleteNecessarySubscriptionProduct"))
		{
			string shopId = whfShopId.Value;
			string productId = whfProductId.Value;
			string variationId = whfVariationId.Value;
			Constants.AddCartKbn addCartKbn = Constants.AddCartKbn.Normal;
			if (whfhfAddCartKbn.InnerControl != null)
			{
				Enum.TryParse<Constants.AddCartKbn>(whfhfAddCartKbn.Value, out addCartKbn);
			}
			else
			{
				addCartKbn = (Constants.FIXEDPURCHASE_OPTION_ENABLED && bool.Parse(whfIsFixedPurchase.Value)) ? Constants.AddCartKbn.FixedPurchase : Constants.AddCartKbn.Normal;
			}
			string productSaleId = whfProductSaleId.Value;
			string productOptionValue = whfProductOptionValue.Value;

			// 対象カート取得
			var targetCart = this.CartList.Items[0];
			if (this.IsCartListLp == false)
			{
				cartId = targetCart.CartId;
			}
			else
			{
				targetCart = this.CartList.GetCart(cartId);
			}
			this.SubscriptionBoxCourseId = targetCart.SubscriptionBoxCourseId;
			if (targetCart != null)
			{
				// 頒布会必須商品ならカートごと削除
				if (e.CommandName =="DeleteNecessarySubscriptionProduct")
				{
					this.CartList.DeleteCartVurtual(targetCart);
					SetCartListSession();
				}

				// 削除対象商品取得
				var targetCartProduct = targetCart.GetProduct(
					shopId,
					productId,
					variationId,
					false,
					bool.Parse(whfIsFixedPurchase.Value),
					productSaleId,
					productOptionValue,
					"",
					this.SubscriptionBoxCourseId);
				if (targetCartProduct != null)
				{
					// 削除数取得
					int deleteQuantity = 0;
					if (whfUnallocatedQuantity.Value != "0")
					{
						deleteQuantity = int.Parse(whfUnallocatedQuantity.Value);
					}
					else
					{
						deleteQuantity = int.Parse(whfAllocatedQuantity.Value);
					}

					if (targetCartProduct.CountSingle > deleteQuantity)
					{
						// 対象商品のカート内の合計数が削除数量より多ければ数量を減らすだけ
						targetCartProduct.SetProductCount(cartId, targetCartProduct.CountSingle - deleteQuantity);
					}
					else
					{
						// カート商品削除（商品数が0になればカート削除）
						this.CartList.DeleteProduct(
							shopId,
							productId,
							variationId,
							addCartKbn,
							productSaleId,
							productOptionValue);
						// ノベルティ商品削除
						if (string.IsNullOrEmpty(targetCartProduct.NoveltyId) == false)
						{
							this.CartList.NoveltyIdsDelete = this.CartList.NoveltyIdsDelete ?? new List<string>();
							this.CartList.NoveltyIdsDelete.Add(targetCartProduct.NoveltyId);
						}
						SetCartListSession();
					}

					targetCart.Calculate(true, isCartItemChanged: true);
				}
			}
		}
		// ノベルティ追加
		else if (e.CommandName == "AddNovelty")
		{
			AddNovelty(e.CommandArgument.ToString(), cartId);

			// 配送先情報作成
			CreateOrderShipping();

			// カート注文メモ作成
			CreateOrderMemo();

			// カート決済情報作成（デフォルトで配送希望は希望するようにする）
			CreateCartPayment();

			// クレジットカード番号再表示の際は、上位４桁と下位４桁以外を空文字設定、セキュリティコードを空文字設定
			AdjustCreditCardNo();
		}
		// 商品セット情報削除
		else if (e.CommandName == "DeleteProductSet")
		{
			var productSetId = whfProductSetId.Value;
			var productSetNo = whfProductSetNo.Value;

			// カート商品削除（商品数が0になればカート削除）
			// HACK: デザイン側のhfCartIdを削除されるとカートIDが取得できない
			this.CartList.DeleteProductSet(cartId, productSetId, int.Parse(productSetNo));
			SetCartListSession();
		}

		Reload(e);

		// カート部分更新(カート商品削除を画面に反映)
		if (this.IsCartListLp && (cartCount != this.CartList.Items.Count))
		{
			this.WupCartListUpdatePanel.Update();
		}
	}

	/// <summary>
	/// 新LPカートの場合は商品を削除する際にカートリストを再セット
	/// </summary>
	private void SetCartListSession()
	{
		if (this.IsCartListLp)
		{
			var cartListLp = SessionManager.CartListLp;
			SessionManager.CartList = cartListLp;
		}
	}

	/// <summary>
	/// （トークン決済向け）カード情報編集リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void lbEditCreditCardNoForToken_Click(object sender, System.EventArgs e)
	{
		// トークンなどクレジットカード情報削除
		ResetCreditTokenInfoFromForm(GetParentRepeaterItem((LinkButton)sender, "rPayment"));

		// 画面更新
		Reload(e);
	}

	/// <summary>
	/// 次回購入の利用ポイントの全適用フラグ変更時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void cbUseAllPointFlg_Changed(object sender, EventArgs e)
	{
		var useAllPointFlgInputMethod = (CheckBox)sender;
		var riCart = GetParentRepeaterItem(useAllPointFlgInputMethod, "rCartList");

		this.CartList.Items[riCart.ItemIndex].UseAllPointFlg = useAllPointFlgInputMethod.Checked;

		lbRecalculate_Click(riCart.FindControl("lbRecalculateCart"), null);
	}

	/// <summary>
	/// 次回購入の利用ポイントの全適用フラグデータバインド時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void cbUseAllPointFlg_DataBinding(object sender, EventArgs e)
	{
		var useAllPointFlgInputMethod = (CheckBox)sender;
		var riCart = GetParentRepeaterItem(useAllPointFlgInputMethod, "rCartList");

		useAllPointFlgInputMethod.Checked = this.CartList.Items[riCart.ItemIndex].UseAllPointFlg;
	}


	/// <summary>
	/// クーポン入力方法変更時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void rblCouponInputMethod_SelectedIndexChanged(object sender, EventArgs e)
	{
		var couponInputMethod = (RadioButtonList)sender;
		var riCart = GetParentRepeaterItem(couponInputMethod, "rCartList");

		// ラップ済みコントロール宣言
		string couponCode = (this.CartList.Items[riCart.ItemIndex].Coupon != null) ? this.CartList.Items[riCart.ItemIndex].Coupon.CouponCode : "";
		var wddlCouponList = GetWrappedControl<WrappedDropDownList>(riCart, "ddlCouponList");
		var whgcCouponCodeInputArea = GetWrappedControl<WrappedHtmlGenericControl>(riCart, "hgcCouponCodeInputArea");
		var wtbCouponCode = GetWrappedControl<WrappedTextBox>(riCart, "tbCouponCode", couponCode);

		wddlCouponList.Visible = (couponInputMethod.Text != CouponOptionUtility.COUPON_INPUT_METHOD_MANUAL_INPUT);
		wddlCouponList.SelectedValue = "";
		whgcCouponCodeInputArea.Visible = (couponInputMethod.Text == CouponOptionUtility.COUPON_INPUT_METHOD_MANUAL_INPUT);
		wtbCouponCode.Text = "";

		this.CartList.Items[riCart.ItemIndex].CouponInputMethod = (StringUtility.ToEmpty(couponInputMethod.Text) == "")
			? CouponOptionUtility.COUPON_INPUT_METHOD_SELECT
			: couponInputMethod.Text;
		this.CartList.Items[riCart.ItemIndex].Coupon = null;

		lbRecalculate_Click(riCart.FindControl("lbRecalculateCart"), null);
	}

	/// <summary>
	/// 付帯情報の選択肢クリック時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void cbProductOptionValueSettingListOnCheckedChanged(object sender, EventArgs e)
	{
		var control = (Control)sender;
		var riCart = GetParentRepeaterItem(control, "rCartList");
		CheckProductOptionValueSetting();
		lbRecalculate_Click(riCart.FindControl("lbRecalculateCart"), null);
	}

	/// <summary>
	/// クーポン入力方法データバインド時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void rblCouponInputMethod_DataBinding(object sender, EventArgs e)
	{
		var couponInputMethod = (RadioButtonList)sender;
		var riCart = GetParentRepeaterItem(couponInputMethod, "rCartList");

		couponInputMethod.SelectedValue = (StringUtility.ToEmpty(this.CartList.Items[riCart.ItemIndex].CouponInputMethod) == "")
			? CouponOptionUtility.COUPON_INPUT_METHOD_SELECT
			: this.CartList.Items[riCart.ItemIndex].CouponInputMethod;
	}

	/// <summary>
	/// クーポンリスト変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ddlCouponList_TextChanged(object sender, EventArgs e)
	{
		var couponCodeList = (DropDownList)sender;
		var riCart = GetParentRepeaterItem(couponCodeList, "rCartList");

		// ラップ済みコントロール宣言
		string couponCode = (this.CartList.Items[riCart.ItemIndex].Coupon != null) ? this.CartList.Items[riCart.ItemIndex].Coupon.CouponCode : "";
		var wtbCouponCode = GetWrappedControl<WrappedTextBox>(riCart, "tbCouponCode", couponCode);

		wtbCouponCode.Text = couponCodeList.SelectedValue;

		lbRecalculate_Click(riCart.FindControl("lbRecalculateCart"), null);
	}

	/// <summary>
	/// クーポンBOXクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void lbShowCouponBox_Click(object sender, EventArgs e)
	{
		var showCouponBox = (LinkButton)sender;
		var riCart = GetParentRepeaterItem((LinkButton)sender, "rCartList");

		// ラップ済みコントロール宣言
		string couponCode = (this.CartList.Items[riCart.ItemIndex].Coupon != null) ? this.CartList.Items[riCart.ItemIndex].Coupon.CouponCode : "";
		var wtbCouponCode = GetWrappedControl<WrappedTextBox>(riCart, "tbCouponCode", couponCode);
		var wddlCouponList = GetWrappedControl<WrappedDropDownList>(riCart, "ddlCouponList");

		wtbCouponCode.Text = "";
		wddlCouponList.SelectedValue = "";
		this.CartList.Items[riCart.ItemIndex].CouponBoxVisible = true;

		lbRecalculate_Click(riCart.FindControl("lbRecalculateCart"), null);
	}

	/// <summary>
	/// モーダルクーポンBOX クーポン選択時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void lbCouponSelect_Click(object sender, EventArgs e)
	{
		var lbCouponSelect = (LinkButton)sender;
		var riCart = GetParentRepeaterItem(lbCouponSelect, "rCartList");
		var riCoupon = GetParentRepeaterItem(lbCouponSelect, "rCouponList");

		// ラップ済みコントロール宣言
		string couponCode = (this.CartList.Items[riCart.ItemIndex].Coupon != null) ? this.CartList.Items[riCart.ItemIndex].Coupon.CouponCode : "";
		var wtbCouponCode = GetWrappedControl<WrappedTextBox>(riCart, "tbCouponCode", couponCode);
		var whfCouponBoxCouponCode = GetWrappedControl<WrappedHiddenField>(riCoupon, "hfCouponBoxCouponCode");
		var wddlCouponList = GetWrappedControl<WrappedDropDownList>(riCart, "ddlCouponList");

		wtbCouponCode.Text = whfCouponBoxCouponCode.Value;
		wddlCouponList.SelectedValue = whfCouponBoxCouponCode.Value;
		this.CartList.Items[riCart.ItemIndex].CouponBoxVisible = false;

		lbRecalculate_Click(riCart.FindControl("lbRecalculateCart"), null);
	}

	/// <summary>
	/// モーダルクーポンBOX 閉じるボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void lbCouponBoxClose_Click(object sender, EventArgs e)
	{
		var couponBoxClose = (LinkButton)sender;
		var riCart = GetParentRepeaterItem(couponBoxClose, "rCartList");

		this.CartList.Items[riCart.ItemIndex].CouponBoxVisible = false;

		lbRecalculate_Click(riCart.FindControl("lbRecalculateCart"), null);
	}

	/// <summary>
	/// 配送方法選択ドロップダウンリスト
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public new void ddlShippingMethodList_OnSelectedIndexChanged(object sender, System.EventArgs e)
	{
		base.ddlShippingMethodList_OnSelectedIndexChanged(sender, e);

		var shippingMethod = (DropDownList)sender;
		var riCart = GetParentRepeaterItem(shippingMethod, "rCartList");
		lbRecalculate_Click(riCart.FindControl("lbRecalculateCart"), null);
	}

	/// <summary>
	/// 配送サービス選択ドロップダウンリスト
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public new void ddlDeliveryCompanyList_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		base.ddlDeliveryCompanyList_OnSelectedIndexChanged(sender, e);
		var deliveryCompany = (DropDownList)sender;
		var riCart = GetParentRepeaterItem(deliveryCompany, "rCartList");
		lbRecalculate_Click(riCart.FindControl("lbRecalculateCart"), null);
	}

	/// <summary>
	/// DropDownList Shipping Receiving Store Type Selected Index Changed Event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public new void ddlShippingReceivingStoreType_SelectedIndexChanged(object sender, EventArgs e)
	{
		base.ddlShippingReceivingStoreType_SelectedIndexChanged(sender, e);
		var shippingReceivingStoreType = (DropDownList)sender;
		var riCart = GetParentRepeaterItem(shippingReceivingStoreType, "rCartList");
		lbRecalculate_Click(riCart.FindControl("lbRecalculateCart"), null);
	}

	/// <summary>
	/// DropDownList Shipping Kbn List Selected Index Changed Event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public new void ddlShippingKbnList_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		base.ddlShippingKbnList_OnSelectedIndexChanged(sender, e);
		var shippingKbnList = (DropDownList)sender;
		var riCart = GetParentRepeaterItem(shippingKbnList, "rCartList");

		lbRecalculate_Click(riCart.FindControl("lbRecalculateCart"), null);
	}

	/// <summary>
	/// Radio button group payment on checked changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public new void rbgPayment_OnCheckedChanged(object sender, System.EventArgs e)
	{
		base.rbgPayment_OnCheckedChanged(sender, e);
		if (Constants.PAYMENT_CHOOSE_TYPE_LP == Constants.PAYMENT_CHOOSE_TYPE_RB)
		{
			WrappedRadioButtonGroup wrbgPayments;
			if (sender is RadioButtonGroup)
			{
				wrbgPayments = GetWrappedControl<WrappedRadioButtonGroup>(((Control)sender).Parent, ((Control)sender).ID);
			}
			else if (sender is WrappedRadioButtonGroup)
			{
				wrbgPayments = (WrappedRadioButtonGroup)sender;
			}
			else
			{
				return;
			}

			if (wrbgPayments != null)
			{
				var riCart = GetParentRepeaterItem(wrbgPayments, "rCartList");
				lbRecalculate_Click(riCart.FindControl("lbRecalculateCart"), null);
			}
		}
		else if (Constants.PAYMENT_CHOOSE_TYPE_LP == Constants.PAYMENT_CHOOSE_TYPE_DDL)
		{
			WrappedDropDownList wddlPayments;
			if (sender is DropDownList)
			{
				wddlPayments = GetWrappedControl<WrappedDropDownList>(((Control)sender).Parent, ((Control)sender).ID);
			}
			else if (sender is WrappedDropDownList)
			{
				wddlPayments = (WrappedDropDownList)sender;
			}
			else
			{
				return;
			}

			if (wddlPayments != null)
			{
				var riCart = GetParentRepeaterItem(wddlPayments, "rCartList");
				lbRecalculate_Click(riCart.FindControl("lbRecalculateCart"), null);
			}
		}
	}

	/// <summary>
	/// ログインボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void lbLogin_Click(object sender, EventArgs e)
	{
		// Optionによって、ログインＩＤにセットする値が変わる
		string loginId = Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED ? this.WtbLoginIdInMailAddr.Text : this.WtbLoginId.Text;

		// Check account is locked
		if (LoginAccountLockManager.GetInstance().IsAccountLocked(Request.UserHostAddress, loginId, this.WtbPassword.Text))
		{
			// Set login account locked error message
			var loginAccountLockedErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_USER_LOGIN_ACCOUNT_LOCK);
			SetLoginErrorMessage(sender, loginAccountLockedErrorMessage);
			return;
		}

		// ログイン判定処理
		var loggedUser = new UserService().TryLogin(loginId, this.WtbPassword.Text, Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED);
		if (loggedUser == null)
		{
			// Set login denied error message
			var loginDeniedErrorMessage = GetLoginDeniedErrorMessage(loginId, this.WtbPassword.Text);
			SetLoginErrorMessage(sender, loginDeniedErrorMessage);
			return;
		}

		// ログイン成功処理実行＆次の画面へ遷移（ログイン向け）
		ExecLoginSuccessProcessAndGoNextForLogin(
			loggedUser,
			Request.RawUrl,
			this.WcbAutoCompleteLoginIdFlg.Checked,
			BasePage.LoginType.Normal,
			UpdateHistoryAction.Insert);
	}

	/// <summary>
	/// 「会員登録する」チェックボックスクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void cbUserRegister_OnCheckedChanged(object sender, System.EventArgs e)
	{
		WdvUserPassword.Visible = this.WcbUserRegister.Checked;

		// Display Checkbox Regist CreditCard
		DisplayCheckboxRegistCreditCard();

		// Reset authentication code when user register
		this.HasAuthenticationCode = false;
	}

	/// <summary>
	/// Amazonお支払いをやめるクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void lbCancelAmazonPay_Click(object sender, System.EventArgs e)
	{
		UnlinkAmazonPaySession();
		Response.Redirect(Request.Url.AbsolutePath);
	}

	/// <summary>
	/// Amazon連携状態を解除
	/// </summary>
	public void UnlinkAmazonPaySession()
	{
		Session.Remove(AmazonConstants.SESSION_KEY_AMAZON_MODEL);
		Session.Remove(AmazonConstants.SESSION_KEY_AMAZON_ADDRESS);
		Session.Remove(AmazonCv2Constants.SESSION_KEY_AMAZON_CHECKOUT_SESSION_ID);

		if (this.CartList.Items.Count > 0)
		{
			this.CartList.Items[0].Shippings.Clear();
		}
		CreateOrderShipping();
	}

	/// <summary>
	/// 楽天IDConnectリクエストクリック（ログイン）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void lbRakutenIdConnectRequestAuth_Click(object sender, EventArgs e)
	{
		RedirectRakutenIdConnect(ActionType.Login, Request.RawUrl, Request.RawUrl, true);
	}

	/// <summary>
	/// 注文情報取得
	/// </summary>
	/// <param name="orderReferenceIdOrBillingAgreementId">注文リファレンスIDor支払い契約ID</param>
	/// <param name="orderType">注文種別</param>
	/// <param name="addressType">住所種別</param>
	/// <returns>エラーメッセージ</returns>
	public static string GetAmazonAddress(string orderReferenceIdOrBillingAgreementId, string orderType, string addressType)
	{
		// トークン取得
		var session = HttpContext.Current.Session;
		var amazonModel = (AmazonModel)session[AmazonConstants.SESSION_KEY_AMAZON_MODEL];
		var token = amazonModel.Token;

		// 注文種別、住所種別
		AmazonConstants.OrderType eOrderType;
		AmazonConstants.AddressType eAddressType;
		var isValidOrderType = Enum.TryParse<AmazonConstants.OrderType>(orderType, out eOrderType);
		var isValidAddressType = Enum.TryParse<AmazonConstants.AddressType>(addressType, out eAddressType);
		var errorMessage = string.Empty;
		if (string.IsNullOrEmpty(orderReferenceIdOrBillingAgreementId)
			|| string.IsNullOrEmpty(token)
			|| (isValidOrderType == false)
			|| (isValidAddressType == false))
		{
			errorMessage = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_USER_INVALID_NAME_FOR_AMAZON_ADDRESS_WIDGET);
			return JsonConvert.SerializeObject(new { Error = errorMessage });
		}

		// ウィジェットから住所情報取得
		AmazonAddressInput input = null;
		if (eOrderType == AmazonConstants.OrderType.OneTime)
		{
			var res = AmazonApiFacade.GetOrderReferenceDetails(orderReferenceIdOrBillingAgreementId, token);
			input = new AmazonAddressInput(res);
		}
		else
		{
			var res = AmazonApiFacade.GetBillingAgreementDetails(orderReferenceIdOrBillingAgreementId, token);
			input = new AmazonAddressInput(res);
		}
		// 入力チェック
		errorMessage = input.Validate();
		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			// エラーメッセージ保持
			if (eAddressType == AmazonConstants.AddressType.Owner) session[AmazonConstants.SESSION_KEY_AMAZON_OWNER_ADDRESS_ERROR_MSG] = errorMessage;
			if (eAddressType == AmazonConstants.AddressType.Shipping) session[AmazonConstants.SESSION_KEY_AMAZON_SHIPPING_ADDRESS_ERROR_MSG] = errorMessage;
			return JsonConvert.SerializeObject(new { Error = errorMessage });
		}

		// モデル生成
		var model = AmazonAddressParser.Parse(input);
		if (eAddressType == AmazonConstants.AddressType.Owner)
		{
			session[AmazonConstants.SESSION_KEY_AMAZON_OWNER_ADDRESS] = model;
			session[AmazonConstants.SESSION_KEY_AMAZON_OWNER_ADDRESS_ERROR_MSG] = string.Empty;
		}
		if (eAddressType == AmazonConstants.AddressType.Shipping)
		{
			session[AmazonConstants.SESSION_KEY_AMAZON_SHIPPING_ADDRESS] = model;
			session[AmazonConstants.SESSION_KEY_AMAZON_SHIPPING_ADDRESS_ERROR_MSG] = string.Empty;
		}

		var oldAmazonAddress = (AmazonAddressModel)session[AmazonConstants.SESSION_KEY_AMAZON_ADDRESS];
		session[AmazonConstants.SESSION_KEY_AMAZON_ADDRESS] = model;

		if ((oldAmazonAddress == null)
			|| (model.Addr != oldAmazonAddress.Addr)
			|| (model.Addr1 != oldAmazonAddress.Addr1)
			|| (model.Addr2 != oldAmazonAddress.Addr2)
			|| (model.Addr3 != oldAmazonAddress.Addr3)
			|| (model.Addr4 != oldAmazonAddress.Addr4))
		{
			return JsonConvert.SerializeObject(
				new
				{
					RequestPostBack = string.Empty
				});
		}

		return JsonConvert.SerializeObject(new { Error = string.Empty });
	}

	/// <summary>
	/// カスタムバリデータの属性値を変更する（EFOオプションONのとき、カスタムバリデータを無効化する）
	/// </summary>
	public void UpdateAttributeValueForCustomValidator()
	{
		// AmazonAddressManagerオプションの対象に対してカスタムバリデータコントロールを設定
		if (IsTargetToExtendedAmazonAddressManagerOption()) SetCustomValidatorControlsForExtendedAmazonAddressManagerOption(this.WrCartList);

		// EFOオプション時のカスタムバリデータコントロールを設定
		if (IsUseDefaultCustomValidators() == false) SetCustomValidatorControlsForEfoOption();

		// デフォルトのカスタムバリデータを使用
		SetCustomValidatorControlInformationList(this.Page);
	}

	/// <summary>
	/// デフォルトのカスタムバリデータを使用するかどうか
	/// </summary>
	/// <returns>結果</returns>
	private bool IsUseDefaultCustomValidators()
	{
		// EFOオプションチェック（無効な場合、カスタムバリデータを無効化しない）
		if (this.IsEfoOptionEnabled == false) return true;

		// 新LPかつEFO CUBE利用フラグがオフの場合、カスタムバリデータを無効化しない(旧LPの場合は、フラグに関係なく無効化する)
		if (this.IsCmsLandingPage && (this.LandingPageDesignModel.EfoCubeUseFlg == LandingPageConst.EFO_CUBE_USE_FLG_OFF)) return true;

		// カートが存在しなければスルー
		if (this.WrCartList.Items.Count == 0) return true;

		// デフォルトを利用しない
		return false;
	}

	/// <summary>
	/// EFOオプション時のカスタムバリデータコントロールを設定
	/// </summary>
	private void SetCustomValidatorControlsForEfoOption()
	{
		var searchTag = new List<string>
		{
			"cvOwnerName1",
			"cvOwnerName2",
			"cvOwnerNameKana1",
			"cvOwnerNameKana2",
			"cvOwnerBirth",
			"cvOwnerSex",
			"cvOwnerMailAddr",
			"cvOwnerMailAddrConf",
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
			"cvOwnerTel2_3"
		};
		var repeaterItem = this.WrCartList.Items.Cast<RepeaterItem>().First();
		var customValidatorControls = searchTag
			.Select(target => GetWrappedControl<WrappedCustomValidator>(repeaterItem, target))
			.ToList();

		var searchRepTag = new List<string>
		{
			"cvShippingName1",
			"cvShippingName2",
			"cvShippingNameKana1",
			"cvShippingNameKana2",
			"cvShippingZip1",
			"cvShippingZip2",
			"cvShippingAddr1",
			"cvShippingAddr2",
			"cvShippingAddr3",
			"cvShippingTel1_1",
			"cvShippingTel1_2",
			"cvShippingTel1_3",
			"cvUserShippingName",
			"cvFixedPurchaseMonth",
			"cvFixedPurchaseMonthlyDate",
			"cvFixedPurchaseWeekOfMonth",
			"cvFixedPurchaseDayOfWeek",
			"cvFixedPurchaseIntervalDays",
			"cvRealShopName"
		};
		customValidatorControls.AddRange(
			this.WrCartList.Items
				.Cast<RepeaterItem>()
				.ToList()
				.SelectMany(
					rpItem => searchRepTag.Select(tag => GetWrappedControl<WrappedCustomValidator>(rpItem, tag))));

		var searchrPaymentItemTag = new List<string>
		{
			"cvCreditCardNo1",
			"cvCreditCardNo2",
			"cvCreditCardNo3",
			"cvCreditCardNo4",
			"cvCreditAuthorName",
			"cvCreditSecurityCode",
			"cvUserCreditCardName",
		};
		customValidatorControls.AddRange(
			this.WrCartList.Items
				.Cast<RepeaterItem>()
				.ToList()
				.SelectMany(
					rpItem => GetWrappedControl<WrappedRepeater>(rpItem, "rPayment").Items
						.Cast<RepeaterItem>()
						.ToList()
						.SelectMany(
							rPayItem => searchrPaymentItemTag.Select(
								pTag => GetWrappedControl<WrappedCustomValidator>(rPayItem, pTag)))));
		SetDisableAndHideCustomValidatorControlInformationList(customValidatorControls);
	}

	/// <summary>
	/// ユーザー管理レベルにより制限されているかどうかチェック
	/// </summary>
	/// <param name="productId">商品Id</param>
	/// <param name="productName">商品名</param>
	public void CheckUserLevelIsLimited(string productId, string productName)
	{
		if (CheckFixedPurchaseLimitedUserLevel(this.ShopId, productId))
		{
			this.CartList.Items.Clear();
			Session[Constants.SESSION_KEY_ERROR_MSG] = OrderCommon.GetErrorMessage(OrderErrorcode.ProducFixedPurchaseInvalidError).Replace("@@ 1 @@", productName);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}
	}

	/// <summary>
	/// ソーシャルログインボタン 表示チェック
	/// </summary>
	/// <param name="socialLoginType">ソーシャルログイン種類</param>
	/// <returns>表示可否</returns>
	public bool DisplaySocialLoginBtnCheck(LandingPageConst.SocialLoginType socialLoginType)
	{
		var result = false;

		switch (this.LandingPageDesignModel.SocialLoginUseType)
		{
			case LandingPageConst.SOCIAL_LOGIN_USE_TYPE_ALL:
				result = true;
				break;

			case LandingPageConst.SOCIAL_LOGIN_USE_TYPE_ONLY:
				result = this.LandingPageDesignModel.SocialLoginList.Split(',').Any(s => s == socialLoginType.ToText());
				break;
		}

		return result;
	}

	/// <summary>
	/// 画面表示
	/// </summary>
	/// <param name="e"></param>
	/// <param name="isChangeProductSet">商品セット選択変更時か</param>
	private void Reload(EventArgs e, bool isChangeProductSet = false)
	{
		// 頒布会エラーチェック
		this.IsSubscriptionBoxError = SubscriptionErrorCheck();

		// カートリストに各情報をセット
		SetCartListForDataBind();

		// カートノベルティリストをセット
		var isCartChanged = SetCartNovelty();

		// ノベルティによってカート分割された場合、新しいカートに情報がないためカート情報を再セット
		if (isCartChanged)
		{
			SetCartInformation();
		}

		// 商品データの画面設定にて入力済みの商品付帯情報がクリアされるため、一時退避
		var inputtedProductOptionValueSettings = GetInputtedProductOptionValueSettings();

		// ユーザー拡張項目にて入力済の内容を保持させる
		this.UserExtendInput = CreateUserExtendInput();

		// Add fixed purchase carts to session for future order display
		Session[Constants.SESSION_KEY_FIXED_PURCHASE_CART_LIST_LANDING] = this.CartList.Items.ToArray();

		// データバインド
		this.WrProductList.DataSource = this.ProductList;
		this.WrProductList.DataBind();

		this.WrCartList.DataSource = this.CartList;
		this.WrCartList.DataBind();

		var errorMessageForMemberRank = CheckLpProductMemberRank(this.ProductList);

		if (string.IsNullOrEmpty(errorMessageForMemberRank) == false)
		{
			foreach (RepeaterItem riCart in this.WrCartList.Items)
			{
				var memberRankErrorMessage = GetWrappedControl<WrappedLiteral>(riCart, "lMemberRankError");
				memberRankErrorMessage.Text = HtmlSanitizer.HtmlEncode(errorMessageForMemberRank);
			}
		}

		UserExtendUserControl wucUserExtend;
		var isSuccess = GetUserExtendControl(out wucUserExtend);
		if (isSuccess)
		{
			// 確認画面以外から入力画面へ遷移
			if (wucUserExtend.CheckReturnFromConfirmPage() == false)
			{
				wucUserExtend.WrUserExtendInput.DataSource = wucUserExtend.UserExtendSettingList;
				wucUserExtend.WrUserExtendInput.DataBind();
				wucUserExtend.SetUserExtendFromDefault();
			}
		}

		// カートエラー表示
		if ((string.IsNullOrEmpty(this.DispCartErrorMessage) == false)
			&& (this.DispCartErrorMessage.Length != 0))
		{
			foreach (RepeaterItem riCart in this.WrCartList.Items)
			{
				DispCartError(riCart, this.DispCartErrorMessage);
			}
		}

		// 一時退避した入力済み商品付帯情報を再セット(商品セット自体を切り替えている場合は復元不要のためスキップ)
		if (isChangeProductSet == false) SetProductOptionValueSettings(inputtedProductOptionValueSettings);

		//// 各種表示初期化
		SetEvent();

		// 注文者情報・配送先グローバル関連項目設定
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			// 注文者情報グローバル関連項目設定
			SetOrderOwnerGlobalColumn();

			// 配送先グローバル関連項目設定
			foreach (var cart in this.WrCartList.Items.Cast<RepeaterItem>().ToArray())
			{
				SetOrderShippingGlobalColumn(cart, this.CartList.Items[cart.ItemIndex]);
			}
		}

		// 各種表示初期化(データバインド後に実行する必要がある)
		InitComponentsDispOrderShipping(e);

		// 配送サービス選択初期化
		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			SelectDeliveryCompany(riCart, this.CartList.Items[riCart.ItemIndex]);
		}

		//------------------------------------------------------
		// エラーメッセージ表示
		//------------------------------------------------------
		this.ErrorMessage += this.ErrorMessages.Get();

		// トークン決済利用の場合はカスタムバリデータを一部無効にする
		DisableCreditInputCustomValidatorForGetCreditToken(this.WrCartList);

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			// 有効なカートが存在し、かつAmazonログイン中またはAmazonログイン連携ユーザーの場合国をJapan固定にする
			if ((this.WrCartList.Items.Count > 0)
				&& ((string.IsNullOrEmpty(AmazonUtil.GetAmazonUserIdByUseId(this.LoginUserId)) == false)
					|| this.IsAmazonLoggedIn))
			{
				var wddlOwnerCountry = GetWrappedControl<WrappedDropDownList>(this.WrCartList.Items.Cast<RepeaterItem>().ToList()[0], "ddlOwnerCountry");
				var whgcCountryAlertMessage = GetWrappedControl<WrappedHtmlGenericControl>(this.WrCartList.Items.Cast<RepeaterItem>().ToList()[0], "countryAlertMessage");

				if (wddlOwnerCountry.InnerControl != null) wddlOwnerCountry.InnerControl.Enabled = false;
				whgcCountryAlertMessage.Visible = true;
			}
		}

		if (this.IsLoggedIn)
		{
			foreach (var cart in this.CartList.Items)
			{
				cart.UpdateUserId("");
			}
		}
	}

	/// <summary>
	/// カートリストにデータバインド用情報をセット
	/// </summary>
	public void SetCartListForDataBind()
	{
		if (this.IsLoggedIn) this.CartList.Items.ForEach(item => item.UpdateUserId(this.LoginUserId));
		this.CartList.CartListShippingMethodUserUnSelected();

		//------------------------------------------------------
		// データバインド準備（配送種別が取得できない場合はエラー画面へ遷移）
		//------------------------------------------------------
		PrepareForDataBindOrderShipping(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);

		this.CartList.CalculateAllCart();

		//------------------------------------------------------
		// 有効決済種別取得
		//------------------------------------------------------
		this.ValidPayments = new List<PaymentModel[]>();
		this.DispLimitedPaymentMessages = new Hashtable();

		foreach (CartObject coCart in this.CartList)
		{
			// 決済種別情報セット
			var validPaymentList = OrderCommon.GetValidPaymentList(
				coCart,
				this.LoginUserId,
				isMultiCart: this.CartList.IsMultiCart);

			if (PaygentUtility.CanUsePaidyPayment(coCart) == false)
			{
				validPaymentList = validPaymentList.Where(item => (item.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)).ToArray();
			}

			// Set limited payments messages for cart
			this.DispLimitedPaymentMessages[this.CartList.Items.IndexOf(coCart)] = OrderCommon.GetLimitedPaymentsMessagesForCart(coCart, validPaymentList);

			// Get the payments un-limit by product
			var paymentsUnlimit = OrderCommon.GetPaymentsUnLimitByProduct(coCart, validPaymentList);

			// AmazonPayは表示しない
			paymentsUnlimit = AmazonUtil.RemoveAmazonPayFromValidPayments(paymentsUnlimit);

			if (Constants.PAYMENT_SMS_DEF_KBN == Constants.PaymentSmsDef.YamatoKa)
			{
				if ((this.CartList.Items.IndexOf(coCart) != 0)
					|| ((Constants.PAYMENT_SETTING_YAMATO_KA_SMS_USE_FIXEDPURCHASE == false)
						&& coCart.Items.Any(item => item.IsFixedPurchase)))
				{
					paymentsUnlimit = paymentsUnlimit
						.Where(item => (OrderCommon.CheckPaymentYamatoKaSms(item.PaymentId) == false)).ToArray();
				}
			}

			if (this.IsCmsLandingPage)
			{
				var disablePaymentList = this.LandingPageDesignModel.UnpermittedPaymentIds.Split(',');
				paymentsUnlimit = paymentsUnlimit
					.Where(i => disablePaymentList.Any(dp => dp == i.PaymentId) == false)
					.ToArray();

				// デフォルト決済注文はLPビルダー設定のデフォルト支払方法とする
				// LPビルダー設定のデフォルト支払方法が注文に利用できない場合は、ログインユーザーのデフォルト設定とする
				var defaultPaymentId =
					paymentsUnlimit.Any(payment => (payment.PaymentId == this.LandingPageDesignModel.DefaultPaymentId))
						? this.LandingPageDesignModel.DefaultPaymentId
						: OrderCommon.GetUserDefaultOrderSettingPaymentId(this.LoginUserId);

				defaultPaymentId = paymentsUnlimit.Any(payment => (payment.PaymentId == defaultPaymentId))
					? defaultPaymentId
					: paymentsUnlimit.Any()
						? paymentsUnlimit.First().PaymentId
						: null;

				// デフォルト決済方法を先頭にする
				paymentsUnlimit = SortPaymentListForDefaultCheckedPayment(paymentsUnlimit, defaultPaymentId);

				// Set payment to the user's default order setting if the page is CartListLp
				if (Constants.CART_LIST_LP_OPTION
					&& this.LandingPageDesignModel.IsCartListLp
					&& (string.IsNullOrEmpty(this.UserDefaultOrderSettingPaymentId) == false))
				{
					paymentsUnlimit = SortPaymentListForDefaultCheckedPayment(paymentsUnlimit, this.UserDefaultOrderSettingPaymentId);
				}

				if (this.LandingPageDesignModel.OrderConfirmPageSkipFlg == LandingPageConst.ORDER_CONFIRM_PAGE_SKIP_FLG_ON)
				{
					coCart.Payment.PaymentId = defaultPaymentId;
					coCart.Payment.PriceExchange = OrderCommon.GetPaymentPrice(
						coCart.ShopId,
						coCart.Payment.PaymentId,
						coCart.PriceSubtotal,
						coCart.PriceCartTotalWithoutPaymentPrice);

					coCart.Calculate(false, isPaymentChanged: true);
				}
			}
			else
			{
				// デフォルト決済方法を先頭にする
				paymentsUnlimit = SortPaymentListForDefaultCheckedPayment(paymentsUnlimit, this.DefaultPaymentId);
			}

			if (Constants.RAKUTEN_LOGIN_ENABLED)
			{
				// 楽天IDConnectでログインしている場合は楽天ペイを先頭にする
				paymentsUnlimit = SortPaymentListForRakutenIdConnectLoggedIn(paymentsUnlimit);
			}

			// For case this cart use shipping is convenience store
			var cartShipping = coCart.GetShipping();
			var logisticsCollectionFlg = ECPayUtility.GetIsCollection(cartShipping.ShippingReceivingStoreType);

			if (OrderCommon.IsReceivingStoreTwEcPayCvsOptionEnabled()
				&& (logisticsCollectionFlg == Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_COLLECTION_ON))
			{
				paymentsUnlimit = paymentsUnlimit
					.Where(item => (item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE))
					.ToArray();
			}
			else if (OrderCommon.IsReceivingStoreTwEcPayCvsOptionEnabled()
					&& ((cartShipping.ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF)
				|| (logisticsCollectionFlg == Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_COLLECTION_OFF)))
			{
				paymentsUnlimit = paymentsUnlimit
					.Where(item => (item.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE))
					.ToArray();
			}

			if (this.CartList.Items.Any(item => item.Shippings[0].IsShippingStorePickup))
			{
				paymentsUnlimit = OrderCommon.GetPaymentsUnLimitByProduct(coCart, validPaymentList)
					.Where(item => Constants.SETTING_CAN_STORE_PICKUP_OPTION_PAYMENT_IDS.Contains(item.PaymentId)
						&& (Constants.SETTING_CAN_NOT_STORE_PICKUP_OPTION_PAYMENT_IDS.Contains(item.PaymentId) == false))
					.ToArray();
			}

			if (PaygentUtility.CanUseBanknetPayment(coCart) == false)
			{
				paymentsUnlimit = paymentsUnlimit
					.Where(item => (item.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_BANKNET))
					.ToArray();
			}

			// メール便の場合は代引き不可
			// 配送方法未定の場合は判定できないのでそのまま
			if ((coCart.Shippings != null) && (coCart.Shippings[0].ShippingMethod != null))
			{
				this.ValidPayments.Add(paymentsUnlimit
					.Where(i => ((i.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT) || coCart.IsExpressDelivery))
					.ToArray());
			}
			else
			{
				// 配送方法未定の場合は判定不可のためそのまま
				this.ValidPayments.Add(paymentsUnlimit);
			}
		}

		//------------------------------------------------------
		// カートリストデータバインド
		//------------------------------------------------------
		// お支払い情報をカート情報へセット
		SetPayment();

		// 配送種別情報取得後＆画面データバインド前
		CreateFixedPurchaseSettings();
	}

	/// <summary>
	/// 販売対象が会員ランク商品か
	/// </summary>
	/// <param name="cartList"></param>
	/// <returns></returns>
	private string CheckLpProductMemberRank(LandingCartProduct[] products)
	{
		// カート選択方式がドロップダウンの場合
		if (this.IsDropDownList) return GetMemberRankErrorMessageForDropDown(products);

		return GetMemberRankErrorMessage(products);
	}

	/// <summary>
	/// エラーメッセージ取得
	/// </summary>
	/// <param name="products">商品情報</param>
	/// <returns>エラーメッセージ</returns>
	private string GetMemberRankErrorMessageForDropDown(LandingCartProduct[] products)
	{
		var product = products.First(item => item.Selected);
		var productModel = new ProductService().Get(product.ShopId, product.ProductId);
		var memberRankModel = MemberRankService.Get(productModel.BuyableMemberRank);

		if (memberRankModel == null) return string.Empty;

		// 未ログインで商品が販売可能会員ランクを設けられているか
		if ((this.IsLoggedIn == false) && (string.IsNullOrEmpty(productModel.BuyableMemberRank) == false))
		{
			return WebMessages.GetMessages(
					WebMessages.ERRMSG_MANAGER_LANDING_PAGE_NOT_LOGIN_AND_PRODUCT_BUYBLE_MEMBER_RANK_ERROR)
					.Replace("@@ 1 @@", memberRankModel.MemberRankName);
		}

		// ユーザーのランクより商品の販売可能ランクの方が高いか
		if ((this.IsLoggedIn) && (memberRankModel.MemberRankOrder < this.LoginMemberRankInfo.MemberRankOrder))
		{
			return WebMessages.GetMessages(
					WebMessages.ERRMSG_MANAGER_LANDING_PAGE_LOWER_USER_RANK_THAN_BUYBLE_MEMBERRANK_ERROR)
					.Replace("@@ 1 @@", productModel.Name)
					.Replace("@@ 2 @@", memberRankModel.MemberRankName);
		}

		return string.Empty;
	}

	/// <summary>
	/// エラーメッセージ取得
	/// </summary>
	/// <param name="products">商品情報</param>
	/// <returns>エラーメッセージ</returns>
	private string GetMemberRankErrorMessage(LandingCartProduct[] products)
	{
		var messages = new List<string>();
		var productService = new ProductService();

		foreach (var product in products)
		{
			var productModel = productService.Get(product.ShopId, product.ProductId);

			if (string.IsNullOrEmpty(productModel.BuyableMemberRank)) continue;
			var memberRankModel = MemberRankService.Get(productModel.BuyableMemberRank);
			if (memberRankModel == null) continue;

			// ゲストユーザーか
			if ((this.IsLoggedIn == false) && (string.IsNullOrEmpty(productModel.BuyableMemberRank) == false))
			{
				// 商品に販売可能会員ランクが設けられているか
				messages.Add(
					WebMessages.GetMessages(
						WebMessages.ERRMSG_MANAGER_LANDING_PAGE_NOT_LOGIN_AND_PRODUCT_BUYBLE_MEMBER_RANK_ERROR)
						.Replace("@@ 1 @@", memberRankModel.MemberRankName));
			}

			// ユーザーのランクより商品の販売可能ランクの方が高いか
			if((this.IsLoggedIn) && (memberRankModel.MemberRankOrder < this.LoginMemberRankInfo.MemberRankOrder))
			{
				messages.Add(
					WebMessages.GetMessages(
						WebMessages.ERRMSG_MANAGER_LANDING_PAGE_LOWER_USER_RANK_THAN_BUYBLE_MEMBERRANK_ERROR)
						.Replace("@@ 1 @@", productModel.Name)
						.Replace("@@ 2 @@", memberRankModel.MemberRankName));
			}
		}

		return string.Join("\r\n", messages);
	}


	/// <summary>
	/// 商品付帯情報チェック
	/// </summary>
	private void CheckProductOptionValueSetting()
	{
		if (this.IsCartListLp) return;

		//------------------------------------------------------
		// 商品付帯情報を別途設定
		//------------------------------------------------------
		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			// ラップ済みコントロール宣言
			WrappedRepeater wrCart = GetWrappedControl<WrappedRepeater>(riCart, "rCart");

			CartObject coCart = this.CartList.GetCart(this.CartList.Items[riCart.ItemIndex].CartId);

			if (coCart != null)
			{
				foreach (RepeaterItem riCartProduct in wrCart.Items)
				{
					var whfShopId = GetWrappedControl<WrappedHiddenField>(riCartProduct, "hfShopId", "");
					var whfProductId = GetWrappedControl<WrappedHiddenField>(riCartProduct, "hfProductId", "");
					var whfVariationId = GetWrappedControl<WrappedHiddenField>(riCartProduct, "hfVariationId", "");
					var whfIsFixedPurchase = GetWrappedControl<WrappedHiddenField>(riCartProduct, "hfIsFixedPurchase", "false");
					var wrProductOptionValueSettings = GetWrappedControl<WrappedRepeater>(riCartProduct, "rProductOptionValueSettings");
					var whfProductSaleId = GetWrappedControl<WrappedHiddenField>(riCartProduct, "hfProductSaleId", "");

					// カート商品取得
					CartProduct cp
						= coCart.GetProduct(
							whfShopId.Value,
							whfProductId.Value,
							whfVariationId.Value,
							false,
							(Constants.FIXEDPURCHASE_OPTION_ENABLED && bool.Parse(whfIsFixedPurchase.Value)) ? Constants.AddCartKbn.FixedPurchase : Constants.AddCartKbn.Normal,
							whfProductSaleId.Value);

					foreach (RepeaterItem riProductOptionValueSetting in wrProductOptionValueSettings.Items)
					{
						var wrCblProductOptionValueSetting = GetWrappedControl<WrappedRepeater>(riProductOptionValueSetting, "rCblProductOptionValueSetting");
						var wddlProductOptionValueSetting = GetWrappedControl<WrappedDropDownList>(riProductOptionValueSetting, "ddlProductOptionValueSetting");
						var wtbProductOptionValueSetting = GetWrappedControl<WrappedTextBox>(riProductOptionValueSetting, "txtProductOptionValueSetting");

						if (wrCblProductOptionValueSetting.Visible)
						{
							var productOptionSetting = cp.ProductOptionSettingList.Items[riProductOptionValueSetting.ItemIndex];
							StringBuilder sbSelectedValue = new StringBuilder();
							var checkBoxCount = 0;
							foreach (RepeaterItem riCheckBox in wrCblProductOptionValueSetting.Items)
							{
								var wcbProductOptionValueSetting = GetWrappedControl<WrappedCheckBox>(riCheckBox, "cbProductOptionValueSetting");
								if (sbSelectedValue.Length != 0)
								{
									sbSelectedValue.Append(Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SELECT_SETTING_VALUE);
								}
								sbSelectedValue.Append(wcbProductOptionValueSetting.Checked ? wcbProductOptionValueSetting.Text : "");
								if (wcbProductOptionValueSetting.Checked) checkBoxCount++;
							}
							if ((checkBoxCount == 0) && productOptionSetting.IsNecessary)
							{
								this.ErrorMessages.Add(riCart.ItemIndex, riCartProduct.ItemIndex, WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_OPTION_NOT_SELECTED_ITEM_ERROR).Replace("@@ 1 @@", productOptionSetting.ValueName));
							}
							else
							{
								cp.ProductOptionSettingList.Items[riProductOptionValueSetting.ItemIndex].SelectedSettingValue = sbSelectedValue.ToString();
							}
						}
						else if (wddlProductOptionValueSetting.Visible)
						{
							var productOptionSetting = cp.ProductOptionSettingList.Items[riProductOptionValueSetting.ItemIndex];
							if ((wddlProductOptionValueSetting.SelectedIndex == 0) && productOptionSetting.IsNecessary)
							{
								this.ErrorMessages.Add(riCart.ItemIndex, riCartProduct.ItemIndex, WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_OPTION_NOT_SELECTED_ITEM_ERROR).Replace("@@ 1 @@", productOptionSetting.ValueName));
							}
							else
							{
								cp.ProductOptionSettingList.Items[riProductOptionValueSetting.ItemIndex].SelectedSettingValue
									= wddlProductOptionValueSetting.SelectedValue == ReplaceTag("@@DispText.variation_name_list.unselected@@")
										? string.Empty
										: wddlProductOptionValueSetting.SelectedValue;
							}
						}
						else if (wtbProductOptionValueSetting.Visible)
						{
							ProductOptionSetting pos = cp.ProductOptionSettingList.Items[riProductOptionValueSetting.ItemIndex];
							string checkKbn = "OptionValueValidate";

							// XML ドキュメントの検証を生成します。
							var validatorXML = pos.CreateValidatorXml(checkKbn);
							var param = new Hashtable();
							param[pos.ValueName] = wtbProductOptionValueSetting.Text;

							var errorMessage = Validator.Validate(checkKbn, validatorXML.InnerXml, param);
							if (string.IsNullOrEmpty(errorMessage) == false)
							{
								this.ErrorMessages.Add(riCart.ItemIndex, riCartProduct.ItemIndex, errorMessage);
							}

							// 設定値には全角スペースと全角：は入力させない
							if (wtbProductOptionValueSetting.Text.Contains('　') || wtbProductOptionValueSetting.Text.Contains('：'))
							{
								this.ErrorMessages.Add(riCart.ItemIndex, riCartProduct.ItemIndex, pos.ValueName + "は全角スペースおよび全角コロン（：）を入力できません。");
							}

							if ((this.ErrorMessages.Count == 0) && (string.IsNullOrWhiteSpace(wtbProductOptionValueSetting.Text) == false))
							{
								cp.ProductOptionSettingList.Items[riProductOptionValueSetting.ItemIndex].SelectedSettingValue = wtbProductOptionValueSetting.Text;
							}
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// 付帯情報が未選択か
	/// </summary>
	/// <param name="cartObject">カートオブジェクト</param>
	/// <returns>付帯情報が未選択か</returns>
	/// <remarks>
	/// 商品付帯情報（ドロップダウン形式、ドロップダウン(価格)形式）が未選択かどうかを判定<br/>
	/// ※付帯価格オプションが有効な時のみ判定する点に注意してください
	/// </remarks>
	private static bool IsUnselectedProductOption(CartObject cartObject)
	{
		var isUnselected = cartObject.Items.Any(
			cartProduct =>
			{
				return cartProduct.ProductOptionSettingList.Items
					.Where(productOptionSetting => productOptionSetting.IsSelectMenu || productOptionSetting.IsDropDownPrice)
					.Any(productOptionSetting => string.IsNullOrEmpty(productOptionSetting.SelectedSettingValue));
			});
		return isUnselected;
	}

	/// <summary>
	/// 入力済みの商品付帯情報を取得
	/// </summary>
	/// <returns>入力済み商品付帯情報</returns>
	private List<InputtedProductOptionValueSetting> GetInputtedProductOptionValueSettings()
	{
		// 入力済みの商品付帯情報を取得
		var inputtedProductOptionValueSettings = new List<InputtedProductOptionValueSetting>();
		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			var wrCart = GetWrappedControl<WrappedRepeater>(riCart, "rCart");
			var cartProductIndex = 0;
			foreach (RepeaterItem riCartProduct in wrCart.Items)
			{
				// 最新カート情報に存在しなければ取得しない
				var whfProductId = GetWrappedControl<WrappedHiddenField>(riCartProduct, "hfProductId");
				var whfVariationId = GetWrappedControl<WrappedHiddenField>(riCartProduct, "hfVariationId");
				if (this.CartList.Items.SelectMany(cart => cart.Items)
					.Any(
						product => ((product.ProductId == whfProductId.Value)
							&& (product.VariationId == whfVariationId.Value))) == false)
				{
					continue;
				}

				var wrProductOptionValueSettings = GetWrappedControl<WrappedRepeater>(riCartProduct, "rProductOptionValueSettings");

				foreach (RepeaterItem riProductOptionValueSetting in wrProductOptionValueSettings.Items)
				{
					var wrCblProductOptionValueSetting = GetWrappedControl<WrappedRepeater>(riProductOptionValueSetting, "rCblProductOptionValueSetting");
					var wddlProductOptionValueSetting = GetWrappedControl<WrappedDropDownList>(riProductOptionValueSetting, "ddlProductOptionValueSetting");
					var wtbProductOptionValueSetting = GetWrappedControl<WrappedTextBox>(riProductOptionValueSetting, "txtProductOptionValueSetting");
					var inputtedValues = new List<string>();

					// チェックボックスの場合
					if (wrCblProductOptionValueSetting.Visible)
					{
						foreach (RepeaterItem riCheckBox in wrCblProductOptionValueSetting.Items)
						{
							var wcbProductOptionValueSetting = GetWrappedControl<WrappedCheckBox>(riCheckBox, "cbProductOptionValueSetting");
							if (wcbProductOptionValueSetting.Checked)
							{
								inputtedValues.Add(wcbProductOptionValueSetting.Text);
							}
						}
					}
					// ドロップダウンリストの場合
					else if (wddlProductOptionValueSetting.Visible)
					{
						inputtedValues.Add(wddlProductOptionValueSetting.SelectedValue);
					}
					// テキストボックスの場合
					else if (wtbProductOptionValueSetting.Visible)
					{
						inputtedValues.Add(wtbProductOptionValueSetting.Text);
					}

					var inputtedProductOptionValueSetting = new InputtedProductOptionValueSetting
					{
						CartIndex = riCart.ItemIndex,
						CartProductIndex = cartProductIndex,
						ProductOptionValueSettingIndex = riProductOptionValueSetting.ItemIndex,
						ProductOptionValueSettingValues = inputtedValues
					};

					inputtedProductOptionValueSettings.Add(inputtedProductOptionValueSetting);
				}
				cartProductIndex++;
			}
		}

		return inputtedProductOptionValueSettings;
	}

	/// <summary>
	/// 指定された値で商品付帯情報をセット
	/// </summary>
	/// <param name="inputtedProductOptionValueSettings">商品付帯情報</param>
	private void SetProductOptionValueSettings(List<InputtedProductOptionValueSetting> inputtedProductOptionValueSettings)
	{
		// 商品付帯情報が0件の場合、処理中止
		if ((inputtedProductOptionValueSettings.Count == 0) || this.IsCartListLp) return;

		foreach (var inputtedProductOptionValueSettingsByCart in inputtedProductOptionValueSettings.GroupBy(setting => setting.CartIndex))
		{
			foreach (var inputtedProductOptionValueSettingsByCartProduct in inputtedProductOptionValueSettingsByCart.GroupBy(setting => setting.CartProductIndex))
			{
				var cartIndex = inputtedProductOptionValueSettingsByCartProduct.First().CartIndex;
				var riCart = this.WrCartList.Items[cartIndex];
				var wrCart = GetWrappedControl<WrappedRepeater>(riCart, "rCart");

				var cartProductIndex = inputtedProductOptionValueSettingsByCartProduct.First().CartProductIndex;
				var riCartProduct = wrCart.Items[cartProductIndex];
				var wrProductOptionValueSettings = GetWrappedControl<WrappedRepeater>(riCartProduct, "rProductOptionValueSettings");

				foreach (var inputtedProductOptionValueSetting in inputtedProductOptionValueSettingsByCartProduct)
				{
					if (inputtedProductOptionValueSetting.ProductOptionValueSettingValues.Count == 0) continue;

					var productOptionValueSettingIndex = inputtedProductOptionValueSetting.ProductOptionValueSettingIndex;
					var riProductOptionValueSetting = wrProductOptionValueSettings.Items[productOptionValueSettingIndex];

					var wrCblProductOptionValueSetting = GetWrappedControl<WrappedRepeater>(riProductOptionValueSetting, "rCblProductOptionValueSetting");
					var wddlProductOptionValueSetting = GetWrappedControl<WrappedDropDownList>(riProductOptionValueSetting, "ddlProductOptionValueSetting");
					var wtbProductOptionValueSetting = GetWrappedControl<WrappedTextBox>(riProductOptionValueSetting, "txtProductOptionValueSetting");

					// チェックボックスの場合
					if (wrCblProductOptionValueSetting.Visible)
					{
						foreach (RepeaterItem riCheckBox in wrCblProductOptionValueSetting.Items)
						{
							var wcbProductOptionValueSetting = GetWrappedControl<WrappedCheckBox>(riCheckBox, "cbProductOptionValueSetting");
							if (inputtedProductOptionValueSetting.ProductOptionValueSettingValues.Contains(wcbProductOptionValueSetting.Text))
							{
								wcbProductOptionValueSetting.Checked = true;
							}
						}
					}
					// ドロップダウンリストの場合
					else if (wddlProductOptionValueSetting.Visible)
					{
						wddlProductOptionValueSetting.SelectedValue = inputtedProductOptionValueSetting.ProductOptionValueSettingValues[0];
					}
					// テキストボックスの場合
					else if (wtbProductOptionValueSetting.Visible)
					{
						wtbProductOptionValueSetting.Text = inputtedProductOptionValueSetting.ProductOptionValueSettingValues[0];
					}
				}
			}
		}
	}

	/// <summary>
	/// 後付款(TriLink後払い)を利用できるかチェックする
	/// </summary>
	private void CheckPaymentForTriLinkAfterPay()
	{
		if (TriLinkAfterPayHelper.CheckUsedPaymentForTriLinkAfterPay(
			this.CartList.Items[0].Payment.PaymentId,
			this.CartList.Items[0].Shippings[0].ShippingCountryIsoCode,
			this.CartList.Items[0].Owner.AddrCountryIsoCode))
		{
			this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_USER_DEFAULT_PAYMENT_SETTING_INVALID_FOR_USER_TRYLINK_AFTERPAY2));
		}
	}

	/// <summary>
	/// 会員登録して次へ
	/// </summary>
	private void UserRegisterAndNextUrl()
	{
		// ユーザー情報取得
		var userInfo = CreateInputData();
		var userId = UserService.CreateNewUserId(
			Constants.CONST_DEFAULT_SHOP_ID,
			Constants.NUMBER_KEY_USER_ID,
			Constants.CONST_USER_ID_HEADER,
			Constants.CONST_USER_ID_LENGTH);
		userInfo.UserId = userId;
		var user = UserRegist(userInfo);

		// IPとアカウント、IPとパスワードでそれぞれアカウントロックがかかっている可能性がある。
		// そのため、登録時にアカウントロックキャンセルを行う
		LoginAccountLockManager.GetInstance().CancelAccountLock(
			this.Page.Request.UserHostAddress,
			user.LoginId,
			user.PasswordDecrypted);

		var loggedUser = this.IsVisible_UserPassword
			? new UserService().TryLogin(user.LoginId, user.PasswordDecrypted, Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED)
			: new UserService().Get(user.UserId);

		// LPカート確認画面へ遷移
		var nextUrl = this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM;

		// 画面遷移の正当性チェックのため遷移先ページURLを設定
		Session[this.LadingCartNextPageForCheck] = Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM;

		// ログインIDをCookieから削除 ※クッキーのログインIDが他者の可能性があるため
		UserCookieManager.CreateCookieForLoginId("", false);

		// ログイン成功アクション実行
		ExecLoginSuccessActionAndGoNextInner(loggedUser, nextUrl, UpdateHistoryAction.Insert);
	}

	/// <summary>
	/// 入力値を取得してUserInputに格納
	/// </summary>
	protected UserInput CreateInputData()
	{
		var wtbLoginId = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbLoginId");
		var wtbPassword = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbPassword");
		var wtbLoginIdInMailAddr = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbLoginIdInMailAddr");
		var wdLoginErrorMessage = GetWrappedControl<WrappedHtmlGenericControl>(this.FirstRpeaterItem, "dLoginErrorMessage", "");
		var wcbAutoCompleteLoginIdFlg = GetWrappedControl<WrappedCheckBox>(this.FirstRpeaterItem, "cbAutoCompleteLoginIdFlg", false);
		var wtbUserLoginId = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbUserLoginId");
		var wtbUserPassword = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbUserPassword");
		var wtbUserPasswordConf = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbUserPasswordConf");

		var userPassword = wtbUserPassword.Text.Trim();
		var userPasswordConf = wtbUserPasswordConf.Text.Trim();

		if ((string)Session[Constants.SESSION_KEY_LP_PASSWORD] != (string)Session[Constants.SESSION_KEY_LP_PASSWORDCONF])
		{
			Session.Remove(Constants.SESSION_KEY_LP_PASSWORD);
			Session.Remove(Constants.SESSION_KEY_LP_PASSWORDCONF);
		}

		// セッションにパスワードが存在するならそちらを適用
		var userSessionInPassword = (string)Session[Constants.SESSION_KEY_LP_PASSWORD];
		var userSessionInPasswordCnf = (string)Session[Constants.SESSION_KEY_LP_PASSWORDCONF];

		if ((string.IsNullOrEmpty(userSessionInPassword)) == false)
		{
			userPassword = userSessionInPassword;
			userPasswordConf = userSessionInPasswordCnf;
		}

		var userInput = new UserInput(new UserModel());

		userInput.LoginId = Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED ? this.CartList.Owner.MailAddr : StringUtility.ToHankaku(wtbUserLoginId.Text);
		userInput.Password = StringUtility.ToHankaku(userPassword);
		userInput.PasswordConf = StringUtility.ToHankaku(userPasswordConf);
		userInput.UserExtendInput = CreateUserExtendInput();

		return userInput;
	}

	/// <summary>
	/// ユーザー拡張項目入力情報を取得
	/// </summary>
	/// <returns>ユーザー拡張項目入力情報</returns>
	private UserExtendInput CreateUserExtendInput()
	{
		UserExtendUserControl wucUserExtend;
		var isSuccess = GetUserExtendControl(out wucUserExtend);

		// ユーザ-拡張項目ユーザコントロールが表示されているかつ存在している場合情報取得
		var userExtendInput = isSuccess
			? wucUserExtend.UserExtend
			: new UserExtendInput();

		return userExtendInput;
	}

	/// <summary>
	/// ユーザー拡張項目入力情報を取得
	/// </summary>
	/// <param name="wucUserExtend">ユーザー拡張項目コントロール</param>
	/// <returns>ユーザー拡張項目コントロール取得結果</returns>
	private bool GetUserExtendControl(out UserExtendUserControl wucUserExtend)
	{
		var wdvUserPassword = GetWrappedControl<WrappedHtmlGenericControl>(this.FirstRpeaterItem, "dvUserPassword");
		wucUserExtend = wdvUserPassword.FindControl("ucBodyUserExtendLandingPageRegist") as UserExtendUserControl;
		return (wdvUserPassword.Visible && (wucUserExtend != null));
	}

	/// <summary>
	/// Register Startup Script
	/// </summary>
	public void RegisterStartupScript()
	{
		Page.ClientScript.RegisterStartupScript(
			this.GetType(),
			"ScrollToTop",
			"ScrollToTop()",
				true);
	}

	/// <summary>
	/// デフォルト決済方法を先頭にする
	/// </summary>
	/// <param name="validPaymentList">有効な決済種別リスト</param>
	/// <param name="defaultPaymentId">デフォルト決済方法</param>
	/// <returns>決済種別リスト</returns>
	public PaymentModel[] SortPaymentListForDefaultCheckedPayment(PaymentModel[] validPaymentList, string defaultPaymentId)
	{
		return string.IsNullOrEmpty(defaultPaymentId)
			? validPaymentList
			: validPaymentList.OrderBy(item => (item.PaymentId == defaultPaymentId) ? 0 : 1).ToArray();
	}

	/// <summary>
	/// Add product to cart from request
	/// </summary>
	private void AddProductToCartFromRequest()
	{
		if (SessionManager.IsOnlyAddCartFirstTime == false) return;

		var productIds = StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_LPCART_PRODUCT_ID]).Split(',');
		var variationIds = StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_LPCART_VARIATION_ID]).Split(',');
		var addCartKbn = StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_LPCART_ADD_CART_KBN]);
		var productCounts = StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_LPCART_PRODUCT_COUNT]).Split(',');
		var timeSaleIds = StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_PRODUCT_SALE_ID]).Split(',');

		if ((productIds.Length == 0)
			|| (variationIds.Length == 0)
			|| string.IsNullOrEmpty(addCartKbn)
			|| (productCounts.Length == 0))
		{
			return;
		}

		var errorMessages = new List<string>();
		for (var index = 0; index < productIds.Length; index++)
		{
			var errorMessage = AddProductToCart(
				this.ShopId,
				productIds[index],
				variationIds[index],
				CheckAddCartKbnFromRequest(addCartKbn),
				int.Parse(productCounts[index]),
				new List<string>(),
				null,
				timeSaleIds[index]);

			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				errorMessages.Add(errorMessage);
			}
		}

		if (errorMessages.Count == 0)
		{
			SessionManager.IsOnlyAddCartFirstTime = false;
		}
	}

	/// <summary>
	/// Check add cart kbn from request
	/// </summary>
	/// <param name="addCartKbn">Add cart kbn</param>
	/// <returns>Cart kbn</returns>
	private Constants.AddCartKbn CheckAddCartKbnFromRequest(string addCartKbn)
	{
		if (addCartKbn == Constants.FLG_ADD_CART_KBN_NORMAL) return Constants.AddCartKbn.Normal;
		return Constants.AddCartKbn.FixedPurchase;
	}

	/// <summary>
	/// 指定されたコントロールにフォーカスを当てる
	/// </summary>
	private void FocusOnSpecifiedControls()
	{
		foreach (var controlId in this.ControlFocusOn)
		{
			var controls = this.Page.FindControlsRecursive(controlId, onlyVisible: true);
			controls.FocusAll();
		}

		// 最後にFocus()されたものが最終的にフォーカスされる。
	}

	/// <summary>
	/// Can use paypal payment
	/// </summary>
	/// <returns>True: Can use paypal payment, False: Can't use paypal payment</returns>
	public bool CanUsePayPalPayment()
	{
		var limitPayment = this.LandingPageDesignModel.UnpermittedPaymentIds.Split(',');
		if (this.IsCmsLandingPage
			&& (limitPayment.Contains(Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL_SBPS)
				|| limitPayment.Contains(Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)))
		{
			return false;
		}
		return true;
	}

	/// <summary>
	/// Check amazon payment landing page design limit
	/// </summary>
	/// <returns>True: Can use amazon payment, False: Can't use amazon payment</returns>
	public bool CheckAmazonPaymentLandingPageDesignLimit()
	{
		var limitPayment = this.LandingPageDesignModel.UnpermittedPaymentIds.Split(',');
		if (this.IsCmsLandingPage
			&& (limitPayment.Contains(Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT))
				|| limitPayment.Contains((Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2)))
		{
			return false;
		}
		return true;
	}

	/// <summary>
	/// Update product list from cart list
	/// </summary>
	private void UpdateProductListFromCartList()
	{
		if ((this.ProductList == null)
			|| (this.ProductList.Length == 0)
			|| (this.CartList.Items.SelectMany(co => co.Items).Any() == false)) return;

		// Get product in cart list
		var cartProducts = this.CartList.Items.SelectMany(co => co.Items);
		foreach (var product in this.ProductList)
		{
			var productInCart = cartProducts.FirstOrDefault(cp =>
				(cp.ShopId == product.ShopId)
					&& (cp.ProductId == product.ProductId)
					&& (cp.VariationId == product.VariationId)
					&& (cp.IsFixedPurchase == product.IsFixedPurchase));
			if (productInCart == null)
			{
				product.Selected = false;
				continue;
			}

			product.ProductCount = productInCart.Count;
			product.Selected = true;
		}
	}

	/// <summary>
	/// Set login error message
	/// </summary>
	/// <param name="sender">Sender</param>
	/// <param name="errorMessage">Error message</param>
	private void SetLoginErrorMessage(object sender, string errorMessage)
	{
		var page = ((Control)sender).Page;
		var loginErrorMessage = WebSanitizer.HtmlEncodeChangeToBr(errorMessage);
		var wdLoginErrorMessage = GetWrappedControl<WrappedHtmlGenericControl>(this.FirstRpeaterItem, "dLoginErrorMessage");
		var scriptSetErrorMessageLogin = string.Format(
			"$('#{0}').html('{1}');",
			wdLoginErrorMessage.ClientID,
			loginErrorMessage);
		ScriptManager.RegisterStartupScript(
			page,
			page.GetType(),
			"SetLoginErrorMessage",
			scriptSetErrorMessageLogin,
			true);
	}

	/// <summary>
	/// Display checkbox regist credit card
	/// </summary>
	private void DisplayCheckboxRegistCreditCard()
	{
		if ((this.IsCmsLandingPage == false) || this.IsLoggedIn) return;

		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			var wrPayment = GetWrappedControl<WrappedRepeater>(riCart, "rPayment");
			foreach (RepeaterItem riPayment in wrPayment.Items)
			{
				var wcbRegistCreditCard = GetWrappedControl<WrappedCheckBox>(riPayment, "cbRegistCreditCard");
				var wdivUserCreditCardName = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "divUserCreditCardName");
				wcbRegistCreditCard.Visible = this.WcbUserRegister.Checked;
				if (this.WcbUserRegister.Checked == false)
				{
					wdivUserCreditCardName.Visible = false;
					wcbRegistCreditCard.Checked = false;
				}
			}
		}
	}

	/// <summary>
	/// Display control authentication
	/// </summary>
	/// <param name="model">Landing page design model</param>
	private void DisplayControlAuthentication(LandingPageDesignModel model)
	{
		if ((Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED == false)
			|| (this.IsCmsLandingPage == false)) return;

		this.AuthenticationUsable = ((model.PersonalAuthenticationUseFlg == LandingPageConst.PERSONAL_AUTHENTICATION_USE_FLG_ON)
			&& (this.WcbUserRegister.Checked || this.IsLoggedIn)
			&& (this.IsSocialLogin == false));

		if (this.AuthenticationUsable)
		{
			var wtbAuthenticationCode = GetWrappedTextBoxAuthenticationCode(this.CartList.Owner.IsAddrJp, this.FirstRpeaterItem);
			var wlbGetAuthenticationCode = GetWrappedControlOfLinkButtonAuthenticationCode(this.CartList.Owner.IsAddrJp, this.FirstRpeaterItem);

			if (this.IsLoggedIn)
			{
				var telNew = this.CartList.Owner.IsAddrJp
					? this.WtbOwnerTel1.Text
					: this.WtbOwnerTel1Global.Text;

				// Check if phone number has not changed
				if (this.CartList.Owner.Tel1 == telNew)
				{
					this.HasAuthenticationCode = true;
					wlbGetAuthenticationCode.Enabled = false;
				}
			}

			if (string.IsNullOrEmpty(this.CartList.AuthenticationCode) == false)
			{
				wtbAuthenticationCode.Text
					= this.AuthenticationCode
					= this.CartList.AuthenticationCode;
				wtbAuthenticationCode.Enabled
					= wlbGetAuthenticationCode.Enabled
					= false;

				this.HasAuthenticationCode = this.CartList.HasAuthenticationCode;
				this.CartList.AuthenticationCode = string.Empty;
				this.CartList.HasAuthenticationCode = false;
			}
		}

		if (model.PersonalAuthenticationUseFlg == LandingPageConst.PERSONAL_AUTHENTICATION_USE_FLG_OFF) this.HasAuthenticationCode = true;
	}

	/// <summary>
	/// カート情報を設定
	/// </summary>
	private void SetCartInformation()
	{
		// 配送先情報作成
		CreateOrderShipping();

		// カート注文メモ作成
		CreateOrderMemo();

		// カート決済情報作成（デフォルトで配送希望は希望するようにする）
		CreateCartPayment();

		// クレジットカード番号再表示の際は、上位４桁と下位４桁以外を空文字設定、セキュリティコードを空文字設定
		AdjustCreditCardNo();

		// カートリストに各情報をセット
		SetCartListForDataBind();
	}

	/// <summary>
	/// 配送先区分が「注文者情報の住所」の配送先情報に郵便番号設定がなかった場合
	/// 配送先情報を注文者情報に初期化する
	/// </summary>
	private void InitializeCartOrderShippingToCartOwnerShippingIfNoZip()
	{
		if (this.CartList.Items.Count == 0) return;

		var targetCart = this.CartList.Items[0];
		if (targetCart.Owner == null) return;

		var shipping = targetCart.Shippings.FirstOrDefault();
		if ((shipping == null)
			|| (string.IsNullOrEmpty(shipping.Zip) == false)
			|| (shipping.ShippingAddrKbn != CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER)) return;

		shipping.UpdateShippingAddr(targetCart.Owner, false);
	}

	/// <summary>
	/// 注文者の郵便番号の入力
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void lbSearchOwnergAddr_Click(object sender, System.EventArgs e)
	{
		((OrderCartPage)this.Page).lbSearchOwnergAddr_Click(sender, e);

		UserExtendUserControl wucUserExtend;
		var isSuccess = GetUserExtendControl(out wucUserExtend);
		if (isSuccess)
		{
			wucUserExtend.WrUserExtendInput.DataSource = wucUserExtend.UserExtendSettingList;
			wucUserExtend.WrUserExtendInput.DataBind();

			if (this.UserExtendInput == null)
			{
				wucUserExtend.SetUserExtendFromDefault();
			}
			else
			{
				wucUserExtend.SetUserExtendFromUserExtendInput(this.UserExtendInput);
			}
		}
	}

	/// <summary>LPカートの商品選択方式</summary>
	public ChooseProductType ChooseProduct { get; set; }
	/// <summary>DropDownListか？</summary>
	public bool IsDropDownList
	{
		get { return (this.ChooseProduct == ChooseProductType.DropDownList); }
	}
	/// <summary>CheckBoxか？</summary>
	public bool IsCheckBox
	{
		get { return (this.ChooseProduct == ChooseProductType.CheckBox); }
	}
	/// <summary>リストから選択か？</summary>
	public bool IsSelectFromList
	{
		get { return (this.IsCheckBox); }
	}
	/// <summary>一つだけ選択するか？</summary>
	protected bool IsChooseOnlyOne
	{
		get { return (this.IsDropDownList); }
	}

	/// <summary>商品番号1</summary>
	public string ProductId1 { get; set; }
	/// <summary>商品番号2</summary>
	public string ProductId2 { get; set; }
	/// <summary>商品番号3</summary>
	public string ProductId3 { get; set; }
	/// <summary>商品番号4</summary>
	public string ProductId4 { get; set; }
	/// <summary>商品番号5</summary>
	public string ProductId5 { get; set; }
	/// <summary>商品番号6</summary>
	public string ProductId6 { get; set; }
	/// <summary>商品番号7</summary>
	public string ProductId7 { get; set; }
	/// <summary>商品番号8</summary>
	public string ProductId8 { get; set; }
	/// <summary>商品番号9</summary>
	public string ProductId9 { get; set; }
	/// <summary>商品番号10</summary>
	public string ProductId10 { get; set; }
	/// <summary>商品番号1投入数</summary>
	public int ProductId1Quantity { get; set; }
	/// <summary>商品番号2投入数</summary>
	public int ProductId2Quantity { get; set; }
	/// <summary>商品番号3投入数</summary>
	public int ProductId3Quantity { get; set; }
	/// <summary>商品番号4投入数</summary>
	public int ProductId4Quantity { get; set; }
	/// <summary>商品番号5投入数</summary>
	public int ProductId5Quantity { get; set; }
	/// <summary>商品番号6投入数</summary>
	public int ProductId6Quantity { get; set; }
	/// <summary>商品番号7投入数</summary>
	public int ProductId7Quantity { get; set; }
	/// <summary>商品番号8投入数</summary>
	public int ProductId8Quantity { get; set; }
	/// <summary>商品番号9投入数</summary>
	public int ProductId9Quantity { get; set; }
	/// <summary>商品番号10投入数</summary>
	public int ProductId10Quantity { get; set; }
	/// <summary>商品番号1購入タイプ</summary>
	public string Product1BuyType { get; set; }
	/// <summary>商品番号2購入タイプ</summary>
	public string Product2BuyType { get; set; }
	/// <summary>商品番号3購入タイプ</summary>
	public string Product3BuyType { get; set; }
	/// <summary>商品番号4購入タイプ</summary>
	public string Product4BuyType { get; set; }
	/// <summary>商品番号5購入タイプ</summary>
	public string Product5BuyType { get; set; }
	/// <summary>商品番号6購入タイプ</summary>
	public string Product6BuyType { get; set; }
	/// <summary>商品番号7購入タイプ</summary>
	public string Product7BuyType { get; set; }
	/// <summary>商品番号8購入タイプ</summary>
	public string Product8BuyType { get; set; }
	/// <summary>商品番号9購入タイプ</summary>
	public string Product9BuyType { get; set; }
	/// <summary>商品番号10購入タイプ</summary>
	public string Product10BuyType { get; set; }
	/// <summary>初期チェック1</summary>
	public bool DefaultChecked1 { get; set; }
	/// <summary>初期チェック2</summary>
	public bool DefaultChecked2 { get; set; }
	/// <summary>初期チェック3</summary>
	public bool DefaultChecked3 { get; set; }
	/// <summary>初期チェック4</summary>
	public bool DefaultChecked4 { get; set; }
	/// <summary>初期チェック5</summary>
	public bool DefaultChecked5 { get; set; }
	/// <summary>初期チェック6</summary>
	public bool DefaultChecked6 { get; set; }
	/// <summary>初期チェック7</summary>
	public bool DefaultChecked7 { get; set; }
	/// <summary>初期チェック8</summary>
	public bool DefaultChecked8 { get; set; }
	/// <summary>初期チェック9</summary>
	public bool DefaultChecked9 { get; set; }
	/// <summary>初期チェック10</summary>
	public bool DefaultChecked10 { get; set; }
	/// <summary>カート投入区分</summary>
	public Constants.AddCartKbn AddCartKbn { get; set; }
	/// <summary>Subscription Box Course Id </summary>
	public string SubscriptionBoxCourseId { get; set; }
	/// <summary>頒布会定額コースか</summary>
	public bool IsSubscriptionBoxFixedAmount
	{
		get
		{
			if (string.IsNullOrEmpty(this.SubscriptionBoxCourseId)) return false;

			var subscriptionBox = new SubscriptionBoxService().GetByCourseId(this.SubscriptionBoxCourseId);
			var result = (subscriptionBox.FixedAmountFlg == Constants.FLG_SUBSCRIPTIONBOX_FIXED_AMOUNT_TRUE);
			return result;
		}
	}
	/// <summary>デフォルト決済方法</summary>
	public string DefaultPaymentId { get; set; }

	/// <summary>Amazon支払いボタンを表示するかどうか</summary>
	protected bool IsVisibleAmazonPayButton
	{
		get
		{
			return ((this.IsAmazonLoggedIn == false)
				&& this.CanUseAmazonPayment());
		}
	}
	/// <summary>商品リスト</summary>
	public LandingCartProduct[] ProductList
	{
		get
		{
			return (LandingCartProduct[])Session[this.LandingCartProductListSessionKey];
		}
		set
		{
			Session[this.LandingCartProductListSessionKey] = value;
		}
	}
	/// <summary>ドロップダウンリスト用コレクション</summary>
	protected ListItemCollection ProductListCollection
	{
		get
		{
			var lic = new ListItemCollection();
			foreach (var item in this.ProductList)
			{
				var li = new ListItem(item.ProductJointName, item.Index.ToString());
				li.Selected = item.Selected;
				lic.Add(li);
			}
			return lic;
		}
	}

	/// <summary>次へ進むイベント格納用</summary>
	public string NextEvent
	{
		get { return "javascript:doPostbackEvenIfCardAuthFailed=false;WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions('" + this.WlbNext.UniqueID + "', '', true, '" + this.WlbNext.ValidationGroup + "', '', false, true))"; }
	}
	/// <summary>次へ進むonclick</summary>
	public string NextOnClick
	{
		get
		{
			if (Constants.PAYMENT_PAIDY_OPTION_ENABLED
				&& (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED == false))
			{
				return "SetPaidyBuyer(); PaidyPayProcess(); return false;";
			}

			if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED)
			{
				return "SetPaidyBuyer(); return CheckBeforeNextPage();";
			}
			return "return true;";
		}
	}
	/// <summary>商品セット選択肢リスト ドロップダウンリスト用コレクション</summary>
	private ListItemCollection ProductSetCollection
	{
		get
		{
			var lic = new ListItemCollection();
			lic.AddRange(this.ValidLandingPageProductSetModels.Select(ps => new ListItem(ps.SetName, ps.BranchNo.ToString())).ToArray());
			return lic;
		}
	}

	/// <summary>メールアドレス確認フォームの表示可否</summary>
	public bool DisplayMailAddressConfirmForm
	{
		get
		{
			return (this.LandingPageDesignModel.MailAddressConfirmFormUseFlg == LandingPageConst.MAIL_ADDRESS_CONFIRM_FORM_USE_FLG_ON);
		}
	}
	// <summary>次へボタンの表示・非表示</summary>
	public bool DisplayNextBtn
	{
		get
		{
			return (this.CartList.Items.Count > 0);
		}
	}
	/// <summary>確認画面のスキップ</summary>
	public bool SkipOrderConfirm
	{
		get
		{
			return (this.LandingPageDesignModel.OrderConfirmPageSkipFlg == LandingPageConst.ORDER_CONFIRM_PAGE_SKIP_FLG_ON);
		}
	}
	/// <summary>CMSランディングページかどうか</summary>
	public bool IsCmsLandingPage { get { return this.LandingPageDesignModel != null; } }
	/// <summary>商品を選択しない？</summary>
	public bool IsNotChoose
	{
		get { return (this.ChooseProduct != ChooseProductType.DoNotChoose); }
	}
	/// <summary>Limited Payment Messages</summary>
	public Hashtable DispLimitedPaymentMessages { get; set; }
	/// <summary>アマゾンアカウントで会員登録しているか</summary>
	public bool IsUserRegistedForAmazon
	{
		get
		{
			var amazonModel = (AmazonModel)Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL];
			if (amazonModel == null) return false;
			return (AmazonUtil.GetUserByAmazonUserId(amazonModel.UserId) != null);
		}
	}
	/// <summary>ログインしているAmazonPayユーザーと同じメールアドレスを持つユーザーが存在するか</summary>
	public bool ExistsUserWithSameAmazonEmailAddress
	{
		get
		{
			var amazonModel = (AmazonModel)Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL];
			if (amazonModel == null) return false;
			var user = new UserInput
			{
				UserId = amazonModel.UserId,
				LoginId = amazonModel.Email
			};
			// DB重複チェック
			var result = (user.ValidateDuplication(UserInput.EnumUserInputValidationKbn.UserRegist).Count > 0);
			return result;
		}
	}
	/// <summary>User default order setting payment id</summary>
	public string UserDefaultOrderSettingPaymentId
	{
		get { return OrderCommon.GetUserDefaultOrderSettingPaymentId(this.LoginUserId); }
	}
	/// <summary>User default order setting shipping</summary>
	public string UserDefaultOrderSettingShipping
	{
		get { return OrderCommon.GetUserDefaultOrderSettingShippingKbn(this.LoginUserId); }
	}
	/// <summary>カートリストランディングページかどうか</summary>
	public new bool IsCartListLp
	{
		get
		{
			var isCartListLp = (Constants.CART_LIST_LP_OPTION
				&& this.IsCmsLandingPage
				&& this.LandingPageDesignModel.IsCartListLp);
			return isCartListLp;
		}
	}

	/// <summary>
	/// 入力された商品付帯情報
	/// </summary>
	private class InputtedProductOptionValueSetting
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public InputtedProductOptionValueSetting()
		{
		}

		/// <summary> 商品付帯情報が入力されたカートインデックス </summary>
		public int CartIndex { get; set; }
		/// <summary> 商品付帯情報が入力された商品インデックス </summary>
		public int CartProductIndex { get; set; }
		/// <summary> 商品付帯情報が入力された商品インデックス </summary>
		public int ProductOptionValueSettingIndex { get; set; }
		/// <summary> 商品付帯情報に入力された値リスト ※チェックボックスもあるためリスト </summary>
		public List<string> ProductOptionValueSettingValues { get; set; }
	}

	/// <summary>アマゾンリクエスト</summary>
	public AmazonCv2Redirect AmazonRequest { get; set; }
	/// <summary>アマゾンチェックアウトセッション</summary>
	public CheckoutSessionResponse AmazonCheckoutSession { get; set; }
	/// <summary>AmazonPayセッション</summary>
	public AmazonModel AmazonPaySessionAmazonModel
	{
		get { return (AmazonModel)Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL]; }
		set { Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL] = value; }
	}
	/// <summary>AmazonPay注文者住所セッション</summary>
	public AmazonAddressModel AmazonPaySessionOwnerAddress
	{
		get { return (AmazonAddressModel)Session[AmazonConstants.SESSION_KEY_AMAZON_OWNER_ADDRESS]; }
		set { Session[AmazonConstants.SESSION_KEY_AMAZON_OWNER_ADDRESS] = value; }
	}
	/// <summary>AmazonPay配送先住所セッション</summary>
	public AmazonAddressModel AmazonPaySessionShippingAddress
	{
		get { return (AmazonAddressModel)Session[AmazonConstants.SESSION_KEY_AMAZON_SHIPPING_ADDRESS]; }
		set { Session[AmazonConstants.SESSION_KEY_AMAZON_SHIPPING_ADDRESS] = value; }
	}
	/// <summary>AmazonPay決済デスクリプター</summary>
	public string AmazonPaySessionPaymentDescriptor
	{
		get { return (string)Session[AmazonConstants.SESSION_KEY_AMAZON_PAYMENT_DESCRIPTOR]; }
		set { Session[AmazonConstants.SESSION_KEY_AMAZON_PAYMENT_DESCRIPTOR] = value; }
	}
	/// <summary>AmazonCv2チェックアウトセッションID</summary>
	public string AmazonCheckoutSessionId
	{
		get { return (string)Session[AmazonCv2Constants.SESSION_KEY_AMAZON_CHECKOUT_SESSION_ID]; }
		set { Session[AmazonCv2Constants.SESSION_KEY_AMAZON_CHECKOUT_SESSION_ID] = value; }
	}
	/// <summary>頒布会更新商品一覧リスト</summary>
	public List<SubscriptionBoxDefaultItemModel> SubscriputionBoxProductListModify
	{
		get { return (List<SubscriptionBoxDefaultItemModel>)Session[Constants.SESSION_KEY_SUBSCRIPTION_BOX_OPTIONAL_LIST]; }
		set { Session[Constants.SESSION_KEY_SUBSCRIPTION_BOX_OPTIONAL_LIST] = value; }
	}
	/// <summary>Amazon自動連携解除アラートを表示する</summary>
	protected bool ShouldDisplayAmazonPayAutoUnlinkAlert
	{
		get { return (string.IsNullOrEmpty(this.Request[Constants.REQUEST_KEY_AMAZON_PAY_AUTO_UNLINK_ALERT]) == false); }
	}
	/// <summary>Is show payment fee</summary>
	public bool IsShowPaymentFee
	{
		get
		{
			var hasCartPayment = this.CartList.Items.Any(co =>
				(co.Payment != null) && (string.IsNullOrEmpty(co.Payment.PaymentId) == false));

			var result = this.IsCmsLandingPage
				&& (this.LandingPageDesignModel.OrderConfirmPageSkipFlg == LandingPageConst.ORDER_CONFIRM_PAGE_SKIP_FLG_ON)
				&& hasCartPayment;
			return result;
		}
	}
}
