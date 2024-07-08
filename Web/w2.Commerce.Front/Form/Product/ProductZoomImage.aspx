<%--
=========================================================================================================
  Module      : 商品詳細画面(ProductZoomImage.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="~/Form/Product/ProductZoomImage.aspx.cs" Inherits="Form_Product_ProductZoomImage" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>拡大写真</title>
	<link href="../../Css/common.css" rel="stylesheet" type="text/css" media="all" />
	<link href="../../Css/product.css" rel="stylesheet" type="text/css" media="all" />
	<%-- マウス操作処理 --%>
	<script type="text/javascript">
		$(document).ready(function () {
			$('.navi1').mouseover(
				function () { $('#caption').text($(this).attr('alt')); });

			$('.navi2').mouseover(
				function () { $('#caption').text($(this).attr('alt')); });
		});
	</script>
</head>
<body id="ProductZoomImage" onclick="javascript:window.close()">
<div id="ProductZoomImageWrap">
	<form id="form1" runat="server">
	<table cellpadding="0" cellspacing="0" width="900">
		<tr>
			<td align="left" valign="top" width="700">
				<div class="main">
					<a href="#" onclick="javascript:parent.tb_remove();">
					<% if (this.SubImageNo == 0) { %>
						<w2c:ProductImage ID="ProductImage1" IsVariation="True" AltString="" ImageTagId="main_picture" ImageSize="LL" ProductMaster="<%# this.ProductImageInfo %>" runat="server" />
					<% } else { %>
						<img id="main_picture" src="<%# WebSanitizer.HtmlEncode(CreateProductSubImageUrl(this.ImageHead, Constants.PRODUCTIMAGE_FOOTER_LL, this.SubImageNo)) %>" alt="" />
					<% } %>
					<span id="caption"></span>
					</a>
				</div>
			</td>
			<td align="right" valign="top" width="200">
				<div class="thumb">
				<div class="title">バリエーション</div>
				<div>
					<asp:Repeater ID="rVariation" runat="server" >
						<ItemTemplate>
							<img class="navi1"
								src="<%# WebSanitizer.HtmlEncode(CreateVariationImagePath(Container.DataItem, Constants.PRODUCTIMAGE_FOOTER_M)) %>" 
								onmouseover="change_picture('main_picture', '<%# WebSanitizer.HtmlEncode(CreateVariationImagePath(Container.DataItem, Constants.PRODUCTIMAGE_FOOTER_LL)) %>')"
								alt="<%# WebSanitizer.HtmlEncode(GetVariationValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1)) %>"
								title=""
								width="84" />
						</ItemTemplate>
					</asp:Repeater>
				</div>
				<div class="title">詳細</div>
				<div>
					<asp:Repeater ID="rSubImage" runat="server" >
						<ItemTemplate>
							<img  class="navi2"
								src="<%# WebSanitizer.HtmlEncode(CreateProductSubImageUrl(this.ImageHead, Constants.PRODUCTIMAGE_FOOTER_M, (int)Eval(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO))) %>"
								onmouseover="change_picture('main_picture', '<%# WebSanitizer.HtmlEncode(CreateProductSubImageUrl(this.ImageHead, Constants.PRODUCTIMAGE_FOOTER_LL, (int)Eval(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO))) %>')"
								alt="" 
								width="84"/>
						</ItemTemplate>
					</asp:Repeater>
				</div>
				</div>
			</td>
		</tr>
	</table>
	</form>
</div>
</body>
</html>