<%--
=========================================================================================================
  Module      : アクセスレポート一覧ページ(AccessReportList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="AccessReportList.aspx.cs" Inherits=" Form_AccessReport_AccessReportList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.7.2/Chart.bundle.js"></script>
<script src="../../Js/Manager.chart.js"></script>
<table width="791" border="0">
<tr>
	<td>
		<h1 class="page-title">アクセスレポート</h1>
	</td>
</tr>
<tr>
	<td>
		<img height="10" alt="" src="../../Images/Common/sp.gif" width="1" border="0"/>
	</td>
</tr> <!--▽ 検索 ▽-->
<tr>
	<td>
		<div class="tabs-wrapper">
			<a href="#" class="current">レポート対象選択</a>
			<a href="<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ACCESS_TABLE_REPORT + "?" + Constants.REQUEST_KEY_CURRENT_YEAR + "=" + m_iCurrentYear + "&" + Constants.REQUEST_KEY_CURRENT_MONTH + "=" + m_iCurrentMonth + "&" + REQUEST_KEY_REPORT_TYPE + "=" + rblReportType.SelectedValue) %>">詳細レポート対象選択</a>
		</div>
	</td>
</tr>
<tr>
	<td class="tab-contents">
		<table class="box_border" cellpadding="0" width="784" border="0">
			<tr>
				<td class="search_box_bg">
					<table cellpadding="0" width="100%" border="0">
						<tr>
							<td align="center">
								<table cellspacing="0" cellpadding="0" border="0">
									<tr>
										<td>
											<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/>
										</td>
									</tr>
									<tr>
										<td class="search_box">
											<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
												<tr class="search_title_bg">
													<td width="30%">
														&nbsp;
														<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align: middle" border="0"/>対象年月
													</td>
													<td width="70%">
														<table class="trans_table" width="100%" border="0">
															<tr>
																<td style="vertical-align: middle">
																	&nbsp;
																	<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align: middle" border="0"/>
																	グラフレポート
																</td>
																<td style="text-align: right">
																	<asp:RadioButtonList id="rblChartType" Runat="server" RepeatDirection="Horizontal" AutoPostBack="True" CssClass="radio_button_list">
																		<asp:ListItem Value="0" Selected="True">棒グラフ</asp:ListItem>
																		<asp:ListItem Value="1">折れ線グラフ</asp:ListItem>
																	</asp:RadioButtonList>
																</td>
															</tr>
														</table>
													</td>
												</tr>
												<tr class="search_item_bg" valign="top">
													<td>
														<br/>
														<!-- ▽カレンダ▽ -->
														<asp:label id="lblCurrentCalendar" Runat="server"></asp:label>
														<!-- △カレンダ△ -->
														<br/>
														<div align="center">
															<asp:RadioButtonList id="rblReportType" RepeatDirection="Horizontal" AutoPostBack="true" runat="server"></asp:RadioButtonList>
														</div>
														<table width="95%" align="center" border="1">
															<tr>
																<td>
																	<asp:RadioButton ID="rbPV" Runat="server" Text="ページビュー数" AutoPostBack="True" GroupName="ReportType" Checked="True"></asp:RadioButton><br/>
																	<asp:RadioButton ID="rbUsers" Runat="server" Text="訪問ユーザ数" AutoPostBack="True" GroupName="ReportType"></asp:RadioButton><br/>
																	&nbsp;&nbsp;&nbsp;<asp:RadioButton ID="rbNewUsers" Text="新規訪問ユーザ数" Runat="server" AutoPostBack="True" GroupName="ReportType"></asp:RadioButton><br/>
																	&nbsp;&nbsp;&nbsp;<asp:RadioButton ID="rbRepeatUsers" Runat="server" Text="リピート訪問ユーザ数" AutoPostBack="True" GroupName="ReportType"></asp:RadioButton><br/>
																	<asp:RadioButton ID="rbSessions" Runat="server" Text="訪問数" AutoPostBack="True" GroupName="ReportType"></asp:RadioButton><br/>
																	<asp:RadioButton ID="rbAveragePVSession" Runat="server" Text="1 回の訪問あたりの平均ページビュー数" AutoPostBack="True" GroupName="ReportType"></asp:RadioButton><br/>
																</td>
															</tr>
														</table>
													</td>
													<td>
														<canvas id="Chart" align="middle" style="margin-top: 20px; display: block; height: 400px; width: 1000px;"></canvas>
														<asp:Literal runat="server" ID="lCreateScript"></asp:Literal>
														<div style="float: left">
															<asp:RadioButtonList id="rblCheckNumber" runat="server" Width="160px" RepeatDirection="Horizontal" AutoPostBack="True" CssClass="radio_button_list">
																<asp:ListItem Value="0" >数値あり</asp:ListItem>
																<asp:ListItem Value="1" Selected="true">数値なし</asp:ListItem>
															</asp:RadioButtonList>
														</div>
													</td>
												</tr>
											</table>
										</td>
									</tr>
									<tr>
										<td>
											<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/>
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
</tr> <!--△ 検索 △-->
<tr>
	<td>
		<img height="10" alt="" src="../../Images/Common/sp.gif" width="1" border="0"/>
	</td>
</tr> <!--▽ 一覧 ▽-->
<tr>
	<td>
		<h2 class="cmn-hed-h2">レポート表示</h2>
	</td>
</tr>
<tr>
	<td>
		<table class="box_border" cellpadding="0" width="784" border="0">
			<tr>
				<td class="list_box_bg">
					<table cellpadding="0" width="100%" border="0">
						<tr>
							<td align="center">
								<table cellspacing="0" cellpadding="0" border="0">
									<tr>
										<td>
											<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/>
										</td>
									</tr>
									<tr>
										<td>
											<div class="search_btn_sub_rt">
												<asp:LinkButton id="lbReportExport" Runat="server" OnClick="lbReportExport_Click">CSVダウンロード</asp:LinkButton>
											</div>
											<div class="x_scrollable">
												<table class="list_table" cellspacing="1" cellpadding="2" width="<%= (m_alTableData.Count + 1) * 30 %>" border="0">
													<tr class="list_title_bg">
														<td colspan="<%= m_alTableData.Count + 1 %>">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align: middle" border="0"/>
															<%: DateTimeUtility.ToStringForManager(new DateTime(m_iCurrentYear, m_iCurrentMonth, 1), DateTimeUtility.FormatType.LongYearMonth) %>
															&nbsp;
															【<%= rblReportType.Items.FindByValue(rblReportType.SelectedValue).Text %>】 <%= CreateDisplayKbnStrinig() %>
														</td>
													</tr>
													<tr class="list_item_bg1" valign="top">
														<asp:Repeater ID="rTableHeader" runat="server">
															<ItemTemplate>
																<td width="30" class="<%# CreateTableDayClassName(m_iCurrentYear + "/" + m_iCurrentMonth + "/" + (Container.ItemIndex + 1)) %>">
																	<div align="center">
																		<asp:Literal ID="lHeader" runat="server"
																					Text="<%# DateTimeUtility.ToStringForManager(new DateTime(m_iCurrentYear, m_iCurrentMonth, (Container.ItemIndex + 1)), DateTimeUtility.FormatType.ShortMonthDay) %>"/>
																	</div>
																</td>
															</ItemTemplate>
														</asp:Repeater>
													</tr>
													<tr class="list_item_bg1" valign="top">
														<asp:Repeater ID="rTableData" runat="server">
															<ItemTemplate>
																<td width="30">
																	<%-- ここの値を取得してくる --%>
																	<div align="center">
																		<asp:Literal ID="lData" runat="server" Text="<%# WebSanitizer.HtmlEncode(((Hashtable)Container.DataItem)[TABLE_DATA_VALUE]) %>"/>
																	</div>
																</td>
															</ItemTemplate>
														</asp:Repeater>
													</tr>
												</table>
											</div>
											<!-- 備考 -->
											<img height="30" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/>
											<table class="info_table" width="758" border="0" cellspacing="1" cellpadding="3">
												<tr class="info_item_bg">
													<td align="left">
														<% if (Constants.PATH_ROOT_FRONT_MOBILE != "")
														   { %>
															<table>
																<tr>
																	<td>■対象</td>
																</tr>
																<tr>
																	<td>訪問ユーザ数（新規・リピート）はPCサイトおよびスマフォサイトが対象となります。</td>
																</tr>
															</table>
															<br/>
														<% } %>
														<table>
															<tr>
																<td colspan="3">■項目説明</td>
															</tr>
															<tr>
																<td>・ページビュー数</td>
																<td>・・・</td>
																<td>ユーザーがページを閲覧した回数（PV数）</td>
															</tr>
															<% if (Constants.PATH_ROOT_FRONT_MOBILE != "")
															   { %>
																<tr>
																	<td>・訪問ユーザ数</td>
																	<td>・・・</td>
																	<td>クッキーにて識別されたユニークなユーザ数</td>
																</tr>
															<% } %>
															<tr>
																<td>・訪問数</td>
																<td>・・・</td>
																<td>セッション（ブラウザアクセスの開始～終了まで）の数</td>
															</tr>
															<tr>
																<td>・1回の訪問あたりの平均ページビュー数</td>
																<td>・・・</td>
																<td>ページビュー数/訪問数</td>
															</tr>
														</table>
													</td>
												</tr>
											</table>
										</td>
									</tr>
									<tr>
										<td>
											<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/>
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
</tr> <!--△ 一覧 △-->
<tr>
	<td>
		<img height="10" alt="" src="../../Images/Common/sp.gif" width="1" border="0"/>
	</td>
</tr>
</table>
</asp:Content>