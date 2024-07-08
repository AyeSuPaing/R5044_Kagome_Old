<%--
=========================================================================================================
  Module      : おすすめ商品ランダム出力コントローラ(BodyProductRecommend.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" Inherits="ProductRecommendUserControl" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
タイトルタグはカスタムパーツのみ利用します。
<%@ Page Title="おすすめ商品アイコン2(大)" %>
<%@ FileInfo LastChanged="最終更新者" %>

--%>
<script runat="server">
public new void Page_Init(Object sender, EventArgs e)
{
base.Page_Init(sender, e);

<%-- ▽編集可能領域：プロパティ設定▽ --%>
//商品最大表示件数を設定します (MAX50)
this.MaxDispCount = 4;

//商品画像サイズを設定します (S/M/L/LL)
this.ImageSize = "M";

//表示区分を設定します (IC1～10,SRK)
this.DataKbn = "IC2";
<%-- △編集可能領域△ --%>
}
</script>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<%-- ▽おすすめ商品一覧ループ▽ --%>
<% if (this.ProductCount > 0) { %>
<asp:Repeater id="rProducts" runat="server">
<HeaderTemplate>
<div id="dvRecommend">
<dl>
<%if (this.DataKbn == "IC1") {%>
<dt><img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/label_recommend.jpg" alt="おすすめ商品" /></dt>
<%} %>
<%if (this.DataKbn == "IC2") {%>
<dt><img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/label_pickup.jpg" alt="ピックアップ" /></dt>
<%} %>
<%if (this.DataKbn == "IC3") {%>
<dt></dt>
<%} %>
<%if (this.DataKbn == "IC4") {%>
<dt></dt>
<%} %>
<%if (this.DataKbn == "IC5") {%>
<dt></dt>
<%} %>
<%if (this.DataKbn == "IC6") {%>
<dt></dt>
<%} %>
<%if (this.DataKbn == "IC7") {%>
<dt></dt>
<%} %>
<%if (this.DataKbn == "IC8") {%>
<dt></dt>
<%} %>
<%if (this.DataKbn == "IC9") {%>
<dt></dt>
<%} %>
<%if (this.DataKbn == "IC10") {%>
<dt></dt>
<%} %>
<%if (this.DataKbn == "SRK") {%>
<dt></dt>
<%} %>
</HeaderTemplate>
<ItemTemplate>
<dd class="productInfoList">
<ul class="productInfoBlock clearFix">
<li class="thumnail">
<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem)) %>">
	<w2c:ProductImage ImageTagId="picture" ImageSize="<%# this.ImageSize %>" ProductMaster="<%# Container.DataItem %>" IsVariation="false" runat="server" /></a></li>

<li class="productInfo">
<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem)) %>">
	<%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_NAME)) %></a><br />

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
<%-- ▽在庫切れ可否▽ --%>
<p visible='<%# ProductListUtility.IsProductSoldOut(Container.DataItem) %>' runat="server" class="soldout">SOLDOUT</p>
<%-- △在庫切れ可否△ --%>

</li>

</ul>
</dd>
</ItemTemplate>
<FooterTemplate>
</dl>
</div>
</FooterTemplate>
</asp:Repeater>
<% } %>
<%-- △おすすめ商品一覧ループ△ --%>
<%-- △編集可能領域△ --%>