<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Global/GlobalChangeList.ascx.cs" Inherits="Form_Common_Global_GlobalChangeList" %>
<% if (Constants.GLOBAL_OPTION_ENABLE)
   { %>
<link id="lGlobalChangeMenuCss" rel="stylesheet" href="<%: Constants.PATH_ROOT %>SmartPhone/Css/globalChangeMenu.css" type="text/css" media="screen" />
<section class="header-toggle toggle-global popup_window">
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
	<div class="button">
		<a href="javascript:void(0);" class="close btn">閉じる</a>
	</div>
</section>
<% } %>