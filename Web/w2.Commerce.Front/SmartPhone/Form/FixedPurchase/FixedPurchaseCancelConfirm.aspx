<%--
=========================================================================================================
  Module      : スマートフォン用定期購入情報解約確認画面(FixedPurchaseCancelConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/FixedPurchase/FixedPurchaseCancelConfirm.aspx.cs" Inherits="Form_FixedPurchase_FixedPurchaseCancelConfirm" Title="定期購入キャンセルの確認" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-user fixed-purchase-detail">
<div class="user-unit">
	<h2>定期購入情報詳細</h2>
	<div class="content">

	<%-- 解約理由 --%>
	<% if (this.IsCancel) { %>
	<% if (this.IsSetCancelReasonInput) { %>
	<h3>購入解約</h3>
		<dl class="user-form">
			<% if (this.IsSetCancelReasonInput) { %>
			<dt>解約理由</dt>
			<dd>
				<%# WebSanitizer.HtmlEncode(this.CancelReasonInput.CancelReasonName)%>
			</dd>
			<% } %>
			<dt>解約メモ</dt>
			<dd>
				<%# WebSanitizer.HtmlEncodeChangeToBr(this.CancelReasonInput.CancelMemo)%>
			</dd>
		</dl>
	<% } else { %>
	<h3>定期購入解約</h3>
	<% } %>
		<p style = "margin-top:10px;padding-bottom:20px;color:#ff0000;font-weight:bold;">
			まだ解約されていません。「解約する」ボタンで解約が完了します。
			<span runat="server" Visible='<%# this.IsInvalidResumePaypay %>'>
				<br />
				<asp:Literal ID="lCancelPaypayNotification" runat="server"/>
			</span>
		</p>
	<% } %>

	<%-- 休止理由 --%>
	<% if (this.IsSuspend) { %>
	<h3>購入休止</h3>
	<dl class="user-form">
		<dt>再開予定日</dt>
		<dd>
			<%#: (string.IsNullOrEmpty(this.SuspendReasonInput.ResumeDate) == false)
				? DateTimeUtility.ToStringFromRegion(this.SuspendReasonInput.ResumeDate, DateTimeUtility.FormatType.ShortDateWeekOfDay1Letter)
				: "指定なし" %>
		</dd>
		<% if (string.IsNullOrEmpty(this.SuspendReasonInput.NextShippingDate) == false) { %>
			<dt>次回配送日</dt>
			<dd>
				<%#: DateTimeUtility.ToStringFromRegion(this.SuspendReasonInput.NextShippingDate, DateTimeUtility.FormatType.ShortDateWeekOfDay1Letter) %>
			</dd>
		<% } %>
		<dt>休止理由</dt>
		<dd>
			<%# WebSanitizer.HtmlEncodeChangeToBr(this.SuspendReasonInput.SuspendReason) %>
		</dd>
	</dl>
	<p style = "margin-top:10px;padding-bottom:20px;color:#ff0000;font-weight:bold;">上記の内容で更新します。「休止する」ボタンで休止が完了します。</p>
	<% } %>

	<%-- 定期購入情報 --%>
	<h3>定期購入情報</h3>
	<dl class="user-form">
		<dt>定期購入ID</dt>
		<dd>
			<%: this.FixedPurchaseContainer.FixedPurchaseId%>
		</dd>
		<dt>定期購入設定</dt>
		<dd>
			<%: OrderCommon.CreateFixedPurchaseSettingMessage(this.FixedPurchaseContainer)%>
		</dd>
		<dt>最終購入日</dt>
		<dd>
			<%: DateTimeUtility.ToStringFromRegion(this.FixedPurchaseContainer.LastOrderDate, DateTimeUtility.FormatType.ShortDateWeekOfDay1Letter)%>
		</dd>
		<dt>購入回数</dt>
		<dd>
			<%: this.FixedPurchaseContainer.OrderCount%>
		</dd>
		<dt>お支払い方法</dt>
		<dd>
			<%: this.Payment.PaymentName %>
			<% if (string.IsNullOrEmpty(this.FixedPurchaseContainer.CardInstallmentsCode) == false) { %>
			（<%: ValueText.GetValueText(Constants.TABLE_ORDER, OrderCommon.CreditInstallmentsValueTextFieldName, this.FixedPurchaseContainer.CardInstallmentsCode)%>払い）
			<% } %>
		</dd>
		<dt>定期購入ステータス</dt>
		<dd>
			<span id="spFixedPurchaseStatus" runat="server">
			<%: this.FixedPurchaseContainer.FixedPurchaseStatusText%>
			</span>
		</dd>
		<dt>決済ステータス</dt>
		<dd>
			<span id="spPaymentStatus" runat="server"><%: this.FixedPurchaseContainer.PaymentStatusText%></span>
		</dd>
		<% if (this.FixedPurchaseContainer.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) {%>
		<dt>ご利用カード情報</dt>
		<dd>
			<% if ((this.FixedPurchaseContainer.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) && (this.UserCreditCardInfo != null)) { %>
			<div>
				<% if (this.UserCreditCardInfo.DispFlg == Constants.FLG_USERCREDITCARD_DISP_FLG_ON) { %>
				登録名: <%: WebSanitizer.HtmlEncode(this.UserCreditCardInfo.CardDispName) %><%: this.UserCreditCardInfo.DispFlag ? "" : " (削除済)" %>
				<br />
				<% } %>
				<%if (OrderCommon.CreditCompanySelectable && (this.UserCreditCardInfo.CompanyName != string.Empty)) {%>
				カード会社: <%: WebSanitizer.HtmlEncode(this.UserCreditCardInfo.CompanyName) %>
				<br />
				<% } %>
				カード番号: XXXXXXXXXXXX<%: WebSanitizer.HtmlEncode(this.UserCreditCardInfo.LastFourDigit) %>
				<br />
				有効期限: <%: WebSanitizer.HtmlEncode(this.UserCreditCardInfo.ExpirationMonth) + "/" + WebSanitizer.HtmlEncode(this.UserCreditCardInfo.ExpirationYear) + " (月/年)" %>
				<br />
				カード名義人: <%: WebSanitizer.HtmlEncode(this.UserCreditCardInfo.AuthorName) %>
			</div>
			<% } %>
		</dd>
		<% } %>
		<dt>次回配送日</dt>
		<dd>
			<span id="spNextShippingDate" runat="server">
			<%: this.CanCancelFixedPurchase ? DateTimeUtility.ToStringFromRegion(this.FixedPurchaseContainer.NextShippingDate, DateTimeUtility.FormatType.ShortDateWeekOfDay1Letter) : "-"%>
			</span>
		</dd>
		<dt>次々回配送日</dt>
		<dd>
			<span id="spNextNextShippingDate" runat="server">
			<%: this.CanCancelFixedPurchase ? DateTimeUtility.ToStringFromRegion(this.FixedPurchaseContainer.NextNextShippingDate, DateTimeUtility.FormatType.ShortDateWeekOfDay1Letter) : "-"%>
			</span>
		</dd>
	</dl>

	<h3>お届け先情報</h3>
	<dl class="user-form">
		<dt>
			<%: ReplaceTag("@@User.addr.name@@") %>
		</dt>
		<dd>
			<%= IsCountryJp(this.FixedPurchaseShippingContainer.ShippingCountryIsoCode)
				? "〒" + WebSanitizer.HtmlEncode(this.FixedPurchaseShippingContainer.ShippingZip) + "<br />"
				: "" %>
			<%: this.FixedPurchaseShippingContainer.ShippingAddr1%>
			<%: this.FixedPurchaseShippingContainer.ShippingAddr2%><br />
			<%: this.FixedPurchaseShippingContainer.ShippingAddr3%>
			<%: this.FixedPurchaseShippingContainer.ShippingAddr4%><br />
			<%: this.FixedPurchaseShippingContainer.ShippingAddr5%>
			<%: (IsCountryJp(this.FixedPurchaseShippingContainer.ShippingCountryIsoCode) == false)
				? this.FixedPurchaseShippingContainer.ShippingZip
				: "" %>
			<%: this.FixedPurchaseShippingContainer.ShippingCountryName%>
		</dd>
		<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
		<dt><%: ReplaceTag("@@User.company_name.name@@") %>/<%: ReplaceTag("@@User.company_post_name.name@@") %></dt>
		<dd>
			<%: this.FixedPurchaseShippingContainer.ShippingCompanyName%>/<%: this.FixedPurchaseShippingContainer.ShippingCompanyPostName%>
		</dd>
		<% } %>
		<dt><%: ReplaceTag("@@User.name.name@@") %></dt>
		<dd>
			<%: this.FixedPurchaseShippingContainer.ShippingName1%><%: this.FixedPurchaseShippingContainer.ShippingName2%>&nbsp;様<br />
			<% if (IsCountryJp(this.FixedPurchaseShippingContainer.ShippingCountryIsoCode)) { %>
			（<%: this.FixedPurchaseShippingContainer.ShippingNameKana1%><%: this.FixedPurchaseShippingContainer.ShippingNameKana2%>&nbsp;さま）
			<% } %>
		</dd>
		<dt><%: ReplaceTag("@@User.tel1.name@@") %></dt>
		<dd>
			<%: this.FixedPurchaseShippingContainer.ShippingTel1%>
		</dd>
		<% if (this.DeliveryCompany.IsValidShippingTimeSetFlg) { %>
		<dt>配送希望時間帯</dt>
		<dd>
			<%: this.DeliveryCompany.GetShippingTimeMessage(this.FixedPurchaseShippingContainer.ShippingTime) != "" ? this.DeliveryCompany.GetShippingTimeMessage(this.FixedPurchaseShippingContainer.ShippingTime) : ReplaceTag("@@DispText.shipping_time_list.none@@") %>
		</dd>
		<% } %>
	</dl>

	<%-- 購入商品一覧 --%>
	<h3>お届け商品</h3>
	<asp:Repeater ID="rItem" runat="server">
	<HeaderTemplate>
		<dl class="user-form">
	</HeaderTemplate>
	<ItemTemplate>
		<dd>
			<ul>
				<li>
					<%# WebSanitizer.HtmlEncode(((FixedPurchaseItemInput)Container.DataItem).CreateProductJointName()) %>
					[<span class="productId"><%# WebSanitizer.HtmlEncode((((FixedPurchaseItemInput)Container.DataItem).ProductId == ((FixedPurchaseItemInput)Container.DataItem).VariationId) ? ((FixedPurchaseItemInput)Container.DataItem).ProductId : ((FixedPurchaseItemInput)Container.DataItem).VariationId)%></span>]
					<span id="Span1" visible='<%# ((FixedPurchaseItemInput)Container.DataItem).ProductOptionTexts != "" %>' runat="server">
					<br />
					<%# WebSanitizer.HtmlEncode(((FixedPurchaseItemInput)Container.DataItem).GetDisplayProductOptionTexts()).Replace("　", "<br />")%>
					</span>
				</li>
				<li>
					<%#: CurrencyManager.ToPrice(((FixedPurchaseItemInput)Container.DataItem).ProductPriceIncludedOptionPrice) %>（<%#: this.ProductPriceTextPrefix %>）&nbsp;x&nbsp;
					<%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(((FixedPurchaseItemInput)Container.DataItem).ItemQuantity))%>&nbsp;=&nbsp;
					<%#: CurrencyManager.ToPrice(((FixedPurchaseItemInput)Container.DataItem).GetItemPrice() + ((FixedPurchaseItemInput)Container.DataItem).GetItemOptionPrice()) %>
				</li>
			</ul>
		</dd>
	</ItemTemplate>
	<FooterTemplate>
		</dl>
	</FooterTemplate>
	</asp:Repeater>
	</div>

	<div class="user-footer">
		<div class="button-next">
			<asp:LinkButton id="lbUpdate" Text="  解約する  " runat="server" CssClass="btn" OnClick="lbUpdate_Click" Visible="<%# this.IsCancel %>" />
			<asp:LinkButton id="lbSuspend" Text="  休止する  " runat="server" CssClass="btn" OnClick="lbSuspend_Click" Visible="<%# this.IsSuspend %>" />
		</div>
		<div class="button-next">
			<a class="btn" href="javascript:history.back();">  戻る  </a>
		</div>
	</div>
</div>
</section>
</asp:Content>
