<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="False" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.GMO.Zcom" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.GMO.Zcom.Sales" %>
<script runat="server">
void Main()
{
	try
	{
		var response = new ZcomSalesResponse();
		var res = new List<ZcomSalesResponse.Result>();
		res.Add(new ZcomSalesResponse.Result() { result = ZcomConst.RESULT_CODE_OK });
		response.Results = res.ToArray();
		Response.ContentType = "application/xml";
		Response.Write(response.CreateResponseString());
	}
	catch(Exception ex)
	{
		var response = new ZcomSalesResponse();
		var res = new List<ZcomSalesResponse.Result>();
		res.Add(new ZcomSalesResponse.Result() { result = ZcomConst.RESULT_CODE_SYSTEM_ERROR });
		res.Add(new ZcomSalesResponse.Result() { ErrCode = "999" });
		res.Add(new ZcomSalesResponse.Result() { ErrDetail = ex.Message });
		response.Results = res.ToArray();

		Response.ContentType = "application/xml";
		Response.Write(response.CreateResponseString());
	}
}
</script>
<% this.Main(); %>