/*
=========================================================================================================
  Module      : レコメンド表示出力コントローラ処理(BodyRecommend.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Order;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Extensions;
using w2.Domain;

public partial class Form_Common_BodyRecommend : RecommendUserControl
{
	#region ラップ済みコントロール
	WrappedLinkButton WlbAddItem { get { return GetWrappedControl<WrappedLinkButton>("lbAddItem"); } }
	WrappedDropDownList WddlSubscriptionCourseId { get { return GetWrappedControl<WrappedDropDownList>("ddlSubscriptionCourseId", string.Empty); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// レコメンド設定が無効の場合は何もしない
		if (Constants.RECOMMEND_OPTION_ENABLED == false) return;

		if (!IsPostBack)
		{
			// レコメンド設定セット
			SetRecommend();
		}
	}

	/// <summary>
	/// レコメンドアイテム投入イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbAddItem_Click(object sender, EventArgs e)
	{
		// 支払方法がAmazonペイメントを保持している？
		var hasAmazonPayment = this.CartList.Items.Any(
			cart => ((cart.Payment != null)
				&& (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)));

			foreach (var item in this.Recommend.Items)
			{
			item.SubscriptionBoxCourseId = this.WddlSubscriptionCourseId.SelectedValue;
			}

		// レコメンドアイテム投入
		this.CartList.AddRecommendItem(this.Recommend);

		// 注文確認ページレコメンドで注文確定完了場合、レコメンド履歴更新対応用
		if (this.Recommend != null)
		{
			if (this.Recommend.RecommendDisplayPage == Constants.FLG_RECOMMEND_RECOMMEND_DISPLAY_PAGE_ORDER_CONFIRM)
			{
				// 注文確認ページレコメンド情報をカートリストに渡す
				this.CartList.RecommendOrderConfirm = this.Recommend;
				this.CartList.RecommendHistoryNoOrderConfirm = this.RecommendHistoryNo;
			}
		}

		// 画面遷移
		RedirectPage(hasAmazonPayment);
	}

	#region メソッド
	/// <summary>
	/// レコメンド設定セット
	/// </summary>
	private void SetRecommend()
	{
		// カート情報セット
		// ※ページ側でカート情報がセットされている場合はそちらを優先する
		var targetCartList =
			(this.Cart != null)
			? new[] { this.Cart }
			: this.CartList.Items.ToArray();

		// ギフト購入の場合はレコメンド表示しない
		if (targetCartList.Any(cart => cart.IsGift))
		{
			this.Visible = false;
			return;
		}

		// レコメンド表示ページチェック
		var displayPage = this.Cart.IsNeedCheckOrderCompleteRecommend
			? Constants.FLG_RECOMMEND_RECOMMEND_DISPLAY_PAGE_ORDER_COMPLETE
			: Constants.FLG_RECOMMEND_RECOMMEND_DISPLAY_PAGE_ORDER_CONFIRM;

		// レコメンド表示をセット
		var buttonId = this.WlbAddItem.HasInnerControl
			? this.WlbAddItem.InnerControl.UniqueID
			: string.Empty;

		var recommend = SetRecommend(
			targetCartList,
			this.CartList,
			this.IsLoggedIn ? this.LoginUserId : targetCartList.First().CartUserId,
			buttonId,
			displayPage);

		if ((recommend == null)
			|| (recommend.IsUpsell == false)
			|| (recommend.Items.Any(item => item.IsSubscriptionBox) == false))
		{
			return;
		}

		var recommendSubscriptionCourses = recommend.Items
			.Where(item => item.IsSubscriptionBox)
			.SelectMany(
				item => DomainFacade.Instance.SubscriptionBoxService.GetAvailableSubscriptionBoxesByProductId(
					item.RecommendItemProductId,
					item.RecommendItemVariationId,
					item.ShopId))
			.Distinct(subscription => subscription.CourseId)
			.Where(subscription => subscription.IsValid)
			.Select(subscription => new ListItem(subscription.DisplayName, subscription.CourseId))
			.ToArray();

		this.WddlSubscriptionCourseId.Items.AddRange(recommendSubscriptionCourses);
		if (this.WddlSubscriptionCourseId.Items.FindByValue(this.Cart.SubscriptionBoxCourseId) != null)
		{
			this.WddlSubscriptionCourseId.SelectedValue = this.Cart.SubscriptionBoxCourseId;
		}
		this.WddlSubscriptionCourseId.Visible = recommendSubscriptionCourses.Any();
	}

	/// <summary>
	/// 画面遷移
	/// </summary>
	/// <param name="hasAmazonPayment">支払方法がAmazonペイメントを保持している？</param>
	private void RedirectPage(bool hasAmazonPayment)
	{
		// 注文 and ランディングカート画面ではない
		if ((this.IsOrderPage == false)
			&& (this.IsLandingCartPage == false))
		{
			// 同じ画面で画面遷移（※再描画）
			Response.Redirect(Request.Url.AbsolutePath);
		}

		// レコメンドアイテム追加時パラメータを付与し、入力エラーになるまで注文画面を遷移
		var urlParameter = string.Format("?{0}=1", Constants.REQUEST_KEY_ADD_RECOMMEND_ITEM_FLG);

		// 支払方法がAmazonペイメントから変更があった場合はカート画面へ遷移
		var notHasAmazonPayment = this.CartList.Items.Any(cart =>
			(cart.Payment != null)
			&& (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)) == false;
		if (this.IsOrderPage && hasAmazonPayment && notHasAmazonPayment)
		{
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST + urlParameter);
		}

		// 注文者情報を保持していない？
		var notHasOwner = this.CartList.Items.Any(
			cart => ((cart.Payment != null)
				&& (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2))
					? string.IsNullOrEmpty(cart.Owner.Name)
					: (string.IsNullOrEmpty(cart.Owner.Addr1))
					&& (string.IsNullOrEmpty(cart.Owner.Addr2)));

		// 配送先情報を保持していない？
		var notHasShipping = this.CartList.Items.Any(
			cart => cart.Shippings.Any(
				s => (string.IsNullOrEmpty(s.Addr1))
					&& (string.IsNullOrEmpty(s.Addr2))));

		// 送り主情報を保持していない？
		var notHasSender = false;
		if (Constants.GIFTORDER_OPTION_ENABLED)
		{
			notHasSender = this.CartList.Items.Any(cart => 
				cart.Shippings.Any(s => 
					string.IsNullOrEmpty(s.SenderAddr1)));
		}

		// 支払方法情報を保持していない？
		var notHasPayment = this.CartList.Items.Any(cart => cart.Payment == null);

		// 定期情報を保持していない？
		var notHasFixedPurchase = this.CartList.Items.Any(cart =>
			cart.Items.Any(i => i.IsFixedPurchase)
			&& cart.Shippings.Any(s => string.IsNullOrEmpty(s.FixedPurchaseKbn)));

		// 再入力が必要である場合、各入力画面へ遷移
		var reRegister = (notHasOwner || notHasShipping || notHasSender || notHasPayment || notHasFixedPurchase);
		var page = string.Empty;
		// 注文？
		if (this.IsOrderPage)
		{
			page = reRegister
				? Constants.PAGE_FRONT_ORDER_SHIPPING
				: Constants.PAGE_FRONT_ORDER_CONFIRM;
		}
		// ランディングカート？
		else if (this.IsLandingCartPage)
		{
			page = reRegister
				? this.LandingCartInputAbsolutePath.Substring(Constants.PATH_ROOT.Length)
				: Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM;
		}

		// ランディングカート保持用画面遷移正当性チェック用セッションキー
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK + this.LandingCartInputAbsolutePath] = page;

		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = page;
		if (this.IsLandingCartPage)
		{
			if (string.IsNullOrEmpty(Request.Url.Query) == false) urlParameter = urlParameter.Replace('?', '&');
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + page + Request.Url.Query + (reRegister ? urlParameter : string.Empty));
		}
		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + page + (reRegister ? urlParameter : string.Empty));
	}
	#endregion

	#region プロパティ
	/// <summary>カート情報</summary>
	/// <remarks>ページ側で設定される</remarks>
	public CartObject Cart { get; set; }
	/// <summary>カートリスト情報</summary>
	private CartObjectList CartList
	{
		get
		{
			// ランディングカート場合はランディングカート用セッションを返す
			if (this.IsLandingCartPage) return (CartObjectList)Session[this.LadingCartSessionKey];
			return this.GetCartObjectList();
		}
	}
	#endregion
}