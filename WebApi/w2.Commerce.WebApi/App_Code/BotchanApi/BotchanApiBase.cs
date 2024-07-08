/*
=========================================================================================================
  Module      : BOTCHAN_API基底クラス(BotchanApiBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using w2.App.Common;
using w2.App.Common.Botchan;
using w2.Common.Util;

namespace BotchanApi
{
	/// <summary>
	/// BOTCHAN_API基底クラス
	/// </summary>
	public abstract class BotchanApiBase
	{
		/// <summary>Status Code: Success</summary>
		public const int STATUS_CODE_SUCCESS = 200;

		/// <summary>Status Code: Parameter Error</summary>
		public const int STATUS_CODE_PARAMETER_ERROR = 400;

		/// <summary>httpステータス：不許可</summary>
		public const int STATUS_CODE_UNAUTHORIZED = 401;

		/// <summary>Status Code: Error Other</summary>
		public const int STATUS_CODE_ERROR_OTHER = 500;

		/// <summary>
		/// BOTCHAN_APIメンインプロセス
		/// </summary>
		/// <param name="context">コンテキスト</param>
		/// <param name="apiName">API名</param>
		public void BotChanApiProcess(HttpContext context, string apiName)
		{
			// リクエスト取得
			var requestContents = string.Empty;
			try
			{
				// 開始ログ
				WriteStartLog(apiName);
				// リクエスト取得
				requestContents = GetRequest(context);
				// IPアドレス制御
				var errorTypList = AllowedIpAccess(context.Request);
				if (errorTypList.Count > 0)
				{
					WriteErrorResponse(
						context,
						apiName,
						requestContents,
						STATUS_CODE_UNAUTHORIZED,
						errorTypList);

					return;
				}

				// パラメータチェック
				var parametersValidateError = ParametersValidate(requestContents);
				if (parametersValidateError.Count > 0)
				{
					WriteParametersErrorResponse(
						context,
						apiName,
						requestContents,
						parametersValidateError);

					return;
				}

				// 共通チェック
				errorTypList = BotChanUtilityValidate(requestContents, apiName);
				if (errorTypList.Count > 0)
				{
					WriteErrorResponse(
						context,
						apiName,
						requestContents,
						STATUS_CODE_ERROR_OTHER,
						errorTypList);

					return;
				}

				// レスポンス取得
				var memo = string.Empty;
				var resultResponse = GetResponseData(context, requestContents, ref errorTypList, ref memo);
				if (errorTypList.Count > 0)
				{
					WriteErrorResponse(
						context,
						apiName,
						requestContents,
						STATUS_CODE_ERROR_OTHER,
						errorTypList,
						memo);
				}
				else
				{
					WriteResponseResult(
						context,
						apiName,
						requestContents,
						resultResponse);
				}
			}
			catch (Exception exception)
			{
				WriteExceptionResponse(context, apiName, exception, requestContents);
			}
		}

		/// <summary>
		/// レスポンス作成
		/// </summary>
		/// <param name="context">コンテキスト</param>
		/// <param name="request">リクエスト</param>
		/// <param name="data">レスポンス</param>
		/// <param name="apiName">API名</param>
		private static void WriteResponseResult(
			HttpContext context,
			string apiName,
			string request,
			object data)
		{
			var responseJson = JsonConvert.SerializeObject(
				data,
				new JsonSerializerSettings
				{
					Formatting = Formatting.Indented,
					NullValueHandling = NullValueHandling.Ignore
				});
			context.Response.StatusCode = STATUS_CODE_SUCCESS;
			context.Response.ContentType = Constants.BOTCHAN_RESPONSE_CONTENTTYPE;
			context.Response.Write(responseJson);

			WriteEndLog(
				apiName,
				STATUS_CODE_SUCCESS,
				"処理成功",
				request,
				responseJson);
		}

		/// <summary>
		/// エラーレスポンス作成
		/// </summary>
		/// <param name="context">コンテキスト</param>
		/// <param name="apiName">API名</param>
		/// <param name="requestContents">リクエスト</param>
		/// <param name="statusCode">httpステータス</param>
		/// <param name="errorList">エラーリスト</param>
		/// <param name="memo">メモー</param>
		private static void WriteErrorResponse(
			HttpContext context,
			string apiName,
			string requestContents,
			int statusCode,
			List<BotchanMessageManager.MessagesCode> errorList,
			string memo = null)
		{
			var response = new BotchanResponse()
			{
				Result = false,
				MessageId = MessageManager.GetMessages(errorList[0].ToString())
					.Split(Constants.BOTCHAN_ERROR_SPLIT.ToCharArray())[0],
				Message = errorList[0].ToString(),
				Data = string.IsNullOrEmpty(memo)
					? new List<string>()
					: new List<string>
					{
						memo,
					}
			};
			var responseJson = JsonConvert.SerializeObject(
				response,
				new JsonSerializerSettings
				{
					Formatting = Formatting.Indented,
					NullValueHandling = NullValueHandling.Ignore
				});
			context.Response.StatusCode = statusCode;
			context.Response.ContentType = Constants.BOTCHAN_RESPONSE_CONTENTTYPE;
			context.Response.Write(responseJson);

			WriteEndLog(
				apiName,
				statusCode,
				string.Join(Constants.BOTCHAN_ERROR_SPLIT, errorList),
				requestContents,
				responseJson);
		}

		/// <summary>
		/// パラメータエラーレスポンス作成
		/// </summary>
		/// <param name="context">コンテキスト</param>
		/// <param name="apiName">API名</param>
		/// <param name="requestContents">リクエスト</param>
		/// <param name="errorList">エラーリスト</param>
		private static void WriteParametersErrorResponse(
			HttpContext context,
			string apiName,
			string requestContents,
			Validator.ErrorMessageList errorList)
		{
			var dateList = errorList.Select(kvpMessage =>
				string.Format("{0}:{1}", kvpMessage.Key, kvpMessage.Value)).ToList();

			var response = new BotchanResponse()
			{
				Result = false,
				MessageId = "E00-1001",
				Message = "Irregular Parameter",
				Data = dateList
			};
			var responseJson = JsonConvert.SerializeObject(
				response,
				new JsonSerializerSettings
				{
					Formatting = Formatting.Indented,
					NullValueHandling = NullValueHandling.Ignore
				});
			context.Response.StatusCode = STATUS_CODE_PARAMETER_ERROR;
			context.Response.ContentType = Constants.BOTCHAN_RESPONSE_CONTENTTYPE;
			context.Response.Write(responseJson);

			WriteEndLog(
				apiName,
				STATUS_CODE_PARAMETER_ERROR,
				"ParametersError",
				requestContents,
				responseJson);
		}

		/// <summary>
		/// Exceptionレスポンス作成
		/// </summary>
		/// <param name="context">コンテキスト</param>
		/// <param name="apiName">API名</param>
		/// <param name="exception">異常情報</param>
		/// <param name="requestContents">リクエスト</param>
		private static void WriteExceptionResponse(HttpContext context, string apiName, Exception exception, string requestContents = "")
		{
			var messageData = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.SYSTEM_ERROR.ToString());

			var response = new BotchanResponse()
			{
				Result = false,
				MessageId = messageData.Split(Constants.BOTCHAN_ERROR_SPLIT.ToCharArray())[0],
				Message = BotchanMessageManager.MessagesCode.SYSTEM_ERROR.ToString(),
				Data = new List<string>
				{
					messageData,
				},
			};
			var responseJson = JsonConvert.SerializeObject(
				response,
				new JsonSerializerSettings
				{
					Formatting = Formatting.Indented,
					NullValueHandling = NullValueHandling.Ignore
				});
			context.Response.StatusCode = STATUS_CODE_ERROR_OTHER;
			context.Response.ContentType = Constants.BOTCHAN_RESPONSE_CONTENTTYPE;
			context.Response.Write(responseJson);

			WriteEndLog(
				apiName,
				STATUS_CODE_ERROR_OTHER,
				"exception",
				requestContents,
				responseJson);

			//Log出力用文字列整形
			var exceptionMessage = exception.Message + "\r\n" + exception.StackTrace.Replace(") 場所", ")\r\n   場所");

			BotchanApiLogger.Write(exceptionMessage);
		}

		/// <summary>
		/// リクエスト取得
		/// </summary>
		/// <param name="context">コンテキスト</param>
		/// <returns>リクエスト文字列</returns>
		private string GetRequest(HttpContext context)
		{
			context.Response.ContentType = Constants.BOTCHAN_RESPONSE_CONTENTTYPE;
			using (var reader = new StreamReader(context.Request.GetBufferedInputStream()))
			{
				return reader.ReadToEnd();
			}
		}

		/// <summary>
		/// 接続先IP判断
		/// </summary>
		/// <param name="request">リクエスト</param>
		/// <returns>エラーメッセージ</returns>
		private List<BotchanMessageManager.MessagesCode> AllowedIpAccess(HttpRequest request)
		{
			var errorTypList = new List<BotchanMessageManager.MessagesCode>();
			var ipAllowList = Constants.ALLOWED_IP_ADDRESS_FOR_BOTCHAN.Split('|');

			var ipAddress = BotChanUtility.GetIpAddress(request);
			BotchanApiLogger.Write("ipAddress:" + ipAddress);
			BotchanApiLogger.Write("ipAllowList:" + string.Join(",", ipAllowList));
			if (ipAllowList.Contains(ipAddress) == false)
			{
				errorTypList.Add(BotchanMessageManager.MessagesCode.NOT_ALLOWED_IP_ADDRESS);
			}

			return errorTypList;
		}

		/// <summary>
		/// ログ出力
		/// </summary>
		/// <param name="name">接続API名</param>
		protected static void WriteStartLog(string name)
		{
			BotchanApiLogger.Write(string.Format("{0} {1}", "[開始]", name));
			BotchanApiLogger.Write(string.Format("{0} {1}", "[HttpMethod]", "POST"));
		}

		/// <summary>
		/// ログ出力
		/// </summary>
		/// <param name="name">接続API名</param>
		/// <param name="code">ステータスコード</param>
		/// <param name="message">実行結果</param>
		/// <param name="param">リクエストデータ</param>
		/// <param name="response">レスポンスデータ</param>
		protected static void WriteEndLog(
			string name,
			int code,
			string message,
			string param,
			string response)
		{
			if (Constants.BOTCHAN_OUTPUT_REQUEST_PARAM_AMD_RESPONSE_TO_THE_LOG_ENABLED == false)
			{
				param = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.BOTCHAN_OUTPUT_REQUEST_PARAM_AMD_RESPONSE_TO_THE_LOG_OFF.ToString()); ;
				response = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.BOTCHAN_OUTPUT_REQUEST_PARAM_AMD_RESPONSE_TO_THE_LOG_OFF.ToString()); ;
			}

			var output = BotchanApiLogger.GetOutPut(code, message, param, response);
			BotchanApiLogger.Write(string.Format("{0} {1} {2}", "[終了]", name, output));
		}

		/// <summary>
		/// レスポンス取得
		/// </summary>
		/// <param name="context">httpコンテキスト</param>
		/// <param name="requestContents">リクエスト文字列</param>
		/// <param name="errorList">エラーリスト</param>
		/// <param name="memo"></param>
		/// <returns>レスポンス</returns>
		protected abstract object GetResponseData(
			HttpContext context,
			string requestContents,
			ref List<BotchanMessageManager.MessagesCode> errorList,
			ref string memo);

		/// <summary>
		/// BOTCHAN共通チェック
		/// </summary>
		/// <param name="requestContents">リクエスト文字列</param>
		/// <param name="apiName">API名</param>
		/// <returns>エラーリスト</returns>
		protected abstract List<BotchanMessageManager.MessagesCode> BotChanUtilityValidate(string requestContents, string apiName);

		/// <summary>
		/// パラメータバリエーション
		/// </summary>
		/// <param name="requestContents">リクエスト文字列</param>
		/// <returns>エラーリスト</returns>
		protected abstract Validator.ErrorMessageList ParametersValidate(string requestContents);
	}
}