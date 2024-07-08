<%--
=========================================================================================================
  Module      : スマートフォン用商品一覧画面(ProductList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%@ Register TagPrefix="uc" TagName="BodyProductRecommendByRecommendEngine" Src="~/SmartPhone/Form/Common/Product/BodyProductRecommendByRecommendEngine.ascx" %>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Register TagPrefix="uc" TagName="BodyProductRecommend" Src="~/SmartPhone/Form/Common/Product/BodyProductRecommend.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductRecommendAdvanced" Src="~/SmartPhone/Form/Common/Product/BodyProductRecommendAdvanced.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductAdvancedSearchBox" Src="~/SmartPhone/Form/Common/Product/BodyProductAdvancedSearchBox.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductCategoryLinks" Src="~/SmartPhone/Form/Common/Product/BodyProductCategoryLinks.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductArrivalMailRegister" Src="~/SmartPhone/Form/Common/Product/BodyProductArrivalMailRegister.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductGroupContentsHtml" Src="~/Form/Common/Product/BodyProductGroupContentsHtml.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductSortBox" Src="~/SmartPhone/Form/Common/Product/BodyProductSortBox.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductVariationImages" Src="~/SmartPhone/Form/Common/Product/BodyProductVariationImages.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyRecommendTagsRelatedCategory" Src="~/SmartPhone/Form/Common/Product/BodyRecommendTagsRelatedCategory.ascx" %>
<%@ Register TagPrefix="uc" TagName="Criteo" Src="~/Form/Common/Criteo.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProductDetailModal" Src="~/SmartPhone/Form/Common/Product/ProductDetailModal.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Form/Product/ProductList.aspx.cs" Inherits="Form_Product_ProductList" Title="商品一覧ページ" %>
<%@ Import Namespace="ProductListDispSetting" %>
<%@ Import Namespace="w2.App.Common.DataCacheController" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<link href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "SmartPhone/Css/product.css") %>" rel="stylesheet" type="text/css" media="all" />
<script type="text/javascript" charset="UTF-8" src="<%= Constants.PATH_ROOT %>Js/jquery.biggerlink.min.js"></script>
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

		// 商品一覧:レイアウト切り替え
		$("#btn-layout-list").click(function () {
			$(".product-list-2").addClass("product-list");
			$(".product-list-2").removeClass("product-list-2");
			$(".layout-nav a").removeClass('active');
			$(this).addClass("active");
			$(".wrap-product-list > ul > li").heightLine("refresh");
			$(".modal-button").css("display", "none");
			sessionStorage.displayType = "list";
		});

		$("#btn-layout-list-2").click(function () {
			$(".product-list").addClass("product-list-2");
			$(".product-list").removeClass("product-list");
			$(".layout-nav a").removeClass('active');
			$(this).addClass("active");
			$(".wrap-product-list > ul > li").heightLine("refresh");
			$(".modal-button").css("display", "block");
			sessionStorage.displayType = "grid";
		});

		// レイアウトを引継ぐ
		if ((document.referrer.indexOf("<%= Constants.PAGE_FRONT_PRODUCT_LIST %>") != -1)
			&& (sessionStorage.displayType != undefined))
		{
			if (sessionStorage.displayType == "list")
			{
				$("#btn-layout-list").click();
			}
			else
			{
				$("#btn-layout-list-2").click();
			}
		} else {
			<% if (this.IsDispImageKbnOn){ %>
			$("#btn-layout-list").click();
			<% } else { %>
			$("#btn-layout-list-2").click();
			<% } %>
		}

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

		$(".btn-sort-search").click(function () {
			
			var urlVars = getUrlVars();

			// 店舗ID
			var $shop = (urlVars["<%= Constants.REQUEST_KEY_SHOP_ID %>"] == undefined)
				? "<%= Constants.CONST_DEFAULT_SHOP_ID %>"
				: encodeURIComponent(urlVars["<%= Constants.REQUEST_KEY_SHOP_ID %>"]);
			// カテゴリ及びカテゴリ名
			var $catId = "<%= this.CategoryId %>";
			if ($(".sort-category select").val() == $catId.substr(0, 3)) {
				var $cat = "<%= this.CategoryId %>";
				var $catName = "<%= this.CategoryName %>";
			} else if ($(".sort-category select").val() != "") {
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
					$bid = $("input[name='iBrand']:checked").val();
				} else {
					$bid = undefined;
				}
			}

			// Product Group ID
			var $productGroupId = "&<%= Constants.REQUEST_KEY_PRODUCT_GROUP_ID %>=";
			$productGroupId = $productGroupId + ((urlVars["<%= Constants.REQUEST_KEY_PRODUCT_GROUP_ID %>"] == undefined) ? "" : encodeURIComponent(urlVars["<%= Constants.REQUEST_KEY_PRODUCT_GROUP_ID %>"]));
			// キャンペーンアイコン
			var $cicon = "&<%= Constants.REQUEST_KEY_CAMPAINGN_ICOM %>="
				+ ((urlVars["<%= Constants.REQUEST_KEY_CAMPAINGN_ICOM %>"] == undefined)
					? ""
					: encodeURIComponent(urlVars["<%= Constants.REQUEST_KEY_CAMPAINGN_ICOM %>"]));
			// 特別価格商品の表示
			var $dosp = "&<%= Constants.REQUEST_KEY_DISP_ONLY_SP_PRICE %>="
				+ ((urlVars["<%= Constants.REQUEST_KEY_DISP_ONLY_SP_PRICE %>"] == undefined)
					? ""
					: encodeURIComponent(urlVars["<%= Constants.REQUEST_KEY_DISP_ONLY_SP_PRICE %>"]));
			// 表示件数
			if ($("input[name='dpcnt']:checked").val() != "") {
				var $dpcnt = "&<%= Constants.REQUEST_KEY_DISP_PRODUCT_COUNT %>="
					+ encodeURIComponent($("input[name='dpcnt']:checked").val());
			} else {
				var $dpcnt = "&<%= Constants.REQUEST_KEY_DISP_PRODUCT_COUNT %>="
					+ ((urlVars["<%= Constants.REQUEST_KEY_DISP_IMG_KBN %>"] == "<%= Constants.KBN_REQUEST_DISP_IMG_KBN_ON %>")
						? encodeURIComponent("<%= ProductListDispSettingUtility.CountDispContentsImgOn %>")
						: encodeURIComponent("<%= ProductListDispSettingUtility.CountDispContentsWindowShopping %>"));
			}
			// 画像表示区分
			var $img = "&<%= Constants.REQUEST_KEY_DISP_IMG_KBN %>="
				+ ((urlVars["<%= Constants.REQUEST_KEY_DISP_IMG_KBN %>"] == undefined)
					? ""
					: encodeURIComponent(urlVars["<%= Constants.REQUEST_KEY_DISP_IMG_KBN %>"]));
			// 価格帯
			if ($("input[name='price']:checked").val() != "") {
				var price = $("input[name='price']:checked").val();
				priceValue = price.split(",");
				var $min = "&<%= Constants.REQUEST_KEY_MIN_PRICE %>=" + encodeURIComponent(priceValue[0]);
				var $max = "&<%= Constants.REQUEST_KEY_MAX_PRICE %>=" + encodeURIComponent(priceValue[1]);
			} else {
				var $min = "&<%= Constants.REQUEST_KEY_MIN_PRICE %>=";
				var $max = "&<%= Constants.REQUEST_KEY_MAX_PRICE %>=";
			}
			// 表示順
			if ($("input[name='sort']:checked").val() != "") {
				var $sort = "&<%= Constants.REQUEST_KEY_SORT_KBN %>=" + encodeURIComponent($("input[name='sort']:checked").val());
			} else {
				var $sort = "&<%= Constants.REQUEST_KEY_SORT_KBN %>=<%= ProductListDispSettingUtility.SortDefault %>";
			}
			// キーワード
			if ($(".sort-word input").val() != "") {
				var $swrd = "&<%= Constants.REQUEST_KEY_SEARCH_WORD %>=" + encodeURIComponent($(".sort-word input").val());
			} else {
				var $swrd = "&<%= Constants.REQUEST_KEY_SEARCH_WORD %>=";
			}
			// 在庫
			if ($("input[name='udns']:checked").val() != "") {
				var $udns = "&<%= Constants.REQUEST_KEY_UNDISPLAY_NOSTOCK_PRODUCT %>=" + encodeURIComponent($("input[name='udns']:checked").val());
			} else {
				var $udns = "&<%= Constants.REQUEST_KEY_UNDISPLAY_NOSTOCK_PRODUCT %>=";
			}
			// 定期購入フィルタ
			if ($("input[name=<%= Constants.FORM_NAME_FIXED_PURCHASE_RADIO_BUTTON %>]:checked").val() != "") {
				var $fpfl = "&<%= Constants.REQUEST_KEY_FIXED_PURCHASE_FILTER %>=" + encodeURIComponent($("input[name=<%= Constants.FORM_NAME_FIXED_PURCHASE_RADIO_BUTTON %>]:checked").val());
			} else {
				var $fpfl = "&<%= Constants.REQUEST_KEY_FIXED_PURCHASE_FILTER %>=";
			}
			// 頒布会キーワード
			if ($(".sort-word input").val() != "") {
				var $sbswrd = "&<%= Constants.REQUEST_KEY_SUBSCRIPTION_BOX_SEARCH_WORD %>=" + encodeURIComponent($(".sort-word input").val());
			} else {
				var $sbswrd = "&<%= Constants.REQUEST_KEY_SUBSCRIPTION_BOX_SEARCH_WORD %>=";
			}

			// 詳細検索キーワード
			var advancedSearchVars = [];
			for (urlVarKey in urlVars) {
				// 詳細検索キーワード以外(_で始まらない)の場合スキップ
				if (urlVarKey.slice(0, 1) != "_") continue;
				advancedSearchVars[urlVarKey] = urlVars[urlVarKey];
			}

			// 詳細検索キーをアルファベット順に並び替え
			advancedSearchVars.sort(
				function(a, b) {
					if (a.key < b.key) return -1;
					if (a.key > b.key) return 1;
					return 0;
				});

			var $advancedSearch = "";
			for (varKey in advancedSearchVars) {
				$advancedSearch = $advancedSearch + "&" + varKey + "=" + encodeURIComponent(advancedSearchVars[varKey]);
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
			location.href = rootUrl + $productGroupId + $cicon + $dosp + $dpcnt + $img + $max + $min + $sort + $swrd + $udns + $fpfl+ $sbswrd + $advancedSearch + "&<%= Constants.REQUEST_KEY_PAGE_NO %>=1";
		});
	});
</script>

<div id="divTopArea">
<%-- ▽レイアウト領域：トップエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<section class="wrap-product-list">

<%-- ▽パンくず▽ --%>
<uc:BodyProductCategoryLinks runat="server" />
<%-- △パンくず△ --%>

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

<div class="sort-wrap">
	<nav class="clearfix">
	<ul class="sort-nav" <% if (ProductListDispSettingUtility.ImgSetting.Length <= 1) { %>style="width: 100%"<% } %>>
		<li><a href="javascript:void(0);" id="toggle-advance" class="btn"><i class="fa fa-angle-down"></i> 絞り込み</a></li>
		<% if ((ProductListDispSettingUtility.CountSetting.Length > 1) || (ProductListDispSettingUtility.SortSetting.Length > 1)) { %>
			<li><a href="javascript:void(0);" id="toggle-sort" class="btn"><i class="fa fa-angle-down"></i> 並び替え</a></li>
		<% } %>
	</ul>
		<asp:Repeater ID="rImgList" runat="server" DataSource="<%# ProductListDispSettingUtility.GetImgSetting(DataCacheControllerFacade.GetProductListDispSettingCacheController().CacheData) %>">
			<HeaderTemplate>
				<ul class="layout-nav" <% if (ProductListDispSettingUtility.ImgSetting.Length <= 1) { %>style="display: none"<% } %>>
			</HeaderTemplate>
			<ItemTemplate>
				<li visible="<%# ((ProductListDispSettingModel)Container.DataItem).SettingId == Constants.KBN_REQUEST_DIST_IMG_KBN_WINDOWSHOPPING%>" runat="server">
					<a href="javascript:void(0);" id="btn-layout-list-2" class="<%# this.IsDispImageKbnWindowsShopping? "active" : "" %>">
						<img src="<%= Constants.PATH_ROOT %>SmartPhone/Contents/ImagePkg/common/icon-layout-grid.png" alt="グリッド" width="40" />
					</a>
				</li>
				<li visible="<%# ((ProductListDispSettingModel)Container.DataItem).SettingId == Constants.KBN_REQUEST_DISP_IMG_KBN_ON  %>" runat="server">
					<a href="javascript:void(0);" id="btn-layout-list" class="<%# this.IsDispImageKbnOn  ? "active" : "" %>">
						<img src="<%= Constants.PATH_ROOT %>SmartPhone/Contents/ImagePkg/common/icon-layout-list.png" alt="リスト" width="40" />
					</a>
				</li>
			</ItemTemplate>
			<FooterTemplate>
			</ul>
			</FooterTemplate>
		</asp:Repeater>
	</nav>

	<!--▽ 商品グループページHTML領域 ▽-->
	<uc:BodyProductGroupContentsHtml runat="server" />
	<!--△ 商品グループページHTML領域 △-->

	<div class="sort-toggle">
		<div class="toggle-advance">
			<uc:BodyProductAdvancedSearchBox runat="server" />
		</div>
		<div class="toggle-sort">
			<uc:BodyProductSortBox runat="server" />
		</div>
	</div>
</div>

<%--▽ 商品一覧ループ ▽--%>
<asp:UpdatePanel runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
	<ContentTemplate>
		<% if (this.IsInfiniteLoad) { %>
		<div id="loadingAnimationUpper" style="width:100%" class="loading">
			<img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/loading.gif" style="opacity:0; display: block; width: 30px; height: 30px; margin: 0 auto 20px;"/>
		</div>
		<asp:HiddenField ID="hfPageNumber" runat="server"/>
		<asp:HiddenField ID="hfDisplayPageNumberMax" runat="server"/>
		<% } %>
		<asp:Repeater ID="rTopProductList" runat="server">
			<ItemTemplate>
				<div class="infiniteLoadProducts" runat="server">
					<asp:Repeater ID="rProductsWindowShopping" runat="server" OnItemCommand="InnerRepeater_ItemCommand">
						<HeaderTemplate>
						<ul class="product-list-2 clearfix">
						</HeaderTemplate>
						<ItemTemplate>
							<li id="dInfiniteLoadProduct" class="windowpanel" runat="server">
								<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem, true)) %>'>
								<div class="product-image">
									<w2c:ProductImage ImageSize="M" ProductMaster="<%# Container.DataItem %>" IsVariation="false" runat="server" />
									<%-- ▽在庫切れ可否▽ --%>
									<span visible='<%# ProductListUtility.IsProductSoldOut(Container.DataItem) %>' runat="server" class="sold-out">SOLD OUT</span>
									<%-- △在庫切れ可否△ --%>
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
									<%# WebSanitizer.HtmlEncode(GetProductData(Container.DataItem, "name")) %>
								</div>
								<div class="product-price">
								<%-- ▽商品会員ランク価格有効▽ --%>
								<p visible='<%# GetProductMemberRankPriceValid(Container.DataItem) %>' runat="server" class="special">
									<%#: CurrencyManager.ToPrice(ProductPage.GetProductMemberRankPrice(Container.DataItem)) %>
									（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(Container.DataItem)) %>）
									<span class="line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %>（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(Container.DataItem)) %>）</span>
								</p>
								<%-- △商品会員ランク価格有効△ --%>
								<%-- ▽商品セール価格有効▽ --%>
								<p visible='<%# GetProductTimeSalesValid(Container.DataItem) %>' runat="server" class="special">
									<%#: CurrencyManager.ToPrice(ProductPage.GetProductTimeSalePriceNumeric(Container.DataItem)) %>
									（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(Container.DataItem)) %>）
									<span class="line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %>（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(Container.DataItem)) %>）</span>
								</p>
								<%-- △商品セール価格有効△ --%>
								<%-- ▽商品特別価格有効▽ --%>
								<p visible='<%# GetProductSpecialPriceValid(Container.DataItem) %>' runat="server" class="special">
									<%#: CurrencyManager.ToPrice(ProductPage.GetProductSpecialPriceNumeric(Container.DataItem)) %>
									（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(Container.DataItem)) %>）
									<span class="line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %>（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(Container.DataItem)) %>）</span>
								</p>
								<%-- △商品特別価格有効△ --%>
								<%-- ▽商品通常価格有効▽ --%>
								<p visible='<%# GetProductNormalPriceValid(Container.DataItem) %>' runat="server">
									<%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %>
									（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(Container.DataItem)) %>）
								</p>
								<%-- △商品通常価格有効△ --%>
								<%-- ▽商品加算ポイント▽ --%>
								<p visible='<%# (this.IsLoggedIn && (GetProductAddPointString(Container.DataItem) != "")) %>' runat="server">
									ポイント<%# WebSanitizer.HtmlEncode(GetProductAddPointString(Container.DataItem)) %>
								</p>
								<%-- △商品加算ポイント△ --%>
								<%-- ▽定期購入価格有効▽ --%>
								<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) {%>
								<span visible='<%# (GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG).ToString() != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) && ((CheckFixedPurchaseLimitedUserLevel(this.ShopId, (string)GetProductData(Container.DataItem, "product_id")) == false)) %>' runat="server">
									<p visible='<%# IsProductFixedPurchaseFirsttimePriceValid(Container.DataItem) %>' runat="server">
										定期初回価格:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchaseFirsttimePrice(Container.DataItem)) %>（<%#: GetTaxIncludeString(Container.DataItem) %>）
										<br />
									</p>
									定期通常価格:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchasePrice(Container.DataItem)) %>（<%#: GetTaxIncludeString(Container.DataItem) %>）
								</span>
								<% } %>
								<%-- △定期購入価格有効△ --%>
								<%-- ▽定期商品加算ポイント▽ --%>
									<p visible='<%# (this.IsLoggedIn && (GetProductAddPointString(Container.DataItem) != "") && (GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG).ToString() != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) && ((CheckFixedPurchaseLimitedUserLevel(this.ShopId, (string)GetProductData(Container.DataItem, "product_id")) == false))) %>' runat="server">
										<span class="addPoint">ポイント<%# WebSanitizer.HtmlEncode(GetProductAddPointString(Container.DataItem, false, false, true)) %></span><span id="Span1" visible='<%# ((string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_POINT_KBN2)) == Constants.FLG_PRODUCT_POINT_KBN1_RATE %>' runat="server">
											(<%# WebSanitizer.HtmlEncode(GetProductAddPointCalculateAfterString(Container.DataItem, false, false, true))%>)
										</span>
									</p>
								<%-- △定期商品加算ポイント△ --%>
								<%-- ▽商品頒布会購入価格▽ --%>
								<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED) {%>
									<span visible='<%# (GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG).ToString() != Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_INVALID) && ((CheckFixedPurchaseLimitedUserLevel(this.ShopId, (string)GetProductData(Container.DataItem, "product_id")) == false)) %>' runat="server">
										<br />
										頒布会通常価格:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchasePrice(Container.DataItem)) %>
									</span>
								<% } %>
								<%-- △商品頒布会購入価格△ --%>
									<%-- ▽お気に入りの登録人数表示▽ --%>
								<p>お気に入りの登録人数：<%# this.GetFavoriteCount((string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID)) %>人</p>
								<%-- △お気に入りの登録人数表示△ --%>
								</div>
								<%-- ▽セットプロモーション情報▽ --%>
								<asp:Repeater ID="rSetPromotion" DataSource="<%# GetSetPromotionByProduct((DataRowView)Container.DataItem) %>" runat="server">
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
									<uc:BodyProductVariationImages ImageSize="M" ProductMaster="<%# Container.DataItem %>" VariationList="<%# this.ProductVariationList %>" VariationNo="<%# Container.ItemIndex.ToString() %>" runat="server" />
								</div>
								<% if (Constants.USE_MODAL_PRODUCT_LIST) { %>
									<a class="productlist_detailsLink modal-button" href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem, true)) %>">詳細ページへ</a>
									<a><asp:Button ID="btnShowDetail" runat="server" Text="注文する" CommandArgument="<%# GetProductData(Container.DataItem, Constants.FIELD_PRODUCT_PRODUCT_ID) %>" CommandName="btnOpenModalOrAddCart" CssClass="productlist_orderButton modal-button"/></a>
								<% } %>
								</a>
								</li>
						</ItemTemplate>
						<FooterTemplate>
						</ul>
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

<div visible='<%# this.AlertMessage != "" %>' runat="server" class="msg-alert"><%# WebSanitizer.HtmlEncode(this.AlertMessage) %></div>

<%--△ 商品一覧ループ △--%>

<%-- ▽ページャー▽ --%>
<% if (this.IsInfiniteLoad == false) { %>
<div class="pager-wrap below">
	<%# this.PagerHtml %>
</div>
<% } %>
<%-- △ページャー△ --%>
<uc:BodyRecommendTagsRelatedCategory CategoryName="<%# this.CategoryName %>" runat="server"></uc:BodyRecommendTagsRelatedCategory>
<%-- CRITEOタグ（引数：商品一覧情報） --%>
<uc:Criteo ID="criteo" runat="server" Datas="<%# this.ProductMasterList %>" />
</section>
<%-- △編集可能領域△ --%>
<div id="divBottomArea">
<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<%-- △レイアウト領域△ --%>
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
