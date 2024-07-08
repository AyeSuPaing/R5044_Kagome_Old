﻿<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="False" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.GMO" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.GMO.ReduceSales" %>
<script runat="server">
void Main()
{
	StreamReader reader = new StreamReader(Request.InputStream);
	string xml = HttpUtility.UrlDecode(reader.ReadToEnd(), Encoding.UTF8);

	try
	{
		var request = w2.Common.Helper.SerializeHelper.Deserialize<GmoRequestReduceSales>(xml);
		var response = new GmoResponseReduceSales();
		response.Result = ResultCode.OK;
		response.Errors = new ErrorsElement();
		response.Errors.Error = new ErrorElement[] { new ErrorElement() { ErrorCode = "", ErrorMessage = "" } };
		response.TransactionResult = new TransactionResultElement();
		response.TransactionResult.AuthorResult = "OK";
		response.TransactionResult.OldGmoTransactionId = request.Buyer.GmoTransactionId;
		response.TransactionResult.NewGmoTransactionId = "new_" + request.Buyer.GmoTransactionId;
		Response.ContentType = "application/xml";
		Response.Write(response.CreateResponseString());
	}
	catch(Exception ex)
	{
		var response = new GmoResponseReduceSales();
		response.Result = ResultCode.NG;
		response.Errors = new ErrorsElement();
		response.Errors.Error = new ErrorElement[] { new ErrorElement() { ErrorCode = "9999", ErrorMessage = ex.Message } };
		response.TransactionResult = new TransactionResultElement();
		response.TransactionResult.AuthorResult = "NG";
		response.TransactionResult.OldGmoTransactionId = "";
		response.TransactionResult.NewGmoTransactionId = "";
		Response.ContentType = "application/xml";
		Response.Write(response.CreateResponseString());
	}
}
</script>
<% this.Main(); %>