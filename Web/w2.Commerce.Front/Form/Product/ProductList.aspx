<%--
=========================================================================================================
  Module      : 商品一覧画面(ProductList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%@ Register TagPrefix="uc" TagName="BodyProductRecommendAdvanced" Src="~/Form/Common/Product/BodyProductRecommendAdvanced.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductSearchBox" Src="~/Form/Common/Product/BodyProductSearchBox.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductAdvancedSearchBox" Src="~/Form/Common/Product/BodyProductAdvancedSearchBox.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductCategoryTree" Src="~/Form/Common/Product/BodyProductCategoryTree.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductRanking" Src="~/Form/Common/Product/BodyProductRanking.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductHistory" Src="~/Form/Common/Product/BodyProductHistory.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyMiniCart" Src="~/Form/Common/BodyMiniCart.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductRecommendByRecommendEngine" Src="~/Form/Common/Product/BodyProductRecommendByRecommendEngine.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyCategoryRecommendByRecommendEngine" Src="~/Form/Common/Product/BodyCategoryRecommendByRecommendEngine.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProductColorSearchBox" Src="~/Form/Common/Product/ProductColorSearchBox.ascx" %>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Register TagPrefix="uc" TagName="BodyProductArrivalMailRegisterTr" Src="~/Form/Common/Product/BodyProductArrivalMailRegisterTr.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductCategoryHtml" Src="~/Form/Common/Product/BodyProductCategoryHtml.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductSortBox" Src="~/Form/Common/Product/BodyProductSortBox.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductCategoryLinks" Src="~/Form/Common/Product/BodyProductCategoryLinks.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductVariationImages" Src="~/Form/Common/Product/BodyProductVariationImages.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductGroupContentsHtml" Src="~/Form/Common/Product/BodyProductGroupContentsHtml.ascx" %>
<%@ Register TagPrefix="uc" TagName="Criteo" Src="~/Form/Common/Criteo.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyRecommendTagsRelatedCategory" Src="~/Form/Common/Product/BodyRecommendTagsRelatedCategory.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProductDetailModal" Src="~/Form/Common/Product/ProductDetailModal.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Form/Product/ProductList.aspx.cs" Inherits="Form_Product_ProductList" Title="商品一覧ページ" %>
<%@ Import Namespace="ProductListDispSetting" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。In
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<link href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT) %>Css/product.css" rel="stylesheet" type="text/css" media="all" />
<link rel="canonical" href="<%# CreateProductListCanonicalUrl() %>" />
<% if (Constants.MOBILEOPTION_ENABLED){%>
	<link rel="Alternate" media="handheld" href="<%= WebSanitizer.HtmlEncode(GetMobileUrl()) %>" />
<% } %>
<%= this.BrandAdditionalDsignTag %>
<% if (Constants.SEOTAG_IN_PRODUCTLIST_ENABLED){ %>
	<meta name="Keywords" content="<%: this.SeoKeywords %>" />
<% } %>
<%-- △編集可能領域△ --%>
<%# this.PaginationTag %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<script type="text/javascript">
	$(function () {

		// 商品一覧:詳細検索
		function getUrlVars() {
			var vars = [], hash;
			var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
			for (var i = 0; i < hashes.length; i++) {
				hash = hashes[i].split('=');
				vars.push(hash[0]);
				vars[hash[0]] = hash[1];
			}
			return vars;
		}

		//ソート
		$(".btn-sort-search").click(function () {

			var urlVars = getUrlVars();

			// 店舗ID
			var $shop = (urlVars["<%= Constants.REQUEST_KEY_SHOP_ID %>"] == undefined)
				? "<%= Constants.CONST_DEFAULT_SHOP_ID %>"
				: urlVars["<%= Constants.REQUEST_KEY_SHOP_ID %>"];
			// カテゴリ及びカテゴリ名
			if ($(".sort-category select").val() != "") {
				var $cat = $(".sort-category select").val();
				var $catName = $(".sort-category select option:selected").text();
			} else {
				var $cat = "";
				var $catName = "";
			}
			// ブランド
			var $bid = urlVars["<%= Constants.REQUEST_KEY_BRAND_ID %>"];
			var $brand = "<%= this.BrandName %>";
			if ($("input[name='iBrand']").length != 0) {
				if (("<%= Constants.PRODUCT_BRAND_ENABLED %>" == "True") && ($("input[name='iBrand']:checked").val() != undefined)) {
					$bid  = $("input[name='iBrand']:checked").val();
				} else {
					$bid = undefined;
				}
			}

			// Product Group ID
			var $productGroupId = "&<%= Constants.REQUEST_KEY_PRODUCT_GROUP_ID %>=";
			$productGroupId = $productGroupId + ((urlVars["<%= Constants.REQUEST_KEY_PRODUCT_GROUP_ID %>"] == undefined) ? "" : urlVars["<%= Constants.REQUEST_KEY_PRODUCT_GROUP_ID %>"]);
			// キャンペーンアイコン
			var $cicon = "&<%= Constants.REQUEST_KEY_CAMPAINGN_ICOM %>="
				+ ((urlVars["<%= Constants.REQUEST_KEY_CAMPAINGN_ICOM %>"] == undefined)
					? ""
					: urlVars["<%= Constants.REQUEST_KEY_CAMPAINGN_ICOM %>"]);
			// 特別価格商品の表示
			var $dosp = "&<%= Constants.REQUEST_KEY_DISP_ONLY_SP_PRICE %>="
				+ ((urlVars["<%= Constants.REQUEST_KEY_DISP_ONLY_SP_PRICE %>"] == undefined)
					? ""
					: urlVars["<%= Constants.REQUEST_KEY_DISP_ONLY_SP_PRICE %>"]);
			// 表示件数
			if ($("input[name='dpcnt']:checked").val() != "") {
				var $dpcnt = "&<%= Constants.REQUEST_KEY_DISP_PRODUCT_COUNT %>="
					+ $("input[name='dpcnt']:checked").val();
			} else {
				var $dpcnt = "&<%= Constants.REQUEST_KEY_DISP_PRODUCT_COUNT %>="
					+ ((urlVars["<%= Constants.REQUEST_KEY_DISP_IMG_KBN %>"] == "<%= Constants.KBN_REQUEST_DISP_IMG_KBN_ON %>")
						? "<%= ProductListDispSettingUtility.CountDispContentsImgOn %>"
						: "<%= ProductListDispSettingUtility.CountDispContentsWindowShopping %>");
			}
			// 画像表示区分
			var $img = "&<%= Constants.REQUEST_KEY_DISP_IMG_KBN %>="
				+ ((urlVars["<%= Constants.REQUEST_KEY_DISP_IMG_KBN %>"] == undefined)
					? ""
					: urlVars["<%= Constants.REQUEST_KEY_DISP_IMG_KBN %>"]);
			// 価格帯
			if ($("input[name='price']:checked").val() != "") {
				var price = $("input[name='price']:checked").val();
				priceValue = price.split(",");
				var $min = "&<%= Constants.REQUEST_KEY_MIN_PRICE %>=" + priceValue[0];
				var $max = "&<%= Constants.REQUEST_KEY_MAX_PRICE %>=" + priceValue[1];
			} else {
				var $min = "&<%= Constants.REQUEST_KEY_MIN_PRICE %>=";
				var $max = "&<%= Constants.REQUEST_KEY_MAX_PRICE %>=";
			}
			// 表示順
			if ($("input[name='sort']:checked").val() != "") {
				var $sort = "&<%= Constants.REQUEST_KEY_SORT_KBN %>=" + $("input[name='sort']:checked").val();
			} else {
				var $sort = "&<%= Constants.REQUEST_KEY_SORT_KBN %>=<%= ProductListDispSettingUtility.SortDefault %>";
			}
			// キーワード
			if ($(".sort-word input").val() != "") {
				var $swrd = "&<%= Constants.REQUEST_KEY_SEARCH_WORD %>=" + $(".sort-word input").val();
			} else {
				var $swrd = "&<%= Constants.REQUEST_KEY_SEARCH_WORD %>=";
			}
			// 在庫
			if ($("input[name='udns']:checked").val() != "") {
				var $udns = "&<%= Constants.REQUEST_KEY_UNDISPLAY_NOSTOCK_PRODUCT %>=" + $("input[name='udns']:checked").val();
			} else {
				var $udns = "&<%= Constants.REQUEST_KEY_UNDISPLAY_NOSTOCK_PRODUCT %>=";
			}
			// 定期購入フィルタ
			if ($("input[name=<%= Constants.FORM_NAME_FIXED_PURCHASE_RADIO_BUTTON %>]:checked").val() != "") {
				var $fpfl = "&<%= Constants.REQUEST_KEY_FIXED_PURCHASE_FILTER %>=" + $("input[name=<%= Constants.FORM_NAME_FIXED_PURCHASE_RADIO_BUTTON %>]:checked").val();
			} else {
				var $fpfl = "&<%= Constants.REQUEST_KEY_FIXED_PURCHASE_FILTER %>=";
			}
			// 頒布会キーワード
			if ($(".sort-word input").val() != "") {
				var $sbswrd = "&<%= Constants.REQUEST_KEY_SUBSCRIPTION_BOX_SEARCH_WORD %>=" + $(".sort-word input").val();
			} else {
				var $sbswrd = "&<%= Constants.REQUEST_KEY_SUBSCRIPTION_BOX_SEARCH_WORD %>=";
			}

			// 指定したURLにジャンプ(1ページ目へ)
			if (("<%= Constants.FRIENDLY_URL_ENABLED %>" == "True") && ($catName != "")) {
				if (("<%= Constants.PRODUCT_BRAND_ENABLED %>" == "True") && ($brand != "")) {
					var rootUrl = "<%= Constants.PATH_ROOT %>" + $brand + "-" + $catName + "/brandcategory/" + $bid + "/" + $shop + "/" + $cat + "/?";
				} else {
					var rootUrl = "<%= Constants.PATH_ROOT %>" + $catName + "/category/" + $shop + "/" + $cat + "/?" + (($bid != undefined) ? "<%= Constants.REQUEST_KEY_BRAND_ID %>=" + $bid : "");
				}
			} else {
				var rootUrl = "<%= Constants.PATH_ROOT %><%= Constants.PAGE_FRONT_PRODUCT_LIST %>?<%= Constants.REQUEST_KEY_SHOP_ID %>=" + $shop
					+ "&<%= Constants.REQUEST_KEY_CATEGORY_ID %>=" + $cat + (($bid != undefined) ? "&<%= Constants.REQUEST_KEY_BRAND_ID %>=" + $bid : "");
			}
			location.href = rootUrl + $productGroupId + $cicon + $dosp + $dpcnt + $img + $max + $min + $sort + $swrd + $udns + $fpfl + $sbswrd + "&<%= Constants.REQUEST_KEY_PAGE_NO %>=1";
		});

	});

	function enterSearch() {
		//EnterキーならSubmit
		if (window.event.keyCode == 13) document.formname.submit();
	}

</script>

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

<!--▽ 上部カテゴリリンク ▽-->
<div id="breadcrumb">
<uc:BodyProductCategoryLinks runat="server"></uc:BodyProductCategoryLinks>
</div>
<!--△ 上部カテゴリリンク △-->

<!--▽ カテゴリHTML領域 ▽-->
<uc:BodyProductCategoryHtml runat="server" />
<!--△ カテゴリHTML領域 △-->

<!--▽ 商品グループページHTML領域 ▽-->
<uc:BodyProductGroupContentsHtml runat="server" />
<!--△ 商品グループページHTML領域 △-->

<uc:BodyProductAdvancedSearchBox runat="server" />

<!--▽ ソートコントロール ▽-->
<uc:BodyProductSortBox CategoryName="<%# this.CategoryName %>" runat="server"></uc:BodyProductSortBox>
<!--△ ソートコントロール △-->

<!--▽ ページャ ▽-->
<% if (this.IsInfiniteLoad == false) { %>
<div id="pagination" class="above clearFix">
<%# this.PagerHtml %>
</div>
<% } %>
<!--△ ページャ △-->

<div class="listProduct">

<%-- カート投入ボタン押下時にどの画面へ遷移するか？ --%>
<%-- CART：カート一覧画面 その他：画面遷移しない --%>
<asp:HiddenField ID="hfIsRedirectAfterAddProduct" Value="CART" runat="server" />

<%-- お気に入り追加ボタン押下時にどの画面へ遷移するか？ --%>
<%-- true:ポップアップ表示、false:お気に入り一覧ページへ遷移 --%>
<% IsDisplayPopupAddFavorite = true; %>
<div>
<p id="addFavoriteTip" class="toolTip" style="display: none; position: fixed;">
	<span style="margin: 10px;" id="txt-tooltip"></span>
	<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_FAVORITE_LIST) %>" class="btn btn-mini btn-inverse">お気に入り一覧</a>
</p>
</div>

<!-- 商品一覧ループ(通常表示) -->
<%-- UPDATE PANEL開始 --%>
	<asp:UpdatePanel runat="server">
		<ContentTemplate>
			<% if (this.IsInfiniteLoad) { %>
				<div id="loadingAnimationUpper" style="width:100%; padding:0" class="loading">
					<img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/loading.gif" style="opacity:0; display: block; width: 30px; height: 30px; margin: 0 auto 20px;"/>
				</div>
				<p><%: GetDisplayPageCountText(this.DisplayCount) %></p>
				<asp:HiddenField ID="hfPageNumber" runat="server"/>
				<asp:HiddenField ID="hfDisplayPageNumberMax" runat="server"/>
			<% } %>
			<asp:Repeater ID="rTopProductList" runat="server">
			<ItemTemplate>
				<div class="infiniteLoadProducts listProduct" runat="server">
				<%-- ▽商品一覧ループ(通常表示)▽ --%>
				<asp:Repeater ID="rProductsListView" Visible="<%# this.IsDispImageKbnOn %>" runat="server">
				<HeaderTemplate>
				<ul>
				</HeaderTemplate>
				<ItemTemplate>
				<li class="productList" id="dInfiniteLoadProduct" runat="server">
				<ul>
				<li>
				<ul class="clearFix">
				<!-- 商品画像表示 -->
				<li class="plPhoto">
				<% if(Constants.LAYER_DISPLAY_VARIATION_IMAGES_ENABLED){ %>
				<uc:BodyProductVariationImages ImageSize="M" ProductMaster="<%# Container.DataItem %>" VariationList="<%# this.ProductVariationList %>" VariationNo="<%# Container.ItemIndex.ToString() %>" runat="server" />
				<% } else { %>
				<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem, true)) %>'>
				<w2c:ProductImage ImageSize="M" ProductMaster="<%# Container.DataItem %>" IsVariation="false" runat="server" /></a>
				<% } %>
				<%-- ▽在庫切れ可否▽ --%>
				<p visible='<%# ProductListUtility.IsProductSoldOut(Container.DataItem) %>' runat="server" class="soldout">SOLDOUT</p>
				<%-- ▽在庫切れ可否▽ --%>
				</li>
				<li class="plProductInfo">
				<ul>
				<li class="plName">
				<!-- アイコン表示 -->
				<p>
				<w2c:ProductIcon IconNo="1" ProductMaster="<%# Container.DataItem %>" runat="server" />
				<w2c:ProductIcon IconNo="2" ProductMaster="<%# Container.DataItem %>" runat="server" />
				<w2c:ProductIcon IconNo="3" ProductMaster="<%# Container.DataItem %>" runat="server" />
				<w2c:ProductIcon IconNo="4" ProductMaster="<%# Container.DataItem %>" runat="server" />
				<w2c:ProductIcon IconNo="5" ProductMaster="<%# Container.DataItem %>" runat="server" />
				<w2c:ProductIcon IconNo="6" ProductMaster="<%# Container.DataItem %>" runat="server" />
				<w2c:ProductIcon IconNo="7" ProductMaster="<%# Container.DataItem %>" runat="server" />
				<w2c:ProductIcon IconNo="8" ProductMaster="<%# Container.DataItem %>" runat="server" />
				<w2c:ProductIcon IconNo="9" ProductMaster="<%# Container.DataItem %>" runat="server" />
				<w2c:ProductIcon IconNo="10" ProductMaster="<%# Container.DataItem %>" runat="server" />
				</p>
				<!-- 商品名表示 -->
				<h3><a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem, true)) %>'>
				<%# WebSanitizer.HtmlEncode(GetProductData(Container.DataItem, "name")) %></a></h3>
				<!-- TODO:表示テスト -->
				<!-- 商品ID表示 -->
				[<span class="productId"><%# WebSanitizer.HtmlEncode(GetProductData(Container.DataItem, "product_id")) %></span>]
				</li>
				<li class="plExcerpt">
				<!-- 概要表示 -->
				<%# GetProductDataHtml(Container.DataItem, "outline") %>
				</li>
				<li>
				<asp:Repeater ID="rSetPromotion" DataSource="<%# GetSetPromotionByProduct((DataRowView)Container.DataItem) %>" runat="server">
				<ItemTemplate>
				<span visible='<%# ((SetPromotionModel)Container.DataItem).Url != "" %>' runat="server">
					<a href="<%# WebSanitizer.HtmlEncode(Constants.PATH_ROOT + ((SetPromotionModel)Container.DataItem).Url) %>"><%# WebSanitizer.HtmlEncode(((SetPromotionModel)Container.DataItem).SetpromotionDispName) %></a><br />
				</span>
				<span visible='<%# (string)Eval("Url") == "" %>' runat="server">
					<%# WebSanitizer.HtmlEncode(((SetPromotionModel)Container.DataItem).SetpromotionDispName) %><br />
				</span>
				</ItemTemplate>
				</asp:Repeater>
				</li>

				<li class="plPrice">

				<%-- ▽商品会員ランク価格有効▽ --%>
				<span visible='<%# GetProductMemberRankPriceValid(Container.DataItem) %>' runat="server">
				販売価格:<span class="productPrice"><strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %>（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(Container.DataItem)) %>）</strike></span><br />
				<span>会員ランク価格:<%#: CurrencyManager.ToPrice(ProductPage.GetProductMemberRankPrice(Container.DataItem)) %></span>(<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(Container.DataItem)) %>)
				</span>
				<%-- △商品会員ランク価格有効△ --%>
				<%-- ▽商品セール価格有効▽ --%>
				<span visible='<%# GetProductTimeSalesValid(Container.DataItem) %>' runat="server">
				販売価格:<span class="productPrice"><strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %>（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(Container.DataItem)) %>）</strike></span><br />
				<span>タイムセールス価格:<%#: CurrencyManager.ToPrice(ProductPage.GetProductTimeSalePriceNumeric(Container.DataItem)) %></span>（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(Container.DataItem)) %>）
				</span>
				<%-- △商品セール価格有効△ --%>
				<%-- ▽商品特別価格有効▽ --%>
				<span visible='<%# GetProductSpecialPriceValid(Container.DataItem) %>' runat="server">
				販売価格:<span class="productPrice"><strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %>（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(Container.DataItem)) %>）</strike></span><br />
				<span>特別価格:<%#: CurrencyManager.ToPrice(ProductPage.GetProductSpecialPriceNumeric(Container.DataItem)) %></span>（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(Container.DataItem)) %>）
				</span>
				<%-- △商品特別価格有効△ --%>
				<%-- ▽商品通常価格有効▽ --%>
				<span visible='<%# GetProductNormalPriceValid(Container.DataItem) %>' runat="server">
				販売価格:<span class="productPrice"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></span>（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(Container.DataItem)) %>）
				</span>
				<%-- △商品通常価格有効△ --%>
				<%-- ▽商品加算ポイント▽ --%>
				<p visible='<%# (this.IsLoggedIn && (GetProductAddPointString(Container.DataItem) != "")) %>' runat="server">
					<span class="addPoint">ポイント<%# WebSanitizer.HtmlEncode(GetProductAddPointString(Container.DataItem)) %></span><span id="Span1" visible='<%# ((string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_POINT_KBN1)) != Constants.FLG_PRODUCT_POINT_KBN1_NUM %>' runat="server">(<%# WebSanitizer.HtmlEncode(GetProductAddPointCalculateAfterString(Container.DataItem, false, false))%>)
					</span>
				</p>
				<%-- △商品加算ポイント△ --%>
				<%-- ▽商品定期購入価格▽ --%>
				<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) {%>
				<span visible='<%# (GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG).ToString() != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) && ((CheckFixedPurchaseLimitedUserLevel(this.ShopId, (string)GetProductData(Container.DataItem, "product_id")) == false)) %>' runat="server">
					<span visible='<%# IsProductFixedPurchaseFirsttimePriceValid(Container.DataItem) %>' runat="server">
						<br />
						定期初回価格:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchaseFirsttimePrice(Container.DataItem)) %>（<%#: GetTaxIncludeString(Container.DataItem) %>）
					</span>
					<br />
					定期通常価格:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchasePrice(Container.DataItem)) %>（<%#: GetTaxIncludeString(Container.DataItem) %>）
				</span>
				<% } %>
				<%-- △商品定期購入価格△ --%>
				<%-- ▽定期商品加算ポイント▽ --%>
				<p visible='<%# (this.IsLoggedIn && (GetProductAddPointString(Container.DataItem) != "") && (GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG).ToString() != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) && ((CheckFixedPurchaseLimitedUserLevel(this.ShopId, (string)GetProductData(Container.DataItem, "product_id")) == false))) %>' runat="server">
					<span class="addPoint">ポイント<%# WebSanitizer.HtmlEncode(GetProductAddPointString(Container.DataItem, false, false, true)) %></span>
					<span visible='<%# ((string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_POINT_KBN2)) != Constants.FLG_PRODUCT_POINT_KBN1_NUM %>' runat="server">(<%# WebSanitizer.HtmlEncode(GetProductAddPointCalculateAfterString(Container.DataItem, false, false, true))%>)
					</span>
				</p>
				<%-- △定期商品加算ポイント△ --%>

				<%-- ▽商品タグ項目：メーカー▽ --%>
				<span visible='<%# StringUtility.ToEmpty(GetKeyValue(Container.DataItem, "tag_manufacturer")) != "" %>' runat="server">
				メーカー:<%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, "tag_manufacturer")) %>
				</span>
				<%-- △商品タグ項目：メーカー△ --%>

				<%-- ▽お気に入りの登録人数表示▽ --%>
				<p visible="true" runat="server" class="favoriteRegistration">
				お気に入りの登録人数：<%# this.GetFavoriteCount((string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID)) %>人
				</p>
				<%-- △お気に入りの登録人数表示△ --%>

				<%-- ▽お気に入り追加▽ --%>
				<asp:LinkButton ID="lbAddFavorite" runat="server"
				CommandName="FavoriteAdd" CommandArgument="<%# GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID) %>" class="btn btn-mini">
					<%# (FavoriteDisplayWord((string) GetKeyValue(Container.DataItem, Constants.FIELD_FAVORITE_SHOP_ID), this.LoginUserId, (string) GetKeyValue(Container.DataItem, Constants.FIELD_FAVORITE_PRODUCT_ID), "")) ? "お気に入り登録済み" : "お気に入りに追加" %>
				</asp:LinkButton>

				<%-- △お気に入り追加△ --%>
				</li>
				</ul>
				</li>
				</ul>
				</li>

				<%-- ▽バリエーションリストループ▽ --%>
				<%if (Constants.PRODUCTLIST_VARIATION_DISPLAY_ENABLED) { %>
				<li id="divProductListMultiVariation">
				<table>
				<tr>
				<th>表示名1</th> <%-- 表示名1 --%>
				<th>表示名2</th> <%-- 表示名2 --%>
				<th>価格</th> <%-- 価格 --%>
				<th>&nbsp;</th> <%-- カート投入ボタン・入荷通知メールボタン --%>
				</tr>
					<asp:Repeater ID="rAddCartVariationList" DataSource="<%# GetProductListVariation((DataRowView)Container.DataItem) %>"
						onitemcommand="AddCartVariationList_ItemCommand" runat="server" OnItemDataBound="rAddCartVariationList_ItemDataBound">
				<HeaderTemplate>
				</HeaderTemplate>
				<ItemTemplate>
				<tr>

				<%-- 表示名1 --%>
				<td>
				<%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1)) %>
				</td>

				<%-- 表示名2 --%>
				<td>
				<%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2)) %>
				</td>

				<%-- 価格 --%>
				<td>
				<%-- ▽商品会員ランク価格有効▽ --%>
				<div visible='<%# GetProductMemberRankPriceValid(Container.DataItem, true) %>' runat="server">
					<p>
					販売価格:<strike><%#: CurrencyManager.ToPrice(GetProductPriceNumeric(Container.DataItem, true)) %>（<%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, "TaxIncluded")) %>）</strike>
					</p>
					<p>
					会員ランク価格:<%#: CurrencyManager.ToPrice(GetProductMemberRankPrice(Container.DataItem, true)) %>(<%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, "TaxIncluded")) %>)
					</p>
				</div>
				<%-- △商品会員ランク価格有効△ --%>
				<%-- ▽商品セール価格有効▽ --%>
				<div visible='<%# GetProductTimeSalesValid(Container.DataItem) %>' runat="server">
					<p>
					販売価格:<strike><%#: CurrencyManager.ToPrice(GetProductPriceNumeric(Container.DataItem, true)) %>（<%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, "TaxIncluded")) %>）</strike>
					</p>
					<p>
					タイムセールス価格:<%#: CurrencyManager.ToPrice(ProductPage.GetProductTimeSalePriceNumeric(Container.DataItem)) %>（<%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, "TaxIncluded")) %>）
					</p>
				</div>
				<%-- △商品セール価格有効△ --%>
				<%-- ▽商品特別価格有効▽ --%>
				<div visible='<%# GetProductSpecialPriceValid(Container.DataItem, true) %>' runat="server">
					<p>
					販売価格:<strike><%#: CurrencyManager.ToPrice(GetProductPriceNumeric(Container.DataItem, true)) %>（<%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, "TaxIncluded")) %>）</strike>
					</p>
					<p>
				特別価格:<%#: CurrencyManager.ToPrice(GetProductSpecialPriceNumeric(Container.DataItem, true)) %></span>（<%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, "TaxIncluded")) %>）
					</p>
				</div>
				<%-- △商品特別価格有効△ --%>
				<%-- ▽商品通常価格有効▽ --%>
				<p visible='<%# GetProductNormalPriceValid(Container.DataItem, true) %>' runat="server">
				販売価格:<span class="productPrice"><%#: CurrencyManager.ToPrice(GetProductPriceNumeric(Container.DataItem, true)) %></span>（<%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, "TaxIncluded")) %>）</p>
				<%-- △商品通常価格有効△ --%>
				<%-- ▽商品加算ポイント▽ --%>
				<p visible='<%# (this.IsLoggedIn && (GetProductAddPointString(Container.DataItem, true, true) != "")) %>' runat="server">
				<span class="addPoint">
				ポイント：<%# WebSanitizer.HtmlEncode(GetProductAddPointString(Container.DataItem, true, true)) %></span><span visible='<%# ((string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_POINT_KBN1)) != Constants.FLG_PRODUCT_POINT_KBN1_NUM %>' runat="server">(<%# WebSanitizer.HtmlEncode(GetProductAddPointCalculateAfterString(Container.DataItem, true, true))%>)</span>
				</p>
				<%-- △商品加算ポイント△ --%>
				<%-- ▽商品定期購入価格 --%>
				<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) {%>
				<p visible='<%# (GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG).ToString() != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) && ((CheckFixedPurchaseLimitedUserLevel(this.ShopId, (string)GetProductData(Container.DataItem, "product_id")) == false)) %>' runat="server">
					<p visible='<%# IsProductFixedPurchaseFirsttimePriceValid(Container.DataItem, true) %>' runat="server">
						定期初回価格:<%#: CurrencyManager.ToPrice(GetProductFixedPurchaseFirsttimePrice(Container.DataItem, true)) %>（<%#: GetKeyValue(Container.DataItem, "TaxIncluded") %>）
					</p>
					<p>
						定期通常価格:<%#: CurrencyManager.ToPrice(GetProductFixedPurchasePrice(Container.DataItem, true)) %>（<%#: GetKeyValue(Container.DataItem, "TaxIncluded") %>）
					</p>
				</p>
				<% } %>
				<%-- △商品定期購入価格△ --%>
				<%-- ▽商品加算ポイント▽ --%>
				<p visible='<%# (this.IsLoggedIn && (GetProductAddPointString(Container.DataItem, true, true, true) != "") && (((bool)GetKeyValue(Container.DataItem, "CanFixedPurchase")) && ((CheckFixedPurchaseLimitedUserLevel(this.ShopId, (string)GetProductData(Container.DataItem, "product_id")) == false)))) %>' runat="server">
				<span class="addPoint">
				ポイント：<%# WebSanitizer.HtmlEncode(GetProductAddPointString(Container.DataItem, true, true, true)) %></span><span visible='<%# ((string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_POINT_KBN2)) != Constants.FLG_PRODUCT_POINT_KBN2_NUM %>' runat="server">(<%# WebSanitizer.HtmlEncode(GetProductAddPointCalculateAfterString(Container.DataItem, true, true, true))%>)</span>
				</p>

				<%-- △商品加算ポイント△ --%>
				</td>

				<td>
				<%-- カート投入ボタン --%>
				<p class="addCart">
				<%-- カートに入れるボタン表示 --%>
				<asp:PlaceHolder ID="phNotSubscriptionBoxOnly" runat="server">
				<div class="mb5">
				<asp:LinkButton ID="lbCartAddVariationList" runat="server" Visible='<%# (bool)GetKeyValue(Container.DataItem, "CanCart") %>' CommandName="CartAdd" class="btn btn-mid btn-inverse">
				カートに入れる
				</asp:LinkButton>
				</div>
				<%-- 定期購入ボタン表示 --%>
				<div class="mb5">
				<asp:LinkButton ID="lbCartAddFixedPurchaseVariationList" runat="server" Visible='<%# (((bool)GetKeyValue(Container.DataItem, "CanFixedPurchase")) && (this.IsUserFixedPurchaseAble)) %>' OnClientClick="return add_cart_check_for_fixedpurchase_variationlist();" CommandName="CartAddFixedPurchase" class="btn btn-mid btn-inverse">
				カートに入れる(定期購入)
				</asp:LinkButton>
				<span runat="server" Visible='<%# ((bool)GetKeyValue(Container.DataItem, "CanFixedPurchase")) && ((string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG) == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY) && (this.IsUserFixedPurchaseAble == false) %>' style="color: red;">定期購入のご利用はできません</span>
				</div>
				</asp:PlaceHolder>
				<%-- 頒布会 --%>
				<div id="subscriptionBox">
				<asp:LinkButton ID="lbCartAddSubscriptionBoxList" runat="server" Visible='<%# (((bool)GetKeyValue(Container.DataItem, Constants.ADD_CART_INFO_CAN_SUBSCRIPTION_BOX)) && this.IsUserFixedPurchaseAble) %>' OnClientClick="return add_cart_check_for_subscriptionBox_variationlist(this);" CommandName="CartAddSubscriptionBox" class="btn btn-mid btn-inverse">
					頒布会申し込み
				</asp:LinkButton>
				<asp:HiddenField ID="hfVariation" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) %>" runat="server"/>
				<asp:HiddenField ID="htShopId" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_SHOP_ID) %>" runat="server"/>
				<asp:HiddenField ID="hfProduct" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID) %>" runat="server"/>
				<asp:HiddenField ID="hfSubscriptionBoxFlg" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG) %>" runat="server"/>
				<asp:HiddenField ID="hfCanFixedPurchase" Value='<%# (bool)GetKeyValue(Container.DataItem, "CanFixedPurchase") %>' runat="server"/>
				<asp:HiddenField ID="hfFixedPurchase" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG) %>" runat="server"/>
				<asp:HiddenField ID="hfSubscriptionBoxDisplayName" runat="server"/>
				<asp:DropDownList ID="ddlSubscriptionBox" DataTextField="DisplayName" DataValueField="CourseID" runat="server" Visible="false"></asp:DropDownList>
				</div>
				<%-- ギフト購入ボタン表示 --%>
				<div class="mb5">
				<asp:LinkButton ID="lbCartAddForGiftVariationList" runat="server" Visible='<%# (bool)GetKeyValue(Container.DataItem, "CanGiftOrder") %>' CommandName="CartAddGift" class="btn btn-mid btn-inverse">
				カートに入れる(ギフト購入)
				</asp:LinkButton>
				</div>
				<% if (Constants.VARIATION_FAVORITE_CORRESPONDENCE){ %>
				<%-- お気に入りに追加ボタン表示 --%>
				<div class="mb5">
				<asp:LinkButton ID="lbAddFavoriteVariation" runat="server" CommandName="AddFavorite" class="btn btn-mid">
					<%# (FavoriteDisplayWord(this.ShopId, this.LoginUserId, (string)GetKeyValue(Container.DataItem, Constants.FIELD_FAVORITE_PRODUCT_ID), (string)GetKeyValue(Container.DataItem, Constants.FIELD_FAVORITE_VARIATION_ID))) ? "お気に入り登録済み" : "お気に入りに追加" %>
				</asp:LinkButton>
				</div>
				<% } %>
				</p>
					
				<%-- 入荷通知メールボタン --%>
				<p class="arrivalMailButton">
				<%-- 再入荷通知メール申し込みボタン表示 --%>
				<div visible='<%# ((string)GetKeyValue(Container.DataItem, "ArrivalMailKbn") == Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL) %>' runat="server">
				<asp:LinkButton CommandName="SmartArrivalMail" CommandArgument="Arrival" Runat="server" class="btn btn-mid btn-inverse">
				入荷お知らせメール申込
				</asp:LinkButton>
				</div>
				<%-- 販売開始通知メール申し込みボタン表示 --%>
				<div visible='<%# ((string)GetKeyValue(Container.DataItem, "ArrivalMailKbn") == Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE) %>' runat="server">
				<asp:LinkButton CommandName="SmartArrivalMail" CommandArgument="Release" Runat="server" class="btn btn-mid btn-inverse">
				販売開始通知メール申込
				</asp:LinkButton>
				</div>
				<%-- 再販売通知メール申し込みボタン表示 --%>
				<div visible='<%# ((string)GetKeyValue(Container.DataItem, "ArrivalMailKbn") == Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE) %>' runat="server">
				<asp:LinkButton CommandName="SmartArrivalMail" CommandArgument="Resale" Runat="server" class="btn btn-mid btn-inverse">
				再販売通知メール申込
				</asp:LinkButton>
				</div>
				<%-- エラー表示 --%>
				<p class="error"><%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, "ErrorMessage")) %></p>
				</p>	

				<p>
				<asp:Repeater ID="rSetPromotionByVariation" DataSource="<%# GetSetPromotionByVariation((Dictionary<string, object>)Container.DataItem) %>" runat="server">
				<ItemTemplate>
				<%# WebSanitizer.HtmlEncode(((SetPromotionModel)Container.DataItem).SetpromotionDispName) %><br />
				</ItemTemplate>
				</asp:Repeater>
				</p>

				<%-- 隠しフィールド --%>
				<asp:HiddenField ID="hfProductId" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID) %>" runat="server" />
				<asp:HiddenField ID="hfVariationId" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) %>" runat="server" />
				<asp:HiddenField ID="hfArrivalMailKbn" Value='<%# GetKeyValue(Container.DataItem, "ArrivalMailKbn") %>' runat="server" />

				</td>
				</tr>

				<%-- 再入荷通知メール登録フォーム表示 --%>
				<uc:BodyProductArrivalMailRegisterTr runat="server" ID="ucBpamrArrival" ArrivalMailKbn="<%#: Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL %>" ProductId="<%#: GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_PRODUCT_ID) %>" VariationId="<%#: GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) %>" Visible="false" />
				<%-- 販売開始通知メール登録フォーム表示 --%>
				<uc:BodyProductArrivalMailRegisterTr runat="server" ID="ucBpamrRelease" ArrivalMailKbn="<%#: Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE %>" ProductId="<%#: GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_PRODUCT_ID) %>" VariationId="<%#: GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) %>" Visible="false" />
				<%-- 再販売知メール登録フォーム表示 --%>
				<uc:BodyProductArrivalMailRegisterTr runat="server" ID="ucBpamrResale" ArrivalMailKbn="<%#: Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE %>" ProductId="<%#: GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_PRODUCT_ID) %>" VariationId="<%#: GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) %>" Visible="false" />

				</ItemTemplate>
				<FooterTemplate>
				</FooterTemplate>
				</asp:Repeater>
				</table>
				</li>
				<% } %>
				<%-- △バリエーションリストループ△ --%>
				</ul>
				</li>
				</ItemTemplate>
				<FooterTemplate>
				</ul>
				</FooterTemplate>
				</asp:Repeater>
				<%-- △商品一覧ループ(通常表示)△ --%>
				<%-- ▽商品一覧ループ(ウインドウショッピング)▽ --%>
				<asp:Repeater ID="rProductsWindowShopping" runat="server" Visible="<%# this.IsDispImageKbnWindowsShopping %>" OnItemCommand="InnerRepeater_ItemCommand">
				<HeaderTemplate>
				<div class="heightLineParent clearFix">
				</HeaderTemplate>
				<ItemTemplate>
				<div id="dInfiniteLoadProduct" class="glbPlist column5 windowpanel" runat="server">

				<ul>
				<li class="icon">
				<w2c:ProductIcon IconNo="1" ProductMaster="<%# Container.DataItem %>" runat="server" />
				<w2c:ProductIcon IconNo="2" ProductMaster="<%# Container.DataItem %>" runat="server" />
				<w2c:ProductIcon IconNo="3" ProductMaster="<%# Container.DataItem %>" runat="server" />
				<w2c:ProductIcon IconNo="4" ProductMaster="<%# Container.DataItem %>" runat="server" />
				<w2c:ProductIcon IconNo="5" ProductMaster="<%# Container.DataItem %>" runat="server" />
				<w2c:ProductIcon IconNo="6" ProductMaster="<%# Container.DataItem %>" runat="server" />
				<w2c:ProductIcon IconNo="7" ProductMaster="<%# Container.DataItem %>" runat="server" />
				<w2c:ProductIcon IconNo="8" ProductMaster="<%# Container.DataItem %>" runat="server" />
				<w2c:ProductIcon IconNo="9" ProductMaster="<%# Container.DataItem %>" runat="server" />
				<w2c:ProductIcon IconNo="10" ProductMaster="<%# Container.DataItem %>" runat="server" />
				</li>
				<li class="thumb">
				<% if(Constants.LAYER_DISPLAY_VARIATION_IMAGES_ENABLED
					&& (Constants.SETTING_PRODUCT_LIST_SEARCH_KBN == false)) { %>
				<uc:BodyProductVariationImages ImageSize="M" ProductMaster="<%# Container.DataItem %>" VariationList="<%# this.ProductVariationList %>" VariationNo="<%# Container.ItemIndex.ToString() %>" runat="server" />
				<% } else { %>
				<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem, true)) %>'>
				<% if (Constants.SETTING_PRODUCT_LIST_SEARCH_KBN) { %>
					<w2c:ProductImage ImageSize="M" ProductMaster="<%# Container.DataItem %>" IsVariation="false" IsGroupVariation="true" runat="server" />
				<% } else { %>
					<w2c:ProductImage ImageSize="M" ProductMaster="<%# Container.DataItem %>" IsVariation="false" runat="server" />
				<% } %>
				</a>
				<% } %><span visible='<%# ProductListUtility.IsProductSoldOut(Container.DataItem) %>' runat="server" class="soldout">SOLDOUT</span>
				</li>
				<li class="name">
					<p class="pid"><%# WebSanitizer.HtmlEncode(GetProductData(Container.DataItem, Constants.FIELD_PRODUCT_PRODUCT_ID)) %></p>
					<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem, true)) %>'><%# WebSanitizer.HtmlEncode(GetProductData(Container.DataItem, "name")) %></a>
				<!-- 商品ID表示 -->
						<p><%#: StringUtility.ToEmpty(ProductPage.GetKeyValueToNull(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1)) %></p>
				</li>
				<li class="price">

				<p>
					<%# CurrencyManager.ToPrice(ProductPage.GetKeyValueToNull(
						Container.DataItem,
						Constants.SETTING_PRODUCT_LIST_SEARCH_KBN
							? Constants.FIELD_PRODUCTVARIATION_PRICE
							: Constants.FIELD_PRODUCT_DISPLAY_PRICE)) %>
					<%# StringUtility.ToEmpty(GetVariationPriceTax(Container.DataItem)) %>
				</p>

				<%-- ▽商品会員ランク価格有効▽ --%>
				<p visible='<%# GetProductMemberRankPriceValid(Container.DataItem, Constants.SETTING_PRODUCT_LIST_SEARCH_KBN) %>' runat="server">
				<span style="text-decoration: line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem, Constants.SETTING_PRODUCT_LIST_SEARCH_KBN)) %>（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(Container.DataItem)) %>）</span><br />
				<span style="color: #f00;"><%#: CurrencyManager.ToPrice(ProductPage.GetProductMemberRankPrice(Container.DataItem, Constants.SETTING_PRODUCT_LIST_SEARCH_KBN)) %>（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(Container.DataItem)) %>）</span>
				</p>

				<%-- ▽商品セール価格有効▽ --%>
				<p visible='<%# GetProductTimeSalesValid(Container.DataItem, Constants.SETTING_PRODUCT_LIST_SEARCH_KBN) %>' runat="server">
					<span style="text-decoration: line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem, Constants.SETTING_PRODUCT_LIST_SEARCH_KBN)) %>（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(Container.DataItem)) %>）</span><br />
					<span style="color: #f00;"><%#: CurrencyManager.ToPrice(ProductPage.GetProductTimeSalePriceNumeric(Container.DataItem)) %>（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(Container.DataItem)) %>）</span>
				</p>

				<%-- ▽商品特別価格有効▽ --%>
				<p visible='<%# GetProductSpecialPriceValid(Container.DataItem, Constants.SETTING_PRODUCT_LIST_SEARCH_KBN) %>' runat="server">
				<span style="text-decoration: line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem, Constants.SETTING_PRODUCT_LIST_SEARCH_KBN)) %>（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(Container.DataItem)) %>）</span><br />
				<span style="color: #f00;"><%#: CurrencyManager.ToPrice(ProductPage.GetProductSpecialPriceNumeric(Container.DataItem, Constants.SETTING_PRODUCT_LIST_SEARCH_KBN)) %>（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(Container.DataItem)) %>）</span>
				</p>

				<%-- ▽商品通常価格有効▽ --%>
				<p visible='<%# GetProductNormalPriceValid(Container.DataItem, Constants.SETTING_PRODUCT_LIST_SEARCH_KBN) %>' runat="server">
				<%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem, Constants.SETTING_PRODUCT_LIST_SEARCH_KBN)) %>（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(Container.DataItem)) %>）
				</p>
				<%-- ▽定期購入価格有効▽ --%>
				<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) {%>
				<p visible='<%# (GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG).ToString() != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) && (CheckFixedPurchaseLimitedUserLevel(this.ShopId, (string)GetProductData(Container.DataItem, "product_id")) == false) %>' runat="server">
					<p visible='<%# IsProductFixedPurchaseFirsttimePriceValid(Container.DataItem, Constants.SETTING_PRODUCT_LIST_SEARCH_KBN) %>' runat="server">
						定期初回:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchaseFirsttimePrice(Container.DataItem, Constants.SETTING_PRODUCT_LIST_SEARCH_KBN)) %>（<%#: GetTaxIncludeString(Container.DataItem) %>）
					</p>
					<p>
						定期通常:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchasePrice(Container.DataItem, Constants.SETTING_PRODUCT_LIST_SEARCH_KBN)) %>（<%#: GetTaxIncludeString(Container.DataItem) %>）
					</p>
				</p>
				<% } %>
				<%-- ▽頒布会購入価格有効▽ --%>
				<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED) {%>
				<p visible='<%# (GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG).ToString() != Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_INVALID) && (CheckFixedPurchaseLimitedUserLevel(this.ShopId, (string)GetProductData(Container.DataItem, "product_id")) == false) %>' runat="server">
					<p>
						頒布会通常:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchasePrice(Container.DataItem, Constants.SETTING_PRODUCT_LIST_SEARCH_KBN)) %>（<%#: GetTaxIncludeString(Container.DataItem) %>）
					</p>
				</p>
				<% } %>
				<%-- △頒布会購入価格有効△ --%>
				</li>
				<%-- ▽お気に入りの登録人数表示▽ --%>
				<li class="favorite" runat="server">
				お気に入りの登録人数：<%# this.GetFavoriteCount((string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID)) %>人
				</li>
				<%-- △お気に入りの登録人数表示△ --%>

				<% if (Constants.USE_MODAL_PRODUCT_LIST) { %>
					<a class="productlist_detailsLink" href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem, true)) %>">詳細ページへ</a>
					<a><asp:Button ID="btnShowDetail" runat="server" Text="注文する" CommandArgument="<%# GetProductData(Container.DataItem, Constants.FIELD_PRODUCT_PRODUCT_ID) %>" CommandName="btnOpenModalOrAddCart" CssClass="productlist_orderButton"/></a>
				<% } else { %>
					<a class="link_product_detail" href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem, true)) %>">▶ 詳細ページへ</a>
				<% } %>
				</ul>
				</div>
				</ItemTemplate>
				<FooterTemplate>
				</div>
				</FooterTemplate>
				</asp:Repeater>
				<%-- △商品一覧ループ(ウインドウショッピング)△ --%>
				</div>
			</ItemTemplate>
		</asp:Repeater>
		<% if (this.IsInfiniteLoad) { %>
			<asp:LinkButton ID="lbInfiniteLoadingUpperNextButton" CommandArgument="<%# LOADING_TYPE_UPPER %>" OnClick="lbInfiniteLoadingNextButton_OnClick" runat="server"></asp:LinkButton>
			<asp:LinkButton ID="lbInfiniteLoadingLowerNextButton" CommandArgument="<%# LOADING_TYPE_LOWER %>" OnClick="lbInfiniteLoadingNextButton_OnClick" runat="server"></asp:LinkButton>
			<p><%: GetDisplayPageCountText(this.DisplayCount) %></p>
			<div id="loadingAnimationLower" style="width:100%" class="loading">
				<img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/loading.gif" style="opacity:0; display: block; width: 30px; height: 30px; margin: 0 auto 20px;"/>
			</div>
			<div id="pagination" class="below clearFix">
				<asp:Label runat="server" ID="lbPagination" />
			</div>
		<% } %>
	</ContentTemplate>
	<Triggers>
		<asp:AsyncPostBackTrigger ControlID="lbInfiniteLoadingUpperNextButton" />
		<asp:AsyncPostBackTrigger ControlID="lbInfiniteLoadingLowerNextButton" />
	</Triggers>
</asp:UpdatePanel>
<%--△ 商品一覧（無限ロード）△--%>
<!--▽ ページャ ▽-->
<% if (this.IsInfiniteLoad == false) { %>
	<div id="pagination" class="below clearFix">
	<%# this.PagerHtml %>
	</div>
<% } %>
<!--△ ページャ △-->
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
<div visible="<%# (this.ProductMasterList.Count == 0) %>" runat="server" class="noProduct">
<!--▽ 商品が1つもなかった場合のエラー文言 ▽-->
<%# WebSanitizer.HtmlEncode(this.AlertMessage) %>
<!--△ 商品が1つもなかった場合のエラー文言 △-->
</div><!-- (this.ProductMasterList.Count != 0) -->

<%-- ▽最近チェックした商品▽ --%>
<uc:BodyProductHistory runat="server" />
<uc:BodyRecommendTagsRelatedCategory CategoryName="<%# this.CategoryName %>" runat="server"></uc:BodyRecommendTagsRelatedCategory>
<%-- △最近チェックした商品△ --%>

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

<script runat="server">
public new void Page_Load(Object sender, EventArgs e)
{
base.Page_Load(sender, e);

var recommendEngineUserControls = WebControlUtility.GetRecommendEngineUserControls(this.Form.FindControl("ContentPlaceHolder1"));
var lProductRecommendByRecommendEngineUserControls = recommendEngineUserControls.Item1;
var lCategoryRecommendByRecommendEngineUserControls = recommendEngineUserControls.Item2;

<%-- ▽編集可能領域：プロパティ設定▽ --%>
// 外部レコメンド連携パーツ設定
// 1つ目の商品レコメンド
if (lProductRecommendByRecommendEngineUserControls.Count > 0)
{
	// レコメンドコードを設定します
	lProductRecommendByRecommendEngineUserControls[0].RecommendCode = "pc211";
	// レコメンドタイトルを設定します
	lProductRecommendByRecommendEngineUserControls[0].RecommendTitle = "おすすめ商品一覧";
	// 商品最大表示件数を設定します
	lProductRecommendByRecommendEngineUserControls[0].MaxDispCount = 5;
	// レコメンド対象にするカテゴリIDを設定します（複数選択時はカンマ区切りで指定）
	lProductRecommendByRecommendEngineUserControls[0].DispCategoryId = "";
	// レコメンド非対象にするカテゴリIDを設定します（複数選択時はカンマ区切りで指定）
	lProductRecommendByRecommendEngineUserControls[0].NotDispCategoryId = "";
	// レコメンド非対象にするアイテムIDを設定します（複数選択時はカンマ区切りで指定）
	lProductRecommendByRecommendEngineUserControls[0].NotDispRecommendProductId = "";
}
<%-- △編集可能領域△ --%>
}
</script>

<script type="text/javascript">
<!--
	// バリエーション選択チェック(定期)
	function add_cart_check_for_fixedpurchase_variationlist() {
		return confirm('定期的に商品が送られてくる「定期購入」で購入します。\nよろしいですか？');
	}

	// 入荷通知登録画面をポップアップウィンドウで開く
	function show_arrival_mail_popup(pid, vid, amkbn) {
		show_popup_window('<%= this.SecurePageProtocolAndHost %><%= Constants.PATH_ROOT %><%= Constants.PAGE_FRONT_USER_PRODUCT_ARRIVAL_MAIL_REGIST %>?<%= Constants.REQUEST_KEY_PRODUCT_ID %>=' + pid + '&<%= Constants.REQUEST_KEY_VARIATION_ID %>=' + vid + '&<%= Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN %>=' + amkbn, 520, 310, true, true, 'Information');
	}

	// バリエーションリスト用選択チェック(頒布会)
	var subscriptionBoxMessage = '頒布会「@@ 1 @@」の内容確認画面に進みます。\nよろしいですか？';
	function add_cart_check_for_subscriptionBox_variationlist(value) {
		var subscriptionBoxName = ($(value).parent().find("[id$='ddlSubscriptionBox']").length > 0)
			? $(value).parent().find("[id$='ddlSubscriptionBox'] option:selected")[0].innerText
			: $(value).parent().find("[id$='hfSubscriptionBoxDisplayName']").val();
		return confirm(subscriptionBoxMessage.replace('@@ 1 @@', subscriptionBoxName));
	}

	// マウスイベントの初期化
	addOnload(function () { init(); });
//-->
</script>

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
			pagenationThreshold: 0.5
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

<%-- CRITEOタグ（引数：商品一覧情報） --%>
<uc:Criteo ID="criteo" runat="server" Datas="<%# this.ProductMasterList %>" />

</asp:Content>
