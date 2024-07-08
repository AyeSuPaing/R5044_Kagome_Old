<%--
=========================================================================================================
  Module      : 店舗購入履歴詳細 (StoreOrderHistoryDetail.aspx)
  ････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/StoreOrderHistory/StoreOrderHistoryDetail.aspx.cs" Inherits="Form_StoreOrderHistory_StoreOrderHistoryDetail" Title="店舗購入履歴詳細" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
	<section class="wrap-order order-history-detail">
		<div class="order-unit">
			<h2>店舗購入履歴一覧</h2>
			<div class="content">
				<h3>購入情報</h3>
					<dl class="order-form">
						<dt>ご注文番号</dt>
						<dd><%# this.ShopOrder.OrderNo %></dd>
						<dt>購入店舗</dt>
						<dd><%# this.ShopOrder.ShopName %></dd>
						<dt>ご購入日</dt>
						<dd><%#: StringUtility.ToDateString(this.ShopOrder.OrderDate, "yyyy/MM/dd") %></dd>
						<dt>購入時付与ポイント</dt>
						<dd><%# GetNumeric(this.ShopOrder.GrantPoint) %><%: Constants.CONST_UNIT_POINT_PT %></dd>
						<dt>ご利用ポイント</dt>
						<dd><%# GetNumeric(this.ShopOrder.UsePoint) %><%: Constants.CONST_UNIT_POINT_PT %></dd>
						<dt>総合計（税込)</dt>
						<dd><%#: CurrencyManager.ToPrice(this.ShopOrder.PriceTotalInTax) %></dd>
					</dl>
				<h3>ご注文商品</h3>
				<asp:Repeater ID="rStoreOderHistoryDetail" DataSource='<%# this.ShopOrder.Items %>' ItemType="w2.App.Common.CrossPoint.OrderHistory.OrderDetail" runat="server">
					<ItemTemplate>
						<div class="cart-unit">
							<dl>
								<dt>商品名</dt>
								<dd><%#: Item.ProductName %></dd>
								<dt>単価（税込）</dt>
								<dd><%#: StringUtility.ToPrice(Item.SalesPriceInTax) %></dd>
								<dt>購入数</dt>
								<dd><%#: StringUtility.ToNumeric(Item.Quantity) %></dd>
							</dl>
						</div>
					</ItemTemplate>
				</asp:Repeater>
			</div>
		</div>
	</section>
</asp:Content>
