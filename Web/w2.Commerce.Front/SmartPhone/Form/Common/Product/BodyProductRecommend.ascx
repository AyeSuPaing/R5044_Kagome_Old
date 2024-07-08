<%--
=========================================================================================================
  Module      : スマートフォン用おすすめ商品ランダム出力コントローラ(BodyProductRecommend.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
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
//商品最大表示件数を設定します (MAX50)
this.MaxDispCount = 5;

//商品画像サイズを設定します (S/M/L/LL)
this.ImageSize = "M";

//表示区分を設定します (IC1～5,SRK)
this.DataKbn = "IC1";
<%-- △編集可能領域△ --%>
}
</script>
<%-- ▽編集可能領域：コンテンツ▽ --%>

<%-- ▽おすすめ商品一覧ループ▽ --%>
<% if (this.ProductCount > 0) { %>
<asp:Repeater id="rProducts" runat="server">
<HeaderTemplate>
<div class="panel">
<%if (this.DataKbn == "IC1") {%>
<h2>おすすめ商品</h2>
<%} %>
<%if (this.DataKbn == "IC2") {%>
<h2>ピックアップ</h2>
<%} %>
<%if (this.DataKbn == "IC3") {%>
<h2></h2>
<%} %>
<%if (this.DataKbn == "IC4") {%>
<h2></h2>
<%} %>
<%if (this.DataKbn == "IC5") {%>
<h2></h2>
<%} %>
<%if (this.DataKbn == "SRK") {%>
<h2></h2>
<%} %>
<fieldset>
</HeaderTemplate>
<ItemTemplate>
	<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem)) %>">
	<div class="row ProductRankingArea">
		<div>
			<w2c:ProductImage ImageTagId="picture" ImageSize="<%# this.ImageSize %>" ProductMaster="<%# Container.DataItem %>" IsVariation="false" runat="server" />
		</div>
		<div>
			<div class="TextWithEllipsis">
				<%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_NAME)) %>
			</div>
			<%-- ▽商品会員ランク価格有効▽ --%>
			<span visible='<%# ProductPage.GetProductMemberRankPriceValid(Container.DataItem) %>' runat="server">
				<del><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem))%></del><br />
				<%#: CurrencyManager.ToPrice(ProductPage.GetProductMemberRankPrice(Container.DataItem)) %></span>
			</span>
			<%-- △商品会員ランク価格有効△ --%>
			<%-- ▽商品セール価格有効▽ --%>
			<span visible='<%# ProductPage.GetProductTimeSalesValid(Container.DataItem) %>' runat="server">
				<del><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></del><br />
				<%#: CurrencyManager.ToPrice(ProductPage.GetProductTimeSalePriceNumeric(Container.DataItem)) %>
			</span>
			<%-- △商品セール価格有効△ --%>
			<%-- ▽商品特別価格有効▽ --%>
			<span visible='<%# ProductPage.GetProductSpecialPriceValid(Container.DataItem) %>' runat="server">
				<del><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></del><br />
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
			<span visible='<%# (StringUtility.ToValue(ProductPage.GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG), "").ToString() != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) %>' runat="server">
				<span visible='<%# ProductPage.IsProductFixedPurchaseFirsttimePriceValid(Container.DataItem) %>' runat="server">
					<br />
					定期初回:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchaseFirsttimePrice(Container.DataItem)) %>
				</span>
				<br />定期通常:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchasePrice(Container.DataItem)) %>
			</span>
			<% } %>
			<%-- △商品定期購入価格△ --%>
			<%-- ▽在庫切れ可否▽ --%>
			<p visible='<%# ProductListUtility.IsProductSoldOut(Container.DataItem) %>' runat="server" class="soldout">SOLDOUT</p>
			<%-- △在庫切れ可否△ --%>
		</div>
	</div>
	</a>
</ItemTemplate>
<FooterTemplate>
</fieldset>
</div>
</FooterTemplate>
</asp:Repeater>
<% } %>
<%-- △おすすめ商品一覧ループ△ --%>
<%-- △編集可能領域△ --%>