<%--
=========================================================================================================
  Module      : タグ出力(AffiliateTag.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AffiliateTag.ascx.cs" Inherits="Form_Common_AffiliateTag" %>
<asp:Repeater ID="rAffiliateTag" runat="server">
	<ItemTemplate>
		<%# Container.DataItem %>
	</ItemTemplate>
</asp:Repeater>