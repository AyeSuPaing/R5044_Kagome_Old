<%--
=========================================================================================================
  Module      : インシデント集計ページ(IncidentReport.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="IncidentReport.aspx.cs" Inherits="Form_ReportIncident_IncidentReport" %>
<%@ Import Namespace="w2.App.Common.Cs.Reports" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderHead" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">インシデント集計</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<td>
		<!--▽ 上部レポート ▽-->
		<table class="box_border" cellspacing="0" cellpadding="3" width="784" border="0">
		<tr>
		<td>
		<table cellspacing="1" cellpadding="0" width="100%" border="0">
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
						■現在のステータス別件数
						<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
							<tr>
								<td class="detail_title_bg" align="center" width="10%">未対応</td>
								<td class="detail_title_bg" align="center" width="10%">対応中</td>
								<td class="detail_title_bg" align="center" width="10%">保留</td>
								<td class="detail_title_bg" align="center" width="10%">至急</td>
								<td class="detail_title_bg" align="center" width="10%">小計（未完了インシデント数）</td>
							</tr>
							<tr>
								<td class="detail_item_bg" align="center"><asp:Literal ID="lIncidentNoneCount" runat="server"></asp:Literal> 件</td>
								<td class="detail_item_bg" align="center"><asp:Literal ID="lIncidentActiveCount" runat="server"></asp:Literal> 件</td>
								<td class="detail_item_bg" align="center"><asp:Literal ID="lIncidentSuspendCount" runat="server"></asp:Literal> 件</td>
								<td class="detail_item_bg" align="center"><asp:Literal ID="lIncidentUrgentCount" runat="server"></asp:Literal> 件</td>
								<td class="detail_item_bg" align="center"><asp:Literal ID="lIncidentTotalCount" runat="server"></asp:Literal> 件</td>
							</tr>
						</table>
						<br />

						■インシデントの対応状況
						<asp:Repeater ID="rIncidentCompleteRepot" ItemType="w2.App.Common.Cs.Reports.IncidentActionCountByTermModel" runat="server">
						<HeaderTemplate>
							<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
								<tr>
									<td class="detail_title_bg" align="center" width="25%"></td>
									<td class="detail_title_bg" align="center" width="25%">発生件数</td>
									<td class="detail_title_bg" align="center" width="25%">完了件数</td>
									<td class="detail_title_bg" align="center" width="25%">完了/発生</td>
								</tr>
						</HeaderTemplate>
						<ItemTemplate>
							<tr>
								<td class="detail_title_bg" align="center" style="white-space: nowrap" >
									<span visible="<%# Item.DaySpan == 1 %>" runat="server">昨日</span>
									<span visible="<%# Item.DaySpan != 1 %>" runat="server">過去<%# Item.DaySpan %>日間</span><br />
									（<span visible="<%# Item.DaySpan != 1 %>" runat="server"><%# DateTimeUtility.ToStringForManager(DateTime.Now.AddDays(-1 * Item.DaySpan), DateTimeUtility.FormatType.ShortDate2Letter) %> ～ </span>
									<%# DateTimeUtility.ToStringForManager(DateTime.Now.AddDays(-1), DateTimeUtility.FormatType.ShortDate2Letter) %>）
								</td>
								<td class="detail_item_bg" align="center"><%#: StringUtility.ToNumeric(Item.Occurred) %> 件</td>
								<td class="detail_item_bg" align="center"><%#: StringUtility.ToNumeric(Item.Completed) %> 件</td>
								<td class="detail_item_bg" align="center"><%#: (Item.Occurred != 0) ? StringUtility.ToNumeric((100 * Item.Completed / Item.Occurred)) : "-" %> %</td>
							</tr>
						</ItemTemplate>
						<FooterTemplate>
							</table>
						</FooterTemplate>
						</asp:Repeater>
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
		</table>
		<!--△ 上部レポート △-->

		<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />

		<!--▽ 検索フォーム ▽-->
		<table class="box_border" cellspacing="0" cellpadding="3" width="784" border="0">
		<tr>
		<td>
		<table cellspacing="1" cellpadding="0" width="100%" border="0">
		<tr>
		<td class="search_box_bg">
		<table cellspacing="0" cellpadding="0" width="100%" border="0">
		<tr>
		<td align="center">
			<!--▽ 検索フォーム ▽-->
			<table cellspacing="0" cellpadding="0" border="0">
				<tr>
					<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
				</tr>
				<tr>
					<td class="search_box">
						<asp:UpdatePanel ID="up1" runat="server">
						<ContentTemplate>

						■検索
						<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
							<tr>
								<td class="search_title_bg" width="110">
									<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" border="0" />レポートタイプ</td>
								<td class="search_item_bg">
									<span class="radio_button_list">
										<asp:RadioButton ID="rbReportTypeGroup" GroupName="ReportType" Text="担当グループ別" AutoPostBack="true" OnCheckedChanged="rReportType_CheckedChanged" runat="server" />
										<asp:RadioButton ID="rbReportTypeOperator" GroupName="ReportType" Text="担当オペレータ別" AutoPostBack="true" OnCheckedChanged="rReportType_CheckedChanged" runat="server" />
										<asp:RadioButton ID="rbReportTypeGroupOperator" GroupName="ReportType" Text="担当グループ-オペレータ別" AutoPostBack="true" OnCheckedChanged="rReportType_CheckedChanged" runat="server" />
										<asp:RadioButton ID="rbReportTypeCategory" GroupName="ReportType" Text="インシデントカテゴリ別" AutoPostBack="true" OnCheckedChanged="rReportType_CheckedChanged" runat="server" /><br />
										<asp:RadioButton ID="rbReportTypeVoc" GroupName="ReportType" Text="VOC区分別" AutoPostBack="true" OnCheckedChanged="rReportType_CheckedChanged" runat="server" />
										<asp:RadioButton ID="rbReportTypeSummary" GroupName="ReportType" Text="集計区分別" AutoPostBack="true" OnCheckedChanged="rReportType_CheckedChanged" runat="server" />
										<asp:DropDownList ID="ddlReportSummaryKbn" Enabled="false" runat="server"></asp:DropDownList><br />
										<asp:RadioButton ID="rbReportTypeMonth" GroupName="ReportType" Text="月別" AutoPostBack="true" OnCheckedChanged="rReportType_CheckedChanged" runat="server" />
										<asp:RadioButton ID="rbReportTypeMonthDay" GroupName="ReportType" Text="月-日別" AutoPostBack="true" OnCheckedChanged="rReportType_CheckedChanged" runat="server" />
										<asp:RadioButton ID="rbReportTypeWeekday" GroupName="ReportType" Text="曜日別" AutoPostBack="true" OnCheckedChanged="rReportType_CheckedChanged" runat="server" />
										<asp:RadioButton ID="rbReportTypeTime" GroupName="ReportType" Text="時間帯別" AutoPostBack="true" OnCheckedChanged="rReportType_CheckedChanged" runat="server" />
										<asp:RadioButton ID="rbReportTypeWeekdayTime" GroupName="ReportType" Text="曜日-時間帯別" AutoPostBack="true" OnCheckedChanged="rReportType_CheckedChanged" runat="server" />
									</span>
								</td>
								<td class="search_btn_bg" width="83" rowspan="4">
									<div class="search_btn_main"><asp:button id="btnSearch" runat="server" Text="　検索　" OnClick="btnSearch_Click"></asp:button></div>
									<div class="search_btn_sub"><a href="<%= Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_INCIDENT_REPORT %>">クリア</a></div>
								</td>
							</tr>
							<tr>
								<td class="search_title_bg">
									<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" border="0" />期間<br />（100日以内）</td>
								<td class="search_item_bg">
									<uc:DateTimePickerPeriodInput id="ucCsWorkflowDateTimePickerPeriod" runat="server"></uc:DateTimePickerPeriodInput>
									<span class="search_btn_sub">(<a href="Javascript:SetToday();">今日</a>｜<a href="Javascript:SetThisMonth();">今月</a>)</span>
								</td>
							</tr>
							<tr>
								<td class="search_title_bg">
									<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" border="0" />対象インシデント</td>
								<td class="search_item_bg">
									<asp:RadioButtonList ID="rblDateType" CssClass="radio_button_list" RepeatLayout="Flow" RepeatDirection="Horizontal" runat="server">
										<asp:ListItem Text="発生" Value="CREATE"></asp:ListItem>
										<asp:ListItem Text="完了" Value="COMPLETE"></asp:ListItem>
									</asp:RadioButtonList>
								</td>
							</tr>

						</table>

						</ContentTemplate>
						<Triggers>
							<asp:PostBackTrigger ControlID="btnSearch" />
						</Triggers>
						</asp:UpdatePanel>

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
		</table>
		<!--△ 検索フォーム △-->
		</td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>

<table id="tblResult" cellspacing="0" cellpadding="0" width="791" border="0" visible="false" runat="server">
	<tr>
		<td>
			<h2 class="cmn-hed-h2">レポート一覧</h2>
		</td>
	</tr>
	<tr>
		<td>
			<!--▽ 検索結果 ▽-->
			<table class="box_border" cellspacing="0" cellpadding="0" width="784" border="0">
			<tr>
			<td>
			<table cellspacing="1" cellpadding="0" width="100%" border="0">
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
							<asp:repeater id="rList" Runat="server">
								<HeaderTemplate>
									<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
									<tr class="list_title_bg">
										<td align="<%# (rbReportTypeCategory.Checked || rbReportTypeGroupOperator.Checked) ? "left" : "center" %>" width="35">
											<%# WebSanitizer.HtmlEncode(GetReportTypeString()) %>
											<%# rbReportTypeSummary.Checked ? WebSanitizer.HtmlEncode("-" + ddlReportSummaryKbn.SelectedItem.Text) : "" %>
										</td>
										<td align="center" width="70">
											インシデント件数</td>
									</tr>
								</HeaderTemplate>
								<ItemTemplate>
									<tr class="list_item_bg1">
										<td align="<%= (rbReportTypeCategory.Checked || rbReportTypeGroupOperator.Checked || rbReportTypeWeekdayTime.Checked) ? "left" : "center" %>" style="<%# DrawTextLineThrough(Container.DataItem) ? "" : "text-decoration: line-through" %>">
											<%# WebSanitizer.HtmlEncode(GetDisplayName((ReportRowModel)Container.DataItem)) %></td>
										<td align="center">
											<%# WebSanitizer.HtmlEncode(((ReportRowModel)Container.DataItem).Count.HasValue ? StringUtility.ToNumeric(((ReportRowModel)Container.DataItem).Count) + " 件" : "-") %></td>
									</tr>
								</ItemTemplate>
								<FooterTemplate>
									</table>
								</FooterTemplate>
							</asp:repeater>
						</td>
					</tr>
				</table>
				<div id="dispReportListNoInfo" runat="server">
					<table class="list_table">
						<tr class="list_item_bg1" align="center">
							<td>データが存在しません</td>
						</tr>
					</table>
				</div>
			</td>
			</tr>
			</table>
			</td>
			</tr>
			</table>
			</td>
			</tr>
			</table>
			<!--△ 検索結果 △-->
		</td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<script type="text/javascript">
	// Set today
	function SetToday() {
		document.getElementById('<%= ucCsWorkflowDateTimePickerPeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
		document.getElementById('<%= ucCsWorkflowDateTimePickerPeriod.HfStartTime.ClientID %>').value = '00:00:00';
		document.getElementById('<%= ucCsWorkflowDateTimePickerPeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
		document.getElementById('<%= ucCsWorkflowDateTimePickerPeriod.HfEndTime.ClientID %>').value = '23:59:59';
		reloadDisplayDateTimePeriod('<%= ucCsWorkflowDateTimePickerPeriod.ID %>');
	}

	// Set this month
	function SetThisMonth() {
		document.getElementById('<%= ucCsWorkflowDateTimePickerPeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetStartDateThisMonthString() %>';
		document.getElementById('<%= ucCsWorkflowDateTimePickerPeriod.HfStartTime.ClientID %>').value = '00:00:00';
		document.getElementById('<%= ucCsWorkflowDateTimePickerPeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetEndDateThisMonthString() %>';
		document.getElementById('<%= ucCsWorkflowDateTimePickerPeriod.HfEndTime.ClientID %>').value = '23:59:59';
		reloadDisplayDateTimePeriod('<%= ucCsWorkflowDateTimePickerPeriod.ID %>');
	}

	if (window.history.replaceState) {
		window.history.replaceState(null, null, window.location.href);
	}
</script>
</asp:Content>

