<%--
=========================================================================================================
  Module      : スマートフォン用エラー画面(Error.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Form/Error.aspx.cs" Inherits="Form_Error" Title="エラーページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<meta http-equiv="pragma" content="no-cache">
<%-- △編集可能領域△ --%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-error">
<div class="error-unit">
	<h2 id="hErrorTitle" runat="server">エラー</h2>
	<p class="msg" id="dvErrorContents" runat="server" ></p>
	<div class="button">
		<div class="button-next" id="spGoBack" runat="server" visible="true">
			<a id="aGoBack" href="Javascript:history.back();" runat="server" class="btn">戻る</a>
		</div>
		<div class="button-next" id="spGoTop" runat="server" visible="false">
			<asp:LinkButton ID="lbGoTop" Runat="server" onclick="lbGoTop_Click" CssClass="btn">トップページへ</asp:LinkButton>
		</div>
	</div>
</div>
</section>
</asp:Content>