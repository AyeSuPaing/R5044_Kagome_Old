<%--
=========================================================================================================
  Module      : 定期台帳取得API (FixedPurchaseGet.ashx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="FixedPurchaseGet" %>

using System;
using System.Linq;
using System.Web;
using w2.Common.Util;
using w2.Domain.FixedPurchase;
using w2.Domain.User;

/// <summary>
/// 定期台帳取得API
/// </summary>
public class FixedPurchaseGet : LineBasePage, IHttpHandler
{
	/// <summary>
	/// プロセスリクエスト
	/// </summary>
	/// <param name="context">Context</param>
	public override void ProcessRequest(HttpContext context)
	{
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
		this.IdType = (string)context.Request[Constants.REQUEST_KEY_ID_TYPE];
		this.UserId = (string)context.Request[Constants.REQUEST_KEY_USER_ID];
		this.Limit = (string)context.Request[Constants.REQUEST_KEY_LIMIT];
		this.Offset = (string)context.Request[Constants.REQUEST_KEY_OFFSET];
		this.UpdateAtString = (string)context.Request[Constants.REQUEST_KEY_UPDATE_AT];
	}

	/// <summary>
	/// ソーシャルプラスオプションチェック抽象メソッド
	/// </summary>
	/// <returns>チェック結果</returns>
	protected override bool IsErrorSocialProviderIdLine()
	{
		var result = ((this.IdType == Constants.ID_TYPE_LINE_USER_ID)
			&& string.IsNullOrEmpty(Constants.SOCIAL_PROVIDER_ID_LINE));
		return result;
	}

	/// <summary>
	/// パラメータチェック
	/// </summary>
	/// <returns>チェック結果</returns>
	protected override bool IsValidParameters()
	{
		DateTime updateAt;
		if (IsDateTimeFormatISO8601(this.UpdateAtString, out updateAt) == false)
		{
			return false;
		}
		this.UpdateAt = updateAt;

		var isValid = (string.IsNullOrEmpty(this.UserId) == false)
			&& Validator.IsHalfwidthNumber(this.Limit)
			&& Validator.IsHalfwidthNumber(this.Offset)
			&& ((this.IdType == Constants.ID_TYPE_EC_USER_ID)
				|| (this.IdType == Constants.ID_TYPE_LINE_USER_ID));

		return isValid;
	}

	/// <summary>
	/// レスポンス作成
	/// </summary>
	/// <returns>レスポンス</returns>
	protected override object GetResponseData()
	{
		// Get orders
		var fixedPurchase = GetFixedPurchaseData();
		var responseData = fixedPurchase
			.Select(ConvertFixedPurchaseData)
			.ToArray();
		var response = new LineFixedPurchaseGetResponse
		{
			Offset = int.Parse(this.Offset),
			Limit = int.Parse(this.Limit),
			FixedPurchases = responseData.ToArray(),
			Status = STATUS_CODE_SUCCESS,
		};
		return response;
	}

	/// <summary>
	/// リクエストによって、定期データ取得
	/// </summary>
	/// <returns>定期データリスト</returns>
	private FixedPurchaseModel[] GetFixedPurchaseData()
	{
		var userId = string.Empty;
		switch (this.IdType)
		{
			case Constants.ID_TYPE_LINE_USER_ID:
				var userModel = new UserService().GetByExtendColumn(
					Constants.SOCIAL_PROVIDER_ID_LINE,
					this.UserId);
				userId = (userModel == null)
					? string.Empty
					: userModel.UserId;
				break;

			case Constants.ID_TYPE_EC_USER_ID:
				userId = this.UserId;
				break;
		}
		var result = new FixedPurchaseService().GetFixedPurchasesForLine(
			userId,
			int.Parse(this.Offset),
			int.Parse(this.Limit),
			this.UpdateAt);
		return result;
	}

	/// <summary>
	/// データ変換
	/// </summary>
	/// <param name="fixedPurchase">定期台帳モデル</param>
	/// <returns>定期台帳レスポンス</returns>
	private LineFixedPurchase ConvertFixedPurchaseData(FixedPurchaseModel fixedPurchase)
	{
		var fixedPurchaseData = new LineFixedPurchase
		{
			FixedPurchaseId = fixedPurchase.FixedPurchaseId,
			FixedPurchaseKbn = fixedPurchase.FixedPurchaseKbn,
			FixedPurchaseSetting1 = fixedPurchase.FixedPurchaseSetting1,
			FixedPurchaseStatus = fixedPurchase.FixedPurchaseStatus,
			PaymentStatus = fixedPurchase.PaymentStatus,
			LastOrderDate = StringUtility.ToEmpty(fixedPurchase.LastOrderDate),
			OrderCount = fixedPurchase.OrderCount,
			UserId = fixedPurchase.UserId,
			ShopId = fixedPurchase.ShopId,
			OrderKbn = fixedPurchase.OrderKbn,
			OrderPaymentKbn = fixedPurchase.OrderPaymentKbn,
			FixedPurchaseDateBgn = StringUtility.ToEmpty(fixedPurchase.FixedPurchaseDateBgn),
			ValidFlg = fixedPurchase.ValidFlg,
			DateCreated = StringUtility.ToEmpty(fixedPurchase.DateCreated),
			DateChanged = StringUtility.ToEmpty(fixedPurchase.DateChanged),
			LastChanged = fixedPurchase.LastChanged,
			CreditBranchNo = fixedPurchase.CreditBranchNo,
			NextShippingDate = StringUtility.ToEmpty(fixedPurchase.NextShippingDate),
			NextNextShippingDate = StringUtility.ToEmpty(fixedPurchase.NextNextShippingDate),
			FixedPurchaseManagementMemo = fixedPurchase.FixedPurchaseManagementMemo,
			CardInstallmentsCode = fixedPurchase.CardInstallmentsCode,
			ShippedCount = fixedPurchase.ShippedCount,
			CancelReasonId = fixedPurchase.CancelReasonId,
			CancelMemo = fixedPurchase.CancelMemo,
			NextShippingUsePoint = fixedPurchase.NextShippingUsePoint,
			CombinedOrgFixedpurchaseIds = fixedPurchase.CombinedOrgFixedpurchaseIds,
			Memo = fixedPurchase.Memo,
			CancelDate = StringUtility.ToEmpty(fixedPurchase.CancelDate),
			RestartDate = StringUtility.ToEmpty(fixedPurchase.RestartDate),
			ExternalPaymentAgreementId = fixedPurchase.ExternalPaymentAgreementId,
			ResumeDate = StringUtility.ToEmpty(fixedPurchase.ResumeDate),
			SuspendReason = fixedPurchase.SuspendReason,
			ShippingMemo = fixedPurchase.ShippingMemo,
			NextShippingUseCouponId = fixedPurchase.NextShippingUseCouponId,
			NextShippingUseCouponNo = fixedPurchase.NextShippingUseCouponNo,
			ReceiptFlg = fixedPurchase.ReceiptFlg,
			ReceiptAddress = fixedPurchase.ReceiptAddress,
			ReceiptProviso = fixedPurchase.ReceiptProviso,
		};
		return fixedPurchaseData;
	}

	/// <summary>
	/// エラーメッセージ作成
	/// </summary>
	/// <param name="parameters">パラメータ</param>
	/// <returns>エラーレスポンス</returns>
	protected override object GetErrorResponse(params object[] parameters)
	{
		var errorResponse = new LineErrorResponse()
		{
			Status = (int)parameters[0],
			Reason = StringUtility.ToEmpty(parameters[1])
		};
		return errorResponse;
	}

	/// <summary>Idタイプ</summary>
	public string IdType { get; set; }
	/// <summary>ユーザーID</summary>
	public string UserId { get; set; }
	/// <summary>最大取得件数</summary>
	public string Limit { get; set; }
	/// <summary>開始位置</summary>
	public string Offset { get; set; }
	/// <summary>指定時刻_文字列</summary>
	public string UpdateAtString { set; get; }
	/// <summary>指定時刻</summary>
	public DateTime UpdateAt { set; private get; }
	/// <summary>IsReusable</summary>
	public bool IsReusable { get { return false; } }
}