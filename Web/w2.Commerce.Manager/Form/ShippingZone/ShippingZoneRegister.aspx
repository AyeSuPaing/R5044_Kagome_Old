<%--
=========================================================================================================
  Module      : 特別配送先設定登録ページ(ShippingZoneRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ShippingZoneRegister.aspx.cs" Inherits="Form_ShippingZone_ShippingZoneRegister" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">特別配送先設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 登録 ▽-->
	<tr id="trEdit" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">特別配送先設定一覧編集</h2></td>
	</tr>
	<tr id="trRegister" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">特別配送先設定登録</h2></td>
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
															<td class="edit_title_bg" align="center" colspan="10">基本情報</td>
														</tr>
														<tr>
															<td class="edit_item_bg" align="center" colspan="10">対象郵便番号を複数入力する場合はカンマ「,」で区切って入力してください。
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="20%">配送種別<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left" colspan="9">
																<asp:DropDownList id="ddlName" runat="server" SelectedValue='<%# m_htParam[Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ID] %>'
																 OnSelectedIndexChanged="ddlName_OnSelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="20%">地帯区分<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left" colspan="9"><asp:TextBox id="tbShippingZoneNo" runat="server" Text="<%# m_htParam[Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NO] %>" MaxLength="9" Width="70"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="20%">地帯名<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left" colspan="9"><asp:TextBox id="tbName" runat="server" Text="<%# m_htParam[Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NAME] %>" MaxLength="30" Width="250"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="20%">郵便番号(<span class="notice">※</span>「-」は不要)<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left" colspan="9">
																<asp:TextBox ID="tbZip" TextMode="MultiLine" Rows="3" Width="420" runat="server" Text="<%# m_htParam[Constants.FIELD_SHOPSHIPPINGZONE_ZIP] %>"></asp:TextBox><br/>
																特別配送先かつ、配送不可エリアを郵便番号で指定することが可能です。
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="20%">配送不可エリア指定<br/>配送サービス</td>
															<td class="edit_item_bg" id="tdUnavailableShippingDelivaryCompany" align="left" colspan="9">
																<asp:Repeater ID="rDeliveryCompanyName" runat="server" DataSource ="<%# m_htParam[DELIVERY_ZONE_PRICES] %>" ItemType="w2.Domain.ShopShipping.ShopShippingZoneModel">
																	<ItemTemplate>
																		<asp:CheckBox ID="cbUnavailableShippingDelivaryCompanyName" Text="<%#: GetDeliveryCompanyName(Item.DeliveryCompanyId) %>" runat="server" Checked='<%# Item.IsUnavailableShippingAreaFlg %>' /><br/>
																	</ItemTemplate>
																</asp:Repeater>
																設定した郵便番号を配送不可エリアとする配送サービスを選択することが可能です。
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="20%">配送サービス／サイズ重量区分</td>
															<td class="edit_title_bg" align="center" width="8%">メール</td>
															<td class="edit_title_bg" align="center" width="8%">XXS</td>
															<td class="edit_title_bg" align="center" width="8%">XS</td>
															<td class="edit_title_bg" align="center" width="8%">S</td>
															<td class="edit_title_bg" align="center" width="8%">M</td>
															<td class="edit_title_bg" align="center" width="8%">L</td>
															<td class="edit_title_bg" align="center" width="8%">XL</td>
															<td class="edit_title_bg" align="center" width="8%">XXL</td>
															<td class="edit_title_bg" align="left" width="16%">配送料条件（サイズ量に関わらず一律設定）</td>
														</tr>
													<asp:Repeater ID="rDeliveryZonePrices" runat="server" DataSource ="<%# m_htParam[DELIVERY_ZONE_PRICES] %>" ItemType="w2.Domain.ShopShipping.ShopShippingZoneModel">
													<ItemTemplate>
														<tr>
															<td class="edit_title_bg" align="left" width="20%"><%#: GetDeliveryCompanyName(Item.DeliveryCompanyId) %></td>
															<td class="edit_item_bg" runat="server" align="center" colspan="9" Visible='<%# Item.IsUnavailableShippingAreaFlg %>'>配送不可</td>
															<div runat="server" Visible='<%# Item.IsUnavailableShippingAreaFlg == false %>'>
																<td class="edit_item_bg" id="tdSizeMailShippingPrice" align="right"><%#: Item.SizeMailShippingPrice.ToPriceString(true) %></td>
																<td class="edit_item_bg" id="tdSizeXxsShippingPrice" align="right"><%#: Item.SizeXxsShippingPrice.ToPriceString(true) %></td>
																<td class="edit_item_bg" id="tdSizeXsShippingPrice" align="right"><%#: Item.SizeXsShippingPrice.ToPriceString(true) %></td>
																<td class="edit_item_bg" id="tdSizeSShippingPrice" align="right"><%#: Item.SizeSShippingPrice.ToPriceString(true) %></td>
																<td class="edit_item_bg" id="tdSizeMShippingPrice" align="right"><%#: Item.SizeMShippingPrice.ToPriceString(true) %></td>
																<td class="edit_item_bg" id="tdSizeLShippingPrice" align="right"><%#: Item.SizeLShippingPrice.ToPriceString(true) %></td>
																<td class="edit_item_bg" id="tdSizeXlShippingPrice" align="right"><%#: Item.SizeXlShippingPrice.ToPriceString(true) %></td>
																<td class="edit_item_bg" id="tdSizeXXlShippingPrice" align="right"><%#: Item.SizeXxlShippingPrice.ToPriceString(true)%></td>
																<td class="edit_item_bg" id="tdConditionalShippingPrice" align="left" width="16%">
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
