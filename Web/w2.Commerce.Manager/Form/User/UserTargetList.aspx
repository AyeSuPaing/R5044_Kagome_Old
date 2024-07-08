<%-- 
=========================================================================================================
	Module      : ターゲットリスト設定登録ページ処理(UserTargetList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
	Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="UserTargetList.aspx.cs" Inherits="Form_User_UserTargetList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<table cellspacing="0" cellpadding="0" width="641" border="0">
		<tr>
			<td>
				<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
			</td>
		</tr>
		<!--▽ タイトル ▽-->
		<tr>
			<td>
				<h2 class="cmn-hed-h2">ターゲットリスト一覧</h2>
			</td>
		</tr>
		<!--△ タイトル △-->
		<tr>
			<td>
				<table class="box_border" cellspacing="1" cellpadding="3" width="634" border="0">
					<tbody>
						<tr>
							<td>
								<table class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0"
									id="pnRegister" runat="server" visible="true">
									<tbody>
										<tr>
											<td align="center">
												<div class="action_part_top" style="width: 830px;">
													<asp:Button ID="btnRegisterTop" Text="  登録する  " runat="server" OnClick="btnRegisterTop_Click" OnClientClick="disableButton();" UseSubmitBehavior="true" />
												</div>
												<table cellspacing="0" cellpadding="0" border="0">
													<tbody>
														<tr>
															<td>
																<table class="detail_table" width="608" border="0" cellspacing="1" cellpadding="3">
																	<tbody>
																		<tr>
																			<td align="left" class="search_title_bg" width="130">
																				ターゲットリスト名<span class="notice">*</span>
																			</td>
																			<td align="left" class="search_item_bg">
																				<asp:TextBox ID="tbTargetListName" Width="300" runat="server"></asp:TextBox>
																			</td>
																		</tr>
																	</tbody>
																</table>
																<br/>
																<table class="detail_table" width="608" border="0" cellspacing="1" cellpadding="3">
																	<tbody>
																		<tr>
																			<td align="left" class="search_title_bg" width="130">
																				抽出元
																			</td>
																			<td align="left" class="search_item_bg">
																				<asp:Label ID="lbSourceName" runat="server"></asp:Label>
																			</td>
																		</tr>
																		<tr>
																			<td align="left" class="search_title_bg">
																				件数
																			</td>
																			<td align="left" class="search_item_bg">
																				<asp:Label ID="lbDataCount" runat="server"></asp:Label>
																			</td>
																		</tr>
																	</tbody>
																</table>
																<div class="action_part_bottom">
																</div>
															</td>
														</tr>
													</tbody>
												</table>
											</td>
										</tr>
									</tbody>
								</table>
								<table class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0" id="pnComplete" runat="server" visible="false">
									<tr>
										<td>
											<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
										</td>
									</tr>
									<tr>
											<td align="center">
												ターゲットリストの登録を開始しました。<br />
												結果はメールにてご連絡いたします。<br />
												※ターゲットリストの登録が完了するまで、ターゲットリスト一覧には表示されません。
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
					</tbody>
				</table>
			</td>
		</tr>
		<tr>
			<td>
				<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
			</td>
		</tr>
	</table>
	<script language="javascript" type="text/javascript">
		function disableButton() {
			document.getElementById("<%=btnRegisterTop.ClientID %>").style.display = "none";
		}
	</script>
</asp:Content>
