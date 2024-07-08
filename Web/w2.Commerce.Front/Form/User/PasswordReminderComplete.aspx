<%--
=========================================================================================================
  Module      : リマインダー完了画面(PasswordReminderComplete.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/PasswordReminderComplete.aspx.cs" Inherits="Form_User_PasswordReminderComplete" Title="パスワード変更受付完了ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserContents">

		<h2>受付完了</h2>

	<div id="dvPasswordReminderComplete" class="unit">
		<p class="completeInfo"><%: ReplaceTag("@@User.password.name@@") %>再設定用のリンクを記載したメールを<br/><span><%= this.MailAddress %></span><br/>にお送りいたしましたのでご確認下さい。<br /><br />
		今後とも、「<%= WebSanitizer.HtmlEncode(ShopMessage.GetMessage("ShopName")) %>」をどうぞ宜しくお願い申し上げます。<br />
		※メールアドレスが未登録の場合、メールは送信されません。
		</p>
		<p class="receptionInfo">
			<%= ShopMessage.GetMessageHtmlEncodeChangeToBr("ContactCenterInfo") %>
		</p>
		<div class="dvUserBtnBox">
			<% if (string.IsNullOrEmpty(this.NextUrl) == false) { %>
			<p><span><asp:LinkButton ID="lbReturn" runat="server" OnClick="lbReturn_Click" class="btn btn-large btn-inverse">
				戻る</asp:LinkButton></span></p>
			<% } else { %>
			<p><span><asp:LinkButton ID="lbTopPage" runat="server" OnClick="lbTopPage_Click" class="btn btn-large btn-inverse">
				トップページへ</asp:LinkButton></span></p>
			<% } %>
		</div>
	</div>
</div>
</asp:Content>