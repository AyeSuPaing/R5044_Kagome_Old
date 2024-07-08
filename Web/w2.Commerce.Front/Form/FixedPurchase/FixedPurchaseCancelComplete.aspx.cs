/*
=========================================================================================================
  Module      : 定期購入情報解約完了画面(FixedPurchaseCancelComplete.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;

public partial class Form_FixedPurchase_FixedPurchaseCancelComplete : FixedPurchasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		// ログインチェック（ログイン後は定期購入詳細から）
		CheckLoggedIn(PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(this.RequestFixedPurchaseId));

		// HTTPS通信チェック（HTTPのとき、トップ画面へ）
		CheckHttps();
	}

	/// <summary>
	/// 定期購入情報詳細ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbFixedPurchaseDetail_Click(object sender, EventArgs e)
	{
		// 詳細画面へ遷移
		Response.Redirect(PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(this.RequestFixedPurchaseId));
	}
}