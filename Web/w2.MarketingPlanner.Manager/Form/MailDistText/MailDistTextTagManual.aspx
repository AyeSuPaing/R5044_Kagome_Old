<%--
=========================================================================================================
  Module      : メール配信文章タグマニュアルページ(MailDistTextTagManual.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Xml" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="MailDistTextTagManual.aspx.cs" Inherits="Form_MailDistText_MailDistTextTagManual" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="98%" border="0">
	<tbody>
		<tr>
			<td><h2 class="cmn-hed-h2">メール配信文章タグマニュアル</h2></td>
		</tr>
		<tr>
			<td>
				<table class="box_border" cellspacing="1" cellpadding="3" width="100%" border="0">
					<tr>
						<td>
							<table class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
								</tr>
								<tr>
									<td align="center">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td>
													<table class="detail_table" cellspacing="1" cellpadding="3" width="584" border="0">
														<tr>
															<td class="detail_title_bg" align="center" colspan="3">メール配信文章タグマニュアル</td>
														</tr>
														<asp:Repeater ID="rMailDistTextTagManual" runat="server">
															<HeaderTemplate>
																<tr>
																	<td class="detail_title_bg" align="center" width="33%">差し込みスクリプト</td>
																	<td class="detail_title_bg" align="center" width="33%">表示される項目</td>
																	<td class="detail_title_bg" align="center" width="34%">表示例</td>
																</tr>
															</HeaderTemplate>
															<ItemTemplate>
																<tr>
																	<td class="detail_title_bg" align="left" width="33%">
																		<%# WebSanitizer.HtmlEncode(((XmlNode)Container.DataItem).SelectSingleNode(XPATH_PAGE_NODE_TAG).InnerText)%></td>
																	<td class="detail_item_bg" align="left" width="33%">
																		<%# WebSanitizer.HtmlEncode(((XmlNode)Container.DataItem).SelectSingleNode(XPATH_PAGE_NODE_DESCRIPTION).InnerText)%></td>
																	<td class="detail_item_bg" align="left" width="34%">
																		<%# WebSanitizer.HtmlEncodeChangeToBr(((XmlNode)Container.DataItem).SelectSingleNode(XPATH_PAGE_NODE_SAMPLE).InnerText)%></td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
													</table>
												</td>
											</tr>
										</table>
									</td>
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
		<tr>
			<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
		</tr>
	</tbody>
</table>
</asp:Content>
