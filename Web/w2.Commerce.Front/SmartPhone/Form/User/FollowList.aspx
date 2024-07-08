<%--
=========================================================================================================
  Module      : スマートフォン用フォロー一覧画面(FollowList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/FollowList.aspx.cs" Inherits="Form_User_FollowList" Title="フォローリスト" %>
<%@ Import Namespace="w2.Domain.Staff" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-user favorite-list">
<div class="user-unit">
	<h2>フォローリスト</h2>
	<p class=msg>フォローしているスタッフ一覧です。</p>

	<%-- ページャ --%>
	<div class="pager-wrap above"><asp:Label id="lPager1" runat="server"></asp:Label></div>

	<!-- お気に入りリスト一覧 -->
	<asp:Repeater ID="rFollowList" runat="server">
		<HeaderTemplate>
		</HeaderTemplate>
		<ItemTemplate>
		<table class="cart-table">
		<tbody>
			<tr class="cart-unit-product">
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
				<td class="product-control">
					<div class="delete">
						<asp:LinkButton id="LinkButton1" Text="削除" CssClass="btn" CommandArgument="<%# ((StaffModel)Container.DataItem).StaffId %>" OnClientClick="return confirm('本当に削除してもよろしいですか？')" OnClick="lbDelete_Click" runat="server"></asp:LinkButton>
					</div>
				</td>
			</tr>
		</tbody>
		</table>
		</ItemTemplate>
		<FooterTemplate>
		</FooterTemplate>
	</asp:Repeater>
	<%-- エラーメッセージ --%>
	<div class="msg-alert"><asp:Label id="lAlertMessage" runat="server"></asp:Label></div>
	
	<%-- ページャ --%>
	<div class="pager-wrap below"><asp:Label id="lPager2" runat="server"></asp:Label></div>

</div>

<div class="user-footer">
	<div class="button-next">
		<a href="<%= WebSanitizer.HtmlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_MYPAGE) %>" class="btn">マイページトップへ</a>
	</div>
</div>

</section>
</asp:Content>