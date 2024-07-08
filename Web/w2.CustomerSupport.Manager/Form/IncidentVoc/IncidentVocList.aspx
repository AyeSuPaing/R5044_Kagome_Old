<%--
=========================================================================================================
  Module      : VOC区分設定一覧ページ(IncidentVocList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Cs.IncidentVoc" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="IncidentVocList.aspx.cs" Inherits="Form_IncidentVoc_IncidentVocList" Title="" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td>
			<h1 class="page-title">VOC区分設定</h1>
		</td>
	</tr>
	<tr>
		<td><img height="10" width="100%" border="0" alt="" src="../../Images/Common/sp.gif" /></td>
	</tr>

	<!--▽ 一覧 ▽-->
	<tr>
		<td>
			<h2 class="cmn-hed-h2">VOC区分設定一覧</h2>
		</td>
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
											<td><img height="12" alt="" src="../../Images/Common/sp.gif" border="0" class="hide" /></td>
											<td align="center">
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
													<tr>
														<td>
															<!--▽ ページング ▽-->
															<table class="list_pager" cellspacing="0" cellpadding="0" border="0" width="758">
																<tr>
																	<td width="675"><asp:label id="lbPager1" Runat="server"></asp:label></td>
																	<td width="83" class="action_list_sp" style="height: 22px"><asp:button id="btnInsertTop" runat="server" Text="　新規登録　" onclick="btnInsert_Click" /></td>
																</tr>
															</table>
															<!--△ ページング △-->
														</td>
													</tr>
													<tr>
														<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
													<tr>
														<td>
															<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
																<tr class="list_title_bg">
																	<td align="center" width="120">VOC番号</td>
																	<td align="center" width="378">表示文言</td>
																	<td align="center" width="80">表示順</td>
																	<td align="center" width="80">有効フラグ</td>
																</tr>
																<asp:repeater id="rList" Runat="server">
																	<ItemTemplate>
																		<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# CreateVocDetailUrl(((CsIncidentVocModel)Container.DataItem).VocId) %>')">
																			<td align="center"><%# WebSanitizer.HtmlEncode(((CsIncidentVocModel)Container.DataItem).VocId) %></td>
																			<td align="left">&nbsp;<%# WebSanitizer.HtmlEncode(((CsIncidentVocModel)Container.DataItem).VocText) %></td>
																			<td align="center"><%# WebSanitizer.HtmlEncode(((CsIncidentVocModel)Container.DataItem).DisplayOrder) %></td>
																			<td align="center"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_CSINCIDENTVOC, Constants.FIELD_CSINCIDENTVOC_VALID_FLG, ((CsIncidentVocModel)Container.DataItem).ValidFlg)) %></td>
																		</tr>
																	</ItemTemplate>
																</asp:repeater>
																<tr id="trListError" class="list_alert" runat="server" Visible="False">
																	<td id="tdErrorMessage" colspan="6" runat="server"></td>
																</tr>
															</table>
														</td>
													</tr>
													<tr>
														<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
													<tr>
														<td class="action_part_bottom"><asp:Button id="btnInsertBotttom" runat="server" Text="　新規登録　" onclick="btnInsert_Click"></asp:Button></td>
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