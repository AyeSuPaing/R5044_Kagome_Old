<%--
=========================================================================================================
  Module      : スマートフォン用コーディネートランキング出力コントローラ(BodyCoordinateRanking.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Coordinate/BodyCoordinateRanking.ascx.cs" Inherits="Form_Common_Coordinate_BodyCoordinateRanking" %>
<%@ Import Namespace="w2.Domain.Coordinate" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
タイトルタグはカスタムパーツのみ利用します。
<%@ Page Title="無名のパーツ" %>
<%@ FileInfo LastChanged="最終更新者" %>

--%>

<%-- ▽編集可能領域：プロパティ設定▽ --%>
<script runat="server">
	public new void Page_Init(Object sender, EventArgs e)
	{
		base.Page_Init(sender, e);
		// 最大表示数
		this.MaxDispCount = 4;

		// 集計日数
		this.CountDays = 7;

		// 集計区分設定
		// LIKE:いいねの数
		// PV:プレイビューの数
		// CV:コンバージョンの数
		this.SummaryClass = "LIKE";
	}
</script>
<%-- △編集可能領域△ --%>

<%-- ▽編集可能領域：コンテンツ▽ --%>
  <%-- ▽コーディネートランキング一覧ループ▽ --%>
<% if(this.CoordinateList.Length > 0) { %>
  <asp:Repeater ID="rRankingList" runat="server">
  <HeaderTemplate>
  <section class="ranking unit" visible="<%# (Container.ItemIndex > 0) %>">
  <h3 class="title">コーディネートランキング</h3>
  <ul class="product-list-2 clearfix" style="padding-top: 10px;">
  </HeaderTemplate>
  <ItemTemplate>
  <li>
	<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CoordinatePage.CreateCoordinateDetailUrl(Container.DataItem)) %>'>
		<div class="product-image">
		<img src="<%#: CoordinatePage.CreateCoordinateImageUrl((((CoordinateModel)Container.DataItem).CoordinateId), 1) %>"/>
	<%-- ▽ランキング表記▽ --%>
	<span class="rank rank<%# Container.ItemIndex+1 %>"><%# Container.ItemIndex+1 %></span>
	<%-- △ランキング表記△ --%>
	</div>
	</a>
	<div class="product-name">
		<div runat="server" Visible="<%# ShouldShowStaff((CoordinateModel)Container.DataItem) %>">
			<img style="padding-right: 5px;" align="left"  id="picture" src="<%# CoordinatePage.GetStaffImagePath(((CoordinateModel)Container.DataItem).StaffId) %>" alt="スタッフ画像" height="60px" border="0"/>
			<p><%# ((CoordinateModel)Container.DataItem).StaffName %></p>
			<p><%# ((CoordinateModel)Container.DataItem).StaffHeight %>cm</p>
		</div>
		<p><%# ((CoordinateModel)Container.DataItem).RealShopName %></p>
		<% if(this.SummaryClass == "LIKE") { %>
			<p>Like:<%# ((CoordinateModel)Container.DataItem).RankingCount %></p>
		<% } %>
		<% if(this.SummaryClass == "PV") { %>
			<p>View:<%# ((CoordinateModel)Container.DataItem).RankingCount %></p>
		<% } %>
		<% if(this.SummaryClass == "CV") { %>
			<p>CV:<%# ((CoordinateModel)Container.DataItem).RankingCount %></p>
		<% } %>
	</div>
  </li>
  </ItemTemplate>
  <FooterTemplate>
  </ul>
  </section>
  </FooterTemplate>
  </asp:Repeater>
  <% } %>
  <%-- △コーディネートランキング一覧ループ△ --%>
<%-- △編集可能領域△ --%>