<%--
=========================================================================================================
  Module      : カテゴリレコメンドリスト出力コントローラ(BodyCategoryRecommendByRecommendEngine.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="CategoryRecommendByRecommendEngineUserControl" %>
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
}
</script>

 <% if(this.IsAsync == false){ %>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<div id="divRecommendTitle" visible="false" runat="server">
<%if (this.RecommendCode == "p005") {%>
<h3>あなたにおすすめのカテゴリ一覧</h3>
<%} %>
<%if (this.RecommendCode == "p007") {%>
<h3>あなたにおすすめのカテゴリ一覧</h3>
<%} %>
<%if (this.RecommendCode == "p008") {%>
<h3>このカテゴリの商品を買った人は、他にもこんなカテゴリの商品を買っています</h3>
<%} %>
<%if (this.RecommendCode == "p011") {%>
<h3>あなたにおすすめのカテゴリ一覧</h3>
<%} %>
</div>
<%-- ▽おすすめカテゴリ一覧ループ▽ --%>
<asp:Repeater id="rRecommendCategories" runat="server">
<HeaderTemplate>
<div>
<dl>
</HeaderTemplate>
<ItemTemplate>
<dd class="productInfoList">
<ul class="productInfoBlock clearFix">
<li>
<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateSendTrackingLogUrl(Container.DataItem)) %>">
	<%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTCATEGORY_NAME)) %></a><br />
</li>
</ul>
</dd>
</ItemTemplate>
<FooterTemplate>
</dl>
</div>
</FooterTemplate>
</asp:Repeater>
<%-- △おすすめカテゴリ一覧ループ△ --%>

<%-- ▽おすすめカテゴリ一覧ループ（パンくずリスト表示）▽ --%>
<asp:Repeater id="rRecommendCategoryBreadcrumbs" runat="server">
<HeaderTemplate>
<div>
<dl>
</HeaderTemplate>
<ItemTemplate>
<dd class="productInfoList">
<ul class="productInfoBlock clearFix">
<li>
<%-- ▽TOPから該当カテゴリまで階層ループ▽ --%>
<asp:Repeater DataSource="<%# (List<DataRowView>)Container.DataItem %>" runat="server">
<HeaderTemplate>
<a href="<%# WebSanitizer.HtmlEncode(Constants.PATH_ROOT) %>">トップ</a>
</HeaderTemplate>
<ItemTemplate>
<span>&gt;</span>
<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateSendTrackingLogUrl(Container.DataItem)) %>'>
	<%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTCATEGORY_NAME))%></a>
</ItemTemplate>
<FooterTemplate>
</FooterTemplate>
</asp:Repeater>
<%-- △TOPから該当カテゴリまで階層ループ△ --%>
</li>
</ul>
</dd>
</ItemTemplate>
<FooterTemplate>
</dl>
</div>
</FooterTemplate>
</asp:Repeater>
<%-- △おすすめカテゴリ一覧ループ（パンくずリスト表示）△ --%>

<%-- ▽表示商品がない場合の代替レコメンド▽ --%>
<%
	switch (this.RecommendCode)
	{
		case "p005":
			divAlternativeRecommend.Controls.Add(LoadControl("~/Form/Common/Product/BodyProductRecommend.ascx"));
			break;

		case "p007":
		case "p008":
		case "p011":
			divAlternativeRecommend.Controls.Add(LoadControl("~/Page/Parts/Parts010RCMD_003.ascx"));
			break;
				
		default:
			divAlternativeRecommend.Controls.Add(LoadControl("~/Form/Common/Product/BodyProductRecommend.ascx"));
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
				url: "<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_COMMON_BODY_CATEGORY_RECOMMEND_BY_RECOMMEND_ENGINE %>",
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
					'<%= Constants.REQUEST_KEY_DISP_KBN %>': '<%= HttpUtility.JavaScriptStringEncode(this.DispKbn) %>'
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

		},1000);
	});
// -->
</script>

 <% } %>