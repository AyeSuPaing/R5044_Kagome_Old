<%--
=========================================================================================================
  Module      : ユーザ情報拡張項目登録系の出力コントローラ(BodyUserExtendRegist.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="UserExtendUserControl" %>
<% if (this.HasInput){ %>
<asp:Repeater ID="rUserExtendInput" ItemType="UserExtendSettingModel" runat="server">
<HeaderTemplate>
<ul>
</HeaderTemplate>
<ItemTemplate>
	<li class="title">
		<%-- 項目名 --%>
		<%#WebSanitizer.HtmlEncode(Item.SettingName) %>
		<span class="necessary" runat="server" visible="<%# IsNecessary(Item.SettingId)%>">*</span>
	</li>
	<li>
		<%-- TEXT --%>
		<div runat="server" visible="<%# Item.InputType == Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT %>">
			<w2c:ExtendedTextBox Runat="server" id="tbSelect"></w2c:ExtendedTextBox>
		</div>
		<%-- DDL --%>
		<div runat="server" visible="<%# Item.InputType == Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_DROPDOWN %>">
			<asp:DropDownList runat="server" ID="ddlSelect"></asp:DropDownList>
		</div>
		<%-- RADIO --%>
		<div runat="server" visible="<%# Item.InputType == Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_RADIO %>">
			<asp:RadioButtonList runat="server" ID="rblSelect" RepeatDirection="Horizontal" RepeatColumns="1" RepeatLayout="Flow"></asp:RadioButtonList>
		</div>
		<%-- CHECK --%>
		<div runat="server" visible="<%# Item.InputType == Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_CHECKBOX %>">
			<asp:CheckBoxList runat="server" ID="cblSelect" RepeatDirection="Horizontal" RepeatColumns="1" RepeatLayout="Flow"></asp:CheckBoxList>
		</div>
		<%-- 検証文言 --%>
		<asp:Label runat="server" ID="lbErrMessage"></asp:Label>
		<%-- 概要 --%>
		<%# GetUserExtendSettingOutLine(Item.OutlineKbn, Item.Outline) %>
		<%-- 隠し値 --%>
		<asp:HiddenField ID="hfSettingId" runat="server" Value="<%# Item.SettingId %>" />
		<asp:HiddenField ID="hfInputType" runat="server" Value="<%# Item.InputType %>" />
	</li>
</ItemTemplate>
<FooterTemplate>
</ul>
</FooterTemplate>
</asp:Repeater>
<%} else { %>
<asp:Repeater ID="rUserExtendDisplay" runat="server">
<HeaderTemplate>
<ul>
</HeaderTemplate>
<ItemTemplate>
	<li class="title">
		<%# WebSanitizer.HtmlEncode(this.UserExtendSettingList.Items[Container.ItemIndex].SettingName) %>
	</li>
	<li>
		<%# WebSanitizer.HtmlEncode(this.UserExtend.UserExtendDataText[(string)Container.DataItem]) %>
	</li>
</ItemTemplate>
<FooterTemplate>
</ul>
</FooterTemplate>
</asp:Repeater>
<%} %>