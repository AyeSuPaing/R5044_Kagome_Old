<%--
=========================================================================================================
  Module      : スマートフォン用商品一覧ソートリンク出力コントローラ(BodyProductSortBox.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Product/BodyProductSortBox.ascx.cs" Inherits="Form_Common_Product_BodyProductSortBox" %>
<%@ Import Namespace="ProductListDispSetting" %>
<%--
下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LastChanged="最終更新者" %>
--%>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<% if ((ProductListDispSettingUtility.CountSetting.Length > 1) || (ProductListDispSettingUtility.SortSetting.Length > 1)) { %>

<div id="dvProductSortSearch" runat="server">
<h3>表示方法</h3>
<table>
<tbody>
<% if (this.DisplayCountFilter) { %>
<tr>
	<th>表示件数</th>
	<td>
	<%-- 表示件数 --%>
	<asp:Repeater ID="rNumberDisplayLinks" runat="server">
	<HeaderTemplate>
	<ul class="horizon">
	</HeaderTemplate>
	<ItemTemplate>
	<%-- 未選択 --%>
	<li visible='<%# this.DisplayCount != (int)Container.DataItem %>' runat="server">
		<input id="dpcnt<%# Container.ItemIndex+1 %>" name="dpcnt" type="radio" value="<%# WebSanitizer.HtmlEncode(Container.DataItem)%>" />
		<label for="dpcnt<%# Container.ItemIndex+1 %>"><%# WebSanitizer.HtmlEncode(Container.DataItem)%>件</label>
	</li>
	<%-- 選択中 --%>
	<li visible='<%# this.DisplayCount == (int)Container.DataItem %>' runat="server">
		<input id="dpcnt<%# Container.ItemIndex+1 %>" name="dpcnt" type="radio" value="<%# WebSanitizer.HtmlEncode(Container.DataItem)%>" checked="checked" />
		<label for="dpcnt<%# Container.ItemIndex+1 %>"><%# WebSanitizer.HtmlEncode(Container.DataItem)%>件</label>
	</li>
	</ItemTemplate>
	<FooterTemplate>
	</ul>
	</FooterTemplate>
	</asp:Repeater>
	</td>
</tr>
<% } %>
<% if (ProductListDispSettingUtility.SortSetting.Length > 1) { %>
<tr>
	<th>表示順</th>
	<td>
	<%-- ソート --%>
	<asp:Repeater ID="rSortList" runat="server">
	<HeaderTemplate>
	<ul class="horizon">
	</HeaderTemplate>
	<ItemTemplate>
	<%-- 未選択 --%>
	<li visible='<%# this.SortKbn != ((ProductListDispSettingModel)Container.DataItem).SettingId %>' runat="server">
		<input id="sort<%# Container.ItemIndex+1 %>" name="sort" type="radio" value="<%# WebSanitizer.HtmlEncode(((ProductListDispSettingModel)Container.DataItem).SettingId) %>" />
		<label for="sort<%# Container.ItemIndex+1 %>"><%# WebSanitizer.HtmlEncode(((ProductListDispSettingModel)Container.DataItem).SettingName) %></label>
	</li>
	<%-- 選択中 --%>
	<li visible='<%# this.SortKbn == ((ProductListDispSettingModel)Container.DataItem).SettingId %>' runat="server">
		<input id="sort<%# Container.ItemIndex+1 %>" name="sort" type="radio" value="<%# WebSanitizer.HtmlEncode(((ProductListDispSettingModel)Container.DataItem).SettingId) %>" checked="checked" />
		<label for="sort<%# Container.ItemIndex+1 %>"><%# WebSanitizer.HtmlEncode(((ProductListDispSettingModel)Container.DataItem).SettingName) %></label>
	</li>
	</ItemTemplate>
	<FooterTemplate>
	<%--- 未選択 ---%>
	<li visible='<%# (string.IsNullOrEmpty(this.ProductGroupId) == false) && (this.SortKbn != Constants.KBN_SORT_PRODUCT_LIST_PRODUCT_GROUP_ITEM_NO_ASC)  %>' runat="server">
		<input id="sort99" name="sort" value="<%:Constants.KBN_SORT_PRODUCT_LIST_PRODUCT_GROUP_ITEM_NO_ASC %>" type="radio" />
		<label>特集おすすめ順</label>
	</li>
	<%--- 選択中 ---%>
	<li visible='<%# (string.IsNullOrEmpty(this.ProductGroupId) == false) && (this.SortKbn == Constants.KBN_SORT_PRODUCT_LIST_PRODUCT_GROUP_ITEM_NO_ASC) %>' runat="server">
		<input id="sort99" name="sort" value="<%:Constants.KBN_SORT_PRODUCT_LIST_PRODUCT_GROUP_ITEM_NO_ASC %>" type="radio" checked="checked" />
		<label>特集おすすめ順</label>
	</li>
	</ul>
	</FooterTemplate>
	</asp:Repeater>
	</td>
</tr>
<% } %>
</tbody>
</table>
<div class="button">
	<a href="javascript:void(0);" class="btn-sort-search btn">並び替える</a>
</div>
</div>
<% } %>
<%-- △編集可能領域△ --%>
