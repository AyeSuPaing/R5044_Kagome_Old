<%--
=========================================================================================================
  Module      : User Get(UserGet.ashx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="UserGet" %>

using System.Web;
using w2.Common.Util;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.UserIntegration;

/// <summary>
/// User Get
/// </summary>
public class UserGet : LineBasePage, IHttpHandler
{
	/// <summary>
	/// プロセスリクエスト
	/// </summary>
	/// <param name="context">Context</param>
	public override void ProcessRequest(HttpContext context)
	{
		GetRequest(context);
		WriteResponse(context, (response) =>
		{
			var userGetResponse = response as LineUserGetResponse;
			if (userGetResponse == null) return false;

			if (string.Equals(userGetResponse.Status, STATUS_CODE_ANY_USER))
			{
				Write301ErrorResponse(context);
				return false;
			}
			return true;
		});
	}

	/// <summary>
	/// ソーシャルプラスオプションチェック抽象メソッド
	/// </summary>
	/// <returns>チェック結果</returns>
	protected override bool IsErrorSocialProviderIdLine()
	{
		var result = (((this.IdType == Constants.ID_TYPE_LINE_USER_ID) || (this.IdType == Constants.ID_TYPE_REGISRT_LINE_ID))
			&& string.IsNullOrEmpty(Constants.SOCIAL_PROVIDER_ID_LINE));
		return result;
	}

	/// <summary>
	/// リクエスト取得
	/// </summary>
	/// <param name="context">コンテキスト</param>
	/// <returns>リクエスト文字列</returns>
	protected override void GetRequest(HttpContext context)
	{
		this.IdType = (string)context.Request[Constants.REQUEST_KEY_ID_TYPE];
		this.UserId = (string)context.Request[Constants.REQUEST_KEY_USER_ID];
		this.Name = (string)context.Request[Constants.REQUEST_KEY_NAME];
		this.NameKana = (string)context.Request[Constants.REQUEST_KEY_NAME_KANA];
		this.Tel = (string)context.Request[Constants.REQUEST_KEY_TEL];
		this.MailId = (string)context.Request[Constants.REQUEST_KEY_MAIL_ID];
		this.LineId = (string)context.Request[Constants.REQUEST_KEY_LINE_ID];
	}

	/// <summary>
	/// Is valid authorization
	/// </summary>
	/// <returns>True if parameter is valid, otherwise false</returns>
	protected override bool IsValidParameters()
	{
		// Check Id type
		if ((string.IsNullOrEmpty(this.IdType) == false)
			&& (this.IdType != Constants.ID_TYPE_EC_USER_ID)
			&& (this.IdType != Constants.ID_TYPE_LINE_USER_ID)
			&& (this.IdType != Constants.ID_TYPE_REGISRT_LINE_ID))
		{
			return false;
		}

		// Check Line Id and User Id when IdType is regist line id
		if (this.IdType == Constants.ID_TYPE_REGISRT_LINE_ID
			&& ((string.IsNullOrEmpty(this.LineId) == false)
				&& (string.IsNullOrEmpty(this.UserId) == false)))
		{
			return true;
		}

		// ユーザーID　OR　LINE番号が一致している場合の情報出力
		if ((string.IsNullOrEmpty(this.UserId) == false)
			&& ((this.IdType == Constants.ID_TYPE_EC_USER_ID)
				|| (this.IdType == Constants.ID_TYPE_LINE_USER_ID)))
		{
			return true;
		}

		// お名前と電話番号が一致している場合の情報出力
		if (((string.IsNullOrEmpty(this.Name) == false)
				|| (string.IsNullOrEmpty(this.NameKana) == false))
			&& (string.IsNullOrEmpty(this.Tel) == false))
		{
			return true;
		}

		// お名前と電話番号とメールアドレスが一致している場合の情報出力
		if (((string.IsNullOrEmpty(this.Name) == false)
				|| (string.IsNullOrEmpty(this.NameKana) == false))
			&& (string.IsNullOrEmpty(this.Tel) == false)
			&& (string.IsNullOrEmpty(this.MailId) == false))
		{
			return true;
		}

		return false;
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
	/// Get user data
	/// </summary>
	/// <returns>An user model</returns>
	private UserModel[] GetUserData()
	{
		var isSearchWithLineUserId = (this.IdType == Constants.ID_TYPE_LINE_USER_ID);
		var tel = (string.IsNullOrEmpty(this.Tel))
			? this.Tel
			: this.Tel.Replace("-", "");
		var model = new UserService().GetUsersForLine(
			Constants.SOCIAL_PROVIDER_ID_LINE,
			this.UserId,
			this.Name,
			this.NameKana,
			tel,
			this.MailId,
			isSearchWithLineUserId);
		return model;
	}

	/// <summary>
	/// Get response data
	/// </summary>
	/// <returns>A response object</returns>
	protected override object GetResponseData()
	{
		var userModel = GetUserData();

		if (userModel.Length == 0) return null;
		if (userModel.Length > 1)
		{
			return new LineUserGetResponse() { Status = STATUS_CODE_ANY_USER };
		}

		if (IdType == Constants.ID_TYPE_REGISRT_LINE_ID
			&& string.IsNullOrEmpty(Constants.SOCIAL_PROVIDER_ID_LINE) == false)
		{
			RegistLineId(userModel[0]);
		}

		// User
		var response = new LineUserGetResponse
		{
			UserId = userModel[0].UserId,
			UserKbn = userModel[0].UserKbn,
			Name = userModel[0].Name,
			Name1 = userModel[0].Name1,
			Name2 = userModel[0].Name2,
			NameKana = userModel[0].NameKana,
			NameKana1 = userModel[0].NameKana1,
			NameKana2 = userModel[0].NameKana2,
			MailAddr = userModel[0].MailAddr,
			MailAddr2 = userModel[0].MailAddr2,
			Zip = userModel[0].Zip,
			Addr1 = userModel[0].Addr1,
			Addr2 = userModel[0].Addr2,
			Addr3 = userModel[0].Addr3,
			Addr4 = userModel[0].Addr4,
			Addr5 = userModel[0].Addr5,
			Tel1 = userModel[0].Tel1,
			Sex = userModel[0].Sex,
			Birth = StringUtility.ToEmpty(userModel[0].Birth),
			BirthYear = userModel[0].BirthYear,
			BirthMonth = userModel[0].BirthMonth,
			BirthDay = userModel[0].BirthDay,
			MailFlg = userModel[0].MailFlg,
			EasyRegisterFlg = userModel[0].EasyRegisterFlg,
			UserMemo = userModel[0].UserMemo,
			AdvcodeFirst = userModel[0].AdvcodeFirst,
			DelFlg = userModel[0].DelFlg,
			MemberRankId = userModel[0].MemberRankId,
			FixedPurchaseMemberFlg = userModel[0].FixedPurchaseMemberFlg,
			OrderCountOrderRealtime = userModel[0].OrderCountOrderRealtime,
			OrderCountOld = userModel[0].OrderCountOld,
			LineUserId =
				userModel[0].UserExtend.UserExtendDataValue.ContainsKey(Constants.SOCIAL_PROVIDER_ID_LINE)
					? userModel[0].UserExtend.UserExtendDataValue[Constants.SOCIAL_PROVIDER_ID_LINE]
					: string.Empty,
			DateCreated = StringUtility.ToEmpty(userModel[0].DateCreated),
			DateChanged = StringUtility.ToEmpty(userModel[0].DateChanged),
			Status = STATUS_CODE_SUCCESS,
			IntegratedUserId = (userModel[0].IntegratedFlg == Constants.FLG_USER_INTEGRATED_FLG_DONE)
				? new UserIntegrationService().GetIntegratedUserId(this.UserId)
				: string.Empty,
		};

		// User attribute
		var userAttributeModel = new UserService().GetUserAttribute(this.UserId);
		if (userAttributeModel != null)
		{
			response.FirstOrderDate = StringUtility.ToEmpty(userAttributeModel.FirstOrderDate);
			response.SecondOrderDate = StringUtility.ToEmpty(userAttributeModel.SecondOrderDate);
			response.LastOrderDate = StringUtility.ToEmpty(userAttributeModel.LastOrderDate);
			response.EnrollmentDays = StringUtility.ToEmpty(userAttributeModel.EnrollmentDays);
			response.AwayDays = StringUtility.ToEmpty(userAttributeModel.AwayDays);
			response.OrderAmountOrderAll = userAttributeModel.OrderAmountOrderAll;
			response.OrderAmountOrderFp = userAttributeModel.OrderAmountOrderFp;
			response.OrderCountOrderAll = userAttributeModel.OrderCountOrderAll;
			response.OrderCountOrderFp = userAttributeModel.OrderCountOrderFp;
			response.OrderAmountShipAll = userAttributeModel.OrderAmountShipAll;
			response.OrderAmountShipFp = userAttributeModel.OrderAmountShipFp;
			response.OrderCountShipAll = userAttributeModel.OrderCountShipAll;
			response.OrderCountShipFp = userAttributeModel.OrderCountShipFp;
			response.AttributeDateChanged = StringUtility.ToEmpty(userAttributeModel.DateChanged);
			response.CpmClusterAttribute1 = userAttributeModel.CpmClusterAttribute1;
			response.CpmClusterAttribute2 = userAttributeModel.CpmClusterAttribute2;
		}
		else
		{
			response.FirstOrderDate = string.Empty;
			response.SecondOrderDate = string.Empty;
			response.LastOrderDate = string.Empty;
			response.EnrollmentDays = string.Empty;
			response.AwayDays = string.Empty;
			response.OrderAmountOrderAll = 0m;
			response.OrderAmountOrderFp = 0m;
			response.OrderCountOrderAll = 0;
			response.OrderCountOrderFp = 0;
			response.OrderAmountShipAll = 0m;
			response.OrderAmountShipFp = 0m;
			response.OrderCountShipAll = 0;
			response.OrderCountShipFp = 0;
			response.AttributeDateChanged = string.Empty;
			response.CpmClusterAttribute1 = string.Empty;
			response.CpmClusterAttribute2 = string.Empty;
		}
		return response;
	}

	/// <summary>
	/// Regist Line Id
	/// </summary>
	/// <param name="userModel">User Model</param>
	private void RegistLineId(UserModel userModel)
	{
		var userService = new UserService();
		userModel.UserExtend.UserExtendDataValue[Constants.SOCIAL_PROVIDER_ID_LINE] = this.LineId;
		userService.UpdateUserExtend(
			userModel.UserExtend,
			userModel.UserId,
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.Insert);
	}

	/// <summary>Request key: ID Type</summary>
	public string IdType { get; set; }
	/// <summary>Request key: User ID</summary>
	public string UserId { get; set; }
	/// <summary>Request key: Name</summary>
	public string Name { get; set; }
	/// <summary>Request key: Name Kana</summary>
	public string NameKana { get; set; }
	/// <summary>Request key: Tel</summary>
	public string Tel { get; set; }
	/// <summary>Request key: Mail ID</summary>
	public string MailId { get; set; }
	/// <summary>Request key: Line ID</summary>
	public string LineId { get; set; }
	/// <summary>IsReusable</summary>
	public bool IsReusable { get { return false; } }
}
