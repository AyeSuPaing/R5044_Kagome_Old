<%--
=========================================================================================================
  Module      : コーディネート一覧画面(CoordinateList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Register TagPrefix="uc" TagName="BodyCoordinateCategoryTree" Src="~/Form/Common/Coordinate/BodyCoordinateCategoryTree.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyCoordinateCategoryLinks" Src="~/Form/Common/Coordinate/BodyCoordinateCategoryLinks.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodySearchCoordinate" Src="~/Form/Common/Coordinate/BodySearchCoordinate.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Form/Coordinate/CoordinateList.aspx.cs" Inherits="Form_Coordinate_CoordinateList" Title="コーディネート一覧ページ"%>
<%@ Import Namespace="w2.Domain.Coordinate" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="最終更新者" %>

--%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<script type="text/javascript" src="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "Js/jquery.elevateZoom-3.0.8.min.js") %>"></script>
<link href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "Css/product.css") %>" rel="stylesheet" type="text/css" media="all" />
<%-- △編集可能領域△ --%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table id="tblLayout" class="tblLayout_ProductList">
<tr>
<td>
<div id="secondary">
<%-- ▽レイアウト領域：レフトエリア▽ --%>
<uc:BodyCoordinateCategoryTree runat="server"/>
<%-- △レイアウト領域△ --%>
</div>
</td>
<td>
<div id="divTopArea">
<%-- ▽レイアウト領域：トップエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<div id="primary">

<!--▽ 上部カテゴリバー ▽-->
<div id="breadcrumb">
<ul>
	<uc:BodyCoordinateCategoryLinks ID="BodyCoordinateCategoryTree2" runat="server" />
</ul>
</div>
<!--△ 上部カテゴリバー △-->
<!--▽ ページャ ▽-->
<div id="pagination" class="above clearFix" style="width: 100%;">
	<asp:Label id="lPager1" runat="server"></asp:Label>
</div>
<!--△ ページャ △-->
<%-- ▽コーディネート一覧ループ▽ --%>
<asp:Repeater ID="rCoordinateList" runat="server">
<HeaderTemplate>
	<div class="heightLineParent clearFix">
</HeaderTemplate>
<ItemTemplate>
	<div class="glbPlist column4">
		<ul>
			<li class="coordinatethumb">
				<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateDetailUrl(Container.DataItem)) %>'>
					<img src="<%#: CreateCoordinateImageUrl((((CoordinateModel)Container.DataItem).CoordinateId), 1) %>"/></a>
			</li>
			<li class="name">
				<a href="#" class="pid" style="padding-top: 5px;">
					<div runat="server" Visible="<%# ShouldShowStaff((CoordinateModel)Container.DataItem) %>">
						<img style="padding-right: 5px;" align="left"  id="picture" src="<%# GetStaffImagePath(((CoordinateModel)Container.DataItem).StaffId) %>" alt="スタッフ画像" height="40px" border="0"/>
						<p style="padding-bottom: 2px;"><%# ((CoordinateModel)Container.DataItem).StaffName %></p>
						<p style="padding-bottom: 2px;"><%# ((CoordinateModel)Container.DataItem).StaffHeight %>cm</p>
					</div>
					<p style="padding-bottom: 2px;"><%# ((CoordinateModel)Container.DataItem).RealShopName %></p>
				</a>
			</li>
		</ul>
	</div>
</ItemTemplate>
<FooterTemplate>
</div>
</FooterTemplate>
</asp:Repeater>
<%-- △コーディネート一覧ループ△ --%>

<!--▽ ページャ ▽-->
<div id="pagination" class="above clearFix" style="width: 100%; padding-bottom: 20px;">
	<asp:Label id="lPager2" runat="server"></asp:Label>
</div>
<!--△ ページャ △-->
	<div visible="<%# (this.CoordinateList.Count == 0) %>" runat="server" class="noProduct">
		<!--▽ 商品が1つもなかった場合のエラー文言 ▽-->
		<asp:Label id="lAlertMessage" runat="server"></asp:Label>
		<!--△ 商品が1つもなかった場合のエラー文言 △-->
	</div>
<uc:BodySearchCoordinate ID="BodySearchCoordinate" runat="server" />


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
