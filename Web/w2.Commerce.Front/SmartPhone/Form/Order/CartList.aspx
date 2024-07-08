<%--
=========================================================================================================
  Module      : スマートフォン用カート画面(CartList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Register TagPrefix="uc" TagName="BodyAnnounceFreeShipping" Src="~/SmartPhone/Form/Common/BodyAnnounceFreeShipping.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductRecommendByRecommendEngine" Src="~/SmartPhone/Form/Common/Product/BodyProductRecommendByRecommendEngine.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/Order/CartList.aspx.cs" Inherits="Form_Order_CartList" Title="ショッピングカートページ" %>
<%@ Register TagPrefix="uc" TagName="Criteo" Src="~/Form/Common/Criteo.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaypalScriptsForm" Src="~/Form/Common/PayPalScriptsForm.ascx" %>
<%@ Register TagPrefix="uc" TagName="AffiliateTag" Src="~/Form/Common/AffiliateTag.ascx" %>
<%@ Import Namespace="w2.Domain.Coupon.Helper" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<%-- ▽▽Amazonペイメントを使う場合はウィジェットを配置するページは必ずSSLでなければいけない▽▽ --%>
<script runat="server">
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }
</script>
<%-- △△Amazonペイメントを使う場合はウィジェットを配置するページは必ずSSLでなければいけない△△ --%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>

<%-- △編集可能領域△ --%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="divTopArea">
<%-- ▽レイアウト領域：トップエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<uc:AffiliateTag ID="AffiliateTagFree" Location="free" runat="server"/>
<section class="wrap-cart">

<%if (this.CartList.Items.Count == 0) {%>
	<div class="msg-alert">カートに商品がありません</div>
<%} %>

<%-- 次へイベント用リンクボタン（イベント作成用。通常はUpdatePanel内部に設置する） --%>
<asp:LinkButton ID="lbNext" OnClick="lbNext_Click" ValidationGroup="OrderShipping" runat="server"></asp:LinkButton>
<% if (string.IsNullOrEmpty(this.DispErrorMessage) == false) { %>
	<span style="padding: 10px 10px; display: inline-block; color: red"><%= WebSanitizer.HtmlEncodeChangeToBr(this.DispErrorMessage) %></span>
<% } %>

<%if (this.CartList.Items.Count != 0) {%>

<div class="step">
	<img src="<%= Constants.PATH_ROOT %>SmartPhone/Contents/ImagePkg/common/cart-step01.jpg" alt="カート内容の確認" width="320" />
</div>

<%-- カート毎に表示する --%>
<asp:Repeater id="rCartList" Runat="server" OnItemCommand="rCartList_ItemCommand">
<HeaderTemplate>
	<w2c:FacebookConversionAPI
		EventName="AddToCart"
		CartList="<%# this.CartList %>"
		UserId="<%#: this.LoginUserId %>"
		runat="server" />
</HeaderTemplate>
<ItemTemplate>
<asp:PlaceHolder ID="phSubscriptionBoxErrorMsg" runat="server" Visible="<%# string.IsNullOrEmpty(((CartObject)Container.DataItem).SubscriptionBoxErrorMsg) == false %>">
	<span style="color:red"><%# ((CartObject)Container.DataItem).SubscriptionBoxErrorMsg %></span>
</asp:PlaceHolder>
<asp:HiddenField ID="hfSubscriptionBoxCourseId" runat="server" Value="<%# ((CartObject)Container.DataItem).SubscriptionBoxCourseId %>" />
<%if (Constants.ProductOrderLimitKbn.ProductOrderLimitOn == Constants.PRODUCT_ORDER_LIMIT_KBN_CAN_BUY) { %>
<div runat="server" visible="<%# ((CartObject)Container.DataItem).HasNotFirstTimeOrderIdList %>" style="text-align:left;position:relative;bottom:30px;">
</div>
<%} %>

<%if (Constants.CARTCOPY_OPTION_ENABLED && (Constants.GIFTORDER_OPTION_WITH_SHORTENING_GIFT_OPTION_ENABLED == false)) { %>
<%-- ▼カート削除完了メッセージ▼ --%>
<div runat="server" visible="<%# ((CartObject)Container.DataItem).IsCartDeleteCompleteMesseges %>">
	<span>カートの削除が完了しました。</span>
</div>
<%-- ▲カート削除完了メッセージ▲ --%>
<%} %>

<div class="cart-unit">
	<h2><%# this.CartList.Items.Count > 1 ? "カート番号" + (Container.ItemIndex + 1).ToString() + "の" : "" %>ご注文内容</h2>
		<%-- カート内商品を表示する --%>
		<%-- 通常商品 --%>
		<asp:Repeater id="rCart" runat="server" DataSource='<%# (CartObject)Container.DataItem %>' OnItemCommand="CallCartList_ItemCommand" OnItemDataBound="rCartList_ItemDataBound">
		<HeaderTemplate>
			<table class="cart-table">
			<tbody>
		</HeaderTemplate>
		<ItemTemplate>
			<tr class="cart-unit-product" visible="<%# ((CartProduct)Container.DataItem).IsSetItem == false && ((CartProduct)Container.DataItem).QuantitiyUnallocatedToSet != 0 %>" runat="server">
				<td class="product-image">
					<%-- 隠し値 --%>
					<asp:HiddenField ID="hfShopId" runat="server" Value="<%# ((CartProduct)Container.DataItem).ShopId %>" />
					<asp:HiddenField ID="hfProductId" runat="server" Value="<%# ((CartProduct)Container.DataItem).ProductId %>" />
					<asp:HiddenField ID="hfVariationId" runat="server" Value="<%# ((CartProduct)Container.DataItem).VariationId %>" />
					<asp:HiddenField ID="hfIsFixedPurchase" runat="server" Value="<%# ((CartProduct)Container.DataItem).IsFixedPurchase %>" />
					<asp:HiddenField ID="hfAddCartKbn" runat="server" Value="<%# ((CartProduct)Container.DataItem).AddCartKbn %>" />
					<asp:HiddenField ID="hfProductSaleId" runat="server" Value="<%# ((CartProduct)Container.DataItem).ProductSaleId %>" />
					<asp:HiddenField ID="hfProductOptionValue" runat="server" Value='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues() %>' />
					<asp:HiddenField ID="hfUnallocatedQuantity" runat="server" Value='<%# ((CartProduct)Container.DataItem).QuantitiyUnallocatedToSet %>' />
					<asp:HiddenField ID="hfSubscriptionBoxCourseId" runat="server" Value="<%# ((CartProduct)Container.DataItem).SubscriptionBoxCourseId %>" />
					<input type="hidden" id="hfProductName" value="<%# ((CartProduct)Container.DataItem).ProductName %>" disabled />
					<asp:HiddenField ID="hfSubscriptionBoxCourseName" runat="server" Value="<%# GetSubscriptionBoxDisplayName(((CartProduct)Container.DataItem).SubscriptionBoxCourseId) %>" />
					<asp:HiddenField ID="hfIsSubscriptionBox" runat="server" Value="<%# ((CartProduct)Container.DataItem).IsSubscriptionBox %>" />
					<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
						<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" /></a>
					<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false %>" />
				</td>
				<td class="product-info">
					<ul>
						<li class="product-name">
							<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
								<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %></a>
							<%# (((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false) ? WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) : "" %>
							<%# (((CartProduct)Container.DataItem).GetProductTag("tag_cart_product_message").Length != 0) ? "<p class=\"product-msg\">" + WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).GetProductTag("tag_cart_product_message")) + "</p>" : "" %>
						</li>
						<li visible='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList.IsSelectedProductOptionValueAll %>' runat="server">
							<asp:Repeater ID="rProductOptionSettings" DataSource='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList %>' runat="server">
								<ItemTemplate>
									<%# WebSanitizer.HtmlEncode(((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue()) %>
									<%# (((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue() != "") ? "<br />" : "" %>
								</ItemTemplate>
							</asp:Repeater>
						</li>
						<li class="product-price" Visible="<%# (((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false) && (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false) %>" runat="server">
							<%#: ProductOptionSettingHelper.ToDisplayProductOptionPriceAndPrefix((CartProduct)Container.DataItem, this.ProductPriceTextPrefix) %>
						</li>
						<li visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
							<span style="color:red;">※配送料無料適用外商品です</span>
						</li>
					</ul>
					<p class="attention" visible="<%# this.ErrorMessages.HasMessages(((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex) %>" runat="server">
						<%# WebSanitizer.HtmlEncode(this.ErrorMessages.Get(((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex)) %>
					</p>
				</td>

				<td class="product-control">
					<div class="amout">
					<span visible='<%# IsChangeProductCount((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem, (CartProduct)Container.DataItem) %>' runat="server">
						<w2c:ExtendedTextBox ID="tbProductCount" Type="tel" Runat="server" Text='<%# ((CartProduct)Container.DataItem).QuantitiyUnallocatedToSet %>' MaxLength="3"></w2c:ExtendedTextBox>
					</span>
					<span visible='<%# IsChangeProductCount((CartObject)((RepeaterItem)Container.Parent.Parent).DataItem, (CartProduct)Container.DataItem) == false %>' runat="server" style="vertical-align:text-bottom;">
						<p>数量：<%# StringUtility.ToNumeric(((CartProduct)Container.DataItem).QuantitiyUnallocatedToSet) %></p>
					</span>
					</div>
					<div class="delete">
						<asp:LinkButton ID="lbDeleteProduct" CommandName="DeleteProduct" Visible="<%# HasNecessaryProduct(((CartProduct)Container.DataItem).SubscriptionBoxCourseId, ((CartProduct)Container.DataItem).ProductId) == false %>" Runat="server" CssClass="btn">
							削除
						</asp:LinkButton>
						<asp:LinkButton ID="lbDeleteNecessaryProduct" OnClick="lbDeleteCart_Click" CommandArgument="<%# ((IDataItemContainer)((RepeaterItem)Container.Parent.Parent)).DisplayIndex %>" OnClientClick="return delete_product_check_for_subscriptionBox(this);" Visible="<%# HasNecessaryProduct(((CartProduct)Container.DataItem).SubscriptionBoxCourseId, ((CartProduct)Container.DataItem).ProductId) %>" Runat="server" CssClass="btn">
							削除
						</asp:LinkButton>
					</div>
				</td>
			</tr>
			<%-- セット商品 --%>
			<div visible="<%# (((CartProduct)Container.DataItem).IsSetItem) && (((CartProduct)Container.DataItem).ProductSetItemNo == 1) %>" runat="server">
				<%-- 隠し値 --%>
				<asp:HiddenField ID="hfIsSetItem" runat="server" Value="<%# ((CartProduct)Container.DataItem).IsSetItem %>" />
				<asp:HiddenField ID="hfProductSetId" runat="server" Value="<%# GetProductSetId((CartProduct)Container.DataItem) %>" />
				<asp:HiddenField ID="hfProductSetNo" runat="server" Value="<%# GetProductSetNo((CartProduct)Container.DataItem) %>" />
				<asp:HiddenField ID="hfProductSetItemNo" runat="server" Value="<%# ((CartProduct)Container.DataItem).ProductSetItemNo %>" />
				<div>
					<asp:Repeater id="rProductSet" DataSource="<%# (((CartProduct)Container.DataItem).ProductSet != null) ? ((CartProduct)Container.DataItem).ProductSet.Items : null %>" OnItemCommand="rCartList_ItemCommand" runat="server">
					<ItemTemplate>
						<tr class="cart-unit-product">
							<td class="product-image">
								<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
									<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" />
								</a>
								<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false %>" />
							</td>
							<td class="product-info">
								<span>
									<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
										<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %> x <%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).CountSingle) %>
									</a>
									<%# (((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false) ? WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) + " x " + WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).CountSingle) : ""%>
									<%# (((CartProduct)Container.DataItem).GetProductTag("tag_cart_product_message").Length != 0) ? "<br/><p class=\"message\">" + WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).GetProductTag("tag_cart_product_message")) + "</p>" : "" %>
								</span>
								<p class="product-price" Visible="<%# ((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false %>" runat="server"><%#: CurrencyManager.ToPrice(((CartProduct)Container.DataItem).Price) %>(<%#: this.ProductPriceTextPrefix %>)</p>
								<p visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
									<span style="color:red;">※配送料無料適用外商品です</span>
								</p>
							</td>
							<td class="product-control" visible="<%# (((CartProduct)Container.DataItem).ProductSetItemNo == 1) %>" rowspan="<%# (((CartProduct)Container.DataItem).ProductSet != null) ? ((CartProduct)Container.DataItem).ProductSet.Items.Count : 1 %>" runat="server">
								<div class="amout">
									<asp:TextBox ID="tbProductSetCount" Runat="server" Text='<%# GetProductSetCount((CartProduct)Container.DataItem) %>' MaxLength="3" CssClass="orderCount"></asp:TextBox>
								</div>
								<div class="product-price" style="text-align:center;" Visible="<%# ((CartProduct)Container.DataItem).IsSubscriptionBoxFixedAmount() == false %>" runat="server">
									<%#: CurrencyManager.ToPrice(GetProductSetPriceSubtotal((CartProduct)Container.DataItem)) %>(<%#: this.ProductPriceTextPrefix %>)
								</div>
								<div class="delete">
									<asp:LinkButton ID="lbDeleteProductSet" CommandName="DeleteProductSet" CommandArgument='' Runat="server" CssClass="btn">削除</asp:LinkButton>
								</div>
							</td>
						</tr>
					</ItemTemplate>
					</asp:Repeater>
				</div>
				<tr runat="server" visible="<%# this.ErrorMessages.HasMessages(((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex) %>">
					<td colspan="2">
						<p class="attention">
							<%# WebSanitizer.HtmlEncode(this.ErrorMessages.Get(((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex)) %>
						</p>
					</td>
					<td class="product-control"></td>
				</tr>
			</div>
		</ItemTemplate>
		<FooterTemplate>
			</tbody>
			</table>
		</FooterTemplate>
		</asp:Repeater>

		<%-- セットプロモーション --%>
		<asp:Repeater ID="rCartSetPromotion" DataSource="<%# ((CartObject)Container.DataItem).SetPromotions %>" runat="server">
		<ItemTemplate>

		<div class="cart-set-promotion-unit">

		<asp:HiddenField ID="hfCartSetPromotionNo" runat="server" Value="<%# ((CartSetPromotion)Container.DataItem).CartSetPromotionNo %>" />

			<asp:Repeater ID="rCartSetPromotionItem" DataSource="<%# ((CartSetPromotion)Container.DataItem).Items %>" OnItemCommand="rCartList_ItemCommand" runat="server">
			<HeaderTemplate>
				<table class="cart-table">
				<tbody>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td class="product-image">
					<%-- 隠し値 --%>
					<asp:HiddenField ID="hfShopId" runat="server" Value="<%# ((CartProduct)Container.DataItem).ShopId %>" />
					<asp:HiddenField ID="hfProductId" runat="server" Value="<%# ((CartProduct)Container.DataItem).ProductId %>" />
					<asp:HiddenField ID="hfVariationId" runat="server" Value="<%# ((CartProduct)Container.DataItem).VariationId %>" />
					<asp:HiddenField ID="hfIsFixedPurchase" runat="server" Value="<%# ((CartProduct)Container.DataItem).IsFixedPurchase %>" />
					<asp:HiddenField ID="hfAddCartKbn" runat="server" Value="<%# ((CartProduct)Container.DataItem).AddCartKbn %>" />
					<asp:HiddenField ID="hfProductSaleId" runat="server" Value="<%# ((CartProduct)Container.DataItem).ProductSaleId %>" />
					<asp:HiddenField ID="hfProductOptionValue" runat="server" Value='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues() %>' />
					<asp:HiddenField ID="hfAllocatedQuantity" runat="server" Value='<%# ((CartProduct)Container.DataItem).QuantityAllocatedToSet[((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).CartSetPromotionNo] %>' />
					<asp:HiddenField ID="hfSubscriptionBoxCourseId" runat="server" Value='<%# ((CartProduct)Container.DataItem).SubscriptionBoxCourseId %>' />

					<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
						<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" /></a>
					<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false %>" />
					</td>
					<td class="product-info">
						<ul>
							<li class="product-name">
								<a href='<%# WebSanitizer.UrlAttrHtmlEncode(((CartProduct)Container.DataItem).CreateProductDetailUrl()) %>' runat="server" Visible="<%# ((CartProduct)Container.DataItem).IsProductDetailLinkValid() %>">
									<%# WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) %></a>
								<%# (((CartProduct)Container.DataItem).IsProductDetailLinkValid() == false) ? WebSanitizer.HtmlEncode(((CartProduct)Container.DataItem).ProductJointName) : "" %>
								<div visible='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList.IsSelectedProductOptionValueAll %>' runat="server">
								<asp:Repeater ID="rProductOptionSettings" DataSource='<%# ((CartProduct)Container.DataItem).ProductOptionSettingList %>' runat="server">
								<ItemTemplate>
									<%# WebSanitizer.HtmlEncode(((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue()) %>
									<%# (((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue() != "") ? "<br />" : "" %>
								</ItemTemplate>
								</asp:Repeater>
								</div>
							</li>
							<li class="product-price">
								<%#: CurrencyManager.ToPrice(((CartProduct)Container.DataItem).Price) %> (<%#: this.ProductPriceTextPrefix %>)
							</li>
							<li visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
								<span style="color:red;">※配送料無料適用外商品です</span>
							</li>
						</ul>
						<p class="attention" visible="<%# this.ErrorMessages.HasMessages(((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex) %>" runat="server">
							<%# WebSanitizer.HtmlEncode(this.ErrorMessages.Get(((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex)) %>
						</p>
						<p class="attention" visible="<%# this.ErrorMessages.HasMessages(((RepeaterItem)Container.Parent.Parent.Parent.Parent).ItemIndex, ((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex) %>" runat="server">
							<%# WebSanitizer.HtmlEncode(this.ErrorMessages.Get(((RepeaterItem)Container.Parent.Parent.Parent.Parent).ItemIndex, ((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex))%>
						</p>
					</td>
					<td class="product-control">
						<div class="amout">
							<span>
								<w2c:ExtendedTextBox ID="tbSetPromotionItemCount" Type="tel" Runat="server" Text='<%# ((CartProduct)Container.DataItem).QuantityAllocatedToSet[((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).CartSetPromotionNo] %>' MaxLength="3"></w2c:ExtendedTextBox>
							</span>
						</div>
						<div class="delete">
							<asp:LinkButton ID="lbDeleteProduct" CommandName="DeleteProduct" Runat="server" CssClass="btn">
								削除
							</asp:LinkButton>
						</div>
					</td>
				</tr>
			</ItemTemplate>
			<FooterTemplate>
				</tbody>
				</table>
			</FooterTemplate>
			</asp:Repeater>

			<div class="set-promotion-footer">
				<dl>
					<dt><%# WebSanitizer.HtmlEncode(((CartSetPromotion)Container.DataItem).SetpromotionDispName)%></dt>
					<dd>
						<span class="line-through" visible="<%# ((CartSetPromotion)Container.DataItem).IsDiscountTypeProductDiscount %>" runat="server">
						<%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).UndiscountedProductSubtotal) %> (税込)
						</span>
						<%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).UndiscountedProductSubtotal - ((CartSetPromotion)Container.DataItem).ProductDiscountAmount) %> (税込)
					</dd>
				</dl>
			</div>

		</div>
		</ItemTemplate>
		</asp:Repeater>
		<!-- △セットプロモーション△ -->

		<%-- ▽ノベルティ▽ --%>
		<asp:Repeater ID="rNoveltyList" runat="server" DataSource="<%# GetCartNovelty(((CartObject)Container.DataItem).CartId) %>" Visible="<%# GetCartNovelty(((CartObject)Container.DataItem).CartId).Length != 0 %>">
			<HeaderTemplate>
				<div class="cart-novelty-unit">
			</HeaderTemplate>
			<ItemTemplate>
				<p class="title">
					<%# WebSanitizer.HtmlEncode(((CartNovelty)Container.DataItem).NoveltyDispName)%>を追加してください。<br/>
				</p>
				<p class="info" runat="server" visible="<%#((CartNovelty)Container.DataItem).GrantItemList.Length == 0 %>">
					ただいま付与できるノベルティはございません。
				</p>
				<asp:Repeater ID="rNoveltyItem" runat="server" DataSource="<%# ((CartNovelty)Container.DataItem).GrantItemList %>" OnItemCommand="rCartList_ItemCommand">
					<HeaderTemplate>
						<table class="cart-table">
						<tbody>
					</HeaderTemplate>
					<ItemTemplate>
						<tr class="cart-unit-product">
							<td class="product-image">
								<w2c:ProductImage ProductMaster="<%# ((CartNoveltyGrantItem)Container.DataItem).ProductInfo %>" IsVariation="true" ImageSize="M" runat="server" />
							</td>
							<td class="product-info">
								<ul>
									<li class="product-name">
										<%# WebSanitizer.HtmlEncode(((CartNoveltyGrantItem)Container.DataItem).JointName) %>
									</li>
									<li class="product-price">
										<%#: CurrencyManager.ToPrice(((CartNoveltyGrantItem)Container.DataItem).Price) %>(<%#: this.ProductPriceTextPrefix %>)
									</li>
								</ul>
							</td>
							<td class="product-control">
								<div class="add">
									<asp:LinkButton ID="lbAddNovelty" CssClass="btn" runat="server" CommandName="AddNovelty" CommandArgument='<%#  string.Format("{0},{1}", ((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex) %>'>追加</asp:LinkButton>
								</div>
							</td>
						</tr>
					</ItemTemplate>
					<FooterTemplate>
						</toboy>
						</table>
					</FooterTemplate>
				</asp:Repeater>
			</ItemTemplate>
			<FooterTemplate>
				</div>
			</FooterTemplate>
		</asp:Repeater>
		<%-- △ノベルティ△ --%>

		<uc:BodyAnnounceFreeShipping runat="server" TargetCart="<%# ((CartObject)Container.DataItem) %>" />

		<div class="cart-unit-footer">
			<dl class="use-point" visible="<%# Constants.W2MP_POINT_OPTION_ENABLED && this.IsLoggedIn %>" runat="server">
				<dt>ポイントを使う</dt>
				<dd runat="server" visible="<%# ((CartObject)Container.DataItem).CanUsePointForPurchase %>">
					<w2c:ExtendedTextBox ID="tbOrderPointUse" Type="tel" Runat="server" Text="<%# GetUsePoint((CartObject)Container.DataItem) %>" MaxLength="6" CssClass="input_widthA"></w2c:ExtendedTextBox>
					<br />
					<span>
						※1<%= Constants.CONST_UNIT_POINT_PT %> = <%: CurrencyManager.ToPrice(1m) %><br />
						<%# WebSanitizer.HtmlEncode(GetNumeric(this.LoginUserPointUsable))%> ポイントまで利用できます
					</span>
					<p class="attention" visible="<%# this.ErrorMessages.HasMessages(Container.ItemIndex, CartErrorMessages.ErrorKbn.Point) %>" runat="server">
						<%# WebSanitizer.HtmlEncode(this.ErrorMessages.Get(Container.ItemIndex, CartErrorMessages.ErrorKbn.Point)) %> 
					</p>
				</dd>
				<dd runat="server" visible="<%# (((CartObject)Container.DataItem).CanUsePointForPurchase == false) %>">
					<p>
						あと「<%#: GetPriceCanPurchaseUsePoint(((CartObject)Container.DataItem).PurchasePriceTotal) %>」の購入でポイントをご利用いただけます。
					</p>
					<p runat="server" visible="<%# (this.LoginUserPointUsable > 0) %>">
						※利用可能ポイント「<%#: GetNumeric(this.LoginUserPointUsable) %>pt」
					</p>
				</dd>
				<% if (this.CanUseAllPointFlg && this.IsLoggedIn) { %>
					<span>
						<asp:CheckBox ID="cbUseAllPointFlg" Text="定期注文で利用可能なポイントすべてを継続使用する"
						OnCheckedChanged="cbUseAllPointFlg_Changed" OnDataBinding="cbUseAllPointFlg_DataBinding"
						CssClass="cbUseAllPointFlgSpCart" AutoPostBack="True" runat="server" /><br/>
						<span>※注文後はマイページ＞定期購入情報より変更できます。</span>
					</span>
				<% } %>
			</dl>
			<dl class="coupon-point" style="list-style: none;" visible="<%# Constants.W2MP_COUPON_OPTION_ENABLED %>" runat="server">
				<dt>クーポンを使う</dt>
				<dd id="divCouponInputMethod" runat="server" style="font-size: 10px; padding: 10px 10px 0px 10px; font-family: 'Lucida Grande','メイリオ',Meiryo,'Hiragino Kaku Gothic ProN', sans-serif; color: #333;">
					<%-- RadioButtonListで生成されるInputタグのレイアウト修正用 --%>
					<style type="text/css">
						.coupon_input_method input
						{
							width: initial !important;
						}
					</style>
					<asp:RadioButtonList runat="server" AutoPostBack="true" ID="rblCouponInputMethod"
						OnSelectedIndexChanged="rblCouponInputMethod_SelectedIndexChanged" OnDataBinding="rblCouponInputMethod_DataBinding"
						DataSource="<%# GetCouponInputMethod() %>" DataTextField="Text" DataValueField="Value" RepeatColumns="2" RepeatDirection="Horizontal"
						CssClass="coupon_input_method"
						style="width: 100%;"></asp:RadioButtonList>
				</dd>
				<dd id="hgcCouponSelect" runat="server" >
					<asp:DropDownList ID="ddlCouponList" runat="server" DataTextField="Text" DataValueField="Value"
						OnTextChanged="ddlCouponList_TextChanged" AutoPostBack="true" style="width: 100%" ></asp:DropDownList>
				</dd>
				<div id="hgcCouponCodeInputArea" runat="server">
					<dd>
					<w2c:ExtendedTextBox ID="tbCouponCode" runat="server" Type="text" Text="<%# GetCouponCode(((CartObject)Container.DataItem).Coupon) %>" MaxLength="30" placeholder="コードを入力" CssClass="input_widthN" autocomplete="off"></w2c:ExtendedTextBox>
					</dd>
				</div>
				<dd>
					<p class="attention" visible="<%# this.ErrorMessages.HasMessages(Container.ItemIndex, CartErrorMessages.ErrorKbn.Coupon) %>" runat="server">
						<%# WebSanitizer.HtmlEncode(this.ErrorMessages.Get(Container.ItemIndex, CartErrorMessages.ErrorKbn.Coupon)) %>
					</p>
				</dd>
				<dd style="text-align: right;">
					<asp:LinkButton runat="server" ID="lbShowCouponBox" Text="クーポンBOX"
						style="padding: .3em 0; background-color: #333; color: #fff; margin-top: 1em; text-align: center; display: block; text-decoration: none !important; width: 100px; margin-left: auto;"
						OnClick="lbShowCouponBox_Click" ></asp:LinkButton>
				</dd>
				<div runat="server" id="hgcCouponBox" style="z-index: 20000; top: 0; left: 0; width: 100%; height: 120%; position: fixed; background-color: rgba(128, 128, 128, 0.75);"
					Visible="<%# ((CartObject)Container.DataItem).CouponBoxVisible %>">
				<div id="hgcCouponList" style="width: 100%; height: 320px; top: 50%; left: 0; text-align: center; background: #fff; position: fixed; z-index: 200001; margin:-180px 0 0 0;">
				<h2 style="height: 20px; color: #fff; background-color: #000; margin-bottom: 0; margin-top: 0px; z-index: 20003">クーポンBOX</h2>
				<div style="height: 260px; overflow: auto; -webkit-overflow-scrolling: touch; z-index: 20003">
				<asp:Repeater ID="rCouponList" Runat="server" ItemType="UserCouponDetailInfo" DataSource="<%# GetUsableCoupons((CartObject)Container.DataItem) %>">
					<HeaderTemplate></HeaderTemplate>
					<ItemTemplate>
						<li>
							<h3 style="margin: 0 auto; border: 1px #888888;  background-color: #CCC; color:black; font-weight: bold;">
								<%# (StringUtility.ToEmpty(Item.CouponDispName) != "")
									? StringUtility.ToEmpty(Item.CouponDispName)
									: StringUtility.ToEmpty(Item.CouponCode) %></h3>
							<dl style="text-align: left;">
								<dd style="padding: 2px; text-align: left; margin-left: 0px;">
									クーポンコード：<%#: StringUtility.ToEmpty(Item.CouponCode) %>
										<asp:HiddenField runat="server" ID="hfCouponBoxCouponCode" Value="<%# Item.CouponCode %>" />
								</dd>
								<dd style="padding: 2px; text-align: left; margin-left: 0px;">有効期限：<%#: DateTimeUtility.ToStringFromRegion(Item.ExpireEnd, DateTimeUtility.FormatType.LongDateHourMinute1Letter) %></dd>
								<dd style="padding: 2px; text-align: left; margin-left: 0px;">割引金額/割引率：
									<%#: GetCouponDiscountString(Item) %>
								</dd>
								<dd style="padding: 2px; text-align: left; margin-left: 0px;">利用可能回数：
									<%#: GetCouponCount(Item) %>
								</dd>
								<dd style="padding: 2px; text-align: left; margin-left: 0px;"><%#: StringUtility.ToEmpty(Item.CouponDispDiscription) %></dd>
							</dl>
							<div style="margin: 0 auto; width: 150px; padding: 10px; height: 60px; background-color: white;">
								<asp:LinkButton runat="server" id="lbCouponSelect" OnClick="lbCouponSelect_Click"
									style="padding: .3em 0; background-color: #333; color: #fff; margin-top: 1em; text-align: center; display: block; text-decoration: none !important; line-height: 1.5;" >このクーポンを使う</asp:LinkButton>
							</div>
						</li>
					</ItemTemplate>
					<FooterTemplate></FooterTemplate>
				</asp:Repeater>
				</div>
				<div style="width: 100%; height: 40px; left: 0; text-align: center; border: 0.5px solid #efefef; background: #fff; position: fixed; z-index: 200002;">
					<asp:LinkButton ID="lbCouponBoxClose" OnClick="lbCouponBoxClose_Click" runat="server"
						style="width: 150px; align-content:center; padding: .3em 0; background-color: #ddd; color: #333; margin-top: 1em; text-align: center; display: block; text-decoration: none !important; line-height: 1.5; margin: auto; margin-top: 7px;">クーポンを利用しない</asp:LinkButton>
				</div>
				</div>
				</div>
			</dl>

			<%-- 小計 --%>
			<dl>
				<dt>小計（<%#: this.ProductPriceTextPrefix %>）</dt>
				<dd><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceSubtotal) %></dd>
			</dl>
			<%if (this.ProductIncludedTaxFlg == false) { %>
				<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
					<dt>消費税額</dt>
					<dd><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceSubtotalTax) %></dd>
				</dl>
			<%} %>
			<%-- セットプロモーション(商品割引) --%>
			<asp:Repeater DataSource="<%# ((CartObject)Container.DataItem).SetPromotions %>" Visible="<%# ((CartObject)Container.DataItem).IsSubscriptionBoxFixedAmount == false %>" runat="server">
			<HeaderTemplate>
			</HeaderTemplate>
			<ItemTemplate>
			<dl visible="<%# ((CartSetPromotion)Container.DataItem).IsDiscountTypeProductDiscount %>" runat="server">
				<dt>
					<%# WebSanitizer.HtmlEncode(((CartSetPromotion)Container.DataItem).SetpromotionDispName)%>
				</dt>
				<dd>
					<%# (((CartSetPromotion)Container.DataItem).ProductDiscountAmount > 0) ? "-" : ""%><%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).ProductDiscountAmount) %>
				</dd>
			</dl>
			</ItemTemplate>
			<FooterTemplate>
			</FooterTemplate>
			</asp:Repeater>

			<%if (Constants.MEMBER_RANK_OPTION_ENABLED && this.IsLoggedIn){ %>
			<dl>
				<dt>会員ランク割引額</dt>
				<dd>
					<%# (((CartObject)Container.DataItem).MemberRankDiscount > 0) ? "-" : ""%><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).MemberRankDiscount * ((((CartObject)Container.DataItem).MemberRankDiscount < 0) ? -1 : 1)) %>
				</dd>
			</dl>
			<%} %>

			<%if (Constants.MEMBER_RANK_OPTION_ENABLED && Constants.FIXEDPURCHASE_OPTION_ENABLED && this.IsLoggedIn){ %>
			<dl>
				<dt>定期会員割引額</dt>
				<dd>
					<%# (((CartObject)Container.DataItem).FixedPurchaseMemberDiscountAmount > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).FixedPurchaseMemberDiscountAmount * ((((CartObject)Container.DataItem).FixedPurchaseMemberDiscountAmount < 0) ? -1 : 1)) %>
				</dd>
			</dl>
			<%} %>

			<%if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
			<div runat="server" visible="<%# (((CartObject)Container.DataItem).HasFixedPurchase) %>">
				<dl>
					<dt>定期購入割引額</dt>
					<dd>
						<%#: (((CartObject)Container.DataItem).FixedPurchaseDiscount > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).FixedPurchaseDiscount * ((((CartObject)Container.DataItem).FixedPurchaseDiscount < 0) ? -1 : 1)) %>
					</dd>
				</dl>
			</div>
			<%} %>

			<%if (Constants.W2MP_COUPON_OPTION_ENABLED){ %>
				<dl>
					<dt>クーポン割引額</dt>
					<dd>
						<%#: GetCouponName(((CartObject)Container.DataItem)) %>
						<%# (((CartObject)Container.DataItem).UseCouponPrice > 0) ? "-" : "" %>
						<%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).UseCouponPrice * ((((CartObject)Container.DataItem).UseCouponPrice < 0) ? -1 : 1)) %>
					</dd>
				</dl>
			<%} %>

			<%if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsLoggedIn){ %>
			<dl>
				<dt>ポイント利用額</dt>
				<dd>
					<%# (((CartObject)Container.DataItem).UsePointPrice > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).UsePointPrice * ((((CartObject)Container.DataItem).UsePointPrice < 0) ? -1 : 1)) %>
				</dd>
			</dl>
			<%} %>
			<%-- 配送料金 --%>
			<dl>
				<dt>配送料金</dt>
				<dd runat="server" style='<%# ((((CartObject)Container.DataItem).ShippingPriceSeparateEstimateFlg) || (((CartObject)Container.DataItem).IsDisplayShippingPriceUnsettled)) ? "display:none;" : ""%>'>
					<%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceShipping) %>
				</dd>
				<dd runat="server" style='<%# (((CartObject)Container.DataItem).ShippingPriceSeparateEstimateFlg == false) ? "display:none;" : ""%>'>
					<%# WebSanitizer.HtmlEncode(((CartObject)Container.DataItem).ShippingPriceSeparateEstimateMessage)%>
				</dd>
				<dd runat="server" style='<%# (((CartObject)(Container).DataItem).ShippingPriceSeparateEstimateFlg == false) && (((CartObject)(Container).DataItem).IsDisplayShippingPriceUnsettled == false) ? "display:none;" : "color:red"%>'>
					配送先入力後に確定となります。
				</dd>
			</dl>
			<%-- 決済手数料 --%>
			<span runat="server" Visible="<%# ((((CartObject)Container.DataItem).Payment) != null) %>">
				<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
				<dt>決済手数料</dt>
				<dd><%#: (((CartObject)Container.DataItem).Payment != null) ? CurrencyManager.ToPrice(((CartObject)Container.DataItem).Payment.PriceExchange) : "" %></dd>
			</dl>
			</span>

			<%-- セットプロモーション(配送料割引) --%>
			<asp:Repeater DataSource="<%# ((CartObject)Container.DataItem).SetPromotions %>" runat="server">
			<HeaderTemplate>
			</HeaderTemplate>
			<ItemTemplate>
			<dl visible='<%# ((CartSetPromotion)Container.DataItem).IsDiscountTypeShippingChargeFree %>' runat="server">
			<dt>
				<%# WebSanitizer.HtmlEncode(((CartSetPromotion)Container.DataItem).SetpromotionDispName)%>(送料割引)
			</dt>
			<dd>
				<%# (((CartSetPromotion)Container.DataItem).ShippingChargeDiscountAmount > 0) ? "-" : ""%><%#: CurrencyManager.ToPrice(((CartSetPromotion)Container.DataItem).ShippingChargeDiscountAmount) %>
			</dd>
			</dl>
			</ItemTemplate>
			<FooterTemplate>
			</FooterTemplate>
			</asp:Repeater>
			<dl>
				<dt>合計(税込)</dt>
				<dd>
					<%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceTotal) %>
				</dd>
			</dl>


			<%-- 隠し値：カートID --%>
			<asp:HiddenField ID="hfCartId" runat="server" Value="<%# ((CartObject)Container.DataItem).CartId %>" />
			<%-- 隠し再計算ボタン --%>
			<asp:LinkButton id="lbRecalculateCart" runat="server" CommandArgument="<%# Container.ItemIndex %>" onclick="lbRecalculate_Click"></asp:LinkButton>

			<%if (Constants.CARTCOPY_OPTION_ENABLED){ %>
			<asp:LinkButton ID="lbCopyCart" runat="server" Text="カートコピー" CommandArgument="<%# Container.ItemIndex %>" OnClick="lbCopyCart_Click" style="padding: .5em 0; background-color: #333; color: #fff; margin-top: 1em; text-align: center; display: inline-block; text-decoration: none !important; width: 120px; margin-left:10px;" Visible="<%# Constants.GIFTORDER_OPTION_WITH_SHORTENING_GIFT_OPTION_ENABLED == false %>" />
			<asp:LinkButton ID="lbDeleteCart" runat="server" Text="カート削除" CommandArgument="<%# Container.ItemIndex %>" OnClick="lbDeleteCart_Click" style="padding: .5em 0; background-color: #333; color: #fff; margin-top: 1em; text-align: center; display: inline-block; text-decoration: none !important; width: 120px; margin-left:10px;" />
			<%-- ▼カートコピー完了メッセージ▼ --%>
			<div runat="server" visible="<%# ((CartObject)Container.DataItem).IsCartCopyCompleteMesseges %>" style="margin-left:10px;margin-top:10px;">
				<span>カートのコピーが完了しました。</span>
			</div>
			<%-- ▲カートコピー完了メッセージ▲ --%>
			<%} %>

		</div>
	</div>
</ItemTemplate>
</asp:Repeater>

<%} %>

<div class="cart-footer">
	<%-- はカート内に商品がある場合のみ表示する --%>
	<% if (this.Process.IsSubscriptionBoxError == false){ %>
	<%if (this.CartList.Items.Count != 0){ %>
	<div class="button-next" style="text-align: right;display:<%= (this.CartList.Items.Count != 0) ? "block" : "none" %>;">

		<%-- ▼PayPalログインここから▼ --%>
		<%if (Constants.PAYPAL_LOGINPAYMENT_ENABLED) {%>
		<%if (this.DispPayPalShortCut) {%>
			<div style="float: right; width: 190px; margin: 0 0 0 0;">
				<%
					ucPaypalScriptsForm.LogoDesign = "Cart";
					ucPaypalScriptsForm.AuthCompleteActionControl = lbPayPalAuthComplete;
					ucPaypalScriptsForm.GetShippingAddress = (this.IsLoggedIn == false);
				%>
				<uc:PaypalScriptsForm ID="ucPaypalScriptsForm" runat="server" />
				<div id="paypal-button"></div>
				<div style="font-size: 9pt;text-align: center">
					<%if (SessionManager.PayPalCooperationInfo != null) {%>
						<%: (SessionManager.PayPalCooperationInfo != null) ? SessionManager.PayPalCooperationInfo.AccountEMail : "" %> 連携済
					<%} %>
					<asp:LinkButton ID="lbPayPalAuthComplete" runat="server" OnClick="lbPayPalAuthComplete_Click"></asp:LinkButton>
				</div>
			</div>
		<%} %>
		<%} %>
		<%-- ▲PayPalログインここまで▲ --%>
		<%-- ▼▼Amazonお支払いボタンウィジェット▼▼ --%>
		<%if (this.CanUseAmazonPaymentForFront()) { %>
		<div id="AmazonPayButton"></div>
		<div style="text-align: center">
			<%--▼▼ Amazon Pay(CV2)ボタン ▼▼--%>
			<div style="display: inline-block;" id="AmazonPayCv2Button"></div>
			<%--▲▲ Amazon Pay(CV2)ボタン ▲▲--%>
		</div>
		<%} %>
		<%-- ▲▲Amazonお支払いボタンウィジェット▲▲ --%>
		</div>
	<%--
	<div class="total">
		総合計 <%#: CurrencyManager.ToPrice(this.CartList.PriceCartListTotal) %>
	</div>
	--%>
	<div class="button-next">
		<asp:Linkbutton Visible="<%# this.IsDisplayTwoClickButton %>" ID="lbTwoClickButton1" OnClick="lbTwoClickButton_Click" class="btn" runat="server">２クリック購入</asp:Linkbutton>
	</div>
	<div class="button-next">
		<a href="<%= WebSanitizer.HtmlEncode(this.NextEvent) %>" class="btn">ご購入手続きへ進む</a>
	</div>
	<%} %>
	<%} %>
	<div class="button-prev">
		<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT) %>" class="btn">ショッピングを続ける</a>
	</div>
</div>

<%-- CRITEOタグ（引数：カート情報） --%>
<uc:Criteo ID="criteo" runat="server" Datas="<%# this.CartList %>" />

<% if(Constants.AMAZON_PAYMENT_CV2_ENABLED){ %>
	<%--▼▼ Amazon Pay(CV2)スクリプト ▼▼--%>
	<script src="https://static-fe.payments-amazon.com/checkout.js"></script>
	<script type="text/javascript" charset="utf-8">
		showAmazonPayCv2Button(
			'#AmazonPayCv2Button',
			'<%= Constants.PAYMENT_AMAZON_SELLERID %>',
			<%= Constants.PAYMENT_AMAZON_ISSANDBOX.ToString().ToLower() %>,
			'<%= this.AmazonRequest.Payload %>',
			'<%= this.AmazonRequest.Signature %>',
			'<%= Constants.PAYMENT_AMAZON_PUBLIC_KEY_ID %>');
	</script>
	<%--▲▲ Amazon Pay(CV2)スクリプト ▲▲--%>
<% } else { %>
	<%--▼▼Amazonウィジェット用スクリプト▼▼--%>
	<script type="text/javascript">
		
		window.onAmazonLoginReady = function () {
			amazon.Login.setClientId('<%=Constants.PAYMENT_AMAZON_CLIENTID %>');
			amazon.Login.setUseCookie(true);
		};
		window.onAmazonPaymentsReady = function () {
			if ($('#AmazonPayButton').length) showButton();
		};

		<%-- Amazonボタン表示ウィジェット --%>
		function showButton() {
			var authRequest;
			OffAmazonPayments.Button("AmazonPayButton", "<%=Constants.PAYMENT_AMAZON_SELLERID %>", {
				type: "PwA",
				color: "Gold",
				size: "medium",
				authorization: function () {
					loginOptions = {
						scope: "payments:widget payments:shipping_address profile",
						popup: false
					};
					authRequest = amazon.Login.authorize(loginOptions, "<%=w2.App.Common.Amazon.Util.AmazonUtil.CreateCallbackUrl(Constants.PAGE_FRONT_AMAZON_ORDER_CALLBACK) %>");
				},
				onError: function (error) {
					alert(error.getErrorMessage());
				}
			});
		};
	</script>
	<script async="async" type="text/javascript" charset="utf-8" src="<%=Constants.PAYMENT_AMAZON_WIDGETSSCRIPT %>"></script>
	<%-- ▲▲Amazonウィジェット用スクリプト▲▲ --%>
<% } %>

</section>
<%-- △編集可能領域△ --%>

<div id="divBottomArea">
<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>
<script>
	var subscriptionBoxMessage = '「@@ 1 @@」は「@@ 2 @@」の必須商品です。\n削除すると、「@@ 2 @@」の申し込みがキャンセルされます。\nよろしいですか？';

	// 必須商品チェック(頒布会)
	function delete_product_check_for_subscriptionBox(value) {
		var subscriptionBoxProductName = $(value).parent().parent().parent().find("[id$='hfProductName']").val();
		var subscriptionBoxProductId = $(value).parent().parent().parent().find("[id$='hfProductId']").val();
		var subscriptionBoxName = $(value).parent().parent().parent().find("[id$='hfSubscriptionBoxCourseName']").val();
		var subscriptionBoxId = $(value).parent().parent().parent().find("[id$='hfSubscriptionBoxCourseId']").val();
		subscriptionBoxMessage = subscriptionBoxMessage.replace('@@ 1 @@', subscriptionBoxProductName);
		subscriptionBoxMessage = subscriptionBoxMessage.replace('@@ 2 @@', subscriptionBoxName);
		subscriptionBoxMessage = subscriptionBoxMessage.replace('@@ 2 @@', subscriptionBoxName);
		return confirm(subscriptionBoxMessage);
	}
</script>
</asp:Content>
