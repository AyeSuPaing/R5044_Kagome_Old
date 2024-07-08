<%--
=========================================================================================================
  Module      : お気に入り一覧画面(FavoriteList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="BodyProductVariationImages" Src="~/Form/Common/Product/BodyProductVariationImages.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductRecommendByRecommendEngine" Src="~/Form/Common/Product/BodyProductRecommendByRecommendEngine.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/Product/FavoriteList.aspx.cs" Inherits="Form_Product_FavoriteList" Title="お気に入りページ" %>
<%@ Import Namespace="System.ComponentModel" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<link href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "Css/product.css")%>" rel="stylesheet" type="text/css" media="all" />
<%-- △編集可能領域△ --%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserFltContents">
		<h2>お気に入りリスト</h2>
	<div id="dvFavoriteList" class="unit">
		<h4>お気に入りリストにストックしている一覧です。</h4>
		<!-- ///// ページャ ///// -->
		<div id="pagination" class="above clearFix"><%= this.PagerHtml %></div>
		<!-- ///// お気に入りリスト一覧 ///// -->
			<asp:Repeater id="rFavoriteList" runat="server">
				<HeaderTemplate>
					<table cellspacing="0">
						<tr>
							<th class="productImage"></th>
							<th class="productPatternNum">商品ID</th>
							<th class="productName">商品名</th>
							<th>&nbsp;</th>
							<th class="delete">&nbsp;</th>
						</tr>
				</HeaderTemplate>
				<ItemTemplate>
					<tr>
						<td class="productImage">
						<div class="favoriteProductImage">
							<% if(Constants.LAYER_DISPLAY_VARIATION_IMAGES_ENABLED){ %>
							<uc:BodyProductVariationImages ImageSize="M" ProductMaster="<%# Container.DataItem %>" VariationList="<%# this.ProductVariationList %>" VariationNo="<%# Container.ItemIndex.ToString() %>" runat="server" />
							<% } else { %>
							<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem, StringUtility.ToEmpty(GetKeyValueToNull(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)))) %>">
							<w2c:ProductImage ImageSize="M" ProductMaster="<%# Container.DataItem %>" IsVariation="false" runat="server" /></a>
							<% } %>
						</div>
						</td>
						<td class="productPatternNum">
							<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem, StringUtility.ToEmpty(GetKeyValueToNull(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)))) %>">
								<%# WebSanitizer.HtmlEncode(Eval(FavoriteDisplayId(Container.DataItem))) %>
							</a>
						</td>
						<td class="productName">
							<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem, StringUtility.ToEmpty(GetKeyValueToNull(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)))) %>">
								<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCT_NAME)) %></a>
						</td>
						<td>
							<!-- カートに入れるボタン -->
							<div class ="favorite-addcart">
								<div visible='<%#CheckAddCart(Container.DataItem) %>' runat="server">
									<asp:LinkButton id="lbCartAdd" Text="カートに入れる" class="btn btn-mini btn-inverse" CommandArgument='<%# Eval(Constants.FIELD_FAVORITE_PRODUCT_ID)+";" + Eval(Constants.FIELD_FAVORITE_VARIATION_ID)+ ";" + Eval(Constants.FIELD_FAVORITE_SHOP_ID) %>' OnClick="lbCartAdd_Click" runat="server" visible="<%# this.CanAddCart %>"></asp:LinkButton>
									<asp:LinkButton id="lbCartAddFixedPurchase" Text="カートに入れる(定期購入)" class="btn btn-mini btn-inverse" CommandArgument='<%# Eval(Constants.FIELD_FAVORITE_PRODUCT_ID)+";" + Eval(Constants.FIELD_FAVORITE_VARIATION_ID)+ ";" + Eval(Constants.FIELD_FAVORITE_SHOP_ID) %>' OnClick="lbCartAddFixedPurchase_Click" runat="server" visible="<%# this.CanFixedPurchase %>" OnClientClick="return add_cart_check_for_fixedpurchase();"></asp:LinkButton>
									<asp:LinkButton id="lbCartAddForGift" Text="カートに入れる(ギフト購入)" class="btn btn-mini btn-inverse" CommandArgument='<%# Eval(Constants.FIELD_FAVORITE_PRODUCT_ID)+";" + Eval(Constants.FIELD_FAVORITE_VARIATION_ID)+ ";" + Eval(Constants.FIELD_FAVORITE_SHOP_ID) %>' OnClick="lbCartAddGift_Click" runat="server" visible="<%# this.CanGiftOrder%>"></asp:LinkButton>
								</div>
								<div Visible='<%# this.StockDisplay %>' runat="server">
									<div visible='<%# (this.StockMessage == "") %>' runat="server">
										<p>在庫数量：<strong><%#: GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTSTOCK_STOCK) %></strong></p>
									</div>
									<div visible='<%# (this.StockMessage != "") %>' runat="server">
										<p><%#: this.StockMessage %></p>
									</div>
								</div>
							</div>
						</td>
						<td class="delete">
							<!-- ///// お気に入りから削除 ///// -->
							<asp:LinkButton id="lbDelete" Text="削除" CssClass="btn btn-mini" CommandArgument='<%# Eval(Constants.FIELD_FAVORITE_PRODUCT_ID)+";" + Eval(Constants.FIELD_FAVORITE_VARIATION_ID) %>' OnClientClick="return confirm('本当に削除してもよろしいですか？')" OnClick="lbDelete_Click" runat="server"></asp:LinkButton>
						</td>
					</tr>
				</ItemTemplate>
				<FooterTemplate>
					</table>
				</FooterTemplate>
			</asp:Repeater>
		<%-- エラーメッセージ --%>
		<% if (StringUtility.ToEmpty(this.ErrorMessage) != ""){ %>
			<p><%= this.ErrorMessage %></p>
		<% } %>

		<!-- ///// ページャ ///// -->
		<div id="pagination" class="below clearFix"><%= this.PagerHtml %></div>
		
		<uc:BodyProductRecommendByRecommendEngine runat="server" RecommendCode="pc911" RecommendTitle="おすすめ商品一覧" MaxDispCount="4" DispCategoryId="" NotDispCategoryId="" NotDispRecommendProductId="" />
	</div>
</div>
<script type="text/javascript">
	function add_cart_check_for_fixedpurchase() {
		var fixedpurchaseMessage = '定期的に商品をお送りする「定期購入」で購入します。\nよろしいですか？';
		return confirm(fixedpurchaseMessage);
	}
</script>
	</asp:Content>