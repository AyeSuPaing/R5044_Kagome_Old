<%--
=========================================================================================================
  Module      : スマートフォン用登録解除画面(Unsubscribe.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Form/Mail/Unsubscribe.aspx.cs" Inherits="Form_Mail_Unsubscribe" Title="メール登録解約ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<meta http-equiv="pragma" content="no-cache">
<%-- △編集可能領域△ --%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-unsubscribe">
<div class="unsubscribe-unit">
	<h2 id="hUnsubscribeTitle" runat="server">お知らせメール配信解除</h2>
	<p class="msg">お知らせメールの配信を解除しました</p>
	<div class="button">
		<div class="button-next">
			<a href="<%: Constants.PATH_ROOT %>" class="btn">トップページへ</a>
		</div>
	</div>
</div>
</section>
</asp:Content>
