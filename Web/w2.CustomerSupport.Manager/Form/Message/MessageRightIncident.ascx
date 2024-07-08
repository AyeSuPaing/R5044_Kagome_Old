<%--
=========================================================================================================
  Module      : メッセージページインシデントフォーム出力コントローラ(MessageRightIncident.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Cs.SummarySetting" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MessageRightIncident.ascx.cs" Inherits="Form_Message_MessageRightIncident" %>

<a name="a_incident_title" id="a_incident_title"></a>
<div class="dataresult larger" id="hidearea1">
	<asp:UpdatePanel ID="up1" runat="server">
	<ContentTemplate>

	<table>
	<thead>
	<tr>
		<a id="aIncidentTitle" href="#" runat="server"></a>
		<th colspan="5">インシデント</th>
	</tr>
	</thead>
	</table>

		<asp:Label ID="lErrorMessages" CssClass="notice" runat="server"></asp:Label>

	<table>
	<tbody>
		<tr>
			<td width="15%" class="alt">インシデントID</td>
			<td width="85%" colspan="3">
				<asp:Literal ID="lIncidentIdCreateMessage" Text="新規作成されます" runat="server"></asp:Literal>
				<asp:Literal ID="lIncidentId" Text="" Visible="false" runat="server"></asp:Literal>
				<img id="imgLock" visible="false" src="../../Images/Cs/icon_lock.png" alt="ロック" width="14" height="14" runat="server" />
				<asp:Button id="btnClearIncident" Text="クリア" runat="server" OnClick="btnClearIncident_Click" />
			</td>
		</tr>
		<tr>
			<td class="alt">ユーザーID</td>
			<td colspan="3"><asp:TextBox ID="tbIncidentUserId" Width="20%" runat="server"></asp:TextBox></td>
		</tr>
		<tr>
			<td class="alt">タイトル  <span class="notice">*</span></td>
			<td colspan="3">
				<asp:TextBox ID="tbIncidentTitle" Width="60%" runat="server"></asp:TextBox>
				<input id="iptRefrectTitleFromMessage" type="button" value=" メッセージから差込 " onclick="Javascript:reflect_incident()" />
			</td>
		</tr>
		<tr>
			<td class="alt">カテゴリ</td>
			<td colspan="3" style="overflow: visible">
				<asp:DropDownList ID="ddlIncidentCategory" CssClass="select2-select" Width="80%" runat="server"></asp:DropDownList>
			</td>
		</tr>
		<tr>
			<td class="alt">ステータス  <span class="notice">*</span></td>
			<td>
				<asp:DropDownList ID="ddlIncidentStatus" Width="60%" runat="server"></asp:DropDownList>
			</td>
			<td class="alt" width="20%">重要度  <span class="notice">*</span></td>
			<td width="30%">
				<asp:DropDownList ID="ddlImportance" Width="60%" runat="server"></asp:DropDownList>
			</td>
		</tr>
		<tr>
			<td class="alt" width="20%">VOC</td>
			<td colspan="3" width="30%">
				<asp:DropDownList ID="ddlVoc" CssClass="select2-select" Width="25%" runat="server"></asp:DropDownList><br />
				<asp:TextBox id="tbVocMemo" Width="400" runat="server"></asp:TextBox>
			</td>
		</tr>
		<tr>
			<td rowspan="2" class="alt">担当</td>
			<td colspan="3" style="overflow: visible">
				<span style="width:100px;display:inline-block">グループ：</span><!--
					--><asp:DropDownList ID="ddlCsGroups" CssClass ="select2-select" AutoPostBack="true" Width="40%" runat="server" OnSelectedIndexChanged="ddlCsGroup_SelectedIndexChanged"></asp:DropDownList>
			</td>
		</tr>
		<tr>
			<td colspan="3" style="overflow: visible">
				<span style="width:100px;display:inline-block">オペレータ：</span><!--
				--><asp:DropDownList ID="ddlCsOperators" CssClass ="select2-select" Width="40%" runat="server"></asp:DropDownList>
				<asp:HiddenField ID="hfCsOperatorBefore" runat="server" />
				<asp:Button ID="btnSetOperatorAndGroup" Text="  担当を自分にセット  " runat="server" OnClick="btnSetOperatorAndGroup_Click" />
			</td>
		</tr>
		<tr>
			<td class="alt">内部メモ</td>
			<td colspan="3"><asp:TextBox ID="tbIncidentComment" Rows="6" Width="90%" TextMode="MultiLine" runat="server" CssClass="larger"></asp:TextBox></td>
		</tr>
		<asp:Repeater ID="rIncidentSummary" runat="server">
		<ItemTemplate>
			<tr>
				<td class="alt"><%# WebSanitizer.HtmlEncode(((CsSummarySettingModel)Container.DataItem).SummarySettingTitle) %></td>
				<td colspan="4" style="overflow: visible">
					<asp:HiddenField ID="hfSummaryNo" Value="<%# ((CsSummarySettingModel)Container.DataItem).SummarySettingNo %>" runat="server" />
					<asp:HiddenField ID="hfSummarySettingTitle" Value="<%# ((CsSummarySettingModel)Container.DataItem).SummarySettingTitle %>" runat="server" />
					<asp:RadioButtonList ID="rblSummaryValue" CssClass="radio_button_list" RepeatLayout="Flow" RepeatDirection="Vertical" visible="<%# ((CsSummarySettingModel)Container.DataItem).SummarySettingType == Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_RADIO %>" DataSource="<%# ((CsSummarySettingModel)Container.DataItem).EX_ListItems.Select(p => new ListItem(WebSanitizer.HtmlEncode(p.Text), p.Value)) %>" DataTextField="Text" DataValueField="Value" runat="server"></asp:RadioButtonList>
					<asp:DropDownList ID="ddlSummaryValue" CssClass ="select2-select" visible="<%# ((CsSummarySettingModel)Container.DataItem).SummarySettingType == Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_DROPDOWN %>" DataSource="<%# ((CsSummarySettingModel)Container.DataItem).EX_ListItemsWithEmptyItem %>" DataTextField="Text" DataValueField="Value" Width="70%" runat="server"></asp:DropDownList>
					<asp:TextBox ID="tbSummaryValue" Width="400" MaxLength="50" visible="<%# ((CsSummarySettingModel)Container.DataItem).SummarySettingType == Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_TEXT %>" runat="server"></asp:TextBox>
				</td>
			</tr>
		</ItemTemplate>
		</asp:Repeater>
		<tr>
			<td class="alt">最終更新者</td>
			<td colspan="3"><asp:Literal ID="lIncidentLastChanged" runat="server"></asp:Literal></td>
		</tr>
		</tbody>
	</table>
	<a name="a_requestinfo_title" id="a_requestinfo_title"></a>

	<script type="text/javascript">
		// メッセージのタイトル差し込み
		function reflect_incident()
		{
			$('#<%= tbIncidentTitle.ClientID %>').val($('#spTilteArea input:text').val());
		}
	</script>

	</ContentTemplate>
	</asp:UpdatePanel>

</div>
