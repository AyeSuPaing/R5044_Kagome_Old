<%--
=========================================================================================================
  Module      : 商品在庫状況一覧画面(ProductStockList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="HeaderScriptDeclaration" Src="~/SmartPhone/Form/Common/HeaderScriptDeclaration.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductStockList" Src="~/SmartPhone/Form/Common/Product/BodyProductStockList.ascx" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="~/Form/Product/ProductStockList.aspx.cs" Inherits="Form_Product_ProductStockList" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
	<title>商品在庫一覧ページ</title>
	<%-- SmartPhone用にViewPortを最適化。--%>
	<meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no">
	<meta http-equiv="Content-Script-Type" content="text/javascript" />
	<%-- 各種Js読み込み --%>
	<uc:HeaderScriptDeclaration id="HeaderScriptDeclaration" runat="server"></uc:HeaderScriptDeclaration>
	<%
		lProductCss.Href = Constants.PATH_ROOT + "SmartPhone/Css/product.css";
	%>
	<link id="lProductCss" rel="stylesheet" type="text/css" media="screen,print" runat="server" />
</head>
<body>
<section class="wrap-product-stock-list">
	<div class="order-unit">
		<form id="form2" onsubmit="return (document.getElementById('__EVENTVALIDATION') != null);" runat="server">
		<uc:BodyProductStockList ShopId="<%# this.ShopId %>" ProductId="<%# this.ProductId %>" DisplayPrice="true" ProductStockTitle="true" runat="server" /></form>
	</div>
	<div class="unit"><a href="Javascript:window.close();" class="btn">閉じる</a></div>
</section>
</body>
</html>