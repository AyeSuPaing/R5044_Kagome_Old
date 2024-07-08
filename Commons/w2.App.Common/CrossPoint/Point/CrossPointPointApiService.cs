/*
=========================================================================================================
  Module      : CrossPoint API ポイントサービスクラス (CrossPointPointApiService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;

namespace w2.App.Common.CrossPoint.Point
{
	/// <summary>
	/// ポイントサービスクラス
	/// </summary>
	public class CrossPointPointApiService : ServiceBase
	{
		/// <summary>
		/// 発行ポイント情報取得
		/// </summary>
		/// <param name="param">リクエストパラメータ</param>
		/// <returns>発行ポイントモデル</returns>
		public PointApiResult Get(Dictionary<string, string> param)
		{
			var result = GetResult<GetResponse>(
				Constants.CROSS_POINT_URL_GET_GRANT_POINT,
				param,
				() => new CrossPointPointApiRepository().Get(param));
			return result;
		}

		/// <summary>
		/// 購買ポイント情報登録
		/// </summary>
		/// <param name="param">リクエストパラメータ</param>
		/// <returns>処理結果</returns>
		/// <remarks>ここで登録したポイントは仮ポイント扱いになる。
		/// 仮ポイントはfixGrantPointAPIを呼び出すことで有効ポイントに確定される。</remarks>
		public ResultStatus Register(Dictionary<string, string> param)
		{
			var result = GetResultSet<RegisterResponse>(
					Constants.CROSS_POINT_URL_UPDATE_PURCHASE_POINT_BY_DETAIL,
					param,
					() => new CrossPointPointApiRepository().Register(param))
				.ResultStatus;
			return result;
		}

		/// <summary>
		/// 購買ポイント情報修正
		/// </summary>
		/// <param name="param">リクエストパラメータ</param>
		/// <returns>処理結果</returns>
		public ResultStatus Modify(Dictionary<string, string> param)
		{
			var result = GetResultSet<ModifyResponse>(
					Constants.CROSS_POINT_URL_CANCEL_PURCHASE_POINT_BY_DETAIL,
					param,
					() => new CrossPointPointApiRepository().Modify(param))
				.ResultStatus;
			return result;
		}

		/// <summary>
		/// 購買ポイント情報取消
		/// </summary>
		/// <param name="param">リクエストパラメータ</param>
		/// <returns>処理結果</returns>
		public ResultStatus Delete(Dictionary<string, string> param)
		{
			var result = GetResultSet<DeleteResponse>(
					Constants.CROSS_POINT_URL_DELETE_PURCHASE_POINT,
					param,
					() => new CrossPointPointApiRepository().Delete(param))
				.ResultStatus;
			return result;
		}

		/// <summary>
		/// 購買ポイント確定
		/// </summary>
		/// <param name="param">リクエストパラメータ</param>
		/// <returns>処理結果</returns>
		public ResultStatus Grant(Dictionary<string, string> param)
		{
			var result = GetResultSet<GrantResponse>(
					Constants.CROSS_POINT_URL_FIX_GRANT_POINT,
					param,
					() => new CrossPointPointApiRepository().Grant(param))
				.ResultStatus;
			return result;
		}

		/// <summary>
		/// Update recovery purchase point
		/// </summary>
		/// <param name="param">Request parameters</param>
		/// <returns>Processing result</returns>
		public ResultStatus Recovery(Dictionary<string, string> param)
		{
			var result = GetResultSet<RegisterResponse>(
					Constants.CROSS_POINT_URL_UPDATE_RECOVERY_PURCHASE_POINT_BY_DETAIL,
					param,
					() => new CrossPointPointApiRepository().Recovery(param))
				.ResultStatus;
			return result;
		}

		/// <summary>
		/// Update point
		/// </summary>
		/// <param name="param">Request parameters</param>
		/// <returns>Processing result</returns>
		public ResultStatus UpdatePoint(Dictionary<string, string> param)
		{
			var result = GetResultSet<UpdatePointResponse>(
					Constants.CROSS_POINT_URL_UPDATE_POINT,
					param,
					() => new CrossPointPointApiRepository().UpdatePoint(param))
				.ResultStatus;
			return result;
		}

		/// <summary>
		/// Get point change reason setting
		/// </summary>
		/// <param name="param">Request parameters</param>
		/// <returns>Processing result</returns>
		public ResultStatus GetPointChangeReasonSetting(Dictionary<string, string> param)
		{
			var result = GetResultSet<PointChangeReasonSettingResponse>(
					Constants.CROSS_POINT_URL_GET_POINT_CHANGE_REASON_SETTING,
					param,
					() => new CrossPointPointApiRepository().GetPointChangeReasonSetting(param))
				.ResultStatus;
			return result;
		}

		/// <summary>
		/// Get point history list
		/// </summary>
		/// <param name="param">Request parameters</param>
		/// <returns>Result set</returns>
		public ResultSet<PointApiResult> GetPointHisList(Dictionary<string, string> param)
		{
			var result = GetResultSet<GetPointHisListResponse>(
				Constants.CROSS_POINT_URL_GET_POINT_HISTORY_LIST,
				param,
				() => new CrossPointPointApiRepository().GetPointHistoryList(param));

			return result;
		}

		/// <summary>
		/// ポイント情報取得
		/// </summary>
		/// <param name="name">接続API名</param>
		/// <param name="param">リクエストパラメータ</param>
		/// <param name="function">実行処理</param>
		/// <returns>ポイントモデル</returns>
		private PointApiResult GetResult<TResponse>(
			string name,
			Dictionary<string, string> param,
			Func<string> function)
			where TResponse : ResponseBase<PointApiResult>, new()
		{
			var resultSet = GetResultSet<TResponse>(name, param, function);
			var result = (resultSet.TotalResult > 0)
				? resultSet.Result[0]
				: null;
			return result;
		}

		/// <summary>
		/// ポイント情報取得 共通処理
		/// </summary>
		/// <param name="name">接続API名</param>
		/// <param name="param">リクエストパラメータ</param>
		/// <param name="function">実行処理</param>
		/// <returns>結果セット</returns>
		private ResultSet<PointApiResult> GetResultSet<TResponse>(
			string name,
			Dictionary<string, string> param,
			Func<string> function)
			where TResponse : ResponseBase<PointApiResult>, new()
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
