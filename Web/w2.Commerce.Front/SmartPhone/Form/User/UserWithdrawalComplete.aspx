<%--
=========================================================================================================
  Module      : スマートフォン用会員退会完了画面(UserWithdrawalComplete.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserWithdrawalComplete.aspx.cs" Inherits="Form_User_UserWithdrawalComplete" Title="退会完了ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-user user-withdrawal-complete">
<div class="user-unit">
	<h2>会員退会完了</h2>
	<div class="msg">
		<p>退会処理が完了いたしました。<br />
			<%= WebSanitizer.HtmlEncode(ShopMessage.GetMessage("ShopName")) %>をご利用頂き、誠にありがございました。
		</p>
	</div>
</div>
<div class="user-footer">
	<div class="button-next">
	<asp:LinkButton ID="lbTopPage" Text="トップページへ" CssClass="btn" runat="server" OnClick="lbTopPage_Click" />
	</div>
</div>
</section>
</asp:Content>