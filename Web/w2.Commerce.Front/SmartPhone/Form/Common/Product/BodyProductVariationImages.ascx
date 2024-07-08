<%--
=========================================================================================================
  Module      : 商品バリエーション画像レイヤー出力コントローラ(BodyProductVariationImages.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="c#" AutoEventWireup="true" Inherits="Form_Common_Product_BodyProductVariationImages" CodeFile="~/Form/Common/Product/BodyProductVariationImages.ascx.cs" %>
<%--
下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LastChanged="最終更新者" %>
--%>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<% if(Constants.VARIATION_FAVORITE_CORRESPONDENCE) {%>
	<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(this.ProductMaster, StringUtility.ToEmpty(GetKeyValueToNull(this.ProductMaster, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)))) %>'>
<w2c:ProductImage IsVariation="<%# this.IsColorVriationDisplay %>" ImageSize="<%#: this.ImageSize %>" ImageTagId="picture" ProductMaster="<%# this.ProductMaster %>" runat="server" />
</a>
<%-- ▽商品バリエーション使用時▽ --%>
<div class="variation" visible="<%# IsUseHover() %>" runat="server">
	<asp:Repeater ID="rProdcutMaster" DataSource="<%# CreateProductVariationMaster() %>" runat="server">
	<HeaderTemplate>
	</HeaderTemplate>
	<ItemTemplate>
		<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailSelectedVariationUrl(Container.DataItem)) %>">
			<img id="<%# WebSanitizer.HtmlEncode(CreateImageTagId(Container.DataItem)) %>"
				src="<%# WebSanitizer.UrlAttrHtmlEncode(CreateImageUrl(Container.DataItem)) %>"
				alt="<%# WebSanitizer.HtmlEncode(CreateAltString(Container.DataItem))%>" />
		</a>
	</ItemTemplate>
	<FooterTemplate>
	</FooterTemplate>
	</asp:Repeater>
</div>
<%-- △商品バリエーション使用時△ --%>
<% }else{ %>
<%-- ▽商品バリエーション使用時▽ --%>
<div class="variation" visible="<%# IsUseVariation() %>" runat="server">
	<asp:Repeater ID="rProdcutVariationMaster" DataSource="<%# CreateProductVariationMaster() %>" runat="server">
	<HeaderTemplate>
	</HeaderTemplate>
	<ItemTemplate>
		<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailSelectedVariationUrl(Container.DataItem)) %>">
			<img id="<%# WebSanitizer.HtmlEncode(CreateImageTagId(Container.DataItem)) %>"
				src="<%# WebSanitizer.UrlAttrHtmlEncode(CreateImageUrl(Container.DataItem)) %>"
				alt="<%# WebSanitizer.HtmlEncode(CreateAltString(Container.DataItem))%>" />
		</a>
	</ItemTemplate>
	<FooterTemplate>
	</FooterTemplate>
	</asp:Repeater>
</div>
<%-- △商品バリエーション使用時△ --%>

<%-- ▽商品バリエーション未使用時▽ --%>
<%if (Constants.LAYER_DISPLAY_NOVARIATION_UNDISPLAY_ENABLED == false) { %>
<div visible="<%# (IsUseVariation() == false) %>" runat="server" >
	<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailSelectedVariationUrl(this.ProductMaster)) %>">
		<img id="<%# WebSanitizer.HtmlEncode(CreateImageTagId(this.ProductMaster)) %>"
				src="<%# WebSanitizer.UrlAttrHtmlEncode(CreateImageUrl(this.ProductMaster, true)) %>"
			alt="<%# WebSanitizer.HtmlEncode(CreateAltString(this.ProductMaster))%>" />
	</a>
</div>
<% } %>
<%-- △商品バリエーション未使用時△ --%>
<% } %>
<%-- △編集可能領域△ --%>