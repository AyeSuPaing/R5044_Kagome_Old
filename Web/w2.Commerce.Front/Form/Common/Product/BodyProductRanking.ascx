<%--
=========================================================================================================
  Module      : 商品ランキング出力コントローラ(BodyProductRanking.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="BodyProductVariationImages" Src="~/Form/Common/Product/BodyProductVariationImages.ascx" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Product/BodyProductRanking.ascx.cs" Inherits="Form_Common_Product_BodyProductRanking" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
タイトルタグはカスタムパーツのみ利用します。
<%@ Page Title="無名のパーツ" %>
<%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<script runat="server">
public new void Page_Init(Object sender, EventArgs e)
{
base.Page_Init(sender, e);

<%-- ▽編集可能領域：プロパティ設定▽ --%>
//商品最大表示件数を設定します (MAX10)
this.MaxDispCount = 10;

//商品画像サイズを設定します (S/M/L/LL)
this.ImageSize = "M";

//表示区分を設定します (SRK,商品ランキングID)
this.DataKbn = "";
<%-- △編集可能領域△ --%>
}
</script>

<%-- ▽編集可能領域：コンテンツ▽ --%>
	<% if (this.ProductCount > 0) { %>
	<asp:Repeater ID="rProducts" runat="server">
	<HeaderTemplate>
	<div id="dvTopRanking" class="unit">
		<h3>TOTAL RANKING<span>総合ランキング</span></h3>
		<div class="listProduct clearFix">
	</HeaderTemplate>
		<ItemTemplate>
			<div class="glbPlist column4">
			<p class="rank">No.<%# Container.ItemIndex+1 %></p>
			<ul>
				<li class="icon">
				<span>
					<w2c:ProductIcon IconNo="1" ProductMaster="<%# Container.DataItem %>" runat="server" />
					<w2c:ProductIcon IconNo="2" ProductMaster="<%# Container.DataItem %>" runat="server" />
					<w2c:ProductIcon IconNo="3" ProductMaster="<%# Container.DataItem %>" runat="server" />
					<w2c:ProductIcon IconNo="4" ProductMaster="<%# Container.DataItem %>" runat="server" />
					<w2c:ProductIcon IconNo="5" ProductMaster="<%# Container.DataItem %>" runat="server" />
					<w2c:ProductIcon IconNo="6" ProductMaster="<%# Container.DataItem %>" runat="server" />
					<w2c:ProductIcon IconNo="7" ProductMaster="<%# Container.DataItem %>" runat="server" />
					<w2c:ProductIcon IconNo="8" ProductMaster="<%# Container.DataItem %>" runat="server" />
					<w2c:ProductIcon IconNo="9" ProductMaster="<%# Container.DataItem %>" runat="server" />
					<w2c:ProductIcon IconNo="10" ProductMaster="<%# Container.DataItem %>" runat="server" />
				</span>
				</li>
				<li class="thumb">
				<% if(Constants.LAYER_DISPLAY_VARIATION_IMAGES_ENABLED){ %>
				<uc:BodyProductVariationImages ImageSize="<%# this.ImageSize %>" ProductMaster="<%# Container.DataItem %>" VariationList="<%# this.ProductVariationList %>" VariationNo="<%# Container.ItemIndex.ToString() %>" runat="server" />
				<% } else { %>
				<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem)) %>'>
				<w2c:ProductImage ImageSize="<%# this.ImageSize %>" ProductMaster="<%# Container.DataItem %>" IsVariation="false" runat="server" /></a>
				<% } %>
				<%-- ▽在庫切れ可否▽ --%>
				<span visible='<%# ProductListUtility.IsProductSoldOut(Container.DataItem) %>' runat="server" class="soldout">SOLDOUT</span>
				<%-- △在庫切れ可否△ --%>
				</li>
				<li class="name">
				<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem)) %>"><%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_NAME)) %></a>
				</li>
				<li class="price">

				<%-- ▽商品会員ランク価格有効▽ --%>
				<p visible='<%# ProductPage.GetProductMemberRankPriceValid(Container.DataItem) %>' runat="server">
				<span style="text-decoration: line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %>（<%# WebSanitizer.HtmlEncode(ProductPage.GetTaxIncludeString(Container.DataItem)) %>）</span><br />
				<span style="color: #f00;"><%#: CurrencyManager.ToPrice(ProductPage.GetProductMemberRankPrice(Container.DataItem)) %>（<%# WebSanitizer.HtmlEncode(ProductPage.GetTaxIncludeString(Container.DataItem)) %>）</span>
				</p>

				<%-- ▽商品セール価格有効▽ --%>
				<p visible='<%# ProductPage.GetProductTimeSalesValid(Container.DataItem) %>' runat="server" class="special">
				<span style="text-decoration: line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %>（<%# WebSanitizer.HtmlEncode(ProductPage.GetTaxIncludeString(Container.DataItem)) %>）</span><br />
				<span style="color: #f00;"><%#: CurrencyManager.ToPrice(ProductPage.GetProductTimeSalePriceNumeric(Container.DataItem)) %>（<%# WebSanitizer.HtmlEncode(ProductPage.GetTaxIncludeString(Container.DataItem)) %>）</span>
				</p>
				<%-- △商品セール価格有効△ --%>

				<%-- ▽商品特別価格有効▽ --%>
				<p visible='<%# ProductPage.GetProductSpecialPriceValid(Container.DataItem) %>' runat="server">
				<span style="text-decoration: line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %>（<%# WebSanitizer.HtmlEncode(ProductPage.GetTaxIncludeString(Container.DataItem)) %>)</span><br />
				<span style="color: #f00;"><%#: CurrencyManager.ToPrice(ProductPage.GetProductSpecialPriceNumeric(Container.DataItem)) %>（<%# WebSanitizer.HtmlEncode(ProductPage.GetTaxIncludeString(Container.DataItem)) %>）</span>
				</p>

				<%-- ▽商品通常価格有効▽ --%>
				<p visible='<%# ProductPage.GetProductNormalPriceValid(Container.DataItem) %>' runat="server">
				<%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %>（<%# WebSanitizer.HtmlEncode(ProductPage.GetTaxIncludeString(Container.DataItem)) %>）
				</p>

				<%-- ▽商品定期購入価格▽ --%>
				<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) {%>
				<p visible='<%# (StringUtility.ToValue(ProductPage.GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG), "").ToString() != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) %>' runat="server">
					<span visible='<%# ProductPage.IsProductFixedPurchaseFirsttimePriceValid(Container.DataItem) %>' runat="server">
						定期初回:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchaseFirsttimePrice(Container.DataItem)) %>（<%#: ProductPage.GetTaxIncludeString(Container.DataItem) %>）
						<br />
					</span>
					定期通常:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchasePrice(Container.DataItem)) %>（<%#: ProductPage.GetTaxIncludeString(Container.DataItem) %>）
				</p>
				<% } %>
				<%-- △商品定期購入価格△ --%>
				</li>
			</ul>
			</div>
		</ItemTemplate>
	<FooterTemplate>
		</div>
	</div>
	</FooterTemplate>
	</asp:Repeater>
	<% } %>
<%-- △編集可能領域△ --%>