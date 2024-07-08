<%--
=========================================================================================================
  Module     頒布会初回選択画面 : (SubscriptionBoxFirstSelection.aspx)
 ････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Form/SubscriptionBox/SubscriptionBoxFirstSelection.aspx.cs" Inherits="Form_SubscriptionBox_SubscriptionBoxFirstSelection" Title="頒布会初回選択画面" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
	<link  href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "Css/subsc_list.css") %>" rel="stylesheet" type="text/css" />
	<style>
		#Header, #Foot {
			display: none;
		}

		#Wrap, .wrapBottom, .wrapTop, #Contents .mainContents {
			width: 100%;
		}

		.mainContents {
			display: flex;
		}

		.leftPage {
			display: flex;
			flex-wrap: wrap;
			align-items: flex-end;
			flex-direction: row;
			align-content: space-around;
		}

		.product {
			width: calc(25% - 20px);
			margin: 10px 10px;
		}

		.container li.icon img {
			height: 20px;
			margin-bottom: 5px;
			border: none;
		}

		.container ul li.thumb img {
			max-width: 90%;
			margin-bottom: 10px;
		}

		.count {
			display: flex;
			flex-wrap: nowrap;
			align-items: stretch;
			justify-content: flex-end;
			width: 90px;
			border: 1px solid #EBE9E5;
			background: #FFFFFF;
		}

		.count span, .count a{
			display: flex;
			flex-wrap: nowrap;
			align-items: center;
			justify-content: center;
			height: 30px;
			width: 30px;
			color: #4C3820;
			font-weight: 500;
			font-size: 1rem;
			position: relative;
		}

		.rightPage {
			width: 25%;
			margin: 20px 5% 0 0;
			background-color: white;
			position: relative;
			height: 90vh;
		}

		.subImg {
			display: flex;
			align-items: center;
		}
		.subImg li {
			margin: 10px 0 10px 10px;
		}

		.display-block {
			display: block;
		}

		.display-none {
			display: none;
		}

		.selected {
			position: absolute;
			bottom: 0;
			width: 100%;
			height: 150px;
		}

		.nowStatus {
			height: 100px;
			width : 100%;
			position: absolute;
			bottom: 0;
			left: 0;
			background-color: #E1E1E1;
			color: #333333;
			z-index: 100;
		}
		
		.addCart {
			height: 100px;
			width : 100%;
			position: absolute;
			bottom: 0;
			left: 0;
			z-index: 200;
		}

		.addCart a {
			height: 100%;
			width : 100%;
			display: block;
			background: #000000;
			text-align: center;
			line-height: 100px;
			color: #fff;
			font-size: 1.5rem;
			text-decoration: none;
		}

		.addcartbutton {
			position: absolute;
			bottom: 0;
			width: 95%;
			text-align: center;
			height: 70px;
			background-color: #ffcc00;
			margin-left: 2.5%;
		}

		.selectedprice {
			height: 70px;
			text-align: center;
			background-color: #00bfff;
			margin: 5px
		}

		.messages p {
			text-align: center;
			margin-bottom: 2px;
			color: #ff0000;
		}
	</style>
	<%-- ▽編集可能領域：HEAD追加部分▽ --%>
	<link href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT) %>Css/product.css" rel="stylesheet" type="text/css" media="all" />
	<%= this.BrandAdditionalDsignTag %>
	<%-- △編集可能領域△ --%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<div class="listProduct">
		<%-- カート投入ボタン押下時にどの画面へ遷移するか？ --%>
		<%-- CART：カート一覧画面 その他：画面遷移しない --%>
		<asp:HiddenField ID="hfIsRedirectAfterAddProduct" Value="CART" runat="server" />
	</div>
	<div class="mainContents">
		<div style="width: 80%">
			<h1 style="font-size:2rem; margin: 20px 0; text-align: center;">頒布会初回選択</h1>
			<asp:UpdatePanel runat="server" ID="upLeftCount">
				<ContentTemplate>
					<div class="leftPage">
						<asp:Repeater DataSource="<%# this.ProductMasterList %>" runat="server" ID="rLeft" ClientIDMode="Predictable" OnItemCommand="rLeft_ItemCommnad">
							<ItemTemplate>
								<div class="product">
									<div class="container products">
										<ul style="text-align: center;">
											<li class="thumb">
												<img src="<%# CreateProductSubImageUrl(Container.DataItem, Constants.PRODUCTIMAGE_FOOTER_L, Constants.PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO + 1) %>" />
											</li>
											<li class="icon" style="margin: 0 auto 20px;">
												<w2c:ProductIcon IconNo="1" ProductMaster="<%# Container.DataItem %>" runat="server" />
												<w2c:ProductIcon IconNo="2" ProductMaster="<%# Container.DataItem %>" runat="server" />
												<w2c:ProductIcon IconNo="3" ProductMaster="<%# Container.DataItem %>" runat="server" />
												<w2c:ProductIcon IconNo="4" ProductMaster="<%# Container.DataItem %>" runat="server" />
												<w2c:ProductIcon IconNo="5" ProductMaster="<%# Container.DataItem %>" runat="server" />
												<w2c:ProductIcon IconNo="6" ProductMaster="<%# Container.DataItem %>" runat="server" />
												<w2c:ProductIcon IconNo="7" ProductMaster="<%# Container.DataItem %>" runat="server" />
												<w2c:ProductIcon IconNo="8" ProductMaster="<%# Container.DataItem %>" runat="server" />
												<w2c:ProductIcon IconNo="9" ProductMaster="<%# Container.DataItem %>" runat="server" />
												<w2c:ProductIcon IconNo="10" ProductMaster="<%# Container.DataItem %>" runat="server" />
											</li>
											<li style="margin-bottom: 20px;">
												<p style="font-size: 1rem; text-indent: 20px;"><%#: StringUtility.ToEmpty(ProductPage.GetKeyValueToNull(Container.DataItem, Constants.FIELD_PRODUCT_NAME)) %></p>
												<p style="text-indent: 20px;"><%#: StringUtility.ToEmpty(ProductPage.GetKeyValueToNull(Container.DataItem, Constants.FIELD_PRODUCT_NAME_KANA)) %></p>
											</li>
											<li style="width: calc(100% - 40px); margin: 0 auto; display: flex; justify-content: space-between;">
												<p style="font-size: 1rem;"><%# GetValidPrice(Container.DataItem) %></p>
												<div class="count">
													<asp:LinkButton runat="server" Text="-" ID="lbLeftMinus" CommandName="Minus" CommandArgument="<%# GetKeyValue(((RepeaterItem)Container).DataItem, Constants.FIELD_PRODUCT_PRODUCT_ID) %>" />
													<span><asp:Label runat="server" ID="lLeftCount" Text='<%# (IsNecessary(((RepeaterItem)Container).DataItem) ? (int)(GetKeyValue(((RepeaterItem)Container).DataItem, Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_ITEM_QUANTITY)) : Eval("item_quantity") )%>' /></span>
													<asp:LinkButton runat="server" Text="+" ID="lbLeftPlus" CommandName="Plus" CommandArgument="<%# GetKeyValue(((RepeaterItem)Container).DataItem, Constants.FIELD_PRODUCT_PRODUCT_ID) %>" />
												</div>
											</li>
										</ul>
									</div>
								</div>
							</ItemTemplate>
						</asp:Repeater>
					</div>
				</ContentTemplate>
			</asp:UpdatePanel>
		</div>
		<div class="rightPage">
			<div style="height: 70vh; overflow-y: auto;">
				<h2 style="background-color: #999999; height: 3rem; font-size: 1.2rem; display: flex; justify-content: center; align-items: center; color: #fff;">
					選択中の商品
				</h2>
				<asp:Repeater runat="server" ID="rRight" DataSource="<%# this.ProductMasterList %>" ClientIDMode="Predictable" OnItemCommand="rRight_ItemCommnad">
					<ItemTemplate>
						<asp:UpdatePanel runat="server">
							<ContentTemplate>
								<div runat="server" id="dRight">
									<div class="continer products" runat="server" Visible="<%# IsDisplay(((RepeaterItem)Container).DataItem) %>">
										<ul class="subImg">
											<li class="thumb" style="width: 20%;">
												<img style="max-width: 100%;" src="<%# WebSanitizer.HtmlEncode(CreateProductSubImageUrl(((RepeaterItem)Container).DataItem, Constants.PRODUCTIMAGE_FOOTER_M, Constants.PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO + 1)) %>" />
											</li>
											<li>
												<p><%#: StringUtility.ToEmpty(ProductPage.GetKeyValueToNull(((RepeaterItem)Container).DataItem, Constants.FIELD_PRODUCT_NAME)) %></p>
												<p><%#: StringUtility.ToEmpty(ProductPage.GetKeyValueToNull(((RepeaterItem)Container).DataItem, Constants.FIELD_PRODUCT_NAME_KANA)) %></p>
												<div style="display: flex; align-items: center; margin-top: 5px;">
													<asp:Label runat="server" ID="lPrice" Text="<%# GetValidPrice(((RepeaterItem)Container).DataItem) %>" />
													<div class="count" style="margin-left: 25px;">
														<asp:LinkButton runat="server" Text="-" ID="lbRightMinus" CommandName="Minus" CommandArgument="<%# GetKeyValue(((RepeaterItem)Container).DataItem, Constants.FIELD_PRODUCT_PRODUCT_ID) %>" />
														<asp:Label runat="server" ID="lRightCount" Text='<%# (IsNecessary(((RepeaterItem)Container).DataItem) ? (int)(GetKeyValue(((RepeaterItem)Container).DataItem, Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_ITEM_QUANTITY)) : Eval("item_quantity") )%>' />
														<asp:LinkButton runat="server" Text="+" ID="lbRightPlus" CommandName="Plus" CommandArgument="<%# GetKeyValue(((RepeaterItem)Container).DataItem, Constants.FIELD_PRODUCT_PRODUCT_ID) %>" />
													</div>
													<asp:LinkButton runat="server" Text="削除" ID="lbRightDelete" Visible="<%# IsNecessary(((RepeaterItem)Container).DataItem) == false %>"  CommandName="Delete" CommandArgument="<%# GetKeyValue(((RepeaterItem)Container).DataItem, Constants.FIELD_PRODUCT_PRODUCT_ID) %>" style="margin-left: 10px;" />
													<p style="margin-left: 10px;" visible="<%# IsNecessary(((RepeaterItem)Container).DataItem) %>" runat="server">必須商品</p>
												</div>
											</li>
										</ul>
									</div>
								</div>
							</ContentTemplate>
						</asp:UpdatePanel>
					</ItemTemplate>
				</asp:Repeater>
			</div>
			<asp:UpdatePanel runat="server">
				<ContentTemplate>
					<div class="selected">
						<% if (this.MaxBoxCount > 0) { %>
						<div class="maxCount">
							<p style="padding: 0px 5px; font-size:1rem;">
								※１回の購入で<%# WebSanitizer.HtmlEncode(this.MaxBoxCount) %>点までご購入いただけます
							</p>
						</div>
						<%} %>
						<% if (this.MaxAmount > 0) { %>
						<div>
							<p style="padding: 0px 5px; font-size:1rem;">
								※１回の購入で<%# CurrencyManager.ToPrice(this.MaxAmount) %>までご購入いただけます
							</p>
						</div>
						<%} %>
						<div id="dAddCartButton" runat="server" Visible="<%# this.CanAddCart %>" class="addCart">
							<asp:LinkButton runat="server" OnClick="lbAddCartButton_Click" Text="カートに入れる" />
						</div>
						<div class="nowStatus">
							<div style="font-size: 1.2rem; margin: 15px 0 5px; text-align: center;">
								現在の購入金額：<asp:Label runat="server" ID="lNowPrice" Text="<%# CurrencyManager.ToPrice(GetTotalAmount()) %>" /><br />
							</div>
							<div class="messages" style="margin-bottom: 5px;" >
								<%--購入金額が商品合計金額下限（税込）未満の場合--%>
								<% if (CalculateRemainingMinAmount() > 0) { %>
								<p>
									※あと<asp:Label runat="server" ID="lUnderMinAmount" Text="<%# CurrencyManager.ToPrice(CalculateRemainingMinAmount()) %>" />分追加してください
								</p>
								<% } %>
								<%--合計商品選択数が最低購入数量未満の場合--%>
								<% if (CalculateRemainingQuantity() > 0) { %>
								<p>
									※あと<asp:Label runat="server" ID="lDiffCount" Text="<%# CalculateRemainingQuantity() %>" />点選んでください
								</p>
								<% } %>
								<%--合計商品種類数が最低購入種類未満の場合--%>
								<% if (CalculateRemainingMinNumberOfProducts() > 0) { %>
								<p>
									※あと<asp:Label runat="server" ID="lUnderMinNumberOfProducts" Text="<%# CalculateRemainingMinNumberOfProducts() %>" />種類選んでください
								</p>
								<% } %>
								<%--合計商品金額が商品合計金額上限（税込）を超過する場合--%>
								<% if (CalculateRemainingMaxAmount() > 0) { %>
								<p>
									※購入金額が<asp:Label runat="server" ID="lOrverMaxAmount" />超過しています。
								</p>
								<% } %>
								<%--合計商品選択数が最大購入数量を超過する場合--%>
								<% if ((this.MaxBoxCount > 0) && (GetTotalItemQuantity() > this.MaxBoxCount)) { %>
								<p>
									※数量が<asp:Label runat="server" ID="lOverMaxQuantity" />点超過しています。
								</p>
								<% } %>
								<%--合計商品種類数が最大購入種類を超過する場合--%>
								<% if (CalculateRemainingMaxNumberOfProducts() > 0) { %>
								<p>
									※購入種類が<asp:Label runat="server" ID="lOrverMaxNumberOfProducts" />種類超過しています。
								</p>
								<% } %>
							</div>
						</div>
					</div>
				</ContentTemplate>
			</asp:UpdatePanel>
		</div>
	</div>
</asp:Content>
