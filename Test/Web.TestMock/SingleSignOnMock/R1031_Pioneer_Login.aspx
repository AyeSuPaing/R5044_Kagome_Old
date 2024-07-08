<%--
=========================================================================================================
  Module      : シングルサインオンテスト用モック（R1031_Pioneer用）クラス(R1031_Pioneer_Login.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="R1031_Pioneer_Login.aspx.cs" Inherits="R1031_Pioneer_Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <div>
		<form name="form1" action="<%= this.NextUrl %>" method="Post">
			<div>
				<h1>JAF会員ログイン</h1>
				<label>ログインID：</label>
				<input id="loginId" name="loginId" value="jafloginid" style="width: 100px;" /><br/>
				<label>パスワード：</label>
				<input id="password" type="password" name="password" value="123456" style="width: 100px;" /><br/>
				<a href="javascript:form1.submit()">ログイン</a><br/>
			</div>
			<br/><hr/><br/>
			<div id="dvConfirm" runat="server" visible="true">
				<label>遷移先URL（※確認用）</label>
				<input id="nurl" name="qUrl" value="<%= this.NextUrl %>" style="width: 900px;" /><br/>
				<label>サービスID（※確認用）</label>
				<input id="timestamp" name="qServiceId" value="<%= this.ServiceId %>" style="width: 100px;" />
			</div>
		</form>
	</div>
</body>
</html>