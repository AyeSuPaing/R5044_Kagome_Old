<%--
=========================================================================================================
  Module      : ユーザ情報拡張項目編集系の出力コントローラ(BodyUserExtendModify.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="UserExtendUserControl" %>
<% if (this.HasInput){ %>
<!-- ▽INPUT▽ -->
<asp:Repeater ID="rUserExtendInput" ItemType="UserExtendSettingModel" runat="server">
<ItemTemplate>
<tr>
<th>
	<%-- 項目名 --%>
	<%# WebSanitizer.HtmlEncode(Item.SettingName) %>
	<span class="necessary" runat="server" visible="<%# IsNecessary(Item.SettingId)%>">*</span>
</th>
<td>
	<%-- 概要 --%>
	<%# GetUserExtendSettingOutLine(Item.OutlineKbn, Item.Outline) %>
	<%-- TEXT --%>
	<div runat="server" visible="<%# Item.InputType == Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT %>">
		<asp:TextBox runat="server" ID="tbSelect" Width="250px"></asp:TextBox>
	</div>
	<%-- DDL --%>
	<div runat="server" visible="<%# Item.InputType == Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_DROPDOWN %>">
		<asp:DropDownList runat="server" ID="ddlSelect"></asp:DropDownList>
	</div>
	<%-- RADIO --%>
	<div runat="server" visible="<%# Item.InputType == Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_RADIO %>">
		<asp:RadioButtonList runat="server" ID="rblSelect" RepeatDirection="Horizontal" RepeatColumns="3" RepeatLayout="Flow" CssClass="radioBtn"></asp:RadioButtonList>
	</div>
	<%-- CHECK --%>
	<div runat="server" visible="<%# Item.InputType == Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_CHECKBOX %>">
		<asp:CheckBoxList runat="server" ID="cblSelect" RepeatDirection="Horizontal" RepeatColumns="3" RepeatLayout="Flow" CssClass="checkBox"></asp:CheckBoxList>
	</div>
	<%-- 検証文言 --%>
	<asp:Label runat="server" ID="lbErrMessage" CssClass="error_inline"></asp:Label>
	<%-- 隠し値 --%>
	<asp:HiddenField ID="hfSettingId" runat="server" Value="<%# Item.SettingId %>" />
	<asp:HiddenField ID="hfInputType" runat="server" Value="<%# Item.InputType %>" />
</td>
</tr>
</ItemTemplate>
</asp:Repeater>
<!-- △INPUT△ -->
<%} else { %>
<!-- ▽CONFIRM▽ -->
<asp:Repeater ID="rUserExtendDisplay" runat="server">
<ItemTemplate>
<tr>
<th><%# WebSanitizer.HtmlEncode(this.UserExtendSettingList.Items[Container.ItemIndex].SettingName) %></th>
<td><%# WebSanitizer.HtmlEncode(this.UserExtend.UserExtendDataText[(string)Container.DataItem]) %></td>
</tr>
</ItemTemplate>
</asp:Repeater>
<!-- △CONFIRM△ -->
<%} %>
