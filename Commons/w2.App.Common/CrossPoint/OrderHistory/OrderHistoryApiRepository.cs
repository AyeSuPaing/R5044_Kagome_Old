/*
=========================================================================================================
  Module      : Order History Api Repository (OrderHistoryApiRepository.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;

namespace w2.App.Common.CrossPoint.OrderHistory
{
	/// <summary>
	/// 伝票取得レポジトリクラス
	/// </summary>
	internal class OrderHistoryApiRepository : CrossPointHttpApiReoisitory
	{
		/// <summary>
		/// 伝票リスト取得
		/// </summary>
		/// <param name="param">パラメータ</param>
		/// <returns>レスポンスデータ</returns>
		internal string GetList(IDictionary<string, string> param)
		{
			var response = GetResponse(Constants.CROSS_POINT_URL_GET_ORDER_HISTORY_LIST, param);
			return response;
		}

		/// <summary>
		/// 伝票詳細取得
		/// </summary>
		/// <param name="param">パラメータ</param>
		/// <returns>レスポンスデータ</returns>
		internal string GetDetail(IDictionary<string, string> param)
		{
			var response = GetResponse(Constants.CROSS_POINT_URL_GET_ORDER_HISTORY, param);
			return response;
		}
	}
}
