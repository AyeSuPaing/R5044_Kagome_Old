<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="False" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.GMO" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.GMO.GetDefPaymentStatus" %>
<script runat="server">
void Main()
{
	var reader = new StreamReader(Request.InputStream);
	var xml = HttpUtility.UrlDecode(reader.ReadToEnd(), Encoding.UTF8);

	try
	{
		var request = w2.Common.Helper.SerializeHelper.Deserialize<GmoRequestGetDefPaymentStatus>(xml);
		var response = new GmoResponseGetDefPaymentStatus();
		response.Result = ResultCode.OK;
		response.Errors = new ErrorsElement();
		response.Errors.Error = new ErrorElement[] { new ErrorElement() { ErrorCode = "", ErrorMessage = "" } };
		response.TransactionResult = new TransactionResultElement();
		switch (request.Transaction.GmoTransactionId)
		{
			case "0":
				response.TransactionResult.PaymentStatus = PaymentStatusCode.NOT;
				break;
			case "1":
				response.TransactionResult.PaymentStatus = PaymentStatusCode.DEFINITE;
				break;
			case "2":
				response.TransactionResult.PaymentStatus = PaymentStatusCode.IRREGULAR;
				break;
			case "3":
				throw new ApplicationException("テスト");
				break;
			default:
				response.TransactionResult.PaymentStatus = PaymentStatusCode.PROMPT;
				break;
		}
		response.TransactionResult.GmoTransactionId = request.Transaction.GmoTransactionId;
		response.TransactionResult.PromptDate = "2020/05/09";
		response.TransactionResult.DecisionDate = "2021/05/10";
		Response.ContentType = "application/xml";
		Response.Write(response.CreateResponseString());
	}
	catch(Exception ex)
	{
		var request = w2.Common.Helper.SerializeHelper.Deserialize<GmoRequestGetDefPaymentStatus>(xml);
		var response = new GmoResponseGetDefPaymentStatus();
		response.Result = ResultCode.NG;
		response.Errors = new ErrorsElement();
		response.Errors.Error = new ErrorElement[] { new ErrorElement() { ErrorCode = "9999", ErrorMessage = ex.Message } };
		response.TransactionResult = new TransactionResultElement();
		response.TransactionResult.PaymentStatus = PaymentStatusCode.PROMPT;
		response.TransactionResult.GmoTransactionId = request.Transaction.GmoTransactionId;
		Response.ContentType = "application/xml";
		Response.Write(response.CreateResponseString());
	}
}
</script>
<% this.Main(); %>