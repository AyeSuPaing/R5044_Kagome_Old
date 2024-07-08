<%--
=========================================================================================================
  Module      : Invoice Reissue(InvoiceReissue.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="False" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.GMO" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.GMO.Reissue" %>
<script runat="server">
	void Main()
	{
		var reader = new StreamReader(Request.InputStream);
		var xml = HttpUtility.UrlDecode(reader.ReadToEnd(), Encoding.UTF8);
		var response = new GmoResponseReissue();

		try
		{
			var request = w2.Common.Helper.SerializeHelper.Deserialize<GmoRequestReissue>(xml);
			response.Result = ResultCode.OK;
			response.Errors = new ErrorsElement();
			response.Errors.Error = new ErrorElement[]
			{
				new ErrorElement
				{
					ErrorCode = string.Empty,
					ErrorMessage = string.Empty
				}
			};
			response.TransactionResult = new TransactionResultElement();
			response.TransactionResult.GmoTransactionId = request.Transaction.GmoTransactionId;
			Response.ContentType = "application/xml";
			Response.Write(response.CreateResponseString());
		}
		catch(Exception ex)
		{
			response.Result = ResultCode.NG;
			response.Errors = new ErrorsElement();
			response.Errors.Error = new ErrorElement[]
			{
				new ErrorElement
				{
					ErrorCode = "9999",
					ErrorMessage = ex.Message
				}
			};
			response.TransactionResult = new TransactionResultElement();
			response.TransactionResult.GmoTransactionId = string.Empty;
			Response.ContentType = "application/xml";
			Response.Write(response.CreateResponseString());
		}
	}
</script>
<% this.Main(); %>