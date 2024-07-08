/*
=========================================================================================================
  Module      : 注文カートユーザーコントロール（配送先系）(OrderCartUserControl_OrderShipping.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.Order;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.ShopShipping;

/// <summary>
/// Class1 の概要の説明です
/// </summary>
public partial class OrderCartUserControl
{
	#region 配送情報入力画面系処理

	/// <summary>
	/// 注文者情報作成
	/// </summary>
	protected void CreateOrderOwner()
	{
		this.Process.CreateOrderOwner();
	}

	/// <summary>
	/// 配送先情報作成
	/// </summary>
	protected void CreateOrderShipping()
	{
		this.Process.CreateOrderShipping();
	}

	/// <summary>
	/// 注文メモ作成
	/// </summary>
	protected void CreateOrderMemo()
	{
		this.Process.CreateOrderMemo();
	}

	/// <summary>
	/// 配送方法選択
	/// </summary>
	/// <param name="riCart">リピーターアイテム</param>
	/// <param name="cartObj">カートオブジェクト</param>
	protected void SelectShippingMethod(RepeaterItem riCart, CartObject cartObj)
	{
		this.Process.SelectShippingMethod(riCart, cartObj);
	}

	/// <summary>
	/// 配送先グローバル関連項目設定
	/// </summary>
	/// <param name="riCart">リピーターアイテム</param>
	/// <param name="cartObj">カートオブジェクト</param>
	protected void SetOrderShippingGlobalColumn(RepeaterItem riCart, CartObject cartObj)
	{
		this.Process.SetOrderShippingGlobalColumn(riCart, cartObj);
	}

	/// <summary>
	/// 配送情報入力画面初期処理
	/// </summary>
	protected void InitComponentsOrderShipping()
	{
		this.Process.InitComponentsOrderShipping();
	}

	/// <summary>
	/// 配送情報入力画面表示初期処理
	/// </summary>
	/// <param name="e"></param>
	protected void InitComponentsDispOrderShipping(EventArgs e)
	{
		this.Process.InitComponentsDispOrderShipping(e);
	}
	/// <summary>
	/// 配送情報入力画面表示初期処理
	/// </summary>
	/// <param name="riTarget">対象リピータアイテム(cart/shipping)</param>
	/// <param name="e">イベント属性</param>
	protected void InitComponentsDispOrderShipping(RepeaterItem riTarget, EventArgs e)
	{
		this.Process.InitComponentsDispOrderShipping(riTarget, e);
	}

	/// <summary>
	/// 注文メモデザインセット
	/// </summary>
	/// <param name="riCart">対象カート</param>
	protected void SetOrderMemoDesign(RepeaterItem riCart)
	{
		this.Process.SetOrderMemoDesign(riCart);
	}

	/// <summary>
	/// 配送情報入力画面次へ画面クリック（注文者情報）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected bool lbNext_Click_OrderShipping_Owner(object sender, System.EventArgs e)
	{
		return this.Process.lbNext_Click_OrderShipping_Owner(sender, e);
	}

	/// <summary>
	/// 配送情報入力画面 送り主情報セット（ギフトのみ）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks>配送日時、注文メモは別</remarks>
	protected bool lbNext_Click_OrderShipping_ShippingSender(object sender, System.EventArgs e)
	{
		return this.Process.lbNext_Click_OrderShipping_ShippingSender(sender, e);
	}

	/// <summary>
	///  配送情報入力画面次へ画面クリック（Amazon注文者情報）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected bool lbNext_Click_OrderShipping_AmazonOwner(object sender, System.EventArgs e)
	{
		return this.Process.lbNext_Click_OrderShipping_AmazonOwner(sender, e);
	}

	/// <summary>
	/// 配送情報入力画面次へ画面クリック（配送先情報）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks>配送日時、注文メモは別</remarks>
	protected bool lbNext_Click_OrderShipping_Shipping(object sender, System.EventArgs e)
	{
		return this.Process.lbNext_Click_OrderShipping_Shipping(sender, e);
	}

	/// <summary>
	/// 配送情報入力画面次へ画面クリック（Amazon配送先情報）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks>配送日時、注文メモは別</remarks>
	protected bool lbNext_Click_OrderShipping_AmazonShipping(object sender, System.EventArgs e)
	{
		return this.Process.lbNext_Click_OrderShipping_AmazonShipping(sender, e);
	}

	/// <summary>
	/// 配送情報入力画面次へ画面クリック（その他情報）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected bool lbNext_Click_OrderShipping_Others(object sender, System.EventArgs e)
	{
		return this.Process.lbNext_Click_OrderShipping_Others(sender, e);
	}

	/// <summary>
	/// 配送情報入力画面次へ画面クリック（Amazonその他情報）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <returns>True:エラーなし, False:エラーあり</returns>
	protected bool lbNext_Click_OrderShipping_AmazonOthers(object sender, System.EventArgs e)
	{
		return this.Process.lbNext_Click_OrderShipping_AmazonOthers(sender, e);
	}

	/// <summary>
	/// 配送希望日ドロップダウンリスト作成
	/// </summary>
	/// <param name="coCart">カート情報</param>
	/// <param name="shopShipping">配送種別情報</param>
	/// <returns>配送希望日ドロップダウンリスト</returns>
	protected ListItemCollection GetShippingDateList(CartObject coCart, ShopShippingModel shopShipping)
	{
		return this.Process.GetShippingDateList(coCart, shopShipping);
	}
	/// <summary>
	/// 配送希望日ドロップダウンリスト作成
	/// </summary>
	/// <param name="csShipping">カート配送情報</param>
	/// <param name="shopShipping">配送種別情報</param>
	/// <returns>配送希望日ドロップダウンリスト</returns>
	protected ListItemCollection GetShippingDateList(CartShipping csShipping, ShopShippingModel shopShipping)
	{
		return this.Process.GetShippingDateList(csShipping, shopShipping);
	}

	/// <summary>
	/// 配送希望時間帯ドロップダウンリスト作成
	/// </summary>
	/// <param name="index">rCartListのインデックス</param>
	/// <param name="shippingNo">配送ナンバー</param>
	/// <returns>配送希望時間帯ドロップダウンリスト</returns>
	protected ListItemCollection GetShippingTimeList(int index, int shippingNo = 0)
	{
		return this.Process.GetShippingTimeList(index, shippingNo);
	}

	/// <summary>
	/// 配送希望日取得
	/// </summary>
	/// <param name="coCart">カート情報</param>
	/// <param name="shopShipping">配送種別情報</param>
	/// <returns>配送希望日</returns>
	protected string GetShippingDate(CartObject coCart, ShopShippingModel shopShipping)
	{
		return this.Process.GetShippingDate(coCart, shopShipping);
	}
	/// <summary>
	/// 配送希望日取得
	/// </summary>
	/// <param name="csShipping">カート配送情報</param>
	/// <param name="shopShipping">配送種別情報</param>
	/// <returns>配送希望日</returns>
	protected string GetShippingDate(CartShipping csShipping, ShopShippingModel shopShipping)
	{
		return this.Process.GetShippingDate(csShipping, shopShipping);
	}

	/// <summary>
	/// 配送希望時間帯取得
	/// </summary>
	/// <param name="cartNo">rCartListのインデックス</param>
	/// <param name="shippingNo">rCartShippingsのインデックス</param>
	/// <returns>配送希望時間帯</returns>
	protected string GetShippingTime(int cartNo, int shippingNo = 0)
	{
		return this.Process.GetShippingTime(cartNo, shippingNo);
	}
	/// <summary>
	/// 配送希望時間帯取得
	/// </summary>
	/// <param name="coCart">カート情報</param>
	/// <param name="index">rCartListのインデックス</param>
	/// <returns>配送希望時間帯</returns>
	protected string GetShippingTime(CartObject coCart, int index)
	{
		return this.Process.GetShippingTime(coCart, index);
	}
	/// <summary>
	/// 配送希望時間帯取得
	/// </summary>
	/// <param name="csShipping">カート配送情報</param>
	/// <param name="index">rCartListのインデックス</param>
	/// <returns>配送希望時間帯</returns>
	protected string GetShippingTime(CartShipping csShipping, int index)
	{
		return this.Process.GetShippingTime(csShipping, index);
	}

	/// <summary>
	/// 「住所検索」ボタン押下イベント（注文者情報）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSearchOwnergAddr_Click(object sender, EventArgs e)
	{
		this.Process.lbSearchOwnergAddr_Click(sender, e, isFocus : false);
	}

	/// <summary>
	/// 「住所検索」ボタン押下イベント（送り主情報）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSearchSenderAddr_Click(object sender, System.EventArgs e)
	{
		this.Process.lbSearchSenderAddr_Click(sender, e);
	}

	/// <summary>
	/// 郵便番号 変更イベント（注文者情報）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void tbOwnerZip2_OnTextChanged(object sender, EventArgs e)
	{
		lbSearchOwnergAddr_Click(sender, e);
	}

	/// <summary>
	/// 郵便番号 変更イベント（配送先情報）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void tbShippingZip2_OnTextChanged(object sender, EventArgs e)
	{
		lbSearchShippingAddr_Click(sender, e);
	}

	/// <summary>
	/// 「住所検索」ボタン押下イベント（配送先情報）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSearchShippingAddr_Click(object sender, System.EventArgs e)
	{
		this.Process.lbSearchShippingAddr_Click(sender, e);
	}

	/// <summary>
	/// カート１の配送先へ配送するチェックボックスクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbShipToCart1Address_OnCheckedChanged(object sender, System.EventArgs e)
	{
		this.Process.cbShipToCart1Address_OnCheckedChanged(sender, e);
	}

	/// <summary>
	/// 配送情報ドロップダウンリスト選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlShippingKbnList_OnSelectedIndexChanged(object sender, System.EventArgs e)
	{
		this.Process.ddlShippingKbnList_OnSelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// 配送先を保存するラジオボタン変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblSaveToUserShipping_OnSelectedIndexChanged(object sender, System.EventArgs e)
	{
		this.Process.rblSaveToUserShipping_OnSelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// 配送方法ドロップダウンリスト選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlShippingMethodList_OnSelectedIndexChanged(object sender, System.EventArgs e)
	{
		this.Process.ddlShippingMethodList_OnSelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// 注文者国ドロップダウンリスト選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlOwnerCountry_SelectedIndexChanged(object sender, EventArgs e)
	{
		this.Process.ddlOwnerCountry_SelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// 配送先国ドロップダウンリスト選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlShippingCountry_SelectedIndexChanged(object sender, EventArgs e)
	{
		this.Process.ddlShippingCountry_SelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// 配送先国ドロップダウンリスト選択インナーメソッド
	/// </summary>
	/// <param name="oParent">親コントロール</param>
	protected void ddlShippingCountry_SelectedIndexChangedInner(Control oParent)
	{
		this.Process.ddlShippingCountry_SelectedIndexChangedInner(oParent);
	}

	/// <summary>
	/// 送り主国ドロップダウンリスト選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlSenderCountry_SelectedIndexChanged(object sender, EventArgs e)
	{
		this.Process.ddlSenderCountry_SelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// ラップ済配送方法ドロップダウンリスト取得
	/// </summary>
	/// <param name="parent">親コントロール</param>
	/// <returns>配送方法ドロップダウンリスト</returns>
	/// <remarks>デフォルトで宅急便が選択されている。</remarks>
	protected WrappedDropDownList CreateWrappedDropDownListShippingMethod(Control parent)
	{
		return this.Process.CreateWrappedDropDownListShippingMethod(parent);
	}

	/// <summary>
	/// Display shipping date error message
	/// </summary>
	/// <returns>Return true if has error, otherwise return false</returns>
	protected bool DisplayShippingDateErrorMessage()
	{
		return this.Process.DisplayShippingDateErrorMessage();
	}

	/// <summary>
	/// AmazonPayで配送希望日についてのチェックとエラーメッセージ表示
	/// </summary>
	/// <returns>エラーあり：TRUE；エラーなし：FALSE</returns>
	protected bool CheckAndDisplayAmazonShippingDateErrorMessage()
	{
		return this.Process.CheckAndDisplayAmazonShippingDateErrorMessage();
	}

	/// <summary>
	/// 送り主住所国ISOコード取得
	/// </summary>
	/// <param name="cartIndex">カートインデックス</param>
	/// <param name="shippingIndex">配送先インデックス</param>
	/// <returns>ISOコード</returns>
	protected string GetSenderAddrCountryIsoCode(int cartIndex, int shippingIndex)
	{
		return this.Process.GetSenderAddrCountryIsoCode(cartIndex, shippingIndex);
	}

	/// <summary>
	/// 配送先住所国ISOコード取得
	/// </summary>
	/// <param name="cartIndex">カートインデックス</param>
	/// <param name="shippingIndex">配送先インデックス</param>
	/// <returns>ISOコード</returns>
	protected string GetShippingAddrCountryIsoCode(int cartIndex, int shippingIndex = 0)
	{
		return this.Process.GetShippingAddrCountryIsoCode(cartIndex, shippingIndex);
	}

	/// <summary>
	/// 台湾都市ドロップダウンリスト選択(注文者)
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlOwnerAddr2_SelectedIndexChanged(object sender, EventArgs e)
	{
		this.Process.ddlOwnerAddr2_SelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// 台湾都市ドロップダウンリスト選択(配送先)
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlShippingAddr2_SelectedIndexChanged(object sender, EventArgs e)
	{
		this.Process.ddlShippingAddr2_SelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// 配送先が指定可能かどうかを返します。
	/// </summary>
	/// <param name="index">rCartListのインデックス</param>
	/// <returns>指定カートで、配送先が設定可能であれば true</returns>
	protected bool CanInputShippingTo(int index)
	{
		return this.Process.CanInputShippingTo(index);
	}

	/// <summary>
	/// 配送希望日が指定可能かどうかを返します。
	/// </summary>
	/// <param name="index">rCartListのインデックス</param>
	/// <param name="shippingNo">配送ナンバー</param>
	/// <returns>指定カートの配送希望日が指定可能であれば true</returns>
	protected bool CanInputDateSet(int index, int shippingNo = 0)
	{
		return this.Process.CanInputDateSet(index, shippingNo);
	}

	/// <summary>
	/// 配送希望時間帯が指定可能かどうかを返します。
	/// </summary>
	/// <param name="index">rCartListのインデックス</param>
	/// <param name="shippingNo">配送ナンバー</param>
	/// <returns>指定カートの配送希望時間帯が指定可能であれば true</returns>
	protected bool CanInputTimeSet(int index, int shippingNo = 0)
	{
		return this.Process.CanInputTimeSet(index, shippingNo);
	}

	/// <summary>
	/// 配送希望日or時間帯が指定可能かどうかを返します。
	/// </summary>
	/// <param name="index">rCartListのインデックス</param>
	/// <param name="shippingNo">配送ナンバー</param>
	/// <returns>指定カートで、配送希望日or時間帯のどちらか一方でも指定可能であれば true</returns>
	protected bool CanInputDateOrTimeSet(int index, int shippingNo = 0)
	{
		return this.Process.CanInputDateOrTimeSet(index);
	}

	/// <summary>
	/// 宅配便か
	/// </summary>
	/// <param name="index">インデックス</param>
	/// <param name="shippingNo">配送ナンバー</param>
	/// <returns>宅配便か？</returns>
	protected bool IsExpressDelivery(int index, int shippingNo = 0)
	{
		return this.Process.IsExpressDelivery(index, shippingNo);
	}

	/// <summary>
	/// 注文者住所国ISOコード取得
	/// </summary>
	/// <param name="cartIndex">カートNO</param>
	/// <returns>ISOコード</returns>
	protected string GetOwnerAddrCountryIsoCode(int cartIndex)
	{
		return this.Process.GetOwnerAddrCountryIsoCode(cartIndex);
	}

	/// <summary>
	/// 正しい性別情報取得（データバインド用）
	/// </summary>
	/// <param name="sex">性別</param>
	/// <returns>性別</returns>
	protected string GetCorrectSexForDataBind(string sex)
	{
		return this.Process.GetCorrectSexForDataBind(sex);
	}

	/// <summary>
	/// 配送サービス選択肢取得
	/// </summary>
	/// <param name="cartNo">カートナンバー</param>
	/// <param name="shippingNo">配送ナンバー</param>
	/// <returns>配送サービス選択肢</returns>
	protected Dictionary<string, string> GetDeliveryCompanyListItem(int cartNo, int shippingNo = 0)
	{
		return this.Process.GetDeliveryCompanyListItem(cartNo, shippingNo);
	}

	/// <summary>
	/// 配送サービスドロップダウンリスト選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlDeliveryCompanyList_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		this.Process.ddlDeliveryCompanyList_OnSelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// 配送サービス選択
	/// </summary>
	/// <param name="parentItem">親リピーターアイテム</param>
	/// <param name="cartObj">カートオブジェクト</param>
	/// <param name="shippingNo">配送ナンバー</param>
	protected void SelectDeliveryCompany(RepeaterItem parentItem, CartObject cartObj, int shippingNo = 0)
	{
		this.Process.SelectDeliveryCompany(parentItem, cartObj, shippingNo);
	}

	/// <summary>
	/// 配送サービス名取得
	/// </summary>
	/// <param name="deliveryCompanyId">配送会社ID</param>
	/// <returns>配送サービス名</returns>
	protected string GetDeliveryCompanyName(string deliveryCompanyId)
	{
		return this.Process.GetDeliveryCompanyName(deliveryCompanyId);
	}

	/// <summary>
	/// 配送サービス表示可能の判断
	/// </summary>
	/// <param name="cartNo">カートナンバー</param>
	/// <param name="shippingNo">配送ナンバー</param>
	/// <returns>配送サービスが表示かどうか</returns>
	/// <remarks>指定したカートの選択可能な配送サービス個数が１であれば非表示</remarks>
	protected bool CanDisplayDeliveryCompany(int cartNo, int shippingNo = 0)
	{
		return this.Process.CanDisplayDeliveryCompany(cartNo, shippingNo);
	}

	/// <summary>
	/// コントロルが入っているカートのインデックス取得
	/// </summary>
	/// <param name="control">コントロル</param>
	/// <returns>コントロルが入っているカートのインデックス</returns>
	protected int GetCartIndexFromControl(Control control)
	{
		return this.Process.GetCartIndexFromControl(control);
	}

	/// <summary>
	/// 注文者情報グローバル関連項目設定
	/// </summary>
	protected void SetOrderOwnerGlobalColumn()
	{
		this.Process.SetOrderOwnerGlobalColumn();
	}

	/// <summary>
	/// Get Shipping Kbn List
	/// </summary>
	/// <param name="index">Cart Index</param>
	/// <returns>List Item Shipping Kbn</returns>
	protected ListItem[] GetShippingKbnList(int index)
	{
		return this.Process.GetShippingKbnList(index);
	}

	/// <summary>
	/// Check Box Carry Type Option Regist Checked Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbCarryTypeOptionRegist_CheckedChanged(object sender, EventArgs e)
	{
		this.Process.cbCarryTypeOptionRegist_CheckedChanged(sender, e);
	}

	/// <summary>
	/// Dropdown Invoice Carry Type Selected Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlInvoiceCarryType_SelectedIndexChanged(object sender, EventArgs e)
	{
		this.Process.ddlInvoiceCarryType_SelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// Dropdown Invoice Carry Type Option Selected Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlInvoiceCarryTypeOption_SelectedIndexChanged(object sender, EventArgs e)
	{
		this.Process.ddlInvoiceCarryTypeOption_SelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// Dropdown List Uniform Invoice Type Selected Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlUniformInvoiceType_SelectedIndexChanged(object sender, EventArgs e)
	{
		this.Process.ddlUniformInvoiceType_SelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// Dropdown List Uniform Invoice Type Option Selected Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlUniformInvoiceTypeOption_SelectedIndexChanged(object sender, EventArgs e)
	{
		this.Process.ddlUniformInvoiceTypeOption_SelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// Check Box Save To User Invoice Checked Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbSaveToUserInvoice_CheckedChanged(object sender, EventArgs e)
	{
		this.Process.cbSaveToUserInvoice_CheckedChanged(sender, e);
	}

	/// <summary>
	/// Shipping Receiving Store Type
	/// </summary>
	/// <returns>List item collection shipping receiving store type</returns>
	protected ListItem[] ShippingReceivingStoreType()
	{
		return this.Process.ShippingReceivingStoreType();
	}

	/// <summary>
	/// DropDownList Shipping Receiving Store Type Selected Index Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlShippingReceivingStoreType_SelectedIndexChanged(object sender, EventArgs e)
	{
		this.Process.ddlShippingReceivingStoreType_SelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// Click Open Window EcPay
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbOpenEcPay_Click(object sender, EventArgs e)
	{
		this.Process.lbOpenEcPay_Click(sender, e);
	}

	#region 古い形式のメソッド（非推奨）
	/// <summary>
	/// 配送日設定可能状態取得
	/// </summary>
	/// <param name="iCartIndex">rCartListのインデックス</param>
	/// <returns>indexで指定したカートの配送日設定可能フラグが有効かどうか</returns>
	[Obsolete("[V5.2] CanInputDateSet() を使用してください")]
	protected bool GetShippingDateSetFlgValid(int iCartIndex)
	{
		return this.Process.GetShippingDateSetFlgValid(iCartIndex);
	}
	/// <summary>
	/// 配送希望時間帯設定可能状態取得
	/// </summary>
	/// <param name="iCartIndex">rCartListのインデックス</param>
	/// <returns>indexで指定したカートの配送希望時間帯設定可能フラグが有効かどうか</returns>
	[Obsolete("[V5.2] CanInputTimeSet() を使用してください")]
	protected bool GetShippingTimeSetFlgValid(int iCartIndex)
	{
		return this.Process.GetShippingTimeSetFlgValid(iCartIndex);
	}
	#endregion

	#endregion

	#region プロパティ
	/// <summary>ユーザー区分</summary>
	protected string UserKbn
	{
		get { return this.Process.UserKbn; }
		set { this.Process.UserKbn = value; }
	}
	/// <summary>アドレス帳情報</summary>
	protected ListItemCollection UserShippingList
	{
		get { return this.Process.UserShippingList; }
	}
	/// <summary>都道府県情報</summary>
	protected ListItemCollection Addr1List
	{
		get { return this.Process.Addr1List; }
	}
	/// <summary>ユーザー国表示情報(</summary>
	protected ListItemCollection UserCountryDisplayList
	{
		get { return this.Process.UserCountryDisplayList; }
	}
	/// <summary>ユーザー州情報</summary>
	protected ListItemCollection UserStateList
	{
		get { return this.Process.UserStateList; }
	}
	/// <summary>ユーザー台湾都市情報</summary>
	protected ListItemCollection UserTwCityList
	{
		get { return this.Process.UserTwCityList; }
		set { this.Process.UserTwCityList = value; }
	}
	/// <summary>配送可能な国表示情報</summary>
	protected ListItemCollection ShippingAvailableCountryDisplayList
	{
		get { return this.Process.ShippingAvailableCountryDisplayList; }
	}
	/// <summary>注文者生年月日 年</summary>
	protected ListItemCollection OrderOwnerBirthYear
	{
		get { return this.Process.OrderOwnerBirthYear; }
	}
	/// <summary>注文者生年月日 月</summary>
	protected ListItemCollection OrderOwnerBirthMonth
	{
		get { return this.Process.OrderOwnerBirthMonth; }
	}
	/// <summary>注文者生年月日 日</summary>
	protected ListItemCollection OrderOwnerBirthDay
	{
		get { return this.Process.OrderOwnerBirthDay; }
	}
	/// <summary>注文者性別</summary>
	protected ListItemCollection OrderOwnerSex
	{
		get { return this.Process.OrderOwnerSex; }
	}
	/// <summary>配送方法リスト</summary>
	protected List<ListItemCollection> ShippingMethodList
	{
		get { return this.Process.ShippingMethodList; }
	}
	/// <summary>データバインド用配送種別情報</summary>
	protected List<ShopShippingModel> ShopShippingList
	{
		get { return this.Process.ShopShippingList; }
		set { this.Process.ShopShippingList = value; }
	}
	/// <summary>最初のリピータアイテムユニークID</summary>
	protected RepeaterItem FirstRpeaterItem
	{
		get { return this.Process.FirstRpeaterItem; }
	}
	/// <summary>パスワード表示フラグ</summary>
	protected bool IsVisible_UserPassword
	{
		get { return this.Process.IsVisible_UserPassword; }
	}
	/// <summary>カートアイテムインデックス番号（複数カート時のグローバル情報設定に利用）</summary>
	protected int CartItemIndexTmp
	{
		get { return this.Process.CartItemIndexTmp; }
		set { this.Process.CartItemIndexTmp = value; }
	}
	/// <summary>カート配送先アイテムインデックス番号（複数カート時のグローバル情報設定に利用。ギフトのみで利用）</summary>
	protected int CartShippingItemIndexTmp
	{
		get { return this.Process.CartShippingItemIndexTmp; }
		set { this.Process.CartShippingItemIndexTmp = value; }
	}
	/// <summary>配送先国コード</summary>
	public string CountryIsoCode
	{
		get { return this.Process.CountryIsoCode; }
		set { this.Process.CountryIsoCode = value; }
	}
	/// <summary>Convenience Store Limit Kg</summary>
	public decimal ConvenienceStoreLimitKg
	{
		get { return this.Process.ConvenienceStoreLimitKg; }
	}
	/// <summary>Convenience Store Limit Kg 7-ELEVEN</summary>
	public decimal ConvenienceStoreLimitKg7Eleven
	{
		get { return this.Process.ConvenienceStoreLimitKg7Eleven; }
	}
	#endregion
}