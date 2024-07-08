/*
=========================================================================================================
  Module      : Point api repository (CrossPointPointApiRepository.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;

namespace w2.App.Common.CrossPoint.Point
{
	/// <summary>
	/// ポイントレポジトリクラス
	/// </summary>
	internal class CrossPointPointApiRepository : CrossPointHttpApiReoisitory
	{
		/// <summary>
		/// 発行ポイント情報取得
		/// </summary>
		/// <param name="param">パラメータ</param>
		/// <returns>レスポンスデータ</returns>
		internal string Get(IDictionary<string, string> param)
		{
			var response = GetResponse(Constants.CROSS_POINT_URL_GET_GRANT_POINT, param);
			return response;
		}

		/// <summary>
		/// 購買ポイント更新
		/// </summary>
		/// <param name="param">パラメータ</param>
		/// <returns>レスポンスデータ</returns>
		internal string Register(IDictionary<string, string> param)
		{
			var response = GetResponse(Constants.CROSS_POINT_URL_UPDATE_PURCHASE_POINT_BY_DETAIL, param);
			return response;
		}

		/// <summary>
		/// 購買ポイント一部キャンセル
		/// </summary>
		/// <param name="param">パラメータ</param>
		/// <returns>レスポンスデータ</returns>
		internal string Modify(IDictionary<string, string> param)
		{
			var response = GetResponse(Constants.CROSS_POINT_URL_CANCEL_PURCHASE_POINT_BY_DETAIL, param);
			return response;
		}

		/// <summary>
		/// 購買ポイント取消
		/// </summary>
		/// <param name="param">パラメータ</param>
		/// <returns>レスポンスデータ</returns>
		internal string Delete(IDictionary<string, string> param)
		{
			var response = GetResponse(Constants.CROSS_POINT_URL_DELETE_PURCHASE_POINT, param);
			return response;
		}

		/// <summary>
		/// 購買ポイント確定
		/// </summary>
		/// <param name="param">パラメータ</param>
		/// <returns>レスポンスデータ</returns>
		internal string Grant(IDictionary<string, string> param)
		{
			var response = GetResponse(Constants.CROSS_POINT_URL_FIX_GRANT_POINT, param);
			return response;
		}

		/// <summary>
		/// Update recovery purchase point
		/// </summary>
		/// <param name="param">Parameters</param>
		/// <returns>Response data</returns>
		internal string Recovery(IDictionary<string, string> param)
		{
			var response = GetResponse(Constants.CROSS_POINT_URL_UPDATE_RECOVERY_PURCHASE_POINT_BY_DETAIL, param);
			return response;
		}

		/// <summary>
		/// Update point
		/// </summary>
		/// <param name="param">Parameters</param>
		/// <returns>Response data</returns>
		internal string UpdatePoint(IDictionary<string, string> param)
		{
			var response = GetResponse(Constants.CROSS_POINT_URL_UPDATE_POINT, param);
			return response;
		}

		/// <summary>
		/// Get point change reason setting
		/// </summary>
		/// <param name="param">Parameters</param>
		/// <returns>Point change reason setting</returns>
		internal string GetPointChangeReasonSetting(IDictionary<string, string> param)
		{
			var response = GetResponse(Constants.CROSS_POINT_URL_GET_POINT_CHANGE_REASON_SETTING, param);
			return response;
		}

		/// <summary>
		/// Get point history list
		/// </summary>
		/// <param name="param">Request parameters</param>
		/// <returns>Point history list</returns>
		internal string GetPointHistoryList(IDictionary<string, string> param)
		{
			var response = GetResponse(Constants.CROSS_POINT_URL_GET_POINT_HISTORY_LIST, param);
			return response;
		}
	}
}
