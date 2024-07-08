/*
=========================================================================================================
  Module      : 注文表示系ページ(OrderPageDisp.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common.Order;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.Payment;

///*********************************************************************************************
/// <summary>
/// 注文表示系ページ
/// </summary>
///*********************************************************************************************
public partial class OrderPageDisp : OrderPage
{
	#region ラップ済みコントロール宣言
	protected WrappedRepeater WrCartList { get { return this.Process.WrCartList; } }
	/// <summary>注文系エラーメッセージ</summary>
	protected WrappedLiteral WlOrderErrorMessage { get { return GetWrappedControl<WrappedLiteral>("lOrderErrorMessage"); } }
	/// <summary>Paidy token hidden field control</summary>
	protected WrappedHiddenField WhfPaidyTokenId { get { return GetWrappedControl<WrappedHiddenField>(this.Process.FirstRpeaterItem, "hfPaidyTokenId"); } }
	/// <summary>Paidy pay seleced hidden field control</summary>
	protected WrappedHiddenField WhfPaidyPaySelected { get { return GetWrappedControl<WrappedHiddenField>(this.Process.FirstRpeaterItem, "hfPaidyPaySelected"); } }
	#endregion

	#region 配送先表示関連

	/// <summary>
	/// 注文者へ配送するか取得
	/// </summary>
	/// <param name="csShipping">配送先情報</param>
	/// <returns>チェック値</returns>
	protected bool GetShipToOwner(CartShipping csShipping)
	{
		return this.Process.GetShipToOwner(csShipping);
	}

	/// <summary>
	/// 注文者へ配送するか取得
	/// </summary>
	/// <param name="csShipping">配送先情報</param>
	/// <returns>チェック値</returns>
	protected bool GetShipToNew(CartShipping csShipping)
	{
		return this.Process.GetShipToNew(csShipping);
	}

	/// <summary>
	/// 注文者から配送するか取得
	/// </summary>
	/// <param name="csShipping">配送先情報</param>
	/// <returns>チェック値</returns>
	protected bool GetSendFromOwner(CartShipping csShipping)
	{
		return this.Process.GetSendFromOwner(csShipping);
	}

	/// <summary>
	/// 配送先情報取得（新規入力の場合のみ値を返す）
	/// </summary>
	/// <param name="coCartObject">カート情報</param>
	/// <param name="strFieldName">フィールド名</param>
	/// <returns>配送先氏名</returns>
	protected string GetShippingValue(CartObject coCartObject, string strFieldName)
	{
		return this.Process.GetShippingValue(coCartObject, strFieldName);
	}
	/// <summary>
	/// 配送先情報取得（新規入力の場合のみ値を返す）
	/// </summary>
	/// <param name="csShipping">配送先情報</param>
	/// <param name="strFieldName">フィールド名</param>
	/// <returns>配送先氏名</returns>
	protected string GetShippingValue(CartShipping csShipping, string strFieldName)
	{
		return this.Process.GetShippingValue(csShipping, strFieldName);
			}

	/// <summary>
	/// 送り主情報取得（新規入力の場合のみ値を返す）
	/// </summary>
	/// <param name="csShipping">配送先情報</param>
	/// <param name="strFieldName">フィールド名</param>
	/// <returns>配送先氏名</returns>
	protected string GetSenderValue(CartShipping csShipping, string strFieldName)
	{
		return this.Process.GetSenderValue(csShipping, strFieldName);
		}

	/// <summary>
	/// 配送希望日取得
	/// </summary>
	/// <param name="csShipping">配送方法情報</param>
	/// <returns>配送希望日</returns>
	protected string GetShippingDate(CartShipping csShipping)
	{
		return this.Process.GetShippingDate(csShipping);
	}

	/// <summary>
	/// 配送希望時間帯取得
	/// </summary>
	/// <param name="csShipping">配送方法情報</param>
	/// <returns>配送希望時間帯</returns>
	protected string GetShippingTime(CartShipping csShipping)
	{
		return this.Process.GetShippingTime(csShipping);
		}


	/// <summary>
	/// カード決済フィールドデフォルト値取得
	/// </summary>
	/// <param name="payment">カート支払方法</param>
	/// <param name="field">フィールド名</param>
	/// <returns>カード決済フィールドデフォルト値</returns>
	protected string GetCreditValue(CartPayment payment, string field)
	{
		return this.Process.GetCreditValue(payment, field);
	}

	/// <summary>
	/// 電算システムコンビニ決済支払先取得
	/// </summary>
	/// <param name="cpPayment">カート支払方法</param>
	/// <returns>電算システムコンビニ決済支払先値</returns>
	protected string GetDskConveniType(CartPayment cpPayment)
	{
		return this.Process.GetDskConveniType(cpPayment);
	}

	/// <summary>
	/// SBPSコンビニ決済支払先取得
	/// </summary>
	/// <param name="cpPayment">カート支払方法</param>
	/// <returns>コンビニタイプ</returns>
	protected string GetSBPSConveniType(CartPayment cpPayment)
	{
		return this.Process.GetSBPSConveniType(cpPayment);
	}

	/// <summary>
	/// ヤマトKWCコンビニ決済支払先取得
	/// </summary>
	/// <param name="cpPayment">カート支払方法</param>
	/// <returns>コンビニタイプ</returns>
	protected string GetYamatoKwcConveniType(CartPayment cpPayment)
	{
		return this.Process.GetYamatoKwcConveniType(cpPayment);
	}

	/// <summary>
	/// Gmo convenience store
	/// </summary>
	/// <param name="payment">Cart payment</param>
	/// <returns>Convenience store type</returns>
	protected string GetGmoConveniType(CartPayment payment)
	{
		return this.Process.GetGmoConveniType(payment);
	}

	/// <summary>
	/// 登録済みクレジットカードを利用するか新規カードか
	/// </summary>
	/// <param name="cpPayment">カート支払方法</param>
	/// <returns>電算システムコンビニ決済支払先値</returns>
	protected bool IsNewCreditCard(CartPayment cpPayment)
	{
		return this.Process.IsNewCreditCard(cpPayment);
	}

	/// <summary>
	/// 楽天IDConnectログイン時の決済種別ソート（楽天ペイを先頭にする)
	/// </summary>
	/// <param name="validPaymentList">有効な決済種別リスト</param>
	protected PaymentModel[] SortPaymentListForRakutenIdConnectLoggedIn(PaymentModel[] validPaymentList)
	{
		return this.Process.SortPaymentListForRakutenIdConnectLoggedIn(validPaymentList);
	}

	/// <summary>
	/// Get rakuten convenience type
	/// </summary>
	/// <param name="payment">Payment</param>
	/// <returns>Convenience store type</returns>
	protected string GetRakutenConvenienceType(CartPayment payment)
	{
		return this.Process.GetRakutenConvenienceType(payment);
	}

	/// <summary>
	/// Get Zeus convenience type
	/// </summary>
	/// <param name="payment">Payment</param>
	/// <returns>Convenience store type</returns>
	protected string GetZeusConvenienceType(CartPayment payment)
	{
		return this.Process.GetZeusConvenienceType(payment);
	}

	/// <summary>
	/// Get Paygent convenience type
	/// </summary>
	/// <param name="payment">Payment</param>
	/// <returns>Convenience store type</returns>
	protected string GetPaygentConvenienceType(CartPayment payment)
	{
		return this.Process.GetPaygentConvenienceType(payment);
	}
	#endregion

	/// <summary>
	/// エラーメッセージ表示（デフォルト注文方法用）
	/// </summary>
	/// <param name="errorMessage">エラーメッセージ</param>
	protected void ShowErrorMessageForUserDefaultOrderSetting(string errorMessage)
	{
		if (errorMessage != null) this.WlOrderErrorMessage.Text = errorMessage;
	}

	/// <summary>表示用番号</summary>
	protected int DispNum { get; set; }
}
