<%--
=========================================================================================================
  Module      : LP入力フォーム(LpInputForm.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="Layer" Src="~/Form/Common/Layer/SearchResultLayer.ascx" %>
<%@ Register TagPrefix="uc" TagName="UserRegistRegulationMessage" Src="~/Form/User/UserRegistRegulationMessage.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionPayPal" Src="~/Form/Common/Order/PaymentDescriptionPayPal.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaypalScriptsForm" Src="~/Form/Common/PayPalScriptsForm.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyAnnounceFreeShipping" Src="~/Form/Common/BodyAnnounceFreeShipping.ascx" %>
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
<%@ Register TagPrefix="uc" TagName="RakutenCreditCard" Src="~/Form/Common/RakutenCreditCardModal.ascx" %>
<%@ Register TagPrefix="uc" TagName="RakutenPaymentScript" Src="~/Form/Common/RakutenPaymentScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="MailDomains" Src="~/Form/Common/MailDomains.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyFixedPurchaseOrderPrice" Src="~/Form/Common/BodyFixedPurchaseOrderPrice.ascx" %>
<%@ Register Src="~/Form/Common/UserLoginScript.ascx" TagPrefix="uc" TagName="UserLoginScript" %>
<%@ Register Src="~/Form/Common/SendAuthenticationCodeModal.ascx" TagPrefix="uc" TagName="SendAuthenticationCodeModal" %>
<%-- ▼削除禁止：クレジットカードTokenコントロール▼ --%>
<%@ Register TagPrefix="uc" TagName="CreditToken" Src="~/Form/Common/CreditToken.ascx" %>
<%-- ▲削除禁止：クレジットカードTokenコントロール▲ --%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LpInputForm.ascx.cs" Inherits="Landing_formlp_LpInputForm" %>
<%@ Import Namespace="w2.Domain.LandingPage" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.Paidy" %>
<%@ Import Namespace="w2.App.Common.Amazon" %>
<%@ Import Namespace="Extensions" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>

<% if (this.IsCartListLp) { %>
	<p id="CartFlow"><img src="../../Contents/ImagesPkg/order/cart_lp_step00.gif" alt="カート内容確認 " width="781" height="58" /></p>
<% } %>

<% if(Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED) { %>
<style>
.convenience-store-item {
	margin: 2px;
	padding: 4px;
}

.convenience-store-button {
	padding: 12px 17px;
	font-size: 16px;
	text-decoration: none !important;
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
		.SubscriptionOptionalItem .dvSubscriptionOptional{background-color: #cccccc; height: 60px; width: 500px!important;}
		.SubscriptionOptionalItem .subscriptionMessage { font-size: 18px;  text-align: center; position: relative; top: 15px;height: 20px;}
	</style>
<% } %>
<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
<uc:EcPayScript runat="server" ID="ucECPayScript" />
<% } %>

<div class="formlp-front-section-form">
<asp:UpdatePanel runat="server" UpdateMode="Conditional">
	<ContentTemplate>
		<asp:DropDownList ID="ddlProductSet" CssClass="input_border" style="width: 500px; margin:10px;" OnSelectedIndexChanged="ddlProductSet_OnSelectedIndexChanged" runat="server" DataTextField="Text" DataValueField="Value" AutoPostBack="true"></asp:DropDownList>
	</ContentTemplate>
</asp:UpdatePanel>
<div id="CartList">
	<% lbNext.ValidationGroup = this.IsLoggedIn ? "OrderShipping-OrderPayment" : "OrderShippingGuest-OrderPayment"; %>
	<asp:LinkButton ID="lbNext" OnClick="lbNext_Click" ValidationGroup="OrderShipping-OrderPayment" runat="server"></asp:LinkButton>
	<%-- UPDATE PANEL開始 --%>
	<asp:UpdatePanel ID="UpdatePanel" UpdateMode="Conditional" ChildrenAsTriggers="false" runat="server">
	<ContentTemplate>
	<asp:UpdatePanel runat="server" ID="upSocialLogin">
	<ContentTemplate>
	<% if (this.Process.IsSubscriptionBoxError == false){ %>
	<% if (this.CanUseAmazonPaymentForFront() && this.CheckAmazonPaymentLandingPageDesignLimit()) { %>
		<% if (this.IsAmazonLoggedIn == false) { %>
			<%-- ▼▼Amazonボタンウィジェット▼▼ --%>
			<div id="AmazonPayButton" style="width:200px;margin-left:auto;"></div>
			<div style="margin-left:auto;width:200px;">
				<%--▼▼ Amazon Pay(CV2)ボタン ▼▼--%>
				<div id="AmazonPayCv2Button"></div>
				<%--▲▲ Amazon Pay(CV2)ボタン ▲▲--%>

				<asp:HiddenField ID="hfAmazonCv2Payload" Value="<%# this.AmazonRequest.Payload %>" runat="server" />
				<asp:HiddenField ID="hfAmazonCv2Signature" Value="<%# this.AmazonRequest.Signature %>" runat="server" />
			</div>
			<%-- ▲▲Amazonボタンウィジェット▲▲ --%>
		<% } else { %>
			<div style="text-align:right;"><asp:LinkButton id="lbCancelAmazonPay" runat="server" class="btn btn-large" OnClick="lbCancelAmazonPay_Click">Amazonお支払いをやめる</asp:LinkButton></div>
		<% } %>
	<% } %>
	<% } %>

	<span style="color: red; text-align: left; display: block"><asp:Literal ID="lPaymentErrorMessage" runat="server"></asp:Literal></span>
	<%-- ▼PayPalログインここから▼ --%>
	<% if (Constants.PAYPAL_LOGINPAYMENT_ENABLED && DisplaySocialLoginBtnCheck(LandingPageConst.SocialLoginType.PayPal) && this.CanUsePayPalPayment()) { %>
		<%
			ucPaypalScriptsForm.LogoDesign = "Payment";
			ucPaypalScriptsForm.AuthCompleteActionControl = lbPayPalAuthComplete;
			ucPaypalScriptsForm.GetShippingAddress = (this.IsLoggedIn == false);
		%>
		<uc:PaypalScriptsForm ID="ucPaypalScriptsForm" runat="server" />
		<% if (this.Process.IsSubscriptionBoxError == false){ %>
		<div id="paypal-button" style="height: 25px"></div>
		<%if (SessionManager.PayPalCooperationInfo != null) {%>
			<%: (SessionManager.PayPalCooperationInfo != null) ? SessionManager.PayPalCooperationInfo.AccountEMail : "" %> 連携済
		<%} %>
		<br /><asp:LinkButton ID="lbPayPalAuthComplete" runat="server" OnClick="lbPayPalAuthComplete_Click"></asp:LinkButton>
		<%} %>
	<%} %>
	<%-- ▲PayPalログインここまで▲ --%>

	</ContentTemplate>
	</asp:UpdatePanel>
	<asp:UpdatePanel id="upCartListItemsCount" runat="server">
	<ContentTemplate>
	<% if (this.IsCartListLp) { %>
		<div class="cartstep">
			<h2 class="ttlB">
				カート内容確認 <%: (this.CartList.Items.Count > 1) ? ("(合計" + this.CartList.Items.Count + "カート)") : "" %>
			</h2>
		</div>
	<% } %>
	<% if (this.IsCartListLp && (this.CartList.Items.Count == 0)) { %>
		カートに商品がありません。
	<% } %>
	<% if (this.ErrorMessages.HasMessages(-1, -1)) { %>
		<p><span style="color:red"><%: this.ErrorMessages.Get(-1, -1) %></span></p>
	<% } %>
	</ContentTemplate>
	</asp:UpdatePanel>

	<asp:UpdatePanel id="upInitializeCartItemIndexTmp" runat="server">
	<ContentTemplate>
	<% this.CartItemIndexTmp = -1; %>
	</ContentTemplate>
	</asp:UpdatePanel>
	<%-- ▼商品リスト▼ --%>
	<asp:UpdatePanel ID="upProductList" runat="server">
	<ContentTemplate>
	<% if (this.IsDropDownList) { %>
	<h3>商品を選択してください</h3>
	<asp:DropDownList ID="ddlProductList" CssClass="input_border" style="width: 500px" OnSelectedIndexChanged="ddlProductList_OnSelectedIndexChanged" runat="server" DataTextField="Text" DataValueField="Value" AutoPostBack="true"></asp:DropDownList>
	<% } else if (this.IsSelectFromList) { %>
	<div class="productList">
		<p class="ttl"><img src="<%: Constants.PATH_ROOT %>Contents/ImagesPkg/cartlist/<%: this.ProductIncludedTaxFlg ? this.Process.IsSubscriptionBoxFixedAmount == false ? "sttl03.gif" : "sttl04.gif" : "sttl02.gif" %>" alt="商品名  単価（<%: this.ProductPriceTextPrefix %>）  注文数  小計（<%: this.ProductPriceTextPrefix %>）" width="847" height="12" /></p>
		<div class="product">
			<asp:Repeater ID="rProductList" ItemType="LandingCartProduct" runat="server">
			<ItemTemplate>
				<div>
				<dl class="name">
				<dt><w2c:ProductImage ID="ProductImage1" ProductMaster="<%# Item.Product %>" ImageSize="M" runat="server" /></dt>
				<dd><%# WebSanitizer.HtmlEncode(Item.ProductJointName) %></dd>
				</dl>
				<% if(this.Process.IsSubscriptionBoxFixedAmount == false){ %>
				<p class="price" Visible="<%# this.Process.IsSubscriptionBoxFixedAmount == false %>" runat="server"><%#: CurrencyManager.ToPrice(Item.Price) %>(<%#: this.ProductPriceTextPrefix %>)</p>
				<% } %>
				<p class="quantity"><asp:TextBox ID="tbProductCount" Runat="server" Text='<%# Item.ProductCount %>' MaxLength="3"></asp:TextBox></p>
				<% if(this.Process.IsSubscriptionBoxFixedAmount == false){ %>
				<p class="taxRate" Visible="<%# this.Process.IsSubscriptionBoxFixedAmount == false %>" runat="server"><%#: TaxCalculationUtility.GetTaxRateForDIsplay(Item.TaxRate) %>%</p>
				<p class="subtotal" Visible="<%# this.Process.IsSubscriptionBoxFixedAmount == false %>" runat="server"><%#: CurrencyManager.ToPrice(Item.Price * Item.ProductCount) %>(<%#: this.ProductPriceTextPrefix %>)</p>
				<% } %>
				<% if (this.IsCheckBox) { %>
				<p class="delete"><asp:CheckBox ID="cbPurchase" Checked="<%# Item.Selected %>" Enabled="<%# CanNecessaryProducts(this.Process.SubscriptionBoxCourseId, Item.VariationId, false) == false %>" runat="server" /></p>
				<% } %>
				<p class="clr"><img src="<%: Constants.PATH_ROOT %>Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1" /></p>
				</div>
			</ItemTemplate>
			</asp:Repeater>
		</div>
	</div>
	<% } %>
	</ContentTemplate>
	</asp:UpdatePanel>
	<%-- 隠し再計算ボタン --%>
	<asp:LinkButton id="lbCreateCart" runat="server" onclick="lbCreateCart_Click"></asp:LinkButton>
	<%-- ▲商品リスト▲ --%>
	<%-- UPDATE PANEL開始(カートリスト) --%>
	<asp:UpdatePanel ID="upCartListUpdatePanel" UpdateMode="Conditional" ChildrenAsTriggers="False" runat="server">
	<ContentTemplate>
	<asp:Repeater ID="rCartList" runat="server">
	<ItemTemplate>
		<%-- UPDATE PANEL開始(商品一覧) --%>
		<asp:UpdatePanel ID="upProductListUpdatePanel" runat="server">
		<ContentTemplate>
		<%-- ▼商品一覧▼ --%>
		<div class="productList">
		<div class="background">
		<asp:PlaceHolder ID="phSubscriptionBoxErrorMsg" runat="server" Visible="<%# string.IsNullOrEmpty(((CartObject)((IDataItemContainer)Container).DataItem).SubscriptionBoxErrorMsg) == false %>">
			<span style="color:red"><%# ((CartObject)((IDataItemContainer)Container).DataItem).SubscriptionBoxErrorMsg %></span>
		</asp:PlaceHolder>
		<asp:HiddenField ID="hfSubscriptionBoxCourseId" runat="server" Value="<%#((CartObject)((IDataItemContainer)Container).DataItem).SubscriptionBoxCourseId%>" />
		<h3 style="clear: both;" runat="server">カート番号 <%# ((RepeaterItem)Container).ItemIndex + 1 %> のご注文内容</h3>
		<div id="Div1" class="list" runat="server">
			<p class="ttl">
				<div runat="server"
					visible="<%# ((CartObject)((IDataItemContainer)Container).DataItem).IsSubscriptionBox
						&& (((CartObject)((IDataItemContainer)Container).DataItem).IsSubscriptionBoxFixedAmount == false) %>">
					<% if(Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false) { %>
					<img src="<%: Constants.PATH_ROOT %>Contents/ImagesPkg/cartlist/<%# this.ProductIncludedTaxFlg ? "sttl02IsSubscriptionBox.gif" : "sttl03IsSubscriptionBox.gif" %>" alt="商品名  単価（<%#: this.ProductPriceTextPrefix %>）  初回商品数  小計（<%#: this.ProductPriceTextPrefix %>）" width="847" height="12" />
					<% } else { %>
						<span style="padding-left:100px; padding-right:60px;">商品名</span>
						<span style="padding-left: 215px; padding-right: 50px;">単価(<%#:this.ProductPriceTextPrefix %>)</span>
						<span Visible="<%# HasProductOptionPrice(((CartObject)((IDataItemContainer)Container).DataItem)) %>" runat="server" style="padding-left: 0px; padding-right: 30px;">オプション価格(<%#:this.ProductPriceTextPrefix %>)</span>
						<span style="padding-left: 10px;padding-right: 30px;">注文数</span>
						<span style="padding-left: 5px;padding-right: 20px;">消費税率</span>
						<span style="padding-left: 15px;padding-right: 0px;">小計(<%#:this.ProductPriceTextPrefix %>)</span>
					<% } %>
				</div>
				<div runat="server"
					visible="<%# ((CartObject)((IDataItemContainer)Container).DataItem).IsSubscriptionBox == false %>">
					<% if(Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false) { %>
						<img src="<%: Constants.PATH_ROOT %>Contents/ImagesPkg/cartlist/<%# this.ProductIncludedTaxFlg ? "sttl03.gif" : "sttl02.gif" %>" alt="商品名  単価（<%#: this.ProductPriceTextPrefix %>）  注文数  小計（<%#: this.ProductPriceTextPrefix %>）" width="847" height="12" />
					<% } else { %>
						<span style="padding-left:100px; padding-right:60px;">商品名</span>
						<span style="padding-left: 215px; padding-right: 50px;">単価(<%#:this.ProductPriceTextPrefix %>)</span>
						<span Visible="<%# HasProductOptionPrice(((CartObject)((IDataItemContainer)Container).DataItem)) %>" runat="server" style="padding-left: 0px; padding-right: 30px;">オプション価格(<%#:this.ProductPriceTextPrefix %>)</span>
						<span style="padding-left: 10px;padding-right: 30px;">注文数</span>
						<span style="padding-left: 5px;padding-right: 20px;">消費税率</span>
						<span style="padding-left: 15px;padding-right: 0px;">小計(<%#:this.ProductPriceTextPrefix %>)</span>
					<% } %>
				</div>
				<div runat="server"
					visible="<%# (((CartObject)((IDataItemContainer)Container).DataItem).IsSubscriptionBox)
						&& (((CartObject)((IDataItemContainer)Container).DataItem).IsSubscriptionBoxFixedAmount) %>">
					<span style="padding-left: 120px; padding-right: 203px;">商品名</span>
					<span style="padding-left: 120px; padding-right: 203px;" runat="server">頒布会コース名</span>
					<span style="padding-left: 30px;padding-right: 24px;">初回商品数</span>
				</div>
			</p>
			<span class="fred"><asp:Literal ID="lMemberRankError" runat="server"></asp:Literal></span>
		<asp:Repeater id="rCart" runat="server" DataSource='<%# (CartObject)((RepeaterItem)Container).DataItem %>' OnItemCommand="rCartList_ItemCommand">
		<ItemTemplate>
			<%-- 通常商品 --%>
			<div id="Div2" class="product" visible="<%# ((CartProduct)Container.DataItem).IsSetItem == false && ((CartProduct)Container.DataItem).QuantitiyUnallocatedToSet != 0 %>" runat="server">
			<%-- 隠し値 --%>
			<asp:HiddenField ID="hfShopId" runat="server" Value="<%# ((CartProduct)Container.DataItem).ShopId %>" />
			<asp:HiddenField ID="hfProductId" runat="server" Value="<%# ((CartProduct)Container.DataItem).ProductId %>" />
			<asp:HiddenField ID="hfVariationId" runat="server" Value="<%# ((CartProduct)Container.DataItem).VariationId %>" />
			<asp:HiddenField ID="hfIsFixedPurchase" runat="server" Value="<%# ((CartProduct)Container.DataItem).IsFixedPurchase %>" />
			<asp:HiddenField ID="hfProductSaleId" runat="server" Value="<%# ((CartProduct)Container.DataItem).ProductSaleId %>" />
			<asp:HiddenField ID="hfProductOptionValue" runat="server" Value='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues() %>' />
			<asp:HiddenField ID="hfUnallocatedQuantity" runat="server" Value='<%# ((CartProduct)Container.DataItem).QuantitiyUnallocatedToSet %>' />
			<asp:HiddenField ID="hfSubscriptionBoxCourseId" runat="server" Value='<%# ((CartProduct)Container.DataItem).SubscriptionBoxCourseId %>' />
			<asp:HiddenField ID="hfSubscriptionBoxCourseName" runat="server" Value="<%# GetSubscriptionBoxDisplayName(((CartProduct)Container.DataItem).SubscriptionBoxCourseId) %>" />
			<asp:HiddenField ID="hfAddCartKbn" runat="server" Value="<%# ((CartProduct)Container.DataItem).AddCartKbn %>" />
			<input type="hidden" id="hfProductName" value="<%# ((CartProduct)Container.DataItem).ProductName %>" />
			<div>
				<dl class="name">
				<dt><w2c:ProductImage ID="ProductImage2" ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" /></dt>
					<dd><%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %></dd>
					<dd visible='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList.IsSelectedProductOptionValueAll %>' runat="server">
						<asp:Repeater ID="rProductOptionSettings" ItemType="w2.App.Common.Product.ProductOptionSetting" DataSource='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList %>' runat="server">
							<ItemTemplate>
								<%#: Item.GetDisplayProductOptionSettingSelectValue() %>
								<%# (Item.GetDisplayProductOptionSettingSelectValue() != "") ? "<br />" : "" %>
							</ItemTemplate>
						</asp:Repeater>
					</dd>
					<dd Visible="<%# this.SkipOrderConfirm %>" style="font-weight:normal;"　runat="server">
						<div Visible="<%# ((CartProduct)Container.DataItem).IsDisplaySell && ((CartProduct.IsDisplaySubscriptionBoxSelectTime(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)) == false) %>" class="specifiedcommercialtransactionsLpCartSellTimeName" runat="server">販売期間：<br/></div>
						<div Visible="<%# ((CartProduct)Container.DataItem).IsDisplaySell && ((CartProduct.IsDisplaySubscriptionBoxSelectTime(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)) == false) %>" class="specifiedcommercialtransactionsLpCartSellTime" runat="server">
							<span class="specifiedcommercialtransactionsCampaignTime"><%#: DateTimeUtility.ToStringFromRegion(((CartProduct)Container.DataItem).SellFrom, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime) %>～<br/></span>
							<span class="specifiedcommercialtransactionsCampaignTime"><%#: DateTimeUtility.ToStringFromRegion(((CartProduct)Container.DataItem).SellTo, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime) %></span>
						</div>
						<%--頒布会の選択可能期間--%>
						<div Visible="<%# (CartProduct.IsDisplaySubscriptionBoxSelectTime(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)) %>" class="specifiedcommercialtransactionsLpCampaignTimeName" runat="server" >販売期間</div>
						<div Visible="<%# (CartProduct.IsDisplaySubscriptionBoxSelectTime(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)) %>" class="specifiedcommercialtransactionsLpCampaignTime" runat="server" >
							<%# WebSanitizer.HtmlEncodeChangeToBr((CartProduct.GetSubscriptionBoxSelectTermBr(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)))%>
						</div>
						<div Visible="<%# ((CartProduct)Container.DataItem).IsDispSaleTerm %>" class="specifiedcommercialtransactionsLpCartSellTimeName" runat="server">タイムセール期間:<br/></div>
						<div Visible="<%# ((CartProduct)Container.DataItem).IsDispSaleTerm %>" class="specifiedcommercialtransactionsLpCartSellTime" runat="server">
							<span class="specifiedcommercialtransactionsCampaignTime"><%#: (ProductCommon.GetProductSaleTermBrLpBeginDate(this.ShopId, ((CartProduct)Container.DataItem).ProductSaleId)) %>～<br/></span>
							<span class="specifiedcommercialtransactionsCampaignTime"><%#: (ProductCommon.GetProductSaleTermBrLpEndDate(this.ShopId, ((CartProduct)Container.DataItem).ProductSaleId)) %></span>
						</div>
						<div Visible="<%# ((((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount()) == false) && (CartProduct.IsSubscriptionBoxCampaignPeriod(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)) %>" class="specifiedcommercialtransactionsLpCampaignTimeName" runat="server" >キャンペーン期間：</div>
						<div Visible="<%# ((((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount()) == false) && (CartProduct.IsSubscriptionBoxCampaignPeriod(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)) %>" class="specifiedcommercialtransactionsLpCampaignTime" runat="server" >
							<%# WebSanitizer.HtmlEncodeChangeToBr((CartProduct.GetSubscriptionBoxTermBr(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)))%>
						</div>
					</dd>
					<dd visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
						<span style="color:red;">※配送料無料適用外商品です</span>
					</dd>
				</dl>
				<p class="price" runat="server" Visible="<%# (((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount()) == false %> " style="padding-left: 110px;"><%#: CurrencyManager.ToPrice(((CartProduct)Container.DataItem).Price) %>(<%#: this.ProductPriceTextPrefix %>)</p>
				<p id="pOptionPrice" style="width:160px;text-align:center;float:left;padding-top:25px;" runat="server" Visible="<%# ((((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount()) == false && HasProductOptionPrice((CartObject)((Repeater)Container.Parent).DataSource)) %> "> <%#:(((CartProduct)Container.DataItem).ProductOptionSettingList.HasOptionPrice == false)? "ー" : CurrencyManager.ToPrice(((CartProduct)Container.DataItem).ProductOptionSettingList.SelectedOptionTotalPrice) +"("+ this.ProductPriceTextPrefix+")" %></p>
				<p class="quantity" style="padding-left: 210px; padding-right: 215px;" Visible="<%# ((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() %>" runat="server"><%#: ((CartProduct)Container.DataItem).GetSubscriptionDisplayName() %></p>
				<p class="quantity" <%= Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED ? "style=\"" + "width:90px;text-align:center;float:left;padding-top:25px;" + "\"" : string.Empty %> ><asp:TextBox ID="tbProductCount" Runat="server" Text='<%# ((CartProduct)Container.DataItem).QuantitiyUnallocatedToSet %>' MaxLength="3"></asp:TextBox></p>
				<p class="taxRate" runat="server" Visible="<%# (((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount()) == false %> "><%#: TaxCalculationUtility.GetTaxRateForDIsplay(((CartProduct)Container.DataItem).TaxRate) %>%</p>
				<p class="subtotal" runat="server" Visible="<%# (((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount()) == false %> " style="padding-left: 10px;"><%#: CurrencyManager.ToPrice(((CartProduct)Container.DataItem).PriceIncludedOptionPrice * ((CartProduct)Container.DataItem).QuantitiyUnallocatedToSet) %>(<%#: this.ProductPriceTextPrefix %>)</p>
				<p class="delete" visible="<%# CanDeleteProduct((CartProduct)Container.DataItem) %>" runat="server">
					<asp:LinkButton ID="lbDeleteProduct" CommandName="DeleteProduct" Runat="server" Visible="<%# HasNecessaryProduct(((CartProduct)Container.DataItem).SubscriptionBoxCourseId, ((CartProduct)Container.DataItem).ProductId) == false %>" Text="削除" />
					<asp:LinkButton ID="lbDeleteNecessaryProduct" OnClientClick="return delete_product_check_for_subscriptionBox(this);" Runat="server" Visible="<%# HasNecessaryProduct(((CartProduct)Container.DataItem).SubscriptionBoxCourseId, ((CartProduct)Container.DataItem).ProductId) %>" CommandName="DeleteNecessarySubscriptionProduct" Text="削除" />
				</p>
			</div>
			<p class="clr"><img src="<%: Constants.PATH_ROOT %>Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
			<%-- ▽商品付帯情報▽ --%>
			<div visible="<%# this.IsCartListLp == false %>" runat="server">
				<asp:Repeater ID="rProductOptionValueSettings" DataSource='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList %>' runat="server">
				<ItemTemplate>
					<%# WebSanitizer.HtmlEncode(((ProductOptionSetting)Container.DataItem).ValueName) %>
					<span id="productOptionNecessary" class="necessary" style="color: red;" runat="server" visible="<%# ((ProductOptionSetting)Container.DataItem).IsNecessary %>">*</span>
					<asp:Repeater ID="rCblProductOptionValueSetting" DataSource='<%# ((ProductOptionSetting)Container.DataItem).SettingValuesListItemCollection %>' ItemType="System.Web.UI.WebControls.ListItem" Visible='<%# ((ProductOptionSetting)Container.DataItem).DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_CHECKBOX || ((ProductOptionSetting)Container.DataItem).DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_PRICE_CHECKBOX %>' runat="server" >
					<ItemTemplate>
						<asp:CheckBox AutoPostBack="True" ID="cbProductOptionValueSetting" OnCheckedChanged="cbProductOptionValueSettingListOnCheckedChanged" Text='<%# Item.Text %>' Checked='<%# Item.Selected %>' runat="server" />
					</ItemTemplate>
					</asp:Repeater>
					<asp:DropDownList　OnSelectedIndexChanged="cbProductOptionValueSettingListOnCheckedChanged"　AutoPostBack="True" ID="ddlProductOptionValueSetting" DataSource='<%# InsertDefaultAtFirstToDdlProductOptionSettingList(((ProductOptionSetting)Container.DataItem).SettingValuesListItemCollection, ((ProductOptionSetting)Container.DataItem).IsNecessary) %>' visible='<%# ((ProductOptionSetting)Container.DataItem).DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_SELECTMENU || ((ProductOptionSetting)Container.DataItem).DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_PRICE_DROPDOWNMENU %>' SelectedValue='<%# ((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectedValue() %>' runat="server" />
					<asp:TextBox ID ="txtProductOptionValueSetting"
						Text='<%# (string.IsNullOrEmpty(((ProductOptionSetting)Container.DataItem).SelectedSettingValue) == false)
							          ? ((ProductOptionSetting)Container.DataItem).SelectedSettingValue : ((ProductOptionSetting)Container.DataItem).DefaultValue %>' visible='<%# ((ProductOptionSetting)Container.DataItem).IsTextBox %>' runat="server" />
					<br />
				</ItemTemplate>
				</asp:Repeater>
				<small style="color: red;" class="fred pdg_rightA" visible='<%# this.ErrorMessages.HasMessages(GetParentRepeaterItem(Container, "rCartList").ItemIndex, Container.ItemIndex) %>' runat="server">
					<%# WebSanitizer.HtmlEncode(this.ErrorMessages.Get(GetParentRepeaterItem(Container, "rCartList").ItemIndex, Container.ItemIndex)).Replace("\r\n", "<br />") %>
				</small>
			</div>
			<%-- △商品付帯情報△ --%>
			</div><!--product-->
			
			<% if (this.IsCartListLp) { %>
			<%-- セット商品 --%>
			<div class="product" visible="<%# (((CartProduct)Container.DataItem).IsSetItem) && (((CartProduct)Container.DataItem).ProductSetItemNo == 1) %>" runat="server">
			<%-- 隠し値 --%>
			<asp:HiddenField ID="hfIsSetItem" runat="server" Value="<%# ((CartProduct)Container.DataItem).IsSetItem %>" />
			<asp:HiddenField ID="hfProductSetId" runat="server" Value="<%# OrderPage.GetProductSetId((CartProduct)Container.DataItem) %>" />
			<asp:HiddenField ID="hfProductSetNo" runat="server" Value="<%# OrderPage.GetProductSetNo((CartProduct)Container.DataItem) %>" />
			<asp:HiddenField ID="hfProductSetItemNo" runat="server" Value="<%# ((CartProduct)Container.DataItem).ProductSetItemNo %>" />
			<div>
			<asp:Repeater id="rProductSet" ItemType="w2.App.Common.Order.CartProduct" DataSource="<%# (((CartProduct)Container.DataItem).ProductSet != null) ? ((CartProduct)Container.DataItem).ProductSet.Items : null %>" OnItemCommand="rCartList_ItemCommand" runat="server">
			<HeaderTemplate>
				<table cellpadding="0" cellspacing="0" width="950" summary="ショッピングカート">
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
				<td class="name">
				<dl>
				<dt>
					<w2c:ProductImage ID="ProductImage4" ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" />
				</dt>
				<dd>
					<span>
						<%#: (Item.ProductJointName + " x " + Item.CountSingle) %>
						<%#: (Item.GetProductTag("tag_cart_product_message").Length != 0) ? ("<br/><p class=\"message\">" + Item.GetProductTag("tag_cart_product_message") + "</p>") : "" %>
					</span>
				</dd>
				<dd visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
					<span style="color:red;">※配送料無料適用外商品です</span>
				</dd>
				</dl>
				<p class="price"><%#: CurrencyManager.ToPrice(Item.Price) %> (<%#: this.ProductPriceTextPrefix %>)</p>
				</td>
				<td visible="<%# (Item.ProductSetItemNo == 1) %>" rowspan="<%# (Item.ProductSet != null) ? Item.ProductSet.Items.Count : 1 %>" class="quantity" runat="server">
					<asp:TextBox ID="tbProductSetCount" Runat="server" Text='<%# OrderPage.GetProductSetCount(Item) %>' MaxLength="3" CssClass="orderCount"></asp:TextBox></td>
				<td class="taxRate" runat="server">
					<%#: TaxCalculationUtility.GetTaxRateForDIsplay(Item.TaxRate) %>%</td>
				<td visible="<%# (Item.ProductSetItemNo == 1) %>" rowspan="<%# (Item.ProductSet != null) ? Item.ProductSet.Items.Count : 1 %>" class="subtotal" runat="server">
					<%#: CurrencyManager.ToPrice(OrderPage.GetProductSetPriceSubtotal(Item)) %> (<%#: this.ProductPriceTextPrefix %>)</td>
				<td visible="<%# (Item.ProductSetItemNo == 1) %>" rowspan="<%# (Item.ProductSet != null) ? Item.ProductSet.Items.Count : 1 %>" class="delete" runat="server">
					<asp:LinkButton ID="lbDeleteProductSet" CommandName="DeleteProductSet" Runat="server">削除</asp:LinkButton></td>
				</tr>
			</ItemTemplate>
			<FooterTemplate>
				</table>
			</FooterTemplate>
			</asp:Repeater>
			</div>
			<small class="fred pdg_leftA" visible='<%# this.ErrorMessages.HasMessages(GetParentRepeaterItem(Container, "rCartList").ItemIndex, Container.ItemIndex) %>' runat="server">
				<%#: this.ErrorMessages.Get(GetParentRepeaterItem(Container, "rCartList").ItemIndex, Container.ItemIndex) %>
			</small>
			</div>
			<% } %>
			<%-- ▲商品一覧▲ --%>
		</ItemTemplate>
		</asp:Repeater><!--rCart-->

		<div class="SubscriptionOptionalItem" id="dvListProduct" runat="server" visible="<%# this.HasOptionalProdects %>" style="width:800px;">
			<table cellspacing="0">
			<div runat="server" Visible="<%# CanNecessaryProducts(((CartObject)((RepeaterItem)Container).DataItem).SubscriptionBoxCourseId) %>">
				<tr>
					<td class="title_bg" colspan="6">
						<div style="display: inline-block;" runat="server" Visible="<%# this.SubscriputionBoxProductListModify.Any() %>">
							<p style="display: inline-block;">頒布会選択商品</p>
						</div>
						<div class="right" style=" text-align: right; display: inline-block;">
							<asp:Button ID="btnChangeProduct" Text="選択する" runat="server" OnClick="btnChangeProduct_Click" class="btn" />
						</div>
						<div class="dvSubscriptionOptional" runat="server" Visible="<%# this.SubscriputionBoxProductListModify.Any() == false %>">
							<p class="subscriptionMessage">頒布会商品を選択可能です</p>
						</div>
					</td>
				</tr>
				
				</div>
				<asp:Repeater Visible="<%# this.SubscriputionBoxProductListModify.Any() %>" DataSource="<%# this.SubscriputionBoxProductListModify %>" ItemType="w2.Domain.SubscriptionBox.SubscriptionBoxDefaultItemModel" runat="server" OnItemDataBound="rItem_OnItemDataBound">
					<HeaderTemplate>
						<tr class="rtitle">
							<th class="productName" colspan="2">
								商品名
							</th>
							<% if(this.Process.IsSubscriptionBoxFixedAmount == false){ %>
							<th class="productPrice">
								単価（<%#: this.ProductPriceTextPrefix %>）
							</th>
							<% } %>
							<th class="orderCount">
								注文数
							</th>
							<% if(this.Process.IsSubscriptionBoxFixedAmount == false){ %>
							<th class="orderSubtotal">
								小計（<%#: this.ProductPriceTextPrefix %>）
							</th>
							<% } %>
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
		<div class="SubscriptionOptionalItem" id="dvModifySubscription" runat="server" visible="<%# this.HasOptionalProdects == false %>" style="width:800px;" >
			<div>
				<small ID="sErrorQuantity" class="error" runat="server"></small>
			</div>
			<table cellspacing="0">
				<tr>
					<td class="title_bg" colspan="6">
						<p style="display: inline-block;">頒布会任意商品の選択</p>
						<div class="right" style="text-align: right;" >
							<asp:Button Text="  商品追加  " runat="server" class="btn" style="margin-right: 10px; display: inline-block;" OnClick="btnAddProduct_Click" CommandArgument="<%# ((CartObject)((RepeaterItem)Container).DataItem).SubscriptionBoxCourseId %>"/>
							<asp:Button Text="  決定  " runat="server" class="btn" style="display: inline-block;" OnClick="btnUpdateProduct_Click" CommandArgument="<%# ((CartObject)((RepeaterItem)Container).DataItem).SubscriptionBoxCourseId %>"/>
						</div>
					</td>
				</tr>
				<asp:Repeater ID="rItemModify" DataSource="<%# this.SubscriputionBoxProductList %>" ItemType="w2.Domain.SubscriptionBox.SubscriptionBoxDefaultItemModel" runat="server" OnItemDataBound="rItem_OnItemDataBound" OnItemCommand="rProductChange_ItemCommand">
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
							</td>
							<td class="productPrice">
								<%#: CurrencyManager.ToPrice(SubscriptionBoxPrice(Item.ProductId, Item.VariationId, 1)) %>
							</td>
							<td class="orderCount">
								<asp:TextBox  ID="tbQuantityUpdate" runat="server" Text="<%# StringUtility.ToNumeric(Item.ItemQuantity) %>" OnTextChanged="ReCalculation" AutoPostBack="True" MaxLength="3" />
							</td>
							<td class="orderSubtotal">
								<%#: CurrencyManager.ToPrice(SubscriptionBoxPrice(Item.ProductId, Item.VariationId, Item.ItemQuantity)) %>
							</td>
							<td class="orderDelete">
								<asp:LinkButton Text="x" runat="server" CommandName="DeleteRow" CommandArgument='<%# Container.ItemIndex %>' />
							</td>
						</tr>
					</ItemTemplate>
				</asp:Repeater>
			</table>
		</div>

		<%-- 頒布会コース内容確認 --%>
		<div class="toggle-wrap"
			runat="server"
			visible="<%# (string.IsNullOrEmpty(((CartObject)((IDataItemContainer)Container).DataItem).SubscriptionBoxCourseId) == false)
			&& Constants.SUBSCRIPTION_BOX_OPTION_ENABLED %>">
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
											<table>
												<tr>
													<td style="width: 541px; text-align: left;">
														<div style="margin-left: 70px">
															商品名
														</div>
													</td>
													<td style="width: 331px; text-align: left;">
														<div style="margin-left: 10px" Visible="<%# this.SubscriptionBox.FixedAmountFlg == Constants.FLG_SUBSCRIPTIONBOX_FIXED_AMOUNT_FALSE %>" runat="server">
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

										<asp:Repeater ID="rProductsList" Visible="<%# CheckTakeOverProduct(GetItemsModel((string)Container.DataItem)) == false %>" ItemType="w2.Domain.SubscriptionBox.SubscriptionBoxDefaultItemModel" DataSource='<%# GetItemsModel((string)Container.DataItem) %>' runat="server" OnItemDataBound="rProductsList_OnItemDataBound">
											<ItemTemplate>
												<div visible="<%# string.IsNullOrEmpty(Item.VariationId) == false %>" class="product" style="background-color: white;" runat="server">
													<table>
														<tr>
															<td class="name" style="float: left; width: 535px;">
																<w2c:ProductImage Visible="<%# string.IsNullOrEmpty(Item.VariationId) == false %>" ImageSize="S" IsVariation='<%# Item.ProductId != Item.VariationId %>' ProductMaster="<%# ProductCommon.GetProductVariationInfo(Item.ShopId, Item.ProductId, Item.VariationId, this.MemberRankId) %>" runat="server" />
																<%#: CreateProductName(Item.Name, Item.VariationName1, Item.VariationName2, Item.VariationName3) %>
															</td>
															<td class="price" width="355">
																<% if (this.SubscriptionBox.FixedAmountFlg == Constants.FLG_SUBSCRIPTIONBOX_FIXED_AMOUNT_FALSE) { %>
																<div ID="dSubscriptionBoxPrice" style="text-decoration: line-through" runat="server" Visible="False"><%# CurrencyManager.ToPrice(Item.Price) %></div>
																<div ID="dSubscriptionBoxCampaignPrice" runat="server"><asp:Literal ID="lSubscriptionBoxCampaignPrice" runat="server" /></div>
																<% } %>
															</td>
															<td class="quantity" style="padding-top: 0px;">
																<%#: Item.ItemQuantity %>
															</td>
														</tr>
													</table>
												</div>
											</ItemTemplate>
										</asp:Repeater>

									<div visible="<%# CheckTakeOverProduct(GetItemsModel((string)Container.DataItem)) %>" class="product" style="background-color: white;" runat="server">
										<table>
											<tr>
												<td class="name" width="535">選択された商品を配送します。
												</td>
											</tr>
										</table>
									</div>
								</div>
							</div>
							<!--background-->
						</div>
						<!--productList-->
					</ItemTemplate>
				</asp:Repeater>
			</div>
		<!--toggle-content-->
		</div>
		<!--/.toggle-wrap -->

		<%-- ▽セットプロモーション商品▽ --%>
		<asp:Repeater ID="rCartSetPromotion" DataSource="<%# ((CartObject)((RepeaterItem)Container).DataItem).SetPromotions %>" runat="server">
		<ItemTemplate>
		<asp:HiddenField ID="hfCartSetPromotionNo" runat="server" Value="<%# ((CartSetPromotion)Container.DataItem).CartSetPromotionNo %>" />
		<div class="product">
		<div>
			<asp:Repeater ID="rCartSetPromotionItem" DataSource="<%# ((CartSetPromotion)Container.DataItem).Items %>" OnItemCommand="rCartList_ItemCommand" runat="server">
			<HeaderTemplate>
				<table cellpadding="0" cellspacing="0" width="950" summary="ショッピングカート">
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td class="name">
					<dl>
					<dt><w2c:ProductImage ID="ProductImage3" ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" /></dt>
					<dd><%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %></dd>
					<dd visible='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList.IsSelectedProductOptionValueAll %>' runat="server">
						<asp:Repeater ID="rProductOptionSettings" ItemType="w2.App.Common.Product.ProductOptionSetting" DataSource='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList %>' runat="server">
							<ItemTemplate>
								<%#: Item.GetDisplayProductOptionSettingSelectValue() %>
								<%# (Item.GetDisplayProductOptionSettingSelectValue() != "") ? "<br />" : "" %>
							</ItemTemplate>
						</asp:Repeater>
					</dd>
					<dd Visible="<%# this.SkipOrderConfirm %>" style="font-weight:normal;" runat="server">
						<%# WebSanitizer.HtmlEncode(((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).SetpromotionDispName) %>
						<div Visible="<%# (((CartProduct)Container.DataItem).IsDisplaySell) %>" style="line-height:1.0; font-weight:normal;" runat="server">販売期間：<br/></div>
						<div Visible="<%# (((CartProduct)Container.DataItem).IsDisplaySell) %>" style="line-height:1.0; font-weight:normal; width: 200px;" runat="server">&nbsp;&nbsp;<%#: DateTimeUtility.ToStringFromRegion(((CartProduct)Container.DataItem).SellFrom, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime) %>～<br/>&nbsp;&nbsp;<%#: DateTimeUtility.ToStringFromRegion(((CartProduct)Container.DataItem).SellTo, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime) %></div>
						<div Visible="<%# ((CartProduct)Container.DataItem).IsDispSaleTerm %>" style="line-height:1.0; font-weight:normal;" runat="server">タイムセール期間:<br/></div>
						<div Visible="<%# ((CartProduct)Container.DataItem).IsDispSaleTerm %>" style="line-height:1.0; font-weight:normal; width: 200px;" runat="server">&nbsp;&nbsp;<%#: (ProductCommon.GetProductSaleTermBrLpBeginDate(this.ShopId, ((CartProduct)Container.DataItem).ProductSaleId)) %>～<br/>&nbsp;&nbsp;<%#: (ProductCommon.GetProductSaleTermBrLpEndDate(this.ShopId, ((CartProduct)Container.DataItem).ProductSaleId)) %></div>
						<div style="line-height:1.0; font-weight:normal; width: 200px;">セットプロモーション期間:</div>
						<div style="line-height:1.0; font-weight:normal; width: 200px; padding-left:10px;"><%# WebSanitizer.HtmlEncodeChangeToBr(ProductCommon.GetSetPromotionTermBr(((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).SetPromotionId)) %></div>
					</dd>
					</dl>
					</td>
					<td class="price" style="padding-left: 140px;"><%#: CurrencyManager.ToPrice(((CartProduct)Container.DataItem).Price) %> (<%#: this.ProductPriceTextPrefix %>)</td>
					<td class="quantity" style="padding-top: 0px;">
						<span id="Span2" visible='<%# ((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).IsGift == false %>' runat="server">
						<asp:TextBox ID="tbSetPromotionItemCount" Text='<%# ((CartProduct)Container.DataItem).QuantityAllocatedToSet[((CartSetPromotion)GetParentRepeaterItem(Container, "rCartSetPromotion").DataItem).CartSetPromotionNo] %>' MaxLength="3" CssClass="orderCount" runat="server"></asp:TextBox><br />
						<small id="Small2" class="fred" visible='<%# this.ErrorMessages.HasMessages(GetParentRepeaterItem(Container, "rCartList").ItemIndex, GetParentRepeaterItem(Container, "rCartSetPromotion").ItemIndex, Container.ItemIndex) %>' runat="server">
							<%# WebSanitizer.HtmlEncode(this.ErrorMessages.Get(GetParentRepeaterItem(Container, "rCartList").ItemIndex, GetParentRepeaterItem(Container, "rCartSetPromotion").ItemIndex, Container.ItemIndex)) %>
						</small>
						</span>
						<span id="Span3" visible='<%# ((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).IsGift %>' runat="server">
							<%# StringUtility.ToNumeric(((CartProduct)Container.DataItem).QuantityAllocatedToSet[((CartSetPromotion)GetParentRepeaterItem(Container, "rCartSetPromotion").DataItem).CartSetPromotionNo]) %>
						</span>
					</td>
					<td class="taxRate" style="padding-top: 0px;" runat="server">
						<%#: TaxCalculationUtility.GetTaxRateForDIsplay(((CartProduct)Container.DataItem).TaxRate) %>%</td>
					<td style="float:none; padding-top:0px" id="Td1" visible="<%# (Container.ItemIndex == 0) %>" rowspan='<%# ((CartSetPromotion)GetParentRepeaterItem(Container, "rCartSetPromotion").DataItem).Items.Count %>' class="subtotal" runat="server">
						<span id="Span4" visible='<%# ((CartSetPromotion)GetParentRepeaterItem(Container, "rCartSetPromotion").DataItem).IsDiscountTypeProductDiscount %>' runat="server">
						<strike>&yen;<%# WebSanitizer.HtmlEncode(GetNumeric(((CartSetPromotion)GetParentRepeaterItem(Container, "rCartSetPromotion").DataItem).UndiscountedProductSubtotal))%> (<%#: this.ProductPriceTextPrefix %>)</strike><br />
						</span>
						&yen;<%# WebSanitizer.HtmlEncode(GetNumeric(((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).UndiscountedProductSubtotal - ((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).ProductDiscountAmount))%> (<%#: this.ProductPriceTextPrefix %>)<br />
					</td>
					<td class="delete">
						<asp:LinkButton ID="lbDeleteProduct" visible="<%# CanDeleteSetPromotionProduct((CartProduct)Container.DataItem) %>" CommandName="DeleteProduct" CommandArgument='' Runat="server">削除</asp:LinkButton>
						<%-- 隠し値 --%>
						<asp:HiddenField ID="hfShopId" runat="server" Value="<%# ((CartProduct)Container.DataItem).ShopId %>" />
						<asp:HiddenField ID="hfProductId" runat="server" Value="<%# ((CartProduct)Container.DataItem).ProductId %>" />
						<asp:HiddenField ID="hfVariationId" runat="server" Value="<%# ((CartProduct)Container.DataItem).VariationId %>" />
						<asp:HiddenField ID="hfIsFixedPurchase" runat="server" Value="<%# ((CartProduct)Container.DataItem).IsFixedPurchase %>" />
						<asp:HiddenField ID="hfProductSaleId" runat="server" Value="<%# ((CartProduct)Container.DataItem).ProductSaleId %>" />
						<asp:HiddenField ID="hfProductOptionValue" runat="server" Value='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues() %>' />
						<asp:HiddenField ID="hfAllocatedQuantity" runat="server" Value='<%# ((CartProduct)Container.DataItem).QuantityAllocatedToSet[((CartSetPromotion)GetParentRepeaterItem(Container, "rCartSetPromotion").DataItem).CartSetPromotionNo] %>' />
					</td>
				</tr>
				<tr>
					<td>
					<%-- ▽商品付帯情報▽ --%>
					<div visible="<%# this.IsCartListLp == false %>" runat="server">
						<asp:Repeater ID="rSetPromotionProductOptionValueSettings" DataSource='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList %>' runat="server">
						<ItemTemplate>
							<%# WebSanitizer.HtmlEncode(((ProductOptionSetting)Container.DataItem).ValueName) %>

							<asp:Repeater ID="rSetPromotionCblProductOptionValueSetting" DataSource='<%# ((ProductOptionSetting)Container.DataItem).SettingValuesListItemCollection %>' ItemType="System.Web.UI.WebControls.ListItem" Visible='<%# ((ProductOptionSetting)Container.DataItem).DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_CHECKBOX || ((ProductOptionSetting)Container.DataItem).DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_PRICE_CHECKBOX %>' runat="server" >
							<ItemTemplate>
								<asp:CheckBox AutoPostBack="True" ID="cbSetPromotionProductOptionValueSetting" OnCheckedChanged="cbProductOptionValueSettingListOnCheckedChanged" Text='<%# Item.Text %>' Checked='<%# Item.Selected %>' runat="server" />
							</ItemTemplate>
							</asp:Repeater>

							<asp:DropDownList ID="ddlSetPromotionProductOptionValueSetting"
											OnSelectedIndexChanged="cbProductOptionValueSettingListOnCheckedChanged"
											DataSource='<%# InsertDefaultAtFirstToDdlProductOptionSettingList(((ProductOptionSetting)Container.DataItem).SettingValuesListItemCollection, false) %>'
											visible='<%# ((ProductOptionSetting)Container.DataItem).DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_SELECTMENU 
												|| ((ProductOptionSetting)Container.DataItem).DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_PRICE_DROPDOWNMENU %>'
											SelectedValue='<%# ((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectedValue() %>'
											runat="server" />
							<span id="Span1" class="necessary" runat="server" visible="<%# ((ProductOptionSetting)Container.DataItem).IsNecessary %>">*</span>
							<asp:TextBox ID ="txtSetPromotionProductOptionValueSetting" 
								Text = '<%# (string.IsNullOrEmpty(((ProductOptionSetting)Container.DataItem).SelectedSettingValue) == false)
											? ((ProductOptionSetting)Container.DataItem).SelectedSettingValue : ((ProductOptionSetting)Container.DataItem).DefaultValue %>' visible='<%# ((ProductOptionSetting)Container.DataItem).IsTextBox %>' runat="server" />
							<br />
						</ItemTemplate>
						</asp:Repeater>
					</div>
					<%-- △商品付帯情報△ --%>
					</td>
				</tr>
			</ItemTemplate>
			<FooterTemplate>
				</table>
			</FooterTemplate>
			</asp:Repeater>
		</div>
		</div>
		</ItemTemplate>
		</asp:Repeater>
		<%-- △セットプロモーション商品△ --%>

		<%-- ▽ノベルティ▽ --%>
		<asp:Repeater ID="rNoveltyList" runat="server" ItemType="w2.App.Common.Order.CartNovelty" DataSource="<%# GetCartNovelty(((CartObject)((RepeaterItem)Container).DataItem).CartId)%>" Visible="<%# IsDisplayNovelty(((CartObject)((RepeaterItem)Container).DataItem).CartId) %>">
		<ItemTemplate>
			<div class="novelty clearFix">
				<h4 class="title">
					<%#: Item.NoveltyDispName %>を追加してください。
				</h4>
				<p runat="server" visible="<%# (Item.GrantItemList.Length == 0) %>">
					ただいま付与できるノベルティはございません。
				</p>
				<asp:Repeater ID="rNoveltyItem" runat="server" ItemType="w2.App.Common.Order.CartNoveltyGrantItem" DataSource="<%# Item.GrantItemList %>" OnItemCommand="rCartList_ItemCommand">
				<ItemTemplate>
					<div class="plist">
						<p class="image">
							<w2c:ProductImage ProductMaster="<%# Item.ProductInfo %>" IsVariation="true" ImageSize="M" runat="server" />
						</p>
						<p class="name"><%#: Item.JointName %></p>
						<p class="price"><%#: CurrencyManager.ToPrice(Item.Price) %>(<%#: this.ProductPriceTextPrefix %>)</p>
						<p class="add">
							<asp:LinkButton ID="lbAddNovelty" runat="server" CommandName="AddNovelty" CommandArgument='<%# string.Format("{0},{1}", ((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex) %>' class="btn btn-mini">カートに追加</asp:LinkButton>
						</p>
						<p class="clr"><img src="<%: Constants.PATH_ROOT %>Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1" /></p>
					</div>
				</ItemTemplate>
				</asp:Repeater>
			</div>
		</ItemTemplate>
		</asp:Repeater>
		<%-- △ノベルティ△ --%>
			</div><!--list-->

		<uc:BodyAnnounceFreeShipping ID="BodyAnnounceFreeShipping1" runat="server" TargetCart="<%# ((CartObject)((RepeaterItem)Container).DataItem) %>" />

		<div class="cartOrder">
		<div class="subcartOrder">
		<div class="pointBox" visible="<%# Constants.W2MP_POINT_OPTION_ENABLED && this.IsLoggedIn %>" runat="server">
		<div class="box">
		<p><img src="<%: Constants.PATH_ROOT %>Contents/ImagesPkg/common/ttl_point.gif" alt="ポイントを使う" width="262" height="23" /></p>
		<div class="boxbtm">
		<div>
		<div>
		<dl runat="server" visible="<%# ((CartObject)((RepeaterItem)Container).DataItem).CanUsePointForPurchase %>">
		<dt>今回合計 <%#: GetNumeric(this.LoginUserPointUsable)%> ポイントまでご利用いただけます
		<span>※1<%: Constants.CONST_UNIT_POINT_PT %> = <%: CurrencyManager.ToPrice(1m) %></span>
		</dt>
		<dd><asp:TextBox ID="tbOrderPointUse" Runat="server" Text="<%# GetUsePoint((CartObject)((RepeaterItem)Container).DataItem) %>" MaxLength="6"></asp:TextBox>&nbsp;&nbsp;<%: Constants.CONST_UNIT_POINT_PT %></dd>
		</dl>
		<dl runat="server" visible="<%# (((CartObject)((RepeaterItem)Container).DataItem).CanUsePointForPurchase == false) %>">
			<p>
				あと「<%#: GetPriceCanPurchaseUsePoint(((CartObject)((RepeaterItem)Container).DataItem).PurchasePriceTotal) %>」の購入でポイントをご利用いただけます。
			</p>
			<p runat="server" visible="<%# (this.LoginUserPointUsable > 0) %>">
				※利用可能ポイント「<%#: GetNumeric(this.LoginUserPointUsable) %>pt」
			</p>
		</dl>
		<p class="clr"><img src="<%: Constants.PATH_ROOT %>Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
		</div>
		<% if (Constants.W2MP_POINT_OPTION_ENABLED
			&& this.IsLoggedIn
			&& Constants.FIXEDPURCHASE_OPTION_ENABLED
			&& Constants.FIXEDPURCHASE_NEXT_SHIPPING_USE_POINT_ALL_OPTION_ENABLE) { %>
		<asp:CheckBox Text="定期注文で利用可能なポイント<br>すべてを継続使用する"
			Visible="<%# ((CartObject)((RepeaterItem)Container).DataItem).HasFixedPurchase %>"
			OnCheckedChanged="cbUseAllPointFlg_Changed" OnDataBinding="cbUseAllPointFlg_DataBinding"
			CssClass="cbUseAllPointFlg" Style="margin-left: 1.4em; text-indent: -1.6em;" AutoPostBack="True" runat="server"/>
		<span Visible="<%# ((CartObject)((RepeaterItem)Container).DataItem).HasFixedPurchase %>" runat="server">※注文後はマイページ＞定期購入情報より<br/>変更できます。</span>
		<% } %>
		<p class="clr"><img src="<%: Constants.PATH_ROOT %>Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
		</div>
		<span id="Span5" class="fred" visible="<%# this.ErrorMessages.HasMessages(((RepeaterItem)Container).ItemIndex, OrderPage.CartErrorMessages.ErrorKbn.Point) %>" runat="server">
			<%#: this.ErrorMessages.Get(((RepeaterItem)Container).ItemIndex, OrderPage.CartErrorMessages.ErrorKbn.Point) %>
		</span>
		</div><!--boxbtm-->
		</div><!--box-->
		</div><!--pointBox-->
		<div id="Div4" class="couponBox" visible="<%# Constants.W2MP_COUPON_OPTION_ENABLED %>" runat="server">
		<div class="box">
		<p><img src="<%: Constants.PATH_ROOT %>Contents/ImagesPkg/common/ttl_coupon.gif" alt="クーポンを使う" width="262" height="23" /></p>
		<div id="divCouponInputMethod" runat="server" style="font-size: 10px; padding: 10px 10px 0px 10px; font-family: 'Lucida Grande','メイリオ',Meiryo,'Hiragino Kaku Gothic ProN', sans-serif; color: #333;">
			<asp:RadioButtonList runat="server" AutoPostBack="true" ID="rblCouponInputMethod"
				OnSelectedIndexChanged="rblCouponInputMethod_SelectedIndexChanged" OnDataBinding="rblCouponInputMethod_DataBinding"
				DataSource="<%# GetCouponInputMethod() %>" DataTextField="Text" DataValueField="Value" RepeatColumns="2" RepeatDirection="Horizontal"></asp:RadioButtonList>
		</div>
		<div class="boxbtm">
		<div>
		<div id="hgcCouponSelect" runat="server">
			<asp:DropDownList CssClass="input_border" style="width: 240px" ID="ddlCouponList" runat="server" DataTextField="Text" DataValueField="Value" OnTextChanged="ddlCouponList_TextChanged" AutoPostBack="true"></asp:DropDownList>
		</div>
		<div>
		<dl id="hgcCouponCodeInputArea" runat="server">
		<dt><span>クーポンコード</span></dt>
		<dd><asp:TextBox ID="tbCouponCode" runat="server" Text="<%# GetCouponCode(((CartObject)((RepeaterItem)Container).DataItem).Coupon) %>" MaxLength="30" autocomplete="off"></asp:TextBox></dd>
		</dl>
		<p class="clr"><img src="<%: Constants.PATH_ROOT %>Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
		</div>
		<p class="clr"><img src="<%: Constants.PATH_ROOT %>Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
		</div>
		<span id="Span6" class="fred" visible="<%# this.ErrorMessages.HasMessages(((RepeaterItem)Container).ItemIndex, OrderPage.CartErrorMessages.ErrorKbn.Coupon) %>" runat="server">
			<%# WebSanitizer.HtmlEncode(this.ErrorMessages.Get(((RepeaterItem)Container).ItemIndex, OrderPage.CartErrorMessages.ErrorKbn.Coupon)) %>
		</span>
		<asp:LinkButton runat="server" ID="lbShowCouponBox" Text="クーポンBOX" style="color: #ffffff !important; background-color: #000 !important;
			border-color: rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.25); text-shadow: 0 -1px 0 rgba(0, 0, 0, 0.25); display: inline-block;
			padding: 4px 10px 4px; margin-bottom: 0; font-size: 13px; line-height: 18px; text-align: center; vertical-align: middle; cursor: pointer;
			border: 1px solid #cccccc; border-radius: 4px; box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.2), 0 1px 2px rgba(0, 0, 0, 0.05); white-space: nowrap;"
			OnClick="lbShowCouponBox_Click" ></asp:LinkButton>
		</div><!--boxbtm-->
		</div><!--box-->
		<div runat="server" id="hgcCouponBox" style="z-index: 13; top: 0; left: 0; width: 100%; height: 120%; position: fixed; background-color: rgba(128, 128, 128, 0.75);"
			Visible='<%# ((CartObject)((RepeaterItem)Container).DataItem).CouponBoxVisible %>'>
		<div id="hgcCouponList" style="width: 800px; height: 500px; top: 50%; left: 50%; text-align: center; border: 2px solid #aaa; background: #fff; position: fixed; z-index: 14; margin:-250px 0 0 -400px;">
		<h2 style="height: 20px; color: #fff; background-color: #000; font-size: 16px; padding: 3px 0px; border-bottom: solid 1px #ccc; z-index: 15">クーポンBOX</h2>
		<div style="height: 400px; overflow: auto; z-index: 15">
		<asp:Repeater ID="rCouponList" ItemType="w2.Domain.Coupon.Helper.UserCouponDetailInfo" Runat="server" DataSource="<%# GetUsableCoupons((CartObject)((RepeaterItem)Container).DataItem, SessionManager.ReferralCode) %>">
			<HeaderTemplate>
			<table>
				<tr>
					<th style="border-bottom-style:solid; border-bottom-width:1px; background-color:#ececec; padding:10px; text-align:center;width:150px;">クーポンコード</th>
					<th style="border-bottom-style:solid; border-bottom-width:1px; background-color:#ececec; padding:10px; text-align:center;width:230px;">クーポン名</th>
					<th style="border-bottom-style:solid; border-bottom-width:1px; background-color:#ececec; padding:10px; text-align:center;width:100px;">割引金額<br />/割引率</th>
					<th style="border-bottom-style:solid; border-bottom-width:1px; background-color:#ececec; padding:10px; text-align:center;width:70px;">利用可能回数</th>
					<th style="border-bottom-style:solid; border-bottom-width:1px; background-color:#ececec; padding:10px; text-align:center;width:350px;">有効期限</th>
					<th style="border-bottom-style:solid; border-bottom-width:1px; background-color:#ececec; padding:10px; text-align:center;width:100px;"></th>
				</tr>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td style="border-bottom-style:solid; border-bottom-width:1px; padding:10px 8px; text-align:left; text-align:center;width:150px; background-color: white;">
						<%#: StringUtility.ToEmpty(Item.CouponCode) %><br />
						<asp:HiddenField runat="server" ID="hfCouponBoxCouponCode" Value="<%# Item.CouponCode %>" />
					</td>
					<td style="border-bottom-style:solid; border-bottom-width:1px; padding:10px 8px; text-align:left; text-align:center;width:230px; background-color: white;"
						title="<%#: StringUtility.ToEmpty(Item.CouponDispDiscription) %>">
						<%#: StringUtility.ToEmpty(Item.CouponDispName) %>
					</td>
					<td style="border-bottom-style:solid; border-bottom-width:1px; padding:10px 8px; text-align:left; text-align:center;width:70px; background-color: white;">
						<%#: (StringUtility.ToEmpty(Item.DiscountPrice) != "")
								? CurrencyManager.ToPrice(Item.DiscountPrice)
								: (StringUtility.ToEmpty(Item.DiscountRate) != "")
									? StringUtility.ToEmpty(Item.DiscountRate) + "%"
									: "-" %>
					</td>
					<td style="border-bottom-style:solid; border-bottom-width:1px; padding:10px 8px; text-align:left; text-align:center;width:70px; background-color: white;">
						<%#: GetCouponCount(Item) %>
					</td>
					<td style="border-bottom-style:solid; border-bottom-width:1px; padding:10px 8px; text-align:left; text-align:center;width:350px; background-color: white;">
						<%#: DateTimeUtility.ToStringFromRegion(Item.ExpireEnd, DateTimeUtility.FormatType.LongDateHourMinute1Letter) %>
					</td>
					<td style="border-bottom-style:solid; border-bottom-width:1px; padding:10px 8px; text-align:left; text-align:center;width:100px; background-color: white;">
						<asp:LinkButton runat="server" id="lbCouponSelect" OnClick="lbCouponSelect_Click" style="color: #ffffff !important; background-color: #000 !important;
							border-color: rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.25); text-shadow: 0 -1px 0 rgba(0, 0, 0, 0.25); display: inline-block;
							padding: 4px 10px 4px; margin-bottom: 0; font-size: 13px; line-height: 18px; text-align: center; vertical-align: middle; cursor: pointer;
							border: 1px solid #cccccc; border-radius: 4px; box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.2), 0 1px 2px rgba(0, 0, 0, 0.05); white-space: nowrap;">このクーポンを使う</asp:LinkButton>
					</td>
				</tr>
			</ItemTemplate>
			<FooterTemplate>
				</table>
			</FooterTemplate>
		</asp:Repeater>
		</div>
		<div style="width: 100%; height: 50px; display: block; z-index: 3">
			<asp:LinkButton ID="lbCouponBoxClose" OnClick="lbCouponBoxClose_Click" runat="server"
				style="padding: 8px 12px; font-size: 14px; color: #333; text-decoration: none; border-color: rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.25);
				display: inline-block; line-height: 18px; color: #333333; text-align: center; vertical-align: middle; border-radius: 5px; cursor: pointer; background-color: #f5f5f5;
				border: 1px solid #cccccc; box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.2), 0 1px 2px rgba(0, 0, 0, 0.05); text-decoration: none; background-image: none; margin: 5px auto">クーポンを利用しない</asp:LinkButton>
		</div>
		</div>
		</div>
		</div><!--couponBox-->

		<div class="priceList">
		<div>
		<dl class="bgc">
		<dt>小計(<%#: this.ProductPriceTextPrefix %>)</dt>
		<dd><%#: CurrencyManager.ToPrice(((CartObject)((RepeaterItem)Container).DataItem).PriceSubtotal) %></dd>
		</dl>
		<%-- セットプロモーション割引額(商品割引) --%>
		<asp:Repeater ID="Repeater1" DataSource="<%# ((CartObject)((RepeaterItem)Container).DataItem).SetPromotions %>" runat="server">
		<ItemTemplate>
			<span id="Span7" visible="<%# ((CartSetPromotion)Container.DataItem).IsDiscountTypeProductDiscount %>" runat="server">
			<dl class='<%: (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
			<dt><%# WebSanitizer.HtmlEncode(((CartSetPromotion)Container.DataItem).SetpromotionDispName) %></dt>
			<dd class='<%# (((CartSetPromotion)Container.DataItem).ProductDiscountAmount > 0) ? "minus" : "" %>'><%# (((CartSetPromotion)Container.DataItem).ProductDiscountAmount > 0) ? "-" : ""%><%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).ProductDiscountAmount) %></dd>
			</dl>
			</span>
		</ItemTemplate>
		</asp:Repeater>
		<%if (Constants.MEMBER_RANK_OPTION_ENABLED && this.IsLoggedIn){ %>
		<dl class='<%: (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
		<dt>会員ランク割引額</dt>
		<dd class='<%# (((CartObject)((RepeaterItem)Container).DataItem).MemberRankDiscount > 0) ? "minus" : "" %>'><%# (((CartObject)((RepeaterItem)Container).DataItem).MemberRankDiscount > 0) ? "-" : ""%><%#: CurrencyManager.ToPrice(((CartObject)((RepeaterItem)Container).DataItem).MemberRankDiscount * ((((CartObject)((RepeaterItem)Container).DataItem).MemberRankDiscount < 0) ? -1 : 1)) %></dd>
		</dl>
		<%} %>
		<%if (Constants.MEMBER_RANK_OPTION_ENABLED && Constants.FIXEDPURCHASE_OPTION_ENABLED && this.IsLoggedIn){ %>
		<dl class='<%: (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
		<dt>定期会員割引額</dt>
		<dd class='<%# (((CartObject)((RepeaterItem)Container).DataItem).FixedPurchaseMemberDiscountAmount > 0) ? "minus" : "" %>'><%# (((CartObject)((RepeaterItem)Container).DataItem).FixedPurchaseMemberDiscountAmount > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(((CartObject)((RepeaterItem)Container).DataItem).FixedPurchaseMemberDiscountAmount * ((((CartObject)((RepeaterItem)Container).DataItem).FixedPurchaseMemberDiscountAmount < 0) ? -1 : 1)) %></dd>
		</dl>
		<%} %>
		<%if (Constants.FIXEDPURCHASE_OPTION_ENABLED){ %>
		<span id="Span8" runat="server" visible="<%# (((CartObject)((RepeaterItem)Container).DataItem).HasFixedPurchase) %>">
		<dl class='<%: (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
		<dt>定期購入割引額</dt>
		<dd class='<%# (((CartObject)((RepeaterItem)Container).DataItem).FixedPurchaseDiscount > 0) ? "minus" : "" %>'><%#: (((CartObject)((RepeaterItem)Container).DataItem).FixedPurchaseDiscount > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(((CartObject)((RepeaterItem)Container).DataItem).FixedPurchaseDiscount * ((((CartObject)((RepeaterItem)Container).DataItem).FixedPurchaseDiscount < 0) ? -1 : 1)) %></dd>
		</dl>
		</span>
		<%} %>
		<%if (Constants.W2MP_COUPON_OPTION_ENABLED){ %>
		<dl class='<%: (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
		<dt>クーポン割引額</dt>
		<dd class='<%# (((CartObject)((RepeaterItem)Container).DataItem).UseCouponPrice > 0) ? "minus" : "" %>'>
			<%#: GetCouponName(((CartObject)((RepeaterItem)Container).DataItem)) %>
			<%# (((CartObject)((RepeaterItem)Container).DataItem).UseCouponPrice > 0) ? "-" : "" %>
			<%#: CurrencyManager.ToPrice(((CartObject)((RepeaterItem)Container).DataItem).UseCouponPrice * ((((CartObject)((RepeaterItem)Container).DataItem).UseCouponPrice < 0) ? -1 : 1)) %>
		</dd>
		</dl>
		<%} %>
		<%if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsLoggedIn){ %>
		<dl class='<%: (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
		<dt>ポイント利用額</dt>
		<dd class='<%# (((CartObject)((RepeaterItem)Container).DataItem).UsePointPrice > 0) ? "minus" : "" %>'><%# (((CartObject)((RepeaterItem)Container).DataItem).UsePointPrice > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(((CartObject)((RepeaterItem)Container).DataItem).UsePointPrice * ((((CartObject)((RepeaterItem)Container).DataItem).UsePointPrice < 0) ? -1 : 1)) %></dd>
		</dl>
		<%} %>
		<dl class='<%: (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
		<dt>配送料金</dt>
		<dd id="Dd2" runat="server" style='<%# (((CartObject)((RepeaterItem)Container).DataItem).ShippingPriceSeparateEstimateFlg) || ((((CartObject)((RepeaterItem)Container).DataItem).IsDisplayShippingPriceUnsettled)) ? "display:none;" : ""%>'>
			<asp:Label ID="lbPriceShipping" Text="<%#: CurrencyManager.ToPrice(((CartObject)((RepeaterItem)Container).DataItem).PriceShipping) %>" runat="server"></asp:Label></dd>
		<dd id="Dd3" runat="server" style='<%# (((CartObject)((RepeaterItem)Container).DataItem).ShippingPriceSeparateEstimateFlg == false) ? "display:none;" : ""%>'>
			<%# WebSanitizer.HtmlEncode(((CartObject)((RepeaterItem)Container).DataItem).ShippingPriceSeparateEstimateMessage)%></dd>
			<dd runat="server" style='<%# (((CartObject)((RepeaterItem)Container).DataItem).ShippingPriceSeparateEstimateFlg == false) && (((CartObject)((RepeaterItem)Container).DataItem).IsDisplayShippingPriceUnsettled == false) ? "display:none;" : "color:red"%>'>
				配送先入力後に確定となります。</dd>
		</dl>
		<% if (this.IsShowPaymentFee) { %>
		<dl class='<%: (this.DispNum++ % 2 == 0) ? String.Empty : "bgc" %>'>
			<dt>決済手数料</dt>
			<dd><%#: CurrencyManager.ToPrice(((CartObject)((RepeaterItem)Container).DataItem).Payment.PriceExchange) %></dd>
		</dl>
		<% } %>
		<%-- セットプロモーション割引額(配送料割引) --%>
		<asp:Repeater ID="Repeater2" DataSource="<%# ((CartObject)((RepeaterItem)Container).DataItem).SetPromotions %>" runat="server">
		<ItemTemplate>
			<span id="Span9" visible="<%# ((CartSetPromotion)Container.DataItem).IsDiscountTypeShippingChargeFree %>" runat="server">
			<dl class='<%: (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
			<dt><%# WebSanitizer.HtmlEncode(((CartSetPromotion)Container.DataItem).SetpromotionDispName) %>(送料割引)</dt>
			<dd class='<%# (((CartSetPromotion)Container.DataItem).ShippingChargeDiscountAmount > 0) ? "minus" : "" %>'><%# (((CartSetPromotion)Container.DataItem).ShippingChargeDiscountAmount > 0) ? "-" : ""%><%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).ShippingChargeDiscountAmount) %></dd>
			</dl>
			</span>
		</ItemTemplate>
		</asp:Repeater>
		<%if (this.ProductIncludedTaxFlg == false) { %>
		<dl class='<%: (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
		<dt>消費税額</dt>
		<dd><%# CurrencyManager.ToPrice(((CartObject)((RepeaterItem)Container).DataItem).PriceTax) %></dd>
		</dl>
		<%} %>
		</div>
		<p class="clr"><img src="<%: Constants.PATH_ROOT %>Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
		<div>
		<dl class="result">
		<dt>合計(税込)</dt>
		<% if (this.IsShowPaymentFee) { %>
		<dd><%#: CurrencyManager.ToPrice(((CartObject)((RepeaterItem)Container).DataItem).PriceTotal) %></dd>
		<% } else { %>
		<dd><%#: CurrencyManager.ToPrice(((CartObject)((RepeaterItem)Container).DataItem).PriceCartTotalWithoutPaymentPrice) %></dd>
		<% } %>
		</dl>
		</div>
		</div><!--priceList-->
		<p class="clr"><img src="<%: Constants.PATH_ROOT %>Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
		</div><!--subcartOrder-->
		</div><!--cartOrder-->
		</div><!--background-->
		</div><!--productList-->

		<%-- 隠し更新専用ボタン --%>
		<asp:LinkButton ID="lbPostBack" runat="server" />
		<%-- 隠し値：カートID --%>
		<asp:HiddenField ID="hfCartId" runat="server" Value="<%# this.CartList.Items[((RepeaterItem)Container).ItemIndex].CartId %>" />
		<%-- 隠し再計算ボタン --%>
		<asp:LinkButton id="lbRecalculateCart" runat="server" CommandArgument="<%# ((RepeaterItem)Container).ItemIndex %>" onclick="lbRecalculate_Click"></asp:LinkButton>
		<%-- ▲商品一覧▲ --%>
		</ContentTemplate>
		</asp:UpdatePanel>
		<%-- UPDATE PANELここまで(商品一覧) --%>

		<%-- 各種Js読み込み --%>
		<uc:MailDomains id="MailDomains" runat="server"></uc:MailDomains>
		<!--Login-->
		<% if ((this.IsLoggedIn == false) && (this.IsAmazonLoggedIn == false) && this.IsVisible_UserPassword && this.DisplayLoginForm && (this.IsConnectedLine == false)) { %>
		<div id="Div5" class="clearFix" runat="server" visible ="<%# (((RepeaterItem)Container).ItemIndex == 0) %>">
			<div id="dvUserContents" class="cartstep">
				<h2 class="ttlA">会員の方</h2>
				<div id="dvLogin" class="unit clearFix">
					<div id="">
						<div class="dvLoginLogin">
							<p>ログインIDをお持ちの方は、こちらからログインを行ってください。</p>
							<div style="width:700px; margin:0 auto;">
							<dl style="margin-top:30px;">
							<% if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED) { %>
							<dt style="width:130px; float:left;">メールアドレス</dt>
							<dd style="width:400px; float:left"><asp:TextBox ID="tbLoginIdInMailAddr" runat="server" CssClass="input_widthC input_border mail-domain-suggest" MaxLength="256" Type="email" Width="300px"></asp:TextBox></dd>
							<% } else { %>
							<dt style="width:130px; float:left;">ログインID</dt>
							<dd style="width:400px; float:left"><asp:TextBox ID="tbLoginId" runat="server" CssClass="input_widthC input_border" MaxLength="15" Type="email"></asp:TextBox></dd>
							<% } %>
							</dl>
							<p class="clr" style="height:19px;"><img src="<%: Constants.PATH_ROOT %>Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
							</div>
							<div style="width:700px; margin:0 auto;">
							<dl>
							<dt style="width:130px; float:left;">パスワード</dt>
							<dd style="width:400px; float:left"><asp:TextBox ID="tbPassword" TextMode="Password" autocomplete="off" runat="server" CssClass="input_widthC input_border" MaxLength='<%# GetMaxLength("@@User.password.length_max@@") %>'></asp:TextBox></dd>
							</dl>
							<p class="clr" style="height:19px;"><img src="<%: Constants.PATH_ROOT %>Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
							</div>
							<div style="width:800px; margin:0 auto; ">
								<small id="dLoginErrorMessage" class="fred" runat="server"></small>
								<br />
								<span><asp:CheckBox ID="cbAutoCompleteLoginIdFlg" runat="server" Text="ログインIDを記憶する" /></span>
							</div>
							<div style="width:800px; margin:0 auto; ">
								<div style="width:200px; float:left;">
								<asp:LinkButton ID="lbLogin" runat="server" onclick="lbLogin_Click" OnClientClick="return onClientClickLogin();" class="btn btn-large btn-success">ログイン</asp:LinkButton>
								<br />
								<span><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_PASSWORD_REMINDER_INPUT + "?" + Constants.REQUEST_KEY_NEXT_URL + "=" + this.LandingCartInputAbsolutePath) %>" ><p style="margin:8px 0 4px;text-align: left;">パスワードを忘れた方はこちら</p></a></span>
								</div>
								<div style="width: 600px; float: left; margin-top: 0.5em;">
								<% if (Constants.SOCIAL_LOGIN_ENABLED) { %>
								<%-- Apple --%>
								<% if (DisplaySocialLoginBtnCheck(LandingPageConst.SocialLoginType.Apple)) { %>
									<div style="float:left; margin: 5px;">
										<a class="social-login-lpinputform apple-color"
											href="<%: w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
													w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Apple,
													Constants.PAGE_FRONT_SOCIAL_LOGIN_LANDINGCART_CALLBACK,
													Constants.PAGE_FRONT_SOCIAL_LOGIN_LANDINGCART_CALLBACK,
													true,
													Request.Url.Authority,
													string.Format("{0}#CartList", this.LandingCartInputAbsolutePath)) %>">
											<div class="social-icon-width-landing-apple ">
												<img class="apple-icon-registinput"
													src="<%= Constants.PATH_ROOT %>
													Contents\ImagesPkg\socialLogin\logo_apple.png" />
											</div>
											<div class="social-login-wording-landing">Appleでサインイン</div>
										</a>
									</div>
								<% } %>
								<%-- Facebook --%>
								<% if (DisplaySocialLoginBtnCheck(LandingPageConst.SocialLoginType.FaceBook)) { %>
											<div style="float:left; margin: 5px;">
												<a class="social-login-lpinputform facebook-color"
										href="<%: w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
												w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Facebook,
												Constants.PAGE_FRONT_SOCIAL_LOGIN_LANDINGCART_CALLBACK,
												Constants.PAGE_FRONT_SOCIAL_LOGIN_LANDINGCART_CALLBACK,
												true,
												Request.Url.Authority,
															string.Format("{0}#CartList", this.LandingCartInputAbsolutePath)) %>">
													<div class="social-icon-width">
														<img class="facebook-icon-registinput"
															src="<%= Constants.PATH_ROOT %>
															Contents\ImagesPkg\socialLogin\logo_facebook.png" />
								</div>
													<div class="social-login-wording-landing">Facebookでログイン</div>
												</a>
											</div>
								<% } %>
								<%-- Twitter --%>
								<% if (DisplaySocialLoginBtnCheck(LandingPageConst.SocialLoginType.Twitter)) { %>
											<div style="float:left; margin: 5px;">
												<a class="social-login-lpinputform twitter-color"
										href="<%: w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
											w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Twitter, 
												Constants.PAGE_FRONT_SOCIAL_LOGIN_LANDINGCART_CALLBACK,
												Constants.PAGE_FRONT_SOCIAL_LOGIN_LANDINGCART_CALLBACK,
												true,
												Request.Url.Authority,
															string.Format("{0}#CartList", this.LandingCartInputAbsolutePath)) %>">
													<div class="social-icon-width">
														<img class="twittericon-registinput"
															src="<%= Constants.PATH_ROOT %>
															Contents\ImagesPkg\socialLogin\logo_x.png" />
								</div>
													<div class="twitter-label">Xでログイン</div>
												</a>
											</div>
								<% } %>
								<%-- Yahoo --%>
								<% if (DisplaySocialLoginBtnCheck(LandingPageConst.SocialLoginType.Yahoo)) { %>
											<div style="float:left; margin: 5px;">
												<a class="social-login-lpinputform yahoo-color"
										href="<%: w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
												w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Yahoo,
												Constants.PAGE_FRONT_SOCIAL_LOGIN_LANDINGCART_CALLBACK,
												Constants.PAGE_FRONT_SOCIAL_LOGIN_LANDINGCART_CALLBACK,
												true,
												Request.Url.Authority,
															string.Format("{0}#CartList", this.LandingCartInputAbsolutePath)) %>">
													<div class="social-icon-width">
														<img class="yahoo-icon-registinput"
															src="<%= Constants.PATH_ROOT %>
															Contents\ImagesPkg\socialLogin\logo_yahoo.png" />
								</div>
													<div class="social-login-wording-landing">Yahoo! JAPAN IDでログイン</div>
												</a>
											</div>
								<% } %>
									<%-- Google --%>
								<% if (DisplaySocialLoginBtnCheck(LandingPageConst.SocialLoginType.Gplus)) { %>
										<div style="float:left; margin: 5px;">
											<a class="social-login-lpinputform google-color"
												href="<%=w2.App.Common.User.SocialLogin.Util.SocialLoginUtil.GetAuthenticateUrl(
												w2.App.Common.User.SocialLogin.Helper.SocialLoginApiProviderType.Gplus,
												Constants.PAGE_FRONT_SOCIAL_LOGIN_LANDINGCART_CALLBACK,
												Constants.PAGE_FRONT_SOCIAL_LOGIN_LANDINGCART_CALLBACK,
												true,
												Request.Url.Authority,
														this.LandingCartInputAbsolutePath) %>">
												<div class="social-icon-width">
													<img class="google-icon-registinput"
														src="<%= Constants.PATH_ROOT %>
														Contents\ImagesPkg\socialLogin\logo_google.png" />
								</div>
												<div class="google-label-landing">Sign in with Google</div>
											</a>
										</div>
								<% } %>
								<% } %>
								<%-- LINE --%>
								<% if ((Constants.SOCIAL_LOGIN_ENABLED && DisplaySocialLoginBtnCheck(LandingPageConst.SocialLoginType.Line)) || w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED) { %>
										<div style="float:left; margin: 5px;">
											<div class="social-login-lpinputform line-color">
												<div class="social-login-lpinputform line-hover-color line-active-color">
													<a href="<%: LineConnect(this.LandingCartInputAbsolutePath, 
														Constants.PAGE_FRONT_SOCIAL_LOGIN_LANDINGCART_CALLBACK,
														Constants.PAGE_FRONT_SOCIAL_LOGIN_LANDINGCART_CALLBACK,
														true,
														Request.Url.Authority,
																		string.Format("{0}#CartList", this.LandingCartInputAbsolutePath)) %>">
														<div class="social-icon-width">
															<img class="line-icon-registinput"
																src="<%= Constants.PATH_ROOT %>Contents\ImagesPkg\socialLogin\logo_line.png" />
									</div>
														<div class="social-login-wording-landing">LINEでログイン</div>
													</a>
									</div>
											</div>
											<p style="margin-top: 3px;">※LINE連携時に友だち追加します</p>
										</div>
								<% } %>
								<%-- 楽天ID Connect --%>
								<% if (Constants.RAKUTEN_LOGIN_ENABLED && DisplaySocialLoginBtnCheck(LandingPageConst.SocialLoginType.Rakuten)) { %>
										<div style="float:left; margin: 5px;">
										<asp:LinkButton runat="server" ID="lbRakutenIdConnectRequestAuth" OnClick="lbRakutenIdConnectRequestAuth_Click">
												<img src="https://static.id.rakuten.co.jp/static/btn-japanese-2x/idconnect_01-login_r_280.png" />
										</asp:LinkButton>
											<p style="margin-top: 3px;">
											楽天会員のお客様は、<br/>
											楽天IDに登録している情報を利用して、<br/>
											「新規会員登録/ログイン」が可能です。
										</p>
									</div>
								<% } %>
								</div>
							</div>
						</div><!--dvLoginLogin-->
					</div><!--dvLoginWrap-->
				</div>
			</div>
		</div>
		<% } %>
		<!--Login-->
	<div class="btmbtn above cartstep">
		<h2 id="orderDetailsInput" class="ttlA">ご注文内容入力</h2>
		</div>
		<div class="main">
		<div class="submain clearFix">
		<div class="columnLeft">
		<%-- ▼注文者情報▼ --%>
		<% if ((this.IsAmazonLoggedIn == false) || this.IsLoggedIn || IsTargetToExtendedAmazonAddressManagerOption()) { %>
		<div class="column" visible="<%# (Container.ItemIndex == 0) %>" runat="server">
		<h2><img src="<%: Constants.PATH_ROOT %>Contents/ImagesPkg/order/sttl_user.gif" alt="注文者情報" width="80" height="16" /></h2>
		<p>以下の項目をご入力ください。<br />
		</p>
		<span class="fred">※</span>&nbsp;は必須入力です。<br />
		</div><!--column-->
		<div id="divOwnerColumn" class="column" visible='<%# (Container.ItemIndex == 0) %>' runat="server">
		<div class="userBox">
		<div class="top">
		<div class="bottom">
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
		<dl>
		<%-- 注文者：氏名 --%>
		<dt>
			<%: ReplaceTag("@@User.name.name@@") %>
			&nbsp;<span class="fred">※</span><span id="efo_sign_name"></span>
		</dt>
		<dd>
		姓&nbsp;&nbsp;&nbsp;&nbsp;<asp:TextBox ID="tbOwnerName1" Text="<%# this.CartList.Owner.Name1 %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name1.length_max@@") %>' runat="server"></asp:TextBox>&nbsp;&nbsp;
		名&nbsp;&nbsp;&nbsp;&nbsp;<asp:TextBox ID="tbOwnerName2" Text="<%# this.CartList.Owner.Name2 %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name2.length_max@@") %>' runat="server"></asp:TextBox><br />
		<small>
		<asp:CustomValidator
			ID="cvOwnerName1"
			runat="Server"
			ControlToValidate="tbOwnerName1"
			ValidationGroup="OrderShipping-OrderPayment"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" />
		<asp:CustomValidator
			ID="cvOwnerName2"
			runat="Server"
			ControlToValidate="tbOwnerName2"
			ValidationGroup="OrderShipping-OrderPayment"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" /></small>
		</dd>
		<%-- 注文者：氏名（かな） --%>
		<% if (isOwnerAddrCountryJp) { %>
		<dt>
			<%: ReplaceTag("@@User.name_kana.name@@") %>
			<% if (IsTargetToExtendedAmazonAddressManagerOption() == false) { %>&nbsp;<span class="fred">※</span><span id="efo_sign_kana"></span><% } %>
		</dt>
		<dd>
		姓&nbsp;&nbsp;<asp:TextBox ID="tbOwnerNameKana1" Text="<%# this.CartList.Owner.NameKana1 %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name_kana1.length_max@@") %>' runat="server"></asp:TextBox>&nbsp;&nbsp;
		名&nbsp;&nbsp;<asp:TextBox ID="tbOwnerNameKana2" Text="<%# this.CartList.Owner.NameKana2 %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name_kana2.length_max@@") %>' runat="server"></asp:TextBox><br />
		<small>
		<asp:CustomValidator
			ID="cvOwnerNameKana1"
			runat="Server"
			ControlToValidate="tbOwnerNameKana1"
			ValidationGroup="OrderShipping-OrderPayment"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" />
		<asp:CustomValidator
			ID="cvOwnerNameKana2"
			runat="Server"
			ControlToValidate="tbOwnerNameKana2"
			ValidationGroup="OrderShipping-OrderPayment"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" /></small>
		</dd>
		<% } %>
		<%-- 注文者：生年月日 --%>
		<dt>
			<%: ReplaceTag("@@User.birth.name@@") %>
			<% if (IsTargetToExtendedAmazonAddressManagerOption() == false) { %>&nbsp;<span class="fred">※</span><span id="efo_sign_birth"></span><% } %>
		</dt>
		<dd>
		<asp:DropDownList ID="ddlOwnerBirthYear" DataSource='<%# this.OrderOwnerBirthYear %>' SelectedValue="<%# this.CartList.Owner.Birth.HasValue ? this.CartList.Owner.Birth.Value.Year.ToString() : string.Empty %>" CssClass="input_border" runat="server"></asp:DropDownList>&nbsp;&nbsp;年&nbsp;&nbsp;
		<asp:DropDownList ID="ddlOwnerBirthMonth" DataSource='<%# this.OrderOwnerBirthMonth %>' SelectedValue="<%# this.CartList.Owner.Birth.HasValue ? this.CartList.Owner.Birth.Value.Month.ToString() : string.Empty %>" CssClass="input_widthA input_border" runat="server"></asp:DropDownList>&nbsp;&nbsp;月&nbsp;&nbsp;
		<asp:DropDownList ID="ddlOwnerBirthDay" DataSource='<%# this.OrderOwnerBirthDay %>' SelectedValue="<%# this.CartList.Owner.Birth.HasValue ? this.CartList.Owner.Birth.Value.Day.ToString() : string.Empty %>" CssClass="input_widthA input_border" runat="server"></asp:DropDownList>&nbsp;&nbsp;日
		<small>
		<asp:CustomValidator
			ID="cvOwnerBirth"
			runat="Server"
			ControlToValidate="ddlOwnerBirthDay"
			ValidationGroup="OrderShipping-OrderPayment"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			EnableClientScript="false"
			CssClass="error_inline" /></small>
		</dd>
		<%-- 注文者：性別 --%>
		<dt>
			<%: ReplaceTag("@@User.sex.name@@") %>
			&nbsp;<span class="fred">※</span><span id="efo_sign_sex"></span>
		</dt>
		<dd class="input_align">
		<asp:RadioButtonList ID="rblOwnerSex" DataSource='<%# this.OrderOwnerSex %>' SelectedValue='<%# GetCorrectSexForDataBindDefault() %>' DataTextField="Text" DataValueField="Value" RepeatDirection="Horizontal" CellSpacing="5" RepeatLayout="Flow" CssClass="input_radio" runat="server" />
		<small>
		<asp:CustomValidator
			ID="cvOwnerSex"
			runat="Server"
			ControlToValidate="rblOwnerSex"
			ValidationGroup="OrderShipping-OrderPayment"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			EnableClientScript="false"
			CssClass="error_inline" /></small>
		</dd>
		<%-- 注文者：PCメールアドレス --%>
		<dt>
			<%: ReplaceTag("@@User.mail_addr.name@@") %>
			&nbsp;<span class="fred">※</span><span id="efo_sign_mail_addr"></span>
		</dt>
		<dd><asp:TextBox ID="tbOwnerMailAddr" Text="<%# this.CartList.Owner.MailAddr %>" CssClass="input_widthE input_border mail-domain-suggest" MaxLength="256" runat="server" Type="email"></asp:TextBox><br />
		<small>
		<asp:CustomValidator runat="Server"
			ID="cvOwnerMailAddr"
			ControlToValidate="tbOwnerMailAddr"
			ValidationGroup="OrderShipping-OrderPayment"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" /></small>
		</dd>
		<%-- 注文者：PCメールアドレス（確認用） --%>
		<% if (this.DisplayMailAddressConfirmForm){ %>
		<dt>
			<%: ReplaceTag("@@User.mail_addr.name@@") %>（確認用）
			&nbsp;<span class="fred">※</span><span id="efo_sign_mail_addr_conf"></span>
		</dt>
		<dd><asp:TextBox ID="tbOwnerMailAddrConf" Text="<%# this.CartList.Owner.MailAddr %>" CssClass="input_widthE input_border mail-domain-suggest" MaxLength="256" runat="server" Type="email"></asp:TextBox><br />
		<small>
		<asp:CustomValidator runat="Server"
			ID="cvOwnerMailAddrConf"
			ControlToValidate="tbOwnerMailAddrConf"
			ValidationGroup="OrderShipping-OrderPayment"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" /></small>
		</dd>
		<% } %>
		<%-- 注文者：国 --%>
		<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
		<dt>
			<%: ReplaceTag("@@User.country.name@@", ownerAddrCountryIsoCode) %>
			&nbsp;<span class="fred">※</span>
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
			&nbsp;<span class="fred">※</span><span id="efo_sign_zip"></span>
		</dt>
		<dd>
		<p class="pdg_topC">
		<asp:TextBox ID="tbOwnerZip" Text="<%# this.CartList.Owner.Zip %>" OnTextChanged="lbSearchOwnergAddr_Click" CssClass="input_widthC input_border" Type="tel" MaxLength="8" runat="server" /></p>
		<span class="btn_add_sea"><asp:LinkButton ID="lbSearchOwnergAddr" runat="server" onclick="lbSearchOwnergAddr_Click" class="btn btn-mini" OnClientClick="return false;">住所検索</asp:LinkButton></span>
		<%--検索結果レイヤー--%>
		<uc:Layer ID="ucLayer" runat="server" />
		<p class="clr">
		<small class="fred">
		<asp:CustomValidator
			ID="cvOwnerZip1"
			runat="Server"
			ControlToValidate="tbOwnerZip"
			ValidationGroup="OrderShipping-OrderPayment"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline cvOwnerZipShortInput" /></small>
		<small id="sOwnerZipError" runat="server" class="fred sOwnerZipError"></small>
		</p></dd>
		<%-- 注文者：都道府県 --%>
		<dt>
			<%: ReplaceTag("@@User.addr1.name@@") %>
			&nbsp;<span class="fred">※</span><span id="efo_sign_addr1"></span>
		</dt>
		<dd><asp:DropDownList ID="ddlOwnerAddr1" DataSource="<%# this.Addr1List %>" DataTextField="Text" DataValueField="Value" SelectedValue="<%# this.CartList.Owner.Addr1 %>" runat="server"></asp:DropDownList>
		<small>
		<asp:CustomValidator
			ID="cvOwnerAddr1"
			runat="Server"
			ControlToValidate="ddlOwnerAddr1"
			ValidationGroup="OrderShipping-OrderPayment"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" /></small>
		</dd>
		<% } %>
		<%-- 注文者：市区町村 --%>
		<dt>
			<%: ReplaceTag("@@User.addr2.name@@", ownerAddrCountryIsoCode) %>
			&nbsp;<span class="fred">※</span><% if (isOwnerAddrCountryJp) { %><span id="efo_sign_addr2"></span><% } %>
		</dt>
		<dd>
			<% if (isOwnerAddrCountryTw) { %>
				<asp:DropDownList runat="server" ID="ddlOwnerAddr2" DataSource="<%# this.UserTwCityList %>" AutoPostBack="true" DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlOwnerAddr2_SelectedIndexChanged"></asp:DropDownList>
			<% } else { %>
				<asp:TextBox ID="tbOwnerAddr2" Text="<%# this.CartList.Owner.Addr2 %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.addr2.length_max@@") %>' runat="server"></asp:TextBox><br />
				<small>
				<asp:CustomValidator
					ID="cvOwnerAddr2"
					runat="Server"
					ControlToValidate="tbOwnerAddr2"
					ValidationGroup="OrderShipping-OrderPayment"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					CssClass="error_inline" /></small>
			<% } %>
		</dd>
		<%-- 注文者：番地 --%>
		<dt>
			<%: ReplaceTag("@@User.addr3.name@@", ownerAddrCountryIsoCode) %>
			<% if (isOwnerAddrCountryJp) { %>&nbsp;<span class="fred">※</span><span id="efo_sign_addr3"></span><% } %>
		</dt>
		<dd>
			<% if (isOwnerAddrCountryTw) { %>
				<asp:DropDownList runat="server" ID="ddlOwnerAddr3" AutoPostBack="true" DataTextField = "Key" DataValueField = "Value" Width="95"><asp:ListItem value="">區域</asp:ListItem></asp:DropDownList>
			<% } else { %>
				<asp:TextBox ID="tbOwnerAddr3" Text="<%# this.CartList.Owner.Addr3 %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.addr3.length_max@@") %>' runat="server"></asp:TextBox><br />
				<small>
				<asp:CustomValidator
					ID="cvOwnerAddr3"
					runat="Server"
					ControlToValidate="tbOwnerAddr3"
					ValidationGroup="OrderShipping-OrderPayment"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					CssClass="error_inline" /></small>
			<% } %>
		</dd>
		<%-- 注文者：ビル・マンション名 --%>
		<dt>

			<%: ReplaceTag("@@User.addr4.name@@", ownerAddrCountryIsoCode) %>
			<% if (isOwnerAddrCountryJp == false) { %>&nbsp;<span class="fred">※</span><% } %>
		</dt>
		<dd><asp:TextBox ID="tbOwnerAddr4" Text="<%# this.CartList.Owner.Addr4 %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.addr4.length_max@@") %>' runat="server"></asp:TextBox><br />
		<small>
		<asp:CustomValidator ID="CustomValidator1" runat="Server"
			ControlToValidate="tbOwnerAddr4"
			ValidationGroup="OrderShipping-OrderPayment"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" /></small>
		</dd>
		<% if (isOwnerAddrCountryJp == false) { %>
		<%-- 注文者：州 --%>
		<dt>
			<%: ReplaceTag("@@User.addr5.name@@", ownerAddrCountryIsoCode) %>
			<% if (isOwnerAddrCountryUs) { %>&nbsp;<span class="fred">※</span><% } %>
		</dt>
		<dd>
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
					CssClass="error_inline" />
			<% } else { %>
			<asp:TextBox runat="server" ID="tbOwnerAddr5" Text="<%# this.CartList.Owner.Addr5 %>"></asp:TextBox>
			<small>
			<asp:CustomValidator
				ID="cvOwnerAddr5"
				runat="Server"
				ControlToValidate="tbOwnerAddr5"
				ValidationGroup="OrderShippingGlobal-OrderPayment"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				CssClass="error_inline" /></small>
			<% } %>
		</dd>
		<%-- 注文者：郵便番号（海外向け） --%>
		<dt>
			<%: ReplaceTag("@@User.zip.name@@", ownerAddrCountryIsoCode) %>
			<% if (isOwnerAddrZipNecessary) { %>&nbsp;<span class="fred">※</span><% } %>
		</dt>
		<dd>
			<asp:TextBox runat="server" id="tbOwnerZipGlobal" MaxLength="20" Text="<%# this.CartList.Owner.Zip %>"></asp:TextBox>
			<small>
			<asp:CustomValidator
				id="cvOwnerZipGlobal"
				runat="Server"
				ControlToValidate="tbOwnerZipGlobal"
				ValidationGroup="OrderShippingGlobal-OrderPayment"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				CssClass="error_inline" /></small>
			<asp:LinkButton
				ID="lbSearchAddrOwnerFromZipGlobal"
				OnClick="lbSearchAddrOwnerFromZipGlobal_Click"
				Style="display:none;"
				runat="server" />
		</dd>
		<% } %>
		<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
		<%-- 注文者：企業名 --%>
		<dt>
			<%: ReplaceTag("@@User.company_name.name@@") %>
			&nbsp;<span class="fred"></span>
		</dt>
		<dd><asp:TextBox ID="tbOwnerCompanyName" Text="<%# this.CartList.Owner.CompanyName %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.company_name.length_max@@") %>' runat="server"></asp:TextBox><br />
		<small>
		<asp:CustomValidator
			ID="cvOwnerCompanyName"
			runat="Server"
			ControlToValidate="tbOwnerCompanyName"
			ValidationGroup="OrderShipping-OrderPayment"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" /></small>
		</dd>
		<%-- 注文者：部署名 --%>
		<dt>
			<%: ReplaceTag("@@User.company_post_name.name@@") %>
			&nbsp;<span class="fred"></span>
		</dt>
		<dd><asp:TextBox ID="tbOwnerCompanyPostName" Text="<%# this.CartList.Owner.CompanyPostName %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.company_post_name.name@@") %>' runat="server"></asp:TextBox><br />
		<small>
		<asp:CustomValidator
			ID="cvOwnerCompanyPostName"
			runat="Server"
			ControlToValidate="tbOwnerCompanyPostName"
			ValidationGroup="OrderShipping-OrderPayment"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" /></small>
		</dd>
		<%} %>
		<%-- 注文者：電話番号1 --%>
		<% if (isOwnerAddrCountryJp) { %>
		<dt>
			<%: ReplaceTag("@@User.tel1.name@@") %>
			&nbsp;<span class="fred">※</span><span id="efo_sign_tel1"></span>
		</dt>
		<dd>
			<asp:TextBox ID="tbOwnerTel1" Text="<%#: this.CartList.Owner.Tel1 %>" CssClass="input_widthD input_border shortTel" MaxLength="13" Type="tel" runat="server" onchange="resetAuthenticationCodeInput('cvOwnerTel1_1')" />
			<% if (this.AuthenticationUsable) { %>
			<span class="btn_add_sea">
				<asp:LinkButton
					ID="lbGetAuthenticationCode"
					class="btn btn-mini"
					runat="server"
					Text="認証コードの取得"
					OnClick="lbGetAuthenticationCode_Click" />
			</span>
			<br />
			<asp:Label ID="lbAuthenticationStatus" runat="server" />
			<% } %>
			<small>
				<asp:CustomValidator
					ID="cvOwnerTel1_1"
					ControlToValidate="tbOwnerTel1"
					ValidationGroup="OrderShipping-OrderPayment"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					CssClass="error_inline"
					runat="server" />
			</small>
		</dd>
		<% if (this.AuthenticationUsable) { %>
		<dt>
			<%: ReplaceTag("@@User.authentication_code.name@@") %>
			<span class="fred">※</span>
		</dt>
		<dd>
			<asp:TextBox
				ID="tbAuthenticationCode"
				CssClass="input_widthA input_border"
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
		<dt>
			<%: ReplaceTag("@@User.tel2.name@@", ownerAddrCountryIsoCode) %>
		</dt>
		<dd>
		<asp:TextBox ID="tbOwnerTel2" Text="<%#: this.CartList.Owner.Tel2 %>" CssClass="input_widthD input_border shortTel" MaxLength="13" runat="server" /><br />
		<small>
		<asp:CustomValidator
			ID="cvOwnerTel2_1"
			runat="Server"
			ControlToValidate="tbOwnerTel2"
			ValidationGroup="OrderShipping-OrderPayment"
			ValidateEmptyText="false"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" /></small>
		</dd>
		<% } else { %>
		<%-- 注文者：電話番号1（海外向け） --%>
		<dt>
			<%: ReplaceTag("@@User.tel1.name@@", ownerAddrCountryIsoCode) %>
			&nbsp;<span class="fred">※</span>
		</dt>
		<dd>
			<asp:TextBox runat="server" ID="tbOwnerTel1Global" MaxLength="30" Text="<%# this.CartList.Owner.Tel1 %>" onchange="resetAuthenticationCodeInput('cvOwnerTel1Global')" />
			<% if (this.AuthenticationUsable) { %>
			<span class="btn_add_sea">
				<asp:LinkButton
					ID="lbGetAuthenticationCodeGlobal"
					class="btn btn-mini"
					runat="server"
					Text="認証コードの取得"
					OnClick="lbGetAuthenticationCode_Click" />
				<br />
				<asp:Label ID="lbAuthenticationStatusGlobal" runat="server" />
			</span>
			<% } %>
			<small>
			<asp:CustomValidator
				ID="cvOwnerTel1Global"
				runat="Server"
				ControlToValidate="tbOwnerTel1Global"
				ValidationGroup="OrderShippingGlobal-OrderPayment"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				CssClass="error_inline" /></small>
		</dd>
		<% if (this.AuthenticationUsable) { %>
		<dt>
			<%: ReplaceTag("@@User.authentication_code.name@@") %>
			<span class="fred">※</span>
		</dt>
		<dd>
			<asp:TextBox
				ID="tbAuthenticationCodeGlobal"
				CssClass="input_widthA input_border shortTel"
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
		</dt>
		<dd>
			<asp:TextBox runat="server" ID="tbOwnerTel2Global" MaxLength="30" Text="<%# this.CartList.Owner.Tel2 %>"></asp:TextBox>
			<small>
			<asp:CustomValidator
				ID="cvOwnerTel2Global"
				runat="Server"
				ControlToValidate="tbOwnerTel2Global"
				ValidationGroup="OrderShippingGlobal-OrderPayment"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				CssClass="error_inline" /></small>
		</dd>
		<% } %>
		<dt>
			<%: ReplaceTag("@@User.mail_flg.name@@") %>
		</dt>
		<dd><asp:CheckBox ID="cbOwnerMailFlg" Checked="<%# this.CartList.Owner.MailFlg %>" Text=" 配信する" CssClass="checkBox" runat="server" /></dd>
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
				Text='<%#: IsTargetToExtendedAmazonAddressManagerOption() ? " 注文者情報で会員登録をする " : " 会員登録する " %>'
				OnCheckedChanged="cbUserRegister_OnCheckedChanged"
				CssClass="checkBox"
				runat="server"
				AutoPostBack="true" />
		</dd>
		<% } %>
		<div id="dvUserPassword" runat="server" visible="<%# this.IsLoggedIn == false %>">
		<%-- ▼ログイン情報入力▼ --%>
		<% if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED == false) { %>
			<dt>
				<%-- ログインID --%>
				<%: ReplaceTag("@@User.login_id.name@@") %><span class="necessary">&nbsp;<span class="fred">※</span></span>
			</dt>
			<dd>
				<asp:TextBox id="tbUserLoginId" Width="120" Runat="server"></asp:TextBox><span class="notes" MaxLength='<%# GetMaxLength("@@User.login_id.length_max@@") %>'>※6～15桁</span>
				<asp:CustomValidator ID="cvUserLoginId" runat="Server"
					ControlToValidate="tbUserLoginId"
					ValidationGroup="OrderShipping-OrderPayment"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					CssClass="error_inline" />
			</dd>
		<% } %>
		<% if (this.IsVisible_UserPassword && (IsTargetToExtendedAmazonAddressManagerOption() == false)) { %>
			<asp:UpdatePanel runat="server" ID="upPasswordUpdatePanel" UpdateMode="Conditional">
			<ContentTemplate>
			<dt>
				<%: ReplaceTag("@@User.password.name@@") %>
				<span class="fred">※</span>

			</dt>
			<dd>
				<asp:Literal ID="lUserPassword" runat="server"></asp:Literal>
				<asp:TextBox id="tbUserPassword" TextMode="Password" autocomplete="off" CssClass="input_border" Runat="server" MaxLength='<%# GetMaxLength("@@User.password.length_max@@") %>'></asp:TextBox>
				<p class="notes">※半角英数字混合7～15文字</p>
				<asp:Button ID="btnUserPasswordReType" runat="server" Visible = false Text="再入力" OnClick="btnUserPasswordReType_Click"></asp:Button>
				<asp:CustomValidator ID="cvUserPassword" runat="Server"
					ControlToValidate="tbUserPassword"
					ValidationGroup="OrderShipping-OrderPayment"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					CssClass="error_inline" />
			</dd>
			<dt>
				<%: ReplaceTag("@@User.password.name@@") %>(確認用)
				<span class="fred">※</span>

			</dt>
			<dd>
				<asp:Literal ID="lUserPasswordConf" runat="server"></asp:Literal>
				<asp:TextBox id="tbUserPasswordConf" TextMode="Password" autocomplete="off" CssClass="input_border" Runat="server" MaxLength='<%# GetMaxLength("@@User.password.length_max@@") %>'></asp:TextBox>
				<p class="notes">※半角英数字混合7～15文字</p>
				<asp:CustomValidator ID="cvUserPasswordConf" runat="Server"
					ControlToValidate="tbUserPasswordConf"
					ValidationGroup="OrderShipping-OrderPayment"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					CssClass="error_inline" />
			</dd>
			</ContentTemplate>
			</asp:UpdatePanel>
		<% } %>
		<%-- ▲ログイン情報入力▲ --%>
			<%-- ユーザー拡張項目　HasInput:true(入力画面)/false(確認画面)　HasRegist:true(新規登録)/false(登録編集) --%>
			<uc:BodyUserExtendLandingPageRegist ID="ucBodyUserExtendLandingPageRegist" Visible="<%# IsTargetToExtendedAmazonAddressManagerOption() == false %>" runat="server" HasInput="true" HasRegist="true" IsLandingPage="true"/>
		<div id="dvUserBox">
			<div id="dvUserRegistRegulation" class="unit">
				<%-- メッセージ --%>
				<h3>会員規約について</h3>
				<div class="dvContentsInfo">
					<p>「<%: ShopMessage.GetMessage("ShopName") %>」入会お申込の前に、以下の会員規約・利用規約を必ずお読み下さい。
					</p>
				</div>

				<div class="dvRegulation">
					<uc:UserRegistRegulationMessage ID="UserRegistRegulationMessage1" runat="server" />
				</div>
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
		</div><!--bottom-->
		</div><!--top-->
		</div><!--userBox-->
		</div><!--column-->
		<% } else { %>
		<div class="column">
			<h2><img src="<%: Constants.PATH_ROOT %>Contents/ImagesPkg/order/sttl_user.gif" alt="注文者情報" width="80" height="16" /></h2>
			
			<%--▼▼ Amazon Pay(CV2)注文者情報 ▼▼--%>
			<% if ((this.AmazonPaySessionOwnerAddress != null) && Constants.AMAZON_PAYMENT_CV2_ENABLED && (IsTargetToExtendedAmazonAddressManagerOption() == false)) { %>

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
					<asp:CheckBox ID="cbGuestOwnerMailFlg2" Checked="<%# this.CartList.Owner.MailFlg %>" Text="配信する" CssClass="checkBox" runat="server" />
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
				<div id="ownerAddressBookWidgetDiv" style="width:340px;height:300px;"></div>
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
						<div class="userBox">
							<div class="checkBox">
								<asp:CheckBox ID="cbUserRegisterForExternalSettlement" Checked="true" Text=" Amazonお届け先住所で会員登録する " OnCheckedChanged="cbUserRegisterForExternalSettlement_OnCheckedChanged" CssClass="checkBox" runat="server" AutoPostBack="true" />
							</div>
							<div id="dvUserBoxVisible" runat="server">
								<div id="dvUserBox">
									<div id="dvUserRegistRegulation" class="unit">
										<%-- メッセージ --%>
										<h3>会員規約について</h3>
										<div class="dvContentsInfo">
											<p>「<%: ShopMessage.GetMessage("ShopName") %>」入会お申込の前に、以下の会員規約・利用規約を必ずお読み下さい。
											</p>
										</div>
										<div class="dvRegulation">
											<uc:UserRegistRegulationMessage ID="UserRegistRegulationMessage2" runat="server" />
										</div>
									</div>
								</div>
							</div>
						</div>
					</ContentTemplate>
				</asp:UpdatePanel>
			<% } %>
			<%-- ▲Amazon Pay会員登録▲ --%>

		</div>
		<% } %>
		<%-- ▲注文者情報▲ --%>
		</div>

		<div class="columnRight">
		<%-- ▼配送先情報▼ --%>
        <h2><img src="<%: Constants.PATH_ROOT %>Contents/ImagesPkg/order/sttl_esd.gif" alt="配送先情報" width="80" height="16" /></h2>
		<% if (this.IsAmazonLoggedIn == false) { %>
		<%-- UPDATE PANEL開始(配送先情報) --%>
		<asp:UpdatePanel ID="upShippingUpdatePanel" runat="server" Visible="<%#(this.IsAmazonLoggedIn == false) %>">
        <ContentTemplate>
		<p>「注文者情報」で入力した住所宛にお届けする場合は、以下の「注文者情報の住所へ配送する」を選択してください。<br /><span class="fred">※</span>&nbsp;は必須入力です。</p>
		<div class="orderBox">
			<h3><div class="cartNo">カート番号<%# ((RepeaterItem)Container).ItemIndex + 1 %></div>　</h3>
			<asp:HiddenField id="hcShowShippingInputForm" value="<%# CanInputShippingTo(((RepeaterItem)Container).ItemIndex) %>" runat="server" />
			<div id="divShipToCart1Address" class="userList" Visible="<%# CanInputShippingTo(((RepeaterItem)Container).ItemIndex) && (((RepeaterItem)Container).ItemIndex != 0) %>" runat="server">
			<asp:CheckBox id="cbShipToCart1Address" Text="カート１の配送先へ配送する" OnCheckedChanged="cbShipToCart1Address_OnCheckedChanged" AutoPostBack="true" Checked="<%# ((CartObject)((RepeaterItem)Container).DataItem).Shippings[0].IsSameShippingAsCart1 %>" CssClass="checkBox" runat="server" />
			<span style="color:red;display:block;"><asp:Literal ID="lShipToCart1AddressInvalidMessage" runat="server" /></span>
		</div>

		<div id="divShippingInputForm" class="userList" runat="server">
		配送先を選択して下さい。<br /><br />
		<asp:DropDownList ID="ddlShippingKbnList" DataSource="<%# GetShippingKbnList(((RepeaterItem)Container).ItemIndex) %>" DataTextField="text" DataValueField="value" SelectedValue="<%# ((CartObject)((RepeaterItem)Container).DataItem).Shippings[0].ShippingAddrKbn %>" OnSelectedIndexChanged="ddlShippingKbnList_OnSelectedIndexChanged" AutoPostBack="true" runat="server"></asp:DropDownList><br />
		<span style="color:red;display:block;"><asp:Literal ID="lStorePickUpInvalidMessage" runat="server" /></span><br />
		<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
			<dd runat="server" id="ddShippingReceivingStoreType">
				<br />
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
		<span style="color:red;display:block;"><asp:Literal ID="lShippingCountryErrorMessage" runat="server"></asp:Literal></span>
		<span id='<%# "spErrorConvenienceStore" + ((RepeaterItem)Container).ItemIndex.ToString() %>' style="color:red;display:block;"></span>
		<%-- ▽コンビニ受取り▽ --%>
		<div id="divShippingInputFormConvenience" class="<%# ((RepeaterItem)Container).ItemIndex %>" runat="server">
			<dl>
				<br />
				<span style="color:red;display:block;"><asp:Literal ID="lConvenienceStoreErrorMessage" runat="server"></asp:Literal></span><br />
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
				<dt id="ddCvsShopId">
					<%: ReplaceTag("@@DispText.shipping_convenience_store.shopId@@") %><br />
					<span style="font-weight: normal;">
						<asp:Literal ID="lCvsShopId" runat="server" Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID) %>"></asp:Literal>
					</span>
					<asp:HiddenField ID="hfCvsShopId" runat="server" Value="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID) %>" />
				</dt>
				<dt id="ddCvsShopName">
					<%: ReplaceTag("@@DispText.shipping_convenience_store.shopName@@") %><br />
					<span style="font-weight: normal;">
						<asp:Literal ID="lCvsShopName" runat="server" Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1) %>"></asp:Literal>
					</span>
					<asp:HiddenField ID="hfCvsShopName" runat="server" Value="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1) %>" />
				</dt>
				<dt id="ddCvsShopAddress">
					<%: ReplaceTag("@@DispText.shipping_convenience_store.shopAddress@@") %><br />
					<span style="font-weight: normal;">
						<asp:Literal ID="lCvsShopAddress" runat="server" Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4) %>"></asp:Literal>
					</span>
					<asp:HiddenField ID="hfCvsShopAddress" runat="server" Value="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4) %>" />
				</dt>
				<dt id="ddCvsShopTel">
					<%: ReplaceTag("@@DispText.shipping_convenience_store.shopTel@@") %><br />
					<span style="font-weight: normal;">
						<asp:Literal ID="lCvsShopTel" runat="server" Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %>"></asp:Literal>
					</span>
					<asp:HiddenField ID="hfCvsShopTel" runat="server" Value="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %>" />
				</dt>
				<dt>＜コンビニ受取の際の注意事項＞</br>
					注文者情報は必ず正しい「<%: ReplaceTag("@@DispText.shipping_convenience_store.Name@@") %>」と「<%: ReplaceTag("@@DispText.shipping_convenience_store.Tel@@") %>」を入力してください。（ショートメールが受け取れる電話番号を入力する必要があります）
					コンビニで商品を受け取る際、店舗ではお客様の「<%: ReplaceTag("@@DispText.shipping_convenience_store.Name@@") %>と「<%: ReplaceTag("@@DispText.shipping_convenience_store.Tel@@") %>」下3桁を確認します。
				</dt></br>
				<asp:HiddenField ID="hfCvsShopFlg" runat="server" Value="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG) %>" />
			</dl>
		</div>
		<%-- △コンビニ受取り△ --%>

		<%-- ▽配送先表示▽ --%>
			<div id="divShippingDisp" visible="<%# GetShipToOwner(((CartObject)((RepeaterItem)Container).DataItem).Shippings[0]) %>" runat="server">
				<% var isShippingAddrCountryJp = IsCountryJp(this.CountryIsoCode); %>
		<dl>
		<dt><%: ReplaceTag("@@User.name.name@@") %></dt>
		<dd>
			<asp:Literal ID="lShippingName1" runat="server"></asp:Literal><asp:Literal ID="lShippingName2" runat="server"></asp:Literal>&nbsp;様　
			<% if (isShippingAddrCountryJp){ %>（<asp:Literal ID="lShippingNameKana1" runat="server"></asp:Literal><asp:Literal ID="lShippingNameKana2" runat="server"></asp:Literal>&nbsp;さま）<% } %><br />
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
			<asp:Literal ID="lShippingCountryName" runat="server"></asp:Literal>
		</dd>
		<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
		<dt><%: ReplaceTag("@@User.company_name.name@@")%>・
			<%: ReplaceTag("@@User.company_post_name.name@@")%></dt>
		<dd>
		<asp:Literal ID="lShippingCompanyName" runat="server"></asp:Literal>&nbsp<asp:Literal ID="lShippingCompanyPostName" runat="server"></asp:Literal>
		</dd>
		<%} %>
		<dt><%: ReplaceTag("@@User.tel1.name@@") %></dt>
		<dd>
		<asp:Literal ID="lShippingTel1" runat="server"></asp:Literal>
		</dd>
		</dl>
		</div>
		<%-- △配送先表示△ --%>
		
		<% this.CartItemIndexTmp++; %>

		<%-- ▽配送先入力フォーム▽ --%>
			<div id="divShippingInputFormInner" visible="<%# GetShipToOwner(((CartObject)((RepeaterItem)Container).DataItem).Shippings[0]) == false %>" runat="server">
			<div id="divShippingVisibleConvenienceStore" class="<%# ((RepeaterItem)Container).ItemIndex %>" runat="server">
		<%
            var shippingAddrCountryIsoCode = GetShippingAddrCountryIsoCode(this.CartItemIndexTmp);
            var isShippingAddrCountryJp = IsCountryJp(shippingAddrCountryIsoCode);
            var isShippingAddrCountryUs = IsCountryUs(shippingAddrCountryIsoCode);
            var isShippingAddrCountryTw = IsCountryTw(shippingAddrCountryIsoCode);
            var isShippingAddrZipNecessary = IsAddrZipcodeNecessary(shippingAddrCountryIsoCode);
		%>
		<dl>
		<%-- 配送先：氏名 --%>
		<dt>
			<%: ReplaceTag("@@User.name.name@@") %>
			&nbsp;<span class="fred">※</span><span id='<%# "efo_sign_ship_name" + ((RepeaterItem)Container).ItemIndex %>'></span>
		</dt>
		<dd>
			姓&nbsp;&nbsp;&nbsp;&nbsp;<asp:TextBox ID="tbShippingName1" Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1) %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name1.length_max@@") %>' runat="server"></asp:TextBox>&nbsp;&nbsp;
			名&nbsp;&nbsp;&nbsp;&nbsp;<asp:TextBox ID="tbShippingName2" Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2) %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name2.length_max@@") %>' runat="server"></asp:TextBox><br />
		<small>
			<asp:CustomValidator
			ID="cvShippingName1"
			runat="Server"
			ControlToValidate="tbShippingName1"
			ValidationGroup="OrderShipping-OrderPayment"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" />
			<asp:CustomValidator
			ID="cvShippingName2"
			runat="Server"
			ControlToValidate="tbShippingName2"
			ValidationGroup="OrderShipping-OrderPayment"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" /></small>
		</dd>
		<%-- 配送先：氏名（かな） --%>
		<% if (isShippingAddrCountryJp) { %>
		<dt>
			<%: ReplaceTag("@@User.name_kana.name@@") %>
			&nbsp;<span class="fred">※</span><span id='<%# "efo_sign_ship_kana" + ((RepeaterItem)Container).ItemIndex %>'></span>
		</dt>
		<dd class="<%: ReplaceTag("@@User.name_kana.type@@") %>">
			姓&nbsp;&nbsp;<asp:TextBox ID="tbShippingNameKana1"  Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1) %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name_kana1.length_max@@") %>' runat="server"></asp:TextBox>&nbsp;&nbsp;
			名&nbsp;&nbsp;<asp:TextBox ID="tbShippingNameKana2"  Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2) %>" CssClass="input_widthA input_border" MaxLength='<%# GetMaxLength("@@User.name_kana2.length_max@@") %>' runat="server"></asp:TextBox><br />
		<small>
			<asp:CustomValidator
			ID="cvShippingNameKana1"
			runat="Server"
			ControlToValidate="tbShippingNameKana1"
			ClientValidationFunction="ClientValidate"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ValidationGroup="OrderShipping-OrderPayment"
			CssClass="error_inline" />
			<asp:CustomValidator
			ID="cvShippingNameKana2"
			runat="Server"
			ControlToValidate="tbShippingNameKana2"
			ValidationGroup="OrderShipping-OrderPayment"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" /></small>
		</dd>
		<% } %>
		<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
		<%-- 国 --%>
		<dt>
			<%: ReplaceTag("@@User.country.name@@", shippingAddrCountryIsoCode) %>
			&nbsp;<span class="fred">※</span>
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
		<%-- 配送先：郵便番号 --%>
		<% if (isShippingAddrCountryJp) { %>
		<dt>
			<%: ReplaceTag("@@User.zip.name@@") %>
			&nbsp;<span class="fred">※</span><span id='<%# "efo_sign_ship_zip" + ((RepeaterItem)Container).ItemIndex %>'></span>
		</dt>
		<dd>
		<p class="pdg_topC">
			<asp:TextBox ID="tbShippingZip" Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP) %>" OnTextChanged="lbSearchShippingAddr_Click" CssClass="input_widthC input_border" Type="tel" MaxLength="8" runat="server" /></p>
		<span class="btn_add_sea"><asp:LinkButton ID="lbSearchShippingAddr" runat="server" OnClick="lbSearchShippingAddr_Click" class="btn btn-mini" OnClientClick="return false;">住所検索</asp:LinkButton></span>
		<p class="clr">
		<small class="fred">
			<asp:CustomValidator
			ID="cvShippingZip1"
			runat="Server"
			ControlToValidate="tbShippingZip"
			ValidationGroup="OrderShipping-OrderPayment"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline cvShippingZipShortInput" /></small>
		<small id="sShippingZipError" runat="server" class="fred sShippingZipError"></small>
		</p></dd>
		<%-- 配送先：都道府県 --%>
		<dt>
			<%: ReplaceTag("@@User.addr1.name@@") %>
			&nbsp;<span class="fred">※</span><span id='<%# "efo_sign_ship_addr1" + ((RepeaterItem)Container).ItemIndex %>'></span>
		</dt>
		<dd><asp:DropDownList ID="ddlShippingAddr1" DataSource="<%# this.Addr1List %>" DataTextField="Text" DataValueField="Value" SelectedValue="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_ADDR1) %>" runat="server"></asp:DropDownList>
		<small>
			<asp:CustomValidator
			ID="cvShippingAddr1"
			runat="Server"
			ControlToValidate="ddlShippingAddr1"
			ValidationGroup="OrderShipping-OrderPayment"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" /></small>
		</dd>
		<% } %>
		<%-- 配送先：市区町村 --%>
		<dt>
			<%: ReplaceTag("@@User.addr2.name@@", shippingAddrCountryIsoCode) %>
			&nbsp;<span class="fred">※</span><% if (isShippingAddrCountryJp) { %><span id='<%# "efo_sign_ship_addr2" + ((RepeaterItem)Container).ItemIndex %>'></span><%} %>
		</dt>
		<dd>
			<% if (isShippingAddrCountryTw) { %>
				<asp:DropDownList runat="server" ID="ddlShippingAddr2" DataSource="<%# this.UserTwCityList %>" AutoPostBack="true" DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlShippingAddr2_SelectedIndexChanged"></asp:DropDownList>
			<% } else { %>
				<asp:TextBox ID="tbShippingAddr2" Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_ADDR2) %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.addr2.length_max@@") %>' runat="server"></asp:TextBox><br />
				<small>
					<asp:CustomValidator
					ID="cvShippingAddr2"
					runat="Server"
					ControlToValidate="tbShippingAddr2"
					ValidationGroup="OrderShipping-OrderPayment"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					CssClass="error_inline" /></small>
			<% } %>
		</dd>
		<%-- 配送先：番地 --%>
		<dt>
			<%: ReplaceTag("@@User.addr3.name@@", shippingAddrCountryIsoCode) %>
			<% if (isShippingAddrCountryJp) { %>&nbsp;<span class="fred">※</span><span id='<%# "efo_sign_ship_addr3" + ((RepeaterItem)Container).ItemIndex %>'></span><% } %>
		</dt>
		<dd>
			<% if (isShippingAddrCountryTw) { %>
				<asp:DropDownList runat="server" ID="ddlShippingAddr3" DataTextField = "Key" DataValueField = "Value" Width="95" ><asp:ListItem value="">區域</asp:ListItem></asp:DropDownList>
			<% } else { %>
				<asp:TextBox ID="tbShippingAddr3" Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_ADDR3) %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.addr3.length_max@@") %>' runat="server"></asp:TextBox><br />
				<small>
					<asp:CustomValidator
					ID="cvShippingAddr3"
					runat="Server"
					ControlToValidate="tbShippingAddr3"
					ValidationGroup="OrderShipping-OrderPayment"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					CssClass="error_inline" /></small>
			<% } %>
		</dd>
		<%-- 配送先：ビル・マンション名 --%>
		<dt>
			<%: ReplaceTag("@@User.addr4.name@@", shippingAddrCountryIsoCode) %>
			<% if (isShippingAddrCountryJp == false) { %>&nbsp;<span class="fred">※</span><% } %>
		</dt>
		<dd>
			<asp:TextBox ID="tbShippingAddr4" Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_ADDR4) %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.addr4.length_max@@") %>' runat="server"></asp:TextBox><br />
		<small>
			<asp:CustomValidator
				 ID="cvShippingAddr4"
				 runat="Server"
			ControlToValidate="tbShippingAddr4"
			ValidationGroup="OrderShipping-OrderPayment"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" /></small>
		</dd>
		<% if (isShippingAddrCountryJp == false) { %>
		<%-- 配送先：州 --%>
		<dt>
			<%: ReplaceTag("@@User.addr5.name@@", shippingAddrCountryIsoCode) %>
			<% if (isShippingAddrCountryUs) { %>&nbsp;<span class="fred">※</span><% } %>
		</dt>
		<dd>
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
					CssClass="error_inline" />
			<% } else {%>
			<asp:TextBox runat="server" ID="tbShippingAddr5" Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5) %>"></asp:TextBox>
			<small>
			<asp:CustomValidator
				ID="cvShippingAddr5"
				runat="Server"
				ControlToValidate="tbShippingAddr5"
				ValidationGroup="OrderShippingGlobal-OrderPayment"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				CssClass="error_inline" /></small>
			<% } %>
		</dd>
		<%-- 配送先：郵便番号（海外向け） --%>
		<dt>
			<%: ReplaceTag("@@User.zip.name@@", shippingAddrCountryIsoCode) %>
			<% if (isShippingAddrZipNecessary) { %>&nbsp;<span class="fred">※</span><% } %>
		</dt>
		<dd>
			<asp:TextBox runat="server" ID="tbShippingZipGlobal" MaxLength="20" Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP) %>"></asp:TextBox>
			<small>
			<asp:CustomValidator
				ID="cvShippingZipGlobal"
				runat="Server"
				ControlToValidate="tbShippingZipGlobal"
				ValidationGroup="OrderShippingGlobal-OrderPayment"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				CssClass="error_inline" /></small>
			<asp:LinkButton
				ID="lbSearchAddrShippingFromZipGlobal"
				OnClick="lbSearchAddrShippingFromZipGlobal_Click"
				Style="display:none;"
				Runat="server" />
		</dd>
		<% } %>
		<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
		<%-- 配送先：企業名 --%>
		<dt>
			<%: ReplaceTag("@@User.company_name.name@@") %>
			&nbsp;<span class="fred"></span>
		</dt>
		<dd><asp:TextBox ID="tbShippingCompanyName" Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_COMPANY_NAME) %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.company_name.length_max@@") %>' runat="server"></asp:TextBox><br />
		<small>
		<asp:CustomValidator
			ID="cvShippingCompanyName"
			runat="Server"
			ControlToValidate="tbShippingCompanyName"
			ValidationGroup="OrderShipping-OrderPayment"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" /></small>
		</dd>
		<%-- 配送先：部署名 --%>
		<dt>
			<%: ReplaceTag("@@User.company_post_name.name@@") %>
			&nbsp;<span class="fred"></span>
		</dt>
		<dd><asp:TextBox ID="tbShippingCompanyPostName" Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_USERSHIPPING_SHIPPING_COMPANY_POST_NAME) %>" CssClass="input_widthD input_border" MaxLength='<%# GetMaxLength("@@User.company_post_name.length_max@@") %>' runat="server"></asp:TextBox><br />
		<small>
		<asp:CustomValidator
			ID="cvShippingCompanyPostName"
			runat="Server"
			ControlToValidate="tbShippingCompanyPostName"
			ValidationGroup="OrderShipping-OrderPayment"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" /></small>
		</dd>
		<%} %>
		<%-- 配送先：電話番号 --%>
		<% if (isShippingAddrCountryJp) { %>
		<dt>
			<%: ReplaceTag("@@User.tel1.name@@") %>
			&nbsp;<span class="fred">※</span><span id='<%# "efo_sign_ship_tel1" + ((RepeaterItem)Container).ItemIndex %>'></span>
		</dt>
		<dd><asp:TextBox ID="tbShippingTel1" Text="<%#: GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %>" CssClass="input_widthD input_border shortTel" MaxLength="13" Type="tel" runat="server" />
		<small>
			<asp:CustomValidator
			ID="cvShippingTel1_1"
			runat="Server"
			ControlToValidate="tbShippingTel1"
			ValidationGroup="OrderShipping-OrderPayment"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" /></small>
		</dd>
		<% } else { %>
		<%-- 配送先：電話番号1（海外向け） --%>
		<dt>
			<%: ReplaceTag("@@User.tel1.name@@", shippingAddrCountryIsoCode) %>
			&nbsp;<span class="fred">※</span></dt>
		<dd>
			<asp:TextBox runat="server" ID="tbShippingTel1Global" MaxLength="30" Text="<%# GetShippingValue((CartObject)((RepeaterItem)Container).DataItem, Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %>"></asp:TextBox>
			<small>
			<asp:CustomValidator ID="CustomValidator2" runat="Server"
				ControlToValidate="tbShippingTel1Global"
				ValidationGroup="OrderShippingGlobal-OrderPayment"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				CssClass="error_inline" /></small>
		</dd>
		<% } %>
		</dl>
		</div>
		<div id="divSaveToUserShipping" class="subbox" visible="<%# this.IsLoggedIn %>" runat="server">
		<p>
		<%-- ポストバックCustomValidatorの状態がクリアされてしまうため、JaavScirptで表示非表示を制御する --%>
			<asp:RadioButtonList ID="rblSaveToUserShipping" OnSelectedIndexChanged="rblSaveToUserShipping_OnSelectedIndexChanged" AutoPostBack="true" SelectedValue='<%# ((CartObject)((RepeaterItem)Container).DataItem).Shippings[0].UserShippingRegistFlg ? "1" : "0" %>' RepeatLayout="Flow" CssClass="radioBtn" runat="server">
		<asp:ListItem Text="配送先情報を保存しない" Value="0"></asp:ListItem>
		<asp:ListItem Text="配送先情報を保存する" Value="1"></asp:ListItem>
		</asp:RadioButtonList>
		</p>
		<img src="<%: Constants.PATH_ROOT %>Contents/ImagesPkg/common/btm_sub_boxA.gif" alt="bottom" width="298" height="4" /></div>
		<!--subbox-->

		<dl id="dlUserShippingName" visible="false" runat="server">
		<dt><span>配送先を保存する場合は、以下をご入力ください。</span></dt>
		<dt>配送先名&nbsp;<span class="fred">※</span><span id='<%# "efo_sign_ship_addr_name" + ((RepeaterItem)Container).ItemIndex %>'></span></dt>
			<dd class="last"><asp:TextBox ID="tbUserShippingName" Text="<%# ((CartObject)((RepeaterItem)Container).DataItem).Shippings[0].UserShippingName %>" MaxLength="30" CssClass="input_widthD input_border" runat="server"></asp:TextBox><br />
			<asp:CustomValidator
			ID="cvUserShippingName"
			runat="Server"
			ControlToValidate="tbUserShippingName"
			ValidationGroup="OrderShipping-OrderPayment"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" /></small>
		</dd>
		</dl>
		</div>
		</div><!--userList-->
			</div><!--orderBox-->
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
			<div id="shippingAddressBookContainer" style='<%: (this.IsLoggedIn == false) && (this.CartList.Items.Count > 0) && this.CartList.Items[0].GetShipping().IsSameShippingAsCart1 ? "display:none;" : "" %>'>
				<%-- ▼▼Amazonアドレス帳ウィジェット(配送先情報)▼▼ --%>
				<div id="shippingAddressBookWidgetDiv" style="width:340px;height:300px;"></div>
				<div id="shippingAddressBookErrorMessage" style="color:red;padding:5px" ClientIDMode="Static" runat="server"></div>
				<%-- ▲▲Amazonアドレス帳ウィジェット(配送先情報)▲▲ --%>
			</div>
			<% } %>
		<% } %>
		<%-- UPDATE PANEL開始(その他情報) --%>
		<asp:UpdatePanel ID="upOthersUpdatePanel" runat="server">
		<ContentTemplate>
			<div class="orderBox" style="margin:0;">
			<div class="bottom">
				<span id="sInvoices" runat="server" visible="false">
					<div id="divUniformInvoiceType" runat="server">
						<h4>発票種類</h4>
						<div class="userList">
							<dl>
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
							</dl>
							<dl id="dlUniformInvoiceOption1_8" runat="server" visible="false">
								<br />
								<dd>統一編号</dd>
								<dd>
									<asp:TextBox ID="tbUniformInvoiceOption1_8" placeholder="例:12345678" Text="<%# ((CartObject)((RepeaterItem)Container).DataItem).Shippings[0].UniformInvoiceOption1 %>" Width="220" runat="server" MaxLength="8"/>
									<asp:CustomValidator
										ID="cvUniformInvoiceOption1_8" runat="server"
										ControlToValidate="tbUniformInvoiceOption1_8"
										ValidationGroup="OrderShippingGlobal"
										ValidateEmptyText="true"
										ClientValidationFunction="ClientValidate"
										SetFocusOnError="true"
										CssClass="error_inline" />
									<asp:Label ID="lbUniformInvoiceOption1_8" runat="server" Text="<%# ((CartObject)((RepeaterItem)Container).DataItem).Shippings[0].UniformInvoiceOption1 %>" Visible="false"></asp:Label>
								</dd>
								<br />
								<dd>会社名</dd>
								<dd>
									<asp:TextBox ID="tbUniformInvoiceOption2" placeholder="例:○○有限股份公司" Text="<%# ((CartObject)((RepeaterItem)Container).DataItem).Shippings[0].UniformInvoiceOption2 %>" Width="220" runat="server" MaxLength="20"/>
									<asp:CustomValidator
										ID="cvUniformInvoiceOption2" runat="server"
										ControlToValidate="tbUniformInvoiceOption2"
										ValidationGroup="OrderShippingGlobal"
										ValidateEmptyText="true"
										ClientValidationFunction="ClientValidate"
										SetFocusOnError="true"
										CssClass="error_inline" />
									<asp:Label ID="lbtbUniformInvoiceOption2" runat="server" Text="<%# ((CartObject)((RepeaterItem)Container).DataItem).Shippings[0].UniformInvoiceOption2 %>" Visible="false"></asp:Label>
								</dd>
							</dl>
							<dl id="dlUniformInvoiceOption1_3" runat="server" visible="false">
								<br />
								<dd>寄付先コード</dd>
								<dd>
									<asp:TextBox ID="tbUniformInvoiceOption1_3" Text="<%# ((CartObject)((RepeaterItem)Container).DataItem).Shippings[0].UniformInvoiceOption1 %>" Width="220" runat="server" MaxLength="7" />
									<asp:CustomValidator
										ID="cvUniformInvoiceOption1_3" runat="server"
										ControlToValidate="tbUniformInvoiceOption1_3"
										ValidationGroup="OrderShippingGlobal"
										ValidateEmptyText="true"
										ClientValidationFunction="ClientValidate"
										SetFocusOnError="true"
										CssClass="error_inline" />
									<asp:Label ID="lbUniformInvoiceOption1_3" Text="<%# ((CartObject)((RepeaterItem)Container).DataItem).Shippings[0].UniformInvoiceOption1 %>" runat="server" Visible="false"></asp:Label>
								</dd>
							</dl>
							<dl id="dlUniformInvoiceTypeRegist" runat="server" visible="false">
								<dd visible="<%# this.IsLoggedIn %>" runat="server">
									<asp:CheckBox ID="cbSaveToUserInvoice" Checked="<%# ((CartObject)((RepeaterItem)Container).DataItem).Shippings[0].UserInvoiceRegistFlg %>" runat="server" Text="電子発票管理情報を保存する" AutoPostBack="true" OnCheckedChanged="cbSaveToUserInvoice_CheckedChanged" />
								</dd>
								<div id="dlUniformInvoiceTypeRegistInput" runat="server" visible="false">
									電子発票情報名 <span class="fred">※</span><br />
									<asp:TextBox ID="tbUniformInvoiceTypeName" MaxLength="30" runat="server" Text="<%# ((CartObject)((RepeaterItem)Container).DataItem).Shippings[0].InvoiceName %>"></asp:TextBox>
									<asp:CustomValidator
										ID="cvUniformInvoiceTypeName" runat="server"
										ControlToValidate="tbUniformInvoiceTypeName"
										ValidationGroup="OrderShippingGlobal"
										ValidateEmptyText="true"
										ClientValidationFunction="ClientValidate"
										SetFocusOnError="true"
										CssClass="error_inline" />
								</div>
							</dl>
						</div>
					</div>
					<div id="divInvoiceCarryType" runat="server">
						<h4>共通性載具</h4>
						<div class="userList">
							<dl>
								<dd>
									<asp:DropDownList ID="ddlInvoiceCarryType" runat="server"
										CssClass="input_border"
										DataSource="<%# ValueText.GetValueItemList(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE) %>"
										DataTextField="text"
										DataValueField="value"
										OnSelectedIndexChanged="ddlInvoiceCarryType_SelectedIndexChanged"
										AutoPostBack="true"></asp:DropDownList><br />
									<asp:DropDownList ID="ddlInvoiceCarryTypeOption" runat="server"
										CssClass="input_border"
										DataTextField="text"
										DataValueField="value"
										OnSelectedIndexChanged="ddlInvoiceCarryTypeOption_SelectedIndexChanged"
										AutoPostBack="true"
										Visible="false"
										style="margin-top:5px">
									</asp:DropDownList>
								</dd>
								<br />
								<div id="divCarryTypeOption" runat ="server">
									<div id="divCarryTypeOption_8" runat="server" visible="false">
										<asp:TextBox ID="tbCarryTypeOption_8" Width="220" runat="server" Text="<%# ((CartObject)((RepeaterItem)Container).DataItem).Shippings[0].CarryTypeOptionValue %>" placeholder="例:/AB201+9(限8個字)" MaxLength="8" />
										<asp:CustomValidator
										ID="cvCarryTypeOption_8" runat="server"
										ControlToValidate="tbCarryTypeOption_8"
										ValidationGroup="OrderShippingGlobal"
										ValidateEmptyText="true"
										ClientValidationFunction="ClientValidate"
										SetFocusOnError="true"
										CssClass="error_inline" />
									</div>
									<div id="divCarryTypeOption_16" runat="server" visible="false">
										<asp:TextBox ID="tbCarryTypeOption_16" Width="220" Text="<%# ((CartObject)((RepeaterItem)Container).DataItem).Shippings[0].CarryTypeOptionValue %>" runat="server" placeholder="例:TP03000001234567(限16個字)" MaxLength="16" />
										<asp:CustomValidator
											ID="cvCarryTypeOption_16" runat="server"
											ControlToValidate="tbCarryTypeOption_16"
											ValidationGroup="OrderShippingGlobal"
											ValidateEmptyText="true"
											ClientValidationFunction="ClientValidate"
											SetFocusOnError="true"
											CssClass="error_inline" />
									</div>
								</div>
								<dl id="dlCarryTypeOptionRegist" runat="server" visible="false">
									<dd visible="<%# this.IsLoggedIn %>" runat="server">
										<asp:CheckBox ID="cbCarryTypeOptionRegist" runat="server" Text="電子発票管理情報を保存する" AutoPostBack="true" OnCheckedChanged="cbCarryTypeOptionRegist_CheckedChanged" />
									</dd>
									<div id="divCarryTypeOptionName" runat="server" visible="false">
										電子発票情報名 <span class="fred">※</span><br />
										<asp:TextBox ID="tbCarryTypeOptionName" Text="<%# ((CartObject)((RepeaterItem)Container).DataItem).Shippings[0].InvoiceName %>" runat="server" MaxLength="30"></asp:TextBox>
										<asp:CustomValidator
											ID="cvCarryTypeOptionName" runat="server"
											ControlToValidate="tbCarryTypeOptionName"
											ValidationGroup="OrderShippingGlobal"
											ValidateEmptyText="true"
											ClientValidationFunction="ClientValidate"
											SetFocusOnError="true"
											CssClass="error_inline" />
									</div>
								</dl>
								<asp:Label runat="server" ID="lbCarryTypeOption" Visible="false"></asp:Label>
							</dl>
						</div>
					</div>
				</span>
			<h4 id="h4DeliveryOptions" visible="<%# CanInputShippingTo(((RepeaterItem)Container).ItemIndex) %>" runat="server">配送指定</h4>
				<div id="dvStorePickUp" visible="false" runat="server">
					<div runat="server" class="userList">
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
									runat="server">
								</asp:DropDownList>
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
						<dl id="dlRealShopAddress" Visible="false" runat="server">
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
						<dl id="dlRealShopOpenningHours" Visible="false" runat="server">
							<dt runat="server">営業時間：</dt>
							<dd>
								<asp:Literal ID="lRealShopOpenningHours" runat="server" />
							</dd>
						</dl>
						<dl id="dlRealShopTel" Visible="false" runat="server">
							<dt runat="server">電話番号：</dt>
							<dd>
								<asp:Literal ID="lRealShopTel" runat="server" />
							</dd>
						</dl>
					</div>
				</div>
		<div id="dvShippingMethod" visible="<%# CanInputShippingTo(((RepeaterItem)Container).ItemIndex) %>" runat="server" class="userList">
			配送方法を選択して下さい。
			<asp:DropDownList ID="ddlShippingMethod" DataSource="<%# this.ShippingMethodList[((RepeaterItem)Container).ItemIndex] %>" OnSelectedIndexChanged="ddlShippingMethodList_OnSelectedIndexChanged" DataTextField="text" DataValueField="value" AutoPostBack="true" runat="server"></asp:DropDownList>
		</div>
		<div id="dvDeliveryCompany" visible="<%# (CanInputShippingTo(((RepeaterItem)Container).ItemIndex) && CanDisplayDeliveryCompany(((RepeaterItem)Container).ItemIndex)) %>" runat="server" class="userList">
			配送サービスを選択して下さい。
			<asp:DropDownList ID="ddlDeliveryCompany" DataSource="<%# GetDeliveryCompanyListItem(((RepeaterItem)Container).ItemIndex) %>" OnSelectedIndexChanged="ddlDeliveryCompanyList_OnSelectedIndexChanged" DataTextField="Value" DataValueField="Key" AutoPostBack="true" runat="server"/>
		</div>
			<div id="dvShipppingDateTime" visible="<%# CanInputDateOrTimeSet(((RepeaterItem)Container).ItemIndex) %>" runat="server" class="userList" style='<%# HasFixedPurchase((RepeaterItem)Container) && (DisplayFixedPurchaseShipping((RepeaterItem)Container) == false) ? "padding-bottom: 0px" : "" %>'>
		配送希望日時を選択して下さい。
		<dl id="dlShipppingDateTime" runat="server">
			<dd></dd>
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
				<asp:Label ID="lShippingDateErrorMessage" CssClass="fred" runat="server"></asp:Label>
			</dd>
			<div id="divShippingTime" runat="server">
				<dt id="dtShippingTime" visible="<%# CanInputTimeSet(((RepeaterItem)Container).ItemIndex) %>" runat="server">配送希望時間帯</dt>
				<dd id="ddShippingTime" visible="<%# CanInputTimeSet(((RepeaterItem)Container).ItemIndex) %>" runat="server" class="last">
					<asp:DropDownList ID="ddlShippingTime" runat="server" DataSource="<%# GetShippingTimeList(((RepeaterItem)Container).ItemIndex) %>" DataTextField="Text" DataValueField="Value" SelectedValue="<%# GetShippingTime(((RepeaterItem)Container).ItemIndex) %>"></asp:DropDownList>
				</dd>
			</div>
		</dl>
		</div><!--userList-->

			<h4 style="margin-top:15px" visible="<%# DisplayFixedPurchaseShipping((RepeaterItem)Container) %>" runat="server">定期購入 配送パターンの指定</h4>
		<%-- ▽デフォルトチェックの設定▽--%>
			<%-- ラジオボタンのデータバインド <%#.. より前で呼び出してください。 --%>
			<%# Constants.FIXED_PURCHASE_USESHIPPINGINTERVALDAYSDEFAULT_FLG ? SetFixedPurchaseDefaultCheckPriority(((RepeaterItem)Container).ItemIndex, 3, 2, 1, 4) : SetFixedPurchaseDefaultCheckPriority(((RepeaterItem)Container).ItemIndex, 2, 3, 1, 4) %>
		<%-- △ - - - - - - - - - - - △--%>
			<div visible="<%# DisplayFixedPurchaseShipping((RepeaterItem)Container) %>" runat="server" class="userList orderBox list" style='<%# DisplayFixedPurchaseShipping((RepeaterItem)Container) ? "" : "margin-top: 0px;padding-top: 0px" %>'><span id="efo_sign_fixed_purchase"></span>
				<span visible="<%# DisplayFixedPurchaseShipping((RepeaterItem)Container) %>" runat="server">「定期購入」はご希望の配送パターン・配送時間を指定して定期的に商品をお届けするサービスです。下記の配送パターンからお選び下さい。</span>

			<dl style="margin-top: 10px;" visible="<%# DisplayFixedPurchaseShipping((RepeaterItem)Container) %>" runat="server">
					<dt id="Dt1" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE, false) %>" runat="server">
					<asp:RadioButton ID="rbFixedPurchaseMonthlyPurchase_Date" 
							Text="月間隔日付指定" Checked="<%# GetFixedPurchaseKbnInputChecked(((RepeaterItem)Container).ItemIndex, 1) %>" 
						GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" /><span id='<%# "efo_sign_fixed_purchase_month" + ((RepeaterItem)Container).ItemIndex %>'></span></dt>
					<dd id="ddFixedPurchaseMonthlyPurchase_Date" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE, true) %>" runat="server">
					<asp:DropDownList ID="ddlFixedPurchaseMonth"
							DataSource="<%# GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container).ItemIndex, true) %>"
							DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container).ItemIndex, Constants.FIXED_PURCHASE_SETTING_MONTH) %>'
						OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnSelectedIndexChanged" AutoPostBack="true" runat="server">
					</asp:DropDownList>
					ヶ月ごと
					<asp:DropDownList ID="ddlFixedPurchaseMonthlyDate"
						DataSource="<%# GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container).ItemIndex, true, false, true) %>"
							DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container).ItemIndex, Constants.FIXED_PURCHASE_SETTING_MONTHLY_DATE) %>'
						OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnSelectedIndexChanged" AutoPostBack="true"
						runat="server">
					</asp:DropDownList>
					日に届ける
				</dd>
				<small><asp:CustomValidator
					ID="cvFixedPurchaseMonth"
					runat="Server"
					ControlToValidate="ddlFixedPurchaseMonth" 
					ValidationGroup="OrderShipping" 
					ValidateEmptyText="true" 
					SetFocusOnError="true" 
					CssClass="error_inline" />
				</small>
				<small><asp:CustomValidator
					ID="cvFixedPurchaseMonthlyDate"
					runat="Server"
					ControlToValidate="ddlFixedPurchaseMonthlyDate" 
					ValidationGroup="OrderShipping" 
					ValidateEmptyText="true" 
					SetFocusOnError="true" 
					CssClass="error_inline" />
				</small>
					<dt id="Dt2" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY, false) %>" runat="server">
					<asp:RadioButton ID="rbFixedPurchaseMonthlyPurchase_WeekAndDay" 
							Text="月間隔・週・曜日指定" Checked="<%# GetFixedPurchaseKbnInputChecked(((RepeaterItem)Container).ItemIndex, 2) %>" 
						GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" /><span id='<%# "efo_sign_fixed_purchase_week_and_day" + ((RepeaterItem)Container).ItemIndex %>' ></span></dt>
					<dd id="ddFixedPurchaseMonthlyPurchase_WeekAndDay" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY, true) %>" runat="server">
					<asp:DropDownList ID="ddlFixedPurchaseIntervalMonths"
						DataSource="<%# GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container).ItemIndex, true, true) %>"
						DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container).ItemIndex, Constants.FIXED_PURCHASE_SETTING_INTERVAL_MONTHS) %>'
						OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnSelectedIndexChanged" AutoPostBack="true" runat="server" />
					ヶ月ごと
					<asp:DropDownList ID="ddlFixedPurchaseWeekOfMonth"
						DataSource="<%# ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_WEEK_LIST) %>"
							DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container).ItemIndex, Constants.FIXED_PURCHASE_SETTING_WEEK_OF_MONTH) %>'
						OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnSelectedIndexChanged" AutoPostBack="true"
						runat="server">
					</asp:DropDownList>
					<asp:DropDownList ID="ddlFixedPurchaseDayOfWeek"
						DataSource="<%# ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DAY_LIST) %>"
							DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container).ItemIndex, Constants.FIXED_PURCHASE_SETTING_DAY_OF_WEEK) %>'
						OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnSelectedIndexChanged" AutoPostBack="true"
						runat="server">
					</asp:DropDownList>
					に届ける
				</dd>
				<small><asp:CustomValidator ID="cvFixedPurchaseIntervalMonths" runat="Server"
					ControlToValidate="ddlFixedPurchaseIntervalMonths"
					ValidationGroup="OrderShipping"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					CssClass="error_inline" />
				</small>
				<small><asp:CustomValidator
					ID="cvFixedPurchaseWeekOfMonth"
					runat="Server"
					ControlToValidate="ddlFixedPurchaseWeekOfMonth" 
					ValidationGroup="OrderShipping" 
					ValidateEmptyText="true" 
					SetFocusOnError="true" 
					CssClass="error_inline" />
				</small>
				<small><asp:CustomValidator
					ID="cvFixedPurchaseDayOfWeek"
					runat="Server"
					ControlToValidate="ddlFixedPurchaseDayOfWeek" 
					ValidationGroup="OrderShipping" 
					ValidateEmptyText="true" 
					SetFocusOnError="true" 
					CssClass="error_inline" />
				</small>
					<dt id="Dt3" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS, false) %>" runat="server">
					<asp:RadioButton ID="rbFixedPurchaseRegularPurchase_IntervalDays" 
							Text="配送日間隔指定" Checked="<%# GetFixedPurchaseKbnInputChecked(((RepeaterItem)Container).ItemIndex, 3) %>" 
						GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" /><span id='<%# "efo_sign_fixed_purchase_interval_days" + ((RepeaterItem)Container).ItemIndex %>' ></span></dt>
					<dd id="ddFixedPurchaseRegularPurchase_IntervalDays" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS, true) %>" runat="server">
					<asp:DropDownList ID="ddlFixedPurchaseIntervalDays"
							DataSource='<%# GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container).ItemIndex, false) %>'
							DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container).ItemIndex, Constants.FIXED_PURCHASE_SETTING_INTERVAL_DAYS) %>'
						OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnSelectedIndexChanged" AutoPostBack="true"
						runat="server">
					</asp:DropDownList>
					日ごとに届ける
				</dd>
					<asp:HiddenField ID="hfFixedPurchaseDaysRequired" Value="<%# this.ShopShippingList[((RepeaterItem)Container).ItemIndex].FixedPurchaseShippingDaysRequired %>" runat="server" />
					<asp:HiddenField ID="hfFixedPurchaseMinSpan" Value="<%# this.ShopShippingList[((RepeaterItem)Container).ItemIndex].FixedPurchaseMinimumShippingSpan %>" runat="server" />
				<small><asp:CustomValidator
					ID="cvFixedPurchaseIntervalDays"
					runat="Server"
					ControlToValidate="ddlFixedPurchaseIntervalDays" 
					ValidationGroup="OrderShipping" 
					ValidateEmptyText="true" 
					SetFocusOnError="true" 
					CssClass="error_inline" />
				</small>
				<dt id="Dt4" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY, false) %>" runat="server">
					<asp:RadioButton ID="rbFixedPurchaseEveryNWeek"
						Text="週間隔・曜日指定" Checked="<%# GetFixedPurchaseKbnInputChecked(((RepeaterItem)Container).ItemIndex, 4) %>"
						GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" /><span id="<%#: "efo_sign_fixed_purchase_week" + ((RepeaterItem)Container).ItemIndex %>" /></dt>
				<dd id="ddFixedPurchaseEveryNWeek" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY, true) %>" runat="server">
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
				</dd>
				<small>
				<asp:CustomValidator
					ID="cvFixedPurchaseEveryNWeek"
					runat="Server"
					ControlToValidate="ddlFixedPurchaseEveryNWeek_Week"
					ValidationGroup="OrderShipping"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					CssClass="error_inline"/>
				</small>
				<small>
				<asp:CustomValidator
					ID="cvFixedPurchaseEveryNWeekDayOfWeek"
					runat="Server"
					ControlToValidate="ddlFixedPurchaseEveryNWeek_DayOfWeek"
					ValidationGroup="OrderShipping"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					CssClass="error_inline"/>
				</small>
			</dl>
			<small><p id="P2" class="attention" runat="server" visible="<%# GetAllFixedPurchaseKbnEnabled(((RepeaterItem)Container).ItemIndex) == false %>">同時に定期購入できない商品が含まれております。</p></small>
			<small ID="sErrorMessage" class="fred" runat="server"></small>
			<br /><hr />
			<dl>
				<dt id="dtFirstShippingDate" visible="true" runat="server">初回配送予定日</dt>
				<dd id="Dd4" visible="true" runat="server" style="padding-left: 20px;">
					<asp:Label ID="lblFirstShippingDate" runat="server"></asp:Label>
					<asp:DropDownList
						ID="ddlFirstShippingDate"
						visible="false"
						OnDataBound="ddlFirstShippingDate_OnDataBound"
						AutoPostBack="True"
						OnSelectedIndexChanged="ddlFirstShippingDate_ItemSelected"
						runat="server" />
					<asp:Literal ID="lFirstShippingDateDayOfWeekNoteMessage" visible="false" runat="server">
						<br>曜日指定は次回配送日より適用されます。
					</asp:Literal>
				</dd>
				<dt id="dtNextShippingDate" visible="true" runat="server">2回目の配送日を選択</dt>
				<dd id="Dd5" visible="true" runat="server" style="padding-left: 20px;">
					<asp:Label ID="lblNextShippingDate" visible="false" runat="server"></asp:Label>
					<asp:DropDownList
						ID="ddlNextShippingDate"
						visible="false"
						OnDataBound="ddlNextShippingDate_OnDataBound"
						OnSelectedIndexChanged="ddlNextShippingDate_OnSelectedIndexChanged"
						AutoPostBack="True"
						runat="server" />
				</dd>
			</dl>
		</div>

			<asp:Repeater ID="rMemos" runat="server" DataSource="<%# ((CartObject)((RepeaterItem)Container).DataItem).OrderMemos %>" Visible="<%# ((CartObject)((RepeaterItem)Container).DataItem).OrderMemos.Count != 0 %>">
		<HeaderTemplate>
			<h4>注文メモ</h4>
			<div class="list">
		</HeaderTemplate>
		<ItemTemplate>
			<strong><%# WebSanitizer.HtmlEncode(Eval(CartOrderMemo.FIELD_ORDER_MEMO_NAME)) %></strong>
			<p><asp:TextBox ID="tbMemo"  runat="server" Text="<%# Eval(CartOrderMemo.FIELD_ORDER_MEMO_TEXT) %>" CssClass="<%# Eval(CartOrderMemo.FIELD_ORDER_MEMO_CSS) %>" TextMode="MultiLine"></asp:TextBox><br /></p><br />
			<small id="sErrorMessageMemo" runat="server" class="fred" ></small>
		</ItemTemplate>
		<FooterTemplate>
			</div>
		</FooterTemplate>
		</asp:Repeater>
		<asp:Repeater ID="rOrderExtendInput" ItemType="OrderExtendItemInput" runat="server" Visible="<%# IsDisplayOrderExtend() %>" >
			<HeaderTemplate>
				<h4>注文確認事項</h4>
				<div class="list">
			</HeaderTemplate>
			<ItemTemplate>
				<%-- 項目名 --%>
				<p>
					<strong><%#: Item.SettingModel.SettingName %></strong>
					<span class="fred"  runat="server" visible="<%# Item.SettingModel.IsNeecessary%>">※</span>
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
					<small><asp:Label runat="server" ID="lbErrMessage" CssClass="error_inline"></asp:Label></small>
					<asp:HiddenField ID="hfSettingId" runat="server" Value="<%# Item.SettingModel.SettingId %>" />
					<asp:HiddenField ID="hfInputType" runat="server" Value="<%# Item.SettingModel.InputType %>" />
				</p>
			</ItemTemplate>
			<FooterTemplate>
				</div>
			</FooterTemplate>
		</asp:Repeater>
			</div><!--bottom-->
			</div>
		</ContentTemplate>
		</asp:UpdatePanel>
		<%-- UPDATE PANELここまで(その他情報) --%>
		<%-- ▲配送先情報▲ --%>

		<%-- ▼お支払い情報▼ --%>
		<% if (this.IsAmazonLoggedIn == false) { %>
		<%-- UPDATE PANEL開始(お支払い情報) --%>
		<asp:UpdatePanel ID="upPaymentUpdatePanel" runat="server" Visible="<%#(this.IsAmazonLoggedIn == false) %>">
		<ContentTemplate>
			<h2><img src="<%: Constants.PATH_ROOT %>Contents/ImagesPkg/order/sttl_cash.gif" alt="お支払い情報" width="96" height="16" /></h2>
		<div class="column">
			<div id="Div8" visible="<%# (((RepeaterItem)Container).ItemIndex == 0) %>" runat="server">
		<p class="pdg_bottomA">お支払い方法を選択し以下の内容をご入力ください。<br /><span class="fred">※</span>&nbsp;は必須入力です。</p>
		</div>

		<div class="orderBox">
			<h3>カート番号<%# ((RepeaterItem)Container).ItemIndex + 1 %></h3>
		<div class="bottom">
		<div class="list">
			<span id="Span10" style="color:red" runat="server" visible="<%# (string.IsNullOrEmpty(StringUtility.ToEmpty(this.DispLimitedPaymentMessages[((RepeaterItem)Container).ItemIndex])) == false) %>">
				<%# StringUtility.ToEmpty(this.DispLimitedPaymentMessages[((RepeaterItem)Container).ItemIndex]) %>
		</span>
		<asp:CheckBox ID="cbUseSamePaymentAddrAsCart1" visible="<%# (((RepeaterItem)Container).ItemIndex != 0) %>" Checked="<%# ((CartObject)((RepeaterItem)Container).DataItem).Payment.IsSamePaymentAsCart1 %>" Text="カート番号「１」同じお支払いを指定する" OnCheckedChanged="cbUseSamePaymentAddrAsCart1_OnCheckedChanged" AutoPostBack="true" CssClass="checkBox" runat="server" />

		<dl class="list">

		<%--▼▼ クレジット Token保持用（カート1と同じ決済の場合） ▼▼--%>
		<asp:HiddenField ID="hfCreditTokenSameAs1" Value="<%# ((CartObject)((RepeaterItem)Container).DataItem).Payment.CreditTokenSameAs1 %>" runat="server" />
		<%--▲▲ クレジット Token保持用（カート1と同じ決済の場合） ▲▲--%>

		<span id='<%# "efo_sign_payment" + ((RepeaterItem)Container).ItemIndex %>' ></span>
		<asp:HiddenField ID="hfPaidyTokenId" runat="server" />
		<asp:HiddenField ID="hfPaidyPaySelected" runat="server" />
		<% if ((Constants.PAYMENT_CHOOSE_TYPE_LP_OPTION
				? string.IsNullOrEmpty(this.LandingPageDesignModel.PaymentChooseType)
					? Constants.PAYMENT_CHOOSE_TYPE : this.LandingPageDesignModel.PaymentChooseType
				: Constants.PAYMENT_CHOOSE_TYPE) == Constants.PAYMENT_CHOOSE_TYPE_DDL) { %>
			<dt>
				<asp:DropDownList
					ID="ddlPayment"
					runat="server"
					DataSource="<%# this.ValidPayments[((RepeaterItem)Container).ItemIndex] %>"
					SelectedValue='<%# GetSelectedPaymentId(this.ValidPayments[((RepeaterItem)Container).ItemIndex], ((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment) %>'
					ItemType="w2.Domain.Payment.PaymentModel"
					visible="<%# (((RepeaterItem)Container).ItemIndex == 0) %>"
					OnSelectedIndexChanged="rbgPayment_OnCheckedChanged"
					AutoPostBack="true"
					DataTextField="PaymentName"
					DataValueField="PaymentId" />
			</dt>
		<% } %>
		<asp:Repeater ID="rPayment" runat="server" DataSource="<%# this.ValidPayments[((RepeaterItem)Container).ItemIndex] %>" ItemType="w2.Domain.Payment.PaymentModel" visible="<%# (((RepeaterItem)Container).ItemIndex == 0) %>">
		<ItemTemplate>
		<div id="<%# Item.PaymentId %>">
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
			<dt><w2c:RadioButtonGroup ID="rbgPayment" Checked='<%# ((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment.PaymentId == Item.PaymentId %>' GroupName='<%# "Payment_" + GetParentRepeaterItem(Container, "rCartList").ItemIndex %>' Text="<%# WebSanitizer.HtmlEncode(Item.PaymentName) %>" OnCheckedChanged="rbgPayment_OnCheckedChanged" AutoPostBack="true" CssClass="radioBtn" runat="server" /></dt>
			<% } %>

			<%-- クレジット --%>
			<dd id="ddCredit" runat="server">
			<p id="P3" runat="server" visible="<%# OrderCommon.GetRegistedCreditCardSelectable(this.IsLoggedIn, this.CreditCardList.Count - 1)%>">
				<asp:DropDownList ID="ddlUserCreditCard" runat="server" DataSource="<%# this.CreditCardList %>" SelectedValue='<%# GetListItemValue(this.CreditCardList ,((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment.CreditCardBranchNo) %>' OnSelectedIndexChanged="ddlUserCreditCard_OnSelectedIndexChanged" AutoPostBack="true" DataTextField="text" DataValueField="value" ></asp:DropDownList></p>
			<%-- ▽新規カード▽ --%>
				<div id="divCreditCardInputForm" runat="server" visible='<%# IsNewCreditCard(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment) %>'>
				
			<% if(this.IsCreditCardLinkPayment() == false) { %>
			<%--▼▼ クレジット Token保持用 ▼▼--%>
				<asp:HiddenField ID="hfCreditToken" Value='<%# ((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment.CreditToken %>' runat="server" />
			<%--▲▲ クレジット Token保持用 ▲▲--%>
			<%--▼▼ カード情報取得用 ▼▼--%>
				<input type="hidden" id="hidCinfo" name="hidCinfo" value="<%# CreateGetCardInfoJsScriptForCreditTokenForCart(GetParentRepeaterItem(Container, "rCartList"), Container) %>" />
			<span id="spanErrorMessageForCreditCard" style="color: red; display: none" runat="server"></span>
			<%--▲▲ カード情報取得用 ▲▲--%>
	
			<%--▼▼ カード情報入力（トークン未取得・利用なし） ▼▼--%>
			<%if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten) { %>
			<!-- Rakuten Card Form -->
			<asp:LinkButton id="lbEditCreditCardNoForRakutenToken" CssClass="lbEditCreditCardNoForRakutenToken" OnClick="lbEditCreditCardNoForToken_Click" runat="server">再入力</asp:LinkButton>
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
			<% if (OrderCommon.CreditCompanySelectable) {%>
			<strong>カード会社</strong>
				<p><asp:DropDownList id="ddlCreditCardCompany" runat="server" DataSource="<%# this.CreditCompanyList %>" DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetCreditValue(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment, CartPayment.FIELD_CREDIT_COMPANY) %>' CssClass="input_widthG input_border"></asp:DropDownList></p>
			<% } %>
			<strong>カード番号</strong>&nbsp;<span class="fred">※</span><span id='<%# "efo_sign_credit_card_no" + GetParentRepeaterItem(Container, "rCartList").ItemIndex %>' ></span>
				<p><w2c:ExtendedTextBox id="tbCreditCardNo1" runat="server" MaxLength="16" Text='<%# GetCreditValue(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment, CartPayment.FIELD_CREDIT_CARD_NO) %>' autocomplete="off" Type="tel"></w2c:ExtendedTextBox><br />
				<small class="fred">
				<asp:CustomValidator
					ID="cvCreditCardNo1"
					runat="Server"
					ControlToValidate="tbCreditCardNo1"
					ValidationGroup="OrderShipping-OrderPayment"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					CssClass="error_inline" />
					<span id="sErrorMessage" style="color :Red" runat="server" />
				</small>
			<small class="fgray">
			カードの表記のとおりご入力ください。<br />
			例：<br />
				1234567890123456（ハイフンなし）
			</small></p>
			<strong>有効期限</strong>
				<p><asp:DropDownList id="ddlCreditExpireMonth" runat="server" DataSource="<%# this.CreditExpireMonth %>" SelectedValue='<%# GetCreditValue(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment, CartPayment.FIELD_CREDIT_EXPIRE_MONTH) %>' CssClass="input_widthA input_border"></asp:DropDownList>&nbsp;&nbsp;
			&nbsp;/&nbsp;
				<asp:DropDownList id="ddlCreditExpireYear" runat="server" DataSource="<%# this.CreditExpireYear %>" SelectedValue='<%# GetCreditValue(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment, CartPayment.FIELD_CREDIT_EXPIRE_YEAR) %>' CssClass="input_border"></asp:DropDownList>&nbsp;&nbsp;(月/年)</p>
			<strong>カード名義人</strong>&nbsp;<span class="fred">※</span>&nbsp;例：「TAROU YAMADA」<span id='<%# "efo_sign_credit_author_name" + GetParentRepeaterItem(Container, "rCartList").ItemIndex %>' ></span>
				<p><asp:TextBox id="tbCreditAuthorName" runat="server" MaxLength="50" Text='<%# GetCreditValue(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment, CartPayment.FIELD_CREDIT_AUTHOR_NAME) %>' class="input_widthB input_border" autocomplete="off" Type="email" title=""></asp:TextBox><br />
			<small class="fred">
			<asp:CustomValidator
				ID="cvCreditAuthorName"
				runat="Server"
				ControlToValidate="tbCreditAuthorName"
				ValidationGroup="OrderShipping-OrderPayment"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				CssClass="error_inline" />
			</small></p>
			<div id="Div9" visible="<%# OrderCommon.CreditSecurityCodeEnable %>" runat="server">
			<strong>セキュリティコード</strong>&nbsp;<span class="fred">※</span><span id='<%# "efo_sign_credit_sec_code" + GetParentRepeaterItem(Container, "rCartList").ItemIndex %>' ></span>
				<p><asp:TextBox id="tbCreditSecurityCode" runat="server" MaxLength="4" Text='<%# GetCreditValue(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment, CartPayment.FIELD_CREDIT_SECURITY_CODE) %>' class="input_widthA input_border" autocomplete="off" Type="tel"></asp:TextBox><br />
			<small class="fred">
			<asp:CustomValidator
				ID="cvCreditSecurityCode"
				runat="Server"
				ControlToValidate="tbCreditSecurityCode"
				ValidationGroup="OrderShipping-OrderPayment"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				CssClass="error_inline" />
			</small></p>
			</div>
			</div>
			<% } %>
			<%--▲▲ カード情報入力（トークン未取得・利用なし） ▲▲--%>

			<%--▼▼ カード情報入力（トークン取得済） ▼▼--%>
			<%if (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Rakuten) { %>
			<div id="divCreditCardForTokenAcquired" Visible='<%# HasCreditToken(Container) %>' runat="server">
			<%if (OrderCommon.CreditCompanySelectable) {%>
			<strong>カード会社</strong>
				<p><asp:Literal ID="lCreditCardCompanyNameForTokenAcquired" Text='<%# ((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment.CreditCardCompanyName %>' runat="server"></asp:Literal><br /></p>
			<%} %>
			<strong>カード番号</strong>
			<asp:LinkButton id="lbEditCreditCardNoForToken" OnClick="lbEditCreditCardNoForToken_Click" runat="server">再入力</asp:LinkButton>
				<p>XXXXXXXXXXXX<asp:Literal ID="lLastFourDigitForTokenAcquired" Text='<%# ((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment.CreditCardNo4 %>' runat="server"></asp:Literal><br /></p>
			<strong>有効期限</strong>
				<p><asp:Literal ID="lExpirationMonthForTokenAcquired" Text='<%# ((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment.CreditExpireMonth %>' runat="server"></asp:Literal>
			&nbsp;/&nbsp;
				<asp:Literal ID="lExpirationYearForTokenAcquired" Text='<%# ((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment.CreditExpireYear %>' runat="server"></asp:Literal> (月/年)</p>
			<strong>カード名義人</strong>
				<p><asp:Literal ID="lCreditAuthorNameForTokenAcquired" Text='<%# ((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment.CreditAuthorName %>' runat="server"></asp:Literal><br /></p>
			</div>
			<%--▲▲ カード情報入力（トークン取得済） ▲▲ --%>

			<div id="Div3" visible="<%# OrderCommon.CreditInstallmentsSelectable %>" runat="server">
			<strong>支払い回数</strong>&nbsp;&nbsp;<span class="fgray">※AMEX/DINERSは一括のみとなります。</span>
				<p><asp:DropDownList id="dllCreditInstallments" runat="server" DataSource="<%# this.CreditInstallmentsList %>" DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetCreditValue(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment, CartPayment.FIELD_CREDIT_INSTALLMENTS_CODE) %>' CssClass="input_border"></asp:DropDownList>
			</div>
			<% } %>
			<% } else { %>
				<div>注文完了後に遷移する外部サイトで<br />
					カード番号を入力してください。</div>
			<% } %>
			<asp:CheckBox ID="cbRegistCreditCard" runat="server" Checked='<%# ((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment.UserCreditCardRegistFlg || UserRegisterDefaultChecked %>' Visible="<%# OrderCommon.GetCreditCardRegistable(this.IsLoggedIn, this.CreditCardList.Count - 1) %>" Text="登録する" OnCheckedChanged="cbRegistCreditCard_OnCheckedChanged" AutoPostBack="true"/>
			<div id="divUserCreditCardName" Visible="<%# (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten) %>"  runat="server">
			<p>クレジットカードを保存する場合は、以下をご入力ください。</p>
			<strong>クレジットカード登録名&nbsp;<span class="fred">※</span></strong><span id='<%# "efo_sign_register_name" + GetParentRepeaterItem(Container, "rCartList").ItemIndex %>' ></span>
				<p><asp:TextBox ID="tbUserCreditCardName" Text='<%# (UserRegisterDefaultChecked) ? ReplaceTag("@@CreditCard.disp_name.text@@") : ((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment.UserCreditCardName %>' MaxLength="100" CssClass="input_widthD input_border" runat="server"></asp:TextBox><br />

			<% if (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Rakuten) {%>
			<small class="fred">
			<asp:CustomValidator
				ID="cvUserCreditCardName"
				runat="Server"
				ControlToValidate="tbUserCreditCardName"
				ValidationGroup="OrderShipping-OrderPayment"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				CssClass="error_inline" />
			</small></p>
			<% } %>
			</div>
			</div>
			<%-- △新規カード△ --%>
			<%-- ▽登録済みカード▽ --%>
				<div id="divCreditCardDisp" visible='<%# IsNewCreditCard(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment) == false %>' runat="server">
			<%if (OrderCommon.CreditCompanySelectable) {%>
			<strong>カード会社</strong>
			<p><asp:Literal ID="lCreditCardCompanyName" runat="server"></asp:Literal><br /></p>
			<%} %>
			<strong>カード番号</strong>
			<p>XXXXXXXXXXXX<asp:Literal ID="lLastFourDigit" runat="server"></asp:Literal><br /></p>
			<strong>有効期限</strong>
			<p><asp:Literal ID="lExpirationMonth" runat="server"></asp:Literal>&nbsp;/&nbsp;<asp:Literal ID="lExpirationYear" runat="server"></asp:Literal> (月/年)</p>
			<strong>カード名義人</strong>
			<p><asp:Literal ID="lCreditAuthorName" runat="server"></asp:Literal><br /></p>
			<asp:HiddenField ID="hfCreditCardId" runat="server" />
			<div id="Div10" visible="<%# OrderCommon.CreditInstallmentsSelectable %>" runat="server">
			<strong>支払い回数</strong>&nbsp;&nbsp;<span class="fgray">※AMEX/DINERSは一括のみとなります。</span>
				<p><asp:DropDownList id="dllCreditInstallments2" runat="server" DataSource="<%# this.CreditInstallmentsList %>" DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetCreditValue(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment, CartPayment.FIELD_CREDIT_INSTALLMENTS_CODE) %>' CssClass="input_border"></asp:DropDownList>
			</div>
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
			<div id="Div5" visible="<%# (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.SBPS) %>" runat="server">
			<strong>支払いコンビニ選択</strong>
				<p><asp:DropDownList ID="ddlSBPSCvsType" DataSource='<%# this.CvsTypeList %>' DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetSBPSConveniType(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment) %>' CssClass="input_widthC input_border" runat="server"></asp:DropDownList></p>
			</div>
			<%-- コンビニ(前払い)：ヤマトKWC --%>
			<div id="Div11" visible="<%# (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.YamatoKwc) %>" runat="server">
			<strong>支払いコンビニ選択</strong>
				<p><asp:DropDownList ID="ddlYamatoKwcCvsType" DataSource='<%# this.CvsTypeList %>' DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetYamatoKwcConveniType(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment) %>' CssClass="input_widthC input_border" runat="server"></asp:DropDownList></p>
			</div>
			<%-- コンビニ(前払い)：GMO --%>
			<div id="Div12" visible="<%# (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Gmo) %>" runat="server">
			<strong>支払コンビニ選択</strong>
			<p><asp:DropDownList ID="ddlGmoCvsType" DataSource='<%# this.CvsTypeList %>' DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetGmoConveniType(((CartObject)GetParentRepeaterItem(Container, "rCartList").DataItem).Payment) %>' CssClass="input_widthC input_border" runat="server"></asp:DropDownList></p>
			</div>
			<%-- コンビニ(前払い)：Rakuten --%>
			<div visible="<%# (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Rakuten) %>" runat="server">
				<strong>支払コンビニ選択</strong>
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
			<div visible="<%# OrderCommon.IsPaymentCvsTypeZeus %>" runat="server">
				<strong>支払コンビニ選択</strong>
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
			<div visible="<%# OrderCommon.IsPaymentCvsTypePaygent %>" runat="server">
				<strong>支払コンビニ選択</strong>
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

			<%-- ドコモケータイ払い --%>
			<dd id="ddDocomoPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_ORG) %>" runat="server">
			<strong>【注意事項】</strong>
			<ul>
			<li>決済には「i-mode対応」の携帯電話が必要です。</li>
			<li>携帯電話のメールのドメイン指定受信を設定されている方は、必ず「xxxxx.co.jp」を受信できるように設定してください。</li>
			<li>１回の購入金額が<%: CurrencyManager.ToPrice(10000m) %>を超えてしまう場合はケータイ払いサービスをご利用いただけません。</li>
			<li>「i-mode」はＮＴＴドコモの商権、または登録商標です。</li>
			</ul></dd>

			<%-- 代金引換 --%>
			<dd id="ddCollect" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT) %>" runat="server">
			</dd>

			<%-- コンビニ(後払い) --%>
			<dd id="ddCvsDef" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF) %>" runat="server">
				<uc:PaymentDescriptionCvsDef runat="server" id="ucPaymentDescriptionCvsDef" />
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
		
			<%-- LinePay --%>
			<dd id="ddLinePay" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY) %>" runat="server">
				<uc:PaymentDescriptionLinePay runat="server" id="PaymentDescriptionLinePay" />
			</dd>
		
			<!-- PayPay -->
			<dd id="ddPayPay" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY) %>" runat="server">
				<uc:PaymentDescriptionPayPay runat="server" id="PaymentDescriptionPayPay" />
			</dd>
			
			<%-- GMOアトカラ --%>
			<dd id="ddGmoAtokara" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA) %>" runat="server">
				GMOアトカラ
			</dd>
		</div>
		</ItemTemplate>
		</asp:Repeater>
		</dl>

		<div>
		<small id="sErrorMessage2" class="fred" EnableViewState="False" runat="server"></small>
		</div>

		</div><!--list-->
		</div><!--bottom-->
		</div><!--orderBox-->
		</div><!--column-->
		</ContentTemplate>
		</asp:UpdatePanel>
		<%-- UPDATE PANELここまで(お支払い情報) --%>
		<% } %>
		<%-- ▲お支払い情報▲ --%>
		</div>
		<% if (this.IsAmazonLoggedIn) { %>
		<div class="columnRight">
			<h2 style="padding-top:18px;padding-bottom:18px;"><img src="<%: Constants.PATH_ROOT %>Contents/ImagesPkg/order/sttl_cash.gif" alt="お支払い情報" width="96" height="16" /></h2>
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
				<div id="walletWidgetDiv" style="width:340px;height:300px;"></div>
				<%-- ▲▲Amazon決済ウィジェット▲▲ --%>
				<% if ((this.CartList.Items.Count > 0) && this.CartList.Items[0].HasFixedPurchase) { %>
				<div style="margin: 10px 0;">下記のお支払い契約に同意してください。</div>
				<%-- ▼▼Amazon支払契約同意ウィジェット▼▼ --%>
				<div id="consentWidgetDiv" style="width:340px;height:120px;margin-top: 0.5em;"></div>
				<%-- ▲▲Amazon支払契約同意ウィジェット▲▲ --%>
				<div id="consentWidgetErrorMessage" style="color:red;padding:5px" ClientIDMode="Static" runat="server"></div>
				<% } %>
			<% } %>
		</div>
		<% } %>
		<%-- ▼領収書情報▼ --%>
		<asp:UpdatePanel ID="upReceiptInfo" runat="server">
		<ContentTemplate>
		<% if (Constants.RECEIPT_OPTION_ENABLED) { %>
		<div class="columnRight" id="divReceipt">
		<div class="orderBox">
			<h3>領収書情報</h3>
			<div class="bottom">
			<div id="divDisplayCanNotInputMessage" runat="server" visible="false" class="userList fred">指定したお支払い方法は、領収書の発行ができません。</div>
			<div id="divReceiptInfoInputForm" runat="server" class="userList">
				<strong>領収書希望有無を選択してください。</strong>
				<dd><asp:DropDownList id="ddlReceiptFlg" runat="server" DataTextField="text" DataValueField="value" DataSource="<%# this.ReceiptFlgListItems %>"
					SelectedValue="<%# ((CartObject)((RepeaterItem)Container).DataItem).ReceiptFlg %>"
					OnSelectedIndexChanged="ddlReceiptFlg_OnSelectedIndexChanged" AutoPostBack="true" CssClass="input_border" />
				</dd>
				<div id="divReceiptAddressProviso" runat="server">
				<dt>宛名<span class="fred">※</span></dt>
				<dd>
					<asp:TextBox id="tbReceiptAddress" runat="server" Text="<%# ((CartObject)((RepeaterItem)Container).DataItem).ReceiptAddress %>" MaxLength="100" CssClass="input_widthD" />
					<asp:CustomValidator ID="cvReceiptAddress" runat="Server"
						ControlToValidate="tbReceiptAddress"
						ValidationGroup="ReceiptRegisterModify"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						ClientValidationFunction="ClientValidate"/>
				</dd>
				<dt>但し書き<span class="fred">※</span></dt>
				<dd class="last">
					<asp:TextBox id="tbReceiptProviso" runat="server" Text="<%# ((CartObject)((RepeaterItem)Container).DataItem).ReceiptProviso %>" MaxLength="100" CssClass="input_widthD" />
					<asp:CustomValidator ID="cvReceiptProviso" runat="Server"
						ControlToValidate="tbReceiptProviso"
						ValidationGroup="ReceiptRegisterModify"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						ClientValidationFunction="ClientValidate"/>
				</dd>
				</div><!--divReceiptAddressProviso-->
			</div><!--divReceiptInfoInputForm-->
			</div><!--bottom-->
		</div><!--orderBox-->
		</div><!--divReceipt-->
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
				<div class="columnRight">
					<uc:BodyFixedPurchaseOrderPrice
						Cart="<%# (CartObject)((RepeaterItem)Container).DataItem %>"
						runat="server" />
				</div>
			</ContentTemplate>
		</asp:UpdatePanel>
		<!-- 定期注文購入金額 -->
		</div><!--subMain-->
		</div>
		<%-- ▼▼AmazonリファレンスID格納▼▼ --%>
		<asp:HiddenField runat="server" ID="hfAmazonOrderRefID"/>
		<%-- ▲▲AmazonリファレンスID格納▲▲ --%>
		<%-- ▼▼Amazon支払契約ID格納▼▼ --%>
		<asp:HiddenField runat="server" ID="hfAmazonBillingAgreementId"/>
		<%-- ▲▲Amazon支払契約ID格納▲▲ --%>
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
	
	<% if (this.IsDispCorrespondenceSpecifiedCommericalTransactionsSkipOrderConfirm) { %>
	<div style="text-align:left;padding:10px 0; width: 780px; margin:0 auto" id="hgcCompleteMessage" runat="server">
		以下の内容をご確認のうえ、「注文を確定する」ボタンをクリックしてください。
		<br/><%= ShopMessage.GetMessageByPaymentId() %> 
	</div>
	<% } %>
	<br class="clr" />
	<div class="btmbtn below">
	<ul>
		<asp:UpdatePanel ID="UpdatePanel1" runat="server">
		<ContentTemplate>
		<% if (this.IsCartListLp) { %>
		<li>
			<a href="<%: Constants.PATH_ROOT %>" class="btn btn-large btn-gry">買い物を続ける</a>
		</li>
		<% } %>
		<% if (this.DisplayNextBtn) { %>
		<li>
			<a onclick="<%: this.NextOnClick %>" href="<%: this.NextEvent %>" class="btn btn-large btn-success">
			<%: (((Constants.LANDING_CART_USER_REGISTER_WHEN_ORDER_COMPLETE == false) || this.SkipOrderConfirm) && (this.WcbUserRegister.Checked && (this.IsLoggedIn == false)
				&& (this.WcbUserRegisterForExternalSettlement.Checked && (this.IsUserRegistedForAmazon == false) && (this.IsAmazonLoggedIn == false) && (this.ExistsUserWithSameAmazonEmailAddress == false))))
				? this.SkipOrderConfirm ? "会員登録して注文を確定する" : "会員登録してご注文内容の確認へ"
				: IsTargetToExtendedAmazonAddressManagerOption() && this.WcbUserRegisterForExternalSettlement.Checked
					? "会員登録してご注文内容の確認へ"
					: this.SkipOrderConfirm ? "注文を確定する" : "ご注文内容の確認へ" %>
			</a>
			<a id="lbNextToConfirm" href="<%: this.NextEvent %>" style="display:none;"></a>
		</li>
		<% } %>
		</ContentTemplate>
		</asp:UpdatePanel>
	</ul>
	</div>
</div>
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

	<% if(Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED) { %>
	function HandleVisibility() {
		var cartCount = <%= this.CartList.Items.Count %>;
		for (var i = 0; i < cartCount ; i++) {
			var elements = document.getElementsByClassName(i)[0];
			if(typeof elements != 'undefined')
			{
				if((elements.querySelector('[id$="hfCvsShopId"]') != null)
					&& (elements.querySelector('[id$="hfCvsShopId"]').value == ''))
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

	var selectedCartIndex = 0;

	<%-- Open convenience store map popup --%>
	function openConvenienceStoreMapPopup(cartIndex) {
		selectedCartIndex = cartIndex;

		var url = '<%= OrderCommon.CreateConvenienceStoreMapUrl() %>';
		window.open(url, "", "width=1000,height=800");
	}

	<%-- Set convenience store data --%>
	function setConvenienceStoreData(cvsspot, name, addr, tel) {
		var elements = document.getElementsByClassName(selectedCartIndex)[0];

		// For display
		elements.querySelector('[id$="ddCvsShopId"] > span').innerHTML = cvsspot;
		elements.querySelector('[id$="ddCvsShopName"] > span').innerHTML = name;
		elements.querySelector('[id$="ddCvsShopAddress"] > span').innerHTML = addr;
		elements.querySelector('[id$="ddCvsShopTel"] > span').innerHTML = tel;

		// For get value
		elements.querySelector('[id$="hfCvsShopId"]').value = cvsspot;
		elements.querySelector('[id$="hfCvsShopName"]').value = name;
		elements.querySelector('[id$="hfCvsShopAddress"]').value = addr;
		elements.querySelector('[id$="hfCvsShopTel"]').value = tel;

		var element = document.getElementById('spErrorConvenienceStore' + selectedCartIndex);
		element.innerHTML = '';
		HandleVisibility();
	}

	<%-- Check Before Next Page --%>
	function CheckBeforeNextPage() {
		var hasError = false;
		<% foreach (RepeaterItem ri in rCartList.Items) { %>
		var shippingKbn = $('#<%= ((DropDownList)ri.FindControl("ddlShippingKbnList")).ClientID %>').val();
		if (shippingKbn == '<%= CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE %>') {
			var shopId = $('#<%= ((HiddenField)ri.FindControl("hfCvsShopId")).ClientID %>').val();

			var element = document.getElementById('spErrorConvenienceStore' + '<%= ri.ItemIndex %>');
			if (shopId == '') {
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
	<% } %>

	<%-- イベントをバインドする --%>
	<% var serializer = new JavaScriptSerializer(); %>
	function bindEvent() {
		bindExecAutoKana();
		bindExecAutoChangeKana();
		bindZipCodeSearch();
		bindTwAddressSearch();
		<% if(Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED) { %>
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
		<% if(Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED) { %>
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
		clickSearchZipCodeInRepeater(
			'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZip").ClientID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZip1").ClientID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbOwnerZip2").ClientID %>',
				'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchOwnergAddr").ClientID %>',
				'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchOwnergAddr").UniqueID %>',
				'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON %>',
				'<%= '#' + (ri.FindControl("sOwnerZipError")).ClientID %>',
				"owner");

		// Check owner zip code input on  text box change
		textboxChangeSearchZipCodeInRepeater(
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
			clickSearchZipCodeInRepeater(
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbShippingZip").ClientID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbShippingZip1").ClientID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbShippingZip2").ClientID %>',
				'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchShippingAddr").ClientID %>',
				'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchShippingAddr").UniqueID %>',
				'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON %>',
				'<%= '#' + (ri.FindControl("sShippingZipError")).ClientID %>',
				"shipping");

		// Check shipping zip code input on text box change
			textboxChangeSearchZipCodeInRepeater(
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
			<% if (((DropDownList)ri.FindControl("ddlShippingAddr3") != null) && ((TextBox)ri.FindControl("tbShippingZipGlobal") != null)) { %>
		$('#<%= ((DropDownList)ri.FindControl("ddlShippingAddr3")).ClientID %>').change(function (e) {
			$('#<%= ((TextBox)ri.FindControl("tbShippingZipGlobal")).ClientID %>').val(
					$('#<%= ((DropDownList)ri.FindControl("ddlShippingAddr3")).ClientID %>').val());
		});
		<%} %>
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
			design: { designMode: 'responsive' },
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
			<% } %>
			onReady: function (arg) {
				<% if ((this.CartList.Items.Count > 0) && this.CartList.Items[0].HasFixedPurchase){ %>
				var billingAgreementId = arg.getAmazonBillingAgreementId();
				$('#<%=this.WhfAmazonBillingAgreementId.ClientID %>').val(billingAgreementId);
				<% } else { %>
				var orderReferenceId = arg.getAmazonOrderReferenceId();
				$('#<%=this.WhfAmazonOrderRefID.ClientID %>').val(orderReferenceId);
				<% } %>
			},
			onAddressSelect: function (orderReference) {
				var $ownerAddressBookErrorMessage = $('#<%# this.WhgcOwnerAddressBookErrorMessage.ClientID %>');
				$ownerAddressBookErrorMessage.empty();
				getAmazonAddress('<%=((this.CartList.Items.Count > 0) && this.CartList.Items[0].HasFixedPurchase) ? w2.App.Common.Amazon.AmazonConstants.OrderType.AutoPay : w2.App.Common.Amazon.AmazonConstants.OrderType.OneTime %>', '<%= w2.App.Common.Amazon.AmazonConstants.AddressType.Owner %>', function (response) {
					var data = JSON.parse(response.d);
					if (($("#<%= WcbShipToOwnerAddress.ClientID %>").prop('checked')) && (typeof data.RequestPostBack !== "undefined")) location.href = $("#<%= (lbPostBack != null) ? lbPostBack.ClientID : "" %>").attr('href');
					if (data.Error) $ownerAddressBookErrorMessage.html(data.Error);
				});
			},
			design: { designMode: 'responsive' },
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
			design: { designMode: 'responsive' },
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
			design: { designMode: 'responsive' },
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
		webMethodUrl = "<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_LOGIN %>";
	});
</script>
<uc:UserLoginScript runat="server" ID="ucUserLoginScript" />
<%--▲▲ O-MOTION用スクリプト ▲▲--%>

<script>
	var subscriptionBoxMessage = '「@@ 1 @@」は「@@ 2 @@」の必須商品です。\n削除すると、「@@ 2 @@」の申し込みがキャンセルされます。\nよろしいですか？';

	// 必須商品チェック(頒布会)
	function delete_product_check_for_subscriptionBox(element) {
		var productDiv = findParentWithClass(element, 'product');
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
