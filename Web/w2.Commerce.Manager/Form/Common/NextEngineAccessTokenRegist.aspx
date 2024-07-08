<%--
=========================================================================================================
  Module      : ネクストエンジンアクセストークン取得＆更新ページ(NextEngineAccessTokenRegist.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="NextEngineAccessTokenRegist.aspx.cs" Inherits="Form_NextEngineAccessToken_NextEngineAccessTokenRegist" Title="ネクストエンジンアクセストークン取得" %>
<%@ Import Namespace="w2.Common.Web" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<h1 class="page-title">ネクストエンジンアクセストークン</h1>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr></tr><asp:Literal runat="server" ID="lNextEngineAccessTokenStatus">現在有効なアクセストークンはありません。</asp:Literal>
	<tr>
		<td colspan="2"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
