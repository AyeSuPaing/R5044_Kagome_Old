<%--
=========================================================================================================
  Module      : ミニカート出力コントローラ(BodyMiniCart.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/BodyMiniCart.ascx.cs" Inherits="Form_Common_BodyMiniCart" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
タイトルタグはカスタムパーツのみ利用します。
<%@ Page Title="無名のパーツ" %>
<%@ FileInfo LastChanged="最終更新者" %>

--%>

<%-- ▽ミニカート（UpdatePanel）▽ --%>
<asp:UpdatePanel ID="upMiniCart" runat="server">
<ContentTemplate>

<% this.Reload(); %>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<%-- ▽カート商品あり▽ --%>
<div id="divMiniCart" class="dvMiniCart" runat="server">
	<h3>ショッピングカート</h3>
	<div class="inner">
		<ul>
		<%-- ▽カート商品一覧ループ▽ --%>
		<asp:Repeater ID="rMiniCartList" runat="server" OnItemCommand="rCartList_ItemCommand" ItemType="CartObject">
		<ItemTemplate>
			<asp:Repeater ID="rMiniCart" runat="server" DataSource="<%# Item %>" OnItemCommand="rCartList_ItemCommand" ItemType="CartProduct">
			<ItemTemplate>
				<%-- 通常商品 --%>
				<div class="product" visible="<%# (Item.IsSetItem == false) && (Item.Count != 0) %>" runat="server">
				<%-- 隠し値 --%>
				<asp:HiddenField ID="hfShopId" runat="server" Value="<%# Item.ShopId %>" />
				<asp:HiddenField ID="hfProductId" runat="server" Value="<%# Item.ProductId %>" />
				<asp:HiddenField ID="hfSubscriptionBoxCourseId" runat="server" Value="<%# Item.SubscriptionBoxCourseId %>" />
				<asp:HiddenField ID="hfVariationId" runat="server" Value="<%# Item.VariationId %>" />
				<asp:HiddenField ID="hfIsFixedPurchase" runat="server" Value="<%# Item.IsFixedPurchase || Item.IsSubscriptionBox %>" />
				<asp:HiddenField ID="hfAddCartKbn" runat="server" Value="<%# Item.AddCartKbn %>" />
				<asp:HiddenField ID="hfProductSaleId" runat="server" Value="<%# Item.ProductSaleId %>" />
				<asp:HiddenField ID="hfProductOptionValue" runat="server" Value='<%# Item.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues() %>' />
				<li class="productInfo clearFix">
				<table class="miniCart">
					<tr>
						<td>
							<a href="<%#: Item.CreateProductDetailUrl() %>" runat="server" Visible="<%# CanDisplayProductDetailUrl(Item)%>">
								<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="S" runat="server" ImageWidth="60" CssClass="pimg" />
							</a>
						</td>
						<td style="padding-left: 5px;">
							<span class="productName">
								<a href="<%#: Item.CreateProductDetailUrl() %>" runat="server" Visible="<%# Item.IsProductDetailLinkValid() %>">
									<%#: Item.ProductJointName %></a>
								<%#: (Item.IsProductDetailLinkValid() == false) ? Item.ProductJointName : "" %>
								<br />
								[<%#: Item.VariationId %>]
							</span>
							<span class="productOption">
								<asp:Repeater ID="rProductOptionSettings" DataSource='<%# Item.ProductOptionSettingList %>' runat="server" ItemType="w2.App.Common.Product.ProductOptionSetting">
								<ItemTemplate>
									<span style="font-size:0.9em">
										<%#: Item.GetDisplayProductOptionSettingSelectValue() %>
										<%# (string.IsNullOrEmpty(Item.GetDisplayProductOptionSettingSelectValue()) == false) ? "<br />" : "" %>
									</span>
								</ItemTemplate>
								</asp:Repeater>
							</span>
							<span class="productPrice">
								<% if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false) { %>
								<span Visible="<%# (Item.IsSubscriptionBoxFixedAmount() == false) %>" runat="server">
								<%#: CurrencyManager.ToPrice(Item.Price) %>
								×
								<span class="productCount"><%#: Item.Count %></span>
								</span>
								<% } else { %>
								<span Visible="<%# (Item.IsSubscriptionBoxFixedAmount() == false) %>" runat="server">
									<%#: ProductOptionSettingHelper.ToDisplayProductOptionPrice(Item) %>
									×
									<span class="productCount"><%#: Item.Count %></span>
								</span>
								<% } %>
								<div class="deleteProduct"><asp:LinkButton ID="lbDeleteProduct" CommandName="DeleteProduct" Runat="server" Visible="<%# CanDeleteProduct() %>">削除</asp:LinkButton></div>
							</span>
						</td>
					</tr>
				</table>
				</li>
				</div><!--product-->
				<%-- セット商品 --%>
				<div class="product" visible="<%# (Item.IsSetItem) && (Item.ProductSetItemNo == 1) %>" runat="server">
				<%-- 隠し値 --%>
				<asp:HiddenField ID="hfIsSetItem" runat="server" Value="<%# Item.IsSetItem %>" />
				<asp:HiddenField ID="hfProductSetId" runat="server" Value="<%# OrderPage.GetProductSetId(Item) %>" />
				<asp:HiddenField ID="hfProductSetNo" runat="server" Value="<%# OrderPage.GetProductSetNo(Item) %>" />
				<asp:HiddenField ID="hfProductSetItemNo" runat="server" Value="<%# Item.ProductSetItemNo %>" />
				<asp:Repeater id="rProductSet" DataSource="<%# (Item.ProductSet != null) ? Item.ProductSet.Items : null %>" OnItemCommand="rCartList_ItemCommand" runat="server" ItemType="CartProduct">
				<HeaderTemplate>
					<li class="productInfo clearFix">
					<table class="miniCart">
				</HeaderTemplate>
				<ItemTemplate>
					<tr>
						<td style="padding: 5px 0px;">
							<a href="<%#: Item.CreateProductDetailUrl() %>" runat="server" Visible="<%# Item.IsProductDetailLinkValid() %>">
								<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="S" runat="server" ImageWidth="60" CssClass="pimg" />
							</a>
						</td>
						<td style="padding-left: 5px;">
							<span class="productName">
								<a href="<%#: Item.CreateProductDetailUrl() %>" runat="server" Visible="<%# Item.IsProductDetailLinkValid() %>">
								<%#: Item.ProductJointName %></a>
								<%#: (Item.IsProductDetailLinkValid() == false) ? Item.ProductJointName : "" %>
								<br />
								[<%#: Item.VariationId %>]
							</span>
							<span class="productOption">
								<asp:Repeater ID="rProductOptionSettings" DataSource='<%# Item.ProductOptionSettingList %>' runat="server" ItemType="ProductOptionSetting">
								<ItemTemplate>
									<span style="font-size:0.9em">
										<%#: Item.GetDisplayProductOptionSettingSelectValue() %>
										<%# (string.IsNullOrEmpty(Item.GetDisplayProductOptionSettingSelectValue()) == false) ? "<br />" : "" %>
									</span>
								</ItemTemplate>
								</asp:Repeater>
							</span>
							<span class="productPrice" Visible="<%# (Item.IsSubscriptionBoxFixedAmount() == false) %>">
								<%#: CurrencyManager.ToPrice(Item.Price) %>
								×
								<span class="productCount"><%#: Item.CountSingle %></span>
							</span>
						</td>
					</tr>
				</ItemTemplate>
				<FooterTemplate>
					</table>
					<span class="deleteProduct"><asp:LinkButton ID="lbDeleteProductSet" CommandName="DeleteProductSet" CommandArgument='' Runat="server"  Visible="<%# CanDeleteProduct() %>">セット商品を削除</asp:LinkButton></span>
					</li>
				</FooterTemplate>
				</asp:Repeater>
				</div><!--product-->
			</ItemTemplate>
			</asp:Repeater>
			<%-- 隠し値：カートID --%>
			<asp:HiddenField ID="hfCartId" runat="server" Value="<%# Item.CartId %>" />
		</ItemTemplate>
		</asp:Repeater>
		<%-- △カート商品一覧ループ△ --%>
		<li class="priceTotal">合計 <%: CurrencyManager.ToPrice(this.TotalPrice) %>(<%: this.ProductPriceTextPrefix %>)</li>
		<li class="viewCartLink">
			<a href="<%: this.CartListPageUrl %>" class="btn btn-mini btn-inverse">カートを見る</a>
		</li>
		</ul>
	</div>
</div>
<%-- △カート商品あり△ --%>

<%-- ▽カート商品なし▽ --%>
<div id="divMiniCartEmpty" class="dvMiniCart" runat="server">
	<h3>ショッピングカート</h3>
	<div class="inner">
		<ul><li class="vacant">カートに商品がありません。</li></ul>
	</div>
</div>
<%-- △カート商品なし△ --%>

</ContentTemplate>
</asp:UpdatePanel>
<%-- △ミニカート（UpdatePanel）△ --%>

<%-- △編集可能領域△ --%>
