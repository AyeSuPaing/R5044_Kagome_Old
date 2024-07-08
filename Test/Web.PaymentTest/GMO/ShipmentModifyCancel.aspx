<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="False" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.GMO" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.GMO.ShipmentModifyCancel" %>
<script runat="server">
void Main()
{
	StreamReader reader = new StreamReader(Request.InputStream);
	string xml = HttpUtility.UrlDecode(reader.ReadToEnd(), Encoding.UTF8);

	try
	{
		var request = w2.Common.Helper.SerializeHelper.Deserialize<GmoRequestShipmentModifyCancel>(xml);
		var response = new GmoResponseShipmentModifyCancel();
		response.Result = ResultCode.OK;
		response.Errors = new ErrorsElement();
		response.Errors.Error = new ErrorElement[] { new ErrorElement() { ErrorCode = "", ErrorMessage = "" } };
		response.TransactionResult = new transactionResultElement();
		response.TransactionResult.GmoTransactionId = request.Transaction.GmoTransactionId;
		Response.ContentType = "application/xml";
		Response.Write(response.CreateResponseString());
	}
	catch(Exception ex)
	{
		var response = new GmoResponseShipmentModifyCancel();
		response.Result = ResultCode.NG;
		response.Errors = new ErrorsElement();
		response.Errors.Error = new ErrorElement[] { new ErrorElement() { ErrorCode = "9999", ErrorMessage = ex.Message } };
		response.TransactionResult = new transactionResultElement();
		response.TransactionResult.GmoTransactionId = "";
		Response.ContentType = "application/xml";
		Response.Write(response.CreateResponseString());
	}
}
</script>
<% this.Main(); %>