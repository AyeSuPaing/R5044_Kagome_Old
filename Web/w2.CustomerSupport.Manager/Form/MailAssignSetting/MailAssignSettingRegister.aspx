<%--
=========================================================================================================
  Module      : 受信時振分けルール設定登録ページ(MailAssignSettingRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MailAssignSettingRegister.aspx.cs" Inherits="Form_MailAssignSetting_MailAssignSettingRegister" MaintainScrollPositionOnPostback="true" %>
<%@ Import Namespace="w2.App.Common.MailAssignSetting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">受信時振分けルール設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 登録 ▽-->
	<tr id="trEdit" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">受信時振分けルール設定編集</h2></td>
	</tr>
	<tr id="trRegister" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">受信時振分けルール設定登録</h2></td>
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
																<asp:button id="btnbackTop" runat="server" Text="　戻る　" onclick="btnbackTop_Click" />
																<asp:button id="btnConfirmTop" runat="server" Text="　確認する　" onclick="btnConfirm_Click"></asp:button>
															</div>
															<!-- ▼▼▼ 条件指定 ▼▼▼ -->
															<asp:UpdatePanel ID="upCondition" runat="server">
															<ContentTemplate>
															<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
																<tbody>
																	<!-- ▼▼▼ 実行条件エラー表示 ▼▼▼ -->
																	<tr id="trMailAssignErrorMessagesTitle" runat="server" visible="false">
																		<td class="edit_title_bg" align="center" colspan="2">エラーメッセージ</td>
																	</tr>
																	<tr id="trMailAssignErrorMessages" runat="server" visible="false">
																		<td class="edit_item_bg" align="left" colspan="2">
																			<asp:Label ID="lbMailAssignErrorMessages" runat="server" ForeColor="red" />
																		</td>
																	</tr>
																	<!-- ▲▲▲ 実行条件エラー表示 ▲▲▲ -->
																	<tr>
																		<td class="edit_title_bg" align="center" colspan="2">実行条件</td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="15%">優先順<span class="notice">*</span></td>
																		<td class="edit_item_bg" align="left"><asp:TextBox runat="server" ID="tbAssignPriority" MaxLength="3" /> 1～999の範囲で指定</td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="15%">振分け設定名<span class="notice">*</span></td>
																		<td class="edit_item_bg" align="left"><asp:TextBox runat="server" ID="tbMailAssignName" Width="280" MaxLength="256" /></td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="15%">振分け条件<span class="notice">*</span></td>
																		<td class="edit_item_bg" align="left" ID="conditionNormal" runat="server">
																			<asp:RadioButtonList ID="rblLogicalOperation" RepeatDirection="Horizontal" RepeatLayout="Flow" CellSpacing=0 CellPadding=0 style="margin-left: -7px;" runat="server" OnSelectedIndexChanged="rblLogicalOperation_Click" AutoPostBack="true" />
																			<br />
																			<asp:Repeater ID="rConditionList" runat="server">
																			<ItemTemplate>
																				<asp:DropDownList ID="ddlAssignItem" DataSource="<%# ((MailAssignSettingItem)Container.DataItem).AssignItemList %>" SelectedValue="<%# ((MailAssignSettingItem)Container.DataItem).SelectedMatchingTarget.Value %>" DataTextField="text" DataValueField="value" runat="server" />
																				が
																				<asp:TextBox ID="tbKeyword" Text='<%# ((MailAssignSettingItem)Container.DataItem).ConditionValue %>' Width="170" MaxLength="100" runat="server" />
																				<asp:DropDownList ID="ddlAssignItemCondition" DataSource="<%# ((MailAssignSettingItem)Container.DataItem).IncludeConditionList %>" SelectedValue="<%# ((MailAssignSettingItem)Container.DataItem).SelectedIncludeCondition.Value %>"  DataTextField="text" DataValueField="value" runat="server" />
																				<asp:CheckBox ID="cbCaseSensitive" Text="英字の大文字小文字を区別する" Checked='<%# ((MailAssignSettingItem)Container.DataItem).CaseSensitive %>' runat="server" />
																				<asp:Button ID="btnAdd" Text="  追加  " runat="server" OnClick='btnAddDel_Click' CommandName="Add" CommandArgument="<%# Container.ItemIndex %>" />
																				<asp:Button ID="btnDel" Text="  削除  " runat="server" OnClick='btnAddDel_Click' CommandName="Del" CommandArgument="<%# Container.ItemIndex %>" Visible="<%# (Container.ItemIndex != 0) || (((IEnumerable)rConditionList.DataSource).Cast<object>().Count() > 1) %>" /><br />
																			</ItemTemplate>
																			<SeparatorTemplate><%# rblLogicalOperation.SelectedValue.Replace("AND", "かつ").Replace("OR", "または") %><br /></SeparatorTemplate>
																			</asp:Repeater>
																		</td>
																		<td class="edit_item_bg" align="left" ID="conditionBind" Visible="false" runat="server">
																			<b>既存インシデントに紐付く受信メール [固定]</b>
																		</td>
																		<td class="edit_item_bg" align="left" ID="conditionNoMatch" Visible="false" runat="server">
																			<b>全ての受信メール [固定]</b>
																		</td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="15%">振分け停止</td>
																		<td class="edit_item_bg" align="left"><asp:CheckBox runat="server" ID="cbStopFiltering" Text="このルールが適用されたら振分けを停止する"/></td>
																	</tr>
																</tbody>
															</table>
															</ContentTemplate>
															</asp:UpdatePanel>
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
																		<td class="edit_item_bg" align="left"><asp:DropDownList runat="server" ID="ddlAssignStatus"/></td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">カテゴリ</td>
																		<td class="edit_item_bg" align="left"><asp:DropDownList runat="server" ID="ddlIncidentCateory" CssClass="select2-select" Width="50%" /></td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">重要度</td>
																		<td class="edit_item_bg" align="left"><asp:DropDownList runat="server" ID="ddlAssignImportance"/></td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%" rowspan="2">担当</td>
																		<td class="edit_item_bg" align="left">グループ　 ： <asp:DropDownList runat="server" ID="ddlAssignCsGroup" CssClass="select2-select" AutoPostBack="true" OnSelectedIndexChanged="ddlAssignCsGroup_SelectedIndexChanged" Width="20%"/></td>
																	</tr>
																	<tr>
																		<td class="edit_item_bg" align="left">オペレータ ： <asp:DropDownList runat="server" CssClass="select2-select" ID="ddlAssignOperator" Width="30%"/></td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">ゴミ箱</td>
																		<td class="edit_item_bg" align="left"><asp:CheckBox runat="server" ID="cbTrash" Text="ゴミ箱へ移動する"/></td>
																	</tr>
																</tbody>
															</table>
															<!-- ▲▲▲ 振分けルール ▲▲▲ -->
															<br />
															<br />
															<!-- ▼▼▼ オートレスポンス ▼▼▼ -->
															<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
																<tbody>
																	<!-- ▼▼▼ オートレスポンスエラー表示 ▼▼▼ -->
																	<tr id="trAutoResponseErrorMessagesTitle" runat="server" visible="false">
																		<td class="edit_title_bg" align="center" colspan="2">エラーメッセージ</td>
																	</tr>
																	<tr id="trAutoResponseErrorMessages" runat="server" visible="false">
																		<td class="edit_item_bg" align="left" colspan="2">
																			<asp:Label ID="lbAutoResponseErrorMessages" runat="server" ForeColor="red" />
																		</td>
																	</tr>
																	<!-- ▲▲▲ オートレスポンスエラー表示 ▲▲▲ -->
																	<tr>
																		<td class="edit_title_bg" align="center" colspan="2">オートレスポンス</td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">オートレスポンスメール</td>
																		<td class="edit_item_bg" align="left">
																			<asp:CheckBox runat="server" ID="cbAutoResponse" Text="送信する" OnCheckedChanged="cbAutoResponse_CheckedChanged" AutoPostBack="True" />
																		</td>
																	</tr>
																	<tr runat="server" id="trAutoResponseFrom" Visible="false">
																		<td class="edit_title_bg" align="left" width="30%">差出人From<span class="notice">*</span></td>
																		<td class="edit_item_bg" align="left"><asp:TextBox runat="server" ID="tbAutoResponseFrom" Width="400px" MaxLength="256" /></td>
																	</tr>
																	<tr runat="server" id="trAutoResponseCc" Visible="false">
																		<td class="edit_title_bg" align="left" width="30%">送信先Cc</td>
																		<td class="edit_item_bg" align="left"><asp:TextBox runat="server" ID="tbAutoResponseCc" Width="400px" /></td>
																	</tr>
																	<tr runat="server" id="trAutoResponseBcc" Visible="false">
																		<td class="edit_title_bg" align="left" width="30%">送信先Bcc</td>
																		<td class="edit_item_bg" align="left"><asp:TextBox runat="server" ID="tbAutoResponseBcc" Width="400px" /></td>
																	</tr>
																	<tr runat="server" id="trAutoResponseSubject" Visible="false">
																		<td class="edit_title_bg" align="left" width="30%">件名<span class="notice">*</span></td>
																		<td class="edit_item_bg" align="left"><asp:TextBox runat="server" ID="tbAutoResponseSubject" Width="400px" MaxLength="256" /></td>
																	</tr>
																	<tr runat="server" id="trAutoResponseBody" Visible="false">
																		<td class="edit_title_bg" align="left" width="30%">本文<span class="notice">*</span></td>
																		<td class="edit_item_bg" align="left"><asp:TextBox runat="server" ID="tbAutoResponseBody" TextMode="MultiLine" Width="517px" Height="216px" /></td>
																	</tr>
																</tbody>
															</table>
															<!-- ▲▲▲ オートレスポンス ▲▲▲ -->
															<div class="action_part_bottom">
																<asp:button id="btnbBackBottom" runat="server" Text="　戻る　" onclick="btnbackTop_Click" />
																<asp:button id="btnConfirmBottom" runat="server" Text="　確認する　" onclick="btnConfirm_Click"></asp:button>
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
	<!--△ 登録 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
	<script type="text/javascript">
		$(function () {
			ddlSelect();
		});
	</script>
</asp:Content>
