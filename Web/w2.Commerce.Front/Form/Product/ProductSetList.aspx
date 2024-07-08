<%--
=========================================================================================================
  Module      : 商品セット組立画面(ProductSetList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Form/Product/ProductSetList.aspx.cs" Inherits="Form_Product_ProductSetList" Title="セット商品組立ページ" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
	<link href="../../Css/product.css" rel="stylesheet" type="text/css" media="all" />
	<% if (Constants.MOBILEOPTION_ENABLED){%>
		<link rel="Alternate" media="handheld" href="<%= WebSanitizer.HtmlEncode(GetMobileUrl()) %>" />
	<% } %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvProductSetList">

	<div id="dvProductSetListArea">
		<h2><span>
			<img src="../../Contents/ImagesPkg/order/h2_productset.gif" alt="セット商品組み立て" /></span></h2>
		
		<div class="dvContentsInfo">
		<p>
		<%: this.SetName %>
		</p>
		<%= this.Description %>
		</div>

		<asp:UpdatePanel runat="server">
		<ContentTemplate>
		<asp:Repeater ID="rProductList" runat="server">
			<HeaderTemplate>
				<table cellspacing="0">
					<tr>
						<th class="thumnail">&nbsp;</th>
						<th class="productId">商品ID</th>
						<th class="productName">商品名</th>
						<th class="variationName">バリエーション</th>
						<th class="productPrice">セット価格<br />（通常価格）</th>
						<th class="itemCount">数量</th>
						<th class="itemSetPriceTotal">金額</th>
					</tr>
			</HeaderTemplate>
			<ItemTemplate>
					<tr>
						<td class="thumnail">
							<w2c:ProductImage ID="ProductImageWindowShopping" ImageTagId="picture" ImageSize="S" ProductMaster="<%# Container.DataItem %>" IsVariation="true" runat="server" />
						</td>
						<td class="productId">
							<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) %>
							<asp:HiddenField ID="hfProductId" runat="server" />
							<asp:HiddenField ID="hfVariationId" runat="server" />
							<asp:HiddenField ID="hfCategoryId" runat="server" /></td>
						<td class="productName"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCT_NAME)) %><br />
							<strong><%# WebSanitizer.HtmlEncodeChangeToBr(Eval(Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE)) %></strong>
							<span id="spanReturnExchangeMessage" runat="server" visible='<%# StringUtility.ToEmpty(Eval(Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE)) != "" %>'>
								（<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT) %><%= WebSanitizer.HtmlEncode(ShopMessage.GetMessage("ReturnSpecialContractPage")) %>" target='<%= (ShopMessage.GetMessage("ReturnSpecialContractPage") == "#") ? "_self" : "_blank" %>' style='font-size:10px'>返品特約</a>）
							</span>						
							<span style="color:Red;" id="sSetItemErrorMessage" runat="server"></span>
						</td>
						<td class="variationName"><%# WebSanitizer.HtmlEncode(CreateVariationName(Container.DataItem, "", string.Empty, Constants.CONST_PRODUCTVARIATIONNAME_PUNCTUATION)) %> </td>
						<td class="productPrice"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTSETITEM_SETITEM_PRICE, "{0:c}")) %><br />
							(<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTVARIATION_PRICE, "{0:c}")) %>)
							<asp:HiddenField ID="hfSetItemPrice" Value="<%# Eval(Constants.FIELD_PRODUCTSETITEM_SETITEM_PRICE) %>" runat="server" />
							<asp:HiddenField ID="hfFamilyFlg" Value="<%# Eval(Constants.FIELD_PRODUCTSETITEM_FAMILY_FLG) %>" runat="server" />
							</td>
						<td class="itemCount"><asp:DropDownList ID="ddlItemCount" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlItemCount_SelectedIndexChanged"></asp:DropDownList></td>
						<td class="itemSetPriceTotal"><span id="sSetItemPriceTotal" runat="server"></span></td>
					</tr>
			</ItemTemplate>
			<FooterTemplate>
					<tr>
						<th class="setPriceTotal" colspan="6">セット小計</th>
						<td class="setPriceTotal"><span ID="spSetPriceTotal" runat="server"></span></td>
					</tr>
				</table>
			</FooterTemplate>
		</asp:Repeater>
		</ContentTemplate>
		</asp:UpdatePanel>

	</div>
		
	<div class="dvProductBtnBox">
		<p>
			<%-- 前の画面へ戻る --%>
			<span><asp:LinkButton id="lbBack" runat="server" OnClick="lbBack_Click" Text="戻る" CssClass="btn btn-large" /></span>
			<%-- カート投入 --%>
			<span><asp:LinkButton id="lbAddCart" runat="server" OnClick="lbAddCart_Click" Text="カート投入" CssClass="btn btn-large btn-inverse" /></span></p>
	</div>

</div>
</asp:Content>

