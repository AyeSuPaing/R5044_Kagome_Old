<%--
=========================================================================================================
  Module      : 問合せ確認画面(InquiryConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="Captcha" Src="~/SmartPhone/Form/Common/Captcha.ascx" %>
<%@ page language="C#" masterpagefile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/Inquiry/InquiryConfirm.aspx.cs" Inherits="Form_Inquiry_InquiryConfirm" title="問合せ確認ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-user inquiry-comfirm">
<div class="user-unit">
	<h2>問合せ情報の確認</h2>
	<dl class="user-form">
		<dt>件名</dt>
		<dd><%= WebSanitizer.HtmlEncode(this.InquiryInput["inquiry_title"])%></dd>
		<dt>内容</dt>
		<dd><%= WebSanitizer.HtmlEncodeChangeToBr(this.InquiryInput["inquiry_text"])%></dd>
		<%-- 氏名 --%>
		<dt>
			<%: ReplaceTag("@@User.name.name@@") %>
		</dt>
		<dd><%= WebSanitizer.HtmlEncode(this.InquiryInput[Constants.FIELD_USER_NAME1])%>
			<%= WebSanitizer.HtmlEncode(this.InquiryInput[Constants.FIELD_USER_NAME2])%></dd>
		<%-- 氏名（かな） --%>
		<% if (this.IsJapanese) { %>
		<dt>
			<%: ReplaceTag("@@User.name_kana.name@@") %>
		</dt>
		<dd><%= WebSanitizer.HtmlEncode(this.InquiryInput[Constants.FIELD_USER_NAME_KANA1])%>
			<%= WebSanitizer.HtmlEncode(this.InquiryInput[Constants.FIELD_USER_NAME_KANA2])%></dd>
		<% } %>
		<%-- メールアドレス --%>
		<dt>
			<%: ReplaceTag("@@User.mail_addr.name@@") %>
		</dt>
		<dd><%= WebSanitizer.HtmlEncode(this.InquiryInput[Constants.FIELD_USER_MAIL_ADDR])%></dd>
		<%-- 電話番号 --%>
		<dt>
			<%: ReplaceTag("@@User.tel1.name@@") %>
		</dt>
		<dd><%= WebSanitizer.HtmlEncode(this.InquiryInput[Constants.FIELD_USER_TEL1_1])%>-<%= WebSanitizer.HtmlEncode(this.InquiryInput[Constants.FIELD_USER_TEL1_2])%>-<%= WebSanitizer.HtmlEncode(this.InquiryInput[Constants.FIELD_USER_TEL1_3])%></dd>
	</dl>
</div>

<%-- キャプチャ認証 --%>
<uc:Captcha ID="ucCaptcha" runat="server" EnabledControlClientID="<%# lbSend.ClientID %>" />

<div class="user-footer">
	<div class="button-next">
		<asp:LinkButton ID="lbSend" runat="server" OnClientClick="return exec_submit();" OnClick="lbSend_Click" CssClass="btn">送信する</asp:LinkButton>
	</div>
	<div class="button-prev">
		<asp:LinkButton ID="lbBack" runat="server" OnClientClick="return exec_submit()" OnClick="lbBack_Click" CssClass="btn">戻る</asp:LinkButton>
	</div>
</div>

</section>
</asp:Content>