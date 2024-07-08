<%--
=========================================================================================================
  Module      : User Invoice Confirmation Screen (UserInvoiceConfirm.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Title="ユーザー電子発票情報確認" Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="UserInvoiceConfirm.aspx.cs" Inherits="Form_User_UserInvoiceConfirm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderHead" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
	<table cellpadding="0" cellspacing="0">
		<tbody>
			<tr>
				<td>
					<h2 class="cmn-hed-h2">電子発票管理情報確認</h2>
				</td>
			</tr>
			<tr>
				<td>
					<table class="box_border" cellspacing="1" cellpadding="3" border="0">
						<tbody>
							<tr>
								<td>
									<table class="info_box_bg" cellspacing="0" cellpadding="0" border="0">
										<tbody>
											<tr>
												<td>
													<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
												</td>
											</tr>
											<tr>
												<td>
													<img width="10" alt="" src="../../Images/Common/sp.gif" border="0" />
												</td>
												<td align="center">
													<table cellspacing="0" cellpadding="0" border="0">
														<tbody>
															<tr>
																<td>
																	<table class="info_table" cellpadding="3" cellspacing="1" border="0" width="658">
																		<tbody>
																			<tr class="info_item_bg">
																				<td align="left">
																					<p id="pRegistInfo" runat="server" visible="false">電子発票管理に間違いがなければ、登録する」ボタンを押してください。</p>
																					<p id="pModifyInfo" runat="server" visible="false">電子発票管理に間違いがなければ、更新する」ボタンを押してください。</p>
																				</td>
																			</tr>
																		</tbody>
																	</table>
																	<br />
																</td>
															</tr>
															<tr>
																<td>
																	<table cellspacing="1" cellpadding="3" class="detail_table" width="658">
																		<tr>
																			<td class="detail_title_bg" width="30%">
																				電子発票情報名
																			</td>
																			<td class="detail_item_bg">
																				<%: this.TwUserInvoice.TwInvoiceName %>
																			</td>
																		</tr>
																		<tr>
																			<td class="detail_title_bg">
																				發票種類
																			</td>
																			<td class="detail_item_bg">
																				<%: ValueText.GetValueText(
																				Constants.TABLE_TWUSERINVOICE,
																				Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE,
																				this.TwUserInvoice.TwUniformInvoice) %>
																			</td>
																		</tr>
																		<% if (this.TwUserInvoice.TwUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL) { %>
																		<tr>
																			<td class="detail_title_bg">
																				共通性載具
																			</td>
																			<td class="detail_item_bg">
																				<%: ValueText.GetValueText(
																				Constants.TABLE_TWUSERINVOICE,
																				Constants.FIELD_TWUSERINVOICE_TW_CARRY_TYPE,
																				this.TwUserInvoice.TwCarryType) %>
																				<br />
																				<%: string.IsNullOrEmpty(this.TwUserInvoice.TwCarryTypeOption) ? string.Empty : this.TwUserInvoice.TwCarryTypeOption %>
																			</td>
																		</tr>
																		<% } %>
																		<% if (string.IsNullOrEmpty(this.TwUserInvoice.TwUniformInvoiceOption1) == false) { %>
																		<tr>
																				<% if (this.TwUserInvoice.TwUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_COMPANY) { %>
																				<td class="detail_title_bg">
																				統一編号
																				</td>
																				<% } %>
																				<% if (this.TwUserInvoice.TwUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_DONATE) { %>
																				<td class="detail_title_bg">寄付先コード</td>
																				<% } %>
																				<td class="detail_item_bg"><%: this.TwUserInvoice.TwUniformInvoiceOption1 %></td>
																		</tr>
																		<% } %>
																		<% if (string.IsNullOrEmpty(this.TwUserInvoice.TwUniformInvoiceOption2) == false) { %>
																		<tr>
																			<td class="detail_title_bg">会社名</td>
																			<td class="detail_item_bg"><%: this.TwUserInvoice.TwUniformInvoiceOption2 %></td>
																		</tr>
																		<% } %>
																	</table>
																</td>
															</tr>
														</tbody>
													</table>
												</td>
												<td>
													<img width="10" alt="" src="../../Images/Common/sp.gif" border="0" />
												</td>
											</tr>
											<tr>
												<td colspan="2">
													<table cellspacing="0" cellpadding="5" border="0" width="100%">
														<tr><td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
														<tr>
															<td align="right">
																<asp:Button ID="btnBack" runat="server" Text="  戻る  " onclick="btnBack_Click"></asp:Button>
																<asp:Button ID="btnRegist" runat="server" Text = "  登録する  " onclick="btnSend_Click"></asp:Button>
																<asp:Button ID="btnModify" runat="server" Text = "  更新する  " onclick="btnSend_Click"></asp:Button>
															</td>
														</tr>
														<tr><td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
														<tr>
															<td align="right">
																<input id="btnClose" type="button" onclick="Javascript: close_popup();" Value="  閉じる  " />
															</td>
														</tr>
													</table>
												</td>
											</tr>
										</tbody>
									</table>
								</td>
							</tr>
						</tbody>
					</table>
				</td>
			</tr>
		</tbody>
		</table>
		<br />
		<script type="text/javascript">
			$(document).ready(function () {
				$('.action_popup_bottom').hide();
			});
		</script>
</asp:Content>

