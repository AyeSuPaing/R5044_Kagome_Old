<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%@ Register TagPrefix="uc" TagName="BodyProductAdvancedSearchBox" Src="~/Form/Common/Product/BodyProductAdvancedSearchBox.ascx" %>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Register TagPrefix="uc" TagName="BodyProductSortBox" Src="~/Form/Common/Product/BodyProductSortBox.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductVariationImages" Src="~/Form/Common/Product/BodyProductVariationImages.ascx" %>
<%@ Register TagPrefix="uc" TagName="HeaderScriptDeclaration" Src="~/Form/Common/HeaderScriptDeclaration.ascx" %>
<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeFile="FixedPurchaseProductAdd.aspx.cs" Inherits="Form_FixedPurchase_FixedPurchaseProductAdd" %>
<%@ Import Namespace="ProductListDispSetting" %>

<html lang="ja">
<head runat="server">
	<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
	<title>定期追加商品選択画面</title>
	<meta http-equiv="Content-Script-Type" content="text/javascript" />
	<%
		SetCssLink("lCommonCss", Constants.PATH_ROOT + "Css/common.css?20180216");
		SetCssLink("lPrintCss", Constants.PATH_ROOT + "Css/imports/print.css");
		SetCssLink("lTooltipCss", Constants.PATH_ROOT + "Css/tooltip.css");
	%>
	<link id="lCommonCss" rel="stylesheet" type="text/css" media="screen,print" />
	<link id="lPrintCss" rel="stylesheet" type="text/css"  media="print" />
	<link id="lTooltipCss" rel="stylesheet" type="text/css"  media="all" />
	<%-- 各種Js読み込み --%>
	<uc:HeaderScriptDeclaration id="HeaderScriptDeclaration" runat="server"></uc:HeaderScriptDeclaration>
	<script type="text/javascript">
		$(function () {

			// 詳細検索
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
					$bid = $("input[name='iBrand']:checked").val();
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
			var rootUrl = "<%= Constants.PATH_ROOT %><%= Constants.PAGE_FRONT_FIXED_PURCHASE_PRODUCT_ADD %>?<%= Constants.REQUEST_KEY_SHOP_ID %>=" + $shop
				+ "&<%= Constants.REQUEST_KEY_CATEGORY_ID %>=" + $cat + (($bid != undefined) ? "&<%= Constants.REQUEST_KEY_BRAND_ID %>=" + $bid : "");
			location.href = rootUrl + $productGroupId + $cicon + $dosp + $dpcnt + $img + $max + $min + $sort + $swrd + $udns + $fpfl + $sbswrd + "&<%= Constants.REQUEST_KEY_PAGE_NO %>=1";
		});

	});

		function enterSearch() {
			//EnterキーならSubmit
			if (window.event.keyCode == 13) document.formname.submit();
		}
	</script>


	<script type="text/javascript">
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
	</script>
	
	<style>
		.heightLineParent {
			display: flex;
			flex-wrap: wrap;
		}
		.fixed-purchase-product-list {
			float: left;
			border: 1px solid #d0d0d0;
			margin: -1px 0 0 -1px;
			position: relative;
			background-color: #fff;
		}
		.fixed-purchase-product-list {
			width: 155px;
		}
		.fixed-purchase-product-list ul {
			padding: 10px;
		}
		.fixed-purchase-product-list ul li {
			margin-bottom: 5px;
		}
		.fixed-purchase-product-list li.icon img {
			height: 20px;
			border: none;
		}
		.fixed-purchase-product-list li.thumb {
			text-align: center;
		}
		.fixed-purchase-product-list li.thumb img {
			width: 133px;
		}
		.fixed-purchase-product-list.column5 .variationview_wrap {
			top: -1px !important;
			left: 153px !important;
		}
		.fixed-purchase-product-list ul li.name {
			font-size: 93%;
			line-height: 1.8;
		}
		.fixed-purchase-product-list ul li.price {
			font-size: 93%;
			line-height: 1.8;
			padding-top: 5px;
			border-top: 1px dotted #d0d0d0;
		}
		.fixed-purchase-product-list ul li.price p {
			line-height: 1.5;
		}
		.back-btn {
			width: 100px;
			margin-left: 50%;
		}
		.listProduct ul li.productList{
			margin-bottom: 20px;
			padding-bottom: 20px;
			border-bottom: 1px dotted #333;
		}
		.listProduct ul li.productList:last-child { margin-bottom: 10px; }
		.listProduct ul li.productList ul{}

		.listProduct ul li.productList ul li.plPhoto{width:220px;text-align:center;float:left;position: relative;}
		.listProduct ul li.productList ul li.plPhoto img { width: 100%; }

		.listProduct ul li.productList ul li.plPhoto .soldout {
			color: #fff;
			word-wrap: normal;
			position: absolute;
			top: 45%;
			left: 0%;
			text-align: center;
			width: 100%;
			height: 26px;
			line-height: 26px;
			background-color: #000;
			filter:alpha(opacity=50);
			-moz-opacity: 0.5;
			opacity: 0.5;
		}

		.listProduct ul li.productList ul li.plProductInfo{width:540px;float:right;}
		.listProduct ul li.productList ul li.plProductInfo ul{padding:0px 5px;}
		.listProduct ul li.productList ul li.plProductInfo ul li{
			margin:5px 0;
			line-height: 1.5;
		}
		.listProduct ul li.productList ul li.plProductInfo li.plName h3 {
			margin: 3px 0;
			padding: 0;
		}
		.listProduct ul li.productList ul li.plProductInfo li.plName img{height:25px;vertical-align:bottom;}
		.listProduct ul li.productList ul li.plProductInfo li.plExcerpt{}
		.listProduct ul li.productList ul li.plProductInfo li.plIcon{}
		.listProduct ul li.productList ul li.plProductInfo li.plPrice{ margin-top: 15px; }
		.listProduct ul li.productList ul li.plProductInfo li.plPrice strike { color: #f00; }
		.listProduct ul li.productList ul li.plProductInfo li.plPrice .favoriteRegistration { margin: 5px 0; }
	</style>
</head>

<body>
<form id="form1" runat="server">
<%-- スクリプトマネージャ --%>
<asp:ScriptManager ID="smScriptManager" runat="server" ScriptMode="Release" />
<div class="main-container">
	<table id="tblLayout" class="tblLayout_ProductList">
<tr>
<td>
<div id="divTopArea">
</div>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<div id="primary" style="margin: auto;">

<uc:BodyProductAdvancedSearchBox IsFixedPurchaseProductAdd="True" TargetUrl="<%# Constants.PAGE_FRONT_FIXED_PURCHASE_PRODUCT_ADD %>" runat="server" />

<!--▽ ソートコントロール ▽-->
<uc:BodyProductSortBox CategoryName="<%# this.CategoryName %>" IsSortOnly="True" TargetUrl="<%# Constants.PAGE_FRONT_FIXED_PURCHASE_PRODUCT_ADD %>" runat="server"></uc:BodyProductSortBox>
<!--△ ソートコントロール △-->

<!--▽ ページャ ▽-->
<div id="pagination" class="above clearFix">
<%# this.PagerHtml %>
</div>
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
	<%-- ▽商品一覧ループ(通常表示)▽ --%>
	<asp:Repeater ID ="rProductsListView" DataSource="<%# this.ProductMasterList %>" runat="server" Visible="<%# this.IsDispImageKbnOn %>" OnItemCommand="ProductMasterList_ItemCommand">
	<HeaderTemplate>
	<ul>
	</HeaderTemplate>
	<ItemTemplate>
		<li class="productList">
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
									<%# WebSanitizer.HtmlEncode(GetProductData(Container.DataItem, "name")) %>
								<!-- TODO:表示テスト -->
								<!-- 商品ID表示 -->
								[<span class="productId"><%# WebSanitizer.HtmlEncode(GetProductData(Container.DataItem, "product_id")) %></span>]
								</li>
								<li class="plExcerpt">
									<!-- 概要表示 -->
									<%# GetProductDataHtml(Container.DataItem, "outline") %>
								</li>
								
								<li>
									<%-- エラー文言の表示 --%>
									<p class="error" visible='<%# GetVariationErrorMessage(Container.ItemIndex).ToString() != "" %>' runat="server">
										<%# GetVariationErrorMessage(Container.ItemIndex).ToString() %><br />
									</p>
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
								<%-- ▽商品定期購入価格▽ --%>
								<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) {%>
								<span visible='<%# (GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG).ToString() != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) && ((CheckFixedPurchaseLimitedUserLevel(this.ShopId, (string)GetProductData(Container.DataItem, "product_id")) == false)) %>' runat="server">
									<span visible='<%# IsProductFixedPurchaseFirsttimePriceValid(Container.DataItem) %>' runat="server">
									</span>
									<br />
									定期通常価格:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchasePrice(Container.DataItem)) %>（<%#: GetTaxIncludeString(Container.DataItem) %>）
								</span>
								<% } %>
								<%-- △商品定期購入価格△ --%>

									<%-- ▽商品タグ項目：メーカー▽ --%>
								<span visible='<%# StringUtility.ToEmpty(GetKeyValue(Container.DataItem, "tag_manufacturer")) != "" %>' runat="server">
								メーカー:<%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, "tag_manufacturer")) %>
								</span>
								<%-- △商品タグ項目：メーカー△ --%>
								</li>
								<asp:DropDownList ID="ddlVariationSelect" DataSource='<%# SetVariationSelectForDropDownList((string)GetProductData(Container.DataItem, Constants.FIELD_PRODUCT_PRODUCT_ID)) %>' DataTextField="Text" DataValueField="Value" Visible="<%# HasVariation(Container.DataItem) %>" AutoPostBack="True" runat="server" />
								<asp:HiddenField runat="server" ID="hfProductId" Value="<%# GetProductData(Container.DataItem, Constants.FIELD_PRODUCT_PRODUCT_ID) %>"/>
								<asp:LinkButton runat="server" Visible="<%# DisplayTheAddProductButton(Container.ItemIndex) %>" CssClass="btn" Text="商品を追加する" ID="lbAddProduct" OnClick="lbAddProduct_OnClick" CommandArgument="<%# ((RepeaterItem)Container).ItemIndex %>" />
								<asp:LinkButton runat="server" CssClass="btn" Visible="<%# DisplayTheChangeProductButton(Container.ItemIndex) %>" Text="商品を変更する" ID="lbChangeProduct" OnClick="lbChangeProduct_OnClick" CommandArgument="<%# ((RepeaterItem)Container).ItemIndex %>" />
							</ul>
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
	</ContentTemplate>
	</asp:UpdatePanel>
<%-- UPDATE PANELここまで --%>
	<%-- △商品一覧ループ(通常表示)△ --%>
	</div>

<%-- ▽商品一覧ループ(ウインドウショッピング)▽ --%>
<asp:Repeater ID="rProductList" DataSource="<%# this.ProductMasterList %>" runat="server" Visible="<%# this.IsDispImageKbnWindowsShopping %>">
<HeaderTemplate>
<div class="heightLineParent clearFix">
</HeaderTemplate>
<ItemTemplate>

<div class="fixed-purchase-product-list column5">

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
<li>
	<%-- エラー文言の表示 --%>
	<p class="error" visible='<%# GetVariationErrorMessage(Container.ItemIndex).ToString() != "" %>' runat="server">
		<%# GetVariationErrorMessage(Container.ItemIndex).ToString() %><br />
	</p>
</li>
<li class="price">

<p>
	<%# CurrencyManager.ToPrice(ProductPage.GetKeyValueToNull(
		Container.DataItem,
		Constants.SETTING_PRODUCT_LIST_SEARCH_KBN
			? Constants.FIELD_PRODUCTVARIATION_PRICE
			: Constants.FIELD_PRODUCT_DISPLAY_PRICE)) %>
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
	<p>
		定期通常:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchasePrice(Container.DataItem, Constants.SETTING_PRODUCT_LIST_SEARCH_KBN)) %>（<%#: GetTaxIncludeString(Container.DataItem) %>）
	</p>
</p>
<% } %>
</li>
	<asp:DropDownList ID="ddlVariationSelect" DataSource='<%# SetVariationSelectForDropDownList((string)GetProductData(Container.DataItem, Constants.FIELD_PRODUCT_PRODUCT_ID)) %>' DataTextField="Text" DataValueField="Value" Visible="<%# HasVariation(Container.DataItem) %>" AutoPostBack="True" runat="server" />
	<asp:HiddenField runat="server" ID="hfProductId" Value="<%# GetProductData(Container.DataItem, Constants.FIELD_PRODUCT_PRODUCT_ID) %>"/>
	<asp:LinkButton runat="server" CssClass="btn" Visible="<%# DisplayTheAddProductButton(Container.ItemIndex) %>" Text="商品を追加する" ID="lbAddProduct" OnClick="lbAddProduct_OnClick" CommandArgument="<%# ((RepeaterItem)Container).ItemIndex %>" />
	<asp:LinkButton runat="server" CssClass="btn" Visible="<%# DisplayTheChangeProductButton(Container.ItemIndex) %>" Text="商品を変更する" ID="lbChangeProduct" OnClick="lbChangeProduct_OnClick" CommandArgument="<%# ((RepeaterItem)Container).ItemIndex %>" />
</ul>
</div>
</ItemTemplate>
<FooterTemplate>
</div>
</FooterTemplate>
</asp:Repeater>
<%-- △商品一覧ループ(ウインドウショッピング)△ --%>
		
<!--▽ ページャ ▽-- >
<div id="pagination" class="below clearFix">
<%# this.PagerHtml %>
</div>
<!--△ ページャ △-->

<div visible="<%# (this.ProductMasterList.Count == 0) %>" runat="server" class="noProduct">
<!--▽ 商品が1つもなかった場合のエラー文言 ▽-->
<%# WebSanitizer.HtmlEncode(this.AlertMessage) %>
<!--△ 商品が1つもなかった場合のエラー文言 △-->
</div><!-- (this.ProductMasterList.Count != 0) -->

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
</div>
<div id="footer" style="width: auto;">
	<div style="padding: 15px 0px;">
		<asp:LinkButton runat="server" CssClass="btn back-btn" ID="lbBack" OnClick="lbBack_OnClick">  戻る  </asp:LinkButton>
	</div>

</div>
</form>
</body>

</html>
