<%--
=========================================================================================================
  Module      : ユーザクレジットカード入力画面(UserCreditCardInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="Captcha" Src="~/Form/Common/Captcha.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserCreditCardInput.aspx.cs" Inherits="Form_User_UserCreditCardInput" Title="登録クレジットカード入力ページ"%>
<%-- ▼削除禁止：クレジットカードTokenコントロール▼ --%>
<%@ Register TagPrefix="uc" TagName="CreditToken" Src="~/Form/Common/CreditToken.ascx" %>
<%-- ▲削除禁止：クレジットカードTokenコントロール▲ --%>
<%@ Register TagPrefix="uc" TagName="RakutenCreditCard" Src="~/Form/Common/RakutenCreditCardModal.ascx" %>
<%@ Register TagPrefix="uc" TagName="RakutenPaymentScript" Src="~/Form/Common/RakutenPaymentScript.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserFltContents">
	<%-- メッセージ --%>
	<div id="dvHeaderUserCreditCardClumbs">
	<p>
		<img src="../../Contents/ImagesPkg/user/clumbs_usercreditcard_1.gif" alt="クレジットカードの登録" /></p>
	</div>

		<h2>カード情報の入力</h2>

	<div id="dvUserCreditCardInput" class="unit">
		<div class="dvContentsInfo">
			<p>クレジットカード情報を登録します。<br/>下のフォームに入力し、「確認する」ボタンを押してください。
			登録するクレジットカードには、「クレジットカード登録名」を登録する事ができます。（例：「VISA」「Master」など）</p>
		</div>
		<div class="dvUserCreditCardInfo">
			<h3>クレジットカード情報</h3>
			<ins><span class="necessary">*</span>は必須入力となります。</ins>
			
			<%-- UPDATE PANEL開始 --%>
			<asp:UpdatePanel ID="upUpdatePanel" runat="server">
			<ContentTemplate>
			<div id="divCreditCardInputForm">
			<table cellspacing="0">
				<tr>
					<th>クレジットカード登録名<span class="necessary">*</span></th>
					<td>
						<asp:TextBox id="tbUserCreditCardName" Runat="server" maxlength="30" CssClass="nameCreditCard"></asp:TextBox>
						<asp:CustomValidator ID="cvUserCreditCardName" runat="Server"
							ControlToValidate="tbUserCreditCardName"
							ValidationGroup="UserCreditCardRegist"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<% if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten) { %>
							<span id="cvUserCreditCardNameRakuten" class="error_inline" style="color:Red;visibility:hidden;"></span>
						<% } %>
						<%--▼▼ クレジット Token保持用 ▼▼--%>
						<asp:HiddenField ID="hfCreditToken" Value="" runat="server" />
						<%--▲▲ クレジット Token保持用 ▲▲--%>
					</td>
				</tr>
				<% if (this.IsCreditCardLinkPayment() == false) { %>
				<%--▼▼ カード情報入力（トークン未取得・利用なし） ▼▼--%>
				<tbody id="divRakutenCredit" runat="server">
				<tr>
					<th>クレジットカード情報</th>
					<td>
						<asp:LinkButton id="lbEditCreditCardNoForRakutenToken" CssClass="lbEditCreditCardNoForRakutenToken" OnClick="lbEditCreditCardNoForToken_Click" runat="server">再入力</asp:LinkButton>
						<uc:RakutenCreditCard
							ID="ucRakutenCreditCard"
							IsOrder="false"
							CartIndex="1"
							InstallmentCodeList="<%# this.CreditInstallmentsList %>"
							runat="server"/>
					</td>
				</tr>
				</tbody>
				<tbody id="divCreditCardNoToken" runat="server">
					<%if (OrderCommon.CreditCompanySelectable) {%>
					<tr>
						<th>カード会社<span class="necessary">*</span></th>
						<td>
							<asp:DropDownList id="ddlCreditCardCompany" runat="server" CssClass="input_border"></asp:DropDownList>
						</td>
					</tr>
					<%} %>
					<tr id="trCreditCardNo" runat="server">
						<th>カード番号<span class="necessary">*</span></th>
						<td>
							<w2c:ExtendedTextBox id="tbCreditCardNo1" Type="tel" runat="server" MaxLength="16" autocomplete="off"></w2c:ExtendedTextBox>
							※&nbsp;例：1234567890123456（ハイフンなし）
							<asp:CustomValidator ID="cvCreditCardNo1" runat="Server"
								ControlToValidate="tbCreditCardNo1"
								ValidationGroup="UserCreditCardRegist"
								ValidateEmptyText="true"
								SetFocusOnError="true"
								ClientValidationFunction="ClientValidate"
								CssClass="error_inline" />
							<%--▼▼ カード情報取得用 ▼▼--%>
							<input type="hidden" id="hidCinfo" name="hidCinfo" value="<%= CreateGetCardInfoJsScriptForCreditToken() %>" />
							<span id="spanErrorMessageForCreditCard" style="color: red; display: none" runat="server"></span>
							<%--▲▲ カード情報取得用 ▲▲--%>
						</td>
					</tr>
					<tr>
						<th>有効期限<span class="necessary">*</span></th>
						<td>
							<asp:DropDownList ID="ddlCreditExpireMonth" runat="server" ></asp:DropDownList>&nbsp;
							/
							<asp:DropDownList ID="ddlCreditExpireYear" runat="server" ></asp:DropDownList>&nbsp;(月/年)
						</td>
					</tr>
					<tr>
						<th>カード名義人<span class="necessary">*</span></th>
						<td>
							<asp:TextBox ID="tbCreditAuthorName" runat="server" MaxLength="50" class="input_widthB" autocomplete="off"></asp:TextBox>
							※&nbsp;例：「TAROU YAMADA」
							<asp:CustomValidator ID="cvCreditAuthorName" runat="Server"
								ControlToValidate="tbCreditAuthorName"
								ValidationGroup="UserCreditCardRegist"
								ValidateEmptyText="true"
								SetFocusOnError="true"
								ClientValidationFunction="ClientValidate"
								CssClass="error_inline" />
						</td>
					</tr>
					<tr id="trSecurityCode" runat="server">
						<th>セキュリティコード<span class="necessary">*</span></th>
						<td>
							<asp:TextBox ID="tbCreditSecurityCode" runat="server" MaxLength="4" class="input_widthA" autocomplete="off" Type="tel"></asp:TextBox>
							<asp:CustomValidator ID="cvCreditSecurityCode" runat="Server"
								ControlToValidate="tbCreditSecurityCode"
								ValidationGroup="UserCreditCardRegist"
								ValidateEmptyText="true"
								SetFocusOnError="true"
								ClientValidationFunction="ClientValidate"
								CssClass="error_inline" />
						</td>
					</tr>
				</tbody>
				<%--▲▲ カード情報入力（トークン未取得・利用なし） ▲▲--%>
				<%--▼▼ カード情報入力（トークン取得済） ▼▼--%>
				<tbody id="divCreditCardForTokenAcquired" runat="server">
				<%if (OrderCommon.CreditCompanySelectable) {%>
				<tr>
					<th>カード会社</th>
					<td><asp:Literal ID="lCreditCardCompanyNameForTokenAcquired" runat="server"></asp:Literal></td>
				</tr>
				<%} %>
				<tr>
					<th>カード番号</th>
					<td>
						XXXXXXXXXXXX<asp:Literal ID="lLastFourDigitForTokenAcquired" runat="server"></asp:Literal>
						<asp:LinkButton id="lbEditCreditCardNoForToken" OnClick="lbEditCreditCardNoForToken_Click" runat="server">再入力</asp:LinkButton>
					</td>
				</tr>
				<tr>
					<th>有効期限</th>
					<td>
						<asp:Literal ID="lExpirationMonthForTokenAcquired" runat="server"></asp:Literal>
						/
						<asp:Literal ID="lExpirationYearForTokenAcquired" runat="server"></asp:Literal>
						(月/年)
					</td>
				</tr>
				<tr>
					<th>カード名義人</th>
					<td>
						<asp:Literal ID="lCreditAuthorNameForTokenAcquired" runat="server"></asp:Literal>
					</td>
				</tr>
				</tbody>
				<%--▲▲ カード情報入力（トークン取得済） ▲▲ --%>
				<% } else { %>
				<tr>
					<th></th>
					<td>遷移する外部サイトでカード番号を入力してください。</td>
				</tr>
				<% } %>
			</table>
			</div>
			</ContentTemplate>
			</asp:UpdatePanel>
			<%-- UPDATE PANELここまで --%>
		</div>

		<%if (this.BranchNo == 0){ %>
		<%-- キャプチャ認証 --%>
			<uc:Captcha ID="ucCaptcha" runat="server" EnabledControlClientID="<%# lbConfirm.ClientID %>" />
		<% } %>

		<div class="dvUserBtnBox">
			<p>
				<span><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_CREDITCARD_LIST) %>" class="btn btn-large">戻る</a></span>
				<span><asp:LinkButton ID="lbConfirm" OnClientClick="doPostbackEvenIfCardAuthFailed=true;return true;" runat="server" OnClick="lbConfirm_Click" class="btn btn-large btn-inverse">確認する</asp:LinkButton></span>
			</p>
		</div>
	</div>
</div>

<%--▼▼ クレジットカードToken用スクリプト ▼▼--%>
<script type="text/javascript">
	var getTokenAndSetToFormJs = "<%= CreateGetCreditTokenAndSetToFormJsScript().Replace("\"", "\\\"") %>";
	var maskFormsForTokenJs = "<%= CreateMaskFormsForCreditTokenJsScript().Replace("\"", "\\\"") %>";
</script>
<uc:CreditToken runat="server" ID="CreditToken" />
<%--▲▲ クレジットカードToken用スクリプト ▲▲--%>
<uc:RakutenPaymentScript ID="ucRakutenPaymentScript" runat="server" />
</asp:Content>
