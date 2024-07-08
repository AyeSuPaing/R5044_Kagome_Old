/*
=========================================================================================================
  Module      : ネクストエンジン受注連携API (NextEngineApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Text;
using Newtonsoft.Json;
using System.Collections;
using w2.App.Common.NextEngine.Helper;
using w2.App.Common.NextEngine.Response;
using w2.Common.Logger;
using w2.Common.Web;
using w2.Domain.TempDatas;
using System.Linq;

namespace w2.App.Common.NextEngine
{
	public static class NextEngineApi
	{
		/// <summary>
		/// DBにトークンが存在するか（トークン取得DBにない場合は空文字返却）
		/// </summary>
		/// <param name="accessToken">取得したアクセストークン</param>
		/// <param name="refreshToken">取得したリフレッシュトークン</param>
		/// <returns>存在するか</returns>
		public static bool IsExistsToken(out string accessToken, out string refreshToken)
		{
			var accessTokenData = new TempDatasService().Resotre(TempDatasService.TempType.NextEngineToken, NextEngineConstants.PARAM_ACCESS_TOKEN);
			var refreshTokenData = new TempDatasService().Resotre(TempDatasService.TempType.NextEngineToken, NextEngineConstants.PARAM_REFRESH_TOKEN);

			var isExists = ((accessTokenData != null) && (refreshTokenData != null));

			accessToken = isExists ? (string)accessTokenData.TempDataDeserialized : string.Empty;
			refreshToken = isExists ? (string)refreshTokenData.TempDataDeserialized : string.Empty;

			return isExists;
		}

		/// <summary>
		/// アクセストークン取得API呼び出し
		/// </summary>
		/// <param name="uid">ネクストエンジンユーザID</param>
		/// <param name="state">ネクストエンジンステート</param>
		/// <param name="clientId">クライアントID</param>
		/// <param name="clientSecret">クライアントシークレット</param>
		/// <returns>アクセストークン取得APIレスポンス</returns>
		public static NextEngineAccessTokenApiResponse CallAccessTokenApi(
			string uid,
			string state,
			string clientId,
			string clientSecret)
		{
			var request = NextEngineRequest.CreateRequestForAccessTokenApi(
				uid,
				state,
				clientId,
				clientSecret);

			var result = ApiConnector<NextEngineAccessTokenApiResponse>(request);
			return result;
		}

		/// <summary>
		/// ログインユーザー情報取得API呼び出し
		/// </summary>
		/// <param name="accessToken">アクセストークン</param>
		/// <param name="refreshToken">リフレッシュトークン</param>
		/// <returns>ログインユーザ情報取得APIレスポンス</returns>
		public static NextEngineLoginUserApiResponse CallLoginUserApi(
			string accessToken,
			string refreshToken)
		{
			var request = NextEngineRequest.CreateRequestForLoginUserApi(
				accessToken,
				refreshToken);

			var result = ApiConnector<NextEngineLoginUserApiResponse>(request);
			return result;
		}

		/// <summary>
		/// 受注伝票検索API呼び出し
		/// </summary>
		/// <param name="accessToken">アクセストークン</param>
		/// <param name="refreshToken">リフレッシュトークン</param>
		/// <param name="waitFlg">待ちフラグ</param>
		/// <param name="fields">取得カラム指定</param>
		/// <param name="orderIds">取得受注ID配列</param>
		/// <returns>受注伝票検索APIレスポンス</returns>
		public static NextEngineSearchOrderApiResponse CallSearchOrderApi(
			string accessToken,
			string refreshToken,
			string waitFlg,
			string[] fields,
			string[] orderIds)
		{
			var result = new NextEngineSearchOrderApiResponse();

			for (var count = 0; count < orderIds.Length; count += NextEngineConstants.MAX_SEARCHABLE_ORDERS)
			{
				var spritOrderIds = orderIds.Skip(count).Take(NextEngineConstants.MAX_SEARCHABLE_ORDERS).ToArray();
				var request = NextEngineRequest.CreateRequestForSearchOrderApi(
					accessToken,
					refreshToken,
					waitFlg,
					fields,
					spritOrderIds);

				var apiResult = ApiConnector<NextEngineSearchOrderApiResponse>(request);
				switch (apiResult.Result)
				{
					// API結果：成功
					case NextEngineConstants.FLG_RESULT_SUCCESS:
						// 初回API実行時
						if (count == 0)
						{
							result = apiResult;
						}
						// 2回目以降API実行時
						else
						{
							result.Data.ToList().AddRange(apiResult.Data);
							result.Count = (int.Parse(result.Count) + int.Parse(apiResult.Count)).ToString();
						}
						break;

					// API結果：リダイレクトまたは失敗
					case NextEngineConstants.FLG_RESULT_REDIRECT:
					case NextEngineConstants.FLG_RESULT_ERROR:
						return apiResult;

					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			return result;
		}

		/// <summary>
		/// 受注一括登録パターン情報取得API呼び出し
		/// </summary>
		/// <param name="accessToken"></param>
		/// <param name="refreshToken"></param>
		/// <param name="waitFlg">待ちフラグ</param>
		/// <returns>受注一括登録パターン情報取得APIレスポンス</returns>
		public static NextEngineGetUploadPatternApiResponse CallGetUploadPatternApi(
			string accessToken,
			string refreshToken,
			string waitFlg)
		{
			var request = NextEngineRequest.CreateRequestForGetUploadPatternApi(
				accessToken,
				refreshToken,
				waitFlg);

			var result = ApiConnector<NextEngineGetUploadPatternApiResponse>(request);
			return result;
		}

		/// <summary>
		/// 受注伝票アップロードAPI呼び出し
		/// </summary>
		/// <param name="accessToken">アクセストークン</param>
		/// <param name="refreshToken">リフレッシュトークン</param>
		/// <param name="patternId">受注一括登録パターンID</param>
		/// <param name="uploadOrderData">CSVのファイル内容</param>
		/// <param name="waitFlg">待機フラグ</param>
		/// <returns>リクエストインスタンス</returns>
		public static NextEngineUploadOrderApiResponse CallUploadOrderApi(
			string accessToken,
			string refreshToken,
			string patternId,
			string uploadOrderData,
			string waitFlg)
		{
			var request = NextEngineRequest.CreateRequestForUploadOrderApi(
				accessToken,
				refreshToken,
				patternId,
				uploadOrderData,
				waitFlg);

			var result = ApiConnector<NextEngineUploadOrderApiResponse>(request);
			return result;
		}

		/// <summary>
		/// 受注伝票更新(キャンセル処理)
		/// </summary>
		/// <param name="accessToken">アクセストークン</param>
		/// <param name="refreshToken">リフレッシュトークン</param>
		/// <param name="waitFlg">待機フラグ</param>
		/// <param name="receiveOrderId">伝票番号</param>
		/// <param name="lastModifiedDate">最終更新日</param>
		/// <param name="updateOrderData">更新するデータ</param>
		/// <returns>リクエストインスタンス</returns>
		public static NextEngineUpdateOrderApiResponse CallUpdateOrderApiForCancell(
			string accessToken,
			string refreshToken,
			string waitFlg,
			string receiveOrderId,
			string lastModifiedDate,
			string updateOrderData)
		{
			var request = NextEngineRequest.CreateRequestForUpdateEngineRequest(
				accessToken,
				refreshToken,
				waitFlg,
				receiveOrderId,
				lastModifiedDate,
				updateOrderData);

			var result = ApiConnector<NextEngineUpdateOrderApiResponse>(request);
			return result;
		}

		/// <summary>
		/// Call bulk update order api
		/// </summary>
		/// <param name="accessToken">Access token</param>
		/// <param name="refreshToken">Refresh token</param>
		/// <param name="waitFlg">Wait flag</param>
		/// <param name="updateOrderData">Update order data</param>
		/// <returns>Next Engine update order api response</returns>
		public static NextEngineBulkUpdateOrderApiResponse CallBulkUpdateOrderApi(
			string accessToken,
			string refreshToken,
			string waitFlg,
			string updateOrderData)
		{
			var request = NextEngineRequest.CreateRequestForBulkUpdateEngineRequest(
				accessToken,
				refreshToken,
				waitFlg,
				updateOrderData);

			var result = ApiConnector<NextEngineBulkUpdateOrderApiResponse>(request);
			return result;
		}

		/// <summary>
		/// APIコネクタ
		/// </summary>
		/// <typeparam name="T">レスポンス型</typeparam>
		/// <param name="request">リクエスト</param>
		/// <returns>レスポンス</returns>
		private static T ApiConnector<T>(NextEngineRequest request) where T : NextEngineApiResponseBase, new()
		{
			var oldAccessToken = request.AccessToken;
			using (var conn = new HttpApiConnector())
			{
				T result;
				try
				{
					var response = conn.Do(request.EndpointUri, Encoding.UTF8, request);
					result = JsonConvert.DeserializeObject<T>(response);
					var castedToBaseClass = result;
					if (castedToBaseClass != null)
					{
						castedToBaseClass.UpdateAccessToken(oldAccessToken);
					}
					NextEngineApiLogger.WriteApiLog(request, response);
				}
				catch (Exception ex)
				{
					FileLogger.WriteError(ex);
					NextEngineApiLogger.WriteApiLog(ex);

					result = new T
					{
						Result = NextEngineConstants.FLG_RESULT_ERROR
					};
				}
				
				return result;
			}
		}

		/// <summary>
		/// メール送信
		/// </summary>
		/// <param name="ht">送信情報</param>
		public static void MailSend(Hashtable ht)
		{
			try
			{
				using (var sender = new MailSendUtility(
							Constants.CONST_DEFAULT_SHOP_ID,
							Constants.CONST_MAIL_ID_NO_EXIST_TMP_ORDER,
							"",
							ht))
				{
					sender.SendMail();
				}
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
			}
		}

		/// <summary>
		/// Send failure cancel order mail
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <param name="userId">User id</param>
		public static void SendFailureCancelOrderMail(string orderId, string userId)
		{
			try
			{
				var mailMessage = new StringBuilder()
					.AppendLine(NextEngineConstants.ERROR_MESSAGE_FAIL_TMP_CNSL)
					.AppendLine(NextEngineConstants.ERROR_MESSAGE_FAIL_TMP_CNSL_FOR_ADMIN)
					.AppendLine(string.Format(
						NextEngineConstants.ERROR_MESSAGE_FORMAT_TARGET,
						orderId,
						userId));
				var replace = new Hashtable
				{
					{ "message", mailMessage }
				};
				using (var sender = new MailSendUtility(
					Constants.CONST_DEFAULT_SHOP_ID,
					Constants.CONST_MAIL_ID_NEXT_ENGINE_ORDER_CANCEL_FAIL_FOR_MANAGER,
					string.Empty,
					replace,
					true,
					Constants.MailSendMethod.Auto))
				{
					sender.SendMail();
				}
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
			}
		}
	}
}
