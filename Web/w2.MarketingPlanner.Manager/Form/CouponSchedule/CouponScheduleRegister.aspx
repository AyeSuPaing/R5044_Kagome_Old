<%--
=========================================================================================================
  Module      : クーポン発行スケジュール登録ページ(CouponScheduleRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ MasterType VirtualPath="~/Form/Common/DefaultPage.master" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="CouponScheduleRegister.aspx.cs" Inherits="Form_CouponSchedule_CouponScheduleRegister" %>
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
		<td><h1 class="page-title">クーポン発行スケジュール設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ タイトル △-->
	<!--▽ 編集入力 ▽-->
	<tr>
		<% if ((this.ActionStatus == Constants.ACTION_STATUS_INSERT) || (this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT)){%>
		<td><h2 class="cmn-hed-h2">クーポン発行スケジュール設定登録</h2></td>
		<%} %>
		<% if (this.ActionStatus == Constants.ACTION_STATUS_UPDATE){%>
		<td><h2 class="cmn-hed-h2">クーポン発行スケジュール設定編集</h2></td>
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
												<asp:UpdatePanel runat="server">
												<ContentTemplate>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<asp:HiddenField ID="hfCouponScheduleId" runat="server" />
													<tr>
														<td class="edit_title_bg" align="left" width="30%">クーポン発行スケジュール名<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox ID="tbCouponScheduleName" Width="300" runat="server" MaxLength="100"></asp:TextBox>
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
														<td class="edit_title_bg" align="left" width="30%">クーポン設定<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:DropDownList id="ddlCoupon" runat="server" OnSelectedIndexChanged="ddlCoupon_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
														</td>
													</tr>
													<tr runat="server" id="trCouponQuantity"><%-- 選択中のクーポン種別によって表示制御 --%>
														<td class="edit_title_bg" align="left" width="30%">クーポン発行枚数<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox ID="tbCouponQuantity" runat="server"></asp:TextBox>
															枚（一人当たりに発行される枚数）
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
																	<span id="spScheTopString" style="padding:7px; float:left"></span>
																	<span id="spScheDayOfWeek" style="padding-right:8px; padding-top:3px; float:left">
																		<asp:RadioButtonList ID="rblScheDayOfWeek" RepeatLayout="Flow" RepeatDirection="Horizontal" runat="server"></asp:RadioButtonList>
																	</span>
																	<span id="spScheYearMonth" style="float:left;">
																		<asp:DropDownList ID="ddlScheYear" runat="server"></asp:DropDownList>年
																		<asp:DropDownList ID="ddlScheMonth" runat="server"></asp:DropDownList>月
																	</span>
																	<span id="spScheDay" style="padding-right:5px; float:left;">
																		<asp:DropDownList ID="ddlScheDay" runat="server"></asp:DropDownList>日
																	</span>
																	<span id="spScheTime" style="float:left;">
																	<asp:DropDownList ID="ddlScheHour" runat="server"></asp:DropDownList>時
																	<asp:DropDownList ID="ddlScheMinute" runat="server"></asp:DropDownList>分
																	<asp:DropDownList ID="ddlScheSecond" runat="server"></asp:DropDownList>秒
																	</span>
																	<span id="spScheDateTimeOnce" style="float:left">
																		<uc:DateTimePickerPeriodInput ID="ucScheDateTimeOnce" runat="server" CanShowEndDatePicker="False" IsNullEndDateTime="True" />
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
												</ContentTemplate>
												</asp:UpdatePanel>
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
				document.getElementById('spScheYearMonth').style.display = 'none';
				document.getElementById('spScheDay').style.display = 'none';
				document.getElementById('spScheDayOfWeek').style.display = 'none';
				document.getElementById('spScheTime').style.display = 'block';
				document.getElementById("spScheDateTimeOnce").style.display = "none";
			}
				// 週単位
			else if (document.getElementById('<%= rbScheRepeatWeek.ClientID %>').checked) {
				document.getElementById('spScheTopString').innerText = '毎週';
				document.getElementById('spScheYearMonth').style.display = 'none';
				document.getElementById('spScheDay').style.display = 'none';
				document.getElementById('spScheDayOfWeek').style.display = 'inline';
				document.getElementById('spScheTime').style.display = 'block';
				document.getElementById("spScheDateTimeOnce").style.display = "none";
			}
				// 月単位
			else if (document.getElementById('<%= rbScheRepeatMonth.ClientID %>').checked) {
				document.getElementById('spScheTopString').innerText = '毎月';
				document.getElementById('spScheYearMonth').style.display = 'none';
				document.getElementById('spScheDay').style.display = 'inline';
				document.getElementById('spScheDayOfWeek').style.display = 'none';
				document.getElementById('spScheTime').style.display = 'block';
				document.getElementById("spScheDateTimeOnce").style.display = "none";
			}
				// 一回のみ
			else if (document.getElementById('<%= rbScheRepeatOnce.ClientID %>').checked) {
				document.getElementById('spScheTopString').innerText = '';
				document.getElementById('spScheDayOfWeek').style.display = 'none';
				document.getElementById('spScheYearMonth').style.display = 'none';
				document.getElementById('spScheDay').style.display = 'none';
				document.getElementById('spScheTime').style.display = 'none';
				document.getElementById("spScheDateTimeOnce").style.display = "block";
			}
		} else {
			document.getElementById('dvScheduleDetail').style.display = 'none';
		}
}

function bodyPageLoad() {
	var isAsyncPostback = Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();
	if (isAsyncPostback) {
		RefreshSchedule();
	}
}
//-->
</script>
</asp:Content>
