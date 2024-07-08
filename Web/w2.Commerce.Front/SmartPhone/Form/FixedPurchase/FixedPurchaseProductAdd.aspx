<%--
=========================================================================================================
  Module      : スマートフォン用商品一覧画面(ProductList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Title="" Language="C#" MasterPageFile="~/SmartPhone/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="~/Form/FixedPurchase/FixedPurchaseProductAdd.aspx.cs" Inherits="Form_FixedPurchase_FixedPurchaseProductAdd" %>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Register TagPrefix="uc" TagName="BodyProductAdvancedSearchBox" Src="~/SmartPhone/Form/Common/Product/BodyProductAdvancedSearchBox.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductCategoryLinks" Src="~/SmartPhone/Form/Common/Product/BodyProductCategoryLinks.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductSortBox" Src="~/SmartPhone/Form/Common/Product/BodyProductSortBox.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductVariationImages" Src="~/SmartPhone/Form/Common/Product/BodyProductVariationImages.ascx" %>
<%@ Register TagPrefix="uc" TagName="Criteo" Src="~/Form/Common/Criteo.ascx" %>

<%@ Import Namespace="ProductListDispSetting" %>
<%@ Import Namespace="w2.App.Common.DataCacheController" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
	<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<link href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "SmartPhone/Css/product.css") %>" rel="stylesheet" type="text/css" media="all" />
<link rel="canonical" href="<%# CreateProductListCanonicalUrl() %>" />
	<%= this.BrandAdditionalDsignTag %>
<% if (Constants.SEOTAG_IN_PRODUCTLIST_ENABLED){ %>
	<meta name="Keywords" content="<%: this.SeoKeywords %>" />
<% } %>
	<%-- △編集可能領域△ --%>
<%# this.PaginationTag %>
	<style type="text/css">
		.breadcrumbs {
			display: none;
		}
		.product-add-button {
			background-color: #000;
			color: #fff !important;
			padding: 0.5em 0;
		}
	</style>
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
			sessionStorage.displayType = "list";
		});

		$("#btn-layout-list-2").click(function () {
			$(".product-list").addClass("product-list-2");
			$(".product-list").removeClass("product-list");
			$(".layout-nav a").removeClass('active');
			$(this).addClass("active");
			$(".wrap-product-list > ul > li").heightLine("refresh");
			sessionStorage.displayType = "grid";
		});

		// レイアウトを引継ぐ
		if ((document.referrer.indexOf("<%= Constants.PAGE_FRONT_FIXED_PURCHASE_PRODUCT_ADD %>") != -1)
			&& (sessionStorage.displayType != undefined)) {
			if (sessionStorage.displayType == "list") {
				$("#btn-layout-list").click();
			}
			else {
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
				var rootUrl = "<%= Constants.PATH_ROOT %><%= Constants.PAGE_FRONT_FIXED_PURCHASE_PRODUCT_ADD %>?<%= Constants.REQUEST_KEY_SHOP_ID %>=" + $shop
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
<div class="pager-wrap above">
	<%# this.PagerHtml %>
</div>
<%-- △ページャー△ --%>

<div class="sort-wrap">
	<nav class="clearfix">
	<ul class="sort-nav" style="width: 100%;">
		<li style="width: 50%;">
			<a href="javascript:void(0);" id="toggle-advance" class="btn"><i class="fa fa-angle-down"></i> 絞り込み</a>
		</li>
		<% if ((ProductListDispSettingUtility.CountSetting.Length > 1) || (ProductListDispSettingUtility.SortSetting.Length > 1)) { %>
			<li style="width: 50%;">
				<a href="javascript:void(0);" id="toggle-sort" class="btn"><i class="fa fa-angle-down"></i> 並び替え</a>
			</li>
		<% } %>
	</ul>
	</nav>

	<div class="sort-toggle">
		<div class="toggle-advance">
			<uc:BodyProductAdvancedSearchBox IsFixedPurchaseProductAdd="True" TargetUrl="<%# Constants.PAGE_FRONT_FIXED_PURCHASE_PRODUCT_ADD %>" runat="server" />
		</div>
		<div class="toggle-sort">
			<uc:BodyProductSortBox IsSortOnly="True" TargetUrl="<%# Constants.PAGE_FRONT_FIXED_PURCHASE_PRODUCT_ADD %>" runat="server" />
		</div>
	</div>
</div>

<%-- ▽商品一覧ループ(通常表示)▽ --%>
<asp:Repeater ID="rProductsListView" DataSource="<%# this.ProductMasterList %>" runat="server">
	<HeaderTemplate>
		<ul class="product-list-2 clearfix">
	</HeaderTemplate>
		<ItemTemplate>
		<li>
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
			<%-- エラー文言の表示 --%>
			<p visible='<%# GetVariationErrorMessage(Container.ItemIndex).ToString() != "" %>' style="color: red;" runat="server" >
				<%# GetVariationErrorMessage(Container.ItemIndex).ToString() %><br />
			</p>
		</div>
		<div class="product-price">
		<%-- ▽商品通常価格有効▽ --%>
		<p visible='<%# GetProductNormalPriceValid(Container.DataItem) %>' runat="server">
			<%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %>
			（<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(Container.DataItem)) %>）
		</p>
		<%-- △商品通常価格有効△ --%>
		<%-- ▽定期購入価格有効▽ --%>
		<span visible='<%# (GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG).ToString() != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) && ((CheckFixedPurchaseLimitedUserLevel(this.ShopId, (string)GetProductData(Container.DataItem, "product_id")) == false)) %>' runat="server">
				定期通常価格:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchasePrice(Container.DataItem)) %>（<%#: GetTaxIncludeString(Container.DataItem) %>）
		</span>
			<%-- △定期購入価格有効△ --%>
		</div>
		<div class="product-image-variation">
			<uc:BodyProductVariationImages ImageSize="M" ProductMaster="<%# Container.DataItem %>" VariationList="<%# this.ProductVariationList %>" VariationNo="<%# Container.ItemIndex.ToString() %>" runat="server" />
		</div>
			<div style="width: 90%; margin: 0 auto;">
				<br />
				<asp:DropDownList ID="ddlVariationSelect" DataSource='<%# SetVariationSelectForDropDownList((string)GetProductData(Container.DataItem, Constants.FIELD_PRODUCT_PRODUCT_ID)) %>' DataTextField="Text" DataValueField="Value" Visible="<%# HasVariation(Container.DataItem) %>" AutoPostBack="True" runat="server" />
				<br />
				<asp:HiddenField runat="server" ID="hfProductId" Value="<%# GetProductData(Container.DataItem, Constants.FIELD_PRODUCT_PRODUCT_ID) %>"/>
				<div style="  position: absolute; bottom: 0; margin: 0;">
					<asp:LinkButton runat="server" CssClass="btn product-add-button" Visible="<%# DisplayTheAddProductButton(Container.ItemIndex) %>" Text="商品を追加する" ID="lbAddProduct" OnClick="lbAddProduct_OnClick" CommandArgument="<%# ((RepeaterItem)Container).ItemIndex %>" />
					<asp:LinkButton runat="server" CssClass="btn product-add-button" Visible="<%# DisplayTheChangeProductButton(Container.ItemIndex) %>" Text="商品を変更する" ID="lbChangeProduct" OnClick="lbChangeProduct_OnClick" CommandArgument="<%# ((RepeaterItem)Container).ItemIndex %>" />
				</div>
			</div>
			</li>
		</ItemTemplate>
	<FooterTemplate>
		</ul>
	</FooterTemplate>
</asp:Repeater>
<%-- △商品一覧ループ(通常表示)△ --%>

<div visible='<%# this.AlertMessage != "" %>' runat="server" class="msg-alert"><%# WebSanitizer.HtmlEncode(this.AlertMessage) %></div>
	
	<div style="padding: 10px 0px; margin: 10px;">
		<asp:LinkButton runat="server" CssClass="btn product-add-button" Text="戻る" ID="lbBack" OnClick="lbBack_OnClick"/>
	</div>

<%-- ▽ページャー▽ --%>
<div class="pager-wrap below">
	<%# this.PagerHtml %>
</div>
<%-- △ページャー△ --%>

<%-- CRITEOタグ（引数：商品一覧情報） --%>
<uc:Criteo ID="criteo" runat="server" Datas="<%# this.ProductMasterList %>" />
</section>
<%-- △編集可能領域△ --%>
<div id="divBottomArea">
<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>

</asp:Content>
