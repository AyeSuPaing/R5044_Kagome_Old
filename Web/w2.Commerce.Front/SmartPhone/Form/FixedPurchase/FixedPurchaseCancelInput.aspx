<%--
=========================================================================================================
  Module      : スマートフォン用定期購入情報解約入力画面(FixedPurchaseCancelInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/FixedPurchase/FixedPurchaseCancelInput.aspx.cs" Inherits="Form_FixedPurchase_FixedPurchaseCancelInput" Title="定期購入キャンセルの入力" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-user fixed-purchase-detail">
<div class="user-unit">
	<h2>定期購入情報詳細</h2>
	<div class="content">

	<%-- 解約理由 --%>
	<% if (this.IsCancel) { %>
	<h3>購入解約</h3>
	<dl class="user-form">
		<% if (ddlCancelReason.Items.Count != 0) { %>
		<dt>解約理由<span class="require">※</span></dt>
		<dd>
			<asp:DropDownList ID="ddlCancelReason" runat="server" Width="100%"></asp:DropDownList>
			<asp:CustomValidator ID="cvCancelReason" runat="Server"
								ControlToValidate="ddlCancelReason"
								ValidationGroup="FixedPurchaseModifyInput"
								ValidateEmptyText="true"
								SetFocusOnError="true"
								ClientValidationFunction="ClientValidate"
								EnableClientScript="false"
								CssClass="error_inline" />
		</dd>
		<% } %>
		<dt>解約メモ<span class="require">※</span></dt>
		<dd>
			<asp:TextBox ID="tbCancelMemo" runat="server" TextMode="MultiLine" Rows="5" CssClass="inquirytext" Text="" Width="100%"></asp:TextBox>
			<asp:CustomValidator ID="cvCancelMemo" runat="Server"
								ControlToValidate="tbCancelMemo"
								ValidationGroup="FixedPurchaseModifyInput"
								ValidateEmptyText="true"
								SetFocusOnError="true"
								ClientValidationFunction="ClientValidate"
								EnableClientScript="false"
								CssClass="error_inline" />
		</dd>
	</dl>
	<br/>
	<div class="user-footer">
		<div class="button-next">
			<asp:LinkButton id="lbConfirm" Text="  確認する  " runat="server" CssClass="btn" ValidationGroup="FixedPurchaseModifyInput" OnClick="lbConfirm_Click" />
		</div>
		<div class="button-next">
			<a class="btn" href="javascript:history.back();">  戻る  </a>
		</div>
	</div>
	<% } %>

	<%-- 一時休止理由 --%>
	<% if (this.IsSuspend) { %>
	<h3>購入一時休止</h3>
	<dl class="user-form">
		<dt>再開予定日</dt>
		<dd>
			<asp:DropDownList ID="ddlResumeDateYear" runat="server" CssClass="year" ></asp:DropDownList>&nbsp;&nbsp;/&nbsp;
			<asp:DropDownList ID="ddlResumeDateMonth" runat="server" CssClass="month"></asp:DropDownList>&nbsp;&nbsp;/&nbsp;
			<asp:DropDownList ID="ddlResumeDateDay" runat="server" CssClass="date"></asp:DropDownList>
			<p>※日付を指定しない場合は、無期限で登録します。</p>
			<asp:CustomValidator id="cvResumeDateYear" runat="Server"
				ControlToValidate="ddlResumeDateYear"
				ValidationGroup="FixedPurchaseModifyInput"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				EnableClientScript="false"
				CssClass="error_inline" />
			<asp:CustomValidator id="cvResumeDateMonth" runat="Server"
				ControlToValidate="ddlResumeDateMonth"
				ValidationGroup="FixedPurchaseModifyInput"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				EnableClientScript="false"
				CssClass="error_inline" />
			<asp:CustomValidator id="cvResumeDateDay" runat="Server"
				ControlToValidate="ddlResumeDateDay"
				ValidationGroup="FixedPurchaseModifyInput"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				EnableClientScript="false"
				CssClass="error_inline" />
		</dd>
		<dt>次回配送日</dt>
		<dd>
			<asp:DropDownList ID="ddlNextShippingDateYear" runat="server" CssClass="year"></asp:DropDownList>&nbsp;&nbsp;/&nbsp;
			<asp:DropDownList ID="ddlNextShippingDateMonth" runat="server" CssClass="month"></asp:DropDownList>&nbsp;&nbsp;/&nbsp;
			<asp:DropDownList ID="ddlNextShippingDateDay" runat="server" CssClass="date"></asp:DropDownList>
			<p>※日付を指定しない場合は、更新しません。</p>
			<p>※「再開予定日」以降に定期注文の出荷準備を進めてまいります。
			その為、設定によっては「次回配送日」の指定日にお届けできない場合もございますので、予めご了承ください。</p>
			<asp:CustomValidator id="cvNextShippingDateYear" runat="Server"
				ControlToValidate="ddlNextShippingDateYear"
				ValidationGroup="FixedPurchaseModifyInput"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				EnableClientScript="false"
				CssClass="error_inline" />
			<asp:CustomValidator id="cvNextShippingDateMonth" runat="Server"
				ControlToValidate="ddlNextShippingDateMonth"
				ValidationGroup="FixedPurchaseModifyInput"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				EnableClientScript="false"
				CssClass="error_inline" />
			<asp:CustomValidator id="cvNextShippingDateDay" runat="Server"
				ControlToValidate="ddlNextShippingDateDay"
				ValidationGroup="FixedPurchaseModifyInput"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				EnableClientScript="false"
				CssClass="error_inline" />
		</dd>
		<dt>休止理由<span class="require">※</span></dt>
		<dd>
			<asp:TextBox ID="tbSuspendReason" runat="server" TextMode="MultiLine" Rows="5" CssClass="inquirytext" Text="" Width="100%"></asp:TextBox>
			<asp:CustomValidator ID="cvSuspendReason" runat="Server"
				ControlToValidate="tbSuspendReason"
				ValidationGroup="FixedPurchaseModifyInput"
				ValidateEmptyText="true"
				SetFocusOnError="true"
				ClientValidationFunction="ClientValidate"
				EnableClientScript="false"
				CssClass="error_inline" />
		</dd>
	</dl>
	<br/>
	<div class="user-footer">
		<div class="button-next">
			<asp:LinkButton id="lbSuspendConfirm" Text="  確認する  " runat="server" CssClass="btn" ValidationGroup="FixedPurchaseModifyInput" OnClick="lbSuspendConfirm_Click" />
		</div>
		<div class="button-next">
			<a class="btn" href="javascript:history.back();">  戻る  </a>
		</div>
	</div>
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
			<br />
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
		<%} %>
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
					<span visible='<%# ((FixedPurchaseItemInput)Container.DataItem).ProductOptionTexts != "" %>' runat="server">
					<br />
					<%# WebSanitizer.HtmlEncode(((FixedPurchaseItemInput)Container.DataItem).GetDisplayProductOptionTexts()).Replace("　", "<br />")%>
					</span>
				</li>
				<li>
					<%#: CurrencyManager.ToPrice(((FixedPurchaseItemInput)Container.DataItem).ProductPriceIncludedOptionPrice) %>（<%#: this.ProductPriceTextPrefix %>）&nbsp;x&nbsp;
					<%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(((FixedPurchaseItemInput)Container.DataItem).ItemQuantity))%>&nbsp;=&nbsp;
					<%#: CurrencyManager.ToPrice(((FixedPurchaseItemInput)Container.DataItem).ItemPriceIncludedOptionPrice) %>
				</li>
			</ul>
		</dd>
	</ItemTemplate>
	<FooterTemplate>
		</dl>
	</FooterTemplate>
	</asp:Repeater>
	</div>
</div>
</section>
</asp:Content>