<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="False" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="Test.HogePaymentLib" %>
<%@ Import Namespace="Test.HogePaymentLib.Sale" %>
<script runat="server">
	void Main()
	{
		StreamReader reader = new StreamReader(Request.InputStream);
		string xml = HttpUtility.UrlDecode(reader.ReadToEnd(), Encoding.UTF8);

		try
		{
			var request = HogeHelper.Deserialize<SaleRequest>(xml);
			if (request.Amount == 2000) { throw new Exception(); }
			var response = new SaleResponse();
			response.ResultCode = "OK";
			Response.ContentType = "application/xml";
			Response.Write(HogeHelper.Serialize(response));
		}
		catch
		{
			var response = new SaleResponse();
			response.ResultCode = "NG";
			Response.ContentType = "application/xml";
			Response.Write(HogeHelper.Serialize(response));
		}
	}
</script>
<% this.Main(); %>

