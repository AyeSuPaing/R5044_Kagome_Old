<%--
=========================================================================================================
  Module      : 特別配送先設定確認ページ(ShippingZoneConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ShippingZoneConfirm.aspx.cs" Inherits="Form_ShippingZone_ShippingZoneConfirm" %>
<%@ Import Namespace="w2.App.Common.LohacoCreatorWebApi.OrderInfo" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">特別配送先設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 詳細 ▽-->
	<tr id="trDetail" runat="server" Visible="True">
		<td><h2 class="cmn-hed-h2">特別配送先設定詳細</h2></td>
	</tr>
	<tr id="trConfirm" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">特別配送先設定入力確認</h2></td>
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
													<asp:Button id="btnEditTop" runat="server" Text="  編集する  " Visible="False" OnClick="btnEdit_Click" />
													<asp:Button id="btnCopyInsertTop" runat="server" Text="  コピー新規登録する  " Visible="False" OnClick="btnCopyInsert_Click" />
													<asp:Button id="btnDeleteTop" runat="server" Text="  削除する  " Visible="False" OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
													<asp:Button id="btnInsertTop" runat="server" Text="  登録する  " Visible="False" OnClick="btnInsert_Click" />
													<asp:Button id="btnUpdateTop" runat="server" Text="  更新する  " Visible="False" OnClick="btnUpdate_Click" /></div>
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="10">基本情報</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="20%">配送種別</td>
														<td class="detail_item_bg" align="left" colspan="9"><%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_SHOPSHIPPING_SHOP_SHIPPING_NAME]) %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="20%">地帯区分</td>
														<td class="detail_item_bg" align="left" colspan="9"><%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NO]) %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="20%">地帯名</td>
														<td class="detail_item_bg" align="left" colspan="9"><%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NAME]) %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="20%">郵便番号</td>
														<td class="detail_item_bg" align="left" colspan="9" style="word-break:break-all"><%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_SHOPSHIPPINGZONE_ZIP]) %></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="20%">配送不可エリア指定<br/>配送サービス</td>
														<td class="edit_item_bg" id="tdUnavailableShippingDelivaryCompany" align="left" colspan="9">
															<asp:Literal ID="lNotDelivaryCompanyNames" runat="server" />
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="20%">配送サービス／サイズ重量区分</td>
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
													<asp:Repeater ID="rDeliveryZonePrices" runat="server" DataSource ="<%# m_htParam[DELIVERY_ZONE_PRICES] %>" ItemType="w2.Domain.ShopShipping.ShopShippingZoneModel">
													<ItemTemplate>
													<tr>
														<td class="detail_title_bg" align="left" width="20%"><%#: GetDeliveryCompanyName(Item.DeliveryCompanyId) %></td>
														<td class="edit_item_bg" runat="server" align="center" colspan="9" Visible='<%# Item.IsUnavailableShippingAreaFlg %>'>配送不可</td>
														<div runat="server" Visible='<%# Item.IsUnavailableShippingAreaFlg == false %>'>
															<td class="detail_item_bg" align="right" width="8%"><%#: Item.SizeMailShippingPrice.ToPriceString(true) %></td>
															<td class="detail_item_bg" align="right" width="8%"><%#: Item.SizeXxsShippingPrice.ToPriceString(true) %></td>
															<td class="detail_item_bg" align="right" width="8%"><%#: Item.SizeXsShippingPrice.ToPriceString(true) %></td>
															<td class="detail_item_bg" align="right" width="8%"><%#: Item.SizeSShippingPrice.ToPriceString(true) %></td>
															<td class="detail_item_bg" align="right" width="8%"><%#: Item.SizeMShippingPrice.ToPriceString(true) %></td>
															<td class="detail_item_bg" align="right" width="8%"><%#: Item.SizeLShippingPrice.ToPriceString(true) %></td>
															<td class="detail_item_bg" align="right" width="8%"><%#: Item.SizeXlShippingPrice.ToPriceString(true) %></td>
															<td class="detail_item_bg" align="right" width="8%"><%#: Item.SizeXxlShippingPrice.ToPriceString(true)%></td>
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
													<tr id="trDateChanged" runat="server" Visible="False">
														<td class="detail_title_bg" align="left" width="20%">更新日</td>
														<td class="detail_item_bg" align="left" colspan="9"><%#: DateTimeUtility.ToStringForManager(m_htParam[Constants.FIELD_SHOPSHIPPINGZONE_DATE_CHANGED], DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
													</tr>
													<tr id="trLastChanged" runat="server" Visible="False">
														<td class="detail_title_bg" align="left" width="20%">最終更新者</td>
														<td class="detail_item_bg" align="left" colspan="9"><%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_SHOPSHIPPINGZONE_LAST_CHANGED]) %></td>
													</tr>
												</table>
												<div class="action_part_bottom"><input type="button" onclick="Javascript:history.back();" value="  戻る  " />
													<asp:Button id="btnEditBottom" runat="server" Text="  編集する  " Visible="False" OnClick="btnEdit_Click" />
													<asp:Button id="btnCopyInsertBottom" runat="server" Text="  コピー新規登録する  " Visible="False" OnClick="btnCopyInsert_Click" />
													<asp:Button id="btnDeleteBottom" runat="server" Text="  削除する  " Visible="False" OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
													<asp:Button id="btnInsertBottom" runat="server" Text="  登録する  " Visible="False" OnClick="btnInsert_Click" />
													<asp:Button id="btnUpdateBottom" runat="server" Text="  更新する  " Visible="False" OnClick="btnUpdate_Click" /></div>
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
