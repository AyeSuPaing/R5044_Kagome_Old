<%--
=========================================================================================================
  Module      : スマートフォン用特集ページ一覧出力コントローラ(BodyFeaturePageList.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/FeaturePage/BodyFeaturePageList.ascx.cs" Inherits="Form_Common_FeaturePage_BodyFeaturePageList" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
タイトルタグはカスタムパーツのみ利用します。
<%@ Page Title="無名のパーツ" %>
<%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>

<script runat="server">
public new void Page_Init(Object sender, EventArgs e)
{
base.Page_Init(sender, e);

<%-- ▽編集可能領域：プロパティ設定▽ --%>
// 最大表示件数を指定します
this.MaxDispCount = 3;

// 特集ページカテゴリの親カテゴリIDを指定します
this.FeaturePageParentCategoryId = "";
<%-- △編集可能領域△ --%>
}
</script>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<asp:Repeater ID="rFeaturePageList" ItemType="w2.Domain.FeaturePage.FeaturePageModel" runat="server">
	<HeaderTemplate>
		<div class="unit feature-page-parts-list">
		<% if (this.FeaturePageCategoryData != null) { %>
			<h3><%: this.FeaturePageCategoryData.CategoryName %></h3>
		<% } %>
			<div class="feature-page-parts-items">
	</HeaderTemplate>
	<ItemTemplate>
				<div class="feature-page-parts-item">
					<ul>
						<li class="feature-page-parts-thumb">
							<a href='<%# WebSanitizer.UrlAttrHtmlEncode(Item.FeaturePagePath) %>'>
								<img src="<%# WebSanitizer.UrlAttrHtmlEncode(Item.SpBannerImageSrc) %>" />
							</a>
						</li>
						<li class="feature-page-parts-title">
							<p><%# Item.SpPageTitle %></p>
						</li>
					</ul>
				</div>
	</ItemTemplate>
	<FooterTemplate>
			</div>
			<div class="feature-page-parts-viewmore"><a class="btn" href="<%: this.ViewMoreUrl %>">もっと見る</a></div>
		</div>
	</FooterTemplate>
</asp:Repeater>
<%-- △編集可能領域△ --%>