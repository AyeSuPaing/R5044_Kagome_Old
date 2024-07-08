<%--
=========================================================================================================
  Module      : スマートフォン用特集ページテンプレート画面(FeaturePageTemplate.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>

<%@ Page Title="" Language="C#" CodeFile="~/Form/PageTemplates/FeaturePageTemplate.aspx.cs" Inherits="Form_PageTemplates_FeaturePageTemplate" MasterPageFile="~/SmartPhone/Form/Common/DefaultPage.master" %>

<%@ Register TagPrefix="uc" TagName="BodyProductArrivalMailRegister" Src="~/SmartPhone/Form/Common/Product/BodyProductArrivalMailRegister.ascx" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%--
下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="NoSide" %><%@ FileInfo LastChanged="最終更新者" %>
--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
	<%-- ▽編集可能領域：HEAD追加部分▽ --%>
	<link rel="stylesheet" href="https://unpkg.com/purecss@1.0.1/build/pure-min.css" integrity="sha384-oAOxQR6DkCoMliIh8yFnu25d7Eq/PHS21PClpwjOTeU2jRSq11vu66rf90/cZr47" crossorigin="anonymous">
	<link rel="stylesheet" href="<%: Constants.PATH_ROOT %>SmartPhone/Css/featurepage.css">
	<%-- △編集可能領域△ --%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<script type="text/javascript">
		<% var serializer = new JavaScriptSerializer(); %>
		var sort = <%= serializer.Serialize(this.Sort) %>
		$(function () {
			sortFeaturePageContents(sort);
			<%-- ▽非表示処理▽ --%>
			<%-- △非表示処理△ --%>
		});
	</script>
	<span>
		最終更新日
		<br>
		<%: DateTimeUtility.ToStringFromRegion(this.ChangedDate, DateTimeUtility.FormatType.ShortDateHourMinute2Letter) %>
	</span>
	<span style="float: right; margin-right: 0;">
		<% if (this.IsPublishDateTo) { %>
			<%: DateTimeUtility.ToStringFromRegion(this.PublishDateTo, DateTimeUtility.FormatType.ShortDateHourMinute2Letter) %>まで公開
		<% } %>
	</span>
	<asp:HiddenField ID="hfCategoryIdIncludeParamName" runat="server" />
	<div id="divTopArea">
		<%-- ▽レイアウト領域：トップエリア▽ --%>
		<%-- △レイアウト領域△ --%>
	</div>
	<h1 id="page-title">
				<asp:Label runat="server" ID="lTitle"></asp:Label></h1>
	<div id="header-banner">
		<img src="" alt='<%: this.AltText %>' />
	</div>
	<div id="upper-contents-area">
		<%-- ▽編集可能領域：コンテンツエリア上部▽ --%>
		<%-- △編集可能領域△ --%>
	</div>
	<asp:UpdatePanel ID="UpdatePanel1" runat="server">
		<ContentTemplate>
			<div id="feature-group-items">
				<%-- ▽グループループ▽ --%>
				<asp:Repeater ID="rFeatureProductGroup" runat="server" ItemType="FeaturePageProductGroup">
					<ItemTemplate>
						<div id="product-list-<%#: Container.ItemIndex %>" class="product-list">
							<div class="product-list-inner">
								<div id="list-title"><%#: Item.Title %></div>
								<%-- ▽商品ループ▽ --%>
								<div class="feature-item-list">
									<asp:Repeater ID="rFeatureProductGroupItem" runat="server" ItemType="FeaturePageProductGroupItem" DataSource="<%# Item.ProductGroupItems %>" OnItemDataBound="rFeatureProductGroupItem_OnItemDataBound" OnItemCommand="ProductMasterList_ItemCommand">
										<ItemTemplate>
											<div class="feature-item">
												<a class="feature-item-link" href='<%#: CreateProductDetailUrl(Item.ProductInfo, Item.Product.HasProductVariation) %>'>
													<div class="feature-item-image">
														<w2c:ProductImage ID="ProductImage1" ImageTagId="productImage" ImageSize="L" IsVariation="<%# Item.Product.HasProductVariation %>" ProductMaster="<%# GetProductData(Item) %>" runat="server" />
														<p visible='<%# ProductListUtility.IsProductSoldOut(Item.Product) %>' runat="server" class="feature-item-sold-out">SOLD OUT</p>
													</div>
													<div class="feature-item-icon">
														<w2c:ProductIcon ID="ProductIcon1" IconNo="1" ProductMaster="<%# Item.Product %>" runat="server" />
														<w2c:ProductIcon ID="ProductIcon2" IconNo="2" ProductMaster="<%# Item.Product %>" runat="server" />
														<w2c:ProductIcon ID="ProductIcon3" IconNo="3" ProductMaster="<%# Item.Product %>" runat="server" />
														<w2c:ProductIcon ID="ProductIcon4" IconNo="4" ProductMaster="<%# Item.Product %>" runat="server" />
														<w2c:ProductIcon ID="ProductIcon5" IconNo="5" ProductMaster="<%# Item.Product %>" runat="server" />
														<w2c:ProductIcon ID="ProductIcon6" IconNo="6" ProductMaster="<%# Item.Product %>" runat="server" />
														<w2c:ProductIcon ID="ProductIcon7" IconNo="7" ProductMaster="<%# Item.Product %>" runat="server" />
														<w2c:ProductIcon ID="ProductIcon8" IconNo="8" ProductMaster="<%# Item.Product %>" runat="server" />
														<w2c:ProductIcon ID="ProductIcon9" IconNo="9" ProductMaster="<%# Item.Product %>" runat="server" />
														<w2c:ProductIcon ID="ProductIcon10" IconNo="10" ProductMaster="<%# Item.Product %>" runat="server" />
													</div>
													<!-- 商品名 -->
													<h3 class="feature-item-name"><%#: CreateProductJointName(Item.Product) %></h3>
													<%-- 商品ID --%>
													<p class="feature-item-product-id" visible="<%# (Item.Product.HasProductVariation == false) %>" runat="server">
														[<%# Item.Product.ProductId %>]
													</p>
													<p class="feature-item-product-id" visible="<%# (Item.Product.HasProductVariation) %>" runat="server">
														[<%# GetKeyValue(Item.ProductInfo, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) %>]
													</p>
												</a>
												<div visible="<%# (Item.Product.HasProductVariation && (ProductListUtility.IsProductSoldOut(Item.Product) == false)) %>" runat="server">
													バリエーション選択<br />
													<asp:DropDownList runat="server" ID="ddlVariationList" OnSelectedIndexChanged="ddlVariationList_OnSelectedIndexChanged" DataTextField="Text" DataValueField="Value" AutoPostBack="True" />
												</div>
												<%-- ▽商品会員ランク価格有効▽ --%>
												<p visible='<%# GetProductMemberRankPriceValid(Item.ProductInfo, true) %>' runat="server" class="feature-item-member-rank-price">
													<%#: CurrencyManager.ToPrice(GetProductMemberRankPrice(Item.ProductInfo, true)) %>
													（<%#: GetTaxIncludeString(Item.ProductInfo) %>）
													<span class="line-through"><%#: CurrencyManager.ToPrice(GetProductPriceNumeric(Item.ProductInfo, true)) %>（<%#: GetTaxIncludeString(Item.ProductInfo) %>）</span>
												</p>
												<%-- △商品会員ランク価格有効△ --%>
												<%-- ▽商品セール価格有効▽ --%>
												<p visible='<%# GetProductTimeSalesValid(Item.ProductInfo, Item.Product.HasProductVariation) %>' runat="server" class="feature-item-sale-price">
													<%#: CurrencyManager.ToPrice(GetProductTimeSalePriceNumeric(Item.ProductInfo)) %>
													（<%#: GetTaxIncludeString(Container.DataItem) %>）
													<span class="line-through"><%#: CurrencyManager.ToPrice(GetProductPriceNumeric(Item.ProductInfo, Item.Product.HasProductVariation)) %>（<%#: GetTaxIncludeString(Item.ProductInfo) %>）</span>
												</p>
												<%-- △商品セール価格有効△ --%>
												<%-- ▽商品特別価格有効▽ --%>
												<p visible='<%# GetProductSpecialPriceValid(Item.ProductInfo, Item.Product.HasProductVariation) %>' runat="server" class="feature-item-special-price">
													<%#: CurrencyManager.ToPrice(GetProductSpecialPriceNumeric(Item.ProductInfo, Item.Product.HasProductVariation)) %>
													（<%#: GetTaxIncludeString(Item.ProductInfo) %>）
													<span class="line-through"><%#: CurrencyManager.ToPrice(GetProductPriceNumeric(Item.ProductInfo, Item.Product.HasProductVariation)) %>（<%#: GetTaxIncludeString(Item.ProductInfo) %>）</span>
												</p>
												<%-- △商品特別価格有効△ --%>
												<%-- ▽商品通常価格有効▽ --%>
												<p visible='<%# GetProductNormalPriceValid(Item.ProductInfo, Item.Product.HasProductVariation) %>' runat="server" class="feature-item-price">
													<%#: CurrencyManager.ToPrice(GetProductPriceNumeric(Item.ProductInfo, Item.Product.HasProductVariation)) %>
													（<%#: GetTaxIncludeString(Item.ProductInfo) %>）
												</p>
												<%-- △商品通常価格有効△ --%>
												<%-- ▽商品加算ポイント▽ --%>
												<p visible='<%# (this.IsLoggedIn && (GetProductAddPointString(Item.ProductInfo, Item.Product.HasProductVariation) != string.Empty)) %>' runat="server" class="feature-item-add-point">
													ポイント<%#: GetProductAddPointString(Item.ProductInfo, Item.Product.HasProductVariation) %>
												</p>
												<%-- △商品加算ポイント△ --%>
												<%-- ▽定期購入価格有効▽ --%>
												<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
												<span visible='<%# ((string)GetKeyValue(Item.ProductInfo, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG) != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) && ((CheckFixedPurchaseLimitedUserLevel(this.ShopId, Item.Product.ProductId) == false)) %>' runat="server" class="feature-item-fixed-purchase-price">
													<p visible='<%# IsProductFixedPurchaseFirsttimePriceValid(Item.ProductInfo, Item.Product.HasProductVariation) %>' runat="server">
														定期初回価格:<%#: CurrencyManager.ToPrice(GetProductFixedPurchaseFirsttimePrice(Item.ProductInfo, Item.Product.HasProductVariation)) %>（<%#: GetTaxIncludeString(Item.ProductInfo) %>）
														<br />
													</p>
													定期通常価格:<%#: CurrencyManager.ToPrice(GetProductFixedPurchasePrice(Item.ProductInfo, Item.Product.HasProductVariation)) %>（<%#: GetTaxIncludeString(Item.ProductInfo) %>）
												</span>
												<% } %>
												<%-- △定期購入価格有効△ --%>
												<%-- ▽定期商品加算ポイント▽ --%>
												<p visible='<%# (this.IsLoggedIn && (GetProductAddPointString(Item.ProductInfo, Item.Product.HasProductVariation, true, true) != "") && (((bool)GetKeyValue(Item.ProductInfo, "CanFixedPurchase")) && ((CheckFixedPurchaseLimitedUserLevel(this.ShopId, (string)GetProductData(Item.ProductInfo, "product_id")) == false)))) %>' runat="server" class="feature-item-fixed-purchase-point">
													<span class="addPoint">ポイント<%#: GetProductAddPointString(Item.ProductInfo, Item.Product.HasProductVariation, false, true) %></span>
													<span visible='<%# (string)GetKeyValue(Item.ProductInfo, Constants.FIELD_PRODUCT_POINT_KBN2) == Constants.FLG_PRODUCT_POINT_KBN1_RATE %>' runat="server">(<%#: GetProductAddPointCalculateAfterString(Item.ProductInfo, Item.Product.HasProductVariation, false, true)%>)
													</span>
												</p>
												<%-- △定期商品加算ポイント△ --%>
												<%-- ▽お気に入りの登録人数表示▽ --%>
												<p class="feature-item-favorite-registration">お気に入りの登録人数：<%# SetFavoriteDataForDisplay((string)GetKeyValue(Item.ProductInfo, Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID), (string)GetKeyValue(Item.ProductInfo, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>人</p>
												<%-- △お気に入りの登録人数表示△ --%>
												<%-- ▽セットプロモーション情報▽ --%>
												<asp:Repeater ID="rSetPromotion" DataSource="<%# GetSetPromotionByProduct(Item.Product) %>" runat="server">
													<HeaderTemplate>
														<div class="product-set-promotion">
													</HeaderTemplate>
													<ItemTemplate>
														<p>
															<%#: ((SetPromotionModel)Container.DataItem).SetpromotionDispName%>
														</p>
													</ItemTemplate>
													<FooterTemplate>
														</div>
													</FooterTemplate>
												</asp:Repeater>
												<%-- △セットプロモーション情報△ --%>

												<%-- カート投入ボタン --%>
												<p class="addCart">
													<%-- カートに入れるボタン表示 --%>
													<div class="mb5 feature-vari-cart-add">
														<asp:LinkButton ID="lbCartAddVariationList" runat="server" Visible='<%# (bool)GetKeyValue(Item.ProductInfo, "CanCart") %>' CommandName="CartAdd" class="btn btn-mid btn-inverse">
															カートに入れる
														</asp:LinkButton>
													</div>
													<%-- 定期購入ボタン表示 --%>
													<div class="feature-vari-cart-add-fixed-purchase">
														<asp:LinkButton ID="lbCartAddFixedPurchaseVariationList" runat="server" Visible='<%# (((bool)GetKeyValue(Item.ProductInfo, "CanFixedPurchase")) && (this.IsUserFixedPurchaseAble)) %>' OnClientClick="return add_cart_check_for_fixedpurchase_variationlist();" CommandName="CartAddFixedPurchase" class="btn btn-mid btn-inverse">
															カートに入れる(定期購入)
														</asp:LinkButton>
														<span id="Span4" runat="server" visible='<%# ((bool)GetKeyValue(Item.ProductInfo, "CanFixedPurchase")) && ((string)GetKeyValue(Item.ProductInfo, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG) == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY) && (this.IsUserFixedPurchaseAble == false) %>' style="color: red;">定期購入のご利用はできません</span>
													</div>
													<%-- ギフト購入ボタン表示 --%>
													<div class="feature-vari-cart-add-gift">
														<asp:LinkButton ID="lbCartAddForGiftVariationList" runat="server" Visible='<%# (bool)GetKeyValue(Item.ProductInfo, "CanGiftOrder") %>' CommandName="CartAddGift" class="btn btn-mid btn-inverse">
															カートに入れる(ギフト購入)
														</asp:LinkButton>
													</div>
												</p>
												<%-- 入荷通知メールボタン --%>
												<p class="arrivalMailButton">
													<%-- 再入荷通知メール申し込みボタン表示 --%>
													<div visible='<%# ((string)GetKeyValue(Item.ProductInfo, "ArrivalMailKbn") == Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL) %>' runat="server" class="feature-vari-arrival">
														<asp:LinkButton CommandName="SmartArrivalMail" CommandArgument="Arrival" runat="server" class="btn btn-mid btn-inverse">
															入荷お知らせメール申込
														</asp:LinkButton>
													</div>
													<%-- 販売開始通知メール申し込みボタン表示 --%>
													<div visible='<%# ((string)GetKeyValue(Item.ProductInfo, "ArrivalMailKbn") == Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE) %>' runat="server" class="feature-vari-release">
														<asp:LinkButton CommandName="SmartArrivalMail" CommandArgument="Release" runat="server" class="btn btn-mid btn-inverse">
															販売開始通知メール申込
														</asp:LinkButton>
													</div>
													<%-- 再販売通知メール申し込みボタン表示 --%>
													<div visible='<%# ((string)GetKeyValue(Item.ProductInfo, "ArrivalMailKbn") == Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE) %>' runat="server" class="feature-vari-resale">
														<asp:LinkButton CommandName="SmartArrivalMail" CommandArgument="Resale" runat="server" class="btn btn-mid btn-inverse">
															再販売通知メール申込
														</asp:LinkButton>
													</div>
												</p>
												<%-- 隠しフィールド --%>
												<asp:HiddenField ID="hfProductId" Value="<%# GetKeyValue(Item.ProductInfo, Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID) %>" runat="server" />
												<asp:HiddenField ID="hfVariationId" Value="<%# GetKeyValue(Item.ProductInfo, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) %>" runat="server" />
												<asp:HiddenField ID="hfArrivalMailKbn" Value='<%# GetKeyValue(Item.ProductInfo, "ArrivalMailKbn") %>' runat="server" />
												<div class="feature-vari-arrival-mail">
													<%-- 再入荷通知メール登録フォーム表示 --%>
													<uc:BodyProductArrivalMailRegister runat="server" ID="ucBpamrArrival" ArrivalMailKbn="<%#: Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL %>" ProductId="<%#: GetKeyValue(Item.ProductInfo, Constants.FIELD_PRODUCT_PRODUCT_ID) %>" VariationId="<%#: GetKeyValue(Item.ProductInfo, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) %>" Visible="false" />
													<%-- 販売開始通知メール登録フォーム表示 --%>
													<uc:BodyProductArrivalMailRegister runat="server" ID="ucBpamrRelease" ArrivalMailKbn="<%#: Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE %>" ProductId="<%#: GetKeyValue(Item.ProductInfo, Constants.FIELD_PRODUCT_PRODUCT_ID) %>" VariationId="<%#: GetKeyValue(Item.ProductInfo, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) %>" Visible="false" />
													<%-- 再販売知メール登録フォーム表示 --%>
													<uc:BodyProductArrivalMailRegister runat="server" ID="ucBpamrResale" ArrivalMailKbn="<%#: Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE %>" ProductId="<%#: GetKeyValue(Item.ProductInfo, Constants.FIELD_PRODUCT_PRODUCT_ID) %>" VariationId="<%#: GetKeyValue(Item.ProductInfo, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) %>" Visible="false" />
												</div>

												<% if (Constants.VARIATION_FAVORITE_CORRESPONDENCE){ %>
												<div class="favorite-btn">
													<asp:LinkButton ID="favoriteVariation" runat="server" CommandName="FavoriteAdd" class="btn btn-mid" OnClientClick=<%# (Alertdisplaycheck(this.ShopId, this.LoginUserId, (string)GetKeyValue(Item.ProductInfo, Constants.FIELD_FAVORITE_PRODUCT_ID), (string)GetKeyValue(Item.ProductInfo, Constants.FIELD_FAVORITE_VARIATION_ID))) ? "display_alert_check_for_mailsend()" : "" %>>
														<%# (FavoriteDisplayWord(this.ShopId, this.LoginUserId, (string)GetKeyValue(Item.ProductInfo, Constants.FIELD_FAVORITE_PRODUCT_ID), (string)GetKeyValue(Item.ProductInfo, Constants.FIELD_FAVORITE_VARIATION_ID))) ? "お気に入り登録済み" : "お気に入りに追加" %>
														(<%# SetFavoriteDataForDisplay((string)GetKeyValue(Item.ProductInfo, Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID), (string)GetKeyValue(Item.ProductInfo, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>人)
													</asp:LinkButton>
												</div>
												<% } %>
											</div>
										</ItemTemplate>
									</asp:Repeater>
								</div>
								<%-- △商品ループ△ --%>
								<br />
								<a href='<%#: Item.ViewMore %>' runat="server" visible='<%# string.IsNullOrEmpty(Item.ViewMore) == false %>'>もっとみる</a>
							</div>
						</div>
					</ItemTemplate>
				</asp:Repeater>
				<%-- △グループループ△ --%>
				<%-- ▽ページ送り▽ --%>
				<div id="pagination" class="below clearFix"><%= this.PagerHtml %></div>
				<%-- △ページ送り△ --%>
			</div>
		</ContentTemplate>
	</asp:UpdatePanel>
	<div id="lower-contents-area">
		<%-- ▽編集可能領域：コンテンツエリア下部▽ --%>
		<%-- △編集可能領域△ --%>
	</div>
	<div id="divBottomArea">
		<%-- ▽レイアウト領域：ボトムエリア▽ --%>
		<%-- △レイアウト領域△ --%>
	</div>
	<script type="text/javascript" language="javascript">
		// マウスイベントの初期化
		addOnload(function () { init(); });
	</script>
</asp:Content>
