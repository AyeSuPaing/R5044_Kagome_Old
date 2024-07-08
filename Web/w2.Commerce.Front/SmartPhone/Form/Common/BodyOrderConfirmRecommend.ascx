<%--
=========================================================================================================
  Module      : Body Order Confirm Recommend (BodyOrderConfirmRecommend.ascx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/BodyOrderConfirmRecommend.ascx.cs" Inherits="Form_Common_BodyOrderConfirmRecommend" %>
<%@ Register TagPrefix="uc" TagName="BodyRecommend" Src="~/SmartPhone/Form/Common/BodyRecommend.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyFixedPurchaseOrderPrice" Src="~/SmartPhone/Form/Common/BodyFixedPurchaseOrderPrice.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionCvsDef" Src="~/Form/Common/Order/PaymentDescriptionCvsDef.ascx" %>

<%
	this.WlbComplete.OnClientClick = (this.HideOrderButtonWithClick)
		? "return exec_submit(true)"
		: "return exec_submit(false)";
%>
<script type="text/javascript">
	var submitted = false;
	var isMyPage = null;
	var completeButton = null;

	function exec_submit(clearSubmitButton) {
		completeButton = document.getElementById('<%= lbCompleteAfterComfirmPayment.ClientID %>');

		if (submitted === false) {
			<% if (Constants.PRODUCT_ORDER_LIMIT_ENABLED) { %>
				var confirmMessage = '<%= WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_NOT_FIXED_PRODUCT_ORDER_LIMIT) %>' + "\nよろしいですか？";
				<% if (this.HasOrderHistorySimilarShipping) { %>
					if (confirm(confirmMessage) === false) return false;
				<% } %>
			<% } %>
		}

		if (submitted) return false;

		submitted = true;

		<% if (this.WrCartList.Items.Count >= 1) { %>
			if (clearSubmitButton) {
				if (document.getElementById('<%= this.WlbComplete.ClientID %>') != null) {
					document.getElementById('<%= this.WlbComplete.ClientID %>').style.display = "none";
				}

				if (document.getElementById('processing2') != null) {
					document.getElementById('processing2').style.display = "inline";
				}
			}
		<% } %>

		return true;
	}
</script>
<div id="modalRecommend" style="display: none;">
	<div class="modal-content" style="width:100%;left: 0%">
		<asp:LinkButton Style="color: #fff; position: absolute; right: 1rem; top: 15px; font-weight: 600" Text="X" OnClientClick="return closeModalRecommend()" runat="server" />
		<asp:Repeater ID="rCartList" runat="server">
			<ItemTemplate>
				<div class="cart-unit">
					<h2>
						<%#: GetCartIndexDisplay(Container.ItemIndex) %>
						注文内容
					</h2>
					<%-- カート内商品を表示する --%>
					<%-- 通常商品 --%>
					<asp:Repeater ID="rCart" runat="server" DataSource='<%# GetCartObject(Container.DataItem) %>' OnItemDataBound="rCart_OnItemDataBound">
						<HeaderTemplate>
							<table class="cart-table">
								<tbody>
						</HeaderTemplate>
						<ItemTemplate>
							<tr class="cart-unit-product" visible="<%# (GetCartProduct(Container.DataItem).IsSetItem == false) && (GetCartProduct(Container.DataItem).QuantitiyUnallocatedToSet != 0) %>" runat="server">
								<td class="product-image">
									<a target="_blank" href='<%# WebSanitizer.UrlAttrHtmlEncode(GetCartProduct(Container.DataItem).CreateProductDetailUrl()) %>' runat="server" visible="<%# GetCartProduct(Container.DataItem).IsProductDetailLinkValid() %>">
										<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" />
									</a>
									<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" Visible="<%# (GetCartProduct(Container.DataItem).IsProductDetailLinkValid() == false) %>" />
								</td>
								<td class="product-info">
									<ul>
										<li class="product-name">
											<a target="_blank" href='<%# WebSanitizer.UrlAttrHtmlEncode(GetCartProduct(Container.DataItem).CreateProductDetailUrl()) %>' runat="server" visible="<%# GetCartProduct(Container.DataItem).IsProductDetailLinkValid() %>">
												<%#: GetCartProduct(Container.DataItem).ProductJointName %>
											</a>
											<%#: GetCartProduct(Container.DataItem).ProductJoinNameIfProductDetailLinkIsInvalid %>
											<p class="product-msg" visible='<%# GetCartProduct(Container.DataItem).IsDisplayProductTagCartProductMessage %>' runat="server">
												<%#: GetCartProduct(Container.DataItem).ProductTagCartProductMessage %>
											</p>
										</li>
										<li visible='<%# GetCartProduct(Container.DataItem).ProductOptionSettingList.IsSelectedProductOptionValueAll %>' runat="server">
											<asp:Repeater ID="rProductOptionSettings" DataSource='<%# GetCartProduct(Container.DataItem).ProductOptionSettingList %>' runat="server">
												<ItemTemplate>
													<%#: GetProductOptionSetting(Container.DataItem).GetDisplayProductOptionSettingSelectValue() %>
													<br visible="<%# (string.IsNullOrEmpty(GetProductOptionSetting(Container.DataItem).GetDisplayProductOptionSettingSelectValue()) == false)%>" runat="server" />
												</ItemTemplate>
											</asp:Repeater>
										</li>
										<li class="product-price">
											<p runat="server" ID="pPrice" style="padding-top: 2px; text-decoration: line-through" Visible="False" ><span runat="server" ID="sPrice"><%#: CurrencyManager.ToPrice(((CartProduct)Container.DataItem).Price) %></span> (<%#: this.ProductPriceTextPrefix %>)</p>
											<p runat="server" ID="pSubscriptionBoxCampaignPrice" style="padding-top: 2px"><span runat="server" ID="sSubscriptionBoxCampaignPrice"><%#: CurrencyManager.ToPrice(((CartProduct)Container.DataItem).Price) %></span> (<%#: this.ProductPriceTextPrefix %>)</p>
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
											<p ID="pSubscriptionBoxCampaignPeriod" Visible="False" style="color: red; padding-top: 2px" runat="server">キャンペーン期間：<br />
												<span ID="sSubscriptionBoxCampaignPeriodSince" class="spSpecifiedcommercialtransactionsPaddingleft10px" runat="server"></span>～<br />
												<span ID="sSubscriptionBoxCampaignPeriodUntil" class="spSpecifiedcommercialtransactionsPaddingleft10px" runat="server"></span>
											</p>
										</li>
										<li runat="server" visible="<%# GetCartProduct(Container.DataItem).IsDispSaleTerm %>">
											タイムセール期間:<br />
										</li>
										<li runat="server" visible="<%# GetCartProduct(Container.DataItem).IsDispSaleTerm %>" class="spSpecifiedcommercialtransactionsPaddingleft10px">
											<%# WebSanitizer.HtmlEncodeChangeToBr(ProductCommon.GetProductSaleTermBr(this.ShopId, ((CartProduct)Container.DataItem).ProductSaleId)) %>
										</li>
										<li><%# WebSanitizer.HtmlEncodeChangeToBr(GetCartProduct(Container.DataItem).ReturnExchangeMessage) %></li>
										<li style="color:red;" visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
											※配送料無料適用外商品です
										</li>
									</ul>
									<p class="attention" visible="<%# this.ErrorMessages.HasMessages(GetRepeaterItem(Container.Parent.Parent).ItemIndex, Container.ItemIndex) %>" runat="server">
										<%#: this.ErrorMessages.Get(GetRepeaterItem(Container.Parent.Parent).ItemIndex, Container.ItemIndex) %>
									</p>
								</td>
								<td class="product-control">
									<div class="amout">
										数量：<%#: GetCartProduct(Container.DataItem).QuantitiyUnallocatedToSet %>
									</div>
								</td>
							</tr>
							<%-- セット商品 --%>
							<div class="cart-unit-product" visible="<%# (GetCartProduct(Container.DataItem).IsSetItem) && (GetCartProduct(Container.DataItem).ProductSetItemNo == 1) %>" runat="server">
								<asp:Repeater ID="rProductSet" DataSource="<%# GetProductSetDisplay(GetCartProduct(Container.DataItem).ProductSet) %>" runat="server">
									<ItemTemplate>
										<tr>
											<td class="product-image">
												<a target="_blank" href='<%# WebSanitizer.UrlAttrHtmlEncode(GetCartProduct(Container.DataItem).CreateProductDetailUrl()) %>' runat="server" visible="<%# GetCartProduct(Container.DataItem).IsProductDetailLinkValid() %>">
													<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" />
												</a>
												<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" Visible="<%# (GetCartProduct(Container.DataItem).IsProductDetailLinkValid() == false) %>" />
											</td>
											<td class="product-info">
												<ul>
													<li class="product-name">
														<a target="_blank" href='<%# WebSanitizer.UrlAttrHtmlEncode(GetCartProduct(Container.DataItem).CreateProductDetailUrl()) %>' runat="server" visible="<%# GetCartProduct(Container.DataItem).IsProductDetailLinkValid() %>">
															<%#: GetCartProduct(Container.DataItem).ProductJointName %>
														</a>
														<%#: GetCartProduct(Container.DataItem).ProductJoinNameIfProductDetailLinkIsInvalid %>
														<p class="product-msg" visible='<%# GetCartProduct(Container.DataItem).IsDisplayProductTagCartProductMessage %>' runat="server">
															<%#: GetCartProduct(Container.DataItem).ProductTagCartProductMessage %>
														</p>
													</li>
													<li class="product-price">
														<p><%#: CurrencyManager.ToPrice(GetCartProduct(Container.DataItem).Price) %> (<%#: this.ProductPriceTextPrefix %>)&nbsp;&nbsp;x&nbsp;&nbsp;<%#: GetCartProduct(Container.DataItem).CountSingle %></p>
														</dd>
													</li>
													<li><%# WebSanitizer.HtmlEncodeChangeToBr(GetCartProduct(Container.DataItem).ReturnExchangeMessage) %></li>
													<li style="color:red;" visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
														※配送料無料適用外商品です
													</li>
												</ul>
											</td>
											<td class="product-control" visible="<%# (GetCartProduct(Container.DataItem).ProductSetItemNo == 1) %>" rowspan="<%# GetCountOfProductSet(GetCartProduct(Container.DataItem).ProductSet) %>" runat="server">
												<div class="amout">
													セット <%#: GetProductSetCount(GetCartProduct(Container.DataItem)) %><br />
													<%#: CurrencyManager.ToPrice(GetProductSetPriceSubtotal(GetCartProduct(Container.DataItem))) %> (<%#: this.ProductPriceTextPrefix %>)
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
					<asp:Repeater ID="rCartSetPromotion" DataSource="<%# GetCartObject(Container.DataItem).SetPromotions %>" runat="server">
						<ItemTemplate>
							<div class="cart-set-promotion-unit">
								<asp:HiddenField ID="hfCartSetPromotionNo" runat="server" Value="<%# GetCartSetPromotion(Container.DataItem).CartSetPromotionNo %>" />
								<asp:Repeater ID="rCartSetPromotionItem" DataSource="<%# GetCartSetPromotion(Container.DataItem).Items %>" runat="server">
									<HeaderTemplate>
										<table class="cart-table">
											<tbody>
									</HeaderTemplate>
									<ItemTemplate>
										<tr>
											<td class="product-image">
												<a target="_blank" href='<%# WebSanitizer.UrlAttrHtmlEncode(GetCartProduct(Container.DataItem).CreateProductDetailUrl()) %>' runat="server" visible="<%# GetCartProduct(Container.DataItem).IsProductDetailLinkValid() %>">
													<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" />
												</a>
												<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" Visible="<%# (GetCartProduct(Container.DataItem).IsProductDetailLinkValid() == false) %>" />
											</td>
											<td class="product-info">
												<ul>
													<li class="product-name">
														<a target="_blank" href='<%# WebSanitizer.UrlAttrHtmlEncode(GetCartProduct(Container.DataItem).CreateProductDetailUrl()) %>' runat="server" visible="<%# GetCartProduct(Container.DataItem).IsProductDetailLinkValid() %>">
															<%#: GetCartProduct(Container.DataItem).ProductJointName %>
														</a>
														<%#: GetCartProduct(Container.DataItem).ProductJoinNameIfProductDetailLinkIsInvalid %>
														<div visible='<%# GetCartProduct(Container.DataItem).ProductOptionSettingList.IsSelectedProductOptionValueAll %>' runat="server">
															<asp:Repeater ID="rProductOptionSettings" DataSource='<%# GetCartProduct(Container.DataItem).ProductOptionSettingList %>' runat="server">
																<ItemTemplate>
																	<%#: GetProductOptionSetting(Container.DataItem).GetDisplayProductOptionSettingSelectValue() %>
																	<br visible="<%# (string.IsNullOrEmpty(GetProductOptionSetting(Container.DataItem).GetDisplayProductOptionSettingSelectValue()) == false) %>" runat="server" />
																</ItemTemplate>
															</asp:Repeater>
														</div>
													</li>
													<li class="product-price">
														<%#: CurrencyManager.ToPrice(GetCartProduct(Container.DataItem).Price) %> (<%#: this.ProductPriceTextPrefix %>)
													</li>
													<li style="color:red;" visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
														※配送料無料適用外商品です
													</li>
												</ul>
												<p class="attention" visible="<%# this.ErrorMessages.HasMessages(GetRepeaterItem(Container.Parent.Parent).ItemIndex, Container.ItemIndex) %>" runat="server">
													<%#: this.ErrorMessages.Get(GetRepeaterItem(Container.Parent.Parent).ItemIndex, Container.ItemIndex) %>
												</p>
												<p class="attention" visible="<%# this.ErrorMessages.HasMessages(GetRepeaterItem(Container.Parent.Parent.Parent.Parent).ItemIndex, GetRepeaterItem(Container.Parent.Parent).ItemIndex, Container.ItemIndex) %>" runat="server">
													<%#: this.ErrorMessages.Get(GetRepeaterItem(Container.Parent.Parent.Parent.Parent).ItemIndex, GetRepeaterItem(Container.Parent.Parent).ItemIndex, Container.ItemIndex) %>
												</p>
											</td>
											<td class="product-control">
												<div class="amout">
													数量：<%#: GetCartProduct(Container.DataItem).QuantityAllocatedToSet[GetCartSetPromotion(GetRepeaterItem(Container.Parent.Parent).DataItem).CartSetPromotionNo] %>
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
										<dt><%#: GetCartSetPromotion(Container.DataItem).SetpromotionDispName %></dt>
										<dt>
											<span class="line-through" visible="<%# GetCartSetPromotion(Container.DataItem).IsDiscountTypeProductDiscount %>" runat="server">
												<%#: CurrencyManager.ToPrice(GetCartSetPromotion(Container.DataItem).UndiscountedProductSubtotal) %> (税込)
											</span>
											<br>
											<%#: CurrencyManager.ToPrice(GetCartSetPromotion(Container.DataItem).UndiscountedProductSubtotal - GetCartSetPromotion(Container.DataItem).ProductDiscountAmount) %> (税込)
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
							<dd><%#: CurrencyManager.ToPrice(GetCartObject(Container.DataItem).PriceSubtotal) %></dd>
						</dl>
						<% if (this.ProductIncludedTaxFlg == false) { %>
						<dl>
							<dt>消費税額</dt>
							<dd>
								<%#: CurrencyManager.ToPrice(GetCartObject(Container.DataItem).PriceSubtotalTax) %>
							</dd>
						</dl>
						<% } %>
						<%-- セットプロモーション(商品割引) --%>
						<asp:Repeater DataSource="<%# GetCartObject(Container.DataItem).SetPromotions %>" runat="server">
							<ItemTemplate>
								<dl visible="<%# GetCartSetPromotion(Container.DataItem).IsDiscountTypeProductDiscount %>" runat="server">
									<dt>
										<%#: GetCartSetPromotion(Container.DataItem).SetpromotionDispName %>
									</dt>
									<dd>
										<%#: GetPrefixForDisplayPrice(GetCartSetPromotion(Container.DataItem).ProductDiscountAmount) %>
										<%#: CurrencyManager.ToPrice(GetCartSetPromotion(Container.DataItem).ProductDiscountAmount) %>
									</dd>
								</dl>
							</ItemTemplate>
						</asp:Repeater>
						<% if (Constants.MEMBER_RANK_OPTION_ENABLED && this.IsLoggedIn) { %>
						<dl>
							<dt>会員ランク割引額</dt>
							<dd>
								<%#: GetPrefixForDisplayPrice(GetCartObject(Container.DataItem).MemberRankDiscount) %>
								<%#: GetDiscountPriceCalculate(GetCartObject(Container.DataItem).MemberRankDiscount) %>
							</dd>
						</dl>
						<% } %>
						<% if (Constants.MEMBER_RANK_OPTION_ENABLED && Constants.FIXEDPURCHASE_OPTION_ENABLED && this.IsLoggedIn) { %>
						<dl>
							<dt>定期会員割引額</dt>
							<dd>
								<%#: GetPrefixForDisplayPrice(GetCartObject(Container.DataItem).FixedPurchaseMemberDiscountAmount) %>
								<%#: GetDiscountPriceCalculate(GetCartObject(Container.DataItem).FixedPurchaseMemberDiscountAmount) %>
							</dd>
						</dl>
						<% } %>
						<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
						<div runat="server" visible="<%# (GetCartObject(Container.DataItem).HasFixedPurchase) %>">
							<dl>
								<dt>定期購入割引額</dt>
								<dd>
									<%#: GetPrefixForDisplayPrice(GetCartObject(Container.DataItem).FixedPurchaseDiscount) %>
									<%#: GetDiscountPriceCalculate(GetCartObject(Container.DataItem).FixedPurchaseDiscount) %>
								</dd>
							</dl>
						</div>
						<% } %>
						<% if (Constants.W2MP_COUPON_OPTION_ENABLED) { %>
						<dl>
							<dt>クーポン割引額</dt>
							<dd>
								<%#: GetCouponName(GetCartObject(Container.DataItem)) %>
								<%#: GetPrefixForDisplayPrice(GetCartObject(Container.DataItem).UseCouponPrice) %>
								<%#: GetDiscountPriceCalculate(GetCartObject(Container.DataItem).UseCouponPrice) %>
							</dd>
						</dl>
						<% } %>
						<% if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsLoggedIn) { %>
						<dl>
							<dt>ポイント利用額</dt>
							<dd>
								<%#: GetPrefixForDisplayPrice(GetCartObject(Container.DataItem).UsePointPrice) %>
								<%#: GetDiscountPriceCalculate(GetCartObject(Container.DataItem).UsePointPrice) %>
							</dd>
						</dl>
						<% } %>
						<%-- 配送料金 --%>
						<dl runat="server" visible='<%# (GetCartObject(Container.DataItem).ShippingPriceSeparateEstimateFlg == false) %>'>
							<dt>配送料金</dt>
							<dd>
								<%#: CurrencyManager.ToPrice(GetCartObject(Container.DataItem).PriceShipping) %>
							</dd>
						</dl>
						<dl runat="server" visible='<%# GetCartObject(Container.DataItem).ShippingPriceSeparateEstimateFlg %>'>
							<dt>配送料金</dt>
							<dd>
								<%#: GetCartObject(Container.DataItem).ShippingPriceSeparateEstimateMessage %>
							</dd>
							<small style="color:red;" visible="<%# ((CartObject)Container.DataItem).IsDisplayFreeShiipingFeeText %>" runat="server">
								※配送料無料適用外の商品が含まれるため、<%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceShipping) %>の配送料が請求されます
							</small>
						</dl>
						<%-- セットプロモーション(配送料割引) --%>
						<asp:Repeater DataSource="<%# GetCartObject(Container.DataItem).SetPromotions %>" runat="server">
							<ItemTemplate>
								<dl visible="<%# GetCartSetPromotion(Container.DataItem).IsDiscountTypeShippingChargeFree %>" runat="server">
									<dt>
										<%#: GetCartSetPromotion(Container.DataItem).SetpromotionDispName %>(送料割引)
									</dt>
									<dd>
										<%#: GetPrefixForDisplayPrice(GetCartSetPromotion(Container.DataItem).ShippingChargeDiscountAmount) %>
										<%#: CurrencyManager.ToPrice(GetCartSetPromotion(Container.DataItem).ShippingChargeDiscountAmount) %>
									</dd>
								</dl>
							</ItemTemplate>
						</asp:Repeater>
						<dl>
							<dt>決済手数料</dt>
							<dd>
								<%#: CurrencyManager.ToPrice(GetCartPayment(Container.DataItem).PriceExchange) %>
							</dd>
						</dl>
						<%-- セットプロモーション(決済手数料割引) --%>
						<asp:Repeater DataSource="<%# GetCartObject(Container.DataItem).SetPromotions %>" runat="server">
							<ItemTemplate>
								<dl visible="<%# GetCartSetPromotion(Container.DataItem).IsDiscountTypePaymentChargeFree %>" runat="server">
									<dt>
										<%#: GetCartSetPromotion(Container.DataItem).SetpromotionDispName %>(決済手数料割引)
									</dt>
									<dd>
										<%#: GetPrefixForDisplayPrice(GetCartSetPromotion(Container.DataItem).PaymentChargeDiscountAmount) %>
										<%#: CurrencyManager.ToPrice(GetCartSetPromotion(Container.DataItem).PaymentChargeDiscountAmount) %>
									</dd>
								</dl>
							</ItemTemplate>
						</asp:Repeater>
						<dl visible="<%# (GetCartObject(Container.DataItem).PriceRegulation != 0) %>" runat="server">
							<dt>調整金額</dt>
							<dd>
								<%#: GetPrefixForDisplayPrice(GetCartObject(Container.DataItem).PriceRegulation, true) %>
								<%#: CurrencyManager.ToPrice(Math.Abs(GetCartObject(Container.DataItem).PriceRegulation)) %>
							</dd>
						</dl>
						<dl>
							<dt>合計(税込)</dt>
							<dd>
								<%#: CurrencyManager.ToPrice(GetCartObject(Container.DataItem).PriceTotal) %>
							</dd>
							<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
							<dt>決済金額(税込)</dt>
							<dd><%#: GetSettlementAmount(GetCartObject(Container.DataItem)) %></dd>
							<small style="color: red">
								<%#: string.Format(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_AMOUNT_VARIES_WITH_RATE), GetCartObject(Container.DataItem).SettlementCurrency) %>
							</small>
							<% } %>
						</dl>
						<% if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsLoggedIn) { %>
						<dl visible="<%# (GetCartObjectListByRepeater(((Repeater)Container.Parent).DataSource).Items.Count == (Container.ItemIndex + 1)) %>" runat="server">
							<dt visible="<%# GetCartObject(Container.DataItem).IsHasFirstBuyPoint %>" runat="server">
								初回購入獲得ポイント
							</dt>
							<dd visible="<%# GetCartObject(Container.DataItem).IsHasFirstBuyPoint %>" runat="server">
								<%#: GetNumeric(GetCartObjectListByRepeater(((Repeater)Container.Parent).DataSource).TotalFirstBuyPoint) %>pt
							</dd>
							<dt>購入後獲得ポイント</dt>
							<dd><%#: GetNumeric(GetCartObjectListByRepeater(((Repeater)Container.Parent).DataSource).TotalBuyPoint) %>pt</dd>
							<small>※ 1pt = <%: CurrencyManager.ToPrice(1m) %></small>
						</dl>
						<% } %>
						<div class="InternationalShippingAttention" runat="server" visible="<%# IsDisplayProductTaxExcludedMessage(GetCartObject(Container.DataItem)) %>">
							※国外配送をご希望の場合関税・商品消費税は料金に含まれず、商品到着後、現地にて税をお支払いいただくこととなりますのでご注意ください。
						</div>
					</div>
				</div>
				<div class="box">
					<div class="order-unit comfirm-payment">
						<dl class="order-form">
							<dt>本人情報確認</dt>
							<dd>
								<dl>
									<%-- 氏名 --%>
									<dt>
										<%: ReplaceTag("@@User.name.name@@") %>：
									</dt>
									<dd>
										<%#: GetCartOwner(Container.DataItem).Name1 %>
										<%#: GetCartOwner(Container.DataItem).Name2 %>&nbsp;様
									</dd>
									<% if ((this.IsAmazonCv2Guest == false)
										|| (Constants.AMAZON_PAYMENT_CV2_ENABLED && Constants.AMAZON_PAYMENT_CV2_USE_SHIPPING_AS_OWNER_ENABLED)) { %>
									<%-- 氏名（かな） --%>
									<dl visible="<%# ((string.IsNullOrEmpty(GetCartOwner(Container.DataItem).NameKana) == false) && (GetCartOwner(Container.DataItem).IsAddrJp)) %>" runat="server">
										<dt>
											<%: ReplaceTag("@@User.name_kana.name@@") %>：
										</dt>
										<dd>
											<%#: GetCartOwner(Container.DataItem).NameKana1 %>
											<%#: GetCartOwner(Container.DataItem).NameKana2 %>&nbsp;さま
										</dd>
									</dl>
									<% } %>
									<%-- メールアドレス --%>
									<dt>
										<%: ReplaceTag("@@User.mail_addr.name@@") %>：
									</dt>
									<dd>
										<%#: GetTextDisplay(GetCartOwner(Container.DataItem).MailAddr, true) %>
									</dd>
									<% if ((this.IsAmazonCv2Guest == false) || (Constants.AMAZON_PAYMENT_CV2_ENABLED && Constants.AMAZON_PAYMENT_CV2_USE_SHIPPING_AS_OWNER_ENABLED)) { %>
									<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
									<dt>
										<%: ReplaceTag("@@User.mail_addr2.name@@") %>：
									</dt>
									<dd>
										<%#: GetTextDisplay(GetCartOwner(Container.DataItem).MailAddr2, true) %>
									</dd>
									<br />
									<% } %>
									<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
									<dt>
										<%: ReplaceTag("@@User.company_name.name@@") %>・
										<%: ReplaceTag("@@User.company_post_name.name@@") %>：
									</dt>
									<dd>
										<%#: GetCartOwner(Container.DataItem).CompanyName %>
										<br />
										<%#: GetCartOwner(Container.DataItem).CompanyPostName %>
									</dd>
									<% } %>
									<%-- 電話番号 --%>
									<dt><%: ReplaceTag("@@User.tel1.name@@") %>：</dt>
									<dd><%#: GetCartOwner(Container.DataItem).Tel1 %></dd>
									<dt><%: ReplaceTag("@@User.tel2.name@@") %>：</dt>
									<dd><%#: GetCartOwner(Container.DataItem).Tel2 %>&nbsp;</dd>
									<dt><%: ReplaceTag("@@User.mail_flg.name@@") %>：</dt>
									<dd><%#: ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_FLG, GetCartOwner(Container.DataItem).StatusRequestDeliveryOfNotificationMail) %><br />
										&nbsp;</dd>
									<% } %>
								</dl>
							</dd>
							<p class="clr">
								<img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1" />
							</p>
						</dl>
					</div>
				</div>
				<!--box-->
				<div class="order-unit comfirm-payment">
					<dl class="order-form">
						<dt>お支払い方法</dt>
						<dd>
							<%#: GetCartPayment(Container.DataItem).PaymentName %>
							<div visible='<%# GetCartPayment(Container.DataItem).IsPaymentEcPay %>' runat="server">支払い方法：</div>
							<div visible='<%# GetCartPayment(Container.DataItem).IsPaymentEcPay %>' runat="server">
								<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE, GetCartPayment(Container.DataItem).ExternalPaymentType) %>
							</div>
							<div visible='<%# GetCartPayment(Container.DataItem).IsPaymentNewebPay %>' runat="server">支払い方法：</div>
							<div visible='<%# GetCartPayment(Container.DataItem).IsPaymentNewebPay %>' runat="server">
								<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE + "_neweb", GetCartPayment(Container.DataItem).ExternalPaymentType) %>
							</div>
							<div visible='<%# GetCartPayment(Container.DataItem).IsPaymentCvsPre %>' runat="server">
								支払先コンビニ名
							</div>
							<div visible='<%# GetCartPayment(Container.DataItem).IsPaymentCvsPre %>' runat="server">
								<%#: GetCartObject(Container.DataItem).GetPaymentCvsName() %>
							</div>
							<div id="dvCvsDef" visible="<%# GetCartPayment(Container.DataItem).IsPaymentCvsDef %>" runat="server">
								<uc:PaymentDescriptionCvsDef runat="server" id="ucPaymentDescriptionCvsDef" />
							</div>
							<div visible='<%# GetCartPayment(Container.DataItem).HasCreditCardCompany %>' runat="server">
								カード会社：<%#: GetCartPayment(Container.DataItem).CreditCardCompanyName %>
							</div>
							<div visible='<%# GetCartPayment(Container.DataItem).HasCreditCardNo %>' runat="server">
								カード番号：　XXXXXXXXXXXX<%#: GetCreditCardDispString(GetCartPayment(Container.DataItem)) %>
							</div>
							<div visible='<%# GetCartPayment(Container.DataItem).HasCreditCardNo %>' runat="server">
								有効期限：<%#: GetCartPayment(Container.DataItem).CreditExpireMonth %>/<%#: GetCartPayment(Container.DataItem).CreditExpireYear %> (月/年)
							</div>
							<div visible='<%# GetCartPayment(Container.DataItem).HasCreditCardNo %>' runat="server">
								支払い回数：<%#: ValueText.GetValueText(Constants.TABLE_ORDER, OrderCommon.CreditInstallmentsValueTextFieldName, GetCartPayment(Container.DataItem).CreditInstallmentsCode) %>
							</div>
							<div visible='<%# GetCartPayment(Container.DataItem).HasCreditCardNo %>' runat="server">
								カード名義：<%#: GetCartPayment(Container.DataItem).CreditAuthorName %>
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
								<dd><%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_RECEIPT_FLG, GetCartObject(Container.DataItem).ReceiptFlg) %></dd>
								<div runat="server" visible="<%# GetCartObject(Container.DataItem).IsReceiptFlagOn %>">
									<dt>宛名</dt>
									<dd><%#: GetCartObject(Container.DataItem).ReceiptAddress %></dd>
									<dt>但し書き</dt>
									<dd><%#: GetCartObject(Container.DataItem).ReceiptProviso %></dd>
								</div>
							</dl>
						</dd>
					</dl>
					<% } %>
					<%-- ▲領収書情報▲ --%>
				</div>
				<%-- ▼お届け先情報▼ --%>
				<asp:Repeater ID="rCartShippings" DataSource='<%# Eval("Shippings") %>' runat="server">
					<ItemTemplate>
						<div visible="<%# (FindCart(Container.DataItem).IsDigitalContentsOnly == false) %>" runat="server">
							<div class="order-unit comfirm-payment">
								<dl class="order-form">
									<dt>お届け先</dt>
									<dd>
										<div visible="<%# (FindCart(Container.DataItem).IsGift == false) %>" runat="server">
											<dl>
												<div runat="server" visible='<%# ((GetCartShipping(Container.DataItem).IsNotShippingAddressConvenienceStore) && (GetCartShipping(Container.DataItem).IsConvenienceStoreFlagOff)) %>'>
													<%-- 氏名 --%>
													<dt><%: ReplaceTag("@@User.name.name@@") %>：</dt>
													<dd><%#: Eval("Name1") %><%#: Eval("Name2") %>&nbsp;様</dd>
													<%-- 氏名（かな） --%>
													<div visible="<%# (string.IsNullOrEmpty(GetCartShipping(Container.DataItem).NameKana) == false) %>" runat="server">
														<dt visible="<%# GetCartShipping(Container.DataItem).IsShippingAddrJp %>" runat="server">
															<%: ReplaceTag("@@User.name_kana.name@@") %>：
														</dt>
														<dd visible="<%# GetCartShipping(Container.DataItem).IsShippingAddrJp %>" runat="server">
															<%#: Eval("NameKana1") %><%#: Eval("NameKana2") %>&nbsp;さま
														</dd>
													</div>
													<dt>
														<%: ReplaceTag("@@User.addr.name@@") %>：
													</dt>
													<dd>
														<p>
															<%# WebSanitizer.HtmlEncodeChangeToBr(GetAddressDisplayText(GetCartShipping(Container.DataItem))) %>
														</p>
													</dd>
													<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
													<dt>
														<%: ReplaceTag("@@User.company_name.name@@") %>・
														<%: ReplaceTag("@@User.company_post_name.name@@") %>：
													</dt>
													<dd>
														<%#: Eval("CompanyName") %>&nbsp<%#: Eval("CompanyPostName") %>
													</dd>
													<% } %>
													<%-- 電話番号 --%>
													<dt><%: ReplaceTag("@@User.tel1.name@@") %>：</dt>
													<dd><%#: Eval("Tel1") %></dd>
												</div>
												<div runat="server" visible="<%# GetCartShipping(Container.DataItem).IsConvenienceStoreFlagOn %>">
													<dt>店舗ID：</dt>
													<dd><%#: GetCartShipping(Container.DataItem).ConvenienceStoreId %>&nbsp;</dd>
													<dt>店舗名称：</dt>
													<dd><%#: GetCartShipping(Container.DataItem).Name1 %>&nbsp;</dd>
													<dt>店舗住所：</dt>
													<dd><%#: GetCartShipping(Container.DataItem).Addr4 %>&nbsp;</dd>
													<dt>店舗電話番号：</dt>
													<dd><%#: GetCartShipping(Container.DataItem).Tel1 %>&nbsp;</dd>
												</div>
												<span visible="<%# (FindCart(Container.DataItem).IsDigitalContentsOnly == false) && (GetCartShipping(Container.DataItem).IsConvenienceStoreFlagOff) %>" runat="server">
													<dt>配送方法：</dt>
													<dd>
														<%#: ValueText.GetValueText(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD, GetCartShipping(Container.DataItem).ShippingMethod) %>
													</dd>
													<dt visible="<%# CanDisplayDeliveryCompany(GetCartIndexFromControl(Container), Container.ItemIndex) %>" runat="server">配送サービス：</dt>
													<dd visible="<%# CanDisplayDeliveryCompany(GetCartIndexFromControl(Container), Container.ItemIndex) %>" runat="server">
														<%#: GetDeliveryCompanyName(GetCartShipping(Container.DataItem).DeliveryCompanyId) %>
													</dd>
													<dt visible='<%# Eval("SpecifyShippingDateFlg") %>' runat="server">配送希望日：</dt>
													<dd visible='<%# Eval("SpecifyShippingDateFlg") %>' runat="server">
														<%#: GetShippingDate(GetCartShipping(Container.DataItem)) %>
													</dd>
													<dt visible='<%# Eval("SpecifyShippingTimeFlg") %>' runat="server">配送希望時間帯：</dt>
													<dd visible='<%# Eval("SpecifyShippingTimeFlg") %>' runat="server">
														<%#: GetShippingTime(GetCartShipping(Container.DataItem)) %>
													</dd>
												</span>
												<%-- 注文メモ --%>
												<dt visible='<%# GetCartShipping(Container.DataItem).CartObject.GetOrderMemosForOrderConfirm().Trim() != string.Empty %>' runat="server">注文メモ：</dt>
												<dd visible='<%# GetCartShipping(Container.DataItem).CartObject.GetOrderMemosForOrderConfirm().Trim() != string.Empty %>' runat="server">
													<%# WebSanitizer.HtmlEncodeChangeToBr(GetCartShipping(Container.DataItem).CartObject.GetOrderMemosForOrderConfirm()) %>
												</dd>
												<dd>
													<%#: GetReflectMemoToFixedPurchase(GetCartShipping(Container.DataItem).CartObject.ReflectMemoToFixedPurchase) %>
												</dd>
												<asp:Repeater
													ID="rOrderExtendDisplay"
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
															<%#: GetInputTextDisplay(Item.InputValue, Item.InputText) %>
														</dd>
													</ItemTemplate>
													<FooterTemplate>
																</dl>
															</dd>
														</dl>
													</FooterTemplate>
												</asp:Repeater>
											</dl>
										</div>
										<span runat="server" visible="<%# OrderCommon.DisplayTwInvoiceInfo(GetCartShipping(Container.DataItem).ShippingCountryIsoCode) %>">
											<dt>発票種類</dt>
											<dd>
												<%#: ValueText.GetValueText(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE, GetCartShipping(Container.DataItem).UniformInvoiceType).Replace("コード", string.Empty) %>
											</dd>
											<dt runat="server" visible="<%# GetCartShipping(Container.DataItem).IsNotUniformInvoicePersonal %>">
												<%#: GetUniformInvoiceCodeOptionName(GetCartShipping(Container.DataItem).UniformInvoiceType) %>
											</dt>
											<dd runat="server" visible="<%# GetCartShipping(Container.DataItem).IsNotUniformInvoicePersonal %>">
												<%#: GetCartShipping(Container.DataItem).UniformInvoiceOption1 %>
											</dd>
											<dt runat="server" visible="<%# GetCartShipping(Container.DataItem).IsUniformInvoiceCompany %>">
												<%#: ReplaceTag("@@TwInvoice.uniform_invoice_company_name_option.name@@") %>
											</dt>
											<dd runat="server" visible="<%# GetCartShipping(Container.DataItem).IsUniformInvoiceCompany %>">
												<%#: GetCartShipping(Container.DataItem).UniformInvoiceOption2 %>
											</dd>
											<span runat="server" visible="<%# GetCartShipping(Container.DataItem).IsUniformInvoicePersonal %>">
												<dt>共通性載具</dt>
												<dd>
													<%#: GetCarryTypeValue(GetCartShipping(Container.DataItem).CarryType) %>
												</dd>
												<dt visible="<%# (string.IsNullOrEmpty(GetCartShipping(Container.DataItem).CarryType) == false) %>" runat="server">
													載具コード
												</dt>
												<dd visible="<%# (string.IsNullOrEmpty(GetCartShipping(Container.DataItem).CarryType) == false) %>" runat="server">
													<%#: GetCartShipping(Container.DataItem).CarryTypeOptionValue %>
												</dd>
											</span>
											<td>
												<asp:CheckBox ID="cbDefaultInvoice"
													GroupName='<%#: "DefaultInvoiceSetting_" + Container.ItemIndex %>'
													class="radioBtn DefaultInvoice"
													Text="通常の電子発票情報に設定する"
													OnCheckedChanged="cbDefaultInvoice_CheckedChanged"
													AutoPostBack="true"
													runat="server"
													Visible="<%# this.IsLoggedIn %>" />
											</td>
										</span>
									</dd>
								</dl>
							</div>
						</div>
					</ItemTemplate>
				</asp:Repeater>
				<%-- ▼定期配送情報▼ --%>
				<div visible="<%# ((GetCartObject(Container.DataItem).HasFixedPurchase) && (this.IsShowDeliveryPatternInputArea(GetCartObject(Container.DataItem)) == false)) %>" runat="server">
					<div class="order-unit comfirm-fixed" visible="<%# GetCartObject(Container.DataItem).HasFixedPurchase %>" runat="server">
						<dl class="order-form">
							<dt>定期配送情報</dt>
							<dd>
								<dl>
									<dt>配送パターン</dt>
									<dd>
										<%#: GetCartObject(Container.DataItem).GetShipping().GetFixedPurchaseShippingPatternString() %>
									</dd>
									<dt>初回配送予定</dt>
									<dd>
										<%#: DateTimeUtility.ToStringFromRegion(GetCartObject(Container.DataItem).GetShipping().GetFirstShippingDate(), DateTimeUtility.FormatType.LongDateWeekOfDay1Letter) %>
									</dd>
									<dt>今後の配送予定</dt>
									<dd>
										<%#: DateTimeUtility.ToStringFromRegion(GetCartObject(Container.DataItem).GetShipping().NextShippingDate, DateTimeUtility.FormatType.LongDateWeekOfDay1Letter) %>&nbsp;
										<%#: DateTimeUtility.ToStringFromRegion(GetCartObject(Container.DataItem).GetShipping().NextNextShippingDate, DateTimeUtility.FormatType.LongDateWeekOfDay1Letter) %>
									</dd>
									<p>※定期解約されるまで継続して指定した配送パターンでお届けします。</p>
									<br />
									<dt visible='<%# GetCartObject(Container.DataItem).GetShipping().SpecifyShippingTimeFlg %>' runat="server">
										配送希望時間帯
									</dt>
									<dd visible='<%# GetCartObject(Container.DataItem).GetShipping().SpecifyShippingTimeFlg %>' runat="server">
										<%#: GetShippingTime(GetCartObject(Container.DataItem).GetShipping()) %>
									</dd>
								</dl>
							</dd>
						</dl>
					</div>
				</div>
				<div class="box" visible="<%# this.IsShowDeliveryPatternInputArea(GetCartObject(Container.DataItem)) %>" runat="server">
					<%-- ▼定期購入配送パターン▼ --%>
					<div class="order-unit comfirm-fixed">
						<dl class="order-form">
							<dt>定期購入 配送パターンの指定</dt>
							<dd>
								<dl>
									<dd
										visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE, false) %>"
										runat="server">
										<asp:RadioButton
											ID="rbFixedPurchaseMonthlyPurchase_Date"
											Text="月間隔日付指定"
											Checked="<%# GetFixedPurchaseKbnInputChecked(Container.ItemIndex, 1) %>"
											GroupName="FixedPurchaseShippingPattern"
											OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged"
											AutoPostBack="true"
											runat="server" />
										<div
											id="ddFixedPurchaseMonthlyPurchase_Date"
											visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE, true) %>"
											runat="server">
											<asp:DropDownList
												ID="ddlFixedPurchaseMonth"
												DataSource="<%# GetFixedPurchaseIntervalDropdown(Container.ItemIndex, true) %>"
												DataTextField="Text"
												DataValueField="Value"
												SelectedValue='<%#: GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_MONTH) %>'
												OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged"
												AutoPostBack="true"
												runat="server" />
											ヶ月ごと
											<asp:DropDownList
												ID="ddlFixedPurchaseMonthlyDate"
												DataSource='<%# ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DATE_LIST) %>'
												DataTextField="Text"
												DataValueField="Value"
												SelectedValue='<%#: GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_MONTHLY_DATE) %>'
												OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged"
												AutoPostBack="true"
												runat="server" />
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
									<dd
										visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY, false) %>"
										runat="server">
										<asp:RadioButton
											ID="rbFixedPurchaseMonthlyPurchase_WeekAndDay"
											Text="月間隔・週・曜日指定"
											Checked="<%# GetFixedPurchaseKbnInputChecked(Container.ItemIndex, 2) %>"
											GroupName="FixedPurchaseShippingPattern"
											OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged"
											AutoPostBack="true"
											runat="server" />
										<div
											id="ddFixedPurchaseMonthlyPurchase_WeekAndDay"
											visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY, true) %>"
											runat="server">
											<asp:DropDownList
												ID="ddlFixedPurchaseIntervalMonths"
												DataSource="<%# GetFixedPurchaseIntervalDropdown(Container.ItemIndex, true, true) %>"
												DataTextField="Text"
												DataValueField="Value"
												SelectedValue='<%#: GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_INTERVAL_MONTHS) %>'
												OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged"
												AutoPostBack="true"
												runat="server" />
											ヶ月ごと
											<asp:DropDownList
												ID="ddlFixedPurchaseWeekOfMonth"
												DataSource="<%# ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_WEEK_LIST) %>"
												DataTextField="Text"
												DataValueField="Value"
												SelectedValue='<%#: GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_WEEK_OF_MONTH) %>'
												OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged"
												AutoPostBack="true"
												runat="server" />
											<asp:DropDownList
												ID="ddlFixedPurchaseDayOfWeek"
												DataSource="<%# ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DAY_LIST) %>"
												DataTextField="Text"
												DataValueField="Value"
												SelectedValue='<%#: GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_DAY_OF_WEEK) %>'
												OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged"
												AutoPostBack="true"
												runat="server" />
											に届ける
											<p class="attention">
												<asp:CustomValidator
													runat="Server"
													ControlToValidate="ddlFixedPurchaseIntervalMonths"
													ValidationGroup="OrderShipping"
													ValidateEmptyText="true"
													SetFocusOnError="true" />
												<asp:CustomValidator
													runat="Server"
													ControlToValidate="ddlFixedPurchaseWeekOfMonth"
													ValidationGroup="OrderShipping"
													ValidateEmptyText="true"
													SetFocusOnError="true" />
												<asp:CustomValidator
													runat="Server"
													ControlToValidate="ddlFixedPurchaseDayOfWeek"
													ValidationGroup="OrderShipping"
													ValidateEmptyText="true"
													SetFocusOnError="true" />
											</p>
										</div>
									</dd>
									<dd
										visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS, false) %>"
										runat="server">
										<asp:RadioButton
											ID="rbFixedPurchaseRegularPurchase_IntervalDays"
											Text="配送日間隔指定"
											Checked="<%# GetFixedPurchaseKbnInputChecked(Container.ItemIndex, 3) %>"
											GroupName="FixedPurchaseShippingPattern"
											OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged"
											AutoPostBack="true"
											runat="server" />
										<div
											id="ddFixedPurchaseRegularPurchase_IntervalDays"
											visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS, true) %>"
											runat="server">
											<asp:DropDownList
												ID="ddlFixedPurchaseIntervalDays"
												DataSource='<%# GetFixedPurchaseIntervalDropdown(Container.ItemIndex, false) %>'
												DataTextField="Text"
												DataValueField="Value"
												SelectedValue='<%#: GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_INTERVAL_DAYS) %>'
												OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged"
												AutoPostBack="true"
												runat="server" />
											日ごとに届ける
											<p class="attention">
												<asp:CustomValidator
													runat="Server"
													ControlToValidate="ddlFixedPurchaseIntervalDays"
													ValidationGroup="OrderShipping"
													ValidateEmptyText="true"
													SetFocusOnError="true" />
											</p>
										</div>
										<asp:HiddenField
											ID="hfFixedPurchaseDaysRequired"
											Value="<%#: this.ShopShippingList[Container.ItemIndex].FixedPurchaseShippingDaysRequired %>"
											runat="server" />
										<asp:HiddenField
											ID="hfFixedPurchaseMinSpan"
											Value="<%#: this.ShopShippingList[Container.ItemIndex].FixedPurchaseMinimumShippingSpan %>"
											runat="server" />
									</dd>
									<dd
										visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY, false) %>"
										runat="server">
										<asp:RadioButton
											ID="rbFixedPurchaseEveryNWeek"
											Text="週間隔・曜日指定"
											Checked="<%# GetFixedPurchaseKbnInputChecked(Container.ItemIndex, 4) %>"
											GroupName="FixedPurchaseShippingPattern"
											OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged"
											AutoPostBack="true"
											runat="server" />
										<div
											id="ddFixedPurchaseEveryNWeek"
											visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY, true) %>"
											runat="server">
											<asp:DropDownList
												ID="ddlFixedPurchaseEveryNWeek_Week"
												DataSource="<%# GetFixedPurchaseEveryNWeekDropdown(Container.ItemIndex, true) %>"
												DataTextField="Text"
												DataValueField="Value"
												SelectedValue='<%#: GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_WEEK) %>'
												OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged"
												AutoPostBack="true"
												runat="server" />
											週間ごと
											<asp:DropDownList
												ID="ddlFixedPurchaseEveryNWeek_DayOfWeek"
												DataSource="<%# GetFixedPurchaseEveryNWeekDropdown(Container.ItemIndex, false) %>"
												DataTextField="Text"
												DataValueField="Value"
												SelectedValue='<%#: GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_DAY_OF_WEEK) %>'
												OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged"
												AutoPostBack="true"
												runat="server" />
											に届ける
										</div>
										<p class="attention">
											<asp:CustomValidator
												runat="Server"
												ControlToValidate="ddlFixedPurchaseEveryNWeek_Week"
												ValidationGroup="OrderShipping"
												ValidateEmptyText="true"
												SetFocusOnError="true" />
											<asp:CustomValidator
												runat="Server"
												ControlToValidate="ddlFixedPurchaseEveryNWeek_DayOfWeek"
												ValidationGroup="OrderShipping"
												ValidateEmptyText="true"
												SetFocusOnError="true" />
										</p>
									</dd>
								</dl>
							</dd>
						</dl>
					</div>
				</div>
				<%-- ▲定期配送情報▲ --%>
				<!-- レコメンド設定 -->
				<uc:BodyRecommend runat="server" cart="<%# GetCartObjectOrderComplete(Container.DataItem) %>" visible="<%# (this.IsOrderCombined == false) && (GetCartPayment(Container.DataItem).IsNotPaymentAmazon) %>" />
				<!-- 定期注文購入金額 -->
				<uc:BodyFixedPurchaseOrderPrice runat="server" cart="<%# GetCartObject(Container.DataItem) %>" visible="<%# GetCartObject(Container.DataItem).HasFixedPurchase %>" />
			</ItemTemplate>
		</asp:Repeater>
		<% if (SessionManager.IsChangedAmazonPayForFixedOrNormal == false) { %>
		<span>以下の内容をご確認のうえ、「注文を確定する」ボタンをクリックしてください。<br /></span>
		<% } %>
		<% if (this.IsDispCorrespondenceSpecifiedCommericalTransactions) { %>
		<dl class="specified-commercial-transactions">
			<dt style="background-color: #ccc; padding: 0.5em; line-height: 1.5">特商法に基づく表記</dt>
			<dd style="padding: 0.5em">
				<%= ShopMessage.GetMessageByPaymentId((this.BindingCartList == null) ? string.Empty : this.BindingCartList.Payment.PaymentId) %>
			</dd>
		</dl>
		<% } %>
		<div class="cart-footer">
			<div class="button-next order">
				<% if (SessionManager.IsChangedAmazonPayForFixedOrNormal) { %>
				<span style="color: red;">カート内の商品が変更されました。<br />
					お手数ですが再度Amazon Payでの購入手続きに進んでください。<br />
					<br />
				</span>
				<div style="text-align: center">
					<div id="AmazonPayCv2Button" style="display: inline-block"></div>
				</div>
				<% } else { %>
				<asp:LinkButton
					ID="lbComplete"
					runat="server"
					OnClick="lbComplete_Click"
					CssClass="btn"
					Text="注文する" />
				<% } %>
			</div>
			<span id="processing2" style="display: none">ただいま決済処理中です。<br />
				画面が切り替わるまでそのままお待ちください。
			</span>
			<span style="display: none;">
				<asp:LinkButton ID="lbCompleteAfterComfirmPayment" runat="server" OnClick="lbComplete_Click" />
			</span>
		</div>
	</div>
</div>
<script>
	function closeModalRecommend() {
		$('#modalRecommend').css("display", "none");
		return false;
	}
</script>
