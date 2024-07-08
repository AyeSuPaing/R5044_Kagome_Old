<%--
=========================================================================================================
  Module      : スマートフォン用ログイン後カート選択画面：商品表示ユーザーコントロール(CartSelect_ProductItem.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Order/CartSelect_ProductItem.ascx.cs" Inherits="Form_Order_CartSelect_ProductItem" %>
	<%-- セット商品でない場合 --%>
	<tr class="cart-unit-product" visible="<%# this.CartProduct.IsSetItem == false %>" runat="server">
		<td class="product-image">
		<%-- 画像 --%>
		<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(this.CartProduct, (string)GetKeyValue(this.CartProduct, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID))) %>' runat="server" Visible="<%# this.CartProduct.IsProductDetailLinkValid() %>">
			<w2c:ProductImage ImageTagId="picture" ImageSize="M" ProductMaster="<%# this.CartProduct %>" IsVariation="true" runat="server" /></a>
		<w2c:ProductImage ImageTagId="picture" ImageSize="M" ProductMaster="<%# this.CartProduct %>" IsVariation="true" runat="server" Visible="<%# this.CartProduct.IsProductDetailLinkValid() == false %>" />
		</td>
		<td class="product-info">
			<ul>
				<li class="product-name">
				<%-- 商品名 --%>
					<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(this.CartProduct, (string)GetKeyValue(this.CartProduct, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID))) %>' runat="server" Visible="<%# this.CartProduct.IsProductDetailLinkValid() %>">
						<%# WebSanitizer.HtmlEncode(this.CartProduct.ProductJointName) %></a>
					<%# (this.CartProduct.IsProductDetailLinkValid() == false) ? WebSanitizer.HtmlEncode(this.CartProduct.ProductJointName) : "" %>
					<%-- バリエーションID --%>
					[<span class="productId"><%# WebSanitizer.HtmlEncode(this.CartProduct.VariationId) %></span>]<br />
					<%-- 商品付帯情報 --%>
					<span visible='<%# this.CartProduct.ProductOptionSettingList.IsSelectedProductOptionValueAll %>'>
					<%# WebSanitizer.HtmlEncode(this.CartProduct.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues()).Replace("　", "") %>
					</span>
				</li>
				<li class="product-price">
				<%-- 商品価格 --%>
					<% if(Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false) { %>
					<%#: CurrencyManager.ToPrice(this.CartProduct.Price) %>
						（<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_TAX_INCLUDED_FLG, this.CartProduct.TaxIncludedFlg)) %>）
					<% } %>
					<% if(Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED) { %>
						<%#: CurrencyManager.ToPrice(this.CartProduct.Price) %>
						（<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_TAX_INCLUDED_FLG, this.CartProduct.TaxIncludedFlg)) %>）
						<% if(this.CartProduct.ProductOptionSettingList.HasOptionPrice) { %>
						<%#:"+　" + CurrencyManager.ToPrice(this.CartProduct.TotalOptionPrice) %>
						（<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_TAX_INCLUDED_FLG, this.CartProduct.TaxIncludedFlg)) %>）
						<% } %>
					<% } %>
				</li>
			</ul>
		</td>
		<td class="product-control">
			選択<br />
			<asp:CheckBox id="cbAddToCart" Checked="<%# this.DefaultChecked %>" CssClass="checkBox" runat="server" />
		</td>
	</tr>
	<%-- セット商品の場合 --%>
	<tr class="cart-unit-product" visible="<%# this.CartProduct.IsSetItem %>" runat="server">
		<td class="product-image">
		<%-- 画像 --%>
		<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(this.CartProduct, (string)GetKeyValue(this.CartProduct, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID))) %>' runat="server" Visible="<%# this.CartProduct.IsProductDetailLinkValid() %>">
			<w2c:ProductImage ImageTagId="picture" ImageSize="M" ProductMaster="<%# this.CartProduct %>" IsVariation="true" runat="server" /></a>
		<w2c:ProductImage ImageTagId="picture" ImageSize="M" ProductMaster="<%# this.CartProduct %>" IsVariation="true" runat="server" Visible="<%# this.CartProduct.IsProductDetailLinkValid() == false %>" />
		</td>
		<td class="product-info">
			<ul>
				<li class="product-name">
				<%-- 商品名 --%>
					<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(this.CartProduct, (string)GetKeyValue(this.CartProduct, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID))) %>' runat="server" Visible="<%# this.CartProduct.IsProductDetailLinkValid() %>">
						<%# WebSanitizer.HtmlEncode(this.CartProduct.ProductJointName) %></a>
					<%# (this.CartProduct.IsProductDetailLinkValid() == false) ? WebSanitizer.HtmlEncode(this.CartProduct.ProductJointName) : "" %>
					<%-- バリエーションID --%>
					[<span class="productId"><%# WebSanitizer.HtmlEncode(this.CartProduct.VariationId) %></span>]<br />
					<%-- 商品付帯情報 --%>
					<span visible='<%# this.CartProduct.ProductOptionSettingList.IsSelectedProductOptionValueAll %>'>
					<%# WebSanitizer.HtmlEncode(this.CartProduct.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues()).Replace("　", "") %>
					</span>
				</li>
				<li class="product-price">
				<%-- 商品価格 --%>
					<%#: CurrencyManager.ToPrice(this.CartProduct.Price) %>
					x
					<%# WebSanitizer.HtmlEncode(this.CartProduct.CountSingle) %>
					（<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_TAX_INCLUDED_FLG, this.CartProduct.TaxIncludedFlg)) %>）
				</li>
			</ul>
		</td>
		<td class="product-control" runat="server" rowspan="<%# OrderPage.GetProductSetRowspan(this.CartProduct) %>" visible="<%# this.CartProduct.ProductSetItemNo == 1 %>">
			選択<br />
			<asp:CheckBox id="cbAddSetToCart" Checked="<%# this.DefaultChecked %>" CssClass="checkBox" runat="server" />
		</td>
	</tr>