<%--
=========================================================================================================
  Module      : 注文拡張ステータス設定ページ(OrderExtendStatusSettingList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OrderExtendStatusSettingList.aspx.cs" Inherits="Form_OrderExtendStatusSetting_OrderExtendStatusSettingList" Title="" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<!-- タイトル -->
		<td><h1 class="page-title">注文拡張ステータス設定</h1></td>
	</tr>
	<tr>
		<!-- スペース -->
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<!-- タブ -->
		<td><h2 class="cmn-hed-h2">注文拡張ステータス設定一覧</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<td>
						<!--▽コンテンツ部分▽-->
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<!--編集ページ-->
									<div>
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<!-- スペース -->
												<td height="6" colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="6" border="0" /></td>
											</tr>
											<tr>
												<!-- スペース -->
												<td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="4" border="0" /></td>
											</tr>
											<tr>
												<!-- 一括更新ボタン -->
												<td align="right"><asp:Button ID="btnAllUpdateTop" runat="server" Text="  一括更新する  " OnClick="btnAllUpdate_Click" OnClientClick="return confirm('拡張ステータス名を入力したステータスのみ有効になります。\r\n登録更新してもよろしいですか？');" /></td>
												<td width="5"><img alt="" src="../../Images/Common/sp.gif" width="5" height="1" border="0" /></td>
											</tr>
											<tr>
												<!-- スペース -->
												<td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="4" border="0" /></td>
											</tr>
											<tr>
												<td valign="top">
													<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0" align="center">
													<tbody id="tbodyExtend">
														<tr class="list_title_bg">
															<td align="center" style="width:100px"></td>
															<td align="left">拡張ステータス名</td>
														</tr>
														<asp:Repeater id="rExtendList" Runat="server">
															<ItemTemplate>
																<tr class="list_item_bg1">
																	<td align="center">
																		拡張ステータス<%# WebSanitizer.HtmlEncode(Container.ItemIndex + 1) %>
																	</td>
																	<td align="left">
																		<%-- システム用（予約済）の注文拡張項目は編集不可にする --%>
																		<asp:TextBox ID="tbExtendName" runat="server" Width="380" MaxLength="30" Text='<%# Container.DataItem %>' Enabled="<%# (IsSystemReservedOrderExtendStatus(Container.ItemIndex + 1) == false) %>" />
																	</td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
													</tbody>
													</table>
												</td>
												<!-- スペース -->
												<td><img src="../../Images/Common/sp.gif" width="5" height="1" border="0" /></td>
											</tr>
											<tr>
												<!-- スペース -->
												<td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="4" border="0" /></td>
											</tr>
											<tr>
												<!-- 備考 -->
												<td colspan="6">
													<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr class="info_item_bg">
															<td align="left">備考<br />
																拡張ステータス名を入力した拡張ステータスのみ有効になります。<br />
																拡張ステータス名を空に設定した場合には、無効になりますが、過去の受注情報の当該項目のデータには、値が残るため注意してください。<br />
																拡張ステータス31～50はシステム用（予約済み）のため編集ができません。
															</td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<!-- スペース -->
												<td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="4" border="0" /></td>
											</tr>
											<tr>
												<!-- 一括更新ボタン -->
												<td align="right"><asp:Button ID="btnAllUpdateBottom" runat="server" Text="  一括更新する  " OnClick="btnAllUpdate_Click" OnClientClick="return confirm('拡張ステータス名を入力したステータスのみ有効になります。\r\n登録更新してもよろしいですか？');" /></td>
												<td width="5"><img alt="" src="../../Images/Common/sp.gif" width="5" height="1" border="0" /></td>
											</tr>
											<tr>
												<!-- スペース -->
												<td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="4" border="0" /></td>
											</tr>
										</table>
									</div>
								</td>
							</tr>
						</table>
						<!--△コンテンツ部分△-->
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<!-- スペース -->
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>