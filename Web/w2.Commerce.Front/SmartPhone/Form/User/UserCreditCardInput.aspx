<%--
=========================================================================================================
  Module      : スマートフォン用ユーザクレジットカード入力画面(UserCreditCardInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="Captcha" Src="~/SmartPhone/Form/Common/Captcha.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserCreditCardInput.aspx.cs" Inherits="Form_User_UserCreditCardInput" Title="登録クレジットカード入力ページ" %>
<%-- ▼削除禁止：クレジットカードTokenコントロール▼ --%>
<%@ Register TagPrefix="uc" TagName="CreditToken" Src="~/Form/Common/CreditToken.ascx" %>
<%-- ▲削除禁止：クレジットカードTokenコントロール▲ --%>
<%@ Register TagPrefix="uc" TagName="RakutenCreditCard" Src="~/Form/Common/RakutenCreditCardModal.ascx" %>
<%@ Register TagPrefix="uc" TagName="RakutenPaymentScript" Src="~/Form/Common/RakutenPaymentScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="Loading" Src="~/Form/Common/Loading.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-user user-credit-card-input">
<div class="user-unit">
	<h2>クレジットカードの入力</h2>
	<div class="msg">
		クレジットカード情報を登録します。<br/>
		下のフォームに入力し、「確認する」ボタンを押してください。<br/>
		登録名は自由に記述できます。（例：「VISA」「家族用」など）」
	</div>
	<%-- UPDATE PANEL開始 --%>
	<asp:UpdatePanel ID="upUpdatePanel" runat="server">
	<ContentTemplate>
	<div id="divCreditCardInputForm">
	<dl class="user-form">
		<dt>登録名<span class="require">※</span></dt>
		<dd class="card-name">
			<p class="attention">
				<asp:CustomValidator id="cvUserCreditCardName"
					runat="Server"
					ControlToValidate="tbUserCreditCardName"
					ValidationGroup="UserCreditCardRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
			</p>
			<w2c:ExtendedTextBox id="tbUserCreditCardName" Runat="server" maxlength="30"></w2c:ExtendedTextBox>
			<% if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten) { %>
					<span id="cvUserCreditCardNameRakuten" class="error_inline" style="color:Red;visibility:hidden;"></span>
			<% } %>
			
			<%--▼▼ クレジット Token保持用 ▼▼--%>
			<asp:HiddenField ID="hfCreditToken" Value="" runat="server" />
			<%--▲▲ クレジット Token保持用 ▲▲--%>
		</dd>
		<% if (this.IsCreditCardLinkPayment() == false) { %>
		<%--▼▼ カード情報入力（トークン未取得・利用なし） ▼▼--%>
		<div id="divRakutenCredit" runat="server">
			<dt>カード情報<span class="require">※</span></dt>
			<dd class="card-nums">
			<asp:LinkButton id="lbEditCreditCardNoForRakutenToken" CssClass="lbEditCreditCardNoForRakutenToken" OnClick="lbEditCreditCardNoForToken_Click" runat="server">再入力</asp:LinkButton>
			<uc:RakutenCreditCard
				ID="ucRakutenCreditCard"
				IsOrder="false"
				CartIndex="1"
				InstallmentCodeList="<%# this.CreditInstallmentsList %>"
				runat="server" />
			</dd>
		</div>
		<div id="divCreditCardNoToken" runat="server">
		<%if (OrderCommon.CreditCompanySelectable) {%>
		<dt>カード会社<span class="require">※</span></dt>
		<dd class="card-nums">
			<asp:DropDownList id="ddlCreditCardCompany" runat="server"></asp:DropDownList>
		</dd>
		<% } %>
		<dt>カード番号<span class="require">※</span></dt>
		<dd class="card-nums">
			<p class="attention">
				<asp:CustomValidator ID="cvCreditCardNo1" runat="Server"
					ControlToValidate="tbCreditCardNo1"
					ValidationGroup="UserCreditCardRegist"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
			</p>
			<w2c:ExtendedTextBox id="tbCreditCardNo1" Type="tel" runat="server" MaxLength="16" autocomplete="off"></w2c:ExtendedTextBox>
			<%--▼▼ カード情報取得用 ▼▼--%>
			<input type="hidden" id="hidCinfo" name="hidCinfo" value="<%= CreateGetCardInfoJsScriptForCreditToken() %>" />
			<span id="spanErrorMessageForCreditCard" style="color: red; display: none" runat="server"></span>
			<%--▲▲ カード情報取得用 ▲▲--%>
		</dd>
		<dt>有効期限<span class="require">※</span></dt>
		<dd class="credit-expire">
			<asp:DropDownList id="ddlCreditExpireMonth" runat="server"></asp:DropDownList>/
			<asp:DropDownList id="ddlCreditExpireYear" runat="server"></asp:DropDownList> (月/年)
		</dd>
		<dt>カード名義人<span class="require">※</span></dt>
		<dd class="card-name">
			<p class="attention">
			<asp:CustomValidator runat="Server"
				ID="cvCreditAuthorName"
				ControlToValidate="tbCreditAuthorName"
				ValidationGroup="UserCreditCardRegist"
				ValidateEmptyText="true"
				SetFocusOnError="true" />
			</p>
			<w2c:ExtendedTextBox id="tbCreditAuthorName" Type="email" title="" runat="server" MaxLength="50" autocomplete="off"></w2c:ExtendedTextBox><br />
			例：「TAROU YAMADA」<br />
		</dd>
		<dt>セキュリティコード<span class="require">※</span></dt>
		<dd class="card-sequrity">
			<p class="attention">
			<asp:CustomValidator runat="Server"
				ID="cvCreditSecurityCode"
				ControlToValidate="tbCreditSecurityCode"
				ValidationGroup="UserCreditCardRegist"
				ValidateEmptyText="true"
				SetFocusOnError="true" />
			</p>
			<w2c:ExtendedTextBox id="tbCreditSecurityCode" Type="tel" runat="server" MaxLength="4" autocomplete="off"></w2c:ExtendedTextBox>
			<p>
				<img src="<%= Constants.PATH_ROOT %>SmartPhone/Contents/ImagePkg/common/card-sequrity-code.gif" alt="セキュリティコードとは" width="280" />
			</p>
		</dd>
		</div>
		<%--▲▲ カード情報入力（トークン未取得・利用なし） ▲▲--%>
		<%--▼▼ カード情報入力（トークン取得済） ▼▼--%>
		<div id="divCreditCardForTokenAcquired" runat="server">
		<%if (OrderCommon.CreditCompanySelectable) {%>
		<dt>カード会社</dt>
		<dd><asp:Literal ID="lCreditCardCompanyNameForTokenAcquired" runat="server"></asp:Literal></dd>
		<%} %>
		<dt>カード番号</dt>
		<dd>
			XXXXXXXXXXXX<asp:Literal ID="lLastFourDigitForTokenAcquired" runat="server"></asp:Literal>
			<asp:LinkButton id="lbEditCreditCardNoForToken" OnClick="lbEditCreditCardNoForToken_Click" runat="server">再入力</asp:LinkButton>
		</dd>
		<dt>有効期限</dt>
		<dd>
			<asp:Literal ID="lExpirationMonthForTokenAcquired" runat="server"></asp:Literal>
			/
			<asp:Literal ID="lExpirationYearForTokenAcquired" runat="server"></asp:Literal>
			(月/年)
		</dd>
		<dt>カード名義人</dt>
		<dd>
			<asp:Literal ID="lCreditAuthorNameForTokenAcquired" runat="server"></asp:Literal>
		</dd>
		</div>
		<%--▲▲ カード情報入力（トークン取得済） ▲▲ --%>
		<% } else { %>
			<dt></dt>
			<dd>遷移する外部サイトでカード番号を入力してください。</dd>
		<% } %>
	</dl>
	</div>
</div>
</ContentTemplate>
</asp:UpdatePanel>
<%-- UPDATE PANELここまで --%>

<%if (this.BranchNo == 0){ %>
	<%-- キャプチャ認証 --%>
	<uc:Captcha ID="ucCaptcha" runat="server" EnabledControlClientID="<%# lbConfirm.ClientID %>" />
<% } %>

<div class="user-footer">
	<div class="button-next">
		<asp:LinkButton ID="lbConfirm" OnClientClick="doPostbackEvenIfCardAuthFailed=true;return true;" runat="server" OnClick="lbConfirm_Click" CssClass="btn">確認する</asp:LinkButton>
	</div>
	<div class="button-prev">
		<span><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_CREDITCARD_LIST) %>" class="btn btn-large">戻る</a></span>
	</div>
</div>
</section>
	
<%--▼▼ クレジットカードToken用スクリプト ▼▼--%>
<script type="text/javascript">
	var getTokenAndSetToFormJs = "<%= CreateGetCreditTokenAndSetToFormJsScript().Replace("\"", "\\\"") %>";
	var maskFormsForTokenJs = "<%= CreateMaskFormsForCreditTokenJsScript().Replace("\"", "\\\"") %>";
</script>
<uc:CreditToken runat="server" ID="CreditToken" />
<%--▲▲ クレジットカードToken用スクリプト ▲▲--%>
<uc:RakutenPaymentScript ID="ucRakutenPaymentScript" runat="server" />
	<uc:Loading id="ucLoading" UpdatePanelReload="True" runat="server" />
</asp:Content>
