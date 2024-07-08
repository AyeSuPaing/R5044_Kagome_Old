<%--
=========================================================================================================
  Module      : 更新済み顧客情報取得API (ModifyUserList.ashx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="ModifyUserList" %>

using System;
using System.Web;
using w2.Common.Util;
using w2.Domain.User;

/// <summary>
/// 更新済み顧客情報取得API 
/// </summary>
public class ModifyUserList : LineBasePage, IHttpHandler {

	/// <summary>
	/// プロセスリクエスト
	/// </summary>
	/// <param name="context">Context</param>
	public override void ProcessRequest (HttpContext context) {
		GetRequest(context);
		WriteResponse(context);
	}

	/// <summary>
	/// リクエスト取得
	/// </summary>
	/// <param name="context">コンテキスト</param>
	/// <returns>リクエスト文字列</returns>
	protected override void GetRequest(HttpContext context)
	{
		this.Limit = (string)context.Request[Constants.REQUEST_KEY_LIMIT];
		this.Offset = (string)context.Request[Constants.REQUEST_KEY_OFFSET];
		this.UpdateAtString = (string)context.Request[Constants.REQUEST_KEY_UPDATE_AT];
		this.SortType = (string)context.Request[Constants.REQUEST_KEY_SORT_TYPE];
	}

	/// <summary>
	/// ソーシャルプラスオプションチェック抽象メソッド
	/// </summary>
	/// <returns>チェック結果</returns>
	protected override bool IsErrorSocialProviderIdLine() { return false; }

	/// <summary>
	/// Is valid authorization
	/// </summary>
	/// <returns>True if parameter is valid, otherwise false</returns>
	protected override bool IsValidParameters()
	{
		DateTime updateAt;
		if (IsDateTimeFormatISO8601(this.UpdateAtString, out updateAt) == false)
		{
			return false;
		}
		this.UpdateAt = updateAt;

		var isValid = IsNumber(this.Limit)
			&& IsNumber(this.Offset)
			&& Validator.IsHalfwidthNumber(this.Limit)
			&& Validator.IsHalfwidthNumber(this.Offset)
			&& ((this.SortType == Constants.REQUEST_KEY_SORT_TYPE_DESC)
				|| (this.SortType == Constants.REQUEST_KEY_SORT_TYPE_ASC));
		return isValid;
	}

	/// <summary>
	/// Get error response
	/// </summary>
	/// <param name="parameters">A parameters</param>
	/// <returns>An error response object</returns>
	protected override object GetErrorResponse(params object[] parameters)
	{
		var errorResponse = new LineErrorResponse()
		{
			Status = (int)parameters[0],
			Reason = StringUtility.ToEmpty(parameters[1])
		};
		return errorResponse;
	}

	/// <summary>
	/// Get response data
	/// </summary>
	/// <returns>A response object</returns>
	protected override object GetResponseData()
	{
		var userIds = new UserService()
			.GetModifyUsers(this.UpdateAt, int.Parse(this.Limit), int.Parse(this.Offset), this.SortType);
		var response = new LineModifyUserListResponse()
		{
			Limit = int.Parse(this.Limit),
			Offset = int.Parse(this.Offset),
			UserIds = userIds
		};
		return response;
	}

	/// <summary>Request key: Limit</summary>
	public string Limit { get; set; }
	/// <summary>Request key: Offset</summary>
	public string Offset { get; set; }
	/// <summary>Request key: Sort type</summary>
	public string SortType { get; set; }
	/// <summary>指定時刻_文字列</summary>
	public string UpdateAtString { set; get; }
	/// <summary>Request Key: Update At</summary>
	public DateTime UpdateAt { set; private get; }
	/// <summary>IsReusable</summary>
	public bool IsReusable { get { return false; } }
}