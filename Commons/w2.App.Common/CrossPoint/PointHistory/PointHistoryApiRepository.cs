/*
=========================================================================================================
  Module      : Point History Api Repository (PointHistoryApiRepository.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;

namespace w2.App.Common.CrossPoint.PointHistory
{
	/// <summary>
	/// ポイント履歴レポジトリクラス
	/// </summary>
	internal class PointHistoryApiRepository : CrossPointHttpApiReoisitory
	{
		/// <summary>
		/// ポイント履歴情報取得
		/// </summary>
		/// <param name="param">取得条件</param>
		/// <returns>レスポンスデータ</returns>
		internal string GetPointHistory(IDictionary<string, string> param)
		{
			var response = GetResponse(Constants.CROSS_POINT_URL_GET_POINT_HISTORY_LIST, param);
			return response;
		}
	}
}
