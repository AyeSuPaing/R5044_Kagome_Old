/*
=========================================================================================================
  Module      : 注文フロープロセス(OrderFlowProcess.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using w2.App.Common.Amazon;
using w2.App.Common.Amazon.Helper;
using w2.App.Common.Amazon.Util;
using w2.App.Common.AmazonCv2;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.User;
using w2.App.Common.Util;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain.Order;
using w2.Domain.Point;
using w2.Domain.ShopShipping;
using w2.Domain.SubscriptionBox;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

/// <summary>
/// 注文フロープロセス
/// </summary>
public partial class OrderFlowProcess : ProductPageProcess
{

	public WrappedHiddenField WhfAmazonOrderRefID { get { return GetWrappedControl<WrappedHiddenField>(this.FirstRpeaterItem, "hfAmazonOrderRefID"); } }
	public WrappedHiddenField WhfAmazonBillingAgreementId { get { return GetWrappedControl<WrappedHiddenField>(this.FirstRpeaterItem, "hfAmazonBillingAgreementId"); } }
	public WrappedHiddenField WhfAmazonCv2Payload { get { return GetWrappedControl<WrappedHiddenField>("hfAmazonCv2Payload"); } }
	public WrappedHiddenField WhfAmazonCv2Signature { get { return GetWrappedControl<WrappedHiddenField>("hfAmazonCv2Signature"); } }
	public WrappedCheckBox WcbUserRegisterForExternalSettlement { get { return GetWrappedControl<WrappedCheckBox>(this.FirstRpeaterItem, "cbUserRegisterForExternalSettlement", false); } }

	/// <summary>注文情報取得時に算出する、最新の商品IDカウント（SQL側で定義）</summary>
	protected const string CONST_NEWEST_PRODUCT_COUNT = "newest_product_count";

	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="caller">呼び出し元</param>
	/// <param name="viewState">ビューステート</param>
	/// <param name="context">コンテキスト</param>
	public OrderFlowProcess(object caller, StateBag viewState, HttpContext context)
		: base(caller, viewState, context)
	{
	}

	/// <summary>
	/// ページ初期化
	/// </summary>
	public new void Page_Init(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 基底メソッドコール
		//------------------------------------------------------
		base.Page_Init(sender, e);

		//------------------------------------------------------
		// セッション切れチェック
		//------------------------------------------------------
		if (IsPostBack && (Session[Constants.SESSION_KEY_CART_LIST] == null) && (this.IsPreview == false))
		{
			// ポストバックでなおかつカート情報空のときはエラー（トップページからやり直し）
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CART_SESSION_VANISHED);

			if (this is OrderLandingProcess)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] =
					WebMessages.GetMessages(WebMessages.ERRMSG_LANDINGCARTINPUT_INPUT_SESSION_VANISHED);
			}

			var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
				.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOTOP)
				.CreateUrl();
			Response.Redirect(url);
		}

		//------------------------------------------------------
		// カートオブジェクトリストロード
		//------------------------------------------------------
		this.CartList = (this.IsPreview == false) ? GetCartObjectList() : Preview.GetDummyCart(this.ShopId);

		//------------------------------------------------------
		// ポイント情報ロード
		//------------------------------------------------------
		if (Constants.W2MP_POINT_OPTION_ENABLED)
		{
			if (this.LoginUserPoint == null)
			{
				this.LoginUserPoint = new UserPointObject();
			}
		}
	}

	/// <summary>
	/// 利用ポイントチェック
	/// </summary>
	/// <param name="coTarget">対象カートオブジェクト</param>
	/// <returns>エラーメッセージ</returns>
	public string CheckUsePoint(CartObject coTarget)
	{
		var sbErrorMessage = new StringBuilder();

		if (Constants.W2MP_POINT_OPTION_ENABLED)
		{
			//------------------------------------------------------
			// ポイント利用可能単位チェック
			//------------------------------------------------------
			var sv = new PointService();
			var pointMaster = sv.GetPointMaster().Where(x => x.PointKbn == Constants.FLG_POINT_POINT_KBN_BASE);
			if (pointMaster.Any())
			{
				long lUsableUnit = (long)pointMaster.First().UsableUnit;
				if ((coTarget.UsePoint % lUsableUnit) != 0)
				{
					sbErrorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_POINT_USABLE_UNIT_ERROR).Replace("@@ 1 @@", lUsableUnit.ToString()));
				}
			}

			//------------------------------------------------------
			// ポイント利用額チェック
			//------------------------------------------------------
			// ポイント利用額を超えていたらエラー
			if (coTarget.UsePointPrice > coTarget.PointUsablePrice)
			{
				sbErrorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_POINT_PRICE_SUBTOTAL_MINUS_ERROR).Replace("@@ 1 @@", CurrencyManager.ToPrice(coTarget.PointUsablePrice)));
			}
		}

		return StringUtility.ToEmpty(sbErrorMessage.ToString());
	}

	/// <summary>
	/// カート存在チェック
	/// </summary>
	/// <remarks>
	/// カートが存在しない場合、エラーページへ遷移
	/// </remarks>
	public void CheckCartExists()
	{
		CheckCartExists(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CART_SESSION_VANISHED));
	}

	/// <summary>
	/// カート存在チェック
	/// </summary>
	/// <param name="messages">エラーメッセージ</param>
	/// <remarks>
	/// カートが存在しない場合、エラーページへ遷移
	/// </remarks>
	public void CheckCartExists(string messages)
	{
		if ((this.CartList.Items.Count == 0)
			|| (this.CartList.Items.Exists(cart => cart.IsOrderDone)))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = messages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR + "?" + Constants.REQUEST_KEY_ERRORPAGE_KBN + "=" + HttpUtility.UrlEncode(Constants.KBN_REQUEST_ERRORPAGE_GOTOP));
		}
	}

	/// <summary>
	/// カート注文者存在チェック
	/// </summary>
	/// <remarks>
	/// 注文者が存在しない場合カート一覧ページへ飛ぶ
	/// </remarks>
	public void CheckCartOwnerExists()
	{
		if (this.CartList.Owner == null)
		{
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);
		}
	}

	/// <summary>
	/// カート配送先存在チェック
	/// </summary>
	/// <remarks>
	/// 注文者が存在しない場合カート一覧ページへ飛ぶ
	/// </remarks>
	public void CheckCartShippingExists()
	{
		foreach (CartObject cart in this.CartList.Items)
		{
			if ((cart.Shippings.Count == 0)
				|| (cart.Shippings[0].Name1 == null))
			{
				Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);
			}
		}
	}

	/// <summary>
	/// カート注文者チェック
	/// </summary>
	/// <remarks>
	/// メールアドレスが登録されていない OR ログイン状態とカート注文者情報が不整合の場合配送先ページへ飛ぶ
	/// </remarks>
	public void CheckCartOwner()
	{
		if ((this.CartList.Owner == null)
			&& this.IsLoggedIn)
		{
			// ログイン処理で注文者情報が消されるので、ログイン情報から復元
			this.CartList.SetOwner(new CartOwner(this.LoginUser));
			this.CartList.MemberRankId = this.LoginUser.MemberRankId;
		}
		// カート注文者情報が存在しない OR 未ログイン？
		else if ((this.CartList.Owner == null)
			|| (this.IsLoggedIn == false))
		{
			return;
		}

		//------------------------------------------------------
		// カート注文者チェック
		//------------------------------------------------------
		// PC・モバイルメール片方入力許可の場合はモバイルメールアドレスを結合
		StringBuilder mailAddr = new StringBuilder(StringUtility.ToEmpty(this.CartList.Owner.MailAddr));
		if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED)
		{
			mailAddr.Append(StringUtility.ToEmpty(this.CartList.Owner.MailAddr2));
		}

		// ログイン状態とカート注文者情報が不整合（＝ログインしているのにゲストユーザになっている） OR  メールアドレスが登録されていない場合？
		if (UserService.IsGuest(this.CartList.Owner.OwnerKbn)
			|| (Validator.IsNullEmpty(mailAddr.ToString())))
		{
			// 画面遷移の正当性チェックのため遷移先ページURLを設定
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SHIPPING;

			// 画面遷移(配送先入力画面)
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SHIPPING);
		}
	}

	/// <summary>
	/// カート配送先エリアチェック
	/// </summary>
	public void CheckCartOwnerShippingArea()
	{
		foreach (var cart in this.CartList.Items)
		{
			var shippingZip = cart.Shippings[0].HyphenlessZip;
			// カートの配送先情報が配送不可エリアかチェック
			var unavailableShippingArea = OrderCommon.CheckUnavailableShippingArea(this.UnavailableShippingZip, shippingZip);
			// 配送不可エリアメッセージを保持
			if (unavailableShippingArea)
			{
				SessionManager.UnavailableShippingErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ZIPCODE_UNAVAILABLE_SHIPPING_AREA);
			}
		}
		if (string.IsNullOrEmpty(SessionManager.UnavailableShippingErrorMessage)) return;

		// ギフトオプションオンかどうか
		if (Constants.GIFTORDER_OPTION_ENABLED)
		{
			var nextPageForCheck = Constants.GIFTORDER_OPTION_WITH_SHORTENING_GIFT_OPTION_ENABLED
				? Constants.PAGE_FRONT_ORDER_SHIPPING_SELECT
				: Constants.PAGE_FRONT_ORDER_SHIPPING_SELECT_SHIPPING;
			// 画面遷移の正当性チェックのため遷移先ページURLを設定
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = nextPageForCheck;

			// 画面遷移(配送先入力画面)
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + nextPageForCheck);
		}

		// 画面遷移の正当性チェックのため遷移先ページURLを設定
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SHIPPING;

		// 画面遷移(配送先入力画面)
		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SHIPPING);
	}

	/// <summary>
	/// URLセッションチェック(注文時)
	/// </summary>
	/// <param name="hssSession">HttpSessionState</param>
	/// <param name="hrResponse">HttpResponse</param>
	/// <param name="strCurrentPage">現在のページパス</param>
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
	///	３. 注文時のみ使用。URLが存在しない場合はカート画面を表示、URLが一致しない場合はセッションに格納されているURLへ遷移
	///
	/// ※主に、入力データを確認画面で正しく受け取るための制御に使用する。
	/// </remarks>
	public void CheckOrderUrlSession()
	{
		// 遷移元で格納されたURLが存在しない場合はトップ画面へ遷移（イレギュラー）
		string strNextUrl = GetNextUrlForCheck();
		if ((strNextUrl == null))
		{
			Response.Redirect(Constants.PATH_ROOT);
		}

		// 遷移元で格納されたURLと一致しない場合は遷移元で格納されたURLへ遷移
		if (this.RawUrl.IndexOf(strNextUrl) == -1)
		{
			Response.Redirect(Constants.PATH_ROOT + strNextUrl);
		}
	}

	/// <summary>
	/// 郵便番号文字列作成
	/// </summary>
	/// <param name="objCartInfo">カート情報（注文者or配送先）</param>
	/// <returns>郵便番号文字列</returns>
	public string CreateZipString(object objCartInfo)
	{
		if (objCartInfo is CartOwner)
		{
			return CreateZipString(((CartOwner)objCartInfo).Zip1, ((CartOwner)objCartInfo).Zip2);
		}
		else if (objCartInfo is CartShipping)
		{
			return CreateZipString(((CartShipping)objCartInfo).Zip1, ((CartShipping)objCartInfo).Zip2);
		}
		throw new ArgumentException("パラメタエラー: objCartInfo is [" + objCartInfo.GetType().ToString() + "]");
	}
	/// <summary>
	/// 郵便番号文字列作成
	/// </summary>
	/// <param name="strZip1">郵便番号1</param>
	/// <param name="strZip2">郵便番号2</param>
	/// <returns>郵便番号文字列</returns>
	public string CreateZipString(string strZip1, string strZip2)
	{
		return strZip1 + "-" + strZip2; ;
	}

	/// <summary>
	/// 電話番号文字列作成
	/// </summary>
	/// <param name="objCartInfo">カート情報（注文者or配送先）</param>
	/// <returns>電話番号文字列</returns>
	public string CreateTelString(object objCartInfo)
	{
		if (objCartInfo is CartOwner)
		{
			return CreateTelString(((CartOwner)objCartInfo).Tel1_1, ((CartOwner)objCartInfo).Tel1_2, ((CartOwner)objCartInfo).Tel1_3);
		}
		else if (objCartInfo is CartShipping)
		{
			return CreateTelString(((CartShipping)objCartInfo).Tel1_1, ((CartShipping)objCartInfo).Tel1_2, ((CartShipping)objCartInfo).Tel1_3);
		}
		throw new ArgumentException("パラメタエラー: objCartInfo is [" + objCartInfo.GetType().ToString() + "]");
	}
	/// <summary>
	/// 電話番号文字列作成
	/// </summary>
	/// <param name="strTel1">電話番号1</param>
	/// <param name="strTel2">電話番号2</param>
	/// <param name="strTel3">電話番号3</param>
	/// <returns>電話番号文字列</returns>
	public string CreateTelString(string strTel1, string strTel2, string strTel3)
	{
		return strTel1 + "-" + strTel2 + "-" + strTel3;
	}

	/// <summary>
	/// クーポンコード取得
	/// </summary>
	/// <param name="ccCoupon">クーポン情報</param>
	/// <returns>クーポンコード</returns>
	public string GetCouponCode(CartCoupon ccCoupon)
	{
		return (ccCoupon != null) ? ccCoupon.CouponCode : "";
	}

	/// <summary>
	/// 表示用クーポン名取得
	/// </summary>
	/// <param name="ccCoupon">クーポン情報</param>
	/// <returns>表示用クーポン名取得</returns>
	public string GetCouponDispName(CartCoupon ccCoupon)
	{
		return (ccCoupon != null) ? ccCoupon.CouponDispName : "";
	}

	/// <summary>
	/// 商品が有効か?
	/// </summary>
	/// <param name="orderItem">注文商品</param>
	/// <returns>有効：true、無効：false</returns>
	public bool IsProductValid(DataRowView orderItem)
	{
		// 商品の状態が無効の場合、無効（false）を返す
		if ((int)orderItem[CONST_NEWEST_PRODUCT_COUNT] == 0) return false;

		return true;
	}

	/// <summary>
	/// 商品詳細リンク有効か?
	/// </summary>
	/// <param name="orderItem">注文商品</param>
	/// <returns>有効：true、無効：false</returns>
	public bool IsProductDetailLinkValid(DataRowView orderItem)
	{
		var isProductValid = IsProductValid(orderItem);

		return (isProductValid && (Constants.REPEATPLUSONE_OPTION_ENABLED == false));
	}

	/// <summary>
	/// ギフト購入したもののカート商品と配送先情報に紐づく商品情報の整合性チェック
	/// </summary>
	/// <param name="cartList">カートリスト</param>
	/// <returns>整合性チェック結果</returns>
	public bool IsConsistentGiftItemsAndShippings(CartObjectList cartList)
	{
		if (Constants.GIFTORDER_OPTION_ENABLED == false) return true;

		foreach (var cart in cartList.Items.Where(cart => cart.IsGift))
		{
			// カートの商品リスト作成
			var itemList = new SortedDictionary<string, int>();
			foreach (var item in cart.Items)
			{
				if (itemList.ContainsKey(item.VariationId))
				{
					itemList[item.VariationId] += item.Count;
				}
				else
				{
					itemList.Add(item.VariationId, item.Count);
				}
			}

			// 配送先割り当て商品リスト作成
			var shippingList = new SortedDictionary<string, int>();
			foreach (var ship in cart.Shippings)
			{
				foreach (var productCount in ship.ProductCounts)
				{
					if (shippingList.ContainsKey(productCount.Product.VariationId))
					{
						shippingList[productCount.Product.VariationId] += productCount.Count;
					}
					else
					{
						shippingList.Add(productCount.Product.VariationId, productCount.Count);
					}
				}
			}

			if (itemList.Count != shippingList.Count) return false;

			foreach (KeyValuePair<string, int> item in itemList)
			{
				if (shippingList.ContainsKey(item.Key) == false) return false;
				if (shippingList[item.Key] != item.Value) return false;
			}
		}

		return true;
	}

	/// <summary>
	/// 注文拡張項目を表示できるか
	/// </summary>
	/// <returns>表示できるかどうか</returns>
	public bool IsDisplayOrderExtend()
	{
		var result = (Constants.ORDER_EXTEND_OPTION_ENABLED
			&& DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData.SettingModelsForFront.Any());
		return result;
	}

	/// <summary>
	/// 「AmazonPayで会員登録する」チェックボックスクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void cbUserRegisterForExternalSettlement_OnCheckedChanged(object sender, System.EventArgs e)
	{
		var wdvUserBoxVisible = GetWrappedControl<WrappedHtmlGenericControl>(this.FirstRpeaterItem, "dvUserBoxVisible");
		wdvUserBoxVisible.Visible = this.WcbUserRegisterForExternalSettlement.Checked;
	}

	/// <summary>
	/// ユーザ登録処理の実行とログインユーザの取得
	/// </summary>
	/// <param name="user">登録ユーザ</param>
	/// <param name="amazonModel">Amazonから取得した情報</param>
	/// <returns>ログインしたユーザ</returns>
	protected UserModel ExecuteUserRegisterProcessAndReturnLoggedUser(UserModel user, AmazonModel amazonModel)
	{
		// 広告コードより補正
		UserUtility.CorrectUserByAdvCode(user);

		// 登録
		var userService = new UserService();
		userService.InsertWithUserExtend(
			user,
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.DoNotInsert);

		// Userのグローバル情報を更新
		RegionManager.GetInstance().UpdateUserRegion(new RegionModel(), user.UserId);

		// DBから最新情報を取得
		var registedUser = userService.Get(user.UserId);

		// ユーザー拡張項目にAmazonユーザーIDをセット
		AmazonUtil.SetAmazonUserIdForUserExtend(registedUser.UserExtend, registedUser.UserId, amazonModel.UserId);

		// 更新履歴挿入
		new UpdateHistoryService().InsertForUser(registedUser.UserId, Constants.FLG_LASTCHANGED_USER);

		var loggedUser = this.IsVisible_UserPassword
			? userService.TryLogin(registedUser.LoginId, registedUser.PasswordDecrypted, Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED)
			: userService.Get(registedUser.UserId);

		new UserRegister().ExecProcessOnUserRegisted(registedUser, UpdateHistoryAction.DoNotInsert);

		return loggedUser;
	}

	/// <summary>
	/// 注文者情報からユーザ入力情報を作成
	/// </summary>
	/// <param name="amazonModel">アマゾンモデル</param>
	/// <returns>ユーザ入力情報</returns>
	protected UserInput CreateUserInputForAmazonPayByOrderInput(AmazonModel amazonModel)
	{
		// ラップコントロール宣言
		var wtbOwnerName1 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerName1", string.Empty);
		var wtbOwnerName2 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerName2", string.Empty);
		var wtbOwnerNameKana1 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerNameKana1", string.Empty);
		var wtbOwnerNameKana2 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerNameKana2", string.Empty);
		var wddlOwnerBirthYear = GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlOwnerBirthYear", string.Empty);
		var wddlOwnerBirthMonth = GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlOwnerBirthMonth", string.Empty);
		var wddlOwnerBirthDay = GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlOwnerBirthDay", string.Empty);
		var wrblOwnerSex = GetWrappedControl<WrappedRadioButtonList>(this.FirstRpeaterItem, "rblOwnerSex", string.Empty);
		var wtbOwnerMailAddr = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerMailAddr", string.Empty);
		var wtbOwnerMailAddrConf = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerMailAddrConf", string.Empty);
		var wtbOwnerMailAddr2 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerMailAddr2", string.Empty);
		var wtbOwnerMailAddrConf2 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerMailAddrConf2", string.Empty);
		var wtbOwnerZip = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerZip", string.Empty);
		var wddlOwnerAddr1 = GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlOwnerAddr1", string.Empty);
		var wtbOwnerAddr2 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerAddr2", string.Empty);
		var wtbOwnerAddr3 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerAddr3", string.Empty);
		var wtbOwnerAddr4 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerAddr4", string.Empty);
		var wtbOwnerCompanyName = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerCompanyName", string.Empty);
		var wtbOwnerCompanyPostName = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerCompanyPostName", string.Empty);
		var wtbOwnerTel1 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel1", string.Empty);
		var wtbOwnerTel2 = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel2", string.Empty);
		var wcbOwnerMailFlg = this.IsLoggedIn
			? GetWrappedControl<WrappedCheckBox>(this.FirstRpeaterItem, "cbOwnerMailFlg", false)
			: Constants.AMAZON_PAYMENT_CV2_ENABLED
				? GetWrappedControl<WrappedCheckBox>(this.FirstRpeaterItem, "cbGuestOwnerMailFlg2", false)
			: GetWrappedControl<WrappedCheckBox>(this.FirstRpeaterItem, "cbGuestOwnerMailFlg", false);

		// コントロールから値の取得
		var mailAddr = wtbOwnerMailAddr.Text.Trim();
		var mailAddrConf = wtbOwnerMailAddrConf.Text.Trim();
		var mailAddr2 = wtbOwnerMailAddr2.Text.Trim();
		var mailAddr2Conf = wtbOwnerMailAddrConf2.Text.Trim();
		var name1 = wtbOwnerName1.Text.Trim();
		var name2 = wtbOwnerName2.Text.Trim();
		var nameKana1 = wtbOwnerNameKana1.Text.Trim();
		var nameKana2 = wtbOwnerNameKana2.Text.Trim();
		var strBirth = wddlOwnerBirthYear.SelectedValue + "/" + wddlOwnerBirthMonth.SelectedValue + "/" + wddlOwnerBirthDay.SelectedValue;
		DateTime birth;
		var nullrableBirth = DateTime.TryParse(strBirth, out birth)
			? birth
			: (DateTime?)null;
		var sex = wrblOwnerSex.SelectedValue;
		var ownerTel = wtbOwnerTel1.Text.Trim();
		var tel = new Tel(ownerTel);
		var ownerTel2 = wtbOwnerTel2.Text.Trim();
		var tel2 = new Tel(ownerTel2);
		var zip = new ZipCode(wtbOwnerZip.Text.Trim());
		var companyName = wtbOwnerCompanyName.Text.Trim();
		var companyPostName = wtbOwnerCompanyPostName.Text.Trim();
		var addr1 = wddlOwnerAddr1.SelectedValue.Trim();
		var addr2 = wtbOwnerAddr2.Text.Trim();
		var addr3 = wtbOwnerAddr3.Text.Trim();
		var addr4 = wtbOwnerAddr4.Text.Trim();
		var amazonAddress2ToReplace = addr2.Replace("−", "－");
		var mailFlg = wcbOwnerMailFlg.Checked;

		// ユーザ入力情報の作成
		var userInput = new UserInput(new UserModel())
		{
			UserKbn = this.IsSmartPhone
				? Constants.FLG_USER_USER_KBN_SMARTPHONE_USER
				: Constants.FLG_USER_USER_KBN_PC_USER,
			LoginId = mailAddr,
			Name = name1 + name2,
			Name1 = DataInputUtility.ConvertToFullWidthBySetting(name1),
			Name2 = DataInputUtility.ConvertToFullWidthBySetting(name2),
			NameKana = nameKana1 + nameKana2,
			NameKana1 = nameKana1,
			NameKana2 = nameKana2,
			Birth = nullrableBirth.HasValue
				? DateTimeUtility.ToString(
					nullrableBirth.Value,
					DateTimeUtility.FormatType.LongFullDateTimeNoneServerTime,
					this.CartList.Owner.DispLanguageLocaleId)
				: null,
			Sex = sex,
			Tel1 = ownerTel,
			Tel1_1 = tel.Tel1,
			Tel1_2 = tel.Tel2,
			Tel1_3 = tel.Tel3,
			Tel2 = ownerTel2,
			Tel2_1 = tel2.Tel1,
			Tel2_2 = tel2.Tel2,
			Tel2_3 = tel2.Tel3,
			MailAddr = mailAddr,
			MailAddrConf = mailAddrConf,
			MailAddr2 = mailAddr2,
			MailAddr2Conf = mailAddr2Conf,
			Zip = zip.Zip,
			Zip1 = zip.Zip1,
			Zip2 = zip.Zip2,
			AddrCountryName = companyName,
			CompanyPostName = companyPostName,
			AddrCountryIsoCode = this.CartList.Owner.AddrCountryIsoCode,
			Addr = addr1 + companyName,
			Addr1 = StringUtility.ToZenkaku(addr1),
			Addr2 = addr3 == string.Empty
				? StringUtility.ToZenkaku(amazonAddress2ToReplace.Substring(0, amazonAddress2ToReplace.Length - 1))
				: StringUtility.ToZenkaku(amazonAddress2ToReplace),
			Addr3 = addr3 == string.Empty
				? amazonAddress2ToReplace.LastOrDefault().ToString()
				: StringUtility.ToZenkaku(addr3),
			Addr4 = addr4 == string.Empty
				? string.Empty
				: StringUtility.ToZenkaku(addr4),
			MemberRankId = MemberRankOptionUtility.GetDefaultMemberRank(),
			MailFlg = mailFlg ? Constants.FLG_USER_MAILFLG_OK : Constants.FLG_USER_MAILFLG_NG,
			RemoteAddr = this.Page.Request.ServerVariables["REMOTE_ADDR"],
			RecommendUid = UserCookieManager.UniqueUserId,
			LastChanged = Constants.FLG_LASTCHANGED_USER,
			AdvcodeFirst = StringUtility.ToEmpty(Session[Constants.SESSION_KEY_ADVCODE_NOW]),
		};
		return userInput;
	}

	/// <summary>
	/// 注文者情報からユーザ登録
	/// </summary>
	/// <returns>ユーザ情報</returns>
	public UserModel UserRegisterForAmazonPayByOrderOwner()
	{
		// Amazonモデルから情報取得
		var amazonModel = (AmazonModel)Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL];

		var userInput = CreateUserInputForAmazonPayByOrderInput(amazonModel);
		var userId = UserService.CreateNewUserId(
			Constants.CONST_DEFAULT_SHOP_ID,
			Constants.NUMBER_KEY_USER_ID,
			Constants.CONST_USER_ID_HEADER,
			Constants.CONST_USER_ID_LENGTH);
		userInput.UserId = userId;
		var user = userInput.CreateModel();

		var loggedUser = ExecuteUserRegisterProcessAndReturnLoggedUser(user, amazonModel);
		return loggedUser;
	}

	/// <summary>
	/// AmazonPayアカウントで会員登録
	/// </summary>
	/// <returns>ユーザ情報</returns>
	public UserModel UserRegisterAndNextUrlForAmazonPay()
	{
		// AmazonPayから情報取得
		var amazonModel = (AmazonModel)Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL];

		var userInput = CreateUserInputForAmazonPay(amazonModel);
		var userId = UserService.CreateNewUserId(
			Constants.CONST_DEFAULT_SHOP_ID,
			Constants.NUMBER_KEY_USER_ID,
			Constants.CONST_USER_ID_HEADER,
			Constants.CONST_USER_ID_LENGTH);
		userInput.UserId = userId;
		var user = userInput.CreateModel();
		UserUtility.CorrectUserByAdvCode(user);

		// 広告コードより補正
		var userService = new UserService();

		// 登録
		userService.InsertWithUserExtend(
			user,
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.DoNotInsert);

		// Userのグローバル情報を更新
		RegionManager.GetInstance().UpdateUserRegion(new RegionModel(), user.UserId);

		// DBから最新情報を取得
		var registedUser = userService.Get(user.UserId);

		// ユーザー拡張項目にAmazonユーザーIDをセット
		AmazonUtil.SetAmazonUserIdForUserExtend(registedUser.UserExtend, registedUser.UserId, amazonModel.UserId);

		// 更新履歴挿入
		new UpdateHistoryService().InsertForUser(registedUser.UserId, Constants.FLG_LASTCHANGED_USER);

		var loggedUser = this.IsVisible_UserPassword
			? new UserService().TryLogin(registedUser.LoginId, registedUser.PasswordDecrypted, Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED)
			: new UserService().Get(registedUser.UserId);

		new UserRegister().ExecProcessOnUserRegisted(registedUser, UpdateHistoryAction.DoNotInsert);

		return loggedUser;
	}

	/// <summary>
	/// AmazonPayアカウントからUserInputを作成
	/// </summary>
	/// <param name="amazonModel">アマゾンモデル</param>
	/// <returns>ユーザーインプット</returns>
	public UserInput CreateUserInputForAmazonPay(AmazonModel amazonModel)
	{
		var userInput = new UserInput(new UserModel());
		userInput.UserKbn = this.IsSmartPhone ? Constants.FLG_USER_USER_KBN_SMARTPHONE_USER : Constants.FLG_USER_USER_KBN_PC_USER;

		AmazonAddressInput amazonAddressInput;
		if (Constants.AMAZON_PAYMENT_CV2_ENABLED)
		{
			var amazonCheckoutSessionId = (string)Session[AmazonCv2Constants.SESSION_KEY_AMAZON_CHECKOUT_SESSION_ID];
			var checkoutSession = new AmazonCv2ApiFacade().GetCheckoutSession(amazonCheckoutSessionId);
			amazonAddressInput = new AmazonAddressInput(
				checkoutSession.ShippingAddress,
				checkoutSession.Buyer.Email);
		}
		else if (this.CartList.Items[0].HasFixedPurchase)
		{
			var billingAgreementResponse = AmazonApiFacade.GetBillingAgreementDetails(this.WhfAmazonBillingAgreementId.Value, amazonModel.Token);
			amazonAddressInput = new AmazonAddressInput(billingAgreementResponse);
		}
		else
		{
			var orderReferenceResponse = AmazonApiFacade.GetOrderReferenceDetails(this.WhfAmazonOrderRefID.Value, amazonModel.Token);
			amazonAddressInput = new AmazonAddressInput(orderReferenceResponse);
		}
		var amazonAddressModel = AmazonAddressParser.Parse(amazonAddressInput);

		userInput.LoginId = amazonAddressModel.MailAddr;
		userInput.Name = amazonAddressModel.Name.Replace(" ", string.Empty);
		userInput.Name1 = DataInputUtility.ConvertToFullWidthBySetting(amazonAddressModel.Name1);
		userInput.Name2 = DataInputUtility.ConvertToFullWidthBySetting(amazonAddressModel.Name2);
		userInput.NameKana = amazonAddressModel.NameKana;
		userInput.NameKana1 = amazonAddressModel.NameKana1;
		userInput.NameKana2 = amazonAddressModel.NameKana2;

		userInput.Tel1 = amazonAddressModel.Tel;
		userInput.Tel1_1 = amazonAddressModel.Tel1;
		userInput.Tel1_2 = amazonAddressModel.Tel2;
		userInput.Tel1_3 = amazonAddressModel.Tel3;
		userInput.MailAddr = userInput.MailAddrConf = amazonAddressModel.MailAddr;

		userInput.Zip = amazonAddressModel.Zip;
		userInput.Zip1 = amazonAddressModel.Zip1;
		userInput.Zip2 = amazonAddressModel.Zip2;

		userInput.AddrCountryName = this.CartList.Owner.AddrCountryName;
		userInput.AddrCountryIsoCode = this.CartList.Owner.AddrCountryIsoCode;
		userInput.Addr = amazonAddressModel.Addr + userInput.AddrCountryName;
		userInput.Addr1 = StringUtility.ToZenkaku(amazonAddressModel.Addr1);
		// 番地(Addr3)が空白の場合、確認画面から戻った際に会員内容を登録し直さないといけないため、最後の1文字をAddr3に入れる
		var amazonAddress2ToReplace = amazonAddressModel.Addr2.Replace("−", "－");	//（Mac対策）
		userInput.Addr2 = (amazonAddressModel.Addr3 == string.Empty)
			? StringUtility.ToZenkaku(amazonAddress2ToReplace.Substring(0, amazonAddress2ToReplace.Length - 1))
			: StringUtility.ToZenkaku(amazonAddress2ToReplace);
		userInput.Addr3 = (amazonAddressModel.Addr3 == string.Empty)
			? amazonAddress2ToReplace.LastOrDefault().ToString()
			: StringUtility.ToZenkaku(amazonAddressModel.Addr3);
		userInput.Addr4 = (amazonAddressModel.Addr4 == string.Empty)
			? string.Empty
			: StringUtility.ToZenkaku(amazonAddressModel.Addr4);

		userInput.MemberRankId = MemberRankOptionUtility.GetDefaultMemberRank();
		userInput.MailFlg = this.CartList.Owner.MailFlg ? Constants.FLG_USER_MAILFLG_OK : Constants.FLG_USER_MAILFLG_NG;
		userInput.RemoteAddr = this.Page.Request.ServerVariables["REMOTE_ADDR"];
		userInput.RecommendUid = UserCookieManager.UniqueUserId;
		userInput.LastChanged = Constants.FLG_LASTCHANGED_USER;
		userInput.AdvcodeFirst = StringUtility.ToEmpty(Session[Constants.SESSION_KEY_ADVCODE_NOW]);	// 広告コード格納（新規登録時のみ）

		return userInput;
	}

	/// <summary>
	/// Get coupon name
	/// </summary>
	/// <param name="target">Target</param>
	/// <returns>Coupon name</returns>
	public string GetCouponName(object target)
	{
		var couponName = string.Empty;
		if (target is OrderModel)
		{
			var orderModel = (OrderModel)target;
			if (orderModel.OrderCouponUse > 0)
			{
				couponName = string.IsNullOrEmpty(orderModel.Coupons[0].CouponDispName)
					? orderModel.Coupons[0].CouponName
					: orderModel.Coupons[0].CouponDispName;
			}
		}

		if (target is CartObject)
		{
			var cart = (CartObject)target;
			if (cart.UseCouponPrice > 0)
			{
				couponName = string.IsNullOrEmpty(cart.Coupon.CouponDispName)
					? cart.Coupon.CouponName
					: cart.Coupon.CouponDispName;
			}
		}

		if (string.IsNullOrEmpty(couponName)) return string.Empty;

		var result = string.Format("({0}) ", couponName);
		return result;
	}

	/// <summary>
	/// 頒布会必須商品判定
	/// </summary>
	/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
	/// <param name="productId">商品ID</param>
	/// <returns>結果</returns>
	public bool HasNecessaryProduct(string subscriptionBoxCourseId, string productId)
	{
		if (string.IsNullOrEmpty(subscriptionBoxCourseId)) return false;
		var subscriptionBox = new SubscriptionBoxService().GetByCourseId(subscriptionBoxCourseId);

		var subscriptionBoxFirstDefaultItem = (subscriptionBox.IsNumberTime)
			? subscriptionBox.DefaultOrderProducts
				.Where(item => item.Count == 1)
				.ToArray()
			: subscriptionBox.DefaultOrderProducts
				.Where(item => (item.TermSince <= DateTime.Now)
					&& (item.TermUntil >= DateTime.Now))
				.ToArray();
		if (subscriptionBoxFirstDefaultItem.Any(item => item.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_INVALID) == false) return false;

		var necessaryProducts = subscriptionBoxFirstDefaultItem.Where(item => item.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_VALID).ToArray();
		if (necessaryProducts.Any() == false) return false;
		var result = necessaryProducts
			.Where(product => product.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_VALID)
			.Any(p => p.ProductId == productId);
		return result;
	}

	/// <summary>
	/// 頒布会表示名取得
	/// </summary>
	/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
	/// <returns>頒布会表示名</returns>
	public string GetSubscriptionBoxDisplayName(string subscriptionBoxCourseId)
	{
		if (string.IsNullOrEmpty(subscriptionBoxCourseId)) return string.Empty;
		var result = new SubscriptionBoxService().GetDisplayName(subscriptionBoxCourseId);
		return result;
	}

	/// <summary>カートリスト</summary>
	public CartObjectList CartList { get; set; }
	/// <summary>カートリストリピータ</summary>
	public virtual WrappedRepeater WrCartList { get { return GetWrappedControl<WrappedRepeater>("rCartList"); } }
	/// <summary>レコメンド商品が投入されたか？</summary>
	public bool IsAddRecmendItem
	{
		// PostBack時はfalseを返す
		get { return (this.IsPostBack == false) && Request[Constants.REQUEST_KEY_ADD_RECOMMEND_ITEM_FLG] == "1"; }
	}
	/// <summary>Use previous shipping date</summary>
	public bool UsePreviousShippingDate { get; set; }
	/// <summary>アプリの内部ブラウザか（Instagram・Line）</summary>
	public bool IsAppBuiltInBrowser
	{
		get { return m_pattern.IsMatch(StringUtility.ToEmpty(Request.UserAgent).ToLower()); }
	}
	private static readonly Regex m_pattern = new Regex("(instagram|line)");
}
