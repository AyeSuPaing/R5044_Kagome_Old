/*
=========================================================================================================
  Module      : 頒布会商品変更方法 (SubscriptionBoxProductChangeMethod.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.SubscriptionBox
{
	/// <summary>
	/// 頒布会商品変更方法
	/// </summary>
	public enum SubscriptionBoxProductChangeMethod
	{
		/// <summary>プルダウン（ドロップダウン）</summary>
		PullDown,
		/// <summary>モーダル</summary>
		Modal,
	}

	/// <summary>
	/// SubscriptionBoxProductChangeMethod 拡張メソッド郡
	/// </summary>
	public static class SubscriptionBoxProductChangeMethodExtensions
	{
		/// <summary>
		/// 選択方法がモーダルか？
		/// </summary>
		/// <param name="method">頒布会商品変更方法</param>
		/// <returns>モーダル：true</returns>
		public static bool IsModal(this SubscriptionBoxProductChangeMethod method)
		{
			return (method == SubscriptionBoxProductChangeMethod.Modal);
		}

		/// <summary>
		/// 選択方法がプルダウンか？
		/// </summary>
		/// <param name="method">頒布会商品変更方法</param>
		/// <returns>プルダウン：true</returns>
		public static bool IsPullDown(this SubscriptionBoxProductChangeMethod method)
		{
			return (method == SubscriptionBoxProductChangeMethod.PullDown);
		}
	}
}
