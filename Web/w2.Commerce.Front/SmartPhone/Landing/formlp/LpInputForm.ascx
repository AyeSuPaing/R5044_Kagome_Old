<%--
=========================================================================================================
  Module      : LP入力フォーム(LpInputForm.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Landing/formlp/LpInputForm.ascx.cs" Inherits="Landing_formlp_LpInputForm" %>
<%@ Import Namespace="w2.Domain.LandingPage" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.Paidy" %>
<%@ Import Namespace="w2.App.Common.Amazon" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Register TagPrefix="uc" TagName="PaypalScriptsForm" Src="~/Form/Common/PayPalScriptsForm.ascx" %>
<%@ Register TagPrefix="uc" TagName="UserRegistRegulationMessage" Src="~/Form/User/UserRegistRegulationMessage.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionPayPal" Src="~/Form/Common/Order/PaymentDescriptionPayPal.ascx" %>
<%@ Register TagPrefix="uc" TagName="Layer" Src="~/SmartPhone/Form/Common/Layer/SearchResultLayer.ascx" %>
<%@ Register TagPrefix="uc" TagName="CreditToken" Src="~/Form/Common/CreditToken.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaidyCheckoutScript" Src="~/Form/Common/Order/PaidyCheckoutScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaidyCheckoutControl" Src="~/Form/Common/Order/PaidyCheckoutControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionAtone" Src="~/Form/Common/Order/PaymentDescriptionAtone.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionNPAfterPay" Src="~/Form/Common/Order/PaymentDescriptionNPAfterPay.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionLinePay" Src="~/Form/Common/Order/PaymentDescriptionLinePay.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionPayPay" Src="~/Form/Common/Order/PaymentDescriptionPayPay.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionCvsDef" Src="~/Form/Common/Order/PaymentDescriptionCvsDef.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionSmsDef" Src="~/Form/Common/Order/PaymentDescriptionSmsDef.ascx" %>
<%@ Register TagPrefix="uc" TagName="EcPayScript" Src="~/Form/Common/ECPay/EcPayScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyUserExtendLandingPageRegist" Src="~/Form/Common/User/BodyUserExtendLandingPageRegist.ascx" %>
<%@ Register TagPrefix="uc" TagName="RakutenCreditCard" Src="~/SmartPhone/Form/Common/RakutenCreditCardModal.ascx" %>
<%@ Register TagPrefix="uc" TagName="RakutenPaymentScript" Src="~/Form/Common/RakutenPaymentScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="MailDomains" Src="~/Form/Common/MailDomains.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyFixedPurchaseOrderPrice" Src="~/Form/Common/BodyFixedPurchaseOrderPrice.ascx" %>
<%@ Register Src="~/Form/Common/UserLoginScript.ascx" TagPrefix="uc" TagName="UserLoginScript" %>
<%@ Register Src="~/Form/Common/SendAuthenticationCodeModal.ascx" TagPrefix="uc" TagName="SendAuthenticationCodeModal" %>

<% if (this.IsCartListLp) { %>
	<div class="step"><img src="<%= Constants.PATH_ROOT %>SmartPhone/Contents/ImagePkg/commoSetPaidyBuyer(); return CheckBeforeNextPage();n/cart-lp-step01.jpg" alt="カート内容の確認" width="320" /></div>
<% } %>

<% if(Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED) { %>
<style>
	.convenience-store-item {
		margin: 2px;
		padding: 4px;
	}

	.convenience-store-button {
		display: block;
		padding: 1em;
		background-color: #000;
		margin: .5em 0;
		width: 50%;
		text-align: center;
		color: #fff !important;
	}
</style>
<% } %>
<% if(Constants.SUBSCRIPTION_BOX_OPTION_ENABLED) { %>
	<style>
		.toggle-wrap .toggle-button {
			display: block;
			cursor: pointer;
			padding: 3px 10px;
			background-color: #fff;
			border: 1px solid #777;
			text-align: center;
			margin-bottom: 1em;
			border-radius: 3px;
		}

		.toggle-wrap .toggle-button:hover {
			border-color: red;
		}

		.toggle-wrap .toggle-content,
		.toggle-wrap > input[type="checkbox"] {
			display: none;
		}

			.toggle-wrap > input[type="checkbox"]:checked ~ .toggle-content { display: block; }

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
	</style>
<% } %>
<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
<uc:EcPayScript runat="server" ID="ucECPayScript" />
<% } %>
<div class="formlp-front-section-form">
<asp:UpdatePanel runat="server" UpdateMode="Conditional">
	<ContentTemplate>
<asp:DropDownList ID="ddlProductSet" CssClass="input_border" style="width: 100%" OnSelectedIndexChanged="ddlProductSet_OnSelectedIndexChanged" runat="server" DataTextField="Text" DataValueField="Value" AutoPostBack="true"></asp:DropDownList>
	</ContentTemplate>
</asp:UpdatePanel>
<asp:LinkButton ID="lbNext" OnClick="lbNext_Click" ValidationGroup="OrderShipping-OrderPayment" runat="server"></asp:LinkButton>

<section class="wrap-order landing-cart-input">
<div class="order-unit">

<%-- UPDATE PANEL開始 --%>
<asp:UpdatePanel ID="upUpdatePanel" UpdateMode="Conditional" ChildrenAsTriggers="false" runat="server">
<ContentTemplate>

	<asp:UpdatePanel id="upCartListItemsCount" runat="server">
		<ContentTemplate>
			<h3>
				ご注文内容
				<% if (this.IsCartListLp) { %>
					<%: (this.CartList.Items.Count > 1) ? ("(合計" + this.CartList.Items.Count + "カート)") : "" %>
				<% } %>
			</h3>
			<% if (this.IsCartListLp && (this.CartList.Items.Count == 0)) { %>
				<div class="msg-alert">カートに商品がありません。</div>
			<% } %>
		</ContentTemplate>
	</asp:UpdatePanel>

	<asp:UpdatePanel runat="server" ID="upSocialLogin">
	<ContentTemplate>
	<span style="color: red; text-align: left; display: block"><asp:Literal ID="lPaymentErrorMessage" runat="server"></asp:Literal></span>
	<%-- ▼PayPalログインここから▼ --%>
	<% if (Constants.PAYPAL_LOGINPAYMENT_ENABLED && DisplaySocialLoginBtnCheck(LandingPageConst.SocialLoginType.PayPal) && this.CanUsePayPalPayment()) { %>
		<div style="text-align: center;margin: 5px">
			<%
				ucPaypalScriptsForm.LogoDesign = "Payment";
				ucPaypalScriptsForm.AuthCompleteActionControl = lbPayPalAuthComplete;
				ucPaypalScriptsForm.GetShippingAddress = (this.IsLoggedIn == false);
			%>
			<uc:PaypalScriptsForm ID="ucPaypalScriptsForm" runat="server" />
			<% if (this.Process.IsSubscriptionBoxError == false){ %>
			<div id="paypal-button"></div>
			<%if (SessionManager.PayPalCooperationInfo != null) {%>
				<%: (SessionManager.PayPalCooperationInfo != null) ? SessionManager.PayPalCooperationInfo.AccountEMail : "" %> 連携済
			<%} %>
			<br /><asp:LinkButton ID="lbPayPalAuthComplete" runat="server" OnClick="lbPayPalAuthComplete_Click"></asp:LinkButton>
			<% } %>
		</div>
	<%} %>
	<%-- ▲PayPalログインここまで▲ --%>

	<% if (this.Process.IsSubscriptionBoxError == false){ %>
		<% if (this.CanUseAmazonPayment() && this.CheckAmazonPaymentLandingPageDesignLimit()) { %>
		<% if (this.IsAmazonLoggedIn == false) { %>
			<%-- ▼▼Amazonお支払いボタンウィジェット▼▼ --%>
			<div style="width:200px;margin-left:auto;" id="AmazonPayButton"></div>
			<div style="margin-left:auto;width:200px;">
				<%--▼▼ Amazon Pay(CV2)ボタン ▼▼--%>
				<div id="AmazonPayCv2Button"></div>
				<%--▲▲ Amazon Pay(CV2)ボタン ▲▲--%>

				<asp:HiddenField ID="hfAmazonCv2Payload" Value="<%# this.AmazonRequest.Payload %>" runat="server" />
				<asp:HiddenField ID="hfAmazonCv2Signature" Value="<%# this.AmazonRequest.Signature %>" runat="server" />
			</div>
			<%-- ▲▲Amazonお支払いボタンウィジェット▲▲ --%>
		<% } else { %>
			<div style="width:200px;margin-left:auto;"><asp:LinkButton id="lbCancelAmazonPay" runat="server" class="btn" OnClick="lbCancelAmazonPay_Click">Amazonお支払いをやめる</asp:LinkButton></div>
		<% } %>
	<% } %>
	<% } %>
	</ContentTemplate>
	</asp:UpdatePanel>
		
		<div id="CartList"></div>
		<asp:UpdatePanel id="upInitializeCartItemIndexTmp" runat="server">
		<ContentTemplate>
			<% this.CartItemIndexTmp = -1; %>
		</ContentTemplate>
		</asp:UpdatePanel>
		<% if (this.ErrorMessages.HasMessages(-1, -1)) { %><div class="attention msg-alert"><%: this.ErrorMessages.Get(-1, -1) %></div><% } %>
		<asp:UpdatePanel ID="upProductList" runat="server">
		<ContentTemplate>
		<% if (this.IsDropDownList) { %>
		<h2>商品選択</h2>
		<asp:DropDownList ID="ddlProductList" CssClass="input_border" style="width: 100%" OnSelectedIndexChanged="ddlProductList_OnSelectedIndexChanged" runat="server" DataTextField="Text" DataValueField="Value" AutoPostBack="true"></asp:DropDownList>
		<% } else if (this.IsSelectFromList) { %>
		<table class="cart-table">
		<tbody>
		<div class="productList">
			<div class="product">
				<asp:Repeater ID="rProductList" ItemType="LandingCartProduct" runat="server">
				<ItemTemplate>
				<tr class="cart-unit-product">
					<td class="product-image">
						<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Item.ShopId, "", "", "", Item.ProductId, Item.VariationId, Item.ProductJointName, "")) %>'>
							<w2c:ProductImage ProductMaster="<%# Item.Product %>" ImageSize="M" runat="server" />
						</a>
					</td>
					<td class="product-info">
						<ul>
							<li class="product-name">
								<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Item.ShopId, "", "", "", Item.ProductId, Item.VariationId, Item.ProductJointName, "")) %>'>
									<%# WebSanitizer.HtmlEncode(Item.ProductJointName) %>
								</a>
							</li>
							<li class="product-price" Visible="<%# this.Process.IsSubscriptionBoxFixedAmount == false %>" runat="server">
								<%#: CurrencyManager.ToPrice(Item.Price) %> (<%#: this.ProductPriceTextPrefix %>)
							</li>
						</ul>
					</td>

					<td class="product-control">
						<div class="amout">
						<span>
							<asp:TextBox ID="tbProductCount" Runat="server" Text='<%# Item.ProductCount %>' MaxLength="3"></asp:TextBox>
						</span>
						</div>
					</td>

					<td>
						<div  style="width: 15%;">
							<% if (this.IsCheckBox) { %>
							<asp:CheckBox ID="cbPurchase" Checked="<%# Item.Selected %>" Enabled="<%# CanNecessaryProducts(this.Process.SubscriptionBoxCourseId, Item.VariationId, false) == false %>" runat="server" />
							<% } %>
						</div>
					</td>
				</tr>
				</ItemTemplate>
				</asp:Repeater>
			</div>
		</div>
		</tbody>
		</table>
		<% } %>
		</ContentTemplate>
		</asp:UpdatePanel>
		<%-- 隠し再計算ボタン --%>
		<asp:LinkButton id="lbCreateCart" runat="server" onclick="lbCreateCart_Click"></asp:LinkButton>
	<%-- UPDATE PANEL開始(カートリスト) --%>
	<asp:UpdatePanel ID="upCartListUpdatePanel" UpdateMode="Conditional" ChildrenAsTriggers="False" runat="server">
	<ContentTemplate>
	<asp:Repeater ID="rCartList" runat="server">
		<ItemTemplate>
		<div class="cart-unit">
			<%-- ▼商品一覧▼ --%>
			<%-- UPDATE PANEL開始(商品一覧) --%>
			<asp:UpdatePanel ID="upProductListUpdatePanel" runat="server">
			<ContentTemplate>
			<asp:PlaceHolder ID="phHanpukaiErrorMsg" runat="server" Visible="<%# string.IsNullOrEmpty(((CartObject)((IDataItemContainer)Container).DataItem).SubscriptionBoxErrorMsg) == false %>">
				<span style="color:red"><%# ((CartObject)((IDataItemContainer)Container).DataItem).SubscriptionBoxErrorMsg %></span>
			</asp:PlaceHolder>
		<div runat="server">
		<asp:HiddenField ID="hfSubscriptionBoxCourseId" runat="server" Value="<%#((CartObject)((IDataItemContainer)Container).DataItem).SubscriptionBoxCourseId%>" />
		<h3>カート番号 <%# ((RepeaterItem)Container).ItemIndex + 1 %> のご注文内容</h3>
			<span class="fred"><asp:Literal ID="lMemberRankError" runat="server"></asp:Literal></span>
			<asp:Repeater id="rCart" runat="server" DataSource='<%# (CartObject)((RepeaterItem)Container).DataItem %>' OnItemCommand="rCartList_ItemCommand">
			<HeaderTemplate>
				<table class="cart-table">
				<tbody>
			</HeaderTemplate>
			<ItemTemplate>
				<tr class="cart-unit-product" visible="<%# ((CartProduct)Container.DataItem).IsSetItem == false && ((CartProduct)Container.DataItem).QuantitiyUnallocatedToSet != 0 %>" runat="server">
					<td class="product-image">
						<%-- 隠し値 --%>
						<asp:HiddenField ID="hfShopId" runat="server" Value="<%# ((CartProduct)Container.DataItem).ShopId %>" />
						<asp:HiddenField ID="hfProductId" runat="server" Value="<%# ((CartProduct)Container.DataItem).ProductId %>" />
						<asp:HiddenField ID="hfVariationId" runat="server" Value="<%# ((CartProduct)Container.DataItem).VariationId %>" />
						<asp:HiddenField ID="hfIsFixedPurchase" runat="server" Value="<%# ((CartProduct)Container.DataItem).IsFixedPurchase %>" />
						<asp:HiddenField ID="hfProductSaleId" runat="server" Value="<%# ((CartProduct)Container.DataItem).ProductSaleId %>" />
						<asp:HiddenField ID="hfProductOptionValue" runat="server" Value='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues() %>' />
						<asp:HiddenField ID="hfUnallocatedQuantity" runat="server" Value='<%# ((CartProduct)Container.DataItem).QuantitiyUnallocatedToSet %>' />
						<asp:HiddenField ID="hfSubscriptionBoxCourseId" runat="server" Value='<%# ((CartProduct)Container.DataItem).SubscriptionBoxCourseId %>' />
						<asp:HiddenField ID="hfAddCartKbn" runat="server" Value="<%# ((CartProduct)Container.DataItem).AddCartKbn %>" />
						<asp:HiddenField ID="hfSubscriptionBoxCourseName" runat="server" Value="<%# GetSubscriptionBoxDisplayName(((CartProduct)Container.DataItem).SubscriptionBoxCourseId) %>" />
						<input type="hidden" id="hfProductName" value="<%# ((CartProduct)Container.DataItem).ProductName %>" />
						<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(((CartProduct)Container.DataItem).ShopId, "", "", "", ((CartProduct)Container.DataItem).ProductId, ((CartProduct)Container.DataItem).VariationId, ((CartProduct)Container.DataItem).ProductJointName, "")) %>'>
							<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" />
						</a>
					</td>
					<td class="product-info">
						<ul>
							<li class="product-name">
								<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(((CartProduct)Container.DataItem).ShopId, "", "", "", ((CartProduct)Container.DataItem).ProductId, ((CartProduct)Container.DataItem).VariationId, ((CartProduct)Container.DataItem).ProductJointName, "")) %>'>
									<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %>
								</a>
								<%# (((CartProduct)Container.DataItem).GetProductTag("tag_cart_product_message").Length != 0) ? "<p class=\"product-msg\">" + WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).GetProductTag("tag_cart_product_message")) + "</p>" : "" %>
							</li>
							<li visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
								<span style="color:red;">※配送料無料適用外商品です</span>
							</li>
							<li visible='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList.IsSelectedProductOptionValueAll %>' runat="server">
								<asp:Repeater ID="rProductOptionSettings" ItemType="w2.App.Common.Product.ProductOptionSetting" DataSource='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList %>' runat="server">
									<ItemTemplate>
										<%#: Item.GetDisplayProductOptionSettingSelectValue() %>
										<%# (Item.GetDisplayProductOptionSettingSelectValue() != "") ? "<br />" : "" %>
									</ItemTemplate>
								</asp:Repeater>
							</li>
							<li class="product-price" runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false %>">
								<%#: CurrencyManager.ToPrice(((CartProduct)Container.DataItem).Price) %> (<%#: this.ProductPriceTextPrefix %>)
							</li>
							<li Visible="<%# this.SkipOrderConfirm %>" runat="server">
								<div Visible="<%# ((CartProduct)Container.DataItem).IsDisplaySell && ((CartProduct.IsDisplaySubscriptionBoxSelectTime(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)) == false) %>" runat="server">販売期間：<br/></div>
								<div Visible="<%# ((CartProduct)Container.DataItem).IsDisplaySell && ((CartProduct.IsDisplaySubscriptionBoxSelectTime(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)) == false) %>" class="spSpecifiedcommercialtransactionsPaddingleft10px" runat="server">
									<%#: DateTimeUtility.ToStringFromRegion(((CartProduct)Container.DataItem).SellFrom, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime) %>～<br/>
									<%#: DateTimeUtility.ToStringFromRegion(((CartProduct)Container.DataItem).SellTo, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime) %>
								</div>
								<%--頒布会の選択可能期間--%>
								<div Visible="<%# (CartProduct.IsDisplaySubscriptionBoxSelectTime(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)) %>" runat="server" >販売期間</div>
								<div Visible="<%# (CartProduct.IsDisplaySubscriptionBoxSelectTime(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)) %>" class="spSpecifiedcommercialtransactionsPaddingleft10px" runat="server" >
									<%# WebSanitizer.HtmlEncodeChangeToBr((CartProduct.GetSubscriptionBoxSelectTermBr(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)))%>
								</div>
								<div Visible="<%# ((CartProduct)Container.DataItem).IsDispSaleTerm %>" runat="server">タイムセール期間:<br/></div>
								<div Visible="<%# ((CartProduct)Container.DataItem).IsDispSaleTerm %>" class="spSpecifiedcommercialtransactionsPaddingleft10px" runat="server">
									<%#: (ProductCommon.GetProductSaleTermBrLpBeginDate(this.ShopId, ((CartProduct)Container.DataItem).ProductSaleId)) %>～<br/>
									<%#: (ProductCommon.GetProductSaleTermBrLpEndDate(this.ShopId, ((CartProduct)Container.DataItem).ProductSaleId)) %>
								</div>
								<div Visible="<%# ((((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount()) == false) && (CartProduct.IsSubscriptionBoxCampaignPeriod(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)) %>" style="color: red;" runat="server" >キャンペーン期間：</div>
								<div Visible="<%# ((((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount()) == false) && (CartProduct.IsSubscriptionBoxCampaignPeriod(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)) %>" class="spSpecifiedcommercialtransactionsPaddingleft10pxRed" runat="server" >
									<%# WebSanitizer.HtmlEncodeChangeToBr((CartProduct.GetSubscriptionBoxTermBr(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)))%>
								</div>
							</li>
						</ul>
						<p class="attention" visible='<%# this.ErrorMessages.HasMessages(GetParentRepeaterItem((Control)Container, "rCartList").ItemIndex, Container.ItemIndex) %>' runat="server">
							<%# WebSanitizer.HtmlEncode(this.ErrorMessages.Get(GetParentRepeaterItem((Control)Container, "rCartList").ItemIndex, Container.ItemIndex)) %>
						</p>
					</td>

					<td class="product-control">
						<div class="amout">
						<span>
							<asp:TextBox ID="tbProductCount" Runat="server" Text='<%# ((CartProduct)Container.DataItem).QuantitiyUnallocatedToSet %>' MaxLength="3"></asp:TextBox>
						</span>
						</div>
						<div class="delete" Visible="<%# CanDeleteProduct((CartProduct)Container.DataItem) %>" runat="server">
							<asp:LinkButton ID="lbDeleteProduct" CommandName="DeleteProduct" Runat="server" Visible="<%# HasNecessaryProduct(((CartProduct)Container.DataItem).SubscriptionBoxCourseId, ((CartProduct)Container.DataItem).ProductId) == false %>" Text="削除" CssClass="btn"/>
							<asp:LinkButton ID="lbDeleteNecessaryProduct" OnClientClick="return delete_product_check_for_subscriptionBox(this);" Runat="server" Visible="<%# HasNecessaryProduct(((CartProduct)Container.DataItem).SubscriptionBoxCourseId, ((CartProduct)Container.DataItem).ProductId) %>" CommandName="DeleteNecessarySubscriptionProduct" Text="削除" CssClass="btn"/>
						</div>
					</td>
				</tr>
				<table class="wrap-product-option">
				<tr>
					<%-- ▽商品付帯情報▽ --%>
					<td visible="<%# this.IsCartListLp == false %>" runat="server">
							<asp:Repeater ID="rProductOptionValueSettings" DataSource='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList %>' runat="server">
							<ItemTemplate>
								<tr>
									<th class="wrap-product-option-value-name"><%# WebSanitizer.HtmlEncode(((ProductOptionSetting)Container.DataItem).ValueName) %><span id="Span2" class="necessary" style="color: red;" runat="server" visible="<%# ((ProductOptionSetting)Container.DataItem).IsNecessary %>">*</span></th>
								</tr>
								<tr class="wrap-product-option-list">
									<td>
										<div class="wrap-product-option-disp-kbn">
											<asp:Repeater ID="rCblProductOptionValueSetting" DataSource='<%# ((ProductOptionSetting)Container.DataItem).SettingValuesListItemCollection %>' ItemType="System.Web.UI.WebControls.ListItem" visible='<%# ((ProductOptionSetting)Container.DataItem).DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_CHECKBOX || ((ProductOptionSetting)Container.DataItem).DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_PRICE_CHECKBOX %>' runat="server" >
												<ItemTemplate>
													<asp:CheckBox AutoPostBack="True" OnCheckedChanged="cbProductOptionValueSettingListOnCheckedChanged" ID="cbProductOptionValueSetting" Text='<%# Item.Text %>' Checked='<%# Item.Selected %>' runat="server" />
												</ItemTemplate>
											</asp:Repeater>
											<asp:DropDownList
												OnSelectedIndexChanged="cbProductOptionValueSettingListOnCheckedChanged"
												AutoPostBack="True"
												ID="ddlProductOptionValueSetting"
												DataSource='<%# InsertDefaultAtFirstToDdlProductOptionSettingList(((ProductOptionSetting)Container.DataItem).SettingValuesListItemCollection, ((ProductOptionSetting)Container.DataItem).IsNecessary) %>'
												visible='<%# (((ProductOptionSetting)Container.DataItem).DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_SELECTMENU)
															|| (((ProductOptionSetting)Container.DataItem).DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_PRICE_DROPDOWNMENU) %>'
												SelectedValue='<%# ((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectedValue() %>'
												runat="server" />
											<asp:TextBox ID ="txtProductOptionValueSetting"
												Text = '<%# (string.IsNullOrEmpty(((ProductOptionSetting)Container.DataItem).SelectedSettingValue) == false) 
														? ((ProductOptionSetting)Container.DataItem).SelectedSettingValue : ((ProductOptionSetting)Container.DataItem).DefaultValue %>' visible='<%# ((ProductOptionSetting)Container.DataItem).IsTextBox %>' runat="server" />
										</div>
									</td>
								</tr>
							</ItemTemplate>
							</asp:Repeater>
					</td>
					<%-- △商品付帯情報△ --%>
				</tr>
				</table>
				<% if (this.IsCartListLp) { %>
				<%-- セット商品 --%>
				<div visible="<%# (((CartProduct)Container.DataItem).IsSetItem) && (((CartProduct)Container.DataItem).ProductSetItemNo == 1) %>" runat="server">
					<%-- 隠し値 --%>
					<asp:HiddenField ID="hfIsSetItem" runat="server" Value="<%# ((CartProduct)Container.DataItem).IsSetItem %>" />
					<asp:HiddenField ID="hfProductSetId" runat="server" Value="<%# OrderPage.GetProductSetId((CartProduct)Container.DataItem) %>" />
					<asp:HiddenField ID="hfProductSetNo" runat="server" Value="<%# OrderPage.GetProductSetNo((CartProduct)Container.DataItem) %>" />
					<asp:HiddenField ID="hfProductSetItemNo" runat="server" Value="<%# ((CartProduct)Container.DataItem).ProductSetItemNo %>" />
					<div>
						<asp:Repeater id="rProductSet" ItemType="w2.App.Common.Order.CartProduct" DataSource="<%# (((CartProduct)Container.DataItem).ProductSet != null) ? ((CartProduct)Container.DataItem).ProductSet.Items : null %>" OnItemCommand="rCartList_ItemCommand" runat="server">
						<ItemTemplate>
							<tr class="cart-unit-product">
								<td class="product-image">
									<w2c:ProductImage ID="ProductImage1" ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" />
								</td>
								<td class="product-info">
									<span>
										<%#: (Item.ProductJointName + " x " + Item.CountSingle) %>
										<%#: (Item.GetProductTag("tag_cart_product_message").Length != 0) ? ("<br/><p class=\"message\">" + Item.GetProductTag("tag_cart_product_message") + "</p>") : "" %>
									</span>
									<p class="product-price"><%#: CurrencyManager.ToPrice(Item.Price) %>(<%#: this.ProductPriceTextPrefix %>)</p>
									<p visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
										<span style="color:red;">※配送料無料適用外商品です</span>
									</p>
								</td>
								<td class="product-control" visible="<%# (Item.ProductSetItemNo == 1) %>" rowspan="<%# (Item.ProductSet != null) ? Item.ProductSet.Items.Count : 1 %>" runat="server">
									<div class="amout">
										<asp:TextBox ID="tbProductSetCount" Runat="server" Text='<%# OrderPage.GetProductSetCount(Item) %>' MaxLength="3" CssClass="orderCount"></asp:TextBox>
									</div>
									<div class="product-price" style="text-align:center;">
										<%#: CurrencyManager.ToPrice(OrderPage.GetProductSetPriceSubtotal(Item)) %>(<%#: this.ProductPriceTextPrefix %>)
									</div>
									<div class="delete">
										<asp:LinkButton ID="lbDeleteProductSet" CommandName="DeleteProductSet" Runat="server" CssClass="btn">削除</asp:LinkButton>
									</div>
								</td>
							</tr>
						</ItemTemplate>
						</asp:Repeater>
					</div>
					<tr visible='<%# this.ErrorMessages.HasMessages(GetParentRepeaterItem(Container, "rCartList").ItemIndex, Container.ItemIndex) %>' runat="server">
						<td colspan="2">
							<p class="attention">
								<%#: this.ErrorMessages.Get(GetParentRepeaterItem(Container, "rCartList").ItemIndex, Container.ItemIndex) %>
							</p>
						</td>
						<td class="product-control"></td>
					</tr>
				</div>
				<% } %>
			</ItemTemplate>
			<FooterTemplate>
				</tbody>
				</table>
			</FooterTemplate>
			</asp:Repeater>

			<%-- セットプロモーション --%>
				<asp:Repeater ID="rCartSetPromotion" DataSource="<%# ((CartObject)((RepeaterItem)Container).DataItem).SetPromotions %>" runat="server">
			<ItemTemplate>

			<div class="cart-set-promotion-unit">

			<asp:HiddenField ID="hfCartSetPromotionNo" runat="server" Value="<%# ((CartSetPromotion)Container.DataItem).CartSetPromotionNo %>" />

			<asp:Repeater ID="rCartSetPromotionItem" DataSource="<%# ((CartSetPromotion)Container.DataItem).Items %>" OnItemCommand="rCartList_ItemCommand" runat="server">
			<HeaderTemplate>
				<table class="cart-table">
				<tbody>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td class="product-image">
					<%-- 隠し値 --%>
					<asp:HiddenField ID="hfShopId" runat="server" Value="<%# ((CartProduct)Container.DataItem).ShopId %>" />
					<asp:HiddenField ID="hfProductId" runat="server" Value="<%# ((CartProduct)Container.DataItem).ProductId %>" />
					<asp:HiddenField ID="hfVariationId" runat="server" Value="<%# ((CartProduct)Container.DataItem).VariationId %>" />
					<asp:HiddenField ID="hfIsFixedPurchase" runat="server" Value="<%# ((CartProduct)Container.DataItem).IsFixedPurchase %>" />
					<asp:HiddenField ID="hfProductSaleId" runat="server" Value="<%# ((CartProduct)Container.DataItem).ProductSaleId %>" />
					<asp:HiddenField ID="hfProductOptionValue" runat="server" Value='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues() %>' />
					<asp:HiddenField ID="hfAllocatedQuantity" runat="server" Value='<%# ((CartProduct)Container.DataItem).QuantityAllocatedToSet[((CartSetPromotion)GetParentRepeaterItem(Container, "rCartSetPromotion").DataItem).CartSetPromotionNo] %>' />

					<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(((CartProduct)Container.DataItem).ShopId, "", "", "", ((CartProduct)Container.DataItem).ProductId, ((CartProduct)Container.DataItem).VariationId, ((CartProduct)Container.DataItem).ProductJointName, "")) %>'>
						<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" />
					</a>
					</td>
					<td class="product-info">
						<ul>
							<li class="product-name">
								<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(((CartProduct)Container.DataItem).ShopId, "", "", "", ((CartProduct)Container.DataItem).ProductId, ((CartProduct)Container.DataItem).VariationId, ((CartProduct)Container.DataItem).ProductJointName, "")) %>'>
									<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %>
								</a>
								<div visible='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList.IsSelectedProductOptionValueAll %>' runat="server">
								<asp:Repeater ID="rProductOptionSettings" DataSource='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList %>' runat="server">
								<ItemTemplate>
									<%# WebSanitizer.HtmlEncode(((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue()) %>
									<%# (((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue() != "") ? "<br />" : "" %>
								</ItemTemplate>
								</asp:Repeater>
								</div>
							</li>
							<li class="product-price">
								<%#: CurrencyManager.ToPrice(((CartProduct)Container.DataItem).Price) %> (<%#: this.ProductPriceTextPrefix %>)
							</li>
							<li visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
								<span class="error" style="color:red;">※配送料無料適用外商品です</span>
							</li>
						</ul>
						<p class="attention" visible='<%# this.ErrorMessages.HasMessages(GetParentRepeaterItem(Container, "rCartSetPromotion").ItemIndex, Container.ItemIndex) %>' runat="server">
							<%# WebSanitizer.HtmlEncode(this.ErrorMessages.Get(GetParentRepeaterItem(Container, "rCartSetPromotion").ItemIndex, Container.ItemIndex)) %>
						</p>
						<p class="attention" visible='<%# this.ErrorMessages.HasMessages(GetParentRepeaterItem(Container, "rCartList").ItemIndex, GetParentRepeaterItem(Container, "rCartSetPromotion").ItemIndex, Container.ItemIndex) %>' runat="server">
							<%# WebSanitizer.HtmlEncode(this.ErrorMessages.Get(GetParentRepeaterItem(Container, "rCartList").ItemIndex, GetParentRepeaterItem(Container, "rCartSetPromotion").ItemIndex, Container.ItemIndex))%>
						</p>
					</td>
					<td class="product-control">
						<div class="amout">
							<span>
								<asp:TextBox ID="tbSetPromotionItemCount" Runat="server" Text='<%# ((CartProduct)Container.DataItem).QuantityAllocatedToSet[((CartSetPromotion)(GetParentRepeaterItem(Container, "rCartSetPromotion").DataItem)).CartSetPromotionNo] %>' MaxLength="3"></asp:TextBox>
							</span>
							<p class="attention" visible='<%# this.ErrorMessages.HasMessages(GetParentRepeaterItem(Container, "rCartList").ItemIndex, GetParentRepeaterItem(Container, "rCartSetPromotion").ItemIndex, Container.ItemIndex) %>' runat="server">
								<%# WebSanitizer.HtmlEncode(this.ErrorMessages.Get(GetParentRepeaterItem(Container, "rCartList").ItemIndex, GetParentRepeaterItem(Container, "rCartSetPromotion").ItemIndex, Container.ItemIndex)) %>
							</p>
						</div>
						<div class="delete">
							<asp:LinkButton ID="lbDeleteProduct" visible="<%# CanDeleteSetPromotionProduct((CartProduct)Container.DataItem) %>" CommandName="DeleteProduct" CommandArgument='' Runat="server"  CssClass="btn">削除</asp:LinkButton>
						</div>
					</td>
				</tr>
			</ItemTemplate>
			<FooterTemplate>
				</tbody>
				</table>
			</FooterTemplate>
			</asp:Repeater>

			<div class="set-promotion-footer">
				<dl>
					<dt><%# WebSanitizer.HtmlEncode(((CartSetPromotion)Container.DataItem).SetpromotionDispName) %></dt>
					<dd style="text-align:left; margin-left:unset;padding-right:150px" class="line-through" visible="<%# ((CartSetPromotion)Container.DataItem).IsDiscountTypeProductDiscount %>" runat="server">
					<%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).UndiscountedProductSubtotal) %> (<%#: this.ProductPriceTextPrefix %>)
					</dd>
					<dd style="text-align:left; margin-left:unset; padding-top:0px;"><%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).UndiscountedProductSubtotal - ((CartSetPromotion)Container.DataItem).ProductDiscountAmount) %> (<%#: this.ProductPriceTextPrefix %>)</dd>
					<asp:Repeater ID="rCartSetPromotionItemIsDisplaySell" DataSource="<%# ((CartSetPromotion)Container.DataItem).Items %>" OnItemCommand="rCartList_ItemCommand" runat="server">
					<ItemTemplate>
						<span runat="server" Visible="<%# this.SkipOrderConfirm %>">
							<div Visible="<%# (((CartProduct)Container.DataItem).IsDisplaySell) %>" runat="server">販売期間：<br/></div>
							<div class="spSpecifiedcommercialtransactionsPaddingleft10px" Visible="<%# (((CartProduct)Container.DataItem).IsDisplaySell) %>" runat="server">
								<%#: DateTimeUtility.ToStringFromRegion(((CartProduct)Container.DataItem).SellFrom, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime) %>～<br/>
								<%#: DateTimeUtility.ToStringFromRegion(((CartProduct)Container.DataItem).SellTo, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime) %>
							</div>
							<div Visible="<%# ((CartProduct)Container.DataItem).IsDispSaleTerm %>" runat="server">タイムセール期間:<br/></div>
							<div class="spSpecifiedcommercialtransactionsPaddingleft10px" Visible="<%# ((CartProduct)Container.DataItem).IsDispSaleTerm %>" runat="server">
								<%#: (ProductCommon.GetProductSaleTermBrLpBeginDate(this.ShopId, ((CartProduct)Container.DataItem).ProductSaleId)) %>～<br/>
								<%#: (ProductCommon.GetProductSaleTermBrLpEndDate(this.ShopId, ((CartProduct)Container.DataItem).ProductSaleId)) %>
							</div>
							<div >セットプロモーション期間:</div>
							<div class="spSpecifiedcommercialtransactionsPaddingleft10px" ><%# WebSanitizer.HtmlEncodeChangeToBr(ProductCommon.GetSetPromotionTermBr(((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).SetPromotionId)) %></div>
						</span>
					</ItemTemplate>
					</asp:Repeater>
				</dl>
			</div>

			</div>
			</ItemTemplate>
			</asp:Repeater>
			
			<%-- ▽ノベルティ▽ --%>
			<asp:Repeater ID="rNoveltyList" runat="server" ItemType="w2.App.Common.Order.CartNovelty" DataSource="<%# GetCartNovelty(((CartObject)((RepeaterItem)Container).DataItem).CartId)%>" Visible="<%# IsDisplayNovelty(((CartObject)((RepeaterItem)Container).DataItem).CartId) %>">
			<HeaderTemplate>
				<div class="cart-novelty-unit">
			</HeaderTemplate>
			<ItemTemplate>
				<p class="title">
					<%#: Item.NoveltyDispName %>を追加してください。<br/>
				</p>
				<p class="info" runat="server" visible="<%# (Item.GrantItemList.Length == 0) %>">
					ただいま付与できるノベルティはございません。
				</p>
				<asp:Repeater ID="rNoveltyItem" runat="server" ItemType="w2.App.Common.Order.CartNoveltyGrantItem" DataSource="<%# Item.GrantItemList %>" OnItemCommand="rCartList_ItemCommand">
				<HeaderTemplate>
					<table class="cart-table">
					<tbody>
				</HeaderTemplate>
				<ItemTemplate>
					<tr class="cart-unit-product">
						<td class="product-image">
							<w2c:ProductImage ProductMaster="<%# Item.ProductInfo %>" IsVariation="true" ImageSize="M" runat="server" />
						</td>
						<td class="product-info">
							<ul>
								<li class="product-name">
									<%#: Item.JointName %>
								</li>
								<li class="product-price">
									<%#: CurrencyManager.ToPrice(Item.Price) %>(<%#: this.ProductPriceTextPrefix %>)
								</li>
							</ul>
						</td>
						<td class="product-control">
							<div class="add">
								<asp:LinkButton ID="lbAddNovelty" CssClass="btn" runat="server" CommandName="AddNovelty" CommandArgument='<%# string.Format("{0},{1}", ((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex) %>'>追加</asp:LinkButton>
							</div>
						</td>
					</tr>
				</ItemTemplate>
				<FooterTemplate>
					</toboy>
					</table>
				</FooterTemplate>
				</asp:Repeater>
			</ItemTemplate>
			<FooterTemplate>
				</div>
			</FooterTemplate>
			</asp:Repeater>
			<%-- △ノベルティ△ --%>
		</div>

			<div class="SubscriptionOptionalItem" id="dvListProduct" runat="server" Visible="<%# this.HasOptionalProdects %>" style="width:700px;">
				<table cellspacing="0">
					<div runat="server" Visible="<%# CanNecessaryProducts(((CartObject)((RepeaterItem)Container).DataItem).SubscriptionBoxCourseId) %>">
						<tr>
							<td class="title_bg" colspan="6">
								<div style="display: inline-block;" runat="server" Visible="<%# this.SubscriputionBoxProductListModify.Any() %>">
									<p style="display: inline-block;">頒布会選択商品</p>
								</div>
								<div class="right" style=" text-align: right; display: inline-block;">
									<asp:Button style="width: 100px; height: 20px; vertical-align: middle; padding:2px 0; padding-bottom: 5px; margin-bottom: 5px;" Text="選択する" runat="server" OnClick="btnChangeProduct_Click" class="btn" />
								</div>
								<div class="dvSubscriptionOptional" runat="server" Visible="<%# this.SubscriputionBoxProductListModify.Any() == false %>">
									<p class="subscriptionMessage">頒布会商品を選択可能です</p>
								</div>
							</td>
						</tr>
					</div>
					<asp:Repeater ID="rItem" Visible="<%# this.SubscriputionBoxProductListModify.Any() %>" DataSource="<%# this.SubscriputionBoxProductListModify %>" ItemType="w2.Domain.SubscriptionBox.SubscriptionBoxDefaultItemModel" runat="server" OnItemDataBound="rItem_OnItemDataBound">
						<HeaderTemplate>
							<tr class="rtitle">
								<th class="productName" colspan="2">
									商品名
								</th>
								<th class="productPrice">
									単価（<%#: this.ProductPriceTextPrefix %>）
								</th>
								<th class="orderCount">
									注文数
								</th>
								<th class="orderSubtotal">
									小計（<%#: this.ProductPriceTextPrefix %>）
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
			<%-- 購入商品一覧 --%>
			<div class="SubscriptionOptionalItem" id="dvModifySubscription" runat="server" Visible="<%# this.HasOptionalProdects == false %>" style="width:700px;" >
				<div>
					<small ID="sErrorQuantity" class="error" runat="server"></small>
				</div>
				<table cellspacing="0">
					<tr>
						<td class="title_bg" colspan="6">
							<p style="display: inline-block;">頒布会任意商品の選択</p>
							<div class="right" style="text-align: right;" >
								<asp:Button Text="  商品追加  " runat="server" class="btn" style="width: 100px; height: 20px; vertical-align: middle; padding:2px 0; padding-bottom: 5px; margin-bottom: 5px; margin-left: auto; margin-right: 10px; display: inline-block;" OnClick="btnAddProduct_Click" CommandArgument="<%# ((CartObject)((RepeaterItem)Container).DataItem).SubscriptionBoxCourseId %>"/>
								<asp:Button Text="  決定  " runat="server" class="btn" style="width: 100px; height: 20px; vertical-align: middle; padding:2px 0; padding-bottom: 5px; margin-bottom: 5px; margin-left: auto; display: inline-block;" OnClick="btnUpdateProduct_Click" CommandArgument="<%# ((CartObject)((RepeaterItem)Container).DataItem).SubscriptionBoxCourseId %>"/>
							</div>
						</td>
					</tr>
					<asp:Repeater ID="rItemModify" DataSource="<%# this.SubscriputionBoxProductList %>" ItemType="w2.Domain.SubscriptionBox.SubscriptionBoxDefaultItemModel" runat="server" OnItemDataBound="rItem_OnItemDataBound" OnItemCommand="rProductChange_ItemCommand">
						<HeaderTemplate>
							<tr class="rtitle">
								<th class="productName" colspan="2">
									商品名
									<br>
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
									<asp:DropDownList ID="ddlProductName" DataSource="<%# GetSubscriptionBoxProductList(Item.ProductId,Item.VariationId, Item.SubscriptionBoxCourseId) %>" DataTextField="Text" DataValueField="Value" runat="server"
													OnSelectedIndexChanged="ReCalculation" SelectedValue='<%# string.Format("{0}/{1}/{2}", Item.ShopId, Item.ProductId, Item.VariationId) %>' AutoPostBack="True" />
									<br>
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
		<div class="cart-unit-footer">
			<dl class="use-point" visible="<%# Constants.W2MP_POINT_OPTION_ENABLED && this.IsLoggedIn %>" runat="server">
				<dt>ポイントを使う</dt>
				<dd runat="server" visible="<%# ((CartObject)((RepeaterItem)Container).DataItem).CanUsePointForPurchase %>">
					<w2c:ExtendedTextBox ID="tbOrderPointUse" Type="tel" Runat="server" Text="<%# GetUsePoint((CartObject)((RepeaterItem)Container).DataItem) %>" MaxLength="6" CssClass="input_widthA"></w2c:ExtendedTextBox>
					<br />
					<span>
						※1<%: Constants.CONST_UNIT_POINT_PT %> = <%: CurrencyManager.ToPrice(1m) %><br />
						<%# WebSanitizer.HtmlEncode(GetNumeric(this.LoginUserPointUsable))%> ポイントまで利用できます
					</span>
					<p class="attention" visible="<%# this.ErrorMessages.HasMessages(((RepeaterItem)Container).ItemIndex, OrderPage.CartErrorMessages.ErrorKbn.Point) %>" runat="server">
						<%# WebSanitizer.HtmlEncode(this.ErrorMessages.Get(((RepeaterItem)Container).ItemIndex, OrderPage.CartErrorMessages.ErrorKbn.Point)) %> 
					</p>
				</dd>
				<dd runat="server" visible="<%# (((CartObject)((RepeaterItem)Container).DataItem).CanUsePointForPurchase == false) %>">
					<p>
						あと「<%#: GetPriceCanPurchaseUsePoint(((CartObject)((RepeaterItem)Container).DataItem).PurchasePriceTotal) %>」の購入でポイントをご利用いただけます。
					</p>
					<p runat="server" visible="<%# (this.LoginUserPointUsable > 0) %>">
						※利用可能ポイント「<%#: GetNumeric(this.LoginUserPointUsable) %>pt」
					</p>
				</dd>
				<% if (Constants.W2MP_POINT_OPTION_ENABLED
					&& this.IsLoggedIn
					&& Constants.FIXEDPURCHASE_OPTION_ENABLED
					&& Constants.FIXEDPURCHASE_NEXT_SHIPPING_USE_POINT_ALL_OPTION_ENABLE) { %>
					<asp:CheckBox ID="CheckBox1" Text="定期注文で利用可能なポイントすべてを継続使用する"
						Visible="<%# ((CartObject)((RepeaterItem)Container).DataItem).HasFixedPurchase %>"
						OnCheckedChanged="cbUseAllPointFlg_Changed" OnDataBinding="cbUseAllPointFlg_DataBinding"
						CssClass="cbUseAllPointFlgSp" AutoPostBack="True" runat="server"/>
						<br/>
						<span Visible="<%# ((CartObject)((RepeaterItem)Container).DataItem).HasFixedPurchase %>" runat="server">※注文後はマイページ＞定期購入情報より変更できます。</span>
				<% } %>
			</dl>
			<dl class="coupon-point" style="list-style: none;" visible="<%# Constants.W2MP_COUPON_OPTION_ENABLED %>" runat="server">
				<dt>クーポンを使う</dt>
				<dd id="divCouponInputMethod" runat="server" style="font-size: 10px; padding: 10px 10px 0px 10px; font-family: 'Lucida Grande','メイリオ',Meiryo,'Hiragino Kaku Gothic ProN', sans-serif; color: #333;">
					<%-- RadioButtonListで生成されるInputタグのレイアウト修正用 --%>
					<style type="text/css">
						.coupon_input_method input
						{
							width: initial !important;
						}
					</style>
					<asp:RadioButtonList runat="server" AutoPostBack="true" ID="rblCouponInputMethod"
						OnSelectedIndexChanged="rblCouponInputMethod_SelectedIndexChanged" OnDataBinding="rblCouponInputMethod_DataBinding"
						DataSource="<%# GetCouponInputMethod() %>" DataTextField="Text" DataValueField="Value" RepeatColumns="2" RepeatDirection="Horizontal" CssClass="coupon_input_method"
						style="width: 100%;"></asp:RadioButtonList>
				</dd>
				<dd id="hgcCouponSelect" runat="server">
					<asp:DropDownList ID="ddlCouponList" runat="server" DataTextField="Text" DataValueField="Value"
						OnTextChanged="ddlCouponList_TextChanged" AutoPostBack="true"></asp:DropDownList>
				</dd>
				<div id="hgcCouponCodeInputArea" runat="server">
				<dd>
					<asp:TextBox ID="tbCouponCode" runat="server" Text="<%# GetCouponCode(((CartObject)((RepeaterItem)Container).DataItem).Coupon) %>" MaxLength="30" autocomplete="off"></asp:TextBox>
					<p class="attention" visible="<%# this.ErrorMessages.HasMessages(((RepeaterItem)Container).ItemIndex, OrderPage.CartErrorMessages.ErrorKbn.Coupon) %>" runat="server">
						<%# WebSanitizer.HtmlEncode(this.ErrorMessages.Get(((RepeaterItem)Container).ItemIndex, OrderPage.CartErrorMessages.ErrorKbn.Coupon)) %>
					</p>
				</dd>
				</div>
				<dd style="text-align: right;">
					<asp:LinkButton runat="server" ID="lbShowCouponBox" Text="クーポンBOX"
						style="padding: .3em 0; background-color: #333; color: #fff; margin-top: 1em; text-align: center; display: block; text-decoration: none !important; width: 100px; margin-left: auto;"
						OnClick="lbShowCouponBox_Click" ></asp:LinkButton>
				</dd>
				<div runat="server" id="hgcCouponBox" style="z-index: 20000; top: 0; left: 0; width: 100%; height: 120%; position: fixed; background-color: rgba(128, 128, 128, 0.75);"
					Visible="<%# ((CartObject)((RepeaterItem)Container).DataItem).CouponBoxVisible %>">
				<div id="hgcCouponList" style="width: 100%; height: 320px; top: 50%; left: 0; text-align: center; background: #fff; position: fixed; z-index: 200001; margin:-180px 0 0 0;">
				<h2 style="height: 20px; color: #fff; background-color: #000; margin-bottom: 0; margin-top: 0px; z-index: 20003">クーポンBOX</h2>
				<div style="height: 260px; overflow: auto; -webkit-overflow-scrolling: touch; z-index: 20003">
				<asp:Repeater ID="rCouponList" ItemType="w2.Domain.Coupon.Helper.UserCouponDetailInfo" Runat="server" DataSource="<%# GetUsableCoupons((CartObject)((RepeaterItem)Container).DataItem, SessionManager.ReferralCode) %>">
					<HeaderTemplate></HeaderTemplate>
					<ItemTemplate>
						<li>
							<h3 style="margin: 0 auto; border: 1px #888888;  background-color: #CCC; color:black; font-weight: bold;">
								<%# (StringUtility.ToEmpty(Item.CouponDispName) != "")
									? StringUtility.ToEmpty(Item.CouponDispName)
									: StringUtility.ToEmpty(Item.CouponCode) %></h3>
							<dl style="text-align: left;">
								<dd style="padding: 2px; text-align: left; margin-left: 0px;">
									クーポンコード：<%#: StringUtility.ToEmpty(Item.CouponCode) %>
										<asp:HiddenField runat="server" ID="hfCouponBoxCouponCode" Value="<%# Item.CouponCode %>" />
								</dd>
								<dd style="padding: 2px; text-align: left; margin-left: 0px;">有効期限：<%#: DateTimeUtility.ToStringFromRegion(Item.ExpireEnd, DateTimeUtility.FormatType.LongDateHourMinute1Letter) %></dd>
								<dd style="padding: 2px; text-align: left; margin-left: 0px;">割引金額/割引率：
										<%#: (StringUtility.ToEmpty(Item.DiscountPrice) != "")
										? CurrencyManager.ToPrice(Item.DiscountPrice)
										: (StringUtility.ToEmpty(Item.DiscountRate) != "")
											? StringUtility.ToNumeric(Item.DiscountRate) + "%"
											: "-" %>
								</dd>
								<dd style="padding: 2px; text-align: left; margin-left: 0px;">利用可能回数：
									<%#: GetCouponCount(Item) %>
								</dd>
								<dd style="padding: 2px; text-align: left; margin-left: 0px;"><%#: StringUtility.ToEmpty(Item.CouponDispDiscription) %></dd>
			</dl>
							<div style="margin: 0 auto; width: 150px; padding: 10px; height: 60px; background-color: white;">
								<asp:LinkButton runat="server" id="lbCouponSelect" OnClick="lbCouponSelect_Click"
									style="padding: .3em 0; background-color: #333; color: #fff; margin-top: 1em; text-align: center; display: block; text-decoration: none !important; line-height: 1.5;" >このクーポンを使う</asp:LinkButton>
							</div>
						</li>
					</ItemTemplate>
					<FooterTemplate></FooterTemplate>
				</asp:Repeater>
				</div>
				<div style="width: 100%; height: 40px; left: 0; text-align: center; border: 0.5px solid #efefef; background: #fff; position: fixed; z-index: 200002;">
					<asp:LinkButton ID="lbCouponBoxClose" OnClick="lbCouponBoxClose_Click" runat="server"
						style="width: 150px; align-content:center; padding: .3em 0; background-color: #ddd; color: #333; margin-top: 1em; text-align: center; display: block; text-decoration: none !important; line-height: 1.5; margin: auto; margin-top: 7px;">クーポンを利用しない</asp:LinkButton>
				</div>
				</div>
				</div>
			</dl>

			<%-- 小計 --%>
			<dl>
				<dt>小計（<%#: this.ProductPriceTextPrefix %>）</dt>
				<dd><%#: CurrencyManager.ToPrice(((CartObject)((RepeaterItem)Container).DataItem).PriceSubtotal) %></dd>
			</dl>

			<%-- セットプロモーション --%>
			<asp:Repeater DataSource="<%# ((CartObject)((RepeaterItem)Container).DataItem).SetPromotions %>" runat="server">
			<HeaderTemplate>
			</HeaderTemplate>
			<ItemTemplate>
			<dl visible="<%# ((CartSetPromotion)Container.DataItem).IsDiscountTypeProductDiscount %>" runat="server">
				<dt>
					<%# WebSanitizer.HtmlEncode(((CartSetPromotion)Container.DataItem).SetpromotionDispName) %>
				</dt>
				<dd>
					<%# (((CartSetPromotion)Container.DataItem).ProductDiscountAmount > 0) ? "-" : ""%><%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).ProductDiscountAmount) %>
				</dd>
			</dl>
			</ItemTemplate>
			<FooterTemplate>
			</FooterTemplate>
			</asp:Repeater>

			<%if (Constants.MEMBER_RANK_OPTION_ENABLED && this.IsLoggedIn){ %>
			<dl>
				<dt>会員ランク割引額</dt>
				<dd>
					<%# (((CartObject)((RepeaterItem)Container).DataItem).MemberRankDiscount > 0) ? "-" : ""%><%#: CurrencyManager.ToPrice(((CartObject)((RepeaterItem)Container).DataItem).MemberRankDiscount * ((((CartObject)((RepeaterItem)Container).DataItem).MemberRankDiscount < 0) ? -1 : 1)) %>
				</dd>
			</dl>
			<%} %>
			<%if (Constants.FIXEDPURCHASE_OPTION_ENABLED){ %>
			<div runat="server" visible="<%# (((CartObject)((RepeaterItem)Container).DataItem).HasFixedPurchase) %>">
			<dl>
				<dt>定期購入割引額</dt>
				<dd>
					<%#: (((CartObject)((RepeaterItem)Container).DataItem).FixedPurchaseDiscount > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(((CartObject)((RepeaterItem)Container).DataItem).FixedPurchaseDiscount * ((((CartObject)((RepeaterItem)Container).DataItem).FixedPurchaseDiscount < 0) ? -1 : 1)) %>
				</dd>
			</dl>
			</div>
			<%} %>
			<%if (Constants.MEMBER_RANK_OPTION_ENABLED && Constants.FIXEDPURCHASE_OPTION_ENABLED && this.IsLoggedIn){ %>
				<dl>
					<dt>定期会員割引額</dt>
					<dd>
						<%# (((CartObject)((RepeaterItem)Container).DataItem).FixedPurchaseMemberDiscountAmount > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(((CartObject)((RepeaterItem)Container).DataItem).FixedPurchaseMemberDiscountAmount * ((((CartObject)((RepeaterItem)Container).DataItem).FixedPurchaseMemberDiscountAmount < 0) ? -1 : 1)) %>
					</dd>
				</dl>
			<%} %>
			<%if (Constants.W2MP_COUPON_OPTION_ENABLED){ %>
				<dl>
					<dt>クーポン割引額</dt>
					<dd>
						<%#: GetCouponName(((CartObject)((RepeaterItem)Container).DataItem)) %>
						<%# (((CartObject)((RepeaterItem)Container).DataItem).UseCouponPrice > 0) ? "-" : "" %>
						<%#: CurrencyManager.ToPrice(((CartObject)((RepeaterItem)Container).DataItem).UseCouponPrice * ((((CartObject)((RepeaterItem)Container).DataItem).UseCouponPrice < 0) ? -1 : 1)) %>
					</dd>
				</dl>
			<%} %>
			<%if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsLoggedIn){ %>
			<dl>
				<dt>ポイント利用額</dt>
				<dd>
					<%# (((CartObject)((RepeaterItem)Container).DataItem).UsePointPrice > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(((CartObject)((RepeaterItem)Container).DataItem).UsePointPrice * ((((CartObject)((RepeaterItem)Container).DataItem).UsePointPrice < 0) ? -1 : 1)) %>
				</dd>
			</dl>
			<%} %>

			<%-- 配送料金 --%>
			<dl>
				<dt>配送料金</dt>
				<dd runat="server" style='<%# ((((CartObject)((RepeaterItem)Container).DataItem).ShippingPriceSeparateEstimateFlg) || (((CartObject)((RepeaterItem)Container).DataItem).IsDisplayShippingPriceUnsettled)) ? "display:none;" : ""%>'>
						<asp:Label ID="lbPriceShipping" Text="<%#: CurrencyManager.ToPrice(((CartObject)((RepeaterItem)Container).DataItem).PriceShipping) %>" runat="server"></asp:Label>
				</dd>
				<dd runat="server" style='<%# (((CartObject)((RepeaterItem)Container).DataItem).ShippingPriceSeparateEstimateFlg == false) ? "display:none;" : ""%>'>
					<%# WebSanitizer.HtmlEncode(((CartObject)((RepeaterItem)Container).DataItem).ShippingPriceSeparateEstimateMessage)%>
				</dd>
				<dd runat="server" style='<%#  (((CartObject)((RepeaterItem)Container).DataItem).ShippingPriceSeparateEstimateFlg == false) && (((CartObject)((RepeaterItem)Container).DataItem).IsDisplayShippingPriceUnsettled == false) ? "display:none;" : "color:red"%>'>
					配送先入力後に確定となります。
				</dd>
			</dl>
			<% if (this.IsShowPaymentFee) { %>
			<dl>
				<dt>決済手数料</dt>
				<dd>
					<%#: CurrencyManager.ToPrice(((CartObject)((RepeaterItem)Container).DataItem).Payment.PriceExchange) %>
				</dd>
			</dl>
			<% } %>
			<%-- セットプロモーション --%>
			<asp:Repeater DataSource="<%# ((CartObject)((RepeaterItem)Container).DataItem).SetPromotions %>" runat="server">
			<HeaderTemplate>
			</HeaderTemplate>
			<ItemTemplate>
			<dl visible="<%# ((CartSetPromotion)Container.DataItem).IsDiscountTypeShippingChargeFree %>" runat="server">
			<dt>
				<%# WebSanitizer.HtmlEncode(((CartSetPromotion)Container.DataItem).SetpromotionDispName) %>(送料割引)
			</dt>
			<dd>
				<%# (((CartSetPromotion)Container.DataItem).ShippingChargeDiscountAmount > 0) ? "-" : ""%><%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).ShippingChargeDiscountAmount) %>
			</dd>
			</dl>
			</ItemTemplate>
			<FooterTemplate>
			</FooterTemplate>
			</asp:Repeater>
			<%if (this.ProductIncludedTaxFlg == false) { %>
			<dl class='<%: (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
			<dt>消費税額</dt>
			<dd><%# CurrencyManager.ToPrice(((CartObject)((RepeaterItem)Container).DataItem).PriceTax) %></dd>
			</dl>
			<%} %>
			<dl>
				<dt>合計(税込)</dt>
				<% if (this.IsShowPaymentFee) { %>
				<dd>
					<%#: CurrencyManager.ToPrice(((CartObject)((RepeaterItem)Container).DataItem).PriceTotal) %>
				</dd>
				<% } else { %>
				<dd>
					<%#: CurrencyManager.ToPrice(((CartObject)((RepeaterItem)Container).DataItem).PriceCartTotalWithoutPaymentPrice) %>
				</dd>
				<% } %>
			</dl>
	
			<%-- 隠し更新専用ボタン --%>
			<asp:LinkButton ID="lbPostBack" runat="server" />
			<%-- 隠し値：カートID --%>
			<asp:HiddenField ID="hfCartId" runat="server" Value="<%# this.CartList.Items[((RepeaterItem)Container).ItemIndex].CartId %>" />
			<%-- 隠し再計算ボタン --%>
			<asp:LinkButton id="lbRecalculateCart" runat="server" CommandArgument="<%# ((RepeaterItem)Container).ItemIndex %>" onclick="lbRecalculate_Click"></asp:LinkButton>
		</div>
		</ContentTemplate>
		</asp:UpdatePanel>
		<%-- UPDATE PANELここまで(商品一覧) --%>
			<%-- ▲商品一覧▲ --%>
		</div>

		<%-- 頒布会コース内容確認 --%>
			<% if ((string.IsNullOrEmpty(hfSubscriptionBoxCourseId.Value) == false) && Constants.SUBSCRIPTION_BOX_OPTION_ENABLED) { %>
			<div class="toggle-wrap">
				<input type="checkbox" id="toggle-checkbox">
				<label class="toggle-button" for="toggle-checkbox">コース詳細を確認する</label>
				<div class="toggle-content">
					<asp:Repeater ID="rSubscriptionBoxItem" DataSource="<%# GetRepeaterCount(hfSubscriptionBoxCourseId.Value) %>" runat="server">
						<ItemTemplate>

							<% if (this.SubscriptionBox.IsNumberTime) { %>
							<h3 style="background-color: #cccccc;"><%#: (string)Container.DataItem %>回目配送商品</h3>
							<% }
								else
							 { %>
							<h3 style="background-color: #cccccc;"><%#: (string)Container.DataItem %>にお届けする商品</h3>
							<% } %>

							<div class="productList">
								<div class="background">

									<div class="list" style="background-color: #f1f1f1;">
										<p class="ttl">
											<span style="position: relative; left: 100px">商品名</span>
											<span style="position: relative; left: 230px; width: 30px; display: inline-block;"><span Visible="<%# this.SubscriptionBox.FixedAmountFlg == Constants.FLG_SUBSCRIPTIONBOX_FIXED_AMOUNT_FALSE %>" runat="server">単価</span></span>
											<span style="position: relative; left: 250px">注文数</span>
										</p>

										<asp:Repeater Visible="<%# CheckTakeOverProduct(GetItemsModel((string)Container.DataItem)) == false %>" ID="rProductsList" ItemType="w2.Domain.SubscriptionBox.SubscriptionBoxDefaultItemModel" DataSource='<%# GetItemsModel((string)Container.DataItem) %>' runat="server">
											<ItemTemplate>
												<div class="product" style="background-color: white;" runat="server">
													<table>
														<tr>
															<td class="name" style="width: 200px;">
																<w2c:ProductImage ID="ProductImage2" Visible="<%# string.IsNullOrEmpty(Item.VariationId) == false %>" ImageSize="S" IsVariation='<%# Item.ProductId != Item.VariationId %>' ProductMaster="<%# ProductCommon.GetProductVariationInfo(Item.ShopId, Item.ProductId, Item.VariationId, this.MemberRankId) %>" runat="server" />

																<%#: CreateProductName(Item.Name, Item.VariationName1, Item.VariationName2, Item.VariationName3) %>
							
															</td>
															<td class="price"style="position: relative; left: 60px; width: 60px;">
																<% if (this.SubscriptionBox.FixedAmountFlg == Constants.FLG_SUBSCRIPTIONBOX_FIXED_AMOUNT_FALSE) { %>
																<div ID="dSubscriptionBoxPrice" style="text-decoration: line-through" runat="server" Visible="False"><%#: CurrencyManager.ToPrice(Item.Price) %></div>
																<div ID="dSubscriptionBoxCampaignPrice" runat="server"><asp:Literal ID="lSubscriptionBoxCampaignPrice" runat="server" /></div>
																<% } %>

															</td>

															<td class="quantity" style="position: relative; left: 80px;">
																<%#: Item.ItemQuantity %>
															</td>
														</tr>
													</table>
												</div>
											</ItemTemplate>
										</asp:Repeater>
									</div>
								</div>
								<!--background-->
							</div>
							<!--productList-->
						</ItemTemplate>
					</asp:Repeater>
				</div>
			</div>
			<!-- /.toggle-wrap -->
			<% } %>


	<%-- 各種Js読み込み --%>
	<uc:MailDomains id="MailDomains" runat="server"></uc:MailDomains>
	<%--▼ログイン▼--%>
	<% if ((this.IsLoggedIn == false) && (this.IsAmazonLoggedIn == false) && this.IsVisible_UserPassword && this.DisplayLoginForm && (this.IsConnectedLine == false)) { %>
	<div id="Div2" class="order-unit login" runat="server" visible ="<%# (((RepeaterItem)Container).ItemIndex == 0) %>">
		<h2>ログイン</h2>
		<dl class="order-form">
		<dt><%if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED) { %>メールアドレス<%} else { %>ログインID<%} %></dt>
		<dd>
			<%if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED) { %>
				<w2c:ExtendedTextBox ID="tbLoginIdInMailAddr" Type="email" placeholder="メールアドレス" Runat="server" MaxLength="256" CssClass="mail-domain-suggest"></w2c:ExtendedTextBox>
			<%} else { %>
				<w2c:ExtendedTextBox ID="tbLoginId" placeholder="ログインID" Runat="server" MaxLength="15"></w2c:ExtendedTextBox>
			<%} %>
		</dd>
		<dt>パスワード</dt>
		<dd>
			<w2c:ExtendedTextBox ID="tbPassword" placeholder="パスワード" Runat="server" TextMode="Password" autocomplete="off" MaxLength='<%# GetMaxLength("@@User.password.length_max@@") %>'></w2c:ExtendedTextBox>
		</dd>
		</dl>
		<p id="dLoginErrorMessage" class="attention" runat="server"></p>
		<p class="checked">
			<asp:CheckBox ID="cbAutoCompleteLoginIdFlg" runat="server" Text="次回からの入力を省略" />
		</p>
		<div class="order-footer">
			<div class="button-next">
			<asp:LinkButton ID="lbLogin" runat="server" onclick="lbLogin_Click" OnClientClick="return onClientClickLogin();" CssClass="btn">ログイン</asp:LinkButton>
			</div>
			<p><a href="<%= WebSanitizer.HtmlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_PASSWORD_REMINDER_INPUT + "?" + Constants.REQUEST_KEY_NEXT_URL + "=" + this.LandingCartInputAbsolutePath) %>">&raquo; パスワードを忘れた方はこちら</a></p>
		</div>
	</div>

	<div class="order-unit not-login" runat="server" visible ="<%# (((RepeaterItem)Container).ItemIndex == 0) %>">
		<% if (Constants.SOCIAL_LOGIN_ENABLED || Constants.AMAZON_LOGIN_OPTION_ENABLED || Constants.RAKUTEN_LOGIN_ENABLED) { %>
		<div>
			<h2>SNSログイン</h2>
			<div>
				<ul style="list-style:none;text-align:center;padding:0px;margin:1em 0 0 0;">
				<% if (Constants.SOCIAL_LOGIN_ENABLED) { %>
					<%-- Apple --%>
					<% if (DisplaySocialLoginBtnCheck(LandingPageConst.SocialLoginType.Apple)) { %>
						<li class="social-login-margin">
							<a class="social-login apple-color"
								href="<%: w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
									w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Apple,
									Constants.PAGE_FRONT_SOCIAL_LOGIN_LANDINGCART_CALLBACK,
									Constants.PAGE_FRONT_SOCIAL_LOGIN_LANDINGCART_CALLBACK,
									true,
									Request.Url.Authority,
									string.Format("{0}#CartList", this.LandingCartInputAbsolutePath)) %>">
								<div class="social-icon-width-apple">
									<img class="apple-icon"
										src="<%= Constants.PATH_ROOT %>
										Contents\ImagesPkg\socialLogin\logo_apple.png" />
								</div>
								<div class="apple-label">Appleでサインイン</div>
							</a>
						</li>
					<% } %>
				<%-- Facebook --%>
				<% if (DisplaySocialLoginBtnCheck(LandingPageConst.SocialLoginType.FaceBook)) { %>
					<li class="social-login-margin">
						<a class="social-login facebook-color"
							href="<%: w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
								w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Facebook,
								Constants.PAGE_FRONT_SOCIAL_LOGIN_LANDINGCART_CALLBACK,
								Constants.PAGE_FRONT_SOCIAL_LOGIN_LANDINGCART_CALLBACK,
								true,
								Request.Url.Authority,
									string.Format("{0}#CartList", this.LandingCartInputAbsolutePath)) %>">
							<div class="social-icon-width">
								<img class="facebook-icon"
									src="<%= Constants.PATH_ROOT %>
									Contents\ImagesPkg\socialLogin\logo_facebook.png" />
							</div>
							<div class="social-login-label">Facebookでログイン</div>
						</a>
					</li>
				<% } %>
				<%-- Twitter --%>
				<% if (DisplaySocialLoginBtnCheck(LandingPageConst.SocialLoginType.Twitter)) { %>
						<li class="social-login-margin">
							<a class="social-login twitter-color"
						href="<%: w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
								w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Twitter,
								Constants.PAGE_FRONT_SOCIAL_LOGIN_LANDINGCART_CALLBACK,
								Constants.PAGE_FRONT_SOCIAL_LOGIN_LANDINGCART_CALLBACK,
								true,
								Request.Url.Authority,
										string.Format("{0}#CartList", this.LandingCartInputAbsolutePath)) %>">
								<div class="social-icon-width">
									<img class="twitter-icon"
										src="<%= Constants.PATH_ROOT %>
										Contents\ImagesPkg\socialLogin\logo_x.png" />
								</div>
								<div class="twitter-label">Xでログイン</div>
							</a>
				</li>
				<% } %>
				<%-- Yahoo --%>
				<% if (DisplaySocialLoginBtnCheck(LandingPageConst.SocialLoginType.Yahoo)) { %>
						<li class="social-login-margin">
							<a class="social-login yahoo-color"
						href="<%: w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
								w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Yahoo,
								Constants.PAGE_FRONT_SOCIAL_LOGIN_LANDINGCART_CALLBACK,
								Constants.PAGE_FRONT_SOCIAL_LOGIN_LANDINGCART_CALLBACK,
								true,
								Request.Url.Authority,
										string.Format("{0}#CartList", this.LandingCartInputAbsolutePath)) %>">
								<div class="social-icon-width">
									<img class="yahoo-icon"
										src="<%= Constants.PATH_ROOT %>
										Contents\ImagesPkg\socialLogin\logo_yahoo.png" />
								</div>
								<div class="social-login-label">Yahoo! JAPAN IDでログイン</div>
							</a>
				</li>
				<% } %>
					<%-- Google --%>
				<% if (DisplaySocialLoginBtnCheck(LandingPageConst.SocialLoginType.Gplus)) { %>
					<li class="social-login-margin">
						<a class="social-login google-color"
							href="<%=w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
								w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Gplus,
									Constants.PAGE_FRONT_SOCIAL_LOGIN_LANDINGCART_CALLBACK,
									Constants.PAGE_FRONT_SOCIAL_LOGIN_LANDINGCART_CALLBACK,
								true,
								Request.Url.Authority,
									this.LandingCartInputAbsolutePath) %>">
							<div class="social-icon-width">
								<img class="google-icon"
									src="<%= Constants.PATH_ROOT %>
									Contents\ImagesPkg\socialLogin\logo_google.png" />
							</div>
							<div class="google-label">Sign in with Google</div>
						</a>
				</li>
				<% } %>
				<% } %>
				<%-- LINE --%>
					<% if ((Constants.SOCIAL_LOGIN_ENABLED && DisplaySocialLoginBtnCheck(LandingPageConst.SocialLoginType.Line)) || w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED) { %>
						<li class="social-login-margin">
							<div class="social-login line-color">
								<div class="social-login line-active-color">
								<a href="<%: LineConnect(this.LandingCartInputAbsolutePath,
											Constants.PAGE_FRONT_SOCIAL_LOGIN_LANDINGCART_CALLBACK,
											Constants.PAGE_FRONT_SOCIAL_LOGIN_LANDINGCART_CALLBACK,
											true,
											Request.Url.Authority,
												string.Format("{0}#CartList", this.LandingCartInputAbsolutePath)) %>">
									<div class="social-icon-width">
										<img class="line-icon"
											src="<%= Constants.PATH_ROOT %>
											Contents\ImagesPkg\socialLogin\logo_line.png" />
						</div>
									<div class="social-login-label">LINEでログイン</div>
								</a>
								</div>
							</div>
							<p style="margin-top:3px; text-align: center;">※LINE連携時に友だち追加します</p>
					</li>
				<% } %>
					<%-- 楽天Connect --%>
				<% if (Constants.RAKUTEN_LOGIN_ENABLED) { %>
						<li class="social-login-margin">
						<asp:LinkButton runat="server" ID="lbRakutenIdConnectRequestAuth" OnClick="lbRakutenIdConnectRequestAuth_Click">
								<img src="https://static.id.rakuten.co.jp/static/btn-japanese-2x/idconnect_01-login_r@2x.png"
									style="width: 296px; height: 40px" />
						</asp:LinkButton>
							<p style="margin-top: 3px; text-align: center;">
							楽天会員のお客様は、<br/>
							楽天IDに登録している情報を利用して、<br/>
							「新規会員登録/ログイン」が可能です。
						</p>
					</li>
				<% } %>
				</ul>
			</div>
		</div>
		<% } %>
	</div>
	<%} %>
	<%--▲ログイン▲--%>

	<%-- ▼注文者情報▼ --%>
	<div class="owner" visible='<%# Container.ItemIndex == 0 %>' runat="server">
	<h2 id="orderDetailsInput" class="ttlA">注文者情報</h2>
	<% if ((this.IsAmazonLoggedIn == false) || this.IsLoggedIn || IsTargetToExtendedAmazonAddressManagerOption()) { %>
	<%-- UPDATE PANEL開始(注文者情報) --%>
	<asp:UpdatePanel ID="upOwnerUpdatePanel" runat="server" UpdateMode="Conditional" Visible="<%#((this.IsAmazonLoggedIn == false) || this.IsLoggedIn || IsTargetToExtendedAmazonAddressManagerOption()) %>">
	<ContentTemplate>
	<%
		var ownerAddrCountryIsoCode = GetOwnerAddrCountryIsoCode(0);
		var isOwnerAddrCountryJp = IsCountryJp(ownerAddrCountryIsoCode);
		var isOwnerAddrCountryUs = IsCountryUs(ownerAddrCountryIsoCode);
		var isOwnerAddrCountryTw = IsCountryTw(ownerAddrCountryIsoCode);
		var isOwnerAddrZipNecessary = IsAddrZipcodeNecessary(ownerAddrCountryIsoCode);
		if (isOwnerAddrCountryTw){
			tbOwnerZipGlobal.Text = ddlOwnerAddr3.SelectedValue;
		}
	%>
	<dl class="order-form">
		<%-- 注文者：氏名 --%>
		<dt>
			<%: ReplaceTag("@@User.name.name@@") %>
			<span class="require">※</span><span id="efo_sign_name"/>
		</dt>
		<dd class="name">
			<p class="attention">
			<asp:CustomValidator
				ID="cvOwnerName1"
				runat="Server"
				ControlToValidate="tbOwnerName1"
				ValidationGroup="OrderShipping-OrderPayment"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				EnableClientScript="false" />
			<asp:CustomValidator
				ID="cvOwnerName2"
				runat="Server"
				ControlToValidate="tbOwnerName2"
				ValidationGroup="OrderShipping-OrderPayment"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				EnableClientScript="false" />
			</p>
			<w2c:ExtendedTextBox ID="tbOwnerName1" Text="<%# this.CartList.Owner.Name1 %>" MaxLength='<%# GetMaxLength("@@User.name1.length_max@@") %>' runat="server" placeholder='<%# ReplaceTag("@@User.name1.name@@") %>'></w2c:ExtendedTextBox>
			<w2c:ExtendedTextBox ID="tbOwnerName2" Text="<%# this.CartList.Owner.Name2 %>" MaxLength='<%# GetMaxLength("@@User.name2.length_max@@") %>' runat="server" placeholder='<%# ReplaceTag("@@User.name2.name@@") %>'></w2c:ExtendedTextBox>
		</dd>
		<%-- 注文者：氏名（かな） --%>
		<% if (isOwnerAddrCountryJp) { %>
		<dt>
			<%: ReplaceTag("@@User.name_kana.name@@") %>
			<% if (IsTargetToExtendedAmazonAddressManagerOption() == false) { %><span class="require">※</span><span id="efo_sign_kana"/><% } %>
		</dt>
		<dd class="<%: ReplaceTag("@@User.name_kana.type@@") %>">
			<p class="attention">
			<asp:CustomValidator
				ID="cvOwnerNameKana1"
				runat="Server"
				ControlToValidate="tbOwnerNameKana1"
				ValidationGroup="OrderShipping-OrderPayment"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				EnableClientScript="false" />
			<asp:CustomValidator
				ID="cvOwnerNameKana2"
				runat="Server"
				ControlToValidate="tbOwnerNameKana2"
				ValidationGroup="OrderShipping-OrderPayment"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				EnableClientScript="false" />
			</p>
			<% tbOwnerNameKana1.Attributes["placeholder"] = ReplaceTag("@@User.name_kana1.name@@"); %>
			<% tbOwnerNameKana2.Attributes["placeholder"] = ReplaceTag("@@User.name_kana2.name@@"); %>

			<w2c:ExtendedTextBox ID="tbOwnerNameKana1" Text="<%# this.CartList.Owner.NameKana1 %>" MaxLength='<%# GetMaxLength("@@User.name_kana1.length_max@@") %>' runat="server" placeholder='<%# ReplaceTag("@@User.name_kana1.name@@") %>'></w2c:ExtendedTextBox>
			<w2c:ExtendedTextBox ID="tbOwnerNameKana2" Text="<%# this.CartList.Owner.NameKana2 %>" MaxLength='<%# GetMaxLength("@@User.name_kana2.length_max@@") %>' runat="server" placeholder='<%# ReplaceTag("@@User.name_kana2.name@@") %>'></w2c:ExtendedTextBox>
		</dd>
		<% } %>
		<%-- 注文者：生年月日 --%>
		<dt>
			<%: ReplaceTag("@@User.birth.name@@") %>
			<% if (IsTargetToExtendedAmazonAddressManagerOption() == false) { %><span class="require">※</span><span id="efo_sign_birth"/><% } %>
		</dt>
		<dd class="birth">
			<p class="attention">
			<asp:CustomValidator
				ID="cvOwnerBirth"
				runat="Server"
				ControlToValidate="ddlOwnerBirthDay"
				ValidationGroup="OrderShipping-OrderPayment"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				EnableClientScript="false" />
			</p>
			<asp:DropDownList ID="ddlOwnerBirthYear" DataSource='<%# this.OrderOwnerBirthYear %>' SelectedValue='<%# this.CartList.Owner.Birth.HasValue ? this.CartList.Owner.Birth.Value.Year.ToString() : string.Empty %>' CssClass="year" runat="server"></asp:DropDownList>
			年 
			<asp:DropDownList ID="ddlOwnerBirthMonth" DataSource='<%# this.OrderOwnerBirthMonth %>' SelectedValue='<%# this.CartList.Owner.Birth.HasValue ? this.CartList.Owner.Birth.Value.Month.ToString() : string.Empty %>' CssClass="month" runat="server"></asp:DropDownList>
			月 
			<asp:DropDownList ID="ddlOwnerBirthDay" DataSource='<%# this.OrderOwnerBirthDay %>' SelectedValue='<%# this.CartList.Owner.Birth.HasValue ? this.CartList.Owner.Birth.Value.Day.ToString() : string.Empty %>' CssClass="date" runat="server"></asp:DropDownList>
			日
		</dd>
		<%-- 注文者：性別 --%>
		<dt>
			<%: ReplaceTag("@@User.sex.name@@") %>
			<span class="require">※</span><span id="efo_sign_sex"/>
		</dt>
		<dd class="sex">
			<p class="attention">
			<asp:CustomValidator
				ID="cvOwnerSex"
				runat="Server"
				ControlToValidate="rblOwnerSex"
				ValidationGroup="OrderShipping-OrderPayment"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				EnableClientScript="false" />
			</p>
			<asp:RadioButtonList ID="rblOwnerSex" DataSource='<%# this.OrderOwnerSex %>' SelectedValue='<%# GetCorrectSexForDataBindDefault() %>' DataTextField="Text" DataValueField="Value" RepeatDirection="Horizontal" RepeatLayout="Flow" runat="server" />
		</dd>
		<dt>
			<%: ReplaceTag("@@User.mail_addr.name@@") %>
			<span class="require">※</span><span id="efo_sign_mail_addr"/>
		</dt>
		<dd class="mail">
			<% if (this.DisplayMailAddressConfirmForm){ %>
			<p class="msg">お手数ですが、確認のため２度入力してください。</p>
			<% } %>
			<p class="attention">
			<asp:CustomValidator
				ID="cvOwnerMailAddr"
				runat="Server"
				ControlToValidate="tbOwnerMailAddr"
				ValidationGroup="OrderShipping-OrderPayment"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				EnableClientScript="false" />
			<% if (this.DisplayMailAddressConfirmForm){ %>
			<asp:CustomValidator
				runat="Server"
				ID="cvOwnerMailAddrConf"
				ControlToValidate="tbOwnerMailAddrConf"
				ValidationGroup="OrderShipping-OrderPayment"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				EnableClientScript="false" />
			</p>
			<% } %>
			<w2c:ExtendedTextBox ID="tbOwnerMailAddr" Text="<%# this.CartList.Owner.MailAddr %>" Type="email" MaxLength="256" runat="server" CssClass="mail-domain-suggest"></w2c:ExtendedTextBox>
			<% if (this.DisplayMailAddressConfirmForm){ %>
			<w2c:ExtendedTextBox ID="tbOwnerMailAddrConf" Text="<%# this.CartList.Owner.MailAddr %>" Type="email" MaxLength="256" runat="server" CssClass="mail-domain-suggest"></w2c:ExtendedTextBox>
			<% } %>
		</dd>

		<%-- 注文者：国 --%>
		<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
		<dt>
			<%: ReplaceTag("@@User.country.name@@", ownerAddrCountryIsoCode) %>
			<span class="require">※</span>
		</dt>
		<dd>
			<asp:DropDownList runat="server" ID="ddlOwnerCountry" DataSource="<%# this.UserCountryDisplayList %>" AutoPostBack="true" DataTextField="Text" DataValueField="Value"
				OnSelectedIndexChanged="ddlOwnerCountry_SelectedIndexChanged" SelectedValue="<%# this.CartList.Owner.AddrCountryIsoCode %>"></asp:DropDownList><br/>
			<asp:CustomValidator
				ID="cvOwnerCountry"
				runat="Server"
				ControlToValidate="ddlOwnerCountry"
				ValidationGroup="OrderShipping"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				EnableClientScript="false"
				CssClass="error_inline" />
			<span id="countryAlertMessage" class="notes" runat="server" Visible='false'>※Amazonログイン連携では国はJapan以外選択できません。</span>
		</dd>
		<% } %>
		<%-- 注文者：郵便番号 --%>
		<% if (isOwnerAddrCountryJp) { %>
		<dt>
			<%: ReplaceTag("@@User.zip.name@@") %>
			<span class="require">※</span><span id="efo_sign_zip"/>
		</dt>
		<dd class="zip">
			<p class="attention">
				<asp:CustomValidator
					ID="cvOwnerZip1"
					runat="Server"
					ControlToValidate="tbOwnerZip"
					ValidationGroup="OrderShipping-OrderPayment"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					EnableClientScript="false"
					CssClass="error_inline cvOwnerZipShortInput" />
				<span id="sOwnerZipError" runat="server" class="sOwnerZipError"></span>
			</p>
			<w2c:ExtendedTextBox ID="tbOwnerZip" Text="<%# this.CartList.Owner.Zip %>" OnTextChanged="lbSearchOwnergAddr_Click" Type="tel" MaxLength="8" runat="server" /><br />
			<asp:LinkButton ID="lbSearchOwnergAddr" runat="server" onclick="lbSearchOwnergAddr_Click" CssClass="btn-add-search" OnClientClick="return false;">郵便番号から住所を入力</asp:LinkButton>
			<%--検索結果レイヤー--%>
			<uc:Layer ID="ucLayer" runat="server" />
		</dd>
		<% } %>
		<dt>
			<%: ReplaceTag("@@User.addr.name@@") %>
			<span class="require">※</span><% if (isOwnerAddrCountryJp) { %><span id="efo_sign_address"/><% } %>
		</dt>
		<dd class="address">
			<%-- 注文者：都道府県 --%>
			<p class="attention">
			<asp:CustomValidator
				ID="cvOwnerAddr1"
				runat="Server"
				ControlToValidate="ddlOwnerAddr1"
				ValidationGroup="OrderShipping-OrderPayment"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				EnableClientScript="false" />
			</p>
			<% if (isOwnerAddrCountryJp) { %>
			<asp:DropDownList ID="ddlOwnerAddr1" DataSource="<%# this.Addr1List %>" DataTextField="Text" DataValueField="Value" SelectedValue="<%# this.CartList.Owner.Addr1 %>" runat="server"></asp:DropDownList>
			<% } %>
			<%-- 注文者：市区町村 --%>
			<% if (isOwnerAddrCountryTw) { %>
				<asp:DropDownList runat="server" ID="ddlOwnerAddr2" DataSource="<%# this.UserTwCityList %>" AutoPostBack="true" DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlOwnerAddr2_SelectedIndexChanged"></asp:DropDownList>
			<% } else { %>
				<p class="attention">
					<asp:CustomValidator
						ID="cvOwnerAddr2"
						runat="Server"
						ControlToValidate="tbOwnerAddr2"
						ValidationGroup="OrderShipping-OrderPayment"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						ClientValidationFunction="ClientValidate"
						EnableClientScript="false" />
				</p>
				<w2c:ExtendedTextBox ID="tbOwnerAddr2" placeholder='市区町村' Text="<%# this.CartList.Owner.Addr2 %>"  MaxLength='<%# GetMaxLength("@@User.addr2.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
			<% } %>
			<%-- 注文者：番地 --%>
			<% if (isOwnerAddrCountryTw) { %>
				<asp:DropDownList runat="server" ID="ddlOwnerAddr3" AutoPostBack="true" DataTextField = "Key" DataValueField = "Value" Width="95"><asp:ListItem value="">區域</asp:ListItem></asp:DropDownList>
			<% } else { %>
				<p class="attention">
					<asp:CustomValidator
						ID="cvOwnerAddr3"
						runat="Server"
						ControlToValidate="tbOwnerAddr3"
						ValidationGroup="OrderShipping-OrderPayment"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						ClientValidationFunction="ClientValidate"
						EnableClientScript="false" />
				</p>
				<w2c:ExtendedTextBox ID="tbOwnerAddr3" placeholder='番地' Text="<%# this.CartList.Owner.Addr3 %>"  MaxLength='<%# GetMaxLength("@@User.addr3.length_max@@") %>' runat="server" ></w2c:ExtendedTextBox>
			<% } %>
			<%-- 注文者：ビル・マンション名 --%>
			<p class="attention">
			<asp:CustomValidator
				ID="cvOwnerAddr4"
				runat="Server"
				ControlToValidate="tbOwnerAddr4"
				ValidationGroup="OrderShipping-OrderPayment"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				EnableClientScript="false" />
			</p>
			<w2c:ExtendedTextBox ID="tbOwnerAddr4" placeholder='建物名' Text="<%# this.CartList.Owner.Addr4 %>"  MaxLength='<%# GetMaxLength("@@User.addr4.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
			<%-- 注文者：州 --%>
			<% if (isOwnerAddrCountryJp == false) { %>
				<% if (isOwnerAddrCountryUs) { %>
			<asp:DropDownList runat="server" ID="ddlOwnerAddr5" DataSource="<%# this.UserStateList %>" ></asp:DropDownList>
					<asp:CustomValidator
						ID="cvOwnerAddr5Ddl"
						runat="Server"
						ControlToValidate="ddlOwnerAddr5"
						ValidationGroup="OrderShippingGlobal"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						ClientValidationFunction="ClientValidate"
						EnableClientScript="false"
						CssClass="error_inline" />
			<% } else if (isOwnerAddrCountryTw) { %>
				<p class="attention">
					<asp:CustomValidator
						ID="cvOwnerAddrTw"
						runat="Server"
						ControlToValidate="tbOwnerAddr5"
						ValidationGroup="OrderShippingGlobal-OrderPayment"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						ClientValidationFunction="ClientValidate"
						EnableClientScript="false" />
				</p>
				<w2c:ExtendedTextBox ID="tbOwnerAddrTw" placeholder='省' Text="<%# this.CartList.Owner.Addr5 %>" MaxLength='<%# GetMaxLength("@@User.addr5.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
			<% } else { %>
			<p class="attention">
				<asp:CustomValidator
					ID="cvOwnerAddr5"
					runat="Server"
				ControlToValidate="tbOwnerAddr5"
				ValidationGroup="OrderShippingGlobal-OrderPayment"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				EnableClientScript="false" />
			</p>
			<w2c:ExtendedTextBox ID="tbOwnerAddr5" placeholder='州' Text="<%# this.CartList.Owner.Addr5 %>"  MaxLength='<%# GetMaxLength("@@User.addr5.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
			<% } %>
			<% } %>
		</dd>
		<%-- 注文者：郵便番号（海外向け） --%>
		<% if (isOwnerAddrCountryJp == false) { %>
		<dt>
			<%: ReplaceTag("@@User.zip.name@@", ownerAddrCountryIsoCode) %>
			<% if (isOwnerAddrZipNecessary) { %><span class="require">※</span><% } %>
		</dt>
		<dd>
			<p class="attention">
			<asp:CustomValidator
				ID="cvOwnerZipGlobal"
				runat="Server"
				ControlToValidate="tbOwnerZipGlobal"
				ValidationGroup="OrderShippingGlobal-OrderPayment"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				EnableClientScript="false" />
			</p>
			<w2c:ExtendedTextBox ID="tbOwnerZipGlobal" Type="tel" Text="<%# this.CartList.Owner.Zip %>" MaxLength="20" runat="server"></w2c:ExtendedTextBox>
			<asp:LinkButton
				ID="lbSearchAddrOwnerFromZipGlobal"
				OnClick="lbSearchAddrOwnerFromZipGlobal_Click"
				Style="display:none;"
				Runat="server" />
		</dd>
		<% } %>
		<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
			<%-- 注文者：企業名 --%>
			<dt>
				<%: ReplaceTag("@@User.company_name.name@@")%>
			</dt>
			<dd class="company-name">
				<p class="attention">
				<asp:CustomValidator
				ID="cvOwnerCompanyName"	
				runat="Server"
				ControlToValidate="tbOwnerCompanyName"
				ValidationGroup="OrderShipping-OrderPayment"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				EnableClientScript="false" />
				</p>
				<w2c:ExtendedTextBox ID="tbOwnerCompanyName" placeholder='<%# ReplaceTag("@@User.company_name.name@@") %>' Text="<%# this.CartList.Owner.CompanyName %>"  MaxLength='<%# GetMaxLength("@@User.company_name.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
			</dd>
			<%-- 注文者：部署名 --%>
			<dt>
				<%: ReplaceTag("@@User.company_post_name.name@@")%>
			</dt>
			<dd class="company-post">
				<p class="attention">
				<asp:CustomValidator
				ID="cvOwnerCompanyPostName"	
				runat="Server"
				ControlToValidate="tbOwnerCompanyPostName"
				ValidationGroup="OrderShipping-OrderPayment"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				EnableClientScript="false" />
				</p>
				<w2c:ExtendedTextBox ID="tbOwnerCompanyPostName" placeholder='<%# ReplaceTag("@@User.company_post_name.name@@") %>' Text="<%# this.CartList.Owner.CompanyPostName %>"  MaxLength='<%# GetMaxLength("@@User.company_post_name.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
			</dd>
		<%} %>
	<%-- 注文者：電話番号1 --%>
	<% if (isOwnerAddrCountryJp) { %>
		<dt>
			<%: ReplaceTag("@@User.tel1.name@@") %>
			<span class="require">※</span><span id="efo_sign_tel1"/>
		</dt>
		<dd class="tel">
			<p class="attention">
				<asp:CustomValidator
					ID="cvOwnerTel1_1"
					runat="Server"
					ControlToValidate="tbOwnerTel1"
					ValidationGroup="OrderShipping-OrderPayment"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					EnableClientScript="false" />
			</p>
			<w2c:ExtendedTextBox ID="tbOwnerTel1" MaxLength="13" Text="<%# this.CartList.Owner.Tel1 %>" Type="tel" style="width:100%;" runat="server" CssClass="shortTel" onchange="resetAuthenticationCodeInput('cvOwnerTel1_1')" />
			<% if (this.AuthenticationUsable) { %>
			<asp:LinkButton
				ID="lbGetAuthenticationCode"
				CssClass="btn-add-get"
				runat="server"
				Text="認証コードの取得"
				OnClick="lbGetAuthenticationCode_Click"
				OnClientClick="return checkTelNoInput();" />
			<asp:Label ID="lbAuthenticationStatus" runat="server" />
			<% } %>
		</dd>
		<% if (this.AuthenticationUsable) { %>
		<dt>
			<%: ReplaceTag("@@User.authentication_code.name@@") %>
		</dt>
		<dd>
			<asp:TextBox
				ID="tbAuthenticationCode"
				Width="30%"
				Enabled="false"
				MaxLength='<%# GetMaxLength("@@User.authentication_code.length_max@@") %>'
				runat="server" />
			<span class="notes">
				<% if (this.HasAuthenticationCode) { %>
				<asp:Label ID="lbHasAuthentication" CssClass="authentication_success" runat="server"><%: ReplaceTag("@@User.authenticated.name@@") %></asp:Label>
				<% } %>
				<span><%: GetVerificationCodeNote(ownerAddrCountryIsoCode) %></span>
				<asp:Label ID="lbAuthenticationMessage" runat="server" />
			</span>
			<asp:CustomValidator
				ID="cvAuthenticationCode"
				runat="Server"
				ControlToValidate="tbAuthenticationCode"
				ValidationGroup="OrderShipping-OrderPayment"
				ValidateEmptyText="false"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				CssClass="error_inline" />
		</dd>
		<% } %>
		<%-- 注文者：電話番号2 --%>
		<dt><%: ReplaceTag("@@User.tel2.name@@") %></dt>
		<dd class="tel">
			<p class="attention">
				<asp:CustomValidator
					ID="cvOwnerTel2_1"
					runat="Server"
					ControlToValidate="tbOwnerTel2"
					ValidationGroup="OrderShipping-OrderPayment"
					ValidateEmptyText="false"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					EnableClientScript="false"/>
			</p>
			<w2c:ExtendedTextBox ID="tbOwnerTel2" MaxLength="13" Text="<%# this.CartList.Owner.Tel2 %>" Type="tel" style="width:100%;" runat="server" CssClass="shortTel" ></w2c:ExtendedTextBox>
		</dd>
		<% } else { %>
		<%-- 注文者：電話番号1（海外向け） --%>
		<dt>
			<%: ReplaceTag("@@User.tel1.name@@", ownerAddrCountryIsoCode) %>
			&nbsp;<span class="fred">※</span>
		</dt>
		<dd class="tel">
			<p class="attention">
			<asp:CustomValidator
				ID="cvOwnerTel1Global"
				runat="Server"
				ControlToValidate="tbOwnerTel1Global"
				ValidationGroup="OrderShippingGlobal-OrderPayment"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				EnableClientScript="false" />
			</p>
			<w2c:ExtendedTextBox ID="tbOwnerTel1Global" Text="<%# this.CartList.Owner.Tel1 %>" Width="100%" Type="tel" MaxLength="30" runat="server" onchange="resetAuthenticationCodeInput('cvOwnerTel1Global')" />
			<% if (this.AuthenticationUsable) { %>
			<asp:LinkButton
				ID="lbGetAuthenticationCodeGlobal"
				CssClass="btn-add-get"
				runat="server"
				Text="認証コードの取得"
				OnClick="lbGetAuthenticationCode_Click"
				OnClientClick="return checkTelNoInput();" />
			<asp:Label ID="lbAuthenticationStatusGlobal" runat="server" />
			<% } %>
		</dd>
		<% if (this.AuthenticationUsable) { %>
		<dt>
			<%: ReplaceTag("@@User.authentication_code.name@@") %>
		</dt>
		<dd>
			<asp:TextBox
				ID="tbAuthenticationCodeGlobal"
				Width="30%"
				Enabled="false"
				MaxLength='<%# GetMaxLength("@@User.authentication_code.length_max@@") %>'
				runat="server" />
			<span class="notes">
			<% if (this.HasAuthenticationCode) { %>
				<asp:Label ID="lbHasAuthenticationGlobal" CssClass="authentication_success" runat="server"><%: ReplaceTag("@@User.authenticated.name@@") %></asp:Label>
			<% } %>
				<span><%: GetVerificationCodeNote(ownerAddrCountryIsoCode) %></span>
				<asp:Label ID="lbAuthenticationMessageGlobal" runat="server" />
			</span>
			<asp:CustomValidator
				ID="cvAuthenticationCodeGlobal"
				runat="Server"
				ControlToValidate="tbAuthenticationCodeGlobal"
				ValidationGroup="OrderShippingGlobal-OrderPayment"
				ValidateEmptyText="false"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				CssClass="error_inline" />
		</dd>
		<% } %>
		<%-- 注文者：電話番号2（海外向け） --%>
		<dt>
			<%: ReplaceTag("@@User.tel2.name@@", ownerAddrCountryIsoCode) %>
			&nbsp;<span class="fred"></span>
		</dt>
		<dd class="tel">
			<p class="attention">
			<asp:CustomValidator
				ID="cvOwnerTel2Global"
				runat="Server"
				ControlToValidate="tbOwnerTel2Global"
				ValidationGroup="OrderShippingGlobal-OrderPayment"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				EnableClientScript="false" />
			</p>
			<w2c:ExtendedTextBox ID="tbOwnerTel2Global" Text="<%# this.CartList.Owner.Tel2 %>" Type="tel" MaxLength="30" runat="server"></w2c:ExtendedTextBox>
		</dd>
		<% } %>
		<dt>
			<%: ReplaceTag("@@User.mail_flg.name@@") %>
		</dt>
		<dd>
			<asp:CheckBox ID="cbOwnerMailFlg" Checked="<%# this.CartList.Owner.MailFlg %>" Text="登録する" CssClass="checkBox" runat="server" />
		</dd>
		<% if(this.UserRegisterDefaultChecked || this.UserRegisterUsable) { %>
		<% if (this.IsLoggedIn == false) { %>
		<dt>
			会員登録
		</dt>
		<dd>
			<asp:CheckBox
				ID="cbUserRegister"
				Checked="<%# this.UserRegisterDefaultChecked %>"
				Enabled="<%# this.UserRegisterUsable %>"
				Text='<%#: IsTargetToExtendedAmazonAddressManagerOption() ? " 注文者情報で会員登録をする " : " Amazonお届け先住所で会員登録する " %>'
				OnCheckedChanged="cbUserRegister_OnCheckedChanged"
				CssClass="checkBox"
				runat="server"
				AutoPostBack="true" />
		</dd>
		<% } %>
		<div id="dvUserPassword" runat="server" visible="<%# this.IsLoggedIn == false %>">
		<%-- ログイン情報 --%>
		<% if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED == false) { %>
			<dt>
				<%: ReplaceTag("@@User.login_id.name@@") %><span class="require">※</span>
			</dt>
			<dd>
				<w2c:ExtendedTextBox id="tbUserLoginId" Type="email" MaxLength="15" Runat="server" placeholder="ログインID"></w2c:ExtendedTextBox>
				<asp:CustomValidator ID="cvUserLoginId" runat="Server"
					ControlToValidate="tbUserLoginId"
					ValidationGroup="OrderShipping-OrderPayment"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					EnableClientScript="false"
					CssClass="error_inline" />
			</dd>
		<%-- ソーシャルログイン連携されている場合はパスワードスキップ --%>
		<% } %>
		<% if (this.IsVisible_UserPassword && (IsTargetToExtendedAmazonAddressManagerOption() == false)) { %>
			<dt>
				<%: ReplaceTag("@@User.password.name@@") %><span class="require">※</span>
			</dt>
			<dd>
				<asp:TextBox id="tbUserPassword" TextMode="Password" autocomplete="off" CssClass="input_border" Runat="server" MaxLength='<%# GetMaxLength("@@User.password.length_max@@") %>'></asp:TextBox>
				<p class="notes" style="display:none;">※半角英数字混合7文字〜15文字以内でお願いいたします</p>
				<asp:CustomValidator ID="cvUserPassword" runat="Server"
					ControlToValidate="tbUserPassword"
					ValidationGroup="OrderShipping-OrderPayment"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					EnableClientScript="false"
					CssClass="error_inline" />
				<br />
				<asp:TextBox id="tbUserPasswordConf" TextMode="Password" autocomplete="off" CssClass="input_border" Runat="server" MaxLength='<%# GetMaxLength("@@User.password.length_max@@") %>'></asp:TextBox>
				<p class="notes">※お手数ですが、確認のため２度入力してください</p>
				<asp:CustomValidator ID="cvUserPasswordConf" runat="Server"
					ControlToValidate="tbUserPasswordConf"
					ValidationGroup="OrderShipping-OrderPayment"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					EnableClientScript="false"
					CssClass="error_inline" />
			</dd>
		<% } %>
			<%-- ユーザー拡張項目　HasInput:true(入力画面)/false(確認画面)　HasRegist:true(新規登録)/false(登録編集) --%>
			<uc:BodyUserExtendLandingPageRegist ID="ucBodyUserExtendLandingPageRegist" runat="server" HasInput="true" HasRegist="true" IsLandingPage="true" Visible="<%# IsTargetToExtendedAmazonAddressManagerOption() == false %>" />
		<div class="order-unit">
			<%-- メッセージ --%>
			<h3>会員規約について</h3>
			<div class="dvContentsInfo">
				<p>「<%: ShopMessage.GetMessage("ShopName") %>」入会お申込の前に、以下の会員規約・利用規約を必ずお読み下さい。
				</p>
			</div>
		
			<div class="regulation">
				<uc:UserRegistRegulationMessage runat="server" />
			</div>
		</div>
		</div>
	<% } %>
	</dl>
	<% if (this.AuthenticationUsable) { %>
	<asp:LinkButton ID="lbCheckAuthenticationCode" OnClick="lbCheckAuthenticationCode_Click" style="display: none" runat="server" />
	<asp:HiddenField ID="hfResetAuthenticationCode" runat="server" />
	<% } %>
	</ContentTemplate>
	<Triggers>
		<asp:AsyncPostBackTrigger ControlID="lbOpenEcPay" EventName="Click" />
	</Triggers>
	</asp:UpdatePanel>
	<%-- UPDATE PANELここまで(注文者情報) --%>
	<% } else { %>
	<div class="column">
		<%--▼▼ Amazon Pay(CV2)注文者情報 ▼▼--%>
		<% if ((this.AmazonPaySessionOwnerAddress != null) && Constants.AMAZON_PAYMENT_CV2_ENABLED) { %>

		<div style="width: 340px;">
		<div class="userBox">
		<div class="top">
		<div class="bottom">
			<dl>
			<%-- 氏名 --%>
			<dt>
				<%: ReplaceTag("@@User.name.name@@") %>：
			</dt>
			<dd>
				<% if (Constants.AMAZON_PAYMENT_CV2_USE_SHIPPING_AS_OWNER_ENABLED) { %>
					<p><%: this.AmazonPaySessionShippingAddress.Name %></p>
					<p><%: this.AmazonPaySessionShippingAddress.NameKana %></p>
				<% } else { %>
					<p><%: this.AmazonPaySessionOwnerAddress.Name %></p>
					<p><%: this.AmazonPaySessionOwnerAddress.NameKana %></p>
				<% } %>
			</dd>
			<%-- メールアドレス --%>
			<dt>
				<%: ReplaceTag("@@User.mail_addr.name@@") %>：
			</dt>
			<dd>
				<p><%: this.AmazonPaySessionOwnerAddress.MailAddr %></p>
			</dd>
			<dt>
				<%: ReplaceTag("@@User.mail_flg.name@@") %>
			</dt>
			<dd>
				<asp:CheckBox ID="cbGuestOwnerMailFlg2" Checked="<%# this.CartList.Owner.MailFlg %>" Text="登録する" CssClass="checkBox" runat="server" />
			</dd>
			</dl>
			<dl>
		</div><!--bottom-->
		</div><!--top-->
		</div><!--userBox-->
		</div>
		<% } else { %>
		<%--▲▲ Amazon Pay(CV2)注文者情報 ▲▲--%>
		<div id="ownerAddressBookContainer">
			<%-- ▼▼Amazonアドレス帳ウィジェット(注文者情報)▼▼ --%>
			<div id="ownerAddressBookWidgetDiv"></div>
			<div id="ownerAddressBookErrorMessage" style="color:red;padding:5px;" ClientIDMode="Static" runat="server"></div>
			<%-- ▲▲Amazonアドレス帳ウィジェット(注文者情報)▲▲ --%>
		</div>
		<div class="checkBox">
			<asp:CheckBox ID="cbGuestOwnerMailFlg" Text="お知らせメールの配信を希望する " Checked="<%# this.CartList.Owner.MailFlg %>" CssClass="checkBox" runat="server" />
		</div>
		<% } %>
		
		<%-- ▼Amazon Pay会員登録▼ --%>
		<% if ((this.IsAmazonLoggedIn && (this.IsUserRegistedForAmazon == false) && (this.ExistsUserWithSameAmazonEmailAddress == false)) && (this.UserRegisterDefaultChecked || this.UserRegisterUsable)) { %>
			<asp:UpdatePanel ID="upAmazonPayRegisterUpdatePanel" runat="server" UpdateMode="Conditional" Visible="<%# this.IsAmazonLoggedIn && (this.IsUserRegistedForAmazon == false) && (this.ExistsUserWithSameAmazonEmailAddress == false) %>">
				<ContentTemplate>
					<dd>
						<asp:CheckBox ID="cbUserRegisterForExternalSettlement" Checked="true" Text=" Amazonお届け先住所で会員登録する " OnCheckedChanged="cbUserRegisterForExternalSettlement_OnCheckedChanged" CssClass="checkBox" runat="server" AutoPostBack="true" />
					</dd>
					<div id="dvUserBoxVisible" runat="server">
						<div class="order-unit">
							<%-- メッセージ --%>
							<h3>会員規約について</h3>
							<div class="dvContentsInfo">
								<p>「<%: ShopMessage.GetMessage("ShopName") %>」入会お申込の前に、以下の会員規約・利用規約を必ずお読み下さい。
								</p>
							</div>
							<div class="regulation">
								<uc:UserRegistRegulationMessage ID="UserRegistRegulationMessage" runat="server" />
							</div>
						</div>
					</div>
				</ContentTemplate>
			</asp:UpdatePanel>
		<% } %>
		<%-- ▲Amazon Pay会員登録▲ --%>

	</div>
	<% } %>
	</div>
	<%-- ▲注文者情報▲ --%>

	<%-- ▼配送先情報▼ --%>
	<div class="shipping">
	<h2>配送先情報</h2>
	<h3>カート番号<%# Container.ItemIndex + 1 %></h3>
		<asp:HiddenField id="hcShowShippingInputForm" value="<%# CanInputShippingTo(Container.ItemIndex) %>" runat="server" />
		<div id="divShipToCart1Address" Visible="<%# CanInputShippingTo(Container.ItemIndex) && (Container.ItemIndex != 0) %>" runat="server" class="shipping-select">
			<asp:CheckBox id="cbShipToCart1Address" Text="カート番号1の配送先を使用" OnCheckedChanged="cbShipToCart1Address_OnCheckedChanged" AutoPostBack="true" Checked="<%# ((CartObject)((RepeaterItem)Container).DataItem).Shippings[0].IsSameShippingAsCart1 %>" runat="server" />
		</div>
		<% if (this.IsAmazonLoggedIn == false) { %>
		<%-- UPDATE PANEL開始(配送先情報) --%>
		<asp:UpdatePanel ID="upShippingUpdatePanel" runat="server" Visible="<%#(this.IsAmazonLoggedIn == false) %>">
		<ContentTemplate>
		<dl class="order-form">
			<dt>配送先</dt>
			<dd id="divShippingInputForm" runat="server">
				<asp:DropDownList ID="ddlShippingKbnList" DataSource="<%# GetShippingKbnList(((RepeaterItem)Container).ItemIndex) %>" DataTextField="text" DataValueField="value" SelectedValue="<%# ((CartObject)((RepeaterItem)Container).DataItem).Shippings[0].ShippingAddrKbn %>" OnSelectedIndexChanged="ddlShippingKbnList_OnSelectedIndexChanged" AutoPostBack="true" runat="server"></asp:DropDownList>
				<span style="color:red;display:block;"><asp:Literal ID="lStorePickUpInvalidMessage" runat="server" /></span><br />
				<span style="color:red;display:block;"><asp:Literal ID="lShippingCountryErrorMessage" runat="server"></asp:Literal></span>
				<span id='<%# "spErrorConvenienceStore" + ((RepeaterItem)Container).ItemIndex.ToString() %>' style="color:red;display:block;"></span>
				<%-- ▽コンビニ受取り▽ --%>
				<div id="divShippingInputFormConvenience" class="<%# ((RepeaterItem)Container).ItemIndex %>" runat="server">
					<ul>
						<br />
						<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
						<dd runat="server" id="ddShippingReceivingStoreType">
							<asp:DropDownList
								ID="ddlShippingReceivingStoreType"
								DataSource="<%# ShippingReceivingStoreType() %>"
								DataTextField="text"
								DataValueField="value"
								DataMember="<%# ((RepeaterItem)Container).ItemIndex %>"
								OnSelectedIndexChanged="ddlShippingReceivingStoreType_SelectedIndexChanged"
								AutoPostBack="true"
								runat="server" />
						</dd>
						<% } %>
						<span style="color: red; display: block;"><asp:Literal ID="lConvenienceStoreErrorMessage" runat="server"></asp:Literal></span><br />
						購入金額<%# CurrencyManager.ToPrice(Constants.RECEIVINGSTORE_TWPELICAN_CVSLIMITPRICE)%>以上、または<%# this.ConvenienceStoreLimitKg %>kg以上の商品は指定しないでください</br>
						<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
						コンビニがセブンイレブンの場合<%# this.ConvenienceStoreLimitKg7Eleven %>kg以上です。
						<% } %>
						</br>
						<div id="divButtonOpenConvenienceStoreMapPopup" runat="server">
							<li class="convenience-store-item" runat="server" visible='<%# (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false) %>'>
								<a href="javascript:openConvenienceStoreMapPopup(<%# ((RepeaterItem)Container).ItemIndex %>);" class="btn btn-success convenience-store-button">Family/OK/Hi-Life</a>
							</li>
							<li class="convenience-store-item" runat="server" visible='<%# Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED %>'>
								<asp:LinkButton
									ID="lbOpenEcPay"
									runat="server"
									class="btn btn-success convenience-store-button"
									OnClick="lbOpenEcPay_Click"
									CommandArgument="<%# ((RepeaterItem)Container).ItemIndex %>"
									Text="  電子マップ  " />
							</li>
						</div>
						<dl id="ddCvsShopId">
						<li class="convenience-store-item" id="liCvsShopId">
							<strong><%: ReplaceTag("@@DispText.shipping_convenience_store.shopId@@") %></strong><br />
							<span style="font-weight:normal;">
								<asp:Literal ID="lCvsShopId" runat="server" Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID) %>"></asp:Literal>
							</span>
							<asp:HiddenField ID="hfCvsShopId" runat="server" Value="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID) %>" />
						</li>
						</dl>
						<dl id="ddCvsShopName">
						<li class="convenience-store-item" id="liCvsShopName">
							<strong><%: ReplaceTag("@@DispText.shipping_convenience_store.shopName@@") %></strong><br />
							<span style="font-weight:normal;">
								<asp:Literal ID="lCvsShopName" runat="server" Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1) %>"></asp:Literal>
							</span>
							<asp:HiddenField ID="hfCvsShopName" runat="server" Value="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1) %>" />
						</li>
						</dl>
						<dl id="ddCvsShopAddress">
						<li class="convenience-store-item" id="liCvsShopAddress">
							<strong><%: ReplaceTag("@@DispText.shipping_convenience_store.shopAddress@@") %></strong><br />
							<span style="font-weight:normal;">
								<asp:Literal ID="lCvsShopAddress" runat="server" Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4) %>"></asp:Literal>
							</span>
							<asp:HiddenField ID="hfCvsShopAddress" runat="server" Value="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4) %>" />
						</li>
						</dl>
						<dl id="ddCvsShopTel">
						<li class="convenience-store-item" id="liCvsShopTel">
							<strong><%: ReplaceTag("@@DispText.shipping_convenience_store.shopTel@@") %></strong><br />
							<span style="font-weight:normal;">
								<asp:Literal ID="lCvsShopTel" runat="server" Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %>"></asp:Literal>
							</span>
							<asp:HiddenField ID="hfCvsShopTel" runat="server" Value="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %>" />
						</li>
						</dl>
						<dt>＜コンビニ受取の際の注意事項＞</br>
							注文者情報は必ず正しい「<%: ReplaceTag("@@DispText.shipping_convenience_store.Name@@") %>」と「<%: ReplaceTag("@@DispText.shipping_convenience_store.Tel@@") %>」を入力してください。（ショートメールが受け取れる電話番号を入力する必要があります）
							コンビニで商品を受け取る際、店舗ではお客様の「<%: ReplaceTag("@@DispText.shipping_convenience_store.Name@@") %>と「<%: ReplaceTag("@@DispText.shipping_convenience_store.Tel@@") %>」下3桁を確認します。
						</dt></br>
						<asp:HiddenField ID="hfCvsShopFlg" runat="server" Value="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG) %>" />
					</ul>
				</div>
				<%-- △コンビニ受取り△ --%>

				<%-- ▽配送先表示▽ --%>
					<div id="divShippingDisp" visible="<%# GetShipToOwner(((CartObject)((RepeaterItem)Container).DataItem).Shippings[0]) %>" runat="server">
						<% var isShippingAddrCountryJp = IsCountryJp(this.CountryIsoCode); %>
					<dl class="order-form">
					<%-- 氏名 --%>
					<dt><%: ReplaceTag("@@User.name.name@@") %></dt>
					<dd>
						<asp:Literal ID="lShippingName1" runat="server"></asp:Literal><asp:Literal ID="lShippingName2" runat="server"></asp:Literal>&nbsp;様　
						<% if (isShippingAddrCountryJp) { %>（<asp:Literal ID="lShippingNameKana1" runat="server"></asp:Literal><asp:Literal ID="lShippingNameKana2" runat="server"></asp:Literal>&nbsp;さま）<% } %><br />
					</dd>
					<dt>
						<%: ReplaceTag("@@User.addr.name@@") %>
					</dt>
					<dd>
						<%if (isShippingAddrCountryJp) {%>〒<asp:Literal ID="lShippingZip" runat="server"></asp:Literal><br /><%} %>
						<asp:Literal ID="lShippingAddr1" runat="server"></asp:Literal> <asp:Literal ID="lShippingAddr2" runat="server"></asp:Literal><br />
						<asp:Literal ID="lShippingAddr3" runat="server"></asp:Literal> <asp:Literal ID="lShippingAddr4" runat="server"></asp:Literal> 
						<asp:Literal ID="lShippingAddr5" runat="server"></asp:Literal><br />
						<%if (isShippingAddrCountryJp == false ) {%><asp:Literal ID="lShippingZipGlobal" runat="server"></asp:Literal><br /><%} %>
						<asp:Literal ID="lShippingCountryName" runat="server"></asp:Literal><br/>
					</dd>
					<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
					<dt><%: ReplaceTag("@@User.company_name.name@@")%>・<%: ReplaceTag("@@User.company_post_name.name@@")%></dt>
					<dd>
						<asp:Literal ID="lShippingCompanyName" runat="server"></asp:Literal>&nbsp<asp:Literal ID="lShippingCompanyPostName" runat="server"></asp:Literal>
					</dd>
					<%} %>
					<%-- 電話番号 --%>
					<dt><%: ReplaceTag("@@User.tel1.name@@") %></dt>
					<dd>
						<asp:Literal ID="lShippingTel1" runat="server"></asp:Literal>
					</dd>
					</dl>
				</div>
				<%-- △配送先表示△ --%>
			
				<% this.CartItemIndexTmp++; %>

				<%-- ▽配送先入力フォーム▽ --%>
					<div id="divShippingInputFormInner" class="shipping-input" visible="<%# GetShipToOwner(((CartObject)((RepeaterItem)Container).DataItem).Shippings[0]) == false %>" runat="server">
					<div id="divShippingVisibleConvenienceStore" class="<%# ((RepeaterItem)Container).ItemIndex %>" runat="server">
				<%
					var shippingAddrCountryIsoCode = GetShippingAddrCountryIsoCode(this.CartItemIndexTmp);
					var isShippingAddrCountryJp = IsCountryJp(shippingAddrCountryIsoCode);
					var isShippingAddrCountryUs = IsCountryUs(shippingAddrCountryIsoCode);
					var isShippingAddrCountryTw = IsCountryTw(shippingAddrCountryIsoCode);
					var isShippingAddrZipNecessary = IsAddrZipcodeNecessary(shippingAddrCountryIsoCode);
				%>
				<dl class="order-form">
					<%-- 氏名 --%>
					<dt>
						<%: ReplaceTag("@@User.name.name@@") %>
						<span class="require">※</span><span id="<%# "efo_sign_ship_name" + ((RepeaterItem)Container).ItemIndex %>"/>
					</dt>
					<dd class="name">
						<p class="attention">
						<asp:CustomValidator
							ID="cvShippingName1"
							runat="Server"
							ControlToValidate="tbShippingName1"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
						<asp:CustomValidator
							ID="cvShippingName2"
							runat="Server"
							ControlToValidate="tbShippingName2"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
						</p>
							<w2c:ExtendedTextBox ID="tbShippingName1" placeholder='<%# ReplaceTag("@@User.name1.name@@") %>' Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1) %>" MaxLength='<%# GetMaxLength("@@User.name1.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
							<w2c:ExtendedTextBox ID="tbShippingName2" placeholder='<%# ReplaceTag("@@User.name2.name@@") %>' Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2) %>" MaxLength='<%# GetMaxLength("@@User.name2.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
					</dd>
					<%-- 氏名（かな） --%>
					<% if (isShippingAddrCountryJp) { %>
					<dt>
						<%: ReplaceTag("@@User.name_kana.name@@") %>
						<span class="require">※</span><span id="<%# "efo_sign_ship_kana" + ((RepeaterItem)Container).ItemIndex %>"/>
					</dt>
					<dd class="name-kana">
						<p class="attention">
						<asp:CustomValidator
							ID="cvShippingNameKana1"
							runat="Server"
							ControlToValidate="tbShippingNameKana1"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ValidationGroup="OrderShipping" />
						<asp:CustomValidator
							ID="cvShippingNameKana2"
							runat="Server"
							ControlToValidate="tbShippingNameKana2"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
						</p>
							<w2c:ExtendedTextBox ID="tbShippingNameKana1" placeholder='<%# ReplaceTag("@@User.name_kana1.name@@") %>' Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1) %>" MaxLength='<%# GetMaxLength("@@User.name_kana1.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
							<w2c:ExtendedTextBox ID="tbShippingNameKana2" placeholder='<%# ReplaceTag("@@User.name_kana2.name@@") %>'  Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2) %>" MaxLength='<%# GetMaxLength("@@User.name_kana2.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
					</dd>
					<% } %>
					<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
					<dt>
						<%: ReplaceTag("@@User.country.name@@", shippingAddrCountryIsoCode) %>
						&nbsp;<span class="require">※</span>
					</dt>
					<dd>
						<asp:DropDownList ID="ddlShippingCountry" runat="server" DataSource="<%# this.ShippingAvailableCountryDisplayList %>" AutoPostBack="true" DataTextField="Text" DataValueField="Value"
							 OnSelectedIndexChanged="ddlShippingCountry_SelectedIndexChanged"></asp:DropDownList>
						<asp:CustomValidator
							ID="cvShippingCountry"
							runat="Server"
							ControlToValidate="ddlShippingCountry"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							EnableClientScript="false"
							CssClass="error_inline" />
					</dd>
					<% } %>
				<%-- 郵便番号 --%>
				<% if (isShippingAddrCountryJp) { %>
					<dt>
						<%: ReplaceTag("@@User.zip.name@@") %>
						<span class="require">※</span><span id="<%# "efo_sign_ship_zip" + ((RepeaterItem)Container).ItemIndex %>"/>
					</dt>
					<dd class="zip">
						<p class="attention">
							<asp:CustomValidator
								ID="cvShippingZip1"
								runat="Server"
								ControlToValidate="tbShippingZip"
								ValidationGroup="OrderShipping"
								ValidateEmptyText="true"
								SetFocusOnError="true"
								ClientValidationFunction="ClientValidate"
								EnableClientScript="false" />
							<span id="sShippingZipError" class="attention shortZipInputErrorMessage" runat="server"></span>
						</p>
						<w2c:ExtendedTextBox ID="tbShippingZip" MaxLength="8" OnTextChanged="lbSearchShippingAddr_Click" Type="tel" Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP) %>" runat="server"></w2c:ExtendedTextBox>
						<br />
						<asp:LinkButton ID="lbSearchShippingAddr" runat="server" OnClick="lbSearchShippingAddr_Click" CssClass="btn-add-search" OnClientClick="return false;">
						郵便番号から住所を入力</asp:LinkButton>
					</dd>
					<% } %>
					<dt>
						<%: ReplaceTag("@@User.addr.name@@") %>
						<% if (isShippingAddrCountryJp) { %><span id="<%# "efo_sign_ship_address" + ((RepeaterItem)Container).ItemIndex %>"></span><% } %>
					</dt>
					<dd class="address">
						<% if (isShippingAddrCountryJp) { %>
						<p class="attention">
						<asp:CustomValidator
							ID="cvShippingAddr1"
							runat="Server"
							ControlToValidate="ddlShippingAddr1"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
						</p>
						<asp:DropDownList ID="ddlShippingAddr1" DataSource="<%# this.Addr1List %>" DataTextField="Text" DataValueField="Value" SelectedValue="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_ADDR1) %>" runat="server"></asp:DropDownList>
						<% } %>
						<%-- 市区町村 --%>
						<% if (isShippingAddrCountryTw) { %>
							<asp:DropDownList runat="server" ID="ddlShippingAddr2" DataSource="<%# this.UserTwCityList %>" AutoPostBack="true" DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlShippingAddr2_SelectedIndexChanged"></asp:DropDownList>
						<% } else { %>
							<p class="attention">
							<asp:CustomValidator
								ID="cvShippingAddr2"
								runat="Server"
								ControlToValidate="tbShippingAddr2"
								ValidationGroup="OrderShipping"
								ValidateEmptyText="true"
								SetFocusOnError="true" />
							</p>
								<w2c:ExtendedTextBox ID="tbShippingAddr2" placeholder='市区町村' Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_ADDR2) %>" MaxLength='<%# GetMaxLength("@@User.addr2.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
						<% } %>
						<%-- 番地 --%>
						<% if (isShippingAddrCountryTw) { %>
							<asp:DropDownList runat="server" ID="ddlShippingAddr3" DataTextField = "Key" DataValueField = "Value" Width="64" ><asp:ListItem value="">區域</asp:ListItem></asp:DropDownList>
						<% } else { %>
							<p class="attention">
							<asp:CustomValidator
								ID="cvShippingAddr3"
								runat="Server"
								ControlToValidate="tbShippingAddr3"
								ValidationGroup="OrderShipping"
								ValidateEmptyText="true"
								SetFocusOnError="true" />
							</p>
								<w2c:ExtendedTextBox ID="tbShippingAddr3" placeholder='番地' Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_ADDR3) %>" MaxLength='<%# GetMaxLength("@@User.addr3.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
						<% } %>
						<%-- ビル・マンション名 --%>
						<p class="attention">
						<asp:CustomValidator
							ID="cvShippingAddr4"
							runat="Server"
							ControlToValidate="tbShippingAddr4"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
						</p>
						<w2c:ExtendedTextBox ID="tbShippingAddr4" placeholder='建物名' Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_ADDR4) %>" MaxLength='<%# GetMaxLength("@@User.addr4.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>

						<% if (isShippingAddrCountryJp == false) { %>
							<% if (isShippingAddrCountryUs) { %>
						<asp:DropDownList runat="server" ID="ddlShippingAddr5" DataSource="<%# this.UserStateList %>" 
							SelectedValue="<%# Constants.GLOBAL_OPTION_ENABLE ? GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, CartShipping.FIELD_ORDERSHIPPING_SHIPPING_ADDR5_US) : null %>"></asp:DropDownList>
								<asp:CustomValidator
									ID="cvShippingAddr5Ddl"
									runat="Server"
									ControlToValidate="ddlShippingAddr5"
									ValidationGroup="OrderShippingGlobal"
									ValidateEmptyText="true"
									SetFocusOnError="true"
									ClientValidationFunction="ClientValidate"
									EnableClientScript="false"
									CssClass="error_inline" />
						<% } else if (isShippingAddrCountryTw) { %>
						<p class="attention">
							<asp:CustomValidator
								ID="cvShippingAddrTw"
								runat="Server"
								ControlToValidate="tbShippingAddr5"
								ValidationGroup="OrderShippingGlobal"
								ValidateEmptyText="true"
								SetFocusOnError="true" />
						</p>
						<w2c:ExtendedTextBox ID="tbShippingAddrTw" placeholder='省' Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5) %>" MaxLength='<%# GetMaxLength("@@User.addr5.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
						<% } else {%>
						<p class="attention">
							<asp:CustomValidator
								ID="cvShippingAddr5"
								runat="Server"
							ControlToValidate="tbShippingAddr5"
							ValidationGroup="OrderShippingGlobal"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
						</p>
						<w2c:ExtendedTextBox ID="tbShippingAddr5" placeholder='州' Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5) %>" MaxLength='<%# GetMaxLength("@@User.addr5.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
						<% } %>
						<% } %>
					</dd>
					<%-- 郵便番号（海外向け） --%>
					<% if (isShippingAddrCountryJp == false) { %>
					<dt>
						<%: ReplaceTag("@@User.zip.name@@", shippingAddrCountryIsoCode) %>
						<% if (isShippingAddrZipNecessary) { %>&nbsp;<span class="require">※</span><% } %>
					</dt>
					<dd>
						<p class="attention">
						<asp:CustomValidator
							ID="cvShippingZipGlobal"
							runat="Server"
							ControlToValidate="tbShippingZipGlobal"
							ValidationGroup="OrderShippingGlobal"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
						</p>
						<w2c:ExtendedTextBox ID="tbShippingZipGlobal" Type="tel" Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP) %>" MaxLength="20" runat="server"></w2c:ExtendedTextBox>
						<asp:LinkButton
							ID="lbSearchAddrShippingFromZipGlobal"
							OnClick="lbSearchAddrShippingFromZipGlobal_Click"
							Style="display:none;"
							runat="server" />
					</dd>
					<% } %>
					<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
					<%-- 企業名 --%>
					<dt><%: ReplaceTag("@@User.company_name.name@@")%></dt>
					<dd class="company-name">
						<p class="attention">
						<asp:CustomValidator
							ID="cvShippingCompanyName"
							runat="Server"
							ControlToValidate="tbShippingCompanyName"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
						</p>
							<w2c:ExtendedTextBox ID="tbShippingCompanyName" placeholder='<%# ReplaceTag("@@User.company_name.name@@") %>' Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_COMPANY_NAME) %>" MaxLength='<%# GetMaxLength("@@User.company_name.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
					</dd>
					<%-- 部署名 --%>
					<dt><%: ReplaceTag("@@User.company_post_name.name@@")%></dt>
					<dd class="company-post">
						<p class="attention">
						<asp:CustomValidator
							ID="cvShippingCompanyPostName"
							runat="Server"
							ControlToValidate="tbShippingCompanyPostName"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
						</p>
							<w2c:ExtendedTextBox ID="tbShippingCompanyPostName" placeholder='<%# ReplaceTag("@@User.company_post_name.name@@") %>' Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_COMPANY_POST_NAME) %>" MaxLength='<%# GetMaxLength("@@User.company_post_name.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
					</dd>
					<%} %>
				<%-- 電話番号 --%>
				<% if (isShippingAddrCountryJp) { %>
					<dt>
						<%: ReplaceTag("@@User.tel1.name@@") %>
						<span class="require">※</span><span id="<%# "efo_sign_ship_tel1" + ((RepeaterItem)Container).ItemIndex %>"/>
					</dt>
					<dd class="tel">
						<p class="attention">
							<asp:CustomValidator
								ID="cvShippingTel1_1"
								runat="Server"
								ControlToValidate="tbShippingTel1"
								ValidationGroup="OrderShipping"
								ValidateEmptyText="true"
								SetFocusOnError="true"
								ClientValidationFunction="ClientValidate"
								EnableClientScript="false"
								CssClass="error_inline" />
						</p>
						<w2c:ExtendedTextBox ID="tbShippingTel1" Text="<%#: GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %>" CssClass="input_widthD input_border shortTel" Type="tel" MaxLength="13" style="width:100%;" runat="server" />
					</dd>
					<% } else { %>
					<dt><%: ReplaceTag("@@User.tel1.name@@", shippingAddrCountryIsoCode) %><span class="require">※</span></dt>
					<dd>
						<p class="attention">
						<asp:CustomValidator
							ID="cvShippingTel1Global"
							runat="Server"
							ControlToValidate="tbShippingTel1Global"
							ValidationGroup="OrderShippingGlobal"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
						</p>
						<w2c:ExtendedTextBox ID="tbShippingTel1Global" Type="tel" Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %>" MaxLength="30" runat="server"></w2c:ExtendedTextBox>
					</dd>

				<% } %>
					</div>
					<dd id="divSaveToUserShipping" visible="<%# this.IsLoggedIn %>" runat="server">
					<%-- ポストバックCustomValidatorの状態がクリアされてしまうため、JavaScirptで表示非表示を制御する --%>
					<p>
							<asp:RadioButtonList ID="rblSaveToUserShipping" OnSelectedIndexChanged="rblSaveToUserShipping_OnSelectedIndexChanged" AutoPostBack="true" SelectedValue='<%# ((CartObject)((RepeaterItem)Container).DataItem).Shippings[0].UserShippingRegistFlg ? "1" : "0" %>' RepeatLayout="Flow" CssClass="radioBtn" runat="server">
						<asp:ListItem Text="配送先情報を保存しない" Value="0"></asp:ListItem>
						<asp:ListItem Text="配送先情報を保存する" Value="1"></asp:ListItem>
						</asp:RadioButtonList>
					</p>
					</dd>

					<dd id="dlUserShippingName" visible="false" runat="server"><span id="<%# "efo_sign_ship_addr_name" + ((RepeaterItem)Container).ItemIndex %>"/>
					<p>配送先名をご入力ください</p>
							<w2c:ExtendedTextBox ID="tbUserShippingName" Text="<%# ((CartObject)((RepeaterItem)Container).DataItem).Shippings[0].UserShippingName %>" MaxLength="15" runat="server"></w2c:ExtendedTextBox>
						<br />
						<asp:CustomValidator
							ID="cvUserShippingName"
							runat="Server"
							ControlToValidate="tbUserShippingName"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
					</dd>
					
				</div>
				<%-- △配送先入力フォーム△ --%>
			</dd>
		</dl>
		</ContentTemplate>
		</asp:UpdatePanel>
		<%-- UPDATE PANELここまで(配送先情報) --%>
		<% } else { %>
			<%--▼▼ Amazon Pay(CV2)配送先情報 ▼▼--%>
			<% if ((this.AmazonPaySessionShippingAddress != null) && Constants.AMAZON_PAYMENT_CV2_ENABLED) { %>
			<div style="width: 340px;">
			<div class="userBox">
			<div class="top">
			<div class="bottom">
				<dl>
				<%-- 住所 --%>
				<dt>
					<%: ReplaceTag("@@User.addr.name@@") %>：
				</dt>
				<dd>
					<p>
						〒<%: this.AmazonPaySessionShippingAddress.Zip %><br />
						<%: this.AmazonPaySessionShippingAddress.Addr1 %><br />
						<%: this.AmazonPaySessionShippingAddress.Addr2 %><br />
						<%: this.AmazonPaySessionShippingAddress.Addr3 %><br />
						<%: this.AmazonPaySessionShippingAddress.Addr4 %>
					</p>
				</dd>
				<dt>
					<%: ReplaceTag("@@User.name.name@@") %>：
				</dt>
				<dd>
					<p><%: this.AmazonPaySessionShippingAddress.Name %></p>
				</dd>
				</dl>
				<asp:LinkButton ID="lbChangeAddress" ClientIDMode="Static" class="btn btn-mini" style="text-decoration:none; margin:10px 0;"　runat="server">配送先住所を変更</asp:LinkButton><br />
			</div><!--bottom-->
			</div><!--top-->
			</div><!--userBox-->
			</div>
			<% } else { %>
			<%--▲▲ Amazon Pay(CV2)配送先情報 ▲▲--%>
		<div id="divShipToOwnerAddress" Visible="<%# (this.IsLoggedIn == false) && CanInputShippingTo(Container.ItemIndex) %>" runat="server">
			<asp:CheckBox ID="cbShipToOwnerAddress" Text="注文者情報の住所へ配送する" Checked="<%# ((CartObject)Container.DataItem).Shippings[0].IsSameShippingAsCart1 %>" onclick="$('#shippingAddressBookContainer').toggle();" CssClass="checkBox" runat="server" />
		</div>
		<div id="shippingAddressBookContainer" style='margin-top: 10px;'>
			<%-- ▼▼Amazonアドレス帳ウィジェット(配送先情報)▼▼ --%>
			<div id="shippingAddressBookWidgetDiv"></div>
			<div id="shippingAddressBookErrorMessage" style="color:red;padding:5px" ClientIDMode="Static" runat="server"></div>
			<%-- ▲▲Amazonアドレス帳ウィジェット(配送先情報)▲▲ --%>
		</div>
		<% } %>
		<% } %>
		<%-- UPDATE PANEL開始(その他情報) --%>
		<asp:UpdatePanel ID="upOthersUpdatePanel" runat="server">
		<ContentTemplate>
		<span id="sInvoices" runat="server" visible="false">
			<dl class="order-form shipping-require">
				<div id="divUniformInvoiceType" runat="server">
					<dt>発票種類</dt>
					<dd>
						<asp:DropDownList ID="ddlUniformInvoiceType" runat="server"
							CssClass="input_border"
							DataSource="<%# ValueText.GetValueItemList(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE) %>"
							DataTextField="text"
							DataValueField="value"
							OnSelectedIndexChanged="ddlUniformInvoiceType_SelectedIndexChanged"
							AutoPostBack="true">
						</asp:DropDownList>
						<asp:DropDownList ID="ddlUniformInvoiceTypeOption" runat="server"
							CssClass="input_border"
							DataTextField="text"
							DataValueField="value"
							OnSelectedIndexChanged="ddlUniformInvoiceTypeOption_SelectedIndexChanged"
							AutoPostBack="true"
							Visible="false">
						</asp:DropDownList>
					</dd>
					<dl id="dlUniformInvoiceOption1_8" runat="server" visible="false">
						<dd>統一編号</dd>
						<dd>
							<asp:TextBox ID="tbUniformInvoiceOption1_8" placeholder="例:12345678" Text="<%# ((CartObject)((RepeaterItem)Container).DataItem).Shippings[0].UniformInvoiceOption1 %>" Width="220" runat="server" MaxLength="8"/>
							<p class="attention">
							<asp:CustomValidator
								ID="cvUniformInvoiceOption1_8" runat="server"
								ControlToValidate="tbUniformInvoiceOption1_8"
								ValidationGroup="OrderShippingGlobal"
								ValidateEmptyText="true"
								ClientValidationFunction="ClientValidate"
								SetFocusOnError="true"
								EnableClientScript="false"
								CssClass="error_inline" />
							</p>
							<asp:Label ID="lbUniformInvoiceOption1_8" runat="server" Text="<%# ((CartObject)((RepeaterItem)Container).DataItem).Shippings[0].UniformInvoiceOption1 %>" Visible="false"></asp:Label>
						</dd>
						<dd>会社名</dd>
						<dd>
							<asp:TextBox ID="tbUniformInvoiceOption2" placeholder="例:○○有限股份公司" Text="<%# ((CartObject)((RepeaterItem)Container).DataItem).Shippings[0].UniformInvoiceOption2 %>" Width="220" runat="server" MaxLength="20"/>
							<p class="attention">
							<asp:CustomValidator
								ID="cvUniformInvoiceOption2" runat="server"
								ControlToValidate="tbUniformInvoiceOption2"
								ValidationGroup="OrderShippingGlobal"
								ValidateEmptyText="true"
								ClientValidationFunction="ClientValidate"
								SetFocusOnError="true"
								EnableClientScript="false"
								CssClass="error_inline" />
							</p>
							<asp:Label ID="lbtbUniformInvoiceOption2" runat="server" Text="<%# ((CartObject)((RepeaterItem)Container).DataItem).Shippings[0].UniformInvoiceOption2 %>" Visible="false"></asp:Label>
						</dd>
					</dl>
					<dl id="dlUniformInvoiceOption1_3" runat="server" visible="false">
						<dd>寄付先コード</dd>
						<dd>
							<asp:TextBox ID="tbUniformInvoiceOption1_3" Text="<%# ((CartObject)((RepeaterItem)Container).DataItem).Shippings[0].UniformInvoiceOption1 %>" Width="220" runat="server" MaxLength="7" />
							<p class="attention">
							<asp:CustomValidator
								ID="cvUniformInvoiceOption1_3" runat="server"
								ControlToValidate="tbUniformInvoiceOption1_3"
								ValidationGroup="OrderShippingGlobal"
								ValidateEmptyText="true"
								ClientValidationFunction="ClientValidate"
								SetFocusOnError="true"
								EnableClientScript="false"
								CssClass="error_inline" />
							</p>
							<asp:Label ID="lbUniformInvoiceOption1_3" Text="<%# ((CartObject)((RepeaterItem)Container).DataItem).Shippings[0].UniformInvoiceOption1 %>" runat="server" Visible="false"></asp:Label>
						</dd>
					</dl>
					<dl id="dlUniformInvoiceTypeRegist" runat="server" visible="false">
						<dd visible="<%# this.IsLoggedIn %>" runat="server">
							<asp:CheckBox ID="cbSaveToUserInvoice" Checked="<%# ((CartObject)((RepeaterItem)Container).DataItem).Shippings[0].UserInvoiceRegistFlg %>" runat="server" Text="電子発票管理情報を保存する" AutoPostBack="true" OnCheckedChanged="cbSaveToUserInvoice_CheckedChanged" />
						</dd>
						<dd id="dlUniformInvoiceTypeRegistInput" runat="server" visible="false">
							電子発票情報名 <span class="require">※</span><br />
							<asp:TextBox ID="tbUniformInvoiceTypeName" MaxLength="30" runat="server" Text="<%# ((CartObject)((RepeaterItem)Container).DataItem).Shippings[0].InvoiceName %>"></asp:TextBox>
							<p class="attention">
							<asp:CustomValidator
								ID="cvUniformInvoiceTypeName" runat="server"
								ControlToValidate="tbUniformInvoiceTypeName"
								ValidationGroup="OrderShippingGlobal"
								ValidateEmptyText="true"
								ClientValidationFunction="ClientValidate"
								SetFocusOnError="true"
								EnableClientScript="false"
								CssClass="error_inline" />
							</p>
						</dd>
					</dl>
				</div>
				<div id="divInvoiceCarryType" runat="server">
					<dt>共通性載具</dt>
					<dd>
						<asp:DropDownList ID="ddlInvoiceCarryType" runat="server"
							CssClass="input_border"
							DataSource="<%# ValueText.GetValueItemList(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE) %>"
							DataTextField="text"
							DataValueField="value"
							OnSelectedIndexChanged="ddlInvoiceCarryType_SelectedIndexChanged"
							AutoPostBack="true"></asp:DropDownList>
						<asp:DropDownList ID="ddlInvoiceCarryTypeOption" runat="server"
							CssClass="input_border"
							DataTextField="text"
							DataValueField="value"
							OnSelectedIndexChanged="ddlInvoiceCarryTypeOption_SelectedIndexChanged"
							AutoPostBack="true"
							Visible="false">
						</asp:DropDownList>
					</dd>
					<dd id="divCarryTypeOption" runat ="server">
						<div id="divCarryTypeOption_8" runat="server" visible="false">
							<asp:TextBox ID="tbCarryTypeOption_8" Width="220" runat="server" Text="<%# ((CartObject)((RepeaterItem)Container).DataItem).Shippings[0].CarryTypeOptionValue %>" placeholder="例:/AB201+9(限8個字)" MaxLength="8" />
							<p class="attention">
							<asp:CustomValidator
								ID="cvCarryTypeOption_8" runat="server"
								ControlToValidate="tbCarryTypeOption_8"
								ValidationGroup="OrderShippingGlobal"
								ValidateEmptyText="true"
								ClientValidationFunction="ClientValidate"
								SetFocusOnError="true"
								EnableClientScript="false"
								CssClass="error_inline" />
							</p>
						</div>
						<div id="divCarryTypeOption_16" runat="server" visible="false">
							<asp:TextBox ID="tbCarryTypeOption_16" Width="220" Text="<%# ((CartObject)((RepeaterItem)Container).DataItem).Shippings[0].CarryTypeOptionValue %>" runat="server" placeholder="例:TP03000001234567(限16個字)" MaxLength="16" />
							<p class="attention">
							<asp:CustomValidator
								ID="cvCarryTypeOption_16" runat="server"
								ControlToValidate="tbCarryTypeOption_16"
								ValidationGroup="OrderShippingGlobal"
								ValidateEmptyText="true"
								ClientValidationFunction="ClientValidate"
								SetFocusOnError="true"
								EnableClientScript="false"
								CssClass="error_inline" />
							</p>
						</div>
					</dd>
					<dl id="dlCarryTypeOptionRegist" runat="server" visible="false">
						<dd visible="<%# this.IsLoggedIn %>" runat="server">
							<asp:CheckBox ID="cbCarryTypeOptionRegist" runat="server" Text="電子発票管理情報を保存する" AutoPostBack="true" OnCheckedChanged="cbCarryTypeOptionRegist_CheckedChanged" />
						</dd>
						<dd id="divCarryTypeOptionName" runat="server" visible="false">
							電子発票情報名 <span class="require">※</span><br />
							<asp:TextBox ID="tbCarryTypeOptionName" Text="<%# ((CartObject)((RepeaterItem)Container).DataItem).Shippings[0].InvoiceName %>" runat="server" MaxLength="30"></asp:TextBox>
							<p class="attention">
							<asp:CustomValidator
								ID="cvCarryTypeOptionName" runat="server"
								ControlToValidate="tbCarryTypeOptionName"
								ValidationGroup="OrderShippingGlobal"
								ValidateEmptyText="true"
								ClientValidationFunction="ClientValidate"
								SetFocusOnError="true"
								EnableClientScript="false"
								CssClass="error_inline" />
							</p>
						</dd>
					</dl>
					<dd>
						<asp:Label runat="server" ID="lbCarryTypeOption" Visible="false"></asp:Label>
					</dd>
				</div>
			</dl>
		</span>
		<dl class="order-form shipping-require">
			<dt visible="<%# CanInputShippingTo(((RepeaterItem)Container).ItemIndex) %>" runat="server">配送指定</dt>
			<dd>
				<dl>
					<div id="dvStorePickup" visible="false" runat="server">
						<div class="userList" runat="server">
							受け取り店舗を選択してください。
							<dl runat="server">
								<%-- 地域での絞り込みドロップダウン --%>
								<dt runat="server">地域：</dt>
								<dd>
									<asp:DropDownList
										ID="ddlRealShopArea"
										DataSource="<%# this.Process.RealShopAreaList %>"
										OnSelectedIndexChanged="ddlRealShopNarrowDown_OnSelectedIndexChanged"
										DataTextField="AreaName"
										DataValueField="AreaId"
										AutoPostBack="true"
										OnDataBound="ddlRealShopArea_DataBound"
										runat="server" />
								</dd>
								<%-- 都道府県での絞り込みドロップダウン --%>
								<dt runat="server">都道府県：</dt>
								<dd>
									<asp:DropDownList
										ID="ddlRealShopAddr1List"
										DataSource="<%# this.Addr1List %>"
										OnSelectedIndexChanged="ddlRealShopNarrowDown_OnSelectedIndexChanged"
										DataTextField="Text"
										DataValueField="Value"
										AutoPostBack="true"
										runat="server" />
								</dd>
							</dl>
							<dl id="dlRealShopName" runat="server">
								<dt runat="server">店舗名：</dt>
								<dd>
									<asp:DropDownList
										ID="ddlRealShopName"
										DataSource="<%# this.Process.RealShopNameList %>"
										OnSelectedIndexChanged="ddlRealShopNameList_OnSelectedIndexChanged"
										DataTextField="Name"
										DataValueField="RealShopId"
										AutoPostBack="true"
										Width="150"
										runat="server" />
									<asp:CustomValidator
										ID="cvRealShopName"
										runat="Server"
										ControlToValidate="ddlRealShopName"
										ValidationGroup="OrderShipping"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										ClientValidationFunction="ClientValidate"
										EnableClientScript="false"
										CssClass="error_inline" />
								</dd>
							</dl>
							<dl id="dlRealShopAddress" runat="server" visible="false">
								<dt runat="server">住所：</dt>
								<dd>
									<p>
										〒<asp:Literal ID="lRealShopZip" runat="server" /><br />
										<asp:Literal ID="lRealShopAddr1" runat="server" />
										<asp:Literal ID="lRealShopAddr2" runat="server" /><br />
										<asp:Literal ID="lRealShopAddr3" runat="server" /><br />
										<asp:Literal ID="lRealShopAddr4" runat="server" /><br />
										<asp:Literal ID="lRealShopAddr5" runat="server" />
									</p>
								</dd>
							</dl>
							<dl id="dlRealShopOpenningHours" runat="server" visible="false">
								<dt runat="server">営業時間：</dt>
								<dd>
									<asp:Literal ID="lRealShopOpenningHours" runat="server" />
								</dd>
							</dl>
							<dl id="dlRealShopTel" runat="server" visible="false">
								<dt runat="server">電話番号：</dt>
								<dd>
									<asp:Literal ID="lRealShopTel" runat="server" />
								</dd>
							</dl>
						</div>
					</div>
				</dl>
				<dl>
					<div id="dvShippingMethod" visible="<%# CanInputShippingTo(((RepeaterItem)Container).ItemIndex) %>" runat="server" class="userList">
						<dt>配送方法を選択して下さい。</dt>
						<dd>
							<asp:DropDownList ID="ddlShippingMethod" DataSource="<%# this.ShippingMethodList[((RepeaterItem)Container).ItemIndex] %>" OnSelectedIndexChanged="ddlShippingMethodList_OnSelectedIndexChanged" DataTextField="text" DataValueField="value" AutoPostBack="true" runat="server"></asp:DropDownList>
						</dd>
					</div>
				</dl>
				<dd id="dvDeliveryCompany" visible="<%# (CanInputShippingTo(((RepeaterItem)Container).ItemIndex) && CanDisplayDeliveryCompany(((RepeaterItem)Container).ItemIndex)) %>" runat="server">
					<dl>
						<dt>配送サービスを選択して下さい。</dt>
						<dd>
							<asp:DropDownList ID="ddlDeliveryCompany" DataSource="<%# GetDeliveryCompanyListItem(((RepeaterItem)Container).ItemIndex) %>" OnSelectedIndexChanged="ddlDeliveryCompanyList_OnSelectedIndexChanged" DataTextField="Value" DataValueField="Key" AutoPostBack="true" runat="server" />
						</dd>
					</dl>
				</dd>
				<div id="dvShipppingDateTime" visible="<%# CanInputDateOrTimeSet(((RepeaterItem)Container).ItemIndex) %>" runat="server" class="userList">
					<dl id="dlShipppingDateTime" runat="server">
						<dt id="dtShippingDate" visible="<%# CanInputDateSet(((RepeaterItem)Container).ItemIndex) %>" runat="server">配送希望日</dt>
						<dd id="ddShippingDate" visible="<%# CanInputDateSet(((RepeaterItem)Container).ItemIndex) %>" runat="server">
							<asp:DropDownList
								id="ddlShippingDate"
								CssClass="input_border"
								DataTextField="text"
								DataValueField="value"
								OnSelectedIndexChanged="ddlFixedPurchaseShippingDate_OnSelectedIndexChanged"
								AutoPostBack="true"
								runat="server" />
							<br />
							<asp:Label ID="lShippingDateErrorMessage" CssClass="attention" runat="server"></asp:Label>
						</dd>
						<div id="divShippingTime" runat="server">
							<dt id="dtShippingTime" visible="<%# CanInputTimeSet(((RepeaterItem)Container).ItemIndex) %>" runat="server">配送希望時間帯</dt>
							<dd id="ddShippingTime" visible="<%# CanInputTimeSet(((RepeaterItem)Container).ItemIndex) %>" runat="server">
								<asp:DropDownList ID="ddlShippingTime" runat="server" DataSource="<%# GetShippingTimeList(((RepeaterItem)Container).ItemIndex) %>" DataTextField="Text" DataValueField="Value" SelectedValue="<%# GetShippingTime(((RepeaterItem)Container).ItemIndex) %>"></asp:DropDownList>
							</dd>
						</div>
					</dl>
				</div>
			</dd>
		</dl>

		<dl class="order-form memo">
		<%-- 注文メモ --%>
		<dt>注文メモ</dt>
		<dd>
			<asp:Repeater ID="rMemos" runat="server" DataSource="<%# ((CartObject)((RepeaterItem)Container).DataItem).OrderMemos %>" Visible="<%# ((CartObject)((RepeaterItem)Container).DataItem).OrderMemos.Count != 0 %>">
		<HeaderTemplate>
			<dl>
		</HeaderTemplate>
		<ItemTemplate>
			<dt><%# WebSanitizer.HtmlEncode(Eval(CartOrderMemo.FIELD_ORDER_MEMO_NAME)) %></dt>
			<dd>
				<p class="attention"><span id="sErrorMessageMemo" runat="server"></span></p>
				<w2c:ExtendedTextBox ID="tbMemo"  runat="server" Text="<%# Eval(CartOrderMemo.FIELD_ORDER_MEMO_TEXT) %>" CssClass="<%# Eval(CartOrderMemo.FIELD_ORDER_MEMO_CSS) %>" TextMode="MultiLine"></w2c:ExtendedTextBox>
			</dd>
		</ItemTemplate>
		<FooterTemplate>
			</dl>
		</FooterTemplate>
		</asp:Repeater>
		</dd>
		<asp:CheckBox ID="cbOnlyReflectMemoToFirstOrder"
			Checked="<%# ((CartObject)((RepeaterItem)Container).DataItem).ReflectMemoToFixedPurchase %>"
			visible="<%# (((CartObject)((RepeaterItem)Container).DataItem).OrderMemos.Count != 0) && ((CartObject)((RepeaterItem)Container).DataItem).ReflectMemoToFixedPurchaseVisible %>"
			Text="2回目以降の注文メモにも追加する"
			CssClass="checkBox"
			runat="server" />
		</dl>

		<asp:Repeater ID="rOrderExtendInput" ItemType="OrderExtendItemInput" runat="server" Visible="<%# IsDisplayOrderExtend() %>" >
			<HeaderTemplate>
				<dl class="order-form">
					<dt>注文確認事項</dt>
					<dd>
						<dl>
			</HeaderTemplate>
			<ItemTemplate>
				<%-- 項目名 --%>
				<dt>
					</strong><%#: Item.SettingModel.SettingName %></strong>
					<span class="require"  runat="server" visible="<%# Item.SettingModel.IsNeecessary%>">※</span>
				</dt>
				<dd>
					<%-- 概要 --%>
					<div><%# Item.SettingModel.OutlineHtmlEncode %></div>
					
					<%-- TEXT --%>
					<div runat="server" visible="<%# Item.SettingModel.IsInputTypeText%>">
						<asp:TextBox runat="server" ID="tbSelect" Width="250px" MaxLength="100"></asp:TextBox>
					</div>
					<%-- DDL --%>
					<div runat="server" visible="<%# Item.SettingModel.IsInputTypeDropDown %>">
						<asp:DropDownList runat="server" ID="ddlSelect"></asp:DropDownList>
					</div>
					<%-- RADIO --%>
					<div runat="server" visible="<%# Item.SettingModel.IsInputTypeRadio %>">
						<asp:RadioButtonList runat="server" ID="rblSelect" RepeatDirection="Horizontal" RepeatColumns="3" RepeatLayout="Flow" CssClass="radioBtn"></asp:RadioButtonList>
					</div>
					<%-- CHECK --%>
					<div runat="server" visible="<%# Item.SettingModel.IsInputTypeCheckBox %>">
						<asp:CheckBoxList runat="server" ID="cblSelect" RepeatDirection="Horizontal" RepeatColumns="3" RepeatLayout="Flow" CssClass="checkBox"></asp:CheckBoxList>
					</div>
					<%-- 検証文言 --%>
					<small><p class="attention"><asp:Label runat="server" ID="lbErrMessage" CssClass="error_inline"></asp:Label></p></small>
				</dd>
				<asp:HiddenField ID="hfSettingId" runat="server" Value="<%# Item.SettingModel.SettingId %>" />
				<asp:HiddenField ID="hfInputType" runat="server" Value="<%# Item.SettingModel.InputType %>" />
			</ItemTemplate>
			<FooterTemplate>
						</dl>
					</dd>
				</dl>
			</FooterTemplate>
		</asp:Repeater>
		<%-- ▼定期購入配送パターン▼ --%>
			<div class="fixed" visible="<%# DisplayFixedPurchaseShipping((RepeaterItem)Container) %>" runat="server"><span id="efo_sign_fixed_purchase"/>
		<h2 visible="<%# DisplayFixedPurchaseShipping((RepeaterItem)Container) %>" runat="server">定期購入 配送パターンの指定</h2>
		<div id='<%# "efo_sign_fixed_purchase" + ((RepeaterItem)Container).ItemIndex %>'></div>
		<%-- ▽デフォルトチェックの設定▽--%>
			<%-- ラジオボタンのデータバインド <%#.. より前で呼び出してください。 --%>
			<%# Constants.FIXED_PURCHASE_USESHIPPINGINTERVALDAYSDEFAULT_FLG ? SetFixedPurchaseDefaultCheckPriority(((RepeaterItem)Container).ItemIndex, 3, 2, 1, 4) : SetFixedPurchaseDefaultCheckPriority(((RepeaterItem)Container).ItemIndex, 2, 3, 1, 4) %>
		<%-- △ - - - - - - - - - - - △--%>
		<dl class="order-form" visible="<%# DisplayFixedPurchaseShipping((RepeaterItem)Container) %>" runat="server">
				<dd id="Div4" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE, false) %>" runat="server">
				<asp:RadioButton ID="rbFixedPurchaseMonthlyPurchase_Date" 
						Text="毎月日付指定" Checked="<%# GetFixedPurchaseKbnInputChecked(((RepeaterItem)Container).ItemIndex, 1) %>" 
					GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" /><span id='<%# "efo_sign_fixed_purchase_month" + ((RepeaterItem)Container).ItemIndex %>' />
					<div id="ddFixedPurchaseMonthlyPurchase_Date" class="fixed-date" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE, true) %>" runat="server">
				<asp:DropDownList ID="ddlFixedPurchaseMonth"
						DataSource="<%# GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container).ItemIndex, true) %>"
						DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container).ItemIndex, Constants.FIXED_PURCHASE_SETTING_MONTH) %>'
					OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnSelectedIndexChanged" AutoPostBack="true" runat="server">
				</asp:DropDownList>
					ヶ月ごと
				<asp:DropDownList ID="ddlFixedPurchaseMonthlyDate"
					DataSource="<%# GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container).ItemIndex, true, false, true) %>"
						DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container).ItemIndex, Constants.FIXED_PURCHASE_SETTING_MONTHLY_DATE) %>'
					OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnSelectedIndexChanged" AutoPostBack="true" runat="server">
				</asp:DropDownList>
					日に届ける
				</div>
				<p class="attention">
				<asp:CustomValidator
					ID="cvFixedPurchaseMonth"
					runat="Server"
					ControlToValidate="ddlFixedPurchaseMonth" 
					ValidationGroup="OrderShipping" 
					ValidateEmptyText="true" 
					SetFocusOnError="true" />
				<asp:CustomValidator
					ID="cvFixedPurchaseMonthlyDate"
					runat="Server"
					ControlToValidate="ddlFixedPurchaseMonthlyDate" 
					ValidationGroup="OrderShipping" 
					ValidateEmptyText="true" 
					SetFocusOnError="true" />
				</p>
			</dd>
				<dd visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY, false) %>" runat="server">
				<asp:RadioButton ID="rbFixedPurchaseMonthlyPurchase_WeekAndDay" 
						Text="月間隔・週・曜日指定" Checked="<%# GetFixedPurchaseKbnInputChecked(((RepeaterItem)Container).ItemIndex, 2) %>" 
					GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" /><span id='<%# "efo_sign_fixed_purchase_week_and_day" + ((RepeaterItem)Container).ItemIndex %>' />
				<div id="ddFixedPurchaseMonthlyPurchase_WeekAndDay" class="fixed-date" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY, true) %>" runat="server">
				<asp:DropDownList ID="ddlFixedPurchaseIntervalMonths"
					DataSource="<%# GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container).ItemIndex, true, true) %>"
					DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container).ItemIndex, Constants.FIXED_PURCHASE_SETTING_INTERVAL_MONTHS) %>'
					OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnSelectedIndexChanged" AutoPostBack="true" runat="server" />
					ヶ月ごと
				<asp:DropDownList ID="ddlFixedPurchaseWeekOfMonth"
					DataSource="<%# ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_WEEK_LIST) %>"
						DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container).ItemIndex, Constants.FIXED_PURCHASE_SETTING_WEEK_OF_MONTH) %>'
					OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnSelectedIndexChanged" AutoPostBack="true" runat="server">
				</asp:DropDownList>
				<asp:DropDownList ID="ddlFixedPurchaseDayOfWeek"
					DataSource="<%# ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DAY_LIST) %>"
						DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container).ItemIndex, Constants.FIXED_PURCHASE_SETTING_DAY_OF_WEEK) %>'
					OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnSelectedIndexChanged" AutoPostBack="true" runat="server">
				</asp:DropDownList>
					に届ける
				</div>
				<p class="attention">
				<asp:CustomValidator ID="cvFixedPurchaseIntervalMonths" runat="Server"
					ControlToValidate="ddlFixedPurchaseIntervalMonths"
					ValidationGroup="OrderShipping"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<asp:CustomValidator
					ID="cvFixedPurchaseWeekOfMonth"
					runat="Server"
					ControlToValidate="ddlFixedPurchaseWeekOfMonth" 
					ValidationGroup="OrderShipping" 
					ValidateEmptyText="true" 
					SetFocusOnError="true" />
				<asp:CustomValidator
					ID="cvFixedPurchaseDayOfWeek"
					runat="Server"
					ControlToValidate="ddlFixedPurchaseDayOfWeek" 
					ValidationGroup="OrderShipping" 
					ValidateEmptyText="true" 
					SetFocusOnError="true" />
				</p>
			</dd>
				<dd id="Div6" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS, false) %>" runat="server">
				<asp:RadioButton ID="rbFixedPurchaseRegularPurchase_IntervalDays" 
						Text="配送日間隔指定" Checked="<%# GetFixedPurchaseKbnInputChecked(((RepeaterItem)Container).ItemIndex, 3) %>" 
					GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" /><span id='<%# "efo_sign_fixed_purchase_interval_days" + ((RepeaterItem)Container).ItemIndex %>' />
					<div id="ddFixedPurchaseRegularPurchase_IntervalDays" class="fixed-date" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS, true) %>" runat="server">
					<asp:DropDownList ID="ddlFixedPurchaseIntervalDays"
							DataSource='<%# GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container).ItemIndex, false) %>'
							DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container).ItemIndex, Constants.FIXED_PURCHASE_SETTING_INTERVAL_DAYS) %>'
						OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnSelectedIndexChanged" AutoPostBack="true" runat="server">
					</asp:DropDownList>
						日ごとに届ける
				</div>
					<asp:HiddenField ID="hfFixedPurchaseDaysRequired" Value="<%# this.ShopShippingList[((RepeaterItem)Container).ItemIndex].FixedPurchaseShippingDaysRequired %>" runat="server" />
					<asp:HiddenField ID="hfFixedPurchaseMinSpan" Value="<%# this.ShopShippingList[((RepeaterItem)Container).ItemIndex].FixedPurchaseMinimumShippingSpan %>" runat="server" />
				<p class="attention">
				<asp:CustomValidator
					ID="cvFixedPurchaseIntervalDays"
					runat="Server"
					ControlToValidate="ddlFixedPurchaseIntervalDays" 
					ValidationGroup="OrderShipping" 
					ValidateEmptyText="true" 
					SetFocusOnError="true" />
				</p>
			</dd>
			<dd visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY, false) %>" runat="server">
				<asp:RadioButton ID="rbFixedPurchaseEveryNWeek"
					Text="週間隔・曜日指定" Checked="<%# GetFixedPurchaseKbnInputChecked(((RepeaterItem)Container).ItemIndex, 4) %>"
					GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" /><span id='<%#: "efo_sign_fixed_purchase_week" + ((RepeaterItem)Container).ItemIndex %>' />
				<div id="ddFixedPurchaseEveryNWeek" class="fixed-date" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY, true) %>" runat="server">
					<asp:DropDownList ID="ddlFixedPurchaseEveryNWeek_Week"
						DataSource="<%# GetFixedPurchaseEveryNWeekDropdown(((RepeaterItem)Container).ItemIndex, true) %>"
						DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container).ItemIndex, Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_WEEK) %>'
						OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnSelectedIndexChanged" AutoPostBack="true"
						runat="server">
					</asp:DropDownList>
					週間ごと
					<asp:DropDownList ID="ddlFixedPurchaseEveryNWeek_DayOfWeek"
						DataSource="<%# GetFixedPurchaseEveryNWeekDropdown(((RepeaterItem)Container).ItemIndex, false) %>"
						DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container).ItemIndex, Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_DAY_OF_WEEK) %>'
						OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnSelectedIndexChanged" AutoPostBack="true"
						runat="server">
					</asp:DropDownList>
					に届ける
				</div>
				<p class="attention">
				<asp:CustomValidator
					ID="cvFixedPurchaseEveryNWeek"
					runat="Server"
					ControlToValidate="ddlFixedPurchaseEveryNWeek_Week"
					ValidationGroup="OrderShipping"
					ValidateEmptyText="true"
					SetFocusOnError="true"/>
				<asp:CustomValidator
					ID="cvFixedPurchaseEveryNWeekDayOfWeek"
					runat="Server"
					ControlToValidate="ddlFixedPurchaseEveryNWeek_DayOfWeek"
					ValidationGroup="OrderShipping"
					ValidateEmptyText="true"
					SetFocusOnError="true"/>
				</p>
			</dd>
		</dl>
			<small><p class="attention" runat="server" visible="<%# GetAllFixedPurchaseKbnEnabled(((RepeaterItem)Container).ItemIndex) == false %>">同時に定期購入できない商品が含まれております。</p></small>
		<dl class="order-form">
			<dt id="dtFirstShippingDate" visible="true" runat="server">初回配送予定日</dt>
			<dd visible="true" runat="server">
				<asp:Label ID="lblFirstShippingDate" runat="server"></asp:Label>
				<asp:DropDownList
					ID="ddlFirstShippingDate"
					visible="true"
					OnDataBound="ddlFirstShippingDate_OnDataBound"
					OnSelectedIndexChanged="ddlFirstShippingDate_ItemSelected"
					AutoPostBack="true"
					runat="server" />
				<asp:Literal ID="lFirstShippingDateDayOfWeekNoteMessage" visible="false" runat="server">
					<br>曜日指定は次回配送日より適用されます。
				</asp:Literal>
			</dd>
			<dt id="dtNextShippingDate" visible="true" runat="server">2回目の配送日を選択</dt>
			<dd visible="true" runat="server">
				<asp:Label ID="lblNextShippingDate" visible="false" runat="server"></asp:Label>&nbsp;
				<asp:DropDownList
					ID="ddlNextShippingDate"
					visible="false"
					OnDataBound="ddlNextShippingDate_OnDataBound"
					OnSelectedIndexChanged="ddlNextShippingDate_OnSelectedIndexChanged"
					AutoPostBack="True"
					runat="server" />
			</dd>
		</dl>
		<p class="attention"><span ID="sErrorMessage" runat="server"></span></p>
		</div>
		<%-- ▲定期購入配送パターン▲ --%>
		</ContentTemplate>
		</asp:UpdatePanel>
		<%-- UPDATE PANELここまで(その他情報) --%>
	</div>
	<%-- ▲配送先情報▲ --%>

	<%-- ▼お支払い情報▼ --%>
	<div class="order-unit order-payment">
	<h2>お支払い方法</h2>
		<% if (this.IsAmazonLoggedIn == false) { %>
		<%-- UPDATE PANEL開始(お支払い情報) --%>
		<asp:UpdatePanel ID="upPaymentUpdatePanel" runat="server" Visible="<%#this.IsAmazonLoggedIn == false %>">
		<ContentTemplate>

		<h3>カート番号<%# ((RepeaterItem)Container).ItemIndex + 1 %></h3>
		<span style="color:red" runat="server" visible="<%# (string.IsNullOrEmpty(StringUtility.ToEmpty(this.DispLimitedPaymentMessages[((RepeaterItem)Container).ItemIndex])) == false) %>">
				<%# StringUtility.ToEmpty(this.DispLimitedPaymentMessages[((RepeaterItem)Container).ItemIndex]) %>
		</span>
			<asp:CheckBox ID="cbUseSamePaymentAddrAsCart1" visible="<%# (((RepeaterItem)Container).ItemIndex != 0) %>" Checked="<%# ((CartObject)((RepeaterItem)Container).DataItem).Payment.IsSamePaymentAsCart1 %>" Text="カート番号「1」と同じお支払い方法を指定する" OnCheckedChanged="cbUseSamePaymentAddrAsCart1_OnCheckedChanged" AutoPostBack="true" CssClass="select-same-payment" runat="server" />

		<div id='<%# "efo_sign_payment" + ((RepeaterItem)Container).ItemIndex %>'></div>
		<asp:HiddenField ID="hfPaidyTokenId" runat="server" />
		<asp:HiddenField ID="hfPaidyPaySelected" runat="server" />
		<% if ((Constants.PAYMENT_CHOOSE_TYPE_LP_OPTION
				? string.IsNullOrEmpty(this.LandingPageDesignModel.PaymentChooseType)
					? Constants.PAYMENT_CHOOSE_TYPE : this.LandingPageDesignModel.PaymentChooseType
				: Constants.PAYMENT_CHOOSE_TYPE) == Constants.PAYMENT_CHOOSE_TYPE_DDL) { %>
			<dl class="order-form payment-list">
				<dt class="title">
					<asp:DropDownList
						ID="ddlPayment"
						runat="server"
						DataSource="<%# this.ValidPayments[((RepeaterItem)Container).ItemIndex] %>"
						SelectedValue='<%# GetSelectedPaymentId(this.ValidPayments[((RepeaterItem)Container).ItemIndex], ((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment) %>'
						ItemType="w2.Domain.Payment.PaymentModel" visible="<%# (((RepeaterItem)Container).ItemIndex == 0) %>"
						OnSelectedIndexChanged="rbgPayment_OnCheckedChanged"
						AutoPostBack="true"
						DataTextField="PaymentName"
						DataValueField="PaymentId" />
				</dt>
			</dl>
		<% } %>
		<asp:Repeater ID="rPayment" runat="server" DataSource="<%# this.ValidPayments[((RepeaterItem)Container).ItemIndex] %>" ItemType="w2.Domain.Payment.PaymentModel" visible="<%# (((RepeaterItem)Container).ItemIndex == 0) %>">
			<HeaderTemplate>
				<dl class="order-form payment-list">
			</HeaderTemplate>
			<ItemTemplate>
				<asp:HiddenField ID="hfShopId" Value='<%# Item.ShopId %>' runat="server" />
				<asp:HiddenField ID="hfPaymentId" Value='<%# Item.PaymentId %>' runat="server" />
				<asp:HiddenField ID="hfPaymentName" Value='<%# Item.PaymentName %>' runat="server" />
				<asp:HiddenField
					ID="hfCreditBincode"
					Value='<%# GetCreditValue(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment, CartPayment.FIELD_CREDIT_BINCODE) %>'
					runat="server" />
				<% if ((Constants.PAYMENT_CHOOSE_TYPE_LP_OPTION
						? string.IsNullOrEmpty(this.LandingPageDesignModel.PaymentChooseType)
							? Constants.PAYMENT_CHOOSE_TYPE : this.LandingPageDesignModel.PaymentChooseType
						: Constants.PAYMENT_CHOOSE_TYPE) == Constants.PAYMENT_CHOOSE_TYPE_RB) { %>
				<dt class="title">
						<w2c:RadioButtonGroup ID="rbgPayment" Checked='<%# ((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment.PaymentId == Item.PaymentId %>' GroupName='<%# "Payment_" + GetParentRepeaterItem(Container, "rCartList").ItemIndex %>' Text="<%# WebSanitizer.HtmlEncode(Item.PaymentName) %>" OnCheckedChanged="rbgPayment_OnCheckedChanged" AutoPostBack="true" runat="server" />
				</dt>
				<% } %>
				<span id='<%# "efo_sign_payment" + ((RepeaterItem)Container).ItemIndex %>' ></span>
				<dd id="ddCredit" class="credit-card" runat="server">
					<%-- クレジット --%>
					<div class="box-center" runat="server" visible="<%# OrderCommon.GetRegistedCreditCardSelectable(this.IsLoggedIn, this.CreditCardList.Count - 1)%>">
							<asp:DropDownList ID="ddlUserCreditCard" runat="server" DataSource="<%# this.CreditCardList %>" SelectedValue='<%# GetListItemValue(this.CreditCardList ,((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment.CreditCardBranchNo) %>' OnSelectedIndexChanged="ddlUserCreditCard_OnSelectedIndexChanged" AutoPostBack="true" DataTextField="text" DataValueField="value" ></asp:DropDownList>
					</div>

					<%-- ▽新規カード▽ --%>
						<ul id="divCreditCardInputForm" class="new" runat="server" visible='<%# IsNewCreditCard(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment) %>'>
						
						<% if(this.IsCreditCardLinkPayment() == false) { %>
						<%--▼▼ カード情報取得用 ▼▼--%>
							<input type="hidden" id="hidCinfo" name="hidCinfo" value="<%# CreateGetCardInfoJsScriptForCreditTokenForCart(GetParentRepeaterItem(Container, "rCartList"), Container) %>" />
						<%--▲▲ カード情報取得用 ▲▲--%>

						<%--▼▼ クレジット Token用 ▼▼--%>
						<asp:HiddenField ID="hfCreditToken" Value='<%# ((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment.CreditToken %>' runat="server" />
						<span id="spanErrorMessageForCreditCard" style="color: red; display: none" runat="server"></span>
						<%--▲▲ クレジット Token用 ▲▲--%>
						<ul>
							<%--▼▼ カード情報入力（トークン未取得・利用なし） ▼▼--%>
							<% if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten) { %>
								<!-- Rakuten Card Form -->
								<asp:LinkButton id="lbEditCreditCardNoForRakutenToken" CssClass="lbEditCreditCardNoForRakutenToken" OnClick="lbEditCreditCardNoForToken_Click" style="display:none" runat="server">再入力</asp:LinkButton>
								<uc:RakutenCreditCard
									IsOrder="true"
									CartIndex='<%# GetParentRepeaterItem(Container, "rCartList").ItemIndex + 1 %>'
									InstallmentCodeList="<%# this.CreditInstallmentsList %>"
									SelectedInstallmentCode = '<%# GetCreditValue(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment, CartPayment.FIELD_CREDIT_INSTALLMENTS_CODE) %>'
									SelectedExpireMonth='<%# GetCreditValue(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment, CartPayment.FIELD_CREDIT_EXPIRE_MONTH) %>'
									SelectedExpireYear='<%# GetCreditValue(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment, CartPayment.FIELD_CREDIT_EXPIRE_YEAR) %>'
									AuthorName='<%# GetCreditValue(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment, CartPayment.FIELD_CREDIT_AUTHOR_NAME) %>'
									CreditCardNo4='<%# ((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment.CreditCardNo4 %>'
									CreditCompany='<%# ((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment.CreditCardCompany %>'
									SecurityCode='<%# ((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment.RakutenCvvToken %>'
									runat="server"
									ID="ucRakutenCreditCard" />
							<% } else { %>
							<div id="divCreditCardNoToken" visible='<%# (HasCreditToken(Container) == false) %>' runat="server">
							<%if (OrderCommon.CreditCompanySelectable) {%>
							<li class="card-company">
								<h4>カード会社</h4>
									<div><asp:DropDownList id="ddlCreditCardCompany" runat="server" DataSource="<%# this.CreditCompanyList %>" DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetCreditValue(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment, CartPayment.FIELD_CREDIT_COMPANY) %>' CssClass="input_widthG input_border"></asp:DropDownList></div>
							</li>
							<% } %>
							<li class="card-nums">
								<p class="attention">
								<asp:CustomValidator
									ID="cvCreditCardNo1"
									runat="Server"
										ControlToValidate="tbCreditCardNo1"
										ValidationGroup="OrderPayment"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										CssClass="error_inline" />
									<span id="sErrorMessage" runat="server" />
								</p>
								<h4>カード番号<span class="require">※</span></h4><span id='<%# "efo_sign_credit_card_no" + GetParentRepeaterItem(Container, "rCartList").ItemIndex %>' ></span>
								<div>
									<w2c:ExtendedTextBox id="tbCreditCardNo1" Type="tel" runat="server" CssClass="tel" MaxLength="16" Text='<%# GetCreditValue(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment, CartPayment.FIELD_CREDIT_CARD_NO) %>' autocomplete="off"></w2c:ExtendedTextBox>
									<br />
								</div>
								<p>
									カードの表記のとおりご入力ください。<br />
									例：<br />
									1234567890123456（ハイフンなし）
								</p>
							</li>
								
							<li class="card-exp">
								<h4>有効期限</h4>
								<div>
										<asp:DropDownList id="ddlCreditExpireMonth" runat="server" DataSource="<%# this.CreditExpireMonth %>" SelectedValue='<%# GetCreditValue(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment, CartPayment.FIELD_CREDIT_EXPIRE_MONTH) %>'></asp:DropDownList>
										/
										<asp:DropDownList id="ddlCreditExpireYear" runat="server" DataSource="<%# this.CreditExpireYear %>" SelectedValue='<%# GetCreditValue(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment, CartPayment.FIELD_CREDIT_EXPIRE_YEAR) %>'></asp:DropDownList>
										 (月/年)
								</div>
							</li>

							<li class="card-name">
								<h4>カード名義人<span class="require">※</span></h4><span id='<%# "efo_sign_credit_author_name" + GetParentRepeaterItem(Container, "rCartList").ItemIndex %>' ></span>
								<div>
									<p class="attention">
									<asp:CustomValidator
										ID="cvCreditAuthorName"
										runat="Server"
											ControlToValidate="tbCreditAuthorName"
											ValidationGroup="OrderPayment"
											ValidateEmptyText="true"
											SetFocusOnError="true"
											CssClass="error_inline" />
									</p>
										<w2c:ExtendedTextBox id="tbCreditAuthorName" Type="text" runat="server" MaxLength="50" Text='<%# GetCreditValue(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment, CartPayment.FIELD_CREDIT_AUTHOR_NAME) %>' autocomplete="off"></w2c:ExtendedTextBox>
								<p>例：「TAROU YAMADA」</p>
								</div>
							</li>

							<li class="card-sequrity" visible="<%# OrderCommon.CreditSecurityCodeEnable %>" runat="server">
								<h4>セキュリティコード<span class="require">※</span></h4><span id='<%# "efo_sign_credit_sec_code" + GetParentRepeaterItem(Container, "rCartList").ItemIndex %>' ></span>
								<div>
									<p class="attention">
									<asp:CustomValidator
										ID="cvCreditSecurityCode"
										runat="Server"
											ControlToValidate="tbCreditSecurityCode"
											ValidationGroup="OrderPayment"
											ValidateEmptyText="true"
											SetFocusOnError="true"
											CssClass="error_inline" />
									</p>
										<w2c:ExtendedTextBox id="tbCreditSecurityCode" Type="tel" runat="server" MaxLength="4" Text='<%# GetCreditValue(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment, CartPayment.FIELD_CREDIT_SECURITY_CODE) %>' autocomplete="off"></w2c:ExtendedTextBox>
									<p>
										<img src="<%: Constants.PATH_ROOT %>SmartPhone/Contents/ImagePkg/common/card-sequrity-code.gif" alt="セキュリティコードとは" width="280" />
									</p>
								</div>
							</li>
							</div>
							<% } %>
							<%--▲▲ カード情報入力（トークン未取得・利用なし） ▲▲--%>

							<%--▼▼ カード情報入力（トークン取得済） ▼▼--%>
							<% if (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Rakuten) { %>
							<div id="divCreditCardForTokenAcquired" Visible='<%# HasCreditToken(Container) %>' runat="server">
							<%if (OrderCommon.CreditCompanySelectable) {%>
							<li class="card-company">
							<h4>カード会社</h4>
								<p><asp:Literal ID="lCreditCardCompanyNameForTokenAcquired" Text='<%# ((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment.CreditCardCompanyName %>' runat="server"></asp:Literal><br /></p>
							</li>
							<%} %>
							<li class="card-nums">
							<h4>カード番号</h4>
								<p>XXXXXXXXXXXX<asp:Literal ID="lLastFourDigitForTokenAcquired" Text='<%# ((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment.CreditCardNo4 %>' runat="server"></asp:Literal><br /></p>
							<asp:LinkButton id="lbEditCreditCardNoForToken" OnClick="lbEditCreditCardNoForToken_Click" runat="server">再入力</asp:LinkButton>
							</li>
							<li class="card-exp">
							<h4>有効期限</h4>
								<p><asp:Literal ID="lExpirationMonthForTokenAcquired" Text='<%# ((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment.CreditExpireMonth %>' runat="server"></asp:Literal>
							&nbsp;/&nbsp;
								<asp:Literal ID="lExpirationYearForTokenAcquired" Text='<%# ((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment.CreditExpireYear %>' runat="server"></asp:Literal> (月/年)</p>
							</li>
							<li class="card-name">
							<h4>カード名義人</h4>
								<p><asp:Literal ID="lCreditAuthorNameForTokenAcquired" Text='<%# ((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment.CreditAuthorName %>' runat="server"></asp:Literal><br /></p>
							</li>
							</div>
							<%--▲▲ カード情報入力（トークン取得済） ▲▲ --%>
							<li class="card-time" id="Div3" visible="<%# OrderCommon.CreditInstallmentsSelectable %>" runat="server">
								<h4>支払い回数</h4>
								<div>
										<asp:DropDownList id="dllCreditInstallments" runat="server" DataSource="<%# this.CreditInstallmentsList %>" DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetCreditValue(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment, CartPayment.FIELD_CREDIT_INSTALLMENTS_CODE) %>'></asp:DropDownList>
									<p>AMEX/DINERSは一括のみとなります。</p>
								</div>
							</li>

							</ul>
							<% } %>
							<% } else { %>
								<div>注文完了後に遷移する外部サイトで<br />
									カード番号を入力してください。</div>
							<% } %>

							<div class="box-center">
								<asp:CheckBox ID="cbRegistCreditCard" runat="server" Checked='<%# ((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment.UserCreditCardRegistFlg || UserRegisterDefaultChecked %>' Visible="<%# OrderCommon.GetCreditCardRegistable(this.IsLoggedIn, this.CreditCardList.Count - 1) %>" Text="このカードを登録する" OnCheckedChanged="cbRegistCreditCard_OnCheckedChanged" AutoPostBack="true" />
							</div>

							<div id="divUserCreditCardName" class="card-save" Visible="<%# (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten) %>" runat="server">
								<p class="msg">クレジットカードを保存する場合は、以下をご入力ください。</p>
								<h4>登録名<span class="require">※</span></h4><span id='<%# "efo_sign_register_name" + GetParentRepeaterItem(Container, "rCartList").ItemIndex %>' ></span>
								<div class="card-title">
									<% if (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Rakuten) { %>
										<p class="attention">
									<asp:CustomValidator
										ID="cvUserCreditCardName"
										runat="Server"
										ControlToValidate="tbUserCreditCardName"
										ValidationGroup="OrderShipping-OrderPayment"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										ClientValidationFunction="ClientValidate"
										CssClass="error_inline" />
									</p>
										<% } %>
										<asp:TextBox ID="tbUserCreditCardName" Text='<%# (UserRegisterDefaultChecked) ? ReplaceTag("@@CreditCard.disp_name.text@@") : ((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment.UserCreditCardName %>' MaxLength="100" CssClass="" runat="server"></asp:TextBox>
								</div>
							</div>

					</ul>
						<%-- △新規カード△ --%>

						<%-- ▽登録済みカード▽ --%>
							<div id="divCreditCardDisp" visible='<%# IsNewCreditCard(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment) == false %>' runat="server">
							<ul>
								<%if (OrderCommon.CreditCompanySelectable) {%>
								<li>
									<h4>カード会社</h4>
									<div>
										<asp:Literal ID="lCreditCardCompanyName" runat="server"></asp:Literal>
									</div>
								</li>
								<%} %>
								<li>
									<h4>カード番号</h4>
									<div>
										XXXXXXXXXXXX<asp:Literal ID="lLastFourDigit" runat="server"></asp:Literal>
									</div>
								</li>
								<li>
									<h4>有効期限</h4>
									<div>
										<asp:Literal ID="lExpirationMonth" runat="server"></asp:Literal>/&nbsp;<asp:Literal ID="lExpirationYear" runat="server"></asp:Literal> (月/年)
									</div>
								</li>
								<li>
									<h4>カード名義人</h4>
									<div>
										<asp:Literal ID="lCreditAuthorName" runat="server"></asp:Literal>
									</div>
								</li>
								<li>
									<h4>支払い回数</h4>
										<div><asp:DropDownList id="dllCreditInstallments2" runat="server" DataSource="<%# this.CreditInstallmentsList %>" DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetCreditValue(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment, CartPayment.FIELD_CREDIT_INSTALLMENTS_CODE) %>'></asp:DropDownList>
									<p class="attention">※AMEX/DINERSは一括のみとなります。</p></div>
								</li>
							</ul>
						</div>
						<%-- △登録済みカード△ --%>
				</dd>


				<%-- コンビニ(前払い) --%>
				<dd id="ddCvsPre" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE) %>" runat="server">

					<%-- コンビニ(前払い)：電算システム --%>
					<div id="Div4" visible="<%# (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Dsk) %>" runat="server">
						<strong>支払いコンビニ選択</strong>
							<p><asp:DropDownList ID="ddlDskCvsType" DataSource='<%# this.CvsTypeList %>' DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetDskConveniType(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment) %>' CssClass="input_widthC input_border" runat="server"></asp:DropDownList></p>
					</div>

					<%-- コンビニ(前払い)：SBPS --%>
					<div id="Div1" visible="<%# (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.SBPS) %>" runat="server">
						<strong>支払いコンビニ選択</strong>
							<p><asp:DropDownList ID="ddlSBPSCvsType" DataSource='<%# this.CvsTypeList %>' DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetSBPSConveniType(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment) %>' CssClass="input_widthC input_border" runat="server"></asp:DropDownList></p>
					</div>
					
					<%-- コンビニ(前払い)：ヤマトKWC --%>
					<div class="inner" visible="<%# (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.YamatoKwc) %>" runat="server">
						<p class="title">支払いコンビニ選択</p>
							<p><asp:DropDownList ID="ddlYamatoKwcCvsType" DataSource='<%# this.CvsTypeList %>' DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetYamatoKwcConveniType(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment) %>' CssClass="input_widthC input_border" runat="server"></asp:DropDownList></p>
					</div>
					<%-- コンビニ(前払い)：Gmo --%>
					<div class="inner" visible="<%# (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Gmo) %>" runat="server">
						<p class="title">支払いコンビニ選択</p>
						<p><asp:DropDownList ID="ddlGmoCvsType" DataSource='<%# this.CvsTypeList %>' DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetGmoConveniType(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment) %>' CssClass="input_widthC input_border" runat="server"></asp:DropDownList></p>
					</div>
					<%-- コンビニ(前払い)：Rakuten --%>
					<div class="inner" visible="<%# (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Rakuten) %>" runat="server">
						<p class="title">支払いコンビニ選択</p>
						<p>
							<asp:DropDownList
								ID="ddlRakutenCvsType"
								DataSource='<%# this.CvsTypeList %>'
								DataTextField="Text"
								DataValueField="Value"
								SelectedValue='<%# GetRakutenConvenienceType(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment) %>'
								CssClass="input_widthC input_border"
								runat="server" />
						</p>
					</div>
					<%-- コンビニ(前払い)：Zeus --%>
					<div class="inner" visible="<%# OrderCommon.IsPaymentCvsTypeZeus %>" runat="server">
						<p class="title">支払いコンビニ選択</p>
						<p>
							<asp:DropDownList
								ID="ddlZeusCvsType"
								DataSource='<%# this.CvsTypeList %>'
								DataTextField="Text"
								DataValueField="Value"
								SelectedValue='<%# GetZeusConvenienceType(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment) %>'
								CssClass="input_widthC input_border"
								runat="server" />
						</p>
					</div>
					<%-- コンビニ(前払い)：Paygent --%>
					<div class="inner" visible="<%# OrderCommon.IsPaymentCvsTypePaygent %>" runat="server">
						<p class="title">支払いコンビニ選択</p>
						<p>
							<asp:DropDownList
								ID="ddlPaygentCvsType"
								DataSource='<%# this.CvsTypeList %>'
								DataTextField="Text"
								DataValueField="Value"
								SelectedValue='<%# GetPaygentConvenienceType(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment) %>'
								CssClass="input_widthC input_border"
								runat="server" />
						</p>
					</div>
				</dd>
				<%-- コンビニ(前払い)ここまで --%>

				<%-- コンビニ(後払い) --%>
				<dd id="ddCvsDef" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF) %>" runat="server">
					<uc:PaymentDescriptionCvsDef runat="server" id="PaymentDescriptionCvsDef" />
				</dd>

				<%-- ヤマト後払いSMS認証連携 --%>
				<dd id="ddSmsDef" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_SMS_DEF) %>" runat="server">
					<uc:PaymentDescriptionSmsDef runat="server" id="ucPaymentDescriptionSmsDef" />
				</dd>

				<%-- 後付款(TriLink後払い) --%>
				<dd id="ddTriLinkAfterPayPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY) %>" runat="server">
				</dd>

				<%-- 銀行振込（前払い） --%>
				<dd id="ddBankPre" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_BANK_PRE) %>" runat="server">
				</dd>

				<%-- 銀行振込（後払い） --%>
				<dd id="ddBankDef" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_BANK_DEF) %>" runat="server">
				</dd>

				<%-- 郵便振込（前払い） --%>
				<dd id="ddPostPre" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_POST_PRE) %>" runat="server">
				</dd>

				<%-- 郵便振込（後払い） --%>
				<dd id="ddPostDef" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_POST_DEF) %>" runat="server">
				</dd>

				<%-- ドコモケータイ払い --%>
				<dd id="ddDocomoPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_ORG) %>" runat="server">
					<div>
						<strong>【注意事項】</strong>
					</div>
					<div>
						決済には「i-mode対応」の携帯電話が必要です。<br />
						携帯電話のメールのドメイン指定受信を設定されている方は、必ず「<%: ShopMessage.GetMessage("ShopMailDomain") %>」を受信できるように設定してください。<br />
						1回の購入金額が<%: CurrencyManager.ToPrice(10000m) %>を超えてしまう場合はケータイ払いサービスをご利用いただけません。<br />
						i-mode」はＮＴＴドコモの商権、または登録商標です。<br />
					</div>
				</dd>

				<%-- S!まとめて支払い --%>
				<dd id="ddSMatometePayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_SMATOMETE_ORG) %>" runat="server">
				</dd>

				<%-- まとめてau支払い --%>
				<dd id="ddAuMatometePayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AUMATOMETE_ORG) %>" runat="server">
				</dd>

				<%-- ソフトバンク・ワイモバイルまとめて支払い(SBPS) --%>
				<dd id="ddSoftBankKeitaiSBPSPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS) %>" runat="server">
				</dd>

				<%-- auかんたん決済(SBPS) --%>
				<dd id="ddAuKantanSBPSPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS) %>" runat="server">
				</dd>

				<%-- ドコモケータイ払い(SBPS) --%>
				<dd id="ddDocomoKeitaiSBPSPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS) %>" runat="server">
				</dd>

				<%-- S!まとめて支払い(SBPS) --%>
				<dd id="ddSMatometeSBPSPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_SMATOMETE_SBPS) %>" runat="server">
				</dd>

				<%-- 代金引換 --%>
				<dd id="ddCollect" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT) %>" runat="server">
				</dd>

				<%-- PayPal --%>
				<dd id="ddPayPal" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL) %>" runat="server">
					<%if (SessionManager.PayPalCooperationInfo != null) {%>
						ご利用のPayPal アカウント：<br/>
						<b><%: SessionManager.PayPalCooperationInfo.AccountEMail %></b>
					<%} else {%>
						ご利用にはPayPalログインが必要です。
					<%} %>
					<uc:PaymentDescriptionPayPal runat="server" id="PaymentDescriptionPayPal" />
				</dd>

				<%-- Paidy --%>
				<dd id="ddPaidy" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY) %>" runat="server">
					<uc:PaidyCheckoutControl ID="ucPaidyCheckoutControl" runat="server" />
				</dd>

				<!-- NP後払い -->
				<dd id="ddNpAfterPay" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY) %>" runat="server">
					<uc:PaymentDescriptionNPAfterPay runat="server" id="PaymentDescriptionNPAfterPay" />
				</dd>

				<%-- LinePay --%>
				<dd id="ddLinePay" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY) %>" runat="server">
					<uc:PaymentDescriptionLinePay runat="server" id="PaymentDescriptionLinePay" />
				</dd>
			
				<!-- PayPay -->
				<dd id="ddPayPay" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY) %>" runat="server">
					<uc:PaymentDescriptionPayPay runat="server" id="PaymentDescriptionPayPay" />
				</dd>

				<%-- 決済なし --%>
				<dd id="ddNoPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT) %>" runat="server">
				</dd>

				<%-- atone翌月払い --%>
				<dd id="ddAtonePayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE) %>" runat="server">
				<uc:PaymentDescriptionAtone ID="PaymentDescriptionAtone" runat="server" />
			</dd>

				<%-- aftee翌月払い --%>
				<dd id="ddAfteePayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE) %>" runat="server">
				</dd>

				<%-- Ec Payment --%>
				<dd id="ddEcPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY) %>" runat="server">
					<strong>支払い方法</strong><br />
					<asp:DropDownList id="ddlEcPayment" runat="server"
						CssClass="input_border"
						DataSource="<%# ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE) %>"
						SelectedValue='<%# ((this.CartList.Items[0].Payment != null)
							&& (this.CartList.Items[0].Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY))
							? this.CartList.Items[0].Payment.ExternalPaymentType
							: Constants.FLG_PAYMENT_TYPE_ECPAY_CREDIT %>'
						DataTextField="text"
						DataValueField="value"
						AutoPostBack="true"
						OnSelectedIndexChanged="ddlEcPayment_SelectedIndexChanged" />
					<br />
					<asp:CheckBox ID="cbEcPayCreditInstallment"
						runat="server"
						Checked="<%# ((this.CartList.Items[0].Payment != null) && this.CartList.Items[0].Payment.IsPaymentEcPayWithCreditInstallment) %>"
						Visible="<%# IsDisplayEcPayCreditInstallment(this.CartList.Items[0].Payment) && (string.IsNullOrEmpty(Constants.ECPAY_PAYMENT_CREDIT_INSTALLMENT) == false) %>"
						Text="分割払い" />
				</dd>
				<%-- （DSK）後払い --%>
				<dd id="ddDskDef" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_DSK_DEF) %>" runat="server">
					コンビニ後払い（DSK）
				</dd>

				<%-- NewebPay --%>
				<dd id="ddNewebPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY) %>" runat="server">
					<strong>支払い方法<br /></strong>
					<asp:DropDownList id="ddlNewebPayment"
						runat="server"
						CssClass="input_border"
						DataSource='<%# ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE + "_neweb") %>'
						SelectedValue="<%# ((this.CartList.Items[0].Payment != null)
							&& (this.CartList.Items[0].Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY))
								? this.CartList.Items[0].Payment.ExternalPaymentType
								: Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT %>"
						DataTextField="text"
						DataValueField="value"
						AutoPostBack="true"
						OnSelectedIndexChanged="ddlNewebPayment_SelectedIndexChanged" />
					<div id="dvCreditInstallment" visible="<%# IsDisplayNewebPayCreditInstallment(this.CartList.Items[0].Payment) %>" runat="server">
					<strong>支払い回数<br /></strong>
					<asp:DropDownList id="ddlCreditInstallment"
						runat="server"
						DataSource="<%# this.NewebPayInstallmentsList %>"
						SelectedValue="<%# ((this.CartList.Items[0].Payment != null)
							&& (string.IsNullOrEmpty(this.CartList.Items[0].Payment.NewebPayCreditInstallmentsCode) == false))
								? this.CartList.Items[0].Payment.NewebPayCreditInstallmentsCode
								: Constants.FLG_ORDER_CARD_INSTALLMENTS_CODE_ONCE %>"
						DataTextField="Text"
						DataValueField="Value"
						CssClass="input_border" />
					</div>
				</dd>
				<%-- ID決済(wechatpay、aripay、キャリア決済) --%>
				<dd id="ddCarrierbillingBokuPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU) %>" runat="server">
					ID決済(wechatpay、aripay、キャリア決済)
				</dd>
				
				<%-- GMOアトカラ --%>
				<dd id="ddGmoAtokara" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA) %>" runat="server">
					アトカラ
				</dd>
			</ItemTemplate>
			<FooterTemplate>
				</dl>
			</FooterTemplate>
			</asp:Repeater>
		<div id="sErrorMessage2" class="attention" EnableViewState="False" runat="server"></div>
		</ContentTemplate>
		</asp:UpdatePanel>
		<%-- UPDATE PANELここまで(お支払い情報) --%>
		<% } else { %>
			<%--▼▼ Amazon Pay(CV2)お支払方法 ▼▼--%>
			<% if ((this.AmazonPaySessionPaymentDescriptor != null) && Constants.AMAZON_PAYMENT_CV2_ENABLED) { %>
			<div class="bottom">
			<div style="font-size: 14px;" class="text-center m-3">
				<img src="../../Contents/ImagesPkg/Amazon/logo-amzn_pay.png" height="20px" align="top">&nbsp;&nbsp;
				<%= this.AmazonPaySessionPaymentDescriptor %>
				<asp:LinkButton ID="lbChangePayment" ClientIDMode="Static" class="btn btn-mini"　style="text-decoration:none; margin:10px 0;" runat="server">お支払い方法を変更</asp:LinkButton>
			</div>
			</div>
			<% } else { %>
			<%--▲▲ Amazon Pay(CV2)お支払方法 ▲▲--%>
		<%-- ▼▼Amazon決済ウィジェット▼▼ --%>
		<div id="walletWidgetDiv"></div>
		<%-- ▲▲Amazon決済ウィジェット▲▲ --%>
		<% if ((this.CartList.Items.Count > 0) && this.CartList.Items[0].HasFixedPurchase) { %>
		<div style="margin: 10px 0;">下記のお支払い契約に同意してください。</div>
		<%-- ▼▼Amazon支払契約同意ウィジェット▼▼ --%>
		<div id="consentWidgetDiv" style="margin-top: 0.5em;"></div>
		<%-- ▲▲Amazon支払契約同意ウィジェット▲▲ --%>
		<div id="consentWidgetErrorMessage" style="color:red;padding:5px" ClientIDMode="Static" runat="server"></div>
		<% } %>
		<% } %>
		<% } %>
		</div>
		<%-- ▲お支払い情報▲ --%>
		<%-- ▼▼AmazonリファレンスID格納▼▼ --%>
		<asp:HiddenField runat="server" ID="hfAmazonOrderRefID"/>
		<%-- ▲▲AmazonリファレンスID格納▲▲ --%>
		<%-- ▼▼Amazon支払契約ID格納▼▼ --%>
		<asp:HiddenField runat="server" ID="hfAmazonBillingAgreementId"/>
		<%-- ▲▲Amazon支払契約ID格納▲▲ --%>
		<%-- ▼領収書情報▼ --%>
		<asp:UpdatePanel ID="upReceiptInfo" runat="server">
		<ContentTemplate>
		<% if (Constants.RECEIPT_OPTION_ENABLED) { %>
			<dl class="order-form receipt-form" id="divReceipt">
			<dt>領収書情報</dt>
			<dd id="divDisplayCanNotInputMessage" runat="server" visible="false" class="attention">指定したお支払い方法は、領収書の発行ができません。</dd>
			<dd id="divReceiptInfoInputForm" runat="server">
			<strong>領収書希望有無を選択してください。</strong>
			<dd><asp:DropDownList id="ddlReceiptFlg" runat="server" DataTextField="text" DataValueField="value" DataSource="<%# this.ReceiptFlgListItems %>"
				SelectedValue="<%# ((CartObject)((RepeaterItem)Container).DataItem).ReceiptFlg %>"
				OnSelectedIndexChanged="ddlReceiptFlg_OnSelectedIndexChanged" AutoPostBack="true" CssClass="input_border" />
			</dd>
			<div id="divReceiptAddressProviso" runat="server">
				<dd>宛名<span class="attention">※</span></dd>
				<dd>
					<asp:TextBox id="tbReceiptAddress" runat="server" Text="<%# ((CartObject)((RepeaterItem)Container).DataItem).ReceiptAddress %>" MaxLength="100" Width="450" />
					<p><asp:CustomValidator ID="cvReceiptAddress" runat="Server"
						ControlToValidate="tbReceiptAddress"
						ValidationGroup="ReceiptRegisterModify"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						ClientValidationFunction="ClientValidate"
						EnableClientScript="false"/></p>
				</dd>
				<dd>但し書き<span class="attention">※</span></dd>
				<dd>
					<asp:TextBox id="tbReceiptProviso" runat="server" Text="<%# ((CartObject)((RepeaterItem)Container).DataItem).ReceiptProviso %>" MaxLength="100" Width="450" />
					<p><asp:CustomValidator ID="cvReceiptProviso" runat="Server"
						ControlToValidate="tbReceiptProviso"
						ValidationGroup="ReceiptRegisterModify"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						ClientValidationFunction="ClientValidate"
						EnableClientScript="false"/></p>
				</dd>
			</div><!--divReceiptAddressProviso-->
			</dd><!--divReceiptInfoInputForm-->
			</dl><!--divReceipt-->
		<% } %>
		</ContentTemplate>
		</asp:UpdatePanel>
		<%-- ▲領収書情報▲ --%>
		<!-- 定期注文購入金額 -->
		<asp:UpdatePanel
			ID="upFixedPurchaseFutureOrder"
			Visible='<%# ((CartObject)((RepeaterItem)Container).DataItem).HasFixedPurchase
				&& this.SkipOrderConfirm %>'
			runat="server">
			<ContentTemplate>
				<uc:BodyFixedPurchaseOrderPrice
					Cart="<%# (CartObject)((RepeaterItem)Container).DataItem %>"
					runat="server" />
			</ContentTemplate>
		</asp:UpdatePanel>
		<!-- 定期注文購入金額 -->
		</ItemTemplate>
	</asp:Repeater>
	</ContentTemplate>
	</asp:UpdatePanel>

		<%-- ソーシャルログイン用 --%>
		<% if (Constants.SOCIAL_LOGIN_ENABLED) { %>
			<asp:HiddenField ID="hfSocialLoginJson" runat="server" />
		<% } %>

</ContentTemplate>
</asp:UpdatePanel>
<%-- UPDATE PANELここまで --%>

</div>
<% if (this.IsDispCorrespondenceSpecifiedCommericalTransactionsSkipOrderConfirm) { %>
	<dl class="specified-commercial-transactions">
		<dt style="background-color: #ccc;padding: 0.5em;line-height: 1.5">特商法に基づく表記</dt>
		<dd style="padding: 0.5em"><%= ShopMessage.GetMessageByPaymentId() %></dd> 
	</dl>
<% } %>
<div class="order-footer">
	<div class="button-next">
	<asp:UpdatePanel ID="UpdatePanel1" runat="server">
	<ContentTemplate>
		<% if (this.DisplayNextBtn) { %>
		<a onclick="<%= this.NextOnClick %>" href="<%: this.NextEvent %>" class="btn">
			<%: ((Constants.LANDING_CART_USER_REGISTER_WHEN_ORDER_COMPLETE == false) && (this.WcbUserRegister.Checked && (this.IsAmazonLoggedIn == false) && (this.IsLoggedIn == false)
				|| (this.WcbUserRegisterForExternalSettlement.Checked && (this.IsUserRegistedForAmazon == false) && this.IsAmazonLoggedIn && (this.ExistsUserWithSameAmazonEmailAddress == false))))
				? this.SkipOrderConfirm
					? "会員登録して注文を確定する"
					: IsTargetToExtendedAmazonAddressManagerOption()
						? "会員登録してご注文内容の確認へ"
						: "会員登録して次へ進む"
				: (IsTargetToExtendedAmazonAddressManagerOption() && this.WcbUserRegister.Checked)
					? "会員登録してご注文内容の確認へ"
					: this.SkipOrderConfirm ? "注文を確定する" : "次へ進む" %>
		</a>
		<a id="lbNextToConfirm" href="<%: this.NextEvent %>" style="display:none;"></a>
		<% } %>
		<% if (this.IsCartListLp) { %>
		<br />
		<div class="button-prev">
			<a href="<%: Constants.PATH_ROOT %>" class="btn">ショッピングを続ける</a>
		</div>
		<% } %>
	</ContentTemplate>
	</asp:UpdatePanel>
	</div>
</div>
</section>
</div>
<uc:SendAuthenticationCodeModal runat="server" ID="ucSendAuthenticationCodeModal" />

<%--▼▼ クレジットカードToken用スクリプト ▼▼--%>
<script type="text/javascript">
	var getTokenAndSetToFormJs = "<%= CreateGetCreditTokenAndSetToFormJsScript().Replace("\"", "\\\"") %>";
	var maskFormsForTokenJs = "<%= CreateMaskFormsForCreditTokenJsScript().Replace("\"", "\\\"") %>";
</script>
<uc:CreditToken runat="server" ID="CreditToken" />
<%--▲▲ クレジットカードToken用スクリプト ▲▲--%>

<%--▼▼ Paidy用スクリプト ▼▼--%>
<script type="text/javascript">
	var buyer = SetPaidyBuyer();
	var hfPaidyTokenIdControlId = "<%= this.WhfPaidyTokenId.ClientID %>";
	var hfPaidyPaySelectedControlId = "<%= this.WhfPaidyPaySelected.ClientID %>";
	var lbNextProcess = "lbNextToConfirm";
	var isHistoryPage = false;

	function SetPaidyBuyer() {
		<% if (this.IsLoggedIn == false) { %>
			var ownerMailAddr = $('[id$=tbOwnerMailAddr]').val();
			var name = $('[id$=tbOwnerName1]').val()
				+ $('[id$=tbOwnerName2]').val();
			var ownerNameKana = $('[id$=tbOwnerNameKana1]').val()
				+ $('[id$=tbOwnerNameKana2]').val();
			var ownerTel1 = $('[id$=tbOwnerTel1_1]').val()
				+ $('[id$=tbOwnerTel1_2]').val()
				+ $('[id$=tbOwnerTel1_3]').val();

			buyer = { email: ownerMailAddr, name1: name, name2: ownerNameKana, phone: ownerTel1 };
			return buyer;
		<% } else { %>
			return <%= PaidyUtility.CreatedBuyerDataObjectForPaidyPayment(this.CartList) %>;
		<% } %>
	}
</script>
<uc:PaidyCheckoutScript ID="ucPaidyCheckoutScript" runat="server" />
<%--▲▲ Paidy用スクリプト ▲▲--%>

<script type="text/javascript" src="<%= Constants.PATH_ROOT %>Js/SocialPlusInputCompletion.js?<%: Constants.QUERY_STRING_FOR_UPDATE_EXTERNAL_FILE_URLENCODED %>"></script>
<script type="text/javascript">
<!--
	bindEvent();

	<%-- UpdataPanelの更新時のみ処理を行う --%>
	function bodyPageLoad() {
		if (Sys.WebForms == null) return;
		focus_ddlProductSet();
		var isAsyncPostback = Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();
		if (isAsyncPostback) {
			bindEventForUpdate();
		}
	}

	<%-- 商品セット選択ドロップダウンリストにフォーカスが当たっている場合はアンカー移動 --%>
	function focus_ddlProductSet() {
		var ddlProductSetDom = $('#<%= this.WddlProductSet.ClientID %>');
		if (ddlProductSetDom != null) {
			if (ddlProductSetDom.is(':focus')) {
				location.href = "#" + ddlProductSetDom.attr('id');
			} else if (ddlProductSetDom.is(':focus') == false
				&& location.href == "#" + ddlProductSetDom.attr('id')) {
				location.hash = "";
			}
		}
	}

	<% if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED) { %>
	function HandleVisibility() {
		var cartCount = <%= this.CartList.Items.Count %>;
		for (var i = 0; i < cartCount ; i++) {
			var elements = document.getElementsByClassName(i)[0];
			if(typeof elements != 'undefined')
			{
				if((elements.querySelector('[id$="hfCvsShopId"]') != null) && (elements.querySelector('[id$="hfCvsShopId"]').value == ''))
				{
					$(elements.querySelector('[id$="ddCvsShopId"]')).hide();
					$(elements.querySelector('[id$="ddCvsShopName"]')).hide();
					$(elements.querySelector('[id$="ddCvsShopAddress"]')).hide();
					$(elements.querySelector('[id$="ddCvsShopTel"]')).hide();
				}
				else
				{
					$(elements.querySelector('[id$="ddCvsShopId"]')).show();
					$(elements.querySelector('[id$="ddCvsShopName"]')).show();
					$(elements.querySelector('[id$="ddCvsShopAddress"]')).show();
					$(elements.querySelector('[id$="ddCvsShopTel"]')).show();
				}
			}
		}
	}

	<%-- Check Before Next Page --%>
	function CheckBeforeNextPage() {
		var hasError = false;
		<% foreach (RepeaterItem ri in rCartList.Items) { %>
		var shippingKbn = $('#<%= ((DropDownList)ri.FindControl("ddlShippingKbnList")).ClientID %>').val();
		if (shippingKbn == '<%= CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE %>') {
			var shopNo = $('#<%= ((HiddenField)ri.FindControl("hfCvsShopId")).ClientID %>').val();

			var element = document.getElementById('spErrorConvenienceStore' + '<%= ri.ItemIndex %>');
			if (shopNo == '') {
				element.innerHTML = 'コンビニが選択されていません';

				hasError = true;
			}
			else {
				element.innerHTML = '';
			}
		}
		<%} %>

		<% if (Constants.PAYMENT_PAIDY_OPTION_ENABLED) { %>
		if (hasError == false) {
			PaidyPayProcess();
			return false;
		}
		<% } %>

		return (hasError == false);
	}

	var selectedCartIndex = 0;
	<%-- Open convenience store map popup --%>
	function openConvenienceStoreMapPopup(cartIndex) {
		selectedCartIndex = cartIndex;

		var url = '<%= OrderCommon.CreateConvenienceStoreMapUrl(true) %>';
		window.open(url, "", "width=1000,height=800");
	}

	<%-- Set convenience store data --%>
	function setConvenienceStoreData(cvsspot, name, addr, tel, shopNum) {
		var elements = document.getElementsByClassName(selectedCartIndex)[0];

		// For display
		elements.querySelector('[id$="liCvsShopId"] > span').innerHTML = cvsspot;
		elements.querySelector('[id$="liCvsShopName"] > span').innerHTML = name;
		elements.querySelector('[id$="liCvsShopAddress"] > span').innerHTML = addr;
		elements.querySelector('[id$="liCvsShopTel"] > span').innerHTML = tel;

		// For get value
		elements.querySelector('[id$="hfCvsShopId"]').value = cvsspot;
		elements.querySelector('[id$="hfCvsShopName"]').value = name;
		elements.querySelector('[id$="hfCvsShopAddress"]').value = addr;
		elements.querySelector('[id$="hfCvsShopTel"]').value = tel;

		elements.querySelector('[id$="ddCvsShopId"]').style.removeProperty('display');
		elements.querySelector('[id$="ddCvsShopName"]').style.removeProperty('display');
		elements.querySelector('[id$="ddCvsShopAddress"]').style.removeProperty('display');
		elements.querySelector('[id$="ddCvsShopTel"]').style.removeProperty('display');

		var element = document.getElementById('spErrorConvenienceStore' + selectedCartIndex);
		element.innerHTML = '';
	}
	<% } %>

	<%-- イベントをバインドする --%>
	<% var serializer = new JavaScriptSerializer(); %>
	function bindEvent() {
		bindExecAutoKana();
		bindExecAutoChangeKana();
		bindZipCodeSearch();
		bindTwAddressSearch();
		<% if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED) { %>
		HandleVisibility();
		<% } %>
		<% if (this.IsEfoOptionEnabled) { %>
		var customValidatorControlDisabledInformationList = <%= serializer.Serialize(this.CustomValidatorControlDisabledInformationList) %>
		bindRemoveCustomValidateErrorOnInputChangeValue(customValidatorControlDisabledInformationList);
		<% } else { %>
		var customValidatorControlInformationList = <%= serializer.Serialize(this.CustomValidatorControlInformationList) %>
		bindRemoveCustomValidateErrorWhenNoErrorDisplay(customValidatorControlInformationList);
		<% } %>
	}

	<%-- イベントをバインドする（更新用） --%>
	function bindEventForUpdate() {
		bindExecAutoKanaForUpdate();
		bindExecAutoChangeKanaForUpdate();
		bindZipCodeSearch();
		bindTwAddressSearch();
		<% if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED) { %>
		HandleVisibility();
		<% } %>
		<% if (this.IsEfoOptionEnabled) { %>
		var customValidatorControlDisabledInformationList = <%= serializer.Serialize(this.CustomValidatorControlDisabledInformationList) %>
		bindRemoveCustomValidateErrorOnInputChangeValue(customValidatorControlDisabledInformationList);
		<% } else { %>
		var customValidatorControlInformationList = <%= serializer.Serialize(this.CustomValidatorControlInformationList) %>
		bindRemoveCustomValidateErrorWhenNoErrorDisplay(customValidatorControlInformationList);
		<% } %>
	}

	<%-- 氏名（姓・名）の自動振り仮名変換のイベントをバインドする --%>
	function bindExecAutoKana() {
		<% foreach (RepeaterItem ri in rCartList.Items) { %>
			execAutoKanaWithKanaType(
				$('#<%= ((TextBox)ri.FindControl("tbOwnerName1")).ClientID %>'),
				$('#<%= ((TextBox)ri.FindControl("tbOwnerNameKana1")).ClientID %>'),
				$('#<%= ((TextBox)ri.FindControl("tbOwnerName2")).ClientID %>'),
				$('#<%= ((TextBox)ri.FindControl("tbOwnerNameKana2")).ClientID %>'));
			execAutoKanaWithKanaType(
				$('#<%= ((TextBox)ri.FindControl("tbShippingName1")).ClientID %>'),
				$('#<%= ((TextBox)ri.FindControl("tbShippingNameKana1")).ClientID %>'),
				$('#<%= ((TextBox)ri.FindControl("tbShippingName2")).ClientID %>'),
				$('#<%= ((TextBox)ri.FindControl("tbShippingNameKana2")).ClientID %>'));
		<%} %>
	}

	<%-- 氏名（姓・名）の自動振り仮名変換のイベントをバインドする（更新用） --%>
	function bindExecAutoKanaForUpdate() {
		<% foreach (RepeaterItem ri in rCartList.Items) { %>
			execAutoKanaWithKanaType(
				$('#<%= ((TextBox)ri.FindControl("tbShippingName1")).ClientID %>'),
				$('#<%= ((TextBox)ri.FindControl("tbShippingNameKana1")).ClientID %>'),
				$('#<%= ((TextBox)ri.FindControl("tbShippingName2")).ClientID %>'),
				$('#<%= ((TextBox)ri.FindControl("tbShippingNameKana2")).ClientID %>'));
		<%} %>
	}

	<%-- ふりがな（姓・名）のかな←→カナ自動変換イベントをバインドする --%>
	function bindExecAutoChangeKana() {
		<% foreach (RepeaterItem ri in rCartList.Items) { %>
			execAutoChangeKanaWithKanaType(
				$('#<%= ((TextBox)ri.FindControl("tbOwnerNameKana1")).ClientID %>'),
				$('#<%= ((TextBox)ri.FindControl("tbOwnerNameKana2")).ClientID %>'));
			execAutoChangeKanaWithKanaType(
				$('#<%= ((TextBox)ri.FindControl("tbShippingNameKana1")).ClientID %>'),
				$('#<%= ((TextBox)ri.FindControl("tbShippingNameKana2")).ClientID %>'));
		<%} %>
	}

	<%-- ふりがな（姓・名）のかな←→カナ自動変換イベントをバインドする（更新用） --%>
	function bindExecAutoChangeKanaForUpdate() {
		<% foreach (RepeaterItem ri in rCartList.Items) { %>
			execAutoChangeKanaWithKanaType(
				$('#<%= ((TextBox)ri.FindControl("tbShippingNameKana1")).ClientID %>'),
				$('#<%= ((TextBox)ri.FindControl("tbShippingNameKana2")).ClientID %>'));
		<%} %>
	}

	var multiAddrsearchTriggerType = "";
	<%-- 郵便番号検索のイベントをバインドする --%>
	function bindZipCodeSearch() {
		<% foreach (RepeaterItem ri in rCartList.Items) { %>
		// Check owner zip code input on click
		clickSearchZipCodeInRepeaterForSp(
			'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZip").ClientID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZip1").ClientID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZip2").ClientID %>',
				'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchOwnergAddr").ClientID %>',
				'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchOwnergAddr").UniqueID %>',
					'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON %>',
				'<%= '#' + (ri.FindControl("sOwnerZipError")).ClientID %>',
				"owner");

		// Check owner zip code input on text box change
			textboxChangeSearchZipCodeInRepeaterForSp(
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZip").ClientID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZip1").ClientID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZip2").ClientID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZip1").UniqueID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZip2").UniqueID %>',
				'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchOwnergAddr").ClientID %>',
					'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON %>',
				'<%= '#' + (ri.FindControl("sOwnerZipError")).ClientID %>',
				"owner");

		// Check shipping zip code input on click
			clickSearchZipCodeInRepeaterForSp(
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbShippingZip").ClientID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbShippingZip1").ClientID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbShippingZip2").ClientID %>',
				'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchShippingAddr").ClientID %>',
				'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchShippingAddr").UniqueID %>',
					'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON %>',
				'<%= '#' + (ri.FindControl("sShippingZipError")).ClientID %>',
				"shipping");

		// Check shipping zip code input on text box change
			textboxChangeSearchZipCodeInRepeaterForSp(
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbShippingZip").ClientID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbShippingZip1").ClientID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbShippingZip2").ClientID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbShippingZip1").UniqueID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbShippingZip2").UniqueID %>',
				'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchShippingAddr").ClientID %>',
					'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON %>',
				'<%= '#' + (ri.FindControl("sShippingZipError")).ClientID %>',
				"shipping");

			<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
			// Textbox change search owner zip global
			textboxChangeSearchGlobalZip(
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZipGlobal").ClientID %>',
				'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchAddrOwnerFromZipGlobal").UniqueID %>');

			// Textbox change search shipping zip global
			textboxChangeSearchGlobalZip(
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbShippingZipGlobal").ClientID %>',
				'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchAddrShippingFromZipGlobal").UniqueID %>');
			<% } %>
		<% } %>
	}

	$(document).on('click', '.search-result-layer-close', function () {
		closePopupAndLayer();
	});

	$(document).on('click', '.search-result-layer-addr', function () {
		bindSelectedAddr($('li.search-result-layer-addr').index(this), multiAddrsearchTriggerType);
	});

	<%-- 複数住所検索結果からの選択値を入力フォームにバインドする --%>
	function bindSelectedAddr(selectedIndex, multiAddrsearchTriggerType) {
		var selectedAddr = $('.search-result-layer-addrs li').eq(selectedIndex);
		if (multiAddrsearchTriggerType == "owner") {
			<% foreach (RepeaterItem ri in rCartList.Items) { %>
			$('#<%= ((DropDownList)ri.FindControl("ddlOwnerAddr1")).ClientID %>').val(selectedAddr.find('.addr').text());
			$('#<%= ((TextBox)ri.FindControl("tbOwnerAddr2")).ClientID %>').val(selectedAddr.find('.city').text() + selectedAddr.find('.town').text());
			$('#<%= ((TextBox)ri.FindControl("tbOwnerAddr3")).ClientID %>').focus();
			<%} %>
		} else if (multiAddrsearchTriggerType == "shipping") {
			<% foreach (RepeaterItem ri in rCartList.Items) { %>
			$('#<%= ((DropDownList)ri.FindControl("ddlShippingAddr1")).ClientID %>').val(selectedAddr.find('.addr').text());
			$('#<%= ((TextBox)ri.FindControl("tbShippingAddr2")).ClientID %>').val(selectedAddr.find('.city').text() + selectedAddr.find('.town').text());
			$('#<%= ((TextBox)ri.FindControl("tbShippingAddr3")).ClientID %>').focus();
			<%} %>
		}

		closePopupAndLayer();
	}
	<%-- ソーシャルログイン用 --%>
	<% if (Constants.SOCIAL_LOGIN_ENABLED) { %>
	var selectors = {
		mailAddress: '#<%= this.WtbOwnerMailAddr.ClientID %>',
			mailAddressConf: '#<%= this.WtbOwnerMailAddrConf.ClientID %>',
			tell1: '#<%= this.WtbOwnerTel1_1.ClientID %>',
			tell2: '#<%= this.WtbOwnerTel1_2.ClientID %>',
			tell3: '#<%= this.WtbOwnerTel1_3.ClientID %>',
			name1: '#<%= this.WtbOwnerName1.ClientID %>',
			name2: '#<%= this.WtbOwnerName2.ClientID %>',
			nameKanaCheck: '<%= ReplaceTag("@@User.name_kana.type@@") %>',
			nameKana1: '#<%= this.WtbOwnerNameKana1.ClientID %>',
			nameKana2: '#<%= this.WtbOwnerNameKana2.ClientID %>',
			productreviewEnabled: false,
			birthYear: '#<%= this.WddlOwnerBirthYear.ClientID %>',
			birthMonth: '#<%= this.WddlOwnerBirthMonth.ClientID %>',
			birthDay: '#<%= this.WddlOwnerBirthDay.ClientID %>',
			sex: '#<%= this.WrblOwnerSex.ClientID %> input:radio',
			zip1: '#<%= this.WtbOwnerZip1.ClientID %>',
			zip2: '#<%= this.WtbOwnerZip2.ClientID %>',
			address1: '#<%= this.WddlOwnerAddr1.ClientID %>',
			address2: '#<%= this.WtbOwnerAddr2.ClientID %>',
			address3: '#<%= this.WtbOwnerAddr3.ClientID %>',
			tell: '#<%= this.WtbOwnerTel1.ClientID %>',
			zip: '#<%= this.WtbOwnerZip.ClientID %>'
		}

		var data = $('#<%= this.WhfSocialLoginJson.ClientID %>').val();
	var json = data ? JSON.parse(data) : {};
	SocialPlusInputCompletion(json, selectors);
	<% } %>

	<%-- 台湾郵便番号取得関数 --%>
	function bindTwAddressSearch() {
		<% foreach (RepeaterItem ri in rCartList.Items) { %>
		$('#<%= ((DropDownList)ri.FindControl("ddlShippingAddr3")).ClientID %>').change(function (e) {
			$('#<%= ((TextBox)ri.FindControl("tbShippingZipGlobal")).ClientID %>').val(
				$('#<%= ((DropDownList)ri.FindControl("ddlShippingAddr3")).ClientID %>').val())
		});
		<%} %>
	}

	<% if (this.AuthenticationUsable) { %>
	// Set authentication message
	function setAuthenticationMessage() {
		var isAddrJp = true;
		<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
		isAddrJp = document.getElementById('<%= this.WddlOwnerCountry.ClientID %>').value == '<%= Constants.COUNTRY_ISO_CODE_JP %>';
		<% } %>
		var authenticationStatusId = isAddrJp
			? '<%= this.WlbAuthenticationStatus.ClientID %>'
			: '<%= this.WlbAuthenticationStatusGlobal.ClientID %>';
		var authenticationMessageId = isAddrJp
			? '<%= this.WlbAuthenticationMessage.ClientID %>'
			: '<%= this.WlbAuthenticationMessageGlobal.ClientID %>';
		var phoneNumber = document.getElementById(isAddrJp
			? '<%= this.WtbOwnerTel1.ClientID %>'
			: '<%= this.WtbOwnerTel1Global.ClientID %>').value;

		setIntervalAuthenticationMessage(
			authenticationStatusId,
			authenticationMessageId,
			phoneNumber,
			'<%= Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_AUTH_CODE_DIGITS %>',
			'<%= Constants.PERSONAL_AUTHENTICATION_OF_USERR_EGISTRATION_AUTH_CODE_EXPIRATION_TIME %>')
	}

	// Check tel no input
	function checkTelNoInput() {
		var result = checkTelNo(
			'<%= this.WtbOwnerTel1_1.ClientID %>',
			'<%= this.WtbOwnerTel1_2.ClientID %>',
			'<%= this.WtbOwnerTel1_3.ClientID %>',
			'<%= this.WtbOwnerTel1.ClientID %>',
			'<%= this.WtbOwnerTel1Global.ClientID %>');
		return result;
	}
	<% } %>
	//-->
</script>

<% if(Constants.AMAZON_PAYMENT_CV2_ENABLED) { %>
	<%--▼▼ Amazon Pay(CV2)スクリプト ▼▼--%>
	<script src="https://static-fe.payments-amazon.com/checkout.js"></script>
	<script type="text/javascript" charset="utf-8">
		$(function () {
			var process = function () {
		<% if (this.IsAmazonLoggedIn == false) { %>
		showAmazonPayCv2Button(
			'#AmazonPayCv2Button',
			'<%= Constants.PAYMENT_AMAZON_SELLERID %>',
			<%= Constants.PAYMENT_AMAZON_ISSANDBOX.ToString().ToLower() %>,
			$('#<%: this.WhfAmazonCv2Payload.ClientID %>').val(),
			$('#<%: this.WhfAmazonCv2Signature.ClientID %>').val(),
			'<%= Constants.PAYMENT_AMAZON_PUBLIC_KEY_ID %>');
		<% } %>
		<% if (this.IsAmazonLoggedIn && (this.AmazonCheckoutSessionId != null)) { %>
		if ($("#lbChangeAddress").length > 0) {
			amazon.Pay.bindChangeAction('#lbChangeAddress',
				{
					amazonCheckoutSessionId: '<%= this.AmazonCheckoutSessionId %>',
					changeAction: 'changeAddress'
				});
		}
		if ($("#lbChangePayment").length > 0) {
			amazon.Pay.bindChangeAction('#lbChangePayment',
				{
					amazonCheckoutSessionId: '<%= this.AmazonCheckoutSessionId %>',
					changeAction: 'changePayment'
				});
		}
		<% } %>
			};
			process();
			if (Sys && Sys.Application) { Sys.Application.add_load(process); }
		});
	</script>
	<%--▲▲ Amazon Pay(CV2)スクリプト ▲▲--%>
<% } else { %>
<%--▼▼Amazonウィジェット用スクリプト▼▼--%>
<script type="text/javascript">
	$(function () {
		var process = function () {
	<%-- HACK：SmartPhoneの場合は画面レンダリング後でないとウィジェットが描画されない --%>
	window.onload = function() {
		$('#shippingAddressBookContainer').css('display', '<%= ((this.IsLoggedIn == false) && (this.CartList.Items.Count != 0) && this.CartList.Items[0].Shippings[0].IsSameShippingAsCart1 ) ? "none" : "block" %>');
	};
	window.onAmazonLoginReady = function () {
		amazon.Login.setClientId('<%=Constants.PAYMENT_AMAZON_CLIENTID %>');
	};
	window.onAmazonPaymentsReady = function () {
		if ($('#AmazonPayButton').length) showButton();
		showAddressBookWidget();
	};
	<%-- Amazonボタン表示ウィジェット --%>
	<% if (this.IsAmazonLoggedIn == false) { %>
	function showButton() {
		var authRequest;
		OffAmazonPayments.Button("AmazonPayButton", "<%=Constants.PAYMENT_AMAZON_SELLERID %>", {
			type: "PwA",
			color: "Gold",
			size: "medium",
			authorization: function () {
				loginOptions = {
					scope: "payments:widget payments:shipping_address profile",
					popup: true,
					state: '<%= Request.RawUrl %>'
				};
				authRequest = amazon.Login.authorize(loginOptions, "<%=w2.App.Common.Amazon.Util.AmazonUtil.CreateCallbackUrl(Constants.PAGE_FRONT_AMAZON_LANDING_PAGE_CALLBACK) %>");
			},
			onError: function (error) {
				alert(error.getErrorMessage());
			}
		});
	};
	<% } %>

	<%-- Amazonアドレス帳表示ウィジェット --%>
	function showAddressBookWidget() {
		<%-- Amazon配送先情報 --%>
		new OffAmazonPayments.Widgets.AddressBook({
			sellerId: '<%=Constants.PAYMENT_AMAZON_SELLERID %>',
			<% if ((this.CartList.Items.Count > 0) && this.CartList.Items[0].HasFixedPurchase){ %>
			agreementType: 'BillingAgreement',
			<% } %>
			onReady: function (arg) {
				<% if ((this.CartList.Items.Count > 0) && this.CartList.Items[0].HasFixedPurchase){ %>
				var billingAgreementId = arg.getAmazonBillingAgreementId();
				$('#<%=this.WhfAmazonBillingAgreementId.ClientID %>').val(billingAgreementId);
				<% } else { %>
				var orderReferenceId = arg.getAmazonOrderReferenceId();
				$('#<%=this.WhfAmazonOrderRefID.ClientID %>').val(orderReferenceId);
				<% } %>
				showWalletWidget(arg);
				showConsentWidget(arg);
			},
			onAddressSelect: function (orderReference) {
				var $shippingAddressBookErrorMessage = $('#<%# this.WhgcShippingAddressBookErrorMessage.ClientID %>');
				$shippingAddressBookErrorMessage.empty();
				getAmazonAddress('<%=((this.CartList.Items.Count > 0) && this.CartList.Items[0].HasFixedPurchase) ? w2.App.Common.Amazon.AmazonConstants.OrderType.AutoPay : w2.App.Common.Amazon.AmazonConstants.OrderType.OneTime %>', '<%= w2.App.Common.Amazon.AmazonConstants.AddressType.Shipping %>', function (response) {
					var data = JSON.parse(response.d);
					if (typeof data.RequestPostBack !== "undefined") location.href = $("#<%= (lbPostBack != null) ? lbPostBack.ClientID : "" %>").attr('href');
					if (data.Error) $shippingAddressBookErrorMessage.html(data.Error);
				});
			},
			design: { designMode: 'smartphoneCollapsible' },
			onError: function (error) {
				var message = error.getErrorMessage();
				switch (error.getErrorCode()) {
					case 'UnknownError':
						message = 'エラーが発生しました。\r\n時間を空けて再度お試しください。';
						break;
				}
				alert(message);
			}
		}).bind("shippingAddressBookWidgetDiv");

		<%-- Amazon注文者情報 --%>
		new OffAmazonPayments.Widgets.AddressBook({
			sellerId: '<%=Constants.PAYMENT_AMAZON_SELLERID %>',
			<% if ((this.CartList.Items.Count > 0) && this.CartList.Items[0].HasFixedPurchase){ %>
			agreementType: 'BillingAgreement',
			onReady: function (arg) {
				<% if ((this.CartList.Items.Count > 0) && this.CartList.Items[0].HasFixedPurchase){ %>
				var billingAgreementId = arg.getAmazonBillingAgreementId();
				$('#<%=this.WhfAmazonBillingAgreementId.ClientID %>').val(billingAgreementId);
				<% } else { %>
				var orderReferenceId = arg.getAmazonOrderReferenceId();
				$('#<%=this.WhfAmazonOrderRefID.ClientID %>').val(orderReferenceId);
				<% } %>
			},
			<% } %>
			onAddressSelect: function (orderReference) {
				var $ownerAddressBookErrorMessage = $('#<%# this.WhgcOwnerAddressBookErrorMessage.ClientID %>');
				$ownerAddressBookErrorMessage.empty();
				getAmazonAddress('<%=((this.CartList.Items.Count > 0) && this.CartList.Items[0].HasFixedPurchase) ? w2.App.Common.Amazon.AmazonConstants.OrderType.AutoPay : w2.App.Common.Amazon.AmazonConstants.OrderType.OneTime %>', '<%= w2.App.Common.Amazon.AmazonConstants.AddressType.Owner %>', function (response) {
					var data = JSON.parse(response.d);
					if (($("#<%= WcbShipToOwnerAddress.ClientID %>").prop('checked')) && (typeof data.RequestPostBack !== "undefined")) location.href = $("#<%= (lbPostBack != null) ? lbPostBack.ClientID : "" %>").attr('href');
					if (data.Error) $ownerAddressBookErrorMessage.html(data.Error);
				});
			},
			design: { designMode: 'smartphoneCollapsible' },
			onError: function (error) {
				var message = error.getErrorMessage();
				switch (error.getErrorCode()) {
					case 'UnknownError':
						message = 'エラーが発生しました。\r\n時間を空けて再度お試しください。';
						break;
				}
				alert(message);
			}
		}).bind("ownerAddressBookWidgetDiv");
	}

	<%-- Amazon決済方法表示ウィジェット --%>
	function showWalletWidget(arg) {
		new OffAmazonPayments.Widgets.Wallet({
			sellerId: '<%=Constants.PAYMENT_AMAZON_SELLERID %>',
			<% if ((this.CartList.Items.Count > 0) && this.CartList.Items[0].HasFixedPurchase){ %>
			agreementType: 'BillingAgreement',
			amazonBillingAgreementId: arg.getAmazonBillingAgreementId(),
			<% } %>
			design: { designMode: 'smartphoneCollapsible' },
			onError: function (error) {
				alert(error.getErrorMessage());
			}
		}).bind("walletWidgetDiv");
	}

	<%-- Amazon支払契約同意ウィジェット --%>
	function showConsentWidget(arg) {
		new OffAmazonPayments.Widgets.Consent({
			sellerId: '<%=Constants.PAYMENT_AMAZON_SELLERID %>',
			amazonBillingAgreementId: arg.getAmazonBillingAgreementId(),
			onConsent: function (billingAgreementConsentStatus) {
				buyerBillingAgreementConsentStatus = billingAgreementConsentStatus.getConsentStatus();
				if (buyerBillingAgreementConsentStatus) {
					$('#consentWidgetErrorMessage').empty();
				}
			},
			design: { designMode: 'smartphoneCollapsible' },
			onError: function (error) {
				alert(error.getErrorMessage());
			}
		}).bind("consentWidgetDiv");
	}

	<%-- Amazon住所取得関数 --%>
	function getAmazonAddress(orderType, addressType, callback) {
		$.ajax({
			type: "POST",
			url: "<%= Request.FilePath %>/GetAmazonAddress",
			contentType: "application/json; charset=utf-8",
			dataType: "json",
			data: JSON.stringify({
				orderReferenceIdOrBillingAgreementId: $('#<%=((this.CartList.Items.Count > 0) && this.CartList.Items[0].HasFixedPurchase) ? this.WhfAmazonBillingAgreementId.ClientID : this.WhfAmazonOrderRefID.ClientID %>').val(),
				orderType: orderType,
				addressType: addressType
			}),
			success: callback
		});
	}
		};
		window.onload = function() { process(); };
		if (Sys && Sys.Application) { Sys.Application.add_load(process); }
	});
</script>
<script async="async" type="text/javascript" charset="utf-8" src="<%: Constants.PAYMENT_AMAZON_WIDGETSSCRIPT %>"></script>
<%-- ▲▲Amazonウィジェット用スクリプト▲▲ --%>
<% } %>
<input type="hidden" id="fraudbuster" name="fraudbuster" />
<script type="text/javascript" src="//cdn.credit.gmo-ab.com/psdatacollector.js"></script>
<uc:RakutenPaymentScript ID="ucRakutenPaymentScript" runat="server" />
<script type="text/javascript">
	// ドロップダウンリスト、ラジオボタンが初期値に戻らないようにページ読込完了時に値を再設定する（注文者情報）
	$(window).bind('load', function () {
		UpdateDdlAndRbl(
			'<%= this.WddlOwnerBirthYear.ClientID %>',
			'<%= this.WddlOwnerBirthMonth.ClientID %>',
			'<%= this.WddlOwnerBirthDay.ClientID %>',
			'<%= this.WrblOwnerSex.ClientID %>',
			'<%= this.WddlOwnerCountry.ClientID %>',
			<%= this.WddlOwnerBirthYear.SelectedIndex %>,
			<%= this.WddlOwnerBirthMonth.SelectedIndex %>,
			<%= this.WddlOwnerBirthDay.SelectedIndex %>,
			'<%= this.WrblOwnerSex.SelectedValue %>',
			<%= this.WddlOwnerCountry.SelectedIndex %>,
			'<%= this.WddlOwnerAddr1.ClientID %>',
			<%= this.WddlOwnerAddr1.SelectedIndex %>
		);
	});
</script>
<%--▼▼ O-MOTION用スクリプト ▼▼--%>
<script type="text/javascript">
	var setOmotionClientIdJs;
	var webMethodUrl;
	$(function () {
		setOmotionClientIdJs = "<%= CreateSetOmotionClientIdJsScript().Replace("\"", "\\\"") %>";
		webMethodUrl = "<%= Constants.PATH_ROOT + "SmartPhone/" + Constants.PAGE_FRONT_LOGIN %>";
	});
</script>
<uc:UserLoginScript runat="server" ID="ucUserLoginScript" />
<%--▲▲ O-MOTION用スクリプト ▲▲--%>

<script>
	var subscriptionBoxMessage = '「@@ 1 @@」は「@@ 2 @@」の必須商品です。\n削除すると、「@@ 2 @@」の申し込みがキャンセルされます。\nよろしいですか？';

	// 必須商品チェック(頒布会)
	function delete_product_check_for_subscriptionBox(element) {
		var productDiv = findParentWithClass(element, 'cart-unit');
		if (productDiv == null) {
			return false;
		}

		var subscriptionBoxProductName = $(productDiv).find("[id$='hfProductName']").val();
		var subscriptionBoxName = $(productDiv).find("[id$='hfSubscriptionBoxCourseName']").val();

		subscriptionBoxMessage = subscriptionBoxMessage.replace('@@ 1 @@', subscriptionBoxProductName);
		subscriptionBoxMessage = subscriptionBoxMessage.replace(/@@ 2 @@/g, subscriptionBoxName);

		return confirm(subscriptionBoxMessage);
	}

	// 指定のクラスを持つ親要素を検索
	function findParentWithClass(element, className) {
		while (element && (element.classList.contains(className) == false)) {
			element = element.parentElement;
		}
		return element;
	}
</script>
