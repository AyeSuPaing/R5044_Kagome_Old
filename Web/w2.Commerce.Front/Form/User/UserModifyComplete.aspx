<%--
=========================================================================================================
  Module      : 会員登録変更完了画面(UserModifyComplete.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserModifyComplete.aspx.cs" Inherits="Form_User_UserModifyComplete" Title="登録情報変更完了ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserFltContents">
	<%-- 会員情報変更系パンくず --%>
	<div id="dvHeaderModifyClumbs">
		<p>
			<img src="../../Contents/ImagesPkg/user/clumbs_modify_3.gif" alt="変更完了" /></p>
	</div>
		<h2>変更完了</h2>
	<div id="dvUserModifyConfirm" class="unit">
		<%-- メッセージ --%>
		<p class="completeInfo"><%= WebSanitizer.HtmlEncode(ShopMessage.GetMessage("ShopName")) %>会員情報の変更を受け付けました。<br />
			今後とも<%= WebSanitizer.HtmlEncode(ShopMessage.GetMessage("ShopName")) %>をどうぞ宜しくお願い申し上げます。</p>
		<p class="receptionInfo">
			<%= ShopMessage.GetMessageHtmlEncodeChangeToBr("ContactCenterInfo") %>
		</p>
		<div class="dvUserBtnBoxB">
			<p>
				<a href="<%= WebSanitizer.HtmlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_MYPAGE) %>" class="btn btn-large btn-inverse">マイトップページへ</a>
			</p>
		</div>
	</div>
</div>
</asp:Content>