<%--
=========================================================================================================
  Module      : 送料無料案内商品出力コントローラ(BodyRecommendFreeShipping.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/BodyRecommendFreeShipping.ascx.cs" Inherits="Form_Common_BodyRecommendFreeShipping" %>
<%--

下記は保持用のダミー情報です。削除しないでください。
<%@ FileInfo LastChanged="最終更新者" %>

--%>
<script runat="server">
public new void Page_Init(Object sender, EventArgs e)
{
<%-- ▽編集可能領域：プロパティ設定▽ --%>
//商品最大表示件数を設定します (MAX50)
this.MaxDispCount = 6;

//商品画像サイズを設定します (S/M/L/LL)
this.ImageSize = "M";
<%-- △編集可能領域△ --%>
}
</script>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<asp:Repeater runat="server" DataSource="<%# GetRecommendFreeShippingProductList() %>">
<HeaderTemplate>
<div class="dvRecommendFreeShipping">
	<div class="label"><img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/label_recommend_freeshipping.jpg" alt="送料無料まであと一歩の時はこちらの商品もあわせていかがでしょうか？" /></div>
	<ul class="cfix">
</HeaderTemplate>
<ItemTemplate>
		<li class="liThreeColumns">
			<a href='<%# WebSanitizer.UrlAttrHtmlEncode(ProductPage.CreateProductDetailUrl(Eval(Constants.FIELD_PRODUCT_SHOP_ID).ToString(), "", "", "", Eval(Constants.FIELD_PRODUCT_PRODUCT_ID).ToString(), "", Eval(Constants.FIELD_PRODUCT_NAME).ToString(), "")) %>'>
				<w2c:ProductImage ImageTagId="picture" ImageSize="<%# this.ImageSize %>" ProductMaster="<%# Container.DataItem %>" IsVariation="false" runat="server" CssClass="productimage" />
				<%# WebSanitizer.HtmlEncode(Eval("name")) %><br />
			</a>
				<%-- ▽商品会員ランク価格有効▽ --%>
				<span visible='<%# ProductPage.GetProductMemberRankPriceValid(Container.DataItem) %>' runat="server">
					<strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></strike>
					<%#: CurrencyManager.ToPrice(ProductPage.GetProductMemberRankPrice(Container.DataItem)) %></span>
				</span>
				<%-- △商品会員ランク価格有効△ --%>
				<%-- ▽商品セール価格有効▽ --%>
				<span visible='<%# ProductPage.GetProductTimeSalesValid(Container.DataItem) %>' runat="server">
					<strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></strike>
					<%#: CurrencyManager.ToPrice(ProductPage.GetProductTimeSalePriceNumeric(Container.DataItem)) %>
				</span>
				<%-- △商品セール価格有効△ --%>
				<%-- ▽商品特別価格有効▽ --%>
				<span visible='<%# ProductPage.GetProductSpecialPriceValid(Container.DataItem) %>' runat="server">
					<strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></strike>
					<%#: CurrencyManager.ToPrice(ProductPage.GetProductSpecialPriceNumeric(Container.DataItem)) %>
				</span>
				<%-- △商品特別価格有効△ --%>
				<%-- ▽商品通常価格有効▽ --%>
				<span visible='<%# ProductPage.GetProductNormalPriceValid(Container.DataItem) %>' runat="server">
					<%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %>
				</span>
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
				<%-- ▽在庫切れ可否▽ --%>
				<p visible='<%# ProductListUtility.IsProductSoldOut(Container.DataItem) %>' runat="server" class="soldout">SOLDOUT</p>
				<%-- △在庫切れ可否△ --%>
		</li>
</ItemTemplate>
<FooterTemplate>
	</ul>
</div>
<div></div>
</FooterTemplate>
</asp:Repeater>
<%-- △編集可能領域△ --%>