/*
=========================================================================================================
  Module      : Point History Api Service (CrossPointPointHistoryApiService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;

namespace w2.App.Common.CrossPoint.PointHistory
{
	/// <summary>
	/// ポイント履歴サービスクラス
	/// </summary>
	public class CrossPointPointHistoryApiService : ServiceBase
	{
		/// <summary>取得開始番号インデックス</summary>
		private const int BEGIN_NO_INDEX = 1;
		/// <summary>一度に取得できる最大件数</summary>
		private const int MAX_GET_COUNT = 100;

		/// <summary>
		/// ポイント履歴全件取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>ポイント履歴</returns>
		public List<PointHistoryItem> GetAllPointHistories(string userId)
		{
			var beginNo = BEGIN_NO_INDEX;
			var endNo = MAX_GET_COUNT;
			var allHistoryItems = new List<PointHistoryItem>();

			// 1度に100件しか取得できないため、全件取得するまで繰り返し
			while (true)
			{
				// w2Commerceで付与されたポイント履歴データと結合する際にhistory_noをセットするため、昇順で取得
				var historyItem = GetPointHistory(
					userId,
					beginNo.ToString(),
					endNo.ToString(),
					Constants.CROSS_POINT_PARAM_POINT_HISTORY_INFO_SORT_ASC);

				// API取得エラーの場合はnullを返す
				if (historyItem == null) return null;
				// 総件数が0件の場合は空のリストを返す
				if (historyItem.TotalCount == 0) break;

				allHistoryItems.AddRange(historyItem.List.ToArray());

				// APIから取得してきたアイテムのリストの要素数がAPI側の総件数に達したら、繰り返しを抜ける
				if (allHistoryItems.Count == historyItem.TotalCount) break;

				beginNo += MAX_GET_COUNT;
				endNo += MAX_GET_COUNT;
			}

			return allHistoryItems;
		}

		/// <summary>
		/// ポイント履歴取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="beginNo">取得開始番号</param>
		/// <param name="endNo">取得終了番号</param>
		/// <param name="sort">順序（デフォルト：降順）</param>
		/// <returns>ポイント履歴</returns>
		public PointHistoryApiResult GetPointHistory(
			string userId,
			string beginNo,
			string endNo,
			string sort = Constants.CROSS_POINT_PARAM_POINT_HISTORY_INFO_SORT_DESC)
		{
			var param = new Dictionary<string, string>
			{
				{ Constants.CROSS_POINT_PARAM_POINT_HISTORY_INFO_MEMBER_ID, userId },
				{ Constants.CROSS_POINT_PARAM_POINT_HISTORY_INFO_START_NO, beginNo },
				{ Constants.CROSS_POINT_PARAM_POINT_HISTORY_INFO_END_NO, endNo },
				{ Constants.CROSS_POINT_PARAM_POINT_HISTORY_INFO_SORT, sort },
			};

			var resultSet = GetResultSet<GetPointHistoryResponse>(
				Constants.CROSS_POINT_URL_GET_POINT_HISTORY_LIST,
				param,
				() => new PointHistoryApiRepository().GetPointHistory(param));

			var pointHistory = (resultSet.ResultStatus.IsSuccess)
				? resultSet.Result[0]
				: null;

			return pointHistory;
		}

		/// <summary>
		/// ポイント情報取得 共通処理
		/// </summary>
		/// <param name="name">接続API名</param>
		/// <param name="param">リクエストパラメータ</param>
		/// <param name="function">実行処理</param>
		/// <returns>結果セット</returns>
		private ResultSet<PointHistoryApiResult> GetResultSet<TResponse>(
			string name,
			Dictionary<string, string> param,
			Func<string> function)
			where TResponse : ResponseBase<PointHistoryApiResult>, new()
		{
			WriteStartLog(name, param);

			var resultSet = new TResponse().GetResultSet<TResponse>(function());

			// 異常系エラーの場合、もう一度試行する
			if ((resultSet.ResultStatus.IsSuccess == false)
				&& resultSet.ResultStatus.Error.Any(error => error.IsAbnormalError))
			{
				resultSet = new TResponse().GetResultSet<TResponse>(function());
			}

			resultSet.ResultStatus.RequestParameter = param;
			WriteEndLog(
				name,
				resultSet.ResultStatus,
				resultSet.XmlResponse);

			return resultSet;
		}
	}
}
