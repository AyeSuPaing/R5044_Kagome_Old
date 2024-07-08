<%--
=========================================================================================================
  Module      : ダッシュボードページ(SummaryInformation.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage_responsive.master" AutoEventWireup="true" CodeFile="SummaryInformation.aspx.cs" Inherits="Form_Summary_SummaryInformation" %>
<%@ Import Namespace="w2.App.Common.Order" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="w2.App.Common.Product" %>
<%@ Import Namespace="w2.Domain.SummaryReport" %>
<%@ Import Namespace="w2.App.Common.Manager" %>
<%@ Import Namespace="w2.Domain.MenuAuthority.Helper" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<style>
		.disp-max-content {
			overflow: hidden;
			text-overflow: ellipsis;
		}
		.loader {
			border: 4px solid #cdcdcd;
			border-top: 4px solid #333333;
			border-radius: 50%;
			width: 30px;
			height: 30px;
			animation: spin 2s linear infinite;
		}
		@keyframes spin {
			0% { transform: rotate(0deg); }
			100% { transform: rotate(360deg); }
		}
		.order-workflow-loading {
			width: 100%;
			margin: 131px 0;
		}
		.order-workflow-loading > div {
			margin-left: auto;
			margin-right: auto;
		}
		.order-workflow-loading > div > .loader {
			margin-left: auto;
			margin-right: auto;
		}
		table .page-title {
			margin-bottom: 20px;
			float: left;
		}
		.page-title, .h1 {
			font-size: 20px;
			margin-bottom: 20px;
		}
		.dashboard-update {
			font-size: 16px;
			font-weight: normal;
			margin-left: 20px;
			position: absolute;
			padding-top: 5px;
		}
	</style>
	<table cellspacing="0" cellpadding="0" width="791" border="0">
		<!--▽ タイトル ▽-->
		<tr>
			<td>
				<h1 class="page-title">ダッシュボード</h1>
				<span class="dashboard-update"><%: DateTime.Now.ToString(CONST_FORMATDATE_LONGDATE) %></span>
			</td>
		</tr>
		<tr>
			<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
		</tr>
		<!--△ タイトル △-->
		<!--▽ 詳細 ▽-->
		<tr>
			<td>
				<table class="box_border" width="784" border="0" cellspacing="1" cellpadding="0">
					<tr>
						<!-- 売上進捗状況 -->
						<td>
							<div class="dashboard-section">
								<div class="dashboard-section-header">
									<h2 class="dashboard-section-title dashboard-section-title-sale-progress">売上進捗状況（集計月：<%: DateTimeUtility.ToStringForManager(
											DateTime.Now,
											DateTimeUtility.FormatType.LongYearMonth)
										.Replace("(JST)", string.Empty)%>）
									</h2>
									<a id="btn-configuration" href="javascript:openSaleGoalSetting();" class="dashboard-info-link btn btn-txt btn-size-m btn-modal-open" data-modal-selector="#modal-dashboard-goal-progress-setting" data-modal-classname="modal-dashboard-goal-progress-setting">設定</a>
									<div class="modal-content-hide">
										<div id="modal-dashboard-goal-progress-setting">
											<p class="modal-inner-ttl">売上目標設定</p>
											<div class="dashboard-goal-progress-setting-content">
												<div class="dashboard-goal-progress-setting-content-row">
													<div class="dashboard-goal-progress-setting-block">
														<span class="dashboard-goal-progress-setting-title">年間</span>
														<asp:TextBox ID="tbAnnualGoal" CssClass="dashboard-goal-progress-setting-input" runat="server" MaxLength="15" />
														<span class="dashboard-goal-progress-setting-unit">円</span>
													</div>
													<div class="dashboard-goal-progress-setting-block">
														<span class="dashboard-goal-progress-setting-title">開始月</span>
														<asp:DropDownList ID="ddlApplicableMonth" runat="server" onchange="displaySaleGoalInput();" />
														<span class="dashboard-goal-progress-setting-unit">月</span>
													</div>
												</div>
												<div class="dashboard-goal-progress-setting-content-row" id="dvMonthly">
												</div>
											</div>
											<div>
												<asp:Label ID="lbErrorMessage" runat="server" ForeColor="red" />
											</div>
											<div class="modal-footer-action">
												<input type="button" class="btn btn-main btn-size-l" value="  設定する  " onclick="javascript:updateSaleGoal();" />
											</div>
										</div>
									</div>
									<a class="btn-toggle dashboard-section-toggle-btn" data-toggle-content-selector=".dashboard-section-goal-progress-wrapper" data-toggle-trigger-label=""></a>
								</div>
								<div class="dashboard-section-body dashboard-section-goal-progress-wrapper">
									<div ID="dvGuide" runat="server">
										<div ID="dvGuideBackground" runat="server">
											<div class="dashboard-goal-progress-wrapper">
											</div>
											<div class="dashboard-goal-progress-notes">
												<p class="dashboard-goal-progress-notes-title">達成見通し</p>
												<p class="dashboard-goal-progress-notes-item">
													<span class="dashboard-goal-progress-notes-item-icon dashboard-goal-progress-notes-item-icon-green"></span>
													<span class="dashboard-goal-progress-notes-item-label">100%以上</span>
												</p>
												<p class="dashboard-goal-progress-notes-item">
													<span class="dashboard-goal-progress-notes-item-icon dashboard-goal-progress-notes-item-icon-yellow"></span>
													<span class="dashboard-goal-progress-notes-item-label">80～99%</span>
												</p>
												<p class="dashboard-goal-progress-notes-item">
													<span class="dashboard-goal-progress-notes-item-icon dashboard-goal-progress-notes-item-icon-red"></span>
													<span class="dashboard-goal-progress-notes-item-label">80%未満</span>
												</p>
											</div>
										</div>
										<div ID="dvGuideLink" class="dashboard-goal-progress-guide-link" Visible="false" runat="server">
											売上目標設定がされていません。
											<a href="javascript:openSaleGoalSetting();" class="btn-modal-open" data-modal-selector="#modal-dashboard-goal-progress-setting" data-modal-classname="modal-dashboard-goal-progress-setting">設定する</a>
										</div>
									</div>
								</div>
							</div>
						</td>
						<!-- // 売上進捗状況 -->
					</tr>
					<tr>
						<!-- 最新レポート -->
						<td>
							<div id="dvLatestReport" class="dashboard-section" runat="server">
								<div class="dashboard-section-header">
									<h2 class="dashboard-section-title">最新レポート</h2>
									<a class="btn-toggle dashboard-section-toggle-btn" data-toggle-content-selector=".dashboard-section-body-latest-report" data-toggle-trigger-label=""></a>
									<div class="btn-group dashboard-section-header-period">
										<div class="btn-group-title">期間</div>
										<div class="btn-group-body">
											<a href="javascript:void(0)" onclick="getAndSetLatestReportsByPeriodKbn(this, '<%: Constants.FLG_SUMMARYREPORT_PERIOD_KBN_YESTERDAY %>')" class="btn-group-btn is-current">昨日</a>
											<a href="javascript:void(0)" onclick="getAndSetLatestReportsByPeriodKbn(this, '<%: Constants.FLG_SUMMARYREPORT_PERIOD_KBN_TODAY %>')" class="btn-group-btn">今日</a>
										</div>
									</div>
								</div>
								<div class="dashboard-section-body dashboard-section-body-latest-report">
									<div class="dashboard-latest-report">
										<div class="dashboard-box dashboard-latest-report-box is-type-user">
											<h3 id="hLatestReportRegisteredUserTitle" class="dashboard-latest-report-box-title">
												<span class="latest-report-prefix-title">昨日の</span>
												<span class="dashboard-latest-report-box-title-strong">ユニークユーザー数</span>
												<a class="btn-toggle dashboard-section-toggle-btn dashboard-section-toggle-btn-registered-user" data-toggle-content-selector=".dashboard-latest-report-box-breakdown-registered-user" data-toggle-trigger-label="" style="display:none"></a>
											</h3>
											<div class="dashboard-latest-report-box-value user-latest-report-type-yesterday">
												<span id="spRegisteredUserTotalCount" class="dashboard-latest-report-box-value-text">
													<%: this.LatestReport.RegisteredUser.TotalCount %>
												</span>
												<span class="dashboard-latest-report-box-value-unit">人</span>
											</div>
											<div class="dashboard-latest-report-box-value user-latest-report-type-today" style="display:none;">
												<div style="padding:10px;">
													<span class="dashboard-latest-report-box-value-text-none odometer odometer-auto-theme">
														<div class="odometer-inside">
															<span class="odometer-digit">
																<span class="odometer-digit-spacer">8</span>
																<span class="odometer-digit-inner">
																	<span class="odometer-ribbon">
																		<span class="odometer-ribbon-inner">
																			<span class="odometer-value">0</span>
																		</span>
																	</span>
																</span>
															</span>
														</div>
													</span>
													<span class="dashboard-latest-report-box-value-unit">人</span>
												</div>
												<p style="position:absolute;">集計結果は翌日表示されます</p>
											</div>
											<div class="dashboard-latest-report-box-breakdown dashboard-latest-report-box-breakdown-registered-user">
												<table class="dashboard-latest-report-box-breakdown-table">
													<tr>
														<th class="dashboard-latest-report-box-breakdown-th">PC</th>
														<td id="tdRegisteredUserPcCount" class="dashboard-latest-report-box-breakdown-value">
															<%: this.LatestReport.RegisteredUser.DispPcCount %>
														</td>
														<td class="dashboard-latest-report-box-breakdown-rate">
															<span class="dashboard-latest-report-box-breakdown-rate-bar">
																<span id="spRegisteredUserPcRateBar" class="dashboard-latest-report-box-breakdown-rate-bar-bar" style='width: <%: this.LatestReport.RegisteredUser.PcRate %>'></span>
																<span id="spRegisteredUserPcRate" class="dashboard-latest-report-box-breakdown-rate-bar-text">
																	<%: this.LatestReport.RegisteredUser.PcRate %>
																</span>
															</span>
														</td>
													</tr>
													<tr>
														<th class="dashboard-latest-report-box-breakdown-th">SP</th>
														<td id="tdRegisteredUserSpCount" class="dashboard-latest-report-box-breakdown-value">
															<%: this.LatestReport.RegisteredUser.DispSpCount %>
														</td>
														<td class="dashboard-latest-report-box-breakdown-rate">
															<span class="dashboard-latest-report-box-breakdown-rate-bar">
																<span id="spRegisteredUserSpRateBar" class="dashboard-latest-report-box-breakdown-rate-bar-bar" style='width: <%: this.LatestReport.RegisteredUser.SpRate %>'></span>
																<span id="spRegisteredUserSpRate" class="dashboard-latest-report-box-breakdown-rate-bar-text">
																	<%: this.LatestReport.RegisteredUser.SpRate %>
																</span>
															</span>
														</td>
													</tr>
												</table>
											</div>
										</div>
										<div class="dashboard-box dashboard-latest-report-box is-type-price">
											<h3 id="hLatestReportOrderAmountTitle" class="dashboard-latest-report-box-title">
												<span class="latest-report-prefix-title">昨日の</span>
												<span class="dashboard-latest-report-box-title-strong">注文金額</span>
												<a class="btn-toggle dashboard-section-toggle-btn dashboard-section-toggle-btn-order-amount" data-toggle-content-selector=".dashboard-latest-report-box-breakdown-order-amount" data-toggle-trigger-label="" style="display:none"></a>
											</h3>
											<div class="dashboard-latest-report-box-value">
												<span id="spOrderAmountTotalAmount" class="dashboard-latest-report-box-value-text">
													<%: this.LatestReport.OrderAmount.TotalAmount %>
												</span>
												<span class="dashboard-latest-report-box-value-unit">円</span>
											</div>
											<div class="dashboard-latest-report-box-breakdown dashboard-latest-report-box-breakdown-order-amount">
												<table class="dashboard-latest-report-box-breakdown-table">
													<tr>
														<th class="dashboard-latest-report-box-breakdown-th">PC</th>
														<td id="tdOrderAmountPcAmount" class="dashboard-latest-report-box-breakdown-value">
															<%: this.LatestReport.OrderAmount.DispPcAmount %>
														</td>
														<td class="dashboard-latest-report-box-breakdown-rate">
															<span class="dashboard-latest-report-box-breakdown-rate-bar">
																<span id="spOrderAmountPcRateBar" class="dashboard-latest-report-box-breakdown-rate-bar-bar" style='width: <%: this.LatestReport.OrderAmount.PcRate %>'></span>
																<span id="spOrderAmountPcRate" class="dashboard-latest-report-box-breakdown-rate-bar-text">
																	<%: this.LatestReport.OrderAmount.PcRate %>
																</span>
															</span>
														</td>
													</tr>
													<tr>
														<th class="dashboard-latest-report-box-breakdown-th">SP</th>
														<td id="tdOrderAmountSpAmount" class="dashboard-latest-report-box-breakdown-value">
															<%: this.LatestReport.OrderAmount.DispSpAmount %>
														</td>
														<td class="dashboard-latest-report-box-breakdown-rate">
															<span class="dashboard-latest-report-box-breakdown-rate-bar">
																<span id="spOrderAmountSpRateBar" class="dashboard-latest-report-box-breakdown-rate-bar-bar" style='width: <%: this.LatestReport.OrderAmount.SpRate %>'></span>
																<span id="spOrderAmountSpRate" class="dashboard-latest-report-box-breakdown-rate-bar-text">
																	<%: this.LatestReport.OrderAmount.SpRate %>
																</span>
															</span>
														</td>
													</tr>
													<tr>
														<th class="dashboard-latest-report-box-breakdown-th">その他</th>
														<td id="tdOrderAmountOtherAmount" class="dashboard-latest-report-box-breakdown-value">
															<%: this.LatestReport.OrderAmount.DispOtherAmount %>
														</td>
														<td class="dashboard-latest-report-box-breakdown-rate">
															<span class="dashboard-latest-report-box-breakdown-rate-bar">
																<span id="spOrderAmountOtherRateBar" class="dashboard-latest-report-box-breakdown-rate-bar-bar" style='width: <%: this.LatestReport.OrderAmount.OtherRate %>'></span>
																<span id="spOrderAmountOtherRate" class="dashboard-latest-report-box-breakdown-rate-bar-text">
																	<%: this.LatestReport.OrderAmount.OtherRate %>
																</span>
															</span>
														</td>
													</tr>
												</table>
											</div>
										</div>
										<div class="dashboard-box dashboard-latest-report-box is-type-order">
											<h3 id="hLatestReportOrderCountTitle" class="dashboard-latest-report-box-title">
												<span class="latest-report-prefix-title">昨日の</span>
												<span class="dashboard-latest-report-box-title-strong">注文数</span>
												<a class="btn-toggle dashboard-section-toggle-btn dashboard-section-toggle-btn-order-count" data-toggle-content-selector=".dashboard-latest-report-box-breakdown-order-count" data-toggle-trigger-label="" style="display:none"></a>
											</h3>
											<div class="dashboard-latest-report-box-value">
												<span id="spOrderCountTotalCount" class="dashboard-latest-report-box-value-text">
													<%: this.LatestReport.OrderCount.TotalCount %>
												</span>
												<span class="dashboard-latest-report-box-value-unit">件</span>
											</div>
											<div class="dashboard-latest-report-box-breakdown dashboard-latest-report-box-breakdown-order-count">
												<div class="dashboard-latest-report-box-breakdown-col">
													<table class="dashboard-latest-report-box-breakdown-table">
														<tr>
															<th class="dashboard-latest-report-box-breakdown-th">PC</th>
															<td id="tdOrderCountPcCount" class="dashboard-latest-report-box-breakdown-value">
																<%: this.LatestReport.OrderCount.DispPcCount %>
															</td>
															<td class="dashboard-latest-report-box-breakdown-rate">
																<span class="dashboard-latest-report-box-breakdown-rate-bar">
																	<span id="spOrderCountPcRateBar" class="dashboard-latest-report-box-breakdown-rate-bar-bar" style='width: <%: this.LatestReport.OrderCount.PcRate %>'></span>
																	<span id="spOrderCountPcRate" class="dashboard-latest-report-box-breakdown-rate-bar-text">
																		<%: this.LatestReport.OrderCount.PcRate %>
																	</span>
																</span>
															</td>
														</tr>
														<tr>
															<th class="dashboard-latest-report-box-breakdown-th">SP</th>
															<td id="tdOrderCountSpCount" class="dashboard-latest-report-box-breakdown-value">
																<%: this.LatestReport.OrderCount.DispSpCount %>
															</td>
															<td class="dashboard-latest-report-box-breakdown-rate">
																<span class="dashboard-latest-report-box-breakdown-rate-bar">
																	<span id="spOrderCountSpRateBar" class="dashboard-latest-report-box-breakdown-rate-bar-bar" style='width: <%: this.LatestReport.OrderCount.SpRate %>'></span>
																	<span id="spOrderCountSpRate" class="dashboard-latest-report-box-breakdown-rate-bar-text">
																		<%: this.LatestReport.OrderCount.SpRate %>
																	</span>
																</span>
															</td>
														</tr>
														<tr>
															<th class="dashboard-latest-report-box-breakdown-th">その他</th>
															<td id="tdOrderCountOtherCount" class="dashboard-latest-report-box-breakdown-value">
																<%: this.LatestReport.OrderCount.DispOtherCount %>
															</td>
															<td class="dashboard-latest-report-box-breakdown-rate">
																<span class="dashboard-latest-report-box-breakdown-rate-bar">
																	<span id="spOrderCountOtherRateBar" class="dashboard-latest-report-box-breakdown-rate-bar-bar" style='width: <%: this.LatestReport.OrderCount.OtherRate %>'></span>
																	<span id="spOrderCountOtherRate" class="dashboard-latest-report-box-breakdown-rate-bar-text">
																		<%: this.LatestReport.OrderCount.OtherRate %>
																	</span>
																</span>
															</td>
														</tr>
													</table>
												</div>
												<div class="dashboard-latest-report-box-breakdown-col">
													<table class="dashboard-latest-report-box-breakdown-table">
														<tr>
															<th class="dashboard-latest-report-box-breakdown-th">通常</th>
															<td id="tdOrderCountNormalCount" class="dashboard-latest-report-box-breakdown-value">
																<%: this.LatestReport.OrderCount.DispNormalCount %>
															</td>
															<td class="dashboard-latest-report-box-breakdown-rate">
																<span class="dashboard-latest-report-box-breakdown-rate-bar">
																	<span id="spOrderCountNormalRateBar" class="dashboard-latest-report-box-breakdown-rate-bar-bar" style='width: <%: this.LatestReport.OrderCount.NormalRate %>'></span>
																	<span id="spOrderCountNormalRate" class="dashboard-latest-report-box-breakdown-rate-bar-text">
																		<%: this.LatestReport.OrderCount.NormalRate %>
																	</span>
																</span>
															</td>
														</tr>
														<tr style="display: <%= Constants.FIXEDPURCHASE_OPTION_ENABLED ? string.Empty : "none" %>">
															<th class="dashboard-latest-report-box-breakdown-th">定期</th>
															<td id="tdOrderCountFixedPurchaseCount" class="dashboard-latest-report-box-breakdown-value">
																<%: this.LatestReport.OrderCount.DispFixedPurchaseCount %>
															</td>
															<td class="dashboard-latest-report-box-breakdown-rate">
																<span class="dashboard-latest-report-box-breakdown-rate-bar">
																	<span id="spOrderCountFixedPurchaseRateBar" class="dashboard-latest-report-box-breakdown-rate-bar-bar" style='width: <%: this.LatestReport.OrderCount.FixedPurchaseRate %>'></span>
																	<span id="spOrderCountFixedPurchaseRate" class="dashboard-latest-report-box-breakdown-rate-bar-text">
																		<%: this.LatestReport.OrderCount.FixedPurchaseRate %>
																	</span>
																</span>
															</td>
														</tr>
													</table>
												</div>
											</div>
										</div>
									</div>
								</div>
							</div>
						</td>
						<!-- // 最新レポート -->
					</tr>
					<tr>
						<!-- 推移レポート -->
						<td>
							<div class="dashboard-section">
								<div class="dashboard-section-header">
									<h2 class="dashboard-section-title">推移レポート</h2>
									<a class="btn-toggle dashboard-section-toggle-btn" data-toggle-content-selector=".dashboard-section-body-transition-report" data-toggle-trigger-label=""></a>
									<div class="btn-group dashboard-section-header-period">
										<div class="btn-group-title">期間</div>
										<div class="btn-group-body">
											<a href="javascript:void(0)" onclick="getAndSetChartReportsByPeriodKbn(this, '<%: Constants.FLG_SUMMARYREPORT_PERIOD_KBN_LAST_SEVEN_DAYS %>')" class="btn-group-btn chart-firt-selected is-current">過去7日間</a>
											<a href="javascript:void(0)" onclick="getAndSetChartReportsByPeriodKbn(this, '<%: Constants.FLG_SUMMARYREPORT_PERIOD_KBN_THIS_MONTH %>')" class="btn-group-btn">今月</a>
											<a href="javascript:void(0)" onclick="getAndSetChartReportsByPeriodKbn(this, '<%: Constants.FLG_SUMMARYREPORT_PERIOD_KBN_LAST_MONTH %>')" class="btn-group-btn">先月</a>
											<a href="javascript:void(0)" onclick="getAndSetChartReportsByPeriodKbn(this, '<%: Constants.FLG_SUMMARYREPORT_PERIOD_KBN_THIS_YEAR %>')" class="btn-group-btn" id="thisYear">今年</a>
										</div>
									</div>
								</div>
								<div class="dashboard-section-body dashboard-section-body-transition-report">
									<div class="dashboard-transition-report">
									</div>
								</div>
							</div>
							<span id="lastMonthLtv" style="visibility:hidden"></span>
						</td>
						<!-- // 推移レポート -->
					</tr>
					<tr>
						<!-- ランキング -->
						<td>
							<asp:UpdatePanel ID="upRanking" runat="server" >
								<ContentTemplate>
									<div class="dashboard-section">
										<div class="dashboard-section-header">
											<h2 class="dashboard-section-title">ランキング</h2>
											<a class="btn-toggle dashboard-section-toggle-btn" data-toggle-content-selector=".dashboard-section-body-ranking" data-toggle-trigger-label=""></a>
											<div class="btn-group dashboard-section-header-period">
												<div class="btn-group-title">期間</div>
												<div class="btn-group-body">
													<asp:LinkButton ID="lbRankingPeriodYesterday" Text="  昨日  " CssClass="btn-group-btn is-current" runat="server" OnClick="lbRankingPeriod_Click" />
													<asp:LinkButton ID="lbRankingPeriodLast7Days" Text="  過去7日間  " CssClass="btn-group-btn" runat="server" OnClick="lbRankingPeriod_Click" />
													<asp:LinkButton ID="lbRankingPeriodThisMonth" Text="  今月  " CssClass="btn-group-btn" runat="server" OnClick="lbRankingPeriod_Click" />
													<asp:LinkButton ID="lbRankingPeriodLastMonth" Text="  先月  " CssClass="btn-group-btn" runat="server" OnClick="lbRankingPeriod_Click" />
													<asp:LinkButton ID="lbRankingPeriodThisYear" Text="  今年  " CssClass="btn-group-btn" runat="server" OnClick="lbRankingPeriod_Click" />
												</div>
											</div>
										</div>
										<div class="dashboard-section-body dashboard-section-body-ranking">
											<div id="dashboard-ranking-affiliate-order-count" class="dashboard-box dashboard-ranking dashboard-ranking-affiliate">
												<div class="dashboard-ranking-header">
													<h3 id="hAdvertisingCodeOrderRankingTitle" runat="server" class="dashboard-ranking-header-title">広告コード注文数ランキング（昨日）</h3>
													<div class="dashboard-ranking-header-switch">
														<div class="btn-group-body">
															<asp:LinkButton ID="lbAdvCodeOrderRankingOrderByNumber" OnClick="lbAdvCodeOrderRankingOrderByKbn_Click" CssClass="btn-group-btn btn-smartphone is-current" runat="server">注文数</asp:LinkButton>
															<asp:LinkButton ID="lbAdvCodeOrderRankingOrderByAmount" OnClick="lbAdvCodeOrderRankingOrderByKbn_Click" CssClass="btn-group-btn btn-smartphone" runat="server">注文金額</asp:LinkButton>
														</div>
													</div>
												</div>
												<div class="dashboard-ranking-body">
													<table class="dashboard-ranking-table">
														<asp:Repeater ID="rAdvCodeOrderRankingList" ItemType="w2.Domain.AdvCode.Helper.AdvCodeListForReport" runat="server">
															<HeaderTemplate>
																<tr>
																	<th class="ta-left"></th>
																	<th class="ta-left">広告コード（広告媒体区分）</th>
																	<th class="ta-right">注文数</th>
																	<th class="ta-right">注文金額</th>
																</tr>
															</HeaderTemplate>
															<ItemTemplate>
																<tr>
																	<td class='rank-num <%# GetHighLightCss(Container.ItemIndex) %>'>
																		<%#: (Container.ItemIndex + 1) %>
																	</td>
																	<td class='ta-left adv-code-media <%# GetHighLightCss(Container.ItemIndex) %>'>
																		<span class="adv-code-name"><%#: Item.AdvertisementCode %></span>
																		<span class="icon-popup-detail-sp">
																			<span class="affiliate-name" style="<%# (string.IsNullOrEmpty(Item.AdvcodeMediaTypeName) ? "visibility: hidden;" : string.Empty) %>">
																				<%#: Item.AdvcodeMediaTypeName %>
																			</span>
																			<a href="#" onclick ='javascript:open_window("<%#: SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Mp, CreateAdvCodeDetailUrl(Item.AdvertisementCode)) %>", "search_adv_code", "width=1200,height=850,top=110,left=380,status=NO,scrollbars=yes,resizable=yes");'>
																				<span class="icon icon-arrow-out"></span>
																			</a>
																		</span>
																	</td>
																	<td class='ta-right order-quantity <%# GetOrderByNumberHighLightCss(Container.ItemIndex, this.AdvertisingCodeOrderByKbn) %>'>
																		<%#: StringUtility.ToNumeric(Item.TotalOrderCount) %>
																		<span class="rate">(<%#: Item.OrderCountRate %>%)</span>
																	</td>
																	<td class='ta-right order-price <%# GetOrderByAmountHighLightCss(Container.ItemIndex, this.AdvertisingCodeOrderByKbn) %>'>
																		<%#: StringUtility.ToNumeric(Item.TotalOrderAmount) %>
																		<span class="rate">(<%#: Item.OrderAmountRate %>%)</span>
																	</td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
													</table>
												</div>
											</div>
											<div id="dashboard-ranking-item-sale-order-count" class="dashboard-box dashboard-ranking dashboard-ranking-item-sale">
												<div class="dashboard-ranking-header">
													<h3 id="hProductSalesRankingTitle" runat="server" class="dashboard-ranking-header-title">商品販売数ランキング（昨日）</h3>
													<div class="dashboard-ranking-header-switch">
														<div class="dashboard-ranking-header-switch-valiation">
															<asp:CheckBox ID="cbByVariation" runat="server" Text="バリエーション別" AutoPostBack="True" OnCheckedChanged="cbByVariation_CheckedChanged" />
														</div>
														<div class="btn-group-body">
															<asp:LinkButton ID="lbProductSalesRankingOrderByNumber" OnClick="lbProductSalesRankingOrderByKbn_Click" CssClass="btn-group-btn btn-smartphone is-current" runat="server">販売数</asp:LinkButton>
															<asp:LinkButton ID="lbProductSalesRankingOrderByAmount" OnClick="lbProductSalesRankingOrderByKbn_Click" CssClass="btn-group-btn btn-smartphone" runat="server">販売金額</asp:LinkButton>
														</div>
													</div>
												</div>
												<div class="dashboard-ranking-body">
													<table class="dashboard-ranking-table">
														<asp:Repeater ID="rProductSalesRankingList" ItemType="w2.Domain.Product.Helper.ProductListForReport" runat="server">
															<HeaderTemplate>
															<tr>
																<th></th>
																<th></th>
																<th class="ta-left">商品ID</th>
																<th class="ta-left">商品名</th>
																<th class="ta-right">販売数</th>
																<th class="ta-right">販売金額</th>
															</tr>
															</HeaderTemplate>
															<ItemTemplate>
																<tr>
																	<td class='rank-num <%# GetHighLightCss(Container.ItemIndex) %>'>
																		<%#: (Container.ItemIndex + 1) %>
																	</td>
																	<td class='item-img <%# GetHighLightCss(Container.ItemIndex) %>'>
																		<%# ProductImage.GetHtmlImageTag(Item, ProductType.Product, SiteType.Pc, Constants.PRODUCTIMAGE_FOOTER_M) %>
																	</td>
																	<td class='ta-left product-id <%# GetHighLightCss(Container.ItemIndex) %>'>
																		<%#: Item.ProductId %>
																	</td>
																	<td class='ta-left product-item-name <%# GetHighLightCss(Container.ItemIndex) %>'>
																		<span class="product-name"><%#: ProductCommon.CreateProductJointName(Item) %></span>
																		<a href='javascript:open_window("<%#: CreateProductDetailUrl(Item.ProductId, true) %>", "search_product_detail", "width=1200,height=850,top=110,left=380,status=NO,scrollbars=yes,resizable=yes");'>
																			<span class="icon icon-arrow-out"></span>
																		</a>
																	</td>
																	<td class='ta-right sale-quantity <%# GetOrderByNumberHighLightCss(Container.ItemIndex ,this.ProductSalesOrderByKbn) %>'>
																		<%#: StringUtility.ToNumeric(Item.TotalProductCount) %>
																	</td>
																	<td class='ta-right sale-price <%# GetOrderByAmountHighLightCss(Container.ItemIndex ,this.ProductSalesOrderByKbn) %>'>
																		<%#: Item.TotalOrderAmount.ToPriceString(true) %>
																	</td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
														<asp:Repeater ID="rProductVariationSalesRankingList" ItemType="w2.Domain.Product.Helper.ProductListForReport" Visible="false" runat="server">
															<HeaderTemplate>
																<tr>
																	<th></th>
																	<th></th>
																	<th class="ta-left">商品バリエーションID</th>
																	<th class="ta-left">商品名</th>
																	<th class="ta-right">販売数</th>
																	<th class="ta-right">販売金額</th>
																</tr>
															</HeaderTemplate>
															<ItemTemplate>
																<tr>
																	<td class='rank-num <%# GetHighLightCss(Container.ItemIndex) %>'>
																		<%#: (Container.ItemIndex + 1) %>
																	</td>
																	<td class='item-img <%# GetHighLightCss(Container.ItemIndex) %>'>
																		<%# ProductImage.GetHtmlImageTag(Container.DataItem, ProductType.Variation, SiteType.Pc, Constants.PRODUCTIMAGE_FOOTER_M) %>
																	</td>
																	<td class='ta-left variation-item-id <%# GetHighLightCss(Container.ItemIndex) %>'>
																		<span class="variation-id"><%#: Item.VariationId %></span>
																	</td>
																	<td class='ta-left variation-item-name <%# GetHighLightCss(Container.ItemIndex) %>'>
																		<span class="variation-name"><%#: ProductCommon.CreateProductJointName(Item) %></span>
																		<a href='javascript:open_window("<%#: CreateProductDetailUrl(Item.ProductId, true) %>", "search_product_detail", "width=1200,height=850,top=110,left=380,status=NO,scrollbars=yes,resizable=yes");'>
																			<span class="icon icon-arrow-out"></span>
																		</a>
																	</td>
																	<td class='ta-right <%# GetOrderByNumberHighLightCss(Container.ItemIndex, this.ProductSalesOrderByKbn) %>'>
																		<%#: StringUtility.ToNumeric(Item.TotalProducVariationtCount) %>
																	</td>
																	<td class='ta-right <%# GetOrderByAmountHighLightCss(Container.ItemIndex, this.ProductSalesOrderByKbn) %>'>
																		<%#: Item.TotalOrderAmount.ToPriceString(true) %>
																	</td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
													</table>
												</div>
											</div>
										</div>
									</div>
								</ContentTemplate>
							</asp:UpdatePanel>
						</td>
						<!-- // ランキング -->
					</tr>
					<tr>
						<!-- 注意書き -->
						<td>
							<p>※上記は注文時の金額で集計されています</p>
							<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED){ %>
							<p>※定額頒布会の場合、商品販売数ランキング・広告コード注文数ランキングは商品価格で計算されます</p>
							<% } %>
						</td>
					</tr>
					<tr>
						<!-- 注文状況 -->
						<td>
							<div class="dashboard-section dashboard-section-order-report">
								<div class="dashboard-section-header">
									<h2 class="dashboard-section-title">注文状況</h2>
									<a class="btn-toggle dashboard-section-toggle-btn" data-toggle-content-selector=".dashboard-section-body-shipment-report" data-toggle-trigger-label=""></a>
								</div>
								<div class="dashboard-section-body dashboard-section-body-shipment-report">
									<%--▽ 出荷予定日オプションが有効な場合は表示 ▽--%>
									<% if (this.UseLeadTime) { %>
									<div class="dashboard-status">
										<a id="aUnshippedCount" href='javascript:open_window("<%: CreateOrderListUrlForSearchOrderShippedStatus(SummaryReportModel.ORDERSHIPPEDSTATUS_REPORT_UNSHIPPED) %>", "search_order_shipping_status", "width=1200,height=850,top=110,left=380,status=NO,scrollbars=yes,resizable=yes");'
											class="dashboard-box dashboard-status-box">
											<h3 class="dashboard-status-box-title">
												<span class="dashboard-status-box-title-strong">出荷予定日を過ぎた</span>未出荷注文
											</h3>
											<div class="dashboard-status-box-value">
												<span id="sUnshippedCount" class="dashboard-status-box-value-text disp-max-content disp-max-content-sp">
													<div class="loader"></div>
												</span>
												<span id="sUnshippedCountUnit" class="dashboard-status-box-value-unit">件</span>
											</div>
										</a>
										<a href='javascript:open_window("<%: CreateOrderListUrlForSearchOrderShippedStatus(SummaryReportModel.ORDERSHIPPEDSTATUS_REPORT_TODAY) %>", "search_order_shipping_status", "width=1200,height=850,top=110,left=380,status=NO,scrollbars=yes,resizable=yes");'
											class="dashboard-box dashboard-status-box">
											<h3 class="dashboard-status-box-title">
												<span class="dashboard-status-box-title-strong">今日出荷予定日</span>の未出荷注文
											</h3>
											<div class="dashboard-status-box-value">
												<span id="sShippedTodayCount" class="dashboard-status-box-value-text disp-max-content disp-max-content-sp">
													<div class="loader"></div>
												</span>
												<span id="sShippedTodayCountUnit" class="dashboard-status-box-value-unit">件</span>
											</div>
										</a>
										<a href='javascript:open_window("<%: CreateOrderListUrlForSearchOrderShippedStatus(SummaryReportModel.ORDERSHIPPEDSTATUS_REPORT_FUTURE) %>", "search_order_shipping_status", "width=1200,height=850,top=110,left=380,status=NO,scrollbars=yes,resizable=yes");'
											class="dashboard-box dashboard-status-box">
											<h3 class="dashboard-status-box-title">
												<span class="dashboard-status-box-title-strong">明日以降出荷予定日</span>の未出荷注文
											</h3>
											<div class="dashboard-status-box-value">
												<span id="sShippedFutureCount" class="dashboard-status-box-value-text disp-max-content disp-max-content-sp">
													<div class="loader"></div>
												</span>
												<span id="sShippedFutureCountUnit" class="dashboard-status-box-value-unit">件</span>
											</div>
										</a>
									</div>
									<% } %>
									<%--△ 出荷予定日オプションが有効な場合は表示 △--%>
									<button id="nextSlideOrderStatus" type="button" class="image-right-nav" onclick="plusSlideOrderStatus(1)">
										<span class="toggle-state-icon icon-arrow-down" style="transform: rotate(-90deg);"></span>
									</button>
									<button id="prevSlideOrderStatus" type="button" class="image-left-nav" onclick="plusSlideOrderStatus(-1)">
										<span class="toggle-state-icon icon-arrow-down" style="transform: rotate(90deg);"></span>
									</button>
									<div class="dashboard-status-row dashboard-status-row-order-status dashboard-status-row-order-status-slider">
										<a href='javascript:open_window("<%: CreateOrderListUrlForSearchOrderStatus(Constants.FLG_ORDER_ORDER_STATUS_TEMP) %>", "search_order_status", "width=1200,height=850,top=110,left=380,status=NO,scrollbars=yes,resizable=yes");'
											class="dashboard-status-row-box dashboard-status-row-box-1 dashboard-status-row-order-status-slide <%: this.OrderStatusReport.HasOrderStatusTempCount ? "is-alert": string.Empty %>">
											<div class="dashboard-status-row-box-icon">
												<img src="<%: Constants.PATH_ROOT %>Images/dashboard/icon-order-status-1.png" alt="" />
											</div>
											<h3 class="dashboard-status-row-box-title">仮注文</h3>
											<div class="dashboard-status-row-box-value">
												<span class="dashboard-status-row-box-value-text disp-max-content"><%: StringUtility.ToNumeric(this.OrderStatusReport.OrderStatusTempCount) %></span>
												<span class="dashboard-status-row-box-value-unit">件</span>
											</div>
										</a>
										<a href='javascript:open_window("<%: CreateOrderListUrlForSearchOrderStatus(Constants.FLG_ORDER_ORDER_STATUS_ORDERED) %>", "search_order_status", "width=1200,height=850,top=110,left=380,status=NO,scrollbars=yes,resizable=yes");'
											class="dashboard-status-row-box dashboard-status-row-box-2 dashboard-status-row-order-status-slide">
											<div class="dashboard-status-row-box-icon">
												<img src="<%: Constants.PATH_ROOT %>Images/dashboard/icon-order-status-2.png" alt="" />
											</div>
											<h3 class="dashboard-status-row-box-title">注文済み</h3>
											<div class="dashboard-status-row-box-value">
												<span class="dashboard-status-row-box-value-text disp-max-content"><%: StringUtility.ToNumeric(this.OrderStatusReport.OrderStatusOrderedCount) %></span>
												<span class="dashboard-status-row-box-value-unit">件</span>
											</div>
										</a>
										<a href='javascript:open_window("<%: CreateOrderListUrlForSearchOrderStatus(Constants.FLG_ORDER_ORDER_STATUS_ORDER_RECOGNIZED) %>", "search_order_status", "width=1200,height=850,top=110,left=380,status=NO,scrollbars=yes,resizable=yes");'
											class="dashboard-status-row-box dashboard-status-row-box-3 dashboard-status-row-order-status-slide">
											<div class="dashboard-status-row-box-icon">
												<img src="<%: Constants.PATH_ROOT %>Images/dashboard/icon-order-status-3.png" alt="" />
											</div>
											<h3 class="dashboard-status-row-box-title">受注承認</h3>
											<div class="dashboard-status-row-box-value">
												<span class="dashboard-status-row-box-value-text disp-max-content"><%: StringUtility.ToNumeric(this.OrderStatusReport.OrderStatusRecognizedCount) %></span>
												<span class="dashboard-status-row-box-value-unit">件</span>
											</div>
										</a>
										<a href='javascript:open_window("<%: CreateOrderListUrlForSearchOrderStatus(Constants.FLG_ORDER_ORDER_STATUS_STOCK_RESERVED) %>", "search_order_status", "width=1200,height=850,top=110,left=380,status=NO,scrollbars=yes,resizable=yes");'
											class="dashboard-status-row-box dashboard-status-row-box-4 dashboard-status-row-order-status-slide">
											<div class="dashboard-status-row-box-icon">
												<img src="<%: Constants.PATH_ROOT %>Images/dashboard/icon-order-status-4.png" alt="" />
											</div>
											<h3 class="dashboard-status-row-box-title">在庫引当済み</h3>
											<div class="dashboard-status-row-box-value">
												<span class="dashboard-status-row-box-value-text disp-max-content"><%: StringUtility.ToNumeric(this.OrderStatusReport.OrderStatusStockReservedCount) %></span>
												<span class="dashboard-status-row-box-value-unit">件</span>
											</div>
										</a>
										<a href='javascript:open_window("<%: CreateOrderListUrlForSearchOrderStatus(Constants.FLG_ORDER_ORDER_STATUS_SHIP_ARRANGED) %>", "search_order_status", "width=1200,height=850,top=110,left=380,status=NO,scrollbars=yes,resizable=yes");'
											class="dashboard-status-row-box dashboard-status-row-box-5 dashboard-status-row-order-status-slide">
											<div class="dashboard-status-row-box-icon">
												<img src="<%: Constants.PATH_ROOT %>Images/dashboard/icon-order-status-5.png" alt="" />
											</div>
											<h3 class="dashboard-status-row-box-title">出荷手配済み</h3>
											<div class="dashboard-status-row-box-value">
												<span class="dashboard-status-row-box-value-text disp-max-content"><%: StringUtility.ToNumeric(this.OrderStatusReport.OrderStatusShipArrangedCount) %></span>
												<span class="dashboard-status-row-box-value-unit">件</span>
											</div>
										</a>
										<a href='javascript:open_window("<%: CreateOrderListUrlForSearchOrderStatus(Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP) %>", "search_order_status", "width=1200,height=850,top=110,left=380,status=NO,scrollbars=yes,resizable=yes");'
											class="dashboard-status-row-box dashboard-status-row-box-6 dashboard-status-row-order-status-slide">
											<div class="dashboard-status-row-box-icon">
												<img src="<%: Constants.PATH_ROOT %>Images/dashboard/icon-order-status-6.png" alt="" />
											</div>
											<h3 class="dashboard-status-row-box-title">出荷完了（今月）</h3>
											<div class="dashboard-status-row-box-value">
												<span class="dashboard-status-row-box-value-text disp-max-content"><%: StringUtility.ToNumeric(this.OrderStatusReport.OrderStatusShipCompleteCount) %></span>
												<span class="dashboard-status-row-box-value-unit">件</span>
											</div>
										</a>
									</div>
									<% if (Constants.ORDERWORKFLOW_OPTION_ENABLED) { %>
									<div class="dashboard-list dashboard-order-workflow-report">
										<div class="dashboard-list-header">
											<h3 class="dashboard-list-header-title">受注ワークフロー状況</h3>
										</div>
										<div class="dashboard-list-body dashboard-section-order-workflow-report">
											<ul id="ulOrderWorkflowList" class="dashboard-list-ul"></ul>
											<div id="dOrderWorkflowListLoading" class="dashboard-list-li order-workflow-loading">
												<div>
													<div class="loader"></div>
												</div>
											</div>
										</div>
									</div>
									<% } %>
								</div>
							</div>
						</td>
						<!-- // 注文状況 -->
					</tr>
					<% if (Constants.CS_OPTION_ENABLED || Constants.PRODUCT_STOCK_OPTION_ENABLE) { %>
					<tr>
						<!-- その他の状況 -->
						<td>
							<div class="dashboard-section dashboard-section-other-report">
								<div class="dashboard-section-header">
									<h2 class="dashboard-section-title">その他の状況</h2>
									<a class="btn-toggle dashboard-section-toggle-btn" data-toggle-content-selector=".dashboard-section-body-other-report" data-toggle-trigger-label=""></a>
								</div>
								<div class="dashboard-section-body dashboard-section-body-other-report">
									<% if (Constants.CS_OPTION_ENABLED) { %>
									<h3 class="h3">お問い合わせ状況</h3>
									<button id="nextSlideIncidentStatus" type="button" class="image-right-nav" onclick="plusSlideIncidentStatus(1)">
										<span class="toggle-state-icon icon-arrow-down" style="transform: rotate(-90deg);"></span>
									</button>
									<button id="prevSlideIncidentStatus" type="button" class="image-left-nav" onclick="plusSlideIncidentStatus(-1)">
										<span class="toggle-state-icon icon-arrow-down" style="transform: rotate(90deg);"></span>
									</button>
									<div class="dashboard-status-row dashboard-status-row-incident-status dashboard-status-row-incident-status-slider">
										<a href="#" onclick='javascript:open_window("<%: SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Cs, CreateIncidentDetailUrl(Constants.CONST_CSINCIDENT_STATUS_NONE)) %>", "search_incident_status", "width=1200,height=850,top=110,left=380,status=NO,scrollbars=yes,resizable=yes");'
											class="dashboard-status-row-box dashboard-status-row-box-1 dashboard-status-row-incident-status-slide">
											<div class="dashboard-status-row-box-icon">
												<img src="<%: Constants.PATH_ROOT %>Images/dashboard/icon-incident-status-1.png" alt="" />
											</div>
											<h3 class="dashboard-status-row-box-title">未対応</h3>
											<div class="dashboard-status-row-box-value">
												<span class="dashboard-status-row-box-value-text disp-max-content"><%: StringUtility.ToNumeric(this.IncidentReport.StatusNoneCount) %></span>
												<span class="dashboard-status-row-box-value-unit">件</span>
											</div>
										</a>
										<a href="#" onclick='javascript:open_window("<%: SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Cs, CreateIncidentDetailUrl(Constants.CONST_CSINCIDENT_STATUS_ACTIVE)) %>", "search_incident_status", "width=1200,height=850,top=110,left=380,status=NO,scrollbars=yes,resizable=yes");'
											class="dashboard-status-row-box dashboard-status-row-box-2 dashboard-status-row-incident-status-slide">
											<div class="dashboard-status-row-box-icon">
												<img src="<%: Constants.PATH_ROOT %>Images/dashboard/icon-incident-status-2.png" alt="" />
											</div>
											<h3 class="dashboard-status-row-box-title">対応中</h3>
											<div class="dashboard-status-row-box-value">
												<span class="dashboard-status-row-box-value-text disp-max-content"><%: StringUtility.ToNumeric(this.IncidentReport.StatusActiveCount) %></span>
												<span class="dashboard-status-row-box-value-unit">件</span>
											</div>
										</a>
										<a href="#" onclick='javascript:open_window("<%: SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Cs, CreateIncidentDetailUrl(Constants.CONST_CSINCIDENT_STATUS_SUSPEND)) %>", "search_incident_status", "width=1200,height=850,top=110,left=380,status=NO,scrollbars=yes,resizable=yes");'
											class="dashboard-status-row-box dashboard-status-row-box-3 dashboard-status-row-incident-status-slide dashboard-status-row-box-noarrow">
											<div class="dashboard-status-row-box-icon">
												<img src="<%: Constants.PATH_ROOT %>Images/dashboard/icon-incident-status-3.png" alt="" />
											</div>
											<h3 class="dashboard-status-row-box-title">保留</h3>
											<div class="dashboard-status-row-box-value">
												<span class="dashboard-status-row-box-value-text disp-max-content"><%: StringUtility.ToNumeric(this.IncidentReport.StatusSuspendCount) %></span>
												<span class="dashboard-status-row-box-value-unit">件</span>
											</div>
										</a>
										<a href="#" onclick='javascript:open_window("<%: SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Cs, CreateIncidentDetailUrl(Constants.CONST_CSINCIDENT_STATUS_URGENT)) %>", "search_incident_status", "width=1200,height=850,top=110,left=380,status=NO,scrollbars=yes,resizable=yes");'
											class="dashboard-status-row-box dashboard-status-row-box-4 dashboard-status-row-incident-status-slide dashboard-status-row-box-noarrow <%: this.IncidentReport.HasStatusUrgentCount ? "is-alert" : string.Empty %>">
											<div class="dashboard-status-row-box-icon">
												<img src="<%: Constants.PATH_ROOT %>Images/dashboard/icon-incident-status-4.png" alt="" />
											</div>
											<h3 class="dashboard-status-row-box-title">至急</h3>
											<div class="dashboard-status-row-box-value">
												<span class="dashboard-status-row-box-value-text disp-max-content"><%: StringUtility.ToNumeric(this.IncidentReport.StatusUrgentCount) %></span>
												<span class="dashboard-status-row-box-value-unit">件</span>
											</div>
										</a>
										<a href="#" onclick='javascript:open_window("<%: SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Cs, CreateIncidentDetailUrl(Constants.CONST_CSINCIDENT_STATUS_COMPLETE)) %>", "search_incident_status", "width=1200,height=850,top=110,left=380,status=NO,scrollbars=yes,resizable=yes");'
											class="dashboard-status-row-box dashboard-status-row-box-5 dashboard-status-row-incident-status-slide">
											<div class="dashboard-status-row-box-icon">
												<img src="<%: Constants.PATH_ROOT %>Images/dashboard/icon-incident-status-5.png" alt="" />
											</div>
											<h3 class="dashboard-status-row-box-title">完了</h3>
											<div class="dashboard-status-row-box-value">
												<span class="dashboard-status-row-box-value-text disp-max-content"><%: StringUtility.ToNumeric(this.IncidentReport.StatusCompleteCount) %></span>
												<span class="dashboard-status-row-box-value-unit">件</span>
											</div>
										</a>
									</div>
									<% } %>
									<% if (Constants.PRODUCT_STOCK_OPTION_ENABLE) { %>
									<h3 class="h3">商品在庫状況</h3>
									<div class="dashboard-status">
										<a href='javascript:open_window("<%: CreateProductStockDetailUrl(SummaryReportModel.PRODUCTSTOCK_REPORT_ALERT_KBN_SAFETY_STOCK_ALERT) %>", "search_product_stock_status", "width=1200,height=850,top=110,left=380,status=NO,scrollbars=yes,resizable=yes");'
											class="dashboard-box dashboard-status-box <%: this.ProductStockReport.HasStockSafetyAlertCount ? "dashboard-status-box-is-alert is-alert" : string.Empty %>">
											<h3 class="dashboard-status-box-title"><span class="dashboard-status-box-title-strong">安全在庫アラート</span></h3>
											<div class="dashboard-status-box-value">
												<span class="dashboard-status-box-value-text disp-max-content disp-max-content-sp"><%: StringUtility.ToNumeric(this.ProductStockReport.StockSafetyAlertCount) %></span>
												<span class="dashboard-status-box-value-unit">件</span>
											</div>
										</a>
										<a href='javascript:open_window("<%: CreateProductStockDetailUrl(SummaryReportModel.PRODUCTSTOCK_REPORT_ALERT_KBN_OUT_OF_STOCK) %>", "search_product_stock_status", "width=1200,height=850,top=110,left=380,status=NO,scrollbars=yes,resizable=yes");'
											class="dashboard-box dashboard-status-box">
											<h3 class="dashboard-status-box-title"><span class="dashboard-status-box-title-strong">在庫切れ販売停止</span></h3>
											<div class="dashboard-status-box-value">
												<span class="dashboard-status-box-value-text disp-max-content disp-max-content-sp"><%: StringUtility.ToNumeric(this.ProductStockReport.StockOutOfStockCount) %></span>
												<span class="dashboard-status-box-value-unit">件</span>
											</div>
										</a>
										<a href='javascript:open_window("<%: CreateProductStockDetailUrl(SummaryReportModel.PRODUCTSTOCK_REPORT_ALERT_KBN_ALL) %>", "search_product_stock_status", "width=1200,height=850,top=110,left=380,status=NO,scrollbars=yes,resizable=yes");'
											class="dashboard-box dashboard-status-box">
											<h3 class="dashboard-status-box-title"><span class="dashboard-status-box-title-strong">在庫売り上げ</span></h3>
											<div class="dashboard-status-box-value">
												<span class="dashboard-status-box-value-text disp-max-content disp-max-content-sp"><%: StringUtility.ToNumeric(this.ProductStockReport.StockPriceTotal.ToString("0")) %></span>
												<span class="dashboard-status-box-value-unit">円</span>
											</div>
										</a>
									</div>
									<div class="dashboard-status dashboard-status-text">
										<p>※在庫売り上げについて<br /><span style="padding-left: 15px;">すべての商品の在庫残数に、商品単価を乗算したもの</span></p>
									</div>
									<% } %>
									<% if (Constants.MARKETINGPLANNER_MAIL_OPTION_ENABLE && Constants.MP_OPTION_ENABLED)
									   { %>
										<h3 class="h3">メール配信状況</h3>
										<div class="dashboard-status">
											<div class="dashboard-box dashboard-box-mp">
												<h3 class="dashboard-status-box-title"><span class="dashboard-status-box-title-strong">総数</span></h3>
												<div class="dashboard-status-box-value">
													<span id="spanMailSentThisMonth" class="dashboard-status-box-value-text disp-max-content"><%: StringUtility.ToNumeric(this.TaskScheduleHistory.SendMailCount_ThisMonth) %></span>
													<span class="dashboard-status-box-value-unit">件</span>
												</div>
												<span class="dashboard-box-mp-value">先月</span>
												<span id="spanMailSentLastMonth" class="dashboard-box-mp-value">(<%: StringUtility.ToNumeric(this.TaskScheduleHistory.SendMailCount_LastMonth) %>件)</span>
											</div>
											<div class="dashboard-box dashboard-box-mp">
												<h3 class="dashboard-status-box-title"><span class="dashboard-status-box-title-strong">URLクリック数</span></h3>
												<div class="dashboard-status-box-value">
													<span id="spanURLClickThisMonth" class="dashboard-status-box-value-text disp-max-content"><%: StringUtility.ToNumeric(this.TaskScheduleHistory.MailClickCount_ThisMonth) %></span>
													<span class="dashboard-status-box-value-unit">件</span>
												</div>
												<span class="dashboard-box-mp-value">先月</span>
												<span id="spanURLClickLastMonth" class="dashboard-box-mp-value">(<%: StringUtility.ToNumeric(this.TaskScheduleHistory.MailClickCount_LastMonth) %>件)</span>
											</div>
											<div class="dashboard-box dashboard-box-mp">
												<h3 class="dashboard-status-box-title"><span class="dashboard-status-box-title-strong">クリック率</span></h3>
												<div class="dashboard-status-box-value">
													<span id="spanURLClickRateThisMonth" class="dashboard-status-box-value-text disp-max-content"><%: StringUtility.ToNumeric(this.TaskScheduleHistory.ClickRate_ThisMonth) %></span>
													<span class="dashboard-status-box-value-unit">％</span>
												</div>
												<span class="dashboard-box-mp-value">先月</span>
												<span id="spanURLClickRateLastMonth" class="dashboard-box-mp-value">(<%: StringUtility.ToNumeric(this.TaskScheduleHistory.ClickRate_LastMonth) %>％)</span>
											</div>
										</div>
										<div class="dashboard-status dashboard-status-text">
												<p>※集計対象<br /><span style="padding-left: 15px;">メール配信設定からの配信</span></p>
											</div>
									<% } %>

								</div>
							</div>
						</td>
					</tr>
					<% } %>
				</table>
			</td>
		</tr>
		<tr>
			<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
		</tr>
	</table>
	<script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.7.2/Chart.bundle.js"></script>
	<script type="text/javascript" src="<%: this.ResolveClientUrl("~/Js/Manager.dashboard.js") %>"></script>
	<script type="text/javascript">

		// Chart data array display
		var chartDataArray = [];

		// Sale goal progress data
		var data_goal_progress = eval(<%= this.CurrentSalesRevenue %>);

		$(document).ready(function () {
			getShippedReport();

			<% if (Constants.ORDERWORKFLOW_OPTION_ENABLED) { %>
			getOrderWorkflowReport();
			<% } %>

			getAndSetChartReportsByPeriodKbn($('.chart-firt-selected'), '<%: Constants.FLG_SUMMARYREPORT_PERIOD_KBN_LAST_SEVEN_DAYS %>');

			// For case the current browser is IE
			if (isIeBrowser()) {
				bindChartReportEvent();
			}

			// Load button slide
			loadDefaultButtonSlideOrderStatus();
			loadDefaultButtonSlideIncidentStatus();

			// What if screen size is less than 1024px already so call this funciton at starting the page
			sizeChanged();

			// Now set this function as width change handler
			$(window).resize(function () {
				sizeChanged();
			});

			var containerOrderStatusSlider = document.querySelector(".dashboard-status-row-order-status-slider");
			containerOrderStatusSlider.addEventListener("touchstart", startTouchOrderStatusSlider, false);
			containerOrderStatusSlider.addEventListener("scroll", moveTouchOrderStatusSlider, false);
			containerOrderStatusSlider.addEventListener("touchmove", displayTouchOrderStatusSlider, false);

			var containerIncidentStatusSlider = document.querySelector(".dashboard-status-row-incident-status-slider");
			containerIncidentStatusSlider.addEventListener("touchstart", startTouchIncidentStatusSlider, false);
			containerIncidentStatusSlider.addEventListener("scroll", moveTouchIncidentStatusSlider, false);
			containerIncidentStatusSlider.addEventListener("touchmove", displayTouchIncidentStatusSlider, false);
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
				displayWorkflowTargetOrderCount.clear();
				abort();
			}
		})(jQuery);

		// Size changed
		function sizeChanged() {
			var width = $(window).width();
			if (width < 1025) {
				// Less than 1025 so remove href with javascript:(0)
				$('.dashboard-list-li').find('a').each(function () {
					var old = $(this).attr('href');
					if (old != 'javascript:(0)') $(this).attr('data-hrf', old);
					$(this).attr('href', 'javascript:(0)');
				});

				$('#btn-configuration').attr('class', 'dashboard-info-link btn btn-main btn-size-ml btn-modal-open')

				// Show button slide
				$('#nextSlideOrderStatus').css('display', 'inline');
				$('#prevSlideOrderStatus').css('display', 'none');
				$('#nextSlideIncidentStatus').css('display', 'inline');
				$('#prevSlideIncidentStatus').css('display', 'none');
			} else {
				// More then 1025 so restore
				$('.dashboard-list-li').find('a').each(function () {
					var old = $(this).attr('data-hrf');
					if (old != "") $(this).attr('href', old);
				});

				$('#btn-configuration').attr('class', 'dashboard-info-link btn btn-txt btn-size-m btn-modal-open')

				// Hide button slide
				$('#nextSlideOrderStatus').hide();
				$('#prevSlideOrderStatus').hide();
				$('#nextSlideIncidentStatus').hide();
				$('#prevSlideIncidentStatus').hide();
			}
		}

		// Open sale goal setting
		function openSaleGoalSetting() {
			$.ajax({
				type: "POST",
				url: "<%: this.SummaryRootUrl %>/OpenSaleGoalSetting",
				contentType: "application/json; charset=utf-8",
				dataType: "json",
				success: function (response) {
					if ((response == null) || (response.d == undefined)) return;

					// Display sale goal setting. If sale goal has not been set display default values.
					var data = JSON.parse(response.d);
					document.getElementById("<%= tbAnnualGoal.ClientID %>").value = data.model.AnnualGoal;
					document.getElementById("<%= ddlApplicableMonth.ClientID %>").value = data.model.ApplicableMonth;
					document.getElementById("<%= lbErrorMessage.ClientID %>").innerText = '';
					document.getElementById("<%= lbErrorMessage.ClientID %>").style.display = 'none';
					displaySaleGoalInput();
					$("#tbMonthlyGoal1").val(data.model.MonthlyGoal1);
					$("#tbMonthlyGoal2").val(data.model.MonthlyGoal2);
					$("#tbMonthlyGoal3").val(data.model.MonthlyGoal3);
					$("#tbMonthlyGoal4").val(data.model.MonthlyGoal4);
					$("#tbMonthlyGoal5").val(data.model.MonthlyGoal5);
					$("#tbMonthlyGoal6").val(data.model.MonthlyGoal6);
					$("#tbMonthlyGoal7").val(data.model.MonthlyGoal7);
					$("#tbMonthlyGoal8").val(data.model.MonthlyGoal8);
					$("#tbMonthlyGoal9").val(data.model.MonthlyGoal9);
					$("#tbMonthlyGoal10").val(data.model.MonthlyGoal10);
					$("#tbMonthlyGoal11").val(data.model.MonthlyGoal11);
					$("#tbMonthlyGoal12").val(data.model.MonthlyGoal12);
				},
				error: pageReload
			});
		}

		// Update sale goal
		function updateSaleGoal() {
			// Get sale goal
			var annualGoal = document.getElementById("<%= tbAnnualGoal.ClientID %>").value;
			var applicableMonth = document.getElementById("<%= ddlApplicableMonth.ClientID %>").value;
			var monthlyGoal1 = $("#tbMonthlyGoal1").val();
			var monthlyGoal2 = $("#tbMonthlyGoal2").val();
			var monthlyGoal3 = $("#tbMonthlyGoal3").val();
			var monthlyGoal4 = $("#tbMonthlyGoal4").val();
			var monthlyGoal5 = $("#tbMonthlyGoal5").val();
			var monthlyGoal6 = $("#tbMonthlyGoal6").val();
			var monthlyGoal7 = $("#tbMonthlyGoal7").val();
			var monthlyGoal8 = $("#tbMonthlyGoal8").val();
			var monthlyGoal9 = $("#tbMonthlyGoal9").val();
			var monthlyGoal10 = $("#tbMonthlyGoal10").val();
			var monthlyGoal11 = $("#tbMonthlyGoal11").val();
			var monthlyGoal12 = $("#tbMonthlyGoal12").val();

			$.ajax({
				type: "POST",
				url: "<%: this.SummaryRootUrl %>/UpdateSaleGoal",
				contentType: "application/json; charset=utf-8",
				dataType: "json",
				data: JSON.stringify({
					annualGoal: annualGoal,
					applicableMonth: applicableMonth,
					monthlyGoal1: monthlyGoal1,
					monthlyGoal2: monthlyGoal2,
					monthlyGoal3: monthlyGoal3,
					monthlyGoal4: monthlyGoal4,
					monthlyGoal5: monthlyGoal5,
					monthlyGoal6: monthlyGoal6,
					monthlyGoal7: monthlyGoal7,
					monthlyGoal8: monthlyGoal8,
					monthlyGoal9: monthlyGoal9,
					monthlyGoal10: monthlyGoal10,
					monthlyGoal11: monthlyGoal11,
					monthlyGoal12: monthlyGoal12
				}),
				success: function (response) {
					var data = JSON.parse(response.d);
					if ((response == null) || (response.d == undefined)) return;

					var lbErrorMessage = document.getElementById("<%= lbErrorMessage.ClientID %>");

					// Reload data after success Insert/Update sale goal
					if (data.success) {
						// Reset error message
						lbErrorMessage.innerText = '';
						lbErrorMessage.style.display = 'none';

						$('.dashboard-goal-progress-wrapper').empty();
						$('.js-modal-close-btn').click();

						if (data.updatedData != '') {
							// Dislay sale goal
							data_goal_progress = eval(data.updatedData);
							dashboard.goal_progress.ini();

							document.getElementById("<%= dvGuide.ClientID %>").classList.remove("dashboard-goal-progress-guide");
							document.getElementById("<%= dvGuideBackground.ClientID %>").classList.remove("dashboard-goal-progress-guide-bg");
							var dvGuideLink = document.getElementById("<%= dvGuideLink.ClientID %>");
							if (dvGuideLink !== null) {
								dvGuideLink.style.display = "none";
							}
						} else {
							// Display warning when sale goal has not been set
							document.getElementById("<%= dvGuide.ClientID %>").classList.add("dashboard-goal-progress-guide");
							document.getElementById("<%= dvGuideBackground.ClientID %>").classList.add("dashboard-goal-progress-guide-bg");
							var dvGuideLink = document.getElementById("<%= dvGuideLink.ClientID %>");
							if (dvGuideLink !== null) {
								dvGuideLink.style.display = "inline";
							}
						}
					} else {
						// Display error messages
						lbErrorMessage.innerText = data.error;
						lbErrorMessage.style.display = 'inline';
					}
				},
				error: pageReload
			});
		}

		// Get and set chart reports by period kbn
		function getAndSetChartReportsByPeriodKbn(element, periodKbn) {
			$.ajax({
				type: "POST",
				url: "<%: this.SummaryRootUrl %>/GetChartReportsByPeriodKbn",
				data: JSON.stringify({ periodKbn: periodKbn }),
				contentType: "application/json; charset=utf-8",
				dataType: "json",
				success: function (response) {
					if ((response == null) || (response.d == undefined)) return;

					removeAllSelectedCss(element.parentElement);
					setSelectedCss(element);

					chartDataArray = JSON.parse(response.d);

					if ((periodKbn == 'TY')
						&& (chartDataArray.length > 0)) {
						$("#lastMonthLtv").text(chartDataArray[3]["data"]["datasets"][0]["data"][5]);
					}

					dashboard.transition_report_chart.ini();

					// For case the current browser is IE
					if (isIeBrowser()) {
						fixWidthAndHeightForChartReport();
					}

					setTimeout(function () {
						dashboard.transition_report_chart.render_all();
					}, 1000);
				},
				error: pageReload
			});
		}

		// Get and set latest reports by period kbn
		function getAndSetLatestReportsByPeriodKbn(element, periodKbn) {
			$.ajax({
				type: "POST",
				url: "<%: this.SummaryRootUrl %>/GetLatestReportsByPeriodKbn",
				data: JSON.stringify({ periodKbn: periodKbn }),
				contentType: "application/json; charset=utf-8",
				dataType: "json",
				success: function (response) {
					if ((response == null)|| (response.d == undefined)) return;

					removeAllSelectedCss(element.parentElement);
					setSelectedCss(element);
					changeLatestReportPrefixTitle(periodKbn);

					// Set latest report registered user
					var data = JSON.parse(response.d);
					var userRegistedReport = data.registeredUser;
					setText("spRegisteredUserTotalCount", userRegistedReport.totalCount);
					setText("tdRegisteredUserPcCount", userRegistedReport.pcCount);
					setText("tdRegisteredUserSpCount", userRegistedReport.spCount);
					setCssWidth("spRegisteredUserPcRateBar", userRegistedReport.pcRate);
					setText("spRegisteredUserPcRate", userRegistedReport.pcRate);
					setCssWidth("spRegisteredUserSpRateBar", userRegistedReport.spRate);
					setText("spRegisteredUserSpRate", userRegistedReport.spRate);
					if (periodKbn == '<%: Constants.FLG_SUMMARYREPORT_PERIOD_KBN_TODAY %>') {
						$('.user-latest-report-type-yesterday').css('display', 'none');
						$('.user-latest-report-type-today').css('display', '');
						$('.user-latest-report-type-today .odometer-value').text('ー')
					} else {
						$('.user-latest-report-type-yesterday').css('display', '');
						$('.user-latest-report-type-today').css('display', 'none');
					}

					// Set latest report order amount
					var orderAmountReport = data.orderAmount;
					setText("spOrderAmountTotalAmount", orderAmountReport.totalAmount);
					setText("tdOrderAmountPcAmount", orderAmountReport.pcAmount);
					setText("tdOrderAmountSpAmount", orderAmountReport.spAmount);
					setText("tdOrderAmountOtherAmount", orderAmountReport.otherAmount);
					setCssWidth("spOrderAmountPcRateBar", orderAmountReport.pcRate);
					setText("spOrderAmountPcRate", orderAmountReport.pcRate);
					setCssWidth("spOrderAmountSpRateBar", orderAmountReport.spRate);
					setText("spOrderAmountSpRate", orderAmountReport.spRate);
					setCssWidth("spOrderAmountOtherRateBar", orderAmountReport.otherRate);
					setText("spOrderAmountOtherRate", orderAmountReport.otherRate);

					// Set latest report order count
					var orderCountReport = data.orderCount;
					setText("spOrderCountTotalCount", orderCountReport.totalCount);
					setText("tdOrderCountPcCount", orderCountReport.pcCount);
					setText("tdOrderCountSpCount", orderCountReport.spCount);
					setText("tdOrderCountOtherCount", orderCountReport.otherCount);
					setText("tdOrderCountNormalCount", orderCountReport.normalCount);
					setText("tdOrderCountFixedPurchaseCount", orderCountReport.fixedPurchaseCount);
					setCssWidth("spOrderCountPcRateBar", orderCountReport.pcRate);
					setText("spOrderCountPcRate", orderCountReport.pcRate);
					setCssWidth("spOrderCountSpRateBar", orderCountReport.spRate);
					setText("spOrderCountSpRate", orderCountReport.spRate);
					setCssWidth("spOrderCountOtherRateBar", orderCountReport.otherRate);
					setText("spOrderCountOtherRate", orderCountReport.otherRate);
					setCssWidth("spOrderCountNormalRateBar", orderCountReport.normalRate);
					setText("spOrderCountNormalRate", orderCountReport.normalRate);
					setCssWidth("spOrderCountFixedPurchaseRateBar", orderCountReport.fixedPurchaseRate);
					setText("spOrderCountFixedPurchaseRate", orderCountReport.fixedPurchaseRate);
				},
				error: pageReload
			});
		}

		// Get shipped report
		function getShippedReport() {
			// Hide unit price
			$("#sUnshippedCountUnit").hide();
			$("#sShippedTodayCountUnit").hide();
			$("#sShippedFutureCountUnit").hide();

			$.ajax({
				type: "POST",
				url: "<%: this.SummaryRootUrl %>/GetShippedReport",
				contentType: "application/json; charset=utf-8",
				dataType: "json",
				success: function (response) {
					if ((response == null) || (response.d == undefined)) return;

					var data = JSON.parse(response.d);

					setText("sUnshippedCount", data.unshippedCount);
					setText("sShippedTodayCount", data.shippedTodayCount);
					setText("sShippedFutureCount", data.shippedFutureCount);
					if (data.hasUnshippedCount) {
						$("#aUnshippedCount").addClass('dashboard-status-box-is-alert is-alert');
					}

					// Show unit price
					$("#sUnshippedCountUnit").show();
					$("#sShippedTodayCountUnit").show();
					$("#sShippedFutureCountUnit").show();
				},
				error: pageReload
			});
		}

		// Get order workflow report
		function getOrderWorkflowReport() {
			// Clear all ajax request for display workflow target order count
			displayWorkflowTargetOrderCount.clear();

			$.ajax({
				type: "POST",
				url: "<%: this.SummaryRootUrl %>/GetOrderWorkflowReport",
				contentType: "application/json; charset=utf-8",
				dataType: "json",
				success: function (response) {
					if ((response == null) || (response.d == undefined)) return;

					var data = JSON.parse(response.d);

					for (var index = 0; index < data.length; index++) {
						var dataWorkflowKbnAttribute = ' data-workflow-kbn="kbn' + data[index].workflowKbn + '"';
						var dataWorkflowNoAttribute = ' data-workflow-no="no' + data[index].workflowNo + '"';
						var dataWorkflowTypeAttribute = ' data-workflow-type="' + data[index].workflowType + '"';
						var li = '<li class="dashboard-list-li order-workflow-list-item"' + dataWorkflowKbnAttribute + dataWorkflowNoAttribute + dataWorkflowTypeAttribute + '>'
							+ '    <a href="' + data[index].url + '">'
							+ '    <span class="dashboard-list-label">' + data[index].workflowName + '</span>'
							+ '    <span class="dashboard-list-count">'
							+ '        <span class="dashboard-list-count-value order-workflow-list-item-num-value">...</span>'
							+ '        <span class="dashboard-list-count-unit">件</span>'
							+ '    </span>'
							+ '    </a>'
							+ '</li>';

						$("#ulOrderWorkflowList").append(li);
						sizeChanged();
					}

					$("#dOrderWorkflowListLoading").hide();
					displayWorkflowTargetOrderCount.ini();
				},
				error: pageReload
			});
		}

		// Display workflow target order count
		var displayWorkflowTargetOrderCount = {
			wrapperSelector: '.order-workflow-list-item',
			wrapperTargetOrderCountValueSelector: '.order-workflow-list-item-num-value',
			loadingElementTemplate: '...',
			ajaxRequests: [],
			workflowTargetOrderCountRequests: [],
			sequenceCount: 1,
			requestCount: 0,
			next: function() {},
			ini: function () {
				var _this = this;
				_this.ajaxRequests = [];
				_this.workflowTargetOrderCountRequests = [];
				_this.requestCount = 0;
				$(_this.wrapperSelector).each(function (i) {
					var wrapper = $(this);
					var wrapperTargetOrderCountValue = wrapper.find(_this.wrapperTargetOrderCountValueSelector);
					var workflowKbn = wrapper.data('workflow-kbn').replace('kbn', '');
					var workflowNo = wrapper.data('workflow-no').replace('no', '');
					var workflowType = wrapper.data('workflow-type');

					// Add loading
					$(wrapperTargetOrderCountValue).html(_this.loadingElementTemplate);

					// Create requests
					var workflowTargetOrderCountRequest = {
						workflowKbn: workflowKbn,
						workflowNo: workflowNo,
						workflowType: workflowType
					};
					_this.workflowTargetOrderCountRequests.push(workflowTargetOrderCountRequest);
				});

				// Create iterator function to take request by the sequence count setting (default setting: 1)
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
				formData.append('<%: Constants.PARAM_ORDERWORKFLOW_ACTION_KBN %>', '<%: Constants.ORDERWORKFLOW_ACTION_KBN_ORDER_COUNT %>');
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

					if ((xmlHttpRequest.responseText === '') || (xmlHttpRequest.responseText === undefined)) return;

					// Write log error
					writeLogErrorMessages(xmlHttpRequest.status, xmlHttpRequest.responseText);
				});

				// Add ajax request to list
				_this.ajaxRequests.push(request)
			},
			set: function (data) {
				var _this = this;
				$(_this.wrapperSelector).each(function (i) {
					var wrapper = $(this);
					var wrapperTargetOrderCountValue = wrapper.find(_this.wrapperTargetOrderCountValueSelector);
					if ((data.workflowKbn === wrapper.data('workflow-kbn').replace('kbn', ''))
						&& (data.workflowNo === wrapper.data('workflow-no').replace('no', ''))
						&& (data.workflowType === wrapper.data('workflow-type'))) {
						// Set display order count
						$(wrapperTargetOrderCountValue).html(data.orderCount);
						return false;
					}
				});
			},
			clear: function () {
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

		// Write log error messages
		function writeLogErrorMessages(statusCode, responseText) {
			sendAjaxRequestWriteLogErrorMessages("<%: Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR %>/WriteLogErrorForCallAjax", statusCode, responseText);
		}

		// Set text
		function setText(idTag, textDisplay) {
			$("#" + idTag).text(textDisplay);
		}

		// Set css width
		function setCssWidth(idTag, number) {
			$("#" + idTag).css("width", number);
		}

		// Add transition
		function addCssTransition() {
			var elements = document.getElementsByClassName('icon-arrow-down');
			for (var index = 0; index < elements.length; index++)
			{
				elements[index].style.transition = "all 0.5s";
			}
		}

		// Change latest report prefix title
		function changeLatestReportPrefixTitle(periodKbn) {
			$('.latest-report-prefix-title').each(function () {
				$(this).text((periodKbn === '<%: Constants.FLG_SUMMARYREPORT_PERIOD_KBN_YESTERDAY %>')
					? '昨日の'
					: '今日の');
			});
		}

		// Remove all selected css
		function removeAllSelectedCss(parentElement) {
			$(parentElement).find('.is-current').removeClass('is-current');
		}

		// Set selected css
		function setSelectedCss(element) {
			$(element).addClass('is-current');
		}

		// Page reload
		function pageReload(xmlHttpRequest, status, error) {
			// Reload page when login session expired
			if (xmlHttpRequest.status == 401) {
				window.location.reload();
			}
		}

		// Reload init javascript function that missing when have post-back event (using for update panel)
		var isUseUpdatePanel = false;
		function bodyPageLoad() {
			if (Sys.WebForms == null) return;
			var isAsyncPostback = Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();
			if (isAsyncPostback) {
				isUseUpdatePanel = true;
				toggle.ini(isUseUpdatePanel);
				addCssTransition();
				sizeChanged();
			} else {
				isUseUpdatePanel = false;
			}
		}

		// Display sale goal input
		function displaySaleGoalInput() {
			// Get current sale goal values
			var applicableMonth = document.getElementById("<%= ddlApplicableMonth.ClientID %>").value;
			var monthlyGoal1 = $("#tbMonthlyGoal1").val();
			var monthlyGoal2 = $("#tbMonthlyGoal2").val();
			var monthlyGoal3 = $("#tbMonthlyGoal3").val();
			var monthlyGoal4 = $("#tbMonthlyGoal4").val();
			var monthlyGoal5 = $("#tbMonthlyGoal5").val();
			var monthlyGoal6 = $("#tbMonthlyGoal6").val();
			var monthlyGoal7 = $("#tbMonthlyGoal7").val();
			var monthlyGoal8 = $("#tbMonthlyGoal8").val();
			var monthlyGoal9 = $("#tbMonthlyGoal9").val();
			var monthlyGoal10 = $("#tbMonthlyGoal10").val();
			var monthlyGoal11 = $("#tbMonthlyGoal11").val();
			var monthlyGoal12 = $("#tbMonthlyGoal12").val();

			// Display sale goal
			$("#dvMonthly").empty();
			for (var month = applicableMonth; month <= 12; month++) {
				var div = '<div id="dvMonthlyGoal' + month + '" class="dashboard-goal-progress-setting-block">'
					+ '    <span class="dashboard-goal-progress-setting-title">' + month + '月</span>'
					+ '    <input type="text" id="tbMonthlyGoal' + month + '" class="dashboard-goal-progress-setting-input" MaxLength="15" />'
					+ '    <span class="dashboard-goal-progress-setting-unit">円</span>'
					+ '</div>'
				$("#dvMonthly").append(div);
			}
			for (var month = 1; month < applicableMonth; month++) {
				var div = '<div id="dvMonthlyGoal' + month + '" class="dashboard-goal-progress-setting-block">'
					+ '    <span class="dashboard-goal-progress-setting-title">' + month + '月</span>'
					+ '    <input type="text" id="tbMonthlyGoal' + month + '" class="dashboard-goal-progress-setting-input" MaxLength="15" />'
					+ '    <span class="dashboard-goal-progress-setting-unit">円</span>'
					+ '</div>'
				$("#dvMonthly").append(div);
			}

			// Set sale goal values
			$("#tbMonthlyGoal1").val(monthlyGoal1);
			$("#tbMonthlyGoal2").val(monthlyGoal2);
			$("#tbMonthlyGoal3").val(monthlyGoal3);
			$("#tbMonthlyGoal4").val(monthlyGoal4);
			$("#tbMonthlyGoal5").val(monthlyGoal5);
			$("#tbMonthlyGoal6").val(monthlyGoal6);
			$("#tbMonthlyGoal7").val(monthlyGoal7);
			$("#tbMonthlyGoal8").val(monthlyGoal8);
			$("#tbMonthlyGoal9").val(monthlyGoal9);
			$("#tbMonthlyGoal10").val(monthlyGoal10);
			$("#tbMonthlyGoal11").val(monthlyGoal11);
			$("#tbMonthlyGoal12").val(monthlyGoal12);
		}

		// Is IE browser
		function isIeBrowser() {
			if (typeof (browser) === "undefined") {
				var userAgent = window.navigator.userAgent.toLowerCase();
				if ((userAgent.indexOf('msie') != -1)
					|| (userAgent.indexOf('trident') != -1)) {
					var browser = 'ie';
				} else {
					var browser = 'other';
				}
			}

			return (browser === 'ie');
		}

		// Fix width and height for chart report (Only for IE browser)
		function fixWidthAndHeightForChartReport() {
			var width = $('.main-contents-inner').outerWidth();
			var maxWidth = width;
			var height = window.innerHeight;
			if (width <= 797) {
				width = 797;
				maxWidth = 797;
				height = 1108;
			}

			$('.dashboard-box-default').each(function () {
				$(this).width((width / 2));
				$(this).height((height / 4));
			});

			$('.dashboard-transition-report').css('maxWidth', (maxWidth + 15) + 'px');
			$('.dashboard-box-default').each(function (index) {
				$(this).width($(this).find('.dashboard-transition-report-canvas').outerWidth() + 16);
				$(this).height($(this).find('.dashboard-transition-report-canvas').outerHeight() + 60);
				if ((index % 2) == 0) {
					$(this).css('margin-right', '30px');
				}

				$(this).css('margin-bottom', '30px');
			});
		}

		// Bind chart report event (Only for IE browser)
		function bindChartReportEvent() {
			// For case the window resize
			$(window).bind('resize', function () {
				dashboard.transition_report_chart.ini();
				fixWidthAndHeightForChartReport();
				setTimeout(function () {
					dashboard.transition_report_chart.render_all();
				}, 1000);
			});

			// For case open/close side menu
			$('.sidemenu-btn-slimtoggle > span').each(function () {
				$(this).click(function () {
					setTimeout(function () {
						dashboard.transition_report_chart.ini();
						fixWidthAndHeightForChartReport();
						setTimeout(function () {
							dashboard.transition_report_chart.render_all();
						}, 1000);
					}, 400);
				});
			});
			$('#header').css('opacity', '0.3');
			$('#header').css('z-index', '200');
		}

		// Global variable of the slide show order status area
		var currentSlideOrderStatusIndex = 0;
		var currentSlideOrderStatusScrollLeft = 0;

		// Plus slide order status
		function plusSlideOrderStatus(index) {
			var slides = $('.dashboard-status-row-order-status-slide');
			var parent = $('.dashboard-status-row-order-status');
			currentSlideOrderStatusScrollLeft = parent[0].scrollLeft;
			currentSlideOrderStatusIndex = Math.ceil(parent[0].scrollLeft / $(slides[currentSlideOrderStatusIndex]).width());
			if ((slides.length == 0) || (parent.length == 0)) return;

			// For previous slide
			if ((index == -1) && ((currentSlideOrderStatusIndex + index) >= 0)) {
				currentSlideOrderStatusIndex += index;
				currentSlideOrderStatusScrollLeft -= $(slides[currentSlideOrderStatusIndex]).width();
			}

			// For next slide
			if ((index == 1) && ((currentSlideOrderStatusIndex + index) < slides.length)) {
				currentSlideOrderStatusIndex += index;
				currentSlideOrderStatusScrollLeft += $(slides[currentSlideOrderStatusIndex]).width();
			}

			$('#prevSlideOrderStatus').css('display', (currentSlideOrderStatusIndex == 0) ? 'none' : 'block');
			$('#nextSlideOrderStatus').css('display', (currentSlideOrderStatusIndex == (slides.length - 1)) ? 'none' : 'block');

			parent[0].scrollLeft = currentSlideOrderStatusScrollLeft;
		}

		// Load default button slide order status
		function loadDefaultButtonSlideOrderStatus() {
			if ($('#prevSlideOrderStatus').length > 0) {
				$('#prevSlideOrderStatus').css('display', 'none');
			}

			if ($('#nextSlideOrderStatus').length > 0) {
				$('#nextSlideOrderStatus').css('display', 'block');
			}
		}

		// Global variable of the slide show order status area
		var currentSlideIncidentStatusIndex = 0;
		var currentSlideIncidentStatusScrollLeft = 0;

		// Plus slide order status
		function plusSlideIncidentStatus(index) {
			var slides = $('.dashboard-status-row-incident-status-slide');
			var parent = $('.dashboard-status-row-incident-status');
			currentSlideIncidentStatusScrollLeft = parent[0].scrollLeft;
			currentSlideIncidentStatusIndex = Math.ceil(parent[0].scrollLeft / $(slides[currentSlideIncidentStatusIndex]).width());
			if ((slides.length == 0) || (parent.length == 0)) return;

			// For previous slide
			if ((index == -1) && ((currentSlideIncidentStatusIndex + index) >= 0)) {
				currentSlideIncidentStatusIndex += index;
				currentSlideIncidentStatusScrollLeft -= $(slides[currentSlideIncidentStatusIndex]).width();
			}

			// For next slide
			if ((index == 1) && ((currentSlideIncidentStatusIndex + index) < slides.length)) {
				currentSlideIncidentStatusIndex += index;
				currentSlideIncidentStatusScrollLeft += $(slides[currentSlideIncidentStatusIndex]).width();
			}

			$('#prevSlideIncidentStatus').css('display', (currentSlideIncidentStatusIndex == 0) ? 'none' : 'block');
			$('#nextSlideIncidentStatus').css('display', (currentSlideIncidentStatusIndex == (slides.length - 1)) ? 'none' : 'block');

			parent[0].scrollLeft = currentSlideIncidentStatusScrollLeft;
		}

		// Load default button slide order status
		function loadDefaultButtonSlideIncidentStatus() {
			if ($('#prevSlideIncidentStatus').length > 0) {
				$('#prevSlideIncidentStatus').css('display', 'none');
			}

			if ($('#nextSlideIncidentStatus').length > 0) {
				$('#nextSlideIncidentStatus').css('display', 'block');
			}
		}

		// Swipe Up / Down / Left / Right
		var initialX = null;
		var initialY = null;

		function startTouchOrderStatusSlider(e) {
			initialX = e.touches[0].clientX;
			initialY = e.touches[0].clientY;
		};

		function moveTouchOrderStatusSlider(e) {
			if (initialX === null) {
				return;
			}

			if (initialY === null) {
				return;
			}

			initialX = null;
			initialY = null;

			e.preventDefault();
		};

		// Display Touch Order Status Slider
		function displayTouchOrderStatusSlider(e) {
			var currentX = e.touches[0].clientX;
			var currentY = e.touches[0].clientY;

			var diffX = initialX - currentX;
			var diffY = initialY - currentY;

			var slides = $('.dashboard-status-row-order-status-slide');
			var parent = $('.dashboard-status-row-order-status');
			currentSlideOrderStatusIndex = Math.ceil(parent[0].scrollLeft / $(slides[currentSlideOrderStatusIndex]).width());

			// sliding horizontally
			if (diffX > 0) {
				// swiped left
				$('#prevSlideOrderStatus').css('display', (currentSlideOrderStatusIndex == 0) ? 'none' : 'block');
				$('#nextSlideOrderStatus').css('display', (currentSlideOrderStatusIndex == (slides.length - 1)) ? 'none' : 'block');
			} else {
				// swiped right
				$('#prevSlideOrderStatus').css('display', (currentSlideOrderStatusIndex == 0) ? 'none' : 'block');
				$('#nextSlideOrderStatus').css('display', (currentSlideOrderStatusIndex == (slides.length - 1)) ? 'none' : 'block');
			}
		};

		// Start Touch Incident Status Slider
		function startTouchIncidentStatusSlider(e) {
			initialX = e.touches[0].clientX;
			initialY = e.touches[0].clientY;
		};

		function moveTouchIncidentStatusSlider(e) {
			if (initialX === null) {
				return;
			}

			if (initialY === null) {
				return;
			}

			initialX = null;
			initialY = null;

			e.preventDefault();
		};

		// Display Touch Incident Status Slider
		function displayTouchIncidentStatusSlider(e) {
			var currentX = e.touches[0].clientX;
			var currentY = e.touches[0].clientY;

			var diffX = initialX - currentX;
			var diffY = initialY - currentY;

			var slides = $('.dashboard-status-row-incident-status-slide');
			var parent = $('.dashboard-status-row-incident-status');
			currentSlideIncidentStatusIndex = Math.ceil(parent[0].scrollLeft / $(slides[currentSlideIncidentStatusIndex]).width());

			// sliding horizontally
			if (diffX > 0) {
				// swiped left
				$('#prevSlideIncidentStatus').css('display', (currentSlideIncidentStatusIndex == 0) ? 'none' : 'block');
				$('#nextSlideIncidentStatus').css('display', (currentSlideIncidentStatusIndex == (slides.length - 1)) ? 'none' : 'block');
			} else {
				// swiped right
				$('#prevSlideIncidentStatus').css('display', (currentSlideIncidentStatusIndex == 0) ? 'none' : 'block');
				$('#nextSlideIncidentStatus').css('display', (currentSlideIncidentStatusIndex == (slides.length - 1)) ? 'none' : 'block');
			}
		};
	</script>
</asp:Content>
