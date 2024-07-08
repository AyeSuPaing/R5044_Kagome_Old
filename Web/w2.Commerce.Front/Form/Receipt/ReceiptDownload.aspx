<%--
=========================================================================================================
  Module      : 領収書ダウンロード画面(ReceiptDownload.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Form/Receipt/ReceiptDownload.aspx.cs" Inherits="Form_Receipt_ReceiptDownload" Title="領収書ダウンロード" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
	<%-- ▽編集可能領域：HEAD追加部分▽ --%>
	<meta http-equiv="pragma" content="no-cache" />
	<%-- △編集可能領域△ --%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
	<div id="dvReceiptDownloadBox">
		<h2>領収書ダウンロード</h2>
		<div id="dvReceiptDownload">
			<div class="error-msg" id="dvReceiptError" runat="server" visible="false">
				<asp:Literal ID="lReceiptErrorMessage" runat="server" />
			</div>
			<asp:LinkButton ID="lbReceiptDownload" runat="server" class="btn btn-large btn-inverse" OnClick="lbReceiptDownload_Click">領収書ダウンロード</asp:LinkButton>
		</div>
	</div>
</asp:Content>