<%--
=========================================================================================================
  Module      : CSオペレータ所属グループ登録ページ(CsOperatorGroupRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="CsOperatorGroupRegister.aspx.cs" Inherits="Form_CsOperatorGroup_CsOperatorGroupRegister" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
	<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">オペレータ所属設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧・詳細 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">オペレータ所属設定編集</h2></td>
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
												<div class="action_part_top" align="right">
													<asp:button id="btnBackTop" runat="server" Text="　戻る　" onclick="btnBack_Click" ></asp:button>
													<asp:button id="btnUpdateTop" runat="server" Text="　更新する　" onclick="btnUpdate_Click"></asp:button>
												</div>
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" width="30%">グループ名</td>
														<td class="detail_item_bg" align="left" width="70%"><asp:Label ID="lbGroupName" runat="server"></asp:Label></td>
													</tr>
												</table>
												<br />
												<table class="info_box_bg" cellspacing="0" cellpadding="0" width="758" border="0">
													<tr>
														<td valign="top" align="center">
															<table cellspacing="0" cellpadding="0" border="0">
																<tr>
																	<td>
																		<table class="edit_table" cellspacing="1" cellpadding="3" width="" border="0">
																			<tr>
																				<td class="edit_title_bg" align="left" width="120">オペレータ</td>
																				<td class="edit_item_bg" align="left">
																					<asp:ListBox id="lbUnassignedOperatorList" runat="server" SelectionMode="Multiple"  Width="240" Height="480" />
																				</td>
																			</tr>
																		</table>
																	</td>
																</tr>
															</table>
														</td>
														<td align="center">
															<asp:Button runat="server" ID="btAssign" Text="＞＞" onclick="btnAssign_Click"/>
															<asp:Button runat="server" ID="btUnassign" Text="＜＜" onclick="btnUnassign_Click"/>
														</td>
														<td valign="top" align="center">
															<table cellspacing="0" cellpadding="0" border="0">
																<tr>
																	<td>
																		<table class="edit_table" cellspacing="1" cellpadding="3" width="" border="0">
																			<tr>
																				<td class="edit_title_bg" align="left" width="120">所属オペレータ<span class="notice"></span></td>
																				<td class="edit_item_bg" align="left">
																					<asp:ListBox id="lbAssignedOperatorList" runat="server" SelectionMode="Multiple" Width="240" Height="480" />
																				</td>
																			</tr>
																		</table>
																	</td>
																</tr>
															</table>
														</td>
													</tr>
												</table>
												<div class="action_part_bottom" align="right">
													<asp:button id="btnBackBottom" runat="server" Text="　戻る　" onclick="btnBack_Click" ></asp:button>
													<asp:button id="btnUpdateBottom" runat="server" Text="　更新する　"  onclick="btnUpdate_Click"></asp:button>
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
	<!--△ 一覧・詳細 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
