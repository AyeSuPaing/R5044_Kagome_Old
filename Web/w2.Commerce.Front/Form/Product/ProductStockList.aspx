<%--
=========================================================================================================
  Module      : 商品在庫状況一覧画面(ProductStockList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="BodyProductStockList" Src="~/Form/Common/Product/BodyProductStockList.ascx" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="~/Form/Product/ProductStockList.aspx.cs" Inherits="Form_Product_ProductStockList" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
	<meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
	<title>商品在庫一覧ページ</title>
	<link href="../../Css/common.css" rel="stylesheet" type="text/css" media="all" />
	<link href="../../Css/product.css" rel="stylesheet" type="text/css" media="all" />
</head>
<body id="ProductStock">
<div id="dvContainer">
<form id="form2" onsubmit="return (document.getElementById('__EVENTVALIDATION') != null);" runat="server">
<uc:BodyProductStockList ShopId="<%# this.ShopId %>" ProductId="<%# this.ProductId %>" DisplayPrice="true" ProductStockTitle="true" runat="server" />
</form>
<p class="btnClose">
	<a href="Javascript:window.close();" class="btn">
		閉じる</a>
</p>
</div>
</body>
</html>