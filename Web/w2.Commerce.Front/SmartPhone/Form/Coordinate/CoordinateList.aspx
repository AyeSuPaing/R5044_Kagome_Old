<%--
=========================================================================================================
  Module      : スマートフォン用コーディネート一覧画面(CoordinateList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Form/Coordinate/CoordinateList.aspx.cs" Inherits="Form_Coordinate_CoordinateList" Title="コーディネート一覧ページ" %>
<%@ Register TagPrefix="uc" TagName="BodySearchCoordinate" Src="~/SmartPhone/Form/Common/Coordinate/BodySearchCoordinate.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyCoordinateCategoryLinks" Src="~/Form/Common/Coordinate/BodyCoordinateCategoryLinks.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyCoordinateCategoryTree" Src="~/SmartPhone/Form/Common/Coordinate/BodyCoordinateCategoryTree.ascx" %>
<%@ Import Namespace="w2.Domain.Coordinate" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="最終更新者" %>

--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<link href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "SmartPhone/Css/product.css") %>" rel="stylesheet" type="text/css" media="all" />
<%-- △編集可能領域△ --%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
	
<%-- ▽レイアウト領域：トップエリア▽ --%>
<%-- △レイアウト領域△ --%>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<section class="wrap-product-list">
	
	<%-- ▽パンくず▽ --%>
	<div class="breadcrumbs"><uc:BodyCoordinateCategoryLinks ID="BodyCoordinateCategoryLinks1" runat="server" /></div>
	<%-- △パンくず△ --%>

<%-- ▽ページャー▽ --%>
<div class="pager-wrap above">
	<asp:Label id="lPager1" Runat="server"></asp:Label>
</div>

<div class="sort-wrap">
	<nav class="clearfix">
	<ul class="sort-nav" style="width: 100%;">
		<li><a href="javascript:void(0);" id="toggle-advance" class="btn"><i class="fa fa-angle-down"></i> 絞り込み</a></li>
	</ul>
	</nav>

	<div class="sort-toggle">
		<div class="toggle-advance">
			<uc:BodySearchCoordinate ID="BodyCoordinateSearchBox1" runat="server" />
		</div>
	</div>
</div>
<%-- △ページャー△ --%>
	<asp:Repeater ID="rCoordinateList" runat="server">
		<HeaderTemplate>
			<ul class="product-list-2 clearfix">
		</HeaderTemplate>
		<ItemTemplate>
			<li>
				<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateDetailUrl(Container.DataItem)) %>'>
					<div class="product-image">
					<img src="<%#: CreateCoordinateImageUrl((((CoordinateModel)Container.DataItem).CoordinateId), 1) %>"/></a>
					</div>
				</a>
			<div class="product-name">
				<div runat="server" Visible="<%# ShouldShowStaff((CoordinateModel)Container.DataItem) %>">
					<img style="padding-right: 5px;" align="left"  id="picture" src="<%# GetStaffImagePath(((CoordinateModel)Container.DataItem).StaffId) %>" alt="スタッフ画像" height="60px" border="0"/>
					<p><%# ((CoordinateModel)Container.DataItem).StaffName %></p>
					<p><%# ((CoordinateModel)Container.DataItem).StaffHeight %>cm</p>
				</div>
				<p><%# ((CoordinateModel)Container.DataItem).RealShopName %></p>
			</div>
			</li>
		</ItemTemplate>
		<FooterTemplate>
		</ul>
		</FooterTemplate>
	</asp:Repeater>
<%-- △コーディネート一覧ループ(通常表示)△ --%>

<div id="Div1" visible="<%# (this.CoordinateList.Count == 0) %>" runat="server" class="msg-alert"><asp:Label id="lAlertMessage" Runat="server"></asp:Label></div>

<uc:BodyCoordinateCategoryTree ID="BodyCoordinateCategoryTree1" runat="server" />

<%-- ▽ページャー▽ --%>
<div class="pager-wrap below">
	<asp:Label id="lPager2" Runat="server"></asp:Label>
</div>
<%-- △ページャー△ --%>
</section>
<%-- △編集可能領域△ --%>
	
<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<%-- △レイアウト領域△ --%>
</asp:Content>