/*
=========================================================================================================
  Module      : Order history api service (CrossPointOrderHistoryApiService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w2.Common.Util;

namespace w2.App.Common.CrossPoint.OrderHistory
{
	/// <summary>
	/// 伝票取得サービスクラス
	/// </summary>
	public class CrossPointOrderHistoryApiService : ServiceBase
	{
		/// <summary>
		/// 伝票リスト取得
		/// </summary>
		/// <param name="param">リクエストパラメータ</param>
		/// <returns>伝票モデル</returns>
		public OrderHistoryApiResult[] GetList(Dictionary<string, string> param)
		{
			var result = GetResultList<GetListResponse>(
				Constants.CROSS_POINT_URL_GET_ORDER_HISTORY_LIST,
				param,
				() => new OrderHistoryApiRepository().GetList(param));
			return result;
		}

		/// <summary>
		/// 伝票詳細取得
		/// </summary>
		/// <param name="orderNo">伝票番号</param>
		/// <returns>伝票モデル</returns>
		public OrderHistoryApiResult GetDetail(string orderNo)
		{
			var param = new Dictionary<string, string>
			{
				{ Constants.CROSS_POINT_PARAM_ORDER_HISTORY_ORDER_ID, orderNo },
			};
			var result = GetDetail(param);
			return result;
		}

		/// <summary>
		/// 伝票詳細取得
		/// </summary>
		/// <param name="param">リクエストパラメータ</param>
		/// <returns>伝票モデル</returns>
		public OrderHistoryApiResult GetDetail(Dictionary<string, string> param)
		{
			var result = GetResult<GetDetailResponse>(
				Constants.CROSS_POINT_URL_GET_ORDER_HISTORY,
				param,
				() => new OrderHistoryApiRepository().GetDetail(param));
			return result;
		}

		/// <summary>
		/// 伝票情報取得
		/// </summary>
		/// <param name="name">接続API名</param>
		/// <param name="param">リクエストパラメータ</param>
		/// <param name="function">実行処理</param>
		/// <returns>伝票モデル</returns>
		private OrderHistoryApiResult GetResult<TResponse>(
			string name,
			Dictionary<string, string> param,
			Func<string> function)
			where TResponse : ResponseBase<OrderHistoryApiResult>, new()
		{
			var resultSet = GetResultSet<TResponse>(name, param, function);
			var result = (resultSet.TotalResult > 0)
				? resultSet.Result[0]
				: null;
			return result;
		}

		/// <summary>
		/// 伝票情報取得
		/// </summary>
		/// <param name="name">接続API名</param>
		/// <param name="param">リクエストパラメータ</param>
		/// <param name="function">実行処理</param>
		/// <returns>伝票モデル</returns>
		private OrderHistoryApiResult[] GetResultList<TResponse>(
			string name,
			Dictionary<string, string> param,
			Func<string> function)
			where TResponse : ResponseBase<OrderHistoryApiResult>, new()
		{
			var resultSet = GetResultSet<TResponse>(name, param, function);
			var result = (resultSet.TotalResult > 0)
				? resultSet.Result
				: null;
			return result;
		}

		/// <summary>
		/// 伝票情報取得 共通処理
		/// </summary>
		/// <param name="name">接続API名</param>
		/// <param name="param">リクエストパラメータ</param>
		/// <param name="function">実行処理</param>
		/// <returns>結果セット</returns>
		private ResultSet<OrderHistoryApiResult> GetResultSet<TResponse>(
			string name,
			Dictionary<string, string> param,
			Func<string> function)
			where TResponse : ResponseBase<OrderHistoryApiResult>, new()
		{
			WriteStartLog(name, param);

			var resultSet = new TResponse().GetResultSet<TResponse>(function());

			// 異常系エラーの場合、もう一度試行する
			if ((resultSet.ResultStatus.IsSuccess == false)
				&& resultSet.ResultStatus.Error.Any(error => error.IsAbnormalError))
			{
				resultSet = new TResponse().GetResultSet<TResponse>(function());
			}

			// Error message
			HttpContext.Current.Session[Constants.SESSION_KEY_CROSSPOINT_ERROR_MESSAGE] =
				((resultSet.ResultStatus.IsSuccess == false)
					&& (resultSet.ResultStatus.ErrorCodeList.Contains(Constants.CROSS_POINT_NO_RESULT_ERROR_CODE) == false))
						? MessageManager.GetMessages(Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR)
						: string.Empty;

			resultSet.ResultStatus.RequestParameter = param;
			WriteEndLog(
				name,
				resultSet.ResultStatus,
				resultSet.XmlResponse);

			return resultSet;
		}
	}
}
