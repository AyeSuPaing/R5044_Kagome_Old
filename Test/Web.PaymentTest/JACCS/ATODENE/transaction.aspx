<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="False" %>

<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.JACCS.ATODENE" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.JACCS.ATODENE.Transaction" %>
<script runat="server">
	void Main()
	{
		try
		{
			StreamReader reader = new StreamReader(Request.InputStream);
			string xml = HttpUtility.UrlDecode(reader.ReadToEnd(), Encoding.UTF8);

			var request = w2.Common.Helper.SerializeHelper.Deserialize<AtodeneTransactionRequest>(xml);
			var response = new AtodeneTransactionResponse();

			response.Result = "OK";
			response.TransactionInfo = new AtodeneTransactionResponse.TransactionInfoElement();
			response.TransactionInfo.ShopOrderId = request.Customer.ShopOrderId;
			response.TransactionInfo.TransactionId = DateTime.Now.ToString("yyyyMMddHHmmssfff");
			response.TransactionInfo.AutoAuthoriresult = "OK";

			Response.ContentType = "application/xml";
			Response.Write(response.CreateResponseString());
		}
		catch (Exception ex)
		{
			var response = new AtodeneTransactionResponse();
			response.Result = "NG";
			response.Errors = new BaseAtodeneResponse.ErrorsElement();
			if (ex.InnerException != null)
				response.Errors.Error = new BaseAtodeneResponse.ErrorElement[] { new BaseAtodeneResponse.ErrorElement()
				{
					ErrorCode = "9999",
					ErrorMessage = ex.Message + " " + ((ex.InnerException == null) ? "" :  ex.InnerException.Message),
					ErrorPoint = ""
				} };
			Response.ContentType = "application/xml";
			Response.Write(response.CreateResponseString());
		}
	}
</script>
<% this.Main(); %>
