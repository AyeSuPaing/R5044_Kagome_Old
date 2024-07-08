<%--
=========================================================================================================
  Module      : スマートフォン用おすすめ商品ランダム出力コントローラ(BodyProductRecommendAdvanced.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="ProductRecommendAdvancedUserControl" %>
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
//商品最大表示件数を設定します (MAX50)
this.MaxDispCount = 10;

//商品画像サイズを設定します (S/M/L/LL)
this.ImageSize = "M";

//表示区分を設定します (IC1～10)
this.CampaignIcons = "IC10";

//カテゴリIDを設定します
this.CategoryId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_CATEGORY_ID]);

//データ取得時の並び順を設定します (ASC:昇順,DESC:降順)
this.DbSortType = "DESC";

//データ取得時の並び順の項目を設定します (product_id:商品ID,product_name:商品名,product_kana_name:商品カナ名,category_id:カテゴリID,category_name:カテゴリ名,category_kana_name:カテゴリカナ名,sell_from:販売開始日)
this.DbSortBy = "sell_from";

//データ表示時の並び順を設定します (ASC:昇順,DESC:降順)
this.SortType = "DESC";

//データ表示時の並び順の項目を設定します (product_id:商品ID,product_name:商品名,product_kana_name:商品カナ名,category_id:カテゴリID,category_name:カテゴリ名,category_kana_name:カテゴリカナ名,sell_from:販売開始日)
this.SortBy = "sell_from";

//販売開始時期を設定します (ThisMonth:今月,ThisWeek:今週,PreviousWeek:前週,PreviousMonth:前月,YYYYMMDD-YYYYMMDD:指定期間)
//this.SellTimeFrom = "ThisMonth";

//販売終了時期を設定します (ThisMonth:今月,ThisWeek:今週,PreviousWeek:前週,PreviousMonth:前月,YYYYMMDD-YYYYMMDD:指定期間)
//this.SellTimeTo = "20130620-20130623";

//商品タグの条件を設定します (プロパティは5まであります)
//this.ProductTag_1 = "tag_title=a1";

}
</script>
<%-- ▽編集可能領域：コンテンツ▽ --%>
  <%-- ▽おすすめ商品一覧ループ▽ --%>
<% if (this.ProductCount > 0) { %>
  <asp:Repeater ID="rProducts" runat="server">
  <HeaderTemplate>
  <section class="recommend unit" visible="<%# (Container.ItemIndex > 0) %>">
  <h3 class="title">新着商品</h3>
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
  <div class="view-more">
    <a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "SmartPhone/Form/Product/ProductList.aspx?cicon=1&cat=") %>" class="btn">新着商品をもっと見る</a>
  </div>
  </section>
  </FooterTemplate>
  </asp:Repeater>
<% } %>
  <%-- △おすすめ商品一覧ループ△ --%>
<%-- △編集可能領域△ --%>