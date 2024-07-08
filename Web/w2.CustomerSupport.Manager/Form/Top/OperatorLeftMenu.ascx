<%--
=========================================================================================================
  Module      : オペレータ左メニューユーザーコントロール(OperatorLeftMenu.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OperatorLeftMenu.ascx.cs" Inherits="Form_Top_OperatorLeftMenu" %>
<%@ Import Namespace="w2.App.Common.Manager.Menu" %>

<% if (this.LoginOperatorCsInfo != null) { %>

<!-- インシデント -->
<asp:Repeater id="rLargeMenuIncident" ItemType="MenuUtility.MenuLargeCs" Runat="server">
	<ItemTemplate>
		<li class="sidemenu-list open">
			<p class="sidemenu-list-label"><a href="javascript:void(0)"><span class="icon"><span class="<%# Item.IconCss %>"></span></span><span class="label"><%#: Item.Name %></span></a></p>
			<ul class="sidemenu-child-list">
				<li class="sidemenu-list-parent-label"><p class="sidemenu-list-label"><a href="#"><span class="label"><%#: Item.Name %></span></a></p></li>
				<asp:Repeater ID="rSmallMenu" DataSource='<%# Item.SmallMenus %>' ItemType="MenuUtility.MenuSmallCs" Runat="server">
				<ItemTemplate>
					<li class="sidemenu-list <%#: IsSameTaskStatusMode(Item) ? "current" : "" %>"><p class="sidemenu-list-label"><a href="<%#: Item.Href %>"><%#: Item.Name %><span class="count"><%# GetTaskCountString(Item.GetTaskCount(this.LoginOperatorId, TaskTargetMode.Personal)) %>(<%# GetTaskCountString(Item.GetTaskCount(this.LoginOperatorId, TaskTargetMode.Group)) %>)</span></a></p></li>
				</ItemTemplate>
				</asp:Repeater>
			</ul>
		</li>
	</ItemTemplate>
</asp:Repeater>

<!-- カテゴリツリー -->
<li class="sidemenu-list open">
	<p class="sidemenu-list-label"><a href="javascript:void(0)"><span class="icon"><span class="icon-category"></span></span><span class="label">カテゴリ</span></a></p>
	<ul class="sidemenu-child-list">
	<li class="sidemenu-list-parent-label"><p class="sidemenu-list-label"><a href="#"><span class="label">カテゴリ</span></a></p></li>
	<div class="tree_table">
		<asp:UpdatePanel runat="server">
			<ContentTemplate>
				<p class="sidemenu-list-label">
					<a href="#">
						<span class="label">
							<asp:TreeView ID="tvCategoryTree" runat="server" ImageSet="Custom" SelectedNodeStyle-CssClass="current" Font-Size="13px"></asp:TreeView>
						</span>
					</a>
				</p>
			</ContentTemplate>
		</asp:UpdatePanel>
	</div>
	</ul>
</li>

<!-- 各種メッセージ、検索、ゴミ箱 -->
<asp:Repeater id="rLargeMenuOperator" ItemType="MenuUtility.MenuLargeCs" Runat="server">
	<ItemTemplate>
		<li class="sidemenu-list open">
			<p class="sidemenu-list-label"><a href="javascript:void(0)"><span class="icon"><span class="<%# Item.IconCss %>"></span></span><span class="label"><%#: Item.Name %></span></a></p>
			<ul class="sidemenu-child-list">
				<li class="sidemenu-list-parent-label"><p class="sidemenu-list-label"><a href="#"><span class="label"><%#: Item.Name %></span></a></p></li>
				<asp:Repeater ID="rSmallMenu" DataSource='<%# Item.SmallMenus %>' ItemType="MenuUtility.MenuSmallCs" Runat="server">
				<ItemTemplate>
					<li class="sidemenu-list <%#: IsSameCsKbn(Item) ? "current" : "" %>">
						<p class="sidemenu-list-label">
							<a href="<%#: Item.Href %>"><%#: Item.Name %>
								<span class="count"><%# GetTaskCountString(Item.GetTaskCount(this.LoginOperatorId, TaskTargetMode.Personal)) %></span>
							</a>
						</p>
					</li>
				</ItemTemplate>
				</asp:Repeater>
			</ul>
		</li>
	</ItemTemplate>
</asp:Repeater>

<% } %>