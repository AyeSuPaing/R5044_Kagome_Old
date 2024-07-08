<%--
=========================================================================================================
Module      : コーディネートカテゴリツリー出力コントローラ(BodyCoordinateCategoryTree.ascx)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Coordinate/BodyCoordinateCategoryTree.ascx.cs" Inherits="Form_Common_Coordinate_BodyCoordinateCategoryTree" %>
<%@ Import Namespace="w2.Domain.Staff" %>
<%@ Import Namespace="w2.Domain.RealShop" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
タイトルタグはカスタムパーツのみ利用します。
<%@ Page Title="無名のパーツ" %>
<%@ FileInfo LastChanged="最終更新者" %>

--%>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<%-- １階層目 --%>
<asp:Repeater ID="rCategoryList" runat="server">
<HeaderTemplate>
<div class="categoryList">
<a href="javascript:void(0);" class="title toggle active">カテゴリから探す</a>
<ul>
</HeaderTemplate>
<ItemTemplate>
	<li><a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrl(Constants.REQUEST_KEY_COORDINATE_CATEGORY_ID, ((CoordinateCategoryTreeNode)Container.DataItem).CategoryId)) %>'>
		<%# WebSanitizer.HtmlEncode(((CoordinateCategoryTreeNode)Container.DataItem).CategoryName) %></a>
	<%-- ２階層目 --%>
	<asp:Repeater ID="Repeater1" DataSource='<%# ((CoordinateCategoryTreeNode)Container.DataItem).Childs %>' Visible='<%# ((CoordinateCategoryTreeNode)Container.DataItem).Childs.Count != 0 %>' runat="server">
	<HeaderTemplate><ul></HeaderTemplate>
	<ItemTemplate>
	<li><a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrl(Constants.REQUEST_KEY_COORDINATE_CATEGORY_ID, ((CoordinateCategoryTreeNode)Container.DataItem).CategoryId)) %>'>┗
		<%# WebSanitizer.HtmlEncode(((CoordinateCategoryTreeNode)Container.DataItem).CategoryName) %></a>
		<%-- ３階層目 --%>
		<asp:Repeater ID="Repeater2" DataSource='<%# ((CoordinateCategoryTreeNode)Container.DataItem).Childs %>' Visible='<%# ((CoordinateCategoryTreeNode)Container.DataItem).Childs.Count != 0 %>' runat="server">
		<HeaderTemplate><ul></HeaderTemplate>
		<ItemTemplate>
			<li><a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrl(Constants.REQUEST_KEY_COORDINATE_CATEGORY_ID, ((CoordinateCategoryTreeNode)Container.DataItem).CategoryId)) %>'>┗
				<%# WebSanitizer.HtmlEncode(((CoordinateCategoryTreeNode)Container.DataItem).CategoryName) %></a>
			<%-- ４階層目 --%>
			<asp:Repeater ID="Repeater3" DataSource='<%# ((CoordinateCategoryTreeNode)Container.DataItem).Childs %>' Visible='<%# ((CoordinateCategoryTreeNode)Container.DataItem).Childs.Count != 0 %>' runat="server">
			<HeaderTemplate><ul></HeaderTemplate>
			<ItemTemplate>
				<li><a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrl(Constants.REQUEST_KEY_COORDINATE_CATEGORY_ID, ((CoordinateCategoryTreeNode)Container.DataItem).CategoryId)) %>'>┗
					<%# WebSanitizer.HtmlEncode(((CoordinateCategoryTreeNode)Container.DataItem).CategoryName) %></a>
				<%-- ５階層目 --%>
				<asp:Repeater ID="Repeater4" DataSource='<%# ((CoordinateCategoryTreeNode)Container.DataItem).Childs %>' Visible='<%# ((CoordinateCategoryTreeNode)Container.DataItem).Childs.Count != 0 %>' runat="server">
				<HeaderTemplate><ul></HeaderTemplate>
				<ItemTemplate>
					<li><a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrl(Constants.REQUEST_KEY_COORDINATE_CATEGORY_ID, ((CoordinateCategoryTreeNode)Container.DataItem).CategoryId)) %>'>┗
						<%# WebSanitizer.HtmlEncode(((CoordinateCategoryTreeNode)Container.DataItem).CategoryName) %></a>
					<%-- ６階層目 --%>
					<asp:Repeater ID="Repeater5" DataSource='<%# ((CoordinateCategoryTreeNode)Container.DataItem).Childs %>' Visible='<%# ((CoordinateCategoryTreeNode)Container.DataItem).Childs.Count != 0 %>' runat="server">
					<HeaderTemplate><ul></HeaderTemplate>
					<ItemTemplate>
						<li><a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrl(Constants.REQUEST_KEY_COORDINATE_CATEGORY_ID, ((CoordinateCategoryTreeNode)Container.DataItem).CategoryId)) %>'>┗
							<%# WebSanitizer.HtmlEncode(((CoordinateCategoryTreeNode)Container.DataItem).CategoryName) %></a>
						<%-- ７階層目 --%>
						<asp:Repeater ID="Repeater6" DataSource='<%# ((CoordinateCategoryTreeNode)Container.DataItem).Childs %>' Visible='<%# ((CoordinateCategoryTreeNode)Container.DataItem).Childs.Count != 0 %>' runat="server">
						<HeaderTemplate><ul></HeaderTemplate>
						<ItemTemplate>
							<li><a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrl(Constants.REQUEST_KEY_COORDINATE_CATEGORY_ID, ((CoordinateCategoryTreeNode)Container.DataItem).CategoryId)) %>'>┗
								<%# WebSanitizer.HtmlEncode(((CoordinateCategoryTreeNode)Container.DataItem).CategoryName) %></a>
							<%-- ８階層目 --%>
							<asp:Repeater ID="Repeater7" DataSource='<%# ((CoordinateCategoryTreeNode)Container.DataItem).Childs %>' Visible='<%# ((CoordinateCategoryTreeNode)Container.DataItem).Childs.Count != 0 %>' runat="server">
							<HeaderTemplate><ul></HeaderTemplate>
							<ItemTemplate>
								<li><a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrl(Constants.REQUEST_KEY_COORDINATE_CATEGORY_ID, ((CoordinateCategoryTreeNode)Container.DataItem).CategoryId)) %>'>┗
									<%# WebSanitizer.HtmlEncode(((CoordinateCategoryTreeNode)Container.DataItem).CategoryName) %></a>
								<%-- ９階層目 --%>
								<asp:Repeater ID="Repeater8" DataSource='<%# ((CoordinateCategoryTreeNode)Container.DataItem).Childs %>' Visible='<%# ((CoordinateCategoryTreeNode)Container.DataItem).Childs.Count != 0 %>' runat="server">
								<HeaderTemplate><ul></HeaderTemplate>
								<ItemTemplate>
									<li><a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrl(Constants.REQUEST_KEY_COORDINATE_CATEGORY_ID, ((CoordinateCategoryTreeNode)Container.DataItem).CategoryId)) %>'>┗
										<%# WebSanitizer.HtmlEncode(((CoordinateCategoryTreeNode)Container.DataItem).CategoryName) %></a>
									<%-- １０階層目 --%>
									<asp:Repeater ID="Repeater9" DataSource='<%# ((CoordinateCategoryTreeNode)Container.DataItem).Childs %>' Visible='<%# ((CoordinateCategoryTreeNode)Container.DataItem).Childs.Count != 0 %>' runat="server">
									<HeaderTemplate><ul></HeaderTemplate>
									<ItemTemplate>
										<li><a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrl(Constants.REQUEST_KEY_COORDINATE_CATEGORY_ID, ((CoordinateCategoryTreeNode)Container.DataItem).CategoryId)) %>'>┗
											<%# WebSanitizer.HtmlEncode(((CoordinateCategoryTreeNode)Container.DataItem).CategoryName) %></a></li>
									</ItemTemplate>
									<FooterTemplate></ul></FooterTemplate>
									</asp:Repeater>
									</li>
								</ItemTemplate>
								<FooterTemplate></ul></FooterTemplate>
								</asp:Repeater>
								</li>
							</ItemTemplate>
							</asp:Repeater>
							</li>
						</ItemTemplate>
						<FooterTemplate></ul></FooterTemplate>
						</asp:Repeater>
						</li>
					</ItemTemplate>
					<FooterTemplate></ul></FooterTemplate>
					</asp:Repeater>
					</li>
				</ItemTemplate>
				<FooterTemplate></ul></FooterTemplate>
				</asp:Repeater>
				</li>
			</ItemTemplate>
			<FooterTemplate></ul></FooterTemplate>
			</asp:Repeater>
			</li>
		</ItemTemplate>
		<FooterTemplate></ul></FooterTemplate>
		</asp:Repeater>
		</li>
	</ItemTemplate>
	<FooterTemplate></ul></FooterTemplate>
	</asp:Repeater>
	</li>
</ItemTemplate>
<FooterTemplate>
</ul>
</div>
</FooterTemplate>
</asp:Repeater>

<div class="categoryList">
	<a href="javascript:void(0);" class="title toggle">スタッフから探す</a>
	<ul>
	<asp:Repeater ID="rStaffList" runat="server">
		<ItemTemplate>
		<li><a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrl(Constants.REQUEST_KEY_COORDINATE_STAFF_ID, ((StaffModel)Container.DataItem).StaffId)) %>"><%#: Eval("StaffName") %></a></li>
		</ItemTemplate>
	</asp:Repeater>
	</ul>
</div>

<div class="HeightList" runat="server">
<div class="categoryList">
	<a href="javascript:void(0);" class="title toggle">身長から探す</a>
	<ul>
		<li><a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrlForHeight("","149")) %>">～149cm</a></li>
		<li><a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrlForHeight("150","154")) %>">150cm～154cm</a></li>
		<li><a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrlForHeight("155","159")) %>">155cm～159cm</a></li>
		<li><a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrlForHeight("160","164")) %>">160cm～164cm</a></li>
		<li><a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrlForHeight("165","169")) %>">165cm～169cm</a></li>
		<li><a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrlForHeight("170","174")) %>">170cm～174cm</a></li>
		<li><a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrlForHeight("175","")) %>">175cm～</a></li>
	</ul>
</div>
</div>

<div class="categoryList">
	<a href="javascript:void(0);" class="title toggle">店舗から探す</a>
	<ul>
		<asp:Repeater ID="rRealShopList" runat="server">
			<ItemTemplate>
				<li><a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrl(Constants.REQUEST_KEY_REAL_SHOP_ID, ((RealShopModel)Container.DataItem).RealShopId)) %>"><%#: Eval("Name") %></a></li>
			</ItemTemplate>
		</asp:Repeater>
	</ul>
</div>
<%-- △編集可能領域△ --%>