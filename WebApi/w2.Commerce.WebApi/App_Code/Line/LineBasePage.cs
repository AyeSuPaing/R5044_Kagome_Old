/*
=========================================================================================================
  Module      : Line Base Page (LineBasePage.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Globalization;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using w2.App.Common.Line.SendLineMessage;

/// <summary>
/// Line base page
/// </summary>
public abstract class LineBasePage : BasePage.BasePage
{
	/// <summary>format iso 8601</summary>
	private readonly string[] m_FormatIso8601 = 
	{
		"yyyyMMdd",
		"yyyyMMddHHmmss",
		"yyyyMMddHHmm",
		"yyyyMMddHH",
		"yyyyMMddTHHmmssZ",
		"yyyyMMddTHHmmZ",
		"yyyyMMddTHHZ",
	};
	/// <summary>Http Header Content Type Application Json</summary>
	public const string HTTP_HEADER_CONTENT_TYPE_APPLICATION_JSON = "application/json";
	/// <summary>Status Code: Success</summary>
	public const int STATUS_CODE_SUCCESS = 200;
	/// <summary>Status Code: Data Not Exist</summary>
	public const int STATUS_CODE_DATA_NOT_EXIST = 300;
	/// <summary>Status Code: Any User</summary>
	public const int STATUS_CODE_ANY_USER = 301;
	/// <summary>Status Code: Parameter Error</summary>
	public const int STATUS_CODE_PARAMETER_ERROR = 400;
	/// <summary>Status Code: Authentication Key Error</summary>
	public const int STATUS_CODE_AUTHENTICATION_KEY_ERROR = 403;
	/// <summary>Status Code: Error Other</summary>
	public const int STATUS_CODE_ERROR_OTHER = 500;
	/// <summary>Error Message Link Line Empty</summary>
	public readonly string ERROR_MESSAGE_LINK_LINE_EMPTY = "該当店舗LINE連携できません";

	/// <summary>
	/// Is valid Http method
	/// </summary>
	/// <param name="context">コンテキスト</param>
	/// <returns>True if method is Get, otherwise False</returns>
	protected bool IsValidHttpMethod(HttpContext context)
	{
		return (context.Request.HttpMethod == WebRequestMethods.Http.Get);
	}

	/// <summary>
	/// アクセスユーザのIpv4アドレスを取得
	/// </summary>
	/// <param name="request">Request</param>
	/// <returns>IP address</returns>
	protected string GetIpAddress(HttpRequest request)
	{
		var result = request.ServerVariables["REMOTE_ADDR"];

		if (string.IsNullOrEmpty(result))
		{
			result = request.ServerVariables["REMOTE_ADDR"];
		}

		if (string.IsNullOrEmpty(result))
		{
			result = request.UserHostAddress;
		}

		if ((result == "::1") || string.IsNullOrEmpty(result))
		{
			result = "127.0.0.1";
		}

		return result;
	}

	/// <summary>
	/// Is valid authorization
	/// </summary>
	/// <returns>True if parameter is valid, otherwise false</returns>
	protected abstract bool IsValidParameters();

	/// <summary>
	/// Get error response
	/// </summary>
	/// <param name="parameters">A parameters</param>
	/// <returns>An error response object</returns>
	protected abstract object GetErrorResponse(params object[] parameters);

	/// <summary>
	/// Get response data
	/// </summary>
	/// <returns>A response object</returns>
	protected abstract object GetResponseData();

	/// <summary>
	/// リクエスト取得抽象メソッド
	/// </summary>
	/// <returns>A response object</returns>
	protected abstract void GetRequest(HttpContext context);

	/// <summary>
	/// ソーシャルプラスオプションチェック抽象メソッド
	/// </summary>
	/// <returns>チェック結果</returns>
	protected abstract bool IsErrorSocialProviderIdLine();

	/// <summary>
	/// Validate request
	/// </summary>
	/// <param name="context">コンテキスト</param>
	/// <returns>True if request is valid, otherwise false</returns>
	public bool ValidateRequest(HttpContext context)
	{
		var ipAddress = GetIpAddress(context.Request);

		if (IsLocked(ipAddress, AuthKeyType.Line))
		{
			Write403ErrorResponse(context, "誤った認証キーで一定回数以上接続試行が行われました");
			return false;
		}

		if (IsValidAuthorization(context, Constants.SECRET_KEY_API_LINE) == false)
		{
			UpdateLockPossibleTrialLoginCount(ipAddress, AuthKeyType.Line);
			Write403ErrorResponse(context, "認証キーエラー");
			return false;
		}

		if (IsValidHttpMethod(context) == false)
		{
			Write400ErrorResponse(context);
			return false;
		}

		if (IsValidParameters() == false)
		{
			Write400ErrorResponse(context);
			return false;
		}

		return true;
	}

	/// <summary>
	/// Write 300 error response
	/// </summary>
	/// <param name="context">コンテキスト</param>
	protected void Write300ErrorResponse(HttpContext context)
	{
		var errorResponse = GetErrorResponse(STATUS_CODE_DATA_NOT_EXIST, "該当データはありません");
		WriteErrorResponse(context, STATUS_CODE_DATA_NOT_EXIST, errorResponse);
	}

	/// <summary>
	/// Write 301 error response
	/// </summary>
	/// <param name="context">コンテキスト</param>
	protected void Write301ErrorResponse(HttpContext context)
	{
		var errorResponse = GetErrorResponse(STATUS_CODE_ANY_USER, "任意の顧客情報となります");
		WriteErrorResponse(context, STATUS_CODE_ANY_USER, errorResponse);
	}

	/// <summary>
	/// Write 400 error response
	/// </summary>
	/// <param name="context">コンテキスト</param>
	protected void Write400ErrorResponse(HttpContext context)
	{
		var errorResponse = GetErrorResponse(STATUS_CODE_PARAMETER_ERROR, "パラメータエラー");
		WriteErrorResponse(context, STATUS_CODE_PARAMETER_ERROR, errorResponse);
	}

	/// <summary>
	/// Write 403 error response
	/// </summary>
	/// <param name="context">コンテキスト</param>
	/// <param name="errorMessage">エラー理由</param>
	protected void Write403ErrorResponse(HttpContext context, string errorMessage)
	{
		var errorResponse = GetErrorResponse(STATUS_CODE_AUTHENTICATION_KEY_ERROR, errorMessage);
		WriteErrorResponse(context, STATUS_CODE_AUTHENTICATION_KEY_ERROR, errorResponse);
	}

	/// <summary>
	/// Write 500 error response
	/// </summary>
	/// <param name="context">コンテキスト</param>
	/// <param name="reason">エラー理由</param>
	protected void Write500ErrorResponse(HttpContext context, string reason = "")
	{
		var errorResponse = GetErrorResponse(STATUS_CODE_ERROR_OTHER, reason);
		WriteErrorResponse(context, STATUS_CODE_ERROR_OTHER, errorResponse);
	}

	/// <summary>
	/// Write error response
	/// </summary>
	/// <param name="context">コンテキスト</param>
	/// <param name="statusCode">HTTP status code</param>
	/// <param name="data">Response data object</param>
	private void WriteErrorResponse(HttpContext context, int statusCode, object data)
	{
		context.Response.StatusCode = statusCode;
		context.Response.ContentType = HTTP_HEADER_CONTENT_TYPE_APPLICATION_JSON;
		context.Response.Write(ConvertToJson(data));
		context.Response.TrySkipIisCustomErrors = true;
		WriteReceiptInfoLog(data, context);
	}

	/// <summary>
	/// Write success response
	/// </summary>
	/// <param name="context">コンテキスト</param>
	/// <param name="data">Response data object</param>
	protected void WriteSuccessResponse(HttpContext context, object data)
	{
		context.Response.ContentType = HTTP_HEADER_CONTENT_TYPE_APPLICATION_JSON;
		context.Response.Write(ConvertToJson(data));
		WriteReceiptInfoLog(data, context);
	}

	/// <summary>
	/// Write response
	/// </summary>
	/// <param name="context">コンテクスト</param>
	/// <param name="responseHandler">The response hanlder</param>
	protected void WriteResponse(HttpContext context, Func<object, bool> responseHandler = null)
	{
		try
		{
			if (IsErrorSocialProviderIdLine())
			{
				Write500ErrorResponse(context, this.ERROR_MESSAGE_LINK_LINE_EMPTY);
				return;
			}

			if (ValidateRequest(context) == false) return;

			var response = GetResponseData();
			if (response == null)
			{
				Write300ErrorResponse(context);
				return;
			}

			if ((responseHandler != null) && (responseHandler(response) == false)) return;

			WriteSuccessResponse(context, response);
		}
		catch (Exception ex)
		{
			var errorMessage = ex.Message.Split(Environment.NewLine.ToCharArray())[0];
			Write500ErrorResponse(context, errorMessage);
		}
	}

	/// <summary>
	/// Convert to Json
	/// </summary>
	/// <param name="data">The data</param>
	/// <returns>A Json string</returns>
	protected string ConvertToJson(object data)
	{
		var settings = new JsonSerializerSettings
		{
			Formatting = Newtonsoft.Json.Formatting.Indented,
			NullValueHandling = NullValueHandling.Ignore
		};
		var json = JsonConvert.SerializeObject(data, settings);
		return json;
	}

	/// <summary>
	/// Is number
	/// </summary>
	/// <param name="value">A value</param>
	/// <returns>True if value as string is number, otherwise false</returns>
	protected bool IsNumber(string value)
	{
		int result;
		var success = int.TryParse(value, out result);
		return success;
	}

	/// <summary>
	/// Write receipt information log
	/// </summary>
	/// <param name="context">コンテキスト</param>
	/// <param name="responseData">Response data</param>
	protected void WriteReceiptInfoLog(object responseData, HttpContext context = null)
	{
		SendLineMessageApiServer.WriteReceiptInfoLog(
			context.Request.Url.ToString(),
			context.Response,
			responseData);
	}

	/// <summary>
	/// Error responses
	/// </summary>
	/// <param name="input">A date time input string</param>
	/// <param name="dateTime">A converted date time</param>
	/// <returns>True if date time input is valid ISO 8601 format, otherwise false</returns>
	public bool IsDateTimeFormatISO8601(string input, out DateTime dateTime)
	{
		var result = DateTime.TryParseExact(
			input,
			m_FormatIso8601,
			null,
			DateTimeStyles.None,
			out dateTime);
		return result;
	}
}
