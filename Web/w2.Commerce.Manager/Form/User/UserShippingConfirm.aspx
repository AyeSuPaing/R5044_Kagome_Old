<%--
=========================================================================================================
  Module      :ユーザアドレス確認ページ(UserShippingConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Title="ユーザー配送先情報確認" Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="UserShippingConfirm.aspx.cs" Inherits="Form_User_UserShippingConfirm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderHead" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
	<table cellpadding="0" cellspacing="0">
		<tbody>
			<tr>
				<td>
					<h2 class="cmn-hed-h2">配送先情報確認</h2>
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
																					<p id="pRegistInfo" runat="server" visible="false">登録する住所に間違いがなければ、「登録する」ボタンを押してください。</p>
																					<p id="pModifyInfo" runat="server" visible="false">編集する住所に間違いがなければ、「更新する」ボタンを押してください。</p>
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
																			<td class="detail_title_bg">
																				配送先名
																			</td>
																			<td class="detail_item_bg">
																				<%: this.UserShipping.Name %>
																			</td>
																		</tr>
																		<tr>
																			<%-- 氏名 --%>
																			<td class="detail_title_bg">
																				<%: ReplaceTag("@@User.name.name@@") %>
																			</td>
																			<td class="detail_item_bg">
																				<%: this.UserShipping.ShippingName1 %><%: this.UserShipping.ShippingName2 %>&nbsp;様
																				<% if (IsCountryJp(this.UserShipping.ShippingCountryIsoCode)) { %>
																				（<%: this.UserShipping.ShippingNameKana1 %><%: this.UserShipping.ShippingNameKana2 %>&nbsp;さま）
																				<% } %>
																			</td>
																		</tr>
																		<tr>
																			<td class="detail_title_bg">
																				住所
																			</td>
																			<td class="detail_item_bg">
																				<% if (IsCountryJp(this.UserShipping.ShippingCountryIsoCode)) { %>
																				〒<%: this.UserShipping.ShippingZip %><br />
																				<%: this.UserShipping.ShippingAddr1 %>
																				<% } %>
																				<%: this.UserShipping.ShippingAddr2 %><br />
																				<%: this.UserShipping.ShippingAddr3 %>
																				<%: this.UserShipping.ShippingAddr4 %>
																				<%: this.UserShipping.ShippingAddr5 %><br />
																				<% if (IsCountryJp(this.UserShipping.ShippingCountryIsoCode) == false) { %>
																				<%: this.UserShipping.ShippingZip %><br />
																				<% } %>
																				<%: this.UserShipping.ShippingCountryName %>
																			</td>
																		</tr>
																		<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
																		<tr>
																			<td class="detail_title_bg">
																				<%: ReplaceTag("@@User.company_name.name@@")%>・
																				<%: ReplaceTag("@@User.company_post_name.name@@")%>
																			</td>
																			<td class="detail_item_bg">
																				<%: this.UserShipping.ShippingCompanyName %><br />
																				<%: this.UserShipping.ShippingCompanyPostName %>
																			</td>
																		</tr>
																		<%} %>
																		<tr>
																			<td class="detail_title_bg">
																				<%: ReplaceTag("@@User.tel1.name@@") %>
																			</td>
																			<td class="detail_item_bg">
																				<%: this.UserShipping.ShippingTel1 %>
																			</td>
																		</tr>
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
</asp:Content>

