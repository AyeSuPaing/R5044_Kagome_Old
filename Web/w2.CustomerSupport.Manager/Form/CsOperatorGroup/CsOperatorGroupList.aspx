<%--
=========================================================================================================
  Module      : CSオペレータ所属グループ一覧ページ(CsOperatorGroupList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Cs.CsOperator" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="CsOperatorGroupList.aspx.cs" Inherits="Form_CsOperatorGroup_CsOperatorGroupList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">オペレータ所属設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">オペレータ所属設定一覧</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="0" cellpadding="0" width="784" border="0">
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
														<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
													<tr>
														<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
													<tr>
														<td>
															<table class="list_table" cellspacing="1" cellpadding="3" width="750" border="0">
																<tr class="list_title_bg">
																	<td align="center" width="120">グループ名</td>
																	<td align="center" width="378">所属オペレータ</td>
																	<td align="center" width="80">所属オペレータ数</td>
																</tr>
																<asp:repeater id="rList" Runat="server">
																	<ItemTemplate>
																		<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# CreateRegisterUrl(((CsGroupModel)Container.DataItem).CsGroupId) %>')">
																			<td align="left">&nbsp;<%# WebSanitizer.HtmlEncode(((CsGroupModel)Container.DataItem).CsGroupName) %>&nbsp;</td>
																			<td align="left">
																				<asp:Repeater runat="server" DataSource="<% #((CsGroupModel)Container.DataItem).Ex_Operators %>" >
																					<ItemTemplate>
																						&nbsp;<%# WebSanitizer.HtmlEncode(((CsOperatorModel)Container.DataItem).EX_ShopOperatorName) %>
																						<br />
																					</ItemTemplate>
																				</asp:Repeater>
																			</td>
																			<td align="center">&nbsp;<%# ((CsGroupModel)Container.DataItem).Ex_Operators.Length %> 人&nbsp;</td>
																		</tr>
																	</ItemTemplate>
																</asp:repeater>
															</table>
														</td>
													</tr>
													<tr>
														<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
													<tr>
														<td class="action_part_bottom">&nbsp;</td>
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
	<!--△ 一覧 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
