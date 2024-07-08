﻿<%--
=========================================================================================================
  Module      : 商品在庫一覧出力コントローラ処理(BodyProductStockList.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Product/BodyProductStockList.ascx.cs" Inherits="Form_Common_Product_BodyProductStockList" %>
<%--
下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LastChanged="最終更新者" %>
--%>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<% if (ProductStockTitle){ %>
<header>
	<h1>在庫状況一覧</h1>
	<h2><%= this.ProductName %></h2>
</header>
<%} %>
<div id="divSingleVariation" runat="server">
	<table>
		<tr>
			<th>バリエーション</th>
			<% if (this.DisplayPrice){ %>
			<th>販売価格</th>
			<%} %>
			<th>在庫状況</th>
		</tr>
		<asp:Repeater ID="rStockList" runat="server">
		<ItemTemplate>
		<tr>
			<td>
			<!--バリエーション名-->
			<%# WebSanitizer.HtmlEncode(ProductPage.CreateVariationName(Container.DataItem, "", "", Constants.CONST_PRODUCTVARIATIONNAME_PUNCTUATION))%></td>
			<% if (this.DisplayPrice){ %>
			<td>
				<!--価格-->
				<%-- ▽商品会員ランク価格有効▽ --%>
				<p visible='<%# ProductPage.GetProductMemberRankPriceValid(Container.DataItem, HasVariation(Container.DataItem)) %>' runat="server">
					<%#: CurrencyManager.ToPrice(ProductPage.GetProductMemberRankPrice(Container.DataItem, true)) %>
					(<%# WebSanitizer.HtmlEncode(ProductPage.GetTaxIncludeString(Container.DataItem))%>)
					<span class="line-through">
						<%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem, true)) %>
						(<%# WebSanitizer.HtmlEncode(ProductPage.GetTaxIncludeString(Container.DataItem))%>)
					</span>
				</p>
				<%-- △商品会員ランク価格有効△ --%>
				<%-- ▽商品セール価格有効▽ --%>
				<p visible='<%# ProductPage.GetProductTimeSalesValid(Container.DataItem, HasVariation(Container.DataItem)) %>' runat="server">
					<%#: CurrencyManager.ToPrice(ProductPage.GetProductTimeSalePriceNumeric(Container.DataItem)) %>
					(<%# WebSanitizer.HtmlEncode(ProductPage.GetTaxIncludeString(Container.DataItem))%>)
					<span class="line-through">
						<%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem, true)) %>
						(<%# WebSanitizer.HtmlEncode(ProductPage.GetTaxIncludeString(Container.DataItem))%>)
					</span>
				</p>
				<%-- △商品セール価格有効△ --%>
				<%-- ▽商品特別価格有効▽ --%>
				<p visible='<%# ProductPage.GetProductSpecialPriceValid(Container.DataItem, HasVariation(Container.DataItem)) %>' runat="server">
					<%#: CurrencyManager.ToPrice(ProductPage.GetProductSpecialPriceNumeric(Container.DataItem, true)) %>
					(<%# WebSanitizer.HtmlEncode(ProductPage.GetTaxIncludeString(Container.DataItem))%>)
					<span class="line-through">
						<%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem, true)) %>
						(<%# WebSanitizer.HtmlEncode(ProductPage.GetTaxIncludeString(Container.DataItem))%>)
					</span>
				</p>
				<%-- △商品特別価格有効△ --%>
				<%-- ▽商品通常価格有効▽ --%>
				<p visible='<%# ProductPage.GetProductNormalPriceValid(Container.DataItem, HasVariation(Container.DataItem)) %>' runat="server">
					<%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem, true)) %>
					(<%# WebSanitizer.HtmlEncode(ProductPage.GetTaxIncludeString(Container.DataItem)) %>)
				</p>
				<%-- △商品通常価格有効△ --%>
				<%-- ▽定期購入初回価格有効▽ --%>
				<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) {%>
				<p visible='<%# (GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG).ToString() != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) %>' runat="server">
					<p visible='<%# ProductPage.IsProductFixedPurchaseFirsttimePriceValid(Container.DataItem) %>' runat="server">
						定期初回:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchaseFirsttimePrice(Container.DataItem, true)) %>
						(<%#: ProductPage.GetTaxIncludeString(Container.DataItem) %>)
					</p>
					定期通常<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchasePrice(Container.DataItem, true)) %>
					(<%#: ProductPage.GetTaxIncludeString(Container.DataItem)%>)
				</p>
				<% } %>
				<%-- △定期購入初回価格有効△ --%>
			</td>
			<%} %>
			<td>
				<!--在庫状況-->
				<%#: GetStockMessage(Container.DataItem) %>
			</td>
		</tr>
		</ItemTemplate>
		</asp:Repeater>
	</table>
</div>

<div id="divPluralVariation" runat="server">
	<table>
		<tr>
			<asp:Repeater ID="rStockMatrixHorizonalTitile" runat="server">
				<HeaderTemplate>
					<th></th>
				</HeaderTemplate>
				<ItemTemplate>
					<th><%# Container.DataItem %></th>
				</ItemTemplate>
				<FooterTemplate>
				</FooterTemplate>
			</asp:Repeater>
		</tr>
		<asp:Repeater ID="rStockMatrix" runat="server">
		<ItemTemplate>
			<tr>
				<th>
					<%# this.VariationName1s[Container.ItemIndex] %>
				</th>
				<asp:Repeater ID="rStockMatrixHorizonal" DataSource="<%# this.VariationName23s %>" runat="server">
					<ItemTemplate>
						<td><%# WebSanitizer.HtmlEncode(m_strVariationMatrixs[((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex]) %></td>
					</ItemTemplate>
				</asp:Repeater>
			</tr>
		</ItemTemplate>
		</asp:Repeater>
	</table>
</div>
<%-- △編集可能領域△ --%>