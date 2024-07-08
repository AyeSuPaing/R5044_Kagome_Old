<%--
=========================================================================================================
  Module      : 配送料情報確認ページ(ShippingConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ShippingConfirm.aspx.cs" Inherits="Form_Shipping_ShippingConfirm" %>
<%@ Import Namespace="w2.Domain.ShopShipping" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">配送種別設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 詳細 ▽-->
	<tr id="trDetailTop" runat="server" Visible="True">
		<td><h2 class="cmn-hed-h2">配送種別設定詳細</h2></td>
	</tr>
	<tr id="trConfirmTop" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">配送種別設定入力確認</h2></td>
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
													<asp:Button ID="btnBackTop" runat="server" Text="  戻る  " OnClick="btnBack_Click" />
													<asp:Button id="btnEditTop" runat="server" Visible="False" Text="  編集する  " OnClick="btnEdit_Click" />
													<asp:Button id="btnCopyInsertTop" runat="server" Visible="False" Text="  コピー新規登録する  " OnClick="btnCopyInsert_Click" />
													<asp:Button id="btnDeleteTop" runat="server" Visible="False" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
													<asp:Button id="btnInsertTop" runat="server" Visible="False" Text="  登録する  " OnClick="btnInsert_Click" />
													<asp:Button id="btnUpdateTop" runat="server" Visible="False" Text="  更新する  " OnClick="btnUpdate_Click" /></div>
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">基本情報</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">配送種別ID</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_SHOPSHIPPING_SHIPPING_ID]) %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">配送種別</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_SHOPSHIPPING_SHOP_SHIPPING_NAME])%></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">配送拠点</td>
														<td class="detail_item_bg" align="left"><%#: this.shippingBaseName %></td>
													</tr>
													<tr id="trDateCreated" runat="server" Visible="False">
														<td class="detail_title_bg" align="left" width="30%">作成日</td>
														<td class="detail_item_bg" align="left"><%#: DateTimeUtility.ToStringForManager(m_htParam[Constants.FIELD_SHOPSHIPPING_DATE_CREATED], DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
													</tr>
													<tr id="trDateChanged" runat="server" Visible="False">
														<td class="detail_title_bg" align="left" width="30%">更新日</td>
														<td class="detail_item_bg" align="left"><%#: DateTimeUtility.ToStringForManager(m_htParam[Constants.FIELD_SHOPSHIPPING_DATE_CHANGED], DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
													</tr>
													<tr id="trLastChanged" runat="server" Visible="False">
														<td class="detail_title_bg" align="left" width="30%">最終更新者</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_SHOPSHIPPING_LAST_CHANGED]) %></td>
													</tr>
												</table>
												<br />
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">配送可能日付範囲情報</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">日付範囲設定の利用の有無</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_FLG, (string)m_htParam[Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_FLG])) %></td>
													</tr>
													<% if (IsShippingDateUsable){ %>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">日付範囲</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(CreateShippingDate((string)m_htParam[Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_FLG], StringUtility.ToEmpty(m_htParam[Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_BEGIN].ToString()),StringUtility.ToEmpty(m_htParam[Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_END]), StringUtility.ToEmpty(m_htParam[Constants.FIELD_SHOPSHIPPING_BUSINESS_DAYS_FOR_SHIPPING]))) %></td>
													</tr>
													<%} %>
												</table>
												<br />
												<%if (Constants.FIXEDPURCHASE_OPTION_ENABLED){ %>
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">定期購入配送パターン情報</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">定期購入の利用の有無</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FLG, (string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FLG]))%></td>
													</tr>
													<% if (IsFixedPurchaseUsable){ %>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">配送パターン</td>
														<td class="detail_item_bg" align="left">
															<%# WebSanitizer.HtmlEncode(CreateFixedPurchasePatternsString((string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FLG]))%>
															<%# WebSanitizer.HtmlEncode(CreateFixedPurchasePatternsString((string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FLG], (string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_FLG], Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN1_FLG_VALID, "月間隔日付指定"))%>
															<%#: CreateFixedPurchasePatternsString((string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FLG], (string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN2_FLG], Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN2_FLG_VALID, "月間隔・週・曜日指定")%>
															<%# WebSanitizer.HtmlEncode(CreateFixedPurchasePatternsString((string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FLG], (string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN3_FLG], Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN3_FLG_VALID, "配送日間隔指定"))%>
															<%# WebSanitizer.HtmlEncode(CreateFixedPurchasePatternsString((string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FLG], (string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN4_FLG], Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN4_FLG_VALID, "週間隔・曜日指定"))%>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">月間隔選択肢</td>
														<td class="detail_item_bg" align="left">
															<%#: CreateKbn1SettingMessage((string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FLG], (string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_FLG], (string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN2_FLG], (string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_SETTING])%>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">日付選択肢</td>
														<td class="detail_item_bg" align="left">
															<%#: CreateKbn1SettingMessage((string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FLG], (string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_FLG], (string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN2_FLG], (string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_SETTING2])%>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">配送間隔選択肢</td>
														<td class="detail_item_bg" align="left">
															<%# WebSanitizer.HtmlEncode(CreateKbn3SettingMessage((string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FLG], (string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN3_FLG], (string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN3_SETTING]))%>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">週間隔選択肢</td>
														<td class="detail_item_bg" align="left">
															<%# WebSanitizer.HtmlEncode(CreateKbn4Setting1Message((string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FLG], (string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN4_FLG], (string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN4_SETTING1]))%>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">曜日選択肢</td>
														<td class="detail_item_bg" align="left">
															<%# WebSanitizer.HtmlEncode(CreateKbn4Setting2Message((string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FLG], (string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN4_FLG], (string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN4_SETTING2]))%>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">配送キャンセル期限</td>
														<td class="detail_item_bg" align="left">
															次回配送日の　<%# WebSanitizer.HtmlEncode(CreateFixedPurchaseCommonMessage((string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FLG], m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_CANCEL_DEADLINE] + "日前"))%>　 （マイページでスキップ可能な期限／次回配送日変更可能期限）
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">自動注文タイミング</td>
														<td class="detail_item_bg" align="left">
															次回配送日の　<%# WebSanitizer.HtmlEncode(CreateFixedPurchaseCommonMessage((string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FLG], m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_ORDER_ENTRY_TIMING] + "日前"))%>　 （バッチ処理による自動注文登録）
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">配送所要日数</td>
														<td class="detail_item_bg" align="left">
															<%# WebSanitizer.HtmlEncode(CreateFixedPurchaseCommonMessage((string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FLG], m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SHIPPING_DAYS_REQUIRED] + "日"))%>　 （配送希望日が指定されないときの初回配送日計算に利用／次回配送日変更時に、今日から最短配送日までにあける日数）　※営業日は考慮しておりません
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">最低配送間隔</td>
														<td class="detail_item_bg" align="left">
															<%# WebSanitizer.HtmlEncode(CreateFixedPurchaseCommonMessage((string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FLG], m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_MINIMUM_SHIPPING_SPAN] + "日"))%>　 （今回の配送日と次回の配送日の間に最低限あける日数）
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">次回配送日の選択可能最大日数</td>
														<td class="detail_item_bg" align="left">
															次回配送日の　<%#: CreateFixedPurchaseCommonMessage((string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FLG], m_htParam[Constants.FIELD_SHOPSHIPPING_NEXT_SHIPPING_MAX_CHANGE_DAYS].ToString())%>日後まで　 （マイページで次回配送日変更時に選択可能な最大日数）
														</td>
													</tr>
													<% if (Constants.FIXED_PURCHASE_FIRST_SHIPPING_DATE_NEXT_MONTH_ENABLED) { %>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">初回配送日</td>
														<td class="detail_item_bg" align="left">
															<%#: ValueText.GetValueText(
																Constants.TABLE_SHOPSHIPPING,
																Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FIRST_SHIPPING_NEXT_MONTH_FLG,
																(string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FIRST_SHIPPING_NEXT_MONTH_FLG]) %>
														</td>
													</tr>
													<% } %>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">定期配送料無料フラグ</td>
														<td class="edit_item_bg" align="left">
															<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FREE_SHIPPING_FLG, (string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FREE_SHIPPING_FLG]))%>
														</td>
													</tr>
													<%} %>
												</table>
												<%} %>
												<% if (Constants.GIFTORDER_OPTION_ENABLED) { %>
												<br />
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">のし情報</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">のし設定の利用の有無</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_WRAPPING_PAPER_FLG, (string)m_htParam[Constants.FIELD_SHOPSHIPPING_WRAPPING_PAPER_FLG]))%></td>
													</tr>
													<% if (IsWrappingPaperUsable){ %>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">のしの種類</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncodeChangeToBr((string)m_htParam[Constants.FIELD_SHOPSHIPPING_WRAPPING_PAPER_FLG] == Constants.FLG_SHOPSHIPPING_WRAPPING_PAPER_FLG_VALID ?
															string.Join("\n", StringUtility.SplitCsvLine((string)m_htParam[Constants.FIELD_SHOPSHIPPING_WRAPPING_PAPER_TYPES])) : "-")%></td>
													</tr>
													<%} %>
												</table>
												<br />
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">包装情報</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">包装設定の利用の有無</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_WRAPPING_BAG_FLG, (string)m_htParam[Constants.FIELD_SHOPSHIPPING_WRAPPING_BAG_FLG]))%></td>
													</tr>
													<% if (IsWrappingBagUsable){ %>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">包装の種類</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncodeChangeToBr((string)m_htParam[Constants.FIELD_SHOPSHIPPING_WRAPPING_BAG_FLG] == Constants.FLG_SHOPSHIPPING_WRAPPING_BAG_FLG_VALID ?
															string.Join("\n", StringUtility.SplitCsvLine((string)m_htParam[Constants.FIELD_SHOPSHIPPING_WRAPPING_BAG_TYPES])) : "-")%></td>
													</tr>
													<%} %>
												</table>
												<% } %>
												<br />
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">決済種別</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">決済種別任意指定の利用の有無</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_PAYMENT_SELECTION_FLG, (string)m_htParam[Constants.FIELD_SHOPSHIPPING_PAYMENT_SELECTION_FLG]))%></td>
													</tr>
													<% if (IsPaymentSelectionUsable){ %>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">決済種別の種類</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncodeChangeToBr((string)m_htParam[Constants.FIELD_SHOPSHIPPING_PAYMENT_SELECTION_FLG] == Constants.FLG_SHOPSHIPPING_PAYMENT_SELECTION_FLG_VALID ?
															CreatePaymentNames((string)m_htParam[Constants.FIELD_SHOPSHIPPING_PERMITTED_PAYMENT_IDS]) : "-")%></td>
													</tr>
													<%} %>
												</table>
												<br />
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">配送サービス</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">宅配便</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(GetDeliveryCompanyString(GetExpressCompany(this.ShopShippingCompanyList))) %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">メール便</td>
														<td class="detail_item_bg" align="left"><%#: GetDeliveryCompanyString(GetMailCompany(this.ShopShippingCompanyList), Constants.DELIVERYCOMPANY_MAIL_ESCALATION_ENBLED) %></td>
													</tr>
												</table>
												<% if (Constants.SHIPPINGPRICE_SEPARATE_ESTIMATE_ENABLED){ %>
												<!-- ▽配送料別途見積もり表示利用する場合▽ -->
												<br />
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">配送料の別途見積もり情報</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">配送料の別途見積もり利用の有無</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG, (string)m_htParam[Constants.FIELD_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG]))%></td>
													</tr>
													<% if (this.IsShippingpriceSeparateEstimateUsable){ %>
														<tr>
															<td class="detail_title_bg" align="left" width="30%">表記文言</td>
															<td class="detail_item_bg" align="left"><%# (string)m_htParam[Constants.FIELD_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_MESSAGE]%></td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left" width="30%">表記文言（モバイル）</td>
															<td class="detail_item_bg" align="left"><%# (string)m_htParam[Constants.FIELD_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_MESSAGE_MOBILE]%></td>
														</tr>
													<%} %>
												</table>
												<!-- △配送料別途見積もり表示利用する場合△ -->
												<%} %>
												<div class="action_part_bottom"></div>
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
	<tr id="trDetailBottom" runat="server" Visible="True">
		<td><h2 class="cmn-hed-h2">配送サービスごとの配送料金設定詳細</h2></td>
	</tr>
	<tr id="trConfirmBottom" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">配送サービスごとの配送料金設定入力確認</h2></td>
	</tr>
	<tr id="trShippingPostage" runat="server" visible='<%# (this.IsShippingpriceSeparateEstimateUsable == false) %>'>
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
												<div class="action_part_top"></div>
												<asp:Repeater runat="server" DataSource="<%# (ShippingDeliveryPostageModel[])m_htParam[Constants.SESSIONPARAM_KEY_SHIPPINGDELIVERYPOSTAGE_INFO] %>"
													ItemType="w2.Domain.ShopShipping.ShippingDeliveryPostageModel" visible="<%# (this.SelectedDeliveryCompanyCount > 1) %>">
													<ItemTemplate>
														<div><a href="#<%#: Item.DeliveryCompanyId %>"><%#: GetDeliveryCompanyName(Item.DeliveryCompanyId) %></a></div>
													</ItemTemplate>
												</asp:Repeater>
												<asp:Repeater ID="rShippingPostage" runat="server" DataSource="<%# (ShippingDeliveryPostageModel[])m_htParam[Constants.SESSIONPARAM_KEY_SHIPPINGDELIVERYPOSTAGE_INFO] %>"
													ItemType="w2.Domain.ShopShipping.ShippingDeliveryPostageModel" OnItemCommand="rShippingPostage_ItemCommand">
												<ItemTemplate>
												<div runat="server" visible="<%# (this.SelectedDeliveryCompanyCount > 1) %>">
													<span id="<%#: Item.DeliveryCompanyId %>"></span><span id="a_idx_<%#:Container.ItemIndex %>"></span>
													<br />
												</div>
												<span runat="server" visible="<%# (Container.ItemIndex != 0) %>"><img height="15" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></span>
												<h2 class="cmn-hed-h2">
													■配送サービス：<%#: GetDeliveryCompanyName(Item.DeliveryCompanyId) %>
													　<span runat="server" visible="<%# Container.ItemIndex < this.SelectedDeliveryCompanyCount - 1 %>"><a href='#a_idx_<%#Container.ItemIndex + 1 %>'>↓</a></span>
													　<span runat="server" visible="<%# Container.ItemIndex > 0 %>"><a href='#a_idx_<%#Container.ItemIndex - 1 %>'>↑</a></span>
												</h2>
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center">配送料設定</td>
													</tr>
													<tr>
														<td class="detail_item_bg" align="center">
															<span visible="<%# (Item.ShippingPriceKbn == Constants.FLG_SHIPPING_PRICE_KBN_NONE) %>" runat="server">
																配送料無し
															</span>
															<span visible="<%# (Item.ShippingPriceKbn == Constants.FLG_SHIPPING_PRICE_KBN_SAME) %>" runat="server">
																配送料が全国一律
															</span>
															<span visible="<%# (Item.ShippingPriceKbn == Constants.FLG_SHIPPING_PRICE_KBN_AREA) %>" runat="server">
																配送料を地域別に設定
															</span>
														</td>
													</tr>
												</table>
												<br />
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">配送料無料購入金額情報</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">無料購入金額設定</td>
														<td class="detail_item_bg" align="left">
															<span visible="<%# (Item.ShippingFreePriceFlg == Constants.FLG_SHOPSHIPPING_SHIPPING_FREE_PRICE_FLG_VALID) %>" runat="server">
																１配送先の購入金額<%= (Constants.MANAGEMENT_INCLUDED_TAX_FLAG) ? "（税込総額）" : "（税抜総額）" %>が
																<%#: Item.ShippingFreePrice.ToPriceString(true) %>
																以上の場合は送料を無料にする（特別配送先を除く）。
															</span>
															<span visible="<%# (Item.ShippingFreePriceFlg == Constants.FLG_SHOPSHIPPING_SHIPPING_FREE_PRICE_FLG_INVALID) %>" runat="server">
																利用なし
															</span>
														</td>
													</tr>
													<tr visible="<%# (Item.ShippingFreePriceFlg == Constants.FLG_SHOPSHIPPING_SHIPPING_FREE_PRICE_FLG_VALID) %>" runat="server">
														<td class="detail_title_bg" align="left" width="30%">配送料無料案内表示の有無</td>
														<td class="detail_item_bg" align="left">
															<span visible="<%# (Item.AnnounceFreeShippingFlg == Constants.FLG_SHOPSHIPPING_ANNOUNCE_FREE_SHIPPING_FLG_VALID) %>" runat="server">
																配送料無料案内を表示する。
															</span>
															<span visible="<%# (Item.AnnounceFreeShippingFlg == Constants.FLG_SHOPSHIPPING_ANNOUNCE_FREE_SHIPPING_FLG_INVALID) %>" runat="server">
																配送料無料案内を表示しない。
															</span>
														</td>
													</tr>
													<% if (Constants.FREE_SHIPPING_FEE_OPTION_ENABLED) { %>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">配送料無料時の請求料金設定</td>
														<td class="detail_item_bg" align="left">
															<span>
																送料無料時に<%#: Item.FreeShippingFee.ToPriceString(true) %>の送料を請求する(特別配送先を除く)
															</span>
														</td>
													</tr>
													<% } %>
												</table>
												<% if (Constants.STORE_PICKUP_OPTION_ENABLED) { %>
												<br />
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">店舗受取時配送料</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">店舗受取時配送料無料設定</td>
														<td class="detail_item_bg" align="left">
															<span><%#: (Item.StorePickupFreePriceFlg == Constants.FLG_ON) ? "利用する" : "利用なし" %></span>
														</td>
													</tr>
												</table>
												<% } %>
												<br />
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">複数商品計算方法情報</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">複数商品計算設定</td>
														<td class="detail_item_bg" align="left">
															<span visible="<%# (Item.CalculationPluralKbn == Constants.FLG_SHOPSHIPPING_CALCULATION_PLURAL_KBN_SUM_OF_PRODUCT_SHIPPING_PRICE) %>" runat="server">
																利用なし （商品１個ずつに送料がかかる。）
															</span>
															<span visible="<%# (Item.CalculationPluralKbn != Constants.FLG_SHOPSHIPPING_CALCULATION_PLURAL_KBN_SUM_OF_PRODUCT_SHIPPING_PRICE) %>" runat="server">
																最も<% if (Constants.SHIPPINGPRIORITY_SETTING == Constants.ShippingPriority.HIGH) { %>高い<%} else {%>低い<%}%>送料１点＋
																<%#: Item.PluralShippingPrice.ToPriceString(true) %>
																／個の送料を設定する。
															</span>
														</td>
													</tr>
												</table>
												<%if (this.IsShippingCountryAvailableJp || this.IsShippingCountryAvailableTw) { %>
												<br />
												<table cellspacing="0" cellpadding="0" border="0" runat="server">
													<tr>
														<td>
															<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
																<tbody>
																<tr>
																	<td class="detail_title_bg" align="center" colspan="10">各エリア配送料情報</td>
																</tr>
																<tr>
																	<td class="detail_title_bg" align="left" width="20%">エリア／サイズ重量区分</td>
																	<td class="detail_title_bg" align="center" width="8%">メール</td>
																	<td class="detail_title_bg" align="center" width="8%">XXS</td>
																	<td class="detail_title_bg" align="center" width="8%">XS</td>
																	<td class="detail_title_bg" align="center" width="8%">S</td>
																	<td class="detail_title_bg" align="center" width="8%">M</td>
																	<td class="detail_title_bg" align="center" width="8%">L</td>
																	<td class="detail_title_bg" align="center" width="8%">XL</td>
																	<td class="detail_title_bg" align="center" width="8%">XXL</td>
																	<td class="detail_title_bg" align="left" width="16%">配送料条件（サイズ量に関わらず一律設定）</td>
																</tr>
																<asp:Repeater id="rShippingZone" Runat="server"
																	DataSource="<%# GetShippingZonesByDeliveryCompany((ShopShippingZoneModel[])m_htParam[Constants.SESSIONPARAM_KEY_SHIPPINGZONE_INFO], Item.DeliveryCompanyId) %>"
																	ItemType="w2.Domain.ShopShipping.ShopShippingZoneModel">
																	<ItemTemplate>
																		<tr>
																			<td class="detail_title_bg" align="left" width="20%"><%#: Item.ShippingZoneName %></td>
																			<td class="edit_item_bg" runat="server" align="center" colspan="9" Visible='<%# Item.IsUnavailableShippingAreaFlg %>'>配送不可</td>
																			<div runat="server" Visible='<%# Item.IsUnavailableShippingAreaFlg == false %>'>
																				<td class="detail_item_bg" align="right" width="8%"><%#: Item.SizeMailShippingPrice.ToPriceString(true) %></td>
																				<td class="detail_item_bg" align="right" width="8%"><%#: Item.SizeXxsShippingPrice.ToPriceString(true) %></td>
																				<td class="detail_item_bg" align="right" width="8%"><%#: Item.SizeXsShippingPrice.ToPriceString(true) %></td>
																				<td class="detail_item_bg" align="right" width="8%"><%#: Item.SizeSShippingPrice.ToPriceString(true) %></td>
																				<td class="detail_item_bg" align="right" width="8%"><%#: Item.SizeMShippingPrice.ToPriceString(true) %></td>
																				<td class="detail_item_bg" align="right" width="8%"><%#: Item.SizeLShippingPrice.ToPriceString(true) %></td>
																				<td class="detail_item_bg" align="right" width="8%"><%#: Item.SizeXlShippingPrice.ToPriceString(true) %></td>
																				<td class="detail_item_bg" align="right" width="8%"><%#: Item.SizeXxlShippingPrice.ToPriceString(true) %></td>
																				<td class="detail_item_bg" align="left" width="16%">
																					<span visible="<%# (Item.ConditionalShippingPriceThreshold != null) %>" runat="server">
																						<%#: Item.ConditionalShippingPriceThreshold.ToPriceString(true) %>
																						以上の購入で
																						<br/><%#: Item.ConditionalShippingPrice.ToPriceString(true) %>
																						の配送料
																					</span>
																					<span visible="<%# (Item.ConditionalShippingPriceThreshold == null) %>" runat="server">
																						利用なし
																					</span>
																				</td>
																			</div>
																		</tr>
																	</ItemTemplate>
																</asp:Repeater>
																</tbody>
															</table>
														</td>
													</tr>
												</table>
												<% } %>
												<% if (Constants.GLOBAL_OPTION_ENABLE && this.IsActionDetail) { %>
												<br />
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td><h2 class="cmn-hed-h2">配送エリア／重量別配送料金表【<%#: GetDeliveryCompanyName(Item.DeliveryCompanyId) %>】</h2>
															<asp:Button runat="server" ID="btnEditGlobalShippingPostage" CommandName="EditGlobalShippingPostage" CommandArgument="<%# Item.DeliveryCompanyId %>" Text="海外送料表を編集する" />
														</td>
													</tr>
													<tr>
														<td>
															<table cellspacing="1" cellpadding="3" border="0" width="480">
																<tr>
																	<td>
																		<asp:Repeater id="repGlobalShippingArea" Runat="server" DataSource="<%# m_htParam[Constants.SESSIONPARAM_KEY_GLOBALSHIPPING_AREA_POSTAGE] %>" ItemType="GlobalPostageMap">
																			<ItemTemplate>
																				<h2 class="cmn-hed-h2">
																					<%#:Item.GlobalShippingAreaName %>
																				</h2>
																				<table cellspacing="1" cellpadding="3" width="300" border="0" class="detail_table">
																					<tr>
																						<td align="left" class="detail_title_bg" width="20%">重量（g以上）～重量（g未満）</td>
																						<td align="left" class="detail_title_bg" width="80%">送料</td>
																					</tr>
																					<asp:Repeater id="repGlobalShippingAreaPostage" Runat="server"
																						DataSource="<%# GetGlobalShippingPostageByDeliveryCompany(Item.Postage, ((ShippingDeliveryPostageModel)((RepeaterItem)Container.Parent.Parent).DataItem).DeliveryCompanyId) %>"
																						ItemType="w2.Domain.GlobalShipping.GlobalShippingPostageModel">
																						<ItemTemplate>
																							<tr>
																								<td class="detail_item_bg" align="left"><%#: StringUtility.ToNumeric(Item.WeightGramGreaterThanOrEqualTo) %>g～<%#:StringUtility.ToNumeric(Item.WeightGramLessThan) %>g</td>
																								<td class="detail_item_bg" align="left"><%#: GlobalPriceDisplayControl(Item.GlobalShippingPostage) %></td>
																							</tr>
																						</ItemTemplate>
																					</asp:Repeater>
																				</table>
																			</ItemTemplate>
																		</asp:Repeater>
																	</td>
																</tr>
															</table>
														</td>
													</tr>
												</table>
												<% } %>
												</ItemTemplate>
												</asp:Repeater>
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
		<td>
			<div class="action_part_bottom">
				<asp:Button ID="btnBackBottom" runat="server" Text="  戻る  " OnClick="btnBack_Click" />
				<asp:Button id="btnEditBottom" runat="server" Visible="False" Text="  編集する  " OnClick="btnEdit_Click" />
				<asp:Button id="btnCopyInsertBottom" runat="server" Visible="False" Text="  コピー新規登録する  " OnClick="btnCopyInsert_Click" />
				<asp:Button id="btnDeleteBottom" runat="server" Visible="False" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
				<asp:Button id="btnInsertBottom" runat="server" Visible="False" Text="  登録する  " OnClick="btnInsert_Click" />
				<asp:Button id="btnUpdateBottom" runat="server" Visible="False" Text="  更新する  " OnClick="btnUpdate_Click" /></div>
		</td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
