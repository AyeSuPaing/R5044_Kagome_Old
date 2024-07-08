<%--
=========================================================================================================
  Module      : 受信時振分けルール設定確認ページ(MailAssignSettingConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MailAssignSettingConfirm.aspx.cs" Inherits="Form_MailAssignSetting_MailAssignSettingConfirm" %>
<%@ Import Namespace="w2.App.Common.MailAssignSetting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">受信時振分けルール設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 詳細 ▽-->
	<tr id="trDetail" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">受信時振分けルール設定詳細</h2></td>
	</tr>
	<tr id="trConfirm" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">受信時振分けルール設定確認</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="0" cellpadding="3" width="784" border="0">
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
														<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
													<tr>
														<td>
															<div class="action_part_top">
																<asp:button id="btnBackTop" runat="server" Text="　戻る　" onclick="btnBack_Click" ></asp:button>
																<asp:button id="btnEditTop" runat="server" Text="　編集する　" Visible="False" onclick="btnEdit_Click"></asp:button>
																<asp:button id="btnDeleteTop" runat="server" Text="　削除する　" Visible="False" onclick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');"></asp:button>
																<asp:button id="btnInsertTop" runat="server" Text="　登録する　" Visible="False" onclick="btnInsert_Click"></asp:button>
																<asp:button id="btnUpdateTop" runat="server" Text="　更新する　" Visible="False" Width="72px" onclick="btnUpdate_Click"></asp:button>
															</div>
															<!-- ▼▼▼ 条件指定 ▼▼▼ -->
															<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
																<tbody>
																	<tr>
																		<td class="edit_title_bg" align="center" colspan="2">実行条件</td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">優先順</td>
																		<td class="edit_item_bg" align="left">
																			<% if (this.AssignSetting.EX_IsMatchOnBind || this.AssignSetting.EX_IsMatchAnything) { %><b><% } %>
																			<%: this.AssignSetting.AssignPriority %>
																			<% if (this.AssignSetting.EX_IsMatchOnBind || this.AssignSetting.EX_IsMatchAnything) { %></b><% } %>
																		</td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">振分け設定名</td>
																		<td class="edit_item_bg" align="left">
																			<%: this.AssignSetting.MailAssignName %>
																		</td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">振分け条件</td>
																		<td class="edit_item_bg" align="left">
																			<% for (int i = 0; i < this.AssignSetting.Items.Length; i++) { %>
																				<% if (this.AssignSetting.EX_IsMatchOnBind) { %><b>既存インシデントに紐付く受信メール [固定]</b><% continue;} %>
																				<% if (this.AssignSetting.EX_IsMatchAnything) { %><b>全ての受信メール [固定]</b><% continue;} %>
																				<% if ((i == 0) && (this.AssignSetting.Items.Length > 1)) { %> <%: "▼" + this.AssignSetting.EX_LogicalOperationName%><br /><br /> <% } %>
																				<% if (i > 0) { %><%: this.AssignSetting.EX_LogicalOperationName_Short %><br /><% } %>
																				<% if (this.AssignSetting.Items.Length>1) { %><% } %>
																				&lt;<%: this.AssignSetting.Items[i].EX_AssignItemMatchingTargetName %>&gt; が
																				[<%: this.AssignSetting.Items[i].MatchingValue %>] <%: this.AssignSetting.Items[i].EX_MatchingTypeName %>
																				(<%: this.AssignSetting.Items[i].EX_IgnoreCaseName %>)<br />
																			<% } %>
																		</td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">振分け停止</td>
																		<td class="edit_item_bg" align="left"><%: this.AssignSetting.EX_StopFiltering %></td>
																	</tr>
																</tbody>
															</table>
															<!-- ▲▲▲ 条件指定 ▲▲▲ -->
															<br />
															<br />
															<!-- ▼▼▼ 振分けルール ▼▼▼ -->
															<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
																<tbody>
																	<tr>
																		<td class="edit_title_bg" align="center" colspan="2">振分けアクション</td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">ステータス</td>
																		<td class="edit_item_bg" align="left"><%: this.AssignSetting.EX_AssignStatusName %></td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">カテゴリ</td>
																		<td class="edit_item_bg" align="left"><%: this.AssignSetting.EX_AssignIncidentCategoryName %></td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">重要度</td>
																		<td class="edit_item_bg" align="left"><%: this.AssignSetting.EX_AssignImportanceName %></td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%" rowspan="2">担当</td>
																		<td class="edit_item_bg" align="left">グループ　 ： <%: this.AssignSetting.EX_AssignCsGroupName %></td>
																	</tr>
																	<tr>
																		<td class="edit_item_bg" align="left">オペレータ ： <%: this.AssignSetting.EX_AssignOperatorName %></td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">メッセージをゴミ箱へ移動する</td>
																		<td class="edit_item_bg" align="left"><%: this.AssignSetting.EX_TrashName %></td>
																	</tr>
																</tbody>
															</table>
															<!-- ▲▲▲ 振分けルール ▲▲▲ -->
															<br />
															<br />
															<!-- ▼▼▼ オートレスポンス ▼▼▼ -->
															<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
																<tbody>
																	<tr>
																		<td class="edit_title_bg" align="center" colspan="2">オートレスポンス</td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">オートレスポンスメール</td>
																		<td class="edit_item_bg" align="left">
																			<%: this.AssignSetting.EX_AutoResponseName %>
																		</td>
																	</tr>
																	<% if (this.AssignSetting.AutoResponse == Constants.FLG_CSMAILASSIGNSETTING_AUTO_RESPONSE_VALID) { %>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">差出人From</td>
																		<td class="edit_item_bg" align="left">
																			<%: this.AssignSetting.AutoResponseFrom %>
																		</td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">送信先Cc</td>
																		<td class="edit_item_bg" align="left">
																			<%: this.AssignSetting.AutoResponseCc %>
																		</td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">送信先Bcc</td>
																		<td class="edit_item_bg" align="left">
																			<%: this.AssignSetting.AutoResponseBcc %>
																		</td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">件名</td>
																		<td class="edit_item_bg" align="left">
																			<%: this.AssignSetting.AutoResponseSubject %>
																		</td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">本文</td>
																		<td class="edit_item_bg" align="left">
																			<%= WebSanitizer.HtmlEncodeChangeToBr(this.AssignSetting.AutoResponseBody) %>
																		</td>
																	</tr>
																	<% } %>
																</tbody>
															</table>
															<!-- ▲▲▲ オートレスポンス ▲▲▲ -->
															<div class="action_part_bottom">
																<asp:button id="btnBackBottom" runat="server" Text="　戻る　" onclick="btnBack_Click"></asp:button>
																<asp:button id="btnEditBottom" runat="server" Text="　編集する　" Visible="False" onclick="btnEdit_Click"></asp:button>
																<asp:button id="btnDeleteBottom" runat="server" Text="　削除する　" Visible="False" onclick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');"></asp:button>
																<asp:button id="btnInsertBottom" runat="server" Text="　登録する　" Visible="False" onclick="btnInsert_Click"></asp:button>
																<asp:button id="btnUpdateBottom" runat="server" Text="　更新する　" Visible="False" Width="72px" onclick="btnUpdate_Click"></asp:button>
															</div>
														</td>
													</tr>
													<tr>
														<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
	<!--△ 詳細 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
