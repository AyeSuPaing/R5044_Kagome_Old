<%--
=========================================================================================================
  Module      : スマートフォン用リマインダー完了画面(PasswordReminderComplete.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/User/PasswordReminderComplete.aspx.cs" Inherits="Form_User_PasswordReminderComplete" Title="パスワード変更受付完了ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-order password-reminder-complete">
<div class="order-unit">
	<h2>受付完了</h2>
		<div class="msg">
			メールを<br/><span><%= this.MailAddress %></span><br/>にお送りいたしました<br /><br />
			今後とも、「<%= WebSanitizer.HtmlEncode(ShopMessage.GetMessage("ShopName")) %>」をどうぞ宜しくお願い申し上げます。<br />
			※<%: ReplaceTag("@@User.mail_addr.name@@") %>が未登録の場合、メールは送信されません。
			<p class="receptionInfo">
				<%= ShopMessage.GetMessageHtmlEncodeChangeToBr("ContactCenterInfo") %>
			</p>
		</div>
</div>
<div class="order-footer">
	<div class="button-next">
		<% if (string.IsNullOrEmpty(this.NextUrl) == false) { %>
		<asp:LinkButton ID="lbReturn" CssClass="btn" runat="server" OnClick="lbReturn_Click">戻る</asp:LinkButton>
		<% } else { %>
		<asp:LinkButton ID="lbTopPage" CssClass="btn" runat="server" OnClick="lbTopPage_Click">トップページへ</asp:LinkButton>
		<% } %>
	</div>
</div>
</asp:Content>