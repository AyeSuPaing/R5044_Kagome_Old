<%--
=========================================================================================================
  Module      : オペレータ権限設定一覧ページ(OperatorAuthorityList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OperatorAuthorityList.aspx.cs" Inherits="Form_OperatorAuthority_OperatorAuthorityList" %>
<%@ MasterType VirtualPath="~/Form/Common/DefaultPage.master" %>
<%@ Import Namespace="w2.App.Common.Cs.CsOperator" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">オペレータ権限設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">オペレータ権限設定一覧</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<!--▽ ページング ▽-->
												<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
													<tr>
														<td width="675"><asp:Label ID="lbPager" Runat="server"></asp:Label></td>
														<td width="83" class="action_list_sp" style="text-align:right">
															<asp:Button id="btnNewTop" Runat="server" Text="  新規登録  " OnClick="btnNew_Click" /></td>
													</tr>
												</table>
												<!-- ページング-->
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="list_title_bg">
														<td align="center" rowspan="2" width="379">オペレータ権限名</td>
														<td align="center" colspan="5">オペレーション</td>
														<td align="center" colspan="1">システム設定</td>
														<td align="center" colspan="1">メール通知</td>
													</tr>
													<tr class="list_title_bg">
														<td align="center" width="11%">編集</td>
														<td align="center" width="11%">メール<br />直接送信</td>
														<td align="center" width="11%">承認</td>
														<td align="center" width="11%">ロック<br />強制解除</td>
														<td align="center" width="11%">完全削除</td>
														<td align="center" width="11%">共通署名編集</td>
														<td align="center" width="11%">担当未設定<br />警告</td>
													</tr>
													<asp:Repeater id="operatorAuthorityList" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateDetailUrl(((CsOperatorAuthorityModel)Container.DataItem).OperatorAuthorityId)) %>')">
																<td align="left">&nbsp;<%# WebSanitizer.HtmlEncode(((CsOperatorAuthorityModel)Container.DataItem).OperatorAuthorityName)%></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(((CsOperatorAuthorityModel)Container.DataItem).EX_PermitEditFlgText) %></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(((CsOperatorAuthorityModel)Container.DataItem).EX_PermitMailSendFlgText) %></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(((CsOperatorAuthorityModel)Container.DataItem).EX_PermitApprovalFlgText) %></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(((CsOperatorAuthorityModel)Container.DataItem).EX_PermitUnlockFlgText) %></td>
																<td align="center"><%#: ((CsOperatorAuthorityModel)Container.DataItem).EX_PermitPermanentDeleteFlgText %></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(((CsOperatorAuthorityModel)Container.DataItem).EX_PermitEditSignatureFlgText) %></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(((CsOperatorAuthorityModel)Container.DataItem).EX_ReceiveNoAssignWarningFlgText) %></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="7"></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td class="action_list_sp">
												<asp:Button id="btnNewBottom" Runat="server" Text="  新規登録  " OnClick="btnNew_Click" /></td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
	<!--△ 一覧 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>