<%--
=========================================================================================================
  Module      : 会員ランク変動ルール登録ページ(MemberRankRuleRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Global.Region.Currency" %>
<%@ MasterType VirtualPath="~/Form/Common/DefaultPage.master" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MemberRankRuleRegister.aspx.cs" Inherits="Form_MemberRankRule_MemberRankRuleRegister" %>
<%@ Register TagPrefix="uc" TagName="DateTimeInput" Src="~/Form/Common/DateTimeInput.ascx" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<%
	rbExecBySchedule.Attributes.Add("OnClick", "RefleshMemberRankRuleSchedule();");
	//rbExecByAction.Attributes.Add("OnClick", "RefleshMemberRankRuleSchedule();");
	rbExecByManual.Attributes.Add("OnClick", "RefleshMemberRankRuleSchedule();");

	rbScheRepeatDay.Attributes.Add("OnClick", "RefleshMemberRankRuleSchedule();");
	rbScheRepeatWeek.Attributes.Add("OnClick", "RefleshMemberRankRuleSchedule();");
	rbScheRepeatMonth.Attributes.Add("OnClick", "RefleshMemberRankRuleSchedule();");
	rbScheRepeatOnce.Attributes.Add("OnClick", "RefleshMemberRankRuleSchedule();");
%>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr>
		<td><h1 class="page-title">会員ランク変動ルール設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ タイトル △-->
	<!--▽ 編集入力 ▽-->
	<tr>
		<% if ((Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_INSERT) || (Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_COPY_INSERT)){%>
		<td><h2 class="cmn-hed-h2">会員ランク変動ルール設定登録</h2></td>
		<%} %>
		<% if (Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_UPDATE){%>
		<td><h2 class="cmn-hed-h2">会員ランク変動ルール設定編集</h2></td>
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
													<asp:HiddenField ID="hdnMemberRankRuleId" runat="server" />
													<tr>
														<td class="edit_title_bg" align="left" width="30%" colspan="2">会員ランク変動ルール名<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox ID="tbMemberRankRuleName" Width="300" runat="server" MaxLength="100"></asp:TextBox>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="15%" rowspan="<%= (Constants.MARKETINGPLANNER_TARGETLIST_OPTION_ENABLE) ? 5 : 4 %>" >抽出条件</td>
														<td class="edit_title_bg" align="left" width="15%">集計期間<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:RadioButton ID="rbTargetExtractTypeDuring" GroupName="TargetExtractType" runat="server" Text="期間指定" />
															<div id="targetExtract" style="display: inline-block;">
																<uc:DateTimePickerPeriodInput ID="ucTargetExtractDatePeriod" runat="server" />
															</div>
															<br />
															<br />
															<asp:RadioButton ID="rbTargetExtractTypeDaysAgo" GroupName="TargetExtractType" runat="server" Text="前日指定" />
															<asp:TextBox ID="tbTargetExtractDaysAgo" Width="50" runat="server" MaxLength="5"></asp:TextBox>日前
															～
															当日
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="15%">集計期間内の<br />合計購入金額<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox ID="tbTargetExtractTotalPriceFrom" Width="80" runat="server" MaxLength="7"></asp:TextBox>&nbsp;<%= (CurrencyManager.IsJapanKeyCurrencyCode) ? "円" : "" %>以上
															～
															<asp:TextBox ID="tbTargetExtractTotalPriceTo" Width="80" runat="server" MaxLength="7"></asp:TextBox>&nbsp;<%= (CurrencyManager.IsJapanKeyCurrencyCode) ? "円" : "" %>以下
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="15%">集計期間内の<br />合計購入回数<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox ID="tbTargetExtractTotalCountFrom" Width="80" runat="server" MaxLength="4"></asp:TextBox>&nbsp;回以上
															～
															<asp:TextBox ID="tbTargetExtractTotalCountTo" Width="80" runat="server" MaxLength="4"></asp:TextBox>&nbsp;回以下
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="15%">旧会員ランク時の<br />(注文)情報も抽出</td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBox ID="cbTargetExtractOldRankFlg" Text="抽出" runat="server" />
														</td>
													</tr>
													<% if (Constants.MARKETINGPLANNER_TARGETLIST_OPTION_ENABLE) { %>
													<tr>
														<td class="edit_title_bg" align="left" width="15%">ターゲット<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:DropDownList id="ddlTargetId" runat="server"><asp:ListItem></asp:ListItem></asp:DropDownList>
															<asp:CheckBox id="cbTargetExtract" Text="適用時抽出" runat="server" /><br />
															<asp:DropDownList id="ddlTargetId2" runat="server"><asp:ListItem></asp:ListItem></asp:DropDownList>
															<asp:CheckBox id="cbTargetExtract2" Text="適用時抽出" runat="server" /><br />
															<asp:DropDownList id="ddlTargetId3" runat="server"><asp:ListItem></asp:ListItem></asp:DropDownList>
															<asp:CheckBox id="cbTargetExtract3" Text="適用時抽出" runat="server" /><br />
															<asp:DropDownList id="ddlTargetId4" runat="server"><asp:ListItem></asp:ListItem></asp:DropDownList>
															<asp:CheckBox id="cbTargetExtract4" Text="適用時抽出" runat="server" /><br />
															<asp:DropDownList id="ddlTargetId5" runat="server"><asp:ListItem></asp:ListItem></asp:DropDownList>
															<asp:CheckBox id="cbTargetExtract5" Text="適用時抽出" runat="server" /><br />
															※ () 内の数はターゲット作成時のデータ数です。
														</td>
													</tr>
													<% } %>
													<tr>
														<td class="edit_title_bg" align="left" width="30%" colspan="2">ランク付与方法<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:RadioButtonList ID="rblRankChangeType" RepeatDirection="Horizontal" runat="server"></asp:RadioButtonList>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%" colspan="2">指定付与ランク<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:DropDownList id="ddlRankChangeRank" runat="server"></asp:DropDownList>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%" colspan="2">メールテンプレート</td>
														<td class="edit_item_bg" align="left">
															<asp:DropDownList id="ddlMailTemp" runat="server"></asp:DropDownList>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" colspan="2">実行タイミング<span class="notice">*</span></td>
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
																<div style="padding-left:20px;">
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
														<td class="edit_title_bg" align="left" width="30%" colspan="2">有効フラグ</td>
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
														<td align="left">備考（各項目の説明）<br />
															■会員ランク変動ルール<br />
															・集計期間 ・・・ユーザーの受注履歴を集計する期間を指定(出荷日基準）<br />
															・集計期間内の合計購入金額 ・・・集計期間内でユーザーが購入した合計金額との比較金額を指定<br />
															・集計期間内の合計購入回数 ・・・集計期間内でユーザーが購入した合計回数との比較回数を指定<br />
															・旧会員ランク時の(注文)情報も抽出 ・・・ユーザーの現在の会員ランクで購入した以外の受注も集計するかを指定<br />
															・ランク付与方法 ・・・現在のランクをUPさせるのか、DOWNさせるのかを指定<br />
															・メールテンプレート ・・・ランク付与時に送信するメールのテンプレートを指定<br />
															<br />
															■ スケジュール実行について<br />
															月単位 ・・・指定した日付で毎月実行されます。 対象月に指定した日付が存在しない場合は対象月での実行がスキップされます。<br />
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
	function RefleshMemberRankRuleSchedule() {
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

				document.getElementById("sucScheDateTime").style.display = "block";
				document.getElementById("sucScheDateTimeOnce").style.display = "none";
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

				document.getElementById("sucScheDateTime").style.display = "block";
				document.getElementById("sucScheDateTimeOnce").style.display = "none";
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

				document.getElementById("sucScheDateTime").style.display = "block";
				document.getElementById("sucScheDateTimeOnce").style.display = "none";
			}
			// 一回のみ
			else if (document.getElementById('<%= rbScheRepeatOnce.ClientID %>').checked) {
				document.getElementById('spScheTopString').innerText = '';
				document.getElementById('spScheDayOfWeek').style.display = 'none';

				document.getElementById('<%= ucScheDateTimeOnce.HfEndDate.ClientID %>').Value = '<%= DateTime.Now.ToString("yyyy/MM/dd") %>';
				document.getElementById('<%= ucScheDateTimeOnce.HfEndTime.ClientID %>').Value = '00:00:00';

				document.getElementById("sucScheDateTimeOnce").style.display = "block";
				document.getElementById("sucScheDateTime").style.display = "none";
			}
		}
		else {
			document.getElementById('dvScheduleDetail').style.display = 'none';

		}
}
//-->
</script>
</asp:Content>
