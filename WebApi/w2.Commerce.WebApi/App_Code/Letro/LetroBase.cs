/*
=========================================================================================================
  Module      : Letroベース (LetroBase.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using w2.App.Common;

namespace Letro
{
	/// <summary>
	/// Letro base
	/// </summary>
	public abstract class LetroBase : BasePage.BasePage
	{
		/// <summary>Http Header Content Type Application Json</summary>
		public const string HTTP_HEADER_CONTENT_TYPE_APPLICATION_JSON = "application/json";
		/// <summary>Status Code: Error Other</summary>
		public const int STATUS_CODE_ERROR_OTHER = 500;

		/// <summary>
		/// リクエスト取得抽象メソッド
		/// </summary>
		/// <returns>レスポンスオブジェクト</returns>
		protected abstract void GetRequest();

		/// <summary>
		/// 有効なパラメーターか
		/// </summary>
		/// <returns>有効かどうか</returns>
		protected abstract bool IsValidParameters();

		/// <summary>
		/// レスポンス取得
		/// </summary>
		/// <returns>レスポンスオブジェクト</returns>
		protected abstract object GetResponseData();

		/// <summary>
		/// レスポンス書き込み
		/// </summary>
		protected void WriteResponse()
		{
			try
			{
				this.CurrentContext.Response.ContentType = HTTP_HEADER_CONTENT_TYPE_APPLICATION_JSON;
				if (Constants.LETRO_OPTION_ENABLED == false)
				{
					WriteErrorResponse(CommerceMessages.ERRMSG_LETRO_API_LETRO_OPTION_DISABLED);
					return;
				}

				if (ValidateRequest() == false) return;

				var response = new LetroBaseResponse();
				var responseData = GetResponseData();
				response.Data = responseData;
				WriteSuccessResponse(response);
			}
			catch
			{
				WriteErrorResponse(CommerceMessages.ERRMSG_LETRO_SYSTEM_ERROR);
			}
		}

		/// <summary>
		/// リクエストの検証
		/// </summary>
		/// <returns>結果</returns>
		protected bool ValidateRequest()
		{
			var ipAddress = GetIpAddress();

			if (IsLocked(ipAddress, AuthKeyType.Letro))
			{
				WriteErrorResponse(CommerceMessages.ERRMSG_LETRO_API_UNAUTHORIZED_IP_ADDRESS);
				return false;
			}

			if (Constants.LETRO_ALLOWED_IP_ADDRESS.Contains(ipAddress) == false)
			{
				WriteErrorResponse(CommerceMessages.ERRMSG_LETRO_API_UNAUTHORIZED_IP_ADDRESS);
				return false;
			}

			if (IsValidAuthorization(this.CurrentContext, Constants.LETRO_API_AUTHENTICATION_KEY) == false)
			{
				UpdateLockPossibleTrialLoginCount(ipAddress, AuthKeyType.Letro);
				WriteErrorResponse(CommerceMessages.ERRMSG_LETRO_INVALID_AUTHENTICATION_KEY);
				return false;
			}

			if (this.CurrentContext.Request.HttpMethod != WebRequestMethods.Http.Get)
			{
				WriteErrorResponse(CommerceMessages.ERRMSG_LETRO_SYSTEM_ERROR);
				return false;
			}

			if (IsValidParameters() == false) return false;

			return true;
		}

		/// <summary>
		/// アクセスユーザのIpv4アドレスを取得
		/// </summary>
		/// <returns>IPアドレス</returns>
		protected string GetIpAddress()
		{
			var result = this.CurrentContext.Request.ServerVariables["REMOTE_ADDR"];
			if (string.IsNullOrEmpty(result))
			{
				result = this.CurrentContext.Request.UserHostAddress;
			}

			if ((result == "::1") || string.IsNullOrEmpty(result))
			{
				result = "127.0.0.1";
			}

			return result ?? string.Empty;
		}

		/// <summary>
		/// エラーレスポンス書き込み
		/// </summary>
		/// <param name="errorCode">エラーコード</param>
		/// <param name="replaces">置換文字列</param>
		protected void WriteErrorResponse(string errorCode, string[] replaces = null)
		{
			this.CurrentContext.Response.StatusCode = STATUS_CODE_ERROR_OTHER;
			var response = new LetroBaseResponse();
			var errorMessage = GetLetroErrorMessage(errorCode);
			if ((replaces != null)
				&& (replaces.Length > 0)
				&& (string.IsNullOrEmpty(errorMessage.Message) == false))
			{
				for (var index = 0; index < replaces.Length; index++)
				{
					errorMessage.Message = errorMessage.Message.Replace(
						string.Format("@@ {0} @@", index + 1),
						replaces[index]);
				}
			}
			response.MessageId = errorMessage.MessageId;
			response.Data = new List<string>
			{
				errorMessage.Message
			};
			response.Message = errorMessage.MessageCode;
			this.CurrentContext.Response.Write(ConvertToJson(response));
		}

		/// <summary>
		/// 成功レスポンス書き込み
		/// </summary>
		/// <param name="response">レスポンス</param>
		protected void WriteSuccessResponse(LetroBaseResponse response)
		{
			var errorMessage = GetLetroErrorMessage(CommerceMessages.ERRMSG_LETRO_API_SUCCESS);
			response.Result = true;
			response.MessageId = errorMessage.MessageId;
			response.Message = errorMessage.MessageCode;
			this.CurrentContext.Response.Write(ConvertToJson(response));
		}

		/// <summary>
		/// Jsonへ変換
		/// </summary>
		/// <param name="data">Data</param>
		/// <returns>Json文字列</returns>
		protected string ConvertToJson(object data)
		{
			var settings = new JsonSerializerSettings
			{
				Formatting = Formatting.Indented,
				NullValueHandling = NullValueHandling.Ignore
			};
			var json = JsonConvert.SerializeObject(data, settings);
			return json;
		}

		/// <summary>
		/// Letroエラーメッセージ取得
		/// </summary>
		/// <param name="messageCode">メッセージコード</param>
		/// <returns>Letroエラーメッセージ</returns>
		protected LetroErrorMessage GetLetroErrorMessage(string messageCode)
		{
			var messages = CommerceMessages.GetMessages(messageCode).Split('|');
			var getItemByIndex = (Func<int, string>)(index =>
				messages.ElementAtOrDefault(index) ?? string.Empty);

			var errorMessage = new LetroErrorMessage(
				getItemByIndex(0),
				string.Format(
					"{0} | {1}",
					getItemByIndex(0),
					getItemByIndex(1)),
				string.IsNullOrEmpty(getItemByIndex(2))
					? messageCode
					: getItemByIndex(2));
			return errorMessage;
		}
		/// <summary>現在のコンテキスト</summary>
		public HttpContext CurrentContext { get; set; }
	}

	/// <summary>
	/// Letroエラーメッセージ
	/// </summary>
	public struct LetroErrorMessage
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="messageId">Message id</param>
		/// <param name="message">Message</param>
		/// <param name="messageCode">Messge code</param>
		public LetroErrorMessage(string messageId, string message, string messageCode)
		{
			this.MessageId = messageId;
			this.Message = message;
			this.MessageCode = messageCode;
		}

		/// <summary>メッセージID</summary>
		public string MessageId;
		/// <summary>メッセージ</summary>
		public string Message;
		/// <summary>メッセージコード</summary>
		public string MessageCode;
	}
}
