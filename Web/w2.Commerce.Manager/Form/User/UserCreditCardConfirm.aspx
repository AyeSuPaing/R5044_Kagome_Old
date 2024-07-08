<%--
=========================================================================================================
  Module      : ユーザクレジットカード確認画面(UserCreditCardConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true"
	CodeFile="~/Form/User/UserCreditCardConfirm.aspx.cs" Inherits="Form_User_UserCreditCardConfirm"
	Title="ユーザークレジットカード確認" %>

<%@ Import Namespace="w2.App.Common.Order" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<table cellspacing="0" cellpadding="0" width="791" border="0">
		<tr>
			<td>
				<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
			</td>
		</tr>
		<tr>
			<td>
				<h2 class="cmn-hed-h2">
					<%if (this.NeedsRegisterProvisionalCreditCardCardKbnExceptZeus && (Request[Constants.REQUEST_KEY_CREDITCARD_NO] == null)) {%>仮<%} %>クレジットカード情報確認
				</h2>
			</td>
		</tr>
		<tr>
			<td>
				<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
					<tr>
						<td>
							<table class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td align="center">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td>
													<img height="15" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
												</td>
											</tr>
											<tr>
												<td>
													<%-- メッセージ --%>
														<div class="dvUserCreditCardInfo">
															<table class="detail_table" width="758" border="0" cellspacing="1" cellpadding="3">
																<%if (this.NeedsRegisterProvisionalCreditCardCardKbnExceptZeus == false) {%>
																<%if (OrderCommon.CreditCompanySelectable) {%>
																<tr>
																	<td align="left" class="edit_title_bg" width="20%">
																		カード会社
																	</td>
																	<td class="edit_item_bg">
																		<asp:Literal ID="lCreditCardCompanyName" runat="server"></asp:Literal>
																	</td>
																</tr>
																<%} %>
																<tr id="trCreditCardNumber" runat="server">
																	<td align="left" class="edit_title_bg" width="20%">
																		<%if (this.CreditTokenizedPanUse) {%>永久トークン<%}else{%>カード番号<%} %>
																	</td>
																	<td class="edit_item_bg">
																		<asp:Literal ID="lDispCreditCardNo" runat="server"></asp:Literal>
																	</td>
																</tr>
																<tr id="trlExpiration" runat="server">
																	<td align="left" class="edit_title_bg" width="20%">
																		有効期限
																	</td>
																	<td class="edit_item_bg">
																		<asp:Literal ID="lExpirationMonth" runat="server"></asp:Literal>/<asp:Literal ID="lExpirationYear" runat="server"></asp:Literal> (月/年)
																	</td>
																</tr>
																<tr id ="trCreditAuthorName" runat="server">
																	<td align="left" class="edit_title_bg">
																		カード名義人
																	</td>
																	<td class="edit_item_bg">
																		<asp:Literal ID="lAutherName" runat="server"></asp:Literal>
																	</td>
																</tr>
																<%} %>
																<tr>
																	<td align="left" class="edit_title_bg" width="20%">
																		クレジットカード登録名
																	</td>
																	<td class="edit_item_bg">
																		<asp:Literal ID="lCardDispName" runat="server"></asp:Literal>
																	</td>
																</tr>
															</table>
														</div>
														<br />
														<div class="dvUserBtnBox" style="text-align: right;">
															<asp:Button ID="btnHistoryBackTop" Text="戻る" runat="server" OnClick="btnBuckHistoryBackTop_OnClick" />
															<asp:Button ID="btnSend" Text="  登録する  " ValidationGroup="UserCreditCardRegist" Visible="False" runat="server" OnClick="btnSend_Click"></asp:Button>
															<asp:Button ID="btnUpdate" Text="  更新する  " ValidationGroup="UserCreditCardUpdate" Visible="False" runat="server" OnClick="btnUpdate_Click"></asp:Button>
														</div>
													<%--△ ユーザ情報表示 △--%>
												</td>
											</tr>
											<tr>
												<td>
													<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
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
		<tr>
			<td>
				<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
			</td>
		</tr>
	</table>
</asp:Content>
