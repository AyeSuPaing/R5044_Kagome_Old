<%--
=========================================================================================================
  Module      : 日付入力コントロール(DateTimeInput.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="w2.App.Common.Web.WebCustomControl.DateTimeInputControl" %>

<asp:DropDownList ID="ddlDatePart1" runat="server" DataSource="<%# this.DataSourceDatePart1 %>" SelectedValue="<%# this.SelectedValueDatePart1 %>"
				DataTextField="Text" DataValueField="Value" AutoPostBack="<%# this.ControlAutoPostBack %>" OnSelectedIndexChanged="ddlDateTime_SelectedIndexChanged" />
<span id="spSeparator1" runat="server">/</span>
<asp:DropDownList ID="ddlDatePart2" runat="server" DataSource="<%# this.DataSourceDatePart2 %>" SelectedValue="<%# this.SelectedValueDatePart2 %>"
				DataTextField="Text" DataValueField="Value" AutoPostBack="<%# this.ControlAutoPostBack %>" OnSelectedIndexChanged="ddlDateTime_SelectedIndexChanged" />
<span id="spSeparator2" runat="server">/</span>
<asp:DropDownList ID="ddlDatePart3" runat="server" DataSource="<%# this.DataSourceDatePart3 %>" SelectedValue="<%# this.SelectedValueDatePart3 %>"
				DataTextField="Text" DataValueField="Value" AutoPostBack="<%# this.ControlAutoPostBack %>" OnSelectedIndexChanged="ddlDateTime_SelectedIndexChanged" />
<span id="spDayString" runat="server" style="display: none; padding-right: 5px;">日</span>

<%if(this.HasTime) { %>
<asp:DropDownList ID="ddlTimeHour" runat="server" DataSource="<%# this.HourList %>" SelectedValue="<%# this.SelectedValueHour %>"
				DataTextField="Text" DataValueField="Value" AutoPostBack="<%# this.ControlAutoPostBack %>" OnSelectedIndexChanged="ddlDateTime_SelectedIndexChanged" />
:
<asp:DropDownList ID="ddlTimeMinute" runat="server" DataSource="<%# this.MinuteList %>" SelectedValue="<%# this.SelectedValueMinute %>"
				DataTextField="Text" DataValueField="Value" AutoPostBack="<%# this.ControlAutoPostBack %>" OnSelectedIndexChanged="ddlDateTime_SelectedIndexChanged" />
:
<asp:DropDownList ID="ddlTimeSecond" runat="server" DataSource="<%# this.SecondList %>" SelectedValue="<%# this.SelectedValueSecond %>"
				DataTextField="Text" DataValueField="Value" AutoPostBack="<%# this.ControlAutoPostBack %>" OnSelectedIndexChanged="ddlDateTime_SelectedIndexChanged" />
<%} %>
