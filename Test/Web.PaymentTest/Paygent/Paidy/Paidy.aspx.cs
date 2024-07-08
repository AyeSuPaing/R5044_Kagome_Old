/*
=========================================================================================================
  Module      : Paidy (Paidy.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web.UI;

/// <summary>
/// Paidy
/// </summary>
public partial class Paygent_Paidy_Paidy : Page
{
	/// <summary>Paygent Paidy telegram kind: Cancel</summary>
	private const string CONST_TELEGRAM_KIND_CANCEL = "340";
	/// <summary>Paygent Paidy telegram kind: Sale</summary>
	private const string CONST_TELEGRAM_KIND_SALE = "341";
	/// <summary>Paygent Paidy telegram kind: Refund</summary>
	private const string CONST_TELEGRAM_KIND_REFUND = "342";
	/// <summary>Paygent Paidy telegram kind: Authorize</summary>
	private const string CONST_TELEGRAM_KIND_AUTHORIZE = "343";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			var telegramKind = this.Request.Form["telegram_kind"];
			var paymentId = this.Request.Form["payment_id"];
			var sucessResult = string.IsNullOrEmpty(paymentId)
				? "1"
				: "0";
			Dictionary<string, string> responseKeyValues;
			switch (telegramKind)
			{
				case CONST_TELEGRAM_KIND_CANCEL:
					responseKeyValues = new Dictionary<string, string>
					{
						{ "result", sucessResult },
						{ "trading_id", "0" },
						{ "payment_id", "0" },
						{ "response_detail", string.Format("({0})Paidy Processing failed.", telegramKind) },
					};
					break;

				case CONST_TELEGRAM_KIND_SALE:
					responseKeyValues = new Dictionary<string, string>
					{
						{ "result", sucessResult },
						{ "trading_id", "0" },
						{ "payment_id", "0" },
						{ "response_detail", string.Format("({0})Paidy Processing failed.", telegramKind) },
					};
					break;

				case CONST_TELEGRAM_KIND_REFUND:
					responseKeyValues = new Dictionary<string, string>
					{
						{ "result", sucessResult },
						{ "trading_id", "0" },
						{ "payment_id", "0" },
						{ "response_detail", string.Format("({0})Paidy Processing failed.", telegramKind) },
					};
					break;

				case CONST_TELEGRAM_KIND_AUTHORIZE:
					responseKeyValues = new Dictionary<string, string>
					{
						{ "result", "0" },
						{ "trading_id", "0" },
						{ "payment_id", "0" },
					};
					break;

				default:
					responseKeyValues = new Dictionary<string, string>();
					break;
			}
			WriteResponse(BuildResponseData(responseKeyValues));
			return;
		}
	}

	/// <summary>
	/// Build response data
	/// </summary>
	/// <param name="keyValuePairs">Key value pairs</param>
	/// <returns>Response data</returns>
	private string BuildResponseData(Dictionary<string, string> keyValuePairs)
	{
		var result = new StringBuilder();
		foreach (var keyValue in keyValuePairs)
		{
			result.AppendLine(string.Format("{0}={1}", keyValue.Key, keyValue.Value));
		}

		return result.ToString();
	}

	/// <summary>
	/// Write response
	/// </summary>
	private void WriteResponse(string response)
	{
		this.Response.StatusCode = HttpStatusCode.OK.GetHashCode();
		this.Response.Write(response);
	}
}
