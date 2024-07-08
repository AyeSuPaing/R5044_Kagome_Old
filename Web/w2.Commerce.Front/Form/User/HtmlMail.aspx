<%--
=========================================================================================================
  Module      : Htmlメール画面(HtmlMail.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/MailPage.master" AutoEventWireup="true" CodeFile="~/Form/User/HtmlMail.aspx.cs" Inherits="HtmlMail" Title="Htmlメールページ" %>
<%@ Import Namespace="w2.Common.Web" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
	<asp:Literal ID="lMailBody" runat="server" />
</asp:Content>