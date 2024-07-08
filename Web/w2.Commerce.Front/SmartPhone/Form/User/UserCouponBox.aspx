<%--
=========================================================================================================
  Module      : スマートフォン用クーポンBOX画面(UserCouponBox.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserCouponBox.aspx.cs" Inherits="Form_User_CouponBox" Title="クーポンBOXページ" %>
<%@ Import Namespace="w2.Domain.Coupon.Helper" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<link href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "SmartPhone/Css/order.css") %>" rel="stylesheet" type="text/css" media="all" />
<link href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "SmartPhone/Css/user.css") %>" rel="stylesheet" type="text/css" media="all" />
<script type="text/javascript" charset="Shift_JIS" src="<%= Constants.PATH_ROOT %>Js/floatingWindow.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<section class="wrap-order order-history-list">
<%-- UPDATE PANEL開始 --%>
<asp:UpdatePanel ID="upUpdatePanel" runat="server">
<ContentTemplate>
<div class="order-unit">
	<h2>クーポンBOX</h2>
	<%-- 表示対象クーポンなし --%>
	<% if(StringUtility.ToEmpty(this.AlertMessage) != "") {%>
	<div class="msg-alert">
		<%: this.AlertMessage %>
	</div>
	<%} else { %>
		<p class="msg">ご利用いただけるクーポンの一覧です。</p>
	<%} %>
	<asp:Repeater ID="rCouponList" ItemType="UserCouponDetailInfo" Runat="server">
		<HeaderTemplate>
			<%-- ページャ --%>
			<div class="pager-wrap above"><%= this.PagerHtml %></div>
			<div class="content">
			<ul>
		</HeaderTemplate>
		<ItemTemplate>
			<div>
			<li>
				<h3 style="color: #fff; background-color: #000; padding: .5em; font-size: 14px; margin: 1em auto; border: 1px #888888; font-weight: bold;">
					<span runat="server" visible="<%# (Item.ExpireEnd < DateTime.Now) %>" >
						[有効期限切れ]<br/>
					</span>
					<%#: (StringUtility.ToEmpty(Item.CouponDispName) != "")
						? StringUtility.ToEmpty(Item.CouponDispName)
						: StringUtility.ToEmpty(Item.CouponCode) %></h3>
				<dl style="text-align: left;">
					<dd style="padding: 2px;">クーポンコード：<%#: StringUtility.ToEmpty(Item.CouponCode) %></dd>
					<dd style="padding: 2px;">有効期限：<%#: DateTimeUtility.ToStringFromRegion(Item.ExpireEnd, DateTimeUtility.FormatType.LongDateHourMinute1Letter) %></dd>
					<dd style="padding: 2px;">割引金額/割引率：
						<%#: GetCouponDiscountString(Item) %>
					</dd>
					<dd style="padding: 2px;">利用可能回数：<%#: GetCouponCount(Item) %></dd>
					<dd style="padding: 2px;"><%#: StringUtility.ToEmpty(Item.CouponDispDiscription) %></dd>
				</dl>
			</li>
		</div>
		</ItemTemplate>
		<FooterTemplate>
			</ul>
			</div>
		</FooterTemplate>
	</asp:Repeater>
	<%-- ページャ --%>
	<div class="pager-wrap below"><%= this.PagerHtml %></div>
</div>

</ContentTemplate>
</asp:UpdatePanel>
<%-- UPDATE PANELここまで --%>
	
<div class="user-footer">
	<div class="button-next">
		<a href="<%: this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_MYPAGE %>" class="btn">マイページトップへ</a>
	</div>
</div>

</section>
</asp:Content>