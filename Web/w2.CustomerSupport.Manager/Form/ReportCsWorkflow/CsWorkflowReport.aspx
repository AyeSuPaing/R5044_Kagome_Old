<%--
=========================================================================================================
  Module      : CS業務フロー集計ページ(CsWorkflowReport.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="CsWorkflowReport.aspx.cs" Inherits="Form_ReportCsWorkflow_CsWorkflowReport" %>
<%@ Import Namespace="w2.App.Common.Cs.Reports" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderHead" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">業務フロー集計</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<td>

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

						<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
							<tr>
								<td class="search_title_bg" width="187">
									<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" border="0" />レポートタイプ</td>
								<td class="search_item_bg">
									<asp:RadioButtonList ID="rblReportType" CssClass="radio_button_list" RepeatLayout="Flow" RepeatDirection="Horizontal" runat="server">
										<asp:ListItem Text="回答者（オペレータ別）" Value="GroupOperator" Selected="True"></asp:ListItem>
									</asp:RadioButtonList>

								</td>
								<td class="search_btn_bg" width="83" rowspan="4">
									<div class="search_btn_main"><asp:button id="btnSearch" runat="server" Text="　検索　" OnClick="btnSearch_Click"></asp:button></div>
									<div class="search_btn_sub"><a href="<%= Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_CSWORKFLOW_REPORT %>">クリア</a></div>
								</td>
							</tr>
							<tr>
								<td class="search_title_bg">
									<img height="5" alt="" src="../../Images/Common/arrow_01.gif" border="0" />期間（100日以内）</td>
								<td class="search_item_bg">
									<div>
										<uc:DateTimePickerPeriodInput id="ucCsWorkflowDateTimePickerPeriod" runat="server" />
										<span class="search_btn_sub">(<a href="Javascript:SetToday();">今日</a>｜<a href="Javascript:SetThisMonth();">今月</a>)</span>
									</div>
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
										<td colspan="11" align="center">業務フロー</td>
									</tr>
									<tr class="list_title_bg">
										<td rowspan="2" width="10%" align="center">
											回答者<br />
											（オペレータ別）
										</td>
										<td align="center" colspan="2">承認依頼</td>
										<td align="center" colspan="2">承認対応</td>
										<td align="center" colspan="2">送信代行依頼</td>
										<td align="center" colspan="2">送信代行対応</td>
									</tr>
									<tr class="list_title_bg">
										<td align="center" width="5%">依頼数</td>
										<td align="center" width="5%">取下げ数</td>
										<td align="center" width="5%">完了<br />（承認）</td>
										<td align="center" width="5%">完了<br />（差戻し）</td>
										<td align="center" width="5%">依頼数</td>
										<td align="center" width="5%">取下げ数</td>
										<td align="center" width="5%">完了<br />（送信）</td>
										<td align="center" width="5%">完了<br />（差戻し）</td>
									</tr>
								</HeaderTemplate>
								<ItemTemplate>
									<tr class="list_item_bg1">
										<td align="left" style="<%# DrawTextLineThrough(Container.DataItem) ? "" : "text-decoration: line-through" %>">
											<%# WebSanitizer.HtmlEncode(GetDisplayName((ReportMatrixRowModelForCsWorkflow)Container.DataItem)) %></td>
										<td align="center">
											<%# WebSanitizer.HtmlEncode(DispReportValue((((ReportMatrixRowModelForCsWorkflow)Container.DataItem).ApprReqCount), "0")) %> 件</td>
										<td align="center">
											<%# WebSanitizer.HtmlEncode(DispReportValue((((ReportMatrixRowModelForCsWorkflow)Container.DataItem).ApprReqCancelCount), "0")) %> 件</td>
										<td align="center">
											<%# WebSanitizer.HtmlEncode(DispReportValue((((ReportMatrixRowModelForCsWorkflow)Container.DataItem).ApprOkCount), "0")) %> 件</td>
										<td align="center">
											<%# WebSanitizer.HtmlEncode(DispReportValue((((ReportMatrixRowModelForCsWorkflow)Container.DataItem).ApprNgCount), "0")) %> 件</td>
										<td align="center">
											<%# WebSanitizer.HtmlEncode(DispReportValue((((ReportMatrixRowModelForCsWorkflow)Container.DataItem).SendReqCount), "0")) %> 件</td>
										<td align="center">
											<%# WebSanitizer.HtmlEncode(DispReportValue((((ReportMatrixRowModelForCsWorkflow)Container.DataItem).SendReqCancelCount), "0")) %> 件</td>
										<td align="center">
											<%# WebSanitizer.HtmlEncode(DispReportValue((((ReportMatrixRowModelForCsWorkflow)Container.DataItem).SendOkCount), "0")) %> 件</td>
										<td align="center">
											<%# WebSanitizer.HtmlEncode(DispReportValue((((ReportMatrixRowModelForCsWorkflow)Container.DataItem).SendNgCount), "0")) %> 件</td>
									</tr>
								</ItemTemplate>
								<FooterTemplate>
									</table>
								</FooterTemplate>
							</asp:repeater>
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

