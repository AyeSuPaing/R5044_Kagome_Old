<%--
=========================================================================================================
  Module      : トップページページャーユーザーコントロール(ListPager.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ListPager.ascx.cs" Inherits="Form_Top_ListPager" %>
<table border="0" cellspacing="0" cellpadding="0">
	<tr>
	<td style="width:45px; text-align:center; vertical-align:bottom;" nowrap="nowrap">
		<asp:LinkButton ID="lbPagerBack" runat="server" OnClick="lbPagerBack_Click"><img src="../../Images/Common/paging_back_01.gif" alt="前のページへ" border="0" /></asp:LinkButton>
		<img id="imgPagerBackNoLink" src="../../Images/Common/paging_back_01.gif" alt="前のページへ" border="0" runat="server" />
		<asp:LinkButton ID="lbPagerNext" runat="server" OnClick="lbPagerNext_Click"><img src="../../Images/Common/paging_next_01.gif" alt="次のページへ" border="0" /></asp:LinkButton>
		<img id="imgPagerNextNoLink" src="../../Images/Common/paging_next_01.gif" alt="次のページへ" border="0" runat="server" />
	</td>
	<td style="text-align:left; vertical-align:bottom;">
		&nbsp;
		<asp:Repeater id="rPagerPageLink" runat="server" OnItemCommand="rPagerPageLink_ItemCommand">
		<ItemTemplate>
			<span visible="<%# (int)Container.DataItem == -1 %>" runat="server"> ... </span>
			<span visible="<%# (int)Container.DataItem != -1 %>" runat="server">
			<b visible="<%# (int)Container.DataItem == this.CurrentPageNumber  %>" runat="server">&nbsp;<%# Container.DataItem %>&nbsp;</b>
			<asp:LinkButton ID="lbPagerPageLink" Visible="<%# (int)Container.DataItem != this.CurrentPageNumber %>" CommandArgument="<%# Container.DataItem %>" runat="server">&nbsp;<%# Container.DataItem %>&nbsp;</asp:LinkButton>
			</span>
		</ItemTemplate>
		<SeparatorTemplate> | </SeparatorTemplate>
		</asp:Repeater>
	</td>
	</tr>
</table>
