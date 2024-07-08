<%--
=========================================================================================================
  Module      : 注文ワークフロー情報一覧ページ(OrderWorkflowList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OrderWorkflowList.aspx.cs" Inherits="Form_OrderWorkflow_OrderWorkflowList" MaintainScrollPositionOnPostback="true" %>

<%@ Import Namespace="w2.Common.Web" %>
<%@ Import Namespace="w2.App.Common.Order" %>
<%@ Import Namespace="w2.App.Common.Order.Workflow" %>
<%@ Import Namespace="w2.App.Common.Global.Config" %>
<%@ Reference Page="~/Form/PdfOutput/PdfOutput.aspx" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
	<script src="https://cdnjs.cloudflare.com/ajax/libs/url-search-params/1.1.0/url-search-params.js"></script>
	<script type="text/javascript" charset="Shift_JIS" src="<%= this.ResolveClientUrl("~/Js/Manager.workflow.js") %>"></script>
	<style>
		#processInfo
		{
			text-align: center;
			padding-top: 10px;
		}

		.popup_bg td div
		{
			text-align: left;
		}

		.popup_bg
		{
			margin-left: 10px !important;
			margin-right: 10px !important;
		}

		.popup_bg .page-title
		{
			margin-bottom: 10px;
		}

		.popup_bg .list_box_bg td
		{
			text-align: right !important;
		}

		.popup_bg .order-workflow-list-block-data-message
		{
			text-align: center;
		}

		#divDataBody
		{
			display: inline-grid;
		}

		.order-workflow-list-block-data-cassette-list-item-order-info-dest-address,
		.order-workflow-list-block-data-cassette-list-item-order-info-item-name,
		.order-workflow-list-block-data-cassette-list-item-order-info-buyer-email
		{
			word-break: break-all;
			white-space: normal;
		}

		.pt-1
		{
			padding-top: calc(1rem + 3px);
		}

		@media screen and (-ms-high-contrast: active), (-ms-high-contrast: none)
		{
			#dvCheckBoxListPrefectures
			{
				min-width: 100vw;
			}

			#divBottomPager
			{
				float: right;
			}

			.order-workflow-list-block-data
			{
				padding-bottom: 30px;
			}
		}

		table
		{
			table-layout: unset;
		}

		.btn-size-m,
		input[type="submit"].btn-size-m,
		input[type="button"].btn-size-m,
		a.btn-size-m
		{
			font-size: 16px;
			padding: 0 15px;
			height: 32px;
			line-height: 32px;
			white-space: nowrap;
			margin: 0 !important;
		}

		.export-button-list
		{
			display: inline-block;
		}

		.export-button-list input[type="button"].btn-sub
		{
			margin: 5px 2px !important;
		}

		.order-workflow-list-block-validate-message
		{
			display: block;
			color: red;
			font-size: 18px;
			text-align: left;
			padding: 10px 10px 14px;
			box-sizing: border-box;
		}
		.fixed_header_display_none_at_print
		{
			overflow: scroll hidden !important;
		}
		.error-message-extend-status-update,
		.error-message-order-update,
		.error-message-external-payment-auth,
		.error-message-scheduled-shipping,
		.error-message-shipping,
		.error-message-fixed-purchase-extend-status-update,
		.error-message-fixed-purchase-created,
		.error-message-fixed-purchase-changed,
		.error-message-fixed-purchase-last-ordered,
		.error-message-fixed-purchase-begin,
		.error-message-fixed-purchase-next-shipping,
		.error-message-fixed-purchase-next-next-shipping
		{
			text-align: left;
			color: red;
			padding-left: 5px;
			word-wrap: break-word;
			display: none;
		}
	</style>
	<h1 class="page-title">受注ワークフロー</h1>
	<div class="order-workflow-list-blocks">
		<div class="order-workflow-list-block order-workflow-list-block1">
			<div class="order-workflow-list-block-num">
				<div class="order-workflow-list-block-num-inner">
					<span class="order-workflow-list-block-num-number">1</span>
					<span class="order-workflow-list-block-num-text">選択</span>
				</div>
			</div>
			<div class="order-workflow-list-block-content">
				<div class="order-workflow-list-block-header">
					<h2 class="order-workflow-list-block-title">ワークフロー選択</h2>
					<div class="order-workflow-list-block-header-btns">
						<% if (Constants.ORDERWORKFLOW_AUTOEXEC_OPTION_ENABLE)
							{ %>
						<a href="Javascript:openScenarioList();" class="btn btn-sub btn-size-m">シナリオ設定</a>
						<% } %>
						<a href="Javascript:openHistoryList();" class="btn btn-sub btn-size-m">実行履歴</a>
					</div>
					<div class="order-workflow-list-block-tabs">
						<div id="divWorkflowKbnList" class="tabs tabs-type2"></div>
					</div>
				</div>
				<div class="order-workflow-list-block-list-header">
					<div class="order-workflow-list-block-select-view">
						<div class="order-workflow-list-block-select-view-option">
							<input type="radio" id="select-view-list" name="select-view" value="list" checked />
							<label for="select-view-list"><span class="icon icon-list"></span>リスト</label>
						</div>
						<div class="order-workflow-list-block-select-view-option">
							<input type="radio" id="select-view-detail" name="select-view" value="detail" />
							<label for="select-view-detail"><span class="icon icon-detail"></span>詳細</label>
						</div>
					</div>
					<div class="order-workflow-list-block-keyword-search keyword-search">
						<input id="tbWorkflowSearch" type="text" class="order-workflow-list-block-keyword-search-input keyword-search-input" placeholder="絞り込む" onkeydown="searchOrderWorkflow();" />
						<a href="javascript:void(0)" class="order-workflow-list-block-keyword-search-submit keyword-search-submit" onclick="getOrderWorkflowList();">
							<span class="icon-search"></span>
						</a>
					</div>
				</div>
				<div class="order-workflow-list-block-inner">
					<ul id="ulOrderWorkflowList" class="order-workflow-list-block-list">
						<div class="order-workflow-list-block-content-loading" style="border: none;">
							<span class="loading-animation-circle"></span>
						</div>
					</ul>
				</div>
				<div class="order-workflow-list-block-select-other">
					<div style="border: 1px solid #bababa; width: 80%; padding: 15px; border-radius: 5px; display: none" id="divWorkflowRunningMessage">
						<span id="spWorkflowRunningMessage" style="color: red"></span>
						<a href="javascript:void(0)" id="btnOpenRunningHistory" onclick="openWorkflowRunningHistory(this)">履歴詳細はこちら</a>
					</div>
					<a id="lNextWorkflow" href="javascript:void(0)" class="order-workflow-list-block-next-workflow-btn btn btn-main btn-size-m" onclick="getNextWorkflow()">次のワークフローへ</a>
				</div>
			</div>
		</div>
		<div class="order-workflow-list-block order-workflow-list-block2 order-workflow-list-block-fixed">
			<div class="order-workflow-list-block-num">
				<div class="order-workflow-list-block-num-inner">
					<span class="order-workflow-list-block-num-number">2</span>
					<span class="order-workflow-list-block-num-text">実行</span>
				</div>
			</div>
			<div class="order-workflow-list-block-content">
				<div class="order-workflow-list-block-header">
					<h2 class="order-workflow-list-block-title">ワークフロー実行</h2>
				</div>
				<div class="order-workflow-list-block-content-inner">
					<div class="order-workflow-list-block-data" id="divDetailNormalData">
						<div class="order-workflow-list-block-data-header">
							<div class="order-workflow-list-block-data-search">
								<div class="order-workflow-list-block-data-search-form">
									<div class="order-workflow-list-block-data-search-form-title">
										条件を指定して絞り込む
									</div>
									<div id="divOrderSearch">
										<div class="order-workflow-list-block-data-search-pickup">
											<div class="form-element-group-row">
												<div class="form-element-group form-element-group-vertical">
													<div class="form-element-group-title">
														<label for="tbOrderId">注文ID</label>
													</div>
													<div class="form-element-group-content">
														<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_ID %>" type="text" id="tbOrderId" style="width: 200px" class="needInput" onkeypress="keyPressSearch()" />
													</div>
												</div>
												<div class="form-element-group form-element-group-vertical">
													<div class="form-element-group-title">
														<label for="tbProductId">商品ID</label>
													</div>
													<div class="form-element-group-content">
														<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_PRODUCT_ID %>" type="text" id="tbProductId" class="needInput" onkeypress="keyPressSearch()" />
													</div>
												</div>
												<div class="form-element-group form-element-group-vertical">
													<div class="form-element-group-title">
														<label for="tbProductName">商品名</label>
													</div>
													<div class="form-element-group-content">
														<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_PRODUCT_NAME %>" type="text" id="tbProductName" class="needInput" onkeypress="keyPressSearch()" />
													</div>
												</div>
												<div class="form-element-group form-element-group-vertical">
													<div class="form-element-group-title">
														<label for="tbUserId">ユーザーID</label>
													</div>
													<div class="form-element-group-content">
														<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_USER_ID %>" type="text" id="tbUserId" class="needInput" onkeypress="keyPressSearch()" />
													</div>
												</div>
												<% if (Constants.SETPROMOTION_OPTION_ENABLED)
													{ %>
												<div class="form-element-group form-element-group-vertical">
													<div class="form-element-group-title">
														<label for="tbSetPromotionId">セットプロモーションID</label>
													</div>
													<div class="form-element-group-content">
														<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SETPROMOTION_ID %>" type="text" id="tbSetPromotionId" class="needInput" onkeypress="keyPressSearch()" />
													</div>
												</div>
												<% } %>
												<% if (Constants.NOVELTY_OPTION_ENABLED)
													{ %>
												<div class="form-element-group form-element-group-vertical">
													<div class="form-element-group-title">
														<label for="tbNoveltyId">ノベルティID</label>
													</div>
													<div class="form-element-group-content">
														<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_NOVELTY_ID %>" type="text" id="tbNoveltyId" class="needInput" onkeypress="keyPressSearch()" />
													</div>
												</div>
												<% } %>
											</div>
										</div>
										<div class="order-workflow-list-block-data-search-other" style="display: none;">
											<div class="order-workflow-list-block-data-search-more">
												<div class="form-element-group-row">
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label for="tbPaymentOrderId">決済注文ID</label>
														</div>
														<div class="form-element-group-content">
															<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_PAYMENT_ORDER_ID %>" type="text" id="tbPaymentOrderId" class="needInput" onkeypress="keyPressSearch()" />
														</div>
													</div>
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label for="tbCardTranId">決済取引ID</label>
														</div>
														<div class="form-element-group-content">
															<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_CARD_TRAN_ID %>" type="text" id="tbCardTranId" class="needInput" onkeypress="keyPressSearch()" />
														</div>
													</div>
													<% if (Constants.RECOMMEND_OPTION_ENABLED)
														{ %>
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label for="tbRecommendId">レコメンドID</label>
														</div>
														<div class="form-element-group-content">
															<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_RECOMMEND_ID %>" type="text" id="tbRecommendId" class="needInput" onkeypress="keyPressSearch()" />
														</div>
													</div>
													<% } %>
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label for="ddlExternalPaymentStatus">決済ステータス</label>
														</div>
														<div class="form-element-group-content">
															<select name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_STATUS %>" id="ddlExternalPaymentStatus" class="needInput"></select>
														</div>
													</div>
													<% if (OrderCommon.CanDisplayInvoiceBundle())
														{ %>
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label for="ddlInvoiceBundleFlg">請求書同梱フラグ</label>
														</div>
														<div class="form-element-group-content">
															<select name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_INVOICE_BUNDLE_FLG %>" id="ddlInvoiceBundleFlg" class="needInput"></select>
														</div>
													</div>
													<% } %>
												</div>
												<div class="form-element-group-row">
													<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED)
														{ %>
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label for="tbSubscriptionBoxCourseId">頒布会コースID</label>
														</div>
														<div class="form-element-group-content">
															<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SUBSCRIPTION_BOX_COURSE_ID %>" type="text" id="tbSubscriptionBoxCourseId" maxlength="30" class="needInput" onkeypress="keyPressSearch()" />
														</div>
													</div>
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label>頒布会注文回数</label>
														</div>
														<div class="form-element-group-content">
															<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SUBSCRIPTION_BOX_COUNT_FROM %>" type="text" id="tbSubscriptionBoxOrderCountFrom" style="width: 100px" class="needInput" onkeypress="keyPressSearch()" maxlength="38" />
															～
																<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SUBSCRIPTION_BOX_COUNT_TO %>" type="text" id="tbSubscriptionBoxOrderCountTo" style="width: 100px" class="needInput" onkeypress="keyPressSearch()" maxlength="38" />
														</div>
													</div>
													<%} %>
												</div>
												<div class="form-element-group-row">
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label for="ddlExtendStatusUpdateDate">拡張ステータス更新日</label>
														</div>
														<div class="form-element-group-content">
															<select name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_UPDATE_DATE_EXTEND_STATUS_NO %>" id="ddlExtendStatusUpdateDate" class="needInput"></select>
														</div>
													</div>
													<div class="form-element-group form-element-group-vertical pt-1">
														<div class="form-element-group-content">
															<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_EXTEND_STATUS_UPDATE_DATE_FROM %>" type="text" class="input-datepicker needInput" id="dpExtendStatusUpdateDateFrom" onchange="validateDate('extendStatusUpdate')" />
															～
															<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_EXTEND_STATUS_UPDATE_DATE_TO %>" type="text" class="input-datepicker needInput" id="dpExtendStatusUpdateDateTo" onchange="validateDate('extendStatusUpdate')" />
															の間
															<div class="btn-group">
																<div class="btn-group-body">
																	<a href="Javascript:setYesterday('extendStatusUpdate');" class="btn-group-btn">昨日</a>
																	<a href="Javascript:setToday('extendStatusUpdate');" class="btn-group-btn">今日</a>
																	<a href="Javascript:setThisMonth('extendStatusUpdate');" class="btn-group-btn">今月</a>
																	<a href="Javascript:clearDatePeriod('extendStatusUpdate');" class="btn-group-btn">クリア</a>
																</div>
															</div>
														</div>
														<p class="error-message-extend-status-update">拡張ステータス更新日（開始日）は半角の正しい日付形式で入力して下さい。</p>
														<p class="error-message-extend-status-update">拡張ステータス更新日（終了日）は半角の正しい日付形式で入力して下さい。</p>
														<p class="error-message-extend-status-update">拡張ステータス更新日（開始日）は拡張ステータス更新日（終了日）より前に指定して下さい。</p>
													</div>
												</div>
												<div class="form-element-group-row">
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label for="ddlOrderUpdateDate">ステータス更新日</label>
														</div>
														<div class="form-element-group-content">
															<select name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_STATUS %>" id="ddlOrderUpdateDate" class="needInput"></select>
															<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_UPDATE_FROM %>" type="text" class="input-datepicker needInput" id="dpOrderUpdateDateFrom" onchange="validateDate('orderUpdate')" />
															～
															<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_UPDATE_TO %>" type="text" class="input-datepicker needInput" id="dpOrderUpdateDateTo" onchange="validateDate('orderUpdate')" />
															の間
															<div class="btn-group">
																<div class="btn-group-body">
																	<a href="Javascript:setYesterday('orderUpdate');" class="btn-group-btn">昨日</a>
																	<a href="Javascript:setToday('orderUpdate');" class="btn-group-btn">今日</a>
																	<a href="Javascript:setThisMonth('orderUpdate');" class="btn-group-btn">今月</a>
																	<a href="Javascript:clearDatePeriod('orderUpdate');" class="btn-group-btn">クリア</a>
																</div>
															</div>
														</div>
														<p class="error-message-order-update">ステータス更新日（開始日）は半角の正しい日付形式で入力して下さい。</p>
														<p class="error-message-order-update">ステータス更新日（終了日）は半角の正しい日付形式で入力して下さい。</p>
														<p class="error-message-order-update">ステータス更新日（開始日）はステータス更新日（終了日）より前に指定して下さい。</p>
													</div>
												</div>
												<div class="form-element-group-row">
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label>最終与信日時</label>
														</div>
														<div class="form-element-group-content">
															<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_AUTH_DATE_FROM %>" type="text" class="input-datepicker needInput" id="dpExternalPaymentAuthDateFrom" onchange="validateDate('externalPaymentAuth')" />
															～
															<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_AUTH_DATE_TO %>" type="text" class="input-datepicker needInput" id="dpExternalPaymentAuthDateTo" onchange="validateDate('externalPaymentAuth')" />
															の間
															<div class="btn-group">
																<div class="btn-group-body">
																	<a href="Javascript:setYesterday('externalPaymentAuth');" class="btn-group-btn">昨日</a>
																	<a href="Javascript:setToday('externalPaymentAuth');" class="btn-group-btn">今日</a>
																	<a href="Javascript:setThisMonth('externalPaymentAuth');" class="btn-group-btn">今月</a>
																	<a href="Javascript:clearDatePeriod('externalPaymentAuth');" class="btn-group-btn">クリア</a>
																</div>
															</div>
															<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_AUTH_DATE_NONE %>" type="checkbox" id="cbExternalPaymentAuthDateNone" class="needInput checkbox" />
															<label for="cbExternalPaymentAuthDateNone">値なしを含む</label>
														</div>
														<p class="error-message-external-payment-auth">最終与信日時（開始日）は半角の正しい日付形式で入力して下さい。</p>
														<p class="error-message-external-payment-auth">最終与信日時（終了日）は半角の正しい日付形式で入力して下さい。</p>
														<p class="error-message-external-payment-auth">最終与信日時（開始日）は最終与信日時（終了日）より前に指定して下さい。</p>
													</div>
												</div>
												<%--▽ 出荷予定日オプションが有効な場合は表示 ▽--%>
												<div class="form-element-group-row">
													<% if (this.UseLeadTime)
														{ %>
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label>出荷予定日</label>
														</div>
														<div class="form-element-group-content">
															<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SCHEDULED_SHIPPINGDATE_FROM %>" type="text" class="input-datepicker needInput" id="dpScheduledShippingDateFrom" onchange="validateDate('scheduledShipping')" />
															～
															<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SCHEDULED_SHIPPINGDATE_TO %>" type="text" class="input-datepicker needInput" id="dpScheduledShippingDateTo" onchange="validateDate('scheduledShipping')" />
															の間
															<div class="btn-group">
																<div class="btn-group-body">
																	<a href="Javascript:setYesterday('scheduledShipping');" class="btn-group-btn">昨日</a>
																	<a href="Javascript:setToday('scheduledShipping');" class="btn-group-btn">今日</a>
																	<a href="Javascript:setThisMonth('scheduledShipping');" class="btn-group-btn">今月</a>
																	<a href="Javascript:clearDatePeriod('scheduledShipping');" class="btn-group-btn">クリア</a>
																</div>
															</div>
															<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SCHEDULED_SHIPPINGDATE %>" type="checkbox" id="cbScheduledShippingDate" class="needInput" />
															<label for="cbScheduledShippingDate">指定なしを含む</label>
														</div>
														<p class="error-message-scheduled-shipping">出荷予定日（開始日）は半角の正しい日付形式で入力して下さい。</p>
														<p class="error-message-scheduled-shipping">出荷予定日（終了日）は半角の正しい日付形式で入力して下さい。</p>
														<p class="error-message-scheduled-shipping">出荷予定日（開始日）は出荷予定日（終了日）より前に指定して下さい。</p>
													</div>
													<% } %>
												</div>
												<div class="form-element-group-row">
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label>配送希望日</label>
														</div>
														<div class="form-element-group-content">
															<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPINGDATE_FROM %>" type="text" class="input-datepicker needInput" id="dpShippingDateFrom" onchange="validateDate('shipping')" />
															～
															<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPINGDATE_TO %>" type="text" class="input-datepicker needInput" id="dpShippingDateTo" onchange="validateDate('shipping')" />
															の間
															<div class="btn-group">
																<div class="btn-group-body">
																	<a href="Javascript:setYesterday('shipping');" class="btn-group-btn">昨日</a>
																	<a href="Javascript:setToday('shipping');" class="btn-group-btn">今日</a>
																	<a href="Javascript:setThisMonth('shipping');" class="btn-group-btn">今月</a>
																	<a href="Javascript:clearDatePeriod('shipping');" class="btn-group-btn">クリア</a>
																</div>
															</div>
															<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPINGDATE %>" type="checkbox" id="cbShippingDate" class="needInput" />
															<label for="cbShippingDate">指定なしを含む</label>
														</div>
														<p class="error-message-shipping">配送希望日（開始日）は半角の正しい日付形式で入力して下さい。</p>
														<p class="error-message-shipping">配送希望日（終了日）は半角の正しい日付形式で入力して下さい。</p>
														<p class="error-message-shipping">配送希望日（開始日）は配送希望日（終了日）より前に指定して下さい。</p>
													</div>
												</div>
												<div class="form-element-group-row">
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label for="ddlAnotherShippingFlg">配送先</label>
														</div>
														<div class="form-element-group-content">
															<select name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ANOTHER_SHIPPING_FLG %>" id="ddlAnotherShippingFlg" class="needInput"></select>
														</div>
													</div>
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label for="ddlShippingStatus">配送状態</label>
														</div>
														<div class="form-element-group-content">
															<select name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_STATUS %>" id="ddlShippingStatus" class="needInput"></select>
														</div>
													</div>
													<% if (Constants.TWPELICAN_COOPERATION_EXTEND_ENABLED)
														{ %>
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label for="ddlShippingStatusCode">完了状態コード</label>
														</div>
														<div class="form-element-group-content">
															<select name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_STATUS_CODE %>" id="ddlShippingStatusCode" class="needInput"></select>
														</div>
													</div>
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label for="ddlShippingCurrentStatus">現在の状態</label>
														</div>
														<div class="form-element-group-content">
															<select name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_CURRENT_STATUS %>" id="ddlShippingCurrentStatus" class="needInput"></select>
														</div>
													</div>
													<% } %>
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label>拡張ステータス</label>
														</div>
														<div class="form-element-group-content">
															<select name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_EXTEND_STATUS_NO %>" id="ddlExtendStatusNo" class="needInput"></select>が
															<select name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_EXTEND_STATUS %>" id="ddlExtendStatus" class="needInput"></select>のステータス
														</div>
													</div>
												</div>
												<% if (this.IsSearchShippingAddr1)
													{%>
												<div class="form-element-group-row">
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label>配送先：都道府県</label>
														</div>
														<div class="form-element-group-content">
															<span id="LocalArea1">
																<input id="cbLocalArea1" class="needInput" type="checkbox" /><label for="cbLocalArea1" style="font-weight: bold" />北海道</span>
															<span id="LocalArea2">
																<input id="cbLocalArea2" class="needInput" type="checkbox" /><label for="cbLocalArea2" style="font-weight: bold" />東北</span>
															<span id="LocalArea3">
																<input id="cbLocalArea3" class="needInput" type="checkbox" /><label for="cbLocalArea3" style="font-weight: bold" />関東</span>
															<span id="LocalArea4">
																<input id="cbLocalArea4" class="needInput" type="checkbox" /><label for="cbLocalArea4" style="font-weight: bold" />中部</span>
															<span id="LocalArea5">
																<input id="cbLocalArea5" class="needInput" type="checkbox" /><label for="cbLocalArea5" style="font-weight: bold" />近畿</span>
															<span id="LocalArea6">
																<input id="cbLocalArea6" class="needInput" type="checkbox" /><label for="cbLocalArea6" style="font-weight: bold" />中国</span>
															<span id="LocalArea7">
																<input id="cbLocalArea7" class="needInput" type="checkbox" /><label for="cbLocalArea7" style="font-weight: bold" />四国</span>
															<span id="LocalArea8">
																<input id="cbLocalArea8" class="needInput" type="checkbox" /><label for="cbLocalArea8" style="font-weight: bold" />九州/沖縄</span>
															<div id="dvCheckBoxListPrefectures"></div>
														</div>
													</div>
												</div>
												<div>
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label>市区町村</label>
														</div>
														<div class="form-element-group-content">
															<input name="<%= HtmlSanitizer.HtmlEncode(Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_CITY) %>" type="text" style="width: 30%" id="tbShippingCity" class="needInput" onkeypress="keyPressSearch()" />
														</div>
														<div class="form-element-group-content">
														</div>
													</div>
												</div>
												<% } %>
												<div class="form-element-group-row">
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label>注文メモ</label>
														</div>
														<div class="form-element-group-content">
															<select name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_MEMO_FLG %>" id="ddlOrderMemoFlg" class="needInput"></select>
															<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_MEMO %>" type="text" id="tbOrderMemo" class="needInput" onkeypress="keyPressSearch()" />
														</div>
													</div>
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label>管理メモ</label>
														</div>
														<div class="form-element-group-content">
															<select name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_MANAGEMENT_MEMO_FLG %>" id="ddlOrderManagementMemoFlg" class="needInput"></select>
															<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_MANAGEMENT_MEMO %>" type="text" id="tbManagementMemo" class="needInput" onkeypress="keyPressSearch()" />
														</div>
													</div>
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label>配送メモ</label>
														</div>
														<div class="form-element-group-content">
															<select name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_MEMO_FLG %>" id="ddlShippingMemoFlg" class="needInput"></select>
															<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_MEMO %>" class="needInput" type="text" id="tbShippingMemo" onkeypress="keyPressSearch()" />
														</div>
													</div>
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label>決済連携メモ</label>
														</div>
														<div class="form-element-group-content">
															<select name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_PAYMENT_MEMO_FLG %>" id="ddlOrderPaymentMemoFlg" class="needInput"></select>
															<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_PAYMENT_MEMO %>" type="text" id="tbPaymentMemo" class="needInput" onkeypress="keyPressSearch()" />
														</div>
													</div>
												</div>
												<div class="form-element-group-row">
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label>外部連携メモ</label>
														</div>
														<div class="form-element-group-content">
															<select name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_RELATION_MEMO_FLG %>" id="ddlOrderRelationMemoFlg" class="needInput"></select>
															<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_RELATION_MEMO %>" type="text" id="tbRelationMemo" class="needInput" onkeypress="keyPressSearch()" />
														</div>
													</div>
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label>ユーザー特記欄</label>
														</div>
														<div class="form-element-group-content">
															<select name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_USER_MEMO_FLG %>" id="ddlUserMemoFlg" class="needInput"></select>
															<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_USER_MEMO %>" type="text" id="tbUserMemo" class="needInput" onkeypress="keyPressSearch()" />
														</div>
													</div>
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label>商品付帯情報</label>
														</div>
														<div class="form-element-group-content">
															<select name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_PRODUCT_OPTION_FLG %>" id="ddlProductOptionFlg" class="needInput"></select>
															<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_PRODUCT_OPTION_TEXTS %>" type="text" id="tbProductOption" class="needInput" onkeypress="keyPressSearch()" />
														</div>
													</div>
												</div>
												<div class="form-element-group-row">
													<% if (Constants.RECEIPT_OPTION_ENABLED)
														{ %>
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label>領収書希望</label>
														</div>
														<div class="form-element-group-content">
															<select name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_RECEIPT_FLG %>" id="ddlOrderReceiptFlg" class="needInput"></select>
														</div>
													</div>
													<% } %>
													<% if (OrderCommon.DisplayTwInvoiceInfo())
														{ %>
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label>發票ステータス</label>
														</div>
														<div class="form-element-group-content">
															<select name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_TW_INVOICE_STATUS %>" id="ddlInvoiceStatus" class="needInput"></select>
														</div>
													</div>
													<% } %>
												</div>
												<% if (Constants.ORDER_EXTEND_OPTION_ENABLED)
													{ %>
												<div class="form-element-group-row">
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label>注文拡張項目</label>
														</div>
														<div class="form-element-group-content">
															<select name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_NAME %>" id="ddlOrderExtendName" style="max-width: 24em;" class="needInput"></select>
															<select name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_FLG %>" id="ddlOrderExtendFlg" class="needInput"></select>
															<input name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_TEXT %>" type="text" id="tbOrderExtendText" class="needInput" />
															<select name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_TEXT %>" id="ddlOrderExtendText" style="max-width: 24em;" class="needInput"></select>
														</div>
													</div>
												</div>
												<% } %>
												<% if (Constants.STORE_PICKUP_OPTION_ENABLED) { %>
												<div class="form-element-group-row">
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label>店舗受取ステータス</label>
														</div>
														<div class="form-element-group-content">
															<select name="<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_STORE_PICKUP_STATUS %>" id="ddlStorePickupStatus" style="max-width: 24em;" class="needInput"></select>
														</div>
													</div>
												</div>
												<% } %>
											</div>
										</div>
									</div>
									<div id="divFixedPurchaseSearch">
										<div class="order-workflow-list-block-data-search-pickup">
											<div class="form-element-group-row">
												<div class="form-element-group form-element-group-vertical">
													<div class="form-element-group-title">
														<label>定期購入ID</label>
													</div>
													<div class="form-element-group-content">
														<input name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_FIXEDPURCHASE_ID %>" type="text" id="tbFixedPurchaseId" class="needInput" onkeypress="keyPressSearch()" />
													</div>
												</div>
												<div class="form-element-group form-element-group-vertical">
													<div class="form-element-group-title">
														<label>商品ID</label>
													</div>
													<div class="form-element-group-content">
														<input name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_FIXEDPURCHASE_PRODUCT_ID %>" type="text" id="tbFixedPurchaseProductId" class="needInput" onkeypress="keyPressSearch()" />
													</div>
												</div>
												<div class="form-element-group form-element-group-vertical">
													<div class="form-element-group-title">
														<label>商品名</label>
													</div>
													<div class="form-element-group-content">
														<input name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_FIXEDPURCHASE_PRODUCT_NAME %>" type="text" id="tbFixedPurchaseProductName" class="needInput" onkeypress="keyPressSearch()" />
													</div>
												</div>
												<div class="form-element-group form-element-group-vertical">
													<div class="form-element-group-title">
														<label>ユーザーID</label>
													</div>
													<div class="form-element-group-content">
														<input name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_USER_ID %>" type="text" id="tbFixedPurchaseUserId" class="needInput" onkeypress="keyPressSearch()" />
													</div>
												</div>
											</div>
										</div>
										<div class="order-workflow-list-block-data-search-other" style="display: none;">
											<div class="order-workflow-list-block-data-search-more">
												<div class="form-element-group-row">
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label>定期購入ステータス</label>
														</div>
														<div class="form-element-group-content">
															<select name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_FIXEDPURCHASE_STAUS %>" id="ddlFixedPurchaseStatus" class="needInput"></select>
														</div>
													</div>
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label>決済ステータス</label>
														</div>
														<div class="form-element-group-content">
															<select name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_FIXEDPURCHASE_PAYMENT_STAUS %>" id="ddlFixedPurchasePaymentStatus" class="needInput"></select>
														</div>
													</div>
													<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED)
														{ %>
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label>頒布会コースID</label>
														</div>
														<div class="form-element-group-content">
															<input name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_SUBSCRIPTION_BOX_COURSE_ID %>" type="text" id="tbFixedPurchaseSubscriptionBoxCourseId" maxlength="30" class="needInput" onkeypress="keyPressSearch()" />
														</div>
													</div>
													<%} %>
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label>購入回数(注文基準)</label>
														</div>
														<div class="form-element-group-content">
															<input name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDERED_COUNT_FROM %>" type="text" id="tbOrderCountFrom" style="width: 100px" class="needInput" onkeypress="keyPressSearch()" />
															～
															<input name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDERED_COUNT_TO %>" type="text" id="tbOrderCountTo" style="width: 100px" class="needInput" onkeypress="keyPressSearch()" />
														</div>
													</div>
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label>購入回数(出荷基準)</label>
														</div>
														<div class="form-element-group-content">
															<input name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_SHIPPED_COUNT_FROM %>" type="text" id="tbShippedCountFrom" style="width: 100px" class="needInput" onkeypress="keyPressSearch()" />
															～
															<input name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_SHIPPED_COUNT_TO %>" type="text" id="tbShippedCountTo" style="width: 100px" class="needInput" onkeypress="keyPressSearch()" />
														</div>
													</div>
													<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED)
														{ %>
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label>頒布会注文回数</label>
														</div>
														<div class="form-element-group-content">
															<input name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_SUBSCRIPTION_BOX_COUNT_FROM %>" type="text" id="tbFixedPurchaseSubscriptionBoxOrderCountFrom" style="width: 100px" class="needInput" onkeypress="keyPressSearch()" />
															～
																<input name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_SUBSCRIPTION_BOX_COUNT_TO %>" type="text" id="tbFixedPurchaseSubscriptionBoxOrderCountTo" style="width: 100px" class="needInput" onkeypress="keyPressSearch()" />
														</div>
													</div>
													<%} %>
												</div>
												<div class="form-element-group-row">
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label>管理メモ</label>
														</div>
														<div class="form-element-group-content">
															<select name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_MANAGEMENT_MEMO_FLG %>" id="ddlFixedPurchaseManagementMemoFlg" class="needInput"></select>
															<input name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_MANAGEMENT_MEMO %>" type="text" id="tbFixedPurchaseManagementMemo" class="needInput" onkeypress="keyPressSearch()" />
														</div>
													</div>
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label>配送メモ</label>
														</div>
														<div class="form-element-group-content">
															<select name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_SHIPPING_MEMO_FLG %>" id="ddlFixedPurchaseShippingMemoFlg" class="needInput"></select>
															<input name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_SHIPPING_MEMO %>" type="text" id="tbFixedPurchaseShippingMemo" class="needInput" onkeypress="keyPressSearch()" />
														</div>
													</div>
													<% if (Constants.RECEIPT_OPTION_ENABLED)
														{ %>
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label>領収書希望</label>
														</div>
														<div class="form-element-group-content">
															<select name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_RECEIPT_FLG %>" id="ddlFixedPurchaseReceiptFlg" class="needInput"></select>
														</div>
													</div>
													<% } %>
												</div>
												<div class="form-element-group-row">
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label>拡張ステータス更新日</label>
														</div>
														<div class="form-element-group-content">
															<select name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_EXTEND_STATUS_NO_UPDATE_DATE %>" id="ddlFixedPurchaseExtendStatusUpdateDate" class="needInput"></select>
														</div>
													</div>
													<div class="form-element-group form-element-group-vertical pt-1">
														<div class="form-element-group-content">
															<input name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_EXTEND_STATUS_UPDATE_DATE_FROM %>" type="text" class="input-datepicker needInput" id="dpFixedPurchaseExtendStatusFrom" onchange="validateDate('fixedPurchaseExtendStatusUpdate')" />
															～
															<input name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_EXTEND_STATUS_UPDATE_DATE_TO %>" type="text" class="input-datepicker needInput" id="dpFixedPurchaseExtendStatusTo" onchange="validateDate('fixedPurchaseExtendStatusUpdate')" />
															の間
															<div class="btn-group">
																<div class="btn-group-body">
																	<a href="Javascript:setYesterday('fixedPurchaseExtendStatusUpdate')" class="btn-group-btn">昨日</a>
																	<a href="Javascript:setToday('fixedPurchaseExtendStatusUpdate');" class="btn-group-btn">今日</a>
																	<a href="Javascript:setThisMonth('fixedPurchaseExtendStatusUpdate');" class="btn-group-btn">今月</a>
																	<a href="Javascript:clearDatePeriod('fixedPurchaseExtendStatusUpdate');" class="btn-group-btn">クリア</a>
																</div>
															</div>
														</div>
														<p class="error-message-fixed-purchase-extend-status-update">拡張ステータス更新日（開始日）は半角の正しい日付形式で入力して下さい。</p>
														<p class="error-message-fixed-purchase-extend-status-update">拡張ステータス更新日（終了日）は半角の正しい日付形式で入力して下さい。</p>
														<p class="error-message-fixed-purchase-extend-status-update">拡張ステータス更新日（開始日）は拡張ステータス更新日（終了日）より前に指定して下さい。</p>
													</div>
												</div>
												<div class="form-element-group-row">
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															拡張ステータス
														</div>
														<div class="form-element-group-content">
															<select name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_EXTEND_STATUS_NO %>" id="ddlFixedPurchaseExtendStatusNo" class="needInput"></select>が
															<select name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_EXTEND_STATUS %>" id="ddlFixedPurchaseExtendStatus" class="needInput"></select>のステータス
														</div>
													</div>
												</div>
												<div class="form-element-group-row">
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															作成日
														</div>
														<div class="form-element-group-content">
															<input name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_CREATED_FROM %>" type="text" id="dpFixedPurchaseDateCreatedFrom" class="input-datepicker needInput" onchange="validateDate('fixedPurchaseCreated')" />
															～
															<input name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_CREATED_TO %>" type="text" id="dpFixedPurchaseDateCreatedTo" class="input-datepicker needInput" onchange="validateDate('fixedPurchaseCreated')" />
															の間
															<div class="btn-group">
																<div class="btn-group-body">
																	<a href="Javascript:setYesterday('fixedPurchaseCreated');" class="btn-group-btn">昨日</a>
																	<a href="Javascript:setToday('fixedPurchaseCreated');" class="btn-group-btn">今日</a>
																	<a href="Javascript:setThisMonth('fixedPurchaseCreated');" class="btn-group-btn">今月</a>
																	<a href="Javascript:clearDatePeriod('fixedPurchaseCreated');" class="btn-group-btn">クリア</a>
																</div>
															</div>
														</div>
														<p class="error-message-fixed-purchase-created">作成日（開始日）は半角の正しい日付形式で入力して下さい。</p>
														<p class="error-message-fixed-purchase-created">作成日（終了日）は半角の正しい日付形式で入力して下さい。</p>
														<p class="error-message-fixed-purchase-created">作成日（開始日）は作成日（終了日）より前に指定して下さい。</p>
													</div>
												</div>
												<div class="form-element-group-row">
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															更新日
														</div>
														<div class="form-element-group-content">
															<input name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_CHANGED_FROM %>" type="text" id="dpFixedPurchaseDateChangedFrom" class="input-datepicker needInput" onchange="validateDate('fixedPurchaseChanged')" />
															～
															<input name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_CHANGED_TO %>" type="text" id="dpFixedPurchaseDateChangedTo" class="input-datepicker needInput" onchange="validateDate('fixedPurchaseChanged')" />
															の間
															<div class="btn-group">
																<div class="btn-group-body">
																	<a href="Javascript:setYesterday('fixedPurchaseChanged');" class="btn-group-btn">昨日</a>
																	<a href="Javascript:setToday('fixedPurchaseChanged');" class="btn-group-btn">今日</a>
																	<a href="Javascript:setThisMonth('fixedPurchaseChanged');" class="btn-group-btn">今月</a>
																	<a href="Javascript:clearDatePeriod('fixedPurchaseChanged');" class="btn-group-btn">クリア</a>
																</div>
															</div>
														</div>
														<p class="error-message-fixed-purchase-changed">更新日（開始日）は半角の正しい日付形式で入力して下さい。</p>
														<p class="error-message-fixed-purchase-changed">更新日（終了日）は半角の正しい日付形式で入力して下さい。</p>
														<p class="error-message-fixed-purchase-changed">更新日（開始日）は更新日（開始日）より前に指定して下さい。</p>
													</div>
												</div>
												<div class="form-element-group-row">
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															最終購入日
														</div>
														<div class="form-element-group-content">
															<input name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_LAST_ORDERED_FROM %>" type="text" id="dpFixedPurchaseDateLastOrderedFrom" class="input-datepicker needInput" onchange="validateDate('fixedPurchaseLastOrdered')" />
															～
															<input name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_LAST_ORDERED_TO %>" type="text" id="dpFixedPurchaseDateLastOrderedTo" class="input-datepicker needInput" onchange="validateDate('fixedPurchaseLastOrdered')" />
															の間
															<div class="btn-group">
																<div class="btn-group-body">
																	<a href="Javascript:setYesterday('fixedPurchaseLastOrdered');" class="btn-group-btn">昨日</a>
																	<a href="Javascript:setToday('fixedPurchaseLastOrdered');" class="btn-group-btn">今日</a>
																	<a href="Javascript:setThisMonth('fixedPurchaseLastOrdered');" class="btn-group-btn">今月</a>
																	<a href="Javascript:clearDatePeriod('fixedPurchaseLastOrdered');" class="btn-group-btn">クリア</a>
																</div>
															</div>
														</div>
														<p class="error-message-fixed-purchase-last-ordered">最終購入日（開始日）は半角の正しい日付形式で入力して下さい。</p>
														<p class="error-message-fixed-purchase-last-ordered">最終購入日（終了日）は半角の正しい日付形式で入力して下さい。</p>
														<p class="error-message-fixed-purchase-last-ordered">最終購入日（開始日）は最終購入日（終了日）より前に指定して下さい。</p>
													</div>
												</div>
												<div class="form-element-group-row">
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															定期購入開始日
														</div>
														<div class="form-element-group-content">
															<input name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_BGN_FROM %>" type="text" id="dpFixedPurchaseDateBeginFrom" class="input-datepicker needInput" onchange="validateDate('fixedPurchaseBegin')" />
															～
															<input name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_BGN_TO %>" type="text" id="dpFixedPurchaseDateBeginTo" class="input-datepicker needInput" onchange="validateDate('fixedPurchaseBegin')" />
															の間
															<div class="btn-group">
																<div class="btn-group-body">
																	<a href="Javascript:setYesterday('fixedPurchaseBegin');" class="btn-group-btn">昨日</a>
																	<a href="Javascript:setToday('fixedPurchaseBegin');" class="btn-group-btn">今日</a>
																	<a href="Javascript:setThisMonth('fixedPurchaseBegin');" class="btn-group-btn">今月</a>
																	<a href="Javascript:clearDatePeriod('fixedPurchaseBegin');" class="btn-group-btn">クリア</a>
																</div>
															</div>
														</div>
														<p class="error-message-fixed-purchase-begin">定期購入開始日（開始日）は半角の正しい日付形式で入力して下さい。</p>
														<p class="error-message-fixed-purchase-begin">定期購入開始日（終了日）は半角の正しい日付形式で入力して下さい。</p>
														<p class="error-message-fixed-purchase-begin">定期購入開始日（開始日）は定期購入開始日（終了日）より前に指定して下さい。</p>
													</div>
												</div>
												<div class="form-element-group-row">
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															次回配送日
														</div>
														<div class="form-element-group-content">
															<input name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_NEXT_SHIPPING_FROM %>" type="text" id="dpFixedPurchaseNextShippingDateFrom" class="input-datepicker needInput" onchange="validateDate('fixedPurchaseNextShipping')" />
															～
															<input name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_NEXT_SHIPPING_TO %>" type="text" id="dpFixedPurchaseNextShippingDateTo" class="input-datepicker needInput" onchange="validateDate('fixedPurchaseNextShipping')" />
															の間
															<div class="btn-group">
																<div class="btn-group-body">
																	<a href="Javascript:setYesterday('fixedPurchaseNextShipping');" class="btn-group-btn">昨日</a>
																	<a href="Javascript:setToday('fixedPurchaseNextShipping');" class="btn-group-btn">今日</a>
																	<a href="Javascript:setThisMonth('fixedPurchaseNextShipping');" class="btn-group-btn">今月</a>
																	<a href="Javascript:clearDatePeriod('fixedPurchaseNextShipping');" class="btn-group-btn">クリア</a>
																</div>
															</div>
														</div>
														<p class="error-message-fixed-purchase-next-shipping">次回配送日（開始日）は半角の正しい日付形式で入力して下さい。</p>
														<p class="error-message-fixed-purchase-next-shipping">次回配送日（終了日）は半角の正しい日付形式で入力して下さい。</p>
														<p class="error-message-fixed-purchase-next-shipping">次回配送日（開始日）は次回配送日（終了日）より前に指定して下さい。</p>
													</div>
												</div>
												<div class="form-element-group-row">
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															次々回配送日
														</div>
														<div class="form-element-group-content">
															<input name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_NEXT_NEXT_SHIPPING_FROM %>" type="text" id="dpFixedPurchaseNextNextShippingDateFrom" class="input-datepicker needInput" onchange="validateDate('fixedPurchaseNextNextShipping')" />
															～
															<input name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_NEXT_NEXT_SHIPPING_TO %>" type="text" id="dpFixedPurchaseNextNextShippingDateTo" class="input-datepicker needInput" onchange="validateDate('fixedPurchaseNextNextShipping')" />
															の間
															<div class="btn-group">
																<div class="btn-group-body">
																	<a href="Javascript:setYesterday('fixedPurchaseNextNextShipping');" class="btn-group-btn">昨日</a>
																	<a href="Javascript:setToday('fixedPurchaseNextNextShipping');" class="btn-group-btn">今日</a>
																	<a href="Javascript:setThisMonth('fixedPurchaseNextNextShipping');" class="btn-group-btn">今月</a>
																	<a href="Javascript:clearDatePeriod('fixedPurchaseNextNextShipping');" class="btn-group-btn">クリア</a>
																</div>
															</div>
														</div>
														<p class="error-message-fixed-purchase-next-next-shipping">次々回配送日（開始日）は半角の正しい日付形式で入力して下さい。</p>
														<p class="error-message-fixed-purchase-next-next-shipping">次々回配送日（終了日）は半角の正しい日付形式で入力して下さい。</p>
														<p class="error-message-fixed-purchase-next-next-shipping">次々回配送日（開始日）は次々回配送日（終了日）より前に指定して下さい。</p>
													</div>
												</div>
												<% if (Constants.ORDER_EXTEND_OPTION_ENABLED)
													{ %>
												<div class="form-element-group-row">
													<div class="form-element-group form-element-group-vertical">
														<div class="form-element-group-title">
															<label>注文拡張項目</label>
														</div>
														<div class="form-element-group-content">
															<select name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_NAME %>" id="ddlFixedPurchaseOrderExtendName" class="needInput"></select>
															<select name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_FLG %>" id="ddlFixedPurchaseOrderExtendFlg" class="needInput"></select>
															<input name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_TEXT %>" type="text" id="tbFixedPurchaseOrderExtendText" class="needInput" />
															<select name="<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_TEXT %>" id="ddlFixedPurchaseOrderExtendText" class="needInput"></select>
														</div>
													</div>
												</div>
												<% } %>
											</div>
										</div>
									</div>
									<a class="btn-toggle" data-toggle-content-selector=".order-workflow-list-block-data-search-other" data-toggle-trigger-label="すべてを表示する,閉じる"><span class="toggle-trigger-label">すべてを表示する</span></a>
									<div class="order-workflow-list-block-data-search-btn">
										<a href="javascript:void(0);" onclick="clearSearch(true)">クリア</a>
										&nbsp;&nbsp;|&nbsp;&nbsp;
										<a href="javascript:void(0);" onclick="clearSearch(false)">リセット</a>
										&nbsp;&nbsp;
										<input type="button" value="絞り込み検索" onclick="search()" class="btn btn-main btn-size-m" />
									</div>
								</div>
							</div>
							<br />
							<div class="order-workflow-list-block-data-pager" id="divTopPager">
								<!--▽ ページング ▽-->
								<table class="list_pager" cellspacing="0" cellpadding="0" border="0">
									<tbody>
										<tr>
											<td style="height: 22px">
												<span id="ctl00_ContentPlaceHolderBody_lbPager1">
													<table border="0" cellspacing="0" cellpadding="0">
														<tbody>
															<tr>
																<td style=""></td>
																<td style="" id="tdTotalCase" nowrap="">該当件数&nbsp;0件</td>
																<td class="list_pager-slide"></td>
																<td style="" id="tdPageList"></td>
															</tr>
														</tbody>
													</table>
												</span>
											</td>
										</tr>
									</tbody>
								</table>
								<!-- ページング-->
							</div>
						</div>
						<div id="divDataBody"></div>
					</div>
					<div class="order-workflow-list-block-data-message"></div>
					<div class="order-workflow-list-block-list order-workflow-list-block-validate-message" id="errorMessage"></div>
					<div class="order-workflow-list-block-data-bottom" id="divDataBottom">
						<div class="order-workflow-list-block-data-bottom-row-select">
							<div class="order-workflow-list-block-data-bottom-title" id="bottomTitle">選択実行</div>
							<dl class="order-workflow-list-block-data-update">
								<dt class="order-workflow-list-block-data-update-label extend-status-update">ステータス更新日</dt>
								<dd class="order-workflow-list-block-data-update-date extend-status-update">
									<input type="text" name="" class="input-datepicker" id="dpSelectExtendStatusUpdate" value="<%= DateTime.Now.ToString("yyyy/MM/dd") %>" />
								</dd>
								&nbsp;
								<dt class="order-workflow-list-block-data-update-label shipping-date-update">配送希望日指定</dt>
								<dd class="order-workflow-list-block-data-update-date shipping-date-update">
									<input type="text" name="" class="input-datepicker" id="dpSelectShippingDateUpdate" value="<%= DateTime.Now.ToString("yyyy/MM/dd") %>" />
								</dd>
								&nbsp;
								<dt class="order-workflow-list-block-data-update-label scheduled-shipping-date-update">出荷予定日指定</dt>
								<dd class="order-workflow-list-block-data-update-date scheduled-shipping-date-update">
									<input type="text" name="" class="input-datepicker" id="dpSelectScheduledShippingDateUpdate" value="<%= DateTime.Now.ToString("yyyy/MM/dd") %>" />
								</dd>
								&nbsp;
								<dt class="order-workflow-list-block-data-update-label next-shipping-date-update">次回配送日指定</dt>
								<dd class="order-workflow-list-block-data-update-date next-shipping-date-update">
									<input type="text" name="" class="input-datepicker" id="dpSelectNextShippingDateUpdate" value="<%= DateTime.Now.ToString("yyyy/MM/dd") %>" />
								</dd>
								&nbsp;
								<dt class="order-workflow-list-block-data-update-label next-next-shipping-date-update">次々回配送日指定</dt>
								<dd class="order-workflow-list-block-data-update-date next-next-shipping-date-update">
									<input type="text" name="" class="input-datepicker" id="dpSelectNextNextShippingDateUpdate" value="<%= DateTime.Now.ToString("yyyy/MM/dd") %>" />
								</dd>
							</dl>
							<div class="order-workflow-list-block-data-submit">
								<div class="order-workflow-list-block-data-submit-num">
									<span class="order-workflow-list-block-data-submit-num-label">対象件数</span>
									<em class="order-workflow-list-block-data-submit-num-value order-workflow-list-block-data-submit-num-value-select">0</em>
									<span class="order-workflow-list-block-data-submit-num-unit">件</span>
								</div>
								<div class="order-workflow-list-block-data-submit-btn">
									<input type="button" value="実行" class="btn btn-main btn-size-l order-workflow-list-block-data-submit-btn-select" id="btnSubmit" disabled>
								</div>
							</div>
							<div class="order-workflow-list-block-output" id="cassetteExport"></div>
						</div>
						<div class="order-workflow-list-block-data-bottom-row-all">
							<div class="order-workflow-list-block-data-bottom-title">全件実行</div>
							<dl class="order-workflow-list-block-data-update">
								<dt class="order-workflow-list-block-data-update-label extend-status-update">ステータス更新日</dt>
								<dd class="order-workflow-list-block-data-update-date extend-status-update">
									<input type="text" name="" class="input-datepicker" id="dpAllExtendStatusUpdate" value="<%= DateTime.Now.ToString("yyyy/MM/dd") %>" />
								</dd>
								&nbsp;
								<dt class="order-workflow-list-block-data-update-label shipping-date-update">配送希望日指定</dt>
								<dd class="order-workflow-list-block-data-update-date shipping-date-update">
									<input type="text" name="" class="input-datepicker" id="dpAllShippingDateUpdate" value="<%= DateTime.Now.ToString("yyyy/MM/dd") %>" />
								</dd>
								&nbsp;
								<dt class="order-workflow-list-block-data-update-label scheduled-shipping-date-update">出荷予定日指定</dt>
								<dd class="order-workflow-list-block-data-update-date scheduled-shipping-date-update">
									<input type="text" name="" class="input-datepicker" id="dpAllScheduledShippingDateUpdate" value="<%= DateTime.Now.ToString("yyyy/MM/dd") %>" />
								</dd>
								&nbsp;
								<dt class="order-workflow-list-block-data-update-label next-shipping-date-update">次回配送日指定</dt>
								<dd class="order-workflow-list-block-data-update-date next-shipping-date-update">
									<input type="text" name="" class="input-datepicker" id="dpAllNextShippingDateUpdate" value="<%= DateTime.Now.ToString("yyyy/MM/dd") %>" />
								</dd>
								&nbsp;
								<dt class="order-workflow-list-block-data-update-label next-next-shipping-date-update">次々回配送日指定</dt>
								<dd class="order-workflow-list-block-data-update-date next-next-shipping-date-update">
									<input type="text" name="" class="input-datepicker" id="dpAllNextNextShippingDateUpdate" value="<%= DateTime.Now.ToString("yyyy/MM/dd") %>" />
								</dd>
							</dl>
							<div class="order-workflow-list-block-data-submit">
								<div class="order-workflow-list-block-data-submit-num">
									<span class="order-workflow-list-block-data-submit-num-label">対象件数</span>
									<em class="order-workflow-list-block-data-submit-num-value order-workflow-list-block-data-submit-num-value-all">0</em>
									<span class="order-workflow-list-block-data-submit-num-unit">件</span>
								</div>
								<div class="order-workflow-list-block-data-submit-btn">
									<input type="button" value="全件実行" class="btn btn-main btn-size-l order-workflow-list-block-data-submit-btn-all" />
								</div>
							</div>
							<div class="order-workflow-list-block-output" id="lineExport"></div>
						</div>
					</div>
					<div class="order-workflow-list-block-content-inner" id="divUpload">
						<h3 class="order-workflow-list-block-content-title">関連ファイル取り込み</h3>
						<div class="order-workflow-list-block-upload-wrapper">
							<div class="order-workflow-list-block-upload">
								<label id="lbOrderFileInfo" class="dd-file-select-filename"></label>
								<dl class="order-workflow-list-block-upload-inner">
									<dt class="order-workflow-list-block-upload-label">ファイル種別</dt>
									<dd class="order-workflow-list-block-upload-select">
										<select id="ddlOrderFile" class="order-workflow-list-block-upload-select-select" onchange="displayImportSettingInfo(this)"></select>
										<span id="spShipmentEntry">
											<input type="checkbox" id="cbExecExternalShipmentEntry" class="mycheckbox" />
											<label>出荷情報登録連携</label>
										</span>
									</dd>
								</dl>
								<dl class="order-workflow-list-block-upload-inner">
									<dt class="order-workflow-list-block-upload-label">ファイルパス</dt>
									<dd class="order-workflow-list-block-upload-select">
										<div class="dd-file-select js-dd-file-select">
											<div class="dd-file-select-file">
												<p id="lbFilePath" class="dd-file-select-filename">選択されていません</p>
												<a href="javascript:void(0);" id="btnClearInputFile" class="dd-file-select-file-cancel" onclick="clearInputFile()" data-popover-message="削除する"><span class="icon-close"></span></a>
											</div>
											<label class="dd-file-select-label">
												<input id="fFile" type="file" name="fFile" class="dd-file-select-input" onchange="checkValidInput()" />
												<span class="dd-file-select-icon icon-clip"></span>
												<p class="dd-file-select-message">ファイルをドラッグ＆ドロップしてファイルを指定してください。</p>
												<div class="dd-file-select-btns">
													<p class="dd-file-select-btns-text">または</p>
													<a href="javascript:void(0);" class="dd-file-select-btn btn btn-main btn-size-s">ファイルを選択</a>
												</div>
											</label>
										</div>
									</dd>
								</dl>
							</div>
							<div class="order-workflow-list-block-upload-btn-wrapper">
								<input type="submit" id="btnImport" onclick="importOrderFile()" value="ファイル取込" disabled="disabled" class="order-workflow-list-block-upload-btn btn btn-main btn-size-l js-execute-upload" />
							</div>
						</div>
					</div>
				</div>
				<div class="order-workflow-list-block-content-loading">
					<span class="loading-animation-circle"></span>
				</div>
				<div class="modal-content-hide">
					<div id="modal-order-workflow-submit-confirm-select" class="modal-order-workflow-submit-confirm">
						<p class="modal-inner-ttl">
							実行内容をご確認の上、「実行」ボタンを押してください。
						</p>
						<div class="modal-order-workflow-submit-confirm-content">
							<div class="modal-order-workflow-submit-confirm-content-row">
								<div class="modal-order-workflow-submit-confirm-content-count">
									<h3 class="modal-order-workflow-submit-confirm-content-count-label">対象件数</h3>
									<div class="modal-order-workflow-submit-confirm-content-count-value">
										<span class="modal-order-workflow-submit-confirm-content-count-value-num">0</span>
										<span class="modal-order-workflow-submit-confirm-content-count-value-unit">件</span>
									</div>
								</div>
								<div class="modal-order-workflow-submit-confirm-content-date">
									<h3 class="modal-order-workflow-submit-confirm-content-date-label">ステータス更新日</h3>
									<div class="modal-order-workflow-submit-confirm-content-date-value">
										2020/09/04
									</div>
								</div>
							</div>
							<div class="modal-order-workflow-submit-confirm-content-action">
								<h3 class="modal-order-workflow-submit-confirm-content-action-label">アクション</h3>
								<div class="modal-order-workflow-submit-confirm-content-action-list"></div>
							</div>
						</div>
						<div class="modal-footer-action">
							<input type="button" class="btn btn-sub btn-size-l" onclick="modal.close();" value="キャンセル" />
							<input type="button" class="btn btn-main btn-size-l js-execute-all" onclick="execWorkflow('select')" value="実行" />
						</div>
					</div>
				</div>
				<div class="modal-content-hide">
					<div id="modal-order-workflow-submit-confirm-all" class="modal-order-workflow-submit-confirm">
						<p class="modal-inner-ttl">実行内容をご確認の上、「実行」ボタンを押してください。</p>
						<div class="modal-order-workflow-submit-confirm-content">
							<div class="modal-order-workflow-submit-confirm-content-row">
								<div class="modal-order-workflow-submit-confirm-content-count">
									<h3 class="modal-order-workflow-submit-confirm-content-count-label">対象件数</h3>
									<div class="modal-order-workflow-submit-confirm-content-count-value">
										<span class="modal-order-workflow-submit-confirm-content-count-value-num"></span>
										<span class="modal-order-workflow-submit-confirm-content-count-value-unit">件</span>
									</div>
								</div>
								<div class="modal-order-workflow-submit-confirm-content-date">
									<h3 class="modal-order-workflow-submit-confirm-content-date-label">ステータス更新日</h3>
									<div class="modal-order-workflow-submit-confirm-content-date-value">
										2020/09/04
									</div>
								</div>
							</div>
							<div class="modal-order-workflow-submit-confirm-content-action">
								<h3 class="modal-order-workflow-submit-confirm-content-action-label">アクション</h3>
								<div class="modal-order-workflow-submit-confirm-content-action-list"></div>
							</div>
						</div>
						<div class="modal-footer-action">
							<input type="button" class="btn btn-sub btn-size-l" onclick="modal.close();" value="キャンセル" />
							<input type="button" class="btn btn-main btn-size-l js-execute-all" value="実行" onclick="execWorkflow('all')" />
						</div>
					</div>
				</div>
			</div>
		</div>
		<div class="order-workflow-list-block order-workflow-list-block3">
			<div class="order-workflow-list-block-num">
				<div class="order-workflow-list-block-num-inner">
					<span class="order-workflow-list-block-num-number">3</span>
					<span class="order-workflow-list-block-num-text">結果</span>
				</div>
			</div>
			<div class="order-workflow-list-block-content">
				<div class="order-workflow-list-block-header">
					<h2 class="order-workflow-list-block-title">実行結果</h2>
				</div>
				<div class="order-workflow-list-block-content-inner">
					<p class="order-workflow-list-block-content-text">
						すべての処理が完了いたしました。
					</p>
					<p id="mailSendMessage" style="display: none;" class="order-workflow-list-block-content-text">
						結果はメールにてお知らせします。
					</p>
					<div class="order-workflow-list-block-upload-result" id="divUploadResult">
						<dl class="order-workflow-list-block-upload-inner">
							<dt class="order-workflow-list-block-upload-label">ファイル種別</dt>
							<dd class="order-workflow-list-block-upload-select"><span class="order-workflow-list-block-upload-filetype" id="spFileType"></span></dd>
						</dl>
						<dl class="order-workflow-list-block-upload-inner">
							<dt class="order-workflow-list-block-upload-label">ファイルパス</dt>
							<dd class="order-workflow-list-block-upload-select"><span class="order-workflow-list-block-upload-filepath" id="spFilePath"></span></dd>
						</dl>
						<dl class="order-workflow-list-block-upload-inner">
							<dt class="order-workflow-list-block-upload-label">&nbsp;</dt>
							<dd class="order-workflow-list-block-upload-select"><span id="spImportMessage"></span></dd>
						</dl>
						<dl class="order-workflow-list-block-upload-inner">
							<dt class="order-workflow-list-block-upload-label">&nbsp;</dt>
							<dd class="order-workflow-list-block-upload-select"><span id="errorList"></span></dd>
						</dl>
					</div>
					<div class="order-workflow-list-block-result-wrapper">
						<div class="order-workflow-list-block-result-col">
							<div class="order-workflow-list-block-result" id="divResultNum">
								<div class="order-workflow-list-block-result-total-num">
									<span class="order-workflow-list-block-result-total-num-label">処理件数</span>
									<span class="order-workflow-list-block-result-total-num-value" id="spTotalCase">0</span>
									<span class="order-workflow-list-block-result-total-num-unit">件</span>
								</div>
								<ul class="order-workflow-list-block-result-num">
									<li class="order-workflow-list-block-result-num-item is-ok">
										<span class="icon icon-circle"></span>
										<span class="order-workflow-list-block-result-num-item-label">正常終了</span>
										<span class="order-workflow-list-block-result-num-item-value" id="spSuccessCase">0</span>
										<span class="order-workflow-list-block-result-num-item-unit">件</span>
									</li>
									<li class="order-workflow-list-block-result-num-item is-ng">
										<span class="icon icon-close"></span>
										<span class="order-workflow-list-block-result-num-item-label">問題あり</span>
										<span class="order-workflow-list-block-result-num-item-value" id="spErrorCase">0</span>
										<span class="order-workflow-list-block-result-num-item-unit">件</span>
									</li>
									<li class="order-workflow-list-block-result-num-item is-remaining">
										<span class="order-workflow-list-block-result-num-item-label">残件数</span>
										<span class="order-workflow-list-block-result-num-item-value" id="spRemainCase">0</span>
										<span class="order-workflow-list-block-result-num-item-unit">件</span>
									</li>
								</ul>
							</div>
							<div class="order-workflow-list-block-continue">
								<button type="button" class="order-workflow-list-block-continue-btn btn btn-main btn-size-m" onclick="processContinue()">続けて処理をする  </button>
							</div>
						</div>
						<div class="order-workflow-list-block-result-col">
							<div class="order-workflow-list-block-next-workflow">
								<span></span>
								<input type="button" class="order-workflow-list-block-next-workflow-btn btn btn-main btn-size-l" value="  次のワークフローへ  " onclick="getNextWorkflow()" />
							</div>
						</div>
					</div>
					<div class="order-workflow-list-block-result-table-wrapper table-wrapper scroll-table-wrapper" id="divResultTable">
						<table class="order-workflow-list-block-result-table table" _fixedhead="cols:1;">
							<thead>
								<tr id="trResultHeader">
								</tr>
							</thead>
							<tbody id="ResultBody">
							</tbody>
						</table>
					</div>
					<div class="order-workflow-list-block-result-wrapper bottom" id="divBottom">
						<div class="order-workflow-list-block-result-col">
						</div>
						<div class="order-workflow-list-block-result-col">
							<div class="order-workflow-list-block-next-workflow">
								<span></span>
								<input type="button" onclick="getNextWorkflow()" class="order-workflow-list-block-next-workflow-btn btn btn-main btn-size-l" value="  次のワークフローへ  " />
							</div>
						</div>
					</div>
				</div>
				<div class="order-workflow-list-block-content-loading">
					<span id="spLoading" class="loading-animation-circle"></span>
					<div id="processInfo"></div>
				</div>
			</div>
		</div>
	</div>
	<script type="text/javascript">
		var selectedTargetDispText = '対象<span>@ 1 @</span>件を選択しています。';

		var orderWorkflowListXhr;
		var workflowList;
		var currentWorkflowIndex = 0;
		var currentPage;
		var urlQueryString = {};
		var orderFiles = '';
		var fixedMidashiXhr;
		var workflowDispSetting;
		var orderExtends = {};

		$(document).ready(function () {
				browserBack();
				setUrlQueryString();
				workflow.step.switch(1);
				getExternalPaymentStatuses();
				getInvoiceBundleFlags();
				getUpdateDateExtendStatuses();
				getUpdateDateOrderStatuses();
				getAnotherShippingFlags();
				getShippingStatuses();
				getExtendStatusNos();
				getExtendStatuses();
				getMemoFlags();
				getShippingPrefectures();
				getOrderExtends();
				getShippingStatusCodes();
				getShippingCurrentStatuses();
				getTaiwanInvoiceStatuses();
				getFixedPurchaseStatuses();
				getFixedPurchasePaymentStatuses();
				setDivCss();
				getStorePickupStatusList();
			$.when(
				getWorkflowDispSetting()
			).done(function() {
				getWorkflowKbnList();
			});
		});

		// Cancel ajax request on click of redirect page
		(function ($) {
			var xhrPool = [];
			$(document).ajaxSend(function (e, jqXHR, options) {
				xhrPool.push(jqXHR);
			});
			$(document).ajaxComplete(function (e, jqXHR, options) {
				xhrPool = $.grep(xhrPool, function (x) { return x != jqXHR });
			});
			var abort = function () {
				$.each(xhrPool, function (idx, jqXHR) {
					jqXHR.abort();
				});
			};

			window.onbeforeunload = function () {
				displayWorkflowActionAndCondition.clear();
				abort();
			}
		})(jQuery);

		// ブラウザバック時の読み込み
		function browserBack() {
			window.addEventListener('popstate',function() {
				displayWorkflowActionAndCondition.clear();
				location.reload();
			});
		}

		// Set url query string
		function setUrlQueryString() {
			var query = window.location.search.substring(1);
			if (query === "") return;

			var params = query.split("&");

			$.each(params, function(index, value) {
				var keyValuePair = value.split('=');
				urlQueryString[keyValuePair[0]] = decodeURIComponent(keyValuePair[1]);
			});
		}

		// Update url
		function updateUrl() {
			// Get new search url
			var url = getSearchUrl();

			// Change current url to new url
			history.pushState({}, null, url);
		}

		// Get search url
		function getSearchUrl() {
			var keys = Object.keys(urlQueryString);
			var url = '<%: this.OrderWorkflowBaseUrl %>?';
		
			// Construct new url
			url += keys[0] + '=' + encodeURIComponent(urlQueryString[keys[0]]);

			for (var index = 1; index < keys.length; index++) {
				var key = keys[index];
				url += '&' + key + '=' + encodeURIComponent(urlQueryString[key]);
			}

			return url;
		}

		// Get workflow kbn list
		function getWorkflowKbnList() {
			var workflowKbn = getWorkflowKbnUrlParam();

			// Send request to server to retrieve order workflow kbns
			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/GetWorkflowKbnList";
			var dataRequest = JSON.stringify({ workflowKbn: workflowKbn });
			var request = callAjax(urlRequest, dataRequest);
			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				if (data.length == 0) return;

				// Get current workflow kbn selected index
				var selectedIndex = 0;
				for (var index = 0; index < data.length; index++) {
					if (workflowKbn === data[index].key) {
						selectedIndex = index;
					}
				}

				// Generate HTML
				for (var index = 0; index < data.length; index++) {
					var className = 'tab';
					if (selectedIndex === index) className += ' is-active';

					$('#divWorkflowKbnList').append(
						'<div class="tabs-tab">'
						+ '<a href="javascript:void(0)" onclick="getOrderWorkflowListByWorkflowKbn(\'' + data[index].key + '\');" class="'+ className +'">'
						+ data[index].value
						+ '</a></div>');
				}

				tab.ini();
				getOrderWorkflowList();
			});
		}

		// Get external payment statuses
		function getExternalPaymentStatuses() {

			// Send request to server to retrieve external payment statuses
			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/GetExternalPaymentStatuses";
			var dataRequest = '';
			var request = callAjax(urlRequest, dataRequest);
			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				if (data.length == 0) return;

				// Set data to dropdownlist
				for (var index = 0; index < data.length; index++) {
					var item = data[index];
					addOptionToSelect('#ddlExternalPaymentStatus', item.key, item.value);
				}

				$('#ddlExternalPaymentStatus').val(urlQueryString['<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_STATUS %>']);
			});
		}

		// Get taiwan invoice statuses
		function getTaiwanInvoiceStatuses() {

			// Send request to server to retrieve taiwan invoice statuses
			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/GetTaiwanInvoiceStatuses";
			var dataRequest = '';
			var request = callAjax(urlRequest, dataRequest);
			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				if (data.length == 0) return;

				// Set data to dropdownlist
				for (var index = 0; index < data.length; index++) {
					var item = data[index];
					addOptionToSelect('#ddlInvoiceStatus', item.key, item.value);
				}

				$('#ddlInvoiceStatus').val(urlQueryString['<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_TW_INVOICE_STATUS %>']);
			});
		}

		// Get fixed purchase statuses
		function getFixedPurchaseStatuses() {

			// Send request to server to retrieve fixed purchase statuses
			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/GetFixedPurchaseStatuses";
			var dataRequest = '';
			var request = callAjax(urlRequest, dataRequest);
			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				if (data.length == 0) return;

				// Set data to dropdownlist
				for (var index = 0; index < data.length; index++) {
					var item = data[index];
					addOptionToSelect('#ddlFixedPurchaseStatus', item.key, item.value);
				}

				$('#ddlFixedPurchaseStatus').val(urlQueryString['<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_FIXEDPURCHASE_STAUS %>']);
			});
		}

		// Get fixed purchase payment statuses
		function getFixedPurchasePaymentStatuses() {

			// Send request to server to retrieve fixed purchase payment statuses
			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/GetFixedPurchasePaymentStatuses";
			var dataRequest = '';
			var request = callAjax(urlRequest, dataRequest);
			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				if (data.length == 0) return;

				// Set data to dropdownlist
				for (var index = 0; index < data.length; index++) {
					var item = data[index];
					addOptionToSelect('#ddlFixedPurchasePaymentStatus', item.key, item.value);
				}

				$('#ddlFixedPurchasePaymentStatus').val(urlQueryString['<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_FIXEDPURCHASE_PAYMENT_STAUS %>']);
			});
		}

		// Get invoice bundle flags
		function getInvoiceBundleFlags() {

			// Send request to server to retrieve invoice bundle flags
			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/GetInvoiceBundleFlags";
			var dataRequest = '';
			var request = callAjax(urlRequest, dataRequest);
			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				if (data.length == 0) return;

				// Set data to dropdownlist
				for (var index = 0; index < data.length; index++) {
					var item = data[index];
					addOptionToSelect('#ddlInvoiceBundleFlg', item.key, item.value);
				}

				$('#ddlInvoiceBundleFlg').val(urlQueryString['<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_INVOICE_BUNDLE_FLG %>']);
			});
		}

		// Get update date extend statuses
		function getUpdateDateExtendStatuses() {

			// Send request to server to retrieve update date extend statuses
			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/GetUpdateDateExtendStatuses";
			var dataRequest = '';
			var request = callAjax(urlRequest, dataRequest);
			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				if (data.length == 0) return;

				// Set data to dropdownlist
				for (var index = 0; index < data.length; index++) {
					var item = data[index];
					addOptionToSelect('#ddlExtendStatusUpdateDate', item.key, item.value);
					addOptionToSelect('#ddlFixedPurchaseExtendStatusUpdateDate', item.key, item.value);
				}

				$('#ddlExtendStatusUpdateDate').val(urlQueryString['<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_UPDATE_DATE_EXTEND_STATUS_NO %>']);
				$('#ddlFixedPurchaseExtendStatusUpdateDate').val(urlQueryString['<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_EXTEND_STATUS_NO_UPDATE_DATE %>']);
			});
		}

		// Get update date order statuses
		function getUpdateDateOrderStatuses() {

			// Send request to server to retrieve update date order statuses
			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/GetUpdateDateOrderStatuses";
			var dataRequest = '';
			var request = callAjax(urlRequest, dataRequest);
			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				if (data.length == 0) return;

				// Set data to dropdownlist
				for (var index = 0; index < data.length; index++) {
					var item = data[index];
					addOptionToSelect('#ddlOrderUpdateDate', item.key, item.value);
				}

				$('#ddlOrderUpdateDate').val(urlQueryString['<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_STATUS %>']);
			});
		}

		// Get another shipping flags
		function getAnotherShippingFlags() {

			// Send request to server to retrieve another shippings flags
			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/GetAnotherShippingFlags";
			var dataRequest = '';
			var request = callAjax(urlRequest, dataRequest);
			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				if (data.length == 0) return;

				// Set data to dropdownlist
				for (var index = 0; index < data.length; index++) {
					var item = data[index];
					addOptionToSelect('#ddlAnotherShippingFlg', item.key, item.value);
				}

				$('#ddlAnotherShippingFlg').val(urlQueryString['<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ANOTHER_SHIPPING_FLG %>']);
			});
		}

		// Get another shipping statuses
		function getShippingStatuses() {

			// Send request to server to retrieve shippings statuses
			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/GetShippingStatuses";
			var dataRequest = '';
			var request = callAjax(urlRequest, dataRequest);
			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				if (data.length == 0) return;

				// Set data to dropdownlist
				for (var index = 0; index < data.length; index++) {
					var item = data[index];
					addOptionToSelect('#ddlShippingStatus', item.key, item.value);
				}

				$('#ddlShippingStatus').val(urlQueryString['<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_STATUS %>']);
			});
		}

		// Get extend status nos
		function getExtendStatusNos() {

			// Send request to server to retrieve extend status nos
			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/GetExtendStatusNos";
			var dataRequest = '';
			var request = callAjax(urlRequest, dataRequest);
			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				if (data.length == 0) return;

				// Set data to dropdownlist
				for (var index = 0; index < data.length; index++) {
					var item = data[index];
					addOptionToSelect('#ddlExtendStatusNo', item.key, item.value);
					addOptionToSelect('#ddlFixedPurchaseExtendStatusNo', item.key, item.value);
				}

				$('#ddlExtendStatusNo').val(urlQueryString['<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_EXTEND_STATUS_NO %>']);
				$('#ddlFixedPurchaseExtendStatusNo').val(urlQueryString['<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_EXTEND_STATUS_NO %>']);
			});
		}

		// Get extend statuses
		function getExtendStatuses() {

			// Send request to server to retrieve extend statuses
			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/GetExtendStatuses";
			var dataRequest = '';
			var request = callAjax(urlRequest, dataRequest);
			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				if (data.length == 0) return;

				// Set data to dropdownlist
				for (var index = 0; index < data.length; index++) {
					var item = data[index];
					addOptionToSelect('#ddlExtendStatus', item.key, item.value);
					addOptionToSelect('#ddlFixedPurchaseExtendStatus', item.key, item.value);
				}

				$('#ddlExtendStatus').val(urlQueryString['<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_EXTEND_STATUS %>']);
				$('#ddlFixedPurchaseExtendStatus').val(urlQueryString['<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_EXTEND_STATUS %>']);
			});
		}
	
		// Get memo flags
		function getMemoFlags() {

			// Send request to server to retrieve memo flags
			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/GetMemoFlags";
			var dataRequest = '';
			var request = callAjax(urlRequest, dataRequest);
			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				if (data.length == 0) return;

				// Set data to dropdownlist
				for (var index = 0; index < data.length; index++) {
					var item = data[index];
					addOptionToSelect('#ddlOrderMemoFlg', item.key, item.value);
					addOptionToSelect('#ddlOrderManagementMemoFlg', item.key, item.value);
					addOptionToSelect('#ddlShippingMemoFlg', item.key, item.value);
					addOptionToSelect('#ddlOrderPaymentMemoFlg', item.key, item.value);
					addOptionToSelect('#ddlOrderRelationMemoFlg', item.key, item.value);
					addOptionToSelect('#ddlUserMemoFlg', item.key, item.value);
					addOptionToSelect('#ddlProductOptionFlg', item.key, item.value);
					addOptionToSelect('#ddlOrderReceiptFlg', item.key, item.value);
					addOptionToSelect('#ddlFixedPurchaseManagementMemoFlg', item.key, item.value);
					addOptionToSelect('#ddlFixedPurchaseShippingMemoFlg', item.key, item.value);
					addOptionToSelect('#ddlFixedPurchaseReceiptFlg', item.key, item.value);
				}

				$('#ddlOrderMemoFlg').val(urlQueryString['<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_MEMO_FLG %>']);
				SetDisplayMemoTextBox($("#ddlOrderMemoFlg"), $("#tbOrderMemo"));
				$('#ddlOrderManagementMemoFlg').val(urlQueryString['<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_MANAGEMENT_MEMO_FLG %>']);
				SetDisplayMemoTextBox($("#ddlOrderManagementMemoFlg"), $("#tbManagementMemo"));
				$('#ddlShippingMemoFlg').val(urlQueryString['<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_MEMO_FLG %>']);
				SetDisplayMemoTextBox($("#ddlShippingMemoFlg"), $("#tbShippingMemo"));
				$('#ddlOrderPaymentMemoFlg').val(urlQueryString['<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_PAYMENT_MEMO_FLG %>']);
				SetDisplayMemoTextBox($("#ddlOrderPaymentMemoFlg"), $("#tbPaymentMemo"));
				$('#ddlOrderRelationMemoFlg').val(urlQueryString['<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_RELATION_MEMO_FLG %>']);
				SetDisplayMemoTextBox($("#ddlOrderRelationMemoFlg"), $("#tbRelationMemo"));
				$('#ddlUserMemoFlg').val(urlQueryString['<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_USER_MEMO_FLG %>']);
				SetDisplayMemoTextBox($("#ddlUserMemoFlg"), $("#tbUserMemo"));
				$('#ddlProductOptionFlg').val(urlQueryString['<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_PRODUCT_OPTION_FLG %>']);
				SetDisplayMemoTextBox($("#ddlProductOptionFlg"), $("#tbProductOption"));
				$('#ddlOrderReceiptFlg').val(urlQueryString['<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_RECEIPT_FLG %>']);
				$('#ddlFixedPurchaseManagementMemoFlg').val(urlQueryString['<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_MANAGEMENT_MEMO_FLG %>']);
				SetDisplayMemoTextBox($("#ddlFixedPurchaseManagementMemoFlg"), $("#tbFixedPurchaseManagementMemo"));
				$('#ddlFixedPurchaseShippingMemoFlg').val(urlQueryString['<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_SHIPPING_MEMO_FLG %>']);
				SetDisplayMemoTextBox($("#ddlFixedPurchaseShippingMemoFlg"), $("#tbFixedPurchaseShippingMemo"));
				$('#ddlFixedPurchaseReceiptFlg').val(urlQueryString['<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_RECEIPT_FLG %>']);
			});
		}

		// Get shipping prefectures
		function getShippingPrefectures() {

			// Send request to server to retrieve shipping prefectures
			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/GetShippingPrefectures";
			var dataRequest = '';
			var request = callAjax(urlRequest, dataRequest);
			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				if (data.length == 0) return;
				var checkBoxListPrefectures = "";
				for (var index = 0; index < data.length; index++) {
					var item = data[index];
					var name = '<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_PREFECTURE %>'
					checkBoxListPrefectures += '<input type="checkbox" ID="cblShippingPrefectures_'+ index +'" name="'+ name +'" class="needInput" value="'+ item.value +'"><label for="cblShippingPrefectures_'+ index +'">'+ item.value +'</label>'
				}
				$('#dvCheckBoxListPrefectures').html(checkBoxListPrefectures);
				loadShippingPrefecturesScript();
			});
		}

		// Get order extend
		function getOrderExtends() {
			var orderExtendFlg = JSON.parse('<%= GetOrderExtendFlg() %>');
			for (var index = 0; index < orderExtendFlg.length; index++) {
				var item = orderExtendFlg[index];
				addOptionToSelect('#ddlOrderExtendFlg', item.key, item.value);
				addOptionToSelect('#ddlFixedPurchaseOrderExtendFlg', item.key, item.value);
			}

			orderExtends = JSON.parse('<%= GetOrderExtends() %>');
			for (var index = 0; index < orderExtends.length; index++) {
				var item = orderExtends[index];
				addOptionToSelect('#ddlOrderExtendName', item.setting_id, item.setting_name);
				addOptionToSelect('#ddlFixedPurchaseOrderExtendName', item.setting_id, item.setting_name);
			}

			$('#ddlOrderExtendFlg').hide();
			$('#ddlOrderExtendText').hide();
			$('#tbOrderExtendText').hide();

			$('#ddlFixedPurchaseOrderExtendFlg').hide();
			$('#ddlFixedPurchaseOrderExtendText').hide();
			$('#tbFixedPurchaseOrderExtendText').hide();

			$('#ddlOrderExtendName').change(function() {
				orderExtendNameChange(this.value);
			})

			$('#ddlFixedPurchaseOrderExtendName').change(function() {
				fixedPurchaseOrderExtendNameChange(this.value)
			})

			$('#ddlOrderExtendName').val(urlQueryString['<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_NAME %>']);
			orderExtendNameChange(urlQueryString['<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_NAME %>']);
			$('#ddlFixedPurchaseOrderExtendName').val(urlQueryString['<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_NAME %>']);
			fixedPurchaseOrderExtendNameChange(urlQueryString['<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_NAME %>'])
			$('#ddlOrderExtendFlg').val(urlQueryString['<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_FLG %>']);
			$('#ddlFixedPurchaseOrderExtendFlg').val(urlQueryString['<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_FLG %>']);
		}

		// Order extend name change
		function orderExtendNameChange(value) {
			if((value == '') || (value === "undefined") || (value === undefined)) {
				$('#ddlOrderExtendFlg').val('').hide();
				$('#ddlOrderExtendText').empty().hide();
				$('#tbOrderExtendText').val('').hide();
			} else {
				$('#ddlOrderExtendFlg').show();
				$('#ddlOrderExtendFlg').val('');
				$('#ddlOrderExtendText').empty();
				$('#tbOrderExtendText').val('');

				var orderExtend = getObject(orderExtends, "setting_id", value);
				urlQueryString['<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_TYPE %>'] = orderExtend.setting_type;
				updateUrl();
				if (orderExtend.setting_type == '<%= Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_TEXT %>') {
					$('#tbOrderExtendText').show();
					$('#ddlOrderExtendText').empty().hide();
				} else {
					$('#ddlOrderExtendText').show();
					$('#tbOrderExtendText').val('').hide();

					for (var index = 0; index < orderExtend.setting_default.length; index++) {
						var item = orderExtend.setting_default[index];
						addOptionToSelect('#ddlOrderExtendText', item.key, item.value);
					}

					$('#ddlOrderExtendText').val(urlQueryString['<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_TEXT %>']);
				}
			}
		}

		function fixedPurchaseOrderExtendNameChange(value) {
			if((value == '') || (value === "undefined") || (value === undefined)) {
				$('#ddlFixedPurchaseOrderExtendFlg').val('').hide();
				$('#ddlFixedPurchaseOrderExtendText').empty().hide();
				$('#tbFixedPurchaseOrderExtendText').val('').hide();
			} else {
				$('#ddlFixedPurchaseOrderExtendFlg').show();
				$('#ddlFixedPurchaseOrderExtendText').empty();
				$('#ddlFixedPurchaseOrderExtendFlg').val('');
				$('#tbFixedPurchaseOrderExtendText').val('');
				var orderExtend = getObject(orderExtends, "setting_id", value);
				urlQueryString['<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_TYPE %>'] = orderExtend.setting_type;
				updateUrl();
				if (orderExtend.setting_type == '<%= Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_TEXT %>') {
					$('#tbFixedPurchaseOrderExtendText').show();
					$('#ddlFixedPurchaseOrderExtendText').empty().hide();
				} else {
					$('#ddlFixedPurchaseOrderExtendText').show();
					$('#tbFixedPurchaseOrderExtendText').val('').hide();

					for (var index = 0; index < orderExtend.setting_default.length; index++) {
						var item = orderExtend.setting_default[index];
						addOptionToSelect('#ddlFixedPurchaseOrderExtendText', item.key, item.value);
					}

					$('#ddlFixedPurchaseOrderExtendText').val(urlQueryString['<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_TEXT %>']);
				}
			}
		}

		// 完了状態コードを取得
		function getShippingStatusCodes() {

			// 完了状態コードを取得するためにサーバーにリクエストを送信
			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/GetShippingStatusCodes";
			var dataRequest = '';
			var request = callAjax(urlRequest, dataRequest);
			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				if (data.length == 0) return;

				// ドロップダウンリストにセット
				for (var index = 0; index < data.length; index++) {
					var item = data[index];
					addOptionToSelect('#ddlShippingStatusCode', item.key, item.value);
				}
			});
		}

		// 現在の状態を取得
		function getShippingCurrentStatuses() {

			// 現在の状態を取得するためにサーバーにリクエストを送信
			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/GetShippingCurrentStatuses";
			var dataRequest = '';
			var request = callAjax(urlRequest, dataRequest);
			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				if (data.length == 0) return;

				// ドロップダウンリストにセット
				for (var index = 0; index < data.length; index++) {
					var item = data[index];
					addOptionToSelect('#ddlShippingCurrentStatus', item.key, item.value);
				}
			});
		}

		// Get workflow disp setting
		function getWorkflowDispSetting() {
			var defer = $.Deferred();

			// Send request to server to retrieve workflow disp setting
			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/GetWorkflowDispSetting";
			var dataRequest = '';
			var request = callAjax(urlRequest, dataRequest);
			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				if (data.length == 0) return;
				
				workflowDispSetting = data;
				defer.resolve();
			});

			return defer.promise();
		}

		// Add option to select
		function addOptionToSelect(selector, key, value) {
			$(selector).append(
				$('<option>', {
					value: key,
					text: value
				})
			);
		}

		// Get object in array
		function getObject(array, property, value) {
			for (var index = 0; index < array.length; index++) {
				if(array[index][property] == value)
					return array[index];
			}
		}

		// Get order workflow list
		function getOrderWorkflowList() {
			var workflowKbn = getWorkflowKbnUrlParam();

			getOrderWorkflowListByWorkflowKbn(workflowKbn);
		}

		// Get order workflow list by workflow kbn
		function getOrderWorkflowListByWorkflowKbn(workflowKbn) {

			// Show loading animation circle
			$("#ulOrderWorkflowList").empty();
			$("#ulOrderWorkflowList").append('<div class="order-workflow-list-block-content-loading" style="border: none;">');
			$("#ulOrderWorkflowList").append('<span class="loading-animation-circle"></span></div>');

			// Make sure the last ajax call is canceled before call new ajax request
			if ((orderWorkflowListXhr != null) && (orderWorkflowListXhr != undefined)) {
				orderWorkflowListXhr.abort();
			}

			// Clear all ajax request for display workflow action, condition and target order count
			displayWorkflowActionAndCondition.clear();

			urlQueryString['<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_KBN %>'] = workflowKbn;
			updateUrl();

			// Send request to server to retrieve order workflow items
			orderWorkflowListXhr = $.ajax({
				type: "POST",
				url: "<%: this.OrderWorkflowBaseUrl %>/GetOrderWorkflowList",
				data: JSON.stringify({
					workflowKbn: workflowKbn,
					workflowName: $('#tbWorkflowSearch').val()
				}),
				contentType: "application/json; charset=utf-8",
				success: function (response) {

					// Empty the HTML element (container) before add order workflow items
					$("#ulOrderWorkflowList").empty();

					if ((response == null)
						|| (response.d == undefined)) return;

					var data = JSON.parse(response.d);
					if (data.length == 0) return;

					workflowList = data;

					// Construct the order workflow items
					for (var index = 0; index < data.length; index++) {
						var functionName = "checkProcess('" + index + "')";
						var dataWorkflowKbnAttribute = ' data-workflow-kbn="kbn' + data[index].workflowKbn + '"';
						var dataWorkflowNoAttribute = ' data-workflow-no="no' + data[index].workflowNo + '"';
						var dataWorkflowTypeAttribute = ' data-workflow-type="' + data[index].workflowType + '"';
						var dataIsLoadTargetOrderCountAttribute = (data[index].workflowDetailKbn != '<%= Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN_ODR_IMP %>') ? ' data-is-load="true"' : '';
						var item = '<li id="line' + index + '" class="order-workflow-list-block-list-item" onclick="' + functionName + '"' + dataWorkflowKbnAttribute + dataWorkflowNoAttribute + dataWorkflowTypeAttribute + dataIsLoadTargetOrderCountAttribute + '>'
							+ '<div class="order-workflow-list-block-list-item-col1">'
							+ '    <div class="order-workflow-list-block-list-item-header">'
							+ '        <div class="order-workflow-list-block-list-item-title">'
							+ '            <strong>' + data[index].workflowName + '</strong>'
							+ '        </div>'
							+ '        <div class="order-workflow-list-block-list-item-detail">'
							+ '            <div class="order-workflow-list-block-list-item-detail-btn">'
							+ '                <a href="javascript:void(0);" data-popover-html=""><span class="order-workflow-list-block-list-item-detail-btn-icon icon-popup-detail"></span></a>'
							+ '                <div class="popover-html-content">'
							+ '                    <div class="order-workflow-list-block-list-item-detail-popup">'
							+ '                        <p class="order-workflow-list-block-list-item-detail-popup-text" style="word-break: break-all;">' + data[index].description + '</p>'
							+ '                        <div class="order-workflow-list-block-list-item-detail-popup-table">'
							+ '                            <div class="order-workflow-list-block-list-item-conditions">'
							+ '                                <h3 class="order-workflow-list-block-list-item-conditions-title">抽出条件</h3>'
							+ '                                <div class="order-workflow-list-block-list-item-conditions-list">'
							+ '                                   <div class="order-workflow-list-block-content-loading" style="border: none; padding: 50px 0;"><span class="loading-animation-circle"></span></div>'
							+ '                                </div>'
							+ '                            </div>'
							+ '                            <div class="order-workflow-list-block-list-item-actions">'
							+ '                                <h3 class="order-workflow-list-block-list-item-actions-title">アクション</h3>'
							+ '                                <div class="order-workflow-list-block-list-item-actions-list">'
							+ '                                   <div class="order-workflow-list-block-content-loading" style="border: none; padding: 50px 0;"><span class="loading-animation-circle"></span></div>'
							+ '                                </div>'
							+ '                            </div>'
							+ '                        </div>'
							+ '                    </div>'
							+ '                </div>'
							+ '            </div>'
							+ '        </div>'
							+ '    </div>'
							+ '    <div class="order-workflow-list-block-list-item-content">'
							+ '        <p class="order-workflow-list-block-list-item-text">' + data[index].description + '</p>'
							+ '        <div class="order-workflow-list-block-list-item-table">'
							+ '            <div class="order-workflow-list-block-list-item-conditions">'
							+ '                <h3 class="order-workflow-list-block-list-item-conditions-title">抽出条件</h3>'
							+ '                <div class="order-workflow-list-block-list-item-conditions-list">'
							+ '                    <div class="order-workflow-list-block-content-loading" style="border: none; padding: 50px 0;"><span class="loading-animation-circle"></span></div>'
							+ '                </div>'
							+ '            </div>'
							+ '            <div class="order-workflow-list-block-list-item-actions">'
							+ '                <h3 class="order-workflow-list-block-list-item-actions-title">アクション</h3>'
							+ '                <div class="order-workflow-list-block-list-item-actions-list">'
							+ '                    <div class="order-workflow-list-block-content-loading" style="border: none; padding: 50px 0;"><span class="loading-animation-circle"></span></div>'
							+ '                </div>'
							+ '            </div>'
							+ '        </div>'
							+ '    </div>'
							+ '</div>'
							+ '<div class="order-workflow-list-block-list-item-col2">'
							+ '    <div class="order-workflow-list-block-list-item-num">'
							+ '        <em class="order-workflow-list-block-list-item-num-value">' + ((data[index].workflowDetailKbn != '<%= Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN_ODR_IMP %>') ? '...' : '') + '</em>'
							+ '        <span class="order-workflow-list-block-list-item-num-label">' + ((data[index].workflowDetailKbn != '<%= Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN_ODR_IMP %>') ? '件' : '') + '</span>'
							+ '    </div>'
							+ '</div>'
							+ '<div class="order-workflow-list-block-list-item-btns">'
							+ '    <a href="javascript:void(0)" class="order-workflow-list-block-list-item-btn btn btn-main btn-size-m" >選択する</a>'
							+ '    <a href="javascript:void(0)" class="order-workflow-list-block-list-item-btn-selected btn btn-sub btn-size-m" onclick="backToWorkflowList()"><span class="order-workflow-list-block-list-item-btn-selected-label-1">選択中</span><span class="order-workflow-list-block-list-item-btn-selected-label-2">解除する</span></a>'
							+ '</div>'
							+ '</li>'

						$("#ulOrderWorkflowList").append(item);
					}

					workflow.select.ini();
					popover.ini();
					displayWorkflowActionAndCondition.ini();

					if (urlQueryString.owno) {
						var workflowIndex = findWorkflowIndex();
						$('#line' + workflowIndex).click();
					}
				},
				error: pageReload
			});
		}

		// Display workflow action and condition
		var displayWorkflowActionAndCondition = {
			wrapperSelector : '.order-workflow-list-block-list-item',
			wrapperActionSelector : '.order-workflow-list-block-list-item-actions-list',
			wrapperConditionSelector : '.order-workflow-list-block-list-item-conditions-list',
			loadingElementTemplate : '<div class="order-workflow-list-block-content-loading" style="border: none; padding: 50px 0;"><span class="loading-animation-circle"></span></div>',
			wrapperTargetOrderCountValueSelector : '.order-workflow-list-block-list-item-num-value',
			loadingElementOrderCountTemplate : '...',
			ajaxRequests: [],
			workflowTargetOrderCountRequests: [],
			sequenceCount: 1,
			requestCount: 0,
			next: function() {},
			ini : function () {
				var _this = this;
				_this.ajaxRequests = [];
				_this.workflowTargetOrderCountRequests = [];
				_this.requestCount = 0;
				$(_this.wrapperSelector).each(function (i) {
					var wrapper = $(this);
					var wrapperActions = wrapper.find(_this.wrapperActionSelector);
					var wrapperConditions = wrapper.find(_this.wrapperConditionSelector);
					var wrapperTargetOrderCountValue = wrapper.find(_this.wrapperTargetOrderCountValueSelector);
					var workflowKbn = wrapper.data('workflow-kbn').replace('kbn', '');
					var workflowNo = wrapper.data('workflow-no').replace('no', '');
					var workflowType = wrapper.data('workflow-type');
					var isLoadTargetCount = wrapper.data('is-load');

					// Add loading
					$(wrapperActions).empty();
					$(wrapperActions).append(_this.loadingElementTemplate);
					$(wrapperConditions).empty();
					$(wrapperConditions).append(_this.loadingElementTemplate);
					if (isLoadTargetCount) {
						$(wrapperTargetOrderCountValue).html(_this.loadingElementOrderCountTemplate);
					}
					popover.reloadContent(i);

					// Create requests
					var workflowTargetOrderCountRequest = {
						workflowKbn: workflowKbn,
						workflowNo: workflowNo,
						workflowType: workflowType
					};
					_this.workflowTargetOrderCountRequests.push(workflowTargetOrderCountRequest);
				});

				// Create iterator function to take request by the sequence count setting (default setting: 5)
				_this.next = iterator(_this.workflowTargetOrderCountRequests, _this.sequenceCount);

				// Take requests and call sequence loading
				var requests = _this.next();
				_this.requestCount += _this.sequenceCount;
				_this.loadSequence(requests, _this.next());
			},
			loadSequence: function (requests, nextRequests) {
				var _this = this;
				if (_this.requestCount > _this.workflowTargetOrderCountRequests.length) return;

				// Create formdata request
				var formData = new FormData();
				formData.append('<%: Constants.PARAM_ORDERWORKFLOW_ACTION_KBN %>', '<%: Constants.ORDERWORKFLOW_ACTION_KBN_ACTION_CONDITIONS %>');
				formData.append('<%: Constants.PARAM_ORDERWORKFLOW_REQUESTS %>', JSON.stringify(requests));

				// Send request
				var request = sendAjaxRequestWithPostFormData('<%: this.OrderWorkflowGetterUrl %>', formData);
				request.done(function (response, textStatus, xmlHttpRequest) {
					checkSessionTimeoutForCallAjax(xmlHttpRequest);

					if ((response !== null) && (response !== '')) {
						// Set display
						$(response).each(function (i) {
							var data = this;
							_this.set(data);
						});
					}

					// Call the next request
					_this.loadSequence(nextRequests, _this.next());
					_this.requestCount += _this.sequenceCount;
				});
				request.fail(function (xmlHttpRequest, status, error) {
					// Call the next request
					_this.loadSequence(nextRequests, _this.next());
					_this.requestCount += _this.sequenceCount;
				});

				// Add ajax request to list
				_this.ajaxRequests.push(request)
			},
			set: function (data) {
				var _this = this;
				$(_this.wrapperSelector).each(function (i) {
					var wrapper = $(this);
					var wrapperActions = wrapper.find(_this.wrapperActionSelector);
					var wrapperConditions = wrapper.find(_this.wrapperConditionSelector);
					var wrapperTargetOrderCountValue = wrapper.find(_this.wrapperTargetOrderCountValueSelector);
					var isLoadTargetCount = wrapper.data('is-load');

					if ((data.workflowKbn === wrapper.data('workflow-kbn').replace('kbn', ''))
						&& (data.workflowNo === wrapper.data('workflow-no').replace('no', ''))
						&& (data.workflowType === wrapper.data('workflow-type'))) {

						if (isLoadTargetCount) {
							// Set display order count
							$(wrapperTargetOrderCountValue).html(data.orderCount);
						}

						// Set display workflow actions
						var actions = getOrderWorkflowSearchConditions(data.actions);
						$(wrapperActions).empty();
						$(wrapperActions).append(actions);

						// Set display workflow conditions
						var conditions = getOrderWorkflowSearchConditions(data.conditions);
						$(wrapperConditions).empty();
						$(wrapperConditions).append(conditions);

						popover.reloadContent(i);
						return false;
					}
				});
			},
			reload : function (workflowKbn, workflowNo, workflowType) {
				var _this = this;
				_this.ajaxRequests = [];
				_this.workflowTargetOrderCountRequests = [];
				$(_this.wrapperSelector).each(function (i) {
					var wrapper = $(this);
					var wrapperActions = wrapper.find(_this.wrapperActionSelector);
					var wrapperConditions = wrapper.find(_this.wrapperConditionSelector);
					var wrapperTargetOrderCountValue = wrapper.find(_this.wrapperTargetOrderCountValueSelector);
					var isLoadTargetCount = wrapper.data('is-load');

					if ((workflowKbn === wrapper.data('workflow-kbn').replace('kbn', ''))
						&& (workflowNo === wrapper.data('workflow-no').replace('no', ''))
						&& (workflowType === wrapper.data('workflow-type'))) {

						// Add loading
						$(wrapperActions).empty();
						$(wrapperActions).append(_this.loadingElementTemplate);
						$(wrapperConditions).empty();
						$(wrapperConditions).append(_this.loadingElementTemplate);
						if (isLoadTargetCount) {
							$(wrapperTargetOrderCountValue).html(_this.loadingElementOrderCountTemplate);
						}
						popover.reloadContent(i);

						// Create formdata request
						var workflowTargetOrderCountRequest = {
							workflowKbn: workflowKbn,
							workflowNo: workflowNo,
							workflowType: workflowType
						};
						_this.workflowTargetOrderCountRequests.push(workflowTargetOrderCountRequest);
						var formData = new FormData();
						formData.append('<%= Constants.PARAM_ORDERWORKFLOW_ACTION_KBN %>', '<%= Constants.ORDERWORKFLOW_ACTION_KBN_ACTION_CONDITIONS %>');
						formData.append('<%: Constants.PARAM_ORDERWORKFLOW_REQUESTS %>', JSON.stringify(_this.workflowTargetOrderCountRequests));

						// Send request
						var request = sendAjaxRequestWithPostFormData('<%: this.OrderWorkflowGetterUrl %>', formData);
						request.done(function (response, textStatus, xmlHttpRequest) {
							checkSessionTimeoutForCallAjax(xmlHttpRequest);

							if ((response !== null) && (response !== '')) {
								// Set display
								$(response).each(function (i) {
									var data = this;
									_this.set(data);
								});
							}
						});

						// Add ajax request to list
						_this.ajaxRequests.push(request)
						return false;
					}
				});
			},
			clear : function () {
				var _this = this;
				_this.workflowTargetOrderCountRequests = [];
				if (_this.ajaxRequests.length == 0) return;

				// Cancel current ajax requests are running
				$(_this.ajaxRequests).each(function (index, ajaxRequest) {
					if (ajaxRequest.readyState !== 4) {
						ajaxRequest.abort();
					}
				});

				_this.ajaxRequests = [];
			}
		}

		// Find workflow index
		function findWorkflowIndex() {
			for (var index = 0; index < workflowList.length; index++) {
				var workflow = workflowList[index];
				if ((workflow.workflowType == urlQueryString.workflow_type)
					&& (workflow.workflowNo == urlQueryString.owno)) {
					return index;
				}
			}
		}

		// Back to workflow list
		function backToWorkflowList() {
			clearSearch(false);
			urlQueryString["<%= Constants.REQUEST_KEY_WORKFLOW_TYPE %>"] = '';
			urlQueryString["<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_NO %>"] = '';
			urlQueryString["<%= Constants.REQUEST_KEY_DISPLAY_KBN %>"] = '';
			urlQueryString["<%= Constants.REQUEST_KEY_PAGE_NO %>"] = '';
			updateUrl();

			// Clear all ajax request, reload all display workflow action, condition and target order count
			displayWorkflowActionAndCondition.clear();
			displayWorkflowActionAndCondition.ini();
		}

		// Get order workflow search conditions
		function getOrderWorkflowSearchConditions(conditions) {
			var result = '';
			for (var index = 0; index < conditions.length; index++) {
				result += '<dl>'
					+ '    <dt style="width: 20%;">' + conditions[index].key + '</dt>'
					+ '    <dd class="js-line-clamp-popover">' + conditions[index].value + '</dd>'
					+ '</dl>';
			}
			return result;
		}

		// Search order workflow
		function searchOrderWorkflow() {
			if (event.keyCode != 13) return;

			// When press enter
			var workflowKbn = getWorkflowKbnUrlParam();
			getOrderWorkflowListByWorkflowKbn(workflowKbn);
		}

		// Get workflow kbn url param
		function getWorkflowKbnUrlParam() {
			var urlParams = new URLSearchParams(window.location.search);
			var workflowKbn = urlParams.get("<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_KBN %>");
			if (workflowKbn == null) return '001';
			return workflowKbn;
		}

		// Page reload
		function pageReload(xmlHttpRequest, status, error) {

			// Reload page when login session expired
			if (xmlHttpRequest.status === 401) {
				window.location.reload();
			} else {
				if ((xmlHttpRequest.responseText === '') || (xmlHttpRequest.responseText === undefined)) return;

				// Notification error
				notification.show('<%: WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PROCESS_PROCESSING_IS_INTERRUPTED) %>', 'warning', 'fadeout');

				// Remove loading
				$('.order-workflow-list-block-content-loading').empty();
			}
		}

		// Open the scenario list
		function openScenarioList() {
			var url = '<%= string.Format("{0}{1}",
				Constants.PATH_ROOT_EC,
				Constants.PAGE_MANAGER_ORDERWORKFLOW_SCENARIO_LIST) %>';
			window.open(url, 'OpenScenarioList', 'width=1200,height=800,top=120,left=420,status=no,scrollbars=yes');
		}

		// Open the exec history list
		function openHistoryList() {
			var url = '<%= string.Format("{0}{1}",
				Constants.PATH_ROOT_EC,
				Constants.PAGE_MANAGER_ORDERWORKFLOW_EXEC_HISTORY_LIST) %>';
			window.open(url, 'OpenHistoryList', 'width=1200,height=800,top=120,left=420,status=no,scrollbars=yes');
		}

		// Open the target list
		function openTargetList() {
			var url = '<%= ImportTargetListUrlCreator.Create(Constants.KBN_WINDOW_POPUP) %>';
			window.open(url, 'OpenTargetList', 'width=850,height=370,top=120,left=420,status=no,scrollbars=yes');
		}

		// 今日設定
		function setToday(type) {
			// 注文日
			if (type == 'orderUpdate') {
				$('#dpOrderUpdateDateFrom').datepicker().datepicker('setDate', 'today');
				$('#dpOrderUpdateDateTo').datepicker().datepicker('setDate', 'today');
				$('.error-message-order-update').hide();
			}
				// 最終与信日
			else if (type == 'externalPaymentAuth') {
				$('#dpExternalPaymentAuthDateFrom').datepicker().datepicker('setDate', 'today');
				$('#dpExternalPaymentAuthDateTo').datepicker().datepicker('setDate', 'today');
				$('.error-message-external-payment-auth').hide();
			}
				// 配送希望日
			else if (type == 'shipping') {
				$('#dpShippingDateFrom').datepicker().datepicker('setDate', 'today');
				$('#dpShippingDateTo').datepicker().datepicker('setDate', 'today');
				$('.error-message-shipping').hide();
			}
				// 拡張ステータス更新日
			else if (type == 'extendStatusUpdate') {
				$('#dpExtendStatusUpdateDateFrom').datepicker().datepicker('setDate', 'today');
				$('#dpExtendStatusUpdateDateTo').datepicker().datepicker('setDate', 'today');
				$('.error-message-extend-status-update').hide();
			}
				// 出荷予定日
			else if (type == 'scheduledShipping') {
				$('#dpScheduledShippingDateFrom').datepicker().datepicker('setDate', 'today');
				$('#dpScheduledShippingDateTo').datepicker().datepicker('setDate', 'today');
				$('.error-message-scheduled-shipping').hide();
			}
				// 定期拡張ステータス更新日
			else if (type == 'fixedPurchaseExtendStatusUpdate') {
				$('#dpFixedPurchaseExtendStatusFrom').datepicker().datepicker('setDate', 'today');
				$('#dpFixedPurchaseExtendStatusTo').datepicker().datepicker('setDate', 'today');
				$('.error-message-fixed-purchase-extend-status-update').hide();
			}
				// 定期作成日
			else if (type == 'fixedPurchaseCreated') {
				$('#dpFixedPurchaseDateCreatedFrom').datepicker().datepicker('setDate', 'today');
				$('#dpFixedPurchaseDateCreatedTo').datepicker().datepicker('setDate', 'today');
				$('.error-message-fixed-purchase-created').hide();
			}
				// 定期更新日
			else if (type == 'fixedPurchaseChanged') {
				$('#dpFixedPurchaseDateChangedFrom').datepicker().datepicker('setDate', 'today');
				$('#dpFixedPurchaseDateChangedTo').datepicker().datepicker('setDate', 'today');
				$('.error-message-fixed-purchase-changed').hide();
			}
				// 定期最終購入日
			else if (type == 'fixedPurchaseLastOrdered') {
				$('#dpFixedPurchaseDateLastOrderedFrom').datepicker().datepicker('setDate', 'today');
				$('#dpFixedPurchaseDateLastOrderedTo').datepicker().datepicker('setDate', 'today');
				$('.error-message-fixed-purchase-last-ordered').hide();
			}
				// 定期購入開始日
			else if (type == 'fixedPurchaseBegin') {
				$('#dpFixedPurchaseDateBeginFrom').datepicker().datepicker('setDate', 'today');
				$('#dpFixedPurchaseDateBeginTo').datepicker().datepicker('setDate', 'today');
				$('.error-message-fixed-purchase-begin').hide();
			}
				// 次回配送日
			else if (type == 'fixedPurchaseNextShipping') {
				$('#dpFixedPurchaseNextShippingDateFrom').datepicker().datepicker('setDate', 'today');
				$('#dpFixedPurchaseNextShippingDateTo').datepicker().datepicker('setDate', 'today');
				$('.error-message-fixed-purchase-next-shipping').hide();
			}
				// 次々回配送日
			else if (type == 'fixedPurchaseNextNextShipping') {
				$('#dpFixedPurchaseNextNextShippingDateFrom').datepicker().datepicker('setDate', 'today');
				$('#dpFixedPurchaseNextNextShippingDateTo').datepicker().datepicker('setDate', 'today');
				$('.error-message-fixed-purchase-next-next-shipping').hide();
			}
		}

		// 昨日設定
		function setYesterday(type) {
			// 注文日
			if (type == 'orderUpdate') {
				$('#dpOrderUpdateDateFrom').datepicker().datepicker('setDate', -1);
				$('#dpOrderUpdateDateTo').datepicker().datepicker('setDate', -1);
				$('.error-message-order-update').hide();
			}
				// 最終与信日
			else if (type == 'externalPaymentAuth') {
				$('#dpExternalPaymentAuthDateFrom').datepicker().datepicker('setDate', -1);
				$('#dpExternalPaymentAuthDateTo').datepicker().datepicker('setDate', -1);
				$('.error-message-external-payment-auth').hide();
			}
				// 配送希望日
			else if (type == 'shipping') {
				$('#dpShippingDateFrom').datepicker().datepicker('setDate', -1);
				$('#dpShippingDateTo').datepicker().datepicker('setDate', -1);
				$('.error-message-shipping').hide();
			}
				// 拡張ステータス更新日
			else if (type == 'extendStatusUpdate') {
				$('#dpExtendStatusUpdateDateFrom').datepicker().datepicker('setDate', -1);
				$('#dpExtendStatusUpdateDateTo').datepicker().datepicker('setDate', -1);
				$('.error-message-extend-status-update').hide();
			}
				// 出荷予定日
			else if (type == 'scheduledShipping') {
				$('#dpScheduledShippingDateFrom').datepicker().datepicker('setDate', -1);
				$('#dpScheduledShippingDateTo').datepicker().datepicker('setDate', -1);
				$('.error-message-scheduled-shipping').hide();
			}
				// 定期拡張ステータス更新日
			else if (type == 'fixedPurchaseExtendStatusUpdate') {
				$('#dpFixedPurchaseExtendStatusFrom').datepicker().datepicker('setDate', -1);
				$('#dpFixedPurchaseExtendStatusTo').datepicker().datepicker('setDate', -1);
				$('.error-message-fixed-purchase-extend-status-update').hide();
			}
				// 定期作成日
			else if (type == 'fixedPurchaseCreated') {
				$('#dpFixedPurchaseDateCreatedFrom').datepicker().datepicker('setDate', -1);
				$('#dpFixedPurchaseDateCreatedTo').datepicker().datepicker('setDate', -1);
				$('.error-message-fixed-purchase-created').hide();
			}
				// 定期更新日
			else if (type == 'fixedPurchaseChanged') {
				$('#dpFixedPurchaseDateChangedFrom').datepicker().datepicker('setDate', -1);
				$('#dpFixedPurchaseDateChangedTo').datepicker().datepicker('setDate', -1);
				$('.error-message-fixed-purchase-changed').hide();
			}
				// 定期最終購入日
			else if (type == 'fixedPurchaseLastOrdered') {
				$('#dpFixedPurchaseDateLastOrderedFrom').datepicker().datepicker('setDate', -1);
				$('#dpFixedPurchaseDateLastOrderedTo').datepicker().datepicker('setDate', -1);
				$('.error-message-fixed-purchase-last-ordered').hide();
			}
				// 定期購入開始日
			else if (type == 'fixedPurchaseBegin') {
				$('#dpFixedPurchaseDateBeginFrom').datepicker().datepicker('setDate', -1);
				$('#dpFixedPurchaseDateBeginTo').datepicker().datepicker('setDate', -1);
				$('.error-message-fixed-purchase-begin').hide();
			}
				// 次回配送日
			else if (type == 'fixedPurchaseNextShipping') {
				$('#dpFixedPurchaseNextShippingDateFrom').datepicker().datepicker('setDate', -1);
				$('#dpFixedPurchaseNextShippingDateTo').datepicker().datepicker('setDate', -1);
				$('.error-message-fixed-purchase-next-shipping').hide();
			}
				// 次々回配送日
			else if (type == 'fixedPurchaseNextNextShipping') {
				$('#dpFixedPurchaseNextNextShippingDateFrom').datepicker().datepicker('setDate', -1);
				$('#dpFixedPurchaseNextNextShippingDateTo').datepicker().datepicker('setDate', -1);
				$('.error-message-fixed-purchase-next-next-shipping').hide();
			}
		}

		// 今月設定
		function setThisMonth(type) {
			var date = new Date();
			var currentMonth = date.getMonth();
			var currentYear = date.getFullYear();
			var firstDate = new Date(currentYear, currentMonth, 1);
			var lastDate = new Date(currentYear, currentMonth + 1, 0);

			// 注文日
			if (type == 'orderUpdate') {
				$('#dpOrderUpdateDateFrom').datepicker().datepicker('setDate', firstDate);
				$('#dpOrderUpdateDateTo').datepicker().datepicker('setDate', lastDate);
				$('.error-message-order-update').hide();
			}
				// 最終与信日
			else if (type == 'externalPaymentAuth') {
				$('#dpExternalPaymentAuthDateFrom').datepicker().datepicker('setDate', firstDate);
				$('#dpExternalPaymentAuthDateTo').datepicker().datepicker('setDate', lastDate);
				$('.error-message-external-payment-auth').hide();
			}
				// 配送希望日
			else if (type == 'shipping') {
				$('#dpShippingDateFrom').datepicker().datepicker('setDate', firstDate);
				$('#dpShippingDateTo').datepicker().datepicker('setDate', lastDate);
				$('.error-message-shipping').hide();
			}
				// 拡張ステータス更新日
			else if (type == 'extendStatusUpdate') {
				$('#dpExtendStatusUpdateDateFrom').datepicker().datepicker('setDate', firstDate);
				$('#dpExtendStatusUpdateDateTo').datepicker().datepicker('setDate', lastDate);
				$('.error-message-extend-status-update').hide();
			}
				// 出荷予定日
			else if (type == 'scheduledShipping') {
				$('#dpScheduledShippingDateFrom').datepicker().datepicker('setDate', firstDate);
				$('#dpScheduledShippingDateTo').datepicker().datepicker('setDate', lastDate);
				$('.error-message-scheduled-shipping').hide();
			}
				// 定期拡張ステータス更新日
			else if (type == 'fixedPurchaseExtendStatusUpdate') {
				$('#dpFixedPurchaseExtendStatusFrom').datepicker().datepicker('setDate', firstDate);
				$('#dpFixedPurchaseExtendStatusTo').datepicker().datepicker('setDate', lastDate);
				$('.error-message-fixed-purchase-extend-status-update').hide();
			}
				// 定期作成日
			else if (type == 'fixedPurchaseCreated') {
				$('#dpFixedPurchaseDateCreatedFrom').datepicker().datepicker('setDate', firstDate);
				$('#dpFixedPurchaseDateCreatedTo').datepicker().datepicker('setDate', lastDate);
				$('.error-message-fixed-purchase-created').hide();
			}
				// 定期更新日
			else if (type == 'fixedPurchaseChanged') {
				$('#dpFixedPurchaseDateChangedFrom').datepicker().datepicker('setDate', firstDate);
				$('#dpFixedPurchaseDateChangedTo').datepicker().datepicker('setDate', lastDate);
				$('.error-message-fixed-purchase-changed').hide();
			}
				// 定期最終購入日
			else if (type == 'fixedPurchaseLastOrdered') {
				$('#dpFixedPurchaseDateLastOrderedFrom').datepicker().datepicker('setDate', firstDate);
				$('#dpFixedPurchaseDateLastOrderedTo').datepicker().datepicker('setDate', lastDate);
				$('.error-message-fixed-purchase-last-ordered').hide();
			}
				// 定期購入開始日
			else if (type == 'fixedPurchaseBegin') {
				$('#dpFixedPurchaseDateBeginFrom').datepicker().datepicker('setDate', firstDate);
				$('#dpFixedPurchaseDateBeginTo').datepicker().datepicker('setDate', lastDate);
				$('.error-message-fixed-purchase-begin').hide();
			}
				// 次回配送日
			else if (type == 'fixedPurchaseNextShipping') {
				$('#dpFixedPurchaseNextShippingDateFrom').datepicker().datepicker('setDate', firstDate);
				$('#dpFixedPurchaseNextShippingDateTo').datepicker().datepicker('setDate', lastDate);
				$('.error-message-fixed-purchase-next-shipping').hide();
			}
				// 次々回配送日
			else if (type == 'fixedPurchaseNextNextShipping') {
				$('#dpFixedPurchaseNextNextShippingDateFrom').datepicker().datepicker('setDate', firstDate);
				$('#dpFixedPurchaseNextNextShippingDateTo').datepicker().datepicker('setDate', lastDate);
				$('.error-message-fixed-purchase-next-next-shipping').hide();
			}
		}

		// Clear Date Period
		function clearDatePeriod(type) {
			// 注文日
			if (type == 'orderUpdate') {
				$('#dpOrderUpdateDateFrom').datepicker().datepicker('setDate', null);
				$('#dpOrderUpdateDateTo').datepicker().datepicker('setDate', null);
				$('.error-message-order-update').hide();
			}
				// 最終与信日
			else if (type == 'externalPaymentAuth') {
				$('#dpExternalPaymentAuthDateFrom').datepicker().datepicker('setDate', null);
				$('#dpExternalPaymentAuthDateTo').datepicker().datepicker('setDate', null);
				$('.error-message-external-payment-auth').hide();
			}
				// 配送希望日
			else if (type == 'shipping') {
				$('#dpShippingDateFrom').datepicker().datepicker('setDate', null);
				$('#dpShippingDateTo').datepicker().datepicker('setDate', null);
				$('.error-message-shipping').hide();
			}
				// 拡張ステータス更新日
			else if (type == 'extendStatusUpdate') {
				$('#dpExtendStatusUpdateDateFrom').datepicker().datepicker('setDate', null);
				$('#dpExtendStatusUpdateDateTo').datepicker().datepicker('setDate', null);
				$('.error-message-extend-status-update').hide();
			}
				// 出荷予定日
			else if (type == 'scheduledShipping') {
				$('#dpScheduledShippingDateFrom').datepicker().datepicker('setDate', null);
				$('#dpScheduledShippingDateTo').datepicker().datepicker('setDate', null);
				$('.error-message-scheduled-shipping').hide();
			}
				// 定期拡張ステータス更新日
			else if (type == 'fixedPurchaseExtendStatusUpdate') {
				$('#dpFixedPurchaseExtendStatusFrom').datepicker().datepicker('setDate', null);
				$('#dpFixedPurchaseExtendStatusTo').datepicker().datepicker('setDate', null);
				$('.error-message-fixed-purchase-extend-status-update').hide();
			}
				// 定期作成日
			else if (type == 'fixedPurchaseCreated') {
				$('#dpFixedPurchaseDateCreatedFrom').datepicker().datepicker('setDate', null);
				$('#dpFixedPurchaseDateCreatedTo').datepicker().datepicker('setDate', null);
				$('.error-message-fixed-purchase-created').hide();
			}
				// 定期更新日
			else if (type == 'fixedPurchaseChanged') {
				$('#dpFixedPurchaseDateChangedFrom').datepicker().datepicker('setDate', null);
				$('#dpFixedPurchaseDateChangedTo').datepicker().datepicker('setDate', null);
				$('.error-message-fixed-purchase-changed').hide();
			}
				// 定期最終購入日
			else if (type == 'fixedPurchaseLastOrdered') {
				$('#dpFixedPurchaseDateLastOrderedFrom').datepicker().datepicker('setDate', null);
				$('#dpFixedPurchaseDateLastOrderedTo').datepicker().datepicker('setDate', null);
				$('.error-message-fixed-purchase-last-ordered').hide();
			}
				// 定期購入開始日
			else if (type == 'fixedPurchaseBegin') {
				$('#dpFixedPurchaseDateBeginFrom').datepicker().datepicker('setDate', null);
				$('#dpFixedPurchaseDateBeginTo').datepicker().datepicker('setDate', null);
				$('.error-message-fixed-purchase-begin').hide();
			}
				// 次回配送日
			else if (type == 'fixedPurchaseNextShipping') {
				$('#dpFixedPurchaseNextShippingDateFrom').datepicker().datepicker('setDate', null);
				$('#dpFixedPurchaseNextShippingDateTo').datepicker().datepicker('setDate', null);
				$('.error-message-fixed-purchase-next-shipping').hide();
			}
				// 次々回配送日
			else if (type == 'fixedPurchaseNextNextShipping') {
				$('#dpFixedPurchaseNextNextShippingDateFrom').datepicker().datepicker('setDate', null);
				$('#dpFixedPurchaseNextNextShippingDateTo').datepicker().datepicker('setDate', null);
				$('.error-message-fixed-purchase-next-next-shipping').hide();
			}
		}

		// 日付バリデーション
		// 絞り込み検索ボタン押下
		function validateDateBySearch() {
			var dateTypes = new Array(
				'extendStatusUpdate',
				'orderUpdate',
				'externalPaymentAuth',
				<% if (this.UseLeadTime) { %>
				'scheduledShipping',
				<% } %>
				'shipping',
				'fixedPurchaseExtendStatusUpdate',
				'fixedPurchaseCreated',
				'fixedPurchaseChanged',
				'fixedPurchaseLastOrdered',
				'fixedPurchaseBegin',
				'fixedPurchaseNextShipping',
				'fixedPurchaseNextNextShipping');
			var isError = false;

			$.each(dateTypes, function(index, type) {
				if (validateDate(type)) isError = true;
			});
			return isError;
		}

		// 日付バリデーション
		// 日付入力
		function validateDate(type) {
			var dateFrom;
			var dateTo;
			var errorMessages;
			var regex = new RegExp(/^[0-9]{4}\/(0[1-9]|1[0-2])\/(0[1-9]|[12][0-9]|3[01])$/);

			// 拡張ステータス更新日
			if (type == 'extendStatusUpdate') {
				dateFrom = document.getElementById("dpExtendStatusUpdateDateFrom").value.trim();
				dateTo = document.getElementById("dpExtendStatusUpdateDateTo").value.trim();
				errorMessages = document.getElementsByClassName("error-message-extend-status-update");
			}
				// ステータス更新日
			else if (type == 'orderUpdate') {
				dateFrom = document.getElementById("dpOrderUpdateDateFrom").value.trim();
				dateTo = document.getElementById("dpOrderUpdateDateTo").value.trim();
				errorMessages = document.getElementsByClassName("error-message-order-update");
			}
				// 最終与信日
			else if (type == 'externalPaymentAuth') {
				dateFrom = document.getElementById("dpExternalPaymentAuthDateFrom").value.trim();
				dateTo = document.getElementById("dpExternalPaymentAuthDateTo").value.trim();
				errorMessages = document.getElementsByClassName("error-message-external-payment-auth");
			}
				// 出荷予定日
			else if (type == 'scheduledShipping') {
				dateFrom = document.getElementById("dpScheduledShippingDateFrom").value.trim();
				dateTo = document.getElementById("dpScheduledShippingDateTo").value.trim();
				errorMessages = document.getElementsByClassName("error-message-scheduled-shipping");
			}
				// 配送希望日
			else if (type == 'shipping') {
				dateFrom = document.getElementById("dpShippingDateFrom").value.trim();
				dateTo = document.getElementById("dpShippingDateTo").value.trim();
				errorMessages = document.getElementsByClassName("error-message-shipping");
			}
				// 定期拡張ステータス更新日
			else if (type == 'fixedPurchaseExtendStatusUpdate') {
				dateFrom = document.getElementById("dpFixedPurchaseExtendStatusFrom").value.trim();
				dateTo = document.getElementById("dpFixedPurchaseExtendStatusTo").value.trim();
				errorMessages = document.getElementsByClassName("error-message-fixed-purchase-extend-status-update");
			}
				// 定期作成日
			else if (type == 'fixedPurchaseCreated') {
				dateFrom = document.getElementById("dpFixedPurchaseDateCreatedFrom").value.trim();
				dateTo = document.getElementById("dpFixedPurchaseDateCreatedTo").value.trim();
				errorMessages = document.getElementsByClassName("error-message-fixed-purchase-created");
			}
				// 定期更新日
			else if (type == 'fixedPurchaseChanged') {
				dateFrom = document.getElementById("dpFixedPurchaseDateChangedFrom").value.trim();
				dateTo = document.getElementById("dpFixedPurchaseDateChangedTo").value.trim();
				errorMessages = document.getElementsByClassName("error-message-fixed-purchase-changed");
			}
				// 定期最終購入日
			else if (type == 'fixedPurchaseLastOrdered') {
				dateFrom = document.getElementById("dpFixedPurchaseDateLastOrderedFrom").value.trim();
				dateTo = document.getElementById("dpFixedPurchaseDateLastOrderedTo").value.trim();
				errorMessages = document.getElementsByClassName("error-message-fixed-purchase-last-ordered");
			}
				// 定期購入開始日
			else if (type == 'fixedPurchaseBegin') {
				dateFrom = document.getElementById("dpFixedPurchaseDateBeginFrom").value.trim();
				dateTo = document.getElementById("dpFixedPurchaseDateBeginTo").value.trim();
				errorMessages = document.getElementsByClassName("error-message-fixed-purchase-begin");
			}
				// 次回配送日
			else if (type == 'fixedPurchaseNextShipping') {
				dateFrom = document.getElementById("dpFixedPurchaseNextShippingDateFrom").value.trim();
				dateTo = document.getElementById("dpFixedPurchaseNextShippingDateTo").value.trim();
				errorMessages = document.getElementsByClassName("error-message-fixed-purchase-next-shipping");
			}
				// 次々回配送日
			else if (type == 'fixedPurchaseNextNextShipping') {
				dateFrom = document.getElementById("dpFixedPurchaseNextNextShippingDateFrom").value.trim();
				dateTo = document.getElementById("dpFixedPurchaseNextNextShippingDateTo").value.trim();
				errorMessages = document.getElementsByClassName("error-message-fixed-purchase-next-next-shipping");
			}

			$(errorMessages).hide();
			var isError = false;
			if (dateFrom && !regex.test(dateFrom)) {
				$(errorMessages[0]).show();
				isError = true;
			}
			if (dateTo && !regex.test(dateTo)) {
				$(errorMessages[1]).show();
				isError = true;
			}
			if (isError || !dateFrom || !dateTo) return isError;

			if (new Date(dateFrom) > new Date(dateTo)) {
				$(errorMessages[2]).show();
				isError = true;
			}
			return isError;
		}

		function errorMessageReset() {
            $('.error-message-order-update').hide();
            $('.error-message-external-payment-auth').hide();
            $('.error-message-shipping').hide();
            $('.error-message-extend-status-update').hide();
            $('.error-message-scheduled-shipping').hide();
            $('.error-message-fixed-purchase-extend-status-update').hide();
            $('.error-message-fixed-purchase-created').hide();
            $('.error-message-fixed-purchase-changed').hide();
            $('.error-message-fixed-purchase-last-ordered').hide();
            $('.error-message-fixed-purchase-begin').hide();
            $('.error-message-fixed-purchase-next-shipping').hide();
            $('.error-message-fixed-purchase-next-next-shipping').hide();
        }

		// テキストボックスの有効無効切り替えイベントの設定
		$(function () {
			// 注文メモのテキストボックス
			SetDisplayMemoTextBox($("#ddlOrderMemoFlg"), $("#tbOrderMemo"));
			$("#ddlOrderMemoFlg").change(function() {
				SetDisplayMemoTextBox($("#ddlOrderMemoFlg"), $("#tbOrderMemo"));
			});

			// 管理メモのテキストボックス
			SetDisplayMemoTextBox($("#ddlOrderManagementMemoFlg"), $("#tbManagementMemo"));
			$("#ddlOrderManagementMemoFlg").change(function() {
				SetDisplayMemoTextBox($("#ddlOrderManagementMemoFlg"), $("#tbManagementMemo"));
			});

			// 配送メモのテキストボックス
			SetDisplayMemoTextBox($("#ddlShippingMemoFlg"), $("#tbShippingMemo"));
			$("#ddlShippingMemoFlg").change(function() {
				SetDisplayMemoTextBox($("#ddlShippingMemoFlg"), $("#tbShippingMemo"));
			});

			// 決済連携メモのテキストボックス
			SetDisplayMemoTextBox($("#ddlOrderPaymentMemoFlg"), $("#tbPaymentMemo"));
			$("#ddlOrderPaymentMemoFlg").change(function() {
				SetDisplayMemoTextBox($("#ddlOrderPaymentMemoFlg"), $("#tbPaymentMemo"));
			});

			// 外部連携メモのテキストボックス
			SetDisplayMemoTextBox($("#ddlOrderRelationMemoFlg"), $("#tbRelationMemo"));
			$("#ddlOrderRelationMemoFlg").change(function() {
				SetDisplayMemoTextBox($("#ddlOrderRelationMemoFlg"), $("#tbRelationMemo"));
			});

			// ユーザー特記欄のテキストボックス
			SetDisplayMemoTextBox($("#ddlUserMemoFlg"), $("#tbUserMemo"));
			$("#ddlUserMemoFlg").change(function() {
				SetDisplayMemoTextBox($("#ddlUserMemoFlg"), $("#tbUserMemo"));
			});

			// 商品付帯情報のテキストボックス
			SetDisplayMemoTextBox($("#ddlProductOptionFlg"), $("#tbProductOption"));
			$("#ddlProductOptionFlg").change(function() {
				SetDisplayMemoTextBox($("#ddlProductOptionFlg"), $("#tbProductOption"));
			});

			// 定期管理メモのテキストボックス
			SetDisplayMemoTextBox($("#ddlFixedPurchaseManagementMemoFlg"), $("#tbFixedPurchaseManagementMemo"));
			$("#ddlFixedPurchaseManagementMemoFlg").change(function() {
				SetDisplayMemoTextBox($("#ddlFixedPurchaseManagementMemoFlg"), $("#tbFixedPurchaseManagementMemo"));
			});

			// 定期配送メモのテキストボックス
			SetDisplayMemoTextBox($("#ddlFixedPurchaseShippingMemoFlg"), $("#tbFixedPurchaseShippingMemo"));
			$("#ddlFixedPurchaseShippingMemoFlg").change(function() {
				SetDisplayMemoTextBox($("#ddlFixedPurchaseShippingMemoFlg"), $("#tbFixedPurchaseShippingMemo"));
			});
		});

		// Form Reset
		function reset() {
			document.<%= this.Form.ClientID %>.reset();
			SetDisplayMemoTextBox($("#ddlOrderMemoFlg"), $("#tbOrderMemo"));
			SetDisplayMemoTextBox($("#ddlOrderManagementMemoFlg"), $("#tbManagementMemo"));
			SetDisplayMemoTextBox($("#ddlShippingMemoFlg"), $("#tbShippingMemo"));
			SetDisplayMemoTextBox($("#ddlOrderPaymentMemoFlg"), $("#tbPaymentMemo"));
			SetDisplayMemoTextBox($("#ddlOrderRelationMemoFlg"), $("#tbRelationMemo"));
			SetDisplayMemoTextBox($("#ddlUserMemoFlg"), $("#tbUserMemo"));
			SetDisplayMemoTextBox($("#ddlProductOptionFlg"), $("#tbProductOption"));
			SetDisplayMemoTextBox($("#ddlFixedPurchaseManagementMemoFlg"), $("#tbFixedPurchaseManagementMemo"));
			SetDisplayMemoTextBox($("#ddlFixedPurchaseShippingMemoFlg"), $("#tbFixedPurchaseShippingMemo"));

			orderExtendNameChange('');
			fixedPurchaseOrderExtendNameChange('');
		}

		// ドロップダウンの値に応じて、テキストボックスの有効無効を切り替える。
		function SetDisplayMemoTextBox($ddlElement, $tbElement) {
			if ($ddlElement.val() != 1) {
				$tbElement.val("");
				$tbElement.attr("disabled", "disabled");
			} else {
				$tbElement.removeAttr("disabled");
			}
		}

		// Set bottom cassette
		function setBottomCassette() {
			$('#bottomTitle').text('');
			$('#btnSubmit').val('選択した処理を実行');
			$('#cassetteExport').html(setExportButton());
			$('#lineExport').empty();
		}

		// Set bottom line
		function setBottomLine() {
			$('#bottomTitle').text('選択実行');
			$('#btnSubmit').val('実行');
			$('#lineExport').html(setExportButton());
			$('#cassetteExport').empty();
		}

		// Check process
		function checkProcess(workflowIndex) {
			urlQueryString["<%= Constants.REQUEST_KEY_WORKFLOW_TYPE %>"] = workflowList[workflowIndex].workflowType;
			urlQueryString["<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_KBN %>"] = workflowList[workflowIndex].workflowKbn;
			urlQueryString["<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_NO %>"] = workflowList[workflowIndex].workflowNo;
			urlQueryString["display_kbn"] = workflowList[workflowIndex].displayKbn;

			setInterval(setDivCss, 100);
			currentWorkflowIndex = workflowIndex;

			// Check search type
			if (workflowList[workflowIndex].workflowType == '<%= Constants.FLG_ORDERWORKFLOWSETTING_TARGET_WORKFLOW_TYPE_ORDER %>') {
				$('#divOrderSearch').show();
				$('#divFixedPurchaseSearch').hide();
			} else {
				$('#divOrderSearch').hide();
				$('#divFixedPurchaseSearch').show();
			}

			setEnableNextBtn(workflowIndex);

			if (workflowList[workflowIndex].workflowDetailKbn === '<%= Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN_ODR_IMP %>') {
				$('#divDetailNormalData').hide();
				$('#divDataBottom').hide();
				$('#divUpload').show();

				getOrderFileImportSetting();
			} else {
				$('#divDetailNormalData').show();
				$('#divDataBottom').show();
				$('#divUpload').hide();
				$('#divDataBody').empty();

				if (workflowList[workflowIndex].displayKbn === '<%= Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_LINE %>') {
					$('.order-workflow-list-block-data-bottom-row-all').show();
					$('#divDataBody')
						.append('<div id="divLine" class="order-workflow-list-block-data-table-wrapper scroll-table-wrapper table-wrapper js-list-table-wrapper" style="max-width:179vh">'
							+'    <table class="order-workflow-list-block-data-table table" _fixedhead="cols:1;" id="tbLine">'
							+'        <thead id="tHeader"></thead>'
							+'        <tbody id="tData" class="workflow-data"></tbody>'
							+'    </table>'
							+'</div>');
					setBottomLine();
				} else {
					$('.order-workflow-list-block-data-bottom-row-all').hide();
					$('#divDataBody')
						.append('<div id="divCassette" class="order-workflow-list-block-data-cassette-wrapper">'
							+'    <div class="order-workflow-list-block-data-cassette-inner">'
							+'        <ul class="order-workflow-list-block-data-cassette-list" id="ulData"></ul>'
							+'    </div>'
							+'</div>'
							+'<div class="order-workflow-list-block-data-pager" id="divBottomPager"></div>');
					setBottomCassette();
				}

				var pageNo = (urlQueryString.pno ? urlQueryString.pno : 1);
				getWorkflowExecData(pageNo);
			}
		}

		// Get workflow exec data
		function getWorkflowExecData(page) {
			urlQueryString["<%= Constants.REQUEST_KEY_PAGE_NO%>"] = page;
			updateUrl();
			setSearchParam();
			currentPage = page;
			var requestData = {
				currentPage: page,
				workflowNo: workflowList[currentWorkflowIndex].workflowNo,
				workflowKbn: workflowList[currentWorkflowIndex].workflowKbn,
				workflowType: workflowList[currentWorkflowIndex].workflowType,
				WorkflowDisplayKbn: workflowList[currentWorkflowIndex].displayKbn,
				searchCondition: urlQueryString,
			};

			showLoading(2);

			// Clear all ajax request and call ajax request for current display workflow action, condition and target order count
			displayWorkflowActionAndCondition.clear();
			displayWorkflowActionAndCondition.reload(
				workflowList[currentWorkflowIndex].workflowKbn,
				workflowList[currentWorkflowIndex].workflowNo,
				workflowList[currentWorkflowIndex].workflowType);

			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/GetWorkflowExecData";
			var dataRequest = "{ data:" + JSON.stringify(requestData) + "}";
			var request = callAjax(urlRequest, dataRequest);

			request.done(function(response) {
				var data = JSON.parse(response.d);
				clearData();

				// Export
				loadExportSetting(workflowList[currentWorkflowIndex].workflowType);
				setEnableExportButton(data.TotalCase);

				// Pager
				bindTopPagerData(data, page);

				// Display exec data
				switch(workflowList[currentWorkflowIndex].displayKbn) {
					case '<%= Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_LINE %>':
						bindLineData(data);
						break;

					case '<%= Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_CASSETTE %>':
						bindCassetteData(data);
						break;
				}

				// Set up js function
				workflow.list_table.ini();

				if (workflowList[currentWorkflowIndex].displayKbn == '<%= Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_LINE %>') {
					workflow.list_table.message.show();
				} else {
					workflow.list_table.message.obj.hide();
					if ($('.order-workflow-list-block-data-cassette-list-item').length != 0) {
						$('.order-workflow-list-block-data-cassette-list-item-process-list-radio').first().find('input[type=radio]').trigger("change");
					} else {
						workflow.list_table.html_submit_num.html('0');
					}
				}

				check_list_all.ini();

				setTimeout(function() {
					FixedMidashi.create();
					check_list_all.event_set();
				}, 300)

				table_row_check.ini();
				workflow.list_table.html_submit_all_modal_num.html(data.TotalCase);
				workflow.list_table.html_submit_num_all.html(data.TotalCase);

				// Update display order count for current workflow setting selection
				var workflowKbn = workflowList[currentWorkflowIndex].workflowKbn;
				var workflowNo = workflowList[currentWorkflowIndex].workflowNo;
				var workflowType = workflowList[currentWorkflowIndex].workflowType;
				updateDisplayOrderCountForWorkflowSetting(workflowKbn, workflowNo, workflowType, data.TotalCase);

				// Display search input
				if (data.HasSearchBox) {
					workflow.list_table.html_data_search.show();
				} else {
					workflow.list_table.html_data_search.hide();
				}

				if (data.IsOver100 || (data.HasValue == false)) {
					$('#divDataBody').css('display', 'block');
					$('#divCassette').css('display', 'block');
				} else {
					$('#divDataBody').css('display', 'inline-grid');
					$('#divCassette').css('display', 'inline-grid');
				}

				if (data.HasValue == false) {
					$('.order-workflow-list-block-data-submit-btn-all').prop('disabled', 'disabled');
					$('.export-button-list input[name="btnInteractionDataExport"]').hide();
				} else {
					$('.order-workflow-list-block-data-submit-btn-all').prop('disabled', '');
					$('.export-button-list input[name="btnInteractionDataExport"]').show();
				}

				// Check display datetime input
				if (workflowType == '<%= Constants.FLG_ORDERWORKFLOWSETTING_TARGET_WORKFLOW_TYPE_ORDER %>'
					&& (data.UpdateStatusValid)) {
					$('.extend-status-update').show();
				} else {
					$('.extend-status-update').hide();
				}

				$('.shipping-date-update').hide();
				$('.scheduled-shipping-date-update').hide();
				$('.next-shipping-date-update').hide();
				$('.next-next-shipping-date-update').hide();

				$.each(data.Actions, function(key, action) {
					var actionKey = action.key.split('&')[0];
					var actionValue = action.key.split('&')[1];

					if ((actionKey == '<%= Constants.FIELD_ORDERWORKFLOWSETTING_SCHEDULED_SHIPPING_DATE_ACTION %>')
						&& (actionValue == '<%= Constants.FLG_ORDERWORKFLOWSETTING_SCHEDULED_SHIPPING_DATE_ACTION_ON %>')) {
						$('.scheduled-shipping-date-update').show();
					}

					if ((actionKey == '<%= Constants.FIELD_ORDERWORKFLOWSETTING_SHIPPING_DATE_ACTION %>')
						&& ((actionValue == '<%= Constants.FLG_ORDERWORKFLOWSETTING_SHIPPING_DATE_ACTION_ON_CALCULATE_SCHEDULED_SHIPPING_DATE %>')
							|| (actionValue == '<%= Constants.FLG_ORDERWORKFLOWSETTING_SHIPPING_DATE_ACTION_ON_NONCALCULATE_SCHEDULED_SHIPPING_DATE %>'))) {
						$('.shipping-date-update').show();
					}

					if ((actionKey == '<%= Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_NEXT_SHIPPING_DATE_CHANGE %>')
						&& ((actionValue == '<%= Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_NEXT_SHIPPING_DATE_CHANGE_ACTION_ON_WITH_CALCULATE_NEXT_NEXT_SHIPPINGDATE %>')
							|| (actionValue == '<%= Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_NEXT_SHIPPING_DATE_CHANGE_ACTION_ON %>'))) {
						$('.next-shipping-date-update').show();
					}

					if ((actionKey == '<%= Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_NEXT_NEXT_SHIPPING_DATE_CHANGE %>')
						&& (actionValue == '<%= Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_NEXT_NEXT_SHIPPING_DATE_CHANGE_ACTION_ON %>')) {
						$('.next-next-shipping-date-update').show();
					}
				});

				// Load modal confirm content
				var actionData = getOrderWorkflowSearchConditions(data.ActionsForConfirm);
				if (actionData) {
					$('.modal-order-workflow-submit-confirm-content-action-list').html(actionData);
				} else {
					$('.modal-order-workflow-submit-confirm-content-action-list').html('<%= WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERWORKFLOWSETTING_ACTION_UNSELECTED_ERROR) %>');
				}

				// Check workflow running
				checkRunningWorkflowHistory(data);

				hideLoading(2);
			});
		}

		// Check running workflow history
		function checkRunningWorkflowHistory(data) {
			if (data.RunningWorkflowHistory != null) {
				$('.order-workflow-list-block-select-other').css('justify-content','space-between')
				$('#divWorkflowRunningMessage').show();
				$('#spWorkflowRunningMessage').html(data.RunningWorkflowHistory.ErrorMessage);
				$('#btnOpenRunningHistory').attr('data', data.RunningWorkflowHistory.OrderWorkflowExecHistoryId);
				workflow.list_table.html_submit_all_btn.prop('disabled', true);
				$('#btnSubmit').hide()
			} else {
				$('#divWorkflowRunningMessage').hide();
				$('.order-workflow-list-block-select-other').css('justify-content','flex-end')
				workflow.list_table.html_submit_all_btn.prop('disabled', false);
				$('#btnSubmit').show()
			}
		}

		// Open workflow running history
		function openWorkflowRunningHistory(input) {
			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/CreateUrlAnOrderWorkflowExecHistoryDetails";
			var dataRequest = "{ orderWorkflowExecHistoryId:" + input.getAttribute('data') + "}";
			var request = callAjax(urlRequest, dataRequest);
			request.done(function(response){
				javascript:open_window(response.d,'Import','width=1200,height=600,top=120,left=420,status=no,scrollbars=yes');
			});
		}

		// Set line table header
		function setLineTableHeader() {
			$('#tHeader').empty();
			var header = "";
			if (workflowList[currentWorkflowIndex].workflowType == '<%= Constants.FLG_ORDERWORKFLOWSETTING_TARGET_WORKFLOW_TYPE_ORDER %>') {
				header += '<tr>'
					+ '<td class="ta-center"><input type="checkbox" name="" class="js-check-all-btn" /></td>'
					+ '<th>注文ID</th>';
				if (this.workflowDispSetting) {
					for (var index = 0; index < this.workflowDispSetting.length; index++) {
						var setting = workflowDispSetting[index];
						
						switch(setting.DispColmunName) {
							case '<%= Constants.FIELD_ORDER_CARD_TRAN_ID %>':
								header += '<th>決済取引ID</th>';
								break;

							case '<%= Constants.FIELD_ORDER_DIGITAL_CONTENTS_FLG %>':
								header += '<th>デジタルコンテンツ	</th>';
								break;

							case '<%= Constants.FIELD_ORDER_GIFT_FLG %>':
								<% if (Constants.GIFTORDER_OPTION_ENABLED)
		{ %>
								header += '<th>ギフト</th>';
								<% } %>
								break;

							case '<%= Constants.FIELD_ORDER_MALL_ID %>':
								<% if (Constants.MALLCOOPERATION_OPTION_ENABLED)
		{ %>
								header += '<th>サイト</th>';
								<% } %>
								break;

							case '<%= Constants.FIELD_ORDER_MANAGEMENT_MEMO %>':
								header += '<th>管理メモ</th>';
								break;

							case '<%= Constants.FIELD_ORDER_MEMO %>':
								header += '<th>注文メモ</th>';
								break;

							case '<%= Constants.FIELD_ORDER_ORDER_DATE %>':
								header += '<th>注文日時</th>';
								break;

							case '<%= Constants.FIELD_ORDER_ORDER_KBN %>':
								header += '<th>注文区分</th>';
								break;

							case '<%= Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS %>':
								header += '<th>入金ステータス</th>';
								break;

							case '<%= Constants.FIELD_ORDER_ORDER_PRICE_TOTAL %>':
								header += '<th>合計金額</th>';
								break;

							case '<%= Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS %>':
								header += '<th>返金ステータス</th>'
								break;

							case '<%= Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_RECEIPT_DATE %>':
								header += '<th>返品交換受付日</th>';
								break;

							case '<%= Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS %>':
								header += '<th>返品交換ステータス</th>';
								break;

							case '<%= Constants.FIELD_ORDER_ORDER_SHIPPED_STATUS %>':
								<% if (Constants.REALSTOCK_OPTION_ENABLED) { %>
								header += '<th>出荷状況</th>';
								<% } %>
								break;

							case '<%= Constants.FIELD_ORDER_ORDER_STATUS %>':
								header += '<th>注文ステータス</th>';
								break;

							case '<%= Constants.FIELD_ORDER_STOREPICKUP_STATUS %>':
								header += '<th>店舗受取ステータス</th>';
								break;

							case '<%= Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS %>':
								<% if (Constants.REALSTOCK_OPTION_ENABLED) { %>
								header += '<th>引当状況</th>';
								<% } %>
								break;

							case '<%= Constants.FIELD_ORDEROWNER_OWNER_KBN %>':
								header += '<th>注文者区分</th>';
								break;

							case '<%= Constants.FIELD_ORDEROWNER_OWNER_NAME %>':
								header += '<th>氏名</th>';
								break;

							case '<%= Constants.FIELD_PAYMENT_PAYMENT_NAME %>':
								header += '<th>決済方法</th>';
								break;

							case '<%= Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN %>':
								header += '<th>返品交換区分</th>';
								break;

							case '<%= Constants.FIELD_ORDER_USER_ID %>':
								header += '<th>ユーザーID</th>';
								break;

							case '<%= Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID %>':
								header += '<th>ユーザー管理レベル</th>';
								break;

							case '<%= Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE %>':
								header += '<th>出荷予定日</th>';
								break;

							case '<%= Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE %>':
								header += '<th>配送希望日</th>';
								break;

							case '<%= Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME %>':
								header += '<th>配送希望時間帯</th>';
								break;

							case '<%= Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT %>':
								header += '<th>定期購入回数(注文時点)</th>';
								break;

							case '<%= Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT %>':
								header += '<th>定期購入回数(出荷時点)</th>';
								break;

							case '<%= Constants.FIELD_ORDER_ORDER_COUNT_ORDER %>':
								header += '<th>購入回数（注文基準）</th>';
								break;

							case '<%= Constants.FIELD_ORDER_DEMAND_STATUS %>':
								header += '<th>督促ステータス</th>';
								break;

							case '<%= Constants.FIELD_ORDER_SHIPPING_MEMO %>':
								header += '<th>配送メモ</th>';
								break;
						}
					}
				}
				header += '</tr>';
			} else {
				header += '<tr>'
					+ '<td class="ta-center"><input type="checkbox" name="" class="js-check-all-btn" /></td>'
					+ '<th>定期購入ID</th>'
					+ '<th>氏名</th>'
					+ '<th>定期購入区分</th>'
					+ '<th>購入回数<br />注文基準</th>'
					+ '<th>購入回数<br />出荷基準</th>'
					<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED)
		{ %>
					+ '<th>頒布会注文回数</th>'
					<% } %>
					+ '<th>決済方法</th>'
					+ '<th>定期購入<br />ステータス</th>'
					+ '<th>決済<br />ステータス</th>'
					+ '<th>管理<br />メモ</th>'
					+ '<th>注文区分</th>'
					+ '<th>次回配送日</th>'
					+ '<th>次々回配送日</th>'
					+ '<th>定期購入開始日時</th>'
					+ '<th>最終購入日</th>'
					+ '</tr>'
			}
			$('#tHeader').html(header);
		}

		// Bind line data
		function bindLineData(data) {
			$('#tHeader').empty();
			$('#tData').empty();
			if (data.HasValue == false) {
				$('#tData').append('<tr><td style="font-size:16px;font-weight:bold;background:#ECECEC;padding:50px 0;line-height:1;border:none;text-align:center">該当データが存在しません。</td></tr>');
				return;
			}

			if (data.IsOver100) {
				$('#tData').append('<tr><td style="text-align:center">該当データの総数が'+ data.TotalCase +'件あります。<br />表示可能件数100を超えましたので条件を絞込み、再度検索してください。</td></tr>');
				return;
			}

			setLineTableHeader();
			var appendData = "";
			for (var i = 0; i < data.Orders.length; i++) {
				var order = data.Orders[i];

				if (workflowList[currentWorkflowIndex].workflowType == '<%= Constants.FLG_ORDERWORKFLOWSETTING_TARGET_WORKFLOW_TYPE_ORDER %>') {
					var orderMemo = (order.OrderMemo != '') ? '*' : '';
					var managementMemo = (order.ManagementMemo != '') ? '*' : '';
					var orderShippingMemo = (order.ShippingMemo != '') ? '*' : '';

					appendData += '<tr>'
						+ '<td class="ta-center"><input type="checkbox" name="" value="'+ data.Orders[i].OrderId +'" class="js-check-all-input" /></td>'
						+ '<td><a href="javascript:openOrderDetails(\'' + data.Orders[i].OrderId + '\');">' + data.Orders[i].OrderId + '</a></td>';
					if (this.workflowDispSetting) {
						for (var index = 0; index < this.workflowDispSetting.length; index++) {
							var setting = workflowDispSetting[index];
						
							switch(setting.DispColmunName) {
								case '<%= Constants.FIELD_ORDER_CARD_TRAN_ID %>':
									appendData += '<td>' + order.CardTranId + '</td>';
									break;

								case '<%= Constants.FIELD_ORDER_DIGITAL_CONTENTS_FLG %>':
									appendData += '<td>' + order.DigitalContentsFlg + '</td>';
									break;

								case '<%= Constants.FIELD_ORDER_GIFT_FLG %>':
									<% if (Constants.GIFTORDER_OPTION_ENABLED) { %>
									appendData += '<td>' + order.OrderGiftFlg + '</td>';
									<% } %>
									break;

								case '<%= Constants.FIELD_ORDER_MALL_ID %>':
									<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
									appendData += '<td>' + order.MallId + '</td>';
									<% } %>
									break;

								case '<%= Constants.FIELD_ORDER_MANAGEMENT_MEMO %>':
									appendData += '<td>' + managementMemo + '</td>';
									break;

								case '<%= Constants.FIELD_ORDER_MEMO %>':
									appendData += '<td>' + orderMemo + '</td>';
									break;

								case '<%= Constants.FIELD_ORDER_ORDER_DATE %>':
									appendData += '<td>' + order.OrderDate + '</td>';
									break;

								case '<%= Constants.FIELD_ORDER_ORDER_KBN %>':
									appendData += '<td>' + order.OrderKbn + '</td>';
									break;

								case '<%= Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS %>':
									appendData += '<td>' + order.PaymentStatus + '</td>';
									break;

								case '<%= Constants.FIELD_ORDER_ORDER_PRICE_TOTAL %>':
									appendData += '<td>' + order.PriceTotal + '</td>';
									break;

								case '<%= Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS %>':
									appendData += '<td>' + order.RepaymentStatus + '</td>';
									break;

								case '<%= Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_RECEIPT_DATE %>':
									appendData += '<td>' + order.ReturnExchangeReceiptDate + '</td>';
									break;

								case '<%= Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS %>':
									appendData += '<td>' + order.ReturnExchangeStatus + '</td>';
									break;

								case '<%= Constants.FIELD_ORDER_ORDER_SHIPPED_STATUS %>':
									<% if (Constants.REALSTOCK_OPTION_ENABLED) { %>
									appendData += '<td>' + order.ShippedStatus + '</td>';
									<% } %>
									break;

								case '<%= Constants.FIELD_ORDER_ORDER_STATUS %>':
									appendData += '<td>' + order.OrderStatus + '</td>';
									break;

								case '<%= Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS %>':
									<% if (Constants.REALSTOCK_OPTION_ENABLED) { %>
									appendData += '<td>' + order.StockreservedStatus + '</td>';
									<% } %>
									break;

								case '<%= Constants.FIELD_ORDEROWNER_OWNER_KBN %>':
									appendData += '<td>' + order.OwnerKbn + '</td>';
									break;

								case '<%= Constants.FIELD_ORDEROWNER_OWNER_NAME %>':
									appendData += '<td>' + order.OwnerName + '</td>';
									break;

								case '<%= Constants.FIELD_PAYMENT_PAYMENT_NAME %>':
									appendData += '<td>' + order.PaymentName + '</td>';
									break;

								case '<%= Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN %>':
									appendData += '<td>' + order.ReturnExchangeKbn + '</td>';
									break;

								case '<%= Constants.FIELD_ORDER_USER_ID %>':
									appendData += '<td>' + order.OwnerId + '</td>';
									break;

								case '<%= Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID %>':
									appendData += '<td>' + order.ManagementLevel + '</td>';
									break;

								// 出荷予定日
								case '<%= Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE %>':
									appendData += '<td>' + order.ScheduledShippingDate + '</td>';
									break;

									// 配送希望日
								case '<%= Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE %>':
									appendData += '<td>' + order.ShippingDate + '</td>';
									break;

									// 配送希望時間帯
								case '<%= Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME %>':
									appendData += '<td>' + order.ShippingTime + '</td>';
									break;

									// 定期購入回数(注文時点)
								case '<%= Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT %>':
									appendData += '<td>' + order.FixedPurchaseOrderCount + '</td>';
									break;

									// 定期購入回数(出荷時点)
								case '<%= Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT %>':
									appendData += '<td>' + order.FixedPurchaseShippedCount + '</td>';
									break;

									// ユーザー購入回数（注文基準）
								case '<%= Constants.FIELD_ORDER_ORDER_COUNT_ORDER %>':
									appendData += '<td>' + order.OrderCountOrder + '</td>';
									break;

									// 督促ステータス
								case '<%= Constants.FIELD_ORDER_DEMAND_STATUS %>':
									appendData += '<td>' + order.DemandStatus + '</td>';
									break;

									
									// 配送メモ
								case '<%= Constants.FIELD_ORDER_SHIPPING_MEMO %>':
									appendData += '<td>' + orderShippingMemo + '</td>';
									break;
							}
						}
					}
					appendData += '</tr>';
				}

				if (workflowList[currentWorkflowIndex].workflowType == '<%= Constants.FLG_ORDERWORKFLOWSETTING_TARGET_WORKFLOW_TYPE_FIXED_PURCHASE %>') {
					var managementMemo = (data.Orders[i].ManagementMemo != '') ? '○' : '-';
					appendData += '<tr>'
						+ '<td class="ta-center"><input type="checkbox" name="" value="'+ data.Orders[i].FixedPurchaseId +'" class="js-check-all-input" /></td>'
						+ '<td><a href="javascript:openFixedPurchaseDetails(\'' + data.Orders[i].FixedPurchaseId + '\');">' + data.Orders[i].FixedPurchaseId + '</a></td>'
						+ '<td>' + data.Orders[i].OwnerName + '</td>'
						+ '<td>' + data.Orders[i].FixedPurchaseSetting + '</td>'
						+ '<td>' + data.Orders[i].FixedPurchaseOrderCount + ' 回</td>'
						+ '<td>' + data.Orders[i].FixedPurchaseShippingCount + ' 回</td>'
						<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED) { %>
						+ '<td>' + data.Orders[i].SubscriptionBoxOrderCount + ' 回</td>'
						<% } %>
						+ '<td>' + data.Orders[i].PaymentName + '</td>'
						+ '<td>' + data.Orders[i].FixedPurchaseStatus + '</td>'
						+ '<td>' + data.Orders[i].PaymentStatus + '</td>'
						+ '<td>' + managementMemo + '</td>'
						+ '<td>' + data.Orders[i].OwnerKbn + '</td>'
						+ '<td>' + data.Orders[i].FixedPurchaseNextShippingDate + '</td>'
						+ '<td>' + data.Orders[i].FixedPurchaseNextNextShippingDate + '</td>'
						+ '<td>' + data.Orders[i].FixedPurchaseDateBgn + '</td>'
						+ '<td>' + data.Orders[i].FixedPurchaseLastOrderDate + '</td>'
						+ '</tr>'
				}
			}
			$('#tData').append(appendData);
		}

		// Set cassette order
		function setCassetteOrder(data) {
			var appendData = '';
			if (workflowList[currentWorkflowIndex].workflowType == '<%= Constants.FLG_ORDERWORKFLOWSETTING_TARGET_WORKFLOW_TYPE_ORDER %>') {
				appendData
					+= '<dl class="order-workflow-list-block-data-cassette-list-item-order-item">'
						+ '<dt class="order-workflow-list-block-data-cassette-list-item-order-item-label">注文ID</dt>'
						+ '<dd class="order-workflow-list-block-data-cassette-list-item-order-item-value"><a href="javascript:openOrderDetails(\'' + data.OrderId + '\');">' + data.OrderId + '</a></dd>'
					+ '</dl>'
					+ '<dl class="order-workflow-list-block-data-cassette-list-item-order-item">'
						+ '<dt class="order-workflow-list-block-data-cassette-list-item-order-item-label">サイト</dt>'
						+ '<dd class="order-workflow-list-block-data-cassette-list-item-order-item-value">' + data.MallId + '</dd>'
					+ '</dl>'
					+ '<dl class="order-workflow-list-block-data-cassette-list-item-order-item">'
						+ '<dt class="order-workflow-list-block-data-cassette-list-item-order-item-label">注文区分</dt>'
						+ '<dd class="order-workflow-list-block-data-cassette-list-item-order-item-value">' + data.OrderKbn + '</dd>'
					+ '</dl>'
					+ '<dl class="order-workflow-list-block-data-cassette-list-item-order-item">'
						+ '<dt class="order-workflow-list-block-data-cassette-list-item-order-item-label">決済種別</dt>'
						+ '<dd class="order-workflow-list-block-data-cassette-list-item-order-item-value">' + data.PaymentName + '</dd>'
					+ '</dl>'
					+ '<dl class="order-workflow-list-block-data-cassette-list-item-order-item">'
						+ '<dt class="order-workflow-list-block-data-cassette-list-item-order-item-label">注文日時</dt>'
						+ '<dd class="order-workflow-list-block-data-cassette-list-item-order-item-value">' + data.OrderDate + '</dd>'
					+ '</dl>'
					+ '<dl class="order-workflow-list-block-data-cassette-list-item-order-item">'
						+ '<dt class="order-workflow-list-block-data-cassette-list-item-order-item-label">合計⾦額</dt>'
						+ '<dd class="order-workflow-list-block-data-cassette-list-item-order-item-value">' + data.PriceTotal + '</dd>'
					+ '</dl>'
					+ '<dl class="order-workflow-list-block-data-cassette-list-item-order-item">'
						+ '<dt class="order-workflow-list-block-data-cassette-list-item-order-item-label">注⽂ステータス</dt>'
						+ '<dd class="order-workflow-list-block-data-cassette-list-item-order-item-value">' + data.OrderStatus + '</dd>'
					+ '</dl>'
					+ '<dl class="order-workflow-list-block-data-cassette-list-item-order-item">'
						+ '<dt class="order-workflow-list-block-data-cassette-list-item-order-item-label">⼊⾦ステータス</dt>'
						+ '<dd class="order-workflow-list-block-data-cassette-list-item-order-item-value">' + data.PaymentStatus + '</dd>'
					+ '</dl>'
					+ '<dl class="order-workflow-list-block-data-cassette-list-item-order-item">'
						+ '<dt class="order-workflow-list-block-data-cassette-list-item-order-item-label">決済注文ID</dt>'
						+ '<dd class="order-workflow-list-block-data-cassette-list-item-order-item-value">' + data.CardTranId + '</dd>'
					+ '</dl>'
			} else {
				appendData
					+= '<dl class="order-workflow-list-block-data-cassette-list-item-order-item">'
						+ '<dt class="order-workflow-list-block-data-cassette-list-item-order-item-label">定期購入ID</dt>'
						+ '<dd class="order-workflow-list-block-data-cassette-list-item-order-item-value"><a href="javascript:openFixedPurchaseDetails(\'' + data.FixedPurchaseId + '\');">' + data.FixedPurchaseId + '</a></dd>'
					+ '</dl>'
					+ '<dl class="order-workflow-list-block-data-cassette-list-item-order-item">'
						+ '<dt class="order-workflow-list-block-data-cassette-list-item-order-item-label">定期購入区分</dt>'
						+ '<dd class="order-workflow-list-block-data-cassette-list-item-order-item-value">' + data.FixedPurchaseKbn + '</dd>'
					+ '</dl>'
					+ '<dl class="order-workflow-list-block-data-cassette-list-item-order-item">'
						+ '<dt class="order-workflow-list-block-data-cassette-list-item-order-item-label">決済手段</dt>'
						+ '<dd class="order-workflow-list-block-data-cassette-list-item-order-item-value">' + data.PaymentName + '</dd>'
					+ '</dl>'
					+ '<dl class="order-workflow-list-block-data-cassette-list-item-order-item">'
						+ '<dt class="order-workflow-list-block-data-cassette-list-item-order-item-label">定期購入開始日日時</dt>'
						+ '<dd class="order-workflow-list-block-data-cassette-list-item-order-item-value">' + data.FixedPurchaseDate + '</dd>'
					+ '</dl>'
			}
			return appendData;
		}

		// Set cassette shipping
		function setCassetteShipping(data) {
			var appendData = '';
			if (workflowList[currentWorkflowIndex].workflowType == '<%= Constants.FLG_ORDERWORKFLOWSETTING_TARGET_WORKFLOW_TYPE_ORDER %>') {
				appendData
				+= '<dl class="order-workflow-list-block-data-cassette-list-item-order-info">'
					+ '<dt class="order-workflow-list-block-data-cassette-list-item-order-info-label">配送先</dt>'
					+ '<dd class="order-workflow-list-block-data-cassette-list-item-order-info-value">'
						+ '<div class="order-workflow-list-block-data-cassette-list-item-order-info-value-col1">'
							+ '<span class="order-workflow-list-block-data-cassette-list-item-order-info-dest-address">' + data.ShippingAddress + '</span>'
						+ '</div>'
						+ '<div class="order-workflow-list-block-data-cassette-list-item-order-info-value-col2">'
							+ '<span class="order-workflow-list-block-data-cassette-list-item-order-info-dest-name">' + data.ShippingName + '</span>'
							+ '<span class="order-workflow-list-block-data-cassette-list-item-order-info-dest-tel">(TEL：' + data.ShippingTel + ')</span>'
						+ '</div>'
					+ '</dd>'
				+ '</dl>'
				<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
				+ '<dl class="order-workflow-list-block-data-cassette-list-item-order-info">'
					+ '<dt class="order-workflow-list-block-data-cassette-list-item-order-info-label">企業名</dt>'
					+ '<dd class="order-workflow-list-block-data-cassette-list-item-order-info-value">'
						+ '<div class="order-workflow-list-block-data-cassette-list-item-order-info-value-col1">'
						+ '<span class="order-workflow-list-block-data-cassette-list-item-order-info-dest-address">' + data.ShippingCompanyName + '</span>'
						+ '</div>'
					+ '</dd>'
				+ '</dl>'
				+ '<dl class="order-workflow-list-block-data-cassette-list-item-order-info">'
					+ '<dt class="order-workflow-list-block-data-cassette-list-item-order-info-label">部署名</dt>'
					+ '<dd class="order-workflow-list-block-data-cassette-list-item-order-info-value">'
						+ '<div class="order-workflow-list-block-data-cassette-list-item-order-info-value-col1">'
							+ '<span class="order-workflow-list-block-data-cassette-list-item-order-info-dest-address">' + data.ShippingCompanyPostName + '</span>'
						+ '</div>'
					+ '</dd>'
				+ '</dl>'
				<% } %>
				+ '<dl class="order-workflow-list-block-data-cassette-list-item-order-info">'
					+ '<dt class="order-workflow-list-block-data-cassette-list-item-order-info-label">配送日時</dt>'
					+ '<dd class="order-workflow-list-block-data-cassette-list-item-order-info-value">'
						+ '<div class="order-workflow-list-block-data-cassette-list-item-order-info-value-col1">'
							+ '<div class="order-workflow-list-block-data-cassette-list-item-order-info-delivery-wrapper">'
								+ '<dl class="order-workflow-list-block-data-cassette-list-item-order-info-delivery-date">'
									+ '<dt class="order-workflow-list-block-data-cassette-list-item-order-info-delivery-date-label">希望日</dt>'
									+ '<dd class="order-workflow-list-block-data-cassette-list-item-order-info-delivery-date-value">' + data.ShippingDate + '</dd>'
								+ '</dl>'
								+ '<dl class="order-workflow-list-block-data-cassette-list-item-order-info-delivery-time">'
									+ '<dt class="order-workflow-list-block-data-cassette-list-item-order-info-delivery-time-label">希望時間帯</dt>'
									+ '<dd class="order-workflow-list-block-data-cassette-list-item-order-info-delivery-time-value">' + data.ShippingTime + '</dd>'
								+ '</dl>'
							+ '</div>'
						+ '</div>'
					+ '</dd>'
				+ '</dl>'
			} else {
				appendData
				+= '<dl class="order-workflow-list-block-data-cassette-list-item-order-info">'
					+ '<dt class="order-workflow-list-block-data-cassette-list-item-order-info-label" style="width: 15%">次回以降配送日</dt>'
					+ '<dd class="order-workflow-list-block-data-cassette-list-item-order-info-value">'
						+ '<div class="order-workflow-list-block-data-cassette-list-item-order-info-value-col1">'
							+ '<div class="order-workflow-list-block-data-cassette-list-item-order-info-delivery-wrapper">'
								+ '<dl class="order-workflow-list-block-data-cassette-list-item-order-info-delivery-date">'
									+ '<dt class="order-workflow-list-block-data-cassette-list-item-order-info-delivery-date-label">次回配送日</dt>'
									+ '<dd class="order-workflow-list-block-data-cassette-list-item-order-info-delivery-date-value">' + data.FixedPurchaseNextShippingDate + '</dd>'
								+ '</dl>'
								+ '<dl class="order-workflow-list-block-data-cassette-list-item-order-info-delivery-time">'
									+ '<dt class="order-workflow-list-block-data-cassette-list-item-order-info-delivery-time-label">次々回配送日</dt>'
									+ '<dd class="order-workflow-list-block-data-cassette-list-item-order-info-delivery-time-value">' + data.FixedPurchaseNextNextShippingDate + '</dd>'
								+ '</dl>'
							+ '</div>'
						+ '</div>'
					+ '</dd>'
				+ '</dl>'
			}
			return appendData;
		}

		// Set cassette memo
		function setCassetteMemo(data) {
			var appendData = '';
			if (workflowList[currentWorkflowIndex].workflowType == '<%= Constants.FLG_ORDERWORKFLOWSETTING_TARGET_WORKFLOW_TYPE_ORDER %>') {
				appendData
				+= '<dl class="order-workflow-list-block-data-cassette-list-item-order-info">'
					+ '<dt class="order-workflow-list-block-data-cassette-list-item-order-info-label">注文メモ</dt>'
					+ '<dd class="order-workflow-list-block-data-cassette-list-item-order-info-value">'
						+ data.OrderMemo
					+ '</dd>'
				+ '</dl>'
				+ '<dl class="order-workflow-list-block-data-cassette-list-item-order-info">'
					+ '<dt class="order-workflow-list-block-data-cassette-list-item-order-info-label">管理メモ</dt>'
					+ '<dd class="order-workflow-list-block-data-cassette-list-item-order-info-value">'
						+ data.ManagementMemo
					+ '</dd>'
				+ '</dl>'
				+ '<dl class="order-workflow-list-block-data-cassette-list-item-order-info">'
					+ '<dt class="order-workflow-list-block-data-cassette-list-item-order-info-label">配送メモ</dt>'
					+ '<dd class="order-workflow-list-block-data-cassette-list-item-order-info-value">'
						+ data.ShippingMemo
					+ '</dd>'
				+ '</dl>'
			} else {
				appendData
				+= '<dl class="order-workflow-list-block-data-cassette-list-item-order-info">'
					+ '<dt class="order-workflow-list-block-data-cassette-list-item-order-info-label">管理メモ</dt>'
					+ '<dd class="order-workflow-list-block-data-cassette-list-item-order-info-value">'
						+ data.ManagementMemo
					+ '</dd>'
				+ '</dl>'
				+ '<dl class="order-workflow-list-block-data-cassette-list-item-order-info">'
					+ '<dt class="order-workflow-list-block-data-cassette-list-item-order-info-label">配送メモ</dt>'
					+ '<dd class="order-workflow-list-block-data-cassette-list-item-order-info-value">'
						+ data.ShippingMemo
					+ '</dd>'
				+ '</dl>'
			}
			return appendData;
		}

		// Bind cassette data
		function bindCassetteData(data) {
			$('#ulData').empty();
			if (data.HasValue == false) {
				$('#ulData').append('<div style="font-size:16px;font-weight:bold;background:#ECECEC;padding:50px 0;line-height:1;border:none;text-align:center">該当データが存在しません。</div>');
				return;
			}

			$('#divBottomPager').html($('#divTopPager').html());
			var appendData = "";
			for (var index = 0; index < data.Orders.length; index++) {
				var appendItemData = "";
				for (var itemIndex = 0; itemIndex < data.Orders[index].OrderItems.length; itemIndex++) {
					appendItemData
					+= '<div class="order-workflow-list-block-data-cassette-list-item-order-info-item-block">'
					+ '<div class="order-workflow-list-block-data-cassette-list-item-order-info-value-col1">'
					+ '<a href="javascript:openProductDetails(\'' + data.Orders[index].OrderItems[itemIndex].ProductId + '\');" class="order-workflow-list-block-data-cassette-list-item-order-info-item-id">' + data.Orders[index].OrderItems[itemIndex].ProductId + '</a>'
					+ '<span class="order-workflow-list-block-data-cassette-list-item-order-info-item-name">' + data.Orders[index].OrderItems[itemIndex].ProductName + '</span>'
					+ '</div>'
					+ '<div class="order-workflow-list-block-data-cassette-list-item-order-info-value-col2">'
					+ '<span class="order-workflow-list-block-data-cassette-list-item-order-info-item-price">' + data.Orders[index].OrderItems[itemIndex].ProductPrice + '</span>'
					+ '<span class="order-workflow-list-block-data-cassette-list-item-order-info-item-tax-label">(税込)</span>'
					+ '<span class="order-workflow-list-block-data-cassette-list-item-order-info-item-num">' + data.Orders[index].OrderItems[itemIndex].ItemQuantity + '個</span>'
					+ '<span class="order-workflow-list-block-data-cassette-list-item-order-info-item-subtotal">' + data.Orders[index].OrderItems[itemIndex].ItemPrice + '</span>'
					+ '<span class="order-workflow-list-block-data-cassette-list-item-order-info-item-tax-label">(税込)</span>'
					+ '</div>'
					+ '</div>'
				}
				var actionData = "";
				var id = (workflowList[currentWorkflowIndex].workflowType == '<%= Constants.FLG_ORDERWORKFLOWSETTING_TARGET_WORKFLOW_TYPE_ORDER %>')
					? data.Orders[index].OrderId
					: data.Orders[index].FixedPurchaseId;
				if (data.Actions.length > 0) {
					var defaultAction = (data.Actions[0].key.split('&')[0] == '<%= Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_DEFAULT_SELECT %>')
						? data.Actions[0].key.split('&')[2]
						: "";
					for (var actionIndex = (defaultAction === "") ? 0 : 1; actionIndex < data.Actions.length; actionIndex++) {
						var isChecked = "";
						var cassetteNoUpdateClass = "";
						if ((data.Actions[actionIndex].key.split('&')[1] == defaultAction)
							|| ((data.Actions[actionIndex].key.split('&')[0] == '<%= Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_NO_UPDATE %>')
							&& (defaultAction == ""))) {
							isChecked = "checked";
						}
						if (data.Actions[actionIndex].key.split('&')[0] == '<%= Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_NO_UPDATE %>') {
							cassetteNoUpdateClass = "js-select-no-process";
						}
						actionData
						+= '<li class="order-workflow-list-block-data-cassette-list-item-process-list-radio">'
							+ '<input type="radio" title="" value="'+ data.Actions[actionIndex].key +'" data-value="'+ id +'" name="'+ index +'" id="action'+ index + (actionIndex) +'" class="'+ cassetteNoUpdateClass +'" '+ isChecked +' /><label for="action' + index + (actionIndex) + '">'+ data.Actions[actionIndex].value +'</label>'
						+ '</li>'
					}
				}
				appendData
				+= '<li class="order-workflow-list-block-data-cassette-list-item">'
					+ '<div class="order-workflow-list-block-data-cassette-list-item-index">' + (index + 1) + '</div>'
					+ '<div class="order-workflow-list-block-data-cassette-list-item-order" style="width: -webkit-fill-available;">'
						+ '<div class="order-workflow-list-block-data-cassette-list-item-order-group">'
							+ '<div class="order-workflow-list-block-data-cassette-list-item-order-header">'
								+ setCassetteOrder(data.Orders[index])
							+ '</div>'
							+ '<dl class="order-workflow-list-block-data-cassette-list-item-order-info">'
								+ '<dt class="order-workflow-list-block-data-cassette-list-item-order-info-label">注文者</dt>'
								+ '<dd class="order-workflow-list-block-data-cassette-list-item-order-info-value">'
									+ '<div class="order-workflow-list-block-data-cassette-list-item-order-info-value-col1">'
										+ '<a href="javascript:openUserDetails(\'' + data.Orders[index].OwnerId + '\');" class="order-workflow-list-block-data-cassette-list-item-order-info-buyer-id">' + data.Orders[index].OwnerId + '</a>'
										+ '<span class="order-workflow-list-block-data-cassette-list-item-order-info-buyer-name">' + data.Orders[index].OwnerName + '</span>'
									+ '</div>'
									+ '<div class="order-workflow-list-block-data-cassette-list-item-order-info-value-col2">'
										+ '<span class="order-workflow-list-block-data-cassette-list-item-order-info-buyer-tel">TEL：' + data.Orders[index].OwnerTel + '</span>'
										+ '<span class="order-workflow-list-block-data-cassette-list-item-order-info-buyer-email">E-mail：' + data.Orders[index].OwnerMail + '</span>'
									+ '</div>'
								+ '</dd>'
							+ '</dl>'
							+ setCassetteShipping(data.Orders[index])
							+ '<dl class="order-workflow-list-block-data-cassette-list-item-order-info">'
								+ '<dt class="order-workflow-list-block-data-cassette-list-item-order-info-label">商品明細</dt>'
								+ '<dd class="order-workflow-list-block-data-cassette-list-item-order-info-value">'
									+ appendItemData
								+ '</dd>'
							+ '</dl>'
						+ '</div>'
						+ '<div class="order-workflow-list-block-data-cassette-list-item-order-memo-group">'
							+ setCassetteMemo(data.Orders[index])
						+ '</div>'
					+ '</div>'
					+ '<div class="order-workflow-list-block-data-cassette-list-item-process">'
						+ '<div class="order-workflow-list-block-data-cassette-list-item-process-title">処理内容の選択</div>'
						+ '<ul class="order-workflow-list-block-data-cassette-list-item-process-list">'
							+ actionData
						+ '</ul>'
					+ '</div>'
				+ '</li>'
			}
			$('#ulData').append(appendData);
		}

		// Bind top pager data
		function bindTopPagerData(data, currentPage) {
			$('.list_pager-slide').empty();
			var lbNext = '';
			var lbBack = '';

			if ((currentPage > 1) && (data.TotalPage != 0)) {
				var previousPage = currentPage - 1;
				lbBack += '<a id="lbPageBack" onmouseover="hoverBack(\'over\')" onmouseout="hoverBack(\'out\')" href="javascript:void(0);" onclick="getWorkflowExecData('+ previousPage +')">'
					+'<img src="../../Images/Common/paging_back_01.gif" alt="Back" border="0" id="PagerBackButton">'
					+'</a>'
			} else {
				lbBack = '<img src="../../Images/Common/paging_back_01.gif" alt="Back" border="0" id="PagerBackButton">';
			}
			$('.list_pager-slide').append(lbBack);

			if ((currentPage != data.TotalPage) && (data.TotalPage != 0)) {
				var nextPage = currentPage + 1;
				lbNext += '<a id="lbPageNext" onmouseover="hoverNext(\'over\')" onmouseout="hoverNext(\'out\')" href="javascript:void(0);" onclick="getWorkflowExecData('+ nextPage +')">'
					+'<img src="../../Images/Common/paging_next_01.gif" alt="Next" border="0" id="PagerNextButton">'
					+'</a>'
			} else {
				lbNext = '<img src="../../Images/Common/paging_next_01.gif" alt="Next" border="0" name="PagerNextButton">';
			}
			$('.list_pager-slide').append(lbNext);

			$('#tdPageList').html("&nbsp;");
			$('#tdTotalCase').html('該当件数&nbsp;' + data.TotalCase + '件');
			var pagerData = "";
			for (var i = 0; i < data.Pages.length; i++) {
				if (data.Pages[i] != "") {
					var functionName = "getWorkflowExecData(" + data.Pages[i] + ");";
					var isDisable = "";
					if ((currentPage == data.Pages[i]) || (data.Pages[i] == "...")) {
						isDisable = 'style="pointer-events: none; text-decoration: none; font-weight: bold; color:black"'
					}
					pagerData += '<a id="test"' + isDisable + ' href="javascript:void(0);" onclick="' + functionName + '">' + data.Pages[i] + '</a>';
					if (((i + 1) != data.Pages.length) && (data.Pages.length > 1)) {
						pagerData += "  |  ";
					}
				}
			}
			$('#tdPageList').html(pagerData);
			$('#divBottomPager').empty();
		}

		// Hover next
		function hoverNext(action) {
			if (action === "over") {
				$('#PagerNextButton').attr('src', '../../Images/Common/paging_next_02.gif');
			} else {
				$('#PagerNextButton').attr('src', '../../Images/Common/paging_next_01.gif');
			}
		}

		// Hover back
		function hoverBack(action) {
			if (action === "over") {
				$('#PagerBackButton').attr('src', '../../Images/Common/paging_back_02.gif');
			} else {
				$('#PagerBackButton').attr('src', '../../Images/Common/paging_back_01.gif');
			}
		}

		// Get next workflow
		function getNextWorkflow() {
			var nextIndex = parseInt(currentWorkflowIndex) + 1;
			$('#line' + currentWorkflowIndex).removeClass("order-workflow-list-block-list-item is-selected");
			$('#line' + currentWorkflowIndex).addClass("order-workflow-list-block-list-item");
			$('#line' + nextIndex).removeClass("order-workflow-list-block-list-item");
			$('#line' + nextIndex).addClass("order-workflow-list-block-list-item is-selected");
			workflow.step.switch(1);
			clearSearch(false);
			checkProcess(nextIndex);
		}

		// Set enable next button
		function setEnableNextBtn(index) {
			if (workflowList.length == (parseInt(index) + 1)) {
				$('.order-workflow-list-block-next-workflow-btn').css({
					'pointer-events': 'none',
					'background': '#efefef',
					'border-color': '#e2e2e2'
				});
			} else {
				$('.order-workflow-list-block-next-workflow-btn').css({
					'pointer-events': '',
					'background': '',
					'border-color': ''
				});
			}
		}

		//Open detail
		function openDetails(orderId) {
			(workflowList[currentWorkflowIndex].workflowType == '<%= WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER %>')
				? openOrderDetails(orderId)
				: openFixedPurchaseDetails(orderId);
		}

		// Open order detail
		function openOrderDetails(orderId) {
			var url = '<%= string.Format(
				"{0}{1}?{2}=",
				Constants.PATH_ROOT_EC,
				Constants.PAGE_MANAGER_ORDER_CONFIRM,
				Constants.REQUEST_KEY_ORDER_ID) %>';
			url += orderId
				+= '<%= string.Format(
					"&{0}={1}&{2}={3}",
					Constants.REQUEST_KEY_ACTION_STATUS,
					HttpUtility.UrlEncode(Constants.ACTION_STATUS_DETAIL),
					Constants.REQUEST_KEY_WINDOW_KBN,
					HttpUtility.UrlEncode(Constants.KBN_WINDOW_POPUP)) %>';
			window.open(url, 'ordercontact', 'width=1200,height=800,top=120,left=420,status=no,scrollbars=yes');
		}

		// Open fixed purchase detail
		function openFixedPurchaseDetails(fixedPurchaseId) {
			var url = '<%= string.Format(
				"{0}{1}?{2}=",
				Constants.PATH_ROOT_EC,
				Constants.PAGE_MANAGER_FIXEDPURCHASE_CONFIRM,
				Constants.REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_ID) %>';
			url += fixedPurchaseId
				+= '<%= string.Format(
					"&{0}={1}&{2}={3}",
					Constants.REQUEST_KEY_ACTION_STATUS,
					HttpUtility.UrlEncode(Constants.ACTION_STATUS_DETAIL),
					Constants.REQUEST_KEY_WINDOW_KBN,
					HttpUtility.UrlEncode(Constants.KBN_WINDOW_POPUP)) %>';
			window.open(url, 'fixedpurchasecontact', 'width=1200,height=800,top=120,left=420,status=no,scrollbars=yes');
		}

		// Open user detail
		function openUserDetails(userId) {
			var url = '<%= string.Format(
				"{0}{1}?{2}=",
				Constants.PATH_ROOT_EC,
				Constants.PAGE_MANAGER_USER_CONFIRM,
				Constants.REQUEST_KEY_USER_ID) %>';
			url += userId
				+= '<%= string.Format(
					"&{0}={1}&{2}={3}",
					Constants.REQUEST_KEY_ACTION_STATUS,
					HttpUtility.UrlEncode(Constants.ACTION_STATUS_DETAIL),
					Constants.REQUEST_KEY_WINDOW_KBN,
					HttpUtility.UrlEncode(Constants.KBN_WINDOW_POPUP)) %>';
			window.open(url, 'usercontact', 'width=1200,height=800,top=120,left=420,status=no,scrollbars=yes');
		}

		// Open product detail
		function openProductDetails(productId) {
			var url = '<%= string.Format(
				"{0}{1}?{2}=",
				Constants.PATH_ROOT_EC,
				Constants.PAGE_MANAGER_PRODUCT_CONFIRM,
				Constants.REQUEST_KEY_PRODUCT_ID) %>';
			url += productId
				+= '<%= string.Format(
					"&{0}={1}&{2}={3}",
					Constants.REQUEST_KEY_ACTION_STATUS,
					HttpUtility.UrlEncode(Constants.ACTION_STATUS_DETAIL),
					Constants.REQUEST_KEY_WINDOW_KBN,
					HttpUtility.UrlEncode(Constants.KBN_WINDOW_POPUP)) %>';
			window.open(url, 'productcontact', 'width=1200,height=800,top=120,left=420,status=no,scrollbars=yes');
		}

		// Exec workflow
		function execWorkflow(execType) {
			modal.close();
			var sendData = {
				CurrentPage: currentPage,
				WorkflowNo: workflowList[currentWorkflowIndex].workflowNo,
				WorkflowKbn: workflowList[currentWorkflowIndex].workflowKbn,
				WorkflowType: workflowList[currentWorkflowIndex].workflowType,
				SearchCondition: urlQueryString,
				Orders: [],
				ExecType : execType
			};

			if (execType == 'select') {
				sendData.ExtendStatusDate = ($('#dpSelectExtendStatusUpdate').is(':visible'))
					? $('#dpSelectExtendStatusUpdate').val()
					: '';
				sendData.ShippingDateUpdate = ($('#dpSelectShippingDateUpdate').is(':visible'))
					? $('#dpSelectShippingDateUpdate').val()
					: '';
				sendData.ScheduledShippingDateUpdate = ($('#dpSelectScheduledShippingDateUpdate').is(':visible'))
					? $('#dpSelectScheduledShippingDateUpdate').val()
					: '';
				sendData.NextShippingDateUpdate = ($('#dpSelectNextShippingDateUpdate').is(':visible'))
					? $('#dpSelectNextShippingDateUpdate').val()
					: '';
				sendData.NextNextShippingDateUpdate = ($('#dpSelectNextNextShippingDateUpdate').is(':visible'))
					? $('#dpSelectNextNextShippingDateUpdate').val()
					: '';

				// select
				if (workflowList[currentWorkflowIndex].displayKbn == '<%= Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_LINE %>') {
					$.each($('#tData').find('input[type="checkbox"]:checked'), function(key, input) {
						var order = {
							Index: input.getAttribute("data-row-index"),
							OrderId: input.getAttribute("value"),
							CassetteAction: ""
						}
						sendData.Orders.push(order);
					});
				} else {
					$.each($('#ulData').find('input[type="radio"]:checked').not('.js-select-no-process'), function(key, input) {
						var order = {
							Index: input.getAttribute("name"),
							OrderId: input.getAttribute("data-value"),
							CassetteAction: input.getAttribute("value")
						}
						sendData.Orders.push(order);
					});
				}
			} else {
				sendData.ExtendStatusDate = ($('#dpAllExtendStatusUpdate').is(':visible'))
					? $('#dpAllExtendStatusUpdate').val()
					: '';
				sendData.ShippingDateUpdate = ($('#dpAllShippingDateUpdate').is(':visible'))
					? $('#dpAllShippingDateUpdate').val()
					: '';
				sendData.ScheduledShippingDateUpdate = ($('#dpAllScheduledShippingDateUpdate').is(':visible'))
					? $('#dpAllScheduledShippingDateUpdate').val()
					: '';
				sendData.NextShippingDateUpdate = ($('#dpAllNextShippingDateUpdate').is(':visible'))
					? $('#dpAllNextShippingDateUpdate').val()
					: '';
				sendData.NextNextShippingDateUpdate = ($('#dpAllNextNextShippingDateUpdate').is(':visible'))
					? $('#dpAllNextNextShippingDateUpdate').val()
					: '';
			}

			$('#errorMessage').html("");
			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/ValidateExecData";
			var dataRequest = "{ data:" + JSON.stringify(sendData) + "}";
			var request = callAjax(urlRequest, dataRequest);
			request.done(function(response) {
				var message = response.d;
				if (message != "") {
					$('#errorMessage').html(message);
					return;
				} else {
					workflow.step.switch(3);

					$('#processInfo').hide();
					showLoading(3);
					urlRequest = "<%: this.OrderWorkflowBaseUrl %>/ExecWorkflow";
					dataRequest = "{ data:" + JSON.stringify(sendData) + "}";
					request = callAjax(urlRequest, dataRequest);

					request.done(function(response) {
						var data = JSON.parse(response.d);
						$('li.order-workflow-list-block-result-num-item.is-remaining').show();
						$('#divBottom').show();

						if (data.IsAsync == false) {
							displayResult(data, execType);
							setTimeout(function() {
								hideLoading(3)
							}, 1000);
						} else {
							if (execType == 'select') {
								sendData.ExtendStatusDate = ($('#dpSelectExtendStatusUpdate').is(':visible')
									&& (Date.parse($('#dpSelectExtendStatusUpdate').val())))
										? $('#dpSelectExtendStatusUpdate').val()
										: '';
								sendData.ShippingDateUpdate = ($('#dpSelectShippingDateUpdate').is(':visible')
									&& (Date.parse($('#dpSelectShippingDateUpdate').val())))
										? $('#dpSelectShippingDateUpdate').val()
										: '';
								sendData.ScheduledShippingDateUpdate = ($('#dpSelectScheduledShippingDateUpdate').is(':visible')
									&& (Date.parse($('#dpSelectScheduledShippingDateUpdate').val())))
										? $('#dpSelectScheduledShippingDateUpdate').val()
										: '';
								sendData.NextShippingDateUpdate = ($('#dpSelectNextShippingDateUpdate').is(':visible')
									&& (Date.parse($('#dpSelectNextShippingDateUpdate').val())))
										? $('#dpSelectNextShippingDateUpdate').val()
										: '';
								sendData.NextNextShippingDateUpdate = ($('#dpSelectNextNextShippingDateUpdate').is(':visible')
									&& (Date.parse($('#dpSelectNextNextShippingDateUpdate').val())))
										? $('#dpSelectNextNextShippingDateUpdate').val()
										: '';

								// select
								if (workflowList[currentWorkflowIndex].displayKbn == '<%= Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_LINE %>') {
									$.each($('#tData').find('input[type="checkbox"]:checked'), function(key, input) {
										var order = {
											Index: input.getAttribute("data-row-index"),
											OrderId: input.getAttribute("value"),
											CassetteAction: ""
										}
										sendData.Orders.push(order);
									});
								} else {
									$.each($('#ulData').find('input[type="radio"]:checked').not('.js-select-no-process'), function(key, input) {
										var order = {
											Index: input.getAttribute("name"),
											OrderId: input.getAttribute("data-value"),
											CassetteAction: input.getAttribute("value")
										}
										sendData.Orders.push(order);
									});
								}
							} else {
								sendData.ExtendStatusDate = ($('#dpAllExtendStatusUpdate').is(':visible')
									&& (Date.parse($('#dpAllExtendStatusUpdate').val())))
										? $('#dpAllExtendStatusUpdate').val()
										: '';
								sendData.ShippingDateUpdate = ($('#dpAllShippingDateUpdate').is(':visible')
									&& (Date.parse($('#dpAllShippingDateUpdate').val())))
										? $('#dpAllShippingDateUpdate').val()
										: '';
								sendData.ScheduledShippingDateUpdate = ($('#dpAllScheduledShippingDateUpdate').is(':visible')
									&& (Date.parse($('#dpAllScheduledShippingDateUpdate').val())))
										? $('#dpAllScheduledShippingDateUpdate').val()
										: '';
								sendData.NextShippingDateUpdate = ($('#dpAllNextShippingDateUpdate').is(':visible')
									&& (Date.parse($('#dpAllNextShippingDateUpdate').val())))
										? $('#dpAllNextShippingDateUpdate').val()
										: '';
								sendData.NextNextShippingDateUpdate = ($('#dpAllNextNextShippingDateUpdate').is(':visible')
									&& (Date.parse($('#dpAllNextNextShippingDateUpdate').val())))
										? $('#dpAllNextNextShippingDateUpdate').val()
										: '';
							}
							$('#processInfo').show();
							var processId = data.ProcessId;
							var interval = setInterval(function () {
								var process = callAjax("<%: this.OrderWorkflowBaseUrl %>/GetWorkflowCurrentProcess", '{ processId: "' + processId + '"}');
								process.done(function (res) {
									var processData = JSON.parse(res.d);
									var percent = processData.DoneCount / processData.TotalCount;
									$('#processInfo').html("<strong>処理中です... " + Math.round(percent * 100) + "% 完了 (" + processData.DoneCount + "/" + processData.TotalCount + ")</strong>")
									if (processData.IsSuccess) {
										clearInterval(interval);
										setTimeout(function(){
											displayResult(processData, execType);
											hideLoading(3);
										}, 1000);
									} else if (processData.IsSystemError) {
										clearInterval(interval);
										hideLoading(3);
										notification.show('<%: WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PROCESS_PROCESSING_IS_INTERRUPTED) %>', 'warning', 'fadeout');
									}
								})
							}, 500)
					}

						clearInterval(setDivCss);
					});
			}
			});
	}

	// Key press search
	function keyPressSearch() {
		if (event.keyCode == 13) search();
	}

	// Get the input data from screen and create json
	function search() {
		if (validateDateBySearch()) return;

		var shippingPrefectureParam = "";
		var shippingPrefectureKey =
			"<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_PREFECTURE %>";
			var orderCount = 0;
			var shippedCount = 0;
			var subscriptionBoxOrderCount = 0;
			var regex = new RegExp(/^[+-]?[0-9]*$/);
			if (!regex.test(document.getElementById("tbOrderCountFrom").value))
			{
				document.getElementById("tbOrderCountFrom").value = orderCount;
			}
			if (!regex.test(document.getElementById("tbOrderCountTo").value))
			{
				document.getElementById("tbOrderCountTo").value = orderCount;
			}
			if (!regex.test(document.getElementById("tbShippedCountFrom").value))
			{
				document.getElementById("tbShippedCountFrom").value = shippedCount;
			}
			if (!regex.test(document.getElementById("tbShippedCountTo").value))
			{
				document.getElementById("tbShippedCountTo").value = shippedCount;
			}
			<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED)
		{ %>
			if (!regex.test(document.getElementById("tbSubscriptionBoxOrderCountFrom").value))
			{
				document.getElementById("tbSubscriptionBoxOrderCountFrom").value = subscriptionBoxOrderCount;
			}
			if (!regex.test(document.getElementById("tbSubscriptionBoxOrderCountTo").value))
			{
				document.getElementById("tbSubscriptionBoxOrderCountTo").value = subscriptionBoxOrderCount;
			}
			<% } %>

			$.each($(".needInput"), function(index, input) {
				if (input.type != 'checkbox') {
					urlQueryString[input.name] = input.value.trim();
				} else if ((input.name == shippingPrefectureKey) && ($('#'+input.id).is(':checked'))) {
					shippingPrefectureParam += input.value + ',';
				} else if (input.value) {
					var name = input.name;
					urlQueryString[input.name] = $('#'+input.id).is(':checked');
				}
			});

			urlQueryString['<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_TEXT %>']
				= $("[name='<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_TEXT %>']:visible").val();
			urlQueryString['<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_NAME %>']
				= $("[name='<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_NAME %>']:visible").val();

			urlQueryString['<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_TEXT %>']
				= $("[name='<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_TEXT %>']:visible").val();
			urlQueryString['<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_NAME %>']
				= $("[name='<%= Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_NAME %>']:visible").val();

			urlQueryString[shippingPrefectureKey] = shippingPrefectureParam;
			<% if (Constants.STORE_PICKUP_OPTION_ENABLED) { %>
			urlQueryString['<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_STORE_PICKUP_STATUS %>'] = document.getElementById("ddlStorePickupStatus").value;
			<% } %>

			// Check max length url
			var url = getSearchUrl();
			if (url.length > parseInt('<%= Constants.ORDER_SEARCH_MAX_LENGTH_URL %>')) {
				notification.show('<%: WebMessages.GetMessages(WebMessages.ERRMSG_TOO_MANY_SEARCH_CONDITION) %>', 'warning', 'fadeout');
				return;
			}

			updateUrl();
			getWorkflowExecData(1);
		}

		// Display exec result on screen
		function displayResult(data, execType) {
			if (execType == 'select' || execType == 'all') {
				$('#divUploadResult').hide();
				$('#divResultTable').show();
				$('#spTotalCase').html(data.TotalCount);
				var okCase = 0;
				for (var index = 0; index < data.Results.Results.length; index++) {
					if (data.Results.Results[index].HasError == false) {
						okCase++;
					}
				}

				// Set display result
				$('#spSuccessCase').html(okCase);
				$('#spErrorCase').html((data.Results.Results.length - okCase).toLocaleString());
				$('#spRemainCase').html(data.RemainCount);

				// Update display order count for current workflow setting selection
				updateDisplayOrderCountForWorkflowSetting(data.WorkflowKbn, data.WorkflowNo, data.WorkflowType, data.RemainCount);

				var workflowType = workflowList[currentWorkflowIndex].workflowType;
				var header = '';
				header += '<td align="center" width="20%" rowspan="2">'+ ((workflowType == '<%= WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER %>') ? "注文ID" : "定期台帳ID") +'</td>'
					+ ((data.Results.IsDisplayUpdateOrderStatusResult) ? ('<td align="center">注文ステータス</td>') : "")
					+ ((data.Results.DisplayUpdateShippingDateStatusResult) ? ('<td align="center">配送希望日</td>') : "")
					+ ((data.Results.DisplayUpdateScheduledShippingDateStatusResult) ? ('<td align="center">出荷予定日</td>') : "")
					+ ((data.Results.IsDisplayUpdateProductRealStockResult) ? ('<td align="center">実在庫連動処理</td>') : "")
					+ ((data.Results.IsDisplayUpdatePaymentStatusResult) ? ('<td align="center">入金ステータス</td>') : "")
					+ ((data.Results.IsDisplayExecExternalPaymentActionResult) ? ('<td align="center">外部決済連携</td>') : "")
					+ ((data.Results.IsDisplayUpdateDemandStatusResult) ? ('<td align="center">督促ステータス</td>') : "")
					+ ((data.Results.IsDisplayUpdateReturnExchangeStatusResult) ? ('<td align="center">返品交換ステータス</td>') : "")
					+ ((data.Results.IsDisplayUpdateRepaymentStatusResult) ? ('<td align="center">返金ステータス</td>') : "")
					+ ((data.Results.IsDisplayUpdateFixedPurchaseIsAliveResult) ? ('<td align="center">定期購入状態変更</td>') : "")
					+ ((data.Results.IsDisplayUpdateFixedPurchasePaymentStatusResult) ? ('<td align="center">定期決ステータス</td>') : "")
					+ ((data.Results.IsDisplayUpdateNextShippingDateResult) ? ('<td align="center">次回配送日</td>') : "")
					+ ((data.Results.IsDisplayUpdateNextNextShippingDateResult) ? ('<td align="center">次々回配送日</td>') : "")

				$.each(data.Results.IsDisplayUpdateOrderExtendStatusStatementResults, function(key, value) {
					var extendName = data.DisplayExtendStatusList[(key + 1)];
					if ((extendName !== undefined) && (extendName !== '')) {
						header += (value ? ('<td align="center">拡張ステータス'+ (key + 1) +'：<br />'
							+ extendName
							+ '</td>') : "")
					}
				})

				$.each(data.Results.IsDisplayUpdateFixedPurchaseExtendStatusStatementResults, function(key, value) {
					var extendName = data.DisplayExtendStatusList[(key + 1)];
					if ((extendName !== undefined) && (extendName !== '')) {
						header += (value ? ('<td align="center">拡張ステータス'+ (key + 1) +'：<br />'
							+ extendName
							+ '</td>') : "")
					}
				})

				header += ((data.Results.IsDisplayMailSendResults) ? ('<td align="center">メール送信</td>') : "")
					+ ((data.Results.DisplayUpdateOrderReturnResult) ? ('<td align="center">返品</td>') : "")
					+ ((data.Results.DisplayUpdateOrderInvoiceStatusResult) ? ('<td align="center">発票ステータス</td>') : "")
					+ ((data.Results.DisplayUpdateOrderInvoiceApiResult) ? ('<td align="center">電子発票連携</td>') : "")
					+ ((data.Results.DisplayExecExternalOrderInfoActionResult) ? ('<td align="center">外部受注情報連携</td>') : "")
					+ ((data.Results.DisplayUpdateStorePickupStatusResult) ? ('<td align="center">店舗受取ステータス</td>') : "")
				$('#trResultHeader').html(header);

				$('#ResultBody').empty();
				$.each(data.Results.Results, function(key, result) {
					var body = '';
					body += '<tr>'
						+ '<td><a href="javascript:openDetails(\'' + result.OrderId + '\');">' + result.OrderId + '</a></td>'
						+ displayExecResultAction(
							data.Results.IsDisplayUpdateOrderStatusResult,
							result.ResultOrderStatusChange)
						+ displayExecResultAction(
							data.Results.DisplayUpdateShippingDateStatusResult,
							result.ResultShippingDateAction)
						+ displayExecResultAction(
							data.Results.DisplayUpdateScheduledShippingDateStatusResult,
							result.ResultScheduledShippingDateAction)
						+ displayExecResultAction(
							data.Results.IsDisplayUpdateProductRealStockResult,
							result.ResultProductRealStockChange)
						+ displayExecResultAction(
							data.Results.IsDisplayUpdatePaymentStatusResult,
							result.ResultPaymentStatusChange)
						+ displayExecResultAction(
							data.Results.IsDisplayExecExternalPaymentActionResult,
							result.ResultExternalPaymentAction)
						+ displayExecResultAction(
							data.Results.IsDisplayUpdateDemandStatusResult,
							result.ResultDemandStatusChange)
						+ displayExecResultAction(
							data.Results.IsDisplayUpdateReturnExchangeStatusResult,
							result.ResultReturnExchangeStatusChange)
						+ displayExecResultAction(
							data.Results.IsDisplayUpdateRepaymentStatusResult,
							result.ResultRepaymentStatusChange)
						+ displayExecResultAction(
							data.Results.IsDisplayUpdateFixedPurchaseIsAliveResult,
							result.ResultFixedPurchaseIsAliveChangeAction)
						+ displayExecResultAction(
							data.Results.IsDisplayUpdateFixedPurchasePaymentStatusResult,
							result.ResultFixedPurchasePaymentStatusChangeAction)
						+ displayExecResultAction(
							data.Results.IsDisplayUpdateNextShippingDateResult,
							result.ResultNextShippingDateChangeAction)
						+ displayExecResultAction(
							data.Results.IsDisplayUpdateNextNextShippingDateResult,
							result.ResultNextNextShippingDateChangeAction)
						+ displayExecResultAction(
							data.Results.DisplayUpdateStorePickupStatusResult,
							result.ResultStorePickupStatusChange)

					$.each(data.Results.IsDisplayUpdateOrderExtendStatusStatementResults, function(index, value){
						var extendName = data.DisplayExtendStatusList[(index + 1)];
						if ((extendName !== undefined) && (extendName !== '')) {
							body += displayExecResultAction(
								value,
								result.ResultOrderExtendStatusChange[index])
						}
					})

					$.each(data.Results.IsDisplayUpdateFixedPurchaseExtendStatusStatementResults, function(index, value){
						var extendName = data.DisplayExtendStatusList[(index + 1)];
						if ((extendName !== undefined) && (extendName !== '')) {
							body += displayExecResultAction(
								value,
								result.ResultFixedPurchaseExtendStatusChange[index])
						}
					})

					body += displayExecResultAction(
						data.Results.IsDisplayMailSendResults,
						result.ResultMailSend)
						+ displayExecResultAction(
							data.Results.DisplayUpdateOrderReturnResult,
							result.ResultOrderReturnChange)
						+ displayExecResultAction(
							data.Results.DisplayUpdateOrderInvoiceStatusResult,
							result.ResultOrderInvoiveStatusChange)
						+ displayExecResultAction(
							data.Results.DisplayUpdateOrderInvoiceApiResult,
							result.ResultOrderInvoiveApiChange)
						+ displayExecResultAction(
							data.Results.DisplayExecExternalOrderInfoActionResult,
							result.ResultExternalOrderInfoAction)
						+ '</tr>';
					$('#ResultBody').append(body);
				})
			}
		}

		// Update display order count for current workflow setting selection
		function updateDisplayOrderCountForWorkflowSetting(workflowKbn, workflowNo, workflowType, remainCount) {
			$("#ulOrderWorkflowList li").each(function () {
				var element = $(this);
				if ((element.data('workflowKbn') === ('kbn' + workflowKbn))
					&& (element.data('workflowNo') === ('no' + workflowNo))
					&& (element.data('workflowType') === workflowType)) {
					element.find('.order-workflow-list-block-list-item-num-value').html(remainCount);
					return false;
				}
			});
		}

		// Display exec result action
		function displayExecResultAction(isDisplay, resultAction) {
			var icon = ((resultAction == '<%= (int)OrderCommon.ResultKbn.UpdateOK %>')
				? "icon-circle" 
				: (resultAction == '<%= (int)OrderCommon.ResultKbn.UpdateNG %>')
					? "icon-close"
					: "");
			var htmlString = (isDisplay
				? ('<td class="' + ((resultAction == '<%= (int)OrderCommon.ResultKbn.UpdateNG %>') ? "is-error" : "") + '">'
					+ '<span class="' + icon + '"></span>'
					+ ((resultAction == '<%= (int)OrderCommon.ResultKbn.UpdateOK %>')
						? "" 
						: (resultAction == '<%= (int)OrderCommon.ResultKbn.NoUpdate %>')
							? "更新しない"
							: (resultAction == '<%= (int)OrderCommon.ResultKbn.UpdatePart %>')
								? "保留"
								: "問題あり")
					+ '</td>')
				: "")
			return htmlString;
		}

		// Clear data
		function clearData() {
			$('#tdPageList').empty();
			$('#ulData').empty();
			$('.workflow-data').empty();
		}

		// Process continue
		function processContinue() {
			workflow.step.switch(1);
			checkProcess(currentWorkflowIndex);
			$('#mailSendMessage').hide();
		}

		// Get order file import setting
		function getOrderFileImportSetting() {
			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/GetOrderFileImportSetting";
			var request = callAjax(urlRequest, "");
			request.done(function(response) {
				var data = JSON.parse(response.d);
				orderFiles = data;
				$('#ddlOrderFile').empty();
				for (var index = 0; index < data.length; index++) {
					$('#ddlOrderFile').append('<option value="'+ index +'">'+ data[index].OrderText +'</option>');
				}
				$('#cbExecExternalShipmentEntry').prop('checked', true);
				$('#lbOrderFileInfo').html(data[0].OrderInfo);
				setTimeout(function() {
					hideLoading(2);
				}, 500);
			});
		}

		// Call ajax
		function callAjax(urlRequest, dataRequest) {
			var request = $.ajax({
				type: "POST",
				url: urlRequest,
				data: dataRequest,
				async: false,
				contentType: "application/json; charset=utf-8",
				dataType: "json",
				error: pageReload });
			return request;
		}

		// Display import setting info
		function displayImportSettingInfo(select) {
			$('#lbOrderFileInfo').html(orderFiles[select.value].OrderInfo);
			if (orderFiles[select.value].CanShipmentEntry) {
				$('#spShipmentEntry').show();
				$('#cbExecExternalShipmentEntry').prop('checked', true);
			} else {
				$('#spShipmentEntry').hide();
			}
		}

		// Import order file
		function importOrderFile() {
			var fileControl = $("#fFile")[0].files[0];
			var formData = new FormData();
			var index = Number($('#ddlOrderFile').val());
			formData.append("workflowName",workflowList[currentWorkflowIndex].workflowName);
			formData.append("workflowNo",workflowList[currentWorkflowIndex].workflowNo);
			formData.append("workflowKbn", "<%:Request.QueryString[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_KBN] %>");
			formData.append("selectedValue", orderFiles[index].OrderValue);
			formData.append("selectedIndex", index);
			formData.append("isShipmentEntry", (($('#cbExecExternalShipmentEntry').is(':visible')) && ($('#cbExecExternalShipmentEntry').is(':checked'))));
			formData.append("fileNamePattern", orderFiles[index].FileNamePattern);
			formData.append("mailTemplateId", orderFiles[index].MailTemplateId);
			formData.append("userfile", fileControl);
			showLoading(3);
			$.ajax({
				type: "POST",
				contentType: false,
				url: '<%=Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERWORKFLOW_ORDER_FILE_IMPORT %>',
				processData: false,
				data: formData,
				success: function(response) {
					var data = JSON.parse(response);

					$('#divResultTable').hide();
					$('#divUploadResult').show();
					$("li.order-workflow-list-block-result-num-item.is-remaining").hide();
					$('#spTotalCase').html(data.TotalCase);
					$('#spSuccessCase').html(data.SuccessCase);
					$('#spErrorCase').html(data.ErrorCase);
					$('#spFileType').html(orderFiles[index].OrderText);
					$('#spFilePath').html(fileControl.name);
					$('#spImportMessage').html(data.ResultMessage);

					if (data.ImportSuccess) {
						$('#spImportMessage').css('color', 'black');
					} else {
						$('#spImportMessage').css('color', 'red');

						// エラーデータが存在している場合に画面に表示する
						var errorDatas = data.ErrorData;

						if (errorDatas != null && errorDatas.length > 0) {
							var header = '<tr class="list_title_bg">';
							var body = '';
							for (let i = 0; i < errorDatas.length; i++) {
								body += '<tr class="list_item_bg1">';
								for (let key in errorDatas[i]) {
									if (i == 0) {
										header += '<td align="center">' + key + '    </td>'
									}

									body += '<td align="left" nowrap="nowrap">' + errorDatas[i][key] + '  </td>'
								}
								body += '</tr>';
							}
							header += '</tr>'

							var appendErrorMessage = '<table class="list_table" width="758" border="0" cellspacing="1">' + '  <tbody>' + header + body + '  </tbody>' + '</table>';

							$('#errorList').html(appendErrorMessage);
							$('#errorList').show();
						} else {
							$('#errorList').hide();
						}
					}

					// 非同期実行の場合、件数の結果を非表示にする
					if (data.IsAsyncExec) {
						$('#mailSendMessage').show();
						$('#divResultNum').hide();
					}

					$('#divBottom').hide();
					$('.dd-file-select-file-cancel').click();

					clearInputFile();

					setTimeout(function() {
							hideLoading(3);
						},
						500);
				},
				
				error: function (xhr, ajaxOptions, thrownError) {

					// Reload page when login session expired
					if (xhr.status == 401) {
						window.location.reload();
					}

					$('#divResultTable').hide();
					$('#divUploadResult').show();
					$("li.order-workflow-list-block-result-num-item.is-remaining").hide();
					$('#spTotalCase').html(0);
					$('#spSuccessCase').html(0);
					$('#spErrorCase').html(0);
					$('#spFileType').html(orderFiles[index].OrderText);
					$('#spFilePath').html(fileControl.name);

					$('#spImportMessage').html("システムエラーが発生しました。<br />システム管理者に連絡してください。");
					$('#spImportMessage').css('color', 'red');
					$('#divBottom').hide();
					$('.dd-file-select-file-cancel').click();

					clearInputFile();

					setTimeout(function(){
						hideLoading(3);
					}, 500);
				}
			});
		}

		// Check valid input
		function checkValidInput() {
			if ($('#fFile').prop('files').length == 0) {
				$('#btnImport').attr('disabled', 'true');
				$('#btnClearInputFile').css('display', 'none');
			} else {
				$('#btnImport').removeAttr('disabled');
				$('#btnClearInputFile').css('display', 'block');
			}
		}

		// Clear input file
		function clearInputFile() {
			$("#fFile").val(null);
			checkValidInput();
		}

		// Set search param
		function setSearchParam() {
			var shippingPrefectureKey = '<%= Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_PREFECTURE %>'
			var inputs = $('.needInput');
			$.each(inputs, function(key, input) {
				if (input.type === 'text') {
					$('#'+input.id).val(urlQueryString[input.name]);
				}
				if (input.type === 'checkbox') {
					$('#'+input.id).prop('checked', ((urlQueryString[input.name] === true) || (urlQueryString[input.name] === "true")));
				}
				if ((input.name == shippingPrefectureKey) && urlQueryString[shippingPrefectureKey]) {
					$.each(urlQueryString[shippingPrefectureKey].split(','), function(key, value){
						if (value === input.value) {
							$('#'+input.id).prop('checked', true);
						}
					})
				}
			})

			if (typeof setCheckStateForAllLocalAreaCheckBoxs === 'function') {
				setCheckStateForAllLocalAreaCheckBoxs();
			}
		}

		// Clear search
		function clearSearch(isClear) {
			var inputs = $('.needInput');
			$.each(inputs, function(key, input) {
				if (input.type == 'checkbox') {
					$('#' + input.id).prop('checked', false);
				} else {
					$('#' + input.id).val('');
				}
				urlQueryString[input.name] = '';
			})

			reset();
			errorMessageReset();

			if (isClear) {
				updateUrl();
				getWorkflowExecData(1);
			}
		}

		// Load export setting
		function loadExportSetting(workflowType) {
			$('#ddlExportSetting').empty();
			$('#ddlExportSetting').append(
				$('<option>', {
					value: '',
					text: ''
				})
			);

			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/CreateMasterExportSettingItems";
			var dataRequest = "{ type:" + JSON.stringify(workflowType) + "}";
			var request = callAjax(urlRequest, dataRequest);

			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = JSON.parse(response.d);
				if (data.length == 0) return;

				// Set data to dropdownlist
				for (var index = 0; index < data.length; index++) {
					var item = data[index];
					addOptionToSelect('#ddlExportSetting', item.key, item.value);
				}
			});
		}

		// Export file
		function exportFile() {
			var key = $('#ddlExportSetting').find('option:selected').val();
			var requestData = {
				ExportKey: key,
				workflowNo: workflowList[currentWorkflowIndex].workflowNo,
				workflowKbn: workflowList[currentWorkflowIndex].workflowKbn,
				workflowType: workflowList[currentWorkflowIndex].workflowType,
				searchCondition: urlQueryString,
			};
			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/Export";
			var dataRequest = "{ data: " + JSON.stringify(requestData) + " }";
			var request = callAjax(urlRequest, dataRequest);

			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = response.d;
				if (data) {
					url = '<%= string.Format(
						"{0}{1}",
						Constants.PATH_ROOT,
						Constants.PAGE_MANAGER_MASTEREXPORT) %>';
					window.open(url,'_blank');
				} else {
					url = '<%= string.Format(
						"{0}{1}",
						Constants.PATH_ROOT,
						Constants.PAGE_MANAGER_ERROR) %>';
					window.open(url,'_blank');
				}
			});
		}

		// Check show unsync button
		function checkShowUnSync(checkKey) {
			var requestData = {
				TotalName: checkKey,
				workflowNo: workflowList[currentWorkflowIndex].workflowNo,
				workflowKbn: workflowList[currentWorkflowIndex].workflowKbn,
				workflowType: workflowList[currentWorkflowIndex].workflowType,
				searchCondition: urlQueryString,
			};
			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/GetTotalCount";
			var dataRequest = "{ data: " + JSON.stringify(requestData) + " }";
			var request = callAjax(urlRequest, dataRequest);
			request.done(function(response) {
				if ((response == null)
					|| (response.d == undefined)) return;

				var data = response.d;
				if (parseInt(data) > parseInt(<%= Constants.CONST_COUNT_PDF_DIRECTDOUNLOAD %>)) {
					if (checkKey == '<%= Constants.KBN_PDF_OUTPUT_TOTAL_PICKING_LIST %>') {
						pickingListItemCounts = parseInt(data);
						$('#btnTotalPickingListOutputUnsync').show();
						$('#btnTotalPickingListOutput').hide();
					}

					if (checkKey == '<%= Constants.KBN_PDF_OUTPUT_RECEIPT %>') {
						totalHasReceiptOrderCounts = parseInt(data);
						$('#btnPdfOutputReceipt').hide();
						$('#btnPdfOutputReceiptUnsync').show();
					}
				}

				if (checkKey == '<%= Constants.KBN_PDF_OUTPUT_RECEIPT %>' && parseInt(data) == 0) {
					$('#btnPdfOutputReceipt').prop('disabled', true);
				}
			});
		}

		// Set export button
		function setExportButton() {
			// 送り状ダウンロードリンク表示
			var shippingLabelExportHtml = '';
			if ('<%: Constants.INVOICECSV_ENABLED
				&& MenuUtility.HasAuthority(
					this.LoginOperatorMenu,
					this.RawUrl,
					Constants.KBN_MENU_FUNCTION_ORDER_FILE_EXPORT_DL) %>' === 'True') {
				var shippingLabelExportSettings = JSON.parse('<%= GetShippingLabelExportSetting() %>');
				for (var index = 0; index < shippingLabelExportSettings.length; index++) {
					shippingLabelExportHtml += '<input name="btnShippingLabelExport" value="  ' + shippingLabelExportSettings[index].Displayname + '  " type="button" class="btn btn-sub btn-size-s" onclick="exportShippingLabel(' + shippingLabelExportSettings[index].Index + ');" />';
				}
			}

			// 基幹システム連携用データのダウンロードアンカーテキストを取得・データバインド
			var downloadAnchorText = JSON.parse('<%= GetDownloadAnchorTextSetting() %>');
			var downloadAnchorTextHtml = '';
			for (var index = 0; index < downloadAnchorText.length; index++) {
				downloadAnchorTextHtml += '<input name="btnInteractionDataExport" value="  ' + downloadAnchorText[index].value + '  " type="button" class="btn btn-sub btn-size-s" onclick="exportInteractionData(' + downloadAnchorText[index].key + ');" />';
			}

			// Set display export button
			var exportList = '<div class="order-workflow-list-block-output-content">'
				+ '  <div class="order-workflow-list-block-output-content-block export-button-list">'
				+ '    <select id="ddlExportSetting" class="input-size-s"></select>'
				+ '    <input type="button" value="  出力  " class="btn btn-sub btn-size-s" onclick="exportFile();" />'
				+ '  </div>'
				+ '  <div class="order-workflow-list-block-output-content-block export-button-list">'
				+ '    <input id="btnPdfOutput" type="button" value="  納品書出力  " class="btn btn-sub btn-size-s" onclick="exportPdfOutput();" />'
				+ '    <input id="btnPdfOutputUnsync" type="button" value="  納品書出力  " class="btn btn-sub btn-size-s" onclick="exportPdfOutputUnsync();" />'
				+ '    <input id="btnPdfOutputReceipt" type="button" value="  領収書出力  " class="btn btn-sub btn-size-s" onclick="exportPdfOutputReceipt();" />'
				+ '    <input id="btnPdfOutputReceiptUnsync" type="button" value="  領収書出力  " class="btn btn-sub btn-size-s" onclick="exportPdfOutputReceiptUnsync();" />'
				+ '    <input id="btnTotalPickingListOutput" type="button" value="  ピッキングリスト出力  " class="btn btn-sub btn-size-s" onclick="exportTotalPickingListOutput();" />'
				+ '    <input id="btnTotalPickingListOutputUnsync" type="button" value="  ピッキングリスト出力  " class="btn btn-sub btn-size-s" onclick="exportTotalPickingListOutputUnsync();" />'
				+      shippingLabelExportHtml
				+      downloadAnchorTextHtml
				+ '    <input id="btnPdfOutputOrderStatement" type="button" value="  受注明細書出力  " class="btn btn-sub btn-size-s" onclick="exportPdfOutputOrderStatement();" />'
				+ '    <input id="btnPdfOutputOrderStatementUnsync" type="button" value="  受注明細書出力  " class="btn btn-sub btn-size-s" onclick="exportPdfOutputOrderStatementUnsync();" />'
				+ '    <input id="btnPrintInvoiceOrderForTwECPay" type="button" value="  荷物送り状印刷  " class="btn btn-sub btn-size-s" onclick="exportPrintInvoiceOrderForTwECPay();" />'
				+ '  </div>'
				+ '  <div class="order-workflow-list-block-output-content-block export-button-list" id="divNormal">'
				+ '    <input name="btnImportTargetList" type="button" value="ターゲットリストを作成" class="btn btn-sub btn-size-s" onclick="openTargetList();" disabled />'
				+ '  </div>'
				+ '  <div class="order-workflow-list-block-output-content-block export-button-list" id="divIndent">'
				+ '    <input name="btnImportTargetList" type="button" value="ターゲットリストを作成" class="btn btn-sub btn-size-s" onclick="openTargetList();" disabled />'
				+ '  </div>'
				+ '</div>'
			return exportList;
		}

		// Set display div element
		function setDivCss() {
			var maxWidth = $('.order-workflow-list-block-output-content').width();
			var totalWidth = 0;
			var width = $('.order-workflow-list-block-output-content-block').first().width();
			$('#divIndent').css('padding-left', width + 10);

			$('.order-workflow-list-block-output-content-block').each(function() {
				totalWidth += $(this).width();
			});
			if ('<%= Constants.MARKETINGPLANNER_TARGETLIST_OPTION_ENABLE %>' == 'True') {
				if (totalWidth > maxWidth) {
					$('#divNormal').hide();
					$('#divIndent').show();
				} else {
					$('#divIndent').hide();
					$('#divNormal').show();
				}
			}
			else {
				$('#divNormal').hide();
				$('#divIndent').hide();
			}
		}

		// Set data for dropdown store pickup status
		function getStorePickupStatusList() {
			var statusList = JSON.parse('<%= GetStorePickupStatusList() %>');
			for (var index = 0; index < statusList.length; index++) {
				var item = statusList[index];
				addOptionToSelect('#ddlStorePickupStatus', item.key, item.value);
			}
		}

		// Set enable export button
		function setEnableExportButton(totalCounts) {
			orderTotals = totalCounts;
			var showUnsync = false;
			$('#btnPdfOutput').hide();
			$('#btnPdfOutputUnsync').hide();
			$('#btnTotalPickingListOutput').hide();
			$('#btnTotalPickingListOutputUnsync').hide();
			$('#btnPdfOutputReceipt').hide();
			$('#btnPdfOutputReceiptUnsync').hide();
			$('#btnPdfOutputOrderStatement').hide();
			$('#btnPdfOutputOrderStatementUnsync').hide();
			$('#btnHomeDeliveryShippingExport').hide();
			$('#btnHomeDeliveryReturnExport').hide();
			$('#btnPrintInvoiceOrderForTwECPay').hide();
			$('#btnPdfOutputReceipt').prop('disabled', false);

			if (totalCounts > 0) $("[name=btnImportTargetList]").removeAttr("disabled");
			if (totalCounts == 0 || workflowList[currentWorkflowIndex].workflowType != '<%= WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER %>') return;

			if ('<%= MenuUtility.HasAuthority(
				this.LoginOperatorMenu,
				this.RawUrl,
				Constants.KBN_MENU_FUNCTION_ORDERWF_PDF_OUTPUT_DL) %>' == 'True') {
				$('#btnPdfOutputUnsync').show();
				showUnsync = (totalCounts <= parseInt(<%= Constants.CONST_COUNT_PDF_DIRECTDOUNLOAD %>));
				if (showUnsync) {
					$('#btnPdfOutput').show();
					$('#btnPdfOutputUnsync').hide();
				}
			}

			if (('<%= (Constants.PDF_OUTPUT_PICKINGLIST_ENABLED
					&& MenuUtility.HasAuthority(
						this.LoginOperatorMenu,
						this.RawUrl,
						Constants.KBN_MENU_FUNCTION_ORDERWF_PICKING_LIST_DL)) %>') == 'True') {
				$('#btnTotalPickingListOutput').show();
				checkShowUnSync('TotalPickingList');
			}

			if (('<%= (Constants.RECEIPT_OPTION_ENABLED
					&& MenuUtility.HasAuthority(
						this.LoginOperatorMenu,
						this.RawUrl,
						Constants.KBN_MENU_FUNCTION_ORDERWF_RECEIPT_DL)) %>') == 'True') {
				$('#btnPdfOutputReceipt').show();
				checkShowUnSync('Receipt');
			}

			if (('<%= (Constants.PDF_OUTPUT_ORDERSTATEMENT_ENABLED
					&& MenuUtility.HasAuthority(
						this.LoginOperatorMenu,
						this.RawUrl,
						Constants.KBN_MENU_FUNCTION_ORDERWF_STATEMENT_DL)) %>') == 'True') {
				$('#btnPdfOutputOrderStatementUnsync').show();
				showUnsync = (totalCounts <= parseInt(<%= Constants.CONST_COUNT_PDF_DIRECTDOUNLOAD %>));
				if (showUnsync) {
					$('#btnPdfOutputOrderStatement').show();
					$('#btnPdfOutputOrderStatementUnsync').hide();
				}
			}

			if ('<%= Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED %>' == 'True') {
				$('#btnPrintInvoiceOrderForTwECPay').show();
			} else if (('<%= Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED %>' == 'True')
					&& ('<%= Constants.TWPELICAN_COOPERATION_EXTEND_ENABLED %>' == 'False')) {
				$('#btnHomeDeliveryShippingExport').show();
				$('#btnHomeDeliveryReturnExport').show();
			}
	}

	// Go to error page
	function goToErrorPage() {
		window.open('<%= CreateErrorPageUrl() %>', '_blank');
		}

		// Export pdf output
		function exportPdfOutput() {
			var requestData = {
				isUnSysnc: false,
				workflowNo: workflowList[currentWorkflowIndex].workflowNo,
				workflowKbn: workflowList[currentWorkflowIndex].workflowKbn,
				workflowType: workflowList[currentWorkflowIndex].workflowType,
				searchCondition: urlQueryString,
			};
			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/ExportPdfOutput";
			var dataRequest = "{ data: " + JSON.stringify(requestData) + " }";

			var request = callAjax(urlRequest, dataRequest);
			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = response.d;
				if (data) {
					var url = '';
					<% if (Constants.GLOBAL_OPTION_ENABLE){ %>
					if (window.confirm('言語別の納品書ファイルが作成されますが、よろしいでしょうか？')) {
						url = '<%= CreateDownloadWaitExportPageUrl() %>';
						window.open(url, 'pdfoutput', 'width=850,height=415,top=120,left=320,status=NO,scrollbars=yes');
						return;
					} else {
						return false;
					};
					<% } %>

					url = '<%= CreatePdfExportPageUrl(Constants.KBN_PDF_OUTPUT_ORDER_INVOICE) %>';
					window.open(url, '_blank');
				} else {
					goToErrorPage();
				}
			});
		}

		// Export pdf output unsync
		function exportPdfOutputUnsync() {
			var maxCounts = parseInt('<%= w2.App.Common.Pdf.PdfCreater.OrderInvoiceCreater.CONST_OUTPUT_ORDER_MAX_COUNT %>');
			var fileNumbers = ((orderTotals > maxCounts)
				? Math.ceil(orderTotals / maxCounts)
				: 1);
			var orderCounts = ((orderTotals > maxCounts)
				? maxCounts
				: orderTotals);
			if (window.confirm('注文情報が' + orderCounts + '件の納品書ファイルが' + fileNumbers + 'ファイル作成されますが、よろしいでしょうか？')) {
				var requestData = {
					isUnSysnc: true,
					workflowNo: workflowList[currentWorkflowIndex].workflowNo,
					workflowKbn: workflowList[currentWorkflowIndex].workflowKbn,
					workflowType: workflowList[currentWorkflowIndex].workflowType,
					searchCondition: urlQueryString,
				};
				var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/ExportPdfOutput";
				var dataRequest = "{ data: " + JSON.stringify(requestData) + " }";

				var request = callAjax(urlRequest, dataRequest);
				request.done(function(response) {
					if ((response == null)
						|| (response.d == undefined)) return;

					var data = response.d;
					if (data) {
						var url = '<%= CreateDownloadWaitExportPageUrl() %>';
						window.open(url, 'pdfoutput', 'width=850,height=415,top=120,left=320,status=NO,scrollbars=yes');
					} else {
						goToErrorPage();
					}
				});
			} else {
				return false; 
			};
		}

		// Export total picking list output
		function exportTotalPickingListOutput() {
			var requestData = {
				isUnSysnc: false,
				workflowNo: workflowList[currentWorkflowIndex].workflowNo,
				workflowKbn: workflowList[currentWorkflowIndex].workflowKbn,
				workflowType: workflowList[currentWorkflowIndex].workflowType,
				searchCondition: urlQueryString,
			};
			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/ExportTotalPickingListOutput";
			var dataRequest = "{ data: " + JSON.stringify(requestData) + " }";

			var request = callAjax(urlRequest, dataRequest);
			request.done(function(response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = response.d;
				if (data) {
					var url = '<%= CreatePdfExportPageUrl(Constants.KBN_PDF_OUTPUT_TOTAL_PICKING_LIST) %>';
					window.open(url, '_blank');
				} else {
					goToErrorPage();
				}
			});
		}

		// Export total picking list output unsync
		function exportTotalPickingListOutputUnsync() {
			var maxCounts = parseInt('<%= w2.App.Common.Pdf.PdfCreater.TotalPickingListCreater.CONST_OUTPUT_ORDER_MAX_COUNT %>');
			var pickingListFileNumbers = ((pickingListItemCounts > maxCounts)
				? Math.ceil(pickingListItemCounts / maxCounts)
				: 1);
			var orderItemCounts = ((pickingListItemCounts > maxCounts)
				? maxCounts
				: orderTotals);
			if (window.confirm('注文商品情報が' + orderItemCounts + '件のピッキングリストファイルがおよそ' + pickingListFileNumbers + 'ファイル作成されますが、よろしいでしょうか？')) {
				var requestData = {
					isUnSysnc: true,
					workflowNo: workflowList[currentWorkflowIndex].workflowNo,
					workflowKbn: workflowList[currentWorkflowIndex].workflowKbn,
					workflowType: workflowList[currentWorkflowIndex].workflowType,
					searchCondition: urlQueryString,
				};
				var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/ExportTotalPickingListOutput";
				var dataRequest = "{ data: " + JSON.stringify(requestData) + " }";

				var request = callAjax(urlRequest, dataRequest);
				request.done(function (response) {
					if ((response == null) || (response.d == undefined)) return;

					var data = response.d;
					if (data) {
						var url = '<%= CreateDownloadWaitExportPageUrl() %>';
						window.open(url, 'pdfoutput', 'width=850,height=415,top=120,left=320,status=NO,scrollbars=yes');
					} else {
						goToErrorPage();
					}
				});
			} else {
				return false; 
			};
		}

		// Export pdf output order statement
		function exportPdfOutputOrderStatement() {
			var requestData = {
				isUnSysnc: false,
				workflowNo: workflowList[currentWorkflowIndex].workflowNo,
				workflowKbn: workflowList[currentWorkflowIndex].workflowKbn,
				workflowType: workflowList[currentWorkflowIndex].workflowType,
				searchCondition: urlQueryString,
			};
			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/ExportPdfOutputOrderStatement";
			var dataRequest = "{ data: " + JSON.stringify(requestData) + " }";

			var request = callAjax(urlRequest, dataRequest);
			request.done(function (response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = response.d;
				if (data) {
					var url = '<%= CreatePdfExportPageUrl(Constants.KBN_PDF_OUTPUT_ORDER_STATEMENT) %>';
					window.open(url, '_blank');
				} else {
					goToErrorPage();
				}
			});
		}

		// Export pdf output order statement unsync
		function exportPdfOutputOrderStatementUnsync() {
			var maxCounts = parseInt('<%= w2.App.Common.Pdf.PdfCreater.OrderStatementCreater.CONST_OUTPUT_ORDER_MAX_COUNT%>');
			var orderStatementFileNumbers = ((orderTotals > maxCounts)
				? Math.ceil(orderTotals / maxCounts)
				: 1);
			var orderStatementOrderCounts = ((orderTotals > maxCounts)
				? maxCounts
				: orderTotals);
			if (window.confirm('注文情報が' + orderStatementOrderCounts + '件の受注明細書ファイルが' + orderStatementFileNumbers + 'ファイル作成されますが、よろしいでしょうか？')) {
				var requestData = {
					isUnSysnc: true,
					workflowNo: workflowList[currentWorkflowIndex].workflowNo,
					workflowKbn: workflowList[currentWorkflowIndex].workflowKbn,
					workflowType: workflowList[currentWorkflowIndex].workflowType,
					searchCondition: urlQueryString,
				};
				var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/ExportPdfOutputOrderStatement";
				var dataRequest = "{ data: " + JSON.stringify(requestData) + " }";

				var request = callAjax(urlRequest, dataRequest);
				request.done(function (response) {
					if ((response == null) || (response.d == undefined)) return;

					var data = response.d;
					if (data) {
						var url = '<%= CreateDownloadWaitExportPageUrl() %>';
						window.open(url, 'pdfoutput', 'width=850,height=415,top=120,left=320,status=NO,scrollbars=yes');
					} else {
						goToErrorPage();
					}
				});
			} else {
				return false; 
			};
		}

		// Export pdf output receipt
		function exportPdfOutputReceipt() {
			var requestData = {
				isUnSysnc: false,
				workflowNo: workflowList[currentWorkflowIndex].workflowNo,
				workflowKbn: workflowList[currentWorkflowIndex].workflowKbn,
				workflowType: workflowList[currentWorkflowIndex].workflowType,
				searchCondition: urlQueryString,
			};
			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/ExportPdfOutputReceipt";
			var dataRequest = "{ data: " + JSON.stringify(requestData) + " }";

			var request = callAjax(urlRequest, dataRequest);
			request.done(function (response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = response.d;
				if (data) {
					var url = '<%= CreatePdfExportPageUrl(Constants.KBN_PDF_OUTPUT_RECEIPT) %>';
					window.open(url, '_blank');
				} else {
					goToErrorPage();
				}
			});
		}

		// Export pdf output receipt unsync
		function exportPdfOutputReceiptUnsync() {
			var maxCounts= parseInt('<%= w2.App.Common.Pdf.PdfCreater.ReceiptCreater.CONST_OUTPUT_ORDER_MAX_COUNT %>');
			var receiptFileNumbers = ((totalHasReceiptOrderCounts > maxCounts)
				? Math.ceil(totalHasReceiptOrderCounts / maxCounts)
				: 1);
			var receiptOrderCounts = ((totalHasReceiptOrderCounts > maxCounts)
				? maxCounts
				: totalHasReceiptOrderCounts);
			if (window.confirm('注文情報' + receiptOrderCounts + '件の領収書ファイルが' + receiptFileNumbers + 'ファイル作成されますが、よろしいでしょうか？')) {
				var requestData = {
					isUnSysnc: true,
					workflowNo: workflowList[currentWorkflowIndex].workflowNo,
					workflowKbn: workflowList[currentWorkflowIndex].workflowKbn,
					workflowType: workflowList[currentWorkflowIndex].workflowType,
					searchCondition: urlQueryString,
				};
				var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/ExportPdfOutputReceipt";
				var dataRequest = "{ data: " + JSON.stringify(requestData) + " }";

				var request = callAjax(urlRequest, dataRequest);
				request.done(function (response) {
					if ((response == null) || (response.d == undefined)) return;

					var data = response.d;
					if (data) {
						var url = '<%= CreateDownloadWaitExportPageUrl() %>';
						window.open(url, 'pdfoutput', 'width=850,height=415,top=120,left=320,status=NO,scrollbars=yes');
					} else {
						goToErrorPage();
					}
				});
			} else {
				return false; 
			};
		}

		// Export print invoice order for tw ecpay
		function exportPrintInvoiceOrderForTwECPay() {
			var requestData = {
				workflowNo: workflowList[currentWorkflowIndex].workflowNo,
				workflowKbn: workflowList[currentWorkflowIndex].workflowKbn,
				workflowType: workflowList[currentWorkflowIndex].workflowType,
				searchCondition: urlQueryString,
			};
			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/PrintInvoiceOrderForTwECPay";
			var dataRequest = "{ data: " + JSON.stringify(requestData) + " }";

			var request = callAjax(urlRequest, dataRequest);
			request.done(function (response) {
				if ((response == null) || (response.d == undefined)) return;

				var data = response.d;

				// execute function
				eval(data);
			});
		}

		// Export shipping label
		function exportShippingLabel(index) {
			var requestData = {
				isUnSysnc: false,
				workflowNo: workflowList[currentWorkflowIndex].workflowNo,
				workflowKbn: workflowList[currentWorkflowIndex].workflowKbn,
				workflowType: workflowList[currentWorkflowIndex].workflowType,
				searchCondition: urlQueryString
			};
			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/ExportShippingLabel";
			var dataRequest = "{ data: " + JSON.stringify(requestData) + " }";

			var request = callAjax(urlRequest, dataRequest);
			request.done(function (response) {
				if ((response == null) || (response.d == undefined)) return;

				var isSuccess = response.d;
				var url = '';
				if (isSuccess) {
					url = '<%= CreateOrderFileExportPageUrl(Constants.REQUEST_KEY_SHIPPING_LABEL_LINK) %>';
					url += encodeURIComponent(index);
					window.open(url, '_blank');
				} else {
					goToErrorPage();
				}
			});
		}

		// Export interaction data
		function exportInteractionData(index) {
			var requestData = {
				isUnSysnc: false,
				workflowNo: workflowList[currentWorkflowIndex].workflowNo,
				workflowKbn: workflowList[currentWorkflowIndex].workflowKbn,
				workflowType: workflowList[currentWorkflowIndex].workflowType,
				searchCondition: urlQueryString
			};
			var urlRequest = "<%: this.OrderWorkflowBaseUrl %>/ExportInteractionData";
			var dataRequest = "{ data: " + JSON.stringify(requestData) + " }";

			var request = callAjax(urlRequest, dataRequest);
			request.done(function (response) {
				if ((response == null) || (response.d == undefined)) return;

				var isSuccess = response.d;
				var url = '';
				if (isSuccess) {
					url = '<%= CreateOrderFileExportPageUrl(Constants.REQUEST_KEY_INTERACTION_DATA_LINK) %>';
					url += encodeURIComponent(index);
					window.open(url, '_blank');
				} else {
					goToErrorPage();
				}
			});
		}

		// Show loading
		function showLoading(blockNumber) {
			$('.order-workflow-list-block'+blockNumber).removeClass('is-loaded');
			$('.order-workflow-list-block'+blockNumber).addClass('is-loading');
		}

		// Hide loading
		function hideLoading(blockNumber) {
			$('.order-workflow-list-block'+blockNumber).removeClass('is-loading');
			$('.order-workflow-list-block'+blockNumber).addClass('is-loaded');
		}

		// Load shipping prefectures script
		function loadShippingPrefecturesScript() {
			var scriptElement = document.createElement("script");
			scriptElement.src = "<%= ResolveUrl("~/Js/prefectures.js") %>";
			document.body.appendChild(scriptElement);
		}

	</script>
</asp:Content>
