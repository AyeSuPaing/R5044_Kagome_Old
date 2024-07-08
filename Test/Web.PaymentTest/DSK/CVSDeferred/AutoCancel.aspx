<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="False" %>

<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.DSKDeferred" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.DSKDeferred.OrderCancel" %>
<script runat="server">
	void Main()
	{
		try
		{
			var reader = new StreamReader(Request.InputStream);
			var xml = HttpUtility.UrlDecode(reader.ReadToEnd(), Encoding.UTF8);

			var request = w2.Common.Helper.SerializeHelper.Deserialize<DskDeferredOrderCancelRequest>(xml);
			var response = new DskDeferredOrderCancelResponse();

			response.Result = "OK";
			response.TransactionInfo = new TransactionInfoElement();
			response.TransactionInfo.TransactionId = request.Transaction.TransactionId;
			response.TransactionInfo.ShopTransactionId = request.Transaction.ShopTransactionId;
			response.Errors = null;

			Response.ContentType = "application/xml";
			Response.Write(response.CreateResponseString());
		}
		catch (Exception ex)
		{
			var response = new DskDeferredOrderCancelResponse();
			response.Result = "NG";
			response.Errors = new BaseDskDeferredResponse.ErrorsElement();
			if (ex.InnerException != null)
				response.Errors.Error = new [] { new BaseDskDeferredResponse.ErrorElement()
				{
					ErrorCode = "9999",
					ErrorMessage = ex.Message + " " + ((ex.InnerException == null) ? "" :  ex.InnerException.Message),
				} };
			else
			{
				response.Errors.Error = new[] { new BaseDskDeferredResponse.ErrorElement()
				{
					ErrorCode = "9999",
					ErrorMessage = "予期せぬエラーが発生しました。",
				}};
			}
			Response.ContentType = "application/xml";
			Response.Write(response.CreateResponseString());
		}
	}
</script>
<% this.Main(); %>
