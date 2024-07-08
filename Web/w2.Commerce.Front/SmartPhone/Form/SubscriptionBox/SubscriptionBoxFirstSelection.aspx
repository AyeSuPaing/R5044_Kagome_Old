<%--
=========================================================================================================
  Module     頒布会初回選択画面 : (SubscriptionBoxFirstSelection.aspx)
 ････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Form/SubscriptionBox/SubscriptionBoxFirstSelection.aspx.cs" Inherits="Form_SubscriptionBox_SubscriptionBoxFirstSelection" Title="頒布会初回選択画面" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
	<style>
		#Header, #Foot {
			display: none;
		}

		#Wrap, .wrapBottom, .wrapTop, #Contents .mainContents {
			width: 100%;
			height: 100vh;
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
			width: calc(50% - 20px);
			margin: 10px 10px;
		}

		div ul li {
			border: none;
		}

		.container li.icon img {
			height: 3	0px;
			border: none;
		}

		.count {
			display: flex;
			flex-wrap: nowrap;
			align-items: center;
			justify-content: flex-end;
			width: 180px;
			border: 1px solid #EBE9E5;
			background: #FFFFFF;
		}

		.count span, .count a{
			display: flex;
			flex-wrap: nowrap;
			align-items: center;
			justify-content: center;
			height: 60px;
			width: 60px;
			font-weight: 500;
			font-size: 2rem;
			position: relative;
		}

		.subImg li {
			display: inline-block;
		}

		.display-block {
			display: block;
		}

		.display-none {
			display: none;
		}

		.selected {
			position: fixed;
			bottom: 0;
			left: 0;
			width: 100%;
			height: 16rem;
			background-color: #fff;
		}

		.warning {
			font-size: 1.5rem;
		}

		.nowStatus {
			height: 12rem;
			width : 100%;
			position: absolute;
			bottom: 0;
			left: 0;
			background: #E1E1E1;
			color: #333333;
			z-index: 100;
		}
		
		.addCart {
			height: 12rem;
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
			line-height: 12rem;
			color: #fff;
			font-size: 3rem;
			text-decoration: none;
		}

		.addcartbutton {
			position: absolute;
			bottom: 0;
			width: 95%;
			text-align: center;
			height: 70px;
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
			font-size: 1.5rem;
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
		<div style="width: 100%">
			<h1 style="font-size:4rem; margin:50px;">頒布会初回選択</h1>
			<asp:UpdatePanel runat="server" ID="upLeftCount">
				<ContentTemplate>
					<div class="leftPage">
						<asp:Repeater DataSource="<%# this.ProductMasterList %>" runat="server" ID="rLeft" ClientIDMode="Predictable" OnItemCommand="rLeft_ItemCommnad">
							<ItemTemplate>
								<div class="product">
									<div class="container products">
										<ul style="text-align: center;">
											<li>
												<img style="max-width: 100%;" src="<%# WebSanitizer.HtmlEncode(CreateProductSubImageUrl(Container.DataItem, Constants.PRODUCTIMAGE_FOOTER_L, Constants.PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO + 1)) %>" />
											</li>
											<li class="icon" style="width: 80%; margin: 0 auto 20px; display: flex; flex-wrap: wrap;">
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
												<p style="font-size: 2rem; text-indent: 20px;"><%#: StringUtility.ToEmpty(ProductPage.GetKeyValueToNull(Container.DataItem, Constants.FIELD_PRODUCT_NAME)) %></p>
												<p style="font-size: 1rem; text-indent: 20px;"><%#: StringUtility.ToEmpty(ProductPage.GetKeyValueToNull(Container.DataItem, Constants.FIELD_PRODUCT_NAME_KANA)) %></p>
											</li>
											<li style="width: calc(100% - 40px); margin: 0 auto; display: flex; justify-content: space-between;">
												<p style="font-size: 2rem;"><%# GetValidPrice(Container.DataItem) %></p>
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
		<asp:UpdatePanel runat="server">
			<ContentTemplate>
				<div class="selected">
					<% if (this.MaxBoxCount > 0) { %>
					<div class="maxCount">
						<p class="warning" style="padding: 0px 5px;">
							※１回の購入で<asp:Label runat="server" ID="lMaxCount" Text="<%# WebSanitizer.HtmlEncode(this.MaxBoxCount) %>" />点までご購入いただけます
						</p>
					</div>
					<%} %>
					<% if (this.MaxAmount > 0) { %>
					<div>
						<p class="warning" style="padding: 0px 5px;">
							※１回の購入で<%# CurrencyManager.ToPrice(this.MaxAmount) %>までご購入いただけます
						</p>
					</div>
					<%} %>
					<div id="dAddCartButton" runat="server" Visible="<%# this.CanAddCart %>" class="addCart">
						<asp:LinkButton runat="server" OnClick="lbAddCartButton_Click" Text="カートに入れる" />
					</div>
					<div class="nowStatus">
						<div style="font-size: 3rem; margin: 15px 0; text-align: center;">
							現在の購入金額：<asp:Label runat="server" ID="lNowPrice" Text="<%# CurrencyManager.ToPrice(GetTotalAmount()) %>" /><br />
						</div>
						<div class="messages" style="margin-bottom: 5px;" >
							<% if (CalculateRemainingMinAmount() > 0) { %>
							<p>
								※あと<asp:Label runat="server" ID="lUnderMinAmount" Text="<%# CurrencyManager.ToPrice(CalculateRemainingMinAmount()) %>" />分追加してください
							</p>
							<% } %>
							<% if (CalculateRemainingQuantity() > 0) { %>
							<p>
								※あと<asp:Label runat="server" ID="lDiffCount" Text="<%# CalculateRemainingQuantity() %>" />点選んでください
							</p>
							<% } %>
							<% if (CalculateRemainingMinNumberOfProducts() > 0) { %>
							<p>
								※あと<asp:Label runat="server" ID="lUnderMinNumberOfProducts" Text="<%# CalculateRemainingMinNumberOfProducts() %>" />種類選んでください
							</p>
							<% } %>
							<% if (CalculateRemainingMaxAmount() > 0) { %>
							<p>
								※購入金額が<asp:Label runat="server" ID="lOrverMaxAmount" />超過しています。
							</p>
							<% } %>
							<% if ((this.MaxBoxCount > 0) && (GetTotalItemQuantity() > this.MaxBoxCount)) { %>
							<p>
								※数量が<asp:Label runat="server" ID="lOverMaxQuantity" />点超過しています。
							</p>
							<% } %>
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
</asp:Content>
