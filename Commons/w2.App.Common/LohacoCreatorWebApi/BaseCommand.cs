/*
=========================================================================================================
  Module      : ロハコAPIコマンドの基底クラス(BaseCommand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.OpenSsl;
using w2.Common.Logger;

namespace w2.App.Common.LohacoCreatorWebApi
{
	/// <summary>
	/// APIコマンドの基底クラス
	/// </summary>
	/// <typeparam name="TRequest">APIリクエストの基底クラス</typeparam>
	/// <typeparam name="TResponse">APIレスポンスの基底クラス</typeparam>
	public abstract class BaseCommand<TRequest, TResponse>
		where TRequest : BaseRequest
		where TResponse : BaseResponse
	{
		#region +Execute コマンド実行（シリアライズしない）
		/// <summary>
		/// コマンド実行（シリアライズしない）
		/// </summary>
		/// <param name="request">送信リクエスト</param>
		/// <param name="sellerId">スタアアカウント</param>
		/// <param name="privateKey">秘密鍵</param>
		/// <param name="errorResponse">基礎エラーレスポンス情報（基礎エラーレスポンスがあれば場合）</param>
		/// <param name="isWriteDebugLogEnabled">デバッグログ出力かどうか</param>
		/// <param name="isMaskPersonalInfoEnabled">個人情報をマスクかどうか</param>
		/// <returns>レスポンスのXML文字列</returns>
		public string Execute(
			BaseRequest request,
			string sellerId,
			string privateKey,
			out BaseErrorResponse errorResponse,
			bool isWriteDebugLogEnabled = false,
			bool isMaskPersonalInfoEnabled = true)
		{
			// XML文字列をそのまま返却
			return MainProcess(request, sellerId, privateKey, out errorResponse, isWriteDebugLogEnabled, isMaskPersonalInfoEnabled);
		}
		#endregion

		#region +ExecuteWithSerialize コマンド実行
		/// <summary>
		/// コマンド実行
		/// </summary>
		/// <param name="request">送信リクエスト</param>
		/// <param name="sellerId">スタアアカウント</param>
		/// <param name="privateKey">秘密鍵</param>
		/// <param name="errorResponse">基礎エラーレスポンス情報（基礎エラーレスポンスがあれば場合）</param>
		/// <param name="isWriteDebugLogEnabled">デバッグログ出力かどうか</param>
		/// <param name="isMaskPersonalInfoEnabled">個人情報をマスクかどうか</param>
		/// <returns>レスポンスのオブジェクト</returns>
		public TResponse ExecuteWithSerialize(
			BaseRequest request,
			string sellerId,
			string privateKey,
			out BaseErrorResponse errorResponse,
			bool isWriteDebugLogEnabled = false,
			bool isMaskPersonalInfoEnabled = true)
		{
			var responseString = MainProcess(request, sellerId, privateKey, out errorResponse, isWriteDebugLogEnabled, isMaskPersonalInfoEnabled);

			if (responseString != null)
			{
				var responseObject = WebApiHelper.DeserializeXml<TResponse>(responseString);
				if (isWriteDebugLogEnabled)
				{
					FileLogger.WriteDebug(string.Format(
						"API送受信内容は下記となります。{0}送信リクエスト:{0}{1}{0}受信レスポンス:{0}{2}。",
						Environment.NewLine,
						request.WriteDebugLog(isMaskPersonalInfoEnabled),
						responseObject.WriteDebugLog(isMaskPersonalInfoEnabled)));
				}
				return responseObject;
			}
			return null;
		}
		#endregion

		#region +OnExecute メイン処理
		/// <summary>
		/// メイン処理
		/// </summary>
		/// <param name="request">リクエスト</param>
		/// <param name="sellerId">スタアアカウント</param>
		/// <param name="privateKey">秘密鍵</param>
		/// <param name="errorResponse">基礎エラーレスポンス情報（基礎エラーレスポンスがあれば場合）</param>
		/// <param name="isWriteDebugLogEnabled">デバッグログ出力かどうか</param>
		/// <param name="isMaskPersonalInfoEnabled">個人情報をマスクかどうか</param>
		/// <returns>レスポンス</returns>
		public abstract TResponse OnExecute(
			TRequest request,
			string sellerId,
			string privateKey,
			out BaseErrorResponse errorResponse,
			bool isWriteDebugLogEnabled = false,
			bool isMaskPersonalInfoEnabled = true);
		#endregion

		#region #MainProcess メイン処理
		/// <summary>
		/// メイン処理
		/// </summary>
		/// <param name="request">リクエスト</param>
		/// <param name="sellerId">スタアアカウント</param>
		/// <param name="privateKey">秘密鍵</param>
		/// <param name="errorResponse">基礎エラーレスポンス情報（基礎エラーレスポンスがあれば場合）</param>
		/// <param name="isWriteDebugLogEnabled">デバッグログ出力かどうか</param>
		/// <param name="isMaskPersonalInfoEnabled">個人情報をマスクかどうか</param>
		/// <returns>レスポンス</returns>
		protected virtual string MainProcess(
			BaseRequest request,
			string sellerId,
			string privateKey,
			out BaseErrorResponse errorResponse,
			bool isWriteDebugLogEnabled = false,
			bool isMaskPersonalInfoEnabled = true)
		{
			// リクエストを作成
			try
			{
				var req = (HttpWebRequest)WebRequest.Create(request.Uri);
				var contentType =
					(request.ContentType == LohacoConstants.RequestContentType.Xml)
					? LohacoConstants.CONTENT_TYPE_XML
					: (request.ContentType == LohacoConstants.RequestContentType.FormUrlEncoded)
						? LohacoConstants.CONTENT_TYPE_FORM_URL_ENCODED
						: LohacoConstants.CONTENT_TYPE_MULTIPART_FORM_DATA;
				req.ContentType = contentType;
				req.Method = WebRequestMethods.Http.Post;
				req.Accept = (request.Accept == LohacoConstants.ResponseContentType.Xml) ? LohacoConstants.CONTENT_TYPE_XML : LohacoConstants.CONTENT_TYPE_JSON;
				req.Timeout = LohacoConstants.API_REQUEST_TIME_OUT;
				req.Headers.Add(LohacoConstants.REQUEST_HEADER_PROVIDER_ID, sellerId);
				req.Headers.Add(LohacoConstants.REQUEST_HEADER_SIGNATURE, WebApiHelper.CreateRequestSignature(request, sellerId, privateKey));
				var body = new UTF8Encoding().GetBytes(WebApiHelper.SerializeXml(request));
				req.ContentLength = body.Length;
				using (var stream = req.GetRequestStream())
				{
					stream.Write(body, 0, body.Length);

					using (var httpResponse = (HttpWebResponse)req.GetResponse())
					using (var reader = new StreamReader(httpResponse.GetResponseStream()))
					{
						var response = reader.ReadToEnd();
						errorResponse = null;
						return response;
					}
				}
			}
			catch (Exception ex)
			{
				// Lohacoからエラーレスポンス返却場合、エラー内容を取得
				if (ex.GetType() == typeof(WebException))
				{
					using (var exHttpResponse = (HttpWebResponse)((WebException)ex).Response)
					using (var reader = new StreamReader(exHttpResponse.GetResponseStream()))
					{
						var exResponse = reader.ReadToEnd();

						// 共通エラーのフォーマットの場合、ベースエラーレスポンスにデシリアライズ
						if (exResponse.Contains("Error xmlns=\"urn:lohaco:api\""))
						{
							errorResponse = WebApiHelper.DeserializeXml<BaseErrorResponse>(exResponse);
							return null;
						}
						else
						{
							errorResponse = null;
							return exResponse;
						}
					}
				}
				else if ((ex.GetType() == typeof(CryptographicException)) || (ex.GetType() == typeof(Org.BouncyCastle.OpenSsl.PemException)))
				{
					throw new Exception("ロハコ秘密鍵が不正です。", ex);
				}
				else
				{
					FileLogger.WriteError(string.Format(
						"APIリクエスト送信時、エラーが発生しました。送信リクエスト：{0}。",
						request),
						ex);
				}
			}
			errorResponse = null;
			return null;
		}
		#endregion
	}
}
