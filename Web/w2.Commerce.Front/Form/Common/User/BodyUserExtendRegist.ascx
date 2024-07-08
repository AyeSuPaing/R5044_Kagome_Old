<%--
=========================================================================================================
  Module      : ユーザ情報拡張項目登録系の出力コントローラ(BodyUserExtendRegist.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="UserExtendUserControl" %>
<% if (this.HasInput){ %>
<!-- ▽INPUT▽ -->
<asp:Repeater ID="rUserExtendInput" ItemType="w2.Domain.UserExtendSetting.UserExtendSettingModel" runat="server">
<ItemTemplate>
<tr>
<th>
	<%-- 項目名 --%>
	<%#: Item.SettingName %>
	<span class="necessary" visible="<%# IsNecessary(Item.SettingId)%>" runat="server">*</span>
</th>
<td>
	<%-- 概要 --%>
	<%# GetUserExtendSettingOutLine(Item.OutlineKbn, Item.Outline) %>
	<%-- TEXT --%>
	<div visible="<%# Item.InputType == Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT %>" runat="server">
		<asp:TextBox ID="tbSelect" Width="250px" runat="server" />
	</div>
	<%-- DDL --%>
	<div visible="<%# Item.InputType == Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_DROPDOWN %>" runat="server">
		<asp:DropDownList ID="ddlSelect" runat="server" />
	</div>
	<%-- RADIO --%>
	<div visible="<%# Item.InputType == Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_RADIO %>" runat="server">
		<asp:RadioButtonList ID="rblSelect" RepeatDirection="Horizontal" RepeatColumns="3" RepeatLayout="Flow" CssClass="radioBtn" runat="server" />
	</div>
	<%-- CHECK --%>
	<div visible="<%# Item.InputType == Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_CHECKBOX %>" runat="server">
		<asp:CheckBoxList ID="cblSelect" RepeatDirection="Horizontal" RepeatColumns="3" RepeatLayout="Flow" CssClass="checkBox" runat="server" />
	</div>
	<%-- 検証文言 --%>
	<asp:Label ID="lbErrMessage" CssClass="error_inline" runat="server" />
	<%-- 隠し値 --%>
	<asp:HiddenField ID="hfSettingId" Value="<%# Item.SettingId %>" runat="server" />
	<asp:HiddenField ID="hfInputType" Value="<%# Item.InputType %>" runat="server" />
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
<th><%#: this.UserExtendSettingList.Items[Container.ItemIndex].SettingName %></th>
<td><%#: this.UserExtend.UserExtendDataText[(string)Container.DataItem] %></td>
</tr>
</ItemTemplate>
</asp:Repeater>
<!-- △CONFIRM△ -->
<%} %>
