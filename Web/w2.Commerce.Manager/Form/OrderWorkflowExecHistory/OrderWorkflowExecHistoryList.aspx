<%--
=========================================================================================================
  Module      : 受注ワークフロー実行履歴ページ(OrderWorkflowExecHistory.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>
<%@ Reference Page="~/Form/PdfOutput/PdfOutput.aspx" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<%@ Page Language="C#" Title="ワークフロー実行履歴" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="OrderWorkflowExecHistoryList.aspx.cs" Inherits="Form.OrderWorkflowExecHistory.OrderWorkflowExecHistoryList"
MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td>
			<h1 class="page-title">ワークフロー実行履歴</h1>
		</td>
	</tr>
	<tr>
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 検索 ▽-->
	<tr>
		<td style="width: 792px">
			<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
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
											<td class="search_table">
												<table cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="search_title_bg" width="70">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															受注ワークフロー
														</td>
														<td class="search_item_bg" width="200">
															<asp:DropDownList ID="ddlWorkflowList" runat="server" Width="100%"></asp:DropDownList>
														</td>
														<% if (Constants.ORDERWORKFLOW_AUTOEXEC_OPTION_ENABLE){ %>
															<td class="search_title_bg" width="80">
																<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
																シナリオワークフロー
															</td>
															<td class="search_item_bg" width="200">
																<asp:DropDownList ID="ddlScenarioList" runat="server" Width="100%"></asp:DropDownList>
															</td>
														<% } %>
														<td class="search_btn_bg" width="88" rowspan="4">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click"/></div>
															<div class="search_btn_sub">
																<a href="<%: Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERWORKFLOW_EXEC_HISTORY_LIST %>">クリア</a>
																<a href="javascript:Reset();">リセット</a></div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															実行ステータス
														</td>
														<td class="search_item_bg">
															<asp:CheckBoxList id="cblExecStatus" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList>
														</td>
														<% if (Constants.ORDERWORKFLOW_AUTOEXEC_OPTION_ENABLE){ %>
															<td class="search_title_bg">
																<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
																実行起点
															</td>
															<td class="search_item_bg">
																<asp:CheckBoxList id="cblExecPlace" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList>
															</td>
														<% } %>
													</tr>
													<tr>
														<% if (Constants.ORDERWORKFLOW_AUTOEXEC_OPTION_ENABLE){ %>
															<td class="search_title_bg">
																<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
																実行タイミング
															</td>
															<td class="search_item_bg">
																<asp:CheckBoxList id="cblExecTiming" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList>
															</td>
														<% } %>
														<td id="tdWorkdlowTypeCheckBoxListTitle" class="search_title_bg" runat="server">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															ワークフロー種別
														</td>
														<td id="tdWorkdlowTypeCheckBoxList" class="search_item_bg" runat="server">
															<asp:CheckBoxList id="cblWorkflowType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															履歴作成日時
														</td>
														<% if (Constants.ORDERWORKFLOW_AUTOEXEC_OPTION_ENABLE){ %>
															<td class="search_item_bg" colspan="3">
														<% } else { %>
															<td class="search_item_bg" colspan="1">
														<% } %>
															<div id="searchDate">
																<uc:DateTimePickerPeriodInput id="ucSearchDatePeriod" runat="server" IsNullStartDateTime="true" />
																の間
																<span class="search_btn_sub">(<a href="Javascript:SetYesterday();">昨日</a>｜<a href="Javascript:SetToday();">今日</a>｜<a href="Javascript:SetThisMonth();">今月</a>)</span>
															</div>
														</td>
													</tr>
													<!-- <tr>
														<td class="search_title_bg" width="95" rowspan>
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															メッセージ</td>
														<td class="search_item_bg" width="230" colspan="3">
															<asp:TextBox id="tbLogMessage" runat="server" size="80"></asp:TextBox></td>
													</tr> -->
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
	</tr>
	<!--△ 検索 △-->
	<tr>
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">履歴一覧</h2></td>
	</tr>
	<tr>
		<td style="width: 792px">
			<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<!--▽ ページング ▽-->
												<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
													<tr>
														<td width="675" style="height: 22px"><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
													</tr>
												</table>
												<!-- ページング-->
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="list_title_bg">
														<td align="center" width="5%" style="height: 17px">履歴ID</td>
														<td align="center" width="8%" style="height: 17px">開始日時<br>終了日時</td>
														<td align="center" width="9%" style="height: 17px">実行ステータス</td>
														<td align="center" width="10%" style="height: 17px">成功件数/実行件数</td>
														<% if (Constants.ORDERWORKFLOW_AUTOEXEC_OPTION_ENABLE){ %>
															<td align="center" width="8%" style="height: 17px">実行起点</td>
															<td align="center" width="21%" style="height: 17px">ワークフロー名<br>(シナリオワークフロー名)</td>
														<% } else { %>
															<td align="center" width="21%" style="height: 17px">ワークフロー名</td>
														<% } %>
														<td align="center" width="9%" style="height: 17px">ワークフロー種別</td>
														<% if (Constants.ORDERWORKFLOW_AUTOEXEC_OPTION_ENABLE){ %>
															<td align="center" width="9%" style="height: 17px">実行タイミング</td>
														<% } %>
														<td align="center" width="9%" style="height: 17px">実行者</td>
														<td align="center" width="8%" style="height: 17px">履歴作成日時</td>
														<td align="center" width="4%" style="height: 17px">詳細</td>
													</tr>
													<asp:Repeater id="rHistoryList" Runat="server" ItemType="w2.Domain.OrderWorkflowExecHistory.OrderWorkflowExecHistoryModel">
														<ItemTemplate>
															<tr class="<%#: GetCssClassToListItemBackGround(Item.ExecStatus) %>" >
																<td align="center"><%#: Item.OrderWorkflowExecHistoryId %></td>
																<td align="center"><%#: DateTimeUtility.ToStringForManager(Item.DateBegin, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %><br><br><%#: DateTimeUtility.ToStringForManager(Item.DateEnd, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
																<td align="center" style="font-weight: <%#: (Item.ExecStatus == Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_RUNNING) ? "bold" : "normal" %>"><%#: ConvertExecStatusForDisplay(Item.ExecStatus) %></td>
																<td align="center"><%#: Item.SuccessRate %></td>
																<% if (Constants.ORDERWORKFLOW_AUTOEXEC_OPTION_ENABLE){ %>
																	<td align="center"><%#: ConvertExecPlaceForDisplay(Item.ExecPlace) %></td>
																<% } %>
																<td align="center"><%#: Item.WorkflowName %><br/><%#: ConvertScenarioNameForDisplay(Item.ScenarioName) %></td>
																<td align="center"><%#: ConvertWorkflowTypeForDisplay(Item.WorkflowType) %></td>
																<% if (Constants.ORDERWORKFLOW_AUTOEXEC_OPTION_ENABLE){ %>
																	<td align="center"><%#: ConvertExecTimingForDisplay(Item.ExecTiming) %></td>
																<% } %>
																<td align="center"><%#: Item.LastChanged %></td>
																<td align="center"><%#: DateTimeUtility.ToStringForManager(Item.DateCreated, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
																<td align="center">
																	<a href="<%#: CreateExecHistoryDetailsUrl(Item.OrderWorkflowExecHistoryId) %>">参照</a>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="11"></td>
													</tr>
												</table>
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
	</tr>
	<!--△ 一覧 △-->
	<tr>
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>

<script type="text/javascript">
	// 昨日設定
	function SetYesterday() {
		document.getElementById('<%= ucSearchDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.Date.AddDays(-1)) %>';
		document.getElementById('<%= ucSearchDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
		document.getElementById('<%= ucSearchDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.Date.AddDays(-1)) %>';
		document.getElementById('<%= ucSearchDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
		reloadDisplayDateTimePeriod('<%= ucSearchDatePeriod.ClientID %>');
	}

	// 今日設定
	function SetToday() {
		document.getElementById('<%= ucSearchDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
		document.getElementById('<%= ucSearchDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
		document.getElementById('<%= ucSearchDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
		document.getElementById('<%= ucSearchDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
		reloadDisplayDateTimePeriod('<%= ucSearchDatePeriod.ClientID %>');
	}

	// 今月設定
	function SetThisMonth() {
		document.getElementById('<%= ucSearchDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetStartDateThisMonthString() %>';
		document.getElementById('<%= ucSearchDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
		document.getElementById('<%= ucSearchDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetEndDateThisMonthString() %>';
		document.getElementById('<%= ucSearchDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
		reloadDisplayDateTimePeriod('<%= ucSearchDatePeriod.ClientID %>');
	}

	// Reset
	function Reset() {
		document.<%= this.Form.ClientID %>.reset();
		document.getElementById('<%= ucSearchDatePeriod.HfStartDate.ClientID %>').value = '';
		document.getElementById('<%= ucSearchDatePeriod.HfStartTime.ClientID %>').value = '';
		document.getElementById('<%= ucSearchDatePeriod.HfEndDate.ClientID %>').value = '';
		document.getElementById('<%= ucSearchDatePeriod.HfEndTime.ClientID %>').value = '';
		reloadDisplayDateTimePeriod('<%= ucSearchDatePeriod.ClientID %>');
	}
</script>

</asp:Content>
