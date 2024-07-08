/*
=========================================================================================================
  Module      : Rakuten Cvs Api (RakutenCvsApi.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using w2.App.Common.Order.Payment.Rakuten;
using w2.App.Common.Order.Payment.Rakuten.Authorize;
using w2.Common.Logger;
using w2.Common.Util;

/// <summary>
/// Rakuten cvs api
/// </summary>
public class RakutenCvsApi
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="context">Context</param>
	/// <param name="typeProcess">Type process</param>
	public RakutenCvsApi(HttpContext context, string typeProcess)
	{
		this.Context = context;
		this.TypeProcess = typeProcess;
	}

	/// <summary>
	/// Exec process
	/// </summary>
	/// <returns>Json response</returns>
	public string ExecProcess()
	{
		InitComponents();
		DoPost();
		var result = GetJsonResponse(this.TypeProcess);
		return result;
	}

	/// <summary>
	/// Do post
	/// </summary>
	private void DoPost()
	{
		try
		{
			using (var client = new WebClient())
			{
				var requestNotification = JObject.Parse(GetJsonResponse("Notification"));

				// Set HTTP headers
				client.Headers.Add(
					RakutenConstants.HTTP_HEADER_CONTENT_TYPE,
					RakutenConstants.HTTP_HEADER_CONTENT_TYPE_APPLICATION_FORM);

				var url = string.Format(
					"{0}{1}{2}{3}",
					Constants.PROTOCOL_HTTP,
					Constants.SITE_DOMAIN,
					Constants.PATH_ROOT_FRONT_PC,
					Constants.PAGE_FRONT_PAYMENT_RAKUTEN_CVS_PAYMENT_RECEIVE);

				// Call api
				var responseBytes = client.UploadString(
					url,
					RakutenConstants.HTTP_METHOD_POST,
					CreateRequestData(requestNotification));

				// Handle success response
				var responseBody = responseBytes;
				var queryData = HttpUtility.ParseQueryString(responseBody);
				if (queryData.AllKeys.All(item => string.IsNullOrEmpty(item)) == false)
				{
					var dictionary = queryData.AllKeys
						.ToDictionary(item => item, item => queryData[item]);

					responseBody = JsonConvert.SerializeObject(
						dictionary,
						new JsonSerializerSettings
						{
							Formatting = Formatting.Indented,
							NullValueHandling = NullValueHandling.Ignore,
						});

					JsonConvert.DeserializeObject<RakutenResponseBase>(responseBody);
				}
			}
		}
		catch (Exception ex)
		{
			FileLogger.Write("RakutenCvs", ex.Message);
		}
	}

	/// <summary>
	/// Get json response
	/// </summary>
	/// <param name="elementName">Element name</param>
	/// <returns>Json response</returns>
	private string GetJsonResponse(string elementName)
	{
		var path = Path.Combine(
			AppDomain.CurrentDomain.BaseDirectory,
			"Json",
			"Rakuten",
			elementName + ".json");

		using (var reader = new StreamReader(path))
		{
			var jsonResponse = reader.ReadToEnd();
			var result = jsonResponse.Replace("@@ paymentId @@", this.PaymentId)
				.Replace("@@ serviceReferenceId @@", StringUtility.ToEmpty(this.ServiceReferenceId))
				.Replace("@@ transactionTime @@", this.TransactionTime)
				.Replace("@@ requestType @@", this.TypeProcess);
			return result;
		}
	}

	/// <summary>
	/// Init components
	/// </summary>
	private void InitComponents()
	{
		var paymentInfoBase64 = this.Context.Request.Form[RakutenConstants.HTTP_PARAMETER_PAYMENT_INFO];

		switch (this.TypeProcess)
		{
			case "Authorize":
				this.RakutenAuthorizeRequest = JsonConvert.DeserializeObject<RakutenAuthorizeRequest>(
					Encoding.UTF8.GetString(Base64UrlDecode(paymentInfoBase64)));
				this.PaymentId = this.RakutenAuthorizeRequest.PaymentId;
				this.ServiceReferenceId = this.RakutenAuthorizeRequest.ServiceReferenceId;
				this.TransactionTime = this.RakutenAuthorizeRequest.Timestamp;
				break;

			case "CancelOrRefund":
				this.RakutenCancelOrRefundRequest = JsonConvert.DeserializeObject<RakutenCancelOrRefundRequest>(
					Encoding.UTF8.GetString(Base64UrlDecode(paymentInfoBase64)));
				this.PaymentId = this.RakutenCancelOrRefundRequest.PaymentId;
				this.TransactionTime = this.RakutenCancelOrRefundRequest.Timestamp;
				break;
		}
	}

	/// <summary>
	/// Create request data
	/// </summary>
	/// <param name="requestParam">Request param</param>
	/// <returns>Request data</returns>
	private string CreateRequestData(object requestParam)
	{
		var jsonData = JsonConvert.SerializeObject(
			requestParam,
			Formatting.None,
			new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			});

		var dataObject = string.Format(
			"{0}={1}&{2}={3}&{4}={5}",
			RakutenConstants.HTTP_PARAMETER_PAYMENT_INFO,
			RakutenApiFacade.EncodeToBase64Url(Encoding.UTF8.GetBytes(jsonData)),
			RakutenConstants.HTTP_PARAMETER_SIGNATURE,
			RakutenApiFacade.CreateSignature(jsonData),
			RakutenConstants.PARAMETER_KEY_VERSION,
			RakutenConstants.KEY_VERSION);

		return dataObject;
	}

	/// <summary>
	/// Base64 url decode
	/// </summary>
	/// <param name="paymentInfoBase64">Payment info base64</param>
	/// <returns>Converted data</returns>
	private byte[] Base64UrlDecode(string paymentInfoBase64)
	{
		var output = paymentInfoBase64;
		output = output.Replace('-', '+').Replace('_', '/');
		switch (output.Length % 4)
		{
			case 0:
				break;

			case 2:
				output += "==";
				break;

			case 3:
				output += "=";
				break;

			default:
				throw new Exception("Illegal base64url string!");
		}
		var converted = Convert.FromBase64String(output);
		return converted;
	}

	/// <summary>The context</summary>
	private HttpContext Context { set; get; }
	/// <summary>Rakuten authorize request</summary>
	private RakutenAuthorizeRequest RakutenAuthorizeRequest { get; set; }
	/// <summary>Rakuten cancel or refund request</summary>
	private RakutenCancelOrRefundRequest RakutenCancelOrRefundRequest { get; set; }
	/// <summary>Type process</summary>
	private string TypeProcess { get; set; }
	/// <summary>Payment id</summary>
	private string PaymentId { get; set; }
	/// <summary>Service reference id</summary>
	private string ServiceReferenceId { get; set; }
	/// <summary>Transaction time</summary>
	private string TransactionTime { get; set; }
}
