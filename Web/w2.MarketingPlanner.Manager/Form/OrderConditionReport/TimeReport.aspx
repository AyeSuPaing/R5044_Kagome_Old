<%--
=========================================================================================================
  Module      : 時間別レポート(TimeReport.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="TimeReport.aspx.cs" Inherits="Form_OrderConditionReport_TimeReport" MaintainScrollPositionOnPostback="true" %>
<%@ Register TagPrefix="uc" TagName="DateTimeInput" Src="~/Form/Common/DateTimeInput.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<tb><h1 class="page-title">時間別売上レポート</h1></tb>
	</tr>
	
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr> <!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">レポート表示</h2></td>
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
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table border="0" cellspacing="0" cellpadding="0">
													<tr>
														<td>
															<div class="search_btn_sub_rt" >
															&nbsp;</div>
															<table class="info_table" width="410" border="0" cellspacing="1" cellpadding="3">
																<tr class="info_item_bg">
																	<td align="left" id="tdReportInfo" runat="server">
																		<asp:Label ID="lbReportInfo" runat="server"></asp:Label><br>
																		<asp:Label ID="lbReportInfoOrderConditionDays" runat="server"></asp:Label>
																	</td>
																</tr>
															</table>
														</td>
														<td class="search_box" width="10"></td>
														<td>
															<div class="search_btn_sub_rt" >
																<asp:LinkButton id="lbReportExport" Runat="server" OnClick="lbReportExport_Click">CSVダウンロード</asp:LinkButton></div>
															<table class="info_table" width="280" border="0" cellspacing="1" cellpadding="3">
																<tr>
																	<td class="list_title_bg" align="center" width="110" id="tdDateAvgInfo" runat="server"></td>
																	<td class="info_item_bg" align="right" width="170" id="tdDateAvgPriceInfo" runat="server"></td>
																</tr>
															</table>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										
										<!--▽ レポート ▽-->
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" border="0" width="700">
													<%--▽ 売上(注文基準) ▽--%>
													<% if (this.ReportSalesType == KBN_SALES_TYPE_ORDER_REPORT){ %>
													<tr class="list_title_bg">
														<td class="list_title_bg" style="white-space:nowrap" width="120" rowspan="2"></td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="245" colspan="3">売上(注文基準)</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="165" colspan="2">キャンセル</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="165" colspan="2">小計</td>
													</tr>
													<%--△ 売上(注文基準) △--%>
													<%--▽ 売上(出荷基準) ▽--%>
													<% }else if (this.ReportSalesType == KBN_SALES_TYPE_SHIP_REPORT){%>
													<tr class="list_title_bg">
														<td class="list_title_bg" style="white-space:nowrap" width="120" rowspan="2"></td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="245" colspan="3">売上(出荷基準)</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="165" colspan="2">返品</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="165" colspan="2">小計</td>
													</tr>
													<% } %>
													<%--△ 売上(出荷基準) △--%>
													<tr>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="98">金額</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="right" width="64"><%= (this.HasOrderAggregateUnit) ? "件数" : "個数"%></td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="83"><%= (this.HasOrderAggregateUnit) ? "平均購入単価" : "平均商品単価"%></td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="100">金額</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="right" width="65"><%= (this.HasOrderAggregateUnit) ? "件数" : "個数"%></td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="100">金額</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="right" width="65"><%= (this.HasOrderAggregateUnit) ? "件数" : "個数"%></td>
														
													</tr>
													<tr valign="top">
														<td class="list_item_bg5_dark" colspan="8" style="padding:0px"></td>
													</tr>
													<tr valign="middle"  height="20">
														<td class="list_title_bg" align="center">
															<a>合計</a>
														</td>
														<td class="list_item_bg1" align="right"><asp:Label ID="lbPriceTotal" runat="server"></asp:Label></td>
														<td class="list_item_bg1" align="right"><asp:Label ID="lbCountTotal" runat="server"></asp:Label><%= (this.HasOrderAggregateUnit) ? "件" : "個"%></td>
														<td class="list_item_bg1" align="right"><asp:Label ID="lbAvgTotal" runat="server"></asp:Label></td>
														<td class="list_item_bg1" align="right"><asp:Label ID="lbMinusPriceTotal" runat="server"></asp:Label></td>
														<td class="list_item_bg1" align="right"><asp:Label ID="lbMinusCountTotal" runat="server"></asp:Label><%= (this.HasOrderAggregateUnit) ? "件" : "個"%></td>
														<td class="list_item_bg1" align="right"><asp:Label ID="lbSubtotalPriceTotal" runat="server"></asp:Label></td>
														<td class="list_item_bg1" align="right"><asp:Label ID="lbSubtotalCountTotal" runat="server"></asp:Label><%= (this.HasOrderAggregateUnit) ? "件" : "個"%></td>
													</tr>

													<tr valign="top">
														<td class="list_item_bg5_dark" colspan="8" style="padding:0px"></td>
													</tr>
													<asp:Repeater id="rDataList" Runat="server" DataSource="<%# this.DisplayDataList %>">
														<ItemTemplate>
															<tr class='<%# ((Hashtable)Container.DataItem)["class"] %>'>
																<td class="list_title_bg" align="center">
																	<%# ((Hashtable)Container.DataItem)[FIELD_ORDERCONDITION_TARGET_TIME]+":00"%>
																</td>
																<asp:Repeater DataSource='<%# ((Hashtable)Container.DataItem)["data"] %>' id="rLine" Runat="server">
																	<ItemTemplate>
																		<%--▽ 月表示 ▽--%>
																		<% if (this.SettingReportType == KBN_REPORT_TYPE_MONTHLY_REPORT){ %>
																			<td align="right">
																			<%#: (Container.DataItem) %></td>
																		<%--△ 月表示 △--%>
																		<%--▽ 日表示 ▽--%>
																		<% }else if (this.SettingReportType == KBN_REPORT_TYPE_DAILY_REPORT){%>
																			<td align="right">
																			<%#: (Container.DataItem) %></td>
																		<% } %>
																		<%--△ 日表示 △--%>
																	</ItemTemplate>
																</asp:Repeater>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
												</table>
											</td>
										</tr>
										<!--△ レポート △-->
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="info_table" width="700" border="0" cellspacing="1" cellpadding="3" id="note">
													<tr class="info_item_bg">
														<td align="left">
															備考<br />
															・売上基準を「出荷基準」に設定している場合、返品項目の金額、件数の表示対象は「返品注文」となります。<br/>
															　キャンセルされた「交換注文」についても返品として扱われますが「交換注文」は含まれません。<br />
															　なお、「交換注文」は、売上（注文基準）項目の金額、件数に含まれます。<br />
															・「商品ID」<%= (Constants.PRODUCT_BRAND_ENABLED) ? "「ブランド指定」" : ""%>は集計単位が「小計（商品単位）」の場合のみ利用可能です。<br />
															・注文単位で検索する場合、金額には注文時の手数料、割引金額を含みます。<br />
															　	※金額は、小計 + 配送料 + 手数料 + 調整金額<%= (Constants.MEMBER_RANK_OPTION_ENABLED) ? " - 会員ランク割引額 " : ""%><%= (Constants.W2MP_COUPON_OPTION_ENABLED) ? " - クーポン割引額 " : ""%><%= (Constants.W2MP_POINT_OPTION_ENABLED) ? " - 利用ポイント " : ""%>を含みます。<br />
															・商品単位の場合は注文時の手数料、割引金額を含みません。<br />
															・消費税が「税抜」の場合は、各合計金額欄の結果から消費税額を引いています。<br />
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr> <!--△ 一覧 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>