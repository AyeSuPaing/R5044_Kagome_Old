<%--
=========================================================================================================
  Module      : インシデントカテゴリ登録ページ(IncidentCategoryRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Cs.IncidentCategory" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="IncidentCategoryRegister.aspx.cs" Inherits="Form_IncidentCategory_IncidentCategoryRegister" Title="" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<script type="text/javascript">
<!--
	var selected_td_id = "";
	var selected_td = null;

	// 選択状態のカテゴリにスタイルを適用
	function set_selected_category_style()
	{
		<% if (String.IsNullOrEmpty(this.CategoryId) == false) { %>
			selected_td_id = "tdDivisionId_<%= this.CategoryId %>";
			selected_td = document.getElementById(selected_td_id);					
			selected_td.className=class_bgcolclck;
		<% } %>
	}

	//----------------------------------------------------
	// リスト選択列保持用（一行選択時、該当列が選択済の場合は色を変えない）
	//----------------------------------------------------
	function listselect_mover_uilst(obj)
	{			
		if (obj != selected_td)
		{
			listselect_mover(obj);
		}
	}
	function listselect_mout1_uilst(obj)
	{
		if (obj != selected_td)
		{
			listselect_mout1(obj)
		}
	}
	function listselect_mout2_uilst(obj)
	{
		if (obj != selected_td)
		{
			listselect_mout2(obj)
		}
	}			

	$(function () {
		ddlSelect();
	});
//-->
</script>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td colspan="2"></td>
	</tr>
	<tr>
		<td colspan="2"><h1 class="page-title">インシデントカテゴリ設定</h1></td>
	</tr>
	<tr>
		<td colspan="2"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧・詳細 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">インシデントカテゴリ設定一覧</h2></td>
		<td>
			<h2 class="cmn-hed-h2">
				<% if ((this.ActionStatus == Constants.ACTION_STATUS_UPDATE) || (this.ActionStatus == Constants.ACTION_STATUS_COMPLETE)){%>
					インシデントカテゴリ設定編集
				<%} %>
				<% if (this.ActionStatus == Constants.ACTION_STATUS_INSERT){%>
					インシデントカテゴリ設定登録
				<%} %>
				<% if (this.ActionStatus == null){%>
					インシデントカテゴリ設定詳細
				<%} %>
			</h2>
		</td>
	</tr>
	<tr>
		<td valign="top">
			<table class="box_border" cellspacing="0" cellpadding="0" width="375" border="0" style="margin-top: -55px;">
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
														<td><img height="52" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
													<tr>
														<td>
															<div class="action_part_top">
																<div style="float: left" class="action_part_top">総カテゴリ件数 ： <%= this.TotalCategoryCount %>件</div>
																<asp:button ID="btnIncidentCategoryInsertTop" runat="server" Text="カテゴリの新規登録" Width="130px" onclick="btnIncidentCategoryInsert_Click"></asp:button>
															</div>
															<table class="list_table" cellspacing="1" cellpadding="3" width="375" border="0">
																<tr class="list_title_bg">
																	<td align="center" width="188">カテゴリ名称</td>
																	<td align="center" width="64" runat="server">表示順</td>
																	<td align="center" width="64" runat="server">有効フラグ</td>
																</tr>
																<asp:repeater id="rList" Runat="server">
																	<ItemTemplate>
																		<tr id='tdDivisionId_<%# ((CsIncidentCategoryModel)Container.DataItem).CategoryId %>' class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover_uilst(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>_uilst(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# CreateEditUrl(((CsIncidentCategoryModel)Container.DataItem).CategoryId) %>')">
																			<td align="left">&nbsp;<%# WebSanitizer.HtmlEncode(((CsIncidentCategoryModel)Container.DataItem).CategoryName) %></td>
																			<td align="center"><%# WebSanitizer.HtmlEncode(((CsIncidentCategoryModel)Container.DataItem).DisplayOrder.ToString()) %></td>
																			<td align="center"><%# WebSanitizer.HtmlEncode(((CsIncidentCategoryModel)Container.DataItem).EX_ValidFlgText + ((((CsIncidentCategoryModel)Container.DataItem).EX_IsParentValid == false) ? "(無効)" : "")) %></td>
																		</tr>
																	</ItemTemplate>
																</asp:repeater>
																<tr class="list_alert" id="trListError" runat="server" Visible="false">
																	<td id="tdErrorMessage" colspan="3" runat="server"></td>
																</tr>
															</table>
															<div class="action_part_bottom">
																<asp:button ID="btnIncidentCategoryInsertBottom" runat="server" Text="カテゴリの新規登録" Width="130px" onclick="btnIncidentCategoryInsert_Click"></asp:button>
															</div>
														</td>
													</tr>
													<tr>
														<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
													<tr>
														<td>
															<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
																<tr class="info_item_bg">
																	<td align="left">備考<br />
																		■有効フラグ<br />
																		・「〇」の場合は有効、「〇(無効)」と「－(無効)」の場合は無効となります。
																	</td>
																</tr>
															</table>
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
														<td style="HEIGHT: 10px"><img height="6" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
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
																		<td align="left">下記の情報でカテゴリ情報を登録/更新しました。
																		</td>
																	</tr>
																</table>
																<br />
															</div>
														</td>
														<td style="HEIGHT: 10px"><img height="6" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
													</tr>
													<tr id="trEdit" runat="server" Visible="False">
														<td><img height="6" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
														<td>
															<table class="edit_table" cellspacing="1" cellpadding="3" width="370" border="0">
																<tr>
																	<td class="edit_title_bg" align="left" width="30%">カテゴリ名称<span class="notice">*</span></td>
																	<td class="edit_item_bg"><asp:textbox id="tbDivisionName" runat="server" Text="<%# this.CategoryInfo.CategoryName %>" Width="210" MaxLength="30"></asp:textbox></td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left" width="30%">表示順<span class="notice">*</span></td>
																	<td class="edit_item_bg" align="left"><asp:DropDownList id="ddlDisplayOrder" Runat="server" SelectedValue='<%# this.CategoryInfo.DisplayOrder %>'></asp:DropDownList></td>
																</tr>
																<tr>
																	<td class="edit_title_bg" align="left" width="30%">有効フラグ</td>
																	<td class="edit_item_bg" align="left">
																		<asp:CheckBox id="cbValidFlg" Runat="server" Checked="<%# this.CategoryInfo.ValidFlg == Constants.FLG_CSINCIDENTCATEGORY_VALID_FLG_VALID %>" Text="有効" />
																	</td>
																</tr>
																<tr>
																	<td class="edit_title_bg" id="tdParentCategoryRegisterHead" align="left" width="30%" runat="server" Visible="False">親カテゴリ</td>
																	<td class="edit_item_bg" id="tdParentCategoryRegister" align="left" runat="server" Visible="False">
																		<asp:DropDownList id="ddlParentCategory" Runat="server" CssClass="select2-select" SelectedValue="<%# this.CategoryInfo.ParentCategoryId %>" Width="60%"></asp:DropDownList>
																	</td>
																</tr>
															</table>
															<div class="action_part_bottom" style="margin-bottom:0px">
																<asp:button id="btnIncidentCategoryRegister" runat="server" Text=" 登録する　" Width="80px" Visible="False" onclick="btnIncidentCategoryRegister_Click"></asp:button>
																<asp:button id="btnIncidentCategoryModify" runat="server" Text=" 更新する　" Width="80px" Visible="False" onclick="btnIncidentCategoryModify_Click"></asp:button>
																<asp:button id="btnIncidentCategoryDelete" runat="server" Text=" 削除する　" Width="80px" Visible="False" onclick="btnIncidentCategoryDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');"></asp:button>
															</div>
														</td>
														<td><img height="6" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
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
	set_selected_category_style();
//-->
</script>
</asp:Content>
