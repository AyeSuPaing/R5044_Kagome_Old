<%--
=========================================================================================================
  Module      : スマートフォン用パスワード変更入力画面(PasswordModifyInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/User/PasswordModifyInput.aspx.cs" Inherits="Form_User_PasswordModifyInput" Title="パスワード変更入力ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-order password-modify-input">
<div class="order-unit login">
	<h2><%: ReplaceTag("@@User.password.name@@") %>変更</h2>
	<dl class="order-form">
		<dt>
			<%: ReplaceTag("@@User.login_id.name@@") %>
		</dt>
		<dd><%= this.LoginId %></dd>
	<%-- かんたん会員の場合は、生年月日や電話番号による確認をスキップ --%>
	<% if (this.EasyRegisterFlg == Constants.FLG_USER_EASY_REGISTER_FLG_NORMAL) {%>
	<% if (Constants.PASSWORDRIMINDER_AUTHITEM == Constants.PasswordReminderAuthItem.Birth){%>
		<dt>
			<%: ReplaceTag("@@User.birth.name@@") %>
		</dt>
		<dd class="birth">
			<p class="attention">
				<asp:CustomValidator ID="cvBirth" runat="Server"
					ControlToValidate="tbBirth"
					ValidationGroup="PasswordModify"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
			</p>
			<% tbBirth.Attributes["placeholder"] = ReplaceTag("@@User.birth.name@@"); %>
			<w2c:ExtendedTextBox ID="tbBirth" Runat="server" Type="tel" MaxLength="8"></w2c:ExtendedTextBox>
			<p class="msg">例）19700101</p>
		</dd>
	<% } %>
	<% if (Constants.PASSWORDRIMINDER_AUTHITEM == Constants.PasswordReminderAuthItem.Tel){%>
		<dt>
			<%: ReplaceTag("@@User.tel1.name@@") %>
		</dt>
		<dd class="tel">
			<p class="attention">
				<asp:CustomValidator ID="cvTel1_1" runat="Server"
					ControlToValidate="tbTel1"
					ValidationGroup="PasswordModify"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
			</p>
			<w2c:ExtendedTextBox ID="tbTel1" Runat="server" Type="tel" style="width:100%;" MaxLength="13" CssClass="shortTel" />
		</dd>
	<%} %>
	<%} %>
		<dt>
			新しい<%: ReplaceTag("@@User.password.name@@") %>
		</dt>
		<dd>
			<p class="attention">
				<asp:CustomValidator ID="cvPassword" runat="Server"
					ControlToValidate="tbPassword"
					ValidationGroup="PasswordModify"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
			</p>
			<% tbPassword.Attributes["placeholder"] = "新しい" + ReplaceTag("@@User.password.name@@"); %>
			<w2c:ExtendedTextBox ID="tbPassword" Runat="server" TextMode="Password" autocomplete="off" MaxLength="15"></w2c:ExtendedTextBox>
			<p class="msg">※半角英数字混合7～15文字</p>
		</dd>
		<dt>新しい<%: ReplaceTag("@@User.password.name@@") %>（確認用）</dt>
		<dd>
			<p class="attention">
				<asp:CustomValidator ID="cvPasswordConf" runat="Server"
					ControlToValidate="tbPasswordConf"
					ValidationGroup="PasswordModify"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
			</p>
			<% tbPasswordConf.Attributes["placeholder"] = "新しい" + ReplaceTag("@@User.password.name@@") + "（確認用）"; %>
			<w2c:ExtendedTextBox ID="tbPasswordConf" Runat="server" TextMode="Password" autocomplete="off" MaxLength="15"></w2c:ExtendedTextBox>
			<p class="msg">※半角英数字混合7～15文字</p>
		</dd>
	</dl>
</div>

<div class="order-footer">
	<div class="button-next">
		<asp:LinkButton class="btn" ID="lbModify" OnClientClick="return exec_submit();" OnClick="lbModify_Click" runat="server">変更</asp:LinkButton>
	</div>
</div>

</section>
</asp:Content>