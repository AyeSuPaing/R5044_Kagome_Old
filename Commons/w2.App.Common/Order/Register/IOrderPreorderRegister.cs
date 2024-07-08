/*
=========================================================================================================
  Module      : 仮注文登録処理のインターフェース(IOrderPreorderRegister.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using w2.Domain.UpdateHistory.Helper;

namespace w2.App.Common.Order.Register
{
	/// <summary>
	/// 仮注文登録処理のインターフェース
	/// </summary>
	public interface IOrderPreorderRegister : IService
	{
		/// <summary>
		/// 仮注文登録
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="registGuestUser">ゲストユーザー登録するか</param>
		/// <param name="isUser">会員か否か</param>
		/// <param name="isFirstCart">最初のカートか</param>
		/// <param name="shippingNo">登録した配送先NO</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="execType">実行タイプ</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>成功/失敗</returns>
		bool RegistPreOrder(
			Hashtable order,
			CartObject cart,
			bool registGuestUser,
			bool isUser,
			bool isFirstCart,
			out int shippingNo,
			string lastChanged,
			OrderRegisterBase.ExecTypes execType,
			UpdateHistoryAction updateHistoryAction);
	}
}
