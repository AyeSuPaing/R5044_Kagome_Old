<%--
=========================================================================================================
  Module      : コーディネートトップ画面(CoordinateTop.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Register TagPrefix="uc" TagName="BodyCoordinateCategoryTree" Src="~/Form/Common/Coordinate/BodyCoordinateCategoryTree.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyCoordinateRanking" Src="~/Form/Common/Coordinate/BodyCoordinateRanking.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyCoordinateList" Src="~/Form/Common/Coordinate/BodyCoordinateList.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyStaffList" Src="~/Form/Common/BodyStaffList.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyCoordinateListByFollowStaff" Src="~/Form/Common/Coordinate/BodyCoordinateListByFollowStaff.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Form/Coordinate/CoordinateTop.aspx.cs" Inherits="Form_Coordinate_CoordinateTop" Title="コーディネートトップページ"%>
<%@ Register TagPrefix="uc" TagName="BodySearchCoordinate" Src="~/Form/Common/Coordinate/BodySearchCoordinate.ascx" %>
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

	<%-- ▽コーディネート一覧ループ▽ --%>
	<uc:BodyCoordinateList runat="server" />
	<%-- △コーディネート一覧ループ△ --%>
	
	<%-- ▽コーディネート一覧ループ▽ --%>
	<uc:BodyCoordinateListByFollowStaff runat="server" />
	<%-- △コーディネート一覧ループ△ --%>
	
	<%-- ▽コーディネートランキング▽ --%>
	<uc:BodyCoordinateRanking runat="server" />
	<%-- △コーディネートランキング△ --%>
	
	<%-- ▽スタッフ一覧▽ --%>
	<uc:BodyStaffList runat="server" />
	<%-- △スタッフ一覧△ --%>

	<%-- ▽コーディネート検索▽ --%>
	<uc:BodySearchCoordinate  runat="server" />
	<%-- △コーディネート検索△ --%>
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
<script type='text/javascript'>
	window.onload = function() {
		window.dispatchEvent(new Event('resize'));
	}
</script>
</asp:Content>