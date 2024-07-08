/*
=========================================================================================================
  Module      : 再与信アクションインタフェース(IReauthAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
namespace w2.App.Common.Order.Reauth
{
	/// <summary>再与信アクションインタフェース</summary>
	public interface IReauthAction
	{
		/// <returns>アクションタイプ取得</returns>
		ActionTypes GetActionType();
		/// <returns>再与信アクション</returns>
		ReauthActionResult Execute();
		/// <returns>アクション名</returns>
		string GetActionName();
		/// <summary> 再与信情報(決済注文ID、取引ID)を更新 </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="cardTransId">取引ID</param>
		void UpdateReauthInfo(string paymentOrderId, string cardTransId);
	}
}