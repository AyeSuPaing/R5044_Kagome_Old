<%--
=========================================================================================================
  Module      : 機能一覧ページ(PageIndexList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="PageIndexList.aspx.cs" Inherits="Form_PageIndexList" Title="ページ一覧" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<h1 class="page-title"><asp:Label ID="lMenuLargeName" runat="server"></asp:Label></h1>
<asp:Repeater ID="rSmallMenuCategories" ItemType="w2.App.Common.Manager.Menu.PageIndexListUtility.PageIndexSmallMenuCategory" runat="server">
<ItemTemplate>
<div class="page-index-list-wrapper" visible="<%# GetAuthorizedPage(Item.PageIndexSmallMenus).Any() %>" runat="server">
	<h2 class="cmn-hed-h2"><%#: Item.Name %></h2>
	<ul class="page-index-list">
	<asp:Repeater ID="rSmallMenus" DataSource="<%# GetAuthorizedPage(Item.PageIndexSmallMenus) %>" ItemType="w2.App.Common.Manager.Menu.PageIndexListUtility.PageIndexSmallMenu" runat="server">
	<ItemTemplate>
		<li>
			<a href="<%#: Item.Href %>">
				<span class="page-name"><%#: Item.Name %></span>
				<span class="subtext"><%# WebSanitizer.HtmlEncodeChangeToBr(Item.SubText) %></span>
			</a>
		</li>
	</ItemTemplate>
	</asp:Repeater>
	</ul>
</div>
</ItemTemplate>
</asp:Repeater>
</asp:Content>
