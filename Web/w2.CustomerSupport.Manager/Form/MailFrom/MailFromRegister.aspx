<%--
=========================================================================================================
  Module      : メール送信元設定登録ページ(MailFromRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MailFromRegister.aspx.cs" Inherits="Form_MailFrom_MailFromRegister" Title="" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">メール送信元設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 登録 ▽-->
	<tr id="trEdit" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">メール送信元設定編集</h2></td>
	</tr>
	<tr id="trRegister" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">メール送信元設定登録</h2></td>
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
																		<td class="edit_title_bg" align="left" width="30%">表示名<span class="notice"></span></td>
																		<td class="edit_item_bg" align="left"><asp:textbox id="tbFromDisplayName" runat="server" Width="300" MaxLength="256"></asp:textbox></td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">メールアドレス<span class="notice">*</span></td>
																		<td class="edit_item_bg" align="left"><asp:textbox id="tbFromAddress" runat="server" Width="300" MaxLength="256"></asp:textbox></td>
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
</asp:Content>