<%--
=========================================================================================================
  Module      : 定期注文購入金額出力コントローラ(BodyFixedPurchaseOrderPrice.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/BodyFixedPurchaseOrderPrice.ascx.cs" Inherits="Form_Common_BodyFixedPurchaseOrderPrice" %>
<script runat="server">
public new void Page_Init(Object sender, EventArgs e)
{
	<%-- ▽編集可能領域：プロパティ設定▽ --%>
	// 定期購入回数最大表示数(何回分の注文金額情報を表示させるか)
	this.DisplayMaxCount = 3;
	<%-- △編集可能領域△ --%>
}
</script>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<div class="order-unit comfirm-payment" runat="server" ID="divSubCartList">
	<dl class="order-form">
		<dt>次回以降の注文内容</dt>
		<dd class="message">次回以降の定期注文内容は、それぞれ以下のとおりになります。</dd>
		<dd class="message" runat="server" Visible="<%# this.IsFixedPurchaseShippingPatternChanged %>">※次回以降のお届けは下記配送パターンに変更されます。</dd>
		<dd class="message"><%#: GetFixedPurchaseShippingPatternString() %></dd>
	</dl>
	<asp:Repeater ID="rFixedPurchaseOrderPriceList" runat="server">
	<ItemTemplate>
		<dl class="order-form">
			<dt><%#: Container.ItemIndex + 2 %>回目の注文内容</dt>
			
				<%-- 商品情報 --%>
				<asp:Repeater id="rItems" runat="server" DataSource='<%# (CartObject)Container.DataItem %>' ItemType="w2.App.Common.Order.CartProduct">
					<HeaderTemplate>
						<table class="cart-table">
							<tbody>
					</HeaderTemplate>
					<ItemTemplate>
						<tr class="cart-unit-product" runat="server" Visible="<%# (Item.IsNovelty == false) %>">
							<td class="product-image">
								<a href='<%# WebSanitizer.UrlAttrHtmlEncode(Item.CreateProductDetailUrl()) %>' runat="server" Visible="<%# Item.IsProductDetailLinkValid() %>">
									<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" />
								</a>
								<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" Visible="<%# (Item.IsProductDetailLinkValid() == false) %>" />
							</td>
							<td class="product-info">
								<ul>
									<li class="product-name">
										<a href='<%# WebSanitizer.UrlAttrHtmlEncode(Item.CreateProductDetailUrl()) %>' runat="server" Visible="<%# Item.IsProductDetailLinkValid() %>">
											<%#: Item.ProductJointName %>
										</a>
										<%#: Item.ProductJoinNameIfProductDetailLinkIsInvalid %>
									</li>
									<asp:Repeater ID="rProductOptionSettings" DataSource='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList %>' runat="server">
										<ItemTemplate>
											<%#: ((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue() %>
											<%# string.IsNullOrEmpty(((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue()) != false ? "<br />" : string.Empty %>
										</ItemTemplate>
									</asp:Repeater>
									<% if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false){ %>
									<li class="product-price" Visible="<%# (((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false) %>" runat="server">
										<%#: CurrencyManager.ToPrice(Item.Price) %> (<%#: this.ProductPriceTextPrefix %>)
									</li>
									<% } else { %>
									<li class="product-price" Visible="<%# (((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false && Item.ProductOptionSettingList != null) %>" runat="server">
										<%#: ProductOptionSettingHelper.ToDisplayProductOptionPrice(Item) %>
									</li>
									<% } %>
								</ul>
							</td>
							<td class="product-control">
								<div class="amout">
									数量：<%#: Item.CountSingle %>
								</div>
							</td>
						</tr>
					</ItemTemplate>
					<FooterTemplate>
							</tbody>
						</table>
					</FooterTemplate>
				</asp:Repeater>
				<dl>
					<dd>配送予定日：<%#:DateTimeUtility.ToStringFromRegion(GetShippingDate(Container.ItemIndex), DateTimeUtility.FormatType.LongDateWeekOfDay1Letter) %></dd>
					<dt>小計（<%#: this.ProductPriceTextPrefix %>）</dt>
					<dd><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceSubtotal) %></dd>
					<dt>各種割引額</dt>
					<dd><%#: ((CartObject)Container.DataItem).DiscountPriceForFixedPurchase > 0 ? "-" : "" %><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).DiscountPriceForFixedPurchase) %></dd>
					<dt>配送料・決済手数料</dt>
					<dd><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceShipping + ((CartObject)Container.DataItem).Payment.PriceExchange) %></dd>
					<%if (this.ProductIncludedTaxFlg == false) { %>
						<dt>消費税</dt>
						<dd><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceTax) %></dd>
					<%} %>
					<dt>合計（税込）</dt>
					<dd><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceTotal) %></dd>
				</dl>
				<dl id="Div1" runat ="server" Visible="<%# ((CartObject)Container.DataItem).SubscriptionTimes != ((CartObject)Container.DataItem).DuplicatedSubscriptionTimesTo %>">
					<div runat ="server" Visible="<%# (((CartObject)Container.DataItem).SubscriptionTimes + 1 != ((CartObject)Container.DataItem).DuplicatedSubscriptionTimesTo) && (this.IsIndefinitePeriod == false) %>">
						※<%# ((CartObject)Container.DataItem).SubscriptionTimes + 1 %>～<%# ((CartObject)Container.DataItem).DuplicatedSubscriptionTimesTo %>回目までは<%# ((CartObject)Container.DataItem).SubscriptionTimes %>回目と同じ注文内容でお届けします。
					</div>
					<div runat ="server" Visible="<%# (((CartObject)Container.DataItem).SubscriptionTimes + 1 == ((CartObject)Container.DataItem).DuplicatedSubscriptionTimesTo) && (this.IsIndefinitePeriod == false) %>">
						※<%# ((CartObject)Container.DataItem).SubscriptionTimes + 1 %>回目は<%# ((CartObject)Container.DataItem).SubscriptionTimes %>回目と同じ注文内容でお届けします。
					</div>
					<div id="divIsIndefinitePeriodMessage" runat ="server" Visible="<%# this.IsIndefinitePeriod %>">
						※<%# ((CartObject)Container.DataItem).SubscriptionTimes + 1 %>回目以降は<%# ((CartObject)Container.DataItem).SubscriptionTimes %>回目と同じ注文内容でお届けします。
					</div>
				</dl>
			</dd>
		</dl>
	</ItemTemplate>
	</asp:Repeater>
	<dl class="order-form">
		<dt><%: this.TotalDisplayMaxCount + 1 %>回購入した場合の総額</dt>
		<dd>
			<dl>
				<dt>合計（税込）</dt>
				<dd><%: CurrencyManager.ToPrice(this.SummaryOfFixedPurchasePriceTotal) %></dd>
			</dl>
		</dd>
		<dd id="divSubscriptionBoxAutomaticReset" runat="server">
			<% if (this.TotalDisplayMaxCount == 0) { %>
				2回目以降、1回目の注文内容を繰り返しお届けします。
			<% } else if (this.IsIndefinitePeriod && (this.SubscriptionTimes == this.DuplicatedSubscriptionTimesTo)) { %>
				<%: this.TotalDisplayMaxCount + 2 %>回目以降、解約されるまで<%: this.TotalDisplayMaxCount + 1 %>回目の注文内容を繰り返しお届けします。
			<% } else if (this.IsIndefinitePeriod == false) { %>
				<%: this.TotalDisplayMaxCount + 2 %>回目以降、1回目から<%: this.TotalDisplayMaxCount + 1 %>回目までの注文内容を繰り返しお届けします。
			<% } %>
		</dd>
	</dl>
	<dl class="order-form">
		<dd class="message">※各回の注文には、配送料金<%: CurrencyManager.ToPrice(this.FixedPurchasePriceShipping) %>、決済手数料<%: CurrencyManager.ToPrice(this.FixedPurchasePriceExchange) %>がかかります。</dd>
		<dd class="message">※各種割引額には、セットプロモーション割引額、会員ランク割引額、定期購入割引額が含まれています。</dd>
		<dd class="message">※各種金額は注文時点に適用されている金額のため、実際には異なることがあります。</dd>
		<dd class="message">※この定期注文は出荷回数が<%: this.CancelableCount %>回以上から定期購入キャンセル、一時休止、スキップが可能になります。</dd>
		<% if (this.UseAllPointFlg) { %>
		<dd class="message">※各回の注文には、利用可能ポイント分の割引が適用されます。</dd>
		<% } %>
	</dl>
</div>
<%-- △編集可能領域：コンテンツ△ --%>
