<%--
=========================================================================================================
  Module      : 拠点グループ設定登録ページ(CsGroupRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Cs.CsOperator" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="CsGroupRegister.aspx.cs" Inherits="Form_CsGroup_CsGroupRegister" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<script type="text/javascript">
<!--
	var selected_tr_id = "";
	var selected_tr = null;

	function selected_cs_group(obj)
	{
		<% if (this.EditModel.CsGroupId != "") { %>
		selected_tr_id = "trCsGroupId_<%= this.EditModel.CsGroupId %>";
		selected_tr = document.getElementById(selected_tr_id);
		selected_tr.className = class_bgcolclck;
		<% } %>
	}

	//----------------------------------------------------
	// リスト選択列保持用（一行選択時、該当列が選択済の場合は色を変えない）
	//----------------------------------------------------
	function listselect_mover_uilst(obj)
	{
		if (obj != selected_tr) listselect_mover(obj);
	}
	function listselect_mout1_uilst(obj)
	{
		if (obj != selected_tr) listselect_mout1(obj);
	}
	function listselect_mout2_uilst(obj)
	{
		if (obj != selected_tr) listselect_mout2(obj);
	}			
	
//-->
</script>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td colspan="2"></td>
	</tr>
	<tr>
		<td colspan="2"><h1 class="page-title">拠点グループ設定</h1></td>
	</tr>
	<tr>
		<td colspan="2"><img height="10" alt="" src="Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧・詳細 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">拠点グループ設定一覧</h2></td>
		<td>
			<h2 class="cmn-hed-h2">
				<% if ((this.ActionStatus == Constants.ACTION_STATUS_UPDATE) || (this.ActionStatus == Constants.ACTION_STATUS_COMPLETE)){%>
					拠点グループ設定編集
				<%} %>
				<% else if (this.ActionStatus == Constants.ACTION_STATUS_INSERT){%>
					拠点グループ設定登録
				<%} %>
				<% else {%>
					拠点グループ設定詳細
				<%} %>
			</h2>
		</td>
	</tr>
	<tr>
		<td valign="top">
			<table class="box_border" cellspacing="0" cellpadding="0" width="375" border="0">
				<tr>
					<td>
						<table cellspacing="1" cellpadding="0" width="100%" border="0">
							<tr>
								<td>
									<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
										<tr>
											<td align="center">
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
													<tr>
														<td>
															<div class="action_part_top">
																<asp:button ID="btnInsertTop" runat="server" Text="グループの新規登録" Width="130px" onclick="btnInsert_Click"></asp:button>
															</div>
														</td>
													</tr>
													<tr>
														<td>
															<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
																<tr class="info_item_bg">
																	<td align="left">総グループ件数 ： <%# StringUtility.ToNumeric(((CsGroupModel[])rList.DataSource).Length) %>件
																	</td>
																</tr>
															</table>
														</td>
													</tr>
													<tr>
														<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
													<tr>
														<td>
															<table class="list_table" cellspacing="1" cellpadding="3" width="375" border="0">
																<tr class="list_title_bg">
																	<td align="center" width="188">グループ名</td>
																	<td align="center" width="64">表示順</td>
																	<td align="center" width="64">有効フラグ</td>
																</tr>
																<asp:repeater id="rList" Runat="server">
																	<ItemTemplate>
																		<tr id="trCsGroupId_<%# ((CsGroupModel)Container.DataItem).CsGroupId %>" class='list_item_bg<%# Container.ItemIndex % 2 + 1 %>' onmouseover="listselect_mover_uilst(this)" onmouseout='listselect_mout<%# Container.ItemIndex % 2 + 1 %>_uilst(this)' onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# CreateEditUrl(((CsGroupModel)Container.DataItem).CsGroupId) %>')">
																			<td align="left">&nbsp;<%# WebSanitizer.HtmlEncode(((CsGroupModel)Container.DataItem).CsGroupName) %></td>
																			<td align="center"><%# WebSanitizer.HtmlEncode(((CsGroupModel)Container.DataItem).DisplayOrder) %></td>
																			<td align="center"><%# WebSanitizer.HtmlEncode(((CsGroupModel)Container.DataItem).EX_ValidFlgText) %></td>
																		</tr>
																	</ItemTemplate>
																</asp:repeater>
																<tr class="list_alert" id="trListError" runat="server" Visible="false">
																	<td id="tdErrorMessage" colspan="3" runat="server"></td>
																</tr>
															</table>
															<div class="action_part_bottom">
																<asp:button id="btnInsertBottom" runat="server" Text="グループの新規登録" Width="130px" onclick="btnInsert_Click"></asp:button>
															</div>
														</td>
													</tr>
												</table>
											</td>
											<td><img height="16" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
		<td valign="top">
			<table class="box_border" cellspacing="0" cellpadding="3" width="375" border="0">
				<tr>
					<td>
						<table cellspacing="1" cellpadding="0" width="100%" border="0">
							<tr>
								<td>
									<table class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
										<tr>
											<td align="center">
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
													<tr id="trNotSelect" runat="server" Visible="True">
														<td style="HEIGHT: 10px">
															<div id="divErrorMessage" runat="server">
																<table class="detail_table" cellspacing="1" cellpadding="3" width="370" border="0">
																	<tr>
																		<td class="detail_item_bg">
																			<div id="divNotSelectMessage" runat="server"></div>
																		</td>
																	</tr>
																</table>
															</div>
															<div id="divComp" runat="server" Visible="False">
																<table class="info_table" cellspacing="1" cellpadding="3" width="370" border="0">
																	<tr class="info_item_bg">
																		<td align="left">下記の情報でグループ設定を登録/更新しました。
																		</td>
																	</tr>
																</table>
																<br />
															</div>
														</td>
													</tr>
													<tr id="trEdit" runat="server" Visible="False">
														<td>
															<table class="edit_table" cellspacing="1" cellpadding="3" width="370" border="0">
																<tr>
																	<td class="edit_title_bg" align="left" width="30%">グループ名<span class="notice">*</span></td>
																	<td class="edit_item_bg"><asp:textbox id="tbCsGroupName" runat="server" MaxLength="30" Text="<%# this.EditModel.CsGroupName %>"></asp:textbox></td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left" width="30%">表示順<span class="notice">*</span></td>
																	<td class="edit_item_bg" align="left"><asp:DropDownList id="ddlDisplayOrder" Runat="server" SelectedValue="<%# this.EditModel.DisplayOrder %>" /></td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left" width="30%">有効フラグ</td>
																	<td class="edit_item_bg" align="left">
																		<asp:CheckBox runat="server" ID="chkValidFlg" Checked='<%# this.EditModel.ValidFlg == Constants.FLG_CSGROUP_VALID_FLG_VALID %>' Text="有効"/>
																	</td>
																</tr>
															</table>
															<div class="action_part_bottom" style="margin-bottom:0px">
																<asp:button id="btnRegisterBottom" runat="server" Text=" 登録する" Width="80px" Visible="False" onclick="btnRegister_Click"></asp:button>
																<asp:button id="btnEditBottom" runat="server" Text=" 更新する" Width="80px" Visible="False" onclick="btnEdit_Click"></asp:button>
																<asp:button id="btnDeleteBottom" runat="server" Text=" 削除する" Width="80px" Visible="False" onclick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');"></asp:button>
															</div>
														</td>
														<td><img height="4" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
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
			</table>
		</td>
	</tr>
	<!--△ 一覧・詳細 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<script type="text/javascript">
<!--
	selected_cs_group();
//-->
</script>
</asp:Content>