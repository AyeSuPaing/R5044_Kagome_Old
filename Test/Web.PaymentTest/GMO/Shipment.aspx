<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="False" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.GMO" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.GMO.Shipment" %>
<script runat="server">
void Main()
{
	StreamReader reader = new StreamReader(Request.InputStream);
	string xml = HttpUtility.UrlDecode(reader.ReadToEnd(), Encoding.UTF8);

	try
	{
		var request = w2.Common.Helper.SerializeHelper.Deserialize<GmoRequestShipment>(xml);
		var response = new GmoResponseShipment();
		response.Result = ResultCode.OK;
		response.Errors = new ErrorsElement();
		response.Errors.Error = new ErrorElement[] { new ErrorElement() { ErrorCode = "", ErrorMessage = "" } };
		response.TransactionInfo = new TransactionInfoElement();
		response.TransactionInfo.GmoTransactionId = request.Transaction.GmoTransactionId;
		Response.ContentType = "application/xml";
		Response.Write(response.CreateResponseString());
	}
	catch(Exception ex)
	{
		var response = new GmoResponseShipment();
		response.Result = ResultCode.NG;
		response.Errors = new ErrorsElement();
		response.Errors.Error = new ErrorElement[] { new ErrorElement() { ErrorCode = "9999", ErrorMessage = ex.Message } };
		response.TransactionInfo = new TransactionInfoElement();
		response.TransactionInfo.GmoTransactionId = "";
		Response.ContentType = "application/xml";
		Response.Write(response.CreateResponseString());
	}
}
</script>
<% this.Main(); %>