/*
=========================================================================================================
  Module      : LP入力フォーム処理(LpInputForm.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using Amazon.Pay.API.WebStore.CheckoutSession;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using w2.App.Common.Amazon.Helper;
using w2.App.Common.AmazonCv2;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Line.Util;
using w2.App.Common.Order;
using w2.App.Common.Product;
using w2.App.Common.User.SocialLogin.Helper;
using w2.App.Common.User.SocialLogin.Util;
using w2.App.Common.Web.Process;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain.LandingPage;
using w2.Domain.Product;
using w2.Domain.SubscriptionBox;
using w2.Domain.User;

public partial class Landing_formlp_LpInputForm : OrderCartUserControl
{
	#region ラップ済コントロール宣言
	protected WrappedRepeater WrCartList { get { return this.Process.WrCartList; } }
	protected WrappedHtmlGenericControl WhgcShippingAddressBookErrorMessage { get { return this.Process.WhgcShippingAddressBookErrorMessage; } }
	protected WrappedHtmlGenericControl WhgcOwnerAddressBookErrorMessage { get { return this.Process.WhgcOwnerAddressBookErrorMessage; } }
	protected WrappedHiddenField WhfAmazonOrderRefID { get { return this.Process.WhfAmazonOrderRefID; } }
	protected WrappedHiddenField WhfAmazonBillingAgreementId { get { return this.Process.WhfAmazonBillingAgreementId; } }
	protected WrappedHiddenField WhfAmazonCv2Payload { get { return this.Process.WhfAmazonCv2Payload; } }
	protected WrappedHiddenField WhfAmazonCv2Signature { get { return this.Process.WhfAmazonCv2Signature; } }
	protected WrappedCheckBox WcbShipToOwnerAddress { get { return this.Process.WcbShipToOwnerAddress; } }

	// ログイン関係
	protected WrappedTextBox WtbOwnerMailAddr { get { return this.Process.WtbOwnerMailAddr; } }
	protected WrappedTextBox WtbOwnerMailAddrConf { get { return this.Process.WtbOwnerMailAddrConf; } }
	protected WrappedHiddenField WhfSocialLoginJson { get { return this.Process.WhfSocialLoginJson; } }
	protected WrappedCheckBox WcbUserRegister { get { return this.Process.WcbUserRegister; } }
	protected WrappedDropDownList WddlProductSet { get { return this.Process.WddlProductSet; } }
	protected WrappedCheckBox WcbUserRegisterForExternalSettlement { get { return this.Process.WcbUserRegisterForExternalSettlement; } }

	protected WrappedTextBox WtbOwnerName1 { get { return this.Process.WtbOwnerName1; } }
	protected WrappedTextBox WtbOwnerName2 { get { return this.Process.WtbOwnerName2; } }
	protected WrappedTextBox WtbOwnerNameKana1 { get { return this.Process.WtbOwnerNameKana1; } }
	protected WrappedTextBox WtbOwnerNameKana2 { get { return this.Process.WtbOwnerNameKana2; } }
	protected WrappedDropDownList WddlOwnerBirthYear { get { return this.Process.WddlOwnerBirthYear; } }
	protected WrappedDropDownList WddlOwnerBirthMonth { get { return this.Process.WddlOwnerBirthMonth; } }
	protected WrappedDropDownList WddlOwnerBirthDay { get { return this.Process.WddlOwnerBirthDay; } }
	protected WrappedRadioButtonList WrblOwnerSex { get { return this.Process.WrblOwnerSex; } }
	protected WrappedTextBox WtbOwnerZip1 { get { return this.Process.WtbOwnerZip1; } }
	protected WrappedTextBox WtbOwnerZip2 { get { return this.Process.WtbOwnerZip2; } }
	protected WrappedDropDownList WddlOwnerAddr1 { get { return this.Process.WddlOwnerAddr1; } }
	protected WrappedTextBox WtbOwnerAddr2 { get { return this.Process.WtbOwnerAddr2; } }
	protected WrappedTextBox WtbOwnerAddr3 { get { return this.Process.WtbOwnerAddr3; } }
	protected WrappedTextBox WtbOwnerTel1_1 { get { return this.Process.WtbOwnerTel1_1; } }
	protected WrappedTextBox WtbOwnerTel1_2 { get { return this.Process.WtbOwnerTel1_2; } }
	protected WrappedTextBox WtbOwnerTel1_3 { get { return this.Process.WtbOwnerTel1_3; } }
	protected WrappedTextBox WtbOwnerZip { get { return this.Process.WtbOwnerZip; } }
	protected WrappedTextBox WtbOwnerTel1 { get { return this.Process.WtbOwnerTel1; } }
	protected WrappedDropDownList WddlOwnerCountry { get { return this.Process.WddlOwnerCountry; } }
	protected WrappedTextBox WtbOwnerTel1Global { get { return this.Process.WtbOwnerTel1Global; } }

	protected WrappedTextBox WtbUserPassword { get { return this.Process.WtbUserPassword; } }
	protected WrappedTextBox WtbUserPasswordConf { get { return this.Process.WtbUserPasswordConf; } }
	protected WrappedButton WdbtnUserPasswordReType { get { return this.Process.WdbtnUserPasswordReType; } }
	protected WrappedLiteral WdlUserPassword { get { return this.Process.WdlUserPassword; } }
	protected WrappedLiteral WdlUserPasswordConf { get { return this.Process.WdlUserPasswordConf; } }
	protected WrappedUpdatePanel WupPasswordUpdatePanel { get { return this.Process.WupPasswordUpdatePanel; } }

	/// <summary>Paidy token hidden field control</summary>
	protected WrappedHiddenField WhfPaidyTokenId { get { return this.Process.WhfPaidyTokenId; } }
	/// <summary>Paidy pay seleced hidden field control</summary>
	protected WrappedHiddenField WhfPaidyPaySelected { get { return this.Process.WhfPaidyPaySelected; } }

	/// <summary>Wrapped label authentication status</summary>
	protected WrappedLabel WlbAuthenticationStatus { get { return this.Process.WlbAuthenticationStatus; } }
	/// <summary>Wrapped label Authentication status global</summary>
	protected WrappedLabel WlbAuthenticationStatusGlobal { get { return this.Process.WlbAuthenticationStatusGlobal; } }
	/// <summary>Wrapped label authentication message</summary>
	protected WrappedLabel WlbAuthenticationMessage { get { return this.Process.WlbAuthenticationMessage; } }
	/// <summary>Wrapped label authentication message global</summary>
	protected WrappedLabel WlbAuthenticationMessageGlobal { get { return this.Process.WlbAuthenticationMessageGlobal; } }

	protected WrappedRepeater WrCart { get { return GetWrappedControl<WrappedRepeater>("rCart"); } }

	# endregion

	/// <summary>ログインフォーム表示するか</summary>
	protected bool DisplayLoginForm { get; set; }
	/// <summary>ユーザー登録を利用するか</summary>
	protected bool UserRegisterUsable { get; set; }
	/// <summary>ユーザー登録デフォルトチェックするか</summary>
	protected bool UserRegisterDefaultChecked { get; set; }
	/// <summary>ノベルティ設定を利用するか</summary>
	protected bool NoveltyUsable { get; set; }

	/// <summary>
	/// ページ初期化
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected new void Page_Init(object sender, EventArgs e)
	{
		this.Process.Page_Init(sender, e);

		ProductSetItemSetting(new LandingPageProductSetModel());
	}

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			//ユーザーが退会済みでないか確認
			if (string.IsNullOrEmpty(this.LoginUserId) == false)
			{
				var user = new UserService().Get(this.LoginUserId);
				if ((user != null) && user.IsDeleted)
				{
					Session.Contents.RemoveAll();
					CookieManager.RemoveCookie(Constants.COOKIE_KEY_AUTH_KEY, Constants.PATH_ROOT);
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CART_SESSION_VANISHED);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}
			}
		}
		DisplayControlLoginForm(this.LandingPageDesignModel);
		DisplayControlUerRegisterCheckBox(this.LandingPageDesignModel);
		DisplayControlProductChoose(this.LandingPageDesignModel);
		DisplayControlNovelty(this.LandingPageDesignModel);

		this.SubscriputionBoxProductList = this.SubscriputionBoxProductList ?? new List<SubscriptionBoxDefaultItemModel>();
		this.SubscriputionBoxProductListModify = this.SubscriputionBoxProductListModify ?? new List<SubscriptionBoxDefaultItemModel>();

		if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED)
		{
			UpdateCartShippingByReceivingStore();
		}

		if (IsPostBack)
		{
			BindSubscriptionItemData();
		}

		this.Process.Page_Load(sender, e);
	}

	/// <summary>
	/// ログインフォーム表示
	/// </summary>
	/// <param name="model">LPページデザインモデル</param>
	private void DisplayControlLoginForm(LandingPageDesignModel model)
	{
		this.DisplayLoginForm = (model.LoginFormType == LandingPageConst.LOGIN_FORM_TYPE_VISIBLE);
	}

	/// <summary>
	/// ユーザー登録チェックボックス表示
	/// </summary>
	/// <param name="model">LPページデザインモデル</param>
	private void DisplayControlUerRegisterCheckBox(LandingPageDesignModel model)
	{
		switch (model.UserRegistrationType)
		{
			case LandingPageConst.USER_REGISTRATION_TYPE_AUTO:
				this.UserRegisterDefaultChecked = (this.IsLoggedIn == false);
				this.UserRegisterUsable = false;
				break;

			case LandingPageConst.USER_REGISTRATION_TYPE_DISABLE:
				this.UserRegisterDefaultChecked = false;
				this.UserRegisterUsable = false;
				break;

			case LandingPageConst.USER_REGISTRATION_TYPE_MANUAL:
				this.UserRegisterDefaultChecked = true;
				this.UserRegisterUsable = true;
				break;
		}
	}

	/// <summary>
	/// 商品選択表示
	/// </summary>
	/// <param name="model">LPページデザインモデル</param>
	private void DisplayControlProductChoose(LandingPageDesignModel model)
	{
		switch (model.ProductChooseType)
		{
			case LandingPageConst.PRODUCT_CHOOSE_TYPE_DONOTCHOOSE:
				this.ChooseProduct = OrderLandingInputProcess.ChooseProductType.DoNotChoose;
				break;
			case LandingPageConst.PRODUCT_CHOOSE_TYPE_CHECKBOX:
				this.ChooseProduct = OrderLandingInputProcess.ChooseProductType.CheckBox;
				break;
			case LandingPageConst.PRODUCT_CHOOSE_TYPE_DROPDOWNLIST:
				this.ChooseProduct = OrderLandingInputProcess.ChooseProductType.DropDownList;
				break;
		}
	}

	/// <summary>
	/// ノベルティ表示
	/// </summary>
	/// <param name="model">LPページデザインモデル</param>
	private void DisplayControlNovelty(LandingPageDesignModel model)
	{
		this.NoveltyUsable = (Constants.NOVELTY_OPTION_ENABLED
			&& (model.NoveltyUseFlg == LandingPageConst.NOVELTY_USE_FLG_ON));
	}

	protected /*override★*/ void OnAddProductError(LandingCartProduct product, string errorMsg)
	{
		// カート投入できなかった商品をログに書いておく
		var msg = string.Format("LPのカート投入に失敗。商品ID：{0} バリエーションID：{1} エラー内容：{2} ページURL：{3}", product.ProductId, product.VariationId, errorMsg, this.Request.Url.AbsoluteUri);
		w2.Common.Logger.FileLogger.WriteWarn(msg);
	}

	/// <summary>
	/// カートノベルティ取得
	/// </summary>
	/// <param name="cartId">カートID</param>
	/// <returns>カートノベルティ</returns>
	public CartNovelty[] GetCartNovelty(string cartId)
	{
		return this.Process.GetCartNovelty(cartId);
	}

	/// <summary>
	/// カート再作成リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCreateCart_Click(object sender, System.EventArgs e)
	{
		this.Process.lbCreateCart_Click(sender, e);
	}

	/// <summary>
	/// 再計算リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected new void lbRecalculate_Click(object sender, System.EventArgs e)
	{
		this.Process.lbRecalculate_Click(sender, e);
	}

	/// <summary>
	/// レジへ進むリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbNext_Click(object sender, System.EventArgs e)
	{
		this.Process.lbNext_Click(sender, e);
	}

	/// <summary>
	/// 商品選択ドロップダウンリスト
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlProductList_OnSelectedIndexChanged(object sender, System.EventArgs e)
	{
		this.Process.ddlProductList_OnSelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// リピータイベント
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected override void rCartList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		this.Process.rCartList_ItemCommand(source, e);
	}

	/// <summary>
	/// 次回購入の利用ポイントの全適用フラグ変更時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbUseAllPointFlg_Changed(object sender, EventArgs e)
	{
		this.Process.cbUseAllPointFlg_Changed(sender, e);
	}

	/// <summary>
	/// 次回購入の利用ポイントの全適用フラグデータバインド時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbUseAllPointFlg_DataBinding(object sender, EventArgs e)
	{
		this.Process.cbUseAllPointFlg_DataBinding(sender, e);
	}

	/// <summary>
	/// （トークン決済向け）カード情報編集リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbEditCreditCardNoForToken_Click(object sender, System.EventArgs e)
	{
		this.Process.lbEditCreditCardNoForToken_Click(sender, e);
	}

	/// <summary>
	/// クーポン入力方法変更時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblCouponInputMethod_SelectedIndexChanged(object sender, EventArgs e)
	{
		this.Process.rblCouponInputMethod_SelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// クーポン入力方法データバインド時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblCouponInputMethod_DataBinding(object sender, EventArgs e)
	{
		this.Process.rblCouponInputMethod_DataBinding(sender, e);
	}

	/// <summary>
	/// 注文者の郵便番号の入力
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void lbSearchOwnergAddr_Click(object sender, EventArgs e)
	{
		this.Process.lbSearchOwnergAddr_Click(sender, e);
	}

	/// <summary>
	/// クーポンリスト変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlCouponList_TextChanged(object sender, EventArgs e)
	{
		this.Process.ddlCouponList_TextChanged(sender, e);
	}

	/// <summary>
	/// クーポンBOXクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbShowCouponBox_Click(object sender, EventArgs e)
	{
		this.Process.lbShowCouponBox_Click(sender, e);
	}

	/// <summary>
	/// モーダルクーポンBOX クーポン選択時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCouponSelect_Click(object sender, EventArgs e)
	{
		this.Process.lbCouponSelect_Click(sender, e);
	}

	/// <summary>
	/// モーダルクーポンBOX 閉じるボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCouponBoxClose_Click(object sender, EventArgs e)
	{
		this.Process.lbCouponBoxClose_Click(sender, e);
	}

	/// <summary>
	/// 配送方法選択ドロップダウンリスト
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected new void ddlShippingMethodList_OnSelectedIndexChanged(object sender, System.EventArgs e)
	{
		this.Process.ddlShippingMethodList_OnSelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// 配送サービス選択ドロップダウンリスト
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected new void ddlDeliveryCompanyList_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		this.Process.ddlDeliveryCompanyList_OnSelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// Radio button group payment on checked changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected new void rbgPayment_OnCheckedChanged(object sender, System.EventArgs e)
	{
		this.Process.rbgPayment_OnCheckedChanged(sender, e);
	}

	/// <summary>
	/// ログインボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbLogin_Click(object sender, EventArgs e)
	{
		this.Process.lbLogin_Click(sender, e);
	}

	/// <summary>
	/// 「会員登録する」チェックボックスクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbUserRegister_OnCheckedChanged(object sender, System.EventArgs e)
	{
		this.Process.cbUserRegister_OnCheckedChanged(sender, e);
	}

	/// <summary>
	/// 「AmazonPayで会員登録する」チェックボックスクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbUserRegisterForExternalSettlement_OnCheckedChanged(object sender, System.EventArgs e)
	{
		this.Process.cbUserRegisterForExternalSettlement_OnCheckedChanged(sender, e);
	}

	/// <summary>
	/// Amazonお支払いをやめるクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCancelAmazonPay_Click(object sender, System.EventArgs e)
	{
		this.Process.lbCancelAmazonPay_Click(sender, e);
	}

	/// <summary>
	/// 楽天IDConnectリクエストクリック（ログイン）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbRakutenIdConnectRequestAuth_Click(object sender, EventArgs e)
	{
		this.Process.lbRakutenIdConnectRequestAuth_Click(sender, e);
	}

	/// <summary>
	/// Is Display EcPay Credit Installment
	/// </summary>
	/// <param name="payment">Payment</param>
	/// <returns>True: If there is a Credit Installer setting and the External payment type is Credit</returns>
	protected bool IsDisplayEcPayCreditInstallment(CartPayment payment)
	{
		return this.Process.IsDisplayCreditInstallment(payment);
	}

	/// <summary>
	/// Dropdown List Ec Payment Selected Index Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlEcPayment_SelectedIndexChanged(object sender, EventArgs e)
	{
		this.Process.ddlEcPayment_OnSelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// DropDownList Shipping Kbn List Selected Index Changed Event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected new void ddlShippingKbnList_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		this.Process.ddlShippingKbnList_OnSelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// 初期値男性で性別を取得（データバインド用）
	/// </summary>
	/// <returns></returns>
	protected string GetCorrectSexForDataBindDefault()
	{
		return this.Process.GetCorrectSexForDataBindDefault();
	}

	/// <summary>
	/// カスタムバリデータの属性値を変更する（EFOオプションONのとき、カスタムバリデータを無効化する）
	/// </summary>
	public void UpdateAttributeValueForCustomValidator()
	{
		this.Process.UpdateAttributeValueForCustomValidator();
	}

	/// <summary>
	/// ユーザー管理レベルにより制限されているかどうかチェック
	/// </summary>
	/// <param name="productId">商品Id</param>
	/// <param name="productName">商品名</param>
	protected void CheckUserLevelIsLimited(string productId, string productName)
	{
		this.Process.CheckUserLevelIsLimited(productId, productName);
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
	/// Linkbutton search address owner from zipcode global click event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSearchAddrOwnerFromZipGlobal_Click(object sender, EventArgs e)
	{
		this.Process.lbSearchAddrOwnerFromZipGlobal_Click(sender, e);
	}

	/// <summary>
	/// Linkbutton search address shipping from zipcode global click event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSearchAddrShippingFromZipGlobal_Click(object sender, EventArgs e)
	{
		this.Process.lbSearchAddrShippingFromZipGlobal_Click(sender, e);
	}

	/// <summary>
	/// Recalculate fixed purchase shipping date when selected index changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlFixedPurchaseShippingDate_OnSelectedIndexChanged(object sender, System.EventArgs e)
	{
		this.ddlFixedPurchaseShippingDate_OnCheckedChanged(sender, e);
		RecalculateCartList((DropDownList)sender, e);
	}

	/// <summary>
	/// 配送パターン各アイテムのドロップダウン変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlFixedPurchaseShippingPatternItem_OnSelectedIndexChanged(object sender, System.EventArgs e)
	{
		// 再計算処理の影響で次回配送日が選択肢を変更しても変わらない場合があるため過去日をセットして選択済みの次回配送日をリセットする
		var riCart = GetParentRepeaterItem(((DropDownList)sender).Parent, "rCartList");
		this.CartList.Items[riCart.ItemIndex].Shippings[0].UpdateNextShippingDates(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(-1));

		this.ddlFixedPurchaseShippingPatternItem_OnCheckedChanged(sender, e);
		RecalculateCartList((DropDownList)sender, e);
	}

	/// <summary>
	/// 次回配送日変更用ドロップダウンへのデータバインド完了
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlNextShippingDate_OnDataBound(object sender, System.EventArgs e)
	{
		this.Process.UsePreviousShippingDate = true;
		this.Process.ddlNextShippingDate_OnDataBound(sender, e);
	}

	/// <summary>
	/// Recalculate next shipping date when selected index changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlNextShippingDate_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		RecalculateCartList((DropDownList)sender, e);
	}

	/// <summary>
	/// Recalculate cart list
	/// </summary>
	/// <param name="control"></param>
	/// <param name="e"></param>
	private void RecalculateCartList(Control control, System.EventArgs e)
	{
		var riCart = GetParentRepeaterItem(control, "rCartList");
		this.Process.lbRecalculate_Click(riCart.FindControl("lbRecalculateCart"), e);
	}
	#region CMSランディングページ処理 メソッド
	/// <summary>
	/// ソーシャルログインボタン 表示チェック
	/// </summary>
	/// <param name="socialLoginType">ソーシャルログイン種類</param>
	/// <returns>表示可否</returns>
	protected bool DisplaySocialLoginBtnCheck(LandingPageConst.SocialLoginType socialLoginType)
	{
		return this.Process.DisplaySocialLoginBtnCheck(socialLoginType);
	}

	/// <summary>
	/// ランディングページ商品セットモデルから商品を設定
	/// </summary>
	/// <param name="model">ランディングページ商品セットモデル</param>
	protected void ProductSetItemSetting(LandingPageProductSetModel model)
	{
		this.Process.ProductSetItemSetting(model);
	}

	/// <summary>
	/// 商品セット選択ドロップダウンリスト 変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlProductSet_OnSelectedIndexChanged(object sender, System.EventArgs e)
	{
		this.Process.ddlProductSet_OnSelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// 商品の削除が可能か
	/// </summary>
	/// <param name="cart">カート商品</param>
	/// <returns>結果</returns>
	protected bool CanDeleteProduct(CartProduct cart)
	{
		return ((cart.QuantityAllocatedToSet.Count != 0)
			|| this.IsCartListLp
			|| cart.IsNovelty);
	}

	/// <summary>
	/// 商品の削除が可能か（セットプロモーション用）
	/// </summary>
	/// <param name="cart">カート商品</param>
	/// <returns>結果</returns>
	protected bool CanDeleteSetPromotionProduct(CartProduct cart)
	{
		return ((cart.QuantitiyUnallocatedToSet != 0)
			|| (cart.QuantityAllocatedToSet.Count != 1)
			|| this.IsCartListLp);
	}

	/// <summary>
	/// Is Display NewebPay Credit Installment
	/// </summary>
	/// <param name="payment">Cart Payment</param>
	/// <returns>True: If There Is A Credit Installment Setting And The External Payment Type Is Credit</returns>
	protected bool IsDisplayNewebPayCreditInstallment(CartPayment payment)
	{
		return this.Process.IsDisplayCreditInstallment(payment);
	}

	/// <summary>
	/// Dropdown List NewebPay Selected Index Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlNewebPayment_SelectedIndexChanged(object sender, EventArgs e)
	{
		this.Process.ddlNewebPayment_OnSelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// ノベルティを表示するか
	/// </summary>
	/// <param name="cartId">カートID</param>
	/// <returns>結果</returns>
	protected bool IsDisplayNovelty(string cartId)
	{
		return (this.NoveltyUsable && (GetCartNovelty(cartId).Length != 0));
	}

	/// <summary>
	/// 再入力ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUserPasswordReType_Click(object sender, EventArgs e)
	{
		Session[Constants.SESSION_KEY_LP_PASSWORD] = string.Empty;
		Session[Constants.SESSION_KEY_LP_PASSWORDCONF] = string.Empty;

		WtbUserPassword.Visible = true;
		WtbUserPasswordConf.Visible = true;
		WdlUserPassword.Visible = false;
		WdlUserPasswordConf.Visible = false;
		WdbtnUserPasswordReType.Visible = false;

		WupPasswordUpdatePanel.Update();
	}

	/// <summary>
	/// Link button get authentication code click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbGetAuthenticationCode_Click(object sender, EventArgs e)
	{
		this.Process.lbGetAuthenticationCode_Click(sender, e);
	}

	/// <summary>
	/// Link button check authentication code click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCheckAuthenticationCode_Click(object sender, EventArgs e)
	{
		this.Process.lbCheckAuthenticationCode_Click(sender, e);
	}

	/// <summary>
	/// Get verification code note
	/// </summary>
	/// <param name="countryIsoCode">Country iso code</param>
	/// <returns>Verification_code note</returns>
	protected string GetVerificationCodeNote(string countryIsoCode = "")
	{
		return this.Process.GetVerificationCodeNote(countryIsoCode);
	}
	# endregion

	/// <summary>
	/// LINE連携
	/// </summary>
	/// <param name="nextUrl">NextURL</param>
	/// <param name="socialPlusCallbackPath">コールバックパス(ソーシャルプラス用)</param>
	/// <param name="socialPlusErrorCallbackPath">エラー時コールバックパス(ソーシャルプラス用)</param>
	/// <param name="socialPlusProfile">取得範囲(ソーシャルプラス用)</param>
	/// <param name="uriAuthority">URLAuthority</param>
	/// <param name="socialPlusReturnPath">戻り先パス</param>
	/// <returns>リクエストURL</returns>
	protected string LineConnect(
		string nextUrl,
		string socialPlusCallbackPath,
		string socialPlusErrorCallbackPath,
		bool socialPlusProfile,
		string uriAuthority,
		string socialPlusReturnPath)
	{
		var requestUrl = string.Empty;
		if (Constants.SOCIAL_LOGIN_ENABLED)
		{
			requestUrl = SocialLoginUtil.GetAuthenticateUrl(
				SocialLoginApiProviderType.Line,
				socialPlusCallbackPath,
				socialPlusErrorCallbackPath,
				socialPlusProfile,
				uriAuthority,
				socialPlusReturnPath);
		}
		else if (w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED && (Constants.SOCIAL_LOGIN_ENABLED == false))
		{
			Session[Constants.SESSION_KEY_NEXT_URL] = nextUrl;
			requestUrl = LineUtil.CreateConnectLineUrl(Session.SessionID);
		}
		return requestUrl;
	}


	/// <summary>
	/// 周回回数の取得
	/// </summary>
	/// <param name="courseId">コースID</param>
	/// <returns>周回文言</returns>
	public List<string> GetRepeaterCount(string courseId)
	{
		var subscriptionBoxService = new SubscriptionBoxService();
		this.SubscriptionBox = subscriptionBoxService.GetByCourseId(courseId);

		var subscriptionItemList = new List<string>();
		if (string.IsNullOrEmpty(courseId) == false)
		{
			this.SubscriptionBox.DefaultOrderProducts = subscriptionBoxService.GetDisplayItems(courseId);

			var repeaterList = (this.SubscriptionBox.IsNumberTime)
				? this.SubscriptionBox.DefaultOrderProducts
					.Select(item => StringUtility.ToEmpty(item.Count))
					.Distinct()
					.Take((Constants.DISP_LIST_CONTENTS_COUNT_SUBSCRIPTION_BOX_PRODUCT_LIST == 0)
						? this.SubscriptionBox.DefaultOrderProducts.Length
						: Constants.DISP_LIST_CONTENTS_COUNT_SUBSCRIPTION_BOX_PRODUCT_LIST)
					.ToList()
				: this.SubscriptionBox.DefaultOrderProducts
					.Where(item => (item.TermUntil > DateTime.Now))
					.OrderBy(item => item.TermSince)
					.Select(item => string.Format(
						"{0}～{1}",
						StringUtility.ToEmpty(item.TermSince),
						StringUtility.ToEmpty(item.TermUntil)))
					.Distinct()
					.Take((Constants.DISP_LIST_CONTENTS_COUNT_SUBSCRIPTION_BOX_PRODUCT_LIST == 0)
						? this.SubscriptionBox.DefaultOrderProducts.Length
						: Constants.DISP_LIST_CONTENTS_COUNT_SUBSCRIPTION_BOX_PRODUCT_LIST)
					.ToList();
			repeaterList.Remove(string.Empty);
			subscriptionItemList = repeaterList;
		}

		this.SubscriptionItemList = subscriptionItemList;

		return subscriptionItemList;
	}

	/// <summary>
	/// 頒布会一覧データバインド
	/// </summary>
	protected void BindSubscriptionItemData()
	{
		foreach (RepeaterItem repeaterItem in rCartList.Items)
		{
			var wrSubscriptionBoxItem = GetWrappedControl<WrappedRepeater>(repeaterItem, "rSubscriptionBoxItem");
			wrSubscriptionBoxItem.DataSource = this.SubscriptionItemList;
			wrSubscriptionBoxItem.DataBind();
		}
	}

	/// <summary>
	/// 商品リストを取得
	/// </summary>
	/// <param name="deliveryTiming">配送タイミング</param>
	/// <returns>商品リスト</returns>
	protected List<SubscriptionBoxDefaultItemModel> GetItemsModel(string deliveryTiming)
	{
		var productsList = new List<SubscriptionBoxDefaultItemModel>();

		var timing = deliveryTiming.Split('～');

		if ((this.SubscriptionBox.IsNumberTime == false) && (timing.Length > 1))
		{
			productsList = this.SubscriptionBox.DefaultOrderProducts
				.Where(product => ((DateTime)product.TermSince).Date == (DateTime.Parse(timing[0]).Date)
					&& ((DateTime)product.TermUntil).Date == (DateTime.Parse(timing[1]).Date))
				.ToList();

			if (string.IsNullOrEmpty(productsList[0].VariationId) && string.IsNullOrEmpty(productsList[0].ProductId))
			{
				for (int i = (productsList[0].BranchNo - 1); i > 0; i--)
				{
					var preList = this.SubscriptionBox.DefaultOrderProducts.First(item => (item.BranchNo == i));

					if ((string.IsNullOrEmpty(preList.ProductId) == false) && (string.IsNullOrEmpty(preList.VariationId) == false))
					{
						productsList = this.SubscriptionBox.DefaultOrderProducts
							.Where(item => (item.TermSince == preList.TermSince)
								&& (item.TermUntil == preList.TermUntil))
							.ToList();

						productsList = (productsList.Any(p => p.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_VALID))
							? productsList.Where(p => p.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_VALID).ToList()
							: productsList;

						return productsList;
					}
				}
			}
			productsList = (productsList.Any(p => p.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_VALID))
				? productsList.Where(p => p.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_VALID).ToList()
				: productsList;
			return productsList;
		}

		productsList = this.SubscriptionBox.DefaultOrderProducts
			.Where(product => (product.Count == int.Parse(timing[0])))
			.ToList();

		if (string.IsNullOrEmpty(productsList[0].VariationId) && string.IsNullOrEmpty(productsList[0].ProductId))
		{
			for (int i = ((int)productsList[0].Count - 1); i > 0; i--)
			{
				productsList = this.SubscriptionBox.DefaultOrderProducts
					.Where(item => (item.Count == i))
					.ToList();

				if ((string.IsNullOrEmpty(productsList[0].ProductId) == false) && (string.IsNullOrEmpty(productsList[0].VariationId) == false))
				{
					productsList = (productsList.Any(p => p.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_VALID))
						? productsList.Where(p => p.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_VALID).ToList()
						: productsList;

					return productsList;
				}
			}
		}

		productsList = (productsList.Any(p => p.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_VALID))
			? productsList.Where(p => p.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_VALID).ToList()
			: productsList;

		return productsList;
	}

	/// <summary>
	/// 引継ぎ商品か判定
	/// </summary>
	/// <param name="productsList">商品リスト</param>
	/// <returns>引継ぎ商品か?</returns>
	protected bool CheckTakeOverProduct(List<SubscriptionBoxDefaultItemModel> productsList)
	{
		var result = (string.IsNullOrEmpty(productsList[0].ProductId) && string.IsNullOrEmpty(productsList[0].VariationId));

		return result;
	}

	/// <summary>
	/// 商品名の作成
	/// </summary>
	/// <param name="name">商品名</param>
	/// <param name="variationName1">バリエーション名1</param>
	/// <param name="variationName2">バリエーション名2</param>
	/// /// <param name="variationName3">バリエーション3</param>
	/// <returns>商品名</returns>
	public string CreateProductName(
		string name,
		string variationName1,
		string variationName2,
		string variationName3)
	{
		var productName = name + (string.IsNullOrEmpty(variationName1)
			? ""
			: ProductCommon.CreateVariationName(
				variationName1,
				variationName2,
				variationName3));

		return productName;
	}

	/// <summary>
	/// 商品エリアリピータイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rItem_OnItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		// ドロップダウンに表示するバリエーションIDを指定
		var wddlControl = GetWrappedControl<WrappedDropDownList>(e.Item, "ddlProductVariationList");
		if (this.TopVariationId == wddlControl.SelectedValue)
		{
			wddlControl.SelectedIndex = this.SelectedVariationIndex;
		}
	}

	/// <summary>
	/// 商品データ取得（表示条件考慮しない）
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID(なしの場合、商品ID)</param>
	/// <returns>商品データ</returns>
	public DataView GetProduct(string shopId, string productId, string variationId)
	{
		var result = this.Process.GetProduct(shopId, productId, variationId);
		return result;
	}

	/// <summary>
	/// 商品名取得
	/// </summary>
	/// <param name="shopId">ショップID</param>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	/// <returns>商品名</returns>
	protected string GetProductName(string shopId, string productId, string variationId)
	{
		if (string.IsNullOrEmpty(productId)) return string.Empty;
		var result = new ProductService().GetProductVariation(shopId, productId, variationId, this.MemberRankId).Name;
		return result;
	}

	/// <summary>
	/// 頒布会商品リストの作成
	/// </summary>
	/// <returns>商品リスト</returns>
	protected ListItemCollection GetSubscriptionBoxProductList(string productId, string varitionId, string courseId)
	{
		var subscriptionBox = new SubscriptionBoxService().GetByCourseId(courseId);

		var subscriptionBoxFirstDefaultItem = (subscriptionBox.IsNumberTime)
			? subscriptionBox.DefaultOrderProducts
				.Where(item => item.Count == 1).ToArray()
			: subscriptionBox.DefaultOrderProducts
				.Where(item => (item.TermSince <= DateTime.Now)
					&& (item.TermUntil >= DateTime.Now))
				.ToArray();
		var optionalProducts = subscriptionBoxFirstDefaultItem
			.Where(item => item.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_INVALID);

		var list = new ListItemCollection
		{
			new ListItem("", this.ShopId + "//")
		};
		// 任意商品がなかった場合何も表示しない
		if (optionalProducts.Any() == false)
		{
			this.SubscriputionBoxProductList = new List<SubscriptionBoxDefaultItemModel>();
			this.SubscriputionBoxProductListModify = new List<SubscriptionBoxDefaultItemModel>();
			return list;
		}

		foreach (var selectableProduct in subscriptionBox.SelectableProducts)
		{
			selectableProduct.ProductName = GetProductName(
				this.ShopId,
				selectableProduct.ProductId,
				selectableProduct.VariationId);
		}
		var optionalListItems = optionalProducts
			.SelectMany(item => subscriptionBox.SelectableProducts, (defaultOrderProduct, selectableProduct) => new { defaultOrderProduct, selectableProduct })
			.Where(product => (product.defaultOrderProduct.VariationId == product.selectableProduct.VariationId))
			.Select(result => new ListItem(
				result.selectableProduct.ProductName,
				string.Format(
					"{0}/{1}/{2}",
					this.ShopId,
					result.defaultOrderProduct.ProductId,
					result.defaultOrderProduct.VariationId))).ToArray();
		list.AddRange(optionalListItems);
		return list;
	}

	/// <summary>
	/// 必須商品判定
	/// </summary>
	/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
	/// <param name="productId">商品ID</param>
	/// <returns>結果</returns>
	protected bool HasNecessaryProduct(string subscriptionBoxCourseId, string productId)
	{
		var result = this.Process.HasNecessaryProduct(subscriptionBoxCourseId, productId);
		return result;
	}

	/// <summary>
	/// 頒布会表示名取得
	/// </summary>
	/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
	/// <returns>頒布会表示名</returns>
	protected string GetSubscriptionBoxDisplayName(string subscriptionBoxCourseId)
	{
		var result = this.Process.GetSubscriptionBoxDisplayName(subscriptionBoxCourseId);
		return result;
	}

	/// <summary>
	/// 商品を追加
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAddProduct_Click(object sender, EventArgs e)
	{
		foreach (RepeaterItem repeaterItem in rCartList.Controls)
		{
			((HtmlGenericControl)repeaterItem.FindControl("sErrorQuantity")).InnerText = "";
		}

		this.SubscriputionBoxProductList.Add(
			new SubscriptionBoxDefaultItemModel
			{
				SubscriptionBoxCourseId = ((Button)sender).CommandArgument,
				Count = null,
				TermSince = null,
				TermUntil = null,
				ShopId = this.ShopId,
				ProductId = string.Empty,
				ItemQuantity = 1,
				VariationId = string.Empty,
				BranchNo = 0,
				NecessaryProductFlg = Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_INVALID
			});

		BindItemModifyData();
	}

	/// <summary>
	/// 任意商品の更新
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateProduct_Click(object sender, EventArgs e)
	{
		var isQuantityError = this.SubscriputionBoxProductList.Any(product => product.ItemQuantity == 0);
		if (isQuantityError)
		{
			foreach (RepeaterItem repeaterItem in rCartList.Controls)
			{
				var whgcErrorQuantity = GetWrappedControl<WrappedHtmlGenericControl>(repeaterItem, "sErrorQuantity");
				whgcErrorQuantity.InnerText =
					WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_QUANTITY_UPDATE_ALERT);
			}
			return;
		}

		var totalQuantity = this.CartList.Items
			.Sum(cart => cart.Items.Sum(cartProduct => this.SubscriputionBoxProductList
				.Where(ninni => cartProduct.VariationId != ninni.VariationId)
				.Sum(ninni => cartProduct.CountSingle)));
		var wsErrorMessage = OrderCommon.CheckLimitProductOrderForSubscriptionBox(((Button)sender).CommandArgument, totalQuantity);
		if (string.IsNullOrEmpty(wsErrorMessage) == false)
		{
			foreach (RepeaterItem repeaterItem in rCartList.Controls)
			{
				var whgcErrorQuantity = GetWrappedControl<WrappedHtmlGenericControl>(repeaterItem, "sErrorQuantity");
				whgcErrorQuantity.InnerText = wsErrorMessage;
			}
			return;
		}

		var variationIds = new List<string>();
		foreach (var product in this.SubscriputionBoxProductList)
		{
			if (string.IsNullOrEmpty(product.VariationId)) continue;
			if (variationIds.Contains(product.VariationId))
			{
				foreach (RepeaterItem repeaterItem in rCartList.Controls)
				{
					var whgcErrorQuantity = GetWrappedControl<WrappedHtmlGenericControl>(repeaterItem, "sErrorQuantity");
					whgcErrorQuantity.InnerText = wsErrorMessage;
					return;
				}
			}
			variationIds.Add(product.VariationId);
		}


		foreach (RepeaterItem repeaterItem in rCartList.Controls)
		{
			var whgcListProduct = GetWrappedControl<WrappedHtmlGenericControl>(repeaterItem, "dvListProduct");
			var whgcModifySubscription = GetWrappedControl<WrappedHtmlGenericControl>(repeaterItem, "dvModifySubscription");
			whgcListProduct.Visible = true;
			whgcModifySubscription.Visible = false;
		}


		this.SubscriputionBoxProductListModify = this.SubscriputionBoxProductList.Where(p => string.IsNullOrEmpty(p.ProductId) == false).ToList();
		this.SubscriputionBoxProductList = this.SubscriputionBoxProductListModify;
		Response.Redirect(Request.Url.AbsolutePath);
	}

	/// <summary>
	/// 商品削除
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rProductChange_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		if (e.CommandName == "DeleteRow") this.SubscriputionBoxProductList.RemoveAt(e.Item.ItemIndex);
		BindItemModifyData();
	}

	/// <summary>
	/// 再計算
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ReCalculation(object sender, EventArgs e)
	{
		GetWrappedControl<WrappedHtmlGenericControl>("sErrorQuantity").InnerText = "";
		var repeater = new RepeaterItemCollection(null);
		foreach (RepeaterItem repeaterItem in rCartList.Controls)
		{
			var wrItemModify = GetWrappedControl<WrappedRepeater>(repeaterItem, "rItemModify");
			repeater = wrItemModify.Items;
			if (repeater != new RepeaterItemCollection(null)) break;
		}
		var quantity = 0;
		var productList = new List<string>();
		var quantityList = new List<int>();

		foreach (RepeaterItem roii in repeater)
		{
			var wtbQuantityUpdate = GetWrappedControl<WrappedTextBox>(roii, "tbQuantityUpdate");
			var wddlProductName = GetWrappedControl<WrappedDropDownList>(roii, "ddlProductName");
			var quantityString = wtbQuantityUpdate.Text;
			productList.Add(wddlProductName.SelectedValue);

			if (string.IsNullOrEmpty(quantityString) || (int.TryParse(quantityString, out quantity) == false) || (quantity < 1))
			{
				quantity = 1;
			}
			quantityList.Add(quantity);
		}

		var count = 0;
		foreach (var list in this.SubscriputionBoxProductList)
		{
			if ((productList[count] == null) || (quantityList[count] == null)) break;
			list.ItemQuantity = quantityList[count];
			list.ShopId = this.ShopId;
			list.ProductId = productList[count].Split('/')[1];
			list.VariationId = productList[count].Split('/')[2];
			count++;
		}
		BindItemModifyData();
	}

	/// <summary>
	/// 頒布会金額取得
	/// </summary>
	/// <param name="prodctId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	/// <param name="count">個数</param>
	/// <returns>金額</returns>
	protected decimal SubscriptionBoxPrice(string prodctId, string variationId, int count)
	{
		if (string.IsNullOrEmpty(prodctId) || string.IsNullOrEmpty(variationId)) return 0;
		var price = new ProductService().GetProductVariation(this.ShopId, prodctId, variationId, this.MemberRankId).Price * count;
		return price;
	}

	/// <summary>
	/// 商品変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnChangeProduct_Click(object sender, EventArgs e)
	{
		this.HasOptionalProdects = (this.HasOptionalProdects == false);
		foreach (RepeaterItem repeaterItem in rCartList.Controls)
		{
			var whgcListProduct = GetWrappedControl<WrappedHtmlGenericControl>(repeaterItem, "dvListProduct");
			var whgcModifySubscription = GetWrappedControl<WrappedHtmlGenericControl>(repeaterItem, "dvModifySubscription");
			whgcListProduct.Visible = false;
			whgcModifySubscription.Visible = true;
		}
		BindItemModifyData();
	}

	/// <summary>
	/// 任意商品データバインド
	/// </summary>
	protected void BindItemModifyData()
	{
		foreach (RepeaterItem repeaterItem in rCartList.Items)
		{
			var wrItemModify = GetWrappedControl<WrappedRepeater>(repeaterItem, "rItemModify");
			wrItemModify.DataSource = this.SubscriputionBoxProductList;
			wrItemModify.DataBind();
		}
	}

	/// <summary>
	/// 必須商品取得
	/// </summary>
	/// <param name="courseId">頒布会コースID</param>
	/// <returns>結果</returns>
	protected bool CanNecessaryProducts(string courseId)
	{
		if (string.IsNullOrEmpty(courseId)) return false;
		var subscriptionBox = new SubscriptionBoxService().GetByCourseId(courseId);

		var subscriptionBoxFirstDefaultItem = (subscriptionBox.IsNumberTime)
			? subscriptionBox.DefaultOrderProducts
				.Where(item => item.Count == 1).ToArray()
			: subscriptionBox.DefaultOrderProducts
				.Where(item => (item.TermSince <= DateTime.Now)
					&& (item.TermUntil >= DateTime.Now))
				.ToArray();

		if (subscriptionBoxFirstDefaultItem
			.Any(item => item.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_INVALID) == false) return false;

		var result = subscriptionBoxFirstDefaultItem.Any(item => item.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_VALID);
		return result;
	}

	/// <summary>
	/// 必須商品取得
	/// </summary>
	/// <param name="courseId">頒布会コースID</param>
	/// <param name="variationId">バリエーションID</param>
	/// <param name="productChecked">商品購入チェック判定</param>
	/// <returns>結果</returns>
	protected bool CanNecessaryProducts(string courseId, string variationId, bool productChecked)
	{
		if (string.IsNullOrEmpty(courseId)) return productChecked;

		var subscriptionBox = new SubscriptionBoxService().GetByCourseId(courseId);

		var subscriptionBoxFirstDefaultItem = (subscriptionBox.IsNumberTime)
			? subscriptionBox.DefaultOrderProducts
				.Where(item => item.Count == 1).ToArray()
			: subscriptionBox.DefaultOrderProducts
				.Where(item => (item.TermSince <= DateTime.Now)
					&& (item.TermUntil >= DateTime.Now))
				.ToArray();

		if (subscriptionBoxFirstDefaultItem
			.Any(item => item.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_INVALID) == false) return true;

		var result = subscriptionBoxFirstDefaultItem
			.Any(item => item.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_VALID
			&& variationId == item.VariationId);
		return result;
	}

	/// <summary>
	/// 商品一覧エリアのリピータイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rProductsList_OnItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		var dSubscriptionBoxPriceControl = (HtmlGenericControl)e.Item.FindControl("dSubscriptionBoxPrice");
		var dSubscriptionBoxCampaignPriceControl = (HtmlGenericControl)e.Item.FindControl("dSubscriptionBoxCampaignPrice");
		var lSubscriptionBoxCampaignPriceControl = (Literal)e.Item.FindControl("lSubscriptionBoxCampaignPrice");

		var subscriptionBoxDefaultItem = (SubscriptionBoxDefaultItemModel)e.Item.DataItem;

		var subscriptionBoxItem = this.SubscriptionBox.SelectableProducts.FirstOrDefault(
			x => (x.ProductId == subscriptionBoxDefaultItem.ProductId)
				&& (x.VariationId == subscriptionBoxDefaultItem.VariationId));

		var isCampaignPeriod = OrderCommon.IsSubscriptionBoxCampaignPeriod(subscriptionBoxItem);

		lSubscriptionBoxCampaignPriceControl.Text =
			CurrencyManager.ToPrice(isCampaignPeriod
				? subscriptionBoxItem.CampaignPrice.ToPriceDecimal()
				: subscriptionBoxDefaultItem.Price);
		dSubscriptionBoxPriceControl.Visible = isCampaignPeriod;
	}

	/// <summary>
	/// Get use point
	/// </summary>
	/// <param name="cart">Cart</param>
	/// <returns>Use point</returns>
	protected decimal GetUsePoint(CartObject cart)
	{
		if (cart.CanUsePointForPurchase == false)
		{
			cart.SetUsePoint(0, 0);
		}
		return cart.UsePoint;
	}

	/// <summary>
	/// Get price can purchase use point
	/// </summary>
	/// <param name="purchasePriceTotal">Purchase price total</param>
	/// <returns>Price can purchase use point</returns>
	protected string GetPriceCanPurchaseUsePoint(decimal purchasePriceTotal)
	{
		var result = CurrencyManager.ToPrice(Constants.POINT_MINIMUM_PURCHASEPRICE - purchasePriceTotal);
		return result;
	}

	/// <summary>
	/// Create set omotion client id js script
	/// </summary>
	/// <returns>js script</returns>
	protected string CreateSetOmotionClientIdJsScript()
	{
		var scripts = "SetOmotionClientId("
			+ "'" + (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED ? this.Process.WtbLoginIdInMailAddr.ClientID : this.Process.WtbLoginId.ClientID) + "', "
			+ "'" + this.Process.WtbPassword.ClientID + "', "
			+ "'" + this.Process.WdLoginErrorMessage.ClientID + "', "
			+ "'" + this.Process.WlbLogin.ClientID + "'"
			+ ");";
		return scripts;
	}

	/// <summary>
	/// Dropdownlist real shop area on databound
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlRealShopArea_DataBound(object sender, EventArgs e)
	{
		this.Process.ddlRealShopArea_DataBound(sender, e);
	}

	/// <summary>
	/// リアル店舗絞り込みドロップダウンリスト変更時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlRealShopNarrowDown_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		this.Process.ddlRealShopNarrowDown_OnSelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// Dropdownlist real shop name list selected index changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlRealShopNameList_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		this.Process.ddlRealShopNameList_OnSelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// コンビニ受取店舗情報で配送先情報を更新
	/// </summary>
	/// <remarks>台湾ファミマコンビニ受取利用時</remarks>
	public void UpdateCartShippingByReceivingStore()
	{
		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			var whfCvsShopId = GetWrappedControl<WrappedHiddenField>(riCart, "hfCvsShopId");
			var whfCvsShopName = GetWrappedControl<WrappedHiddenField>(riCart, "hfCvsShopName");
			var whfCvsShopAddress = GetWrappedControl<WrappedHiddenField>(riCart, "hfCvsShopAddress");
			var whfCvsShopTel = GetWrappedControl<WrappedHiddenField>(riCart, "hfCvsShopTel");
			if (whfCvsShopId != null)
			{
				var cart = this.CartList.Items[riCart.ItemIndex];
				var cartShipping = cart.Shippings[0];
				cartShipping.UpdateConvenienceStoreAddr(
					CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE,
					whfCvsShopId.Value,
					whfCvsShopName.Value,
					whfCvsShopAddress.Value,
					whfCvsShopTel.Value,
					cartShipping.ShippingReceivingStoreType);
			}
		}
	}

	#region CMSランディングページ処理 プロパティ
	/// <summary>ページID</summary>
	public string PageId
	{
		get { return (this.LandingPageDesignModel != null) ? LandingPageDesignModel.PageId : null; }
	}
	/// <summary>CMSランディングページモデルD（外から渡される）</summary>
	public LandingPageDesignModel LandingPageDesignModel
	{
		get { return this.Process.LandingPageDesignModel; }
		set { this.Process.LandingPageDesignModel = value; }
	}
	/// <summary>有効なCMSランディングページ商品セット選択肢モデル</summary>
	protected LandingPageProductSetModel[] ValidLandingPageProductSetModels
	{
		get { return this.Process.ValidLandingPageProductSetModels; }
	}
	/// <summary>メールアドレス確認フォームの表示可否</summary>
	protected bool DisplayMailAddressConfirmForm
	{
		get { return this.Process.DisplayMailAddressConfirmForm; }
	}
	/// <summary>次へボタンの表示・非表示</summary>
	protected bool DisplayNextBtn
	{
		get { return this.Process.DisplayNextBtn; }
	}
	/// <summary>会員ランクエラーの表示・非表示</summary>
	protected string DispMemberRankError
	{
		get { return this.Process.DispCartErrorMessageForMemberRank; }
	}
	/// <summary>確認画面のスキップ</summary>
	protected bool SkipOrderConfirm
	{
		get { return this.Process.SkipOrderConfirm; }
	}
	/// <summary>CMSランディングページかどうか</summary>
	protected bool IsCmsLandingPage { get { return this.Process.IsCmsLandingPage; } }
	# endregion

	#region プロパティ
	/// <summary>カートリスト</summary>
	public new CartObjectList CartList
	{
		get { return this.Process.CartList; }
		set { this.Process.CartList = value; }
	}
	/// <summary>プロセス</summary>
	protected new OrderFormLpInputProcess Process
	{
		get { return (OrderFormLpInputProcess)this.ProcessTemp; }
	}
	/// <summary>プロセステンポラリ</summary>
	protected override IPageProcess ProcessTemp
	{
		get
		{
			if (m_processTmp == null) m_processTmp = new OrderFormLpInputProcess(this, this.ViewState, this.Context);
			return m_processTmp;
		}
	}
	/// <summary>次へ進むイベント格納用</summary>
	protected string NextEvent
	{
		get { return this.Process.NextEvent; }
	}
	/// <summary>次へ進むonclick</summary>
	protected string NextOnClick
	{
		get { return this.Process.NextOnClick; }
	}
	/// <summary>商品番号1</summary>
	protected string ProductId1 { get { return this.Process.ProductId1; } set { this.Process.ProductId1 = value; } }
	/// <summary>商品番号2</summary>
	protected string ProductId2 { get { return this.Process.ProductId2; } set { this.Process.ProductId2 = value; } }
	/// <summary>商品番号3</summary>
	protected string ProductId3 { get { return this.Process.ProductId3; } set { this.Process.ProductId3 = value; } }
	/// <summary>商品番号4</summary>
	protected string ProductId4 { get { return this.Process.ProductId4; } set { this.Process.ProductId4 = value; } }
	/// <summary>商品番号5</summary>
	protected string ProductId5 { get { return this.Process.ProductId5; } set { this.Process.ProductId5 = value; } }
	/// <summary>商品番号6</summary>
	protected string ProductId6 { get { return this.Process.ProductId6; } set { this.Process.ProductId6 = value; } }
	/// <summary>商品番号7</summary>
	protected string ProductId7 { get { return this.Process.ProductId7; } set { this.Process.ProductId7 = value; } }
	/// <summary>商品番号8</summary>
	protected string ProductId8 { get { return this.Process.ProductId8; } set { this.Process.ProductId8 = value; } }
	/// <summary>商品番号9</summary>
	protected string ProductId9 { get { return this.Process.ProductId9; } set { this.Process.ProductId9 = value; } }
	/// <summary>商品番号10</summary>
	protected string ProductId10 { get { return this.Process.ProductId10; } set { this.Process.ProductId10 = value; } }
	/// <summary>商品番号1投入数</summary>
	protected int ProductId1Quantity { get { return this.Process.ProductId1Quantity; } set { this.Process.ProductId1Quantity = value; } }
	/// <summary>商品番号2投入数</summary>
	protected int ProductId2Quantity { get { return this.Process.ProductId2Quantity; } set { this.Process.ProductId2Quantity = value; } }
	/// <summary>商品番号3投入数</summary>
	protected int ProductId3Quantity { get { return this.Process.ProductId3Quantity; } set { this.Process.ProductId3Quantity = value; } }
	/// <summary>商品番号4投入数</summary>
	protected int ProductId4Quantity { get { return this.Process.ProductId4Quantity; } set { this.Process.ProductId4Quantity = value; } }
	/// <summary>商品番号5投入数</summary>
	protected int ProductId5Quantity { get { return this.Process.ProductId5Quantity; } set { this.Process.ProductId5Quantity = value; } }
	/// <summary>商品番号6投入数</summary>
	protected int ProductId6Quantity { get { return this.Process.ProductId6Quantity; } set { this.Process.ProductId6Quantity = value; } }
	/// <summary>商品番号7投入数</summary>
	protected int ProductId7Quantity { get { return this.Process.ProductId7Quantity; } set { this.Process.ProductId7Quantity = value; } }
	/// <summary>商品番号8投入数</summary>
	protected int ProductId8Quantity { get { return this.Process.ProductId8Quantity; } set { this.Process.ProductId8Quantity = value; } }
	/// <summary>商品番号9投入数</summary>
	protected int ProductId9Quantity { get { return this.Process.ProductId9Quantity; } set { this.Process.ProductId9Quantity = value; } }
	/// <summary>商品番号10投入数</summary>
	protected int ProductId10Quantity { get { return this.Process.ProductId10Quantity; } set { this.Process.ProductId10Quantity = value; } }
	/// <summary>初期チェック1</summary>
	protected bool DefaultChecked1 { get { return this.Process.DefaultChecked1; } set { this.Process.DefaultChecked1 = value; } }
	/// <summary>初期チェック2</summary>
	protected bool DefaultChecked2 { get { return this.Process.DefaultChecked2; } set { this.Process.DefaultChecked2 = value; } }
	/// <summary>初期チェック3</summary>
	protected bool DefaultChecked3 { get { return this.Process.DefaultChecked3; } set { this.Process.DefaultChecked3 = value; } }
	/// <summary>初期チェック4</summary>
	protected bool DefaultChecked4 { get { return this.Process.DefaultChecked4; } set { this.Process.DefaultChecked4 = value; } }
	/// <summary>初期チェック5</summary>
	protected bool DefaultChecked5 { get { return this.Process.DefaultChecked5; } set { this.Process.DefaultChecked5 = value; } }
	/// <summary>初期チェック6</summary>
	protected bool DefaultChecked6 { get { return this.Process.DefaultChecked6; } set { this.Process.DefaultChecked6 = value; } }
	/// <summary>初期チェック7</summary>
	protected bool DefaultChecked7 { get { return this.Process.DefaultChecked7; } set { this.Process.DefaultChecked7 = value; } }
	/// <summary>初期チェック8</summary>
	protected bool DefaultChecked8 { get { return this.Process.DefaultChecked8; } set { this.Process.DefaultChecked8 = value; } }
	/// <summary>初期チェック9</summary>
	protected bool DefaultChecked9 { get { return this.Process.DefaultChecked9; } set { this.Process.DefaultChecked9 = value; } }
	/// <summary>初期チェック10</summary>
	protected bool DefaultChecked10 { get { return this.Process.DefaultChecked10; } set { this.Process.DefaultChecked10 = value; } }
	/// <summary>カート投入区分</summary>
	protected Constants.AddCartKbn AddCartKbn
	{
		get { return this.Process.AddCartKbn; }
		set { this.Process.AddCartKbn = value; }
	}
	/// <summary>Limited Payment Messages</summary>
	protected Hashtable DispLimitedPaymentMessages
	{
		get { return this.Process.DispLimitedPaymentMessages; }
	}
	/// <summary>商品リスト</summary>
	protected LandingCartProduct[] ProductList
	{
		get { return this.Process.ProductList; }
		set { this.Process.ProductList = value; }
	}
	/// <summary>LPカートの商品選択方式</summary>
	protected OrderLandingInputProcess.ChooseProductType ChooseProduct
	{
		get { return this.Process.ChooseProduct; }
		set { this.Process.ChooseProduct = value; }
	}
	/// <summary>DropDownListか？</summary>
	protected bool IsDropDownList
	{
		get { return (this.Process.IsDropDownList); }
	}
	/// <summary>CheckBoxか？</summary>
	protected bool IsCheckBox
	{
		get { return this.Process.IsCheckBox; }
	}
	/// <summary>リストから選択か？</summary>
	protected bool IsSelectFromList
	{
		get { return (this.Process.IsSelectFromList); }
	}
	/// <summary>商品を選択しない？</summary>
	protected bool IsNotChoose
	{
		get { return this.Process.IsNotChoose; }
	}
	/// <summary>アマゾンアカウントで会員登録しているか</summary>
	protected bool IsUserRegistedForAmazon
	{
		get { return this.Process.IsUserRegistedForAmazon; }
	}
	/// <summary>ログインしているAmazonPayユーザーと同じメールアドレスを持つユーザーが存在するか</summary>
	protected bool ExistsUserWithSameAmazonEmailAddress
	{
		get { return this.Process.ExistsUserWithSameAmazonEmailAddress; }
	}
	/// <summary>デフォルト決済方法</summary>
	protected string DefaultPaymentId
	{
		get { return this.Process.DefaultPaymentId; }
		set { this.Process.DefaultPaymentId = value; }
	}
	/// <summary>カートリストランディングページかどうか</summary>
	protected new bool IsCartListLp
	{
		get { return this.Process.IsCartListLp; }
	}
	/// <summary>アマゾンリクエスト</summary>
	protected AmazonCv2Redirect AmazonRequest
	{
		get { return this.Process.AmazonRequest; }
	}
	/// <summary>アマゾンチェックアウトセッション</summary>
	public CheckoutSessionResponse AmazonCheckoutSession
	{
		get { return this.Process.AmazonCheckoutSession; }
	}
	/// <summary>AmazonPayセッション</summary>
	public AmazonModel AmazonPaySessionAmazonModel
	{
		get { return this.Process.AmazonPaySessionAmazonModel; }
	}
	/// <summary>AmazonPay注文者住所セッション</summary>
	public AmazonAddressModel AmazonPaySessionOwnerAddress
	{
		get { return this.Process.AmazonPaySessionOwnerAddress; }
	}
	/// <summary>AmazonPay配送先住所セッション</summary>
	public AmazonAddressModel AmazonPaySessionShippingAddress
	{
		get { return this.Process.AmazonPaySessionShippingAddress; }
	}
	/// <summary>AmazonPay決済デスクリプター</summary>
	public string AmazonPaySessionPaymentDescriptor
	{
		get { return this.Process.AmazonPaySessionPaymentDescriptor; }
	}
	/// <summary>AmazonCv2チェックアウトセッションID</summary>
	protected string AmazonCheckoutSessionId
	{
		get { return this.Process.AmazonCheckoutSessionId; }
	}
	/// <summary>LINE連携しているか</summary>
	protected bool IsConnectedLine
	{
		get { return string.IsNullOrEmpty(SessionManager.LineProviderUserId) ? false : true; }
	}
	/// <summary>特商法に基づく記載を表示するか</summary>
	protected bool IsDispCorrespondenceSpecifiedCommericalTransactions
	{
		get
		{
			return Constants.CORRESPONDENCE_SPECIFIEDCOMMERCIALTRANSACTIONS_ENABLE
				&& (string.IsNullOrEmpty(ShopMessage.GetMessage("SpecifiedCommercialTransactions")) == false);
		}
	}
	/// <summary>確認画面スキップ設定の際に特商法に基づく記載を表示するか</summary>
	protected bool IsDispCorrespondenceSpecifiedCommericalTransactionsSkipOrderConfirm
	{
		get
		{
			return Constants.CORRESPONDENCE_SPECIFIEDCOMMERCIALTRANSACTIONS_ENABLE
				&& (string.IsNullOrEmpty(ShopMessage.GetMessage("SpecifiedCommercialTransactions")) == false)
				&& this.SkipOrderConfirm;
		}
	}
	/// <summary>Has authentication code</summary>
	protected bool HasAuthenticationCode
	{
		get { return this.Process.HasAuthenticationCode; }
	}
	/// <summary>Authentication usable</summary>
	protected bool AuthenticationUsable
	{
		get { return this.Process.AuthenticationUsable; }
	}
	/// <summary>頒布会</summary>
	protected SubscriptionBoxModel SubscriptionBox { get; private set; }
	/// <summary>頒布会か</summary>
	public bool IsSubscriptionBox
	{
		get {return this.SubscriptionBox != null
				&& (this.SubscriptionBox.FixedAmountFlg == Constants.FLG_SUBSCRIPTIONBOX_FIXED_AMOUNT_FALSE);
		}
	}
	/// <summary>頒布会定額コースか</summary>
	public bool IsSubscriptionBoxFixedAmount
	{
		get {
			return this.SubscriptionBox != null
				&& (this.SubscriptionBox.FixedAmountFlg == Constants.FLG_SUBSCRIPTIONBOX_FIXED_AMOUNT_TRUE);
		}
	}
	/// <summary> 頒布会回数リスト </summary>
	public List<string> SubscriptionItemList
	{
		get { return (List<string>)ViewState["SubscriptionItemList"]; }
		set { ViewState["SubscriptionItemList"] = value; }
	}
	/// <summary>表示</summary>
	protected bool HasOptionalProdects
	{
		get { return (bool?)ViewState["HasOptionalProdects"] ?? true; }
		set { ViewState["HasOptionalProdects"] = value; }
	}
	/// <summary>ドロップダウン先頭のバリエーションID</summary>
	protected string TopVariationId
	{
		get { return (string)ViewState["TopVariationId"]; }
		set { ViewState["TopVariationId"] = value; }
	}
	/// <summary>選択したバリエーションインデックス</summary>
	protected int SelectedVariationIndex { get; set; }
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
	/// <summary>Is show payment fee</summary>
	public bool IsShowPaymentFee
	{
		get { return this.Process.IsShowPaymentFee; }
	}
	/// <summary>Custom validator control disabled information list</summary>
	protected string[] CustomValidatorControlDisabledInformationList
	{
		get { return this.Process.CustomValidatorControlDisabledInformationList; }
	}
	/// <summary>Custom validator control information list</summary>
	protected string[] CustomValidatorControlInformationList
	{
		get { return this.Process.CustomValidatorControlInformationList; }
	}
	#endregion
	#region CartList向け
	/// <summary>
	/// Amazonペイメントが使えるかどうか
	/// </summary>
	/// <returns>
	/// True：利用可
	/// False：利用不可
	/// </returns>
	protected bool CanUseAmazonPayment()
	{
		return this.Process.CanUseAmazonPayment();
	}

	/// <summary>
	/// 表示サイトがＰＣ・スマフォかの判定
	/// </summary>
	/// <returns>
	/// True：利用可
	/// False：利用不可
	/// </returns>
	protected bool CanUseAmazonPaymentForFront()
	{
		return this.Process.CanUseAmazonPaymentForFront();
	}

	/// <summary>
	/// Can use paypal payment
	/// </summary>
	/// <returns>True: Can use paypal payment, False: Can't use paypal payment</returns>
	protected bool CanUsePayPalPayment()
	{
		return this.Process.CanUsePayPalPayment();
	}

	/// <summary>
	/// Check amazon payment landing page design limit
	/// </summary>
	/// <returns>True: Can use amazon payment, False: Can't use amazon payment</returns>
	protected bool CheckAmazonPaymentLandingPageDesignLimit()
	{
		return this.Process.CheckAmazonPaymentLandingPageDesignLimit();
	}
	#endregion

	/// <summary>
	/// オプション価格を持っているか
	/// </summary>
	/// <param name="cartObject">カートオブジェクト</param>
	/// <returns>オプション価格を持っているか</returns>
	protected bool HasProductOptionPrice(CartObject cartObject)
	{
		var hasProductOptionPrice = ProductOptionSettingHelper.HasProductOptionPriceInCart(cartObject);
		return hasProductOptionPrice;
	}

	/// <summary>
	/// 商品付帯情報設定値一覧の最初にデフォルト値を追加（ドロップダウン形式）
	/// </summary>
	/// <param name="lic">商品付帯情報設定値一覧</param>
	/// <param name="isNecessary">必須かどうか</param>
	/// <returns>ドロップダウン形式の商品付帯情報設定値一覧n</returns>
	/// <remarks>
	/// ※付帯価格オプション有効時のみ※<br/>
	/// ・デフォルト値："選択してください"<br/>
	/// ・ドロップダウン形式の商品付帯情報にて、設定値一覧（選択可能項目）の最初にデフォルト値を追加する<br/>
	/// </remarks>
	protected ListItemCollection InsertDefaultAtFirstToDdlProductOptionSettingList(ListItemCollection lic, bool isNecessary)
	{
		if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED && isNecessary == false)
		{
			lic.Insert(0, new ListItem(ReplaceTag("@@DispText.variation_name_list.unselected@@"), ""));
		}
		return lic;
	}

	/// <summary>
	/// チェックボックス形式の商品付帯情報の値選択時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbProductOptionValueSettingListOnCheckedChanged(object sender, EventArgs e)
	{
		if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false) return;
		this.Process.cbProductOptionValueSettingListOnCheckedChanged(sender, e);
	}
}
