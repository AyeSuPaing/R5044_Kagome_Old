<%--
=========================================================================================================
  Module      : 新規注文登録確認ページ(OrderRegistConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OrderRegistConfirm.aspx.cs" Inherits="Form_OrderRegist_OrderRegistConfirm" %>

<%@ Register TagPrefix="uc" TagName="FieldMemoSetting" Src="~/Form/Common/FieldMemoSetting/BodyFieldMemoSetting.ascx" %>
<%@ Import Namespace="w2.App.Common.Order" %>
<%@ Import Namespace="w2.App.Common.Product" %>
<%@ Import Namespace="w2.App.Common.Order.OrderCombine" %>
<%@ Import Namespace="w2.App.Common.Order.FixedPurchaseCombine" %>
<%@ Import Namespace="w2.Domain.Order" %>
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
	</style>
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
								<% if (this.IsOwnerAddrJp) { %>
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
								<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: StringUtility.ToHankaku("モバイルメールアドレス") %>
									</div>
									<div class="form-element-group-content">
										<asp:Literal ID="lOwnerMailAddr2" runat="server" />
									</div>
								</div>
								<% } %>
								<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.country.name@@", this.OwnerAddrCountryIsoCode) %>
									</div>
									<div class="form-element-group-content">
										<asp:Literal ID="lOwnerCountryName" runat="server" />
									</div>
								</div>
								<% } %>
								<% if (this.IsOwnerAddrJp) { %>
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
								<% if (Constants.DISPLAY_ADDR4_ENABLED || (this.IsOwnerAddrJp == false)) { %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: StringUtility.ToHankaku(ReplaceTag("@@User.addr4.name@@", this.OwnerAddrCountryIsoCode)) %>
									</div>
									<div class="form-element-group-content">
										<asp:Literal ID="lOwnerAddr4" runat="server" />
									</div>
								</div>
								<% } %>
								<% if (this.IsOwnerAddrJp == false) { %>
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
								<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
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
								<% if (Constants.GLOBAL_OPTION_ENABLE){ %>
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
									<% if (this.CanUpdateUser) { %>
									<asp:Literal runat="server">注文者情報の変更をユーザー情報に反映する</asp:Literal>
									<% } else { %>
									<asp:Literal ID="Literal1" runat="server">注文者情報の変更をユーザー情報に反映しない</asp:Literal>
									<% } %>
								</div>
							</div>
						</div>
					</div>
				</div>
				<!-- △注文者情報△ -->
				<!-- ▽配送先情報▽ -->
				<div id="dvShippingTo" class="block-order-regist-input-section ShippingTo" runat="server">
					<div class="block-order-regist-input-section-header">
						<h2 class="block-order-regist-input-section-title">
							<span class="block-order-regist-input-section-title-icon icon-shippingto"></span>
							<span class="block-order-regist-input-section-title-label">配送先情報</span>
						</h2>
					</div>
					<div id="dvOrderShippingSameAsOwner" class="block-order-regist-input-section-contents shipping-same-as-owner-contents" runat="server">
						<div class="form-element-group form-element-group-horizontal-grid">
							配送先を注文者と同じにする
						</div>
					</div>
					<div id="dvOrderShipping" class="block-order-regist-input-section-contents shipping-same-as-owner-contents" runat="server" visible="false">
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<%: ReplaceTag("@@User.name.name@@") %>
							</div>
							<div class="form-element-group-content">
								<asp:Literal ID="lShippingName" runat="server" />
							</div>
						</div>
						<% if (this.IsShippingAddrJp) { %>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<%: ReplaceTag("@@User.name_kana.name@@") %>
							</div>
							<div class="form-element-group-content">
								<asp:Literal ID="lShippingNameKana" runat="server" />
							</div>
						</div>
						<% } %>
						<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<%: ReplaceTag("@@User.country.name@@", this.ShippingAddrCountryIsoCode) %>
							</div>
							<div class="form-element-group-content">
								<asp:Literal ID="lShippingCountryName" runat="server" />
							</div>
						</div>
						<% } %>
						<% if (this.IsShippingAddrJp) { %>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<%: ReplaceTag("@@User.zip.name@@") %>
							</div>
							<div class="form-element-group-content">
								<asp:Literal ID="lShippingZip" runat="server" />
							</div>
						</div>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<%: ReplaceTag("@@User.addr1.name@@") %>
							</div>
							<div class="form-element-group-content">
								<asp:Literal ID="lShippingAddr1" runat="server" />
							</div>
						</div>
						<% } %>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<%: ReplaceTag("@@User.addr2.name@@", this.ShippingAddrCountryIsoCode) %>
							</div>
							<div class="form-element-group-content">
								<asp:Literal ID="lShippingAddr2" runat="server" />
							</div>
						</div>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<%: ReplaceTag("@@User.addr3.name@@", this.ShippingAddrCountryIsoCode) %>
							</div>
							<div class="form-element-group-content">
								<asp:Literal ID="lShippingAddr3" runat="server" />
							</div>
						</div>
						<% if (Constants.DISPLAY_ADDR4_ENABLED || (this.IsShippingAddrJp == false)) { %>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<%: StringUtility.ToHankaku(ReplaceTag("@@User.addr4.name@@", this.ShippingAddrCountryIsoCode)) %>
							</div>
							<div class="form-element-group-content">
								<asp:Literal ID="lShippingAddr4" runat="server" />
							</div>
						</div>
						<% } %>
						<% if (this.IsShippingAddrJp == false) { %>
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
						<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<%: ReplaceTag("@@User.company_name.name@@")%>
							</div>
							<div class="form-element-group-content">
								<asp:Literal ID="lShippingCompanyName" runat="server" />
							</div>
						</div>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<%: ReplaceTag("@@User.company_post_name.name@@")%>
							</div>
							<div class="form-element-group-content">
								<asp:Literal ID="lShippingCompanyPostName" runat="server" />
							</div>
						</div>
						<% } %>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<% if (this.IsShippingAddrJp) { %>
								<%: ReplaceTag("@@User.tel1.name@@") %>
								<% } else { %>
								<%: ReplaceTag("@@User.tel1.name@@", this.ShippingAddrCountryIsoCode) %>
								<% } %>
							</div>
							<div class="form-element-group-content">
								<asp:Literal ID="lShippingTel1" runat="server" />
							</div>
						</div>
					</div>
					<div id="dvOrderShippingConvenienceStore" class="block-order-regist-input-section-contents shipping-same-as-owner-contents" runat="server">
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
					<div id="dvOrderShippingRealStore" class="block-order-regist-input-section-contents shipping-same-as-owner-contents" runat="server">
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								配送先
							</div>
							<div class="form-element-group-content">
								店舗受取
							</div>
						</div>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								受取店舗
							</div>
							<div class="form-element-group-content">
								<asp:Literal ID="lbStoreName" runat="server" />
							</div>
						</div>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								店舗住所
							</div>
							<div class="form-element-group-content">
								<asp:Label ID="lbStoreAddress" runat="server" />
							</div>
						</div>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								営業時間
							</div>
							<div class="form-element-group-content">
								<asp:Literal ID="lbStoreOpeningHours" runat="server" />
							</div>
						</div>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								店舗電話番号
							</div>
							<div class="form-element-group-content">
								<asp:Literal ID="lbStoreTel" runat="server" />
							</div>
						</div>
					</div>
				</div>
				<!-- △配送先情報△ -->
			</div>
			<div class="block-order-regist-input-block2">
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
									<th class="thum" rowspan="3">商品<br />
										画像</th>
									<th class="product-id">商品ID</th>
									<th class="variation-id">バリエーションID</th>
									<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
									<th class="fixed-purchase">定期</th>
									<% } %>
									<% if (Constants.PRODUCT_SALE_OPTION_ENABLED) { %>
									<th class="product-sale-id">セールID</th>
									<% } %>
									<th class="product-price" rowspan="3" Visible="<%# this.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem == false %>" runat="server">
										単価
									</th>
									<th class="item-quantity" rowspan="3">数量</th>
									<th class="tax" rowspan="3" Visible="<%# this.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem == false %>" runat="server">
										税率
									</th>
									<th class="item-price" rowspan="3" Visible="<%# this.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem == false %>" runat="server">
										小計
									</th>
									<th class="delete" rowspan="3"></th>
								</tr>
								<tr>
									<th class="product-name" colspan="<%= this.ProductColSpanNumber %>">商品名</th>
								</tr>
								<tr>
									<th class="option" colspan="<%= this.ProductColSpanNumber %>">付帯情報</th>
								</tr>
								<tr>
									<td colspan="<%: (this.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem ? 8 : 11) %>" class="separate"></td>
								</tr>
								<!-- 商品情報詳細 -->
								<asp:Repeater ID="rItemList" ItemType="CartProduct" runat="server">
									<ItemTemplate>
										<tr>
											<td class="thum" rowspan="3">
											<%# ProductImage.GetHtmlImageTag(Item, ProductType.Product, SiteType.Pc, Constants.PRODUCTIMAGE_FOOTER_S) %>
											<td><%#: Item.ProductId %></td>
											<td><%#: ((string.IsNullOrEmpty(Item.VId) == false) ? Item.VId : "-") %></td>
											<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
											<td class="ta-center"><span class="<%#: (Item.IsFixedPurchase ? "icon-check" : "icon") %>"></span></td>
											<% } %>
											<% if (Constants.PRODUCT_SALE_OPTION_ENABLED) { %>
											<td><%#: ((string.IsNullOrEmpty(Item.ProductSaleId) == false) ? Item.ProductSaleId : "-") %></td>
											<% } %>
											<td class='<%# Item.IsSubscriptionBoxFixedAmount() == false ? "ta-right" : "ta-center" %>' rowspan="3" Visible="<%# this.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem == false %>" runat="server">
												<%#: Item.IsSubscriptionBoxFixedAmount() == false ? Item.Price.ToPriceString(true) : "-" %>
											</td>
											<td rowspan="3"><%#: Item.QuantitiyUnallocatedToSet %></td>
											<td class="ta-center" rowspan="3" Visible="<%# this.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem == false %>" runat="server">
												<%#: Item.IsSubscriptionBoxFixedAmount() == false ? string.Format("{0}%", TaxCalculationUtility.GetTaxRateForDIsplay(Item.TaxRate)) : "-" %>
											</td>
											<td class='<%# Item.IsSubscriptionBoxFixedAmount() == false ? "ta-right" : "ta-center" %>' rowspan="3" Visible="<%# this.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem == false %>" runat="server">
												<div Visible="<%# Item.IsSubscriptionBoxFixedAmount() == false %>" runat="server">
													<%#: ((Item.Price + Item.ProductOptionSettingList.SelectedOptionTotalPrice) * Item.QuantitiyUnallocatedToSet).ToPriceString(true) %>
												</div>
												<div Visible="<%# Item.IsSubscriptionBoxFixedAmount() %>" runat="server">
													<%#: string.Format("定額({0})", Item.SubscriptionBoxFixedAmount.ToPriceString(true)) %>
												</div>
											</td>
											<td class="ta-center" rowspan="3"></td>
										</tr>
										<tr>
											<td colspan="<%= this.ProductColSpanNumber %>" style="padding-bottom: 5px; word-break: break-all">
												<% if (Constants.ORDER_COMBINE_OPTION_ENABLED) { %>
												<%#: OrderCombineUtility.GetCartProductChangesByOrderCombine(Item) %><br />
												<% } %>
												<%#: Item.ProductJointName %>
											</td>
										</tr>
										<tr>
											<td colspan="<%= this.ProductColSpanNumber %>" class="group-memo-area">
												<%#: Item.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues() %>
											</td>
										</tr>
										<tr>
											<td colspan="<%: this.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem ? 8 : 11 %>" class="separate"></td>
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
													<% if (Constants.PRODUCT_SALE_OPTION_ENABLED) { %>
													<td><%#: ((string.IsNullOrEmpty(Item.ProductSaleId) == false) ? Item.ProductSaleId : "-") %></td>
													<% } %>
													<td class='<%# Item.IsSubscriptionBoxFixedAmount() == false ? "ta-right" : "ta-center" %>' rowspan="3" Visible="<%# this.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem == false %>" runat="server">
														<%#: Item.IsSubscriptionBoxFixedAmount() == false ? Item.Price.ToPriceString(true) : "-" %>
													</td>
													<td rowspan="3"><%#: Item.QuantityAllocatedToSet[((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).CartSetPromotionNo] %></td>
													<td rowspan="3" Visible="<%# this.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem == false %>" runat="server">
														<%#: Item.IsSubscriptionBoxFixedAmount() == false ? string.Format("{0}%", TaxCalculationUtility.GetTaxRateForDIsplay(Item.TaxRate)) : "-" %>
													</td>
													<td class='<%# Item.IsSubscriptionBoxFixedAmount() == false ? "ta-right" : "ta-center" %>' rowspan="3" Visible="<%# this.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem == false %>" runat="server">
														<div Visible="<%# Item.IsSubscriptionBoxFixedAmount() == false %>" runat="server">
															<%#: (Item.Price * Item.QuantityAllocatedToSet[((CartSetPromotion)((RepeaterItem)Container.Parent.Parent).DataItem).CartSetPromotionNo]).ToPriceString(true) %>
														</div>
														<div Visible="<%# Item.IsSubscriptionBoxFixedAmount() %>" runat="server">
															<%#: string.Format("定額({0})", Item.SubscriptionBoxFixedAmount.ToPriceString(true)) %>
														</div>
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
													<td colspan="<%: this.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem ? 8 : 11 %>" class="separate"></td>
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
				<!-- ▽頒布会情報▽ -->
				<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED) { %>
					<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
						<tr>
							<td class="edit_title_bg" align="center" colspan="4">頒布会</td>
						</tr>
						<tbody runat="server">
						<tr>
							<td class="edit_item_bg" align="left" colspan="4">
								<asp:Literal id="lSubscriptionName" runat="server"></asp:Literal>
							</td>
						</tr>
						</tbody>
					</table>
					<br />
				<% } %>
				<!-- △頒布会情報△ -->
				<div class="block-order-regist-input-section-row">
					<div class="block-order-regist-input-section discount <%: ((Constants.W2MP_COUPON_OPTION_ENABLED || (Constants.W2MP_POINT_OPTION_ENABLED && this.IsUser)) ? string.Empty : "no-discount")%>">
						<div class="discount-method-list">
							<% if (Constants.W2MP_COUPON_OPTION_ENABLED) { %>
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
							<% if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsUser) { %>
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
								<% if (this.CanUseAllPointFlg) { %>
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
					<div class="block-order-regist-input-section order-detail-info">
						<table class="order-detail-info-table">
							<tbody>
								<tr>
									<th>商品合計</th>
									<td><span class="price-value">
										<asp:Literal ID="lOrderPriceSubTotal" runat="server" Text="0" /></span></td>
								</tr>
								<% if (this.ProductIncludedTaxFlg == false) { %>
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
								<% if (Constants.MEMBER_RANK_OPTION_ENABLED) { %>
								<tr id="trMemberRankDiscount" runat="server">
									<th>会員ランク割引</th>
									<td><span class="price-value">
										<asp:Literal ID="lMemberRankDiscount" runat="server" /></span></td>
								</tr>
								<% } %>
								<%if (Constants.MEMBER_RANK_OPTION_ENABLED && Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
								<tr id="trFixedPurchaseMemberDiscount" runat="server">
									<th>定期会員割引</th>
									<td><span class="price-value">
										<asp:Literal ID="lFixedPurchaseMemberDiscount" runat="server" /></span></td>
								</tr>
								<% } %>
								<% if (Constants.W2MP_COUPON_OPTION_ENABLED) { %>
								<tr id="trCouponDiscount" runat="server">
									<th>クーポン割引額</th>
									<td><span class="price-value">
										<asp:Literal ID="lCouponUsePrice" runat="server" Text="0" /></span></td>
								</tr>
								<% } %>
								<% if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsUser) { %>
								<tr id="trPointDiscount" runat="server">
									<th>ポイント利用額</th>
									<td><span class="price-value">
										<asp:Literal ID="lPointUsePrice" runat="server" Text="0" /></span></td>
								</tr>
								<% } %>
								<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
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
								<%if (Constants.GLOBAL_OPTION_ENABLE) { %>
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
				</div>
				<!-- ▽メモ▽ -->
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
									<uc:FieldMemoSetting runat="server" Title="調整金額メモ" FieldMemoSettingList="<%# this.FieldMemoSettingList %>" TableName="<%# Constants.TABLE_ORDER %>" FieldName="<%# Constants.FIELD_ORDER_REGULATION_MEMO %>" />
								</span>
							</div>
							<div class="form-element-group-content group-memo-area">
								<pre><asp:Literal ID="lRegulationMemo" runat="server" />&nbsp;</pre>
							</div>
						</div>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title group-memo-area">
								<span class="memo-title-text">注文メモ
									<uc:FieldMemoSetting runat="server" Title="注文メモ" FieldMemoSettingList="<%# this.FieldMemoSettingList %>" TableName="<%# Constants.TABLE_ORDER %>" FieldName="<%# Constants.FIELD_ORDER_MEMO %>" />
								</span>
							</div>
							<div class="form-element-group-content group-memo-area">
								<asp:Repeater ID="rOrderMemos" ItemType="CartOrderMemo" runat="server">
									<ItemTemplate>
										<p class="memo-input-title"><%# Item.OrderMemoName %></p>
										<pre><asp:Literal ID="lMemo" runat="server" Text="<%# Item.InputText %>" />&nbsp;</pre>
									</ItemTemplate>
								</asp:Repeater>
							</div>
						</div>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<span class="memo-title-text">決済連携メモ
									<uc:FieldMemoSetting runat="server" Title="決済連携メモ" FieldMemoSettingList="<%# this.FieldMemoSettingList %>" TableName="<%# Constants.TABLE_ORDER %>" FieldName="<%# Constants.FIELD_ORDER_PAYMENT_MEMO %>" />
								</span>
							</div>
							<div class="form-element-group-content group-memo-area">
								<pre><asp:Literal ID="lPaymentMemo" runat="server" />&nbsp;</pre>
							</div>
						</div>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<span class="memo-title-text">管理メモ
									<uc:FieldMemoSetting runat="server" Title="管理メモ" FieldMemoSettingList="<%# this.FieldMemoSettingList %>" TableName="<%# Constants.TABLE_ORDER %>" FieldName="<%# Constants.FIELD_ORDER_MANAGEMENT_MEMO %>" />
								</span>
							</div>
							<div class="form-element-group-content group-memo-area">
								<pre><asp:Literal ID="lManagerMemo" runat="server" />&nbsp;</pre>
							</div>
						</div>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<span class="memo-title-text">定期購入管理メモ
									<uc:FieldMemoSetting runat="server" Title="管理メモ" FieldMemoSettingList="<%# this.FieldMemoSettingList %>" TableName="<%# Constants.TABLE_ORDER %>" FieldName="<%# Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_MANAGEMENT_MEMO %>" />
								</span>
							</div>
							<div class="form-element-group-content group-memo-area">
								<pre><asp:Literal ID="lFixedPurchaseManagementMemo" runat="server" />&nbsp;</pre>
							</div>
						</div>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<span class="memo-title-text">外部連携メモ
									<uc:FieldMemoSetting runat="server" Title="外部連携メモ" FieldMemoSettingList="<%# this.FieldMemoSettingList %>" TableName="<%# Constants.TABLE_ORDER %>" FieldName="<%# Constants.FIELD_ORDER_RELATION_MEMO %>" />
								</span>
							</div>
							<div class="form-element-group-content group-memo-area">
								<pre><asp:Literal ID="lRelationMemo" runat="server" />&nbsp;</pre>
							</div>
						</div>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<span class="memo-title-text">配送メモ
									<uc:FieldMemoSetting runat="server" Title="配送メモ" FieldMemoSettingList="<%# this.FieldMemoSettingList %>" TableName="<%# Constants.TABLE_ORDER %>" FieldName="<%# Constants.FIELD_ORDER_SHIPPING_MEMO %>" />
								</span>
							</div>
							<div class="form-element-group-content group-memo-area">
								<pre><asp:Literal ID="lShippingMemo" runat="server" />&nbsp;</pre>
							</div>
						</div>
					</div>
				</div>
				<!-- △メモ△ -->
			</div>
			<div class="block-order-regist-input-block3">
				<!-- ▽配送指定▽ -->
				<div class="block-order-regist-input-section ShippingInfo" id="dvShippingInfo" runat="server">
					<div class="block-order-regist-input-section-header">
						<h2 class="block-order-regist-input-section-title">
							<span class="block-order-regist-input-section-title-icon icon-shipping"></span>
							<span class="block-order-regist-input-section-title-label">配送指定</span>
						</h2>
					</div>
					<div class="block-order-regist-input-section-contents">
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								配送方法
							</div>
							<div class="form-element-group-content" style="margin-left: 10px">
								<asp:Literal ID="lShippingMethod" runat="server" />
							</div>
						</div>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								配送サービス
							</div>
							<div class="form-element-group-content" style="margin-left: 10px">
								<asp:Literal ID="lDeliveryCompany" runat="server" />
							</div>
						</div>
						<div id="dvShippingTimeSetFlgValid" class="form-element-group form-element-group-horizontal-grid" runat="server">
							<div class="form-element-group-title">
								配送希望時間帯
							</div>
							<div class="form-element-group-content" style="margin-left: 10px">
								<asp:Literal ID="lShippingTime" runat="server" />
							</div>
						</div>
						<div id="dvShippingDateSetFlgValid" class="form-element-group form-element-group-horizontal-grid" runat="server">
							<div class="form-element-group-title">
								配送希望日
							</div>
							<div class="form-element-group-content" style="margin-left: 10px">
								<asp:Literal ID="lShippingDate" runat="server" />
							</div>
						</div>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								配送伝票番号
							</div>
							<div class="form-element-group-content" style="margin-left: 10px">
								<asp:Literal ID="lShippingCheckNo" runat="server" />
							</div>
						</div>
					</div>
				</div>
				<!-- △配送指定△ -->
				<!-- ▽定期配送パターン▽ -->
				<div id="dvFixedPurchaseShippingInfo" class="block-order-regist-input-section shipping-pattern" runat="server">
					<div class="block-order-regist-input-section-header">
						<h2 class="block-order-regist-input-section-title">
							<span class="block-order-regist-input-section-title-icon icon-calendar"></span>
							<span class="block-order-regist-input-section-title-label">定期配送パターン</span>
						</h2>
					</div>
					<div class="block-order-regist-input-section-contents">
						<h3 class="form-element-group-list-title">配送日間隔指定</h3>
						<dl class="form-element-group-list">
							<dt>
								<asp:Literal ID="lFixedPurchasePatternTitle" runat="server" /></dt>
							<dd>
								<asp:Literal ID="lFixedPurchasePattern" runat="server" /></dd>
						</dl>
						<!-- ▽初回配送予定日▽ -->
						<h3 class="form-element-group-list-title">初回配送予定日</h3>
						<dl class="form-element-group-list">
							<dt>
								<asp:Literal ID="lFirstShippingDate" runat="server" /></dt>
						</dl>
						<!-- △次回配送予定日△ -->
						<!-- ▽次回配送予定日▽ -->
						<h3 class="form-element-group-list-title">次回配送予定日</h3>
						<dl class="form-element-group-list">
							<dt>
								<asp:Literal ID="lNextShippingDate" runat="server" /></dt>
						</dl>
						<!-- △次回配送予定日△ -->
						<!-- ▽次々回配送予定日▽ -->
						<h3 class="form-element-group-list-title">次々回配送予定日</h3>
						<dl class="form-element-group-list">
							<dt>
								<asp:Literal ID="lNextNextShippingDate" runat="server" /></dt>
						</dl>
						<!-- △次々回配送予定日△ -->
						<!-- ▽配送希望時間帯▽ -->
						<h3 class="form-element-group-list-title">配送希望時間帯</h3>
						<dl class="form-element-group-list">
							<dt>
								<asp:Literal ID="lShippingTime2" runat="server" /></dt>
						</dl>
						<!-- △配送希望時間帯△ -->
						<p class="note">※メール便の場合、配送予定日は数日ずれる可能性があります。</p>
					</div>
				</div>
				<!-- △定期配送パターン△ -->
				<!-- ▽領収書情報▽ -->
				<% if (Constants.RECEIPT_OPTION_ENABLED) { %>
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
				<!-- ▽電子発票▽ -->
				<% if (OrderCommon.DisplayTwInvoiceInfo() && this.IsShippingAddrTw) { %>
				<div class="block-order-regist-input-section" runat="server">
					<div class="block-order-regist-input-section-header">
						<h2 class="block-order-regist-input-section-title">
							<span class="block-order-regist-input-section-title-label">電子発票</span>
						</h2>
					</div>
					<div class="block-order-regist-input-section-contents">
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								発票種類
							</div>
							<div class="form-element-group-content">
								<asp:Literal ID="lUniformInvoice" runat="server" />
							</div>
						</div>
						<div id="dvUniformInvoicePersonal" runat="server" visible="false" class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								共通性載具
							</div>
							<div class="form-element-group-content">
								<asp:Literal ID="lCarryType" runat="server" />
							</div>
						</div>
						<div id="dvUniformInvoiceCompanyOption1" runat="server" visible="false" class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								統一編号
							</div>
							<div class="form-element-group-content">
								<asp:Literal ID="lUniformInvoiceOption1" runat="server" />
							</div>
						</div>
						<div id="dvUniformInvoiceCompanyOption2" runat="server" visible="false" class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								会社名
							</div>
							<div class="form-element-group-content">
								<asp:Literal ID="lUniformInvoiceOption2" runat="server" />
							</div>
						</div>
						<div id="dvUniformInvoiceDonate" runat="server" visible="false" class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								寄付先コード
							</div>
							<div class="form-element-group-content">
								<asp:Literal ID="lUniformInvoiceDonate" runat="server" />
							</div>
						</div>
					</div>
				</div>
				<% } %>
				<!-- △電子発票△ -->
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
				<!-- ▽注文拡張項目▽ -->
				<% if (Constants.ORDER_EXTEND_OPTION_ENABLED) { %>
				<div class="block-order-regist-input-section" runat="server">
					<div class="block-order-regist-input-section-header">
						<h2 class="block-order-regist-input-section-title">
							<span class="block-order-regist-input-section-title-label">注文拡張項目</span>
						</h2>
					</div>
					<div class="block-order-regist-input-section-contents">
						<asp:Repeater ID="rOrderExtendInput" ItemType="OrderExtendItemInput" runat="server">
							<ItemTemplate>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%#: Item.SettingModel.SettingName %>
									</div>
									<div class="form-element-group-content">
										<%#: Item.InputText %>
									</div>
								</div>
							</ItemTemplate>
						</asp:Repeater>
					</div>
				</div>
				<% } %>
				<!-- △注文拡張項目△-->
				<!-- ▽決済情報▽ -->
				<div class="block-order-regist-input-section payment-info">
					<div class="block-order-regist-input-section-header">
						<h2 class="block-order-regist-input-section-title">
							<span class="block-order-regist-input-section-title-icon icon-payment"></span>
							<span class="block-order-regist-input-section-title-label">決済情報</span>
						</h2>
					</div>
					<div class="block-order-regist-input-section-contents">
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								決済種別
							</div>
							<div class="form-element-group-content">
								<asp:Literal ID="lOrderPaymentKbn" runat="server" />
							</div>
						</div>
						<div id="dvGmoCvsType" runat="server">
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title">
									<%: StringUtility.ToHankaku("支払いコンビニ選択") %>
								</div>
								<div class="form-element-group-content">
									<asp:Literal ID="lGmoCvsType" runat="server" />
								</div>
							</div>
						</div>
						<div id="dvRakutenType" runat="server">
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title">
									支払いコンビニ選択
								</div>
								<div class="form-element-group-content">
									<asp:Literal ID="lRakutenCvsType" runat="server" />
								</div>
							</div>
						</div>
						<div id="dvOrderKbnCredit" runat="server">
							<div class="form-element-group form-element-group-horizontal-grid">
								<%if (OrderCommon.CreditCompanySelectable) { %>
								<div class="form-element-group-title">
									カード会社
								</div>
								<div class="form-element-group-content">
									<asp:Literal ID="lCreditCardCompany" runat="server" />
								</div>
								<% } %>
							</div>
							<div class="form-element-group form-element-group-horizontal-grid" runat="server">
								<div class="form-element-group-title">
									<%if (this.CreditTokenizedPanUse && this.UseNewCreditCard) { %>
										永久トークン
									<% } else { %>
										カード番号
									<% } %>
								</div>
								<div class="form-element-group-content">
									<asp:Literal ID="lCreditCardNo" runat="server" />
								</div>
							</div>
							<div class="form-element-group form-element-group-horizontal-grid" runat="server">
								<div class="form-element-group-title">
									有効期限
								</div>
								<div class="form-element-group-content">
									<asp:Literal ID="lValidity" runat="server" Text="(月/年)" />
								</div>
							</div>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title">
									カード名義人
								</div>
								<div class="form-element-group-content">
									<asp:Literal ID="lName" runat="server" />
								</div>
							</div>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title" id="dvSecurityCode" runat="server">
									セキュリティコード
								</div>
								<div class="form-element-group-content">
									<asp:Literal ID="lSecurityCode" runat="server" />
								</div>
							</div>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div id="dvInstallments" class="form-element-group-title" runat="server">
									支払回数
								</div>
								<div class="form-element-group-content">
									<asp:Literal ID="lInstallments" runat="server" />
								</div>
							</div>
							<div id="dvRegistCreditCard" class="form-element-group form-element-group-horizontal-grid" runat="server">
								<div class="form-element-group-title">
									登録する
								</div>
								<div class="form-element-group-content">
									<asp:Literal ID="lRegistCreditCard" runat="server" />
								</div>
							</div>
							<div id="dvUserCreditCardName" class="form-element-group form-element-group-horizontal-grid" runat="server">
								<div class="form-element-group-title">
									クレジットカード登録名
								</div>
								<div class="form-element-group-content">
									<asp:Literal ID="lUserCreditCardName" runat="server" />
								</div>
							</div>
						</div>
					</div>
				</div>
				<!-- △決済情報△ -->
				<!-- ▽注文同梱選択▽ -->
				<% if (Constants.ORDER_COMBINE_OPTION_ENABLED) { %>
				<div id="dvCombineArea" class="block-order-regist-input-section include-info" runat="server">
					<div class="block-order-regist-input-section-header">
						<h2 class="block-order-regist-input-section-title">
							<span class="block-order-regist-input-section-title-icon icon-box"></span>
							<span class="block-order-regist-input-section-title-label">注文同梱選択</span>
						</h2>
					</div>
					<div class="block-order-regist-input-section-contents">
						<div id="dvOrderCombine" visible="false" class="order-history-section" runat="server">
							<h3 class="order-title">注文同梱対象既存注文一覧</h3>
							<table class="user-info-table order">
								<tbody>
									<tr>
										<th colspan="2">
											<div class="order-data">
												<span class="order-data-inner">
													<span class="order-data-row">
														<span class="order-data-date">注⽂⽇時</span>
													</span>
													<span class="order-data-row">
														<span class="order-data-id">注⽂ID</span>
													</span>
												</span>
												<span class="order-data-status">
													<span class="order-data-row">
														<span class="order-data-status-order-text">注⽂S</span>/<span class="order-data-status-payment-text">⼊⾦S</span>
													</span>
													<span class="order-data-row">
														<span class="order-data-status">注文区分</span>/<span class="order-data-status">決済種別</span>
													</span>
												</span>
											</div>
										</th>
									</tr>
									<tr>
										<th class="item-name">注⽂商品</th>
										<th class="total-price">合計⾦額</th>
									</tr>
									<tr>
										<td colspan="2" class="separate"></td>
									</tr>
									<asp:Repeater ID="rOrderCombine" runat="server" ItemType="CartObject">
										<ItemTemplate>
											<tr>
												<td colspan="2">
													<div class="order-data">
														<span class="order-data-inner">
															<span class="order-data-row">
																<span class="order-data-date">
																	<%#: DateTimeUtility.ToStringForManager(this.OrderCreatedDateForCombine, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></span>
															</span>
															<span class="order-data-row">
																<a href="javascript:open_window('<%#: CreateOrderDetailUrl(Item.OrderCombineParentOrderId, true, false) %>','ordercontact','width=828,height=600,top=110,left=380,status=NO,scrollbars=yes');" class="order-data-id">
																	<%#: Item.OrderCombineParentOrderId %>
																</a>
															</span>
														</span>
														<span class="order-data-status">
															<span class="order-data-row">
																<span class="order-data-status-order-icon <%#: GetCssClassForStatus(this.OrderPaymentStatusForCombine, Constants.FIELD_ORDER_ORDER_STATUS) %>">
																	<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_STATUS, this.OrderStatusForCombine) %>
																</span>
																<span class="order-data-status-payment-icon <%#: GetCssClassForStatus(this.OrderPaymentStatusForCombine, Constants.FIELD_ORDER_ORDER_STATUS) %>">
																	<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, this.OrderPaymentStatusForCombine) %>
																</span>
															</span>
															<span class="order-data-row">
																<span class="order-data-status-payment-icon"><%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_KBN, Item.OrderKbn) %></span>
																<span class="order-data-status-payment-icon"><%#: ValueText.GetValueText(Constants.TABLE_PAYMENT, CONST_KEY_PAYMENT_TYPE, Item.Payment.PaymentId) %></span>
															</span>
														</span>
													</div>
												</td>
											</tr>
											<tr>
												<td class="item-name">
													<ul>
														<li>
															<asp:Repeater ID="rCombinableOrderItem" runat="server" DataSource="<%# this.OrderItemForCombine %>" ItemType="OrderItemModel">
																<ItemTemplate>
																	<a href="javascript:open_window('<%#: CreateProductDetailUrl(Item.ProductId, true) %>','productcontact','width=828,height=600,top=110,left=380,status=NO,scrollbars=yes');" class="order-data-id">
																		<%#: Item.ProductId %>
																	</a>
																	&nbsp;
																		<%#: Item.ProductName %><br />
																</ItemTemplate>
															</asp:Repeater>
														</li>
													</ul>
												</td>
												<td class="total-price"><span class="price-value"><%#: this.OrderPriceForCombine.ToPriceString(true) %></span></td>
											</tr>
											<tr>
												<td colspan="2">
													<dl>
														<dt>配送先</dt>
														<dd>〒<%#: Item.Shippings[Container.ItemIndex].Zip %>&nbsp;&nbsp;
																<%#: Item.Shippings[Container.ItemIndex].Addr1 %>
															<%#: Item.Shippings[Container.ItemIndex].Addr2 %>
															<%#: Item.Shippings[Container.ItemIndex].Addr3 %>
															<%#: Item.Shippings[Container.ItemIndex].Addr4 %>
															<br>
															<%#: Item.Shippings[Container.ItemIndex].Name %>
														</dd>
													</dl>
													<dl visible="<%# (Item.Shippings[Container.ItemIndex].ShippingDate != null) %>" runat="server">
														<dt>配送日時</dt>
														<dd>希望日：<%#: DateTimeUtility.ToStringForManager(Item.Shippings[Container.ItemIndex].ShippingDate, DateTimeUtility.FormatType.ShortDate2Letter) %></dd>
														<dd>希望時間帯：<%#: GetShippingTimeMessage(Item.Shippings[Container.ItemIndex].DeliveryCompanyId, Item.Shippings[Container.ItemIndex].ShippingTime) %></dd>
													</dl>
													<dl visible="<%# (string.IsNullOrEmpty(GetFixedPurchaseId(Item)) == false) %>" runat="server">
														<dt>定期</dt>
														<dd>定期ID：
																<a href="javascript:open_window('<%#: FixedPurchasePage.CreateFixedPurchaseDetailUrl(GetFixedPurchaseId(Item), true) %>','fixedpurchase','width=828,height=600,top=110,left=380,status=NO,scrollbars=yes');" class="order-data-id">
																	<%#: GetFixedPurchaseId(Item) %>
																</a>
														</dd>
														<dd>配送周期：<%#: FixedPurchaseCombineUtility.GetFixedPachasePatternSettingMessage(GetFixedPurchaseId(Item)) %></dd>
													</dl>
													<dl visible="<%# (string.IsNullOrEmpty(this.OrderCouponForCombine) == false) %>" runat="server">
														<dt>クーポン</dt>
														<dd><%#: this.OrderCouponForCombine %></dd>
													</dl>
												</td>
											</tr>
											<tr>
												<td colspan="2" class="separate"></td>
											</tr>
										</ItemTemplate>
									</asp:Repeater>
								</tbody>
							</table>
						</div>
					</div>
				</div>
				<% } %>
				<!-- △注文同梱選択△ -->
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
					<asp:Button ID="btnRegist" class="fixed-bottom-area-btn btn btn-main btn-size-l" runat="server" Text="  注文を確定  " OnClientClick="return exec_submit()" OnClick="btnRegist_Click" />
				</div>
				<span id="processingBottom" style="display: none;"><strong>ただいま処理中です。<br />
					画面が切り替わるまでそのままお待ちください。</strong></span>
			</div>
			<!-- △金額合計△ -->
		</div>
	</div>
	<script type="text/javascript">
		<!--
		$(function () {
			displayMemoPopup();
		});

		var confirmMessage = '<%: WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_NOT_PRODUCT_ORDER_LIMIT) %>' + "\nよろしいですか？";
		var exec_submit_flg = 0;
		function exec_submit() {
			if (exec_submit_flg == 1) return false;

			<% if (this.HasOrderHistorySimilarShipping) { %>
			if (confirm(confirmMessage) == false) return false;
			<% } %>

			// ボタン消去
			document.getElementById('buttonAreaBottom').style.display = "none";

			// 処理中文言表示
			document.getElementById('processingBottom').style.display = "inline";

			exec_submit_flg = 1;
			return true;
		}
		//-->
	</script>
</asp:Content>
