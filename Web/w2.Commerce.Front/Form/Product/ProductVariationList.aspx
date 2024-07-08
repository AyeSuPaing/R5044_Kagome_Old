<%--
=========================================================================================================
  Module      : 商品バリエーション一覧画面(ProductVariationList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="BodyProductCategoryTree" Src="~/Form/Common/Product/BodyProductCategoryTree.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Form/Product/ProductVariationList.aspx.cs" Inherits="Form_Product_ProductVariationList" Title="商品セール一覧" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<link href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "Css/product.css")  %>" rel="stylesheet" type="text/css" media="all" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<%
	// Enter押下でサブミット ※FireFoxでは関数内からevent.keyCodeが呼べないらしい
	this.WtbClosedMarketPassword.Attributes["onkeypress"] = "if (event.keyCode==13){__doPostBack('" + lbSubmitClosedMarketPassword.UniqueID + "',''); return false;}";
%>
<table id="tblLayout" class="tblLayout_ProductList">
<tr>
<td>
<div id="secondary">
<%-- ▽レイアウト領域：レフトエリア▽ --%>
<uc:BodyProductCategoryTree runat="server" />
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

<div id="breadcrumb">
	<ul><li><a href="<%= Constants.PATH_ROOT %>">HOME</a></li>

	<li>
		<span>»</span>
		<%if (this.ProductSaleKbn == Constants.KBN_PRODUCTSALE_KBN_TIMESALES) { %>
			タイムセールス
		<%} else if (this.ProductSaleKbn == Constants.KBN_PRODUCTSALE_KBN_CLOSEDMARKET) { %>
			闇市
		<%} %>
	</li>
</ul>
</div>

<!--▽ ページャ ▽-->
<div id="pagination" class="clearFix">
<%= this.PagerHtml %>
</div>
<!--△ ページャ △-->

<div class="listProduct">

<%-- カート投入ボタン押下時にどの画面へ遷移するか？ --%>
<%-- CART：カート一覧画面 その他：画面遷移しない --%>
<asp:HiddenField ID="hfIsRedirectAfterAddProduct" Value="CART" runat="server" />

<%-- ▽カート投入リンク（UpdatePanel）▽ --%>
<asp:UpdatePanel ID="upCartAdd" runat="server">
<ContentTemplate>

		<!--↓ 商品一覧ループ -->
		<div id="dvProductListArea">
			<%-- 闇市パスワード入力 --%>
			<div id="divClosedmarketLogin" class="divClosedmarketLogin" runat="server">
				闇市パスワード&nbsp;<asp:TextBox ID="tbClosedMarketPassword" OnTextChanged="lbSubmitClosedMarketPassword_Click" MaxLength="30" runat="server" autocomplete="off"></asp:TextBox>
				<asp:LinkButton ID="lbSubmitClosedMarketPassword" OnClick="lbSubmitClosedMarketPassword_Click" runat="server" class="btn btn-mid btn-inverse">ログイン</asp:LinkButton>
			</div>

			<asp:Repeater id="rProductList" runat="server" OnItemCommand="rProductList_ItemCommand">
				<HeaderTemplate>
					<ul>
				</HeaderTemplate>
				<ItemTemplate>
				<li class="productList">
					<ul class="clearFix">
						<li class="plPhoto">
							<!-- 画像１表示 -->
							<w2c:ProductImage ImageTagId="picture" ImageSize="M" ProductMaster="<%# Container.DataItem %>" IsVariation="true" runat="server" />
							<span visible='<%# ProductListUtility.IsProductSoldOut(Container.DataItem) %>' runat="server" class="soldout">SOLDOUT</span>
						</li>
						<li class="plProductInfo">
							<ul>
								<li class="plName">
									<!-- アイコン表示 -->
									<w2c:ProductIcon id="ProductIcon1" IconNo="1" ProductMaster="<%# Container.DataItem %>" runat="server" />
									<w2c:ProductIcon id="ProductIcon2" IconNo="2" ProductMaster="<%# Container.DataItem %>" runat="server" />
									<w2c:ProductIcon id="ProductIcon3" IconNo="3" ProductMaster="<%# Container.DataItem %>" runat="server" />
									<w2c:ProductIcon id="ProductIcon4" IconNo="4" ProductMaster="<%# Container.DataItem %>" runat="server" />
									<w2c:ProductIcon id="ProductIcon5" IconNo="5" ProductMaster="<%# Container.DataItem %>" runat="server" />
									<w2c:ProductIcon id="ProductIcon6" IconNo="6" ProductMaster="<%# Container.DataItem %>" runat="server" />
									<w2c:ProductIcon id="ProductIcon7" IconNo="7" ProductMaster="<%# Container.DataItem %>" runat="server" />
									<w2c:ProductIcon id="ProductIcon8" IconNo="8" ProductMaster="<%# Container.DataItem %>" runat="server" />
									<w2c:ProductIcon id="ProductIcon9" IconNo="9" ProductMaster="<%# Container.DataItem %>" runat="server" />
									<w2c:ProductIcon id="ProductIcon10" IconNo="10" ProductMaster="<%# Container.DataItem %>" runat="server" />
									<!-- 商品名表示 -->
									<h3><%# WebSanitizer.HtmlEncode(CreateProductJointName(Container.DataItem)) %></h3>
									<!-- 商品ID表示 -->
									[<span class="productId"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %></span>]
								</li>
								<li class="plExcerpt">
									<!-- 概要表示 -->
									<%# GetProductDataHtml(Container.DataItem, Constants.FIELD_PRODUCT_OUTLINE) %>
								</li>
								<li id="liReturnExchangeMessage" runat="server" visible='<%# StringUtility.ToEmpty(DataBinder.Eval(Container.DataItem, Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE)) != "" %>'>
									<!-- 返品交換文言 -->
									<strong><%# WebSanitizer.HtmlEncodeChangeToBr(Eval(Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE)) %></strong>
									（<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + ShopMessage.GetMessage("ReturnSpecialContractPage")) %>" target='<%= (ShopMessage.GetMessage("ReturnSpecialContractPage") == "#") ? "_self" : "_blank" %>' style='font-size:10px'>返品特約</a>）
								</li>								
								<li class="plPrice">
									<!-- 商品価格表示・税区分 -->
									<%-- 闇市価格適用時 --%>
									<span runat="server" visible='<%# ProductSaleKbn == Constants.KBN_PRODUCTSALE_KBN_CLOSEDMARKET %>'>
										販売価格:<span class="productPrice"><strike><%#: CurrencyManager.ToPrice(GetProductPriceNumeric(Container.DataItem, true)) %></strike></span><br />
										<span>闇市価格:<%#: CurrencyManager.ToPrice(ProductPage.GetProductClosedMarketPriceNumeric(Container.DataItem)) %></span>
									</span>
									<%-- タイムセールス価格適用時 --%>
									<span runat="server" visible='<%# ProductSaleKbn == Constants.KBN_PRODUCTSALE_KBN_TIMESALES %>'>
										販売価格:<span class="productPrice"><strike><%#: CurrencyManager.ToPrice(GetProductPriceNumeric(Container.DataItem, true)) %></strike></span><br />
										<span>タイムセールス価格:<%#: CurrencyManager.ToPrice(ProductPage.GetProductTimeSalePriceNumeric(Container.DataItem)) %></span>
									</span>
									（<%#  WebSanitizer.HtmlEncode(GetTaxIncludeString(Container.DataItem)) %>）
									<!-- 加算ポイント表示(商品がもつポイント数を使用する) -->
									<%-- ポイント系(有効な場合) --%>
									<%if (Constants.W2MP_POINT_OPTION_ENABLED){ %>
										<span class="addPoint"><%# WebSanitizer.HtmlEncode(StringUtility.AddHeaderFooter("[ポイント", GetProductAddPointString(Container.DataItem, true, true), "]")) %></span>
									<%} %>
								</li>
								<li>
									<asp:LinkButton ID="lbAddCart" CommandName="AddCart" CommandArgument="<%# Container.ItemIndex %>" runat="server" class="btn btn-mid btn-inverse">
									カートに入れる</asp:LinkButton>
									<span style="color:Red;" id="sErrorMessage" runat="server"></span>
									<asp:HiddenField ID="hfProductId" Value='<%# Eval(Constants.FIELD_PRODUCT_PRODUCT_ID) %>' runat="server" />
									<asp:HiddenField ID="hfVariationId" Value='<%# Eval(Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) %>' runat="server" />
									<asp:HiddenField ID="hfBuyableMemberRank" Value='<%# Eval(Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK) %>' runat="server" />
								</li>
							</ul>
						</li>
					</ul>
				</li>
				</ItemTemplate>
				<FooterTemplate>
					</ul>
				</FooterTemplate>
			</asp:Repeater>
			<!--↑ 商品一覧ループ -->
			<%-- アラートメッセージ --%>
			<% if (StringUtility.ToEmpty(this.AlertMessage) != "") { %>
			<p><%= this.AlertMessage%></p>
			<% } %>
		</div>
		<!--△ 商品一覧情報 △-->

		</ContentTemplate>
		</asp:UpdatePanel>
		<%-- △カート投入リンク（UpdatePanel）△ --%>
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