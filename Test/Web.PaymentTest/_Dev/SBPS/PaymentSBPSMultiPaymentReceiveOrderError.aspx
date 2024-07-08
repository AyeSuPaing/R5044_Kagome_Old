<%@ Page Language="C#" ResponseEncoding="Shift_JIS" Inherits="BasePage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

	protected void Page_Load(object sender, EventArgs e)
	{
		LogWriter.Write(LogWriter.LogKbnType.SBPS, "Error.aspx", Request.Form);
	}
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		エラーになりました。
    </div>
    </form>
</body>
</html>
