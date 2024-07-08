/*
=========================================================================================================
  Module      : Authentication 3DSecure (Authentication3DSecure.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.Order.Payment.GMO.Zcom;
using w2.App.Common.Web.Page;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Util;
using w2.Common.Web;

/// <summary>
/// Zcom authentication 3DSecure
/// </summary>
public partial class Zcom_Authentication3DSecure : CommonPage
{
	#region WrappedControls
	/// <summary>Wrapped text box error code</summary>
	private WrappedTextBox WtbErrorCode { get { return GetWrappedControl<WrappedTextBox>(this.Form, "tbErrorCode"); } }
	/// <summary>Wrapped text box error detail</summary>
	private WrappedTextBox WtbErrorDetail { get { return GetWrappedControl<WrappedTextBox>(this.Form, "tbErrorDetail"); } }
	/// <summary>Wrapped label message</summary>
	private WrappedLabel WlbMessage { get { return GetWrappedControl<WrappedLabel>(this.Form, "lbMessage"); } }
	#endregion

	#region ErrorMessages
	/// <summary>Error message: Zcom authentication 3DSecure error code necessary</summary>
	private const string ERRMSG_ZCOM_AUTH3DS_ERROR_CODE_NECESSARY = "エラーコードは入力必須項目です。";
	/// <summary>Error message: Zcom authentication 3DSecure error detail necessary</summary>
	private const string ERRMSG_ZCOM_AUTH3DS_ERROR_DETAIL_NECESSARY = "エラーメッセージは入力必須項目です。";
	#endregion

	/// <summary>
	/// Page load
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			try
			{
				var transCode = StringUtility.ToEmpty(Request.Form[ZcomConst.PARAM_TRANS_CODE]);
				var zcomTransactionInfos = (List<Hashtable>)Cache[Constants.CACHE_KEY_PAYMENT_CREDIT_ZCOM_TRANSACTION_INFO];
				var zcomTransactionInfo = zcomTransactionInfos.FirstOrDefault(item =>
					(StringUtility.ToEmpty(item[ZcomConst.PARAM_TRANS_CODE_HASH]) == transCode));

				this.ContractCode = StringUtility.ToEmpty(zcomTransactionInfo[ZcomConst.PARAM_CONTRACT_CODE]);
				this.TransCode = StringUtility.ToEmpty(zcomTransactionInfo[ZcomConst.PARAM_TRANS_CODE]);
				this.OrderNumber = StringUtility.ToEmpty(zcomTransactionInfo[ZcomConst.PARAM_ORDER_NUMBER]);
				this.UserId = StringUtility.ToEmpty(zcomTransactionInfo[ZcomConst.PARAM_USER_ID]);
				this.PaymentCode = StringUtility.ToEmpty(Request.Form[ZcomConst.PARAM_PAYMENT_CODE]);
				this.BackUrl = StringUtility.ToEmpty(zcomTransactionInfo[ZcomConst.PARAM_BACK_URL]);
				this.ErrUrl = StringUtility.ToEmpty(zcomTransactionInfo[ZcomConst.PARAM_ERR_URL]);
				this.SuccessUrl = StringUtility.ToEmpty(zcomTransactionInfo[ZcomConst.PARAM_SUCCESS_URL]);
			}
			catch (Exception exception)
			{
				ShowMessage(exception.Message);
			}
		}
	}

	/// <summary>
	/// Button btnConfirm click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, EventArgs e)
	{
		var urlCreator = (string.IsNullOrEmpty(this.SuccessUrl)
			? new UrlCreator(this.UrlDefault)
			: new UrlCreator(this.SuccessUrl));
		var url = urlCreator.AddParam(ZcomConst.PARAM_CONTRACT_CODE, this.ContractCode)
			.AddParam(ZcomConst.PARAM_TRANS_CODE, this.TransCode)
			.AddParam(ZcomConst.PARAM_ORDER_NUMBER, this.OrderNumber)
			.AddParam(ZcomConst.PARAM_USER_ID, this.UserId)
			.AddParam(ZcomConst.PARAM_PAYMENT_CODE, this.PaymentCode)
			.AddParam(ZcomConst.PARAM_STATE, ZcomConst.STATUS_PROVISIONAL_SALES)
			.CreateUrl();

		Response.Redirect(url);
	}

	/// <summary>
	/// Button btnBack click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBack_Click(object sender, EventArgs e)
	{
		var urlCreator = (string.IsNullOrEmpty(this.BackUrl)
			? new UrlCreator(this.UrlDefault)
			: new UrlCreator(this.BackUrl));
		var url = urlCreator.AddParam(ZcomConst.PARAM_CONTRACT_CODE, this.ContractCode)
			.AddParam(ZcomConst.PARAM_TRANS_CODE, this.TransCode)
			.AddParam(ZcomConst.PARAM_ORDER_NUMBER, this.OrderNumber)
			.AddParam(ZcomConst.PARAM_USER_ID, this.UserId)
			.CreateUrl();

		RemoveZcomTransactionInfo();
		Response.Redirect(url);
	}

	/// <summary>
	/// Button btnError click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnError_Click(object sender, EventArgs e)
	{
		var errorMessage = CheckValidateInput();

		if (errorMessage.Length > 0)
		{
			ShowMessage(errorMessage);
			return;
		}

		var urlCreator = (string.IsNullOrEmpty(this.ErrUrl)
			? new UrlCreator(this.UrlDefault)
			: new UrlCreator(this.ErrUrl));
		var url = urlCreator.AddParam(ZcomConst.PARAM_CONTRACT_CODE, this.ContractCode)
			.AddParam(ZcomConst.PARAM_TRANS_CODE, this.TransCode)
			.AddParam(ZcomConst.PARAM_ORDER_NUMBER, this.OrderNumber)
			.AddParam(ZcomConst.PARAM_USER_ID, this.UserId)
			.AddParam(ZcomConst.PARAM_ERR_CODE, this.WtbErrorCode.Text)
			.AddParam(ZcomConst.PARAM_ERR_DETAIL, this.WtbErrorDetail.Text)
			.CreateUrl();

		RemoveZcomTransactionInfo();
		Response.Redirect(url);
	}

	/// <summary>
	/// Check validate input
	/// </summary>
	/// <returns>Error message</returns>
	private string CheckValidateInput()
	{
		var errorMessage = new StringBuilder();

		if (string.IsNullOrEmpty(this.WtbErrorCode.Text))
		{
			errorMessage.Append(ERRMSG_ZCOM_AUTH3DS_ERROR_CODE_NECESSARY);
		}

		if (string.IsNullOrEmpty(this.WtbErrorDetail.Text))
		{
			errorMessage
				.Append((errorMessage.Length > 0) ? "<br />" : string.Empty)
				.Append(ERRMSG_ZCOM_AUTH3DS_ERROR_DETAIL_NECESSARY);
		}

		return errorMessage.ToString();
	}

	/// <summary>
	/// Remove Zcom transaction info
	/// </summary>
	private void RemoveZcomTransactionInfo()
	{
		var zcomTransactionInfos = (List<Hashtable>)Cache[Constants.CACHE_KEY_PAYMENT_CREDIT_ZCOM_TRANSACTION_INFO];
		var zcomTransactionInfo = zcomTransactionInfos.FirstOrDefault(item =>
			(StringUtility.ToEmpty(item[ZcomConst.PARAM_TRANS_CODE_HASH]) == this.TransCode));
		zcomTransactionInfos.Remove(zcomTransactionInfo);
	}

	/// <summary>
	/// Show message
	/// </summary>
	/// <param name="message">Message</param>
	private void ShowMessage(string message)
	{
		this.WlbMessage.Visible = true;
		this.WlbMessage.Text = message;
	}

	#region +Properties
	/// <summary>Contract code</summary>
	protected string ContractCode
	{
		get { return StringUtility.ToEmpty(ViewState[ZcomConst.PARAM_CONTRACT_CODE]); }
		set { ViewState[ZcomConst.PARAM_CONTRACT_CODE] = value; }
	}
	/// <summary>Trans code</summary>
	protected string TransCode
	{
		get { return StringUtility.ToEmpty(ViewState[ZcomConst.PARAM_TRANS_CODE]); }
		set { ViewState[ZcomConst.PARAM_TRANS_CODE] = value; }
	}
	/// <summary>Order number</summary>
	protected string OrderNumber
	{
		get { return StringUtility.ToEmpty(ViewState[ZcomConst.PARAM_ORDER_NUMBER]); }
		set { ViewState[ZcomConst.PARAM_ORDER_NUMBER] = value; }
	}
	/// <summary>User id</summary>
	protected string UserId
	{
		get { return StringUtility.ToEmpty(ViewState[ZcomConst.PARAM_USER_ID]); }
		set { ViewState[ZcomConst.PARAM_USER_ID] = value; }
	}
	/// <summary>Payment code</summary>
	protected string PaymentCode
	{
		get { return StringUtility.ToEmpty(ViewState[ZcomConst.PARAM_PAYMENT_CODE]); }
		set { ViewState[ZcomConst.PARAM_PAYMENT_CODE] = value; }
	}
	/// <summary>Back url</summary>
	protected string BackUrl
	{
		get { return StringUtility.ToEmpty(ViewState[ZcomConst.PARAM_BACK_URL]); }
		set { ViewState[ZcomConst.PARAM_BACK_URL] = value; }
	}
	/// <summary>Error url</summary>
	protected string ErrUrl
	{
		get { return StringUtility.ToEmpty(ViewState[ZcomConst.PARAM_ERR_URL]); }
		set { ViewState[ZcomConst.PARAM_ERR_URL] = value; }
	}
	/// <summary>Success url</summary>
	protected string SuccessUrl
	{
		get { return StringUtility.ToEmpty(ViewState[ZcomConst.PARAM_SUCCESS_URL]); }
		set { ViewState[ZcomConst.PARAM_SUCCESS_URL] = value; }
	}
	/// <summary>Url default</summary>
	protected string UrlDefault
	{
		get
		{
			var result = string.Format(
				"{0}{1}{2}{3}",
				Constants.PROTOCOL_HTTPS,
				Constants.SITE_DOMAIN,
				Constants.PATH_ROOT_FRONT_PC,
				Constants.PAGE_FRONT_PAYMENT_ZCOM_CARD_3DSECURE_GET_RESULT);
			return result;
		}
	}
	#endregion
}