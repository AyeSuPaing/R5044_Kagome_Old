<%--/*
=========================================================================================================
  Module      : 商品詳細モーダル画面 (ProductDetailModal.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%@ Register TagPrefix="uc" TagName="BodyProductArrivalMailRegister" Src="~/Form/Common/Product/BodyProductArrivalMailRegister.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductArrivalMailRegisterTr" Src="~/Form/Common/Product/BodyProductArrivalMailRegisterTr.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductStockList" Src="~/Form/Common/Product/BodyProductStockList.ascx" %>

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Product/ProductDetailModal.ascx.cs" Inherits="Form_Common_Product_ProductDetailModal" %>
<asp:HiddenField ID="hfVariationSelectedIndex" Value="0" runat="server" />
<script type="text/javascript" src="<%: Constants.PATH_ROOT + "Js/jquery.elevateZoom-3.0.8.min.js" %>"></script>
<script>
	var strAlertmessage = '<%= MESSAGE_ERROR_VARIATION_UNSELECTED %>';
	var strAlertMessageOption = '<%=MESSAGE_ERROR_OPTION_UNSELECTED%>';
	var fixedpurchaseMessage = '定期的に商品をお送りする「定期購入」で購入します。\nよろしいですか？';
	var subscriptionBoxMessage = '頒布会「@@ 1 @@」の内容確認画面に進みます。\nよろしいですか？';
	var mailSendAlertMessage = 'お気に入り追加商品の在庫減少時にメールを送信いたします。\nマイページ＞登録情報の変更より変更できます。';

	function openModal() {
		const scrollBarWidth = window.innerWidth - document.body.clientWidth;

		// 背景のスクロールを禁止
		document.body.style.overflow = "hidden";
		document.body.style.paddingRight = scrollBarWidth + "px";
	}

	// バリエーション選択チェック判定
	function variation_selected_check() {
    <% if (this.HasVariation == false) { %>
		return true;
	<% } else { %>
		<% if (this.SelectVariationKbn == Constants.SelectVariationKbn.PANEL) { %>
		return $('#<%= this.WhIsSelectingVariationExist.ClientID %>').val() != 'False';
		<% } else if (this.SelectVariationKbn == Constants.SelectVariationKbn.STANDARD || this.SelectVariationKbn == Constants.SelectVariationKbn.DROPDOWNLIST) { %>
		return $('#<%= this.WddlVariationSelect.ClientID %>').val() != '';
		<% } else if (this.SelectVariationKbn == Constants.SelectVariationKbn.DOUBLEDROPDOWNLIST) { %>
			var strVariationSelect = '#<%= this.WddlVariationSelect1.ClientID %>';
			var strVariationSelect2 = '#<%= this.WddlVariationSelect2.ClientID %>';
			return $(strVariationSelect).length && $(strVariationSelect2).length &&
					$(strVariationSelect).val() != '' && $(strVariationSelect2).val() != '';
		<% } else if (this.SelectVariationKbn == Constants.SelectVariationKbn.MATRIX || this.SelectVariationKbn == Constants.SelectVariationKbn.MATRIXANDMESSAGE) { %>
			var blSelectChecked = false;
			$('input[name="Variation"]').each(function() {
				if ($(this).prop('checked')) {
					blSelectChecked = true;
				}
			});
			return blSelectChecked;
		<% } %>
		<% } %>
	}

	// バリエーション選択チェック(通常)
	function add_cart_check() {
		if (variation_selected_check()) {
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

	// 入荷通知登録画面をポップアップウィンドウで開く
	function show_arrival_mail_popup(pid, vid, amkbn) {
		show_popup_window('<%= this.SecurePageProtocolAndHost %><%= Constants.PATH_ROOT %><%= Constants.PAGE_FRONT_USER_PRODUCT_ARRIVAL_MAIL_REGIST %>?<%= Constants.REQUEST_KEY_PRODUCT_ID %>=' + pid + '&<%= Constants.REQUEST_KEY_VARIATION_ID %>=' + vid + '&<%= Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN %>=' + amkbn, 580, 280, false, false, 'Information');
	}

	// マウスイベントの初期化
	addOnload(function () { init(); });

	function bodyPageLoad() {
		if (Sys.WebForms == null) return;
		var isAsyncPostback = Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();
		if (isAsyncPostback) {
			$('.zoomContainer').remove(); // zoomContainerの増殖防止
			tb_init('a.thickbox, area.thickbox, input.thickbox');
			$(function () {
				$(".productInfoList").heightLine().biggerlink({ otherstriggermaster: false })
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
		}
	}
</script>

<table id="tblLayout" class="tblLayout_ProductDetail">
<tr>
<td>
<div id="divTopArea">
<%-- ▽レイアウト領域：トップエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>
<%-- ▽編集可能領域：コンテンツ▽ --%>

<div id="dvProductDetailAreaInModal">
<%-- UPDATE PANEL開始 --%>
<asp:UpdatePanel ID="upUpdatePanel" runat="server">
<ContentTemplate>

<div id="detailImage">

<div class="description">
	<!-- キャッチコピー -->
	<h3><%#: GetProductData("catchcopy") %></h3>

	<!-- 販売期間 -->
	<%if (this.DisplaySell) {%>
	<p>販売期間：<%#: DateTimeUtility.ToStringFromRegion(GetProductData("sell_from"), DateTimeUtility.FormatType.LongDateHourMinute1Letter) %>～<%#: DateTimeUtility.ToStringFromRegion(GetProductData("sell_to"), DateTimeUtility.FormatType.LongDateHourMinute1Letter) %></p>
	<%}%>

	<!-- ホームページリンク -->
	<div visible='<%# StringUtility.ToEmpty(GetProductData("url")) != "" %>' runat="server">
	<a href="<%#: GetProductData("url") %>">メーカのホームページへ</a>
	</div>

	<!-- 問い合わせメールリンク -->
	<div visible='<%# StringUtility.ToEmpty(GetProductData("inquire_email")) != "" %>' runat="server">
	<a href="mailto:<%#: GetProductData("inquire_email") %>">商品のお問合せ</a>
	</div>

	<!-- 電話問い合わせ -->
	<div visible='<%# StringUtility.ToEmpty(GetProductData("inquire_tel")) != "" %>' runat="server">
	お問合せ：<%#: GetProductData("inquire_tel") %></div>
</div>

<%-- ↓バリエーション変更時の表示更新領域を指定しています --%>
<div class="ChangesByVariation" runat="server">
	<!-- 商品画像 -->
	<div class="mainImage">
		<w2c:ProductImage ImageTagId="zoomPicture" data-zoom-image="" ImageSize="LL" IsVariation="<%# (this.VariationSelected) %>" ProductMaster="<%# this.ProductMaster %>" runat="server" />
		<%-- ▽在庫切れ可否▽ --%>
		<span visible='<%# ProductListUtility.IsProductSoldOut(this.ProductMaster) %>' runat="server" class="soldout">SOLDOUT</span>
		<%-- △在庫切れ可否△ --%>
	</div>
</div>
<%-- ↑バリエーション変更時の表示更新領域を指定しています --%>

</div><!-- detailImage -->

<div id="detailOne">

<%-- ↓バリエーション変更時の表示更新領域を指定しています --%>
<div class="ChangesByVariation" runat="server">
	<!-- 商品名 -->
	<h2><%#:GetProductData("name") %></h2>
	<div id="dvProductSubInfo" class="clearFix">
	<!-- 商品ID  -->
	<p class="productDetailId">&nbsp;[<span class="productId"><%#:GetProductData("variation_id") %>]</span></p>
	</div>

	<div class="wrapProductPrice">
	<!-- 商品価格・税区分・加算ポイント -->
	<%-- ▽商品会員ランク価格有効▽ --%>
	<div visible='<%# GetProductMemberRankPriceValid(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected)) %>' runat="server">
		<p class="productPrice">販売価格:<span><strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected))) %></strike></span>(<%#:GetTaxIncludeString(this.ProductMaster) %>)</p>
		<p class="productPrice">会員ランク価格:<span><%#: CurrencyManager.ToPrice(ProductPage.GetProductMemberRankPrice(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected))) %></span>(<%#:GetTaxIncludeString(this.ProductMaster) %>)</p>
	</div>
	<%-- △商品会員ランク価格有効△ --%>
	<%-- ▽商品セール価格有効▽ --%>
	<div visible='<%# GetProductTimeSalesValid(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected)) %>' runat="server">
		<p class="productPrice">販売価格:<span><strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected))) %></strike></span>(<%#:GetTaxIncludeString(this.ProductMaster) %>)</p>
		<p class="productPrice">タイムセールス価格:<span><%#: CurrencyManager.ToPrice(ProductPage.GetProductTimeSalePriceNumeric(this.ProductMaster)) %></span>(<%#:GetTaxIncludeString(this.ProductMaster) %>)</p>
	</div>
	<%-- △商品セール価格有効△ --%>
	<%-- ▽商品特別価格有効▽ --%>
	<div visible='<%# GetProductSpecialPriceValid(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected)) %>' runat="server">
		<p class="productPrice">販売価格:<span><strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected))) %></strike></span>(<%#:GetTaxIncludeString(this.ProductMaster) %>)</p>
		<p class="productPrice">特別価格:<span><%#: CurrencyManager.ToPrice(ProductPage.GetProductSpecialPriceNumeric(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected))) %></span>(<%#:GetTaxIncludeString(this.ProductMaster) %>)</p>
	</div>
	<%-- △商品特別価格有効△ --%>
	<%-- ▽商品通常価格有効▽ --%>
	<div visible='<%# GetProductNormalPriceValid(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected)) %>' runat="server">
		<p class="productPrice">販売価格:<span><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected))) %></span>(<%#:GetTaxIncludeString(this.ProductMaster) %>)</p>
	</div>
	<%-- △商品通常価格有効△ --%>
	<%-- ▽商品加算ポイント▽ --%>
		<p visible='<%# (this.IsLoggedIn && (GetProductAddPointString(this.ProductMaster, this.HasVariation, this.VariationSelected) != "")) %>' runat="server">
			<span class="productPoint">ポイント<%#:GetProductAddPointString(this.ProductMaster, this.HasVariation, this.VariationSelected) %></span>
				<span class="productPoint" visible='<%# (this.IsLoggedIn && ((string)GetKeyValue(this.ProductMaster, Constants.FIELD_PRODUCT_POINT_KBN1)) != Constants.FLG_PRODUCT_POINT_KBN1_NUM) %>' runat="server">(<%#:GetProductAddPointCalculateAfterString(this.ProductMaster, this.HasVariation, this.VariationSelected)%>)
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
		<span class="productPoint">ポイント<%#:GetProductAddPointString(this.ProductMaster, this.HasVariation, this.VariationSelected, true) %></span><span class="productPoint" visible='<%# (this.IsLoggedIn && ((string)GetKeyValue(this.ProductMaster, Constants.FIELD_PRODUCT_POINT_KBN2)) != Constants.FLG_PRODUCT_POINT_KBN1_NUM) %>' runat="server">(<%#:GetProductAddPointCalculateAfterString(this.ProductMaster, this.HasVariation, this.VariationSelected, true)%>)
	</span>
	</p>
	<%-- △定期商品加算ポイント△ --%>
	</div>
</div>
<%-- ↑バリエーション変更時の表示更新領域を指定しています --%>

<div class="wrapDetailImage">
<%-- ▽メイン画像▽ --%>
<div class="unit">
	<div class="mainImage">
		<p class="title">メイン画像</p>
		<a href="javascript:void(0);">
			<img class="zoomTarget" src="<%#:CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)(Constants.PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO + 1)) %>" data-image="<%#:CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)(Constants.PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO + 1)) %>" data-zoom-image="<%#:CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)(Constants.PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO + 1)) %>" />
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
		<img class="zoomTarget" src="<%#:CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)Eval(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO)) %>" data-image="<%#:CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)Eval(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO)) %>" data-zoom-image="<%#:CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)Eval(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO)) %>" /></a>
	</li>
	</ItemTemplate>
	<FooterTemplate>
	</FooterTemplate>
</asp:Repeater>
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
	<% } %>
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
	<% } %>
<% } else if ((this.SelectVariationKbn == Constants.SelectVariationKbn.STANDARD) || (this.SelectVariationKbn == Constants.SelectVariationKbn.DROPDOWNLIST)) { %>
	<asp:DropDownList ID="ddlVariationSelect" DataSource='<%# this.ProductValirationListItemCollection %>' DataTextField="Text" DataValueField="Value" SelectedValue='<%# (this.HasVariation && this.VariationSelected && ((this.SelectVariationKbn == Constants.SelectVariationKbn.STANDARD) || (this.SelectVariationKbn == Constants.SelectVariationKbn.DROPDOWNLIST))) ? this.VariationId : null %>' Visible="<%# this.HasVariation %>" OnSelectedIndexChanged="ddlVariationId_SelectedIndexChanged" AutoPostBack="True" runat="server"></asp:DropDownList>
<% } else if (this.SelectVariationKbn == Constants.SelectVariationKbn.DOUBLEDROPDOWNLIST) { %>
	<div  style="color: red; font-size: 12px; margin-bottom: 10px"><asp:Literal ID="lCombinationErrorMessage" runat="server"/></div>
	<asp:DropDownList ID="ddlVariationSelect1" DataSource='<%# this.ProductValirationListItemCollection %>' DataTextField="Text" DataValueField="Value" SelectedValue='<%# (this.HasVariation && (this.SelectedVariationName1 != "") && (this.SelectVariationKbn == Constants.SelectVariationKbn.DOUBLEDROPDOWNLIST)) ? this.SelectedVariationName1 : null %>' Visible="<%# this.HasVariation %>" OnSelectedIndexChanged="ddlVariationId_SelectedIndexChanged" AutoPostBack="True" runat="server"></asp:DropDownList>
	<asp:DropDownList ID="ddlVariationSelect2" DataSource='<%# this.ProductValirationListItemCollection2 %>' DataTextField="Text" DataValueField="Value" SelectedValue='<%# (this.HasVariation && (this.SelectVariationKbn == Constants.SelectVariationKbn.DOUBLEDROPDOWNLIST)) ? this.SelectedVariationName2 : null %>' Visible="<%# this.HasVariation %>" OnSelectedIndexChanged="ddlVariationId_SelectedIndexChanged" AutoPostBack="True" runat="server"></asp:DropDownList>
<% } else if (this.SelectVariationKbn == Constants.SelectVariationKbn.MATRIX || (this.SelectVariationKbn == Constants.SelectVariationKbn.MATRIXANDMESSAGE)) { %>
	<!--1軸バリエーション-->
	<% if (this.IsPluralVariation == false) { %>
		<table cellspacing="0" border="1">
			<asp:Repeater ID="rVariationSingleList" runat="server">
			<ItemTemplate>
			<tr>
				<td class="selectValiationMatrix">
					<span>&nbsp;<%#:CreateVariationName(Container.DataItem, "", "", Constants.CONST_PRODUCTVARIATIONNAME_PUNCTUATION) %>&nbsp;</span>
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
							<%#:GetStockMessageForMatrix(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1, ((RepeaterItem)Container.Parent.Parent).DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2, Container.DataItem) %>
							<% } %>
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
		在庫状況 : <%#:w2.App.Common.Order.ProductCommon.CreateProductStockMessage(this.ProductMaster, true) %>
	</p>
</div>
<%if (this.HasStockMessage) {%>
<%if (this.HasVariation) {%>
<p class="productStock">
	<a href="" onclick='<%#:"javascript:show_popup_window('" + CreateProductStockListUrl() + "', 700, 400, true, true, 'ProductStockList'); return false;" %>'>在庫状況</a>
</p>
<%} // (this.HasVariation) %>
<%if (this.HasVariation == false) {%>
<p class="productStock">
在庫状況：<%#:w2.App.Common.Order.ProductCommon.CreateProductStockMessage(this.ProductMaster, true) %><%} // (this.HasVariation == false) %></p>
<%} // (this.HasStockMessage) %>
</div>

</div><!-- <%# this.Buyable %> -->

<div visible="<%# (this.IsSelectingVariationExist) %>" runat="server">
<p class="error"><%#:this.AlertMessage %></p>
<div class="error"><%: this.ErrorMessageFixedPurchaseMember %></div>
<p class="error"><%#:this.LimitedPaymentMessages %></p>
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

</div><!-- productSellInfo -->
</div>
<%-- ↑バリエーション変更時の表示更新領域を指定しています --%>

<div>
<p id="addFavoriteTip" class="toolTip" style="display: none;">
	<span style="margin: 10px;" id="txt-tooltip"></span>
	<a href="<%: Constants.PATH_ROOT + Constants.PAGE_FRONT_FAVORITE_LIST %>" class="btn btn-mini btn-inverse">お気に入り一覧</a>
</p>
</div>

<!--在庫表表示-->
<div id="dvProductStock">
<uc:BodyProductStockList ShopId="<%# this.ShopId %>" ProductId="<%# this.ProductId %>" Visible="<%# this.ShouldShowStockList %>" runat="server" />
</div>
<div id="dvProductDescription">
	<div visible='<%# this.IsProductOutlineVisible %>' runat="server">
	<p class="title">概要</p><br />
		<p><%# GetProductDataHtml(this.ProductMaster, "outline") %></p><br />
	</div>

　　<div visible='<%# this.IsProductDetailVisible %>' runat="server">
	<p class="title">詳細情報</p><br />
	<%-- 商品詳細1 --%>
	<p><%# GetProductDataHtml(this.ProductMaster, "desc_detail1") %></p><br />
	<%-- 商品詳細2 --%>
	<p><%# GetProductDataHtml(this.ProductMaster, "desc_detail2") %></p><br />
	<%-- 商品詳細3 --%>
	<p><%# GetProductDataHtml(this.ProductMaster, "desc_detail3") %></p><br />
	<%-- 商品詳細4 --%>
	<p><%# GetProductDataHtml(this.ProductMaster, "desc_detail4") %></p>
	</div>
</div>
</ContentTemplate>
</asp:UpdatePanel>
</div>
</td>
</tr>
</table>
