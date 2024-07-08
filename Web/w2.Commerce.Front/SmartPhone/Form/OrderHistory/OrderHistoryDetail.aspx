<%--
=========================================================================================================
  Module      : スマートフォン用注文履歴詳細画面(OrderHistoryDetail.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/OrderHistory/OrderHistoryDetail.aspx.cs" Inherits="Form_Order_OrderHistoryDetail" Title="購入履歴詳細ページ" %>
<%@ Import Namespace="w2.App.Common.AmazonCv2" %>
<%-- ▼削除禁止：クレジットカードTokenコントロール▼ --%>
<%@ Import Namespace="w2.Domain.UserShipping" %>
<%@ Register TagPrefix="uc" TagName="CreditToken" Src="~/Form/Common/CreditToken.ascx" %>
<%-- ▲削除禁止：クレジットカードTokenコントロール▲ --%>
<%@ Register TagPrefix="uc" TagName="Layer" Src="~/SmartPhone/Form/Common/Layer/SearchResultLayer.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionCvsDef" Src="~/Form/Common/Order/PaymentDescriptionCvsDef.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionSmsDef" Src="~/Form/Common/Order/PaymentDescriptionSmsDef.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionPayPal" Src="~/Form/Common/Order/PaymentDescriptionPayPal.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaypalScriptsForm" Src="~/Form/Common/PayPalScriptsForm.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionTriLinkAfterPay" Src="~/Form/Common/Order/PaymentDescriptionTriLinkAfterPay.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaidyCheckoutScript" Src="~/Form/Common/Order/PaidyCheckoutScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaidyCheckoutControl" Src="~/Form/Common/Order/PaidyCheckoutControl.ascx" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.Paidy" %>
<%@ Import Namespace="w2.Common.Helper" %>
<%@ Import Namespace="w2.Domain.Product.Helper" %>
<%@ Register TagPrefix="uc" TagName="AtonePaymentScript" Src="~/Form/Common/AtonePaymentScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="AfteePaymentScript" Src="~/Form/Common/AfteePaymentScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionAtone" Src="~/Form/Common/Order/PaymentDescriptionAtone.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionNPAfterPay" Src="~/Form/Common/Order/PaymentDescriptionNPAfterPay.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionLinePay" Src="~/Form/Common/Order/PaymentDescriptionLinePay.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionPayPay" Src="~/Form/Common/Order/PaymentDescriptionPayPay.ascx" %>
<%@ Register TagPrefix="uc" TagName="EcPayScript" Src="~/Form/Common/ECPay/EcPayScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentYamatoKaSmsAuthModal" Src="~/SmartPhone/Form/Common/Order/PaymentYamatoKaSmsAuthModal.ascx" %>
<%@ Register TagPrefix="uc" TagName="RakutenCreditCard" Src="~/Form/Common/RakutenCreditCardModal.ascx" %>
<%@ Register TagPrefix="uc" TagName="RakutenPaymentScript" Src="~/Form/Common/RakutenPaymentScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="Loading" Src="~/Form/Common/Loading.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaidyCheckoutScriptForPagent" Src="~/Form/Common/Order/PaidyCheckoutScriptForPaygent.ascx" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.Paygent.Paidy.Checkout" %>
<asp:Content ContentPlaceHolderID="head" Runat="Server">
<link href="<%#: Constants.PATH_ROOT + "SmartPhone/Css/order.css" %>" rel="stylesheet" type="text/css" media="all" />
<link href="<%#: Constants.PATH_ROOT + "SmartPhone/Css/user.css" %>" rel="stylesheet" type="text/css" media="all" />
</asp:Content>

<%-- UpdatePanel開始 --%>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<% if(Constants.PAYMENT_ATONEOPTION_ENABLED) { %>
<asp:HiddenField runat="server" ID="hfAtoneToken"/>
<asp:HiddenField ID="hfAtoneTransactionId" runat="server" />
<% } %>
<% if (Constants.PAYMENT_AFTEEOPTION_ENABLED) { %>
<asp:HiddenField runat="server" ID="hfAfteeToken"/>
<asp:HiddenField ID="hfAfteeTransactionId" runat="server" />
<% } %>
<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
<uc:EcPayScript runat="server" ID="ucECPayScript" />
<% } %>
<asp:UpdatePanel ID="upUpdatePanel" UpdateMode="Conditional" runat="server">
<ContentTemplate>

<section class="wrap-order order-history-detail order-payment">

	<div class="order-unit">
	<h2>購入履歴詳細</h2>
	<div class="errorMessage" style="color: red; border-top: none; font-size: 1em; padding: 0 20px 20px;">
		<asp:Literal ID="lPaymentErrorMessage" runat="server" />
	</div>
	<nav class="order-history-nav">
		<ul>
		<li><a href="<%#: Constants.PATH_ROOT + "Form/OrderHistory/OrderHistoryList.aspx?disp=0" %>">注文一覧</a></li>
		<li><a href="<%#: Constants.PATH_ROOT + "Form/OrderHistory/OrderHistoryList.aspx?disp=1" %>">注文商品一覧</a></li>
		</ul>
	</nav>

	<div class="content">
	<%-- 注文情報 --%>
	<h3>ご注文情報</h3>
		<dl class="order-form">
			<dt>ご注文番号</dt>
			<dd><%#: this.OrderModel.OrderId %></dd>
			<% if(this.IsModifyCancel) { %>
				<div class="button">
					<asp:LinkButton ID="lbCancelOrder" Text="注文キャンセル" runat="server" OnClientClick="return confirm('注文をキャンセルします。よろしいですか？');" OnClick="lbCancelOrder_Click" class="btn" />
					<p style="padding-top: 5px; text-align: center;"><%# WebSanitizer.HtmlEncode(this.OrderCancelTimeMessage) %></p>
				</div>
		<%}%>
			<% if (this.IsFixedPurchase) { %>
			<dt>定期購入ID</dt>
			<dd>
				<%#: this.OrderModel.FixedPurchaseId %>
				<div class="button">
					<asp:LinkButton Text="次回以降の注文変更" runat="server" OnClick="lbDisplayFixedPurchaseDetail_Click" class="btn" />
				</div>
			</dd>

			<%}%>
			<dt>購入日</dt>
			<dd><%#: DateTimeUtility.ToStringFromRegion(this.OrderModel.OrderDate, DateTimeUtility.FormatType.ShortDate2Letter) %></dd>
			<dt>ご注文状況</dt>
			<dd>
				<% if (IsPickupRealShop && (this.OrderModel.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP)) { %>
				<%#: (this.OrderModel.StorePickupStatus == Constants.FLG_STOREPICKUP_STATUS_PENDING) ? ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_STATUS, this.OrderModel.OrderStatus) + "(店舗未到着)" : ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_STOREPICKUP_STATUS, this.OrderModel.StorePickupStatus) %>
				<% } else { %>
				<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_STATUS, this.OrderModel.OrderStatus) %><%#: this.OrderModel.ShippedChangedKbn == Constants.FLG_ORDER_SHIPPED_CHANGED_KBN_CHANAGED ? "（変更有り）" : string.Empty %>
				<% } %>
			</dd>
			<% if (Constants.PAYMENT_GMO_POST_ENABLED) { %>
				<!--show credit status if using GMO-->
				<% if ((this.OrderModel.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO) || (this.OrderModel.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)) { %>
					<dt>与信状況</dt>
					<dd><%#:ValueText.GetValueText(Constants.TABLE_ORDER,Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS, this.OrderModel.ExternalPaymentStatus) %></dd>
				<% } %>
			<% } %>
			<dt>ご入金状況</dt>
			<dd><%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, this.OrderModel.OrderPaymentStatus) %></dd>
			<dt>お支払い方法</dt>
			<dd><%#: this.PaymentModel.PaymentName %>
					<% if ((this.OrderModel.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) && (string.IsNullOrEmpty(this.OrderModel.CardInstruments) == false)) { %>
						(<%: this.OrderModel.CardInstruments %>)
					<% } %>
					<% if (this.OrderModel.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT) { %>
						<div style="margin: 10px 0;">
							<small>※現在のAmazon Payでの配送先情報、お支払い方法を表示しています。</small>
						</div>
						<iframe id="AmazonDetailWidget" src="<%: PageUrlCreatorUtility.CreateAmazonPayWidgetUrl(true, orderId: this.OrderModel.OrderId) %>" style="width:100%;border:none;"></iframe>
					<% } %>
				<div class="button" runat="server" Visible ="<%# this.IsDisplayInputOrderPaymentKbn %>">
					<asp:LinkButton ID="lbDisplayInputOrderPaymentKbn"
						Text="お支払方法変更"
						runat="server"
						Visible="<%# this.IsPickupRealShop == false %>"
						OnClick="lbDisplayInputOrderPaymentKbn_Click"
						class="btn"
						Enabled="<%# this.IsDisplayInputOrderPaymentKbn %>"
						AutoPostBack="true" />
				</div>
				<% if (this.CanRequestCvsDefInvoiceReissue) { %>
				<div class="button">
					<asp:LinkButton
						ID="lbRequestCvsDefInvoiceReissue"
						style="margin-top:15px;"
						runat="server"
						Visible="<%# (this.IsOrderCvsDefInvoiceReissueRequested == false) %>"
						OnClick="lbRequestCvsDefInvoiceReissue_Click"
						AutoPostBack="true"
						class="btn" />
					<asp:Literal ID="lCvsDefInvoiceReissueRequestResultMessage" runat="server" />
				</div>
				<% } %>
				<%-- ▼PayPalログインここから▼ --%>
				<div style="display: none">
					<%if (Constants.PAYPAL_LOGINPAYMENT_ENABLED) {%>
						<%
							ucPaypalScriptsForm.LogoDesign = "Payment";
							ucPaypalScriptsForm.AuthCompleteActionControl = lbPayPalAuthComplete;
						%>
						<uc:PaypalScriptsForm ID="ucPaypalScriptsForm" runat="server" />
						<br /><asp:LinkButton ID="lbPayPalAuthComplete" runat="server" OnClick="lbPayPalAuthComplete_Click"></asp:LinkButton>
					<%} %>
				</div>
				<%-- ▲PayPalログインここまで▲ --%>
				<!--Update payment pattern-->
				<asp:HiddenField ID="hfPaidyTokenId" runat="server" />
				<asp:HiddenField ID="hfPaidyPaymentId" runat="server" />
				<asp:HiddenField ID="hfPaidyPaySelected" runat="server" />
				<asp:HiddenField ID="hfPaidyStatus" runat="server" />
				<div id="dvOrderPaymentPattern" Visible="False" runat="server">
					<tr><!--Input payment update form-->
						<th>お支払い情報</th>
						<td id="CartList">
							<div class="orderBox">
								<div class="list">
								<span id="Span1" style="color:red" runat="server" visible="<%# (string.IsNullOrEmpty(StringUtility.ToEmpty(this.DispLimitedPaymentMessages[0])) == false) %>">
									<%# StringUtility.ToEmpty(this.DispLimitedPaymentMessages[0]) %>
									<br/>
								</span>
								<dl class="list">
									<% if(Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_DDL) { %>
									<dl class="order-form payment-list">
										<dt><asp:DropDownList ID="ddlPayment" runat="server" DataSource="<%# this.ValidPayments[0] %>" ItemType="w2.Domain.Payment.PaymentModel" OnSelectedIndexChanged="rbgPayment_OnCheckedChanged" AutoPostBack="true" DataTextField="PaymentName" DataValueField="PaymentId" /></dt>
									</dl>
									<% } %>
									<asp:Repeater ID="rPayment" runat="server" DataSource="<%# this.ValidPayments[0] %>" ItemType="w2.Domain.Payment.PaymentModel" >
										<HeaderTemplate>
											<dl class="order-form payment-list">
										</HeaderTemplate>
										<ItemTemplate>
											<asp:HiddenField ID="hfPaymentId" Value='<%# Item.PaymentId %>' runat="server" />
											<asp:HiddenField ID="hfPaymentName" Value='<%# Item.PaymentName %>' runat="server" />
											<asp:HiddenField ID="hfPaymentPrice" Value="<%# OrderCommon.GetPaymentPrice(Item.ShopId, Item.PaymentId, this.OrderModel.OrderPriceSubtotal, OrderCommon.GetPriceCartTotalWithoutPaymentPrice(this.OrderModel)) %>" runat="server" />
											<% if(Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_RB) { %>
											<dt class="title">
												<w2c:RadioButtonGroup ID="rbgPayment" GroupName='Payment' Checked="<%# this.OrderModel.OrderPaymentKbn == Item.PaymentId %>" Text="<%#: Item.PaymentName %>" OnCheckedChanged="rbgPayment_OnCheckedChanged" AutoPostBack="true" runat="server" />
											</dt>
											<% } %>
											<dd id="ddCredit" class="credit-card" runat="server" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) %>">
												<div class="inner">
												<%-- クレジット --%>
												<div class="box-center" runat="server">
													<asp:DropDownList ID="ddlUserCreditCard" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlUserCreditCard_OnSelectedIndexChanged" DataTextField="text" DataValueField="value" ></asp:DropDownList>
												</div>

												<%-- ▽新規カード▽ --%>
												<% if (IsNewCreditCard()){ %>
													<div id="divCreditCardInputForm" class="new" runat="server" >
													
													<% if (this.IsCreditCardLinkPayment() == false) { %>
													<%--▼▼ クレジット Token保持用 ▼▼--%>
													<asp:HiddenField ID="hfCreditToken" Value="" runat="server" />
													<%--▲▲ クレジット Token保持用 ▲▲--%>

													<%--▼▼ カード情報取得用 ▼▼--%>
													<input type="hidden" id="hidCinfo" name="hidCinfo" value="<%# CreateGetCardInfoJsScriptForCreditToken(Container) %>" />
													<span id="spanErrorMessageForCreditCard" style="color: red; display: none" runat="server"></span>
													<%--▲▲ カード情報取得用 ▲▲--%>
													
													<ul>
														<div id="divRakutenCredit" runat="server">
															<asp:LinkButton id="lbEditCreditCardNoForRakutenToken" CssClass="lbEditCreditCardNoForRakutenToken" OnClick="lbEditCreditCardNoForToken_Click" runat="server">再入力</asp:LinkButton>
															<uc:RakutenCreditCard
																ID="ucRakutenCreditCard"
																IsOrder="true"
																CartIndex="1"
																InstallmentCodeList="<%# this.CreditInstallmentsList %>"
																runat="server" />
														</div>
														<%--▼▼ カード情報入力（トークン未取得・利用なし） ▼▼--%>
														<div id="divCreditCardNoToken" runat="server">
														<%if (OrderCommon.CreditCompanySelectable) {%>
														<li class="card-company">
															<h4>カード会社</h4>
															<div><asp:DropDownList id="ddlCreditCardCompany" runat="server" DataTextField="Text" DataValueField="Value" CssClass="input_widthG input_border"></asp:DropDownList></div>
														</li>
														<% } %>
														<li class="card-nums">
															<p class="attention">
																<asp:CustomValidator ID="cvCreditCardNo1" runat="Server"
																	ControlToValidate="tbCreditCardNo1"
																	ValidationGroup="OrderPayment"
																	ValidateEmptyText="true"
																	SetFocusOnError="true"
																	ClientValidationFunction="ClientValidate"
																	EnableClientScript="false"
																	CssClass="error_inline" />
																<span id="sErrorMessage" runat="server" />
															</p>
															<h4>カード番号<span class="require">※</span></h4>
															<div>
																<w2c:ExtendedTextBox id="tbCreditCardNo1" Type="tel" runat="server" CssClass="tel" MaxLength="16" autocomplete="off"></w2c:ExtendedTextBox>
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
																<asp:DropDownList id="ddlCreditExpireMonth" runat="server" DataSource="<%# this.CreditExpireMonth %>" ></asp:DropDownList>
																	/
																<asp:DropDownList id="ddlCreditExpireYear" runat="server" DataSource="<%# this.CreditExpireYear %>" ></asp:DropDownList>
																	 (月/年)
															</div>
														</li>
														<li class="card-name">
															<h4>カード名義人<span class="require">※</span></h4>
															<div>
																<p class="attention">
																	<asp:CustomValidator ID="cvCreditAuthorName" runat="Server"
																		ControlToValidate="tbCreditAuthorName"
																		ValidationGroup="OrderPayment"
																		ValidateEmptyText="true"
																		SetFocusOnError="true"
																		ClientValidationFunction="ClientValidate"
																		EnableClientScript="false"
																		CssClass="error_inline" />
																</p>
																<w2c:ExtendedTextBox id="tbCreditAuthorName" Type="text" runat="server" MaxLength="50" autocomplete="off"></w2c:ExtendedTextBox>
															<p>例：「TAROU YAMADA」</p>
															</div>
														</li>

														<li class="card-sequrity" visible="<%# OrderCommon.CreditSecurityCodeEnable %>" runat="server">
															<h4>セキュリティコード<span class="require">※</span></h4>
															<div>
																<p class="attention">
																	<asp:CustomValidator ID="cvCreditSecurityCode" runat="Server"
																		ControlToValidate="tbCreditSecurityCode"
																		ValidationGroup="OrderPayment"
																		ValidateEmptyText="true"
																		SetFocusOnError="true"
																		ClientValidationFunction="ClientValidate"
																		EnableClientScript="false"
																		CssClass="error_inline" />
																</p>
																<w2c:ExtendedTextBox id="tbCreditSecurityCode" Type="tel" runat="server" MaxLength="4" autocomplete="off"></w2c:ExtendedTextBox>
																<p>
																	<img src="<%= Constants.PATH_ROOT %>SmartPhone/Contents/ImagePkg/common/card-sequrity-code.gif" alt="セキュリティコードとは" width="280" />
																</p>
															</div>
														</li>
														</div>
														<%--▲▲ カード情報入力（トークン未取得・利用なし） ▲▲--%>

														<%--▼▼ カード情報入力（トークン取得済） ▼▼--%>
														<div id="divCreditCardForTokenAcquired" runat="server">
															<%if (OrderCommon.CreditCompanySelectable) {%>
															<li>
																<h4>カード会社</h4>
															<div><asp:Literal ID="lCreditCardCompanyNameForTokenAcquired" runat="server"></asp:Literal><br /></div>
															</li>
															<%} %>
															<li>
																<h4>カード番号 <asp:LinkButton id="lbEditCreditCardNoForToken" OnClick="lbEditCreditCardNoForToken_Click" runat="server">再入力</asp:LinkButton></h4>
																<div>
																	<p>XXXXXXXXXXXX<asp:Literal ID="lLastFourDigitForTokenAcquired" runat="server"></asp:Literal><br /></p>
																</div>
															</li>
															<li>
																<h4>有効期限</h4>
																<div><asp:Literal ID="lExpirationMonthForTokenAcquired" runat="server"></asp:Literal>
																&nbsp;/&nbsp;
																<asp:Literal ID="lExpirationYearForTokenAcquired" runat="server"></asp:Literal> (月/年)</div>
															</li>
															<li>
																<h4>カード名義人</h4>
																<div><asp:Literal ID="lCreditAuthorNameForTokenAcquired" runat="server"></asp:Literal><br /></div>
															</li>
														</div>
														<%--▲▲ カード情報入力（トークン取得済） ▲▲ --%>

														<li class="card-time" visible="<%# OrderCommon.CreditInstallmentsSelectable && (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Rakuten) %>" runat="server">
															<h4>支払い回数</h4>
															<div>
																<asp:DropDownList id="dllCreditInstallments" runat="server" DataTextField="Text" DataValueField="Value"></asp:DropDownList>
																<p>AMEX/DINERSは一括のみとなります。</p>
															</div>

														</li>
														<% } else { %>
															<div>遷移する外部サイトでカード番号を入力してください。</div>
														<% } %>
														
														</ul>

														<div class="box-center">
														<asp:CheckBox ID="cbRegistCreditCard" runat="server" Text="このカードを登録する" OnCheckedChanged="cbRegistCreditCard_OnCheckedChanged" AutoPostBack="true" />
														</div>

														<div id="divUserCreditCardName" class="card-save" Visible="false" runat="server">
															<p class="msg">クレジットカードを保存する場合は、以下をご入力ください。</p>
															<h4>登録名<span class="require">※</span></h4>
															<asp:TextBox ID="tbUserCreditCardName" MaxLength="100" CssClass="input_widthD input_border" runat="server"></asp:TextBox>
															<div>
																<p class="attention">
																<asp:CustomValidator ID="cvUserCreditCardName" runat="Server"
																	ControlToValidate="tbUserCreditCardName"
																	ValidationGroup="OrderPayment"
																	ValidateEmptyText="true"
																	SetFocusOnError="true"
																	ClientValidationFunction="ClientValidate"
																	EnableClientScript="false"
																	CssClass="error_inline" />
																</p>
															</div>
														</div>

													</div>
													<%-- △新規カード△ --%>

													<%-- ▽登録済みカード▽ --%>
													<% }else{ %>
													<div id="divCreditCardDisp" runat="server">
														<ul>
															<%if (OrderCommon.CreditCompanySelectable) {%>
															<li>
																<h4>カード会社</h4>
																<div>
																	<%: this.CreditCardCompanyName %>
																</div>
															</li>
															<%} %>
															<li>
																<h4>カード番号</h4>
																<div>
																	XXXXXXXXXXXX<%: this.LastFourDigit %>
																</div>
															</li>
															<li>
																<h4>有効期限</h4>
																<div>
																	<%: this.ExpirationMonth %>/<%: this.ExpirationYear %> (月/年)
																</div>
															</li>
															<li>
																<h4>カード名義人</h4>
																<div>
																	<%: this.CreditAuthorName %>
																</div>
															</li>
															<li id="Li3" visible="<%# OrderCommon.CreditInstallmentsSelectable %>" runat="server">
																<h4>支払い回数</h4>
																<div><asp:DropDownList id="dllCreditInstallments2" runat="server" DataSource="<%# this.CreditInstallmentsList %>" DataTextField="Text" DataValueField="Value"></asp:DropDownList>
																<p class="attention">※AMEX/DINERSは一括のみとなります。</p></div>
															</li>
														</ul>
													</div>
													<% } %>
													<%-- △登録済みカード△ --%>
												</div>
											</dd>

											<%-- コンビニ(後払い) --%>
											<dd id="ddCvsDef" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF) %>" runat="server">
												<uc:PaymentDescriptionCvsDef runat="server" id="ucPaymentDescriptionCvsDef" />
											</dd>

											<%-- コンビニ(後払い・SMS認証) --%>
											<dd id="ddSmsDef" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_SMS_DEF) %>" runat="server">
												<uc:PaymentDescriptionSmsDef runat="server" id="ucPaymentDescriptionSmsDef" />
											</dd>

											<%-- Amazon Pay --%>
											<dd id="ddAmazonPay" class="inner" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT) %>" runat="server">
												<div style="margin: 10px 0;">
													<small>※配送先情報、または、お支払い方法の変更を希望される方は「アドレス帳」→「お支払い方法」の順で選択してください。</small>
												</div>
												<iframe id="AmazonInputWidget" src="<%: PageUrlCreatorUtility.CreateAmazonPayWidgetUrl(false, orderId: this.OrderModel.OrderId) %>" style="width:100%;border:none;"></iframe>
												<asp:HiddenField ID="hfAmazonOrderRefID" ClientIDMode="Static" runat="server" />
											</dd>
											
											<%-- Amazon Pay(CV2) --%>
											<dd id="ddAmazonPayCv2" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2) %>" runat="server">
												<%--▼▼ Amazon Pay(CV2)ボタン ▼▼--%>
												<div id="AmazonPayCv2Button2" style="display: inline-block"></div>
												<%--▲▲ Amazon Pay(CV2)ボタン ▲▲--%>
											</dd>

											<%-- 後付款(TriLink後払い) --%>
											<dd id="ddTriLinkAfterPayPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY) %>" runat="server">
												<uc:PaymentDescriptionTriLinkAfterPay runat="server" id="ucPaymentDescriptionTryLinkAfterPay" />
											</dd>

											<%-- 代金引換 --%>
											<dd id="ddCollect" class="inner" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT) %>" runat="server">
											</dd>
										
											<%-- PayPal --%>
											<dd id="ddPayPal" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL) %>" runat="server">
												<div style="display: <%= dvOrderPaymentPattern.Visible ? "block" : "none"%>">
													<%if (Constants.PAYPAL_LOGINPAYMENT_ENABLED) {%>
														<div id="paypal-button" style="margin: 5px;"></div>
														<%if (SessionManager.PayPalCooperationInfo != null) {%>
															<%: (SessionManager.PayPalCooperationInfo != null) ? SessionManager.PayPalCooperationInfo.AccountEMail : "" %> 連携済<br/>
														<%} else { %>
															<div class="error">
																<%: WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAYPAL_NEEDS_LOGIN_ERROR) %>
															</div>
														<%} %>
													<%} %>
												</div>
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
											<dd id="ddNoPayment" class="inner" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT) %>" runat="server">
											</dd>

											<%-- atone翌月払い --%>
											<dd id="ddPaymentAtone" class="Atone_0" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE) %>" runat="server">
												<span id="spanErrorMessageForAtone" class="spanErrorMessageForAtone" style="color: red; display: none" runat="server"></span>
												<uc:PaymentDescriptionAtone runat="server" id="PaymentDescriptionAtone" />
											</dd>

											<%-- aftee決済設定 --%>
											<dd id="ddPaymentAftee" class="Aftee_0" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE) %>" runat="server">
												<span id="spanErrorMessageForAftee" class="spanErrorMessageForAftee" style="color: red; display: none" runat="server"></span>
											</dd>

											<%-- （DSK）後払い --%>
											<dd id="ddDskDef" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_DSK_DEF) %>" runat="server">
												コンビニ後払い（DSK）
											</dd>
											<%-- ID決済(wechatpay、aripay、キャリア決済) --%>
											<dd id="ddCarrierbillingBokuPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU) %>" runat="server">
												ID決済(wechatpay、aripay、キャリア決済)
											</dd>
										</ItemTemplate>
										<FooterTemplate>
											</dl>
										</FooterTemplate>
									</asp:Repeater>
								</dl>
								</div><!--list-->
							</div>
						</td>
					</tr><!--End input payment update form-->
					<tr><!--Action button payment update form-->
						<th></th>
						<td>
							<div id="divOrderPaymentUpdateButtons" style="display: block">
							<% if (this.CanDisplayChangeFixedPurchasePayment) { %>
								<asp:CheckBox ID="cbIsUpdateFixedPurchaseByOrderPayment" Text="今後の定期注文にも反映させる" Checked="false" runat="server"/><br />
							<% } %>
							<asp:LinkButton ID="lbUpdatePayment" Text="情報更新" runat="server" ValidationGroup="OrderPayment" OnClientClick="doPostbackEvenIfCardAuthFailed=false;return CheckPaymentChange(this);" OnClick="btnUpdatePaymentPatternInfo_Click" class="btn" ></asp:LinkButton>
							<br />
							<asp:LinkButton Text="キャンセル" runat="server" OnClick="btnClosePaymentPatternInfo_Click" class="btn"></asp:LinkButton>
							</div>
							<div id="divOrderPaymentUpdateExecFroms" style="display: none"> 
								更新中です...
							</div>
							<small id="sErrorMessagePayment" style="color:red" class="error" runat="server"></small>
						</td>
					</tr>
				</div>
				<!--End Update payment pattern-->
				<% if ((this.OrderModel.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) && (this.UserCreditCardInfo != null)) { %>
				<tr id="dvFixedPurchaseCurrentCard" runat="server">
					<th>利用クレジットカード情報</th>
					<td>
					<br />
						<% if (this.UserCreditCardInfo.DispFlg == Constants.FLG_USERCREDITCARD_DISP_FLG_ON) { %>
						カード登録名: <%:this.UserCreditCardInfo.CardDispName %><%: this.UserCreditCardInfo.DispFlag ? "" : " (削除済)" %><br />
						<% } %>
						<%if (OrderCommon.CreditCompanySelectable && (this.UserCreditCardInfo.CompanyName != string.Empty)) {%>
						カード会社: <%: this.UserCreditCardInfo.CompanyName %><br />
						<%} %>
						カード番号: XXXXXXXXXXXX<%: this.UserCreditCardInfo.LastFourDigit %><br />
						有効期限: <%: this.UserCreditCardInfo.ExpirationMonth + "/" + this.UserCreditCardInfo.ExpirationYear + " (月/年)" %><br />
						カード名義人: <%: this.UserCreditCardInfo.AuthorName %>
					</td>
				</tr>
				<%} %>
			</dd>
			<dt>注文メモ</dt>
			<dd><%# StringUtility.ToEmpty(this.OrderModel.Memo) != "" ? WebSanitizer.HtmlEncodeChangeToBr(this.OrderModel.Memo) : "指定なし" %></dd>
			<%if (IsDisplayOrderExtend()) {%>
			<dt>注文確認事項</dt>
			<dd>
				<% if (this.IsOrderExtendModify) { %>
				<asp:Repeater ID="rOrderExtendInput" ItemType="OrderExtendItemInput" runat="server">
					<HeaderTemplate>
						<dl>
					</HeaderTemplate>
					<ItemTemplate>
						<%-- 項目名 --%>
						<dt><%#: Item.SettingModel.SettingName %><span class="require" runat="server" visible="<%# Item.SettingModel.IsNeecessary%>">※</span></dt>
						<dd>
							<%-- 概要 --%>
							<%# Item.SettingModel.OutlineHtmlEncode %>
							<%-- TEXT --%>
							<div runat="server" visible="<%# Item.SettingModel.CanUseModify && Item.SettingModel.IsInputTypeText%>">
								<asp:TextBox runat="server" ID="tbSelect" Width="250px"></asp:TextBox>
							</div>
							<%-- DDL --%>
							<div runat="server" visible="<%# Item.SettingModel.CanUseModify && Item.SettingModel.IsInputTypeDropDown %>">
								<asp:DropDownList runat="server" ID="ddlSelect"></asp:DropDownList>
							</div>
							<%-- RADIO --%>
							<div runat="server" visible="<%# Item.SettingModel.CanUseModify && Item.SettingModel.IsInputTypeRadio %>">
								<asp:RadioButtonList runat="server" ID="rblSelect" RepeatDirection="Horizontal" RepeatColumns="3" RepeatLayout="Flow" CssClass="radioBtn"></asp:RadioButtonList>
							</div>
							<%-- CHECK --%>
							<div runat="server" visible="<%# Item.SettingModel.CanUseModify && Item.SettingModel.IsInputTypeCheckBox %>">
								<asp:CheckBoxList runat="server" ID="cblSelect" RepeatDirection="Horizontal" RepeatColumns="3" RepeatLayout="Flow" CssClass="checkBox"></asp:CheckBoxList>
							</div>
							<div runat="server" visible="<%# (Item.SettingModel.CanUseModify == false) %>">
								<%#: Item.InputText %>
							</div>
							<%-- 検証文言 --%>
							<small><asp:Label runat="server" ID="lbErrMessage" CssClass="attention"></asp:Label></small>
							<asp:HiddenField ID="hfSettingId" runat="server" Value="<%# Item.SettingModel.SettingId %>" />
							<asp:HiddenField ID="hfInputType" runat="server" Value="<%# Item.SettingModel.InputType %>" />
						</dd>
					</ItemTemplate>
					<FooterTemplate>
						</dl>
					</FooterTemplate>
				</asp:Repeater>
				<% } else { %>
				<asp:Repeater ID="rOrderExtendDisplay" ItemType="OrderExtendItemInput" runat="server">
					<HeaderTemplate>
						<dl>
					</HeaderTemplate>
					<ItemTemplate>
						<dt><%#: Item.SettingModel.SettingName %>:</dt>
						<dd><%#: Item.InputText %></dd>
					</ItemTemplate>
					<FooterTemplate>
						</dl>
					</FooterTemplate>
				</asp:Repeater>
				<% } %>
				<% if (this.IsModifyOrder) { %>
				<div class="button">
					<asp:LinkButton ID="lbOrderExtend" Text="注文確認事項の変更" Visible="<%# IsDisplayOrderExtendModifyButton() %>" Enabled="<%# this.IsModifyOrder %>" OnClick="lbOrderExtend_OnClick"  runat="server" class="btn" />
				</div>
				<% } %>
			</dd>
			<% if (this.IsOrderExtendModify) { %>
				<dd>
					<div id="divOrderExtendUpdateButtons" class="button" style="display: block"> 
						<asp:LinkButton ID="lbUpdateOrderExtend" Text="情報更新" runat="server" OnClientClick="return AlertDataChange('OrderPointUse', null);" OnClick="lbUpdateOrderExtend_OnClick" class="btn" /><br />
						<asp:LinkButton ID="lbHideOrderExtend" Text="キャンセル" runat="server" OnClientClick="return exec_submit();" OnClick="lbHideOrderExtend_OnClick" class="btn" />
					</div>
					<div id="divOrderExtendUpdateExecFroms" style="display: none"> 更新中です...</div>
				</dd>
			<% } %>
			<% } %>

			<%-- ポイントオプションが有効な場合 --%>
			<%if (Constants.W2MP_POINT_OPTION_ENABLED) {%>
			<dt>購入時付与ポイント</dt>
			<dd><%#: GetNumeric(this.OrderModel.OrderPointAdd) %><%= Constants.CONST_UNIT_POINT_PT %></dd>
			<dt>ご利用ポイント</dt>
			<dd>
				<%#: GetNumeric(this.OrderModel.OrderPointUse) %><%= Constants.CONST_UNIT_POINT_PT %>
				<div class="button" runat="server" Visible ="<%# this.IsModifyUsePoint %>">
					<asp:LinkButton ID="lbDisplayInputOrderPointUse" Text="利用ポイント変更" OnClick="lbDisplayInputOrderPointUse_Click"  runat="server" CssClass="btn" Visible="<%# this.CanUsePointForPurchase %>" />
					<small id="slErrorMessageChangePointUse" runat="server"></small>
				</div>
				<% if (this.CanDisplayPointUse) { %>
				<br />
				利用可能ポイントは<%: StringUtility.ToNumeric(this.LoginUserPointUsable + this.OrderModel.OrderPointUse) %><%: Constants.CONST_UNIT_POINT_PT %>です。<br/>
				※1<%: Constants.CONST_UNIT_POINT_PT %> = <%: CurrencyManager.ToPrice(1m) %><br />
				<asp:TextBox ID="tbOrderPointUse" runat="server" style="width: 70px;"></asp:TextBox><%: Constants.CONST_UNIT_POINT_PT %><br />
				<small id="slErrorMessagePointUse" runat="server" class="fred"></small>
				<% } %>
			</dd>
			<% if (this.IsOrderPointAddDisplayStatus) { %>
			<dt></dt>
			<dd>
				<div id="divOrderPointUpdateButtons" style="display: block"> 
				<asp:LinkButton Text="情報更新" runat="server" OnClientClick="return AlertDataChange('OrderPointUse', null);" OnClick="lbUpdateOrderPointUse_Click" class="btn" />
				<br />
				<asp:LinkButton Text="キャンセル" runat="server" OnClientClick="return exec_submit();" OnClick="lbHideOrderPointUse_Click" class="btn" />
				</div>
				<div id="divOrderPointUpdateExecFroms" style="display: none"> 
					更新中です...
				</div>
			</dd>
			<% } %>
			<%} %>
			<dt>総合計（税込）</dt>
			<dd><%#: CurrencyManager.ToPrice(this.OrderModel.OrderPriceTotal) %></dd>
		</dl>
		<%--▽領収書情報▽--%>
		<% if (Constants.RECEIPT_OPTION_ENABLED) { %>
		<h3>領収書情報</h3>
		<% if (this.IsReceiptInfoModify == false) { %>
		<dl class="order-form">
			<dt>領収書希望</dt>
			<dd>
				<%: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_RECEIPT_FLG, this.OrderModel.ReceiptFlg) %>
				<%: (this.OrderModel.ReceiptOutputFlg == Constants.FLG_ORDER_RECEIPT_OUTPUT_FLG_ON) ? "(出力済み)" : "" %>
			</dd>
			<div runat="server" visible="<%# this.OrderModel.ReceiptFlg == Constants.FLG_ORDER_RECEIPT_FLG_ON %>">
				<dt>宛名</dt>
				<dd><%: this.OrderModel.ReceiptAddress %></dd>
			</div>
			<div runat="server" visible="<%# this.OrderModel.ReceiptFlg == Constants.FLG_ORDER_RECEIPT_FLG_ON %>">
				<dt>但し書き</dt>
				<dd><%: this.OrderModel.ReceiptProviso %></dd>
			</div>
			<% if (lbDisplayReceiptInfoForm.Visible) { %>
			<dd class="button">
				<asp:LinkButton ID="lbDisplayReceiptInfoForm" Text="領収書情報変更" runat="server" Visible="<%# this.CanModifyReceiptInfo %>" OnClick="lbDisplayReceiptInfoForm_Click" CssClass="btn" />
			</dd>
		</dl>
		<% } %>
		<% } else { %>
		<dl class="order-form">
			<dt>領収書希望</dt>
			<dd>
				<asp:DropDownList ID="ddlReceiptFlg" runat="server" DataTextField="Text" DataValueField="Value"
					OnSelectedIndexChanged="ddlReceiptFlg_OnSelectedIndexChanged" AutoPostBack="true" DataSource="<%# this.ReceiptFlgListItems %>" />
			</dd>
			<div id="dvReceiptAddressProvisoInput" runat="server">
			<dt>宛名<span class="require">※</span></dt>
			<dd>
				<asp:TextBox ID="tbReceiptAddress" runat="server" Width="300" MaxLength="100" />
				<p><asp:CustomValidator runat="Server"
					ControlToValidate="tbReceiptAddress"
					ValidationGroup="ReceiptRegisterModify"
					ClientValidationFunction="ClientValidate"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					EnableClientScript="false"
					CssClass="error_inline" /></p>
			</dd>
			<dt>但し書き<span class="require">※</span></dt>
			<dd>
				<asp:TextBox ID="tbReceiptProviso" runat="server" Width="300" MaxLength="100" />
				<p><asp:CustomValidator runat="Server"
					ControlToValidate="tbReceiptProviso"
					ValidationGroup="ReceiptRegisterModify"
					ClientValidationFunction="ClientValidate"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					EnableClientScript="false"
					CssClass="error_inline" /></p>
			</dd>
			</div>
		</dl>
		<div class="user-footer">
			<div class="button-next">
				<asp:LinkButton Text="領収書情報更新" runat="server" OnClientClick="return confirm('領収書情報を変更してもよろしいですか？')" OnClick="lbUpdateReceiptInfo_Click" CssClass="btn" />
			</div>
			<div class="button-prev">
				<asp:LinkButton Text="キャンセル" runat="server" OnClientClick="return exec_submit();" OnClick="lbDisplayReceiptInfoForm_Click" CssClass="btn" />
			</div>
			<p class="msg" style="color: red;"><%: this.ReceiptInfoModifyErrorMessage %></p>
		</div>
		<% } %>
		<div class="order-form">
			<dd class="button" visible="<%# ((this.OrderModel.ReceiptFlg == Constants.FLG_ORDER_RECEIPT_FLG_ON) && (this.OrderModel.ReceiptOutputFlg == Constants.FLG_ORDER_RECEIPT_OUTPUT_FLG_OFF) && (this.OrderModel.IsCanceled == false)) %>" runat="server">
				<asp:LinkButton ID="lbReceiptDownload" Text="領収書ダウンロード" runat="server" OnClientClick="openReceiptDownload()" class="btn" />
			</dd>
		</div>
		<div class="receipt-download-message"><%: this.ReceiptDownloadErrorMessage %></div>
		<% } %>
		<%--△領収書情報△--%>
	<a name="ShippingArea" runat="server"></a>
	<%
		this.CartShippingItemIndexTmp = -1;
	%>
	<asp:Repeater ID="rOrderShipping" DataSource="<%# this.OrderShippingItems %>" Runat="server">
		<ItemTemplate>
		<% if (this.OrderModel.DigitalContentsFlg != Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_ON) { %>
			<%-- お届け先情報 --%>
			<h3>お届け先情報<%# (this.OrderShippingItems.Count > 1) ? (Container.ItemIndex + 1).ToString() : "" %></h3>
			<asp:Label ID="lOrderHistoryErrorMessage" CssClass="attention" runat="server"></asp:Label>
			<% if (this.IsPickupRealShop == false) { %>
			<div id="dShippingInfo" runat="server" visible="true">
				<dl class="order-form">
					<% if (this.UseShippingAddress) { %>
					<% if (this.OrderModel.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT) { %>
						<% if (this.OrderModel.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2) { %>
						<dd id="Dd3" class="button" runat="server" visible="<%# this.IsModifyShipping %>">
							<asp:LinkButton ID="LinkButton1" Text="お届け先変更" CommandArgument="<%# Container.ItemIndex %>" runat="server" OnClientClick="return exec_submit();" OnClick="lbDisplayUserShippingInfoForm_Click" CssClass="btn" />
						</dd>
						<% } else if(this.IsModifyShipping) { %>
							<%--▼▼ Amazon Pay(CV2)ボタン ▼▼--%>
							<div id="AmazonPayCv2Button" style="display: inline-block"></div>
							<%--▲▲ Amazon Pay(CV2)ボタン ▲▲--%>
						<% } %>
					<dt>
						<%: ReplaceTag("@@User.addr.name@@") %>
					</dt>
					<dd>
						<%# IsCountryJp((string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE))
							? "〒" + WebSanitizer.HtmlEncode(GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP)) + "<br />"
							: "" %>
						<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1) %>
						<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2) %>
						<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3) %>
						<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4) %>
						<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5) %>
						<%#: (IsCountryJp((string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE)) == false)
							? GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP)
							: "" %>
						<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_NAME) %>
					</dd>
					<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
					<dt>
						<%-- 企業名・部署名 --%>
						<%: ReplaceTag("@@User.company_name.name@@")%>/<%: ReplaceTag("@@User.company_post_name.name@@")%>
					</dt>
					<dd>
						<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_NAME) %>/<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_POST_NAME) %>
					</dd>
					<%} %>
					<dt>
						<%-- 氏名 --%>
						<%: ReplaceTag("@@User.name.name@@") %>
					</dt>
					<dd>
						<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME) %> 様
						<%#: IsCountryJp((string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE))
							? "(" + GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA) + " さま)" 
							: ""%>
					</dd>
					<%} %>
					<dt>
						<%-- 電話番号 --%>
						<%: ReplaceTag("@@User.tel1.name@@") %>
					</dt>
					<dd>
						<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %>
					</dd>
					<% } else { %>
						<dd class="button" runat="server" visible="<%# this.IsModifyShipping %>">
							<asp:LinkButton Text="お届け先変更" CommandArgument="<%# Container.ItemIndex %>" runat="server" OnClientClick="return exec_submit();" OnClick="lbDisplayUserShippingInfoForm_Click" CssClass="btn" />
						</dd>
						<dt>店舗ID</dt>
						<dd>
							<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID) %>
						</dd>
						<dt>店舗名称</dt>
						<dd><%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME) %></dd>
						<dt>店舗住所</dt>
						<dd><%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4) %></dd>
						<dt>店舗電話番号</dt>
						<dd><%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %></dd>
					<% } %>
				</dl>
			</div>
			<% this.CartShippingItemIndexTmp++; %>
			<div id="dShippngInput" class="order-unit" runat="server" visible="false">
				<div>
					<asp:DropDownList
						Width="100%"
						DataTextField="Text"
						DataValueField="Value"
						SelectedValue='<%# IsDisplayButtonConvenienceStore((string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG)) ? CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE : null %>'
						ID="ddlShippingType"
						AutoPostBack="true"
						OnSelectedIndexChanged="ddlShippingType_SelectedIndexChanged"
						DataSource='<%# GetPossibleShippingType((string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG)) %>'
						runat="server"
						CssClass="UserShippingAddress">
					</asp:DropDownList>
					<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
					<asp:DropDownList
						ID="ddlShippingReceivingStoreType"
						DataTextField="Text"
						DataValueField="Value"
						SelectedValue='<%# GetShippingReceivingStoreTypeValue(
							IsDisplayButtonConvenienceStore((string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG)),
							(string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_TYPE)) %>'
						Visible='<%# IsDisplayButtonConvenienceStore((string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG)) %>'
						AutoPostBack="true"
						OnSelectedIndexChanged="ddlShippingReceivingStoreType_SelectedIndexChanged"
						DataSource='<%# ShippingReceivingStoreType() %>'
						runat="server"
						CssClass="UserShippingAddress" />
					<br />
					<% } %>
				</div>
				<br />
				<% if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED) { %>
				<div class="button-next" id="spConvenienceStoreSelect" runat="server" visible='<%# (IsDisplayButtonConvenienceStore((string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG)) && (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false)) %>'>
					<a href="javascript:openConvenienceStoreMapPopup(<%# Container.ItemIndex %>);" class="btn" >Family/OK/Hi-Life</a>
				</div>
				<div id="spConvenienceStoreEcPaySelect" runat="server" visible='<%# (IsDisplayButtonConvenienceStore((string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG)) && Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) %>'>
					<asp:LinkButton
						ID="lbOpenEcPay"
						runat="server"
						class="btn btn-success convenience-store-button"
						OnClick="lbOpenEcPay_Click"
						CommandArgument="<%# Container.ItemIndex %>"
						Text="  電子マップ  " />
				</div>
				<br />
				<div id="dvErrorShippingConvenience" runat="server" style="display:none;">
					<span class="error_inline" style="color:red;"><%#: WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_GROCERY_STORE) %></span>
				</div>
				<br />
				<div id="dvErrorPaymentAndShippingConvenience" runat="server" visible="false">
					<span class="error_inline" style="color:red;"><%#: WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAYMENT_METHOD_CHANGED_TO_CONVENIENCE_STORE) %></span>
				</div>
				<br />
				<% } %>
				<div id="divShippingInputFormInner" runat="server">
				<%
					var shippingAddrCountryIsoCode = GetShippingAddrCountryIsoCode(this.CartShippingItemIndexTmp);
					var isShippingAddrCountryJp = IsCountryJp(shippingAddrCountryIsoCode);
					var isShippingAddrCountryUs = IsCountryUs(shippingAddrCountryIsoCode);
					var isShippingAddrCountryTw = IsCountryTw(shippingAddrCountryIsoCode);
					var isShippingAddrZipNecessary = IsAddrZipcodeNecessary(shippingAddrCountryIsoCode);
				%>
				<dl class="order-form">
					<dd class="button">
						<asp:LinkButton Text="お届け先変更" CommandArgument="<%# Container.ItemIndex %>" runat="server" OnClientClick="return exec_submit();" OnClick="lbDisplayUserShippingInfoForm_Click" CssClass="btn" />
					</dd>
					<%-- 氏名 --%>
					<dt>
						<%: ReplaceTag("@@User.name.name@@") %>
						<span class="require">※</span>
					</dt>
					<dd class="name">
						<p class="attention">
						<asp:CustomValidator
							ID="cvShippingName1"
							runat="Server"
							ControlToValidate="tbShippingName1"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							EnableClientScript="false"
							CssClass="error_inline" />
						<asp:CustomValidator 
							ID="cvShippingName2"
							runat="Server"
							ControlToValidate="tbShippingName2"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							EnableClientScript="false"
							CssClass="error_inline" />
						</p>
						<w2c:ExtendedTextBox ID="tbShippingName1" placeholder='<%# ReplaceTag("@@User.name1.name@@") %>' MaxLength='<%# GetMaxLength("@@User.name1.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
						<w2c:ExtendedTextBox ID="tbShippingName2" placeholder='<%# ReplaceTag("@@User.name2.name@@") %>' MaxLength='<%# GetMaxLength("@@User.name2.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
					</dd>
					<%-- 氏名（かな） --%>
					<% if (isShippingAddrCountryJp) { %>
					<dt>
						<%: ReplaceTag("@@User.name_kana.name@@") %>
						<span class="require">※</span>
					</dt>
					<dd class="name-kana">
						<p class="attention">
						<asp:CustomValidator
							ID="cvShippingNameKana1"
							runat="Server"
							ControlToValidate="tbShippingNameKana1"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							EnableClientScript="false"
							CssClass="error_inline" />
						<asp:CustomValidator
							ID="cvShippingNameKana2"
							runat="Server"
							ControlToValidate="tbShippingNameKana2"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							EnableClientScript="false"
							CssClass="error_inline" />
						</p>
						<w2c:ExtendedTextBox ID="tbShippingNameKana1" placeholder='<%# ReplaceTag("@@User.name_kana1.name@@") %>' MaxLength='<%# GetMaxLength("@@User.name_kana1.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
						<w2c:ExtendedTextBox ID="tbShippingNameKana2" placeholder='<%# ReplaceTag("@@User.name_kana2.name@@") %>' MaxLength='<%# GetMaxLength("@@User.name_kana2.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
					</dd>
					<% } %>
					<%-- 国 --%>
					<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
					<dt>
						<%: ReplaceTag("@@User.country.name@@", shippingAddrCountryIsoCode) %>
							<span class="require">※</span>
					</dt>
					<dd>
						<asp:DropDownList ID="ddlShippingCountry" runat="server" DataTextField="<%#: Container.ItemIndex %>" AutoPostBack="true" OnSelectedIndexChanged="ddlShippingCountry_SelectedIndexChanged"></asp:DropDownList>
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
						<span class="require">※</span>
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
							EnableClientScript="false"
							CssClass="error_inline cvShippingZipShortInput" />
						<span id="sShippingZipError" runat="server" class="shortZipInputErrorMessage sShippingZipError"></span>
						</p>
						<w2c:ExtendedTextBox ID="tbShippingZip" Type="tel" MaxLength="8" ValidationGroup="<%# Container.ItemIndex %>" OnTextChanged="lbSearchAddr_TextBox_Click" runat="server" />
						<br />
						<asp:LinkButton ID="lbSearchShippingAddr" runat="server" CommandArgument="<%# Container.ItemIndex %>" OnClientClick="return false;" OnClick="lbSearchAddr_LinkButton_Click" CssClass="btn-add-search">
						郵便番号から住所を入力</asp:LinkButton>
					</dd>
					<%--検索結果レイヤー--%>
					<uc:Layer ID="ucLayer" runat="server" />
					<% } %>
					<dt>
						<%: ReplaceTag("@@User.addr.name@@") %>
						<span class="require">※</span>
					</dt>
					<dd class="address">
						<%-- 都道府県 --%>
						<p class="attention">
						<% if (isShippingAddrCountryJp) { %>
						<asp:CustomValidator
							ID="cvShippingAddr1"
							runat="Server"
							ControlToValidate="ddlShippingAddr1"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							EnableClientScript="false"
							CssClass="error_inline" />
						</p>
						<asp:DropDownList ID="ddlShippingAddr1" DataTextField="<%#: Container.ItemIndex %>" DataValueField="Value" runat="server" OnSelectedIndexChanged="ddlShippingAddr1_SelectedIndexChanged"></asp:DropDownList>
						<% } %>
						<%-- 市区町村 --%>
						<% if (isShippingAddrCountryTw) { %>
							<asp:DropDownList runat="server" ID="ddlShippingAddr2" DataSource="<%# this.UserTwCityList %>" AutoPostBack="true" DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlShippingAddr2_SelectedIndexChanged"></asp:DropDownList>
							<br />
						<% } else { %>
							<p class="attention">
							<asp:CustomValidator
								ID="cvShippingAddr2"
								runat="Server"
								ControlToValidate="tbShippingAddr2"
								ValidationGroup="OrderShipping"
								ValidateEmptyText="true"
								SetFocusOnError="true"
								ClientValidationFunction="ClientValidate"
								EnableClientScript="false"
								CssClass="error_inline" />
							</p>
							<w2c:ExtendedTextBox ID="tbShippingAddr2" placeholder='市区町村 ※' MaxLength='<%# GetMaxLength("@@User.addr2.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
						<% } %>
						<%-- 番地 --%>
						<% if (isShippingAddrCountryTw) { %>
							<asp:DropDownList runat="server" ID="ddlShippingAddr3" DataTextField="Key" DataValueField="Value" Width="95" ></asp:DropDownList>
						<% } else { %>
							<p class="attention">
							<asp:CustomValidator
								ID="cvShippingAddr3"
								runat="Server"
								ControlToValidate="tbShippingAddr3"
								ValidationGroup="OrderShipping"
								ValidateEmptyText="true"
								SetFocusOnError="true"
								ClientValidationFunction="ClientValidate"
								EnableClientScript="false"
								CssClass="error_inline" />
							</p>
							<w2c:ExtendedTextBox ID="tbShippingAddr3" placeholder='番地 ※' MaxLength='<%# GetMaxLength("@@User.addr3.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
						<% } %>
						<%-- ビル・マンション名 --%>
						<p class="attention">
						<asp:CustomValidator 
							ID="cvShippingAddr4"
							runat="Server"
							ControlToValidate="tbShippingAddr4"
							ValidationGroup="OrderShipping"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							EnableClientScript="false"
							CssClass="error_inline" />
						</p>
						<w2c:ExtendedTextBox ID="tbShippingAddr4" placeholder='建物名' MaxLength='<%# GetMaxLength("@@User.addr4.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
						<%-- 州 --%>
						<% if (isShippingAddrCountryJp == false) { %>
							<% if (isShippingAddrCountryUs) { %>
						<asp:DropDownList runat="server" ID="ddlShippingAddr5"></asp:DropDownList>
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
									SetFocusOnError="true"
									ClientValidationFunction="ClientValidate"
									EnableClientScript="false"
									CssClass="error_inline" />
							</p>
							<w2c:ExtendedTextBox ID="tbShippingAddrTw" placeholder='省' MaxLength='<%# GetMaxLength("@@User.addr5.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
						<% } else { %>
						<p class="attention">
							<asp:CustomValidator
								ID="cvShippingAddr5"
								runat="Server"
							ControlToValidate="tbShippingAddr5"
							ValidationGroup="OrderShippingGlobal"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							EnableClientScript="false"
							CssClass="error_inline" />
						</p>
						<w2c:ExtendedTextBox ID="tbShippingAddr5" placeholder='州' MaxLength='<%# GetMaxLength("@@User.addr5.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
						<% } %>
						<% } %>
					</dd>
					<%-- 郵便番号（海外向け） --%>
					<% if (isShippingAddrCountryJp == false) { %>
					<dt>
						<%: ReplaceTag("@@User.zip.name@@", shippingAddrCountryIsoCode) %>
						<% if (isShippingAddrZipNecessary) { %><span class="require">※</span><% } %>
					</dt>
					<dd>
						<p class="attention">
						<asp:CustomValidator
							ID="cvShippingZipGlobal"
							runat="Server"
							ControlToValidate="tbShippingZipGlobal"
							ValidationGroup="OrderShippingGlobal"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							EnableClientScript="false"
							CssClass="error_inline" />
						</p>
						<w2c:ExtendedTextBox ID="tbShippingZipGlobal" Type="tel" MaxLength="20" runat="server"></w2c:ExtendedTextBox>
						<asp:LinkButton
							ID="lbSearchAddrFromShippingZipGlobal"
							OnClick="lbSearchAddrFromShippingZipGlobal_Click"
							CommandArgument="<%# Container.ItemIndex %>"
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
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							EnableClientScript="false"
							CssClass="error_inline" />
						</p>
						<w2c:ExtendedTextBox ID="tbShippingCompanyName" placeholder='<%# ReplaceTag("@@User.company_name.name@@") %>' MaxLength='<%# GetMaxLength("@@User.company_name.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
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
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							EnableClientScript="false"
							CssClass="error_inline" />
						</p>
						<w2c:ExtendedTextBox ID="tbShippingCompanyPostName" placeholder='<%# ReplaceTag("@@User.company_post_name.name@@") %>' MaxLength='<%# GetMaxLength("@@User.company_post_name.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
					</dd>
					<%} %>
					<%-- 電話番号 --%>
					<% if (isShippingAddrCountryJp) { %>
					<dt><%: ReplaceTag("@@User.tel1.name@@") %><span class="require">※</span></dt>
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
						<w2c:ExtendedTextBox ID="tbShippingTel1" Type="tel" MaxLength="13" style="width:100%;" runat="server" CssClass="shortTel" />
					</dd>
					<% } else { %>
					<dt>
						<%: ReplaceTag("@@User.tel1.name@@", shippingAddrCountryIsoCode) %>
						<span class="require">※</span>
					</dt>
					<dd>
						<p class="attention">
						<asp:CustomValidator
							ID="cvShippingTel1Global"
							runat="Server"
							ControlToValidate="tbShippingTel1Global"
							ValidationGroup="OrderShippingGlobal"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							EnableClientScript="false"
							CssClass="error_inline" />
						</p>
						<w2c:ExtendedTextBox ID="tbShippingTel1Global" Type="tel" MaxLength="30" runat="server"></w2c:ExtendedTextBox>
					</dd>
					<% } %>
				</dl>
				</div>
				<%-- ▽コンビニ▽ --%>
				<asp:HiddenField ID="hfCvsShopFlg" runat="server" Value='<%# GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG) %>' />
				<asp:HiddenField ID="hfSelectedShopId" runat="server" Value='<%# GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID) %>' />
				<dl id="divConvenienceStore" class="order-form" runat="server">
					<div class="<%# Container.ItemIndex %>">
						<dt>店舗ID</dt>
						<dd class="convenience-store-item" id="ddCvsShopId">
							<span style="font-weight:normal;">
								<asp:Literal ID="lCvsShopId" runat="server" Text='<%# GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID) %>'/>
							</span>
							<asp:HiddenField ID="hfCvsShopId" runat="server" Value='<%# GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID) %>' />
						</dd>
						<dt>店舗名称</dt>
						<dd class="convenience-store-item" id="ddCvsShopName">
							<span style="font-weight:normal;">
								<asp:Literal ID="lCvsShopName" runat="server" Text='<%# GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME) %>'/>
							</span>
							<asp:HiddenField ID="hfCvsShopName" runat="server" Value='<%# GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME) %>' />
						</dd>
						<dt>店舗住所</dt>
						<dd class="convenience-store-item" id="ddCvsShopAddress">
							<span style="font-weight:normal;">
								<asp:Literal ID="lCvsShopAddress" runat="server" Text='<%# GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4) %>'></asp:Literal>
							</span>
							<asp:HiddenField ID="hfCvsShopAddress" runat="server" Value='<%# GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4) %>' />
						</dd>
						<dt>店舗電話番号</dt>
						<dd class="convenience-store-item" id="ddCvsShopTel">
							<span style="font-weight:normal;">
								<asp:Literal ID="lCvsShopTel" runat="server" Text='<%# GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %>'></asp:Literal>
							</span>
							<asp:HiddenField ID="hfCvsShopTel" runat="server" Value='<%# GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %>' />
						</dd>
						</di>
					</div>
				</dl>
				<dl class="order-form" runat="server" id="tbOwnerAddress" visible="false">
					<dt>
						<%: ReplaceTag("@@User.addr.name@@") %>
					</dt>
					<dd>
						<%# IsCountryJp(this.LoginUser.AddrCountryIsoCode)
							? "〒" + WebSanitizer.HtmlEncode(this.LoginUser.Zip) + "<br />"
							: "" %>
						<%#: this.LoginUser.Addr1 %>
						<%#: this.LoginUser.Addr2 %>
						<%#: this.LoginUser.Addr3 %>
						<%#: this.LoginUser.Addr4 %>
						<%#: this.LoginUser.Addr5 %>
						<%# (IsCountryJp(this.LoginUser.AddrCountryIsoCode) == false)
										? WebSanitizer.HtmlEncode(this.LoginUser.Zip) + "<br />"
										: "" %>
						<%#: this.LoginUser.AddrCountryName %>
					</dd>
					<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
					<dt>
						<%-- 企業名・部署名 --%>
						<%: ReplaceTag("@@User.company_name.name@@")%>/<%: ReplaceTag("@@User.company_post_name.name@@")%>
					</dt>
					<dd>
						<%#: this.LoginUser.CompanyName %>/<%#: this.LoginUser.CompanyPostName %>
					</dd>
					<%} %>
					<dt>
						<%-- 氏名 --%>
						<%: ReplaceTag("@@User.name.name@@") %>
					</dt>
					<dd>
						<%#: this.LoginUser.Name %> 様
						<%#: IsCountryJp(this.LoginUser.AddrCountryIsoCode)
							? "(" + this.LoginUser.NameKana + " さま)" 
							: ""%>
					</dd>
					<dt>
						<%-- 電話番号 --%>
						<%: ReplaceTag("@@User.tel1.name@@") %>
					</dt>
					<dd>
						<%#: this.LoginUser.Tel1 %>
					</dd>
				</dl>
				<asp:Repeater Visible="false" runat="server" ItemType="UserShippingModel" DataSource="<%# this.UserShippingAddr %>" ID="rOrderShippingList">
					<ItemTemplate>
						<asp:HiddenField runat="server" ID="hfShippingNo" Value="<%# Item.ShippingNo %>" />
						<dl class='<%# string.Format("order-form user_addres{0}", Item.ShippingNo) %>' runat="server" visible="<%# (Item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON) %>">
							<dt>店舗ID</dt>
							<dd class="convenience-store-item" id="ddCvsShopId">
								<span style="font-weight:normal;">
									<asp:Literal ID="lCvsShopId" runat="server" Text='<%# Item.ShippingReceivingStoreId %>' />
								</span>
								<asp:HiddenField ID="hfCvsShopId" runat="server" Value="<%# Item.ShippingReceivingStoreId %>" />
							</dd>
							<dt>店舗名称</dt>
							<dd class="convenience-store-item" id="ddCvsShopName">
								<span style="font-weight:normal;">
									<asp:Literal ID="lCvsShopName" runat="server" Text='<%# Item.ShippingName %>' />
								</span>
								<asp:HiddenField ID="hfCvsShopName" runat="server" Value="<%# Item.ShippingName %>" />
							</dd>
							<dt>店舗住所</dt>
							<dd class="convenience-store-item" id="ddCvsShopAddress">
								<span style="font-weight:normal;">
									<asp:Literal ID="lCvsShopAddress" runat="server" Text='<%# Item.ShippingAddr4 %>'></asp:Literal>
								</span>
								<asp:HiddenField ID="hfCvsShopAddress" runat="server" Value="<%# Item.ShippingAddr4 %>" />
							</dd>
							<dt>店舗電話番号</dt>
							<dd class="convenience-store-item" id="ddCvsShopTel">
								<span style="font-weight:normal;">
									<asp:Literal ID="lCvsShopTel" runat="server" Text='<%# Item.ShippingTel1 %>'></asp:Literal>
								</span>
								<asp:HiddenField ID="hfCvsShopTel" runat="server" Value="<%# Item.ShippingTel1 %>" />
							</dd>
						</dl>
						<dl class="order-form" runat="server" visible="<%# (Item.ShippingReceivingStoreFlg != Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON) %>">
							<dt>
								<%: ReplaceTag("@@User.addr.name@@") %>
							</dt>
							<dd>
								<%# IsCountryJp(Item.ShippingCountryIsoCode)
									? "〒" + WebSanitizer.HtmlEncode(Item.ShippingZip) + "<br />"
									: "" %>
								<%#: Item.ShippingAddr1 %>
								<%#: Item.ShippingAddr2 %>
								<%#: Item.ShippingAddr3 %>
								<%#: Item.ShippingAddr4 %>
								<%#: Item.ShippingAddr5 %>
								<%# (IsCountryJp(Item.ShippingCountryIsoCode) == false)
												? WebSanitizer.HtmlEncode(Item.ShippingZip) + "<br />"
												: "" %>
								<%#: Item.ShippingCountryName %>
							</dd>
							<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
							<dt>
								<%-- 企業名・部署名 --%>
								<%: ReplaceTag("@@User.company_name.name@@")%>/<%: ReplaceTag("@@User.company_post_name.name@@")%>
							</dt>
							<dd>
								<%#: Item.ShippingCompanyName %>/<%#: Item.ShippingCompanyPostName %>
							</dd>
							<%} %>
							<dt>
								<%-- 氏名 --%>
								<%: ReplaceTag("@@User.name.name@@") %>
							</dt>
							<dd>
								<%#: Item.ShippingName %> 様
								<%#: IsCountryJp(Item.ShippingCountryIsoCode)
									? "(" + Item.ShippingNameKana + " さま)" 
									: ""%>
							</dd>
							<dt>
								<%-- 電話番号 --%>
								<%: ReplaceTag("@@User.tel1.name@@") %>
							</dt>
							<dd>
								<%#: Item.ShippingTel1 %>
							</dd>
						</dl>
					</ItemTemplate>
				</asp:Repeater>
				<div id="divOrderShippingUpdateButtons" style="display: block"> 
					<% if (this.IsFixedPurchase && (this.FixedPurchaseModel.IsCancelFixedPurchaseStatus == false) && (this.IsUpdateShippingFixedPurchase)) { %>
						<asp:CheckBox ID="cbIsUpdateFixedPurchaseByOrderShippingInfo" Text="今後の定期注文にも反映させる" Checked="false" runat="server"/>
					<% } %>
					</dl>
					<div class="button-next">
						<asp:LinkButton Text="情報更新" runat="server" CommandArgument="<%# Container.ItemIndex %>"  ValidationGroup="OrderShipping" OnClientClick="return AlertDataChange('Shipping', this);" OnClick="lbUpdateUserShippingInfo_Click" CssClass="btn" ></asp:LinkButton>
						<input type="hidden" id="parentShippingRepeater" name="parentShippingRepeater" value="<%#: Container.UniqueID %>" />
					</div>
					<br />
					<div class="button-prev">
						<asp:LinkButton Text="キャンセル" runat="server" CommandArgument="<%# Container.ItemIndex %>" OnClientClick="return exec_submit();" OnClick="lbHideUserShippingInfoForm_Click" CssClass="btn" ></asp:LinkButton>
					</div>
				</div>
				<div id="divOrderShippingUpdateExecFroms" style="display: none"> 
						更新中です...
				</div>
				<div runat="server" style="display:none;">
					<span class="error_inline"><%#: WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_GROCERY_STORE) %></span>
				</div>
				<small id="sErrorMessageShipping" runat="server" class="attention" style="padding: 2px;"></small>
			</div>
			<dl class="order-form">
				<% if ((this.OrderModel.Shippings[0].ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF)
					|| (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false)) { %>
				<dt>配送方法</dt>
				<dd>
					<%#: ValueText.GetValueText(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD, GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD)) %>
				</dd>
				<% } %>
				<dt>配送サービス</dt>
				<dd>
					<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_NAME) %>
				</dd>
				<div visible='<%# (string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD) != Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL %>' runat="server">
					<% if ((this.OrderModel.Shippings[0].ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF)
						|| (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false)) { %>
					<div visible='<%# (string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_FLG) == Constants.FLG_SHOPSHIPPING_SHIPPING_DATE_SET_FLG_VALID %>'  runat="server">
						<dt>配送希望日</dt>
						<dd class="button" runat="server" visible="<%# this.IsModifyShippingDates[Container.ItemIndex] %>">
							<asp:LinkButton ID="lbDisplayInputShippingDate" CommandArgument="<%# Container.ItemIndex %>" Text="配送希望日変更" runat="server" OnClick="lbDisplayInputShippingDate_Click" CssClass="btn" />
						</dd>
						<dd>
							<asp:DropDownList ID="ddlShippingDateList" Visible="false" runat="server" DataSource="<%# GetListShippingDate() %>" DataTextField="text" DataValueField="value"></asp:DropDownList>
							<div id="dvShippingDateText" runat="server" ><%#: DateTimeUtility.ToStringFromRegion(GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE), DateTimeUtility.FormatType.LongDateWeekOfDay2Letter, ReplaceTag("@@DispText.shipping_date_list.none@@")) %></div>
							<br />
							<asp:Label ID="lShippingDateErrorMessage" CssClass="attention" runat="server"></asp:Label>
						</dd>
					</div>
					<% } %>
					<div id="trShippingDateInput" visible='false'  runat="server">
						<br />
						<div id="divShippingDateUpdateButtons" style="display: block"> 
							<div class="button-next">
								<asp:LinkButton Text="情報更新" CommandArgument="<%# Container.ItemIndex %>" OnClientClick="return AlertUpdateShippingDate(this);" runat="server" OnClick="lbUpdateShippingDate_Click" CssClass="btn" />
								<input type="hidden" id="parentShippingDateRepeater" name="parentShippingDateRepeater" value="<%#: Container.UniqueID %>" />
							</div>
							<br />
							<div class="button-prev">
								<asp:LinkButton Text="キャンセル" CommandArgument="<%# Container.ItemIndex %>" runat="server" OnClick="lbHideShippingDate_Click" CssClass="btn" />
							</div>
						</div>
						<div id="divShippingDateUpdateExecFroms" style="display: none"> 
							更新中です...
						</div>
						<small id="sErrorMessageShippingDate" runat="server" class="error" style="padding: 2px;"></small>
					</div>
					<div runat="server" visible="<%# this.DisplayScheduledShippingDate %>">
						<dt>出荷予定日</dt>
						<dd>
							<%#: DateTimeUtility.ToStringFromRegion(GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE), DateTimeUtility.FormatType.LongDateWeekOfDay2Letter, ReplaceTag("@@DispText.shipping_date_list.none@@")) %>
						</dd>
					</div>
					<div visible='<%# (string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG) == Constants.FLG_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG_VALID %>'  runat="server">
						<dt>配送希望時間帯</dt>
						<dd class="button" runat="server" visible="<%# this.IsModifyShippingTimes[Container.ItemIndex] %>">
							<asp:LinkButton ID="lbDisplayInputShippingTime" CommandArgument="<%# Container.ItemIndex %>" Text="配送時間帯変更" runat="server" OnClick="lbDisplayInputShippingTime_Click" CssClass="btn" />
						</dd>
						<dd>
							<asp:DropDownList ID="ddlShippingTimeList" Visible="false" runat="server" DataSource='<%# GetShippingTimeList((string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID)) %>' DataTextField="text" DataValueField="value"></asp:DropDownList>
							<div id="dvShippingTimeText"  runat="server">
								<%#: ((w2.Common.Util.Validator.IsNullEmpty(GetKeyValue(((Hashtable)Container.DataItem)["row"], "shipping_time_message")) == false)
									&& (StringUtility.ToEmpty(GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG)) == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF))
									? GetKeyValue(((Hashtable)Container.DataItem)["row"], "shipping_time_message")
									: ReplaceTag("@@DispText.shipping_time_list.none@@") %></div>
						</dd>
					</div>
					<div id="trShippingTimeInput" visible='false'  runat="server">
						<br />
						<div id="divShippingTimeUpdateButtons" style="display: block"> 
							<div class="button-next">
								<asp:LinkButton Text="情報更新" CommandArgument="<%# Container.ItemIndex %>" OnClientClick="return AlertUpdateShippingTime(this);" runat="server" OnClick="lbUpdateShippingTime_Click" CssClass="btn" />
								<input type="hidden" id="parentShippingTimeRepeater" name="parentShippingTimeRepeater" value="<%#: Container.UniqueID %>" />
							</div>
							<br />
							<div class="button-prev">
								<asp:LinkButton Text="キャンセル" CommandArgument="<%# Container.ItemIndex %>" runat="server" OnClick="lbHideShippingTime_Click" CssClass="btn" />
							</div>
						</div>
						<div id="divShippingTimeUpdateExecFroms" style="display: none"> 
							更新中です...
						</div>
						<small id="sErrorMessageShippingTime" runat="server" class="error" style="padding: 2px;"></small>
					</div>
				</div>
				<dt visible='<%# (string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO) != "" %>' runat="server">配送伝票番号</dt>
				<dd visible='<%# (string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO) != "" %>' runat="server">
					<%# WebSanitizer.HtmlEncode(GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO)) %>
					<% if(Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED && IsShowCheckDeliveryStatus()) { %>
						<button><a href="javascript:openSiteMapPopup('<%# StringUtility.ToEmpty(GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO)) %>');">配送状況確認</a></button>
					<% } %>
				</dd>
				<% if (this.IsShowTaiwanOrderInvoiceInfo) { %>
					<dt>発票番号</dt>
					<dd><%: this.TwOrderInvoiceModel.TwInvoiceNo %></dd>
					<dt>発票種類</dt>
					<dd>
						<%: ValueText.GetValueText(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE, this.TwOrderInvoiceModel.TwUniformInvoice) %>
						<% if (this.TwOrderInvoiceModel.TwUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_COMPANY) { %>
							<p style="padding-bottom: 5px; padding-top: 15px;">統一編号</p>
							<%: this.TwOrderInvoiceModel.TwUniformInvoiceOption1 %>
							<p style="padding-bottom: 5px; padding-top: 5px;">会社名</p>
							<%: this.TwOrderInvoiceModel.TwUniformInvoiceOption2 %>
						<% } %>
						<% if (this.TwOrderInvoiceModel.TwUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_DONATE) { %>
							<p style="padding-bottom: 5px; padding-top: 15px;">寄付先コード</p>
							<%: this.TwOrderInvoiceModel.TwUniformInvoiceOption1 %>
						<% } %>
					</dd>
					<% if (this.TwOrderInvoiceModel.TwUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL) { %>
						<dt>共通性載具</dt>
						<dd>
							<%: ValueText.GetValueText(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE, this.TwOrderInvoiceModel.TwCarryType) %>
							<br />
							<%: this.TwOrderInvoiceModel.TwCarryTypeOption %>
						</dd>
					<% } %>
				<% } %>
			</dl>
			<% } else if (this.RealShopModel != null) { %>
			<br />
			<dl class="order-form">
				<dt>受取店舖</dt>
				<dd><%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME) %></dd>
				<dt>店舖住所</dt>
				<dd>
				<th>店舖住所</th>
					<%#: "〒" + GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP) %>
					<br />
					<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1) %>
					<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2) %><br />
					<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3) %><br />
					<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4) %><br />
					<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5) %>
				</dd>
				<dt>営業時間</dt>
				<dd><%: this.RealShopModel.OpeningHours %></dd>
				<dt>店舖電話番号</dt>
				<dd><%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %></dd>
				<dt>配送方法</dt>
				<dd>店舗受取</dd>
			</dl>
			<% } %>
			</dl>
			<% } %>
			<% if (this.HasSubscriptionBoxItemModify) { %>
				<div class="error">
					<%: WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SUBSCRIPTION_BOX_ERROR_MESSAGE) %>
				</div>
			<% } %>
			<%-- 購入商品一覧 --%>
			<h3>ご注文商品</h3>
		<div class="dvOrderHistoryProduct" ID="dvOrderHistoryProduct" runat="server">
				<asp:Repeater ID="rOrderShippingItem" DataSource='<%# ((Hashtable)Container.DataItem)["childs"] %>' Runat="server">
				<HeaderTemplate>
					<dl id="dlOrderItems" class="order-form order-history-product">
				</HeaderTemplate>
				<ItemTemplate>
					<%-- 通常商品 --%>
					<dd class='<%# IsLastItemOnOrderLine((DataRowView)Container.DataItem) == false ? "order-history-detail-line order-history-detail-item" : "order-history-detail-item" %>' style="<%# IsOrderItemSubscriptionBoxFixedAmount(Eval(Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT_WITH_PREFIX)) == false ? "grid-column: 1/3;" : string.Empty %>">
						<ul>
							<li><%#: Eval(Constants.FIELD_ORDERITEM_VARIATION_ID) %></li>
							<li>
								<%-- 一致する商品IDが現在も存在する場合、商品詳細ページへのリンクを表示する --%>
								<a id="A1" href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailVariationUrl(Container.DataItem)) %>' runat="server" Visible="<%# IsProductDetailLinkValid((DataRowView)Container.DataItem) %>">
									<%#: Eval(Constants.FIELD_ORDERITEM_PRODUCT_NAME) %>
								</a>
								<%# (IsProductDetailLinkValid((DataRowView)Container.DataItem) == false) ? WebSanitizer.HtmlEncode(Eval(Constants.FIELD_ORDERITEM_PRODUCT_NAME)) : ""%><%-- 商品名 --%>
								<span id="Span2" visible='<%# (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS) != "" %>' runat="server">
								<br />
								<%#: ProductOptionSettingHelper.GetDisplayProductOptionTexts((string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS)).Replace("　", "\r\n") %>
								</span>
							</li>
							<li>
								<asp:Repeater ID="Repeater1" DataSource='<%# this.OrderItemSerialKeys[((string)Eval(Constants.FIELD_ORDER_ORDER_ID)) + (Eval(Constants.FIELD_ORDERITEM_ORDER_ITEM_NO).ToString())] %>' runat="server">
								<ItemTemplate>
									<br />
									&nbsp;シリアルキー:&nbsp;<%# Eval(Constants.FIELD_SERIALKEY_SERIAL_KEY)%>
								</ItemTemplate>
								</asp:Repeater>
							</li>
							<li>
								<% if ((this.OrderModel.IsSubscriptionBoxFixedAmount == false) && (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false)) { %>
								<%#: CurrencyManager.ToPrice(Eval(Constants.FIELD_ORDERITEM_PRODUCT_PRICE)) %>（<%#: this.ProductPriceTextPrefix %>）&nbsp;x&nbsp;
								<% } %>
								<% if ((this.OrderModel.IsSubscriptionBoxFixedAmount == false) && (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED)) { %>
									<%#: ProductOptionSettingHelper.ToDisplayProductOptionPriceAndPrefix(
											(decimal)Eval(Constants.FIELD_ORDERITEM_PRODUCT_PRICE),
											ProductOptionSettingHelper.ExtractOptionPriceFromProductOptionTexts((string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS)),
											this.ProductPriceTextPrefix,
											ProductOptionSettingHelper.HasOptionPrice((string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS))) %> &nbsp;x&nbsp;
								<% } %>
								<%#: StringUtility.ToNumeric(Eval(Constants.FIELD_ORDERITEM_ITEM_QUANTITY)) + (this.OrderModel.IsSubscriptionBoxFixedAmount ? "個" : "") %>
								<span Visible="<%# (IsOrderItemSubscriptionBoxFixedAmount(Eval(Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT_WITH_PREFIX)) == false) %>" runat="server">
									<%#: CurrencyManager.ToPrice(Eval(Constants.FIELD_ORDERITEM_PRODUCT_PRICE)) %>（<%#: this.ProductPriceTextPrefix %>）&nbsp;x&nbsp;
								</span>
								<%#: StringUtility.ToNumeric(Eval(Constants.FIELD_ORDERITEM_ITEM_QUANTITY)) + (IsOrderItemSubscriptionBoxFixedAmount(Eval(Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT_WITH_PREFIX)) ? "個" : "") %>
								<span Visible="<%# IsOrderItemSubscriptionBoxFixedAmount(Eval(Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT_WITH_PREFIX)) == false %>" runat="server">
									&nbsp;=&nbsp;
									<%#: CurrencyManager.ToPrice(Eval(Constants.FIELD_ORDERITEM_ITEM_PRICE)) %>
								</span>
								<asp:HiddenField runat="server" ID="hfOrderItemProductPrice" Value="<%# Eval(Constants.FIELD_ORDERITEM_ITEM_PRICE) %>" />
							</li>
						</ul>
					</dd>
					<dd Visible="<%# DisplaySubscriptionBoxFixedAmountCourse((DataRowView)Container.DataItem) %>" class="fixed-amount-course-wrap" style='<%# GetFixedAmountCourseAreaGrid((DataRowView)Container.DataItem) %>' runat="server">
						<ul>
							<li>
								頒布会コース名：<%#: GetSubscriptionBoxDisplayName((string)Eval(Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID_WITH_PREFIX)) %>
							</li>
							<li>
								定額：<%#: CurrencyManager.ToPrice(Eval(Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT_WITH_PREFIX)) %>（<%#: this.ProductPriceTextPrefix %>）
							</li>
						</ul>
					</dd>
					<%-- セットプロモーション商品 --%>
					<dd id="Dd1" visible='<%# (StringUtility.ToEmpty(Eval(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO)) != "") %>' class="set-promotion-container" runat="server">
						<ul>
							<li><%#: Eval(Constants.FIELD_ORDERITEM_VARIATION_ID) %></li>
							<li>
								<%-- 一致する商品IDが現在も存在する場合、商品詳細ページへのリンクを表示する --%>
								<a href='<%#: CreateProductDetailVariationUrl(Container.DataItem) %>' runat="server" Visible="<%# IsProductDetailLinkValid((DataRowView)Container.DataItem) %>">
									<%#: Constants.FIELD_ORDERITEM_PRODUCT_NAME%>
								</a>
								<%#: (IsProductDetailLinkValid((DataRowView)Container.DataItem) == false) ? Constants.FIELD_ORDERITEM_PRODUCT_NAME : ""%><%-- 商品名 --%>
								<span visible='<%# (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS) != "" %>' runat="server">
									<br />
									<%#: ProductOptionSettingHelper.GetDisplayProductOptionTexts((string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS)).Replace("　", "\r\n") %>
								</span>
							</li>
							<li>
								<asp:Repeater ID="Repeater2" DataSource='<%# this.OrderItemSerialKeys[((string)Eval(Constants.FIELD_ORDER_ORDER_ID)) + (Eval(Constants.FIELD_ORDERITEM_ORDER_ITEM_NO).ToString())] %>' runat="server">
								<ItemTemplate>
									<br />
									&nbsp;シリアルキー:&nbsp;<%# Eval(Constants.FIELD_SERIALKEY_SERIAL_KEY)%>
								</ItemTemplate>
								</asp:Repeater>
							</li>
							<li>
								<% if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false) { %>
									<%#: CurrencyManager.ToPrice(Eval(Constants.FIELD_ORDERITEM_PRODUCT_PRICE)) %>（<%#: this.ProductPriceTextPrefix %>）&nbsp;x&nbsp;
								<% } %>
								<% if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED) { %>
									<%#: ProductOptionSettingHelper.ToDisplayProductOptionPriceAndPrefix(
											(decimal)Eval(Constants.FIELD_ORDERITEM_PRODUCT_PRICE),
											ProductOptionSettingHelper.ExtractOptionPriceFromProductOptionTexts((string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS)),
											this.ProductPriceTextPrefix,
											ProductOptionSettingHelper.HasOptionPrice((string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS))) %> &nbsp;x&nbsp;
								<% } %>
								<%#: StringUtility.ToNumeric(Eval(Constants.FIELD_ORDERITEM_ITEM_QUANTITY)) %>
							</li>
							<li visible='<%# (StringUtility.ToEmpty(Eval(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO)) != "") && (int)Eval(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_ITEM_NO) == GetSetPromotionRowSpan(Container.DataItem, ((Repeater)Container.Parent).DataSource) %>' runat="server">
								<%#: Eval(Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME) %>
							</li>
							<li style="text-decoration: line-through; " visible='<%# (StringUtility.ToEmpty(Eval(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO)) != "") && ((string)Eval(Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_FLG) == Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_ON) %>' runat="server">
								<%#: CurrencyManager.ToPrice(Eval(Constants.FIELD_ORDERSETPROMOTION_UNDISCOUNTED_PRODUCT_SUBTOTAL)) %>
							</li>
							<li>
								<%#: CurrencyManager.ToPrice(((StringUtility.ToEmpty(Eval(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO)) != "") ? (decimal)Eval(Constants.FIELD_ORDERSETPROMOTION_UNDISCOUNTED_PRODUCT_SUBTOTAL) : 0) - ((StringUtility.ToEmpty(Eval(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO)) != "") ? (decimal)Eval(Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_AMOUNT) : 0)) %>
							</li>
						</ul>
					</dd>
					</ItemTemplate>
					<FooterTemplate>
						</dl>
					</FooterTemplate>
				</asp:Repeater>
		</div>
					<%-- 追加商品一覧 --%>
		<div class="dvOrderHistoryProduct" ID="dvOrderModifyInput" Visible="False" runat="server">
		<asp:Repeater ID="rOrderInputShippingItem" ItemType="w2.App.Common.Input.Order.OrderItemInput" OnItemDataBound="rOrderInputShippingItem_OnItemDataBound" Runat="server">
			<HeaderTemplate>
					<dl class="order-form order-history-product">
				</HeaderTemplate>
				<ItemTemplate>
					<%-- 通常商品 --%>
					<dd>
						<ul>
							<li><%#: Item.VariationId %></li>
							<li>
								<%#: Item.ProductName %>
								<span visible='<%# string.IsNullOrEmpty(Item.ProductOptionTexts) == false %>' runat="server">
								<br />
								<%#: ProductOptionSettingHelper.GetDisplayProductOptionTexts(Item.ProductOptionTexts).Replace("　", "\r\n") %>
								</span>
							</li>
							<li>
								<asp:Repeater DataSource='<%# this.OrderItemSerialKeys[Item.OrderId + Item.OrderItemNo.ToString()] %>' runat="server">
								<ItemTemplate>
									<br />
									&nbsp;シリアルキー:&nbsp;<%# Eval(Constants.FIELD_SERIALKEY_SERIAL_KEY)%>
								</ItemTemplate>
								</asp:Repeater>
							</li>
							<li>
								<span Visible="<%# IsOrderItemSubscriptionBoxFixedAmount(Item.SubscriptionBoxFixedAmount) == false %>" runat="server">
									<%#: CurrencyManager.ToPrice(Item.ProductPrice) %>（<%#: this.ProductPriceTextPrefix %>）&nbsp;x&nbsp;
								</span>
								<asp:DropDownList ID="ddlProductCount"
											data-productId="<%# Item.ProductId %>"
											data-variationId="<%# Item.VariationId %>"
											data-subscriptionBoxId="<%# Item.SubscriptionBoxCourseId %>"
											data-beforeCount="<%# Item.ItemQuantity %>"
											AutoPostBack="True"
											OnSelectedIndexChanged="ddlProductCount_SelectedIndexChanged"
											runat="server"
											DataTextField="Text"
											DataValueField="Value"
											style="width: 50px; text-align: right;"
											Visible="<%# GetCanModifiyProductCount(Item) %>" />
								<asp:Literal Text="<%# Item.ItemQuantity.ToString() %>" Visible="<%# GetCanModifiyProductCount(Item) == false %>" runat="server"></asp:Literal>
								<asp:HiddenField ID="hfModifyProductId" Value='<%# Item.ProductId %>' runat="server"/>
								<%-- 商品変更数選択肢（1から設定分表示） --%>
								<asp:HiddenField ID="hfMaxItemQuantity" runat="server" Value="10" />
								<asp:HiddenField ID="hfItemQuantity" runat="server" Value="<%#: Item.ItemQuantity %>" />
								<%#: (this.OrderModel.IsSubscriptionBoxFixedAmount ? "個" : "") %>
								<% if (this.OrderModel.IsSubscriptionBoxFixedAmount == false) { %>
									&nbsp;=&nbsp;
									<%#: CurrencyManager.ToPrice(Item.ItemPrice) %>
								<% } %>
								<asp:HiddenField runat="server" ID="hfOrderItemProductPrice" Value="<%# Item.ItemPrice %>" />
							</li>
							<li>
								<% if (this.IsProductModify) { %>
									<asp:CheckBox
										ID="cbDeleteProduct"
										Visible="<%# GetCanDeleteProduct(Item) %>"
										OnCheckedChanged="cbDeleteProduct_OnCheckedChanged"
										data-productId="<%# Item.ProductId %>"
										data-variationId="<%# Item.VariationId %>"
										data-subscriptionBoxId="<%# Item.SubscriptionBoxCourseId %>"
										AutoPostBack="True"
										Checked="<%# Item.ModifyDeleteTarget %>"
										runat="server"/>
								<% } %>
							</li>
						</ul>
					</dd>

					<%-- セットプロモーション商品 --%>
					<dd id="Dd1" visible='<%# (StringUtility.ToEmpty(Item.OrderSetpromotionNo) != "") %>' runat="server">
						<ul>
							<li><%#: Item.VariationId %></li>
							<li>
								<%#: Item.ProductName %>
								<span visible='<%# string.IsNullOrEmpty(Item.ProductOptionTexts) == false %>' runat="server">
									<br />
									<%#: ProductOptionSettingHelper.GetDisplayProductOptionTexts(Item.ProductOptionTexts).Replace("　", "\r\n") %>
								</span>
							</li>
							<li>
								<asp:Repeater DataSource='<%# this.OrderItemSerialKeys[Item.OrderId + Item.OrderItemNo.ToString()] %>' runat="server">
								<ItemTemplate>
									<br />
									&nbsp;シリアルキー:&nbsp;<%# Eval(Constants.FIELD_SERIALKEY_SERIAL_KEY)%>
								</ItemTemplate>
								</asp:Repeater>
							</li>
							<li>
								<%#: CurrencyManager.ToPrice(Item.ProductPrice) %>（<%#: this.ProductPriceTextPrefix %>）&nbsp;x&nbsp;
								<%#: StringUtility.ToNumeric(Item.ItemQuantity) %>
							</li>
							<li visible='<%# (StringUtility.ToEmpty(Item.OrderSetpromotionNo) != "") && (string.IsNullOrEmpty(StringUtility.ToEmpty(Item.OrderSetpromotionNo)) ? -1 : int.Parse(Item.OrderSetpromotionItemNo)) == GetSetPromotionRowSpan(Item, ((Repeater)Container.Parent).DataSource) %>' runat="server">
								<%#: GetSetPromotionDispName(Item) %>
							</li>
							<li style="text-decoration: line-through; " visible='<%# (StringUtility.ToEmpty(Item.OrderSetpromotionNo) != "") && (HasSetPromotionDiscount(Item, Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_FLG)) %>' runat="server">
								<%#: CurrencyManager.ToPrice(GetSetPromotionDiscount(Item, Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_FLG, true)) %>
							</li>
							<li>
								<%#: CurrencyManager.ToPrice(GetSetPromotionDiscount(Item, Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_FLG, false)) %>
							</li>
						</ul>
					</dd>
					</ItemTemplate>
					<FooterTemplate>
						</dl>
					</FooterTemplate>
				</asp:Repeater>
		</div>
		</ItemTemplate>
	</asp:Repeater>
	<div class="productModifyBtn" style="padding-bottom: 30px;" runat="server" >
		<small id="sModifyErrorMessage" runat="server" class="notProductError"></small>
		<small id="sNoveltyChangeNoticeMessage" runat="server" class="notProductError"></small>
		<asp:LinkButton id ="btnModifyProducts" CssClass="btn right" OnClick="btnModifyProducts_OnClick" Visible="<%# this.IsOrderModifyBtnDisplay %>" runat="server">お届け商品数変更</asp:LinkButton>
		<asp:LinkButton id ="btnModifyConfirm" OnClientClick="return AlertProductModify()" CssClass="btn right" OnClick="btnModifyConfirm_OnClick" Visible="False" runat="server">更新</asp:LinkButton>
		<asp:LinkButton id ="btnModifyCancel" CssClass="btn right" OnClick="btnModifyCancel_OnClick" Visible="False" runat="server">キャンセル</asp:LinkButton>
	</div>

	<%-- 合計情報 --%>
	<div class="cart-unit">
		<dl>
		<dt>商品合計</dt>
		<dd>
			<asp:HiddenField runat="server" ID="hfOrderPriceSubtotal" Value="<%# CurrencyManager.ToPrice(this.OrderModel.OrderPriceTotal) %>"/>
			<asp:HiddenField runat="server" ID="hfOrderPriceSubtotalNew" Value="<%# CurrencyManager.ToPrice(this.OrderModel.OrderPriceTotal) %>"/>
			<asp:HiddenField runat="server" ID="hfProducts" Value="<%#: SetProduct(0) %>"/>
			<asp:HiddenField runat="server" ID="hfCounts" Value="<%#: SetProduct(1) %>"/>
			<asp:Literal runat="server" ID="lOrderPriceSubtotal" Text="<%#: CurrencyManager.ToPrice(this.OrderModel.OrderPriceSubtotal) %>"/>
		</dd>
		</dl>
		<%if (this.ProductIncludedTaxFlg == false) { %>
			<dl>
				<dt>消費税</dt>
				<dd>
					<asp:Literal runat="server" ID="lOrderPriceSubtptalTax" Text="<%#: CurrencyManager.ToPrice(this.OrderModel.OrderPriceSubtotalTax) %>"/>
				</dd>
			</dl>
		<%} %>
		<asp:Repeater DataSource="<%# this.OrderSetPromotions %>" runat="server">
		<ItemTemplate>
		<dl>
		<dt class="row" visible="<%# (string)((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_FLG] == Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_ON %>" runat="server">
			「<%#: ((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME] %>」割引額
		</dt>
		<dd>
			<%# ((decimal)((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_AMOUNT] > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(((decimal)((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_AMOUNT])) %>
		</dd>
		</dl>
		</ItemTemplate>
		</asp:Repeater>
		<dl>
		<dt>配送料金</dt>
		<dd visible="<%# (this.OrderModel.ShippingPriceSeparateEstimatesFlg == Constants.FLG_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_INVALID) %>" runat="server">
			<asp:Literal runat="server" ID="lOrderPriceShipping" Text="<%#: CurrencyManager.ToPrice(this.OrderModel.OrderPriceShipping) %>"/>
		</dd>
		</dl>
		<dl visible="<%# (this.OrderModel.ShippingPriceSeparateEstimatesFlg == Constants.FLG_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_VALID) %>" runat="server">
		<dt></dt>
		<dd>
			<%#: this.ShopShippingModel.ShippingPriceSeparateEstimatesMessage %>
		</dd>
		</dl>
		<asp:Repeater DataSource="<%# this.OrderSetPromotions %>" runat="server">
		<ItemTemplate>
		<dl>
		<dt visible="<%# (string)((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_FREE_FLG] == Constants.FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_ON %>" runat="server">
			「<%#: ((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME] %>」割引額(送料割引)
		</dt>
		<dd visible="<%# (string)((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_FREE_FLG] == Constants.FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_ON %>" runat="server">
			<%#: ((decimal)((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT] > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice((decimal)((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT]) %>
		</dd>
		</dl>
		</ItemTemplate>
		</asp:Repeater>
		<dl>
		<dt>決済手数料</dt>
		<dd>
			<asp:Literal runat="server" ID="lOrderPriceExchange" Text="<%#: CurrencyManager.ToPrice(this.OrderModel.OrderPriceExchange) %>"/>
		</dd>
		</dl>
		<asp:Repeater DataSource="<%# this.OrderSetPromotions %>" runat="server">
		<ItemTemplate>
		<dl>
		<dt visible="<%# (string)((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_FREE_FLG] == Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_ON %>" runat="server">
				「<%#: ((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME] %>」割引額(決済手数料割引)
		</dt>
		<dd visible="<%# (string)((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_FREE_FLG] == Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_ON %>" runat="server">
			<%#: ((decimal)((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT] > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice((decimal)((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT]) %>
		</dd>
		</dl>
		</ItemTemplate>
		</asp:Repeater>

		<%-- 会員ランク情報リスト(有効な場合) --%>
		<%if (Constants.MEMBER_RANK_OPTION_ENABLED) { %>
		<dl>
			<dt>会員ランク割引額</dt>
			<dd>
				<asp:Literal runat="server" ID="lOrderMemberRankDiscountPrice" Text='<%#: ((this.OrderModel.MemberRankDiscountPrice > 0) ? "-" : "") + CurrencyManager.ToPrice(this.OrderModel.MemberRankDiscountPrice) %>'/>
			</dd>
		</dl>
		<% } %>
		<%-- 定期会員割引額(有効な場合) --%>
		<%if (Constants.MEMBER_RANK_OPTION_ENABLED && Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
		<dl>
			<dt>定期会員割引額</dt>
			<dd>
				<asp:Literal runat="server" ID="lFixedPurchaseMemberDiscountAmount" Text='<%#: ((this.OrderModel.FixedPurchaseMemberDiscountAmount > 0) ? "-" : "") + CurrencyManager.ToPrice(this.OrderModel.FixedPurchaseMemberDiscountAmount) %>'/>
			</dd>
		</dl>
		<%} %>
		<%-- ポイント情報リスト(有効な場合) --%>
		<%if (Constants.W2MP_POINT_OPTION_ENABLED) { %>
		<dl>
			<dt>ポイント利用額</dt>
			<dd>
				<asp:Literal runat="server" ID="lOrderPointUseYen" Text='<%#: ((this.OrderModel.OrderPointUseYen > 0) ? "-" : "") + CurrencyManager.ToPrice(this.OrderModel.OrderPointUseYen) %>'/>
			</dd>
		</dl>
		<%} %>
		<%-- クーポン情報リスト(有効な場合) --%>
		<%if (Constants.W2MP_COUPON_OPTION_ENABLED) { %>
		<dl>
			<dt>クーポン割引額</dt>
			<dd>
				<asp:Literal runat="server" ID="lCouponName" Text="<%#: GetCouponName(this.OrderModel) %>"/>
				<asp:Literal runat="server" ID="lOrderCouponUse" Text='<%#: ((this.OrderModel.OrderCouponUse > 0) ? "-" : "") + CurrencyManager.ToPrice(this.OrderModel.OrderCouponUse) %>'/>
			</dd>
		</dl>
		<%} %>
		<%-- 定期購入割引(有効な場合) --%>
		<%if (this.IsFixedPurchase) { %>
		<dl runat="server">
			<dt>定期購入割引額</dt>
			<dd>
				<asp:Literal runat="server" ID="lFixedPurchaseDiscountPrice" Text='<%#: ((this.OrderModel.FixedPurchaseDiscountPrice > 0) ? "-" : "") + CurrencyManager.ToPrice(this.OrderModel.FixedPurchaseDiscountPrice) %>'/>
			</dd>
		</dl>
		<%} %>
		<dl runat="server" visible='<%# (this.OrderModel.OrderPriceRegulation != 0) %>'>
			<dt>調整金額</dt>
			<dd>
				<asp:Literal runat="server" ID="lOrderPriceRegulation" Text='<%#: ((this.OrderModel.OrderPriceRegulation < 0) ? "-" : "") + CurrencyManager.ToPrice(Math.Abs(this.OrderModel.OrderPriceRegulation)) %>'/>
			</dd>
		</dl>

		<dl>
		<dt>総合計(税込)</dt>
		<dd>
			<asp:Literal runat="server" ID="lOrderPriceTotal" Text="<%#: CurrencyManager.ToPrice(this.OrderModel.OrderPriceTotal) %>"/>
		</dd>
		</dl>
		<%if (Constants.GLOBAL_OPTION_ENABLE) { %>
		<dl>
		<dt>決済金額（税込）</dt>
		<dd><%#: this.SendingAmount %></dd>
		</dl>
		<% } %>
	</div>
	</div>
</div>

	<div class="order-footer">
		<div class="button-next">
			<a href="<%: GetBackBtnPath() %>" class="btn">購入履歴一覧へ</a>
			<a href="<%# GetBackBtnPath(this.OrderModel.FixedPurchaseId) %>" style="margin-top: 1em" class="btn btn-large" Visible="<%# this.IsFixedPurchase %>" runat="server">定期購入履歴詳細へ</a>
		</div>
	</div>
</section>

<asp:HiddenField ID="hfTotalPrice" runat="server" />
<asp:HiddenField ID="hfPaymentNameSelected" runat="server" />
<asp:HiddenField ID="hfPaymentIdSelected" runat="server" />
<asp:HiddenField ID="hfPaymentTotalPriceNew" runat="server" />
<asp:HiddenField ID="hfShippingTotalPriceNew" runat="server" />
<asp:HiddenField ID="hfConfirmSenderId" runat="server"/>
<asp:HiddenField ID="hfIsCheckFixedPurchaseFirstTime" runat="server"/>
<asp:HiddenField ID="hfShowModal" runat="server"/>
<asp:HiddenField ID="hfFixedPurchasePaymentId" Value="<%# this.FixedPurchaseModel.OrderPaymentKbn %>" runat="server"/>
<asp:HiddenField ID="hfShippingAddress" runat="server"/>
</ContentTemplate>
</asp:UpdatePanel>
<%-- UpdatePanel終了 --%>
<uc:Loading id="ucLoading" UpdatePanelReload="True" runat="server" />

<uc:PaymentYamatoKaSmsAuthModal ID="ucPaymentYamatoKaSmsAuthModal" runat="server"/>

<%--▼▼ Amazon Pay(CV2)スクリプト ▼▼--%>
<script src="https://static-fe.payments-amazon.com/checkout.js"></script>
<%--▲▲ Amazon Pay(CV2)スクリプト ▲▲--%>
<script type="text/javascript">
	<%-- UpdataPanelの更新時のみ処理を行う --%>
	function bodyPageLoad() {
		if (Sys.WebForms == null) return;
		var isAsyncPostback = Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();
		if (isAsyncPostback) {
			bindEvent();
			window.exec_submit_flg = 0;

			if (document.getElementById("<%= hfConfirmSenderId.ClientID %>").value != "") {
				document.getElementById('divOrderShippingUpdateButtons').style.display = "none";
				document.getElementById('divOrderShippingUpdateExecFroms').style.display = "block";
				if (confirm('<%: WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_NOT_FIXED_PRODUCT_ORDER_LIMIT) %>' + "\nよろしいですか？")) {
					__doPostBack(document.getElementById("<%= hfConfirmSenderId.ClientID %>").value, "");
				} else {
					document.getElementById("<%= hfConfirmSenderId.ClientID %>").value = "";
					document.getElementById("<%= hfIsCheckFixedPurchaseFirstTime.ClientID %>").value = "";
					document.getElementById('divOrderShippingUpdateButtons').style.display = "block";
					document.getElementById('divOrderShippingUpdateExecFroms').style.display = "none";
				}
			}

			
			if ($("#<%= hfShowModal.ClientID %>").val() === 'true') {
				$("#modal").show();

				if ($('#spantelnum').text() === '') {
					$('#telnuminput').show();
					$('#authcodeinput').hide();
					$("#telnuminput .error-msg").hide();
				}
			} else {
				$("#modal").hide();
			}
		}
		<%--▼▼ Amazon Pay(CV2)スクリプト ▼▼--%>
		showAmazonButton('#AmazonPayCv2Button');
		showAmazonButton('#AmazonPayCv2Button2');
		<%--▲▲ Amazon Pay(CV2)スクリプト ▲▲--%>

		changeOrderItemsAreaLayout();
	}

	<%-- イベントをバインドする --%>
	function bindEvent() {
		bindExecAutoKana();
		bindExecAutoChangeKana();
		bindZipCodeSearch();
		<% if(Constants.GLOBAL_OPTION_ENABLE) { %>
		bindTwAddressSearch();
		<% } %>
	}

	<%-- 氏名（姓・名）の自動振り仮名変換のイベントをバインドする --%>
	function bindExecAutoKana() {
		<% foreach (RepeaterItem ri in rOrderShipping.Items)  { %>
			execAutoKanaWithKanaType(
				$('#<%= ((TextBox)ri.FindControl("tbShippingName1")).ClientID %>'),
				$('#<%= ((TextBox)ri.FindControl("tbShippingNameKana1")).ClientID %>'),
				$('#<%= ((TextBox)ri.FindControl("tbShippingName2")).ClientID %>'),
				$('#<%= ((TextBox)ri.FindControl("tbShippingNameKana2")).ClientID %>'));
		<%} %>
	}

	<%-- ふりがな（姓・名）のかな←→カナ自動変換イベントをバインドする --%>
	function bindExecAutoChangeKana() {
		<% foreach (RepeaterItem ri in rOrderShipping.Items)  { %>
			execAutoChangeKanaWithKanaType(
				$('#<%= ((TextBox)ri.FindControl("tbShippingNameKana1")).ClientID %>'),
				$('#<%= ((TextBox)ri.FindControl("tbShippingNameKana2")).ClientID %>'));
		<%} %>
	}

	<%--▼▼ Amazon Pay(CV2)スクリプト ▼▼--%>
	function showAmazonButton(divId) {
		showAmazonPayCv2Button(
			divId,
			'<%= Constants.PAYMENT_AMAZON_SELLERID %>',
			<%= Constants.PAYMENT_AMAZON_ISSANDBOX.ToString().ToLower() %>,
			'<%= this.AmazonRequest.Payload %>',
			'<%= this.AmazonRequest.Signature %>',
			'<%= Constants.PAYMENT_AMAZON_PUBLIC_KEY_ID %>');
	}
	<%--▲▲ Amazon Pay(CV2)スクリプト ▲▲--%>

	function ValidateAndConfirm(validationGroup, message) {
		if (typeof (Page_ClientValidate) != 'function' || window.Page_ClientValidate(validationGroup)) {
			if (validationGroup === "OrderPayment") {
				if (confirm(message)) {
					doPostbackEvenIfCardAuthFailed = false;
					return true;
				} else {
					doPostbackEvenIfCardAuthFailed = true;
					return false;
				}
			} else {
				return confirm(message);
			}
		}
		else {
			return false;
		}
	}

	function Validate(validationGroup) {
		if (typeof (Page_ClientValidate) != 'function' || window.Page_ClientValidate(validationGroup)) {
			return true;
		}
		else {
			return false;
		}
	}

	//お支払い方法変更時の確認フォーム
	function AlertPaymentChange(priceTotalOld, priceTotalNew, showMessage) {
		var messagePayment;

			messagePayment = 'お支払方法を「' + document.getElementById("<%= hfPaymentNameSelected.ClientID %>").value + '」に変更します。';
		if (showMessage) {
			messagePayment += '\nまた、お支払い方法変更に伴い、請求金額が変更されます。\n\n'
				+ '    変更前の請求金額：' + priceTotalOld + '\n'
				+ '    変更後の請求金額：' + priceTotalNew + '\n\n';
		}

		// 領収書情報が削除される場合、アラート追加
		<% if (Constants.RECEIPT_OPTION_ENABLED
			&& (this.OrderModel.ReceiptFlg == Constants.FLG_ORDER_RECEIPT_FLG_ON)) { %>
		if ("<%= string.Join(",", Constants.NOT_OUTPUT_RECEIPT_PAYMENT_KBN) %>".indexOf(document.getElementById("<%= hfPaymentIdSelected.ClientID %>").value) !== -1)
		{
			messagePayment += '\n指定したお支払い方法は、領収書の発行ができません。\n'
				+ '保存されている「領収書情報」が削除されます。\n\n';
		}
		<% } %>

		messagePayment += 'よろしいですか？\n\n';

		<% if (this.CanDisplayChangeFixedPurchasePayment) { %>
		if (document.getElementById("<%: cbIsUpdateFixedPurchaseByOrderPayment.ClientID %>").checked) {
			messagePayment += "この設定は今後の定期注文につきましても反映されます"
		}
		<% } %>
		var exec = ValidateAndConfirm("OrderPayment", messagePayment);
		if (exec) {
			document.getElementById('divOrderPaymentUpdateButtons').style.display = "none";
			document.getElementById('divOrderPaymentUpdateExecFroms').style.display = "block";
			document.getElementById('<%= sErrorMessagePayment.ClientID %>').style.display = "none";

			onErrorScript = "document.getElementById('divOrderPaymentUpdateButtons').style.display = 'block';"
				+ "document.getElementById('divOrderPaymentUpdateExecFroms').style.display = 'none';"
				+ "document.getElementById('<%= sErrorMessagePayment.ClientID %>').style.display = 'block';";
		}
		return exec;
	}
	//利用ポイント変更時の確認フォーム
	function AlertUpdateOrderPointUse(priceTotalOld, priceTotalNew, showMessage) {
		var messagePoint;

		messagePoint = 'ご利用ポイントを下記に変更します。\n\n    ご利用ポイント： ' + Separate(parseFloat(document.getElementById("<%= tbOrderPointUse.ClientID %>").value)) + '<%: Constants.CONST_UNIT_POINT_PT %>\n\n';
		if (showMessage) {
			messagePoint += 'また、ご利用ポイントの変更に伴い、請求金額が変更されます。\n\n'
				+ '    変更前の請求金額：' + priceTotalOld + '\n'
				+ '    変更後の請求金額：' + priceTotalNew + '\n\n';
		}
		messagePoint += 'よろしいですか？';

		var exec = ValidateAndConfirm("", messagePoint);
		if (exec) {
			document.getElementById('divOrderPointUpdateButtons').style.display = "none";
			document.getElementById('divOrderPointUpdateExecFroms').style.display = "block";
			document.getElementById('<%= slErrorMessagePointUse.ClientID %>').style.display = "none";
		}
		return exec;
	}

	//数値にカンマを付与
	function Separate(num) {
		return String(num).replace(/(\d)(?=(\d\d\d)+(?!\d))/g, '$1,');
	}

	//お届け先変更時の確認フォーム
	function AlertUpdateShippingInfo(priceTotalOld, priceTotalNew, showMessage, e) {
		var parentRepeaterId = $(e.parentNode).children('#parentShippingRepeater').val().replace(/\$/g, '_');
		var tbShippingName1 = $("#" + parentRepeaterId + "_" + "tbShippingName1");
		var tbShippingName2 = $("#" + parentRepeaterId + "_" + "tbShippingName2");
		var tbShippingZip1 = $("#" + parentRepeaterId + "_" + "tbShippingZip1");
		var tbShippingZip2 = $("#" + parentRepeaterId + "_" + "tbShippingZip2");
		var tbShippingZipGlobal = $("#" + parentRepeaterId + "_" + "tbShippingZipGlobal");
		var ddlShippingCountry = $("#" + parentRepeaterId + "_" + "ddlShippingCountry");
		var ddlShippingAddr1 = $("#" + parentRepeaterId + "_" + "ddlShippingAddr1");
		var tbShippingAddr2 = $("#" + parentRepeaterId + "_" + "tbShippingAddr2");
		var tbShippingAddr3 = $("#" + parentRepeaterId + "_" + "tbShippingAddr3");
		var tbShippingAddr4 = $("#" + parentRepeaterId + "_" + "tbShippingAddr4");
		var tbShippingAddr5 = $("#" + parentRepeaterId + "_" + "tbShippingAddr5");
		var ddlShippingAddr5 = $("#" + parentRepeaterId + "_" + "ddlShippingAddr5");
		var tbShippingTel1_1 = $("#" + parentRepeaterId + "_" + "tbShippingTel1_1");
		var tbShippingTel1_2 = $("#" + parentRepeaterId + "_" + "tbShippingTel1_2");
		var tbShippingTel1_3 = $("#" + parentRepeaterId + "_" + "tbShippingTel1_3");
		var tbShippingTel1Global = $("#" + parentRepeaterId + "_" + "tbShippingTel1Global");
		var ddlShippingAddr2 = $("#" + parentRepeaterId + "_" + "ddlShippingAddr2");
		var ddlShippingAddr3 = $("#" + parentRepeaterId + "_" + "ddlShippingAddr3");
		var ddlShippingType = $("#" + parentRepeaterId + "_" + "ddlShippingType");
		var tbShippingZip = $("#" + parentRepeaterId + "_" + "tbShippingZip");
		var tbShippingTel1 = $("#" + parentRepeaterId + "_" + "tbShippingTel1");
		<% if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED) { %>
		var isShopValid = false;
		var isShippingConvenience = ($("#" + parentRepeaterId + "_" + "hfCvsShopFlg").val() == '<%= Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON %>');
		if (isShippingConvenience) {
			var element = document.getElementById(parentRepeaterId);
			var hfCvsShopId = $("#" + parentRepeaterId + "_" + "hfCvsShopId");
			var hfSelectedShopId = $("#" + parentRepeaterId + "_" + "hfSelectedShopId");
			var shopId = hfCvsShopId.val();
			if (shopId == undefined) {
				shopId = hfSelectedShopId.val();
			}
			var dvErrorShippingConvenience = $("#" + parentRepeaterId + "_" + "dvErrorShippingConvenience");
			$.ajax({
				type: "POST",
				url: "<%= Constants.PATH_ROOT + "SmartPhone/" + Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL %>/CheckStoreIdValid",
				data: JSON.stringify({ storeId: shopId }),
				contentType: "application/json; charset=utf-8",
				dataType: "json",
				cache: false,
				async: false,
				success: function (data) {
					if (data.d) {
						isShopValid = true;
						dvErrorShippingConvenience.css("display", "none");
					}
					else {
						dvErrorShippingConvenience.css("display", "");
					}
				}
			});

			return isShopValid;
		}
		<% } %>

		var shippingCountryName = "";
		var shippingAddr1 = "";
		var shippingAddr5 = "";
		var shippingZip = "";
		var shippingTel = "";
		var validateName = "OrderShipping";
		var shippingAddr2 = tbShippingAddr2.val();
		var shippingAddr3 = tbShippingAddr3.val();
		var shippingName = "";
		var shippingAddr = "";

		<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
		shippingCountryName = ddlShippingCountry.children("option:selected").text();

		if (ddlShippingCountry.val() != "<%= Constants.COUNTRY_ISO_CODE_JP %>") {
			shippingAddr2 = (ddlShippingCountry.val() == "<%= Constants.COUNTRY_ISO_CODE_TW %>")
				? ddlShippingAddr2.val()
				: tbShippingAddr2.val();
			shippingAddr3 = (ddlShippingCountry.val() == "<%= Constants.COUNTRY_ISO_CODE_TW %>")
				? ddlShippingAddr3.find('option:selected').text()
				: tbShippingAddr3.val();
			shippingAddr5 = (ddlShippingCountry.val() == "<%= Constants.COUNTRY_ISO_CODE_US %>")
					? ddlShippingAddr5.val()
					: tbShippingAddr5.val();

				shippingZip = tbShippingZipGlobal.val();
				shippingTel = tbShippingTel1Global.val();
				shippingAddr1 = "";

				validateName = "OrderShippingGlobal";

			} else {
				shippingZip = tbShippingZip1.val() + '-' + tbShippingZip2.val();
				shippingTel = tbShippingTel1_1.val() + '-' + tbShippingTel1_2.val() + '-' + tbShippingTel1_3.val();
				shippingAddr1 = ddlShippingAddr1.val();

				// Set value for zip
				if ((tbShippingZip2.val() != "")
					&& (tbShippingZip2.val() != undefined)) {
					shippingZip = tbShippingZip1.val() + '-' + tbShippingZip2.val();
				} else {
					shippingZip = tbShippingZip.val();
				}

				// Set value for telephone
				if ((tbShippingTel1_2.val() != "")
					&& (tbShippingTel1_2.val() != undefined)) {
					shippingTel = tbShippingTel1_1.val() + '-' + tbShippingTel1_2.val() + '-' + tbShippingTel1_3.val();
				} else {
					shippingTel =  tbShippingTel1.val();
				}
			}

		<% } else { %>
		shippingZip = tbShippingZip1.val() + '-' + tbShippingZip2.val();
		shippingTel = tbShippingTel1_1.val() + '-' + tbShippingTel1_2.val() + '-' + tbShippingTel1_3.val();
		shippingAddr1 = ddlShippingAddr1.val();

		// Set value for zip
		if ((tbShippingZip2.val() != "")
			&& (tbShippingZip2.val() != undefined)) {
			shippingZip = tbShippingZip1.val() + '-' + tbShippingZip2.val();
		} else {
			shippingZip = tbShippingZip.val();
		}

		// Set value for telephone
		if ((tbShippingTel1_2.val() != "")
			&& (tbShippingTel1_2.val() != undefined)) {
			shippingTel = tbShippingTel1_1.val() + '-' + tbShippingTel1_2.val() + '-' + tbShippingTel1_3.val();
		} else {
			shippingTel =  tbShippingTel1.val();
		}
		<% } %>

		var messageShipping;
		if (ddlShippingType.val() === 'OWNER') {
			shippingName = '<%#: this.LoginUser.Name %>';
			shippingZip = '<%#: this.LoginUser.Zip %>';
			shippingAddr = '<%#: this.LoginUser.Addr1 %>' +
				' ' +
				'<%#: this.LoginUser.Addr2 %>' +
				' ' +
				'<%#: this.LoginUser.Addr3 %>' +
				'\n' +
				' ' +
				'<%#: this.LoginUser.Addr4%>' +
				' ' +
				'<%#: this.LoginUser.Addr5 %>' +
				' ' +
				'<%#: this.LoginUser.AddrCountryName %>';
			shippingTel = '<%#: this.LoginUser.Tel1 %>';
		} else if (ddlShippingType.val() === 'NEW') {
			shippingName = tbShippingName1.val() + ' ' + tbShippingName2.val();
			shippingAddr = shippingAddr1 +
				' ' +
				shippingAddr2 +
				' ' +
				shippingAddr3 +
				'\n' +
				' ' +
				tbShippingAddr4.val() +
				' ' +
				shippingAddr5 +
				' ' +
				shippingCountryName;
		} else {
			var userShippingAddress = JSON.parse($("#<%= hfShippingAddress.ClientID %>").val());
			shippingName = userShippingAddress.ShippingName;
			shippingZip = userShippingAddress.ShippingZip;
			shippingAddr = userShippingAddress.ShippingAddr1 +
				' ' +
				userShippingAddress.ShippingAddr2 +
				' ' +
				userShippingAddress.ShippingAddr3 +
				'\n' +
				' ' +
				userShippingAddress.ShippingAddr4 +
				' ' +
				userShippingAddress.ShippingAddr5 +
				' ' +
				userShippingAddress.ShippingCountryName;
			shippingTel = userShippingAddress.ShippingTel1;
		}
		messageShipping = 'お届け先を下記に変更します。\n\n'
			+ '    <%: ReplaceTag("@@User.name.name@@") %>： ' + shippingName + '\n'
			+ '    <%: ReplaceTag("@@User.zip.name@@") %>： ' + shippingZip + '\n'
			+ '    <%: ReplaceTag("@@User.addr.name@@") %>： ' + shippingAddr + '\n';

		<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
		var tbShippingCompanyName = $("#" + parentRepeaterId + "_" + "tbShippingCompanyName");
		var tbShippingCompanyPostName = $("#" + parentRepeaterId + "_" + "tbShippingCompanyPostName");
		var shippingCompanyName = '';
		var shippingCompanyPostName = '';
		if (ddlShippingType.val() === 'OWNER') {
			shippingCompanyName = '<%#: this.LoginUser.CompanyName %>';
			shippingCompanyPostName = '<%#: this.LoginUser.CompanyPostName %>';
		} else if (ddlShippingType.val() === 'NEW') {
			shippingCompanyName = tbShippingCompanyName.val();
			shippingCompanyPostName = tbShippingCompanyPostName.val();
		} else {
			shippingCompanyName = userShippingAddress.ShippingCompanyName;
			shippingCompanyPostName = userShippingAddress.ShippingCompanyPostName;
		}

		messageShipping += '    <%: ReplaceTag("@@User.company_name.name@@") %>： ' + shippingCompanyName + '\n'
			+ '    <%: ReplaceTag("@@User.company_post_name.name@@") %>： ' + shippingCompanyPostName + '\n';
		<% } %>

		messageShipping += '    <%: ReplaceTag("@@User.tel1.name@@") %>： ' + shippingTel + '\n\n';

		if (showMessage) {
			messageShipping += 'また、お届け先変更に伴い、請求金額が変更されます。\n\n'
				+ ' 変更前の請求金額：' + priceTotalOld + '\n'
				+ ' 変更後の請求金額：' + priceTotalNew + '\n\n';
		}

		messageShipping += 'よろしいですか？\n\n';

		<% if (this.IsFixedPurchase && (this.FixedPurchaseModel.IsCancelFixedPurchaseStatus == false)) { %>
		if (document.getElementById("<%#: rOrderShipping.Items[0].FindControl("cbIsUpdateFixedPurchaseByOrderShippingInfo").ClientID %>").checked) {
			messageShipping += "この設定は今後の定期注文につきましても反映されます"
		}
		<% } %>

		var exec = true;
		var execUs = true;
		if ((ddlShippingCountry.val() != "<%= Constants.COUNTRY_ISO_CODE_US %>")
			&& (ddlShippingCountry.val() != "<%= Constants.COUNTRY_ISO_CODE_TW %>")) {
			exec = ValidateAndConfirm(validateName, messageShipping);
		} else {
			exec = Validate(validateName);
			execUs = ValidateAndConfirm("OrderHistoryDetailGlobal", messageShipping);
		}

		if (exec && execUs) {
			document.getElementById('divOrderShippingUpdateButtons').style.display = "none";
			document.getElementById('divOrderShippingUpdateExecFroms').style.display = "block";
		}
		return (exec && execUs);
	}

	//配送希望日・時間帯変更時の確認フォーム
	function AlertUpdateShippingDate(e) {
		var parentRepeaterId = $(e.parentNode).children('#parentShippingDateRepeater').val().replace(/\$/g, '_');
		var ddlShippingDateList = $("#" + parentRepeaterId + "_" + "ddlShippingDateList option:selected");
		var messageShippingDate;
		messageShippingDate = '配送希望日を下記に変更します。\n\n'
			+ '    配送希望日： ' + ddlShippingDateList.text() + '\n\n';

		messageShippingDate += 'よろしいですか？';
		var exec = ValidateAndConfirm("OrderShipping", messageShippingDate);
		if (exec) {
			document.getElementById('divShippingDateUpdateButtons').style.display = "none";
			document.getElementById('divShippingDateUpdateExecFroms').style.display = "block";
		}
		return exec;
	}


	//配送希望日・時間帯変更時の確認フォーム
	function AlertUpdateShippingTime(e) {
		var parentRepeaterId = $(e.parentNode).children('#parentShippingTimeRepeater').val().replace(/\$/g, '_');
		var ddlShippingTimeList = $("#" + parentRepeaterId + "_" + "ddlShippingTimeList option:selected");
		var messageShippingTime;
		messageShippingTime = '配送希望時間帯を下記に変更します。\n\n'
			+ '    配送希望時間帯： ' + ddlShippingTimeList.text() + '\n\n';

		messageShippingTime += 'よろしいですか？';
		var exec = ValidateAndConfirm("OrderShipping", messageShippingTime);
		if (exec) {
			document.getElementById('divShippingTimeUpdateButtons').style.display = "none";
			document.getElementById('divShippingTimeUpdateExecFroms').style.display = "block";
		}
		return exec;
	}

	var bindTargetForAddr1 = "";
	var bindTargetForAddr2 = "";
	var bindTargetForAddr3 = "";
	var multiAddrsearchTriggerType = "";
	<%-- 郵便番号検索のイベントをバインドする --%>
	function bindZipCodeSearch() {
		<% foreach (RepeaterItem ri in rOrderShipping.Items) { %>
			// Check shipping zip code input on click
			clickSearchZipCodeInRepeaterForSp(
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbShippingZip").ClientID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbShippingZip1").ClientID %>',
				'<%= GetWrappedTextBoxFromRepeater(ri, "tbShippingZip2").ClientID %>',
				'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchShippingAddr").ClientID %>',
				'<%= GetWrappedLinkButtonFromRepeater(ri, "lbSearchShippingAddr").UniqueID %>',
				'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON %>',
				'<%= '#' + (ri.FindControl("sShippingZipError")).ClientID %>',
				"shipping",
				'<%: w2.Common.Util.Validator.GetErrorMessage("CHECK_NECESSARY", "郵便番号") %>',
				'<%: w2.Common.Util.Validator.GetErrorMessage("CHECK_LENGTH", "郵便番号", "7") %>');

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

			if(multiAddrsearchTriggerType == "shipping")
			{
				bindTargetForAddr1 = "<%= ((DropDownList)ri.FindControl("ddlShippingAddr1")).ClientID %>";
				bindTargetForAddr2 = "<%= ((TextBox)ri.FindControl("tbShippingAddr2")).ClientID %>";
				bindTargetForAddr3 = "<%= ((TextBox)ri.FindControl("tbShippingAddr3")).ClientID %>";
			}

			<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
			// Textbox change search zip global
			textboxChangeSearchGlobalZip(
				'<%= ((TextBox)ri.FindControl("tbShippingZipGlobal")).ClientID %>',
				'<%= ((LinkButton)ri.FindControl("lbSearchAddrFromShippingZipGlobal")).UniqueID %>');
			<% } %>
		<%} %>
	}

	$(document).on('click', '.search-result-layer-close', function () {
		closePopupAndLayer();
	});

	$(document).on('click', '.search-result-layer-addr', function (e) {
		bindSelectedAddr($('li.search-result-layer-addr').index(this), multiAddrsearchTriggerType);
	});

	<%-- 複数住所検索結果からの選択値を入力フォームにバインドする --%>
	function bindSelectedAddr(selectedIndex, multiAddrsearchTriggerType) {
		var selectedAddr = $('.search-result-layer-addrs li').eq(selectedIndex);
		if (multiAddrsearchTriggerType == "shipping") {
			<% foreach (RepeaterItem ri in rOrderShipping.Items) { %>
			$('#' + bindTargetForAddr1).val(selectedAddr.find('.addr').text());
			$('#' + bindTargetForAddr2).val(selectedAddr.find('.city').text() + selectedAddr.find('.town').text());
			$('#' + bindTargetForAddr3).focus();
			<%} %>
		}
		closePopupAndLayer();
	}

	<%-- 変更後の表示通貨価格取得 --%>
	function AlertDataChange(kbnName, e) {
		var priceTotalOld = parseFloat(document.getElementById("<%= hfTotalPrice.ClientID %>").value);
		var priceTotalOldStr = '<%= CurrencyManager.ToPrice(hfTotalPrice.Value) %>';
		var priceTotalNew = '';
		var orderUsePointNew = 0;

		switch (kbnName) {
		case "Payment":
			priceTotalNew = parseFloat(document.getElementById("<%= hfPaymentTotalPriceNew.ClientID %>").value);
			break;

		case "OrderPointUse":
			orderUsePointNew = parseFloat(document.getElementById("<%= tbOrderPointUse.ClientID %>").value);
			priceTotalNew = priceTotalOld + <%#: this.OrderModel.OrderPointUse %> -orderUsePointNew;
			if (isNaN(orderUsePointNew) || (orderUsePointNew < 0)) return;
			break;

		case "Shipping":
			priceTotalNew = parseFloat(document.getElementById("<%= hfShippingTotalPriceNew.ClientID %>").value);
			break;

		default:
			return;
		}

		if (isNaN(priceTotalOld) || isNaN(priceTotalNew) || (priceTotalNew < 0)) return;


		var showMessage = (priceTotalOld != priceTotalNew);

		var exec = false;
		$.ajax({
			type: "POST",
			url: "<%= Constants.PATH_ROOT %>SmartPhone/Form/OrderHistory/OrderHistoryDetail.aspx/GetPriceString",
			data: JSON.stringify({ price: priceTotalNew }),
			contentType: "application/json; charset=utf-8",
			dataType: "json",
			cache: false,
			async: false,
			success: function (data) {
				switch (kbnName) {
				case "Payment":
					exec = AlertPaymentChange(priceTotalOldStr, data.d, showMessage);
					<% if (Constants.PAYMENT_PAIDY_OPTION_ENABLED) { %>
						if (exec
							&& ($('#<%= hfPaymentIdSelected.ClientID %>').val() == '<%= Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY %>')) {
							exec = false;

							if ('<%= Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Direct %>' == false
								|| $('#<%= hfPaymentIdSelected.ClientID %>').val() != '<%= this.PaymentModel.PaymentId %>') {
								// Paidy Pay Process
								PaidyPayProcess();
							}
							else {
								__doPostBack(updatePaymentUniqueID, '');
							}
						}
						<% } %>
					break;

				case "OrderPointUse":
					exec = AlertUpdateOrderPointUse(priceTotalOldStr, data.d, showMessage);
					break;

				case "Shipping":
					exec = AlertUpdateShippingInfo(priceTotalOldStr, data.d, showMessage, e);
					break;

				default:
					return;
				}
			}
		});
		return exec;
	}

		<%-- Check Payment Change --%>
	function CheckPaymentChange(element) {
		completeButton = element;

		if ((typeof isAuthoriesAtone === "boolean") && isAuthoriesAtone) {
			document.getElementById('divOrderPaymentUpdateButtons').style.display = "none";
			document.getElementById('divOrderPaymentUpdateExecFroms').style.display = "block";

			return isAuthoriesAtone;
		}
		else {
			var spanErrorMessageForAtone = document.getElementsByClassName('spanErrorMessageForAtone');
			if ((spanErrorMessageForAtone.length > 0)
				&& (spanErrorMessageForAtone[0].style.display == "block")) {
				return false;
			}
		}
		if ((typeof isAuthoriesAftee === "boolean") && isAuthoriesAftee) {
			document.getElementById('divOrderPaymentUpdateButtons').style.display = "none";
			document.getElementById('divOrderPaymentUpdateExecFroms').style.display = "block";

			return isAuthoriesAftee;
		}
		else {
			var spanErrorMessageForAftee = document.getElementsByClassName('spanErrorMessageForAftee');
			if ((spanErrorMessageForAftee.length > 0)
				&& (spanErrorMessageForAftee[0].style.display == "block")) {
				return false;
			}
		}

		if ((typeof isAuthories === "boolean") && isAuthories) {
			document.getElementById('divOrderPaymentUpdateButtons').style.display = "none";
			document.getElementById('divOrderPaymentUpdateExecFroms').style.display = "block";

			return isAuthories;
		}

		var result = AlertDataChange('Payment', element);
		<% if(Constants.PAYMENT_ATONEOPTION_ENABLED) { %>
		if (result
			&& ($('#<%= hfPaymentIdSelected.ClientID %>').val() == '<%= Constants.FLG_PAYMENT_PAYMENT_ID_ATONE %>')
			&& ('<%= this.OrderModel.OrderPaymentKbn %>' != '<%= Constants.FLG_PAYMENT_PAYMENT_ID_ATONE %>')) {
			AtoneAuthoriesForMyPage('<%# this.OrderModel.OrderId %>', $('#<%= hfPaymentIdSelected.ClientID %>').val(), element);
			document.getElementById('divOrderPaymentUpdateButtons').style.display = "block";
			document.getElementById('divOrderPaymentUpdateExecFroms').style.display = "none";

			return false;
		}
		<% } %>

		<% if (Constants.PAYMENT_AFTEEOPTION_ENABLED) { %>
		if (result
			&& ($('#<%= hfPaymentIdSelected.ClientID %>').val() == '<%= Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE %>')
			&& ('<%= this.OrderModel.OrderPaymentKbn %>' != '<%= Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE %>')) {
			AfteeAuthoriesForMyPage('<%# this.OrderModel.OrderId %>', $('#<%= hfPaymentIdSelected.ClientID %>').val(), element);
			document.getElementById('divOrderPaymentUpdateButtons').style.display = "block";
			document.getElementById('divOrderPaymentUpdateExecFroms').style.display = "none";

			return false;
		}
		<% } %>

		<% if (Constants.PAYMENT_LINEPAY_OPTION_ENABLED) { %>
		if (result
			&& ($('#<%= hfPaymentIdSelected.ClientID %>').val() == '<%= Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY %>')
			&& ('<%= this.OrderModel.OrderPaymentKbn %>' != '<%= Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY %>')) {
			ExecRequestPaymentMyPage('<%= StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ID]) %>', $('#<%= hfPaymentIdSelected.ClientID %>').val());
			document.getElementById('divOrderPaymentUpdateButtons').style.display = "block";
			document.getElementById('divOrderPaymentUpdateExecFroms').style.display = "none";

			return false;
		}
		<% } %>

		return result;
	}

	<% if(Constants.GLOBAL_OPTION_ENABLE) { %>
	<%-- 台湾郵便番号取得関数 --%>
	function bindTwAddressSearch() {
		<% foreach (RepeaterItem item in rOrderShipping.Items) { %>
			<% if (((DropDownList)item.FindControl("ddlShippingAddr3") != null) && ((TextBox)item.FindControl("tbShippingZipGlobal") != null)) { %>
			$('#<%= ((DropDownList)item.FindControl("ddlShippingAddr3")).ClientID %>').change(function (e) {
				$('#<%= ((TextBox)item.FindControl("tbShippingZipGlobal")).ClientID %>').val(
					$('#<%= ((DropDownList)item.FindControl("ddlShippingAddr3")).ClientID %>').val());
			});
			<% } %>
		<% } %>
	}
	<% } %>

	<% if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED) { %>
	<%-- Open convenience store map popup --%>
	function openConvenienceStoreMapPopup(cartIndex) {
		selectedCartIndex = cartIndex;
		currentRepeaterItemId = '<%= string.Join(",", this.WrOrderShipping.Items.Cast<RepeaterItem>().Select(item => item.ClientID)) %>';
		currentRepeaterItemId = currentRepeaterItemId.split(",")[selectedCartIndex];

		var url = '<%= OrderCommon.CreateConvenienceStoreMapUrl() %>';
		window.open(url, "", "width=1000,height=800");
	}

	<%-- Set convenience store data --%>
	function setConvenienceStoreData(cvsspot, name, addr, tel) {
		var elements = document.getElementsByClassName(selectedCartIndex)[0];
		if (elements == undefined || elements == null) {
			var seletedShippingNo = $('.UserShippingAddress').val();
			var className = '.user_addres' + seletedShippingNo;
			elements = $(className)[0];
			var hfSelectedShopId = $("#" + currentRepeaterItemId + "_" + "hfSelectedShopId");
			hfSelectedShopId.val(cvsspot);
		}

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
	}

	<%-- Open site map popup --%>
	function openSiteMapPopup(shippingCheckNo) {
		var url = "http://query2.e-can.com.tw/self_link/id_link.asp?txtMainID=" + shippingCheckNo;
		window.open(url, "", "width=850,height=500");
	}
	<% } %>

	<%-- 領収書出力ページを別タブで開く --%>
	function openReceiptDownload() {
		var url = '<%= WebSanitizer.UrlAttrHtmlEncode(CreateReceiptDownloadUrl(this.OrderModel.OrderId)) %>';
		window.open(url);
	}

	// 注文商品情報のレイアウトを変更する（頒布会定額コース同梱時）
	const changeOrderItemsAreaLayout = () => {
		<% if ((this.OrderModel.IsOrderCombined && this.OrderModel.HasSubscriptionBoxFixedAmountItem)
			   && (this.OrderModel.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false)) { %>
		$('#dlOrderItems').addClass('order-history-detail-items-container');
		$('.set-promotion-container').addClass('order-history-detail-normal');
		<% } %>
	}
</script>

<script type="text/javascript">
	//商品変更時の確認フォーム
	function AlertProductModify() {
		if (document.getElementById("<%= sModifyErrorMessage.ClientID %>").textContent.length > 0)
			return false;
		var messagePayment;
		var priceTotalOld = document.getElementById("<%= hfOrderPriceSubtotal.ClientID %>").value;
		var priceTotalNew = document.getElementById("<%= hfOrderPriceSubtotalNew.ClientID %>").value;
		var product = document.getElementById("<%= hfProducts.ClientID %>").value;
		var count = document.getElementById("<%= hfCounts.ClientID %>").value;
		messagePayment = '注文数を下記に変更します。\n';
		var products = product.split(',');
		var counts = count.split(',');
		for (var i = 0; i <= products.length; i++) {
			if (products[i] == undefined) break;
			messagePayment += "\n" + products[i] + "の注文数：" + counts[i];
		}

		if (priceTotalOld != priceTotalNew) {
			messagePayment += '\n\n変更に伴い、請求金額が変更されます。\n\n'
				+ '    変更前の価格：' + priceTotalOld + '\n'
				+ '    変更後の価格：' + priceTotalNew;
		}
		messagePayment += '\n\nよろしいですか？\n\n';
		return confirm(messagePayment);
	}
</script>

<%--▼▼ クレジットカードToken用スクリプト ▼▼--%>
<script type="text/javascript">
	var getTokenAndSetToFormJs = "<%= CreateGetCreditTokenAndSetToFormJsScript().Replace("\"", "\\\"") %>";
	var maskFormsForTokenJs = "<%= CreateMaskFormsForCreditTokenJsScript().Replace("\"", "\\\"") %>";
	var isMyPage = true;
</script>
<uc:CreditToken runat="server" ID="CreditToken" />
<%--▲▲ クレジットカードToken用スクリプト ▲▲--%>

<%--▼▼ Paidy用スクリプト ▼▼--%>
<script type="text/javascript">
	var buyer = <%= PaidyUtility.CreatedBuyerDataObjectForPaidyPayment(this.LoginUserId) %>;
	var body = <%= new PaidyCheckout(CartObject.CreateCartByOrder(this.OrderModel)).CreateParameterForPaidyCheckout() %>;
	var hfPaidyTokenIdControlId = "<%= this.WhfPaidyTokenId.ClientID %>";
	var hfPaidyPaymentIdControlId = "<%= (this.WhfPaidyPaymentId.ClientID) %>";
	var hfPaidyPaySelectedControlId = "<%= this.WhfPaidyPaySelected.ClientID %>";
	var hfPaidyStatusControlId = "<%= (this.WhfPaidyStatus.ClientID) %>";
	var updatePaymentUniqueID = "<%= this.WlbUpdatePayment.UniqueID %>";
	var isHistoryPage = true;
</script>
<uc:PaidyCheckoutScript ID="ucPaidyCheckoutScript" runat="server" />
<uc:PaidyCheckoutScriptForPagent runat="server" />
<%--▲▲ Paidy用スクリプト ▲▲--%>

<%--▼▼ Payment Atone And Aftee Script ▼▼--%>
<% if(Constants.PAYMENT_ATONEOPTION_ENABLED) { %>
	<script type="text/javascript">
		$('#<%= this.WhfAtoneToken.ClientID %>').val('<%= this.IsLoggedIn
			? this.LoginUser.UserExtend.UserExtendDataValue[Constants.FLG_USEREXTEND_USREX_ATONE_TOKEN_ID]
			: string.Empty %>');

		<%-- Set token --%>
		function SetAtoneTokenFromChildPage(token) {
			$('#<%= this.WhfAtoneToken.ClientID %>').val(token);
		}

		<%-- Get Current Token --%>
		function GetCurrentAtoneToken() {
			return $('#<%= this.WhfAtoneToken.ClientID %>').val();
		}

		<%-- Set Atone Transaction Id From My page --%>
		function SetAtoneTransactionIdFromMypage(id) {
			$('#<%= this.WhfAtoneTransactionId.ClientID %>').val(id);
		}
	</script>
	<% ucAtonePaymentScript.CurrentUrl = string.Format("{0}{1}",
					Constants.PATH_ROOT,
					string.Format("{0}{1}", this.IsSmartPhone
						? "SmartPhone/"
						: string.Empty, Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL)); %>
	<uc:AtonePaymentScript ID="ucAtonePaymentScript" runat="server"/>
<% } %>

<% if(Constants.PAYMENT_AFTEEOPTION_ENABLED) { %>
	<script type="text/javascript">
		$('#<%= this.WhfAfteeToken.ClientID %>').val('<%= this.IsLoggedIn
			? this.LoginUser.UserExtend.UserExtendDataValue[Constants.FLG_USEREXTEND_USREX_AFTEE_TOKEN_ID]
			: string.Empty %>');

		<%-- Set token --%>
		function SetAfteeTokenFromChildPage(token) {
			$('#<%= this.WhfAfteeToken.ClientID %>').val(token);
		}

		<%-- Get Current Aftee token--%>
		function GetCurrentAfteeToken() {
			return $('#<%= this.WhfAfteeToken.ClientID %>').val();
		}

		<%-- Set Aftee Transaction Id From My page --%>
		function SetAfteeTransactionIdFromMypage(id) {
			$('#<%= this.WhfAfteeTransactionId.ClientID %>').val(id);
		}
	</script>
	<% ucAfteePaymentScript.CurrentUrl = string.Format("{0}{1}",
					Constants.PATH_ROOT,
					string.Format("{0}{1}", this.IsSmartPhone
						? "SmartPhone/"
						: string.Empty, Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL)); %>
	<uc:AfteePaymentScript ID="ucAfteePaymentScript" runat="server"/>
<% } %>
<%--▲▲ Payment Atone And Aftee Script ▲▲--%>
<input type="hidden" id="fraudbuster" name="fraudbuster" />
<script type="text/javascript" src="//cdn.credit.gmo-ab.com/psdatacollector.js"></script>
<uc:RakutenPaymentScript ID="ucRakutenPaymentScript" runat="server" />
</asp:Content>
