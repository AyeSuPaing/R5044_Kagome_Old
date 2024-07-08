<%--
=========================================================================================================
  Module      : スマートフォン用会員退会入力画面(UserWithdrawalInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserWithdrawalInput.aspx.cs" Inherits="Form_User_UserWithdrawalInput" Title="退会入力ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-user user-withdrawal-input">
<div class="user-unit">
	<h2>退会手続き</h2>
	<div class="msg">
		<p><%= WebSanitizer.HtmlEncode(ShopMessage.GetMessage("ShopName")) %>をご利用いただきありがとうございました。<br />
		「退会する」ボタンを押すことで退会手続きが完了いたします。</p>
		<p>ご退会後、再び本サービスを利用される場合には、改めて会員登録をして頂くかまたは、注文時にお名前、 ご住所、などの入力をして頂くことになりますのでご了承ください。</p>
		<p class="attention">※退会すると会員情報が削除され購入履歴などは二度と見ることができません。</p>
	</div>
	<div class="button">
		<div class="button-next">
			<% if (IsWithdrawalLimit(this.LoginUserId)) { %>
			<div runat="server" Visible="True" style="color:#ff0000; font-size:13px; line-height: 18px" align="left">
				有効な定期購入情報がございます。<br />
				定期購入情報の解約手続きを先に行ってから、退会を行ってください。
			</div>
			<% } else { %>
			<asp:LinkButton ID="lbWithdrawal" Text="退会する" CssClass="btn" runat="server" OnClick="lbWithdrawal_Click" OnClientClick="return confirm('本当によろしいですか？');" />
			<% } %>
		</div>
	</div>
</div>

<div class="user-footer">
	<div class="button-next">
		<a href="<%= WebSanitizer.HtmlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_MYPAGE) %>" class="btn">マイページトップへ</a>
	</div>
</div>

</section>
</asp:Content>