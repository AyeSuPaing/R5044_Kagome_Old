<%--
=========================================================================================================
  Module      : ログイン後カート選択画面(CartSelect.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Register TagPrefix="uc" TagName="CartSelect_ProductItem" Src="~/Form/Order/CartSelect_ProductItem.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/Order/CartSelect.aspx.cs" Inherits="Form_Order_CartSelect" Title="カート商品選択ページ" %>
<%@ Import Namespace="System.ComponentModel" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<meta http-equiv="pragma" content="no-cache">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<%-- △編集可能領域△ --%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table id="tblLayout">
<tr>
<td>
<%-- ▽レイアウト領域：レフトエリア▽ --%>
<%-- △レイアウト領域△ --%>
</td>
<td>
<div id="divTopArea">
<%-- ▽レイアウト領域：トップエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<div id="dvOrderBox">
	<div id="dvCartSelect">
		<h2><span>
			<img src="../../Contents/ImagesPkg/order/h2_cart_select.gif" alt="カート選択" /></span></h2>

		<p>お客様の以前のカート情報が残っております。<br />カート内の商品を選択後次へ進んでください。</p>
		<h3><img src="../../Contents/ImagesPkg/order/h3_current_cart.gif" alt="現在カートに入っている商品" /></h3>

		<table cellspacing="0">
		<!--▽ 現在カート選択 ▽-->
		<asp:Repeater id="rProductList" runat="server">
		<HeaderTemplate>
			<tr>
				<th class="productImg">商品画像</th>
				<th class="productName">商品名</th>
				<th class="productPrice">商品価格</th>
				<%if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED && IsDisplayOption(this.CartProductsNow)) { %>
					<th class="productPrice">オプション価格</th>
				<% } %>
				<th class="remark">選択</th>
			</tr>
		</HeaderTemplate>
		<ItemTemplate>
		
			<%-- 現在カートに入っている商品 --%>
			<uc:CartSelect_ProductItem id="ucCartSelect_ProductItem" CartProducts="<%# this.CartProductsNow%>" CartProduct="<%# Container.DataItem %>" DefaultChecked="true" runat="server"></uc:CartSelect_ProductItem>
			
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
		</asp:Repeater>
		<!--△ 現在カート選択 △-->
		</table>

		<h3><img src="../../Contents/ImagesPkg/order/h3_prev_cart.gif" alt="以前カートに入れた商品" /></h3>

		<table cellspacing="0">
		<!--▽ 前回カート選択 ▽-->
		<asp:Repeater id="rProductListBefore" runat="server">
		<HeaderTemplate>
			<tr>
				<th class="productImg">商品画像</th>
				<th class="productName">商品名</th>
				<th class="productPrice">商品価格</th>
				<%if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED && IsDisplayOption(this.CartProductsBefore)){ %>
					<th class="productPrice">オプション価格</th>
				<% } %>
				<th class="remark"><a class="addCheckAll" href="#" onclick="controlCheckAllBeforeProduct(true)">全選択</a>／<a class="removeCheckAll" href="#" onclick="controlCheckAllBeforeProduct(false)">全解除</a></th>
			</tr>
		</HeaderTemplate>
		<ItemTemplate>

			<%-- 以前カートに入れた商品 --%>
			<uc:CartSelect_ProductItem id="ucCartSelect_ProductItem" CartProducts="<%# this.CartProductsBefore %>" CartProduct="<%# Container.DataItem %>" DefaultChecked="false" runat="server"></uc:CartSelect_ProductItem>
			
			<%-- 隠し値 --%>
			<asp:HiddenField ID="hfIsSetItem" runat="server" Value="<%# ((CartProduct)Container.DataItem).IsSetItem %>" />
			<%if (Constants.CARTCOPY_OPTION_ENABLED){ %>
			<asp:HiddenField ID="hfCartId" runat="server" Value="<%# ((CartProduct)Container.DataItem).CartId %>" />
			<%} %>
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
		</asp:Repeater>
		<!--△ 前回カート選択 △-->
		</table>
	</div>
	<div class="dvOrderBtnBox">
		<div class="btmbtn">
		<p class="alignC"><asp:LinkButton ID="lbNext" runat="server" OnClick="lbNext_Click" class="btn btn-large btn-success">次へ進む</asp:LinkButton></p>
		</div>
	</div>
</div>
<script>
	function controlCheckAllBeforeProduct(flg) {
		$("input[name*=rProductListBefore]input[type=checkbox]").each(function () {
			$(this).prop("checked", flg);
		});
	}
</script>
<%-- △編集可能領域△ --%>

<div id="divBottomArea">
<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>

</td>
<td>
<%-- ▽レイアウト領域：ライトエリア▽ --%>
<%-- △レイアウト領域△ --%>
</td>
</tr>
</table>
</asp:Content>
