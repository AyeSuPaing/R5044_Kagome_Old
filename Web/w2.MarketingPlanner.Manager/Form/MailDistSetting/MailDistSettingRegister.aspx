<%--
=========================================================================================================
  Module      : メール配信設定登録ページ(MailDistSettingRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ MasterType VirtualPath="~/Form/Common/DefaultPage.master" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MailDistSettingRegister.aspx.cs" Inherits="Form_MailDistSetting_MailDistSettingRegister" %>
<%@ Register TagPrefix="uc" TagName="DateTimeInput" Src="~/Form/Common/DateTimeInput.ascx" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<%
	rbExecBySchedule.Attributes.Add("OnClick", "RefleshTargetListSchedule();");
	//rbExecByAction.Attributes.Add("OnClick", "RefleshTargetListSchedule();");
	rbExecByManual.Attributes.Add("OnClick", "RefleshTargetListSchedule();");

rbScheRepeatDay.Attributes.Add("OnClick", "RefleshTargetListSchedule();");
rbScheRepeatWeek.Attributes.Add("OnClick", "RefleshTargetListSchedule();");
rbScheRepeatMonth.Attributes.Add("OnClick", "RefleshTargetListSchedule();");
rbScheRepeatOnce.Attributes.Add("OnClick", "RefleshTargetListSchedule();");
%>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr>
		<td><h1 class="page-title">メール配信設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ タイトル △-->
	<!--▽ 編集入力 ▽-->
	<tr>
		<% if((m_strActionStatus == Constants.ACTION_STATUS_INSERT) || (m_strActionStatus == Constants.ACTION_STATUS_COPY_INSERT)) {%>
		<td><h2 class="cmn-hed-h2">メール配信設定登録</h2></td>
		<%} %>
		<% if(m_strActionStatus == Constants.ACTION_STATUS_UPDATE) {%>
		<td><h2 class="cmn-hed-h2">メール配信設定編集</h2></td>
		<%} %>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td>
						<table class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td>
												<div class="action_part_top"><input onclick="Javascript:history.back()" type="button" value="  戻る  " />
												<asp:Button id="btnConfirm" runat="server" Text="  確認する  " onclick="btnConfirm_Click"></asp:Button>
												</div>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr id="trId" runat="server" visible="false">
														<td class="edit_title_bg" align="left" width="30%">メール配信設定ID</td>
														<td class="edit_item_bg" align="left">
															<asp:Label ID="lbMailDistId" runat="server"></asp:Label>
															<asp:HiddenField ID="hdnMailDistId" runat="server" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">メール配信設定名<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox ID="tbMailDistName" Width="300" MaxLength="30" runat="server"></asp:TextBox>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">メール文章<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:DropDownList ID="ddlMailTextId" runat="server"></asp:DropDownList>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">ターゲット<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:DropDownList id="ddlTargetId" runat="server"><asp:ListItem></asp:ListItem></asp:DropDownList>
																<asp:CheckBox id="cbTargetExtract" Text="配信時抽出" runat="server" /><br />
															<asp:DropDownList id="ddlTargetId2" runat="server"><asp:ListItem></asp:ListItem></asp:DropDownList>
																<asp:CheckBox id="cbTargetExtract2" Text="配信時抽出" runat="server" /><br />
															<asp:DropDownList id="ddlTargetId3" runat="server"><asp:ListItem></asp:ListItem></asp:DropDownList>
																<asp:CheckBox id="cbTargetExtract3" Text="配信時抽出" runat="server" /><br />
															<asp:DropDownList id="ddlTargetId4" runat="server"><asp:ListItem></asp:ListItem></asp:DropDownList>
																<asp:CheckBox id="cbTargetExtract4" Text="配信時抽出" runat="server" /><br />
															<asp:DropDownList id="ddlTargetId5" runat="server"><asp:ListItem></asp:ListItem></asp:DropDownList>
																<asp:CheckBox id="cbTargetExtract5" Text="配信時抽出" runat="server" /><br />
															※ () 内の数はターゲット作成時のデータ数です。
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">メール配信エラー除外設定<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:DropDownList ID="ddlExceptErrorPoint" runat="server"></asp:DropDownList>pt 以上は除外
														</td>
													</tr>
													<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">モバイルメール除外設定</td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBox ID="cbExceptMobileMail" Text="有効" runat="server" />
														</td>
													</tr>
													<% } %>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">重複配信除外設定</td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBox ID="cbEnableDeduplication" Text="有効" runat="server"/>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">排除リスト（カンマ区切り）</td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox ID="tbExceptList" Width="300" TextMode="MultiLine" Rows="3" runat="server"></asp:TextBox>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg">
															実行タイミング<span class="notice">*</span></td>
														<td class="edit_item_bg">
															<asp:RadioButton id="rbExecBySchedule" GroupName="ExecType" runat="server" />
															<%--asp:RadioButton id="rbExecByAction" GroupName="ExecType" runat="server" /--%>
															<asp:RadioButton id="rbExecByManual" GroupName="ExecType" runat="server" /><br />
															<div id="dvScheduleDetail">
																<div style="padding-left:20px;padding-bottom:10px">
																	<asp:RadioButton ID="rbScheRepeatDay" GroupName="ScheType" runat="server" />
																	<asp:RadioButton ID="rbScheRepeatWeek" GroupName="ScheType" runat="server" />
																	<asp:RadioButton ID="rbScheRepeatMonth" GroupName="ScheType" runat="server" />
																	<asp:RadioButton ID="rbScheRepeatOnce" GroupName="ScheType" runat="server" />
																</div>
																<div style="padding-left:20px">
																	<span id="spScheTopString" style="padding:10px; float:left"></span>
																	<span id="spScheDayOfWeek" style="padding-right:8px; padding-top:3px; float:left">
																		<asp:RadioButtonList ID="rblScheDayOfWeek" RepeatLayout="Flow" RepeatDirection="Horizontal" runat="server"></asp:RadioButtonList>
																	</span>
																	<span id="sucScheDateTime" style="display:block; float:left">
																		<uc:DateTimeInput ID="ucScheDateTime" runat="server" YearList="<%# DateTimeUtility.GetBackwardYearListItem() %>" HasTime="True" HasBlankSign="False" HasBlankValue="False" ControlAutoPostBack="False" />
																	</span>
																	<span id="sucScheDateTimeOnce" style="float:left">
																		<uc:DateTimePickerPeriodInput ID="ucScheDateTimeOnce" runat="server" CanShowEndDatePicker="False" IsNullEndDateTime="True" />
																	</span>
																</div>
															</div>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">有効フラグ<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBox ID="cbValidFlg" runat="server" Text="有効" />
														</td>
													</tr>
												</table>
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
												</table>
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="info_item_bg">
														<td align="left">備考<br />
															メール配信エラー除外設定を使うことで、メール配信エラーの多い宛先にはメールを送らないよう設定できます。<br />
															エラーポイントは、宛先メールアドレスが存在しないなどの理由で配信失敗して加算された、これまでのエラーポイントの累積を表します。<br />
															エラーの原因によって、エラー一回あたりに加算されるポイントは異なります。<br />
															例：存在しないメールアドレスである:5pt。宛先サーバが停止している：1pt。<br ./>
															<br />
															■ スケジュール実行について<br />
															月単位 ・・・指定した日付で毎月実行されます。 対象月に指定した日付が存在しない場合は対象月での実行がスキップされます。<br />
														</td>
													</tr>
												</table>
												<div class="action_part_bottom"><input onclick="Javascript:history.back()" type="button" value="  戻る  " />
													<asp:Button id="btnConfirm2" runat="server" Text="  確認する  " onclick="btnConfirm_Click"></asp:Button></div>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<!--△ 詳細 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<script type="text/javascript">
<!--
function RefleshTargetListSchedule()
{
	// 詳細スケジュール設定？
	if (document.getElementById('<%= rbExecBySchedule.ClientID %>').checked)
	{
		document.getElementById('dvScheduleDetail').style.display = 'inline';
		
		// 日単位
		if (document.getElementById('<%= rbScheRepeatDay.ClientID %>').checked)
		{
			document.getElementById('spScheTopString').innerText = '毎日';
			document.getElementById('spScheDayOfWeek').style.display = 'none';

			document.getElementById('<%= ucScheDateTime.DdlYear.ClientID %>').style.display = 'none';
			document.getElementById('<%= ucScheDateTime.DdlMonth.ClientID %>').style.display = 'none';
			document.getElementById('<%= ucScheDateTime.DdlDay.ClientID %>').style.display = 'none';
			document.getElementById('<%= ucScheDateTime.MonthYearSeparator.ClientID %>').style.display = 'none';
			document.getElementById('<%= ucScheDateTime.DaySeparator.ClientID %>').style.display = 'none';
			document.getElementById('<%= ucScheDateTime.DayString.ClientID %>').style.display = 'none';

			document.getElementById("sucScheDateTimeOnce").style.display = "none";
			document.getElementById("sucScheDateTime").style.display = "block";
		}
		// 週単位
		else if (document.getElementById('<%= rbScheRepeatWeek.ClientID %>').checked)
		{
			document.getElementById('spScheTopString').innerText = '毎週';
			document.getElementById('spScheDayOfWeek').style.display = 'inline';

			document.getElementById('<%= ucScheDateTime.DdlYear.ClientID %>').style.display = 'none';
			document.getElementById('<%= ucScheDateTime.DdlMonth.ClientID %>').style.display = 'none';
			document.getElementById('<%= ucScheDateTime.DdlDay.ClientID %>').style.display = 'none';
			document.getElementById('<%= ucScheDateTime.MonthYearSeparator.ClientID %>').style.display = 'none';
			document.getElementById('<%= ucScheDateTime.DaySeparator.ClientID %>').style.display = 'none';
			document.getElementById('<%= ucScheDateTime.DayString.ClientID %>').style.display = 'none';

			document.getElementById("sucScheDateTimeOnce").style.display = "none";
			document.getElementById("sucScheDateTime").style.display = "block";
		}
		// 月単位
		else if (document.getElementById('<%= rbScheRepeatMonth.ClientID %>').checked)
		{
			document.getElementById('spScheTopString').innerText = '毎月';
			document.getElementById('spScheDayOfWeek').style.display = 'none';

			document.getElementById('<%= ucScheDateTime.DdlYear.ClientID %>').style.display = 'none';
			document.getElementById('<%= ucScheDateTime.DdlMonth.ClientID %>').style.display = 'none';
			document.getElementById('<%= ucScheDateTime.DdlDay.ClientID %>').style.display = 'inline';
			document.getElementById('<%= ucScheDateTime.MonthYearSeparator.ClientID %>').style.display = 'none';
			document.getElementById('<%= ucScheDateTime.DaySeparator.ClientID %>').style.display = 'none';
			document.getElementById('<%= ucScheDateTime.DayString.ClientID %>').style.display = 'inline';

			document.getElementById("sucScheDateTimeOnce").style.display = "none";
			document.getElementById("sucScheDateTime").style.display = "block";
		}
		// 一回のみ
		else if (document.getElementById('<%= rbScheRepeatOnce.ClientID %>').checked)
		{
			document.getElementById('spScheTopString').innerText = '';
			document.getElementById('spScheDayOfWeek').style.display = 'none';

			document.getElementById('<%= ucScheDateTimeOnce.HfStartDate.ClientID %>').Value = '<%= DateTime.Now.ToString("yyyy/MM/dd") %>';
			document.getElementById('<%= ucScheDateTimeOnce.HfStartTime.ClientID %>').Value = '00:00:00';
			document.getElementById("sucScheDateTimeOnce").style.display = "block";
			document.getElementById("sucScheDateTime").style.display = "none";
		}
	}
	else
	{
		document.getElementById('dvScheduleDetail').style.display = 'none';

	}
}

// 配信時抽出チェックボックスを無効にするイベントを設定します
function BindEventDropdownTargetList() {
	var ddlTargetIds = $("#<%=ddlTargetId.ClientID %>, #<%=ddlTargetId2.ClientID %>, #<%=ddlTargetId3.ClientID %>, #<%=ddlTargetId4.ClientID %>, #<%=ddlTargetId5.ClientID %>");
	ddlTargetIds.each(function () {
		var thisDomObj = $(this);
		DisableCheckboxExtractData(thisDomObj);
		thisDomObj.change(function () {
			DisableCheckboxExtractData(thisDomObj);
		});
	});
}

// チェック ボックスを無効にします。
function DisableCheckboxExtractData(domObj) 
{
	var disableExtract = domObj.find(":selected").attr("disable_extract");
	if ((domObj.val() == "") || (disableExtract == "True")) domObj.next().removeAttr("checked").attr("disabled", "disabled");
	if (disableExtract == "False") domObj.next().removeAttr("disabled");
}

$(document).ready(BindEventDropdownTargetList);

//-->
</script>
</asp:Content>

