<%--
=========================================================================================================
  Module      : スマートフォン用コーディネートカテゴリリスト出力コントローラ(BodyCoordinateCategoryLinks.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Coordinate/BodyCoordinateCategoryLinks.ascx.cs" Inherits="Form_Common_Coordinate_BodyCoordinateCategoryLinks" %>
<%@ Import Namespace="w2.Domain.CoordinateCategory" %>
<%@ Reference Control="~/Form/Common/Coordinate/BodyCoordinateCategoryLinks.ascx" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
タイトルタグはカスタムパーツのみ利用します。
<%@ Page Title="無名のパーツ" %>
<%@ FileInfo LastChanged="最終更新者" %>

--%>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<asp:Repeater id="rCategoriesLink" runat="server">
	<HeaderTemplate><div class="breadcrumbs"><a href="<%= Constants.PATH_ROOT %>Form/Coordinate/CoordinateTop.aspx">トップ</a></HeaderTemplate>
	<ItemTemplate>
		<span>＞</span>
		<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CoordinatePage.CreateCoordinateListUrl(Constants.REQUEST_KEY_COORDINATE_CATEGORY_ID, ((CoordinateCategoryModel)Container.DataItem).CoordinateCategoryId)) %>'>
			<%#:((CoordinateCategoryModel)Container.DataItem).CoordinateCategoryName %></a>
	</ItemTemplate>
	<FooterTemplate></div></FooterTemplate>
</asp:Repeater>
<%-- △編集可能領域△ --%>