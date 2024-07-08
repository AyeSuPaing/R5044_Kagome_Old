<%--
=========================================================================================================
  Module      : 商品サブ画像画面(ProductDetailSubImage.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="HeaderScriptDeclaration" Src="~/Form/Common/HeaderScriptDeclaration.ascx" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="~/Form/Product/ProductDetailSubImage.aspx.cs" Inherits="Form_Product_ProductDetailSubImage" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="ja" lang="ja">
<head id="Head1" runat="server">
	<title></title>
	<link href="../../Css/product.css" rel="stylesheet" type="text/css" media="all" />
	<%-- マウス操作処理 --%>
	<script type="text/javascript">
		$(document).ready(function () {
			$('.navi1').mouseover(
				function () { $('#caption').text($(this).attr('alt')); });
		});
	</script>
	<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
	<meta http-equiv="Pragma" content="no-cache" />
	<meta http-equiv="Cache-Control" content="no-store" />
	<meta http-equiv="Cache-Control" content="no-cache" />
	<meta http-equiv="Expires" content="-1" />
	<%-- 各種Js読み込み --%>
	<uc:HeaderScriptDeclaration id="HeaderScriptDeclaration" runat="server"></uc:HeaderScriptDeclaration>
</head>
<body>
<form id="form1" runat="server">
	<table cellpadding="0" cellspacing="0" width="620">
		<tr>
			<td align="center"><span id="caption" style="font-weight:bold; color:#666;"></span></td>
			<td>&nbsp;</td>
		</tr>
		<tr>
			<td align="left" valign="top" width="520">
				<div>
					<!-- 拡大画像 -->
					<a href="#" onclick="javascript:parent.tb_remove();">
					<img id="main_picture" src="<%= CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, this.SubImageNo) %>" alt="" />
					</a>
				</div>
			</td>
			<td align="left" valign="top" width="100">
				<div style="font-weight:bold; color:#666"><strong></strong></div>
				<!-- サブ画像一覧 -->
				<asp:Repeater DataSource="<%# this.ProductSubImageList %>" Visible="<%# (this.ProductSubImageList.Count > 0) %>" runat="server">
				<ItemTemplate>
					<asp:Literal ID="lDiv" runat="server" Visible='<%# (Container.ItemIndex % 2 == 0) %>'>
						<div>
					</asp:Literal>
					<a href="#" title="<%# Eval(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NAME) %>">
						<img class="navi1"
							src="<%# CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_M, (int)Eval(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO)) %>"
							onmouseover="change_picture('main_picture', '<%# CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)Eval(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO)) %>')"
							alt="<%# Eval(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NAME) %>" border="0" width="41" height="41"  /></a>
					<asp:Literal ID="lDiv2" runat="server" Visible='<%# ((Container.ItemIndex % 2 == 1) || (Container.ItemIndex == ((List<DataRowView>)(((Repeater)Container.Parent).DataSource)).Count - 1)) %>'>						
						</div>
					</asp:Literal>
				</ItemTemplate>
				</asp:Repeater>
			</td>
		</tr>
	</table>
</form>
</body>
</html>
