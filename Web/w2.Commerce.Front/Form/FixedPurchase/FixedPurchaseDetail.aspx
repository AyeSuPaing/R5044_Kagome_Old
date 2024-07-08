<%--
=========================================================================================================
  Module      : 定期購入情報詳細画面(FixedPurchaseDetail.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/FixedPurchase/FixedPurchaseDetail.aspx.cs" Inherits="Form_FixedPurchase_FixedPurchaseDetail" Title="定期購入情報詳細ページ" %>
<%-- ▼削除禁止：クレジットカードTokenコントロール▼ --%>
<%@ Register TagPrefix="uc" TagName="CreditToken" Src="~/Form/Common/CreditToken.ascx" %>
<%-- ▲削除禁止：クレジットカードTokenコントロール▲ --%>
<%-- ▽▽Amazonペイメントを使う場合はウィジェットを配置するページは必ずSSLでなければいけない▽▽ --%>
<script runat="server">
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }
</script>
<%-- △△Amazonペイメントを使う場合はウィジェットを配置するページは必ずSSLでなければいけない△△ --%>
<%@ Register TagPrefix="uc" TagName="Layer" Src="~/Form/Common/Layer/SearchResultLayer.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionCvsDef" Src="~/Form/Common/Order/PaymentDescriptionCvsDef.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionPayPal" Src="~/Form/Common/Order/PaymentDescriptionPayPal.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaypalScriptsForm" Src="~/Form/Common/PayPalScriptsForm.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionTriLinkAfterPay" Src="~/Form/Common/Order/PaymentDescriptionTriLinkAfterPay.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaidyCheckoutScript" Src="~/Form/Common/Order/PaidyCheckoutScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionAtone" Src="~/Form/Common/Order/PaymentDescriptionAtone.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaidyCheckoutControl" Src="~/Form/Common/Order/PaidyCheckoutControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionNPAfterPay" Src="~/Form/Common/Order/PaymentDescriptionNPAfterPay.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionLinePay" Src="~/Form/Common/Order/PaymentDescriptionLinePay.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionPayPay" Src="~/Form/Common/Order/PaymentDescriptionPayPay.ascx" %>
<%@ Register TagPrefix="uc" TagName="EcPayScript" Src="~/Form/Common/ECPay/EcPayScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="RakutenCreditCard" Src="~/Form/Common/RakutenCreditCardModal.ascx" %>
<%@ Register TagPrefix="uc" TagName="RakutenPaymentScript" Src="~/Form/Common/RakutenPaymentScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="Loading" Src="~/Form/Common/Loading.ascx" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.Paidy" %>
<%@ Import Namespace="w2.Domain.UserShipping" %>
<%@ Import Namespace="w2.App.Common.SubscriptionBox" %>
<%@ Import Namespace="Newtonsoft.Json" %>

<asp:Content ContentPlaceHolderID="head" runat="server">
	<ContentTemplate>
	<style>
		.modal-content {
			width: 90vw;
			height: 90vh; /* for all but iOS Safari */
			height: 90svh; /* for iOS Safari */
			margin: 0;
			padding: 0;
			background-color: #ffffff;
			position: fixed;
			z-index: 1;
			left: calc(100vw - 95vw);
			top: calc(100vh - 95vh);
			box-shadow: 15px #000;
		}
		.modal-content iframe {
			width: 100%;
			height: 100%;
			border: none;
		}
		.modal-bg {
			width: 100%;
			height: 100%;
			background-color: rgba(0, 0, 0, 0.7);
			position: fixed;
			top: 0;
			left: 0;
			z-index: 0;
		}
	</style>
	<script>
		window.app = {
			closeModal: () => {
				document.getElementById('<%= lCloseSubscriptionBoxProductChangeModal.ClientID %>').click();
			}
		}
	</script>
	</ContentTemplate>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
<uc:EcPayScript runat="server" ID="ucECPayScript" />
<% } %>
<asp:LinkButton style="display: none;" OnClick="lCloseSubscriptionBoxProductChangeModal_OnClick" ID="lCloseSubscriptionBoxProductChangeModal" runat="server" />

<%-- UpdatePanel開始 --%>
<asp:UpdatePanel ID="upUpdatePanel" UpdateMode ="Conditional" runat="server">
<ContentTemplate>

<div id="dvUserFltContents">
	<h2>定期購入情報詳細</h2>
	<div id="dvFixedPurchaseDetail" class="unit">

		<%-- 解約理由 --%>
		<% if (this.FixedPurchaseContainer.IsCancelFixedPurchaseStatus) { %>
		<div class="dvFixedPurchaseDetail">
			<h3>購入解約</h3>
			<table cellspacing="0">
				<% if (this.FixedPurchaseContainer.CancelReasonId != "") { %>
				<tr>
					<th>解約理由</th>
					<td>
						<%# WebSanitizer.HtmlEncode(this.CancelReason.CancelReasonName) %>
					</td>
				</tr>
				<% } %>
				<tr>
					<th>解約メモ</th>
					<td>
						<%# WebSanitizer.HtmlEncodeChangeToBr(this.FixedPurchaseContainer.CancelMemo) %>
					</td>
				</tr>
			</table>
		</div>
		<% } %>

		<%-- 休止理由 --%>
		<% if (this.FixedPurchaseContainer.IsSuspendFixedPurchaseStatus) { %>
			<div class="dvFixedPurchaseDetail">
				<h3>購入一時休止</h3>
				<table>
					<tr>
						<th>再開予定日</th>
						<td>
							<%: (this.FixedPurchaseContainer.ResumeDate != null)
                                    ? DateTimeUtility.ToStringFromRegion(this.FixedPurchaseContainer.ResumeDate, DateTimeUtility.FormatType.ShortDateWeekOfDay1Letter)
                                    : "指定なし" %>
						</td>
					</tr>
					<tr>
						<th>休止理由</th>
						<td>
							<%# WebSanitizer.HtmlEncodeChangeToBr(this.FixedPurchaseContainer.SuspendReason) %>
						</td>
					</tr>
				</table>
			</div>
		<% } %>

		<%-- 注文情報 --%>
		<div class="dvFixedPurchaseDetail">
			<div runat="server" visible="<%# ((string.IsNullOrEmpty(this.OrderNowMessagesHtmlEncoded) == false) || (this.RegisteredOrderIds.Count > 0)) %>" style="color: red; margin:5px 0px 5px 5px" >
				<asp:Label ID="lbOrderNowMessages" runat="server" />
				<%if ((this.RegisteredOrderIds.Count > 0) && (string.IsNullOrEmpty(this.OrderNowMessagesHtmlEncoded) == false)) { %> <br /> <%} %>
				<asp:Repeater ID="rOrderSuccess" runat="server" Visible="<%#  (this.RegisteredOrderIds.Count > 0) %>" ItemType="System.string">
					<HeaderTemplate>定期注文(</HeaderTemplate>
					<ItemTemplate>
						<span runat="server" visible ="<%# (Container.ItemIndex > 0) %>">&nbsp;</span>
						<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateOrderHistoryDetailUrl(Item)) %>"> <%#: Item %> </a>
					</ItemTemplate>
					<FooterTemplate>)が登録されました。</FooterTemplate>
				</asp:Repeater>
			</div>
			<div runat="server" ID="divSkipedMessage" visible="False" style="color: red; margin:5px 0px 5px 5px" >
				審査内容の有効性を判定するためのSMSを送信いたしました。<br/>
				審査が完了後、今すぐ注文で注文された内容が反映されます。
			</div>
			<div class="dvContentsInfo" runat="server" Visible="<%# string.IsNullOrEmpty(this.ResumeFixedPurchaseMessageHtmlEncoded) == false %>">
				<asp:Literal runat="server" ID="lResumeFixedPurchaseMessage" />
			</div>
			<div class="dvPointResetMessages" runat="server" Visible="<%# string.IsNullOrEmpty(this.PointResetMessages) == false %>">
				<asp:Literal runat="server" ID="lPointResetMessages" />
			</div>
			<h3>定期購入情報</h3>
			<table cellspacing="0">
				<tr>
					<th>定期購入ID</th>
					<td>
						<%: this.FixedPurchaseContainer.FixedPurchaseId%><br />
						<span style="color:red" runat="server" Visible="<%# ((this.IsCancelable == false) && (this.FixedPurchaseContainer.IsCompleteStatus == false)) %>">※出荷回数が<%#: this.FixedPurchaseCancelableCount %>回以上から定期購入解約、一時休止が可能になります。</span>
						<%-- 定期キャンセル --%>
						<asp:LinkButton Text="定期購入解約 " runat="server" ID="btnCancelFixedPurchase" OnClick="btnCancelFixedPurchase_Click" class="btn" />
						<%-- 定期キャンセル（解約理由登録） --%>
						<asp:LinkButton Text="定期購入解約（解約理由）" runat="server" ID="btnCancelFixedPurchaseReason" OnClick="btnCancelFixedPurchaseReason_Click" class="btn" />
						<%-- 定期休止 --%>
						<asp:LinkButton Text="定期購入一時休止" runat="server" ID="btnSuspendFixedPurchase" OnClick="btnSuspendFixedPurchase_Click" class="btn" />
					</td>
				</tr>
				<tr>
					<th>定期購入設定</th>
					<td>
						<%: OrderCommon.CreateFixedPurchaseSettingMessage(this.FixedPurchaseContainer)%>
						&nbsp;
						<asp:LinkButton Visible="<%# ((this.DisplayFixedPurchaseShipping) && (this.FixedPurchaseContainer.IsCompleteStatus == false)) %>" Text="パターン変更" runat="server" OnClick="lbDisplayShippingPatternInfoForm_Click" class="btn" />
					</td>
				</tr>
				
				<%-- 配送パターン --%>
				<div class="dvFixedPurchaseDetailShippingPattern" id="dvFixedPurchaseDetailShippingPattern" visible="false" runat="server">
				<tr>
					<th>配送パターン<span class="necessary">*</span></th>
					<td>
						<dl>
							<dt id="dtMonthlyDate" runat="server">
								<asp:RadioButton ID="rbFixedPurchaseDays" Text="月間隔日付指定" GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseDays_OnCheckedChanged" AutoPostBack="true" runat="server" />
							</dt>
							<dd id="ddMonthlyDate" style="padding-left:25px" runat="server">
								<asp:DropDownList ID="ddlFixedPurchaseMonth" runat="server"></asp:DropDownList>
								ヶ月ごと
								<asp:DropDownList ID="ddlFixedPurchaseMonthlyDate" runat="server"></asp:DropDownList>
								日に届ける
								<small><asp:CustomValidator runat="Server" ControlToValidate="ddlFixedPurchaseMonth" ValidationGroup="OrderShipping" ValidateEmptyText="true" SetFocusOnError="true" ClientValidationFunction="ClientValidate" CssClass="error_inline" /></small>
								<small><asp:CustomValidator runat="Server" ControlToValidate="ddlFixedPurchaseMonthlyDate" ValidationGroup="OrderShipping" ValidateEmptyText="true" SetFocusOnError="true" ClientValidationFunction="ClientValidate" CssClass="error_inline" /></small>
								<br />
							</dd>
							<dt id="dtWeekAndDay" runat="server">
								<asp:RadioButton ID="rbFixedPurchaseWeekAndDay" Text="月間隔・週・曜日指定" GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseWeekAndDay_OnCheckedChanged" AutoPostBack="true" runat="server" />
							</dt>
							<dd id="ddWeekAndDay" style="padding-left:25px" runat="server">
								<asp:DropDownList ID="ddlFixedPurchaseIntervalMonths" runat="server" />
								ヶ月ごと
								<asp:DropDownList ID="ddlFixedPurchaseWeekOfMonth" runat="server"></asp:DropDownList>
								<asp:DropDownList ID="ddlFixedPurchaseDayOfWeek" runat="server"></asp:DropDownList>
								に届ける
								<small><asp:CustomValidator runat="Server" ControlToValidate="ddlFixedPurchaseIntervalMonths" ValidationGroup="OrderShipping" ValidateEmptyText="true" SetFocusOnError="true" ClientValidationFunction="ClientValidate" CssClass="error_inline" /></small>
								<small><asp:CustomValidator runat="Server" ControlToValidate="ddlFixedPurchaseWeekOfMonth" ValidationGroup="OrderShipping" ValidateEmptyText="true" SetFocusOnError="true" ClientValidationFunction="ClientValidate" CssClass="error_inline" /></small>
								<small><asp:CustomValidator runat="Server" ControlToValidate="ddlFixedPurchaseDayOfWeek" ValidationGroup="OrderShipping" ValidateEmptyText="true" SetFocusOnError="true" ClientValidationFunction="ClientValidate" CssClass="error_inline" /></small>
								<br />
							</dd>
							<dt id="dtIntervalDays" runat="server">
								<asp:RadioButton ID="rbFixedPurchaseIntervalDays" Text="配送日間隔指定" GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseIntervalDays_OnCheckedChanged" AutoPostBack="true" runat="server" />
							</dt>
							<dd id="ddIntervalDays" style="padding-left:25px" runat="server">
								<asp:DropDownList ID="ddlFixedPurchaseIntervalDays" runat="server"></asp:DropDownList>
								日ごとに届ける
								<small><asp:CustomValidator runat="Server" ControlToValidate="ddlFixedPurchaseIntervalDays" ValidationGroup="OrderShipping" ValidateEmptyText="true" SetFocusOnError="true" ClientValidationFunction="ClientValidate" CssClass="error_inline" /></small>
								<br />
							</dd>
							<dt id="dtFixedPurchaseEveryNWeek" runat="server">
								<asp:RadioButton ID="rbFixedPurchaseEveryNWeek" Text="週間隔・曜日指定" GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseFixedPurchaseEveryNWeek_OnCheckedChanged" AutoPostBack="true" runat="server" />
							</dt>
							<dd id="ddFixedPurchaseEveryNWeek" style="padding-left:25px" runat="server">
								<asp:DropDownList ID="ddlFixedPurchaseEveryNWeek_Week" runat="server"></asp:DropDownList>
								週間ごと
								<asp:DropDownList ID="ddlFixedPurchaseEveryNWeek_DayOfWeek" runat="server"></asp:DropDownList>
								に届ける
								<small><asp:CustomValidator runat="Server" ControlToValidate="ddlFixedPurchaseEveryNWeek_Week" ValidationGroup="OrderShipping" ValidateEmptyText="true" SetFocusOnError="true" ClientValidationFunction="ClientValidate" CssClass="error_inline" /></small>
								<small><asp:CustomValidator runat="Server" ControlToValidate="ddlFixedPurchaseEveryNWeek_DayOfWeek" ValidationGroup="OrderShipping" ValidateEmptyText="true" SetFocusOnError="true" ClientValidationFunction="ClientValidate" CssClass="error_inline" /></small>
								<br />
							</dd>
						</dl>
					</td>
				</tr>
				<tr>
					<th></th>
					<td>
						<asp:Button Text="配送パターン更新" runat="server" ValidationGroup="OrderShipping" OnClientClick="return confirm('配送パターンを変更します。\n本当によろしいですか？')" OnClick="btnUpdateShippingPatternInfo_Click" class="btn" />
						<asp:Button Text="キャンセル" runat="server" OnClick="btnCloseShippingPatternInfo_Click" class="btn" />
						<small ID="sErrorMessage" class="error" runat="server"></small><br />
					</td>
				</tr>
				</div>
				<tr>
					<th>最終購入日</th>
					<td>
						<%: DateTimeUtility.ToStringFromRegion(this.FixedPurchaseContainer.LastOrderDate, DateTimeUtility.FormatType.ShortDateWeekOfDay1Letter)%>
					</td>
				</tr>
				<tr>
					<th>購入回数</th>
					<td>
						<%: this.FixedPurchaseContainer.OrderCount%>
					</td>
				</tr>

				<tr>
					<th>定期購入ステータス</th>
					<td>
						<span id="spFixedPurchaseStatus" runat="server">
							<%: this.FixedPurchaseContainer.FixedPurchaseStatusText %></span>
						<asp:CheckBox runat="server" ID ="cbResumeFixedPurchase" Visible="<%# this.CanResumeFixedPurchase %>" autoPostBack="true" Text="定期購入を再開" />
						
						<span runat="server" Visible='<%# this.IsInvalidResumePaypay %>'>
							<br />
							<asp:Literal ID="lCancelPaypayNotification" runat="server"/>
						</span>
						<asp:CheckBox runat="server" ID ="cbResumeSubscriptionBox" Visible="<%# CanResumeCourse(this.FixedPurchaseContainer) %>" autoPostBack="true" Text="頒布会コースを再開" />
					</td>
					<% lbResumeFixedPurchase.OnClientClick = "return confirm('"
						+ "定期購入を再開します。よろしいですか？\\n\\n"
						+ "■注意点\\n"
						+ "' + getResumeShippingMesasge() + '"
						+ "');"; %>
					<% lbResumeSubscriptionBox.OnClientClick = "return confirm('"
						+ "頒布会コースを再開します。よろしいですか？\\n\\n"
						+ "■注意点\\n"
						+ "' + getResumeSubscriptionBoxShippingMesasge() + '"
						+ "');"; %>
				</tr>
				<% if(cbResumeFixedPurchase.Checked == true) {%>
				<tr>
					<th>配送再開日</th>
					<td>
						次回配送日：<asp:DropDownList ID="ddlResumeFixedPurchaseDate" runat="server" ></asp:DropDownList>
						<asp:LinkButton runat="server" ID="lbResumeFixedPurchase" OnClick="lbResumeFixedPurchase_Click" CssClass="btn" Text="  定期購入を再開する  " ></asp:LinkButton>
						<asp:LinkButton runat="server" ID="lbResumeFixedPurchaseCancel" OnClick="lbResumeFixedPurchaseCancel_Click" CssClass="btn" Text="  キャンセル  "></asp:LinkButton>
						<br/>
						<div id="dvResumeFixedPurchaseErr" class="error" visible = "false" runat="server"></div>
					</td>
				</tr>
				<% } %>
				<% if (cbResumeSubscriptionBox.Checked == true){%>
				<tr>
					<th>配送再開日</th>
					<td>
						次回配送日：<asp:DropDownList ID="ddlResumeSubscriptionBoxDate" runat="server" ></asp:DropDownList>
						<asp:LinkButton runat="server" ID="lbResumeSubscriptionBox" OnClick="lbResumeSubscriptionBox_Click" CssClass="btn" Text="  頒布会コースを再開する  "></asp:LinkButton>
						<asp:LinkButton runat="server" ID="lbResumeSubscriptionBoxCancel" OnClick="lbResumeFixedPurchaseCancel_Click" CssClass="btn" Text="  キャンセル  "></asp:LinkButton>
						<br/>
						<div id="dvResumeSubscriptionBoxErr" class="error" visible = "false" runat="server"></div>
					</td>
				</tr>
				<%} %>
				<tr>
					<th>決済ステータス</th>
					<td>
						<span id="spPaymentStatus" runat="server">
							<%: this.FixedPurchaseContainer.PaymentStatusText%></span>
						&nbsp;
					</td>
				</tr>

				<tr>
					<th>お支払い方法</th>
					<td>
						<%: this.Payment.PaymentName %>
						<%if (string.IsNullOrEmpty(this.FixedPurchaseContainer.CardInstallmentsCode) == false) { %>
						（<%: ValueText.GetValueText(Constants.TABLE_ORDER, OrderCommon.CreditInstallmentsValueTextFieldName, this.FixedPurchaseContainer.CardInstallmentsCode)%>払い）
						<%} %>
						<% if (this.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT) { %>
							<div style="margin: 10px 0;">
								<small>※現在のAmazon Payでの配送先情報、お支払い方法を表示しています。</small>
							</div>
							<iframe id="AmazonDetailWidget" src="<%: PageUrlCreatorUtility.CreateAmazonPayWidgetUrl(true, fixedPurchaseId: this.FixedPurchaseContainer.FixedPurchaseId) %>" style="width:100%;border:none;"></iframe>
						<% } %>
						<div style="float: right">
							<asp:LinkButton ID="lbDisplayInputOrderPaymentKbn"
								Text="お支払い方法変更" runat="server"
								OnClick="lbDisplayInputOrderPaymentKbn_Click"
								Visible="<%# ((this.CanCancelFixedPurchase) && (this.IsDisplayInputOrderPaymentKbn) && (this.FixedPurchaseContainer.IsCompleteStatus == false) && (this.FixedPurchaseContainer.IsUnavailableShippingAreaStatus == false)) %>" class="btn" />
						</div>
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
						<!--Update payment pattern-->
						<asp:HiddenField ID="hfPaidyTokenId" runat="server" />
						<asp:HiddenField ID="hfPaidyPaySelected" runat="server" />
						<div id="dvOrderPaymentPattern" visible="false" runat="server">
							<tr><!--Input payment update form-->
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
												<% if(Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_RB) { %>
												<dt><w2c:RadioButtonGroup ID="rbgPayment" GroupName='Payment' Checked="<%# this.FixedPurchaseContainer.OrderPaymentKbn == (string)Item.PaymentId %>" Text="<%#: Item.PaymentName %>" OnCheckedChanged="rbgPayment_OnCheckedChanged" AutoPostBack="true" CssClass="radioBtn" runat="server" /></dt>
												<% } %>

												<%-- クレジット --%>
												<dd id="ddCredit" visible="<%# ((string)Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) %>" runat="server">
													<asp:DropDownList ID="ddlUserCreditCard" visible="<%# this.IsCreditListDropDownDisplay %>" runat="server" AutoPostBack="true" DataTextField="text" DataValueField="value" OnSelectedIndexChanged="ddlUserCreditCard_OnSelectedIndexChanged"></asp:DropDownList>
													<%-- ▽新規カード▽ --%>
													<% if (IsNewCreditCard()){ %>
													<div id="divCreditCardInputForm">
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
													</small>
													</p>
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

													<div id="Div3" visible="<%# OrderCommon.CreditInstallmentsSelectable && (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Rakuten) %>" runat="server">
														<strong>支払い回数</strong>
														<p>
															<asp:DropDownList id="dllCreditInstallments" runat="server" DataTextField="Text" DataValueField="Value" CssClass="input_border" autocomplete="off"></asp:DropDownList><br/>
															<span class="fgray">※AMEX/DINERSは一括のみとなります。</span>
														</p>
													</div>
													<% } else { %>
														<div>遷移する外部サイトでカード番号を入力してください。</div>
													<% } %>
													</div>
													<asp:CheckBox ID="cbRegistCreditCard" visible="<%# this.IsCreditRegistCheckBoxDisplay %>" runat="server" Checked="false" OnCheckedChanged="cbRegistCreditCard_OnCheckedChanged" Text="登録する" autocomplete="off" AutoPostBack="true" />
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
												<dd id="ddCvsDef" visible="<%# ((string)Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF) %>" runat="server">
													<uc:PaymentDescriptionCvsDef runat="server" id="ucPaymentDescriptionCvsDef" />
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
													<iframe id="AmazonInputWidget" src="<%: PageUrlCreatorUtility.CreateAmazonPayWidgetUrl(false, fixedPurchaseId: this.FixedPurchaseContainer.FixedPurchaseId) %>" style="width:100%;border:none;"></iframe>
													<div id="constraintErrorMessage" style="color:red;padding:5px" ClientIDMode="Static" runat="server"></div>
													<asp:HiddenField ID="hfAmazonBillingAgreementId" ClientIDMode="Static" runat="server" />
													<table cellspacing="0">
														<tr>
															<th>配送方法</th>
															<td>
																<asp:DropDownList ID="ddlShippingMethodForAmazonPay" OnSelectedIndexChanged="ddlShippingMethod_OnSelectedIndexChanged" runat="server" AutoPostBack="true"></asp:DropDownList>
															</td>
														</tr>
														<tr>
															<th>配送サービス</th>
															<td>
																<asp:DropDownList ID="ddlDeliveryCompanyForAmazonPay" OnSelectedIndexChanged="ddlDeliveryCompanyList_OnSelectedIndexChanged" AutoPostBack="true" runat="server"/>
															</td>
														</tr>
														<% if (this.DeliveryCompany.IsValidShippingTimeSetFlg && (this.WddlShippingMethodForAmazonPay.SelectedValue == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)) { %>
														<tr>
															<th>配送希望時間帯</th>
															<td>
																<asp:DropDownList ID="ddlShippingTimeForAmazonPay" runat="server"></asp:DropDownList>
															</td>
														</tr>
														<% } %>
													</table>
												</dd>

												<%-- Amazon Pay(CV2) --%>
												<dd id="ddAmazonPayCv2" visible="<%# Item.IsPaymentIdAmazonPayCv2 %>" runat="server">
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

												<%-- atone翌月払い --%>
												<dd id="ddPaymentAtone" class="Atone_0" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE) %>" runat="server">
													<span id="spanErrorMessageForAtone" class="spanErrorMessageForAtone" style="color: red; display: none" runat="server"></span>
													<uc:PaymentDescriptionAtone runat="server" id="PaymentDescriptionAtone" />
												</dd>

												<%-- aftee決済設定 --%>
												<dd id="ddPaymentAftee" class="Aftee_0" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE) %>" runat="server">
													<span id="spanErrorMessageForAftee" class="spanErrorMessageForAftee" style="color: red; display: none" runat="server"></span>
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

												<%-- （DSK）後払い --%>
												<dd id="ddDskDef" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_DSK_DEF) %>" runat="server">
													コンビニ後払い（DSK）
												</dd>
											</ItemTemplate>
											</asp:Repeater>
											<%--▼▼Amazon支払契約ID格納▼▼--%>
											<asp:HiddenField runat="server" value="<%# this.FixedPurchaseContainer.ExternalPaymentAgreementId %>" ID="hfAmazonBillingAgreementId" />
											</dl>
										</div><!--list-->
									</div>
								</td>
							</tr><!--End input payment update form-->

							<tr><!--Action button payment update form-->
								<th></th>
								<td>
									<div id="divOrderPaymentUpdateButtons" style="display: block"> 
									<asp:HiddenField ID="hfPaymentNameSelected" runat="server" />
									<asp:HiddenField ID="hfPaymentIdSelected" runat="server" />
									<asp:HiddenField ID="hfPaymentPriceSelected" runat="server" />
									<asp:LinkButton ID="lbUpdatePayment" Text="情報更新" runat="server" ValidationGroup="OrderPayment"
										OnClientClick="doPostbackEvenIfCardAuthFailed=false;return AlertPaymentChange();"
										OnClick="btnUpdatePaymentPatternInfo_Click" class="btn" ></asp:LinkButton>
									<asp:LinkButton ID="btnClosePaymentPatternInfo" Text="キャンセル" runat="server" OnClick="btnClosePaymentPatternInfo_Click" class="btn"></asp:LinkButton>
									</div>
									<div id="divOrderPaymentUpdateExecFroms" style="display: none"> 
										更新中です...
									</div>
									<small id="sErrorMessagePayment" class="error" runat="server"></small>
								</td>
							</tr>
						</div>
						<!--End Update payment pattern-->
					</td>
				</tr>
				<tr>
					<th>注文メモ</th>
					<td>
						<%#: (string.IsNullOrEmpty(this.FixedPurchaseContainer.Memo) ? "指定なし" : this.FixedPurchaseContainer.Memo) %>
					</td>
				</tr>
				<%if (IsDisplayOrderExtend()) {%>
				<tr>
					<th>注文確認事項</th>
					<td>
						<div style="text-align:right; float:right; ">
							<asp:LinkButton ID="lbOrderExtend" Text="注文確認事項の変更"　Visible="<%# this.CanCancelFixedPurchase && this.BeforeCancelDeadline && IsDisplayOrderExtendModifyButton() && (this.FixedPurchaseContainer.IsUnavailableShippingAreaStatus == false)%>" OnClick="lbOrderExtend_OnClick"  runat="server" class="btn" />
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
						<div id="divOrderExtendUpdateExecFroms" style="display: none"> 更新中です...</div>
					</td>
				</tr>
				<% } %>
				<% } %>
			<% if ((this.FixedPurchaseContainer.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) && (this.UserCreditCardInfo != null)){ %>
				<tr id="dvFixedPurchaseCurrentCard" runat="server">
					<th>利用クレジットカード情報</th>
					<td>
						<% if (this.UserCreditCardInfo.DispFlg == Constants.FLG_USERCREDITCARD_DISP_FLG_ON) { %>
						カード登録名: <%:this.UserCreditCardInfo.CardDispName %><%: this.UserCreditCardInfo.DispFlag ? "" : " (削除済)" %><br />
						<% } %>
						<%if (OrderCommon.CreditCompanySelectable && (this.UserCreditCardInfo.CompanyName != string.Empty)) {%>
						カード会社: <%: this.UserCreditCardInfo.CompanyName %><br />
						<%} %>
						カード番号: XXXXXXXXXXXX<%: this.UserCreditCardInfo.LastFourDigit %><br />
						有効期限: <%: this.UserCreditCardInfo.ExpirationMonth + "/" + this.UserCreditCardInfo.ExpirationYear + " (月/年)" %><br />
						カード名義人: <%: this.UserCreditCardInfo.AuthorName %>
						<span style="font-weight: bold; display: block; margin-top: 2px;"><%= this.CompleteMessage %></span>
					</td>
				</tr>
				<%} %>

				<% if (CanUsePointForPurchase()) { %>
				<tr>
					<th>次回購入の利用ポイント</th>
					<td>
						<% if (this.CanUseAllPointFlg && (this.FixedPurchaseContainer.UseAllPointFlg == Constants.FLG_FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG_ON)) { %>
							<span>全ポイント継続利用</span>
						<% } else { %>
							<%: GetNumeric(this.FixedPurchaseContainer.NextShippingUsePoint) %><%: Constants.CONST_UNIT_POINT_PT %>&nbsp;
						<% } %>
						<asp:LinkButton ID="lbDisplayUpdateNextShippingUsePointForm" Visible="<%# this.CanCancelFixedPurchase && (this.FixedPurchaseContainer.IsUnavailableShippingAreaStatus == false) %>" Enabled="<%# this.BeforeCancelDeadline %>" Text="  利用ポイント変更  " runat="server" OnClick="lbDisplayUpdateNextShippingUsePointForm_Click" class="btn" />&nbsp;
						（利用可能ポイント：<%: GetNumeric(this.LoginUserPointUsableForFixedPurchase) %><%: Constants.CONST_UNIT_POINT_PT %>）<br />
						<div id="dvNextShippingUsePoint" visible="false" runat="server">
							<asp:TextBox ID="tbNextShippingUsePoint" style="margin-top: 10px;" runat="server"/><br />
							<% if (this.CanUseAllPointFlg) { %>
							<asp:CheckBox ID="cbUseAllPointFlg" Text="全ポイント継続利用 ※以降の注文にも適用されます"
								OnCheckedChanged="cbUseAllPointFlg_Changed" OnDataBinding="cbUseAllPointFlg_DataBinding"
								CssClass="checkBox" style="font-size: 11px;" AutoPostBack="True" runat="server" />
							<% } %>
							<asp:CustomValidator runat="server" ControlToValidate="tbNextShippingUsePoint" ValidationGroup="FixedPurchaseModifyInput" ValidateEmptyText="true" SetFocusOnError="true" ClientValidationFunction="ClientValidate" CssClass="error_inline" />
							<span style='color: red;'><%= this.NextShippingUsePointErrorMessage %><br /></span>
							<asp:LinkButton ID="lbUpdateNextShippingUsePoint" Text="  利用ポイント更新  " OnClick="lbUpdateNextShippingUsePoint_Click" OnClientClick="return confirm('ご利用ポイントを変更します。よろしいですか？');" runat="server" class="btn" />&nbsp;
							<asp:LinkButton ID="lbCloseUpdateNextShippingUsePointForm" Text="  キャンセル  " OnClientClick="return exec_submit();" OnClick="lbCloseUpdateNextShippingUsePointForm_Click" runat="server" class="btn" />&nbsp;
						</div>
						<small>
<pre>
※入力した利用ポイントは次回購入時に適用されます。次回配送日の <%# this.ShopShipping.FixedPurchaseCancelDeadline %>日前 まで変更可能です。
　（一度入力済みの利用ポイントを減らす際、入力済みポイントが有効期限切れの場合には、
　ポイントが消滅しますのでご注意ください。）
※次回注文生成時、ポイント数が注文時の利用可能ポイント数より大きい場合、
　適用されなかった分のポイントはお客様のポイントに戻されます。
　（ポイント有効期限切れの場合は、ポイントは戻らず消滅します。）
※定期購入に期間限定ポイントを利用することはできません。
</pre>
						</small>
					</td>
				</tr>
				<% } %>
				<% if (Constants.W2MP_COUPON_OPTION_ENABLED) { %>
				<tr>
					<th>次回購入の利用クーポン</th>
					<td>
						<%: (this.FixedPurchaseContainer.NextShippingUseCouponDetail == null) ? "利用なし" : this.FixedPurchaseContainer.NextShippingUseCouponDetail.DisplayName %>
						<asp:LinkButton ID="lbDisplayUpdateNextShippingUseCouponForm" Visible="<%# this.CanAddNextShippingUseCoupon && (this.FixedPurchaseContainer.IsUnavailableShippingAreaStatus == false) %>" Enabled="<%# this.BeforeCancelDeadline %>" Text="  利用クーポン変更  " runat="server" OnClick="lbDisplayUpdateNextShippingUseCouponForm_Click" class="btn" />
						<asp:LinkButton ID="lbCancelNextShippingUseCoupon" Visible="<%# this.CanCancelNextShippingUseCoupon %>" Enabled="<%# this.BeforeCancelDeadline %>" Text="  キャンセル  " runat="server" OnClick="lbCancelNextShippingUseCoupon_Click" class="btn" />
						<div id="dvNextShippingUseCoupon" visible="false" runat="server">
							<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1" /></p>
							<div id="divCouponInputMethod" runat="server" Visible="<%# this.UsableCoupons.Length > 0 %>">
							<asp:RadioButtonList ID="rblCouponInputMethod" runat="server" AutoPostBack="true"
								OnSelectedIndexChanged="rblCouponInputMethod_SelectedIndexChanged" OnDataBinding="rblCouponInputMethod_DataBinding"
								DataSource="<%# GetCouponInputMethod() %>" DataTextField="Text" DataValueField="Value" RepeatColumns="2" RepeatDirection="Horizontal" RepeatLayout="Flow"/>
								<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1" /></p>
							</div>
							<span id="dvCouponSelectArea" runat="server"  Visible="<%# this.UsableCoupons.Length > 0 %>">
								<asp:DropDownList CssClass="input_border" style="width: 240px" ID="ddlNextShippingUseCouponList" runat="server" DataSource="<%# GetUsableCouponListItems() %>" DataTextField="Text" DataValueField="Value" AutoPostBack="true"></asp:DropDownList>
							</span>
							<span id="dvCouponCodeInputArea" runat="server">
								クーポンコード&nbsp;<asp:TextBox ID="tbNextShippingUseCouponCode" runat="server" MaxLength="30" autocomplete="off" />
							</span>
							<span><asp:LinkButton runat="server" ID="lbShowCouponBox" Text="クーポンBOX" OnClick="lbShowCouponBox_Click" Visible="<%# this.UsableCoupons.Length > 0 %>"
								style="color: #ffffff !important; background-color: #000 !important;
										border-color: rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.25); text-shadow: 0 -1px 0 rgba(0, 0, 0, 0.25); display: inline-block;
										padding: 4px 10px 4px; margin-bottom: 0; font-size: 13px; line-height: 18px; text-align: center; vertical-align: middle; cursor: pointer;
										border: 1px solid #cccccc; border-radius: 4px; box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.2), 0 1px 2px rgba(0, 0, 0, 0.05); white-space: nowrap;"/></span>

							<div style='color: red; margin-top:5px; margin-bottom:5px;'><%: this.NextShippingUseCouponErrorMessage %><br /></div>
							<span><asp:LinkButton ID="lbUseCoupon" Text="クーポンを適用" OnClick="btnUpdateUseCoupon_Click"  OnClientClick="return confirm('ご利用クーポンを変更します。よろしいですか？');" CssClass="btn" runat="server"/></span>
							<span><asp:LinkButton ID="lbCancel" Text="キャンセル" OnClientClick="return exec_submit();" OnClick="lbCloseUpdateNextShippingUseCouponForm_Click" CssClass="btn" runat="server"/></span>
							<!--▽クーポンBOX▽-->
							<div runat="server" id="dvCouponBox" style="z-index: 1; top: 0; left: 0; width: 100%; height: 120%; position: fixed; background-color: rgba(128, 128, 128, 0.75);" visible="false">
							<div id="dvCouponList" style="width: 800px; height: 500px; top: 50%; left: 50%; text-align: center; border: 2px solid #aaa; background: #fff; position: fixed; z-index: 2; margin:-250px 0 0 -400px;">
							<h2 style="height: 20px; color: #fff; background-color: #000; font-size: 16px; padding: 3px 0px; border-bottom: solid 1px #ccc; ">クーポンBOX</h2>
							<div style="height: 400px; overflow: auto;">
							<asp:Repeater ID="rCouponList" ItemType="w2.Domain.Coupon.Helper.UserCouponDetailInfo" runat="server" DataSource="<%# this.UsableCoupons %>">
								<HeaderTemplate>
								<table>
									<tr>
										<th style="border-bottom-style:solid; border-bottom-width:1px; background-color:#ececec; padding:10px; text-align:center;width:150px;">クーポンコード</th>
										<th style="border-bottom-style:solid; border-bottom-width:1px; background-color:#ececec; padding:10px; text-align:center;width:230px;">クーポン名</th>
										<th style="border-bottom-style:solid; border-bottom-width:1px; background-color:#ececec; padding:10px; text-align:center;width:100px;">割引金額<br />/割引率</th>
										<th style="border-bottom-style:solid; border-bottom-width:1px; background-color:#ececec; padding:10px; text-align:center;width:70px;">利用可能回数</th>
										<th style="border-bottom-style:solid; border-bottom-width:1px; background-color:#ececec; padding:10px; text-align:center;width:150px;">有効期限</th>
										<th style="border-bottom-style:solid; border-bottom-width:1px; background-color:#ececec; padding:10px; text-align:center;width:100px;"></th>
									</tr>
								</HeaderTemplate>
								<ItemTemplate>
									<tr>
										<td style="border-bottom-style:solid; border-bottom-width:1px; padding:10px 8px; text-align:left; text-align:center;width:150px; background-color: white;">
											<%#: Item.CouponCode %><br />
											<asp:HiddenField runat="server" ID="hfCouponBoxCouponCode" Value="<%# Item.CouponCode %>" />
										</td>
										<td style="border-bottom-style:solid; border-bottom-width:1px; padding:10px 8px; text-align:left; text-align:center;width:230px; background-color: white;"
											title="<%#: Item.CouponDispDiscription %>">
											<%#: Item.CouponDispName %>
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
										<td style="border-bottom-style:solid; border-bottom-width:1px; padding:10px 8px; text-align:left; text-align:center;width:150px; background-color: white;">
											<%#:DateTimeUtility.ToStringFromRegion(Item.ExpireEnd, DateTimeUtility.FormatType.LongDateHourMinute1Letter) %>
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
							<!--△クーポンBOX△-->
						</div>
						<small>
							<br />
							※入力した利用クーポンは次回購入時に適用されます。次回お届け予定日の <%#: this.ShopShipping.FixedPurchaseCancelDeadline %>日前 まで変更可能です。<br />
							※次回注文生成時、クーポンが本注文に適用不可となる場合、クーポンが戻されます。
						</small>
					</td>
				</tr>
				<% } %>

				<% if ((this.FixedPurchaseShippingContainer.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF)
					|| (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false)) { %>
				<tr>
					<th>次回配送日</th>
					<td>
						<%#: (this.FixedPurchaseContainer.IsCancelFixedPurchaseStatus == false) ? DateTimeUtility.ToStringFromRegion(this.FixedPurchaseContainer.NextShippingDate, DateTimeUtility.FormatType.ShortDateWeekOfDay1Letter) : "-" %>
						<br />
						<div visible="<%# this.CanCancelFixedPurchase && (this.FixedPurchaseContainer.IsUnavailableShippingAreaStatus == false) %>" runat="server">
						<% if (this.DisplaySkipNextShipping) { %>
						<asp:Button Text="次回配送スキップ" runat="server" Enabled="<%# this.BeforeCancelDeadline && this.IsCancelable && this.HasClickSkipNextShipping %>" OnClientClick="return confirm('次回配送をスキップします。よろしいですか？')" OnClick="btnSkipNextShipping_Click" class="btn" />
						<% if (this.HasClickSkipNextShipping) { %>
						<span runat="server" Visible="<%# this.IsCancelable %>">次回配送日の <%# this.ShopShipping.FixedPurchaseCancelDeadline %>日前 までスキップ可能です。</span>
						<span runat="server" Visible="<%# this.IsCancelable == false %>">出荷回数が<%# this.FixedPurchaseCancelableCount %>回以上からスキップ可能です。</span>
						<% } else { %>
						<span>定期購入スキップ制限回数を超えました。</span>
						<% } %>
						<% } %>
						<br />
						<asp:Button ID="btnChangeNextShippingDate" style="margin-top:5px" runat="server" OnClick="btnChangeNextShippingDate_Click" Text="次回配送日変更"
							Enabled="<%# this.BeforeCancelDeadline %>" class="btn" />
						<span runat="server">次回配送日の <%# this.ShopShipping.FixedPurchaseCancelDeadline %>日前 まで変更可能です。</span>
						</div>
						<div id="dvChangeNextShippingDate" visible="false" runat="server" style="margin-top:15px">
							<asp:DropDownList ID="ddlNextShippingDate" runat="server" DataSource="<%# GetChangeNextShippingDateList() %>"
								DataTextField="text" DataValueField="value" SelectedValue ='<%# this.FixedPurchaseContainer.NextShippingDate.Value.ToString("yyyy/MM/dd") %>'
								OnSelectedIndexChanged="ddlNextShippingDate_SelectedIndexChanged" AutoPostBack="true" />
							<br />
							<asp:CheckBox id="cbUpdateNextNextShippingDate" runat="server" Checked="<%# Constants.FIXEDPURCHASEORDERNOW_NEXT_NEXT_SHIPPING_DATE_UPDATE_DEFAULT %>" Text="次々回配送日を自動計算する"></asp:CheckBox><br />
							<p><asp:Label runat="server" ID="lNextShippingDateErrorMessage" CssClass="fred" /></p>
							<div style="margin-top:15px">
							<asp:Button ID="btnUpdateNextShippingDate" Text="次回配送日更新" runat="server" OnClientClick="return confirmUpdateNextNextShippingDate(this);" OnClick="btnUpdateNextShippingDate_Click" class="btn" />
							<asp:Button ID="btnCancelNextShippingDate" Text="キャンセル" runat="server" OnClick="btnCancelNextShippingDate_Click" class="btn" />
							</div>
						</div>
					</td>
				</tr>
				<% } %>

				<tr>
					<th>次々回配送日</th>
					<td>
						<%#: (this.FixedPurchaseContainer.IsCancelFixedPurchaseStatus == false) ? DateTimeUtility.ToStringFromRegion(this.FixedPurchaseContainer.NextNextShippingDate.Value, DateTimeUtility.FormatType.ShortDateWeekOfDay1Letter) : "-" %>
						<span runat="server" Visible="<%# (this.WcbUpdateNextNextShippingDate.Checked) && (this.FixedPurchaseContainer.IsCancelFixedPurchaseStatus == false) %>">※次回配送日の変更により左記のように変更されます。</span>
					</td>
				</tr>
				<tr runat="server" visible="<%# this.FixedPurchaseContainer.IsOrderRegister && (this.FixedPurchaseContainer.IsUnavailableShippingAreaStatus == false) %>">
					<th>今すぐ注文</th>
					<td>
						<asp:Button ID="btnFixedPurchaseOrder" Text="  今すぐ注文  " runat="server" OnClientClick = "return confirm('今すぐ注文を登録します。よろしいですか？')" OnClick="btnFixedPurchaseOrder_Click" class="btn" />
						<asp:CheckBox ID="cbUpdateNextShippingDate" Text="次回配送日を更新する" runat="server" Checked="<%# Constants.FIXEDPURCHASEORDERNOW_NEXT_SHIPPING_DATE_UPDATE_DEFAULT %>" />
						<br />
						<small>
							※設定された次回購入の利用ポイント数が本注文に適用されます。<br />
							　適用後、次回購入の利用ポイント数が「0」にリセットされます。<br />
							<% if (Constants.W2MP_COUPON_OPTION_ENABLED) { %>
							※設定された次回購入の利用クーポンが本注文に適用されます。<br />
							　適用後、次回購入の利用クーポンが「指定なし」にリセットされます。<br />
							<% } %>
							※今すぐ注文の配送日は「<%# ReplaceTag("@@DispText.shipping_date_list.none@@") %>」で登録されます。 <br />
							※「次回配送日を更新する」にチェックを入れた場合には、 <br />
							　定期購入情報の次回・次々回配送日が変更されます。 <br />
							※「次回配送日を更新する」にチェックをしない場合には、 <br />
							　定期購入情報の次回・次々回配送日は変更されません。 <br />
						</small>
					</td>
				</tr>
			</table>
		</div>
		<%--▽領収書情報▽--%>
		<% if (Constants.RECEIPT_OPTION_ENABLED) { %>
		<div>
			<h3>領収書情報</h3>
			<% if (this.IsReceiptInfoModify == false) { %>
			<table cellspacing="0">
				<tr>
					<th>領収書希望</th>
					<td>
						<div style="float:right"><asp:LinkButton Text="領収書情報変更" runat="server" Visible="<%# this.CanModifyReceiptInfo %>" OnClick="lbDisplayReceiptInfoForm_Click" class="btn" /></div>
						<%: ValueText.GetValueText(Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_RECEIPT_FLG, this.FixedPurchaseContainer.ReceiptFlg) %>
					</td>
				</tr>
				<tr runat="server" visible="<%# this.FixedPurchaseContainer.ReceiptFlg == Constants.FLG_ORDER_RECEIPT_FLG_ON %>">
					<th>宛名</th>
					<td><%: this.FixedPurchaseContainer.ReceiptAddress %></td>
				</tr>
				<tr runat="server" visible="<%# this.FixedPurchaseContainer.ReceiptFlg == Constants.FLG_ORDER_RECEIPT_FLG_ON %>">
					<th>但し書き</th>
					<td><%: this.FixedPurchaseContainer.ReceiptProviso %></td>
				</tr>
			</table>
			<% } else { %>
			<table cellspacing="0">
				<tr>
					<th>領収書希望</th>
					<td>
						<asp:DropDownList ID="ddlReceiptFlg" runat="server" DataTextField="Text" DataValueField="Value"
							OnSelectedIndexChanged="ddlReceiptFlg_OnSelectedIndexChanged" AutoPostBack="true" DataSource="<%# ValueText.GetValueItemList(Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_RECEIPT_FLG) %>" />
					</td>
				</tr>
				<tr id="trReceiptAddressInput" runat="server">
					<th>宛名<span class="necessary">*</span></th>
					<td>
						<asp:TextBox ID="tbReceiptAddress" runat="server" Width="300" MaxLength="100" />
						<asp:CustomValidator runat="Server"
							ControlToValidate="tbReceiptAddress"
							ValidationGroup="ReceiptRegisterModify"
							ClientValidationFunction="ClientValidate"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							CssClass="error_inline" />
					</td>
				</tr>
				<tr id="trReceiptProvisoInput" runat="server">
					<th>但し書き<span class="necessary">*</span></th>
					<td>
						<asp:TextBox ID="tbReceiptProviso" runat="server" Width="300" MaxLength="100" />
						<asp:CustomValidator runat="Server"
							ControlToValidate="tbReceiptProviso"
							ValidationGroup="ReceiptRegisterModify"
							ClientValidationFunction="ClientValidate"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							CssClass="error_inline" />
					</td>
				</tr>
				<tr>
					<th></th>
					<td>
						<asp:Button Text="領収書情報更新" runat="server" OnClientClick="return confirm('領収書情報を変更してもよろしいですか？')" OnClick="btnUpdateReceiptInfo_Click" class="btn" />
						<asp:Button Text="キャンセル" runat="server" OnClientClick="return exec_submit();" OnClick="lbDisplayReceiptInfoForm_Click" class="btn" />
						<br />&nbsp<small>※注文済みの領収書情報は変更されませんのでご注意ください。</small>
						<br /><span style="color: red;"><%: this.ReceiptInfoModifyErrorMessage %></span>
					</td>
				</tr>
			</table>
			<% } %>
		</div>
		<br />
		<% } %>
		<%--△領収書情報△--%>
		<div class="dvFixedPurchaseShipping">
			<%-- お届け先情報 --%>
			<h3>お届け先情報</h3>
			<% if (this.IsUserShippingModify == false) { %>
			<table cellspacing="0">
				<% if (this.UseShippingAddress) { %>
				<tr>
					<th>住所</th>
					<td>
						<div style="float:right">
							<% if (this.Payment.IsPaymentIdAmazonPayCv2 == false) { %>
							<asp:LinkButton Text="お届け先情報変更" runat="server" Visible="<%# this.CanCancelFixedPurchase && this.BeforeCancelDeadline && (this.Payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT) %>" OnClientClick="return exec_submit();" OnClick="lbDisplayUserShippingInfoForm_Click" class="btn" />
							<% } else if(this.CanCancelFixedPurchase && this.BeforeCancelDeadline) { %>
							<%--▼▼ Amazon Pay(CV2)ボタン ▼▼--%>
							<div id="AmazonPayCv2Button" style="display: inline-block"></div>
							<%--▲▲ Amazon Pay(CV2)ボタン ▲▲--%>
							<% } %>
						</div>
							<%= IsCountryJp(this.FixedPurchaseShippingContainer.ShippingCountryIsoCode)
									? "〒" + WebSanitizer.HtmlEncode(this.FixedPurchaseShippingContainer.ShippingZip) + "<br />"
									: "" %>
						<%: this.FixedPurchaseShippingContainer.ShippingAddr1%>
						<%: this.FixedPurchaseShippingContainer.ShippingAddr2%>
							<%: this.FixedPurchaseShippingContainer.ShippingAddr3%><br />
							<%: this.FixedPurchaseShippingContainer.ShippingAddr4%>
							<%: this.FixedPurchaseShippingContainer.ShippingAddr5%><br />
							<%= (IsCountryJp(this.FixedPurchaseShippingContainer.ShippingCountryIsoCode) == false)
									? WebSanitizer.HtmlEncode(this.FixedPurchaseShippingContainer.ShippingZip) + "<br />"
									: "" %>
							<%: this.FixedPurchaseShippingContainer.ShippingCountryName%>
					</td>
				</tr>
				<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
				<tr>
					<%-- 企業名・部署名 --%>
					<th><%: ReplaceTag("@@User.company_name.name@@") %>・
						<%: ReplaceTag("@@User.company_post_name.name@@") %></th>
					<td>
						<%: this.FixedPurchaseShippingContainer.ShippingCompanyName%><br />
						<%: this.FixedPurchaseShippingContainer.ShippingCompanyPostName%>
					</td>
				</tr>
				<% } %>
				<tr>
					<%-- 氏名 --%>
					<th><%: ReplaceTag("@@User.name.name@@", this.ShippingAddrCountryIsoCode) %></th>
					<td>
						<%: this.FixedPurchaseShippingContainer.ShippingName1%><%: this.FixedPurchaseShippingContainer.ShippingName2%>&nbsp;様
						<% if (IsCountryJp(this.FixedPurchaseShippingContainer.ShippingCountryIsoCode)) { %>
						（<%: this.FixedPurchaseShippingContainer.ShippingNameKana1%><%: this.FixedPurchaseShippingContainer.ShippingNameKana2%>&nbsp;さま）
						<% } %>
					</td>
				</tr>
				<tr>
					<%-- 電話番号 --%>
					<th><%: ReplaceTag("@@User.tel1.name@@", this.ShippingAddrCountryIsoCode) %></th>
					<td>
						<%: this.FixedPurchaseShippingContainer.ShippingTel1%>
					</td>
				</tr>
				<% } %>
				<% if (this.UseShippingReceivingStore) { %>
				<tr>
						<th>店舗ID</th>
						<td>
							<div style="float:right"><asp:LinkButton Text="お届け先情報変更" runat="server" Visible="<%# this.CanCancelFixedPurchase && this.BeforeCancelDeadline && (this.Payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT) %>" OnClientClick="return exec_submit();" OnClick="lbDisplayUserShippingInfoForm_Click" class="btn" /></div>
							<%: this.FixedPurchaseShippingContainer.ShippingReceivingStoreId %>
						</td>
					</tr>
					<tr>
						<th>店舗名称</th>
						<td>
							<%: this.FixedPurchaseShippingContainer.ShippingName %>
						</td>
					</tr>
					<tr>
						<th>店舗住所</th>
						<td>
							<%: this.FixedPurchaseShippingContainer.ShippingAddr4 %>
						</td>
					</tr>
					<tr>
						<th>店舗電話番号</th>
						<td>
							<%: this.FixedPurchaseShippingContainer.ShippingTel1 %>
						</td>
					</tr>
				<% } %>
				<tr>
					<th>配送方法</th>
					<td>
						<%: ValueText.GetValueText(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD, this.FixedPurchaseShippingContainer.ShippingMethod) %>
					</td>
				</tr>
				<tr>
					<th>配送サービス</th>
					<td>
						<%: this.DeliveryCompany.DeliveryCompanyName %>
					</td>
				</tr>
				<% if (this.DeliveryCompany.IsValidShippingTimeSetFlg && (this.FixedPurchaseShippingContainer.ShippingMethod == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS) && (this.UseShippingAddress))
				   { %>
					<% if (this.IsShippingTimeModifyAmazonPayCv2 == false) { %>
						<tr>
							<th>配送希望時間帯</th>
							<td>
								<%#: this.HasShippingTime %>
								<div style="float:right">
								<% if (this.Payment.IsPaymentIdAmazonPayCv2) { %>
									<asp:LinkButton Text="配送希望時間帯変更" runat="server" OnClientClick="return exec_submit();" OnClick="lbModifyShippingTimeAmazonPayCv2_Click" class="btn" />
								<% } %>
								</div>
							</td>
						</tr>
					<% } else { %>
						<tr>
							<th>配送希望時間帯</th>
							<td>
								<asp:DropDownList ID="ddlShippingTimeAmazonPayCv2" runat="server"></asp:DropDownList>
							</td>
						</tr>
						<tr align="center">
							<th></th>
							<td>
								<asp:LinkButton Text="配送希望時間帯更新" runat="server" OnClientClick="return confirm('配送希望時間帯を変更してもよろしいですか？')" OnClick="lbUpdateShippingTimeAmazonPayCv2_Click" class="btn" />
								<asp:LinkButton Text="キャンセル" runat="server" OnClick="lbModifyShippingTimeAmazonPayCv2_Click" class="btn" />
							</td>
						</tr>
					<% } %>
				<%} %>
			</table>
			<% }else{ %>
			<asp:HiddenField ID="hfCvsShopFlg" runat="server" Value='<%# this.FixedPurchaseShippingContainer.ShippingReceivingStoreFlg %>' />
			<asp:HiddenField ID="hfSelectedShopId" runat="server" Value='<%# this.FixedPurchaseShippingContainer.ShippingReceivingStoreId %>' />
			<table cellspacing="0">
				<tr>
					<th></th>
					<td>
						<span style="color: red; display: block;">
							<asp:Literal ID="lShippingCountryErrorMessage" runat="server"></asp:Literal>
						</span>
						<asp:DropDownList
							DataTextField="Text"
							DataValueField="Value"
							SelectedValue='<%# IsDisplayButtonConvenienceStore(this.FixedPurchaseContainer.Shippings[0].ShippingReceivingStoreFlg)
								? CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE
								: null %>'
							ID="ddlShippingType"
							AutoPostBack="true"
							OnSelectedIndexChanged="ddlShippingType_SelectedIndexChanged"
							DataSource='<%# GetPossibleShippingType(this.FixedPurchaseContainer.Shippings[0].ShippingReceivingStoreFlg) %>'
							runat="server"
							CssClass="UserShippingAddress">
						</asp:DropDownList>
						<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
						<asp:DropDownList
							ID="ddlShippingReceivingStoreType"
							DataTextField="Text"
							DataValueField="Value"
							Visible='<%# IsDisplayButtonConvenienceStore(this.FixedPurchaseContainer.Shippings[0].ShippingReceivingStoreFlg) %>'
							AutoPostBack="true"
							OnSelectedIndexChanged="ddlShippingReceivingStoreType_SelectedIndexChanged"
							DataSource='<%# ShippingReceivingStoreType() %>'
							runat="server"
							CssClass="UserShippingAddress">
						</asp:DropDownList>
						<% } %>
						<% if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED) { %>
						<span id="spConvenienceStoreSelect" visible="<%# this.UseShippingReceivingStore && (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false) %>" runat="server">
							<a href="javascript:openConvenienceStoreMapPopup();" class="btn btn-success convenience-store-button">Family/OK/Hi-Life</a>
						</span>
						<span id="spConvenienceStoreEcPaySelect" runat="server" visible='<%# ((this.FixedPurchaseContainer.Shippings[0].ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON) && Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) %>'>
							<asp:LinkButton
								ID="lbOpenEcPay"
								runat="server"
								class="btn btn-success convenience-store-button"
								OnClick="lbOpenEcPay_Click"
								Text="  電子マップ  " />
						</span>
						<div id="dvErrorShippingConvenience" style="display: none;" runat="server">
							<span class="error_inline"><%#: WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_GROCERY_STORE) %></span>
						</div>
						<div id="dvErrorPaymentAndShippingConvenience" runat="server" visible="false">
							<span class="error_inline"><%#: WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAYMENT_METHOD_CHANGED_TO_CONVENIENCE_STORE) %></span>
						</div>
						<% } %>
					</td>
				</tr>
				<%-- ▽コンビニ▽ --%>
				<tbody id="divShippingInputFormConvenience" visible="<%# this.UseShippingReceivingStore %>" runat="server" class="shipping_convenience">
						<tr>
							<th><%: ReplaceTag("@@DispText.shipping_convenience_store.shopId@@") %></th>
							<td id="ddCvsShopId">
								<span style="font-weight:normal;">
									<asp:Literal ID="lCvsShopId" runat="server" Text="<%# this.FixedPurchaseShippingContainer.ShippingReceivingStoreId %>"></asp:Literal>
								</span>
								<asp:HiddenField ID="hfCvsShopId" runat="server" Value="<%# this.FixedPurchaseShippingContainer.ShippingReceivingStoreId %>" />
							</td>
						</tr>
						<tr>
							<th><%: ReplaceTag("@@DispText.shipping_convenience_store.shopName@@") %></th>
							<td id="ddCvsShopName">
								<span style="font-weight:normal;">
									<asp:Literal ID="lCvsShopName" runat="server" Text="<%# this.FixedPurchaseShippingContainer.ShippingName %>"></asp:Literal>
								</span>
								<asp:HiddenField ID="hfCvsShopName" runat="server" Value="<%# this.FixedPurchaseShippingContainer.ShippingName %>" />
							</td>
						</tr>
						<tr>
							<th><%: ReplaceTag("@@DispText.shipping_convenience_store.shopAddress@@") %></th>
							<td id="ddCvsShopAddress">
								<span style="font-weight:normal;">
									<asp:Literal ID="lCvsShopAddress" runat="server" Text="<%# this.FixedPurchaseShippingContainer.ShippingAddr4 %>"></asp:Literal>
								</span>
								<asp:HiddenField ID="hfCvsShopAddress" runat="server" Value="<%# this.FixedPurchaseShippingContainer.ShippingAddr4 %>" />
							</td>
						</tr>
						<tr>
							<th><%: ReplaceTag("@@DispText.shipping_convenience_store.shopTel@@") %></th>
							<td id="ddCvsShopTel">
								<span style="font-weight:normal;">
									<asp:Literal ID="lCvsShopTel" runat="server" Text="<%# this.FixedPurchaseShippingContainer.ShippingTel1 %>"></asp:Literal>
								</span>
								<asp:HiddenField ID="hfCvsShopTel" runat="server" Value="<%# this.FixedPurchaseShippingContainer.ShippingTel1 %>" />
							</td>
						</tr>
				</tbody>
				<%-- △コンビニ△ --%>
				<tbody visible="<%# this.UseShippingAddress %>" id="divShippingInputFormInner" runat="server">
					<tr>
					<th><%: ReplaceTag("@@User.name.name@@", this.ShippingAddrCountryIsoCode) %> <span class="necessary">*</span></th>
					<td>
						姓：<asp:TextBox ID="tbShippingName1" runat="server" Width="90" MaxLength="10"></asp:TextBox> 
						名：<asp:TextBox ID="tbShippingName2" runat="server" Width="90" MaxLength="10"></asp:TextBox>
						<asp:CustomValidator
							ID="cvShippingName1"
							runat="Server"
							ControlToValidate="tbShippingName1"
							ValidationGroup="FixedPurchaseModifyInput"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<asp:CustomValidator
							ID="cvShippingName2"
							runat="Server"
							ControlToValidate="tbShippingName2"
							ValidationGroup="FixedPurchaseModifyInput"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% if (this.IsShippingAddrJp) { %>
				<tr>
					<th><%: ReplaceTag("@@User.name_kana.name@@", this.ShippingAddrCountryIsoCode) %> <span class="necessary">*</span></th>
					<td>
						姓：<asp:TextBox ID="tbShippingNameKana1" runat="server" Width="90" MaxLength="20"></asp:TextBox> 
						名：<asp:TextBox ID="tbShippingNameKana2" runat="server" Width="90" MaxLength="20"></asp:TextBox>
						<asp:CustomValidator
							ID="cvShippingNameKana1"
							runat="Server"
							ControlToValidate="tbShippingNameKana1"
							ValidationGroup="FixedPurchaseModifyInput"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<asp:CustomValidator
							ID="cvShippingNameKana2"
							runat="Server"
							ControlToValidate="tbShippingNameKana2"
							ValidationGroup="FixedPurchaseModifyInput"
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
						<%: ReplaceTag("@@User.country.name@@", this.ShippingAddrCountryIsoCode) %>
						<span class="necessary">*</span>
					</th>
					<td>
						<asp:DropDownList runat="server" id="ddlShippingCountry" AutoPostBack="true" OnSelectedIndexChanged="ddlShippingCountry_SelectedIndexChanged"></asp:DropDownList>
							<asp:CustomValidator
								ID="cvShippingCountry"
								runat="Server"
								ControlToValidate="ddlShippingCountry"
								ValidationGroup="FixedPurchaseModifyInput"
								ValidateEmptyText="true"
								SetFocusOnError="true"
								ClientValidationFunction="ClientValidate"
								EnableClientScript="false"
								CssClass="error_inline" />
					</td>
				</tr>
				<% } %>
				<% if (this.IsShippingAddrJp) { %>
				<tr>
					<th>
						<%: ReplaceTag("@@User.zip.name@@", this.ShippingAddrCountryIsoCode) %>
						<span class="necessary">*</span>
					</th>
					<td>
						<asp:TextBox ID="tbShippingZip" OnTextChanged="lbSearchShippingAddr_Click" Width="180" MaxLength="8" runat="server" />
						<asp:LinkButton ID="lbSearchShippingAddr" runat="server" OnClick="lbSearchShippingAddr_Click" class="btn btn-mini" OnClientClick="return false;">
							住所検索
						</asp:LinkButton><br/>
						<%--検索結果レイヤー--%>
						<uc:Layer ID="ucLayer" runat="server" />
						<asp:CustomValidator
							ID="cvShippingZip1"
							runat="Server"
							ControlToValidate="tbShippingZip"
							ValidationGroup="FixedPurchaseModifyInput"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline zip_input_error_message" />
						<span style="color :Red" id="addrSearchErrorMessage" class="shortZipInputErrorMessage"><%: this.ZipInputErrorMessage %></span>
					</td>
				</tr>
				<tr>
					<th>
						<%: ReplaceTag("@@User.addr1.name@@", this.ShippingAddrCountryIsoCode) %>
						<span class="necessary">*</span>
					</th>
					<td>
						<asp:DropDownList ID="ddlShippingAddr1" runat="server"></asp:DropDownList>
						<asp:CustomValidator
							ID="cvShippingAddr1"
							runat="Server"
							ControlToValidate="ddlShippingAddr1"
							ValidationGroup="FixedPurchaseModifyInput"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } %>
				<tr>
					<th>
						<%: ReplaceTag("@@User.addr2.name@@", this.ShippingAddrCountryIsoCode) %> 
						<span class="necessary">*</span>
					</th>
					<td>
						<% if (this.IsShippingAddrTw) { %>
							<asp:DropDownList runat="server" ID="ddlShippingAddr2" DataSource="<%# this.UserTwCityList %>" AutoPostBack="true" DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlShippingAddr2_SelectedIndexChanged"></asp:DropDownList>
						<% } else { %>
							<asp:TextBox ID="tbShippingAddr2" runat="server" Width="300" MaxLength="40"></asp:TextBox>
							<asp:CustomValidator
								ID="cvShippingAddr2"
								runat="Server"
								ControlToValidate="tbShippingAddr2"
								ValidationGroup="FixedPurchaseModifyInput"
								ValidateEmptyText="true"
								SetFocusOnError="true"
								ClientValidationFunction="ClientValidate"
								CssClass="error_inline" />
						<% } %>
					</td>
				</tr>
				<tr>
					<th>
						<%: ReplaceTag("@@User.addr3.name@@", this.ShippingAddrCountryIsoCode) %>
						<% if (IsAddress3Necessary(this.ShippingAddrCountryIsoCode)){ %><span class="necessary">*</span><% } %>
					</th>
					<td>
						<% if (this.IsShippingAddrTw) { %>
							<asp:DropDownList runat="server" ID="ddlShippingAddr3" DataTextField="Key" DataValueField="Value" Width="95" ></asp:DropDownList>
						<% } else { %>
							<asp:TextBox ID="tbShippingAddr3" runat="server" Width="300" MaxLength="40"></asp:TextBox>
							<asp:CustomValidator
								ID="cvShippingAddr3"
								runat="Server"
								ControlToValidate="tbShippingAddr3"
								ValidationGroup="FixedPurchaseModifyInput"
								ValidateEmptyText="true"
								SetFocusOnError="true"
								ClientValidationFunction="ClientValidate"/>
						<% } %>
					</td>
				</tr>
				<tr <%: (Constants.DISPLAY_ADDR4_ENABLED || (this.IsShippingAddrJp == false)) ? "" : "style=\"display:none;\"" %>>
					<th>
						<%: ReplaceTag("@@User.addr4.name@@", this.ShippingAddrCountryIsoCode) %> 
						<%if (this.IsShippingAddrJp == false) {%><span class="necessary">*</span><%} %>
					</th>
					<td>
						<asp:TextBox ID="tbShippingAddr4" runat="server" Width="300" MaxLength="40"></asp:TextBox>
						<asp:CustomValidator
							ID="cvShippingAddr4"
							runat="Server"
							ControlToValidate="tbShippingAddr4"
							ValidationGroup="FixedPurchaseModifyInput"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% if (this.IsShippingAddrJp == false) { %>
				<tr>
					<th>
						<%: ReplaceTag("@@User.addr5.name@@", this.ShippingAddrCountryIsoCode) %>
						<% if (this.IsShippingAddrUs) { %><span class="necessary">*</span><% } %>
					</th>
					<td>
						<% if (this.IsShippingAddrUs) { %>
						<asp:DropDownList runat="server" id="ddlShippingAddr5"></asp:DropDownList>
							<asp:CustomValidator
								ID="cvShippingAddr5Ddl"
								runat="Server"
								ControlToValidate="ddlShippingAddr5"
								ValidationGroup="FixedPurchaseModifyInputGlobal"
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
							ValidationGroup="FixedPurchaseModifyInputGlobal"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<% } %>
					</td>
				</tr>
				<%-- 郵便番号（海外向け） --%>
				<tr>
					<th>
						<%: ReplaceTag("@@User.zip.name@@", this.ShippingAddrCountryIsoCode) %> 
						<% if (this.IsShippingAddrZipNecessary) { %><span class="necessary">*</span><% } %>
					</th>
					<td>
						<asp:TextBox ID="tbShippingZipGlobal" runat="server" MaxLength="20" />
						<asp:CustomValidator
							ID="cvShippingZipGlobal"
							runat="Server"
							ControlToValidate="tbShippingZipGlobal"
							ValidationGroup="FixedPurchaseModifyInputGlobal"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<asp:LinkButton
							ID="lbSearchAddrFromShippingZipGlobal"
							OnClick="lbSearchAddrFromShippingZipGlobal_Click"
							Style="display:none;"
							runat="server" />
					</td>
				</tr>
				<% } %>
				<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
				<tr>
					<th><%: ReplaceTag("@@User.company_name.name@@") %> </th>
					<td>
						<asp:TextBox ID="tbShippingCompanyName" runat="server" Width="300" MaxLength="40"></asp:TextBox>
						<asp:CustomValidator
							ID="cvShippingCompanyName"
							runat="Server"
							ControlToValidate="tbShippingCompanyName"
							ValidationGroup="FixedPurchaseModifyInput"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<tr>
					<th><%: ReplaceTag("@@User.company_post_name.name@@") %> </th>
					<td>
						<asp:TextBox ID="tbShippingCompanyPostName" runat="server" Width="300" MaxLength="40"></asp:TextBox>
						<asp:CustomValidator
							ID="cvShippingCompanyPostName"
							runat="Server"
							ControlToValidate="tbShippingCompanyPostName"
							ValidationGroup="FixedPurchaseModifyInput"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } %>
				<tr>
					<% if (this.IsShippingAddrJp) { %>
					<th>
						<%: ReplaceTag("@@User.tel1.name@@", this.ShippingAddrCountryIsoCode) %>
						<span class="necessary">*</span>
					</th>
					<td>
						<asp:TextBox ID="tbShippingTel" Width="180" MaxLength="13" runat="server" CssClass="shortTel" />
						<asp:CustomValidator
							ID="cvShippingTel1"
							runat="Server"
							ControlToValidate="tbShippingTel"
							ValidationGroup="FixedPurchaseModifyInput"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
					<% } else { %>
					<th>
						<%: ReplaceTag("@@User.tel1.name@@", this.ShippingAddrCountryIsoCode) %>
						<span class="necessary">*</span>
					</th>
					<td>
						<asp:TextBox runat="server" ID="tbShippingTel1Global" MaxLength="30"></asp:TextBox>
						<asp:CustomValidator
							ID="cvShippingTel1Global"
							runat="Server"
							ControlToValidate="tbShippingTel1Global"
							ValidationGroup="FixedPurchaseModifyInputGlobal"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
					<% } %>
				</tr>
				<tr>
					<th>配送方法</th>
					<td>
						<asp:DropDownList ID="ddlShippingMethod" OnSelectedIndexChanged="ddlShippingMethod_OnSelectedIndexChanged" runat="server" AutoPostBack="true"></asp:DropDownList>
					</td>
				</tr>
				<tr>
					<th>配送サービス</th>
					<td>
						<asp:DropDownList ID="ddlDeliveryCompany" OnSelectedIndexChanged="ddlDeliveryCompanyList_OnSelectedIndexChanged" AutoPostBack="true" runat="server"/>
					</td>
				</tr>
				<% if (this.SelectedDeliveryCompany.IsValidShippingTimeSetFlg
						&& (this.WddlShippingMethod.SelectedValue
							== Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
						&& this.UseShippingAddress) { %>
				<tr>
					<th>配送希望時間帯</th>
					<td>
						<asp:DropDownList ID="ddlShippingTime" runat="server"></asp:DropDownList>
					</td>
				</tr>
				<%} %>
					</tbody>
					<tbody runat="server" visible="false" id="tbOwnerAddress">
						<tr>
							<th>住所</th>
							<td>
								<% if (IsCountryJp(this.LoginUser.AddrCountryIsoCode)) { %>
									〒<%: this.LoginUser.Zip %><br />
								<% } %>
								<%: this.LoginUser.Addr1 %>
								<%: this.LoginUser.Addr2 %>
								<%: this.LoginUser.Addr3 %>
								<%: this.LoginUser.Addr4 %><br />
								<%: this.LoginUser.Addr5 %>
								<% if (IsCountryJp(this.LoginUser.AddrCountryIsoCode) == false) { %>
									<%: this.LoginUser.Zip %><br />
								<% } %>
							</td>
						</tr>
						<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
						<tr>
							<%-- 企業名・部署名 --%>
							<th><%: ReplaceTag("@@User.company_name.name@@") %>・
						<%: ReplaceTag("@@User.company_post_name.name@@") %></th>
							<td>
								<%: this.LoginUser.CompanyName %><br />
								<%: this.LoginUser.CompanyPostName %>
							</td>
						</tr>
						<% } %>
						<tr>
							<%-- 氏名 --%>
							<th><%: ReplaceTag("@@User.name.name@@", this.LoginUser.AddrCountryIsoCode) %></th>
							<td>
								<%: this.LoginUser.Name1%><%: this.LoginUser.Name2%>&nbsp;様
						<% if (IsCountryJp(this.LoginUser.AddrCountryIsoCode)) { %>
						（<%: this.LoginUser.NameKana1%><%: this.LoginUser.NameKana2%>&nbsp;さま）
						<% } %>
							</td>
						</tr>
						<tr>
							<%-- 電話番号 --%>
							<th><%: ReplaceTag("@@User.tel1.name@@", this.LoginUser.AddrCountryIsoCode) %></th>
							<td>
								<%: this.LoginUser.Tel1 %>
							</td>
						</tr>
					</tbody>
					<asp:Repeater Visible="false" runat="server" ItemType="UserShippingModel" DataSource="<%# this.UserShippingAddr %>" ID="rOrderShippingList">
						<ItemTemplate>
							<tbody id="Tbody1" class='<%# string.Format("user_addres{0}", Item.ShippingNo) %>' runat="server" visible="<%# (Item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON) %>">
								<asp:HiddenField runat="server" ID="hfShippingNo" Value="<%# Item.ShippingNo %>" />
								<tr>
									<th>店舗ID</th>
									<td class="convenience-store-item" id="ddCvsShopId">
										<span style="font-weight:normal;">
											<asp:Label ID="lbCvsShopId" runat="server" Text="<%# Item.ShippingReceivingStoreId %>"></asp:Label>
										</span>
										<asp:HiddenField ID="hfCvsShopId" runat="server" Value="<%# Item.ShippingReceivingStoreId %>" />
									</td>
								</tr>
								<tr>
									<th>店舗名</th>
									<td class="convenience-store-item" id="ddCvsShopName">
										<span style="font-weight:normal;">
											<asp:Literal ID="lCvsShopName" runat="server" Text="<%# Item.ShippingName %>"></asp:Literal>
										</span>
										<asp:HiddenField ID="hfCvsShopName" runat="server" Value="<%# Item.ShippingName %>" />
									</td>
								</tr>
								<tr>
									<th>店舗住所</th>
									<td class="convenience-store-item" id="ddCvsShopAddress">
										<span style="font-weight:normal;">
											<asp:Literal ID="lCvsShopAddress" runat="server" Text="<%# Item.ShippingAddr4 %>"></asp:Literal>
										</span>
										<asp:HiddenField ID="hfCvsShopAddress" runat="server" Value="<%# Item.ShippingAddr4 %>" />
									</td>
								</tr>
								<tr>
									<th>店舗電話番号</th>
									<td class="convenience-store-item" id="ddCvsShopTel">
										<span style="font-weight:normal;">
											<asp:Literal ID="lCvsShopTel" runat="server" Text="<%# Item.ShippingTel1 %>"></asp:Literal>
										</span>
										<asp:HiddenField ID="hfCvsShopTel" runat="server" Value="<%# Item.ShippingTel1 %>" />
									</td>
								</tr>
							</tbody>
							<tbody id="Tbody2" runat="server" visible="<%# (Item.ShippingReceivingStoreFlg != Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON) %>">
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
				<tr align="center">
					<th></th>
					<td>
							<asp:Button Text="お届け先情報更新" runat="server" OnClientClick="return CheckStoreAvailable();" OnClick="btnUpdateUserShippingInfo_Click" class="btn" />
						<asp:Button Text="キャンセル" runat="server" OnClick="lbDisplayUserShippingInfoForm_Click" class="btn" />
						<br />&nbsp	<small>※注文済みのお届け先情報は変更されませんのでご注意ください。</small>
						<br /><small id="sErrorMessageShipping" runat="server" class="error" style="padding: 2px;"></small>
					</td>
				</tr>
			</table>
			<% } %>
		<br />
		<%-- 利用発票情報 --%>
		<% if (this.IsShowTwFixedPurchaseInvoiceInfo) { %>
			<h3>利用発票情報</h3>
			<% if (this.IsTwFixedPurchaseInvoiceModify == false) { %>
			<table cellspacing="0">
				<tr>
					<th>発票種類</th>
					<td>
						<div style="float: right">
							<asp:LinkButton Text="発票情報変更" runat="server" Visible="<%# this.CanCancelFixedPurchase && this.BeforeCancelDeadline %>" OnClick="lbDisplayTwInvoiceInfoForm_Click" class="btn" /></div>
						<%: ValueText.GetValueText(Constants.TABLE_TWFIXEDPURCHASEINVOICE, Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_UNIFORM_INVOICE, this.TwFixedPurchaseInvoiceModel.TwUniformInvoice) %>
						<% if (this.TwFixedPurchaseInvoiceModel.TwUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_COMPANY) { %>
							<p style="padding-bottom: 5px; padding-top: 15px;">統一編号</p>
							<%: this.TwFixedPurchaseInvoiceModel.TwUniformInvoiceOption1 %>
							<p style="padding-bottom: 5px; padding-top: 5px;">会社名</p>
							<%: this.TwFixedPurchaseInvoiceModel.TwUniformInvoiceOption2 %>
						<% } %>
						<% if (this.TwFixedPurchaseInvoiceModel.TwUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_DONATE) { %>
							<p style="padding-bottom: 5px; padding-top: 15px;">寄付先コード</p>
							<%: this.TwFixedPurchaseInvoiceModel.TwUniformInvoiceOption1 %>
						<% } %>
					</td>
				</tr>
				<% if (this.TwFixedPurchaseInvoiceModel.TwUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL) { %>
				<tr>
					<th>共通性載具</th>
					<td>
						<%: ValueText.GetValueText(Constants.TABLE_TWFIXEDPURCHASEINVOICE, Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_CARRY_TYPE, this.TwFixedPurchaseInvoiceModel.TwCarryType) %>
						<br />
						<%: this.TwFixedPurchaseInvoiceModel.TwCarryTypeOption %>
					</td>
				</tr>
				<% } %>
			</table>
			<% } else { %>
			<table cellspacing="0">
				<tr>
					<th>発票種類</th>
					<td>
						<asp:DropDownList ID="ddlTaiwanUniformInvoice" DataTextField="Text" DataValueField="Value" runat="server"
							OnSelectedIndexChanged="ddlTaiwanUniformInvoice_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
						&nbsp;
						<span id="spTaiwanUniformInvoiceOptionKbnList" runat="server" visible="false">
							<asp:DropDownList ID="ddlTaiwanUniformInvoiceOptionKbnList" DataTextField="Text" DataValueField="Value" runat="server"
								OnSelectedIndexChanged="ddlTaiwanUniformInvoiceOptionKbnList_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
						</span>
						<div id="dvInvoiceForm" runat="server" style="padding-top:5px;" visible="false">
							<div id="dvInvoiceDisp" runat="server" visible="false">
								<br />
								<div id="dvInvoiceDispForCompanyType" runat="server">
									<p style="padding-bottom: 5px; padding-top: 5px;">統一編号</p>
									<asp:Literal ID="lCompanyCode" runat="server"></asp:Literal>
									<p style="padding-bottom: 5px; padding-top: 5px;">会社名</p>
									<asp:Literal ID="lCompanyName" runat="server"></asp:Literal>
								</div>
								<div id="dvInvoiceDispForDonateType" runat="server" visible="false">
									<p style="padding-bottom: 5px; padding-top: 5px;">寄付先コード</p>
									<asp:Literal ID="lDonationCode" runat="server"></asp:Literal>
								</div>
								<br />
							</div>
							<div id="dvInvoiceInput" runat="server" visible="false">
							<br />
							<div id="dvInvoiceInputForCompanyType" runat="server" visible="false">
								<p style="padding-bottom: 5px; padding-top: 5px;">統一編号</p>
								<asp:TextBox runat="server" ID="tbUniformInvoiceOption1_8" placeholder="例:12345678" MaxLength="8"></asp:TextBox>
								<asp:CustomValidator
									ID="cvUniformInvoiceOption1_8"
									runat="Server"
									ControlToValidate="tbUniformInvoiceOption1_8"
									ValidationGroup="OrderShippingGlobal"
									ValidateEmptyText="true"
									SetFocusOnError="true"
									ClientValidationFunction="ClientValidate"
									CssClass="error_inline" />
								<p style="padding-bottom: 5px; padding-top: 5px;">会社名</p>
								<asp:TextBox runat="server" ID="tbUniformInvoiceOption2" placeholder="例:○○有限股份公司" MaxLength="20"></asp:TextBox>
								<asp:CustomValidator
									ID="cvUniformInvoiceOption2"
									runat="Server"
									ControlToValidate="tbUniformInvoiceOption2"
									ValidationGroup="OrderShippingGlobal"
									ValidateEmptyText="true"
									SetFocusOnError="true"
									ClientValidationFunction="ClientValidate"
									CssClass="error_inline" />
							</div>
							<div id="dvInvoiceInputForDonateType" runat="server" visible="false">
								<p style="padding-bottom: 5px; padding-top: 5px;">寄付先コード</p>
								<asp:TextBox runat="server" ID="tbUniformInvoiceOption1_3" MaxLength="7"></asp:TextBox>
								<asp:CustomValidator
									ID="cvUniformInvoiceOption1_3"
									runat="Server"
									ControlToValidate="tbUniformInvoiceOption1_3"
									ValidationGroup="OrderShippingGlobal"
									ValidateEmptyText="true"
									SetFocusOnError="true"
									ClientValidationFunction="ClientValidate"
									CssClass="error_inline" />
							</div>
							<asp:CheckBox ID="cbSaveToUserInvoice" Visible="false" runat="server" Text="電子発票管理情報を保存する" AutoPostBack="true" OnCheckedChanged="cbSaveToUserInvoice_CheckedChanged" />
							<div id="dvUniformInvoiceTypeRegistInput" runat="server" visible="false">
								電子発票情報名<span class="fred">※</span>
								<asp:TextBox ID="tbUniformInvoiceTypeName" MaxLength="30" runat="server"></asp:TextBox>
								<asp:CustomValidator
									ID="cvUniformInvoiceTypeName" runat="server"
									ControlToValidate="tbUniformInvoiceTypeName"
									ValidationGroup="OrderShippingGlobal"
									ValidateEmptyText="true"
									ClientValidationFunction="ClientValidate"
									SetFocusOnError="true"
									CssClass="error_inline" />
							</div>
							<br />
							</div>
						</div>
					</td>
				</tr>
				<tr id="trInvoiceDispForPersonalType" runat="server" visible="false">
					<th>共通性載具</th>
					<td>
						<asp:DropDownList ID="ddlTaiwanCarryType" DataTextField="Text" DataValueField="Value" runat="server"
							OnSelectedIndexChanged="ddlTaiwanCarryType_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
						&nbsp;
						<span id="spTaiwanCarryKbnList" runat="server" visible="false">
							<asp:DropDownList ID="ddlTaiwanCarryKbnList" DataTextField="Text" DataValueField="Value" runat="server"
								OnSelectedIndexChanged="ddlTaiwanCarryKbnList_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
						</span>
						<div id="dvInvoiceNoEquipmentMessage" runat="server" style="color:red; padding-top:15px;" visible="false">
							<asp:Literal ID="lInvoiceNoEquipmentMessage" runat="server" Text="※電子発票は商品と一緒に送ります。"></asp:Literal>
						</div>
						<div id="dvInvoiceDispForPersonalType" runat="server" visible="false">
							<br />
							<asp:Literal ID="lInvoiceCode" runat="server"></asp:Literal>
							<br />
						</div>
						<div id="dvInvoiceInputForPersonalType" runat="server" visible="false">
							<br />
							<div id="dvCarryTypeOption_8" runat="server" visible="false">
								<asp:TextBox runat="server" Width="220" ID="tbCarryTypeOption_8" placeholder="例:/AB201+9(限8個字)" MaxLength="8"></asp:TextBox>
								<asp:CustomValidator
									ID="cvCarryTypeOption_8"
									runat="Server"
									ControlToValidate="tbCarryTypeOption_8"
									ValidationGroup="OrderShippingGlobal"
									ValidateEmptyText="true"
									SetFocusOnError="true"
									ClientValidationFunction="ClientValidate"
									CssClass="error_inline" />
							</div>
							<div id="dvCarryTypeOption_16" runat="server" visible="false">
								<asp:TextBox runat="server" Width="220" ID="tbCarryTypeOption_16" placeholder="例:TP03000001234567(限16個字)" MaxLength="16"></asp:TextBox>
								<asp:CustomValidator
									ID="cvCarryTypeOption_16"
									runat="Server"
									ControlToValidate="tbCarryTypeOption_16"
									ValidationGroup="OrderShippingGlobal"
									ValidateEmptyText="true"
									SetFocusOnError="true"
									ClientValidationFunction="ClientValidate"
									CssClass="error_inline" />
							</div>
							<asp:CheckBox ID="cbCarryTypeOptionRegist" runat="server" Visible="false" Text="電子発票管理情報を保存する" AutoPostBack="true" OnCheckedChanged="cbCarryTypeOptionRegist_CheckedChanged" />
							<div id="dvCarryTypeOptionName" runat="server" visible="false">
								電子発票情報名<span class="fred">※</span>
								<asp:TextBox ID="tbCarryTypeOptionName" runat="server" MaxLength="30"></asp:TextBox>
								<asp:CustomValidator
									ID="cvCarryTypeOptionName" runat="server"
									ControlToValidate="tbCarryTypeOptionName"
									ValidationGroup="OrderShippingGlobal"
									ValidateEmptyText="true"
									ClientValidationFunction="ClientValidate"
									SetFocusOnError="true"
									CssClass="error_inline" />
							</div>
							<br />
						</div>
					</td>
				</tr>
				<tr align="center">
					<th></th>
					<td>
						<asp:Button ID="btnUpdateInvoiceInfo" Text="  情報更新  " runat="server" OnClientClick="return confirm('発票情報を変更してもよろしいですか？')" OnClick="btnUpdateInvoiceInfo_Click" class="btn" />
						<asp:Button Text="  キャンセル  " runat="server" OnClick="lbDisplayTwInvoiceInfoForm_Click" class="btn" />
					</td>
				</tr>
			</table>
			<% } %>
		<% } %>
		<br />
		<div Visible="<%# this.FixedPurchaseContainer.IsSubsctriptionBox %>" runat="server">
			<h3>頒布会情報</h3>
			<table cellspacing="0">
				<tr>
					<th>頒布会コース名</th>
					<td>
						<div>
							<%#: GetSubscriptionBoxDisplayName() %>
						</div>
						<% if(this.IsAbleToChangeSubscriptionBoxItemOnFront && HasAnySubscriptionBoxItemAvailableToSwapWith()) { %>
							<div style="text-align:right; float:right;">
								<asp:Button ID="btnChangeProduct" Text="  お届け商品変更  " runat="server" OnClick="btnChangeProduct_Click" class="btn" Enabled="<%# this.BeforeCancelDeadline %>"/>
								<% if (Constants.SUBSCRIPTION_BOX_PRODUCT_CHANGE_METHOD.IsModal()) { %>
									<asp:Button ID="btnChangeProductWithClearingSelection" Text="  すべて入れ替える  " OnClick="btnChangeProduct_Click" class="btn" Enabled="<%# this.BeforeCancelDeadline %>" runat="server" />
								<% } %>
							</div>
							<div style="text-align:right; padding-top:30px;">
								<small id="sErrorMessageSubscriptionBox" runat="server"></small>
							</div>
						<% } %>
					</td>
				</tr>
				<tr Visible="<%# this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount %>" runat="server">
					<th>定額価格</th>
					<td colspan="2">
						<%: CurrencyManager.ToPrice(this.FixedPurchaseContainer.SubscriptionBoxFixedAmount.ToPriceDecimal()) %>
					</td>
				</tr>
			</table>
		</div>
		<br />
		<br />
		<%-- 購入商品一覧 --%>
		<div class="dvFixedPurchaseItem" id="dvListProduct" runat="server" visible="true">
			<table cellspacing="0">
				<asp:Repeater ID="rItem" ItemType="FixedPurchaseItemInput" runat="server" OnItemDataBound="rItem_OnItemDataBound">
					<HeaderTemplate>
						<tr>
							<th class="productName" colspan="2">
								商品名
							</th>
							<th class="productPrice" Visible="<%# this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount == false %>" runat="server">
								単価（<%#: this.ProductPriceTextPrefix %>）
							</th>
							<% if (IsDisplayOptionPrice()){ %>
							<th class="productOptionPrice">
								オプション価格（<%#: this.ProductPriceTextPrefix %>）
							</th>
							<% } %>
							<th class="orderCount">
								注文数
							</th>
							<th class="orderSubtotal" Visible="<%# this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount == false %>" runat="server">
								小計（<%#: this.ProductPriceTextPrefix %>）
							</th>
						</tr>
					</HeaderTemplate>
					<ItemTemplate>
						<tr>
							<td class="productImage">
								<w2c:ProductImage ImageSize="S" IsVariation='<%# Item.HasVariation %>' ProductMaster="<%# GetProduct(Item.ShopId, Item.ProductId, Item.VariationId) %>" runat="server" Visible="True" />
							</td>
							<td class="productName">
								<%#: Item.CreateProductJointName() %>
								<br />
								[<span class="productId"><%#: (Item.ProductId == Item.VariationId) ? Item.ProductId : Item.VariationId %></span>]
								<span visible='<%# Item.ProductOptionTexts != "" %>' runat="server">
									<br />
									<%# WebSanitizer.HtmlEncode(Item.GetDisplayProductOptionTexts()).Replace("　", "<br />")%>
								</span>

								<!-- 頒布会だったらここを抜ける -->

								<div visible='<%# Item.HasVariation %>' style="margin-top: 5px" runat="server">
									<asp:LinkButton Text="バリエーション変更" runat="server" OnClick="lbChangeProductVariation_Click"
										CommandArgument='<%# string.Format("{0},{1},{2}",Item.ProductId,Item.VariationId,Container.ItemIndex) %>' Enabled="<%# this.BeforeCancelDeadline %>" class="btn" ID="lbChangeVariation" />
									<% this.DisplayVariationArea = true; %>
								</div>
								<div id="dvUpdateProductVariation" visible='<%# (this.WhfOldVariationId.Value == Item.VariationId) %>' style="margin-top: 5px" runat="server">
									<asp:DropDownList ID="ddlProductVariationList" runat="server" AutoPostBack="true"
										DataSource="<%# GetVariationListItems(Item.ProductId) %>" DataTextField="text"
										DataValueField="value" OnSelectedIndexChanged="ddlProductVariationList_OnSelectedIndexChanged">
									</asp:DropDownList>
									<p>
										<w2c:ProductImage ID="ProductImage" ImageSize="S"
											ProductMaster="<%# GetProduct(Item.ShopId, Item.ProductId, this.WhfSelectedVariationId.Value) %>"
											IsVariation="True" runat="server" Visible="True" />
									</p>
									<p>単価（<%#: this.ProductPriceTextPrefix %>）<%# CurrencyManager.ToPrice(this.NewPrice) %></p>
									<div style="margin-top: 5px" runat="server">
										<asp:LinkButton ID="lbProductUpdate" Text="商品更新" OnClick="lbUpdateProductVariation_Click" OnClientClick="return AlertProductChange(this);" CssClass="btn" runat="server" />
										<asp:LinkButton ID="lbUpdateCancel" Text="キャンセル" OnClick="lbCloseUpdateProductVariationForm_Click" CssClass="btn" runat="server" />
									</div>
								</div>
								<div visible='<%# IsDisplayButtonChangeProductOptionValue(Container) %>' style="margin-top: 5px" runat="server">
									<asp:LinkButton Text="選択項目変更" OnClick="lbChangeProductOptionValue_Click" CommandArgument='<%# string.Format("{0},{1}", Container.ItemIndex, Item.ProductOptionTexts) %>' Enabled="<%# this.BeforeCancelDeadline %>" class="btn" ID="lbChangeProductOptionValue" runat="server" />
								</div>
								<div id="dvProductOptionValue" visible='<%# IsDisplayProductOptionValueArea(Container) %>' style="margin: 10px 0" runat="server">
									<!-- ▽ 付帯情報編集欄 ▽ -->
									<asp:Repeater ID="rProductOptionValueSettings" ItemType="w2.App.Common.Product.ProductOptionSetting" DataSource="<%# GetProductOptionSettingList(Item) %>" runat="server">
										<ItemTemplate>
											<div style="margin-top: 6px;">
												<strong style="display: inline-block; padding: 2px 1px;"><%#: Item.ValueName %><span class="notice" style="color: red;" runat="server" visible="<%# Item.IsNecessary %>">*</span></strong><br/>
												<span style="display: inline-block">
													<asp:DropDownList runat="server" ID="ddlProductOptionValueSetting" DataSource="<%# Item.SettingValuesListItemCollection %>" Visible="<%# Item.IsSelectMenu || Item.IsDropDownPrice %>" SelectedValue="<%# Item.GetDisplayProductOptionSettingSelectedValue() %>" Width="120" />
													<p class="attention"><asp:Label ID="lblProductOptionDropdownErrorMessage" runat="server" /></p>
													<asp:TextBox ID="tbProductOptionValueSetting" Text="<%# Item.SelectedSettingValueForTextBox %>" Visible="<%# Item.IsTextBox %>" runat="server" />
													<p class="attention"><asp:Label ID = "lblProductOptionErrorMessage" runat="server" /></p>
													<asp:Repeater ID="rCblProductOptionValueSetting" DataSource="<%# Item.SettingValuesListItemCollection %>" ItemType="System.Web.UI.WebControls.ListItem" Visible="<%# Item.IsCheckBox || Item.IsCheckBoxPrice %>" runat="server">
														<ItemTemplate>
															<span title="<%# Item.Text %>" id="ProductOptionCp">
																<asp:CheckBox ID="cbProductOptionValueSetting" Text='<%# GetAbbreviatedProductOptionTextForDisplay(Item.Text) %>' Checked='<%# Item.Selected %>' runat="server" />
																<asp:HiddenField ID="hfCbOriginalValue" runat="server" Value='<%# Item.Text %>'/>
															</span>
														</ItemTemplate>
													</asp:Repeater>
													<p class="attention"><asp:Label ID="lblProductOptionCheckboxErrorMessage" runat="server" /></p>
												</span>
											</div>
										</ItemTemplate>
									</asp:Repeater>
									<!-- △ 付帯情報編集欄 △ -->
									<div style="margin-top: 5px" runat="server">
										<asp:HiddenField ID="hfProductOptionPrice" Value="<%# Item.OptionPrice %>" runat="server"/>
										<asp:LinkButton ID="lbProductOptionValueUpdate" Text="更新" OnClientClick="return AlertProductOptionValueChange()" CommandArgument="<%# Item.ProductId %>" OnClick="lbUpdateProductOptionValue_Click" CssClass="btn" runat="server" />
										<asp:LinkButton ID="lbProductOptionValueCancel" Text="キャンセル" OnClick="lbCloseUpdateProductOptionValueForm_Click" CssClass="btn" runat="server" />
									</div>
								</div>
								
								<!-- ここまで -->
							</td>
							<td class="productPrice" Visible="<%# this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount == false %>" runat="server">
								<span ID="sProductPrice" runat="server">
								<%#: CurrencyManager.ToPrice(Item.GetValidPrice()) %>
								</span>
							</td>
							<% if (IsDisplayOptionPrice()){ %>
								<td class="productPrice" Visible="<%# this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount == false %>" runat="server">
									<span ID="oProductPrice" runat="server">
										<%#: Item.ProductOptionSettingsWithSelectedValues.HasOptionPrice == false ? "―" : CurrencyManager.ToPrice(Item.OptionPrice) %>
									</span>
								</td>
							<% } %>
							<td class="orderCount">
								<span ID="sItemQuantity" runat="server">
								<%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Item.ItemQuantity))%>
								</span>
							</td>
							<td class="orderSubtotal" Visible="<%# this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount == false %>" runat="server">
								<span ID="sOrderSubtotal" runat="server">
									<%#: CurrencyManager.ToPrice(Item.ItemPriceIncludedOptionPrice) %>
								</span>
							</td>
						</tr>
						<asp:HiddenField ID="hfProductPrice" Value="<%#: Item.GetValidPrice().ToPriceString() %>" runat="server"/>
						<asp:HiddenField ID="hfOrderSubtotal" Value="<%#: Item.ItemPriceIncludedOptionPrice.ToPriceString() %>" runat="server"/>
						<asp:HiddenField ID="hfProductId" Value="<%#: Item.ProductId %>" runat="server"/>
						<asp:HiddenField ID="hfVariationId" Value="<%#: Item.VariationId %>" runat="server"/>
					</ItemTemplate>
				</asp:Repeater>
			</table>
		</div>

		<%-- 頒布会：商品変更（プルダウン方式） --%>
		<div class="dvFixedPurchaseItem" id="dvModifySubscription" runat="server" visible="false">
			<div>
				<small ID="sErrorQuantity" class="error" runat="server"></small>
			</div>
			<div class="right" style="margin-bottom: 20px;">
				<asp:Button ID="btnAddProduct" Text="  商品追加  " runat="server" class="btn" style="margin-right: 10px;" OnClick="btnAddProduct_Click" Visible="<%# CanAddProduct() %>"/>
				<asp:Button ID="btnUpdateProduct" Text="  更新  " runat="server" class="btn" OnClick="btnUpdateProduct_Click" OnClientClick="return AlertSubscriptionBoxProductChange();"/>
				<asp:Button ID="btnCloseProduct" Text="  キャンセル  " runat="server" class="btn" OnClick="btnCloseProduct_Click" />
			</div>
			<table cellspacing="0">
				<asp:Repeater ID="rItemModify" ItemType="FixedPurchaseItemInput" runat="server" OnItemDataBound="rItemModify_OnItemDataBound" OnItemCommand="rProductChange_ItemCommand">
					<HeaderTemplate>
						<tr>
							<th class="subscriptionBoxProductName" colspan="2">
								商品名
							</th>
							<th class="subscriptionBoxProductPrice" Visible="<%# this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount == false %>" runat="server">
								単価（<%#: this.ProductPriceTextPrefix %>）
							</th>
							<th class="subscriptionBoxOrderCount">
								注文数
							</th>
							<th class="subscriptionBoxOrderSubtotal" Visible="<%# this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount == false %>" runat="server">
								小計（<%#: this.ProductPriceTextPrefix %>）
							</th>
							<th class="subscriptionBoxProductDelete">
								削除
							</th>
						</tr>
					</HeaderTemplate>
					<ItemTemplate>
						<tr>
							<td class="productImage">
								<w2c:ProductImage ImageSize="S" IsVariation='<%# Item.HasVariation %>' ProductMaster="<%# GetProduct(Item.ShopId, Item.ProductId, Item.VariationId) %>" runat="server" Visible="True" />
							</td>
							<td class="productName">
								<asp:DropDownList ID="ddlProductName" Enabled="<%# CanModify(Item.VariationId) %>" DataSource="<%# GetSubscriptionBoxProductList(Item.ProductId,Item.VariationId) %>" DataTextField="Text" DataValueField="Value" runat="server"
									OnSelectedIndexChanged="ReCalculation" AutoPostBack="true"></asp:DropDownList>
								<br />
								<span runat="server" Visible="<%# CanModify(Item.VariationId) == false %>"><%: GetSubscriptionBoxNecessaryMessage() %></span>
							</td>
							<td class="productPrice" Visible="<%# this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount == false %>" runat="server">
								<span ID="sProductPrice" runat="server"><%#: CurrencyManager.ToPrice(Item.GetValidPrice()) %></span>
							</td>
							<td class="orderCount">
								<asp:DropDownList ID="ddlQuantityUpdate" runat="server" DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ReCalculation" AutoPostBack="true" />
							</td>
							<td class="orderSubtotal" Visible="<%# this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount == false %>" runat="server">
								<span ID="sOrderSubtotal" runat="server">
									<%#: CurrencyManager.ToPrice(Item.ItemPriceIncludedOptionPrice) %>
								</span>
							</td>
							<td class="delete">
								<asp:LinkButton Text="x" runat="server" Visible="<%# CanModify(Item.VariationId) %>" CommandName="DeleteRow" CommandArgument='<%# Container.ItemIndex %>'/>
							</td>
						</tr>
						<%-- 商品変更数選択肢（1から設定分表示） --%>
						<asp:HiddenField ID="hfMaxItemQuantity" runat="server" Value="10" />
						<asp:HiddenField ID="hfItemQuantity" runat="server" Value="<%#: Item.ItemQuantity %>" />
						<asp:HiddenField ID="hfProductPrice" Value="<%#: Item.GetValidPrice().ToPriceString() %>" runat="server"/>
						<asp:HiddenField ID="hfOrderSubtotal" Value="<%#: Item.ItemPriceIncludedOptionPrice.ToPriceString() %>" runat="server"/>
						<asp:HiddenField ID="hfFixedPurchaseItemNo" Value="<%#: Item.FixedPurchaseItemNo %>" runat="server"/>
						<asp:HiddenField ID="hfProductId" Value="<%#: Item.ProductId %>" runat="server"/>
						<asp:HiddenField ID="hfVariationId" Value="<%#: Item.VariationId %>" runat="server"/>
					</ItemTemplate>
				</asp:Repeater>
			</table>
			<% if (this.DisplayVariationArea && this.IsDisplayMessageAboutChangeVariation) { %>
			<small>
				※バリエーションは次回お届け予定日の <%#: this.ShopShipping.FixedPurchaseCancelDeadline %>日前 まで変更可能です。<br />
			</small>
			<% } %>
			<% if (this.IsDisplayMessageAboutChangeProductOptionValue) { %>
			<small>
				※選択項目は次回お届け予定日の <%#: this.ShopShipping.FixedPurchaseCancelDeadline %>日前 まで変更可能です。<br />
			</small>
			<% } %>
			<div id="dvVariationErrorMessage" class="error" visible="false" runat="server"></div>
			<div id="dvProductOptionValueErrorMessage" class="error" visible="false" runat="server"></div>
			<asp:HiddenField runat="server" ID="hfProductId" />
			<asp:HiddenField runat="server" ID="hfOldVariationId" />
			<asp:HiddenField runat="server" ID="hfSelectedVariationId" />
			<asp:HiddenField runat="server" ID="hfOldSubtotal" />
			<asp:HiddenField runat="server" ID="hfNewSubtotal" />
			<asp:HiddenField runat="server" ID="hfVariationName" />
			<asp:HiddenField runat="server" ID="hfProductOptionTexts" />
		</div>
			
		<div class="right" style="width: 100%; margin-bottom: 20px;" runat="server" Visible="<%# this.IsOrderModifyBtnDisplay %>">
		<div class="right" style="margin-bottom: 20px;">
			<asp:LinkButton id ="btnModifyProducts" CssClass="btn" OnClick="btnModifyProducts_OnClick" runat="server">
				<%# WebSanitizer.HtmlEncode(Constants.FIXEDPURCHASE_PRODUCTCHANGE_ENABLED ? "お届け商品変更" : "お届け商品数変更") %>
			</asp:LinkButton>
		</div>
		</div>
		<div class="dvFixedPurchaseItem" id="dvModifyFixedPurchase" runat="server" visible="false">
			<table cellspacing="0">
				<asp:Repeater ID="rFixedPurchaseModifyProducts" ItemType="FixedPurchaseItemInput" OnItemDataBound="rFixedPurchaseModifyProducts_OnItemDataBound" runat="server">
					<HeaderTemplate>
						<tr>
							<th class="subscriptionBoxProductName" colspan="2">
								商品名
							</th>
							<th class="subscriptionBoxProductPrice" Visible="<%# this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount == false %>" runat="server">
								単価（<%#: this.ProductPriceTextPrefix %>）
							</th>
							<% if (IsDisplayOptionPrice()){ %>
							<th class="productOptionPrice">
								オプション価格（<%#: this.ProductPriceTextPrefix %>）
							</th>
							<% } %>
							<th class="subscriptionBoxOrderCount">
								注文数
							</th>
							<th class="subscriptionBoxOrderSubtotal" Visible="<%# this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount == false %>" runat="server">
								小計（<%#: this.ProductPriceTextPrefix %>）
							</th>
							<th class="subscriptionBoxProductDelete">
								<%# Constants.FIXEDPURCHASE_PRODUCTCHANGE_ENABLED == false ? "削除" : string.Empty %>
							</th>
						</tr>
					</HeaderTemplate>
					<ItemTemplate>
						<tr>
							<td class="productImage">
								<w2c:ProductImage ImageSize="S" IsVariation='<%# Item.HasVariation %>' ProductMaster="<%# GetProduct(Item.ShopId, Item.ProductId, Item.VariationId) %>" runat="server" Visible="True" />
							</td>
							<td class="productName">
								<%# WebSanitizer.HtmlEncode(Item.CreateProductJointName()) %>
								<br />
								[<span class="productId"><%# WebSanitizer.HtmlEncode((Item.ProductId == Item.VariationId) ? Item.ProductId : Item.VariationId) %></span>]
								<span visible='<%# Item.ProductOptionTexts != "" %>' runat="server">
									<br />
									<%# WebSanitizer.HtmlEncode(Item.GetDisplayProductOptionTexts()).Replace("　", "<br />")%>
								</span>
								<div visible='<%# IsDisplayButtonChangeProductOptionValue(Container) %>' style="margin-top: 5px" runat="server">
									<asp:LinkButton Text="選択項目変更" OnClick="lbChangeProductOptionValue_Click" CommandArgument='<%# string.Format("{0},{1}", Container.ItemIndex, Item.ProductOptionTexts) %>' Enabled="<%# this.BeforeCancelDeadline %>" class="btn" ID="lbChangeProductOptionValue" runat="server" />
								</div>
								<div id="dvProductOptionValue" visible="<%# IsDisplayProductOptionValueArea(Container) %>" style="margin: 10px 0" runat="server">
									<!-- ▽ 付帯情報編集欄 ▽ -->
									<asp:Repeater ID="rProductOptionValueSettings" ItemType="w2.App.Common.Product.ProductOptionSetting" DataSource="<%# GetProductOptionSettingList(Item) %>" runat="server">
										<ItemTemplate>
											<div style="margin-top: 6px;">
												<strong style="display: inline-block; padding: 2px 1px;"><%#: Item.ValueName %><span class="notice" style="color: red;" runat="server" visible="<%# Item.IsNecessary %>">*</span></strong><br/>
												<span style="display: inline-block">
													<asp:DropDownList runat="server" ID="ddlProductOptionValueSetting" DataSource="<%# Item.SettingValuesListItemCollection %>" Visible="<%# Item.IsSelectMenu || Item.IsDropDownPrice%>" SelectedValue="<%# Item.GetDisplayProductOptionSettingSelectedValue() %>" />
													<p class="attention"><asp:Label ID="lblProductOptionDropdownErrorMessage" runat="server" /></p>
													<asp:TextBox ID="tbProductOptionValueSetting" Text="<%# Item.SelectedSettingValueForTextBox %>" Visible="<%# Item.IsTextBox %>" runat="server" />
													<p class="attention"><asp:Label ID = "lblProductOptionErrorMessage" runat="server" /></p>
													<asp:Repeater ID="rCblProductOptionValueSetting" ItemType="System.Web.UI.WebControls.ListItem" DataSource="<%# Item.SettingValuesListItemCollection %>" Visible="<%# Item.IsCheckBox || Item.IsCheckBoxPrice %>" runat="server">
														<ItemTemplate>
															<span title="<%# Item.Text %>" id="ProductOptionCp">
																<asp:CheckBox ID="cbProductOptionValueSetting" Text='<%#  GetAbbreviatedProductOptionTextForDisplay(Item.Text) %>' Checked='<%# Item.Selected %>' runat="server" />
																<asp:HiddenField ID="hfCbOriginalValue" runat="server" Value='<%# Item.Text %>'/>
															</span>
														</ItemTemplate>
													</asp:Repeater>
													<p class="attention"><asp:Label ID="lblProductOptionCheckboxErrorMessage" runat="server" /></p>
												</span>
											</div>
										</ItemTemplate>
									</asp:Repeater>
									<!-- △ 付帯情報編集欄 △ -->
									<div style="margin-top: 5px" runat="server">
										<asp:HiddenField ID="hfProductOptionPrice" Value="<%# Item.OptionPrice %>" runat="server"/>
										<asp:LinkButton ID="lbProductOptionValueUpdate" Text="更新" OnClientClick="return AlertProductOptionValueChange()" CommandArgument="<%# Item.ProductId %>" OnClick="lbInputProductOptionValueUpdate_Click" CssClass="btn" runat="server" />
										<asp:LinkButton ID="lbProductOptionValueCancel" Text="キャンセル" OnClick="lbInputProductOptionValueCancel_Click" CssClass="btn" runat="server" />
									</div>
								</div>
							</td>
							<td class="productPrice" runat="server">
								<span ID="sProductPrice" runat="server"><%#: CurrencyManager.ToPrice(Item.GetValidPrice()) %></span>
							</td>
							<% if (IsDisplayOptionPrice()){ %>
								<td class="productPrice" Visible="<%# this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount == false %>" runat="server">
									<span ID="oProductPrice" runat="server">
										<%#: Item.ProductOptionSettingsWithSelectedValues.HasOptionPrice == false ? "―" : CurrencyManager.ToPrice(Item.OptionPrice) %>
									</span>
								</td>
							<% } %>
							<td class="orderCount">
								<asp:DropDownList ID="ddlQuantityUpdate" runat="server" DataTextField="Text" DataValueField="Value" OnTextChanged="ProductModifyReCalculation" AutoPostBack="true" />
							</td>
							<td class="orderSubtotal" Visible="<%# this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount == false %>" runat="server">
								<span ID="sOrderSubtotal" runat="server">
									<%#: CurrencyManager.ToPrice(Item.ItemPriceIncludedOptionPrice) %>
								</span>
							</td>
							<td class="delete" visible="<%# (Constants.FIXEDPURCHASE_PRODUCTCHANGE_ENABLED == false) || (Item.HasProductChangeSetting == false) %>" runat="server">
								<span visible="<%# Constants.FIXEDPURCHASE_PRODUCTCHANGE_ENABLED %>" runat="server">削除</span>
							</td>
							<td class="delete">
								<asp:CheckBox ID="cbDeleteProduct" AutoPostBack="true" OnCheckedChanged="cbDeleteProduct_CheckedChanged" Checked="<%# Item.ModifyDeleteTarget %>" runat="server" />
							</td>
							<td Visible="<%# Constants.FIXEDPURCHASE_PRODUCTCHANGE_ENABLED && Item.HasProductChangeSetting %>" runat="server">
								<asp:Button ID="btnFixedPurchaseProductChange" CssClass="btn" Text="変更" OnClick="btnFixedPurchaseProductChange_OnClick" CommandArgument="<%# Item.ProductId %>" runat="server" />
							</td>
						</tr>
						<asp:HiddenField ID="hfProductPrice" Value="<%# Item.GetValidPrice().ToPriceString() %>" runat="server"/>
						<asp:HiddenField ID="hfOrderSubtotal" Value="<%# Item.ItemPriceIncludedOptionPrice.ToPriceString() %>" runat="server"/>
						<asp:HiddenField ID="hfFixedPurchaseItemNo" Value="<%# Item.FixedPurchaseItemNo %>" runat="server"/>
						<asp:HiddenField ID="hfProductId" Value="<%# Item.ProductId %>" runat="server"/>
						<asp:HiddenField ID="hfVariationId" Value="<%# Item.VariationId %>" runat="server"/>
						<asp:HiddenField ID="hfProductOptionTexts" Value="<%# Item.ProductOptionTexts %>" runat="server"/>
						<%-- 商品変更数選択肢（1から設定分表示） --%>
						<asp:HiddenField ID="hfMaxItemQuantity" runat="server" Value="10" />
						<asp:HiddenField ID="hfItemQuantity" runat="server" Value="<%#: Item.ItemQuantity %>" />
					</ItemTemplate>
				</asp:Repeater>
			</table>
			<% if (this.DisplayVariationArea && this.IsDisplayMessageAboutChangeVariation) { %>
			<small>
				※バリエーションは次回お届け予定日の <%#: this.ShopShipping.FixedPurchaseCancelDeadline %>日前 まで変更可能です。<br />
			</small>
			<% } %>
			<% if (this.IsDisplayMessageAboutChangeProductOptionValue) { %>
			<small>
				※選択項目は次回お届け予定日の <%#: this.ShopShipping.FixedPurchaseCancelDeadline %>日前 まで変更可能です。<br />
			</small>
			<% } %>
			<div id="Div1" class="error" visible="false" runat="server"></div>
			<div id="Div2" class="error" visible="false" runat="server"></div>
			<asp:HiddenField runat="server" ID="HiddenField1" />
			<asp:HiddenField runat="server" ID="HiddenField2" />
			<asp:HiddenField runat="server" ID="HiddenField3" />
			<asp:HiddenField runat="server" ID="HiddenField4" />
			<asp:HiddenField runat="server" ID="HiddenField5" />
			<asp:HiddenField runat="server" ID="HiddenField6" />
			<asp:HiddenField runat="server" ID="HiddenField7" />
			<div>
				<small ID="sProductModifyErrorMessage" class="notProductError" runat="server"></small>
			</div>
			<div class="right" style="margin-bottom: 20px;">
				<asp:LinkButton ID="btnModifyAddProduct" Text="  商品追加  "  CssClass="btn" OnClick="btnModifyAddProduct_OnClick" runat="server"/>
				<asp:LinkButton ID="btnModifyConfirm" OnClientClick="return AlertProductModify()" Text="  情報更新  " CssClass="btn" OnClick="btnModifyConfirm_OnClick" runat="server"/>
				<asp:LinkButton ID="btnModifyCancel" Text="  キャンセル  " CssClass="btn" OnClick="btnModifyCancel_OnClick" runat="server"/>
			</div>
		</div>

		<div class="dvPlannedTotalAmountForTheNextOrderWrap clearFix" style='<%# this.FixedPurchaseContainer.IsSubsctriptionBox ? "" : "margin-top:80px;" %>'>
			<div class="dvPlannedTotalAmountForTheNextOrder">
				<div>次回注文時のお支払い予定金額</div>
				<dl class="plannedTotalAmountForTheNextOrder">
					<dt>商品合計</dt>
					<dd>
						<asp:Literal runat="server" ID="lNextProductSubTotal" Text="<%#: CurrencyManager.ToPrice(this.NextShippingFixedPurchaseCart.PriceSubtotal) %>"></asp:Literal>
					</dd>
					<%-- クーポン情報リスト(有効な場合) --%>
					<% if (Constants.W2MP_COUPON_OPTION_ENABLED) { %>
					<dt class="couponUse">クーポン割引額</dt>
					<dd class="couponUse">
						<span>
							<asp:Literal runat="server" ID="lNextCouponName" Text="<%#: GetCouponName(this.NextShippingFixedPurchaseCart) %>"/>
						</span>
						<span>
							<asp:Literal runat="server" ID="lNextOrderCouponUse" Text='<%#: ((this.NextShippingFixedPurchaseCart.UseCouponPriceForProduct > 0) ? "-" : "") + CurrencyManager.ToPrice(this.NextShippingFixedPurchaseCart.UseCouponPriceForProduct) %>'/>
						</span>
					</dd>
					<% } %>
					<%-- ポイント情報リスト(有効な場合) --%>
					<% if (Constants.W2MP_POINT_OPTION_ENABLED) { %>
					<dt class="pointUse">ポイント利用額</dt>
					<dd class="pointUse">
						<span>
							<% if (this.CanUseAllPointFlg && this.FixedPurchaseContainer.IsUseAllPointFlg) { %>
							全ポイント継続利用
							<% } else { %>
								<asp:Literal runat="server" ID="lNextUsePointPrice" Text='<%#: ((this.NextShippingFixedPurchaseCart.UsePointPrice > 0) ? "-" : "") + CurrencyManager.ToPrice(this.NextShippingFixedPurchaseCart.UsePointPrice) %>'/>
							<% } %>
						</span>
					</dd>
					<% } %>
					<%-- 定期購入割引 --%>
					<dt class="fixedPurchaseDiscountUse" runat="server">定期購入割引額</dt>
					<dd class="fixedPurchaseDiscountUse" runat="server">
						<span>
							<asp:Literal runat="server" ID="lNextFixedPurchaseDiscountPrice" Text='<%#: ((this.NextShippingFixedPurchaseCart.FixedPurchaseDiscount > 0) ? "-" : "") + CurrencyManager.ToPrice(this.NextShippingFixedPurchaseCart.FixedPurchaseDiscount) %>'/>
						</span>
					</dd>
					<% if (Constants.W2MP_COUPON_OPTION_ENABLED && this.IsFreeShippingByCoupon) { %>
					<dt class="nextShippingPrice">配送料金</dt>
					<dd class="nextShippingPrice">クーポン利用のため無料</dd>
					<% } %>
					<dl class="orderTotal">
						<dt>総合計(税込)</dt>
						<dd>
							<asp:Literal runat="server" ID="lNextOrderTotal" Text="<%#: CurrencyManager.ToPrice(this.PlannedTotalAmountForNextOrder) %>"/>
						</dd>
						<span>
							<p>※配送料金と決済手数料は次回注文確定時に確定します。</p>
							<p>※表示価格はあくまでも目安であり、実際の価格は次回購入時に各種キャンペーン状況やポイント・クーポンの適用可否によって異なる場合がございます。予めご了承ください。</p>
						</span>
					</dl>
				</dl>
			</div>
		</div>

		<div ID="dvOrderHistoryList" runat="server">
		<h3>定期購入履歴</h3>
		<asp:HiddenField runat="server" ID="hfPagerMaxDispCount" Value="10"/>
		<p>　※履歴には、定期購入した商品だけではなく一緒に注文した商品も表示されます。</p>
		<asp:Repeater ID="rOrderList" ItemType="" Runat="server">
			<HeaderTemplate>
				<%-- ページャ --%>
				<div id="pagination" class="above clearFix"><%= this.PagerHtml %></div>
			</HeaderTemplate>
			<ItemTemplate>
				<div class="orderList">
					<a href='<%#: Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL + "?" + Constants.REQUEST_KEY_ORDER_ID + "=" + HttpUtility.UrlEncode((string)Eval(Constants.FIELD_ORDER_ORDER_ID)) %>'>
					<tr class="orderBtr">
						<table class="orderHistoryList" cellspacing="0">
							<th class="orderNum">
								<a href='<%#: Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL + "?" + Constants.REQUEST_KEY_ORDER_ID + "=" + HttpUtility.UrlEncode((string)Eval(Constants.FIELD_ORDER_ORDER_ID)) %>'></a>
								ご注文番号</th>
							<th class="orderDate">
								ご購入日</th>
							<th class="paymentTotal">
								総合計</th>
							<th class="orderStatus" >
								ご注文状況</th>
							<% if (Constants.PAYMENT_GMO_POST_ENABLED) { %>
								<th class='creditStatus' runat="server" visible="<%# ((Eval(Constants.FIELD_ORDER_ORDER_PAYMENT_KBN).ToString() == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO) || (Eval(Constants.FIELD_ORDER_ORDER_PAYMENT_KBN).ToString() == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)) %>">
									与信状況
								</th>
							<% } %>
							<tbody class="orderContents">
								<td class="orderNum" >
									<a href='<%#: Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL + "?" + Constants.REQUEST_KEY_ORDER_ID + "=" + HttpUtility.UrlEncode((string)Eval(Constants.FIELD_ORDER_ORDER_ID)) %>'></a>
									<%#: Eval(Constants.FIELD_ORDER_ORDER_ID) %>
								</td>
								<td class="orderDate">
									<%#: DateTimeUtility.ToStringFromRegion(Eval(Constants.FIELD_ORDER_ORDER_DATE), DateTimeUtility.FormatType.ShortDate2Letter) %>
								</td>
								<td class="paymentTotal">
									<%#: CurrencyManager.ToPrice(Eval(Constants.FIELD_ORDER_ORDER_PRICE_TOTAL)) %>
								</td>
								<td class="orderStatus">
									<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_STATUS, Eval(Constants.FIELD_ORDER_ORDER_STATUS)) %><%#: (string)Eval(Constants.FIELD_ORDER_SHIPPED_CHANGED_KBN) == Constants.FLG_ORDER_SHIPPED_CHANGED_KBN_CHANAGED ? "（変更有り）" : "" %>
								</td>
								<% if (Constants.PAYMENT_GMO_POST_ENABLED) { %>
									<!--show credit status if using GMO-->
									<td class='creditStatus' runat="server" visible="<%# ((Eval(Constants.FIELD_ORDER_ORDER_PAYMENT_KBN).ToString() == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO) || (Eval(Constants.FIELD_ORDER_ORDER_PAYMENT_KBN).ToString() == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)) %>">
										<%# ValueText.GetValueText(Constants.TABLE_ORDER,Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS, Eval(Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS)) %>
									</td>
								<% } %>
							</tbody>
						</table>
					</tr>
					<table class="orderHistoryList_secondTable">
						<tr class="orderBtr">
							<th class="shippingDate">
								配送希望日
							</th>
							<% if (this.DisplayScheduledShippingDate == false) { %>
							<th class="emptyColumn" >
							</th>
							<% } %>
							<% if(this.DisplayScheduledShippingDate) { %>
								<th class="scheduledShippingDate" colspan="2">
									出荷予定日
								</th>
							<% } %>
							<% else { %>
							<th class="emptyColumn" >
							</th>
							<% } %>
							<% if (this.DisplayScheduledShippingDate == false) { %>
							<th class="emptyColumn" >
							</th>
							<% } %>
						</tr>
						<tbody class="orderContents">
							<tr class="orderBtr">
								<td class="shippingDate">
									<%# WebSanitizer.HtmlEncodeChangeToBr(GetShippingDate(Eval(Constants.FIELD_ORDER_ORDER_ID).ToString())) %>
								</td>
								<% if (this.DisplayScheduledShippingDate == false) { %>
								<td class="emptyColumn" >
								</td>
								<% } %>
								<% if(this.DisplayScheduledShippingDate) { %>
									<td class="scheduledShippingDate">
										<%#: GetScheduledShippingDate(Eval(Constants.FIELD_ORDER_ORDER_ID).ToString()) %>
									</td>
								<% } %>
								<% else { %>
									<td class="emptyColumn" >
									</td>
								<% } %>
								<% if (this.DisplayScheduledShippingDate == false) { %>
								<td class="emptyColumn" >
								</td>
								<% } %>
							</tr>
						</tbody>
					</table>
					<div class="dvOrderHistoryContain">
						<table class="dvOrderHistoryContain">
							<tr class="orderList">
								<th class="orderName" colspan="2">
									<a href='<%#: Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL + "?" + Constants.REQUEST_KEY_ORDER_ID + "=" + HttpUtility.UrlEncode((string)Eval(Constants.FIELD_ORDER_ORDER_ID)) %>'></a>
									商品名</th>
								<th class="emptyColumn" >
								</th>
								<th colspan="1" class="itemCount">
									注文数</th>
							</tr>
							<asp:Repeater ID="rOrderItemList" DataSource="<%# GetOrderItemModels((string)Eval(Constants.FIELD_ORDER_ORDER_ID)) %>" ItemType="w2.Domain.Order.OrderItemModel" Runat="server">
								<ItemTemplate>
									<tr class="itemTitle">
										<td class="productImage">
											<a href='<%#: Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL + "?" + Constants.REQUEST_KEY_ORDER_ID + "=" + HttpUtility.UrlEncode((string)DataBinder.Eval(((RepeaterItem)Container.Parent.Parent).DataItem, Constants.FIELD_ORDER_ORDER_ID)) %>'></a>
											<div class="itemArea">
												<div class="itemImage">
													<w2c:ProductImage ID="ProductImage" ImageSize="S" ProductMaster="<%# Item.DataSource %>" IsVariation="<%# String.IsNullOrEmpty(Item.VariationId) == false %>" runat="server" Visible="true" />
												</div>
												<td class="productName" colspan="2">
													<div class="itemTitle">
														<p><%#: Item.ProductName %></p>
													</div>
												</td>
											</div>
										</td>
										<td class="orderCount">
											<%#: Item.ItemQuantity %>
										</td>
									</tr>
								</ItemTemplate>
							</asp:Repeater>
						</table>
					</div>
					</a>
				</div>
			</ItemTemplate>
			<FooterTemplate>
				<%-- ページャ--%>
				<div id="pagination" class="below clearFix"><%= this.PagerHtml %></div>
			</FooterTemplate>
		</asp:Repeater>
		</div>
		</div>
		</div>
		<div class="dvUserBtnBox">
			<p><a href="<%# WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_FIXED_PURCHASE_LIST) %>" class="btn btn-large">戻る</a></p>
		</div>
	</div>
</div>

<%-- 頒布会：商品変更（モーダル方式） --%>
<div id="dvModifySubscriptionBoxModalBg" class="modal-bg" Visible="False" runat="server">
	<div id="dvModifySubscriptionBoxModalContent" class="modal-content">
		<iframe src="<%: this.SubscriptionBoxProductChangeModalUrl %>" scrolling="no">
			お使いのブラウザで商品変更画面を表示することができません。
			最新のブラウザをご利用ください。
		</iframe>
	</div>
</div>

<%-- 商品付帯情報：設定変更（モーダル方式） --%>
<div id="dvModifyProductOptionValue" class="modal-bg" Visible="False" runat="server">
	<div id="dvModifyProductOptionValueContent" class="modal-content">
		<asp:Literal ID="lMessage" runat="server"></asp:Literal>
		<asp:LinkButton runat="server" text="更新"></asp:LinkButton>
		<asp:LinkButton runat="server" text="キャンセル"></asp:LinkButton>
	</div>
</div>

<%--▼▼ クレジットカードToken用スクリプト ▼▼--%>
<script type="text/javascript">
	var getTokenAndSetToFormJs = "<%= CreateGetCreditTokenAndSetToFormJsScript().Replace("\"", "\\\"") %>";
	var maskFormsForTokenJs = "<%= CreateMaskFormsForCreditTokenJsScript().Replace("\"", "\\\"") %>";
</script>
<uc:CreditToken runat="server" ID="CreditToken" />
<%--▲▲ クレジットカードToken用スクリプト ▲▲--%>
</ContentTemplate>
</asp:UpdatePanel>
<%-- UpdatePanel終了 --%>
<uc:Loading id="ucLoading" UpdatePanelReload="True" runat="server" />
<%--▼▼ Amazon Pay(CV2)スクリプト ▼▼--%>
<script src="https://static-fe.payments-amazon.com/checkout.js"></script>
<%--▲▲ Amazon Pay(CV2)スクリプト ▲▲--%>
<script type="text/javascript">
	bindEvent();
	scrollToFixedPurchaseItems();

	<%-- UpdataPanelの更新時のみ処理を行う --%>
	function bodyPageLoad() {
		if (Sys.WebForms == null) return;
		var isAsyncPostback = Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();
		if (isAsyncPostback) {
			bindEvent();
			window.exec_submit_flg = 0;
		}
	}

	<%-- イベントをバインドする --%>
	function bindEvent() {
		bindExecAutoKana();
		bindExecAutoChangeKana();
		bindZipCodeSearch();
		<%--▼▼ Amazon Pay(CV2)スクリプト ▼▼--%>
		showAmazonButton('#AmazonPayCv2Button');
		showAmazonButton('#AmazonPayCv2Button2');
		<%--▲▲ Amazon Pay(CV2)スクリプト ▲▲--%>
		<% if(Constants.GLOBAL_OPTION_ENABLE) { %>
		bindTwAddressSearch();
		<% } %>
	}

	<%-- 氏名（姓・名）の自動振り仮名変換のイベントをバインドする --%>
	function bindExecAutoKana() {
		execAutoKanaWithKanaType(
			$("#<%= tbShippingName1.ClientID %>"),
			$("#<%= tbShippingNameKana1.ClientID %>"),
			$("#<%= tbShippingName2.ClientID %>"),
			$("#<%= tbShippingNameKana2.ClientID %>"));
	}

	<%-- ふりがな（姓・名）のかな←→カナ自動変換イベントをバインドする --%>
	function bindExecAutoChangeKana() {
		execAutoChangeKanaWithKanaType(
			$("#<%= tbShippingNameKana1.ClientID %>"),
			$("#<%= tbShippingNameKana2.ClientID %>"));
	}

	<%-- 郵便番号検索のイベントをバインドする --%>
	function bindZipCodeSearch() {
		// Check zip code input on text box change
		clickSearchZipCode(
			'<%= this.WtbShippingZip.ClientID %>',
			'<%= this.WtbShippingZip1.ClientID %>',
			'<%= this.WtbShippingZip2.ClientID %>',
			'<%= this.WlbSearchShippingAddr.ClientID %>',
			'<%= this.WlbSearchShippingAddr.UniqueID %>',
			'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON %>',
			'#addrSearchErrorMessage');

		// Check zip code input on click
		textboxChangeSearchZipCode(
			'<%= this.WtbShippingZip.ClientID %>',
			'<%= this.WtbShippingZip1.ClientID %>',
			'<%= this.WtbShippingZip2.ClientID %>',
			'<%= this.WtbShippingZip1.UniqueID %>',
			'<%= this.WtbShippingZip2.UniqueID %>',
			'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON %>',
			'#addrSearchErrorMessage');

		<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
		// Textbox change search zip global
		textboxChangeSearchGlobalZip(
			'<%= this.WtbShippingZipGlobal.ClientID %>',
			'<%= this.WlbSearchAddrFromShippingZipGlobal.UniqueID %>');
		<% } %>
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

	$(document).on('click', '.search-result-layer-close', function () {
		closePopupAndLayer();
	});

	$(document).on('click', '.search-result-layer-addr', function () {
		bindSelectedAddr($('li.search-result-layer-addr').index(this));
	});

	<%-- 複数住所検索結果からの選択値を入力フォームにバインドする --%>
	function bindSelectedAddr(selectedIndex) {
		var selectedAddr = $('.search-result-layer-addrs li').eq(selectedIndex);
		$("#<%= ddlShippingAddr1.ClientID %>").val(selectedAddr.find('.addr').text());
		$("#<%= tbShippingAddr2.ClientID %>").val(selectedAddr.find('.city').text() + selectedAddr.find('.town').text());
		$("#<%= tbShippingAddr3.ClientID %>").focus();

		closePopupAndLayer();
	}

	<%-- 定期購入再開の確認メッセージを生成 --%>
	function getResumeShippingMesasge() {
		var selectedValue =  $("#<%= ddlResumeFixedPurchaseDate.ClientID %>").val();
		if (selectedValue == "<%= ReplaceTag("@@DispText.shipping_date_list.none@@") %>") {
			return "次回配送日・次々回配送日は自動で計算します。"
		} else {
			return "次々回配送日は自動で計算します。"
		}
	}

	<%-- 頒布会コース再開の確認メッセージを生成 --%>
	function getResumeSubscriptionBoxShippingMesasge() {
		var selectedValue =  $("#<%= ddlResumeSubscriptionBoxDate.ClientID %>").val();
		if (selectedValue == "<%= ReplaceTag("@@DispText.shipping_date_list.none@@") %>") {
			return "次回配送日・次々回配送日は自動で計算します。"
		} else {
			return "次々回配送日は自動で計算します。"
		}
	}

	<% if(Constants.GLOBAL_OPTION_ENABLE) { %>
	<%-- 台湾郵便番号取得関数 --%>
	function bindTwAddressSearch() {
		$('#<%= this.WddlShippingAddr3.ClientID %>').change(function (e) {
			$('#<%= this.WtbShippingZipGlobal.ClientID %>').val(
				$('#<%= this.WddlShippingAddr3.ClientID %>').val().split('|')[0]);
		});
	}
	<% } %>

	<%--領収書情報が削除されるの確認メッセージ作成 --%>
	function getDeleteReceiptInfoMessage() {
		var message = "";

		// 領収書情報が削除される場合、アラート追加
		<% if (Constants.RECEIPT_OPTION_ENABLED && (this.FixedPurchaseContainer.ReceiptFlg == Constants.FLG_ORDER_RECEIPT_FLG_ON)) { %>
		if ("<%= string.Join(",", Constants.NOT_OUTPUT_RECEIPT_PAYMENT_KBN) %>".indexOf(document.getElementById("<%= hfPaymentIdSelected.ClientID %>").value) !== -1)
		{
			message = '\n指定したお支払い方法は、領収書の発行ができません。\n'
				+ '保存されている「領収書情報」が削除されます。\n';
		}
		<% } %>
		return message;
	}

	// Alert Payment Change
	function AlertPaymentChange() {
		var result = false;
		var spanErrorMessageForAtone = document.getElementsByClassName('spanErrorMessageForAtone');
		if ((spanErrorMessageForAtone.length > 0)
			&& (spanErrorMessageForAtone[0].style.display == "block")) {
			return result;
		}

		var spanErrorMessageForAftee = document.getElementsByClassName('spanErrorMessageForAftee');
		if ((spanErrorMessageForAftee.length > 0)
			&& (spanErrorMessageForAftee[0].style.display == "block")) {
			return result;
		}
		if (confirm('お支払い方法を変更します。' + getDeleteReceiptInfoMessage() + '\nよろしいですか？')) {
			doPostbackEvenIfCardAuthFailed = false;
			<% if (Constants.PAYMENT_PAIDY_OPTION_ENABLED) { %>
			result = false;

			PaidyPayProcess();
			<% } else { %>
			result = true;
			<% } %>
		} else {
			doPostbackEvenIfCardAuthFailed = true;
		}

		return result;
	}

	<%-- Check Store Available --%>
	function CheckStoreAvailable() {
		<% if(Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED) { %>
		var isShopValid = false;
		var isShippingConvenience = ($("#<%= WhfCvsShopFlg.ClientID %>").val() == '<%= Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON %>');
		if (isShippingConvenience) {
			var hfCvsShopId = $("#<%= WhfShippingReceivingStoreId.ClientID %>");
			var hfSelectedShopId = $("#<%= WhfSelectedShopId.ClientID %>");
			var shopId = hfCvsShopId.val();
			if (shopId == undefined || shopId == null || shopId == '') {
				shopId = hfSelectedShopId.val();
			}
			var dvErrorShippingConvenience = $("#<%= WdvErrorShippingConvenience.ClientID %>");
			$.ajax({
				type: "POST",
				url: "<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_FIXED_PURCHASE_DETAIL %>/CheckStoreIdValid",
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

			if (isShopValid == false) return isShopValid;
		}
		<% } %>

		return confirm('お届け先情報を変更してもよろしいですか？');
	}

	<% if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED) { %>
	<%-- Open convenience store map popup --%>
	function openConvenienceStoreMapPopup() {
		$("#<%= WdvErrorShippingConvenience.ClientID %>").css("display", "none");
		var url = '<%= OrderCommon.CreateConvenienceStoreMapUrl() %>';
		window.open(url, "", "width=1000,height=800");
	}

	<%-- Set convenience store data --%>
	function setConvenienceStoreData(cvsspot, name, addr, tel) {
		var elements = document.getElementsByClassName("shipping_convenience")[0];
		if (elements == undefined || elements == null) {
			var seletedShippingNo = $('.UserShippingAddress').val();
			var className = '.user_addres' + seletedShippingNo;
			elements = $(className)[0];
			var hfSelectedShopId = $("#<%= WhfSelectedShopId.ClientID %>");
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
	<% } %>

	function confirmUpdateNextNextShippingDate() {
		var isChecked = $('[id$="cbUpdateNextNextShippingDate"]').prop('checked');
		var hasCheckBox = JSON.parse('<%= this.WcbUpdateNextNextShippingDate.HasInnerControl %>'.toLowerCase());
		var isSetting = JSON.parse('<%= Constants.FIXEDPURCHASEORDERNOW_NEXT_NEXT_SHIPPING_DATE_UPDATE_DEFAULT %>'.toLowerCase());
		if (hasCheckBox ? isChecked : isSetting){ 
			return confirm('次回配送予定日と次々回配送予定日を変更します。\n本当によろしいでしょうか？');
		}else{ 
			return confirm('次回配送予定日を変更します。\n本当によろしいですか？');
		} 
	}

	//商品変更時の確認フォーム
	function AlertProductChange() {
		var messagePayment;
		var priceTotalOld = document.getElementById("<%= hfOldSubtotal.ClientID %>").value;
		var priceTotalNew = document.getElementById("<%= hfNewSubtotal.ClientID %>").value;
		messagePayment = '商品を「' + document.getElementById("<%= hfVariationName.ClientID %>").value + '」に変更します。';
		if (priceTotalOld != priceTotalNew) {
			messagePayment += '\n変更に伴い、価格が変更されます。\n\n'
				+ '    変更前の価格：' + priceTotalOld + '\n'
				+ '    変更後の価格：' + priceTotalNew + '\n\n';
		}
		messagePayment += 'よろしいですか？\n\n';
		return confirm(messagePayment);
	}

	//商品変更時の確認フォーム
	function AlertProductModify() {
		return confirm("次回お届け商品を変更します。\n\nよろしいですか？");
	}

	//商品付帯情報変更時の確認フォーム
	function AlertProductOptionValueChange()
	{
		const regex = /(\d+)/; // 正規表現パターンを定義する
		var limitCouponUseMinPrice = <%= this.FixedPurchaseContainer.NextShippingUseCouponDetail == null
			? 0
			: this.FixedPurchaseContainer.NextShippingUseCouponDetail.UsablePrice == null
				? 0
				: this.FixedPurchaseContainer.NextShippingUseCouponDetail.UsablePrice %>;
		var limitPointUseMinPrice = <%= this.FixedPurchaseContainer.NextShippingUsePoint > 0 ? Constants.POINT_MINIMUM_PURCHASEPRICE :0 %>;
		var productOptionPrice = 0;
		var productPrice = 0;

		// チェックボックス型の商品付帯価格の合算
		$('[id$=cbProductOptionValueSetting]:checked').each(function () {
			var id = $(this).attr('id');
			var label = $('label[for="' + id + '"]').text();
			var match = label.replace(",", "").match(regex);
			if (match) productOptionPrice = productOptionPrice + Number(match[0]);
		});

		// ドロップダウンリスト型の商品付帯価格の合算
		$('[id$=ddlProductOptionValueSetting] option:selected').each(function () {
			var match = $(this).text().replace(",", "").match(regex);
			if (match) productOptionPrice = productOptionPrice + Number(match[0]);
		});

		var match = $('[id$=hfOrderSubtotal]').val().replace(",", "").match(regex);
		if (match) productPrice = Number(match[0]);

		var disableUseCoupon = "<%= Constants.W2MP_COUPON_OPTION_ENABLED %>" && ((productPrice + productOptionPrice) < limitCouponUseMinPrice);
		var disableUsePoints = "<%= Constants.W2MP_POINT_OPTION_ENABLED %>" && ((productPrice + productOptionPrice) < limitPointUseMinPrice);

		var messageConfirmModify = "";
		if (disableUseCoupon && disableUsePoints) {
			messageConfirmModify = "現在の価格はクーポンの適用外です。\n現在の価格はポイント適用外です。\nOKを押すことで価格が更新され、クーポンとポイントが\nリセットされます";
		}
		else if (disableUseCoupon) {
			messageConfirmModify = "現在の価格はクーポンの適用外です。\nOKを押すことで価格が更新され、ポイントがリ\nセットされます";
		}
		else if (disableUsePoints) {
			messageConfirmModify = "現在の価格はポイントの適用外です。\nOKを押すことで価格が更新され、ポイントがリ\nセットされます";
		} else {
			messageConfirmModify = '選択項目を変更します。\n\nよろしいですか？\n\n';
		}

		return confirm(messageConfirmModify);
	}

	// 頒布会商品変更時の確認フォーム
	function AlertSubscriptionBoxProductChange() {
		var message = '次回配送商品を変更します。 \nよろしいですか？';
		return confirm(message);
	}

	// 商品追加画面からの遷移時、定期アイテム要素までスクロールする
	function scrollToFixedPurchaseItems() {
		if (document.referrer.indexOf("<%= Constants.PAGE_FRONT_FIXED_PURCHASE_PRODUCT_ADD %>") !== -1) {
			var top = $("#<%= dvModifyFixedPurchase.ClientID %>").offset().top;
			$(window).scrollTop(top);
		}
	}
//-->
</script>
<%--▼▼ Paidy用スクリプト ▼▼--%>
<script type="text/javascript">
	var buyer = <%= PaidyUtility.CreatedBuyerDataObjectForPaidyPayment(this.LoginUserId) %>;
	var hfPaidyTokenIdControlId = "<%= this.WhfPaidyTokenId.ClientID %>";
	var hfPaidyPaySelectedControlId = "<%= this.WhfPaidyPaySelected.ClientID %>";
	var updatePaymentUniqueID = "<%= this.WlbUpdatePayment.UniqueID %>";
	var isHistoryPage = true;
</script>
<uc:PaidyCheckoutScript ID="ucPaidyCheckoutScript" runat="server" />
<%--▲▲ Paidy用スクリプト ▲▲--%>
<input type="hidden" id="fraudbuster" name="fraudbuster" />
<script type="text/javascript" src="//cdn.credit.gmo-ab.com/psdatacollector.js"></script>
<uc:RakutenPaymentScript ID="ucRakutenPaymentScript" runat="server" />
</asp:Content>
