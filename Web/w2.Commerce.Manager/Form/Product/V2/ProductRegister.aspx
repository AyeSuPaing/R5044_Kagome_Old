<%--
=========================================================================================================
  Module      : 商品情報登録ページ(ProductRegister.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ProductRegister.aspx.cs" Inherits="Form_Product_V2_ProductRegister" %>

<%@ Import Namespace="w2.App.Common.Manager" %>
<%@ Import Namespace="w2.Domain.MenuAuthority.Helper" %>
<%@ Import Namespace="w2.App.Common.Option" %>
<%@ Import Namespace="w2.App.Common.Product" %>
<%@ Import Namespace="w2.Common.Util" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderHead" runat="server">
	<script type="text/javascript" charset="Shift_JIS" src="<%= this.ResolveClientUrl("~/Js/Manager.productRegister.js") %>"></script>
	<meta http-equiv="Pragma" content="no-cache" />
	<meta http-equiv="cache-control" content="no-cache" />
	<meta http-equiv="expires" content="0" />

	<style type="text/css">
		.ajax-loading {
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

		.product-register-campaign-icon .select-period {
			justify-content: center;
			margin: 0px;
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

			.product-register-image-selecter-image-sub-wrapper {
				max-height: 20em !important;
			}
		}

		.form-element-group-content select {
			max-width: 21.5em;
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

		.number-textbox {
			text-align: right;
		}

		.table-discount tr td {
			background-color: #f2f2f2;
		}

		.price-discount-form,
		.point-discount-form {
			background: #fff !important;
		}

		.product-color-img {
			padding: 1px;
			background: #e1e1e1;
			display: inline-block;
		}

		.custom-category-selector-category,
		.custom-category-selector-brand {
			width: calc(60% - 5px);
		}

		.custom-category-selector-input-suggest {
			margin-top: -8px;
		}

		.custom-category-selector-input-suggest input[type="text"] {
			width: 100%;
		}

		.custom-category-selector-input-selected-item {
			width: calc(100% - 5px);
		}

		.custom-category-selector-input-selected-items {
			padding-right: 0px;
		}
		.parent-price{
			display: flex;
			padding-bottom: 10px;
			align-items: center;
		}
		.btnBackground {
			border:.5px solid darkgrey;
			padding: 3px 5px 3px 5px;
			border-top-right-radius: 5px;
			border-bottom-right-radius: 5px;
		}
		input.firstControl {
			padding: 0;
			margin: 0;
		}
		.product-image-head-explanation {
			border: 1px solid #bababa;
			border-radius: 9px;
			margin-bottom: 15px;
			padding: 20px;
		}
		.image-explanation-table {
			border:1px solid #aaa;
			border-collapse: separate;
			overflow: hidden;
			border-spacing: 0;
			border-radius: 9px;
			text-align: center;
		}
		.image-explanation-table td{
			border:1px solid #aaa;
		}
	</style>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
	<div class="page-product-register">
		<div class="page-title-wrapper">
			<% if(this.ActionStatus == Constants.ACTION_STATUS_UPDATE) { %>
			<h1 class="page-title">商品情報編集</h1>
			<% } else { %>
			<h1 class="page-title">商品情報登録</h1>
			<% } %>
			<a href="javascript:resetImportFileForm()" class="btn btn-sub btn-size-m btn-modal-open" data-modal-selector="#modal-product-register-master-upload" data-modal-classname="modal-size-m">商品マスタアップロード</a>
			<!-- 商品マスターアップロードモーダル -->
			<div class="modal-content-hide">
				<div id="modal-product-register-master-upload">
					<div class="modal-product-register-master-upload">
						<p class="modal-inner-ttl">商品マスタアップロード</p>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<label for="form-input-modal-product-register-master-upload-1">マスター種別</label>
							</div>
							<div class="form-element-group-content">
								<select id="form-input-modal-product-register-master-upload-1"></select>
							</div>
						</div>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<label for="form-input-modal-product-register-master-upload-2">ファイル指定</label>
							</div>
							<div class="form-element-group-content">
								<div class="dd-file-select js-dd-file-select">
									<div class="dd-file-select-file">
										<p class="dd-file-select-filename">選択されていません</p>
										<a href="javascript:void(0);" class="dd-file-select-file-cancel" data-popover-message="削除する"><span class="icon-close"></span></a>
									</div>
									<label class="dd-file-select-label">
										<input type="file" class="dd-file-select-input" id="form-input-modal-product-register-master-upload-2" />
										<span class="dd-file-select-icon icon-clip"></span>
										<p class="dd-file-select-message">ファイルをドラッグ＆ドロップしてファイルを指定してください。</p>
										<div class="dd-file-select-btns">
											<p class="dd-file-select-btns-text">または</p>
											<a href="javascript:void(0);" class="dd-file-select-btn btn btn-main btn-size-s">ファイルを選択</a>
										</div>
									</label>
								</div>
								<div style="padding-top: 10px; word-wrap: break-word;">
									<p id="form-input-modal-product-register-master-upload-error-message" style="color: red;" />
									<p id="form-input-modal-product-register-master-upload-message" />
								</div>
							</div>
						</div>
						<div class="modal-footer-action">
							<input type="button" class="btn btn-main btn-size-l" value="アップロード" onclick="execImportFile();" />
						</div>
					</div>
				</div>
			</div>
			<!-- 商品マスターアップロードモーダル -->
		</div>
		<div class="main-contents-fixed-header product-register-header">
			<div class="main-contents-fixed-header-inner">
				<div class="main-contents-fixed-header-col-1">
					<div class="slide-checkbox-wrap product-register-header-all-toggle">
						<input type="checkbox" name="form-element-all-toggle-trigger" value="" id="form-element-all-toggle-trigger" class="js-form-element-all-toggle-trigger" />
						<label for="form-element-all-toggle-trigger" class="slide-checkbox">
							<span class="slide-checkbox-label">すべての設定項目を表示</span>
							<span class="slide-checkbox-btn"></span>
						</label>
					</div>
				</div>
				<div class="main-contents-fixed-header-col-2">
					<div class="product-register-header-preview-btn">
						<a class="btn btn-txt btn-size-m" onclick="previewProduct('PC')">
							<%: (w2.App.Common.Design.DesignCommon.UseResponsive) ? "プレビュー（RESPONSIVE）" : "プレビュー（PC）" %><span class="btn-icon-right icon-arrow-out"></span>
						</a>
						<a class="btn btn-txt btn-size-m" onclick="previewProduct('SmartPhone')" style='<%= (w2.App.Common.Design.DesignCommon.UseResponsive) ? "display: none;" : string.Empty %>'>
							プレビュー（SP）<span class="btn-icon-right icon-arrow-out"></span>
						</a>
					</div>
					<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_VALID_FLG)) { %>
					<div class="product-register-header-valid-flg product-validate-form-element-group-container">
						<div class="slide-checkbox-wrap break-text-hover">
							<input id="cbValidFlg" type="checkbox" name="valid-flg" value="" <%= (this.ProductInput.ValidFlg != Constants.FLG_PRODUCT_VALID_FLG_INVALID) ? "checked" : string.Empty %> />
							<label for="cbValidFlg" class="slide-checkbox">
								<span class="slide-checkbox-label">有効フラグ
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_VALID_FLG)) { %>
									<p class="note" style="white-space: normal; margin-bottom: 25px;"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_VALID_FLG) %></p>
									<% } %>
								</span>
								<span class="slide-checkbox-btn"></span>
							</label>
						</div>
						<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_VALID_FLG %>"></div>
					</div>
					<% } %>
					<div class="main-contents-fixed-header-btn">
						<% if (this.IsShowBackButton) { %>
						<% if (this.ActionStatus == Constants.ACTION_STATUS_INSERT) { %>
						<a class="btn btn-sub btn-size-l" href="<%: CreateProductListUrl((Hashtable)Session[Constants.SESSIONPARAM_KEY_PRODUCT_SEARCH_INFO]) %>">一覧へ戻る</a>
						<% } else { %>
						<a class="btn btn-sub btn-size-l" href="<%: CreateProductDetailUrl(this.RequestProductId) %>">戻る</a>
						<% } %>
						<% } %>
						<input id="btnSubmit" type="button" class="btn btn-main btn-size-l" onclick="execConfirmProcess();" value="  確認する  " />
					</div>
					<div class="dropdown">
						<a href="javascript:void(0)" class="btn-dot-menu dropdown-toggle"><span class="icon-dots"></span></a>
						<div class="dropdown-menu">
							<a class="dropdown-menu-item" href="<%: CreateProductRegistDefaultSettingUrl() %>">表示項目を変更する</a>
							<a href="javascript:void(0)" class="dropdown-menu-item" onclick="window.location.reload()">削除</a>
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
									<label for="tbProductId">商品ID<span class="notice">*</span>	</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_PRODUCT_ID)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_PRODUCT_ID) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container" style='<%= (this.ActionStatus == Constants.ACTION_STATUS_UPDATE) ? "margin-top: 0.3em;" : string.Empty %>'>
									<% if(this.ActionStatus == Constants.ACTION_STATUS_UPDATE) { %>
									<label><%= this.ProductInput.ProductId %></label>
									<% } %>
									<input id="tbProductId" value="<%= this.ProductInput.ProductId %>" type="text" maxlength="30" style='<%= (this.ActionStatus == Constants.ACTION_STATUS_UPDATE) ? "display: none;" : string.Empty %>' />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_PRODUCT_ID %>"></div>
								</div>
							</div>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_NAME)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label for="tbProductName">商品名<span class="notice">*</span></label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_NAME)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_NAME) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<input id="tbProductName" value="<%: this.ProductInput.Name %>" type="text" maxlength="100" />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_NAME %>"></div>
								</div>
							</div>
							<% } %>
							<% if (this.IsOperationalCountryJp && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_NAME_KANA)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label for="tbNameKana">商品名(カナ)</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_NAME_KANA)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_NAME_KANA) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<input id="tbNameKana" value="<%: this.ProductInput.NameKana %>" type="text" maxlength="100" />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_NAME_KANA %>"></div>
								</div>
							</div>
							<% } %>
							<% if (Constants.PRODUCTBUNDLE_OPTION_ENABLED && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_PRODUCT_TYPE)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label for="rblProductType">商品区分</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_PRODUCT_TYPE)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_PRODUCT_TYPE) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content">
									<div class="form-element-group-content-item-inline">
										<input id="rblProductType-1" type="radio" class="radio" name="rblProductType" <%= (this.ProductInput.ProductType != Constants.FLG_PRODUCT_PRODUCT_TYPE_FLYER) ? "checked" : string.Empty %> />
										<label for="rblProductType-1">通常商品</label>
									</div>
									<div class="form-element-group-content-item-inline">
										<input id="rblProductType-2" type="radio" class="radio" name="rblProductType" <%= (this.ProductInput.ProductType == Constants.FLG_PRODUCT_PRODUCT_TYPE_FLYER) ? "checked" : string.Empty %> />
										<label for="rblProductType-2">チラシ</label>
									</div>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label for="tbMaxSellQuantity">1注文購入限度数<span class="notice">*</span></label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<input id="tbMaxSellQuantity" value="<%= (string.IsNullOrEmpty(this.ProductInput.MaxSellQuantity) == false) ? this.ProductInput.MaxSellQuantity : CONST_MAX_SELL_QUANTITY_DEFAULT.ToString() %>" type="text" class="w6em number-textbox" maxlength="3" />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY %>"></div>
								</div>
							</div>
							<% } %>
							<% if (Constants.GIFTORDER_OPTION_ENABLED && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_GIFT_FLG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label for="ddlGiftFlg">ギフト購入フラグ</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_GIFT_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_GIFT_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content">
									<select id="ddlGiftFlg">
										<option value="<%= Constants.FLG_PRODUCT_GIFT_FLG_INVALID %>" selected="selected">不可</option>
										<option value="<%= Constants.FLG_PRODUCT_GIFT_FLG_VALID %>" <%= (this.ProductInput.GiftFlg == Constants.FLG_PRODUCT_GIFT_FLG_VALID) ? "selected='selected'" : string.Empty %>>可能</option>
										<option value="<%= Constants.FLG_PRODUCT_GIFT_FLG_ONLY %>" <%= (this.ProductInput.GiftFlg == Constants.FLG_PRODUCT_GIFT_FLG_ONLY) ? "selected='selected'" : string.Empty %>>ギフト購入のみ</option>
									</select>
								</div>
							</div>
							<% } %>
							<% if ((Constants.RECOMMEND_ENGINE_KBN == Constants.RecommendEngine.Silveregg) && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_USE_RECOMMEND_FLG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label for="cbUseRecommendFlg">外部レコメンド利用</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_USE_RECOMMEND_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_USE_RECOMMEND_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content">
									<div class="slide-checkbox-wrap">
										<input id="cbUseRecommendFlg" type="checkbox" name="valid-flg" data-on-label="有効" data-off-label="無効" <%= (this.ProductInput.UseRecommendFlg != Constants.FLG_PRODUCT_USE_RECOMMEND_FLG_INVALID) ? "checked" : string.Empty %> />
										<label for="cbUseRecommendFlg" class="slide-checkbox">
											<span class="slide-checkbox-btn"></span>
											<span class="slide-checkbox-label"></span>
										</label>
									</div>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_NOTE)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label for="tbNote">備考</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_NOTE)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_NOTE) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<textarea id="tbNote" rows="4" cols="80"><%: this.ProductInput.Note %></textarea>
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_NOTE %>"></div>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_SUPPLIER_ID)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label for="tbSupplierId">サプライヤID</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_SUPPLIER_ID)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_SUPPLIER_ID) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<input id="tbSupplierId" value="<%: this.ProductInput.SupplierId %>" type="text" maxlength="20" class="w10em" />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_SUPPLIER_ID %>"></div>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_SEARCH_KEYWORD)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label for="tbSearchKeyword">検索キーワード</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_SEARCH_KEYWORD)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_SEARCH_KEYWORD) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<div class="input-textarea">
										<div class="input-textarea-content ">
											<textarea id="tbSearchKeyword"><%: this.ProductInput.SearchKeyword %></textarea>
										</div>
									</div>
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_SEARCH_KEYWORD %>"></div>
								</div>
							</div>
							<% } %>
							<% if ((Constants.PRODUCTBUNDLE_OPTION_ENABLED) && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label for="cbBundleItemDisplayType">同梱商品明細表示フラグ</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content">
									<div class="slide-checkbox-wrap">
										<input id="cbBundleItemDisplayType" type="checkbox" name="valid-flg" data-on-label="有効" data-off-label="無効" <%= (this.ProductInput.BundleItemDisplayType != Constants.FLG_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE_INVALID) ? "checked" : string.Empty %> />
										<label for="cbBundleItemDisplayType" class="slide-checkbox">
											<span class="slide-checkbox-btn"></span>
											<span class="slide-checkbox-label"></span>
										</label>
									</div>
								</div>
							</div>
							<% } %>
							<div id="divProductExtend"></div>
						</div>
						<div class="block-section-body-inner-col">
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CATCHCOPY)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label for="tbCatchcopy">キャッチコピー</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_CATCHCOPY)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_CATCHCOPY) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<input id="tbCatchcopy" value="<%: this.ProductInput.Catchcopy %>" type="text" maxlength="60" />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_CATCHCOPY %>"></div>
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
								<div class="form-element-group-content product-validate-form-element-group-container">
									<div class="input-textarea">
										<div class="input-textarea-type-selector">
											<input id="rbOutlineFlg1" type="radio" name="rbOutlineFlg" class="btn-radio" <%= (this.ProductInput.OutlineKbn != Constants.FLG_PRODUCT_DESC_DETAIL_HTML) ? "checked" : string.Empty %> />
											<label for="rbOutlineFlg1" class="btn btn-size-s">TEXT</label>
											<input id="rbOutlineFlg2" type="radio" name="rbOutlineFlg" class="btn-radio js-toggle-form-radio" data-toggle-content-selector=".input-textarea-htmleditor-btn-1" <%= (this.ProductInput.OutlineKbn == Constants.FLG_PRODUCT_DESC_DETAIL_HTML) ? "checked" : string.Empty %> />
											<label for="rbOutlineFlg2" class="btn btn-size-s">HTML</label>
										</div>
										<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_OUTLINE_KBN %>"></div>
										<div class="input-textarea-content">
											<textarea id="tbOutline"><%: this.ProductInput.Outline %></textarea>
											<div class="input-textarea-htmleditor-btn-1 input-textarea-htmleditor-btn">
												<input type="button" value="HTMLエディタ" class="btn btn-main btn-size-s" onclick="javascript: open_wysiwyg('tbOutline', 'rbOutlineFlg2', true);" />
											</div>
										</div>
										<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_OUTLINE %>"></div>
									</div>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DESC_DETAIL1)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label>商品詳細説明１</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_DESC_DETAIL1)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DESC_DETAIL1) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<div class="input-textarea">
										<div class="input-textarea-type-selector">
											<input id="rbDescDetailFlg1-1" type="radio" name="rbDescDetailFlg1" class="btn-radio" <%= (this.ProductInput.DescDetailKbn1 != Constants.FLG_PRODUCT_DESC_DETAIL_HTML) ? "checked" : string.Empty %> />
											<label for="rbDescDetailFlg1-1" class="btn btn-size-s">TEXT</label>
											<input id="rbDescDetailFlg1-2" type="radio" name="rbDescDetailFlg1" class="btn-radio js-toggle-form-radio" data-toggle-content-selector=".input-textarea-htmleditor-btn-2" <%= (this.ProductInput.DescDetailKbn1 == Constants.FLG_PRODUCT_DESC_DETAIL_HTML) ? "checked" : string.Empty %> />
											<label for="rbDescDetailFlg1-2" class="btn btn-size-s">HTML</label>
										</div>
										<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_DESC_DETAIL_KBN1 %>"></div>
										<div class="input-textarea-content">
											<textarea id="tbDescDetail1"><%: this.ProductInput.DescDetail1 %></textarea>
											<div class="input-textarea-htmleditor-btn-2 input-textarea-htmleditor-btn">
												<input type="button" value="HTMLエディタ" class="btn btn-main btn-size-s" onclick="javascript: open_wysiwyg('tbDescDetail1', 'rbDescDetailFlg1-2', true);" />
											</div>
										</div>
										<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_DESC_DETAIL1 %>"></div>
									</div>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DESC_DETAIL2)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label>商品詳細説明２</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_DESC_DETAIL2)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DESC_DETAIL2) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<div class="input-textarea">
										<div class="input-textarea-type-selector">
											<input id="rbDescDetailFlg2-1" type="radio" name="rbDescDetailFlg2" class="btn-radio" <%= (this.ProductInput.DescDetailKbn2 != Constants.FLG_PRODUCT_DESC_DETAIL_HTML) ? "checked" : string.Empty %> />
											<label for="rbDescDetailFlg2-1" class="btn btn-size-s">TEXT</label>
											<input id="rbDescDetailFlg2-2" type="radio" name="rbDescDetailFlg2" class="btn-radio js-toggle-form-radio" data-toggle-content-selector=".input-textarea-htmleditor-btn-3" <%= (this.ProductInput.DescDetailKbn2 == Constants.FLG_PRODUCT_DESC_DETAIL_HTML) ? "checked" : string.Empty %> />
											<label for="rbDescDetailFlg2-2" class="btn btn-size-s">HTML</label>
										</div>
										<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_DESC_DETAIL_KBN2 %>"></div>
										<div class="input-textarea-content">
											<textarea id="tbDescDetail2"><%: this.ProductInput.DescDetail2 %></textarea>
											<div class="input-textarea-htmleditor-btn-3 input-textarea-htmleditor-btn">
												<input type="button" value="HTMLエディタ" class="btn btn-main btn-size-s" onclick="javascript: open_wysiwyg('tbDescDetail2', 'rbDescDetailFlg2-2', true);" />
											</div>
										</div>
										<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_DESC_DETAIL2 %>"></div>
									</div>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DESC_DETAIL3)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label>商品詳細説明３</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_DESC_DETAIL3)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DESC_DETAIL3) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<div class="input-textarea">
										<div class="input-textarea-type-selector">
											<input id="rbDescDetailFlg3-1" type="radio" name="rbDescDetailFlg3" class="btn-radio" <%= (this.ProductInput.DescDetailKbn3 != Constants.FLG_PRODUCT_DESC_DETAIL_HTML) ? "checked" : string.Empty %> />
											<label for="rbDescDetailFlg3-1" class="btn btn-size-s">TEXT</label>
											<input id="rbDescDetailFlg3-2" type="radio" name="rbDescDetailFlg3" class="btn-radio js-toggle-form-radio" data-toggle-content-selector=".input-textarea-htmleditor-btn-4" <%= (this.ProductInput.DescDetailKbn3 == Constants.FLG_PRODUCT_DESC_DETAIL_HTML) ? "checked" : string.Empty %> />
											<label for="rbDescDetailFlg3-2" class="btn btn-size-s">HTML</label>
										</div>
										<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_DESC_DETAIL_KBN3 %>"></div>
										<div class="input-textarea-content">
											<textarea id="tbDescDetail3"><%: this.ProductInput.DescDetail3 %></textarea>
											<div class="input-textarea-htmleditor-btn-4 input-textarea-htmleditor-btn">
												<input type="button" value="HTMLエディタ" class="btn btn-main btn-size-s" onclick="javascript: open_wysiwyg('tbDescDetail3', 'rbDescDetailFlg3-2', true);" />
											</div>
										</div>
										<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_DESC_DETAIL3 %>"></div>
									</div>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DESC_DETAIL4)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label>商品詳細説明４</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_DESC_DETAIL4)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DESC_DETAIL4) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<div class="input-textarea">
										<div class="input-textarea-type-selector">
											<input id="rbDescDetailFlg4-1" type="radio" name="rbDescDetailFlg4" class="btn-radio" <%= (this.ProductInput.DescDetailKbn4 != Constants.FLG_PRODUCT_DESC_DETAIL_HTML) ? "checked" : string.Empty %> />
											<label for="rbDescDetailFlg4-1" class="btn btn-size-s">TEXT</label>
											<input id="rbDescDetailFlg4-2" type="radio" name="rbDescDetailFlg4" class="btn-radio js-toggle-form-radio" data-toggle-content-selector=".input-textarea-htmleditor-btn-5" <%= (this.ProductInput.DescDetailKbn4 == Constants.FLG_PRODUCT_DESC_DETAIL_HTML) ? "checked" : string.Empty %> />
											<label for="rbDescDetailFlg4-2" class="btn btn-size-s">HTML</label>
										</div>
										<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_DESC_DETAIL_KBN4 %>"></div>
										<div class="input-textarea-content">
											<textarea id="tbDescDetail4"><%: this.ProductInput.DescDetail4 %></textarea>
											<div class="input-textarea-htmleditor-btn-5 input-textarea-htmleditor-btn">
												<input type="button" value="HTMLエディタ" class="btn btn-main btn-size-s" onclick="javascript: open_wysiwyg('tbDescDetail4', 'rbDescDetailFlg4-2', true);" />
											</div>
										</div>
										<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_DESC_DETAIL4 %>"></div>
									</div>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label for="tbReturnExchangeMessage">返品・交換・解約説明(TEXT)</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<div class="input-textarea">
										<div class="input-textarea-content ">
											<textarea id="tbReturnExchangeMessage"><%: this.ProductInput.ReturnExchangeMessage %></textarea>
										</div>
									</div>
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE %>"></div>
								</div>
							</div>
							<% } %>
							<% if (Constants.GOOGLESHOPPING_COOPERATION_OPTION_ENABLED && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_GOOGLE_SHOPPING_FLG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label for="cbGoogleShoppingFlg">Googleショッピング連携</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_GOOGLE_SHOPPING_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_GOOGLE_SHOPPING_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content">
									<div class="slide-checkbox-wrap">
										<input id="cbGoogleShoppingFlg" type="checkbox" name="valid-flg" data-popover-message="※Googleショッピングへ登録する商品の商品IDは、半角英数で登録して下さい。" data-on-label="有効" data-off-label="無効" <%= (this.ProductInput.GoogleShoppingFlg != Constants.FLG_PRODUCT_GOOGLE_SHOPPING_FLG_INVALID) ? "checked" : string.Empty %> />
										<label for="cbGoogleShoppingFlg" class="slide-checkbox">
											<span class="slide-checkbox-btn"></span>
											<span class="slide-checkbox-label"></span>
										</label>
									</div>
								</div>
							</div>
							<% } %>
							<% if (GetDisplayProductInquireArea()) { %>
							<div class="form-element-group-heading is-form-element-toggle">
								<h3 class="form-element-group-heading-label">問い合わせ</h3>
							</div>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_URL)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label for="tbUrl">紹介URL</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_URL)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_URL) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<input id="tbUrl" value="<%: this.ProductInput.Url %>" type="text" maxlength="256" />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_URL %>"></div>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_INQUIRE_EMAIL)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label for="tbInquiteEmail">問い合わせ用メールアドレス</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_INQUIRE_EMAIL)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_INQUIRE_EMAIL) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<input id="tbInquiteEmail" value="<%= this.ProductInput.InquireEmail %>" type="text" maxlength="256" />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_INQUIRE_EMAIL %>"></div>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_INQUIRE_TEL)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label for="tbInquiteTel">問い合わせ用電話番号</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_INQUIRE_TEL)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_INQUIRE_TEL) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<input id="tbInquiteTel" value="<%: this.ProductInput.InquireTel %>" type="text" class="w10em number-textbox" maxlength="30" />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_INQUIRE_TEL %>"></div>
								</div>
							</div>
							<% } %>
							<% } %>
							<% if (GetDisplayProductCooperationIdsArea()) { %>
							<div class="form-element-group-heading is-form-element-toggle">
								<h3 class="form-element-group-heading-label">商品連携ID</h3>
							</div>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID1)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label for="tbCooperationId1">商品連携ID1</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID1)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID1) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<input id="tbCooperationId1" value="<%: this.ProductInput.CooperationId1 %>" type="text" maxlength="30" class="w10em" />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_COOPERATION_ID1 %>"></div>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID2)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label for="tbCooperationId2">商品連携ID2</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID2)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID2) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<input id="tbCooperationId2" value="<%: this.ProductInput.CooperationId2 %>" type="text" maxlength="30" class="w10em" />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_COOPERATION_ID2 %>"></div>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID3)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label for="tbCooperationId3">商品連携ID3</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID3)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID3) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<input id="tbCooperationId3" value="<%: this.ProductInput.CooperationId3 %>" type="text" maxlength="30" class="w10em" />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_COOPERATION_ID3 %>"></div>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID4)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label for="tbCooperationId4">商品連携ID4</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID4)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID4) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<input id="tbCooperationId4" value="<%: this.ProductInput.CooperationId4 %>" type="text" maxlength="30" class="w10em" />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_COOPERATION_ID4 %>"></div>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID5)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label for="tbCooperationId5">商品連携ID5</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID5)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID5) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<input id="tbCooperationId5" value="<%: this.ProductInput.CooperationId5 %>" type="text" maxlength="30" class="w10em" />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_COOPERATION_ID5 %>"></div>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID6)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label for="tbCooperationId6">商品連携ID6</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID6)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID6) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<input id="tbCooperationId6" value="<%: this.ProductInput.CooperationId6 %>" type="text" maxlength="30" class="w10em" />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_COOPERATION_ID6 %>"></div>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID7)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label for="tbCooperationId7">商品連携ID7</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID7)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID7) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<input id="tbCooperationId7" value="<%: this.ProductInput.CooperationId7 %>" type="text" maxlength="30" class="w10em" />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_COOPERATION_ID7 %>"></div>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID8)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label for="tbCooperationId8">商品連携ID8</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID8)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID8) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<input id="tbCooperationId8" value="<%: this.ProductInput.CooperationId8 %>" type="text" maxlength="30" class="w10em" />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_COOPERATION_ID8 %>"></div>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID9)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label for="tbCooperationId9">商品連携ID9</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID9)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID9) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<input id="tbCooperationId9" value="<%: this.ProductInput.CooperationId9 %>" type="text" maxlength="30" class="w10em" />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_COOPERATION_ID9 %>"></div>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_COOPERATION_ID10)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label for="tbCooperationId10">商品連携ID10</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID10)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_COOPERATION_ID10) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<input id="tbCooperationId10" value="<%: this.ProductInput.CooperationId10 %>" type="text" maxlength="30" class="w10em" />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_COOPERATION_ID10 %>"></div>
								</div>
							</div>
							<% } %>
							<% } %>
						</div>
					</div>
					<div class="block-section-body-inner-row">
						<div class="block-section-body-inner-col">
							<% if (GetDisplayProductOptionValueArea()) { %>
							<div class="form-element-group-heading is-form-element-toggle">
								<h3 class="form-element-group-heading-label">商品付帯</h3>
							</div>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-content product-validate-form-element-group-container">
									<div class="form-element-group-content-item-inline">
										<input id="rbProductOptionValueInputFormBasic-1" type="radio" name="rbProductOptionValueInputFormBasic" class="radio js-toggle-form-radio" data-toggle-content-selector=".product-register-option-toggle-content-basic" onchange="setProductOptionSettingToBasic()" checked="checked" />
										<label for="rbProductOptionValueInputFormBasic-1">簡単入力</label>
									</div>
									<div class="form-element-group-content-item-inline">
										<input id="rbProductOptionValueInputFormBasic-2" type="radio" name="rbProductOptionValueInputFormBasic" class="radio js-toggle-form-radio" data-toggle-content-selector=".product-register-option-toggle-content-advance" onchange="setProductOptionSettingToAdvance()"/>
										<label for="rbProductOptionValueInputFormBasic-2">一括入力</label>
									</div>
									<div class="product-error-message-container" data-id="<%= ProductInput.CONST_ERROR_KEY_PRODUCT_OPTIONS %>"></div>
								</div>
							</div>
							<div class="is-form-element-toggle product-validate-form-element-group-container">
								<div class="product-register-option-toggle-content-basic">
									<div class="form-element-group-list">
										<asp:Repeater ID="rProductOption" runat="server">
											<ItemTemplate>
												<div class="form-element-group-list-item" Visible="<%# GetProductDefaultSettingDisplayField(ProductOptionSettingHelper.GetProductOptionSettingKey((int)Container.DataItem)) %>" runat="server">
													<div class="form-element-group-list-item-title break-text-hover">
														<label><%# GrantNumberToString("付帯情報", Container.DataItem) %></label>
														<p class="note" Visible="<%# HasProductDefaultSettingComment(ProductOptionSettingHelper.GetProductOptionSettingKey((int)Container.DataItem)) %>" runat="server">
															<%# GetProductDefaultSettingComment(ProductOptionSettingHelper.GetProductOptionSettingKey((int)Container.DataItem)) %>
														</p>
													</div>
													<div class="form-element-group-list-item-content">
														<div class="form-element-group">
															<div class="form-element-group-content">
																<input id="<%# GrantNumberToString("cbProductOptionValue", Container.DataItem) %>" type="checkbox" class="checkbox js-toggle-form" name="<%# GrantNumberToString("cbProductOptionValue", Container.DataItem) %>" data-toggle-content-selector="<%# GrantNumberToString(CONST_PRODUCT_REGISTER_OPTION_TOGGLE_CONTENT_BASIC_SETTING, Container.ItemIndex) %>" />
																<label for="<%# GrantNumberToString("cbProductOptionValue", Container.DataItem) %>">付帯情報を設定する</label>
															</div>
														</div>
														<div class="product-register-option-toggle-content-basic-setting">
															<div class="form-element-group">
																<div class="form-element-group-title">
																	<label for="<%# GrantNumberToString(CONST_PRODUCT_OPTION_VALUE_NAME, Container.DataItem) %>">項目名</label>
																</div>
																<div class="form-element-group-content">
																	<input id="<%# GrantNumberToString(CONST_PRODUCT_OPTION_VALUE_NAME, Container.DataItem) %>" type="text" style="width: 50%" />
																</div>
															</div>
															<div class="form-element-group">
																<div class="form-element-group-title">
																	<label for="<%# GrantNumberToString("ddlProductOptionValue", Container.DataItem) %>">表示形式</label>
																</div>
																<div class="form-element-group-content">
																	<select id="<%# GrantNumberToString("ddlProductOptionValue", Container.DataItem) %>" class="js-toggle-form-select">
																		<option value="<%= Constants.PRODUCTOPTIONVALUES_DISP_KBN_SELECTMENU %>" data-toggle-content-selector="<%# GrantNumberToString(CONST_PRODUCT_REGISTER_OPTION_TOGGLE_CONTENT_BASIC_SETTING_STYLE_LIST, Container.ItemIndex) %>">ドロップダウン</option>
																		<option value="<%= Constants.PRODUCTOPTIONVALUES_DISP_KBN_CHECKBOX %>" data-toggle-content-selector="<%# GrantNumberToString(CONST_PRODUCT_REGISTER_OPTION_TOGGLE_CONTENT_BASIC_SETTING_STYLE_LIST, Container.ItemIndex) %>">チェックボックス</option>
																		<option value="<%= Constants.PRODUCTOPTIONVALUES_DISP_KBN_TEXTBOX %>" data-toggle-content-selector="<%# GrantNumberToString(CONST_PRODUCT_REGISTER_OPTION_TOGGLE_CONTENT_BASIC_SETTING_STYLE_TEXT, Container.ItemIndex) %>">テキストボックス</option>
																		<% if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED) { %>
																			<option value="<%= Constants.PRODUCTOPTIONVALUES_DISP_KBN_PRICE_DROPDOWNMENU %>" data-toggle-content-selector="<%# GrantNumberToString(CONST_PRODUCT_REGISTER_OPTION_TOGGLE_CONTENT_BASIC_SETTING_STYLE_LIST_PRICE, Container.ItemIndex) %>">ドロップダウン(価格)</option>
																			<option value="<%= Constants.PRODUCTOPTIONVALUES_DISP_KBN_PRICE_CHECKBOX %>" data-toggle-content-selector="<%# GrantNumberToString(CONST_PRODUCT_REGISTER_OPTION_TOGGLE_CONTENT_BASIC_SETTING_STYLE_LIST_PRICE, Container.ItemIndex) %>">チェックボックス(価格)</option>
																		<% } %>
																	</select>
																</div>
															</div>
															<div class="product-register-option-toggle-content-basic-setting-style-checkbox">
																<!-- チェックボックス形式 -->
																<div class="form-element-group">
																	<div class="form-element-group-title">
																		設定値
																	</div>
																	<div class="form-element-group-content">
																		<div class="custom-list-input">
																			<select id="<%# GrantNumberToString("cbDropItems", Container.DataItem) %>" class="custom-list-input-hidden-input"></select>
																		</div>
																	</div>
																</div>
																<div class="form-element-group">
																	<div class="form-element-group-title">
																		<label for="<%# GrantNumberToString("cbNecessaryForCb", Container.DataItem) %>">必須チェック</label>
																	</div>
																	<div class="form-element-group-content">
																		<div class="form-element-group-content-item">
																			<input id="<%# GrantNumberToString("cbNecessaryForCb", Container.DataItem) %>" type="checkbox" class="checkbox" name="<%# GrantNumberToString("cbNecessaryForCb", Container.DataItem) %>" />
																			<label for="<%# GrantNumberToString("cbNecessaryForCb", Container.DataItem) %>">入力/選択を必須にする</label>
																		</div>
																	</div>
																</div>
															</div>
															<div class="product-register-option-toggle-content-basic-setting-style-list">
																<!-- ドロップダウン、チェックボックス形式 -->
																<div class="form-element-group">
																	<div class="form-element-group-title">
																		設定値
																	</div>
																	<div class="form-element-group-content">
																		<div class="custom-list-input">
																			<select id="<%# GrantNumberToString("ddlDropItems", Container.DataItem) %>" class="custom-list-input-hidden-input"></select>
																		</div>
																	</div>
																</div>
																<div class="form-element-group">
																	<div class="form-element-group-title">
																		<label for="<%# GrantNumberToString("cbNecessaryForDdl", Container.DataItem) %>">必須チェック</label>
																	</div>
																	<div class="form-element-group-content">
																		<div class="form-element-group-content-item">
																			<input id="<%# GrantNumberToString("cbNecessaryForDdl", Container.DataItem) %>" type="checkbox" class="checkbox" name="<%# GrantNumberToString("cbNecessaryForDdl", Container.DataItem) %>" />
																			<label for="<%# GrantNumberToString("cbNecessaryForDdl", Container.DataItem) %>">入力/選択を必須にする</label>
																		</div>
																	</div>
																</div>
																<div id="<%# GrantNumberToString("tbInputDefaultArea", Container.DataItem) %>">
																	<div class="form-element-group">
																		<div class="form-element-group-title">
																			<label for="<%# GrantNumberToString("tbInputDefaultForDdl", Container.DataItem) %>">初期表示値</label>
																		</div>
																		<div class="form-element-group-content">
																			<input id="<%# GrantNumberToString("tbInputDefaultForDdl", Container.DataItem) %>" type="text" value="選択してください" />
																		</div>
																	</div>
																	<div class="form-element-group">
																		<div class="form-element-group-title">
																		</div>
																		<div class="form-element-group-content">
																			<p class="note">「初期表示値」は、「必須チェック」が有効のときに初期表示される項目です。選択肢としては利用できないため、ご注意ください。</p>
																		</div>
																	</div>
																</div>
															</div>

															<% if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED) { %>
																<div class="product-register-option-toggle-content-basic-setting-style-list-price">
																	<!-- ドロップダウン(価格)、チェックボックス形式(価格) -->
																	<div class="form-element-group">
																		<div class="custom-list-input-price">
																			<select id="<%# GrantNumberToString("ddlDropItemsPrice", Container.DataItem) %>" class="custom-list-input-hidden-input"></select>
																		</div>
																	</div>
																</div>
															<% } %>
															<div class="product-register-option-toggle-content-basic-setting-style-text">
																<!-- テキスト形式 -->
																<div class="form-element-group">
																	<div class="form-element-group-title">
																		<label for="<%# GrantNumberToString("tbInputDefaultForTb", Container.DataItem) %>">初期値</label>
																	</div>
																	<div class="form-element-group-content">
																		<input id="<%# GrantNumberToString("tbInputDefaultForTb", Container.DataItem) %>" type="text" />
																	</div>
																</div>
																<div class="form-element-group">
																	<div class="form-element-group-title">
																		<label for="<%# GrantNumberToString("ddlCheckType", Container.DataItem) %>">入力チェック種別</label>
																	</div>
																	<div class="form-element-group-content">
																		<select id="<%# GrantNumberToString("ddlCheckType", Container.DataItem) %>" class="js-toggle-form-select">
																			<option value="" selected="selected"></option>
																			<option value="FULLWIDTH" data-toggle-content-selector="<%# GrantNumberToString(CONST_PRODUCT_REGISTER_OPTION_TOGGLE_CONTENT_BASIC_SETTING_STYLE_TEXT, Container.ItemIndex) %>">全角</option>
																			<option value="FULLWIDTH_HIRAGANA" data-toggle-content-selector="<%# GrantNumberToString(CONST_PRODUCT_REGISTER_OPTION_TOGGLE_CONTENT_BASIC_SETTING_STYLE_TEXT, Container.ItemIndex) %>">全角ひらがな</option>
																			<option value="FULLWIDTH_KATAKANA" data-toggle-content-selector="<%# GrantNumberToString(CONST_PRODUCT_REGISTER_OPTION_TOGGLE_CONTENT_BASIC_SETTING_STYLE_TEXT, Container.ItemIndex) %>">全角カタカナ</option>
																			<option value="HALFWIDTH" data-toggle-content-selector="<%# GrantNumberToString(CONST_PRODUCT_REGISTER_OPTION_TOGGLE_CONTENT_BASIC_SETTING_STYLE_TEXT, Container.ItemIndex) %>">半角</option>
																			<option value="HALFWIDTH_NUMBER" data-toggle-content-selector="<%# GrantNumberToString(CONST_PRODUCT_REGISTER_OPTION_TOGGLE_CONTENT_BASIC_SETTING_STYLE_NUMBER, Container.ItemIndex) %>">半角数字</option>
																			<option value="HALFWIDTH_ALPHNUMSYMBOL" data-toggle-content-selector="<%# GrantNumberToString(CONST_PRODUCT_REGISTER_OPTION_TOGGLE_CONTENT_BASIC_SETTING_STYLE_TEXT, Container.ItemIndex) %>">半角英数記号</option>
																			<option value="MAILADDRESS">メールアドレス</option>
																			<option value="DATE">日付</option>
																			<option value="DATE_FUTURE">日付（未来）</option>
																			<option value="DATE_PAST">日付（過去）</option>
																		</select>
																	</div>
																</div>
																<div class="form-element-group">
																	<div class="form-element-group-title">
																		<label for="<%# GrantNumberToString("cbNecessary", Container.DataItem) %>">必須チェック</label>
																	</div>
																	<div class="form-element-group-content">
																		<div class="form-element-group-content-item">
																			<input id="<%# GrantNumberToString("cbNecessary", Container.DataItem) %>" type="checkbox" class="checkbox" name="<%# GrantNumberToString("cbNecessary", Container.DataItem) %>" />
																			<label for="<%# GrantNumberToString("cbNecessary", Container.DataItem) %>">入力/選択を必須にする</label>
																		</div>
																	</div>
																</div>
																<div class="product-register-option-toggle-content-basic-setting-check-style-text">
																	<div class="form-element-group">
																		<div class="form-element-group-title">
																			<label for="<%# GrantNumberToString("rblFixedLength", Container.DataItem) %>">文字数</label>
																		</div>
																		<div class="form-element-group-content">
																			<div class="form-element-group-content-item-inline">
																				<input id="<%# GrantNumberToString(CONST_FIXED_LENGTH_FIRST, Container.DataItem) %>" type="radio" name="<%# GrantNumberToString("rblFixedLength", Container.DataItem) %>" class="radio js-toggle-form-radio" data-toggle-content-selector="<%# GrantNumberToString(CONST_PRODUCT_REGISTER_OPTION_TOGGLE_CONTENT_BASIC_SETTING_CHECK_STYLE_TEXT_RANGE, Container.ItemIndex) %>" checked="checked" />
																				<label for="<%# GrantNumberToString(CONST_FIXED_LENGTH_FIRST, Container.DataItem) %>">範囲指定</label>
																			</div>
																			<div class="form-element-group-content-item-inline">
																				<input id="<%# GrantNumberToString(CONST_FIXED_LENGTH_SECOND, Container.DataItem) %>" type="radio" name="<%# GrantNumberToString("rblFixedLength", Container.DataItem) %>" class="radio js-toggle-form-radio" data-toggle-content-selector="<%# GrantNumberToString(CONST_PRODUCT_REGISTER_OPTION_TOGGLE_CONTENT_BASIC_SETTING_CHECK_STYLE_TEXT_FIXED, Container.ItemIndex) %>" />
																				<label for="<%# GrantNumberToString(CONST_FIXED_LENGTH_SECOND, Container.DataItem) %>">固定</label>
																			</div>
																		</div>
																	</div>
																	<div class="product-register-option-toggle-content-basic-setting-check-style-text-range">
																		<div class="form-element-group">
																			<div class="form-element-group-title">
																				<label for="<%# GrantNumberToString("tbLengthMin", Container.DataItem) %>">入力文字数制限</label>
																			</div>
																			<div class="form-element-group-content">
																				<input id="<%# GrantNumberToString("tbLengthMin", Container.DataItem) %>" type="text" class="w3em number-textbox" maxlength="5" />
																				文字以上～
																				<input id="<%# GrantNumberToString("tbLengthMax", Container.DataItem) %>" type="text" class="w3em number-textbox" maxlength="5" />
																				文字以下
																			</div>
																		</div>
																	</div>
																	<div class="product-register-option-toggle-content-basic-setting-check-style-text-fixed">
																		<div class="form-element-group">
																			<div class="form-element-group-title">
																				<label for="<%# GrantNumberToString("tbFixedLength", Container.DataItem) %>">入力文字数制限</label>
																			</div>
																			<div class="form-element-group-content">
																				<input id="<%# GrantNumberToString("tbFixedLength", Container.DataItem) %>" type="text" class="w3em number-textbox" maxlength="5" />
																				文字
																			</div>
																		</div>
																	</div>
																</div>
																<div class="product-register-option-toggle-content-basic-setting-check-style-number">
																	<div class="form-element-group">
																		<div class="form-element-group-title">
																			<label for="<%# GrantNumberToString("tbNumMin", Container.DataItem) %>">数値範囲</label>
																		</div>
																		<div class="form-element-group-content">
																			<input id="<%# GrantNumberToString("tbNumMin", Container.DataItem) %>" type="text" class="w3em number-textbox" maxlength="5" />
																			～
																			<input id="<%# GrantNumberToString("tbNumMax", Container.DataItem) %>" type="text" class="w3em number-textbox" maxlength="5" />
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
							<div class="product-register-option-toggle-content-advance">
								<div class="product-register-option-toggle-content-advance-input">
									<textarea id="tbProductOptionValueAdvance" cols="30" rows="10"></textarea>
								</div>
								<div class="product-register-option-toggle-content-advance-description">
									<p class="p">
										■商品付帯情報入力形式<br />
										・チェックボックス：[[C@項目名@値1@値2@値3@必須チェック]]<br />
										・ドロップダウン：[[S@項目名@値1@値2@値3@必須チェック]]<br />
										<% if(Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED) { %>
											・チェックボックス(価格)：[[CP@項目名@値1{{価格}}@値2{{価格}}@値3{{価格}}]]<br />
											・ドロップダウン(価格)：[[SP@項目名@値1{{価格}}@値2{{価格}}@値3{{価格}}]]<br />
										<% } %>
										例：<br />
										[[C@@付属品@@電源ケーブル@@USBケーブル@@ストラップ@@Necessary=1]]<br />
										[[S@@股下サイズ@@60cm@@62cm@@64cm@@66cm@@68cm@@70cm@@Necessary=1]]<br />
										<% if(Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED) { %>
											[[CP@@付属品@@電源ケーブル{{200}}@@USBケーブル{{300}}@@ストラップ{{500}}]]<br />
											[[SP@@股下サイズ@@60cm{{111}}@@62cm{{300}}@@64cm{{200}}@@66cm{{100}}]]<br />
										<% } %>
									</p>
									<table class="table-basic">
										<tbody>
											<tr>
												<th>設定項目</th>
												<th>キーワード</th>
												<th>設定値</th>
											</tr>
											<tr>
												<td>初期値</td>
												<td>DefaultValue</td>
												<td>任意の文字列や値</td>
											</tr>
											<tr>
												<td>必須チェック</td>
												<td>Necessary</td>
												<td>0：任意入力<br />
													1：必須入力
												</td>
											</tr>
											<tr>
												<td>入力タイプ</td>
												<td>Type</td>
												<td>FULLWIDTH：&nbsp;全角<br />
													FULLWIDTH_HIRAGANA：&nbsp;全角ひらがな<br />
													FULLWIDTH_KATAKANA：&nbsp;全角カタカナ<br />
													HALFWIDTH：&nbsp;半角<br />
													HALFWIDTH_NUMBER：&nbsp;半角数字<br />
													HALFWIDTH_ALPHNUMSYMBOL：&nbsp;半角英数記号<br />
													MAILADDRESS：&nbsp;メールアドレス<br />
													DATE：&nbsp;日付<br />
													DATE_FUTURE：&nbsp;日付（未来）<br />
													DATE_PAST：&nbsp;日付（過去）
												</td>
											</tr>
											<tr>
												<td>固定長の長さ</td>
												<td>Length</td>
												<td>任意の値<br />
													※注1, 注2
												</td>
											</tr>
											<tr>
												<td>最小文字列長</td>
												<td>LengthMin</td>
												<td>任意の値<br />
													※注1, 注3
												</td>
											</tr>
											<tr>
												<td>最大文字列長</td>
												<td>LengthMax</td>
												<td>任意の値<br />
													※注1, 注3
												</td>
											</tr>
											<tr>
												<td>最小値</td>
												<td>MinValue</td>
												<td>任意の値<br />
													※注4
												</td>
											</tr>
											<tr>
												<td>最大値</td>
												<td>MaxValue</td>
												<td>任意の値<br />
													※注4
												</td>
											</tr>
										</tbody>
									</table>
									<p class="p">
										注1)&nbsp;入力タイプが全角、全角ひらがな、全角カタカナ、半角、半角英数記号のとき有効<br />
										注2)&nbsp;最小文字列長、最大文字列長を設定する場合は不要<br />
										注3)&nbsp;固定長の長さを設定する場合は不要<br />
										注4)&nbsp;入力タイプが半角数字のみ有効<br />
										例：<br />
										[[T@@名入れ@@Necessary=0@@Type=FULLWIDTH@@LengthMax=10]]<br />
										[[T@@イニシャル@@Necessary=0@@Type=HALFWIDTH_ALPHNUMSYMBOL@@Length=3]]<br />
										[[T@@背番号@@DefaultValue=20@@Necessary=1@@Type=HALFWIDTH_NUMBER@@MinValue=1@@MaxValue=100]]
									</p>
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
			<div class="block-section block-section-product-register-fixedpurchase is-form-element-toggle">
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
									<label for="ddlFixedPurchaseFlg">定期購入フラグ</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content">
									<select id="ddlFixedPurchaseFlg" class="js-toggle-form-select">
										<option value="<%= Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID %>" selected="selected">不可</option>
										<option value="<%= Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_VALID %>" data-toggle-content-selector=".product-register-teiki-purchase-toggle-content" <%= (this.ProductInput.FixedPurchaseFlg == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_VALID) ? "selected='selected'" : string.Empty %>>可能</option>
										<option value="<%= Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY %>" data-toggle-content-selector=".product-register-teiki-purchase-toggle-content" <%= (this.ProductInput.FixedPurchaseFlg == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY) ? "selected='selected'" : string.Empty %>>定期購入のみ</option>
									</select>
								</div>
							</div>
							<% } %>
							<div class="product-register-teiki-purchase-toggle-content">
								<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING)) { %>
								<div id="divLimitedFixedPurchaseKbn1SettingLoading" class="ajax-loading-for-input">
									<span class="loading-animation-circle-small" />
								</div>
								<div id="divLimitedFixedPurchaseKbn1" class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
									<div class="form-element-group-title break-text-hover" style="width: 220px !important">
										<label for="cbLimitedFixedPurchaseKbn1Setting">定期購入配送間隔月利用不可</label>
										<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING)) { %>
										<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING) %></p>
										<% } %>
									</div>
									<div class="form-element-group-content product-validate-form-element-group-container">
										<div id="divLimitedFixedPurchaseKbn1Setting" class="form-element-group-content"></div>
										<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING %>"></div>
									</div>
								</div>
								<% } %>
								<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING)) { %>
								<div id="divLimitedFixedPurchaseKbn4SettingLoading" class="ajax-loading-for-input">
									<span class="loading-animation-circle-small" />
								</div>
								<div id="divLimitedFixedPurchaseKbn4" class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
									<div class="form-element-group-title break-text-hover" style="width: 220px !important">
										<label for="cbLimitedFixedPurchaseKbn4Setting">定期購入配送間隔週利用不可</label>
										<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING)) { %>
										<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING) %></p>
										<% } %>
									</div>
									<div class="form-element-group-content product-validate-form-element-group-container">
										<div id="divLimitedFixedPurchaseKbn4Setting" class="form-element-group-content"></div>
										<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING %>"></div>
									</div>
								</div>
								<% } %>
								<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING)) { %>
								<div id="divLimitedFixedPurchaseKbn3SettingLoading" class="ajax-loading-for-input">
									<span class="loading-animation-circle-small" />
								</div>
								<div id="divLimitedFixedPurchaseKbn3" class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
									<div class="form-element-group-title break-text-hover" style="width: 220px !important">
										<label for="cbLimitedFixedPurchaseKbn3Setting">定期購入配送間隔利用不可</label>
										<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING)) { %>
										<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING) %></p>
										<% } %>
									</div>
									<div class="form-element-group-content product-validate-form-element-group-container">
										<div id="divLimitedFixedPurchaseKbn3Setting" class="form-element-group-content"></div>
										<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING %>"></div>
									</div>
								</div>
								<% } %>
							</div>
							<div class="product-register-teiki-purchase-toggle-content">
								<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT)) { %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title break-text-hover">
										<label for="tbFixedPurchaseCancelableCount">定期購入解約可能回数（出荷基準）</label>
										<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT)) { %>
										<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT) %></p>
										<% } %>
									</div>
									<div class="form-element-group-content product-validate-form-element-group-container">
										<input id="tbFixedPurchaseCancelableCount" value="<%= this.ProductInput.FixedPurchasedCancelableCount %>" type="text" maxlength="2" class="number-textbox" />回
										<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT %>"></div>
									</div>
								</div>
								<% } %>
								<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT)) { %>
								<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
									<div class="form-element-group-title break-text-hover">
										<label for="tbFixedPurchaseLimitedSkippedCount">定期購入スキップ制限回数</label>
										<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT)) { %>
										<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT) %></p>
										<% } %>
									</div>
									<div class="form-element-group-content product-validate-form-element-group-container">
										<input id="tbFixedPurchaseLimitedSkippedCount" value="<%= this.ProductInput.FixedPurchaseLimitedSkippedCount %>" type="text" maxlength="3" class="number-textbox" />回
										<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT %>"></div>
									</div>
								</div>
								<% } %>
								<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_USER_LEVEL_IDS)) { %>
								<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
									<div class="form-element-group-title break-text-hover">
										<label for="cbLimitedUserLevel">定期購入利用不可ユーザー管理レベル</label>
										<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_USER_LEVEL_IDS)) { %>
										<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_USER_LEVEL_IDS) %></p>
										<% } %>
									</div>
									<div id="divLimitUserLevel" class="form-element-group-content"></div>
									<div id="divLimitUserLevelLoading" class="ajax-loading-for-input">
										<span class="loading-animation-circle-small" />
									</div>
								</div>
								<% } %>

								<% if (Constants.FIXEDPURCHASE_NEXTSHIPPING_OPTION_ENABLED && GetProductDefaultSettingDisplayField(PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_SETTING))
								   { %>
								<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
									<div class="form-element-group-title break-text-hover">
										<label>定期購入2回目以降商品</label>
										<% if (HasProductDefaultSettingComment(PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_SETTING))
										   { %>
										<p class="note"><%: GetProductDefaultSettingComment(PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_SETTING) %></p>
										<% } %>
									</div>
									<div id="divProductJoinNameLoading" class="ajax-loading-for-input" style="display: none;">
										<span class="loading-animation-circle-small" />
									</div>
									<div id="divProductJoinNameContent" class="form-element-group-content product-validate-form-element-group-container">
										<div class="mb10">
											<table id="tblFixedPurchaseNextShippingProductErrorMessages" class="table-basic" style="display: none;">
												<tbody>
													<tr>
														<th align="center" colspan="5">エラーメッセージ</th>
													</tr>
													<tr>
														<td style="border-bottom: none;" colspan="5">
															<div id="divFixedPurchaseNextShippingProductErrorMessages" class="product-error-message-container" data-id="<%= ProductPage.PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_SETTING %>"></div>
														</td>
													</tr>
												</tbody>
											</table>
											<table class="table-basic" style="width: auto;">
												<tbody>
													<tr align="center">
														<th width="15%">検索/クリア</th>
														<th width="20%">商品ID</th>
														<th width="20%">(商品ID+)<br />
															バリエーションID</th>
														<th>商品名</th>
														<th width="15%">数量</th>
													</tr>
													<tr>
														<td align="center" style="white-space: normal;">
															<input id="btnSearchProduct" type="button" class="btn btn-main btn-size-s" value="検索" onclick="openProductList()" />
															<input id="btnDeleteProduct" type="button" class="btn btn-main btn-size-s" value="クリア" onclick="clearProduct()" />
														</td>
														<td>
															<input id="tbFixedPurchaseNextShippingProductId" value="<%= this.ProductInput.FixedPurchaseNextShippingProductId %>" type="text" maxlength="30" style="width: 100% !important;" />
														</td>
														<td style="white-space: normal;">
															<input id="tbFixedPurchaseNextShippingVariationId" value="<%= CreatedVariationIdForDisplay(this.ProductInput.DataSource) %>" type="text" maxlength="20" style="width: 58% !important;" />
															<input id="btnGetProductData" type="button" class="btn btn-main btn-size-s" value="取得" onclick="getProductJoinName('', '')" />
														</td>
														<td class="break-text-hover">
															<label id="lbFixedPurchaseNextShippingProductName"><%: GetProductNameForFixedPurchaseNextShippingSetting(this.ProductInput.DataSource) %></label>
														</td>
														<td style="white-space: normal;">
															<label for="tbFixedPurchaseNextShippingItemQuantity" style="padding-bottom: 2px;">元の商品数 x</label>
															<input id="tbFixedPurchaseNextShippingItemQuantity" value="<%= this.ProductInput.FixedPurchaseNextShippingItemQuantity %>" type="text" class="number-textbox" maxlength="3" style="width: 60% !important;" />
														</td>
													</tr>
												</tbody>
											</table>
										</div>
										<p class="note">
											<div id="divNextShippingItemFixedPurchaseShippingPatternContent" class="form-element-group-content-related-link">
												定期配送パターン :
												<a onclick="openPopupModifyFixedPurchaseSetting()" class="btn btn-txt btn-size-s">
													<label id="lbNextShippingItemFixedPurchaseShippingPattern"></label>
												</a>
												<input id="hfNextShippingItemFixedPurchaseKbn" type="hidden" value="<%= this.ProductInput.NextShippingItemFixedPurchaseKbn %>" />
												<input id="hfNextShippingItemFixedPurchaseSetting1" type="hidden" value="<%= this.ProductInput.NextShippingItemFixedPurchaseSetting %>" />
											</div>
											<div id="divNextShippingItemFixedPurchaseShippingPatternLoading" class="ajax-loading-for-input">
												<span class="loading-animation-circle-small" />
											</div>
											<br />
											※同じ配送種別の商品を指定してください。<br />
											※商品入れ替え時に配送間隔や決済利用の不可設定などは考慮しません。
										</p>
									</div>
								</div>
								<% } else { %>
									<input id="hfNextShippingItemFixedPurchaseKbn" type="hidden" value="<%= this.ProductInput.NextShippingItemFixedPurchaseKbn %>" />
									<input id="hfNextShippingItemFixedPurchaseSetting1" type="hidden" value="<%= this.ProductInput.NextShippingItemFixedPurchaseSetting %>" />
								<% } %>
								<% if (Constants.FIXED_PURCHASE_DISCOUNT_PRICE_OPTION_ENABLE) { %>
								<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
									<div class="form-element-group-title">
										<label for="cbShowFixedPurchaseDiscountSetting">定期購入割引</label>
									</div>
									<div class="form-element-group-content product-validate-form-element-group-container">
										<input id="cbShowFixedPurchaseDiscountSetting" type="checkbox" class="checkbox js-toggle-form" name="sample_checkbox" onchange="setFixedPurchaseDiscountSettingEmpty()" data-toggle-content-selector=".sample-checkbox-toggle-area-2" <%= string.IsNullOrEmpty(this.ProductFixedPurchaseDiscountSetting) ? string.Empty :"checked" %> />
										<label for="cbShowFixedPurchaseDiscountSetting">割引設定する</label>
										<div class="sample-checkbox-toggle-area-2" style="overflow-x: auto;">
											<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_EXTEND %>"></div>
											<table class="table-basic table-discount" style="width: auto; display: table-cell;">
												<tbody>
													<tr>
														<td align="right">
															<input type="button" class="btn btn-main btn-size-s" value="個数列追加" onclick="addNew(true)" />
														</td>
													</tr>
													<tr>
														<td>
															<table id="myTable">
																<tbody></tbody>
															</table>
														</td>
													</tr>
													<tr>
														<td>
															<input type="button" class="btn btn-main btn-size-s" value="回数行追加" onclick="addNew(false)" />
														</td>
													</tr>
												</tbody>
											</table>
										</div>
									</div>
								</div>
								<% } %>
								<% if (Constants.PRODUCT_ORDER_LIMIT_ENABLED && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG)) { %>
								<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
									<div class="form-element-group-title break-text-hover">
										<label for="cbCheckFixedProductOrderLimit">定期商品重複購入制限フラグ</label>
										<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG)) { %>
										<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG) %></p>
										<% } %>
									</div>
									<div class="form-element-group-content">
										<div class="slide-checkbox-wrap">
											<input id="cbCheckFixedProductOrderLimit" type="checkbox" name="valid-flg" data-on-label="有効" data-off-label="無効" <%= (this.ProductInput.CheckFixedProductOrderFlg ==  Constants.FLG_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG_VALID) ? "checked" : string.Empty %> />
											<label for="cbCheckFixedProductOrderLimit" class="slide-checkbox">
												<span class="slide-checkbox-btn"></span>
												<span class="slide-checkbox-label"></span>
											</label>
										</div>
									</div>
								</div>
								<% } %>
								<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG)) { %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title break-text-hover">
										<label for="ddlSubscriptionBoxFlg">頒布会フラグ</label>
										<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG)) { %>
										<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG) %></p>
										<% } %>
									</div>
									<div class="form-element-group-content">
										<select id="ddlSubscriptionBoxFlg" class="js-toggle-form-select">
											<option value="<%= Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_INVALID %>" selected="selected">不可</option>
											<option value="<%= Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_VALID %>" <%= (this.ProductInput.SubscriptionBoxFlg == Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_VALID) ? "selected='selected'" : string.Empty %>>可能</option>
											<option value="<%= Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_ONLY %>" <%= (this.ProductInput.SubscriptionBoxFlg == Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_ONLY) ? "selected='selected'" : string.Empty %>>頒布会のみ</option>
										</select>
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
		<div class="block-section-row">
			<% if (GetDisplayLimitedOptionArea()) { %>
			<div class="block-section block-section-product-register-limit is-form-element-toggle">
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
									<label for="cblLimitedPayment">利用不可決済</label>
									<a onclick="<%= this.CreateOpenPaymentListPopupScript("reLoadLimitPayment") %>" class="btn btn-txt btn-size-s">
										決済種別設定へ<span class="btn-icon-right icon-arrow-out"></span>
									</a>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS) %></p>
									<% } %>
								</div>
								<div id="divLimitedPayment" class="form-element-group-content"></div>
								<div id="divLimitedPaymentLoading" class="ajax-loading-for-input">
									<span class="loading-animation-circle-small" />
								</div>
							</div>
							<% } %>
							<% if (Constants.MEMBER_RANK_OPTION_ENABLED) { %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DISPLAY_MEMBER_RANK)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label for="ddlDisplayMemberRank">閲覧可能会員ランク</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_MEMBER_RANK)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_MEMBER_RANK) %></p>
									<% } %>
								</div>
								<div id="divDisplayMemberRankLoading" class="ajax-loading-for-input">
									<span class="loading-animation-circle-small" />
								</div>
								<div class="form-element-group-content">
									<select id="ddlDisplayMemberRank"></select>
									<div class="form-element-group-content-related-link">
										<a onclick="<%= CreateOpenMemberRankListPopupScript("reloadMemberRanks") %>" class="btn btn-txt btn-size-s">
											会員ランク設定を開く<span class="btn-icon-right icon-arrow-out"></span>
										</a>
									</div>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label for="cbDisplayFixedPurchaseMemberLimit">定期会員限定フラグ（閲覧）</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content">
									<div class="slide-checkbox-wrap">
										<input id="cbDisplayFixedPurchaseMemberLimit" type="checkbox" name="valid-flg" data-on-label="有効" data-off-label="無効" <%= (this.ProductInput.DisplayOnlyFixedPurchaseMemberFlg == Constants.FLG_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG_ON) ? "checked" : string.Empty%> />
										<label for="cbDisplayFixedPurchaseMemberLimit" class="slide-checkbox">
											<span class="slide-checkbox-btn"></span>
											<span class="slide-checkbox-label"></span>
										</label>
									</div>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label for="ddlBuyableMemberRank">販売可能会員ランク</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK) %></p>
									<% } %>
								</div>
								<div id="divBuyableMemberRankLoading" class="ajax-loading-for-input">
									<span class="loading-animation-circle-small" />
								</div>
								<div class="form-element-group-content">
									<select id="ddlBuyableMemberRank"></select>
									<div class="form-element-group-content-related-link">
										<a onclick="<%= CreateOpenMemberRankListPopupScript("reloadMemberRanks") %>" class="btn btn-txt btn-size-s">会員ランク設定を開く<span class="btn-icon-right icon-arrow-out"></span>
										</a>
									</div>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label for="cbBuyableFixedPurchaseMemberLimit">定期会員限定フラグ（購入）</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content">
									<div class="slide-checkbox-wrap">
										<input id="cbBuyableFixedPurchaseMemberLimit" type="checkbox" name="valid-flg" data-on-label="有効" data-off-label="無効" <%= (this.ProductInput.SellOnlyFixedPurchaseMemberFlg == Constants.FLG_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG_ON) ? "checked" : string.Empty %> />
										<label for="cbBuyableFixedPurchaseMemberLimit" class="slide-checkbox">
											<span class="slide-checkbox-btn"></span>
											<span class="slide-checkbox-label"></span>
										</label>
									</div>
								</div>
							</div>
							<% } %>
							<% } %>
							<% if (Constants.PRODUCT_ORDER_LIMIT_ENABLED) { %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label for="cbCheckProductOrderLimit">通常商品重複購入制限フラグ</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content">
									<div class="slide-checkbox-wrap">
										<input id="cbCheckProductOrderLimit" type="checkbox" name="valid-flg" data-on-label="有効" data-off-label="無効" <%= (this.ProductInput.CheckProductOrderFlg ==  Constants.FLG_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG_VALID) ? "checked" : string.Empty %> />
										<label for="cbCheckProductOrderLimit" class="slide-checkbox">
											<span class="slide-checkbox-btn"></span>
											<span class="slide-checkbox-label"></span>
										</label>
									</div>
								</div>
							</div>
							<% } %>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ADD_CART_URL_LIMIT_FLG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label for="cbAddCartUrlLimitFlg">カート投入URL制限フラグ</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_ADD_CART_URL_LIMIT_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_ADD_CART_URL_LIMIT_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content">
									<div class="slide-checkbox-wrap">
										<input id="cbAddCartUrlLimitFlg" type="checkbox" name="valid-flg" data-on-label="有効" data-off-label="無効" <%= (this.ProductInput.AddCartUrlLimitFlg == Constants.FLG_PRODUCT_ADD_CART_URL_LIMIT_FLG_VALID) ? "checked" : string.Empty %> />
										<label for="cbAddCartUrlLimitFlg" class="slide-checkbox" data-popover-message="※バリエーションがある場合、バリエーションのカート投入URL制限フラグを優先します。" data-popover-message-position="top">
											<span class="slide-checkbox-btn"></span>
											<span class="slide-checkbox-label"></span>
										</label>
									</div>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_AGE_LIMIT_FLG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<label for="cbAgeLimitFlg">年齢制限フラグ</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_AGE_LIMIT_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_AGE_LIMIT_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content">
									<div class="slide-checkbox-wrap">
										<input id="cbAgeLimitFlg" type="checkbox" name="valid-flg" data-on-label="有効" data-off-label="無効" <%= (this.ProductInput.AgeLimitFlg == Constants.FLG_PRODUCT_AGE_LIMIT_FLG_VALID) ? "checked" : string.Empty %> />
										<label for="cbAgeLimitFlg" class="slide-checkbox">
											<span class="slide-checkbox-btn"></span>
											<span class="slide-checkbox-label"></span>
										</label>
									</div>
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
			<div class="block-section block-section-product-register-mall is-form-element-toggle">
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
									<label for="tbMallProductId">モール拡張商品ID</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_MALL_EX_PRODUCT_ID)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_MALL_EX_PRODUCT_ID) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<input id="tbMallProductId" value="<%: this.ProductInput.MallExProductId %>" type="text" maxlength="30" class="w10em" />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_MALL_EX_PRODUCT_ID %>"></div>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG)) { %>
								<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label for="cblMallExhibitsConfig">モール出品設定</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG) %></p>
									<% } %>
								</div>
								<div id="divMallExhibitsConfigLoading" class="ajax-loading-for-input">
									<span class="loading-animation-circle-small" />
								</div>
								<div id="divMallExhibitsConfig" class="form-element-group-content"></div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_ANDMALL_RESERVATION_FLG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label for="cbAndMallReservationFlg">＆mall連携予約商品フラグ</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_ANDMALL_RESERVATION_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_ANDMALL_RESERVATION_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content">
									<div class="slide-checkbox-wrap">
										<input id="cbAndMallReservationFlg" type="checkbox" name="valid-flg" data-on-label="有効" data-off-label="無効" <%= (this.ProductInput.AndmallReservationFlg == Constants.FLG_PRODUCT_ANDMALL_RESERVATION_FLG_RESERVATION) ? "checked" : string.Empty %> />
										<label for="cbAndMallReservationFlg" class="slide-checkbox" data-popover-message="※バリエーションがある場合、バリエーションの＆mall連携予約商品フラグを優先します。" data-popover-message-position="top">
											<span class="slide-checkbox-btn"></span>
											<span class="slide-checkbox-label"></span>
										</label>
									</div>
								</div>
							</div>
							<% } %>
							<p class="note">※モール拡張商品IDが空の場合、商品IDが自動で格納されます</p>
						</div>
					</div>
				</div>
			</div>
			<% } %>
			<% if (Constants.DIGITAL_CONTENTS_OPTION_ENABLED && GetDisplayDigitalContentArea()) { %>
			<div class="block-section block-section-product-register-digital-contents is-form-element-toggle">
				<div class="block-section-header">
					<div class="block-section-header-icon"><span class="icon-cloud-download"></span></div>
					<h2 class="block-section-header-txt">デジタルコンテンツ</h2>
				</div>
				<div class="block-section-body">
					<div class="block-section-body-inner-row">
						<div class="block-section-body-inner-col">
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label for="cbDigitalContentsFlg">デジタルコンテンツ商品フラグ</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content">
									<div class="slide-checkbox-wrap">
										<input id="cbDigitalContentsFlg" type="checkbox" name="valid-flg" data-on-label="有効" data-off-label="無効" <%= (this.ProductInput.DigitalContentsFlg == Constants.FLG_PRODUCT_DIGITAL_CONTENTS_FLG_VALID) ? "checked" : string.Empty %> />
										<label for="cbDigitalContentsFlg" class="slide-checkbox">
											<span class="slide-checkbox-btn"></span>
											<span class="slide-checkbox-label"></span>
										</label>
									</div>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DOWNLOAD_URL)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label for="tbDownloadUrl">ダウンロードURL</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_DOWNLOAD_URL)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DOWNLOAD_URL) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<input id="tbDownloadUrl" value="<%: this.ProductInput.DownloadUrl %>" type="text" maxlength="1000" />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_DOWNLOAD_URL %>"></div>
								</div>
							</div>
							<% } %>
						</div>
					</div>
				</div>
			</div>
			<% } %>
			<% if (Constants.USERPRODUCTARRIVALMAIL_OPTION_ENABLED && GetDisplayProductNotificationSettingsArea()) { %>
			<div class="block-section block-section-product-register-notice is-form-element-toggle">
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
									<label for="cbArrivalMailValidFlg">再入荷通知メール有効フラグ</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_ARRIVAL_MAIL_VALID_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_ARRIVAL_MAIL_VALID_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content">
									<div class="slide-checkbox-wrap">
										<input id="cbArrivalMailValidFlg" type="checkbox" name="valid-flg" data-on-label="有効" data-off-label="無効" <%= (this.ProductInput.ArrivalMailValidFlg == Constants.FLG_PRODUCT_ARRIVAL_MAIL_VALID_FLG_VALID) ? "checked" : string.Empty %> />
										<label for="cbArrivalMailValidFlg" class="slide-checkbox">
											<span class="slide-checkbox-btn"></span>
											<span class="slide-checkbox-label"></span>
										</label>
									</div>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_RELEASE_MAIL_VALID_FLG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label for="cbReleaseMailValidFlg">販売開始通知メール有効フラグ</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_RELEASE_MAIL_VALID_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_RELEASE_MAIL_VALID_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content">
									<div class="slide-checkbox-wrap">
										<input id="cbReleaseMailValidFlg" type="checkbox" name="valid-flg" data-on-label="有効" data-off-label="無効" <%= (this.ProductInput.ReleaseMailValidFlg == Constants.FLG_PRODUCT_RELEASE_MAIL_VALID_FLG_VALID) ? "checked" : string.Empty %> />
										<label for="cbReleaseMailValidFlg" class="slide-checkbox">
											<span class="slide-checkbox-btn"></span>
											<span class="slide-checkbox-label"></span>
										</label>
									</div>
								</div>
							</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_RESALE_MAIL_VALID_FLG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label for="cbResaleMailValidFlg">再販売通知メール有効フラグ</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_RESALE_MAIL_VALID_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_RESALE_MAIL_VALID_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content">
									<div class="slide-checkbox-wrap">
										<input id="cbResaleMailValidFlg" type="checkbox" name="valid-flg" data-on-label="有効" data-off-label="無効" <%= (this.ProductInput.ResaleMailValidFlg == Constants.FLG_PRODUCT_RESALE_MAIL_VALID_FLG_VALID) ? "checked" : string.Empty %> />
										<label for="cbResaleMailValidFlg" class="slide-checkbox">
											<span class="slide-checkbox-btn"></span>
											<span class="slide-checkbox-label"></span>
										</label>
									</div>
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
							<% if (Constants.PRODUCT_IMAGE_HEAD_ENABLED == false) { %>
								<div id="comProductImage" class="form-element-group form-element-group-vertical">
									<div id="comProductImageTitle" class="form-element-group-title break-text-hover">
										<label for="form-input-2-1">商品画像</label>
										<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_IMAGE_HEAD)) { %>
										<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_IMAGE_HEAD) %></p>
										<% } %>
										<a href="javascript:void(0)" class="btn btn-txt btn-size-s" onclick="<%= CreateOpenProductSubImageSettingListPopupScript("reloadProductImages") %>">商品サブ画像設定へ<span class="btn-icon-right icon-arrow-out"></span></a>
									</div>
									<div id="comProductImageContent" class="form-element-group-content">
										<div class="product-register-image-selecter">
											<div class="product-register-image-selecter-image-main">
												<div class="product-register-image-selecter-image-main-title">
													メイン画像
												</div>
												<div class="product-register-image-selecter-image-main-thum">
													<img src="" alt="" class="product-register-image-selecter-image-main-thum-img" />
												</div>
												<label class="product-register-image-selecter-image-main-droparea">
													<input type="file" name="" id="product-register-image-selecter-image-main" class="product-register-image-selecter-image-main-input" accept="image/jpg, image/jpeg" />
												</label>
												<div class="product-register-image-selecter-image-main-btns">
													<label for="product-register-image-selecter-image-main" class="product-register-image-selecter-image-main-btn-set btn btn-main btn-size-s">設定する</label>
													<label for="product-register-image-selecter-image-main" class="product-register-image-selecter-image-main-btn-change btn btn-main btn-size-s">変更する</label>
													<a href="javascript:void(0);" class="product-register-image-selecter-image-main-btn-delete btn btn-sub btn-size-s">削除する</a>
												</div>
											</div>
											<div class="product-register-image-selecter-image-sub-wrapper" style="overflow-y: auto; max-height: 20rem; max-width: 30rem"></div>
											<label class="product-register-image-selecter-first-droparea" for="product-register-image-selecter-first">
												<p class="product-register-image-selecter-first-droparea-message-drop">ドロップして貼り付け</p>
												<input type="file" name="" id="product-register-image-selecter-first" class="product-register-image-selecter-first-input" multiple accept="image/jpg, image/jpeg" />
												<span class="product-register-image-selecter-first-droparea-icon icon-clip"></span>
												<p class="product-register-image-selecter-first-droparea-message">
													ファイルをドラッグ＆ドロップして<br />
													ファイルを指定してください。
												</p>
												<div class="product-register-image-selecter-first-droparea-btns">
													<p class="product-register-image-selecter-first-droparea-btns-text">または</p>
													<a href="javascript:void(0);" class="product-register-image-selecter-first-droparea-btn btn btn-main btn-size-s">ファイルを選択</a>
												</div>
											</label>
										</div>
										<font color="red">
											<div id="pImageErrorMessage" style='display: none;'>1度に合計20MB以上の商品画像をアップロードすることはできません。</div>
										</font>
									</div>
									<div id="comProductImageLoading" class="form-element-group-content ajax-loading">
										<span class="loading-animation-circle"></span>
									</div>
								</div>
								<p class="note">
									※アップロードする画像のファイル名は下記ルールで自動設定されます。既に同名の画像が存在する場合は画像が上書きされるためご注意ください。
								</p>
								<div class="product-image-head-explanation">
									(商品画像の格納先　→　"ROOT/Contents/ProductImages/0/")<br />
									<% if (Constants.PRODUCT_SUBIMAGE_SETTING_OPTION_ENABLE) {%>
									(商品サブ画像の格納先　→　"ROOT/Contents/ProductSubImages/0/")<br />
									<% } %>
									<br />
									例：商品IDがtestの場合のファイル名<br />
								
									<table class="image-explanation-table">
										<tr>
											<td class="edit_title_bg" align="center" style="border-top: none; border-left: none;"></td>
											<td class="edit_title_bg" align="center" style="border-top: none; border-left: none;">画像S</td>
											<td class="edit_title_bg" align="center" style="border-top: none; border-left: none;">画像M</td>
											<td class="edit_title_bg" align="center" style="border-top: none; border-left: none;">画像L</td>
											<td class="edit_title_bg" align="center" style="border-top: none; border-left: none; border-right: none;">画像LL</td>
										</tr>
										<tr>
											<td class="edit_item_bg" align="center" style="border-top: none; border-left: none;">商品画像</td>
											<td class="edit_item_bg" align="center" style="font-size:8pt; border-top: none; border-left: none;">test_S.jpg</td>
											<td class="edit_item_bg" align="center" style="font-size:8pt; border-top: none; border-left: none;">test_M.jpg</td>
											<td class="edit_item_bg" align="center" style="font-size:8pt; border-top: none; border-left: none;">test_L.jpg</td>
											<td class="edit_item_bg" align="center" style="font-size:8pt; border-top: none; border-left: none; border-right: none;">test_LL.jpg</td>
										</tr>
										<tr id="trProductSubImage1" runat="server" visible="<%# Constants.PRODUCT_SUBIMAGE_SETTING_OPTION_ENABLE %>">
											<td class="edit_item_bg" align="center" style="border-top: none; border-left: none;">商品サブ画像1</td>
											<td class="edit_item_bg" align="center" style="border-top: none; border-left: none;">なし</td>
											<td class="edit_item_bg" align="center" style="font-size:8pt; border-top: none; border-left: none;">test_sub01_M.jpg</td>
											<td class="edit_item_bg" align="center" style="font-size:8pt; border-top: none; border-left: none;">test_sub01_L.jpg</td>
											<td class="edit_item_bg" align="center" style="font-size:8pt; border-top: none; border-left: none; border-right: none;">test_sub01_LL.jpg</td>
										</tr>
										<tr id="trProductSubImage" runat="server" visible="<%# Constants.PRODUCT_SUBIMAGE_SETTING_OPTION_ENABLE %>">
											<td class="edit_item_bg" align="center" style="font-size:6pt; border-top: none; border-left: none;">・<br />・<br />・</td>
											<td class="edit_item_bg" align="center" style="font-size:6pt; border-top: none; border-left: none;">・<br />・<br />・</td>
											<td class="edit_item_bg" align="center" style="font-size:6pt; border-top: none; border-left: none;">・<br />・<br />・</td>
											<td class="edit_item_bg" align="center" style="font-size:6pt; border-top: none; border-left: none;">・<br />・<br />・</td>
											<td class="edit_item_bg" align="center" style="font-size:6pt; border-top: none; border-left: none; border-right: none;">・<br />・<br />・</td>
										</tr>
										<tr id="trProductSubImage10" runat="server" visible="<%# Constants.PRODUCT_SUBIMAGE_SETTING_OPTION_ENABLE %>">
											<td class="edit_item_bg" align="center" style="border-top: none; border-left: none; border-bottom: none">商品サブ画像10</td>
											<td class="edit_item_bg" align="center" style="border-top: none; border-left: none; border-bottom: none">なし</td>
											<td class="edit_item_bg" align="center" style="font-size:8pt; border-top: none; border-left: none; border-bottom: none">test_sub10_M.jpg</td>
											<td class="edit_item_bg" align="center" style="font-size:8pt; border-top: none; border-left: none; border-bottom: none">test_sub10_L.jpg</td>
											<td class="edit_item_bg" align="center" style="font-size:8pt; border-top: none; border-left: none; border-right: none; border-bottom: none">test_sub10_LL.jpg</td>
										</tr>
									</table>
									<br />
									※コンテンツマネージャで画像アップする時は、以下のように自動リサイズ変換されます。<br />
									test.jpg　→　test_S.jpg、test_M.jpg、test_L.jpg、test_LL.jpg<br />
								</div>
								<p class="note is-form-element-toggle">
									※商品編集時に「PCサイト商品画像自動リサイズ」が有効な状態でメイン画像とサブ画像を入れ替えた場合、リサイズの設定次第で画質が荒くなる場合がございます。その場合は再度オリジナルの画像をアップロードしてください。
								</p>
								<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
									<input id="cbResizeProductImage" type="checkbox" class="checkbox" <%= (this.IsCheckResizeImage) ? "checked" : string.Empty %> />
									<label for="cbResizeProductImage">PCサイト商品画像自動リサイズ</label>
								</div>
							<% } else { %>
								<div class="use-product-image-head product-image-toggle-content-head">
									<div class="form-element-group form-element-group-horizontal-grid">
										<div class="form-element-group-title break-text-hover">
											<label for="tbProductImageHead">商品画像名ヘッダ</label>
										</div>
										<div class="form-element-group-content product-validate-form-element-group-container">
											<input id="tbProductImageHaed" type="text"value="<%= this.ProductInput.ImageHead ?? string.Empty %>" maxlength="50" />
										</div>
									</div>
								</div>
																<div class="product-image-head-explanation">
									(商品画像の格納先　→　"ROOT/Contents/ProductImages/0/")<br />
									<% if (Constants.PRODUCT_SUBIMAGE_SETTING_OPTION_ENABLE) {%>
									(商品サブ画像の格納先　→　"ROOT/Contents/ProductSubImages/0/")<br />
									<% } %>
									<br />
									例：商品画像名ヘッダがtestの場合のファイル名<br />
								
									<table class="image-explanation-table">
										<tr>
											<td class="edit_title_bg" align="center" style="border-top: none; border-left: none;"></td>
											<td class="edit_title_bg" align="center" style="border-top: none; border-left: none;">画像S</td>
											<td class="edit_title_bg" align="center" style="border-top: none; border-left: none;">画像M</td>
											<td class="edit_title_bg" align="center" style="border-top: none; border-left: none;">画像L</td>
											<td class="edit_title_bg" align="center" style="border-top: none; border-left: none; border-right: none;">画像LL</td>
										</tr>
										<tr>
											<td class="edit_item_bg" align="center" style="border-top: none; border-left: none;">商品画像</td>
											<td class="edit_item_bg" align="center" style="font-size:8pt; border-top: none; border-left: none;">test_S.jpg</td>
											<td class="edit_item_bg" align="center" style="font-size:8pt; border-top: none; border-left: none;">test_M.jpg</td>
											<td class="edit_item_bg" align="center" style="font-size:8pt; border-top: none; border-left: none;">test_L.jpg</td>
											<td class="edit_item_bg" align="center" style="font-size:8pt; border-top: none; border-left: none; border-right: none;">test_LL.jpg</td>
										</tr>
										<tr id="tr1" runat="server" visible="<%# Constants.PRODUCT_SUBIMAGE_SETTING_OPTION_ENABLE %>">
											<td class="edit_item_bg" align="center" style="border-top: none; border-left: none;">商品サブ画像1</td>
											<td class="edit_item_bg" align="center" style="border-top: none; border-left: none;">なし</td>
											<td class="edit_item_bg" align="center" style="font-size:8pt; border-top: none; border-left: none;">test_sub01_M.jpg</td>
											<td class="edit_item_bg" align="center" style="font-size:8pt; border-top: none; border-left: none;">test_sub01_L.jpg</td>
											<td class="edit_item_bg" align="center" style="font-size:8pt; border-top: none; border-left: none; border-right: none;">test_sub01_LL.jpg</td>
										</tr>
										<tr id="tr2" runat="server" visible="<%# Constants.PRODUCT_SUBIMAGE_SETTING_OPTION_ENABLE %>">
											<td class="edit_item_bg" align="center" style="font-size:6pt; border-top: none; border-left: none;">・<br />・<br />・</td>
											<td class="edit_item_bg" align="center" style="font-size:6pt; border-top: none; border-left: none;">・<br />・<br />・</td>
											<td class="edit_item_bg" align="center" style="font-size:6pt; border-top: none; border-left: none;">・<br />・<br />・</td>
											<td class="edit_item_bg" align="center" style="font-size:6pt; border-top: none; border-left: none;">・<br />・<br />・</td>
											<td class="edit_item_bg" align="center" style="font-size:6pt; border-top: none; border-left: none; border-right: none;">・<br />・<br />・</td>
										</tr>
										<tr id="tr3" runat="server" visible="<%# Constants.PRODUCT_SUBIMAGE_SETTING_OPTION_ENABLE %>">
											<td class="edit_item_bg" align="center" style="border-top: none; border-left: none; border-bottom: none">商品サブ画像10</td>
											<td class="edit_item_bg" align="center" style="border-top: none; border-left: none; border-bottom: none">なし</td>
											<td class="edit_item_bg" align="center" style="font-size:8pt; border-top: none; border-left: none; border-bottom: none">test_sub10_M.jpg</td>
											<td class="edit_item_bg" align="center" style="font-size:8pt; border-top: none; border-left: none; border-bottom: none">test_sub10_L.jpg</td>
											<td class="edit_item_bg" align="center" style="font-size:8pt; border-top: none; border-left: none; border-right: none; border-bottom: none">test_sub10_LL.jpg</td>
										</tr>
									</table>
									<br />
									※コンテンツマネージャで画像アップする時は、以下のように自動リサイズ変換されます。<br />
									test.jpg　→　test_S.jpg、test_M.jpg、test_L.jpg、test_LL.jpg<br />
								</div>
							<% } %>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_PRODUCT_COLOR_ID)) { %>
								<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
									<div class="form-element-group-title break-text-hover">
										<label for="ddlColorImage">カラー設定</label>
										<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_PRODUCT_COLOR_ID)) { %>
										<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_PRODUCT_COLOR_ID) %></p>
										<% } %>
									</div>
									<div class="form-element-group-content">
										<select name="" id="ddlColorImage" data-popover-message="※カラーを追加・変更したい場合は「Contents/ProductColorImages/」に画像ファイルを設置したうえで、「Xml\ProductColors.xml」に設定を追加してください。" data-popover-message-position="top"></select>
										&nbsp;&nbsp;&nbsp;<span class="product-color-img"><img id="iColorImage" src='' style="vertical-align: middle;" height="25" width="25" alt="" /></span>
										<p class="note">
											フォーマット：&lt;ProductColor id="重複しない任意のID" filename="カラーの画像ファイル名" dispname="カラーの表示名"&gt;&lt;/ProductColor&gt;
										</p>
									</div>
								</div>
							<% } %>
						</div>
						<div class="block-section-body-inner-col">
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label for="form-input-2-3">表示期間<span class="notice">*</span></label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_TO)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_TO) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<uc:DateTimePickerPeriodInput ID="ucDisplay" runat="server" IsNullEndDateTime="true" />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_DISPLAY_FROM %>"></div>
									<div class="product-error-message-container" data-id="<%= ProductInput.CONST_FIELD_PRODUCT_DISPLAY_TO_CHECK %>"></div>
									<script type="text/javascript">
										selectDateTimePeriodModal.setValue(
											'<%= ucDisplay.ClientID %>',
											'<%= GetDate(this.ProductInput.DisplayFrom, StringUtility.ToDateString(DateTime.Now, "yyyy/MM/dd")) %>',
											'<%= GetTime(this.ProductInput.DisplayFrom, "00:00:00") %>',
											'<%= GetDate(this.ProductInput.DisplayTo, string.Empty) %>',
											'<%= GetTime(this.ProductInput.DisplayTo, string.Empty) %>')
									</script>
								</div>
							</div>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label for="form-input-2-4">販売期間<span class="notice">*</span></label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_SELL_TO)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_SELL_TO) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<uc:DateTimePickerPeriodInput ID="ucSell" runat="server" IsNullEndDateTime="true" />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_SELL_FROM %>"></div>
									<div class="product-error-message-container" data-id="<%= ProductInput.CONST_FIELD_PRODUCT_SELL_TO_CHECK %>"></div>
									<script type="text/javascript">
										selectDateTimePeriodModal.setValue(
											'<%= ucSell.ClientID %>',
											'<%= GetDate(this.ProductInput.SellFrom, StringUtility.ToDateString(DateTime.Now, "yyyy/MM/dd")) %>',
											'<%= GetTime(this.ProductInput.SellFrom, "00:00:00") %>',
											'<%= GetDate(this.ProductInput.SellTo, string.Empty) %>',
											'<%= GetTime(this.ProductInput.SellTo, string.Empty) %>')
									</script>
									<div class="form-element-group-content-item">
										<input id="cbDisplaySellFlg" type="checkbox" class="checkbox" name="sample_checkbox" <%= (this.ProductInput.DisplaySellFlg == Constants.FLG_PRODUCT_DISPLAY_SELL_FLG_DISP) ? "checked" : string.Empty %> /><label for="cbDisplaySellFlg">販売期間をフロントサイトに表示する</label>
									</div>
								</div>
							</div>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label for="tbDisplayPriority">表示優先順<span class="notice">*</span></label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_PRIORITY)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_PRIORITY) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<input id="tbDisplayPriority" value="<%: (string.IsNullOrEmpty(this.ProductInput.DisplayPriority) == false) ? this.ProductInput.DisplayPriority : CONST_DISPLAY_PRIORITY_DEFAULT.ToString() %>" type="text" class="w6em number-textbox" data-popover-message="フロントサイトの新着順並び替えの際、表示優先度順降順＋登録日（商品情報作成日）降順で並び替えされます。" data-popover-message-position="top" />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_DISPLAY_PRIORITY %>"></div>
								</div>
							</div>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DISPLAY_KBN)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title break-text-hover">
									<label for="form-input-2-7">商品表示区分</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_KBN)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_KBN) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-register-display-kubun product-validate-form-element-group-container">
									<div class="form-element-group-content-item">
										<input id="rbDisplayKbn1" type="radio" name="rbDisplayKbn" class="radio-card" value="<%= Constants.FLG_PRODUCT_DISPLAY_DISP_ALL %>" checked />
										<label for="rbDisplayKbn1">
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
									<div class="form-element-group-content-item">
										<input id="rbDisplayKbn2" type="radio" name="rbDisplayKbn" class="radio-card" value="<%= Constants.FLG_PRODUCT_DISPLAY_DISP_ONLY_DETAIL %>" <%= (this.ProductInput.DisplayKbn == Constants.FLG_PRODUCT_DISPLAY_DISP_ONLY_DETAIL) ? "checked" : string.Empty %> />
										<label for="rbDisplayKbn2">
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
									<div class="form-element-group-content-item">
										<input id="rbDisplayKbn3" type="radio" name="rbDisplayKbn" class="radio-card" value="<%= Constants.FLG_PRODUCT_DISPLAY_UNDISP_ALL %>" <%= (this.ProductInput.DisplayKbn == Constants.FLG_PRODUCT_DISPLAY_UNDISP_ALL) ? "checked" : string.Empty %> />
										<label for="rbDisplayKbn3">
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
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_DISPLAY_KBN %>"></div>
								</div>
							</div>
							<% } %>
						</div>
					</div>
					<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN)) { %>
					<div class="block-section-body-inner-row">
						<div class="block-section-body-inner-col">
							<div class="form-element-group form-element-group-vertical is-form-element-toggle">
								<div class="form-element-group-title break-text-hover">
									<div class="form-element-group-heading is-form-element-toggle">
										<h3 class="form-element-group-heading-label">商品バリエーション表示方法</h3>
									</div>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<div class="product-register-product-variation-kubun"></div>
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN %>"></div>
								</div>
								<div id="divProductSelectVariationKbnsLoading" class="form-element-group-content ajax-loading">
									<span class="loading-animation-circle"></span>
								</div>
							</div>
						</div>
					</div>
					<% } %>
					<% if (GetDisplayIconArea()) { %>
					<div class="block-section-body-inner-row">
						<div class="block-section-body-inner-col">
							<div class="form-element-group form-element-group-vertical is-form-element-toggle">
								<div class="form-element-group-heading is-form-element-toggle">
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
														<div class="slide-checkbox-wrap">
															<input id="cbIconFlg1" type="checkbox" name="valid-flg" data-on-label="表示する" data-off-label="表示しない" <%= (this.ProductInput.IconFlg1 == Constants.FLG_PRODUCT_ICON_ON) ? "checked" : string.Empty %> />
															<label for="cbIconFlg1" class="slide-checkbox">
																<span class="slide-checkbox-btn"></span>
																<span class="slide-checkbox-label w5em"></span>
															</label>
														</div>
														<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_ICON_FLG1 %>"></div>
													</div>
												</div>
												<div class="form-element-group w85" style="flex-flow: wrap;">
													<div class="form-element-group-title">
														有効期限
													</div>
													<div class="form-element-group-content" style="max-width: 22rem;">
														<div class="select-period">
															<div class="select-period-end">
																<div class="select-period-end-input">
																	<div class="select-period-end-input-date">
																		<span class="select-period-end-input-date-label">日付</span>
																		<input type="text" id="tbIconFlgTermEnd1Date" class="input-datepicker" value="<%= GetDate(this.ProductInput.IconTermEnd1, StringUtility.ToDateString(DateTime.Now.AddYears(1), "yyyy/MM/dd")) %>" maxlength="10" />
																	</div>
																	<div class="select-period-end-input-time">
																		<span class="select-period-end-input-time-label">時間</span>
																		<input type="text" id="tbIconFlgTermEnd1Time" class="input-timepicker" value="<%= GetTime(this.ProductInput.IconTermEnd1, "00:00:00") %>" maxlength="8" />
																	</div>
																	<a href="javascript:void(0)" class="select-period-end-clear btn btn-txt btn-size-s">クリア</a>
																</div>
															</div>
														</div>
													</div>
													<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_ICON_TERM_END1 %>"></div>
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
														<div class="slide-checkbox-wrap">
															<input type="checkbox" name="valid-flg" value="" id="cbIconFlg2" data-on-label="表示する" data-off-label="表示しない" <%= (this.ProductInput.IconFlg2 == Constants.FLG_PRODUCT_ICON_ON) ? "checked" : string.Empty %> />
															<label for="cbIconFlg2" class="slide-checkbox">
																<span class="slide-checkbox-btn"></span>
																<span class="slide-checkbox-label w5em"></span>
															</label>
														</div>
														<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_ICON_FLG2 %>"></div>
													</div>
												</div>
												<div class="form-element-group w85" style="flex-flow: wrap;">
													<div class="form-element-group-title">
														有効期限
													</div>
													<div class="form-element-group-content" style="max-width: 22rem;">
														<div class="select-period">
															<div class="select-period-end">
																<div class="select-period-end-input">
																	<div class="select-period-end-input-date">
																		<span class="select-period-end-input-date-label">日付</span>
																		<input type="text" id="tbIconFlgTermEnd2Date" class="input-datepicker" value="<%= GetDate(this.ProductInput.IconTermEnd2, StringUtility.ToDateString(DateTime.Now.AddYears(1), "yyyy/MM/dd")) %>" maxlength="10" />
																	</div>
																	<div class="select-period-end-input-time">
																		<span class="select-period-end-input-time-label">時間</span>
																		<input type="text" id="tbIconFlgTermEnd2Time" class="input-timepicker" value="<%= GetTime(this.ProductInput.IconTermEnd2, "00:00:00") %>" maxlength="8" />
																	</div>
																	<a href="javascript:void(0)" class="select-period-end-clear btn btn-txt btn-size-s">クリア</a>
																</div>
															</div>
														</div>
													</div>
													<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_ICON_TERM_END2 %>"></div>
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
														<div class="slide-checkbox-wrap">
															<input type="checkbox" name="valid-flg" value="" id="cbIconFlg3" data-on-label="表示する" data-off-label="表示しない" <%= (this.ProductInput.IconFlg3 == Constants.FLG_PRODUCT_ICON_ON) ? "checked" : string.Empty %> />
															<label for="cbIconFlg3" class="slide-checkbox">
																<span class="slide-checkbox-btn"></span>
																<span class="slide-checkbox-label w5em"></span>
															</label>
															<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_ICON_FLG3 %>"></div>
														</div>
													</div>
												</div>
												<div class="form-element-group w85" style="flex-flow: wrap;">
													<div class="form-element-group-title">
														有効期限
													</div>
													<div class="form-element-group-content" style="max-width: 22rem;">
														<div class="select-period">
															<div class="select-period-end">
																<div class="select-period-end-input">
																	<div class="select-period-end-input-date">
																		<span class="select-period-end-input-date-label">日付</span>
																		<input type="text" id="tbIconFlgTermEnd3Date" class="input-datepicker" value="<%= GetDate(this.ProductInput.IconTermEnd3, StringUtility.ToDateString(DateTime.Now.AddYears(1), "yyyy/MM/dd")) %>" maxlength="10" />
																	</div>
																	<div class="select-period-end-input-time">
																		<span class="select-period-end-input-time-label">時間</span>
																		<input type="text" id="tbIconFlgTermEnd3Time" class="input-timepicker" value="<%= GetTime(this.ProductInput.IconTermEnd3, "00:00:00") %>" maxlength="8" />
																	</div>
																	<a href="javascript:void(0)" class="select-period-end-clear btn btn-txt btn-size-s">クリア</a>
																</div>
															</div>
														</div>
													</div>
													<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_ICON_TERM_END3 %>"></div>
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
														<div class="slide-checkbox-wrap">
															<input type="checkbox" name="valid-flg" value="" id="cbIconFlg4" data-on-label="表示する" data-off-label="表示しない" <%= (this.ProductInput.IconFlg4 == Constants.FLG_PRODUCT_ICON_ON) ? "checked" : string.Empty %> />
															<label for="cbIconFlg4" class="slide-checkbox">
																<span class="slide-checkbox-btn"></span>
																<span class="slide-checkbox-label w5em"></span>
															</label>
														</div>
														<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_ICON_FLG4 %>"></div>
													</div>
												</div>
												<div class="form-element-group w85" style="flex-flow: wrap;">
													<div class="form-element-group-title">
														有効期限
													</div>
													<div class="form-element-group-content" style="max-width: 22rem;">
														<div class="select-period">
															<div class="select-period-end">
																<div class="select-period-end-input">
																	<div class="select-period-end-input-date">
																		<span class="select-period-end-input-date-label">日付</span>
																		<input type="text" id="tbIconFlgTermEnd4Date" class="input-datepicker" value="<%= GetDate(this.ProductInput.IconTermEnd4, StringUtility.ToDateString(DateTime.Now.AddYears(1), "yyyy/MM/dd")) %>" maxlength="10" />
																	</div>
																	<div class="select-period-end-input-time">
																		<span class="select-period-end-input-time-label">時間</span>
																		<input type="text" id="tbIconFlgTermEnd4Time" class="input-timepicker" value="<%= GetTime(this.ProductInput.IconTermEnd4, "00:00:00") %>" maxlength="8" />
																	</div>
																	<a href="javascript:void(0)" class="select-period-end-clear btn btn-txt btn-size-s">クリア</a>
																</div>
															</div>
														</div>
													</div>
													<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_ICON_TERM_END4 %>"></div>
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
														<div class="slide-checkbox-wrap">
															<input type="checkbox" name="valid-flg" value="" id="cbIconFlg5" data-on-label="表示する" data-off-label="表示しない" <%= (this.ProductInput.IconFlg5 == Constants.FLG_PRODUCT_ICON_ON) ? "checked" : string.Empty %> />
															<label for="cbIconFlg5" class="slide-checkbox">
																<span class="slide-checkbox-btn"></span>
																<span class="slide-checkbox-label w5em"></span>
															</label>
														</div>
														<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_ICON_FLG5 %>"></div>
													</div>
												</div>
												<div class="form-element-group w85" style="flex-flow: wrap;">
													<div class="form-element-group-title">
														有効期限
													</div>
													<div class="form-element-group-content" style="max-width: 22rem;">
														<div class="select-period">
															<div class="select-period-end">
																<div class="select-period-end-input">
																	<div class="select-period-end-input-date">
																		<span class="select-period-end-input-date-label">日付</span>
																		<input type="text" id="tbIconFlgTermEnd5Date" class="input-datepicker" value="<%= GetDate(this.ProductInput.IconTermEnd5, StringUtility.ToDateString(DateTime.Now.AddYears(1), "yyyy/MM/dd")) %>" maxlength="10" />
																	</div>
																	<div class="select-period-end-input-time">
																		<span class="select-period-end-input-time-label">時間</span>
																		<input type="text" id="tbIconFlgTermEnd5Time" class="input-timepicker" value="<%= GetTime(this.ProductInput.IconTermEnd5, "00:00:00") %>" maxlength="8" />
																	</div>
																	<a href="javascript:void(0)" class="select-period-end-clear btn btn-txt btn-size-s">クリア</a>
																</div>
															</div>
														</div>
													</div>
													<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_ICON_TERM_END5 %>"></div>
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
														<div class="slide-checkbox-wrap">
															<input type="checkbox" name="valid-flg" value="" id="cbIconFlg6" data-on-label="表示する" data-off-label="表示しない" <%= (this.ProductInput.IconFlg6 == Constants.FLG_PRODUCT_ICON_ON) ? "checked" : string.Empty %> />
															<label for="cbIconFlg6" class="slide-checkbox">
																<span class="slide-checkbox-btn"></span>
																<span class="slide-checkbox-label w5em"></span>
															</label>
														</div>
														<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_ICON_FLG6 %>"></div>
													</div>
												</div>
												<div class="form-element-group w85" style="flex-flow: wrap;">
													<div class="form-element-group-title">
														有効期限
													</div>
													<div class="form-element-group-content" style="max-width: 22rem;">
														<div class="select-period">
															<div class="select-period-end">
																<div class="select-period-end-input">
																	<div class="select-period-end-input-date">
																		<span class="select-period-end-input-date-label">日付</span>
																		<input type="text" id="tbIconFlgTermEnd6Date" class="input-datepicker" value="<%= GetDate(this.ProductInput.IconTermEnd6, StringUtility.ToDateString(DateTime.Now.AddYears(1), "yyyy/MM/dd")) %>" maxlength="10" />
																	</div>
																	<div class="select-period-end-input-time">
																		<span class="select-period-end-input-time-label">時間</span>
																		<input type="text" id="tbIconFlgTermEnd6Time" class="input-timepicker" value="<%= GetTime(this.ProductInput.IconTermEnd6, "00:00:00") %>" maxlength="8" />
																	</div>
																	<a href="javascript:void(0)" class="select-period-end-clear btn btn-txt btn-size-s">クリア</a>
																</div>
															</div>
														</div>
													</div>
													<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_ICON_TERM_END6 %>"></div>
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
														<div class="slide-checkbox-wrap">
															<input type="checkbox" name="valid-flg" value="" id="cbIconFlg7" data-on-label="表示する" data-off-label="表示しない" <%= (this.ProductInput.IconFlg7 == Constants.FLG_PRODUCT_ICON_ON) ? "checked" : string.Empty %> />
															<label for="cbIconFlg7" class="slide-checkbox">
																<span class="slide-checkbox-btn"></span>
																<span class="slide-checkbox-label w5em"></span>
															</label>
														</div>
														<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_ICON_FLG7 %>"></div>
													</div>
												</div>
												<div class="form-element-group w85" style="flex-flow: wrap;">
													<div class="form-element-group-title">
														有効期限
													</div>
													<div class="form-element-group-content" style="max-width: 22rem;">
														<div class="select-period">
															<div class="select-period-end">
																<div class="select-period-end-input">
																	<div class="select-period-end-input-date">
																		<span class="select-period-end-input-date-label">日付</span>
																		<input type="text" id="tbIconFlgTermEnd7Date" class="input-datepicker" value="<%= GetDate(this.ProductInput.IconTermEnd7, StringUtility.ToDateString(DateTime.Now.AddYears(1), "yyyy/MM/dd")) %>" maxlength="10" />
																	</div>
																	<div class="select-period-end-input-time">
																		<span class="select-period-end-input-time-label">時間</span>
																		<input type="text" id="tbIconFlgTermEnd7Time" class="input-timepicker" value="<%= GetTime(this.ProductInput.IconTermEnd7, "00:00:00") %>" maxlength="8" />
																	</div>
																	<a href="javascript:void(0)" class="select-period-end-clear btn btn-txt btn-size-s">クリア</a>
																</div>
															</div>
														</div>
													</div>
													<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_ICON_TERM_END7 %>"></div>
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
														<div class="slide-checkbox-wrap">
															<input type="checkbox" name="valid-flg" value="" id="cbIconFlg8" data-on-label="表示する" data-off-label="表示しない" <%= (this.ProductInput.IconFlg8 == Constants.FLG_PRODUCT_ICON_ON) ? "checked" : string.Empty %> />
															<label for="cbIconFlg8" class="slide-checkbox">
																<span class="slide-checkbox-btn"></span>
																<span class="slide-checkbox-label w5em"></span>
															</label>
														</div>
														<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_ICON_FLG8 %>"></div>
													</div>
												</div>
												<div class="form-element-group w85" style="flex-flow: wrap;">
													<div class="form-element-group-title">
														有効期限
													</div>
													<div class="form-element-group-content" style="max-width: 22rem;">
														<div class="select-period">
															<div class="select-period-end">
																<div class="select-period-end-input">
																	<div class="select-period-end-input-date">
																		<span class="select-period-end-input-date-label">日付</span>
																		<input type="text" id="tbIconFlgTermEnd8Date" class="input-datepicker" value="<%= GetDate(this.ProductInput.IconTermEnd8, StringUtility.ToDateString(DateTime.Now.AddYears(1), "yyyy/MM/dd")) %>" maxlength="10" />
																	</div>
																	<div class="select-period-end-input-time">
																		<span class="select-period-end-input-time-label">時間</span>
																		<input type="text" id="tbIconFlgTermEnd8Time" class="input-timepicker" value="<%= GetTime(this.ProductInput.IconTermEnd8, "00:00:00") %>" maxlength="8" />
																	</div>
																	<a href="javascript:void(0)" class="select-period-end-clear btn btn-txt btn-size-s">クリア</a>
																</div>
															</div>
														</div>
													</div>
													<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_ICON_TERM_END8 %>"></div>
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
														<div class="slide-checkbox-wrap">
															<input type="checkbox" name="valid-flg" value="" id="cbIconFlg9" data-on-label="表示する" data-off-label="表示しない" <%= (this.ProductInput.IconFlg9 == Constants.FLG_PRODUCT_ICON_ON) ? "checked" : string.Empty %> />
															<label for="cbIconFlg9" class="slide-checkbox">
																<span class="slide-checkbox-btn"></span>
																<span class="slide-checkbox-label w5em"></span>
															</label>
														</div>
														<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_ICON_FLG9 %>"></div>
													</div>
												</div>
												<div class="form-element-group w85" style="flex-flow: wrap;">
													<div class="form-element-group-title">
														有効期限
													</div>
													<div class="form-element-group-content" style="max-width: 22rem;">
														<div class="select-period">
															<div class="select-period-end">
																<div class="select-period-end-input">
																	<div class="select-period-end-input-date">
																		<span class="select-period-end-input-date-label">日付</span>
																		<input type="text" id="tbIconFlgTermEnd9Date" class="input-datepicker" value="<%= GetDate(this.ProductInput.IconTermEnd9, StringUtility.ToDateString(DateTime.Now.AddYears(1), "yyyy/MM/dd")) %>" maxlength="10" />
																	</div>
																	<div class="select-period-end-input-time">
																		<span class="select-period-end-input-time-label">時間</span>
																		<input type="text" id="tbIconFlgTermEnd9Time" class="input-timepicker" value="<%= GetTime(this.ProductInput.IconTermEnd9, "00:00:00") %>" maxlength="8" />
																	</div>
																	<a href="javascript:void(0)" class="select-period-end-clear btn btn-txt btn-size-s">クリア</a>
																</div>
															</div>
														</div>
													</div>
													<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_ICON_TERM_END9 %>"></div>
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
														<div class="slide-checkbox-wrap">
															<input type="checkbox" name="valid-flg" value="" id="cbIconFlg10" data-on-label="表示する" data-off-label="表示しない" <%= (this.ProductInput.IconFlg10 == Constants.FLG_PRODUCT_ICON_ON) ? "checked" : string.Empty %> />
															<label for="cbIconFlg10" class="slide-checkbox">
																<span class="slide-checkbox-btn"></span>
																<span class="slide-checkbox-label w5em"></span>
															</label>
														</div>
														<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_ICON_FLG10 %>"></div>
													</div>
												</div>
												<div class="form-element-group w85" style="flex-flow: wrap;">
													<div class="form-element-group-title">
														有効期限
													</div>
													<div class="form-element-group-content" style="max-width: 22rem;">
														<div class="select-period">
															<div class="select-period-end">
																<div class="select-period-end-input">
																	<div class="select-period-end-input-date">
																		<span class="select-period-end-input-date-label">日付</span>
																		<input type="text" id="tbIconFlgTermEnd10Date" class="input-datepicker" value="<%= GetDate(this.ProductInput.IconTermEnd10, StringUtility.ToDateString(DateTime.Now.AddYears(1), "yyyy/MM/dd")) %>" maxlength="10" />
																	</div>
																	<div class="select-period-end-input-time">
																		<span class="select-period-end-input-time-label">時間</span>
																		<input type="text" id="tbIconFlgTermEnd10Time" class="input-timepicker" value="<%= GetTime(this.ProductInput.IconTermEnd10, "00:00:00") %>" maxlength="8" />
																	</div>
																	<a href="javascript:void(0)" class="select-period-end-clear btn btn-txt btn-size-s">クリア</a>
																</div>
															</div>
														</div>
													</div>
													<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_ICON_TERM_END10 %>"></div>
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
			<% if(Constants.PRODUCT_CTEGORY_OPTION_ENABLE && GetDisplayCategoryArea()) { %>
			<div class="block-section block-section-product-register-category">
				<div id="comProductCategory" class="block-section-header">
					<div class="block-section-header-icon"><span class="icon-category"></span></div>
					<h2 class="block-section-header-txt break-text-hover">カテゴリ設定</h2>
					<div class="block-section-header-col">
						<a id="lbCategoryListSetting" href="javascript:void(0)" onclick="<%= CreateOpenProductCategoryRegisterPopupScript("initProductCategories") %>" class="btn btn-txt btn-size-s">
							商品カテゴリ設定を開く<span class="btn-icon-right icon-arrow-out"></span>
						</a>
					</div>
				</div>
				<div id="comProductCategoryContent" class="block-section-body">
					<div class="block-section-body-inner-row">
						<div id="divProductCategoryHasData" class="block-section-body-inner-col">
							<div class="form-element-group form-element-group-vertical">
								<div class="custom-category-selector-input">
									<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CATEGORY_ID1)) { %>
									<div class="custom-category-selector-input-selected-items">
										<div class="form-element-group-title w10em break-text-hover">
											カテゴリ1
											<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_CATEGORY_ID1)) { %>
											<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_CATEGORY_ID1) %></p>
											<% } %>
										</div>
										<div class="custom-category-selector-category product-validate-form-element-group-container" data-field-name="<%= Constants.FIELD_PRODUCT_CATEGORY_ID1 %>" data-suggest-list="[]">
											<input id="hfProductCategoryId1" type="hidden" value="<%= this.ProductInput.CategoryId1 %>" class="custom-category-selector-hidden" />
										</div>
									</div>
									<% } %>
									<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CATEGORY_ID2)) { %>
									<div class="custom-category-selector-input-selected-items">
										<div class="form-element-group-title w10em break-text-hover">
											カテゴリ2
											<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_CATEGORY_ID2)) { %>
											<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_CATEGORY_ID2) %></p>
											<% } %>
										</div>
										<div class="custom-category-selector-category product-validate-form-element-group-container" data-field-name="<%= Constants.FIELD_PRODUCT_CATEGORY_ID2 %>" data-suggest-list="[]">
											<input id="hfProductCategoryId2" type="hidden" value="<%= this.ProductInput.CategoryId2 %>" class="custom-category-selector-hidden" />
										</div>
									</div>
									<% } %>
									<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CATEGORY_ID3)) { %>
									<div class="custom-category-selector-input-selected-items">
										<div class="form-element-group-title w10em break-text-hover">
											カテゴリ3
											<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_CATEGORY_ID3)) { %>
											<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_CATEGORY_ID3) %></p>
											<% } %>
										</div>
										<div class="custom-category-selector-category product-validate-form-element-group-container" data-field-name="<%= Constants.FIELD_PRODUCT_CATEGORY_ID3 %>" data-suggest-list="[]">
											<input id="hfProductCategoryId3" type="hidden" value="<%= this.ProductInput.CategoryId3 %>" class="custom-category-selector-hidden" />
										</div>
									</div>
									<% } %>
									<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CATEGORY_ID4)) { %>
									<div class="custom-category-selector-input-selected-items">
										<div class="form-element-group-title w10em break-text-hover">
											カテゴリ4
											<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_CATEGORY_ID4)) { %>
											<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_CATEGORY_ID4) %></p>
											<% } %>
										</div>
										<div class="custom-category-selector-category product-validate-form-element-group-container" data-field-name="<%= Constants.FIELD_PRODUCT_CATEGORY_ID4 %>" data-suggest-list="[]">
											<input id="hfProductCategoryId4" type="hidden" value="<%= this.ProductInput.CategoryId4 %>" class="custom-category-selector-hidden" />
										</div>
									</div>
									<% } %>
									<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CATEGORY_ID5)) { %>
									<div class="custom-category-selector-input-selected-items">
										<div class="form-element-group-title w10em break-text-hover">
											カテゴリ5
											<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_CATEGORY_ID5)) { %>
											<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_CATEGORY_ID5) %></p>
											<% } %>
										</div>
										<div class="custom-category-selector-category product-validate-form-element-group-container" data-field-name="<%= Constants.FIELD_PRODUCT_CATEGORY_ID5 %>" data-suggest-list="[]">
											<input id="hfProductCategoryId5" type="hidden" value="<%= this.ProductInput.CategoryId5 %>" class="custom-category-selector-hidden" />
										</div>
									</div>
									<% } %>
								</div>
							</div>
						</div>
						<div id="divProductCategoryNoData" class="block-section-body-inner-col">
							<div class="block-section-body-setting-guide">
								<p class="block-section-body-setting-guide-text">商品カテゴリが登録されていません。</p>
								<div class="block-section-body-setting-guide-btns">
									<a href="javascript:void(0)" onclick="<%= CreateOpenProductCategoryRegisterPopupScript("initProductCategories") %>" class="btn btn-txt btn-size-s">
										商品カテゴリ設定を開く<span class="btn-icon-right icon-arrow-out"></span>
									</a>
								</div>
							</div>
						</div>
					</div>
				</div>
				<div id="comProductCategoryLoading" class="block-section-body ajax-loading">
					<span class="loading-animation-circle"></span>
				</div>
			</div>
			<% } %>
			<% if (Constants.PRODUCT_BRAND_ENABLED) { %>
			<div class='block-section block-section-product-register-brand <%: Constants.PRODUCT_BRAND_ENABLED ? "": "is-form-element-toggle" %>'>
				<div id="comProductBrandTitle" class="block-section-header">
					<div class="block-section-header-icon"><span class="icon-price-tags"></span></div>
					<h2 class="block-section-header-txt break-text-hover">ブランド設定</h2>
					<div class="block-section-header-col">
						<a id="lbBrandListSetting" href="javascript:void(0)" onclick="<%= CreateOpenProductBrandListPopupScript("initProductBrands") %>" class="btn btn-txt btn-size-s">
							商品ブランド設定を開く<span class="btn-icon-right icon-arrow-out"></span>
						</a>
					</div>
				</div>
				<div id="comProductBrandContent" class="block-section-body">
					<div class="block-section-body-inner-row">
						<div id="divProductBrandHasData" class="block-section-body-inner-col">
							<div class="form-element-group form-element-group-vertical">
								<div class="form-element-group form-element-group-vertical">
									<div class="custom-category-selector-input">
										<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_BRAND_ID1) || (GetDisplayBrandArea() == false)) { %>
										<div class="custom-category-selector-input-selected-items">
											<div class="form-element-group-title w10em break-text-hover">
												ブランド1<span class="notice">*</span>
												<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_BRAND_ID1)) { %>
												<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_BRAND_ID1) %></p>
												<% } %>
											</div>
											<div class="custom-category-selector-brand product-validate-form-element-group-container" data-field-name="<%= Constants.FIELD_PRODUCT_BRAND_ID1 %>" data-suggest-list="[]">
												<input id="hfProductBrandId1" type="hidden" value="<%= this.ProductInput.BrandId1 %>" class="custom-category-selector-hidden" />
											</div>
										</div>
										<% } %>
										<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_BRAND_ID2)) { %>
										<div class="custom-category-selector-input-selected-items">
											<div class="form-element-group-title w10em break-text-hover">
												ブランド2
												<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_BRAND_ID2)) { %>
												<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_BRAND_ID2) %></p>
												<% } %>
											</div>
											<div class="custom-category-selector-brand product-validate-form-element-group-container" data-field-name="<%= Constants.FIELD_PRODUCT_BRAND_ID2 %>" data-suggest-list="[]">
												<input id="hfProductBrandId2" type="hidden" value="<%= this.ProductInput.BrandId2 %>" class="custom-category-selector-hidden" />
											</div>
										</div>
										<% } %>
										<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_BRAND_ID3)) { %>
										<div class="custom-category-selector-input-selected-items">
											<div class="form-element-group-title w10em break-text-hover">
												ブランド3
												<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_BRAND_ID3)) { %>
												<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_BRAND_ID3) %></p>
												<% } %>
											</div>
											<div class="custom-category-selector-brand product-validate-form-element-group-container" data-field-name="<%= Constants.FIELD_PRODUCT_BRAND_ID3 %>" data-suggest-list="[]">
												<input id="hfProductBrandId3" type="hidden" value="<%= this.ProductInput.BrandId3 %>" class="custom-category-selector-hidden" />
											</div>
										</div>
										<% } %>
										<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_BRAND_ID4)) { %>
										<div class="custom-category-selector-input-selected-items">
											<div class="form-element-group-title w10em break-text-hover">
												ブランド4
												<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_BRAND_ID4)) { %>
												<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_BRAND_ID4) %></p>
												<% } %>
											</div>
											<div class="custom-category-selector-brand product-validate-form-element-group-container" data-field-name="<%= Constants.FIELD_PRODUCT_BRAND_ID4 %>" data-suggest-list="[]">
												<input id="hfProductBrandId4" type="hidden" value="<%= this.ProductInput.BrandId4 %>" class="custom-category-selector-hidden" />
											</div>
										</div>
										<% } %>
										<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_BRAND_ID5)) { %>
										<div class="custom-category-selector-input-selected-items">
											<div class="form-element-group-title w10em break-text-hover">
												ブランド5
												<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_BRAND_ID5)) { %>
												<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_BRAND_ID5) %></p>
												<% } %>
											</div>
											<div class="custom-category-selector-brand product-validate-form-element-group-container" data-field-name="<%= Constants.FIELD_PRODUCT_BRAND_ID5 %>" data-suggest-list="[]">
												<input id="hfProductBrandId5" type="hidden" value="<%= this.ProductInput.BrandId5 %>" class="custom-category-selector-hidden" />
											</div>
										</div>
										<% } %>
									</div>
								</div>
							</div>
						</div>
						<div id="divProductBrandNoData" class="block-section-body-inner-col">
							<div class="block-section-body-setting-guide">
								<p class="block-section-body-setting-guide-text">商品ブランドが登録されていません。</p>
								<div class="block-section-body-setting-guide-btns">
									<a href="javascript:void(0)" onclick="<%= CreateOpenProductBrandListPopupScript("initProductBrands") %>" class="btn btn-txt btn-size-s">
										商品ブランド設定を開く<span class="btn-icon-right icon-arrow-out"></span>
									</a>
								</div>
							</div>
						</div>
					</div>
				</div>
				<div id="comProductBrandLoading" class="block-section-body ajax-loading">
					<span class="loading-animation-circle"></span>
				</div>
			</div>
			<% } %>
			<% if (GetDisplayUsArea() || GetDisplayCsArea()) { %>
			<div class="block-section block-section-product-register-related-product is-form-element-toggle">
				<div class="block-section-header">
					<div class="block-section-header-icon"><span class="icon-box"></span></div>
					<h2 class="block-section-header-txt">関連商品</h2>
				</div>
				<div id="divRelatedProductsLoading" class="block-section-body ajax-loading">
					<span class="loading-animation-circle" />
				</div>
				<div id="divRelatedProductsContents" class="block-section-body">
					<div class="block-section-body-inner-row">
						<% if (GetDisplayCsArea()) { %>
						<div class="block-section-body-inner-col">
							<div class="form-element-group form-element-group-vertical">
								<div class="form-element-group-title break-text-hover">
									<label for="form-input-5-1">クロスセル商品を指定する</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS1)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS1) %></p>
									<% } %>
									<a href="javascript:void(0)" class="btn btn-txt btn-size-s" onclick="<%= CreateOpenProductSearchPopupScript("selectProductCrossSell") %>">商品検索へ</a>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS1 %>"></div>
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS2 %>"></div>
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS3 %>"></div>
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS4 %>"></div>
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS5 %>"></div>
									<div class="custom-product-selector">
										<input type="button" class="select-product" style="display: none" />
										<input id="hfRelatedProductIdCs1" type="hidden" value="<%= this.ProductInput.RelatedProductIdCs1 %>" class="custom-product-selector-hidden" />
										<input id="hfRelatedProductIdCs2" type="hidden" value="<%= this.ProductInput.RelatedProductIdCs2 %>" class="custom-product-selector-hidden" />
										<input id="hfRelatedProductIdCs3" type="hidden" value="<%= this.ProductInput.RelatedProductIdCs3 %>" class="custom-product-selector-hidden" />
										<input id="hfRelatedProductIdCs4" type="hidden" value="<%= this.ProductInput.RelatedProductIdCs4 %>" class="custom-product-selector-hidden" />
										<input id="hfRelatedProductIdCs5" type="hidden" value="<%= this.ProductInput.RelatedProductIdCs5 %>" class="custom-product-selector-hidden" />
									</div>
								</div>
							</div>
						</div>
						<% } %>
						<% if (GetDisplayUsArea()) { %>
						<div class="block-section-body-inner-col">
							<div class="form-element-group form-element-group-vertical">
								<div class="form-element-group-title break-text-hover">
									<label for="form-input-5-2">アップセル商品を指定する</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US1)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US1) %></p>
									<% } %>
									<a href="javascript:void(0)" class="btn btn-txt btn-size-s" onclick="<%= CreateOpenProductSearchPopupScript("selectProductUpSell") %>">商品検索へ</a>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US1 %>"></div>
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US2 %>"></div>
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US3 %>"></div>
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US4 %>"></div>
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US5 %>"></div>
									<div class="custom-product-selector">
										<input type="button" class="select-product" style="display: none" />
										<input id="hfRelatedProductIdUs1" type="hidden" value="<%= this.ProductInput.RelatedProductIdUs1 %>" class="custom-product-selector-hidden" />
										<input id="hfRelatedProductIdUs2" type="hidden" value="<%= this.ProductInput.RelatedProductIdUs2 %>" class="custom-product-selector-hidden" />
										<input id="hfRelatedProductIdUs3" type="hidden" value="<%= this.ProductInput.RelatedProductIdUs3 %>" class="custom-product-selector-hidden" />
										<input id="hfRelatedProductIdUs4" type="hidden" value="<%= this.ProductInput.RelatedProductIdUs4 %>" class="custom-product-selector-hidden" />
										<input id="hfRelatedProductIdUs5" type="hidden" value="<%= this.ProductInput.RelatedProductIdUs5 %>" class="custom-product-selector-hidden" />
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
									<label for="ddlTaxCategory">商品税率カテゴリ<span class="notice">*</span></label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_TAX_CATEGORY_ID)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_TAX_CATEGORY_ID) %></p>
									<% } %>
								</div>
								<div id="comProductTaxContent" class="form-element-group-content product-validate-form-element-group-container">
									<select id="ddlTaxCategory"></select>
									&nbsp;
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_TAX_CATEGORY_ID %>"></div>
								</div>
								<div id="comProductTaxLoading" class="form-element-group-content ajax-loading" style="padding: 0; background: none; border: none">
									<span class="loading-animation-circle-small" style="width: 18px; height: 18px; float: left"></span>
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
								<div class="form-element-group-content">
									<%: ValueText.GetValueText(Constants.TABLE_PRODUCT,Constants.FIELD_PRODUCT_TAX_INCLUDED_FLG, TaxCalculationUtility.GetPrescribedOrderItemTaxIncludedFlag()) %>
								</div>
							</div>
							<% } %>
							<% if (Constants.MEMBER_RANK_OPTION_ENABLED && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_MEMBER_RANK_DISCOUNT_FLG)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title w10em break-text-hover">
									<label for="cbMemberRankDiscountFlg">会員ランク割引対象</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_MEMBER_RANK_DISCOUNT_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_MEMBER_RANK_DISCOUNT_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content">
									<div class="slide-checkbox-wrap">
										<input id="cbMemberRankDiscountFlg" type="checkbox" name="valid-flg" data-on-label="有効" data-off-label="無効" <%= (this.ProductInput.MemberRankDiscountFlg == Constants.FLG_PRODUCT_MEMBER_RANK_DISCOUNT_FLG_VALID) ? "checked" : string.Empty %> />
										<label for="cbMemberRankDiscountFlg" class="slide-checkbox">
											<span class="slide-checkbox-btn"></span>
											<span class="slide-checkbox-label"></span>
										</label>
									</div>
								</div>
							</div>
							<% } %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title w10em break-text-hover">
									<label for="tbPrice">商品表示価格<span class="notice">*</span></label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_PRICE)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_PRICE) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<input id="tbPrice" value="<%= this.ProductInput.DisplayPrice.ToPriceString() %>" type="text" class="w8em number-textbox" maxlength="7" />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_DISPLAY_PRICE %>"></div>
								</div>
							</div>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title w10em break-text-hover">
									<label for="tbSpecialPrice">商品表示特別価格</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<input id="tbSpecialPrice" value="<%= this.ProductInput.DisplaySpecialPrice.ToPriceString() %>" type="text" class="w8em number-textbox" maxlength="7" />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE %>"></div>
								</div>
							</div>
							<% } %>
							<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title w10em break-text-hover">
									<label for="tbFixedPurchaseFirsttimePrice">定期購入初回価格</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<input id="tbFixedPurchaseFirsttimePrice" value="<%= this.ProductInput.FixedPurchaseFirsttimePrice.ToPriceString() %>" type="text" class="w8em number-textbox" maxlength="7" />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE %>"></div>
								</div>
							</div>
							<% } %>
							<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE)) { %>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title w10em break-text-hover">
									<label for="tbFixedPurchasePrice">定期購入価格</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<input id="tbFixedPurchasePrice" value="<%= this.ProductInput.FixedPurchasePrice.ToPriceString() %>" type="text" class="w8em number-textbox" maxlength="7" />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE %>"></div>
								</div>
							</div>
							<% } %>
							<% if (Constants.MEMBER_RANK_OPTION_ENABLED && GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE)) { %>
							<div class="form-element-group-heading is-form-element-toggle">
								<div class="form-element-group-heading-label">
									<h3 style="display: inline-block;">会員ランク価格</h3>
									<a href="javascript:void(0)" class="btn btn-txt btn-size-s" onclick="<%= CreateOpenMemberRankListPopupScript("reloadMemberRanks") %>">
										会員ランク設定を開く<span class="btn-icon-right icon-arrow-out"></span>
									</a>
								</div>
							</div>
							<div id="comProductMemberRankPrice" class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div class="form-element-group-title w10em break-text-hover">
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE) %></p>
									<% } %>
								</div>
							</div>
							<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">
								<div id="divMemberRankPrice" class="form-element-group-content product-register-member-rank-price-list"></div>
								<div id="divMemberRankPriceLoading" class="form-element-group-content ajax-loading">
									<span class="loading-animation-circle"></span>
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
									<label>ポイント<span class="notice">*</span></label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_POINT1)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_POINT1) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-register-point product-validate-form-element-group-container">
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_POINT_KBN1 %>"></div>
									<div class="form-element-group-content-item">
										<input id="rbKbnPoint1" type="radio" class="radio" name="sample_radio"/>
										<label for="rbKbnPoint1">
											<span class="product-register-point-label-1">購入時商品ごとに</span>
											<input id="tbPoint1" type="text" class="product-register-point-input-text number-textbox" maxlength="7" />
											<span class="product-register-point-label-2">pt</span>
											<span class="product-register-point-label-3">発行する</span>
										</label>
									</div>
									<div class="form-element-group-content-item">
										<input id="rbKbnPercent1" type="radio" class="radio" name="sample_radio" />
										<label for="rbKbnPercent1">
											<span class="product-register-point-label-1">購入時商品ごとに</span>
											<input id="tbPercent1" type="text" class="product-register-point-input-text number-textbox" maxlength="7" />
											<span class="product-register-point-label-2">%</span>
											<span class="product-register-point-label-3">発行する</span>
										</label>
									</div>
									<br>
									<% if(Constants.MEMBER_RANK_OPTION_ENABLED) { %>
									<div class="form-element-group-content-item">
										<input id="cbMemberRankPointExcludeFlg" type="checkbox" class="checkbox" name="sample_checkbox" <%= (this.ProductInput.MemberRankPointExcludeFlg == Constants.FLG_PRODUCT_DISPLAY_SELL_FLG_DISP) ? "checked" : string.Empty %> />
										<label for="cbMemberRankPointExcludeFlg">会員ランクのポイント加算を適用しない</label>
									</div>
									<% } %>
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_POINT1 %>"></div>
									<div class="form-element-group-heading">
										<h3 class="form-element-group-heading-label">定期購入</h3>
									</div>
									<div class="form-element-group-content-item">
										<input id="cbPoint" type="checkbox" class="checkbox js-toggle-form" data-toggle-content-selector=".sample-checkbox-toggle-area-3" name="sample_checkbox" <%= ((string.IsNullOrEmpty(this.ProductInput.PointKbn2) == false) && this.ProductInput.PointKbn2 != Constants.FLG_PRODUCT_POINT_KBN2_INVALID) ? "checked" : string.Empty %> />
										<label for="cbPoint">定期購入ポイント設定する</label>
									</div>
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_POINT_KBN2 %>"></div>
									<div class="form-element-group-content-item sample-checkbox-toggle-area-3" style="display: none">
										<input id="rbKbnPoint2" type="radio" class="radio" name="sample_radio_2" />
										<label for="rbKbnPoint2">
											<span class="product-register-point-label-1">購入時商品ごとに</span>
											<input id="tbPoint2" type="text" class="product-register-point-input-text number-textbox" maxlength="7" />
											<span class="product-register-point-label-2">pt</span>
											<span class="product-register-point-label-3">発行する</span>
										</label>
									</div>
									<div class="form-element-group-content-item sample-checkbox-toggle-area-3" style="display: none">
										<input id="rbKbnPercent2" type="radio" class="radio" name="sample_radio_2" />
										<label for="rbKbnPercent2">
											<span class="product-register-point-label-1">購入時商品ごとに</span>
											<input id="tbPercent2" type="text" class="product-register-point-input-text number-textbox" maxlength="7" />
											<span class="product-register-point-label-2">%</span>
											<span class="product-register-point-label-3">発行する</span>
										</label>
									</div>
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_POINT2 %>"></div>
								</div>
							</div>
							<p class="note">MP管理画面のポイント管理＞基本ルール設定にて、ポイント加算区分が「購入時ポイント発行」、ポイント計算方法が「商品毎」に設定されている場合のみ適用されます。</p>
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
					<div class="block-section-header-col">
						<a id="btnShippingListSetting" href="javascript:void(0)" class="btn btn-txt btn-size-s" onclick="<%= CreateOpenShippingListPopupScript("reloadShippingTypes") %>">
							配送種別設定を開く<span class="btn-icon-right icon-arrow-out"></span>
						</a>
					</div>
				</div>
				<div id="comProductShippingContent" class="block-section-body">
					<div id="divShippingType" class="block-section-body-inner-row">
						<div class="block-section-body-inner-col">
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title w10em break-text-hover">
									<label for="ddlShippingType">配送種別<span class="notice">*</span></label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_SHIPPING_TYPE)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_SHIPPING_TYPE) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<select id="ddlShippingType" onchange="changeShippingType()"></select>
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_SHIPPING_TYPE %>"></div>
								</div>
							</div>
						</div>
						<div class="block-section-body-inner-col">
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title w10em break-text-hover">
									<label for="ddlShippingSize">配送サイズ区分<span class="notice">*</span></label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_SHIPPING_SIZE_KBN)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_SHIPPING_SIZE_KBN) %></p>
									<% } %>
								</div>
								<div id="divShippingSizeLoading" class="ajax-loading-for-input">
									<span class="loading-animation-circle-small" />
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<select id="ddlShippingSize" class="js-toggle-form-select" onchange="changeShippingSize()"></select>
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_SHIPPING_SIZE_KBN %>"></div>
								</div>
							</div>
						</div>
						<div class="block-section-body-inner-col product-size-factor-toggle-content" style="display: none;">
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title w10em break-text-hover">
									<label for="tbProductSizeFactor">商品サイズ係数<span class="notice">*</span></label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_PRODUCT_SIZE_FACTOR)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_PRODUCT_SIZE_FACTOR) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<input id="tbProductSizeFactor" value="<%= this.ProductInput.ProductSizeFactor %>" type="text" class="w8em number-textbox" maxlength="9" />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_PRODUCT_SIZE_FACTOR %>"></div>
								</div>
							</div>
						</div>
						<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
						<div class="block-section-body-inner-col">
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title w10em break-text-hover">
									<label for="tbProductWeightGram">商品重量（g）<span class="notice">*</span></label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_PRODUCT_WEIGHT_GRAM)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_PRODUCT_WEIGHT_GRAM) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<input id="tbProductWeightGram" value="<%= this.ProductInput.ProductWeightGram %>" type="text" class="w8em number-textbox" maxlength="9" />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_PRODUCT_WEIGHT_GRAM %>"></div>
								</div>
							</div>
						</div>
						<% } %>
						<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG)) { %>
						<div class="block-section-body-inner-col">
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title w10em break-text-hover">
									<label for="cbPluralShippingPriceFree">配送料複数個無料</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content">
									<div class="slide-checkbox-wrap">
										<input id="cbPluralShippingPriceFree" type="checkbox" name="valid-flg" data-on-label="有効" data-off-label="無効" <%= (this.ProductInput.PluralShippingPriceFreeFlg !=  Constants.FLG_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG_INVALID) ? "checked" : string.Empty %> />
										<label for="cbPluralShippingPriceFree" class="slide-checkbox">
											<span class="slide-checkbox-btn"></span>
											<span class="slide-checkbox-label"></span>
										</label>
									</div>
								</div>
							</div>
						</div>
						<% } %>
						<% if (Constants.STORE_PICKUP_OPTION_ENABLED
							&& GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_STOREPICKUP_FLG)) { %>
						<div class="block-section-body-inner-col">
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title w10em break-text-hover">
									<label for="cbStorePickupFlg">店舗受取可能フラグ</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_STOREPICKUP_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_STOREPICKUP_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content">
									<div class="slide-checkbox-wrap">
										<input
											id="cbStorePickupFlg"
											type="checkbox"
											name="valid-flg"
											data-on-label="有効"
											data-off-label="無効"
											<%= (this.ProductInput.StorePickupFlg == Constants.FLG_ON) ? "checked" : string.Empty %> />
										<label for="cbStorePickupFlg" class="slide-checkbox">
											<span class="slide-checkbox-btn"></span>
											<span class="slide-checkbox-label"></span>
										</label>
									</div>
								</div>
							</div>
						</div>
						<% } %>
						<% if (Constants.FREE_SHIPPING_FEE_OPTION_ENABLED
							&& GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG)) { %>
						<div class="block-section-body-inner-col">
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title w10em break-text-hover">
									<label for="cbExcludeFreeShippingFlg">配送料無料適用外</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content">
									<div class="slide-checkbox-wrap">
										<input
											id="cbExcludeFreeShippingFlg"
											type="checkbox"
											name="valid-flg"
											data-on-label="有効"
											data-off-label="無効"
											<%= (this.ProductInput.ExcludeFreeShippingFlg == Constants.FLG_ON) ? "checked" : string.Empty %> />
										<label for="cbExcludeFreeShippingFlg" class="slide-checkbox">
											<span class="slide-checkbox-btn"></span>
											<span class="slide-checkbox-label"></span>
										</label>
									</div>
									<p class="note">※配送種別の「配送料無料時の請求料金設定（特定商品のみ）」の金額が請求されます。</p>
								</div>
							</div>
						</div>
						<% } %>
					</div>
					<div id="divShippingTypeNodata" class="block-section-body-inner-row">
						<div class="block-section-body-inner-col">
							<div class="block-section-body-setting-guide">
								<p class="block-section-body-setting-guide-text">配送種別が登録されていません。</p>
								<div class="block-section-body-setting-guide-btns">
									<a href="javascript:void(0)" class="btn btn-txt btn-size-s" onclick="<%= CreateOpenShippingListPopupScript("reloadShippingTypes") %>">
										配送種別設定を開く<span class="btn-icon-right icon-arrow-out"></span>
									</a>
								</div>
							</div>
							<div id="divMessageNoShipping" tabindex="-1" style="color: red;"></div>
						</div>
					</div>
				</div>
				<div id="comProductShippingLoading" class="block-section-body ajax-loading">
					<span class="loading-animation-circle"></span>
				</div>
			</div>
			<% } %>
			<% if (Constants.PRODUCT_TAG_OPTION_ENABLE) { %>
			<div class="block-section block-section-product-register-tags is-form-element-toggle">
				<div id="comProductTagTitle" class="block-section-header">
					<div class="block-section-header-icon"><span class="icon-box"></span></div>
					<h2 class="block-section-header-txt">商品タグ</h2>
					<div class="block-section-header-col">
						<a id="lbTagListSetting" class="btn btn-txt btn-size-s" href="javascript:void(0)" onclick="<%= CreateOpenProductTagSettingPopupScript("reloadProductTags") %>">
							商品タグ設定を開く<span class="btn-icon-right icon-arrow-out"></span>
						</a>
					</div>
				</div>
				<div id="comProductTagContent" class="block-section-body">
					<div class="block-section-body-inner-row">
						<div id="divProductTag" class="block-section-body-inner-col"></div>
					</div>
				</div>
				<div id="comProductTagLoading" class="block-section-body ajax-loading">
					<span class="loading-animation-circle"></span>
				</div>
			</div>
			<% } %>
			<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_SEO_KEYWORDS)) { %>
			<div class="block-section block-section-product-register-seo">
				<div class="block-section-header">
					<div class="block-section-header-icon"><span class="icon-analysis"></span></div>
					<h2 class="block-section-header-txt">SEO設定</h2>
					<div class="block-section-header-col">
						<a href="javascript:void(0)" class="btn btn-txt btn-size-s" onclick="<%: CreateOpenSeoMetadatasModifyPopupScript() %>">SEOタグ設定を開く<span class="btn-icon-right icon-arrow-out"></span>
						</a>
					</div>
				</div>
				<div class="block-section-body">
					<div class="block-section-body-inner-row">
						<div class="block-section-body-inner-col">
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title w10em break-text-hover">
									<label for="tbSeoKeywords">SEOキーワード設定<span class="notice"></span></label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_SEO_KEYWORDS)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_SEO_KEYWORDS) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<input type="text" id="tbSeoKeywords" value="<%: this.ProductInput.SeoKeywords %>" maxlength="200" />
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_SEO_KEYWORDS %>"></div>
									<p class="note">デフォルトでは商品名/カテゴリがキーワードに設定されています。それ以外のキーワードを追加する場合に登録します。（","カンマ区切りで複数指定可。）</p>
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
									<label for="ddlStockManagementKbn">在庫管理方法</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN) %></p>
									<% } %>
								</div>
								<div id="stockManagementKbnLoading" class="ajax-loading-for-input">
									<span class="loading-animation-circle-small" />
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<select id="ddlStockManagementKbn" class="js-toggle-form-select"></select>
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN %>"></div>
								</div>
							</div>
							<div class="product-register-stock-management-toggle-content">
								<div class="form-element-group form-element-group-horizontal-grid" style="<%: (this.ActionStatus == Constants.ACTION_STATUS_UPDATE) ? "display:none;" : "" %>">
									<div class="form-element-group-title w10em">
										<label for="form-input-12-2">在庫管理</label>
									</div>
									<div class="form-element-group-content">
										<div class="product-register-stock-management product-validate-form-element-group-container">
											<div class="product-register-stock-management-header">
												<p class="note">バリエーションを登録している場合それらに対しても同在庫数として在庫数が更新されます。</p>
											</div>
											<div class="product-error-message-container" data-id="<%= ProductInput.CONST_ERROR_KEY_PRODUCT_STOCKS %>"></div>
											<div class="product-register-stock-management-body product-validate-form-element-group-container">
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
																<div class="count-input">
																	<input id="tbStock" type="text" value="<%= ObjectUtility.TryParseInt(this.ProductInput.Stock, 0) %>" class="count-input-input" />
																	<div class="count-input-btns">
																		<a href="javascript:void(0)" class="btn btn-size-s btn-main count-input-btn-up"><span class="icon-plus"></span></a>
																		<a href="javascript:void(0)" class="btn btn-size-s btn-main count-input-btn-down"><span class="icon-minus"></span></a>
																	</div>
																</div>
															</div>
														</div>
														<dl class="product-register-stock-management-current">
															<dt>現在</dt>
															<dd class="product-register-stock-management-current-value">0</dd>
															<dd class="product-register-stock-management-after-change"><span class="icon icon-arrow-right2"></span><span class="product-register-stock-management-after-change-value">0</span></dd>
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
																<div class="count-input">
																	<input id="tbStockAlert" type="text" value="<%= ObjectUtility.TryParseInt(this.ProductInput.StockAlert, 0) %>" class="count-input-input" />
																</div>
															</div>
														</div>
														<dl class="product-register-stock-management-current">
															<dt>現在</dt>
															<dd class="product-register-stock-management-current-value">0</dd>
															<dd class="product-register-stock-management-after-change"><span class="icon icon-arrow-right2"></span><span class="product-register-stock-management-after-change-value">0</span></dd>
														</dl>
													</div>
												</div>
												<div class="product-register-stock-management-block">
													<div class="product-register-stock-management-block-title">
														更新メモ
													</div>
													<div class="product-register-stock-management-block-content">
														<textarea id="tbUpdateMemo" rows="2"><%: this.ProductInput.UpdateMemo %></textarea>
													</div>
												</div>
											</div>
											<div class="product-register-stock-management-footer">
												<%--<a href="#" class="btn btn-txt btn-size-s">履歴一覧<span class="btn-icon-right icon-arrow-out"></span></a>--%>
											</div>
										</div>
									</div>
								</div>
							</div>
							<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_STOCK_MESSAGE_ID)) { %>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title w10em break-text-hover">
									<label for="ddlStockMessage">商品在庫文言</label>
									<% if (HasProductDefaultSettingComment(Constants.FIELD_PRODUCT_STOCK_MESSAGE_ID)) { %>
									<p class="note"><%: GetProductDefaultSettingComment(Constants.FIELD_PRODUCT_STOCK_MESSAGE_ID) %></p>
									<% } %>
								</div>
								<div class="form-element-group-content product-validate-form-element-group-container">
									<select id="ddlStockMessage"></select>
									<div class="form-element-group-content-related-link">
										<a href="javascript:void(0);" onclick="<%= CreateOpenProductStockMessageListPopupScript("reloadStockMessages") %>"
											class="btn btn-txt btn-size-s">在庫文言設定<span class="btn-icon-right icon-arrow-out"></span>
										</a>
									</div>
									<div class="product-error-message-container" data-id="<%= Constants.FIELD_PRODUCT_STOCK_MESSAGE_ID %>"></div>
								</div>
							</div>
							<% } %>
						</div>
					</div>
				</div>
				<div id="comProductStockLoading" class="block-section-body ajax-loading">
					<span class="loading-animation-circle"></span>
				</div>
			</div>
			<% } %>
			<div class="block-section block-section-product-register-variation">
				<div class="block-section-header">
					<div class="block-section-header-icon"><span class="icon-box"></span></div>
					<h2 class="block-section-header-txt">商品バリエーション</h2>
				</div>
				<div class="block-section-body">
					<div class="block-section-body-inner-row">
						<div class="block-section-body-inner-col">
							<div class="product-register-variation">
								<div class="product-register-variation-btns" style="text-align: left;">
									<% if ((Constants.SUBSCRIPTION_BOX_OPTION_ENABLED && CheckVariationIdIncludeSubscriptionBox(this.ProductInput.ProductId)
											&& ((this.ProductInput.SubscriptionBoxFlg == Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_VALID) 
											|| (this.ProductInput.SubscriptionBoxFlg == Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_ONLY))) == false
											|| (this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT))
										{ %>
										<a href="javascript:void(0);" class="btn btn-main btn-size-m product-register-variation-add">追加</a>
										<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
												<a href="javascript:void(0)" class="btn btn-main btn-size-m" onclick="<%= CreateOpenProductVariationMatrixRegisterPopupScript(string.Empty) %>">一括入力支援</a>
										<% } %>
									<% } else { %>
											<p style="color:RED">頒布会利用中のため、バリエーションの追加は行えません。</p>
									<% } %>
								</div>
								<div class="product-register-variation-form"></div>
							</div>
						</div>
					</div>
				</div>
			</div>
		</div>
		<div class="block-section-all-show-btn">
			<a href="javascript:void(0);" class="btn btn-sub btn-size-m block-section-all-show-btn-a js-block-section-all-show-btn-a">すべての設定項目を表示する</a>
		</div>
	</div>
	<div id="divSubmitLoading" class="modal modal-size-s" style="display: none;">
		<div class="modal-bg"></div>
		<div class="modal-content-wrapper">
			<div class="modal-content">
				<div class="modal-inner">
					<div class="submit-loading" style="text-align: center;">
						<span class="loading-animation-circle"></span>
						<p style="font-size: x-large; margin-top: 15px;">読み込み中...</p>
					</div>
				</div>
			</div>
		</div>
	</div>
<script type="text/javascript">
		var delete_iamge_confirm_disp_text = "画像を削除します。よろしいですか？";
		var insert_here_disp_text = "ここに挿入する";
		var missingErrorMessage = ''
		var openedPopup = [];
		var productFixedPurchaseDiscountSettingList = [];
		var selectedProductId = '';
		var selectedVatiationId = '';
		var selectedProductName = '';
		var products = [];
		var isShowFixedPurchaseKbn1Setting = false;
		var isShowFixedPurchaseKbn3Setting = false;
		var isShowFixedPurchaseKbn4Setting = false;
		var productImages = [];
		var variationImages = [];
		var mallExhibitsConfigs = [];
		var productFixedPurchaseDiscount = {
			ProductCount: '.product-count',
			ProductCountForm: '.product-count-form',
			ProductCountColNo: '.product-count-col-no',
			OrderCount: '.order-count',
			OrderCountMore: '.order-count-more',
			OrderCountForm: '.order-count-form',
			PriceDiscount: '.price-discount',
			PriceDiscountType: '.price-discount-type',
			PriceDiscountForm: '.price-discount-form',
			PriceDiscountColNo: '.price-discount-col-no',
			PointDiscount: '.point-discount',
			PointDiscountType: '.point-discount-type',
			PointDiscountForm: '.point-discount-form',
			PointDiscountColNo: '.point-discount-col-no',

			// Get point discount
			getPointDiscount: function() {
				var _this = this;
				var results = [];
				$(_this.PointDiscountForm).each(function() {
					var wrapper = $(this);
					var point = wrapper.find(_this.PointDiscount).val();
					var type = wrapper.find(_this.PointDiscountType).val();
					var colNo = wrapper.find(_this.PointDiscountColNo).val();
					var result = {colNo: colNo, point: point, type: type};
					results.push(result);
				});
				return results;
			},

			// Get price discount
			getPriceDiscount: function() {
				var _this = this;
				var results = [];
				$(_this.PriceDiscountForm).each(function() {
					var wrapper = $(this);
					var price = wrapper.find(_this.PriceDiscount).val();
					var type = wrapper.find(_this.PriceDiscountType).val();
					var colNo = wrapper.find(_this.PriceDiscountColNo).val();
					var result = {colNo: colNo, price: price, type: type};
					results.push(result);
				});
				return results;
			},

			// Get product count list
			getProductCountList: function() {
				var _this = this;
				var results = [];
				$(_this.ProductCountForm).each(function() {
					var wrapper = $(this);
					var productCount = wrapper.find(_this.ProductCount).val();
					var colNo = wrapper.find(_this.ProductCountColNo).val();
					var result = {colNo: colNo, productCount: productCount};
					results.push(result);
				});
				return results;
			},

			// Get order count list
			getOrderCountList: function() {
				var _this = this;
				var results = [];
				$(_this.OrderCountForm).each(function() {
					var wrapper = $(this);
					var orderCount = wrapper.find(_this.OrderCount).val();
					var isMore = wrapper.find(_this.OrderCountMore).is(':checked')
						? '<%= Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_ORDER_COUNT_MORE_THAN_FLG_VALID %>'
						: '<%= Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_ORDER_COUNT_MORE_THAN_FLG_INVALID %>';
					var result = {orderCount: orderCount, isMore: isMore};
					results.push(result);
				});
				return results;
			}
		};

		// Initialzie loading components
		var loadingComponents = {
			productImage: {
				title: $('#comProductImageTitle'),
				content: $('#comProductImageContent'),
				loading: $('#comProductImageLoading'),
				isLoading: false
			},
			productCategory: {
				title: $('#comProductCategoryTitle'),
				content: $('#comProductCategoryContent'),
				loading: $('#comProductCategoryLoading'),
				isLoading: false
			},
			productBrand: {
				title: $('#comProductBrandTitle'),
				content: $('#comProductBrandContent'),
				loading: $('#comProductBrandLoading'),
				isLoading: false
			},
			productMemberRankPrice: {
				content: $('#divMemberRankPrice'),
				loading: $('#divMemberRankPriceLoading'),
				isLoading: false
			},
			productShipping: {
				title: $('#comProductShippingTitle'),
				content: $('#comProductShippingContent'),
				loading: $('#comProductShippingLoading'),
				isLoading: false
			},
			productTag: {
				title: $('#comProductTagTitle'),
				content: $('#comProductTagContent'),
				loading: $('#comProductTagLoading'),
				isLoading: false
			},
			productStock: {
				title: $('#comProductStockTitle'),
				content: $('#comProductStockContent'),
				loading: $('#comProductStockLoading'),
				isLoading: false
			},
			productTax: {
				content: $('#comProductTaxContent'),
				loading: $('#comProductTaxLoading'),
				isLoading: false
			},
			stockManagementKbn: {
				content: $('#ddlStockManagementKbn'),
				loading: $('#stockManagementKbnLoading'),
				isLoading: false
			},
			productSelectVariationKbns: {
				content: $('.product-register-product-variation-kubun'),
				loading: $('#divProductSelectVariationKbnsLoading'),
				isLoading: false
			},
			displayMemberRank: {
				content: $('#ddlDisplayMemberRank'),
				loading: $('#divDisplayMemberRankLoading'),
				isLoading: false
			},
			buyableMemberRank: {
				content: $('#ddlBuyableMemberRank'),
				loading: $('#divBuyableMemberRankLoading'),
				isLoading: false
			},
			limitUserLevel: {
				content: $('#divLimitUserLevel'),
				loading: $('#divLimitUserLevelLoading'),
				isLoading: false
			},
			limitedPayment: {
				content: $('#divLimitedPayment'),
				loading: $('#divLimitedPaymentLoading'),
				isLoading: false
			},
			shippingSize: {
				content: $('#ddlShippingSize'),
				loading: $('#divShippingSizeLoading'),
				isLoading: false
			},
			mallExhibitsConfig: {
				content: $('#divMallExhibitsConfig'),
				loading: $('#divMallExhibitsConfigLoading'),
				isLoading: false
			},
			productJoinName: {
				content: $('#divProductJoinNameContent'),
				loading: $('#divProductJoinNameLoading'),
				isLoading: false
			},
			nextShippingItemFixedPurchaseShippingPattern: {
				content: $('#divNextShippingItemFixedPurchaseShippingPatternContent'),
				loading: $('#divNextShippingItemFixedPurchaseShippingPatternLoading'),
				isLoading: false
			},
			limitedFixedPurchaseKbn1: {
				content: $('#divLimitedFixedPurchaseKbn1'),
				loading: $('#divLimitedFixedPurchaseKbn1SettingLoading'),
				isLoading: false
			},
			limitedFixedPurchaseKbn4: {
				content: $('#divLimitedFixedPurchaseKbn4'),
				loading: $('#divLimitedFixedPurchaseKbn4SettingLoading'),
				isLoading: false
			},
			limitedFixedPurchaseKbn3: {
				content: $('#divLimitedFixedPurchaseKbn3'),
				loading: $('#divLimitedFixedPurchaseKbn3SettingLoading'),
				isLoading: false
			},
			relatedProducts: {
				content: $('#divRelatedProductsContents'),
				loading: $('#divRelatedProductsLoading'),
				isLoading: false
			}
		};

		// Call ajax
		function callAjax(url, data) {
			var request = $.ajax({
				type: "POST",
				url: url,
				data: data,
				contentType: "application/json; charset=utf-8",
				dataType: "json",
				error: pageReload
			});
			return request;
		}

		// Check ajax loading
		function checkAjaxLoading() {
			var isLoading = false;
			Object.keys(loadingComponents).forEach(function (key) {
				if (loadingComponents[key].isLoading === true) {
					isLoading = true;
				}
			})

			if (isLoading) {
				$('#btnSubmit').attr('disabled', true);
			} else {
				$('#btnSubmit').attr('disabled', false);
			}
		}

		// call ajax with form data
		function callAjaxWithFormData(url, formData) {
			var request = $.ajax({
				type: "POST",
				url: url,
				data: formData,
				contentType: false,
				processData: false,
				error: pageReload,
			});
			return request;
		}

		// Page reload
		function pageReload(xmlHttpRequest, status, error) {
			// Reload page when login session expired
			if (xmlHttpRequest.status == 401) {
				window.location.reload();
			} else {
				if ((xmlHttpRequest.responseText === '') || (xmlHttpRequest.responseText === undefined)) return;

				// Notification error and write log
				notification.show('<%: WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PROCESS_PROCESSING_IS_INTERRUPTED) %>', 'warning', 'fadeout');
			}
		}

		// Open popup
		function openPopup(link, name, type, callback) {
			var popup = window.open(link, name, type);
			if (openedPopup.indexOf(name) != -1) return;

			openedPopup.push(name);

			var timer = setInterval(function() {
				if (popup.closed) {
					clearInterval(timer);
					openedPopup.splice(openedPopup.indexOf(name), 1);

					if (callback) callback();
				}
			});
		}

		// Action variation setting
		function action_variation_setting(price, sale_price, variation_type, variation_col_id, variation_col_name, variation_row_id, variation_row_name, replace_or_add) {
			var variationColIds = variation_col_id.split(' ');
			var variationColNames = variation_col_name.split(' ');
			var variationRowIds = variation_row_id.split(' ');
			var variationRowNames = variation_row_name.split(' ');

			if (replace_or_add == "replace") {
				productRegisterVariation.clear();
				productRegisterVariation.index = 0;
			}

			for (var index = 0; index < variationColIds.length; index++) {
				$('.product-register-variation-add').click();

				var variationIndex = productRegisterVariation.index - 1;
				var variation = $(productRegisterVariation.formSelector).find(productRegisterVariation.formRow + '[data-index="' + variationIndex + '"]');

				var colId = variationColIds[index] ? variationColIds[index] : '';
				var rowId = variationRowIds[index] ? variationRowIds[index] : '';
				variation.find('#tbVariationId' + variationIndex).val(colId + rowId);
				variation.find('#tbVariationName1' + variationIndex).val(variationColNames[index]);
				variation.find('#tbVariationName2' + variationIndex).val(variationRowNames[index]);

				variation.find('#tbVariationPrice' + variationIndex).val(price);
				variation.find('#tbVariationSpecialPrice' + variationIndex).val(sale_price);

				var mallVarType = '';
				switch (variation_type) {
					case 'rakuten_i':
						mallVarType = '<%= Constants.FLG_PRODUCTVARIATION_MALL_VARIATION_TYPE_MATRIX %>';
						break;
					case 'rakuten_s':
						mallVarType = '<%= Constants.FLG_PRODUCTVARIATION_MALL_VARIATION_TYPE_DROPDOWN %>';
						break;
					case 'rakuten_c':
						mallVarType = '<%= Constants.FLG_PRODUCTVARIATION_MALL_VARIATION_TYPE_CHECKBOX %>';
						break;
				}
				variation.find('#tbMallVariationType' + variationIndex).val(mallVarType);

				variation.find('#tbMallVariationId1' + variationIndex).val(colId);
				variation.find('#tbMallVariationId2' + variationIndex).val(rowId);

				var displayOrder = getProductVariationDisplayOrder();
				variation.find('#tbDisplayOrder' + variationIndex).val(displayOrder);
			}
		}

		// Get product variation display order
		function getProductVariationDisplayOrder() {
			var displayOrder = 1;
			$(productRegisterVariation.formRow).each(function() {
				var index = $(this).data('index');
				var displayOrderTemp = 0;
				if (Number.isInteger(parseInt($(this).find('#tbDisplayOrder' + index).val()))) {
					displayOrderTemp = parseInt($(this).find('#tbDisplayOrder' + index).val());
				}

				if (displayOrder <= displayOrderTemp) {
					displayOrder = $(productRegisterVariation.formRow).length == 1
						? 1
						: displayOrderTemp + 1;
				}
			});

			return displayOrder;
		}

		// Show loading
		function showLoading(component) {
			component.isLoading = true;
			checkAjaxLoading();
			component.content.hide();
			component.loading.show();
		}

		// Hide loading
		function hideLoading(component) {
			component.isLoading = false;
			checkAjaxLoading();
			component.content.show();
			component.loading.hide();
		}

		// Hide error message datepicker container
		function hideErrorMessageDatepickerContainer() {
			$('.error-message-datepicker-container').hide();
		}

		$(window).bind("pageshow", function () {
			if (window.performance && window.performance.navigation.type == window.performance.navigation.TYPE_BACK_FORWARD) {
				var form = $('form');
				// let the browser natively reset defaults
				form[0].reset();
				initPoint();
			}
		});

		// Remove current menu
		function removeCurrentMenu(title) {
			$('.sidemenu-child-list .sidemenu-list a').each(function () {
				if ($(this).text() == title) {
					$(this).closest('.sidemenu-list').removeClass('current');
				}
			});
		}

		// Initialize components
		$(function () {
			var removeCurrentTitle = '新規商品登録';
			<% if(this.IsRedirectFromMenu) { %>
			$(document).prop('title', '<%= Constants.APPLICATION_NAME_DISP %>' + '\t新規商品登録');
			removeCurrentTitle = '商品情報';
			<% } %>
			removeCurrentMenu(removeCurrentTitle);

			// Product
			var images = <%= this.GetProductImages() %>;
			initProductImages(images);
			initProductSelectVariationKbns();
			initProductColors();
			initRelatedProducts();
			initProductTaxCategories();
			initProductCategories();
			initProductBrands();
			initProductTagSettings();
			initProductExtends();
			initProductOptionSettings();

			var productMemberRank = <%= this.GetProductMemberRankPrice() %>;
			initProductMemberRankPrices(productMemberRank);

			// Member rank
			initDisplayMemberRanks();
			initBuyableMemberRanks();

			// User management level
			initUserManagementLevels();

			// Point
			initPoint();

			// Payment
			initLimitPayment();

			// Shipping
			initShippingTypes();
			initShippingSizes();

			// Mall exhibits config
			initMallExhibitsConfigs();

			// Stock
			initStockManagementKbns();
			initProductStockMessages();

			// Import, export
			initImportFileEvents();
			initUploadSettings();

			// Product Variation
			initProductVariation();

			loadTable(false, false, true);
			createNextShippingFixedPurchaseSettingMessage();
		});

		// Initialize product images
		function initProductImages(images) {
			showLoading(loadingComponents.productImage);
			var url = '<%: this.ProductRegisterBaseUrl %>/GetProductSubImageSettings';
			var request = callAjax(url, '');
			request.done(function (response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);

				var warpperIndex = 0;
				var wrapper = $(productRegisterImageSelect.wrapperSelector).eq(warpperIndex);
				var subWrapper = $('.product-register-image-selecter-image-sub-wrapper');
				productImages = [];

				var mainImage = images ? images.filter(function (element) { return element.imageNo == 0 })[0] : undefined;
				productImages.push({
					imageNo: 0,
					sourceIndex: 0,
					source: mainImage ? mainImage.source : undefined,
					delFlg: mainImage ? mainImage.delFlg : undefined,
					fileName: mainImage ? mainImage.fileName : undefined
				});
				if (mainImage && (typeof mainImage.source == "string")) {
					$('.product-register-image-selecter-image-main-thum-img:eq(0)').attr('src', mainImage.source);
					productRegisterImageSelect.displayRender(wrapper, warpperIndex);
				}

				subWrapper.empty();
				for (var index = 0; index < data.length; index++) {
					var image = images ? images.filter(function (element) { return element.imageNo == data[index].ImageNo })[0] : undefined;
					productImages.push({
						imageNo: data[index].ImageNo,
						sourceIndex: image ? image.sourceIndex : data[index].ImageNo,
						source: image ? image.source : undefined,
						delFlg: image ? image.delFlg : undefined,
						fileName: image ? image.fileName : undefined
					});
					productRegisterImageSelect.files[index + 1] = image ? image.source : undefined;
					var html =
						'<div id="divSub' + data[index].ImageNo + '" class="product-register-image-selecter-image-sub" style="display: none">'
						+ '    <div class="product-register-image-selecter-image-sub-title">' + data[index].ImageName + '</div>'
						+ '    <div class="product-register-image-selecter-image-sub-thum">'
						+ '        <img src="" alt="" class="product-register-image-selecter-image-sub-thum-img">'
						+ '    </div>'
						+ '    <label class="product-register-image-selecter-image-sub-droparea">'
						+ '        <input type="file" name="" id="product-register-image-selecter-image-sub-' + (index + 1) + '"class="product-register-image-selecter-image-sub-input" accept="image/jpg, image/jpeg" />'
						+ '    </label>'
						+ '    <div class="product-register-image-selecter-image-sub-btns">'
						+ '        <label for="product-register-image-selecter-image-sub-' + (index + 1) + '" class="product-register-image-selecter-image-sub-btn-set btn btn-main btn-size-s">設定する</label>'
						+ '        <label for="product-register-image-selecter-image-sub-' + (index + 1) + '" class="product-register-image-selecter-image-sub-btn-change btn btn-main btn-size-s">変更する</label>'
						+ '        <a href="javascript:void(0);" class="product-register-image-selecter-image-sub-btn-delete btn btn-sub btn-size-s">削除する</a>'
						+ '    </div>'
						+ '</div>';
					subWrapper.append(html);

					var fileReader = new FileReader();

					// サムネイルセットする際のカウント用
					fileReader.index = index;
					fileReader.onload = function (event) {
						var dataIndex = this.index;
						var loadedImageUri = event.target.result;
						$('#divSub' + data[dataIndex].ImageNo).find('img').attr('src', loadedImageUri);
						productRegisterImageSelect.displayRender(wrapper, warpperIndex);
					}

					if (image) {
						if (image.source && (mainImage.source == false)) {
							wrapper.find(productRegisterImageSelect.imageMainSelector).attr('src', 'tmp');
						}
						if (image.source && (typeof image.source != "string")) {
							fileReader.readAsDataURL(image.source);
						} else {
							$('#divSub' + data[index].ImageNo).find('img').attr('src', image.source);
							productRegisterImageSelect.displayRender(wrapper, warpperIndex);
						}
					}
				}
				$(productRegisterImageSelect.wrapperSelector).data('ini', false);
				productRegisterImageSelect.ini();

				hideLoading(loadingComponents.productImage);
			});
		}

		// Init product select variation kbns
		function initProductSelectVariationKbns() {
			showLoading(loadingComponents.productSelectVariationKbns);
			var defaultKbn = "<%: this.ProductSelectVariationKbn %>"
			var url = "<%: this.ProductRegisterBaseUrl %>/GetSelectVariationKbnSettings";
			var request = callAjax(url, '');

			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				if (data.length == 0) {
					hideLoading(loadingComponents.productSelectVariationKbns);
					return;
				}

				var wrapper = $('.product-register-product-variation-kubun');
				wrapper.empty();

				for (var index = 0; index < data.length; index++) {
					var description = data[index].value.split('(')[1]
						? data[index].value.split('(')[1].slice(0, -1)
						: '';
					var checked = (defaultKbn == data[index].key) ? 'checked' : '';
					var html = '<div class="product-register-product-variation-kubun-item">'
						+ '    <input type="radio" name="rbgSelectVariationKbn" id="rbSelectVariationKbn' + index + '" class="radio-card" value="' + data[index].key + '" ' + checked + ' />'
						+ '    <label for="rbSelectVariationKbn' + index + '">'
						+ '        <span class="product-register-product-variation-kubun-item-thum">'
						+ '            <img src="<%= Constants.PATH_ROOT_EC + Constants.PATH_SELECT_VARIATION_KBN_ICON_IMAGE %>select_variation_kbn_' + data[index].key.toLowerCase() + '.jpg" alt="' + data[index].key + '">'
						+ '        </span>'
						+ '        <span class="product-register-product-variation-kubun-item-description">'
						+ '            <span class="product-register-product-variation-kubun-item-description-txt1">' + data[index].value.split('(')[0] + '</span>'
						+ '            <span class="product-register-product-variation-kubun-item-description-txt2">' + description + '</span>'
						+ '        </span>'
						+ '    </label>'
						+ '</div>';

					wrapper.append(html);
				}

				if (defaultKbn == '') $('#rbSelectVariationKbn0').prop('checked', true);
				hideLoading(loadingComponents.productSelectVariationKbns);
			})
		}

		// Init product colors
		function initProductColors() {
			var colors = <%= GetProductColors() %>;
			var defaultColor = '<%= this.ProductInput.ProductColorId %>';

			// Init value
			var select = $("#ddlColorImage");
			select.empty();

			for (var index = 0; index < colors.length; index++) {
				var selected = (defaultColor == colors[index].Id) ? 'selected' : '';
				if (selected === 'selected') {
					$("#iColorImage").attr('src', colors[index].ImageUrl);
					if (colors[index].ImageUrl == '') {
						$("#iColorImage").hide();
					} else {
						$("#iColorImage").show();
					}
				}

				select.append('<option value="' + colors[index].Id + '" ' + selected + ' url="' + colors[index].ImageUrl + '">' + colors[index].DispName + '</option>');
			}

			select.on('change', function(e) {
				var option = $('option:selected', this);
				var url = option.attr('url');

				$("#iColorImage").attr('src', url);
				if(url == '') {
					$("#iColorImage").hide();
				} else {
					$("#iColorImage").show();
				}
			});
		}

		// Initialize shipping types
		function initShippingTypes(shippingType) {
			showLoading(loadingComponents.productShipping);

			var url = "<%: this.ProductRegisterBaseUrl %>/GetShippingTypes";
			var request = callAjax(url, '');

			request.done(function(reponse) {
				if ((reponse == null) || (reponse.d == undefined)) return;

				var data = JSON.parse(reponse.d);
				if (data.length == 0) {
					$('#divShippingType').hide();
					$('#btnShippingListSetting').hide();
					$('#divShippingTypeNoData').show();
				} else {
					$('#divShippingType').show();
					$('#btnShippingListSetting').show();
					$('#divShippingTypeNodata').hide();
				}

				// Clear data
				$('#ddlShippingType').empty();

				var shippingTypeId = "<%: this.ProductInput.ShippingType %>";
				for (var index = 0; index < data.length; index++) {
					// Create display html
					var selected = (shippingTypeId == data[index].key) ? 'selected' : '';
					var html = '<option value="' + data[index].key + '" ' + selected + '>' + data[index].value + '</option>';

					$('#ddlShippingType').append(html);
				}

				if (shippingType) $('#ddlShippingType').val(shippingType);

				hideLoading(loadingComponents.productShipping);
				initLimitedFixedPurchaseKbnSetting();
			})
		}

		// Initialize shipping sizes
		function initShippingSizes() {
			if ('<%: GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_SHIPPING_TYPE) %>' === 'False') return;

			showLoading(loadingComponents.shippingSize);
			var url = '<%: this.ProductRegisterBaseUrl %>/GetShippingSizes';
			var request = callAjax(url, '');

			request.done(function(reponse) {
				if ((reponse == null) || (reponse.d == undefined)) return;

				var data = JSON.parse(reponse.d);
				if (data.length == 0) {
					hideLoading(loadingComponents.shippingSize);
					return;
				}

				// Clear data
				$('#ddlShippingSize').empty();

				var shippingSize = '<%: this.ProductInput.ShippingSizeKbn %>';
				for (var index = 0; index < data.length; index++) {
					// Create display html
					var selected = (shippingSize == data[index].key) ? 'selected' : '';
					var html = '';

					if (data[index].key == '<%: Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_MAIL %>') {
						html = '<option value="' + data[index].key + '"' + selected + ' data-toggle-content-selector=.product-size-factor-toggle-content>' + data[index].value + '</option>';
						if (selected == 'selected') $('.product-size-factor-toggle-content').show();
					} else {
						html = '<option value="' + data[index].key + '"' + selected + '>' + data[index].value + '</option>';
					}

					$('#ddlShippingSize').append(html);
				}

				hideLoading(loadingComponents.shippingSize);
			})
		}

		// Initialize stock management kbns
		function initStockManagementKbns() {
			if ('<%: Constants.PRODUCT_STOCK_OPTION_ENABLE %>' === 'False') return;

			showLoading(loadingComponents.stockManagementKbn);
			var url = '<%: this.ProductRegisterBaseUrl %>/GetStockManagementKbns';
			var request = callAjax(url, '');

			request.done(function(reponse) {
				if ((reponse == null) || (reponse.d == undefined)) return;

				var data = JSON.parse(reponse.d);
				if (data.length == 0) {
					hideLoading(loadingComponents.stockManagementKbn);
					return;
				}

				// Clear data
				$('#ddlStockManagementKbn').empty();

				var stockManagementKbn = '<%: this.ProductInput.StockManagementKbn %>';
				for (var index = 0; index < data.length; index++) {
					// Create display html
					var selected = (stockManagementKbn == data[index].key) ? 'selected' : '';
					var html = '';
					if (data[index].key != '<%= Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED %>') {
						html = '<option value="' + data[index].key + '"' + selected + ' data-toggle-content-selector=.product-register-stock-management-toggle-content>'
							+ data[index].value
							+ '</option>';
					} else {
						html = '<option value="' + data[index].key + '"' + selected + '>'
							+ data[index].value
							+ '</option>';
					}

					$('#ddlStockManagementKbn').append(html);
				}

				toggle.ini();
				hideLoading(loadingComponents.stockManagementKbn);
			});
		}

		// Initialize product stock messages
		function initProductStockMessages(stockMessage) {
			if (('<%: Constants.PRODUCT_STOCK_OPTION_ENABLE %>' === 'False')
				|| ('<%: GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN) %>' === 'False')
				|| ('<%: GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_STOCK_MESSAGE_ID) %>' === 'False')) {
				return;
			}

			showLoading(loadingComponents.productStock);

			var url = '<%: this.ProductRegisterBaseUrl %>/GetProductStockMessages';
			var request = callAjax(url, '');

			request.done(function(reponse) {
				if ((reponse == null) || (reponse.d == undefined)) return;

				var data = JSON.parse(reponse.d);

				// Clear data
				$('#ddlStockMessage').empty();
				var stockMessageId = '<%: this.ProductInput.StockMessageId %>';
				for (var index = 0; index < data.length; index++) {
					// Create display html
					var selected = (stockMessageId == data[index].key) ? 'selected' : '';
					var html = '<option value="' + data[index].key + '"' + selected + '>' + data[index].value + '</option>';

					$('#ddlStockMessage').append(html);
				}
				if (stockMessage) $('#ddlStockMessage').val(stockMessage);

				hideLoading(loadingComponents.productStock);
			});
		}

		// Initialize point
		function initPoint() {
			// 商品新規登録時はptと%どちらも初期値0をセット
			if ('<%= this.ActionStatus %>' == 'insert') {
				$("#rbKbnPoint1").prop("checked", true);
				$("#tbPoint1").val('<%= this.ProductInput.Point1 %>');
				$("#tbPercent1").val('<%= this.ProductInput.Point1 %>');

				$("#rbKbnPoint2").prop("checked", true);
				$("#tbPoint2").val('<%= this.ProductInput.Point2 %>');
				$("#tbPercent2").val('<%= this.ProductInput.Point2 %>');
			} else {
				var point1 = "<%: this.ProductInput.PointKbn1 %>";
				if (point1 == "<%= Constants.FLG_PRODUCT_POINT_KBN1_RATE %>") {
					$("#rbKbnPercent1").prop("checked", true);
					$("#tbPercent1").val('<%= this.ProductInput.Point1 %>');
				} else {
					$("#rbKbnPoint1").prop("checked", true);
					$("#tbPoint1").val('<%= this.ProductInput.Point1 %>')
				}

				var point2 = "<%: this.ProductInput.PointKbn2 %>";
				if (point2 == "<%= Constants.FLG_PRODUCT_POINT_KBN2_RATE %>") {
					$("#rbKbnPercent2").prop("checked", true);
					$("#tbPercent2").val('<%= this.ProductInput.Point2 %>');
				} else {
					$("#rbKbnPoint2").prop("checked", true);
					$("#tbPoint2").val('<%= this.ProductInput.Point2 %>');
				}
			}
		}

		// Initialize product related
		function initRelatedProducts() {
			showLoading(loadingComponents.relatedProducts);
			var relatedProductIds = getRelatedProductIds();
			var url = "<%: this.ProductRegisterBaseUrl %>/GetRelatedProducts";
			var request = callAjax(url, JSON.stringify({ productIds: relatedProductIds }));

			request.done(function (response) {
				if ((response == null) || (response.d == undefined)) {
					hideLoading(loadingComponents.relatedProducts);
					customProductSelector.ini();
					return;
				}

				var data = JSON.parse(response.d);

				products = data || [];
				customProductSelector.ini();
				hideLoading(loadingComponents.relatedProducts);
			});
		}

		// Initialize product member rank prices
		function initProductMemberRankPrices(productMemberRankPrice) {
			if (('<%: Constants.MEMBER_RANK_OPTION_ENABLED %>' === 'False')
				|| ('<%: GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE) %>' === 'False')) {
				return;
			}

			showLoading(loadingComponents.productMemberRankPrice);

			var url = "<%: this.ProductRegisterBaseUrl %>/GetMemberRanks";
			var request = callAjax(url, JSON.stringify({ addEmptyValue: false }));

			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);

				// Clear data
				$('#divMemberRankPrice').empty();

				var html = '<div class="form-element-group-horizontal-grid" style="flex-wrap: wrap;">';

				for (var index = 0; index < data.length; index++) {
					var price = productMemberRankPrice
						? productMemberRankPrice.filter(function(element) { return ((element.member_rank_id == data[index].key) && (element.variation_id == '')) })[0]
						: undefined;
					var value = price ? price.member_rank_price : '';
					html += '<div class="form-element-group">'
						+ '    <div class="form-element-group-title member-rank-price-item break-text-hover">'
						+ '        <label for="' + encodeURI(data[index].key) + '">' + data[index].value + '</label>'
						+ '    </div>'
						+ '    <div class="form-element-group-content product-price-validate-form-element-group-container form-element-group-content-item-inline member-rank-price-item">'
						+ '        <input id="' + encodeURI(data[index].key) + '" type="text" value="' + value + '" class="w8em number-textbox" maxlength="7" >'
						+ '        <div class="product-price-error-message-container" data-id="' + encodeURI(data[index].key) + '"></div>'
						+ '    </div>'
						+ '</div>';
				}

				html += '</div>';

				$('#divMemberRankPrice').html(html);

				propductRegisterPriceErrorContainer.ini();

				hideLoading(loadingComponents.productMemberRankPrice);
			});
		}

		// Initialize product tax categories
		function initProductTaxCategories(productTax) {
			showLoading(loadingComponents.productTax);
			var url = "<%: this.ProductRegisterBaseUrl %>/GetProductTaxCategories";
			var request = callAjax(url, '');
			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);

				// Clear data
				$('#ddlTaxCategory').empty();

				var taxCategoryId = '<%= this.ProductInput.TaxCategoryId %>';
				for (index = 0; index < data.length; index++) {
					var selected = (data[index].TaxCategoryId == taxCategoryId) ? 'selected' : '';
					var option = '<option value="' + data[index].TaxCategoryId + '" ' + selected + '>'
						+ data[index].TaxCategoryName + ":" + data[index].TaxRate + '(%)'
						+ '</option>';

					$('#ddlTaxCategory').append(option);
				}

				// Set selected value
				if (productTax) $('#ddlTaxCategory').val(productTax);

				hideLoading(loadingComponents.productTax);
			});
		}

		// Initialize product brands
		function initProductBrands() {
			if('<%: Constants.PRODUCT_BRAND_ENABLED %>' === 'False') return;
			showLoading(loadingComponents.productBrand);

			var url = "<%: this.ProductRegisterBaseUrl %>/GetProductBrands";

			var request = callAjax(url, '');

			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				if (data.length == 0) {
					$('#divProductBrandHasData').hide();
					$('#lbBrandListSetting').hide();
					$('#divProductBrandNoData').show();
				} else {
					$('#divProductBrandHasData').show();
					$('#lbBrandListSetting').show();
					$('#divProductBrandNoData').hide();
				}

				$('.custom-category-selector-brand').each(function () {
					$(this).data('suggest-list', data);
				});
				customCategorySelector.ini('-brand');
				hideLoading(loadingComponents.productBrand);
			});
		}

		// Initialize product categories
		function initProductCategories() {
			if ('<%: Constants.PRODUCT_CTEGORY_OPTION_ENABLE %>' === 'False') return;
			showLoading(loadingComponents.productCategory);

			var url = "<%: this.ProductRegisterBaseUrl %>/GetProductCategories";
			var request = callAjax(url, '');
			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				if (data.length == 0) {
					$('#divProductCategoryHasData').hide();
					$('#lbCategoryListSetting').hide();
					$('#divProductCategoryNoData').show();
				} else {
					$('#divProductCategoryHasData').show();
					$('#lbCategoryListSetting').show();
					$('#divProductCategoryNoData').hide();
				}

				$('.custom-category-selector-category').each(function () {
					$(this).data('suggest-list', data);
				});
				customCategorySelector.ini('-category');
				hideLoading(loadingComponents.productCategory);
			});
		}

		// Initialize product category and product tag settings
		function initProductTagSettings(productTag) {
			if ('<%: Constants.PRODUCT_TAG_OPTION_ENABLE %>' === 'False') return;

			showLoading(loadingComponents.productTag);

			// Send request to server to retrieve product tag settings
			var url = "<%: this.ProductRegisterBaseUrl %>/GetProductTagSettings";
			var request = callAjax(url, JSON.stringify({productId: '<%= this.RequestProductId %>'}));
			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);

				var wrapper = $('#divProductTag');
				wrapper.empty();
				$('#lbTagListSetting').show();
				if (data.length == 0) {
					var event = '<%: CreateOpenProductTagSettingPopupScript("initProductTagSettings") %>';
					var html = '<div class="block-section-body-setting-guide">'
						+ '    <p class="block-section-body-setting-guide-text">商品タグが登録されていません。</p>'
						+ '    <div class="block-section-body-setting-guide-btns">'
						+ '        <a class="btn btn-txt btn-size-s" href="javascript:void(0)" onclick="' + event + '" class="btn btn-txt btn-size-m">商品タグ設定を開く<span class="btn-icon-right icon-arrow-out"></span></a>'
						+ '    </div>'
						+ '</div>';
					wrapper.html(html);
					$('#lbTagListSetting').hide();
				}

				var tag = ('<%: EncodeBackslash(CreateProductTagFromInput()) %>' != '')
					? <%= CreateProductTagFromInput() %>
					: {};

				for (var index = 0; index < data.length; index++) {
					var value = data[index].TagValue;
					if (tag.length > 0) {
						tag.forEach(function (element) {
							if (element.key == data[index].TagId) {
								value = element.value;
							}
						});
					}

					var html = '<div class="form-element-group form-element-group-horizontal-grid">'
						+ '    <div class="form-element-group-title break-text-hover">'
						+ '        <label for="' + data[index].TagId + '">' + data[index].TagName + '（' + data[index].TagId + '）<span class="notice"></span></label>'
						+ '    </div>'
						+ '    <div class="form-element-group-content product-tag-validate-form-element-group-container">'
						+ '        <input type="text" id="' + data[index].TagId + '" value="' + replaceSpecialChar(value) + '" maxlength="90" />'
						+ '        <input id="' + data[index].TagName + '" type="hidden" value="' + replaceSpecialChar(data[index].TagName) + '"/>'
						+ '        <div class="product-tag-error-message-container" data-id="' + data[index].TagId + '"></div>'
						+ '    </div>'
						+ '</div>'
					wrapper.append(html);
				}

				if (productTag) {
					for (var index = 0; index < productTag.product_tag_ids.length; index++) {
						$('#' + productTag.product_tag_ids[index]).val(productTag.product_tag_values[index]);
					}
				}

				propductRegisterTagErrorContainer.ini();

				hideLoading(loadingComponents.productTag);
			})
		}

		// Import file events initialization
		function initImportFileEvents() {
			$('#form-input-modal-product-register-master-upload-2').change(function() {
				$('#form-input-modal-product-register-master-upload-message').empty();
				$('#form-input-modal-product-register-master-upload-error-message').empty();
			});

			$('.dd-file-select-file-cancel').click(function() {
				$('#form-input-modal-product-register-master-upload-message').empty();
				$('#form-input-modal-product-register-master-upload-error-message').empty();
			});
		}

		// Initialize upload settings
		function initUploadSettings() {
			var url = "<%: this.ProductRegisterBaseUrl %>/GetMasterUploadSettings";
			var request = callAjax(url, '');

			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				if (data.length == 0) return;

				for (index = 0; index < data.length; index++) {
					var option = new Option(data[index].key, data[index].value);
					$('#form-input-modal-product-register-master-upload-1').append(option);
				}
			});
		}

		// Initialize variation
		function initVariation(index) {
			// Initialize variation color image
			initVariationColorImage(index);
			
			var variationMemberRankPrices = [];
			var formWrapper = $(productRegisterVariation.formSelector);
			formWrapper.find(productRegisterVariation.formRow).each(function() {
				var index = $(this).data('index');
				variationMemberRankPrices.push(getProductVariationMemberRankPrice(index, $(this)));
			});

			initVariationMemberRankPrices(index, variationMemberRankPrices);
		}

		// Initialzie variation color image
		function initVariationColorImage(index) {
			var select = $('#ddlVariationColorImage' + index + '');
			select.on('change', function (e) {
				var option = $('option:selected', this);
				var url = option.attr('url');
				$('#iColorImage' + index + '').attr('src', url);
				if (url == '') {
					$('#iColorImage' + index + '').hide();
				} else {
					$('#iColorImage' + index + '').show();
				}
			});
		}

		// Initialzie variation member rank prices
		function initVariationMemberRankPrices(variationIndex, variationMemberRankPrices) {
			if ('<%: Constants.MEMBER_RANK_OPTION_ENABLED %>' === 'False') return;

			var component = {
				content: $('.product-register-variation-form-row').find('.product-register-member-rank-price-list').eq(variationIndex),
				loading: $('.product-register-variation-form-row').find('.ajax-loading').eq(variationIndex)
			};
			showLoading(component);

			var url = "<%: this.ProductRegisterBaseUrl %>/GetMemberRanks";
			var request = callAjax(url, JSON.stringify({ addEmptyValue: false }));

			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var memberRanks = JSON.parse(response.d);

				var priceWrapper = $(productRegisterVariation.formSelector)
					.find(productRegisterVariation.formRow + '[data-index="' + variationIndex + '"]')
					.find('.product-register-member-rank-price-list');
				var variationMemberRankPrice = variationMemberRankPrices ? variationMemberRankPrices[variationIndex] : null;

				// Clear data
				priceWrapper.empty();

				for (var index = 0; index < memberRanks.length; index++) {
					var price = variationMemberRankPrice
						? variationMemberRankPrice.filter(function(element) { return element.member_rank_id == memberRanks[index].key })[0]
						: undefined;
					var value = price ? price.member_rank_price : '';
					var html = '<div class="form-element-group form-element-group-horizontal-grid">'
						+ '    <div class="form-element-group-title member-rank-price-item break-text-hover">'
						+ '        <label for="variation' + variationIndex + encodeURI(memberRanks[index].key) + '">' + memberRanks[index].value + '</label>'
						+ '    </div>'
						+ '    <div class="form-element-group-content product-variation-price-validate-form-element-group-container member-rank-price-item" style="align-self: flex-start;">'
						+ '        <input name="' + encodeURI(memberRanks[index].key) + '" value="' + value + '" type="text" id="variation' + variationIndex + encodeURI(memberRanks[index].key) + '" maxlength="7" class="w8em number-textbox">'
						+ '        <div class="product-variation-price-error-message-container" data-id="' + encodeURI(memberRanks[index].key) + '"></div>'
						+ '    </div>'
						+ '</div>';

					priceWrapper.append(html);
				}

				propductRegisterVariationPriceErrorContainer.ini(variationIndex);

				hideLoading(component);
			});
		}

		// Initialize mall exhibits configs
		function initMallExhibitsConfigs() {
			if (('<%: Constants.MALLCOOPERATION_OPTION_ENABLED %>' === 'False')
				|| ('<%: GetProductDefaultSettingDisplayField(Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG) %>' === 'False')
				|| ('<%: GetDisplayMallCooperationArea() %>' === 'False')) {
				return;
			}

			showLoading(loadingComponents.mallExhibitsConfig);
			var url = "<%: this.ProductRegisterBaseUrl %>/GetMallExhibitsConfigs";
			var request = callAjax(url, '');

			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				if (data.length == 0) {
					hideLoading(loadingComponents.mallExhibitsConfig);
					return;
				}

				mallExhibitsConfigs = data;
				var mallExhibitsConfig = ('<%= this.MallExhibitsConfig %>' != '')
					? JSON.parse('<%= this.MallExhibitsConfig %>')
					: [];

				$('#divMallExhibitsConfig').empty();
				var html = '';
				for (var index = 0; index < data.length; index++) {
					var id = 'cbMallExhibitsConfig' + (index + 1);
					var checked = '';

					if (mallExhibitsConfig.length > 0) {
						mallExhibitsConfig.forEach(function(element) {
							if(element.key == data[index].value) {
								checked = (element.value == '<%= Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON %>')
									? 'checked'
									: '';
							}
						});
					}

					html += '<div class="form-element-group-content-item-inline">'
						+ '<input id="' + id + '" type="checkbox" class="checkbox" name="sample_checkbox" value="' + data[index].value + '" ' + checked + '>'
						+ '<label for="' + id + '">' + data[index].key + '</label>'
						+ '</div>';
				}
				$('#divMallExhibitsConfig').html(html);
				hideLoading(loadingComponents.mallExhibitsConfig);
			});
		}

		// Initialize limit payment
		function initLimitPayment(payments) {
			if (('<%: GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS) %>' === 'False')
				|| ('<%: GetDisplayLimitedOptionArea() %>' === 'False')) {
				return;
			}

			showLoading(loadingComponents.limitedPayment);
			var urlRequest = "<%: this.ProductRegisterBaseUrl %>/GetLimitPayments";
			var request = callAjax(urlRequest, '');
			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				if (data.length == 0) {
					hideLoading(loadingComponents.limitedPayment);
					return;
				}

				var html = '';
				$('#divLimitedPayment').empty();
				var limitPayments = '<%= this.ProductInput.LimitedPaymentIds %>';
				limitPayments = limitPayments.split(',');
				for (var index = 0; index < data.length; index++) {
					var id = 'cblLimitedPayment-' + (index + 1);
					var check = (limitPayments.indexOf(data[index].value) != -1) ? 'checked' : '';
					if (payments) {
						check = (payments.indexOf(data[index].value) != -1) ? 'checked' : check;
					}
					html += '<div class="form-element-group-content-item-inline">'
						+ '    <input id="' + id + '" type="checkbox" class="checkbox" name="sample_checkbox" value="' + data[index].value + '" ' + check + ' />'
						+ '    <label for="' + id + '">' + data[index].key + '</label>'
						+ '</div>';
				}

				html += '<p class="note">'
					+ '利用不可決済種別は管理画面上、決済種別情報にて有効な決済種別が表示されます。<br />'
					+ '実際のフロントサイトへの表示制限は配送種別情報での決済種別設定、および、決済種別情報の利用不可ユーザー管理レベルでの制限を考慮した上で表示されますのでご注意ください。'
					+ '</p>';

				$('#divLimitedPayment').html(html);
				hideLoading(loadingComponents.limitedPayment);
			});
		}

		// Initialize product extends
		function initProductExtends() {
			var productExtends = <%= this.GetProductExtends() %>;
			if (productExtends.length == 0) return;

				// Clear data
			$('#divProductExtend').empty();

			var html = '<div class="form-element-group-heading is-form-element-toggle">'
				+ '  <h3 class="form-element-group-heading-label">拡張項目</h3>'
				+ '</div>';

			for (var index = 0; index < productExtends.length; index++) {
				var id = 'tbExtend-' + (index + 1);
				var input = '';

				var extend = replaceSpecialChar(productExtends[index].Extend);
				if (parseInt(productExtends[index].ExtendNo) <= 50) {
					input = '<input id="' + id + '" value="' + extend + '" type="text" maxlength="10" />';
				} else if (parseInt(productExtends[index].ExtendNo) <= 100) {
					input = '<input id="' + id + '" value="' + extend + '" type="text" maxlength="30" />';
				} else if (parseInt(productExtends[index].ExtendNo) <= 140) {
					input = '<textarea id="' + id + '" rows="4" cols="50">' + extend + '</textarea>';
				}

				html += '<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">'
					+ '    <div class="form-element-group-title break-text-hover">'
					+ '      <label id="lbExtend-' + (index + 1) + '" for="' + id + '">' + productExtends[index].ExtendName + '</label>'
					+ '      <br />'
					+ '      <p class="note">' + productExtends[index].ExtendDiscrition + '</p>'
					+ '    </div>'
					+ '    <div class="form-element-group-content product-extend-validate-form-element-group-container">'
					+        input
					+ '        <div class="product-extend-error-message-container" data-id="extend' + productExtends[index].ExtendNo + '"></div>'
					+ '        <br />'
					+ '        <input id="hfExtendNo-' + (index + 1) + '" type="hidden" value="' + productExtends[index].ExtendNo + '"/>'
					+ '    </div>'
					+ '</div>';
			}

			$('#divProductExtend').html(html);

			if ('<%= Constants.MALLCOOPERATION_OPTION_ENABLED %>' == 'False') {
				$('#divProductExtend').css('display', 'none');
			}

			blockSectionToggle.ini();

			propductRegisterExtendErrorContainer.ini();
		}

		// Initialize limited fixed purchase kbn setting
		function initLimitedFixedPurchaseKbnSetting() {
			initLimitedFixedPurchaseKbn1Setting();
			initLimitedFixedPurchaseKbn3Setting();
			initLimitedFixedPurchaseKbn4Setting();
		}

		// Initialize limited fixed purchase kbn 1 setting
		function initLimitedFixedPurchaseKbn1Setting() {
			if (('<%: Constants.FIXEDPURCHASE_OPTION_ENABLED %>' === 'False')
				|| ('<%: GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING) %>' === 'False')
				|| ('<%: GetDisplayFixedPurchaseArea() %>' === 'False')) {
				return;
			}

			showLoading(loadingComponents.limitedFixedPurchaseKbn1);
			var url = "<%: this.ProductRegisterBaseUrl %>/GetLimitedFixedPurchaseKbnSetting";
			var shippingType = $('#ddlShippingType').val();
			var data = JSON.stringify({
				shippingType: shippingType,
				productFieldName: '<%= Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING %>'
			});
			var request = callAjax(url, data);

			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				if (data.length == 0) {
					hideLoading(loadingComponents.limitedFixedPurchaseKbn1);
					$('#divLimitedFixedPurchaseKbn1').hide();
					return;
				}

				$('#divLimitedFixedPurchaseKbn1Setting').empty();
				var limitFixedPurchaseKbn1Settings = '<%= this.ProductInput.LimitedFixedPurchaseKbn1Setting %>';
				limitFixedPurchaseKbn1Settings = limitFixedPurchaseKbn1Settings.split(',');
				var html = '';
				for (var index = 0; index < data.length; index++) {
					var id = 'cbLimitedFixedPurchaseKbn1Setting-' + (index + 1);
					var check = (limitFixedPurchaseKbn1Settings.indexOf(data[index].value) != -1) ? 'checked' : '';
					html += '<div class="form-element-group-content-item-inline">'
						+ '<input id="' + id + '" type="checkbox" class="checkbox" name="sample_checkbox" value="' + data[index].value + '" ' + check + ' />'
						+ '<label for="' + id + '">' + data[index].key + '</label>'
						+ '</div>';
				}

				$('#divLimitedFixedPurchaseKbn1Setting').html(html);
				isShowFixedPurchaseKbn1Setting = true;
				hideLoading(loadingComponents.limitedFixedPurchaseKbn1);
			});
		}

		// Initialize limited fixed purchase kbn 3 setting
		function initLimitedFixedPurchaseKbn3Setting() {
			if (('<%: Constants.FIXEDPURCHASE_OPTION_ENABLED %>' === 'False')
				|| ('<%: GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING) %>' === 'False')
				|| ('<%: GetDisplayFixedPurchaseArea() %>' === 'False')) {
				return;
			}

			showLoading(loadingComponents.limitedFixedPurchaseKbn3);
			var url = "<%: this.ProductRegisterBaseUrl %>/GetLimitedFixedPurchaseKbnSetting";
			var shippingType = $('#ddlShippingType').val();
			var data = JSON.stringify({
				shippingType: shippingType,
				productFieldName: '<%= Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING %>'
			});
			var request = callAjax(url, data);

			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				if (data.length == 0) {
					hideLoading(loadingComponents.limitedFixedPurchaseKbn3);
					$('#divLimitedFixedPurchaseKbn3').hide();
					return;
				}

				$('#divLimitedFixedPurchaseKbn3Setting').empty();
				var limitFixedPurchaseKbn3Settings = '<%= this.ProductInput.LimitedFixedPurchaseKbn3Setting %>';
				limitFixedPurchaseKbn3Settings = limitFixedPurchaseKbn3Settings.split(',');
				var html = '';
				for (var index = 0; index < data.length; index++) {
					var id = 'cbLimitedFixedPurchaseKbn3Setting-' + (index + 1);
					var check = (limitFixedPurchaseKbn3Settings.indexOf(data[index].value) != -1) ? 'checked' : '';
					html += '<div class="form-element-group-content-item-inline">'
						+ '<input id="' + id + '" type="checkbox" class="checkbox" name="sample_checkbox" value="' + data[index].value + '" ' + check + ' />'
						+ '<label for="' + id + '">' + data[index].key + '</label>'
						+ '</div>';
				}

				$('#divLimitedFixedPurchaseKbn3Setting').html(html);
				isShowFixedPurchaseKbn3Setting = true;
				hideLoading(loadingComponents.limitedFixedPurchaseKbn3);
			});
		}

		// Initialize limited fixed purchase kbn 4 setting
		function initLimitedFixedPurchaseKbn4Setting() {
			if (('<%: Constants.FIXEDPURCHASE_OPTION_ENABLED %>' === 'False')
				|| ('<%: GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING) %>' === 'False')
				|| ('<%: GetDisplayFixedPurchaseArea() %>' === 'False')) {
				return;
			}

			showLoading(loadingComponents.limitedFixedPurchaseKbn4);
			var url = "<%: this.ProductRegisterBaseUrl %>/GetLimitedFixedPurchaseKbnSetting";
			var shippingType = $('#ddlShippingType').val();
			var data = JSON.stringify({
				shippingType: shippingType,
				productFieldName: '<%= Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING %>'
			});
			var request = callAjax(url, data);

			request.done(function (response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				if (data.length == 0) {
					hideLoading(loadingComponents.limitedFixedPurchaseKbn4);
					$('#divLimitedFixedPurchaseKbn4').hide();
					return;
				}

				$('#divLimitedFixedPurchaseKbn4Setting').empty();
				var limitFixedPurchaseKbn4Settings = '<%= this.ProductInput.LimitedFixedPurchaseKbn4Setting %>';
				limitFixedPurchaseKbn4Settings = limitFixedPurchaseKbn4Settings.split(',');
				var html = '';
				for (var index = 0; index < data.length; index++) {
					var id = 'cbLimitedFixedPurchaseKbn4Setting-' + (index + 1);
					var check = (limitFixedPurchaseKbn4Settings.indexOf(data[index].value) != -1) ? 'checked' : '';
					html += '<div class="form-element-group-content-item-inline">'
						+ '<input id="' + id + '" type="checkbox" class="checkbox" name="sample_checkbox" value="' + data[index].value + '" ' + check + ' />'
						+ '<label for="' + id + '">' + data[index].key + '</label>'
						+ '</div>';
				}

				$('#divLimitedFixedPurchaseKbn4Setting').html(html);
				isShowFixedPurchaseKbn4Setting = true;
				hideLoading(loadingComponents.limitedFixedPurchaseKbn4);
			});
		}

		// Initialize display member ranks
		function initDisplayMemberRanks(selectedMemberRank) {
			if (('<%: Constants.MEMBER_RANK_OPTION_ENABLED %>' === 'False')
				|| ('<%: GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_DISPLAY_MEMBER_RANK) %>' === 'False')) {
				return;
			}

			showLoading(loadingComponents.displayMemberRank);
			var url = "<%: this.ProductRegisterBaseUrl %>/GetMemberRanks";
			var request = callAjax(url, JSON.stringify({ addEmptyValue: true }));

			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				if (data.length == 0) {
					hideLoading(loadingComponents.displayMemberRank);
					return;
				}

				// Clear data
				$('#ddlDisplayMemberRank').empty();

				for (var index = 0; index < data.length; index++) {
					var option = new Option(data[index].value, data[index].key);
					$('#ddlDisplayMemberRank').append(option);
				}

				var selected = '<%= this.ProductInput.DisplayMemberRank %>';
				$('#ddlDisplayMemberRank').val(selected);

				if(selectedMemberRank) $('#ddlDisplayMemberRank').val(selectedMemberRank);
				hideLoading(loadingComponents.displayMemberRank);
			});
		}

		// Initialize buyable member ranks
		function initBuyableMemberRanks(selectedMemberRank) {
			if (('<%: Constants.MEMBER_RANK_OPTION_ENABLED %>' === 'False')
				|| ('<%: GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK) %>' === 'False')) {
				return;
			}

			showLoading(loadingComponents.buyableMemberRank);
			var url = "<%: this.ProductRegisterBaseUrl %>/GetMemberRanks";
			var request = callAjax(url, JSON.stringify({ addEmptyValue: true }));
			
			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				if (data.length == 0) {
					hideLoading(loadingComponents.buyableMemberRank);
					return;
				}

				// Clear data
				$('#ddlBuyableMemberRank').empty();

				for (var index = 0; index < data.length; index++) {
					var option = new Option(data[index].value, data[index].key);
					$('#ddlBuyableMemberRank').append(option);
				}

				var selected = '<%= this.ProductInput.BuyableMemberRank %>';
				$('#ddlBuyableMemberRank').val(selected);

				if(selectedMemberRank) $('#ddlBuyableMemberRank').val(selectedMemberRank);
				hideLoading(loadingComponents.buyableMemberRank);
			});
		}

		// Initialize user management levels
		function initUserManagementLevels() {
			if (('<%: Constants.FIXEDPURCHASE_OPTION_ENABLED %>' === 'False')
				|| ('<%: GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_USER_LEVEL_IDS) %>' === 'False')
				|| ('<%: GetDisplayFixedPurchaseArea() %>' === 'False')) {
				return;
			}

			showLoading(loadingComponents.limitUserLevel);
			var url = "<%: this.ProductRegisterBaseUrl %>/GetUserManagementLevels";
			var request = callAjax(url, '');
			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				if (data.length == 0) {
					hideLoading(loadingComponents.limitUserLevel);
					return;
				}

				// Clear data
				$('#divLimitUserLevel').empty();

				var html = '';
				var limitUserLevel = '<%= this.ProductInput.FixedPurchasedLimitedUserLevelIds %>'.split(',');

				for (var index = 0; index < data.length; index++) {
					var id = 'cbLimitedUserLevel-' + (index + 1);
					var checked = (limitUserLevel.indexOf(data[index].value) != -1) ? 'checked' : '';
					html += '<div class="form-element-group-content-item-inline">'
						+ '    <input id="' + id + '" type="checkbox" class="checkbox" name="sample_checkbox" value="' + data[index].value + '" ' + checked + '/>'
						+ '    <label for="' + id + '">' + data[index].key + '</label>'
						+ '</div>';
				}
				$('#divLimitUserLevel').html(html);
				hideLoading(loadingComponents.limitUserLevel);
			});
		}

		// Initialize product variation
		function initProductVariation() {
			// Get variation images
			var images = <%= this.GetVariationImages() %>;

			var variations = <%= this.GetProductVariation() %>;
			var memberRankPrices = <%= this.GetProductMemberRankPrice() %>;
			var variationMemberRankPrices = [];

			variations.forEach(function(variation, variationIndex) {
				var variationMemberRankPrice = memberRankPrices.filter(function(price) { return price.variation_id == variation.variation_id });
				variationMemberRankPrices[variationIndex] = variationMemberRankPrice;

				var pattern = getVariationPattern(variationIndex, variation);
				$(propductRegisterVariationErrorContainer.formSelector).append(pattern);
				var btnDelete = $(propductRegisterVariationErrorContainer.formSelector).find(productRegisterVariation.btnDeleteSelector);
				btnDelete.each(function () {
					$(this).off('click').on('click', function () {
						var parent = $(this).closest('.product-register-variation-form-row');
						var index = $(".product-register-variation-form-row").index(parent);
						$(this).closest('.product-register-variation-form-row').remove();
						variationImages.splice(index, 1);
						blockSectionToggle.ini();
					});
				});

				$('#iVariationImageHead' + variationIndex).attr('src', images[variationIndex].source);
				productRegisterImageSelect.variationFiles[variationIndex] = {
					source: images[variationIndex].source,
					fileName: images[variationIndex].fileName,
					delFlg: images[variationIndex].delFlg
				};

				initVariationColorImage(variationIndex);
				initVariationMemberRankPrices(variationIndex, variationMemberRankPrices);
				propductRegisterVariationErrorContainer.ini(variationIndex);
			});

			setTimeout(function () {
				productRegisterVariation.ini();
				blockSectionToggle.ini();
			}, 200);
		}

		// Reload member ranks
		function reloadMemberRanks() {
			var productMemberRankPrice = getProductMemberRankPrice();
			var variationMemberRankPrices = [];
			var formWrapper = $(productRegisterVariation.formSelector);

			formWrapper.find(productRegisterVariation.formRow).each(function(index) {
				variationMemberRankPrices.push(getProductVariationMemberRankPrice(index, $(this)));
			});

			initProductMemberRankPrices(productMemberRankPrice);

			formWrapper.find(productRegisterVariation.formRow).each(function(index) {
				initVariationMemberRankPrices(index, variationMemberRankPrices);
			});

			initDisplayMemberRanks($('#ddlDisplayMemberRank').val());
			initBuyableMemberRanks($('#ddlBuyableMemberRank').val());
		}

		// Reload product tags
		function reloadProductTags() {
			var product = {};
			getProductTagRequest(product)
			initProductTagSettings(product.EX_ProductTag);
		}

		// Reload stock messages
		function reloadStockMessages() {
			var stockMessage = $('#ddlStockMessage').val();
			initProductStockMessages(stockMessage);
		}

		// Reload shipping types
		function reloadShippingTypes() {
			var shippingType = $('#ddlShippingType').val();
			initShippingTypes(shippingType);
		}

		// Reload limit payment
		function reLoadLimitPayment(){
			var payments = getCheckBoxListValueFromInput('#cblLimitedPayment-', '#divLimitedPayment');
			payments = payments.split(',');
			initLimitPayment(payments);
		}

		// Reload product images
		function reloadProductImages() {
			var images = productImages;
			initProductImages(images);
		}

		// Select product cross sell
		function selectProductCrossSell() {
			$('.custom-product-selector-input-suggest-search').eq(0).val(selectedProductId);
			$('.select-product').eq(0).click();
			selectedProductId = '';
		}

		// Select product up sell
		function selectProductUpSell() {
			<% if (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS1)) { %>
			$('.custom-product-selector-input-suggest-search').eq(1).val(selectedProductId);
			$('.select-product').eq(1).click();
			selectedProductId = '';
			<% } else { %>
			$('.custom-product-selector-input-suggest-search').eq(0).val(selectedProductId);
			$('.select-product').eq(0).click();
			selectedProductId = '';
			<% } %>
		}

		// Execute confirm process
		function execConfirmProcess() {
			var isActiveFixedPurchase = '<%= CheckActiveFixedPurchase(this.ProductInput.ProductId) %>';

			if (isActiveFixedPurchase === 'True') {
				var message = "有効な定期台帳があります。\n配送種別の変更を行うと既存の定期台帳が使えなくなる可能性がありますがよろしいですか。\n";
				var result = window.confirm(message);
				if (result == false) return;
			}

			if (checkShippingType() == false) return;

			$('#divSubmitLoading').show();
			$('#divSubmitLoading').addClass('is-open');

			var formData = new FormData();
			formData.append('<%= Constants.PARAM_PRODUCT_INPUT %>', JSON.stringify(getProductRequest()));
			formData.append('<%= Constants.PARAM_PRODUCT_PROCESS_ACTION_KBN %>', '<%= this.ActionStatus %>');
			formData.append('<%= Constants.PARAM_GUID_STRING %>', '<%= this.GuidString %>');
			formData.append('<%= Constants.FIELD_PRODUCT_PRODUCT_ID %>', '<%= this.RequestProductId %>');
			formData.append('<%= Constants.REQUEST_KEY_HIDE_BACK_BUTTON %>', '<%= Request[Constants.REQUEST_KEY_HIDE_BACK_BUTTON] %>');

			var url = '<%: this.ProductHandlingProcessUrl %>';
			var request = callAjaxWithFormData(url, formData);

			request.done(function (response, textStatus, xmlHttpRequest) {
				// Clear all error contents
				clearAllErrorContents();

				if (response == null) return;

				checkSessionTimeout(xmlHttpRequest);

				var confirmUrl = response.confirmUrl;
				if (response.isSuccess) {
					var process = '<%= (this.ActionStatus == Constants.ACTION_STATUS_INSERT) ? Constants.PRODUCT_PROCESS_ACTION_KBN_UPLOAD_IMAGE_INSERT : Constants.PRODUCT_PROCESS_ACTION_KBN_UPLOAD_IMAGE_UPDATE_OR_COPY_INSERT %>';
					var uploadData = getProductImageRequest(process);
					if(uploadData == null){
					$('#divSubmitLoading').removeClass('is-open');
					$('#divSubmitLoading').hide();
					document.getElementById("pImageErrorMessage").style.display="block";
					return;
					}
					uploadData.append("<%= Constants.PARAM_GUID_STRING %>", response.guidString);
					uploadData.append("<%= Constants.PARAM_IS_BACK_FROM_CONFIRM %>", '<%: this.IsBackFromConfirm %>');
					var uploadRequest = callAjaxWithFormData(url, uploadData);
					$.when.apply($, uploadRequest).done(function () {
						setTimeout(function() {
							window.open(confirmUrl, '_self', '', false);
							}, 500);				
					});
				}
				
				// Bind error contents
				bindErrorContents(response.errorMessage);
			});
		}

		// Bind error contents
		function bindErrorContents(errorContents) {
			if (isJson(errorContents) === false) return;
			
			if ($('#form-element-all-toggle-trigger').is(':checked') === false) {
				$('#form-element-all-toggle-trigger').click();
			}
			
			// For product
			var errorContentsObject = JSON.parse(errorContents);
			var hasSetFocus = propductRegisterErrorContainer.bind(errorContentsObject, false);
			
			// For product prices
			var productPriceErrorContents = errorContentsObject['<%= Constants.FIELD_PRODUCT_PRODUCTPRICE_EXTEND %>'];
			if (isJson(productPriceErrorContents)) {
				hasSetFocus = propductRegisterPriceErrorContainer.bind(JSON.parse(productPriceErrorContents), hasSetFocus);
			}
			
			// For product tags
			var productTagErrorContents = errorContentsObject['<%= Constants.FIELD_PRODUCT_PRODUCTTAG_EXTEND %>'];
			if (isJson(productTagErrorContents)) {
				hasSetFocus = propductRegisterTagErrorContainer.bind(JSON.parse(productTagErrorContents), hasSetFocus);
			}
			
			// For product extends
			var productExtendErrorContents = errorContentsObject['<%= Constants.FIELD_PRODUCT_PRODUCTEXTEND_EXTEND %>'];
			if (isJson(productExtendErrorContents)) {
				hasSetFocus = propductRegisterExtendErrorContainer.bind(JSON.parse(productExtendErrorContents), hasSetFocus);
			}
			
			// For product variations
			var productVariationErrorContents = errorContentsObject['<%= Constants.FIELD_PRODUCT_PRODUCTVARIATION_EXTEND %>'];
			if (isJson(productVariationErrorContents)) {
				var productVariationErrorContentsObject = JSON.parse(productVariationErrorContents);
				var duplicationVariationIdError = productVariationErrorContentsObject['<%= ProductInput.CONST_ERROR_KEY_PRODUCT_VARIATION_DUPLICATION_ID %>'];

				var form = $(propductRegisterVariationErrorContainer.formSelector);
				var formRows = form.find(propductRegisterVariationErrorContainer.formRow);

				formRows.each(function(index, element) {
					if ((duplicationVariationIdError !== undefined) && (index === 0)) {
						var duplicationVariationIdErrorElement = $(element).find(
							propductRegisterVariationErrorContainer.errorSelecter
								+ "[data-id='"
								+ '<%= ProductInput.CONST_ERROR_KEY_PRODUCT_VARIATION_DUPLICATION_ID %>'
								+"']");
						$(duplicationVariationIdErrorElement).html(duplicationVariationIdError);
						$(duplicationVariationIdErrorElement).show();
						var control = $(duplicationVariationIdErrorElement).closest(propductRegisterVariationErrorContainer.wrapperSelector)
							.find(propductRegisterVariationErrorContainer.elementsSelector);

						if ((control.length !== 0) && (hasSetFocus == false)) {
							control[0].focus();
							hasSetFocus = true;
						}
					}
			
					// For variation
					var variationErrorContents = productVariationErrorContentsObject['<%= Constants.FIELD_PRODUCT_PRODUCTVARIATION_EXTEND %>' + index];
					if (isJson(variationErrorContents)) {
						var variationErrorContentsObject = JSON.parse(variationErrorContents);
						hasSetFocus = propductRegisterVariationErrorContainer.bind(
							index,
							variationErrorContentsObject,
							hasSetFocus);
			
						// For variation price
						var variationPriceErrorContents = variationErrorContentsObject['<%= Constants.FIELD_PRODUCTVARIATION_PRODUCTPRICE_EXTEND %>'];
						if (isJson(variationPriceErrorContents)) {
							var variationPriceErrorContentsObject = JSON.parse(variationPriceErrorContents);
							hasSetFocus = propductRegisterVariationPriceErrorContainer.bind(
								index,
								variationPriceErrorContentsObject,
								hasSetFocus);
						}
					}
				});
			}
			
			if (missingErrorMessage !== '') {
				notification.show(missingErrorMessage, 'warning', 'fadeout');
			};
			
			$('#divSubmitLoading').removeClass('is-open');
			$('#divSubmitLoading').hide();
		}

		// Clear all error contents
		function clearAllErrorContents() {

			missingErrorMessage = '';

			// For product
			propductRegisterErrorContainer.clear();
			propductRegisterPriceErrorContainer.clear();
			propductRegisterTagErrorContainer.clear();
			propductRegisterExtendErrorContainer.clear();

			// For variation
			propductRegisterVariationPriceErrorContainer.clearAll();
			propductRegisterVariationErrorContainer.clearAll();
		}

		// Get product image request
		function getProductImageRequest(process) {
			var submitProductImages = productImages.slice();
			var submitVariationImages = variationImages.slice();
			var subImageRequest = [];
			var variationImageRequest = [];
			var formData = new FormData();

			if ('<%= Constants.PRODUCT_IMAGE_HEAD_ENABLED %>' == 'False'){
				var fileIndex = submitProductImages[0].source instanceof File ? 0 : -1;
				formData.append("file", submitProductImages[0].source);
				var mainImage = {
					ImageNo: 0,
					SourceIndex: submitProductImages[0].sourceIndex,
					DelFlg: submitProductImages[0].delFlg,
					FileIndex: fileIndex,
					FileName: submitProductImages[0].fileName
				};
				submitProductImages.shift();
				fileIndex++;

				submitProductImages.forEach(function(element, index) {
					formData.append("file", element.source);
					subImageRequest.push({
						ImageNo: element.imageNo,
						SourceIndex: element.sourceIndex,
						DelFlg: element.delFlg,
						FileIndex: element.source instanceof File ? fileIndex : -1,
						FileName: element.fileName
					});
					if (element.source instanceof File) fileIndex++;
				});

				submitVariationImages.forEach(function (element) {
					formData.append("file", element.source);
					variationImageRequest.push({
						RefVariationId: $('#hfRefVatiationId' + element.variationIndex).val(),
						ImageHead: $('#hfVariationImageHead' + element.variationIndex).val(),
						FileIndex: element.source instanceof File ? fileIndex : -1,
						DelFlg: element.delFlg,
						FileName: element.fileName
					});
					if (element.source instanceof File) fileIndex++;
				});
			} else {
				var mainImage = {
					ImageNo: 0,
					SourceIndex: submitProductImages[0].sourceIndex,
					DelFlg: false,
					FileIndex: -1,
					FileName: $('#tbProductImageHaed').val() ? $('#tbProductImageHaed').val() + '<%= Constants.PRODUCTIMAGE_FOOTER_LL %>' : ""
				};
				submitProductImages.shift();

				submitProductImages.forEach(function(element, index) {
					formData.append("file", element.source);
					subImageRequest.push({
						ImageNo: element.imageNo,
						SourceIndex: element.sourceIndex,
						DelFlg: false,
						FileIndex: -1,
						FileName: $('#tbProductImageHaed').val()
							? $('#tbProductImageHaed').val() + '<%= Constants.PRODUCTSUBIMAGE_FOOTER %>' + element.imageNo.toString().padStart(2, "0") + '<%= Constants.PRODUCTIMAGE_FOOTER_LL %>'
							: ""
					});
				});

				var formWrapper = $(productRegisterVariation.formSelector);
				formWrapper.find(productRegisterVariation.formRow).each(function() {
					var wrapper = $(this);
					var index = wrapper.data('index');
					var imageHead = '<%= Constants.PRODUCT_IMAGE_HEAD_ENABLED %>' == 'False'
						? $('#tbProductId').val() + '<%= Constants.PRODUCTVARIATIONIMAGE_FOOTER %>' + wrapper.find('#tbVariationId' + index).val().trim()
						: wrapper.find('#tbVariationImageHead' + index).val();
					variationImageRequest.push({
						RefVariationId: $('#hfRefVatiationId' + index).val(),
						ImageHead: imageHead,
						FileIndex: -1,
						DelFlg: false,
						FileName: wrapper.find('#tbVariationImageHead' + index).val()
							? wrapper.find('#tbVariationImageHead' + index).val() + '<%= Constants.PRODUCTIMAGE_FOOTER_LL %>'
							: ""
					});
				});
			}

			var uploadImageInput = {
				ImageHead: '<%= Constants.PRODUCT_IMAGE_HEAD_ENABLED %>' == 'False' ? $('#tbProductId').val() : $('#tbProductImageHaed').val(),
				AutoResize: $('#cbResizeProductImage').prop('checked'),
				MainImage: mainImage,
				SubImages: subImageRequest,
				VariationImages: variationImageRequest
			};

			//アップロードファイルサイズ取得処理
			// 20971520 ファイルアップロード上限
			var size = 0;
			formData.getAll('file').forEach(function (element) {
				if ((element !== 'undefined') && (typeof element === 'object')) size = size + element['size'];
			});
			
			if (size > 20971520) {
				return null;
			}

			formData.append('<%= Constants.PARAM_PRODUCT_PROCESS_ACTION_KBN %>', process);
			formData.append('<%= Constants.PARAM_PRODUCT_UPLOAD_IMAGE_INPUT %>', JSON.stringify(uploadImageInput));
			formData.append('<%= Constants.PARAM_REFERENCE_PRODUCT_ID %>', '<%= this.Request[Constants.REQUEST_KEY_PRODUCT_ID] %>');
			return formData;
		}

		// Get product request
		function getProductRequest() {
			var request = {};

			getProductDisplayRequest(request);
			getProductCategoryRequest(request);
			getProductBrandRequest(request);
			getProductRelatedRequest(request);
			getProductPriceRequest(request);
			getProductPointKbnRequest(request);
			getProductMemberRankPointExcludeFlgRequest(request);
			getProductShippingTypeRequest(request);
			getProductVariationRequest(request);
			getProductTagRequest(request)
			getProductStockManagerKbnRequest(request);
			getProductInformationRequest(request);
			getProductExtendRequest(request);
			getProductOptionSettingRequest(request);

			return request;
		}

		// Get icon flag value
		function getIconFlgValue(index) {
			var value = $('#cbIconFlg' + index).is(":checked")
				? '<%= Constants.FLG_PRODUCT_ICON_ON %>'
				: '<%= Constants.FLG_PRODUCT_ICON_OFF %>';
			return value;
		}

		// Get icon flag term end value
		function getIconFlgTermEndValue(index) {
			if($('#cbIconFlg' + index).is(":checked") == false) return null;

			var date = $('#tbIconFlgTermEnd' + index + 'Date').val();
			var time = $('#tbIconFlgTermEnd' + index + 'Time').val();

			if (date == '') return time;

			return (time != '') ? (date + ' ' + time) : date;
		}

		// Get product display request
		function getProductDisplayRequest(request) {
			var displayPeriod = selectDateTimePeriodModal.getValue('<%= ucDisplay.ClientID %>');
			request['<%= Constants.FIELD_PRODUCT_DISPLAY_FROM %>'] = displayPeriod.valStartDate + ' ' + displayPeriod.valStartTime;
			request['<%= Constants.FIELD_PRODUCT_DISPLAY_TO %>'] = displayPeriod.valEndDate + ' ' + displayPeriod.valEndTime;

			var sellPeriod = selectDateTimePeriodModal.getValue('<%= ucSell.ClientID %>');
			var displaySellFlg = $('#cbDisplaySellFlg').prop('checked')
				? '<%= Constants.FLG_PRODUCT_DISPLAY_SELL_FLG_DISP %>'
				: '<%= Constants.FLG_PRODUCT_DISPLAY_SELL_FLG_UNDISP %>';
			request['<%= Constants.FIELD_PRODUCT_SELL_FROM %>'] = sellPeriod.valStartDate + ' ' + sellPeriod.valStartTime;
			request['<%= Constants.FIELD_PRODUCT_SELL_TO %>'] = sellPeriod.valEndDate + ' ' + sellPeriod.valEndTime;
			request['<%= Constants.FIELD_PRODUCT_DISPLAY_SELL_FLG %>'] = displaySellFlg;

			request['<%= Constants.FIELD_PRODUCT_DISPLAY_PRIORITY %>'] = ($('#tbDisplayPriority').val() != '') ? $('#tbDisplayPriority').val() : '<%= CONST_DISPLAY_PRIORITY_DEFAULT %>';

			var displayKbn = $('.product-register-display-kubun').find('input:checked').val();
			displayKbn = isUndefined(displayKbn) ? '<%= this.ProductInput.DisplayKbn %>' : displayKbn;
			request['<%= Constants.FIELD_PRODUCT_DISPLAY_KBN %>'] = (displayKbn != '') ? displayKbn : '<%= Constants.FLG_PRODUCT_DISPLAY_DISP_ALL %>';
			request['<%= Constants.FIELD_PRODUCT_PRODUCT_COLOR_ID %>'] = isUndefined($('#ddlColorImage').val()) ? '<%= this.ProductInput.ProductColorId %>' : $('#ddlColorImage').val();

			var selectVariationKbn = $('.product-register-product-variation-kubun').find('input:checked').val();
			selectVariationKbn = isUndefined(selectVariationKbn) ? '<%= this.ProductInput.SelectVariationKbn %>' : selectVariationKbn;
			request['<%= Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN %>'] = selectVariationKbn;

			var iconFlg1 = isUndefined($('#cbIconFlg1').val()) ? '<%= this.ProductInput.IconFlg1 %>' : getIconFlgValue(1);
			var iconFlg2 = isUndefined($('#cbIconFlg2').val()) ? '<%= this.ProductInput.IconFlg2 %>' : getIconFlgValue(2);
			var iconFlg3 = isUndefined($('#cbIconFlg3').val()) ? '<%= this.ProductInput.IconFlg3 %>' : getIconFlgValue(3);
			var iconFlg4 = isUndefined($('#cbIconFlg4').val()) ? '<%= this.ProductInput.IconFlg4 %>' : getIconFlgValue(4);
			var iconFlg5 = isUndefined($('#cbIconFlg5').val()) ? '<%= this.ProductInput.IconFlg5 %>' : getIconFlgValue(5);
			var iconFlg6 = isUndefined($('#cbIconFlg6').val()) ? '<%= this.ProductInput.IconFlg6 %>' : getIconFlgValue(6);
			var iconFlg7 = isUndefined($('#cbIconFlg7').val()) ? '<%= this.ProductInput.IconFlg7 %>' : getIconFlgValue(7);
			var iconFlg8 = isUndefined($('#cbIconFlg8').val()) ? '<%= this.ProductInput.IconFlg8 %>' : getIconFlgValue(8);
			var iconFlg9 = isUndefined($('#cbIconFlg9').val()) ? '<%= this.ProductInput.IconFlg9 %>' : getIconFlgValue(9);
			var iconFlg10 = isUndefined($('#cbIconFlg10').val()) ? '<%= this.ProductInput.IconFlg10 %>' : getIconFlgValue(10);
			request['<%= Constants.FIELD_PRODUCT_ICON_FLG1 %>'] = iconFlg1;
			request['<%= Constants.FIELD_PRODUCT_ICON_FLG2 %>'] = iconFlg2;
			request['<%= Constants.FIELD_PRODUCT_ICON_FLG3 %>'] = iconFlg3;
			request['<%= Constants.FIELD_PRODUCT_ICON_FLG4 %>'] = iconFlg4;
			request['<%= Constants.FIELD_PRODUCT_ICON_FLG5 %>'] = iconFlg5;
			request['<%= Constants.FIELD_PRODUCT_ICON_FLG6 %>'] = iconFlg6;
			request['<%= Constants.FIELD_PRODUCT_ICON_FLG7 %>'] = iconFlg7;
			request['<%= Constants.FIELD_PRODUCT_ICON_FLG8 %>'] = iconFlg8;
			request['<%= Constants.FIELD_PRODUCT_ICON_FLG9 %>'] = iconFlg9;
			request['<%= Constants.FIELD_PRODUCT_ICON_FLG10 %>'] = iconFlg10;

			var iconTermEnd1 = isUndefined($('#cbIconFlg1').val()) ? '<%= this.ProductInput.IconTermEnd1 %>' : getIconFlgTermEndValue(1);
			var iconTermEnd2 = isUndefined($('#cbIconFlg2').val()) ? '<%= this.ProductInput.IconTermEnd2 %>' : getIconFlgTermEndValue(2);
			var iconTermEnd3 = isUndefined($('#cbIconFlg3').val()) ? '<%= this.ProductInput.IconTermEnd3 %>' : getIconFlgTermEndValue(3);
			var iconTermEnd4 = isUndefined($('#cbIconFlg4').val()) ? '<%= this.ProductInput.IconTermEnd4 %>' : getIconFlgTermEndValue(4);
			var iconTermEnd5 = isUndefined($('#cbIconFlg5').val()) ? '<%= this.ProductInput.IconTermEnd5 %>' : getIconFlgTermEndValue(5);
			var iconTermEnd6 = isUndefined($('#cbIconFlg6').val()) ? '<%= this.ProductInput.IconTermEnd6 %>' : getIconFlgTermEndValue(6);
			var iconTermEnd7 = isUndefined($('#cbIconFlg7').val()) ? '<%= this.ProductInput.IconTermEnd7 %>' : getIconFlgTermEndValue(7);
			var iconTermEnd8 = isUndefined($('#cbIconFlg8').val()) ? '<%= this.ProductInput.IconTermEnd8 %>' : getIconFlgTermEndValue(8);
			var iconTermEnd9 = isUndefined($('#cbIconFlg9').val()) ? '<%= this.ProductInput.IconTermEnd9 %>' : getIconFlgTermEndValue(9);
			var iconTermEnd10 = isUndefined($('#cbIconFlg10').val()) ? '<%= this.ProductInput.IconTermEnd10 %>' : getIconFlgTermEndValue(10);
			request['<%= Constants.FIELD_PRODUCT_ICON_TERM_END1 %>'] = iconTermEnd1;
			request['<%= Constants.FIELD_PRODUCT_ICON_TERM_END2 %>'] = iconTermEnd2;
			request['<%= Constants.FIELD_PRODUCT_ICON_TERM_END3 %>'] = iconTermEnd3;
			request['<%= Constants.FIELD_PRODUCT_ICON_TERM_END4 %>'] = iconTermEnd4;
			request['<%= Constants.FIELD_PRODUCT_ICON_TERM_END5 %>'] = iconTermEnd5;
			request['<%= Constants.FIELD_PRODUCT_ICON_TERM_END6 %>'] = iconTermEnd6;
			request['<%= Constants.FIELD_PRODUCT_ICON_TERM_END7 %>'] = iconTermEnd7;
			request['<%= Constants.FIELD_PRODUCT_ICON_TERM_END8 %>'] = iconTermEnd8;
			request['<%= Constants.FIELD_PRODUCT_ICON_TERM_END9 %>'] = iconTermEnd9;
			request['<%= Constants.FIELD_PRODUCT_ICON_TERM_END10 %>'] = iconTermEnd10;

			request['<%= Constants.FIELD_PRODUCT_IMAGE_HEAD %>']
				= '<%= Constants.PRODUCT_IMAGE_HEAD_ENABLED %>' == 'False'
				? $('#tbProductId').val()
				: $('#tbProductImageHaed').val();
		}

		// Get variation request
		function getProductVariationRequest(request) {
			var variationDefaultSetting = <%= this.ProductVariationDefaultSetting %>;
			var refVariation = <%= this.GetProductVariation() %>;
			var defaultVariation = {};
			var variations = [];
			var formWrapper = $(productRegisterVariation.formSelector);
			formWrapper.find(productRegisterVariation.formRow).each(function() {
				var wrapper = $(this);
				var index = wrapper.data('index');

				if (wrapper.data('is-new')) {
					defaultVariation = variationDefaultSetting;
				} else {
					defaultVariation = refVariation[index];
				}

				var variation = {
					'<%: Constants.FIELD_PRODUCTVARIATION_SHOP_ID %>': '<%= this.LoginOperatorShopId %>',
					'<%: Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID %>': $('#tbProductId').val().trim(),
					'<%: Constants.FIELD_PRODUCTVARIATION_V_ID %>': wrapper.find('#tbVariationId' + index).val().trim(),
					'<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_ID %>': $('#tbProductId').val().trim() + wrapper.find('#tbVariationId' + index).val().trim(),
					'<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID1 %>': isUndefined(wrapper.find('#tbVariationCooperationId1' + index).val())
						? defaultVariation['<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID1 %>']
						: wrapper.find('#tbVariationCooperationId1' + index).val(),
					'<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID2 %>': isUndefined(wrapper.find('#tbVariationCooperationId2' + index).val())
						? defaultVariation['<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID2 %>']
						: wrapper.find('#tbVariationCooperationId2' + index).val(),
					'<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID3 %>': isUndefined(wrapper.find('#tbVariationCooperationId3' + index).val())
						? defaultVariation['<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID3 %>']
						: wrapper.find('#tbVariationCooperationId3' + index).val(),
					'<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID4 %>': isUndefined(wrapper.find('#tbVariationCooperationId4' + index).val())
						? defaultVariation['<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID4 %>']
						: wrapper.find('#tbVariationCooperationId4' + index).val(),
					'<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID5 %>': isUndefined(wrapper.find('#tbVariationCooperationId5' + index).val())
						? defaultVariation['<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID5 %>']
						: wrapper.find('#tbVariationCooperationId5' + index).val(),
					'<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID6 %>': isUndefined(wrapper.find('#tbVariationCooperationId6' + index).val())
						? defaultVariation['<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID6 %>']
						: wrapper.find('#tbVariationCooperationId6' + index).val(),
					'<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID7 %>': isUndefined(wrapper.find('#tbVariationCooperationId7' + index).val())
						? defaultVariation['<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID7 %>']
						: wrapper.find('#tbVariationCooperationId7' + index).val(),
					'<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID8 %>': isUndefined(wrapper.find('#tbVariationCooperationId8' + index).val())
						? defaultVariation['<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID8 %>']
						: wrapper.find('#tbVariationCooperationId8' + index).val(),
					'<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID9 %>': isUndefined(wrapper.find('#tbVariationCooperationId9' + index).val())
						? defaultVariation['<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID9 %>']
						: wrapper.find('#tbVariationCooperationId9' + index).val(),
					'<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID10 %>': isUndefined(wrapper.find('#tbVariationCooperationId10' + index).val())
						? defaultVariation['<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID10 %>']
						: wrapper.find('#tbVariationCooperationId10' + index).val(),
					'<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG %>': isUndefined(wrapper.find('#cbVariationAndMallReservationFlg' + index).val())
						? defaultVariation['<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG %>']
						: wrapper.find('#cbVariationAndMallReservationFlg' + index).prop('checked')
							? '<%= Constants.FLG_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG_RESERVATION %>'
							: '<%= Constants.FLG_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG_COMMON %>',
					'<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG %>': isUndefined(wrapper.find('#cbValiationAddCartUrlLimitFlg' + index).val())
						? defaultVariation['<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG %>']
						: wrapper.find('#cbValiationAddCartUrlLimitFlg' + index).prop('checked')
							? '<%= Constants.FLG_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG_VALID %>'
							: '<%= Constants.FLG_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG_INVALID %>',
					'<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_DOWNLOAD_URL %>': isUndefined(wrapper.find('#tbVariationDownloadUrl' + index).val())
						? defaultVariation['<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_DOWNLOAD_URL %>']
						: wrapper.find('#tbVariationDownloadUrl' + index).val(),
					'<%: Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID1 %>': isUndefined(wrapper.find('#tbMallVariationId1' + index).val())
						? defaultVariation['<%: Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID1 %>']
						: wrapper.find('#tbMallVariationId1' + index).val(),
					'<%: Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID2 %>': isUndefined(wrapper.find('#tbMallVariationId2' + index).val())
						? defaultVariation['<%: Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID2 %>']
						: wrapper.find('#tbMallVariationId2' + index).val(),
					'<%: Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_TYPE %>': isUndefined(wrapper.find('#tbMallVariationType' + index).val())
						? defaultVariation['<%: Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_TYPE %>']
						: wrapper.find('#tbMallVariationType' + index).val(),
					'<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1 %>': wrapper.find('#tbVariationName1' + index).val(),
					'<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2 %>': isUndefined(wrapper.find('#tbVariationName2' + index).val())
						? defaultVariation['<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2 %>']
						: wrapper.find('#tbVariationName2' + index).val(),
					'<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3 %>': isUndefined(wrapper.find('#tbVariationName3' + index).val())
						? defaultVariation['<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3 %>']
						: wrapper.find('#tbVariationName3' + index).val(),
					'<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_COLOR_ID %>': isUndefined(wrapper.find('#ddlVariationColorImage' + index).val())
						? defaultVariation['<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_COLOR_ID %>']
						: wrapper.find('#ddlVariationColorImage' + index).val(),
					'<%: Constants.FIELD_PRODUCTVARIATION_PRICE %>': wrapper.find('#tbVariationPrice' + index).val(),
					'<%: Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE %>': isUndefined(wrapper.find('#tbVariationSpecialPrice' + index).val())
						? defaultVariation['<%: Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE %>']
						: wrapper.find('#tbVariationSpecialPrice' + index).val(),
					'<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE %>': isUndefined(wrapper.find('#tbVariationFixedPurchaseFirsttimePrice' + index).val())
						? defaultVariation['<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE %>']
						: wrapper.find('#tbVariationFixedPurchaseFirsttimePrice' + index).val(),
					'<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE %>': isUndefined(wrapper.find('#tbVariationFixedPurchasePrice' + index).val())
						? defaultVariation['<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE %>']
						: wrapper.find('#tbVariationFixedPurchasePrice' + index).val(),
					'<%: Constants.FIELD_PRODUCTVARIATION_DISPLAY_ORDER %>': wrapper.find('#tbDisplayOrder' + index).val(),
					'<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_WEIGHT_GRAM %>': '<%= Constants.GLOBAL_OPTION_ENABLE %>' == 'True' ? wrapper.find('#tbVariationWeightGram' + index).val() : '0',
					'<%: Constants.FIELD_PRODUCTVARIATION_PRODUCTPRICE_EXTEND %>': getProductVariationMemberRankPrice(index, wrapper),
					'<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD %>': '<%= Constants.PRODUCT_IMAGE_HEAD_ENABLED %>' == 'False'
						? $('#tbProductId').val() + '<%= Constants.PRODUCTVARIATIONIMAGE_FOOTER %>' + wrapper.find('#tbVariationId' + index).val().trim()
						: wrapper.find('#tbVariationImageHead' + index).val(),
				};
				variation['<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG %>'] = ('<%= Constants.MALLCOOPERATION_OPTION_ENABLED %>' == 'True')
					? variation['<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG %>']
					: '<%= Constants.FLG_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG_COMMON %>';
				variation['<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_DOWNLOAD_URL %>'] = ('<%= Constants.DIGITAL_CONTENTS_OPTION_ENABLED %>' == 'True')
					? variation['<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_DOWNLOAD_URL %>']
					: '';
				if('<%= Constants.FIXEDPURCHASE_OPTION_ENABLED %>' == 'False') {
					variation['<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE %>'] = null;
					variation['<%: Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE %>'] = null;
				}
				variations.push(variation);
			})
			request['<%= Constants.FIELD_PRODUCT_PRODUCTVARIATION_EXTEND %>'] = variations;
			request['<%= Constants.FIELD_PRODUCT_USE_VARIATION_FLG %>'] = (variations.length > 0)
				? '<%= Constants.FLG_PRODUCT_USE_VARIATION_FLG_USE_USE %>'
				: '<%= Constants.FLG_PRODUCT_USE_VARIATION_FLG_USE_UNUSE %>';
		}

		// Get member rank price vatiation from input
		function getProductVariationMemberRankPrice(index, wrapper) {
			var variationMemberRankPrices = [];
			var shopId = '<%= this.LoginOperatorShopId %>';
			var productId = $('#tbProductId').val().trim();
			var variationId = productId + wrapper.find('#tbVariationId' + index).val().trim();
			wrapper.find('.product-register-member-rank-price-list .form-element-group').each(function() {
				var price = $(this);
				var variationMemberRankPrice = {
					'variation_index': index,
					'<%= Constants.FIELD_PRODUCTPRICE_SHOP_ID %>': '<%= this.LoginOperatorShopId %>',
					'<%= Constants.FIELD_PRODUCTPRICE_PRODUCT_ID %>': productId,
					'<%= Constants.FIELD_PRODUCTPRICE_VARIATION_ID %>': variationId,
					'<%= Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_ID %>': decodeURI(price.find('input').attr('name')),
					'<%= Constants.FIELD_MEMBERRANK_MEMBER_RANK_NAME %>': price.find('label').html(),
					'<%= Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE %>': price.find('input').val(),
				};
				variationMemberRankPrices.push(variationMemberRankPrice);
			})
			return variationMemberRankPrices;
		}

		// Get point kbn request
		function getProductPointKbnRequest(request) {
			if ('<%= Constants.W2MP_POINT_OPTION_ENABLED %>' == 'True') {
				if ($('#rbKbnPoint1').prop('checked')) {
					request['<%= Constants.FIELD_PRODUCT_POINT_KBN1 %>'] = '<%= Constants.FLG_PRODUCT_POINT_KBN1_NUM %>';
					request['<%= Constants.FIELD_PRODUCT_POINT1 %>'] = $('#tbPoint1').val();
				} else {
					request['<%= Constants.FIELD_PRODUCT_POINT_KBN1 %>'] = '<%= Constants.FLG_PRODUCT_POINT_KBN1_RATE %>';
					request['<%= Constants.FIELD_PRODUCT_POINT1 %>'] = $('#tbPercent1').val();
				}

				if ($('#cbPoint').prop('checked') == false) {
					request['<%= Constants.FIELD_PRODUCT_POINT_KBN2 %>'] = '<%= Constants.FLG_PRODUCT_POINT_KBN2_INVALID %>';
					request['<%= Constants.FIELD_PRODUCT_POINT2 %>'] = '<%= Constants.FLG_PRODUCT_POINT1_DEFAULT %>';
					return;
				}

				if ($('#rbKbnPoint2').prop('checked')) {
					request['<%= Constants.FIELD_PRODUCT_POINT_KBN2 %>'] = '<%= Constants.FLG_PRODUCT_POINT_KBN2_NUM %>';
					request['<%= Constants.FIELD_PRODUCT_POINT2 %>'] = ($('#tbPoint2').val() != '') ? $('#tbPoint2').val() : '<%= Constants.FLG_PRODUCT_POINT1_DEFAULT %>';
				} else if ($('#rbKbnPercent2').prop('checked')) {
					request['<%= Constants.FIELD_PRODUCT_POINT_KBN2 %>'] = '<%= Constants.FLG_PRODUCT_POINT_KBN2_RATE %>';
					request['<%= Constants.FIELD_PRODUCT_POINT2 %>'] = ($('#tbPercent2').val() != '') ? $('#tbPercent2').val() : '<%= Constants.FLG_PRODUCT_POINT1_DEFAULT %>';
				}
			} else {
				request['<%= Constants.FIELD_PRODUCT_POINT_KBN1 %>'] = '<%= Constants.FLG_PRODUCT_POINT_KBN1_INVALID %>';
				request['<%= Constants.FIELD_PRODUCT_POINT1 %>'] = '<%= Constants.FLG_PRODUCT_POINT1_DEFAULT %>';
				request['<%= Constants.FIELD_PRODUCT_POINT_KBN2 %>'] = '<%= Constants.FLG_PRODUCT_POINT_KBN2_INVALID %>';
				request['<%= Constants.FIELD_PRODUCT_POINT2 %>'] = '<%= Constants.FLG_PRODUCT_POINT1_DEFAULT %>';
			}
		}

		// 会員ランクポイント付与率除外設定フラグリクエスト取得
		function getProductMemberRankPointExcludeFlgRequest(request) {
			if ('<%= Constants.W2MP_POINT_OPTION_ENABLED %>' == 'True' && '<%= Constants.MEMBER_RANK_OPTION_ENABLED %>' == 'True') {
				var memberRankPointExcludeFlg = $('#cbMemberRankPointExcludeFlg').prop('checked')
					? '<%= Constants.FLG_PRODUCT_CHECK_MEMBER_RANK_POINT_EXCLUDE_FLG_VALID %>'
					: '<%= Constants.FLG_PRODUCT_CHECK_MEMBER_RANK_POINT_EXCLUDE_FLG_INVALID %>';
				request['<%= Constants.FIELD_PRODUCT_MEMBER_RANK_POINT_EXCLUDE_FLG %>'] = memberRankPointExcludeFlg;
			} else {
				request['<%= Constants.FIELD_PRODUCT_MEMBER_RANK_POINT_EXCLUDE_FLG %>'] = '<%= Constants.FLG_PRODUCT_CHECK_MEMBER_RANK_POINT_EXCLUDE_FLG_INVALID %>';
			}
		}


		// Get shipping type request
		function getProductShippingTypeRequest(request) {
			request['<%= Constants.FIELD_PRODUCT_SHIPPING_TYPE %>'] = $('#ddlShippingType').val();
			request['<%= Constants.FIELD_SHOPSHIPPING_SHOP_SHIPPING_NAME %>'] = $('#ddlShippingType :selected').text();
			request['<%= Constants.FIELD_PRODUCT_SHIPPING_SIZE_KBN %>'] = $('#ddlShippingSize').val();
			request['<%= Constants.FIELD_PRODUCT_PRODUCT_SIZE_FACTOR %>'] = ($('#ddlShippingSize').val() == '<%= Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_MAIL %>')
				? $('#tbProductSizeFactor').val()
				: 1;
			request['<%= Constants.FIELD_PRODUCT_PRODUCT_WEIGHT_GRAM %>'] = $('#tbProductWeightGram').val();

			var pluralShippingPriceFree = isUndefined($('#cbPluralShippingPriceFree').val())
				? '<%= this.ProductInput.PluralShippingPriceFreeFlg %>'
				: ($('#cbPluralShippingPriceFree').is(':checked')
					? '<%= Constants.FLG_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG_VALID %>'
					: '<%= Constants.FLG_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG_INVALID %>');
			request['<%= Constants.FIELD_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG %>'] = pluralShippingPriceFree;

			var storePickupFlg = isUndefined($('#cbStorePickupFlg').val())
				? '<%= this.ProductInput.StorePickupFlg %>'
				: ($('#cbStorePickupFlg').is(':checked')
					? '<%= Constants.FLG_ON %>'
					: '<%= Constants.FLG_OFF %>');
			request['<%= Constants.FIELD_PRODUCT_STOREPICKUP_FLG %>'] = storePickupFlg;

			var excludeFreeShippingFlg = isUndefined($('#cbExcludeFreeShippingFlg').val())
				? '<%= this.ProductInput.ExcludeFreeShippingFlg %>'
				: ($('#cbExcludeFreeShippingFlg').is(':checked')
					? '<%= Constants.FLG_ON %>'
					: '<%= Constants.FLG_OFF %>');
			request['<%= Constants.FIELD_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG %>'] = excludeFreeShippingFlg;
		}

		// Get stock manager kbn request
		function getProductStockManagerKbnRequest(request) {
			if ('<%= Constants.PRODUCT_STOCK_OPTION_ENABLE %>' == 'True') {
				request['<%= Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN %>'] = isUndefined($('#ddlStockManagementKbn').val()) ? '<%= this.ProductInput.StockManagementKbn %>' : $('#ddlStockManagementKbn').val();
				request['<%= Constants.FIELD_PRODUCT_STOCK_MESSAGE_ID %>'] = isUndefined($('#ddlStockMessage').val()) ? '<%= this.ProductInput.StockMessageId %>' : $('#ddlStockMessage').val();
				request['<%= Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_NAME %>'] = $('#ddlStockMessage').find(':selected').text();
				request['<%= Constants.FIELD_PRODUCTSTOCK_STOCK %>'] = $('#tbStock').val();
				request['<%= Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT %>'] = $('#tbStockAlert').val();
				request['<%= Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO %>'] = $('#tbUpdateMemo').val();
			} else {
				request['<%= Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN %>'] = '<%= Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED %>';
				request['<%= Constants.FIELD_PRODUCT_STOCK_MESSAGE_ID %>'] = '';
				request['<%= Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_NAME %>'] = '';
				request['<%= Constants.FIELD_PRODUCTSTOCK_STOCK %>'] = '0';
				request['<%= Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT %>'] = '0';
				request['<%= Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO %>'] = '';
			}
		}

		// Get product category request
		function getProductCategoryRequest(request) {
			if ('<%= Constants.PRODUCT_CTEGORY_OPTION_ENABLE %>' == 'True') {
				request['<%= Constants.FIELD_PRODUCT_CATEGORY_ID1 %>'] = isUndefined($('#hfProductCategoryId1').val()) ? '<%= this.ProductInput.CategoryId1 %>' : $('#hfProductCategoryId1').val();
				request['<%= Constants.FIELD_PRODUCT_CATEGORY_ID2 %>'] = isUndefined($('#hfProductCategoryId2').val()) ? '<%= this.ProductInput.CategoryId2 %>' : $('#hfProductCategoryId2').val();
				request['<%= Constants.FIELD_PRODUCT_CATEGORY_ID3 %>'] = isUndefined($('#hfProductCategoryId3').val()) ? '<%= this.ProductInput.CategoryId3 %>' : $('#hfProductCategoryId3').val();
				request['<%= Constants.FIELD_PRODUCT_CATEGORY_ID4 %>'] = isUndefined($('#hfProductCategoryId4').val()) ? '<%= this.ProductInput.CategoryId4 %>' : $('#hfProductCategoryId4').val();
				request['<%= Constants.FIELD_PRODUCT_CATEGORY_ID5 %>'] = isUndefined($('#hfProductCategoryId5').val()) ? '<%= this.ProductInput.CategoryId5 %>' : $('#hfProductCategoryId5').val();
			} else {
				request['<%= Constants.FIELD_PRODUCT_CATEGORY_ID1 %>'] = '';
				request['<%= Constants.FIELD_PRODUCT_CATEGORY_ID2 %>'] = '';
				request['<%= Constants.FIELD_PRODUCT_CATEGORY_ID3 %>'] = '';
				request['<%= Constants.FIELD_PRODUCT_CATEGORY_ID4 %>'] = '';
				request['<%= Constants.FIELD_PRODUCT_CATEGORY_ID5 %>'] = '';
			}
		}

		// Get product brand request
		function getProductBrandRequest(request) {
			if ('<%= Constants.PRODUCT_BRAND_ENABLED %>' == 'True') {
				request['<%= Constants.FIELD_PRODUCT_BRAND_ID1 %>'] = isUndefined($('#hfProductBrandId1').val()) ? '<%= this.ProductInput.BrandId1 %>' : $('#hfProductBrandId1').val();
				request['<%= Constants.FIELD_PRODUCT_BRAND_ID2 %>'] = isUndefined($('#hfProductBrandId2').val()) ? '<%= this.ProductInput.BrandId2 %>' : $('#hfProductBrandId2').val();
				request['<%= Constants.FIELD_PRODUCT_BRAND_ID3 %>'] = isUndefined($('#hfProductBrandId3').val()) ? '<%= this.ProductInput.BrandId3 %>' : $('#hfProductBrandId3').val();
				request['<%= Constants.FIELD_PRODUCT_BRAND_ID4 %>'] = isUndefined($('#hfProductBrandId4').val()) ? '<%= this.ProductInput.BrandId4 %>' : $('#hfProductBrandId4').val();
				request['<%= Constants.FIELD_PRODUCT_BRAND_ID5 %>'] = isUndefined($('#hfProductBrandId5').val()) ? '<%= this.ProductInput.BrandId5 %>' : $('#hfProductBrandId5').val();
			} else {
				request['<%= Constants.FIELD_PRODUCT_BRAND_ID1 %>'] = '';
				request['<%= Constants.FIELD_PRODUCT_BRAND_ID2 %>'] = '';
				request['<%= Constants.FIELD_PRODUCT_BRAND_ID3 %>'] = '';
				request['<%= Constants.FIELD_PRODUCT_BRAND_ID4 %>'] = '';
				request['<%= Constants.FIELD_PRODUCT_BRAND_ID5 %>'] = '';
			}
		}

		// Get product related request
		function getProductRelatedRequest(request) {
			request['<%= Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS1 %>'] = isUndefined($('#hfRelatedProductIdCs1').val()) ? '<%= this.ProductInput.RelatedProductIdCs1 %>' : $('#hfRelatedProductIdCs1').val();
			request['<%= Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS2 %>'] = isUndefined($('#hfRelatedProductIdCs2').val()) ? '<%= this.ProductInput.RelatedProductIdCs2 %>' : $('#hfRelatedProductIdCs2').val();
			request['<%= Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS3 %>'] = isUndefined($('#hfRelatedProductIdCs3').val()) ? '<%= this.ProductInput.RelatedProductIdCs3 %>' : $('#hfRelatedProductIdCs3').val();
			request['<%= Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS4 %>'] = isUndefined($('#hfRelatedProductIdCs4').val()) ? '<%= this.ProductInput.RelatedProductIdCs4 %>' : $('#hfRelatedProductIdCs4').val();
			request['<%= Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS5 %>'] = isUndefined($('#hfRelatedProductIdCs5').val()) ? '<%= this.ProductInput.RelatedProductIdCs5 %>' : $('#hfRelatedProductIdCs5').val();

			request['<%= Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US1 %>'] = isUndefined($('#hfRelatedProductIdUs1').val()) ? '<%= this.ProductInput.RelatedProductIdUs1 %>' : $('#hfRelatedProductIdUs1').val();
			request['<%= Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US2 %>'] = isUndefined($('#hfRelatedProductIdUs2').val()) ? '<%= this.ProductInput.RelatedProductIdUs2 %>' : $('#hfRelatedProductIdUs2').val();
			request['<%= Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US3 %>'] = isUndefined($('#hfRelatedProductIdUs3').val()) ? '<%= this.ProductInput.RelatedProductIdUs3 %>' : $('#hfRelatedProductIdUs3').val();
			request['<%= Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US4 %>'] = isUndefined($('#hfRelatedProductIdUs4').val()) ? '<%= this.ProductInput.RelatedProductIdUs4 %>' : $('#hfRelatedProductIdUs4').val();
			request['<%= Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US5 %>'] = isUndefined($('#hfRelatedProductIdUs5').val()) ? '<%= this.ProductInput.RelatedProductIdUs5 %>' : $('#hfRelatedProductIdUs5').val();
		}

		// Get product price request
		function getProductPriceRequest(request) {
			request['<%= Constants.FIELD_PRODUCT_TAX_CATEGORY_ID %>'] = $('#ddlTaxCategory').val();
			request['<%= Constants.FIELD_PRODUCTTAXCATEGORY_TAX_CATEGORY_NAME %>'] = $("#ddlTaxCategory :selected").text();
			request['<%= Constants.FIELD_PRODUCT_TAX_INCLUDED_FLG %>'] = '<%= Constants.MANAGEMENT_INCLUDED_TAX_FLAG ? Constants.FLG_PRODUCT_TAX_INCLUDED_PRETAX : Constants.FLG_PRODUCT_TAX_EXCLUDE_PRETAX %>'
			request['<%= Constants.FIELD_PRODUCT_DISPLAY_PRICE %>'] = $('#tbPrice').val();
			request['<%= Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE %>'] = isUndefined($('#tbSpecialPrice').val()) ? '<%= this.ProductInput.DisplaySpecialPrice.ToPriceString() %>' : $('#tbSpecialPrice').val();
			if ('<%= Constants.FIXEDPURCHASE_OPTION_ENABLED %>' == 'True') {
				request['<%= Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE %>'] = isUndefined($('#tbFixedPurchaseFirsttimePrice').val())
					? '<%= this.ProductInput.FixedPurchaseFirsttimePrice.ToPriceString() %>'
					: $('#tbFixedPurchaseFirsttimePrice').val();
				request['<%= Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE %>'] = isUndefined($('#tbFixedPurchasePrice').val())
					? '<%= this.ProductInput.FixedPurchasePrice.ToPriceString() %>'
					: $('#tbFixedPurchasePrice').val();
			} else {
				request['<%= Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE %>'] = null;
				request['<%= Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE %>'] = null;
			}
			request['<%= Constants.FIELD_PRODUCT_PRODUCTPRICE_EXTEND %>'] = getProductMemberRankPrice();
		}

		// Get product member rank price request
		function getProductMemberRankPrice() {
			var productMemberRankPrices = [];
			var shopId = '<%= this.LoginOperatorShopId %>';
			var productId = $('#tbProductId').val().trim();

			$('#divMemberRankPrice').find('.form-element-group').each(function() {
				var price = $(this);
				var productMemberRankPrice = {
					'variation_index': 0,
					'<%= Constants.FIELD_PRODUCTPRICE_SHOP_ID %>': '<%= this.LoginOperatorShopId %>',
					'<%= Constants.FIELD_PRODUCTPRICE_PRODUCT_ID %>': productId,
					'<%= Constants.FIELD_PRODUCTPRICE_VARIATION_ID %>': '',
					'<%= Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_ID %>': decodeURI(price.find('input').attr('id')),
					'<%= Constants.FIELD_MEMBERRANK_MEMBER_RANK_NAME %>': price.find('label').html(),
					'<%= Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE %>': price.find('input').val(),
				};

				productMemberRankPrices.push(productMemberRankPrice);
			})

			return productMemberRankPrices;
		}

		// Get product tag request
		function getProductTagRequest(request) {
			var productTagIds = [];
			var productTagValues = [];
			var productTagNames = [];
			$('#divProductTag').find('input').each(function() {
				var tag = $(this);
				if(tag.attr('type') == 'hidden') {
					productTagNames.push(tag.val());
				} else {
					productTagIds.push(tag.attr('id'));
					productTagValues.push(tag.val());
				}
			})

			var productTag = {
				'product_tag_ids': productTagIds,
				'product_tag_names': productTagNames,
				'product_tag_values': productTagValues
			};
			request['<%= Constants.FIELD_PRODUCT_PRODUCTTAG_EXTEND %>'] = productTag;
			request['<%= Constants.FIELD_PRODUCT_SEO_KEYWORDS %>'] = isUndefined($('#tbSeoKeywords').val()) ? '<%: EncodeBackslash(this.ProductInput.SeoKeywords) %>' : $('#tbSeoKeywords').val();
		}

		// Get product information request
		function getProductInformationRequest(request) {
			request['<%= Constants.FIELD_PRODUCT_PRODUCT_ID %>'] = $('#tbProductId').val();

			var validFlg = isUndefined($('#cbValidFlg').val())
				? '<%= this.ProductInput.ValidFlg %>'
				: ($('#cbValidFlg').is(':checked')
					? '<%= Constants.FLG_PRODUCT_VALID_FLG_VALID %>'
					: '<%= Constants.FLG_PRODUCT_VALID_FLG_INVALID %>')
			request['<%= Constants.FIELD_PRODUCT_VALID_FLG %>'] = (validFlg != '') ? validFlg : '<%= Constants.FLG_PRODUCT_VALID_FLG_INVALID %>';

			request['<%= Constants.FIELD_PRODUCT_NAME %>'] = $('#tbProductName').val();
			request['<%= Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY %>'] = ($('#tbMaxSellQuantity').val() != '') ? $('#tbMaxSellQuantity').val() : '<%= CONST_MAX_SELL_QUANTITY_DEFAULT %>';
			request['<%= Constants.FIELD_PRODUCT_NOTE %>'] = isUndefined($('#tbNote').val()) ? '<%: EncodeBackslash(GetEscapeValue(this.ProductInput.Note)) %>' : $('#tbNote').val();
			request['<%= Constants.FIELD_PRODUCT_SUPPLIER_ID %>'] = isUndefined($('#tbSupplierId').val()) ? replaceSpecialChar(`<%: EncodeBackslash(this.ProductInput.SupplierId) %>`) : $('#tbSupplierId').val();
			request['<%= Constants.FIELD_PRODUCT_NAME_KANA %>'] = ('<%= this.IsOperationalCountryJp %>' == 'True')
				? isUndefined($('#tbNameKana').val())
					? '<%= this.ProductInput.NameKana %>'
					: $('#tbNameKana').val()
				: '';
			request['<%= Constants.FIELD_PRODUCT_CATCHCOPY %>'] = isUndefined($('#tbCatchcopy').val()) ? `<%: EncodeBackslash(this.ProductInput.Catchcopy) %>` : $('#tbCatchcopy').val();
			request['<%= Constants.FIELD_PRODUCT_SEARCH_KEYWORD %>'] = isUndefined($('#tbSearchKeyword').val()) ? '<%: EncodeBackslash(GetEscapeValue(this.ProductInput.SearchKeyword)) %>' : $('#tbSearchKeyword').val();
			request['<%= Constants.FIELD_PRODUCT_URL %>'] = isUndefined($('#tbUrl').val()) ? '<%: EncodeBackslash(this.ProductInput.Url) %>' : $('#tbUrl').val();
			request['<%= Constants.FIELD_PRODUCT_MALLEXHIBITSCONFIG_EXTEND %>'] = getMallExhibitsConfigFromInput();

			var googleShoppingFlg = isUndefined($('#cbGoogleShoppingFlg').val())
				? '<%= this.ProductInput.GoogleShoppingFlg %>'
				: ($('#cbGoogleShoppingFlg').is(':checked')
					? '<%= Constants.FLG_PRODUCT_GOOGLE_SHOPPING_FLG_VALID %>'
					: '<%= Constants.FLG_PRODUCT_GOOGLE_SHOPPING_FLG_INVALID %>');
			request['<%= Constants.FIELD_PRODUCT_GOOGLE_SHOPPING_FLG %>'] = ('<%= Constants.GOOGLESHOPPING_COOPERATION_OPTION_ENABLED %>' == 'True')
				? googleShoppingFlg
				: '<%= Constants.FLG_PRODUCT_GOOGLE_SHOPPING_FLG_INVALID %>';

			var checkProductOrderLimit = isUndefined($('#cbCheckProductOrderLimit').val())
				? '<%= this.ProductInput.CheckProductOrderFlg %>'
				: ($('#cbCheckProductOrderLimit').is(':checked')
					? '<%= Constants.FLG_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG_VALID %>'
					: '<%= Constants.FLG_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG_INVALID %>');
			request['<%= Constants.FIELD_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG %>'] = ('<%= Constants.PRODUCT_ORDER_LIMIT_ENABLED %>' == 'True')
				? checkProductOrderLimit
				: '<%= Constants.FLG_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG_INVALID %>';

			var checkFixedProductOrderLimit = isUndefined($('#cbCheckFixedProductOrderLimit').val())
				? '<%= this.ProductInput.CheckFixedProductOrderFlg %>'
				: ($('#cbCheckFixedProductOrderLimit').is(':checked')
					? '<%= Constants.FLG_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG_VALID %>'
					: '<%= Constants.FLG_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG_INVALID %>');
			request['<%= Constants.FIELD_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG %>'] = ('<%= (Constants.FIXEDPURCHASE_OPTION_ENABLED && Constants.PRODUCT_ORDER_LIMIT_ENABLED) %>' == 'True')
				? checkFixedProductOrderLimit
				: '<%= Constants.FLG_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG_INVALID %>';

			request['<%= Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS %>'] = isUndefined($('#cblLimitedPayment-1').val())
				? '<%= this.ProductInput.LimitedPaymentIds %>'
				: getCheckBoxListValueFromInput('#cblLimitedPayment-', '#divLimitedPayment');

			var useRecommendFlg = isUndefined($('#cbUseRecommendFlg').val())
				? '<%= this.ProductInput.UseRecommendFlg %>'
				: ($('#cbUseRecommendFlg').is(':checked')
					? '<%= Constants.FLG_PRODUCT_USE_RECOMMEND_FLG_VALID %>'
					: '<%= Constants.FLG_PRODUCT_USE_RECOMMEND_FLG_INVALID %>');
			request['<%= Constants.FIELD_PRODUCT_USE_RECOMMEND_FLG %>'] = ('<%= Constants.RECOMMEND_ENGINE_KBN == Constants.RecommendEngine.Silveregg %>' == 'True')
				? useRecommendFlg
				: '<%= Constants.FLG_PRODUCT_USE_RECOMMEND_FLG_INVALID %>';

			var outlineKbn = isUndefined($('#rbOutlineFlg1').val())
				? '<%= this.ProductInput.OutlineKbn %>'
				: ($('#rbOutlineFlg1').is(':checked')
					? '<%= Constants.FLG_PRODUCT_DESC_DETAIL_TEXT %>'
					: '<%= Constants.FLG_PRODUCT_DESC_DETAIL_HTML %>');
			request['<%= Constants.FIELD_PRODUCT_OUTLINE_KBN %>'] = (outlineKbn != '') ? outlineKbn : '<%= Constants.FLG_PRODUCT_DESC_DETAIL_TEXT %>';
			request['<%= Constants.FIELD_PRODUCT_OUTLINE %>'] = isUndefined($('#tbOutline').val()) ? '<%: EncodeBackslash(GetEscapeValue(this.ProductInput.Outline)) %>' : $('#tbOutline').val();

			var descDetail1Kbn = isUndefined($('#rbDescDetailFlg1-1').val())
				? '<%= this.ProductInput.DescDetailKbn1 %>'
				: ($('#rbDescDetailFlg1-1').is(':checked')
					? '<%= Constants.FLG_PRODUCT_DESC_DETAIL_TEXT %>'
					: '<%= Constants.FLG_PRODUCT_DESC_DETAIL_HTML %>');
			request['<%= Constants.FIELD_PRODUCT_DESC_DETAIL_KBN1 %>'] = (descDetail1Kbn != '') ? descDetail1Kbn : '<%= Constants.FLG_PRODUCT_DESC_DETAIL_TEXT %>';
			request['<%= Constants.FIELD_PRODUCT_DESC_DETAIL1 %>'] = isUndefined($('#tbDescDetail1').val()) ? '<%: EncodeBackslash(GetEscapeValue(this.ProductInput.DescDetail1)) %>' : $('#tbDescDetail1').val();

			var descDetail2Kbn = isUndefined($('#rbDescDetailFlg2-1').val())
				? '<%= this.ProductInput.DescDetailKbn2 %>'
				: ($('#rbDescDetailFlg2-1').is(':checked')
					? '<%= Constants.FLG_PRODUCT_DESC_DETAIL_TEXT %>'
					: '<%= Constants.FLG_PRODUCT_DESC_DETAIL_HTML %>');
			request['<%= Constants.FIELD_PRODUCT_DESC_DETAIL_KBN2 %>'] = (descDetail2Kbn != '') ? descDetail2Kbn : '<%= Constants.FLG_PRODUCT_DESC_DETAIL_TEXT %>';
			request['<%= Constants.FIELD_PRODUCT_DESC_DETAIL2 %>'] = isUndefined($('#tbDescDetail2').val()) ? '<%: EncodeBackslash(GetEscapeValue(this.ProductInput.DescDetail2)) %>' : $('#tbDescDetail2').val();

			var descDetail3Kbn = isUndefined($('#rbDescDetailFlg3-1').val())
				? '<%= this.ProductInput.DescDetailKbn3 %>'
				: ($('#rbDescDetailFlg3-1').is(':checked')
					? '<%= Constants.FLG_PRODUCT_DESC_DETAIL_TEXT %>'
					: '<%= Constants.FLG_PRODUCT_DESC_DETAIL_HTML %>');
			request['<%= Constants.FIELD_PRODUCT_DESC_DETAIL_KBN3 %>'] = (descDetail3Kbn != '') ? descDetail3Kbn : '<%= Constants.FLG_PRODUCT_DESC_DETAIL_TEXT %>';
			request['<%= Constants.FIELD_PRODUCT_DESC_DETAIL3 %>'] = isUndefined($('#tbDescDetail3').val()) ? '<%: EncodeBackslash(GetEscapeValue(this.ProductInput.DescDetail3)) %>' : $('#tbDescDetail3').val();

			var descDetail4Kbn = isUndefined($('#rbDescDetailFlg4-1').val())
				? '<%= this.ProductInput.DescDetailKbn4 %>'
				: ($('#rbDescDetailFlg4-1').is(':checked')
					? '<%= Constants.FLG_PRODUCT_DESC_DETAIL_TEXT %>'
					: '<%= Constants.FLG_PRODUCT_DESC_DETAIL_HTML %>');
			request['<%= Constants.FIELD_PRODUCT_DESC_DETAIL_KBN4 %>'] = (descDetail4Kbn != '') ? descDetail4Kbn : '<%= Constants.FLG_PRODUCT_DESC_DETAIL_TEXT %>';
			request['<%= Constants.FIELD_PRODUCT_DESC_DETAIL4 %>'] = isUndefined($('#tbDescDetail4').val()) ? '<%: EncodeBackslash(GetEscapeValue(this.ProductInput.DescDetail4)) %>' : $('#tbDescDetail4').val();
			request['<%= Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE %>'] = isUndefined($('#tbReturnExchangeMessage').val()) ? '<%: EncodeBackslash(GetEscapeValue(this.ProductInput.ReturnExchangeMessage)) %>' : $('#tbReturnExchangeMessage').val();

			var isbundleItemDisplayTypeChecked = isUndefined($('#cbBundleItemDisplayType').val())
				? '<%= this.ProductInput.BundleItemDisplayType %>'
				: $('#cbBundleItemDisplayType').is(':checked');
			var bundleItemDisplayType = (('<%= Constants.PRODUCTBUNDLE_OPTION_ENABLED %>' == 'False') || isbundleItemDisplayTypeChecked)
				? '<%= Constants.FLG_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE_VALID %>'
				: '<%= Constants.FLG_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE_INVALID %>';
			request['<%= Constants.FIELD_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE %>'] = bundleItemDisplayType;

			var productType = isUndefined($('#rblProductType-1').val())
				? '<%= this.ProductInput.ProductType %>'
				: ($('#rblProductType-1').is(':checked')
					? '<%= Constants.FLG_PRODUCT_PRODUCT_TYPE_PRODUCT %>'
					: '<%= Constants.FLG_PRODUCT_PRODUCT_TYPE_FLYER %>');
			request['<%= Constants.FIELD_PRODUCT_PRODUCT_TYPE %>'] = ('<%= Constants.PRODUCTBUNDLE_OPTION_ENABLED %>' == 'True')
				? productType
				: '<%= Constants.FLG_PRODUCT_PRODUCT_TYPE_PRODUCT %>';

			request['<%= Constants.FIELD_PRODUCT_COOPERATION_ID1 %>'] = isUndefined($('#tbCooperationId1').val()) ? '<%: EncodeBackslash(this.ProductInput.CooperationId1) %>' : $('#tbCooperationId1').val();
			request['<%= Constants.FIELD_PRODUCT_COOPERATION_ID2 %>'] = isUndefined($('#tbCooperationId2').val()) ? '<%: EncodeBackslash(this.ProductInput.CooperationId2) %>' : $('#tbCooperationId2').val();
			request['<%= Constants.FIELD_PRODUCT_COOPERATION_ID3 %>'] = isUndefined($('#tbCooperationId3').val()) ? '<%: EncodeBackslash(this.ProductInput.CooperationId3) %>' : $('#tbCooperationId3').val();
			request['<%= Constants.FIELD_PRODUCT_COOPERATION_ID4 %>'] = isUndefined($('#tbCooperationId4').val()) ? '<%: EncodeBackslash(this.ProductInput.CooperationId4) %>' : $('#tbCooperationId4').val();
			request['<%= Constants.FIELD_PRODUCT_COOPERATION_ID5 %>'] = isUndefined($('#tbCooperationId5').val()) ? '<%: EncodeBackslash(this.ProductInput.CooperationId5) %>' : $('#tbCooperationId5').val();
			request['<%= Constants.FIELD_PRODUCT_COOPERATION_ID6 %>'] = isUndefined($('#tbCooperationId6').val()) ? '<%: EncodeBackslash(this.ProductInput.CooperationId6) %>' : $('#tbCooperationId6').val();
			request['<%= Constants.FIELD_PRODUCT_COOPERATION_ID7 %>'] = isUndefined($('#tbCooperationId7').val()) ? '<%: EncodeBackslash(this.ProductInput.CooperationId7) %>' : $('#tbCooperationId7').val();
			request['<%= Constants.FIELD_PRODUCT_COOPERATION_ID8 %>'] = isUndefined($('#tbCooperationId8').val()) ? '<%: EncodeBackslash(this.ProductInput.CooperationId8) %>' : $('#tbCooperationId8').val();
			request['<%= Constants.FIELD_PRODUCT_COOPERATION_ID9 %>'] = isUndefined($('#tbCooperationId9').val()) ? '<%: EncodeBackslash(this.ProductInput.CooperationId9) %>' : $('#tbCooperationId9').val();
			request['<%= Constants.FIELD_PRODUCT_COOPERATION_ID10 %>'] = isUndefined($('#tbCooperationId10').val()) ? '<%: EncodeBackslash(this.ProductInput.CooperationId10) %>' : $('#tbCooperationId10').val();
			request['<%= Constants.FIELD_PRODUCT_INQUIRE_EMAIL %>'] = isUndefined($('#tbInquiteEmail').val()) ? '<%= this.ProductInput.InquireEmail %>' : $('#tbInquiteEmail').val();
			request['<%= Constants.FIELD_PRODUCT_INQUIRE_TEL %>'] = isUndefined($('#tbInquiteTel').val()) ? '<%: EncodeBackslash(this.ProductInput.InquireTel) %>' : $('#tbInquiteTel').val();

			var arrivalMailValidFlg = isUndefined($('#cbArrivalMailValidFlg').val())
				? '<%= this.ProductInput.ArrivalMailValidFlg %>'
				: ($('#cbArrivalMailValidFlg').is(':checked')
					? '<%= Constants.FLG_PRODUCT_ARRIVAL_MAIL_VALID_FLG_VALID %>'
					: '<%= Constants.FLG_PRODUCT_ARRIVAL_MAIL_VALID_FLG_INVALID %>');
			var releaseMailValidFlg = isUndefined($('#cbReleaseMailValidFlg').val())
				? '<%= this.ProductInput.ReleaseMailValidFlg %>'
				: ($('#cbReleaseMailValidFlg').is(':checked')
					? '<%= Constants.FLG_PRODUCT_RELEASE_MAIL_VALID_FLG_VALID %>'
					: '<%= Constants.FLG_PRODUCT_RELEASE_MAIL_VALID_FLG_INVALID %>');
			var resaleMailValidFlg = isUndefined($('#cbResaleMailValidFlg').val())
				? '<%= this.ProductInput.ResaleMailValidFlg %>'
				: ($('#cbResaleMailValidFlg').is(':checked')
					? '<%= Constants.FLG_PRODUCT_RESALE_MAIL_VALID_FLG_VALID %>'
					: '<%= Constants.FLG_PRODUCT_RESALE_MAIL_VALID_FLG_INVALID %>');
			request['<%= Constants.FIELD_PRODUCT_ARRIVAL_MAIL_VALID_FLG %>'] = arrivalMailValidFlg;
			request['<%= Constants.FIELD_PRODUCT_RELEASE_MAIL_VALID_FLG %>'] = releaseMailValidFlg;
			request['<%= Constants.FIELD_PRODUCT_RESALE_MAIL_VALID_FLG %>'] = resaleMailValidFlg;

			var ageLimitFlg = isUndefined($('#cbAgeLimitFlg').val())
				? '<%= this.ProductInput.AgeLimitFlg %>'
				: ($('#cbAgeLimitFlg').is(':checked')
					? '<%= Constants.FLG_PRODUCT_AGE_LIMIT_FLG_VALID %>'
					: '<%= Constants.FLG_PRODUCT_AGE_LIMIT_FLG_INVALID %>');
			request['<%= Constants.FIELD_PRODUCT_AGE_LIMIT_FLG %>'] = (ageLimitFlg != '') ? ageLimitFlg : '<%= Constants.FLG_PRODUCT_AGE_LIMIT_FLG_INVALID %>';
			var addCartUrlLimitFlg = isUndefined($('#cbAddCartUrlLimitFlg').val())
				? '<%= this.ProductInput.AgeLimitFlg %>'
				: ($('#cbAddCartUrlLimitFlg').is(':checked')
					? '<%= Constants.FLG_PRODUCT_ADD_CART_URL_LIMIT_FLG_VALID %>'
					: '<%= Constants.FLG_PRODUCT_ADD_CART_URL_LIMIT_FLG_INVALID %>');
			request['<%= Constants.FIELD_PRODUCT_ADD_CART_URL_LIMIT_FLG %>'] = (addCartUrlLimitFlg != '') ? addCartUrlLimitFlg : '<%= Constants.FLG_PRODUCT_ADD_CART_URL_LIMIT_FLG_INVALID %>';

			request['<%= Constants.FIELD_PRODUCT_GIFT_FLG %>'] = ('<%= Constants.GIFTORDER_OPTION_ENABLED %>' == 'True')
				? isUndefined($('#ddlGiftFlg').val())
					? ('<%= this.ProductInput.GiftFlg %>' != '')
						? '<%= this.ProductInput.GiftFlg %>'
						: '<%= Constants.FLG_PRODUCT_GIFT_FLG_INVALID %>'
					: $('#ddlGiftFlg').val()
				: '<%= Constants.FLG_PRODUCT_GIFT_FLG_INVALID %>';
			request['<%= Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG %>'] = ('<%= Constants.SUBSCRIPTION_BOX_OPTION_ENABLED %>' == 'True')
				? isUndefined($('#ddlSubscriptionBoxFlg').val())
					? ('<%= this.ProductInput.SubscriptionBoxFlg %>' != '')
						? '<%= this.ProductInput.SubscriptionBoxFlg %>'
						: '<%= Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_INVALID %>'
					: $('#ddlSubscriptionBoxFlg').val()
				: '<%= Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_INVALID %>';
			request['<%= Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG %>'] = isUndefined($('#ddlFixedPurchaseFlg').val()) ? '<%= this.ProductInput.FixedPurchaseFlg %>' : $('#ddlFixedPurchaseFlg').val();

			if ('<%= Constants.DIGITAL_CONTENTS_OPTION_ENABLED %>' == 'True') {
				var digitalContentsFlg = isUndefined($('#cbDigitalContentsFlg').val())
					? '<%= this.ProductInput.DigitalContentsFlg %>'
					: ($('#cbDigitalContentsFlg').is(':checked')
						? '<%= Constants.FLG_PRODUCT_DIGITAL_CONTENTS_FLG_VALID %>'
						: '<%= Constants.FLG_PRODUCT_DIGITAL_CONTENTS_FLG_INVALID %>');
				request['<%= Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG %>'] = (digitalContentsFlg != '') ? digitalContentsFlg : '<%= Constants.FLG_PRODUCT_DIGITAL_CONTENTS_FLG_INVALID %>';
				request['<%= Constants.FIELD_PRODUCT_DOWNLOAD_URL %>'] = isUndefined($('#tbDownloadUrl').val())
					? '<%: EncodeBackslash(this.ProductInput.DownloadUrl) %>'
					: $('#tbDownloadUrl').val();
			} else {
				request['<%= Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG %>'] = '<%= Constants.FLG_PRODUCT_DIGITAL_CONTENTS_FLG_INVALID %>';
				request['<%= Constants.FIELD_PRODUCT_DOWNLOAD_URL %>'] = '';
			}

			if ('<%= Constants.MALLCOOPERATION_OPTION_ENABLED %>' == 'True') {
				request['<%= Constants.FIELD_PRODUCT_MALL_EX_PRODUCT_ID %>'] = isUndefined($('#tbMallProductId').val())
					? '<%: Constants.MALLCOOPERATION_RAKUTEN_SKUMIGRATION ? string.Empty : EncodeBackslash(this.ProductInput.MallExProductId) %>'
					: $('#tbMallProductId').val();
				var andMallReservationFlg = isUndefined($('#cbAndMallReservationFlg').val())
				? '<%= this.ProductInput.AndmallReservationFlg %>'
				: ($('#cbAndMallReservationFlg').is(':checked')
					? '<%= Constants.FLG_PRODUCT_ANDMALL_RESERVATION_FLG_RESERVATION %>'
					: '<%= Constants.FLG_PRODUCT_ANDMALL_RESERVATION_FLG_COMMON %>');
				request['<%= Constants.FIELD_PRODUCT_ANDMALL_RESERVATION_FLG %>'] = andMallReservationFlg;
			} else {
				request['<%= Constants.FIELD_PRODUCT_MALL_EX_PRODUCT_ID %>'] = '';
				request['<%= Constants.FIELD_PRODUCT_ANDMALL_RESERVATION_FLG %>'] = '<%= Constants.FLG_PRODUCT_ANDMALL_RESERVATION_FLG_COMMON %>';
			}

			if ('<%= Constants.MEMBER_RANK_OPTION_ENABLED %>' == 'True') {
				var displayFixedPurchaseMemberLimit = isUndefined($('#cbDisplayFixedPurchaseMemberLimit').val())
					? '<%= this.ProductInput.DisplayOnlyFixedPurchaseMemberFlg %>'
					: ($('#cbDisplayFixedPurchaseMemberLimit').is(':checked')
						? '<%= Constants.FLG_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG_ON %>'
						: '<%= Constants.FLG_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG_OFF %>');
				var memberRankDiscountFlg = isUndefined($('#cbMemberRankDiscountFlg').val())
					? '<%= this.ProductInput.MemberRankDiscountFlg %>'
					: ($('#cbMemberRankDiscountFlg').is(':checked')
						? '<%= Constants.FLG_PRODUCT_MEMBER_RANK_DISCOUNT_FLG_VALID %>'
						: '<%= Constants.FLG_PRODUCT_MEMBER_RANK_DISCOUNT_FLG_INVALID %>');
				var buyableFixedPurchaseMemberLimit = isUndefined($('#cbBuyableFixedPurchaseMemberLimit').val())
					? '<%= this.ProductInput.SellOnlyFixedPurchaseMemberFlg %>'
					: ($('#cbBuyableFixedPurchaseMemberLimit').is(':checked')
						? '<%= Constants.FLG_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG_ON %>'
						: '<%= Constants.FLG_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG_OFF %>');

				request['<%= Constants.FIELD_PRODUCT_DISPLAY_MEMBER_RANK %>'] = isUndefined($('#ddlDisplayMemberRank').val()) ? '<%= this.ProductInput.DisplayMemberRank %>' : $('#ddlDisplayMemberRank').val();
				request['<%= Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK %>'] = isUndefined($('#ddlBuyableMemberRank').val()) ? '<%= this.ProductInput.BuyableMemberRank %>' : $('#ddlBuyableMemberRank').val();
				request['<%= Constants.FIELD_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG %>'] = displayFixedPurchaseMemberLimit;
				request['<%= Constants.FIELD_PRODUCT_MEMBER_RANK_DISCOUNT_FLG %>'] = memberRankDiscountFlg;
				request['<%= Constants.FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG %>'] = buyableFixedPurchaseMemberLimit;
			} else {
				request['<%= Constants.FIELD_PRODUCT_DISPLAY_MEMBER_RANK %>'] = '';
				request['<%= Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK %>'] = '';
				request['<%= Constants.FIELD_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG %>'] = '<%= Constants.FLG_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG_OFF %>';
				request['<%= Constants.FIELD_PRODUCT_MEMBER_RANK_DISCOUNT_FLG %>'] = '<%= Constants.FLG_PRODUCT_MEMBER_RANK_DISCOUNT_FLG_INVALID %>';
				request['<%= Constants.FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG %>'] = '<%= Constants.FLG_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG_OFF %>';
			}

			if ('<%= Constants.FIXEDPURCHASE_OPTION_ENABLED %>' == 'True') {
				request['<%= Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING %>'] = getLimitedFixedPurchaseKbnSettingValue('<%= Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING %>');
				request['<%= Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING %>'] = getLimitedFixedPurchaseKbnSettingValue('<%= Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING %>');
				request['<%= Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING %>'] = getLimitedFixedPurchaseKbnSettingValue('<%= Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING %>');
			} else {
				request['<%= Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING %>'] = '';
				request['<%= Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING %>'] = '';
				request['<%= Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING %>'] = '';
			}

			if (($('#ddlFixedPurchaseFlg').val() != '<%= Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID %>') && ('<%= Constants.FIXEDPURCHASE_OPTION_ENABLED %>' == 'True')) {
				request['<%= Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT %>'] = isUndefined($('#tbFixedPurchaseCancelableCount').val()) ? '<%= this.ProductInput.FixedPurchasedCancelableCount %>' : $('#tbFixedPurchaseCancelableCount').val();
				request['<%= Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT %>'] = isUndefined($('#tbFixedPurchaseLimitedSkippedCount').val()) ? '<%= this.ProductInput.FixedPurchaseLimitedSkippedCount %>' : $('#tbFixedPurchaseLimitedSkippedCount').val();
				request['<%= Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_USER_LEVEL_IDS %>'] = isUndefined($('#cbLimitedUserLevel-1').val()) ? '<%= this.ProductInput.FixedPurchasedLimitedUserLevelIds %>' : getCheckBoxListValueFromInput('#cbLimitedUserLevel-', '#divLimitUserLevel');
			} else {
				request['<%= Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT %>'] = '';
				request['<%= Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT %>'] = null;
				request['<%= Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_USER_LEVEL_IDS %>'] = '';
			}

			if (('<%= (Constants.FIXEDPURCHASE_OPTION_ENABLED
				&& GetProductDefaultSettingDisplayField(PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_SETTING)
				&& Constants.FIXEDPURCHASE_NEXTSHIPPING_OPTION_ENABLED) %>' == 'True')
				&& ($('#ddlFixedPurchaseFlg').val() != '<%= Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID %>')) {
				request['<%= Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_ID %>'] = $('#tbFixedPurchaseNextShippingProductId').val();
				request['<%= Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_VARIATION_ID %>'] = $('#tbFixedPurchaseNextShippingProductId').val() + $('#tbFixedPurchaseNextShippingVariationId').val();
				request['<%= Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY %>'] = $('#tbFixedPurchaseNextShippingItemQuantity').val();
				request['<%= Constants.FIELD_PRODUCT_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_KBN %>'] = $('#hfNextShippingItemFixedPurchaseKbn').val();
				request['<%= Constants.FIELD_PRODUCT_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_SETTING %>'] = $('#hfNextShippingItemFixedPurchaseSetting1').val();
			} else {
				request['<%= Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_ID %>'] = '';
				request['<%= Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_VARIATION_ID %>'] = '';
				request['<%= Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY %>'] = '';
				request['<%= Constants.FIELD_PRODUCT_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_KBN %>'] = '';
				request['<%= Constants.FIELD_PRODUCT_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_SETTING %>'] = '';
			}

			var fixedpurchaseDiscountsetting = null;
			if (isUndefined($('#cbShowFixedPurchaseDiscountSetting').val())) {
				if ('<%= this.ProductFixedPurchaseDiscountSetting %>' != '') {
					fixedpurchaseDiscountsetting = JSON.parse('<%= this.ProductFixedPurchaseDiscountSetting %>');
				}
			} else if ($('#cbShowFixedPurchaseDiscountSetting').is(':checked')) {
				saveTableData();
				fixedpurchaseDiscountsetting = productFixedPurchaseDiscountSettingList;
			}
			request['<%= Constants.FIELD_PRODUCT_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_EXTEND %>'] = fixedpurchaseDiscountsetting;
		}

		// Get product extend request
		function getProductExtendRequest(request) {
			var productExtend = {};
			var index = 1;
			$('#divProductExtend .form-element-group-content').each(function () {
				var name = 'extend' + $(this).find('#hfExtendNo-' + index).val();
				productExtend[name] = $('#tbExtend-' + index).val();
				productExtend['name_' + name] = $('#lbExtend-' + index).text();
				index++;
			});
			request['<%= Constants.FIELD_PRODUCT_PRODUCTEXTEND_EXTEND %>'] = productExtend;
		}

		// Get product option setting request
		function getProductOptionSettingRequest(request) {
			<% if (GetDisplayProductOptionValueArea()) { %>
			var productOptionSetting = $('#rbProductOptionValueInputFormBasic-1').is(':checked') ? getProductOptionSettingFromInputBasic().replace(/\n/g, '') : getProductOptionSettingFromInputAdvance();
			<% } else { %>
			var productOptionSetting = '';
			<% } %>
			request['<%= Constants.FIELD_PRODUCT_PRODUCT_OPTION_SETTINGS %>'] = productOptionSetting;
			request['product_option_value_input_basic_type'] = $('#rbProductOptionValueInputFormBasic-1').is(':checked') ? 'basic' : 'advance';
		}

		// Get limited fixed purchase kbn setting value
		function getLimitedFixedPurchaseKbnSettingValue(settingKbn) {
			switch (settingKbn) {
				case '<%= Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING %>':
					if (('<%= GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING) %>' == 'True')
						&& isShowFixedPurchaseKbn1Setting) {
						return getCheckBoxListValueFromInput('#cbLimitedFixedPurchaseKbn1Setting-', '#divLimitedFixedPurchaseKbn1Setting');
					}

				case '<%= Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING %>':
					if (('<%= GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING) %>' == 'True')
						&& isShowFixedPurchaseKbn3Setting) {
						return getCheckBoxListValueFromInput('#cbLimitedFixedPurchaseKbn3Setting-', '#divLimitedFixedPurchaseKbn3Setting');
					}

				case '<%= Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING %>':
					if (('<%= GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING) %>' == 'True')
						&& isShowFixedPurchaseKbn4Setting) {
						return getCheckBoxListValueFromInput('#cbLimitedFixedPurchaseKbn4Setting-', '#divLimitedFixedPurchaseKbn4Setting');
					}
			}
			return '';
		}

		// Get check box list value from input
		function getCheckBoxListValueFromInput(id, divId) {
			var index = 1;
			var results = [];
			$(divId + '> .form-element-group-content-item-inline').each(function () {
				var input = $(this).find(id + index);
				if (input.is(':checked')) {
					results.push(input.val());
				}
				index++;
			});
			return results.join(',');
		}

		// Get mall exhibits config from input
		function getMallExhibitsConfigFromInput() {
			var index = 1;
			var mallExhibitsConfig = {};

			if (isUndefined($('#cbMallExhibitsConfig' + index).val())) {
				var mallExhibitsConfigValue = ('<%= this.ActionStatus %>' != 'insert')
					? JSON.parse('<%= this.MallExhibitsConfig %>')
					: [];
				if (mallExhibitsConfigValue.length > 0) {
					mallExhibitsConfigValue.forEach(function (element) {
						var key = element.key.replace('EXH', 'exhibits_flg');
						mallExhibitsConfig[key] = element.value;
					});
				} else {
					mallExhibitsConfigs.forEach(function (element) {
						var key = element.value.replace('EXH', 'exhibits_flg');
						mallExhibitsConfig[key] = '<%= Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_OFF %>';
					});
				}
			} else {
				$('#divMallExhibitsConfig > .form-element-group-content-item-inline').each(function () {
					var cbMallExhibitsConfig = $(this).find('#cbMallExhibitsConfig' + index);
					var value = (cbMallExhibitsConfig.is(':checked')
						? '<%= Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON %>'
						: '<%= Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_OFF %>');
					var key = cbMallExhibitsConfig.val().replace('EXH', 'exhibits_flg');
					mallExhibitsConfig[key] = value;
					index++;
				});
			}

			return mallExhibitsConfig;
		}

		//Get display variation cooperation
		function getDisplayVariationCooperation(cooperationIndex, variationIndex, value, comment) {
			var html = '<div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">'
				+ '    <div class="form-element-group-title break-text-hover">'
				+ '        <label for="tbVariationCooperationId' + cooperationIndex + variationIndex + '">商品バリエーション連携ID' + cooperationIndex + '</label>';

			if (comment != '') {
				html += '        <p class="note">' + comment + '</p>';
			}

			html += '    </div>'
				+ '    <div class="form-element-group-content product-variation-validate-form-element-group-container">'
				+ '        <input name="variationCooperation' + variationIndex + '" value="' + replaceSpecialChar(value) + '" type="text" id="tbVariationCooperationId' + cooperationIndex + variationIndex + '" maxlength="200" class="w8em" />'
				+ '        <div class="product-variation-error-message-container" data-id="<%= ProductInput.CONST_FIELD_BASE_PRODUCT_VARIATION_COOPERATION_ID %>' + cooperationIndex + '"></div>'
				+ '    </div>'
				+ '</div>';
			return html;
		}

		// Get variation pattern
		function getVariationPattern(index, variation) {
			var isNew = false;
			if (variation == undefined) {
				variation = <%= this.ProductVariationDefaultSetting %>;
				isNew = true;
			}
			var colors = <%= GetProductColors() %>;
			// For subscription box
			var variationIdsIncludeSubscriptionBox = JSON.parse('<%= GetVariationIdsIncludeSubscriptionBoxString() %>');
			var defaultColor = toEmpty(variation['<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_COLOR_ID %>']);

			// Init value
			var option = '';
			var imageSource = '';
			var display = '';
			for (var indexColor = 0; indexColor < colors.length; indexColor++) {
				var selected = (defaultColor == colors[indexColor].Id) ? 'selected' : '';
				if (selected === 'selected') {
					imageSource = colors[indexColor].ImageUrl;
					if (colors[indexColor].ImageUrl == '') {
						display = 'display: none;';
					}
				}

				option += '<option value="' + colors[indexColor].Id + '" ' + selected + ' url="' + colors[indexColor].ImageUrl + '">' + colors[indexColor].DispName + '</option>';
			}

			var variationCooperation1 = ('<%= GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID1) %>' == 'True')
				? getDisplayVariationCooperation(1, index, toEmpty(variation['<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID1 %>']), '<%= GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID1) %>')
				: '';

			var variationCooperation2 = ('<%= GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID2) %>' == 'True')
				? getDisplayVariationCooperation(2, index, toEmpty(variation['<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID2 %>']), '<%= GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID2) %>')
				: '';

			var variationCooperation3 = ('<%= GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID3) %>' == 'True')
				? getDisplayVariationCooperation(3, index, toEmpty(variation['<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID3 %>']), '<%= GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID3) %>')
				: '';

			var variationCooperation4 = ('<%= GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID4) %>' == 'True')
				? getDisplayVariationCooperation(4, index, toEmpty(variation['<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID4 %>']), '<%= GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID4) %>')
				: '';

			var variationCooperation5 = ('<%= GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID5) %>' == 'True')
				? getDisplayVariationCooperation(5, index, toEmpty(variation['<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID5 %>']), '<%= GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID5) %>')
				: '';

			var variationCooperation6 = ('<%= GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID6) %>' == 'True')
				? getDisplayVariationCooperation(6, index, toEmpty(variation['<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID6 %>']), '<%= GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID6) %>')
				: '';

			var variationCooperation7 = ('<%= GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID7) %>' == 'True')
				? getDisplayVariationCooperation(7, index, toEmpty(variation['<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID7 %>']), '<%= GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID7) %>')
				: '';

			var variationCooperation8 = ('<%= GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID8) %>' == 'True')
				? getDisplayVariationCooperation(8, index, toEmpty(variation['<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID8 %>']), '<%= GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID8) %>')
				: '';

			var variationCooperation9 = ('<%= GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID9) %>' == 'True')
				? getDisplayVariationCooperation(9, index, toEmpty(variation['<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID9 %>']), '<%= GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID9) %>')
				: '';

			var variationCooperation10 = ('<%= GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID10) %>' == 'True')
				? getDisplayVariationCooperation(10, index, toEmpty(variation['<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID10 %>']), '<%= GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID10) %>')
				: '';

			var variationId = (variation['<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_ID %>'] != null)
				? variation['<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_ID %>'].slice(variation['<%= Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID %>'].length, variation['<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_ID %>'].length)
				: '';

			// For subscription box
			var isIncludeSubscriptionBox = false;
			variationIdsIncludeSubscriptionBox.forEach(function (item) {
				if (item.key === variationId) {
					isIncludeSubscriptionBox = (item.value === 'True');
					return false;
				}
			});
			var inputVariationArea = '';
			var deleteVariationButtonArea = '';
			if (isIncludeSubscriptionBox && (this.ActionStatus != '<%= Constants.ACTION_STATUS_COPY_INSERT %>')) {
				inputVariationArea = '商品ID＋ <label>' + variationId+ '</label><p style="color:RED">頒布会利用中のため、バリエーションIDの変更・バリエーションの削除は行えません。</p>'
					+ '<input id="tbVariationId' + index + '" value="' + variationId + '" type="hidden" />';
			} else {
				inputVariationArea = '商品ID＋<input id="tbVariationId' + index + '" value="' + variationId + '" type="text" class="w8em" maxlength="30" />';
				deleteVariationButtonArea ='<div class="product-register-variation-form-delete">'
					+ '  <a href="javascript:void(0);" class="btn btn-main btn-size-m product-register-variation-delete-btn">削除</a>'
					+ '</div>'
			}

			var andMallReservationFlgChecked = (variation['<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG %>'] == "<%= Constants.FLG_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG_RESERVATION %>") ? "checked" : "";
			var addCartUrlLimitFlg = (variation['<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG %>'] == "<%= Constants.FLG_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG_VALID %>") ? "checked" : "";
			var pattern = '<div class="product-register-variation-form-row" data-index="' + index + '" data-is-new="' + isNew + '">'
				+ '    <div class="product-register-variation-form-group1">'
				+ '        <div class="form-element-group form-element-group-horizontal-grid">'
				+ '            <div class="form-element-group-title">'
				+ '                <label for="tbVariationId' + index + '">商品バリエーションID<span class="notice">*</span></label>'
				+ '            </div>'
				+ '            <div class="form-element-group-content product-variation-validate-form-element-group-container">'
				+ inputVariationArea
				+ '                <input id="hfRefVatiationId' + index + '" type="hidden" value="' + toEmpty(variation['<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_ID %>']) + '" />'
				+ '                <div class="product-variation-error-message-container" data-id="<%= Constants.FIELD_PRODUCTVARIATION_V_ID %>"></div>'
				+ '                <div class="product-variation-error-message-container" data-id="<%= ProductInput.CONST_ERROR_KEY_PRODUCT_VARIATION_DUPLICATION_ID %>"></div>'
				+ '            </div>'
				+ '        </div>'
				+ '    </div>'
				+ deleteVariationButtonArea
				+ '    <div class="product-register-variation-form-group2">'
				+ '        <div class="product-register-variation-form-group2-col">'
				+ '            <div class="form-element-group form-element-group-horizontal-grid">'
				+ '                <div class="form-element-group-title break-text-hover">'
				+ '                    <label for="tbDisplayOrder' + index + '">表示順<span class="notice">*</span></label>'
				<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_DISPLAY_ORDER)) { %>
				+ '                    <p class="note">' + '<%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_DISPLAY_ORDER) %>' + '</p>'
				<% } %>
				+ '                </div>'
				+ '                <div class="form-element-group-content product-variation-validate-form-element-group-container">'
				+ '                    <input id="tbDisplayOrder' + index + '" value="' + toEmpty(variation['<%= Constants.FIELD_PRODUCTVARIATION_DISPLAY_ORDER %>']) + '" type="text" class="w8em number-textbox" maxlength="3" />'
				+ '                    <div class="product-variation-error-message-container" data-id="<%= Constants.FIELD_PRODUCTVARIATION_DISPLAY_ORDER %>"></div>'
				+ '                </div>'
				+ '            </div>'
				+ '            <div class="form-element-group form-element-group-horizontal-grid">'
				+ '                <div class="form-element-group-title break-text-hover">'
				+ '                    <label for="tbVariationName1' + index + '">表示名1<span class="notice">*</span></label>'
				<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1)) { %>
				+ '                    <p class="note">' + '<%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1) %>' + '</p>'
				<% } %>
				+ '                </div>'
				+ '                <div class="form-element-group-content product-variation-validate-form-element-group-container">'
				+ '                    <input id="tbVariationName1' + index + '" value="' + replaceSpecialChar(toEmpty(variation['<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1 %>'])) + '" type="text" maxlength="30" />'
				+ '                    <div class="product-variation-error-message-container" data-id="<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1 %>"></div>'
				+ '                </div>'
				+ '            </div>'
				<% if (GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2)) { %>
				+ '            <div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">'
				+ '                <div class="form-element-group-title break-text-hover">'
				+ '                    <label for="tbVariationName2' + index + '">表示名2</label>'
				<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2)) { %>
				+ '                    <p class="note">' + '<%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2) %>' + '</p>'
				<% } %>
				+ '                </div>'
				+ '                <div class="form-element-group-content product-variation-validate-form-element-group-container">'
				+ '                    <input id="tbVariationName2' + index + '" value="' + replaceSpecialChar(toEmpty(variation['<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2 %>'])) + '" type="text" maxlength="30" />'
				+ '                    <div class="product-variation-error-message-container" data-id="<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2 %>"></div>'
				+ '                </div>'
				+ '            </div>'
				<% } %>
				<% if (GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3)) { %>
				+ '            <div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">'
				+ '                <div class="form-element-group-title break-text-hover">'
				+ '                    <label for="tbVariationName3' + index + '">表示名3</label>'
				<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3)) { %>
				+ '                    <p class="note">' + '<%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3) %>' + '</p>'
				<% } %>
				+ '                </div>'
				+ '                <div class="form-element-group-content product-variation-validate-form-element-group-container">'
				+ '                    <input id="tbVariationName3' + index + '" value="' + replaceSpecialChar(toEmpty(variation['<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3 %>'])) + '" type="text" maxlength="30" />'
				+ '                    <div class="product-variation-error-message-container" data-id="<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3 %>"></div>'
				+ '                </div>'
				+ '            </div>'
				<% } %>
				<% if (GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_COLOR_ID)) { %>
				+ '            <div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">'
				+ '                <div class="form-element-group-title break-text-hover">'
				+ '                    <label for="ddlVariationColorImage' + index + '">カラー設定</label>'
				<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COLOR_ID)) { %>
				+ '                    <p class="note">' + '<%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_COLOR_ID) %>' + '</p>'
				<% } %>
				+ '                </div>'
				+ '                <div class="form-element-group-content">'
				+ '                    <select name="" id="ddlVariationColorImage' + index + '">'
				+ option
				+ '                    </select>'
				+ '                    &nbsp;&nbsp;&nbsp;<span class="product-color-img"><img id="iColorImage' + index + '" src="' + imageSource + '" style="vertical-align: middle; ' + display + '" height="25" width="25" alt="" /></span>'
				+ '                </div>'
				+ '            </div>'
				<% } %>
				+ '            <div class="form-element-group form-element-group-horizontal-grid">'
				+ '                <div class="form-element-group-title break-text-hover">'
				+ '                    <label for="tbVariationPrice' + index + '">販売価格<span class="notice">*</span></label>'
				<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_PRICE)) { %>
				+ '                    <p class="note">' + '<%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_PRICE) %>' + '</p>'
				<% } %>
				+ '                </div>'
				+ '                <div class="form-element-group-content product-variation-validate-form-element-group-container">'
				+ '                    <input id="tbVariationPrice' + index + '" value="' + toEmpty(variation['<%= Constants.FIELD_PRODUCTVARIATION_PRICE %>']) + '" type="text" class="w8em number-textbox" maxlength="7" />'
				+ '                    <div class="product-variation-error-message-container" data-id="<%= Constants.FIELD_PRODUCTVARIATION_PRICE %>"></div>'
				+ '                </div>'
				+ '            </div>'
				<% if (GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE)) { %>
				+ '            <div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">'
				+ '                <div class="form-element-group-title break-text-hover">'
				+ '                    <label for="tbVariationSpecialPrice' + index + '">特別価格</label>'
				<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE)) { %>
				+ '                    <p class="note">' + '<%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE) %>' + '</p>'
				<% } %>
				+ '                </div>'
				+ '                <div class="form-element-group-content product-variation-validate-form-element-group-container">'
				+ '                    <input id="tbVariationSpecialPrice' + index + '" value="' + toEmpty(variation['<%= Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE %>']) + '" type="text" class="w8em number-textbox" maxlength="7" />'
				+ '                    <div class="product-variation-error-message-container" data-id="<%= Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE %>"></div>'
				+ '                </div>'
				+ '            </div>'
				<% } %>
				<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
				<% if (GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE)) { %>
				+ '            <div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">'
				+ '                <div class="form-element-group-title break-text-hover">'
				+ '                    <label for="tbVariationFixedPurchaseFirsttimePrice' + index + '">定期購入初回価格</label>'
				<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE)) { %>
				+ '                    <p class="note">' + '<%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE) %>' + '</p>'
				<% } %>
				+ '                </div>'
				+ '                <div class="form-element-group-content product-variation-validate-form-element-group-container">'
				+ '                    <input id="tbVariationFixedPurchaseFirsttimePrice' + index + '" value="' + toEmpty(variation['<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE %>']) + '" type="text" class="w8em number-textbox" maxlength="7" />'
				+ '                    <div class="product-variation-error-message-container" data-id="<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE %>"></div>'
				+ '                </div>'
				+ '            </div>'
				<% } %>
				<% if (GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE)) { %>
				+ '            <div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">'
				+ '                <div class="form-element-group-title break-text-hover">'
				+ '                    <label for="tbVariationFixedPurchasePrice' + index + '">定期購入価格</label>'
				<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE)) { %>
				+ '                    <p class="note">' + '<%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE) %>' + '</p>'
				<% } %>
				+ '                </div>'
				+ '                <div class="form-element-group-content product-variation-validate-form-element-group-container">'
				+ '                    <input id="tbVariationFixedPurchasePrice' + index + '" value="' + toEmpty(variation['<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE %>']) + '" type="text" class="w8em number-textbox" maxlength="7" />'
				+ '                    <div class="product-variation-error-message-container" data-id="<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE %>"></div>'
				+ '                </div>'
				+ '            </div>'
				<% } %>
				<% } %>
				<% if (Constants.MEMBER_RANK_OPTION_ENABLED && GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE)) { %>
				+ '            <div class="form-element-group-heading is-form-element-toggle" style="margin: unset;">'
				+ '                <h3 class="form-element-group-heading-label" style="background: #F2F2F2;">会員ランク価格</h3>'
				+ '            </div>'
				+ '            <div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">'
				+ '                <div class="form-element-group-title break-text-hover">'
				<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE)) { %>
				+ '                    <p class="note">' + '<%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE) %>' + '</p>'
				<% } %>
				+ '                </div>'
				+ '            </div>'
				+ '            <div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">'
				+ '                <div class="form-element-group-content product-register-member-rank-price-list form-element-group-horizontal-grid" style="flex-wrap: wrap;"></div>'
				+ '                <div class="form-element-group-content ajax-loading" style="max-width: 240px; background: #ffffff">'
				+ '                    <span class="loading-animation-circle"></span>'
				+ '                </div>'
				+ '            </div>'
				<% } %>
				+ '        </div>'
				+ '        <div class="product-register-variation-form-group2-col">'
				+ '            <div class="form-element-group form-element-group-horizontal-grid">'
				+ '                <div class="form-element-group-title break-text-hover">'
				+ '                    <label for="iVariationImageHead' + index + '"><%= Constants.PRODUCT_IMAGE_HEAD_ENABLED ? "商品画像名ヘッダ" : "商品バリエーション画像" %></label>'
				+ '                    <input id="hfVariationImageHead' + index + '" value="' + toEmpty(variation['<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD %>']) + '" type="hidden" />'
				+ '                </div>'
				+ '                <div class="form-element-group-content">'
				+ '                    <div class="product-register-image-selecter" data-flg-variation="true">'
				+ '                        <div class="product-register-image-selecter-image-main">'
				+ '                            <div class="product-register-image-selecter-image-main-title">画像</div>'
				+ '                        <div class="product-register-image-selecter-image-main-thum">'
				+ '                            <img id="iVariationImageHead' + index + '" src="" alt="" class="product-register-image-selecter-image-main-thum-img">'
				+ '                        </div>'
				+ '                        <label class="product-register-image-selecter-image-main-droparea">'
				+ '                            <input type="file" name="" id="product-register-image-selecter-image-main" class="product-register-image-selecter-image-main-input" accept="image/jpg, image/jpeg">'
				+ '                        </label>'
				+ '                        <div class="product-register-image-selecter-image-main-btns">'
				+ '                            <label for="product-register-image-selecter-image-main" class="product-register-image-selecter-image-main-btn-set btn btn-main btn-size-s">設定する</label>'
				+ '                            <label for="product-register-image-selecter-image-main" class="product-register-image-selecter-image-main-btn-change btn btn-main btn-size-s">変更する</label>'
				+ '                            <a href="javascript:void(0);" class="product-register-image-selecter-image-main-btn-delete btn btn-sub btn-size-s">削除する</a>'
				+ '                        </div>'
				+ '                    </div>'
				<% if (Constants.PRODUCT_IMAGE_HEAD_ENABLED == false) { %>
				+ '                    <label class="product-register-image-selecter-first-droparea" for="product-register-image-selecter-first">'
				+ '                        <p class="product-register-image-selecter-first-droparea-message-drop">ドロップして貼り付け</p>'
				+ '                        <input type="file" name="" id="product-register-image-selecter-first" class="product-register-image-selecter-first-input" accept="image/jpg, image/jpeg">'
				+ '                        <span class="product-register-image-selecter-first-droparea-icon icon-clip"></span>'
				+ '                        <p class="product-register-image-selecter-first-droparea-message">ファイルをドラッグ＆ドロップして<br />ファイルを指定してください。</p>'
				+ '                        <div class="product-register-image-selecter-first-droparea-btns">'
				+ '                            <p class="product-register-image-selecter-first-droparea-btns-text">または</p>'
				+ '                            <a href="javascript:void(0);" class="product-register-image-selecter-first-droparea-btn btn btn-main btn-size-s">ファイルを選択</a>'
				+ '                        </div>'
				+ '                    </label>'
				<% } else { %>
				+ '                    <div class="form-element-group-content">'
				+ '                        <input id="tbVariationImageHead' + index + '" value="' + replaceSpecialChar(toEmpty(variation['<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD %>'])) + '" type="text" maxlength="50" />'
				+ '                    </div>'
				<% } %>
				+ '                </div>'
				+ '                    <br />'
				<% if (Constants.PRODUCT_IMAGE_HEAD_ENABLED == false) { %>
				+ '                    <p class="note">'
				+ '                        ※アップロードする画像のファイル名は下記ルールで自動設定されます。既に同名の画像が存在する場合は画像が上書きされるためご注意ください。<br />'
				+ '                    <p />'
				<% } %>
				+ '                    (商品バリエーション画像の格納先　→　"ROOT/Contents/ProductImages/0/")<br />'
				+ '                    <br />'
				+ '                    例：<%= Constants.PRODUCT_IMAGE_HEAD_ENABLED ? "商品画像名ヘッダがtest" : "商品IDがtest、バリエーションIDが1" %>の場合のファイル名<br />'
				+ '                    <table class="image-explanation-table">'
				+ '                        <tr>'
				+ '                            <td class="edit_title_bg" align="center" style="border-top: none; border-left: none;"></td>'
				+ '                            <td class="edit_title_bg" align="center" style="border-top: none; border-left: none;">画像S</td>'
				+ '                            <td class="edit_title_bg" align="center" style="border-top: none; border-left: none;">画像M</td>'
				+ '                            <td class="edit_title_bg" align="center" style="border-top: none; border-left: none;">画像L</td>'
				+ '                            <td class="edit_title_bg" align="center" style="border-top: none; border-left: none; border-right: none;">画像LL</td>'
				+ '                        </tr>'
				+ '                        <tr>'
				+ '                            <td class="edit_item_bg" align="center" style="border-top: none; border-left: none;">商品画像</td>'
				+ '                            <td class="edit_item_bg" align="center" style="font-size:8pt; border-top: none; border-left: none;"><%= Constants.PRODUCT_IMAGE_HEAD_ENABLED ? "test_S.jpg" : "test_var1_S.jpg" %></td>'
				+ '                            <td class="edit_item_bg" align="center" style="font-size:8pt; border-top: none; border-left: none;"><%= Constants.PRODUCT_IMAGE_HEAD_ENABLED ? "test_M.jpg" : "test_var1_M.jpg" %></td>'
				+ '                            <td class="edit_item_bg" align="center" style="font-size:8pt; border-top: none; border-left: none;"><%= Constants.PRODUCT_IMAGE_HEAD_ENABLED ? "test_L.jpg" : "test_var1_L.jpg" %></td>'
				+ '                            <td class="edit_item_bg" align="center" style="font-size:8pt; border-top: none; border-left: none; border-right: none;"><%= Constants.PRODUCT_IMAGE_HEAD_ENABLED ? "test_LL.jpg" : "test_var1_LL.jpg" %></td>'
				+ '                        </tr>'
				+ '                    </table>'
				<% if (GetDisplayVariationCooperationIdArea()) { %>
				+ '        <div class="form-element-group-heading is-form-element-toggle">'
				+ '            <h3 class="form-element-group-heading-label" style="background: #F2F2F2;">商品バリエーション連携ID</h3>'
				+ '        </div>'
				+ '        <div id="variationCooperation' + index + '">'
				+ variationCooperation1
				+ variationCooperation2
				+ variationCooperation3
				+ variationCooperation4
				+ variationCooperation5
				+ variationCooperation6
				+ variationCooperation7
				+ variationCooperation8
				+ variationCooperation9
				+ variationCooperation10

				+ '        </div>'
				<% } %>
				<% if (GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG)) { %>
				+ '        <div class="form-element-group-heading is-form-element-toggle">'
				+ '            <h3 class="form-element-group-heading-label" style="background: #F2F2F2;">制限設定</h3>'
				+ '        </div>'
				+ '        <div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">'
				+ '            <div class="form-element-group-title break-text-hover">'
				+ '                <label for="cbValiationAddCartUrlLimitFlg' + index + '">カート投入URL制限フラグ</label>'
				<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG)) { %>
				+ '                <p class="note">' + '<%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG) %>' + '</p>'
				<% } %>
				+ '            </div>'
				+ '            <div class="form-element-group-content">'
				+ '                <div class="slide-checkbox-wrap">'
				+ '                    <input id="cbValiationAddCartUrlLimitFlg' + index + '" data-on-label="有効" data-off-label="無効" ' + addCartUrlLimitFlg + ' type="checkbox" />'
				+ '                    <label for="cbValiationAddCartUrlLimitFlg' + index + '" class="slide-checkbox">'
				+ '                        <span class="slide-checkbox-btn"></span>'
				+ '                        <span class="slide-checkbox-label"></span>'
				+ '                    </label>'
				+ '                </div>'
				+ '            </div>'
				+ '        </div>'
				<% } %>
				<% if (GetDisplayVariationMallArea()) { %>
				+ '        <div class="form-element-group-heading is-form-element-toggle">'
				+ '            <h3 class="form-element-group-heading-label" style="background: #F2F2F2;">モール</h3>'
				+ '        </div>'
				<% if (Constants.MALLCOOPERATION_OPTION_ENABLED && GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG)) { %>
				+ '        <div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">'
				+ '            <div class="form-element-group-title break-text-hover">'
				+ '                <label for="cbVariationAndMallReservationFlg' + index + '">＆mall連携予約商品フラグ</label>'
				<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG)) { %>
				+ '                <p class="note">' + '<%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG) %>' + '</p>'
				<% } %>
				+ '            </div>'
				+ '            <div class="form-element-group-content">'
				+ '                <div class="slide-checkbox-wrap">'
				+ '                    <input id="cbVariationAndMallReservationFlg' + index + '" data-on-label="有効" data-off-label="無効" ' + andMallReservationFlgChecked + ' type="checkbox" />'
				+ '                    <label for="cbVariationAndMallReservationFlg' + index + '" class="slide-checkbox">'
				+ '                        <span class="slide-checkbox-btn"></span>'
				+ '                        <span class="slide-checkbox-label"></span>'
				+ '                    </label>'
				+ '                </div>'
				+ '            </div>'
				+ '        </div>'
				<% } %>
				<% if (GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID1)) { %>
				+ '        <div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">'
				+ '            <div class="form-element-group-title break-text-hover">'
				+ '                <h3 for="form-input-9-1-17-1">モールバリエーション</h3>'
				<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID1)) { %>
				+ '                <p class="note">' + '<%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID1) %>' + '</p>'
				<% } %>
				+ '            </div>'
				+ '            <div class="form-element-group-content product-register-variation-mall-variation-list">'
				+ '            </div>'
				+ '        </div>'
				+ '        <div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">'
				+ '            <div class="form-element-group-title">'
				+ '                <label for="tbMallVariationId1' + index + '">ID1</label>'
				+ '            </div>'
				+ '            <div class="form-element-group-content product-variation-validate-form-element-group-container">'
				+ '                 <input id="tbMallVariationId1' + index + '" value="' + replaceSpecialChar(toEmpty(variation['<%= Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID1 %>'])) + '" type="text" class="w8em" maxlength="200" />'
				+ '                 <div class="product-variation-error-message-container" data-id="<%= Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID1 %>"></div>'
				+ '            </div>'
				+ '        </div>'
				+ '        <div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">'
				+ '            <div class="form-element-group-title">'
				+ '                <label for="tbMallVariationId2' + index + '">ID2</label>'
				+ '            </div>'
				+ '            <div class="form-element-group-content product-variation-validate-form-element-group-container">'
				+ '                <input id="tbMallVariationId2' + index + '" value="' + replaceSpecialChar(toEmpty(variation['<%= Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID2 %>'])) + '" type="text" class="w8em" maxlength="200" />'
				+ '                <div class="product-variation-error-message-container" data-id="<%= Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID2 %>"></div>'
				+ '            </div>'
				+ '        </div>'
				+ '        <div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">'
				+ '            <div class="form-element-group-title">'
				+ '                <label for="tbMallVariationType' + index + '">種別</label>'
				+ '            </div>'
				+ '            <div class="form-element-group-content product-variation-validate-form-element-group-container">'
				+ '                <input id="tbMallVariationType' + index + '" value="' + replaceSpecialChar(toEmpty(variation['<%= Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_TYPE %>'])) + '" type="text" class="w8em" maxlength="200" />'
				+ '                <div class="product-variation-error-message-container" data-id="<%= Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_TYPE %>"></div>'
				+ '            </div>'
				+ '        </div>'
				+ '        <p class="note">※モールバリエーションID1が空の場合、バリエーションIDが自動で格納されます</p>'
				<% } %>
				<% } %>
				<% if (Constants.DIGITAL_CONTENTS_OPTION_ENABLED && GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_DOWNLOAD_URL)) { %>
				+ '        <div class="form-element-group-heading is-form-element-toggle">'
				+ '            <h3 class="form-element-group-heading-label" style="background: #F2F2F2;">デジタルコンテンツ</h3>'
				+ '        </div>'
				+ '        <div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">'
				+ '            <div class="form-element-group-title break-text-hover">'
				+ '                <label for="tbVariationDownloadUrl' + index + '">ダウンロードURL</label>'
				<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_DOWNLOAD_URL)) { %>
				+ '                <p class="note">' + '<%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_DOWNLOAD_URL) %>' + '</p>'
				<% } %>
				+ '            </div>'
				+ '            <div class="form-element-group-content product-variation-validate-form-element-group-container">'
				+ '                <input id="tbVariationDownloadUrl' + index + '" value="' + replaceSpecialChar(toEmpty(variation['<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_DOWNLOAD_URL %>'])) + '" type="text" maxlength="1000" />'
				+ '                <div class="product-variation-error-message-container" data-id="<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_DOWNLOAD_URL %>"></div>'
				+ '            </div>'
				+ '        </div>'
				<% } %>
				<% if (Constants.GLOBAL_OPTION_ENABLE && GetVariationDefaultSettingDisplayField(Constants.FIELD_PRODUCTVARIATION_VARIATION_WEIGHT_GRAM)) { %>
				+ '        <div class="form-element-group-heading is-form-element-toggle">'
				+ '            <h3 class="form-element-group-heading-label" style="background: #F2F2F2;">配送設定</h3>'
				+ '        </div>'
				+ '        <div class="form-element-group form-element-group-horizontal-grid is-form-element-toggle">'
				+ '            <div class="form-element-group-title break-text-hover">'
				+ '                <label for="tbVariationWeightGram">重量（g）<span class="notice">*</span></label>'
				<% if (HasVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_WEIGHT_GRAM)) { %>
				+ '                <p class="note">' + '<%: GetVariationDefaultSettingComment(Constants.FIELD_PRODUCTVARIATION_VARIATION_WEIGHT_GRAM) %>' + '</p>'
				<% } %>
				+ '            </div>'
				+ '            <div class="form-element-group-content product-variation-validate-form-element-group-container">'
				+ '                <input name="variationWeightGram" value="' + toEmpty(variation['<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_WEIGHT_GRAM %>']) + '" type="text" id="tbVariationWeightGram' + index + '" placeholder="" class="w8em number-textbox" maxlength="28" />'
				+ '                <div class="product-variation-error-message-container" data-id="<%= Constants.FIELD_PRODUCTVARIATION_VARIATION_WEIGHT_GRAM %>"></div>'
				+ '            </div>'
				+ '        </div>'
				<% } %>
				+ '</div>';
			return pattern;
		}

		// Execute import file
		function execImportFile() {
			var importMasterType = $('#form-input-modal-product-register-master-upload-1').val();
			var file = $('#form-input-modal-product-register-master-upload-2').prop('files')[0];
			$('#form-input-modal-product-register-master-upload-message').empty();
			$('#form-input-modal-product-register-master-upload-error-message').empty();
			if (file == undefined) {
				var html = '<%: WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MASTERUPLOAD_FILE_UNSELECTED) %>';
				$('#form-input-modal-product-register-master-upload-error-message').html(html);
				return;
			}

			var formData = new FormData();
			formData.append('file', file);
			formData.append('<%= Constants.PARAM_PRODUCT_IMPORT_MASTER_TYPE %>', importMasterType);
			formData.append('<%= Constants.PARAM_PRODUCT_PROCESS_ACTION_KBN %>', '<%= Constants.PRODUCT_PROCESS_ACTION_KBN_IMPORT %>');
			var url = "<%: this.ProductHandlingProcessUrl %>";
			var request = callAjaxWithFormData(url, formData);
			request.done(function (response, textStatus, xmlHttpRequest) {
				if (response == null) return;

				checkSessionTimeout(xmlHttpRequest);

				if (response.isSuccess) {
					$('.dd-file-select-file-cancel').trigger('click');

					var message = 'マスタファイルの取込を実行しました。<br /> 終了後メールにてお知らせします。';
					$('#form-input-modal-product-register-master-upload-message').html(message);
					return;
				}

				$('#form-input-modal-product-register-master-upload-error-message').html(response.errorMessage);
			});
		}

		// Add new column or row of table
		function addNew(isCol) {
			saveTableData();
			if (isCol) {
				loadTable(true, false, false);
			} else {
				loadTable(false, true, false);
			}
		}

		// Set fixed purchase discount setting to empty
		function setFixedPurchaseDiscountSettingEmpty() {
			if ($('cbShowFixedPurchaseDiscountSetting').is(':checked') == false) {
				productFixedPurchaseDiscountSettingList = [];
				loadTable(false, false, true);
			}
		}

		// Load table
		function loadTable(addCol, addRow, isFirst) {
			if (isFirst) {
				if ('<%= this.ProductFixedPurchaseDiscountSetting %>' != '') {
					productFixedPurchaseDiscountSettingList = JSON.parse('<%= this.ProductFixedPurchaseDiscountSetting %>');
				}
			}
			var html = ((productFixedPurchaseDiscountSettingList.length == 0) && isFirst) ? setDefaultTableHtml() : addRows(addCol, addRow);
			$('#myTable > tbody').empty();
			$('#myTable > tbody').html(html);
			setInterval(function () {
				setWidthFixedPurchaseDiscountSettingTable();
			}, 500);
		}

		// Set width fixed purchase discount setting table
		function setWidthFixedPurchaseDiscountSettingTable() {
			var parentWidth = parseInt($('.page-product-register').width()) - 350;
			var widthTable = parseInt($('#myTable > tbody').width());
			if (widthTable > parentWidth) {
				$('.sample-checkbox-toggle-area-2').width(parentWidth);
			} else {
				$('.sample-checkbox-toggle-area-2').width('fit-content');
			}
		}

		// Set default table html
		function setDefaultTableHtml() {
			var rowSpan = ('<%= Constants.W2MP_POINT_OPTION_ENABLED %>' == 'True') ? 2 : 1;
			var html = '<tr>'
				+ '  <td></td>'
				+ '  <td colspan="2" align="center" class="product-count-form">'
				+ '    <input class="product-count-col-no" value="1" type="hidden" />'
				+ '    <input class="product-count number-textbox" value="1" type="text" placeholder=""/>個から'
				+ '  </td>'
				+ '</tr>'
				+ '<tr>'
				+ '  <td rowspan="' + rowSpan + '" align="center" class="order-count-form">'
				+ '    <input class="order-count number-textbox" value="1" type="text" placeholder="" />回<br />'
				+ '    <input id="form-input-1-32-1" type="checkbox" class="order-count-more checkbox" name="sample_checkbox" />'
				+ '    <label for="form-input-1-32-1">以降</label>'
				+ '  </td>'
				+ '  <td>値引き</td>'
				+ '  <td class="price-discount-form">'
				+ '    <input class="price-discount-col-no" value="1" type="hidden" />'
				+ '    <input class="price-discount number-textbox" value="" type="text" placeholder="" />'
				+ '    <select class="price-discount-type" style="min-width: auto;">'
				+ '      <option value="YEN" selected="selected">円</option>'
				+ '      <option value="PERCENT">%</option>'
				+ '    </select>'
				+ '  </td>'
				+ '</tr>'
				<% if (Constants.W2MP_POINT_OPTION_ENABLED) { %>
				+ '<tr>'
				+ '  <td>ポイント特典</td>'
				+ '  <td class="point-discount-form">'
				+ '    <input class="point-discount-col-no" value="1" type="hidden" />'
				+ '    <input class="point-discount number-textbox" value="" type="text" placeholder="" />'
				+ '    <select class="point-discount-type" style="min-width: auto;">'
				+ '      <option value="POINT" selected="selected">pt</option>'
				+ '      <option value="PERCENT">%</option>'
				+ '    </select>'
				+ '  </td>'
				+ '</tr>';
				<% } %>
			return html;
		}

		// Save table data
		function saveTableData() {
			productFixedPurchaseDiscountSettingList = [];

			var orderCountList = productFixedPurchaseDiscount.getOrderCountList();
			for (var index = 0; index < orderCountList.length; index++) {
				var orderCount = orderCountList[index].orderCount;
				var isMore = orderCountList[index].isMore;
				var productFixedPurchaseDiscountSettingDetail = setProductFixedPurchaseDiscountSettingDetail(index + 1);
				var productFixedPurchaseDiscountSetting = {
					'<%= Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_ORDER_COUNT %>': orderCount,
					'<%= Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_ORDER_COUNT_MORE_THAN_FLG %>': isMore,
					ProductFixedPurchaseDiscountSettingDetail: productFixedPurchaseDiscountSettingDetail
				};

				productFixedPurchaseDiscountSettingList.push(productFixedPurchaseDiscountSetting);
			}
		}

		// Set product fixed purchase discount setting detail
		function setProductFixedPurchaseDiscountSettingDetail(rowNo) {
			var results = [];
			var priceDiscountList = productFixedPurchaseDiscount.getPriceDiscount();
			var pointDiscountList = productFixedPurchaseDiscount.getPointDiscount();
			var productCountList = productFixedPurchaseDiscount.getProductCountList();
			var startIndex = (rowNo > 1) ? (rowNo * productCountList.length - productCountList.length) : 0;
			for (var index = 0; index < productCountList.length; index++) {
				var result = {
					colNo: productCountList[index].colNo,
					'<%= Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_PRODUCT_COUNT %>': productCountList[index].productCount,
					'<%= Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_VALUE %>': priceDiscountList[startIndex].price,
					'<%= Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE %>': priceDiscountList[startIndex].type,
					'<%= Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_POINT_VALUE %>': ('<%= Constants.W2MP_POINT_OPTION_ENABLED %>' == 'True')
						? pointDiscountList[startIndex].point
						: 0,
					'<%= Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_POINT_TYPE %>': ('<%= Constants.W2MP_POINT_OPTION_ENABLED %>' == 'True')
					? pointDiscountList[startIndex].type
					: '<%= Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_POINT_TYPE_POINT %>'
				};
				startIndex++;
				results.push(result);
			}
			return results;
		}

		// Add rows
		function addRows(addCol, addRow) {
			var show = 'display: block;';
			var hide = 'display: none;';
			var colNo = 1;
			var rowSpan = ('<%= Constants.W2MP_POINT_OPTION_ENABLED %>' == 'True') ? 2 : 1;
			var html = '<tr>' + '<td></td>';

			for (var index = 0; index < productFixedPurchaseDiscountSettingList[0].ProductFixedPurchaseDiscountSettingDetail.length; index++) {
				html += '<td colspan="2" align="center" class="product-count-form">'
					+ '  <input class="product-count-col-no" value="' + colNo + '" type="hidden" />'
					+ '  <input class="product-count number-textbox" value="' + productFixedPurchaseDiscountSettingList[0].ProductFixedPurchaseDiscountSettingDetail[index].product_count + '" type="text" placeholder=""/>個から'
					+ '  <br />'
					+ '  <input type="button" class="btn btn-main btn-size-s" value="削除" onclick="deleteColumn(' + colNo + ')" style="' + (((productFixedPurchaseDiscountSettingList[0].ProductFixedPurchaseDiscountSettingDetail.length > 1) || addCol) ? show : hide) + '" />'
					+ '</td>';
				colNo++;
			}

			if (addCol) {
				html += '<td colspan="2" align="center" class="product-count-form">'
					+ '  <input class="product-count-col-no" value="' + colNo + '" type="hidden" />'
					+ '  <input class="product-count number-textbox" value="' + (Number(productFixedPurchaseDiscountSettingList[0].ProductFixedPurchaseDiscountSettingDetail[productFixedPurchaseDiscountSettingList[0].ProductFixedPurchaseDiscountSettingDetail.length - 1].product_count) + 1) + '" type="text" placeholder=""/>個から'
					+ '  <br />'
					+ '  <input type="button" class="btn btn-main btn-size-s" value="削除" onclick="deleteColumn(' + colNo + ')" style="' + (((productFixedPurchaseDiscountSettingList[0].ProductFixedPurchaseDiscountSettingDetail.length > 1) || addCol) ? show : hide) + '" />'
					+ '</td>';
			}

			html += '</tr>';

			for (var index = 0; index < productFixedPurchaseDiscountSettingList.length; index++) {
				var id = 'form-input-1-32-' + (index + 1);
				var checked = (productFixedPurchaseDiscountSettingList[index].order_count_more_than_flg == '<%= Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_ORDER_COUNT_MORE_THAN_FLG_VALID %>')
					? 'checked'
					: '';
				html += '<tr>'
					+ '  <td rowspan="' + rowSpan + '" align="center" class="order-count-form">'
					+ '    <input class="order-count number-textbox" value="' + productFixedPurchaseDiscountSettingList[index].order_count + '" type="text" placeholder="" />回'
					+ '    <br />'
					+ '    <input id="' + id + '" type="checkbox" class="order-count-more checkbox" name="sample_checkbox" ' + checked + ' />'
					+ '    <label for="' + id + '">以降</label>'
					+ '    <br />'
					+ '    <input type="button" class="btn btn-main btn-size-s" value="削除" onclick="deleteRow(' + index + ')" style="' + (((productFixedPurchaseDiscountSettingList.length > 1) || addRow) ? show : hide) + '" />'
					+ '  </td>'
					+ addPriceDiscount(productFixedPurchaseDiscountSettingList[index].ProductFixedPurchaseDiscountSettingDetail, addCol)
					+ '</tr>'
					<% if (Constants.W2MP_POINT_OPTION_ENABLED) { %>
					+ '<tr>'
					+ addPointDiscount(productFixedPurchaseDiscountSettingList[index].ProductFixedPurchaseDiscountSettingDetail, addCol)
					+ '</tr>';
					<% } %>
			}

			if (addRow) {
				var colCount = productFixedPurchaseDiscountSettingList[0].ProductFixedPurchaseDiscountSettingDetail.length;
				var id = 'form-input-1-32-' + (productFixedPurchaseDiscountSettingList.length + 1);
				html += '<tr>'
					+ '  <td rowspan="' + rowSpan + '" align="center" class="order-count-form">'
					+ '    <input class="order-count number-textbox" value="' + (Number(productFixedPurchaseDiscountSettingList[productFixedPurchaseDiscountSettingList.length - 1].order_count) + 1) + '" type="text" placeholder="" />回<br />'
					+ '    <input id="' + id + '" type="checkbox" class="order-count-more checkbox" name="sample_checkbox" />'
					+ '    <label for="' + id + '">以降</label>'
					+ '    <br />'
					+ '    <input type="button" class="btn btn-main btn-size-s" value="削除" onclick="deleteRow(' + productFixedPurchaseDiscountSettingList.length + ')" style="' + (((productFixedPurchaseDiscountSettingList.length > 1) || addRow) ? show : hide) + '" />'
					+ '  </td>'
					+ addDefaultPriceDiscount(colCount)
					+ '</tr>'
					<% if (Constants.W2MP_POINT_OPTION_ENABLED) { %>
					+ '<tr>'
					+ addDefaultPointDiscount(colCount)
					+ '</tr>';
					<% } %>
			}

			return html;
		}

		// Add default price discount html
		function addDefaultPriceDiscount(count) {
			var html = '';
			var colNo = 1;
			for (var index = 0; index < count; index++) {
				html += '<td>値引き</td>'
					+ '  <td class="price-discount-form">'
					+ '    <input class="price-discount-col-no" value="' + colNo + '" type="hidden" />'
					+ '    <input class="price-discount number-textbox" value="" type="text" placeholder="" />'
					+ '    <select class="price-discount-type" style="min-width: auto;">'
					+ '      <option value="YEN" selected="selected">円</option>'
					+ '      <option value="PERCENT">%</option>'
					+ '    </select>'
					+ '  </td>';
				colNo++;
			}
			return html;
		}

		// Add default point discount html
		function addDefaultPointDiscount(count) {
			var html = '';
			var colNo = 1;
			for (var index = 0; index < count; index++) {
				html += '<td>ポイント特典</td>'
					+ '  <td class="point-discount-form">'
					+ '    <input class="point-discount-col-no" value="' + colNo + '" type="hidden" />'
					+ '    <input class="point-discount number-textbox" value="" type="text" placeholder="" />'
					+ '    <select class="point-discount-type" style="min-width: auto;">'
					+ '      <option value="POINT" selected="selected">pt</option>'
					+ '      <option value="PERCENT">%</option>'
					+ '    </select>'
					+ '  </td>';
				colNo++;
			}
			return html;
		}

		// Add price discount html
		function addPriceDiscount(productFixedPurchaseDiscountSettingDetail, addCol) {
			var html = '';
			var colNo = 1;
			for (var index = 0; index < productFixedPurchaseDiscountSettingDetail.length; index++) {
				var selectedYen = (productFixedPurchaseDiscountSettingDetail[index].discount_type == 'YEN') ? 'selected="selected"' : '';
				var selectedPercent = (productFixedPurchaseDiscountSettingDetail[index].discount_type == 'PERCENT') ? 'selected="selected"' : '';
				html += '<td>値引き</td>'
					+ '<td class="price-discount-form">'
					+ '  <input class="price-discount-col-no" value="' + colNo + '" type="hidden" />'
					+ '  <input class="price-discount number-textbox" value="' + productFixedPurchaseDiscountSettingDetail[index].discount_value.replace('.000', '') + '" type="text" placeholder="" />'
					+ '  <select class="price-discount-type" style="min-width: auto;">'
					+ '    <option value="YEN" ' + selectedYen + '>円</option>'
					+ '    <option value="PERCENT" ' + selectedPercent + '>%</option>'
					+ '  </select>'
					+ '</td>';
				colNo++;
			}

			if (addCol) {
				html += '<td>値引き</td>'
					+ '<td class="price-discount-form">'
					+ '  <input class="price-discount-col-no" value="' + colNo + '" type="hidden" />'
					+ '  <input class="price-discount number-textbox" value="" type="text" placeholder="" />'
					+ '  <select class="price-discount-type" style="min-width: auto;">'
					+ '    <option value="YEN" selected="selected">円</option>'
					+ '    <option value="PERCENT">%</option>'
					+ '  </select>'
					+ '</td>';
			}

			return html;
		}

		// Add point discount html
		function addPointDiscount(productFixedPurchaseDiscountSettingDetail, addCol) {
			var html = '';
			var colNo = 1;
			for (var index = 0; index < productFixedPurchaseDiscountSettingDetail.length; index++) {
				var selectedPoint = (productFixedPurchaseDiscountSettingDetail[index].point_type == 'POINT') ? 'selected="selected"' : '';
				var selectedPercent = (productFixedPurchaseDiscountSettingDetail[index].point_type == 'PERCENT') ? 'selected="selected"' : '';
				html += '<td>ポイント特典</td>'
					+ '<td class="point-discount-form">'
					+ '  <input class="point-discount-col-no" value="' + colNo + '" type="hidden" />'
					+ '  <input class="point-discount number-textbox" value="' + productFixedPurchaseDiscountSettingDetail[index].point_value + '" type="text" placeholder="" />'
					+ '  <select class="point-discount-type" style="min-width: auto;">'
					+ '    <option value="POINT" ' + selectedPoint + '>pt</option>'
					+ '    <option value="PERCENT" ' + selectedPercent + '>%</option>'
					+ '  </select>'
					+ '</td>';
				colNo++;
			}

			if (addCol) {
				html += '<td>ポイント特典</td>'
					+ '<td class="point-discount-form">'
					+ '  <input class="point-discount-col-no" value="' + colNo + '" type="hidden" />'
					+ '  <input class="point-discount number-textbox" value="" type="text" placeholder="" />'
					+ '  <select class="point-discount-type" style="min-width: auto;">'
					+ '    <option value="POINT" selected="selected">pt</option>'
					+ '    <option value="PERCENT">%</option>'
					+ '  </select>'
					+ '</td>';
			}

			return html;
		}

		// Delete table row
		function deleteRow(index) {
			saveTableData();
			productFixedPurchaseDiscountSettingList.splice(index, 1);
			loadTable(false, false, false);
		}

		// Delete column table
		function deleteColumn(colNo) {
			saveTableData();
			productFixedPurchaseDiscountSettingList.forEach(function (productFixedPurchaseDiscountSetting) {
				productFixedPurchaseDiscountSetting.ProductFixedPurchaseDiscountSettingDetail.forEach(function (detail, index) {
					if (detail.colNo == colNo) {
						productFixedPurchaseDiscountSetting.ProductFixedPurchaseDiscountSettingDetail.splice(index, 1);
					}
				});
			});
			loadTable(false, false, false);
		}

		// Reset import file form
		function resetImportFileForm() {
			$('.dd-file-select-file-cancel').trigger('click');
			$('#form-input-modal-product-register-master-upload-message').empty();
			$('#form-input-modal-product-register-master-upload-error-message').empty();
			$("#form-input-modal-product-register-master-upload-1").val($("#form-input-modal-product-register-master-upload-1 option:first").val());
		}

		// Change shipping type
		function changeShippingType() {
			initLimitedFixedPurchaseKbnSetting();
			checkDisplayLimitedFixedPurchaseKbn();
		}

		// Change shipping size
		function changeShippingSize() {
			var text = ($('#ddlShippingSize').val() == 'MAIL') ? '1' : '';
			$('#tbProductSizeFactor').val(text);
		}

		// Check display limited fixedpurchase kbn
		function checkDisplayLimitedFixedPurchaseKbn() {
			$('#divLimitedFixedPurchaseKbn1').hide();
			$('#divLimitedFixedPurchaseKbn3').hide();
			$('#divLimitedFixedPurchaseKbn4').hide();

			if ($('#ddlFixedPurchaseFlg').is(':hidden') || ($('#ddlShippingType').val() == '')) return;

			if (isShowFixedPurchaseKbn1Setting) {
				$('#divLimitedFixedPurchaseKbn1').show();
			}

			if (isShowFixedPurchaseKbn3Setting) {
				$('#divLimitedFixedPurchaseKbn3').show();
			}

			if (isShowFixedPurchaseKbn4Setting) {
				$('#divLimitedFixedPurchaseKbn4').show();
			}
		}

		// Open product list
		function openProductList() {
			var url = '<%= string.Format("{0}{1}?{2}={3}&{4}={5}&{6}=",
				Constants.PATH_ROOT,
				Constants.PAGE_MANAGER_PRODUCT_SEARCH,
				Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN,
				HttpUtility.UrlEncode(Constants.KBN_PRODUCT_SEARCH_ORDERPRODUCT),
				Constants.REQUEST_KEY_PRODUCT_VALID_FLG,
				Constants.FLG_PRODUCT_VALID_FLG_VALID,
				Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE) %>' + $('#ddlShippingType').val();
			var fixed_purchase = '';
			var shipping_type_product_ids = '';
			fixed_purchase = '<%= Constants.FIXEDPURCHASE_OPTION_CART_SEPARATION ? "1" : string.Empty %>';

			<%-- 商品IDをカンマ区切りで連結 --%>
			var product_id = $('#tbProductId').text();
			if (product_id != '') {
				if (shipping_type_product_ids != '') shipping_type_product_ids += ','
				shipping_type_product_ids += product_id;
			}

			url += '&<%= Constants.REQUEST_KEY_PRODUCT_FIXEDPURCHASE_PRODUCT %>=' + encodeURIComponent(fixed_purchase);
			url += '&<%= Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE_PRODUCT_IDS %>=' + encodeURIComponent(shipping_type_product_ids);

			openPopup(url, 'ProductSearch', 'width=850,height=700,top=120,left=420,status=NO,scrollbars=yes', setFixedPurchaseNextShippingValueToInput);
		}

		// Set fixed purchase next shipping value to input
		function setFixedPurchaseNextShippingValueToInput() {
			$('#tbFixedPurchaseNextShippingProductId').val(selectedProductId.toString());
			$('#tbFixedPurchaseNextShippingVariationId').val(selectedVatiationId.toString());
			$('#lbFixedPurchaseNextShippingProductName').html(selectedProductName);
		}

		// 商品一覧で選択された商品情報を設定
		function set_productinfo(product_id, supplier_id, variation_id, product_name) {
			selectedProductId = product_id;
			selectedVatiationId = variation_id;
			selectedProductName = product_name;
		}

		// Clear product
		function clearProduct() {
			$('#tbFixedPurchaseNextShippingProductId').val('');
			$('#tbFixedPurchaseNextShippingVariationId').val('');
			$('#lbFixedPurchaseNextShippingProductName').html('');
			$('#tbFixedPurchaseNextShippingItemQuantity').val('');

			clearFixedPurchaseNextShippingProductErrorMessages();
			clearFixedPurchaseNextShippingSetting();
		}

		// Clear fixed purchase next shipping product error messages
		function clearFixedPurchaseNextShippingProductErrorMessages() {
			$('#tblFixedPurchaseNextShippingProductErrorMessages').css('display', 'none');
			$('#divFixedPurchaseNextShippingProductErrorMessages').html('');
			$('#divFixedPurchaseNextShippingProductErrorMessages').hide();
		}

		// Clear fixed purchase next shipping setting
		function clearFixedPurchaseNextShippingSetting() {
			$('#hfNextShippingItemFixedPurchaseKbn').val('');
			$('#hfNextShippingItemFixedPurchaseSetting1').val('');
			$('#lbNextShippingItemFixedPurchaseShippingPattern').html('設定する');
		}

		// Get product join name
		function getProductJoinName(fixedPurchaseKbn, fixedPurchaseSetting1) {
			showLoading(loadingComponents.productJoinName);
			var urlRequest = "<%: this.ProductRegisterBaseUrl %>/GetProductJoinName";
			var productId = $('#tbFixedPurchaseNextShippingProductId').val();
			var variationId = $('#tbFixedPurchaseNextShippingVariationId').val();
			var dataRequest = { productId: productId, variationId: variationId };
			var request = callAjax(urlRequest, JSON.stringify(dataRequest));
			request.done(function (response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				var message = '<%= WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_OR_PRODUCTVARIATION_NOT_EXIST) %>';

				clearFixedPurchaseNextShippingProductErrorMessages();
				clearFixedPurchaseNextShippingSetting();

				if (data == '') {
					hideLoading(loadingComponents.productJoinName);
					$('#tblFixedPurchaseNextShippingProductErrorMessages').css('display', 'table');
					$('#divFixedPurchaseNextShippingProductErrorMessages').html(message);
					return;
				}

				$('#lbFixedPurchaseNextShippingProductName').html(data);
				$('#hfNextShippingItemFixedPurchaseKbn').val(fixedPurchaseKbn);
				$('#hfNextShippingItemFixedPurchaseSetting1').val(fixedPurchaseSetting1);
				createNextShippingFixedPurchaseSettingMessage();
				hideLoading(loadingComponents.productJoinName);
			});
		}

		// Open popup modify fixed purchase setting
		function openPopupModifyFixedPurchaseSetting() {
			var url = '<%= this.CreateModifyFixedPurchaseSettingUrl(this.ProductInput) %>';
			var fixedPurchaseNextShippingProductId = $('#tbFixedPurchaseNextShippingProductId').val();
			var fixedPurchaseNextShippingVariationId = $('#tbFixedPurchaseNextShippingVariationId').val();
			if (fixedPurchaseNextShippingVariationId != '') {
				url += '&<%= Constants.REQUEST_KEY_PRODUCT_ID %>=' + encodeURIComponent(fixedPurchaseNextShippingProductId);
				url += '&<%= Constants.REQUEST_KEY_VARIATION_ID %>=' + encodeURIComponent(fixedPurchaseNextShippingProductId + fixedPurchaseNextShippingVariationId);
			} else {
				url += '&<%= Constants.REQUEST_KEY_PRODUCT_ID %>=' + encodeURIComponent(fixedPurchaseNextShippingProductId);
				url += '&<%= Constants.REQUEST_KEY_VARIATION_ID %>=' + encodeURIComponent(fixedPurchaseNextShippingProductId);
			}

			var nextShippingItemFixedPurchaseKbn = $('#hfNextShippingItemFixedPurchaseKbn').val();
			var nextShippingItemFixedPurchaseSetting = $('#hfNextShippingItemFixedPurchaseSetting1').val();
			url += '&<%= Constants.REQUEST_KEY_RECOMMENDITEM_FIXED_PURCHASE_KBN %>=' + encodeURIComponent(nextShippingItemFixedPurchaseKbn);
			url += '&<%= Constants.REQUEST_KEY_RECOMMENDITEM_FIXED_PURCHASE_SETTING1 %>=' + encodeURIComponent(nextShippingItemFixedPurchaseSetting);

			window.open(url, 'shipping_pattern', 'width=800,height=400,top=320,left=420,status=NO,scrollbars=yes');
		}

		// 設定した配送パターンをセット
		function set_modify_fixedpurchase_setting(fixed_purchase_kbn, fixed_purchase_setting1) {
			getProductJoinName(fixed_purchase_kbn, fixed_purchase_setting1);
		}

		// Create next shipping fixed purchase setting message
		function createNextShippingFixedPurchaseSettingMessage() {
			if (('<%: Constants.FIXEDPURCHASE_OPTION_ENABLED %>' === 'False')
				|| ('<%: Constants.FIXEDPURCHASE_NEXTSHIPPING_OPTION_ENABLED %>' === 'False')
				|| ('<%: GetProductDefaultSettingDisplayField(PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_SETTING) %>' === 'False')) {
				return;
			}

			showLoading(loadingComponents.nextShippingItemFixedPurchaseShippingPattern);
			var urlRequest = "<%: this.ProductRegisterBaseUrl %>/CreateNextShippingFixedPurchaseSettingMessage";
			var fixedPurchaseKbn = $('#hfNextShippingItemFixedPurchaseKbn').val();
			var fixedPurchaseSetting = $('#hfNextShippingItemFixedPurchaseSetting1').val();
			var dataRequest = { fixedPurchaseKbn: fixedPurchaseKbn, fixedPurchaseSetting: fixedPurchaseSetting };

			var request = callAjax(urlRequest, JSON.stringify(dataRequest));
			request.done(function (response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);

				if (data == '') {
					$('#lbNextShippingItemFixedPurchaseShippingPattern').html('設定する');
					hideLoading(loadingComponents.nextShippingItemFixedPurchaseShippingPattern);
					return;
				}

				$('#lbNextShippingItemFixedPurchaseShippingPattern').html(data);
				hideLoading(loadingComponents.nextShippingItemFixedPurchaseShippingPattern);
			});
		}

		// Set product option index
		function setProductOptionIndex() {
			var htmlIndex = 0;
			for (var index = 1; index < <%= Constants.PRODUCTOPTIONVALUES_MAX_COUNT + 1 %>; index++) {
				if (isUndefined($('#cbProductOptionValue' + index).val()) == false) {
					$('#cbProductOptionValue' + index).data('toggle-content-selector', '.product-register-option-toggle-content-basic-setting:eq(' + htmlIndex + ')');
					$('#ddlProductOptionValue' + index).find('option').each(function () {
						if ($(this).val() == '<%= Constants.PRODUCTOPTIONVALUES_DISP_KBN_CHECKBOX %>') {
							$(this).data('toggle-content-selector', '.product-register-option-toggle-content-basic-setting-style-checkbox:eq(' + htmlIndex + ')');
						} else if ($(this).val() == '<%= Constants.PRODUCTOPTIONVALUES_DISP_KBN_TEXTBOX%>'){
							$(this).data('toggle-content-selector', '.product-register-option-toggle-content-basic-setting-style-text:eq(' + htmlIndex + ')');
						} else if ($(this).val() == '<%= Constants.PRODUCTOPTIONVALUES_DISP_KBN_SELECTMENU %>') {
							$(this).data('toggle-content-selector', '.product-register-option-toggle-content-basic-setting-style-list:eq(' + htmlIndex + ')');
						} else {
							$(this).data('toggle-content-selector', '.product-register-option-toggle-content-basic-setting-style-list-price:eq(' + htmlIndex + ')');
						}
					});
					$('#ddlCheckType' + index).find('option').each(function () {
						switch ($(this).val()) {
							case '<%= w2.Common.Util.Validator.STRTYPE_HALFWIDTH_NUMBER %>':
								$(this).data('toggle-content-selector', '.product-register-option-toggle-content-basic-setting-check-style-number:eq(' + htmlIndex + ')');
								break;

							case '<%= w2.Common.Util.Validator.STRTYPE_FULLWIDTH %>':
							case '<%= w2.Common.Util.Validator.STRTYPE_FULLWIDTH_HIRAGANA %>':
							case '<%= w2.Common.Util.Validator.STRTYPE_FULLWIDTH_KATAKANA %>':
							case '<%= w2.Common.Util.Validator.STRTYPE_HALFWIDTH %>':
							case '<%= w2.Common.Util.Validator.STRTYPE_HALFWIDTH_ALPHNUMSYMBOL %>':
								$(this).data('toggle-content-selector', '.product-register-option-toggle-content-basic-setting-check-style-text:eq(' + htmlIndex + ')');
								break;
						}
					});
					$('#rblFixedLength' + index + '-1').data('toggle-content-selector', '.product-register-option-toggle-content-basic-setting-check-style-text-range:eq(' + htmlIndex + ')');
					$('#rblFixedLength' + index + '-2').data('toggle-content-selector', '.product-register-option-toggle-content-basic-setting-check-style-text-fixed:eq(' + htmlIndex + ')');
					$('.custom-list-input:eq(' + (htmlIndex * 2 - 1) + ')').find('.custom-list-input-list').remove();
					$('.custom-list-input:eq(' + (htmlIndex * 2) + ')').find('.custom-list-input-list').remove();
					$('.custom-list-input-price:eq(' + htmlIndex + ')').find('.custom-list-input-list').remove();
					htmlIndex++;
				}
			}
		}

		// Initialize product option settings
		function initProductOptionSettings(value) {
			setProductOptionIndex();
			if (('<%: EncodeBackslash(this.ProductOptionSettingList) %>' == '') && isUndefined(value)) return;

			var productOptionSettingList = isUndefined(value) ? <%= this.ProductOptionSettingList %> : value;

			if ($('#rbProductOptionValueInputFormBasic-2').is(':checked')) {
				$('.product-register-option-toggle-content-advance').addClass('is-form-element-toggle');
				productOptionSettingList = productOptionSettingList.join('\r\n');
				$('#tbProductOptionValueAdvance').val(productOptionSettingList);
				return;
			}

			var optionIndex = 1;
			productOptionSettingList.forEach(function (element) {
				if (element == null) {
					optionIndex++;
					return;
				}

				if (element.startsWith('[[')) {
					element = element.substring(2, element.length);
				}

				if (element.endsWith(']]')) {
					element = element.substring(0, element.length - 2);
				}

				element = element.split('@@');

				if ((element.length < 3) || (element[1] == '')) {
					optionIndex++;
					return;
				}

				$('#cbProductOptionValue' + optionIndex).prop('checked', true);
				$('#ddlProductOptionValue' + optionIndex).val(element[0]);
				$('#tbProductOptionValue' + optionIndex + 'Name').val(element[1]);


				if (element[0] == '<%= Constants.PRODUCTOPTIONVALUES_DISP_KBN_TEXTBOX %>')
				{
					for (var index = 2; index < element.length; index++) {
						var key = element[index].split('=')[0];
						var value = element[index].split('=')[1];
						switch (key) {
						case 'DefaultValue':
							$('#tbInputDefaultForTb' + optionIndex).val(value);
							break;

						case 'Type':
							$('#ddlCheckType' + optionIndex).val(value);
							break;

						case 'Necessary':
							$('#cbNecessary' + optionIndex).prop('checked', (value != 0));
							break;

						case 'Length':
							$('#rblFixedLength' + optionIndex + '-2').prop('checked', true);
							$('#tbFixedLength' + optionIndex).val(value);
							break;

						case 'LengthMin':
							$('#tbLengthMin' + optionIndex).val(value);
							break;

						case 'LengthMax':
							$('#tbLengthMax' + optionIndex).val(value);
							break;

						case 'MinValue':
							$('#tbNumMin' + optionIndex).val(value);
							break;

						case 'MaxValue':
							$('#tbNumMax' + optionIndex).val(value);
							break;
						}
					}
				} else if (element[0] == '<%= Constants.PRODUCTOPTIONVALUES_DISP_KBN_CHECKBOX %>') {
					var cbDropItems = '#cbDropItems' + optionIndex;
					$(cbDropItems).empty();
					
					const isNecessary = element[element.length - 1].split('=')[1] == 1;
					const isOldOptionType = element[element.length - 1].includes("Necessary") == false;
					if (isNecessary) $('#cbNecessaryForCb' + optionIndex).prop('checked', (value != 0));
					var loopCount = isOldOptionType ? element.length : element.length - 1;
					for (var index = 2; index < loopCount; index++) {
						var option = new Option(element[index], element[index]);
						$(cbDropItems).append(option);
					}
				} else if (element[0] == '<%= Constants.PRODUCTOPTIONVALUES_DISP_KBN_SELECTMENU %>') {
					var ddlDropItems = '#ddlDropItems' + optionIndex;
					$(ddlDropItems).empty();

					const isNecessary = element[element.length - 1].split('=')[1] == 1;
					const isOldOptionType = element[element.length - 1].includes("Necessary") == false;
					if (isNecessary) $('#cbNecessaryForDdl' + optionIndex).prop('checked', (value != 0));
					const loopCount = isOldOptionType ? element.length : element.length - 1;
					for (var index = 2; index < loopCount; index++) {
						if (isNecessary && index == 2) {
							
							if (element[index].includes("DefaultValue"))
							{
								var values = element[index].split('=');
								$('#tbInputDefaultForDdl' + optionIndex).val(values[1]);
								continue;
							}

							$('#tbInputDefaultForDdl' + optionIndex).val(element[index]);
							continue;
						}

						var option = new Option(element[index], element[index]);
						$(ddlDropItems).append(option);
					}
				} else {
					var ddlDropItemsPrice = '#ddlDropItemsPrice' + optionIndex;
					$(ddlDropItemsPrice).empty();

					for (var index = 2; index < element.length; index++) {
						var option = new Option(element[index], element[index]);
						$(ddlDropItemsPrice).append(option);
					}
				}
				optionIndex++;
			});

			toggle.ini();
			customlistInputPrice.ini();
			customlistInput.ini();
		}

		// Get product option setting with input
		function getProductOptionSettingWithIndex(index) {
			var result = '';

			if ($('#cbProductOptionValue' + index).is(':checked')) {
				result += '[[' + $('#ddlProductOptionValue' + index).val()
					+ '@@' + $('#tbProductOptionValue' + index + 'Name').val();

				if ($('#ddlProductOptionValue' + index).val() == '<%= Constants.PRODUCTOPTIONVALUES_DISP_KBN_CHECKBOX %>') {
					$('#cbDropItems' + index).find('option').each(function () {
						result += '@@' + $(this).val();
					});

					result += '@@Necessary=' + ($('#cbNecessaryForCb' + index).prop("checked") ? '1' : '0');
				} else if ($('#ddlProductOptionValue' + index).val() == '<%= Constants.PRODUCTOPTIONVALUES_DISP_KBN_SELECTMENU %>') {
					if ($('#cbNecessaryForDdl' + index).prop("checked")) {
						result += '@@DefaultValue=' + $('#tbInputDefaultForDdl' + index).val();
					}

					$('#ddlDropItems' + index).find('option').each(function () {
						result += '@@' + $(this).val();
					});

					result += '@@Necessary=' + ($('#cbNecessaryForDdl' + index).prop("checked") ? '1' : '0');
				} else if ($('#ddlProductOptionValue' + index).val() == '<%= Constants.PRODUCTOPTIONVALUES_DISP_KBN_TEXTBOX %>') {
					if ($('#tbInputDefaultForTb' + index).val() != '') {
						result += '@@DefaultValue=' + $('#tbInputDefaultForTb' + index).val();
					}

					result += '@@Necessary=' + ($('#cbNecessary' + index).is(':checked') ? '1' : '0');

					if ($('#ddlCheckType' + index).val() != '') {
						result += '@@Type=' + $('#ddlCheckType' + index).val();
					}

					if (isNumber($('#tbFixedLength' + index).val())) {
						result += '@@Length=' + $('#tbFixedLength' + index).val();
					}

					if (isNumber($('#tbLengthMin' + index).val())) {
						result += '@@LengthMin=' + $('#tbLengthMin' + index).val();
					}

					if (isNumber($('#tbLengthMax' + index).val())) {
						result += '@@LengthMax=' + $('#tbLengthMax' + index).val();
					}

					if (isNumber($('#tbNumMin' + index).val())) {
						result += '@@MinValue=' + $('#tbNumMin' + index).val();
					}

					if (isNumber($('#tbNumMax' + index).val())) {
						result += '@@MaxValue=' + $('#tbNumMax' + index).val();
					}

					if ($('#ddlCheckType' + index).val() == 'MAILADDRESS') {
						result += '@@LengthMax=256';
					}
				} else {
					$('#ddlDropItemsPrice' + index).find('option').each(function () {
						result += '@@' + $(this).val();
					});
				}
				result += ']]';
			}
			return result;
		}

		// Check value is number
		function isNumber(value) {
			return ((value != '') && (isNaN(value) == false));
		}

		// Get product option setting from input basic
		function getProductOptionSettingFromInputBasic() {
			var result = [];
			for (var index = 1; index <= parseInt('<%= Constants.PRODUCTOPTIONVALUES_MAX_COUNT %>') ; index++) {
				var productOptionSetting = getProductOptionSettingWithIndex(index);
				if (productOptionSetting != '') {
					result.push(productOptionSetting);
				}
			}
			result = result.join('\n');
			return result;
		}

		// Get product option setting from input advance
		function getProductOptionSettingFromInputAdvance() {
			var result = toEmpty($('#tbProductOptionValueAdvance').val());
			return result.replace(/\n/g, '');
		}

		// Set product option setting to advance
		function setProductOptionSettingToAdvance() {
			var result = getProductOptionSettingFromInputBasic();
			$('#tbProductOptionValueAdvance').val(result);
		}

		// Set product option setting to basic
		function setProductOptionSettingToBasic() {
			clearProductOptionValue();

			var result = $('#tbProductOptionValueAdvance').val();
			var regex = /\[\[.*?\]\]/gm;
			var list = result.match(regex);

			if (list != null) {
				initProductOptionSettings(list);
			}
		}

		// Clear product option value
		function clearProductOptionValue() {
			for (var index = 1; index <= 5; index++) {
				$('#cbProductOptionValue' + index).prop('checked', false);
				$('#tbProductOptionValue' + index + 'Name').val('');
				$('#ddlProductOptionValue' + index).val('<%= Constants.PRODUCTOPTIONVALUES_DISP_KBN_SELECTMENU %>');
				$('#ddlDropItems' + index).empty();
				$('#ddlDropItems' + index).next().remove();
				$('#cbDropItems' + index).empty();
				$('#cbDropItems' + index).next().remove();
				$('#ddlDropItemsPrice' + index).empty();
				$('#ddlDropItemsPrice' + index).next().remove();
				$('#tbInputDefaultForTb' + index).val('');
				$('#ddlCheckType' + index).val('');
				$('#cbNecessary' + index).prop('checked', false);
				$('#cbNecessaryForCb' + index).prop('checked', false);
				$('#cbNecessaryForDdl' + index).prop('checked', false);
				$('#tbFixedLength' + index).val('');
				$('#tbLengthMin' + index).val('');
				$('#tbLengthMax' + index).val('');
				$('#tbNumMin' + index).val('');
				$('#tbNumMax' + index).val('');
				$('#rblFixedLength' + index + '-1').prop('checked', true);
			}
			toggle.ini();
			customlistInputPrice.ini();
			customlistInput.ini();
		}

		// Preview product
		function previewProduct(site) {
			if (checkShippingType() == false) return;

			$('#divSubmitLoading').show();
			$('#divSubmitLoading').addClass('is-open');

			var request = getProductRequest();
			var formData = new FormData();
			formData.append('<%= Constants.PARAM_PRODUCT_INPUT %>', JSON.stringify(request));
			formData.append(
				'<%= Constants.PARAM_PRODUCT_PROCESS_ACTION_KBN %>',
				'<%= (this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
					? Constants.PRODUCT_PROCESS_ACTION_KBN_UPDATE_PREVIEW
					: Constants.PRODUCT_PROCESS_ACTION_KBN_REGIST_PREVIEW %>');
			formData.append('<%= Constants.PARAM_PRODUCT_PREVIEW_SITE %>', site);
			formData.append('<%= Constants.PARAM_GUID_STRING %>', '<%= this.GuidString %>');
			var url = "<%: this.ProductHandlingProcessUrl %>";
			var request = callAjaxWithFormData(url, formData);
			request.done(function (response, textStatus, xmlHttpRequest) {
				// Clear all error contents
				clearAllErrorContents();

				if (response == null) return;

				checkSessionTimeout(xmlHttpRequest);

				if (response.isSuccess) {
					var process = '<%= (this.ActionStatus == Constants.ACTION_STATUS_INSERT) ? Constants.PRODUCT_PROCESS_ACTION_KBN_UPLOAD_IMAGE_INSERT : Constants.PRODUCT_PROCESS_ACTION_KBN_UPLOAD_IMAGE_UPDATE_OR_COPY_INSERT %>';
					var uploadData = getProductImageRequest(process);		
					uploadData.append("<%= Constants.PARAM_GUID_STRING %>", response.guidString);
					uploadData.append("<%= Constants.PARAM_IS_BACK_FROM_CONFIRM %>", '<%: this.IsBackFromConfirm %>');
					var uploadRequest = callAjaxWithFormData(url, uploadData);
					$.when.apply($, uploadRequest).done(function () {
						setTimeout(function () {
							var specs = (site == 'SmartPhone') ? 'width=450,height=800,scrollbars=yes' : '';
							window.open(response.reviewUrl, '_blank', specs);
						}, 1000);
					});
				}

				// bind error contents
				bindErrorContents(response.errorMessage);

				$('#divSubmitLoading').removeClass('is-open');
				$('#divSubmitLoading').hide();
			});
		}

		// Check session timeout
		function checkSessionTimeout(xmlHttpRequest) {
			if (xmlHttpRequest.getResponseHeader("Content-Type").indexOf('text/html') !== -1) {
				window.location.reload();
			}
		}

		// Is undefined value
		function isUndefined(value) {
			return (typeof (value) === 'undefined');
		}

		// To empty
		function toEmpty(value) {
			return value ? value : '';
		}

		// Check shipping type
		function checkShippingType() {
			if ($('#divShippingTypeNodata').is(':visible')) {
				var message = '<%= WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_SHOP_SHIPPING_NO_DATA) %>';
				$('#divMessageNoShipping').html(message);
				$('#divMessageNoShipping').focus();
				return false;
			}
			return true;
		}

		// Replace special characters
		function replaceSpecialChar(object) {
			var object = object.replace(/["]/g, "&quot;")
				.replace(/[']/g, "&apos;")
				.replace(/\\r/g, "\u000A")
				.replace(/\\n/g, "\u000A");
			return object;
		}

		// 関連商品IDリスト取得
		function getRelatedProductIds() {
			var hiddenInputs = $('.custom-product-selector .custom-product-selector-hidden');
			var productIds = [];
			hiddenInputs.each(function() {
				var val = $(this).val();
				if ((val != '') && (productIds.indexOf(val) == -1)) {
					productIds.push(val);
				}
			});

			return productIds
		}

		// カスタム商品選択
		var customProductSelector = {
			wrapperSelector: '.custom-product-selector',
			hiddenInputSelector: '.custom-product-selector-hidden',
			suggestList: null,
			ini: function () {
				var _this = this;
				$(_this.wrapperSelector).each(function () {
					var wrapper = $(this);
					var hiddenInput = wrapper.find(_this.hiddenInputSelector);
					var maxLength = hiddenInput.length;
					var relatedProducts = []
					if (typeof products !== undefined) {
						// ローカル用データをセット
						relatedProducts = products;
					};

					// 初期HTML生成
					var selectedProductHtml = '';
					var selectedProductIds = [];
					hiddenInput.each(function () {
						var val = $(this).val();
						if ((val != '') && (selectedProductIds.indexOf(val) == -1)) {
							relatedProducts.forEach(function (element) {
								if (element.productId == val) {
									selectedProductHtml +=
										'<div class="custom-product-selector-input-selected-item" data-id="' + element.productId + '">\
											<div class="custom-product-selector-input-selected-item-img">' + element.productImg + '</div>\
											<div class="custom-product-selector-input-selected-item-text">\
												<div class="custom-product-selector-input-selected-item-label">'+ element.encodedProductName + '</div>\
												<div class="custom-product-selector-input-selected-item-id">'+ element.productId + '</div>\
											</div>\
											<div class="custom-product-selector-input-selected-item-delete"><span class="icon-close"></span></div>\
										</div>';
									selectedProductIds.push(val);
									return false;
								}
							});
						} else $(this).val('');
					});

					var html =
						'<div class="custom-product-selector-input">\
							<div class="custom-product-selector-input-selected-items">'+ selectedProductHtml + '</div>\
							<div class="custom-product-selector-input-suggest">\
								<input type="text" class="custom-product-selector-input-suggest-search" placeholder="商品ID/商品名を指定してください">\
								<div class="custom-product-selector-input-suggest-list"></div>\
							</div>\
						</div>';
					wrapper.append(html);

					var searchInput = wrapper.find('.custom-product-selector-input-suggest-search');
					var suggestList = wrapper.find('.custom-product-selector-input-suggest-list');
					var focusTimer = null;

					// 各種イベントセット
					var eventSet = function () {
						searchInput.off('blur').on('blur', function () {
							focusTimer = setTimeout(function () {
								suggestList.hide();
							}, 1000);
						});

						// サジェスト動作
						searchInput.off('keypress').on('keypress', function (event) {
							var keycode = (event.keyCode ? event.keyCode : event.which);
							if (keycode == '13') {
								suggestList.empty();

								// サジェスト商品検索
								var url = "<%: this.ProductRegisterBaseUrl %>/GetProductsLikeIdOrName";
								var request = callAjax(url, JSON.stringify({ searchWord: searchInput.val() }));
								request.done(function (response) {
									if ((response == null) || (response.d == undefined)) return false;

									var data = JSON.parse(response.d);
									var suggestListHtml = '';
									data.forEach(function (element) {
										suggestListHtml +=
											'<div class="custom-product-selector-input-suggest-list-item" data-id="' + element.productId + '" title="' + element.encodedProductName + '(' + element.productId + ')">' + element.encodedProductName + '<span class="custom-product-selector-input-suggest-list-item-id">（' + element.productId + '）</span></div>';
									});
									suggestList.html(suggestListHtml);
									suggestList.show();
									suggestListClick();
									clearTimeout(focusTimer);
								});
							}
						});

						// 削除処理
						wrapper.find('.custom-product-selector-input-selected-item-delete').each(function () {
							$(this).off('click').on('click', function () {
								$(this).closest('.custom-product-selector-input-selected-item').remove();
								hiddenDataSet();
							});
						})

						wrapper.find('.select-product').off('click').on('click', function () {
							var isDuplicate = false;
							hiddenInput.each(function () {
								if ($(this).val() == searchInput.val()) isDuplicate = true;
							});

							if (isDuplicate == false) _submit(searchInput.val());
							searchInput.val('');
						})
					}
					eventSet();

					// サジェストリストクリック
					var suggestListClick = function () {
						wrapper.find('.custom-product-selector-input-suggest-list-item').each(function (i) {
							$(this).off('click').on('click', function () {
								var targetId = $(this).data('id');
								var isDuplicate = false;
								hiddenInput.each(function () {
									if ($(this).val() == targetId) isDuplicate = true;
								})
								if (isDuplicate == false) _submit(targetId);
							});
						})
					}

					var _submit = function (targetId) {
						var selectedItems = wrapper.find('.custom-product-selector-input-selected-item').length;
						if (selectedItems < maxLength) {
							var url = "<%: this.ProductRegisterBaseUrl %>/GetProduct";
							var request = callAjax(url, JSON.stringify({ productId: targetId }));
							request.done(function (response) {
								if ((response == null) || (response.d == undefined)) return;
								var data = JSON.parse(response.d);
								var html =
									'<div class="custom-product-selector-input-selected-item" data-id="' + data.productId + '">\
										<div class="custom-product-selector-input-selected-item-img">' + data.productImg + '</div>\
										<div class="custom-product-selector-input-selected-item-text">\
											<div class="custom-product-selector-input-selected-item-label">'+ data.encodedProductName + '</div>\
											<div class="custom-product-selector-input-selected-item-id">'+ data.productId + '</div>\
										</div>\
										<div class="custom-product-selector-input-selected-item-delete"><span class="icon-close"></span></div>\
									</div>';
								wrapper.find('.custom-product-selector-input-selected-items').append(html);
								searchInput.val('');
								eventSet();
								suggestList.hide();
								hiddenDataSet();
							});
						}
					}

					wrapper.append('<p class="custom-product-selector-alert-message" style="display: none">これ以上登録できません（最大' + maxLength + 'つまで）</p>')
					var hiddenDataSet = function () {
						$(hiddenInput).val('');
						wrapper.find('.custom-product-selector-input-selected-item').each(function (i) {
							$(hiddenInput[i]).val($(this).data('id'));
						});
						// 最大値まで登録されているかチェック
						if ($(hiddenInput).last().val() != '') {
							wrapper.find('.custom-product-selector-input-suggest').hide();
							wrapper.find('.custom-product-selector-alert-message').show();
						} else {
							wrapper.find('.custom-product-selector-input-suggest').show();
							wrapper.find('.custom-product-selector-alert-message').hide();
						}
					}

					hiddenDataSet();
				});
			}
		}

</script>
</asp:Content>
