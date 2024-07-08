<%--
=========================================================================================================
  Module      : スマートフォン用商品詳細モーダル画面 (ProductDetailModal.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%@ Register TagPrefix="uc" TagName="BodyProductArrivalMailRegister" Src="~/SmartPhone/Form/Common/Product/BodyProductArrivalMailRegister.ascx" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Product/ProductDetailModal.ascx.cs" Inherits="Form_Common_Product_ProductDetailModal" %>
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<link href="<%: Constants.PATH_ROOT + "SmartPhone/Css/product.css" %>" rel="stylesheet" type="text/css" media="all" />
<script type="text/javascript" charset="UTF-8" src="<%= Constants.PATH_ROOT %>SmartPhone/Js/imagesloaded.pkgd.min.js"></script>
<script type="text/javascript" charset="UTF-8" src="<%= Constants.PATH_ROOT %>SmartPhone/Js/flipsnap.js"></script>
<script type="text/javascript" language="javascript">
	function openModal() {
		const scrollBarWidth = window.innerWidth - document.body.clientWidth;

		// 背景のスクロールを禁止
		document.body.style.overflow = "hidden";
		document.body.style.paddingRight = scrollBarWidth + "px";
	}

	// 商品詳細：画像スライド
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

		$('.flipsnap').css({ width: len * 215 });
		var flipsnap = Flipsnap('.flipsnap', {
			distance: 215
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
					|| (this.SelectVariationKbn == Constants.SelectVariationKbn.MATRIXANDMESSAGE)))) { %>
				flipsnap.moveToPoint(clickName, 0);
				$pointer.filter('.current').removeClass('current');
				$('.product-image-sub a[name=' + (clickName) + ']').addClass('current');
		<%} else { %>
			if (0 < $variation.size()) {
				flipsnap.moveToPoint(clickName, 0);
				$pointer.filter('.current').removeClass('current');
				$('.product-image-sub a[name=' + (clickName) + ']').addClass('current');
			}
		<% } %>
	}

	var strAlertmessage = '<%= MESSAGE_ERROR_VARIATION_UNSELECTED %>';
	var strAlertMessageOption = '<%= MESSAGE_ERROR_OPTION_UNSELECTED %>';
	var fixedpurchaseMessage = '定期的に商品をお送りする「定期購入」で購入します。\nよろしいですか？';
	var subscriptionBoxMessage = '頒布会「@@ 1 @@」の内容確認画面に進みます。\nよろしいですか？';
	var mailSendAlertMessage = 'お気に入り追加商品の在庫減少時にメールを送信いたします。\nマイページ＞登録情報の変更より変更できます。';

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

	// カート投入時チェック
	function add_cart_check() {
		if (variation_selected_check()) {
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

	// 頒布会カート投入時チェック
	function add_cart_check_for_subscriptionBox() {
		var value = event.target;

		if (value == null) return;

		if (variation_selected_check()) {
			var $parent = $(value).parent();
			var subscriptionBoxElement = $parent.find("[id$='ddlSubscriptionBox']");
			var hiddenFieldElement = $parent.find("[id$='hfSubscriptionBoxDisplayName']");
			var subscriptionBoxName = (subscriptionBoxElement.length > 0)
				? $('#<%: ddlSubscriptionBox.ClientID %> option:selected').text()
				: (hiddenFieldElement.length > 0) ? hiddenFieldElement.val() : null;

			if (subscriptionBoxName != null) {
				return confirm('頒布会「' + subscriptionBoxName + '」の内容確認画面に進みます。\nよろしいですか？');
			} else {
				alert("エラー: サブスクリプションボックス名が見つかりません。");
				return false;
			}
		} else {
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
</script>
<%-- △編集可能領域△ --%>

<%-- 表示更新の為のpageLoad --%>
<script type="text/javascript" language="javascript">
	function bodyPageLoad() {
		if (Sys.WebForms == null) return;
		var isAsyncPostback = Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();
		if (isAsyncPostback) {
			detailImageFlick();
		}
	}
</script>
<asp:Repeater ID="rProductsAll" runat="server">
	<ItemTemplate>
		</ItemTemplate>
	</asp:Repeater>
<%-- UPDATE PANEL開始 --%>
<asp:UpdatePanel ID="upUpdatePanel" runat="server">
<ContentTemplate>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<%-- カート投入ボタン押下時にどの画面へ遷移するか？ --%>
<%-- CART：カート一覧画面 その他：画面遷移しない --%>
<asp:HiddenField ID="hfIsRedirectAfterAddProduct" Value="CART" runat="server" />

<asp:HiddenField ID="hfVariationSelectedIndex" Value="0" runat="server" />
<section class="wrap-product-detail">

<%-- class:"ChangesByVariation"→バリエーション変更時の表示更新領域を指定しています --%>
<!-- 商品詳細TOP -->

<article class="ChangesByVariation" runat="server">
	<ul class="product-info">
		<%-- 商品名 --%>
		<h1><%#:GetProductData("name") %></h1>
		<li>
			<div class="product-price">
			<!-- 商品価格・税区分・加算ポイント -->
			<%-- ▽商品会員ランク価格有効▽ --%>
			<p visible='<%# GetProductMemberRankPriceValid(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected)) %>' runat="server" class="special">
				<%#: CurrencyManager.ToPrice(ProductPage.GetProductMemberRankPrice(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected))) %>
					(<%#:GetTaxIncludeString(this.ProductMaster) %>)
				<span class="line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected))) %>
				(<%#:GetTaxIncludeString(this.ProductMaster) %>)</span>
			</p>
			<%-- △商品会員ランク価格有効△ --%>

			<%-- ▽商品セール価格有効▽ --%>
			<p visible='<%# GetProductTimeSalesValid(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected)) %>' runat="server" class="special">
				<%#: CurrencyManager.ToPrice(ProductPage.GetProductTimeSalePriceNumeric(this.ProductMaster)) %>
				(<%#:GetTaxIncludeString(this.ProductMaster) %>)
				<span class="line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected))) %>
				(<%#:GetTaxIncludeString(this.ProductMaster) %>)</span>
			</p>
			<%-- △商品セール価格有効△ --%>

			<%-- ▽商品特別価格有効▽ --%>
			<p visible='<%# GetProductSpecialPriceValid(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected)) %>' runat="server" class="special">
				<%#: CurrencyManager.ToPrice(ProductPage.GetProductSpecialPriceNumeric(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected))) %>
				(<%#:GetTaxIncludeString(this.ProductMaster) %>)
				<span class="line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected))) %>
				(<%#:GetTaxIncludeString(this.ProductMaster) %>)</span>
			</p>
			<%-- △商品特別価格有効△ --%>

			<%-- ▽商品通常価格有効▽ --%>
			<p visible='<%# GetProductNormalPriceValid(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected)) %>' runat="server">
				<%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected))) %>
				(<%#:GetTaxIncludeString(this.ProductMaster) %>)
			</p>
			<%-- △商品通常価格有効△ --%>
			<%-- ▽商品定期購入価格▽ --%>
			<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
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
			<span class="point">ポイント<%#:GetProductAddPointString(this.ProductMaster, this.HasVariation, this.VariationSelected) %>還元</span>
			<%-- △商品加算ポイント△ --%>
		</li>
		<li visible='<%# (this.IsLoggedIn && (this.CanFixedPurchase) && (this.IsUserFixedPurchaseAble) && (GetProductAddPointString(this.ProductMaster, this.HasVariation, this.VariationSelected) != "")) %>' runat="server">
			<%-- ▽定期商品加算ポイント▽ --%>
			<span class="point">ポイント（定期購入）<%#:GetProductAddPointString(this.ProductMaster, this.HasVariation, this.VariationSelected, true) %>還元</span>
			<%-- △定期商品加算ポイント△ --%>
		</li>
	</ul>

	<div class="wrap-product-image">
		<%-- ▽メイン画像▽ --%>
		<div class="product-image-detail">
			<div class="flick-wrap loading">
				<div class="flipsnap">
					<%-- ▽メイン画像▽ --%>
					<div class="item">
						<img class="zoomTarget" src="<%#:CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)(Constants.PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO + 1)) %>" data-image="<%#:CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)(Constants.PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO + 1)) %>" data-zoom-image="<%#:CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)(Constants.PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO + 1)) %>" />

					</div>
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
							<img src="<%#:CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)Eval(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO)) %>" alt="" />
						</div>
					</ItemTemplate>
					</asp:Repeater>
					<%-- △サブ画像一覧△ --%>
				</div>
			</div>

			<div class="product-image-sub">
				<%--メイン画像一覧--%>
				<a href="javascript:void(0);">
					<img class="zoomTarget" src="<%#:CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)(Constants.PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO + 1)) %>" data-image="<%#:CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)(Constants.PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO + 1)) %>" data-zoom-image="<%#:CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)(Constants.PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO + 1)) %>" />
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
						<img src="<%#:CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_M, (int)Eval(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO)) %>" alt="" /></a>
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

	<div class="product-vatiation unit" runat="server">
		<%-- ドロップダウン形式 --%>
		<% if(this.HasVariation) { %>
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
				<% } %>
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
				<% } %>
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
			<% } %>
		<% } %>
		</div>

	<div class="wrap-product-cart">
		<div Visible="<%# this.IsDisplayExcludeFreeShippingText %>" runat="server">
			<p style="color:red;">※本商品は配送料無料適用外です</p>
		</div>

		<div visible="<%# (this.IsSelectingVariationExist) %>" runat="server">
			<p class="attention">
				<%#:this.AlertMessage %>
			</p>
			<div class="attention"><%: this.ErrorMessageFixedPurchaseMember %></div>
			<p class="attention">
				<%#:this.LimitedPaymentMessages %>
			</p>
		</div>
		<div visible="<%# this.Buyable %>" class="wrap-buyable" runat="server">

		<%-- 在庫文言表示 --%>
			<div visible="<%# this.IsStockManaged == false %>" runat="server">
				<p>
					在庫状況 : <%#:w2.App.Common.Order.ProductCommon.CreateProductStockMessage(this.ProductMaster, true) %>
				</p>
			</div>
			<% if (this.HasStockMessage) { %>
			<div class="unit stock">
				<% if (this.HasVariation) { %>
					<a href="" onclick='<%#:"javascript:show_popup_window('" + CreateProductStockListUrl() + "', 700, 400, true, true, 'ProductStockList');return false;" %>'>在庫状況はこちら</a>
				<% } %>
				<% if (this.HasVariation == false) { %>
					在庫状況：<%#:w2.App.Common.Order.ProductCommon.CreateProductStockMessage(this.ProductMaster, true) %>
				<% } %>
			</div>
			<% } %>

			<div visible="<%# this.IsDisplayValiationStock %>" runat="server">
				<p>在庫有り</p>
			</div>

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
				<asp:LinkButton ID="lbCartAddSubscriptionBox" class="btn btn-mid btn-inverse" runat="server" Visible="<%# (this.CanFixedPurchase) && this.IsUserFixedPurchaseAble && (this.IsSubscriptionBoxValid) %>" OnClick="lbCartAddSubscriptionBox_Click" OnClientClick="return add_cart_check_for_subscriptionBox();">
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
			<% if (StringUtility.ToEmpty(this.ErrorMessage) != "") { %>
			<br /><span class="attention"><%: this.ErrorMessage %></span>
			<% } %>
		</div>
	</div>
</article>
</section>

<%-- △編集可能領域△ --%>
<div id="divBottomArea">
<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<%-- △レイアウト領域△ --%>
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
<%-- UPDATE PANELここまで --%>
