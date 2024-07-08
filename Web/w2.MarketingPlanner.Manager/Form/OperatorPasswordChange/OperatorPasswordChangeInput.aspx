<%--
=========================================================================================================
  Module      : パスワード変更入力ページ(OperatorPasswordChangeInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OperatorPasswordChangeInput.aspx.cs" Inherits="Form_OperatorPasswordChange_OperatorPasswordChangeInput" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr>
		<td><h1 class="page-title">パスワード変更</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ タイトル △-->
	<!--▽ パスワード変更 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">パスワード変更入力</h2></td>
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
												<asp:Button id="btnChange" runat="server" Text="  更新する  " onclick="btnChange_Click"></asp:Button>
												</div>
												<table class="edit_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr>
														<td class="edit_title_bg" align="left" width="30%">現在のパスワード<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left"><asp:TextBox ID="tbPassowrdOld" runat="server" TextMode="Password" MaxLength="16"></asp:TextBox></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">新しいパスワード<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left"><asp:TextBox ID="tbPassowrd" runat="server" TextMode="Password" MaxLength="16"></asp:TextBox></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">新しいパスワード(確認)<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left"><asp:TextBox ID="tbPassowrdConfirm" runat="server" TextMode="Password" MaxLength="16"></asp:TextBox></td>
													</tr>
												</table>
												<div class="action_part_bottom">
												<asp:Button id="btnChange2" runat="server" Text="  更新する  " onclick="btnChange_Click"></asp:Button>
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
	<!--△ パスワード変更 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>