<%--
=========================================================================================================
  Module      : 注文関連ファイル取込ページ(OrderFileImport.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OrderFileImport.aspx.cs" Inherits="Form_OrderFileImport_OrderFileImport" %>
<%@ Register TagPrefix="uc" TagName="BodyOrderFileImport" Src="~/Form/Common/BodyOrderFileImport.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr id="trTitleOrderFileImportMiddle" runat="server">
		<td><h1 class="page-title">注文関連ファイル取込</h1></td>
	</tr>
	<tr id="trTitleOrderFileImportBottom" runat="server">
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ タイトル △-->
</table>
<uc:BodyOrderFileImport runat="server" />
</asp:Content>