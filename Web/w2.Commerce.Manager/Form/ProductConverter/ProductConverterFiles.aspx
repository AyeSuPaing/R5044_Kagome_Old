<%--
=========================================================================================================
  Module      : 商品コンバータ 結果ファイル一覧ページ(ProductConverterFiles.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="ProductConverterFiles.aspx.cs" Inherits="Form_ProductConverter_ProductConverterFiles" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<td><h2 class="cmn-hed-h2">結果ファイル一覧</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td class="list_box_bg" align="center">
						<table cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td>
												<table>
													<tr>
														<td><img height="10px" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
												</table>
												<!-- ▽ページング▽ -->
												<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
													<tr>
														<td style="width:600px;height: 22px"><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
														<td class="action_list_sp" style="text-align:right">
														</td>
													</tr>
												</table>
												<!-- △ページング△ -->
												<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td>ダウンロード</td>
													</tr>
													<!-- ▽商品コンバータ一覧▽ -->
													<asp:Repeater id="rProductConverterList" Runat="server">
														<ItemTemplate>
															<tr id="rProductConverterListItem<%# Container.ItemIndex%>" class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)">
																<td><a href="<%# WebSanitizer.HtmlEncode(Constants.PATH_ROOT_FRONT_PC)%><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_MALLPRDCNV_PATH))%>"><%# WebSanitizer.HtmlEncode(Constants.PATH_ROOT_FRONT_PC)%><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_MALLPRDCNV_PATH))%></a></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<!-- △商品コンバータ一覧△ -->
													<tr id="trProductConverterListError" class="list_alert" runat="server" visible="false">
														<td id="tdErrorMessage" runat="server" colspan="8"></td>
													</tr>
												</table>
												<table>
													<tr>
														<td><img height="10px" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
												</table>
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="info_item_bg">
														<td align="left">
															備考<br />
															・過去10,000件の履歴を確認することが出来ます。（過去10,000件を遡る履歴については、システム管理者へお問い合わせください。）<br />
														</td>
													</tr>
												</table>
												<table>
													<tr>
														<td><img height="10px" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>