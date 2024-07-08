<%--
=========================================================================================================
  Module      : 休日情報登録画面 (HolidayRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="HolidayRegister.aspx.cs" Inherits="Form_HolidayManagement_HolidayRegister" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
<style type="text/css">
/* カレンダーモジュール */
.calendar_control td
{
	color: #505050;
	padding-top: 2px;
	padding-left: 0px;
	padding-right: 0px;
	padding-bottom: 2px;
	font-size: 13pt;
	font-family: "MS UI Gothic";
}

/* カレンダーモジュール・タイトル */
.calendar_title td
{
	background-color: #e7e7e7;
	color :#505050;
}

/* カレンダーモジュール・曜日 */
.calendar_dayheader_bg
{
	background-color: #e7e7e7;
	font-size: 12pt;
}
</style>
<asp:UpdatePanel runat="server">
	<ContentTemplate>
	<table cellspacing="0" cellpadding="0" width="791" border="0">
		<tr><td><h1 class="page-title">休日設定</h1></td></tr>
		<tr><td><img height="10" alt="" src="../../Images/Common/sp.gif" border="0" /></td></tr>
		<tr id="trRegister" runat="server" Visible="False">
			<td><h1 class="cmn-hed-h2">休日設定登録</h1></td>
		</tr>
		<tr id="trEdit" runat="server" Visible="False">
			<td><h1 class="cmn-hed-h2">休日設定編集</h1></td>
		</tr>
		<tr>
			<td style="width: 792px">
				<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
					<tr>
						<td>
							<table class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td><img height="10" width="1" border="0" alt="" src="../../Images/Common/sp.gif" /></td>
									<td align="center">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td>
													<div id="divComp" runat="server" class="action_part_top" Visible="false">
														<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
															<tr class="info_item_bg">
																<td align="left">
																	<asp:Literal id="lCompMessages" runat="server" Text="休日情報を登録/更新しました。" /></td>
															</tr>
														</table>
													</div>
													<div class="action_part_top">
														<asp:Button ID="btnToListTop" runat="server" Text="  一覧へ戻る  " OnClick="btnToList_Click" />
														<asp:Button ID="btnInsertTop" runat="server" Text="  登録する  " OnClick="btnInsert_Click" />
														<asp:Button ID="btnUpdateTop" runat="server" Text="  更新する  " OnClick="btnUpdate_Click" />
														<asp:Button ID="btnDeleteTop" runat="server" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('休日情報を削除してもよろしいですか？');" />
													</div>
													<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr>
															<td class="detail_title_bg" align="center" colspan="2">休日情報</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="20%">設定対象<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left">
																<asp:DropDownList ID="ddlYear" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlYear_SelectedIndexChanged" />年
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" rowspan="2">休日詳細設定<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left">
																毎週の
																<asp:CheckBoxList ID="cblDayOfWeek" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" />
																<asp:Button ID="btnReflect" Text="  反映する  " runat="server" OnClick="btnReflect_Click" />
															</td>
														</tr>
														<tr>
															<td class="edit_item_bg" align="center">
																<table cellspacing="0" cellpadding="0" border="0">
																	<asp:Repeater id="rHoliday" Runat="server" DataSource="<%# m_calendarList %>" ItemType="System.DateTime">
																		<ItemTemplate>
																			<%# (Container.ItemIndex + 1) % 2 == 1 ? "<tr>":"" %>
																			<td class="edit_item_bg" align="center">
																			<span>
																				<asp:Calendar id="cldHoliday" runat="server" Width="270px" 
																					OnSelectionChanged="cldHoliday_SelectionChanged" OnDayRender="cldHoliday_DayRender" 
																					VisibleDate="<%# Item %>" 
																					ShowNextPrevMonth="False" CellPadding="3" CellSpacing="1" ForeColor="#0864AA" 
																					BorderColor="#ece9d8" CssClass="calendar_control">
																					<TitleStyle CssClass="calendar_title"></TitleStyle>
																					<DayHeaderStyle CssClass="calendar_dayheader_bg"></DayHeaderStyle>
																					<OtherMonthDayStyle ForeColor="LightGray"></OtherMonthDayStyle>
																				</asp:Calendar>
																			</span>
																			</td>
																			<%# (Container.ItemIndex + 1) % 2 == 0 ? "</tr>":"" %>
																		</ItemTemplate>
																	</asp:Repeater>
																</table>
															</td>
														</tr>
													</table>
													<div class="action_part_bottom">
														<asp:Button ID="btnToListBottom" runat="server" Text="  一覧へ戻る  " OnClick="btnToList_Click" />
														<asp:Button ID="btnInsertBottom" runat="server" Text="  登録する  " OnClick="btnInsert_Click" />
														<asp:Button ID="btnUpdateBottom" runat="server" Text="  更新する  " OnClick="btnUpdate_Click" />
														<asp:Button ID="btnDeleteBottom" runat="server" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('休日情報を削除してもよろしいですか？');" />
													</div>
												</td>
											</tr>
											<tr>
												<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
		<tr>
			<td>
				<img height="10" width="1" border="0" alt="" src="../../Images/Common/sp.gif" />
			</td>
		</tr>
	</table>
	</ContentTemplate>
</asp:UpdatePanel>
</asp:Content>

