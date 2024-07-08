<%--
=========================================================================================================
  Module      : 商品バリエーション画像レイヤー出力コントローラ(BodyProductVariationImages.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="c#" AutoEventWireup="true" Inherits="Form_Common_Product_BodyProductVariationImages" CodeFile="~/Form/Common/Product/BodyProductVariationImages.ascx.cs" %>
<%@ Import Namespace="System.ComponentModel" %>
<% if(Constants.VARIATION_FAVORITE_CORRESPONDENCE) {%>
<div>
<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(this.ProductMaster, StringUtility.ToEmpty(GetKeyValueToNull(this.ProductMaster, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)))) %>'>
	<w2c:ProductImage IsVariation="<%# this.IsColorVriationDisplay %>" ImageSize="<%#: this.ImageSize %>" ImageTagId="picture" ProductMaster="<%# this.ProductMaster %>" runat="server" />
</a>
<%-- ▽商品バリエーション使用時▽ --%>
<div visible="<%# IsUseHover() %>" runat="server" >
<div id="variationview1_<%# this.VariationNo %>" class="variationview_wrap">
<p class="variationview_bg" >
	<asp:Repeater ID="rProdcutnMaster" DataSource="<%# CreateProductVariationMaster() %>" runat="server">
	<HeaderTemplate>
		</HeaderTemplate>
		<ItemTemplate>
			<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailSelectedVariationUrl(Container.DataItem)) %>" />
					<img id="<%# WebSanitizer.HtmlEncode(CreateImageTagId(Container.DataItem)) %>"
						src="<%# WebSanitizer.HtmlEncode(CreateImageUrl(Container.DataItem)) %>"
						alt="<%# WebSanitizer.HtmlEncode(CreateAltString(Container.DataItem))%>" width="50" /></a>
		</ItemTemplate>
		<FooterTemplate>
		</FooterTemplate>
	</asp:Repeater>
</p>
</div>
</div>
</div>
<%-- △商品バリエーション使用時△ --%>
<% } else{%>
<div>
<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(this.ProductMaster, StringUtility.ToEmpty(GetKeyValueToNull(this.ProductMaster, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)))) %>'>
	<w2c:ProductImage IsVariation="<%# this.IsColorVriationDisplay %>" ImageSize="<%#: this.ImageSize %>" ImageTagId="picture" ProductMaster="<%# this.ProductMaster %>" runat="server" />
</a>
<%-- ▽商品バリエーション使用時▽ --%>
<div visible="<%# IsUseVariation() %>" runat="server" >
<div id="variationview1_<%# this.VariationNo %>" class="variationview_wrap">
<p class="variationview_bg" >
<asp:Repeater ID="rProdcutVariationMaster" DataSource="<%# CreateProductVariationMaster() %>" runat="server">
<HeaderTemplate>
</HeaderTemplate>
<ItemTemplate>
<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailSelectedVariationUrl(Container.DataItem)) %>" />
	<img id="<%# WebSanitizer.HtmlEncode(CreateImageTagId(Container.DataItem)) %>"
		src="<%# WebSanitizer.UrlAttrHtmlEncode(CreateImageUrl(Container.DataItem)) %>"
		alt="<%# WebSanitizer.HtmlEncode(CreateAltString(Container.DataItem))%>"
		width="50" /></a>
</ItemTemplate>
<FooterTemplate>
</FooterTemplate>
</asp:Repeater>
</p>
</div>
</div>
<%-- △商品バリエーション使用時△ --%>
<%-- ▽商品バリエーション未使用時▽ --%>
<%if (Constants.LAYER_DISPLAY_NOVARIATION_UNDISPLAY_ENABLED == false) { %>
<div visible="<%# (IsUseVariation() == false) %>" style="position:relative;display:block;margin:0;padding:0;" runat="server" >
<div id="variationview2_<%# this.VariationNo %>" class="variationview_wrap">
<p class="variationview_bg" >
<span class="variationview_inner" style="display:block;padding:8px;">
<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailSelectedVariationUrl(this.ProductMaster)) %>">
	<img id="<%# WebSanitizer.HtmlEncode(CreateImageTagId(this.ProductMaster)) %>"
		src="<%# WebSanitizer.UrlAttrHtmlEncode(CreateImageUrl(this.ProductMaster, Constants.VARIATION_FAVORITE_CORRESPONDENCE)) %>"
		border="0"
		alt="<%# WebSanitizer.HtmlEncode(CreateAltString(this.ProductMaster))%>"
		class="margin:2px; border:1px" /></a>
</span>
</p>
</div>
</div>
<% } %>
<%-- △商品バリエーション未使用時△ --%>
</div>
<% } %>