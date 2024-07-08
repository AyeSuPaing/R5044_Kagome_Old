<%--
=========================================================================================================
  Module      : User Invoice Input Screen (UserInvoiceInput.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Title="ユーザー電子発票情報入力" Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="UserInvoiceInput.aspx.cs" Inherits="Form_User_UserInvoiceInput" %>

<%@ Import Namespace="Braintree" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderHead" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<%-- UPDATE PANEL開始 --%>
	<table cellpadding="0" cellspacing="0">
		<tr>
			<td>
				<h2 class="cmn-hed-h2">電子発票管理情報編集</h2>
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
												<img width="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
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
																				<p id="pRegistInfo" runat="server" visible="false">
																					電子発票管理に新しい電子発票情管理情報を登録します。<br />下のフォームに入力し、「確認する」ボタンを押してください。
																					電子発票情報名には、「任意な名称」を登録<br />する事ができます。（例：「会社用」、「自分ショッピング用 」など）
																				</p>
																				<p id="pModifyInfo" runat="server" visible="false">
																					電子発票管理に登録されているお届け先を編集します。<br />下のフォームに入力し、「確認する」ボタンを押してください。
																				</p>
																			</td>
																		</tr>
																	</tbody>
																</table>
																<br />
															</td>
														</tr>
														<tr>
															<td>
																<asp:UpdatePanel ID="upUpdatePanel" runat="server">
																	<ContentTemplate>
																		<div id="divErrorMessage" runat="server" visible="false">
																			<table class="info_table" cellpadding="3" width="658" border="0" cellspacing="1">
																				<tr class="info_item_bg">
																					<td>
																						<asp:Label ID="lbErrorMessage" runat="server" ForeColor="Red"></asp:Label></td>
																				</tr>
																			</table>
																			<br />
																		</div>
																		<table cellspacing="1" cellpadding="3" width="658" border="0" class="edit_table">
																			<tr>
																				<td class="edit_title_bg" width="30%">電子発票情報名<span class="notice">*</span>
																				</td>
																				<td class="edit_item_bg">
																					<asp:TextBox ID="tbInvoiceName" runat="server" MaxLength="30"></asp:TextBox>
																				</td>
																			</tr>
																			<tr>
																				<td class="edit_title_bg">發票種類<span class="notice">*</span>
																				</td>
																				<td class="edit_item_bg">
																					<asp:DropDownList ID="ddlTwUniformInvoice" OnSelectedIndexChanged="ddlTwUniformInvoice_SelectedIndexChanged" AutoPostBack="true" runat="server" DataTextField="Text" DataValueField="Value">
																					</asp:DropDownList>
																				</td>
																			</tr>
																			<tr id="trCarryType" runat="server" visible="false">
																				<td class="edit_title_bg">共通性載具<span class="notice">*</span>
																				</td>
																				<td class="edit_item_bg">
																					<asp:DropDownList ID="ddlTwCarryType" OnSelectedIndexChanged="ddlTwCarryType_SelectedIndexChanged" AutoPostBack="true" runat="server" DataTextField="Text" DataValueField="Value">
																					</asp:DropDownList>
																					<div id="divCarryTypeOption" runat="server" visible="false">
																						<br />
																						<div id="divTwCarryTypeOption_8" runat="server" visible="false">
																							<asp:TextBox ID="tbCarryTypeOption_8" Width="220" placeholder="例:/AB201+9(限8個字)" runat="server" MaxLength="8"></asp:TextBox>
																						</div>
																						<div id="divTwCarryTypeOption_16" runat="server" visible="false">
																							<asp:TextBox ID="tbCarryTypeOption_16" Width="220" placeholder="例:TP03000001234567(限16個字)" runat="server" MaxLength="16"></asp:TextBox>
																						</div>
																					</div>
																				</td>
																			</tr>
																			<tr id="trUniformInvoiceOption1" runat="server" visible="false">
																				<td class="edit_title_bg" id="companyTittle" runat="server" visible="false">統一編号<span class="notice">*</span></td>
																				<td class="edit_title_bg" id="donateTittle" runat="server" visible="false">寄付先コード<span class="notice">*</span></td>
																				<td class="edit_item_bg">
																					<asp:TextBox ID="tbUniformInvoiceOption1_3" runat="server" MaxLength="7" Visible="false"></asp:TextBox>
																					<asp:TextBox ID="tbUniformInvoiceOption1_8" placeholder="例:12345678" runat="server" MaxLength="8" Visible="false"></asp:TextBox>
																				</td>
																			</tr>
																			<tr id="trUniformInvoiceOption2" runat="server" visible="false">
																				<td class="edit_title_bg">会社名<span class="notice">*</span>
																				</td>
																				<td class="edit_item_bg">
																					<asp:TextBox ID="tbUniformInvoiceOption2" placeholder="例:○○有限股份公司" runat="server" MaxLength="20"></asp:TextBox>
																				</td>
																			</tr>
																		</table>
																		<table cellspacing="0" cellpadding="5" width="658" border="0">
																			<tr>
																				<td>
																					<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
																			</tr>
																			<tr>
																				<td align="right">
																					<asp:Button ID="btnReset" runat="server" Text="  リセット  " OnClick="btnReset_Click" />
																					<asp:Button ID="btnComfirm" runat="server" Text="  確認する  " OnClick="btnComfirm_Click" />
																				</td>
																			</tr>
																			<tr>
																				<td>
																					<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
																			</tr>
																			<tr>
																				<td align="right">
																					<input id="btnClose" type="button" onclick="Javascript: close_popup();" Value="  閉じる  " />
																				</td>
																			</tr>
																		</table>
																	</ContentTemplate>
																</asp:UpdatePanel>
															</td>
														</tr>
													</tbody>
												</table>
											</td>
											<td>
												<img width="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
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
	</table>
	<%-- UPDATE PANELここまで --%>
	<br />
	<script type="text/javascript">
		$(document).ready(function () {
			$('.action_popup_bottom').hide();
		});
	</script>
</asp:Content>
