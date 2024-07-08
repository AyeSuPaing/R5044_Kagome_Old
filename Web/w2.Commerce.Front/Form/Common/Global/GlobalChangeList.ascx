<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GlobalChangeList.ascx.cs" Inherits="Form_Common_Global_GlobalChangeList" %>
<% if (Constants.GLOBAL_OPTION_ENABLE)
   { %>
<ul>
	<asp:Repeater ID="rRegionMenuList" runat="server" ItemType="RegionMenuViewModel">
		<ItemTemplate>
			<li>
				<a href="<%#: Item.Url %>">
					<img src="<%#: Item.Image %>"><p><%#: Item.SelectName %></p>
				</a>
			</li>
		</ItemTemplate>
	</asp:Repeater>
</ul>
<% } %>
