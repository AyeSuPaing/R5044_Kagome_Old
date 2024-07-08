<%--
=========================================================================================================
  Module      : スマートフォン用コーディネートリスト(BodyProductCoordinate.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
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
		this.MaxDispCount = 2;
		this.AddDispCount = 2;
	}
</script>
<%-- △編集可能領域△ --%>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<asp:UpdatePanel ID="UpdatePanel2" runat="server">
<ContentTemplate>
<div ID="ProductCoordinateList" runat="server">
<section class="search unit">
	<h3>この商品を使ったコーディネート</h3>
		<div style="padding-top: 10px">
			<asp:Repeater ID="rCoordinateList" runat="server">
				<HeaderTemplate>
					<ul class="product-list-2 clearfix">
				</HeaderTemplate>
				<ItemTemplate>
					<li>
						<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CoordinatePage.CreateCoordinateDetailUrl(Container.DataItem)) %>'>
							<div class="product-image">
							<img src="<%#: CoordinatePage.CreateCoordinateImageUrl((((CoordinateModel)Container.DataItem).CoordinateId), 1) %>"/></a>
					</div>
					</a>
					<div class="product-name">
						<img style="padding-right: 5px;" align="left"  id="picture" src="<%# CoordinatePage.GetStaffImagePath(((CoordinateModel)Container.DataItem).StaffId) %>" alt="スタッフ画像" height="60px" border="0"/>
						<p><%# ((CoordinateModel)Container.DataItem).StaffName %></p>
						<p><%# ((CoordinateModel)Container.DataItem).StaffHeight %>cm</p>
						<p><%# ((CoordinateModel)Container.DataItem).RealShopName %></p>
					</div>
					</li>
				</ItemTemplate>
				<FooterTemplate>
				</ul>
				</FooterTemplate>
			</asp:Repeater>
		</div>
		<div class="view-more">
			<% if(this.MaxDispCount <= this.CoordinateList.Count()){ %>
				<asp:LinkButton class="btn" OnClick="AddMaxDispCount_Click" runat="server">もっと見る</asp:LinkButton>
			<% } %>
		</div>
</section>
</div>
</ContentTemplate>
</asp:UpdatePanel>
<%-- △編集可能領域△ --%>