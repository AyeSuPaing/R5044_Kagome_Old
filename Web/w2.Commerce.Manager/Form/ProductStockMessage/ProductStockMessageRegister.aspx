<%--
=========================================================================================================
  Module      : 商品在庫文言情報登録ページ(ProductStockMessageRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ProductStockMessageRegister.aspx.cs" Inherits="Form_ProductStockMessage_ProductStockMessageRegister" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">在庫文言設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 登録 ▽-->
	<tr id="trEdit" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">在庫文言設定編集</h2></td>
	</tr>
	<tr id="trRegister" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">在庫文言設定登録</h2></td>
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
													<input onclick="Javascript:history.back();" type="button" value="  戻る  " />
													<asp:Button id="btnConfirmTop" runat="server" Text="  確認する  " OnClick="btnConfirmTop_Click" /></div>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">基本情報</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">在庫文言ID<span class="notice">*</span></td>
															<td id="tdProductStockMessageIdEdit" class="edit_item_bg" align="left" runat="server" visible="false">
																<asp:TextBox ID="tbProductStockMessageId" runat="server" Text="<%# m_htParam[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_ID] %>" MaxLength="3"></asp:TextBox>
																
															</td>
															<td id="tdProductStockMessageIdView" class="edit_item_bg" align="left" runat="server" visible="false">
																<%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_ID]) %>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">在庫文言名<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox id="tbStockMessageName" runat="server" Text="<%# m_htParam[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_NAME] %>" Width="200" MaxLength="30"></asp:TextBox>
																<asp:Literal ID="lStockMessageName" runat="server" Text="<%# m_htParam[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_NAME] %>" Visible="False"></asp:Literal>
															</td>
														</tr>
														<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">言語コード<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left"><asp:DropDownList runat="server" id="ddlLanguageCode"/></td>
														</tr>
														<% } %>
													</tbody>
												</table>
												<br />
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="4">在庫基準情報</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">標準在庫文言<br />
																※在庫管理しない場合この表記を適用</td>
															<td class="edit_item_bg" align="left" width="70%"><asp:TextBox id="tbStockMessageDef" runat="server" Text="<%# m_htParam[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_DEF] %>" Width="200" MaxLength="30"></asp:TextBox></td>
														</tr>
													</tbody>
												</table>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="edit_title_bg" align="left" width="20%">No</td>
															<td class="edit_title_bg" align="left" width="20%">在庫基準</td>
															<td class="edit_title_bg" align="left" width="50%">在庫文言</td>
															<td class="edit_title_bg" align="center" width="10%"><asp:Button id="btnInsertStockMessage" runat="server" Text="  追加  " Enabled="<%# (m_alParam.Count < CONST_USER_PRODUCTSTOCKMESSAGE_STOCK_DATUM) && this.IsEditable %>" OnClick="btnInsertStockMessage_Click"/></td>
														</tr>
														<asp:Repeater id="rStockMessage" Runat="server" DataSource="<%# m_alParam %>">
															<ItemTemplate>
																<tr>
																	<td class="edit_title_bg" align="left" width="20%"><%#: ((Hashtable)Container.DataItem)[FIELD_PRODUCTSTOCKMESSAGE_STOCK_NO + ((int)(Container.ItemIndex + 1)).ToString()] %></td>
																	<td class="edit_item_bg" align="left"><asp:TextBox id="tbStockDatum" runat="server" Text="<%# ((Hashtable)Container.DataItem)[FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM + ((int)(Container.ItemIndex + 1)).ToString()] %>" Width="70" MaxLength="7" Enabled="<%# this.IsEditable %>"></asp:TextBox>&nbsp;以下</td>
																	<td class="edit_item_bg" align="left"><asp:TextBox id="tbStockMessage" runat="server" Text="<%# ((Hashtable)Container.DataItem)[FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE + ((int)(Container.ItemIndex + 1)).ToString()] %>" Width="200" MaxLength="30"></asp:TextBox></td>
																	<td class="edit_item_bg" align="center"><asp:Button id="btnDeleteStockMessage" runat="server" Text="  削除  " OnClick="btnDeleteStockMessage_Click" Enabled="<%# this.IsEditable %>" /></td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
													</tbody>
												</table>
												<div class="action_part_bottom"><input onclick="Javascript:history.back();" type="button" value="  戻る  " />
													<asp:Button id="btnConfirmBottom" runat="server" Text="  確認する  " OnClick="btnConfirmTop_Click" /></div>
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
	<!--△ 登録 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>