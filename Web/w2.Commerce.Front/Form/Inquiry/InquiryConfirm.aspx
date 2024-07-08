<%--
=========================================================================================================
  Module      : 問合せ確認画面(InquiryConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="Captcha" Src="~/Form/Common/Captcha.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/Inquiry/InquiryConfirm.aspx.cs" Inherits="Form_Inquiry_InquiryConfirm" Title="確認ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserContents">
	<%-- 問合せ入力系パンくず --%>
	<div id="dvHeaderRegistClumbs">
		<p><img src="../../Contents/ImagesPkg/inquiry/clumbs_inquiry_2.gif" alt="問合せ情報の確認" /></p>
	</div>

		<h2>問合せ情報の確認</h2>

	<div id="dvUserInquiryConfirm" class="unit">

		<%-- 問合せ項目入力フォーム --%>
		<div class="dvUserInfo">
			<h3>問合せ情報</h3>

			<%-- 問合せ項目開始 --%>
			<table cellspacing="0">
				<%-- 問合せ件名 --%>
				<tr>
					<th>問合せ件名</th>
					<td><%= WebSanitizer.HtmlEncode(this.InquiryInput["inquiry_title"])%></td>
				</tr>
				<tr>
					<th>問い合わせ内容</th>
					<td><%= WebSanitizer.HtmlEncodeChangeToBr(this.InquiryInput["inquiry_text"])%></td>
				</tr>
				<tr>
					<th>
						<%: ReplaceTag("@@User.name.name@@") %>
					</th>
					<td><%= WebSanitizer.HtmlEncode(this.InquiryInput[Constants.FIELD_USER_NAME1])%>
					<%= WebSanitizer.HtmlEncode(this.InquiryInput[Constants.FIELD_USER_NAME2])%></td>
				</tr>
				<% if (this.IsJapanese){ %>
				<tr>
					<th>
						<%: ReplaceTag("@@User.name_kana.name@@") %>
					</th>
					<td><%= WebSanitizer.HtmlEncode(this.InquiryInput[Constants.FIELD_USER_NAME_KANA1])%>
					<%= WebSanitizer.HtmlEncode(this.InquiryInput[Constants.FIELD_USER_NAME_KANA2])%></td>
				</tr>
				<% } %>
				<tr>
					<th><%: ReplaceTag("@@User.mail_addr.name@@") %></th>
					<td><%= WebSanitizer.HtmlEncode(this.InquiryInput[Constants.FIELD_USER_MAIL_ADDR])%></td>
				</tr>
				<tr>
					<th>
						<%: ReplaceTag("@@User.tel1.name@@") %>
					</th>
					<td><%= WebSanitizer.HtmlEncode(this.InquiryInput[Constants.FIELD_USER_TEL1_1])%>-<%= WebSanitizer.HtmlEncode(this.InquiryInput[Constants.FIELD_USER_TEL1_2])%>-<%= WebSanitizer.HtmlEncode(this.InquiryInput[Constants.FIELD_USER_TEL1_3])%></td>
				</tr>
			</table>
		</div>

		<%-- キャプチャ認証 --%>
		<uc:Captcha ID="ucCaptcha" runat="server" EnabledControlClientID="<%# lbSend.ClientID %>" />

		<%-- 問合せ項目ここまで --%>
		<div class="dvUserBtnBox">
			<p>
				<span><asp:LinkButton ID="lbBack" runat="server" OnClientClick="return exec_submit()" OnClick="lbBack_Click" class="btn btn-large">
					戻る</asp:LinkButton></span>
				<span><asp:LinkButton ID="lbSend" runat="server" OnClientClick="return exec_submit();" OnClick="lbSend_Click" class="btn btn-large btn-inverse">
					送信する</asp:LinkButton></span>
			</p>
		</div>
	</div>
</div>
</asp:Content>