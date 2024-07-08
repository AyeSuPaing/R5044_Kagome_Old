<%--
=========================================================================================================
  Module      : 決済種別設定一覧ページ(PaymentList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="PaymentList.aspx.cs" Inherits="Form_Payment_PaymentList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">決済種別設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">決済種別設定一覧</h2></td>
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
														<td width="675"><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
														<td width="83" class="action_list_sp" style="white-space: nowrap;">
															<!--<asp:Button id="btnInsertTop" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" />-->
															<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
															<asp:LinkButton id="lbExportTranslationData" Runat="server" OnClick="lbExportTranslationData_Click">翻訳設定出力</asp:LinkButton>
															<% } %>
														</td>
													</tr>
												</table>
												<!-- ページング--></td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<asp:Repeater id="rList" ItemType="w2.Domain.Payment.PaymentModel" Runat="server">
														<HeaderTemplate>
															<tr class="list_title_bg">
																<td align="center" width="70">決済種別ID</td>
																<td align="left" width="140">決済種別</td>
																<td align="left" width="140">決済種別名</td>
																<td align="center" width="70">決済手数料</td>
																<td align="center" width="70">下限金額</td>
																<td align="center" width="70">上限金額</td>
																<td align="center" width="128">表示サイト</td>
																<td align="center" width="60">有効フラグ</td>
																<td align="center" width="50">表示順</td>
															</tr>
														</HeaderTemplate>
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%#: CreatePaymentDetailUrl(Item.PaymentId) %>')">
																<td align="center"><%#: Item.PaymentId %></td>
																<td align="left"><%#: ValueText.GetValueText(Constants.TABLE_PAYMENT, Constants.VALUETEXT_PARAM_PAYMENT_TYPE, Item.PaymentId) %></td>
																<td align="left"><%#: Item.PaymentName %></td>
																<td align="center"><%#: (Item.PaymentPriceKbn == Constants.FLG_PAYMENT_PAYMENT_PRICE_KBN_SINGULAR) ? Item.PaymentPrice.ToPriceString(true) : "複数手数料指定" %></td>
																<td align="center"><%#: CreateUsablePriceString(Item.UsablePriceMin) %></td>
																<td align="center"><%#: CreateUsablePriceString(Item.UsablePriceMax) %></td>
																<td align="center"><%#: ValueText.GetValueText(Constants.TABLE_PAYMENT, Constants.FIELD_PAYMENT_MOBILE_DISP_FLG, Item.MobileDispFlg) %></td>
																<td align="center"><%#: ValueText.GetValueText(Constants.TABLE_PAYMENT, Constants.FIELD_PAYMENT_VALID_FLG, Item.ValidFlg) %></td>
																<td align="center"><%#: Item.DisplayOrder %></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="8"></td>
													</tr>
												</table>
												<br />
												<table class="info_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr>
														<td align="left" class="info_item_bg" colspan="2">備考<br />
														決済種別は配送種別毎に利用可否を設定可能です。
														　※ただし、有効な決済種別のみ可能<br />
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td class="action_list_sp">
												<!--<asp:Button id="btnInsertBotttom" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" />-->
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
			</table>
		</td>
	</tr>
	<!--△ 一覧 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>