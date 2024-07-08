<%--
=========================================================================================================
  Module      : 問合せ完了画面(InquiryComplete.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ page language="C#" masterpagefile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/Inquiry/InquiryComplete.aspx.cs" Inherits="Form_Inquiry_InquiryComplete" title="問合せ完了ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-user inquiry-complete">
<div class="user-unit">
	<h2>問合せ情報の受付完了</h2>
	<div class="msg">
		<%= WebSanitizer.HtmlEncode(ShopMessage.GetMessage("ShopName")) %>にお問合せ頂きありがとうございます。<br />
		今後とも、<%= WebSanitizer.HtmlEncode(ShopMessage.GetMessage("ShopName")) %>をどうぞ宜しくお願い申し上げます。<br /><br />
		<%= ShopMessage.GetMessageHtmlEncodeChangeToBr("ContactCenterInfo") %>
	</div>
</div>

<div class="user-footer">
	<div class="button-next">
		<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT) %>" Class="btn">トップページへ</a>
	</div>
</div>

</section>
</asp:Content>