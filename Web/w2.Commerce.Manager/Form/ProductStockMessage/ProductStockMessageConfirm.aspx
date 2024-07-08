<%--
=========================================================================================================
  Module      : 商品在庫文言情報確認ページ(ProductStockMessageConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ProductStockMessageConfirm.aspx.cs" Inherits="Form_ProductStockMessage_ProductStockMessageConfirm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">在庫文言設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 詳細 ▽-->
	<tr id="trDetail" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">在庫文言設定詳細</h2></td>
	</tr>
	<tr id="trConfirm" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">在庫文言設定入力確認</h2></td>
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
												<div class="action_part_top" style="float: left;">
													<asp:DropDownList ID="ddlLanguageCode" runat="server" Visible="False" OnSelectedIndexChanged="ddlLanguageCode_OnSelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
												</div>
												<div class="action_part_top"><input type="button" id="btnGoBackTop" runat="server" Visible="False" value="  戻る  " onclick="Javascript:history.back();" />
													<asp:Button id="btnBackToListTop" runat="server" Text="  一覧へ戻る  " Visible="False" onclick="btnBackToListTop_Click"></asp:Button>
													<asp:Button id="btnEditTop" runat="server" Text="  編集する  " Visible="False" OnClick="btnEditTop_Click" />
													<asp:Button id="btnCopyInsertTop" runat="server" Text="  コピー新規登録する  " Visible="False" OnClick="btnCopyInsertTop_Click" />
													<asp:Button id="btnDeleteTop" runat="server" Text="  削除する  " Visible="False" OnClick="btnDeleteTop_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
													<asp:Button id="btnInsertTop" runat="server" Text="  登録する  " Visible="False" OnClick="btnInsertTop_Click" />
													<asp:Button id="btnUpdateTop" runat="server" Text="  更新する  " Visible="False" OnClick="btnUpdateTop_Click" />
													<asp:Button id="btnInsertGlobalTop" runat="server" Text="  他言語コードで登録する  " OnClick="btnInsertGlobal_Click" Visible="False" />
												</div>
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">基本情報</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">在庫文言ID</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_ID]) %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">在庫文言名</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_NAME]) %></td>
													</tr>
													<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">言語コード</td>
														<td class="detail_item_bg" align="left">
															<%#: (string.IsNullOrEmpty((string)m_htParam[Constants.FIELD_PRODUCTSTOCKMESSAGE_LANGUAGE_CODE]) == false)
																? string.Format("{0}({1})", (string)m_htParam[Constants.FIELD_PRODUCTSTOCKMESSAGE_LANGUAGE_CODE], (string)m_htParam[Constants.FIELD_PRODUCTSTOCKMESSAGE_LANGUAGE_LOCALE_ID])
																: ValueText.GetValueText(Constants.TABLE_PRODUCTSTOCKMESSAGE, Constants.FIELD_PRODUCTSTOCKMESSAGE_LANGUAGE_CODE, string.Empty) %>
														</td>
													</tr>
													<% } %>
													<tr id="trDateCreated" runat="server" Visible="False">
														<td class="detail_title_bg" align="left" width="30%">作成日</td>
														<td class="detail_item_bg" align="left"><%#: DateTimeUtility.ToStringForManager(m_htParam[Constants.FIELD_PAYMENT_DATE_CREATED], DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
													</tr>
													<tr id="trDateChanged" runat="server" Visible="False">
														<td class="detail_title_bg" align="left" width="30%">更新日</td>
														<td class="detail_item_bg" align="left"><%#: DateTimeUtility.ToStringForManager(m_htParam[Constants.FIELD_PAYMENT_DATE_CHANGED], DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
													</tr>
													<tr id="trLastChanged" runat="server" Visible="False">
														<td class="detail_title_bg" align="left" width="30%">最終更新者</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode((string)m_htParam[Constants.FIELD_PAYMENT_LAST_CHANGED]) %></td>
													</tr>
												</table>
												<br />
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="4">在庫基準情報</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">標準在庫文言<br />
															※在庫管理しない場合この表記を適用</td>
														<td class="detail_item_bg" align="left" width="70%"><%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_DEF]) %></td>
													</tr>
												</table>
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="left" width="20%">No</td>
														<td class="detail_title_bg" align="left" width="20%">在庫基準</td>
														<td class="detail_title_bg" align="left" width="60%">在庫文言</td>
													</tr>
													<asp:Repeater id="rStockMessage" Runat="server" DataSource="<%# m_alParam %>">
														<ItemTemplate>
															<tr>
																<td class="detail_title_bg" align="left" width="20%">
																	<%#: StringUtility.ToNumeric(((Hashtable)Container.DataItem)[FIELD_PRODUCTSTOCKMESSAGE_STOCK_NO + ((int)(Container.ItemIndex + 1)).ToString()]) %>
																</td>
																<td class="detail_item_bg" align="left">
																	<%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(((Hashtable)Container.DataItem)[FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM + ((int)(Container.ItemIndex + 1)).ToString()])) %>以下
																</td>
																<td class="detail_item_bg" align="left">
																	<%#:ToProductStockMessage((string)((Hashtable)Container.DataItem)[FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE + (int)(Container.ItemIndex + 1)])%>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
												</table>
												<div class="action_part_bottom"><input type="button" id="btnGoBackBottom" runat="server" Visible="False"  value="  戻る  " onclick="Javascript:history.back();" />
													<asp:Button id="btnBackToListBottom" runat="server" Text="  一覧へ戻る  " Visible="False" onclick="btnBackToListTop_Click"></asp:Button>
													<asp:Button id="btnEditBottom" runat="server" Text="  編集する  " Visible="False" OnClick="btnEditTop_Click" />
													<asp:Button id="btnCopyInsertBottom" runat="server" Text="  コピー新規登録する  " Visible="False" OnClick="btnCopyInsertTop_Click" />
													<asp:Button id="btnDeleteBottom" runat="server" Text="  削除する  " Visible="False" OnClick="btnDeleteTop_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
													<asp:Button id="btnInsertBottom" runat="server" Text="  登録する  " Visible="False" OnClick="btnInsertTop_Click" />
													<asp:Button id="btnUpdateBottom" runat="server" Text="  更新する  " Visible="False" OnClick="btnUpdateTop_Click" />
													<asp:Button id="btnInsertGlobalBottom" runat="server" Text="  他言語コードで登録する  " OnClick="btnInsertGlobal_Click" Visible="False" />
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
	<!--△ 詳細 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>