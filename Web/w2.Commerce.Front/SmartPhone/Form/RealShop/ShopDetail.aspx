<%--
=========================================================================================================
  Module      : Shop Detail (ShopDetail.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Form/RealShop/ShopDetail.aspx.cs" Inherits="Form_RealShop_ShopDetail" Title="店舗明細" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="最終更新者" %>

--%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
	<%-- ▽編集可能領域：HEAD追加部分▽ --%>
	<%-- △編集可能領域△ --%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<div id="divTopArea">
		<%-- ▽レイアウト領域：トップエリア▽ --%>
		<%-- △レイアウト領域△ --%>
	</div>

	<%-- ▽編集可能領域：コンテンツ▽ --%>
	<div class="shopdetail-unit">
		<h2 style="word-break: break-all;">
			<%#: this.RealShop.Name %>
		</h2>
		<p class="msg">
			※店舗の混雑状況によりお電話が繋がらない場合がございます。<br />
			ご迷惑をお掛けしますが、予めご了承ください。
		</p>
		<dl class="shopdetail-form">
			<dt>住所</dt>
			<dd>
				<%#: this.RealShop.Addr %>
			</dd>
			<dt>電話番号</dt>
			<dd>
				<%#: this.RealShop.Tel %>
			</dd>
			<dt>営業時間</dt>
			<dd>
				<%#: this.RealShop.OpeningHours %>
			</dd>
			<dt>店舗</dt>
			<dd>
				<a href="<%#: this.RealShop.Url %>" style="text-decoration: underline"><%#: this.RealShop.Name %></a>
			</dd>
			<dt>説明</dt>
			<!-- 説明1 -->
			<dd>
				<%# GetRealShopDataHtml("desc1_sp") %>
			</dd>
			<!-- 説明2 -->
			<dd>
				<%# GetRealShopDataHtml("desc2_sp") %>
			</dd>
		</dl>
		<hr />
	</div>
	<div class="clearfix shopdetail-unit" visible="<%# this.HasSettingGoogleMap %>" runat="server">
		<iframe style="border: 0; width: 100%;" src="<%# CreateGoogleMapUrl() %>"></iframe>
	</div>
	<hr />
	<div class="shopdetail-footer">
		<div class="button">
			<a class="btn" href="<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_SHOP_LIST %>">戻る</a>
		</div>
	</div>
	<%-- △編集可能領域△ --%>

	<div id="divBottomArea">
		<%-- ▽レイアウト領域：ボトムエリア▽ --%>
		<%-- △レイアウト領域△ --%>
	</div>
</asp:Content>
