<%--
=========================================================================================================
  Module      : スマートフォン用新着情報出力コントローラ(BodyNews.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
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
<section class="news unit">
  <h3 class="title">新着情報</h3>
  <!--
  <a href="javascript:show_popup_window('<%= Constants.PATH_ROOT %>Form/NewsList.aspx?<%= Constants.REQUEST_KEY_BRAND_ID %>=<%= Request[Constants.REQUEST_KEY_BRAND_ID] %>', 680, 350, true, true, 'Information')">一覧を見る</a>
  -->
  <%-- ▽新着情報ループ▽ --%>
  <asp:Repeater ID="rTopNewsList" runat="server" ItemType="NewsModel">
    <HeaderTemplate>
    <dl class="clearfix">
    </HeaderTemplate>
    <ItemTemplate>
      <dt visible='<%# Container.ItemIndex <= 3 %>' runat="server"><%#: DateTimeUtility.ToStringFromRegion(Item.DisplayDateFrom, DateTimeUtility.FormatType.ShortDate2Letter) %></dt>
      <dd visible='<%# Container.ItemIndex <= 3 %>' runat="server"><%# Item.GetNewsTextReplaceBr() %></dd>
    </ItemTemplate>
    <FooterTemplate>
    </dl>
    </FooterTemplate>
  </asp:Repeater>
  <%-- △新着情報ループ△ --%>
</section>
<% } %>
<%-- △編集可能領域△ --%>