<%--
=========================================================================================================
  Module      : いいねリスト出力画面(LikeList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/LikeList.aspx.cs" Inherits="Form_User_LikeList" Title="いいねリスト" %>
<%@ Import Namespace="System.ComponentModel" %>
<%@ Import Namespace="w2.Domain.Coordinate" %>

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
		<h2>いいねリスト</h2>
	<div id="dvFavoriteList" class="unit">
		<h4>いいねしているコーディネート一覧です。</h4>
		<!-- ///// ページャ ///// -->
		<div id="pagination" class="above clearFix"><asp:Label id="lPager1" runat="server"></asp:Label></div>
		<!-- ///// いいねリスト一覧 ///// -->
		<asp:Repeater ID="rLikeList" runat="server">
			<HeaderTemplate>
				<table cellspacing="0">
				<tr>
					<th ></th>
					<th >コーディネートタイトル</th>
					<th >スタッフ名</th>
					<th >&nbsp;</th>
				</tr>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td class="productImage">
						<div class="favoriteProductImage">
							<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateDetailUrl(Container.DataItem)) %>'>
								<img style="padding-right: 5px;" align="left" height="106px;" width="80px;" src="<%#: CreateCoordinateImageUrl((((CoordinateModel)Container.DataItem).CoordinateId), 1) %>"/></a>
						</div>
					</td>
					<td>
						<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateDetailUrl(Container.DataItem)) %>'>
						<%# ((CoordinateModel)Container.DataItem).CoordinateTitle %></a>
					</td>
					<td>
						<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrl(Constants.REQUEST_KEY_COORDINATE_STAFF_ID, ((CoordinateModel)Container.DataItem).StaffId)) %>'>
						<%# ((CoordinateModel)Container.DataItem).StaffName %></a>
					</td>
					<td>
						<asp:LinkButton OnClick="lbDelete_Click" id="lbDelete" Text="いいねを外す" CommandArgument="<%# ((CoordinateModel)Container.DataItem).CoordinateId %>" CssClass="btn btn-mini" OnClientClick="return confirm('本当に削除してもよろしいですか？')" style="float: right;" runat="server"></asp:LinkButton>
					</td>
				</tr>
			</ItemTemplate>
			<FooterTemplate>
			</table>
			</FooterTemplate>
		</asp:Repeater>
		<%-- エラーメッセージ --%>
		<asp:Label id="lAlertMessage" runat="server"></asp:Label>

		<!-- ///// ページャ ///// -->
		<div id="pagination" class="below clearFix"><asp:Label id="lPager2" runat="server"></asp:Label></div>
	</div>
</div>

</asp:Content>