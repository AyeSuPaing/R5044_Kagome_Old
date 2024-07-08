<%--
=========================================================================================================
  Module      : 会員退会完了画面(UserWithdrawalComplete.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserWithdrawalComplete.aspx.cs" Inherits="Form_User_UserWithdrawalComplete" Title="退会完了ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserFltContents">
	<%-- 会員退会変更系パンくず --%>
	<div id="dvHeaderWithdrawClumbs">
		<p>
			<img src="../../Contents/ImagesPkg/user/clumbs_withdraw_2.gif" alt="退会の受付完了" /></p>
	</div>

		<h2>退会の受付完了</h2>

	<div class="dvWithdrawInfo">
		<p class="completeInfo">
			退会処理が完了いたしました。<br />
			<%= WebSanitizer.HtmlEncode(ShopMessage.GetMessage("ShopName")) %>をご利用頂き、真に有り難うございました。</p>
	</div>
	<div class="dvUserBtnBox">
		<p>
			<span><asp:LinkButton ID="lbTopPage" runat="server" OnClick="lbTopPage_Click" class="btn btn-large btn-inverse">
				トップページへ</asp:LinkButton></span>
		</p>
	</div>
</div>
</asp:Content>