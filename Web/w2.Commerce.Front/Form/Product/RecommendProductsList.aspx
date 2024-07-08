<%--
=========================================================================================================
  Module      : おすすめタグ・商品一覧画面(RecommendProductsList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%@ Register TagPrefix="uc" TagName="BodyProductCategoryTree" Src="~/Form/Common/Product/BodyProductCategoryTree.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProductColorSearchBox" Src="~/Form/Common/Product/ProductColorSearchBox.ascx" %>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Register TagPrefix="uc" TagName="BodyProductVariationImages" Src="~/Form/Common/Product/BodyProductVariationImages.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProductDetailModal" Src="~/Form/Common/Product/ProductDetailModal.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Form/Product/RecommendProductsList.aspx.cs" Inherits="Form_Product_RecommendProductsList" Title="おすすめタグ・商品一覧ページ" %>
<%@ Import Namespace="w2.Common.Helper" %>
<%--
下記のタグはファイル情報保持用です。削除しないでください。In
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="ｗ２ユーザー" %>
--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<link href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT) %>Css/product.css" rel="stylesheet" type="text/css" media="all" />
<%= this.BrandAdditionalDsignTag %>
<link rel="canonical" href="<%# this.CanonicalTag %>" />
<%# this.PaginationTag %>
<%-- △編集可能領域△ --%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<table id="tblLayout" class="tblLayout_ProductList">
<tr>
<td>
<div id="secondary">
<%-- ▽レイアウト領域：レフトエリア▽ --%>
<uc:ProductColorSearchBox runat="server" />
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
	<div class="recommendPageInfoArea">
		<div class="contents">
			<asp:Repeater DataSource="<%# this.SuggestionTags %>" ItemType="w2.App.Common.Awoo.GetPage.SuggestionTags" runat="server" >
				<HeaderTemplate>
				</HeaderTemplate>
				<ItemTemplate>
					<a class="recommendTag" href ="<%# CreateSuggestionTagUrl(Item) %>">
						<span><%# Item.Text %></span>
					</a>
				</ItemTemplate>
			</asp:Repeater>
		</div>
		<div class="contents">
			<h1><%: this.H1 %></h1>
		</div>
		<div>
			<%: this.Description  %>
		</div>
	</div>
	<div id="sortBox" style="text-align: right;">
		<asp:DropDownList
			id="ddlSort"
			CssClass="input_border"
			Visible="<%# this.HasDisplayProduct %>"
			DataSource="<%# this.SortContents %>"
			SelectedValue="<%# this.Sort.ToText() %>"
			DataTextField="text"
			DataValueField="value"
			OnSelectedIndexChanged="ddlSort_OnSelectedIndexChanged"
			AutoPostBack="true"
			runat="server"></asp:DropDownList>
	</div>
<!--▽ ページャ ▽-->
<% if (this.IsInfiniteLoad == false) { %>
<div id="pagination" class="above clearFix" >
<%# this.PagerHtml %>
</div>
<% } %>
<!--△ ページャ △-->
<%-- カート投入ボタン押下時にどの画面へ遷移するか？ --%>
<%-- CART：カート一覧画面 その他：画面遷移しない --%>
<asp:HiddenField ID="hfIsRedirectAfterAddProduct" Value="CART" runat="server" />

<div class="listProduct">

<%-- ▽商品一覧ループ▽ --%>
<asp:UpdatePanel runat="server">
	<ContentTemplate>
		<% if (this.IsInfiniteLoad) { %>
		<div id="loadingAnimationUpper" style="width:100%; padding:0" class="loading">
			<img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/loading.gif" style="opacity:0; display: block; width: 30px; height: 30px; margin: 0 auto 20px;"/>
		</div>
		<asp:HiddenField ID="hfPageNumber" runat="server"/>
		<asp:HiddenField ID="hfDisplayPageNumberMax" runat="server"/>
		<% } %>
		<asp:Repeater ID="rTopProductList" runat="server" >
			<ItemTemplate>
				<div class="infiniteLoadProducts listProduct" runat="server">
					<%-- ▽商品一覧ループ(通常表示)▽ --%>
					<asp:Repeater ID="rProductsListView" runat="server" Visible="<%# this.IsInfiniteLoad == false %>" ItemType="w2.App.Common.Awoo.GetPage.Products" OnItemCommand="InnerRepeater_ItemCommand">
						<HeaderTemplate>
						<div class="heightLineParent clearFix">
						</HeaderTemplate>
						<ItemTemplate>
						<div id="dInfiniteLoadProduct" class="glbPlist column5 windowpanel" runat="server">
							<ul>
								<li class="icon">
									<w2c:ProductIcon IconNo="1" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
									<w2c:ProductIcon IconNo="2" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
									<w2c:ProductIcon IconNo="3" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
									<w2c:ProductIcon IconNo="4" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
									<w2c:ProductIcon IconNo="5" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
									<w2c:ProductIcon IconNo="6" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
									<w2c:ProductIcon IconNo="7" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
									<w2c:ProductIcon IconNo="8" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
									<w2c:ProductIcon IconNo="9" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
									<w2c:ProductIcon IconNo="10" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
								</li>
								<li class="thumb">
								<% if(Constants.LAYER_DISPLAY_VARIATION_IMAGES_ENABLED
									&& (Constants.SETTING_PRODUCT_LIST_SEARCH_KBN == false)) { %>
								<uc:BodyProductVariationImages ImageSize="M" ProductMaster="<%# GetProductMaster(Item) %>" VariationList="<%# this.ProductVariationList %>" VariationNo="<%# Container.ItemIndex.ToString() %>" runat="server" />
								<% } else { %>
								<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(GetProductMaster(Item), true)) %>'>
								<% if (Constants.SETTING_PRODUCT_LIST_SEARCH_KBN) { %>
									<w2c:ProductImage ImageSize="M" ProductMaster="<%# GetProductMaster(Item) %>" IsVariation="false" IsGroupVariation="true" runat="server" />
								<% } else { %>
									<w2c:ProductImage ImageSize="M" ProductMaster="<%# GetProductMaster(Item) %>" IsVariation="false" runat="server" />
								<% } %>
								</a>
								<% } %><span visible='<%# ProductListUtility.IsProductSoldOut(GetProductMaster(Item)) %>' runat="server" class="soldout">SOLDOUT</span>
								</li>
								<li class="name">
									<p class="pid"><%# WebSanitizer.HtmlEncode(GetProductData(GetProductMaster(Item), Constants.FIELD_PRODUCT_PRODUCT_ID)) %></p>
									<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(GetProductMaster(Item), true)) %>'><%# WebSanitizer.HtmlEncode(GetProductData(GetProductMaster(Item), "name")) %></a>
								<!-- 商品ID表示 -->
									<p><%#: StringUtility.ToEmpty(ProductPage.GetKeyValueToNull(GetProductMaster(Item), Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1)) %></p>
								</li>
								<li class="price">

								<p>
									<%# CurrencyManager.ToPrice(ProductPage.GetKeyValueToNull(
											GetProductMaster(Item),
											Constants.SETTING_PRODUCT_LIST_SEARCH_KBN
												? Constants.FIELD_PRODUCTVARIATION_PRICE
												: Constants.FIELD_PRODUCT_DISPLAY_PRICE)) %>
									<%# StringUtility.ToEmpty(GetVariationPriceTax(GetProductMaster(Item))) %>
								</p>
								<%-- ▽商品会員ランク価格有効▽ --%>
								<p visible='<%# GetProductMemberRankPriceValid(GetProductMaster(Item), Constants.SETTING_PRODUCT_LIST_SEARCH_KBN) %>' runat="server">
									<span style="text-decoration: line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(GetProductMaster(Item), Constants.SETTING_PRODUCT_LIST_SEARCH_KBN)) %>（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(GetProductMaster(Item))) %>）</span><br />
									<span style="color: #f00;"><%#: CurrencyManager.ToPrice(ProductPage.GetProductMemberRankPrice(GetProductMaster(Item), Constants.SETTING_PRODUCT_LIST_SEARCH_KBN)) %>（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(GetProductMaster(Item))) %>）</span>
								</p>
								<%-- ▽商品セール価格有効▽ --%>
								<p visible='<%# GetProductTimeSalesValid(GetProductMaster(Item), Constants.SETTING_PRODUCT_LIST_SEARCH_KBN) %>' runat="server">
									<span style="text-decoration: line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(GetProductMaster(Item), Constants.SETTING_PRODUCT_LIST_SEARCH_KBN)) %>（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(GetProductMaster(Item))) %>）</span><br />
									<span style="color: #f00;"><%#: CurrencyManager.ToPrice(ProductPage.GetProductTimeSalePriceNumeric(GetProductMaster(Item))) %>（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(GetProductMaster(Item))) %>）</span>
								</p>
								<%-- ▽商品特別価格有効▽ --%>
								<p visible='<%# GetProductSpecialPriceValid(GetProductMaster(Item), Constants.SETTING_PRODUCT_LIST_SEARCH_KBN) %>' runat="server"><span style="text-decoration: line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(GetProductMaster(Item), Constants.SETTING_PRODUCT_LIST_SEARCH_KBN)) %>（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(GetProductMaster(Item))) %>）</span><br />
								<span style="color: #f00;"><%#: CurrencyManager.ToPrice(ProductPage.GetProductSpecialPriceNumeric(GetProductMaster(Item), Constants.SETTING_PRODUCT_LIST_SEARCH_KBN)) %>（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(GetProductMaster(Item))) %>）</span>
								</p>
								<%-- ▽商品通常価格有効▽ --%>
								<p visible='<%# GetProductNormalPriceValid(GetProductMaster(Item), Constants.SETTING_PRODUCT_LIST_SEARCH_KBN) %>' runat="server">
								<%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(GetProductMaster(Item), Constants.SETTING_PRODUCT_LIST_SEARCH_KBN)) %>（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(GetProductMaster(Item))) %>）
								</p>
								<%-- ▽定期購入価格有効▽ --%>
								<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) {%>
								<p visible='<%# (GetKeyValue(GetProductMaster(Item), Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG).ToString() != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) && (CheckFixedPurchaseLimitedUserLevel(this.ShopId, (string)GetProductData(GetProductMaster(Item), "product_id")) == false) %>' runat="server">
									<p visible='<%# IsProductFixedPurchaseFirsttimePriceValid(GetProductMaster(Item), Constants.SETTING_PRODUCT_LIST_SEARCH_KBN) %>' runat="server">
										定期初回:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchaseFirsttimePrice(GetProductMaster(Item), Constants.SETTING_PRODUCT_LIST_SEARCH_KBN)) %>（<%#: GetTaxIncludeString(GetProductMaster(Item)) %>）
									</p>
									<p>
										定期通常:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchasePrice(GetProductMaster(Item), Constants.SETTING_PRODUCT_LIST_SEARCH_KBN)) %>（<%#: GetTaxIncludeString(GetProductMaster(Item)) %>）
									</p>
								</p>
								<% } %>
								<%-- ▽頒布会購入価格有効▽ --%>
								<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED) {%>
								<p visible='<%# (GetKeyValue(GetProductMaster(Item), Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG).ToString() != Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_INVALID) && (CheckFixedPurchaseLimitedUserLevel(this.ShopId, (string)GetProductData(GetProductMaster(Item), "product_id")) == false) %>' runat="server">
									<p>
										頒布会通常:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchasePrice(GetProductMaster(Item), Constants.SETTING_PRODUCT_LIST_SEARCH_KBN)) %>（<%#: GetTaxIncludeString(GetProductMaster(Item)) %>）
									</p>
								</p>
								<% } %>
								<%-- △頒布会購入価格有効△ --%>
								</li>
								<%-- ▽お気に入りの登録人数表示▽ --%>
								<li class="favorite" runat="server">
								お気に入りの登録人数：<%# GetFavoriteCount(Item.ProductId) %>人
								</li>
								<%-- △お気に入りの登録人数表示△ --%>
								<% if (Constants.USE_MODAL_PRODUCT_LIST) { %>
									<a class="productlist_detailsLink" href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(GetProductMaster(Item), true)) %>">詳細ページへ</a>
									<a><asp:Button ID="btnShowDetail" runat="server" Text="注文する" CommandArgument="<%# GetProductData(GetProductMaster(Item), Constants.FIELD_PRODUCT_PRODUCT_ID) %>" CommandName="btnOpenModalOrAddCart" CssClass="productlist_orderButton" /></a>
								<% } else { %>
									<a class="link_product_detail" href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(GetProductMaster(Item), true)) %>">▶ 詳細ページへ</a>
								<% } %>
							</ul>
						</div>
						</ItemTemplate>
						<FooterTemplate>
						</div>
						</FooterTemplate>
					</asp:Repeater>
					<%-- △商品一覧ループ△ --%>
					<%--▽ 商品一覧（無限ロード）▽--%>
					<asp:Repeater ID="rProductsWindowShopping" Visible="<%# this.IsInfiniteLoad %>"  ItemType="w2.App.Common.Awoo.GetPage.Products" runat="server" OnItemCommand="InnerRepeater_ItemCommand">
						<HeaderTemplate>
						<div class="heightLineParent clearFix">
						</HeaderTemplate>
						<ItemTemplate>
							<div class="glbPlist column5 windowpanel" id="dInfiniteLoadProduct" runat="server">
								<ul>
									<li class="icon">
										<w2c:ProductIcon IconNo="1" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
										<w2c:ProductIcon IconNo="2" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
										<w2c:ProductIcon IconNo="3" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
										<w2c:ProductIcon IconNo="4" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
										<w2c:ProductIcon IconNo="5" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
										<w2c:ProductIcon IconNo="6" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
										<w2c:ProductIcon IconNo="7" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
										<w2c:ProductIcon IconNo="8" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
										<w2c:ProductIcon IconNo="9" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
										<w2c:ProductIcon IconNo="10" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
									</li>
									<li class="thumb">
										<% if(Constants.LAYER_DISPLAY_VARIATION_IMAGES_ENABLED
											&& (Constants.SETTING_PRODUCT_LIST_SEARCH_KBN == false)) { %>
										<uc:BodyProductVariationImages ImageSize="M" ProductMaster="<%# GetProductMaster(Item) %>" VariationList="<%# this.ProductVariationList %>" VariationNo="<%# Container.ItemIndex.ToString() %>" runat="server" />
										<% } else { %>
										<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(GetProductMaster(Item), true)) %>'>
										<% if (Constants.SETTING_PRODUCT_LIST_SEARCH_KBN) { %>
											<w2c:ProductImage ImageSize="M" ProductMaster="<%# GetProductMaster(Item) %>" IsVariation="false" IsGroupVariation="true" runat="server" />
										<% } else { %>
											<w2c:ProductImage ImageSize="M" ProductMaster="<%# GetProductMaster(Item) %>" IsVariation="false" runat="server" />
										<% } %>
										</a>
										<% } %><span visible='<%# ProductListUtility.IsProductSoldOut(GetProductMaster(Item)) %>' runat="server" class="soldout">SOLDOUT</span>
									</li>
									<li class="name">
										<p class="pid"><%#:GetProductData(GetProductMaster(Item), Constants.FIELD_PRODUCT_PRODUCT_ID) %></p>
										<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(GetProductMaster(Item), true)) %>'><%#:GetProductData(GetProductMaster(Item), "name") %></a>
										<!-- 商品ID表示 -->
										<p><%#: StringUtility.ToEmpty(ProductPage.GetKeyValueToNull(GetProductMaster(Item), Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1)) %></p>
									</li>
									<li class="price">
										<p>
											<%# CurrencyManager.ToPrice(ProductPage.GetKeyValueToNull(
													GetProductMaster(Item),
													Constants.SETTING_PRODUCT_LIST_SEARCH_KBN
														? Constants.FIELD_PRODUCTVARIATION_PRICE
														: Constants.FIELD_PRODUCT_DISPLAY_PRICE)) %>
											<%# StringUtility.ToEmpty(GetVariationPriceTax(GetProductMaster(Item))) %>
										</p>
										<%-- ▽商品会員ランク価格有効▽ --%>
										<p visible='<%# GetProductMemberRankPriceValid(GetProductMaster(Item), Constants.SETTING_PRODUCT_LIST_SEARCH_KBN) %>' runat="server">
											<span style="text-decoration: line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(GetProductMaster(Item), Constants.SETTING_PRODUCT_LIST_SEARCH_KBN)) %>（<%#:GetTaxIncludeString(GetProductMaster(Item)) %>）</span><br />
											<span style="color: #f00;"><%#: CurrencyManager.ToPrice(ProductPage.GetProductMemberRankPrice(GetProductMaster(Item), Constants.SETTING_PRODUCT_LIST_SEARCH_KBN)) %>（<%#:GetTaxIncludeString(GetProductMaster(Item)) %>）</span>
										</p>
										<%-- ▽商品セール価格有効▽ --%>
										<p visible='<%# GetProductTimeSalesValid(GetProductMaster(Item), Constants.SETTING_PRODUCT_LIST_SEARCH_KBN) %>' runat="server">
											<span style="text-decoration: line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(GetProductMaster(Item), Constants.SETTING_PRODUCT_LIST_SEARCH_KBN)) %>（<%#:GetTaxIncludeString(GetProductMaster(Item)) %>）</span><br />
											<span style="color: #f00;"><%#: CurrencyManager.ToPrice(ProductPage.GetProductTimeSalePriceNumeric(GetProductMaster(Item))) %>（<%#:GetTaxIncludeString(GetProductMaster(Item)) %>）</span>
										</p>
										<%-- ▽商品特別価格有効▽ --%>
										<p visible='<%# GetProductSpecialPriceValid(GetProductMaster(Item), Constants.SETTING_PRODUCT_LIST_SEARCH_KBN) %>' runat="server"><span style="text-decoration: line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(GetProductMaster(Item), Constants.SETTING_PRODUCT_LIST_SEARCH_KBN)) %>（<%#:GetTaxIncludeString(GetProductMaster(Item)) %>）</span><br />
										<span style="color: #f00;"><%#: CurrencyManager.ToPrice(ProductPage.GetProductSpecialPriceNumeric(GetProductMaster(Item), Constants.SETTING_PRODUCT_LIST_SEARCH_KBN)) %>（<%#:GetTaxIncludeString(GetProductMaster(Item)) %>）</span>
										</p>
										<%-- ▽商品通常価格有効▽ --%>
										<p visible='<%# GetProductNormalPriceValid(GetProductMaster(Item), Constants.SETTING_PRODUCT_LIST_SEARCH_KBN) %>' runat="server">
										<%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(GetProductMaster(Item), Constants.SETTING_PRODUCT_LIST_SEARCH_KBN)) %>（<%#:GetTaxIncludeString(GetProductMaster(Item)) %>）
										</p>
										<%-- ▽定期購入価格有効▽ --%>
										<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) {%>
										<p visible='<%# (GetKeyValue(GetProductMaster(Item), Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG).ToString() != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) && (CheckFixedPurchaseLimitedUserLevel(this.ShopId, (string)GetProductData(GetProductMaster(Item), "product_id")) == false) %>' runat="server">
											<p visible='<%# IsProductFixedPurchaseFirsttimePriceValid(GetProductMaster(Item), Constants.SETTING_PRODUCT_LIST_SEARCH_KBN) %>' runat="server">
												定期初回:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchaseFirsttimePrice(GetProductMaster(Item), Constants.SETTING_PRODUCT_LIST_SEARCH_KBN)) %>（<%#: GetTaxIncludeString(GetProductMaster(Item)) %>）
											</p>
											<p>
												定期通常:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchasePrice(GetProductMaster(Item), Constants.SETTING_PRODUCT_LIST_SEARCH_KBN)) %>（<%#: GetTaxIncludeString(GetProductMaster(Item)) %>）
											</p>
										</p>
										<% } %>
										<%-- ▽頒布会購入価格有効▽ --%>
										<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED) {%>
										<p visible='<%# (GetKeyValue(GetProductMaster(Item), Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG).ToString() != Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_INVALID) && (CheckFixedPurchaseLimitedUserLevel(this.ShopId, (string)GetProductData(GetProductMaster(Item), "product_id")) == false) %>' runat="server">
											<p>
												頒布会通常:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchasePrice(GetProductMaster(Item), Constants.SETTING_PRODUCT_LIST_SEARCH_KBN)) %>（<%#: GetTaxIncludeString(GetProductMaster(Item)) %>）
											</p>
										</p>
										<% } %>
										<%-- △頒布会購入価格有効△ --%>
									</li>
									<%-- ▽お気に入りの登録人数表示▽ --%>
									<li class="favorite" runat="server">
									お気に入りの登録人数：<%# this.GetFavoriteCount((string)GetKeyValue(GetProductMaster(Item), Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID)) %>人
									</li>
									<%-- △お気に入りの登録人数表示△ --%>
									<% if (Constants.USE_MODAL_PRODUCT_LIST) { %>
										<a class="productlist_detailsLink" href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(GetProductMaster(Item), true)) %>">詳細ページへ</a>
										<a><asp:Button ID="btnShowDetail" runat="server" Text="注文する" CommandArgument="<%# GetProductData(GetProductMaster(Item), Constants.FIELD_PRODUCT_PRODUCT_ID) %>" CommandName="btnOpenModalOrAddCart" CssClass="productlist_orderButton" /></a>
									<% } else { %>
										<a class="link_product_detail" href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(GetProductMaster(Item), true)) %>">▶ 詳細ページへ</a>
									<% } %>
								</ul>
							</div>
						</ItemTemplate>
						<FooterTemplate>
						</div>
						</FooterTemplate>
					</asp:Repeater>
				</div>
			</ItemTemplate>
		</asp:Repeater>
		<% if (this.IsInfiniteLoad) { %>
			<asp:LinkButton ID="lbInfiniteLoadingUpperNextButton" CommandArgument="<%# LOADING_TYPE_UPPER %>" OnClick="lbInfiniteLoadingNextButton_OnClick" runat="server"></asp:LinkButton>
			<asp:LinkButton ID="lbInfiniteLoadingLowerNextButton" CommandArgument="<%# LOADING_TYPE_LOWER %>" OnClick="lbInfiniteLoadingNextButton_OnClick" runat="server"></asp:LinkButton>
			<div id="loadingAnimationLower" style="width:100%" class="loading">
				<img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/loading.gif" style="opacity:0; display: block; width: 30px; height: 30px; margin: 0 auto 20px;"/>
			</div>
			<div id="pagination" class="below clearFix">
				<asp:Label runat="server" ID="lbPagination" />
			</div>
		<% } %>
	</ContentTemplate>
	<Triggers>
		<asp:AsyncPostBackTrigger ControlID="lbInfiniteLoadingLowerNextButton" />
		<asp:AsyncPostBackTrigger ControlID="lbInfiniteLoadingUpperNextButton" />
	</Triggers>
</asp:UpdatePanel>
<%--△ 商品一覧（無限ロード）△--%>

</div>

<!--▽ ページャ ▽-->
<% if (this.IsInfiniteLoad == false) { %>
<div id="pagination" class="below clearFix">
	<%# this.PagerHtml %>
</div>
<% } %>
<!--△ ページャ △-->
<div visible="<%# this.HasDisplayProduct == false %>" runat="server" class="noProduct">
<!--▽ 商品が1つもなかった場合のエラー文言 ▽-->
	<%# WebSanitizer.HtmlEncode(this.AlertMessage) %>
	<p style="text-align: center;padding: 10px;"><a href="javascript:history.back();" class="btn btn-large">前のページに戻る</a></p>
<!--△ 商品が1つもなかった場合のエラー文言 △-->
</div>
</div>

<!--▽ 商品詳細モーダル ▽-->
<asp:UpdatePanel ID="upProductDetailModal" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
	<ContentTemplate>
		<div runat="server" id="dvModalBackground" class="productlist_modalBackground">
			<div id="dvProductDetailArea">
				<input type="hidden" id="hfTargetProductIdForModal" />
				<asp:Button runat="server" class="productlist_closeModalButton" OnCommand="btnCloseProductDetailModal_OnClick" Text="✕" CommandName="Close" OnClientClick="enableBackgroundScroll();"/>
				<asp:Repeater ID="rpProductModal" runat="server">
					<ItemTemplate>
						<uc:ProductDetailModal ID="ucProductDetailModal" runat="server" />
					</ItemTemplate>
				</asp:Repeater>
			</div>
		</div>
	</ContentTemplate>
</asp:UpdatePanel>
<!--△ 商品詳細モーダル △-->

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

<% if (this.IsInfiniteLoad) { %>
<script type="text/javascript" src="<%= Constants.PATH_ROOT %>Js/ProductListInfiniteLoad.js?<%: Constants.QUERY_STRING_FOR_UPDATE_EXTERNAL_FILE_URLENCODED %>"></script>
<script type="text/javascript">
	// 初期化
	$(document).ready(function () {
		// UpdatePanelの非同期更新かどうかを判定
		if (Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack()) {
			return;
		}

		// 無限ロード関数初期化
		ProductListInfiniteLoad.init({
			useInfiniteLoadProductList: "<%= Constants.USE_INFINITE_LOAD_PRODUCT_LIST %>",
			hfPageNumberClientId: "<%= this.WhfPageNumber.ClientID %>",
			hfDisplayPageNumberMaxClientId: "<%= this.WhfDisplayPageNumberMax.ClientID %>",
			lbInfiniteLoadingUpperNextButtonClientId: "<%= this.WlbInfiniteLoadingUpperNextButton.ClientID %>",
			lbInfiniteLoadingLowerNextButtonClientId: "<%= this.WlbInfiniteLoadingLowerNextButton.ClientID %>",
			pagenationThreshold: 0.4
		});
	});
</script>
<% } %>

<% if (Constants.USE_MODAL_PRODUCT_LIST) { %>
<script>
	// 背後のスクロールを再度有効にする
	function enableBackgroundScroll() {
		document.body.style.overflow = 'auto';
	}
</script>
<% } %>

</asp:Content>
