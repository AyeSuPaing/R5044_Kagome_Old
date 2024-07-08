<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="False" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.GMO" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.GMO.GetAuthResult" %>
<script runat="server">
void Main()
{
	StreamReader reader = new StreamReader(Request.InputStream);
	string xml = HttpUtility.UrlDecode(reader.ReadToEnd(), Encoding.UTF8);

	try
	{
		var request = w2.Common.Helper.SerializeHelper.Deserialize<GmoRequestGetAuthResult>(xml);
		var response = new GmoResponseGetAuthResult();
		response.Result = ResultCode.OK;
		response.Errors = new ErrorsElement();
		response.Errors.Error = new ErrorElement[] { new ErrorElement() { ErrorCode = "", ErrorMessage = "" } };
		response.TransactionResult = new TransactionResultElement();
		response.TransactionResult.AutoAuthorResult = "OK";
		response.TransactionResult.GmoTransactionId = request.Transaction.GmoTransactionId;
		response.TransactionResult.MaulAuthorResult = "OK";
		response.TransactionResult.Reasons = new ReasonsElement();
		response.TransactionResult.Reasons.Reason = "問題なかった"; 
		Response.ContentType = "application/xml";
		Response.Write(response.CreateResponseString());
	}
	catch(Exception ex)
	{
		var response = new GmoResponseGetAuthResult();
		response.Result = ResultCode.NG;
		response.Errors = new ErrorsElement();
		response.Errors.Error = new ErrorElement[] { new ErrorElement() { ErrorCode = "9999", ErrorMessage = ex.Message } };
		response.TransactionResult = new TransactionResultElement();
		response.TransactionResult.AutoAuthorResult = "NG";
		response.TransactionResult.GmoTransactionId = "";
		response.TransactionResult.MaulAuthorResult = "NG";
		response.TransactionResult.Reasons = new ReasonsElement();
		response.TransactionResult.Reasons.Reason = "問題あり"; 
		Response.ContentType = "application/xml";
		Response.Write(response.CreateResponseString());
	}
}
</script>
<% this.Main(); %>