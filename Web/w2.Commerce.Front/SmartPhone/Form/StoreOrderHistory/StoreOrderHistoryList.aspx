<%--
=========================================================================================================
  Module      : 店舗購入履歴一覧 (StoreOrderHistoryList.aspx)
  ････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/StoreOrderHistory/StoreOrderHistoryList.aspx.cs" Inherits="Form_StoreOrderHistory_StoreOrderHistoryList" Title="店舗購入履歴一覧" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
	<section class="wrap-order order-history-list">
	<div class="order-unit">
		<h2>店舗購入履歴一覧</h2>
		<div class="pager-wrap above"><%= this.PagerHtml %></div>
		<asp:Repeater id="rStoreOrderHistoryList" ItemType="w2.App.Common.CrossPoint.OrderHistory.OrderHistoryApiResult" runat="server">
			<HeaderTemplate>
				<div class="content">
				<ul>
			</HeaderTemplate>
			<ItemTemplate>
				<li>
					<h3>伝票番号</h3>
					<h4 class="order-id"><a href='<%#: CreateUrlToDetail(Item.OrderNo) %>'>
						<%#: Item.OrderNo %></a>
					</h4>
					<dl class="order-form">
						<dt>購入店舗</dt>
						<dd><%#: Item.ShopName %></dd>
						<dt>ご購入日</dt>
						<dd><%#: StringUtility.ToDateString(Item.OrderDate, "yyyy/MM/dd") %></dd>
						<dt>総合計</dt>
						<dd><%#: CurrencyManager.ToPrice(Item.PriceTotalInTax) %></dd>
					</dl>
				</li>
			</ItemTemplate>
			<FooterTemplate>
				</ul>
				</div>
			</FooterTemplate>
		</asp:Repeater>
		<% if (string.IsNullOrEmpty(StringUtility.ToEmpty(this.ErrorMessage)) == false) { %>
		<br />
		<p><%= this.ErrorMessage %></p>
		<% } %>
		<div class="pager-wrap above"><%= this.PagerHtml %></div>
	</div>
	</section>
</asp:Content>
