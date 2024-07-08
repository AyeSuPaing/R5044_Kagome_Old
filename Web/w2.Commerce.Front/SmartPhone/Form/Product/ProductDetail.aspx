<%--
=========================================================================================================
  Module      : スマートフォン用商品詳細画面(ProductDetail.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%@ Register TagPrefix="uc" TagName="BodyProductRecommendByRecommendEngine" Src="~/SmartPhone/Form/Common/Product/BodyProductRecommendByRecommendEngine.ascx" %>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Register TagPrefix="uc" TagName="BodyProductCategoryLinks" Src="~/SmartPhone/Form/Common/Product/BodyProductCategoryLinks.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductArrivalMailRegister" Src="~/SmartPhone/Form/Common/Product/BodyProductArrivalMailRegister.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductReview" Src="~/SmartPhone/Form/Common/Product/BodyProductReview.ascx" %>
<%@ Register TagPrefix="uc" TagName="Criteo" Src="~/Form/Common/Criteo.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductCoordinate" Src="~/SmartPhone/Form/Common/Product/BodyProductCoordinate.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyRecommendProductsWithTag" Src="~/SmartPhone/Form/Common/Product/BodyRecommendProductsWithTag.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Form/Product/ProductDetail.aspx.cs" Inherits="Form_Product_ProductDetail" Title="商品詳細ページ" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<link href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "SmartPhone/Css/product.css") %>" rel="stylesheet" type="text/css" media="all" />
<link rel="canonical" href="<%# CreateProductDetailCanonicalUrl() %>" />
<script type="text/javascript" charset="UTF-8" src="<%= Constants.PATH_ROOT %>SmartPhone/Js/imagesloaded.pkgd.min.js"></script>
<script type="text/javascript" charset="UTF-8" src="<%= Constants.PATH_ROOT %>SmartPhone/Js/flipsnap.js"></script>

<% if ((Constants.MOBILEOPTION_ENABLED)
	&& (this.ProductMaster != null)
	&& ((string)this.ProductMaster[Constants.FIELD_PRODUCT_MOBILE_DISP_FLG] == Constants.FLG_PRODUCT_MOBILE_DISP_FLG_ALL)){%>
	<link rel="Alternate" media="handheld" href="<%= WebSanitizer.HtmlEncode(GetMobileUrl()) %>" />
<% } %>
<%= this.BrandAdditionalDsignTag %>
<% if (Constants.SEOTAG_AND_OGPTAG_IN_PRODUCTDETAIL_ENABLED){ %>
	<meta name="Keywords" content="<%: this.SeoKeywords %>" />
<% } %>
<script type="text/javascript" language="javascript">
	// 商品詳細：画像スライド
	//$(window).load(function(){
	$(function () {
		detailImageFlick();
	});

	var clickName;

	function detailImageFlick() {
		var len = $('.flipsnap .item').length,

			$pointer = $('.product-image-sub a'),
			$variation = $('.drop-down'),
			$targetDropDown,
			$targetPoint,
			$next = $('.product-image-detail .next').click(function () { flipsnap.toNext(); }),
			$prev = $('.product-image-detail .prev').click(function () { flipsnap.toPrev(); }),
			lenGap = len - $pointer.length;

		$('.flick-wrap').removeClass('loading');
		$('.flipsnap').css('visibility', 'visible');

		$('.flipsnap').css({ width: len * 304 });
		var flipsnap = Flipsnap('.flipsnap', {
			distance: 304
		});

		// ページング
		$pointer.each(function (i) {
			if ((i === 0) && (lenGap === 0)) { $(this).attr('class', 'current'); }
			$(this).attr('name', i);
			
		});

		flipsnap.element.addEventListener('fspointmove', function () {
			$pointer.filter('.current').removeClass('current');
			$pointer.eq(flipsnap.currentPoint - lenGap).addClass('current');
			var variationId = $pointer.eq(flipsnap.currentPoint - lenGap).data('variationid');
			$variation.find('option[value=' + variationId + ']').prop("selected", true);
			// メイン画像にスライドした時、サブ画像リストが選択された表記をしないようにする
			if ((lenGap !== 0) && (flipsnap.currentPoint === 0))
			{
				$pointer.filter('.current').removeClass('current');
			}
		}, false);
		$pointer.click(
			function () {
				clickName = Number(this.name);
				flipsnap.moveToPoint(clickName + lenGap);
				$pointer.filter('.current').removeClass('current');
				$('.product-image-sub a[name=' + (clickName) + ']').addClass('current');
			}
		);
		// 前へ 次へ
		$('.product-image-detail .prev').attr('disabled', 'disabled');
		flipsnap.element.addEventListener('fspointmove', function () {
			$next.attr('disabled', !flipsnap.hasNext());
			$prev.attr('disabled', !flipsnap.hasPrev());
		}, false);

		<% if ((this.SelectVariationKbn == Constants.SelectVariationKbn.PANEL)
				|| (this.IsVariationName3 && ((this.SelectVariationKbn == Constants.SelectVariationKbn.DOUBLEDROPDOWNLIST)
					|| (this.SelectVariationKbn == Constants.SelectVariationKbn.MATRIX)
					|| (this.SelectVariationKbn == Constants.SelectVariationKbn.MATRIXANDMESSAGE)))) {%>
				flipsnap.moveToPoint(clickName, 0);
				$pointer.filter('.current').removeClass('current');
				$('.product-image-sub a[name=' + (clickName) + ']').addClass('current');
		<%} else {%>
				if (0 < $variation.size()) {
					flipsnap.moveToPoint(clickName, 0);
					$pointer.filter('.current').removeClass('current');
					$('.product-image-sub a[name=' + (clickName) + ']').addClass('current');
				}
		<%} %>
	}
</script>
<%-- △編集可能領域△ --%>

<style type="text/css">
	.VariationPanel
	{
		min-width:50px;
		border:1px;
		border-style:solid;
		border-color: #adb0b0;
		background-color: #f5f7f7;
		padding:2px 2px 2px 2px;
		margin-right: 15px;
		margin-bottom: 10px;
		float:left;
		margin:5px 5px 5px 5px;
	}
	.VariationPanelSelected
	{
		min-width:50px;
		border:1px;
		border-style:solid;
		padding:2px 2px 2px 2px;
		margin-right: 15px;
		margin-bottom: 10px;
		background-color: #dbdfdf;
		float:left;
		margin:5px 5px 5px 5px;
	}
	[id$="ddlSubscriptionBox"] {
		width: 100%;
		height: 33px;
		margin-top: 10px;
		margin-bottom: 10px;
	}
</style>
</asp:Content>

<%--
	通常価格・特別価格表示については
	・バリエーションなし			→ 商品バリエーションマスタ参照
	・バリエーションあり・未選択	→ 商品マスタ参照
	・バリエーションあり・選択中	→ 商品バリエーションマスタ参照
	となる。
	
	一方、商品セール価格表示部分は
	・バリエーションなし			→ 商品バリエーションマスタ参照
	・バリエーションあり・未選択	→ 商品バリエーションマスタ参照
	・バリエーションあり・選択中	→ 商品バリエーションマスタ参照
	としたいため、結局は取得出来ているバリエーションを参照する
--%>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<%-- 表示更新の為のpageLoad --%>
<script type="text/javascript" language="javascript">
	function bodyPageLoad() {
		if (Sys.WebForms == null) return;
		var isAsyncPostback = Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();
		if (isAsyncPostback) {
			twttr.widgets.load(); //ツイートボタン
			detailImageFlick(); //画像スライド
		}
	}
</script>

<%-- UPDATE PANEL開始 --%>
<asp:UpdatePanel ID="upUpdatePanel" runat="server">
<ContentTemplate>
<div id="divTopArea">
<%-- ▽レイアウト領域：トップエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<%-- カート投入ボタン押下時にどの画面へ遷移するか？ --%>
<%-- CART：カート一覧画面 その他：画面遷移しない --%>
<asp:HiddenField ID="hfIsRedirectAfterAddProduct" Value="CART" runat="server" />

<asp:HiddenField ID="hfVariationSelectedIndex" Value="0" runat="server" />

<%-- お気に入り追加ボタン押下時にどの画面へ遷移するか？ --%>
<%-- true:ポップアップ表示、false:お気に入り一覧ページへ遷移 --%>
<% IsDisplayPopupAddFavorite = false; %>

<%-- ▽パンくず▽ --%>
<uc:BodyProductCategoryLinks runat="server" />
<%-- △パンくず△ --%>

<section class="wrap-product-detail">

<%-- class:"ChangesByVariation"→バリエーション変更時の表示更新領域を指定しています --%>
<!-- 商品詳細TOP -->
<article class="ChangesByVariation" runat="server">

	<header>
		<%-- 商品名 --%>
		<h1><%# WebSanitizer.HtmlEncode(GetProductData("name")) %></h1>
		<%-- キャッチコピー --%>
		<strong visible='<%# WebSanitizer.HtmlEncode(GetProductData("catchcopy")) != "" %>' runat="server"><%# WebSanitizer.HtmlEncode(GetProductData("catchcopy")) %></strong>
	</header>

	<ul class="product-info">
		<li>
			<div class="product-price">
			<!-- 商品価格・税区分・加算ポイント -->
			<%-- ▽商品会員ランク価格有効▽ --%>
			<p visible='<%# GetProductMemberRankPriceValid(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected)) %>' runat="server" class="special">
				<%#: CurrencyManager.ToPrice(ProductPage.GetProductMemberRankPrice(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected))) %>
					(<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(this.ProductMaster)) %>)
				<span class="line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected))) %>
				(<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(this.ProductMaster)) %>)</span>
			</p>
			<%-- △商品会員ランク価格有効△ --%>

			<%-- ▽商品セール価格有効▽ --%>
			<p visible='<%# GetProductTimeSalesValid(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected)) %>' runat="server" class="special">
				<%#: CurrencyManager.ToPrice(ProductPage.GetProductTimeSalePriceNumeric(this.ProductMaster)) %>
				(<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(this.ProductMaster)) %>)
				<span class="line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected))) %>
				(<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(this.ProductMaster)) %>)</span>
			</p>
			<%-- △商品セール価格有効△ --%>

			<%-- ▽商品特別価格有効▽ --%>
			<p visible='<%# GetProductSpecialPriceValid(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected)) %>' runat="server" class="special">
				<%#: CurrencyManager.ToPrice(ProductPage.GetProductSpecialPriceNumeric(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected))) %>
				(<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(this.ProductMaster)) %>)
				<span class="line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected))) %>
				(<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(this.ProductMaster)) %>)</span>
			</p>
			<%-- △商品特別価格有効△ --%>

			<%-- ▽商品通常価格有効▽ --%>
			<p visible='<%# GetProductNormalPriceValid(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected)) %>' runat="server">
				<%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected))) %>
				(<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(this.ProductMaster)) %>)
			</p>
			<%-- △商品通常価格有効△ --%>
			<%-- ▽商品定期購入価格▽ --%>
			<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) {%>
			<div id="Div1" visible='<%# (GetKeyValue(this.ProductMaster, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG).ToString() != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) && (this.IsUserFixedPurchaseAble) %>' runat="server">
				<span id="Span1" visible='<%# IsProductFixedPurchaseFirsttimePriceValid(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected)) %>' runat="server">
					<p class="productPrice">定期初回価格:<span><%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchaseFirsttimePrice(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected))) %></span>(<%#: GetTaxIncludeString(this.ProductMaster) %>)</p>
				</span>
				<p class="productPrice">定期通常価格:<span><%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchasePrice(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected))) %></span>(<%#: GetTaxIncludeString(this.ProductMaster) %>）</p>
			</div>
			<% } %>
			<%-- △商品定期購入価格△ --%>
			</div>
		</li>
		<li>
			<%-- ▽セットプロモーション情報▽ --%>
			<div class="set-promotion">
			<asp:Repeater ID="rSetPromotion" DataSource="<%# this.SetPromotions %>" runat="server">
				<ItemTemplate>
				<p>
					<%# GetProductDescHtml(((SetPromotionModel)Container.DataItem).DescriptionKbn, ((SetPromotionModel)Container.DataItem).Description)%>
				</p>
				</ItemTemplate>
			</asp:Repeater>
			</div>
			<%-- △セットプロモーション情報△ --%>
		</li>
		<li visible='<%# (this.IsLoggedIn && (GetProductAddPointString(this.ProductMaster, this.HasVariation, this.VariationSelected) != "")) %>' runat="server">
			<%-- ▽商品加算ポイント▽ --%>
			<span class="point">ポイント<%# WebSanitizer.HtmlEncode(GetProductAddPointString(this.ProductMaster, this.HasVariation, this.VariationSelected)) %>還元</span>
			<%-- △商品加算ポイント△ --%>
		</li>
		<li visible='<%# (this.IsLoggedIn && (this.CanFixedPurchase) && (this.IsUserFixedPurchaseAble) && (GetProductAddPointString(this.ProductMaster, this.HasVariation, this.VariationSelected) != "")) %>' runat="server">
			<%-- ▽定期商品加算ポイント▽ --%>
			<span class="point">ポイント（定期購入）<%# WebSanitizer.HtmlEncode(GetProductAddPointString(this.ProductMaster, this.HasVariation, this.VariationSelected, true)) %>還元</span>
			<%-- △定期商品加算ポイント△ --%>
		</li>

	</ul>

	<%-- 商品アイコン --%>
	<div class="product-icon clearfix">
		<w2c:ProductIcon ID="ProductIcon1" IconNo="1" ProductMaster="<%# this.ProductMaster %>" runat="server" />
		<w2c:ProductIcon ID="ProductIcon2" IconNo="2" ProductMaster="<%# this.ProductMaster %>" runat="server" />
		<w2c:ProductIcon ID="ProductIcon3" IconNo="3" ProductMaster="<%# this.ProductMaster %>" runat="server" />
		<w2c:ProductIcon ID="ProductIcon4" IconNo="4" ProductMaster="<%# this.ProductMaster %>" runat="server" />
		<w2c:ProductIcon ID="ProductIcon5" IconNo="5" ProductMaster="<%# this.ProductMaster %>" runat="server" />
		<w2c:ProductIcon ID="ProductIcon6" IconNo="6" ProductMaster="<%# this.ProductMaster %>" runat="server" />
		<w2c:ProductIcon ID="ProductIcon7" IconNo="7" ProductMaster="<%# this.ProductMaster %>" runat="server" />
		<w2c:ProductIcon ID="ProductIcon8" IconNo="8" ProductMaster="<%# this.ProductMaster %>" runat="server" />
		<w2c:ProductIcon ID="ProductIcon9" IconNo="9" ProductMaster="<%# this.ProductMaster %>" runat="server" />
		<w2c:ProductIcon ID="ProductIcon10" IconNo="10" ProductMaster="<%# this.ProductMaster %>" runat="server" />
	</div>

	<div class="wrap-product-image">

		<%-- ▽メイン画像▽ --%>
		<div class="product-image-detail">
			<div class="flick-wrap loading">
				<div class="flipsnap">
					<%-- ▽メイン画像▽ --%>
					<div class="item">
						<img class="zoomTarget" src="<%# WebSanitizer.HtmlEncode(CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)(Constants.PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO + 1))) %>" data-image="<%# WebSanitizer.HtmlEncode(CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)(Constants.PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO + 1))) %>" data-zoom-image="<%# WebSanitizer.HtmlEncode(CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)(Constants.PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO + 1))) %>" />					</div>
					<%-- △メイン画像△ --%>
					<%-- ▽バリエーション画像一覧▽ --%>
					<asp:Repeater DataSource="<%# FilteringImages(this.ProductVariationMasterList, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3) %>" Visible="<%# this.HasVariation %>" runat="server">
					<ItemTemplate>
						<div class="item">
							<w2c:ProductImage ImageSize="LL" ProductMaster="<%# Container.DataItem %>" IsVariation="true" runat="server" />
							<span class="var-name"><%# Eval(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1) %> <%# Eval(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3) %></span>
						</div>
					</ItemTemplate>
					</asp:Repeater>
					<%-- △バリエーション画像一覧△ --%>
					<%-- ▽サブ画像一覧▽ --%>
					<asp:Repeater DataSource="<%# this.ProductSubImageList %>" Visible="<%# (this.ProductSubImageList.Count != 0) %>" runat="server">
					<ItemTemplate>
						<div class="item" visible='<%# IsSubImagesNoLimit((int)Eval(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO)) %>' runat="server">
							<img src="<%# WebSanitizer.HtmlEncode(CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)Eval(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO))) %>" alt="" />
						</div>
					</ItemTemplate>
					</asp:Repeater>
					<%-- △サブ画像一覧△ --%>
				</div>
			</div>

			<div class="product-image-sub">
				<%--メイン画像一覧--%>
				<a href="javascript:void(0);">
					<img class="zoomTarget" src="<%# WebSanitizer.HtmlEncode(CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)(Constants.PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO + 1))) %>" data-image="<%# WebSanitizer.HtmlEncode(CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)(Constants.PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO + 1))) %>" data-zoom-image="<%# WebSanitizer.HtmlEncode(CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)(Constants.PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO + 1))) %>" />
				</a>
				<%--バリエーション画像一覧--%>
				<asp:Repeater ID="rVariationImageList" DataSource='<%# FilteringImages(this.ProductVariationMasterList, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3) %>' Visible="<%# this.HasVariation %>" runat="server">
					<HeaderTemplate></HeaderTemplate>
					<ItemTemplate>
						<asp:LinkButton ID="lbVariationPicture" runat="server" OnClick="lbVariaionImages_OnClick" CommandArgument="<%# Eval(Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) %>" data-variationid="<%# Eval(Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) %>">
							<w2c:ProductImage ImageTagId="picture" ImageSize="S" ProductMaster="<%# Container.DataItem %>" IsVariation="true" runat="server" />
						</asp:LinkButton>
					</ItemTemplate>
					<FooterTemplate>
					</FooterTemplate>
				</asp:Repeater>
				<%--サブ画像一覧--%>
				<asp:Repeater DataSource="<%# this.ProductSubImageList %>" Visible="<%# (this.ProductSubImageList.Count != 0) %>" runat="server">
					<HeaderTemplate></HeaderTemplate>
					<ItemTemplate>
						<a href="javascript:void(0);" visible='<%# IsSubImagesNoLimit((int)Eval(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO)) %>' runat="server">
						<img src="<%# WebSanitizer.HtmlEncode(CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_M, (int)Eval(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO))) %>" alt="" /></a>
					</ItemTemplate>
					<FooterTemplate></FooterTemplate>
				</asp:Repeater>
			</div>

			<div class="arrow" Visible="<%# this.HasVariation || (this.ProductSubImageList.Count != 0) %>" runat="server">
				<p class="prev">前へ</p>
				<p class="next">次へ</p>
			</div>
		
		</div>
	</div>


	<%-- SNSボタン ※mixiチェックはクライアント毎にデベロッパ登録したキーを設定する必要あり --%>
	<div class="sns">
		<%-- twitter --%>
		<div class="button">
			<a href="https://twitter.com/share" class="twitter-share-button" data-count="none" data-lang="ja">ツイート</a><script type="text/javascript" src="https://platform.twitter.com/widgets.js"></script>
		</div>
		<%-- mixi --%>
		<div class="button">
			<a href="javascript:void(0);" onclick='<%# WebSanitizer.HtmlEncode("window.open('http://mixi.jp/share.pl?u=" + HttpUtility.UrlEncode(this.UnsecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_PRODUCT_DETAIL + "?" + Constants.REQUEST_KEY_PRODUCT_ID + "=" + this.ProductId + (Constants.PRODUCT_BRAND_ENABLED ? "&" + Constants.REQUEST_KEY_BRAND_ID + "=" + this.BrandId : "") + "&k=01ac61d95d41a50ea61d0c5ab84adf0cfbf62f7d") + "','share',['width=632','height=456','location=yes','resizable=yes','toolbar=no','menubar=no','scrollbars=no','status=no'].join(','));") %>'><img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/mixi_bt_check_1.png" alt="mixiチェック" border="0" /></a>
		</div>
		<%-- facebook --%>
		<div class="button">
			<iframe src="//www.facebook.com/plugins/like.php?href=<%# HttpUtility.UrlEncode(this.UnsecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_PRODUCT_DETAIL + "?" + Constants.REQUEST_KEY_PRODUCT_ID + "=" + this.ProductId) %><%# HttpUtility.UrlEncode(Constants.PRODUCT_BRAND_ENABLED ? "&" + Constants.REQUEST_KEY_BRAND_ID + "=" + this.BrandId : "") %>&amp;send=false&amp;layout=button_count&amp;width=450&amp;show_faces=false&amp;action=like&amp;colorscheme=light&amp;font=tahoma&amp;height=21" scrolling="no" frameborder="0" style="border:none; overflow:hidden; width:100px; height:21px;" allowTransparency="true"></iframe>
		</div>
	</div>

	<div class="wrap-product-cart">

		<div class="product-vatiation unit" runat="server">

		<%-- ドロップダウン形式 --%>
		<% if(this.HasVariation) {%>
			<% if ((this.SelectVariationKbn == Constants.SelectVariationKbn.PANEL)
				|| (this.IsVariationName3 && ((this.SelectVariationKbn == Constants.SelectVariationKbn.DOUBLEDROPDOWNLIST)
					|| (this.SelectVariationKbn == Constants.SelectVariationKbn.MATRIX)
					|| (this.SelectVariationKbn == Constants.SelectVariationKbn.MATRIXANDMESSAGE)))) { %>
				<asp:HiddenField ID="hIsSelectingVariationExist" Value="<%# this.IsSelectingVariationExist %>" runat="server" />
				<asp:Repeater ID="rVariationName1List" DataSource="<%# this.ProductVariationName1List %>" runat="server">
					<HeaderTemplate>
						<div style="width:100%; padding-bottom:30px; clear:both">
							<div style="width:100%">
								<span>Color</span><br />
							</div>
							<div style="width:100%">
								<div style="padding-left:10px; width:10%; float:left">&nbsp;</div>
								<div style="float:left; width:80%">
					</HeaderTemplate>
					<ItemTemplate>
						<div>
							<asp:LinkButton ID="lbVariationName1List" OnClick="lbVariationName1List_OnClick" CommandArgument="<%# Container.DataItem %>" runat="server">
								<div class="<%# ((string)Container.DataItem == this.SelectedVariationName1) ? "VariationPanelSelected" : "VariationPanel" %>"><%#: Container.DataItem %></div>
							</asp:LinkButton>
						</div>
					</ItemTemplate>
					<FooterTemplate>
								</div>
							</div>
						</div>
					</FooterTemplate>
				</asp:Repeater>
				<br />
				<% if (this.ProductVariationName2List.Count > 0) { %>
				<asp:Repeater ID="rVariationName2List" DataSource="<%# this.ProductVariationName2List %>" runat="server">
					<HeaderTemplate>
						<div style="width:100%; padding-bottom:30px; clear:both">
							<hr /><br />
							<div style="width:100%">
								<span>Size</span><br />
							</div>
							<div style="width:100%">
								<div style="padding-left:10px; width:10%; float:left">&nbsp;</div>
								<div style="float:left; width:80%">
					</HeaderTemplate>
					<ItemTemplate>
						<div>
							<asp:LinkButton ID="lbVariationName2List" OnClick="lbVariationName2List_OnClick" CommandArgument="<%# Container.DataItem %>" runat="server">
								<div class="<%# ((string)Container.DataItem == this.SelectedVariationName2) ? "VariationPanelSelected" : "VariationPanel" %>"><%#: Container.DataItem %></div>
							</asp:LinkButton>
						</div>
					</ItemTemplate>
					<FooterTemplate>
								</div>
							</div>
						</div>
					</FooterTemplate>
				</asp:Repeater>
				<%} %>
				<% if (this.ProductVariationName3List.Count > 0) { %>
				<br />
				<asp:Repeater ID="rVariationName3List" DataSource="<%# this.ProductVariationName3List %>" runat="server">
					<HeaderTemplate>
						<div style="width:100%; padding-bottom:30px; clear:both">
							<hr /><br />
							<div style="width:100%">
								<span>Type</span><br />
							</div>
							<div style="width:100%">
								<div style="padding-left:10px; width:10%; float:left">&nbsp;</div>
								<div style="float:left; width:80%">
					</HeaderTemplate>
					<ItemTemplate>
						<div>
						<asp:LinkButton ID="lbVariationName3List" OnClick="lbVariationName3List_OnClick" CommandArgument="<%# Container.DataItem %>" runat="server">
							<div class="<%# ((string)Container.DataItem == this.SelectedVariationName3) ? "VariationPanelSelected" : "VariationPanel" %>"><%#: Container.DataItem %></div>
						</asp:LinkButton>
						</div>
					</ItemTemplate>
					<FooterTemplate>
								</div>
							</div>
						</div>
					</FooterTemplate>
				</asp:Repeater>
				<%} %>
			<%} else if ((this.SelectVariationKbn == Constants.SelectVariationKbn.STANDARD) || (this.SelectVariationKbn == Constants.SelectVariationKbn.DROPDOWNLIST)) { %>
				<div class="drop-down">
					<asp:DropDownList ID="ddlVariationSelect" CssClass="variation01" DataSource='<%# this.ProductValirationListItemCollection %>' DataTextField="Text" DataValueField="Value" SelectedValue='<%# (this.HasVariation && this.VariationSelected && ((this.SelectVariationKbn == Constants.SelectVariationKbn.STANDARD) || (this.SelectVariationKbn == Constants.SelectVariationKbn.DROPDOWNLIST))) ? this.VariationId : null %>' Visible="<%# this.HasVariation %>" OnSelectedIndexChanged="ddlVariationId_SelectedIndexChanged" AutoPostBack="True" runat="server"></asp:DropDownList>
				</div>
			<%} else if ((this.SelectVariationKbn == Constants.SelectVariationKbn.DOUBLEDROPDOWNLIST)) { %>
				<div class="drop-down">
					<div  style="color: red; font-size: 12px; margin-bottom: 10px"><asp:Literal ID="lCombinationErrorMessage" runat="server" /></div>
					<asp:DropDownList ID="ddlVariationSelect1" CssClass="variation01" DataSource='<%# this.ProductValirationListItemCollection %>' DataTextField="Text" DataValueField="Value" SelectedValue='<%# (this.HasVariation && (this.SelectedVariationName1 != "") && (this.SelectVariationKbn == Constants.SelectVariationKbn.DOUBLEDROPDOWNLIST)) ? this.SelectedVariationName1 : null %>' Visible="<%# this.HasVariation %>" OnSelectedIndexChanged="ddlVariationId_SelectedIndexChanged" AutoPostBack="True" runat="server"></asp:DropDownList>
					<asp:DropDownList ID="ddlVariationSelect2" DataSource='<%# this.ProductValirationListItemCollection2 %>' DataTextField="Text" DataValueField="Value" SelectedValue='<%# (this.HasVariation && (this.SelectVariationKbn == Constants.SelectVariationKbn.DOUBLEDROPDOWNLIST)) ? this.SelectedVariationName2 : null %>' Visible="<%# this.HasVariation %>" OnSelectedIndexChanged="ddlVariationId_SelectedIndexChanged" AutoPostBack="True" runat="server"></asp:DropDownList>
				</div>
			<%} %>
		<%} %>
		
		<%-- マトリックス形式 --%>
		<asp:Repeater ID="rAddCartVariationList" DataSource="<%# this.ProductVariationAddCartList %>" 
			onitemcommand="rAddCartVariationList_ItemCommand" runat="server"
			OnItemDataBound="rAddCartVariationList_ItemDataBound">
			<HeaderTemplate>
			<table class="matrix">
				<tbody>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
				<td class="variation">
					<%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1)) %>
					<span visible='<%# ((string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2) != "") %>' runat="server">
						/
						<%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2)) %>
					</span>
					<span visible='<%# ((string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3) != "") %>' runat="server">
						/
						<%#: GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3) %>
					</span>
				</td>
				<td class="stock" visible='<%# ((string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN) != "0") %>' runat="server">
					<!-- / -->
					<span visible='<%# ((string)GetKeyValue(Container.DataItem, "StockMessage") == "") %>' runat="server">
						在庫数量：<%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTSTOCK_STOCK)) %>
					</span>
					<span visible='<%# ((string)GetKeyValue(Container.DataItem, "StockMessage") != "") %>' runat="server">
						<%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, "StockMessage")) %>
					</span>
				</td>
				<td class="button">
					<asp:PlaceHolder ID="plhNotSubscriptionBoxOnly" runat="server">
					<asp:LinkButton ID="lbCartAddVariationList" runat="server" Visible='<%# (bool)GetKeyValue(Container.DataItem, "CanCart") %>' CommandName="CartAdd" CssClass="btn" OnClientClick="return add_cart_check_for_variationlist();">
						カートへ
					</asp:LinkButton>

					<asp:LinkButton ID="lbCartAddFixedPurchaseVariationList" runat="server" Visible='<%# ((bool)GetKeyValue(Container.DataItem, "CanFixedPurchase")) && (this.IsUserFixedPurchaseAble) %>' OnClientClick="return add_cart_check_for_fixedpurchase_variationlist();" CommandName="CartAddFixedPurchase" CssClass="btn">
						カートへ(定期購入)
					</asp:LinkButton>
					<span runat="server" Visible='<%# (this.CanFixedPurchase) && ((string)this.ProductMaster[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG] == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY) && (this.IsUserFixedPurchaseAble == false) %>' style="color: red;">定期購入のご利用はできません</span>
					</asp:PlaceHolder>
					<asp:DropDownList ID="ddlSubscriptionBox" DataTextField="DisplayName" DataValueField="CourseID" runat="server" Visible="false" AutoPostBack="True"></asp:DropDownList>
					<asp:LinkButton ID="lbCartAddSubscriptionBoxList" runat="server" OnClientClick="return add_cart_check_for_subscriptionBox_variationlist(this);" CommandName="CartAddSubscriptionBox" class="btn btn-mid btn-inverse" Visible="false">
						頒布会申し込み
					</asp:LinkButton>
					<asp:HiddenField ID="hfVariation" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) %>" runat="server"/>
					<asp:HiddenField ID="htShopId" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_SHOP_ID) %>" runat="server"/>
					<asp:HiddenField ID="hfProduct" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID) %>" runat="server"/>
					<asp:HiddenField ID="hfSubscriptionBoxFlg" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG) %>" runat="server"/>
					<asp:HiddenField ID="hfCanFixedPurchase" Value='<%# (bool)GetKeyValue(Container.DataItem, "CanFixedPurchase") %>' runat="server"/>
					<asp:HiddenField ID="hfFixedPurchase" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG) %>" runat="server"/>
					<asp:HiddenField ID="hfSubscriptionBoxDisplayName"  runat="server" />
					<div ID="dSubscriptionBoxItemPriceStrikethrough" Visible="False" runat="server" style="text-decoration: line-through" >
						頒布会通常価格：<strong><%# CurrencyManager.ToPrice(GetProductFixedPurchasePrice(Container.DataItem)) %></strong>(<%#: this.ProductPriceTextPrefix %>)
					</div>
					<div ID="dSubscriptionBoxItemPrice" runat="server" Visible="False">
						頒布会通常価格：<strong><%# CurrencyManager.ToPrice(GetProductFixedPurchasePrice(Container.DataItem)) %></strong>(<%#: this.ProductPriceTextPrefix %>)
					</div>
					<div ID="dSubscriptionBoxItemCampaignPrice" Visible="False" runat="server">
						頒布会特別価格：<strong><asp:Literal ID="lSubscriptionBoxItemCampaignPrice" runat="server"></asp:Literal></strong>(<%#: this.ProductPriceTextPrefix %>)
					</div>
					<div ID="dSubscriptionBoxItemCampaignPeriod" Visible="False" runat="server" style="color: red;">
						キャンペーン期間：<asp:Literal ID="lSubscriptionBoxItemCampaignPeriodSince" runat="server"></asp:Literal> ~ <asp:Literal ID="lSubscriptionBoxItemCampaignPeriodUntil" runat="server"></asp:Literal>
					</div>
					<div ID="dSubscriptionBoxPrice" Visible="False" runat="server">
						頒布会申し込み価格：<strong><asp:Literal ID="lSubscriptionBoxPriceSince" runat="server"></asp:Literal><span runat="server" ID="sSince"> ~ </span><asp:Literal ID="lSubscriptionBoxPriceUntil" runat="server"></asp:Literal></strong>(<%#: this.ProductPriceTextPrefix %>)
					</div>
					<asp:LinkButton ID="lbCartAddForGiftVariationList" runat="server" Visible='<%# (bool)GetKeyValue(Container.DataItem, "CanGiftOrder") %>' CommandName="CartAddGift" CssClass="btn">
					カートへ(ギフト購入)
					</asp:LinkButton>
					<% if (Constants.VARIATION_FAVORITE_CORRESPONDENCE){ %>
					<asp:LinkButton ID="favoriteVariation" runat="server" CommandName="AddFavorite" class="btn btn-mid" OnClientClick=<%# (Alertdisplaycheck(this.ShopId, this.LoginUserId, (string)GetKeyValue(Container.DataItem, Constants.FIELD_FAVORITE_PRODUCT_ID), (string)GetKeyValue(Container.DataItem, Constants.FIELD_FAVORITE_VARIATION_ID))) ? "display_alert_check_for_mailsend()" : "" %>>
						<%# (FavoriteDisplayWord(this.ShopId, this.LoginUserId, (string)GetKeyValue(Container.DataItem, Constants.FIELD_FAVORITE_PRODUCT_ID), (string)GetKeyValue(Container.DataItem, Constants.FIELD_FAVORITE_VARIATION_ID))) ? "お気に入り登録済み" : "お気に入りに追加" %>
						(<%# SetFavoriteDataForDisplay((string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID), (string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>人)
					</asp:LinkButton>
					<% } %>

					<%-- 完売表示 
					<div class="sold-out" visible='<%# ProductListUtility.IsProductSoldOut(this.ProductMaster) %>' runat="server">
						SOLD OUT
					</div>
					--%>

					<asp:Repeater ID="rSetPromotionVariationList" DataSource='<%# GetKeyValue(Container.DataItem, "SetPromotionList") %>' runat="server">
					<ItemTemplate>
						<%# ((SetPromotionModel)Container.DataItem).SetpromotionDispName%><br />
					</ItemTemplate>
					</asp:Repeater>

					<%--再入荷メールボタン表示--%>
					<div visible='<%# ((string)GetKeyValue(Container.DataItem, "ArrivalMailKbn") == Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL) %>' runat="server">
						<asp:LinkButton ID="lbRequestArrivalMailVariationList2" Runat="server" CommandName="SmartArrivalMail" CommandArgument="Arrival" CssClass="btn re-stock">
							入荷お知らせメール申込
						</asp:LinkButton>
						<span visible="<%# IsArrivalMailAnyRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL, (string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>" runat="server"><br />登録済みです。</span>
						<span visible="<%# IsArrivalMailPcRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL, (string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>" runat="server"> [PC]</span>
						<span visible="<%# IsArrivalMailMobileRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL, (string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>" runat="server"> [モバイル]</span>
						<span visible="<%# IsArrivalMailGuestRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL, (string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>" runat="server"> [その他]</span>
					</div>

					<%--販売開始メールボタン表示--%>
					<div visible='<%# ((string)GetKeyValue(Container.DataItem, "ArrivalMailKbn") == Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE) %>' runat="server">
						<asp:LinkButton ID="lbRequestReleaseMailVariationList2" Runat="server" CommandName="SmartArrivalMail" CommandArgument="Release" CssClass="btn start-sale">
							販売開始通知メール申込
						</asp:LinkButton>
						<span visible="<%# IsArrivalMailAnyRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE, (string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>" runat="server"><br />登録済み!!</span>
						<span visible="<%# IsArrivalMailPcRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE, (string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>" runat="server"> [PC]</span>
						<span visible="<%# IsArrivalMailMobileRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE, (string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>" runat="server"> [モバイル]</span>
						<span visible="<%# IsArrivalMailGuestRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE, (string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>" runat="server"> [その他]</span>
					</div>

					<%--再販売メールボタン表示--%>
					<div visible='<%# ((string)GetKeyValue(Container.DataItem, "ArrivalMailKbn") == Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE) %>' runat="server">
						<asp:LinkButton ID="lbRequestResaleMailVariationList2" Runat="server" CommandName="SmartArrivalMail" CommandArgument="Resale" CssClass="btn re-sale">
							再販売通知メール申込
						</asp:LinkButton>
						<span visible="<%# IsArrivalMailAnyRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE, (string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>" runat="server"><br />登録済み!!</span>
						<span visible="<%# IsArrivalMailPcRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE, (string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>" runat="server"> [PC]</span>
						<span visible="<%# IsArrivalMailMobileRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE, (string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>" runat="server"> [モバイル]</span>
						<span visible="<%# IsArrivalMailGuestRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE, (string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>" runat="server"> [その他]</span>
					</div>

					<p class="attention"><%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, "ErrorMessage")) %></p>

					<asp:HiddenField ID="hfVariationId" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) %>" runat="server" />
					<asp:HiddenField ID="htArrivalMailKbn" Value='<%# GetKeyValue(Container.DataItem, "ArrivalMailKbn") %>' runat="server" />
				</td>
				</tr>
				
				<%-- 再入荷通知メール登録フォーム表示 --%>
				<uc:BodyProductArrivalMailRegister runat="server" ID="ucBpamrArrival" ArrivalMailKbn="<%#: Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL %>" ProductId="<%#: GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_PRODUCT_ID) %>" VariationId="<%#: GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) %>" Visible="false" />
				<%-- 販売開始メール登録フォーム表示 --%>
				<uc:BodyProductArrivalMailRegister runat="server" ID="ucBpamrRelease" ArrivalMailKbn="<%#: Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE %>" ProductId="<%#: GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_PRODUCT_ID) %>" VariationId="<%#: GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) %>" Visible="false" />
				<%-- 再販売メール登録フォーム表示 --%>
				<uc:BodyProductArrivalMailRegister runat="server" ID="ucBpamrResale" ArrivalMailKbn="<%#: Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE %>" ProductId="<%#: GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_PRODUCT_ID) %>" VariationId="<%#: GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) %>" Visible="false" />

			</ItemTemplate>
			<FooterTemplate>
				</tbody>
			</table>
			</FooterTemplate>
		</asp:Repeater>

		</div>
	
		<div Visible="<%# this.IsDisplayExcludeFreeShippingText %>" runat="server">
			<p style="color:red;">※本商品は配送料無料適用外です</p>
		</div>

		<%-- リアル店舗在庫一覧 --%>
		<% if (Constants.REALSHOP_OPTION_ENABLED){ %>
		<div class="unit real-shop">
			<a href="" onclick='<%# WebSanitizer.HtmlEncode("javascript:show_popup_window('" + CreateRealShopProductStockListUrl(this.ProductId, (string)this.ProductMaster[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]) + "', 630, 900, true, true, 'ProductRealShopStockList');return false;") %>'>リアル店舗在庫状況</a>
		</div>
		<%} %>

		<div visible="<%# (this.IsSelectingVariationExist) %>" runat="server">
			<p class="attention">
				<%# WebSanitizer.HtmlEncode(this.AlertMessage) %>
			</p>
			<div class="attention"><%: this.ErrorMessageFixedPurchaseMember %></div>
			<p class="attention">
				<%# WebSanitizer.HtmlEncode(this.LimitedPaymentMessages) %>
			</p>
		</div>

		<div visible="<%# this.Buyable %>" class="wrap-buyable" runat="server">

		<%-- 在庫文言表示 --%>
			<div visible="<%# this.IsStockManaged == false %>" runat="server">
				<p>
					在庫状況 : <%# WebSanitizer.HtmlEncode(w2.App.Common.Order.ProductCommon.CreateProductStockMessage(this.ProductMaster, true)) %>
				</p>
			</div>
			<%if (this.HasStockMessage) {%>
			<div class="unit stock">
				<%if (this.HasVariation) {%>
					<a href="" onclick='<%# WebSanitizer.HtmlEncode("javascript:show_popup_window('" + CreateProductStockListUrl() + "', 700, 400, true, true, 'ProductStockList');return false;") %>'>在庫状況はこちら</a>
				<% } %>
				<%if (this.HasVariation == false) {%>
					在庫状況：<%# WebSanitizer.HtmlEncode(w2.App.Common.Order.ProductCommon.CreateProductStockMessage(this.ProductMaster, true)) %>
				<%} %>
			</div>
			<%}%>

			<%-- if (this.IsLoggedIn) { --%>
			<%-- お気に入り --%>
			<div class="unit button favorite">
				<asp:LinkButton CSSClass="btn" ID="lbAddFavorite" OnClick="lbAddFavorite_Click" runat="server" OnClientClick=<%# (Alertdisplaycheck((string) GetKeyValue(this.ProductMaster, Constants.FIELD_FAVORITE_SHOP_ID), this.LoginUserId, (string) GetKeyValue(this.ProductMaster, Constants.FIELD_FAVORITE_PRODUCT_ID), "")) ? "display_alert_check_for_mailsend()" : "" %>>
					<%# (FavoriteDisplayWord((string) GetKeyValue(this.ProductMaster, Constants.FIELD_FAVORITE_SHOP_ID), this.LoginUserId, (string) GetKeyValue(this.ProductMaster, Constants.FIELD_FAVORITE_PRODUCT_ID), "")) ? "お気に入り登録済み" : "お気に入りに追加" %>
					(<%# SetFavoriteDataOfProductForDisplay() %>人)
				</asp:LinkButton>
				<p id="addCartTip" class="toolTip" style="display: none;">
					<span id="txt-tooltip"></span><br/>
					<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_FAVORITE_LIST) %>"><img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/user/btn_menu_favolite.gif" alt="お気に入り一覧"></a>
				</p>
				<% if(Constants.VARIATION_FAVORITE_CORRESPONDENCE){ %>
					<p><br />お気に入りの総登録人数：<%# SetTotalFavoriteDataForDisplay() %>人</p>
				<% }else{ %>	
					<p><br />お気に入りの登録人数：<%# this.FavoriteUserCount %>人</p>
				<% } %>
			</div>
			<%-- } --%>

			<div visible="<%# this.IsDisplayValiationStock %>" runat="server">
				<p>在庫有り</p>
			</div><%-- マトリックス形式 --%>

			<div class="button add-to-cart">
				<asp:LinkButton ID="lbCartAdd" runat="server" Visible="<%# (this.CanAddCart) && (this.IsSubscriptionBoxOnly == false) %>" Onclick="lbCartAdd_Click" OnClientClick="return add_cart_check();" CssClass="btn">
				カートへ
				</asp:LinkButton>
			</div>

			<div class="button fixed" visible="<%# this.CanFixedPurchase %>" runat="server">
				<asp:LinkButton ID="lbCartAddFixedPurchase" runat="server" Visible="<%# (this.Buyable && this.CanFixedPurchase) && (this.IsUserFixedPurchaseAble) && (this.IsSubscriptionBoxOnly == false) %>" OnClick="lbCartAddFixedPurchase_Click" OnClientClick="return add_cart_check_for_fixedpurchase();" CssClass="btn">
				カートへ(定期)
				</asp:LinkButton>
			</div>

			<div class="button fixed" visible="<%# this.CanFixedPurchase %>" runat="server">
				<asp:DropDownList ID="ddlSubscriptionBox" DataTextField="DisplayName" DataValueField="CourseID" runat="server" Visible="false"></asp:DropDownList>
				<asp:LinkButton ID="lbCartAddSubscriptionBox" class="btn btn-mid btn-inverse" runat="server" Visible="<%# (this.CanFixedPurchase) && this.IsUserFixedPurchaseAble && (this.IsSubscriptionBoxValid) %>" OnClick="lbCartAddSubscriptionBox_Click" OnClientClick="return add_cart_check_for_subscriptionBox(this);">
				頒布会申し込み
				</asp:LinkButton>
				<asp:HiddenField ID="hfSubscriptionBoxDisplayName"  runat="server" />
			</div>

			<div class="button add-to-cart" visible="<%# this.CanGiftOrder %>" runat="server">
				<asp:LinkButton ID="lbCartAddForGift" runat="server" Visible="<%# (this.CanGiftOrder) %>" OnClick="lbCartAddGift_Click" OnClientClick="return add_cart_check();" CssClass="btn">
				カートへ(ギフト購入)
				</asp:LinkButton>
			</div>

		</div>

		<div class="wrap-not-buyable">
			<div visible="<%# (this.IsSelectingVariationExist == false) %>" runat="server">
				<p class="attention"><%#: this.AlertMessageVariationNotExist %></p>
			</div>
			<%-- 完売表示 --%>
			<div class="button sold-out" visible='<%# ProductListUtility.IsProductSoldOut(this.ProductMaster) %>' runat="server">
				SOLD OUT
			</div>

			<%-- 再入荷メールボタン表示 --%>
			<div visible="<%# this.ArrivalMailKbn == Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL %>" runat="server" class="button others">
				<asp:LinkButton ID="lbRequestArrivalMail2" Runat="server" OnCommand="ViewRegsiterArrivalMailForm_Command" CommandArgument="Arrival" CssClass="btn">
					入荷お知らせメール申込
				</asp:LinkButton>
				<p class="msg-alert" visible="<%# IsArrivalMailAnyRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL) %>" runat="server">
					登録済み
					<span visible="<%# IsArrivalMailPcRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL) %>" runat="server">[PC]</span>
					<span visible="<%# IsArrivalMailMobileRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL) %>" runat="server">[モバイル]</span>
					<span visible="<%# IsArrivalMailGuestRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL) %>" runat="server">[その他]</span>
				</p>
				<%-- 再入荷通知メール登録フォーム表示 --%>
				<uc:BodyProductArrivalMailRegister runat="server" ID="ucBpamrArrival" ArrivalMailKbn="<%#: Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL %>" ProductId="<%# this.ProductId %>" HasVariation="<%# this.HasVariation %>" Visible="false" />
			</div>

			<%--販売開始メール表示--%>
			<div visible="<%# this.ArrivalMailKbn == Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE %>" runat="server" class="button others">
				<asp:LinkButton ID="lbRequestReleaseMail2" Runat="server" OnCommand="ViewRegsiterArrivalMailForm_Command" CommandArgument="Release" CssClass="btn">
					販売開始通知メール申込
				</asp:LinkButton>
				<p class="msg-alert" visible="<%# IsArrivalMailAnyRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE) %>" runat="server">
					登録済み
					<span visible="<%# IsArrivalMailPcRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE) %>" runat="server"> [PC]</span>
					<span visible="<%# IsArrivalMailMobileRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE) %>" runat="server"> [モバイル]</span>
					<span visible="<%# IsArrivalMailGuestRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE) %>" runat="server"> [その他]</span>
				</p>
				<%-- 販売開始通知メール登録フォーム表示 --%>
				<uc:BodyProductArrivalMailRegister runat="server" ID="ucBpamrRelease" ArrivalMailKbn="<%#: Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE %>" ProductId="<%# this.ProductId %>" HasVariation="<%# this.HasVariation %>" Visible="false" />
			</div>

			<%--再販売メール表示--%>
			<div visible="<%# this.ArrivalMailKbn == Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE %>" runat="server" class="button others">
				<asp:LinkButton ID="lbRequestResaleMail2" Runat="server" OnCommand="ViewRegsiterArrivalMailForm_Command" CommandArgument="Resale" CssClass="btn">
					再販売通知メール申込
				</asp:LinkButton>
				<p class="msg-alert" visible="<%# IsArrivalMailAnyRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE) %>" runat="server">
					登録済み
					<span visible="<%# IsArrivalMailPcRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE) %>" runat="server"> [PC]</span>
					<span visible="<%# IsArrivalMailMobileRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE) %>" runat="server"> [モバイル]</span>
					<span visible="<%# IsArrivalMailGuestRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE) %>" runat="server"> [その他]</span>
				</p>
				<%-- 再販売通知メール登録フォーム表示 --%>
				<uc:BodyProductArrivalMailRegister runat="server" ID="ucBpamrResale" ArrivalMailKbn="<%#: Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE %>" ProductId="<%# this.ProductId %>" HasVariation="<%# this.HasVariation %>" Visible="false" />
			</div>
			<% if (StringUtility.ToEmpty(this.ErrorMessage) != "") {%>
			<br /><span class="attention"><%: this.ErrorMessage %></span>
			<%} %>
		</div>

	</div>

	<%-- ▽商品付帯情報(存在しているときのみ表示する）▽ --%>
	<%if (this.ProductOptionSettingList.Items.Count != 0){%>
		<asp:Repeater ID="rProductOptionValueSettings" DataSource="<%# this.ProductOptionSettingList %>" runat="server">
		<HeaderTemplate>
			<div class="wrap-product-incident">
		</HeaderTemplate>
		<ItemTemplate>
			<dl>
				<dt>
					<%--　付帯情報名 --%>
					<asp:Label ID = "lblProductOptionValueSetting" Text = '<%# ((ProductOptionSetting)Container.DataItem).ValueName%>' runat="server" />
					<span class="require" runat="server" visible="<%# ((ProductOptionSetting)Container.DataItem).IsNecessary%>">※</span>
				</dt>
				<dd>
					<%-- ラジオボタン --%>
					<asp:Repeater ID="rCblProductOptionValueSetting"
								DataSource='<%# ((ProductOptionSetting)Container.DataItem).SettingValuesListItemCollection %>'
								ItemType="System.Web.UI.WebControls.ListItem"
								Visible='<%# (((ProductOptionSetting)Container.DataItem).DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_CHECKBOX) || (((ProductOptionSetting)Container.DataItem).DisplayKbn) == Constants.PRODUCTOPTIONVALUES_DISP_KBN_PRICE_CHECKBOX %>'
								runat="server" >
						<ItemTemplate>
							<asp:CheckBox ID="cbProductOptionValueSetting" Text='<%# Item.Text %>' Checked='<%# Item.Selected %>' runat="server" />
						</ItemTemplate>
					</asp:Repeater>
					<p class="attention"><asp:Label ID="lblProductOptionCheckboxErrorMessage" runat="server" /></p>

					<%-- ドロップダウンリスト --%>
					<asp:DropDownList ID="ddlProductOptionValueSetting"
									DataSource='<%# InsertDefaultAtFirstToDdlProductOptionSettingList(((ProductOptionSetting)Container.DataItem).SettingValuesListItemCollection, ((ProductOptionSetting)Container.DataItem).IsNecessary) %>'
									visible='<%# (((ProductOptionSetting)Container.DataItem).DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_SELECTMENU) || (((ProductOptionSetting)Container.DataItem).DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_PRICE_DROPDOWNMENU) %>'
									SelectedValue='<%# ((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectedValue() %>'
									runat="server" />
					<p class="attention"><asp:Label ID="lblProductOptionDropdownErrorMessage" runat="server" /></p>
					<%-- テキストボックス --%>
					<asp:TextBox ID ="txtProductOptionValueSetting" Text = '<%# ((ProductOptionSetting)Container.DataItem).DefaultValue%>' visible='<%# ((ProductOptionSetting)Container.DataItem).DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_TEXTBOX %>' runat="server" />
					<p class="attention"><asp:Label ID = "lblProductOptionErrorMessage" runat="server" /></p>
				</dd>
			</dl>
		</ItemTemplate>
		<FooterTemplate>
			</div>
		</FooterTemplate>
		</asp:Repeater>
	<%} %>
	<%-- △商品付帯情報△ --%>

	<div class="wrap-product-desc">

		<%-- 商品概要 --%>
		<div visible='<%# this.IsProductOutlineVisible %>' runat="server" class="outline unit">
		<h2>概要</h2>
		<p><%# GetProductDataHtml("outline") %></p>
		</div>
		

		<div visible='<%# this.IsProductDetailVisible %>' runat="server" class="desc-detail unit">
		<h2>詳細情報</h2>
		<!-- 商品詳細1 -->
		<p><%# GetProductDataHtml("desc_detail1") %></p>
		<!-- 商品詳細2 -->
		<p><%# GetProductDataHtml("desc_detail2") %></p>
		<!-- 商品詳細3 -->
		<p><%# GetProductDataHtml("desc_detail3") %></p>
		<!-- 商品詳細4 -->
		<p><%# GetProductDataHtml("desc_detail4") %></p>
		</div>

		<div visible='<%# StringUtility.ToEmpty(GetProductData("return_exchange_message")) != "" %>' runat="server" class="unit">
			<%-- 返品交換文言表示 --%>
			<p>
			<%# WebSanitizer.HtmlEncodeChangeToBr(GetProductData("return_exchange_message")) %>
			<%if (ShopMessage.GetMessage("ReturnSpecialContractPage") != "") {%>
			（<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + ShopMessage.GetMessage("ReturnSpecialContractPage")) %>" target="_blank" style='font-size:10px'>返品特約</a>）
			<%} %>
			</p>
		</div>

		<%-- ホームページリンク --%>
		<div visible='<%# StringUtility.ToEmpty(GetProductData("url")) != "" %>' runat="server" class="unit">
			<a href="<%# WebSanitizer.HtmlEncode(GetProductData("url")) %>">メーカのホームページへ</a>
		</div>

		<%-- 問い合わせメールリンク --%>
		<div visible='<%# StringUtility.ToEmpty(GetProductData("inquire_email")) != "" %>' runat="server" class="unit">
			<a href="mailto:<%# WebSanitizer.HtmlEncode(GetProductData("inquire_email")) %>">商品のお問合せ</a>
		</div>

		<%-- 電話問い合わせ --%>
		<div visible='<%# StringUtility.ToEmpty(GetProductData("inquire_tel")) != "" %>' runat="server" class="unit">
			お問合せ：<%# WebSanitizer.HtmlEncode(GetProductData("inquire_tel")) %>
		</div>

		<%-- お問い合わせリンク --%>
		<div class="unit contact" runat="server">
			<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductInquiryUrl(this.ProductMaster, this.VariationSelected)) %>" class="btn">
			この商品を問い合わせる
			</a>
		</div>

	</div>

<%-- アップセル --%>
<asp:Repeater DataSource="<%# this.ProductUpSellList %>" Visible="<%# this.ProductUpSellList.Count != 0 %>" runat="server">
<HeaderTemplate>
	<section class="up-sell unit">
	<h4 class="title">こちらの商品もおすすめ</h4>
	<ul class="product-list-3 clearfix">
</HeaderTemplate>
<ItemTemplate>
<li>
    <a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem)) %>">
    <div class="product-image">
      <w2c:ProductImage ImageTagId="picture" ImageSize="M" ProductMaster="<%# Container.DataItem %>" IsVariation="false" runat="server" />
      <%-- ▽在庫切れ可否▽ --%>
      <span visible='<%# ProductListUtility.IsProductSoldOut(Container.DataItem) %>' runat="server" class="sold-out">SOLD OUT</span>
      <%-- △在庫切れ可否△ --%>
    </div>
    <div class="product-name">
      <%-- ▽商品アイコン▽ --%>
      <%--
      <p>
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
      --%>
      <%-- △商品アイコン△ --%>
      <%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_NAME)) %>
    </div>
    <div class="product-price">
      <%-- ▽商品会員ランク価格有効▽ --%>
      <p visible='<%# ProductPage.GetProductMemberRankPriceValid(Container.DataItem) %>' runat="server" class="special">
        <%#: CurrencyManager.ToPrice(ProductPage.GetProductMemberRankPrice(Container.DataItem, false)) %>
        <span class="line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></span>
      </p>
      <%-- △商品会員ランク価格有効△ --%>

      <%-- ▽商品セール価格有効▽ --%>
      <p visible='<%# ProductPage.GetProductTimeSalesValid(Container.DataItem) %>' runat="server" class="special">
        <%#: CurrencyManager.ToPrice(ProductPage.GetProductTimeSalePriceNumeric(Container.DataItem)) %>
        <span class="line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></span>
      </p>
      <%-- △商品セール価格有効△ --%>

      <%-- ▽商品特別価格有効▽ --%>
      <p visible='<%# ProductPage.GetProductSpecialPriceValid(Container.DataItem) %>' runat="server" class="special">
        <%#: CurrencyManager.ToPrice(ProductPage.GetProductSpecialPriceNumeric(Container.DataItem)) %>
        <span class="line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></span>
      </p>
        <%-- △商品特別価格有効△ --%>

        <%-- ▽商品通常価格有効▽ --%>
        <p visible='<%# ProductPage.GetProductNormalPriceValid(Container.DataItem) %>' runat="server">
          <%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %>
        </p>
        <%-- △商品通常価格有効△ --%>

		<%-- ▽定期購入有効▽ --%>
		<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) {%>
		<p visible='<%# (GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG).ToString() != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) && (this.IsUserFixedPurchaseAble) %>' runat="server">
			<span visible='<%# IsProductFixedPurchaseFirsttimePriceValid(Container.DataItem) %>' runat="server">
				<p class="productPrice">定期初回価格:<span><%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchaseFirsttimePrice(Container.DataItem)) %></span></p>
			</span>
			<p class="productPrice">定期通常価格:<span><%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchasePrice(Container.DataItem)) %></span></p>
		</p>
		<% } %>
		<%-- △定期購入有効△ --%>

    </div>
    </a>
  </li>
  </ItemTemplate>
  <FooterTemplate>
  </ul>
  </section>
  </FooterTemplate>
</asp:Repeater>


<%-- クロスセル --%>
<asp:Repeater DataSource="<%# this.ProductCrossSellList %>" Visible="<%# this.ProductCrossSellList.Count != 0 %>" runat="server">
<HeaderTemplate>
	<section class="up-sell unit">
	<h4 class="title">関連商品</h4>
	<ul class="product-list-3 clearfix">
	</HeaderTemplate>
<ItemTemplate>
<li>
    <a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem)) %>">
    <div class="product-image">
      <w2c:ProductImage ImageTagId="picture" ImageSize="M" ProductMaster="<%# Container.DataItem %>" IsVariation="false" runat="server" />
      <%-- ▽在庫切れ可否▽ --%>
      <span visible='<%# ProductListUtility.IsProductSoldOut(Container.DataItem) %>' runat="server" class="sold-out">SOLD OUT</span>
      <%-- △在庫切れ可否△ --%>
    </div>
    <div class="product-name">
      <%-- ▽商品アイコン▽ --%>
      <%-- 
      <p>
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
      --%>
      <%-- △商品アイコン△ --%>
      <%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_NAME)) %>
    </div>
    <div class="product-price">
      <%-- ▽商品会員ランク価格有効▽ --%>
      <p visible='<%# ProductPage.GetProductMemberRankPriceValid(Container.DataItem) %>' runat="server" class="special">
        <%#: CurrencyManager.ToPrice(ProductPage.GetProductMemberRankPrice(Container.DataItem, false)) %>
        <span class="line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></span>
      </p>
      <%-- △商品会員ランク価格有効△ --%>

      <%-- ▽商品セール価格有効▽ --%>
      <p visible='<%# ProductPage.GetProductTimeSalesValid(Container.DataItem) %>' runat="server" class="special">
        <%#: CurrencyManager.ToPrice(ProductPage.GetProductTimeSalePriceNumeric(Container.DataItem)) %>
        <span class="line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></span>
      </p>
      <%-- △商品セール価格有効△ --%>

      <%-- ▽商品特別価格有効▽ --%>
      <p visible='<%# ProductPage.GetProductSpecialPriceValid(Container.DataItem) %>' runat="server" class="special">
        <%#: CurrencyManager.ToPrice(ProductPage.GetProductSpecialPriceNumeric(Container.DataItem)) %>
        <span class="line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></span>
      </p>
        <%-- △商品特別価格有効△ --%>

        <%-- ▽商品通常価格有効▽ --%>
        <p visible='<%# ProductPage.GetProductNormalPriceValid(Container.DataItem) %>' runat="server">
          <%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %>
        </p>
        <%-- △商品通常価格有効△ --%>

		<%-- ▽定期購入有効▽ --%>
		<p visible='<%# (GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG).ToString() != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) && (this.IsUserFixedPurchaseAble) %>' runat="server">
			<span visible='<%# IsProductFixedPurchaseFirsttimePriceValid(Container.DataItem) %>' runat="server">
				<p class="productPrice">定期初回価格:<span><%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchaseFirsttimePrice(Container.DataItem)) %></span></p>
			</span>
			<p class="productPrice">定期通常価格:<span><%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchasePrice(Container.DataItem)) %></span></p>
		</p>
		<%-- △定期購入有効△ --%>
    </div>
    </a>
  </li>
  </ItemTemplate>
  <FooterTemplate>
  </ul>
  </section>
  </FooterTemplate>
</asp:Repeater>
</article>
</section>

<%-- △編集可能領域△ --%>
<div id="divBottomArea">
<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>
</ContentTemplate>
</asp:UpdatePanel>
<%-- UPDATE PANELここまで --%>

<section class="wrap-product-detail">
<%-- 商品レビュー --%>
<uc:BodyProductReview Visible="<%# Constants.PRODUCTREVIEW_ENABLED %>" ShopId="<%# this.ShopId %>" ProductId="<%# this.ProductId %>" ProductName="<%# this.ProductName %>" ProductReviewCount="5" runat="server"></uc:BodyProductReview>
<%--コーディネート --%>
<uc:BodyProductCoordinate ID="BodyProductCoordinate2" runat="server"/>
<uc:BodyRecommendProductsWithTag ShopId="<%# this.ShopId %>" ProductId="<%# this.ProductId %>" ImageSize="M" DisplayKbn="ALL" runat="server" />
</section>

<script type="text/javascript">
<!--
	var strAlertmessage = '<%= MESSAGE_ERROR_VARIATION_UNSELECTED %>';
	var strAlertMessageOption = '<%= MESSAGE_ERROR_OPTION_UNSELECTED %>';
	var fixedpurchaseMessage = '定期的に商品をお送りする「定期購入」で購入します。\nよろしいですか？';
	var subscriptionBoxMessage = '頒布会「@@ 1 @@」の内容確認画面に進みます。\nよろしいですか？';
	var mailSendAlertMessage = 'お気に入り追加商品の在庫減少時にメールを送信いたします。\nマイページ＞登録情報の変更より変更できます。';

	// バリエーション選択チェック判定
	function variation_selected_check() {
		<% if (this.HasVariation == false) {%>
			return true;
		<%} else {%>
			<% if (this.SelectVariationKbn == Constants.SelectVariationKbn.PANEL) { %>
				return ((document.getElementById('<%# this.WhIsSelectingVariationExist.ClientID %>').value != 'False'));
			<%} else if (this.SelectVariationKbn == Constants.SelectVariationKbn.STANDARD) { %>
				return ((document.getElementById('<%# this.WddlVariationSelect.ClientID %>').value != ''));
			<%} else if (this.SelectVariationKbn == Constants.SelectVariationKbn.DROPDOWNLIST) {%>
				return ((document.getElementById('<%# this.WddlVariationSelect.ClientID %>').value != ''));
			<%} else if (this.SelectVariationKbn == Constants.SelectVariationKbn.DOUBLEDROPDOWNLIST) {%>
				var strVariationSelect = '<%# this.WddlVariationSelect1.ClientID %>';
				var strVariationSelect2 = '<%# this.WddlVariationSelect2.ClientID %>';
				return ((document.getElementById(strVariationSelect) != null) &&
						(document.getElementById(strVariationSelect2) != null) && 
						(document.getElementById(strVariationSelect).value != '') &&
						(document.getElementById(strVariationSelect2).value != ''));
			<%} else if ((this.SelectVariationKbn == Constants.SelectVariationKbn.MATRIX) 
			|| (this.SelectVariationKbn == Constants.SelectVariationKbn.MATRIXANDMESSAGE)) {%>
				var blSelectChecked = false;
				for (var iLoop = 0; iLoop < document.getElementsByName('Variation').length; iLoop++) {
					if (document.getElementsByName('Variation')[iLoop].checked) {
						blSelectChecked =  true;
					}
				}
				return blSelectChecked;
			<%} %>
		<%} %>
	}

	// バリエーション選択チェック(通常)
	function add_cart_check() {
		if (variation_selected_check()) {
			var alertMessage = get_alert_message_for_product_option_unselected();
			if (alertMessage) {
				alert(alertMessage);
				return false;
			}
			return true;
		}
		else {
			alert(strAlertmessage);
			return false;
		}
	}

	// バリエーションカート投入時チェック(定期)
	function add_cart_check_for_fixedpurchase() {
		var strCartAddFixedPurchase = '<%# this.WlbCartAddFixedPurchase.ClientID %>';
		var CartAddFixedPurchaseDisabled = document.getElementById(strCartAddFixedPurchase).attributes.disabled;
		if (variation_selected_check()) {
			var alertMessage = get_alert_message_for_product_option_unselected();
			if (alertMessage) {
				alert(alertMessage);
				return false;
			}
			if (CartAddFixedPurchaseDisabled === undefined) {
				return confirm(fixedpurchaseMessage);
			}
			return false;
		}
		else {
			alert(strAlertmessage);
			return false;
		}
	}

	// バリエーション選択チェック(頒布会)
	function add_cart_check_for_subscriptionBox(value) {
		if (variation_selected_check()) {
			var subscriptionBoxName = ($(value).parent().find("[id$='ddlSubscriptionBox']").length > 0)
				? $('#<%: ddlSubscriptionBox.ClientID %> option:selected')[0].innerText
				: $(value).parent().find("[id$='hfSubscriptionBoxDisplayName']").val();
			return confirm('頒布会「' + subscriptionBoxName + '」の内容確認画面に進みます。\nよろしいですか？');
		}
		else {
			alert(strAlertmessage);
			return false;
		}
	}

	// バリエーション選択チェック(入荷通知メール申込)
	function request_user_product_arrival_mail_check() {
		if (variation_selected_check()) {
			return true;
		}
		else {
			alert(strAlertmessage);
			return false;
		}
	}

	//メール送信があることを確認
	function display_alert_check_for_mailsend() {
		alert(mailSendAlertMessage);
	}

	// 入荷通知登録画面をポップアップウィンドウで開く
	function show_arrival_mail_popup(pid, vid, amkbn) {
		show_popup_window('<%= this.SecurePageProtocolAndHost %><%= Constants.PATH_ROOT %><%= Constants.PAGE_FRONT_USER_PRODUCT_ARRIVAL_MAIL_REGIST %>?<%= Constants.REQUEST_KEY_PRODUCT_ID %>=' + pid + '&<%= Constants.REQUEST_KEY_VARIATION_ID %>=' + vid + '&<%= Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN %>=' + amkbn, 480, 270, true, true, 'Information');
	}

	// バリエーションリスト毎のカート投入時チェック(通常商品)
	function add_cart_check_for_variationlist() {
		var alertMessage = get_alert_message_for_product_option_unselected();
		if (alertMessage) {
			alert(alertMessage);
			return false;
		}
		return true;
	}

	// バリエーションリスト毎のカート投入時チェック(定期商品)
	function add_cart_check_for_fixedpurchase_variationlist() {
		var alertMessage = get_alert_message_for_product_option_unselected();
		if (alertMessage) {
			alert(alertMessage);
			return false;
		}
		return confirm(fixedpurchaseMessage);
	}

	<%-- 商品付帯情報未選択時のアラートメッセージを取得 --%>
	<%-- NOTE: 付帯価格オプションが有効の際は、ドロップダウンの付帯情報の必須入力チェックを行うため利用 --%>
	function get_alert_message_for_product_option_unselected() {
		<% if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false){ %>
			return "";
		<% } %>

		var productOptionUnselectedMessageList = [];
		<% foreach (RepeaterItem ri in rProductOptionValueSettings.Items){ %>
			var ddlProductOptionSetting = document.getElementById('<%= ((DropDownList)ri.FindControl("ddlProductOptionValueSetting")).ClientID %>');
			if (ddlProductOptionSetting) {
				if (ddlProductOptionSetting.value == '<%# ReplaceTag("@@DispText.variation_name_list.unselected@@") %>') {
					var productOptionName = document.getElementById('<%= ((Label)ri.FindControl("lblProductOptionValueSetting")).ClientID %>').innerHTML;
					productOptionUnselectedMessageList.push(strAlertMessageOption.format(productOptionName));
				}
			}
		<% } %>

		var alertMessage = productOptionUnselectedMessageList.join('\r\n');
		return alertMessage;
	}
//-->
</script>

<%-- CRITEOタグ --%>
<uc:Criteo ID="criteo" runat="server" Datas="<%# null %>" />
</asp:Content>
