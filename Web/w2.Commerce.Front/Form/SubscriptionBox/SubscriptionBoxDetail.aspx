<%--
=========================================================================================================
  Module      : 頒布会詳細画面(SubscriptionBoxDetail.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Form/SubscriptionBox/SubscriptionBoxDetail.aspx.cs" Inherits="Form_SubscriptionBox_SubscriptionBoxDetail" Title="頒布会詳細ページ" %>
<%@ Import Namespace="w2.Domain.SubscriptionBox" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	
<style>
	.SubscriptionOptionalItem .productName
	{
		width: 200px;
	}

	.SubscriptionOptionalItem .productImage
	{
		width: 80px;
	}

	.SubscriptionOptionalItem .productImage *
	{
		max-width: 100%;
	}

	.SubscriptionOptionalItem .orderCount > input
	{
		text-align: right;
		width: 70px;
	}

	.SubscriptionOptionalItem .orderSubtotal
	{
		width: 95px;
	}

	.SubscriptionOptionalItem .orderDelete
	{
		width: 35px;
	}

	.SubscriptionOptionalItem table *
	{
		width: auto;
	}

	.SubscriptionOptionalItem table
	{
		margin: auto;
	}

	.SubscriptionOptionalItem .productName
	{
		width: 300px;
	}

	.SubscriptionOptionalItem .productPrice,
	.SubscriptionOptionalItem .orderCount,
	.SubscriptionOptionalItem .orderSubtotal,
	.SubscriptionOptionalItem .orderDelete
	{
		text-align: center;
	}
	/* テーブルブロックごとの余白 */
	.SubscriptionOptionalItem table
	{
		margin-bottom: 20px;
		border: 1px solid #000000;
	}

	.SubscriptionOptionalItem .rtitle
	{
		border-color: #ccc;
		background-color: #ececec;
		height: 30px;
	}

	.SubscriptionOptionalItem .title_bg
	{
		border-color: #ddd;
		background-color: #dcdcdc;
		height: 30px;
	}

	.dvSubscriptionOptional
	{
		background-color: #cccccc;
		height: 60px;
		width: 500px!important;
	}

	.subscriptionMessage
	{
		font-size: 18px;
		text-align: center;
		position: relative;
		top: 20px;
		height: 20px;
	}
</style>

<h2 class="ttlB">
	コース名：<%: this.SubscriptionBox.DisplayName %><br>
	コースID：<%: this.SubscriptionBoxCourseId %><br>
	<% if (IsFixedAmount) { %>
		<p class="fixedAmount">定額金額：<%: CurrencyManager.ToPrice(this.SubscriptionBox.FixedAmount) %></p>
	<% } %><br>
</h2>
<div id="CartList">
	<p class="error" visible="<%# (this.IsWithinSelectionPeriodFirstProduct == false) %>" runat="server">
		<%# WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SUBSCRIPTION_BOX_OUTSIDE_SELECTION_PERIOD) %>
	</p>
	<div class="productList">
		<div class="background">
			<asp:Repeater ID="rSubscriptionBoxDeliveryTiming" runat="server">
				<ItemTemplate>
					<% if (this.SubscriptionBox.IsNumberTime) { %>
						<h3><%# (string)Container.DataItem %>回目配送商品</h3>
					<% } else { %>
						<h3><%# (string)Container.DataItem %>にお届けする商品</h3>
					<% }%>
					<div class="list" style="background-color: #f1f1f1;">
						<p class="ttl">
							<table>
								<tr>
									<td style="width: 580px; text-align: left;">
										<div style="margin-left: 70px">
											商品名
										</div>
									</td>
									<td style="width: 110px; text-align: left;">
										<div style="margin-left: 10px" Visible="<%# IsFixedAmount == false %>" runat="server">
											単価
										</div>
									</td>
									<td style="width: 113px; text-align: center">
										<div>
											注文数
										</div>
									</td>
								</tr>
							</table>
						</p>
					</div>
					<asp:HiddenField ID="hfDeliveryTiming" Value="<%# (string)Container.DataItem %>" runat="server" />
					<div class="product" style="background-color: white;" runat="server">
							<asp:Repeater ID="rProductsList" Visible="<%# (CheckTakeOverProduct(GetItemsModel((string)Container.DataItem))) == false %>" ItemType="w2.Domain.SubscriptionBox.SubscriptionBoxDefaultItemModel" DataSource='<%# GetItemsModel((string)Container.DataItem) %>' runat="server">
								<ItemTemplate>
									<table>
										<tr>
											<td class="name" style="width: 570px;">
												<w2c:ProductImage ImageSize="S" IsVariation='<%# Item.ProductId != Item.VariationId %>' ProductMaster="<%# GetProduct(Item.ShopId, Item.ProductId, Item.VariationId) %>" runat="server" Visible="<%# string.IsNullOrEmpty(Item.VariationId) == false %>" />
												<a href='<%#: GetProductDetailUrl(Item.ProductId, (SubscriptionBoxDefaultItemModel)Container.DataItem) %>'>
													<%#: CreateProductName(Item.ProductId, Item.Name, Item.VariationName1, Item.VariationName2, Item.VariationName3) %>
												</a>
											</td>
											<td class="price" style="width: 110px;">
												<% if (IsFixedAmount == false) { %>
												<div style="text-decoration: line-through" runat="server" Visible="<%# IsSubscriptionBoxCampaignPeriodByProductIdAndVariationId(Item.ProductId, Item.VariationId) %>">
													<%#: GetValidPrice(Item.ProductId, Item.VariationId, Item.Price, true) %>
												</div>
												<div>
													<%#: GetValidPrice(Item.ProductId, Item.VariationId, Item.Price) %>
												</div>
												<% } %>
											</td>
											<td class="quantity" style="padding-top: 0; padding-left: 15px;">
												<%#: Item.ItemQuantity %>
											</td>
											<td class="orderDelete"></td>
										</tr>
									</table>
								</ItemTemplate>
							</asp:Repeater>
							<div class="product" style="background-color: white;" visible="<%# CheckTakeOverProduct(GetItemsModel((string)Container.DataItem)) %>" runat="server">
								選択された商品を配送します。<br />
							</div>
							<asp:Repeater ID="rSubscriptionBoxDuringItemList" Visible="<%# CheckDuringTakeOverProduct((string)Container.DataItem) %>" ItemType="w2.Domain.SubscriptionBox.SubscriptionBoxDefaultItemModel" DataSource="<%# this.SubscriptionBoxDuringItemList %>" OnItemCommand="rProductChange_ItemCommand" runat="server">
								<ItemTemplate>
									<table>
										<tr>
											<td class="name" height="70" width="730">
												<w2c:ProductImage ImageSize="S" IsVariation='<%# Item.ProductId != Item.VariationId %>' ProductMaster="<%# GetProduct(Item.ShopId, Item.ProductId, Item.VariationId) %>" runat="server" Visible="<%# (string.IsNullOrEmpty(Item.VariationId) == false) %>" />
												<asp:HiddenField ID="hfTakeOverProductBranchNo" Value="<%# Item.TakeOverProductBranchNo %>" runat="server" />
												<asp:DropDownList ID="ddlTakeOverProductsList" DataSource="<%# GetDuringItemsList() %>" OnSelectedIndexChanged="ddlValidDefaultItemList_OnSelectedIndexChanged" SelectedValue="<%# Item.VariationId %>" DataTextField="Text" DataValueField="Value" AutoPostBack="true" runat="server" />
											</td>
											<td class="price" width="110">
												<div style="text-decoration: line-through" runat="server" Visible="<%# this.IsFixedAmount %>">
													<asp:Literal Text="<%#: GetValidPrice(Item.ProductId, Item.VariationId, Item.Price) %>" runat="server" /><br>
												</div>
												<div runat="server" Visible="<%# this.IsFixedAmount == false %>">
													<asp:Literal Text="<%#: GetValidPrice(Item.ProductId, Item.VariationId, Item.Price) %>" runat="server" />
												</div>
											</td>
											<td class="quantity" width="120">
												<asp:TextBox ID="tbQuantity" Text="<%#: Item.ItemQuantity %>" MaxLength="3" Width="28px" Visible="<%# (string.IsNullOrEmpty(Item.VariationId) == false) %>" runat="server" />
											</td>
											<td class="orderDelete">
												<asp:LinkButton Text="削除" runat="server" CommandName="DeleteRow" />
											</td>
										</tr>
									</table>
								</ItemTemplate>
								<FooterTemplate>
									<asp:Button ID="addDefaultProduct" runat="server" Text="＋" OnClick="btnAddProduct_Click" AutoPostBack="true" />
								</FooterTemplate>
							</asp:Repeater>
						</div>
					<div class="SubscriptionOptionalItem" id="dvListProduct" runat="server" Visible="<%# IsDispSelectProductList(Container) %>" style="width: 700px; margin: auto;">
						<table cellspacing="0">
							<div id="SubscriptionOptionalItem" runat="server" Visible="<%# CanNecessaryProducts(this.SubscriptionBoxCourseId) %>">
								<tr>
									<td class="title_bg" colspan="6">
										<div style="display: inline-block;" runat="server" Visible="<%# this.SubscriputionBoxProductListModify.Any() %>">
											<p style="display: inline-block;">頒布会選択商品</p>
										</div>
										<div class="right" style="text-align: right; display: inline-block;">
											<asp:Button ID="btnChangeProduct" Text="選択する" runat="server" OnClick="btnChangeProduct_Click" class="btn" />
										</div>
										<div class="dvSubscriptionOptional" runat="server" Visible="<%# this.SubscriputionBoxProductListModify.Any() == false %>">
											<p class="subscriptionMessage">頒布会商品を選択可能です</p>
										</div>
									</td>
								</tr>
							</div>
							<asp:Repeater ID="rItem" Visible="<%# this.SubscriputionBoxProductListModify.Any() %>" DataSource="<%# this.SubscriputionBoxProductListModify %>" ItemType="w2.Domain.SubscriptionBox.SubscriptionBoxDefaultItemModel" runat="server">
								<HeaderTemplate>
									<tr class="rtitle">
										<th class="productName" colspan="2">商品名
										</th>
										<th class="productPrice">単価（<%#: this.ProductPriceTextPrefix %>）
										</th>
										<th class="orderCount">注文数
										</th>
										<th class="orderSubtotal">小計（<%#: this.ProductPriceTextPrefix %>）
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
										</td>
										<td class="productPrice">
											<%#: CurrencyManager.ToPrice(SubscriptionBoxPrice(Item.ProductId, Item.VariationId, 1)) %>
										</td>
										<td class="orderCount">
											<%#: StringUtility.ToNumeric(Item.ItemQuantity)%>
										</td>
										<td class="orderSubtotal">
											<%#: CurrencyManager.ToPrice(SubscriptionBoxPrice(Item.ProductId, Item.VariationId, Item.ItemQuantity)) %>
										</td>
									</tr>
								</ItemTemplate>
							</asp:Repeater>
						</table>
					</div>
					<div runat="server" Visible="<%# ((IDataItemContainer)Container).DisplayIndex == 0 %>">
						<div class="SubscriptionOptionalItem" id="dvModifySubscription" runat="server" Visible="<%# this.HasOptionalProdects == false %>" style="width: 700px; margin: auto;">
							<div>
								<small id="sErrorQuantity" class="error" runat="server"></small> 
							</div>
							<table cellspacing="0">
								<tr>
									<td class="title_bg" colspan="6">
										<p style="display: inline-block;">頒布会任意商品の選択</p>
										<div class="right" style="text-align: right;">
											<asp:Button Text="  商品追加  " runat="server" class="btn" Style="margin-right: 10px; display: inline-block;" OnClick="btnAddAnyProduct_Click" CommandArgument="<%# this.SubscriptionBoxCourseId %>" />
											<asp:Button Text="  決定  " runat="server" class="btn" Style="display: inline-block;" OnClick="btnUpdateProduct_Click" CommandArgument="<%# this.SubscriptionBoxCourseId %>" />
										</div>
									</td>
								</tr>
								<asp:Repeater ID="rItemModify" DataSource="<%# this.SubscriputionBoxProductList %>" ItemType="w2.Domain.SubscriptionBox.SubscriptionBoxDefaultItemModel" runat="server" OnItemCommand="rAnyProductChange_ItemCommand">
									<HeaderTemplate>
										<tr class="rtitle">
											<th class="productName" colspan="2">商品名
											</th>
											<th class="productPrice">単価（<%#: this.ProductPriceTextPrefix %>）
											</th>
											<th class="orderCount">注文数
											</th>
											<th class="orderSubtotal">小計（<%#: this.ProductPriceTextPrefix %>）
											</th>
											<th>削除
											</th>
										</tr>
									</HeaderTemplate>
									<ItemTemplate>
										<tr>
											<td class="productImage">
												<w2c:ProductImage ImageSize="S" IsVariation="<%# Item.ProductId != Item.VariationId %>" ProductMaster="<%# GetProduct(Item.ShopId, Item.ProductId, Item.VariationId) %>" runat="server" Visible="True" />
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
													Width="90%"
													runat="server" />
											</td>
											<td class="productPrice">
												<%#: CurrencyManager.ToPrice(SubscriptionBoxPrice(Item.ProductId, Item.VariationId, 1)) %>
											</td>
											<td class="orderCount">
												<asp:TextBox ID="tbQuantityUpdate" runat="server" Text="<%# StringUtility.ToNumeric(Item.ItemQuantity) %>" OnTextChanged="ReCalculation" AutoPostBack="True" MaxLength="3" />
											</td>
											<td class="orderSubtotal">
												<%#: CurrencyManager.ToPrice(SubscriptionBoxPrice(Item.ProductId, Item.VariationId, Item.ItemQuantity)) %>
											</td>
											<td class="orderDelete">
												<asp:LinkButton Text="x" runat="server" CommandName="DeleteRow" CommandArgument="<%# Container.ItemIndex %>" />
											</td>
										</tr>
									</ItemTemplate>
								</asp:Repeater>
							</table>
						</div>
					</div>
					<!--productList-->
				</ItemTemplate>
			</asp:Repeater>
		</div>
	</div>
</div>

<div class="addCart">
	<p class="btnCart">
		<asp:LinkButton ID="lbCartAddSubscriptionBox" class="btn btn-mid btn-inverse" Visible="<%# this.IsWithinSelectionPeriodFirstProduct %>" runat="server" OnClick="lbCartAddSubscriptionBox_Click">
			頒布会申し込み
		</asp:LinkButton>
	</p>
</div>
</asp:Content>
