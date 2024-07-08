<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="False" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.GMO.Zcom" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.GMO.Zcom.Cancel" %>
<script runat="server">
void Main()
{
	try
	{
		var response = new ZcomCancelResponse();
		var res = new List<ZcomCancelResponse.Result>();
		res.Add(new ZcomCancelResponse.Result(){result= ZcomConst.RESULT_CODE_OK});
		response.Results = res.ToArray();
		Response.ContentType = "application/xml";
		Response.Write(response.CreateResponseString());
	}
	catch(Exception ex)
	{
		var response = new ZcomCancelResponse();
		var res = new List<ZcomCancelResponse.Result>();
		res.Add(new ZcomCancelResponse.Result() { result = ZcomConst.RESULT_CODE_SYSTEM_ERROR });
		res.Add(new ZcomCancelResponse.Result() { ErrCode = "999" });
		res.Add(new ZcomCancelResponse.Result() { ErrDetail = ex.Message });
		response.Results = res.ToArray();

		Response.ContentType = "application/xml";
		Response.Write(response.CreateResponseString());
	}
}
</script>
<% this.Main(); %>