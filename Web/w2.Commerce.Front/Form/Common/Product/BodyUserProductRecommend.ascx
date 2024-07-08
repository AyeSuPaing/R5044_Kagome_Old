<%--
=========================================================================================================
  Module      : ユーザおすすめ商品ランダム出力コントローラ(BodyUserProductRecommend.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Product/BodyUserProductRecommend.ascx.cs" Inherits="Form_Common_Product_BodyUserProductRecommend" %>

<script runat="server">
public new void Page_Init(Object sender, EventArgs e)
{
base.Page_Init(sender, e);

//商品画像サイズを設定します (S/M/L/LL)
if (this.ImageSize == null) this.ImageSize = "M";
}
</script>

<asp:Repeater id="rProducts" runat="server">
	<HeaderTemplate>
	<div id="dvRecommend" class="clearFix">
		<p class="title">ユーザーおすすめ商品</p>
	</HeaderTemplate>
	<ItemTemplate>
		<div class="productInfoList">
			<ul class="clearFix">
		<li class="productInfoList">
			<ul class="clearFix">
			<li class="thumnail"><a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem)) %>">
				<w2c:ProductImage ImageTagId="picture" ImageSize="<%# this.ImageSize %>" ProductMaster="<%# Container.DataItem %>" IsVariation="false" runat="server" /></a>
				<%-- ▽在庫切れ可否▽ --%>
				<span visible='<%# ProductListUtility.IsProductSoldOut(Container.DataItem) %>' runat="server" class="soldout">SOLDOUT</span>
				<%-- △在庫切れ可否△ --%>
			</li>
				
			<li class="productInfo"><a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem)) %>"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCT_NAME)) %></a><br />
				
				<%-- ▽商品会員ランク価格有効▽ --%>
				<p visible='<%# ProductPage.GetProductMemberRankPriceValid(Container.DataItem) %>' runat="server">
					<strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></strike><br />
					<%#: CurrencyManager.ToPrice(ProductPage.GetProductMemberRankPrice(Container.DataItem)) %>
				</p>
				<%-- △商品会員ランク価格有効△ --%>
				<%-- タイムセールス価格適用時 --%>
				<p runat="server" visible='<%# ProductPage.GetProductTimeSalesValid(Container.DataItem) %>'>
					<strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></strike><br />
					<%#: CurrencyManager.ToPrice(ProductPage.GetProductTimeSalePriceNumeric(Container.DataItem)) %>
				</p>
				<%-- 特別価格適用時 --%>
				<p runat="server" visible='<%# ProductPage.GetProductSpecialPriceValid(Container.DataItem) %>'>
					<strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></strike><br />
					<%#: CurrencyManager.ToPrice(ProductPage.GetProductSpecialPriceNumeric(Container.DataItem)) %>
				</p>
				<%-- 通常価格適用時 --%>
				<p runat="server" visible='<%# ProductPage.GetProductNormalPriceValid(Container.DataItem) %>'>
					<%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %>
				</p>
				<%-- ▽定期購入初回価格有効▽ --%>
				<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) {%>
				<p visible='<%# (StringUtility.ToValue(ProductPage.GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG), "").ToString() != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) %>' runat="server">
					<p visible='<%# ProductPage.IsProductFixedPurchaseFirsttimePriceValid(Container.DataItem) %>' runat="server">
						定期初回:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchaseFirsttimePrice(Container.DataItem)) %>
					</p>
					定期通常<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchasePrice(Container.DataItem)) %>
				</p>
				<% } %>
				<%-- △定期購入初回価格有効△ --%>
			</li>
			</ul>
		</li>
			</ul>
		</div>
	</ItemTemplate>
	<FooterTemplate>
	</div>
	</FooterTemplate>
</asp:Repeater>