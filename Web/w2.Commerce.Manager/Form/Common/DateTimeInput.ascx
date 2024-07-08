<%--
=========================================================================================================
  Module      : 日付入力コントロール(DateTimeInput.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DateTimeInput.ascx.cs" Inherits="Form_Common_DateTimeInput" %>
<asp:DropDownList CssClass="<%# this.ID %>" ID="ddlDatePart1" runat="server" DataSource="<%# this.DataSourceDatePart1 %>" AutoPostBack="<%# this.ControlAutoPostBack %>" OnSelectedIndexChanged="ddlDateTime_SelectedIndexChanged" SelectedValue="<%# this.SelectedValueDatePart1 %>"
				DataTextField="Text" DataValueField="Value" />
<span id="spSeparator1" runat="server">/</span>
<asp:DropDownList ID="ddlDatePart2" runat="server" DataSource="<%# this.DataSourceDatePart2 %>" AutoPostBack="<%# this.ControlAutoPostBack %>" OnSelectedIndexChanged="ddlDateTime_SelectedIndexChanged" SelectedValue="<%# this.SelectedValueDatePart2 %>"
				DataTextField="Text" DataValueField="Value" />
<span id="spSeparator2" runat="server">/</span>
<asp:DropDownList ID="ddlDatePart3" runat="server" DataSource="<%# this.DataSourceDatePart3 %>" AutoPostBack="<%# this.ControlAutoPostBack %>" OnSelectedIndexChanged="ddlDateTime_SelectedIndexChanged" SelectedValue="<%# this.SelectedValueDatePart3 %>"
				DataTextField="Text" DataValueField="Value" />
<span id="spDayString" runat="server" style="display: none; padding-right: 5px;">日</span>

<%if(this.HasTime) { %>　
<asp:DropDownList CssClass="<%# this.ID %>" ID="ddlTimeHour" runat="server" DataSource="<%# this.HourList %>" SelectedValue="<%# this.SelectedValueHour %>"
				DataTextField="Text" DataValueField="Value" />
:
<asp:DropDownList CssClass="<%# this.ID %>" ID="ddlTimeMinute" runat="server" DataSource="<%# this.MinuteList %>" SelectedValue="<%# this.SelectedValueMinute %>"
				DataTextField="Text" DataValueField="Value" />
:
<asp:DropDownList CssClass="<%# this.ID %>" ID="ddlTimeSecond" runat="server" DataSource="<%# this.SecondList %>" SelectedValue="<%# this.SelectedValueSecond %>"
				DataTextField="Text" DataValueField="Value" />
<%} %>

<%: this.DateTimeInfoMessage %>

<asp:HiddenField ID="hfDateTimeSelected" runat="server" />
<script type="text/javascript">
	$(document).ready(function () {
		SaveSelectedDate<%= this.ID %>();
	});

	$('.<%= this.ID %>').change(function () {
		SaveSelectedDate<%= this.ID %>();
	});

	// Save value of selected date
	function SaveSelectedDate<%= this.ID %>() {
		var year = $('#<%= ddlDatePart1.ClientID %>').val();
		var month = $('#<%= ddlDatePart2.ClientID %>').val();
		var day = $('#<%= ddlDatePart3.ClientID %>').val();
		<% if (this.HasTime) { %>
		var selectedDate = year + '/' + month + '/' + day + ' ' + $('#<%= ddlTimeHour.ClientID %>').val() + ':' + $('#<%= ddlTimeMinute.ClientID %>').val() + ':' + $('#<%= ddlTimeSecond.ClientID %>').val();
		<% } else { %>
		var selectedDate = year + '/' + month + '/' + day + ' 00:00:00';
		<% } %>
		$('#<%= hfDateTimeSelected.ClientID %>').val(selectedDate);
	}
</script>