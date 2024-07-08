<%--
=========================================================================================================
  Module      : 特集ページカテゴリ一覧出力コントローラ(BodyFeaturePageCategoryLinks.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/FeaturePage/BodyFeaturePageCategoryLinks.ascx.cs" Inherits="Form_Common_FeaturePage_BodyFeaturePageCategoryLinks" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
タイトルタグはカスタムパーツのみ利用します。
<%@ Page Title="無名のパーツ" %>
<%@ FileInfo LastChanged="最終更新者" %>

--%>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<div class="feature-page-parts-category-links">
	<p class="feature-page-parts-category-links-title"><%: this.HasFeaturePageCategoryData ? this.FeaturePageCategoryData.CategoryName : "カテゴリ" %></p>
	<p class="feature-page-parts-category-links-description"><%= WebSanitizer.HtmlEncodeChangeToBr(this.HasFeaturePageCategoryData ? this.FeaturePageCategoryData.CategoryOutline : "") %></p>
	<ul class="feature-page-parts-category-links-items">
		<asp:Repeater ID="rFeaturePageCategoryList" ItemType="w2.Domain.FeaturePageCategory.FeaturePageCategoryModel" runat="server">
			<ItemTemplate>
				<a class="feature-page-parts-category-links-item <%#: IsActiveCategory(Item.CategoryId) ? "feature-page-parts-category-links-item-isactive" : "" %>" href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateFeaturePageCategoryLink(Item.CategoryId)) %>"><%#: Item.CategoryName %></a>
			</ItemTemplate>
		</asp:Repeater>
	</ul>
</div>
<%-- △編集可能領域△ --%>