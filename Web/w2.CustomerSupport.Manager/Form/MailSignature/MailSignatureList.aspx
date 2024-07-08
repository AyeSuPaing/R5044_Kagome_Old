<%--
=========================================================================================================
  Module      : メール署名設定一覧ページ(MailSignatureList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Cs.MailSignature" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MailSignatureList.aspx.cs" Inherits="Form_MailSignature_MailSignatureList" Title="" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">メール署名設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">メール署名設定一覧</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="0" cellpadding="0" width="784" border="0">
				<tr>
					<td>
						<table cellspacing="1" cellpadding="0" width="100%" border="0">
							<tr>
								<td>
									<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
										<tr>
											<td align="center">
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
													<tr>
														<td>
															<!--▽ ページング ▽-->
															<table class="list_pager" cellspacing="0" cellpadding="0" border="0" width="758">
																<tr>
																	<td width="675"><asp:label id="lbPager1" Runat="server"></asp:label></td>
																	<td width="83" class="action_list_sp" style="height: 22px"><asp:button id="btnInsertTop" runat="server" Text="　新規登録　" onclick="btnInsert_Click" /></td>
																</tr>
															</table>
															<!--△ ページング △-->
														</td>
													</tr>
													<tr>
														<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
													<tr>
														<td>
															<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
																<tr class="list_title_bg">
																	<td align="center" width="100">署名タイトル</td>
																	<td align="center" width="358">署名本文</td>
																	<td align="center" width="80">所有者</td>
																	<td align="center" width="60">表示順</td>
																	<td align="center" width="60">有効フラグ</td>
																</tr>
																<asp:repeater id="rList" Runat="server">
																	<ItemTemplate>
																		<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# CreateDetailUrl(((CsMailSignatureModel)Container.DataItem).MailSignatureId) %>')">
																			<td align="left">&nbsp;<%# WebSanitizer.HtmlEncode(((CsMailSignatureModel)Container.DataItem).SignatureTitle) %></td>
																			<td align="left"><%# WebSanitizer.HtmlEncodeChangeToBr(((CsMailSignatureModel)Container.DataItem).SignatureText) %></td>
																			<td align="center">&nbsp;<%# WebSanitizer.HtmlEncode((((CsMailSignatureModel)Container.DataItem).OwnerId == "") ? "（共通）" : ((((CsMailSignatureModel)Container.DataItem).OwnerId == this.LoginOperatorId) ? this.LoginOperatorName : "不明")) %></td>
																			<td align="center"><%# WebSanitizer.HtmlEncode(((CsMailSignatureModel)Container.DataItem).DisplayOrder.ToString()) %></td>
																			<td align="center"><%# WebSanitizer.HtmlEncode(((CsMailSignatureModel)Container.DataItem).EX_ValidFlgText) %></td>
																		</tr>
																	</ItemTemplate>
																</asp:repeater>
																<tr id="trListError" class="list_alert" runat="server" Visible="False">
																	<td id="tdErrorMessage" colspan="5" runat="server"></td>
																</tr>
															</table>
														</td>
													</tr>
													<tr>
														<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
													<tr>
														<td class="action_part_bottom"><asp:Button id="btnInsertBotttom" runat="server" Text="　新規登録　" onclick="btnInsert_Click"></asp:Button></td>
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
			</table>
		</td>
	</tr>
	<!--△ 一覧 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>