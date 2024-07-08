<%--
=========================================================================================================
  Module      : 注文メモ設定確認ページ(OrderMemoConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OrderMemoConfirm.aspx.cs" Inherits="Form_OrderMemoInfo_OrderMemoConfirm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">注文メモ設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/></td>
	</tr>
	<!--▽ 詳細 ▽-->
	<tr id="trDetailTop" runat="server" Visible="True">
		<td><h2 class="cmn-hed-h2">注文メモ設定詳細</h2></td>
	</tr>
	<tr id="trConfirmTop" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">注文メモ設定入力確認</h2></td>
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
												<div class="action_part_top"><input type="button" onclick="Javascript:history.back();" value="  戻る  "/>
													<asp:Button id="btnEditTop" runat="server" Text="  編集する  " Visible="False" OnClick="btnEdit_Click" />
													<asp:Button id="btnCopyInsertTop" runat="server" Text="  コピー新規登録する  " Visible="False" OnClick="btnCopyInsert_Click" />
													<asp:Button id="btnDeleteTop" runat="server" Text="  削除する  " Visible="False" OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
													<asp:Button id="btnInsertTop" runat="server" Text="  登録する  " Visible="False" OnClick="btnInsert_Click" />
													<asp:Button id="btnUpdateTop" runat="server" Text="  更新する  " Visible="False" OnClick="btnUpdate_Click" />
												</div>
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">注文メモ設定</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="20%">注文メモID</td>
														<td class="detail_item_bg" align="left" colspan="1"><%# WebSanitizer.HtmlEncode(GetKeyValue(OrderMemoSetting, Constants.FIELD_ORDERMEMOSETTING_ORDER_MEMO_ID))%></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="20%">注文メモ名称</td>
														<td class="detail_item_bg" align="left" colspan="1"><%# WebSanitizer.HtmlEncode(GetKeyValue(OrderMemoSetting, Constants.FIELD_ORDERMEMOSETTING_ORDER_MEMO_NAME))%></td>
													</tr>

													<%-- 注文メモ名称翻訳設定情報 --%>
													<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
													<asp:Repeater runat="server" ID="rTranslationOrderMemoName"
														 DataSource="<%# this.OrderMemoSettingTranslationData.Where(d => d.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_ORDERMEMOSETTING_ORDER_MEMO_NAME) %>"
														 ItemType="w2.Domain.NameTranslationSetting.NameTranslationSettingModel">
													<ItemTemplate>
													<tr>
														<td class="detail_title_bg" align="left" width="20%">　言語コード:<%#: Item.LanguageCode %> 言語ロケールID:<%#: Item.LanguageLocaleId %></td>
														<td class="detail_item_bg" align="left" colspan="1"><%#: Item.AfterTranslationalName %></td>
													</tr>
													</ItemTemplate>
													</asp:Repeater>
													<% } %>

													<tr>
														<td class="detail_title_bg" align="left" width="20%">入力項目縦幅</td>
														<td class="detail_item_bg" align="left" colspan="1"><%# WebSanitizer.HtmlEncode(GetKeyValue(OrderMemoSetting, Constants.FIELD_ORDERMEMOSETTING_HEIGHT))%></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="20%">入力項目横幅</td>
														<td class="detail_item_bg" align="left" colspan="1"><%# WebSanitizer.HtmlEncode(GetKeyValue(OrderMemoSetting, Constants.FIELD_ORDERMEMOSETTING_WIDTH))%></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="20%">CSS Class</td>
														<td class="detail_item_bg" align="left" colspan="1" ><%# WebSanitizer.HtmlEncode(GetKeyValue(OrderMemoSetting, Constants.FIELD_ORDERMEMOSETTING_CSS_CLASS))%></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="20%">最大入力可能文字数</td>
														<td class="detail_item_bg" align="left" colspan="1"><%# WebSanitizer.HtmlEncode(GetKeyValue(OrderMemoSetting, Constants.FIELD_ORDERMEMOSETTING_MAX_LENGTH))%></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="20%">デフォルトテキスト</td>
														<td class="detail_item_bg" align="left" colspan="1"><%# WebSanitizer.HtmlEncodeChangeToBr(GetKeyValue(OrderMemoSetting, Constants.FIELD_ORDERMEMOSETTING_DEFAULT_TEXT))%></td>
													</tr>
													<%-- デフォルトテキスト翻訳設定情報 --%>
													<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
													<asp:Repeater runat="server" ID="rTranslationDefaultText"
														 DataSource="<%# this.OrderMemoSettingTranslationData.Where(d => d.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_ORDERMEMOSETTING_DEFAULT_TEXT) %>"
														 ItemType="w2.Domain.NameTranslationSetting.NameTranslationSettingModel">
													<ItemTemplate>
													<tr>
														<td class="detail_title_bg" align="left" width="20%">　言語コード:<%#: Item.LanguageCode %> 言語ロケールID:<%#: Item.LanguageLocaleId %></td>
														<td class="detail_item_bg" align="left" colspan="1"><%#: Item.AfterTranslationalName %></td>
													</tr>
													</ItemTemplate>
													</asp:Repeater>
													<% } %>
													<tr>
														<td class="detail_title_bg" align="left" width="20%">有効期限</td>
														<td class="detail_item_bg" align="left" colspan="1"><%#: DateTimeUtility.ToStringForManager(GetKeyValue(OrderMemoSetting, Constants.FIELD_ORDERMEMOSETTING_TERM_BGN), DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter)%>
															～<%#: DateTimeUtility.ToStringForManager(GetKeyValue(OrderMemoSetting, Constants.FIELD_ORDERMEMOSETTING_TERM_END), DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter)%></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="20%">表示順</td>
														<td class="detail_item_bg" align="left" colspan="1"><%# WebSanitizer.HtmlEncode(GetKeyValue(OrderMemoSetting, Constants.FIELD_ORDERMEMOSETTING_DISPLAY_ORDER))%></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="20%">有効フラグ</td>
														<td class="detail_item_bg" align="left" colspan="1"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_ORDERMEMOSETTING, Constants.FIELD_ORDERMEMOSETTING_VALID_FLG, (string)GetKeyValue(this.OrderMemoSetting, Constants.FIELD_ORDERMEMOSETTING_VALID_FLG)))%></td>
													</tr>
													<tr id="trDateCreated" runat="server" Visible="False">
														<td class="detail_title_bg" align="left" width="20%">作成日</td>
														<td class="detail_item_bg" align="left" colspan="1"><%#: DateTimeUtility.ToStringForManager(GetKeyValue(OrderMemoSetting, Constants.FIELD_ORDERMEMOSETTING_DATE_CREATED), DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter)%></td>
													</tr>
													<tr id="trDateChanged" runat="server" Visible="False">
														<td class="detail_title_bg" align="left" width="20%">更新日</td>
														<td class="detail_item_bg" align="left" colspan="1"><%#: DateTimeUtility.ToStringForManager(GetKeyValue(OrderMemoSetting, Constants.FIELD_ORDERMEMOSETTING_DATE_CHANGED), DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter)%></td>
													</tr>
													<tr id="trLastChanged" runat="server" Visible="False">
														<td class="detail_title_bg" align="left" width="20%">最終更新者</td>
														<td class="detail_item_bg" align="left" colspan="1"><%# WebSanitizer.HtmlEncode(GetKeyValue(OrderMemoSetting, Constants.FIELD_ORDERMEMOSETTING_LAST_CHANGED))%></td>
													</tr>
												</table>
												<div class="action_part_bottom"><input type="button" onclick="Javascript:history.back();" value="  戻る  "/>
													<asp:Button id="btnEditBottom" runat="server" Text="  編集する  " Visible="False" OnClick="btnEdit_Click" />
													<asp:Button id="btnCopyInsertBottom" runat="server" Text="  コピー新規登録する  " Visible="False" OnClick="btnCopyInsert_Click" />
													<asp:Button id="btnDeleteBottom" runat="server" Text="  削除する  " Visible="False" OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
													<asp:Button id="btnInsertBottom" runat="server" Text="  登録する  " Visible="False" OnClick="btnInsert_Click" />
													<asp:Button id="btnUpdateBottom" runat="server" Text="  更新する  " Visible="False" OnClick="btnUpdate_Click" />
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
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/></td>
	</tr>
</table>
</asp:Content>