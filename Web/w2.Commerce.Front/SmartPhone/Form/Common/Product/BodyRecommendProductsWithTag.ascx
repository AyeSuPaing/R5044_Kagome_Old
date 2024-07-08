<%--
=========================================================================================================
  Module      : スマートフォン関連タグ・おすすめ商品出力コントローラ(BodyRecommendProductsWithTag.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Product/BodyRecommendProductsWithTag.ascx.cs" Inherits="Form_Common_Product_BodyRecommendProductsWithTag" %>
<%--
下記のタグはファイル情報保持用です。削除しないでください。
タイトルタグはカスタムパーツのみ利用します。
<%@ Page Title="無名のパーツ" %>
<%@ FileInfo LastChanged="ｗ２ユーザー" %>
--%>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<div id="dvTagList" Visible="<%# (this.TagList != null && this.TagList.Count != 0) %>" runat="server">
	<section class="recommend unit">
	<h3 class="title">関連タグ</h3>
		<div style="margin-top: 10px;">
	<asp:Repeater DataSource="<%# this.TagList %>" ItemType="w2.App.Common.Awoo.GetTags.Tags" runat="server" >
		<HeaderTemplate>
		</HeaderTemplate>
		<ItemTemplate>
			<a class="recommendTag" href="<%# CreateRecommendProductsUrl(Item) %>">
				<span><%# Item.Text %></span>
			</a>
		</ItemTemplate>
	</asp:Repeater>
        </div>
    </section>
</div>
<asp:Repeater ID="rProducts" DataSource="<%# this.ProductMasterList %>" Visible="<%# this.ProductMasterList != null && this.ProductMasterList.Count != 0 %>" runat="server">
  <HeaderTemplate>
  <section class="recommend unit" visible="<%# (Container.ItemIndex > 0) %>">
  <h3 class="title">おすすめ商品</h3>
  <ul class="product-list-3 clearfix">
  </HeaderTemplate>
  <ItemTemplate>
  <li>
    <a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem)) %>">
    <div class="product-image">
      <w2c:ProductImage ImageTagId="picture" ImageSize="<%# this.ImageSize %>" ProductMaster="<%# Container.DataItem %>" IsVariation="false" runat="server" />
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
			<span id="Span2" visible='<%# ProductPage.IsProductFixedPurchaseFirsttimePriceValid(Container.DataItem) %>' runat="server">
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
<%-- △編集可能領域△ --%>
