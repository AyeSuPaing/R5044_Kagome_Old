<%--
=========================================================================================================
  Module      : Shop Detail (ShopDetail.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Form/RealShop/ShopDetail.aspx.cs" Inherits="Form_RealShop_ShopDetail" Title="店舗明細" %>
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
				<div id="wrap">
					<div id="dvRealShopContents">
						<div class="shopDetail">
							<div class="shopInformation">
								<div class="header">
									<p>
										<%#: this.RealShop.Name %>
									</p>
									<p>
										※店舗の混雑状況によりお電話が繋がらない場合がございます。<br />
										ご迷惑をお掛けしますが、予めご了承ください。
									</p>
								</div>
								<div class="content">
									<table>
										<tr>
											<td>住所</td>
											<td><%#: this.RealShop.Addr %></td>
										</tr>
										<tr>
											<td>電話番号</td>
											<td><%#: this.RealShop.Tel %></td>
										</tr>
										<tr>
											<td>営業時間</td>
											<td><%#: this.RealShop.OpeningHours %></td>
										</tr>
										<tr>
											<td>店舗</td>
											<td><a href="<%#: this.RealShop.Url %>" style="text-decoration: underline"><%#: this.RealShop.Name %></a></td>
										</tr>
										<tr>
											<td>説明</td>
											<!-- 説明1 -->
											<td><%# GetRealShopDataHtml("desc1_pc") %></td>
											<!-- 説明2 -->
											<td><%# GetRealShopDataHtml("desc2_pc") %></td>
										</tr>
									</table>
								</div>
							</div>
							<div class="shopMap" visible="<%# this.HasSettingGoogleMap %>" style="display: inline-block" runat="server">
								<iframe class="shopMapDetail" src="<%# CreateGoogleMapUrl() %>"></iframe>
							</div>
						</div>
						<div class="btnBack">
							<a class="btn-org btn-large" href="<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_SHOP_LIST %>">戻る</a>
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
</asp:Content>
