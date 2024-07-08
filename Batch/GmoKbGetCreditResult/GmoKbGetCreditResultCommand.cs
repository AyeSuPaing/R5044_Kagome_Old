/*
=========================================================================================================
  Module      : GmoKb Get Credit Result Command (GmoKbGetCreditResultCommand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Data;
using System.Linq;
using w2.App.Common.Order;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.Order;

namespace w2.Commerce.Batch.GmoKbGetCreditResult
{
	/// <summary>
	/// 取引の与信結果取得
	/// </summary>
	public class GmoKbGetCreditResultCommand
	{
		/// <summary>
		/// 実行
		/// </summary>
		public void Exec()
		{
			var listOrders = GetInReviewGmoOrders();
			if (listOrders != null)
			{
				foreach (var order in listOrders)
				{
					try
					{
						OrderCommon.CheckGmoCredit(order);
					}
					catch (System.Exception ex)
					{
						AppLogger.WriteError(ex);
					}
				}
			}
		}

		/// <summary>
		/// 審査中の注文リスト取得
		/// </summary>
		/// <returns>注文一覧</returns>
		private OrderModel[] GetInReviewGmoOrders()
		{
			var listOrders = new OrderService().GetInReviewGmoOrders();
			return listOrders;
		}
	}
}
