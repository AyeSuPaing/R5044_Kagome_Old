<%--
=========================================================================================================
  Module      : 購買区分レポート一覧ページ(OrderKbnAnalysisList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OrderKbnReportList.aspx.cs" Inherits="Form_OrderKbnReport_OrderKbnReportList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">購買区分レポート</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
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
												<div class="search_btn_sub_rt" >
													<asp:LinkButton id="lbReportExport" Runat="server" OnClick="lbReportExport_Click">CSVダウンロード</asp:LinkButton></div>
												<table class="info_table" width="700" border="0" cellspacing="1" cellpadding="3">
													<tr class="info_item_bg">
														<td align="left">データ作成日時：<%: DateTimeUtility.ToStringForManager(DateTime.Now, DateTimeUtility.FormatType.LongDateHourMinute2Letter) %></td>
													</tr>
												</table>
											</td>
										</tr>
										<!--▽ 分析結果 ▽-->
										<asp:Repeater ID="rResults" Runat="server">
											<ItemTemplate>
												<tr>
													<td><img height="12" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
												</tr>
												<tr>
													<td>
														<table class="list_table" cellspacing="1" cellpadding="3" width="700" border="0">
															<tr class="list_title_bg">
																<td align="center" width="30%" colspan="4"><asp:Literal ID="lTitle" runat="server" Text="<%# WebSanitizer.HtmlEncode(((KbnAnalysisUtility.KbnAnalysisTable)Container.DataItem).Title) %>" /></td>
															</tr>
															<asp:Repeater id="Result2" Runat="server" DataSource='<%# ((KbnAnalysisUtility.KbnAnalysisTable)Container.DataItem).Rows %>'>
																<ItemTemplate>
																	<tr>
																		<td class="list_item_bg2" align="center" width="20%">
																			<asp:Literal ID="lName" runat="server" Text="<%# WebSanitizer.HtmlEncode(((KbnAnalysisUtility.KbnAnalysisRow)Container.DataItem).Name)%>" /></td>
																		<td class="list_item_bg1" align="left" width="60%">
																			<img height="12" alt="<%# WebSanitizer.HtmlEncode(((KbnAnalysisUtility.KbnAnalysisRow)Container.DataItem).Count) %>人" src="../../Images/graph_bar01.gif" width="<%# KbnAnalysisUtility.GetRateImgWidth(((KbnAnalysisUtility.KbnAnalysisRow)Container.DataItem).Count ,((KbnAnalysisUtility.KbnAnalysisTable)((RepeaterItem)(Container.Parent.Parent)).DataItem).Total) %>%" border="0" /></td>
																		<td class="list_item_bg1" align="right" width="10%">
																			<asp:Literal ID="lRate" runat="server" Text="<%# WebSanitizer.HtmlEncode(KbnAnalysisUtility.GetRateString(((KbnAnalysisUtility.KbnAnalysisRow)Container.DataItem).Count, ((KbnAnalysisUtility.KbnAnalysisTable)((RepeaterItem)(Container.Parent.Parent)).DataItem).Total, 1))%>" />%	</td>
																		<td class="list_item_bg1" align="right" width="10%">
																			<asp:Literal ID="lCount" runat="server" Text="<%# WebSanitizer.HtmlEncode(((KbnAnalysisUtility.KbnAnalysisRow)Container.DataItem).Count)%>" />
																			<asp:Literal ID="lUnit" runat="server" Text="<%# ((KbnAnalysisUtility.KbnAnalysisTable)((RepeaterItem)(Container.Parent.Parent)).DataItem).Unit %>" /></td>
																	</tr>
																</ItemTemplate>
															</asp:Repeater>
														</table>
													</td>
												</tr>
											</ItemTemplate>
										</asp:Repeater>
										<!--△ 分析結果 △-->
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="info_table" width="700" border="0" cellspacing="1" cellpadding="3">
													<tr class="info_item_bg">
														<td align="left">
															備考<br />
															・購入回数による新規/リピート分析、購入数量分析、購入金額分析、顧客区分分析、注文区分分析、決済区分分析は、<br />
															　返品、交換注文を含みません。<br />
															・キャンセル/返品/交換区分分析のキャンセルには、交換注文のキャンセルは含みません。
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