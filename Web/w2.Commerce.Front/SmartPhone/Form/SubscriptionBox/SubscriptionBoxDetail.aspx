<%--
=========================================================================================================
  Module      : 頒布会詳細画面(SubscriptionBoxDetail.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Form/SubscriptionBox/SubscriptionBoxDetail.aspx.cs" Inherits="Form_SubscriptionBox_SubscriptionBoxDetail" Title="頒布会詳細ページ" %>
<%@ Import Namespace="w2.Domain.SubscriptionBox" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
	<style>
		.SubscriptionOptionalItem .productName {
			width: 200px;
		}
		.SubscriptionOptionalItem .productImage {
			width: 80px;
		}
		.SubscriptionOptionalItem .productImage *{
			max-width: 100%;
		}
		.SubscriptionOptionalItem .orderCount > input{
			width: 55px;
		}
		.SubscriptionOptionalItem .orderDelete{
			width: 35px;
		}
		.SubscriptionOptionalItem table *{width:auto;}
		.SubscriptionOptionalItem table { margin: auto;}
		.SubscriptionOptionalItem .productName{width:300px;}
		.SubscriptionOptionalItem .productPrice,
		.SubscriptionOptionalItem .orderCount,
		.SubscriptionOptionalItem .orderSubtotal,
		.SubscriptionOptionalItem .orderDelete{text-align:center;}
		/* テーブルブロックごとの余白 */
		.SubscriptionOptionalItem table {margin-bottom:20px; border: 1px solid #000000;}
		.SubscriptionOptionalItem .rtitle{border-color:#ccc;background-color:#ececec; height: 30px;}
		.SubscriptionOptionalItem .title_bg{border-color:#ddd;background-color:#dcdcdc; height: 30px;}
		.dvSubscriptionOptional{height: 60px; width: 400px;}
		.subscriptionMessage { font-size: 18px; width: 400px!important; text-align: center; position: relative; top: 15px; left: 130px; height: 20px;}
		.title_bg{ width: 700px!important;}
		.btn{ padding: 6px 10px 6px 15px!important; width: 110px !important;}

		.cart-unit h2
		{
			margin-top: .1em;
			padding: .5em;
			background-color: #000;
			color: #fff;
			font-size: 15px;
		}

		.cart-table
		{
			width: 100%;
			border-top: 1px solid #efefef;
			border-bottom: 1px solid #efefef;
		}

			.cart-table tr
			{
				border-bottom: 1px dotted #efefef;
			}

			.cart-table td
			{
				padding: .5em;
				line-height: 1.5;
			}

			.cart-table .product-image
			{
				width: 25%;
				text-align: center;
			}

				.cart-table .product-image img
				{
					width: 60px;
				}

			.cart-table .product-info
			{
				width: 37%;
			}

			.cart-table td.product-control
			{
				width: 15%;
				text-align: center;
				background-color: #eee;
			}

			.cart-table td .right
			{
				text-align: center;
				margin-bottom: .5em;
			}

			.cart-table .delete
			{
				background-color: #333;
				margin-top: 1em;
			}

				.cart-table .delete a
				{
					color: #fff;
				}

		.order-btn
		{
			background-color: #000;
			margin-top: 1em;
			color: #fff;
			padding: 10px;
			margin: 10px;
		}

			.order-btn a
			{
				color: #fff;
			}
	</style>

	<h2 class="ttlB">コース名：<%: this.SubscriptionBox.DisplayName %><br>
		コースID：<%: this.SubscriptionBoxCourseId %>
		<% if (IsFixedAmount)
	 { %>
		<p class="fixedAmount">定額金額：<%: CurrencyManager.ToPrice(this.SubscriptionBox.FixedAmount) %></p>
		<% } %><br>
		<asp:Literal ID="lErrorMessage" Visible="False" runat="server" />
	</h2>
	<div class="cart-unit">
		<p class="error" visible="<%# (this.IsWithinSelectionPeriodFirstProduct == false) %>" style="margin: 10px 0; color: red;" runat="server">
			<%# WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SUBSCRIPTION_BOX_OUTSIDE_SELECTION_PERIOD) %>
		</p>
		<asp:Repeater ID="rSubscriptionBoxDeliveryTiming" runat="server">
			<ItemTemplate>
				<% if (this.SubscriptionBox.IsNumberTime) { %>
					<h2><%# (string)Container.DataItem %>回目配送商品</h2>
				<% } else { %>
					<h2><%# (string)Container.DataItem %>にお届けする商品</h2>
				<% }%>

				<asp:HiddenField ID="hfDeliveryTiming" Value="<%# (string)Container.DataItem %>" runat="server" />
				<asp:Repeater ID="rProductsList" Visible="<%# (CheckTakeOverProduct(GetItemsModel((string)Container.DataItem))) == false %>" ItemType="w2.Domain.SubscriptionBox.SubscriptionBoxDefaultItemModel" DataSource='<%# GetItemsModel((string)Container.DataItem) %>' runat="server">
					<ItemTemplate>
						<table class="cart-table">
							<tr class="cart-unit-product" runat="server">
								<td class="product-image">
									<w2c:ProductImage ID="ProductImage1" ImageSize="S" IsVariation='<%# Item.ProductId != Item.VariationId %>' ProductMaster="<%# GetProduct(Item.ShopId, Item.ProductId, Item.VariationId) %>" runat="server" Visible="<%# string.IsNullOrEmpty(Item.VariationId) == false %>" />
								</td>
								<td class="product-info">
									<ul>
										<li class="product-name">
											<a href='<%#: GetProductDetailUrl(Item.ProductId, (SubscriptionBoxDefaultItemModel)Container.DataItem) %>'>
												<%#: CreateProductName(Item.ProductId, Item.Name, Item.VariationName1, Item.VariationName2, Item.VariationName3) %>
											</a>
										</li>
										<li class="product-price" runat="server" visible="<%# this.IsFixedAmount == false %>">
											<div style="text-decoration: line-through" runat="server" Visible="<%# IsSubscriptionBoxCampaignPeriodByProductIdAndVariationId(Item.ProductId, Item.VariationId) %>">
												<%#: GetValidPrice(Item.ProductId, Item.VariationId, Item.Price, true) %>
											</div>
											<div>
												<%#: GetValidPrice(Item.ProductId, Item.VariationId, Item.Price) %>
											</div>
										</li>
									</ul>
								</td>
								<td class="product-control">
									<div class="amout">
										<%#: Item.ItemQuantity %>
									</div>
								</td>
							</tr>
						</table>
					</ItemTemplate>

				</asp:Repeater>

				<div class="product" style="background-color: white;" visible="<%# (CheckTakeOverProduct(GetItemsModel((string)Container.DataItem))) %>" runat="server">
					選択された商品を配送します。<br />
				</div>
				<asp:Repeater ID="rSubscriptionBoxDuringItemList" Visible="<%# CheckDuringTakeOverProduct((string)Container.DataItem) %>" ItemType="w2.Domain.SubscriptionBox.SubscriptionBoxDefaultItemModel" DataSource="<%# this.SubscriptionBoxDuringItemList %>" OnItemCommand="rProductChange_ItemCommand" runat="server">
					<ItemTemplate>
						<table class="cart-table">
							<tr class="cart-unit-product" runat="server">
								<td class="product-image">
									<w2c:ProductImage ID="ProductImage2" ImageSize="S" IsVariation='<%# Item.ProductId != Item.VariationId %>' ProductMaster="<%# GetProduct(Item.ShopId, Item.ProductId, Item.VariationId) %>" runat="server" Visible="<%# (string.IsNullOrEmpty(Item.VariationId) == false) %>" />
									<asp:HiddenField ID="hfTakeOverProductBranchNo" Value="<%# Item.TakeOverProductBranchNo %>" runat="server" />
								</td>
								<td class="product-info">
									<ul>
										<li>
											<asp:DropDownList ID="ddlTakeOverProductsList" DataSource="<%# GetDuringItemsList() %>" OnSelectedIndexChanged="ddlValidDefaultItemList_OnSelectedIndexChanged" SelectedValue="<%# Item.VariationId %>" DataTextField="Text" DataValueField="Value" AutoPostBack="true" runat="server" />
										</li>
										<li class="product-price" runat="server" visible="<%# this.IsFixedAmount == false %>">
											<asp:Literal Text="<%#: GetValidPrice(Item.ProductId, Item.VariationId, Item.Price) %>" runat="server" />
										</li>
									</ul>
								</td>
								<td class="product-control">
									<div class="amout">
										<asp:TextBox ID="tbQuantity" Text="<%#: Item.ItemQuantity %>" MaxLength="3" Width="28px" Visible="<%# (string.IsNullOrEmpty(Item.VariationId) == false) %>" AutoPostBack="True" CssClass="orderCount" runat="server" />
									</div>
									<div class="delete">
										<asp:LinkButton runat="server" Text="削除" CommandName="DeleteRow"></asp:LinkButton>
									</div>
								</td>
							</tr>
						</table>
					</ItemTemplate>
					<FooterTemplate>
						<asp:Button ID="addDefaultProduct" runat="server" Text="＋" OnClick="btnAddProduct_Click" AutoPostBack="true" />
					</FooterTemplate>
				</asp:Repeater>
					</div>
				</div>

				<div id="subscriptionOptionalItems" runat="server" Visible="<%# ((IDataItemContainer)Container).DisplayIndex == 0 %>">
					<div class="SubscriptionOptionalItem" id="dvListProduct" runat="server" Visible="<%# IsDispSelectProductList(Container) %>" style="width:800px;">
						<table cellspacing="0">
							<div runat="server" Visible="<%# CanNecessaryProducts(this.SubscriptionBoxCourseId) %>">
								<tr>
									<td class="title_bg" colspan="6">
										<div style="display: inline-block;" runat="server" Visible="<%# this.SubscriputionBoxProductListModify.Any() %>">
											<p style="display: inline-block;">頒布会選択商品</p>
										</div>
										<div class="right" style=" text-align: right; display: inline-block; float: right;">
											<asp:Button Text="選択する" runat="server" OnClick="btnChangeProduct_Click" class="btn" />
										</div>
										<div class="dvSubscriptionOptional" runat="server" Visible="<%# this.SubscriputionBoxProductListModify.Any() == false %>">
											<p class="subscriptionMessage">頒布会商品を選択可能です</p>
										</div>
									</td>
								</tr>
							</div>
							<asp:Repeater ID="rItem" Visible="<%# this.SubscriputionBoxProductListModify.Any() %>" DataSource="<%# this.SubscriputionBoxProductListModify %>" ItemType="w2.Domain.SubscriptionBox.SubscriptionBoxDefaultItemModel" runat="server" >
								<HeaderTemplate>
									<tr class="rtitle">
										<th class="productName" colspan="2">
											商品名 <br/>
											<% if (IsFixedAmount == false) { %>
											単価（<%#: this.ProductPriceTextPrefix %>）
											<% } %>
										</th>
										<th class="orderCount">
											注文数
										</th>
									</tr>
								</HeaderTemplate>
								<ItemTemplate>
									<tr>
										<td class="productImage">
											<w2c:ProductImage ImageSize="S" IsVariation='<%# Item.ProductId != Item.VariationId %>' ProductMaster="<%# GetProduct(Item.ShopId, Item.ProductId, Item.VariationId) %>" runat="server" Visible="True" />
										</td>
										<td class="productName">
											<%#: GetProductName(Item.ShopId, Item.ProductId, Item.VariationId) %>
											<br/>
											<% if (IsFixedAmount == false) { %>
												<div style="text-decoration: line-through" runat="server" Visible="<%# IsSubscriptionBoxCampaignPeriodByProductIdAndVariationId(Item.ProductId, Item.VariationId) %>">
													<%#: GetValidPrice(Item.ProductId, Item.VariationId, Item.Price, true) %>
												</div>
												<div>
													<%#: GetValidPrice(Item.ProductId, Item.VariationId, Item.Price) %>
												</div>
											<% } %>
										</td>
										<td class="orderCount">
											<%#: StringUtility.ToNumeric(Item.ItemQuantity)%>
										</td>
									</tr>
								</ItemTemplate>
							</asp:Repeater>
						</table>
					</div>
					<%-- 購入商品一覧 --%>
					<div class="SubscriptionOptionalItem" id="dvModifySubscription" runat="server" visible="<%# this.HasOptionalProdects == false %>" style="width:80%;" >
						<div>
							<small ID="sErrorQuantity" class="error" runat="server"></small>
						</div>
						<table cellspacing="0">
							<tr>
								<td class="title_bg" colspan="6">
									<p style="display: inline-block;">頒布会任意商品の選択</p>
									<div class="right" style="text-align: right;" >
										<asp:Button Text="  商品追加  " runat="server" class="btn" style="margin-right: 10px; display: inline-block;" OnClick="btnAddAnyProduct_Click" CommandArgument="<%# this.SubscriptionBoxCourseId %>"/>
										<asp:Button Text="  決定  " runat="server" class="btn" style="display: inline-block;" OnClick="btnUpdateProduct_Click" CommandArgument="<%# this.SubscriptionBoxCourseId %>"/>
									</div>
								</td>
							</tr>
							<asp:Repeater ID="rItemModify" DataSource="<%# this.SubscriputionBoxProductList %>" ItemType="w2.Domain.SubscriptionBox.SubscriptionBoxDefaultItemModel" runat="server" OnItemCommand="rAnyProductChange_ItemCommand">
								<HeaderTemplate>
									<tr class="rtitle">
										<th class="productName" colspan="2">
											商品名 <br>
											単価（<%#: this.ProductPriceTextPrefix %>）
										</th>
										<th class="orderCount">
											注文数
										</th>
										<th>
											削除
										</th>
									</tr>
								</HeaderTemplate>
								<ItemTemplate>
									<tr>
										<td class="productImage">
											<w2c:ProductImage ImageSize="S" IsVariation='<%# Item.ProductId != Item.VariationId %>' ProductMaster="<%# GetProduct(Item.ShopId, Item.ProductId, Item.VariationId) %>" runat="server" Visible="True" />
										</td>
										<td class="productName">
											<asp:DropDownList
												ID="ddlProductName"
												DataSource="<%# GetSubscriptionBoxProductList(Item.ProductId,Item.VariationId, Item.SubscriptionBoxCourseId) %>"
												DataTextField="Text"
												DataValueField="Value"
												OnSelectedIndexChanged="ReCalculation"
												SelectedValue='<%# string.Format("{0}/{1}/{2}", Item.ShopId, Item.ProductId, Item.VariationId) %>'
												AutoPostBack="True"
												runat="server" />
											<br/>
											<%#: CurrencyManager.ToPrice(SubscriptionBoxPrice(Item.ProductId, Item.VariationId, 1)) %>
										</td>
										<td class="orderCount">
											<asp:TextBox  ID="tbQuantityUpdate" runat="server" Text="<%# StringUtility.ToNumeric(Item.ItemQuantity) %>" OnTextChanged="ReCalculation" AutoPostBack="True" MaxLength="3" />
										</td>
										<td class="orderDelete">
											<asp:LinkButton Text="x" runat="server" CommandName="DeleteRow" CommandArgument='<%# Container.ItemIndex %>' />
										</td>
									</tr>
								</ItemTemplate>
							</asp:Repeater>
						</table>
					</div>
				</div>
			</div><!--productList-->
			</ItemTemplate>
		</asp:Repeater>
		<% if (this.IsWithinSelectionPeriodFirstProduct){ %>
		<div class="order-btn">
			<asp:LinkButton ID="lbCartAddSubscriptionBox" Text="頒布会申し込み" class="btn btn-inverse" runat="server" OnClick="lbCartAddSubscriptionBox_Click" />
		</div>
		<% } %>
	</div>
</asp:Content>
