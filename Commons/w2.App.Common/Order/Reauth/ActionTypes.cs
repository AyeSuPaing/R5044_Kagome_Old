/*
=========================================================================================================
  Module      : アクションタイプ列挙対(ActionTypes.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
namespace w2.App.Common.Order.Reauth
{
	/// <summary>
	/// アクションタイプ
	/// </summary>
	public enum ActionTypes
	{
		/// <summary>与信</summary>
		Reauth,
		/// <summary>キャンセル</summary>
		Cancel,
		/// <summary>請求</summary>
		Billing,
		/// <summary>請求金額減額</summary>
		Reduce,
		/// <summary>注文情報更新</summary>
		Update,
		/// <summary>（コンビニ後払い）請求書再発行</summary>
		Reprint,
		/// <summary>売上確定</summary>
		Sales,
		/// <summary>AmazonPay返金</summary>
		Refund,
		/// <summary>何もしない</summary>
		NoAction,
	}
}
