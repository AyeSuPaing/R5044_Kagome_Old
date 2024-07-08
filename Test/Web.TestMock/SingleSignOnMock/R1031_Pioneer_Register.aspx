<%--
=========================================================================================================
  Module      : シングルサインオン会員登録テスト用モック（R1031_Pioneer用）クラス(R1031_Pioneer_Register.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="R1031_Pioneer_Register.aspx.cs" Inherits="SingleSignOnMock_R1031_Pioneer_Register" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
	<title></title>
</head>
<body>
	<div>
		<form name="form2" action="#">
			<div>
				<h1>JAF会員新規登録</h1>
				<label>会員番号　：</label>
				<input id="memberNo" name="memberNo" value="129889321094" style="width: 100px;" /><br />
				<label>パスワード：</label>
				<input id="regPassword" type="password" name="regPassword" value="123456" style="width: 100px;" /><br />
				<label>氏名　　　：</label>
				<input id="name" name="name" value="jafusername" style="width: 100px;" /><br />
				<a href="javascript:form2.submit()">会員登録</a><br />
				<a href="javascript:history.back();">戻る</a>
			</div>
		</form>
	</div>
</body>
</html>
