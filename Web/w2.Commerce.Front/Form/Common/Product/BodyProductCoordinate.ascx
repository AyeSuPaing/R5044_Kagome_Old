<%--
=========================================================================================================
Module      : 商品コーディネート出力コントローラー(BodyProductCoordinate.ascx)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Product/BodyProductCoordinate.ascx.cs" Inherits="Form_Common_Product_BodyProductCoordinate" %>
<%@ Import Namespace="w2.Domain.Coordinate" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
タイトルタグはカスタムパーツのみ利用します。
<%@ Page Title="無名のパーツ" %>
<%@ FileInfo LastChanged="最終更新者" %>

--%>

<%-- ▽編集可能領域：プロパティ設定▽ --%>
<script runat="server">
	public new void Page_Init(Object sender, EventArgs e)
	{
		base.Page_Init(sender, e);
		this.MaxDispCount = 4;
		this.AddDispCount = 4;
	}
</script>
<%-- △編集可能領域△ --%>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<%-- ▽コーディネート一覧ループ▽ --%>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
<ContentTemplate>

<div id="ProductCoordinateList" runat="server">

<div id="dvProductReviewImage">
	<p class="title"><a>この商品を使ったコーディネート</a></p>
</div>

<asp:Repeater ID="rCoordinateList" runat="server">
	<HeaderTemplate>
		<div class="heightLineParent clearFix">
	</HeaderTemplate>
	<ItemTemplate>
		<div class="glbPlist column4">
			<ul>
				<li class="coordinatethumb">
					<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CoordinatePage.CreateCoordinateDetailUrl(Container.DataItem)) %>'>
						<img src="<%#: CoordinatePage.CreateCoordinateImageUrl(((CoordinateModel)Container.DataItem).CoordinateId , 1) %>"/></a>
				</li>
				<li class="name">
					<a href="#" class="pid" >
						<img align="left" style="padding-right: 5px;" id="picture" src="<%# CoordinatePage.GetStaffImagePath(((CoordinateModel)Container.DataItem).StaffId) %>" alt="スタッフ画像" height="40px" border="0"/>
						<p style="padding-bottom: 2px;"><%# ((CoordinateModel)Container.DataItem).StaffName %></p>
						<p style="padding-bottom: 2px;"><%# ((CoordinateModel)Container.DataItem).StaffHeight %>cm</p>
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
<div style="padding: 20px;text-align: center; clear: left;">
	<% if(this.MaxDispCount <= this.CoordinateList.Count()){ %>
	<asp:LinkButton class="btn btn-mid btn-inverse" ID="AddMaxDispCount" OnClick="AddMaxDispCount_Click" runat="server">もっと見る</asp:LinkButton>
	<% } %>
</div>
</div>
</ContentTemplate>
</asp:UpdatePanel>
<%-- △編集可能領域△ --%>