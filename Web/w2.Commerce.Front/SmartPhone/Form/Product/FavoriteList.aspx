<%--
=========================================================================================================
  Module      : スマートフォン用お気に入り一覧画面(FavoriteList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%@ Register TagPrefix="uc" TagName="BodyProductRecommendByRecommendEngine" Src="~/SmartPhone/Form/Common/Product/BodyProductRecommendByRecommendEngine.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/Product/FavoriteList.aspx.cs" Inherits="Form_Product_FavoriteList" Title="お気に入りページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="wrap-user favorite-list">
<div class="user-unit">
	<h2>お気に入りリスト</h2>
	<p class=msg>お気に入りリストにストックしている一覧です。</p>

	<%-- ページャ --%>
	<div class="pager-wrap above"><%= this.PagerHtml %></div>

	<!-- お気に入りリスト一覧 -->
	<asp:Repeater id="rFavoriteList" runat="server">
		<HeaderTemplate>
		</HeaderTemplate>
		<ItemTemplate>
		<table class="cart-table">
		<tbody>
			<tr class="cart-unit-product">
				<td class="product-image">
					<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem, StringUtility.ToEmpty(GetKeyValueToNull(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)))) %>'>
						<w2c:ProductImage ID="ProductImage1" ImageSize="M" ProductMaster="<%# Container.DataItem %>" IsVariation="false" runat="server" />
					</a>
				</td>
				<td class="product-info">
					<ul>
						<li class="product-name">
						<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem, StringUtility.ToEmpty(GetKeyValueToNull(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)))) %>">
							<%# WebSanitizer.HtmlEncode(Eval(FavoriteDisplayId(Container.DataItem))) %><br />
							<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCT_NAME)) %>
						</a>
						</li>
					</ul>
				</td>
				<td class="product-control">
					<div class="delete">
						<asp:LinkButton id="lbDelete" Text="削除" CssClass="btn" CommandArgument='<%# Eval(Constants.FIELD_FAVORITE_PRODUCT_ID)+";" + Eval(Constants.FIELD_FAVORITE_VARIATION_ID) %>' OnClientClick="return confirm('本当に削除してもよろしいですか？')" OnClick="lbDelete_Click" runat="server"></asp:LinkButton>
					</div>
				</td>
			</tr>
		</tbody>
		</table>
			<!-- カートに入れるボタン -->
			<div class="favorite-addcart" >
				<div visible='<%#CheckAddCart(Container.DataItem) %>' runat="server">
				<asp:LinkButton id="lbCartAdd" Text="カートに入れる" class="favorite-addcart btn btn-mini btn-inverse" CommandArgument='<%# Eval(Constants.FIELD_FAVORITE_PRODUCT_ID)+";" + Eval(Constants.FIELD_FAVORITE_VARIATION_ID)+ ";" + Eval(Constants.FIELD_FAVORITE_SHOP_ID) %>' OnClick="lbCartAdd_Click" runat="server" visible="<%# this.CanAddCart %>"></asp:LinkButton>
				<asp:LinkButton id="lbCartAddFixedPurchase" Text="カートに入れる(定期購入)" class="favorite-addcart btn btn-mini btn-inverse" CommandArgument='<%# Eval(Constants.FIELD_FAVORITE_PRODUCT_ID)+";" + Eval(Constants.FIELD_FAVORITE_VARIATION_ID)+ ";" + Eval(Constants.FIELD_FAVORITE_SHOP_ID) %>' OnClick="lbCartAddFixedPurchase_Click" runat="server" visible="<%# this.CanFixedPurchase%>" OnClientClick="return add_cart_check_for_fixedpurchase();"></asp:LinkButton>
				<asp:LinkButton id="lbCartAddForGift" Text="カートに入れる(ギフト購入)" class="favorite-addcart btn btn-mini btn-inverse" CommandArgument='<%# Eval(Constants.FIELD_FAVORITE_PRODUCT_ID)+";" + Eval(Constants.FIELD_FAVORITE_VARIATION_ID)+ ";" + Eval(Constants.FIELD_FAVORITE_SHOP_ID) %>' OnClick="lbCartAddGift_Click" runat="server" visible="<%# this.CanGiftOrder%>"></asp:LinkButton>
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
		</ItemTemplate>
		<FooterTemplate>
		</FooterTemplate>
	</asp:Repeater>
	<%-- エラーメッセージ --%>
	<% if (StringUtility.ToEmpty(this.ErrorMessage) != ""){ %>
		<div class="msg-alert"><%= this.ErrorMessage %></div>
	<% } %>

	<%-- ページャ --%>
	<div class="pager-wrap below"><%= this.PagerHtml %></div>

</div>

<div class="user-footer">
	<div class="button-next">
		<a href="<%= WebSanitizer.HtmlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_MYPAGE) %>" class="btn">マイページトップへ</a>
	</div>
</div>

</section>
<script>
	function add_cart_check_for_fixedpurchase() {
		var fixedpurchaseMessage = '定期的に商品をお送りする「定期購入」で購入します。\nよろしいですか？';
		return confirm(fixedpurchaseMessage);
	}
</script>
</asp:Content>