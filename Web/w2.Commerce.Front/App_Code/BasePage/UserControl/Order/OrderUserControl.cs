/*
=========================================================================================================
  Module      : 注文ユーザーコントロール(OrderUserControl.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Data;
using System.Web.UI.WebControls;
using w2.App.Common.Order;
using w2.App.Common.Web.Process;

/// <summary>
/// 注文ユーザーコントロール
/// </summary>
public class OrderUserControl : ProductUserControl
{
	/// <summary>
	/// 利用ポイントチェック
	/// </summary>
	/// <param name="coTarget">対象カートオブジェクト</param>
	/// <returns>エラーメッセージ</returns>
	public string CheckUsePoint(CartObject coTarget)
	{
		return this.Process.CheckUsePoint(coTarget);
	}

	/// <summary>
	/// カート存在チェック
	/// </summary>
	/// <remarks>
	/// カートが存在しない場合、エラーページへ遷移
	/// </remarks>
	public void CheckCartExists()
	{
		this.Process.CheckCartExists();
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
		this.Process.CheckCartExists(messages);
	}

	/// <summary>
	/// カート注文者存在チェック
	/// </summary>
	/// <remarks>
	/// 注文者が存在しない場合カート一覧ページへ飛ぶ
	/// </remarks>
	public void CheckCartOwnerExists()
	{
		this.Process.CheckCartOwnerExists();
	}

	/// <summary>
	/// カート配送先存在チェック
	/// </summary>
	/// <remarks>
	/// 注文者が存在しない場合カート一覧ページへ飛ぶ
	/// </remarks>
	public void CheckCartShippingExists()
	{
		this.Process.CheckCartShippingExists();
	}

	/// <summary>
	/// カート注文者チェック
	/// </summary>
	/// <remarks>
	/// メールアドレスが登録されていない OR ログイン状態とカート注文者情報が不整合の場合配送先ページへ飛ぶ
	/// </remarks>
	public void CheckCartOwner()
	{
		this.Process.CheckCartOwner();
	}

	/// <summary>
	/// URLセッションチェック(注文時)
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
	///	３. 注文時のみ使用。URLが存在しない場合はカート画面を表示、URLが一致しない場合はセッションに格納されているURLへ遷移
	///
	/// ※主に、入力データを確認画面で正しく受け取るための制御に使用する。
	/// </remarks>
	public void CheckOrderUrlSession()
	{
		this.Process.CheckOrderUrlSession();
	}

	/// <summary>
	/// 郵便番号文字列作成
	/// </summary>
	/// <param name="objCartInfo">カート情報（注文者or配送先）</param>
	/// <returns>郵便番号文字列</returns>
	public string CreateZipString(object objCartInfo)
	{
		return this.Process.CreateZipString(objCartInfo);
	}
	/// <summary>
	/// 郵便番号文字列作成
	/// </summary>
	/// <param name="strZip1">郵便番号1</param>
	/// <param name="strZip2">郵便番号2</param>
	/// <returns>郵便番号文字列</returns>
	public string CreateZipString(string strZip1, string strZip2)
	{
		return this.Process.CreateZipString(strZip1, strZip2);
	}

	/// <summary>
	/// 電話番号文字列作成
	/// </summary>
	/// <param name="objCartInfo">カート情報（注文者or配送先）</param>
	/// <returns>電話番号文字列</returns>
	public string CreateTelString(object objCartInfo)
	{
		return this.Process.CreateTelString(objCartInfo);
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
		return this.Process.CreateTelString(strTel1, strTel2, strTel3);
	}

	/// <summary>
	/// クーポンコード取得
	/// </summary>
	/// <param name="ccCoupon">クーポン情報</param>
	/// <returns>クーポンコード</returns>
	public string GetCouponCode(CartCoupon ccCoupon)
	{
		return this.Process.GetCouponCode(ccCoupon);
	}

	/// <summary>
	/// 表示用クーポン名取得
	/// </summary>
	/// <param name="ccCoupon">クーポン情報</param>
	/// <returns>表示用クーポン名取得</returns>
	public string GetCouponDispName(CartCoupon ccCoupon)
	{
		return this.Process.GetCouponCode(ccCoupon);
	}

	/// <summary>
	/// 商品が有効か?
	/// </summary>
	/// <param name="orderItem">注文商品</param>
	/// <returns>有効：true、無効：false</returns>
	public bool IsProductValid(DataRowView orderItem)
	{
		return this.Process.IsProductValid(orderItem);
	}

	/// <summary>
	/// ギフト購入したもののカート商品と配送先情報に紐づく商品情報の整合性チェック
	/// </summary>
	/// <param name="cartList">カートリスト</param>
	/// <returns>整合性チェック結果</returns>
	public bool IsConsistentGiftItemsAndShippings(CartObjectList cartList)
	{
		return this.Process.IsConsistentGiftItemsAndShippings(cartList);
	}

	/// <summary>
	/// 新しいセッションを生成し、配送先入力画面へ遷移
	/// </summary>
	protected void RedirectToOrderShippingWithNewSession()
	{
		this.Process.RedirectToOrderShippingWithNewSession();
	}

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
	protected virtual void rCartList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		this.Process.rCartList_ItemCommand(source, e);
	}

	/// <summary>
	/// Get coupon name
	/// </summary>
	/// <param name="target">Target</param>
	/// <returns>Coupon name</returns>
	protected string GetCouponName(object target)
	{
		return this.Process.GetCouponName(target);
	}

	/// <summary>
	/// 請求先住所取得オプションの対象か判定
	/// </summary>
	/// <returns>判定結果（TRUE：対象、FALSE：）</returns>
	public bool IsTargetToExtendedAmazonAddressManagerOption()
	{
		return this.Process.IsTargetToExtendedAmazonAddressManagerOption();
	}

	/// <summary>プロセス</summary>
	protected new OrderFlowProcess Process
	{
		get { return (OrderFlowProcess)this.ProcessTemp; }
	}
	/// <summary>プロセステンポラリ</summary>
	protected override IPageProcess ProcessTemp
	{
		get
		{
			if (m_processTmp == null) m_processTmp = new OrderFlowProcess(this, this.ViewState, this.Context);
			return m_processTmp;
		}
	}
	/// <summary>カートリスト</summary>
	protected CartObjectList CartList
	{
		get { return this.Process.CartList; }
		set { this.Process.CartList = value; }
	}
}
