<%--
=========================================================================================================
  Module      : 注文同梱ページ(OrderCombine.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OrderCombine.aspx.cs" Inherits="Form_OrderCombine_OrderCombine" MaintainScrollPositionOnPostback="true" %>
<%@ Register TagPrefix="w2cm" Assembly="w2.App.Common" Namespace="w2.App.Common.Web.WebCustomControl" %>
<%@ Import Namespace="w2.App.Common.Order" %>
<%@ Import Namespace="w2.App.Common.Order.OrderCombine" %>
<%@ Import Namespace="w2.App.Common.Order.FixedPurchaseCombine" %>
<%@ Import Namespace="w2.Domain.Order" %>
<%@ Import Namespace="w2.App.Common.Global" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<table cellspacing="0" cellpadding="0" width="791" border="0">
		<tr>
			<td><h1 class="page-title">注文同梱</h1></td>
		</tr>
		<tr>
			<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
		</tr>
		<!--▽ 検索 ▽-->
		<tr>
			<td>
				<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
					<tr>
						<td class="search_box_bg">
							<table cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td align="center">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td>
													<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td class="search_table">
													<table cellspacing="1" cellpadding="2" width="758" border="0">
														<tr>
															<td class="search_title_bg">
																<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
																ユーザーID
															</td>
															<td class="search_item_bg">
																<asp:TextBox runat="server" ID="tbUserId" />
															</td>
															<td class="search_title_bg">
																<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
																ユーザー名
															</td>
															<td class="search_item_bg">
																<asp:TextBox runat="server" ID="tbUserName" />
															</td>
															<td class="search_btn_bg" width="83" rowspan="4">
																<div class="search_btn_main"><asp:Button runat="server" ID="bSearch" Text="　検索　" OnClick="bSearch_Click" /></div>
																<div class="search_btn_sub">
																	<a href="<%: Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERCOMBINE_ORDER_COMBINE %>">クリア</a>
																	<a href="javascript:document.<%: this.Form.ClientID %>.reset();">リセット</a>
																</div>
															</td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
		<tr>
			<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
		</tr>
		<!--▽ 一覧 ▽-->
		<tr>
			<td><h2 class="cmn-hed-h2">注文同梱対象一覧</h2></td>
		</tr>
		<tr>
			<td>
				<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
					<tr>
						<td>
							<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td>
										<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
										<tr>
											<td width="675" style="height: 22px"><asp:Label ID="lbPager" runat="server" /></td>
										</tr>
										</table>
									</td>
								</tr>
								<tr>
									<td align="center">
										<div id="hgcCombineList" runat="server">
											<asp:HiddenField ID="hfSelectedOrderCombineParentOrderId" runat="server" />
											<table class="list_table" cellspacing="1" cellpadding="1" width="750">
											<tr class="list_title_bg">
												<td width="17">親指定</td>
												<td colspan="2" style="text-align: center">注文ID　|　注文区分　|　決済種別　|　注文日時</td>
											</tr>
											<tr id="hgcErrorMessageRow" class="list_alert" runat="server">
												<td colspan="3"><asp:Literal id="lErrorMessage" runat="server"></asp:Literal></td>
											</tr>
											<asp:Repeater runat="server" ID="rOrderCombineParentOrder" ItemType="w2.Domain.Order.OrderModel">	
												<ItemTemplate>
													<tr class="list_item_bg<%#: Container.ItemIndex % 2 + 1 %>">
														<td width="17">
															<w2cm:RadioButtonGroup ID="rbgSelectedOrderCombineParentOrder" GroupName="SelectedOrderCombineParentOrder" Name="rbgSelectedOrderCombineParentOrder"
																OnCheckedChanged="rbgSelectedOrderCombineParentOrder_CheckedChanged" Checked="<%# (hfSelectedOrderCombineParentOrderId.Value == Item.OrderId) %>" AutoPostBack="true" runat="server" />
															<asp:HiddenField ID="hfOrderCombineParentOrderId" Value="<%#: Item.OrderId %>" runat="server" />
														</td>
														<td width="38"><label for='<%#: Container.FindControl("rbgSelectedOrderCombineParentOrder").ClientID %>'><%#: Container.ItemIndex + 1 %></label></td>
														<td>
															<table width="100%" cellspacing="0" cellpadding="1" border="1">
															<tr>
																<td colspan="2">
																	<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_ORDER_CONFIRM)) { %>
																		<a href="javascript:open_window('<%# WebSanitizer.UrlAttrHtmlEncode(OrderPage.CreateOrderDetailUrl(Item.OrderId, true, false, "order_combine")) %>','ordercontact','width=828,height=600,top=110,left=380,status=NO,scrollbars=yes');"><%#: Item.OrderId %></a>
																	<% } else { %>
																		<%#: Item.OrderId %>
																	<% } %>
																	<label for='<%#: Container.FindControl("rbgSelectedOrderCombineParentOrder").ClientID %>'>
																	　|　<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_KBN, Item.OrderKbn) %>
																	　|　<%#: Item.PaymentName %>
																	　|　<%#: DateTimeUtility.ToStringForManager(Item.OrderDate, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %>
																	<br />
																	<div style="text-align: right;">
																		（合計金額：<%#: Item.OrderPriceTotal.ToPriceString(true) %>-
																		　注文ステータス：<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_STATUS, Item.OrderStatus) %>
																		　入金ステータス：<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, Item.OrderPaymentStatus) %>）
																	</div>
																	</label>
																</td>
															</tr>
															<tr>
																<td width="25%"><label for='<%#: Container.FindControl("rbgSelectedOrderCombineParentOrder").ClientID %>'>配送先</label></td>
																<td>
																	<label for='<%# Container.FindControl("rbgSelectedOrderCombineParentOrder").ClientID %>'>
																		<%#:  IsCountryJp(Item.Shippings[0].ShippingCountryIsoCode)
																			? "〒 " + Item.Shippings[0].ShippingZip + "　" + Item.Shippings[0].ShippingAddr1 + Item.Shippings[0].ShippingAddr2 + Item.Shippings[0].ShippingAddr3 + Item.Shippings[0].ShippingAddr4 + Item.Shippings[0].ShippingCountryName
																			: string.Format("{0} {1} {2} {3} {4} {5}"
																				, Item.Shippings[0].ShippingAddr2
																				, Item.Shippings[0].ShippingAddr3
																				, Item.Shippings[0].ShippingAddr4
																				, Item.Shippings[0].ShippingAddr5
																				, Item.Shippings[0].ShippingZip
																				, Item.Shippings[0].ShippingCountryName) %><br />
																	<%#: Item.Shippings[0].ShippingName %>
																	</label>
																</td>
															</tr>
															<tr>
																<td><label for='<%#: Container.FindControl("rbgSelectedOrderCombineParentOrder").ClientID %>'>配送日時</label></td>
																<td>
																	<label for='<%#: Container.FindControl("rbgSelectedOrderCombineParentOrder").ClientID %>'>
																	希望日：<%#: DateTimeUtility.ToStringForManager(Item.Shippings[0].ShippingDate, DateTimeUtility.FormatType.ShortDate2Letter, "―") %>
																	　|　希望時間帯：<%#: GetShippingTimeMessage(Item.Shippings[0].DeliveryCompanyId, Item.Shippings[0].ShippingTime) %>
																	</label>
																</td>
															</tr>
															<tr>
																<td><label for='<%#: Container.FindControl("rbgSelectedOrderCombineParentOrder").ClientID %>'>商品明細</label></td>
																<td>
																	<asp:Repeater runat="server" ID="rOrderCombineParentOrderProduct" DataSource="<%# Item.Shippings[0].Items %>" ItemType="w2.Domain.Order.OrderItemModel">
																	<ItemTemplate>
																		<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_PRODUCT_CONFIRM)) { %>
																			<a href="javascript:open_window('<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Item.ProductId, true)) %>','productcontact','width=828,height=615,top=120,left=320,status=NO,scrollbars=yes');"><%#: Item.ProductId %></a>
																		<% } else { %>
																			<%#: Item.ProductId %>
																		<% } %>
																		<label for='<%#: Container.Parent.Parent.FindControl("rbgSelectedOrderCombineParentOrder").ClientID %>'>
																		　<%#: Item.ProductName %>
																		<br />
																		</label>
																	</ItemTemplate>
																	</asp:Repeater>
																</td>
															</tr>
															<tr>
																<td><label for='<%#: Container.FindControl("rbgSelectedOrderCombineParentOrder").ClientID %>'>定期ID　｜　配送周期</label></td>
																<td>
																	<label for='<%#: Container.FindControl("rbgSelectedOrderCombineParentOrder").ClientID %>'>
																	<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_FIXEDPURCHASE_CONFIRM)) { %>
																			定期ID：<a href="javascript:open_window('<%# WebSanitizer.UrlAttrHtmlEncode(FixedPurchasePage.CreateFixedPurchaseDetailUrl(Item.FixedPurchaseId, true)) %>','ordercontact','width=828,height=600,top=110,left=380,status=NO,scrollbars=yes');"><%#: Item.FixedPurchaseId %></a>
																	<% } else { %>
																			定期ID：<%#: Item.FixedPurchaseId %>
																	<% } %>
																	<%#: string.IsNullOrEmpty(Item.FixedPurchaseId) ? "―" : "" %>
																	　｜　配送周期：<%#: string.IsNullOrEmpty(Item.FixedPurchaseId) ? "―" : FixedPurchaseCombineUtility.GetFixedPachasePatternSettingMessage(Item.FixedPurchaseId) %>
																	</label>
																</td>
															</tr>
															<tr>
																<td><label for='<%#: Container.FindControl("rbgSelectedOrderCombineParentOrder").ClientID %>'>定期注文回数｜定期出荷回数</label></td>
																<td>
																	<label for='<%#: Container.FindControl("rbgSelectedOrderCombineParentOrder").ClientID %>'>
																	<%#: GetFixedPurchaseOrderCountAndShippedCount(Item) %>
																	</label>
																</td>
															</tr>
															<tr>
																<td><label for='<%#: Container.FindControl("rbgSelectedOrderCombineParentOrder").ClientID %>'>クーポン</label></td>
																<td>
																	<label for='<%#: Container.FindControl("rbgSelectedOrderCombineParentOrder").ClientID %>'>
																	<%#: (Item.Coupons.Any() == false) ? "―" : string.Format("利用あり（{0}）",Item.Coupons[0].CouponName) %>
																	</label>
																</td>
															</tr>
															</table>
														</td>
													</tr>
													<asp:Repeater runat="server" ID="rOrderCombineChildOrder" Visible="<%# (Item == null) ? false : IsParentOrderSelect(Item.OrderId) %>"
														DataSource="<%# (Item == null) ? null : OrderCombineUtility.GetCombinableChildOrders(Item.OrderId) %>" ItemType="w2.Domain.Order.OrderModel">
													<HeaderTemplate>
													<tr>
														<td colspan="3" style="padding: 0 0 0 0; margin: 0 0 0 0">
														<table class="list_table" cellspacing="1" cellpadding="1" width="750">
														<tr class="list_title_bg">
															<td width="17">&nbsp;</td>
															<td width="17">同梱対象</td>
															<td colspan="2" style="text-align: center">注文ID　|　注文区分　|　決済種別　|　注文日時</td>
														</tr>
													</HeaderTemplate>
													<ItemTemplate>
														<tr class="list_item_bg<%#: Container.ItemIndex % 2 + 1 %>">
															<td width="17">&nbsp;</td>
															<td width="17"><input type="checkbox" name="OrderCombineChildOrder" id='<%#: "OrderCombineChildOrder" + Item.OrderId %>' value="<%#: Item.OrderId %>" checked="checked" /></td>
															<td width="17"><label for='<%#: "OrderCombineChildOrder" + Item.OrderId %>'><%#: Container.ItemIndex + 1 %></label></td>
															<td>
																<table width="100%" cellspacing="0" cellpadding="1" border="1">
																<tr>
																	<td colspan="2">
																		<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_ORDER_CONFIRM)) { %>
																			<a href="javascript:open_window('<%# WebSanitizer.UrlAttrHtmlEncode(OrderPage.CreateOrderDetailUrl(Item.OrderId, true, false, "order_combine")) %>','ordercontact','width=828,height=600,top=110,left=380,status=NO,scrollbars=yes');"><%#: Item.OrderId %></a>
																		<% } else { %>
																			<%#: Item.OrderId %>
																		<% } %>
																		<label for='<%#: "OrderCombineChildOrder" + Item.OrderId %>'>
																	　	 |　<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_KBN, Item.OrderKbn) %>
																	　	 |　<%#: Item.PaymentName %>
																	　	 |　<%#: DateTimeUtility.ToStringForManager(Item.OrderDate, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %>
																		<br />
																		<div style="text-align: right;">
																			（合計金額：<%#: Item.OrderPriceTotal.ToPriceString(true) %>-
																		　	 注文ステータス：<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_STATUS, Item.OrderStatus) %>
																		　	 入金ステータス：<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, Item.OrderPaymentStatus) %>）
																		</div>
																		</label>
																	</td>
																</tr>
																<tr>
																	<td width="25%"><label for='<%#: "OrderCombineChildOrder" + Item.OrderId %>'>配送先</label></td>
																	<td><label for='<%#: "OrderCombineChildOrder" + Item.OrderId %>'>
																		<%#: IsCountryJp(Item.Shippings[0].ShippingCountryIsoCode) ? "〒" + Item.Shippings[0].ShippingZip : "" %>
																	　	 <%#: Item.Shippings[0].ShippingAddr1 + Item.Shippings[0].ShippingAddr2 + Item.Shippings[0].ShippingAddr3 + Item.Shippings[0].ShippingAddr4 + Item.Shippings[0].ShippingAddr5 %>
																		<%#: (IsCountryJp(Item.Shippings[0].ShippingCountryIsoCode) == false) ? Item.Shippings[0].ShippingZip : "" %>
																		 <%#: Item.Shippings[0].ShippingCountryName %><br />
																		<%#: Item.Shippings[0].ShippingName %>
																		</label>
																	</td>
																</tr>
																<tr>
																	<td><label for='<%#: "OrderCombineChildOrder" + Item.OrderId %>'>配送日時</label></td>
																	<td>
																		<label for='<%#: "OrderCombineChildOrder" + Item.OrderId %>'>
																		希望日：<%#: DateTimeUtility.ToStringForManager(Item.Shippings[0].ShippingDate, DateTimeUtility.FormatType.ShortDate2Letter, "―") %>
																	　	 |　希望時間帯：<%#: GetShippingTimeMessage(Item.Shippings[0].DeliveryCompanyId, Item.Shippings[0].ShippingTime) %>
																		</label>
																	</td>
																</tr>
																<tr>
																	<td><label for='<%#: "OrderCombineChildOrder" + Item.OrderId %>'>商品明細</label></td>
																	<td>
																		<asp:Repeater runat="server" ID="Repeater1" DataSource="<%# Item.Shippings[0].Items %>" ItemType="w2.Domain.Order.OrderItemModel">
																		<ItemTemplate>
																			<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_PRODUCT_CONFIRM)) { %>
																				<a href="javascript:open_window('<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Item.ProductId, true)) %>','productcontact','width=828,height=615,top=120,left=320,status=NO,scrollbars=yes');"><%#: Item.ProductId %></a>
																			<% } else { %>
																				<%#: Item.ProductId %>
																			<% } %>
																			<label for='<%#: "OrderCombineChildOrder" + Item.OrderId %>'>
																			　<%#: Item.ProductName %>
																			<br />
																			</label>
																		</ItemTemplate>
																		</asp:Repeater>
																	</td>
																</tr>
																<tr>
																	<td><label for='<%#: "OrderCombineChildOrder" + Item.OrderId %>'>定期ID　｜　配送周期</label></td>
																	<td>
																		<label for='<%#: "OrderCombineChildOrder" + Item.OrderId %>'>
																			<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_FIXEDPURCHASE_CONFIRM)) { %>
																				定期ID：<a href="javascript:open_window('<%# WebSanitizer.UrlAttrHtmlEncode(FixedPurchasePage.CreateFixedPurchaseDetailUrl(Item.FixedPurchaseId, true)) %>','ordercontact','width=828,height=600,top=110,left=380,status=NO,scrollbars=yes');"><%#: Item.FixedPurchaseId %></a>
																			<% } else { %>
																				定期ID：<%#: Item.FixedPurchaseId %>
																			<% } %>
																		<%#: string.IsNullOrEmpty(Item.FixedPurchaseId) ? "―" : "" %>　｜　配送周期：<%#: string.IsNullOrEmpty(Item.FixedPurchaseId) ? "―" : FixedPurchaseCombineUtility.GetFixedPachasePatternSettingMessage(Item.FixedPurchaseId) %>
																		</label>
																	</td>
																</tr>
																<tr>
																	<td><label for='<%#: "OrderCombineChildOrder" + Item.OrderId %>'>定期注文回数｜定期出荷回数</label></td>
																	<td>
																		<label for='<%#: "OrderCombineChildOrder" + Item.OrderId %>'>
																		<%#: GetFixedPurchaseOrderCountAndShippedCount(Item) %>
																		</label>
																	</td>
																</tr>
																<tr>
																	<td><label for='<%#: "OrderCombineChildOrder" + Item.OrderId %>'>クーポン</label></td>
																	<td><label for='<%#: "OrderCombineChildOrder" + Item.OrderId %>'><%#: (Item.Coupons.Any() == false) ? "―" : string.Format("利用あり（{0}）",Item.Coupons[0].CouponName) %></label></td>
																</tr>
																</table>
															</td>
														</tr>
													</ItemTemplate>
													<FooterTemplate>
														<tr class="list_title_bg">
															<td colspan="4" style="text-align: right;"><asp:Button id="bCreateCombinedOrder" Text="　同梱注文作成　" OnClick="bCreateCombinedOrder_Click" runat="server" /></td>
														</tr>
														</table>
													</td>
													</tr>
													</FooterTemplate>
													</asp:Repeater>
												</ItemTemplate>
											</asp:Repeater>
										</table>
										<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
										</div>
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
