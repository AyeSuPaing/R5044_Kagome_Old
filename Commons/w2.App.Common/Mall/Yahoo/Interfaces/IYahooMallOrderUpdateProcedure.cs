/*
=========================================================================================================
  Module      : YAHOO API Yahooモール注文更新進行インターフェース (IYahooMallOrderUpdateProcedure.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Mall.Yahoo.YahooMallOrders;

namespace w2.App.Common.Mall.Yahoo.Interfaces
{
	/// <summary>
	/// YAHOO API Yahooモール注文更新進行インターフェース
	/// </summary>
	public interface IYahooMallOrderUpdateProcedure
	{
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="order">注文</param>
		/// <param name="tempUserId">一時ユーザーのユーザーID</param>
		/// <param name="mallId">モールID</param>
		/// <param name="orderId">ｗ２モール注文ID</param>
		/// <returns>更新結果</returns>
		bool Update(YahooMallOrder order, string tempUserId, string mallId, string orderId);
		
		/// <summary>
		/// 失敗したことを記録する
		/// </summary>
		/// <param name="orderId">注文ID</param>
		void RecordFailedResult(string orderId);
	}
}
