<%--
=========================================================================================================
	Module      : ターゲットリストマージ完了画面(TargetListMergeComplete.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
	Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="TargetListMergeComplete.aspx.cs" Inherits="Form_TargetListMerge_TargetListMergeComplete" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderHead" runat="Server">
<style type="text/css">
	.list_item_bg1
	{
		height: 32px;
	}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<table cellspacing="0" cellpadding="0" width="791" border="0">
		<!--▽ タイトル ▽-->
		<tr>
			<td>
				<h1 class="page-title">ターゲットリストマージ</h1>
			</td>
		</tr>
		<tr>
			<td>
				<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
			</td>
		</tr>
		<!--△ タイトル △-->
		<tr>
			<td>
				<table width="758" border="0" cellspacing="0" cellpadding="0">
					<tr>
						<td>
							<h2 class="cmn-hed-h2">ターゲットリストマージ</h2>
						</td>
					</tr>
					<tr>
						<td>
							<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
								<tr>
									<td>
										<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
											<tr>
												<td width="12">
													<img height="12" src="../../Images/Common/sp.gif" border="0">
												</td>
												<td align="left">
													<table cellspacing="0" cellpadding="5" border="0" width="100%">
														<tr>
															<td>
																<img height="12" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
															</td>
														</tr>
														<tr align="center">
															<td>
																<div>マージが完了しました。</div>
															</td>
														</tr>
														<tr>
															<td>
																<img height="12" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
															</td>
														</tr>
														<tr>
															<td align="left">
																<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
																	<tr class="list_title_bg">
																		<td colspan="4" align="center">
																			ターゲットリスト情報
																		</td>
																	</tr>
																	<tr class="list_item_bg1" valign="center">
																		<td width="120" align="left" class="list_title_bg">
																			ターゲットリスト１
																		</td>
																		<td width="550px">
																			&nbsp;<asp:Label ID="lbTargetList1" runat="server"></asp:Label>
																		</td>
																		<td colspan="2" width="100px" align="center">
																			<asp:Label ID="lbDataCount1" runat="server"></asp:Label>&nbsp;件
																		</td>
																	</tr>
																	<tr class="list_item_bg1" valign="center">
																		<td width="120" align="left" class="list_title_bg">
																			ターゲットリスト２
																		</td>
																		<td>
																			&nbsp;<asp:Label ID="lbTargetList2" runat="server"></asp:Label>
																		</td>
																		<td colspan="2" align="center">
																			<asp:Label ID="lbDataCount2" runat="server"></asp:Label>&nbsp;件
																		</td>
																	</tr>
																	<tr class="list_item_bg1">
																		<td align="left" class="list_title_bg">
																			マージパターン
																		</td>
																		<td colspan="3" style="padding: 4px">
																			&nbsp;<asp:Label ID="lbMergeKbn" runat="server"></asp:Label>
																		</td>
																	</tr>
																</table>
																<div><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></div>
																<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
																	<tr class="list_title_bg">
																		<td colspan="3" align="center">
																			マージ結果
																		</td>
																	</tr>
																	<tr class="list_item_bg1">
																		<td align="left" class="list_title_bg" width="120">
																			ターゲットリスト名&nbsp;
																		</td>
																		<td align="left" width="550px">
																			&nbsp;<asp:Label ID="lbTargetList" runat="server"></asp:Label>
																		</td>
																		<td align="center">
																			&nbsp;<asp:Label ID="lbDataCount" runat="server"></asp:Label>&nbsp;件
																		</td>
																	</tr>
																</table>
															</td>
														</tr>
														<tr>
															<td>
																<img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0">
															</td>
														</tr>
														<tr>
															<td class="action_list_sp">
																<asp:Button ID="btnBack" runat="server" Text="  戻る  " onclick="btnBack_Click" />
															</td>
														</tr>
														<tr>
															<td>
																<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
															</td>
														</tr>
													</table>
												</td>
												<td width="12">
													<img height="12" alt="" src="../../Images/Common/sp.gif" border="0">
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
							<img height="12" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
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