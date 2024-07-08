<%--
=========================================================================================================
  Module      : スマートフォン用特集ページカテゴリパンくずリスト出力コントローラ(BodyFeaturePageCategoryBreadcrumbs.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/FeaturePage/BodyFeaturePageCategoryBreadcrumbs.ascx.cs" Inherits="Form_Common_FeaturePage_BodyFeaturePageCategoryBreadcrumbs" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
タイトルタグはカスタムパーツのみ利用します。
<%@ Page Title="無名のパーツ" %>
<%@ FileInfo LastChanged="最終更新者" %>

--%>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<div class="breadcrumbs">
	<asp:Repeater id="rFeaturePageCategoryBreadcrumbs" ItemType="w2.Domain.FeaturePageCategory.FeaturePageCategoryModel" runat="server">
		<HeaderTemplate><a href="<%= WebSanitizer.UrlAttrHtmlEncode(CreateFeaturePageCategoryLink()) %>">TOP</a>
		</HeaderTemplate>
		<ItemTemplate>
			<span>＞</span>
			<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateFeaturePageCategoryLink(Item.CategoryId)) %>'>
				<%#:Item.CategoryName %></a>
		</ItemTemplate>
		<FooterTemplate>
		</FooterTemplate>
	</asp:Repeater>
</div>
<%-- △編集可能領域△ --%>