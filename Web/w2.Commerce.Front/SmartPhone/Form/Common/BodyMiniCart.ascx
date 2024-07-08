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

<div class="minicart">

<h3 class="title">カートの中身合計</h3>
	
	<%-- ▽カート商品あり▽ --%>
	<div id="divMiniCart" class="inner" runat="server">
	<asp:Repeater ID="rMiniCartList" runat="server" OnItemCommand="rCartList_ItemCommand" ItemType="CartObject">
	<ItemTemplate>
		<asp:Repeater id="rMiniCart" runat="server" DataSource='<%# Item %>' OnItemCommand="rCartList_ItemCommand" ItemType="CartProduct">
		<ItemTemplate>
			<%-- 通常商品 --%>
			<div visible="<%# (Item.IsSetItem == false) && (Item.Count != 0) %>" runat="server">
			<%-- 隠し値 --%>
			<asp:HiddenField ID="hfShopId" runat="server" Value="<%# Item.ShopId %>" />
			<asp:HiddenField ID="hfProductId" runat="server" Value="<%# Item.ProductId %>" />
			<asp:HiddenField ID="hfSubscriptionBoxCourseId" runat="server" Value="<%# Item.SubscriptionBoxCourseId %>" />
			<asp:HiddenField ID="hfVariationId" runat="server" Value="<%# Item.VariationId %>" />
			<asp:HiddenField ID="hfIsFixedPurchase" runat="server" Value="<%# Item.IsFixedPurchase %>" />
			<asp:HiddenField ID="hfAddCartKbn" runat="server" Value="<%# Item.AddCartKbn %>" />
			<asp:HiddenField ID="hfProductSaleId" runat="server" Value="<%# Item.ProductSaleId %>" />
			<asp:HiddenField ID="hfProductOptionValue" runat="server" Value='<%# Item.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues() %>' />
			<asp:HiddenField ID="hfUnallocatedQuantity" runat="server" Value='<%# Item.QuantitiyUnallocatedToSet %>' />
				<table class="minicart-table">
				<tbody>
					<tr>
						<td>商品名</td>
						<td>
							<span class="product-name">
								<a href='<%#: Item.CreateProductDetailUrl() %>' runat="server" Visible="<%# Item.IsProductDetailLinkValid() %>">
									<%#: Item.ProductJointName %></a>
								<br />
								<%#: (Item.IsProductDetailLinkValid() == false) ? Item.ProductJointName : "" %>
							</span>

						</td>
					</tr>
					<tr visible='<%# Item.ProductOptionSettingList.IsSelectedProductOptionValueAll %>' runat="server">
						<td>付帯情報</td>
						<td>
							<span visible='<%# Item.ProductOptionSettingList.IsSelectedProductOptionValueAll %>' runat="server">
								<asp:Repeater ID="rProductOptionSettings" DataSource='<%# Item.ProductOptionSettingList %>' runat="server" ItemType="ProductOptionSetting">
								<ItemTemplate>
									<%#: Item.GetDisplayProductOptionSettingSelectValue() %>
									<%# (string.IsNullOrEmpty(Item.GetDisplayProductOptionSettingSelectValue()) == false) ? "<br />" : "" %>
								</ItemTemplate>
								</asp:Repeater>
							</span>
						</td>
					</tr>
					<tr>
						<td><%#: (Item.IsSubscriptionBoxFixedAmount() == false) ? "価格×" : "" %>数量</td>
						<td>
							<span class="product-price">
								<% if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false){ %>
								<%#: (Item.IsSubscriptionBoxFixedAmount() == false) ? CurrencyManager.ToPrice(Item.Price) + " x " : "" %>
								<% } else { %>
								<%#: (Item.IsSubscriptionBoxFixedAmount() == false)
										? string.Format(
											"{0}{1}{2}{3}{4} x ",
											(((CartProduct)Container.DataItem).TotalOptionPrice > 0) ? "(" : string.Empty,
											CurrencyManager.ToPrice(((CartProduct)Container.DataItem).Price),
											(((CartProduct)Container.DataItem).TotalOptionPrice > 0) ? "+" : string.Empty,
											(((CartProduct)Container.DataItem).TotalOptionPrice > 0) ? CurrencyManager.ToPrice(((CartProduct)Container.DataItem).TotalOptionPrice) : string.Empty,
											(((CartProduct)Container.DataItem).TotalOptionPrice > 0) ? ")" : string.Empty)
										: "" %>
								<% } %>
								<%#: StringUtility.ToNumeric(Item.Count) %>
							</span>
						</td>
					</tr>
				</tbody>
				</table>
				<div class="delete-product" visible="<%# Item.IsSetItem == false && Item.Count != 0 %>" runat="server">
					<asp:LinkButton ID="lbDeleteProduct" CommandName="DeleteProduct" Runat="server" CssClass="btn">削除</asp:LinkButton>
				</div>
				<hr />
			</div>
			<%-- セット商品 --%>
			<div visible="<%# (Item.IsSetItem) && (Item.ProductSetItemNo == 1) %>" runat="server">
			<%-- 隠し値 --%>
			<asp:HiddenField ID="hfIsSetItem" runat="server" Value="<%# Item.IsSetItem %>" />
			<asp:HiddenField ID="hfProductSetId" runat="server" Value="<%# OrderPage.GetProductSetId(Item) %>" />
			<asp:HiddenField ID="hfProductSetNo" runat="server" Value="<%# OrderPage.GetProductSetNo(Item) %>" />
			<asp:HiddenField ID="hfProductSetItemNo" runat="server" Value="<%# Item.ProductSetItemNo %>" />
				<asp:Repeater id="rProductSet" DataSource="<%# (Item.ProductSet != null) ? Item.ProductSet.Items : null %>" OnItemCommand="rCartList_ItemCommand" runat="server" ItemType="CartProduct">
				<HeaderTemplate>
					<table class="minicart-table">
					<tbody>
				</HeaderTemplate>
				<ItemTemplate>
					<tr>
						<td>商品名</td>
						<td>
							<span class="product-name">
								<a href='<%#: Item.CreateProductDetailUrl() %>' runat="server" Visible="<%# Item.IsProductDetailLinkValid() %>">
									<%#: Item.ProductJointName %>
								</a>
								<br />
								<%#: (Item.IsProductDetailLinkValid() == false) ? Item.ProductJointName + " x " + Item.CountSingle : ""%>
							</span>
						</td>
					</tr>
					<tr visible='<%# Item.ProductOptionSettingList.IsSelectedProductOptionValueAll %>' runat="server">
						<td>付帯情報</td>
						<td>
							<span visible='<%# Item.ProductOptionSettingList.IsSelectedProductOptionValueAll %>' runat="server">
								<asp:Repeater ID="rProductOptionSettings" DataSource='<%# Item.ProductOptionSettingList %>' runat="server" ItemType="ProductOptionSetting">
								<ItemTemplate>
									<%#: Item.GetDisplayProductOptionSettingSelectValue() %>
									<%# (string.IsNullOrEmpty(Item.GetDisplayProductOptionSettingSelectValue()) == false) ? "<br />" : "" %>
								</ItemTemplate>
								</asp:Repeater>
							</span>
						</td>
					</tr>
					<tr>
						<td><%#: (Item.IsSubscriptionBoxFixedAmount() == false) ? "価格×" : "" %>数量</td>
						<td>
							<span class="product-price">
							<% if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false){ %>
							<p class="product-price"><%#: (Item.IsSubscriptionBoxFixedAmount() == false) ? (CurrencyManager.ToPrice(Item.Price) + " x ") : "" %><%#: Item.CountSingle %></p>
							<% } else { %>
									<p class="product-price">
										<%#: (Item.IsSubscriptionBoxFixedAmount() == false)
											? ProductOptionSettingHelper.ToDisplayProductOptionPrice(Item) + " x "
											: "" %><%#: Item.CountSingle %>
									</p>
							<% } %>
							</span>
						</td>
					</tr>
				</ItemTemplate>
				<FooterTemplate>
					</tbody>
					</table>
					<div class="delete-product">
						<asp:LinkButton ID="lbDeleteProductSet" CommandName="DeleteProductSet" CommandArgument='' Runat="server" CssClass="btn">セット商品を削除</asp:LinkButton>
					</div>
					<hr />
				</FooterTemplate>
				</asp:Repeater>
			</div>
		</ItemTemplate>
		</asp:Repeater>
		<%-- 隠し値：カートID --%>
		<asp:HiddenField ID="hfCartId" runat="server" Value="<%# Item.CartId %>" />
	</ItemTemplate>
	</asp:Repeater>
	<%-- △カート商品一覧ループ△ --%>
	<div class="total">
		<%: GetNumeric(this.CartProductCount) %>点&nbsp;
		合計<%: CurrencyManager.ToPrice(this.TotalPrice) %>(<%: this.ProductPriceTextPrefix %>)
	</div>

	<div class="view-cart">
		<a href="<%: this.CartListPageUrl %>" class="btn">カートを見る</a>
	</div>
	
	</div>
	<%-- △カート商品あり△ --%>

	<%-- ▽カート商品なし▽ --%>
	<div id="divMiniCartEmpty" class="inner" runat="server">
		<p class="empty">カートの中身は空です。</p>
	</div>
	<%-- △カート商品なし△ --%>

</div>
</ContentTemplate>
</asp:UpdatePanel>
<%-- △ミニカート（UpdatePanel）△ --%>

<%-- △編集可能領域△ --%>
