<%--
=========================================================================================================
  Module      : ランディングカート確認画面(LandingCartConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Register TagPrefix="uc" TagName="BodyRecommend" Src="~/SmartPhone/Form/Common/BodyRecommend.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/LandingOrderPage.master" AutoEventWireup="true" CodeFile="~/Landing/LandingCartConfirm.aspx.cs" Inherits="Landing_LandingCartConfirm" Title="注文確認画面" %>
<%@ Register TagPrefix="uc" TagName="PaypalScriptsForm" Src="~/Form/Common/PayPalScriptsForm.ascx" %>
<%@ Register TagPrefix="uc" TagName="AffiliateTag" Src="~/Form/Common/AffiliateTag.ascx" %>
<%@ Register TagPrefix="uc" TagName="AtonePaymentScript" Src="~/Form/Common/AtonePaymentScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="AfteePaymentScript" Src="~/Form/Common/AfteePaymentScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyFixedPurchaseOrderPrice" Src="~/SmartPhone/Form/Common/BodyFixedPurchaseOrderPrice.ascx" %>
<%@ Register TagPrefix="uc" TagName="Loading" Src="~/SmartPhone/Form/Common/Loading.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaidyCheckoutScript" Src="~/Form/Common/Order/PaidyCheckoutScriptForPaygent.ascx" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.Paygent.Paidy.Checkout" %>

<asp:Content ContentPlaceHolderID="AffiliateTagHead" Runat="Server">
	<uc:AffiliateTag ID="AffiliateTagHead"
					Location="head"
					Datas="<%# this.CartList %>"
					runat="server"/>
</asp:Content>
<asp:Content ContentPlaceHolderID="AffiliateTagBodyTop" Runat="Server">
	<uc:AffiliateTag ID="AffiliateTagBodyTop"
					Location="body_top"
					Datas="<%# this.CartList %>"
					runat="server"/>
</asp:Content>
<asp:Content ContentPlaceHolderID="AffiliateTagBodyBottom" Runat="Server">
	<uc:AffiliateTag ID="AffiliateTagBodyBottom"
					Location="body_bottom"
					Datas="<%# this.CartList %>"
					runat="server"/>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<link href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "SmartPhone/Css/order.css") %>" rel="stylesheet" type="text/css" media="all" />
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<%-- △編集可能領域△ --%>
<div id="divTopArea">
<%-- ▽レイアウト領域：トップエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<%
	// 注文ボタン押下した際のJavascript処理追加
	if (this.WrCartList.Items.Count >= 1)
	{
		lbComplete1.OnClientClick
			= ((LinkButton)this.WrCartList.Items[this.WrCartList.Items.Count - 1].FindControl("lbComplete2")).OnClientClick
			= (this.HideOrderButtonWithClick) ? "return exec_submit(true)" : "return exec_submit(false)";

		if ((this.CartList.Items[this.WrCartList.Items.Count - 1]).Payment.IsPaymentPaygentPaidy)
		{
			lbComplete1.OnClientClick
				= ((LinkButton)this.WrCartList.Items[this.WrCartList.Items.Count - 1].FindControl("lbComplete2")).OnClientClick
				= "PaidyPayProcess(); return false;";
		}
	}
%>
<%-- 注文ボタン押下した際の処理 --%>
<script type="text/javascript">
<!--
	var blSubmitted = false;
	var isLastItemCart = false;
	var isPageConfirm = false;
	var isMyPage = null;
	var completeButton = null;
	var paymentNeedSubmitted = false;

	function exec_submit(blClearSubmitButton)
	{
		completeButton = document.getElementById('<%= lbCompleteAfterComfirmPayment.ClientID %>');

		<% if(Constants.PAYMENT_ATONEOPTION_ENABLED && this.IsUseAtonePaymentAndNotYetCardTranId) { %>
		GetAtoneAuthority();
		<% } %>
		<% if (Constants.PAYMENT_AFTEEOPTION_ENABLED && this.IsUseAfteePaymentAndNotYetCardTranId) { %>
		GetAfteeAuthority();
		<% } %>
		if (blSubmitted) return false;

		<% if (Constants.PRODUCT_ORDER_LIMIT_ENABLED && this.HasOrderHistorySimilarShipping) { %>
		var confirmMessage = '<%: WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_NOT_FIXED_PRODUCT_ORDER_LIMIT) %>' + "\nよろしいですか？";
		if (confirm(confirmMessage) == false) return false;
		<% } %>

		blSubmitted = true;

		return true;
	}

	window.onload = function () {
		<% if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED) { %>
		var isUnselected = <%= IsUnselectedProductOption() ? "true" : "false" %>;
		if (isUnselected) {
			alert('<%= WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_OPTION_UNSELECTED) %>');
			__doPostBack('ctl00$ContentPlaceHolder1$rCartList$ctl00$lbBack', '');
		}
		<% } %>
//-->
</script>

<asp:HiddenField ID="hfPaidyPaymentId" runat="server" />
<asp:HiddenField ID="hfPaidySelect" runat="server" />
<asp:HiddenField ID="hfPaidyStatus" runat="server" />

<% if (Constants.PAYMENT_AFTEEOPTION_ENABLED && this.IsUseAfteePaymentAndNotYetCardTranId) { %>
<asp:HiddenField runat="server" ID="hfAfteeToken" />
	<script type="text/javascript">
		$('#<%= this.WhfAfteeToken.ClientID %>').val('<%= this.IsLoggedIn
			? this.LoginUser.UserExtend.UserExtendDataValue[Constants.FLG_USEREXTEND_USREX_AFTEE_TOKEN_ID]
			: string.Empty %>');

		// Set token
		function SetAfteeTokenFromChildPage(token) {
			$('#<%= this.WhfAfteeToken.ClientID %>').val(token);
		}

		// Get Current Token
		function GetCurrentAfteeToken() {
			return $('#<%= this.WhfAfteeToken.ClientID %>').val();
		}

		// Get Cart Index
		function GetIndexCartHavingPaymentAtoneOrAftee(isAtone, callBack) {
			$.ajax({
				type: "POST",
				url: "<%= string.Format("{0}{1}",
					Constants.PATH_ROOT,
					string.Format("{0}{1}", this.IsSmartPhone
						? "SmartPhone/"
						: string.Empty, Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM)) %>/GetIndexCartHavingPaymentAtoneOrAftee", // Must bind from code behind to get current url
				contentType: "application/json; charset=utf-8",
				dataType: "json",
				data: JSON.stringify({ isAtone: isAtone }),
				async: false,
				success: callBack
			});
		}

		// Aftee Authority
		function GetAfteeAuthority() {
			GetIndexCartHavingPaymentAtoneOrAftee(false, function (response) {
				var data = JSON.parse(response.d);
				if (data.indexs.length > 0) {
					for (var index = 0; index < data.indexs.length; index++) {
						AfteeAuthories(data.indexs[index]);
						isLastItemCart = (index == (data.indexs.length - 1));
						break;
					}
					blSubmitted = true;
				}
				else {
					blSubmitted = false;
				}
			});
		}

		// Execute Order
		function ExecuteOrder() {
			var buttonComplete = document.getElementById('<%= lbComplete1.ClientID %>');
			buttonComplete.click();
		}
	</script>
	<% ucAfteePaymentScript.CurrentUrl = string.Format("{0}{1}",
					Constants.PATH_ROOT,
					string.Format("{0}{1}", this.IsSmartPhone
						? "SmartPhone/"
						: string.Empty, Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM)); %>
	<uc:AfteePaymentScript ID="ucAfteePaymentScript" runat="server"/>
<% } %>

<% if(Constants.PAYMENT_ATONEOPTION_ENABLED && this.IsUseAtonePaymentAndNotYetCardTranId) { %>
<asp:HiddenField runat="server" ID="hfAtoneToken" />
	<script type="text/javascript">
		$('#<%= this.WhfAtoneToken.ClientID %>').val('<%= this.IsLoggedIn
			? this.LoginUser.UserExtend.UserExtendDataValue[Constants.FLG_USEREXTEND_USREX_ATONE_TOKEN_ID]
			: string.Empty %>');

		// Set token
		function SetAtoneTokenFromChildPage(token) {
			$('#<%= this.WhfAtoneToken.ClientID %>').val(token);
		}

		// Get Current Token
		function GetCurrentAtoneToken() {
			return $('#<%= this.WhfAtoneToken.ClientID %>').val();
		}

		// Get Cart Index
		function GetIndexCartHavingPaymentAtoneOrAftee(isAtone, callBack) {
			$.ajax({
				type: "POST",
				url: "<%= string.Format("{0}{1}",
					Constants.PATH_ROOT,
					string.Format("{0}{1}", this.IsSmartPhone
						? "SmartPhone/"
						: string.Empty, Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM)) %>/GetIndexCartHavingPaymentAtoneOrAftee", // Must bind from code behind to get current url
				contentType: "application/json; charset=utf-8",
				dataType: "json",
				data: JSON.stringify({ isAtone: isAtone }),
				async: false,
				success: callBack
			});
		}

		// Atone Authority
		function GetAtoneAuthority() {
			GetIndexCartHavingPaymentAtoneOrAftee(true, function (response) {
				var data = JSON.parse(response.d);

				if (data.indexs.length > 0) {

					for (var index = 0; index < data.indexs.length; index++) {
						AtoneAuthories(data.indexs[index]);
						isLastItemCart = (index == (data.indexs.length - 1));
						break;
					}
					blSubmitted = true;
				}
				else {
					blSubmitted = false;
				}
			});
		}

		// Execute Order
		function ExecuteOrder() {
			var buttonComplete = document.getElementById('<%= lbComplete1.ClientID %>');
			buttonComplete.click();
		}
	</script>
	<% ucAtonePaymentScript.CurrentUrl = string.Format("{0}{1}",
					Constants.PATH_ROOT,
					string.Format("{0}{1}", this.IsSmartPhone
						? "SmartPhone/"
						: string.Empty, Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM)); %>
	<uc:AtonePaymentScript ID="ucAtonePaymentScript" runat="server"/>
<% } %>

<% if (this.IsCartListLp) { %>
<div class="step"><img src="<%= Constants.PATH_ROOT %>SmartPhone/Contents/ImagePkg/common/cart-lp-step02.jpg" alt="ご注文内容の確認" width="320" /></div>
<% } %>
<asp:UpdatePanel ID="upUpdatePanel" runat="server">
<ContentTemplate>
<div style="display: none">
<asp:LinkButton id="lbComplete1" runat="server" onclick="lbComplete_Click">注文する</asp:LinkButton>
<asp:LinkButton ID="lbBack1" OnClick="lbBack_Click" runat="server">戻る</asp:LinkButton>
<span style="display:none;">
	<asp:LinkButton ID="lbCompleteAfterComfirmPayment" runat="server" onclick="lbComplete_Click"></asp:LinkButton>
</span>
</div>

<section class="wrap-order landing-cart-comfirm order-comfirm">
<div class="order-unit">

<h2>ご注文内容の確認</h2>
<div style="color: red; font-weight: bold; margin : 10px">
	<asp:Label id="lblNotFirstTimeFixedPurchaseAlert" runat="server" visible="false"></asp:Label>
</div>

<%-- ▼PayPalログインここから▼ --%>
<%if (Constants.PAYPAL_LOGINPAYMENT_ENABLED) {%>
<%if (SessionManager.IsPayPalOrderfailed) {%>
	<%
		ucPaypalScriptsForm.LogoDesign = "Payment";
		ucPaypalScriptsForm.AuthCompleteActionControl = lbPayPalAuthComplete;
	%>
	<uc:PaypalScriptsForm ID="ucPaypalScriptsForm" runat="server" />
	<div id="paypal-button" style="height: 30px"></div>
	<%if (SessionManager.PayPalCooperationInfo != null) {%>
		<%: (SessionManager.PayPalCooperationInfo != null) ? SessionManager.PayPalCooperationInfo.AccountEMail : "" %> 連携済<br/>
	<%} %>
	<br /><asp:LinkButton ID="lbPayPalAuthComplete" runat="server" OnClick="lbPayPalAuthComplete_Click"></asp:LinkButton>
	<% SessionManager.IsPayPalOrderfailed = false; %>
<%} %>
<%} %>
<%-- ▲PayPalログインここまで▲ --%>
<div style="color: red; font-weight: bold">
	<asp:Label id="lblPaypayErrorMessage" runat="server" visible="false" />
</div>
<asp:Repeater id="rCartList" OnItemCommand="rCartList_OnItemCommand" Runat="server">
<ItemTemplate>
	<div class="cart-unit">
	<h3>カート番号<%# Container.ItemIndex + 1 %></h3>
		<%-- カート内商品を表示する --%>
		<%-- 通常商品 --%>
		<asp:Repeater id="rCart" runat="server" DataSource='<%# (CartObject)Container.DataItem %>' OnItemDataBound="rCart_OnItemDataBound">
		<HeaderTemplate>
			<table class="cart-table">
			<tbody>
		</HeaderTemplate>
		<ItemTemplate>
			<tr class="cart-unit-product" visible="<%# ((CartProduct)Container.DataItem).IsSetItem == false && ((CartProduct)Container.DataItem).QuantitiyUnallocatedToSet != 0 %>" runat="server">
				<td class="product-image">
					<% if (this.IsCartListLp) { %>
						<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
						<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" /></a>
					<% } else { %>
						<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" />
					<% } %>
				</td>
				<td class="product-info">
					<ul>
						<li class="product-name">
							<% if (this.IsCartListLp) { %>
								<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
								<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %></a>
							<% } else { %>
								<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %>
							<% } %>
							<%# (((CartProduct)Container.DataItem).GetProductTag("tag_cart_product_message").Length != 0) ? "<p class=\"product-msg\">" + WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).GetProductTag("tag_cart_product_message")) + "</p>" : "" %>
						</li>
						<li visible='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList.IsSelectedProductOptionValueAll %>' runat="server">
							<asp:Repeater ID="rProductOptionSettings" DataSource='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList %>' runat="server">
								<ItemTemplate>
									<%# WebSanitizer.HtmlEncode(((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue()) %>
									<%# (((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue() != "") ? "<br />" : "" %>
								</ItemTemplate>
							</asp:Repeater>
						</li>
						<p Visible="<%# ((CartProduct)Container.DataItem).IsDisplaySell && ((CartProduct.IsDisplaySubscriptionBoxSelectTime(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)) == false) %>" runat="server">販売期間：</p>
						<p Visible="<%# ((CartProduct)Container.DataItem).IsDisplaySell && ((CartProduct.IsDisplaySubscriptionBoxSelectTime(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)) == false) %>" class="spSpecifiedcommercialtransactionsPaddingleft10px" runat="server">
							<%#: DateTimeUtility.ToStringFromRegion(((CartProduct)Container.DataItem).SellFrom, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime) %>～<br />
							<%#: DateTimeUtility.ToStringFromRegion(((CartProduct)Container.DataItem).SellTo, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime) %>
						</p>
						<%--頒布会の選択可能期間--%>
						<div Visible="<%# (CartProduct.IsDisplaySubscriptionBoxSelectTime(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)) %>" runat="server" >販売期間</div>
						<div Visible="<%# (CartProduct.IsDisplaySubscriptionBoxSelectTime(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)) %>" class="spSpecifiedcommercialtransactionsPaddingleft10px" runat="server" >
							<%# WebSanitizer.HtmlEncodeChangeToBr((CartProduct.GetSubscriptionBoxSelectTermBr(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)))%>
						</div>
						<li class="product-price" Visible="<%# ((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false %>" runat="server">
							<p runat="server" ID="pPrice" style="text-decoration: line-through" Visible="False"><span runat="server" ID="sPrice"><%#: CurrencyManager.ToPrice(((CartProduct)Container.DataItem).Price) %></span> (<%#: this.ProductPriceTextPrefix %>)</p>
							<p runat="server" ID="pSubscriptionBoxCampaignPrice" style="padding-top: 2px"><span runat="server" ID="sSubscriptionBoxCampaignPrice"><%#: ProductOptionSettingHelper.ToDisplayProductOptionPrice((CartProduct)Container.DataItem) %></span> (<%#: this.ProductPriceTextPrefix %>)</p>
							<p ID="pSubscriptionBoxCampaignPeriod" Visible="False" style="color: red; padding-top: 2px" runat="server">キャンペーン期間：<br/>
								<span ID="sSubscriptionBoxCampaignPeriodSince" class="spSpecifiedcommercialtransactionsPaddingleft10px" runat="server"></span>～<br/>
								<span ID="sSubscriptionBoxCampaignPeriodUntil" class="spSpecifiedcommercialtransactionsPaddingleft10px" runat="server"></span>
							</p>
						</li>
						<li Visible="<%# ((CartProduct)Container.DataItem).IsDispSaleTerm %>" runat="server">
							タイムセール期間:<br/>
						</li>
						<li Visible="<%# ((CartProduct)Container.DataItem).IsDispSaleTerm %>" class="spSpecifiedcommercialtransactionsPaddingleft10px" runat="server">
							<%# WebSanitizer.HtmlEncodeChangeToBr(ProductCommon.GetProductSaleTermBr(this.ShopId, ((CartProduct)Container.DataItem).ProductSaleId)) %>
						</li>
						<li><%# WebSanitizer.HtmlEncodeChangeToBr(((CartProduct)Container.DataItem).ReturnExchangeMessage) %></li>
						<li style="color:red;" visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
							※配送料無料適用外商品です
						</li>
					</ul>
				</td>

				<td class="product-control">
					<div class="amout">
					<%--
					<span>
						<w2c:ExtendedTextBox ID="tbProductCount" Type="tel" Runat="server" Text='<%# ((CartProduct)Container.DataItem).QuantitiyUnallocatedToSet %>' MaxLength="3"></w2c:ExtendedTextBox>
					</span>
					--%>
					数量：<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).QuantitiyUnallocatedToSet) %>
					</div>
				</td>
			</tr>
		</ItemTemplate>
		<FooterTemplate>
			</tbody>
			</table>
		</FooterTemplate>
		</asp:Repeater>

		<%-- セットプロモーション --%>
		<asp:Repeater ID="rCartSetPromotion" DataSource="<%# ((CartObject)Container.DataItem).SetPromotions %>" runat="server">
		<ItemTemplate>

		<div class="cart-set-promotion-unit">

		<asp:HiddenField ID="hfCartSetPromotionNo" runat="server" Value="<%# ((CartSetPromotion)Container.DataItem).CartSetPromotionNo %>" />

		<asp:Repeater ID="rCartSetPromotionItem" DataSource="<%# ((CartSetPromotion)Container.DataItem).Items %>" runat="server">
		<HeaderTemplate>
			<table class="cart-table">
			<tbody>
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
				<td class="product-image">
					<% if (this.IsCartListLp) { %>
						<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
						<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" /></a>
					<% } else { %>
						<w2c:ProductImage IProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" />
					<% } %>
				</td>
				<td class="product-info">
					<ul>
						<li class="product-name">
							<% if (this.IsCartListLp) { %>
								<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
								<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %></a>
							<% } else { %>
								<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %>
							<% } %>
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
						<li><%# WebSanitizer.HtmlEncodeChangeToBr(((CartProduct)Container.DataItem).ReturnExchangeMessage) %></li>
						<li style="color:red;" visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
							※配送料無料適用外商品です
						</li>
					</ul>
				</td>
				<td class="product-control">
					<div class="amout">
					<%--
						<span>
							<w2c:ExtendedTextBox ID="tbSetPromotionItemCount" Type="tel" Runat="server" Text='<%# ((CartProduct)Container.DataItem).QuantityAllocatedToSet[((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).CartSetPromotionNo] %>' MaxLength="3"></w2c:ExtendedTextBox>
						</span>
					--%>
					数量：<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).QuantityAllocatedToSet[((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).CartSetPromotionNo]) %>
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
				<dt><%# WebSanitizer.HtmlEncode(((CartSetPromotion)Container.DataItem).SetpromotionDispName)%></dt>
				<dt>
					<span class="line-through" visible="<%# ((CartSetPromotion)Container.DataItem).IsDiscountTypeProductDiscount %>" runat="server">
					<%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).UndiscountedProductSubtotal) %> (<%#: this.ProductPriceTextPrefix %>)
					</span>
					<br>
					<%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).UndiscountedProductSubtotal - ((CartSetPromotion)Container.DataItem).ProductDiscountAmount) %> (<%#: this.ProductPriceTextPrefix %>)
				</dt>
				<dt Visible="<%# (((CartSetPromotion)Container.DataItem).Items[0].IsDisplaySell) %>" runat="server">
					販売期間：
				</dt>
				<dt Visible="<%# (((CartSetPromotion)Container.DataItem).Items[0].IsDisplaySell) %>" class="spSpecifiedcommercialtransactionsPaddingleft10px" runat="server">
					<%#: DateTimeUtility.ToStringFromRegion(((CartSetPromotion)Container.DataItem).Items[0].SellFrom, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime) %>～<br/>
					<%#: DateTimeUtility.ToStringFromRegion(((CartSetPromotion)Container.DataItem).Items[0].SellTo, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime) %>
				</dt>
				<dt Visible="<%# ((CartSetPromotion)Container.DataItem).Items[0].IsDispSaleTerm %>" runat="server">
					タイムセール期間:
				</dt>
				<dt Visible="<%# ((CartSetPromotion)Container.DataItem).Items[0].IsDispSaleTerm %>" class="spSpecifiedcommercialtransactionsPaddingleft10px" runat="server">
					<%# WebSanitizer.HtmlEncodeChangeToBr(ProductCommon.GetProductSaleTermBr(this.ShopId, ((CartSetPromotion)Container.DataItem).Items[0].ProductSaleId)) %>
				</dt>
				<dt Visible="<%# ((CartSetPromotion)Container.DataItem).IsDispSetPromotionTerm %>" runat="server">
					<br>セットプロモーション期間：
				</dt>
				<dt Visible="<%# ((CartSetPromotion)Container.DataItem).IsDispSetPromotionTerm %>" class="spSpecifiedcommercialtransactionsPaddingleft10px" runat="server">
					<%# WebSanitizer.HtmlEncodeChangeToBr(ProductCommon.GetSetPromotionTermBr(((CartSetPromotion)Container.DataItem).SetPromotionId)) %>
				</dt>
			</dl>
		</div>

		</div>
		</ItemTemplate>
		</asp:Repeater>

		<div class="cart-unit-footer">

			<%-- 小計 --%>
			<dl>
				<dt>小計（<%#: this.ProductPriceTextPrefix %>）</dt>
				<dd><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceSubtotal) %></dd>
			</dl>
			<%if (this.ProductIncludedTaxFlg == false) { %>
				<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
					<dt>消費税額</dt>
					<dd><%# CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceSubtotalTax) %></dd>
				</dl>
			<%} %>
			<%-- セットプロモーション --%>
			<asp:Repeater DataSource="<%# ((CartObject)Container.DataItem).SetPromotions %>" runat="server">
			<HeaderTemplate>
			</HeaderTemplate>
			<ItemTemplate>
			<dl visible="<%# ((CartSetPromotion)Container.DataItem).IsDiscountTypeProductDiscount %>" runat="server">
				<dt>
					<%# WebSanitizer.HtmlEncode(((CartSetPromotion)Container.DataItem).SetpromotionDispName)%>
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
					<%# (((CartObject)Container.DataItem).MemberRankDiscount > 0) ? "-" : ""%><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).MemberRankDiscount * ((((CartObject)Container.DataItem).MemberRankDiscount < 0) ? -1 : 1)) %>
				</dd>
			</dl>
			<%} %>
			<%if (Constants.FIXEDPURCHASE_OPTION_ENABLED){ %>
			<div runat="server" visible="<%# (((CartObject)Container.DataItem).HasFixedPurchase) %>">
			<dl>
				<dt>定期購入割引額</dt>
				<dd>
					<%#: (((CartObject)Container.DataItem).FixedPurchaseDiscount > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).FixedPurchaseDiscount * ((((CartObject)Container.DataItem).FixedPurchaseDiscount < 0) ? -1 : 1)) %>
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
						<%#: GetCouponName(((CartObject)Container.DataItem)) %>
						<%# (((CartObject)Container.DataItem).UseCouponPrice > 0) ? "-" : "" %>
						<%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).UseCouponPrice * ((((CartObject)Container.DataItem).UseCouponPrice < 0) ? -1 : 1)) %>
					</dd>
				</dl>
			<%} %>
			<%if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsLoggedIn){ %>
			<dl>
				<dt>ポイント利用額</dt>
				<dd>
					<%# (((CartObject)Container.DataItem).UsePointPrice > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).UsePointPrice * ((((CartObject)Container.DataItem).UsePointPrice < 0) ? -1 : 1)) %>
				</dd>
			</dl>
			<%} %>

			<%-- 配送料金 --%>
			<dl runat="server" style='<%# (((CartObject)Container.DataItem).ShippingPriceSeparateEstimateFlg) ? "display:none;" : ""%>'>
				<dt>配送料金</dt>
				<dd>
					<%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceShipping) %>
				</dd>
				<small style="color:red;" visible="<%# ((CartObject)Container.DataItem).IsDisplayFreeShiipingFeeText %>" runat="server">
					※配送料無料適用外の商品が含まれるため、<%# CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceShipping) %>の配送料が請求されます
				</small>
			</dl>

			<dl runat="server" style='<%# (((CartObject)Container.DataItem).ShippingPriceSeparateEstimateFlg == false) ? "display:none;" : ""%>'>
				<dt>配送料金</dt>
				<dd>
					<%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).ShippingPriceSeparateEstimateMessage)%>
				</dd>
			</dl>
			<%-- セットプロモーション --%>
			<asp:Repeater DataSource="<%# ((CartObject)Container.DataItem).SetPromotions %>" runat="server">
			<HeaderTemplate>
			</HeaderTemplate>
			<ItemTemplate>
			<dl visible="<%# ((CartSetPromotion)Container.DataItem).IsDiscountTypeShippingChargeFree  %>" runat="server">
			<dt>
				<%# WebSanitizer.HtmlEncode(((CartSetPromotion)Container.DataItem).SetpromotionDispName)%>(送料割引)
			</dt>
			<dd>
				<%# (((CartSetPromotion)Container.DataItem).ShippingChargeDiscountAmount > 0) ? "-" : ""%><%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).ShippingChargeDiscountAmount) %>
			</dd>
			</dl>
			</ItemTemplate>
			<FooterTemplate>
			</FooterTemplate>
			</asp:Repeater>
			<dl>
				<dt>決済手数料</dt>
				<dd>
					<%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).Payment.PriceExchange) %>
				</dd>
			</dl>

			<%-- セットプロモーション(決済手数料割引) --%>
			<asp:Repeater DataSource="<%# ((CartObject)Container.DataItem).SetPromotions %>" runat="server">
			<HeaderTemplate>
			</HeaderTemplate>
			<ItemTemplate>
			<dl visible="<%# ((CartSetPromotion)Container.DataItem).IsDiscountTypePaymentChargeFree %>" runat="server">
				<dt>
					<%# WebSanitizer.HtmlEncode(((CartSetPromotion)Container.DataItem).SetpromotionDispName) %>(決済手数料割引)
				</dt>
				<dd>
					<%# (((CartSetPromotion)Container.DataItem).PaymentChargeDiscountAmount > 0) ? "-" : ""%><%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).PaymentChargeDiscountAmount) %>
				</dd>
			</dl>
			</ItemTemplate>
			<FooterTemplate>
			</FooterTemplate>
			</asp:Repeater>

			<dl>
				<dt>合計(税込)</dt>
				<dd>
					<%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceTotal) %>
				</dd>
				<%if (Constants.GLOBAL_OPTION_ENABLE) { %>
				<dt>決済金額(税込)</dt>
				<dd><%#: GetSettlementAmount(((CartObject)Container.DataItem)) %></dd>
				<small style="color: red"><%#: string.Format(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_AMOUNT_VARIES_WITH_RATE), ((CartObject)Container.DataItem).SettlementCurrency) %></small>
				<% } %>
			</dl>
			<div class="InternationalShippingAttention" runat="server" visible="<%# IsDisplayProductTaxExcludedMessage((CartObject)Container.DataItem) %>">※国外配送をご希望の場合関税・商品消費税は料金に含まれず、商品到着後、現地にて税をお支払いいただくこととなりますのでご注意ください。</div>
		</div>

		<div class="button-change">
			<asp:LinkButton CommandName="GoBackLp" CommandArgument="<%# this.FocusingControlsOnCartList %>" runat="server" class="btn">変更する</asp:LinkButton>
		</div>
	</div>

	<div class="order-unit comfirm-payment">
		<dl class="order-form">
			<dt>お支払い方法</dt>
			<dd>		
				<%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Payment.PaymentName) %>
				<div visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.PaymentId) == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE %>' runat="server">
				支払先コンビニ名</div>
				<div visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.PaymentId) == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE %>' runat="server">
				<%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).GetPaymentCvsName()) %></div>
				<div visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.CreditCardCompany) != "" %>' runat="server">
				カード会社：<%# ((CartObject)Container.DataItem).Payment.CreditCardCompanyName %></div>
				<div visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.CreditCardNo) != "" %>' runat="server">
				カード番号：　XXXXXXXXXXXX<%# WebSanitizer.HtmlEncode(GetCreditCardDispString(((CartObject)Container.DataItem).Payment)) %></div>
				<div visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.CreditCardNo) != "" %>' runat="server">
				有効期限：<%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Payment.CreditExpireMonth) %>/<%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Payment.CreditExpireYear) %> (月/年)</div>
				<div visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.CreditCardNo) != "" %>' runat="server">
				支払い回数：<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_ORDER, OrderCommon.CreditInstallmentsValueTextFieldName, ((CartObject)Container.DataItem).Payment.CreditInstallmentsCode))%></div>
				<div visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.CreditCardNo) != "" %>' runat="server">
				カード名義：<%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Payment.CreditAuthorName) %></div>
				<div visible='<%# (StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.PaymentId) == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY) %>' runat="server">支払い方法</div>
				<div visible='<%# (StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.PaymentId) == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY) %>' runat="server">
					<%# ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE, ((CartObject)Container.DataItem).Payment.ExternalPaymentType) %>
				</div>
				<div visible='<%# (StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.PaymentId) == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY) %>' runat="server">支払い方法</div>
				<div visible='<%# (StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.PaymentId) == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY) %>' runat="server">
					<%# ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE + "_neweb", ((CartObject)Container.DataItem).Payment.ExternalPaymentType) %>
				</div>
			</dd>
		</dl>
		<%-- ▼領収書情報▼ --%>
		<% if (Constants.RECEIPT_OPTION_ENABLED) { %>
		<dl class="order-form">
			<dt>領収書情報</dt>
			<dd>
				<dl>
					<dt>領収書希望</dt>
					<dd><%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_RECEIPT_FLG, ((CartObject)Container.DataItem).ReceiptFlg) %></dd>
					<div runat="server" Visible="<%# ((CartObject)Container.DataItem).ReceiptFlg == Constants.FLG_ORDER_RECEIPT_FLG_ON %>">
						<dt>宛名</dt>
						<dd><%#: ((CartObject)Container.DataItem).ReceiptAddress %></dd>
						<dt>但し書き</dt>
						<dd><%#: ((CartObject)Container.DataItem).ReceiptProviso %></dd>
					</div>
				</dl>
			</dd>
		</dl>
		<% } %>
		<%-- ▲領収書情報▲ --%>
		<div class="button-change">
			<asp:LinkButton CommandName="GoBackLp" CommandArgument='<%# this.FocusingControlsOnOrderPayment %>' runat="server" class="btn">変更する</asp:LinkButton>
		</div>
	</div>

	<%-- ▼お届け先情報▼ --%>
	<div class="order-unit comfirm-shipping">
		<dl class="order-form">
			<dt>お届け先</dt>
			<dd>
				<div class="order-unit comfirm-shipping">
					<dl visible="<%# ((CartObject)Container.DataItem).GetShipping().IsShippingStorePickup %>" runat="server">
						<dt>受取店舗：</dt>
						<dd><%#: ((CartObject)Container.DataItem).GetShipping().RealShopName %></dd>
						<dt>受取店舗住所：</dt>
						<dd>
							<p>
								<%#: "〒" + ((CartObject)Container.DataItem).GetShipping().Zip %>
								<%#: ((CartObject)Container.DataItem).GetShipping().Addr1 %>
								<%#: ((CartObject)Container.DataItem).GetShipping().Addr2 %><br />
								<%#: ((CartObject)Container.DataItem).GetShipping().Addr3 %><br />
								<%#: ((CartObject)Container.DataItem).GetShipping().Addr4 %><br />
								<%#: ((CartObject)Container.DataItem).GetShipping().Addr5 %><br />
							</p>
						</dd>
						<dt>営業時間：</dt>
						<dd>
							<%#: ((CartObject)Container.DataItem).GetShipping().RealShopOpenningHours %>
						</dd>
						<dt>店舗電話番号：</dt>
						<dd><%#: ((CartObject)Container.DataItem).GetShipping().Tel1 %></dd>
					</dl>
				</div>
				<div visible="<%# ((CartObject)Container.DataItem).GetShipping().IsShippingStorePickup == false %>" runat="server">
					<div visible="<%# ((CartObject)Container.DataItem).IsDigitalContentsOnly == false %>" runat="server">
						<dl>
							<div runat="server" visible="<%# (((CartObject)Container.DataItem).GetShipping().ShippingAddrKbn != CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE) && (((CartObject)Container.DataItem).GetShipping().ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF) %>">
								<%-- 氏名 --%>
								<dt>
									<%: ReplaceTag("@@User.name.name@@") %>
								</dt>
								<dd>
									<%#: ((CartObject)Container.DataItem).GetShipping().Name1 %><%#: ((CartObject)Container.DataItem).GetShipping().Name2 %>&nbsp;様
								</dd>
								<div <%# (string.IsNullOrEmpty(((CartObject)Container.DataItem).GetShipping().NameKana) == false) ? string.Empty : "style=\"display:none;\"" %>>
									<%-- 氏名（かな） --%>
									<dt <%# (((CartObject)Container.DataItem).GetShipping().IsShippingAddrJp) ? string.Empty : "style=\"display:none;\"" %>>
										<%: ReplaceTag("@@User.name_kana.name@@") %>
									</dt>
									<dd <%# (((CartObject)Container.DataItem).GetShipping().IsShippingAddrJp) ? string.Empty : "style=\"display:none;\"" %>>
										<%#: ((CartObject)Container.DataItem).GetShipping().NameKana1 %><%#: ((CartObject)Container.DataItem).GetShipping().NameKana2 %>&nbsp;さま
									</dd>
								</div>
								<%-- 住所 --%>
								<dt>
									<%: ReplaceTag("@@User.addr.name@@") %>
								</dt>
								<dd>
									<%# ((CartObject)Container.DataItem).GetShipping().IsShippingAddrJp ? "〒" + WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).GetShipping().Zip) + "<br />" : string.Empty %>
									<%#: ((CartObject)Container.DataItem).GetShipping().Addr1 %>
									<%#: ((CartObject)Container.DataItem).GetShipping().Addr2 %>
									<%#: ((CartObject)Container.DataItem).GetShipping().Addr3 %><br />
									<%#: ((CartObject)Container.DataItem).GetShipping().Addr4 %><br />
									<%#: ((CartObject)Container.DataItem).GetShipping().Addr5 %><br />
									<%# (((CartObject)Container.DataItem).GetShipping().IsShippingAddrJp == false) ? WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).GetShipping().Zip) + "<br />" : string.Empty %>
									<%#: ((CartObject)Container.DataItem).GetShipping().ShippingCountryName %>
									<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
									<br />
									<%#: ((CartObject)Container.DataItem).GetShipping().CompanyName %><br />
									<%#: ((CartObject)Container.DataItem).GetShipping().CompanyPostName %>
									<% } %>
								</dd>
								<%-- 電話番号 --%>
								<dt><%: ReplaceTag("@@User.tel1.name@@") %></dt>
								<dd>
									<%#: ((CartObject)Container.DataItem).GetShipping().Tel1 %>
								</dd>
								<dt><%: ReplaceTag("@@User.tel2.name@@") %></dt>
								<dd>
									<%#: ((CartObject)Container.DataItem).Owner.Tel2 %>
								</dd>
								<%-- メルマガ登録 --%>
								<dt>
									<%: ReplaceTag("@@User.mail_flg.name@@") %>
								</dt>
								<dd>
									<%#: ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_FLG, ((CartObject)Container.DataItem).Owner.MailFlg ? Constants.FLG_USER_MAILFLG_OK : Constants.FLG_USER_MAILFLG_NG) %>
								</dd>
							</div>
							<div id="Div1" runat="server" visible="<%# (((CartObject)Container.DataItem).GetShipping().ShippingAddrKbn == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE) || (((CartObject)Container.DataItem).GetShipping().ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON) %>">
								<dt>店舗ID：</dt>
								<dd><%#: ((CartObject)Container.DataItem).GetShipping().ConvenienceStoreId %>&nbsp;</dd>
								<dt>店舗名称：</dt>
								<dd><%#: ((CartObject)Container.DataItem).GetShipping().Name1 %>&nbsp;</dd>
								<dt>店舗住所：</dt>
								<dd><%#: ((CartObject)Container.DataItem).GetShipping().Addr4 %>&nbsp;</dd>
								<dt>店舗電話番号：</dt>
								<dd><%#: ((CartObject)Container.DataItem).GetShipping().Tel1 %>&nbsp;</dd>
							</div>
							<% if (Constants.LANDING_CART_USER_REGISTER_WHEN_ORDER_COMPLETE) { %>
							<dt>会員登録</dt>
							<dd><%= (this.RegisterUser != null) ? "登録する" : "登録しない" %></dd>
							<% } %>
							<dt>配送方法：</dt>
							<dd>
								<%#: ValueText.GetValueText(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD, ((CartObject)Container.DataItem).Shippings[0].ShippingMethod) %>
							</dd>
							<dt visible="<%# CanDisplayDeliveryCompany(Container.ItemIndex) %>" runat="server">配送サービス</dt>
							<dd visible="<%# CanDisplayDeliveryCompany(Container.ItemIndex) %>" runat="server">
								<%#: GetDeliveryCompanyName(((CartObject)Container.DataItem).Shippings[0].DeliveryCompanyId) %>
							</dd>
							<%-- 配送日時 --%>
							<dt visible='<%# ((CartObject)Container.DataItem).GetShipping().SpecifyShippingDateFlg %>' runat="server">配送希望日</dt>
							<dd visible='<%# ((CartObject)Container.DataItem).GetShipping().SpecifyShippingDateFlg %>' runat="server">
								<%#: GetShippingDate(((CartObject)Container.DataItem).GetShipping()) %>
							</dd>
							<p>※定期解約されるまで継続して指定した配送パターンでお届けします。</p>
							<br />
							<dt visible='<%# ((CartObject)Container.DataItem).GetShipping().SpecifyShippingTimeFlg && ((CartObject)Container.DataItem).GetShipping().ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF %>' runat="server">配送希望時間帯</dt>
							<dd visible='<%# ((CartObject)Container.DataItem).GetShipping().SpecifyShippingTimeFlg && ((CartObject)Container.DataItem).GetShipping().ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF %>' runat="server">
								<%# WebSanitizer.HtmlEncode(GetShippingTime(((CartObject)Container.DataItem).GetShipping())) %>
							</dd>
							<%-- 注文メモ --%>
							<dt visible='<%# ((CartObject)Container.DataItem).GetOrderMemosForOrderConfirm().ToString().Trim() != string.Empty %>' runat="server">注文メモ</dt>
							<dd visible='<%# ((CartObject)Container.DataItem).GetOrderMemosForOrderConfirm().ToString().Trim() != string.Empty %>' runat="server">
								<%# WebSanitizer.HtmlEncodeChangeToBr(((CartObject)Container.DataItem).GetOrderMemosForOrderConfirm()) %>
							</dd>
							<asp:Repeater ID="rOrderExtendDisplay"
								ItemType="OrderExtendItemInput"
								DataSource="<%# GetDisplayOrderExtendItemInputs(GetCartIndexFromControl(Container)) %>"
								runat="server"
								Visible="<%# IsDisplayOrderExtend() %>">
								<HeaderTemplate>
									<dl class="order-form">
										<dt>注文確認事項</dt>
										<dd>
											<dl>
								</HeaderTemplate>
								<ItemTemplate>
									<dt><%#: Item.SettingModel.SettingName %>：</dt>
									<dd>
										<%#: (string.IsNullOrEmpty(Item.InputValue)) ? "指定なし" : Item.InputText %>
									</dd>
								</ItemTemplate>
							</asp:Repeater>
							<span runat="server" visible="<%# OrderCommon.DisplayTwInvoiceInfo(((CartObject)Container.DataItem).GetShipping().ShippingCountryIsoCode) %>">
								<dt>発票種類</dt>
								<dd>
									<%# ValueText.GetValueText(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE, ((CartObject)Container.DataItem).GetShipping().UniformInvoiceType).Replace("コード", string.Empty) %>
								</dd>
								<dt visible="<%# (((CartObject)Container.DataItem).GetShipping().UniformInvoiceType != Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL) %>" runat="server">
									<%# (((CartObject)Container.DataItem).GetShipping().UniformInvoiceType == Constants.FLG_TW_UNIFORM_INVOICE_COMPANY)
										? ReplaceTag("@@TwInvoice.uniform_invoice_company_code_option.name@@")
										: ReplaceTag("@@TwInvoice.uniform_invoice_donate_code_option.name@@") %> :
								</dt>
								<dd runat="server" visible="<%# (((CartObject)Container.DataItem).GetShipping().UniformInvoiceType != Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL) %>">
									<%# (((CartObject)Container.DataItem).GetShipping().UniformInvoiceOption1) %>
								</dd>
								<dt runat="server" visible="<%# (((CartObject)Container.DataItem).GetShipping().UniformInvoiceType == Constants.FLG_TW_UNIFORM_INVOICE_COMPANY) %>">
									<%# ReplaceTag("@@TwInvoice.uniform_invoice_company_name_option.name@@") %>：
								</dt>
								<dd runat="server" visible="<%# (((CartObject)Container.DataItem).GetShipping().UniformInvoiceType == Constants.FLG_TW_UNIFORM_INVOICE_COMPANY) %>">
									<%# (((CartObject)Container.DataItem).GetShipping().UniformInvoiceOption2) %>
								</dd>
								<span runat="server" visible="<%# (((CartObject)Container.DataItem).GetShipping().UniformInvoiceType == Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL) %>">
									<dt>共通性載具</dt>
									<dd>
										<%# string.IsNullOrEmpty(((CartObject)Container.DataItem).GetShipping().CarryType)
											? ValueText.GetValueText(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE, ((CartObject)Container.DataItem).GetShipping().CarryType)
											: ValueText.GetValueText(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE, ((CartObject)Container.DataItem).GetShipping().CarryType).Replace("コード", string.Empty) %>
									</dd>
									<dt visible="<%# string.IsNullOrEmpty(((CartObject)Container.DataItem).GetShipping().CarryType) == false %>" runat="server">
										載具コード：
									</dt>
									<dd visible="<%# string.IsNullOrEmpty(((CartObject)Container.DataItem).GetShipping().CarryType) == false %>" runat="server">
										<%# ((CartObject)Container.DataItem).GetShipping().CarryTypeOptionValue %>
									</dd>
								</span>
							</span>
							<dd><%#: (((CartObject)Container.DataItem).ReflectMemoToFixedPurchase) ? "※2回目以降の注文メモにも追加する" : string.Empty %></dd>
						</dl>
					</div>
				</div>
			</dd>
			<div class="button-change">
				<asp:LinkButton CommandName="GoBackLp" CommandArgument='<%# this.FocusingControlsOnOrderShipping %>' runat="server" class="btn">変更する</asp:LinkButton>
			</div>
		</dl>
	</div>

	<%-- ▼定期配送情報▼ --%>
	<div class="order-unit comfirm-fixed" visible="<%# ((CartObject)Container.DataItem).HasFixedPurchase %>" runat="server">
		<dl class="order-form">
			<dt>定期配送情報</dt>
			<dd>
				<dl>
					<dt>配送パターン</dt>
					<dd>
						<%#: ((CartObject)Container.DataItem).GetShipping().GetFixedPurchaseShippingPatternString() %>
					</dd>
					<dt>初回配送予定</dt>
					<dd>
						<%#: GetFirstShippingDate(((CartObject)Container.DataItem).GetShipping()) %>
					</dd>
					<dt>今後の配送予定</dt>
					<dd>
						<%#: DateTimeUtility.ToStringFromRegion(((CartObject)Container.DataItem).GetShipping().NextShippingDate, DateTimeUtility.FormatType.LongDateWeekOfDay1Letter) %>&nbsp;
						<%#: DateTimeUtility.ToStringFromRegion(((CartObject)Container.DataItem).GetShipping().NextNextShippingDate, DateTimeUtility.FormatType.LongDateWeekOfDay1Letter) %>
					</dd>
					<dt visible='<%# ((CartObject)Container.DataItem).GetShipping().SpecifyShippingTimeFlg && ((CartObject)Container.DataItem).GetShipping().ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF %>' runat="server">
						配送希望時間帯
					</dt>
					<dd visible='<%# ((CartObject)Container.DataItem).GetShipping().SpecifyShippingTimeFlg && ((CartObject)Container.DataItem).GetShipping().ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF %>' runat="server">
						<%#: GetShippingTime(((CartObject)Container.DataItem).GetShipping())%>
					</dd>
				</dl>
			</dd>
		</dl>
	</div>
	<%-- ▲定期配送情報▲ --%>

	<!-- レコメンド設定 -->
	<uc:BodyRecommend ID="BodyRecommend1" runat="server" Cart="<%# (CartObject)Container.DataItem %>" />

	<!-- 定期注文購入金額 -->
	<uc:BodyFixedPurchaseOrderPrice runat="server" Cart="<%# (CartObject)Container.DataItem %>" Visible="<%# ((CartObject)Container.DataItem).HasFixedPurchase %>" />

</div>

<div class="cart-footer" visible="<%# ((this.CartList.Items.Count > 1) ? ((this.CartList.Items.Count - Container.ItemIndex) == 1) : (Container.ItemIndex == 0)) %>" runat="server">
	<% if (this.IsDispCorrespondenceSpecifiedCommericalTransactions) { %>
		<dl>
			<dt style="background-color: #ccc;padding: 0.5em; line-height: 1.5;">特商法に基づく表記</dt>
			<dd style="padding: 0.5em"><%= ShopMessage.GetMessageByPaymentId(this.CartList.Items[0].Payment.PaymentId) %></dd>
		</dl>
	<% } %>
	<% if (SessionManager.IsChangedAmazonPayForFixedOrNormal) { %>
	<div class="button-next order">
		<span style="color: red;">
			カート内の商品が変更されました。<br/>
			お手数ですが再度Amazon Payでの購入手続きに進んでください。<br/><br/>
		</span>
		<div style="text-align:center">
			<div id="AmazonPayCv2Button" style="display: inline-block"></div>
		</div>
	</div>
	<% } else {%>
		<div class="button-next order">
		<asp:LinkButton id="lbComplete2" CssClass="btn" runat="server" onclick="lbComplete_Click">注文する</asp:LinkButton>
	</div>
	<div class="button-prev">
		<asp:LinkButton id="lbBack2" CssClass="btn" runat="server" onclick="lbBack_Click">戻る</asp:LinkButton>
	</div>
	<% } %>
</div>

</ItemTemplate>
</asp:Repeater>

</section>
</ContentTemplate>
</asp:UpdatePanel>
<%-- △編集可能領域△ --%>

<div id="divBottomArea">
<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>
<input type="hidden" id="fraudbuster" name="fraudbuster" />
<uc:Loading id="ucLoading" UpdatePanelReload="True" runat="server" />
<script type="text/javascript" src="//cdn.credit.gmo-ab.com/psdatacollector.js"></script>
<% if (Constants.AMAZON_PAYMENT_CV2_ENABLED) { %>
<script src="https://static-fe.payments-amazon.com/checkout.js"></script>
<script type="text/javascript" charset="utf-8">
	showAmazonPayCv2Button(
		'#AmazonPayCv2Button',
		'<%= Constants.PAYMENT_AMAZON_SELLERID %>',
		<%= Constants.PAYMENT_AMAZON_ISSANDBOX.ToString().ToLower() %>,
		'<%= this.AmazonRequest.Payload %>',
		'<%= this.AmazonRequest.Signature %>',
		'<%= Constants.PAYMENT_AMAZON_PUBLIC_KEY_ID %>')
</script>
<% } %>
<%--▼▼ Paidy用スクリプト ▼▼--%>
<script type="text/javascript">
	var body = <%= new PaidyCheckout(this.CartList.Items.FirstOrDefault(item => item.Payment.IsPaymentPaygentPaidy)).CreateParameterForPaidyCheckout() %>;
	var hfPaidyPaySelectedControlId = "<%= (this.WhfPaidySelect.ClientID) %>";
	var hfPaidyPaymentIdControlId = "<%= (this.WhfPaidyPaymentId.ClientID) %>";
	var hfPaidyStatusControlId = "<%= (this.WhfPaidyStatus.ClientID) %>";
	var isHistoryPage = false;
	var lbNextProcess = "<%= lbCompleteAfterComfirmPayment.ClientID %>";
</script>
<uc:PaidyCheckoutScript runat="server" />
</asp:Content>
