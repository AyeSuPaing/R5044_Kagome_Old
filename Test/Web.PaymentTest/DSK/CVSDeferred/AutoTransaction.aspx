<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="False" %>

<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.DSKDeferred" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.DSKDeferred.OrderRegister" %>
<script runat="server">
	void Main()
	{
		try
		{
			var reader = new StreamReader(Request.InputStream);
			var xml = HttpUtility.UrlDecode(reader.ReadToEnd(), Encoding.UTF8);

			var request = w2.Common.Helper.SerializeHelper.Deserialize<DskDeferredOrderRegisterRequest>(xml);
			var response = new DskDeferredOrderRegisterResponse();

			response.Result = "OK";
			response.TransactionResult = new TransactionResultElement();
			response.TransactionResult.TransactionId = "DSK_" + request.Buyer.ShopTransactionId;
			response.TransactionResult.AuthorResult = "OK";
			response.Errors = null;

			Response.ContentType = "application/xml";
			Response.Write(response.CreateResponseString());
		}
		catch (Exception ex)
		{
			var response = new DskDeferredOrderRegisterResponse();
			response.Result = "NG";
			response.Errors = new BaseDskDeferredResponse.ErrorsElement();
			if (ex.InnerException != null)
				response.Errors.Error = new BaseDskDeferredResponse.ErrorElement[] { new BaseDskDeferredResponse.ErrorElement()
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
