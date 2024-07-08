/*
=========================================================================================================
  Module      : ネクストエンジン受注連携APIリクエスト生成クラス (NextEngineRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.Linq;
using w2.Common.Extensions;
using w2.Common.Web;

namespace w2.App.Common.NextEngine
{
	public class NextEngineRequest : IHttpApiRequestData
	{
		/// <summary>
		/// プライベートコンストラクタ
		/// </summary>
		/// <param name="path">エンドポイントパス</param>
		/// <param name="accessToken">アクセストークン</param>
		private NextEngineRequest(string path, string accessToken)
		{
			this.AccessToken = accessToken;
			this.EndpointUri = path;
			this.Parameters = new List<KeyValuePair<string, string>>();
		}

		/// <summary>
		/// アクセストークン取得API用リクエスト生成
		/// </summary>
		/// <param name="uid">ネクストエンジンユーザID</param>
		/// <param name="state">ネクストエンジンステート</param>
		/// <param name="clientId">クライアントID</param>
		/// <param name="clientSecret">クライアントシークレット</param>
		/// <returns>リクエストインスタンス</returns>
		public static NextEngineRequest CreateRequestForAccessTokenApi(
			string uid,
			string state,
			string clientId,
			string clientSecret)
		{
			var request = new NextEngineRequest(NextEngineConstants.PATH_ACCESS_TOKEN_ENDPOINT, string.Empty)
				.AddParam(NextEngineConstants.PARAM_UID, uid)
				.AddParam(NextEngineConstants.PARAM_STATE, state)
				.AddParam(NextEngineConstants.PARAM_CLIENT_ID, clientId)
				.AddParam(NextEngineConstants.PARAM_CLIENT_SECRET, clientSecret);
			return request;
		}

		/// <summary>
		/// ログインユーザ情報取得API用リクエスト生成
		/// </summary>
		/// <param name="accessToken">アクセストークン</param>
		/// <param name="refreshToken">リフレッシュトークン</param>
		/// <returns>リクエストインスタンス</returns>
		public static NextEngineRequest CreateRequestForLoginUserApi(
			string accessToken,
			string refreshToken)
		{
			var request = new NextEngineRequest(NextEngineConstants.PATH_LOGIN_USER_ENDPOINT, accessToken)
				.AddParam(NextEngineConstants.PARAM_ACCESS_TOKEN, accessToken)
				.AddParam(NextEngineConstants.PARAM_REFRESH_TOKEN, refreshToken);
			return request;
		}

		/// <summary>
		/// 受注伝票検索API用リクエスト生成
		/// </summary>
		/// <param name="accessToken">アクセストークン</param>
		/// <param name="refreshToken">リフレッシュトークン</param>
		/// <param name="waitFlg">待機フラグ</param>
		/// <param name="fields">取得フィールド配列</param>
		/// <param name="orderIds">取得受注ID配列</param>
		/// <returns>リクエストインスタンス</returns>
		public static NextEngineRequest CreateRequestForSearchOrderApi(
			string accessToken,
			string refreshToken,
			string waitFlg,
			string[] fields,
			string[] orderIds)
		{
			var request = new NextEngineRequest(NextEngineConstants.PATH_SEARCH_ORDER_ENDPOINT, accessToken)
				.AddParam(NextEngineConstants.PARAM_ACCESS_TOKEN, accessToken)
				.AddParam(NextEngineConstants.PARAM_REFRESH_TOKEN, refreshToken)
				.AddParam(NextEngineConstants.PARAM_WAIT_FLG, waitFlg)
				.AddParam(NextEngineConstants.PARAM_FIELDS, fields.JoinToString(","))
				.AddParam(NextEngineConstants.PARAM_SEARCH_ORDER_ORDER_ID_IN, orderIds.JoinToString(","));
			return request;
		}

		/// <summary>
		/// 受注一括登録パターン取得API用リクエスト生成
		/// </summary>
		/// <param name="accessToken">アクセストークン</param>
		/// <param name="refreshToken">リフレッシュトークン</param>
		/// <param name="waitFlg">待機フラグ</param>
		/// <returns>リクエストインスタンス</returns>
		public static NextEngineRequest CreateRequestForGetUploadPatternApi(
			string accessToken,
			string refreshToken,
			string waitFlg)
		{
			var request = new NextEngineRequest(NextEngineConstants.PATH_GET_UPLOAD_PATTERN_ENDPOINT, accessToken)
				.AddParam(NextEngineConstants.PARAM_ACCESS_TOKEN, accessToken)
				.AddParam(NextEngineConstants.PARAM_REFRESH_TOKEN, refreshToken)
				.AddParam(NextEngineConstants.PARAM_WAIT_FLG, waitFlg);
			return request;
		}

		/// <summary>
		/// 受注伝票アップロードAPI用リクエスト生成
		/// </summary>
		/// <param name="accessToken">アクセストークン</param>
		/// <param name="refreshToken">リフレッシュトークン</param>
		/// <param name="patternId">受注一括登録パターンID</param>
		/// <param name="uploadOrderData">注文情報CSVのファイル内容</param>
		/// <param name="waitFlg">待機フラグ</param>
		/// <returns>リクエストインスタンス</returns>
		public static NextEngineRequest CreateRequestForUploadOrderApi(
			string accessToken,
			string refreshToken,
			string patternId,
			string uploadOrderData,
			string waitFlg)
		{
			var request = new NextEngineRequest(NextEngineConstants.PATH_UPLOAD_ORDER_ENDPOINT, accessToken)
				.AddParam(NextEngineConstants.PARAM_ACCESS_TOKEN, accessToken)
				.AddParam(NextEngineConstants.PARAM_REFRESH_TOKEN, refreshToken)
				.AddParam(NextEngineConstants.PARAM_PATTERN_ID, patternId)
				.AddParam(NextEngineConstants.PARAM_DATA_TYPE, NextEngineConstants.FLG_DATA_TYPE_CSV)
				.AddParam(NextEngineConstants.PARAM_UPLOAD_ORDER_DATA, uploadOrderData)
				.AddParam(NextEngineConstants.PARAM_WAIT_FLG, waitFlg);
			return request;
		}

		/// <summary>
		/// 受注伝票更新API用リクエスト生成
		/// </summary>
		/// <param name="accessToken">アクセストークン</param>
		/// <param name="refreshToken">リフレッシュトークン</param>
		/// <param name="waitFlg">待機フラグ</param>
		/// <param name="receiveOrderId">伝票番号</param>
		/// <param name="lastModifiedDate">最終更新日</param>
		/// <param name="updateData">更新データ</param>
		/// <returns>リクエストインスタンス</returns>
		public static NextEngineRequest CreateRequestForUpdateEngineRequest(
			string accessToken,
			string refreshToken,
			string waitFlg,
			string receiveOrderId,
			string lastModifiedDate,
			string updateData)
		{
			var request = new NextEngineRequest(NextEngineConstants.PATH_UPDATE_ORDER_ENDPOINT, accessToken)
				.AddParam(NextEngineConstants.PARAM_ACCESS_TOKEN, accessToken)
				.AddParam(NextEngineConstants.PARAM_REFRESH_TOKEN, refreshToken)
				.AddParam(NextEngineConstants.PARAM_WAIT_FLG, waitFlg)
				.AddParam(NextEngineConstants.PARAM_SEARCH_ORDER_ORDER_ID, receiveOrderId)
				.AddParam(NextEngineConstants.PARAM_SEARCH_ORDER_ORDER_LAST_MODIFIED_DATE, lastModifiedDate)
				.AddParam(NextEngineConstants.PARAM_SEARCH_ORDER_UPDATE_DATA, updateData)
				.AddParam(NextEngineConstants.PARAM_SEARCH_ORDER_CANCEL_UPDATE_FLG, NextEngineConstants.FLG_SEARCH_ORDER_CANCEL_UPDATE_ON);
			return request;
		}

		/// <summary>
		/// Create request for bulk update engine request
		/// </summary>
		/// <param name="accessToken">Access token</param>
		/// <param name="refreshToken">Refresh token</param>
		/// <param name="waitFlg">Wait flag</param>
		/// <param name="updateData">Update data</param>
		/// <returns>Next engine request</returns>
		public static NextEngineRequest CreateRequestForBulkUpdateEngineRequest(
			string accessToken,
			string refreshToken,
			string waitFlg,
			string updateData)
		{
			var request = new NextEngineRequest(NextEngineConstants.PATH_BULK_UPDATE_ORDER_ENDPOINT, accessToken)
				.AddParam(NextEngineConstants.PARAM_ACCESS_TOKEN, accessToken)
				.AddParam(NextEngineConstants.PARAM_REFRESH_TOKEN, refreshToken)
				.AddParam(NextEngineConstants.PARAM_WAIT_FLG, waitFlg)
				.AddParam(NextEngineConstants.PARAM_DATA_TYPE_FOR_BULK_UPDATE_ORDER, NextEngineConstants.FLG_DATA_TYPE_XML)
				.AddParam(NextEngineConstants.PARAM_SEARCH_ORDER_UPDATE_DATA, updateData);
			return request;
		}

		/// <summary>
		/// クエリパラメータ追加
		/// </summary>
		/// <param name="key">キー</param>
		/// <param name="value">値</param>
		private NextEngineRequest AddParam(string key, string value)
		{
			this.Parameters.Add(new KeyValuePair<string, string>(key, value));
			return this;
		}

		/// <summary>
		/// POST文字列生成
		/// </summary>
		/// <returns>POST文字列</returns>
		public string CreatePostString()
		{
			var result = this.Parameters
				.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value))
				.JoinToString("&");

			return result;
		}

		/// <summary>
		/// POST文字列生成(ログ出力用)
		/// </summary>
		/// <returns>POST文字列</returns>
		public string CreatePostStringForWriteApiLog()
		{
			if (Constants.NE_WRITE_UPLOAD_ORDER_DATA_TO_LOG == false)
			{
				this.Parameters.Remove(this.Parameters.FirstOrDefault(d => d.Key == NextEngineConstants.PARAM_UPLOAD_ORDER_DATA));
			}
			var result = this.Parameters
				.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value))
				.JoinToString("&");

			return result;
		}

		/// <summary>パラメータ</summary>
		private List<KeyValuePair<string, string>> Parameters { get; set; }
		/// <summary>エンドポイントURI</summary>
		public string EndpointUri { get; set; }
		/// <summary>アクセストークン</summary>
		public string AccessToken { get; set; }
	}
}
