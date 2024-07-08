<%--
=========================================================================================================
  Module      : Store Order History Detail (StoreOrderHistoryDetail.aspx)
  ････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="StoreOrderHistoryDetail.aspx.cs" Inherits="Form_StoreOrderHistory_StoreOrderHistoryDetail" Title="店舗購入履歴詳細" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserFltContents">
	<h2>店舗購入履歴詳細</h2>
	<div id="dvOrderHistoryDetail" class="unit">
		<div id="dvStoreOderHistoryInfo">
			<h3>購入情報</h3>
			<table>
				<tr>
					<th>伝票番号</th>
					<td><%# this.ShopOrder.OrderNo %></td>
				</tr>
				<tr>
					<th>購入店舗</th>
					<td><%# this.ShopOrder.ShopName %></td>
				</tr>
				<tr>
					<th>ご購入日	</th>
					<td><%#: StringUtility.ToDateString(this.ShopOrder.OrderDate, "yyyy/MM/dd") %></td>
				</tr>
				<tr>
					<th>購入時付与ポイント</th>
					<td><%# GetNumeric(this.ShopOrder.GrantPoint) %><%: Constants.CONST_UNIT_POINT_PT %></td>
				</tr>
				<tr>
					<th>ご利用ポイント</th>
					<td><%# GetNumeric(this.ShopOrder.UsePoint) %><%: Constants.CONST_UNIT_POINT_PT %></td>
				</tr>
				<tr>
					<th style="font-weight: bold;">総合計（税込)</th>
					<td style="font-weight: bold;"><%#: CurrencyManager.ToPrice(this.ShopOrder.PriceTotalInTax) %></td>
				</tr>
			</table>
		</div>
		<br />
		<div id="dvStoreOderHistoryProduct">
			<table>
				<asp:Repeater ID="rStoreOderHistoryDetail" DataSource='<%# this.ShopOrder.Items %>' ItemType="w2.App.Common.CrossPoint.OrderHistory.OrderDetail" runat="server">
					<HeaderTemplate>
						<tr>
							<th style="width:60%">商品名</th>
							<th style="text-align:right">単価（税込）</th>
							<th style="text-align:right"class="salesNum">購入数</th>
						</tr>
					</HeaderTemplate>
					<ItemTemplate>
						<tr>
							<td style="width:60%"><%#: Item.ProductName %></td>
							<td style="text-align:right"><%#: StringUtility.ToPrice(Item.SalesPriceInTax) %></td>
							<td style="text-align:right"><%#: StringUtility.ToNumeric(Item.Quantity) %></td>
						</tr>
					</ItemTemplate>
				</asp:Repeater>
			</table>
		</div>
	</div>
</div>
</asp:Content>
