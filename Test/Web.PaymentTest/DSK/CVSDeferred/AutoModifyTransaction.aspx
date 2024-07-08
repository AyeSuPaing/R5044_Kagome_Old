<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="False" %>

<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.DSKDeferred" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.DSKDeferred.OrderModify" %>
<script runat="server">
	void Main()
	{
		try
		{
			var reader = new StreamReader(Request.InputStream);
			var xml = HttpUtility.UrlDecode(reader.ReadToEnd(), Encoding.UTF8);

			var request = w2.Common.Helper.SerializeHelper.Deserialize<DskDeferredOrderModifyRequest>(xml);
			var response = new DskDeferredOrderModifyResponse();

			response.Result = "OK";
			response.TransactionResult = new TransactionResultElement();
			response.TransactionResult.TransactionId = request.Buyer.TransactionId;
			response.TransactionResult.ShopTransactionId = request.Buyer.ShopTransactionId;
			response.TransactionResult.AuthorResult = "OK";
			response.Errors = null;

			Response.ContentType = "application/xml";
			Response.Write(response.CreateResponseString());
		}
		catch (Exception ex)
		{
			var response = new DskDeferredOrderModifyResponse();
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
