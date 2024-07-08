<%--
=========================================================================================================
  Module      : スマートフォン用注文確認画面(OrderConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Register TagPrefix="uc" TagName="BodyRecommend" Src="~/SmartPhone/Form/Common/BodyRecommend.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyFixedPurchaseOrderPrice" Src="~/SmartPhone/Form/Common/BodyFixedPurchaseOrderPrice.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/Order/OrderConfirm.aspx.cs" Inherits="Form_Order_OrderConfirm" Title="注文確認ページ" maintainScrollPositionOnPostback="true"%>
<%@ Import Namespace="System.Security.Policy" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionCvsDef" Src="~/Form/Common/Order/PaymentDescriptionCvsDef.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaypalScriptsForm" Src="~/Form/Common/PayPalScriptsForm.ascx" %>
<%@ Register TagPrefix="uc" TagName="AtonePaymentScript" Src="~/Form/Common/AtonePaymentScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="AfteePaymentScript" Src="~/Form/Common/AfteePaymentScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="Loading" Src="~/SmartPhone/Form/Common/Loading.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaidyCheckoutScript" Src="~/Form/Common/Order/PaidyCheckoutScriptForPaygent.ascx" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.Paygent.Paidy.Checkout" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<%-- △編集可能領域△ --%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="divTopArea">
<%-- ▽レイアウト領域：トップエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<%
	// 注文完了ボタン押下した際のJavascript処理追加
	this.CompleteButtonList.ForEach(button =>
	{
		button.OnClientClick = (this.HideOrderButtonWithClick) ? "return exec_submit(true)" : "return exec_submit(false)";

		if (this.CartList.Items.Any(item => item.Payment.IsPaymentPaygentPaidy)
			&& (this.CartList.Items.Count == 1))
		{
			button.OnClientClick = "PaidyPayProcess(); return false;";
		}
	});
%>
<%-- 注文ボタン押下した際の処理 --%>
<script type="text/javascript">
<!--
	var submitted = false;
	var isLastItemCart = false;
	var isPageConfirm = false;
	var isMyPage = null;
	var completeButton = null;
	var paymentNeedSubmitted = false;

	function exec_submit(clearSubmitButton)
	{
		completeButton = document.getElementById('<%= lbCompleteAfterComfirmPayment.ClientID %>');

		if (submitted === false) {
			var confirmMessage = '<%= WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_NOT_FIXED_PRODUCT_ORDER_LIMIT) %>' + "\nよろしいですか？";
			<% if (Constants.PRODUCT_ORDER_LIMIT_ENABLED && this.HasOrderHistorySimilarShipping) { %>
			if (confirm(confirmMessage) === false) return false;
			<% } %>
		}

		<% if(Constants.PAYMENT_ATONEOPTION_ENABLED && this.IsUseAtonePaymentAndNotYetCardTranId) { %>
		GetAtoneAuthority();
		<% } %>
		<% if (Constants.PAYMENT_AFTEEOPTION_ENABLED && this.IsUseAfteePaymentAndNotYetCardTranId) { %>
		GetAfteeAuthority();
		<% } %>
		if (submitted) return false;

		submitted = true;

		return true;
	}
//-->
</script>
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
					string.Format("{0}{1}", (this.IsSmartPhone
						? "SmartPhone/"
						: string.Empty), Constants.PAGE_FRONT_ORDER_CONFIRM)) %>/GetIndexCartHavingPaymentAtoneOrAftee", // Must bind from code behind to get current url
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
					submitted = true;
				}
				else {
					submitted = false;
				}
			});
		}

		// Execute Order
		function ExecuteOrder() {
			var buttonComplete = document.getElementById('<%= lbComplete2.ClientID %>');
			buttonComplete.click();
		}
	</script>
	<% ucAtonePaymentScript.CurrentUrl = string.Format("{0}{1}",
					Constants.PATH_ROOT,
					string.Format("{0}{1}", (this.IsSmartPhone
						? "SmartPhone/"
						: string.Empty), Constants.PAGE_FRONT_ORDER_CONFIRM)); %>
	<uc:AtonePaymentScript ID="ucAtonePaymentScript" runat="server"/>
<% } %>

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
					string.Format("{0}{1}", (this.IsSmartPhone
						? "SmartPhone/"
						: string.Empty), Constants.PAGE_FRONT_ORDER_CONFIRM)) %>/GetIndexCartHavingPaymentAtoneOrAftee", // Must bind from code behind to get current url
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
					submitted = true;
				}
				else {
					submitted = false;
				}
			});
		}

		// Execute Order
		function ExecuteOrder() {
			var buttonComplete = document.getElementById('<%= lbComplete2.ClientID %>');
			buttonComplete.click();
		}
	</script>
	<% ucAfteePaymentScript.CurrentUrl = string.Format("{0}{1}",
					Constants.PATH_ROOT,
					string.Format("{0}{1}", (this.IsSmartPhone
						? "SmartPhone/"
						: string.Empty), Constants.PAGE_FRONT_ORDER_CONFIRM)); %>
	<uc:AfteePaymentScript ID="ucAfteePaymentScript" runat="server"/>
<% } %>
<section class="wrap-order order-comfirm">

<div class="step">
	<img src="<%= Constants.PATH_ROOT %>SmartPhone/Contents/ImagePkg/common/cart-step04.jpg" alt="ご注文内容の確認" width="320" />
</div>
<div style="text-align: left">
	<asp:Label id="lblPaymentAlert" runat="server">
		同梱後の金額が各決済方法の上限額を超えました。<br />お手数ですが、カートに戻って別の注文と同梱、または同梱せずに注文実行してください。
	</asp:Label>
</div>
<div style="color: red; font-weight: bold">
	<asp:Label id="lblNotFirstTimeFixedPurchaseAlert" runat="server" visible="false"></asp:Label>
</div>
<div style="color: red; font-weight: bold">
	<asp:Label id="lblDeliveryPatternAlert" runat="server" visible="false">配送パターンを選択してください</asp:Label>
</div>
<div style="color: red; font-weight: bold">
	<asp:Label id="lblPaypayErrorMessage" runat="server" visible="false" />
</div>
<div style="color: red; font-weight: bold;">
	<asp:Literal id="lOrderCombineMessage" visible="false" runat="server" />
</div>
<div style="color: red; font-weight: bold;">
	<asp:Literal id="lSubscriptionOrderCombineMessage" visible="true" runat="server" />
</div>
<%-- ▼PayPalログインここから▼ --%>
<%if (Constants.PAYPAL_LOGINPAYMENT_ENABLED) {%>
<%if (SessionManager.IsPayPalOrderfailed) {%>
	<%
		ucPaypalScriptsForm.LogoDesign = "Payment";
		ucPaypalScriptsForm.AuthCompleteActionControl = lbPayPalAuthComplete;
	%>
	<uc:PaypalScriptsForm ID="ucPaypalScriptsForm" runat="server" />
	<div id="paypal-button" style="height: 25px"></div>
	<%if (SessionManager.PayPalCooperationInfo != null) {%>
		<%: (SessionManager.PayPalCooperationInfo != null) ? SessionManager.PayPalCooperationInfo.AccountEMail : "" %> 連携済<br/>
	<%} %>
	<br /><asp:LinkButton ID="lbPayPalAuthComplete" runat="server" OnClick="lbPayPalAuthComplete_Click"></asp:LinkButton>
	<% SessionManager.IsPayPalOrderfailed = false; %>
<%} %>
<%} %>
<%-- ▲PayPalログインここまで▲ --%>

<asp:Repeater id="rCartList" Runat="server" OnItemCommand="rCartList_ItemCommand">
<ItemTemplate>
	<div class="cart-unit">
	<h2><%# this.CartList.Items.Count > 1 ? "カート番号" + (Container.ItemIndex + 1).ToString() + "の" : "" %>注文内容</h2>
		<%-- カート内商品を表示する --%>
		<%-- 通常商品 --%>
		<asp:Repeater id="rCart" runat="server" DataSource='<%# (CartObject)Container.DataItem %>' OnItemCommand="rCartList_ItemCommand" OnItemDataBound="rCart_OnItemDataBound">
		<HeaderTemplate>
			<table class="cart-table">
			<tbody>
		</HeaderTemplate>
		<ItemTemplate>
			<tr class="cart-unit-product" visible="<%# ((CartProduct)Container.DataItem).IsSetItem == false && ((CartProduct)Container.DataItem).QuantitiyUnallocatedToSet != 0 %>" runat="server">
				<td class="product-image">
					<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
						<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" /></a>
					<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false %>" />
				</td>
				<td class="product-info">
					<ul>
						<li class="product-name">
							<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
								<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %></a>
							<%# (((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false) ? WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) : "" %>
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
						<li class="product-price" Visible="<%# DisplaySubscriptionBoxFixedAmountCourse(((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount()) %>" runat="server">
							<p style="padding-top: 2px;">
								頒布会コース名：<%#: ((CartProduct)Container.DataItem).GetSubscriptionDisplayName() %>&nbsp;定額：<%#: CurrencyManager.ToPrice(((CartProduct)Container.DataItem).SubscriptionBoxFixedAmount) %>(<%#: this.ProductPriceTextPrefix %>)
							</p>
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
							<p runat="server" ID="pPrice" style="padding-top: 2px; text-decoration: line-through" Visible="False"><span runat="server" ID="sPrice"><%#: ProductOptionSettingHelper.ToDisplayProductOptionPrice((CartProduct)Container.DataItem) %></span> (<%#: this.ProductPriceTextPrefix %>)</p>
							<p runat="server" ID="pSubscriptionBoxCampaignPrice" style="padding-top: 2px"><span runat="server" ID="sSubscriptionBoxCampaignPrice"><%#: ProductOptionSettingHelper.ToDisplayProductOptionPrice((CartProduct)Container.DataItem) %></span> (<%#: this.ProductPriceTextPrefix %>)</p>
							<p ID="pSubscriptionBoxCampaignPeriod" Visible="False" style="color: red; padding-top: 2px" runat="server">キャンペーン期間：<br />
								<span ID="sSubscriptionBoxCampaignPeriodSince" class="spSpecifiedcommercialtransactionsPaddingleft10px" runat="server"></span>～<br/>
								<span ID="sSubscriptionBoxCampaignPeriodUntil" class="spSpecifiedcommercialtransactionsPaddingleft10px" runat="server"></span>
							</p>
						</li>
						<li runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsDispSaleTerm %>">
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
					<p class="attention" visible="<%# this.ErrorMessages.HasMessages(((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex) %>" runat="server">
						<%# WebSanitizer.HtmlEncode(this.ErrorMessages.Get(((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex)) %>
					</p>
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
			<%-- セット商品 --%>
			<div class="cart-unit-product" visible="<%# (((CartProduct)Container.DataItem).IsSetItem) && (((CartProduct)Container.DataItem).ProductSetItemNo == 1) %>" runat="server">
				<asp:Repeater id="rProductSet" DataSource="<%# (((CartProduct)Container.DataItem).ProductSet != null) ? ((CartProduct)Container.DataItem).ProductSet.Items : null %>" runat="server">
				<ItemTemplate>
				<tr>
					<td class="product-image">
						<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
							<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" />
						</a>
						<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false %>" />
					</td>
					<td class="product-info">
						<ul>
							<li class="product-name">
								<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
									<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %>
								</a>
								<%# (((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false) ? WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) : "" %>
								<%# (((CartProduct)Container.DataItem).GetProductTag("tag_cart_product_message").Length != 0) ? "<p class=\"product-msg\">" + WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).GetProductTag("tag_cart_product_message")) + "</p>" : "" %>
							</li>
							<li class="product-price" Visible="<%# ((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false %>" runat="server">
								<p><%#: CurrencyManager.ToPrice(((CartProduct)Container.DataItem).Price) %> (<%#: this.ProductPriceTextPrefix %>)&nbsp;&nbsp;x&nbsp;&nbsp;<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).CountSingle) %></p></dd>
							</li>
							<li><%# WebSanitizer.HtmlEncodeChangeToBr(((CartProduct)Container.DataItem).ReturnExchangeMessage) %></li>
							<li style="color:red;" visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
								※配送料無料適用外商品です
							</li>
						</ul>
					</td>
					<td class="product-control" visible="<%# (((CartProduct)Container.DataItem).ProductSetItemNo == 1) %>" rowspan="<%# (((CartProduct)Container.DataItem).ProductSet != null) ? ((CartProduct)Container.DataItem).ProductSet.Items.Count : 1 %>" runat="server">
						<div class="amout">
							セット <%# GetProductSetCount((CartProduct)Container.DataItem) %><br />
							<p Visible="<%# ((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false %>" runat="server">
								<%#: CurrencyManager.ToPrice(GetProductSetPriceSubtotal((CartProduct)Container.DataItem)) %> (<%#: this.ProductPriceTextPrefix %>)
							</p>
						</div>
					</td>
				</tr>
				</ItemTemplate>
				</asp:Repeater>
			</div>
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

		<asp:Repeater ID="rCartSetPromotionItem" DataSource="<%# ((CartSetPromotion)Container.DataItem).Items %>" OnItemCommand="rCartList_ItemCommand" runat="server">
		<HeaderTemplate>
			<table class="cart-table">
			<tbody>
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
				<td class="product-image">
				<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
					<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" /></a>
				<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false %>" />
				</td>
				<td class="product-info">
					<ul>
						<li class="product-name">
							<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
								<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %></a>
							<%# (((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false) ? WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) : "" %>
							<div visible='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList.IsSelectedProductOptionValueAll %>' runat="server">
							<asp:Repeater ID="rProductOptionSettings" DataSource='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList %>' runat="server">
							<ItemTemplate>
								<%# WebSanitizer.HtmlEncode(((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue()) %>
								<%# (((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue() != "") ? "<br />" : "" %>
							</ItemTemplate>
							</asp:Repeater>
							</div>
						</li>
						<li class="product-price"  runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false %>">
							<p runat="server" ID="pPrice" style="padding-top: 2px; text-decoration: line-through" Visible="False">
								<span runat="server" ID="sPrice">
									<%#: ProductOptionSettingHelper.ToDisplayProductOptionPrice((CartProduct)Container.DataItem) %>
								</span>
							</p>
							<p runat="server" ID="pSubscriptionBoxCampaignPrice" style="padding-top: 2px"><span runat="server" ID="sSubscriptionBoxCampaignPrice"><%#: CurrencyManager.ToPrice(((CartProduct)Container.DataItem).Price) %></span> (<%#: this.ProductPriceTextPrefix %>)</p>
						</li>
						<li style="color:red;" visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
							※配送料無料適用外商品です
						</li>
					</ul>
					<p class="attention" visible="<%# this.ErrorMessages.HasMessages(((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex) %>" runat="server">
						<%# WebSanitizer.HtmlEncode(this.ErrorMessages.Get(((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex)) %>
					</p>
					<p class="attention" visible="<%# this.ErrorMessages.HasMessages(((RepeaterItem)Container.Parent.Parent.Parent.Parent).ItemIndex, ((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex) %>" runat="server">
						<%# WebSanitizer.HtmlEncode(this.ErrorMessages.Get(((RepeaterItem)Container.Parent.Parent.Parent.Parent).ItemIndex, ((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex))%>
					</p>
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
					<%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).UndiscountedProductSubtotal) %> (税込)
					</span>
					<br />
					<%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).UndiscountedProductSubtotal - ((CartSetPromotion)Container.DataItem).ProductDiscountAmount) %> (税込)
				</dt>
				<dt Visible="<%# (((CartSetPromotion)Container.DataItem).Items[0].IsDisplaySell) %>" runat="server">
					販売期間：
				</dt>
				<dt Visible="<%# (((CartSetPromotion)Container.DataItem).Items[0].IsDisplaySell) %>" class="spSpecifiedcommercialtransactionsPaddingleft10px" runat="server">
					<%#: DateTimeUtility.ToStringFromRegion(((CartSetPromotion)Container.DataItem).Items[0].SellFrom, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime) %>～<br/><%#: DateTimeUtility.ToStringFromRegion(((CartSetPromotion)Container.DataItem).Items[0].SellTo, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime) %>
				</dt>
				<dt  Visible="<%# ((CartSetPromotion)Container.DataItem).Items[0].IsDispSaleTerm %>" runat="server">
					タイムセール期間:
				</dt>
				<dt Visible="<%# ((CartSetPromotion)Container.DataItem).Items[0].IsDispSaleTerm %>" class="spSpecifiedcommercialtransactionsPaddingleft10px" runat="server">
					<%# WebSanitizer.HtmlEncodeChangeToBr(ProductCommon.GetProductSaleTermBr(this.ShopId, ((CartSetPromotion)Container.DataItem).Items[0].ProductSaleId)) %>
				</dt>
				<dt  Visible="<%# ((CartSetPromotion)Container.DataItem).IsDispSetPromotionTerm %>" runat="server">
					セットプロモーション期間：
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
				<dl>
					<dt>消費税額</dt>
					<dd>
						<%# CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceSubtotalTax) %>
					</dd>
				</dl>
			<%} %>
			<%-- セットプロモーション(商品割引) --%>
			<asp:Repeater DataSource="<%# ((CartObject)Container.DataItem).SetPromotions %>" Visible="<%# ((CartObject)Container.DataItem).IsAllItemsSubscriptionBoxFixedAmount == false %>" runat="server">
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

			<%if (Constants.MEMBER_RANK_OPTION_ENABLED && Constants.FIXEDPURCHASE_OPTION_ENABLED && this.IsLoggedIn) { %>
			<dl>
				<dt>定期会員割引額</dt>
				<dd>
					<%# (((CartObject)Container.DataItem).FixedPurchaseMemberDiscountAmount > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).FixedPurchaseMemberDiscountAmount * ((((CartObject)Container.DataItem).FixedPurchaseMemberDiscountAmount < 0) ? -1 : 1)) %>
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

			<%-- セットプロモーション(配送料割引) --%>
			<asp:Repeater DataSource="<%# ((CartObject)Container.DataItem).SetPromotions %>" runat="server">
			<HeaderTemplate>
			</HeaderTemplate>
			<ItemTemplate>
			<dl visible="<%# ((CartSetPromotion)Container.DataItem).IsDiscountTypeShippingChargeFree %>" runat="server">
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
			<dl visible="<%# (((CartObject)Container.DataItem).PriceRegulation != 0) %>" runat="server">
				<dt>調整金額</dt>
				<dd>
					<%#: (((CartObject)Container.DataItem).PriceRegulation < 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(Math.Abs(((CartObject)Container.DataItem).PriceRegulation)) %>
				</dd>
			</dl>
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
			 <%if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsLoggedIn) { %>
				<dl Visible="<%# ((CartObjectList)((Repeater)Container.Parent).DataSource).Items.Count == Container.ItemIndex + 1 %>" runat="server">
					<dt Visible="<%# ((CartObject)Container.DataItem).FirstBuyPoint != 0 %>" runat="server">
						初回購入獲得ポイント
					</dt>
					<dd Visible="<%# ((CartObject)Container.DataItem).FirstBuyPoint != 0 %>" runat="server">
						<%#: GetNumeric(((CartObjectList)((Repeater)Container.Parent).DataSource).TotalFirstBuyPoint) %>pt
					</dd>
					<dt>購入後獲得ポイント</dt>
					<dd><%#: GetNumeric(((CartObjectList)((Repeater)Container.Parent).DataSource).TotalBuyPoint) %>pt</dd>
					<small>※ 1pt = <%: CurrencyManager.ToPrice(1m) %></small>
				</dl>
			<%} %>
			<div class="InternationalShippingAttention" runat="server" visible="<%# IsDisplayProductTaxExcludedMessage((CartObject)Container.DataItem) %>">※国外配送をご希望の場合関税・商品消費税は料金に含まれず、商品到着後、現地にて税をお支払いいただくこととなりますのでご注意ください。</div>
		</div>

		<div class="button-change" id="hgcChangeCartInfoBtn" runat="server" >
			<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST) %>" class="btn">変更する</a>
		</div>
	</div>

	<div class="box" visible="<%# Container.ItemIndex == 0 %>" runat="server">
	
	<div class="order-unit comfirm-payment">
		<dl class="order-form">
			<dt>本人情報確認</dt>
			<dd>
				<dl>
				<%-- 氏名 --%>
				<dt>
					<%: ReplaceTag("@@User.name.name@@") %>：
				</dt>
				<dd><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Owner.Name1) %><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Owner.Name2) %>&nbsp;様</dd>
				<% if ((this.IsAmazonCv2Guest == false) || (Constants.AMAZON_PAYMENT_CV2_ENABLED && Constants.AMAZON_PAYMENT_CV2_USE_SHIPPING_AS_OWNER_ENABLED) || IsTargetToExtendedAmazonAddressManagerOption()) { %>
				<%-- 氏名（かな） --%>
				<div <%# ((string.IsNullOrEmpty(((CartObject)Container.DataItem).Owner.NameKana) == false)) ? "" : "style=\"display:none;\"" %>>
					<dt <%# (((CartObject)Container.DataItem).Owner.IsAddrJp) ? "" : "style=\"display:none;\"" %>>
						<%: ReplaceTag("@@User.name_kana.name@@") %>：
					</dt>
					<dd <%# (((CartObject)Container.DataItem).Owner.IsAddrJp) ? "" : "style=\"display:none;\"" %>><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Owner.NameKana1) %><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Owner.NameKana2) %>&nbsp;さま</dd>
				</div>
				<% } %>
				<%-- メールアドレス --%>
				<dt>
					<%: ReplaceTag("@@User.mail_addr.name@@") %>：
				</dt>
				<dd><%# ((((CartObject)Container.DataItem).Owner.MailAddr) != "") ? WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Owner.MailAddr) : "-&nbsp;" %></dd>
				<% if ((this.IsAmazonCv2Guest == false) || (Constants.AMAZON_PAYMENT_CV2_ENABLED && Constants.AMAZON_PAYMENT_CV2_USE_SHIPPING_AS_OWNER_ENABLED) || IsTargetToExtendedAmazonAddressManagerOption())
	   { %>
				<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
				<dt>
					<%: ReplaceTag("@@User.mail_addr2.name@@") %>：
				</dt>
				<dd><%# ((((CartObject)Container.DataItem).Owner.MailAddr2) != "") ? WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Owner.MailAddr2):"-&nbsp;" %></dd><br />
				<%} %>
				<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
				<dt><%: ReplaceTag("@@User.company_name.name@@")%>・
					<%: ReplaceTag("@@User.company_post_name.name@@")%>：</dt>
				<dd><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Owner.CompanyName) %><br />
					<%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Owner.CompanyPostName) %></dd>
				<%} %>
				<%-- 電話番号 --%>
				<dt><%: ReplaceTag("@@User.tel1.name@@") %>：</dt>
				<dd><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Owner.Tel1) %></dd>
				<dt><%: ReplaceTag("@@User.tel2.name@@") %>：</dt>
				<dd><%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Owner.Tel2) %>&nbsp;</dd>
				<dt><%: ReplaceTag("@@User.mail_flg.name@@") %>：</dt>
				<dd><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_FLG, ((CartObject)Container.DataItem).Owner.MailFlg ? Constants.FLG_USER_MAILFLG_OK : Constants.FLG_USER_MAILFLG_NG))%><br />&nbsp;</dd>
				<% } %>
				</dl>
			</dd>
			<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
		</dl>
		<div class="button-change" id="hgcChangeUserInfoBtn" runat="server">
		<asp:LinkButton CommandName="GotoShipping" runat="server"  CssClass="btn">変更する</asp:LinkButton>
		</div>
	</div>
	</div><!--box-->

	<div class="order-unit comfirm-payment">
		<dl class="order-form">
			<dt>お支払い方法</dt>
			<dd>
				<%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Payment.PaymentName) %>
				<div visible='<%# (StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.PaymentId) == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY) %>' runat="server">支払い方法：</div>
				<div visible='<%# (StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.PaymentId) == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY) %>' runat="server">
					<%# ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE, ((CartObject)Container.DataItem).Payment.ExternalPaymentType) %>
				</div>
				<div visible='<%# (StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.PaymentId) == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY) %>' runat="server">支払い方法：</div>
				<div visible='<%# (StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.PaymentId) == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY) %>' runat="server">
					<%# ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE + "_neweb", ((CartObject)Container.DataItem).Payment.ExternalPaymentType) %>
				</div>
				<div visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.PaymentId) == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE %>' runat="server">
				支払先コンビニ名</div>
				<div visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.PaymentId) == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE %>' runat="server">
				<%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).GetPaymentCvsName()) %></div>
				<div id="dvCvsDef" visible="<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.PaymentId) == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF %>" runat="server">
					<uc:PaymentDescriptionCvsDef runat="server" ID="ucPaymentDescriptionCvsDef"  />
				</div>
				<div visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.CreditCardCompany) != "" %>' runat="server">
				カード会社：<%#: ((CartObject)Container.DataItem).Payment.CreditCardCompanyName %></div>
				<div visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.CreditCardNo) != "" %>' runat="server">
				カード番号：　XXXXXXXXXXXX<%# WebSanitizer.HtmlEncode(GetCreditCardDispString(((CartObject)Container.DataItem).Payment)) %></div>
				<div visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.CreditCardNo) != "" %>' runat="server">
				有効期限：<%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Payment.CreditExpireMonth) %>/<%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Payment.CreditExpireYear) %> (月/年)</div>
				<div visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.CreditCardNo) != "" %>' runat="server">
				支払い回数：<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_ORDER, OrderCommon.CreditInstallmentsValueTextFieldName, ((CartObject)Container.DataItem).Payment.CreditInstallmentsCode))%></div>
				<div visible='<%# StringUtility.ToEmpty(((CartObject)Container.DataItem).Payment.CreditCardNo) != "" %>' runat="server">
				カード名義：<%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).Payment.CreditAuthorName) %></div>
			</dd>
		</dl>
		<% if (this.IsLoggedIn && Constants.TWOCLICK_OPTION_ENABLE && CheckPaymentCanSaveDefaultValue(this.CartList.Items[0].Payment.PaymentId)){ %>
		<td><asp:CheckBox id="cbDefaultPayment" GroupName="DefaultPaymentSetting" Text=" 通常の支払方法に設定する" CssClass="radioBtn" runat="server" OnCheckedChanged="cbDefaultPayment_OnCheckedChanged" AutoPostBack="true"/></td>
		<%} %>
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
		<div class="button-change" id="hgcChangePaymentInfoBtn" runat="server" >
			<asp:LinkButton CommandName="GotoPayment" runat="server" CssClass="btn">変更する</asp:LinkButton>
		</div>
	</div>

	<%-- ▼お届け先情報▼ --%>
	<asp:Repeater id="rCartShippings" DataSource='<%# Eval("Shippings") %>' OnItemCommand="rCartShippings_ItemCommand" runat="server">
	<ItemTemplate>
		<div visible="<%# (FindCart(Container.DataItem).IsDigitalContentsOnly == false) %>" runat="server">
		<div class="order-unit comfirm-payment">
			<dl class="order-form">
			<dt>お届け先<span visible="<%# FindCart(Container.DataItem).IsGift %>" runat="server"><%# Container.ItemIndex + 1 %></span></dt>
			<div>
			<dl>
			<dd>
			<dl>
				<div runat="server" visible="<%# (((CartShipping)Container.DataItem).ShippingAddrKbn != CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE)
					&& (((CartShipping)Container.DataItem).ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF) %>">
				<span runat="server" visible="<%# (((CartShipping)Container.DataItem).IsShippingStorePickup == false) %>">
				<%-- 氏名 --%>
				<dt><%: ReplaceTag("@@User.name.name@@") %>：</dt>
				<dd><%# WebSanitizer.HtmlEncode(Eval("Name1")) %><%# WebSanitizer.HtmlEncode(Eval("Name2")) %>&nbsp;様</dd>
				<%-- 氏名（かな） --%>
				<div <%# (string.IsNullOrEmpty((string)Eval("NameKana")) == false) ? "" : "style=\"display:none;\"" %>>
					<dt <%# ((bool)Eval("IsShippingAddrJp")) ? "" : "style=\"display:none;\"" %>><%: ReplaceTag("@@User.name_kana.name@@") %>：</dt>
					<dd <%# ((bool)Eval("IsShippingAddrJp")) ? "" : "style=\"display:none;\"" %>><%# WebSanitizer.HtmlEncode(Eval("NameKana1")) %><%# WebSanitizer.HtmlEncode(Eval("NameKana2")) %>&nbsp;さま</dd>
				</div>
				<dt>
					<%: ReplaceTag("@@User.addr.name@@") %>：
				</dt>
				<dd>
					<p>
						<%# ((bool)Eval("IsShippingAddrJp")) ? WebSanitizer.HtmlEncode("〒" + Eval("Zip")) + "<br />" : ""  %>
						<%#: Eval("Addr1") %> <%#: Eval("Addr2") %><br />
						<%#: Eval("Addr3") %> <%#: Eval("Addr4") %> <%#: Eval("Addr5") %><br />
						<%# ((bool)Eval("IsShippingAddrJp") == false) ? WebSanitizer.HtmlEncode(Eval("Zip")) + "<br />" : ""  %>
						<%#: Eval("ShippingCountryName") %>
					</p>
					<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
						<dt><%: ReplaceTag("@@User.company_name.name@@")%>・
							<%: ReplaceTag("@@User.company_post_name.name@@")%>：</dt>
						<dd>
							<%# WebSanitizer.HtmlEncode(Eval("CompanyName"))%>&nbsp<%# WebSanitizer.HtmlEncode(Eval("CompanyPostName"))%></dd>
					<%} %>
				</dd>
				<%-- 電話番号 --%>
				<dt><%: ReplaceTag("@@User.tel1.name@@") %>：</dt>
				<dd><%# WebSanitizer.HtmlEncode(Eval("Tel1")) %></dd>
				</span>
				</div>
				<div id="Div1" runat="server" visible="<%# (((CartShipping)Container.DataItem).ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON) %>">
					<dt>店舗ID：</dt>
					<dd><%#: ((CartShipping)Container.DataItem).ConvenienceStoreId %>&nbsp;</dd>
					<dt>店舗名称：</dt>
					<dd><%#: ((CartShipping)Container.DataItem).Name1 %>&nbsp;</dd>
					<dt>店舗住所：</dt>
					<dd><%#: ((CartShipping)Container.DataItem).Addr4 %>&nbsp;</dd>
					<dt>店舗電話番号：</dt>
					<dd><%#: ((CartShipping)Container.DataItem).Tel1 %>&nbsp;</dd>
				</div>
				<div visible="<%# IsStorePickupDisplayed(Container.DataItem, true) %>" runat="server">
					<dl>
						<dt>
							受取店舗：
						</dt>
						<dd>
							<p style="word-wrap: break-word;"><%#: Eval("RealShopName") %></p>
						</dd>
						<dt>
							受取店舗住所：
						</dt>
						<dd>
							<p>
								<%#: "〒" + Eval("Zip") %>
								<br />
								<%#: Eval("Addr1") %> <%#: Eval("Addr2") %>
								<br />
								<%#: Eval("Addr3") %>
								<br />
								<%#: Eval("Addr4") %>
								<br />
								<%#: Eval("Addr5") %>
							</p>
						</dd>
						<dt>
							営業時間：
						</dt>
						<dd>
							<p><%#: Eval("RealShopOpenningHours") %></p>
						</dd>
						<dt>
							店舗電話番号：
						</dt>
						<dd>
							<p><%#: Eval("Tel1") %></p>
						</dd>
					</dl>
				</div>
				<span visible="<%# IsStorePickupDisplayed(Container.DataItem, true) %>" runat="server">
					<dt>配送方法：</dt>
					<dd>店舗受取</dd>
				<dd visible="<%# IsStorePickupDisplayed(Container.DataItem) %>" runat="server">
					<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD, ((CartShipping)Container.DataItem).ShippingMethod)) %>
				</dd>
				<dt visible ="<%# CanDisplayDeliveryCompany(GetCartIndexFromControl(Container), Container.ItemIndex) %>" runat="server">配送サービス：</dt>
				<dd visible ="<%# CanDisplayDeliveryCompany(GetCartIndexFromControl(Container), Container.ItemIndex) %>" runat="server">
					<%#: GetDeliveryCompanyName(((CartShipping)Container.DataItem).DeliveryCompanyId) %>
				</dd>
				<dt visible='<%# Eval("SpecifyShippingDateFlg") %>' runat="server">
					配送希望日：</dt>
				<dd visible='<%# Eval("SpecifyShippingDateFlg") %>' runat="server"><%# WebSanitizer.HtmlEncode(GetShippingDate((CartShipping)Container.DataItem)) %></dd>
				<dt visible='<%# Eval("SpecifyShippingTimeFlg") %>' runat="server">
					配送希望時間帯：</dt>
				<dd visible='<%# Eval("SpecifyShippingTimeFlg") %>' runat="server"><%# WebSanitizer.HtmlEncode(GetShippingTime((CartShipping)Container.DataItem)) %></dd>
				</span>
				<%-- 注文メモ --%>
				<dt visible='<%# ((CartShipping)Container.DataItem).CartObject.GetOrderMemosForOrderConfirm().Trim() != ""  %>' runat="server">
				注文メモ：</dt>
				<dd visible='<%# ((CartShipping)Container.DataItem).CartObject.GetOrderMemosForOrderConfirm().Trim() != ""  %>' runat="server">
					<%# WebSanitizer.HtmlEncodeChangeToBr(((CartShipping)Container.DataItem).CartObject.GetOrderMemosForOrderConfirm()) %>
				</dd>
				<dd><%# WebSanitizer.HtmlEncode(((CartShipping)Container.DataItem).CartObject.ReflectMemoToFixedPurchase ? "※2回目以降の注文メモにも追加する" : string.Empty) %></dd>
				<asp:Repeater ID="rOrderExtendDisplay"
							ItemType="OrderExtendItemInput"
							DataSource="<%# GetDisplayOrderExtendItemInputs(GetCartIndexFromControl(Container)) %>"
							runat="server"
							Visible="<%# IsDisplayOrderExtend() %>" >
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
					<FooterTemplate>
								</dl>
							</dd>
						</dl>
					</FooterTemplate>
				</asp:Repeater>
			</dl>
			</dd>
			</dl>
			</div>
			<span runat="server" visible="<%# OrderCommon.DisplayTwInvoiceInfo(((CartShipping)Container.DataItem).ShippingCountryIsoCode) %>">
					<dt>発票種類</dt>
					<dd>
						<%# ValueText.GetValueText(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE, ((CartShipping)Container.DataItem).UniformInvoiceType).Replace("コード", string.Empty) %>
					</dd>
					<dt runat="server" visible="<%# (((CartShipping)Container.DataItem).UniformInvoiceType != Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL)%>">
						<%# (((CartShipping)Container.DataItem).UniformInvoiceType == Constants.FLG_TW_UNIFORM_INVOICE_COMPANY)
								? ReplaceTag("@@TwInvoice.uniform_invoice_company_code_option.name@@")
								: ReplaceTag("@@TwInvoice.uniform_invoice_donate_code_option.name@@") %>
					</dt>
					<dd runat="server" visible="<%# (((CartShipping)Container.DataItem).UniformInvoiceType != Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL)%>">
						<%# (((CartShipping)Container.DataItem).UniformInvoiceOption1) %>
					</dd>
					<dt runat="server" visible="<%# (((CartShipping)Container.DataItem).UniformInvoiceType == Constants.FLG_TW_UNIFORM_INVOICE_COMPANY)%>">
						<%# ReplaceTag("@@TwInvoice.uniform_invoice_company_name_option.name@@") %>
					</dt>
					<dd runat="server" visible="<%# (((CartShipping)Container.DataItem).UniformInvoiceType == Constants.FLG_TW_UNIFORM_INVOICE_COMPANY)%>">
						<%# (((CartShipping)Container.DataItem).UniformInvoiceOption2) %>
					</dd>
					<span runat="server" visible="<%# (((CartShipping)Container.DataItem).UniformInvoiceType == Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL) %>">
						<dt>共通性載具</dt>
						<dd>
							<%# string.IsNullOrEmpty(((CartShipping)Container.DataItem).CarryType)
								? ValueText.GetValueText(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE, ((CartShipping)Container.DataItem).CarryType)
								: ValueText.GetValueText(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE, ((CartShipping)Container.DataItem).CarryType).Replace("コード", string.Empty) %>
						</dd>
						<dt visible="<%# string.IsNullOrEmpty(((CartShipping)Container.DataItem).CarryType) == false %>" runat="server" >
							載具コード
						</dt>
						<dd visible="<%# string.IsNullOrEmpty(((CartShipping)Container.DataItem).CarryType) == false %>" runat="server">
							<%# ((CartShipping)Container.DataItem).CarryTypeOptionValue %>
						</dd>
					</span>
					<td>
						<asp:CheckBox id="cbDefaultInvoice"
						GroupName='<%# "DefaultInvoiceSetting_" + Container.ItemIndex %>'
						class="radioBtn DefaultInvoice"
						Text ="通常の電子発票情報に設定する"
						OnCheckedChanged="cbDefaultInvoice_CheckedChanged"
						AutoPostBack="true"
						runat="server"
						Visible="<%# this.IsLoggedIn %>" />
					</td>
				</span>
			</dl>
			</div>
		<% if (this.IsLoggedIn && Constants.TWOCLICK_OPTION_ENABLE && ((Constants.GIFTORDER_OPTION_ENABLED == false || Constants.GIFTORDER_OPTION_WITH_SHORTENING_GIFT_OPTION_ENABLED) && (this.IsAmazonLoggedIn == false))){%>
		<td><asp:CheckBox id="cbDefaultShipping" GroupName='<%# "DefaultShippingSetting_" + Container.ItemIndex %>' Text=" 通常の配送先に設定する" CssClass="radioBtn" runat="server" OnCheckedChanged="cbDefaultShipping_OnCheckedChanged" AutoPostBack="true"/></td>
		<%} %>
		</dl>
		<div class="button-change" id="hgcChangeShippingInfoBtn" runat="server">
		<asp:LinkButton CommandName="GotoShipping" runat="server" CssClass="btn">変更する</asp:LinkButton>
		</div>
		</div>
	</ItemTemplate>
	</asp:Repeater>

	<%-- ▼定期配送情報▼ --%>
	<div visible="<%# ((((CartObject)(Container.DataItem)).HasFixedPurchase) && (this.IsShowDeliveryPatternInputArea((CartObject)Container.DataItem) == false)) %>" runat="server">
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
					<p>※定期解約されるまで継続して指定した配送パターンでお届けします。</p><br />
					<dt visible='<%# ((CartObject)Container.DataItem).GetShipping().SpecifyShippingTimeFlg %>' runat="server">
						配送希望時間帯
					</dt>
					<dd visible='<%# ((CartObject)Container.DataItem).GetShipping().SpecifyShippingTimeFlg %>' runat="server">
						<%# WebSanitizer.HtmlEncode(GetShippingTime(((CartObject)Container.DataItem).GetShipping()))%>
					</dd>
				</dl>
			</dd>
		</dl>
		<div class="button-change" id="hgcChangeFixedPurchaseShippingInfoBtn" runat="server">
			<asp:LinkButton CommandName="GotoShipping" runat="server" CssClass="btn">変更する</asp:LinkButton>
		</div>
	</div>
	</div>
	<div class="box" visible="<%# (this.IsShowDeliveryPatternInputArea((CartObject)Container.DataItem)) %>" runat="server">
		<%-- ▽デフォルトチェックの設定▽--%>
		<%-- ラジオボタンのデータバインド <%#.. より前で呼び出してください。 --%>
		<%# Constants.FIXED_PURCHASE_USESHIPPINGINTERVALDAYSDEFAULT_FLG ? SetFixedPurchaseDefaultCheckPriority(Container.ItemIndex, 3, 2, 1) : SetFixedPurchaseDefaultCheckPriority(Container.ItemIndex, 2, 3, 1) %>
		<%-- ▼定期購入配送パターン▼ --%>
		<div class="order-unit comfirm-fixed">
			<dl class="order-form">
				<dt>定期購入 配送パターンの指定</dt>
				<dd>
				<dl>
					<dd visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE, false) %>" runat="server">
						<asp:RadioButton ID="rbFixedPurchaseMonthlyPurchase_Date" 
							Text="月間隔日付指定" Checked="<%# GetFixedPurchaseKbnInputChecked(Container.ItemIndex, 1) %>"
							GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" />
						<div id="ddFixedPurchaseMonthlyPurchase_Date" visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE, true) %>" runat="server">
							<asp:DropDownList ID="ddlFixedPurchaseMonth"
								DataSource="<%# GetFixedPurchaseIntervalDropdown(Container.ItemIndex, true) %>"
								DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_MONTH) %>'
								OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" runat="server">
							</asp:DropDownList>
								ヶ月ごと
							<asp:DropDownList ID="ddlFixedPurchaseMonthlyDate" 
								DataSource='<%# ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DATE_LIST) %>'
								DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_MONTHLY_DATE) %>'
								OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged"  AutoPostBack="true" runat="server">
							</asp:DropDownList>
							日に届ける
							<p class="attention">
								<asp:CustomValidator runat="Server" 
									ControlToValidate="ddlFixedPurchaseMonth" 
									ValidationGroup="OrderShipping" 
									ValidateEmptyText="true" 
									SetFocusOnError="true" />
								<asp:CustomValidator runat="Server" 
									ControlToValidate="ddlFixedPurchaseMonthlyDate" 
									ValidationGroup="OrderShipping" 
									ValidateEmptyText="true" 
									SetFocusOnError="true" />
							</p>
						</div>
					</dd>
					<dd visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY, false) %>" runat="server">
						<asp:RadioButton ID="rbFixedPurchaseMonthlyPurchase_WeekAndDay" 
							Text="月間隔・週・曜日指定" Checked="<%# GetFixedPurchaseKbnInputChecked(Container.ItemIndex, 2) %>" 
							GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" />
						<div id="ddFixedPurchaseMonthlyPurchase_WeekAndDay" visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY, true) %>" runat="server">　
						<asp:DropDownList ID="ddlFixedPurchaseIntervalMonths"
							DataSource="<%# GetFixedPurchaseIntervalDropdown(Container.ItemIndex, true, true) %>"
							DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_INTERVAL_MONTHS) %>'
							OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" runat="server" />
						ヶ月ごと
						<asp:DropDownList ID="ddlFixedPurchaseWeekOfMonth"
							DataSource="<%# ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_WEEK_LIST) %>"
							DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_WEEK_OF_MONTH) %>'
							OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged"  AutoPostBack="true" runat="server">
						</asp:DropDownList>
						<asp:DropDownList ID="ddlFixedPurchaseDayOfWeek"
							DataSource="<%# ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DAY_LIST) %>"
							DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_DAY_OF_WEEK) %>'
							OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged"  AutoPostBack="true" runat="server">
						</asp:DropDownList>
							に届ける
						<p class="attention">
						<asp:CustomValidator runat="Server"
							ControlToValidate="ddlFixedPurchaseIntervalMonths"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true" />
						<asp:CustomValidator runat="Server" 
							ControlToValidate="ddlFixedPurchaseWeekOfMonth" 
							ValidationGroup="OrderShipping" 
							ValidateEmptyText="true" 
							SetFocusOnError="true" />
						<asp:CustomValidator runat="Server" 
							ControlToValidate="ddlFixedPurchaseDayOfWeek" 
							ValidationGroup="OrderShipping" 
							ValidateEmptyText="true" 
							SetFocusOnError="true" />
						</p>
						</div>
					</dd>
					<dd id="Div6" visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS, false) %>" runat="server">
						<asp:RadioButton ID="rbFixedPurchaseRegularPurchase_IntervalDays" 
							Text="配送日間隔指定" Checked="<%# GetFixedPurchaseKbnInputChecked(Container.ItemIndex, 3) %>" 
							GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" />
						<div id="ddFixedPurchaseRegularPurchase_IntervalDays" visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS, true) %>" runat="server">
							<asp:DropDownList ID="ddlFixedPurchaseIntervalDays"
						DataSource='<%# GetFixedPurchaseIntervalDropdown(Container.ItemIndex, false) %>'
						DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_INTERVAL_DAYS) %>'
								OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged"  AutoPostBack="true" runat="server">
							</asp:DropDownList>
							日ごとに届ける
							<p class="attention">
							<asp:CustomValidator runat="Server" 
								ControlToValidate="ddlFixedPurchaseIntervalDays" 
								ValidationGroup="OrderShipping" 
								ValidateEmptyText="true" 
								SetFocusOnError="true" />
							</p>
						</div>
						<asp:HiddenField ID="hfFixedPurchaseDaysRequired" Value="<%#: this.ShopShippingList[Container.ItemIndex].FixedPurchaseShippingDaysRequired %>" runat="server" />
						<asp:HiddenField ID="hfFixedPurchaseMinSpan" Value="<%#: this.ShopShippingList[Container.ItemIndex].FixedPurchaseMinimumShippingSpan %>" runat="server" />
					</dd>
					<dd visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY, false) %>" runat="server">
						<asp:RadioButton ID="rbFixedPurchaseEveryNWeek"
							Text="週間隔・曜日指定" Checked="<%# GetFixedPurchaseKbnInputChecked(Container.ItemIndex, 4) %>"
							GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" />
						<div id="ddFixedPurchaseEveryNWeek" visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY, true) %>" runat="server">
							<asp:DropDownList ID="ddlFixedPurchaseEveryNWeek_Week"
								DataSource="<%# GetFixedPurchaseEveryNWeekDropdown(Container.ItemIndex, true) %>"
								DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_WEEK) %>'
								OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true"
								runat="server">
							</asp:DropDownList>
							週間ごと
							<asp:DropDownList ID="ddlFixedPurchaseEveryNWeek_DayOfWeek"
								DataSource="<%# GetFixedPurchaseEveryNWeekDropdown(Container.ItemIndex, false) %>"
								DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_DAY_OF_WEEK) %>'
								OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true"
								runat="server">
							</asp:DropDownList>
							に届ける
						</div>
						<p class="attention">
						<asp:CustomValidator
							runat="Server"
							ControlToValidate="ddlFixedPurchaseEveryNWeek_Week"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"/>
						<asp:CustomValidator
							runat="Server"
							ControlToValidate="ddlFixedPurchaseEveryNWeek_DayOfWeek"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"/>
						</p>
					</dd>
				</dl>
				<dl class="order-form">
					<dt id="dtFirstShippingDate" visible="true" runat="server">初回配送予定日</dt>
					<dd visible="true" runat="server">
						<asp:Label ID="lblFirstShippingDate" runat="server"></asp:Label>
						<asp:DropDownList
							ID="ddlFirstShippingDate"
							visible="false"
							OnDataBound="ddlFirstShippingDate_OnDataBound"
							AutoPostBack="True"
							OnSelectedIndexChanged="ddlFirstShippingDate_ItemSelected"
							runat="server" />
						<asp:Label ID="lblFirstShippingDateNoteMessage" visible="false" runat="server">
							<br>配送予定日は変更となる可能性がありますことをご了承ください。
						</asp:Label>
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
				<dl>
					メール便の場合は数日ずれる可能性があります。
				</dl>
				</dd>
			</dl>
		</div>
	</div>
	<%-- ▲定期配送情報▲ --%>

	<!-- レコメンド設定 -->
	<uc:BodyRecommend runat="server" Cart="<%# (CartObject)Container.DataItem %>" Visible="<%# (this.IsOrderCombined == false) && (this.CartList.Items[0].Payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT) %>" />

	<!-- 定期注文購入金額 -->
	<uc:BodyFixedPurchaseOrderPrice runat="server" Cart="<%# (CartObject)Container.DataItem %>" Visible="<%# ((CartObject)Container.DataItem).HasFixedPurchase %>" />

</ItemTemplate>
</asp:Repeater>

<div class="cart-unit">
<h2>総合計</h2>
	<dl>
		<dt>総合計(税込み)</dt>
		<dd><%#: CurrencyManager.ToPrice(this.CartList.PriceCartListTotal) %></dd>
	</dl>
</div>
<% if (this.IsDispCorrespondenceSpecifiedCommericalTransactions) { %>
	<dl class="specified-commercial-transactions">
		<dt style="background-color: #ccc;padding: 0.5em;line-height: 1.5">特商法に基づく表記</dt>
		<dd style="padding: 0.5em"><%= ShopMessage.GetMessageByPaymentId(this.BindingCartList[0].Payment.PaymentId) %></dd> 
	</dl>
<% } %>
<asp:UpdatePanel ID="upUpdatePanel" runat="server">
<ContentTemplate>
<div class="cart-footer">
	<div class="button-next order">
		<% if (SessionManager.IsChangedAmazonPayForFixedOrNormal) { %>
			<span style="color: red;">
				カート内の商品が変更されました。<br/>
				お手数ですが再度Amazon Payでの購入手続きに進んでください。<br/><br/>
			</span>
			<div style="text-align:center">
				<div id="AmazonPayCv2Button" style="display: inline-block"></div>
			</div>
		<% } else {%>
			<asp:LinkButton id="lbComplete2" runat="server" onclick="lbComplete_Click" CssClass="btn">注文する</asp:LinkButton>
			<asp:LinkButton id="lbCart" runat="server" OnClick="lbCart_Click" class="btn btn-large btn-org-gry" Visible ="true">カートへ戻る</asp:LinkButton>
		<% } %>
	</div>
	<span style="display:none;">
		<asp:LinkButton ID="lbCompleteAfterComfirmPayment" runat="server" onclick="lbComplete_Click"></asp:LinkButton>
	</span>
</div>
</ContentTemplate>
</asp:UpdatePanel>
</section>
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
	var body = <%= new PaidyCheckout(this.CartList.Items.FirstOrDefault()).CreateParameterForPaidyCheckout() %>;
	var hfPaidyPaySelectedControlId = "<%= (this.WhfPaidySelect.ClientID) %>";
	var hfPaidyPaymentIdControlId = "<%= (this.WhfPaidyPaymentId.ClientID) %>";
	var hfPaidyStatusControlId = "<%= (this.WhfPaidyStatus.ClientID) %>";
	var isHistoryPage = false;
	var lbNextProcess = "<%= this.lbCompleteAfterComfirmPayment.ClientID %>";
</script>
<uc:PaidyCheckoutScript runat="server" />
<%--▲▲ Paidy用スクリプト ▲▲--%>
</asp:Content>
