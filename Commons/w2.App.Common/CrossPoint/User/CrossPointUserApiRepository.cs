/*
=========================================================================================================
  Module      : CrossPoint API ユーザーレポジトリクラス (CrossPointUserApiRepository.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;

namespace w2.App.Common.CrossPoint.User
{
	/// <summary>
	/// ユーザーレポジトリクラス
	/// </summary>
	internal class CrossPointUserApiRepository : CrossPointHttpApiReoisitory
	{
		/// <summary>
		/// ユーザー情報取得
		/// </summary>
		/// <param name="param">検索条件</param>
		/// <returns>レスポンスデータ</returns>
		internal string Get(IDictionary<string, string> param)
		{
			var response = GetResponse(Constants.CROSS_POINT_URL_GET_USER, param);
			return response;
		}

		/// <summary>
		/// ユーザー情報検索
		/// </summary>
		/// <param name="param">検索条件</param>
		/// <returns>レスポンスデータ</returns>
		internal string Search(IDictionary<string, string> param)
		{
			var response = GetResponse(Constants.CROSS_POINT_URL_GET_USER_LIST, param);
			return response;
		}

		/// <summary>
		/// ユーザー情報登録
		/// </summary>
		/// <param name="param">パラメータ</param>
		/// <returns>レスポンスデータ</returns>
		internal string Insert(IDictionary<string, string> param)
		{
			var response = GetResponse(Constants.CROSS_POINT_URL_INSERT_USER, param);
			return response;
		}

		/// <summary>
		/// ユーザー情報更新
		/// </summary>
		/// <param name="param">パラメータ</param>
		/// <returns>レスポンスデータ</returns>
		internal string Update(IDictionary<string, string> param)
		{
			var response = GetResponse(Constants.CROSS_POINT_URL_UPDATE_USER, param);
			return response;
		}

		/// <summary>
		/// ユーザー情報削除
		/// </summary>
		/// <param name="param">パラメータ</param>
		/// <returns>レスポンスデータ</returns>
		internal string Delete(IDictionary<string, string> param)
		{
			var response = GetResponse(Constants.CROSS_POINT_URL_DELETE_USER, param);
			return response;
		}

		/// <summary>
		/// ポイントカード情報更新
		/// </summary>
		/// <param name="param">パラメータ</param>
		/// <returns>レスポンスデータ</returns>
		internal string Merge(IDictionary<string, string> param)
		{
			var response = GetResponse(Constants.CROSS_POINT_URL_MERGE_USER, param);
			return response;
		}
	}
}
