<%--
=========================================================================================================
  Module      : オペレータ情報登録ページ(OperatorRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OperatorRegister.aspx.cs" Inherits="Form_Operator_OperatorRegister" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr>
		<td><h1 class="page-title">オペレータ情報</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ タイトル △-->
	<!--▽ 編集入力 ▽-->
	<tr>
		<% if (Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_INSERT) {%>
		<td><h2 class="cmn-hed-h2">オペレータ情報登録</h2></td>
		<%} %>
		<% if (Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_UPDATE) {%>
		<td><h2 class="cmn-hed-h2">オペレータ情報編集</h2></td>
		<%} %>
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
												<div class="action_part_top">
													<input onclick="Javascript:history.back()" type="button" value="  戻る  " />
													<asp:Button id="btnConfirm" runat="server" Text="  確認する  " onclick="btnConfirm_Click"></asp:Button>
												</div>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr id="trOperatorId" runat="server" visible="false">
														<td class="edit_title_bg" align="left" width="30%">オペレータID</td>
														<td class="edit_item_bg" align="left"><asp:Label ID="lbOperatorId" runat="server"></asp:Label></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">オペレータ名<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left"><asp:TextBox ID="tbOperatorName" runat="server" MaxLength="20" Width="180" /></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">メニュー権限<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left"><asp:DropDownList id="ddlMenuAccessLevel" Runat="server"></asp:DropDownList></td>
													</tr>
													<% if (Constants.TWO_STEP_AUTHENTICATION_OPTION_ENABLED) { %>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">メールアドレス<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left"><asp:TextBox ID="tbMailAddress" runat="server" MaxLength="256" Width="150" /></td>
													</tr>
													<% } %>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">ログインＩＤ<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left"><asp:TextBox ID="tbLoginId" runat="server" MaxLength="16" Width="150" /></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">パスワード<span class="notice">*</span><br /><%= trOperatorId.Visible ? "（変更する場合のみ入力してください）" : "" %></td>
														<td class="edit_item_bg" align="left"><asp:TextBox ID="tbPassWord" runat="server" MaxLength="16" Width="150" TextMode="Password" /></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">有効フラグ</td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBox ID="cbValid" runat="server" Checked="true" Text="有効" />
														</td>
													</tr>
												</table>
												<div class="action_part_bottom">
													<input onclick="Javascript:history.back()" type="button" value="  戻る  " />
													<asp:Button id="btnConfirm2" runat="server" Text="  確認する  " onclick="btnConfirm_Click"></asp:Button>
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
	<!--△ 詳細 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>