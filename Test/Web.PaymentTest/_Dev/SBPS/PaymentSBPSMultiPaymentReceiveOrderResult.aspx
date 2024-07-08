<%@ Import Namespace="w2.App.Common.Order" %>
<%@ Page Language="C#" ResponseEncoding="Shift_JIS" Inherits="BasePage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

	protected void Page_Load(object sender, EventArgs e)
	{
		if (Request.Form.Keys.Count != 0)
		{
			PaymentSBPSMultiPaymentReceiverOrderResult receiveOrderResult = new PaymentSBPSMultiPaymentReceiverOrderResult(Request.Form);

			if (receiveOrderResult.Action() == false)
			{
				// エラー
			}
			lOrderId.Text = receiveOrderResult.PaymentOrderId + "(" + Request["item_name"] + ")";

			// TODO:注文完了ページにリダイレクト？
		}
	}
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>

	<asp:Literal ID="lOrderId" runat="server"></asp:Literal> の注文が完了しました。

</body>
</html>
