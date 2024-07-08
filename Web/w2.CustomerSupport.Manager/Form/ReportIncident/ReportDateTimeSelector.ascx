<%--
=========================================================================================================
  Module      : 集計ページ向け日付選択ユーザーコントロール(ReportDateTimeSelector.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ReportDateTimeSelector.ascx.cs" Inherits="Form_ReportIncident_ReportDateTimeSelector" %>

<asp:DropDownList ID="ddlBeginDatePart1" runat="server"></asp:DropDownList>
<asp:DropDownList ID="ddlBeginDatePart2" runat="server"></asp:DropDownList>
<asp:DropDownList ID="ddlBeginDatePart3" runat="server"></asp:DropDownList>
～
<asp:DropDownList ID="ddlEndDatePart1" runat="server"></asp:DropDownList>
<asp:DropDownList ID="ddlEndDatePart2" runat="server"></asp:DropDownList>
<asp:DropDownList ID="ddlEndDatePart3" runat="server"></asp:DropDownList>
<div style="letter-spacing:4px; margin-left: 5px">
	<a href="Javascript:set_yesterday()">昨日</a>
	<a href="Javascript:set_today()">今日</a>
	<a href="Javascript:set_last_week()">先週</a>
	<a href="Javascript:set_this_week()">今週</a>
	<a href="Javascript:set_last_month()">先月</a>
	<a href="Javascript:set_this_month()">今月</a>
	<a href="Javascript:set_all()">直近100日</a>
</div>

<script type="text/javascript">
	function set_yesterday()
	{
		<% var yesterday = DateTime.Now.AddDays(-1); %>
		document.getElementById("<%= ddlBeginDatePart1.ClientID %>").value = '<%= GetDatePart1(yesterday) %>';
		document.getElementById("<%= ddlBeginDatePart2.ClientID %>").value = '<%= GetDatePart2(yesterday) %>';
		document.getElementById("<%= ddlBeginDatePart3.ClientID %>").value = '<%= GetDatePart3(yesterday) %>';
		document.getElementById("<%= ddlEndDatePart1.ClientID %>").value = '<%= GetDatePart1(yesterday) %>';
		document.getElementById("<%= ddlEndDatePart2.ClientID %>").value = '<%= GetDatePart2(yesterday) %>';
		document.getElementById("<%= ddlEndDatePart3.ClientID %>").value = '<%= GetDatePart3(yesterday) %>';
	}
	function set_today()
	{
		<% var today = DateTime.Now; %>
		document.getElementById("<%= ddlBeginDatePart1.ClientID %>").value = '<%= GetDatePart1(today) %>';
		document.getElementById("<%= ddlBeginDatePart2.ClientID %>").value = '<%= GetDatePart2(today) %>';
		document.getElementById("<%= ddlBeginDatePart3.ClientID %>").value = '<%= GetDatePart3(today) %>';
		document.getElementById("<%= ddlEndDatePart1.ClientID %>").value = '<%= GetDatePart1(today) %>';
		document.getElementById("<%= ddlEndDatePart2.ClientID %>").value = '<%= GetDatePart2(today) %>';
		document.getElementById("<%= ddlEndDatePart3.ClientID %>").value = '<%= GetDatePart3(today) %>';
	}
	function set_this_month()
	{
		<% var thisMonth = DateTime.Now.AddMonths(0); %>
		<% var thisMonthFrom = new DateTime(thisMonth.Year, thisMonth.Month, 1); %>
		<% var thisMonthTo = thisMonthFrom.AddMonths(1).AddDays(-1); %>
		document.getElementById("<%= ddlBeginDatePart1.ClientID %>").value = '<%= GetDatePart1(thisMonthFrom) %>';
		document.getElementById("<%= ddlBeginDatePart2.ClientID %>").value = '<%= GetDatePart2(thisMonthFrom) %>';
		document.getElementById("<%= ddlBeginDatePart3.ClientID %>").value = '<%= GetDatePart3(thisMonthFrom) %>';
		document.getElementById("<%= ddlEndDatePart1.ClientID %>").value = '<%= GetDatePart1(thisMonthTo) %>';
		document.getElementById("<%= ddlEndDatePart2.ClientID %>").value = '<%= GetDatePart2(thisMonthTo) %>';
		document.getElementById("<%= ddlEndDatePart3.ClientID %>").value = '<%= GetDatePart3(thisMonthTo) %>';
	}
	function set_last_month()
	{
		<% DateTime lastMonth = DateTime.Now.AddMonths(-1); %>
		<% var lastMonthFrom = new DateTime(lastMonth.Year, lastMonth.Month, 1); %>
		<% var lastMonthTo = lastMonthFrom.AddMonths(1).AddDays(-1); %>
		document.getElementById("<%= ddlBeginDatePart1.ClientID %>").value = '<%= GetDatePart1(lastMonthFrom) %>';
		document.getElementById("<%= ddlBeginDatePart2.ClientID %>").value = '<%= GetDatePart2(lastMonthFrom) %>';
		document.getElementById("<%= ddlBeginDatePart3.ClientID %>").value = '<%= GetDatePart3(lastMonthFrom) %>';
		document.getElementById("<%= ddlEndDatePart1.ClientID %>").value = '<%= GetDatePart1(lastMonthTo) %>';
		document.getElementById("<%= ddlEndDatePart2.ClientID %>").value = '<%= GetDatePart2(lastMonthTo) %>';
		document.getElementById("<%= ddlEndDatePart3.ClientID %>").value = '<%= GetDatePart3(lastMonthTo) %>';
	}
	function set_this_week()
	{
		<% DateTime firstDayOfWeek = DateTime.Now.AddDays(DayOfWeek.Sunday - DateTime.Now.DayOfWeek); %>
		<% DateTime lastDayOfWeek = firstDayOfWeek.AddDays(6); %>
		document.getElementById("<%= ddlBeginDatePart1.ClientID %>").value = '<%= GetDatePart1(firstDayOfWeek) %>';
		document.getElementById("<%= ddlBeginDatePart2.ClientID %>").value = '<%= GetDatePart2(firstDayOfWeek) %>';
		document.getElementById("<%= ddlBeginDatePart3.ClientID %>").value = '<%= GetDatePart3(firstDayOfWeek) %>';
		document.getElementById("<%= ddlEndDatePart1.ClientID %>").value = '<%= GetDatePart1(lastDayOfWeek) %>';
		document.getElementById("<%= ddlEndDatePart2.ClientID %>").value = '<%= GetDatePart2(lastDayOfWeek) %>';
		document.getElementById("<%= ddlEndDatePart3.ClientID %>").value = '<%= GetDatePart3(lastDayOfWeek) %>';
	}
	function set_last_week()
	{
		<% DateTime firstDayOfLastWeek = firstDayOfWeek.AddDays(-7); %>
		<% DateTime lastDayOfLastWeek = lastDayOfWeek.AddDays(-7); %>
		document.getElementById("<%= ddlBeginDatePart1.ClientID %>").value = '<%= GetDatePart1(firstDayOfLastWeek) %>';
		document.getElementById("<%= ddlBeginDatePart2.ClientID %>").value = '<%= GetDatePart2(firstDayOfLastWeek) %>';
		document.getElementById("<%= ddlBeginDatePart3.ClientID %>").value = '<%= GetDatePart3(firstDayOfLastWeek) %>';
		document.getElementById("<%= ddlEndDatePart1.ClientID %>").value = '<%= GetDatePart1(lastDayOfLastWeek) %>';
		document.getElementById("<%= ddlEndDatePart2.ClientID %>").value = '<%= GetDatePart2(lastDayOfLastWeek) %>';
		document.getElementById("<%= ddlEndDatePart3.ClientID %>").value = '<%= GetDatePart3(lastDayOfLastWeek) %>';
	}
	function set_all()
	{
		document.getElementById("<%= ddlBeginDatePart1.ClientID %>").value = '<%= GetDatePart1(DateTime.Now.AddDays(-100)) %>';
		document.getElementById("<%= ddlBeginDatePart2.ClientID %>").value = '<%= GetDatePart2(DateTime.Now.AddDays(-100)) %>';
		document.getElementById("<%= ddlBeginDatePart3.ClientID %>").value = '<%= GetDatePart3(DateTime.Now.AddDays(-100)) %>';
		document.getElementById("<%= ddlEndDatePart1.ClientID %>").value = '<%= GetDatePart1(DateTime.Now) %>';
		document.getElementById("<%= ddlEndDatePart2.ClientID %>").value = '<%= GetDatePart2(DateTime.Now) %>';
		document.getElementById("<%= ddlEndDatePart3.ClientID %>").value = '<%= GetDatePart3(DateTime.Now) %>';
	}

</script>