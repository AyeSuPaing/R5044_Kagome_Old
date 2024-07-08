/*
=========================================================================================================
  Module      : カートリスト系画面入力基底ページ(CartListPage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Web.UI.WebControls;
using w2.Domain.SetPromotion;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.App.Common.Order;

/// <summary>
/// CartListPage の概要の説明です
/// </summary>
public class CartListPage : OrderCartPage
{
	/// <summary>
	/// 画面表示
	/// </summary>
	protected void Reload()
	{
		this.Process.Reload();
	}

	/// <summary>
	/// 再計算リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbRecalculate_Click(object sender, System.EventArgs e)
	{
		this.Process.lbRecalculate_Click(sender, e);
	}

	/// <summary>
	/// カートコピーボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCopyCart_Click(object sender, System.EventArgs e)
	{
		this.Process.lbCopyCart_Click(sender, e);
	}

	/// <summary>
	/// カート削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDeleteCart_Click(object sender, System.EventArgs e)
	{
		this.Process.lbDeleteCart_Click(sender, e);
	}

	/// <summary>
	/// 入力チェック＆オブジェクトへセット
	/// </summary>
	protected void CheckAndSetInputData()
	{
		this.Process.CheckAndSetInputData();
	}

	/// <summary>
	/// リピータイベント
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rCartList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		this.Process.rCartList_ItemCommand(source, e);
	}

	/// <summary>
	/// 対象商品を含むセットプロモーションを取得
	/// </summary>
	/// <param name="productInfo">カート商品情報</param>
	/// <returns>対象商品を含むセットプロモーション</returns>
	protected SetPromotionModel[] GetSetPromotionByProduct(CartProduct productInfo)
	{
		var setPromotionList = DataCacheControllerFacade.GetSetPromotionCacheController().GetSetPromotionByProduct(
			productInfo,
			this.MemberRankId,
			this.IsPc ? Constants.FLG_ORDER_ORDER_KBN_PC : Constants.FLG_ORDER_ORDER_KBN_SMARTPHONE,
			this.LoginUserHitTargetListIds);

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			setPromotionList = NameTranslationCommon.SetSetPromotionTranslationData(
				setPromotionList,
				RegionManager.GetInstance().Region.LanguageCode,
				RegionManager.GetInstance().Region.LanguageLocaleId);
		}
		return setPromotionList;
	}

	/// <summary>
	/// カートノベルティ取得
	/// </summary>
	/// <param name="cartId">カートID</param>
	/// <returns>カートノベルティ</returns>
	protected CartNovelty[] GetCartNovelty(string cartId)
	{
		return this.Process.GetCartNovelty(cartId);
	}

	/// <summary>
	/// 数量変更可能？
	/// </summary>
	/// <param name="cart">カート</param>
	/// <param name="product">カート商品</param>
	/// <returns>可能：true、不可：false</returns>
	protected bool IsChangeProductCount(CartObject cart, CartProduct product)
	{
		// ギフトカート or ノベルティの場合、不可（false）を返す
		if (cart.IsGift) return false;
		if (Constants.NOVELTY_OPTION_ENABLED && (product.IsNovelty)) return false;

		return true;
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
	/// ２クリックボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbTwoClickButton_Click(object sender, EventArgs e)
	{
		SessionManager.IsTwoClickButton = true;
		this.Process.lbNext_Click(sender, e);
	}

	/// <summary>
	/// カートチェック
	/// </summary>
	/// <returns>チェックOKか</returns>
	protected bool CheckCart()
	{
		return this.Process.CheckCart();
	}

	/// <summary>
	/// カート一覧ページ向けペイパル認証完了
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void PayPalAuthCompleteForCartList(object sender, EventArgs e)
	{
		this.Process.PayPalAuthCompleteForCartList(sender, e);
	}

	/// <summary>
	/// リクエストからカートに商品投入
	/// </summary>
	/// <param name="addCartHttpRequest">カート投入URL情報</param>
	protected void AddProductToCartFromRequest(AddCartHttpRequest addCartHttpRequest)
	{
		this.Process.AddProductToCartFromRequest(addCartHttpRequest);
	}

	/// <summary>会員登録ポイント</summary>
	protected decimal UserRegistPoint
	{
		get { return this.Process.UserRegistPoint; }
		set { this.Process.UserRegistPoint = value; }
	}
	/// <summary>エラーメッセージ表示用</summary>
	protected string DispErrorMessage
	{
		get { return this.Process.DispErrorMessage; }
		set { this.Process.DispErrorMessage = value; }
	}
	/// <summary>カートノベルティリスト</summary>
	protected CartNoveltyList CartNoveltyList
	{
		get { return this.Process.CartNoveltyList; }
		set { this.Process.CartNoveltyList = value; }
	}
	/// <summary>PayPalショートカット表示するか</summary>
	protected bool DispPayPalShortCut
	{
		get { return this.Process.DispPayPalShortCut; }
	}
}
