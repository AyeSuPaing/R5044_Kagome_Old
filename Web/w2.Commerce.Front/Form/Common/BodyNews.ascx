<%--
=========================================================================================================
  Module      : 新着情報出力コントローラ(BodyNews.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace = "w2.Domain.News" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/BodyNews.ascx.cs" Inherits="Form_Common_BodyNews" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
タイトルタグはカスタムパーツのみ利用します。
<%@ Page Title="無名のパーツ" %>
<%@ FileInfo LastChanged="最終更新者" %>

--%>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<% if (rTopNewsList.DataSource != null) { %>
<div id="dvTopNews">
	<%-- ▽新着情報ループ▽ --%>
	<asp:Repeater ID="rTopNewsList" runat="server" ItemType="NewsModel">
	<HeaderTemplate>
	<p class="title">News</p>
	<ul class="news">
	</HeaderTemplate>
	<ItemTemplate>
	<li>
	<span class="date"><%#: DateTimeUtility.ToStringFromRegion(Item.DisplayDateFrom, DateTimeUtility.FormatType.ShortDate2Letter) %></span><br />
	<%# Item.GetNewsTextHtml() %>
	</li>
	</ItemTemplate>
	<FooterTemplate>
	</ul>
	</FooterTemplate>
	</asp:Repeater>
	<%-- △新着情報ループ△ --%>
	<p class="viewAll">
		<a href="javascript:show_popup_window('<%= Constants.PATH_ROOT %>Form/NewsList.aspx?<%= Constants.REQUEST_KEY_BRAND_ID %>=<%= this.BrandId %>', 680, 350, true, true, 'Information')">一覧を見る</a>
	</p>
</div>
<% } %>
<%-- △編集可能領域△ --%>