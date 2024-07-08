<%--
=========================================================================================================
  Module      : Shop List (ShopList.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Form/RealShop/ShopList.aspx.cs" Inherits="Form_RealShop_ShopList" Title="店舗一覧" %>
<%@ Import Namespace="w2.App.Common.RealShop" %>
<%@ Import Namespace="w2.Domain.RealShop" %>
<%@ Import Namespace="w2.Domain.ProductBrand" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="最終更新者" %>

--%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
	<%-- ▽編集可能領域：HEAD追加部分▽ --%>
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
				<div class="wrap">
					<h2>店舗一覧</h2>
					<div id="dvRealShopContents">
						<div class="tags">
							<ul class="areaTags" style="flex-wrap: nowrap">
								<asp:Repeater ID="rAreaTags" ItemType="RealShopArea" runat="server">
									<ItemTemplate>
										<li id="<%#: Item.AreaId %>">
											<a href="<%#: CreateAreaLinkUrl(Item.AreaId) %>">
												<%#: Item.AreaName %>
											</a>
										</li>
									</ItemTemplate>
								</asp:Repeater>
							</ul>
						</div>
						<div id="container" style="margin: 2rem 1rem;">
						<% if (Constants.PRODUCT_BRAND_ENABLED) { %>
							<ul class="brandList">
								<asp:Repeater ID="rBrandList" ItemType="ProductBrandModel" runat="server">
									<ItemTemplate>
										<li id="<%#: Item.BrandId %>" class="brandItem">
											<span class="brandName"><%#: Item.BrandName %></span>
											<ul class="realShopList">
												<asp:Repeater ID="rRealShopList" DataSource="<%# GetRealShops(Item.BrandId) %>" ItemType="RealShopModel" runat="server">
													<ItemTemplate>
														<li class="shopDetail">
															<a href="<%#: CreateShopDetailLinkUrl(Item.RealShopId) %>">
																<div class="realShopName">
																	<%#: Item.Name %>
																</div>
																<div class="realShopAddress">
																	<p><%#: Item.Addr %></p>
																	<p><%#: Item.Tel %></p>
																</div>
																<div class="arrowNext"></div>
															</a>
														</li>
													</ItemTemplate>
												</asp:Repeater>
											</ul>
										</li>
									</ItemTemplate>
								</asp:Repeater>
							</ul>
						<% } else { %>
							<ul class="realShopList">
								<asp:Repeater ID="rRealShopList" DataSource="<%# GetRealShops(this.BrandId) %>" ItemType="RealShopModel" runat="server">
									<ItemTemplate>
										<li class="shopDetail">
											<a href="<%#: CreateShopDetailLinkUrl(Item.RealShopId) %>">
												<div class="realShopName">
													<%#: Item.Name %>
												</div>
												<div class="realShopAddress">
													<p><%#: Item.Addr %></p>
													<p><%#: Item.Tel %></p>
												</div>
												<div class="arrowNext"></div>
											</a>
										</li>
									</ItemTemplate>
								</asp:Repeater>
							</ul>
						<% } %>
						</div>
					</div>
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
	<script type="text/javascript">
		// Add class to area tags
		if ('<%# this.AreaId %>') {
			$('.areaTags #<%# this.AreaId %>').addClass('active');
		}
	</script>
</asp:Content>
