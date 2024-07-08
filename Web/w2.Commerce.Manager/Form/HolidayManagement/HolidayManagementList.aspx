<%--
=========================================================================================================
  Module      : 休日管理一覧ページ (HolidayManagementList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="HolidayManagementList.aspx.cs" Inherits="Form_HolidayManagement_HolidayManagementList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<table cellspacing="0" cellpadding="0" width="791" border="0">
		<tr><td><h1 class="page-title">休日設定</h1></td></tr>
		<tr><td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
		<tr><td><h2 class="cmn-hed-h2">休日設定一覧</h2></td></tr>
		<tr>
			<td style="width: 792px">
				<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
					<tr>
						<td>
							<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td><img height="10" width="1" border="0" alt="" src="../../Images/Common/sp.gif" /></td>
									<td align="center">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td>
													<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
														<tr>
															<td width="675" style="height: 22px"></td>
															<td width="83" class="action_list_sp" style="height: 22px">
																<asp:Button id="btnInsertTop" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" />
															</td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td>
													<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr class="list_title_bg">
															<td align="center" width="130">年</td>
															<td align="center" width="130">休日数</td>
															<td align="center" width="249">更新日</td>
															<td align="center" width="249">更新者</td>
														</tr>
														<asp:Repeater id="rList" Runat="server" ItemType="w2.Domain.Holiday.Helper.HolidaysInfoListItem">
															<ItemTemplate>
																<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)"
																	onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)"
																	onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateHolidayDetailUrl(Item.Year)) %>')">
																	<td align="center"><%#: Item.Year%></td>
																	<td align="center"><%#: Item.HolidayNumbers%></td>
																	<td align="center"><%#: DateTimeUtility.ToStringForManager(Item.DateChanged, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter)%></td>
																	<td align="center"><%#: Item.LastChanged%></td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
														<tr id="trListError" class="list_alert" runat="server" Visible="false">
															<td id="tdErrorMessage" runat="server" colspan="4"></td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td class="action_list_sp"><asp:Button id="btnInsertBottom" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" /></td>
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
</asp:Content>
