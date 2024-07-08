<%--
=========================================================================================================
  Module      : スマートフォン用注文履歴一覧画面(OrderHistoryList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%@ Register TagPrefix="uc" TagName="BodyProductArrivalMailRegister" Src="~/SmartPhone/Form/Common/Product/BodyProductArrivalMailRegister.ascx" %>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/OrderHistory/OrderHistoryList.aspx.cs" Inherits="Form_Order_OrderHistoryList" Title="購入履歴一覧ページ" %>
<%@ Import Namespace = "w2.Domain.Order" %>
<asp:Content ContentPlaceHolderID="head" Runat="Server">
<link href="<%: Constants.PATH_ROOT + "SmartPhone/Css/order.css" %>" rel="stylesheet" type="text/css" media="all" />
<link href="<%: Constants.PATH_ROOT + "SmartPhone/Css/user.css" %>" rel="stylesheet" type="text/css" media="all" />
<script type="text/javascript" charset="Shift_JIS" src="<%= Constants.PATH_ROOT %>Js/floatingWindow.js"></script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<section class="wrap-order order-history-list">

<%-- UPDATE PANEL開始 --%>
<asp:UpdatePanel ID="upUpdatePanel" runat="server">
<ContentTemplate>

<%-- カート投入ボタン押下時にどの画面へ遷移するか？ --%>
<%-- CART：カート一覧画面 その他：画面遷移しない --%>
<asp:HiddenField ID="hfIsRedirectAfterAddProduct" Value="" runat="server" />

<div class="order-unit">

	<h2>購入履歴一覧</h2>

	<%-- 注文一覧 --%>
	<asp:Repeater ID="rOrderList" Runat="server">
		<HeaderTemplate>
			<nav class="order-history-nav">
				<ul>
				<li>注文一覧</li>
				<li><a href="<%: UrlChangeDisplayType %>">注文商品一覧</a></li>
				</ul>
			</nav>
			<p class="msg">※ご注文内容の詳細をご覧になるには【ご注文番号】のリンクを押してください。</p>
			<%-- ページャ --%>
			<div class="pager-wrap above"><%= this.PagerHtml %></div>
			<div class="content">
			<ul>
		</HeaderTemplate>
		<ItemTemplate>
			<li>
				<h3>ご注文番号</h3>
				<h4 class="order-id"><a href='<%#: Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL + "?" + Constants.REQUEST_KEY_ORDER_ID + "=" + HttpUtility.UrlEncode((string)Eval(Constants.FIELD_ORDER_ORDER_ID)) %>'>
						<%#: Eval(Constants.FIELD_ORDER_ORDER_ID) %></a></h4>
				<dl class="order-form">
					<dt>ご購入日</dt>
					<dd><%#: DateTimeUtility.ToStringFromRegion(Eval(Constants.FIELD_ORDER_ORDER_DATE), DateTimeUtility.FormatType.ShortDate2Letter) %></dd>
					<dt>総合計</dt>
					<dd><%#: CurrencyManager.ToPrice(Eval(Constants.FIELD_ORDER_ORDER_PRICE_TOTAL)) %></dd>
					<dt>ご注文状況</dt>
					<dd>
						<%#: (IsPickupRealShop(new OrderModel((DataRowView)Container.DataItem)) && ((string)Eval(Constants.FIELD_ORDER_ORDER_STATUS) == Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP))
							? ((string)Eval(Constants.FIELD_ORDER_STOREPICKUP_STATUS) == Constants.FLG_STOREPICKUP_STATUS_PENDING) ? ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_STATUS, (string)Eval(Constants.FIELD_ORDER_ORDER_STATUS)) + "(店舗未到着)" : ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_STOREPICKUP_STATUS, (string)Eval(Constants.FIELD_ORDER_STOREPICKUP_STATUS))
							: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_STATUS, Eval(Constants.FIELD_ORDER_ORDER_STATUS)) %><%#: (string)Eval(Constants.FIELD_ORDER_SHIPPED_CHANGED_KBN) == Constants.FLG_ORDER_SHIPPED_CHANGED_KBN_CHANAGED ? "（変更有り）" : string.Empty %>
					</dd>
					<% if (Constants.PAYMENT_GMO_POST_ENABLED) { %>
						<!--show credit status if using GMO-->
						<dt runat="server" visible="<%# ((Eval(Constants.FIELD_ORDER_ORDER_PAYMENT_KBN).ToString() == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO) || (Eval(Constants.FIELD_ORDER_ORDER_PAYMENT_KBN).ToString() == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)) %>">
							与信状況
						</dt>
						<dd visible="<%# ((Eval(Constants.FIELD_ORDER_ORDER_PAYMENT_KBN).ToString() == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO) || (Eval(Constants.FIELD_ORDER_ORDER_PAYMENT_KBN).ToString() == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)) %>" runat="server">
							<%# ValueText.GetValueText(Constants.TABLE_ORDER,Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS, Eval(Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS))%>
						</dd>
					<% } %>
					<dt>配送希望日</dt>
					<dd><%# WebSanitizer.HtmlEncodeChangeToBr(GetShippingDate(Eval(Constants.FIELD_ORDER_ORDER_ID).ToString())) %></dd>
					<% if(this.DisplayScheduledShippingDate) { %>
					<dt>出荷予定日</dt>
					<dd><%#: GetScheduledShippingDate(Eval(Constants.FIELD_ORDER_ORDER_ID).ToString()) %></dd>
					<% } %>
					<% if(Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
					<dt>定期購入ID</dt>
					<dd><%#: Eval(Constants.FIELD_ORDER_FIXED_PURCHASE_ID) %></dd>
					<% } %>
				</dl>
				<div class="button">
					<a href='<%#: Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL + "?" + Constants.REQUEST_KEY_ORDER_ID + "=" + HttpUtility.UrlEncode((string)Eval(Constants.FIELD_ORDER_ORDER_ID)) %>' class="btn">
					詳細はこちら</a>
				</div>
			</li>
		</ItemTemplate>
		<FooterTemplate>
			</ul>
			</div>
		</FooterTemplate>
	</asp:Repeater>
	
	<%-- 注文商品一覧 --%>
	<asp:Repeater ID="rOrderProductsList" onitemcommand="AddCartVariationList_ItemCommand" ItemType="DataRowView" Runat="server">
		<HeaderTemplate>
			<nav class="order-history-nav">
				<ul>
				<li><a href="<%: UrlChangeDisplayType %>">注文一覧</a></li>
				<li>注文商品一覧</li>
				</ul>
			</nav>
			<p class="msg">※商品情報が取得できない場合、一覧に表示できないことがあります。</p>
			<%-- ページャ --%>
			<div class="pager-wrap above"><%= this.PagerHtml %></div>
			<div class="content">
			<ul class="order-history-product-list">
		</HeaderTemplate>
		<ItemTemplate>
			<li>
				<table class="cart-table">
				<tbody>
				<tr class="cart-unit-product">
				<td class="product-image">
					<%-- 一致する商品IDが現在も存在する場合、商品詳細ページへのリンクを表示する --%>
					<a href='<%#: CreateProductDetailVariationUrl(Item) %>' runat="server" Visible="<%# IsProductDetailLinkValid(Item) %>">
						<w2c:ProductImage ImageSize="M" ProductMaster="<%# Item %>" IsVariation="false" runat="server" />
					</a>
					<w2c:ProductImage ImageSize="M" ProductMaster="<%# Item %>" IsVariation="false" runat="server" Visible="<%# (IsProductDetailLinkValid(Item) == false) %>" />
					<%-- ▽在庫切れ可否▽ --%>
					<span visible='<%# ProductListUtility.IsProductSoldOut(Item) %>' runat="server" class="sold-out">SOLD OUT</span>
					<%-- △在庫切れ可否△ --%>
				</td>
				<td class="product-info">
					<ul>
						<li class="order-date">
							ご購入日　
							<a href='<%#: Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL + "?" + Constants.REQUEST_KEY_ORDER_ID + "=" + HttpUtility.UrlEncode((string)Eval(Constants.FIELD_ORDER_ORDER_ID)) %>'>
							<%#: DateTimeUtility.ToStringFromRegion(Eval(Constants.FIELD_ORDER_ORDER_DATE), DateTimeUtility.FormatType.ShortDate2Letter) %>
							</a>
						</li>
						<li class="product-name">
							<%-- 一致する商品IDが現在も存在する場合、商品詳細ページへのリンクを表示する --%>
							<a href='<%#: CreateProductDetailVariationUrl(Item) %>' target="_blank" runat="server" Visible="<%# IsProductDetailLinkValid(Item) %>">
								<%#: Eval(CONST_ORDER_ITEM_VARIATION_NAME) %>
							</a>
							<%#: (IsProductDetailLinkValid(Item) == false) ? Eval(CONST_ORDER_ITEM_VARIATION_NAME) : "" %><%-- 商品名 --%>
						</li>
						<li class="product-price">
							<%-- ▽商品会員ランク価格有効▽ --%>
							<p visible='<%# GetProductMemberRankPriceValid(Item, true) %>' runat="server" class="special">
								<%#: CurrencyManager.ToPrice(ProductPage.GetProductMemberRankPrice(Item, true)) %>
								（<%#: GetTaxIncludeString(Item) %>）
								<span class="line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Item, true)) %>（<%#: GetTaxIncludeString(Item) %>）</span>
							</p>
							<%-- △商品会員ランク価格有効△ --%>
							<%-- ▽商品セール価格有効▽ --%>
							<p visible='<%# GetProductTimeSalesValid(Item) %>' runat="server" class="special">
								<%#: CurrencyManager.ToPrice(ProductPage.GetProductTimeSalePriceNumeric(Item)) %>
								（<%#: GetTaxIncludeString(Item) %>）
								<span class="line-through"><span class="line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Item, true)) %>（<%#: GetTaxIncludeString(Item) %>）</span>
							</p>
							<%-- △商品セール価格有効△ --%>
							<%-- ▽商品特別価格有効▽ --%>
							<p visible='<%# GetProductSpecialPriceValid(Item, true) %>' runat="server" class="special">
								<%#: CurrencyManager.ToPrice(ProductPage.GetProductSpecialPriceNumeric(Item, true)) %>（<%#: GetTaxIncludeString(Item) %>）
								<span class="line-through"><span class="line-through"><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Item, true)) %>（<%#: GetTaxIncludeString(Item) %>）</span>
							</p>
							<%-- △商品特別価格有効△ --%>
							<%-- ▽商品通常価格有効▽ --%>
							<p visible='<%# GetProductNormalPriceValid(Item, true) %>' runat="server">
								<%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Item, true)) %>
								（<%#: GetTaxIncludeString(Item) %>）
							</p>
							<%-- △商品通常価格有効△ --%>
							<%-- ▽商品加算ポイント▽ --%>
							<p visible='<%# (this.IsLoggedIn && (GetProductAddPointString(Item) != "")) %>' runat="server">
								<span>ポイント<%#: GetProductAddPointString(Item) %></span>
							</p>
							<%-- △商品加算ポイント△ --%>
							<%-- ▽商品定期購入価格▽ --%>
							<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) {%>
							<p visible='<%# (GetKeyValue(Item, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG).ToString() != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) && ((CheckFixedPurchaseLimitedUserLevel(this.ShopId, (string)GetProductData(Item, "product_id")) == false)) %>' runat="server">
								<span visible='<%# IsProductFixedPurchaseFirsttimePriceValid(Item, true) %>' runat="server">
									定期初回価格:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchaseFirsttimePrice(Item, true)) %>（<%#: GetTaxIncludeString(Item) %>）
									<br />
								</span>
								定期通常価格:<%#: CurrencyManager.ToPrice(ProductPage.GetProductFixedPurchasePrice(Item, true)) %>（<%#: GetTaxIncludeString(Item) %>）
							</p>
							<% } %>
							<%-- △商品定期購入価格△ --%>
							<%-- ▽定期商品加算ポイント▽ --%>
							<p visible='<%# (this.IsLoggedIn && (GetProductAddPointString(Item) != "") && (GetKeyValue(Item, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG).ToString() != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) && ((CheckFixedPurchaseLimitedUserLevel(this.ShopId, (string)GetProductData(Item, "product_id")) == false))) %>' runat="server">
								<span class="addPoint">ポイント<%#: GetProductAddPointString((object)Item, false, false, true) %></span><span visible='<%# ((string)GetKeyValue(Item, Constants.FIELD_PRODUCT_POINT_KBN2)) == Constants.FLG_PRODUCT_POINT_KBN2_RATE %>' runat="server">(<%# WebSanitizer.HtmlEncode(GetProductAddPointCalculateAfterString(Item, false, false, true))%>)
								</span>
							</p>
							<%-- △定期商品加算ポイント△ --%>
							<%-- ▽商品アイコン▽ --%>
							<%-- 
							<p>
							<w2c:ProductIcon ID="ProductIcon1" IconNo="1" ProductMaster="<%# Item %>" runat="server" />
							<w2c:ProductIcon ID="ProductIcon2" IconNo="2" ProductMaster="<%# Item %>" runat="server" />
							<w2c:ProductIcon ID="ProductIcon3" IconNo="3" ProductMaster="<%# Item %>" runat="server" />
							<w2c:ProductIcon ID="ProductIcon4" IconNo="4" ProductMaster="<%# Item %>" runat="server" />
							<w2c:ProductIcon ID="ProductIcon5" IconNo="5" ProductMaster="<%# Item %>" runat="server" />
							<w2c:ProductIcon ID="ProductIcon6" IconNo="6" ProductMaster="<%# Item %>" runat="server" />
							<w2c:ProductIcon ID="ProductIcon7" IconNo="7" ProductMaster="<%# Item %>" runat="server" />
							<w2c:ProductIcon ID="ProductIcon8" IconNo="8" ProductMaster="<%# Item %>" runat="server" />
							<w2c:ProductIcon ID="ProductIcon9" IconNo="9" ProductMaster="<%# Item %>" runat="server" />
							<w2c:ProductIcon ID="ProductIcon10" IconNo="10" ProductMaster="<%# Item %>" runat="server" />
							</p>
							--%>
							<%-- △商品アイコン△ --%>
						</li>
					</ul>
				</td>
				</tr>
				</tbody>
				</table>
			
				<%-- ===================================================== --%>
				<div class="button">
					<%-- カート投入ボタン --%>
					<%-- カートに入れるボタン表示 --%>
					<%-- カートに入れる処理（商品一覧ページと同じ） --%>
					<asp:LinkButton ID="lbCartAddVariationList" CSSClass="btn" runat="server" Visible='<%# (bool)GetHistoryItemKeyValue(Container.ItemIndex, "CanCart") && IsProductValid(Item) %>' CommandName="CartAdd" OnClientClick="return actionDisplayAddCartPopup();" >
						再注文する
					</asp:LinkButton>
					<%-- 定期購入ボタン表示 --%>
					<asp:LinkButton ID="lbCartAddFixedPurchaseVariationList" CSSClass="btn" runat="server" Visible='<%# (bool)GetHistoryItemKeyValue(Container.ItemIndex, "CanFixedPurchase") && IsProductValid(Item) && ((CheckFixedPurchaseLimitedUserLevel(this.ShopId, (string)GetProductData(Container.DataItem, "product_id")) == false)) %>' OnClientClick="return actionDisplayAddCartPopup();" CommandName="CartAddFixedPurchase" >
						再注文する(定期購入)
					</asp:LinkButton>
					<%-- ===================================================== --%>
					<%-- 入荷通知メールボタン --%>
					<%-- 再入荷通知メール申し込みボタン表示 --%>
					<div visible='<%# ((string)GetHistoryItemKeyValue(Container.ItemIndex, "ArrivalMailKbn") == Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL) %>' runat="server">
						<asp:LinkButton CommandName="SmartArrivalMail" CommandArgument="Arrival" Runat="server" class="btn btn-mini btn-inverse">
						入荷お知らせメール申込
						</asp:LinkButton>
						<%-- 再入荷通知メール登録フォーム表示 --%>
						<uc:BodyProductArrivalMailRegister runat="server" ID="ucBpamrArrival" ArrivalMailKbn="<%#: Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL %>" ProductId="<%#: GetKeyValue(Item, Constants.FIELD_PRODUCT_PRODUCT_ID) %>" VariationId="<%#: GetKeyValue(Item, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) %>" Visible="false" />
					</div>
					<%-- 販売開始通知メール申し込みボタン表示 --%>
					<%-- 
					<div visible='<%# ((string)GetHistoryItemKeyValue(Container.ItemIndex, "ArrivalMailKbn") == Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE) %>' runat="server">
					<asp:LinkButton ID="lbRequestReleaseMailVariationList2" Runat="server" OnClientClick="<%# CreateArivalMail2ClientScript(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE, (string)GetKeyValue(Item, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID), (string)GetKeyValue(Item, Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID)) %>" class="btn btn-mini btn-inverse">
					販売開始通知メール申込
					</asp:LinkButton>
					</div>
					--%>
					<%-- 再販売通知メール申し込みボタン表示 --%>
					<div visible='<%# ((string)GetHistoryItemKeyValue(Container.ItemIndex, "ArrivalMailKbn") == Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE) %>' runat="server">
						<asp:LinkButton CommandName="SmartArrivalMail" CommandArgument="Resale" Runat="server" class="btn btn-mini btn-inverse">
						再販売通知メール申込
						</asp:LinkButton>
						<%-- 再入荷通知メール登録フォーム表示 --%>
						<uc:BodyProductArrivalMailRegister runat="server" ID="ucBpamrResale" ArrivalMailKbn="<%#: Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE %>" ProductId="<%#: GetKeyValue(Item, Constants.FIELD_PRODUCT_PRODUCT_ID) %>" VariationId="<%#: GetKeyValue(Item, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) %>" Visible="false" />
					</div>
					<%-- エラー表示 --%>
					<p class="attention"><%#: GetKeyValue(Item, "ErrorMessage") %></p>
					<%-- ===================================================== --%>
					<%-- 隠しフィールド --%>
					<asp:HiddenField ID="hfProductId" Value="<%# GetKeyValue(Item, Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID) %>" runat="server" />
					<asp:HiddenField ID="hfVariationId" Value="<%# GetKeyValue(Item, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) %>" runat="server" />
					<asp:HiddenField ID="hfArrivalMailKbn" Value='<%# GetHistoryItemKeyValue(Container.ItemIndex, "ArrivalMailKbn") %>' runat="server" />
					<%-- ===================================================== --%>
				</div>
			</li>
		</ItemTemplate>
		<FooterTemplate>
			</ul>
			</div>
		</FooterTemplate>
	</asp:Repeater>

	<%-- 購入履歴なし--%>
	<% if(StringUtility.ToEmpty(this.AlertMessage) != "") {%>
	<div class="msg-alert">
		<%= this.AlertMessage %>
	</div>
	<%} %>

	<%-- ページャ --%>
	<div class="pager-wrap below"><%= this.PagerHtml %></div>

<%-- ポップアップ表示内容の定義 --%>
<script type="text/javascript">
<!--
	// フローティングウィンドウ表示
	function actionDisplayAddCartPopup() {
		displayAddCartPopup($('#<%: hfIsRedirectAfterAddProduct.ClientID %>').val());
		closeAddCartPopup();
	}

	// 入荷通知登録画面をポップアップウィンドウで開く
	function show_arrival_mail_popup(pid, vid, amkbn) {
		show_popup_window('<%= this.SecurePageProtocolAndHost %><%= Constants.PATH_ROOT %><%= Constants.PAGE_FRONT_USER_PRODUCT_ARRIVAL_MAIL_REGIST %>?<%= Constants.REQUEST_KEY_PRODUCT_ID %>=' + pid + '&<%= Constants.REQUEST_KEY_VARIATION_ID %>=' + vid + '&<%= Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN %>=' + amkbn, 480, 270, true, true, 'Information');
	}

	// フローティングウィンドウ閉じる
	function closeAddCartPopup() {
		$('.close-popup').each(function () {
			$(this).click(function () {
				$(this).parent('div').fadeOut();
			});
		});
	}
	-->
</script>
<style type="text/css">
<!--
/* フローティングウィンドウデザイン */
#addCartResultPopup {
	display: none;
	position: absolute;
	z-index: 0;
	background: #fff;
	width: 200px;
	height: 170px;
	border: 1px solid #777;
	text-align: center;
	line-height: 1.5;
	box-shadow: 8px 8px 16px #aaa;
	padding-bottom: .5em;
}

/* フローティングウィンドウ内、タイトル */
#addCartResultPopup .title
{
	width:100%;
	background:#777;
	color:#fff;
	padding:.5em 0;
	margin-bottom: .5em;
	font-weight:bold;
}

#addCartResultPopup .btn {
	width: 60%;
	padding: .3em;
	margin: .2em auto;
	background-color: #000;
	color: #fff;
}
-->
</style>
<div id="addCartResultPopup">
	<div class="title"><strong>商品をカートへ入れました</strong></div>
	カート画面で、商品数量を<br />
	ご確認の上ご購入ください。<br />
	<br />
	<a href="<%: Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST %>" class="btn">カートを見る</a>
	<a class="btn close-popup" href="javascript:void(0);">閉じる</a>
</div>

</div>

<div class="order-footer">
	<div class="button-next">
		<a href="<%: this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_MYPAGE %>" class="btn">マイページトップへ</a>
	</div>
</div>

</ContentTemplate>
</asp:UpdatePanel>
<%-- UPDATE PANELここまで --%>

</section>
</asp:Content>