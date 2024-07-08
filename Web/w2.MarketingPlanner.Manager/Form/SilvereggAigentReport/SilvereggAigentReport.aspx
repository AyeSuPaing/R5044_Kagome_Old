<%--
=========================================================================================================
  Module      : シルバーエッグレコメンド結果レポートページ(SilvereggAigentReport.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="SilvereggAigentReport.aspx.cs" Inherits="Form_SilvereggAigentReport_SilvereggAigentReport" MaintainScrollPositionOnPostback="true" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">

<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<tb><h1 class="page-title">レコメンド結果レポート</h1></tb>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr> <!--▽ 検索 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">レポート対象選択</h2></td>
	</tr>
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
						<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
					</tr>
					<tr>
					<td class="search_box">
						<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
						<tr class="search_title_bg">
							<td width="30%" colspan="2"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />対象年月</td>
						</tr>
						<tr class="search_item_bg">
						<td width="200">
							<!-- ▽カレンダ▽ --><asp:label id="lblCurrentCalendar" Runat="server"></asp:label>
							<!-- △カレンダ△ -->
						</td>
						<td width="558">
							<table cellspacing="0" cellpadding="0" border="0" class="order-condition-report-list-search">
							<tr>
							<td class="search_box" width="5"></td>
							<td class="search_box">
							<table cellspacing="0" cellpadding="0" border="0">
								<tr>
								<td class="search_box" valign="top">
									<table class="search_table" cellspacing="1" cellpadding="2" width="100" border="0">
									<tr class="search_title_bg">
									<td width="30%"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />レポート種類</td>
									<td width="30%"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />月日種別</td>
									</tr>
									<tr class="search_item_bg">
									<td>
										<asp:RadioButtonList id="rblReportType" Runat="server" AutoPostBack="True" CssClass="radio_button_list">
											<asp:ListItem Value="0" Selected="True">通常レポート</asp:ListItem>
											<asp:ListItem Value="1">レコメンドID毎レポート</asp:ListItem>
										</asp:RadioButtonList>
									</td>
									<td>
										<asp:RadioButtonList id="rblDateType" Runat="server" AutoPostBack="True" CssClass="radio_button_list">
											<asp:ListItem Value="0" Selected="True">日別レポート</asp:ListItem>
											<asp:ListItem Value="1">月別レポート</asp:ListItem>
										</asp:RadioButtonList>
									</td>
									</tr>
									</table>
								</td>
								<td class="search_box" width="5"></td>
								</tr>
							</table>
							</td>
							</tr>
							<tr>
								<td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
							</tr>
							<tr>
							<td class="search_box" width="5"></td>
							<td class="search_box">
								<table cellspacing="0" cellpadding="0" border="0">
								<tr>
								<td>
									<table class="search_table" cellspacing="1" cellpadding="2" border="0">
									<tr>
									<td class="search_title_bg" >
										<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />期間指定
									</td>
									</tr>
									<tr>
									<td class="search_item_bg" >
										<uc:DateTimePickerPeriodInput ID="ucDatePeriod" runat="server" />
										<span style="display:<%= (rblReportType.SelectedValue == KBN_REPORT_TYPE_PERSPEC_REPORT) ? "none" : "" %>">
											<br>
										<asp:Label ID="lblOrderContitionError" style="color:#f00" runat="server"></asp:Label>
										</span>
									</td>
									</tr>
									</table>
								</td>
								</tr>
								</table>
							</td>
							</tr>
							<tr>
								<td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
							</tr>
							<tr style="display:<%= (rblReportType.SelectedValue == KBN_REPORT_TYPE_DEFAULT_REPORT) ? "" : "none" %>">
								<td class="search_box" width="5"></td>
								<td class="search_box">
								<table cellspacing="0" cellpadding="0" border="0">
								<tr>
								<td>
									<table class="search_table" cellspacing="1" cellpadding="2" border="0">
									<tr>
									<td class="search_title_bg" >
										<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />レコメンドID
									</td>
									</tr>
									<tr>
									<td class="search_item_bg" >
										<asp:DropDownList id="ddlSpecIdList" runat="server" AutoPostBack="True"></asp:DropDownList>
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
		</td>
	</tr> <!--△ 検索 △-->
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
		<td class="list_box_bg">
			<table cellspacing="0" cellpadding="0" width="100%" border="0">
			<tr>
			<td align="center">
				<table cellspacing="0" cellpadding="0" border="0">
				<tr>
					<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
				</tr>
				<tr>
				<td>
					<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
						<% if (rblReportType.SelectedValue == KBN_REPORT_TYPE_DEFAULT_REPORT) { %>
						<tr class="list_title_bg">
							<td align="center" width="158">日付</td>
							<td align="center" width="150">表示回数</td>
							<td align="center" width="150">クリック数</td>
							<td align="center" width="150">受注件数</td>
							<td align="center" width="150">受注金額</td>
						</tr>
						<% } else { %>
						<tr class="list_title_bg">
							<td align="center" width="158">レコメンドID</td>
							<td align="center" width="150">表示回数</td>
							<td align="center" width="150">クリック数</td>
							<td align="center" width="150">受注件数</td>
							<td align="center" width="150">受注金額</td>
						</tr>
						<% } %>
						<asp:Repeater id="rList" ItemType="System.Collections.Hashtable" Runat="server">
							<ItemTemplate>
								<% if (rblReportType.SelectedValue == KBN_REPORT_TYPE_DEFAULT_REPORT) { %>
								<tr>
									<td align="center">
										<%#: DateTimeUtility.ToStringForManager(
											     Item[FIELD_RESPONSE_DATE],
											     (rblDateType.SelectedValue == KBN_DATE_TYPE_DAILY_REPORT)
												     ? DateTimeUtility.FormatType.ShortDate2Letter
												     : DateTimeUtility.FormatType.ShortYearMonth) %></td>
									<td align="center">
										<%#: ((Hashtable)Container.DataItem)[FIELD_RESPONSE_IMPRESSION] %></td>
									<td align="center">
										<%#: ((Hashtable)Container.DataItem)[FIELD_RESPONSE_CLICK] %></td>
									<td align="center">
										<%#: ((Hashtable)Container.DataItem)[FIELD_RESPONSE_PURCHASE] %></td>
									<td align="center">
										<%#: ((Hashtable)Container.DataItem)[FIELD_RESPONSE_SALES] %></td>
								</tr>
								<% } else { %>
								<tr>
									<td align="center">
										<%#: ((Hashtable)Container.DataItem)[FIELD_RESPONSE_SPECNAME] %></td>
									<td align="center">
										<%#: ((Hashtable)Container.DataItem)[FIELD_RESPONSE_IMPRESSION] %></td>
									<td align="center">
										<%#: ((Hashtable)Container.DataItem)[FIELD_RESPONSE_CLICK] %></td>
									<td align="center">
										<%#: ((Hashtable)Container.DataItem)[FIELD_RESPONSE_PURCHASE] %></td>
									<td align="center">
										<%#: ((Hashtable)Container.DataItem)[FIELD_RESPONSE_SALES] %></td>
								</tr>
								<% } %>
							</ItemTemplate>
						</asp:Repeater>
						<tr id="trListError" class="list_alert" runat="server" Visible="false">
							<td id="tdErrorMessage" runat="server" colspan="5">test</td>
						</tr>
					</table>
				</td>
				</tr>
				<tr>
					<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
