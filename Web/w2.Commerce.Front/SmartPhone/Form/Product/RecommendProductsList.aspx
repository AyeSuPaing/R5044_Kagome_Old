<%--
=========================================================================================================
  Module      : おすすめタグ・商品一覧画面(RecommendProductsList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Register TagPrefix="uc" TagName="Criteo" Src="~/Form/Common/Criteo.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductVariationImages" Src="~/SmartPhone/Form/Common/Product/BodyProductVariationImages.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProductDetailModal" Src="~/SmartPhone/Form/Common/Product/ProductDetailModal.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Form/Product/RecommendProductsList.aspx.cs" Inherits="Form_Product_RecommendProductsList" Title="おすすめタグ・商品一覧ページ" %>
<%@ Import Namespace="w2.Common.Helper" %>
<%--
下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="ｗ２ユーザー" %>
--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
	<link href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "SmartPhone/Css/product.css") %>" rel="stylesheet" type="text/css" media="all" />
	<%= this.BrandAdditionalDsignTag %>
	<link rel="canonical" href="<%# this.CanonicalTag %>" />
	<script type="text/javascript" charset="UTF-8" src="<%= Constants.PATH_ROOT %>Js/jquery.biggerlink.min.js"></script>
	<%# this.PaginationTag %>
<%-- △編集可能領域△ --%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<div id="divTopArea">
<%-- ▽レイアウト領域：トップエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<section class="wrap-product-list">
<div>
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
</div>
<%-- ▽ページャー▽ --%>
<% if (this.IsInfiniteLoad == false) { %>
<div class="pager-wrap above">
	<%# this.PagerHtml %>
</div>
<% } %>
<%-- △ページャー△ --%>
<%-- カート投入ボタン押下時にどの画面へ遷移するか？ --%>
<%-- CART：カート一覧画面 その他：画面遷移しない --%>
<asp:HiddenField ID="hfIsRedirectAfterAddProduct" Value="CART" runat="server" />

<%-- ▽商品一覧ループ▽ --%>
<asp:UpdatePanel runat="server" UpdateMode="Conditional">
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
			<div class="infiniteLoadProducts" runat="server">
				<%-- ▽商品一覧ループ(通常表示)▽ --%>
				<asp:Repeater ID="rProductsListView" runat="server" Visible="<%# this.IsInfiniteLoad == false %>" ItemType="w2.App.Common.Awoo.GetPage.Products" OnItemCommand="InnerRepeater_ItemCommand">
					<HeaderTemplate>
						<ul class="product-list-2 clearfix">
					</HeaderTemplate>
						<ItemTemplate>
						<li>
						<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(GetProductMaster(Item), true)) %>'>
						<div class="product-image">
							<w2c:ProductImage ImageSize="M" ProductMaster="<%# GetProductMaster(Item) %>" IsVariation="false" runat="server" />
							<%-- ▽在庫切れ可否▽ --%>
							<span visible='<%# ProductListUtility.IsProductSoldOut(GetProductMaster(Item)) %>' runat="server" class="sold-out">SOLD OUT</span>
							<%-- △在庫切れ可否△ --%>
						</div>
						<div class="product-name">
							<%-- ▽商品アイコン▽ --%>
							<p class="icon">
								<w2c:ProductIcon ID="ProductIcon1" IconNo="1" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
								<w2c:ProductIcon ID="ProductIcon2" IconNo="2" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
								<w2c:ProductIcon ID="ProductIcon3" IconNo="3" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
								<w2c:ProductIcon ID="ProductIcon4" IconNo="4" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
								<w2c:ProductIcon ID="ProductIcon5" IconNo="5" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
								<w2c:ProductIcon ID="ProductIcon6" IconNo="6" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
								<w2c:ProductIcon ID="ProductIcon7" IconNo="7" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
								<w2c:ProductIcon ID="ProductIcon8" IconNo="8" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
								<w2c:ProductIcon ID="ProductIcon9" IconNo="9" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
								<w2c:ProductIcon ID="ProductIcon10" IconNo="10" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
							</p>
							<%-- △商品アイコン△ --%>
							<%# WebSanitizer.HtmlEncode(GetProductData(GetProductMaster(Item), "name")) %>
						</div>
						<div class="product-price">
						<%-- ▽商品会員ランク価格有効▽ --%>
						<p visible='<%# GetProductMemberRankPriceValid(GetProductMaster(Item)) %>' runat="server" class="special">
							<%#: CurrencyManager.ToPrice(ProductPage.GetProductMemberRankPrice(GetProductMaster(Item))) %>
							（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(GetProductMaster(Item))) %>）
							<span class="line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(GetProductMaster(Item))) %>（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(GetProductMaster(Item))) %>）</span>
						</p>
						<%-- △商品会員ランク価格有効△ --%>
						<%-- ▽商品セール価格有効▽ --%>
						<p visible='<%# GetProductTimeSalesValid(GetProductMaster(Item)) %>' runat="server" class="special">
							<%#: CurrencyManager.ToPrice(ProductPage.GetProductTimeSalePriceNumeric(GetProductMaster(Item))) %>
							（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(GetProductMaster(Item))) %>）
							<span class="line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(GetProductMaster(Item))) %>（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(GetProductMaster(Item))) %>）</span>
						</p>
						<%-- △商品セール価格有効△ --%>
						<%-- ▽商品特別価格有効▽ --%>
						<p visible='<%# GetProductSpecialPriceValid(GetProductMaster(Item)) %>' runat="server" class="special">
							<%#: CurrencyManager.ToPrice(ProductPage.GetProductSpecialPriceNumeric(GetProductMaster(Item))) %>
							（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(GetProductMaster(Item))) %>）
							<span class="line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(GetProductMaster(Item))) %>（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(GetProductMaster(Item))) %>）</span>
						</p>
						<%-- △商品特別価格有効△ --%>
						<%-- ▽商品通常価格有効▽ --%>
						<p visible='<%# GetProductNormalPriceValid(GetProductMaster(Item)) %>' runat="server">
							<%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(GetProductMaster(Item))) %>
							（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(GetProductMaster(Item))) %>）
						</p>
						<%-- △商品通常価格有効△ --%>
						<%-- ▽商品加算ポイント▽ --%>
						<p visible='<%# (this.IsLoggedIn && (GetProductAddPointString(GetProductMaster(Item)) != "")) %>' runat="server">
							ポイント<%# WebSanitizer.HtmlEncode(GetProductAddPointString(GetProductMaster(Item))) %>
						</p>
						<%-- △商品加算ポイント△ --%>
						<%-- ▽定期購入価格有効▽ --%>
						<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) {%>
						<span visible='<%# (GetKeyValue(GetProductMaster(Item), Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG).ToString() != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) && ((CheckFixedPurchaseLimitedUserLevel(this.ShopId, (string)GetProductData(GetProductMaster(Item), "product_id")) == false)) %>' runat="server">
							<p visible='<%# IsProductFixedPurchaseFirsttimePriceValid(GetProductMaster(Item)) %>' runat="server">
								定期初回価格:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchaseFirsttimePrice(GetProductMaster(Item))) %>（<%#: GetTaxIncludeString(GetProductMaster(Item)) %>）
								<br />
							</p>
							定期通常価格:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchasePrice(GetProductMaster(Item))) %>（<%#: GetTaxIncludeString(GetProductMaster(Item)) %>）
						</span>
						<% } %>
						<%-- △定期購入価格有効△ --%>
						<%-- ▽定期商品加算ポイント▽ --%>
							<p visible='<%# (this.IsLoggedIn && (GetProductAddPointString(GetProductMaster(Item)) != "") && (GetKeyValue(GetProductMaster(Item), Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG).ToString() != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) && ((CheckFixedPurchaseLimitedUserLevel(this.ShopId, (string)GetProductData(GetProductMaster(Item), "product_id")) == false))) %>' runat="server">
								<span class="addPoint">ポイント<%# WebSanitizer.HtmlEncode(GetProductAddPointString(GetProductMaster(Item), false, false, true)) %></span><span id="Span1" visible='<%# ((string)GetKeyValue(GetProductMaster(Item), Constants.FIELD_PRODUCT_POINT_KBN2)) == Constants.FLG_PRODUCT_POINT_KBN1_RATE %>' runat="server">
									(<%# WebSanitizer.HtmlEncode(GetProductAddPointCalculateAfterString(GetProductMaster(Item), false, false, true))%>)
								</span>
							</p>
						<%-- △定期商品加算ポイント△ --%>
						<%-- ▽商品頒布会購入価格▽ --%>
						<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED) {%>
							<span visible='<%# (GetKeyValue(GetProductMaster(Item), Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG).ToString() != Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_INVALID) && ((CheckFixedPurchaseLimitedUserLevel(this.ShopId, (string)GetProductData(GetProductMaster(Item), "product_id")) == false)) %>' runat="server">
								<br />
								頒布会通常価格:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchasePrice(GetProductMaster(Item))) %>
							</span>
						<% } %>
						<%-- △商品頒布会購入価格△ --%>
							<%-- ▽お気に入りの登録人数表示▽ --%>
						<p>お気に入りの登録人数：<%# this.GetFavoriteCount(Item.ProductId) %>人</p>
						<%-- △お気に入りの登録人数表示△ --%>
						</div>
						<%-- ▽セットプロモーション情報▽ --%>
						<asp:Repeater ID="rSetPromotion" DataSource="<%# GetSetPromotionByProduct((DataRowView)GetProductMaster(Item)) %>" runat="server">
							<HeaderTemplate>
							<div class="product-set-promotion">
							</HeaderTemplate>
							<ItemTemplate>
							<p>
							<%# ((SetPromotionModel)Container.DataItem).SetpromotionDispName%>
							</p>
							</ItemTemplate>
							<FooterTemplate>
							</div>
							</FooterTemplate>
						</asp:Repeater>
						<%-- △セットプロモーション情報△ --%>
						<div class="product-image-variation">
							<uc:BodyProductVariationImages ImageSize="M" ProductMaster="<%# GetProductMaster(Item) %>" VariationList="<%# this.ProductVariationList %>" VariationNo="<%# Container.ItemIndex.ToString() %>" runat="server" />
						</div>
						<% if (Constants.USE_MODAL_PRODUCT_LIST) { %>
							<a class="productlist_detailsLink" href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(GetProductMaster(Item), true)) %>">詳細ページへ</a>
							<a><asp:Button ID="btnShowDetail" runat="server" Text="注文する" CommandArgument="<%# GetProductData(GetProductMaster(Item), Constants.FIELD_PRODUCT_PRODUCT_ID) %>" CommandName="btnOpenModalOrAddCart" CssClass="productlist_orderButton" /></a>
						<% } %>
						</a>
						</li>
						</ItemTemplate>
					<FooterTemplate>
						</ul>
					</FooterTemplate>
				</asp:Repeater>
				<%-- △商品一覧ループ△ --%>
				<%-- ▽商品一覧ループ無限ロード▽ --%>
				<asp:Repeater ID="rProductsWindowShopping" Visible="<%# this.IsInfiniteLoad %>" ItemType="w2.App.Common.Awoo.GetPage.Products" runat="server" OnItemCommand="InnerRepeater_ItemCommand">
					<HeaderTemplate>
						<ul class="product-list-2 clearfix">
					</HeaderTemplate>
						<ItemTemplate>
							<li>
								<div id="dInfiniteLoadProduct" class="windowpanel" runat="server">
								<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(GetProductMaster(Item), true)) %>'>
								<div class="product-image">
									<w2c:ProductImage ImageSize="M" ProductMaster="<%# GetProductMaster(Item) %>" IsVariation="false" runat="server" />
									<%-- ▽在庫切れ可否▽ --%>
									<span visible='<%# ProductListUtility.IsProductSoldOut(GetProductMaster(Item)) %>' runat="server" class="sold-out">SOLD OUT</span>
									<%-- △在庫切れ可否△ --%>
								</div>
								<div class="product-name">
									<%-- ▽商品アイコン▽ --%>
									<p class="icon">
										<w2c:ProductIcon ID="ProductIcon1" IconNo="1" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
										<w2c:ProductIcon ID="ProductIcon2" IconNo="2" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
										<w2c:ProductIcon ID="ProductIcon3" IconNo="3" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
										<w2c:ProductIcon ID="ProductIcon4" IconNo="4" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
										<w2c:ProductIcon ID="ProductIcon5" IconNo="5" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
										<w2c:ProductIcon ID="ProductIcon6" IconNo="6" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
										<w2c:ProductIcon ID="ProductIcon7" IconNo="7" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
										<w2c:ProductIcon ID="ProductIcon8" IconNo="8" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
										<w2c:ProductIcon ID="ProductIcon9" IconNo="9" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
										<w2c:ProductIcon ID="ProductIcon10" IconNo="10" ProductMaster="<%# GetProductMaster(Item) %>" runat="server" />
									</p>
									<%-- △商品アイコン△ --%>
									<%#: GetProductData(GetProductMaster(Item), "name") %>
								</div>
								<div class="product-price">
								<%-- ▽商品会員ランク価格有効▽ --%>
								<p visible='<%# GetProductMemberRankPriceValid(GetProductMaster(Item)) %>' runat="server" class="special">
									<%#: CurrencyManager.ToPrice(ProductPage.GetProductMemberRankPrice(GetProductMaster(Item))) %>
									（<%#: GetTaxIncludeString(GetProductMaster(Item)) %>）
									<span class="line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(GetProductMaster(Item))) %>（<%#: GetTaxIncludeString(GetProductMaster(Item)) %>）</span>
								</p>
								<%-- △商品会員ランク価格有効△ --%>
								<%-- ▽商品セール価格有効▽ --%>
								<p visible='<%# GetProductTimeSalesValid(GetProductMaster(Item)) %>' runat="server" class="special">
									<%#: CurrencyManager.ToPrice(ProductPage.GetProductTimeSalePriceNumeric(GetProductMaster(Item))) %>
									（<%#: GetTaxIncludeString(GetProductMaster(Item)) %>）
									<span class="line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(GetProductMaster(Item))) %>（<%#: GetTaxIncludeString(GetProductMaster(Item)) %>）</span>
								</p>
								<%-- △商品セール価格有効△ --%>
								<%-- ▽商品特別価格有効▽ --%>
								<p visible='<%# GetProductSpecialPriceValid(GetProductMaster(Item)) %>' runat="server" class="special">
									<%#: CurrencyManager.ToPrice(ProductPage.GetProductSpecialPriceNumeric(GetProductMaster(Item))) %>
									（<%#: GetTaxIncludeString(GetProductMaster(Item)) %>）
									<span class="line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(GetProductMaster(Item))) %>（<%#: GetTaxIncludeString(GetProductMaster(Item)) %>）</span>
								</p>
								<%-- △商品特別価格有効△ --%>
								<%-- ▽商品通常価格有効▽ --%>
								<p visible='<%# GetProductNormalPriceValid(GetProductMaster(Item)) %>' runat="server">
									<%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(GetProductMaster(Item))) %>
									（<%#: GetTaxIncludeString(GetProductMaster(Item)) %>）
								</p>
								<%-- △商品通常価格有効△ --%>
								<%-- ▽商品加算ポイント▽ --%>
								<p visible='<%# (this.IsLoggedIn && (GetProductAddPointString(GetProductMaster(Item)) != "")) %>' runat="server">
									ポイント<%#: GetProductAddPointString(GetProductMaster(Item)) %>
								</p>
								<%-- △商品加算ポイント△ --%>
								<%-- ▽定期購入価格有効▽ --%>
								<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) {%>
								<span visible='<%# (GetKeyValue(GetProductMaster(Item), Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG).ToString() != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) && ((CheckFixedPurchaseLimitedUserLevel(this.ShopId, (string)GetProductData(GetProductMaster(Item), "product_id")) == false)) %>' runat="server">
									<p visible='<%# IsProductFixedPurchaseFirsttimePriceValid(GetProductMaster(Item)) %>' runat="server">
										定期初回価格:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchaseFirsttimePrice(GetProductMaster(Item))) %>（<%#: GetTaxIncludeString(GetProductMaster(Item)) %>）
										<br />
									</p>
									定期通常価格:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchasePrice(GetProductMaster(Item))) %>（<%#: GetTaxIncludeString(GetProductMaster(Item)) %>）
								</span>
								<% } %>
								<%-- △定期購入価格有効△ --%>
								<%-- ▽定期商品加算ポイント▽ --%>
									<p visible='<%# (this.IsLoggedIn && (GetProductAddPointString(GetProductMaster(Item)) != "") && (GetKeyValue(GetProductMaster(Item), Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG).ToString() != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) && ((CheckFixedPurchaseLimitedUserLevel(this.ShopId, (string)GetProductData(GetProductMaster(Item), "product_id")) == false))) %>' runat="server">
										<span class="addPoint">ポイント<%#: GetProductAddPointString(GetProductMaster(Item), false, false, true) %></span><span id="Span1" visible='<%# ((string)GetKeyValue(GetProductMaster(Item), Constants.FIELD_PRODUCT_POINT_KBN2)) == Constants.FLG_PRODUCT_POINT_KBN1_RATE %>' runat="server">
											(<%#: GetProductAddPointCalculateAfterString(GetProductMaster(Item), false, false, true)%>)
										</span>
									</p>
								<%-- △定期商品加算ポイント△ --%>
								<%-- ▽商品頒布会購入価格▽ --%>
								<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED) {%>
									<span visible='<%# (GetKeyValue(GetProductMaster(Item), Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG).ToString() != Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_INVALID) && ((CheckFixedPurchaseLimitedUserLevel(this.ShopId, (string)GetProductData(GetProductMaster(Item), "product_id")) == false)) %>' runat="server">
										<br />
										頒布会通常価格:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchasePrice(GetProductMaster(Item))) %>
									</span>
								<% } %>
								<%-- △商品頒布会購入価格△ --%>
									<%-- ▽お気に入りの登録人数表示▽ --%>
								<p>お気に入りの登録人数：<%# this.GetFavoriteCount(Item.ProductId) %>人</p>
								<%-- △お気に入りの登録人数表示△ --%>
								</div>
								<%-- ▽セットプロモーション情報▽ --%>
								<asp:Repeater ID="rSetPromotion" DataSource="<%# GetSetPromotionByProduct((DataRowView)GetProductMaster(Item)) %>" runat="server">
									<HeaderTemplate>
									<div class="product-set-promotion">
									</HeaderTemplate>
									<ItemTemplate>
									<p>
									<%# ((SetPromotionModel)Container.DataItem).SetpromotionDispName%>
									</p>
									</ItemTemplate>
									<FooterTemplate>
									</div>
									</FooterTemplate>
								</asp:Repeater>
								<%-- △セットプロモーション情報△ --%>
								<div class="product-image-variation">
									<uc:BodyProductVariationImages ImageSize="M" ProductMaster="<%# GetProductMaster(Item) %>" VariationList="<%# this.ProductVariationList %>" VariationNo="<%# Container.ItemIndex.ToString() %>" runat="server" />
								</div>
								</a>
								<% if (Constants.USE_MODAL_PRODUCT_LIST) { %>
									<a class="productlist_detailsLink" href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(GetProductMaster(Item), true)) %>">詳細ページへ</a>
									<a><asp:Button ID="btnShowDetail" runat="server" Text="注文する" CommandArgument="<%# GetProductData(GetProductMaster(Item), Constants.FIELD_PRODUCT_PRODUCT_ID) %>" CommandName="btnOpenModalOrAddCart" CssClass="productlist_orderButton" /></a>
								<% } %>
								</div>
							</li>
						</ItemTemplate>
					<FooterTemplate>
						</ul>
					</FooterTemplate>
				</asp:Repeater>
				<%-- △商品一覧ループ無限ロード△ --%>
			</div>
		</ItemTemplate>
	</asp:Repeater>
	<% if (this.IsInfiniteLoad) { %>
		<asp:LinkButton ID="lbInfiniteLoadingUpperNextButton" CommandArgument="<%# LOADING_TYPE_UPPER %>" OnClick="lbInfiniteLoadingNextButton_OnClick" runat="server"></asp:LinkButton>
		<asp:LinkButton ID="lbInfiniteLoadingLowerNextButton" CommandArgument="<%# LOADING_TYPE_LOWER %>" OnClick="lbInfiniteLoadingNextButton_OnClick" runat="server"></asp:LinkButton>
		<div id="loadingAnimationLower" style="width:100%" class="loading">
			<img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/loading.gif" style="opacity:0; display: block; width: 30px; height: 30px; margin: 0 auto 20px;"/>
		</div>
		<div class="pager-wrap below">
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

<%-- ▽ページャー▽ --%>
<% if (this.IsInfiniteLoad == false) { %>
<div class="pager-wrap below">
	<%# this.PagerHtml %>
</div>
<% } %>
<%-- △ページャー△ --%>

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

<div class="user-footer">
<div visible="<%# this.HasDisplayProduct == false %>" runat="server" class="msg-alert button-prev">
	<!--▽ 商品が1つもなかった場合のエラー文言 ▽-->
	<%# WebSanitizer.HtmlEncode(this.AlertMessage) %>
	<p style="text-align: center; padding: 10px;"><a href="javascript:history.back();" class="btn btn-large">前のページに戻る</a></p>
	<!--△ 商品が1つもなかった場合のエラー文言 △-->
</div>
</div>

<%-- CRITEOタグ（引数：商品一覧情報） --%>
<uc:Criteo ID="criteo" runat="server" Datas="<%# this.ProductMasterList %>" />
</section>
<%-- △編集可能領域△ --%>
<div id="divBottomArea">
<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>

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
			pagenationThreshold: 0.1
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
