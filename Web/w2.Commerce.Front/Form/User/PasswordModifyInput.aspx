<%--
=========================================================================================================
  Module      : パスワード変更入力画面(PasswordModifyInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/PasswordModifyInput.aspx.cs" Inherits="Form_User_PasswordModifyInput" Title="パスワード変更入力ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserContents">

		<h2>パスワード変更</h2>

	<div id="dvPasswordModifyInput" class="unit">
		<p>
			下記のフォームに必要事項をご入力の上、「変更する」ボタンをクリックして下さい。<br /><ins><span class="necessary">*</span>は必須入力となります。</ins></p>
		<%-- UPDATE PANEL開始 --%>
		<asp:UpdatePanel ID="upUpdatePanel" runat="server">
		<ContentTemplate>
		<div class="dvModify">
			<table cellspacing="0">
			    <tr>
					<th>
						<%: ReplaceTag("@@User.login_id.name@@") %>
					</th>
					<td>
						<%: this.LoginId %>
					</td>
			    </tr>
				<%-- かんたん会員の場合は、生年月日や電話番号による確認をスキップ --%>
				<% if (this.EasyRegisterFlg == Constants.FLG_USER_EASY_REGISTER_FLG_NORMAL) {%>
				<% if (Constants.PASSWORDRIMINDER_AUTHITEM == Constants.PasswordReminderAuthItem.Birth) {%>
				<tr>
					<th>
						<%: ReplaceTag("@@User.birth.name@@") %>
						<span class="necessary">*</span>
					</th>
					<td>
						<asp:TextBox ID="tbBirth" Runat="server" CssClass="loginId" MaxLength="8"></asp:TextBox>※例）19700101
						<asp:CustomValidator ID="cvBirth" runat="Server"
							ControlToValidate="tbBirth"
							ValidationGroup="PasswordModify"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } else if (Constants.PASSWORDRIMINDER_AUTHITEM == Constants.PasswordReminderAuthItem.Tel) {%>
				<tr>
					<th>
						<%: ReplaceTag("@@User.tel1.name@@") %>
						<span class="necessary">*</span>
					</th>
					<td>
						<asp:TextBox ID="tbTel1" MaxLength="13" Type="tel" runat="server" CssClass="shortTel" />
						<asp:CustomValidator ID="cvTel1_1" runat="Server"
							ControlToValidate="tbTel1"
							ValidationGroup="PasswordModify"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<% } %>
				<% } %>
				<tr>
					<th>
						新しい<%: ReplaceTag("@@User.password.name@@") %>
						<span class="necessary">*</span>
					</th>
					<td>
						<asp:TextBox ID="tbPassword" Runat="server" TextMode="Password" autocomplete="off" CssClass="loginId" MaxLength="15"></asp:TextBox>※半角英数字混合7～15文字
						<asp:CustomValidator ID="cvPassword" runat="Server"
							ControlToValidate="tbPassword"
							ValidationGroup="PasswordModify"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
				<tr>
					<th>
						新しい<%: ReplaceTag("@@User.password.name@@") %>（確認用）
						<span class="necessary">*</span>
					</th>
					<td>
						<asp:TextBox ID="tbPasswordConf" Runat="server" TextMode="Password" autocomplete="off" CssClass="loginId" MaxLength="15"></asp:TextBox>※半角英数字混合7～15文字
						<asp:CustomValidator ID="cvPasswordConf" runat="Server"
							ControlToValidate="tbPasswordConf"
							ValidationGroup="PasswordModify"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							CssClass="error_inline" />
					</td>
				</tr>
			</table>
			<span style="display:block;color:Red;margin:5px 0;">注意：<%= this.PasswordReminder.ChangeTrialLimitCount %>回失敗するとパスワードの変更が出来なくなります。</span>
		</div>
		<div class="dvUserBtnBox">
			<p>
				<span><asp:LinkButton ID="lbModify" ValidationGroup="PasswordModify" OnClientClick="return exec_submit();" OnClick="lbModify_Click" runat="server" class="btn btn-large btn-inverse">
					変更</asp:LinkButton></span>
			</p>
		</div>
		</ContentTemplate>
		</asp:UpdatePanel>
		<%-- UPDATE PANELここまで --%>
	</div>
</div>
</asp:Content>