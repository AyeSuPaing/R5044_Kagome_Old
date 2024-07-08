<%--
=========================================================================================================
  Module      : スマートフォン用コーディネートトップ画面(CoordinateTop.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Form/Coordinate/CoordinateList.aspx.cs" Inherits="Form_Coordinate_CoordinateList" Title="コーディネートトップページ" %>
<%@ Register TagPrefix="uc" TagName="BodyCoordinateList" Src="~/SmartPhone/Form/Common/Coordinate/BodyCoordinateList.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyCoordinateRanking" Src="~/SmartPhone/Form/Common/Coordinate/BodyCoordinateRanking.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyCoordinateListByFollowStaff" Src="~/SmartPhone/Form/Common/Coordinate/BodyCoordinateListByFollowStaff.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyCoordinateCategoryTree" Src="~/SmartPhone/Form/Common/Coordinate/BodyCoordinateCategoryTree.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyStaffList" Src="~/SmartPhone/Form/Common/BodyStaffList.ascx" %>
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
<uc:BodyCoordinateList runat="server" />

<uc:BodyCoordinateListByFollowStaff runat="server" />

<uc:BodyCoordinateRanking runat="server" />

<uc:BodyCoordinateCategoryTree runat="server" />

<uc:BodyStaffList runat="server" />
<%-- △編集可能領域△ --%>

<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<%-- △レイアウト領域△ --%>
</asp:Content>