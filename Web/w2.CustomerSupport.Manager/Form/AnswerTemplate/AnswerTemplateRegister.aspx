<%--
=========================================================================================================
  Module      : 回答例文章設定登録ページ(AnswerTemplateRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="AnswerTemplateRegister.aspx.cs" Inherits="Form_AnswerTemplate_AnswerTemplateRegister" Title="" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">回答例文章設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 登録 ▽-->
	<tr id="trEdit" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">回答例文章設定編集</h2></td>
	</tr>
	<tr id="trRegister" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">回答例文章設定登録</h2></td>
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
																<input type="button" onclick="Javascript:history.back();" value="　戻る　" />
																<asp:button id="btnConfirmTop" runat="server" Text="　確認する　" onclick="btnConfirm_Click"></asp:button>
															</div>
															<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
																<tbody>
																	<tr>
																		<td class="edit_title_bg" align="center" colspan="2">基本情報</td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">回答例カテゴリ</td>
																		<td class="edit_item_bg" align="left"><asp:DropDownList ID="ddlAnswerCategoryId" CssClass="select2-select" Width="15%" runat="server"></asp:DropDownList></td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">回答例タイトル<span class="notice">*</span></td>
																		<td class="edit_item_bg" align="left"><asp:textbox id="tbAnswerTitle" runat="server" Width="500" MaxLength="50"></asp:textbox></td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">回答例件名</td>
																		<td class="edit_item_bg" align="left"><asp:Textbox ID="tbAnswerMailTitle" runat="server" Width="500" MaxLength="50" /></td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">回答例本文<span class="notice">*</span></td>
																		<td class="edit_item_bg" align="left"><asp:textbox id="tbAnswerText" runat="server" Width="500" Height="400" TextMode="MultiLine"></asp:textbox></td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">表示順<span class="notice">*</span></td>
																		<td class="edit_item_bg" align="left"><asp:dropdownlist id="ddlDisplayOrder" runat="server"></asp:dropdownlist></td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">有効フラグ</td>
																		<td class="edit_item_bg" align="left"><asp:CheckBox id="cbValidFlg" Runat="server" Checked="true" Text="有効" /></td>
																	</tr>
																</tbody>
															</table>
														</td>
													</tr>
													<tr>
														<td>
															<br/>
															<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
																<tr class="info_item_bg">
																	<td align="left">
																		備考<br />
																		&nbsp;&nbsp;回答例本文に以下の文字列を指定した場合、回答例を差込した際に、入力しているユーザ情報に自動で置換します。<br/>
																		&nbsp;&nbsp;※入力しているユーザー情報の該当項目が空の場合には、差し込みスクリプトがそのまま出力されます。<br/><br/>
																		<table class="info_table" cellspacing="1" cellpadding="3" width="300" border="0">
																			<tr class="info_title_bg">
																				<td align="center">差し込みスクリプト</td>
																				<td align="center">表示される項目</td>
																			</tr>
																			<tr>
																				<td class="info_title_bg">@@ user_name1 @@</td>
																				<td class="info_item_bg">氏名(姓)</td>
																			</tr>
																			<tr>
																				<td class="info_title_bg">@@ user_name2 @@</td>
																				<td class="info_item_bg">氏名(名)</td>
																			</tr>
																			<tr>
																				<td class="info_title_bg">@@ user_name_kana1 @@</td>
																				<td class="info_item_bg"><%: ReplaceTag("@@User.name_kana1.name@@") %></td>
																			</tr>
																			<tr>
																				<td class="info_title_bg">@@ user_name_kana2 @@</td>
																				<td class="info_item_bg"><%: ReplaceTag("@@User.name_kana2.name@@") %></td>
																			</tr>
																			<tr>
																				<td class="info_title_bg">@@ user_mail_addr @@</td>
																				<td class="info_item_bg">メールアドレス</td>
																			</tr>
																			<tr>
																				<td class="info_title_bg">@@ user_tel1 @@</td>
																				<td class="info_item_bg">電話番号</td>
																			</tr>
																			<tr>
																				<td class="info_title_bg">@@ operator_name @@</td>
																				<td class="info_item_bg">オペレータ名</td>
																			</tr>
																			<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
																			<tr>
																				<td class="info_title_bg">@@ company_name @@</td>
																				<td class="info_item_bg"><%: ReplaceTag("@@User.company_name.name@@")%></td>
																			</tr>
																			<tr>
																				<td class="info_title_bg">@@ company_post_name @@</td>
																				<td class="info_item_bg"><%: ReplaceTag("@@User.company_post_name.name@@")%></td>
																			</tr>
																			<%} %>
																		</table>
																	</td>
																</tr>
															</table>
														</td>
													</tr>
													<tr>
														<td>
															<div class="action_part_bottom">
																<input type="button" onclick="Javascript:history.back();" value="　戻る　" />
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
