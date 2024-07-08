/*
=========================================================================================================
  Module      : Paygent Difference Notification (PaygentDifferenceNotification.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using w2.App.Common.Order.Payment.Paygent;

/// <summary>
/// Paygent Difference Notification
/// </summary>
public partial class Paygent_CVS_PaygentDifferenceNotification : System.Web.UI.Page
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			tbUrl.Text = "https://localhost/V5.14/Web/w2.Commerce.Front/Payment/Paygent/PaymentPaygentPaymentRecv.aspx";
			DataBind();
		}
	}

	/// <summary>
	/// Send event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btSend_Click(object sender, EventArgs e)
	{
		ServicePointManager.ServerCertificateValidationCallback =
			((senders, certificate, chain, sslPolicyErrors) => true);
		ServicePointManager.Expect100Continue = true;
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
		var requestParams = new Dictionary<string, string>()
		{
			{ PaygentConstants.PAYGENT_API_RESPONSE_RESULT, PaygentConstants.FLG_PAYGENT_API_RESPONSE_RESULT_NORMAL },
			{ PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_NOTICE_ID, tbPaymentNoticeId.Text.Trim() },
			{ PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_ID, tbPaymentId.Text.Trim() },
			{ PaygentConstants.PAYGENT_API_REQUEST_TRADING_ID, tbTradingId.Text.Trim() },
			{ PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_TYPE, tbPaymentType.Text.Trim() },
			{ PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_STATUS, tbPaymentStatus.Text.Trim() },
			{ PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_AMOUNT, tbPaymentAmount.Text.Trim() },
		};

		var hashRawData = string.Concat(
			requestParams[PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_NOTICE_ID],
			requestParams[PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_ID],
			requestParams[PaygentConstants.PAYGENT_API_REQUEST_TRADING_ID],
			requestParams[PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_TYPE],
			requestParams[PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_AMOUNT],
			tbHashKey.Text.Trim()
		);
		var hashData = CreateHashSha256(hashRawData);
		requestParams.Add(PaygentConstants.PAYGENT_API_REQUEST_HC, hashData);

		var request = (HttpWebRequest)WebRequest.Create(tbUrl.Text.Trim());
		request.Method = "POST";
		request.ContentType = "application/x-www-form-urlencoded";

		var postData = string.Join(
			"&",
			requestParams.Select(requestParam =>
				string.Format(
					"{0}={1}",
					requestParam.Key,
					HttpUtility.UrlEncode(requestParam.Value))));

		using (var stream = request.GetRequestStream())
		using (var writer = new StreamWriter(stream))
		{
			writer.Write(postData);
		}

		using (var response = (HttpWebResponse)request.GetResponse())
		using (var stream = response.GetResponseStream())
		using (var reader = new StreamReader(stream))
		{
			var responseBody = reader.ReadToEnd();
			lResponse.Text = responseBody;
		}
	}

	/// <summary>
	/// SHA-256でハッシュ化する
	/// </summary>
	/// <param name="data">データ</param>
	/// <returns>ハッシュ化されたデータ</returns>
	private string CreateHashSha256(string data)
	{
		var keyBytesForHash = Encoding.UTF8.GetBytes(data.Trim());
		using (var sha256 = new SHA256CryptoServiceProvider())
		{
			var hashBytes = sha256.ComputeHash(keyBytesForHash);
			var result = string.Join(string.Empty, hashBytes.Select(item => item.ToString("x2")));
			return result;
		}
	}
}
