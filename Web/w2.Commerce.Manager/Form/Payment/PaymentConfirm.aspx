<%--
=========================================================================================================
  Module      : 決済種別確認ページ(PaymentConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="PaymentConfirm.aspx.cs" Inherits="Form_Payment_PaymentConfirm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">決済種別設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 詳細 ▽-->
	<tr id="trDetail" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">決済種別設定詳細</h2></td>
	</tr>
	<tr id="trConfirm" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">決済種別設定入力確認</h2></td>
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
												<div class="action_part_top"><input type="button" onclick="Javascript:history.back();" value="  戻る  " />
													<asp:Button id="btnEditTop" runat="server" Text="  編集する  " Visible="False"  OnClick="btnEdit_Click" />
													<!--<asp:Button id="btnCopyInsertTop" runat="server" Text="  コピー新規登録する  " Visible="False" OnClick="btnCopyInsert_Click" />-->
													<!--<asp:Button id="btnDeleteTop" runat="server" Text="  削除する  " Visible="False" OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />-->
													<asp:Button id="btnInsertTop" runat="server" Text="  登録する  " Visible="False" OnClick="btnInsert_Click" />
													<asp:Button id="btnUpdateTop" runat="server" Text="  更新する  " Visible="False" OnClick="btnUpdateTop_Click" /></div>
													<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">基本情報</td>
													</tr>
													<tr id="trPaymentId" runat="server" Visible="False">
														<td class="detail_title_bg" align="left" width="30%">決済種別ID</td>
														<td class="detail_item_bg" align="left">
															<%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_PAYMENT_PAYMENT_ID]) %>
															&nbsp;&nbsp;&nbsp;<%: CreatePaymentAlertMessage(this.LoginOperatorShopId, (string)m_htParam[Constants.FIELD_PAYMENT_PAYMENT_ID]) %>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">決済種別</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PAYMENT, Constants.VALUETEXT_PARAM_PAYMENT_TYPE, (string)m_htParam[Constants.FIELD_PAYMENT_PAYMENT_ID]))%></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">決済種別名</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_PAYMENT_PAYMENT_NAME]) %></td>
													</tr>
													<%-- 決済種別名翻訳設定情報  --%>
													<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
													<asp:Repeater ID="rTranslationPaymentName" runat="server"
														DataSource="<%# this.PaymentTranslationData %>"
														ItemType="w2.Domain.NameTranslationSetting.NameTranslationSettingModel">
													<ItemTemplate>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">　言語コード:<%#: Item.LanguageCode %> 言語ロケールID:<%#: Item.LanguageLocaleId %></td>
														<td class="detail_item_bg" align="left"><%#: Item.AfterTranslationalName %></td>
													</tr>
													</ItemTemplate>
													</asp:Repeater>
													<% } %>
													<tr>
														<td class="detail_title_bg" align="left">下限金額</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(CreateUsablePriceString(m_htParam[Constants.FIELD_PAYMENT_USABLE_PRICE_MIN])) %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">上限金額</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(CreateUsablePriceString(m_htParam[Constants.FIELD_PAYMENT_USABLE_PRICE_MAX])) %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">手数料区分</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PAYMENT, Constants.FIELD_PAYMENT_PAYMENT_PRICE_KBN, (string)m_htParam[Constants.FIELD_PAYMENT_PAYMENT_PRICE_KBN])) %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">表示サイト</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PAYMENT, Constants.FIELD_PAYMENT_MOBILE_DISP_FLG, (string)m_htParam[Constants.FIELD_PAYMENT_MOBILE_DISP_FLG]))%></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">表示順</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_PAYMENT_DISPLAY_ORDER]) %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">有効フラグ</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PAYMENT, Constants.FIELD_PAYMENT_VALID_FLG, (string)m_htParam[Constants.FIELD_PAYMENT_VALID_FLG]))%></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">利用不可ユーザー管理レベル</td>
														<td class="detail_item_bg" align="left"><asp:Label id="lbUserManagementLevelNameList" runat="server"></asp:Label></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">利用不可注文者区分</td>
														<td class="detail_item_bg" align="left"><asp:Label id="lOrderOwnerKbnNameList" runat="server"></asp:Label></td>
													</tr>
													<tr id="trDateCreated" runat="server" Visible="False">
														<td class="detail_title_bg" align="left">作成日</td>
														<td class="detail_item_bg" align="left"><%#: DateTimeUtility.ToStringForManager(m_htParam[Constants.FIELD_PAYMENT_DATE_CREATED], DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
													</tr>
													<tr id="trDateChanged" runat="server" Visible="False">
														<td class="detail_title_bg" align="left">更新日</td>
														<td class="detail_item_bg" align="left"><%#: DateTimeUtility.ToStringForManager(m_htParam[Constants.FIELD_PAYMENT_DATE_CHANGED], DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
													</tr>
													<tr id="trLastChanged" runat="server" Visible="False">
														<td class="detail_title_bg" align="left">最終更新者</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_PAYMENT_LAST_CHANGED]) %></td>
													</tr>
												</table>
												<br />
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="4">決済手数料情報</td>
													</tr>
													<tr id="trPaymentPrice" runat="server" Visible="True">
														<td class="detail_title_bg" align="left" width="30%">決済手数料</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_PAYMENTPRICE_PAYMENT_PRICE].ToPriceString(true)) %></td>
													</tr>
													<asp:Repeater id="rPaymentPrice" Visible="False" DataSource="<%# (ArrayList)m_alParam %>" Runat="server">
														<HeaderTemplate>
															<tr>
																<td class="detail_title_bg" align="left" width="31%"></td>
																<td class="detail_title_bg" align="left" width="20%">対象購入金額（From）</td>
																<td class="detail_title_bg" align="center" width="20%">対象購入金額（To）</td>
																<td class="detail_title_bg" align="center" width="15%">決済手数料</td>
															</tr>
														</HeaderTemplate>
														<ItemTemplate>
															<tr>
																<td class="detail_title_bg" align="left" width="30%"></td>
																<td class="detail_item_bg" align="left">
																	<%# WebSanitizer.HtmlEncode(((Hashtable)Container.DataItem)[Constants.FIELD_PAYMENTPRICE_TGT_PRICE_BGN].ToPriceString(true))%>
																</td>
																<td class="detail_item_bg" align="left">
																	<%# WebSanitizer.HtmlEncode(((Hashtable)Container.DataItem)[Constants.FIELD_PAYMENTPRICE_TGT_PRICE_END].ToPriceString(true)) %>
																</td>
																<td class="detail_item_bg" align="left">
																	<%# WebSanitizer.HtmlEncode(((Hashtable)Container.DataItem)[Constants.FIELD_PAYMENTPRICE_PAYMENT_PRICE].ToPriceString(true)) %>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
												</table>
												<div class="action_part_bottom"><input type="button" onclick="Javascript:history.back();" value="  戻る  " />
													<asp:Button id="btnEditBottom" runat="server" Text="  編集する  " Visible="False" OnClick="btnEdit_Click" />
													<!--<asp:Button id="btnCopyInsertBottom" runat="server" Text="  コピー新規登録する  " Visible="False" OnClick="btnCopyInsert_Click" />-->
													<!--<asp:Button id="btnDeleteBottom" runat="server" Text="  削除する  " Visible="False" OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />-->
													<asp:Button id="btnInsertBottom" runat="server" Text="  登録する  " Visible="False" OnClick="btnInsert_Click" />
													<asp:Button id="btnUpdateBottom" runat="server" Text="  更新する  " Visible="False" OnClick="btnUpdateTop_Click" /></div>
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
