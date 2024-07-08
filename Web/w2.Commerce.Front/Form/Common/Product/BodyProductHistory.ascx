<%--
=========================================================================================================
  Module      : 商品表示履歴出力コントローラ(BodyProductHistory.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Product/BodyProductHistory.ascx.cs" Inherits="Form_Common_Product_BodyProductHistory" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
タイトルタグはカスタムパーツのみ利用します。
<%@ Page Title="無名のパーツ" %>
<%@ FileInfo LastChanged="最終更新者" %>

--%>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<%-- ▽商品表示履歴ループ▽ --%>
<asp:Repeater ID="rProductHistory" runat="server">
<HeaderTemplate>
<div id="dvCheckList">
<p class="title">最近チェックした商品</p>
</HeaderTemplate>
<ItemTemplate>
<div class="productCheckList ">
<ul class="productInfoBlock clearFix">
<li class="thumnail">
	<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem)) %>">
	<w2c:ProductImage ImageTagId="picture" ImageSize="M" ProductMaster="<%# Container.DataItem %>" IsVariation="false" runat="server" /></a>
	<%-- ▽在庫切れ可否▽ --%>
	<span visible='<%# ProductListUtility.IsProductSoldOut(Container.DataItem) %>' runat="server" class="soldout">SOLDOUT</span>
	<%-- △在庫切れ可否△ --%>
</li>
<!--
<li class="productName">
<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem)) %>">
	<%# WebSanitizer.HtmlEncode(GetProductName((string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_NAME))) %></a><br />

<%-- ▽商品会員ランク価格有効▽ --%>
<p id="Span1" visible='<%# ProductPage.GetProductMemberRankPriceValid(Container.DataItem) %>' runat="server">
	<strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></strike><br />
	<%#: CurrencyManager.ToPrice(ProductPage.GetProductMemberRankPrice(Container.DataItem)) %>
</p>
<%-- △商品会員ランク価格有効△ --%>
<%-- ▽商品セール価格有効▽ --%>
<p visible='<%# ProductPage.GetProductTimeSalesValid(Container.DataItem) %>' runat="server">
	<strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></strike><br />
	<%#: CurrencyManager.ToPrice(ProductPage.GetProductTimeSalePriceNumeric(Container.DataItem)) %>
</p>
<%-- △商品セール価格有効△ --%>
<%-- ▽商品特別価格有効▽ --%>
<p visible='<%# ProductPage.GetProductSpecialPriceValid(Container.DataItem) %>' runat="server">
	<strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></strike><br />
	<%#: CurrencyManager.ToPrice(ProductPage.GetProductSpecialPriceNumeric(Container.DataItem)) %>
</p>
<%-- △商品特別価格有効△ --%>
<%-- ▽商品通常価格有効▽ --%>
<p visible='<%# ProductPage.GetProductNormalPriceValid(Container.DataItem) %>' runat="server">
	<%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %>
</p>
<%-- △商品通常価格有効△ --%>
<%-- ▽商品定期購入価格▽ --%>
<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) {%>
<p visible='<%# (StringUtility.ToValue(ProductPage.GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG), "").ToString() != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) %>' runat="server">
	<span visible='<%# ProductPage.IsProductFixedPurchaseFirsttimePriceValid(Container.DataItem) %>' runat="server">
		定期初回:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchaseFirsttimePrice(Container.DataItem)) %>
		<br />
	</span>
	定期通常:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchasePrice(Container.DataItem)) %>
</p>
<% } %>
<%-- △商品定期購入価格△ --%>
</li>
-->
</ul>
</div>
</ItemTemplate>
<FooterTemplate>
</div>
</FooterTemplate>
</asp:Repeater>
<%-- △商品表示履歴ループ△ --%>
<%-- △編集可能領域△ --%>