<%--
=========================================================================================================
  Module      : 注文方法設定入力画面(UserDefaultOrderSettingInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ page language="C#" masterpagefile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserDefaultOrderSettingInput.aspx.cs" Inherits="Form_User_UserDefaultOrderSettingInput" title="注文方法設定入力ページ"  MaintainScrollPositionOnPostBack="true" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionCvsDef" Src="~/Form/Common/Order/PaymentDescriptionCvsDef.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaypalScriptsForm" Src="~/Form/Common/PayPalScriptsForm.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionPayPal" Src="~/Form/Common/Order/PaymentDescriptionPayPal.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class="user-unit">
	<h2>注文方法の入力</h2>
	<div class="msg">
		<p>既定の注文方法のご変更を希望の方は、下記のフォームに必要事項をご入力の上、<br />「確認する」ボタンをクリックして下さい。</p>
	</div>

	<div style="padding:0 18px">
		<% if (Constants.GIFTORDER_OPTION_ENABLED == false || Constants.GIFTORDER_OPTION_WITH_SHORTENING_GIFT_OPTION_ENABLED) {%>
		<div class="dvDefaultOrderSetting">
			<h3 style="color:#fff;background-color:#000">既定の配送先住所</h3>
				<dl class="user-form">
					<div id="trDefaultShippingName" runat="server">
						<dt>配送先名</dt>
						<dd>
							<asp:DropDownList ID="ddlDefaultShipping" runat="server" Width="100%" OnSelectedIndexChanged="ddlDefaultShipping_SelectedIndexChanged" AutoPostBack="true" />
						</dd>
					</div>
					<div id="trDefaultShippingInfo" runat="server">
						<dt>お届け先</dt>
						<dd>
							<% if (this.IsShippingAddrJp) { %>
							〒<asp:Literal ID="lShippingZip" runat="server"></asp:Literal><br />
							<% } %>
							<asp:Literal ID="lShippingAddr1" runat="server"></asp:Literal>
							<asp:Literal ID="lShippingAddr2" runat="server"></asp:Literal><br />
							<asp:Literal ID="lShippingAddr3" runat="server"></asp:Literal>
							<asp:Literal ID="lShippingAddr4" runat="server"></asp:Literal><br />
							<asp:Literal ID="lShippingAddr5" runat="server"></asp:Literal>
							<% if (this.IsShippingAddrJp == false) { %>
							<asp:Literal ID="lShippingZipGlobal" runat="server"></asp:Literal><br />
							<% } %>
							<asp:Literal ID="lShippingCountryName" runat="server"></asp:Literal><br />
							<asp:Literal ID="lShippingName1" runat="server"></asp:Literal><asp:Literal ID="lShippingName2" runat="server"></asp:Literal>&nbsp;様
							<% if (this.IsShippingAddrJp) { %>
							（<asp:Literal ID="lShippingNameKana1" runat="server"></asp:Literal><asp:Literal ID="lShippingNameKana2" runat="server"></asp:Literal>&nbsp;さま）
							<% } %><br />
							<asp:Literal ID="lShippingTel1" runat="server"></asp:Literal>
							<br />
							<br />
							<span style="font-weight:bold;font-size:11px;">※ユーザー情報を変更した場合、それに合わせて既定のお届け先情報も変更されます。</span><br/>
							<span style="color:red"><asp:Literal ID="lShippingCountryErrorMessage" runat="server"></asp:Literal></span>
						</dd>
					</div>
				</dl>
		</div>
		<%} %>
		<div class="dvDefaultOrderSetting">
			<h3 style="color:#fff;background-color:#000">既定のお支払方法</h3>
			<%-- ▼PayPalログインここから▼ --%>
			<div style="display: <%= this.IsSelectedPaymentPayPal ? "block" : "none"%>; margin-top: 5px;">
				<%if (Constants.PAYPAL_LOGINPAYMENT_ENABLED) {%>
					<%
						ucPaypalScriptsForm.LogoDesign = "Payment";
						ucPaypalScriptsForm.AuthCompleteActionControl = lbPayPalAuthComplete;
					%>
					<uc:PaypalScriptsForm ID="ucPaypalScriptsForm" runat="server" />
					<div id="paypal-button"></div>
					<%if (SessionManager.PayPalCooperationInfo != null) {%>
						<%: (SessionManager.PayPalCooperationInfo != null) ? SessionManager.PayPalCooperationInfo.AccountEMail : "" %> 連携済<br/>
					<%} %>
					<asp:LinkButton ID="lbPayPalAuthComplete" runat="server" OnClick="lbPayPalAuthComplete_Click"></asp:LinkButton>
				<%} %>
			</div>
			<%-- ▲PayPalログインここまで▲ --%>
			<dl class="user-form">
				<div id="trDefaultPayment" runat="server" visible="false">
					<dt>お支払方法</dt>
					<dd><asp:DropDownList ID="ddlDefaultPayment" runat="server" Width="100%" OnSelectedIndexChanged="ddlDefaultPayment_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList></dd>
				</div>

				<%-- CreditCard --%>
				<div id="dvUserCreditCardInfo" runat="server" visible="false" class="dvUserCreditCardInfo">
					<dt>クレジットカード登録名</dt>
					<dd><asp:DropDownList ID="ddlDefaultUserCreditCardName" runat="server" Width="100%" AutoPostBack="true" OnSelectedIndexChanged="ddlDefaultUserCreditCardName_SelectedIndexChanged" ></asp:DropDownList></td>
					</dd>
					<dt>登録カード詳細</dt>
					<dd>
						<%if (OrderCommon.CreditCompanySelectable) {%>
						<ul>
							<li class="itemname">カード会社&nbsp;：<asp:Literal ID="lCardCompanyName" runat="server"></asp:Literal></li>
						</ul>
						<%} %>
						<ul>
							<li class="itemname">カード番号&nbsp;：XXXXXXXXXXXX<asp:Literal ID="lLastFourDigit" runat="server"></asp:Literal></li>
						</ul>
						<ul>
							<li class="itemname">有効期限&nbsp;&nbsp;&nbsp;&nbsp;：<asp:Literal ID="lExpirationMonth" runat="server"></asp:Literal>/<asp:Literal ID="lExpirationYear" runat="server"></asp:Literal> (月/年)</li>
						</ul>
						<ul>
							<li class="itemname">カード名義&nbsp;：<asp:Literal ID="lAuthorName" runat="server"></asp:Literal></li>
						</ul>
					</dd>
				</div>

				<%-- PayPal --%>
				<div id="divPayPalInfo" runat="server" visible="false">
					<tr>
						<th></th>
						<td>
							<%if (Constants.PAYPAL_LOGINPAYMENT_ENABLED) {%>
								<div id="paypal-button"></div>
							<%} %>
							<uc:PaymentDescriptionPayPal runat="server" id="PaymentDescriptionPayPal" />
						</td>
					</tr>
				</div>
				<%-- コンビニ(前払い)：rakuten --%>
				<% if(this.IsSelectedPaymentCvsPre
						&& (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Rakuten)){ %>
					<div>
						<tr>
							<th>支払いコンビニ選択</th>
							<td>
								<asp:DropDownList
									ID="ddlRakutenCvsType"
									DataSource='<%# this.RakutenCvsTypeList %>'
									DataTextField="Text"
									DataValueField="Value"
									CssClass="input_widthC input_border"
									runat="server" />
							</td>
						</tr>
					</div>
				<% } %>
				<%-- コンビニ(前払い)：Zeus --%>
				<% if (this.IsSelectedPaymentCvsPre && OrderCommon.IsPaymentCvsTypeZeus) { %>
					<div>
						<tr>
							<th>支払いコンビニ選択</th>
							<td>
								<asp:DropDownList
									ID="ddlZeusCvsType"
									CssClass="input_widthC input_border"
									runat="server" />
							</td>
						</tr>
					</div>
				<% } %>
				<%-- コンビニ(前払い)：Paygent --%>
				<% if (this.IsSelectedPaymentCvsPre && OrderCommon.IsPaymentCvsTypePaygent) { %>
					<div>
						<tr>
							<th>支払いコンビニ選択</th>
							<td>
								<asp:DropDownList
									ID="ddlPaygentCvsType"
									CssClass="input_widthC input_border"
									runat="server" />
							</td>
						</tr>
					</div>
				<% } %>
			</dl>
		</div>
		<%if (this.IsSelectedPaymentCvsDef){%>
			<uc:PaymentDescriptionCvsDef runat="server" ID="ucPaymentDescriptionCvsDef" />
		<%} %>
		<span style="color:red"><asp:Literal ID="lCreditCardNoErrorMessage" runat="server"></asp:Literal></span>
		<div id="dvCreditCardNoErrorMessageLink" runat="server" visible="false">
			<a href="<%: (Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_CREDITCARD_LIST) %>" style="color:red;text-decoration: underline;">
				<asp:Literal ID="lRegistCreditCardErrorMessage" runat="server"></asp:Literal>
			</a>
			<br />
			<br />
		</div>
		<span style="color:red"><asp:Literal ID="lTryLinkAfterPayErrorMessage" runat="server"></asp:Literal></span>
		<% if (OrderCommon.DisplayTwInvoiceInfo()) { %>
		<br />
		<div class="dvDefaultOrderSetting">
			<h3 style="color:#fff;background-color:#000">既定の電子発票</h3>
			<dl class="user-form">
				<span id="trDefaultInvoiceName" runat="server">
					<dt>電子発票情報名</dt>
					<dd>
						<asp:DropDownList ID="ddlDefaultInvoice" runat="server" Width="100%" OnSelectedIndexChanged="ddlDefaultInvoice_SelectedIndexChanged" AutoPostBack="true" />
					</dd>
				</span>
				<span id="trDefaultInvoiceInfo" runat="server">
					<dt>電子発票</dt>
					<dd>
						<p><asp:Literal ID="lUniformInvoiceInformation" runat="server"></asp:Literal></p>
						<p><asp:Literal ID="lCarryTypeInformation" runat="server"></asp:Literal></p>
						<p><asp:Literal ID="lUniformInvoiceTypeOption1" runat="server"></asp:Literal></p>
						<p><asp:Literal ID="lUniformInvoiceTypeOption2" runat="server"></asp:Literal></p>
					</dd>
				</span>
			</dl>
		</div>
		<% } %>
	</div>

	<br />

	<div class="user-footer">
		<div class="button-next">
			<span><asp:LinkButton ID="lbConfirm"  OnClientClick="return exec_submit();" runat="server" class="btn btn-large btn-inverse" AutoPostBack="true" OnClick="lbConfirm_Click">確認する</asp:LinkButton></span>
		</div>
		<div class="button-prev">
			<span><a href="<%: (Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_DEFAULT_ORDER_SETTING_LIST) %>" class="btn btn-large">戻る</a></span>
		</div>
	</div>
</div>

</asp:Content>
