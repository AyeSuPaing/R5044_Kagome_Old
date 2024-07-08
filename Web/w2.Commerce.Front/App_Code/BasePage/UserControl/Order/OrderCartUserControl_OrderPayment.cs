/*
=========================================================================================================
  Module      : 注文カートユーザーコントロール（決済系）(OrderCartUserControl_OrderPayment.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using w2.Domain.Payment;

/// <summary>
/// 注文カートユーザーコントロール（決済系）
/// </summary>
public partial class OrderCartUserControl
{
	#region 決済情報入力画面系処理

	/// <summary>
	/// 決済情報入力画面初期処理
	/// </summary>
	protected void InitComponentsOrderPayment()
	{
		this.Process.InitComponentsOrderPayment();
	}

	/// <summary>
	/// カート決済情報作成
	/// </summary>
	protected void CreateCartPayment()
	{
		this.Process.CreateCartPayment();
	}

	/// <summary>
	/// クレジットカード番号表示設定
	/// </summary>
	protected void AdjustCreditCardNo()
	{
		this.Process.AdjustCreditCardNo();
	}

	/// <summary>
	/// お支払い情報をカート情報へセット
	/// </summary>
	/// <returns>エラー有り：true、エラーなし：false</returns>
	protected bool SetPayment()
	{
		return this.Process.SetPayment();
	}

	/// <summary>
	/// カート番号「１」同じお支払いを指定するチェックボックスクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbUseSamePaymentAddrAsCart1_OnCheckedChanged(object sender, System.EventArgs e)
	{
		this.Process.cbUseSamePaymentAddrAsCart1_OnCheckedChanged(sender, e);
	}

	/// <summary>
	/// 決済種別変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rbgPayment_OnCheckedChanged(object sender, System.EventArgs e)
	{
		this.Process.rbgPayment_OnCheckedChanged(sender, e);
	}

	/// <summary>
	/// クレジットカード選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlUserCreditCard_OnSelectedIndexChanged(object sender, System.EventArgs e)
	{
		this.Process.ddlUserCreditCard_OnSelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// クレジットカードを登録するチェックボックスをクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbRegistCreditCard_OnCheckedChanged(object sender, System.EventArgs e)
	{
		this.Process.cbRegistCreditCard_OnCheckedChanged(sender, e);
	}

	/// <summary>
	/// クレジットトークン向け  カード情報取得JSスクリプト作成
	/// </summary>
	/// <param name="riParent">決済リピータアイテム</param>
	/// <returns>スクリプト</returns>
	protected string CreateGetCardInfoJsScriptForCreditToken(RepeaterItem riParent)
	{
		var script = this.Process.CreateGetCardInfoJsScriptForCreditToken(riParent);
		return script;
	}

	/// <summary>
	/// クレジットトークン向け  カード情報取得JSスクリプト作成
	/// </summary>
	/// <param name="riCart">カートリピータアイテム</param>
	/// <param name="riParent">決済リピータアイテム</param>
	/// <returns>スクリプト</returns>
	protected string CreateGetCardInfoJsScriptForCreditTokenForCart(RepeaterItem riCart, RepeaterItem riParent)
	{
		return this.Process.CreateGetCardInfoJsScriptForCreditTokenForCart(riCart, riParent);
	}

	/// <summary>
	/// 入力フォームからトークン情報削除（再入力ボタンで利用）
	/// </summary>
	/// <param name="riPayment">決済リピータアイテム</param>
	protected void ResetCreditTokenInfoFromForm(RepeaterItem riPayment)
	{
		this.Process.ResetCreditTokenInfoFromForm(riPayment);
	}

	/// <summary>
	/// クレジットトークン取得＆フォームセットJSスクリプト作成
	/// </summary>
	/// <returns>JSスクリプト</returns>
	protected string CreateGetCreditTokenAndSetToFormJsScript()
	{
		return this.Process.CreateGetCreditTokenAndSetToFormJsScript();
	}

	/// <summary>
	/// クレジットトークン向け フォームマスキングJSスクリプト作成
	/// </summary>
	/// <returns>JSスクリプト</returns>
	protected string CreateMaskFormsForCreditTokenJsScript()
	{
		return this.Process.CreateMaskFormsForCreditTokenJsScript();
	}

	/// <summary>
	/// ペイパル認証完了（決済入力画面向け）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbPayPalAuthComplete_Click(object sender, EventArgs e)
	{
		this.Process.lbPayPalAuthComplete_Click(sender, e);
	}

	/// <summary>
	/// 選択した領収書希望フラグ値の取得
	/// </summary>
	/// <param name="cartIndex">カート番号</param>
	/// <param name="isSameReceiptCart1">カート１と同じにするか</param>
	/// <param name="receiptFlg">領収書希望フラグ</param>
	/// <returns>適当な領収書希望値</returns>
	protected string GetSelectedValueOfReceiptFlg(int cartIndex, bool isSameReceiptCart1, string receiptFlg)
	{
		return this.Process.GetSelectedValueOfReceiptFlg(cartIndex, isSameReceiptCart1, receiptFlg);
	}

	/// <summary>
	/// 領収書希望有無ドロップダウンリスト変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlReceiptFlg_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		this.Process.ddlReceiptFlg_OnSelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// 領収書情報をカート情報へセット
	/// </summary>
	/// <returns>エラー有り：true、エラーなし：false</returns>
	protected bool SetReceipt()
	{
		return this.Process.SetReceipt();
	}

	#endregion

	#region プロパティ
	/// <summary>カードリスト</summary>
	protected ListItemCollection CreditCardList
	{
		get { return this.Process.CreditCardList; }
	}
	/// <summary>カード会社名</summary>
	protected ListItemCollection CreditCompanyList
	{
		get { return this.Process.CreditCompanyList; }
		set { this.Process.CreditCompanyList = value; }
	}
	/// <summary>有効期限(月)</summary>
	protected ListItemCollection CreditExpireMonth
	{
		get { return this.Process.CreditExpireMonth; }
		set { this.Process.CreditExpireMonth = value; }
	}
	/// <summary>有効期限(年)</summary>
	protected ListItemCollection CreditExpireYear
	{
		get { return this.Process.CreditExpireYear; }
		set { this.Process.CreditExpireYear = value; }
	}
	/// <summary>支払回数</summary>
	protected ListItemCollection CreditInstallmentsList
	{
		get { return this.Process.CreditInstallmentsList; }
		set { this.Process.CreditInstallmentsList = value; }
	}
	///// <summary>コンビニ支払先</summary>
	protected ListItemCollection CvsTypeList
	{
		get { return this.Process.CvsTypeList; }
	}
	/// <summary>有効決済種別</summary>
	protected List<PaymentModel[]> ValidPayments
	{
		get { return this.Process.ValidPayments; }
		set { this.Process.ValidPayments = value; }
	}
	/// <summary>領収書希望のプルダウン選択肢リスト</summary>
	protected List<ListItemCollection> DdlReceiptFlgListItems
	{
		get { return this.Process.DdlReceiptFlgListItems; }
	}
	/// <summary>NewebPay Installments List</summary>
	protected ListItemCollection NewebPayInstallmentsList
	{
		get { return this.Process.NewebPayInstallmentsList; }
		set { this.Process.NewebPayInstallmentsList = value; }
	}
	# endregion
}