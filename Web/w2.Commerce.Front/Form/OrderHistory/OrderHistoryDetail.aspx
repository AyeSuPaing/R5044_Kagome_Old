<%--
=========================================================================================================
  Module      : 注文履歴詳細画面(OrderHistoryDetail.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/OrderHistory/OrderHistoryDetail.aspx.cs" Inherits="Form_Order_OrderHistoryDetail" Title="購入履歴詳細ページ" %>
<%@ Import Namespace="System.ComponentModel" %>
<%@ Import Namespace="System.Runtime.CompilerServices" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.Paidy" %>
<%@ Import Namespace="w2.Domain.UserShipping" %>
<%@ Import Namespace="w2.App.Common.AmazonCv2" %>
<%-- ▼削除禁止：クレジットカードTokenコントロール▼ --%>
<%@ Register TagPrefix="uc" TagName="CreditToken" Src="~/Form/Common/CreditToken.ascx" %>
<%-- ▲削除禁止：クレジットカードTokenコントロール▲ --%>
<%@ Register TagPrefix="uc" TagName="RakutenCreditCard" Src="~/Form/Common/RakutenCreditCardModal.ascx" %>
<%@ Register TagPrefix="uc" TagName="RakutenPaymentScript" Src="~/Form/Common/RakutenPaymentScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="Layer" Src="~/Form/Common/Layer/SearchResultLayer.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionCvsDef" Src="~/Form/Common/Order/PaymentDescriptionCvsDef.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionSmsDef" Src="~/Form/Common/Order/PaymentDescriptionSmsDef.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionPayPal" Src="~/Form/Common/Order/PaymentDescriptionPayPal.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaypalScriptsForm" Src="~/Form/Common/PayPalScriptsForm.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionTriLinkAfterPay" Src="~/Form/Common/Order/PaymentDescriptionTriLinkAfterPay.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaidyCheckoutScript" Src="~/Form/Common/Order/PaidyCheckoutScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaidyCheckoutControl" Src="~/Form/Common/Order/PaidyCheckoutControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="AtonePaymentScript" Src="~/Form/Common/AtonePaymentScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="AfteePaymentScript" Src="~/Form/Common/AfteePaymentScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionAtone" Src="~/Form/Common/Order/PaymentDescriptionAtone.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionNPAfterPay" Src="~/Form/Common/Order/PaymentDescriptionNPAfterPay.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionLinePay" Src="~/Form/Common/Order/PaymentDescriptionLinePay.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionPayPay" Src="~/Form/Common/Order/PaymentDescriptionPayPay.ascx" %>
<%@ Register TagPrefix="uc" TagName="EcPayScript" Src="~/Form/Common/ECPay/EcPayScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentYamatoKaSmsAuthModal" Src="~/Form/Common/Order/PaymentYamatoKaSmsAuthModal.ascx" %>
<%@ Register TagPrefix="uc" TagName="Loading" Src="~/Form/Common/Loading.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaidyCheckoutScriptForPagent" Src="~/Form/Common/Order/PaidyCheckoutScriptForPaygent.ascx" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.Paygent.Paidy.Checkout" %>
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
<%-- UpdatePanel開始 --%>
<asp:UpdatePanel ID="upUpdatePanel" UpdateMode="Conditional" runat="server">
<ContentTemplate>

<div id="dvUserFltContents">
		<h2>購入履歴詳細</h2>
		<div class="errorMessage" style="color: red; border-top: none; font-size: 1em; padding: 0 20px 20px;">
			<asp:Literal ID="lPaymentErrorMessage" runat="server" />
		</div>
	<div id="dvOrderHistoryDetail" class="unit">
			
		<%-- 注文情報 --%>
		<div class="dvOrderHistoryInfo">
			<h3>ご注文情報</h3>
			<table>
				<tr>
					<th>ご注文番号</th>
					<td>
						<%#: this.OrderModel.OrderId %>
						<div style="text-align:right; float:right;">
							<asp:LinkButton ID="lbCancelOrder" Text="注文キャンセル" runat="server" OnClientClick="return ConfirmOrderCancel()" OnClick="lbCancelOrder_Click"
											Enabled="<%# this.IsModifyCancel %>"
											class="btn" />
							<div Visible="<%# (this.IsModifyCancel && OrderCommon.DisplayTwInvoiceInfo()) %>" runat="server">
								<asp:RadioButton ID="rbTwInvoiceCancel" Text="電子発票キャンセル" Checked="True" GroupName="twInvoice" runat="server"/>
								<asp:RadioButton ID="rbTwInvoiceRefund" Text="電子発票払い戻し" GroupName="twInvoice" runat="server"/>
							</div>
							<p style="padding-top: 5px" runat="server" Visible="<%# this.IsModifyCancelMessage != null %>"><%# WebSanitizer.HtmlEncode(this.IsModifyCancelMessage) %></p>
							<p style="padding-top: 5px" runat="server" Visible="<%# this.IsModifyCancelMessage == null %>"><%# WebSanitizer.HtmlEncode(this.OrderCancelTimeMessage) %></p>
						</div>
					</td>
				</tr>
				<% if (this.IsFixedPurchase){ %>
				<tr>
					<th>定期購入ID</th>
					<td>
						<%#: this.OrderModel.FixedPurchaseId %>
						<div style="float: right">
							<asp:LinkButton Text="次回以降の注文変更" runat="server" OnClick="lbDisplayFixedPurchaseDetail_Click" class="btn" />
						</div>
					</td>
				</tr>
				<%}%>
				<tr>
					<th>購入日</th>
					<td>
						<%#: DateTimeUtility.ToStringFromRegion(this.OrderModel.OrderDate, DateTimeUtility.FormatType.ShortDate2Letter) %></td>
				</tr>
				<tr>
					<th>ご注文状況</th>
					<td>
						<% if (IsPickupRealShop && (this.OrderModel.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP)) { %>
						<%#: (this.OrderModel.StorePickupStatus == Constants.FLG_STOREPICKUP_STATUS_PENDING) ? ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_STATUS, this.OrderModel.OrderStatus) + "(店舗未到着)" : ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_STOREPICKUP_STATUS, this.OrderModel.StorePickupStatus) %>
						<% } else { %>
						<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_STATUS, this.OrderModel.OrderStatus) %><%#: this.OrderModel.ShippedChangedKbn == Constants.FLG_ORDER_SHIPPED_CHANGED_KBN_CHANAGED ? "（変更有り）" : string.Empty %>
						<% } %>
					</td>
				</tr>
				<% if (Constants.PAYMENT_GMO_POST_ENABLED) { %>
					<!--show credit status if using GMO-->
					<% if ((this.OrderModel.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO) || (this.OrderModel.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)) { %>
					<tr>
						<th>与信状況</th>
						<td><%#: ValueText.GetValueText(Constants.TABLE_ORDER,Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS, this.OrderModel.ExternalPaymentStatus) %></td>
					</tr>
					<% } %>
				<% } %>
				<tr>
					<th>ご入金状況</th>
					<td>
						<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, this.OrderModel.OrderPaymentStatus) %></td>
				</tr>
				<tr>
					<th>お支払い方法</th>
					<td>
						<%#: this.PaymentModel.PaymentName %>
						<% if ((this.OrderModel.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) && (string.IsNullOrEmpty(this.OrderModel.CardInstruments) == false)) { %>
							(<%: this.OrderModel.CardInstruments %>)
						<% } %>
						<% if (this.OrderModel.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT) { %>
							<div style="margin: 10px 0;">
								<small>※現在のAmazon Payでの配送先情報、お支払い方法を表示しています。</small>
							</div>
							<iframe id="AmazonDetailWidget" src="<%: PageUrlCreatorUtility.CreateAmazonPayWidgetUrl(true, orderId: this.OrderModel.OrderId) %>" style="width:100%;border:none;"></iframe>
						<% } %>
						<div style="text-align:right; float:right;">
							<asp:LinkButton ID="lbDisplayInputOrderPaymentKbn"
								Text="お支払い方法変更"
								Visible="<%# this.IsPickupRealShop == false %>"
								OnClick="lbDisplayInputOrderPaymentKbn_Click"
								Enabled="<%# this.IsDisplayInputOrderPaymentKbn %>"
								class="btn"
								AutoPostBack="true"
								runat="server" />
						</div>
						<div style="text-align:right; padding-top:15px;">
							<%#: this.ExplanationOrderPaymentKbn %>
						</div>
						<% if (this.CanRequestCvsDefInvoiceReissue) { %>
							<div style="text-align:right; float:right; padding-top:15px;">
								<asp:LinkButton
									ID="lbRequestCvsDefInvoiceReissue"
									runat="server"
									Enabled="<%# (this.IsOrderCvsDefInvoiceReissueRequested == false) %>"
									OnClick="lbRequestCvsDefInvoiceReissue_Click"
									AutoPostBack="true"
									class="btn" />
								<div style="text-align:right; padding-top:10px;">
									<asp:Literal ID="lCvsDefInvoiceReissueRequestResultMessage" runat="server" />
								</div>
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
							<% } %>
						</div>
						<%-- ▲PayPalログインここまで▲ --%>

						<asp:HiddenField ID="hfPaidyTokenId" runat="server" />
						<asp:HiddenField ID="hfPaidyPaymentId" runat="server" />
						<asp:HiddenField ID="hfPaidyPaySelected" runat="server" />
						<asp:HiddenField ID="hfPaidyStatus" runat="server" />
						<div id="dvOrderPaymentPattern" Visible="False" runat="server">
							<tr>
								<th>お支払い情報</th>
								<td id="CartList">
									<div class="orderBox" style="background: url() !important;">
										<div class="list">
											<span style="color:red" runat="server" visible="<%# (string.IsNullOrEmpty(StringUtility.ToEmpty(this.DispLimitedPaymentMessages[0])) == false) %>">
												<%# StringUtility.ToEmpty(this.DispLimitedPaymentMessages[0]) %><br/>
											</span>
										<dl class="list">
											<% if(Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_DDL) { %>
												<dt><asp:DropDownList ID="ddlPayment" runat="server" DataSource="<%# this.ValidPayments[0] %>" ItemType="w2.Domain.Payment.PaymentModel" OnSelectedIndexChanged="rbgPayment_OnCheckedChanged" AutoPostBack="true" DataTextField="PaymentName" DataValueField="PaymentId" /></dt>
											<% } %>
											<asp:Repeater ID="rPayment" DataSource="<%# this.ValidPayments[0] %>" ItemType="w2.Domain.Payment.PaymentModel" runat="server" >
											<ItemTemplate>
												<asp:HiddenField ID="hfPaymentId" Value='<%# Item.PaymentId %>' runat="server" />
												<asp:HiddenField ID="hfPaymentName" Value='<%# Item.PaymentName %>' runat="server" />
												<asp:HiddenField ID="hfPaymentPrice" Value="<%# OrderCommon.GetPaymentPrice(Item.ShopId, Item.PaymentId, this.OrderModel.OrderPriceSubtotal, OrderCommon.GetPriceCartTotalWithoutPaymentPrice(this.OrderModel)) %>" runat="server" />
												<% if(Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_RB) { %>
												<dt><w2c:RadioButtonGroup ID="rbgPayment" GroupName='Payment' Checked="<%# this.OrderModel.OrderPaymentKbn == Item.PaymentId %>" Text="<%#: Item.PaymentName %>" OnCheckedChanged="rbgPayment_OnCheckedChanged" AutoPostBack="true" CssClass="radioBtn" runat="server" /></dt>
												<% } %>

												<%-- クレジット --%>
												<dd id="ddCredit" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) %>" runat="server">
													<div id="divCreditCardInputForm">
													<asp:DropDownList ID="ddlUserCreditCard" runat="server" AutoPostBack="true" DataTextField="text" DataValueField="value" OnSelectedIndexChanged="ddlUserCreditCard_OnSelectedIndexChanged"></asp:DropDownList>
												
													<%-- ▽新規カード▽ --%>
													<% if (IsNewCreditCard()){ %>
													
													<% if (this.IsCreditCardLinkPayment() == false) { %>
													<%--▼▼ カード情報取得用 ▼▼--%>
													<input type="hidden" id="hidCinfo" name="hidCinfo" value="<%# CreateGetCardInfoJsScriptForCreditToken(Container) %>" />
													<%--▲▲ カード情報取得用 ▲▲--%>

													<%--▼▼ クレジット Token保持用 ▼▼--%>
													<asp:HiddenField ID="hfCreditToken" Value="" runat="server" />
													<%--▲▲ クレジット Token保持用 ▲▲--%>
													<div id="divRakutenCredit" runat="server">
														<asp:LinkButton id="lbEditCreditCardNoForRakutenToken" CssClass="lbEditCreditCardNoForRakutenToken" OnClick="lbEditCreditCardNoForToken_Click" runat="server">再入力</asp:LinkButton>
														<uc:RakutenCreditCard
															ID="ucRakutenCreditCard"
															IsOrder="true"
															CartIndex="1"
															InstallmentCodeList="<%# this.CreditInstallmentsList %>"
															runat="server"/>
													</div>
													<%--▼▼ カード情報入力（トークン未取得・利用なし） ▼▼--%>
													<div id="divCreditCardNoToken" runat="server">
														<%if (OrderCommon.CreditCompanySelectable) {%>
														<strong>カード会社</strong>
														<p><asp:DropDownList id="ddlCreditCardCompany" runat="server" DataTextField="Text" DataValueField="Value" CssClass="input_widthG input_border"></asp:DropDownList></p>
														<% } %>
														<strong>カード番号</strong>&nbsp;<span class="fred">※</span>
														<p>
															<w2c:ExtendedTextBox id="tbCreditCardNo1" Type="tel" runat="server" CssClass="tel" MaxLength="16" autocomplete="off"></w2c:ExtendedTextBox><br />
														<small class="fred">
															<asp:CustomValidator ID="cvCreditCardNo1" runat="Server"
																ControlToValidate="tbCreditCardNo1"
																ValidationGroup="OrderPayment"
																ValidateEmptyText="true"
																SetFocusOnError="true"
																ClientValidationFunction="ClientValidate"
																CssClass="error_inline" />
														</small>
														<small class="fgray">
														カードの表記のとおりご入力ください。<br />
														例：<br />
															1234567890123456（ハイフンなし）
														</small></p>
														<strong>有効期限</strong>
														<p>
															<asp:DropDownList id="ddlCreditExpireMonth" runat="server" CssClass="expMonth"></asp:DropDownList>/
															<asp:DropDownList id="ddlCreditExpireYear" runat="server" CssClass="expYear"></asp:DropDownList> (月/年)</p>
														<strong>カード名義人</strong><span class="fred">※</span>例：「TAROU YAMADA」
														<p>
															<asp:TextBox id="tbCreditAuthorName" runat="server" CssClass="nameFull" MaxLength="50" autocomplete="off"></asp:TextBox><br />
																<small class="fred">
																	<asp:CustomValidator ID="cvCreditAuthorName" runat="Server"
																		ControlToValidate="tbCreditAuthorName"
																		ValidationGroup="OrderPayment"
																		ValidateEmptyText="true"
																		SetFocusOnError="true"
																		ClientValidationFunction="ClientValidate"
																		CssClass="error_inline" />
																</small>
														</p>
														<div id="trSecurityCode" visible="<%# OrderCommon.CreditSecurityCodeEnable %>" runat="server">
														<strong>セキュリティコード</strong>&nbsp;<span class="fred">※</span>
														<p>
															<asp:TextBox id="tbCreditSecurityCode" runat="server" CssClass="securityCode" MaxLength="4" autocomplete="off"></asp:TextBox><br />
															<small class="fred">
															<asp:CustomValidator ID="cvCreditSecurityCode" runat="Server"
																ControlToValidate="tbCreditSecurityCode"
																ValidationGroup="OrderPayment"
																ValidateEmptyText="true"
																SetFocusOnError="true"
																ClientValidationFunction="ClientValidate"
																CssClass="error_inline" />
															</small>
														</p>
														</div>
													</div>
													<%--▲▲ カード情報入力（トークン未取得・利用なし） ▲▲--%>

													<%--▼▼ カード情報入力（トークン取得済） ▼▼--%>
													<div id="divCreditCardForTokenAcquired" runat="server">
														<%if (OrderCommon.CreditCompanySelectable) {%>
														<strong>カード会社</strong>
														<p><asp:Literal ID="lCreditCardCompanyNameForTokenAcquired" runat="server"></asp:Literal></p>
														<%} %>
														<strong>カード番号</strong>
														<p>XXXXXXXXXXXX<asp:Literal ID="lLastFourDigitForTokenAcquired" runat="server"></asp:Literal>
														<asp:LinkButton id="lbEditCreditCardNoForToken" OnClick="lbEditCreditCardNoForToken_Click" runat="server">再入力</asp:LinkButton><br /></p>
														<strong>有効期限</strong>
														<p><asp:Literal ID="lExpirationMonthForTokenAcquired" runat="server"></asp:Literal>
															/
															<asp:Literal ID="lExpirationYearForTokenAcquired" runat="server"></asp:Literal>
														(月/年)</p>
														<strong>カード名義人</strong>
														<p><asp:Literal ID="lCreditAuthorNameForTokenAcquired" runat="server"></asp:Literal></p>
													</div>
													<%--▲▲ カード情報入力（トークン取得済） ▲▲ --%>

													<div id="Div3" visible="<%# OrderCommon.CreditInstallmentsSelectable && Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Rakuten %>" runat="server">
														<strong>支払い回数</strong>
														<p>
															<asp:DropDownList id="dllCreditInstallments" runat="server" DataTextField="Text" DataValueField="Value" CssClass="input_border" autocomplete="off"></asp:DropDownList><br/>
															<span class="fgray">※AMEX/DINERSは一括のみとなります。</span>
														</p>
													</div>
													<% } else { %>
														<div>遷移する外部サイトでカード番号を入力してください。</div>
													<% } %>
													<asp:CheckBox ID="cbRegistCreditCard" runat="server" Checked="false" OnCheckedChanged="cbRegistCreditCard_OnCheckedChanged" Text="登録する" autocomplete="off" AutoPostBack="true"/>
													<div id="divUserCreditCardName" Visible="false" runat="server">
														<p>クレジットカードを保存する場合は、以下をご入力ください。</p>
														<strong>クレジットカード登録名&nbsp;<span class="fred">※</span></strong>
														<p>
															<asp:TextBox ID="tbUserCreditCardName" Text="" MaxLength="100" CssClass="input_widthD input_border" runat="server" autocomplete="off"></asp:TextBox><br />
															<small class="fred">
															<asp:CustomValidator ID="cvUserCreditCardName" runat="Server"
																ControlToValidate="tbUserCreditCardName"
																ValidationGroup="OrderPayment"
																ValidateEmptyText="true"
																SetFocusOnError="true"
																ClientValidationFunction="ClientValidate"
																CssClass="error_inline" />
															</small>
														</p>
													</div>
													<span id="spanErrorMessageForCreditCard" style="color: red; display: none" runat="server"></span>
													</div>
													<%-- △新規カード△ --%>

													<%-- ▽登録済みカード▽ --%>
													<% }else{ %>
													<div id="divCreditCardDisp" runat="server">
														<%if (OrderCommon.CreditCompanySelectable) {%>
														<strong>カード会社</strong>
														<p><%: this.CreditCardCompanyName %><br /></p>
														<%} %>
														<strong>カード番号</strong>
														<p>XXXXXXXXXXXX<%: this.LastFourDigit %><br /></p>
														<strong>有効期限</strong>
														<p><%: this.ExpirationMonth %>/<%: this.ExpirationYear %> (月/年)</p>
														<strong>カード名義人</strong>
														<p><%: this.CreditAuthorName %></p>
														<asp:HiddenField ID="hfCreditCardId" runat="server" />

														<div visible="<%# OrderCommon.CreditInstallmentsSelectable %>" runat="server">
															<strong>支払い回数</strong>
															<p>
																<asp:DropDownList id="dllCreditInstallments2" runat="server" CssClass="input_border"></asp:DropDownList>
																<br/>
																<span class="fgray">※AMEX/DINERSは一括のみとなります。</span>
															</p>
														</div>
													</div>
													<% } %>
												<%-- △登録済みカード△ --%>
												</dd>

												<%-- コンビニ(後払い) --%>
												<dd id="ddCvsDef" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF) %>" runat="server">
													<uc:PaymentDescriptionCvsDef runat="server" id="ucPaymentDescriptionCvsDef" />
												</dd>

												<%-- コンビニ(後払い・SMS認証) --%>
												<dd id="ddSmsDef" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_SMS_DEF) %>" runat="server">
													<uc:PaymentDescriptionSmsDef runat="server" id="ucPaymentDescriptionSmsDef" />
												</dd>

												<%-- 後付款(TriLink後払い) --%>
												<dd id="ddTriLinkAfterPayPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY) %>" runat="server">
													<uc:PaymentDescriptionTriLinkAfterPay runat="server" id="ucPaymentDescriptionTryLinkAfterPay" />
												</dd>

												<%-- Amazon Pay --%>
												<dd id="ddAmazonPay" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT) %>" runat="server">
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

												<%-- 代金引換 --%>
												<dd id="ddCollect" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT) %>" runat="server">
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

												<!-- LinePay -->
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
										<asp:LinkButton Text="キャンセル" runat="server" OnClick="btnClosePaymentPatternInfo_Click" class="btn"></asp:LinkButton>
									</div>
									<div id="divOrderPaymentUpdateExecFroms" style="display: none"> 
										更新中です...
									</div>
									<small id="sErrorMessagePayment" class="error" runat="server"></small>
								</td>
							</tr>
						</div>
					</td>
				</tr>
				<% if ((this.OrderModel.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) && (this.UserCreditCardInfo != null)) { %>
				<tr id="dvFixedPurchaseCurrentCard" runat="server">
					<th>利用クレジットカード情報</th>
					<td>
						<% if (this.UserCreditCardInfo.DispFlg == Constants.FLG_USERCREDITCARD_DISP_FLG_ON) { %>
						クレジットカード登録名: <%:this.UserCreditCardInfo.CardDispName %><%: this.UserCreditCardInfo.DispFlag ? "" : " (削除済)" %><br />
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
				<tr>
					<th>注文メモ</th>
					<td>
						<%#: StringUtility.ToEmpty(this.OrderModel.Memo) != "" ? this.OrderModel.Memo : "指定なし" %></td>
				</tr>
				<%if (IsDisplayOrderExtend()) {%>
				<tr>
					<th>注文確認事項</th>
					<td>
						<div style="text-align:right; float:right; ">
							<asp:LinkButton ID="lbOrderExtend" Text="注文確認事項の変更" Visible="<%# IsDisplayOrderExtendModifyButton() %>" Enabled="<%# this.IsModifyOrder %>" OnClick="lbOrderExtend_OnClick"  runat="server" class="btn" />
							<p style="padding-top: 5px" runat="server" Visible="<%# this.ExplanationOrderExtend != null %>"><%#: this.ExplanationOrderExtend %></p>
						</div>
						<% if (this.IsOrderExtendModify) { %>
						<asp:Repeater ID="rOrderExtendInput" ItemType="OrderExtendItemInput" runat="server">
							<ItemTemplate>
								<div style="padding-bottom: 10px;"> 
									<%-- 項目名 --%>
									<strong><%#: Item.SettingModel.SettingName %></strong>
									<span class="fred" runat="server" visible="<%# Item.SettingModel.IsNeecessary%>">※</span>:<br />
									<%-- 概要 --%>
									<%# Item.SettingModel.OutlineHtmlEncode %><br />
									<%-- TEXT --%>
									<div runat="server" visible="<%# Item.SettingModel.CanUseModify && Item.SettingModel.IsInputTypeText%>">
										<asp:TextBox runat="server" ID="tbSelect" Width="250px" MaxLength="100"></asp:TextBox>
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
									<small><asp:Label runat="server" ID="lbErrMessage" CssClass="error_inline"></asp:Label></small>
									<asp:HiddenField ID="hfSettingId" runat="server" Value="<%# Item.SettingModel.SettingId %>" />
									<asp:HiddenField ID="hfInputType" runat="server" Value="<%# Item.SettingModel.InputType %>" />
								</div>
							</ItemTemplate>
						</asp:Repeater>
						<% } else { %>
						<asp:Repeater ID="rOrderExtendDisplay" ItemType="OrderExtendItemInput" runat="server">
							<ItemTemplate>
								<div style="padding-bottom: 5px;"> 
									<strong><%#: Item.SettingModel.SettingName %>:</strong><br />
									<%#: Item.InputText %>
								</div>
							</ItemTemplate>
						</asp:Repeater>
						<% } %>
					</td>
				</tr>
				<% if (this.IsOrderExtendModify) { %>
				<tr>
					<th></th>
					<td>
						<div id="divOrderExtendUpdateButtons" style="display: block"> 
							<asp:LinkButton ID="lbUpdateOrderExtend" Text="情報更新" runat="server" OnClientClick="return AlertDataChange('OrderPointUse', null);" OnClick="lbUpdateOrderExtend_OnClick" class="btn" />
							<asp:LinkButton ID="lbHideOrderExtend" Text="キャンセル" runat="server" OnClientClick="return exec_submit();" OnClick="lbHideOrderExtend_OnClick" class="btn" />
						</div>
						<div id="divOrderExtendUpdateExecFroms" style="display: none"> 
							更新中です...
						</div>
					</td>
				</tr>
				<% } %>
				<% } %>
				<%-- ポイントオプションが有効な場合 --%>
				<%if (Constants.W2MP_POINT_OPTION_ENABLED) {%>
				<tr>
					<th>購入時付与ポイント</th>
					<td>
						<%#: GetNumeric(this.OrderModel.OrderPointAdd) %><%: Constants.CONST_UNIT_POINT_PT %></td>
				</tr>
				<tr>
					<th>ご利用ポイント</th>
					<td>
						<%#: GetNumeric(this.OrderModel.OrderPointUse) %><%: Constants.CONST_UNIT_POINT_PT %>
						<div style="text-align:right; float:right; ">
							<asp:LinkButton ID="lbDisplayInputOrderPointUse" Text="利用ポイント変更" OnClick="lbDisplayInputOrderPointUse_Click"  runat="server" Enabled="<%# this.IsModifyUsePoint %>" class="btn" Visible="<%# this.CanUsePointForPurchase %>" />
						</div>
						<br />
						<br />
						<small id="slErrorMessageChangePointUse" runat="server" style="text-align:right; float:right; "></small>
						<div style="text-align:right; padding-top:15px;">
							<%: this.ExplanationPointUse %>
						</div>
						<% if (this.CanDisplayPointUse) { %>
						<br />
						利用可能ポイントは<%: StringUtility.ToNumeric(this.LoginUserPointUsable + this.OrderModel.OrderPointUse) %><%: Constants.CONST_UNIT_POINT_PT %>です。<br />
						※1<%: Constants.CONST_UNIT_POINT_PT %> = <%: CurrencyManager.ToPrice(1m) %><br />
						<asp:TextBox ID="tbOrderPointUse" runat="server" style="width: 70px;"></asp:TextBox> <%: Constants.CONST_UNIT_POINT_PT %>
						<br />
						<small id="slErrorMessagePointUse" runat="server" class="fred"></small>
						<% } %>
					</td>
				</tr>
					<% if (this.IsOrderPointAddDisplayStatus) { %>
				<tr>
					<th></th>
					<td>
						<div id="divOrderPointUpdateButtons" style="display: block"> 
							<asp:LinkButton Text="情報更新" runat="server" OnClientClick="return AlertDataChange('OrderPointUse', null);" OnClick="lbUpdateOrderPointUse_Click" class="btn" />
							<asp:LinkButton Text="キャンセル" runat="server" OnClientClick="return exec_submit();" OnClick="lbHideOrderPointUse_Click" class="btn" />
						</div>
						<div id="divOrderPointUpdateExecFroms" style="display: none"> 
							更新中です...
						</div>
					</td>
				</tr>
					<% } %>
				<% } %>
				<tr>
					<th class="orderTotal">
						総合計（税込）</th>
					<td class="orderTotal">
						<%#: CurrencyManager.ToPrice(this.OrderModel.OrderPriceTotal) %></td>
				</tr>
				<%if (Constants.GLOBAL_OPTION_ENABLE) { %>
				<tr>
					<th class="orderTotal">
						決済金額（税込）</th>
					<td class="orderTotal">
						<%#: this.SendingAmount %></td>
				</tr>
				<% } %>
			</table>
		</div>

		<%--▽領収書情報▽--%>
		<% if (Constants.RECEIPT_OPTION_ENABLED) { %>
		<h3 style="display: inline-block;">領収書情報</h3>
		<asp:LinkButton ID="lbReceiptDownload" runat="server" OnClientClick="openReceiptDownload(); return false;" visible="<%# (this.OrderModel.ReceiptFlg == Constants.FLG_ORDER_RECEIPT_FLG_ON) %>" class="btn btnReceiptDownload" Text="領収書ダウンロード" />
		<div class="dvReceiptDownloadErrorMessage"><%: this.ReceiptDownloadErrorMessage %></div>
			<% if (this.IsReceiptInfoModify == false) { %>
			<table>
				<tr>
					<th>領収書希望</th>
					<td>
						<div style="text-align:right; float:right;"><asp:LinkButton id="lbReceiptFlg" runat="server" OnClick="lbDisplayReceiptInfoForm_Click" Text="領収書情報変更" class="btn" Visible="<%# this.CanModifyReceiptInfo %>" /></div>
						<%: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_RECEIPT_FLG, this.OrderModel.ReceiptFlg) %>
						<%: (this.OrderModel.ReceiptOutputFlg == Constants.FLG_ORDER_RECEIPT_OUTPUT_FLG_ON) ? "(出力済み)" : "" %>
					</td>
				</tr>
				<tr runat="server" visible="<%# this.OrderModel.ReceiptFlg == Constants.FLG_ORDER_RECEIPT_FLG_ON %>">
					<th>宛名</th>
					<td><%: this.OrderModel.ReceiptAddress %></td>
				</tr>
				<tr runat="server" visible="<%# this.OrderModel.ReceiptFlg == Constants.FLG_ORDER_RECEIPT_FLG_ON %>">
					<th>但し書き</th>
					<td><%: this.OrderModel.ReceiptProviso %></td>
				</tr>
			</table>
			<% } else { %>
			<table>
				<tr>
					<th>領収書希望</th>
					<td>
						<asp:DropDownList ID="ddlReceiptFlg" runat="server" DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlReceiptFlg_OnSelectedIndexChanged"
							AutoPostBack="true" DataSource="<%# this.ReceiptFlgListItems %>" />
					</td>
				</tr>
				<tr id="trReceiptAddressInput" runat="server">
					<th>宛名<span class="necessary">*</span></th>
					<td>
						<asp:TextBox ID="tbReceiptAddress" runat="server" Width="300" MaxLength="100" />
						<p><asp:CustomValidator runat="Server"
							ControlToValidate="tbReceiptAddress"
							ValidationGroup="ReceiptRegisterModify"
							ClientValidationFunction="ClientValidate"
							ValidateEmptyText="true"
							SetFocusOnError="true" /></p>
					</td>
				</tr>
				<tr id="trReceiptProvisoInput" runat="server">
					<th>但し書き<span class="necessary">*</span></th>
					<td>
						<asp:TextBox ID="tbReceiptProviso" runat="server" Width="300" MaxLength="100" />
						<p><asp:CustomValidator runat="Server"
							ControlToValidate="tbReceiptProviso"
							ValidationGroup="ReceiptRegisterModify"
							ClientValidationFunction="ClientValidate"
							ValidateEmptyText="true"
							SetFocusOnError="true" /></p>
					</td>
				</tr>
				<tr>
					<th></th>
					<td>
						<div style="display: block">
							<asp:LinkButton Text="領収書情報更新" runat="server" OnClientClick="return confirm('領収書情報を変更してもよろしいですか？')" OnClick="lbUpdateReceiptInfo_Click" class="btn" />
							<asp:LinkButton Text="キャンセル" runat="server" OnClientClick="return exec_submit();" OnClick="lbDisplayReceiptInfoForm_Click" class="btn" />
						</div>
						<small class="error" style="padding: 2px;"><%: this.ReceiptInfoModifyErrorMessage %></small>
					</td>
				</tr>
			</table>
			<% } %>
		<br />
		<% } %>
		<%--△領収書情報△--%>

		<a name="ShippingArea" runat="server"></a>
		<%
			this.CartShippingItemIndexTmp = -1;
		%>
		<asp:Repeater ID="rOrderShipping" DataSource="<%# this.OrderShippingItems %>" Runat="server">
			<ItemTemplate>
				<% if (this.OrderModel.DigitalContentsFlg != Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_ON) { %>
				<div class="dvOrderHistoryShipping">
					<%-- お届け先情報 --%>
					<h3>お届け先情報</h3>
					<asp:Label ID="lOrderHistoryErrorMessage" CssClass="fred" runat="server" Visible="false"></asp:Label>
					<% if (this.IsPickupRealShop == false) { %>
					<table>
						<tr visible='<%# StringUtility.ToEmpty(this.OrderModel.GiftFlg) == Constants.FLG_ORDER_GIFT_FLG_ON %>' runat="server">
							<th colspan="2">送り主</th>
						</tr>
						<tr visible='<%# StringUtility.ToEmpty(this.OrderModel.GiftFlg) == Constants.FLG_ORDER_GIFT_FLG_ON %>' runat="server">
							<th>
								<%: ReplaceTag("@@User.addr.name@@") %>
							</th>
							<td>
								<%# IsCountryJp((string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_ISO_CODE))
									? "〒" + WebSanitizer.HtmlEncode(GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SENDER_ZIP)) + "<br />"
									: "" %>
								<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SENDER_ADDR1) %>
								<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SENDER_ADDR2) %>
								<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SENDER_ADDR3) %>
								<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SENDER_ADDR4) %>
								<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SENDER_ADDR5) %>
								<%#: (IsCountryJp((string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_ISO_CODE)) == false)
									? GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SENDER_ZIP)
									: "" %><br />
								<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_NAME) %>
							</td>
						</tr>
						<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
						<tr visible='<%# StringUtility.ToEmpty(this.OrderModel.GiftFlg) == Constants.FLG_ORDER_GIFT_FLG_ON %>' runat="server">
							<%-- 企業名・部署名 --%>
							<th><%: ReplaceTag("@@User.company_name.name@@")%>・
								<%: ReplaceTag("@@User.company_post_name.name@@")%></th>
							<td>
								<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_NAME) %><br />
								<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_POST_NAME) %>
							</td>
						</tr>
						<%} %>
						<tr visible='<%# StringUtility.ToEmpty(this.OrderModel.GiftFlg) == Constants.FLG_ORDER_GIFT_FLG_ON %>' runat="server">
							<%-- 氏名 --%>
							<th><%: ReplaceTag("@@User.name.name@@") %></th>
							<td>
								<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SENDER_NAME) %>&nbsp;様<br />
								<%#: IsCountryJp((string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_ISO_CODE))
									? "(" + GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA) + " さま)"
									: "" %>
							</td>
						</tr>
						<tr visible='<%# StringUtility.ToEmpty(this.OrderModel.GiftFlg) == Constants.FLG_ORDER_GIFT_FLG_ON %>' runat="server">
							<%-- 電話番号 --%>
							<th><%: ReplaceTag("@@User.tel1.name@@") %></th>
							<td>
								<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SENDER_TEL1) %>
							</td>
						</tr>
						<tr visible='<%# StringUtility.ToEmpty(this.OrderModel.GiftFlg) == Constants.FLG_ORDER_GIFT_FLG_ON %>' runat="server">
							<th colspan="2">お届け先</th>
						</tr>
						<div id="dShippingInfo" runat="server" visible="true">
							<% if (this.UseShippingAddress) { %>
							<% if (this.OrderModel.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT) { %>
							<tr>
								<th>
									<%: ReplaceTag("@@User.addr.name@@") %>
								</th>
								<td>
									<%# IsCountryJp((string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE))
										? "〒" + WebSanitizer.HtmlEncode(GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP)) + "<br />"
										: "" %>
									<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1) %>
									<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2) %>
									<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3) %>
									<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4) %>
									<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5) %>
									<%# (IsCountryJp((string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE)) == false)
										? WebSanitizer.HtmlEncode(GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP)) + "<br />"
										: "" %>
									<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_NAME) %>
									<% if (this.OrderModel.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2) { %>
									<div style="text-align:right; float:right; ">
										<asp:LinkButton ID="LinkButton1" Text="お届け先変更" runat="server" CommandArgument="<%# Container.ItemIndex %>" OnClientClick="return exec_submit();" OnClick="lbDisplayUserShippingInfoForm_Click" Enabled="<%# this.IsModifyShipping %>" class="btn" />
									</div>
									<div style="text-align:right; padding-top:15px;">
										<%#: this.ExplanationShipping %>
									</div>
									<% } else if(this.IsModifyShipping) { %>
									<%--▼▼ Amazon Pay(CV2)ボタン ▼▼--%>
									<div id="AmazonPayCv2Button" style="display: inline-block"></div>
									<%--▲▲ Amazon Pay(CV2)ボタン ▲▲--%>
									<% } %>
								</td>
							</tr>
							<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
							<tr>
								<%-- 企業名・部署名 --%>
								<th><%: ReplaceTag("@@User.company_name.name@@")%>・
									<%: ReplaceTag("@@User.company_post_name.name@@")%></th>
								<td>
									<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_NAME) %><br />
									<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_POST_NAME) %>
								</td>
							</tr>
							<%} %>
							<tr>
								<%-- 氏名 --%>
								<th><%: ReplaceTag("@@User.name.name@@") %></th>
								<td>
									<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME) %>&nbsp;様<br />
									<%#: IsCountryJp((string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE))
										? "(" + GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA) + " さま)" 
										: ""%>
								</td>
							</tr>
							<%} %>
							<tr>
								<%-- 電話番号 --%>
								<th><%: ReplaceTag("@@User.tel1.name@@") %></th>
								<td>
									<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %>
								</td>
							</tr>
							<% } else { %>
							<tr>
								<th>店舗ID</th>
								<td>
									<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID) %>
									<div style="text-align:right; float:right; ">
										<asp:LinkButton ID="lbDisplayUserShippingInfoForm" Text="お届け先変更" runat="server" CommandArgument="<%# Container.ItemIndex %>" OnClientClick="return exec_submit();" OnClick="lbDisplayUserShippingInfoForm_Click" Enabled="<%# this.IsModifyShipping %>" class="btn" />
									</div>
								</td>
							</tr>
							<tr>
								<th>店舗名称</th>
								<td><%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME) %></td>
							</tr>
							<tr>
								<th>店舗住所</th>
								<td><%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4) %></td>
							</tr>
							<tr>
								<th>店舗電話番号</th>
								<td><%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %></td>
							</tr>
							<% } %>
						</div>
						<% this.CartShippingItemIndexTmp++; %>
						<div id="dShippngInput" runat="server" visible="false">
							<tbody>
								<tr>
									<th></th>
									<td class="convenience-store-item">
										<asp:DropDownList
											DataTextField="Text"
											DataValueField="Value"
											SelectedValue='<%# IsDisplayButtonConvenienceStore((string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG))
												? CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE
												: null %>'
											ID="ddlShippingType"
											AutoPostBack="true"
											OnSelectedIndexChanged="ddlShippingType_SelectedIndexChanged"
											DataSource='<%# GetPossibleShippingType((string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG)) %>'
											runat="server"
											CssClass="UserShippingAddress">
										</asp:DropDownList>
										<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
										<asp:DropDownList
											DataTextField="Text"
											DataValueField="Value"
											SelectedValue='<%# GetShippingReceivingStoreTypeValue(
												IsDisplayButtonConvenienceStore((string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG)),
												(string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_TYPE)) %>'
											ID="ddlShippingReceivingStoreType"
											Visible='<%# IsDisplayButtonConvenienceStore((string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG)) %>'
											AutoPostBack="true"
											OnSelectedIndexChanged="ddlShippingReceivingStoreType_SelectedIndexChanged"
											DataSource='<%# ShippingReceivingStoreType() %>'
											runat="server"
											CssClass="UserShippingAddress" />
										<% } %>
										<% if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED) { %>
										<span id="spConvenienceStoreSelect" runat="server" visible='<%# (IsDisplayButtonConvenienceStore((string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG)) && (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false)) %>'>
											<a href='<%# string.Format(@"javascript:openConvenienceStoreMapPopup({0});", Container.ItemIndex) %>' class="btn btn-success convenience-store-button" >Family/OK/Hi-Life</a>
										</span>
										<span id="spConvenienceStoreEcPaySelect" runat="server" visible='<%# (IsDisplayButtonConvenienceStore((string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG)) && Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) %>'>
											<asp:LinkButton
												ID="lbOpenEcPay"
												runat="server"
												class="btn btn-success convenience-store-button"
												OnClick="lbOpenEcPay_Click"
												CommandArgument="<%# Container.ItemIndex %>"
												Text="  電子マップ  " />
										</span>
										<div id="dvErrorShippingConvenience" runat="server" style="display:none;">
											<span class="error_inline"><%#: WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_GROCERY_STORE) %></span>
										</div>
										<div id="dvErrorPaymentAndShippingConvenience" runat="server" visible="false">
											<span class="error_inline"><%#: WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAYMENT_METHOD_CHANGED_TO_CONVENIENCE_STORE) %></span>
										</div>
										<% } %>
									</td>
								</tr>
							</tbody>
							<div id="divShippingInputFormInner" runat="server">
							<%
								var shippingAddrCountryIsoCode = GetShippingAddrCountryIsoCode(this.CartShippingItemIndexTmp);
								var isShippingAddrCountryJp = IsCountryJp(shippingAddrCountryIsoCode);
								var isShippingAddrCountryUs = IsCountryUs(shippingAddrCountryIsoCode);
								var isShippingAddrCountryTw = IsCountryTw(shippingAddrCountryIsoCode);
								var isShippingAddrZipNecessary = IsAddrZipcodeNecessary(shippingAddrCountryIsoCode);
							%>
							<th><%: ReplaceTag("@@User.name.name@@") %> <span class="necessary">*</span></th>
								<td>
									<div style="float:right; float:right; ">
										<asp:LinkButton Text="お届け先変更" runat="server" CommandArgument="<%# Container.ItemIndex %>" OnClientClick="return exec_submit();" OnClick="lbDisplayUserShippingInfoForm_Click" Enabled="<%# this.IsModifyShipping %>" class="btn" />
									</div>
									姓：<asp:TextBox ID="tbShippingName1" runat="server" Width="90" MaxLength="10" ></asp:TextBox>
									名：<asp:TextBox ID="tbShippingName2" runat="server" Width="90" MaxLength="10" ></asp:TextBox>
									<asp:CustomValidator ID="CustomValidator1" runat="Server"
										ControlToValidate="tbShippingName1"
										ValidationGroup="OrderShipping"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										ClientValidationFunction="ClientValidate"
										CssClass="error_inline" />
									<asp:CustomValidator ID="CustomValidator2" runat="Server"
										ControlToValidate="tbShippingName2"
										ValidationGroup="OrderShipping"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										ClientValidationFunction="ClientValidate"
										CssClass="error_inline" />
								</td>
							</tr>
							<% if (isShippingAddrCountryJp) { %>
							<tr>
								<th><%: ReplaceTag("@@User.name_kana.name@@") %> <span class="necessary">*</span></th>
								<td>姓：<asp:TextBox ID="tbShippingNameKana1" runat="server" Width="90" MaxLength="20"></asp:TextBox>
									名：<asp:TextBox ID="tbShippingNameKana2" runat="server" Width="90" MaxLength="20"></asp:TextBox>
									<asp:CustomValidator ID="CustomValidator3" runat="Server"
										ControlToValidate="tbShippingNameKana1"
										ValidationGroup="OrderShipping"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										ClientValidationFunction="ClientValidate"
										CssClass="error_inline" />
									<asp:CustomValidator ID="CustomValidator4" runat="Server"
										ControlToValidate="tbShippingNameKana2"
										ValidationGroup="OrderShipping"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										ClientValidationFunction="ClientValidate"
										CssClass="error_inline" />
								</td>
							</tr>
							<% } %>
							<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
							<tr>
								<th>
									<%: ReplaceTag("@@User.country.name@@", shippingAddrCountryIsoCode) %>
									<span class="necessary">*</span>
								</th>
								<td>
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
								</td>
							</tr>
							<% } %>
							<tr>
								<% if (isShippingAddrCountryJp) { %>
								<th>
									<%: ReplaceTag("@@User.zip.name@@") %>
									<span class="necessary">*</span>
								</th>
								<td>
									<asp:TextBox ID="tbShippingZip" Width="180" MaxLength="8" ValidationGroup="<%# Container.ItemIndex %>" OnTextChanged="lbSearchAddr_TextBox_Click" runat="server" />
									<asp:LinkButton ID="lbSearchShippingAddr" runat="server" CommandArgument="<%# Container.ItemIndex %>" OnClientClick="return false;" OnClick="lbSearchAddr_LinkButton_Click" class="btn btn-mini">
									郵便番号から住所を入力
									</asp:LinkButton><br />
									<%--検索結果レイヤー--%>
									<uc:Layer ID="ucLayerForOwner" runat="server" />
									<asp:CustomValidator
										ID="cvShippingZip1"
										runat="Server"
										ControlToValidate="tbShippingZip"
										ValidationGroup="OrderShipping"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										ClientValidationFunction="ClientValidate"
										CssClass="error_inline zip_input_error_message" />
									<small id="sShippingZipError" runat="server" class="fred shortZipInputErrorMessage"></small>
								</td>
							</tr>
							<tr>
								<%-- 都道府県 --%>
								<th>
									<%: ReplaceTag("@@User.addr1.name@@") %>
									<span class="necessary">*</span>
								</th>
								<td>
									<asp:DropDownList ID="ddlShippingAddr1" runat="server" DataTextField="<%#: Container.ItemIndex %>" OnSelectedIndexChanged="ddlShippingAddr1_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
									<asp:CustomValidator
										ID="cvShippingAddr1"
										runat="Server"
										ControlToValidate="ddlShippingAddr1"
										ValidationGroup="OrderShipping"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										ClientValidationFunction="ClientValidate"
										CssClass="error_inline" />
								</td>
							</tr>
							<% } %>
							<tr>
								<%-- 市区町村 --%>
								<th>
									<%: ReplaceTag("@@User.addr2.name@@", shippingAddrCountryIsoCode) %>
									<span class="necessary">*</span>
								</th>
								<td>
									<% if (isShippingAddrCountryTw) { %>
										<asp:DropDownList runat="server" ID="ddlShippingAddr2" DataSource="<%# this.UserTwCityList %>" AutoPostBack="true" DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlShippingAddr2_SelectedIndexChanged"></asp:DropDownList>
									<% } else { %>
										<asp:TextBox ID="tbShippingAddr2" runat="server" Width="300" MaxLength="40"></asp:TextBox>
										<asp:CustomValidator
											ID="cvShippingAddr2"
											runat="Server"
											ControlToValidate="tbShippingAddr2"
											ValidationGroup="OrderShipping"
											ValidateEmptyText="true"
											SetFocusOnError="true"
											ClientValidationFunction="ClientValidate"
											CssClass="error_inline" />
									<% } %>
								</td>
							</tr>
							<tr>
								<%-- 番地 --%>
								<th>
									<%: ReplaceTag("@@User.addr3.name@@", shippingAddrCountryIsoCode) %>
									<% if (IsAddress3Necessary(shippingAddrCountryIsoCode)){ %><span class="necessary">*</span><% } %>
								</th>
								<td>
									<% if (isShippingAddrCountryTw) { %>
										<asp:DropDownList runat="server" ID="ddlShippingAddr3" DataTextField="Key" DataValueField="Value" Width="95" ></asp:DropDownList>
									<% } else { %>
										<asp:TextBox ID="tbShippingAddr3" runat="server" Width="300" MaxLength="50"></asp:TextBox>
										<asp:CustomValidator
											ID="cvShippingAddr3"
											runat="Server"
											ControlToValidate="tbShippingAddr3"
											ValidationGroup="OrderShipping"
											ValidateEmptyText="true"
											SetFocusOnError="true"
											ClientValidationFunction="ClientValidate"
											CssClass="error_inline" />
									<% } %>
								</td>
							</tr>
							<tr <%: (Constants.DISPLAY_ADDR4_ENABLED || (isShippingAddrCountryJp == false)) ? "" : "style=\"display:none;\""  %>>
								<%-- ビル・マンション名 --%>
								<th>
									<%: ReplaceTag("@@User.addr4.name@@", shippingAddrCountryIsoCode) %>
									<% if (isShippingAddrCountryJp == false) {%><span class="necessary">*</span><% } %>
								</th>
								<td>
									<asp:TextBox ID="tbShippingAddr4" runat="server" Width="300" MaxLength="50"></asp:TextBox>
									<asp:CustomValidator
										ID="cvShippingAddr4"
										runat="Server"
										ControlToValidate="tbShippingAddr4"
										ValidationGroup="OrderShipping"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										ClientValidationFunction="ClientValidate"
										CssClass="error_inline" />
								</td>
							</tr>
							<% if (isShippingAddrCountryJp == false) { %>
							<tr>
								<%-- 州 --%>
								<th>
									<%: ReplaceTag("@@User.addr5.name@@", shippingAddrCountryIsoCode) %>
									<% if (isShippingAddrCountryUs) { %> <span class="necessary">*</span><% } %>
								</th>
								<td>
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
											CssClass="error_inline" />
									<% } else { %>
									<asp:TextBox runat="server" ID="tbShippingAddr5"></asp:TextBox>
									<asp:CustomValidator
										ID="cvShippingAddr5"
										runat="Server"
										ControlToValidate="tbShippingAddr5"
										ValidationGroup="OrderShippingGlobal"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										ClientValidationFunction="ClientValidate"
										CssClass="error_inline" />
									<% } %>
								</td>
							</tr>
							<tr>
								<%-- 郵便番号（海外向け） --%>
								<th>
									<%: ReplaceTag("@@User.zip.name@@", shippingAddrCountryIsoCode) %>
									<% if (isShippingAddrZipNecessary) { %><span class="necessary">*</span><% } %>
								</th>
								<td>
									<asp:TextBox runat="server" ID="tbShippingZipGlobal" MaxLength="20"></asp:TextBox>
									<asp:CustomValidator
										ID="cvShippingZipGlobal"
										runat="Server"
										ControlToValidate="tbShippingZipGlobal"
										ValidationGroup="OrderHistoryDetailGlobal"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										ClientValidationFunction="ClientValidate"
										CssClass="error_inline" />
									<asp:LinkButton
										ID="lbSearchAddrFromShippingZipGlobal"
										OnClick="lbSearchAddrFromShippingZipGlobal_Click"
										CommandArgument="<%# Container.ItemIndex %>"
										Style="display:none;"
										runat="server" />
								</td>
							</tr>
							<% } %>
							<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
							<tr>
								<th><%: ReplaceTag("@@User.company_name.name@@") %> </th>
								<td>
									<asp:TextBox ID="tbShippingCompanyName" runat="server" Width="300" MaxLength="50"></asp:TextBox>
									<asp:CustomValidator
										ID="cvShippingCompanyName"
										runat="Server"
										ControlToValidate="tbShippingCompanyName"
										ValidationGroup="OrderShipping"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										ClientValidationFunction="ClientValidate"
										CssClass="error_inline" />
								</td>
							</tr>
							<tr>
								<th><%: ReplaceTag("@@User.company_post_name.name@@") %> </th>
								<td>
									<asp:TextBox ID="tbShippingCompanyPostName" runat="server" Width="300" MaxLength="50"></asp:TextBox>
									<asp:CustomValidator
										ID="cvShippingCompanyPostName"
										runat="Server"
										ControlToValidate="tbShippingCompanyPostName"
										ValidationGroup="OrderShipping"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										ClientValidationFunction="ClientValidate"
										CssClass="error_inline" />
								</td>
							</tr>
							<% } %>
							<tr>
								<% if (isShippingAddrCountryJp) { %>
								<th>
									<%: ReplaceTag("@@User.tel1.name@@") %>
									<span class="necessary">*</span></th>
								<td>
									<asp:TextBox ID="tbShippingTel1" Width="180" MaxLength="13" runat="server" CssClass="shortTel" />
									<asp:CustomValidator
										ID="cvShippingTel1_1"
										runat="Server"
										ControlToValidate="tbShippingTel1"
										ValidationGroup="OrderShipping"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										ClientValidationFunction="ClientValidate"
										CssClass="error_inline" />
								</td>
								<% } else { %>
								<th>
									<%: ReplaceTag("@@User.tel1.name@@", shippingAddrCountryIsoCode) %>
									<span class="necessary">*</span></th>
								<td>
									<asp:TextBox runat="server" ID="tbShippingTel1Global" MaxLength="30"></asp:TextBox>
									<asp:CustomValidator
										ID="cvShippingTel1Global"
										runat="Server"
										ControlToValidate="tbShippingTel1Global"
										ValidationGroup="OrderShippingGlobal"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										ClientValidationFunction="ClientValidate"
										CssClass="error_inline" />
								</td>
								<% } %>
							</tr>
							</div>
							<%-- ▽コンビニ▽ --%>
							<asp:HiddenField ID="hfCvsShopFlg" runat="server" Value='<%# GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG) %>' />
							<asp:HiddenField ID="hfSelectedShopId" runat="server" Value='<%# GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID) %>' />
							<tbody id="divConvenienceStore" class="<%# Container.ItemIndex %>" runat="server">
								<tr>
									<th>店舗ID</th>
									<td class="convenience-store-item" id="ddCvsShopId">
										<span style="font-weight:normal;">
											<asp:Literal ID="lCvsShopId" runat="server" Text='<%# GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID) %>'/>
										</span>
										<asp:HiddenField ID="hfCvsShopId" runat="server" Value='<%# GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID) %>' />
									</td>
								</tr>
								<tr>
									<th>店舗名称</th>
									<td class="convenience-store-item" id="ddCvsShopName">
										<span style="font-weight:normal;">
											<asp:Literal ID="lCvsShopName" runat="server" Text='<%# GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME) %>'/>
										</span>
										<asp:HiddenField ID="hfCvsShopName" runat="server" Value='<%# GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME) %>' />
									</td>
								</tr>
								<tr>
									<th>店舗住所</th>
									<td class="convenience-store-item" id="ddCvsShopAddress">
										<span style="font-weight:normal;">
											<asp:Literal ID="lCvsShopAddress" runat="server" Text='<%# GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4) %>'></asp:Literal>
										</span>
										<asp:HiddenField ID="hfCvsShopAddress" runat="server" Value='<%# GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4) %>' />
									</td>
								</tr>
								<tr>
									<th>店舗電話番号</th>
									<td class="convenience-store-item" id="ddCvsShopTel">
										<span style="font-weight:normal;">
											<asp:Literal ID="lCvsShopTel" runat="server" Text='<%# GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %>'></asp:Literal>
										</span>
										<asp:HiddenField ID="hfCvsShopTel" runat="server" Value='<%# GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %>' />
									</td>
								</tr>
							</tbody>
							<tbody id="tbOwnerAddress" runat="server" visible="false">
								<tr>
									<th>
										<%: ReplaceTag("@@User.addr.name@@") %>
									</th>
									<td>
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
									</td>
								</tr>
								<% if (Constants.DISPLAY_CORPORATION_ENABLED)
								{ %>
								<tr>
									<%-- 企業名・部署名 --%>
									<th>
										<%: ReplaceTag("@@User.company_name.name@@")%>・
										<%: ReplaceTag("@@User.company_post_name.name@@")%>
									</th>
									<td>
										<%#: this.LoginUser.CompanyName %><br />
										<%#: this.LoginUser.CompanyPostName %>
									</td>
								</tr>
								<%} %>
								<tr>
									<%-- 氏名 --%>
									<th><%: ReplaceTag("@@User.name.name@@") %></th>
									<td>
										<%#: this.LoginUser.Name %>&nbsp;様<br />
										<%#: IsCountryJp(this.LoginUser.AddrCountryIsoCode)
													? "(" + this.LoginUser.NameKana + " さま)" 
													: "" %>
									</td>
								</tr>
								<tr>
									<%-- 電話番号 --%>
									<th><%: ReplaceTag("@@User.tel1.name@@") %></th>
									<td>
										<%#: this.LoginUser.Tel1 %>
									</td>
								</tr>
							</tbody>
						<asp:Repeater Visible="false" runat="server" ItemType="UserShippingModel" DataSource="<%# this.UserShippingAddr %>" ID="rOrderShippingList">
								<ItemTemplate>
									<tbody runat="server" class='<%# string.Format("user_addres{0}", Item.ShippingNo) %>' visible="<%# (Item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON) %>">
										<asp:HiddenField runat="server" ID="hfShippingNo" Value="<%# Item.ShippingNo %>" />
										<tr>
											<th>店舗ID</th>
											<td class="convenience-store-item" id="ddCvsShopId">
												<span style="font-weight:normal;">
													<asp:Literal ID="lCvsShopId" runat="server" Text='<%# Item.ShippingReceivingStoreId %>' />
												</span>
												<asp:HiddenField ID="hfCvsShopId" runat="server" Value="<%# Item.ShippingReceivingStoreId %>" />
											</td>
										</tr>
										<tr>
											<th>店舗名称</th>
											<td class="convenience-store-item" id="ddCvsShopName">
												<span style="font-weight:normal;">
													<asp:Literal ID="lCvsShopName" runat="server" Text='<%# Item.ShippingName %>' />
												</span>
												<asp:HiddenField ID="hfCvsShopName" runat="server" Value="<%# Item.ShippingName %>" />
											</td>
										</tr>
										<tr>
											<th>店舗住所</th>
											<td class="convenience-store-item" id="ddCvsShopAddress">
												<span style="font-weight:normal;">
													<asp:Literal ID="lCvsShopAddress" runat="server" Text='<%# Item.ShippingAddr4 %>'></asp:Literal>
												</span>
												<asp:HiddenField ID="hfCvsShopAddress" runat="server" Value="<%# Item.ShippingAddr4 %>" />
											</td>
										</tr>
										<tr>
											<th>店舗電話番号</th>
											<td class="convenience-store-item" id="ddCvsShopTel">
												<span style="font-weight:normal;">
													<asp:Literal ID="lCvsShopTel" runat="server" Text='<%# Item.ShippingTel1 %>'></asp:Literal>
												</span>
												<asp:HiddenField ID="hfCvsShopTel" runat="server" Value="<%# Item.ShippingTel1 %>" />
											</td>
										</tr>
									</tbody>
									<tbody runat="server" visible="<%# (Item.ShippingReceivingStoreFlg != Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON) %>">
										<tr>
											<th>
												<%: ReplaceTag("@@User.addr.name@@") %>
											</th>
											<td>
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
											</td>
										</tr>
										<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
										<tr>
											<%-- 企業名・部署名 --%>
											<th><%: ReplaceTag("@@User.company_name.name@@")%>・
													<%: ReplaceTag("@@User.company_post_name.name@@")%></th>
											<td>
												<%#: Item.ShippingCompanyName %><br />
												<%#: Item.ShippingCompanyPostName %>
											</td>
										</tr>
										<%} %>
										<tr>
											<%-- 氏名 --%>
											<th><%: ReplaceTag("@@User.name.name@@") %></th>
											<td>
												<%#: Item.ShippingName %>&nbsp;様<br />
												<%#: IsCountryJp(Item.ShippingCountryIsoCode)
													? "(" + Item.ShippingNameKana + " さま)" 
													: "" %>
											</td>
										</tr>
										<tr>
											<%-- 電話番号 --%>
											<th><%: ReplaceTag("@@User.tel1.name@@") %></th>
											<td>
												<%#: Item.ShippingTel1 %>
											</td>
										</tr>
									</tbody>
								</ItemTemplate>
							</asp:Repeater>
							<tr>
								<th></th>
								<td>
									<div id="divOrderShippingUpdateButtons" style="display: block">
										<% if (this.IsFixedPurchase && (this.FixedPurchaseModel.IsCancelFixedPurchaseStatus == false) && (this.IsUpdateShippingFixedPurchase)) { %>
											<asp:CheckBox ID="cbIsUpdateFixedPurchaseByOrderShippingInfo" Text="今後の定期注文にも反映させる" Checked="false" runat="server"/><br />
										<% } %>
										<asp:LinkButton Text="情報更新" runat="server" ValidationGroup="OrderShipping" CommandArgument="<%# Container.ItemIndex %>" OnClientClick="return AlertDataChange('Shipping', this);" OnClick="lbUpdateUserShippingInfo_Click" class="btn" ></asp:LinkButton>
										<asp:LinkButton Text="キャンセル" runat="server" CommandArgument="<%# Container.ItemIndex %>" OnClientClick="return exec_submit();"  OnClick="lbHideUserShippingInfoForm_Click" class="btn" ></asp:LinkButton>
										<input type="hidden" id="parentShippingRepeater" name="parentShippingRepeater" value="<%#: Container.UniqueID %>" />
									</div>
									<div id="divOrderShippingUpdateExecFroms" style="display: none"> 
										更新中です...
									</div>
									<small id="sErrorMessageShipping" runat="server" class="error" style="padding: 2px;"></small>
								</td>
							</tr>
						</div>
						<% if ((this.OrderModel.Shippings[0].ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF)
							|| (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false)) { %>
						<tr>
							<th>配送方法</th>
							<td>
								<%#: ValueText.GetValueText(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD, GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD)) %>
							</td>
						</tr>
						<% } %>
						<tr>
							<th>配送サービス</th>
							<td>
								<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_NAME) %>
							</td>
						</tr>
						<div visible='<%# (string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD) != Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL %>' runat="server">
							<% if ((this.OrderModel.Shippings[0].ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF)
								|| (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false)) { %>
							<tr runat="server" visible='<%# (string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_FLG) == Constants.FLG_SHOPSHIPPING_SHIPPING_DATE_SET_FLG_VALID %>'>
								<th>配送希望日</th>
								<td>
									<asp:DropDownList ID="ddlShippingDateList" Visible="false" runat="server" DataSource="<%# GetListShippingDate() %>" DataTextField="text" DataValueField="value"></asp:DropDownList>
									<div id="dvShippingDateText"  runat="server" ><%#: DateTimeUtility.ToStringFromRegion(GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE), DateTimeUtility.FormatType.LongDateWeekOfDay2Letter, ReplaceTag("@@DispText.shipping_date_list.none@@")) %></div>
									<div style="text-align:right; float:right;">
										<asp:LinkButton CommandArgument="<%# Container.ItemIndex %>" Text="配送希望日変更" runat="server" OnClick="lbDisplayInputShippingDate_Click" Enabled="<%# this.IsModifyShippingDates[Container.ItemIndex] %>" class="btn" />
									</div>
									<div style="text-align:right; padding-top:30px;"><%#: this.ExplanationShippingDates[Container.ItemIndex] %></div>
									<asp:Label ID="lShippingDateErrorMessage" CssClass="fred" runat="server" Visible="false"></asp:Label>
								</td>
							</tr>
							<% } %>
							<tr id="trShippingDateInput" visible='false'  runat="server">
								<th></th>
								<td>
									<div id="divShippingDateUpdateButtons" style="display: block"> 
										<asp:LinkButton Text="情報更新" runat="server" ValidationGroup="OrderShipping" CommandArgument="<%# Container.ItemIndex %>" OnClientClick="return AlertUpdateShippingDate(this);" OnClick="lbUpdateShippingDate_Click" class="btn" ></asp:LinkButton>
										<asp:LinkButton Text="キャンセル" runat="server" CommandArgument="<%# Container.ItemIndex %>" OnClick="lbHideShippingDate_Click" class="btn" />
										<input type="hidden" id="parentShippingDateRepeater" name="parentShippingDateRepeater" value="<%#: Container.UniqueID %>" />
									</div>
									<div id="divShippingDateUpdateExecFroms" style="display: none"> 
										更新中です...
									</div>
									<small id="sErrorMessageShippingDate" runat="server" class="error" style="padding: 2px;"></small>
								</td>
							</tr>
							<% if (this.OrderModel.Shippings[0].ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF) { %>
							<tr runat="server" visible="<%# this.DisplayScheduledShippingDate %>">
								<th>出荷予定日</th>
								<td>
									<%#: DateTimeUtility.ToStringFromRegion(GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE), DateTimeUtility.FormatType.LongDateWeekOfDay2Letter, ReplaceTag("@@DispText.shipping_date_list.none@@")) %>
								</td>
							</tr>
							<% } %>
							<% if (this.UseShippingAddress) { %>
							<tr runat="server" visible='<%# (string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG) == Constants.FLG_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG_VALID %>'>
								<th>配送希望時間帯</th>
								<td>
									<asp:DropDownList ID="ddlShippingTimeList" Visible="false" runat="server" DataSource='<%# GetShippingTimeList((string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID)) %>' DataTextField="text" DataValueField="value" ></asp:DropDownList>
									<div id="dvShippingTimeText"  runat="server">
										<%#: ((w2.Common.Util.Validator.IsNullEmpty(GetKeyValue(((Hashtable)Container.DataItem)["row"], "shipping_time_message")) == false)
											&& (StringUtility.ToEmpty(GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG)) == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF))
												? GetKeyValue(((Hashtable)Container.DataItem)["row"], "shipping_time_message")
												: ReplaceTag("@@DispText.shipping_time_list.none@@") %>
									</div>
									<div style="text-align:right; float:right;">
										<asp:LinkButton CommandArgument="<%# Container.ItemIndex %>" Text="配送時間帯変更" runat="server" OnClick="lbDisplayInputShippingTime_Click" Enabled="<%# this.IsModifyShippingTimes[Container.ItemIndex] %>" class="btn" />
									</div>
									<div style="text-align:right; padding-top:30px;"><%#: this.ExplanationShippingTimes[Container.ItemIndex] %></div>
								</td>
							</tr>
							<% } %>
							<tr id="trShippingTimeInput" visible='false'  runat="server">
								<th></th>
								<td>
									<div id="divShippingTimeUpdateButtons" style="display: block"> 
										<asp:LinkButton Text="情報更新" runat="server" ValidationGroup="OrderShipping" CommandArgument="<%# Container.ItemIndex %>" OnClientClick="return AlertUpdateShippingTime(this);" OnClick="lbUpdateShippingTime_Click" class="btn" ></asp:LinkButton>
										<asp:LinkButton Text="キャンセル" runat="server" CommandArgument="<%# Container.ItemIndex %>" OnClick="lbHideShippingTime_Click" class="btn" />
										<input type="hidden" id="parentShippingTimeRepeater" name="parentShippingTimeRepeater" value="<%#: Container.UniqueID %>" />
									</div>
									<div id="divShippingTimeUpdateExecFroms" style="display: none"> 
										更新中です...
									</div>
									<small id="sErrorMessageShippingTime" runat="server" class="error" style="padding: 2px;"></small>
								</td>
							</tr>
						</div>
						<tr visible='<%# (string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO) != "" %>' runat="server">
							<th>配送伝票番号</th>
							<td>
								<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO) %>
								<% if(Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED && IsShowCheckDeliveryStatus()) { %>
									<a href="javascript:openSiteMapPopup('<%# StringUtility.ToEmpty(GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO)) %>');" class="btn">配送状況確認</a>
								<% } %>
							</td>
						</tr>
						<tbody visible='<%# Constants.GIFTORDER_OPTION_ENABLED %>' runat="server">
						<tbody visible='<%# StringUtility.ToEmpty(this.OrderModel.GiftFlg) == Constants.FLG_ORDER_GIFT_FLG_ON %>' runat="server">
						<tr visible='<%# StringUtility.ToEmpty(this.ShopShippingModel.WrappingPaperFlg) == Constants.FLG_SHOPSHIPPING_WRAPPING_PAPER_FLG_VALID %>' runat="server">
							<th>のし種類</th>
							<td>
								<%#: (string.IsNullOrEmpty((string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_TYPE)) == false) ? WebSanitizer.HtmlEncodeChangeToBr(GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_TYPE)) : "なし" %>
							</td>
						</tr>
						<tr visible='<%# StringUtility.ToEmpty(this.ShopShippingModel.WrappingPaperFlg) == Constants.FLG_SHOPSHIPPING_WRAPPING_PAPER_FLG_VALID %>' runat="server">
							<th>のし差出人</th>
							<td>
								<%#: (string.IsNullOrEmpty((string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_NAME)) == false) ? WebSanitizer.HtmlEncodeChangeToBr(GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_NAME)) : "なし" %>
							</td>
						</tr>
						<tr visible='<%# StringUtility.ToEmpty(this.ShopShippingModel.WrappingPaperFlg) == Constants.FLG_SHOPSHIPPING_WRAPPING_BAG_FLG_VALID %>' runat="server">
							<th>包装種類</th>
							<td>
								<%#: (string.IsNullOrEmpty((string)GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_WRAPPING_BAG_TYPE)) == false) ? WebSanitizer.HtmlEncodeChangeToBr(GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_WRAPPING_BAG_TYPE)) : "なし" %>
							</td>
						</tr>
						</tbody>
						</tbody>
						<% if (this.IsShowTaiwanOrderInvoiceInfo) { %>
							<tr>
								<th>発票番号</th>
								<td><%: this.TwOrderInvoiceModel.TwInvoiceNo %></td>
							</tr>
							<tr>
								<th>発票種類</th>
								<td>
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
								</td>
							</tr>
							<% if (this.TwOrderInvoiceModel.TwUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL) { %>
								<tr>
									<th>共通性載具</th>
									<td>
										<%: ValueText.GetValueText(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE, this.TwOrderInvoiceModel.TwCarryType) %>
										<br />
										<%: this.TwOrderInvoiceModel.TwCarryTypeOption %>
									</td>
								</tr>
							<% } %>
						<% } %>
					</table>
					<% } else if (this.RealShopModel != null) { %>
					<table>
						<tr>
							<th>受取店舖</th>
							<td><%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME) %></td>
						</tr>
						<tr>
							<th>店舖住所</th>
							<td><%#: "〒" + GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP) %>
								<br />
								<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1) %>
								<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2) %><br />
								<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3) %><br />
								<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4) %><br />
								<%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5) %>
							</td>
						</tr>
						<tr>
							<th>営業時間</th>
							<td><%: this.RealShopModel.OpeningHours %></td>
						</tr>
						<tr>
							<th>店舖電話番号</th>
							<td><%#: GetKeyValue(((Hashtable)Container.DataItem)["row"], Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %></td>
						</tr>
						<tr>
							<th>配送方法</th>
							<td>店舗受取</td>
						</tr>
					</table>
					<% } %>
				</div>
				<%} %>
				<br />
				<% if (this.HasSubscriptionBoxItemModify) { %>
				<div class="error">
					<%: WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SUBSCRIPTION_BOX_ERROR_MESSAGE) %>
				</div>
				<% } %>
				<%-- 購入商品一覧 --%>
				<div class="dvOrderHistoryProduct" ID="dvOrderHistoryProduct" runat="server">
					<table cellspacing="0">
						<asp:Repeater ID="rOrderShippingItem" DataSource='<%# ((Hashtable)Container.DataItem)["childs"] %>' Runat="server">
							<HeaderTemplate>
								<tr>
									<th class="productName">
										商品名
									</th>
									<% if (this.OrderModel.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false) { %>
									<th class="productPrice">
										単価（<%#: this.ProductPriceTextPrefix %>）
									</th>
									<% } %>
									<% if (HasOptionPrice(this.OrderShippingItems)) { %>
										<th class="productPrice">
											オプション価格（<%#: this.ProductPriceTextPrefix %>）
										</th>
									<% } %>
									<th class="orderCount">
										注文数
									</th>
									<% if (this.OrderModel.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false) { %>
									<th class="taxRate">
										消費税率
									</th>
									<th class="orderSubtotal">
										小計（<%#: this.ProductPriceTextPrefix %>）
									</th>
									<% } %>
								</tr>
							</HeaderTemplate>
							<ItemTemplate>
								
								<%-- 通常商品 --%>
								<tr class='<%# IsLastItemOnOrderLine((DataRowView)Container.DataItem) == false ? "fixed-amount-course-item-line" : string.Empty %>' visible='<%# (((string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_SET_ID)).Length == 0) %>' runat="server">
									<td class="productName">
										<%-- 一致する商品IDが現在も存在する場合、商品詳細ページへのリンクを表示する --%>
										<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailVariationUrl(Container.DataItem)) %>' target="_blank" runat="server" Visible="<%# IsProductDetailLinkValid((DataRowView)Container.DataItem) %>">
											<%#: Eval(Constants.FIELD_ORDERITEM_PRODUCT_NAME) %>
										</a>
										<%# (IsProductDetailLinkValid((DataRowView)Container.DataItem) == false) ? WebSanitizer.HtmlEncode(Eval(Constants.FIELD_ORDERITEM_PRODUCT_NAME)) : ""%>
										<br />
										[<span class="productId"><%#: Eval(Constants.FIELD_ORDERITEM_VARIATION_ID) %></span>]
										<span visible='<%# (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS) != "" %>' runat="server">
											<br />
											<%# WebSanitizer.HtmlEncodeChangeToBr(ProductOptionSettingHelper.GetDisplayProductOptionTexts((string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS)).Replace("　", "\r\n")) %>
										</span>
										<small>
											<asp:Repeater DataSource='<%# this.OrderItemSerialKeys[((string)Eval(Constants.FIELD_ORDER_ORDER_ID)) + (Eval(Constants.FIELD_ORDERITEM_ORDER_ITEM_NO).ToString())] %>' runat="server">
											<ItemTemplate>
												<br />
												&nbsp;シリアルキー:&nbsp;<%# Eval(Constants.FIELD_SERIALKEY_SERIAL_KEY)%>
											</ItemTemplate>
											</asp:Repeater>
										</small>
									</td>
									<td class="productPrice" Visible="<%# this.OrderModel.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
										<%#: IsOrderItemSubscriptionBoxFixedAmount(Eval(Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT_WITH_PREFIX)) == false ? CurrencyManager.ToPrice(Eval(Constants.FIELD_ORDERITEM_PRODUCT_PRICE)) : string.Empty %>
									</td>
									<td class="productPrice" Visible="<%# HasOptionPrice(this.OrderShippingItems) %>" runat="server">
										<%#: CurrencyManager.ToPrice(ProductOptionSettingHelper.ExtractOptionPriceFromProductOptionTexts((string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS))) %>
									</td>
									<td class="orderCount">
										<%#: StringUtility.ToNumeric(Eval(Constants.FIELD_ORDERITEM_ITEM_QUANTITY)) %>
									</td>
									<td class="fixed-amount-course-container" Visible="<%# IsFirstItemInFixedAmountCourse((string)Eval(Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID_WITH_PREFIX), Eval(Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT_WITH_PREFIX)) %>" colspan="2" rowspan='<%# this.OrderModel.GetFixedAmountCourseItemCount((string)Eval(Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID_WITH_PREFIX)) %>' runat="server">
										<p>頒布会コース名：&nbsp;<%#: GetSubscriptionBoxDisplayName((string)Eval(Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID_WITH_PREFIX)) %></p>
										<p>定額：&nbsp;<%#: CurrencyManager.ToPrice(Eval(Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT_WITH_PREFIX)) %>(<%#: this.ProductPriceTextPrefix %>)</p>
										<p>税率：&nbsp;<%#: TaxCalculationUtility.GetTaxRateForDIsplay(Eval(Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE)) %>%</p>
									</td>
									<td class="taxRate" Visible="<%# IsOrderItemSubscriptionBoxFixedAmount(Eval(Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT_WITH_PREFIX)) == false %>" runat="server">
										<%#: TaxCalculationUtility.GetTaxRateForDIsplay(Eval(Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE)) %>
									</td>
									<td class="orderSubtotal" Visible="<%# IsOrderSubtotalVisible(StringUtility.ToEmpty(Eval(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO)), Eval(Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT_WITH_PREFIX)) %>" runat="server">
										<%#: CurrencyManager.ToPrice(Eval(Constants.FIELD_ORDERITEM_ITEM_PRICE)) %>
									</td>
									<td class="orderSubtotal" Visible="<%# IsOrderSubtotalVisible(StringUtility.ToEmpty(Eval(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO)), Eval(Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT_WITH_PREFIX), (string.IsNullOrEmpty(StringUtility.ToEmpty(Eval(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO))) ? -1 : (int)Eval(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_ITEM_NO))) %>" rowspan="<%# GetSetPromotionRowSpan(Container.DataItem, ((Repeater)Container.Parent).DataSource) %>" runat="server">
										<%#: Eval(Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME) %><br />
										<span visible='<%# ((StringUtility.ToEmpty(Eval(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO)) != "") && ((string)Eval(Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_FLG) == Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_ON)) || (IsOrderItemSubscriptionBoxFixedAmount(Eval(Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT_WITH_PREFIX)) == false) %>' runat="server">
											<strike><%#: CurrencyManager.ToPrice(Eval(Constants.FIELD_ORDERSETPROMOTION_UNDISCOUNTED_PRODUCT_SUBTOTAL)) %></strike><br />
										</span>
										<%#: CurrencyManager.ToPrice(((StringUtility.ToEmpty(Eval(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO)) != "") ? (decimal)Eval(Constants.FIELD_ORDERSETPROMOTION_UNDISCOUNTED_PRODUCT_SUBTOTAL) : 0) - ((StringUtility.ToEmpty(Eval(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO)) != "") ? (decimal)Eval(Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_AMOUNT) : 0)) %>(税込)
									</td>
								</tr>

								<%-- セット商品 --%>
								<tr visible='<%# (((string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_SET_ID)).Length != 0) %>' runat="server">
									<td class="productName">
										<%-- 一致する商品IDが現在も存在する場合、商品詳細ページへのリンクを表示する --%>
										<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailVariationUrl(Container.DataItem)) %>' target="_blank" runat="server" Visible="<%# IsProductDetailLinkValid((DataRowView)Container.DataItem) %>">
											<%#: Eval(Constants.FIELD_ORDERITEM_PRODUCT_NAME) %>
										</a>
										<%# (IsProductDetailLinkValid((DataRowView)Container.DataItem) == false) ? WebSanitizer.HtmlEncode(Eval(Constants.FIELD_ORDERITEM_PRODUCT_NAME)) : ""%>
										<br />
										[<span class="productId"><%#: Eval(Constants.FIELD_ORDERITEM_VARIATION_ID) %></span>]
									</td>
									<td class="productPrice" Visible="<%# this.OrderModel.IsSubscriptionBoxFixedAmount == false %>" runat="server">
										<%#: CurrencyManager.ToPrice(Eval(Constants.FIELD_ORDERITEM_PRODUCT_PRICE)) %>
										x
										<%#: StringUtility.ToNumeric(Eval(Constants.FIELD_ORDERITEM_ITEM_QUANTITY_SINGLE)) %>
									</td>
									<td class="orderCount" rowspan='<%# GetProductSetRowspan(Container.DataItem, ((Repeater)Container.Parent).DataSource) %>' visible='<%# IsProductSetItemTop(Container.DataItem, ((Repeater)Container.Parent).DataSource) %>' runat="server">
										<%#: StringUtility.ToNumeric(Eval(Constants.FIELD_ORDERITEM_PRODUCT_SET_COUNT)) %>
									</td>
									<td class="taxRate" Visible="<%# this.OrderModel.IsSubscriptionBoxFixedAmount == false %>" runat="server">
										<%#: TaxCalculationUtility.GetTaxRateForDIsplay(Eval(Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE)) %>
									</td>
									<td class="orderSubtotal" rowspan='<%# GetProductSetRowspan(Container.DataItem, ((Repeater)Container.Parent).DataSource) %>' Visible='<%# (this.OrderModel.IsSubscriptionBoxFixedAmount == false) && (IsProductSetItemTop(Container.DataItem, ((Repeater)Container.Parent).DataSource)) %>' runat="server">
										<%#: CurrencyManager.ToPrice(CreateSetPriceSubtotal(Container.DataItem, ((Repeater)Container.Parent).DataSource)) %>
									</td>
								</tr>
							</ItemTemplate>
						</asp:Repeater>
					</table>
				</div>
			
			<%-- 追加商品一覧 --%>
				<div class="dvOrderHistoryProduct" ID="dvOrderModifyInput" Visible="False" runat="server">
					<table cellspacing="0">
						<asp:Repeater ID="rOrderInputShippingItem" ItemType="w2.App.Common.Input.Order.OrderItemInput" OnItemDataBound="rOrderInputShippingItem_OnItemDataBound" Runat="server">
							<HeaderTemplate>
								<tr>
									<th class="productName">
										商品名
									</th>
									<% if (this.OrderModel.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false) { %>
										<th class="productPrice">
											単価（<%#: this.ProductPriceTextPrefix %>）
										</th>
									<% } %>
									<th class="orderCount">
										注文数
									</th>
									<% if (this.OrderModel.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false) { %>
										<th class="taxRate">
											消費税率
										</th>
										<th class="orderSubtotal">
											小計（<%#: this.ProductPriceTextPrefix %>）
										</th>
									<% } %>
									<% if(this.IsProductModify) { %>
										<th class="productDelete">
											削除
										</th>
									<% } %>
								</tr>
							</HeaderTemplate>
							<ItemTemplate>
								
								<%-- 通常商品 --%>
								<tr visible='<%# Item.IsProductSet == false %>' runat="server">
									<td class="productName">
										<%#: Item.ProductName %>
										<br />
										[<span class="productId"><%#: Item.VariationId %></span>]
										<span visible='<%# string.IsNullOrEmpty(Item.ProductOptionTexts) == false %>' runat="server">
											<br />
											<%# WebSanitizer.HtmlEncodeChangeToBr(ProductOptionSettingHelper.GetDisplayProductOptionTexts(Item.ProductOptionTexts).Replace("　", "\r\n")) %>
										</span>
										<small>
											<asp:Repeater DataSource='<%# this.OrderItemSerialKeys[Item.OrderId + Item.OrderItemNo] %>' runat="server">
											<ItemTemplate>
												<br />
												&nbsp;シリアルキー:&nbsp;<%# Eval(Constants.FIELD_SERIALKEY_SERIAL_KEY)%>
											</ItemTemplate>
											</asp:Repeater>
										</small>
									</td>
									<td class="productPrice" Visible="<%# this.OrderModel.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
										<%#: IsOrderItemSubscriptionBoxFixedAmount(Item.SubscriptionBoxFixedAmount) ? string.Empty : CurrencyManager.ToPrice(Item.ProductPrice) %>
									</td>
									<td class="orderCount">
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
									</td>
									<td class="fixed-amount-course-container" Visible="<%# IsFirstItemInFixedAmountCourse(Item.SubscriptionBoxCourseId, Item.SubscriptionBoxFixedAmount) %>" colspan="2" rowspan='<%# this.OrderModel.GetFixedAmountCourseItemCount(Item.SubscriptionBoxCourseId) %>' runat="server">
										<p>頒布会コース名：&nbsp;<%#: GetSubscriptionBoxDisplayName(Item.SubscriptionBoxCourseId) %></p>
										<p>定額：&nbsp;<%#: CurrencyManager.ToPrice(Item.SubscriptionBoxFixedAmount) %>(<%#: this.ProductPriceTextPrefix %>)</p>
										<p>税率：&nbsp;<%#: TaxCalculationUtility.GetTaxRateForDIsplay(Item.ProductTaxRate) %>%</p>
									</td>
									<td class="taxRate" Visible="<%# IsOrderItemSubscriptionBoxFixedAmount(Item.SubscriptionBoxFixedAmount) == false %>" runat="server">
										<%#: TaxCalculationUtility.GetTaxRateForDIsplay(Item.ProductTaxRate) %>%
									</td>
									<td class="orderSubtotal" Visible="<%# IsOrderSubtotalVisible(StringUtility.ToEmpty(Item.OrderSetpromotionNo), Item.SubscriptionBoxFixedAmount) %>" runat="server">
										<asp:Literal runat="server" ID="lProductSubtotal" Text="<%#: CurrencyManager.ToPrice(Item.ItemPrice) %>"/>
									</td>
									<td class="orderSubtotal" Visible="<%# IsOrderSubtotalVisible(StringUtility.ToEmpty(Item.OrderSetpromotionNo), Item.SubscriptionBoxFixedAmount, (string.IsNullOrEmpty(StringUtility.ToEmpty(Item.OrderSetpromotionNo)) ? -1 : int.Parse(Item.OrderSetpromotionItemNo))) %>" rowspan="<%# GetSetPromotionRowSpan(Item, ((Repeater)Container.Parent).DataSource) %>" runat="server">
										<%#: GetSetPromotionDispName(Item) %><br />
										<span visible='<%# (StringUtility.ToEmpty(Item.OrderSetpromotionNo) != "") && HasSetPromotionDiscount(Item, Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_FLG) %>' runat="server">
											<strike><%#: CurrencyManager.ToPrice(GetSetPromotionDiscount(Item, Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_FLG, true)) %></strike><br />
										</span>
										<%#: CurrencyManager.ToPrice(GetSetPromotionDiscount(Item, Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_FLG, false)) %>(税込)
									</td>
									<td class="deleteProduct">
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
									</td>
								</tr>

								<%-- セット商品 --%>
								<tr visible='<%# Item.IsProductSet %>' runat="server">
									<td class="productName">
										<%#: Item.ProductName %>
										<br />
										[<span class="productId"><%#: Item.VariationId %></span>]
									</td>
									<td class="productPrice" Visible="<%# this.OrderModel.IsSubscriptionBoxFixedAmount == false %>" runat="server">
										<%#: CurrencyManager.ToPrice(Item.ProductPrice) %>
										x
										<%#: StringUtility.ToNumeric(Item.ItemQuantitySingle) %>
									</td>
									<td class="orderCount" rowspan='<%# GetProductSetRowspan(Item, ((Repeater)Container.Parent).DataSource) %>' visible='<%# IsProductSetItemTop(Item, ((Repeater)Container.Parent).DataSource) %>' runat="server">
										<%#: StringUtility.ToNumeric(Item.ProductSetCount) %>
									</td>
									<td class="taxRate" Visible="<%# this.OrderModel.IsSubscriptionBoxFixedAmount == false %>" runat="server">
										<%#: TaxCalculationUtility.GetTaxRateForDIsplay(Item.ProductTaxRate) %>
									</td>
									<td class="orderSubtotal" rowspan='<%# GetProductSetRowspan(Item, ((Repeater)Container.Parent).DataSource) %>' Visible='<%# (this.OrderModel.IsSubscriptionBoxFixedAmount == false) && (IsProductSetItemTop(Item, ((Repeater)Container.Parent).DataSource)) %>' runat="server">
										<asp:Literal runat="server" ID="lSetProductSubtotal" Text="<%#: CurrencyManager.ToPrice(CreateSetPriceSubtotal(Item, ((Repeater)Container.Parent).DataSource)) %>"/>
									</td>
									<td class="deleteProduct">

									</td>
								</tr>
							</ItemTemplate>
						</asp:Repeater>
					</table>
				</div>
			</ItemTemplate>
		</asp:Repeater>
		<div class="productModifyBtn" style="padding-bottom: 30px;" runat="server" >
			<small id="sModifyErrorMessage" runat="server" class="error"></small>
			<small id="sNoveltyChangeNoticeMessage" runat="server" class="error"></small>
			<asp:LinkButton id ="btnModifyProducts" CssClass="btn right" OnClick="btnModifyProducts_OnClick" Visible="<%# this.IsOrderModifyBtnDisplay %>" runat="server">お届け商品数変更</asp:LinkButton>
			<div style="display: block" class="right">
				<asp:LinkButton id ="btnModifyConfirm" Text="情報更新" CssClass="btn" OnClientClick="return AlertProductModify()" OnClick="btnModifyConfirm_OnClick" Visible="False" runat="server" />
				<asp:LinkButton id ="btnModifyCancel" Text="キャンセル" CssClass="btn" OnClick="btnModifyCancel_OnClick" Visible="False" runat="server" />
			</div>
		</div>
		<div class="dvOrderSumWrap clearFix">
			<%-- 合計情報 --%>
			<div class="dvOrderSum">
				<dl class="orderSum" id="orderSum">
					<dt>商品合計</dt>
					<dd>
						<asp:HiddenField runat="server" ID="hfOrderPriceSubtotal" Value="<%# CurrencyManager.ToPrice(this.OrderModel.OrderPriceTotal) %>"/>
						<asp:HiddenField runat="server" ID="hfOrderPriceSubtotalNew" Value="<%# CurrencyManager.ToPrice(this.OrderModel.OrderPriceTotal) %>"/>
						<asp:HiddenField runat="server" ID="hfProducts" Value="<%#: SetProduct(0) %>"/>
						<asp:HiddenField runat="server" ID="hfCounts" Value="<%#: SetProduct(1) %>"/>
						<asp:Literal runat="server" ID="lOrderPriceSubtotal" Text="<%#: CurrencyManager.ToPrice(this.OrderModel.OrderPriceSubtotal) %>"/>
					</dd>
					<%if (this.ProductIncludedTaxFlg == false) { %>
						<dt>消費税</dt>
						<dd>
							<asp:Literal runat="server" ID="lOrderPriceSubtptalTax" Text="<%#: CurrencyManager.ToPrice(this.OrderModel.OrderPriceSubtotalTax) %>"/>
						</dd>
					<%} %>
					<asp:Repeater ID="rProductSetPromotions" DataSource="<%# this.OrderSetPromotions %>" runat="server">
					<ItemTemplate>
						<span visible="<%# (string)((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_FLG] == Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_ON %>" runat="server">
						<dt>
						<%#: ((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME] %>
						</dt>
						<dd style="color: #ff0000;">
							<%#: (((decimal)((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_AMOUNT] > 0) ? "-" : "") + CurrencyManager.ToPrice((decimal)((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_AMOUNT]) %>
						</dd>
						</span>
						<asp:HiddenField ID="hfOrderSetPromotionPaymentChargeFreeFlg" Value="<%# (string)((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_FREE_FLG] %>" runat="server" />
					</ItemTemplate>
					</asp:Repeater>

					<%-- 会員ランク情報リスト(有効な場合) --%>
					<%if (Constants.MEMBER_RANK_OPTION_ENABLED) { %>
						<dt class="MemberRankUse">会員ランク割引額</dt>
						<dd class="MemberRankUse">
							<span>
								<asp:Literal runat="server" ID="lOrderMemberRankDiscountPrice" Text='<%#: ((this.OrderModel.MemberRankDiscountPrice > 0) ? "-" : "") + CurrencyManager.ToPrice(this.OrderModel.MemberRankDiscountPrice) %>'/>
							</span>
						</dd>
					<%} %>

					<%-- 定期会員割引額(有効な場合) --%>
					<%if (Constants.MEMBER_RANK_OPTION_ENABLED && Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
						<dt class="couponUse">定期会員割引額</dt>
						<dd class="couponUse">
							<span>
								<asp:Literal runat="server" ID="lFixedPurchaseMemberDiscountAmount" Text='<%#: ((this.OrderModel.FixedPurchaseMemberDiscountAmount > 0) ? "-" : "") + CurrencyManager.ToPrice(this.OrderModel.FixedPurchaseMemberDiscountAmount) %>'/>
							</span>
						</dd>
					<%} %>

					<%-- クーポン情報リスト(有効な場合) --%>
					<%if (Constants.W2MP_COUPON_OPTION_ENABLED) { %>
						<dt class="couponUse">クーポン割引額</dt>
						<dd class="couponUse">
							<span>
								<asp:Literal runat="server" ID="lCouponName" Text="<%#: GetCouponName(this.OrderModel) %>"/>
								<asp:Literal runat="server" ID="lOrderCouponUse" Text='<%#: ((this.OrderModel.OrderCouponUse > 0) ? "-" : "") + CurrencyManager.ToPrice(this.OrderModel.OrderCouponUse) %>'/>
							</span>
						</dd>
					<%} %>
					<%-- ポイント情報リスト(有効な場合) --%>
					<%if (Constants.W2MP_POINT_OPTION_ENABLED) { %>
						<dt class="pointUse">ポイント利用額</dt>
						<dd class="pointUse">
							<span>
								<asp:Literal runat="server" ID="lOrderPointUseYen" Text='<%#: ((this.OrderModel.OrderPointUseYen > 0) ? "-" : "") + CurrencyManager.ToPrice(this.OrderModel.OrderPointUseYen) %>'/>
							</span>
						</dd>
					<%} %>
					<%-- 定期購入割引(有効な場合) --%>
					<%if (this.IsFixedPurchase) { %>
						<dt style="margin-top: 8px; width: 150px; text-align: right; line-height: 1.4em;" runat="server">定期購入割引額</dt>
						<dd style="color: #ff0000;" runat="server">
							<span>
								<asp:Literal runat="server" ID="lFixedPurchaseDiscountPrice" Text='<%#: ((this.OrderModel.FixedPurchaseDiscountPrice > 0) ? "-" : "") + CurrencyManager.ToPrice(this.OrderModel.FixedPurchaseDiscountPrice) %>'/>
							</span>
						</dd>
					<%} %>
					<div visible='<%# (this.OrderModel.OrderPriceRegulation != 0) %>' runat="server">
						<dt style="margin-top: 8px; width: 150px; text-align: right; line-height: 1.4em;">調整金額</dt>
						<dd style="color: #ff0000;" runat="server">
							<span>
								<asp:Literal runat="server" ID="lOrderPriceRegulation" Text='<%#: ((this.OrderModel.OrderPriceRegulation < 0) ? "-" : "") + CurrencyManager.ToPrice(Math.Abs(this.OrderModel.OrderPriceRegulation)) %>'/>
							</span>
						</dd>
					</div>
					<dt>配送料金</dt>
					<dd runat="server" style='<%# (this.OrderModel.ShippingPriceSeparateEstimatesFlg == Constants.FLG_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_VALID) ? "display:none;" : "" %>'>
						<asp:Literal runat="server" ID="lOrderPriceShipping" Text="<%#: CurrencyManager.ToPrice(this.OrderModel.OrderPriceShipping) %>"/>
					</dd>
					<dd runat="server" style='<%# (this.OrderModel.ShippingPriceSeparateEstimatesFlg == Constants.FLG_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_INVALID) ? "display:none;" : "" %>'>
						<%#: this.ShopShippingModel.ShippingPriceSeparateEstimatesMessage %></dd>
					<asp:Repeater ID="rShippingSetPromotions" DataSource="<%# this.OrderSetPromotions %>" runat="server">
					<ItemTemplate>
						<span visible="<%# (string)((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_FREE_FLG] == Constants.FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_ON %>" runat="server">
						<dt>
						<%#: ((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME] %>(送料割引)
						</dt>
						<dd style="color: #ff0000;">
							<%#: (((decimal)((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT] > 0) ? "-" : "") + CurrencyManager.ToPrice((decimal)((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT]) %>
						</dd>
						</span>
					</ItemTemplate>
					</asp:Repeater>
					<dt>決済手数料</dt>
					<dd>
						<asp:Literal runat="server" ID="lOrderPriceExchange" Text="<%#: CurrencyManager.ToPrice(this.OrderModel.OrderPriceExchange) %>"/>
					</dd>
					<asp:Repeater ID="rPaymentSetPromotions" DataSource="<%# this.OrderSetPromotions %>" runat="server">
					<ItemTemplate>
						<span visible="<%# (string)((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_FREE_FLG] == Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_ON %>" runat="server">
						<dt>
						<%#: ((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME] %>(決済手数料割引)
						</dt>
						<dd style="color: #ff0000;">
							<%#: (((decimal)((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT] > 0) ? "-" : "") + CurrencyManager.ToPrice((decimal)((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT]) %>
						</dd>
						</span>
					</ItemTemplate>
					</asp:Repeater>
				</dl>
				<dl class="orderTotal">
					<dt>総合計(税込)</dt>
					<dd>
						<asp:Literal runat="server" ID="lOrderPriceTotal" Text="<%#: CurrencyManager.ToPrice(this.OrderModel.OrderPriceTotal) %>"/>
					</dd>
				</dl>
			</div>
		</div>
		
		<div class="dvUserBtnBox" style="display:flex; justify-content: center;">
			<p><a href="<%: GetBackBtnPath() %>" class="btn btn-large" >購入履歴一覧へ</a></p>
			<p><a href="<%# GetBackBtnPath(this.OrderModel.FixedPurchaseId) %>" style="margin-left: 50px;" class="btn btn-large" Visible="<%# this.IsFixedPurchase %>" runat="server">定期購入履歴詳細へ</a></p>
		</div>
	</div>
</div>
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
		<% if (Constants.RECEIPT_OPTION_ENABLED && (this.OrderModel.ReceiptFlg == Constants.FLG_ORDER_RECEIPT_FLG_ON)) { %>
		if ("<%= string.Join(",", Constants.NOT_OUTPUT_RECEIPT_PAYMENT_KBN) %>".indexOf(document.getElementById("<%= hfPaymentIdSelected.ClientID %>").value) !== -1)
		{
			messagePayment += '\n指定したお支払い方法は、領収書の発行ができません。\n'
				+ '保存されている「領収書情報」が削除されます。\n\n';
		}
		<% } %>

		messagePayment += 'よろしいですか？\n\n';

		<% if (this.CanDisplayChangeFixedPurchasePayment) { %>
		if (document.getElementById("<%: cbIsUpdateFixedPurchaseByOrderPayment.ClientID %>").checked)
		{
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
			var hfCvsShopId = $("#" + parentRepeaterId + "_" + "hfCvsShopId");
			var hfSelectedShopId = $("#" + parentRepeaterId + "_" + "hfSelectedShopId");
			var shopId = hfCvsShopId.val();
			if (shopId == undefined) {
				shopId = hfSelectedShopId.val();
			}
			var dvErrorShippingConvenience = $("#" + parentRepeaterId + "_" + "dvErrorShippingConvenience");
			$.ajax({
				type: "POST",
				url: "<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL %>/CheckStoreIdValid",
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
		var execZip = true;
		if ((ddlShippingCountry.val() != "<%= Constants.COUNTRY_ISO_CODE_US %>")
			&& (ddlShippingCountry.val() != "<%= Constants.COUNTRY_ISO_CODE_TW %>"))
		{
			exec = ValidateAndConfirm(validateName, messageShipping);
			if (ddlShippingCountry.val() == "<%= Constants.COUNTRY_ISO_CODE_JP %>")
			{
				<% foreach (RepeaterItem ri in rOrderShipping.Items) { %>
					var shippingZipErrorId = "<%=  ri.FindControl("cvShippingTel1_1").ClientID %>";
					if((document.getElementById(shippingZipErrorId).innerHTML != undefined)
						&& (document.getElementById(shippingZipErrorId).innerHTML != ""))
					{
						exec = true;
						execZip = false;
					}
				<% } %>
			}
		} else {
			exec = Validate(validateName);
			execUs = ValidateAndConfirm("OrderHistoryDetailGlobal", messageShipping);
		}

		if (exec && execUs && execZip) {
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
			url: "<%= Constants.PATH_ROOT %>Form/OrderHistory/OrderHistoryDetail.aspx/GetPriceString",
			data: JSON.stringify({ price: priceTotalNew }),
			contentType: "application/json; charset=utf-8",
			dataType: "json",
			cache: false,
			async: false,
			success: function(data) {
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
		else
		{
			var spanErrorMessageForAtone = document.getElementsByClassName('spanErrorMessageForAtone');
			if ((spanErrorMessageForAtone.length > 0)
				&& (spanErrorMessageForAtone[0].style.display == "block"))
			{
				return false;
			}
		}

		if ((typeof isAuthoriesAftee === "boolean") && isAuthoriesAftee) {
			document.getElementById('divOrderPaymentUpdateButtons').style.display = "none";
			document.getElementById('divOrderPaymentUpdateExecFroms').style.display = "block";

			return isAuthoriesAftee;
		}
		else
		{
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

		return result;
	}

	<% if(Constants.GLOBAL_OPTION_ENABLE) { %>
	<%-- 台湾郵便番号取得関数 --%>
	function bindTwAddressSearch() {
		<% foreach (RepeaterItem item in rOrderShipping.Items) { %>
			<% if (((DropDownList)item.FindControl("ddlShippingAddr3") != null) && ((TextBox)item.FindControl("tbShippingZipGlobal") != null)) { %>
			$('#<%= ((DropDownList)item.FindControl("ddlShippingAddr3")).ClientID %>').change(function (e) {
				$('#<%= ((TextBox)item.FindControl("tbShippingZipGlobal")).ClientID %>').val(
					$('#<%= ((DropDownList)item.FindControl("ddlShippingAddr3")).ClientID %>').val().split('|')[0]);
			});
			<% } %>
		<% } %>
	}
	<% } %>

	<% if(Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED) { %>
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

	<%-- 注文をキャンセルすることを確認するポップアップを表示します --%>
	function ConfirmOrderCancel() {
		if($('#<%=lbCancelOrder.ClientID%>').attr('disabled') === undefined)
		{
			return confirm('注文をキャンセルします。よろしいですか？');
		}
		else 
		{
			return false;
		}
	}
	
	<%-- 領収書出力ページを別タブで開く --%>
	function openReceiptDownload() {
		var url = '<%= WebSanitizer.UrlAttrHtmlEncode(CreateReceiptDownloadUrl(this.OrderModel.OrderId)) %>';
		window.open(url);
	}
</script>
<script>
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
		for(var i = 0; i <= products.length; i++){
			if(products[i] == undefined) break;
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
