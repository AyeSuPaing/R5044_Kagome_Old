<%--
=========================================================================================================
  Module      : 商品レコメンドリスト出力コントローラ(BodyProductRecommendByRecommendEngine.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="ProductRecommendByRecommendEngineUserControl" %>
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
<%-- △編集可能領域△ --%>
}
</script>

 <% if(this.IsAsync == false){ %>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<%-- ▽タイトル▽ --%>
<div id="divRecommendTitle" visible="false" runat="server">
<%if (this.RecommendCode == "p001"){%>
<h3>あなたの閲覧履歴からおすすめの商品はこちら</h3>
<%} %>
<%if (this.RecommendCode == "p002") {%>
<h3>あなたのお気に入りからおすすめの商品はこちら</h3>
<%} %>
<%if (this.RecommendCode == "p003") {%>
<h3>あなたにおすすめの人気商品はこちら</h3>
<%} %>
<%if (this.RecommendCode == "p004") {%>
<h3>あなたにおすすめの新着商品はこちら</h3>
<%} %>
<%if (this.RecommendCode == "p006") {%>
<h3>このカテゴリでの閲覧履歴からおすすめの商品はこちら</h3>
<%} %>
<%if (this.RecommendCode == "p009") {%>
<h2>この商品を見た人は他にもこんな商品も見ています</h2>
<%} %>
<%if (this.RecommendCode == "p010") {%>
このカテゴリでの閲覧履歴からおすすめの商品はこちら
<%} %>
<%if (this.RecommendCode == "p014") {%>
<h3>このショッピングカートにある商品を買った人は、他にもこんな商品も買っています</h3>
<%} %>
<%if (this.RecommendCode == "p015") {%>
<h3>この商品を見た人は他にもこんな商品も見ています</h3>
<%} %>
<%if (this.RecommendCode == "p016") {%>
<h3>最近の注文履歴からあなたにおすすめの商品はこちら</h3>
<%} %>
<%if (this.RecommendCode == "p017") {%>
<h3>あなたの閲覧履歴からおすすめの商品はこちら</h3>
<%} %>
<%if (this.RecommendCode == "p018") {%>
<h3>あなたの注文履歴からおすすめの商品はこちら</h3>
<%} %>
<%if (this.RecommendCode == "p019") {%>
<h3>あなたにおすすめの人気商品</h3>
<%} %>
<%if (this.RecommendCode == "p020") {%>
<h3>あなたにおすすめの新着商品</h3>
<%} %>
<%if (this.RecommendCode == "p021") {%>
<h3>あなたのお気に入りリストにある商品を買った人は、他にもこんな商品も買っています</h3>
<%} %>
<%if (this.RecommendCode == "p022") {%>
<h3>入荷お知らせメールにお申し込みいただいた商品を買った人は、他にもこんな商品も買っています</h3>
<%} %>
</div>
<%-- △タイトル△ --%>

<%-- ▽おすすめ商品一覧ループ▽ --%>
<asp:Repeater id="rRecommendProducts" runat="server">
<HeaderTemplate>
<div id="dvRecommend">
<dl>
</HeaderTemplate>
<ItemTemplate>
<dd class="productInfoList">
<ul class="productInfoBlock clearFix">
<li class="thumnail">
<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateSendTrackingLogUrl(Container.DataItem)) %>">
	<w2c:ProductImage ImageTagId="picture" ImageSize="<%# this.ImageSize %>" ProductMaster="<%# Container.DataItem %>" IsVariation="false" runat="server" /></a>
	<%-- ▽在庫切れ可否▽ --%>
	<span visible='<%# ProductListUtility.IsProductSoldOut(Container.DataItem) %>' runat="server" class="soldout">SOLDOUT</span>
	<%-- △在庫切れ可否△ --%>
</li>
<li class="productInfo">
<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateSendTrackingLogUrl(Container.DataItem)) %>">
	<%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_NAME)) %></a><br />
<%-- ▽商品会員ランク価格有効▽ --%>
<span visible='<%# ProductPage.GetProductMemberRankPriceValid(Container.DataItem) %>' runat="server">
<strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem))%></strike>
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
<%#: CurrencyManager.ToPrice(ProductPage.GetProductSpecialPriceNumeric(Container.DataItem)) %></span>
<%-- △商品特別価格有効△ --%>
<%-- ▽商品通常価格有効▽ --%>
<span visible='<%# ProductPage.GetProductNormalPriceValid(Container.DataItem) %>' runat="server">
<%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></span>
<%-- △商品通常価格有効△ --%>
<%-- ▽商品定期購入価格▽ --%>
<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) {%>
<span visible='<%# (StringUtility.ToValue(ProductPage.GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG), "").ToString() != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) %>' runat="server">
	<span visible='<%# ProductPage.IsProductFixedPurchaseFirsttimePriceValid(Container.DataItem) %>' runat="server">
		<br />
		定期初回:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchaseFirsttimePrice(Container.DataItem)) %>
	</span>
	<br />
	定期通常:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchasePrice(Container.DataItem)) %>
</span>
<% } %>
<%-- △商品定期購入価格△ --%>
</li>
</ul>
</dd>
</ItemTemplate>
<FooterTemplate>
</dl>
</div>
</FooterTemplate>
</asp:Repeater>
<%-- △おすすめ商品一覧ループ△ --%>
	
<%-- ▽表示商品がない場合の代替レコメンド▽ --%>
<%
	Control control = null;
	int maxDispCount = 0;
	int.TryParse(Request[Constants.REQUEST_KEY_MAX_DISP_COUNT], out maxDispCount);
	string imageSize = null;
	if (Request[Constants.REQUEST_KEY_IMAGE_SIZE] != null) imageSize = Request[Constants.REQUEST_KEY_IMAGE_SIZE];
	switch (this.RecommendCode)
	{
		case "p001":
		case "p002":
		case "p003":
		case "p004":
		case "p009":
		case "p014":
		case "p015":
		case "p016":
		case "p017":
		case "p018":
		case "p019":
		case "p020":
			control = LoadControl("~/Form/Common/Product/BodyProductRecommend.ascx");
			((ProductRecommendUserControl)control).MaxDispCount = maxDispCount;
			((ProductRecommendUserControl)control).ImageSize = imageSize;
			divAlternativeRecommend.Controls.Add(control);
			break;

		case "p006":
		case "p010":
			control = LoadControl("~/Form/Common/Product/Parts010RCMD_003.ascx");
			((ProductRecommendUserControl)control).MaxDispCount = maxDispCount;
			((ProductRecommendUserControl)control).ImageSize = imageSize;
			divAlternativeRecommend.Controls.Add(control);
			break;

		case "p021":
		case "p022":
			control = LoadControl("~/Form/Common/Product/BodyUserProductRecommend.ascx");
			((ProductRecommendUserControl)control).MaxDispCount = maxDispCount;
			((ProductRecommendUserControl)control).ImageSize = imageSize;
			divAlternativeRecommend.Controls.Add(control);
			break;

		default:
			control = LoadControl("~/Form/Common/Product/BodyProductRecommend.ascx");
			((ProductRecommendUserControl)control).MaxDispCount = maxDispCount;
			((ProductRecommendUserControl)control).ImageSize = imageSize;
			divAlternativeRecommend.Controls.Add(control);
			break;
	}
%>
<div id="divAlternativeRecommend" runat="server">
	<%-- 代替レコメンド表示エリア --%>
</div>
<%-- △表示商品がない場合の代替レコメンド△ --%>
<%-- △編集可能領域△ --%>

<% }else{ %>

<%-- レコメンド格納用タグ --%>
<div runat="server" id="divSetRecommend" class="loading"><img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/loading.gif" alt="Loading" /></div>
<%-- レコメンド非同期用javascript --%>
<script type="text/javascript" charset="UTF-8">
<!--
	$(function(){
		setTimeout(function(){
			<%-- レコメンド格納用タグID取得 --%>
			var id = "<%= divSetRecommend.ClientID %>";
			<%-- レコメンドデータ（HTML文字列）取得 --%>
			$.ajax({
				url: "<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_COMMON_BODY_PRODUCT_RECOMMEND_BY_RECOMMEND_ENGINE %>",
				type: "post",
				data: {
					'<%= Constants.REQUEST_KEY_SHOP_ID %>': '<%= HttpUtility.JavaScriptStringEncode(this.ShopId) %>',
					'<%= Constants.REQUEST_KEY_CATEGORY_ID %>': '<%= HttpUtility.JavaScriptStringEncode(this.CategoryId) %>',
					'<%= Constants.REQUEST_KEY_PRODUCT_ID %>': '<%= HttpUtility.JavaScriptStringEncode(this.ProductId) %>',
					'<%= Constants.REQUEST_KEY_VARIATION_ID %>': '<%= HttpUtility.JavaScriptStringEncode(this.VariationId) %>',
					'<%= Constants.REQUEST_KEY_SEARCH_WORD %>': '<%= HttpUtility.JavaScriptStringEncode(this.SearchWord) %>',
					'<%= Constants.REQUEST_KEY_CAMPAINGN_ICOM %>': '<%= HttpUtility.JavaScriptStringEncode(this.CampaignIcon) %>',
					'<%= Constants.REQUEST_KEY_MIN_PRICE %>': '<%= this.MinPrice %>',
					'<%= Constants.REQUEST_KEY_MAX_PRICE %>': '<%= this.MaxPrice %>',
					'<%= Constants.REQUEST_KEY_RECOMMEND_CODE %>': '<%= HttpUtility.JavaScriptStringEncode(this.RecommendCode) %>',
					'<%= Constants.REQUEST_KEY_ITEM_CODE %>': '<%= HttpUtility.JavaScriptStringEncode(this.ItemCode) %>',
					'<%= Constants.REQUEST_KEY_MAX_DISP_COUNT %>': '<%= this.MaxDispCount %>',
					'<%= Constants.REQUEST_KEY_IMAGE_SIZE %>': '<%= HttpUtility.JavaScriptStringEncode(this.ImageSize) %>'
				},
				async: false,
				success: function (data) {
					<%-- DOMを使ってレコメンドデータを操作 --%>
					var el = document.createElement("div");
					el.innerHTML = data;

					if (el.querySelectorAll) { // for IE8, Fx3.5, Safari4, Chrome
						var nodes = el.querySelectorAll('#divData');
						document.getElementById(id).innerHTML = nodes[0].innerHTML;
					} else {
						var st = el.getElementsByTagName('div');
						var nodes = [];
						for (var i = 0; i < st.length; i++) {
							if (st[i].id == 'divData') {
								nodes.push(st[i].innerHTML);
								document.getElementById(id).innerHTML = st[i].innerHTML;
							}
						}
					}
				},
				error: function (data) {
					document.getElementById(id).innerHTML = "";
				}
			});

			<%-- 読込み中画像削除 --%>
			document.getElementById(id).className = "";

			$(".productInfoList").heightLine().biggerlink({ otherstriggermaster: false });

		},1000);
	});
// -->
</script>

 <% } %>
