<%--
=========================================================================================================
  Module      : クーポンBOX画面(UserCouponBox.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserCouponBox.aspx.cs" Inherits="Form_User_CouponBox" Title="クーポンBOXページ" %>
<%@ Import Namespace="w2.Domain.Coupon.Helper" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script type="text/javascript" charset="Shift_JIS" src="<%= Constants.PATH_ROOT %>Js/floatingWindow.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<%-- UPDATE PANEL開始 --%>
<asp:UpdatePanel ID="upUpdatePanel" runat="server">
<ContentTemplate>
<div id="dvUserFltContents">
	<h2>クーポンBOX</h2>
	<div class="unit">
		<h4 style="margin: 0px 0px 10px 0px;">
		<%-- 利用可能クーポンなし--%>
		<% if(StringUtility.ToEmpty(this.AlertMessage) != "") {%>
			<%: this.AlertMessage %>
		<%} else {%>
			ご利用いただけるクーポンの一覧です。
		<%} %>
		</h4>
		<asp:Repeater ID="rCouponList" ItemType="UserCouponDetailInfo" Runat="server">
			<HeaderTemplate>
			<div id="sortBox" class="clearFix">
				<%-- ページャ --%>
				<div id="pagination" class="above clearFix"><%= this.PagerHtml %></div>
			</div>
			<table>
				<tr>
					<th style="border-bottom-style:solid; border-bottom-width:1px; background-color:#ececec; padding:10px; text-align:center;width:130px;">クーポンコード</th>
					<th style="border-bottom-style:solid; border-bottom-width:1px; background-color:#ececec; padding:10px; text-align:center;width:170px;">クーポン名</th>
					<th style="border-bottom-style:solid; border-bottom-width:1px; background-color:#ececec; padding:10px; text-align:center;width:125px;">割引金額<br />/割引率</th>
					<th style="border-bottom-style:solid; border-bottom-width:1px; background-color:#ececec; padding:10px; text-align:center;width:70px;">利用可能回数</th>
					<th style="border-bottom-style:solid; border-bottom-width:1px; background-color:#ececec; padding:10px; text-align:center;width:250px;">有効期限</th>
				</tr>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td style="border-bottom-style:solid; border-bottom-width:1px; padding:10px 8px; text-align:left; text-align:center;width:130px;">
					<span runat="server" visible="<%# (Item.ExpireEnd < DateTime.Now) %>">
						[有効期限切れ]<br />
					</span>
					<%#: Item.CouponCode %></td>
					<td style="border-bottom-style:solid; border-bottom-width:1px; padding:10px 8px; text-align:left; text-align:center;width:200px;"
						title="<%#: Item.CouponDispDiscription %>">
						<%#: Item.CouponDispName %>
					</td>
					<td style="border-bottom-style:solid; border-bottom-width:1px; padding:10px 8px; text-align:left; text-align:center;width:70px;">
						<%# WebSanitizer.HtmlEncodeChangeToBr(GetCouponDiscountString(Item)) %>
					</td>
					<td style="border-bottom-style:solid; border-bottom-width:1px; padding:10px 8px; text-align:left; text-align:center;width:70px;">
						<%#: GetCouponCount(Item) %>
					</td>
					<td style="border-bottom-style:solid; border-bottom-width:1px; padding:10px 8px; text-align:left; text-align:center;width:200px;">
						<%#: DateTimeUtility.ToStringFromRegion(Item.ExpireEnd, DateTimeUtility.FormatType.LongDateHourMinute1Letter) %>
					</td>
				</tr>
			</ItemTemplate>
			<FooterTemplate>
				</table>
			</FooterTemplate>
		</asp:Repeater>
	<%-- ページャ--%>
	<div id="pagination" class="below clearFix"><%= this.PagerHtml %></div>
	</div>
</div>
</ContentTemplate>
</asp:UpdatePanel>
</asp:Content>