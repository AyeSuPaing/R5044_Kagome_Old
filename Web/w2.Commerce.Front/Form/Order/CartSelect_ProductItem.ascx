<%--
=========================================================================================================
  Module      : ログイン後カート選択画面：商品表示ユーザーコントロール(CartSelect_ProductItem.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Order/CartSelect_ProductItem.ascx.cs" Inherits="Form_Order_CartSelect_ProductItem" %>
<%-- セット商品でない場合 --%>
<tr visible="<%# this.CartProduct.IsSetItem == false %>" runat="server">
	<td class="productImg">
		<!-- リンク・一覧画像 -->
		<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(this.CartProduct, (string)GetKeyValue(this.CartProduct, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID))) %>' runat="server" Visible="<%# this.CartProduct.IsProductDetailLinkValid() %>">
			<w2c:ProductImage ImageTagId="picture" ImageSize="S" ProductMaster="<%# this.CartProduct %>" IsVariation="true" runat="server" /></a>
		<w2c:ProductImage ImageTagId="picture" ImageSize="S" ProductMaster="<%# this.CartProduct %>" IsVariation="true" runat="server" Visible="<%# this.CartProduct.IsProductDetailLinkValid() == false %>" />
	</td>
	<td class="productName">
		<!-- アイコン -->
		<w2c:ProductIcon id="ProductIcon1s_1" IconNo="1" ProductMaster="<%# this.CartProduct %>" runat="server" />
		<w2c:ProductIcon id="ProductIcon1s_2" IconNo="2" ProductMaster="<%# this.CartProduct %>" runat="server" />
		<w2c:ProductIcon id="ProductIcon1s_3" IconNo="3" ProductMaster="<%# this.CartProduct %>" runat="server" />
		<w2c:ProductIcon id="ProductIcon1s_4" IconNo="4" ProductMaster="<%# this.CartProduct %>" runat="server" />
		<w2c:ProductIcon id="ProductIcon1s_5" IconNo="5" ProductMaster="<%# this.CartProduct %>" runat="server" />
		<w2c:ProductIcon id="ProductIcon1s_6" IconNo="6" ProductMaster="<%# this.CartProduct %>" runat="server" />
		<w2c:ProductIcon id="ProductIcon1s_7" IconNo="7" ProductMaster="<%# this.CartProduct %>" runat="server" />
		<w2c:ProductIcon id="ProductIcon1s_8" IconNo="8" ProductMaster="<%# this.CartProduct %>" runat="server" />
		<w2c:ProductIcon id="ProductIcon1s_9" IconNo="9" ProductMaster="<%# this.CartProduct %>" runat="server" />
		<w2c:ProductIcon id="ProductIcon1s_10" IconNo="10" ProductMaster="<%# this.CartProduct %>" runat="server" /><br />
		<!-- 商品名 -->
		<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(this.CartProduct, (string)GetKeyValue(this.CartProduct, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID))) %>' runat="server" Visible="<%# this.CartProduct.IsProductDetailLinkValid() %>">
			<%# WebSanitizer.HtmlEncode(this.CartProduct.ProductJointName) %></a>
		<%# (this.CartProduct.IsProductDetailLinkValid() == false) ? WebSanitizer.HtmlEncode(this.CartProduct.ProductJointName) : "" %>
		<!-- バリエーションID -->
		[<span class="productId"><%# WebSanitizer.HtmlEncode(this.CartProduct.VariationId) %></span>]<br />
		<span visible='<%# this.CartProduct.ProductOptionSettingList.IsSelectedProductOptionValueAll %>'>
			<%# WebSanitizer.HtmlEncode(this.CartProduct.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues()).Replace("　", "<br />") %>
		</span>
		<!-- 商品概要 -->
		<%# ProductPage.GetProductDescHtml(this.CartProduct.OutlineKbn, this.CartProduct.Outline) %><br />
		<!-- 返品交換文言 -->
		<strong><%# WebSanitizer.HtmlEncodeChangeToBr(this.CartProduct.ReturnExchangeMessage) %></strong>
		<span runat="server" visible='<%# (this.CartProduct.ReturnExchangeMessage != "") %>'>
			（<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT) %><%= WebSanitizer.HtmlEncode(ShopMessage.GetMessage("ReturnSpecialContractPage")) %>" target='<%= (ShopMessage.GetMessage("ReturnSpecialContractPage") == "#") ? "_self" : "_blank" %>' style='font-size:10px'>返品特約</a>）
		</span>
	</td>
	<td class="productPrice">
		<div>
			<!-- 商品価格 -->
			<%#: CurrencyManager.ToPrice(this.CartProduct.Price) %>
			<!-- 税区分 -->
			（<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_TAX_INCLUDED_FLG, this.CartProduct.TaxIncludedFlg)) %>）
		</div>
	</td>
	<td class="productPrice" runat="server" Visible="<%# IsDisplayOption(this.CartProducts) %>">
		<div>
			<%#: this.CartProduct.ProductOptionSettingList.HasOptionPrice
				? string.Format("{0} ({1})", CurrencyManager.ToPrice(this.CartProduct.TotalOptionPrice), this.ProductPriceTextPrefix)
				: "―" %>
		</div>
	</td>
	<td class="remark">
		<asp:CheckBox id="cbAddToCart" Checked="<%# this.DefaultChecked %>" CssClass="checkBox" runat="server" />
	</td>
</tr>

<%-- セット商品の場合 --%>
<tr visible="<%# this.CartProduct.IsSetItem %>" runat="server">
	<td class="productImg">
		<!-- リンク・一覧画像 -->
		<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(this.CartProduct, (string)GetKeyValue(this.CartProduct, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID))) %>' runat="server" Visible="<%# this.CartProduct.IsProductDetailLinkValid() %>">
			<w2c:ProductImage ImageTagId="picture" ImageSize="S" ProductMaster="<%# this.CartProduct %>" IsVariation="true" runat="server" /></a>
		<w2c:ProductImage ImageTagId="picture" ImageSize="S" ProductMaster="<%# this.CartProduct %>" IsVariation="true" runat="server" Visible="<%# this.CartProduct.IsProductDetailLinkValid() == false %>" />
	</td>
	<td class="productName">
		<!-- アイコン -->
		<w2c:ProductIcon id="ProductIcon1_1" IconNo="1" ProductMaster="<%# this.CartProduct %>" runat="server" />
		<w2c:ProductIcon id="ProductIcon1_2" IconNo="2" ProductMaster="<%# this.CartProduct %>" runat="server" />
		<w2c:ProductIcon id="ProductIcon1_3" IconNo="3" ProductMaster="<%# this.CartProduct %>" runat="server" />
		<w2c:ProductIcon id="ProductIcon1_4" IconNo="4" ProductMaster="<%# this.CartProduct %>" runat="server" />
		<w2c:ProductIcon id="ProductIcon1_5" IconNo="5" ProductMaster="<%# this.CartProduct %>" runat="server" />
		<w2c:ProductIcon id="ProductIcon1_6" IconNo="6" ProductMaster="<%# this.CartProduct %>" runat="server" />
		<w2c:ProductIcon id="ProductIcon1_7" IconNo="7" ProductMaster="<%# this.CartProduct %>" runat="server" />
		<w2c:ProductIcon id="ProductIcon1_8" IconNo="8" ProductMaster="<%# this.CartProduct %>" runat="server" />
		<w2c:ProductIcon id="ProductIcon1_9" IconNo="9" ProductMaster="<%# this.CartProduct %>" runat="server" />
		<w2c:ProductIcon id="ProductIcon1_10" IconNo="10" ProductMaster="<%# this.CartProduct %>" runat="server" /><br />
		<!-- 商品名 -->
		<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(this.CartProduct, (string)GetKeyValue(this.CartProduct, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID))) %>' runat="server" Visible="<%# this.CartProduct.IsProductDetailLinkValid() %>">
			<%# WebSanitizer.HtmlEncode(this.CartProduct.ProductJointName) %></a>
		<%# (this.CartProduct.IsProductDetailLinkValid() == false) ? WebSanitizer.HtmlEncode(this.CartProduct.ProductJointName) : "" %>
		<!-- バリエーションID -->
		[<span class="productId"><%# WebSanitizer.HtmlEncode(this.CartProduct.VariationId) %></span>]<br />
		<!-- 返品交換文言 -->
		<strong><%# WebSanitizer.HtmlEncodeChangeToBr(this.CartProduct.ReturnExchangeMessage) %></strong>
		<span runat="server" visible='<%# (this.CartProduct.ReturnExchangeMessage != "") %>'>
			（<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT) %><%= WebSanitizer.HtmlEncode(ShopMessage.GetMessage("ReturnSpecialContractPage")) %>" target='<%= (ShopMessage.GetMessage("ReturnSpecialContractPage") == "#") ? "_self" : "_blank" %>' style='font-size:10px'>返品特約</a>）
		</span>
	</td>
	<td class="productPrice">
		<!-- バリエーション価格 -->
		<%#: CurrencyManager.ToPrice(this.CartProduct.Price) %>
		x
		<%# WebSanitizer.HtmlEncode(this.CartProduct.CountSingle) %>
		<!-- 税区分 -->
		（<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_TAX_INCLUDED_FLG, this.CartProduct.TaxIncludedFlg)) %>）
		<!-- 加算ポイント -->
		<%# WebSanitizer.HtmlEncode(StringUtility.AddHeaderFooter("ポイント", GetProductAddPointString(this.ShopId, this.CartProduct.PointKbn1, this.CartProduct.Point1, this.CartProduct.Price), "")) %>
	</td>
	<td class="remark" rowspan="<%# OrderPage.GetProductSetRowspan(this.CartProduct) %>" runat="server" visible="<%# this.CartProduct.ProductSetItemNo == 1 %>">
		<!-- チェックボックス -->
		<asp:CheckBox id="cbAddSetToCart" Checked="<%# this.DefaultChecked %>" CssClass="checkBox" runat="server" />
	</td>
</tr>
