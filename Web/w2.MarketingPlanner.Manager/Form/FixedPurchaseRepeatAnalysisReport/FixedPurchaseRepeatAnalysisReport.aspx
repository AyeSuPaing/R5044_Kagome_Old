<%--
=========================================================================================================
  Module      : 定期継続分析レポートページ(FixedPurchaseRepeatAnalysisReport.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>

<%@ Import Namespace="w2.App.Common.Product" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="FixedPurchaseRepeatAnalysisReport.aspx.cs" Inherits="Form_FixedPurchaseRepeatAnalysisReport_FixedPurchaseRepeatAnalysisReport" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
	<style type="text/css">
		.tableHeader {
			width: 15%;
			word-wrap: break-word;
		}
		.tableFixedHeader {
			position: -webkit-sticky;
			position: sticky;
			z-index: 49;
			top: 39px;
			background-color: #bababa;
			background: #e0dfdf;
			font-weight: bold;
			box-shadow: 0 0 0 1px #d0cfcf;
		}
		.tableTitle1 {
			background-color: #c9dbe6;
		}
		.tableTitle2 {
			background-color: #c9dbe6;
		}
		.tableTitle3 {
			background-color: #c9dbe6;
		}
		.tableTitle4 {
			background-color: #fde0e0;
		}
		.tableTitle5 {
			background-color: #f6cef5;
		}
		.tableTitle6 {
			background-color: #f6cef5;
		}
		.tableTitle7 {
			background-color: #f6cef5;
		}
		.tableTitle8 {
			background-color: #c9dbe6;
		}
		.tableTitle9 {
			background-color: #f6cef5;
		}
		.tableTitle10 {
			background-color: #d3c7e1;
		}
		.tableTitle11 {
			background-color: lightyellow;
		}
		.tableContent:hover {
			background-color: #ffD390;
			cursor: pointer;
		}
		.list_month_bg1 {
			background-color: #ffffff;
		}
		.list_month_bg2 {
			background-color: #e7e7e7;
		}
	</style>
	<table cellspacing="0" cellpadding="0" width="791" border="0">
		<tr>
			<td>
				<h1 style="display: inline-block; width: 45%;" class="page-title">定期継続分析レポート</h1>
				<asp:RadioButtonList
					ID="rblReportKbn"
					runat="server"
					Width="300"
					RepeatDirection="Horizontal"
					RepeatLayout="Flow"
					AutoPostBack="True"
					OnSelectedIndexChanged="rblReportKbn_SelectedIndexChanged" />
			</td>
		</tr>
		<tr>
			<td>
				<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
			</td>
		</tr>
		<!--▽ 検索 ▽-->
		<tr>
			<td>
				<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
					<tr>
						<td class="search_box_bg">
							<table cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td align="center">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td>
													<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
												</td>
											</tr>
											<tr>
												<td class="search_box">
													<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
														<tr>
															<td class="search_title_bg" width="140" style="height: 38px">
																<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align: middle" border="0" />
																商品ID
															</td>
															<td class="search_item_bg" style="height: 38px">
																<asp:TextBox ID="tbProductId" runat="server" />
															</td>
															<td class="search_title_bg" width="140" style="height: 38px">
																<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align: middle" border="0" />
																バリエーションID
															</td>
															<td class="search_item_bg" style="height: 38px">
																<asp:TextBox ID="tbVariationId" runat="server" />
															</td>
															<td class="search_btn_bg" width="83" rowspan="5" style="height: 38px">
																<div class="search_btn_main">
																	<asp:Button ID="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" />
																</div>
																<div class="search_btn_sub">
																	<a href="<%: CreateDefaultReportListUrl() %>">クリア</a>&nbsp;
																	<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a>
																</div>
															</td>
														</tr>
														<% if (this.IsLtvReportKbn) { %>
														<tr>
															<td class="search_title_bg" width="140" style="height: 38px;">
																<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align: middle" border="0" />
																広告媒体区分
															</td>
															<td class="search_item_bg">
																<asp:DropDownList Width="160px" ID="ddlSearchAdvMediaKbn" runat="server" />
															</td>
															<td class="search_title_bg" width="140" style="height: 38px;">
																<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align: middle" border="0" />
																広告コード
															</td>
															<td class="search_item_bg" style="height: 38px;">
																<asp:TextBox ID="tbSearchAdvCode" runat="server" />
															</td>
														</tr>
														<tr>
															<td class="search_title_bg" width="140" style="height: 38px;">
																<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align: middle" border="0" />
																ユーザー拡張
															</td>
															<td class="search_item_bg" style="width: 37%;">
																<asp:DropDownList Width="210px" ID="ddlUserExtend" runat="server" />が
															<br />
																<asp:TextBox Width="99%" ID="tbUserExtend" runat="server" />
															</td>
															<td class="search_title_bg" width="140" style="height: 38px;">
																<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align: middle" border="0" />
																決済種別
															</td>
															<td class="search_item_bg" style="height: 38px;">
																<asp:DropDownList Width="160px" ID="ddlPaymentType" runat="server" />
															</td>
														</tr>
														<tr>
															<td class="search_title_bg" width="140" style="height: 38px;">
																<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align: middle" border="0" />
																定期開始期間
															</td>
															<td class="search_item_bg" style="height: 38px;" colspan="3">
																<uc:DateTimePickerPeriodInput ID="ucTargetPeriod" runat="server" IsNullStartDateTime="false" />
																の間
															<span class="search_btn_sub">(<a href="Javascript:SetYesterday('fixedPurchaseRepeatAnalysis');">昨日</a>｜<a href="Javascript:SetToday('fixedPurchaseRepeatAnalysis');">今日</a>｜<a href="Javascript:SetThisMonth('fixedPurchaseRepeatAnalysis');">今月</a>)</span>
															</td>
														</tr>
														<% } %>
													</table>
												</td>
											</tr>
											<tr>
												<td>
													<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
												</td>
											</tr>
										</table>
									</td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<!--△ 検索 △-->
		<tr>
			<td>
				<img height="3" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
			</td>
		</tr>

		<!--▽ レポート表示 (LTVレポート) ▽-->
		<% if (this.IsLtvReportKbn) { %>
		<tr>
			<td>
				<h2 class="cmn-hed-h2">レポート表示</h2>
			</td>
		</tr>
		<tr>
			<td>
				<asp:UpdatePanel runat="server">
					<ContentTemplate>
						<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
							<tr>
								<td>
									<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
										<tr>
											<td>
												<img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
											</td>
										</tr>
										<tr>
											<td align="center">
												<div runat="server" id="dvLoadingImg">
													<img alt="" src="../../Images/Common/loading.gif" width="20" height="20" border="0" />
													現在レポート作成待ちです…<br />
												</div>
											</td>
										</tr>
										<tr>
											<td align="center">
												<asp:Literal id="lProcessMessage" runat="server" />
											</td>
										</tr>
										<% if(this.IsShowLtvContents == false) { %>
										<tr  id="trShowLtvContents" runat="server">
											<td>
												<table border="0" cellspacing="0" cellpadding="0">
													<tr>
														<td>
															<table class="info_table" width="750" border="0" cellspacing="1" cellpadding="3">
																<tr class="info_item_bg">
																	<td align="left">検索条件を指定してください。</td>
																</tr>
															</table>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<% } %>
										<% if (this.DisplayLtvSummaryData != null) { %>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="list_title_bg">
														<td align="center" width="100">平均単価</td>
														<td align="center" width="200">平均受注数(月)</td>
														<td align="center" width="200">解約率</td>
														<td align="center" width="200">LTV</td>
													</tr>
													<tr align="center" runat="server">
														<td><%: StringUtility.ToNumeric(GetDisplayLtvSummaryDataValue(this.DisplayLtvSummaryData.AverageOrderPriceSubtotal)) %>円</td>
														<td><%: StringUtility.ToNumeric(GetDisplayLtvSummaryDataValue(this.DisplayLtvSummaryData.AverageOrderCount)) %>回</td>
														<td><%: GetDisplayLtvSummaryDataValue(this.DisplayLtvSummaryData.CancelationRate) %>%</td>
														<td><%: StringUtility.ToNumeric(GetDisplayLtvSummaryDataValue(this.DisplayLtvSummaryData.Ltv)) %>円</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td>
												<img height="3" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
											</td>
										</tr>
										<tr>
											<td align="right">
												<asp:CheckBox ID="cbLtv" runat="server" Visible="false" AutoPostBack="true" Text="  LTVに解約率を含む  " OnCheckedChanged="cbLtv_OnCheckedChanged" Checked="true" />
											</td>
										</tr>
										<% } %>
										<tr>
											<td>
												<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
											</td>
										</tr>
										<tr>
											<td align="left">
												<%--▽ ページング ▽--%>
												<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
													<tr>
														<td width="675"><asp:Label ID="lbPager1" runat="server" /></td>
													</tr>
												</table>
												<%--△ ページング △--%>
											</td>
										</tr>
										<!--▽ メインエリア ▽-->
										<tr>
											<td>
												<table class="list_table" border="0" style="height: 1px; border-collapse: collapse;" cellspacing="0" cellpadding="0">
													<div class="search_btn_sub_rt">
														<asp:LinkButton runat="server" Visible="false" ID="lbLtvReportExport" OnClick="lbLtvReportExport_Click" Text="  CSVダウンロード  " />
													</div>
													<asp:Repeater DataSource="<%# this.DisplayLtvDataList %>" ItemType="ReportLtvData" runat="server">
														<HeaderTemplate>
															<tr>
																<td class="tableFixedHeader" style="width:250px; min-width: 250px;" align="center">商品ID<br />
																	バリエーションID<br />
																	商品名
																</td>
																<td class="tableFixedHeader" style="width:100px; min-width: 100px;" align="center">初回受注月</td>
																<td class="tableFixedHeader" style="width:110px; min-width: 110px;" align="center">LTVレポート</td>
																<asp:Repeater DataSource="<%# this.ProcessInfo.MonthDataList %>" ItemType="System.String" runat="server">
																	<ItemTemplate>
																		<td class="tableFixedHeader" style="width: 75px; min-width: 75px;" align="center"><%#: Container.DataItem.ToString() %></td>
																	</ItemTemplate>
																</asp:Repeater>
															</tr>
														</HeaderTemplate>
														<ItemTemplate>
															<asp:Repeater ID="rLtvData" DataSource="<%# Item.MonthData %>" ItemType="ReportMonthData" runat="server">
																<ItemTemplate>
																	<asp:Repeater ID="rLtvRow" DataSource="<%# Item.LtvData %>" ItemType="ReportLtvRowData" runat="server">
																		<ItemTemplate>
																			<tr>
																				<td align="left" class="list_item_bg3" rowspan="<%# ((ReportLtvData)(((RepeaterItem)Container.Parent.Parent.Parent.Parent).DataItem)).MonthData.Count * 4 %>" visible="<%# ((((RepeaterItem)Container.Parent.Parent).ItemIndex == 0) && (Container.ItemIndex == 0)) %>" runat="server">
																					<p style="word-break: break-all;">商品ID: <%#: ((ReportLtvData)(((RepeaterItem)Container.Parent.Parent.Parent.Parent).DataItem)).ProductId %></p>
																					<p style="word-break: break-all;">バリエーションID: <%#: ((ReportLtvData)(((RepeaterItem)Container.Parent.Parent.Parent.Parent).DataItem)).VariationId %></p>
																					<p style="word-break: break-all;">商品名: <%#: ((ReportLtvData)(((RepeaterItem)Container.Parent.Parent.Parent.Parent).DataItem)).ProductName %></p>
																					<div style="text-align: center; padding-top: 5px;">
																						<%# ProductImage.GetHtmlImageTag(((ReportLtvData)(((RepeaterItem)Container.Parent.Parent.Parent.Parent).DataItem)).ImageProduct, ProductType.Product, SiteType.Pc, Constants.PRODUCTIMAGE_FOOTER_M)%>
																					</div>
																				</td>
																				<td style="padding: 0px;" rowspan="4" visible="<%# (Container.ItemIndex == 0) %>" class="list_month_bg1" runat="server">
																					<%#: ((ReportMonthData)(((RepeaterItem)Container.Parent.Parent).DataItem)).Year %>年
																					<%#: ((ReportMonthData)(((RepeaterItem)Container.Parent.Parent).DataItem)).Month %>月
																				</td>
																				<td align="left" class="tableTitle<%# (Container.ItemIndex + 8) %>"><%#: Item.TitleLtvReport %></td>
																				<asp:Repeater ID="lbCreateTargetList" DataSource="<%# Item.ContentsLtvData %>" ItemType="System.String" runat="server">
																					<ItemTemplate>
																						<td align="center" style="padding: 0px;" class="tableContent" visible="<%# (((RepeaterItem)Container.Parent.Parent).ItemIndex <= 1) %>" runat="server">
																							<div title="対象受注の注文者一覧が作成可能です。" style="display: grid; height: 100%; align-content: space-around;" onclick="javascript:open_window('<%#: ImportTargetListUrlCreator.CreateUrlTargetList(Constants.KBN_WINDOW_POPUP, ((ReportLtvData)(((RepeaterItem)Container.Parent.Parent.Parent.Parent.Parent.Parent).DataItem)).ProductId, ((ReportLtvData)(((RepeaterItem)Container.Parent.Parent.Parent.Parent.Parent.Parent).DataItem)).VariationId, ((ReportMonthData)(((RepeaterItem)Container.Parent.Parent.Parent.Parent).DataItem)).Year, ((ReportMonthData)(((RepeaterItem)Container.Parent.Parent.Parent.Parent).DataItem)).Month, Container.ItemIndex, ((RepeaterItem)Container.Parent.Parent).ItemIndex, this.ProcessInfo.CurrentPageNumber) %>','Import','width=850,height=370,top=120,left=420,status=no,scrollbars=yes');">
																								<%#: Item %>
																							</div>
																						</td>
																						<td align="center" visible="<%# (((RepeaterItem)Container.Parent.Parent).ItemIndex > 1) %>" runat="server">
																							<%#: Item %>
																						</td>
																					</ItemTemplate>
																				</asp:Repeater>
																			</tr>
																		</ItemTemplate>
																	</asp:Repeater>
																</ItemTemplate>
															</asp:Repeater>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" visible="false">
														<td id="tdErrorMessage" runat="server" colspan="6"></td>
													</tr>
												</table>
											</td>
										</tr>
										<!--△ メインエリア △-->
										<tr>
											<td>
												<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
											</td>
										</tr>
										<tr>
											<td>
												<table class="info_table" width="750" border="0" cellspacing="1" cellpadding="3" id="note">
													<tr class="info_item_bg">
														<td align="left">備考<br />
															13ヶ月以降は次のページに表示されています。<br />
															<br />
															<table class="no-border no-padding">
																<tr>
																	<td>受注件数</td>
																	<td>：商品が出荷された顧客数</td>
																</tr>
																<tr>
																	<td>解約件数</td>
																	<td>：前回の出荷から今回の出荷が行われなかった顧客件数</td>
																</tr>
																<tr>
																	<td>受注金額</td>
																	<td>：継続月数ごとの合計受注金額</td>
																</tr>
																<tr>
																	<td>残存率</td>
																	<td>：1回目の出荷からの残存率</td>
																</tr>
															</table>
															<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED) { %>
															<br />
															頒布会のLTVレポートとしては商品ではなくコース軸になるため、本LTVレポートには頒布会で購入された商品は含まれていません。
															<% } %>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td>
												<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</ContentTemplate>
					<Triggers>
						<asp:AsyncPostBackTrigger ControlID="tProcessTimer" EventName="Tick" />
						<asp:PostBackTrigger ControlID="lbLtvReportExport" />
					</Triggers>
				</asp:UpdatePanel>
			</td>
		</tr>
		<% } %>
		<!--△ レポート表示 (LTVレポート) △-->

		<!--▽ レポート表示 (回数別レポート) ▽-->
		<% if (this.IsLtvReportKbn == false) { %>
		<tr>
			<td>
				<h2 class="cmn-hed-h2">レポート表示</h2>
			</td>
		</tr>
		<tr>
			<td>
				<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
					<tr>
						<td>
							<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td align="center">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td>
													<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<% if (this.ShowContents == false) { %>
											<tr>
												<td>
													<table border="0" cellspacing="0" cellpadding="0">
														<tr>
															<td>
																<table class="info_table" width="750" border="0" cellspacing="1" cellpadding="3">
																	<tr class="info_item_bg">
																		<td align="left">検索条件を指定してください。
																		<asp:Label ID="lbReportInfo" runat="server"></asp:Label>
																		</td>
																	</tr>
																</table>
															</td>
														</tr>
													</table>
												</td>
											</tr>
											<% } else { %>
											<!--▽ メインエリア ▽-->
											<tr>
												<td>
													<div class="search_btn_sub_rt">
														<asp:LinkButton ID="lbReportExport" runat="server" OnClick="lbReportExport_Click">CSVダウンロード</asp:LinkButton>
													</div>
													<table class="list_table" border="0" cellspacing="0" cellpadding="0">
														<asp:Repeater DataSource="<%# this.DisplayDataList %>" ItemType="ReportData" runat="server">
															<HeaderTemplate>
																<tr class="list_title_bg">
																	<td class="tableHeader" align="center" width="15%">商品ID<br />
																		バリエーションID<br />
																		商品名</td>
																	<td align="center" width="15%">購入回数</td>
																	<td align="center" width="7%">1回</td>
																	<td align="center" width="7%">2回</td>
																	<td align="center" width="7%">3回</td>
																	<td align="center" width="7%">4回</td>
																	<td align="center" width="7%">5回</td>
																	<td align="center" width="7%">6回</td>
																	<td align="center" width="7%">7回</td>
																	<td align="center" width="7%">8回</td>
																	<td align="center" width="7%">9回</td>
																	<td align="center" width="7%">10回</td>
																</tr>
															</HeaderTemplate>
															<ItemTemplate>
																<tr valign="top">
																	<td class="list_item_bg5_dark" colspan="2" style="padding: 0"></td>
																</tr>
																<asp:Repeater DataSource="<%# Item.Data %>" ItemType="ReportRowData" runat="server">
																	<HeaderTemplate></HeaderTemplate>
																	<ItemTemplate>
																		<tr class="list_item_bg<%# Item.ReportDataIndex % 2 + 1 %>">
																			<td align="left" class="list_item_bg3" rowspan="7" visible="<%# Container.ItemIndex == 0 %>" runat="server">
																				<%#: Item.ProductId %><br />
																				<%#: Item.VariationId %><br />
																				<%#: Item.ProductName %>
																			</td>
																			<td align="left" class="tableTitle<%# Container.ItemIndex + 1 %>"><%#: Item.Title %></td>
																			<asp:Repeater DataSource="<%# Item.ContentsData %>" ItemType="System.String" runat="server">
																				<HeaderTemplate></HeaderTemplate>
																				<ItemTemplate>
																					<td align="right"><%#: Item %></td>
																				</ItemTemplate>
																			</asp:Repeater>
																		</tr>
																	</ItemTemplate>
																</asp:Repeater>
															</ItemTemplate>
														</asp:Repeater>
													</table>
												</td>
											</tr>
											<% } %>
											<!--△ メインエリア △-->
											<tr>
												<td>
													<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr runat="server" visible="<%# this.IsLtvReportKbn == false %>">
												<td>
													<table class="info_table" width="750" border="0" cellspacing="1" cellpadding="3" id="note">
														<tr class="info_item_bg">
															<td align="left">備考<br />
																11回以降は表示されません。<br />
																<br />
																<table class="no-border no-padding">
																	<tr>
																		<td>継続顧客数(出荷)</td>
																		<td>：商品が出荷された顧客数</td>
																	</tr>
																	<tr>
																		<td>継続率</td>
																		<td>：前回の出荷から今回の出荷が行われた顧客率</td>
																	</tr>
																	<tr>
																		<td>1回目からの残存率</td>
																		<td>：1回目の出荷から今回の出荷が行われた顧客率</td>
																	</tr>
																	<tr>
																		<td>(未注文・未出荷)</td>
																		<td>：商品が未出荷、または次回の注文が作成されていない顧客数</td>
																	</tr>
																	<tr>
																		<td>離脱顧客数</td>
																		<td>：商品の購入をやめた顧客数</td>
																	</tr>
																	<tr>
																		<td>離脱率</td>
																		<td>：継続顧客数（出荷）のうち離脱顧客数の離脱率</td>
																	</tr>
																	<tr>
																		<td>1回目からの離脱率</td>
																		<td>：1回目の出荷からの離脱率</td>
																	</tr>
																</table>
															</td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<td>
													<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
										</table>
									</td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<!--△ レポート表示 △-->
		<tr>
			<td>
				<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
		</tr>
		<% } %>
		<!--△ レポート表示 (回数別レポート) △-->
	</table>
	<script type="text/javascript">
		function SetYesterday(set_date) {
			if (set_date == 'fixedPurchaseRepeatAnalysis') {
				document.getElementById('<%= ucTargetPeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.Date.AddDays(-1)) %>';
				document.getElementById('<%= ucTargetPeriod.HfStartTime.ClientID %>').value = '00:00:00';
				document.getElementById('<%= ucTargetPeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.Date.AddDays(-1)) %>';
				document.getElementById('<%= ucTargetPeriod.HfEndTime.ClientID %>').value = '23:59:59';
				reloadDisplayDateTimePeriod('<%= ucTargetPeriod.ClientID %>');
			}
		}
	
		function SetThisMonth(set_date) {
			if (set_date == 'fixedPurchaseRepeatAnalysis') {
				document.getElementById('<%= ucTargetPeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetStartDateThisMonthString() %>';
				document.getElementById('<%= ucTargetPeriod.HfStartTime.ClientID %>').value = '00:00:00';
				document.getElementById('<%= ucTargetPeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetEndDateThisMonthString() %>';
				document.getElementById('<%= ucTargetPeriod.HfEndTime.ClientID %>').value = '23:59:59';
				reloadDisplayDateTimePeriod('<%= ucTargetPeriod.ClientID %>');
			}
		}
	
		function SetToday(set_date) {
			if (set_date == 'fixedPurchaseRepeatAnalysis') {
				document.getElementById('<%= ucTargetPeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
				document.getElementById('<%= ucTargetPeriod.HfStartTime.ClientID %>').value = '00:00:00';
				document.getElementById('<%= ucTargetPeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
				document.getElementById('<%= ucTargetPeriod.HfEndTime.ClientID %>').value = '23:59:59';
				reloadDisplayDateTimePeriod('<%= ucTargetPeriod.ClientID %>');
			}
		}
	</script>
	<asp:Timer id="tProcessTimer" Interval="1000" OnTick="tProcessTimer_Tick" Enabled="False" runat="server"/>
</asp:Content>

