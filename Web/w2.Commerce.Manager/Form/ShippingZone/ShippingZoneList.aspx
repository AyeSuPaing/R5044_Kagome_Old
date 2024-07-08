<%--
=========================================================================================================
  Module      : 特別配送先設定一覧ページ(ShippingZoneList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ShippingZoneList.aspx.cs" Inherits="Form_ShippingZone_ShippingZoneList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">特別配送先設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 検索 ▽-->
	<tr>
		<td colspan="2">
			<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td class="search_box_bg">
						<table cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td class="search_table">
												<table cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="search_title_bg" width="130">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />配送種別</td>
														<td class="search_item_bg">
															<asp:DropDownList id="ddlSearchKey" runat="server" OnSelectedIndexChanged="btnSearch_Click" AutoPostBack="true">
																<asp:ListItem Selected="True"></asp:ListItem>
															</asp:DropDownList></td>
														<td class="search_btn_bg" width="83" rowspan="4">
															<div class="search_btn_main"><asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub"><a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_SHIPPING_ZONE_LIST %>">クリア</a></div>
														</td>

													</tr>
												</table>
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
	<!--△ 検索 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">特別配送先設定一覧</h2></td>
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
														<td class="action_list_sp"><asp:Button id="btnInsertTop" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" /></td>
													</tr>
												</table>
												<!-- ページング-->
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<asp:Repeater id="rList" Runat="server" ItemType="ShippingZonePrices">
														<HeaderTemplate>
															<tr class="list_title_bg item_no_padding">
																<td align="center" width="7%" class="item_no_padding">配送種別ID</td>
																<td align="center" width="15%" class="item_no_padding">配送種別</td>
																<td align="center" width="5%" class="item_no_padding">地帯区分</td>
																<td align="center" width="15%" class="item_no_padding">地帯名</td>
																<td width="58%" class="item_no_padding">
																	<table cellpadding="0" cellspacing="0" width="100%">
																		<tr>
																			<td align="center" width="18%" class="item_bottom_right_line">配送サービス</td>
																			<td align="center" width="9%" class="item_bottom_right_line">メール</td>
																			<td align="center" width="9%" class="item_bottom_right_line">XXS</td>
																			<td align="center" width="9%" class="item_bottom_right_line">XS</td>
																			<td align="center" width="9%" class="item_bottom_right_line">S</td>
																			<td align="center" width="9%" class="item_bottom_right_line">M</td>
																			<td align="center" width="9%" class="item_bottom_right_line">L</td>
																			<td align="center" width="9%" class="item_bottom_right_line">XL</td>
																			<td align="center" width="9%" class="item_bottom_right_line">XXL</td>
																			<td align="center" width="10%" class="item_bottom_right_line">配送料条件</td>
																		</tr>
																	</table>
																</td>
															</tr>
														</HeaderTemplate>
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>"
																onmouseover="listselect_mover(this)"
																onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)"
																onmousedown="listselect_mdown(this)"
																onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateShippingZoneDetailUrl(Item.ShippingId, Item.ShippingZoneNo.ToString())) %>')">
																<td align="center"><%#: Item.ShippingId %></td>
																<td align="left"><%#: Item.ShopShippingName %></td>
																<td align="center"><%#: Item.ShippingZoneNo %></td>
																<td align="left"><%#: Item.ShippingZoneName %></td>
																<td style="padding: 0;">
																<table cellpadding="0" cellspacing="0" width="100%">
																<asp:Repeater ID="rDeliveryZoneList" Runat="server" DataSource="<%# Item.DeliveryZonePriceList %>" ItemType="w2.Domain.ShopShipping.ShopShippingZoneModel">
																	<ItemTemplate>
																	<tr>
																		<td align="left" width="18%" class="item_bottom_right_line"><%#: GetDeliveryCompanyName(Item.DeliveryCompanyId) %></td>
																		<td class="item_bottom_right_line" runat="server" align="center" colspan="9" Visible='<%# Item.IsUnavailableShippingAreaFlg %>'>配送不可</td>
																		<div runat="server" Visible='<%# Item.IsUnavailableShippingAreaFlg == false %>'>
																			<td align="right" width="9%" class="item_bottom_right_line"><%#: Item.SizeMailShippingPrice.ToPriceString(true) %></td>
																			<td align="right" width="9%" class="item_bottom_right_line"><%#: Item.SizeXxsShippingPrice.ToPriceString(true) %></td>
																			<td align="right" width="9%" class="item_bottom_right_line"><%#: Item.SizeXsShippingPrice.ToPriceString(true) %></td>
																			<td align="right" width="9%" class="item_bottom_right_line"><%#: Item.SizeSShippingPrice.ToPriceString(true) %></td>
																			<td align="right" width="9%" class="item_bottom_right_line"><%#: Item.SizeMShippingPrice.ToPriceString(true) %></td>
																			<td align="right" width="9%" class="item_bottom_right_line"><%#: Item.SizeLShippingPrice.ToPriceString(true) %></td>
																			<td align="right" width="9%" class="item_bottom_right_line"><%#: Item.SizeXlShippingPrice.ToPriceString(true) %></td>
																			<td align="right" width="9%" class="item_bottom_right_line"><%#: Item.SizeXxlShippingPrice.ToPriceString(true) %></td>
																			<td align="center" width="10%" class="item_bottom_right_line">
																				<%#: ValueText.GetValueText(Constants.TABLE_SHOPSHIPPINGZONE, Constants.FIELD_SHOPSHIPPINGZONE_CONDITIONAL_SHIPPING_PRICE_THRESHOLD, (Item.ConditionalShippingPriceThreshold != null) ? "1" : "0")%>
																			</td>
																		</div>
																	</tr>
																	</ItemTemplate>
																</asp:Repeater>
																</table>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="12"></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td class="action_list_sp"><asp:Button id="btnInsertBottom" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" /></td>
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
