<%--
=========================================================================================================
  Module      : 商品一覧ソートリンク出力コントローラ(BodyProductSortBox.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="c#" AutoEventWireup="true" Inherits="Form_Common_Product_BodyProductSortBox" CodeFile="~/Form/Common/Product/BodyProductSortBox.ascx.cs" %>
<%@ Import Namespace="ProductListDispSetting" %>
<div id="sortBox" class="clearFix">
<%--- 表示件数 ---%>
<% if (this.DisplayCountFilter) { %>
<asp:Repeater ID="rNumberDisplayLinks" runat="server">
	<HeaderTemplate>
	<div class="box clearFix">
	<p class="title">表示件数</p>
	<ul class="nav clearFix">
	</HeaderTemplate>
		<ItemTemplate>
		<%--- 未選択 ---%>
		<li visible='<%# this.DisplayCount != (int)Container.DataItem %>' runat="server">
		<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateDisplayCountUrl(int.Parse(WebSanitizer.HtmlEncode(Container.DataItem)))) %>'>
		<%# WebSanitizer.HtmlEncode(Container.DataItem)%></a>
		</li>
		<%--- 選択中 ---%>
		<li visible='<%# this.DisplayCount == (int)Container.DataItem %>' class='active' runat="server">
		<%# WebSanitizer.HtmlEncode(Container.DataItem)%>
		</li>
		</ItemTemplate>
	<FooterTemplate>
	</ul>
	</div>
	</FooterTemplate>
</asp:Repeater>
<% } %>

<%--- 並び替え ---%>
<% if (ProductListDispSettingUtility.SortSetting.Length > 1) { %>
<asp:Repeater ID="rSortList" runat="server">
	<HeaderTemplate>
	<div class="box clearFix">
	<p class="title">並び替え</p>
	<ul class="nav clearFix">
	</HeaderTemplate>
		<ItemTemplate>
		<%--- 未選択 ---%>
		<li visible='<%# this.SortKbn != ((ProductListDispSettingModel)Container.DataItem).SettingId %>' runat="server"><a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateSortUrl(((ProductListDispSettingModel)Container.DataItem).SettingId)) %>"><%# WebSanitizer.HtmlEncode(((ProductListDispSettingModel)Container.DataItem).SettingName)%></a></li>
		<%--- 選択中 ---%>
		<li visible='<%# this.SortKbn == ((ProductListDispSettingModel)Container.DataItem).SettingId %>' class='active' runat="server">
		<%# WebSanitizer.HtmlEncode(((ProductListDispSettingModel)Container.DataItem).SettingName)%></li>
		</ItemTemplate>
	<FooterTemplate>
		<%--- 未選択 ---%>
		<li visible='<%# (string.IsNullOrEmpty(this.ProductGroupId) == false) && (this.SortKbn != Constants.KBN_SORT_PRODUCT_LIST_PRODUCT_GROUP_ITEM_NO_ASC)  %>' runat="server">
			<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateSortUrl(Constants.KBN_SORT_PRODUCT_LIST_PRODUCT_GROUP_ITEM_NO_ASC)) %>">特集おすすめ順</a></li>
		<%--- 選択中 ---%>
		<li visible='<%# (string.IsNullOrEmpty(this.ProductGroupId) == false) && (this.SortKbn == Constants.KBN_SORT_PRODUCT_LIST_PRODUCT_GROUP_ITEM_NO_ASC) %>'
			class='active' runat="server">特集おすすめ順</li>
	</ul>
	</div>
	</FooterTemplate>
</asp:Repeater>
<% } %>
	
<% if (this.DisplayChangeFilter) { %>
<asp:Repeater ID="rImgList" runat="server">
	<HeaderTemplate>
	<div class="box clearFix">
	<p class="title">表示切替</p>
	<ul class="nav clearFix">
	</HeaderTemplate>
		<ItemTemplate>
		<%--- 未選択 ---%>
		<li visible='<%# this.DispImageKbn != ((ProductListDispSettingModel)Container.DataItem).SettingId %>' runat="server"><a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateImageDispTypeUrl(((ProductListDispSettingModel)Container.DataItem).SettingId)) %>"><%# WebSanitizer.HtmlEncode(((ProductListDispSettingModel)Container.DataItem).SettingName)%></a></li>
		<%--- 選択中 ---%>
		<li visible='<%# this.DispImageKbn == ((ProductListDispSettingModel)Container.DataItem).SettingId %>' class='active' runat="server">
		<%# WebSanitizer.HtmlEncode(((ProductListDispSettingModel)Container.DataItem).SettingName)%></li>
		</ItemTemplate>
	<FooterTemplate>
	</ul>
	</div>
	</FooterTemplate>
</asp:Repeater>
<% } %>

<% if (this.DisplayStockFilter) { %>
<%--- 在庫有無 ---%>
<asp:Repeater ID="rStockList" runat="server">
	<HeaderTemplate>
		<div class="box clearFix">
		<p class="title">在庫</p>
		<ul class="nav clearFix">
	</HeaderTemplate>
	<ItemTemplate>
		<%--- 未選択 ---%>
		<li visible='<%# this.UndisplayNostock != ((ProductListDispSettingModel)Container.DataItem).SettingId %>' runat="server"><a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateDisplayStockUrl(((ProductListDispSettingModel)Container.DataItem).SettingId)) %>"><%# WebSanitizer.HtmlEncode(((ProductListDispSettingModel)Container.DataItem).SettingName)%></a></li>
		<%--- 選択中 ---%>
		<li visible='<%# this.UndisplayNostock == ((ProductListDispSettingModel)Container.DataItem).SettingId %>' class='active' runat="server">
			<%# WebSanitizer.HtmlEncode(((ProductListDispSettingModel)Container.DataItem).SettingName)%></li>
	</ItemTemplate>
	<FooterTemplate>
	</ul>
	</div>
	</FooterTemplate>
</asp:Repeater>
<% } %>

<% if (this.DisplayFixedPurchaseFilter) {%>
<%--- 定期購入フィルタ ---%>
<div class="box clearFix">
	<p class="title">通常・定期</p>
	<ul class="nav clearFix">
		<li<%if (this.FixedPurchaseFilter == Constants.KBN_PRODUCT_LIST_FIXED_PURCHASE_FILTER_ALL){%> class="active"<% } %>><%if (this.FixedPurchaseFilter != Constants.KBN_PRODUCT_LIST_FIXED_PURCHASE_FILTER_ALL){%>
		<a href="<%#: CreateFixedPurchaseFilterUrl(Constants.KBN_PRODUCT_LIST_FIXED_PURCHASE_FILTER_ALL) %>">すべて表示</a><%}
		else{%>すべて表示<%} %></li>
		<li<%if (this.FixedPurchaseFilter == Constants.KBN_PRODUCT_LIST_FIXED_PURCHASE_FILTER_NORMAL){%> class="active"<% } %>><%if (this.FixedPurchaseFilter != Constants.KBN_PRODUCT_LIST_FIXED_PURCHASE_FILTER_NORMAL){%>
		<a href="<%#: CreateFixedPurchaseFilterUrl(Constants.KBN_PRODUCT_LIST_FIXED_PURCHASE_FILTER_NORMAL) %>">通常購入可能</a><%}
		else{%>通常購入可能<%} %></li>
		<li<%if (this.FixedPurchaseFilter == Constants.KBN_PRODUCT_LIST_FIXED_PURCHASE_FILTER_FIXED_PURCHASE){%> class="active"<% } %>><%if (this.FixedPurchaseFilter != Constants.KBN_PRODUCT_LIST_FIXED_PURCHASE_FILTER_FIXED_PURCHASE){%>
		<a href="<%#: CreateFixedPurchaseFilterUrl(Constants.KBN_PRODUCT_LIST_FIXED_PURCHASE_FILTER_FIXED_PURCHASE) %>">定期購入可能</a><%}
		else{%>定期購入可能<%} %></li>
	</ul>
</div>
<% } %>
</div>
