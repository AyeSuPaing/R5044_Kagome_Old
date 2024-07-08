<%--
=========================================================================================================
  Module      : スマートフォン用商品ランキング出力コントローラ(BodyProductRanking.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
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
  <%-- ▽商品ランキング一覧ループ▽ --%>
  <% if (this.ProductCount > 0) { %>
  <asp:Repeater ID="rProducts" runat="server">
  <HeaderTemplate>
  <section class="ranking unit" visible="<%# (Container.ItemIndex > 0) %>">
  <h3 class="title">売上げランキング</h3>
  <ul class="product-list-3 clearfix">
  </HeaderTemplate>
  <ItemTemplate>
  <li>
    <a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem)) %>">
    <div class="product-image">
      <w2c:ProductImage ImageTagId="picture" ImageSize="<%# this.ImageSize %>" ProductMaster="<%# Container.DataItem %>" IsVariation="false" runat="server" />
      <%-- ▽ランキング表記▽ --%>
      <span class="rank rank<%# Container.ItemIndex+1 %>"><%# Container.ItemIndex+1 %></span>
      <%-- △ランキング表記△ --%>
      <%-- ▽在庫切れ可否▽ --%>
      <span visible='<%# ProductListUtility.IsProductSoldOut(Container.DataItem) %>' runat="server" class="sold-out">SOLD OUT</span>
      <%-- △在庫切れ可否△ --%>
    </div>
    <div class="product-name">
      <%-- ▽商品アイコン▽ --%>
      <p class="icon">
        <w2c:ProductIcon ID="ProductIcon1" IconNo="1" ProductMaster="<%# Container.DataItem %>" runat="server" />
        <w2c:ProductIcon ID="ProductIcon2" IconNo="2" ProductMaster="<%# Container.DataItem %>" runat="server" />
        <w2c:ProductIcon ID="ProductIcon3" IconNo="3" ProductMaster="<%# Container.DataItem %>" runat="server" />
        <w2c:ProductIcon ID="ProductIcon4" IconNo="4" ProductMaster="<%# Container.DataItem %>" runat="server" />
        <w2c:ProductIcon ID="ProductIcon5" IconNo="5" ProductMaster="<%# Container.DataItem %>" runat="server" />
        <w2c:ProductIcon ID="ProductIcon6" IconNo="6" ProductMaster="<%# Container.DataItem %>" runat="server" />
        <w2c:ProductIcon ID="ProductIcon7" IconNo="7" ProductMaster="<%# Container.DataItem %>" runat="server" />
        <w2c:ProductIcon ID="ProductIcon8" IconNo="8" ProductMaster="<%# Container.DataItem %>" runat="server" />
        <w2c:ProductIcon ID="ProductIcon9" IconNo="9" ProductMaster="<%# Container.DataItem %>" runat="server" />
        <w2c:ProductIcon ID="ProductIcon10" IconNo="10" ProductMaster="<%# Container.DataItem %>" runat="server" />
      </p>
      <%-- △商品アイコン△ --%>
      <span><%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_NAME)) %></span>
    </div>
    <div class="product-price">
      <%-- ▽商品会員ランク価格有効▽ --%>
      <p visible='<%# ProductPage.GetProductMemberRankPriceValid(Container.DataItem) %>' runat="server" class="special">
        <%#: CurrencyManager.ToPrice(ProductPage.GetProductMemberRankPrice(Container.DataItem)) %>
        <span class="line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></span>
      </p>
      <%-- △商品会員ランク価格有効△ --%>

      <%-- ▽商品セール価格有効▽ --%>
      <p visible='<%# ProductPage.GetProductTimeSalesValid(Container.DataItem) %>' runat="server" class="special">
        <%#: CurrencyManager.ToPrice(ProductPage.GetProductTimeSalePriceNumeric(Container.DataItem)) %>
        <span class="line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></span>
      </p>
      <%-- △商品セール価格有効△ --%>

      <%-- ▽商品特別価格有効▽ --%>
      <p visible='<%# ProductPage.GetProductSpecialPriceValid(Container.DataItem) %>' runat="server" class="special">
        <%#: CurrencyManager.ToPrice(ProductPage.GetProductSpecialPriceNumeric(Container.DataItem)) %>
        <span class="line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></span>
      </p>
        <%-- △商品特別価格有効△ --%>

        <%-- ▽商品通常価格有効▽ --%>
        <p visible='<%# ProductPage.GetProductNormalPriceValid(Container.DataItem) %>' runat="server">
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
     </div>
    </a>
  </li>
  </ItemTemplate>
  <FooterTemplate>
  </ul>
  </section>
  </FooterTemplate>
  </asp:Repeater>
  <% } %>
  <%-- △商品ランキング一覧ループ△ --%>
<%-- △編集可能領域△ --%>