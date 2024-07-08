<%--
=========================================================================================================
  Module      : リマインダー入力画面(PasswordReminderInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/PasswordReminderInput.aspx.cs" Inherits="Form_User_PasswordReminderInput" Title="リマインダー入力ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserContents">

		<h2><%: ReplaceTag("@@User.password.name@@") %>再発行手続き</h2>

	<div id="dvPasswordReminderInput" class="unit">
		
			<%: ReplaceTag("@@User.password.name@@") %>をお忘れの方は、下記のフォームに必要事項をご入力の上、「送信」ボタンをクリックして下さい。<br />
			※<%: ReplaceTag("@@User.password.name@@") %>再設定用のリンクを記載したメールをお送りします。<br/>
			<ins><span class="necessary">*</span>は必須入力となります。</ins>
		<div class="dvReminder">
			<table cellspacing="0">
				<%if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED == false) { %>
				<tr>
					<th>
						ログインＩＤ<span class="necessary">*</span>
					</th>
					<td>
						<asp:TextBox ID="tbLoginId" Runat="server" CssClass="loginId" MaxLength="15" Type="email"></asp:TextBox>
						<asp:CustomValidator ID="CustomValidator1" runat="Server"
							ControlToValidate="tbLoginId"
							ValidationGroup="PasswordReminderInput"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<%} %>
				<tr>
					<th>
						<%: ReplaceTag("@@User.mail_addr.name@@") %>
						<span class="necessary">*</span>
					</th>
					<td>
						<asp:TextBox ID="tbMailAddr" Runat="server" CssClass="mailAddr mail-domain-suggest" MaxLength="256" Type="email"></asp:TextBox>
						<asp:CustomValidator ID="CustomValidator2" runat="Server"
							ControlToValidate="tbMailAddr"
							ValidationGroup="PasswordReminderInput"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
						<%if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED) { %>
						<br />※ログイン時に利用する<%: ReplaceTag("@@User.mail_addr.name@@") %>を入力してください
						<%} %>
						<% if(StringUtility.ToEmpty(this.ErrorMessage) != "") {%>
							<span class="error_inline">
								<%: this.ErrorMessage %></span>
						<%} %>
					</td>
				</tr>
			</table>
		</div>
		<div class="dvUserBtnBox">
			<p>
				<span><a href="javascript:history.back();" class="btn btn-large">キャンセル</a></span>
				<span><asp:LinkButton ID="lbSend" ValidationGroup="PasswordReminderInput" runat="server" OnClientClick="return exec_submit()" OnClick="lbSend_Click" class="btn btn-large btn-inverse">送信</asp:LinkButton></span>
			</p>
		</div>
	</div>
</div>
</asp:Content>
