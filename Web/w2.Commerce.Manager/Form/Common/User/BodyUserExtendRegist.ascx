<%--
=========================================================================================================
  Module      : ユーザ情報拡張項目編集系の出力コントローラ(BodyUserExtendRegist.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BodyUserExtendRegist.ascx.cs" Inherits="Form_Common_User_BodyUserExtendRegist" %>
<asp:Repeater ID="rUserExtendInput" ItemType="w2.Domain.UserExtendSetting.UserExtendSettingModel" runat="server">
<ItemTemplate>
	<tr>
<td align="left" class="edit_title_bg" width="30%">
	<%-- 項目名 --%>
	<%#  WebSanitizer.HtmlEncode(Item.SettingName) %>
	<span class="notice" runat="server" visible="<%# IsNecessary(Item.SettingId)%>">*</span>
</td>
<td align="left" class="edit_item_bg">
	<%-- TEXT --%>
	<div runat="server" visible="<%# (Item.InputType == Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT) %>">
		<asp:TextBox runat="server" ID="tbSelect" Width="250px"></asp:TextBox>
	</div>
	<%-- DDL --%>
	<div runat="server" visible="<%# (Item.InputType == Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_DROPDOWN) %>">
		<asp:DropDownList runat="server" ID="ddlSelect"></asp:DropDownList>
	</div>
	<%-- RADIO --%>
	<div runat="server" visible="<%# (Item.InputType == Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_RADIO) %>">
		<asp:RadioButtonList runat="server" ID="rblSelect" RepeatDirection="Horizontal" RepeatColumns="3" RepeatLayout="Flow" CssClass="radioBtn"></asp:RadioButtonList>
	</div>
	<%-- CHECK --%>
	<div runat="server" visible="<%# (Item.InputType == Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_CHECKBOX) %>">
		<asp:CheckBoxList runat="server" ID="cblSelect" RepeatDirection="Horizontal" RepeatColumns="3" RepeatLayout="Flow" CssClass="checkBox"></asp:CheckBoxList>
	</div>
	<%-- 隠し値 --%>
	<asp:HiddenField ID="hfSettingId" runat="server" Value="<%# Item.SettingId %>" />
	<asp:HiddenField ID="hfInputType" runat="server" Value="<%# Item.InputType %>" />
</td>
</tr>
</ItemTemplate>
</asp:Repeater>