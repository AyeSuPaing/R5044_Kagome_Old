<%--
=========================================================================================================
  Module      : エラー画面(Error.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="Error.aspx.cs" Inherits="Form_Error" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table width="791" border="0" cellspacing="0" cellpadding="0">
	<tr id="trFrameTopLine" runat="server">
	</tr>
	<tr>
		<td><img src="../Images/Common/sp.gif" alt="" height="19" width="100%" border="0" /></td>
	</tr>
	<!-- Main Contents List Area START -->
	<tr>
		<td><img src="../Images/Common/sp.gif" alt="" height="10" width="100%" border="0" /></td>
	</tr>
	<tr>
		<td align="center">
			<table width="600" border="0" cellspacing="0" cellpadding="0">
				<tr>
					<td><img src="../Images/Common/sp.gif" alt="" height="40" width="600" border="0" /></td>
				</tr>
				<tr>
					<td>
						<table width="100%" border="0" cellspacing="1" cellpadding="0" class="error_box_border">
							<tr>
								<td>
									<table class="error_box_bg" width="100%" border="0" cellspacing="0" cellpadding="0">
										<tr>
											<td align="center">
												<table border="0" cellspacing="0" cellpadding="0">
													<tr>
														<td><img src="../Images/Common/sp.gif" alt="" height="12" width="100%" border="0" /></td>
													</tr>
													<tr>
														<td>
															<table width="500" border="0" cellspacing="1" cellpadding="3" class="error_table">
																<tr class="error_title_bg">
																	<td align="left">下記の内容についてエラーが発生しました。</td>
																</tr>
																<tr class="error_item_bg">
																	<td align="left">
																		<div class="error_text">
																			<!-- エラー内容表示開始 -->
																			<asp:label id="LBErrorMsg" runat="server"></asp:label>
																			<!-- エラー内容表示終了 -->
																		</div>
																	</td>
																</tr>
															</table>
															<div id="DIVHistoryBack" runat="server" class="action_part_bottom" Visible="False">
																<input id="BtnHistoryBack" onclick="javascript:history.back();" type="button" value="  戻る  " name="BtnHistoryBack" />
															</div>
															<div class="action_part_bottom">
																<asp:Button id="BTNLogin" runat="server" Width="106px" Visible="False" Text="  ログイン画面へ  " onclick="BTNLogin_Click"></asp:Button>
															</div>
														</td>
													</tr>
													<tr>
														<td><img src="../Images/Common/sp.gif" alt="" height="19" width="100%" border="0" /></td>
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
			<!-- Main Contents List Area END -->
		</td>
	</tr>
	<tr>
		<td><img src="../Images/Common/sp.gif" alt="" height="10" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>