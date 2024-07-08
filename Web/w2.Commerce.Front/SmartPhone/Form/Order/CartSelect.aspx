<%--
=========================================================================================================
  Module      : スマートフォン用ログイン後カート選択画面(CartSelect.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Register TagPrefix="uc" TagName="CartSelect_ProductItem" Src="~/SmartPhone/Form/Order/CartSelect_ProductItem.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/Order/CartSelect.aspx.cs" Inherits="Form_Order_CartSelect" Title="カート商品選択ページ" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<meta http-equiv="pragma" content="no-cache" />
<link href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "SmartPhone/Css/order.css") %>" rel="stylesheet" type="text/css" media="all" />
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<%-- △編集可能領域△ --%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="divTopArea">
<%-- ▽レイアウト領域：トップエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<section class="wrap-cart cart-select">
	<div class="cart-unit">
	<h2>カート選択</h2>
	<p class="msg">お客様の以前のカート情報が残っております。<br />カート内の商品を選択後次へ進んでください。</p>
	<h3>現在カートに入っている商品</h3>

	<!--▽ 現在カート選択 ▽-->
	<asp:Repeater id="rProductList" runat="server">
		<HeaderTemplate>
			<table class="cart-table">
			<tbody>
		</HeaderTemplate>
		<ItemTemplate>
			<%-- 現在カートに入っている商品 --%>
			<uc:CartSelect_ProductItem id="ucCartSelect_ProductItem" CartProduct="<%# Container.DataItem %>" DefaultChecked="true" runat="server"></uc:CartSelect_ProductItem>
			
			<%-- 隠し値 --%>
			<asp:HiddenField ID="hfIsSetItem" runat="server" Value="<%# ((CartProduct)Container.DataItem).IsSetItem %>" />
			<asp:HiddenField ID="hfShopId" runat="server" Value="<%# ((CartProduct)Container.DataItem).ShopId %>" />
			<asp:HiddenField ID="hfProductId" runat="server" Value="<%# ((CartProduct)Container.DataItem).ProductId %>" />
			<asp:HiddenField ID="hfVariationId" runat="server" Value="<%# ((CartProduct)Container.DataItem).VariationId %>" />
			<asp:HiddenField ID="hfIsFixedPurchase" runat="server" Value="<%# ((CartProduct)Container.DataItem).IsFixedPurchase %>" />
			<asp:HiddenField ID="hfAddCartKbn" runat="server" Value="<%# ((CartProduct)Container.DataItem).AddCartKbn %>" />
			<asp:HiddenField ID="hfProductSaleId" runat="server" Value="<%# ((CartProduct)Container.DataItem).ProductSaleId %>" />
			<asp:HiddenField ID="hfProductSetId" runat="server" Value="<%# OrderPage.GetProductSetId(((CartProduct)Container.DataItem)) %>" />
			<asp:HiddenField ID="hfProductSetNo" runat="server" Value="<%# OrderPage.GetProductSetNo(((CartProduct)Container.DataItem)) %>" />
			<asp:HiddenField ID="hfProductSetItemNo" runat="server" Value="<%# ((CartProduct)Container.DataItem).ProductSetItemNo %>" />
			<asp:HiddenField ID="hfProductSetName" runat="server" Value="<%# OrderPage.GetProductSetName(((CartProduct)Container.DataItem)) %>" />
			<asp:HiddenField ID="hfProductOptionSettingList" runat="server" Value="<%# ((CartProduct)Container.DataItem).ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues() %>" />
		</ItemTemplate>
		<FooterTemplate>
			</tbody>
			</table>
		</FooterTemplate>
		</asp:Repeater>
		<!--△ 現在カート選択 △-->

		<!--▽ 前回カート選択 ▽-->
		<asp:Repeater id="rProductListBefore" runat="server">
		<HeaderTemplate>
			<table class="cart-table">
				<th class="cart-title-product-list-before" colspan="2">
					<h3 >以前カートに入れた商品</h3>
				</th>
				<th class="product-control-all">
					<span class="checkBox">全選択<br /><input id="checkboxAddAllBeforeProduct" type="checkbox" onchange="addAllBeforeProduct()"/></span>
				</th>
			<tbody>
		</HeaderTemplate>
		<ItemTemplate>

			<%-- 以前カートに入れた商品 --%>
			<uc:CartSelect_ProductItem id="ucCartSelect_ProductItem" CartProduct="<%# Container.DataItem %>" DefaultChecked="false" runat="server"></uc:CartSelect_ProductItem>
			
			<%-- 隠し値 --%>
			<asp:HiddenField ID="hfIsSetItem" runat="server" Value="<%# ((CartProduct)Container.DataItem).IsSetItem %>" />
			<%if (Constants.CARTCOPY_OPTION_ENABLED){ %>
			<asp:HiddenField ID="hfCartId" runat="server" Value="<%# ((CartProduct)Container.DataItem).CartId %>" />
			<%} %>
			<asp:HiddenField ID="hfShopId" runat="server" Value="<%# ((CartProduct)Container.DataItem).ShopId %>" />
			<asp:HiddenField ID="hfProductId" runat="server" Value="<%# ((CartProduct)Container.DataItem).ProductId %>" />
			<asp:HiddenField ID="hfVariationId" runat="server" Value="<%# ((CartProduct)Container.DataItem).VariationId %>" />
			<asp:HiddenField ID="hfAddCartKbn" runat="server" Value="<%# ((CartProduct)Container.DataItem).AddCartKbn %>" />
			<asp:HiddenField ID="hfIsFixedPurchase" runat="server" Value="<%# ((CartProduct)Container.DataItem).IsFixedPurchase %>" />
			<asp:HiddenField ID="hfProductSaleId" runat="server" Value="<%# ((CartProduct)Container.DataItem).ProductSaleId %>" />
			<asp:HiddenField ID="hfProductSetId" runat="server" Value="<%# OrderPage.GetProductSetId(((CartProduct)Container.DataItem)) %>" />
			<asp:HiddenField ID="hfProductSetNo" runat="server" Value="<%# OrderPage.GetProductSetNo(((CartProduct)Container.DataItem)) %>" />
			<asp:HiddenField ID="hfProductSetItemNo" runat="server" Value="<%# ((CartProduct)Container.DataItem).ProductSetItemNo %>" />
			<asp:HiddenField ID="hfProductSetName" runat="server" Value="<%# OrderPage.GetProductSetName(((CartProduct)Container.DataItem)) %>" />
			<asp:HiddenField ID="hfProductOptionSettingList" runat="server" Value="<%# ((CartProduct)Container.DataItem).ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues() %>" />

		</ItemTemplate>
		<FooterTemplate>
			</tbody>
			</table>
		</FooterTemplate>
		</asp:Repeater>
		<!--△ 前回カート選択 △-->
	</div>

	<div class="cart-footer">
		<div class="button-next">
			<asp:LinkButton ID="lbNext" CssClass="btn" runat="server" OnClick="lbNext_Click">次へ進む</asp:LinkButton>
		</div>
	</div>
	
</section>
<script>
	function addAllBeforeProduct() {
		var chk = document.getElementById("checkboxAddAllBeforeProduct").checked;
		$("input[name*=rProductListBefore]input[type=checkbox]").each(function () {
			$(this).prop("checked", chk);
		});
	}
</script>
<%-- △編集可能領域△ --%>

<div id="divBottomArea">
<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>
</asp:Content>