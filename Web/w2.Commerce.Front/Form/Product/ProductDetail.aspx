<%--
=========================================================================================================
  Module      : 商品詳細画面(ProductDetail.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%@ Register TagPrefix="uc" TagName="BodyProductCategoryTree" Src="~/Form/Common/Product/BodyProductCategoryTree.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductRanking" Src="~/Form/Common/Product/BodyProductRanking.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductHistory" Src="~/Form/Common/Product/BodyProductHistory.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyMiniCart" Src="~/Form/Common/BodyMiniCart.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductRecommendByRecommendEngine" Src="~/Form/Common/Product/BodyProductRecommendByRecommendEngine.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyCategoryRecommendByRecommendEngine" Src="~/Form/Common/Product/BodyCategoryRecommendByRecommendEngine.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProductColorSearchBox" Src="~/Form/Common/Product/ProductColorSearchBox.ascx" %>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Register TagPrefix="uc" TagName="BodyProductCoordinate" Src="~/Form/Common/Product/BodyProductCoordinate.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductArrivalMailRegister" Src="~/Form/Common/Product/BodyProductArrivalMailRegister.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductArrivalMailRegisterTr" Src="~/Form/Common/Product/BodyProductArrivalMailRegisterTr.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductStockList" Src="~/Form/Common/Product/BodyProductStockList.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductCategoryLinks" Src="~/Form/Common/Product/BodyProductCategoryLinks.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductReview" Src="~/Form/Common/Product/BodyProductReview.ascx" %>
<%@ Register TagPrefix="uc" TagName="Criteo" Src="~/Form/Common/Criteo.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyRecommendProductsWithTag" Src="~/Form/Common/Product/BodyRecommendProductsWithTag.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Form/Product/ProductDetail.aspx.cs" Inherits="Form_Product_ProductDetail" Title="商品詳細ページ" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<script type="text/javascript" src="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "Js/jquery.elevateZoom-3.0.8.min.js") %>"></script>
<link href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "Css/product.css") %>" rel="stylesheet" type="text/css" media="all" />
<link rel="canonical" href="<%# CreateProductDetailCanonicalUrl() %>" />
<% if ((Constants.MOBILEOPTION_ENABLED)
	&& (this.ProductMaster != null)
	&& ((string)this.ProductMaster[Constants.FIELD_PRODUCT_MOBILE_DISP_FLG] == Constants.FLG_PRODUCT_MOBILE_DISP_FLG_ALL)){%>
	<link rel="Alternate" media="handheld" href="<%= WebSanitizer.HtmlEncode(GetMobileUrl()) %>" />
<% } %>
<%= this.BrandAdditionalDsignTag %>
<% if (Constants.SEOTAG_AND_OGPTAG_IN_PRODUCTDETAIL_ENABLED){ %>
	<meta name="Keywords" content="<%: this.SeoKeywords %>" />
<% } %>

<script type="text/javascript">
//<![CDATA[
	$(function () {
		/* 詳細画像切り替え
		var regrep = "_M.jpg";
		$(".subImage li img").mouseover( changePhoto );
		function changePhoto(){
		var setname = $(this).attr("src").replace(regrep,"_L.jpg");
		$("#picture").attr("src",setname).css("opacity","0.2").fadeTo(300,1);
		}*/

		$('#zoomPicture').elevateZoom({
			zoomWindowWidth: 393,
			zoomWindowHeight: 393,
			responsive: true,
			zoomWindowOffetx: 15,
			borderSize: 1,
			cursor: "pointer"
		});

		$('.zoomTarget').click(function (e) {
			var image = $(this).data('image');
			var zoom_image = $(this).data('zoom-image');
			var ez = $('#zoomPicture').data('elevateZoom');
			ez.swaptheimage(image, zoom_image);
		});

	});
//]]>
</script>
<%-- △編集可能領域△ --%>

<style type="text/css">
	.VariationPanel
	{
		width:100px;
		border:1px;
		padding:10px 5px 5px 5px;
		margin-right: 15px;
		margin-bottom: 10px;
		border-style:solid;
		border-color: #adb0b0;
		background-color: #f5f7f7;
		float:left
	}
	.VariationPanelSelected
	{
		width:100px;
		border:1px;
		border-style:solid;
		padding:10px 5px 5px 5px;
		margin-right: 15px;
		margin-bottom: 10px;
		background-color: #dbdfdf;
		float:left
	}
	[id$="ddlSubscriptionBox"] {
		height: 28px;
		margin-left: 10px;
	}
	.field-boxes {
		display: -webkit-box;
		display: -moz-box;
		display: -ms-flexbox;
		display: -webkit-flex;
		display: flex;
		-webkit-flex-flow: row wrap;
		flex-flow: row wrap;
		justify-content: flex-start;
		max-width: 100%;
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

<%-- UPDATEPANELによりthickboxが動作しないバグ対応 --%>
<script type="text/javascript" language="javascript">
	function bodyPageLoad() {
		if (Sys.WebForms == null) return;
		var isAsyncPostback = Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();
		if (isAsyncPostback) {
			$('.zoomContainer').remove(); // zoomContainerの増殖防止
			tb_init('a.thickbox, area.thickbox, input.thickbox');
			$(function () {
				$(".productInfoList").heightLine().biggerlink();
				$('#zoomPicture').elevateZoom({
					zoomWindowWidth: 393,
					zoomWindowHeight: 393,
					responsive: true,
					zoomWindowOffetx: 15,
					borderSize: 1,
					cursor: "pointer"
				});

				$('.zoomTarget').click(function (e) {
					$('.zoomTarget').removeClass('selected');
					$(this).addClass('selected');
					var image = $(this).data('image');
					var zoom_image = $(this).data('zoom-image');
					var ez = $('#zoomPicture').data('elevateZoom');
					ez.swaptheimage(image, zoom_image);
				});
			});
			if (typeof twttr !== 'undefined')
			{
			twttr.widgets.load(); //Reload twitter button
			}

		}
	}
</script>

<table id="tblLayout" class="tblLayout_ProductDetail">
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

<%-- カート投入ボタン押下時にどの画面へ遷移するか？ --%>
<%-- CART：カート一覧画面 その他：画面遷移しない --%>
<asp:HiddenField ID="hfIsRedirectAfterAddProduct" Value="CART" runat="server" />

<%-- お気に入り追加ボタン押下時にどの画面へ遷移するか？ --%>
<%-- true:ポップアップ表示、false:お気に入り一覧ページへ遷移 --%>
<% IsDisplayPopupAddFavorite = true; %>

<!--▽ 上部カテゴリバー ▽-->
<div id="breadcrumb">
	<uc:BodyProductCategoryLinks runat="server"></uc:BodyProductCategoryLinks>
</div>
<!--△ 上部カテゴリバー △-->

<div id="dvProductDetailArea">
<%-- UPDATE PANEL開始 --%>
<asp:UpdatePanel ID="upUpdatePanel" runat="server">
<ContentTemplate>

<div id="detailImage">

<%-- ↓バリエーション変更時の表示更新領域を指定しています --%>
<div class="ChangesByVariation" runat="server">
	<!-- 商品画像 -->
	<div class="mainImage">
	<p class="mb5"><a href='<%# CreateUrlForProductZoomImage() %>' class="thickbox btn btn-mini">拡大画像を表示する</a></p>
	<a class="thickbox" rel="gal1" href='<%# CreateUrlForProductZoomImage() %>'>
	<w2c:ProductImage ImageTagId="zoomPicture" data-zoom-image="" ImageSize="LL" IsVariation="<%# (this.VariationSelected) %>" ProductMaster="<%# this.ProductMaster %>" runat="server" />
	</a>
	<%-- ▽在庫切れ可否▽ --%>
	<span visible='<%# ProductListUtility.IsProductSoldOut(this.ProductMaster) %>' runat="server" class="soldout">SOLDOUT</span>
	<%-- △在庫切れ可否△ --%>
</div>
</div>
<%-- ↑バリエーション変更時の表示更新領域を指定しています --%>

<ul class="btnListContact">
	<li>
	<!-- お問い合わせリンク -->
	<div class="ChangesByVariation" runat="server">
	<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductInquiryUrl(this.ProductMaster, this.VariationSelected)) %>">この商品に関する問い合わせ</a>
	</div>
	</li><!--
	--><li><% if(Constants.VARIATION_FAVORITE_CORRESPONDENCE){ %>
		<asp:LinkButton ID="lbAddFavoriteId" OnClick="lbAddFavorite_Click" runat="server" OnClientClick=<%# (Alertdisplaycheck((string) GetKeyValue(this.ProductMaster, Constants.FIELD_FAVORITE_SHOP_ID), this.LoginUserId, (string) GetKeyValue(this.ProductMaster, Constants.FIELD_FAVORITE_PRODUCT_ID), "")) ? "display_alert_check_for_mailsend()" : "" %>>
		<%# (FavoriteDisplayWord((string) GetKeyValue(this.ProductMaster, Constants.FIELD_FAVORITE_SHOP_ID), this.LoginUserId, (string) GetKeyValue(this.ProductMaster, Constants.FIELD_FAVORITE_PRODUCT_ID), "")) ? "お気に入り登録済み" : "お気に入りに追加" %>
			<% if(HasVariation){ %>&nbsp;(<%# SetFavoriteDataOfProductForDisplay() %>人)<% } %>
		</asp:LinkButton><% }else{ %>
		<asp:LinkButton ID="lbAddFavorite" runat="server" OnClick="lbAddFavorite_Click">お気に入りに追加</asp:LinkButton>	<% } %>
	</li>
</ul>

<div class="description">

	<!-- キャッチコピー -->
	<h3><%# WebSanitizer.HtmlEncode(GetProductData("catchcopy")) %></h3>

	<!-- 販売期間 -->
	<%if (this.DisplaySell) {%>
	<p>販売期間：<%#: DateTimeUtility.ToStringFromRegion(GetProductData("sell_from"), DateTimeUtility.FormatType.LongDateHourMinute1Letter) %>～<%#: DateTimeUtility.ToStringFromRegion(GetProductData("sell_to"), DateTimeUtility.FormatType.LongDateHourMinute1Letter) %></p>
	<%}%>

	<!-- ホームページリンク -->
	<div visible='<%# StringUtility.ToEmpty(GetProductData("url")) != "" %>' runat="server">
	<a href="<%# WebSanitizer.HtmlEncode(GetProductData("url")) %>">メーカのホームページへ</a>
	</div>

	<!-- 問い合わせメールリンク -->
	<div visible='<%# StringUtility.ToEmpty(GetProductData("inquire_email")) != "" %>' runat="server">
	<a href="mailto:<%# WebSanitizer.HtmlEncode(GetProductData("inquire_email")) %>">商品のお問合せ</a>
	</div>

	<!-- 電話問い合わせ -->
	<div visible='<%# StringUtility.ToEmpty(GetProductData("inquire_tel")) != "" %>' runat="server">
	お問合せ：<%# WebSanitizer.HtmlEncode(GetProductData("inquire_tel")) %></div>

</div>

</div><!-- detailImage -->

<div id="detailOne">

<!-- 商品アイコン -->
<p class="icon">
<w2c:ProductIcon IconNo="1" ProductMaster="<%# this.ProductMaster %>" runat="server" />
<w2c:ProductIcon IconNo="2" ProductMaster="<%# this.ProductMaster %>" runat="server" />
<w2c:ProductIcon IconNo="3" ProductMaster="<%# this.ProductMaster %>" runat="server" />
<w2c:ProductIcon IconNo="4" ProductMaster="<%# this.ProductMaster %>" runat="server" />
<w2c:ProductIcon IconNo="5" ProductMaster="<%# this.ProductMaster %>" runat="server" />
<w2c:ProductIcon IconNo="6" ProductMaster="<%# this.ProductMaster %>" runat="server" />
<w2c:ProductIcon IconNo="7" ProductMaster="<%# this.ProductMaster %>" runat="server" />
<w2c:ProductIcon IconNo="8" ProductMaster="<%# this.ProductMaster %>" runat="server" />
<w2c:ProductIcon IconNo="9" ProductMaster="<%# this.ProductMaster %>" runat="server" />
<w2c:ProductIcon IconNo="10" ProductMaster="<%# this.ProductMaster %>" runat="server" />
</p>

<%-- ↓バリエーション変更時の表示更新領域を指定しています --%>
<div class="ChangesByVariation" runat="server">
	<!-- 商品名 -->
	<h2><%# WebSanitizer.HtmlEncode(GetProductData("name")) %></h2>
	<div id="dvProductSubInfo" class="clearFix">
		<!-- 商品ID  -->
		<p class="productDetailId">&nbsp;[<span class="productId"><%# WebSanitizer.HtmlEncode(GetProductData("variation_id")) %>]</span></p>
</div>

	<div class="wrapProductPrice">
	<!-- 商品価格・税区分・加算ポイント -->
	<%-- ▽商品会員ランク価格有効▽ --%>
	<div visible='<%# GetProductMemberRankPriceValid(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected)) %>' runat="server">
		<p class="productPrice">販売価格:<span><strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected))) %></strike></span>(<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(this.ProductMaster)) %>)</p>
		<p class="productPrice">会員ランク価格:<span><%#: CurrencyManager.ToPrice(ProductPage.GetProductMemberRankPrice(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected))) %></span>(<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(this.ProductMaster)) %>)</p>
	</div>
	<%-- △商品会員ランク価格有効△ --%>
	<%-- ▽商品セール価格有効▽ --%>
	<div visible='<%# GetProductTimeSalesValid(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected)) %>' runat="server">
		<p class="productPrice">販売価格:<span><strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected))) %></strike></span>(<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(this.ProductMaster)) %>)</p>
		<p class="productPrice">タイムセールス価格:<span><%#: CurrencyManager.ToPrice(ProductPage.GetProductTimeSalePriceNumeric(this.ProductMaster)) %></span>(<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(this.ProductMaster)) %>)</p>
	</div>
	<%-- △商品セール価格有効△ --%>
	<%-- ▽商品特別価格有効▽ --%>
	<div visible='<%# GetProductSpecialPriceValid(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected)) %>' runat="server">
		<p class="productPrice">販売価格:<span><strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected))) %></strike></span>(<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(this.ProductMaster)) %>)</p>
		<p class="productPrice">特別価格:<span><%#: CurrencyManager.ToPrice(ProductPage.GetProductSpecialPriceNumeric(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected))) %></span>(<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(this.ProductMaster)) %>)</p>
	</div>
	<%-- △商品特別価格有効△ --%>
	<%-- ▽商品通常価格有効▽ --%>
	<div visible='<%# GetProductNormalPriceValid(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected)) %>' runat="server">
		<p class="productPrice">販売価格:<span><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected))) %></span>(<%# WebSanitizer.HtmlEncode(GetTaxIncludeString(this.ProductMaster)) %>)</p>
	</div>
	<%-- △商品通常価格有効△ --%>
	<%-- ▽商品加算ポイント▽ --%>
		<p visible='<%# (this.IsLoggedIn && (GetProductAddPointString(this.ProductMaster, this.HasVariation, this.VariationSelected) != "")) %>' runat="server">
			<span class="productPoint">ポイント<%# WebSanitizer.HtmlEncode(GetProductAddPointString(this.ProductMaster, this.HasVariation, this.VariationSelected)) %></span>
				<span class="productPoint" visible='<%# (this.IsLoggedIn && ((string)GetKeyValue(this.ProductMaster, Constants.FIELD_PRODUCT_POINT_KBN1)) != Constants.FLG_PRODUCT_POINT_KBN1_NUM) %>' runat="server">(<%# WebSanitizer.HtmlEncode(GetProductAddPointCalculateAfterString(this.ProductMaster, this.HasVariation, this.VariationSelected))%>)
			</span>
		</p>
	<%-- △商品加算ポイント△ --%>
	<%-- ▽商品定期購入価格▽ --%>
	<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) {%>
	<div visible='<%# (GetKeyValue(this.ProductMaster, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG).ToString() != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) && (this.IsUserFixedPurchaseAble) %>' runat="server">
		<span visible='<%# IsProductFixedPurchaseFirsttimePriceValid(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected)) %>' runat="server">
			<p class="productPrice">定期初回価格:<span><%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchaseFirsttimePrice(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected))) %></span>(<%#: GetTaxIncludeString(this.ProductMaster) %>)</p>
		</span>
		<p class="productPrice">定期通常価格:<span><%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchasePrice(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected))) %></span>(<%#: GetTaxIncludeString(this.ProductMaster) %>）</p>
	</div>
	<% } %>
	<%-- △商品定期購入価格△ --%>
	<%-- ▽定期商品加算ポイント▽ --%>
	<p visible='<%# (this.IsLoggedIn && (this.CanFixedPurchase) && (this.IsUserFixedPurchaseAble) && (GetProductAddPointString(this.ProductMaster, this.HasVariation, this.VariationSelected) != "")) %>' runat="server">
		<span class="productPoint">ポイント<%# WebSanitizer.HtmlEncode(GetProductAddPointString(this.ProductMaster, this.HasVariation, this.VariationSelected, true)) %></span><span class="productPoint" visible='<%# (this.IsLoggedIn && ((string)GetKeyValue(this.ProductMaster, Constants.FIELD_PRODUCT_POINT_KBN2)) != Constants.FLG_PRODUCT_POINT_KBN1_NUM) %>' runat="server">(<%# WebSanitizer.HtmlEncode(GetProductAddPointCalculateAfterString(this.ProductMaster, this.HasVariation, this.VariationSelected, true))%>)
	</span>
	</p>
	<%-- △定期商品加算ポイント△ --%>
	</div>

</div>
<%-- ↑バリエーション変更時の表示更新領域を指定しています --%>

<%-- SNSボタン ※mixiチェックはクライアント毎にデベロッパ登録したキーを設定する必要あり --%>
<ul class="snsList clearFix">
	<li><iframe src="//www.facebook.com/plugins/like.php?href=<%# HttpUtility.UrlEncode(this.UnsecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_PRODUCT_DETAIL + "?" + Constants.REQUEST_KEY_PRODUCT_ID + "=" + this.ProductId) %><%# HttpUtility.UrlEncode(Constants.PRODUCT_BRAND_ENABLED ? "&" + Constants.REQUEST_KEY_BRAND_ID + "=" + this.BrandId : "") %>&amp;send=false&amp;layout=button_count&amp;width=450&amp;show_faces=false&amp;action=like&amp;colorscheme=light&amp;font=tahoma&amp;height=21" scrolling="no" frameborder="0" style="border:none; overflow:hidden; width:100px; height:21px;" allowTransparency="true"></iframe></li>
	<li><a href="javascript:void(0);" onclick='<%# WebSanitizer.HtmlEncode("window.open('http://mixi.jp/share.pl?u=" + HttpUtility.UrlEncode(this.UnsecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_PRODUCT_DETAIL + "?" + Constants.REQUEST_KEY_PRODUCT_ID + "=" + this.ProductId + (Constants.PRODUCT_BRAND_ENABLED ? "&" + Constants.REQUEST_KEY_BRAND_ID + "=" + this.BrandId : "") + "&k=01ac61d95d41a50ea61d0c5ab84adf0cfbf62f7d") + "','share',['width=632','height=456','location=yes','resizable=yes','toolbar=no','menubar=no','scrollbars=no','status=no'].join(','));") %>'><img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/mixi_bt_check_1.png" alt="mixiチェック" border="0" /></a></li>
	<li><a href="https://twitter.com/share" class="twitter-share-button" data-count="none" data-lang="ja">ツイート</a><script type="text/javascript" src="https://platform.twitter.com/widgets.js"></script></li>
</ul>

<div class="wrapDetailImage">
<%-- ▽メイン画像▽ --%>
<div class="unit">
	<div class="mainImage">
		<p class="title">メイン画像</p>
		<a href="javascript:void(0);">
			<img class="zoomTarget" src="<%# WebSanitizer.HtmlEncode(CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)(Constants.PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO + 1))) %>" data-image="<%# WebSanitizer.HtmlEncode(CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)(Constants.PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO + 1))) %>" data-zoom-image="<%# WebSanitizer.HtmlEncode(CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)(Constants.PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO + 1))) %>" />
		</a>
	</div>
</div>
<%-- △メイン画像△ --%>
<!-- バリエーション画像一覧 -->
<%-- ▽バリエーション画像一覧▽ --%>
<asp:Repeater ID="rVariation" DataSource='<%# this.ProductVariationMasterList %>' Visible="<%# this.HasVariation %>" runat="server" >
<HeaderTemplate>
		<div class="unit">
		<p class="title">バリエーション</p>
	<ul class="variationImage">
</HeaderTemplate>
<ItemTemplate>
	<li>
		<asp:LinkButton ID="lbVariationPicture" runat="server" OnClick="lbVariaionImages_OnClick" CommandArgument="<%# Eval(Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) %>">
			<w2c:ProductImage ImageTagId="picture" ImageSize="LL" ProductMaster='<%# Container.DataItem %>' IsVariation="true" runat="server" /></asp:LinkButton>
	</li>
</ItemTemplate>
<FooterTemplate>
	</ul>
		</div>
</FooterTemplate>
</asp:Repeater>
<%-- △バリエーション画像一覧△ --%>

	<%-- ▽バリエーション表示名1・2の画像一覧▽ 
<asp:Repeater ID="rVariationImageList" DataSource='<%# this.ProductVariationImageListName2 %>' Visible="<%# this.HasVariation %>" runat="server" >
<HeaderTemplate>
		<div class="unit">
		<p class="title">バリエーション画像</p>
	<ul class="variationImage">
</HeaderTemplate>
<ItemTemplate>
	<li>
			<a href="javascript:void(0);">
			<w2c:ProductImage ImageTagId="picture" ImageSize="LL" ProductMaster='<%# Container.DataItem %>' IsVariation="true" runat="server" />
			</a>
	</li>
</ItemTemplate>
<FooterTemplate>
	</ul>
		</div>
</FooterTemplate>
</asp:Repeater>
	 △バリエーション表示名1・2の画像一覧△ --%>

<!-- サブ画像一覧 -->
<%-- ▽サブ画像一覧▽ --%>
<asp:Repeater DataSource="<%# this.ProductSubImageList %>" Visible="<%# (this.ProductSubImageList.Count != 0) %>" runat="server">
<HeaderTemplate>
		<div class="unit">
		<p class="title">詳細画像</p>
		<ul class="subImage clearFix">
</HeaderTemplate>
<ItemTemplate>
		<li visible='<%# IsSubImagesNoLimit((int)Eval(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO)) %>' runat="server">
		<a href="javascript:void(0);" title="<%# Eval(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NAME) %>">
		<!--
			--><img class="zoomTarget" src="<%# WebSanitizer.HtmlEncode(CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)Eval(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO))) %>" data-image="<%# WebSanitizer.HtmlEncode(CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)Eval(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO))) %>" data-zoom-image="<%# WebSanitizer.HtmlEncode(CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)Eval(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO))) %>" /></a>
</li>
</ItemTemplate>
<FooterTemplate>
</ul>
</div>
</FooterTemplate>
</asp:Repeater>
<%-- △サブ画像一覧△ --%>

	<!--
	<div class="btnDetailpopUp">
	<a href="<%# WebSanitizer.HtmlEncode("javascript:show_popup_window('" + CreateProductSubImagePageUrl() + "', 660, 540, false, false, 'ProductImage')") %>" class="btn btn-mini btn-inverse">詳細画像はこちら</a>
	</div>
	-->

</div>

<div class="ChangesByVariation" runat="server">
<asp:Repeater ID="rSetPromotion" DataSource="<%# this.SetPromotions %>" runat="server">
<ItemTemplate>
<p><%# GetProductDescHtml(((SetPromotionModel)Container.DataItem).DescriptionKbn, ((SetPromotionModel)Container.DataItem).Description) %></p>
</ItemTemplate>
</asp:Repeater>
</div>

<%-- ↓バリエーション変更時の表示更新領域を指定しています --%>
<div class="ChangesByVariation" runat="server">
<div class="productSellInfo">

<!-- バリエーション選択 -->
<div class="selectValiation">
<% if(this.HasVariation) {%>
<% if ((this.SelectVariationKbn == Constants.SelectVariationKbn.PANEL)
		|| (this.IsVariationName3 && ((this.SelectVariationKbn == Constants.SelectVariationKbn.DOUBLEDROPDOWNLIST)
			|| (this.SelectVariationKbn == Constants.SelectVariationKbn.MATRIX)
			|| (this.SelectVariationKbn == Constants.SelectVariationKbn.MATRIXANDMESSAGE)))){ %>
	<asp:HiddenField ID="hIsSelectingVariationExist" Value="<%# this.IsSelectingVariationExist %>" runat="server" />
	<asp:Repeater ID="rVariationName1List" DataSource="<%# this.ProductVariationName1List %>" runat="server">
		<HeaderTemplate>
			<div style="width:100%; padding-bottom:30px; clear:both">
				<div style="width:100%">
					<span>Color</span><br />
				</div>
				<div style="width:100%">
					<div style="padding-left:10px; width:10%; float:left">&nbsp;</div>
					<div style="float:left; width:100%">
		</HeaderTemplate>
		<ItemTemplate>
			<div style="padding-left: 14%">
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
			<br />
			<div style="width:100%; padding-bottom:30px; clear:both">
				<hr /><br />
				<div style="width:100%">
					<span>Size</span><br />
				</div>
				<div style="width:100%">
					<div style="padding-left:10px; width:10%; float:left">&nbsp;</div>
					<div style="float:left; width:100%">
		</HeaderTemplate>
		<ItemTemplate>
			<div style="padding-left: 14%">
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
			<br />
			<div style="width:100%; padding-bottom:30px; clear:both">
				<hr /><br />
				<div style="width:100%">
					<span>Type</span><br />
				</div>
				<div style="width:100%">
					<div style="padding-left:10px; width:10%; float:left">&nbsp;</div>
					<div style="float:left; width:100%">
		</HeaderTemplate>
		<ItemTemplate>
			<div style="padding-left: 14%">
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
	<asp:DropDownList ID="ddlVariationSelect" DataSource='<%# this.ProductValirationListItemCollection %>' DataTextField="Text" DataValueField="Value" SelectedValue='<%# (this.HasVariation && this.VariationSelected && ((this.SelectVariationKbn == Constants.SelectVariationKbn.STANDARD) || (this.SelectVariationKbn == Constants.SelectVariationKbn.DROPDOWNLIST))) ? this.VariationId : null %>' Visible="<%# this.HasVariation %>" OnSelectedIndexChanged="ddlVariationId_SelectedIndexChanged" AutoPostBack="True" runat="server"></asp:DropDownList>
<%} else if (this.SelectVariationKbn == Constants.SelectVariationKbn.DOUBLEDROPDOWNLIST) { %>
		<div  style="color: red; font-size: 12px; margin-bottom: 10px"><asp:Literal ID="lCombinationErrorMessage" runat="server"/></div>
		<asp:DropDownList ID="ddlVariationSelect1" DataSource='<%# this.ProductValirationListItemCollection %>' DataTextField="Text" DataValueField="Value" SelectedValue='<%# (this.HasVariation && (this.SelectedVariationName1 != "") && (this.SelectVariationKbn == Constants.SelectVariationKbn.DOUBLEDROPDOWNLIST)) ? this.SelectedVariationName1 : null %>' Visible="<%# this.HasVariation %>" OnSelectedIndexChanged="ddlVariationId_SelectedIndexChanged" AutoPostBack="True" runat="server"></asp:DropDownList>
	<asp:DropDownList ID="ddlVariationSelect2" DataSource='<%# this.ProductValirationListItemCollection2 %>' DataTextField="Text" DataValueField="Value" SelectedValue='<%# (this.HasVariation && (this.SelectVariationKbn == Constants.SelectVariationKbn.DOUBLEDROPDOWNLIST)) ? this.SelectedVariationName2 : null %>' Visible="<%# this.HasVariation %>" OnSelectedIndexChanged="ddlVariationId_SelectedIndexChanged" AutoPostBack="True" runat="server"></asp:DropDownList>
<%} else if (this.SelectVariationKbn == Constants.SelectVariationKbn.MATRIX || (this.SelectVariationKbn == Constants.SelectVariationKbn.MATRIXANDMESSAGE)) { %>
	<!--1軸バリエーション-->
	<% if (this.IsPluralVariation == false) { %>
		<table cellspacing="0" border="1">
			<asp:Repeater ID="rVariationSingleList" runat="server">
			<ItemTemplate>
			<tr>
				<td class="selectValiationMatrix">
					<span>&nbsp;<%# WebSanitizer.HtmlEncode(CreateVariationName(Container.DataItem, "", "", Constants.CONST_PRODUCTVARIATIONNAME_PUNCTUATION)) %>&nbsp;</span>
				</td>
				<td align="center" valign="middle">
					<asp:HiddenField ID="hfVariationId" Value='<%# Eval(Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) %>' runat="server" />
					<w2c:RadioButtonGroup ID="rbgVariationId" Checked="<%# (this.VariationId == (string)Eval(Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>" GroupName="Variation" OnCheckedChanged="ddlVariationId_SelectedIndexChanged" AutoPostBack="true" CssClass="radioBtn" runat="server" />
				</td>
			</tr>
			</ItemTemplate>
			</asp:Repeater>
		</table>
	<%} else { %>
	<!--2軸バリエーション-->
		<%--★ 下記aspxファイル上のデータソースやパラメータの値を入れ替えることで縦軸横軸の表示項目を切り替えることが可能です。（例：1→2、2→1に置き換える） ★--%>
		<table cellspacing="0" border="1">
			<%--▽ バリエーションヘッダ ▽--%>
			<asp:Repeater DataSource="<%# this.VariationName2List %>" runat="server">
				<HeaderTemplate>
					<tr><th class="selectValiationMatrix">&nbsp;</th>
				</HeaderTemplate>
				<ItemTemplate>
					<th class="selectValiationMatrix"><span>&nbsp;<%# Container.DataItem %>&nbsp;</span></th>
				</ItemTemplate>
				<FooterTemplate>
					</tr>
				</FooterTemplate>
			</asp:Repeater>
			<%--△ バリエーションヘッダ △--%>
			<%--▽ バリエーションデータ ▽--%>
			<asp:Repeater ID="rVariationMatrixY" DataSource="<%# this.VariationName1List %>" runat="server">
			<ItemTemplate>
				<tr>
				<asp:Repeater ID="rVariationMatrixX" DataSource="<%# this.VariationName2List %>" runat="server">
				<ItemTemplate>
					<th valign="middle" class="selectValiationMatrix" style='<%# (Container.ItemIndex % this.VariationName2List.Count == 0) ? "" : "display:none" %>'>
						<span>&nbsp;<%# ((RepeaterItem)Container.Parent.Parent).DataItem %>&nbsp;</span>
					</th>
					<td align="center" valign="middle">
						<span visible='<%# GetVariationIdForMatrix(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1, ((RepeaterItem)Container.Parent.Parent).DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2, Container.DataItem) != "" %>' runat="server">				
							<asp:HiddenField ID="hfVariationId" Value='<%# GetVariationIdForMatrix(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1, ((RepeaterItem)Container.Parent.Parent).DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2, Container.DataItem) %>' runat="server" />
							<w2c:RadioButtonGroup ID="rbgVariationId" Checked='<%# ((this.VariationId != "") && (this.VariationId == GetVariationIdForMatrix(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1, ((RepeaterItem)Container.Parent.Parent).DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2, Container.DataItem))) %>' GroupName="Variation" OnCheckedChanged="ddlVariationId_SelectedIndexChanged" AutoPostBack="true" CssClass="radioBtn" runat="server" />
							<% if (this.SelectVariationKbn == Constants.SelectVariationKbn.MATRIXANDMESSAGE) { %>
							<%# WebSanitizer.HtmlEncode(GetStockMessageForMatrix(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1, ((RepeaterItem)Container.Parent.Parent).DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2, Container.DataItem)) %>
							<%} %>
						</span>
						<%--▽ バリエーションが存在しない場合（規定は空欄） ▽--%>
						<span visible='<%# GetVariationIdForMatrix(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1, ((RepeaterItem)Container.Parent.Parent).DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2, Container.DataItem) == "" %>' runat="server">&nbsp;</span>
						<%--△ バリエーションが存在しない場合（規定は空欄） △--%>
					</td>
				</ItemTemplate>
				</asp:Repeater>
				</tr>
			</ItemTemplate>
			</asp:Repeater>
			<%--△ バリエーションデータ △--%>
		</table>
	<%} %>
<%} %>
<%} %>
</div>

<%-- ▽商品付帯情報▽ --%>
<div>
<asp:Repeater ID="rProductOptionValueSettings" DataSource='<%# this.ProductOptionSettingList %>' runat="server">
<ItemTemplate>
<asp:Label ID="lblProductOptionValueSetting" runat="server" Text="<%# WebSanitizer.HtmlEncode(((ProductOptionSetting)Container.DataItem).ValueName) %>" />
	<span class="necessary" runat="server" style="color: red" visible="<%# ((ProductOptionSetting)Container.DataItem).IsNecessary %>">*</span>
	<asp:Repeater ID="rCblProductOptionValueSetting" DataSource='<%# ((ProductOptionSetting)Container.DataItem).SettingValuesListItemCollection %>' ItemType="System.Web.UI.WebControls.ListItem" Visible='<%# (((ProductOptionSetting)Container.DataItem).DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_CHECKBOX) || (((ProductOptionSetting)Container.DataItem).DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_PRICE_CHECKBOX) %>' runat="server" >
		<HeaderTemplate><div class="field-boxes"></HeaderTemplate>
		<ItemTemplate>
			<span style="padding-right: 5px; padding-bottom: 5px;">
			<asp:CheckBox ID="cbProductOptionValueSetting" Text='<%# Item.Text %>' Checked='<%# Item.Selected %>' runat="server" />
			</span>
		</ItemTemplate>
		<FooterTemplate></div></FooterTemplate>
	</asp:Repeater>
	<asp:Label ID="lblProductOptionCheckboxErrorMessage" runat="server" CssClass="error_inline"/>
	<asp:DropDownList ID="ddlProductOptionValueSetting" DataSource='<%# InsertDefaultAtFirstToDdlProductOptionSettingList(((ProductOptionSetting)Container.DataItem).SettingValuesListItemCollection, ((ProductOptionSetting)Container.DataItem).IsNecessary) %>'  visible='<%# (((ProductOptionSetting)Container.DataItem).DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_SELECTMENU) || (((ProductOptionSetting)Container.DataItem).DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_PRICE_DROPDOWNMENU) %>' SelectedValue='<%# ((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectedValue() %>' runat="server" Width="130" />
	<asp:Label ID="lblProductOptionDropdownErrorMessage" runat="server" CssClass="error_inline"/>
	<asp:TextBox ID ="txtProductOptionValueSetting" Text = '<%# ((ProductOptionSetting)Container.DataItem).DefaultValue%>' visible='<%# ((ProductOptionSetting)Container.DataItem).IsTextBox %>' runat="server" />
<br />
<asp:Label ID="lblProductOptionErrorMessage" runat="server" CssClass="error_inline"/>
<br />
</ItemTemplate>
</asp:Repeater>
</div>
<%-- △商品付帯情報△ --%>

<!-- 注文数量指定 -->
<div class="productAmount" style='<%# (this.IsSelectingVariationExist == false) ? "display:none" : "" %>' runat="server">
注文数：<asp:TextBox ID="tbCartAddProductCount" runat="server" Text="1" MaxLength="3" Width="28px" OnTextChanged="ddlVariationId_SelectedIndexChanged" AutoPostBack="true" style="text-align:center;"></asp:TextBox>
</div>

<div visible="<%# this.Buyable %>" runat="server">

<div class="productCart">

<!-- カート投入リンク -->
<div class="addCart">
	<p class="btnCart">
	<asp:LinkButton ID="lbCartAdd" class="btn btn-mid btn-inverse" runat="server" Visible="<%# (this.CanAddCart) && (this.IsSubscriptionBoxOnly == false) %>" Onclick="lbCartAdd_Click" OnClientClick="return add_cart_check();">
	カートに入れる
</asp:LinkButton>
</p>
	<p class="btnCart">
	<asp:LinkButton ID="lbCartAddFixedPurchase" class="btn btn-mid btn-inverse" runat="server" Visible="<%# (this.CanFixedPurchase) && (this.IsUserFixedPurchaseAble) && (this.IsSubscriptionBoxOnly == false) %>" OnClick="lbCartAddFixedPurchase_Click" OnClientClick="return add_cart_check_for_fixedpurchase();">
	カートに入れる(定期購入)
	</asp:LinkButton>
	<span runat="server" Visible='<%# (this.CanFixedPurchase) && ((string)this.ProductMaster[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG] == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY) && (this.IsUserFixedPurchaseAble == false) %>' style="color: red;">定期購入のご利用はできません</span>
	</p>
	<p class="btnCart">
	<asp:LinkButton ID="lbCartAddSubscriptionBox" class="btn btn-mid btn-inverse" runat="server" Visible="<%# (this.CanFixedPurchase) && (this.IsUserFixedPurchaseAble) && (this.IsSubscriptionBoxValid) %>" OnClick="lbCartAddSubscriptionBox_Click" OnClientClick="return add_cart_check_for_subscriptionBox(this);">
	頒布会申し込み
	</asp:LinkButton>
	<asp:DropDownList ID="ddlSubscriptionBox" DataTextField="DisplayName" DataValueField="CourseID" runat="server" Visible="false"></asp:DropDownList>
	<asp:HiddenField ID="hfSubscriptionBoxDisplayName" runat="server" />
	</p>
	<p class="btnCart">
	<asp:LinkButton ID="lbCartAddForGift" class="btn btn-mid btn-inverse" runat="server" Visible="<%# (this.CanGiftOrder) %>" OnClick="lbCartAddGift_Click" OnClientClick="return add_cart_check();">
	カートに入れる(ギフト購入)
	</asp:LinkButton>
	</p>
</div>
	
<div Visible="<%# this.IsDisplayExcludeFreeShippingText %>" runat="server">
	<p style="color:red;">※本商品は配送料無料適用外です</p>
</div>

<div visible="<%# this.IsDisplayValiationStock %>" runat="server">
	<p>在庫有り</p>
</div>
	

	<!--在庫文言表示-->
<div visible="<%# this.IsStockManaged == false %>" runat="server">
	<p>
		在庫状況 : <%# WebSanitizer.HtmlEncode(w2.App.Common.Order.ProductCommon.CreateProductStockMessage(this.ProductMaster, true)) %>
	</p>
</div>
<%if (this.HasStockMessage) {%>
<%if (this.HasVariation) {%>
<p class="productStock">
	<a href="" onclick='<%# WebSanitizer.HtmlEncode("javascript:show_popup_window('" + CreateProductStockListUrl() + "', 700, 400, true, true, 'ProductStockList'); return false;") %>'>在庫状況</a>
</p>
<%} // (this.HasVariation) %>
<%if (this.HasVariation == false) {%>
<p class="productStock">
在庫状況：<%# WebSanitizer.HtmlEncode(w2.App.Common.Order.ProductCommon.CreateProductStockMessage(this.ProductMaster, true)) %><%} // (this.HasVariation == false) %></p>
<%} // (this.HasStockMessage) %>

<% if(Constants.VARIATION_FAVORITE_CORRESPONDENCE){ %>
<p>お気に入りの総登録人数：<%# SetTotalFavoriteDataForDisplay() %>人</p>
<% }else{ %>	
<p>お気に入りの登録人数：<%# this.FavoriteUserCount %>人</p>
<% } %>
</div>

</div><!-- <%# this.Buyable %> -->

<div visible="<%# (this.IsSelectingVariationExist) %>" runat="server">
<p class="error"><%# WebSanitizer.HtmlEncode(this.AlertMessage) %></p>
<div class="error"><%: this.ErrorMessageFixedPurchaseMember %></div>
<p class="error"><%# WebSanitizer.HtmlEncode(this.LimitedPaymentMessages) %></p>
</div>
<!--再入荷通知メール申し込みボタン表示-->
<div visible="<%# this.ArrivalMailKbn == Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL %>" runat="server">
<asp:LinkButton ID="lbRequestArrivalMail2" Runat="server" OnCommand="ViewRegsiterArrivalMailForm_Command" CommandArgument="Arrival" class="btn btn-mid btn-inverse">
入荷お知らせメール申込
</asp:LinkButton>
<p>
<span visible="<%# IsArrivalMailAnyRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL) %>" runat="server"><br />通知登録済み!!</span>
<span visible="<%# IsArrivalMailPcRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL) %>" runat="server"> [PC]</span>
<span visible="<%# IsArrivalMailMobileRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL) %>" runat="server"> [モバイル]</span>
<span visible="<%# IsArrivalMailGuestRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL) %>" runat="server"> [その他]</span>
</p>
<%-- 再入荷通知メール登録フォーム表示 --%>
<uc:BodyProductArrivalMailRegister runat="server" ID="ucBpamrArrival" ArrivalMailKbn="<%#: Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL %>" ProductId="<%#: this.ProductId %>" HasVariation="<%# this.HasVariation %>" Visible="false" />
</div><!-- <%# this.ArrivalMailKbn == Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL %> -->

<!--販売開始通知メール申し込みボタン表示-->
<div visible="<%# this.ArrivalMailKbn == Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE %>" runat="server">
<asp:LinkButton ID="lbRequestReleaseMail2" Runat="server" OnCommand="ViewRegsiterArrivalMailForm_Command" CommandArgument="Release" class="btn btn-mid btn-inverse">
販売開始通知メール申込
</asp:LinkButton>
<p>
<span visible="<%# IsArrivalMailAnyRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE) %>" runat="server"><br />通知登録済み!!</span>
<span visible="<%# IsArrivalMailPcRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE) %>" runat="server"> [PC]</span>
<span visible="<%# IsArrivalMailMobileRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE) %>" runat="server"> [モバイル]</span>
<span visible="<%# IsArrivalMailGuestRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE) %>" runat="server"> [その他]</span>
</p>
<%--販売開始通知メール登録フォーム表示 --%>
<uc:BodyProductArrivalMailRegister runat="server" ID="ucBpamrRelease" ArrivalMailKbn="<%#: Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE %>" ProductId="<%#: this.ProductId %>" HasVariation="<%# this.HasVariation %>" Visible="false" />
</div><!-- <%# this.ArrivalMailKbn == Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE %> -->

<!-- 再販売通知メール申し込みボタン表示 -->
<div visible="<%# this.ArrivalMailKbn == Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE %>" runat="server">
<asp:LinkButton ID="lbRequestResaleMail2" Runat="server" OnCommand="ViewRegsiterArrivalMailForm_Command" CommandArgument="Resale" class="btn btn-mid btn-inverse">
再販売通知メール申込
</asp:LinkButton>
<p>
<span visible="<%# IsArrivalMailAnyRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE) %>" runat="server"><br />通知登録済み!!</span>
<span visible="<%# IsArrivalMailPcRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE) %>" runat="server"> [PC]</span>
<span visible="<%# IsArrivalMailMobileRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE) %>" runat="server"> [モバイル]</span>
<span visible="<%# IsArrivalMailGuestRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE) %>" runat="server"> [その他]</span>
</p>
<%--再販売通知メール登録フォーム表示 --%>
<uc:BodyProductArrivalMailRegister runat="server" ID="ucBpamrResale" ArrivalMailKbn="<%#: Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE %>" ProductId="<%#: this.ProductId %>" HasVariation="<%# this.HasVariation %>" Visible="false" />
</div>
<div visible="<%# (this.IsSelectingVariationExist == false) %>" runat="server">
	<p class="error"><%#: this.AlertMessageVariationNotExist %></p>
</div>
</div><!-- <%# this.ArrivalMailKbn == Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE %> -->
<% if (StringUtility.ToEmpty(this.ErrorMessage) != "") {%>
<br /><span class="error_inline"><%: this.ErrorMessage %></span>
<%} %>

<%-- リアル店舗在庫一覧 --%>
<% if (Constants.REALSHOP_OPTION_ENABLED){ %>
<p class="productStock" style="padding-bottom: 10px;">
	<a class="real_stock_button" href="" onclick='<%# WebSanitizer.HtmlEncode("javascript:show_popup_window('" + CreateRealShopProductStockListUrl(this.ProductId, (string)this.ProductMaster[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]) + "', 630, 900, true, true, 'ProductRealShopStockList'); return false;") %>'>
	リアル店舗在庫状況
	</a>
</p>
<%} %>

</div><!-- productSellInfo -->
</div>
<%-- ↑バリエーション変更時の表示更新領域を指定しています --%>

<div>
<p id="addFavoriteTip" class="toolTip" style="display: none;">
	<span style="margin: 10px;" id="txt-tooltip"></span>
	<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_FAVORITE_LIST) %>" class="btn btn-mini btn-inverse">お気に入り一覧</a>
</p>
</div>

<%-- ▽バリエーション毎のカート投入ボタン表示▽ --%>
<div id="divMultiVariation">
<table>
	<tr>
		<th>表示名1</th>
		<th>表示名2</th>
		<th>表示名3</th>
		<th Visible='<%# this.ProductVariationAddCartList.Any(item => (string)GetKeyValue(item, Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN) != "0") %>' runat="server">在庫状況</th>
		<th>&nbsp;</th>
	</tr>
	<asp:Repeater ID="rAddCartVariationList" DataSource="<%# this.ProductVariationAddCartList %>" 
				onitemcommand="rAddCartVariationList_ItemCommand" runat="server"
				OnItemDataBound="rAddCartVariationList_ItemDataBound">
		<HeaderTemplate>
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
			<td><%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1)) %></td>
			<td><%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2)) %></td>
			<td><%#: GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3) %></td>
			<td Visible='<%# (string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN) != "0" %>' runat="server">
				<div visible='<%# ((string)GetKeyValue(Container.DataItem, "StockMessage") == "") %>' runat="server">
					在庫数量：<strong><%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTSTOCK_STOCK)) %></strong>
				</div>
				<div visible='<%# ((string)GetKeyValue(Container.DataItem, "StockMessage") != "") %>' runat="server">
					<%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, "StockMessage")) %>
				</div>
			</td>
			<td>

			<div class="addCart">
				<asp:PlaceHolder ID="plhNotSubscriptionBoxOnly" runat="server">
					<p class="btnCart">
					<asp:LinkButton ID="lbCartAddVariationList" runat="server" Visible='<%# (bool)GetKeyValue(Container.DataItem, "CanCart") %>' CommandName="CartAdd" class="btn btn-mid btn-inverse" OnClientClick="return add_cart_check_for_variationlist();">
					カートに入れる
					</asp:LinkButton>
					</p>
					<p class="btnCart">
			<asp:LinkButton ID="lbCartAddFixedPurchaseVariationList" runat="server" Visible='<%# (((bool)GetKeyValue(Container.DataItem, "CanFixedPurchase")) && this.IsUserFixedPurchaseAble) %>' OnClientClick="return add_cart_check_for_fixedpurchase_variationlist();" CommandName="CartAddFixedPurchase" class="btn btn-mid btn-inverse">
					カートに入れる(定期購入)
					</asp:LinkButton>
					<span runat="server" Visible='<%# ((bool)GetKeyValue(Container.DataItem, "CanFixedPurchase")) && ((string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG) == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY) && (this.IsUserFixedPurchaseAble == false) %>' style="color: red;">定期購入のご利用はできません</span>
					</p>
				</asp:PlaceHolder>
				<p class="btnCart">
				<asp:LinkButton ID="lbCartAddSubscriptionBoxList" runat="server" OnClientClick="return add_cart_check_for_subscriptionBox_variationlist(this);" CommandName="CartAddSubscriptionBox" class="btn btn-mid btn-inverse" Visible="false">
				頒布会申し込み
				</asp:LinkButton>
				<asp:HiddenField ID="hfVariation" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) %>" runat="server"/>
				<asp:HiddenField ID="htShopId" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_SHOP_ID) %>" runat="server"/>
				<asp:HiddenField ID="hfProduct" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID) %>" runat="server"/>
				<asp:HiddenField ID="hfSubscriptionBoxFlg" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG) %>" runat="server"/>
				<asp:HiddenField ID="hfCanFixedPurchase" Value='<%# (bool)GetKeyValue(Container.DataItem, "CanFixedPurchase") %>' runat="server"/>
				<asp:HiddenField ID="hfFixedPurchase" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG) %>" runat="server"/>
				<asp:HiddenField ID="hfFixedPurchasePrice" Value="<%# CurrencyManager.ToPrice(GetProductFixedPurchasePrice(Container.DataItem)) %>" runat="server"/>
				<asp:DropDownList ID="ddlSubscriptionBox" DataTextField="DisplayName" DataValueField="CourseID" runat="server" Visible="false" AutoPostBack="True"></asp:DropDownList>
				<asp:HiddenField ID="hfSubscriptionBoxDisplayName"  runat="server" />
				</p>
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
				<p class="btnCart">
					<asp:LinkButton ID="lbCartAddForGiftVariationList" runat="server" Visible='<%# (bool)GetKeyValue(Container.DataItem, "CanGiftOrder") %>' CommandName="CartAddGift" class="btn btn-mid btn-inverse">
						カートに入れる(ギフト購入)
					</asp:LinkButton>
				</p>
		<% if(Constants.VARIATION_FAVORITE_CORRESPONDENCE){ %>
				<p class="btnCart">
			<asp:LinkButton ID="favoriteVariation" runat="server" CommandName="AddFavorite" class="btn btn-mid" OnClientClick=<%# (Alertdisplaycheck(this.ShopId, this.LoginUserId, (string)GetKeyValue(Container.DataItem, Constants.FIELD_FAVORITE_PRODUCT_ID), (string)GetKeyValue(Container.DataItem, Constants.FIELD_FAVORITE_VARIATION_ID))) ? "display_alert_check_for_mailsend()" : "" %>>
				<%# (FavoriteDisplayWord(this.ShopId, this.LoginUserId, (string)GetKeyValue(Container.DataItem, Constants.FIELD_FAVORITE_PRODUCT_ID), (string)GetKeyValue(Container.DataItem, Constants.FIELD_FAVORITE_VARIATION_ID))) ? "お気に入り登録済み" : "お気に入りに追加" %>
				</asp:LinkButton>
			(<%# SetFavoriteDataForDisplay((string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID), (string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>人)
			</p>
		<% } %>
			<div>
			<asp:Repeater ID="rSetPromotionVariationList" DataSource='<%# GetKeyValue(Container.DataItem, "SetPromotionList") %>' runat="server">
			<ItemTemplate>
				<%# WebSanitizer.HtmlEncode(((SetPromotionModel)Container.DataItem).SetpromotionDispName) %><br />
			</ItemTemplate>
			</asp:Repeater>
			</div>
			</div>

			<!-- 再入荷通知メール申し込みボタン表示 -->
			<div visible='<%# ((string)GetKeyValue(Container.DataItem, "ArrivalMailKbn") == Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL) %>' runat="server">
			<asp:LinkButton CommandName="SmartArrivalMail" CommandArgument="Arrival" Runat="server" class="btn btn-mid btn-inverse">
			入荷お知らせメール申込
			</asp:LinkButton>
			<p>
			<span visible="<%# IsArrivalMailAnyRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL, (string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>" runat="server"><br />通知登録済み!!</span>
			<span visible="<%# IsArrivalMailPcRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL, (string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>" runat="server"> [PC]</span>
			<span visible="<%# IsArrivalMailMobileRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL, (string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>" runat="server"> [モバイル]</span>
			<span visible="<%# IsArrivalMailGuestRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL, (string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>" runat="server"> [その他]</span>
			</p>
			</div>

			<!-- 販売開始通知メール申し込みボタン表示 -->
			<div visible='<%# ((string)GetKeyValue(Container.DataItem, "ArrivalMailKbn") == Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE) %>' runat="server">
			<asp:LinkButton CommandName="SmartArrivalMail" CommandArgument="Release" Runat="server"  class="btn btn-mid btn-inverse">
			販売開始通知メール申込
			</asp:LinkButton>
			<p>
			<span visible="<%# IsArrivalMailAnyRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE, (string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>" runat="server"><br />通知登録済み!!</span>
			<span visible="<%# IsArrivalMailPcRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE, (string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>" runat="server"> [PC]</span>
			<span visible="<%# IsArrivalMailMobileRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE, (string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>" runat="server"> [モバイル]</span>
			<span visible="<%# IsArrivalMailGuestRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE, (string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>" runat="server"> [その他]</span>
			</p>
			</div>

			<!-- 再販売通知メール申し込みボタン表示 -->
			<div visible='<%# ((string)GetKeyValue(Container.DataItem, "ArrivalMailKbn") == Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE) %>' runat="server">
			<asp:LinkButton CommandName="SmartArrivalMail" CommandArgument="Resale" Runat="server" class="btn btn-mid btn-inverse">
			再販売通知メール申込
			</asp:LinkButton>
			<p>
			<span visible="<%# IsArrivalMailAnyRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE, (string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>" runat="server"><br />通知登録済み!!</span>
			<span visible="<%# IsArrivalMailPcRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE, (string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>" runat="server"> [PC]</span>
			<span visible="<%# IsArrivalMailMobileRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE, (string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>" runat="server"> [モバイル]</span>
			<span visible="<%# IsArrivalMailGuestRegistered(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE, (string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>" runat="server"> [その他]</span>
			</p>
			</div>

			<p class="error"><%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, "ErrorMessage")) %></p>
			<asp:HiddenField ID="hfVariationId" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) %>" runat="server" />
			<asp:HiddenField ID="htArrivalMailKbn" Value='<%# GetKeyValue(Container.DataItem, "ArrivalMailKbn") %>' runat="server" />
			</td>
			</tr>
			<%-- 再入荷通知メール登録フォーム表示 --%>
			<uc:BodyProductArrivalMailRegisterTr runat="server" ID="ucBpamrArrival" ArrivalMailKbn="<%#: Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL %>" ProductId="<%#: GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_PRODUCT_ID) %>" VariationId="<%#: GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) %>" Visible="false" />
			<%-- 販売開始通知メール登録フォーム表示 --%>
			<uc:BodyProductArrivalMailRegisterTr runat="server" ID="ucBpamrRelease" ArrivalMailKbn="<%#: Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE %>" ProductId="<%#: GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_PRODUCT_ID) %>" VariationId="<%#: GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) %>" Visible="false" />
			<%-- 再販売通知メール登録フォーム表示 --%>
			<uc:BodyProductArrivalMailRegisterTr runat="server" ID="ucBpamrResale" ArrivalMailKbn="<%#: Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE %>" ProductId="<%#: GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_PRODUCT_ID) %>" VaridationId="<%#: GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) %>" Visible="false" />
		</ItemTemplate>
		<FooterTemplate>
		</FooterTemplate>
	</asp:Repeater>
</table>
</div>
<%-- △バリエーション毎のカート投入ボタン表示△ --%>

<!--在庫表表示-->
<div id="dvProductStock">
<uc:BodyProductStockList ShopId="<%# this.ShopId %>" ProductId="<%# this.ProductId %>" Visible="<%# this.ShouldShowStockList %>" runat="server" />
</div>


<%-- ▽商品タグ項目：メーカー▽ --%>
<span visible='<%# StringUtility.ToEmpty(GetProductData("tag_manufacturer")) != "" %>' runat="server">
	<%#: GetProductTagName("tag_manufacturer") %>:<%# WebSanitizer.HtmlEncode(GetProductData("tag_manufacturer")) %><br />
</span>
<%-- △商品タグ項目：メーカー△ --%>
<%-- ▽商品タグ項目：国▽ --%>
<span visible='<%# StringUtility.ToEmpty(GetProductData("tag_country")) != "" %>' runat="server">
	<%#: GetProductTagName("tag_country") %>:<%# WebSanitizer.HtmlEncode(GetProductData("tag_country")) %><br />
</span>
<%-- △商品タグ項目：国△ --%>
<%-- ▽商品タグ項目：年代▽ --%>
<span visible='<%# StringUtility.ToEmpty(GetProductData("tag_year")) != "" %>' runat="server">
	<%#: GetProductTagName("tag_year") %>:<%# WebSanitizer.HtmlEncode(GetProductData("tag_year")) %><br />
</span>
<%-- △商品タグ項目：年代△ --%>

<!-- 商品アップセル一覧 -->
<%-- ▽商品アップセル一覧▽ --%>
<asp:Repeater DataSource=<%# this.ProductUpSellList %> Visible="<%# this.ProductUpSellList.Count != 0 %>" runat="server">
<HeaderTemplate>
<div id="dvUpSell" class="clearFix">
<p class="title">こちらの商品もおすすめ</p>
</HeaderTemplate>
<ItemTemplate>
<div class="productInfoList">
	<ul>
<li class="thumnail">
<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrlUseProductCategory(Container.DataItem, "")) %>">
		<w2c:ProductImage ImageTagId="picture" ImageSize="M" ProductMaster=<%# Container.DataItem %> IsVariation="false" runat="server" /></a>
	<%-- ▽在庫切れ可否▽ --%>
	<span visible='<%# ProductListUtility.IsProductSoldOut(Container.DataItem) %>' runat="server" class="soldout">SOLDOUT</span>
	<%-- △在庫切れ可否△ --%>
	</li>
<li class="productName">
<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrlUseProductCategory(Container.DataItem, "")) %>"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCT_NAME)) %></a><br />
<%-- ▽商品会員ランク価格有効▽ --%>
	<p visible='<%# GetProductMemberRankPriceValid(Container.DataItem) %>' runat="server">
	<strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></strike><br />
<%#: CurrencyManager.ToPrice(ProductPage.GetProductMemberRankPrice(Container.DataItem, false)) %>
	</p>
<%-- △商品会員ランク価格有効△ --%>
<%-- ▽商品セール価格有効▽ --%>
	<p visible='<%# ProductPage.GetProductTimeSalesValid(Container.DataItem) %>' runat="server">
	<strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></strike><br />
<%#: CurrencyManager.ToPrice(ProductPage.GetProductTimeSalePriceNumeric(Container.DataItem)) %>
	</p>
<%-- △商品セール価格有効△ --%>
<%-- ▽商品特別価格有効▽ --%>
	<p visible='<%# ProductPage.GetProductSpecialPriceValid(Container.DataItem) %>' runat="server">
	<strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></strike><br />
<%#: CurrencyManager.ToPrice(ProductPage.GetProductSpecialPriceNumeric(Container.DataItem)) %>
	</p>
<%-- △商品特別価格有効△ --%>
<%-- ▽商品通常価格有効▽ --%>
	<p visible='<%# ProductPage.GetProductNormalPriceValid(Container.DataItem) %>' runat="server">
<%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %>
	</p>
<%-- △商品通常価格有効△ --%>
<%-- ▽定期購入有効▽ --%>
<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) {%>
	<p visible='<%# (GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG).ToString() != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) %>' runat="server">
		<span visible='<%# IsProductFixedPurchaseFirsttimePriceValid(Container.DataItem) && (this.IsUserFixedPurchaseAble) %>' runat="server">
			<p class="productPrice">定期初回価格:<span><%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchaseFirsttimePrice(Container.DataItem)) %></span></p>
		</span>
		<p class="productPrice">定期通常価格:<span><%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchasePrice(Container.DataItem)) %></span></p>
	</p>
<% } %>
<%-- △定期購入有効△ --%>
</li>
</ul>
</div>
</ItemTemplate>
<FooterTemplate>
</div>
</FooterTemplate>
</asp:Repeater>
<%-- △商品アップセル一覧△ --%>

<!-- 商品クロスセル一覧 -->
<%-- ▽商品クロスセル一覧▽ --%>
<asp:Repeater DataSource=<%# this.ProductCrossSellList %> Visible="<%# this.ProductCrossSellList.Count != 0 %>" runat="server">
<HeaderTemplate>
<div id="dvCrossSell" class="clearFix">
<p class="title">関連商品</p>
</HeaderTemplate>
<ItemTemplate>
<div class="productInfoList">
<ul class="clearFix">
<li class="thumnail">
<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrlUseProductCategory(Container.DataItem, "")) %>">
		<w2c:ProductImage ImageTagId="picture" ImageSize="M" ProductMaster=<%# Container.DataItem %> IsVariation="false" runat="server" /></a>
	<%-- ▽在庫切れ可否▽ --%>
	<span visible='<%# ProductListUtility.IsProductSoldOut(Container.DataItem) %>' runat="server" class="soldout">SOLDOUT</span>
	<%-- △在庫切れ可否△ --%>
	</li>
<li class="productName">
<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrlUseProductCategory(Container.DataItem, "")) %>"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCT_NAME)) %></a><br />
<%-- ▽商品会員ランク価格有効▽ --%>
	<p visible='<%# GetProductMemberRankPriceValid(Container.DataItem) %>' runat="server">
	<strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></strike><br />
<%#: CurrencyManager.ToPrice(ProductPage.GetProductMemberRankPrice(Container.DataItem, false)) %>
	</p>
<%-- △商品会員ランク価格有効△ --%>
<%-- ▽商品セール価格有効▽ --%>
	<p visible='<%# ProductPage.GetProductTimeSalesValid(Container.DataItem) %>' runat="server">
	<strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></strike><br />
<%#: CurrencyManager.ToPrice(ProductPage.GetProductTimeSalePriceNumeric(Container.DataItem)) %>
	</p>
<%-- △商品セール価格有効△ --%>
<%-- ▽商品特別価格有効▽ --%>
	<p visible='<%# ProductPage.GetProductSpecialPriceValid(Container.DataItem) %>' runat="server">
	<strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem)) %></strike><br />
<%#: CurrencyManager.ToPrice(ProductPage.GetProductSpecialPriceNumeric(Container.DataItem)) %>
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
</li>
</ul>
</div>
</ItemTemplate>
<FooterTemplate>
</div>
</FooterTemplate>
</asp:Repeater>
<%-- △商品クロスセル一覧△ --%>

<div visible='<%# StringUtility.ToEmpty(GetProductData("return_exchange_message")) != "" %>' runat="server">
	<!-- 返品交換文言表示 -->
	<div class="productSellInfo">
		<strong><%= WebSanitizer.HtmlEncodeChangeToBr(GetProductData("return_exchange_message")) %></strong>
		<%if (ShopMessage.GetMessage("ReturnSpecialContractPage") != "") {%>
			（<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT) %><%= WebSanitizer.HtmlEncode(ShopMessage.GetMessage("ReturnSpecialContractPage")) %>" target="_blank" style='font-size:10px'>返品特約</a>）
		<%} %>
	</div>
</div>

</ContentTemplate>
</asp:UpdatePanel>
<%-- UPDATE PANELここまで --%>
<br />

<div id="dvProductDescription">
	<div visible='<%# this.IsProductOutlineVisible %>' runat="server">
	<p class="title">概要</p><br />
		<p><%# GetProductDataHtml("outline") %></p><br />
	</div>

　　<div visible='<%# this.IsProductDetailVisible %>' runat="server">
	<p class="title">詳細情報</p><br />
	<%-- 商品詳細1 --%>
	<p><%# GetProductDataHtml("desc_detail1") %></p><br />
	<%-- 商品詳細2 --%>
	<p><%# GetProductDataHtml("desc_detail2") %></p><br />
	<%-- 商品詳細3 --%>
	<p><%# GetProductDataHtml("desc_detail3") %></p><br />
	<%-- 商品詳細4 --%>
	<p><%# GetProductDataHtml("desc_detail4") %></p>
	</div>
</div>

<%-- ▽最近チェックした商品▽ --%>
<uc:BodyProductHistory runat="server" />
<%-- △最近チェックした商品△ --%>
	
<!-- 商品レビュー -->
<uc:BodyProductReview Visible="<%# Constants.PRODUCTREVIEW_ENABLED %>" ShopId="<%# this.ShopId %>" ProductId="<%# this.ProductId %>" ProductName="<%# this.ProductName %>" ProductReviewCount="5" runat="server"></uc:BodyProductReview >
	
<!-- 同じ商品のコーディネート -->
<uc:BodyProductCoordinate runat="server"></uc:BodyProductCoordinate>
<uc:BodyRecommendProductsWithTag ShopId="<%# this.ShopId %>" ProductId="<%# this.ProductId %>" ImageSize="M" DisplayKbn="ALL" runat="server" />
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

<script runat="server">
public new void Page_Load(Object sender, EventArgs e)
{
base.Page_Load(sender, e);

var recommendEngineUserControls = WebControlUtility.GetRecommendEngineUserControls(this.Form.FindControl("ContentPlaceHolder1"));
var lProductRecommendByRecommendEngineUserControls = recommendEngineUserControls.Item1;
var lCategoryRecommendByRecommendEngineUserControls = recommendEngineUserControls.Item2;

// <%-- ▽編集可能領域：プロパティ設定▽ --%>
// 外部レコメンド連携パーツ設定
// 1つ目の商品レコメンド
if (lProductRecommendByRecommendEngineUserControls.Count > 0)
{
	// レコメンドコードを設定します
	lProductRecommendByRecommendEngineUserControls[0].RecommendCode = "pc311";
	// レコメンドタイトルを設定します
	lProductRecommendByRecommendEngineUserControls[0].RecommendTitle = "おすすめ商品一覧";
	// 商品最大表示件数を設定します
	lProductRecommendByRecommendEngineUserControls[0].MaxDispCount = 5;
	// レコメンドエンジン連携用の選択中の商品ID
	lProductRecommendByRecommendEngineUserControls[0].RecommendProductId = GetRecommendProductId();
	// レコメンド対象にするカテゴリIDを設定します（複数選択時はカンマ区切りで指定）
	lProductRecommendByRecommendEngineUserControls[0].DispCategoryId = "";
	// レコメンド非対象にするカテゴリIDを設定します（複数選択時はカンマ区切りで指定）
	lProductRecommendByRecommendEngineUserControls[0].NotDispCategoryId = "";
	// レコメンド非対象にするアイテムIDを設定します（複数選択時はカンマ区切りで指定）
	lProductRecommendByRecommendEngineUserControls[0].NotDispRecommendProductId = "";
}
// <%-- △編集可能領域△ --%>
}
</script>

<script type="text/javascript">
<!--
	var strAlertmessage = '<%= MESSAGE_ERROR_VARIATION_UNSELECTED %>';
	var strAlertMessageOption = '<%=MESSAGE_ERROR_OPTION_UNSELECTED%>';
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

	// バリエーション選択チェック(定期)
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
			return confirm(subscriptionBoxMessage.replace('@@ 1 @@', subscriptionBoxName));
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

	// バリエーションリスト用選択チェック(定期)
	function add_cart_check_for_fixedpurchase_variationlist() {
		var alertMessage = get_alert_message_for_product_option_unselected();
		if (alertMessage) {
			alert(alertMessage);
			return false;
		}
		return confirm(fixedpurchaseMessage);
	}

	// バリエーションリスト用選択チェック(頒布会)
	function add_cart_check_for_subscriptionBox_variationlist(value) {
		var subscriptionBoxName = ($(value).parent().find("[id$='ddlSubscriptionBox']").length > 0)
			? $(value).parent().find("[id$='ddlSubscriptionBox'] option:selected")[0].innerText
			: $(value).parent().find("[id$='hfSubscriptionBoxDisplayName']").val();
		return confirm(subscriptionBoxMessage.replace('@@ 1 @@', subscriptionBoxName));
	}

	// 入荷通知登録画面をポップアップウィンドウで開く
	function show_arrival_mail_popup(pid, vid, amkbn) {
		show_popup_window('<%= this.SecurePageProtocolAndHost %><%= Constants.PATH_ROOT %><%= Constants.PAGE_FRONT_USER_PRODUCT_ARRIVAL_MAIL_REGIST %>?<%= Constants.REQUEST_KEY_PRODUCT_ID %>=' + pid + '&<%= Constants.REQUEST_KEY_VARIATION_ID %>=' + vid + '&<%= Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN %>=' + amkbn, 580, 280, false, false, 'Information');
	}

	// マウスイベントの初期化
	addOnload(function () { init(); });
	//-->

	// バリエーションリスト毎のカート投入時チェック(通常商品)
	function add_cart_check_for_variationlist() {
		var alertMessage = get_alert_message_for_product_option_unselected();
		if (alertMessage) {
			alert(alertMessage);
			return false;
		}
		return true;
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

</script>

<%-- CRITEOタグ --%>
<uc:Criteo ID="criteo" runat="server" Datas="<%# null %>" />

</asp:Content>
