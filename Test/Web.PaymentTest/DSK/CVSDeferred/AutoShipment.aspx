<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="False" %>

<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.DSKDeferred" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.DSKDeferred.Shipment" %>
<script runat="server">
	void Main()
	{
		try
		{
			var reader = new StreamReader(Request.InputStream);
			var xml = HttpUtility.UrlDecode(reader.ReadToEnd(), Encoding.UTF8);

			var request = w2.Common.Helper.SerializeHelper.Deserialize<DskDeferredShipmentRequest>(xml);
			var response = new DskDeferredShipmentResponse();

			response.Result = "OK";
			response.TransactionInfoResult = new TransactionInfoResultElement();
			response.TransactionInfoResult.TransactionId = request.Transaction.TransactionId;
			response.TransactionInfoResult.ShopTransactionId = request.Transaction.ShopTransactionId;
			response.Errors = null;

			Response.ContentType = "application/xml";
			Response.Write(response.CreateResponseString());
		}
		catch (Exception ex)
		{
			var response = new DskDeferredShipmentResponse();
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
