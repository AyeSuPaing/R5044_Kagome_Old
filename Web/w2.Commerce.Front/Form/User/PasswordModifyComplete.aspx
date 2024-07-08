<%--
=========================================================================================================
  Module      : パスワード変更完了画面(PasswordModifyComplete.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/PasswordModifyComplete.aspx.cs" Inherits="Form_User_PasswordModifyComplete" Title="パスワード変更完了ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserContents">

		<h2>変更完了</h2>

	<div id="dvPasswordReminderComplete" class="unit">
		<p class="completeInfo"><span><strong>パスワード変更を完了いたしました。</strong></span>
		<br /><br />
		今後とも、「<%= WebSanitizer.HtmlEncode(ShopMessage.GetMessage("ShopName")) %>」をどうぞ宜しくお願い申し上げます。<br />
		</p>
		<p class="receptionInfo">
			<%= ShopMessage.GetMessageHtmlEncodeChangeToBr("ContactCenterInfo") %>
		</p>
		<div class="dvUserBtnBox">
			<p><span><asp:LinkButton ID="lbTopPage" runat="server" OnClick="lbTopPage_Click" class="btn btn-large btn-inverse">
				トップページへ</asp:LinkButton></span></p>
		</div>
	</div>
</div>
</asp:Content>
