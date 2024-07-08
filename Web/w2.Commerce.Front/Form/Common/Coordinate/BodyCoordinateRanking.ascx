<%--
=========================================================================================================
Module      : コーディネートランキング出力コントローラ(BodyCoordinateRanking.ascx)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
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

	// 表示件数
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
<% if(this.CoordinateList.Length > 0) { %>
	<%-- ▽コーディネート一覧ループ▽ --%>
	<asp:Repeater ID="rRankingList" runat="server">
		<HeaderTemplate>
			<div id="dvTopRanking" class="unit">
			<h3>COORDINATE RANKING<span>コーディネートランキング</span></h3>
			<div class="listProduct clearFix">
		</HeaderTemplate>
		<ItemTemplate>
			<div class="rankingList column4">
				<p class="rank">No.<%# Container.ItemIndex+1 %></p>
				<ul>
					<li class="coordinatethumb">
						<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CoordinatePage.CreateCoordinateDetailUrl(Container.DataItem)) %>'>
							<img  border="0" src="<%#: CoordinatePage.CreateCoordinateImageUrl(((CoordinateModel)Container.DataItem).CoordinateId , 1) %>" /></a>
					</li>
					<li class="name">
						<a href="#" class="pid" >
							<div runat="server" Visible="<%# ShouldShowStaff((CoordinateModel)Container.DataItem) %>">
								<img align="left" style="padding-right: 5px;" id="picture" src="<%# CoordinatePage.GetStaffImagePath(((CoordinateModel)Container.DataItem).StaffId) %>" alt="スタッフ画像" height="50px" border="0"/>
								<p style="padding-bottom: 2px;"><%# ((CoordinateModel)Container.DataItem).StaffName %></p>
								<p style="padding-bottom: 2px;"><%# ((CoordinateModel)Container.DataItem).StaffHeight %>cm</p>
							</div>
							<p style="padding-bottom: 2px;"><%# ((CoordinateModel)Container.DataItem).RealShopName %></p>
							<% if(this.SummaryClass == "LIKE") { %>
							<p style="padding-bottom: 2px;">Like:<%# ((CoordinateModel)Container.DataItem).RankingCount %></p>
							<% } %>
							<% if(this.SummaryClass == "PV") { %>
							<p style="padding-bottom: 2px;">View:<%# ((CoordinateModel)Container.DataItem).RankingCount %></p>
							<% } %>
							<% if(this.SummaryClass == "CV") { %>
							<p style="padding-bottom: 2px;">CV:<%# ((CoordinateModel)Container.DataItem).RankingCount %></p>
							<% } %>
						</a>
					</li>
				</ul>
			</div>
		</ItemTemplate>
		<FooterTemplate>
		</div>
			</div>
		</FooterTemplate>
	</asp:Repeater>
	<%-- △コーディネート一覧ループ△ --%>
<% } %>
<%-- △編集可能領域△ --%>