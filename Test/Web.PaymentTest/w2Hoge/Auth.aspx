<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="False" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="Test.HogePaymentLib" %>
<%@ Import Namespace="Test.HogePaymentLib.Auth" %>
<script runat="server">
	void Main()
	{
		StreamReader reader = new StreamReader(Request.InputStream);
		string xml = HttpUtility.UrlDecode(reader.ReadToEnd(), Encoding.UTF8);

		try
		{
			var request = HogeHelper.Deserialize<AuthRequest>(xml);
			if (request.Amount == 1000) { throw new Exception(); }
			var response = new AuthResponse();
			response.ResultCode = "OK";
			response.TransactionID = "T" + request.OrderId;
			response.TransactionKey = Guid.NewGuid().ToString("N");
			Response.ContentType = "application/xml";
			Response.Write(HogeHelper.Serialize(response));
		}
		catch
		{
			var response = new AuthResponse();
			response.ResultCode = "NG";
			response.TransactionID = "";
			response.TransactionKey = "";
			Response.ContentType = "application/xml";
			Response.Write(HogeHelper.Serialize(response));
		}
	}
</script>
<% this.Main(); %>
