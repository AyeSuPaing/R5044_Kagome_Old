<%--
=========================================================================================================
Module      : スマートフォン用コーディネートカテゴリツリー出力コントローラ(BodyCoordinateCategoryTree.ascx)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Coordinate/BodyCoordinateCategoryTree.ascx.cs" Inherits="Form_Common_Coordinate_BodyCoordinateCategoryTree" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
タイトルタグはカスタムパーツのみ利用します。
<%@ Page Title="無名のパーツ" %>
<%@ FileInfo LastChanged="最終更新者" %>

--%>

<%-- ▽編集可能領域：コンテンツ▽ --%>
	<section class="search-category unit">
		<h3 class="title">カテゴリーから探す</h3>
		<nav>
			<asp:Repeater ID="rCategoryList" runat="server">
				<HeaderTemplate><ul class="global-nav-2 clearfix"><!--</HeaderTemplate>
					<ItemTemplate>
					--><li><a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrl(Constants.REQUEST_KEY_COORDINATE_CATEGORY_ID, ((CoordinateCategoryTreeNode)Container.DataItem).CategoryId)) %>'>
						<%# WebSanitizer.HtmlEncode(((CoordinateCategoryTreeNode)Container.DataItem).CategoryName) %></a><!--
					</ItemTemplate>
					<FooterTemplate>--></ul></FooterTemplate>
			</asp:Repeater>
		</nav>
	</section>
<%-- △編集可能領域△ --%>