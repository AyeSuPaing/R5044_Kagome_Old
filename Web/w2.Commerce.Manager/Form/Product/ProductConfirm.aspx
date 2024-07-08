<%--
=========================================================================================================
  Module      : 商品情報確認ページ(ProductConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Manager" %>
<%@ Import Namespace="w2.Domain.MenuAuthority.Helper" %>
<%@ Import Namespace="w2.Common.Web" %>
<%@ Import Namespace="w2.App.Common.Product" %>
<%@ Import Namespace="w2.App.Common.Option" %>
<%@ Import Namespace="w2.App.Common.Input" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ProductConfirm.aspx.cs" Inherits="Form_Product_ProductConfirm" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderHead" runat="server">
	<meta http-equiv="Pragma" content="no-cache" />
	<meta http-equiv="cache-control" content="no-cache" />
	<meta http-equiv="expires" content="0" />

	<style type="text/css">
		.ajax-loading
		{
			border: 1px solid #bababa;
			border-radius: 5px;
			padding: 100px 0;
			display: none;
			background: #f2f2f2;
		}

		.error-message-datepicker-container,
		.product-error-message-container,
		.product-price-error-message-container,
		.product-tag-error-message-container,
		.product-extend-error-message-container,
		.product-variation-error-message-container,
		.product-variation-price-error-message-container {
			text-align: left;
			color: red;
			padding: 5px;
			word-wrap: break-word;
			display: block; 
		}

		.error-message-datepicker-container {
			position: absolute;
		}

		#modal-product-register-display-period .select-period,
		#modal-product-register-sale-period .select-period {
			margin: 40px 0px 60px 0px;
		}

		#modal-product-register-display-period .select-period .select-period-start,
		#modal-product-register-display-period .select-period .select-period-start,
		#modal-product-register-sale-period .select-period .select-period-start,
		#modal-product-register-sale-period .select-period .select-period-start {
			position: relative;
		}

		.submit-loading {
			padding: 50px 0;
		}

		.break-text-hover {
			word-break: break-all;
			white-space: normal !important;
		}

		.member-rank-price-item {
			width: 8em;
			max-width: 8em;
			display: inline-block;
			vertical-align: top;
			padding-right: 0px !important;
			padding-bottom: 20px;
			margin-right: 10px;
		}

		.form-element-group-content textarea,
		.product-register-option-toggle-content-advance-input textarea {
			min-width: 100%;
			min-height: 50px;
		}

		.product-register-display-kubun .form-element-group-content-item,
		.product-register-product-variation-kubun .product-register-product-variation-kubun-item {
			position: relative;
		}

		.product-register-display-kubun .form-element-group-content-item input[type="radio"] {
			left: 5px;
			top: 50%;
			margin-top: -8px;
		}

		.product-register-product-variation-kubun .product-register-product-variation-kubun-item input[type="radio"] {
			left: 8px;
			top: 50%;
			margin-top: -10px;
		}

		.input-timepicker {
			width: 4.5rem !important;
		}

		.js-select-period-modal .select-period {
			justify-content: center;
		}

		.modal.modal-size-m .modal-content-wrapper {
			width: 700px !important;
		}

		@media screen and (-ms-high-contrast: active), (-ms-high-contrast: none) {
			.product-register-display-kubun .form-element-group-content-item input[type="radio"] {
				left: 10px;
				margin-top: -14px;
			}

			.product-register-product-variation-kubun .product-register-product-variation-kubun-item input[type="radio"] {
				left: 14px;
				margin-top: -16px;
			}
		}

		.form-element-group-content select {
			max-width: 24em;
		}

		.product-register-option-toggle-content-basic .form-element-group-list .form-element-group-list-item .form-element-group-list-item-title {
			margin-right: 0.5em;
			text-align: left;
		}

		.btn-size-m,
		input[type="submit"].btn-size-m,
		input[type="button"].btn-size-m,
		a.btn-size-m {
			font-size: 16px;
			padding: 0 15px;
			height: 32px;
			line-height: 32px;
			white-space: nowrap;
			margin: 0px !important;
		}

		.form-element-group-content-confirm {
			padding-top: 3px;
		}

		.label-card {
			border: 1px solid #999;
			border-radius: 5px;
			padding: 10px 15px 10px 10px;
			margin: 0 10px 10px 0;
			position: relative;
			display: flex;
			align-items: center;
		}

		.radio-card {
			position: relative;
			display: flex;
			align-items: center;
			border: 1px solid #999;
			border-radius: 5px;
			padding: 10px 15px 10px 10px;
			margin: 0 10px 10px 0;
			cursor: pointer;
		}

		.add_cart_url {
			padding: 0 0 0 10px;
		}

		.product-register-option-toggle-content-basic .form-element-group-list-item-content .form-element-group-title {
			min-width: 8.5em;
		}

		.table-discount tr td {
			background-color: #f2f2f2;
		}

		.empty-value {
			color: #ccc;
		}
		.product-color-img {
			padding: 1px;
			background: #e1e1e1;
			display: inline-block;
		}
		.custom-category-selector-brand,
		.custom-category-selector-category {
			width: calc(60% - 5px);
		}
		.custom-category-selector-input-selected-item {
			width: calc(100% - 5px);
		}
	</style>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
	<div class="page-product-register">
		<div id="dvDetail" runat="server" class="page-title-wrapper" Visible="False">
			<h1 class="page-title">商品情報詳細</h1>
		</div>
		<div id="dvConfirm" runat="server" class="page-title-wrapper" Visible="False">
			<h1 class="page-title">商品情報入力確認</h1>
		</div>
		<% if (this.ActionStatus == Constants.ACTION_STATUS_UPDATE && IsChangedVariationId()) {%>
			<p style="margin-top: -10px; color: red">販売中に商品のバリエーション管理方法の変更や商品バリエーションの削除を行った場合、<br/>受注情報からのメール送信等で商品に関する情報を正しく表示できない可能性があります。</p>
		<% } %>
		<div id="divComp" runat="server" class="page-title-wrapper block-section-header" style="border-radius: 9px 9px 9px 9px; border: 1px solid #bababa;" Visible="False">
			<asp:Label ID="lMessage" runat="server"></asp:Label>
		</div>
		<div id="dvErrorMessage" runat="server" class="page-title-wrapper block-section-header" style="border-radius: 9px 9px 9px 9px; border: 1px solid #bababa;" Visible="False">
			<asp:Label ID="lErrorMessage" ForeColor="Red" runat="server"></asp:Label>
		</div>
		<div class="main-contents-fixed-header product-register-header">
			<div class="main-contents-fixed-header-inner">
				<div class="main-contents-fixed-header-col-1"></div>
				<div class="main-contents-fixed-header-col-2">
					<% if ((Constants.REPEATPLUSONE_OPTION_ENABLED == false) && (string.IsNullOrEmpty(Constants.URL_FRONT_PC) == false)) { %>
					<div class="product-register-header-preview-btn">
						<a href="javascript:void(0)" onclick="open_window('<%= CreatePreviewProductUrl("PC") %>', '_blank','')" class="btn btn-txt btn-size-m">
							<%: (w2.App.Common.Design.DesignCommon.UseResponsive) ? "プレビュー（RESPONSIVE）" : "プレビュー（PC）" %><span class="btn-icon-right icon-arrow-out"></span>
						</a>
						<a href="javascript:void(0)" onclick="open_window('<%= CreatePreviewProductUrl("SmartPhone") %>', '_blank','width=450,height=800,scrollbars=yes')" class="btn btn-txt btn-size-m" style='<%= (w2.App.Common.Design.DesignCommon.UseResponsive) ? "display: none;" : string.Empty %>'>
							プレビュー（SP）<span class="btn-icon-right icon-arrow-out"></span>
						</a>
					</div>
					<% } %>
					<div class="main-contents-fixed-header-btn">
						<asp:Button id="btnBack" runat="server" Text="  戻る  " OnClick="btnBack_Click" OnClientClick="target =''" class="btn btn-sub btn-size-l" />
						<asp:Button ID="btnBackList" runat="server" Text="  一覧へ戻る  " Visible="False" OnClick="btnBackList_Click" OnClientClick="target =''" class="btn btn-sub btn-size-l" />
						<asp:Button id="btnInsert" runat="server" Text="  登録する  " Visible="False" OnClick="btnInsertUpdate_Click" OnClientClick="disableButtonAndSubmit();target =''" class="btn btn-main btn-size-l" />
						<asp:Button id="btnUpdate" runat="server" Text="  更新する  " Visible="False" OnClick="btnInsertUpdate_Click" OnClientClick="disableButtonAndSubmit();target =''" class="btn btn-main btn-size-l" />
						<asp:Button id="btnEdit" runat="server" Text="  編集する  " Visible="False" OnClick="btnEdit_Click" OnClientClick="target ='';" class="btn btn-main btn-size-l" />
						<asp:Button id="btnNewRegist" runat="server" Text="  続けて登録する  " Visible="False" OnClick="btnNewRegist_Click" OnClientClick="target ='';" class="btn btn-main btn-size-l" />
						<% if (Constants.FLAPS_OPTION_ENABLE) { %>
						<asp:Button id="btnGetLatestInfoFromErpTop" runat="server" Text="  ERPから最新情報取得  " OnClick="btnGetLatestInfoFromErp_OnClick" class="btn btn-main btn-size-l" />
						<% } %>
					</div>
					<div id="dvMenu" class="dropdown" runat="server" visible="false">
						<a href="javascript:void(0)" class="btn-dot-menu dropdown-toggle"><span class="icon-dots"></span></a>
						<div class="dropdown-menu">
							<asp:LinkButton id="btnCopyInsert" runat="server" Text="  コピー新規登録する  " Visible="False" OnClick="btnCopyInsert_Click" OnClientClick="target =''" class="dropdown-menu-item" />
							<asp:LinkButton id="btnDelete" runat="server" Text="  削除する  " Visible="False" OnClick="btnDelete_Click" OnClientClick="target =''; return confirm('情報を削除してもよろしいですか？');" class="dropdown-menu-item" />
						</div>
					</div>
				</div>
			</div>
		</div>
		<div class="block-section-row">
			<div class="block-section block-section-product-register-basicinfo">
				<div class="block-section-header">
					<div class="block-section-header-icon"><span class="icon-box"></span></div>
					<h2 class="block-section-header-txt">基本情報</h2>
				</div>
				<div class="block-section-body">
					<div class="block-section-body-inner-row">
						<div class="block-section-body-inner-col">
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>商品ID</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_PRODUCT_ID)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_PRODUCT_ID) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.ProductId) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.ProductId) %>
									</label>
								</div>
							</div>
							<% if (this.DisplayAddCartUrl) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>カート投入URL</label>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm" style="display: flex; flex-direction: column;">
									<% if (this.dvNoVariation.Visible) { %>
										<% if (this.DisplayNormalAddCartUrl) { %>
											<div class="add_cart_url">
												<a class="btn-clipboard" href="#" data-clipboard-text="<%# CreateAddCartUrl(AddCartType.Normal) %>">【通常購入用】URLをコピー</a>
											</div>
										<% } %>
										<% if (this.DisplayFixedPurchaseAddCartUrl) {%>
											<div class="add_cart_url">
												<a class="btn-clipboard" href="#" data-clipboard-text="<%# CreateAddCartUrl(AddCartType.FixedPurchase) %>">【定期購入用】URLをコピー</a>
											</div>
										<% } %>
										<% if (this.DisplayGiftAddCartUrl) { %>
											<div class="add_cart_url">
												<a class="btn-clipboard" href="#" data-clipboard-text="<%# CreateAddCartUrl(AddCartType.Gift) %>">【ギフト購入用】URLをコピー</a>
											</div>
										<% } %>
									<% } else { %>
										<div class="add_cart_url">
											<a href="#anchorVariation">▼商品バリエーション</a>
										</div>
									<% } %>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_NAME)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>商品名</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_NAME)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_NAME) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.Name) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.Name) %>
									</label>
								</div>
							</div>
							<% } %>
							<% if (Constants.GLOBAL_OPTION_ENABLE && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_NAME)) { %>
							<asp:Repeater ID="rTranslationProductName" runat="server" 
								DataSource="<%# this.ProductTranslationData.Where(d => d.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_NAME) %>" 
								ItemType="w2.Domain.NameTranslationSetting.NameTranslationSettingModel">
								<ItemTemplate>
									<div class="form-element-group form-element-group-horizontal-grid">
										<div class="form-element-group-title break-text-hover">
											<label>　言語コード：<%#: Item.LanguageCode %> 言語ロケールID：<%#: Item.LanguageLocaleId %></label>
										</div>
										<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
											<label><%#: Item.AfterTranslationalName %></label>
										</div>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<% } %>
							<% if (this.IsOperationalCountryJp && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_NAME_KANA)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>商品名(カナ)</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_NAME_KANA)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_NAME_KANA) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.NameKana) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.NameKana) %>
									</label>
								</div>
							</div>
							<% } %>
							<% if (Constants.PRODUCTBUNDLE_OPTION_ENABLED && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_PRODUCT_TYPE)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>商品区分</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_PRODUCT_TYPE)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_PRODUCT_TYPE) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<label><%: GetProductValueToDisplay(this.ProductInput.ProductType, Constants.FIELD_PRODUCT_PRODUCT_TYPE) %></label>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label id="tbMaxSellQuantity">1注文購入限度数</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.MaxSellQuantity) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.MaxSellQuantity) %>
									</label>
								</div>
							</div>
							<% } %>
							<% if (Constants.GIFTORDER_OPTION_ENABLED && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_GIFT_FLG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid ">
								<div class="form-element-group-title break-text-hover">
									<label>ギフト購入フラグ</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_GIFT_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_GIFT_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm">
									<label><%: GetProductValueToDisplay(this.ProductInput.GiftFlg, Constants.FIELD_PRODUCT_GIFT_FLG) %></label>
								</div>
							</div>
							<% } %>
							<% if ((Constants.RECOMMEND_ENGINE_KBN == Constants.RecommendEngine.Silveregg) && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_USE_RECOMMEND_FLG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid ">
								<div class="form-element-group-title break-text-hover">
									<label>外部レコメンド利用</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_USE_RECOMMEND_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_USE_RECOMMEND_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm">
									<label><%: GetProductValueToDisplay(this.ProductInput.UseRecommendFlg, Constants.FIELD_PRODUCT_USE_RECOMMEND_FLG) %></label>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_NOTE)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>備考</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_NOTE)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_NOTE) %></p>
									<% } %>
								</div>
								<div class='form-element-group-content form-element-group-content-confirm break-text-hover <%# string.IsNullOrEmpty(this.ProductInput.Note) ? "empty-value" : string.Empty %>'>
									<%= GetEncodedHtmlDisplayMessage(GetProductValueToDisplay(this.ProductInput.Note)) %>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_SUPPLIER_ID)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>サプライヤID</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_SUPPLIER_ID)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_SUPPLIER_ID) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.SupplierId) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.SupplierId) %>
									</label>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_SEARCH_KEYWORD)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>検索キーワード</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_SEARCH_KEYWORD)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_SEARCH_KEYWORD) %></p>
									<% } %>
								</div>
								<div class='form-element-group-content form-element-group-content-confirm break-text-hover <%# string.IsNullOrEmpty(this.ProductInput.SearchKeyword) ? "empty-value" : string.Empty %>'>
									<%= GetEncodedHtmlDisplayMessage(GetProductValueToDisplay(this.ProductInput.SearchKeyword)) %>
								</div>
							</div>
							<% } %>
							<% if ((Constants.PRODUCTBUNDLE_OPTION_ENABLED) && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>同梱商品明細表示フラグ</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<label><%: GetProductValueToDisplay(this.ProductInput.BundleItemDisplayType, Constants.FIELD_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE) %></label>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_VALID_FLG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>有効フラグ</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_VALID_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_VALID_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm">
									<%: GetProductValueToDisplay(this.ProductInput.ValidFlg, Constants.FIELD_PRODUCT_VALID_FLG) %>
								</div>
							</div>
							<% } %>
							<div id="dvDateCreated" class="form-element-group form-element-group-horizontal-grid" runat="server" Visible="False">
								<div class="form-element-group-title break-text-hover">
									<label>作成日</label>
								</div>
								<div class="form-element-group-content break-text-hover" style="margin-top: 5px;">
									<%: DateTimeUtility.ToStringForManager(this.ProductInput.DateCreated, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %>
								</div>
							</div>
							<div id="dvDateChanged" class="form-element-group form-element-group-horizontal-grid" runat="server" Visible="False">
								<div class="form-element-group-title break-text-hover">
									<label>更新日</label>
								</div>
								<div class="form-element-group-content break-text-hover" style="margin-top: 5px;">
									<%: DateTimeUtility.ToStringForManager(this.ProductInput.DateChanged, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %>
								</div>
							</div>
							<div id="dvLastChanged" class="form-element-group form-element-group-horizontal-grid" runat="server" Visible="False">
								<div class="form-element-group-title break-text-hover">
									<label>最終更新者</label>
								</div>
								<div class='form-element-group-content break-text-hover <%# string.IsNullOrEmpty(this.ProductInput.LastChanged) ? "empty-value" : string.Empty %>' style="margin-top: 5px;">
									<%: GetProductValueToDisplay(this.ProductInput.LastChanged) %>
								</div>
							</div>
							<asp:Repeater ID="rExtend" runat="server" DataSource="<%# this.ProductExtends %>" Visible="<%# (Constants.MALLCOOPERATION_OPTION_ENABLED && (this.ProductExtends.Count != 0)) %>">
								<HeaderTemplate>
									<div id="divProductExtend">
										<div class="form-element-group-heading">
											<h3 class="form-element-group-heading-label">拡張項目</h3>
										</div>
								</HeaderTemplate>
								<ItemTemplate>
									<div class="form-element-group form-element-group-horizontal-grid">
										<div class="form-element-group-title break-text-hover">
											<label><%#: ((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NAME] %></label>
										</div>
										<div class="form-element-group-content form-element-group-content-confirm break-text-hover" style="padding-top: 1px;">
											<label class='<%# string.IsNullOrEmpty(StringUtility.ToEmpty(((Hashtable)Container.DataItem)["extend"])) ? "empty-value" : string.Empty %>'>
												<%# GetEncodedHtmlDisplayMessage(GetProductValueToDisplay(StringUtility.ToEmpty(((Hashtable)Container.DataItem)["extend"]))) %>
											</label>
										</div>
									</div>
								</ItemTemplate>
								<FooterTemplate>
									</div>
								</FooterTemplate>
							</asp:Repeater>
						</div>
						<div class="block-section-body-inner-col">
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CATCHCOPY)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>キャッチコピー</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_CATCHCOPY)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_CATCHCOPY) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.Catchcopy) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.Catchcopy) %>
									</label>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_OUTLINE)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>商品概要</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_OUTLINE)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_OUTLINE) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<div class="input-textarea">
										<div class="input-textarea-type-selector">
											<label><%: ValueText.GetValueText(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_OUTLINE_KBN, this.ProductInput.OutlineKbn) %></label>
										</div>
										<hr width="100%" size="2px" align="center" style="margin: 5px 0px 5px 0px;" />
										<div class='input-textarea-content <%# string.IsNullOrEmpty(this.ProductInput.Outline) ? "empty-value" : string.Empty %>'>
											<% if((this.ProductInput.OutlineKbn != Constants.FLG_PRODUCT_DESC_DETAIL_TEXT)
												&& (string.IsNullOrEmpty(this.ProductInput.Outline) == false)) { %>
											<iframe class="HtmlPreview" src="<%: new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_HTML_PREVIEW_FORM).AddParam(Constants.HTML_PREVIEW_NO, "1").CreateUrl() %>" style="width: 100%; height: 100%;" frameborder="0" scrolling="yes">
											</iframe>
											<% } else { %>
											<%# GetProductValueToDisplay(this.ProductInput.Outline, Constants.FIELD_PRODUCT_OUTLINE) %>
											<% } %>
										</div>
									</div>
								</div>
							</div>
							<% } %>
							<% if (Constants.GLOBAL_OPTION_ENABLE && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_OUTLINE)) { %>
							<asp:Repeater id="rTranslationOutline" runat="server" 
								DataSource="<%# this.ProductTranslationData.Where(d => (d.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_OUTLINE)) %>" 
								ItemType="w2.Domain.NameTranslationSetting.NameTranslationSettingModel">
								<ItemTemplate>
									<div class="form-element-group form-element-group-horizontal-grid">
										<div class="form-element-group-title break-text-hover">
											<label>　言語コード：<%#: Item.LanguageCode %> 言語ロケールID：<%#: Item.LanguageLocaleId %></label>
										</div>
										<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
											<div class="input-textarea">
												<div class="input-textarea-type-selector">
													<label><%#: ValueText.GetValueText(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_OUTLINE_KBN, Item.DisplayKbn) %></label>
												</div>
												<br />
												<hr width="100%" size="2px" align="center" />
												<div class="input-textarea-content">
													<%#: Item.AfterTranslationalName %>
												</div>
											</div>
										</div>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DESC_DETAIL1)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>商品詳細説明１</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_DESC_DETAIL1)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DESC_DETAIL1) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<div class="input-textarea">
										<div class="input-textarea-type-selector">
											<label"><%: ValueText.GetValueText(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_DESC_DETAIL_KBN1, this.ProductInput.DescDetailKbn1) %></label>
										</div>
										<hr width="100%" size="2px" align="center" style="margin: 5px 0px 5px 0px;" />
										<div class='input-textarea-content <%# string.IsNullOrEmpty(this.ProductInput.DescDetail1) ? "empty-value" : string.Empty %>'>
											<% if ((this.ProductInput.DescDetailKbn1 != Constants.FLG_PRODUCT_DESC_DETAIL_TEXT)
												&& (string.IsNullOrEmpty(this.ProductInput.DescDetail1) == false)) { %>
											<iframe class="HtmlPreview" src="<%: new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_HTML_PREVIEW_FORM).AddParam(Constants.HTML_PREVIEW_NO, "2").CreateUrl() %>" style="width: 100%; height: 100%;" frameborder="0" scrolling="yes">
											</iframe>
											<% } else { %>
											<%# GetProductValueToDisplay(this.ProductInput.DescDetail1, Constants.FIELD_PRODUCT_DESC_DETAIL1) %>
											<% } %>
										</div>
									</div>
								</div>
							</div>
							<% } %>
							<% if (Constants.GLOBAL_OPTION_ENABLE && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DESC_DETAIL1)) { %>
							<asp:Repeater id="rTranslationDescDetail1" runat="server" 
								DataSource="<%# this.ProductTranslationData.Where(d => (d.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_DESC_DETAIL1)) %>" 
								ItemType="w2.Domain.NameTranslationSetting.NameTranslationSettingModel">
								<ItemTemplate>
									<div class="form-element-group form-element-group-horizontal-grid">
										<div class="form-element-group-title break-text-hover">
											<label>　言語コード：<%#: Item.LanguageCode %> 言語ロケールID：<%#: Item.LanguageLocaleId %></label>
										</div>
										<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
											<div class="input-textarea">
												<div class="input-textarea-type-selector">
													<label><%#: ValueText.GetValueText(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_DESC_DETAIL_KBN1, Item.DisplayKbn) %></label>
												</div>
												<br />
												<hr width="100%" size="2px" align="center" />
												<div class="input-textarea-content">
													<%#: Item.AfterTranslationalName %>
												</div>
											</div>
										</div>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DESC_DETAIL2)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>商品詳細説明２</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_DESC_DETAIL2)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DESC_DETAIL2) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<div class="input-textarea">
										<div class="input-textarea-type-selector">
											<label><%: ValueText.GetValueText(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_DESC_DETAIL_KBN2, this.ProductInput.DescDetailKbn2) %></label>
										</div>
										<hr width="100%" size="2px" align="center" style="margin: 5px 0px 5px 0px;" />
										<div class='input-textarea-content <%# string.IsNullOrEmpty(this.ProductInput.DescDetail2) ? "empty-value" : string.Empty %>'>
											<% if ((this.ProductInput.DescDetailKbn2 != Constants.FLG_PRODUCT_DESC_DETAIL_TEXT)
												&& (string.IsNullOrEmpty(this.ProductInput.DescDetail2) == false)) { %>
											<iframe class="HtmlPreview" src="<%: new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_HTML_PREVIEW_FORM).AddParam(Constants.HTML_PREVIEW_NO, "3").CreateUrl() %>" style="width: 100%; height: 100%;" frameborder="0" scrolling="yes">
											</iframe>
											<% } else { %>
											<%# GetProductValueToDisplay(this.ProductInput.DescDetail2, Constants.FIELD_PRODUCT_DESC_DETAIL2) %>
											<% } %>
										</div>
									</div>
								</div>
							</div>
							<% } %>
							<% if (Constants.GLOBAL_OPTION_ENABLE && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DESC_DETAIL2)) { %>
							<asp:Repeater id="rTranslationDescDetail2" runat="server" 
								DataSource="<%# this.ProductTranslationData.Where(d => (d.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_DESC_DETAIL2)) %>" 
								ItemType="w2.Domain.NameTranslationSetting.NameTranslationSettingModel">
								<ItemTemplate>
									<div class="form-element-group form-element-group-horizontal-grid">
										<div class="form-element-group-title break-text-hover">
											<label>　言語コード：<%#: Item.LanguageCode %> 言語ロケールID：<%#: Item.LanguageLocaleId %></label>
										</div>
										<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
											<div class="input-textarea">
												<div class="input-textarea-type-selector">
													<label><%#: ValueText.GetValueText(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_DESC_DETAIL_KBN2, Item.DisplayKbn) %></label>
												</div>
												<br />
												<hr width="100%" size="2px" align="center" />
												<div class="input-textarea-content">
													<%#: Item.AfterTranslationalName %>
												</div>
											</div>
										</div>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DESC_DETAIL3)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>商品詳細説明３</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_DESC_DETAIL3)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DESC_DETAIL3) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<div class="input-textarea">
										<div class="input-textarea-type-selector">
											<label><%: ValueText.GetValueText(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_DESC_DETAIL_KBN3, this.ProductInput.DescDetailKbn3) %></label>
										</div>
										<hr width="100%" size="2px" align="center" style="margin: 5px 0px 5px 0px;" />
										<div class='input-textarea-content <%# string.IsNullOrEmpty(this.ProductInput.DescDetail3) ? "empty-value" : string.Empty %>'>
											<% if ((this.ProductInput.DescDetailKbn3 != Constants.FLG_PRODUCT_DESC_DETAIL_TEXT)
												&& (string.IsNullOrEmpty(this.ProductInput.DescDetail3) == false)) { %>
											<iframe class="HtmlPreview" src="<%: new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_HTML_PREVIEW_FORM).AddParam(Constants.HTML_PREVIEW_NO, "4").CreateUrl() %>" style="width: 100%; height: 100%;" frameborder="0" scrolling="yes">
											</iframe>
											<% } else { %>
											<%# GetProductValueToDisplay(this.ProductInput.DescDetail3, Constants.FIELD_PRODUCT_DESC_DETAIL3) %>
											<% } %>
										</div>
									</div>
								</div>
							</div>
							<% } %>
							<% if (Constants.GLOBAL_OPTION_ENABLE && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DESC_DETAIL3)) { %>
							<asp:Repeater id="rTranslationDescDetail3" runat="server" 
								DataSource="<%# this.ProductTranslationData.Where(d => (d.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_DESC_DETAIL3)) %>" 
								ItemType="w2.Domain.NameTranslationSetting.NameTranslationSettingModel">
								<ItemTemplate>
									<div class="form-element-group form-element-group-horizontal-grid">
										<div class="form-element-group-title break-text-hover">
											<label>　言語コード：<%#: Item.LanguageCode %> 言語ロケールID：<%#: Item.LanguageLocaleId %></label>
										</div>
										<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
											<div class="input-textarea">
												<div class="input-textarea-type-selector">
													<label><%#: ValueText.GetValueText(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_DESC_DETAIL_KBN3, Item.DisplayKbn) %></label>
												</div>
												<br />
												<hr width="100%" size="2px" align="center" />
												<div class="input-textarea-content">
													<%#: Item.AfterTranslationalName %>
												</div>
											</div>
										</div>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DESC_DETAIL4)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>商品詳細説明４</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_DESC_DETAIL4)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DESC_DETAIL4) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<div class="input-textarea">
										<div class="input-textarea-type-selector">
											<label><%: ValueText.GetValueText(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_DESC_DETAIL_KBN4, this.ProductInput.DescDetailKbn4) %></label>
										</div>
										<hr width="100%" size="2px" align="center" style="margin: 5px 0px 5px 0px;" />
										<div class='input-textarea-content <%# string.IsNullOrEmpty(this.ProductInput.DescDetail4) ? "empty-value" : string.Empty %>'>
											<% if ((this.ProductInput.DescDetailKbn4 != Constants.FLG_PRODUCT_DESC_DETAIL_TEXT)
												&& (string.IsNullOrEmpty(this.ProductInput.DescDetail4) == false)) { %>
											<iframe class="HtmlPreview" src="<%: new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_HTML_PREVIEW_FORM).AddParam(Constants.HTML_PREVIEW_NO, "5").CreateUrl() %>" style="width: 100%; height: 100%;" frameborder="0" scrolling="yes">
											</iframe>
											<% } else { %>
											<%# GetProductValueToDisplay(this.ProductInput.DescDetail4, Constants.FIELD_PRODUCT_DESC_DETAIL4) %>
											<% } %>
										</div>
									</div>
								</div>
							</div>
							<% } %>
							<% if (Constants.GLOBAL_OPTION_ENABLE && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DESC_DETAIL4)) { %>
							<asp:Repeater id="rTranslationDescDetail4" runat="server"
								DataSource="<%# this.ProductTranslationData.Where(d => (d.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_DESC_DETAIL4)) %>" 
								ItemType="w2.Domain.NameTranslationSetting.NameTranslationSettingModel">
								<ItemTemplate>
									<div class="form-element-group form-element-group-horizontal-grid">
										<div class="form-element-group-title break-text-hover">
											<label>　言語コード：<%#: Item.LanguageCode %> 言語ロケールID：<%#: Item.LanguageLocaleId %></label>
										</div>
										<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
											<div class="input-textarea">
												<div class="input-textarea-type-selector">
													<label><%#: ValueText.GetValueText(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_DESC_DETAIL_KBN4, Item.DisplayKbn) %></label>
												</div>
												<br />
												<hr width="100%" size="2px" align="center" />
												<div class="input-textarea-content">
													<%#: Item.AfterTranslationalName %>
												</div>
											</div>
										</div>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>返品・交換・解約説明(TEXT)</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE) %></p>
									<% } %>
								</div>
								<div class='form-element-group-content form-element-group-content-confirm break-text-hover <%# string.IsNullOrEmpty(this.ProductInput.ReturnExchangeMessage) ? "empty-value" : string.Empty %>'>
									<%= GetEncodedHtmlDisplayMessage(GetProductValueToDisplay(this.ProductInput.ReturnExchangeMessage)) %>
								</div>
							</div>
							<% } %>
							<% if (Constants.GLOBAL_OPTION_ENABLE && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE)) { %>
							<asp:Repeater ID="rTranslationReturnExchangeMessage" runat="server"
								DataSource="<%# this.ProductTranslationData.Where(d => (d.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_RETURN_EXCHANGE_MESSAGE)) %>" 
								ItemType="w2.Domain.NameTranslationSetting.NameTranslationSettingModel">
								<ItemTemplate>
									<div class="form-element-group form-element-group-horizontal-grid">
										<div class="form-element-group-title break-text-hover">
											<label> 言語コード:<%#: Item.LanguageCode %> 言語ロケールID:<%#: Item.LanguageLocaleId %></label>
										</div>
										<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
											<%#: Item.AfterTranslationalName %>
										</div>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<% } %>
							<% if (Constants.GOOGLESHOPPING_COOPERATION_OPTION_ENABLED && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_GOOGLE_SHOPPING_FLG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid ">
								<div class="form-element-group-title break-text-hover">
									<label>Googleショッピング連携</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_GOOGLE_SHOPPING_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_GOOGLE_SHOPPING_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<label><%: GetProductValueToDisplay(this.ProductInput.GoogleShoppingFlg, Constants.FIELD_PRODUCT_GOOGLE_SHOPPING_FLG) %></label>
								</div>
							</div>
							<% } %>
							<% if (GetDisplayProductInquireArea()) { %>
							<div class="form-element-group-heading">
								<h3 class="form-element-group-heading-label">問い合わせ</h3>
							</div>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_URL)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>紹介URL</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_URL)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_URL) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content break-text-hover" style="margin-top: 4px;">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.Url) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.Url) %>
									</label>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_INQUIRE_EMAIL)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>問い合わせ用メールアドレス</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_INQUIRE_EMAIL)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_INQUIRE_EMAIL) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content break-text-hover" style="margin-top: 4px;">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.InquireEmail) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.InquireEmail) %>
									</label>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_INQUIRE_TEL)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>問い合わせ用電話番号</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_INQUIRE_TEL)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_INQUIRE_TEL) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content break-text-hover" style="margin-top: 4px;">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.InquireTel) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.InquireTel) %>
									</label>
								</div>
							</div>
							<% } %>
							<% } %>
							<% if (GetDisplayProductCooperationIdsArea()) { %>
							<div class="form-element-group-heading">
								<h3 class="form-element-group-heading-label">商品連携ID</h3>
							</div>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID1)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>商品連携ID1</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID1)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID1) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.CooperationId1) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.CooperationId1) %>
									</label>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID2)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>商品連携ID2</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID2)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID2) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.CooperationId2) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.CooperationId2) %>
									</label>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID3)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>商品連携ID3</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID3)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID3) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.CooperationId3) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.CooperationId3) %>
									</label>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID4)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>商品連携ID4</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID4)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID4) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.CooperationId4) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.CooperationId4) %>
									</label>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID5)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>商品連携ID5</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID5)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID5) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.CooperationId5) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.CooperationId5) %>
									</label>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID6)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>商品連携ID6</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID6)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID6) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.CooperationId6) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.CooperationId6) %>
									</label>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID7)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>商品連携ID7</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID7)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID7) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.CooperationId7) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.CooperationId7) %>
									</label>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID8)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>商品連携ID8</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID8)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID8) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.CooperationId8) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.CooperationId8) %>
									</label>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID9)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>商品連携ID9</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID9)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID9) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.CooperationId9) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.CooperationId9) %>
									</label>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID10)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>商品連携ID10</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID10)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID10) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.CooperationId10) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.CooperationId10) %>
									</label>
								</div>
							</div>
							<% } %>
							<% } %>
						</div>
					</div>
					<div class="block-section-body-inner-row">
						<div class="block-section-body-inner-col">
							<% if(GetDisplayProductOptionValueArea()) { %>
							<div class="form-element-group-heading">
								<h3 class="form-element-group-heading-label">商品付帯</h3>
							</div>
							<div class="product-validate-form-element-group-container">
								<div class="product-register-option-toggle-content-basic">
									<div runat="server" visible="<%# (this.ProductOptionSettingLists.Items.Count == 0) %>" style="text-align: left;">
										<h2 class="form-element-group-heading-label">商品付帯管理しない</h2>
									</div>
									<div class="form-element-group-list" runat="server" visible="<%# (this.ProductOptionSettingLists.Items.Count > 0) %>">
										<asp:Repeater ID="rptProductOptionValue" runat="server" 
											DataSource="<%# this.ProductOptionSettingLists.Items %>" 
											onitemdatabound="rptProductOptionValue_ItemDataBound">
											<ItemTemplate>
												<div class="form-element-group-list-item">
													<div id="dvOptionHeader" class="form-element-group-list-item-title break-text-hover" runat="server" style="min-width: 150px;">
														<label>付帯情報<%# (Container.ItemIndex + 1) %></label><br />
														<asp:Label ID="lbDefaultSettingComment" runat="server" CssClass="note" />
													</div>
													<div id="dvOptionContent" class="form-element-group-list-item-content" runat="server">
														<div class="product-register-option-toggle-content-basic-setting">
															<div class="form-element-group">
																<div class="form-element-group-title">
																	<label>項目名</label>
																</div>
																<div class="form-element-group-content break-text-hover">
																	<asp:Label ID="lbItemName" runat="server" />
																</div>
															</div>
															<div class="form-element-group">
																<div class="form-element-group-title">
																	<label>表示形式</label>
																</div>
																<div class="form-element-group-content break-text-hover">
																	<asp:Label ID="lbDisplayFormat" runat="server" />
																</div>
															</div>
															<div id="dvSettingNonTextbox" class="product-register-option-toggle-content-basic-setting-style-list" runat="server" visible="false">
																<div class="form-element-group">
																	<div class="form-element-group-title">
																		設定値
																	</div>
																	<div class="form-element-group-content break-text-hover">
																		<asp:Label ID="lbSettingValue" runat="server" />
																	</div>
																</div>
															</div>
															<div id="dvSettingTextbox" class="product-register-option-toggle-content-basic-setting-style-text" runat="server" visible="false">
																<div class="form-element-group">
																	<div class="form-element-group-title">
																		<label>初期値</label>
																	</div>
																	<div class="form-element-group-content break-text-hover">
																		<asp:Label ID="lblDefaultForTb" runat="server" Width="250" />
																	</div>
																</div>
																<div class="form-element-group">
																	<div class="form-element-group-title">
																		<label>入力チェック種別</label>
																	</div>
																	<div class="form-element-group-content break-text-hover">
																		<asp:Label ID="lblCheckType" runat="server" />
																	</div>
																</div>
																<div class="form-element-group">
																	<div class="form-element-group-title">
																		<label>必須チェック</label>
																	</div>
																	<div class="form-element-group-content break-text-hover">
																		<div class="form-element-group-content-item">
																			<asp:Label ID="lblNecessary" runat="server" />
																		</div>
																	</div>
																</div>
																<div id="dvLengthInputArea" class="product-register-option-toggle-content-basic-setting-check-style-text" runat="server" visible="false">
																	<div class="form-element-group">
																		<div class="form-element-group-title">
																			<label>文字数</label>
																		</div>
																		<div class="form-element-group-content break-text-hover">
																			<div class="form-element-group-content-item-inline">
																				<asp:Label ID="lblFixedLength" runat="server" />
																			</div>
																		</div>
																	</div>
																	<div id="dvMaxMinLengthInputArea" class="product-register-option-toggle-content-basic-setting-check-style-text-range" runat="server" visible="false">
																		<div class="form-element-group">
																			<div class="form-element-group-title">
																				<label>入力文字数制限</label>
																			</div>
																			<div class="form-element-group-content break-text-hover">
																				<asp:Label ID="lblLengthMin" runat="server" MaxLength="5" class="w3em" />
																				文字以上～
																				<asp:Label ID="lblLengthMax" runat="server" MaxLength="5" class="w3em" />
																				文字以下
																			</div>
																		</div>
																	</div>
																	<div id="dvFixedLengthInputArea" class="product-register-option-toggle-content-basic-setting-check-style-text-fixed" runat="server" visible="false">
																		<div class="form-element-group">
																			<div class="form-element-group-title">
																				<label>入力文字数制限</label>
																			</div>
																			<div class="form-element-group-content break-text-hover">
																				<asp:Label ID="lblLength" runat="server" MaxLength="5" class="w3em" />
																				文字
																			</div>
																		</div>
																	</div>
																</div>
																<div id="dvNumberRangeInputArea" class="product-register-option-toggle-content-basic-setting-check-style-number" runat="server" visible="false">
																	<div class="form-element-group">
																		<div class="form-element-group-title">
																			<label>数値範囲</label>
																		</div>
																		<div class="form-element-group-content break-text-hover">
																			<asp:Label ID="lblNumMin" runat="server" MaxLength="5" class="w3em" />
																			～
																			<asp:Label ID="lblNumMax" runat="server" MaxLength="5" class="w3em" />
																		</div>
																	</div>
																</div>
															</div>
														</div>
													</div>
												</div>
											</ItemTemplate>
										</asp:Repeater>
									</div>
								</div>
							</div>
							<% } %>
						</div>
					</div>
				</div>
			</div>
		</div>
		<div class="block-section-row">
			<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED && GetDisplayFixedPurchaseArea()) { %>
			<div class="block-section block-section-product-register-fixedpurchase">
				<div class="block-section-header">
					<div class="block-section-header-icon"><span class="icon-loop"></span></div>
					<h2 class="block-section-header-txt">定期購入</h2>
				</div>
				<div class="block-section-body">
					<div class="block-section-body-inner-row">
						<div class="block-section-body-inner-col">
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>定期購入フラグ</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<label><%: GetProductValueToDisplay(this.ProductInput.FixedPurchaseFlg, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG) %></label>
								</div>
							</div>
							<% } %>
							<% if (this.CanFixedPurchase) { %>
							<div class="product-register-teiki-purchase-toggle-content">
								<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING)
									&& CheckProductLimitedFixedPurchaseDisplay(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING)) { %>
								<div id="divLimitedFixedPurchaseKbn1" class="form-element-group form-element-group-horizontal-grid ">
									<div class="form-element-group-title break-text-hover" style="width: 220px !important">
										<label>定期購入配送間隔月利用不可</label>
										<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING)) { %>
										<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING) %></p>
										<% } %>
									</div>
									<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
										<label class='<%# string.IsNullOrEmpty(this.ProductInput.LimitedFixedPurchaseKbn1Setting) ? "empty-value" : string.Empty %>'>
											<%: GetProductValueToDisplay(this.ProductInput.LimitedFixedPurchaseKbn1Setting, Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING) %>
										</label>
									</div>
								</div>
								<% } %>
								<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING)
									&& CheckProductLimitedFixedPurchaseDisplay(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING)) { %>
								<div id="divLimitedFixedPurchaseKbn4" class="form-element-group form-element-group-horizontal-grid ">
									<div class="form-element-group-title break-text-hover" style="width: 220px !important">
										<label>定期購入配送間隔週利用不可</label>
										<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING)) { %>
										<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING) %></p>
										<% } %>
									</div>
									<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
										<label class='<%# string.IsNullOrEmpty(this.ProductInput.LimitedFixedPurchaseKbn4Setting) ? "empty-value" : string.Empty %>'>
											<%: GetProductValueToDisplay(this.ProductInput.LimitedFixedPurchaseKbn4Setting, Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING) %>
										</label>
									</div>
								</div>
								<% } %>
								<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING)
									&& CheckProductLimitedFixedPurchaseDisplay(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING)) { %>
								<div id="divLimitedFixedPurchaseKbn3" class="form-element-group form-element-group-horizontal-grid ">
									<div class="form-element-group-title break-text-hover" style="width: 220px !important">
										<label>定期購入配送間隔利用不可</label>
										<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING)) { %>
										<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING) %></p>
										<% } %>
									</div>
									<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
										<label class='<%# string.IsNullOrEmpty(this.ProductInput.LimitedFixedPurchaseKbn3Setting) ? "empty-value" : string.Empty %>'>
											<%: GetProductValueToDisplay(this.ProductInput.LimitedFixedPurchaseKbn3Setting, Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING) %>
										</label>
									</div>
								</div>
								<% } %>
							</div>
							<div class="product-register-teiki-purchase-toggle-content" runat="server" visible="<%# this.IsDisplayFixedpurchaseArea %>">
								<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT)) { %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title break-text-hover">
										<label>定期購入解約可能回数（出荷基準）</label>
										<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT)) { %>
										<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT) %></p>
										<% } %>
									</div>
									<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
										<label class='<%# string.IsNullOrEmpty(this.ProductInput.FixedPurchasedCancelableCount) ? "empty-value" : string.Empty %>'>
											<%: GetProductValueToDisplay(this.ProductInput.FixedPurchasedCancelableCount, Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT) %>
										</label>
									</div>
								</div>
								<% } %>
								<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT)) { %>
								<div class="form-element-group form-element-group-horizontal-grid ">
									<div class="form-element-group-title break-text-hover">
										<label>定期購入スキップ制限回数</label>
										<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT)) { %>
										<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT) %></p>
										<% } %>
									</div>
									<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
										<label class='<%# string.IsNullOrEmpty(this.ProductInput.FixedPurchaseLimitedSkippedCount) ? "empty-value" : string.Empty %>'>
											<%: GetProductValueToDisplay(this.ProductInput.FixedPurchaseLimitedSkippedCount, Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT) %>
										</label>
									</div>
								</div>
								<% } %>
								<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_USER_LEVEL_IDS)) { %>
								<div class="form-element-group form-element-group-horizontal-grid ">
									<div class="form-element-group-title break-text-hover">
										<label>定期購入利用不可ユーザー管理レベル</label>
										<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_USER_LEVEL_IDS)) { %>
										<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_USER_LEVEL_IDS) %></p>
										<% } %>
									</div>
									<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
										<label class='<%# string.IsNullOrEmpty(this.ProductInput.FixedPurchasedLimitedUserLevelIds) ? "empty-value" : string.Empty %>'>
											<%: GetFixedPurchaseLimitedUserLevelDisplay(this.ProductInput.FixedPurchasedLimitedUserLevelIds) %>
										</label>
									</div>
								</div>
								<% } %>
								<% if (Constants.FIXEDPURCHASE_NEXTSHIPPING_OPTION_ENABLED && GetProductDefaultSettingDisplayField(PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_SETTING)) { %>
								<div class="form-element-group form-element-group-horizontal-grid ">
									<div class="form-element-group-title break-text-hover">
										<label>定期購入2回目以降商品</label>
										<% if (HasProductDefaultSettingComment(PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_SETTING)) { %>
										<p class="note"><%: GetProductDefaultSettingComment(PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_SETTING) %></p>
										<% } %>
									</div>
									<% if(string.IsNullOrEmpty(this.ProductInput.FixedPurchaseNextShippingProductId) == false) { %>
									<div class="form-element-group-content">
										<div class="mb10">
											<table class="table-basic" style="width: auto;">
												<tbody>
													<tr align="center">
														<th width="20%">商品ID</th>
														<th width="20%">(商品ID+)<br />バリエーションID</th>
														<th>商品名</th>
														<th width="15%">数量</th>
													</tr>
													<tr>
														<td class="break-text-hover">
															<a href="#" onclick="javascript:open_window(
																'<%: CreateProductDetailUrl(this.ProductInput.FixedPurchaseNextShippingProductId, true) %>','productdetail','width=1000,height=826,top=110,left=380,status=NO,scrollbars=yes');return false;">
																<%: this.ProductInput.FixedPurchaseNextShippingProductId %>
															</a>
														</td>
														<td class="break-text-hover">
															<%: (this.ProductInput.FixedPurchaseNextShippingProductId != this.ProductInput.FixedPurchaseNextShippingVariationId)
																? "商品ID + " + CreatedVariationIdForDisplay(this.ProductInput.DataSource)
																: "-" %>
														</td>
														<td class="break-text-hover">
															<%: GetProductNameForFixedPurchaseNextShippingSetting(this.ProductInput.DataSource) %>
														</td>
														<td class="break-text-hover">
															<label style="padding-bottom: 2px;">元の商品数 x <%: this.ProductInput.FixedPurchaseNextShippingItemQuantity %></label>
														</td>
													</tr>
												</tbody>
											</table>
										</div>
										<p runat="server" class="p" visible="<%# (string.IsNullOrEmpty(CreateNextShippingItemFixedPurchaseSettingMessage(this.ProductInput.DataSource)) == false) %>">
											<div class="form-element-group-content-related-link">
												定期配送パターン : <%: CreateNextShippingItemFixedPurchaseSettingMessage(this.ProductInput.DataSource) %>
											</div>
										</p>
									</div>
									<% } else { %>
									<div class="form-element-group-content" style="margin-top: 1.2px;">
										<label>定期購入2回目以降商品管理しない</label>
									</div>
									<% } %>
								</div>
								<% } %>
								<% if (Constants.FIXED_PURCHASE_DISCOUNT_PRICE_OPTION_ENABLE) { %>
								<div id="dvProductFixedPurchaseDiscountSection" class="form-element-group form-element-group-horizontal-grid" runat="server" visible="false">
									<div class="form-element-group-title">
										<label>定期購入割引</label>
									</div>
									<div class="form-element-group-content product-validate-form-element-group-container">
										<div class="sample-checkbox-toggle-area-2" style="overflow-x: auto;">
											<table class="table-basic table-discount" style="width: auto; display: table-cell;">
												<tbody>
													<tr>
														<td width="70"></td>
														<asp:Repeater id="rFixedPurchaseDiscountProductCount" runat="server">
															<ItemTemplate>
																<td colspan="2" align="center" width="160">
																	購入個数<%#: ((ProductFixedPurchaseDiscountSettingDetail)Container.DataItem).ProductCount %>個から
																</td>
															</ItemTemplate>
														</asp:Repeater>
													</tr>
													<asp:Repeater id="rFixedPurchaseDiscountOrderCount" runat="server">
														<Itemtemplate>
															<tr>
																<td align="center" width="70">
																	購入回数<br />
																	<%#: ((ProductFixedPurchaseDiscountSettingHeader)Container.DataItem).OrderCount %>回
																	<%#: (((ProductFixedPurchaseDiscountSettingHeader)Container.DataItem).OrderCountMoreThanFlg == Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_ORDER_COUNT_MORE_THAN_FLG_VALID)
																		? "以降"
																		: string.Empty %>
																</td>
																<asp:Repeater id="rFixedPurchaseDiscountDetail" runat="server" DataSource="<%# ((ProductFixedPurchaseDiscountSettingHeader)Container.DataItem).ProductCountDiscounts %>">
																	<ItemTemplate>
																		<td align="left" width="90" >
																			<div runat="server" visible="<%# ProductFixedPurchaseDiscountAvailability((ProductFixedPurchaseDiscountSettingDetail)Container.DataItem) %>">値引き</div>
																			<% if (Constants.W2MP_POINT_OPTION_ENABLED) { %>
																				<div runat="server" visible="<%# ProductFixedPurchaseDiscountPointRewardAvailability((ProductFixedPurchaseDiscountSettingDetail)Container.DataItem) %>">ポイント特典</div>
																			<% } %>
																		</td>
																		<td width="70" style="background: #fff;">
																			<div>
																				<%#: ((ProductFixedPurchaseDiscountSettingDetail)Container.DataItem).DiscountValue.ToPriceString() %>
																				<%#: (string.IsNullOrEmpty(((ProductFixedPurchaseDiscountSettingDetail)Container.DataItem).DiscountValue) == false)
																					? ValueText.GetValueText(
																						Constants.TABLE_PRODUCTFIXEDPURCHASEDISCOUNTSETTING,
																						Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE,
																						((ProductFixedPurchaseDiscountSettingDetail)Container.DataItem).DiscountType)
																					: string.Empty %>
																			</div>
																			<% if (Constants.W2MP_POINT_OPTION_ENABLED) { %>
																			<div runat="server" visible="<%# ((string.IsNullOrEmpty(((ProductFixedPurchaseDiscountSettingDetail)Container.DataItem).DiscountValue))
																				|| (string.IsNullOrEmpty(((ProductFixedPurchaseDiscountSettingDetail)Container.DataItem).PointValue))) %>" >
																			</div>
																			<div>
																				<%#: ((ProductFixedPurchaseDiscountSettingDetail)Container.DataItem).PointValue %>
																				<%#: (string.IsNullOrEmpty(((ProductFixedPurchaseDiscountSettingDetail)Container.DataItem).PointValue) == false)
																					? ValueText.GetValueText(
																						Constants.TABLE_PRODUCTFIXEDPURCHASEDISCOUNTSETTING,
																						Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_POINT_TYPE,
																						((ProductFixedPurchaseDiscountSettingDetail)Container.DataItem).PointType)
																					: string.Empty %>
																			</div>
																			<% } %>
																		</td>
																	</ItemTemplate>
																</asp:Repeater>
															</tr>
														</Itemtemplate>
													</asp:Repeater>
												</tbody>
											</table>
										</div>
									</div>
								</div>
								<% } %>
								<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG)
									&& Constants.PRODUCT_ORDER_LIMIT_ENABLED) { %>
								<div class="form-element-group form-element-group-horizontal-grid ">
									<div class="form-element-group-title break-text-hover">
										<label>定期商品重複購入制限フラグ</label>
										<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG)) { %>
										<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG) %></p>
										<% } %>
									</div>
									<div class="form-element-group-content form-element-group-content-confirm">
										<label><%: GetProductValueToDisplay(this.ProductInput.CheckFixedProductOrderFlg, Constants.FIELD_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG) %></label>
									</div>
								</div>
								<% } %>
								<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG)) { %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title break-text-hover">
										<label>頒布会フラグ</label>
										<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG)) { %>
										<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG) %></p>
										<% } %>
									</div>
									<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
										<label><%: GetProductValueToDisplay(this.ProductInput.SubscriptionBoxFlg, Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG) %></label>
									</div>
								</div>
								<% } %>
							</div>
							<% } %>
						</div>
					</div>
				</div>
			</div>
			<% } %>
		</div>
		<div class="block-section-row">
			<% if (GetDisplayLimitedOptionArea()) { %>
			<div class="block-section block-section-product-register-limit ">
				<div class="block-section-header">
					<div class="block-section-header-icon"><span class="icon-lock"></span></div>
					<h2 class="block-section-header-txt">制限設定</h2>
				</div>
				<div class="block-section-body">
					<div class="block-section-body-inner-row">
						<div class="block-section-body-inner-col">
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>利用不可決済</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS) %></p>
									<% } %>
								</div>
								<div class='form-element-group-content form-element-group-content-confirm break-text-hover <%# string.IsNullOrEmpty(this.ProductInput.LimitedPaymentIds) ? "empty-value" : string.Empty %>'>
									<%: GetLimitedPaymentDisplay(this.ProductInput.LimitedPaymentIds) %>
								</div>
							</div>
							<% } %>
							<% if (Constants.MEMBER_RANK_OPTION_ENABLED) { %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DISPLAY_MEMBER_RANK)) { %>
							<div class="form-element-group form-element-group-horizontal-grid ">
								<div class="form-element-group-title break-text-hover">
									<label>閲覧可能会員ランク</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_MEMBER_RANK)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_MEMBER_RANK) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<label class='<%# string.IsNullOrEmpty(MemberRankOptionUtility.GetMemberRankName(this.ProductInput.DisplayMemberRank)) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(MemberRankOptionUtility.GetMemberRankName(this.ProductInput.DisplayMemberRank)) %>
									</label>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid ">
								<div class="form-element-group-title break-text-hover">
									<label>定期会員限定フラグ（閲覧）</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content" style="margin-top: 2px;">
									<label><%: GetProductValueToDisplay(this.ProductInput.DisplayOnlyFixedPurchaseMemberFlg, Constants.FIELD_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG) %></label>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK)) { %>
							<div class="form-element-group form-element-group-horizontal-grid ">
								<div class="form-element-group-title break-text-hover">
									<label>販売可能会員ランク</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<label class='<%# string.IsNullOrEmpty(MemberRankOptionUtility.GetMemberRankName(this.ProductInput.BuyableMemberRank)) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(MemberRankOptionUtility.GetMemberRankName(this.ProductInput.BuyableMemberRank)) %>
									</label>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>定期会員限定フラグ（購入）</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content" style="margin-top: 2px;">
									<label><%: GetProductValueToDisplay(this.ProductInput.SellOnlyFixedPurchaseMemberFlg, Constants.FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG) %></label>
								</div>
							</div>
							<% } %>
							<% } %>
							<% if (Constants.PRODUCT_ORDER_LIMIT_ENABLED) { %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>通常商品重複購入制限フラグ</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content" style="margin-top: 2px;">
									<label><%: GetProductValueToDisplay(this.ProductInput.CheckProductOrderFlg, Constants.FIELD_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG) %></label>
								</div>
							</div>
							<% } %>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ADD_CART_URL_LIMIT_FLG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>カート投入URL制限フラグ</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_ADD_CART_URL_LIMIT_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_ADD_CART_URL_LIMIT_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content" style="margin-top: 2px;">
									<label><%: GetProductValueToDisplay(this.ProductInput.AddCartUrlLimitFlg, Constants.FIELD_PRODUCT_ADD_CART_URL_LIMIT_FLG) %></label>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_AGE_LIMIT_FLG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>年齢制限フラグ</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_AGE_LIMIT_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_AGE_LIMIT_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content" style="margin-top: 2px;">
									<label><%: GetProductValueToDisplay(this.ProductInput.AgeLimitFlg, Constants.FIELD_PRODUCT_AGE_LIMIT_FLG) %></label>
								</div>
							</div>
							<% } %>
						</div>
					</div>
				</div>
			</div>
			<% } %>
		</div>
		<div class="block-section-row">
			<% if (Constants.MALLCOOPERATION_OPTION_ENABLED && GetDisplayMallCooperationArea()) { %>
			<div class="block-section block-section-product-register-mall ">
				<div class="block-section-header">
					<div class="block-section-header-icon"><span class="icon-mall"></span></div>
					<h2 class="block-section-header-txt">モール</h2>
				</div>
				<div class="block-section-body">
					<div class="block-section-body-inner-row">
						<div class="block-section-body-inner-col">
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_MALL_EX_PRODUCT_ID)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>モール拡張商品ID</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_MALL_EX_PRODUCT_ID)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_MALL_EX_PRODUCT_ID) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.MallExProductId) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.MallExProductId) %>
									</label>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>モール出品設定</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<label class='<%# string.IsNullOrEmpty(CreateMallNameList(this.LoginOperatorShopId, this.MallExhibitsConfig)) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(CreateMallNameList(this.LoginOperatorShopId, this.MallExhibitsConfig)) %>
									</label>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ANDMALL_RESERVATION_FLG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>＆mall連携予約商品フラグ</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_ANDMALL_RESERVATION_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_ANDMALL_RESERVATION_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm">
									<label><%: GetProductValueToDisplay(this.ProductInput.AndmallReservationFlg, Constants.FIELD_PRODUCT_ANDMALL_RESERVATION_FLG) %></label>
								</div>
							</div>
							<% } %>
						</div>
					</div>
				</div>
			</div>
			<% } %>
			<% if (Constants.DIGITAL_CONTENTS_OPTION_ENABLED && GetDisplayDigitalContentArea()) { %>
			<div class="block-section block-section-product-register-digital-contents ">
				<div class="block-section-header">
					<div class="block-section-header-icon"><span class="icon-cloud-download"></span></div>
					<h2 class="block-section-header-txt">デジタルコンテンツ</h2>
				</div>
				<div class="block-section-body">
					<div class="block-section-body-inner-row">
						<div class="block-section-body-inner-col">
							<%if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>デジタルコンテンツ商品フラグ</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm">
									<label><%: GetProductValueToDisplay(this.ProductInput.DigitalContentsFlg, Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG) %></label>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DOWNLOAD_URL)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>ダウンロードURL</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_DOWNLOAD_URL)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DOWNLOAD_URL) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.DownloadUrl) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.DownloadUrl) %>
									</label>
								</div>
							</div>
							<% } %>
						</div>
					</div>
				</div>
			</div>
			<% } %>
			<% if (Constants.USERPRODUCTARRIVALMAIL_OPTION_ENABLED && GetDisplayProductNotificationSettingsArea()) { %>
			<div class="block-section block-section-product-register-notice ">
				<div class="block-section-header">
					<div class="block-section-header-icon"><span class="icon-notice"></span></div>
					<h2 class="block-section-header-txt">通知設定</h2>
				</div>
				<div class="block-section-body">
					<div class="block-section-body-inner-row">
						<div class="block-section-body-inner-col">
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ARRIVAL_MAIL_VALID_FLG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>再入荷通知メール有効フラグ</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_ARRIVAL_MAIL_VALID_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_ARRIVAL_MAIL_VALID_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm">
									<label><%: GetProductValueToDisplay(this.ProductInput.ArrivalMailValidFlg, Constants.FIELD_PRODUCT_ARRIVAL_MAIL_VALID_FLG) %></label>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_RELEASE_MAIL_VALID_FLG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>販売開始通知メール有効フラグ</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_RELEASE_MAIL_VALID_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_RELEASE_MAIL_VALID_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm">
									<label><%: GetProductValueToDisplay(this.ProductInput.ReleaseMailValidFlg, Constants.FIELD_PRODUCT_RELEASE_MAIL_VALID_FLG) %></label>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_RESALE_MAIL_VALID_FLG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>再販売通知メール有効フラグ</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_RESALE_MAIL_VALID_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_RESALE_MAIL_VALID_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm">
									<label><%: GetProductValueToDisplay(this.ProductInput.ResaleMailValidFlg, Constants.FIELD_PRODUCT_RESALE_MAIL_VALID_FLG) %></label>
								</div>
							</div>
							<% } %>
						</div>
					</div>
				</div>
			</div>
			<% } %>
		</div>
		<div class="block-section-row">
			<div class="block-section block-section-product-register-display">
				<div class="block-section-header">
					<div class="block-section-header-icon"><span class="icon-operator"></span></div>
					<h2 class="block-section-header-txt">表示設定</h2>
				</div>
				<div class="block-section-body">
					<div class="block-section-body-inner-row">
						<div class="block-section-body-inner-col">
							<div id="comProductImage" class="form-element-group form-element-group-vertical">
								<div id="comProductImageTitle" class="form-element-group-title break-text-hover">
									<label><%: Constants.PRODUCT_IMAGE_HEAD_ENABLED == false ? "商品画像" : "商品画像(画像名ヘッダ:" + this.ProductInput.ImageHead + ")" %></label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_IMAGE_HEAD)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_IMAGE_HEAD) %></p>
									<% } %>
								</div>
								<div id="comProductImageContent" class="form-element-group-content">
									<div class="product-register-image-selecter">
										<div class="product-register-image-selecter-image-main">
											<div class="product-register-image-selecter-image-main-title">
												メイン画像
											</div>
											<div class="product-register-image-selecter-image-main-thum">
												<img alt="" src="<%: GetImageSource(this.UploadImageInput.MainImage.FileName) %>" class="product-register-image-selecter-image-main-thum-img" />
											</div>
										</div>
										<div class="product-register-image-selecter-image-sub-wrapper" style="overflow-y: auto; max-height: 20rem; max-width:30rem">
											<asp:Repeater ID="rProductSubImage" runat="server" ItemType="UploadProductSubImageInput">
												<ItemTemplate>
													<div class="product-register-image-selecter-image-sub" style="">
														<div class="product-register-image-selecter-image-sub-title"><%#: GetSubImageSettingName(Item.ImageNo) %></div>
														<div class="product-register-image-selecter-image-sub-thum">
															<img alt="" src="<%#: GetImageSource(Item.FileName, false) %>" class="product-register-image-selecter-image-sub-thum-img" />
														</div>
													</div>
												</ItemTemplate>
											</asp:Repeater>
										</div>
									</div>
								</div>
							</div>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_PRODUCT_COLOR_ID)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>カラー設定</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_PRODUCT_COLOR_ID)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_PRODUCT_COLOR_ID) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content" style='<%= string.IsNullOrEmpty(this.ProductInput.ProductColorId) ? "margin-top: 0.2em;" : string.Empty %>'>
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.ProductColorId) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(ProductColorUtility.GetColorImageDispName(this.ProductInput.ProductColorId)) %>
									</label>
									&nbsp;&nbsp;&nbsp;<span class="product-color-img"><img src='<%#: ProductColorUtility.GetColorImageUrl(this.ProductInput.ProductColorId) %>' style="vertical-align: middle;" height="25" width="25" alt='<%#: ProductColorUtility.GetColorImageDispName(this.ProductInput.ProductColorId) %>' runat="server" visible="<%# string.IsNullOrEmpty(this.ProductInput.ProductColorId) == false %>" /></span>
								</div>
							</div>
							<% } %>
						</div>
						<div class="block-section-body-inner-col">
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>表示期間</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_TO)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_TO) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm">
									<label><%: DateTimeUtility.ToStringForManager(this.ProductInput.DisplayFrom, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) + " ～ " + DateTimeUtility.ToStringForManager(this.ProductInput.DisplayTo, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></label>
								</div>
							</div>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>販売期間</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_SELL_TO)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_SELL_TO) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm">
									<label><%: DateTimeUtility.ToStringForManager(this.ProductInput.SellFrom, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) + " ～ " + DateTimeUtility.ToStringForManager(this.ProductInput.SellTo, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></label>
									<div class="form-element-group-content-item">
										<label>(<%: ValueText.GetValueText(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_DISPLAY_SELL_FLG, this.ProductInput.DisplaySellFlg) %>)</label>
									</div>
								</div>
							</div>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label>表示優先順</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_PRIORITY)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_PRIORITY) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.DisplayPriority) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.DisplayPriority) %>
									</label>
								</div>
							</div>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DISPLAY_KBN)) { %>
							<div class="form-element-group form-element-group-horizontal-grid ">
								<div class="form-element-group-title break-text-hover">
									<label>商品表示区分</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_KBN)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_KBN) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-register-display-kubun product-validate-form-element-group-container">
									<div runat="server" class="form-element-group-content-item" visible="<%# this.ProductInput.DisplayKbn == Constants.FLG_PRODUCT_DISPLAY_DISP_ALL %>">
										<label class="label-card">
											<dl class="product-register-display-kubun-tag product-register-display-kubun-tag-on">
												<dt>商品一覧</dt>
												<dd>表示あり</dd>
											</dl>
											<dl class="product-register-display-kubun-tag product-register-display-kubun-tag-on">
												<dt>商品詳細</dt>
												<dd>表示あり</dd>
											</dl>
										</label>
									</div>
									<div runat="server" class="form-element-group-content-item" visible="<%# this.ProductInput.DisplayKbn == Constants.FLG_PRODUCT_DISPLAY_DISP_ONLY_DETAIL %>">
										<label class="label-card">
											<dl class="product-register-display-kubun-tag">
												<dt>商品一覧</dt>
												<dd>表示なし</dd>
											</dl>
											<dl class="product-register-display-kubun-tag product-register-display-kubun-tag-on">
												<dt>商品詳細</dt>
												<dd>表示あり</dd>
											</dl>
										</label>
									</div>
									<div runat="server" class="form-element-group-content-item" visible="<%# this.ProductInput.DisplayKbn == Constants.FLG_PRODUCT_DISPLAY_UNDISP_ALL %>">
										<label class="label-card">
											<dl class="product-register-display-kubun-tag">
												<dt>商品一覧</dt>
												<dd>表示なし</dd>
											</dl>
											<dl class="product-register-display-kubun-tag">
												<dt>商品詳細</dt>
												<dd>表示なし</dd>
											</dl>
										</label>
									</div>
								</div>
							</div>
							<% } %>
						</div>
					</div>
					<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN)) { %>
					<div class="block-section-body-inner-row">
						<div class="block-section-body-inner-col">
							<div class="form-element-group form-element-group-vertical ">
								<div class="form-element-group-title break-text-hover">
									<div class="form-element-group-heading">
										<h3 class="form-element-group-heading-label">商品バリエーション表示方法</h3>
									</div>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content">
									<div class="product-register-product-variation-kubun">
										<div class="product-register-product-variation-kubun-item">
											<label class="radio-card">
												<span class="product-register-product-variation-kubun-item-thum">
													<img src='<%: string.Format("{0}select_variation_kbn_{1}.jpg", Constants.PATH_ROOT_EC + Constants.PATH_SELECT_VARIATION_KBN_ICON_IMAGE, this.ProductInput.SelectVariationKbn) %>' alt='<%= this.ProductInput.SelectVariationKbn %>'>
												</span>
												<span class="product-register-product-variation-kubun-item-description">
													<span class="product-register-product-variation-kubun-item-description-txt1"><%: GetFirstDescriptionSelectVariationKbn(this.ProductInput.SelectVariationKbn) %></span>
													<span class="product-register-product-variation-kubun-item-description-txt2"><%: GetSecondDescriptionSelectVariationKbn(this.ProductInput.SelectVariationKbn) %></span>
												</span>
											</label>
										</div>
									</div>
								</div>
							</div>
						</div>
					</div>
					<% } %>
					<% if (GetDisplayIconArea()) { %>
					<div class="block-section-body-inner-row">
						<div class="block-section-body-inner-col">
							<div class="form-element-group form-element-group-vertical ">
								<div class="form-element-group-heading ">
									<h3 class="form-element-group-heading-label">キャンペーンアイコン</h3>
								</div>
								<div class="form-element-group-content">
									<div class="form-element-group-list product-register-campaign-icon">
										<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ICON_FLG1)) { %>
										<div class="form-element-group-list-item">
											<div class="form-element-group-list-item-title break-text-hover" style="min-width: 150px;">
												アイコン１
												<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_ICON_FLG1)) { %>
												<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_ICON_FLG1) %></p>
												<% } %>
											</div>
											<div class="form-element-group-list-item-content product-validate-form-element-group-container">
												<div class="form-element-group w5">
													<img src='<%: Constants.PATH_ROOT_FRONT_PC + Constants.IMG_FRONT_PRODUCT_ICON1 %>' class="w4em">
												</div>
												<div class="form-element-group w10">
													<div class="form-element-group-content">
														<label><%: ValueText.GetValueText(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_ICON_FLG1, this.ProductInput.IconFlg1) %></label>
													</div>
												</div>
												<div class="form-element-group w85" style="flex-flow: wrap;" runat="server" visible="<%# this.ProductInput.IconFlg1 == Constants.FLG_PRODUCT_ICON_ON %>">
													<div class="form-element-group-title">
														有効期限
													</div>
													<div class="form-element-group-content" style="max-width:22rem;">
														<label><%: DateTimeUtility.ToStringForManager(this.ProductInput.IconTermEnd1, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></label>
													</div>
												</div>
											</div>
										</div>
										<% } %>
										<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ICON_FLG2)) { %>
										<div class="form-element-group-list-item">
											<div class="form-element-group-list-item-title break-text-hover" style="min-width: 150px;">
												アイコン２
												<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_ICON_FLG2)) { %>
												<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_ICON_FLG2) %></p>
												<% } %>
											</div>
											<div class="form-element-group-list-item-content product-validate-form-element-group-container">
												<div class="form-element-group w5">
													<img src='<%: Constants.PATH_ROOT_FRONT_PC + Constants.IMG_FRONT_PRODUCT_ICON2 %>' class="w4em">
												</div>
												<div class="form-element-group w10">
													<div class="form-element-group-content">
														<label><%: ValueText.GetValueText(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_ICON_FLG2, this.ProductInput.IconFlg2) %></label>
													</div>
												</div>
												<div class="form-element-group w85" style="flex-flow: wrap;" runat="server" visible="<%# this.ProductInput.IconFlg2 == Constants.FLG_PRODUCT_ICON_ON %>">
													<div class="form-element-group-title">
														有効期限
													</div>
													<div class="form-element-group-content" style="max-width:22rem;">
														<label><%: DateTimeUtility.ToStringForManager(this.ProductInput.IconTermEnd2, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></label>
													</div>
												</div>
											</div>
										</div>
										<% } %>
										<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ICON_FLG3)) { %>
										<div class="form-element-group-list-item">
											<div class="form-element-group-list-item-title break-text-hover" style="min-width: 150px;">
												アイコン３
												<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_ICON_FLG3)) { %>
												<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_ICON_FLG3) %></p>
												<% } %>
											</div>
											<div class="form-element-group-list-item-content product-validate-form-element-group-container">
												<div class="form-element-group w5">
													<img src='<%: Constants.PATH_ROOT_FRONT_PC + Constants.IMG_FRONT_PRODUCT_ICON3 %>' class="w4em">
												</div>
												<div class="form-element-group w10">
													<div class="form-element-group-content">
														<label><%: ValueText.GetValueText(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_ICON_FLG3, this.ProductInput.IconFlg3) %></label>
													</div>
												</div>
												<div class="form-element-group w85" style="flex-flow: wrap;" runat="server" visible="<%# this.ProductInput.IconFlg3 == Constants.FLG_PRODUCT_ICON_ON %>">
													<div class="form-element-group-title">
														有効期限
													</div>
													<div class="form-element-group-content" style="max-width:22rem;">
														<label><%: DateTimeUtility.ToStringForManager(this.ProductInput.IconTermEnd3, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></label>
													</div>
												</div>
											</div>
										</div>
										<% } %>
										<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ICON_FLG4)) { %>
										<div class="form-element-group-list-item">
											<div class="form-element-group-list-item-title break-text-hover" style="min-width: 150px;">
												アイコン４
												<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_ICON_FLG4)) { %>
												<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_ICON_FLG4) %></p>
												<% } %>
											</div>
											<div class="form-element-group-list-item-content product-validate-form-element-group-container">
												<div class="form-element-group w5">
													<img src='<%: Constants.PATH_ROOT_FRONT_PC + Constants.IMG_FRONT_PRODUCT_ICON4 %>' class="w4em">
												</div>
												<div class="form-element-group w10">
													<div class="form-element-group-content">
														<label><%: ValueText.GetValueText(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_ICON_FLG4, this.ProductInput.IconFlg4) %></label>
													</div>
												</div>
												<div class="form-element-group w85" style="flex-flow: wrap;" runat="server" visible="<%# this.ProductInput.IconFlg4 == Constants.FLG_PRODUCT_ICON_ON %>">
													<div class="form-element-group-title">
														有効期限
													</div>
													<div class="form-element-group-content" style="max-width:22rem;">
														<label><%: DateTimeUtility.ToStringForManager(this.ProductInput.IconTermEnd4, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></label>
													</div>
												</div>
											</div>
										</div>
										<% } %>
										<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ICON_FLG5)) { %>
										<div class="form-element-group-list-item">
											<div class="form-element-group-list-item-title break-text-hover" style="min-width: 150px;">
												アイコン５
												<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_ICON_FLG5)) { %>
												<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_ICON_FLG5) %></p>
												<% } %>
											</div>
											<div class="form-element-group-list-item-content product-validate-form-element-group-container">
												<div class="form-element-group w5">
													<img src='<%: Constants.PATH_ROOT_FRONT_PC + Constants.IMG_FRONT_PRODUCT_ICON5 %>' class="w4em">
												</div>
												<div class="form-element-group w10">
													<div class="form-element-group-content">
														<label><%: ValueText.GetValueText(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_ICON_FLG5, this.ProductInput.IconFlg5) %></label>
													</div>
												</div>
												<div class="form-element-group w85" style="flex-flow: wrap;" runat="server" visible="<%# this.ProductInput.IconFlg5 == Constants.FLG_PRODUCT_ICON_ON %>">
													<div class="form-element-group-title">
														有効期限
													</div>
													<div class="form-element-group-content" style="max-width:22rem;">
														<label><%: DateTimeUtility.ToStringForManager(this.ProductInput.IconTermEnd5, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></label>
													</div>
												</div>
											</div>
										</div>
										<% } %>
										<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ICON_FLG6)) { %>
										<div class="form-element-group-list-item">
											<div class="form-element-group-list-item-title break-text-hover" style="min-width: 150px;">
												アイコン６
												<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_ICON_FLG6)) { %>
												<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_ICON_FLG6) %></p>
												<% } %>
											</div>
											<div class="form-element-group-list-item-content product-validate-form-element-group-container">
												<div class="form-element-group w5">
													<img src='<%: Constants.PATH_ROOT_FRONT_PC + Constants.IMG_FRONT_PRODUCT_ICON6 %>' class="w4em">
												</div>
												<div class="form-element-group w10">
													<div class="form-element-group-content">
														<label><%: ValueText.GetValueText(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_ICON_FLG6, this.ProductInput.IconFlg6) %></label>
													</div>
												</div>
												<div class="form-element-group w85" style="flex-flow: wrap;" runat="server" visible="<%# this.ProductInput.IconFlg6 == Constants.FLG_PRODUCT_ICON_ON %>">
													<div class="form-element-group-title">
														有効期限
													</div>
													<div class="form-element-group-content" style="max-width:22rem;">
														<label><%: DateTimeUtility.ToStringForManager(this.ProductInput.IconTermEnd6, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></label>
													</div>
												</div>
											</div>
										</div>
										<% } %>
										<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ICON_FLG7)) { %>
										<div class="form-element-group-list-item">
											<div class="form-element-group-list-item-title break-text-hover" style="min-width: 150px;">
												アイコン７
												<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_ICON_FLG7)) { %>
												<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_ICON_FLG7) %></p>
												<% } %>
											</div>
											<div class="form-element-group-list-item-content product-validate-form-element-group-container">
												<div class="form-element-group w5">
													<img src='<%: Constants.PATH_ROOT_FRONT_PC + Constants.IMG_FRONT_PRODUCT_ICON7 %>' class="w4em">
												</div>
												<div class="form-element-group w10">
													<div class="form-element-group-content">
														<label><%: ValueText.GetValueText(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_ICON_FLG7, this.ProductInput.IconFlg7) %></label>
													</div>
												</div>
												<div class="form-element-group w85" style="flex-flow: wrap;" runat="server" visible="<%# this.ProductInput.IconFlg7 == Constants.FLG_PRODUCT_ICON_ON %>">
													<div class="form-element-group-title">
														有効期限
													</div>
													<div class="form-element-group-content" style="max-width:22rem;">
														<label><%: DateTimeUtility.ToStringForManager(this.ProductInput.IconTermEnd7, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></label>
													</div>
												</div>
											</div>
										</div>
										<% } %>
										<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ICON_FLG8)) { %>
										<div class="form-element-group-list-item">
											<div class="form-element-group-list-item-title break-text-hover" style="min-width: 150px;">
												アイコン８
												<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_ICON_FLG8)) { %>
												<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_ICON_FLG8) %></p>
												<% } %>
											</div>
											<div class="form-element-group-list-item-content product-validate-form-element-group-container">
												<div class="form-element-group w5">
													<img src='<%: Constants.PATH_ROOT_FRONT_PC + Constants.IMG_FRONT_PRODUCT_ICON8 %>' class="w4em">
												</div>
												<div class="form-element-group w10">
													<div class="form-element-group-content">
														<label><%: ValueText.GetValueText(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_ICON_FLG8, this.ProductInput.IconFlg8) %></label>
													</div>
												</div>
												<div class="form-element-group w85" style="flex-flow: wrap;" runat="server" visible="<%# this.ProductInput.IconFlg8 == Constants.FLG_PRODUCT_ICON_ON %>">
													<div class="form-element-group-title">
														有効期限
													</div>
													<div class="form-element-group-content" style="max-width:22rem;">
														<label><%: DateTimeUtility.ToStringForManager(this.ProductInput.IconTermEnd8, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></label>
													</div>
												</div>
											</div>
										</div>
										<% } %>
										<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ICON_FLG9)) { %>
										<div class="form-element-group-list-item">
											<div class="form-element-group-list-item-title break-text-hover" style="min-width: 150px;">
												アイコン９
												<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_ICON_FLG9)) { %>
												<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_ICON_FLG9) %></p>
												<% } %>
											</div>
											<div class="form-element-group-list-item-content product-validate-form-element-group-container">
												<div class="form-element-group w5">
													<img src='<%: Constants.PATH_ROOT_FRONT_PC + Constants.IMG_FRONT_PRODUCT_ICON9 %>' class="w4em">
												</div>
												<div class="form-element-group w10">
													<div class="form-element-group-content">
														<label><%: ValueText.GetValueText(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_ICON_FLG9, this.ProductInput.IconFlg9) %></label>
													</div>
												</div>
												<div class="form-element-group w85" style="flex-flow: wrap;" runat="server" visible="<%# this.ProductInput.IconFlg9 == Constants.FLG_PRODUCT_ICON_ON %>">
													<div class="form-element-group-title">
														有効期限
													</div>
													<div class="form-element-group-content" style="max-width:22rem;">
														<label><%: DateTimeUtility.ToStringForManager(this.ProductInput.IconTermEnd9, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></label>
													</div>
												</div>
											</div>
										</div>
										<% } %>
										<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ICON_FLG10)) { %>
										<div class="form-element-group-list-item">
											<div class="form-element-group-list-item-title break-text-hover" style="min-width: 150px;">
												アイコン１0
												<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_ICON_FLG10)) { %>
												<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_ICON_FLG10) %></p>
												<% } %>
											</div>
											<div class="form-element-group-list-item-content product-validate-form-element-group-container">
												<div class="form-element-group w5">
													<img src='<%: Constants.PATH_ROOT_FRONT_PC + Constants.IMG_FRONT_PRODUCT_ICON10 %>' class="w4em">
												</div>
												<div class="form-element-group w10">
													<div class="form-element-group-content">
														<label><%: ValueText.GetValueText(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_ICON_FLG10, this.ProductInput.IconFlg10) %></label>
													</div>
												</div>
												<div class="form-element-group w85" style="flex-flow: wrap;" runat="server" visible="<%# this.ProductInput.IconFlg10 == Constants.FLG_PRODUCT_ICON_ON %>">
													<div class="form-element-group-title">
														有効期限
													</div>
													<div class="form-element-group-content" style="max-width:22rem;">
														<label><%: DateTimeUtility.ToStringForManager(this.ProductInput.IconTermEnd10, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></label>
													</div>
												</div>
											</div>
										</div>
										<% } %>
									</div>
								</div>
							</div>
						</div>
					</div>
					<% } %>
				</div>
			</div>
		</div>
		<div class="block-section-row">
			<div runat="server" class="block-section block-section-product-register-category" visible="<%# (Constants.PRODUCT_CTEGORY_OPTION_ENABLE && GetDisplayCategoryArea()) %>">
				<div class="block-section-header">
					<div class="block-section-header-icon"><span class="icon-category"></span></div>
					<h2 class="block-section-header-txt break-text-hover">
						カテゴリ設定
					</h2>
				</div>
				<div class="block-section-body">
					<div class="block-section-body-inner-row">
						<div class="block-section-body-inner-col" runat="server" visible="<%# this.HasProductCategory %>">
							<div class="form-element-group form-element-group-vertical">
								<div class="custom-category-selector-input">
									<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CATEGORY_ID1)) { %>
									<div class="custom-category-selector-input-selected-items">
										<div class="form-element-group-title w10em break-text-hover">カテゴリ1
										<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_CATEGORY_ID1)) { %>
											<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_CATEGORY_ID1) %></p>
										<% } %>
										</div>
										<div class="custom-category-selector-category">
											<div class="custom-category-selector-input-selected-item" runat="server" visible="<%# (string.IsNullOrEmpty(this.ProductInput.CategoryId1) == false) %>">
												<span class="custom-category-selector-input-selected-item-label">
													<span class="custom-category-selector-input-selected-item-label-name"><%: this.ProductInput.ProductCategoryName1 %></span>
													<span class="custom-category-selector-input-selected-item-label-id">（<%: this.ProductInput.CategoryId1 %>）</span>
												</span>
											</div>
											<div class="empty-value" runat="server" visible="<%# string.IsNullOrEmpty(this.ProductInput.CategoryId1) %>">
												<label><%: GetProductValueToDisplay(this.ProductInput.CategoryId1) %></label>
											</div>
										</div>
									</div>
									<% } %>
									<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CATEGORY_ID2)) { %>
									<div class="custom-category-selector-input-selected-items">
										<div class="form-element-group-title w10em break-text-hover">カテゴリ2
										<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_CATEGORY_ID2)) { %>
											<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_CATEGORY_ID2) %></p>
										<% } %>
										</div>
										<div class="custom-category-selector-category">
											<div class="custom-category-selector-input-selected-item" runat="server" visible="<%# (string.IsNullOrEmpty(this.ProductInput.CategoryId2) == false) %>">
												<span class="custom-category-selector-input-selected-item-label">
													<span class="custom-category-selector-input-selected-item-label-name"><%: this.ProductInput.ProductCategoryName2 %></span>
													<span class="custom-category-selector-input-selected-item-label-id">（<%: this.ProductInput.CategoryId2 %>）</span>
												</span>
											</div>
											<div class="empty-value" runat="server" visible="<%# string.IsNullOrEmpty(this.ProductInput.CategoryId2) %>">
												<label><%: GetProductValueToDisplay(this.ProductInput.CategoryId2) %></label>
											</div>
										</div>
									</div>
									<% } %>
									<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CATEGORY_ID3)) { %>
									<div class="custom-category-selector-input-selected-items">
										<div class="form-element-group-title w10em break-text-hover">カテゴリ3
										<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_CATEGORY_ID3)) { %>
											<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_CATEGORY_ID3) %></p>
										<% } %>
										</div>
										<div class="custom-category-selector-category">
											<div class="custom-category-selector-input-selected-item" runat="server" visible="<%# (string.IsNullOrEmpty(this.ProductInput.CategoryId3) == false) %>">
												<span class="custom-category-selector-input-selected-item-label">
													<span class="custom-category-selector-input-selected-item-label-name"><%: this.ProductInput.ProductCategoryName3 %></span>
													<span class="custom-category-selector-input-selected-item-label-id">（<%: this.ProductInput.CategoryId3 %>）</span>
												</span>
											</div>
											<div class="empty-value" runat="server" visible="<%# string.IsNullOrEmpty(this.ProductInput.CategoryId3) %>">
												<label><%: GetProductValueToDisplay(this.ProductInput.CategoryId3) %></label>
											</div>
										</div>
									</div>
									<% } %>
									<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CATEGORY_ID4)) { %>
									<div class="custom-category-selector-input-selected-items">
										<div class="form-element-group-title w10em break-text-hover">カテゴリ4
										<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_CATEGORY_ID4)) { %>
											<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_CATEGORY_ID4) %></p>
										<% } %>
										</div>
										<div class="custom-category-selector-category">
											<div class="custom-category-selector-input-selected-item" runat="server" visible="<%# (string.IsNullOrEmpty(this.ProductInput.CategoryId4) == false) %>">
												<span class="custom-category-selector-input-selected-item-label">
													<span class="custom-category-selector-input-selected-item-label-name"><%: this.ProductInput.ProductCategoryName4 %></span>
													<span class="custom-category-selector-input-selected-item-label-id">（<%: this.ProductInput.CategoryId4 %>）</span>
												</span>
											</div>
											<div class="empty-value" runat="server" visible="<%# string.IsNullOrEmpty(this.ProductInput.CategoryId4) %>">
												<label><%: GetProductValueToDisplay(this.ProductInput.CategoryId4) %></label>
											</div>
										</div>
									</div>
									<% } %>
									<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CATEGORY_ID5)) { %>
									<div class="custom-category-selector-input-selected-items">
										<div class="form-element-group-title w10em break-text-hover">カテゴリ5
										<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_CATEGORY_ID5)) { %>
											<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_CATEGORY_ID5) %></p>
										<% } %>
										</div>
										<div class="custom-category-selector-category">
											<div class="custom-category-selector-input-selected-item" runat="server" visible="<%# (string.IsNullOrEmpty(this.ProductInput.CategoryId5) == false) %>">
												<span class="custom-category-selector-input-selected-item-label">
													<span class="custom-category-selector-input-selected-item-label-name"><%: this.ProductInput.ProductCategoryName5 %></span>
													<span class="custom-category-selector-input-selected-item-label-id">（<%: this.ProductInput.CategoryId5 %>）</span>
												</span>
											</div>
											<div class="empty-value" runat="server" visible="<%# string.IsNullOrEmpty(this.ProductInput.CategoryId5) %>">
												<label><%: GetProductValueToDisplay(this.ProductInput.CategoryId5) %></label>
											</div>
										</div>
									</div>
									<% } %>
								</div>
							</div>
						</div>
						<div class="block-section-body-inner-col" runat="server" visible="<%# (this.HasProductCategory == false) %>">
							<div class="block-section-body-setting-guide">
								<p class="block-section-body-setting-guide-text">商品カテゴリが登録されていません。</p>
							</div>
						</div>
					</div>
				</div>
			</div>
			<% if (Constants.PRODUCT_BRAND_ENABLED) { %>
			<div class='block-section block-section-product-register-brand'>
				<div class="block-section-header">
					<div class="block-section-header-icon"><span class="icon-price-tags"></span></div>
					<h2 class="block-section-header-txt break-text-hover">
						ブランド設定
					</h2>
				</div>
				<div class="block-section-body">
					<div class="block-section-body-inner-row">
						<div class="block-section-body-inner-col" runat="server" visible="<%# this.HasProductBrand %>">
							<div class="form-element-group form-element-group-vertical">
								<div class="custom-category-selector-input">
									<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_BRAND_ID1) || (GetDisplayBrandArea() == false)) { %>
									<div class="custom-category-selector-input-selected-items">
										<div class="form-element-group-title w10em break-text-hover">ブランド1
										<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_BRAND_ID1)) { %>
											<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_BRAND_ID1) %></p>
										<% } %>
										</div>
										<div class="custom-category-selector-brand">
											<div id="Div1" class="custom-category-selector-input-selected-item" runat="server" visible="<%# (string.IsNullOrEmpty(this.ProductInput.BrandId1) == false) %>">
												<span class="custom-category-selector-input-selected-item-label">
													<span class="custom-category-selector-input-selected-item-label-name"><%: this.ProductInput.ProductBrandName1 %></span>
													<span class="custom-category-selector-input-selected-item-label-id">（<%: this.ProductInput.BrandId1 %>）</span>
												</span>
											</div>
											<div id="Div2" class="empty-value" runat="server" visible="<%# string.IsNullOrEmpty(this.ProductInput.BrandId1) %>">
												<label><%: GetProductValueToDisplay(this.ProductInput.BrandId1) %></label>
											</div>
										</div>
									</div>
									<% } %>
									<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_BRAND_ID2)) { %>
									<div class="custom-category-selector-input-selected-items">
										<div class="form-element-group-title w10em break-text-hover">ブランド2
										<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_BRAND_ID2)) { %>
											<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_BRAND_ID2) %></p>
										<% } %>
										</div>
										<div class="custom-category-selector-brand">
											<div id="Div3" class="custom-category-selector-input-selected-item" runat="server" visible="<%# (string.IsNullOrEmpty(this.ProductInput.BrandId2) == false) %>">
												<span class="custom-category-selector-input-selected-item-label">
													<span class="custom-category-selector-input-selected-item-label-name"><%: this.ProductInput.ProductBrandName2 %></span>
													<span class="custom-category-selector-input-selected-item-label-id">（<%: this.ProductInput.BrandId2 %>）</span>
												</span>
											</div>
											<div id="Div4" class="empty-value" runat="server" visible="<%# string.IsNullOrEmpty(this.ProductInput.BrandId2) %>">
												<label><%: GetProductValueToDisplay(this.ProductInput.BrandId2) %></label>
											</div>
										</div>
									</div>
									<% } %>
									<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_BRAND_ID3)) { %>
									<div class="custom-category-selector-input-selected-items">
										<div class="form-element-group-title w10em break-text-hover">ブランド3
										<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_BRAND_ID3)) { %>
											<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_BRAND_ID3) %></p>
										<% } %>
										</div>
										<div class="custom-category-selector-brand">
											<div id="Div5" class="custom-category-selector-input-selected-item" runat="server" visible="<%# (string.IsNullOrEmpty(this.ProductInput.BrandId3) == false) %>">
												<span class="custom-category-selector-input-selected-item-label">
													<span class="custom-category-selector-input-selected-item-label-name"><%: this.ProductInput.ProductBrandName3 %></span>
													<span class="custom-category-selector-input-selected-item-label-id">（<%: this.ProductInput.BrandId3 %>）</span>
												</span>
											</div>
											<div id="Div6" class="empty-value" runat="server" visible="<%# string.IsNullOrEmpty(this.ProductInput.BrandId3) %>">
												<label><%: GetProductValueToDisplay(this.ProductInput.BrandId3) %></label>
											</div>
										</div>
									</div>
									<% } %>
									<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_BRAND_ID4)) { %>
									<div class="custom-category-selector-input-selected-items">
										<div class="form-element-group-title w10em break-text-hover">ブランド4
										<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_BRAND_ID4)) { %>
											<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_BRAND_ID4) %></p>
										<% } %>
										</div>
										<div class="custom-category-selector-brand">
											<div id="Div7" class="custom-category-selector-input-selected-item" runat="server" visible="<%# (string.IsNullOrEmpty(this.ProductInput.BrandId4) == false) %>">
												<span class="custom-category-selector-input-selected-item-label">
													<span class="custom-category-selector-input-selected-item-label-name"><%: this.ProductInput.ProductBrandName4 %></span>
													<span class="custom-category-selector-input-selected-item-label-id">（<%: this.ProductInput.BrandId4 %>）</span>
												</span>
											</div>
											<div id="Div8" class="empty-value" runat="server" visible="<%# string.IsNullOrEmpty(this.ProductInput.BrandId4) %>">
												<label><%: GetProductValueToDisplay(this.ProductInput.BrandId4) %></label>
											</div>
										</div>
									</div>
									<% } %>
									<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_BRAND_ID5)) { %>
									<div class="custom-category-selector-input-selected-items">
										<div class="form-element-group-title w10em break-text-hover">ブランド5
										<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_BRAND_ID5)) { %>
											<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_BRAND_ID5) %></p>
										<% } %>
										</div>
										<div class="custom-category-selector-brand">
											<div id="Div9" class="custom-category-selector-input-selected-item" runat="server" visible="<%# (string.IsNullOrEmpty(this.ProductInput.BrandId5) == false) %>">
												<span class="custom-category-selector-input-selected-item-label">
													<span class="custom-category-selector-input-selected-item-label-name"><%: this.ProductInput.ProductBrandName5 %></span>
													<span class="custom-category-selector-input-selected-item-label-id">（<%: this.ProductInput.BrandId5 %>）</span>
												</span>
											</div>
											<div id="Div10" class="empty-value" runat="server" visible="<%# string.IsNullOrEmpty(this.ProductInput.BrandId5) %>">
												<label><%: GetProductValueToDisplay(this.ProductInput.BrandId5) %></label>
											</div>
										</div>
									</div>
									<% } %>
								</div>
							</div>
						</div>
						<div class="block-section-body-inner-col" runat="server" visible="<%# (this.HasProductBrand == false) %>">
							<div class="block-section-body-setting-guide">
								<p class="block-section-body-setting-guide-text">商品ブランドが登録されていません。</p>
							</div>
						</div>
					</div>
				</div>
			</div>
			<% } %>
			<% if (GetDisplayUsArea() || GetDisplayCsArea()) { %>
			<div class="block-section block-section-product-register-related-product ">
				<div class="block-section-header">
					<div class="block-section-header-icon"><span class="icon-box"></span></div>
					<h2 class="block-section-header-txt">関連商品</h2>
				</div>
				<div class="block-section-body">
					<div class="block-section-body-inner-row">
						<% if (GetDisplayCsArea()) { %>
						<div class="block-section-body-inner-col" style="margin-bottom:10px">
							<div class="form-element-group form-element-group-vertical">
								<div class="form-element-group-title break-text-hover">
									<label>クロスセル商品</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS1)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS1) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content">
									<div class="custom-product-selector-input" runat="server" visible="<%# (this.HasRelatedCsProduct && (this.RelatedCsProducts.Length != 0)) %>">
										<div class="custom-product-selector-input-selected-items">
											<asp:Repeater runat="server" DataSource="<%# this.RelatedCsProducts %>" ItemType ="w2.Domain.Product.ProductModel">
												<ItemTemplate>
													<div class="custom-product-selector-input-selected-item">
														<div class="custom-product-selector-input-selected-item-img">
															<%# ProductImage.GetHtmlImageTag(Item, ProductType.Product, SiteType.Pc, Constants.PRODUCTIMAGE_FOOTER_S) %>
														</div>
														<div class="custom-product-selector-input-selected-item-text">
															<span class="custom-product-selector-input-selected-item-label">
																<%#: Item.Name %>
															</span>
															<span class="custom-product-selector-input-selected-item-id">（<%#: Item.ProductId %>）</span>
														</div>
													</div>
												</ItemTemplate>
											</asp:Repeater>
										</div>
									</div>
									<div class="block-section-body-setting-guide" runat="server" visible="<%# (this.HasRelatedCsProduct && (this.RelatedCsProducts.Length == 0)) %>">
										<p class="block-section-body-setting-guide-text">クロスセル商品が見つかりませんでした。</p>
									</div>
									<div class="block-section-body-setting-guide" runat="server" visible="<%# (this.HasRelatedCsProduct == false) %>">
										<p class="block-section-body-setting-guide-text">クロスセル商品が登録されていません。</p>
									</div>
								</div>
							</div>
						</div>
						<% } %>
						<% if (GetDisplayUsArea()) { %>
						<div class="block-section-body-inner-col">
							<div class="form-element-group form-element-group-vertical">
								<div class="form-element-group-title break-text-hover">
									<label>アップセル商品</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US1)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US1) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content">
									<div class="custom-product-selector-input" runat="server" visible="<%# (this.HasRelatedUsProduct && (this.RelatedUsProducts.Length != 0)) %>">
										<div class="custom-product-selector-input-selected-items">
											<asp:Repeater runat="server" DataSource="<%# this.RelatedUsProducts %>" ItemType ="w2.Domain.Product.ProductModel">
												<ItemTemplate>
													<div class="custom-product-selector-input-selected-item">
														<div class="custom-product-selector-input-selected-item-img">
															<%# ProductImage.GetHtmlImageTag(Item, ProductType.Product, SiteType.Pc, Constants.PRODUCTIMAGE_FOOTER_S) %>
														</div>
														<div class="custom-product-selector-input-selected-item-text">
															<span class="custom-product-selector-input-selected-item-label">
																<%#: Item.Name %>
															</span>
															<span class="custom-product-selector-input-selected-item-id">（<%#: Item.ProductId %>）</span>
														</div>
													</div>
												</ItemTemplate>
											</asp:Repeater>
										</div>
									</div>
									<div class="block-section-body-setting-guide" runat="server" visible="<%# (this.HasRelatedUsProduct && (this.RelatedUsProducts.Length == 0)) %>">
										<p class="block-section-body-setting-guide-text">アップセル商品が見つかりませんでした。</p>
									</div>
									<div class="block-section-body-setting-guide" runat="server" visible="<%# (this.HasRelatedUsProduct == false) %>">
										<p class="block-section-body-setting-guide-text">アップセル商品が登録されていません。</p>
									</div>
								</div>
							</div>
						</div>
						<% } %>
					</div>
				</div>
			</div>
			<% } %>
			<div class="block-section block-section-product-register-price">
				<div class="block-section-header">
					<div class="block-section-header-icon"><span class="icon-coupon"></span></div>
					<h2 class="block-section-header-txt">価格設定</h2>
				</div>
				<div class="block-section-body">
					<div class="block-section-body-inner-row">
						<div class="block-section-body-inner-col">
							<div id="divTaxCategory" class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title w10em break-text-hover">
									<label>商品税率カテゴリ</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_TAX_CATEGORY_ID)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_TAX_CATEGORY_ID) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content break-text-hover" style="margin-top: 0.2em;">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.ProductTaxCategoryName) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.ProductTaxCategoryName) %>
									</label>
								</div>
							</div>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_TAX_INCLUDED_FLG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title w10em break-text-hover">
									<label>税区分</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_TAX_INCLUDED_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_TAX_INCLUDED_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content" style="margin-top: 0.2em;">
									<%: GetProductValueToDisplay(TaxCalculationUtility.GetPrescribedOrderItemTaxIncludedFlag(), Constants.FIELD_PRODUCT_TAX_INCLUDED_FLG) %>
								</div>
							</div>
							<% } %>
							<% if (Constants.MEMBER_RANK_OPTION_ENABLED && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_MEMBER_RANK_DISCOUNT_FLG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid ">
								<div class="form-element-group-title w10em break-text-hover">
									<label>会員ランク割引対象</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_MEMBER_RANK_DISCOUNT_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_MEMBER_RANK_DISCOUNT_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm">
									<label><%: GetProductValueToDisplay(this.ProductInput.MemberRankDiscountFlg, Constants.FIELD_PRODUCT_MEMBER_RANK_DISCOUNT_FLG) %></label>
								</div>
							</div>
							<% } %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title w10em break-text-hover">
									<label>商品表示価格</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_PRICE)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_PRICE) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.DisplayPrice) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.DisplayPrice.ToPriceString(true)) %>
									</label>
								</div>
							</div>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title w10em break-text-hover">
									<label>商品表示特別価格</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.DisplaySpecialPrice) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.DisplaySpecialPrice.ToPriceString(true)) %>
									</label>
								</div>
							</div>
							<% } %>
							<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title w10em break-text-hover">
									<label>定期購入初回価格</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.FixedPurchaseFirsttimePrice) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.FixedPurchaseFirsttimePrice.ToPriceString(true)) %>
									</label>
								</div>
							</div>
							<% } %>
							<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title w10em break-text-hover">
									<label>定期購入価格</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.FixedPurchasePrice) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.FixedPurchasePrice.ToPriceString(true)) %>
									</label>
								</div>
							</div>
							<% } %>
							<% if (Constants.MEMBER_RANK_OPTION_ENABLED && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE)) { %>
							<div class="form-element-group-heading">
								<div class="form-element-group-heading-label">
									<h3 style="display: inline-block;">会員ランク価格</h3>
								</div>
							</div>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title w10em break-text-hover">
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE) %></p>
									<% } %>
								</div>
							</div>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-horizontal-grid" style="flex-wrap: wrap;">
									<asp:Repeater ID="rMemberRankPrice" DataSource="<%# GetMemberRankPricesForDatabind() %>" runat="server">
										<ItemTemplate>
											<div class="form-element-group" style="width: 15em; display: flex;">
												<div class="form-element-group-title member-rank-price-item break-text-hover">
													<label><%#: ((Hashtable)Container.DataItem)[Constants.FIELD_MEMBERRANK_MEMBER_RANK_NAME] %></label>
												</div>
												<div class="form-element-group-content form-element-group-content-item-inline member-rank-price-item" style="padding-top: 5px;">
													<label class='<%# string.IsNullOrEmpty(StringUtility.ToEmpty(((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE])) ? "empty-value" : string.Empty %>'>
														<%#: DisplayProductMemberPrice(((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE]) %>
													</label>
												</div>
											</div>
										</ItemTemplate>
									</asp:Repeater>
								</div>
							</div>
							<% } %>
						</div>
					</div>
				</div>
			</div>
			<% if (Constants.W2MP_POINT_OPTION_ENABLED && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_POINT1)) { %>
			<div class="block-section block-section-product-register-point">
				<div class="block-section-header">
					<div class="block-section-header-icon"><span class="icon-point"></span></div>
					<h2 class="block-section-header-txt">ポイント設定</h2>
				</div>
				<div class="block-section-body">
					<div class="block-section-body-inner-row">
						<div class="block-section-body-inner-col">
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title w10em break-text-hover">
									<label>ポイント</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_POINT1)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_POINT1) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-register-point">
									<div class="form-element-group-content-item">
										<label>
											<span class="product-register-point-label-1">購入時商品ごとに</span>
											<label id="lbPoint1" style="vertical-align:baseline"><%: this.ProductInput.Point1 %></label>
											<span class="product-register-point-label-2"><%: GetPointKbn(this.ProductInput.PointKbn1) %></span>
											<span class="product-register-point-label-3">発行する</span>
										</label>
									</div>
									<% if (this.ProductInput.MemberRankPointExcludeFlg == Constants.FLG_PRODUCT_CHECK_MEMBER_RANK_POINT_EXCLUDE_FLG_VALID) { %>
									<div class="form-element-group-content-item">
										<br />
										<label>
											<span class="product-register-point-label-1">会員ランクのポイント加算を適用しない</span>
										</label>
									</div>
									<% } %>
									<% if (string.IsNullOrEmpty(GetPointKbn(this.ProductInput.PointKbn2)) == false) { %>
									<div class="form-element-group-heading">
										<h3 class="form-element-group-heading-label">定期購入</h3>
									</div>
									<div class="form-element-group-content-item sample-checkbox-toggle-area-3">
										<label>
											<span class="product-register-point-label-1">購入時商品ごとに</span>
											<label id="lbPoint2" style="vertical-align:baseline"><%: this.ProductInput.Point2 %></label>
											<span class="product-register-point-label-2"><%: GetPointKbn(this.ProductInput.PointKbn2) %></span>
											<span class="product-register-point-label-3">発行する</span>
										</label>
									</div>
									<% } %>
								</div>
							</div>
						</div>
					</div>
				</div>
			</div>
			<% } %>
			<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_SHIPPING_TYPE)) { %>
			<div class="block-section block-section-product-register-delivery">
				<div id="comProductShippingTitle" class="block-section-header">
					<div class="block-section-header-icon"><span class="icon-shipping"></span></div>
					<h2 class="block-section-header-txt">配送設定</h2>
				</div>
				<div id="comProductShippingContent" class="block-section-body">
					<div id="divShippingType" class="block-section-body-inner-row">
						<div class="block-section-body-inner-col">
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title w10em break-text-hover">
									<label>配送種別</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_SHIPPING_TYPE)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_SHIPPING_TYPE) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content" style="margin-top: 5px;">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.ShopShippingName) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.ShopShippingName) %>
									</label>
								</div>
							</div>
						</div>
						<div class="block-section-body-inner-col">
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title w10em break-text-hover">
									<label>配送サイズ区分</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_SHIPPING_SIZE_KBN)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_SHIPPING_SIZE_KBN) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm">
									<label><%: GetProductValueToDisplay(this.ProductInput.ShippingSizeKbn, Constants.FIELD_PRODUCT_SHIPPING_SIZE_KBN) %></label>
								</div>
							</div>
						</div>
						<div runat="server" class="product-size-factor-toggle-content" visible="<%# this.ProductInput.ShippingSizeKbn == Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_MAIL %>">
							<div class="block-section-body-inner-col">
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title w10em break-text-hover">
										<label>商品サイズ係数</label>
										<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_PRODUCT_SIZE_FACTOR)) { %>
										<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_PRODUCT_SIZE_FACTOR) %></p>
										<% } %>
									</div>
									<div class="form-element-group-content" style="margin-top: 5px;">
										<label class='<%# string.IsNullOrEmpty(this.ProductInput.ProductSizeFactor) ? "empty-value" : string.Empty %>'>
											<%: GetProductValueToDisplay(StringUtility.ToNumeric(this.ProductInput.ProductSizeFactor)) %>
										</label>
									</div>
								</div>
							</div>
						</div>
						<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
						<div class="block-section-body-inner-col">
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title w10em break-text-hover">
									<label>商品重量（g）</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_PRODUCT_WEIGHT_GRAM)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_PRODUCT_WEIGHT_GRAM) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content" style="margin-top: 5px;">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.ProductWeightGram) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.ProductWeightGram) %>
									</label>
								</div>
							</div>
						</div>
						<% } %>
						<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG)) { %>
						<div class="block-section-body-inner-col">
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title w10em break-text-hover">
									<label>配送料複数個無料</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm">
									<label><%: GetProductValueToDisplay(this.ProductInput.PluralShippingPriceFreeFlg, Constants.FIELD_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG) %></label>
								</div>
							</div>
						</div>
						<% } %>
						<% if (Constants.STORE_PICKUP_OPTION_ENABLED
							&& GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_STOREPICKUP_FLG)) { %>
						<div class="block-section-body-inner-col">
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title w10em break-text-hover">
									<label>店舗受取可能フラグ</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_STOREPICKUP_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_STOREPICKUP_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm">
									<label><%: GetProductValueToDisplay(this.ProductInput.StorePickupFlg, Constants.FIELD_PRODUCT_STOREPICKUP_FLG) %></label>
								</div>
							</div>
						</div>
						<% } %>
						<% if (Constants.FREE_SHIPPING_FEE_OPTION_ENABLED
							&& GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG)) { %>
						<div class="block-section-body-inner-col">
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title w10em break-text-hover">
									<label>配送料無料適用外</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm">
									<label><%: GetProductValueToDisplay(this.ProductInput.ExcludeFreeShippingFlg, Constants.FIELD_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG) %></label>
								</div>
							</div>
						</div>
						<% } %>
					</div>
				</div>
			</div>
			<% } %>
			<% if (Constants.PRODUCT_TAG_OPTION_ENABLE) { %>
			<div class="block-section block-section-product-register-tags ">
				<div class="block-section-header">
					<div class="block-section-header-icon"><span class="icon-box"></span></div>
					<h2 class="block-section-header-txt">商品タグ</h2>
				</div>
				<div class="block-section-body">
					<div class="block-section-body-inner-row">
						<div class="block-section-body-inner-col">
							<asp:Repeater ID="rTag" DataSource="<%# this.ProductTags %>" runat="server">
								<ItemTemplate>
									<div class="form-element-group form-element-group-horizontal-grid" style='<%# (StringUtility.ToEmpty(((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCTTAGSETTING_TAG_VALID_FLG]) == Constants.FLG_PRODUCTTAGSETTING_VALID_FLG_VALID) ? "" : "display:none;" %>'>
										<div class="form-element-group-title break-text-hover">
											<label>
												<%#: ((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCTTAGSETTING_TAG_NAME] %>
												（<%#: ((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID] %>）
											</label>
										</div>
										<div class="form-element-group-content break-text-hover" style="padding-top: 3px;">
											<label class='<%# string.IsNullOrEmpty(StringUtility.ToEmpty(((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID + "_value"])) ? "empty-value" : string.Empty %>'>
												<%#: GetProductValueToDisplay(StringUtility.ToEmpty(((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID + "_value"])) %>
											</label>
										</div>
									</div>
								</ItemTemplate>
							</asp:Repeater>
						</div>
					</div>
				</div>
			</div>
			<% } %>
			<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_SEO_KEYWORDS)) { %>
			<div class="block-section block-section-product-register-seo">
				<div class="block-section-header">
					<div class="block-section-header-icon"><span class="icon-analysis"></span></div>
					<h2 class="block-section-header-txt">SEO設定</h2>
				</div>
				<div class="block-section-body">
					<div class="block-section-body-inner-row">
						<div class="block-section-body-inner-col">
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title w10em break-text-hover">
									<label>SEOキーワード設定</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_SEO_KEYWORDS)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_SEO_KEYWORDS) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content break-text-hover" style="margin-top: 3px;">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.SeoKeywords) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.SeoKeywords) %>
									</label>
								</div>
							</div>
						</div>
					</div>
				</div>
			</div>
			<% } %>
			<% if (Constants.PRODUCT_STOCK_OPTION_ENABLE && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN)) { %>
			<div class="block-section block-section-product-register-stock">
				<div id="comProductStockTitle" class="block-section-header">
					<div class="block-section-header-icon"><span class="icon-stock"></span></div>
					<h2 class="block-section-header-txt">在庫</h2>
				</div>
				<div id="comProductStockContent" class="block-section-body">
					<div class="block-section-body-inner-row">
						<div class="block-section-body-inner-col">
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title w10em break-text-hover">
									<label>在庫管理方法</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content form-element-group-content-confirm">
									<% if(this.ProductInput.IsStockUnmanaged == false) { %>
									<a href="" onclick="javascript:open_window('<%#: CreateProductStockDetailUrl(this.ProductInput.ProductId) %>','productstockcontact','width=1000,height=826,top=110,left=380,status=NO,scrollbars=yes');return false;">
										<%: GetProductValueToDisplay(this.ProductInput.StockManagementKbn, Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN) %>
									</a>
									<% } else { %>
									<%: GetProductValueToDisplay(this.ProductInput.StockManagementKbn, Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN) %>
									<% } %>
								</div>
							</div>
							<div runat="server" class="product-register-stock-management-toggle-content" visible="<%# (this.ProductInput.IsStockUnmanaged == false) %>">
								<div runat="server" class="form-element-group form-element-group-horizontal-grid" visible="<%# this.DisplayProductStock %>">
									<div class="form-element-group-title w10em">
										<label>在庫管理</label>
									</div>
									<div class="form-element-group-content">
										<% if (((this.ActionStatus == Constants.ACTION_STATUS_DETAIL)
												|| (this.ActionStatus == Constants.ACTION_STATUS_COMPLETE))
											&& (this.ProductInput.HasProductVariation == false)) { %>
										<div class="product-register-stock-management">
											<div class="product-register-stock-management-body">
												<div class="product-register-stock-management-block">
													<div class="product-register-stock-management-block-title">
														論理在庫
													</div>
													<div class="product-register-stock-management-block-title">
														<label><%: CreateStockInfo(this.ProductInput.StockManagementKbn, (this.ProductInput.HasProductStock) ? this.ProductInput.ProductStocks[0].Stock : string.Empty) %></label>
													</div>
												</div>
												<div class="product-register-stock-management-block">
													<div class="product-register-stock-management-block-title">
														在庫安全基準
													</div>
													<div class="product-register-stock-management-block-title">
														<label><%: CreateStockInfo(this.ProductInput.StockManagementKbn, (this.ProductInput.HasProductStock) ? this.ProductInput.ProductStocks[0].StockAlert : string.Empty) %></label>
													</div>
												</div>
												<% if (Constants.REALSTOCK_OPTION_ENABLED) { %>
												<div class="product-register-stock-management-block">
													<div class="product-register-stock-management-block-title">
														実在庫
													</div>
													<div class="product-register-stock-management-block-title">
														<label><%: CreateStockInfo(this.ProductInput.StockManagementKbn, (this.ProductInput.HasProductStock) ? this.ProductInput.ProductStocks[0].Realstock : string.Empty) %></label>
													</div>
												</div>
												<div class="product-register-stock-management-block">
													<div class="product-register-stock-management-block-title">
														実在庫B品
													</div>
													<div class="product-register-stock-management-block-title">
														<label><%: CreateStockInfo(this.ProductInput.StockManagementKbn, (this.ProductInput.HasProductStock) ? this.ProductInput.ProductStocks[0].RealstockB : string.Empty) %></label>
													</div>
												</div>
												<div class="product-register-stock-management-block">
													<div class="product-register-stock-management-block-title">
														実在庫C品
													</div>
													<div class="product-register-stock-management-block-title">
														<label><%: CreateStockInfo(this.ProductInput.StockManagementKbn, (this.ProductInput.HasProductStock) ? this.ProductInput.ProductStocks[0].RealstockC : string.Empty) %></label>
													</div>
												</div>
												<% } %>
											</div>
										</div>
										<% } else { %>
										<div class="product-register-stock-management">
											<div class="product-register-stock-management-header">
												<p class="note">バリエーションを登録している場合それらに対しても同在庫数として在庫数が更新されます。</p>
											</div>
											<div class="product-register-stock-management-body">
												<div class="product-register-stock-management-block js-product-register-stock-management-stocknum">
													<div class="product-register-stock-management-block-title">
														在庫数（論理在庫）
													</div>
													<div class="product-register-stock-management-block-content">
														<div class="product-register-stock-management-change">
															<div class="product-register-stock-management-change-title">
																変更数（+/-）
															</div>
															<div class="product-register-stock-management-change-input">
																<div class="count-input" style="border: none;">
																	<span class="product-register-stock-management-after-change-value break-text-hover" style="text-align: center; flex-grow: 1;">
																		<%: CreateStockInfo(this.ProductInput.StockManagementKbn, this.ProductInput.Stock) %>
																	</span>
																</div>
															</div>
														</div>
														<dl class="product-register-stock-management-current">
															<dt>現在</dt>
															<dd class="product-register-stock-management-current-value"><%: CreateStockInfo(this.ProductInput.StockManagementKbn, this.ProductInput.Stock) %></dd>
														</dl>
													</div>
												</div>
												<div class="product-register-stock-management-block js-product-register-stock-management-stockalert">
													<div class="product-register-stock-management-block-title">
														安全基準値（論理在庫）
													</div>
													<div class="product-register-stock-management-block-content">
														<div class="product-register-stock-management-change">
															<div class="product-register-stock-management-change-title">
																変更後
															</div>
															<div class="product-register-stock-management-change-input">
																<div class="count-input" style="border: none;">
																	<span class="product-register-stock-management-after-change-value break-text-hover" style="text-align: center; flex-grow: 1;">
																		<%: CreateStockInfo(this.ProductInput.StockManagementKbn, this.ProductInput.StockAlert) %>
																	</span>
																</div>
															</div>
														</div>
														<dl class="product-register-stock-management-current">
															<dt>現在</dt>
															<dd class="product-register-stock-management-current-value"><%: CreateStockInfo(this.ProductInput.StockManagementKbn, this.ProductInput.StockAlert) %></dd>
														</dl>
													</div>
												</div>
												<div class="product-register-stock-management-block">
													<div class="product-register-stock-management-block-title">
														更新メモ
													</div>
													<div class="product-register-stock-management-block-content break-text-hover">
														<label class='<%# string.IsNullOrEmpty(this.ProductInput.UpdateMemo) ? "empty-value" : string.Empty %>'>
															<%: GetProductValueToDisplay(this.ProductInput.UpdateMemo) %>
														</label>
													</div>
												</div>
											</div>
										</div>
										<% } %>
									</div>
								</div>
							</div>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_STOCK_MESSAGE_ID)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title w10em break-text-hover">
									<label>商品在庫文言</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_STOCK_MESSAGE_ID)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_STOCK_MESSAGE_ID) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content" style="margin-top: 2px;">
									<label class='<%# string.IsNullOrEmpty(this.ProductInput.StockMessageId) ? "empty-value" : string.Empty %>'>
										<%: GetProductValueToDisplay(this.ProductInput.ProductStockMessageName) %>
									</label>
								</div>
							</div>
							<% } %>
						</div>
					</div>
				</div>
			</div>
			<% } %>
			<div class="block-section block-section-product-register-variation">
				<div class="block-section-header">
					<div class="block-section-header-icon"><span class="icon-box"></span></div>
					<h2 class="block-section-header-txt">商品バリエーション</h2>
					<p id="anchorVariation" align="right"></p>
				</div>
				<div class="block-section-body">
					<div class="block-section-body-inner-row">
						<div class="block-section-body-inner-col">
							<div class="product-register-variation">
								<div class="product-register-variation-form">
									<div id="dvNoVariation" runat="server" class="block-section-body" visible="<%# (this.ProductVariations.Length == 0) %>">
										<div class="block-section-body-setting-guide">
											<p class="block-section-body-setting-guide-text">バリエーション管理しない</p>
										</div>
									</div>
									<asp:Repeater runat="server" ID="rVariation" DataSource="<%# this.ProductVariations %>" ItemType="ProductVariationInput">
										<ItemTemplate>
											<div class="product-register-variation-form-row">
												<div class="product-register-variation-form-group1" style="margin-bottom:10px">
													<div class="form-element-group form-element-group-horizontal-grid">
														<div class="form-element-group-title">
															<label>商品バリエーションID</label>
														</div>
														<div class='form-element-group-content break-text-hover <%# string.IsNullOrEmpty(Item.VId) ? "empty-value" : string.Empty %>' style="margin-top: 5px;">
															<%#: GetProductValueToDisplay(Item.VId) %>
														</div>
													</div>
												</div>
												<div class="product-register-variation-form-group2">
													<div class="product-register-variation-form-group2-col">
														<div class="form-element-group form-element-group-horizontal-grid">
															<div class="form-element-group-title break-text-hover">
																<label>表示順</label>
																<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_DISPLAY_ORDER)) { %>
																<p class="note"><%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_DISPLAY_ORDER) %></p>
																<% } %>
															</div>
															<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
																<label class='<%# string.IsNullOrEmpty(Item.DisplayOrder) ? "empty-value" : string.Empty %>'>
																	<%#: GetProductValueToDisplay(Item.DisplayOrder) %>
																</label>
															</div>
														</div>
														<div runat="server" class="form-element-group form-element-group-horizontal-grid" visible='<%# this.DisplayAddCartUrl %>'>
															<div class="form-element-group-title break-text-hover">
																<label>カート投入URL</label>
															</div>
															<div class="form-element-group-content" style="display: flex; flex-wrap: wrap; margin-top: 0.2em; flex-direction: column;">
																<% if (this.DisplayNormalAddCartUrl) { %>
																	<div class="add_cart_url">
																		<a class="btn-clipboard" href="javascript:void(0)" data-clipboard-text="<%# CreateAddCartUrl(AddCartType.Normal, Item.VariationId) %>">【通常購入用】URLをコピー</a>
																	</div>
																<% } %>
																<% if (this.DisplayFixedPurchaseAddCartUrl) {%>
																	<div class="add_cart_url">
																		<a class="btn-clipboard" href="javascript:void(0)" data-clipboard-text="<%# CreateAddCartUrl(AddCartType.FixedPurchase, Item.VariationId) %>">【定期購入用】URLをコピー</a>
																	</div>
																<% } %>
																<% if (this.DisplayGiftAddCartUrl) { %>
																	<div class="add_cart_url">
																		<a class="btn-clipboard" href="javascript:void(0)" data-clipboard-text="<%# CreateAddCartUrl(AddCartType.Gift, Item.VariationId) %>">【ギフト購入用】URLをコピー</a>
																	</div>
																<% } %>
															</div>
														</div>
														<div class="form-element-group form-element-group-horizontal-grid">
															<div class="form-element-group-title break-text-hover">
																<label>表示名1</label>
																<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1)) { %>
																<p class="note"><%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1) %></p>
																<% } %>
															</div>
															<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
																<label class='<%# string.IsNullOrEmpty(Item.VariationName1) ? "empty-value" : string.Empty %>'>
																	<%#: GetProductValueToDisplay(Item.VariationName1) %>
																</label>
															</div>
														</div>
														<% if (Constants.GLOBAL_OPTION_ENABLE && GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1)) { %>
														<asp:Repeater ID="rTranslationVariationName1" runat="server" 
															DataSource="<%# this.ProductVariationTranslationData.Where(d => 
																(d.MasterId2 == Item.VariationId)
																	&&(d.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTVARIATION_VARIATION_NAME1)) %>"
															ItemType="w2.Domain.NameTranslationSetting.NameTranslationSettingModel">
															<ItemTemplate>
																<div class="form-element-group form-element-group-horizontal-grid">
																	<div class="form-element-group-title break-text-hover">
																		<label>　言語コード:<%#: Item.LanguageCode %> 言語ロケールID:<%#: Item.LanguageLocaleId %></label>
																	</div>
																	<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
																		<label><%#: Item.AfterTranslationalName %></label>
																	</div>
																</div>
															</ItemTemplate>
														</asp:Repeater>
														<% } %>
														<% if (GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2)) { %>
														<div class="form-element-group form-element-group-horizontal-grid">
															<div class="form-element-group-title break-text-hover">
																<label>表示名2</label>
																<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2)) { %>
																<p class="note"><%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2) %></p>
																<% } %>
															</div>
															<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
																<label class='<%# string.IsNullOrEmpty(Item.VariationName2) ? "empty-value" : string.Empty %>'>
																	<%#: GetProductValueToDisplay(Item.VariationName2) %>
																</label>
															</div>
														</div>
														<% } %>
														<% if (Constants.GLOBAL_OPTION_ENABLE && GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2)) { %>
														<asp:Repeater ID="rTranslationVariationName2" runat="server" 
															DataSource="<%# this.ProductVariationTranslationData.Where(d => 
																(d.MasterId2 == Item.VariationId)
																	&&(d.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTVARIATION_VARIATION_NAME2)) %>"
															ItemType="w2.Domain.NameTranslationSetting.NameTranslationSettingModel">
															<ItemTemplate>
																<div class="form-element-group form-element-group-horizontal-grid">
																	<div class="form-element-group-title break-text-hover">
																		<label>　言語コード:<%#: Item.LanguageCode %> 言語ロケールID:<%#: Item.LanguageLocaleId %></label>
																	</div>
																	<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
																		<label><%#: Item.AfterTranslationalName %></label>
																	</div>
																</div>
															</ItemTemplate>
														</asp:Repeater>
														<% } %>
														<% if (GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3)) { %>
														<div class="form-element-group form-element-group-horizontal-grid">
															<div class="form-element-group-title break-text-hover">
																<label>表示名3</label>
																<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3)) { %>
																<p class="note"><%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3) %></p>
																<% } %>
															</div>
															<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
																<label class='<%# string.IsNullOrEmpty(Item.VariationName3) ? "empty-value" : string.Empty %>'>
																	<%#: GetProductValueToDisplay(Item.VariationName3) %>
																</label>
															</div>
														</div>
														<% } %>
														<% if (Constants.GLOBAL_OPTION_ENABLE && GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3)) { %>
														<asp:Repeater ID="rTranslationVariationName3" runat="server"
															DataSource="<%# this.ProductVariationTranslationData.Where(d =>
																(d.MasterId2 == Item.VariationId)
																	&&(d.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTVARIATION_VARIATION_NAME3)) %>"
															ItemType="w2.Domain.NameTranslationSetting.NameTranslationSettingModel">
															<ItemTemplate>
																<div class="form-element-group form-element-group-horizontal-grid">
																	<div class="form-element-group-title break-text-hover">
																		<label>　言語コード:<%#: Item.LanguageCode %> 言語ロケールID:<%#: Item.LanguageLocaleId %></label>
																	</div>
																	<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
																		<label><%#: Item.AfterTranslationalName %></label>
																	</div>
																</div>
															</ItemTemplate>
														</asp:Repeater>
														<% } %>
														<% if (GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COLOR_ID)) { %>
														<div class="form-element-group form-element-group-horizontal-grid">
															<div class="form-element-group-title break-text-hover">
																<label>カラー設定</label>
																<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COLOR_ID)) { %>
																<p class="note"><%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COLOR_ID) %></p>
																<% } %>
															</div>
															<div class="form-element-group-content form-element-group-content-confirm">
																<label class='<%# string.IsNullOrEmpty(ProductColorUtility.GetColorImageDispName(Item.VariationColorId)) ? "empty-value" : string.Empty %>'>
																	<%#: GetProductValueToDisplay(ProductColorUtility.GetColorImageDispName(Item.VariationColorId)) %>
																</label>
																&nbsp;&nbsp;&nbsp;<span class="product-color-img"><img src='<%# ProductColorUtility.GetColorImageUrl(Item.VariationColorId) %>' style="vertical-align: middle;" height="25" width="25" alt='<%#: ProductColorUtility.GetColorImageDispName(Item.VariationColorId) %>' runat="server" visible="<%# string.IsNullOrEmpty(Item.VariationColorId) == false %>" /></span>
															</div>
														</div>
														<% } %>
														<div class="form-element-group form-element-group-horizontal-grid">
															<div class="form-element-group-title break-text-hover">
																<label>販売価格</label>
																<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_PRICE)) { %>
																<p class="note"><%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_PRICE) %></p>
																<% } %>
															</div>
															<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
																<label class='<%# string.IsNullOrEmpty(Item.Price) ? "empty-value" : string.Empty %>'>
																	<%#: Item.Price.ToPriceString(true) %>
																</label>
															</div>
														</div>
														<% if (GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE)) { %>
														<div class="form-element-group form-element-group-horizontal-grid">
															<div class="form-element-group-title break-text-hover">
																<label>特別価格</label>
																<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE)) { %>
																<p class="note"><%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE) %></p>
																<% } %>
															</div>
															<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
																<label class='<%# string.IsNullOrEmpty(Item.SpecialPrice) ? "empty-value" : string.Empty %>'>
																	<%#: GetProductValueToDisplay(Item.SpecialPrice.ToPriceString(true)) %>
																</label>
															</div>
														</div>
														<% } %>
														<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
														<% if (GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE)) { %>
														<div class="form-element-group form-element-group-horizontal-grid">
															<div class="form-element-group-title break-text-hover">
																<label>定期購入初回価格</label>
																<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE)) { %>
																<p class="note"><%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE) %></p>
																<% } %>
															</div>
															<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
																<label class='<%# string.IsNullOrEmpty(Item.VariationFixedPurchaseFirstTimePrice) ? "empty-value" : string.Empty %>'>
																	<%#: GetProductValueToDisplay(Item.VariationFixedPurchaseFirstTimePrice.ToPriceString(true)) %>
																</label>
															</div>
														</div>
														<% } %>
														<% if (GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE)) { %>
														<div class="form-element-group form-element-group-horizontal-grid">
															<div class="form-element-group-title break-text-hover">
																<label>定期購入価格</label>
																<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE)) { %>
																<p class="note"><%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE) %></p>
																<% } %>
															</div>
															<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
																<label class='<%# string.IsNullOrEmpty(Item.VariationFixedPurchasePrice) ? "empty-value" : string.Empty %>'>
																	<%#: GetProductValueToDisplay(Item.VariationFixedPurchasePrice.ToPriceString(true)) %>
																</label>
															</div>
														</div>
														<% } %>
														<% } %>
														<% if (Constants.MEMBER_RANK_OPTION_ENABLED && GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE)) { %>
														<div class="form-element-group-heading" style="margin: unset;">
															<h3 class="form-element-group-heading-label" style="background: #F2F2F2;">会員ランク価格</h3>
														</div>
														<div class="form-element-group form-element-group-horizontal-grid">
															<div class="form-element-group-title break-text-hover">
																<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE)) { %>
																<p class="note"><%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE) %></p>
																<% } %>
															</div>
														</div>
														<div class="form-element-group form-element-group-horizontal-grid">
															<div class="form-element-group-content product-register-member-rank-price-list form-element-group-horizontal-grid" style="flex-wrap: wrap;">
																<asp:Repeater ID="rMemberRankPrice" DataSource="<%# GetMemberRankPricesVariationListForDatabind(Item.VariationId) %>" runat="server">
																	<ItemTemplate>
																		<div class="form-element-group" style="width: 15em; display: flex;">
																			<div class="form-element-group-title member-rank-price-item break-text-hover">
																				<label><%#: ((Hashtable)Container.DataItem)[Constants.FIELD_MEMBERRANK_MEMBER_RANK_NAME] %></label>
																			</div>
																			<div class="form-element-group-content form-element-group-content-item-inline member-rank-price-item">
																				<label class='<%# string.IsNullOrEmpty(StringUtility.ToEmpty(((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE])) ? "empty-value" : string.Empty %>'>
																					<%#: DisplayProductMemberPrice(((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE]) %>
																				</label>
																			</div>
																		</div>
																	</ItemTemplate>
																</asp:Repeater>
															</div>
														</div>
														<% } %>
													</div>
													<div class="product-register-variation-form-group2-col">
														<div class="form-element-group form-element-group-horizontal-grid use-product-image-upload">
															<div class="form-element-group-title break-text-hover">
																<label>商品バリエーション画像</label>
																<p class="note"><%#: Item.VariationImageHead %></p>
															</div>
															<div class="form-element-group-content">
																<div class="product-register-image-selecter" data-flg-variation="true">
																	<div class="product-register-image-selecter-image-main">
																		<div class="product-register-image-selecter-image-main-title">画像</div>
																		<div class="product-register-image-selecter-image-main-thum">
																			<img src="<%#: GetImageSource(Item.VariationImageHead + Constants.PRODUCTIMAGE_FOOTER_LL) %>" alt="" class="product-register-image-selecter-image-main-thum-img" />
																		</div>
																		<div class="product-register-image-selecter-image-main-inset-area ui-droppable">ここに挿入する</div>
																	</div>
																</div>
															</div>
														</div>
														<% if (GetDisplayVariationCooperationIdArea()) { %>
														<div class="form-element-group-heading">
															<h3 class="form-element-group-heading-label" style="background: #F2F2F2;">商品バリエーション連携ID</h3>
														</div>
														<% if (GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID1)) { %>
														<div class="form-element-group form-element-group-horizontal-grid">
															<div class="form-element-group-title break-text-hover">
																<label>商品バリエーション連携ID1</label>
																<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID1)) { %>
																<p class="note"><%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID1) %></p>
																<% } %>
															</div>
															<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
																<label class='<%# string.IsNullOrEmpty(Item.VariationCooperationId1) ? "empty-value" : string.Empty %>'>
																	<%#: GetProductValueToDisplay(Item.VariationCooperationId1) %>
																</label>
															</div>
														</div>
														<% } %>
														<% if (GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID2)) { %>
														<div class="form-element-group form-element-group-horizontal-grid">
															<div class="form-element-group-title break-text-hover">
																<label>商品バリエーション連携ID2</label>
																<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID2)) { %>
																<p class="note"><%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID2) %></p>
																<% } %>
															</div>
															<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
																<label class='<%# string.IsNullOrEmpty(Item.VariationCooperationId2) ? "empty-value" : string.Empty %>'>
																	<%#: GetProductValueToDisplay(Item.VariationCooperationId2) %>
																</label>
															</div>
														</div>
														<% } %>
														<% if (GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID3)) { %>
														<div class="form-element-group form-element-group-horizontal-grid">
															<div class="form-element-group-title break-text-hover">
																<label>商品バリエーション連携ID3</label>
																<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID3)) { %>
																<p class="note"><%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID3) %></p>
																<% } %>
															</div>
															<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
																<label class='<%# string.IsNullOrEmpty(Item.VariationCooperationId3) ? "empty-value" : string.Empty %>'>
																	<%#: GetProductValueToDisplay(Item.VariationCooperationId3) %>
																</label>
															</div>
														</div>
														<% } %>
														<% if (GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID4)) { %>
														<div class="form-element-group form-element-group-horizontal-grid">
															<div class="form-element-group-title break-text-hover">
																<label>商品バリエーション連携ID4</label>
																<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID4)) { %>
																<p class="note"><%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID4) %></p>
																<% } %>
															</div>
															<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
																<label class='<%# string.IsNullOrEmpty(Item.VariationCooperationId4) ? "empty-value" : string.Empty %>'>
																	<%#: GetProductValueToDisplay(Item.VariationCooperationId4) %>
																</label>
															</div>
														</div>
														<% } %>
														<% if (GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID5)) { %>
														<div class="form-element-group form-element-group-horizontal-grid">
															<div class="form-element-group-title break-text-hover">
																<labe>商品バリエーション連携ID5</label>
																<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID5)) { %>
																<p class="note"><%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID5) %></p>
																<% } %>
															</div>
															<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
																<label class='<%# string.IsNullOrEmpty(Item.VariationCooperationId5) ? "empty-value" : string.Empty %>'>
																	<%#: GetProductValueToDisplay(Item.VariationCooperationId5) %>
																</label>
															</div>
														</div>
														<% } %>
														<% if (GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID6)) { %>
														<div class="form-element-group form-element-group-horizontal-grid">
															<div class="form-element-group-title break-text-hover">
																<label>商品バリエーション連携ID6</label>
																<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID6)) { %>
																<p class="note"><%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID6) %></p>
																<% } %>
															</div>
															<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
																<label class='<%# string.IsNullOrEmpty(Item.VariationCooperationId6) ? "empty-value" : string.Empty %>'>
																	<%#: GetProductValueToDisplay(Item.VariationCooperationId6) %>
																</label>
															</div>
														</div>
														<% } %>
														<% if (GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID7)) { %>
														<div class="form-element-group form-element-group-horizontal-grid">
															<div class="form-element-group-title break-text-hover">
																<label>商品バリエーション連携ID7</label>
																<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID7)) { %>
																<p class="note"><%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID7) %></p>
																<% } %>
															</div>
															<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
																<label class='<%# string.IsNullOrEmpty(Item.VariationCooperationId7) ? "empty-value" : string.Empty %>'>
																	<%#: GetProductValueToDisplay(Item.VariationCooperationId7) %>
																</label>
															</div>
														</div>
														<% } %>
														<% if (GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID8)) { %>
														<div class="form-element-group form-element-group-horizontal-grid">
															<div class="form-element-group-title break-text-hover">
																<label>商品バリエーション連携ID8</label>
																<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID8)) { %>
																<p class="note"><%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID8) %></p>
																<% } %>
															</div>
															<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
																<label class='<%# string.IsNullOrEmpty(Item.VariationCooperationId8) ? "empty-value" : string.Empty %>'>
																	<%#: GetProductValueToDisplay(Item.VariationCooperationId8) %>
																</label>
															</div>
														</div>
														<% } %>
														<% if (GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID9)) { %>
														<div class="form-element-group form-element-group-horizontal-grid">
															<div class="form-element-group-title break-text-hover">
																<label>商品バリエーション連携ID9</label>
																<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID9)) { %>
																<p class="note"><%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID9) %></p>
																<% } %>
															</div>
															<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
																<label class='<%# string.IsNullOrEmpty(Item.VariationCooperationId9) ? "empty-value" : string.Empty %>'>
																	<%#: GetProductValueToDisplay(Item.VariationCooperationId9) %>
																</label>
															</div>
														</div>
														<% } %>
														<% if (GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID10)) { %>
														<div class="form-element-group form-element-group-horizontal-grid">
															<div class="form-element-group-title break-text-hover">
																<label>商品バリエーション連携ID10</label>
																<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID10)) { %>
																<p class="note"><%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID10) %></p>
																<% } %>
															</div>
															<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
																<label class='<%# string.IsNullOrEmpty(Item.VariationCooperationId10) ? "empty-value" : string.Empty %>'>
																	<%#: GetProductValueToDisplay(Item.VariationCooperationId10) %>
																</label>
															</div>
														</div>
														<% } %>
														<% } %>
														<% if (GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG)) { %>
														<div class="form-element-group-heading">
															<h3 class="form-element-group-heading-label" style="background: #F2F2F2;">制限設定</h3>
														</div>
														<div class="form-element-group form-element-group-horizontal-grid">
															<div class="form-element-group-title break-text-hover">
																<label>カート投入URL制限フラグ</label>
																<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG)) { %>
																<p class="note"><%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG) %></p>
																<% } %>
															</div>
															<div class="form-element-group-content form-element-group-content-confirm">
																<label><%#: GetProductValueToDisplay(Item.VariationAddCartUrlLimitFlg, Constants.FIELD_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG) %></label>
															</div>
														</div>
														<% } %>
														<% if (GetDisplayVariationMallArea()) { %>
														<div class="form-element-group-heading">
															<h3 class="form-element-group-heading-label" style="background: #F2F2F2;">モール</h3>
														</div>
														<% if (GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG)) { %>
														<div class="form-element-group form-element-group-horizontal-grid">
															<div class="form-element-group-title break-text-hover">
																<label>＆mall連携予約商品フラグ</label>
																<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG)) { %>
																<p class="note"><%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG) %></p>
																<% } %>
															</div>
															<div class="form-element-group-content form-element-group-content-confirm">
																<label><%#: GetProductValueToDisplay(Item.VariationAndMallReservationFlg, Constants.FIELD_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG) %></label>
															</div>
														</div>
														<% } %>
														<% if (GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID1)) { %>
														<div class="form-element-group form-element-group-horizontal-grid">
															<div class="form-element-group-title break-text-hover">
																<h3>モールバリエーション</h3>
																<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID1)) { %>
																<p class="note"><%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID1) %></p>
																<% } %>
															</div>
															<div class="form-element-group-content product-register-variation-mall-variation-list"></div>
														</div>
														<div class="form-element-group form-element-group-horizontal-grid">
															<div class="form-element-group-title">
																<label>ID1</label>
															</div>
															<div class="form-element-group-content product-variation-validate-form-element-group-container form-element-group-content-confirm break-text-hover">
																<label class='<%# string.IsNullOrEmpty(Item.MallVariationId1) ? "empty-value" : string.Empty %>'>
																	<%#: GetProductValueToDisplay(Item.MallVariationId1) %>
																</label>
															</div>
														</div>
														<div class="form-element-group form-element-group-horizontal-grid">
															<div class="form-element-group-title">
																<label>ID2</label>
															</div>
															<div class="form-element-group-content product-variation-validate-form-element-group-container form-element-group-content-confirm break-text-hover">
																<label class='<%# string.IsNullOrEmpty(Item.MallVariationId2) ? "empty-value" : string.Empty %>'>
																	<%#: GetProductValueToDisplay(Item.MallVariationId2) %>
																</label>
															</div>
														</div>
														<div class="form-element-group form-element-group-horizontal-grid">
															<div class="form-element-group-title">
																<label>種別</label>
															</div>
															<div class="form-element-group-content product-variation-validate-form-element-group-container form-element-group-content-confirm break-text-hover">
																<label class='<%# string.IsNullOrEmpty(Item.MallVariationType) ? "empty-value" : string.Empty %>'>
																	<%#: GetProductValueToDisplay(Item.MallVariationType) %>
																</label>
															</div>
														</div>
														<% } %>
														<% } %>
														<% if (Constants.DIGITAL_CONTENTS_OPTION_ENABLED && GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_DOWNLOAD_URL)) { %>
														<div class="form-element-group-heading">
															<h3 class="form-element-group-heading-label" style="background: #F2F2F2;">デジタルコンテンツ</h3>
														</div>
														<div class="form-element-group form-element-group-horizontal-grid">
															<div class="form-element-group-title break-text-hover">
																<label>ダウンロードURL</label>
																<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_DOWNLOAD_URL)) { %>
																<p class="note"><%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_DOWNLOAD_URL) %></p>
																<% } %>
															</div>
															<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
																<label class='<%# string.IsNullOrEmpty(Item.VariationDownloadUrl) ? "empty-value" : string.Empty %>'>
																	<%#: GetProductValueToDisplay(Item.VariationDownloadUrl) %>
																</label>
															</div>
														</div>
														<% } %>
														<% if (Constants.GLOBAL_OPTION_ENABLE && GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_WEIGHT_GRAM)) { %>
														<div class="form-element-group-heading">
															<h3 class="form-element-group-heading-label" style="background: #F2F2F2;">配送設定</h3>
														</div>
														<div class="form-element-group form-element-group-horizontal-grid">
															<div class="form-element-group-title break-text-hover">
																<label>重量（g）</label>
																<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_WEIGHT_GRAM)) { %>
																<p class="note"><%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_WEIGHT_GRAM) %></p>
																<% } %>
															</div>
															<div class="form-element-group-content form-element-group-content-confirm break-text-hover">
																<label class='<%# string.IsNullOrEmpty(Item.VariationWeightGram) ? "empty-value" : string.Empty %>'>
																	<%#: GetProductValueToDisplay(Item.VariationWeightGram) %>
																</label>
															</div>
														</div>
														<% } %>
														<% if (((this.ActionStatus == Constants.ACTION_STATUS_DETAIL) || (this.ActionStatus == Constants.ACTION_STATUS_COMPLETE)) && Constants.PRODUCT_STOCK_OPTION_ENABLE && (this.ProductInput.IsStockUnmanaged == false)) { %>
														<div class="form-element-group-heading">
															<h3 class="form-element-group-heading-label" style="background: #F2F2F2;">在庫</h3>
														</div>
														<div class="form-element-group form-element-group-horizontal-grid">
															<div class="form-element-group-title break-text-hover">
																<label>在庫管理</label>
															</div>
															<div class="form-element-group-content" style="overflow-x: auto;">
																<table class="table-basic" style="width: auto;">
																	<tr>
																		<td nowrap>論理在庫：</td>
																		<td>
																			<%#: CreateStockInfo(this.ProductInput.StockManagementKbn, (Item.ProductStock != null) ? Item.ProductStock.Stock : string.Empty) %>
																		</td>
																		<td nowrap>在庫安全基準：</td>
																		<td>
																			<%#: CreateStockInfo(this.ProductInput.StockManagementKbn, (Item.ProductStock != null) ? Item.ProductStock.StockAlert : string.Empty) %>
																		</td>
																	</tr>
																	<% if (Constants.REALSTOCK_OPTION_ENABLED) { %>
																	<tr>
																		<td nowrap>実在庫：</td>
																		<td>
																			<%#: CreateStockInfo(this.ProductInput.StockManagementKbn, (Item.ProductStock != null) ? Item.ProductStock.Realstock : string.Empty) %>
																		</td>
																		<td nowrap>実在庫B品：</td>
																		<td>
																			<%#: CreateStockInfo(this.ProductInput.StockManagementKbn, (Item.ProductStock != null) ? Item.ProductStock.RealstockB : string.Empty) %>
																		</td>
																		<td nowrap>実在庫C品：</td>
																		<td>
																			<%#: CreateStockInfo(this.ProductInput.StockManagementKbn, (Item.ProductStock != null) ? Item.ProductStock.RealstockC : string.Empty) %>
																		</td>
																	</tr>
																	<% } %>
																</table>
															</div>
														</div>
														<% } %>
													</div>
												</div>
											</div>
										</ItemTemplate>
									</asp:Repeater>
								</div>
							</div>
						</div>
					</div>
				</div>
			</div>
		</div>
	</div>
	<script type="text/javascript">
		// ページロード処理
		function bodyPageLoad(sender, args) {
			BindCartAddUrlCopyEvent(".btn-clipboard");
		}

		// カート投入URLコピーイベントのバインド処理
		function BindCartAddUrlCopyEvent(cssClass) {
			var cp = new ClipboardJS(cssClass);
			cp.on("success", function (e) {
				alert("コピーに成功しました。");
			});
			cp.on("error", function (e) {
				alert("コピーに失敗しました。");
			});
		}

		// Set width fixed purchase discount setting table
		function setWidthFixedPurchaseDiscountSettingTable() {
			var parentWidth = parseInt($('.page-product-register').width()) - 380;
			var widthTable = parseInt($('.table-basic > tbody').width());
			if (widthTable > parentWidth) {
				$('.sample-checkbox-toggle-area-2').width(parentWidth);
			} else {
				$('.sample-checkbox-toggle-area-2').width('fit-content');
			}
		}

		$(function () {
			$(window).bind('resize', function () {
				setWidthFixedPurchaseDiscountSettingTable();
			});

			// HTML文書表示用フレームのウィンドウ幅調整
			$(window).bind('load', function () {
				$('iframe.HtmlPreview').each(function () {
					var doc = $(this).get(0).contentWindow.document;
					var innerHeight = Math.max(
						doc.body.scrollHeight, doc.documentElement.scrollHeight,
						doc.body.offsetHeight, doc.documentElement.offsetHeight,
						doc.body.clientHeight, doc.documentElement.clientHeight);
					$(this).removeAttr("height").css('height', innerHeight + 'px');
				});

				setWidthFixedPurchaseDiscountSettingTable();
			});
		});

		function disableButtonAndSubmit() {
			var btnInsert = document.getElementById('<%= btnInsert.ClientID %>');
			var btnUpdate = document.getElementById('<%= btnUpdate.ClientID %>');
			if (btnInsert !== null) {
				btnInsert.disabled = true;
				__doPostBack('<%= btnInsert.UniqueID %>', '');
			}
			else if (btnUpdate !== null) {
				btnUpdate.disabled = true;
				__doPostBack('<%= btnUpdate.UniqueID %>', '');
			}
		}
	</script>
</asp:Content>
