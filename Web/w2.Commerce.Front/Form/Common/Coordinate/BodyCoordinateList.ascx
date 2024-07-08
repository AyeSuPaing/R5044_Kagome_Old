<%--
=========================================================================================================
Module      : コーディネートリスト出力コントローラ(BodyCoordinateList.ascx)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Coordinate/BodyCoordinateList.ascx.cs" Inherits="Form_Common_Coordinate_BodyCoordinateList" %>
<%@ Import Namespace="w2.Domain.Coordinate" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
タイトルタグはカスタムパーツのみ利用します。
<%@ Page Title="無名のパーツ" %>
<%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>

<%-- ▽編集可能領域：プロパティ設定▽ --%>
<script runat="server">
public new void Page_Init(Object sender, EventArgs e)
{
	base.Page_Init(sender, e);

	this.StaffId = "";
	this.RealShopId = "";
	this.CoordinateCategoryId = "";
	this.MaxDispCount = 5;
}
</script>
<%-- △編集可能領域△ --%>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<% if (this.CoordinateCount > 0) { %>
<asp:Repeater ID="rCoordinateList" runat="server">
	<HeaderTemplate>
		<div id="dvTopRanking" class="unit">
		<h3>NEW COORDINATES<span>新着コーディネート</span></h3>
		<div class="listProduct clearFix">
	</HeaderTemplate>
	<ItemTemplate>
		<div class="glbPlist column4">
			<ul>
				<li class="coordinatethumb">
					<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CoordinatePage.CreateCoordinateDetailUrl(Container.DataItem)) %>'>
						<img  border="0" src="<%#: CoordinatePage.CreateCoordinateImageUrl(((CoordinateModel)Container.DataItem).CoordinateId , 1) %>" /></a>
				</li>
				<li class="name">
					<a href="#" class="pid">
						<div runat="server" Visible="<%# ShouldShowStaff((CoordinateModel)Container.DataItem) %>">
							<img align="left" style="padding-right: 5px;" id="picture" src="<%# CoordinatePage.GetStaffImagePath(((CoordinateModel)Container.DataItem).StaffId) %>" alt="スタッフ画像" height="40px" border="0"/>
							<p style="padding-bottom: 2px;"><%# ((CoordinateModel)Container.DataItem).StaffName %></p>
							<p style="padding-bottom: 2px;"><%# ((CoordinateModel)Container.DataItem).StaffHeight %>cm</p>
						</div>
						<p style="padding-bottom: 2px;"><%# ((CoordinateModel)Container.DataItem).RealShopName %></p>
					</a>
				</li>
			</ul>
		</div>
	</ItemTemplate>
	<FooterTemplate>
	</div>
		<div style="text-align: center; padding-top: 20px;"><a class="btn btn-mid btn-inverse" href="<%= Constants.PATH_ROOT %>Form/Coordinate/CoordinateList.aspx">一覧を見る</a></div>
	</div>
	</FooterTemplate>
</asp:Repeater>
<% } %>
<%-- △編集可能領域△ --%>
