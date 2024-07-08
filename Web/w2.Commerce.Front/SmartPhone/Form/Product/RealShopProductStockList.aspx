<%--
=========================================================================================================
  Module      : スマートフォン用リアル店舗商品在庫一覧画面(RealShopProductStockList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Register TagPrefix="uc" TagName="BodyRealShopProductStockList" Src="~/SmartPhone/Form/Common/Product/BodyRealShopProductStockList.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="~/Form/Product/RealShopProductStockList.aspx.cs" Inherits="Form_Product_RealShopProductStockList" Title="リアル店舗商品在庫一覧画面ページ" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<link href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "SmartPhone/Css/product.css") %>" rel="stylesheet" type="text/css" media="all" />
<link href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "SmartPhone/Css/cart.css") %>" rel="stylesheet" type="text/css" media="all" />
<%-- △編集可能領域△ --%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<%-- UPDATEPANELによりthickboxが動作しないバグ対応 --%>
<script type="text/javascript" language="javascript">
	function bodyPageLoad() {
		if (Sys.WebForms == null) return;
		var isAsyncPostback = Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();
		if (isAsyncPostback) { tb_init('a.thickbox, area.thickbox, input.thickbox'); }
	}
</script>
<%-- ▽レイアウト領域：レフトエリア▽ --%>
<%-- △レイアウト領域△ --%>
<%-- ▽レイアウト領域：トップエリア▽ --%>
<%-- △レイアウト領域△ --%>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<section class="real-shop-product-stock-list">
	<div class="order-unit ChangesByVariation">
		<table class="cart-table">
			<tr>
			<td class="product-image">
				<w2c:ProductImage ID="piProduct" ImageSize="L" IsVariation="true" ProductMaster="<%# this.ProductMaster %>" runat="server" />
			</td>
			<td>
				<ul>
					<li>商品名：<%# WebSanitizer.HtmlEncode(GetProductData("name")) %></li>
					<li>商品ID：<%# WebSanitizer.HtmlEncode(GetProductData("variation_id")) %></li>
				</ul>
			</td>
			</tr>
		</table>
		<%-- バリエーション有り？ --%>
		<% if (this.HasVariation){ %>
			<div class="variation">
				<p>カラーとサイズを選択してください。在庫があるショップが表示されます。</p>
				<asp:DropDownList ID="ddlVariationSelect" runat="server" OnSelectedIndexChanged="ddlVariationSelect_OnSelectedIndexChanged" DataTextField="Text" DataValueField="Value" AutoPostBack="true"></asp:DropDownList>
			</div>
		<% } %>
		<uc:BodyRealShopProductStockList ID="ucBodyRealShopProductStockList" runat="server" />
	</div>
	<div class="unit"><a href="javascript:window.close();" class="btn">閉じる</a></div>
</section>
<%-- △編集可能領域△ --%>

<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<%-- △レイアウト領域△ --%>

<%-- ▽レイアウト領域：ライトエリア▽ --%>
<%-- △レイアウト領域△ --%>
</asp:Content>