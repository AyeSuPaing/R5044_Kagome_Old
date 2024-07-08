<%--
=========================================================================================================
  Module      : アクセスレポートテーブル一覧ページ(AccessReportTableList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="AccessReportTableList.aspx.cs" Inherits=" Form_AccessReport_AccessReportTableList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">

<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">アクセスレポート</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr> <!--▽ 検索 ▽-->
	<tr>
		<td>
			<div class="tabs-wrapper">
				<a href="<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ACCESS_REPORT + "?" + Constants.REQUEST_KEY_CURRENT_YEAR + "=" + m_iCurrentYear + "&" + Constants.REQUEST_KEY_CURRENT_MONTH + "=" + m_iCurrentMonth + "&" + REQUEST_KEY_REPORT_TYPE + "=" + rblReportType.SelectedValue) %>">レポート対象選択</a>
				<a href="#" class="current">詳細レポート対象選択</a>
			</div>
		</td>
	</tr>
	<tr>
		<td class="tab-contents">
			<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<td class="search_box_bg">
						<table cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td class="search_box">
												<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
													<tr class="search_title_bg">
														<td>&nbsp;
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />対象年月</td>
													</tr>
													<tr class="search_item_bg" valign="top">
														<td>
															<table width="100%" border="0">
																<tr>
																	<td width="150">
																		<!-- ▽カレンダ▽ -->
																		<div align="left"><asp:label id="lblCurrentCalendar" Runat="server"></asp:label></div>
																		<!-- △カレンダ△ -->
																	</td>
																	<td>
																		<asp:RadioButtonList id="rblReportType" RepeatDirection="Horizontal" AutoPostBack="true" runat="server"></asp:RadioButtonList>
																	</td>
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
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr> <!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">詳細レポート表示</h2></td>
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
											<td><img height="8" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<div class="search_btn_sub_rt" >
													<asp:LinkButton id="lbReportExport" Runat="server" OnClick="lbReportExport_Click">CSVダウンロード</asp:LinkButton></div>
												<table class="info_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr class="info_item_bg">
														<td align="left"><%= WebSanitizer.HtmlEncode(GetAccessReportTableInfo()) %></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="list_title_bg" align="center" width="68"><%: DateTimeUtility.ToStringForManager(new DateTime(m_iCurrentYear, m_iCurrentMonth, 1), DateTimeUtility.FormatType.LongYearMonth) %></td>
														<td class="list_title_bg" align="center" width="115">ページビュー数</td>
														<% if (rblReportType.SelectedValue != KBN_REPORT_TYPE_MOBILE) {%>
															<td class="list_title_bg" align="center" width="115">訪問ユーザ数</td>
															<td class="list_title_bg" align="center" width="115">新規訪問ユーザ数</td>
															<td class="list_title_bg" align="center" width="115">リピート訪問<br />ユーザ数</td>
														<% } %>
														<td class="list_title_bg" align="center" width="115">訪問数</td>
														<td class="list_title_bg" align="center" width="120">1 回の訪問あたりの<br />平均ページビュー数</td>
													</tr>
													<tr valign="top">
														<td class="list_item_bg5_dark" colspan="8" style="padding:0px"></td>
													</tr>
													<tr valign="middle" height="20">
														<td class="list_title_bg" align="center">合計</td>
														<td class="list_item_bg1" align="center"><asp:Literal ID="lDataPvNumTotal" runat="server" /></td>
														<% if (rblReportType.SelectedValue != KBN_REPORT_TYPE_MOBILE) {%>
															<td class="list_item_bg1" align="center"><asp:Literal ID="lDataUserNumTotal" runat="server" /></td>
															<td class="list_item_bg1" align="center"><asp:Literal ID="lDataNewUserNumTotal" runat="server" /></td>
															<td class="list_item_bg1" align="center"><asp:Literal ID="lDataRepUserNumTotal" runat="server" /></td>
														<% } %>
														<td class="list_item_bg1" align="center"><asp:Literal ID="lDataSessionNumTotal" runat="server" /></td>
														<td class="list_item_bg1" align="center"><asp:Literal ID="lDataAvePvSessionNumTotal" runat="server" />(平均)</td>
														<asp:HiddenField ID="hfDataPvNumTotal" runat="server" value="<%= WebSanitizer.HtmlEncode(StringUtility.ToNumeric(m_htTotal[KBN_DISP_PV_NUM])) %>" />
													</tr>
													<tr valign="top">
														<td class="list_item_bg5_dark" colspan="8" style="padding:0px"></td>
													</tr>
													<asp:Repeater ID="rAccessReportTable" runat="server">
														<ItemTemplate>
															<tr valign="top">
																<td class="list_title_bg" align="center"><%#: DateTimeUtility.ToStringForManager(new DateTime(m_iCurrentYear, m_iCurrentMonth, Container.ItemIndex + 1), DateTimeUtility.FormatType.DayWeekOfDay) %></td>
																<td class='<%# GetWeekClass(m_iCurrentYear, m_iCurrentMonth, Container.ItemIndex + 1) %>' align="center"><asp:Literal ID="lDataPvNum" runat="server" Text="<%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(((Hashtable)Container.DataItem)[KBN_DISP_PV_NUM])) %>" /></td>
																<% if (rblReportType.SelectedValue != KBN_REPORT_TYPE_MOBILE) {%>
																	<td class="<%# GetWeekClass(m_iCurrentYear, m_iCurrentMonth, Container.ItemIndex + 1) %>" align="center"><asp:Literal ID="lDataUserNum" runat="server" Text='<%# ((rblReportType.SelectedValue == KBN_REPORT_TYPE_PC) || (rblReportType.SelectedValue == KBN_REPORT_TYPE_SMART_PHONE) || (rblReportType.SelectedValue == KBN_REPORT_TYPE_ALL)) ? WebSanitizer.HtmlEncode(StringUtility.ToNumeric(((Hashtable)Container.DataItem)[KBN_DISP_USER_NUM])) : "-" %>' /></td>
																	<td class="<%# GetWeekClass(m_iCurrentYear, m_iCurrentMonth, Container.ItemIndex + 1) %>" align="center"><asp:Literal ID="lDataNewUserNum" runat="server" Text='<%# ((rblReportType.SelectedValue == KBN_REPORT_TYPE_PC) || (rblReportType.SelectedValue == KBN_REPORT_TYPE_SMART_PHONE) || (rblReportType.SelectedValue == KBN_REPORT_TYPE_ALL)) ? WebSanitizer.HtmlEncode(StringUtility.ToNumeric(((Hashtable)Container.DataItem)[KBN_DISP_NEW_USER_NUM])) : "-" %>' /></td>
																	<td class="<%# GetWeekClass(m_iCurrentYear, m_iCurrentMonth, Container.ItemIndex + 1) %>" align="center"><asp:Literal ID="lDataRepUserNum" runat="server" Text='<%# ((rblReportType.SelectedValue == KBN_REPORT_TYPE_PC) || (rblReportType.SelectedValue == KBN_REPORT_TYPE_SMART_PHONE) || (rblReportType.SelectedValue == KBN_REPORT_TYPE_ALL)) ? WebSanitizer.HtmlEncode(StringUtility.ToNumeric(((Hashtable)Container.DataItem)[KBN_DISP_REP_USER_NUM])) : "-" %>' /></td>
																<% } %>
																<td class="<%# GetWeekClass(m_iCurrentYear, m_iCurrentMonth, Container.ItemIndex + 1) %>" align="center"><asp:Literal ID="lDataSessionNum" runat="server" Text="<%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(((Hashtable)Container.DataItem)[KBN_DISP_SESSION_NUM])) %>" /></td>
																<td class="<%# GetWeekClass(m_iCurrentYear, m_iCurrentMonth, Container.ItemIndex + 1) %>" align="center"><asp:Literal ID="lDataAvePvSessionNum" runat="server" Text="<%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(((Hashtable)Container.DataItem)[KBN_DISP_AVE_PVSESSION_NUM])) %>" /></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
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