<%--
=========================================================================================================
  Module      : 特集ページ一覧画面(FeaturePageList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Form/FeaturePage/FeaturePageList.aspx.cs" Inherits="Form_FeaturePage_FeaturePageList" Title="特集ページ一覧ページ" %>
<%@ Register TagPrefix="uc" TagName="BodyFeaturePageCategoryLinks" Src="~/Form/Common/FeaturePage/BodyFeaturePageCategoryLinks.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyFeaturePageCategoryBreadcrumbs" Src="~/Form/Common/FeaturePage/BodyFeaturePageCategoryBreadcrumbs.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<script type="text/javascript" src="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "Js/jquery.elevateZoom-3.0.8.min.js") %>"></script>
<link rel="stylesheet" href="<%: Constants.PATH_ROOT %>Css/featurepage.css">
<%-- △編集可能領域△ --%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
<table id="tblLayout">
<tr>
<td>
<%-- ▽レイアウト領域：レフトエリア▽ --%>
<%-- △レイアウト領域△ --%>
</td>
<td>
<div id="divTopArea">
<%-- ▽レイアウト領域：トップエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<%-- ▽特集ページカテゴリパンくずリスト ▽ --%>
<uc:BodyFeaturePageCategoryBreadcrumbs runat="server" />
<%-- △特集ページカテゴリパンくずリスト △ --%>

<%-- ▽特集ページカテゴリ一覧 ▽ --%>
<uc:BodyFeaturePageCategoryLinks runat="server" />
<%-- △特集ページカテゴリ一覧 △ --%>

<%-- ▽ページャー▽ --%>
<div class="above clearFix" style="width: 100%;">
	<asp:Label id="lPager1" runat="server"></asp:Label>
</div>
<%-- △ページャー△ --%>

<%-- ▽特集ページ一覧ループ▽ --%>
<asp:Repeater ID="rFeaturePageList" ItemType="w2.Domain.FeaturePage.FeaturePageModel" runat="server">
	<HeaderTemplate>
		<div class="unit feature-page-view-list">
			<div class="feature-page-view-items">
	</HeaderTemplate>
	<ItemTemplate>
				<div class="feature-page-view-item">
					<ul>
						<li class="feature-page-view-thumb">
							<a href='<%#: Item.FeaturePagePath %>'>
								<img src="<%#: Item.PcBannerImageSrc %>" />
							</a>
						</li>
						<li class="feature-page-view-title">
							<a href="#" class="pid">
								<p><%# Item.PcPageTitle %></p>
							</a>
						</li>
					</ul>
				</div>
	</ItemTemplate>
	<FooterTemplate>
			</div>
		</div>
	</FooterTemplate>
</asp:Repeater>
<%-- △コーディネート一覧ループ△ --%>

<%-- ▽ページャー▽ --%>
<div class="below clearFix" style="width: 100%; padding-bottom: 20px;">
	<asp:Label ID="lPager2" runat="server"></asp:Label>
</div>
<%-- △ページャー△ --%>

<div class="feature-page-view-no-item" visible="<%# (this.FeaturePageList.Length == 0) %>" runat="server" >
	<%-- ▽特集ページが1つもなかった場合のエラー文言▽ --%>
	<asp:Label ID="lAlertMessage" runat="server"></asp:Label>
	<%-- △特集ページが1つもなかった場合のエラー文言△ --%>
</div>
<%-- △編集可能領域△ --%>

<div id="divBottomArea">
<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>

</td>
<td>
<%-- ▽レイアウト領域：ライトエリア▽ --%>
<%-- △レイアウト領域△ --%>
</td>
</tr>
</table>
</asp:Content>