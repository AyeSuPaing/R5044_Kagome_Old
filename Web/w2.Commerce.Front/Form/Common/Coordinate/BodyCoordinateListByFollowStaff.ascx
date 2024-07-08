<%--
=========================================================================================================
Module      : フォロースタッフコーディネートリスト出力コントローラ(BodyCoordinateListByFollowStaff.ascx)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Coordinate/BodyCoordinateListByFollowStaff.ascx.cs" Inherits="Form_Common_Coordinate_BodyCoordinateListByFollowStaff" %>
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

	this.MaxDispCount = 4;
}
</script>
<%-- △編集可能領域△ --%>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<% if (this.CoordinateCount > 0) { %>
	<asp:Repeater ID="rCoordinateList" runat="server">
		<HeaderTemplate>
			<div id="dvTopRanking" class="unit">
			<h3>COORDINATING STAFF<span>フォローしているスタッフのコーディネート</span></h3>
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
						<a href="#" class="pid" >
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
			<div style="text-align: center; padding-top: 20px;"><asp:LinkButton Id="lbStaffLink" OnClick="lbStaffLink_Click" class="btn btn-mid btn-inverse" runat="server">一覧を見る</asp:LinkButton></div>
		</div>
		</FooterTemplate>
	</asp:Repeater>
<% } %>
<%-- △編集可能領域△ --%>