<%--
=========================================================================================================
  Module      : スマートフォン用商品バリエーション一覧画面(ProductVariationList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%@ Register TagPrefix="uc" TagName="BodyProductCategoryLinks" Src="~/SmartPhone/Form/Common/Product/BodyProductCategoryLinks.ascx" %>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Form/Product/ProductVariationList.aspx.cs" Inherits="Form_Product_ProductVariationList" Title="商品セール一覧" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
	<link href="<%= Constants.PATH_ROOT  %>SmartPhone/Css/product.css" rel="stylesheet" type="text/css" media="all" />
	<link href="<%= Constants.PATH_ROOT  %>SmartPhone/Css/cart.css" rel="stylesheet" type="text/css" media="all" />
	<link href="<%= Constants.PATH_ROOT  %>SmartPhone/Css/order.css" rel="stylesheet" type="text/css" media="all" />
	<%-- △編集可能領域△ --%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<%
	// Enter押下でサブミット ※FireFoxでは関数内からevent.keyCodeが呼べないらしい
	this.WtbClosedMarketPassword.Attributes["onkeypress"] = "if (event.keyCode==13){__doPostBack('" + lbSubmitClosedMarketPassword.UniqueID + "',''); return false;}";
%>

<%-- ▽レイアウト領域：レフトエリア▽ --%>
<%-- △レイアウト領域△ --%>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<!-- ▽商品ページコンテンツ▽ -->

<%-- カート投入ボタン押下時にどの画面へ遷移するか？ --%>
<%-- CART：カート一覧画面 その他：画面遷移しない --%>
<asp:HiddenField ID="hfIsRedirectAfterAddProduct" Value="CART" runat="server" />

<section class="wrap-product-list product-vatiation-list">

<%-- ▽パンくず▽ --%>
<uc:BodyProductCategoryLinks runat="server" />
<%-- △パンくず△ --%>

<div class="order-unit">

<h2 class="title">
	<%if (this.ProductSaleKbn == Constants.KBN_PRODUCTSALE_KBN_TIMESALES) { %>
		タイムセールス
	<%} else if (this.ProductSaleKbn == Constants.KBN_PRODUCTSALE_KBN_CLOSEDMARKET) { %>
		闇市
	<%} %>
</h2>

		<%-- 闇市パスワード入力 --%>
		<div id="divClosedmarketLogin" class="order-unit" runat="server">
			<dl class="order-form">
				<dt>闇市パスワード</dt>
				<dd class="password">
				<asp:TextBox ID="tbClosedMarketPassword" OnTextChanged="lbSubmitClosedMarketPassword_Click" MaxLength="30" runat="server"></asp:TextBox>
				</dd>
			</dl>
			<div class="order-footer">
		<%-- アラートメッセージ --%>
		<% if (StringUtility.ToEmpty(this.AlertMessage) != "") { %>
			<div class="msg-alert"><%= this.AlertMessage%></div>
		<% } %>
			<asp:LinkButton ID="lbSubmitClosedMarketPassword" CssClass="btn" OnClick="lbSubmitClosedMarketPassword_Click" runat="server">
				ログイン
			</asp:LinkButton>
			</div>
		</div>
			
		<%-- ▽ページャー▽ --%>
		<div id="spPager1" class="pager-wrap above" runat="server">
			<%= this.PagerHtml %>
		</div>
		<%-- △ページャー△ --%>

		<%-- ▽カート投入リンク（UpdatePanel）▽ --%>
		<asp:UpdatePanel ID="upCartAdd" runat="server">
		<ContentTemplate>

	<%--▽ 商品一覧ループ ▽--%>
		<asp:Repeater id="rProductList" runat="server" OnItemCommand="rProductList_ItemCommand">
		<HeaderTemplate>
			<ul class="product-list-2 clearfix">
		</HeaderTemplate>
		<ItemTemplate>
			<li>
			<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem)) %>'>
			<div class="product-image">
				<w2c:ProductImage ImageTagId="picture" ImageSize="M" ProductMaster="<%# Container.DataItem %>" IsVariation="true" runat="server" />
				<span visible='<%# ProductListUtility.IsProductSoldOut(Container.DataItem) %>' runat="server" class="sold-out">SOLD OUT</span>
			</div>
			<div class="product-name">
				<%-- ▽商品アイコン▽ --%>
				<p class="icon">
					<w2c:ProductIcon ID="ProductIcon1" IconNo="1" ProductMaster="<%# Container.DataItem %>" runat="server" />
					<w2c:ProductIcon ID="ProductIcon2" IconNo="2" ProductMaster="<%# Container.DataItem %>" runat="server" />
					<w2c:ProductIcon ID="ProductIcon3" IconNo="3" ProductMaster="<%# Container.DataItem %>" runat="server" />
					<w2c:ProductIcon ID="ProductIcon4" IconNo="4" ProductMaster="<%# Container.DataItem %>" runat="server" />
					<w2c:ProductIcon ID="ProductIcon5" IconNo="5" ProductMaster="<%# Container.DataItem %>" runat="server" />
					<w2c:ProductIcon ID="ProductIcon6" IconNo="6" ProductMaster="<%# Container.DataItem %>" runat="server" />
					<w2c:ProductIcon ID="ProductIcon7" IconNo="7" ProductMaster="<%# Container.DataItem %>" runat="server" />
					<w2c:ProductIcon ID="ProductIcon8" IconNo="8" ProductMaster="<%# Container.DataItem %>" runat="server" />
					<w2c:ProductIcon ID="ProductIcon9" IconNo="9" ProductMaster="<%# Container.DataItem %>" runat="server" />
					<w2c:ProductIcon ID="ProductIcon10" IconNo="10" ProductMaster="<%# Container.DataItem %>" runat="server" />
				</p>
				<%-- △商品アイコン△ --%>
				<%# WebSanitizer.HtmlEncode(CreateProductJointName(Container.DataItem)) %>
				<%-- 商品ID --%>
				[<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>]
			</div>
			<div class="product-outline">
				<!-- 概要表示 -->
				<%# GetProductDataHtml(Container.DataItem, Constants.FIELD_PRODUCT_OUTLINE) %>
			</div>
			<div id="liReturnExchangeMessage" runat="server" visible='<%# StringUtility.ToEmpty(DataBinder.Eval(Container.DataItem, Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE)) != "" %>'>
				<!-- 返品交換文言 -->
				<strong><%# WebSanitizer.HtmlEncodeChangeToBr(Eval(Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE)) %></strong>
				（<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + ShopMessage.GetMessage("ReturnSpecialContractPage")) %>" target='<%= (ShopMessage.GetMessage("ReturnSpecialContractPage") == "#") ? "_self" : "_blank" %>' style='font-size:10px'>返品特約</a>）
			</div>
			<div class="product-price">
				<!-- 商品価格表示・税区分 -->
				<%-- 闇市価格適用時 --%>
				<p runat="server" visible='<%# ProductSaleKbn == Constants.KBN_PRODUCTSALE_KBN_CLOSEDMARKET %>' class="special">
					<%#: CurrencyManager.ToPrice(ProductPage.GetProductClosedMarketPriceNumeric(Container.DataItem)) %>（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(Container.DataItem)) %>）
					<span class="line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></span>
				</p>
				<%-- タイムセールス価格適用時 --%>
				<p runat="server" visible='<%# ProductSaleKbn == Constants.KBN_PRODUCTSALE_KBN_TIMESALES %>' class="special">
					<%#: CurrencyManager.ToPrice(ProductPage.GetProductTimeSalePriceNumeric(Container.DataItem)) %>（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(Container.DataItem)) %>)
					<span class="line-through"><%#: CurrencyManager.ToPrice(GetProductPriceNumeric(Container.DataItem, true)) %></span>
				</p>
				<%if (Constants.W2MP_POINT_OPTION_ENABLED){ %>
					<%-- 加算ポイント表示(商品がもつポイント数を使用する) --%>
					<%-- ポイント系(有効な場合) --%>
					<p>
						<%# WebSanitizer.HtmlEncode(StringUtility.AddHeaderFooter("[ポイント", GetProductAddPointString(Container.DataItem, true, true), "]")) %>
					</p>
				<%} %>
			</div>
			</a>
			<p class="button">
				<asp:LinkButton ID="lbAddCart" CssClass="btn" CommandName="AddCart" CommandArgument="<%# Container.ItemIndex %>" runat="server">	カート投入
				</asp:LinkButton>
				<p id="sErrorMessage" class="msg-alert" runat="server"></p>
				<asp:HiddenField ID="hfProductId" Value='<%# Eval(Constants.FIELD_PRODUCT_PRODUCT_ID) %>' runat="server" />
				<asp:HiddenField ID="hfVariationId" Value='<%# Eval(Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) %>' runat="server" />
				<asp:HiddenField ID="hfBuyableMemberRank" Value='<%# Eval(Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK) %>' runat="server" />
			</p>
			</li>
		</ItemTemplate>
		<FooterTemplate>
			</ul>
		</FooterTemplate>
		</asp:Repeater>
		<%--△ 商品一覧ループ △--%>

		</ContentTemplate>
		</asp:UpdatePanel>
		<%-- △カート投入リンク（UpdatePanel）△ --%>

		<%-- ▽ページャー▽ --%>
		<div id="spPager2" class="pager-wrap below" runat="server">
			<%= this.PagerHtml %>
		</div>
		<%-- △ページャー△ --%>
</div>
</section>
<%-- △編集可能領域△ --%>
<!-- △商品ページコンテンツ△ -->
<!-- ▽右メニュー▽ -->
<!-- △右メニュー△ -->
</asp:Content>