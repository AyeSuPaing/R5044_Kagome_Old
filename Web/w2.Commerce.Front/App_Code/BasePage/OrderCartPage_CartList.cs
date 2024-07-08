/*
=========================================================================================================
  Module      : 注文カート系画面入力基底ページ(OrderCartPage_CartList.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/

///*********************************************************************************************
/// <summary>
/// 注文カート系画面入力基底ページ
/// </summary>
///*********************************************************************************************
public partial class OrderCartPage : OrderPageDisp
{
	#region カート一覧画面系処理
	/// <summary>
	/// 入力チェック（カート一覧画面）
	/// </summary>
	protected void CheckInputDataForCartList()
	{
		this.Process.CheckInputDataForCartList();
	}

	/// <summary>
	/// 数量の入力チェック
	/// </summary>
	/// <param name="quantity">数量（入力値）</param>
	/// <returns>エラーメッセージ</returns>
	public string CheckInputQuantity(string quantity)
	{
		return this.Process.CheckInputQuantity(quantity);
	}

	/// <summary>
	/// 入力情報をオブジェクトへセット（カート一覧画面）
	/// </summary>
	protected void SetInputDataForCartList()
	{
		this.Process.SetInputDataForCartList();
	}

	/// <summary>
	/// 入力チェック（ポイント）
	/// </summary>
	protected void CheckInputDataForPoint()
	{
		this.Process.CheckInputDataForPoint();
	}

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

	/// <summary>Is Use Atone Payment And Not Yet Card Tran Id</summary>
	protected bool IsUseAtonePaymentAndNotYetCardTranId
	{
		get
		{
			return this.Process.IsUseAtonePaymentAndNotYetCardTranId;
		}
	}
	/// <summary>Is Use Aftee Payment And Not Yet Card Tran Id</summary>
	protected bool IsUseAfteePaymentAndNotYetCardTranId
	{
		get
		{
			return this.Process.IsUseAfteePaymentAndNotYetCardTranId;
		}
	}
	#endregion
}
