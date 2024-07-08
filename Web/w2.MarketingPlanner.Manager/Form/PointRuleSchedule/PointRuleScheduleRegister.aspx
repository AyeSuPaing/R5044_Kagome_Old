<%--
=========================================================================================================
  Module      : ポイントルールスケジュール登録ページ(PointRuleScheduleRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%@ MasterType VirtualPath="~/Form/Common/DefaultPage.master" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="PointRuleScheduleRegister.aspx.cs" Inherits="Form_PointRuleSchedule_PointRuleScheduleRegister" %>
<%@ Register TagPrefix="uc" TagName="DateTimeInput" Src="~/Form/Common/DateTimeInput.ascx" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<%
	rbExecBySchedule.Attributes.Add("OnClick", "RefreshSchedule();");
	//rbExecByAction.Attributes.Add("OnClick", "RefreshSchedule();");
	rbExecByManual.Attributes.Add("OnClick", "RefreshSchedule();");

	rbScheRepeatDay.Attributes.Add("OnClick", "RefreshSchedule();");
	rbScheRepeatWeek.Attributes.Add("OnClick", "RefreshSchedule();");
	rbScheRepeatMonth.Attributes.Add("OnClick", "RefreshSchedule();");
	rbScheRepeatOnce.Attributes.Add("OnClick", "RefreshSchedule();");
%>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr>
		<td><h1 class="page-title">ポイントルールスケジュール設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ タイトル △-->
	<!--▽ 編集入力 ▽-->
	<tr>
		<% if ((this.ActionStatus == Constants.ACTION_STATUS_INSERT) || (this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT)){%>
		<td><h2 class="cmn-hed-h2">ポイントルールスケジュール設定登録</h2></td>
		<%} %>
		<% if (this.ActionStatus == Constants.ACTION_STATUS_UPDATE){%>
		<td><h2 class="cmn-hed-h2">ポイントルールスケジュール設定編集</h2></td>
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
												<div class="action_part_top"><input onclick="Javascript: history.back()" type="button" value="  戻る  " />
												<asp:Button id="btnConfirm" runat="server" Text="  確認する  " onclick="btnConfirm_Click" ></asp:Button>
												</div>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<asp:HiddenField ID="hfPointRuleScheduleId" runat="server" />
													<asp:HiddenField ID="hfLastCount" runat="server" />
													<asp:HiddenField ID="hfLastExecDate" runat="server" />
													<tr>
														<td class="edit_title_bg" align="left" width="30%">ポイントルールスケジュール名<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox ID="tbPointRuleScheduleName" Width="300" runat="server" MaxLength="100"></asp:TextBox>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">ターゲット<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:DropDownList id="ddlTargetId" runat="server"><asp:ListItem></asp:ListItem></asp:DropDownList>
																<asp:CheckBox id="cbTargetExtract" Text="配信時抽出" runat="server" /><br />
															※ () 内の数はターゲット作成時のデータ数です。
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">ポイントルール<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:DropDownList id="ddlPointRule" runat="server"></asp:DropDownList>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">メール文章</td>
														<td class="edit_item_bg" align="left">
															<asp:DropDownList id="ddlMailTemp" runat="server"></asp:DropDownList>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg">実行タイミング<span class="notice">*</span></td>
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
																	<span id="spScheDateTime" style="display:block; float:left">
																		<uc:DateTimeInput ID="ucScheDateTime" runat="server" YearList="<%# DateTimeUtility.GetBackwardYearListItem() %>" HasTime="True" HasBlankSign="False" HasBlankValue="False" ControlAutoPostBack="False" />
																	</span>
																	<span id="spScheDateTimeOnce" style="float:left">
																		<uc:DateTimePickerPeriodInput ID="ucScheDateTimeOnce" runat="server" CanShowEndDatePicker="False" IsNullEndDateTime="True"/>
																	</span>
																</div>
															</div>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">有効フラグ</td>
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
												<br/>
												<table class="info_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr>
														<td align="left" class="info_item_bg" colspan="2">備考<br />
															■ スケジュール実行について<br />
															<table class="no-border no-padding">
																<tr>
																	<td>月単位</td>
																	<td>指定した日付で毎月実行されます。 対象月に指定した日付が存在しない場合は対象月での実行がスキップされます。</td>
																</tr>
															</table>
														</td>
													</tr>
												</table>
												<div class="action_part_bottom"><input onclick="Javascript: history.back()" type="button" value="  戻る  " />
													<asp:Button id="btnConfirm2" runat="server" Text="  確認する  " onclick="btnConfirm_Click" ></asp:Button></div>
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
	function RefreshSchedule() {
		// 詳細スケジュール設定？
		if (document.getElementById('<%= rbExecBySchedule.ClientID %>').checked) {
			document.getElementById('dvScheduleDetail').style.display = 'inline';

			// 日単位
			if (document.getElementById('<%= rbScheRepeatDay.ClientID %>').checked) {
				document.getElementById('spScheTopString').innerText = '毎日';
				document.getElementById('spScheDayOfWeek').style.display = 'none';

				document.getElementById('<%= ucScheDateTime.DdlYear.ClientID %>').style.display = 'none';
				document.getElementById('<%= ucScheDateTime.DdlMonth.ClientID %>').style.display = 'none';
				document.getElementById('<%= ucScheDateTime.DdlDay.ClientID %>').style.display = 'none';
				document.getElementById('<%= ucScheDateTime.MonthYearSeparator.ClientID %>').style.display = 'none';
				document.getElementById('<%= ucScheDateTime.DaySeparator.ClientID %>').style.display = 'none';
				document.getElementById('<%= ucScheDateTime.DayString.ClientID %>').style.display = 'none';
				document.getElementById("spScheDateTimeOnce").style.display = "none";
				document.getElementById("spScheDateTime").style.display = "block";
			}
			// 週単位
			else if (document.getElementById('<%= rbScheRepeatWeek.ClientID %>').checked) {
				document.getElementById('spScheTopString').innerText = '毎週';
				document.getElementById('spScheDayOfWeek').style.display = 'inline';

				document.getElementById('<%= ucScheDateTime.DdlYear.ClientID %>').style.display = 'none';
				document.getElementById('<%= ucScheDateTime.DdlMonth.ClientID %>').style.display = 'none';
				document.getElementById('<%= ucScheDateTime.DdlDay.ClientID %>').style.display = 'none';
				document.getElementById('<%= ucScheDateTime.MonthYearSeparator.ClientID %>').style.display = 'none';
				document.getElementById('<%= ucScheDateTime.DaySeparator.ClientID %>').style.display = 'none';
				document.getElementById('<%= ucScheDateTime.DayString.ClientID %>').style.display = 'none';
				document.getElementById("spScheDateTimeOnce").style.display = "none";
				document.getElementById("spScheDateTime").style.display = "block";
			}
			// 月単位
			else if (document.getElementById('<%= rbScheRepeatMonth.ClientID %>').checked) {
				document.getElementById('spScheTopString').innerText = '毎月';
				document.getElementById('spScheDayOfWeek').style.display = 'none';

				document.getElementById('<%= ucScheDateTime.DdlYear.ClientID %>').style.display = 'none';
				document.getElementById('<%= ucScheDateTime.DdlMonth.ClientID %>').style.display = 'none';
				document.getElementById('<%= ucScheDateTime.DdlDay.ClientID %>').style.display = 'inline';
				document.getElementById('<%= ucScheDateTime.MonthYearSeparator.ClientID %>').style.display = 'none';
				document.getElementById('<%= ucScheDateTime.DaySeparator.ClientID %>').style.display = 'none';
				document.getElementById('<%= ucScheDateTime.DayString.ClientID %>').style.display = 'inline';
				document.getElementById("spScheDateTimeOnce").style.display = "none";
				document.getElementById("spScheDateTime").style.display = "block";
			}
			// 一回のみ
			else if (document.getElementById('<%= rbScheRepeatOnce.ClientID %>').checked) {
				document.getElementById('spScheTopString').innerText = '';
				document.getElementById('spScheDayOfWeek').style.display = 'none';

				document.getElementById('<%= ucScheDateTimeOnce.HfEndDate.ClientID %>').Value = '<%= DateTime.Now.ToString("yyyy/MM/dd") %>';
				document.getElementById('<%= ucScheDateTimeOnce.HfEndTime.ClientID %>').Value = '00:00:00';
				document.getElementById("spScheDateTimeOnce").style.display = "block";
				document.getElementById("spScheDateTime").style.display = "none";
			}
		}
		else {
			document.getElementById('dvScheduleDetail').style.display = 'none';

		}
}
//-->
</script>
</asp:Content>
