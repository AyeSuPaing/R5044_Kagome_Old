/*
=========================================================================================================
  Module      : 注文カートユーザーコントロール(OrderCartUserControl.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using w2.App.Common.Global.Region;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.ContentsLog;
using w2.Domain.Coupon.Helper;

/// <summary>
/// 注文カートユーザーコントロール
/// </summary>
public partial class OrderCartUserControl : OrderPageDispUserControl
{
	/// <summary>
	/// 商品カート投入
	/// </summary>
	/// <param name="strShopId">店舗ID</param>
	/// <param name="strProductId">商品ID</param>
	/// <param name="strVariationId">バリエーションID</param>
	/// <param name="addCartKbn">カート投入区分</param>
	/// <param name="iProductCount">商品数</param>
	/// <param name="lProductOptionSelectedValues">付帯情報</param>
	/// <param name="contentsLog">コンテンツログ</param>
	/// <returns>エラーメッセージ</returns>
	protected string AddProductToCart(
		string strShopId,
		string strProductId,
		string strVariationId,
		Constants.AddCartKbn addCartKbn,
		int iProductCount,
		List<string> lProductOptionSelectedValues,
		ContentsLogModel contentsLog = null)
	{
		return this.Process.AddProductToCart(
			strShopId,
			strProductId,
			strVariationId,
			addCartKbn,
			iProductCount,
			lProductOptionSelectedValues,
			contentsLog);
	}

	/// <summary>
	/// 商品セットカート投入
	/// </summary>
	/// <param name="strShopId">商品ID</param>
	/// <param name="strProductSetId">商品セットID</param>
	/// <param name="lProductSetItems">商品セットアイテム</param>
	protected string AddProductSetToCart(
		string strShopId,
		string strProductSetId,
		List<Dictionary<string, string>> lProductSetItems)
	{
		return this.Process.AddProductSetToCart(strShopId, strProductSetId, lProductSetItems);
	}

	/// <summary>
	/// ポイント情報セット（再計算は呼び出し元で行う）
	/// </summary>
	/// <param name="wrCartList">カートリスト</param>
	protected void SetUsePointData(WrappedRepeater wrCartList)
	{
		this.Process.SetUsePointData(wrCartList);
	}

	/// <summary>
	/// クーポン情報セット（再計算は呼び出し元で行う）
	/// </summary>
	/// <param name="wrCartList">カートリスト</param>
	protected void SetUseCouponData(WrappedRepeater wrCartList)
	{
		this.Process.SetUseCouponData(wrCartList);
	}

	/// <summary>
	/// 過去に購入した商品がある場合カートから削除する
	/// </summary>
	/// <returns>カート商品削除数</returns>
	protected void ProductOrderLimitItemDelete()
	{
		this.Process.ProductOrderLimitItemDelete();
	}

	/// <summary>
	/// シルバーエッグ用カート内商品ID取得
	/// </summary>
	/// <returns>カート内商品ID</returns>
	/// <remarks>
	/// ・カンマ区切りで並べる
	/// </remarks>
	protected string GetCartProductsForSilveregg()
	{
		return string.Join(",", this.CartList.Items.SelectMany(x => x.Items.Select(y => y.RecommendProductId)));
	}

	#region レコナイズ削除予定記述
	/// <summary>
	/// レコナイズ用カート内商品ID取得
	/// </summary>
	/// <returns>カート内商品ID</returns>
	/// <remarks>
	/// ・商品IDの頭に"P"をつける
	/// ・カンマ区切りで並べる
	/// ・最大10件
	/// </remarks>
	protected string GetCartProductsForReconize()
	{
		StringBuilder sbCartProducts = new StringBuilder();
		int iCount = 0;

		foreach (CartObject coCart in this.CartList)
		{
			foreach (CartProduct cpCartProduct in coCart)
			{
				sbCartProducts.Append((sbCartProducts.Length == 0) ? "" : ",");
				sbCartProducts.Append("P").Append(cpCartProduct.ProductId);
				iCount++;

				if (iCount >= Constants.CONST_RECONIZE_REQUEST_MAX_ITEM_CODE_COUNT)
				{
					return sbCartProducts.ToString();
				}
			}
		}
		return sbCartProducts.ToString();
	}
	#endregion

	/// <summary>
	/// カート装飾文字表示
	/// </summary>
	/// <param name="objCartObject">カートオブジェクト</param>
	/// <param name="strParams">装飾文字（[0]:ギフトのみ, [1]:デジタルコンテンツのみ）</param>
	/// <returns>カート装飾文字</returns>
	/// <remarks>たとえば「(ギフト)」とかの文字を出力する</remarks>
	protected string DispCartDecolationString(object objCartObject, params string[] strParams)
	{
		CartObject co = (CartObject)objCartObject;

		if (co.IsGift) return strParams[0];
		if (co.IsDigitalContentsOnly) return strParams[1];

		return "";
	}

	/// <summary>
	/// カートオブジェクト検索
	/// </summary>
	/// <param name="objTarget">ターゲットオブジェクト</param>
	/// <returns>カートオブジェクト</returns>
	protected CartObject FindCart(object objTarget)
	{
		return this.Process.FindCart(objTarget);
	}

	/// <summary>
	/// カートオブジェクトの定期会員フラグ設定
	/// </summary>
	/// <param name="cartObjList">カートオブジェクトリスト</param>
	protected void SetFixedPurchaseMemberFlgForCartObject(List<CartObject> cartObjList)
	{
		this.Process.SetFixedPurchaseMemberFlgForCartObject(cartObjList);
	}

	/// <summary>
	/// 定期会員割引が無効であるか判定
	/// </summary>
	/// <returns>true：定期会員割引無効、false：定期会員割引有効</returns>
	private bool IsApplyFixedPurchaseMemberDiscountInvaild()
	{
		return this.Process.IsApplyFixedPurchaseMemberDiscountInvaild();
	}

	/// <summary>
	/// クーポン入力方法取得
	/// </summary>
	/// <returns>クーポン入力方法</returns>
	protected ListItemCollection GetCouponInputMethod()
	{
		return CouponOptionUtility.GetCouponInputMethod();
	}

	/// <summary>
	/// カートに対し利用可能なクーポン情報取得
	/// </summary>
	/// <param name="cart">カートオブジェクト</param>
	/// <param name="referralCode">Referral code</param>
	/// <returns>クーポン情報</returns>
	protected UserCouponDetailInfo[] GetUsableCoupons(CartObject cart, string referralCode = "")
	{
		string languageCode = null;
		string languageLocaleId = null;
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			languageCode = RegionManager.GetInstance().Region.LanguageCode;
			languageLocaleId = RegionManager.GetInstance().Region.LanguageLocaleId;
		}

		var usableCoupons = CouponOptionUtility.GetUsableCoupons(
			this.LoginUserId,
			this.LoginUserMail,
			cart,
			languageCode,
			languageLocaleId,
			referralCode);

		// ユーザー表示用のクーポン名が空である場合、管理用のクーポン名を表示するようにする
		foreach (var cp in usableCoupons)
		{
			if (string.IsNullOrEmpty(cp.CouponDispName) == false) continue;
			cp.CouponDispName = cp.CouponName;
		}

		return usableCoupons;
	}

	/// <summary>
	/// 利用可能回数表示文字列取得
	/// </summary>
	/// <param name="coupon">ユーザークーポン詳細情報</param>
	/// <returns>利用可能回数表示</returns>
	protected string GetCouponCount(UserCouponDetailInfo coupon)
	{
		return CouponOptionUtility.GetCouponCount(coupon);
	}

	/// <summary>
	/// クーポンの初期設定
	/// </summary>
	/// <param name="repeaterItem">親リピーターアイテム</param>
	protected void InitializeCouponComponents(RepeaterItem repeaterItem)
	{
		this.Process.InitializeCouponComponents(repeaterItem);
	}

	/// <summary>
	/// カートにて注文拡張項目を表示できるか
	/// </summary>
	/// <returns>表示できるかどうか</returns>
	protected bool IsDisplayOrderExtend()
	{
		return this.Process.IsDisplayOrderExtend();
	}

	/// <summary>入力チェック用カートID</summary>
	protected int CurrentCartIndex
	{
		get { return this.Process.CurrentCartIndex; }
		set { this.Process.CurrentCartIndex = value; }
	}
	/// <summary>エラーメッセージ</summary>
	protected string ErrorMessage
	{
		get { return this.Process.ErrorMessage; }
		set { this.Process.ErrorMessage = value; }
	}
	/// <summary>カートエラーメッセージ</summary>
	protected OrderPage.CartErrorMessages ErrorMessages
	{
		get { return this.Process.ErrorMessages; }
	}
	/// <summary> 注文同梱有無 </summary>
	protected bool IsOrderCombined
	{
		get { return this.Process.IsOrderCombined; }
	}
	/// <summary> 注文同梱後 決済再選択有無 </summary>
	protected bool IsPaymentReselect
	{
		get { return this.Process.IsPaymentReselect; }
	}
	/// <summary>領収書希望選択肢リスト</summary>
	protected ListItemCollection ReceiptFlgListItems
	{
		get { return this.Process.ReceiptFlgListItems; }
	}
}