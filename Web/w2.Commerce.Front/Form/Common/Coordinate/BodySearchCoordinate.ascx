<%--
=========================================================================================================
Module      : コーディネート検索出力コントローラ(BodySearchCoordinate.ascx)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Coordinate/BodySearchCoordinate.ascx.cs" Inherits="Form_Common_Coordinate_BodySearchCoordinate" %>
<%@ Import Namespace="w2.Domain.CoordinateCategory" %>
<%@ Import Namespace="w2.Domain.RealShop" %>
<%@ Import Namespace="w2.Domain.Staff" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
タイトルタグはカスタムパーツのみ利用します。
<%@ Page Title="無名のパーツ" %>
<%@ FileInfo LastChanged="最終更新者" %>

--%>

<asp:UpdatePanel ID="UpdatePanel1" runat="server">
<ContentTemplate>
	<%
		// 検索テキストボックスEnterで検索させる（UpdatePanelで括っておかないと非同期処理時に検索が効かなくなる）
		this.WtbSearchCoordinate.Attributes["onkeypress"] = "if (event.keyCode==13){__doPostBack('" + this.WlbSearch.UniqueID + "',''); return false;}";
	%>
</ContentTemplate>
</asp:UpdatePanel>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<div style="padding-bottom: 20px">
<p class="borderTitle">検索する</p>
<div class="categoryList">
		<div class="keyword-search">
			<form action="">
				<%
					tbSearchCoordinate.Attributes["placeholder"] = "検索";
				%>
				<asp:TextBox ID="tbSearchCoordinate" runat="server" MaxLength="250" name="search" CssClass="keyword-search-input"></asp:TextBox>
				<asp:LinkButton ID="lbSearch" runat="server" OnClick="lbSearch_Click" CssClass="keyword-search-submit">
					<span class="icon-search"></span>
				</asp:LinkButton>
			</form>
		</div>
</div>
</div>

<div style="padding-bottom: 20px">
	<p class="borderTitle">カテゴリ</p>
	<div class="flexbox">
		<asp:Repeater ID="rCategoryList" runat="server">
			<ItemTemplate>
				<a class="borderlink" href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrl(Constants.REQUEST_KEY_COORDINATE_CATEGORY_ID, ((CoordinateCategoryModel)Container.DataItem).CoordinateCategoryId)) %>"><%# Eval("CoordinateCategoryName") %>,</a>&ensp;
			</ItemTemplate>
		</asp:Repeater>
	</div>
</div>
	
<div style="padding-bottom: 20px">
	<p class="borderTitle">スタッフ</p>
	<div class="flexbox">
		<asp:Repeater ID="rStaffList" runat="server">
			<ItemTemplate>
				<a class="borderlink" href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrl(Constants.REQUEST_KEY_COORDINATE_STAFF_ID, ((StaffModel)Container.DataItem).StaffId)) %>"><%# Eval("StaffName") %>,</a>&ensp;
			</ItemTemplate>
		</asp:Repeater>
	</div>
</div>
	
<div style="padding-bottom: 20px">
	<p class="borderTitle">店舗</p>
	<div class="flexbox">
		<asp:Repeater ID="rRealShopList" runat="server">
			<ItemTemplate>
				<a class="borderlink"  href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrl(Constants.REQUEST_KEY_REAL_SHOP_ID, ((RealShopModel)Container.DataItem).RealShopId)) %>"><%# Eval("name") %>,</a>&ensp;
			</ItemTemplate>
		</asp:Repeater>
	</div>
</div>
<%-- △編集可能領域△ --%>