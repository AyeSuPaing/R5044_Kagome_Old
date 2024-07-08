<%--
=========================================================================================================
  Module      : 受注ワークフローシナリオ一覧ページ(OrderWorkflowScenarioList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>
<%@ Reference Page="~/Form/PdfOutput/PdfOutput.aspx" %>
<%@ Page Language="C#" Title="受注ワークフローシナリオ設定" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="OrderWorkflowScenarioList.aspx.cs" Inherits="Form.OrderWorkflowAutoExec.OrderWorkflowScenarioList" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td>
			<h1 class="page-title">受注ワークフローシナリオ設定</h1>
		</td>
	</tr>
	<tr>
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>　
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">シナリオ一覧</h2></td>
	</tr>
	<tr>
		<td style="width: 792px">
			<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<!--▽ ページング ▽-->
												<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
													<tr>
														<td width="675" style="height: 22px"><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
														<td style="text-align:right" class="action_list_sp">
															<asp:Button id="btnInsertTop" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" />
														</td>
													</tr>
												</table>
												<!-- ページング-->
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="list_title_bg">
														<td align="center" width="60%" style="height: 17px">シナリオ名</td>
														<td align="center" width="30%" style="height: 17px">実行タイミング</td>
														<td align="center" width="10%" style="height: 17px">有効フラグ</td>
													</tr>
													<asp:Repeater ID="rScenario" ItemType="w2.Domain.OrderWorkflowScenarioSetting.OrderWorkflowScenarioSettingModel" runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%#: Container.ItemIndex % 2 + 1 %>"
																onmouseover="listselect_mover(this)"
																onmouseout="listselect_mout<%#: Container.ItemIndex % 2 + 1 %>(this)"
																onmousedown="listselect_mdown(this)"
																onclick="listselect_mclick(this, '<%# CreateScenarioUpdateUrl(Item.ScenarioSettingId) %>')">
																<td align="center"><%#: Item.ScenarioName %></td>
																<td align="center"><%# CreateExecTimingForDisplay(Item) %></td>
																<td align="center"><%#: ConversionValidFlgString(Item.ValidFlg) %></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="3"></td>
													</tr>
												</table>
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
												</table>
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
	<!--△ 一覧 △-->
	<tr>
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
