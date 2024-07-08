<%--
=========================================================================================================
  Module      : スマートフォン用リマインダー入力画面(PasswordReminderInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/User/PasswordReminderInput.aspx.cs" Inherits="Form_User_PasswordReminderInput" Title="リマインダー入力ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">	
<section class="wrap-order password-reminder-input">
<div class="order-unit login">
	<h2><%: ReplaceTag("@@User.password.name@@") %>再発行手続き</h2>
	<div class="msg">
		会員登録時に登録された
		<%if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED == false){ %>ログインID及び<% } %><%: ReplaceTag("@@User.mail_addr.name@@") %>を入力頂き「送信」ボタンをクリックしてください。
		入力された<%: ReplaceTag("@@User.mail_addr.name@@") %>へ、<%: ReplaceTag("@@User.password.name@@") %>再設定ページへのリンクを記載したメールをお送りします。
	<% if(StringUtility.ToEmpty(this.ErrorMessage) != "") {%>
		<p class="attention"><%: this.ErrorMessage %></p>
	<%} %>
	</div>
	<dl class="order-form">
	<%if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED == false){ %>
		<dt>
			<%: ReplaceTag("@@User.login_id.name@@") %>
		</dt>
		<dd>
			<w2c:ExtendedTextBox ID="tbLoginId" Type="email" Runat="server" MaxLength="15"></w2c:ExtendedTextBox>
			<asp:CustomValidator runat="Server"
					ControlToValidate="tbLoginId"
					ValidationGroup="PasswordReminderInput"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
		</dd>
	<% } %>
		<dt>
			<%: ReplaceTag("@@User.mail_addr.name@@") %>
		</dt>
		<dd>
			<w2c:ExtendedTextBox ID="tbMailAddr" Type="email" Runat="server" MaxLength="256" CssClass="mail-domain-suggest"></w2c:ExtendedTextBox>
			<asp:CustomValidator runat="Server"
					ControlToValidate="tbMailAddr"
					ValidationGroup="PasswordReminderInput"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
		</dd>
	</dl>
</div>

<div class="order-footer">
	<div class="button-next">
		<asp:LinkButton ID="lbSend" OnClientClick="return exec_submit();" CssClass="btn" runat="server" OnClick="lbSend_Click">送信する</asp:LinkButton>
	</div>
</div>
</section>
</asp:Content>