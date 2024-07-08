<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CrossPointAppMock.aspx.cs" Inherits="CrossPoint_CrossPointAppMock" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
	<form id="form1" action="http://localhost/V5.14-CrossPoint/Web/w2.Commerce.Front/Form/User/UserRegistRegulation.aspx" method="post">
		<div>
			<input id="APP_KEY" name="APP_KEY" value="<%= this.AppKey %>" />
		</div>
		<div>
			<input id="MEMBER_ID" name="MEMBER_ID" value="<%= this.MemberId %>"/>
		</div>
		<div>
			<input id="PIN_CD" name="PIN_CD" value="<%= this.PinCd %>"/>
		</div>
		<div>
			<input type="submit" value="送信"/>
		</div>
	</form>
</body>
</html>
