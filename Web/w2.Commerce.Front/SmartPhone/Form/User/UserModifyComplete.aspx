<%--
=========================================================================================================
  Module      : 会員登録変更完了画面(UserModifyComplete.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ page language="C#" masterpagefile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserModifyComplete.aspx.cs" Inherits="Form_User_UserModifyComplete" title="登録情報変更完了ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-user user-mofify-complete">
<div class="user-unit">
	<h2>受付完了</h2>
	<div class="msg">
	<%-- メッセージ --%>
	<p><%= WebSanitizer.HtmlEncode(ShopMessage.GetMessage("ShopName")) %><br />会員情報の変更を受け付けました。<br />
			今後とも<%= WebSanitizer.HtmlEncode(ShopMessage.GetMessage("ShopName")) %>をどうぞ宜しくお願い申し上げます。</p>
	<p><%= ShopMessage.GetMessageHtmlEncodeChangeToBr("ContactCenterInfo") %></p>
	</div>
</div>
<div class="user-footer">
	<div class="button-next">
		<a href="<%= WebSanitizer.HtmlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_MYPAGE) %>" class="btn">マイページトップへ</a>
	</div>
</div>
</section>
</asp:Content>