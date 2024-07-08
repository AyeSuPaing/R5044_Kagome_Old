/*
=========================================================================================================
  Module      : ランディングカート確認画面処理(LandingCartConfrim.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Services;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using w2.App.Common.Amazon;
using w2.App.Common.Amazon.Helper;
using w2.App.Common.Amazon.Util;
using w2.App.Common.AmazonCv2;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.NewebPay;
using w2.App.Common.Order.Payment.Paygent;
using w2.App.Common.Product;
using w2.App.Common.User;
using w2.App.Common.Util;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Logger;
using w2.Common.Web;
using w2.Domain.ContentsLog;
using w2.Domain.LandingPage;
using w2.Domain.Product;
using w2.Domain.SubscriptionBox;
using w2.Domain.User;

public partial class Landing_LandingCartConfirm : OrderCartPageLanding
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス
	WrappedLabel WlblNotFirstTimeFixedPurchaseAlert { get { return GetWrappedControl<WrappedLabel>("lblNotFirstTimeFixedPurchaseAlert"); } }
	protected WrappedHiddenField WhfAtoneToken { get { return GetWrappedControl<WrappedHiddenField>("hfAtoneToken"); } }
	protected WrappedHiddenField WhfAfteeToken { get { return GetWrappedControl<WrappedHiddenField>("hfAfteeToken"); } }
	protected WrappedLabel WlblPaypayErrorMessage { get { return GetWrappedControl<WrappedLabel>("lblPaypayErrorMessage"); } }
	public WrappedHiddenField WhfPaidyPaymentId { get { return GetWrappedControl<WrappedHiddenField>("hfPaidyPaymentId"); } }
	public WrappedHiddenField WhfPaidySelect { get { return GetWrappedControl<WrappedHiddenField>("hfPaidySelect"); } }
	public WrappedHiddenField WhfPaidyStatus { get { return GetWrappedControl<WrappedHiddenField>("hfPaidyStatus"); } }

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (this.IsPreview)
		{
			Preview.PageInvalidateAction(this.Page);
			this.CartList = Preview.GetDummyCart(this.ShopId);
			this.WrCartList.DataSource = this.CartList;

			this.Process.PrepareForDataBindOrderShipping(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);

			CreateFixedPurchaseSettings();

			this.WrCartList.DataBind();

			//プレビュー時、AmazonRequestが初期化されず例外が発生するのを防ぐ
			this.AmazonRequest = AmazonCv2Redirect.SignPayloadForOneTime();
			return;
		}

		this.DispNum = 0;
		this.CartList.CanSetUserDefaultOrderSettingShipping = false;

		var confirmSkipFlg = false;
		if (Session[this.LadingCartConfirmSkipFlgSessionKey] != null)
		{
			Session[this.LadingCartConfirmSkipFlgSessionKey] = null;
			confirmSkipFlg = true;
		}

		if (confirmSkipFlg)
		{
			// 決済金額決定
			// ここで行わないとグローバルONの際にエラーになる
			foreach (var co in this.CartList.Items)
			{
				co.SettlementCurrency = CurrencyManager.GetSettlementCurrency(co.Payment.PaymentId);
				co.SettlementRate = CurrencyManager.GetSettlementRate(co.SettlementCurrency);
				co.SettlementAmount = CurrencyManager.GetSettlementAmount(co.PriceTotal, co.SettlementRate, co.SettlementCurrency);
			}
			// 確認画面スキップONでAtone or Afteeでトークン未取得なら各々の与信取得画面へ遷移させる
			if (((CartObjectList)Session[this.LadingCartSessionKey]).Items
				.Any(cart => (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE) && (string.IsNullOrEmpty(cart.TokenAtone))))
			{
				Session[this.LadingCartConfirmSkipFlgSessionKey] = LandingPageConst.ORDER_CONFIRM_PAGE_SKIP_FLG_ON;
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FROMT_PAYMENT_ATONE_ATONE_EXEC_ORDER_FOR_LANDINGCART);
			}
			if (((CartObjectList)Session[this.LadingCartSessionKey]).Items
				.Any(cart => (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE) && (string.IsNullOrEmpty(cart.TokenAftee))))
			{
				Session[this.LadingCartConfirmSkipFlgSessionKey] = LandingPageConst.ORDER_CONFIRM_PAGE_SKIP_FLG_ON;
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FROMT_PAYMENT_AFTEE_AFTEE_EXEC_ORDER_FOR_LANDINGCART);
			}
		}

		if ((Session[NewebPayConstants.CONST_SESSION_IS_USE_NEWEBPAY] != null)
			&& ((bool)Session[NewebPayConstants.CONST_SESSION_IS_USE_NEWEBPAY]))
		{
			this.CartList = SessionManager.CartList;
			Session[this.LadingCartNextPageForCheck] = SessionManager.NextPageForCheck;
		}
		Session.Remove(NewebPayConstants.CONST_SESSION_IS_USE_NEWEBPAY);

		//------------------------------------------------------
		// HTTPS通信チェック（HTTP通信の場合、ショッピングカートへ）
		//------------------------------------------------------
		CheckHttps();

		//------------------------------------------------------
		// カート存在チェック（カートが存在しない場合、エラーページへ）
		//------------------------------------------------------
		CheckCartExists();

		//------------------------------------------------------
		// カート注文者チェック
		// カート注文者情報とログイン状態に不整合がある場合、配送先入力ページへ戻る（注文者再設定）
		//------------------------------------------------------
		CheckCartOwner();

		//------------------------------------------------------
		// 画面遷移の正当性チェック
		//------------------------------------------------------
		if (!IsPostBack)
		{
			CheckOrderUrlSession();
			foreach (var cart in this.CartList.Items)
			{
				cart.Payment.IsBackFromConfirm = ((cart.Payment != null)
					&& (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					&& (cart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW));
			}

			if ((this.IsCartListLp == false)
				&& (this.CartList.Items.All(
					cartProductItems => cartProductItems.Items.All(product => string.IsNullOrEmpty(product.RecommendId)))))
			{
				Session[this.BackupLadingCartSessionKey] = this.CartList;
			}
		}

		//------------------------------------------------------
		// カート全チェック
		//------------------------------------------------------
		try
		{
			AllCheckCartList();
		}
		catch (OrderException)
		{
			Session[this.LadingCartNextPageForCheck] = this.LandingCartInputAbsolutePath.Replace(Constants.PATH_ROOT, "");
			Response.Redirect(this.LandingCartInputUrl);
		}

		//------------------------------------------------------
		// トークン有効期限チェック
		//-----------------------------------------------------
		var creditTokenCheck = this.CartList.CheckCreditTokenExpired(false);
		if (creditTokenCheck == false)
		{
			Session[this.LadingCartNextPageForCheck] = this.LandingCartInputUrl;
			Response.Redirect(this.LandingCartInputUrl);
		}

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// カートリスト情報に決済手数料を紐付け、配送先情報で配送料含め再計算
			//------------------------------------------------------
			foreach (CartObject co in this.CartList.Items)
			{
				// 決済手数料設定
				// 決済手数料は商品合計金額から計算する（ポイント等で割引された金額から計算してはいけない）
				co.Payment.PriceExchange = OrderCommon.GetPaymentPrice(
					co.ShopId,
					co.Payment.PaymentId,
					co.PriceSubtotal,
					co.PriceCartTotalWithoutPaymentPrice);

				// カート配送先情報で再計算
				if (this.IsLoggedIn && string.IsNullOrEmpty(co.CartUserId)) co.CartUserId = this.LoginUserId;
				co.Calculate(false, isPaymentChanged: true);
			}

			//決済金額決定
			foreach (var co in this.CartList.Items)
			{
				co.SettlementCurrency = CurrencyManager.GetSettlementCurrency(co.Payment.PaymentId);
				co.SettlementRate = CurrencyManager.GetSettlementRate(co.SettlementCurrency);
				co.SettlementAmount = CurrencyManager.GetSettlementAmount(co.PriceTotal, co.SettlementRate, co.SettlementCurrency);
			}

			this.Process.PrepareForDataBindOrderShipping(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);

			//------------------------------------------------------
			// AmazonPayCV2で決済の時使用
			// 配送先情報を注文者情報にセット
			// 注文者情報は外部連携メモにセット
			//------------------------------------------------------
			if (Constants.AMAZON_PAYMENT_CV2_ENABLED &&
				Constants.AMAZON_PAYMENT_CV2_USE_SHIPPING_AS_OWNER_ENABLED &&
				(this.CartList.Items[0].Payment.PaymentId.Equals("K81")))
			{
				var i = this.CartList.Items[0].Payment.PaymentId;
				var j = this.CartList.Items[0].Payment.PaymentName;
				var relationMemo = Constants.CONST_RELATIONMEMO_AMAZON_PAY + "\r\n" +
					"注文者氏名：" + this.CartList.Owner.Name + "\r\n" +
					"メールアドレス：" + this.CartList.Owner.MailAddr;
				this.CartList.Items[0].RelationMemo = relationMemo;
				this.CartList.Owner.Name = this.CartList.Items[0].Shippings[0].Name;
				this.CartList.Owner.Name1 = this.CartList.Items[0].Shippings[0].Name1;
				this.CartList.Owner.Name2 = this.CartList.Items[0].Shippings[0].Name2;
				this.CartList.Owner.Zip1 = this.CartList.Items[0].Shippings[0].Zip1;
				this.CartList.Owner.Zip2 = this.CartList.Items[0].Shippings[0].Zip2;
				this.CartList.Owner.Addr1 = this.CartList.Items[0].Shippings[0].Addr1;
				this.CartList.Owner.Addr2 = this.CartList.Items[0].Shippings[0].Addr2;
				this.CartList.Owner.Addr3 = this.CartList.Items[0].Shippings[0].Addr3;
				this.CartList.Owner.Addr4 = this.CartList.Items[0].Shippings[0].Addr4;
				this.CartList.Owner.Addr5 = this.CartList.Items[0].Shippings[0].Addr5;
				this.CartList.Owner.Tel1_1 = this.CartList.Items[0].Shippings[0].Tel1_1;
				this.CartList.Owner.Tel1_2 = this.CartList.Items[0].Shippings[0].Tel1_2;
				this.CartList.Owner.Tel1_3 = this.CartList.Items[0].Shippings[0].Tel1_3;
			}

			//------------------------------------------------------
			// カートリスト情報を取得
			//------------------------------------------------------
			this.WrCartList.DataSource = this.CartList;

			//------------------------------------------------------
			// データバインド
			//------------------------------------------------------
			this.DataBind();
		}

		// 商品購入制限チェック（類似配送先を含む）
		if (Constants.PRODUCT_ORDER_LIMIT_ENABLED) CheckProductOrderSimilarShipping();

		this.AmazonRequest = this.CartList.HasFixedPurchase
			? AmazonCv2Redirect.SignPayloadForReccuring(this.CartList.PriceCartListTotal)
			: AmazonCv2Redirect.SignPayloadForOneTime();

		// AmazonPayCv2で、通常商品でCheckoutSessionConfigを作ったのに、今のカートに定期商品がある場合(あるいは逆)、もう一度AmazonPayに遷移し同意を取る必要がある
		SessionManager.IsChangedAmazonPayForFixedOrNormal =
			((this.CartList.Items[0].Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2)
				&& (SessionManager.IsAmazonPayGotRecurringConsent != this.CartList.HasFixedPurchase));

		// 確認画面リダイレクトを初期化
		SessionManager.IsRedirectFromLandingCartConfirm = false;

		if (Session[Constants.SESSION_KEY_ERROR_FOR_PAYPAY_PAYMENT] != null)
		{
			this.WlblPaypayErrorMessage.Visible = true;
			var paypayErrMessage = StringUtility.ToEmpty(Session[Constants.SESSION_KEY_ERROR_FOR_PAYPAY_PAYMENT]).Split(':');
			var errCode = (paypayErrMessage.Length > 1) ? string.Format("（{0}）", paypayErrMessage[0]) : string.Empty;
			var errMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAYPAY_AUTH_ERROR) + errCode;
			this.WlblPaypayErrorMessage.Text = errMessage;
			Session.Remove(Constants.SESSION_KEY_ERROR_FOR_PAYPAY_PAYMENT);
		}

		if (confirmSkipFlg)
		{
			// 確認画面のスキップ
			Session[this.LadingCartProductSetSelectSessionKey] = null;
			lbComplete_Click(sender, e);
		}

		this.WhfPaidySelect.Value = this.CartList.Items.Any(item => item.Payment.IsPaymentPaygentPaidy) ? "True" : "False";
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
				return cartProduct.ProductOptionSettingList.Items.Count > 0 && cartProduct.ProductOptionSettingList.Items
					.Where(productOptionSetting => productOptionSetting.IsDropDownPrice)
					.Any(productOptionSetting => string.IsNullOrEmpty(productOptionSetting.SelectedSettingValue));
			});
		return isUnselected;
	}

	/// <summary>
	/// 付帯情報が未選択か
	/// </summary>
	/// <returns>付帯情報が未選択か</returns>
	/// <remarks>
	/// 商品付帯情報（ドロップダウン形式、ドロップダウン(価格)形式）が未選択かどうかを判定<br/>
	/// ※付帯価格オプションが有効な時のみ判定する点に注意してください
	/// </remarks>
	protected bool IsUnselectedProductOption()
	{
		var result = this.CartList.Items
			.Where(x => x.SetPromotions.Items.Count == 0)
			.Any(x => IsUnselectedProductOption(x));
		return result;
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
			Response.Redirect(
				nextUrl.Contains(Constants.PATH_ROOT)
				? nextUrl
				: Constants.PATH_ROOT + nextUrl);
		}
	}

	/// <summary>
	/// 購入商品を過去に購入したことがあるか（類似配送先含む）
	/// </summary>
	protected void CheckProductOrderSimilarShipping()
	{
		this.CartList.CheckFixedProductOrderLimit();
		if ((Constants.ProductOrderLimitKbn.ProductOrderLimitOff == Constants.PRODUCT_ORDER_LIMIT_KBN_CAN_BUY)
			&& (this.CartList.HasOrderHistorySimilarShipping))
		{
			this.ProductOrderLimitErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_NOT_FIXED_PRODUCT_ORDER_LIMIT);
			this.WlblNotFirstTimeFixedPurchaseAlert.Visible = this.HasOrderHistorySimilarShipping;
			this.WlblNotFirstTimeFixedPurchaseAlert.Text = WebSanitizer.HtmlEncodeChangeToBr(this.ProductOrderLimitErrorMessage);
		}
		else if ((Constants.ProductOrderLimitKbn.ProductOrderLimitOn == Constants.PRODUCT_ORDER_LIMIT_KBN_CAN_BUY)
			&& (this.CartList.HasOrderHistorySimilarShipping))
		{
			var duplicateProductNames = new List<string>();
			foreach (CartObject co in this.CartList.Items)
			{
				foreach (CartProduct cp in co.Items.Where(cp => cp.IsOrderLimitProduct))
				{
					FileLogger.Write("ProductOrderLimit",
						string.Format("ユーザーID:「{0}」は、商品ID:「{1}({2})」を過去に購入しています。(注文ID:{3})",
							(this.LoginUserId == null) ? "ゲスト" : this.LoginUserId,
							cp.ProductId,
							cp.ProductName,
							string.Join(",", co.ProductOrderLmitOrderIds)),
						true);

					if ((duplicateProductNames.Contains(cp.ProductName) == false)
						&& co.DuplicateLimitProductIds.Contains(cp.ProductId))
					{
						duplicateProductNames.Add(cp.ProductName);
					}
				}
			}
			Session[Constants.SESSION_KEY_ERROR_MSG] = (duplicateProductNames.Any())
				? string.Format(
					WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_NOT_PRODUCT_ORDER_LP_BY_CART),
					string.Join(",", duplicateProductNames))
				: WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_NOT_PRODUCT_ORDER_LP);
			Session[this.LadingCartNextPageForCheck] = this.LandingCartInputUrl;
			var url = (this.IsCartListLp)
				? Request.UrlReferrer.AbsolutePath
				: Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR;
			Response.Redirect(url);
		}
	}

	/// <summary>
	/// 注文を確定するリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbComplete_Click(object sender, System.EventArgs e)
	{
		var paygentCart = this.CartList.Items.FirstOrDefault(item => item.Payment.IsPaymentPaygentPaidy);
		if ((Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Paygent)
			&& (paygentCart != null))
		{
			if (this.WhfPaidyStatus.Value == Constants.FLG_PAYMENT_PAIDY_API_STATUS_REJECTED)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] =
					WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_FAIL_PAIDY_AUTHORIZED);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}
			else
			{
				paygentCart.Payment.CardTranId = this.WhfPaidyPaymentId.Value;
			}
		}

		this.SubscriputionBoxProductList = null;
		this.SubscriputionBoxProductListModify = null;

		//ランディングページの公開時間が過ぎてる場合、エラー画面に遷移する。
		var pageId = this.CartList.LandingCartPageId;
		if (string.IsNullOrEmpty(pageId) == false)
		{
			var lpPageDesign = new LandingPageService().Get(pageId);
			var startDateTime = lpPageDesign.PublicStartDatetime;
			var endDateTime = lpPageDesign.PublicEndDatetime;
			var timeNow = DateTime.Now;

			if ((endDateTime < timeNow) || (startDateTime > timeNow))
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_LANDING_PAGE_OUT_OF_RELEASED_HOURS_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}
		}

		// 現在のカートのセッションキーをセッションに格納
		Session[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY] = this.LadingCartSessionKey;

		if (Session[this.BackupLadingCartSessionKey] != null)
		{
			// セッションクリア
			Session[this.BackupLadingCartSessionKey] = null;
		}

		// 配送価格未設定エラーが出ているか確認
		CheckGlobalShippingPriceCalcError();

		// 会員登録処理
		UserModel registeredUser = null;
		if (Constants.LANDING_CART_USER_REGISTER_WHEN_ORDER_COMPLETE && (this.RegisterUser != null))
		{
			var userId = UserService.CreateNewUserId(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.NUMBER_KEY_USER_ID,
				Constants.CONST_USER_ID_HEADER,
				Constants.CONST_USER_ID_LENGTH);
			this.RegisterUser.UserId = userId;

			Hashtable mail;
			registeredUser = this.Process.UserRegister(this.RegisterUser, out mail);
			SessionManager.MailForUserRegisterWhenOrderComplete = mail;

			// IPとアカウント、IPとパスワードでそれぞれアカウントロックがかかっている可能性がある。
			// そのため、登録時にアカウントロックキャンセルを行う
			if (string.IsNullOrEmpty(this.RegisterUser.PasswordDecrypted) == false)
			{
				LoginAccountLockManager.GetInstance().CancelAccountLock(
					this.Page.Request.UserHostAddress,
					this.RegisterUser.LoginId,
					this.RegisterUser.PasswordDecrypted);
			}

			if (this.IsAmazonLoggedIn)
			{
				var amazonModel = (AmazonModel)Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL];

				// Userのグローバル情報を更新
				RegionManager.GetInstance().UpdateUserRegion(new RegionModel(), userId);

				// ユーザー拡張項目にAmazonユーザーIDをセット
				AmazonUtil.SetAmazonUserIdForUserExtend(
					registeredUser.UserExtend,
					registeredUser.UserId,
					amazonModel.UserId);
			}

			// ログインIDをCookieから削除 ※クッキーのログインIDが他者の可能性があるため
			UserCookieManager.CreateCookieForLoginId("", false);
		}

		ContentsLogModel contentsExecuteLog = null;
		if (Constants.AB_TEST_OPTION_ENABLED)
		{
			var abTestId = (string)Session[Constants.REQUEST_KEY_AB_TEST_ID];
			if (string.IsNullOrEmpty(abTestId) == false)
			{
				Session[Constants.REQUEST_KEY_AB_TEST_ID] = string.Empty;
				contentsExecuteLog = new ContentsLogModel();

				// キャッシュ取得
				var lpId = CookieManager.GetValue(abTestId);
				var lp = new LandingPageService().Get(lpId);

				contentsExecuteLog.AccessKbn = this.IsPc
					? Constants.FLG_CONTENTSLOG_ACCESS_KBN_PC
					: Constants.FLG_CONTENTSLOG_ACCESS_KBN_SP;
				contentsExecuteLog.ReportType = Constants.FLG_CONTENTSLOG_REPORT_TYPE_ODREXCCV;
				contentsExecuteLog.ContentsId = abTestId + "-" + lp.PageId;
				contentsExecuteLog.ContentsType = Constants.FLG_CONTENTSLOG_CONTENTS_TYPE_ABTEST;
				contentsExecuteLog.Date = DateTime.Now;

				contentsExecuteLog.LogNo = new ContentsLogService().InsertAndGetLogNo(contentsExecuteLog);
			}
		}

		// 注文確定処理を呼び出し、注文完了画面へ遷移
		ExecOrder(this.LadingCartSessionKey, registeredUser, this.LadingCartNextPageForCheck, contentsExecuteLog);
	}

	/// <summary>
	/// カートリピータアイテムコマンド
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rCartList_OnItemCommand(object source, RepeaterCommandEventArgs e)
	{
		var argument = (string)e.CommandArgument;
		switch (e.CommandName.ToLowerInvariant())
		{
			case "gobacklp":
				if (argument.StartsWith("#"))
				{
					GoBackToLandingCart(
						new UrlCreator(this.LandingCartInputUrl)
							.WithUrlFragment(argument)
							.CreateUrl());
				}
				else
				{
					GoBackToLandingCart(
						new UrlCreator(this.LandingCartInputUrl)
							.AddParam(Constants.REQUEST_KEY_FOCUS_ON, argument)
							.CreateUrl());
				}
				break;
		}
	}

	/// <summary>
	/// 戻るリンクボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbBack_Click(object sender, EventArgs e)
	{
		GoBackToLandingCart(this.LandingCartInputUrl);
	}

	/// <summary>
	/// LPカートへ戻る
	/// </summary>
	/// <param name="lpPath">LPのパス</param>
	protected void GoBackToLandingCart(string lpPath)
	{
		if (this.IsBackupLandingCartSession
			&& IsCartChanged((CartObjectList)Session[this.BackupLadingCartSessionKey], this.CartList))
		{
			RestoreCartFromSession();
		}

		SessionManager.IsRedirectFromLandingCartConfirm = true;

		// ブラウザバック制御の為、入力画面のURLを画面遷移正当性チェック用セッションへ格納
		Session[this.LadingCartNextPageForCheck] = lpPath;

		// ターゲットページ設定(ユーザー拡張項目用)
		Session[Constants.SESSION_KEY_TARGET_PAGE + "_extend"] = Constants.PAGE_FRONT_LANDING_LANDING_CART_INPUT;

		// ランディング入力画面へ戻る
		Response.Redirect(lpPath);
	}

	/// <summary>
	/// カートが変更されているか
	/// </summary>
	/// <param name="beforeCartList">前カート</param>
	/// <param name="afterCartList">後カート</param>
	/// <returns>変更されているか</returns>
	private bool IsCartChanged(CartObjectList beforeCartList, CartObjectList afterCartList)
	{
		// カート数が違う
		if (beforeCartList.Items.Count != afterCartList.Items.Count) return true;

		foreach (var afterCart in afterCartList.Items)
		{
			var beforeCart = beforeCartList.Items.FirstOrDefault(cart => cart.CartId == afterCart.CartId);

			if (beforeCart == null) continue;

			// 商品点数が違う
			if (beforeCart.Items.Count != afterCart.Items.Count) return true;

			foreach (var afterProduct in afterCart.Items)
			{
				var beforeProduct = beforeCart.GetSameProduct(afterProduct);

				// 前カートに商品がない
				if (beforeProduct == null) return true;
			}
		}

		// 変化なし
		return false;
	}

	/// <summary>
	/// 決済金額取得
	/// </summary>
	/// <param name="cart">カート</param>
	/// <returns>決済金額合計</returns>
	protected string GetSettlementAmount(CartObject cart)
	{
		var settlementAmount = CurrencyManager.ToSettlementCurrencyNotation(
			cart.SettlementAmount,
			cart.SettlementCurrency);
		return settlementAmount;
	}

	/// <summary>
	///  カート情報エリアのリピータイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rCart_OnItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		var sPriceControl = (HtmlGenericControl)e.Item.FindControl("sPrice");
		if (sPriceControl == null) return;

		var pPriceControl = (HtmlGenericControl)e.Item.FindControl("pPrice");
		var pSubscriptionBoxCampaignPriceControl = (HtmlGenericControl)e.Item.FindControl("pSubscriptionBoxCampaignPrice");
		var sSubscriptionBoxCampaignPriceControl = (HtmlGenericControl)e.Item.FindControl("sSubscriptionBoxCampaignPrice");
		var pSubscriptionBoxCampaignPeriodControl = (HtmlGenericControl)e.Item.FindControl("pSubscriptionBoxCampaignPeriod");
		var sSubscriptionBoxCampaignPeriodSinceControl = (HtmlGenericControl)e.Item.FindControl("sSubscriptionBoxCampaignPeriodSince");
		var sSubscriptionBoxCampaignPeriodUntilControl = (HtmlGenericControl)e.Item.FindControl("sSubscriptionBoxCampaignPeriodUntil");
		var cartProduct = (CartProduct)e.Item.DataItem;

		if (cartProduct.IsSubscriptionBox == false) return;

		var selectedSubscriptionBox = DataCacheControllerFacade
			.GetSubscriptionBoxCacheController()
			.Get(cartProduct.SubscriptionBoxCourseId);

		var subscriptionBoxItem = selectedSubscriptionBox.SelectableProducts.FirstOrDefault(
			x => (x.ProductId == cartProduct.ProductId) && (x.VariationId == cartProduct.VariationId));

		var product = new ProductService().GetProductVariation(this.ShopId, cartProduct.ProductId, cartProduct.VariationId, this.MemberRankId);

		// 頒布会キャンペーン期間の場合キャンペーン期間価格を適用
		if (OrderCommon.IsSubscriptionBoxCampaignPeriod(subscriptionBoxItem))
		{
			sPriceControl.InnerText = CurrencyManager.ToPrice(
					product.FixedPurchasePrice ?? product.Price);
			pPriceControl.Visible
				= pSubscriptionBoxCampaignPeriodControl.Visible
					= true;
			sSubscriptionBoxCampaignPriceControl.InnerText = CurrencyManager.ToPrice(subscriptionBoxItem.CampaignPrice);
			if (subscriptionBoxItem.CampaignSince != null)
			{
				sSubscriptionBoxCampaignPeriodSinceControl.InnerText = HtmlSanitizer.HtmlEncode(
					StringUtility.ToEmpty(
						((DateTime)subscriptionBoxItem.CampaignSince).ToString("yyyy年MM月dd日 HH時mm分")));
			}

			if (subscriptionBoxItem.CampaignUntil != null)
			{
				sSubscriptionBoxCampaignPeriodUntilControl.InnerText = HtmlSanitizer.HtmlEncode(
					StringUtility.ToEmpty(
						((DateTime)subscriptionBoxItem.CampaignUntil).ToString("yyyy年MM月dd日 HH時mm分")));
			}
		}

		// 頒布会コースが定額なら価格非表示
		if (cartProduct.IsSubscriptionBoxFixedAmount())
		{
			pPriceControl.Visible
				= pSubscriptionBoxCampaignPriceControl.Visible
					= pSubscriptionBoxCampaignPeriodControl.Visible
						= false;
		}
	}

	/// <summary>
	/// Get Index Cart Having Payment Atone Or Aftee
	/// </summary>
	/// <param name="isAtone">Is Atone</param>
	/// <returns>List Index</returns>
	[WebMethod]
	public static string GetIndexCartHavingPaymentAtoneOrAftee(bool isAtone)
	{
		var session = HttpContext.Current.Session;
		var page = (Landing_LandingCartConfirm)HttpContext.Current.Handler;
		var cartList = (CartObjectList)session[page.LadingCartSessionKey];

		if (cartList == null) return string.Empty;

		var indexs = new List<int>();
		var paymentId = (isAtone
			? Constants.FLG_PAYMENT_PAYMENT_ID_ATONE
			: Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE);
		foreach (var cart in cartList.Items)
		{
			if ((cart.Payment.PaymentId == paymentId)
				&& (string.IsNullOrEmpty(cart.Payment.CardTranId))) indexs.Add(cartList.Items.IndexOf(cart));
		}
		var result = JsonConvert.SerializeObject(new { indexs = indexs });
		return result;
	}

	/// <summary>
	/// Create Data Atone Aftee Token Landing
	/// </summary>
	/// <param name="index">Cart Index</param>
	/// <param name="isAtone">Is Atone or Aftee</param>
	/// <returns>Data Info</returns>
	[WebMethod]
	public static string CreateDataAtoneAfteeTokenLanding(string index, bool isAtone)
	{
		var cartIndex = int.Parse(index);
		var session = HttpContext.Current.Session;
		var page = (Landing_LandingCartConfirm)HttpContext.Current.Handler;
		var cartList = (CartObjectList)session[page.LadingCartSessionKey];
		var cart = cartList.Items[cartIndex];

		if ((cart.Payment != null)
			&& (string.IsNullOrEmpty(cart.Payment.CardTranId) == false))
		{
			return JsonConvert.SerializeObject(new { data = string.Empty });
		}

		if (string.IsNullOrEmpty(cart.OrderId))
		{
			cart.OrderId = OrderCommon.CreateOrderId(Constants.CONST_DEFAULT_SHOP_ID);
		}
		var result = OrderCommon.CreateDataForAuthorizingAtoneAftee(cart, isAtone);
		return result;
	}

	/// <summary>
	/// Create Data Atone Aftee Token For Landing template
	/// </summary>
	/// <param name="index">Cart Index</param>
	/// <param name="isAtone">Is Atone or Aftee</param>
	/// <returns>Data Info</returns>
	[WebMethod]
	public static new string CreateDataAtoneAfteeToken(string index, bool isAtone)
	{
		var cartIndex = int.Parse(index);
		var session = HttpContext.Current.Session;
		var page = (Landing_LandingCartConfirm)HttpContext.Current.Handler;
		var cartList = (CartObjectList)session[page.LadingCartSessionKey];
		var cart = cartList.Items[cartIndex];

		if ((cart.Payment != null)
			&& (string.IsNullOrEmpty(cart.Payment.CardTranId) == false))
		{
			return JsonConvert.SerializeObject(new { data = string.Empty });
		}

		if (string.IsNullOrEmpty(cart.OrderId))
		{
			cart.OrderId = OrderCommon.CreateOrderId(Constants.CONST_DEFAULT_SHOP_ID);
		}
		var result = OrderCommon.CreateDataForAuthorizingAtoneAftee(cart, isAtone);
		return result;
	}

	/// <summary>
	/// Get first shipping date
	/// </summary>
	/// <param name="shipping">Cart shipping</param>
	/// <returns>First shipping date</returns>
	public string GetFirstShippingDate(CartShipping shipping)
	{
		var result = ((shipping.ShippingMethod
					== Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL)
				|| (shipping.FirstShippingDate == DateTime.MinValue)
				|| (shipping.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS)
				|| (shipping.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY))
			? DateTimeUtility.ToStringFromRegion(
				shipping.GetFirstShippingDate(),
				DateTimeUtility.FormatType.LongDateWeekOfDay1Letter)
			: DateTimeUtility.ToStringFromRegion(
				shipping.FirstShippingDate,
				DateTimeUtility.FormatType.LongDateWeekOfDay1Letter);
		return result;
	}

	/// <summary>購入制限エラーメッセージ</summary>
	protected string ProductOrderLimitErrorMessage
	{
		get { return (string)ViewState["ProductOrderLimitErrorMessage"]; }
		set { ViewState["ProductOrderLimitErrorMessage"] = value; }
	}
	/// <summary>LPでフォーカスを当てるコントロールID：注文者情報</summary>
	protected string FocusingControlsOnCartList
	{
		get { return "#CartList"; }
	}
	/// <summary>LPでフォーカスを当てるコントロールID：注文者情報</summary>
	protected string FocusingControlsOnOrderOwner
	{
		get { return "tbOwnerName1"; }
	}
	/// <summary>LPでフォーカスを当てるコントロールID：配送先情報</summary>
	protected string FocusingControlsOnOrderShipping
	{
		get { return this.IsAmazonLoggedIn ? "#shippingAddressBookWidgetDiv" : "ddlShippingKbnList,tbShippingName1"; }
	}
	/// <summary>LPでフォーカスを当てるコントロールID：決済情報</summary>
	protected string FocusingControlsOnOrderPayment
	{
		get { return this.IsAmazonLoggedIn ? "#walletWidgetDiv" : "rbgPayment"; }
	}
	/// <summary>アマゾンリクエスト</summary>
	protected AmazonCv2Redirect AmazonRequest { get; set; }
	/// <summary>頒布会商品一覧リスト</summary>
	public List<SubscriptionBoxDefaultItemModel> SubscriputionBoxProductList
	{
		get { return (List<SubscriptionBoxDefaultItemModel>)Session[Constants.SESSION_KEY_SUBSCRIPTION_BOX_LIST]; }
		set { Session[Constants.SESSION_KEY_SUBSCRIPTION_BOX_LIST] = value; }
	}
	/// <summary>頒布会更新商品一覧リスト</summary>
	public List<SubscriptionBoxDefaultItemModel> SubscriputionBoxProductListModify
	{
		get { return (List<SubscriptionBoxDefaultItemModel>)Session[Constants.SESSION_KEY_SUBSCRIPTION_BOX_OPTIONAL_LIST]; }
		set { Session[Constants.SESSION_KEY_SUBSCRIPTION_BOX_OPTIONAL_LIST] = value; }
	}
}
