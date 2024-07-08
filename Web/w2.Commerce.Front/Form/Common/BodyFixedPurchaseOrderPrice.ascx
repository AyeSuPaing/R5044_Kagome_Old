<%--
=========================================================================================================
  Module      : 定期注文購入金額出力コントローラ(BodyFixedPurchaseOrderPrice.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BodyFixedPurchaseOrderPrice.ascx.cs" Inherits="Form_Common_BodyFixedPurchaseOrderPrice" %>
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
<div class="subCartList" runat="server" ID="divSubCartList">
<h3>次回以降の注文内容</h3>
<div style="font-size:11px; align-content:center; text-align: left; padding:10px">
	<p>次回以降の定期注文内容は、それぞれ以下のとおりになります。</p>
	<p runat="server" Visible="<%# this.IsFixedPurchaseShippingPatternChanged %>">※次回以降のお届けは下記配送パターンに変更されます。</p>
	<p><%#:GetFixedPurchaseShippingPatternString() %></p>
</div>
<div class="bottom">
<asp:Repeater ID="rFixedPurchaseOrderPriceList" runat="server">
<ItemTemplate>
	<div class="block">
		<h3 class="price"><%#: ((CartObject)Container.DataItem).SubscriptionTimes %>回目の注文内容</h3>
		<asp:Repeater ID="rItems" DataSource="<%# ((CartObject)Container.DataItem).Items %>" runat="server" ItemType="w2.App.Common.Order.CartProduct">
			<ItemTemplate>
				<%-- 商品情報 --%>
				<div class="singleProduct" runat="server" Visible="<%# (Item.IsNovelty == false) %>">
					<dl>
						<dt>
							<a href='<%# WebSanitizer.UrlAttrHtmlEncode(Item.CreateProductDetailUrl()) %>' runat="server" Visible="<%# Item.IsProductDetailLinkValid() %>">
								<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" />
							</a>
							<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" Visible="<%# (Item.IsProductDetailLinkValid() == false) %>" />
						</dt>
						<dd>
							<strong>
								<a href='<%# WebSanitizer.UrlAttrHtmlEncode(Item.CreateProductDetailUrl()) %>' runat="server" Visible="<%# Item.IsProductDetailLinkValid() %>">
									<%#: Item.ProductJointName %>
								</a>
								<%#: Item.ProductJoinNameIfProductDetailLinkIsInvalid %>
							</strong>
							<p visible='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList.IsSelectedProductOptionValueAll %>' runat="server">
								<asp:Repeater ID="rProductOptionSettings" DataSource='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList %>' runat="server">
									<ItemTemplate>
										<strong Visible="<%# (string.IsNullOrEmpty(((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue()) == false) %>" runat="server">
											<%#: ((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue() %>
										</strong>
									</ItemTemplate>
								</asp:Repeater>
							</p>
							<p>数量：&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<%#: Item.CountSingle %></p>
							<% if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false) { %>
								<p Visible="<%# ((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false %>" runat="server"><%#: CurrencyManager.ToPrice(Item.Price) %> (<%#: this.ProductPriceTextPrefix %>)</p>
							<% } else { %>
								<p Visible="<%# (((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false) && (Item.ProductOptionSettingList != null) %>" runat="server">
									<%#: ProductOptionSettingHelper.ToDisplayProductOptionPrice(Item) %>
								</p>
							<% } %>
						</dd>
					</dl>
				</div>
			</ItemTemplate>
		</asp:Repeater>
		<dl class="bgc">
			<dt>配送予定日：<%#:DateTimeUtility.ToStringFromRegion(GetShippingDate(((CartObject)Container.DataItem).SubscriptionTimes - 2), DateTimeUtility.FormatType.LongDateWeekOfDay1Letter) %></dt>
		</dl>
		<div class="fixedPurchasePriceList">
			<dl class="bgc">
				<dt>小計（<%#: this.ProductPriceTextPrefix %>）</dt>
				<dd><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceSubtotal) %></dd>
			</dl>
			<%if (this.ProductIncludedTaxFlg == false) { %>
				<dl class="bgc">
					<dt>消費税額</dt>
					<dd><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceSubtotalTax) %></dd>
				</dl>
			<%} %>
			<dl>
				<dt>各種割引額</dt>
				<dd class='<%#: (((CartObject)Container.DataItem).DiscountPriceForFixedPurchase) > 0 ? "minus" : "" %>'><%#: (((CartObject)Container.DataItem).DiscountPriceForFixedPurchase) > 0 ? "-" : "" %><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).DiscountPriceForFixedPurchase) %></dd>
			</dl>
			<dl>
				<dt>配送料・決済手数料</dt>
				<dd><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceShipping + ((CartObject)Container.DataItem).Payment.PriceExchange) %></dd>
			</dl>
			<dl class="result">
				<dt>合計（税込）</dt>
				<dd><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceTotal)%></dd>
			</dl>
		</div>
		<div Visible="<%# ((CartObject)Container.DataItem).SubscriptionTimes != ((CartObject)Container.DataItem).DuplicatedSubscriptionTimesTo %>" runat ="server">
			<div Visible="<%# ((((CartObject)Container.DataItem).SubscriptionTimes + 1) != ((CartObject)Container.DataItem).DuplicatedSubscriptionTimesTo) && (this.IsIndefinitePeriod == false) %>"  runat ="server">
				※<%# ((CartObject)Container.DataItem).SubscriptionTimes + 1 %>～<%# ((CartObject)Container.DataItem).DuplicatedSubscriptionTimesTo %>回目までは<%# ((CartObject)Container.DataItem).SubscriptionTimes %>回目と同じ注文内容でお届けします。
			</div>
			<div Visible="<%# ((((CartObject)Container.DataItem).SubscriptionTimes + 1) == ((CartObject)Container.DataItem).DuplicatedSubscriptionTimesTo) && (this.IsIndefinitePeriod == false) %>" runat ="server">
				※<%# ((CartObject)Container.DataItem).SubscriptionTimes + 1 %>回目は<%# ((CartObject)Container.DataItem).SubscriptionTimes %>回目と同じ注文内容でお届けします。
			</div>
			<div id="divIsIndefinitePeriodMessage" Visible="<%# this.IsIndefinitePeriod %>" runat ="server">
				※<%# ((CartObject)Container.DataItem).SubscriptionTimes + 1 %>回目以降は<%# ((CartObject)Container.DataItem).SubscriptionTimes %>回目と同じ注文内容でお届けします。
			</div>
		</div>
	</div>
</ItemTemplate>
</asp:Repeater>
<div class="block">
<h3 class="price"><%: this.TotalDisplayMaxCount  + 1 %>回購入した場合の総額</h3>
<div class="fixedPurchasePriceList">
	<dl class="result">
		<dt>合計（税込）</dt>
		<dd><%: CurrencyManager.ToPrice(this.SummaryOfFixedPurchasePriceTotal) %></dd>
	</dl>
</div>
	
	<div id="divSubscriptionBoxAutomaticReset" runat="server">
		<% if (this.TotalDisplayMaxCount == 0) { %>
			2回目以降、1回目の注文内容を繰り返しお届けします。
		<% } else if (this.IsIndefinitePeriod && this.SubscriptionTimes == this.DuplicatedSubscriptionTimesTo) { %>
			<%: this.TotalDisplayMaxCount + 2 %>回目以降、解約されるまで<%: this.TotalDisplayMaxCount + 1 %>回目の注文内容を繰り返しお届けします。
		<% } else if (this.IsIndefinitePeriod == false) { %>
			<%: this.TotalDisplayMaxCount + 2 %>回目以降、1回目から<%: this.TotalDisplayMaxCount + 1 %>回目までの注文内容を繰り返しお届けします。
		<% } %>
	</div>
</div>
<div class="message">
	<p>※各回の注文には、配送料金<%: CurrencyManager.ToPrice(this.FixedPurchasePriceShipping) %>、決済手数料<%: CurrencyManager.ToPrice(this.FixedPurchasePriceExchange) %>がかかります。</p>
	<p>※各種割引額には、セットプロモーション割引額、会員ランク割引額、定期購入割引額が含まれています。</p>
	<p>※各種金額は注文時点に適用されている金額のため、実際には異なることがあります。</p>
	<p>※この定期注文は出荷回数が<%: this.CancelableCount %>回以上から定期購入キャンセル、一時休止、スキップが可能になります。</p>
	<% if (this.UseAllPointFlg) { %>
		<p>※各回の注文には、利用可能ポイント分の割引が適用されます。</p>
	<% } %>
</div>
</div>
</div>
<%-- △編集可能領域：コンテンツ△ --%>
