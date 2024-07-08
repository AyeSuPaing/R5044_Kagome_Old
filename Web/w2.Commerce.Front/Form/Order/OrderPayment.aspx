<%--
=========================================================================================================
  Module      : 注文お支払い方法選択画面(OrderPayment.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/Order/OrderPayment.aspx.cs" Inherits="Form_Order_OrderPayment" Title="支払方法選択ページ" %>
<%-- ▼削除禁止：クレジットカードTokenコントロール▼ --%>
<%@ Register TagPrefix="uc" TagName="CreditToken" Src="~/Form/Common/CreditToken.ascx" %>
<%@ Register TagPrefix="uc" TagName="RakutenCreditCard" Src="~/Form/Common/RakutenCreditCardModal.ascx" %>
<%@ Register TagPrefix="uc" TagName="RakutenPaymentScript" Src="~/Form/Common/RakutenPaymentScript.ascx" %>
<%@ Register Src="~/Form/Common/Order/PaymentDescriptionCvsDef.ascx" TagPrefix="uc" TagName="PaymentDescriptionCvsDef" %>
<%@ Register Src="~/Form/Common/Order/PaymentDescriptionSmsDef.ascx" TagPrefix="uc" TagName="PaymentDescriptionSmsDef" %>
<%-- ▲削除禁止：クレジットカードTokenコントロール▲ --%>
<%@ Import Namespace="w2.Domain.Coupon.Helper" %>
<%@ Register TagPrefix="uc" TagName="Loading" Src="~/Form/Common/Loading.ascx" %>
<%@ Register Src="~/Form/Common/PayPalScriptsForm.ascx" TagPrefix="uc" TagName="PaypalScriptsForm" %>
<%@ Register Src="~/Form/Common/Order/PaymentDescriptionPayPal.ascx" TagPrefix="uc" TagName="PaymentDescriptionPayPal" %>
<%@ Register Src="~/Form/Common/Order/PaymentDescriptionTriLinkAfterPay.ascx" TagPrefix="uc" TagName="PaymentDescriptionTriLinkAfterPay" %>
<%@ Register Src="~/Form/Common/Order/PaidyCheckoutScript.ascx" TagPrefix="uc" TagName="PaidyCheckoutScript" %>
<%@ Register Src="~/Form/Common/Order/PaidyCheckoutControl.ascx" TagPrefix="uc" TagName="PaidyCheckoutControl" %>
<%@ Register Src="~/Form/Common/Order/PaymentDescriptionAtone.ascx" TagPrefix="uc" TagName="PaymentDescriptionAtone" %>
<%@ Register Src="~/Form/Common/Order/PaymentDescriptionNPAfterPay.ascx" TagPrefix="uc" TagName="PaymentDescriptionNPAfterPay" %>
<%@ Register Src="~/Form/Common/Order/PaymentDescriptionLinePay.ascx" TagPrefix="uc" TagName="PaymentDescriptionLinePay" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionPayPay" Src="~/Form/Common/Order/PaymentDescriptionPayPay.ascx" %>

<%@ Import Namespace="w2.App.Common.Order.Payment.Paidy" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="最終更新者" %>

--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<%-- △編集可能領域△ --%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table id="tblLayout">
<tr>
<td>
<%-- ▽レイアウト領域：レフトエリア▽ --%>
<%-- △レイアウト領域△ --%>
</td>
<td>
<div id="divTopArea">
<%-- ▽レイアウト領域：トップエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<p id="CartFlow"><img src="../../Contents/ImagesPkg/order/cart_step02.gif" alt="お支払い方法入力" width="781" height="58" /></p>

<div class="btmbtn above cartstep">
	<h2 class="ttlA">お支払い方法入力</h2>
	<ul>
		<li>
			<a onclick="<%= this.NextOnClick %>" href="<%: this.NextEvent %>" class="btn btn-success" >ご注文内容確認へ</a>
		</li>
	</ul>
</div>

<%-- エラーメッセージ（デフォルト注文方法用） --%>
<span style="color:red;text-align:center;display:block"><asp:Literal ID="lOrderErrorMessage" runat="server"></asp:Literal></span>
<span style="color:red;text-align:left;display:block"><asp:Literal ID="lPaymentErrorMessage" runat="server"></asp:Literal></span>

<div id="CartList">

<%-- 次へイベント用リンクボタン --%>
<asp:LinkButton ID="lbNext" OnClick="lbNext_Click" runat="server"></asp:LinkButton>
<%-- 戻るイベント用リンクボタン --%>
<asp:LinkButton ID="lbBack" OnClick="lbBack_Click" runat="server"></asp:LinkButton>

<%-- UPDATE PANEL開始 --%>
<asp:UpdatePanel ID="upUpdatePanel" runat="server">
<ContentTemplate>
<% if (string.IsNullOrEmpty(this.DispErrorMessage) == false) { %>
<span style="color:red"><%: this.DispErrorMessage %></span>
<% } %>
<%-- ▼PayPalログインここから▼ --%>
<%if (Constants.PAYPAL_LOGINPAYMENT_ENABLED) {%>
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
<%} %>
<%-- ▲PayPalログインここまで▲ --%>
<asp:Repeater id="rCartList" runat="server">
<ItemTemplate>
	<div class="main">
	<div class="submain">

	<%-- ▼お支払い情報▼ --%>
	<div class="column">
	<div id="Div1" visible="<%# (Container.ItemIndex == 0) %>" runat="server">
	<h2><img src="../../Contents/ImagesPkg/order/sttl_cash.gif" alt="お支払い情報" width="95" height="16" /></h2>
	<p class="pdg_bottomA">お支払い方法を選択し以下の内容をご入力ください。<br /><span class="fred">※</span>&nbsp;は必須入力です。</p>
	</div>

	<div class="orderBox">
	<h3>
		カート番号<%# Container.ItemIndex + 1 %><%# WebSanitizer.HtmlEncode(DispCartDecolationString(Container.DataItem, "（ギフト）", "（デジタルコンテンツ）"))%></h3>
	<div class="bottom">
	<div class="list">
	<span style="color:red" runat="server" visible="<%# (string.IsNullOrEmpty(StringUtility.ToEmpty(this.DispLimitedPaymentMessages[Container.ItemIndex])) == false) %>">
		<%# StringUtility.ToEmpty(this.DispLimitedPaymentMessages[Container.ItemIndex]) %>
		<br/>
	</span>
	<asp:CheckBox ID="cbUseSamePaymentAddrAsCart1" visible="<%# (Container.ItemIndex != 0) %>" Checked="<%# ((CartObject)Container.DataItem).Payment.IsSamePaymentAsCart1 %>" Text="カート番号「１」と同じお支払いを指定する" OnCheckedChanged="cbUseSamePaymentAddrAsCart1_OrderPayment_OnCheckedChanged" AutoPostBack="true" CssClass="checkBox" runat="server" />

	<dl class="list">

	<%--▼▼ クレジット Token保持用（カート1と同じ決済の場合） ▼▼--%>
	<asp:HiddenField ID="hfCreditTokenSameAs1" Value="<%# ((CartObject)Container.DataItem).Payment.CreditTokenSameAs1 %>" runat="server" />
	<%--▲▲ クレジット Token保持用（カート1と同じ決済の場合） ▲▲--%>
	<asp:HiddenField ID="hfPaidyTokenId" runat="server" />
	<asp:HiddenField ID="hfPaidyPaySelected" runat="server" />
	<% if(Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_DDL) { %>
		<dt><asp:DropDownList ID="ddlPayment" runat="server" DataSource="<%# this.ValidPayments[Container.ItemIndex] %>" visible="<%# (Container.ItemIndex == 0) %>" ItemType="w2.Domain.Payment.PaymentModel" OnSelectedIndexChanged="rbgPayment_OnCheckedChanged" AutoPostBack="true" DataTextField="PaymentName" DataValueField="PaymentId" /></dt>
	<% } %>
	<asp:Repeater ID="rPayment" runat="server" DataSource="<%# this.ValidPayments[Container.ItemIndex] %>" visible="<%# (Container.ItemIndex == 0) %>" ItemType="w2.Domain.Payment.PaymentModel">
	<ItemTemplate>
		<asp:HiddenField ID="hfShopId" Value='<%# Item.ShopId %>' runat="server" />
		<asp:HiddenField ID="hfPaymentId" Value='<%# Item.PaymentId %>' runat="server" />
		<asp:HiddenField ID="hfPaymentName" Value='<%# Item.PaymentName %>' runat="server" />
		<asp:HiddenField
			ID="hfCreditBincode"
			Value="<%# GetCreditValue(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment, CartPayment.FIELD_CREDIT_BINCODE) %>"
			runat="server" />
		<% if(Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_RB) { %>
		<dt><w2c:RadioButtonGroup ID="rbgPayment" Checked="<%# ((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment.PaymentId == Item.PaymentId %>" GroupName='<%# "Payment_" + ((RepeaterItem)Container.Parent.Parent).ItemIndex %>' Text="<%# WebSanitizer.HtmlEncode(Item.PaymentName) %>" OnCheckedChanged="rbgPayment_OnCheckedChanged" AutoPostBack="true" CssClass="radioBtn" runat="server" /></dt>
		<% } %>

		<%-- クレジット --%>
		<dd id="ddCredit" runat="server">
		<p runat="server" visible="<%# OrderCommon.GetRegistedCreditCardSelectable(this.IsLoggedIn, this.CreditCardList.Count - 1)%>">
		<asp:DropDownList ID="ddlUserCreditCard" runat="server" DataSource="<%# this.CreditCardList %>" SelectedValue="<%# GetListItemValue(this.CreditCardList ,((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment.CreditCardBranchNo) %>" OnSelectedIndexChanged="ddlUserCreditCard_OnSelectedIndexChanged" AutoPostBack="true" DataTextField="text" DataValueField="value" ></asp:DropDownList></p>
		<%-- ▽新規カード▽ --%>
		<div id="divCreditCardInputForm" runat="server" visible="<%# IsNewCreditCard(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment) %>">
		<% if (this.IsCreditCardLinkPayment() == false) { %>
		<%--▼▼ クレジット Token保持用 ▼▼--%>
		<asp:HiddenField ID="hfCreditToken" Value="<%# ((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment.CreditToken %>" runat="server" />
		<%--▲▲ クレジット Token保持用 ▲▲--%>
		<%--▼▼ カード情報取得用 ▼▼--%>
		<input type="hidden" id="hidCinfo" name="hidCinfo" value="<%# CreateGetCardInfoJsScriptForCreditTokenForCart(((RepeaterItem)Container.Parent.Parent), Container) %>" />
		<span id="spanErrorMessageForCreditCard" style="color: red; display: none" runat="server"></span>
		<%--▲▲ カード情報取得用 ▲▲--%>
			
		<%--▼▼ カード情報入力（トークン未取得・利用なし） ▼▼--%>
		<%if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten) { %>
		<!-- Rakuten Card Form -->
		<asp:LinkButton id="lbEditCreditCardNoForRakutenToken" CssClass="lbEditCreditCardNoForRakutenToken" OnClick="lbEditCreditCardNoForRakutenToken_Click" runat="server">再入力</asp:LinkButton>
		<uc:RakutenCreditCard
			IsOrder="true"
			CartIndex="<%# ((RepeaterItem)Container.Parent.Parent).ItemIndex + 1 %>"
			InstallmentCodeList="<%# this.CreditInstallmentsList %>"
			SelectedInstallmentCode = "<%# GetCreditValue(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment, CartPayment.FIELD_CREDIT_INSTALLMENTS_CODE) %>"
			SelectedExpireMonth="<%# GetCreditValue(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment, CartPayment.FIELD_CREDIT_EXPIRE_MONTH) %>"
			SelectedExpireYear="<%# GetCreditValue(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment, CartPayment.FIELD_CREDIT_EXPIRE_YEAR) %>"
			AuthorName="<%# GetCreditValue(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment, CartPayment.FIELD_CREDIT_AUTHOR_NAME) %>"
			CreditCardNo4="<%# ((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment.CreditCardNo4 %>"
			CreditCompany="<%# ((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment.CreditCardCompany %>"
			SecurityCode="<%# ((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment.RakutenCvvToken %>"
			runat="server"
			ID="ucRakutenCreditCard" />
		<% } else { %>
		<div id="divCreditCardNoToken" visible='<%# (HasCreditToken(Container) == false) %>' runat="server">
		<%if (OrderCommon.CreditCompanySelectable) {%>
		<strong>カード会社</strong>
		<p><asp:DropDownList id="ddlCreditCardCompany" runat="server" DataSource="<%# this.CreditCompanyList %>" DataTextField="Text" DataValueField="Value" SelectedValue="<%# GetCreditValue(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment, CartPayment.FIELD_CREDIT_COMPANY) %>" CssClass="input_widthG input_border"></asp:DropDownList></p>
		<%} %>
		<strong>カード番号</strong>&nbsp;<span class="fred">※</span>
		<p>
		<w2c:ExtendedTextBox id="tbCreditCardNo1" Type="tel" runat="server" CssClass="tel" MaxLength="16" Text="<%# GetCreditValue(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment, CartPayment.FIELD_CREDIT_CARD_NO) %>" autocomplete="off"></w2c:ExtendedTextBox><br />
		<small class="fred">
			<asp:CustomValidator ID="cvCreditCardNo1" runat="Server"
			ControlToValidate="tbCreditCardNo1"
			ValidationGroup="OrderPayment"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" />
			<span id="sErrorMessage" style="color :Red" runat="server" />
		</small>
		<small class="fgray">
		カードの表記のとおりご入力ください。<br />
		例：<br />
			1234567890123456（ハイフンなし）
		</small></p>
		<strong>有効期限</strong>
		<p><asp:DropDownList id="ddlCreditExpireMonth" runat="server" DataSource="<%# this.CreditExpireMonth %>" SelectedValue="<%# GetCreditValue(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment, CartPayment.FIELD_CREDIT_EXPIRE_MONTH) %>" CssClass="input_widthA input_border"></asp:DropDownList>&nbsp;&nbsp;
		&nbsp;/&nbsp;
		<asp:DropDownList id="ddlCreditExpireYear" DataSource="<%# this.CreditExpireYear %>" runat="server" SelectedValue="<%# GetCreditValue(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment, CartPayment.FIELD_CREDIT_EXPIRE_YEAR) %>" CssClass="input_border"></asp:DropDownList>&nbsp;&nbsp;(月/年)</p>
		<strong>カード名義人</strong>&nbsp;<span class="fred">※</span>&nbsp;例：「TAROU YAMADA」
		<p><asp:TextBox id="tbCreditAuthorName" runat="server" MaxLength="50" Text="<%# GetCreditValue(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment, CartPayment.FIELD_CREDIT_AUTHOR_NAME) %>" class="input_widthB input_border" autocomplete="off" Type="email" title=""></asp:TextBox><br />
		<small class="fred">
		<asp:CustomValidator ID="cvCreditAuthorName" runat="Server"
			ControlToValidate="tbCreditAuthorName"
			ValidationGroup="OrderPayment"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" />
		</small></p>
		<div visible="<%# OrderCommon.CreditSecurityCodeEnable %>" runat="server">
		<strong>セキュリティコード</strong>&nbsp;<span class="fred">※</span>
		<p><asp:TextBox id="tbCreditSecurityCode" runat="server" MaxLength="4" Text="<%# GetCreditValue(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment, CartPayment.FIELD_CREDIT_SECURITY_CODE) %>" class="input_widthA input_border" autocomplete="off" Type="tel"></asp:TextBox><br />
		<small class="fred">
		<asp:CustomValidator ID="cvCreditSecurityCode" runat="Server"
			ControlToValidate="tbCreditSecurityCode"
			ValidationGroup="OrderPayment"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" />
		</small></p>
		</div>
		</div>
		<% } %>
		<%--▲▲ カード情報入力（トークン未取得・利用なし） ▲▲--%>

		<%--▼▼ カード情報入力（トークン取得済） ▼▼--%>
		<%if (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Rakuten) { %>
		<div id="divCreditCardForTokenAcquired" Visible='<%# HasCreditToken(Container) %>' runat="server">
		<%if (OrderCommon.CreditCompanySelectable) {%>
		<strong>カード会社</strong>
		<p><asp:Literal ID="lCreditCardCompanyNameForTokenAcquired" Text="<%# ((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment.CreditCardCompanyName %>" runat="server"></asp:Literal><br /></p>
		<%} %>
		<strong>カード番号</strong>
		<asp:LinkButton id="lbEditCreditCardNoForToken" OnClick="lbEditCreditCardNoForToken_Click" runat="server">再入力</asp:LinkButton>
		<p>XXXXXXXXXXXX<asp:Literal ID="lLastFourDigitForTokenAcquired" Text="<%# ((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment.CreditCardNo4 %>" runat="server"></asp:Literal><br /></p>
		<strong>有効期限</strong>
		<p><asp:Literal ID="lExpirationMonthForTokenAcquired" Text="<%# ((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment.CreditExpireMonth %>" runat="server"></asp:Literal>
		&nbsp;/&nbsp;
		<asp:Literal ID="lExpirationYearForTokenAcquired" Text="<%# ((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment.CreditExpireYear %>" runat="server"></asp:Literal> (月/年)</p>
		<strong>カード名義人</strong>
		<p><asp:Literal ID="lCreditAuthorNameForTokenAcquired" Text="<%# ((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment.CreditAuthorName %>" runat="server"></asp:Literal><br /></p>
		</div>
		<%--▲▲ カード情報入力（トークン取得済） ▲▲ --%>

		<div id="Div3" visible="<%# OrderCommon.CreditInstallmentsSelectable %>" runat="server">
		<strong>支払い回数</strong>
		<p><asp:DropDownList id="dllCreditInstallments" runat="server" DataSource="<%# this.CreditInstallmentsList %>" DataTextField="Text" DataValueField="Value" SelectedValue="<%# GetCreditValue(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment, CartPayment.FIELD_CREDIT_INSTALLMENTS_CODE) %>" CssClass="input_border"></asp:DropDownList>
		<br/><span class="fgray">※AMEX/DINERSは一括のみとなります。</span></p>
		</div>
		<% } %>
		<% } else { %>
				<div>注文完了後に遷移する外部サイトで<br />
				カード番号を入力してください。</div>
		<% } %>
		<asp:CheckBox ID="cbRegistCreditCard" runat="server" Checked="<%# ((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment.UserCreditCardRegistFlg %>" Visible="<%# OrderCommon.GetCreditCardRegistable(this.IsLoggedIn, this.CreditCardList.Count - 1) %>" Text="登録する" OnCheckedChanged="cbRegistCreditCard_OnCheckedChanged" AutoPostBack="true" />
		<div id="divUserCreditCardName" Visible="<%# (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten) %>" runat="server">
		<p>クレジットカードを保存する場合は、以下をご入力ください。</p>
		<strong>クレジットカード登録名&nbsp;<span class="fred">※</span></strong>
		<p><asp:TextBox ID="tbUserCreditCardName" Text="<%# ((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment.UserCreditCardName %>" MaxLength="100" CssClass="input_widthD input_border" runat="server"></asp:TextBox><br />
		<% if (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Rakuten) { %>
		<small class="fred">
		<asp:CustomValidator ID="cvUserCreditCardName" runat="Server"
			ControlToValidate="tbUserCreditCardName"
			ValidationGroup="OrderPayment"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			ClientValidationFunction="ClientValidate"
			CssClass="error_inline" />
		</small>
		<% } %>
		</p>
		</div>
		</div>
		<%-- △新規カード△ --%>
		<%-- ▽登録済みカード▽ --%>
		<div id="divCreditCardDisp" visible="<%# IsNewCreditCard(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment) == false %>" runat="server">
		<%if (OrderCommon.CreditCompanySelectable) {%>
		<strong>カード会社</strong>
		<p><asp:Literal ID="lCreditCardCompanyName" runat="server"></asp:Literal><br /></p>
		<%} %>
		<strong>カード番号</strong>
		<p>XXXXXXXXXXXX<asp:Literal ID="lLastFourDigit" runat="server"></asp:Literal><br /></p>
		<strong>有効期限</strong>
		<p><asp:Literal ID="lExpirationMonth" runat="server"></asp:Literal>&nbsp;/&nbsp;<asp:Literal ID="lExpirationYear" runat="server"></asp:Literal> (月/年)</p>
		<strong>カード名義人</strong>
		<p><asp:Literal ID="lCreditAuthorName" runat="server"></asp:Literal><br /></p>
		<asp:HiddenField ID="hfCreditCardId" runat="server" />
		<div id="Div10" visible="<%# OrderCommon.CreditInstallmentsSelectable %>" runat="server">
		<strong>支払い回数</strong>
		<p><asp:DropDownList id="dllCreditInstallments2" runat="server" DataSource="<%# this.CreditInstallmentsList %>" DataTextField="Text" DataValueField="Value" SelectedValue="<%# GetCreditValue(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment, CartPayment.FIELD_CREDIT_INSTALLMENTS_CODE) %>" CssClass="input_border"></asp:DropDownList>
		<br/><span class="fgray">※AMEX/DINERSは一括のみとなります。</span>
		</p>
		</div>
		</div>
		<%-- △登録済みカード△ --%>
		</dd>
		
		<%-- コンビニ(前払い) --%>
		<dd id="ddCvsPre" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE) %>" runat="server">
		<%-- コンビニ(前払い)：電算システム --%>
		<div id="Div4" visible="<%# (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Dsk) %>" runat="server">
		<strong>支払いコンビニ選択</strong>
		<p><asp:DropDownList ID="ddlDskCvsType" DataSource='<%# this.CvsTypeList %>' DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetDskConveniType(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment) %>' CssClass="input_widthC input_border" runat="server"></asp:DropDownList></p>
		</div>
		<%-- コンビニ(前払い)：SBPS --%>
		<div visible="<%# (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.SBPS) %>" runat="server">
		<strong>支払いコンビニ選択</strong>
		<p><asp:DropDownList ID="ddlSBPSCvsType" DataSource='<%# this.CvsTypeList %>' DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetSBPSConveniType(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment) %>' CssClass="input_widthC input_border" runat="server"></asp:DropDownList></p>
		</div>
		<%-- コンビニ(前払い)：ヤマトKWC --%>
		<div visible="<%# (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.YamatoKwc) %>" runat="server">
		<strong>支払いコンビニ選択</strong>
		<p><asp:DropDownList ID="ddlYamatoKwcCvsType" DataSource='<%# this.CvsTypeList %>' DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetYamatoKwcConveniType(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment) %>' CssClass="input_widthC input_border" runat="server"></asp:DropDownList></p>
		</div>
		<%-- コンビニ(前払い)：Gmo --%>
		<div visible="<%# (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Gmo) %>" runat="server">
		<strong>支払いコンビニ選択</strong>
		<p><asp:DropDownList ID="ddlGmoCvsType" DataSource='<%# this.CvsTypeList %>' DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetGmoConveniType(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment) %>' CssClass="input_widthC input_border" runat="server"></asp:DropDownList></p>
		</div>
		<%-- コンビニ(前払い)：rakuten --%>
		<div visible="<%# (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Rakuten) %>" runat="server">
			<strong>支払いコンビニ選択</strong>
			<p>
				<asp:DropDownList
					ID="ddlRakutenCvsType"
					DataSource='<%# this.CvsTypeList %>'
					DataTextField="Text"
					DataValueField="Value"
					SelectedValue='<%# GetRakutenConvenienceType(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment) %>'
					CssClass="input_widthC input_border"
					runat="server" />
			</p>
		</div>
		<%-- コンビニ(前払い)：Zeus --%>
		<div visible="<%# OrderCommon.IsPaymentCvsTypeZeus %>" runat="server">
			<strong>支払いコンビニ選択</strong>
			<p>
				<asp:DropDownList
					ID="ddlZeusCvsType"
					DataSource='<%# this.CvsTypeList %>'
					DataTextField="Text"
					DataValueField="Value"
					SelectedValue='<%# GetZeusConvenienceType(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment) %>'
					CssClass="input_widthC input_border"
					runat="server" />
			</p>
		</div>
		<%-- コンビニ(前払い)：Paygent --%>
		<div visible="<%# OrderCommon.IsPaymentCvsTypePaygent %>" runat="server">
			<strong>支払いコンビニ選択</strong>
			<p>
				<asp:DropDownList
					ID="ddlPaygentCvsType"
					DataSource='<%# this.CvsTypeList %>'
					DataTextField="Text"
					DataValueField="Value"
					SelectedValue='<%# GetPaygentConvenienceType(((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem).Payment) %>'
					CssClass="input_widthC input_border"
					runat="server" />
			</p>
		</div>
		</dd>
		<%-- コンビニ(後払い) --%>
		<dd id="ddCvsDef" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF) %>" runat="server">
			<uc:PaymentDescriptionCvsDef runat="server" id="ucPaymentDescriptionCvsDef" />
		</dd>

		<%-- ヤマト後払いSMS認証連携 --%>
		<dd id="ddSmsDef" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_SMS_DEF) %>" runat="server">
			<uc:PaymentDescriptionSmsDef runat="server" id="ucPaymentDescriptionSmsDef" />
		</dd>
		
		<%-- 後付款(TriLink後払い) --%>
		<dd id="ddTriLinkAfterPayPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY) %>" runat="server">
			<uc:PaymentDescriptionTriLinkAfterPay runat="server" id="ucPaymentDescriptionTryLinkAfterPay" />
		</dd>

		<%-- 銀行振込（前払い） --%>
		<dd id="ddBankPre" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_BANK_PRE) %>" runat="server">
		</dd>
		<%-- 銀行振込（後払い） --%>
		<dd id="ddBankDef" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_BANK_DEF) %>" runat="server">
		</dd>

		<%-- 郵便振込（前払い） --%>
		<dd id="ddPostPre" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_POST_PRE) %>" runat="server">
		</dd>
		<%-- 郵便振込（後払い） --%>
		<dd id="ddPostDef" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_POST_DEF) %>" runat="server">
		</dd>

		<%-- ドコモケータイ払い --%>
		<dd id="ddDocomoPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_ORG) %>" runat="server">
		<strong>【注意事項】</strong>
		<ul>
		<li>決済には「i-mode対応」の携帯電話が必要です。</li>
		<li>携帯電話のメールのドメイン指定受信を設定されている方は、必ず「<%= WebSanitizer.HtmlEncode(ShopMessage.GetMessage("ShopMailDomain")) %>」を受信できるように設定してください。</li>
		<li>１回の購入金額が<%: CurrencyManager.ToPrice(10000m) %>を超えてしまう場合はケータイ払いサービスをご利用いただけません。</li>
		<li>「i-mode」はＮＴＴドコモの商権、または登録商標です。</li>
		</ul></dd>
		<%-- S!まとめて支払い --%>
		<dd id="ddSMatometePayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_SMATOMETE_ORG) %>" runat="server">
		</dd>
		<%-- まとめてau支払い --%>
		<dd id="ddAuMatometePayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AUMATOMETE_ORG) %>" runat="server">
		</dd>

		<%-- ソフトバンク・ワイモバイルまとめて支払い(SBPS) --%>
		<dd id="ddSoftBankKeitaiSBPSPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS) %>" runat="server">
		</dd>
		<%-- auかんたん決済(SBPS) --%>
		<dd id="ddAuKantanSBPSPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS) %>" runat="server">
		</dd>
		<%-- ドコモケータイ払い(SBPS) --%>
		<dd id="ddDocomoKeitaiSBPSPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS) %>" runat="server">
		</dd>
		<%-- S!まとめて支払い(SBPS) --%>
		<dd id="ddSMatometeSBPSPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_SMATOMETE_SBPS) %>" runat="server">
		</dd>

		<%-- PayPal(SBPS) --%>
		<dd id="ddPayPalSBPSPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL_SBPS) %>" runat="server">
			PayPal支払い
		</dd>
		
		<%-- リクルートかんたん支払い --%>
		<dd id="ddRecruitSBPSPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_RECRUIT_SBPS) %>" runat="server">
			リクルートかんたん支払い
		</dd>

		<%-- 楽天ペイ(SBPS) --%>
		<dd id="ddRakutenIdSBPSPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS) %>" runat="server">
		</dd>

		<%-- 代金引換 --%>
		<dd id="ddCollect" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT) %>" runat="server">
		</dd>

		<%-- PayPal --%>
		<dd id="ddPayPal" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL) %>" runat="server">
			<%if (SessionManager.PayPalCooperationInfo != null) {%>
				ご利用のPayPal アカウント：<br/>
				<b><%: SessionManager.PayPalCooperationInfo.AccountEMail %></b>
			<%} else {%>
				ご利用にはPayPalログインが必要です。
			<%} %>
			<uc:PaymentDescriptionPayPal runat="server" id="PaymentDescriptionPayPal" />
		</dd>

		<%-- Paidy --%>
		<dd id="ddPaidy" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY) %>" runat="server">
			<uc:PaidyCheckoutControl ID="ucPaidyCheckoutControl" runat="server" />
		</dd>
		<%-- EcPayment --%>
		<dd id="ddEcPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY) %>" runat="server">
			<strong>支払い方法</strong><br />
			<asp:DropDownList id="ddlEcPayment"
				runat="server"
				CssClass="input_border"
				DataSource="<%# ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE) %>"
				SelectedValue="<%# ((this.CartList.Items[0].Payment != null)
					&& (this.CartList.Items[0].Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY))
					? this.CartList.Items[0].Payment.ExternalPaymentType
					: Constants.FLG_PAYMENT_TYPE_ECPAY_CREDIT %>"
				DataTextField="text"
				DataValueField="value"
				AutoPostBack="true"
				OnSelectedIndexChanged="ddlEcPayment_SelectedIndexChanged" />
				<br />
				<asp:CheckBox ID="cbEcPayCreditInstallment"
					runat="server"
					Checked="<%# ((this.CartList.Items[0].Payment != null) && this.CartList.Items[0].Payment.IsPaymentEcPayWithCreditInstallment) %>"
					Visible="<%# IsDisplayCreditInstallment(this.CartList.Items[0].Payment) && (string.IsNullOrEmpty(Constants.ECPAY_PAYMENT_CREDIT_INSTALLMENT) == false) %>"
					Text="分割払い" />
		</dd>
		<%-- NewebPay Payment --%>
		<dd id="ddNewebPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY) %>" runat="server">
			<strong>支払い方法<br /></strong>
			<asp:DropDownList id="ddlNewebPayment"
				runat="server"
				CssClass="input_border"
				DataSource='<%# ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE + "_neweb") %>'
				SelectedValue="<%# ((this.CartList.Items[0].Payment != null)
					&& (this.CartList.Items[0].Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY))
						? this.CartList.Items[0].Payment.ExternalPaymentType
						: Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT %>"
				DataTextField="text"
				DataValueField="value"
				AutoPostBack="true"
				OnSelectedIndexChanged="ddlNewebPayment_SelectedIndexChanged" />
			<div id="dvCreditInstallment" visible="<%# IsDisplayCreditInstallment(this.CartList.Items[0].Payment) %>" runat="server">
			<strong>支払い回数<br /></strong>
			<asp:DropDownList id="ddlCreditInstallment"
				runat="server"
				DataSource="<%# this.NewebPayInstallmentsList %>"
				SelectedValue="<%# ((this.CartList.Items[0].Payment != null)
					&& (string.IsNullOrEmpty(this.CartList.Items[0].Payment.NewebPayCreditInstallmentsCode) == false))
						? this.CartList.Items[0].Payment.NewebPayCreditInstallmentsCode
						: Constants.FLG_ORDER_CARD_INSTALLMENTS_CODE_ONCE %>"
				DataTextField="Text"
				DataValueField="Value"
				CssClass="input_border" />
			</div>
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
		<dd id="ddAtonePayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE) %>" runat="server">
			<uc:PaymentDescriptionAtone runat="server" id="PaymentDescriptionAtone" />
		</dd>
	
		<%-- aftee翌月払い --%>
		<dd id="ddAfteePayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE) %>" runat="server">
		</dd>

		<%-- （DSK）後払い --%>
		<dd id="ddDskDef" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_DSK_DEF) %>" runat="server">
			コンビニ後払い（DSK）
		</dd>
		<%-- ID決済(wechatpay、aripay、キャリア決済) --%>
		<dd id="ddCarrierbillingBokuPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU) %>" runat="server">
			ID決済(wechatpay、aripay、キャリア決済)
		</dd>
		
		<%-- GMOアトカラ --%>
		<dd id="ddGmoAtokara" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA) %>" runat="server">
			アトカラ
		</dd>
	</ItemTemplate>
	</asp:Repeater>
	</dl>
	
	<div>
	<small id="sErrorMessage2" class="fred" EnableViewState="False" runat="server"></small>
	</div>
	
	</div><!--list-->
	</div><!--bottom-->
	</div><!--orderBox-->
	<%-- ▲お支払い情報▲ --%>
	<%-- ▼領収書情報▼ --%>
	<% if (Constants.RECEIPT_OPTION_ENABLED) { %>
	<div class="columnRight" id="divReceipt">
	<div class="orderBox">
		<h3>カート番号<%# Container.ItemIndex + 1 %>の領収書情報</h3>
		<div class="bottom">
		<div id="divDisplayCanNotInputMessage" runat="server" visible="false" class="userList fred">指定したお支払い方法は、領収書の発行ができません。</div>
		<div id="divReceiptInfoInputForm" runat="server" class="userList">
			<strong>領収書希望有無を選択してください。</strong>
			<dd><asp:DropDownList id="ddlReceiptFlg" runat="server" DataTextField="text" DataValueField="value" DataSource="<%# this.DdlReceiptFlgListItems[Container.ItemIndex] %>"
				SelectedValue="<%# GetSelectedValueOfReceiptFlg(Container.ItemIndex, ((CartObject)Container.DataItem).IsUseSameReceiptInfoAsCart1, ((CartObject)Container.DataItem).ReceiptFlg) %>"
				OnSelectedIndexChanged="ddlReceiptFlg_OnSelectedIndexChanged" AutoPostBack="true" />
			</dd>
			<div id="divReceiptAddressProviso" runat="server">
			<dt>宛名<span class="fred">※</span></dt>
			<dd>
				<asp:TextBox id="tbReceiptAddress" runat="server" Text="<%# ((CartObject)Container.DataItem).ReceiptAddress %>" MaxLength="100" CssClass="input_widthD" />
				<asp:CustomValidator ID="cvReceiptAddress" runat="Server"
					ControlToValidate="tbReceiptAddress"
					ValidationGroup="ReceiptRegisterModify"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"/>
			</dd>
			<dt>但し書き<span class="fred">※</span></dt>
			<dd class="last">
				<asp:TextBox id="tbReceiptProviso" runat="server" Text="<%# ((CartObject)Container.DataItem).ReceiptProviso %>" MaxLength="100" CssClass="input_widthD" />
				<asp:CustomValidator ID="cvReceiptProviso" runat="Server"
					ControlToValidate="tbReceiptProviso"
					ValidationGroup="ReceiptRegisterModify"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"/>
			</dd>
			</div><!--divReceiptAddressProviso-->
		</div><!--divReceiptInfoInputForm-->
		</div><!--bottom-->
	</div><!--orderBox-->
	</div><!--divReceipt-->
	<% } %>
	<%-- ▲領収書情報▲ --%>
	</div><!--column-->

	<%-- ▼カート情報▼ --%>
	<div class="shoppingCart">
	<div id="Div7" visible="<%# (Container.ItemIndex == 0) %>" runat="server">
	<h2><img src="../../Contents/ImagesPkg/common/ttl_shopping_cart.gif" alt="ショッピングカート" width="141" height="16" /></h2>
	<div class="sumBox mrg_topA">
	<div class="subSumBoxB">
	<p><img src="../../Contents/ImagesPkg/common/ttl_sum.gif" alt="総合計" width="52" height="16" /><strong><%#: CurrencyManager.ToPrice(this.CartList.PriceCartListTotalWithOutPaymentPrice) %></strong></p>
	</div>
	</div><!--sum-->
	</div>

	<div class="subCartList">
	<div class="bottom">
	<h3>
		<div class="cartNo">
			カート番号<%# Container.ItemIndex + 1 %><%# WebSanitizer.HtmlEncode(DispCartDecolationString(Container.DataItem, "（ギフト）", "（デジタルコンテンツ）"))%></div>
		<div class="cartLink"><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST) %>">カートへ戻る</a></div></h3>
	<div class="block">

	<asp:Repeater ID="rCart" DataSource="<%# ((CartObject)Container.DataItem).Items %>" runat="server">
	<ItemTemplate>
		<%-- 通常商品 --%>
		<div class="singleProduct" visible="<%# ((CartProduct)Container.DataItem).IsSetItem == false && ((CartProduct)Container.DataItem).QuantitiyUnallocatedToSet != 0 %>" runat="server">
		<div>
		<dl>
		<dt>
			<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
				<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" /></a>
			<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false %>" />
		</dt>
		<dd>
			<strong>
				<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
					<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %></a>
				<%# (((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false) ? WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) : "" %>
			</strong>
			<%# (((CartProduct)Container.DataItem).GetProductTag("tag_cart_product_message").Length != 0) ? "<small>" + WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).GetProductTag("tag_cart_product_message")) + "</small>" : "" %>
		<p visible='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList.IsSelectedProductOptionValueAll %>' runat="server">
		<asp:Repeater ID="rProductOptionSettings" DataSource='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList %>' runat="server">
			<ItemTemplate>
			<%# (((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue() != "") ? "<strong>" : "" %>
			<%# WebSanitizer.HtmlEncode(((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue()) %>
			<%# (((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue() != "") ? "</strong>" : "" %>
			</ItemTemplate>
		</asp:Repeater>
		</p>
		<p>数量：&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).QuantitiyUnallocatedToSet) %></p>
		<p Visible="<%# ((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false %>" runat="server"><%#: ProductOptionSettingHelper.ToDisplayProductOptionPrice((CartProduct)Container.DataItem) %> (<%#: this.ProductPriceTextPrefix %>)</p></dd>
		<p style="color:red;" visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
			※配送料無料適用外商品です
		</p>
		</dl>
		</div>
		</div><!--singleProduct-->
		<%-- セット商品 --%>
		<div id="Div9" class="multiProduct" visible="<%# (((CartProduct)Container.DataItem).IsSetItem) && (((CartProduct)Container.DataItem).ProductSetItemNo == 1) %>" runat="server">
		<asp:Repeater id="rProductSet" DataSource="<%# (((CartProduct)Container.DataItem).ProductSet != null) ? ((CartProduct)Container.DataItem).ProductSet.Items : null %>" runat="server">
		<ItemTemplate>
			<div>
			<dl>
			<dt>
				<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
					<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" /></a>
				<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false %>" />
			</dt>
			<dd>
				<strong>
					<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
						<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %></a>
					<%# (((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false) ? WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) : "" %>
				</strong>
				<%# (((CartProduct)Container.DataItem).GetProductTag("tag_cart_product_message").Length != 0) ? "<small>" + WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).GetProductTag("tag_cart_product_message")) + "</small>" : "" %>
			<p Visible="<%# ((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false %>" runat="server"><%#: CurrencyManager.ToPrice(((CartProduct)Container.DataItem).Price) %> (<%#: this.ProductPriceTextPrefix %>)&nbsp;&nbsp;x&nbsp;&nbsp;<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).CountSingle) %></p></dd>
			<p style="color:red;" visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
				※配送料無料適用外商品です
			</p>
			</dl>
			</div>
			<table id="Table1" visible="<%# (((CartProduct)Container.DataItem).ProductSetItemNo == ((CartProduct)Container.DataItem).ProductSet.Items.Count) %>" width="297" cellpadding="0" cellspacing="0" class="clr" runat="server">
			<tr>
			<th width="38">セット：</th>
			<th width="50"><%# GetProductSetCount((CartProduct)Container.DataItem) %></th>
			<th width="146" Visible="<%# ((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false %>" runat="server"><%#: CurrencyManager.ToPrice(GetProductSetPriceSubtotal((CartProduct)Container.DataItem)) %> (<%#: this.ProductPriceTextPrefix %>)</th>
			<td width="61"></td>
			</tr>
			</table>
		</ItemTemplate>
		</asp:Repeater>
		</div><!--multiProduct-->
	</ItemTemplate>
	</asp:Repeater>
	<%-- セットプロモーション商品 --%>
	<asp:Repeater ID="rCartSetPromotion" DataSource="<%# ((CartObject)Container.DataItem).SetPromotions %>" runat="server">
	<ItemTemplate>
		<div class="multiProduct">
			<asp:Repeater ID="rCartSetPromotionItem" DataSource="<%# ((CartSetPromotion)Container.DataItem).Items %>" runat="server">
			<ItemTemplate>
				<div>
					<dl>
						<dt>
							<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
								<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" /></a>
							<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false %>" />
						</dt>
						<dd>
							<strong>
								<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
									<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %></a>
								<%# (((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false) ? WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) : "" %>
							</strong>
							<p visible='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList.IsSelectedProductOptionValueAll %>' runat="server">
							<asp:Repeater ID="rProductOptionSettings" DataSource='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList %>' runat="server">
								<ItemTemplate>
								<%# (((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue() != "") ? "<strong>" : "" %>
								<%# WebSanitizer.HtmlEncode(((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue()) %>
								<%# (((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue() != "") ? "</strong>" : "" %>
								</ItemTemplate>
							</asp:Repeater>
							</p>
							<p>数量：&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).QuantityAllocatedToSet[((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).CartSetPromotionNo]) %></p>
							<p Visible="<%# ((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false %>" runat="server"><%#: ProductOptionSettingHelper.ToDisplayProductOptionPrice((CartProduct)Container.DataItem) %> (<%#: this.ProductPriceTextPrefix %>)</p>
							<p style="color:red;" visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
								※配送料無料適用外商品です
							</p>
						</dd>
					</dl>
				</div>
			</ItemTemplate>
			</asp:Repeater>
			<dl class="setpromotion" Visible="<%# ((CartSetPromotion)Container.DataItem).Items[0].IsSubscriptionBoxFixedAmount() == false %>" runat="server">
				<dt><%# WebSanitizer.HtmlEncode(((CartSetPromotion)Container.DataItem).SetpromotionDispName) %></dt>
				<dd>
					<span id="Span3" visible="<%# ((CartSetPromotion)Container.DataItem).IsDiscountTypeProductDiscount %>" runat="server">
						<strike><%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).UndiscountedProductSubtotal) %> (税込)</strike><br />
					</span>
					<%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).UndiscountedProductSubtotal - ((CartSetPromotion)Container.DataItem).ProductDiscountAmount) %> (税込)
				</dd>
			</dl>
		</div>
	</ItemTemplate>
	</asp:Repeater>

	<% if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsLoggedIn) { %>
	<div class="pointBox" >
	<asp:LinkButton ID="lbUsePoint" Visible="<%# CanUsePointForPurchase(Container, false) %>" OnClick="lbUsePoint_Click" runat="server">ポイントを使う</asp:LinkButton>
	<div class="box" id="divPointBox" visible="<%# CanUsePointForPurchase(Container) %>" runat="server">
	<p><img src="../../Contents/ImagesPkg/common/ttl_point.gif" alt="ポイントを使う" width="262" height="23" /></p>
	<div class="boxbtm">
	<div>
	<dl>
	<dt>合計 <%= GetNumeric(this.LoginUserPointUsable) %> ポイント<span>までご利用いただけます</span></dt>
	<dd><asp:TextBox ID="tbOrderPointUse" Runat="server" Text="<%# GetUsePoint((CartObject)Container.DataItem) %>" MaxLength="6"></asp:TextBox>&nbsp;&nbsp;pt</dd>
	</dl>
	<br/><br/>
	<% if (this.CanUseAllPointFlg && this.IsLoggedIn) { %>
	<asp:CheckBox ID="CheckBox1" Text="定期注文で利用可能なポイント<br>すべてを継続使用する" Visible="<%# ((CartObject)Container.DataItem).HasFixedPurchase %>"
		OnCheckedChanged="cbUseAllPointFlg_Changed" OnDataBinding="cbUseAllPointFlg_DataBinding"
		CssClass="cbUseAllPointFlg" Style="margin-left: 1.4em; text-indent: -1.6em;" AutoPostBack="True" runat="server"/>
	<span Visible="<%# ((CartObject)Container.DataItem).HasFixedPurchase %>" runat="server">※注文後はマイページ＞定期購入情報より<br/>変更できます。</span>
	<% } %>
		<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
	</div>
	<span id="Span1" class="fred" visible="<%# this.ErrorMessages.HasMessages(Container.ItemIndex, CartErrorMessages.ErrorKbn.Point) %>" runat="server">
		<%# WebSanitizer.HtmlEncode(this.ErrorMessages.Get(Container.ItemIndex, CartErrorMessages.ErrorKbn.Point))%></span>
	</div><!--boxbtm-->
	</div><!--box-->
		<div runat="server" visible="<%# (((CartObject)Container.DataItem).CanUsePointForPurchase == false) %>">
			<p>
				あと「<%#: GetPriceCanPurchaseUsePoint(((CartObject)Container.DataItem).PurchasePriceTotal) %>」の購入でポイントをご利用いただけます。
			</p>
		</div>
	</div><!--pointBox-->
	<% } %>

	<% if (Constants.W2MP_COUPON_OPTION_ENABLED) { %>
	<div class="couponBox">
	<asp:LinkButton ID="lbUseCoupon" Visible="<%# this.OpenCouponInput[Container.ItemIndex] == false %>" OnClick="lbUseCoupon_Click" runat="server">クーポンを使う</asp:LinkButton>
	<div id="divCouponBox" class="box" visible="<%# this.OpenCouponInput[Container.ItemIndex] %>" runat="server">
	<p><img src="../../Contents/ImagesPkg/common/ttl_coupon.gif" alt="クーポンを使う" width="262" height="23" /></p>
	<div id="divCouponInputMethod" runat="server" style="font-size: 10px; padding: 10px 10px 0px 10px; font-family: 'Lucida Grande','メイリオ',Meiryo,'Hiragino Kaku Gothic ProN', sans-serif; color: #333;">
		<asp:RadioButtonList runat="server" AutoPostBack="true" ID="rblCouponInputMethod"
			OnSelectedIndexChanged="rblCouponInputMethod_SelectedIndexChanged" OnDataBinding="rblCouponInputMethod_DataBinding"
			DataSource="<%# GetCouponInputMethod() %>" DataTextField="Text" DataValueField="Value" RepeatColumns="2" RepeatDirection="Horizontal"></asp:RadioButtonList>
	</div>
	<div class="boxbtm">
	<div>
	<div id="hgcCouponSelect" runat="server">
		<asp:DropDownList CssClass="input_border" style="width: 240px" ID="ddlCouponList" runat="server" DataTextField="Text" DataValueField="Value" OnTextChanged="ddlCouponList_TextChanged" AutoPostBack="true"></asp:DropDownList>
	</div>
	<dl id="hgcCouponCodeInputArea" runat="server">
	<dt><span>クーポンコード</span></dt>
	<dd><asp:TextBox ID="tbCouponCode" runat="server" Text="<%# GetCouponCode(((CartObject)Container.DataItem).Coupon) %>" MaxLength="30" autocomplete="off"></asp:TextBox></dd>
	</dl>
	<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
	</div>
	<span class="fred" visible="<%# this.ErrorMessages.HasMessages(Container.ItemIndex, CartErrorMessages.ErrorKbn.Coupon) %>" runat="server">
		<%# WebSanitizer.HtmlEncode(this.ErrorMessages.Get(Container.ItemIndex, CartErrorMessages.ErrorKbn.Coupon)) %></span>
	<asp:LinkButton runat="server" ID="lbShowCouponBox" Text="クーポンBOX"
		style="color: #ffffff !important; background-color: #000 !important;
		border-color: rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.25); text-shadow: 0 -1px 0 rgba(0, 0, 0, 0.25); display: inline-block;
		padding: 4px 10px 4px; margin-bottom: 0; font-size: 13px; line-height: 18px; text-align: center; vertical-align: middle; cursor: pointer;
		border: 1px solid #cccccc; border-radius: 4px; box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.2), 0 1px 2px rgba(0, 0, 0, 0.05); white-space: nowrap; text-decoration: none; "
		OnClick="lbShowCouponBox_Click" ></asp:LinkButton>
	</div><!--boxbtm-->
	</div><!--box-->
	<div runat="server" id="hgcCouponBox" style="z-index: 1; top: 0; left: 0; width: 100%; height: 120%; position: fixed; background-color: rgba(128, 128, 128, 0.75);" 
		Visible='<%# ((CartObject)Container.DataItem).CouponBoxVisible %>'>
		<div id="hgcCouponList" style="width: 800px; height: 500px; top: 50%; left: 50%; text-align: center; border: 2px solid #aaa; background: #fff; position: fixed; z-index: 2; margin:-250px 0 0 -400px;">
		<h2 style="height: 20px; color: #fff; background-color: #000; font-size: 16px; padding: 3px 0px; border-bottom: solid 1px #ccc; width: initial; width: auto; ">クーポンBOX</h2>
		<div style="height: 400px; overflow: auto;">
		<asp:Repeater ID="rCouponList" ItemType="UserCouponDetailInfo" Runat="server" DataSource="<%# GetUsableCoupons((CartObject)Container.DataItem) %>">
		<HeaderTemplate>
			<table>
			<tr>
				<th style="border-bottom-style:solid; border-bottom-width:1px; background-color:#ececec; padding:10px; text-align:center;width:150px;">クーポンコード</th>
				<th style="border-bottom-style:solid; border-bottom-width:1px; background-color:#ececec; padding:10px; text-align:center;width:230px;">クーポン名</th>
				<th style="border-bottom-style:solid; border-bottom-width:1px; background-color:#ececec; padding:10px; text-align:center;width:170px;">割引金額<br />/割引率</th>
				<th style="border-bottom-style:solid; border-bottom-width:1px; background-color:#ececec; padding:10px; text-align:center;width:70px;">利用可能回数</th>
				<th style="border-bottom-style:solid; border-bottom-width:1px; background-color:#ececec; padding:10px; text-align:center;width:350px;">有効期限</th>
				<th style="border-bottom-style:solid; border-bottom-width:1px; background-color:#ececec; padding:10px; text-align:center;width:100px;"></th>
			</tr>
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
				<td style="border-bottom-style:solid; border-bottom-width:1px; padding:10px 8px; text-align:left; text-align:center;width:150px; background-color: white;">
					<%#: StringUtility.ToEmpty(Item.CouponCode) %><br />
					<asp:HiddenField runat="server" ID="hfCouponBoxCouponCode" Value="<%# Item.CouponCode %>" />
				</td>
				<td style="border-bottom-style:solid; border-bottom-width:1px; padding:10px 8px; text-align:left; text-align:center;width:230px; background-color: white;"
					title="<%#: StringUtility.ToEmpty(Item.CouponDispDiscription) %>">
					<%#: StringUtility.ToEmpty(Item.CouponDispName) %>
				</td>
				<td style="border-bottom-style:solid; border-bottom-width:1px; padding:10px 8px; text-align:left; text-align:center;width:70px; background-color: white;">
					<%# WebSanitizer.HtmlEncodeChangeToBr(GetCouponDiscountString(Item)) %>
				</td>
				<td style="border-bottom-style:solid; border-bottom-width:1px; padding:10px 8px; text-align:left; text-align:center;width:70px; background-color: white;">
					<%#: GetCouponCount(Item) %>
				</td>
				<td style="border-bottom-style:solid; border-bottom-width:1px; padding:10px 8px; text-align:left; text-align:center;width:350px; background-color: white;">
					<%#: DateTimeUtility.ToStringFromRegion(Item.ExpireEnd, DateTimeUtility.FormatType.LongDateHourMinute1Letter) %>
				</td>
				<td style="border-bottom-style:solid; border-bottom-width:1px; padding:10px 8px; text-align:left; text-align:center;width:100px; background-color: white;">
					<asp:LinkButton runat="server" id="lbCouponSelect" OnClick="lbCouponSelect_Click" style="color: #ffffff !important; background-color: #000 !important;
						border-color: rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.25); text-shadow: 0 -1px 0 rgba(0, 0, 0, 0.25); display: inline-block;
						padding: 4px 10px 4px; margin-bottom: 0; font-size: 13px; line-height: 18px; text-align: center; vertical-align: middle; cursor: pointer;
						border: 1px solid #cccccc; border-radius: 4px; box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.2), 0 1px 2px rgba(0, 0, 0, 0.05); white-space: nowrap; text-decoration: none; ">このクーポンを使う</asp:LinkButton>
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
			border: 1px solid #cccccc; box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.2), 0 1px 2px rgba(0, 0, 0, 0.05); text-decoration: none; background-image: none; margin: 5px auto; text-decoration: none; ">クーポンを利用しない</asp:LinkButton>
	</div>
	</div>
	</div>
	</div><!--couponBox-->
	<% } %>

	<div class="priceList">
	<div>
	<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
	<dt>小計(<%#: this.ProductPriceTextPrefix %>)</dt>
	<dd><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceSubtotal) %></dd>
	</dl>
	<%if (this.ProductIncludedTaxFlg == false) { %>
		<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
			<dt>消費税額</dt>
			<dd><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceSubtotalTax) %></dd>
		</dl>
	<%} %>
	<%-- セットプロモーション割引額(商品割引) --%>
	<asp:Repeater DataSource="<%# ((CartObject)Container.DataItem).SetPromotions %>" Visible="<%# ((CartObject)Container.DataItem).IsSubscriptionBoxFixedAmount == false %>" runat="server">
	<ItemTemplate>
		<span visible="<%# ((CartSetPromotion)Container.DataItem).IsDiscountTypeProductDiscount %>" runat="server">
		<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
			<dt><%# WebSanitizer.HtmlEncode(((CartSetPromotion)Container.DataItem).SetpromotionDispName) %></dt>
			<dd class='<%# (((CartSetPromotion)Container.DataItem).ProductDiscountAmount > 0) ? "minus" : "" %>'><%# (((CartSetPromotion)Container.DataItem).ProductDiscountAmount > 0) ? "-" : ""%><%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).ProductDiscountAmount) %></dd>
		</dl>
		</span>
	</ItemTemplate>
	</asp:Repeater>
	<%if (Constants.MEMBER_RANK_OPTION_ENABLED && this.IsLoggedIn){ %>
	<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
	<dt>会員ランク割引額</dt>
	<dd class='<%# (((CartObject)Container.DataItem).MemberRankDiscount > 0) ? "minus" : "" %>'><%# (((CartObject)Container.DataItem).MemberRankDiscount > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).MemberRankDiscount * ((((CartObject)Container.DataItem).MemberRankDiscount < 0) ? -1 : 1)) %></dd>
	</dl>
	<%} %>
	<%if (Constants.MEMBER_RANK_OPTION_ENABLED && Constants.FIXEDPURCHASE_OPTION_ENABLED && this.IsLoggedIn){ %>
	<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
	<dt>定期会員割引額</dt>
	<dd class='<%# (((CartObject)Container.DataItem).FixedPurchaseMemberDiscountAmount > 0) ? "minus" : "" %>'><%# (((CartObject)Container.DataItem).FixedPurchaseMemberDiscountAmount > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).FixedPurchaseMemberDiscountAmount * ((((CartObject)Container.DataItem).FixedPurchaseMemberDiscountAmount < 0) ? -1 : 1)) %></dd>
	</dl>
	<%} %>
	<%if (Constants.W2MP_COUPON_OPTION_ENABLED){ %>
	<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
	<dt>クーポン割引額</dt>
	<dd class='<%# (((CartObject)Container.DataItem).UseCouponPrice > 0) ? "minus" : "" %>'>
		<%#: GetCouponName(((CartObject)Container.DataItem)) %>
		<%# (((CartObject)Container.DataItem).UseCouponPrice > 0) ? "-" : "" %>
		<%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).UseCouponPrice * ((((CartObject)Container.DataItem).UseCouponPrice < 0) ? -1 : 1)) %>
	</dd>
	</dl>
	<%} %>
	<%if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsLoggedIn){ %>
	<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
	<dt>ポイント利用額</dt>
	<dd class='<%# (((CartObject)Container.DataItem).UsePointPrice > 0) ? "minus" : "" %>'><%# (((CartObject)Container.DataItem).UsePointPrice > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).UsePointPrice * ((((CartObject)Container.DataItem).UsePointPrice < 0) ? -1 : 1)) %></dd>
	</dl>
	<%} %>
	<%if (Constants.FIXEDPURCHASE_OPTION_ENABLED){ %>
	<div runat="server" visible="<%# (((CartObject)Container.DataItem).HasFixedPurchase) %>">
	<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
	<dt>定期購入割引額</dt>
	<dd class='<%# (((CartObject)Container.DataItem).FixedPurchaseDiscount > 0) ? "minus" : "" %>'><%#: (((CartObject)Container.DataItem).FixedPurchaseDiscount > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).FixedPurchaseDiscount * ((((CartObject)Container.DataItem).FixedPurchaseDiscount < 0) ? -1 : 1)) %></dd>
	</dl>
	</div>
	<%} %>
	<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
	<dt>配送料金</dt>
	<dd runat="server" style='<%# (((CartObject)Container.DataItem).ShippingPriceSeparateEstimateFlg) ? "display:none;" : ""%>'>
		<%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceShipping) %></dd>
	<dd runat="server" style='<%# (((CartObject)Container.DataItem).ShippingPriceSeparateEstimateFlg == false) ? "display:none;" : ""%>'>
		<%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).ShippingPriceSeparateEstimateMessage)%></dd>
	<small style="color:red;" visible="<%# ((CartObject)Container.DataItem).IsDisplayFreeShiipingFeeText %>" runat="server">
		※配送料無料適用外の商品が含まれるため、<%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceShipping) %>の配送料が請求されます
	</small>
	</dl>
	<%-- セットプロモーション割引額(配送料割引) --%>
	<asp:Repeater DataSource="<%# ((CartObject)Container.DataItem).SetPromotions %>" runat="server">
	<ItemTemplate>
		<span visible="<%# ((CartSetPromotion)Container.DataItem).IsDiscountTypeShippingChargeFree %>" runat="server">
		<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
			<dt><%# WebSanitizer.HtmlEncode(((CartSetPromotion)Container.DataItem).SetpromotionDispName) %>(送料割引)</dt>
			<dd class='<%# (((CartSetPromotion)Container.DataItem).ShippingChargeDiscountAmount > 0) ? "minus" : "" %>'><%# (((CartSetPromotion)Container.DataItem).ShippingChargeDiscountAmount > 0) ? "-" : ""%><%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).ShippingChargeDiscountAmount) %></dd>
		</dl>
		</span>
	</ItemTemplate>
	</asp:Repeater>
	</div>
	<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
	<div>
	<dl class="result">
	<dt>合計(税込)</dt>
	<dd><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceCartTotalWithoutPaymentPrice) %></dd>
	</dl>
	</div>
	</div><!--priceList-->

	</div><!--block-->
	</div><!--bottom-->
	</div><!--subCartList-->

	<div id="Div12" visible="<%# ((CartObjectList)((Repeater)Container.Parent).DataSource).Items.Count == Container.ItemIndex + 1 %>" runat="server">
	<div class="sumBox">
	<div class="subSumBox">
	<p><img src="../../Contents/ImagesPkg/common/ttl_sum.gif" alt="総合計" width="52" height="16" />
		<strong><%#: CurrencyManager.ToPrice(this.CartList.PriceCartListTotalWithOutPaymentPrice) %></strong></p>
	</div>
	<%if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsLoggedIn) { %>
	<dl>
	<dt id="Dt1" Visible="<%# ((CartObject)Container.DataItem).FirstBuyPoint != 0 %>" runat="server">初回購入獲得ポイント</dt>
	<dd id="Dd1" Visible="<%# ((CartObject)Container.DataItem).FirstBuyPoint != 0 %>" runat="server"><%# WebSanitizer.HtmlEncode(GetNumeric(((CartObjectList)((Repeater)Container.Parent).DataSource).TotalFirstBuyPoint)) %>pt</dd>
	<dt>購入後獲得ポイント</dt>
	<dd><%# WebSanitizer.HtmlEncode(GetNumeric(((CartObjectList)((Repeater)Container.Parent).DataSource).TotalBuyPoint)) %>pt</dd>
	</dl>
	<small>※ 1pt = <%: CurrencyManager.ToPrice(1m) %></small>
	<%} %>
	</div><!--sumBox-->

	</div>
	
	</div><!--shoppingCart-->
	<%-- ▲カート情報▲ --%>

	<br class="clr" />
	</div><!--submain-->
	</div><!--main-->
	
	<%-- 隠し値：カートID --%>
	<asp:HiddenField ID="hfCartId" runat="server" Value="<%# ((CartObject)Container.DataItem).CartId %>" />
	<%-- 隠し再計算ボタン --%>
	<asp:LinkButton id="lbRecalculateCart" runat="server" CommandArgument="<%# Container.ItemIndex %>" onclick="lbRecalculate_Click"></asp:LinkButton>
</ItemTemplate>
</asp:Repeater>

<div class="btmbtn below">
<ul>
	<li><a onclick="<%= this.BackOnClick %>" href="<%= WebSanitizer.HtmlEncode(this.BackEvent) %>" class="btn btn-large btn-org-gry">前のページに戻る</a></li>
	<li>
		<a onclick="<%= this.NextOnClick %>" href="<%= WebSanitizer.HtmlEncode(this.NextEvent) %>" class="btn btn-large btn-success">ご注文内容確認へ</a>
		<a id="lbNextToConfirm" href="<%: this.NextEvent %>" style="display:none;"></a>
	</li>
</ul>
</div>

</ContentTemplate>
</asp:UpdatePanel>
<%-- UPDATE PANELここまで --%>

</div>
	
<%--▼▼ クレジットカードToken用スクリプト ▼▼--%>
<script type="text/javascript">
	var getTokenAndSetToFormJs = "<%= CreateGetCreditTokenAndSetToFormJsScript().Replace("\"", "\\\"") %>";
	var maskFormsForTokenJs = "<%= CreateMaskFormsForCreditTokenJsScript().Replace("\"", "\\\"") %>";
</script>
<uc:CreditToken runat="server" ID="CreditToken" />
<%--▲▲ クレジットカードToken用スクリプト ▲▲--%>
<%--▼▼ Paidy用スクリプト ▼▼--%>
<script type="text/javascript">
	var buyer = <%= PaidyUtility.CreatedBuyerDataObjectForPaidyPayment(this.CartList) %>;
	var hfPaidyTokenIdControlId = "<%= this.WhfPaidyTokenId.ClientID %>";
	var hfPaidyPaySelectedControlId = "<%= this.WhfPaidyPaySelected.ClientID %>";
	var lbNextProcess = "lbNextToConfirm";
	var isHistoryPage = false;
</script>
<uc:PaidyCheckoutScript ID="ucPaidyCheckoutScript" runat="server" />
<%--▲▲ Paidy用スクリプト ▲▲--%>
<%-- △編集可能領域△ --%>

<div id="divBottomArea">
<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>

</td>
<td>
<%-- ▽レイアウト領域：ライトエリア▽ --%>
<%-- △レイアウト領域△ --%>
</td>
</tr>
</table>
<uc:RakutenPaymentScript ID="ucRakutenPaymentScript" runat="server" />
<uc:Loading id="ucLoading" UpdatePanelReload="True" runat="server" />
</asp:Content>
