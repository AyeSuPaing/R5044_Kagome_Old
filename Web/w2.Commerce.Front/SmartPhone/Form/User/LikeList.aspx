<%--
=========================================================================================================
  Module      : スマートフォン用いいねリスト出力画面(LikeList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/LikeList.aspx.cs" Inherits="Form_User_LikeList" Title="いいねリスト" %>
<%@ Import Namespace="w2.Domain.Coordinate" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<section class="wrap-user favorite-list">
<div class="user-unit">
	<h2>いいねリスト</h2>
	<p class=msg>いいねしているコーディネート一覧です。</p>

	<%-- ページャ --%>
	<div class="pager-wrap above"><asp:Label id="lPager1" runat="server"></asp:Label></div>

	<!-- いいねリスト一覧 -->
	<asp:Repeater ID="rLikeList" runat="server">
		<HeaderTemplate>
		</HeaderTemplate>
		<ItemTemplate>
		<table class="cart-table">
		<tbody>
			<tr class="cart-unit-product">
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
				<td class="product-control">
					<div class="delete">
						<asp:LinkButton id="LinkButton1" Text="削除" CssClass="btn"  CommandArgument="<%# ((CoordinateModel)Container.DataItem).CoordinateId %>" OnClientClick="return confirm('本当に削除してもよろしいですか？')" OnClick="lbDelete_Click" runat="server"></asp:LinkButton>
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