<%--
=========================================================================================================
  Module      : 決済種別設定登録ページ(PaymentRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="PaymentRegister.aspx.cs" Inherits="Form_Payment_PaymentRegister" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">決済種別設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 登録 ▽-->
	<tr id="trEdit" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">決済種別設定編集</h2></td>
	</tr>
	<tr id="trRegister" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">決済種別設定登録</h2></td>
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
												<div class="action_part_top"><input onclick="Javascript:history.back();" type="button" value="  戻る  " />
													<asp:Button id="btnConfirmTop" runat="server" Text="  確認する  " OnClick="btnConfirm_Click" /></div>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="detail_title_bg" align="center" colspan="2">基本情報</td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left" width="30%">決済種別ID</td>
															<td class="detail_item_bg" align="left">
																<%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_PAYMENT_PAYMENT_ID]) %>
																&nbsp;&nbsp;&nbsp;<%: CreatePaymentAlertMessage(this.LoginOperatorShopId, (string)m_htParam[Constants.FIELD_PAYMENT_PAYMENT_ID]) %>
															</td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left" width="30%">決済種別</td>
															<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PAYMENT, Constants.VALUETEXT_PARAM_PAYMENT_TYPE, m_htParam[Constants.FIELD_PAYMENT_PAYMENT_ID])) %></td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left" width="30%">決済種別名<span class="notice">*</span></td>
															<td class="detail_item_bg" align="left"><asp:TextBox id="tbPaymentName" runat="server" Text="<%# m_htParam[Constants.FIELD_PAYMENT_PAYMENT_NAME] %>" Width="200" MaxLength="20"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left" width="30%">金額下限</td>
															<td class="detail_item_bg" align="left"><asp:TextBox id="tbUsablePriceBgn" runat="server" Text="<%# m_htParam[Constants.FIELD_PAYMENT_USABLE_PRICE_MIN].ToPriceString() %>" Width="200" MaxLength="10"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left" width="30%">金額上限</td>
															<td class="detail_item_bg" align="left"><asp:TextBox id="tbUsablePriceEnd" runat="server" Text="<%# m_htParam[Constants.FIELD_PAYMENT_USABLE_PRICE_MAX].ToPriceString() %>" Width="200" MaxLength="10"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left" width="30%">手数料区分</td>
															<td class="detail_item_bg" align="left"><asp:RadioButtonList id="rblPaymentPriceKbn" Runat="server" RepeatDirection="Horizontal" AutoPostBack="True" CssClass="radio_button_list" OnSelectedIndexChanged="rblPaymentPriceKbn_SelectedIndexChanged" RepeatLayout="Flow"></asp:RadioButtonList></td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left" width="30%">表示サイト</td>
															<td class="detail_item_bg" align="left"><asp:DropDownList ID="ddlMobileDispFlg"  SelectedValue='<%# m_htParam[Constants.FIELD_PAYMENT_MOBILE_DISP_FLG] %>' runat="server"></asp:DropDownList></td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left" width="30%">表示順<span class="notice">*</span></td>
															<td class="detail_item_bg" align="left"><asp:TextBox id="tbDisplayOrder" runat="server" Text="<%# m_htParam[Constants.FIELD_PAYMENT_DISPLAY_ORDER] %>" Width="25" MaxLength="3"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left" width="30%">有効フラグ</td>
															<td class="detail_item_bg" align="left"><asp:CheckBox ID="cbValidFlg" runat="server" Checked="<%# ((string)m_htParam[Constants.FIELD_PAYMENT_VALID_FLG] == Constants.FLG_PAYMENT_VALID_FLG_VALID) %>" Text="有効" /></td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left" width="30%">利用不可ユーザー管理レベル</td>
															<td class="detail_item_bg" align="left"><asp:CheckBoxList ID="cblUserManagementLevelList" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList></td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left" width="30%">利用不可注文者区分</td>
															<td class="detail_item_bg" align="left"><asp:CheckBoxList ID="cblOrderOwnerKbnLevelList" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList></td>
														</tr>
													</tbody>
												</table>
												<br />
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="detail_title_bg" align="center" colspan="<%= (rblPaymentPriceKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_PRICE_KBN_SINGULAR) ? 2 : 5 %>" style="HEIGHT: 24px">決済手数料情報</td>
														</tr>
														<tr id="trPaymentPrice" runat="server" Visible="True">
															<td class="detail_title_bg" align="left" width="30%">決済手数料<span class="notice">*</span></td>
															<td class="detail_item_bg" align="left">
																&yen;<asp:TextBox id="tbPaymentPrice" runat="server" Text="<%# m_htParam[Constants.FIELD_PAYMENTPRICE_PAYMENT_PRICE].ToPriceString() %>" Width="100" MaxLength="7"></asp:TextBox></td>
														</tr>
														<asp:Repeater id="rPaymentPrice" Runat="server" DataSource="<%# (ArrayList)m_alParam %>" Visible="False" OnItemCommand="rPaymentPrice_ItemCommand">
															<HeaderTemplate>
																<tr>
																	<td class="detail_title_bg" align="left" width="31%"></td>
																	<td class="detail_title_bg" align="left" width="20%">対象購入金額（From）</td>
																	<td class="detail_title_bg" align="center" width="20%">対象購入金額（To）<span class="notice">*</span></td>
																	<td class="detail_title_bg" align="center" width="15%">決済手数料<span class="notice">*</span></td>
																	<td class="detail_title_bg" align="center" width="15%">
																		<asp:Button id="btnInsertPaymentPrice" runat="server" Text="  追加  " CommandName="InsertPaymentPrice" CommandArgument="<%# m_alParam.Count %>" /></td>
																</tr>
															</HeaderTemplate>
															<ItemTemplate>
																<tr>
																	<td class="detail_title_bg" align="left" width="30%"></td>
																	<td class="detail_item_bg" align="left">
																		<%#  WebSanitizer.HtmlEncode((int)((Hashtable)Container.DataItem)[Constants.FIELD_PAYMENTPRICE_PAYMENT_PRICE_NO] != 1 ? "上記～" : 0.ToPriceString(true) + "～") %>
																	</td>
																	<td class="detail_item_bg" align="left">
																		&yen;
																		<asp:TextBox ID="tbTgtPriceEnd" Runat="server" Text='<%# ((Hashtable)Container.DataItem)[Constants.FIELD_PAYMENTPRICE_TGT_PRICE_END].ToPriceString() %>' Width="70" MaxLength="7">
																		</asp:TextBox>
																	</td>
																	<td class="detail_item_bg" align="left">
																		&yen;
																		<asp:TextBox ID="tbPaymentPrices" Runat="server" Text='<%# ((Hashtable)Container.DataItem)[Constants.FIELD_PAYMENTPRICE_PAYMENT_PRICE].ToPriceString() %>' Width="70" MaxLength="7">
																		</asp:TextBox>
																	</td>
																	<td class="detail_item_bg" align="center">
																		<asp:Button id="btnDeletePaymentPrice" runat="server" Text="  削除  " CommandName="DeletePaymentPrice" CommandArgument="<%# ((Hashtable)Container.DataItem)[Constants.FIELD_PAYMENTPRICE_PAYMENT_PRICE_NO] %>" Visible="<%# (int)((Hashtable)Container.DataItem)[Constants.FIELD_PAYMENTPRICE_PAYMENT_PRICE_NO] != 1 ? true:false %>" />
																	</td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
													</tbody>
												</table>
												<br />
												<table class="info_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr>
														<td align="left" class="info_item_bg" colspan="2">備考（各項目の説明）<br />
															■決済手数料情報<br />
																・対象購入金額（From）・・・決済手数料が適用される購入金額の範囲指定（From）<br />
																・対象購入金額（To） ・・・決済手数料が適用される購入金額の範囲指定（To）<br />
																・手数料 ・・・１注文時に掛かる決済手数料<br />
																<br />
																	※1.対象購入金額(From、To)の指定は『手数料区分』が『決済手数料を分ける』を選択された時のみ有効です｡<br />
																	※2.購入金額が対象購入金額範囲外の場合、フロント側で決済手数料が取得できないといった主旨のエラーを表示されます。<br />
																		そのため最後の対象購入金額（To）を大きな値で設定してください｡（例：&yen;9,999,999）<br />
																	※3.利用不可ユーザー管理レベルはフロント側のみ制御されます。<br />


														</td>
													</tr>
												</table>
												<div class="action_part_bottom"><input onclick="Javascript:history.back();" type="button" value="  戻る  " />
													<asp:Button id="btnConfirmBottom" runat="server" Text="  確認する  " OnClick="btnConfirm_Click" /></div>
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
	<!--△ 登録 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>