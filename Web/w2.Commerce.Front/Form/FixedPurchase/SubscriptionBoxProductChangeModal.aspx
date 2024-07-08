<%--
=========================================================================================================
  Module      : 頒布会商品変更モーダル (SubscriptionBoxProductChangeModal.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SubscriptionBoxProductChangeModal.aspx.cs" Inherits="Form_FixedPurchase_SubscriptionBoxProductChangeModal" Title="商品変更ページ" %>
<%@ Import Namespace="w2.App.Common.Util" %>
<%@ Register TagPrefix="uc" TagName="HeaderScriptDeclaration" Src="~/Form/Common/HeaderScriptDeclaration.ascx" %>

<%-- このページはモーダルで表示されるため、マスターページを使用しません。 --%>

<html lang="ja">
<head runat="server">
	<title>タイトルはページレンダリング時に置き換えられます</title>
	<meta charset="UTF-8" />
	<uc:HeaderScriptDeclaration id="HeaderScriptDeclaration" runat="server" />
	<%
		SetCssLink("lCommonCss", Constants.PATH_ROOT + "Css/common.css?20180216");
		SetCssLink("lPrintCss", Constants.PATH_ROOT + "Css/imports/print.css");
	%>
	<link id="lCommonCss" rel="stylesheet" type="text/css" media="screen,print" />
	<link id="lPrintCss"  rel="stylesheet" type="text/css"  media="print" />
	<style>
		.main-container {
			box-sizing: border-box;
			height: 100%;
			padding: 5px;
			position: relative;
		}

		h4 {
			font-size: 1.3em;
			line-height: 1.3em;
			margin: 10px 0 10px 0;
		}

		.product-list {
			padding: 15px;
			position: relative;
			overflow-y: scroll;
			height: calc(100% - 192px);
			margin: 0 0 5px 0;
			box-sizing: border-box;
			display: flex;
			align-items: flex-start;
			flex-direction: row;
			flex-wrap: wrap;
		}

		.product-item {
			position: relative;
			width: 260px;
			padding: 10px;
			margin: 0 15px 15px 0;
			box-shadow: 0 8px 10px rgba(0, 0, 0, 0.5);
			border-radius: 10px;
		}

		@media screen and (max-width: 650px) {
			.product-item {
				width: calc(50% - 37px);
			}
		}
		
		.display-none {
			display: none;
		}

		.product-item-selected {
			background-color: #87d3ff;
		}

		.product-item-selected-warn {
			background-color: #b22222;
			color: #ffffff;
		}

		.product-item-unavailable-overlay {
			position: absolute;
			top: 0;
			left: 0;
			width: 100%;
			height: 100%;
			background-color: rgba(0, 0, 0, 0.3);
			border-radius: 10px;
		}

		.product-unselectable-text {
			position: absolute;
			display: inline;
			width: 100%;
			height: 100%;
			font-size: 1.3em;
			text-align: center;
			vertical-align: middle;
		}

		.product-item-image {
			text-align: center;
			margin: 5px 0 20px 0;
		}

		.product-item-image img {
			width: 95%;
		}

		.product-name, .product-price {
			width: 100%;
			display: block;
			word-wrap: break-word;
		}

		.product-price {
			text-align: right;
		}

		.product-item-quantity-container {
			text-align: right;
		}

		.product-quantity-input {
			width: 40px;
			height: 40px;
			font-size: 1.3em;
			text-align: center;
		}

		.campaign-term {
			white-space: nowrap;
		}

		.error-message {
			height: 1em;
			color: #b22222;
		}

		.subscription-modal-content {
			position: relative;
			height: 100%;
		}

		.action-button-container {
			position: absolute;
			bottom: 0;
			width: 100%;
		}

		.action-button {
			display: inline-block;
			width: 100%;
			height: 50px;
			line-height: 50px;
			margin: 0 0 5px 0;
			text-align: center;
			vertical-align: middle;
		}

		.action-button-primary {
			background-color: #000;
			color: #FFF;
		}

		.action-button-secondary {
			background-color: #DDD;
			color: #000;
		}
	</style>
	<script>
		window.app = {
			initialize: () => {
				if (Sys.WebForms !== null) {
					const reqMgr = Sys.WebForms.PageRequestManager.getInstance();
					reqMgr.add_initializeRequest(function (sender, args) {
						window.sessionStorage.setItem(
							'scroll_pos',
							document.getElementById('product-list').scrollTop);
					});

					reqMgr.add_endRequest(function (sender, args) {
						document.getElementById('product-list').scrollTop = window.sessionStorage.getItem('scroll_pos');
					});
				}
			},

			closeModal: () => {
				if (window.parent.app !== 'undefined') {
					window.parent.app.closeModal();
				}
			},

			onSubmit: () => {
				const error = document.getElementsByClassName("error-message")[0].innerHTML.trim().replaceAll('<br>', '\n');
				if (error !== '') {
					alert(error);
					return false;
				}

				return window.confirm('次回お届けする商品を表示内容で更新します。よろしいですか？');
			}
		}

		window.addEventListener('load', () => window.app.initialize());
	</script>
</head>
<body>
	<form id="form1" runat="server">
		<%-- スクリプトマネージャ --%>
		<asp:ScriptManager ID="smScriptManager" runat="server" ScriptMode="Release" />
		<div class="main-container">
			<asp:UpdatePanel ID="upProductList" class="subscription-modal-content" runat="server">
				<ContentTemplate>
					<h4>次回お届けする商品を選択してください。</h4>
					<div id="product-list" class="product-list">
						<asp:Repeater
							ID="rItems"
							DataSource="<%# this.Input.Items %>"
							ItemType="Input.FixedPurchase.SubscriptionBoxNextShippingProductInputItem"
							runat="server">
							<ItemTemplate>
								<asp:CheckBox
									ID="cbSelected"
									Enabled="<%# Item.IsNecessaryItem == false %>"
									CssClass="display-none"
									Checked="<%# Item.IsSelected %>"
									AutoPostBack="True" runat="server" />
								<label
									for="<%# Container.FindControl("cbSelected").ClientID %>"
									class="product-item <%#: Item.IsSelected ? "product-item-selected" : "" %> <%#: Item.IsSelected && (Item.IsSelectable == false) ? "product-item-selected-warn" : "" %>">
									<div class="product-item-image">
										<w2c:ProductImage ProductMaster="<%# Item %>" IsVariation="<%# Item.IsVariation %>" ImageSize="M" runat="server" />
									</div>
									<div class="product-info-container">
										<div class="product-item-subinfo-container">
											<span class="product-name"><%#: Item.ProductName %></span>
											<% if (this.Input.IsFixedAmount == false) { %>
											<span class="product-price"><%#: CurrencyManager.ToPrice(Item.FixedPurchasePrice) %></span>
											<% } %>
											<p style="margin-top: 8px;">
												<asp:Literal Visible="<%# Item.IsNecessaryItem %>" Text="必須商品です。" runat="server" />
												<div Visible="<%# Item.IsAppliedCampaignPrice %>" runat="server">
													キャンペーン期間<br />
													<span class="campaign-term"><%#: DateTimeToString(Item.CampaignSince) %></span> ～ <span class="campaign-term"><%#: DateTimeToString(Item.CampaignUntil) %></span>
												</div>
											</p>
										</div>
										<div class="product-item-quantity-container">
											✕
											<asp:TextBox
												ID="tbQuantity"
												Text="<%#: Item.Quantity %>"
												CssClass="product-quantity-input"
												AutoPostBack="true"
												TextMode="Number"
												Enabled="<%# Item.IsSelected %>"
												runat="server" />
										</div>
									</div>
									<div class="product-item-unavailable-overlay" Visible="<%# (Item.IsSelectable == false) %>" runat="server">
									</div>
								</label>
								<asp:HiddenField ID="hfShopId" Value="<%#: Item.ShopId %>" runat="server" />
								<asp:HiddenField ID="hfProductId" Value="<%#: Item.ProductId %>" runat="server" />
								<asp:HiddenField ID="hfVariationId" Value="<%#: Item.VariationId %>" runat="server" />
							</ItemTemplate>
						</asp:Repeater>
					</div>
					<span class="error-message">
						<asp:Literal ID="lErrors" runat="server" />
					</span>
					<div class="action-button-container">
						<asp:LinkButton ID="lbSubmitChanges" OnClientClick="return window.app.onSubmit();" OnClick="lbSubmitChanges_OnClick" CssClass="action-button action-button-primary" runat="server">
							変更する
						</asp:LinkButton>
						<a class="action-button action-button-secondary" href="javascript: window.app.closeModal();">
							閉じる
						</a>
					</div>
				</ContentTemplate>
			</asp:UpdatePanel>
		</div>
	</form>
</body>
</html>
