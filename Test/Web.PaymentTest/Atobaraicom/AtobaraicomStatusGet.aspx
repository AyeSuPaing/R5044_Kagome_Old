<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="False" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.Atobaraicom.OrderStatus" %>
<%@ Import Namespace="w2.Domain.Order" %>
<script runat="server">
	void Main()
	{
		var reader = new StreamReader(Request.InputStream);
		var request = HttpUtility.UrlDecode(reader.ReadToEnd(), Encoding.UTF8);

		var response = "<?xml version=\"1.0\" encoding=\"utf-8\"?><response><status>success</status><messages/><results>";

		foreach (var item in request.Split('&'))
		{
			if (item.Contains("OrderId[]"))
			{
				var result = item.Split('=');
				var order = new OrderService().GetOrderByPaymentOrderId(result[1]);
				response += "<result><entOrderId>" + order.OrderId + "</entOrderId><orderId>" + result[1] + "</orderId><orderStatus orderStatusName=\"クローズ\">9</orderStatus></result>";
			}
		}
		response += "</results></response>";
		Response.ContentType = "application/xml";
		Response.Write(response);
	}
</script>
<% this.Main(); %>