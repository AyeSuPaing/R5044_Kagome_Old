<%--
=========================================================================================================
  Module      : おすすめ商品ランダム出力コントローラ(BodyProductRecommendAdvanced.ascx)
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

<%-- △編集可能領域△ --%>
}
</script>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<%-- ▽おすすめ商品一覧ループ▽ --%>
<% if (this.ProductCount > 0) { %>
<asp:Repeater ID="rProducts" runat="server">
	<HeaderTemplate>
	<div id="dvRecommend" class="unit">
		<h3>RECOMMEND ITEM<span>お薦めアイテム</span></h3>
		<div class="listProduct clearFix">
	</HeaderTemplate>
	<ItemTemplate>
			<div class="glbPlist column4">
			<ul class="productInfoBlock clearFix">
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
				<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem)) %>'>
				<w2c:ProductImage ImageSize="<%# this.ImageSize %>" ProductMaster="<%# Container.DataItem %>" IsVariation="false" runat="server" /></a>
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
				<span style="text-decoration: line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %>（<%# WebSanitizer.HtmlEncode(ProductPage.GetTaxIncludeString(Container.DataItem)) %>）</span><br />
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
<%-- △おすすめ商品一覧ループ△ --%>
<%-- △編集可能領域△ --%>