<%--
=========================================================================================================
  Module      : 会員退会入力画面(UserWithdrawalInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserWithdrawalInput.aspx.cs" Inherits="Form_User_UserWithdrawalInput" Title="退会入力ページ" %>
<%@ Import Namespace="w2.Common.Web" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserFltContents">
	<%-- 会員退会変更系パンくず --%>
	<div id="dvHeaderWithdrawClumbs">
		<p>
			<img src="../../Contents/ImagesPkg/user/clumbs_withdraw_1.gif" alt="退会手続き" /></p>
	</div>
		<h2>退会手続き</h2>

	<div id="dvUserWithdrawInput" class="unit">
		<div class="dvWithdrawInfo">
			<h3>退会確認</h3>
			<div class="dvContentsInfo">
				<p><%= WebSanitizer.HtmlEncode(ShopMessage.GetMessage("ShopName")) %>をご利用いただきありがとうございました。<br />
				「退会する」ボタンを押すことで退会手続きが完了いたします。</p>

				<p>ご退会後、再び本サービスを利用される場合には、改めて会員登録をして頂くかまたは、注文時にお名前、
				ご住所、などの入力をして頂くことになりますのでご了承ください。</p>
				
				<ins>※退会すると会員情報が削除され購入履歴などは二度と見ることができません。</ins>
			</div>
		</div>
	</div>
	<div class="dvUserBtnBox">
		<p>
			<% if (IsWithdrawalLimit(this.LoginUserId)) { %>
			<div runat="server" Visible="True" style="color:#ff0000; margin-left:120px; margin-bottom:3px; font-size:15px; line-height: 20px" align="left">
				有効な定期購入情報がございます。<br />
				定期購入情報の解約手続きを先に行ってから、退会を行ってください。
			</div>
			<% } else { %>
			<span><asp:LinkButton ID="lbWithdrawal" runat="server" OnClick="lbWithdrawal_Click" OnClientClick="return confirm('本当によろしいですか？')" class="btn btn-large">退会する</asp:LinkButton></span>
			<% } %>
		</p>
	</div>
</div>
</asp:Content>