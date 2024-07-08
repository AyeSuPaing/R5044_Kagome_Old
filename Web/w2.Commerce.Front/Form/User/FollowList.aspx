<%--
=========================================================================================================
  Module      : フォローリスト画面(FollowList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/FollowList.aspx.cs" Inherits="Form_User_FollowList" Title="フォローリスト" %>
<%@ Import Namespace="w2.Domain.Staff" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<link href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "Css/product.css")%>" rel="stylesheet" type="text/css" media="all" />
<%-- △編集可能領域△ --%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
	
	<style>
		h3 {
			font-family: Times, serif;
			font-size: 153.9%;
			padding: 2% 1%;
		}
	</style>

<div id="dvUserFltContents">
		<h2>フォローリスト</h2>
	<div id="dvFavoriteList" class="unit">
		<h4>フォローしているスタッフ一覧です。</h4>
		<!-- ///// ページャ ///// -->
		<div id="pagination" class="above clearFix"><asp:Label id="lPager1" runat="server"></asp:Label></div>
		<!-- ///// フォローリスト一覧 ///// -->
		<asp:Repeater ID="rFollowList" runat="server">
			<HeaderTemplate>
				<table cellspacing="0">
				<tr>
					<th ></th>
					<th >スタッフ名</th>
					<th >店舗名</th>
					<th >&nbsp;</th>
				</tr>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td class="productImage">
						<div class="favoriteProductImage">
							<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrl(Constants.REQUEST_KEY_COORDINATE_STAFF_ID, ((StaffModel)Container.DataItem).StaffId)) %>">
							<img style="padding-right: 5px;" align="left"  id="picture" src="<%# GetStaffImagePath(((StaffModel)Container.DataItem).StaffId) %>" alt="スタッフ画像" height="80px;" width="80px;" border="0"/>
							</a>
						</div>
					</td>
					<td>
						<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrl(Constants.REQUEST_KEY_COORDINATE_STAFF_ID, ((StaffModel)Container.DataItem).StaffId)) %>">
							<%# ((StaffModel)Container.DataItem).StaffName %></a>
					</td>
					<td>
						<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrl(Constants.REQUEST_KEY_REAL_SHOP_ID, ((StaffModel)Container.DataItem).RealShopId)) %>">
							<%# ((StaffModel)Container.DataItem).RealShopName %></a>
					</td>
					<td>
						<asp:LinkButton id="lbDelete" OnClick="lbDelete_Click" Text="フォローを外す" CssClass="btn btn-mini" CommandArgument="<%# ((StaffModel)Container.DataItem).StaffId %>" OnClientClick="return confirm('本当に削除してもよろしいですか？')" style="float: right;" runat="server"></asp:LinkButton>
					</td>
				</tr>
			</ItemTemplate>
			<FooterTemplate>
			</table>
			</FooterTemplate>
		</asp:Repeater>
		<p><asp:Label id="lAlertMessage" runat="server"></asp:Label></p>
		<!-- ///// ページャ ///// -->
		<div id="pagination" class="below clearFix"><asp:Label id="lPager2" runat="server"></asp:Label></div>
	</div>
</div>

</asp:Content>