<%--
=========================================================================================================
  Module      : Store Order History List(StoreOrderHistoryList.aspx)
  ････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="StoreOrderHistoryList.aspx.cs" Inherits="Form_StoreOrderHistory_StoreOrderHistoryList" Title="店舗購入履歴一覧" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserFltContents">
	<h2>店舗購入履歴一覧</h2>
	<div id="dvFavoriteList" class="unit">
		<div id="pagination" class="above clearFix"><%= this.PagerHtml %></div>
		<asp:Repeater id="rStoreOrderHistoryList" ItemType="w2.App.Common.CrossPoint.OrderHistory.OrderHistoryApiResult" runat="server">
			<ItemTemplate>
				<table style="cursor: pointer;" onclick="window.location.href='<%#: CreateUrlToDetail(Item.OrderNo) %>';">
					<tr>
						<th>伝票番号</th>
						<th>購入店舗</th>
						<th>ご購入日</th>
						<th>総合計</th>
					</tr>
					<tr>
						<td style="width:25%">
							<%#: Item.OrderNo %>
						</td>
						<td style="width:25%">
							<%#: Item.ShopName %>
						</td>
							<td style="width:25%">
							<%#: StringUtility.ToDateString(Item.OrderDate, "yyyy/MM/dd") %>
						</td>
							<td style="width:25%">
							<%#: CurrencyManager.ToPrice(Item.PriceTotalInTax) %>
						</td>
					</tr>
				</table>
				<br />
			</ItemTemplate>
		</asp:Repeater>
		<% if (string.IsNullOrEmpty(StringUtility.ToEmpty(this.ErrorMessage)) == false) { %>
		<p><%= this.ErrorMessage %></p>
		<% } %>
		<div id="pagination" class="above clearFix"><%= this.PagerHtml %></div>
	</div>
</div>
</asp:Content>