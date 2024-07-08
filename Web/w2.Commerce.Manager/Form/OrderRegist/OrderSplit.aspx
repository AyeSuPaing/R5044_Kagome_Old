<%--
=========================================================================================================
  Module      : 新規注文登録確認ページ(OrderSplit.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OrderSplit.aspx.cs" Inherits="Form_OrderRegist_OrderSplit" MaintainScrollPositionOnPostback="true" %>

<%@ Register TagPrefix="uc" TagName="FieldMemoSetting" Src="~/Form/Common/FieldMemoSetting/BodyFieldMemoSetting.ascx" %>
<%@ Import Namespace="System.Web.UI.HtmlControls" %>
<%@ Import Namespace="w2.App.Common.Order" %>
<%@ Import Namespace="w2.App.Common.Product" %>
<%@ Import Namespace="w2.App.Common.Order.OrderCombine" %>
<%@ Import Namespace="w2.App.Common.Order.FixedPurchaseCombine" %>
<%@ Import Namespace="w2.Domain.Order" %>
<%@ Import Namespace="w2.Domain.Payment" %>
<%@ Import Namespace="w2.App.Common.Option" %>
<%@ Import Namespace="w2.Domain.FixedPurchase" %>
<asp:Content ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<style>
		.btn_help {
			top: -4px;
		}

		.btn_pagetop {
			bottom: 90px;
		}

		.group-memo-area {
			word-break: break-all;
		}

		.no-discount {
			border: none;
			background-color: #fff;
		}

		.main-contents-inner {
			padding-bottom: 100px;
		}

		.sidemenu_slim .main-contents {
			margin-left: 30px;
			-webkit-transition: 0.3s;
			-o-transition: 0.3s;
			transition: 0.3s;
		}

		.form-element-group-title,
		.form-element-group-content {
			word-break: break-all;
			white-space: normal;
			padding-right: 5px;
		}

		.item-name {
			word-break: break-all;
			white-space: normal;
			max-width: 15em;
		}

		.form-element-group-list {
			border: none !important;
			border-radius: 0px !important;
			background: none !important;
		}

		table {
			table-layout: unset !important;
		}

		.address-width {
			width: 40%;
		}

		.memo-width {
			width: 50%;
		}

		.memo-right-column {
			margin-left: 20px;
		}

		.shippingItem {
			display: none;
		}

		.orderItem {
			display: none;
		}

		.show {
			display: block;
		}

		.showOrder {
			display: block;
		}

		.shipping-list-th {
			font-size: 13px !important;
		}

		.product-list-table tr th {
			position: sticky;
			top: 0;
			left: 0;
		}

		.selected-row {
			background-color: #FFEECC;
		}
	</style>
	<asp:HiddenField ID="hfPaidyTokenId" runat="server" />
	<asp:Button ID="btnTooltipInfo" runat="server" Style="display: none;" />
	<div class="main-contents page-order-regist">
		<h1 class="page-title">新規注文登録</h1>
		<div style="width: 100%">
			<table class="info_table" id="tableNotProductOrderLimitErrorMessages" runat="server" visible="false" style="margin-bottom: 10px;">
				<tbody>
					<tr class="info_item_bg">
						<td align="left" class="info_item_bg" style="color: red; font-weight: bold">過去にご購入されている商品が含まれています。<br />
							※登録すると拡張ステータス39がオンで登録されます。
						</td>
					</tr>
				</tbody>
			</table>
		</div>
		<div id="dvOrderShippingAlert" runat="server" visible="false" style="margin: 5px 0;">
			<asp:Label ID="lbOrderShippingAlertMessage" runat="server" ForeColor="Red" />
		</div>
		<div id="dvMessagePointMinimum" runat="server" visible="false" style="margin: 5px 0; width: -webkit-fill-available; background-color: #e1e1e1; padding: 10px; border: 2px solid #aaaaaa;">
			<asp:Label ID="lMessagePointMinimum" runat="server" ForeColor="Red" />
		</div>
		<div class="block-order-regist-input is-confirm">
			<div class="block-order-regist-input-block1">
				<!-- ▽注文区分▽ -->
				<div class="block-order-regist-input-section order-info">
					<div class="block-order-regist-input-section-contents">
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								注文区分
							</div>
							<div class="form-element-group-content">
								<asp:Literal ID="lOrderKbn" runat="server" />
							</div>
						</div>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								広告コード
							</div>
							<div class="form-element-group-content">
								<asp:Literal ID="lAdvCode" runat="server" />
								<div class="media-name">
									<div class="media-name-contents">
										<span>
											<asp:Literal ID="lAdvName" runat="server" /></span>
									</div>
								</div>
							</div>
						</div>
					</div>
				</div>
				<!-- △注文区分△ -->
				<!-- ▽注文者情報▽ -->
				<div class="block-order-regist-input-section user-info">
					<div class="block-order-regist-input-section-header">
						<h2 class="block-order-regist-input-section-title">
							<span class="block-order-regist-input-section-title-icon icon-user"></span>
							<span class="block-order-regist-input-section-title-label">注文者情報</span>
						</h2>
						<div class="block-order-regist-input-section-header-right"></div>
					</div>
					<div class="tab-contents">
						<div class="tab-content" id="tab-content-1">
							<div class="tab-content-contents">
								<div id="dvUserId" class="form-element-group form-element-group-horizontal-grid" runat="server">
									<div class="form-element-group-title">
										ユーザーID
									</div>
									<div class="form-element-group-content">
										<asp:Literal ID="lUserId" runat="server" />
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.name.name@@") %>
									</div>
									<div class="form-element-group-content">
										<asp:Literal ID="lOwnerName" runat="server" />
									</div>
								</div>
								<% if (this.IsOwnerAddrJp)
									{ %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.name_kana.name@@") %>
									</div>
									<div class="form-element-group-content">
										<asp:Literal ID="lOwnerNameKana" runat="server" />
									</div>
								</div>
								<% } %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										注文者区分
									</div>
									<div class="form-element-group-content">
										<asp:Literal ID="lOwnerKbn" runat="server" />
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.tel1.name@@", this.OwnerAddrCountryIsoCode) %>
									</div>
									<div class="form-element-group-content">
										<asp:Literal ID="lOwnerTel1" runat="server" />
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.tel2.name@@", this.OwnerAddrCountryIsoCode) %>
									</div>
									<div class="form-element-group-content">
										<asp:Literal ID="lOwnerTel2" runat="server" />
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										メールアドレス
									</div>
									<div class="form-element-group-content">
										<asp:Literal ID="lOwnerMailAddr" runat="server" />
									</div>
								</div>
								<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED)
									{ %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: StringUtility.ToHankaku("モバイルメールアドレス") %>
									</div>
									<div class="form-element-group-content">
										<asp:Literal ID="lOwnerMailAddr2" runat="server" />
									</div>
								</div>
								<% } %>
								<% if (Constants.GLOBAL_OPTION_ENABLE)
									{ %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.country.name@@", this.OwnerAddrCountryIsoCode) %>
									</div>
									<div class="form-element-group-content">
										<asp:Literal ID="lOwnerCountryName" runat="server" />
									</div>
								</div>
								<% } %>
								<% if (this.IsOwnerAddrJp)
									{ %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.zip.name@@") %>
									</div>
									<div class="form-element-group-content">
										<asp:Literal ID="lOwnerZip" runat="server" />
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.addr1.name@@") %>
									</div>
									<div class="form-element-group-content">
										<asp:Literal ID="lOwnerAddr1" runat="server" />
									</div>
								</div>
								<% } %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.addr2.name@@", this.OwnerAddrCountryIsoCode) %>
									</div>
									<div class="form-element-group-content">
										<asp:Literal ID="lOwnerAddr2" runat="server" />
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.addr3.name@@", this.OwnerAddrCountryIsoCode) %>
									</div>
									<div class="form-element-group-content">
										<asp:Literal ID="lOwnerAddr3" runat="server" />
									</div>
								</div>
								<% if (Constants.DISPLAY_ADDR4_ENABLED || (this.IsOwnerAddrJp == false))
									{ %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: StringUtility.ToHankaku(ReplaceTag("@@User.addr4.name@@", this.OwnerAddrCountryIsoCode)) %>
									</div>
									<div class="form-element-group-content">
										<asp:Literal ID="lOwnerAddr4" runat="server" />
									</div>
								</div>
								<% } %>
								<% if (this.IsOwnerAddrJp == false)
									{ %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.addr5.name@@", this.OwnerAddrCountryIsoCode) %>
									</div>
									<div class="form-element-group-content">
										<asp:Literal ID="lOwnerAddr5" runat="server" />
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.zip.name@@", this.OwnerAddrCountryIsoCode) %>
									</div>
									<div class="form-element-group-content">
										<asp:Literal ID="lOwnerZipGlobal" runat="server" />
									</div>
								</div>
								<% } %>
								<% if (Constants.DISPLAY_CORPORATION_ENABLED)
									{ %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.company_name.name@@")%>
									</div>
									<div class="form-element-group-content">
										<asp:Literal ID="lOwnerCompanyName" runat="server" />
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.company_post_name.name@@")%>
									</div>
									<div class="form-element-group-content">
										<asp:Literal ID="lOwnerCompanyPostName" runat="server" />
									</div>
								</div>
								<% } %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.sex.name@@") %>
									</div>
									<div class="form-element-group-content">
										<asp:Literal ID="lOwnerSex" runat="server" />
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.birth.name@@") %>
									</div>
									<div class="form-element-group-content">
										<asp:Literal ID="lOwnerBirth" runat="server" />
									</div>
								</div>
								<% if (Constants.GLOBAL_OPTION_ENABLE)
									{ %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										アクセス国ISOコード
									</div>
									<div class="form-element-group-content">
										<asp:Literal ID="lOwnerAccessCountryIsoCode" runat="server" />
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										表示言語コード
									</div>
									<div class="form-element-group-content">
										<asp:Literal ID="lOwnerDispLanguageCode" runat="server" />
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										表示言語ロケールID
									</div>
									<div class="form-element-group-content">
										<asp:Literal ID="lOwnerDispLanguageLocaleId" runat="server" />
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										表示通貨コード
									</div>
									<div class="form-element-group-content">
										<asp:Literal ID="lOwnerDispCurrencyCode" runat="server" />
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										表示通貨ロケールID
									</div>
									<div class="form-element-group-content">
										<asp:Literal ID="lOwnerDispCurrencyLocaleId" runat="server" />
									</div>
								</div>
								<% } %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										ユーザー特記欄
									</div>
									<div class="form-element-group-content group-memo-area">
										<asp:Literal ID="lUserMemo" runat="server" />
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: StringUtility.ToHankaku("ユーザー管理レベル") %>
									</div>
									<div class="form-element-group-content">
										<asp:Literal ID="lUserManagementLevel" runat="server" />
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										メール配信希望
									</div>
									<div class="form-element-group-content">
										<asp:Literal ID="lMailFlg" runat="server" />
									</div>
								</div>
								<div id="dvAllowOrderOwnerSaveToUser" class="form-element-group user-info-save-owner" runat="server">
									<% if (this.CanUpdateUser)
										{ %>
									<asp:Literal runat="server">注文者情報の変更をユーザー情報に反映する</asp:Literal>
									<% }
										else
										{ %>
									<asp:Literal ID="Literal1" runat="server">注文者情報の変更をユーザー情報に反映しない</asp:Literal>
									<% } %>
								</div>
							</div>
						</div>
					</div>
				</div>
				<!-- △注文者情報△ -->
			</div>
			<div class="block-order-regist-input-block2">

				<div id="dvOrderIdErrorMessages" class="block-order-regist-input-section-error" runat="server" visible="false">
					<asp:Literal ID="IOrderIdErrorMessage" runat="server" />
				</div>

				<!-- ▽受注一覧▽ -->
				<div class="block-order-regist-input-section product-data">
					<div class="block-order-regist-input-section-header">
						<h2 class="block-order-regist-input-section-title">
							<span class="block-order-regist-input-section-title-icon icon-goods"></span>
							<span class="block-order-regist-input-section-title-label">受注一覧</span>
						</h2>
					</div>
					<div class="product-list" style="overflow-y: scroll; min-height: 40px; max-height: 82px; white-space: nowrap;">
						<table class="product-list-table">
							<tr>
								<th class="shipping-list-th">受注ID</th>
								<th class="shipping-list-th">配送拠点</th>
							</tr>
							<!-- ▽一覧▽ -->
							<asp:HiddenField ID="hfSelectedShippingNo" runat="server" />
							<asp:HiddenField ID="hfSelectedOrderId" runat="server" />
							<asp:HiddenField ID="hfSelectedOrderIndex" runat="server" />
							<asp:HiddenField ID="hfSelectedShippingBaseId" runat="server" />
							<asp:Repeater ID="rOrderListForTable" ItemType="w2.App.Common.Order.CartObject" runat="server">
								<ItemTemplate>
									<tbody class="orderLine <%#: (Container.ItemIndex == 0) ? "active" : ""%>" onclick="selectOrder('<%#: Container.ItemIndex %>','<%#: Item.OrderId %>',this)">
										<tr>
											<td class="shipping-list-td"><%#: Item.OrderId %></td>
											<td class="shipping-list-td"><%#: GetShippingBaseId(Item.OrderId) %></td>
										</tr>
									</tbody>
								</ItemTemplate>
							</asp:Repeater>
							<!-- △一覧△ -->
						</table>
					</div>
				</div>
				<!-- △受注一覧△ -->

				<!-- ▽配送先一覧▽ -->
				<div class="block-order-regist-input-section product-data">
					<div class="block-order-regist-input-section-header">
						<h2 class="block-order-regist-input-section-title">
							<span class="block-order-regist-input-section-title-label">配送先一覧</span>
						</h2>
					</div>
					<div class="product-list" style="overflow-y: scroll; min-height: 40px; max-height: 82px; white-space: nowrap;">
						<table class="product-list-table">
							<tr>
								<th class="shipping-list-th">送り主</th>
								<th class="shipping-list-th">お届け先</th>
								<th class="shipping-list-th">商品</th>
								<th class="shipping-list-th">商品計</th>
								<th class="shipping-list-th">値引等</th>
							</tr>
							<!-- ▽一覧▽ -->
							<asp:HiddenField ID="hfSelectedShopShippingIndex" runat="server" />
							<asp:HiddenField ID="hfSelectedShopShippingNo" runat="server" />
							<asp:Repeater ID="rShippingListForTable" ItemType="w2.App.Common.Order.CartShipping" runat="server">
								<ItemTemplate>
									<tbody class="shippingLine <%#: (Container.ItemIndex == 0) ? "active" : ""%>" onclick="selectShopShipping('<%#: Container.ItemIndex %>','<%#: Item.ShippingNo %>','<%#:Item.ProductCounts[0].Product.ProductJointName %>',this)">
										<tr>
											<asp:HiddenField ID="hfActiveShippingIndex" runat="server" Value="0"/>	
											<% if (false)
											{ %>
											<td class="shipping-list-td" ><%#: Container.ItemIndex %></td>
											<% } %>
											<td class="shipping-list-td"><%#: (Item.SenderAddrKbn == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER) ? "★注文者" : Item.SenderName1 + Item.SenderName2 %></td>
											<td class="shipping-list-td"><%#:(Item.ShippingAddrKbn == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER) ? "★注文者" : Item.Name %></td>
											<td class="shipping-list-td"><%#:Item.ProductCounts[0].Product.ProductJointName %></td>
											<td class="ta-right shipping-list-td"><%#:Item.PriceSubtotal.ToPriceString(true) %></td>
											<td class="ta-right shipping-list-td" style="color: red;">
												<%# GetMinusNumberNoticeHtml(-1 * (GetOrderSetPromotionDiscountByShipping(Item.ShippingNo) + Item.GetSpecialFpProductDiscountAmount() + Item.PriceShippingDiscountBySpecialFp), true) %>
											</td>
										</tr>
									</tbody>
								</ItemTemplate>
							</asp:Repeater>
							<!-- △一覧△ -->
						</table>
					</div>
				</div>
				<!-- △配送先一覧△ -->

				<asp:Repeater ID="rShippingList" ItemType="w2.App.Common.Order.CartShipping" runat="server">
					<ItemTemplate>
						<div class="shippingItem <%#: (Container.ItemIndex == 0) ? "show" : ""%>">
							<div class="block-order-regist-input-section-row" style="justify-content: flex-start;">
								<!-- ▽送り主情報▽ -->
								<div class="block-order-regist-input-section address-width" style="width: 40%;" runat="server">
									<div class="block-order-regist-input-section-header">
										<h2 class="block-order-regist-input-section-title">
											<span class="block-order-regist-input-section-title-icon icon-user"></span>
											<span class="block-order-regist-input-section-title-label">送り主情報</span>
										</h2>
									</div>
									<div id="dvOrderSenderSameAsOwner" class="block-order-regist-input-section-contents shipping-same-as-owner-contents" runat="server"
										visible="<%# (Item.SenderAddrKbn == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER) %>">
										<div class="form-element-group form-element-group-horizontal-grid">
											送り主を注文者と同じにする
										</div>
									</div>
									<div id="dvOrderSender" class="block-order-regist-input-section-contents shipping-same-as-owner-contents" runat="server"
										visible="<%# (Item.SenderAddrKbn != CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER) %>">
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												<%: ReplaceTag("@@User.name.name@@") %>
											</div>
											<div class="form-element-group-content">
												<%#: Item.SenderName1 %><%#: Item.SenderName2 %>
											</div>
										</div>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												<%: ReplaceTag("@@User.name_kana.name@@") %>
											</div>
											<div class="form-element-group-content">
												<%#: Item.SenderNameKana1 %><%#: Item.SenderNameKana2 %>
											</div>
										</div>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												<%: ReplaceTag("@@User.zip.name@@") %>
											</div>
											<div class="form-element-group-content">
												<%#: Item.SenderZip %>
											</div>
										</div>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												<%: ReplaceTag("@@User.addr1.name@@") %>
											</div>
											<div class="form-element-group-content">
												<%#: Item.SenderAddr1 %>
											</div>
										</div>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												<%: ReplaceTag("@@User.addr2.name@@", Constants.COUNTRY_ISO_CODE_JP) %>
											</div>
											<div class="form-element-group-content">
												<%#: Item.SenderAddr2 %>
											</div>
										</div>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												<%: ReplaceTag("@@User.addr3.name@@", Constants.COUNTRY_ISO_CODE_JP) %>
											</div>
											<div class="form-element-group-content">
												<%#: Item.SenderAddr3 %>
											</div>
										</div>
										<% if (Constants.DISPLAY_ADDR4_ENABLED || (this.IsShippingAddrJp == false))
											{ %>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												<%: StringUtility.ToHankaku(ReplaceTag("@@User.addr4.name@@", Constants.COUNTRY_ISO_CODE_JP)) %>
											</div>
											<div class="form-element-group-content">
												<%#: Item.SenderAddr4 %>
											</div>
										</div>
										<% } %>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												<%: ReplaceTag("@@User.tel1.name@@") %>
											</div>
											<div class="form-element-group-content">
												<%#: Item.SenderTel1 %>
											</div>
										</div>
									</div>
								</div>
								<!-- △送り主情報△ -->
								<!-- ▽配送先情報▽ -->
								<div class="block-order-regist-input-section ShippingTo address-width" runat="server" style="margin-left: 40px;">
									<div class="block-order-regist-input-section-header">
										<h2 class="block-order-regist-input-section-title">
											<span class="block-order-regist-input-section-title-icon icon-shippingto"></span>
											<span class="block-order-regist-input-section-title-label">配送先情報</span>
										</h2>
									</div>
									<div id="dvOrderShippingSameAsOwner" class="block-order-regist-input-section-contents shipping-same-as-owner-contents" runat="server"
										visible="<%# (Item.ShippingAddrKbn == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER) %>">
										<div class="form-element-group form-element-group-horizontal-grid">
											配送先を注文者と同じにする
										</div>
									</div>
									<div id="dvOrderShipping" class="block-order-regist-input-section-contents shipping-same-as-owner-contents" runat="server"
										visible="<%# (Item.ShippingAddrKbn != CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER) %>">
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												<%: ReplaceTag("@@User.name.name@@") %>
											</div>
											<div class="form-element-group-content">
												<%#: Item.Name %>
											</div>
										</div>
										<% if (this.IsShippingAddrJp)
											{ %>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												<%: ReplaceTag("@@User.name_kana.name@@") %>
											</div>
											<div class="form-element-group-content">
												<%#: Item.NameKana %>
											</div>
										</div>
										<% } %>
										<% if (Constants.GLOBAL_OPTION_ENABLE)
											{ %>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												<%: ReplaceTag("@@User.country.name@@", this.ShippingAddrCountryIsoCode) %>
											</div>
											<div class="form-element-group-content">
												<%#: Item.ShippingCountryName %>
											</div>
										</div>
										<% } %>
										<% if (this.IsShippingAddrJp)
											{ %>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												<%: ReplaceTag("@@User.zip.name@@") %>
											</div>
											<div class="form-element-group-content">
												<%#: Item.Zip %>
											</div>
										</div>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												<%: ReplaceTag("@@User.addr1.name@@") %>
											</div>
											<div class="form-element-group-content">
												<%#: Item.Addr1 %>
											</div>
										</div>
										<% } %>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												<%: ReplaceTag("@@User.addr2.name@@", this.ShippingAddrCountryIsoCode) %>
											</div>
											<div class="form-element-group-content">
												<%#: Item.Addr2 %>
											</div>
										</div>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												<%: ReplaceTag("@@User.addr3.name@@", this.ShippingAddrCountryIsoCode) %>
											</div>
											<div class="form-element-group-content">
												<%#: Item.Addr3 %>
											</div>
										</div>
										<% if (Constants.DISPLAY_ADDR4_ENABLED || (this.IsShippingAddrJp == false))
											{ %>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												<%: StringUtility.ToHankaku(ReplaceTag("@@User.addr4.name@@", Constants.COUNTRY_ISO_CODE_JP)) %>
											</div>
											<div class="form-element-group-content">
												<%#: Item.Addr4 %>
											</div>
										</div>
										<% } %>
										<% if (this.IsShippingAddrJp == false)
											{ %>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												<%: ReplaceTag("@@User.addr5.name@@", this.ShippingAddrCountryIsoCode) %>
											</div>
											<div class="form-element-group-content">
												<asp:Literal ID="lShippingAddr5" runat="server" />
											</div>
										</div>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												<%: ReplaceTag("@@User.zip.name@@") %>
											</div>
											<div class="form-element-group-content">
												<asp:Literal ID="lShippingZipGlobal" runat="server" />
											</div>
										</div>
										<% } %>
										<% if (Constants.DISPLAY_CORPORATION_ENABLED)
											{ %>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												<%: ReplaceTag("@@User.company_name.name@@")%>
											</div>
											<div class="form-element-group-content">
												<%#: Item.CompanyName %>
											</div>
										</div>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												<%: ReplaceTag("@@User.company_post_name.name@@")%>
											</div>
											<div class="form-element-group-content">
												<%#: Item.CompanyPostName %>
											</div>
										</div>
										<% } %>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												<% if (this.IsShippingAddrJp)
													{ %>
												<%: ReplaceTag("@@User.tel1.name@@") %>
												<% }
													else
													{ %>
												<%: ReplaceTag("@@User.tel1.name@@", this.ShippingAddrCountryIsoCode) %>
												<% } %>
											</div>
											<div class="form-element-group-content">
												<%#: Item.Tel1 %>
											</div>
										</div>
									</div>
									<div id="dvOrderShippingConvenienceStore" class="block-order-regist-input-section-contents shipping-same-as-owner-contents" runat="server" visible="False">
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												店舗ID
											</div>
											<div class="form-element-group-content">
												<asp:Literal ID="lCvsShopNo" runat="server" />
											</div>
										</div>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												店舗名
											</div>
											<div class="form-element-group-content">
												<asp:Literal ID="lCvsShopName" runat="server" />
											</div>
										</div>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												店舗住所
											</div>
											<div class="form-element-group-content">
												<asp:Literal ID="lCvsShopAddress" runat="server" />
											</div>
										</div>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												店舗電話番号
											</div>
											<div class="form-element-group-content">
												<asp:Literal ID="lCvsShopTel" runat="server" />
											</div>
										</div>
									</div>
								</div>
								<!-- △配送先情報△ -->
							</div>

						</div>
					</ItemTemplate>
				</asp:Repeater>

				<!-- ▽商品情報▽ -->
				<div class="block-order-regist-input-section product-data">
					<div class="block-order-regist-input-section-header">
						<h2 class="block-order-regist-input-section-title">
							<span class="block-order-regist-input-section-title-icon icon-goods"></span>
							<span class="block-order-regist-input-section-title-label">商品情報</span>
						</h2>
						<div class="block-order-regist-input-section-header-right"></div>
					</div>
					<div class="product-list">
						<table class="product-list-table">
							<tbody>
								<tr>
									<th class="number" rowspan="3">行番号</th>
									<th class="thum" rowspan="3">商品<br />
										画像</th>
									<th class="product-id">商品ID</th>
									<th class="variation-id">バリエーションID</th>
									<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
										{ %>
									<th class="fixed-purchase">定期</th>
									<% } %>
									<% if (Constants.PRODUCT_SALE_OPTION_ENABLED)
										{ %>
									<th class="product-sale-id">セールID</th>
									<% } %>
									<th class="product-price" rowspan="3">単価</th>
									<th class="item-quantity" rowspan="3">数量</th>
									<th class="tax" rowspan="3">税率</th>
									<th class="item-price" rowspan="3">小計</th>
									<th class="delete" rowspan="3"></th>
								</tr>
								<tr>
									<th class="product-name" colspan="<%= this.ProductColSpanNumber %>">商品名</th>
								</tr>
								<tr>
									<th class="option" colspan="<%= this.ProductColSpanNumber %>">付帯情報</th>
								</tr>
								<tr>
									<td colspan="11" class="separate"></td>
								</tr>
								<!-- 商品情報詳細 -->

								<asp:Repeater ID="rItemList" ItemType="w2.App.Common.Order.CartShipping.ProductCount" runat="server">
									<ItemTemplate>
										<tr>
											<td class="number" rowspan="3"><%#: Container.ItemIndex + 1 %></td>
											<td class="thum" rowspan="3">
												<%# ProductImage.GetHtmlImageTag(Item.Product, ProductType.Product, SiteType.Pc, Constants.PRODUCTIMAGE_FOOTER_S) %>
												<td><%#: Item.Product.ProductId %></td>
												<td><%#: ((string.IsNullOrEmpty(Item.Product.VId) == false) ? Item.Product.VId : "-") %></td>
												<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
													{ %>
												<td class="ta-center"><span class="<%#: (Item.Product.IsFixedPurchase ? "icon-check" : "icon") %>"></span></td>
												<% } %>
												<% if (Constants.PRODUCT_SALE_OPTION_ENABLED)
													{ %>
												<td><%#: ((string.IsNullOrEmpty(Item.Product.ProductSaleId) == false) ? Item.Product.ProductSaleId : "-") %></td>
												<% } %>
												<td class="ta-right" rowspan="3"><%#: Item.Product.Price.ToPriceString(true) %></td>
												<td rowspan="3"><%#: Item.Count %></td>
												<td rowspan="3"><%#: TaxCalculationUtility.GetTaxRateForDIsplay(Item.Product.TaxRate) %>%</td>
												<td class="ta-right" rowspan="3"><%#: (Item.Product.Price * Item.Count).ToPriceString(true) %></td>
												<td class="ta-center" rowspan="3"></td>
										</tr>
										<tr>
											<td colspan="<%= this.ProductColSpanNumber %>" style="padding-bottom: 5px; word-break: break-all">
												<% if (Constants.ORDER_COMBINE_OPTION_ENABLED)
													{ %>
												<%#: OrderCombineUtility.GetCartProductChangesByOrderCombine(Item.Product) %><br />
												<% } %>
												<%#: Item.Product.ProductJointName %>
											</td>
										</tr>
										<tr>
											<td colspan="<%= this.ProductColSpanNumber %>" class="group-memo-area">
												<%#: Item.Product.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues() %>
											</td>
										</tr>
										<tr>
											<td colspan="11" class="separate"></td>
										</tr>
									</ItemTemplate>
								</asp:Repeater>
								<asp:Repeater ID="rSetPromotion" ItemType="CartSetPromotion" runat="server">
									<ItemTemplate>
										<asp:Repeater ID="rSetPromotionItem" ItemType="CartProduct" DataSource="<%# Item.Items %>" runat="server">
											<ItemTemplate>
												<tr>
													<td class="thum" rowspan="3">
														<%# ProductImage.GetHtmlImageTag(Item, ProductType.Product, SiteType.Pc, Constants.PRODUCTIMAGE_FOOTER_S)%>
														<td><%#: Item.ProductId %></td>
														<td><%#: ((string.IsNullOrEmpty(Item.VId) == false) ? Item.VId : "-") %></td>
														<td class="ta-center"><span class="<%#: (Item.IsFixedPurchase ? "icon-check" : "icon") %>"></span></td>
														<% if (Constants.PRODUCT_SALE_OPTION_ENABLED)
															{ %>
														<td><%#: ((string.IsNullOrEmpty(Item.ProductSaleId) == false) ? Item.ProductSaleId : "-") %></td>
														<% } %>
														<td class="ta-right" rowspan="3"><%#: Item.Price.ToPriceString(true) %></td>
														<td rowspan="3"><%#: Item.QuantityAllocatedToSet[((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).CartSetPromotionNo] %></td>
														<td rowspan="3"><%#: TaxCalculationUtility.GetTaxRateForDIsplay(Item.TaxRate) %>%</td>
														<td class="ta-right" rowspan="3">
															<%#: (Item.Price * Item.QuantityAllocatedToSet[((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).CartSetPromotionNo]).ToPriceString(true) %>
														</td>
														<td class="ta-center" rowspan="3"></td>
												</tr>
												<tr>
													<td style="word-break: break-all" colspan="4">
														<%# CreateProductJointNameHtml(Item) %>
												[<%#: ((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).CartSetPromotionNo %>：
												<%#: ((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).SetpromotionName %>]
													</td>
												</tr>
												<tr>
													<td colspan="4" style="word-break: break-all"><%#: Item.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues() %></td>
												</tr>
												<tr>
													<td colspan="11" class="separate"></td>
												</tr>
											</ItemTemplate>
										</asp:Repeater>
									</ItemTemplate>
								</asp:Repeater>
								<!-- //商品情報詳細 -->
							</tbody>
						</table>
					</div>
				</div>
				<!-- △商品情報△ -->

				<!-- ▽配送指定情報▽ -->
				<div class="block-order-regist-input-section ShippingInfo" id="dvShippingInfo" runat="server">
					<div class="block-order-regist-input-section-header">
						<h2 class="block-order-regist-input-section-title">
							<span class="block-order-regist-input-section-title-icon icon-shipping"></span>
							<span class="block-order-regist-input-section-title-label">配送指定</span>
						</h2>
					</div>

					<div class="block-order-regist-input-section-contents" visible="true" runat="server" id="dvHideDeliveryDesignation" style="text-align: left">
						<p class="note">注⽂者/商品情報確定後に表⽰されます</p>
					</div>

					<div runat="server" id="dvShowDeliveryDesignation" visible="false">
						<div class="block-order-regist-input-section-error" id="dvDeliveryErrorMessage" visible="false" runat="server">
							<asp:Literal ID="lShippingInfoErrorMessages" runat="server" />
						</div>
						<div class="block-order-regist-input-section-contents">
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title">
									配送方法
								</div>
								<div class="form-element-group-content">
									<asp:DropDownList class="w100" ID="ddlShippingMethod" OnSelectedIndexChanged="ddlShippingMethod_SelectedIndexChanged" runat="server" AutoPostBack="true" />
									<p style="padding: 5px 0 5px 0">
										<asp:Button ID="btnSetShippingMethod" class="btn btn-main btn-size-s" runat="server" Text="  配送方法自動判定  " OnClick="btnSetShippingMethod_Click" />
									</p>
								</div>
							</div>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title">
									配送サービス
								</div>
								<asp:HiddenField ID="hfSelectedDeliveryCompanyId" runat="server" />
								<div class="form-element-group-content">
									<asp:DropDownList class="w100" ID="ddlDeliveryCompany" OnSelectedIndexChanged="ddlDeliveryCompany_SelectedIndexChanged" runat="server" AutoPostBack="true" />
								</div>
							</div>
							<div class="form-element-group form-element-group-horizontal-grid" id="dvShippingTime" runat="server">
								<div class="form-element-group-title">
									配送希望時間帯
								</div>
								<div class="form-element-group-content">
									<asp:DropDownList ID="ddlShippingTime" runat="server" DataTextField="Text" DataValueField="Value" class="w100" />
								</div>
							</div>
							<div class="form-element-group form-element-group-horizontal-grid" id="dvShippingDate" runat="server">
								<div class="form-element-group-title">
									配送希望日
								</div>
								<div class="form-element-group-content">
									<asp:DropDownList ID="ddlShippingDate" OnSelectedIndexChanged="ddlShippingDate_SelectedIndexChanged" runat="server" AutoPostBack="true" class="w100" />
									<p>
										<span class="note" style="color: red">
											<asp:Literal ID="lShippingDateErrorMessages" runat="server" />
										</span>
									</p>
								</div>
							</div>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title">
									配送伝票番号
								</div>
								<div class="form-element-group-content">
									<asp:TextBox ID="tbShippingCheckNo" OnTextChanged="tbShippingCheckNo_TextChanged" AutoPostBack="true" MaxLength="50" class="w100" runat="server" />
								</div>
							</div>
						</div>
					</div>
				</div>
				<!-- △配送指定情報△ -->

				<!-- ▽メモ▽ -->
				<asp:HiddenField ID="hfMemoData" runat="server" />
				<div class="block-order-regist-input-section memo">
					<div class="block-order-regist-input-section-header">
						<h2 class="block-order-regist-input-section-title">
							<span class="block-order-regist-input-section-title-icon icon-memo"></span>
							<span class="block-order-regist-input-section-title-label">メモ</span>
						</h2>
					</div>
					<div class="block-order-regist-input-section-contents">
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<span class="memo-title-text">調整金額メモ
							<uc:FieldMemoSetting runat="server"
								Title="調整金額メモ"
								FieldMemoSettingList="<%# this.FieldMemoSettingList %>"
								TableName="<%# Constants.TABLE_ORDER %>"
								FieldName="<%# Constants.FIELD_ORDER_REGULATION_MEMO %>" />
								</span>
							</div>
							<div class="form-element-group-content" style="width: 131px">
								<asp:TextBox ID="tbRegulationMemo" OnTextChanged="tbMemo_TextChanged" runat="server" TextMode="MultiLine" Rows="2" AutoPostBack="true" />
							</div>
						</div>
						<!-- ▽メモ情報▽ -->
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<div class="form-element-group-title">
									<span class="memo-title-text" style="width: 131px">注文メモ
								<% if (cbReflectMemoToFixedPurchase.Visible)
									{ %>
										<br />
										<asp:CheckBox ID="cbReflectMemoToFixedPurchase" Text="定期台帳へも反映" Visible="false" Checked="true" runat="server" />
										<% } %>
										<uc:FieldMemoSetting runat="server"
											Title="注文メモ"
											FieldMemoSettingList="<%# this.FieldMemoSettingList %>"
											TableName="<%# Constants.TABLE_ORDER %>"
											FieldName="<%# Constants.FIELD_ORDER_MEMO %>" />
									</span>
								</div>
							</div>
							<div class="form-element-group-content" style="width: 131px">
								<asp:Repeater ID="rOrderMemos" runat="server">
									<ItemTemplate>
										<p class="memo-input-title">[<%# Eval(Constants.FIELD_ORDERMEMOSETTING_ORDER_MEMO_NAME) %>]</p>
										<asp:TextBox ID="tbMemo" runat="server" Text="<%# Eval(Constants.FIELD_ORDERMEMOSETTING_DEFAULT_TEXT) %>" TextMode="MultiLine" Rows="8" Style="height: 100px;" />
									</ItemTemplate>
								</asp:Repeater>
							</div>
							<div id="dvOrderMemoForCombine" class="form-element-group-content" runat="server" visible="false">
								<asp:TextBox ID="tbMemoForCombine" runat="server" TextMode="MultiLine" Rows="8" Style="height: 100px;" />
							</div>
						</div>
						<!-- △メモ情報△ -->
						<!-- ▽決済連携メモ情報▽ -->
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<span class="memo-title-text">決済連携メモ
							<uc:FieldMemoSetting runat="server"
								Title="決済連携メモ"
								FieldMemoSettingList="<%# this.FieldMemoSettingList %>"
								TableName="<%# Constants.TABLE_ORDER %>"
								FieldName="<%# Constants.FIELD_ORDER_PAYMENT_MEMO %>" />
								</span>
							</div>
							<div class="form-element-group-content" style="width: 131px">
								<asp:TextBox ID="tbPaymentMemo" OnTextChanged="tbMemo_TextChanged" AutoPostBack="true" runat="server" TextMode="MultiLine" Rows="2" ClientIDMode="Static" />
							</div>
						</div>
						<!-- △決済連携メモ情報△ -->
						<!-- ▽管理メモ情報▽ -->
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<span class="memo-title-text">管理メモ
							<uc:FieldMemoSetting runat="server"
								Title="管理メモ"
								FieldMemoSettingList="<%# this.FieldMemoSettingList %>"
								TableName="<%# Constants.TABLE_ORDER %>"
								FieldName="<%# Constants.FIELD_ORDER_MANAGEMENT_MEMO %>" />
								</span>
							</div>
							<div class="form-element-group-content" style="width: 131px">
								<asp:TextBox ID="tbManagerMemo" OnTextChanged="tbMemo_TextChanged" AutoPostBack="true" runat="server" TextMode="MultiLine" Rows="2" ClientIDMode="Static" />
							</div>
						</div>
						<!-- △管理メモ情報△ -->
						<!-- ▽定期購入管理メモ情報▽ -->
						<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
							{ %>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<span class="memo-title-text">定期購入管理メモ
								<uc:FieldMemoSetting runat="server"
									Title="定期購入管理メモ"
									FieldMemoSettingList="<%# this.FieldMemoSettingList %>"
									TableName="<%# Constants.TABLE_ORDER %>"
									FieldName="<%# Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_MANAGEMENT_MEMO %>" />
								</span>
							</div>
							<div class="form-element-group-content" style="width: 131px">
								<asp:TextBox ID="tbFixedPurchaseManagementMemo" OnTextChanged="tbMemo_TextChanged" AutoPostBack="true" runat="server" TextMode="MultiLine" Rows="2" ClientIDMode="Static" />
							</div>
						</div>
						<% } %>
						<!-- △定期購入管理メモ情報△ -->
						<!-- ▽外部連携メモ情報▽ -->
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<span class="memo-title-text">外部連携メモ
							<uc:FieldMemoSetting runat="server"
								Title="外部連携メモ"
								FieldMemoSettingList="<%# this.FieldMemoSettingList %>"
								TableName="<%# Constants.TABLE_ORDER %>"
								FieldName="<%# Constants.FIELD_ORDER_RELATION_MEMO %>" />
								</span>
							</div>
							<div class="form-element-group-content" style="width: 131px">
								<asp:TextBox ID="tbRelationMemo" OnTextChanged="tbMemo_TextChanged" AutoPostBack="true" runat="server" TextMode="MultiLine" Rows="2" ClientIDMode="Static" />
							</div>
						</div>
						<!-- △外部連携メモ情報△ -->
						<!-- ▽配送メモ情報▽ -->
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<span class="memo-title-text">配送メモ
							<uc:FieldMemoSetting runat="server"
								Title="配送メモ"
								FieldMemoSettingList="<%# this.FieldMemoSettingList %>"
								TableName="<%# Constants.TABLE_ORDER %>"
								FieldName="<%# Constants.FIELD_ORDER_SHIPPING_MEMO %>" />
								</span>
							</div>
							<div class="form-element-group-content" style="width: 131px">
								<asp:TextBox ID="tbShippingMemo" OnTextChanged="tbMemo_TextChanged" AutoPostBack="true" runat="server" TextMode="MultiLine" Rows="2" ClientIDMode="Static" />
							</div>
						</div>
						<!-- △配送メモ情報△ -->
					</div>
				</div>
				<!-- △メモ△ -->
			</div>

			<div class="block-order-regist-input-block3">
				<div class="block-order-regist-input-section" runat="server">
					<div class="block-order-regist-input-section <%: ((Constants.W2MP_COUPON_OPTION_ENABLED || (Constants.W2MP_POINT_OPTION_ENABLED && this.IsUser)) ? string.Empty : "no-discount")%>">
						<div class="discount-method-list">
							<% if (Constants.W2MP_COUPON_OPTION_ENABLED)
								{ %>
							<div class="discount-method-list-coupon">
								<h3>クーポン利用</h3>
								<div class="discount-method-list-coupon-use">
									<asp:Literal ID="lCouponCode" runat="server" />
								</div>
								<div id="dvCouponDetail" class="discount-method-list-coupon-detail" runat="server">
									<p><span class="discount-method-list-coupon-detail-title">クーポン名(管理用)：</span><asp:Literal ID="lCouponName" runat="server" /></p>
									<p><span class="discount-method-list-coupon-detail-title">クーポン名(ユーザ表示用)：</span><asp:Literal ID="lCouponDispName" runat="server" /></p>
									<p><span class="discount-method-list-coupon-detail-title">クーポン表示額／率：</span><asp:Literal ID="lCouponDiscount" runat="server" /></p>
								</div>
							</div>
							<% } %>
							<% if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsUser)
								{ %>
							<div class="discount-method-list-point">
								<h3>ポイント利用
									<span class="discount-method-list-coupon-available">
										<asp:Literal ID="lUserPointUsable" runat="server" Text="0" />
										<%: Constants.CONST_UNIT_POINT_PT %>利用可能
									</span>
								</h3>
								<div class="discount-method-list-point-use">
									<asp:Literal ID="lOrderPointUse" runat="server" Text="0" />
									<%: Constants.CONST_UNIT_POINT_PT %>
								</div>
								<% if (this.CanUseAllPointFlg)
									{ %>
								<div class="discount-method-list-point-use">
									<div id="dvUseAllPointFlg" runat="server">
										<asp:Literal ID="lUseAllPointFlg" Text="定期注文で利用可能なポイントすべてを継続使用する" runat="server" />
									</div>
								</div>
								<% } %>
							</div>
							<div class="discount-method-list-point-give">
								付与ポイント<span class="point"><asp:Literal ID="lOrderPointAdd" runat="server" Text="0" />
									<%: Constants.CONST_UNIT_POINT_PT%></span>
							</div>
							<% } %>
						</div>
					</div>
				</div>
				<div class="block-order-regist-input-section order-detail-info">
					<table class="order-detail-info-table">
						<tbody>
							<tr>
								<th>商品合計</th>
								<td><span class="price-value">
									<asp:Literal ID="lOrderPriceSubTotal" runat="server" Text="0" /></span></td>
							</tr>
							<% if (this.ProductIncludedTaxFlg == false)
								{ %>
							<tr>
								<th>消費税額</th>
								<td><span class="price-value">
									<asp:Literal ID="lOrderPriceTax" runat="server" /></span></td>
							</tr>
							<% } %>
							<asp:Repeater ID="rOrderSetPromotionProductDiscount" Visible="<%# this.IsAllItemsSubscriptionBoxFixedAmount == false %>" ItemType="CartSetPromotion" runat="server">
								<ItemTemplate>
									<tr visible="<%# Item.IsDiscountTypeProductDiscount %>" runat="server">
										<th>
											<%#: Item.CartSetPromotionNo %>：<%#: Item.SetpromotionName %>割引額<br />
											(ID:<%#: Item.SetpromotionId %>,商品割引分)
										</th>
										<td>
											<span class="price-value" style='<%# ((Item.ProductDiscountAmount > 0) ? "color:red": string.Empty) %>'>
												<%#: (Item.ProductDiscountAmount * -1).ToPriceString(true) %>
											</span>
										</td>
									</tr>
								</ItemTemplate>
							</asp:Repeater>
							<% if (Constants.MEMBER_RANK_OPTION_ENABLED)
								{ %>
							<tr id="trMemberRankDiscount" runat="server">
								<th>会員ランク割引</th>
								<td><span class="price-value">
									<asp:Literal ID="lMemberRankDiscount" runat="server" /></span></td>
							</tr>
							<% } %>
							<%if (Constants.MEMBER_RANK_OPTION_ENABLED && Constants.FIXEDPURCHASE_OPTION_ENABLED)
								{ %>
							<tr id="trFixedPurchaseMemberDiscount" runat="server">
								<th>定期会員割引</th>
								<td><span class="price-value">
									<asp:Literal ID="lFixedPurchaseMemberDiscount" runat="server" /></span></td>
							</tr>
							<% } %>
							<% if (Constants.W2MP_COUPON_OPTION_ENABLED)
								{ %>
							<tr id="trCouponDiscount" runat="server">
								<th>クーポン割引額</th>
								<td><span class="price-value">
									<asp:Literal ID="lCouponUsePrice" runat="server" Text="0" /></span></td>
							</tr>
							<% } %>
							<% if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsUser)
								{ %>
							<tr id="trPointDiscount" runat="server">
								<th>ポイント利用額</th>
								<td><span class="price-value">
									<asp:Literal ID="lPointUsePrice" runat="server" Text="0" /></span></td>
							</tr>
							<% } %>
							<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
								{ %>
							<tr id="trFixedPurchaseDiscountPrice" runat="server">
								<th>定期購入割引額</th>
								<td><span class="price-value">
									<asp:Literal ID="lFixedPurchaseDiscountPrice" runat="server" Text="0" /></span></td>
							</tr>
							<% } %>
							<tr>
								<th>配送料</th>
								<td><span class="price-value">
									<asp:Literal ID="lOrderPriceShipping" runat="server" /></span></td>
							</tr>
							<asp:Repeater ID="rOrderSetPromotionShippingChargeDiscount" ItemType="CartSetPromotion" runat="server">
								<ItemTemplate>
									<tr visible="<%# Item.IsDiscountTypeShippingChargeFree %>" runat="server">
										<th>
											<%#: Item.CartSetPromotionNo %>：<%#: Item.SetpromotionName %>割引額<br />
											(ID:<%#: Item.SetpromotionId %>,配送料割引分)
										</th>
										<td>
											<span class="price-value" style='<%# ((Item.ShippingChargeDiscountAmount > 0) ? "color:red": string.Empty) %>'>
												<%#: (Item.ShippingChargeDiscountAmount * -1).ToPriceString(true) %>
											</span>
										</td>
									</tr>
								</ItemTemplate>
							</asp:Repeater>
							<tr>
								<th>決済手数料</th>
								<td><span class="price-value">
									<asp:Literal ID="lOrderPriceExchange" runat="server" /></span></td>
							</tr>
							<asp:Repeater ID="rOrderSetPromotionPaymentChargeDiscount" ItemType="CartSetPromotion" runat="server">
								<ItemTemplate>
									<tr visible="<%# Item.IsDiscountTypePaymentChargeFree %>" runat="server">
										<td>
											<%#: Item.CartSetPromotionNo %>：<%#: Item.SetpromotionName %>割引額<br />
											(ID:<%#: Item.SetpromotionId %>,決済手数料料割引分)
										</td>
										<td>
											<span class="price-value" style='<%# ((Item.ShippingChargeDiscountAmount > 0) ? "color:red": string.Empty) %>'>
												<%#: (Item.PaymentChargeDiscountAmount * -1).ToPriceString(true) %>
											</span>
										</td>
									</tr>
								</ItemTemplate>
							</asp:Repeater>
							<tr id="trPriceRegulation" class="adjust" runat="server">
								<th>調整金額</th>
								<td>
									<asp:Literal ID="lOrderPriceRegulation" runat="server" /></td>
							</tr>
							<asp:Repeater ID="rTotalPriceByTaxRate" ItemType="CartPriceInfoByTaxRate" runat="server">
								<ItemTemplate>
									<tr>
										<th>合計金額内訳(税率<%#: TaxCalculationUtility.GetTaxRateForDIsplay(Item.TaxRate) %>%)
										</th>
										<td>
											<span class="price-value">
												<%# GetMinusNumberNoticeHtml(Item.PriceTotal, true) %>
											</span>
										</td>
									</tr>
								</ItemTemplate>
							</asp:Repeater>
							<tr class="total">
								<th><strong>金額合計</strong></th>
								<td><strong><span class="price-value">
									<asp:Literal ID="lOrderPriceTotal" runat="server" /></span><strong></strong></strong></td>
							</tr>
							<%if (Constants.GLOBAL_OPTION_ENABLE)
								{ %>
							<tr>
								<th>決済金額</th>
								<td>
									<span class="price-value">
										<asp:Literal ID="lSettlementAmount" runat="server" /></span>
								</td>
							</tr>
							<% } %>
						</tbody>
					</table>
				</div>

				<!-- ▽定期購入配送パターン情報▽ -->
				<asp:HiddenField ID="hfHasFixedPurchase" Value="false" runat="server" />
				<asp:HiddenField ID="hfShippingType" runat="server" />
				<div id="dvShippingFixedPurchase" class="block-order-regist-input-section shipping-pattern" visible="false" runat="server">
					<div class="block-order-regist-input-section-header">
						<h2 class="block-order-regist-input-section-title">
							<span class="block-order-regist-input-section-title-icon icon-calendar"></span>
							<span class="block-order-regist-input-section-title-label">定期配送パターン</span>
						</h2>
					</div>
					<div id="dvFixedPurchasePatternErrorMessages" class="block-order-regist-input-section-error" runat="server" visible="false">
						<asp:Literal ID="lFixedPurchasePatternErrorMessage" runat="server" />
					</div>
					<div class="block-order-regist-input-section-contents">
						<h3 class="form-element-group-list-title">配送パターン</h3>
						<dl class="form-element-group-list">
							<dt id="dtMonthlyPurchase_Date" runat="server">
								<span>
									<asp:RadioButton ID="rbMonthlyPurchase_Date" Text="月間隔日付指定" GroupName="FixedPurchaseShippingPattern" runat="server"
										OnCheckedChanged="rbMonthlyPurchase_Date_CheckedChanged" AutoPostBack="true" />
								</span>
							</dt>
							<dd id="ddMonthlyPurchase_Date" runat="server">
								<asp:DropDownList ID="ddlMonth" DataTextField="Text" DataValueField="Value" runat="server"
									OnSelectedIndexChanged="ddlMonth_ddlIntervalMonths_OnSelectedIndexChanged" AutoPostBack="true" />
								ヶ月ごと
                        <asp:DropDownList ID="ddlMonthlyDate" DataTextField="Text" DataValueField="Value" runat="server"
							OnSelectedIndexChanged="ddlMonth_ddlIntervalMonths_OnSelectedIndexChanged" AutoPostBack="true" />
								日に届ける
                                                                <br />
							</dd>
							<dt id="dtMonthlyPurchase_WeekAndDay" runat="server">
								<span>
									<asp:RadioButton ID="rbMonthlyPurchase_WeekAndDay" Text="月間隔・週・曜日指定" GroupName="FixedPurchaseShippingPattern" runat="server"
										OnCheckedChanged="rbMonthlyPurchase_WeekAndDay_CheckedChanged" AutoPostBack="true" />
								</span>
							</dt>
							<dd id="ddMonthlyPurchase_WeekAndDay" runat="server">
								<asp:DropDownList ID="ddlIntervalMonths" DataTextField="Text" DataValueField="Value" runat="server"
									OnSelectedIndexChanged="ddlMonth_ddlIntervalMonths_OnSelectedIndexChanged" AutoPostBack="true" />
								ヶ月ごと
                        <asp:DropDownList ID="ddlWeekOfMonth" DataTextField="Text" DataValueField="Value" runat="server"
							OnSelectedIndexChanged="ddlMonth_ddlIntervalMonths_OnSelectedIndexChanged" AutoPostBack="true" />
								<asp:DropDownList ID="ddlDayOfWeek" DataTextField="Text" DataValueField="Value" runat="server"
									OnSelectedIndexChanged="ddlMonth_ddlIntervalMonths_OnSelectedIndexChanged" AutoPostBack="true" />
								に届ける
                            <br />
							</dd>
							<dt id="dtRegularPurchase_IntervalDays" runat="server">
								<span>
									<asp:RadioButton ID="rbRegularPurchase_IntervalDays" Text="配送日間隔指定" GroupName="FixedPurchaseShippingPattern" runat="server"
										OnCheckedChanged="rbRegularPurchase_IntervalDays_CheckedChanged" AutoPostBack="true" />
								</span>
							</dt>
							<dd id="ddRegularPurchase_IntervalDays" runat="server">
								<asp:DropDownList ID="ddlIntervalDays" DataTextField="Text" DataValueField="Value" runat="server"
									OnSelectedIndexChanged="ddlIntervalDays_OnSelectedIndexChanged" AutoPostBack="true" />
								日ごとに届ける
                                                                <br />
							</dd>
							<dt id="dtPurchase_EveryNWeek" runat="server">
								<span>
									<asp:RadioButton ID="rbPurchase_EveryNWeek" Text="週間隔・曜日指定" GroupName="FixedPurchaseShippingPattern" runat="server"
										OnCheckedChanged="rbPurchase_EveryNWeek_CheckedChanged" AutoPostBack="true" />
								</span>
							</dt>
							<dd id="ddPurchase_EveryNWeek" runat="server">
								<asp:DropDownList ID="ddlFixedPurchaseEveryNWeek_Week" DataTextField="Text" DataValueField="Value" runat="server"
									OnSelectedIndexChanged="ddlFixedPurchaseEveryNWeek_OnSelectedIndexChanged" AutoPostBack="true" />
								週間ごとの
                            <asp:DropDownList ID="ddlFixedPurchaseEveryNWeek_DayOfWeek" DataTextField="Text" DataValueField="Value" runat="server"
								OnSelectedIndexChanged="ddlFixedPurchaseEveryNWeek_OnSelectedIndexChanged" AutoPostBack="true" />
								に届ける
                                                            <br />
							</dd>
						</dl>
						<asp:HiddenField ID="hfDaysRequired" runat="server" />
						<asp:HiddenField ID="hfMinSpan" runat="server" />
						<!-- ▽初回配送予定日▽ -->
						<h3 class="form-element-group-list-title">初回配送予定日</h3>
						<dl class="form-element-group-list">
							<dt>
								<asp:Literal ID="lFirstShippingDate" runat="server" />
							</dt>
							<dt>
								<asp:DropDownList
									ID="ddlFirstShippingDate"
									Visible="false"
									OnSelectedIndexChanged="ddlFirstShippingDate_SelectedIndexChanged"
									AutoPostBack="true"
									runat="server" />
							</dt>
						</dl>
						<!-- △初回配送予定日△ -->
						<!-- ▽次回配送日指定▽ -->
						<h3 class="form-element-group-list-title">次回配送日指定</h3>
						<dl class="form-element-group-list">
							<dt>
								<asp:Literal ID="lNextShippingDate" runat="server" />
								<br />
								<asp:DropDownList
									ID="ddlNextShippingDate"
									Visible="false"
									OnSelectedIndexChanged="ddlNextShippingDate_SelectedIndexChanged"
									AutoPostBack="true"
									runat="server" />
							</dt>
						</dl>
						<!-- △次回配送日指定△ -->
						<p class="note">※メール便の場合、配送予定日は数日ずれる可能性があります。</p>
					</div>
				</div>
				<!-- △定期購入配送パターン情報△ -->

				<!-- ▽領収書情報▽ -->
				<% if (Constants.RECEIPT_OPTION_ENABLED)
					{ %>
				<div class="block-order-regist-input-section" runat="server">
					<div class="block-order-regist-input-section-header">
						<h2 class="block-order-regist-input-section-title">
							<span class="block-order-regist-input-section-title-label">領収書情報</span>
						</h2>
					</div>
					<div class="block-order-regist-input-section-contents">
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								領収書希望
							</div>
							<div class="form-element-group-content">
								<asp:Literal ID="lReceiptFlg" runat="server" />
							</div>
						</div>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								宛名
							</div>
							<div class="form-element-group-content" style="word-break: break-all;">
								<asp:Literal ID="lReceiptAddress" runat="server" />
							</div>
						</div>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								但し書き
							</div>
							<div class="form-element-group-content" style="word-break: break-all;">
								<asp:Label ID="lReceiptProviso" runat="server" />
							</div>
						</div>
					</div>
				</div>
				<% } %>
				<!-- △領収書情報△ -->
				<!-- ▽コンバージョン情報▽ -->
				<div class="block-order-regist-input-section" runat="server">
					<div class="block-order-regist-input-section-header">
						<h2 class="block-order-regist-input-section-title">
							<span class="block-order-regist-input-section-title-label">コンバージョン情報</span>
						</h2>
					</div>
					<div class="block-order-regist-input-section-contents">
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<%: StringUtility.ToHankaku("流入コンテンツタイプ") %>
							</div>
							<div class="form-element-group-content">
								<asp:Literal ID="lInflowContentsType" runat="server" />
							</div>
						</div>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<%: StringUtility.ToHankaku("流入コンテンツID") %>
							</div>
							<div class="form-element-group-content">
								<asp:Literal ID="lInflowContentsId" runat="server" />
							</div>
						</div>
					</div>
				</div>

				<!-- △コンバージョン情報△ -->
				<!-- ▽決済情報▽ -->
				<div class="block-order-regist-input-section payment-info" id="dvPayment" visible="false" runat="server">
					<div class="block-order-regist-input-section-header">
						<h2 class="block-order-regist-input-section-title">
							<span class="block-order-regist-input-section-title-icon icon-payment"></span>
							<span class="block-order-regist-input-section-title-label">決済情報</span>
							<%--▼▼ PayTg 端末状態保持用 ▼▼--%>
							<asp:HiddenField ID="hfCanUseDevice" runat="server" />
							<asp:HiddenField ID="hfStateMessage" runat="server" />
							<%--▲▲ PayTg 端末状態保持用 ▲▲--%>																				
						</h2>
					</div>
					<%--<div id="dvHideOrderPayment" class="block-order-regist-input-section-contents" runat="server" style="text-align: left">
						<p class="note">注⽂者/商品情報確定後に表⽰されます</p>
					</div>--%>
					<div id="dvShowOrderPayment" runat="server">
						<div id="dvPaymentErrorMessages" class="block-order-regist-input-section-error" runat="server" visible="false">
							<asp:Literal ID="lPaymentErrorMessage" runat="server" />
						</div>
						<div class="block-order-regist-input-section-contents">
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title">
									決済種別																		
								</div>
								<div class="form-element-group-content">
									<asp:DropDownList ID="ddlOrderPaymentKbn" runat="server" OnSelectedIndexChanged="ddlOrderPaymentKbn_SelectedIndexChanged" AutoPostBack="true" Width="80%" /><br />
									<asp:Literal ID="lOrderPaymentInfo" runat="server" /><%#: string.IsNullOrEmpty(this.AccountEmail) ? "PayPalアカウントが紐づいていません。" : string.Format("ご利用可能なPayPal アカウント：{0}, ", this.AccountEmail) %>
									<%--▼▼ クレジット Token保持用 ▼▼--%>
									<asp:HiddenField ID="hfCreditToken" runat="server" />
									<asp:HiddenField ID="hfLastFourDigit" runat="server" />
									<asp:HiddenField ID="hfCreditBincode" runat="server" />
									<%--▲▲ クレジット Token保持用 ▲▲--%>
								</div>
							</div>
							<%-- ▽クレジット決済の場合は表示▽ --%>
							<div id="dvPaymentKbnCredit" runat="server" visible="false">
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										利用クレジットカード
									</div>
									<div class="form-element-group-content">
										<asp:DropDownList ID="ddlUserCreditCard" runat="server"
											DataSource="<%# this.CreditCardList %>"
											DataTextField="CardDispName"
											DataValueField="BranchNo"
											AutoPostBack="True"
											OnSelectedIndexChanged="ddlUserCreditCard_SelectedIndexChanged"
											Style="min-width: 12em; max-width: 12em;" />
									</div>
								</div>
								<div id="divUserCreditCard" runat="server" visible="false">
									<% if (OrderCommon.CreditCompanySelectable)
										{ %>
									<div class="form-element-group form-element-group-horizontal-grid">
										<div class="form-element-group-title">
											カード会社
										</div>
										<div class="form-element-group-content">
											<asp:Literal ID="lCreditCompany" runat="server" />
										</div>
									</div>
									<% } %>
									<div class="form-element-group form-element-group-horizontal-grid">
										<div class="form-element-group-title">
											カード番号
										</div>
										<div class="form-element-group-content">
											************<asp:Literal ID="lCreditLastFourDigit" runat="server" />
										</div>
									</div>
									<div class="form-element-group form-element-group-horizontal-grid">
										<div class="form-element-group-title">
											有効期限
										</div>
										<div class="form-element-group-content">
											<asp:Literal ID="lCreditExpirationMonth" runat="server" />/<asp:Literal ID="lCreditExpirationYear" runat="server" />(月/年)
										</div>
									</div>
									<div class="form-element-group form-element-group-horizontal-grid">
										<div class="form-element-group-title">
											カード名義人
										</div>
										<div class="form-element-group-content" style="white-space: normal; word-break: break-all;">
											<asp:Literal ID="lCreditAuthorName" runat="server" />
										</div>
									</div>
								</div>
								<div id="divCreditCardInputNew" runat="server">
									<%--▼▼▼ カード情報入力フォーム表示 ▼▼▼--%>
									<% if (this.CanUseCreditCardNoForm)
										{ %>
									<%--▼▼ カード情報入力（トークン未取得・利用なし） ▼▼--%>
									<div id="divCreditCardNoToken" runat="server">
										<% if (OrderCommon.CreditCompanySelectable)
											{ %>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												カード会社<span class="notice">*</span>
											</div>
											<div class="form-element-group-content">
												<asp:DropDownList ID="ddlCreditCardCompany" class="w100" runat="server" />
											</div>
										</div>
										<% } %>
										<asp:PlaceHolder ID="phCreditCardNotRakuten" runat="server">
											<div class="form-element-group form-element-group-horizontal-grid">
												<div class="form-element-group-title">
													<% if (this.CreditTokenizedPanUse)
														{ %>永久トークン<% }
																	  else
																	  { %>カード番号<% } %><span class="notice">*</span>
												</div>
												<div id="tdCreditNumber" class="form-element-group-content" runat="server">
													<asp:TextBox ID="tbCreditCardNo1" pattern="[0-9]*" MaxLength="16" autocomplete="off" class="w100" runat="server" />
													<%--▼▼ カード情報取得用 ▼▼--%>
													<input type="hidden" id="hiddenCardInfo" name="hidCinfo" value="<%= CreateGetCardInfoJsScriptForCreditToken() %>" />
													<span id="spanErrorMessageForCreditCard" style="color: red; display: none" runat="server"></span>
													<%--▲▲ カード情報取得用 ▲▲--%>
												</div>
												<div id="tdGetCardInfo" class="form-element-group-content" runat="server">
													<asp:Button ID="btnGetCreditCardInfo" Text="  決済端末と接続  " class="btn btn-main btn-size-s" runat="server" />
													<br />
													<p>※決済端末と接続ボタンを押下したあと、決済端末でカード番号を入力してください。</p>
												</div>
												<div id="payTgModal" class="payTgModal">
													<div class="payTgModalOuter">
														<div class="payTgModalMargin"></div>
														<div class="payTgModalContents">
															<h1 style="font-size: 16px;">PayTG決済結果待機中・・・</h1>
															<br />
															<h1 style="font-size: 16px;">テンキー端末の操作を完了してください。</h1>
														</div>
													</div>
												</div>
											</div>
											<div id="trCreditExpire" class="form-element-group form-element-group-horizontal-grid" runat="server">
												<div class="form-element-group-title">
													有効期限<span class="notice">*</span>
												</div>
												<div class="form-element-group-content">
													<asp:DropDownList ID="ddlCreditExpireMonth" runat="server" />/<asp:DropDownList ID="ddlCreditExpireYear" runat="server" />
													(月/年)
												</div>
											</div>
											<div class="form-element-group form-element-group-horizontal-grid">
												<div class="form-element-group-title">
													カード名義人<span class="notice">*</span>
												</div>
												<div class="form-element-group-content">
													<asp:TextBox ID="tbCreditAuthorName" runat="server" class="w100" autocomplete="off" />
												</div>
											</div>
											<div id="dvSecurityCode" class="form-element-group form-element-group-horizontal-grid" runat="server">
												<div class="form-element-group-title">
													セキュリティコード<span class="notice">*</span>
												</div>
												<div class="form-element-group-content">
													<asp:TextBox ID="tbCreditSecurityCode" runat="server" class="w100" MaxLength="4" autocomplete="off" />
												</div>
											</div>
										</asp:PlaceHolder>
									</div>
									<%--▲▲ カード情報入力（トークン未取得・利用なし） ▲▲--%>

									<%--▼▼ カード情報入力（トークン取得済） ▼▼--%>
									<div id="divCreditCardForTokenAcquired" visible="false" runat="server">
										<% if (OrderCommon.CreditCompanySelectable)
											{ %>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												カード会社
											</div>
											<div class="form-element-group-content">
												<asp:Literal ID="lCreditCardCompanyNameForTokenAcquired" runat="server" />
											</div>
										</div>
										<% } %>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												カード番号
											</div>
											<div class="form-element-group-content">
												************<asp:Literal ID="lLastFourDigitForTokenAcquired" runat="server" />
												<asp:LinkButton ID="lbEditCreditCardNoForToken" OnClick="lbEditCreditCardNoForToken_Click" runat="server" Text="  再入力  " />
											</div>
										</div>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												有効期限
											</div>
											<div class="form-element-group-content">
												<asp:Literal ID="lExpirationMonthForTokenAcquired" runat="server" />
												/
										<asp:Literal ID="lExpirationYearForTokenAcquired" runat="server" />
												(月/年)
											</div>
										</div>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												カード名義人
											</div>
											<div class="form-element-group-content" style="white-space: normal; word-break: break-all;">
												<asp:Literal ID="lCreditAuthorNameForTokenAcquired" runat="server" />
											</div>
										</div>
									</div>
									<%--▲▲ カード情報入力（トークン取得済） ▲▲ --%>
									<%--▲▲▲ カード情報入力フォーム表示 ▲▲▲--%>
									<% }
										else
										{ %>
									<%--▼▼▼ カード情報入力フォーム非表示 ▼▼▼--%>
									<div class="form-element-group form-element-group-horizontal-grid">
										<p class="note" style="color: red; text-align: left">
											クレジットカード番号は入力できません。<br />
											<% if (this.NeedsRegisterProvisionalCreditCardCardKbn)
												{ %>
											登録すると「<%: new PaymentService().Get(this.LoginOperatorShopId, Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID).PaymentName %>」として登録されます。
										<% } %>
										</p>
									</div>
									<% } %>
									<%--▲▲▲ カード情報入力フォーム非表示 ▲▲▲--%>
								</div>
								<%--▼▼▼ カード情報入力フォーム表示 ▼▼▼--%>
								<%-- ▽分割支払い有効の場合は表示▽ --%>
								<div id="dvInstallments" class="form-element-group form-element-group-horizontal-grid" runat="server" visible="false">
									<div class="form-element-group-title">
										支払回数<span class="notice">*</span>
									</div>
									<div class="form-element-group-content" style="padding-top: 2px;">
										<asp:DropDownList ID="dllCreditInstallments" runat="server" />
										<br />
										<p class="note">※AMEX/DINERSは一括のみとなります。</p>
									</div>
								</div>
								<%-- △分割支払い有効の場合は表示△ --%>
								<% if (this.CanUseCreditCardNoForm)
									{ %>
								<div id="dvRegistCreditCard" class="form-element-group form-element-group-horizontal-grid" runat="server">
									<div class="form-element-group-title">
										登録する
									</div>
									<div class="form-element-group-content">
										<asp:CheckBox ID="cbRegistCreditCard" runat="server" Text="  登録する  " AutoPostBack="True"
											OnCheckedChanged="cbRegistCreditCard_CheckedChanged" />
									</div>
								</div>
								<div id="dvCreditCardName" class="form-element-group form-element-group-horizontal-grid" runat="server">
									<div class="form-element-group-title">
										クレジットカード登録名<span class="notice">*</span>
									</div>
									<div class="form-element-group-content">
										<asp:TextBox ID="tbUserCreditCardName" runat="server" MaxLength="30" /><br />
										※クレジットカードを保存する場合は、以下をご入力ください。
									</div>
								</div>
								<% } %>
								<%--▲▲▲ カード情報入力フォーム表示 ▲▲▲--%>
							</div>
							<div id="dvGmoCvsType" visible="false" class="form-element-group form-element-group-horizontal-grid" runat="server">
								<div class="form-element-group-title">
									<%: StringUtility.ToHankaku("支払いコンビニ選択") %>
								</div>
								<div class="form-element-group-content">
									<asp:DropDownList ID="ddlGmoCvsType" runat="server" Width="80%" />
								</div>
							</div>
							<div id="dvRakutenCvsType" visible="false" class="form-element-group form-element-group-horizontal-grid" runat="server">
								<div class="form-element-group-title">
									支払いコンビニ選択
								</div>
								<div class="form-element-group-content">
									<asp:DropDownList ID="ddlRakutenCvsType" runat="server" Width="80%" />
								</div>
							</div>
							<div id="dvPaymentNoticeMessage" runat="server" visible="false" class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title">
									<asp:Label runat="server" Text="注意喚起" ForeColor="red" />
								</div>
								<div class="form-element-group-content">
									<p>
										<asp:Label ID="lbPaymentUserManagementLevelMessage" runat="server" ForeColor="red" />
									</p>
									<p>
										<asp:Label ID="lbPaymentOrderOwnerKbnMessage" runat="server" ForeColor="red" />
									</p>
									<p>
										<asp:Label ID="lbPaymentLimitedMessage" runat="server" ForeColor="red" />
									</p>
								</div>
							</div>
						</div>
					</div>
				</div>
				<!-- △決済情報△ -->

			</div>
			<!-- △金額合計△ -->
			<div class="fixed-bottom-area">
				<div class="total-price">
					<span class="total-price-title">金額合計</span>
					<span class="price-value">
						<asp:Literal ID="lOrderPriceTotalBottom" runat="server" /></span>
				</div>
				<div id="buttonAreaBottom">
					<asp:Button ID="btnBack" class="fixed-bottom-area-btn btn btn-sub btn-size-l" runat="server" OnClick="btnBack_Click" Text="  戻る  " />
					<asp:Button ID="btnConfirm" class="fixed-bottom-area-btn btn btn-main btn-size-l" runat="server" Text="  注文を確定  " OnClientClick="return exec_submit()" OnClick="btnConfirm_Click" />
				</div>
				<span id="processingBottom" style="display: none;"><strong>ただいま処理中です。<br />
					画面が切り替わるまでそのままお待ちください。</strong></span>
			</div>
			<!-- △金額合計△ -->
		</div>
	</div>
	<asp:HiddenField runat="server" ID="hfAlertMessage" />
	<script type="text/javascript">
		<!--
	var memoData = {}; // Global storage for memo data
	var selectedOrderId = null;

	$(function () {
		displayMemoPopup();
	});

	var confirmMessage = '<%: WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_NOT_PRODUCT_ORDER_LIMIT) %>' + "\nよろしいですか？";
		var exec_submit_flg = 0;
		function exec_submit() {
			var alertMessage = $('#<%= hfAlertMessage.ClientID %>').val();
			if ((alertMessage != '') && (alertMessage != undefined)) {
				alertMessage = alertMessage.replace(/<br \/>/g, '\n');
				var isExec = confirm(alertMessage);
				if (isExec == false) return false;
			}
			if (exec_submit_flg == 1) return false;

			<% if (this.HasOrderHistorySimilarShipping)
		{ %>
			if (confirm(confirmMessage) == false) return false;
			<% } %>

			// ボタン消去
			document.getElementById('buttonAreaBottom').style.display = "none";

			// 処理中文言表示
			document.getElementById('processingBottom').style.display = "inline";

			exec_submit_flg = 1;
			return true;
		}

		function selectOrder(orderIndex, orderId, element) {

			selectedOrderId = orderId;
			var rows = document.querySelectorAll('.orderLine');
			rows.forEach(row => row.classList.remove('selected-row'));
			element.classList.add('selected-row');
			document.getElementById('<%= hfSelectedOrderIndex.ClientID %>').value = orderIndex;
			document.getElementById('<%= hfSelectedOrderId.ClientID %>').value = selectedOrderId;
<%--			document.getElementById('<%= hfSelectedDeliveryCompanyId.ClientID %>').value = document.getElementById('<%= ddlDeliveryCompany.ClientID %>').value;--%>

			localStorage.setItem('selectedOrderId', orderId);

			__doPostBack('<%= rOrderListForTable.ClientID %>', '');
		}

		function selectShopShipping(shopShippingIndex, shippingNo,shippingName, element) {
			var rows = document.querySelectorAll('.shippingLine');
			rows.forEach(row => row.classList.remove('selected-row'));
			element.classList.add('selected-row');
			document.getElementById('<%= hfSelectedShopShippingIndex.ClientID %>').value = shopShippingIndex;
			document.getElementById('<%= hfSelectedShopShippingNo.ClientID %>').value = shippingNo;

			localStorage.setItem('shopShippingIndex', shopShippingIndex);

			__doPostBack('<%= rShippingListForTable.ClientID %>', '');
		}

		document.addEventListener('DOMContentLoaded', function () {
			var selectedOrderId1 = localStorage.getItem('selectedOrderId');
			var shopShippingIndex = localStorage.getItem('shopShippingIndex');
			if (selectedOrderId1) {
				var rows = document.querySelectorAll('.orderLine');
				rows.forEach(row => {
					if (row.querySelector('.shipping-list-td').innerText.trim() === selectedOrderId1) {
						row.classList.add('selected-row');
					}
				});
			}
			if (shopShippingIndex) {
				var rows = document.querySelectorAll('.shippingLine');
				rows.forEach((row, index) => {
					console.log("Index: " + index);
					if (index == shopShippingIndex) {
						row.classList.add('selected-row');
					}
				});
			}
			selectedOrderId = document.getElementById('<%= hfSelectedOrderId.ClientID %>').value;
		});
	</script>
</asp:Content>
