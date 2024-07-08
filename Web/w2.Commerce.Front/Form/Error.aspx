<%--
=========================================================================================================
  Module      : エラー画面(Error.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Form/Error.aspx.cs" Inherits="Form_Error" Title="エラーページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<meta http-equiv="pragma" content="no-cache">
<%-- △編集可能領域△ --%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvErrorInfoBox">
	<h2 id="hErrorTitle" runat="server">エラー情報</h2>
	<div id="dvErrorInfo">
		<h6 id="hErrorContent" runat="server">エラーが発生しました。</h6>
		<div id="dvErrorContents" runat="server" class="dvErrorContents"></div>
	</div>
</div>
<div class="dvErrorBtnBox">
	<p>
		<span id="spGoBack" runat="server" visible="true">
			<a id="aGoBack" href="Javascript:history.back();" runat="server" class="btn-org btn-large btn-org-gry">
				戻る</a>
		</span>
		<span id="spGoTop" runat="server" visible="false">
			<asp:LinkButton ID="lbGoTop" Runat="server" onclick="lbGoTop_Click" class="btn btn-large btn-inverse">
				トップページへ</asp:LinkButton>
		</span>
	</p>
</div>
</asp:Content>