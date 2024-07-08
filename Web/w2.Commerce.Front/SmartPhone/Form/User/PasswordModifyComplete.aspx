<%--
=========================================================================================================
  Module      : スマートフォン用パスワード変更完了画面(PasswordModifyComplete.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/User/PasswordModifyComplete.aspx.cs" Inherits="Form_User_PasswordModifyComplete" Title="パスワード変更完了ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-order password-modify-complete">
<div class="order-unit login">
	<h2>変更完了</h2>
	<div class="msg">
		<%: ReplaceTag("@@User.password.name@@") %>変更を完了いたしました<br />
		<br />
		今後とも、「<%= WebSanitizer.HtmlEncode(ShopMessage.GetMessage("ShopName")) %>」をどうぞ宜しくお願い申し上げます。<br />
		<br />
		<%= ShopMessage.GetMessageHtmlEncodeChangeToBr("ContactCenterInfo") %>
	</div>
</div>

<div class="order-footer">
	<div class="button-next">
	<asp:LinkButton ID="lbTopPage" runat="server" OnClick="lbTopPage_Click" class="btn">トップページへ</asp:LinkButton>
	</div>
</div>

</section>
</asp:Content>
