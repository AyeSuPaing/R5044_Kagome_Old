<%--
=========================================================================================================
  Module      : メニュー権限設定確認ページ(MenuAuthorityConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MenuAuthorityConfirm.aspx.cs" Inherits="Form_MenuAuthority_MenuAuthorityConfirm" %>
<%@ MasterType VirtualPath="~/Form/Common/DefaultPage.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr>
		<td><h1 class="page-title">メニュー権限設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ タイトル △-->
	<!--▽ 確認 ▽-->
	<tr>
		<td>
			<h2 class="cmn-hed-h2">
				<% if (Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_DETAIL){%>
					メニュー権限設定詳細
				<%} %>
				<% if (Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_INSERT){%>
					メニュー権限設定入力確認
				<%} %>
				<% if (Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_UPDATE){%>
					メニュー権限設定入力確認
				<%} %>
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
												<div class="action_part_top"><input onclick="Javascript:history.back()" type="button" value="  戻る  " />
												<asp:Button id="btnEditTop" runat="server" Visible="False" Text="  編集する  " OnClick="btnEdit_Click" />
												<asp:Button id="btnDeleteTop" runat="server" Visible="False" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
												<asp:Button id="btnUpdateTop" runat="server" Visible="False" Text="  更新する  " OnClick="btnUpdate_Click" />
												<asp:Button id="btnInsertTop" runat="server" Visible="False" Text="  登録する  " OnClick="btnInsert_Click" /></div>
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" width="30%">メニュー権限名
														</td>
														<td class="detail_item_bg" align="left" width="70%">
															<asp:Label id="lMenuAuthorityName" Runat="server"></asp:Label></td>
													</tr>
												</table>
												<br />
												<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<asp:Repeater id="rLargeMenu" ItemType="w2.App.Common.Manager.Menu.MenuLarge" Runat="server">
														<ItemTemplate>
															<tr>
																<td align="left" class="detail_title_bg" colspan="3">
																	<%#: Item.Name %>
																</td>
															</tr>
															<asp:Repeater id="rSmallMenu" Runat="server" DataSource='<%# Item.SmallMenus %>' ItemType="w2.App.Common.Manager.Menu.MenuSmall">
																<ItemTemplate>
																	<tr>
																		<td align="center" class="detail_title_bg" width="10%">
																			<span style="font-size:10px" visible="<%# Item.IsAuthorityDefaultDispPage %>" runat="server">
																				デフォルト表示
																			</span>
																		</td>
																		<td align="left" class="detail_item_bg" width="20%">
																			<%#: Item.Name %></td>
																		<td align="left" class="detail_item_bg" width="70%">
																			<asp:Repeater ID="rFunction" runat="server" DataSource='<%# Item.Functions %>' ItemType="w2.App.Common.Manager.Menu.MenuFunction">
																				<ItemTemplate>
																					[<%#: Item.Name%>]
																				</ItemTemplate>
																				<SeparatorTemplate>
																				&nbsp;
																				</SeparatorTemplate>
																			</asp:Repeater>
																		</td>
																	</tr>
																</ItemTemplate>
															</asp:Repeater>
														</ItemTemplate>
													</asp:Repeater>
												</table>
												<div class="action_part_bottom"><input onclick="Javascript:history.back()" type="button" value="  戻る  " />
													<asp:Button id="btnEditBottom" runat="server" Visible="False" Text="  編集する  " OnClick="btnEdit_Click" />
													<asp:Button ID="btnDeleteBottom" Runat="server" visible="False" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
													<asp:Button id="btnUpdateBottom" runat="server" Visible="False" Text="  更新する  " OnClick="btnUpdate_Click" />
													<asp:Button id="btnInsertBottom" runat="server" Visible="False" Text="  登録する  " OnClick="btnInsert_Click" />
												</div>
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
	<!--△ 確認 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
