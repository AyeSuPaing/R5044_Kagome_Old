<%--
=========================================================================================================
  Module      : 外部リンクリストページ(ExternalLinkPreferenceList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.Domain.ExternalLink" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ExternalLinkPreferenceList.aspx.cs" Inherits="Form_ExternalLinkPerference_ExternalLinkPreferecneList" Title="" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<table cellspacing="0" cellpadding="0" width="791" border="0">
		<tr>
			<td><h1 class="page-title">外部リンク設定</h1></td>
		</tr>
		<tr>
			<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
		</tr>
		<!--▽ 検索 ▽-->
		<tr>
			<td>
				<table class="box_border" cellspacing="0" cellpadding="3" width="784" border="0">
					<tr>
						<td>
							<table cellspacing="1" cellpadding="0" width="100%" border="0">
								<tr>
									<td class="search_box_bg">
										<table cellspacing="0" cellpadding="0" width="100%" border="0">
											<tr>
												<td align="center">
													<tr>
														<td>
															<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
													<table cellspacing="0" cellpadding="0" border="0">
														<tr>
															<td class="search_box">
																<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
																	<tr>
																		<td class="search_title_bg" width="120">
																			<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" border="0" />リンク名称</td>
																		<td class="search_item_bg"><asp:textbox id="tbExternalLinkTitle" runat="server" Width="160"/></td>
																		<td class="search_title_bg" width="120">
																			<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" border="0" />有効フラグ</td>
																		<td class="search_item_bg">
																			<asp:DropDownList ID="ddlValidFlg" runat="server"></asp:DropDownList></td>
																		<td class="search_btn_bg" width="83" rowspan="2">
																			<div class="search_btn_main">
																				<asp:Button ID="btnSearch" runat="server" Text="　検索　" OnClick="btnSearch_Click"></asp:Button>
																			</div>
																			<div class="search_btn_sub"><a href="<%= Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_EXTERNALLINKPREFERENCE_LIST %>">クリア</a></div>
																		</td>
																	</tr>
																</table>
															</td>
														</tr>
														<tr>
															<td>
																<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
		<!--△ 検索 △-->
		<tr>
			<td>
				<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
		</tr>
		<!--▽ 一覧 ▽-->
		<tr>
			<td>
				<h2 class="cmn-hed-h2">外部リンク設定一覧</h2>
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
												<td align="center">
													<table cellspacing="0" cellpadding="0" border="0">
														<tr>
															<td>
																<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
														</tr>
														<tr>
															<td>
																<!--▽ ページング ▽-->
																<table class="list_pager" cellspacing="0" cellpadding="0" border="0" width="758">
																	<tr>
																		<td width="675">
																			<asp:Label ID="lbPager1" runat="server"></asp:Label></td>
																		<td width="83" class="action_list_sp" style="height: 22px">
																			<asp:Button ID="btnInsertTop" runat="server" Text="　新規登録　" OnClick="btnInsert_Click" /></td>
																	</tr>
																</table>
																<!--△ ページング △-->
															</td>
														</tr>
														<tr>
															<td>
																<img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
														</tr>
														<tr>
															<td>
																<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
																	<tr class="list_title_bg">
																		<td align="center" width="150">リンク名称</td>
																		<td align="center" width="448">URL</td>
																		<td align="center" width="80">表示順</td>
																		<td align="center" width="80">有効フラグ</td>
																	</tr>
																	<asp:Repeater ID="rList" runat="server">
																		<ItemTemplate>
																			<tr class="list_item_bg<%# (Container.ItemIndex % 2) + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# (Container.ItemIndex % 2) + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# CreateDetailUrl((((CsExternalLinkModel)Container.DataItem).LinkId)) %>')">
																				<td align="center"><%# WebSanitizer.HtmlEncode(AbbreviateString(((CsExternalLinkModel)Container.DataItem).LinkTitle,15)).TrimStart() %></td>
																				<td align="left" title="<%# WebSanitizer.HtmlEncode(((CsExternalLinkModel)Container.DataItem).LinkUrl).TrimStart() %>">
																					&nbsp; <%# WebSanitizer.HtmlEncode(AbbreviateString(((CsExternalLinkModel)Container.DataItem).LinkUrl,110)).TrimStart() %>
																				</td>
																				<td align="center"><%# WebSanitizer.HtmlEncode(((CsExternalLinkModel)Container.DataItem).DisplayOrder) %></td>
																				<td align="center"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_CSEXTERNALLINK, Constants.FIELD_CSEXTERNALLINK_VALID_FLG, ((CsExternalLinkModel)Container.DataItem).ValidFlg)) %></td>
																			</tr>
																		</ItemTemplate>
																	</asp:Repeater>
																	<tr id="trListError" class="list_alert" runat="server" visible="False">
																		<td id="tdErrorMessage" colspan="6" runat="server"></td>
																	</tr>
																</table>
															</td>
														</tr>
														<tr>
															<td>
																<img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
														</tr>
														<tr>
															<td class="action_part_bottom">
																<asp:Button ID="btnInsertBotttom" runat="server" Text="　新規登録　" OnClick="btnInsert_Click"></asp:Button></td>
														</tr>
														<tr>
															<td>
																<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
	</table>

</asp:Content>