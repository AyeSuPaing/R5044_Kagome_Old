<%--
=========================================================================================================
  Module      : Receiving Store For Response(ReceivingStoreForResponse.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReceivingStoreForResponse.aspx.cs" Inherits="Form_Order_ReceivingStoreForResponse" %>
<html>
	<head>
		<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
	</head>
	<body>
		<input type="hidden" id="storeCode" value="<%= this.StoreCode %>">
		<input type="hidden" id="storeName" value="<%= this.StoreName %>">
		<input type="hidden" id="storeAddr" value="<%= this.StoreAddr %>">
		<input type="hidden" id="storeTel" value="<%= this.StoreTel %>">
	</body>

	<script>
		window.opener.setConvenienceStoreData(
			document.querySelector("#storeCode").value,
			document.querySelector("#storeName").value,
			document.querySelector("#storeAddr").value,
			document.querySelector("#storeTel").value);

		window.close();
	</script>
</html>