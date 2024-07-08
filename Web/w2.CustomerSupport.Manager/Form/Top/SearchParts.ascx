<%--
=========================================================================================================
  Module      : 検索パーツ(SearchParts.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Cs.SummarySetting" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SearchParts.ascx.cs" Inherits="Form_Top_SearchForm" %>
<%@ Import Namespace="w2.App.Common.Global.Config" %>

<!-- 表示用スタイル -->
<style type="text/css">
<!--
	.datagrid table tbody td.condition
	{
		padding: 1px 3px;
	}
	
	/* 日曜日の背景を変える */
	.sunday .ui-state-default {    
		background-image: none;
		background-color: #FFF0F5;
	}
	/* 平日の背景を変える */
	.weekday .ui-state-default {
		background-image: none;
		background-color: #FFFFFF;
	}
-->
</style>

<asp:UpdatePanel runat="server">
<ContentTemplate>
<%
	// 検索画面のキーワード検索をEnter押下で実行した場合にデフォルトはトップ画面の検索ボックスでsubmitされてしまうため
	// 検索画面の検索イベントが実行されるようここで制御する。
	tbKeyword.Attributes["onkeypress"] = "if (event.keyCode==13){__doPostBack('" + btnSearch.UniqueID + "',''); return false;}";
%>
<table width="100%"  border="0" cellspacing="0" cellpadding="0">
	<tr valign="top">
		<td width="100%">
			<div class="datagrid larger">
			<table width="100%"  border="1" cellspacing="0" cellpadding="0">
			<thead>
				<tr>
					<th colspan="2">検索条件</th>
				</tr>
			</thead>
			<tbody>
				<tr>
					<td class="alt" width="100">結果表示</td>
					<td>
						<div class="dataresult buttonlist">
							<asp:RadioButtonList ID="rblListMode" RepeatDirection="Horizontal" RepeatLayout="Flow" runat="server" />
						</div>
					</td>
				</tr>
				<tr>
					<td class="alt">検索項目</td>
					<td class="buttonlist">
						<asp:RadioButton ID="rbContentsAndHeader" GroupName="target" Text="本文＋ヘッダ" runat="server" OnCheckedChanged="SelectedItem_Changed" AutoPostBack="true" /><br />
						<asp:RadioButton ID="rbContents" GroupName="target" Text="本文　　　*Body（メール） / 内容＋回答（電話）" runat="server" OnCheckedChanged="SelectedItem_Changed" AutoPostBack="true" /><br />
						<asp:RadioButton ID="rbHeader" GroupName="target" Text="ヘッダ　　　*From/To/Cc/Bcc/Subject（メール） / 問合せ元情報＋件名（電話）" runat="server" OnCheckedChanged="SelectedItem_Changed" AutoPostBack="true" /><br />
						<asp:RadioButton ID="rbIncidentId" GroupName="target" Text="インシデントID" runat="server" OnCheckedChanged="SelectedItem_Changed" AutoPostBack="true" /><br />
						<asp:RadioButton ID="rbMessageItem" GroupName="target" Text="その他メッセージ項目" runat="server" OnCheckedChanged="SelectedItem_Changed" AutoPostBack="true" />
						<asp:CheckBoxList ID="cblTargetMessageItem" RepeatDirection="Horizontal" RepeatLayout="Flow" runat="server" /><br />
						<asp:RadioButton ID="rbIncidentItem" GroupName="target" Text="その他インシデント項目" runat="server" OnCheckedChanged="SelectedItem_Changed" AutoPostBack="true" />
						<asp:CheckBoxList ID="cblTargetIncidentItem" RepeatDirection="Horizontal" RepeatLayout="Flow" runat="server" /><br />
						<!--
						※検索対象項目は次の通り。<br />
						　本文 = Body（メール）、内容/回答（電話）<br />
						　ヘッダ = From/To/Cc/Bcc/Subject（メール）、氏名/電話番号/メールアドレス/件名（電話）
						-->
					</td>
				</tr>
				<tr>
					<td class="alt" width="100">キーワード</td>
					<td valign="middle">
						<asp:TextBox ID="tbKeyword" Width="300" runat="server" style="font-size: 16px;" height="22px" />
						<asp:Button ID="btnSearch" Text=" 検索 " runat="server" OnClick="btnSearch_Click" Height="28px" Width="60px" /><br />
						<span class="dataresult buttonlist">
							<asp:RadioButtonList ID="rblSearchMode" RepeatDirection="Horizontal" RepeatLayout="Flow" runat="server" />
						</span>
						<span data-popover-message="『「すべて含む」「いずれか含む」の場合、空白で区切るとそれぞれの単語に分割して検索します。「完全一致」の場合は空白を含めて一語で検索します。』" class="title-help" style="display:<%= rbIncidentId.Checked ? "none" : "display" %>">
							<span class="icon-help"></span>
						</span>
					</td>
				</tr>
				<tr>
					<td class="alt">絞り込み条件</td>
					<td style="padding: 0px;">
						<div class="dataresult larger">
							<table style="border: 1px solid red;" border="0">
								<tr class="buttonlist">
									<td>期間</td>
									<td><asp:CheckBox ID="cbPeriod" Text="対象期間" runat="server" OnCheckedChanged="SelectedItem_Changed" AutoPostBack="true" /></td>
									<td class="condition" style="border-right: 0px;">
										<asp:TextBox ID="tbPeriodFrom" runat="server" OnTextChanged="SelectedItem_Changed" AutoPostBack="true"></asp:TextBox>
										～
										<asp:TextBox ID="tbPeriodTo" runat="server" OnTextChanged="SelectedItem_Changed" AutoPostBack="true"></asp:TextBox>
										<br />
										<asp:Label ID="label" runat="server" Visible="false" CssClass="notice"></asp:Label>
										<asp:RadioButtonList ID="rblPeriodType" RepeatDirection="Vertical" RepeatLayout="Flow" runat="server">
											<asp:ListItem Text="問合せ・回答日（メッセージ）" Value="InquiryDate"></asp:ListItem>
											<asp:ListItem Text="最新の問合せ・回答日（メッセージ）" Value="LatestInquiryDate"></asp:ListItem>
											<asp:ListItem Text="最終更新日（メッセージ）" Value="MessageDateChanged"></asp:ListItem>
											<asp:ListItem Text="発生日（インシデント）" Value="IncidentDateCreated"></asp:ListItem>
											<asp:ListItem Text="対応完了日（インシデント）" Value="IncidentDateCompleted"></asp:ListItem>
										</asp:RadioButtonList>
									</td>
								</tr>
								<tr class="buttonlist">
									<td rowspan="2">メッセージ</td>
									<td><asp:CheckBox ID="cbMessageType" Text="メッセージタイプ：" runat="server" OnCheckedChanged="SelectedItem_Changed" AutoPostBack="true" /></td>
									<td class="condition"><asp:CheckBoxList ID="cblMessageType" RepeatDirection="Horizontal" RepeatLayout="Flow" runat="server" /></td>
								</tr>
								<tr class="buttonlist">
									<td><asp:CheckBox ID="cbMessageStatus" Text="メッセージステータス：" runat="server" OnCheckedChanged="SelectedItem_Changed" AutoPostBack="true" /></td>
									<td class="condition"><asp:DropDownList ID="ddlMessageStatus" runat="server" /></td>
								</tr>
								<tr class="buttonlist">
									<td rowspan="5">インシデント</td>
									<td><asp:CheckBox ID="cbStatus" Text="ステータス：" runat="server" OnCheckedChanged="SelectedItem_Changed" AutoPostBack="true" /></td>
									<td class="condition"><asp:DropDownList ID="ddlStatus" runat="server" /></td>
								</tr>
								<tr class="buttonlist">
									<td><asp:CheckBox ID="cbCategory" Text="カテゴリ：" runat="server" OnCheckedChanged="SelectedItem_Changed" AutoPostBack="true" /></td>
									<td class="condition" style="overflow: visible"><asp:DropDownList CssClass="select2-select" ID="ddlCategory" runat="server" /></td>
								</tr>
								<tr class="buttonlist">
									<td><asp:CheckBox ID="cbImportance" Text="重要度：" runat="server" OnCheckedChanged="SelectedItem_Changed" AutoPostBack="true" /></td>
									<td class="condition"><asp:DropDownList ID="ddlImportance" runat="server" /></td>
								</tr>
								<tr class="buttonlist">
									<td><asp:CheckBox ID="cbVoc" Text="VOC：" runat="server" OnCheckedChanged="SelectedItem_Changed" AutoPostBack="true" /></td>
									<td class="condition" style="overflow: visible"><asp:DropDownList CssClass="select2-select" ID="ddlVoc" runat="server" /></td>
								</tr>
								<tr class="buttonlist">
									<td><asp:CheckBox ID="cbAssign" Text="担当：" runat="server" OnCheckedChanged="SelectedItem_Changed" AutoPostBack="true" /></td>
									<td class="condition" style="overflow: visible">グループ　：<asp:DropDownList ID="ddlGroup" CssClass="select2-select" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCsGroup_SelectedIndexChanged" Width="70%" /><br />
										オペレータ：<asp:DropDownList ID="ddlOperator" CssClass="select2-select" runat="server" Width="70%" /></td>
								</tr>
								<tr class="buttonlist" id="trSummarySetting" runat="server">
									<td id="tdSummarySettingHead" style="border-bottom-style: none;" runat="server"><br />インシデント集計区分<br /><asp:CheckBox ID="cbDisplayInvalid" Text="無効も表示" runat="server" OnCheckedChanged="SelectedItem_Changed" AutoPostBack="true" /><br /><br /></td>
									<td id="tdSummarySettingData" style="border-bottom-style: none;" colspan="2" runat="server">有効な集計区分はありません。</td>
								</tr>
								<!-- 集計区分 ココカラ  -->
								<asp:Repeater ID="rIncidentSummary" runat="server">
								<ItemTemplate>
								<tr class="buttonlist" id="trSummarySettingRow" runat="server">
									<td style='<%# Container.ItemIndex == ((CsSummarySettingModel[])rIncidentSummary.DataSource).Length-1 ? "border-bottom-style: none;" : "" %>'>
										<asp:HiddenField ID="hfValidFlg" Value="<%# ((CsSummarySettingModel)Container.DataItem).ValidFlg %>" runat="server" />
										<asp:CheckBox ID="cbSummarySetting" Text="<%# WebSanitizer.HtmlEncode(((CsSummarySettingModel)Container.DataItem).SummarySettingTitle) %>" runat="server" OnCheckedChanged="SelectedItem_Changed" AutoPostBack="true" />
										<asp:Label ID="lbSummarySetting" Text="(無効)" Visible="<%# ((CsSummarySettingModel)Container.DataItem).ValidFlg == Constants.FLG_CSSUMMARYSETTING_VALID_FLG_INVALID %>" runat="server"></asp:Label>
									</td>
									<td class="condition" style='<%# Container.ItemIndex == ((CsSummarySettingModel[])rIncidentSummary.DataSource).Length-1 ? "border-bottom-style: none;" : "" %>;overflow: visible'>
										<asp:HiddenField ID="hfSummaryNo" Value="<%# ((CsSummarySettingModel)Container.DataItem).SummarySettingNo %>" runat="server" />
										<asp:HiddenField ID="hfSummarySettingType" Value="<%# ((CsSummarySettingModel)Container.DataItem).SummarySettingType %>" runat="server" />
										<asp:RadioButtonList ID="rblSummaryValue" RepeatLayout="Flow" RepeatDirection="Horizontal" Visible="<%# ((CsSummarySettingModel)Container.DataItem).SummarySettingType == Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_RADIO %>" DataSource="<%# ((CsSummarySettingModel)Container.DataItem).EX_ListItems %>" DataTextField="Text" DataValueField="Value" runat="server"></asp:RadioButtonList>
										<asp:DropDownList ID="ddlSummaryValue" CssClass="select2-select" Visible="<%# ((CsSummarySettingModel)Container.DataItem).SummarySettingType == Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_DROPDOWN %>" DataSource="<%# ((CsSummarySettingModel)Container.DataItem).EX_ListItemsWithEmptyItem %>" DataTextField="Text" DataValueField="Value" Width="80%" runat="server"></asp:DropDownList>
										<asp:TextBox ID="tbSummaryValue" Width="200" MaxLength="50" Visible="<%# ((CsSummarySettingModel)Container.DataItem).SummarySettingType == Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_TEXT %>" runat="server"></asp:TextBox>
									</td>
								</tr>
								</ItemTemplate>
								</asp:Repeater>
								<!-- 集計区分 ココマデ -->
							</table>
						</div>
					</td>
				</tr>
				<tr>
					<td class="alt">その他の条件</td>
					<td class="buttonlist">
						<asp:CheckBox ID="cbIncludeTrash" Text="ゴミ箱の中も検索" runat="server" />
					</td>
				</tr>
			</tbody>
			</table>
			</div>
		</td>
		<td><img alt="" src="../../Images/Common/sp.gif" border="0" height="10" width="10" /></td>
	</tr>
</table>
<img alt="" src="../../Images/Common/sp.gif" border="0" height="5" width="0" /><br />
<asp:Button ID="btnSearchBottom" Text=" 検索 " runat="server" OnClick="btnSearch_Click" Height="28px" Width="60px" /><br />
</ContentTemplate>
</asp:UpdatePanel>

<script type="text/javascript">
	var dateFormat = '<%= GlobalConfigUtil.GetDateTimeFormatText(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE, DateTimeUtility.FormatType.YearMonthDay2LetterNoneServerTimeForDatePicker).ToLower() %>';
	var monthNames = ["1月", "2月", "3月", "4月", "5月", "6月", "7月", "8月", "9月", "10月", "11月", "12月"];
	var dayNames = ["日", "月", "火", "水", "木", "金", "土"];

	var handler = function (date) {
		if (date.getDay() == 0) {return [true, "sunday"];}
		if (date.getDay() != 6) {return [true, "weekday"];}
		return [true];
	}

	var userAgent = window.navigator.userAgent.toLowerCase();
	var appVersion = window.navigator.appVersion.toLowerCase();

	// IE7orそれ以前では、DatePickerを表示しない（ページエラー等の不都合があるので）
	if ((userAgent.indexOf("msie") == -1) || (appVersion.indexOf("msie 6.") == -1 && appVersion.indexOf("msie 7.") == -1)) {
		Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function (evt, args) {
			$("#<%= this.tbPeriodFrom.ClientID %>").datepicker({
				dateFormat: dateFormat, monthNames: monthNames, dayNamesMin: dayNames, showMonthAfterYear: true, yearSuffix: "年", beforeShowDay: handler
			});
		});
		Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function (evt, args) {
			$("#<%= this.tbPeriodTo.ClientID %>").datepicker({
				dateFormat: dateFormat, monthNames: monthNames, dayNamesMin: dayNames, showMonthAfterYear: true, yearSuffix: "年", beforeShowDay: handler
			});
		});
	
		$("#<%= this.tbPeriodFrom.ClientID %>").datepicker({
			dateFormat: dateFormat, monthNames: monthNames, dayNamesMin: dayNames, showMonthAfterYear: true, yearSuffix: "年", beforeShowDay: handler
		});
		$("#<%= this.tbPeriodTo.ClientID %>").datepicker({
			dateFormat: dateFormat, monthNames: monthNames, dayNamesMin: dayNames, showMonthAfterYear: true, yearSuffix: "年", beforeShowDay: handler
		});
	}
</script>
