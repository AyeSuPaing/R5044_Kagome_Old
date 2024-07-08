<%--
=========================================================================================================
  Module      : 顧客状況詳細レポートページ(UserConditionReportDetail.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Reference Page="~/form/userconditionreport/userconditionreportlist.aspx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="UserConditionReportDetail.aspx.cs" Inherits="Form_UserConditionReport_UserConditionReportDetail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">ユーザー状況レポート</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr> <!--▽ 検索 ▽-->
	<tr>
		<td>
			<div class="tabs-wrapper">
				<a href='<%# WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_USER_CONDITION_REPORT_LIST + "?" + Constants.REQUEST_KEY_CURRENT_YEAR + "=" + m_iCurrentYear + "&" + Constants.REQUEST_KEY_CURRENT_MONTH + "=" + m_iCurrentMonth) %>'>
					レポート対象選択
				</a>
				<a href="#" class="current">詳細レポート対象選択</a>
			</div>
		</td>
	</tr>
	<tr>
		<td class="tab-contents">
			<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td class="search_box_bg">
						<table cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td class="search_box">
												<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
													<tr class="search_title_bg">
														<td width="30%"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />対象年月</td>
														<td width="70%"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />詳細レポート</td>
													</tr>
													<tr class="search_item_bg">
														<td>
															<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /><br />
															<!-- ▽カレンダ▽ --><asp:label id="lblCurrentCalendar" Runat="server"></asp:label>
															<!-- △カレンダ△ -->
															<asp:RadioButtonList id="rblReportType" Runat="server" AutoPostBack="True" RepeatColumns="2" CellSpacing="10" CssClass="radio_button_list">
																<asp:ListItem Value="0" Selected="True">日別レポート</asp:ListItem>
																<asp:ListItem Value="1">月別レポート</asp:ListItem>
															</asp:RadioButtonList></td>
														<td style="TEXT-ALIGN: center">
															<table class="detail_table" cellspacing="1" cellpadding="3" width="90%" border="0">
																<tr class="list_item_bg2">
																	<td>潜在ユーザ</td>
																</tr>
																<tr class="list_item_bg1">
																	<td><asp:CheckBoxList ID="cblPotential" Runat="server" RepeatDirection="Horizontal" Width="450" CssClass="radio_button_list" AutoPostBack="True">
																			<asp:ListItem Value="0" Selected="True">新規獲得</asp:ListItem>
																			<asp:ListItem Value="1">全体数</asp:ListItem>
																			<asp:ListItem Value="2">アクティブユーザ</asp:ListItem>
																			<asp:ListItem Value="3">休眠ユーザ</asp:ListItem>
																		</asp:CheckBoxList>
																</tr>
																<tr class="list_item_bg3">
																	<td>認知顧客</td>
																</tr>
																<tr class="list_item_bg1">
																	<td><asp:CheckBoxList ID="cblRecognize" Runat="server" RepeatDirection="Horizontal" Width="450" CssClass="radio_button_list" AutoPostBack="True">
																			<asp:ListItem Value="0" Selected="True">新規獲得</asp:ListItem>
																			<asp:ListItem Value="1">全体数</asp:ListItem>
																			<asp:ListItem Value="2">アクティブ顧客</asp:ListItem>
																			<asp:ListItem Value="3">休眠顧客</asp:ListItem>
																			<asp:ListItem Value="4">退会顧客</asp:ListItem>
																		</asp:CheckBoxList>
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
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<div class="search_btn_sub_rt" >
													<asp:LinkButton id="lbReportExport" Runat="server" OnClick="lbReportExport_Click">CSVダウンロード</asp:LinkButton></div>
												<table class="info_table" width="750" border="0" cellspacing="1" cellpadding="3">
													<tr class="info_item_bg">
														<td align="left" id="tdReportInfo" runat="server"></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="12" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<!--▽ レポート ▽-->
										<tr>
											<td>
												<div class="x_scrollable" style="WIDTH: 100%">
													<table class="list_table" cellspacing="1" cellpadding="3" border="0">
														<tr>
															<td class="list_title_bg" style="white-space:nowrap" width="40"></td>
															<td class="list_item_bg2" id="tdPotentialNewTitle" style="white-space:nowrap" align="center" width="160" colspan="2" runat="server">新規獲得</td>
															<td class="list_item_bg2" id="tdPotentialAllTitle" style="white-space:nowrap" align="center" width="160" colspan="2" runat="server">全体数</td>
															<td class="list_item_bg2" id="tdPotentialActiveTitle" style="white-space:nowrap" align="center" width="160" colspan="2" runat="server">アクティブユーザ</td>
															<td class="list_item_bg2" id="tdPotentialUnActiveTitle" style="white-space:nowrap" align="center" width="160" colspan="2" runat="server">休眠ユーザ</td>
															<td class="list_item_bg3" id="tdRecognizeNewTitle" style="white-space:nowrap" align="center" width="160" colspan="2" runat="server">新規獲得</td>
															<td class="list_item_bg3" id="tdRecognizeAllTitle" style="white-space:nowrap" align="center" width="160" colspan="2" runat="server">全体数</td>
															<td class="list_item_bg3" id="tdRecognizeActiveTitle" style="white-space:nowrap" align="center" width="160" colspan="2" runat="server">アクティブ顧客</td>
															<td class="list_item_bg3" id="tdRecognizeUnactiveTitle" style="white-space:nowrap" align="center" width="160" colspan="2" runat="server">休眠顧客</td>
															<td class="list_item_bg3" id="tdRecognizeLeaveTitle" style="white-space:nowrap" align="center" width="160" colspan="2" runat="server">退会顧客</td>
														</tr>
														<tr>
															<td class="list_title_bg" style="white-space:nowrap" width="40"></td>
															<td class="list_item_bg2" id="tdPotentialNewTitle1" style="white-space:nowrap" align="center" width="80" runat="server">人数</td>
															<td class="list_item_bg2" id="tdPotentialNewTitle2" style="white-space:nowrap" align="center" width="80" runat="server">増加率</td>
															<td class="list_item_bg2" id="tdPotentialAllTitle1" style="white-space:nowrap" align="center" width="80" runat="server">人数</td>
															<td class="list_item_bg2" id="tdPotentialAllTitle2" style="white-space:nowrap" align="center" width="80" runat="server">増加率</td>
															<td class="list_item_bg2" id="tdPotentialActiveTitle1" style="white-space:nowrap" align="center" width="80" runat="server">人数</td>
															<td class="list_item_bg2" id="tdPotentialActiveTitle2" style="white-space:nowrap" align="center" width="80" runat="server">増加率</td>
															<td class="list_item_bg2" id="tdPotentialUnActiveTitle1" style="white-space:nowrap" align="center" width="80" runat="server">人数</td>
															<td class="list_item_bg2" id="tdPotentialUnActiveTitle2" style="white-space:nowrap" align="center" width="80" runat="server">増加率</td>
															<td class="list_item_bg3" id="tdRecognizeNewTitle1" style="white-space:nowrap" align="center" width="80" runat="server">人数</td>
															<td class="list_item_bg3" id="tdRecognizeNewTitle2" style="white-space:nowrap" align="center" width="80" runat="server">増加率</td>
															<td class="list_item_bg3" id="tdRecognizeAllTitle1" style="white-space:nowrap" align="center" width="80" runat="server">人数</td>
															<td class="list_item_bg3" id="tdRecognizeAllTitle2" style="white-space:nowrap" align="center" width="80" runat="server">増加率</td>
															<td class="list_item_bg3" id="tdRecognizeActiveTitle1" style="white-space:nowrap" align="center" width="80" runat="server">人数</td>
															<td class="list_item_bg3" id="tdRecognizeActiveTitle2" style="white-space:nowrap" align="center" width="80" runat="server">増加率</td>
															<td class="list_item_bg3" id="tdRecognizeUnactiveTitle1" style="white-space:nowrap" align="center" width="80" runat="server">人数</td>
															<td class="list_item_bg3" id="tdRecognizeUnactiveTitle2" style="white-space:nowrap" align="center" width="80" runat="server">増加率</td>
															<td class="list_item_bg3" id="tdRecognizeLeaveTitle1" style="white-space:nowrap" align="center" width="80" runat="server">人数</td>
															<td class="list_item_bg3" id="tdRecognizeLeaveTitle2" style="white-space:nowrap" align="center" width="80" runat="server">増加率</td>
														</tr>
														<asp:Repeater id=rDataList Runat="server" DataSource='<%# m_alDispData %>'>
															<ItemTemplate>
																<tr>
																	<td class="list_title_bg" align="center"><%# WebSanitizer.HtmlEncode(((Hashtable)Container.DataItem)[Constants.FIELD_DISPUSERANALYSIS_TGT_DAY]) %></td>
																	<asp:Repeater DataSource='<%# ((Hashtable)Container.DataItem)["data"] %>' id="rLine" Runat="server">
																		<ItemTemplate>
																			<td class="list_item_bg1" align="right" width="80"><%# WebSanitizer.HtmlEncode(((string)((Hashtable)Container.DataItem)["data"] != "") ? StringUtility.ToNumeric(((Hashtable)Container.DataItem)["data"])+"人":"") %></td>
																			<td class="list_item_bg1" align="right" width="80"><%# WebSanitizer.HtmlEncode(((Hashtable)Container.DataItem)["rate"]) %></td>
																		</ItemTemplate>
																	</asp:Repeater>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
													</table>
													<br />
												</div>
											</td>
										</tr>
										<!--△ レポート △-->
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