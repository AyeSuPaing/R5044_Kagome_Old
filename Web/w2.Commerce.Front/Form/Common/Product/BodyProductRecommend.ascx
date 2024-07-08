<%--
=========================================================================================================
  Module      : おすすめ商品ランダム出力コントローラ(BodyProductRecommend.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="ProductRecommendUserControl" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
タイトルタグはカスタムパーツのみ利用します。
<%@ Page Title="無名のパーツ" %>
<%@ FileInfo LastChanged="最終更新者" %>

--%>
<script runat="server">
public new void Page_Init(Object sender, EventArgs e)
{
base.Page_Init(sender, e);

<%-- ▽編集可能領域：プロパティ設定▽ --%>
//商品画像サイズを設定します (S/M/L/LL)
if (this.ImageSize == null) this.ImageSize = "M";

//表示区分を設定します (IC1～10,SRK)
this.DataKbn = "";
<%-- △編集可能領域△ --%>
}
</script>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<%-- ▽おすすめ商品一覧ループ▽ --%>
<% if (this.ProductCount > 0) { %>
<asp:Repeater id="rProducts" runat="server">
<HeaderTemplate>
<div id="dvRecommend" class="clearFix">
<%if (this.DataKbn == "IC1") {%>
<p class="title">おすすめ商品</p>
<%} %>
<%if (this.DataKbn == "IC2") {%>
<p class="title">ピックアップ</p>
<%} %>
<%if (this.DataKbn == "IC3") {%>
<p class="title"></p>
<%} %>
<%if (this.DataKbn == "IC4") {%>
<p class="title"></p>
<%} %>
<%if (this.DataKbn == "IC5") {%>
<p class="title"></p>
<%} %>
<%if (this.DataKbn == "IC6") {%>
<p class="title"></p>
<%} %>
<%if (this.DataKbn == "IC7") {%>
<p class="title"></p>
<%} %>
<%if (this.DataKbn == "IC8") {%>
<p class="title"></p>
<%} %>
<%if (this.DataKbn == "IC9") {%>
<p class="title"></p>
<%} %>
<%if (this.DataKbn == "IC10") {%>
<p class="title"></p>
<%} %>
<%if (this.DataKbn == "SRK") {%>
<p class="title"></p>
<%}else { %>
<p class="title">ユーザーおすすめ商品</p>
<%} %>
</HeaderTemplate>
<ItemTemplate>
<div class="productInfoList">
<ul class="productInfoBlock clearFix">
<li class="thumnail">
<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem)) %>">
	<w2c:ProductImage ID="ProductImage1" ImageTagId="picture" ImageSize="<%# this.ImageSize %>" ProductMaster="<%# Container.DataItem %>" IsVariation="false" runat="server" /></a>
	<%-- ▽在庫切れ可否▽ --%>
	<span visible='<%# ProductListUtility.IsProductSoldOut(Container.DataItem) %>' runat="server" class="soldout">SOLDOUT</span>
	<%-- △在庫切れ可否△ --%>
</li>

<li class="productInfo">
<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem)) %>">
	<%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_NAME)) %></a><br />

<%-- ▽商品会員ランク価格有効▽ --%>
<p id="Span4" visible='<%# ProductPage.GetProductMemberRankPriceValid(Container.DataItem) %>' runat="server">
<strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem))%></strike><br />
<%#: CurrencyManager.ToPrice(ProductPage.GetProductMemberRankPrice(Container.DataItem)) %>
</p>
<%-- △商品会員ランク価格有効△ --%>
<%-- ▽商品セール価格有効▽ --%>
<p id="Span1" visible='<%# ProductPage.GetProductTimeSalesValid(Container.DataItem) %>' runat="server">
	<strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></strike><br />
	<%#: CurrencyManager.ToPrice(ProductPage.GetProductTimeSalePriceNumeric(Container.DataItem)) %>
</p>
<%-- △商品セール価格有効△ --%>
<%-- ▽商品特別価格有効▽ --%>
<p id="Span2" visible='<%# ProductPage.GetProductSpecialPriceValid(Container.DataItem) %>' runat="server">
	<strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></strike><br />
	<%#: CurrencyManager.ToPrice(ProductPage.GetProductSpecialPriceNumeric(Container.DataItem)) %>
</p>
<%-- △商品特別価格有効△ --%>
<%-- ▽商品通常価格有効▽ --%>
<p id="Span3" visible='<%# ProductPage.GetProductNormalPriceValid(Container.DataItem) %>' runat="server">
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
</ul>
</div>
</ItemTemplate>
<FooterTemplate>
</div>
</FooterTemplate>
</asp:Repeater>
<% } %>
<%-- △おすすめ商品一覧ループ△ --%>
<%-- △編集可能領域△ --%>