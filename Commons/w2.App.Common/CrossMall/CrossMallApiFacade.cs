/*
=========================================================================================================
  Module      : 注文結果取得する連携Api (CrossMallApiFacade.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.App.Common.CrossMall.Order;

namespace w2.App.Common.CrossMall
{
	/// <summary>
	/// 注文結果取得する連携Apiクラス
	/// </summary>
	public static class CrossMallApiFacade
	{
		/// <summary>
		/// 注文取得する
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>結果セット</returns>
		public static ResultSet<GetOrderResult> GetOrder(string orderId)
		{
			var requst = new GetOrderRequest(orderId);
			var requestUrl = requst.GetRequstUrl();
			
			var responseXml = requst.CreateGetRequst(requestUrl);
			var resultSet = new ResultSet<GetOrderResult>();

			// 請求のシステムエラー以外はそのままCrossMallからもらったレスポンスを表示する
			if (string.IsNullOrEmpty(responseXml) == false)
			{
				resultSet = new GetOrderResponse().GetResultSet<GetOrderResponse>(responseXml);
			}
			else
			{
				resultSet.ResultStatus.Message = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_SYSTEM_ERROR);
			}

			CrossMallApiLogger.WriteShortInfo(orderId, resultSet.ResultStatus.GetSatus, resultSet.ResultStatus.Message, resultSet.TotalResult);
			return resultSet;
		}

		/// <summary>
		/// 取得結果セットで出荷状況確認
		/// </summary>
		/// <param name="resultSet">結果セット</param>
		/// <param name="orderId">注文ID</param>
		/// <returns>出荷完了フラグ</returns>
		public static bool CheckOrderIsShippedByResultSet(ResultSet<GetOrderResult> resultSet, string orderId)
		{
			if ((resultSet.TotalResult == 0) || (resultSet.ResultStatus.IsGetSuccess == false)) return false;
			
			var isShipped = resultSet.Results.Any(r => (r.OrderCode == orderId) && r.IsAlreadyShipped);
			return isShipped;
		}
	}
}
