<%--
=========================================================================================================
  Module      : ユーザ情報拡張項目編集系の出力コントローラ(BodyUserExtendModify.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="UserExtendUserControl" %>
<% if (this.HasInput){ %>
<%-- ▽ユーザ情報拡張登録▽ --%>
<asp:Repeater ID="rUserExtendInput" ItemType="UserExtendSettingModel" runat="server">
<HeaderTemplate>
<ul>
</HeaderTemplate>
<ItemTemplate>
	<li class="title">
		<%-- 項目名 --%>
		<%# WebSanitizer.HtmlEncode(Item.SettingName) %>
		<span class="necessary" runat="server" visible="<%# IsNecessary(Item.SettingId)%>">*</span>
	</li>
	<li>
		<%-- TEXT --%>
		<div runat="server" visible="<%# Item.InputType == Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT %>">
			<w2c:ExtendedTextBox Runat="server" id="tbSelect" CssClass="input_widthN"></w2c:ExtendedTextBox>
		</div>
		<%-- DDL --%>
		<div runat="server" visible="<%# Item.InputType == Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_DROPDOWN %>">
			<asp:DropDownList runat="server" ID="ddlSelect"></asp:DropDownList>
		</div>
		<%-- RADIO --%>
		<div runat="server" visible="<%# Item.InputType == Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_RADIO %>">
			<asp:RadioButtonList runat="server" ID="rblSelect" RepeatDirection="Horizontal" RepeatColumns="1" RepeatLayout="Flow" CssClass="radioBtn"></asp:RadioButtonList>
		</div>
		<%-- CHECK --%>
		<div runat="server" visible="<%# Item.InputType == Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_CHECKBOX %>">
			<asp:CheckBoxList runat="server" ID="cblSelect" RepeatDirection="Horizontal" RepeatColumns="1" RepeatLayout="Flow" CssClass="checkBox"></asp:CheckBoxList>
		</div>
		<%-- 検証文言 --%>
		<asp:Label runat="server" ID="lbErrMessage" CssClass="error_inline"></asp:Label>
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
<%-- △ユーザ情報拡張登録△ --%>
<%} else { %>
<!-- ▽ユーザ情報拡張確認▽ -->
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
<!-- △ユーザ情報拡張確認△ -->
<%} %>