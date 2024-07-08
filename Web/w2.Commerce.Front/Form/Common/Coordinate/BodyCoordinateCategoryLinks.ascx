<%--
=========================================================================================================
  Module      : コーディネートカテゴリリスト出力コントローラ(BodyCoordinateCategoryLinks.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="c#" Inherits="Form_Common_Coordinate_BodyCoordinateCategoryLinks" CodeFile="~/Form/Common/Coordinate/BodyCoordinateCategoryLinks.ascx.cs" %>
<%@ Import Namespace="w2.Domain.CoordinateCategory" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
タイトルタグはカスタムパーツのみ利用します。
<%@ Page Title="無名のパーツ" %>
<%@ FileInfo LastChanged="最終更新者" %>

--%>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<asp:Repeater id="rCategoriesLink" runat="server">
	<HeaderTemplate><ul><li><li><a href="<%= Constants.PATH_ROOT %>Form/Coordinate/CoordinateTop.aspx">TOP</a></li></li>
	</HeaderTemplate>
	<ItemTemplate>
		<li>
			<span>&raquo;</span>
			<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrl(Constants.REQUEST_KEY_COORDINATE_CATEGORY_ID, ((CoordinateCategoryModel)Container.DataItem).CoordinateCategoryId)) %>'>
				<%#:((CoordinateCategoryModel)Container.DataItem).CoordinateCategoryName %></a>
		</li>
	</ItemTemplate>
	<FooterTemplate>
	</ul>
	</FooterTemplate>
</asp:Repeater>
<%-- △編集可能領域△ --%>