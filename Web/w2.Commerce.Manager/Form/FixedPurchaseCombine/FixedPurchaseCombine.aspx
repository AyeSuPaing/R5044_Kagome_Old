<%--
=========================================================================================================
  Module      : 定期購入同梱ページ(FixedPurchaseCombine.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Form/Common/DefaultPage.master" CodeFile="FixedPurchaseCombine.aspx.cs" Inherits="Form_OrderCombine_FixedPurchaseCombine" MaintainScrollPositionOnPostback="true" %>
<%@ Register TagPrefix="w2cm" Assembly="w2.App.Common" Namespace="w2.App.Common.Web.WebCustomControl" %>
<%@ Register TagPrefix="uc" TagName="DateTimeInput" Src="~/Form/Common/DateTimeInput.ascx" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="w2.App.Common.Order" %>
<%@ Import Namespace="w2.App.Common.Order.OrderCombine" %>
<%@ Import Namespace="w2.Domain.FixedPurchase" %>
<%@ Import Namespace="w2.Domain.Order" %>
<%@ Import Namespace="w2.App.Common.Global" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<table cellspacing="0" cellpadding="0" width="791" border="0">
		<tr><td><h1 class="page-title">定期台帳同梱</h1></td></tr>
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
											<tr><td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
											<tr>
												<td class="search_table">
													<table cellspacing="1" cellpadding="2" width="758" border="0">
														<tr>
															<td class="search_title_bg">
																<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
																ユーザーID
															</td>
															<td class="search_item_bg">
																<asp:TextBox runat="server" ID="tbUserId" /></td>
															<td class="search_title_bg">
																<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
																ユーザー名
															</td>
															<td class="search_item_bg">
																<asp:TextBox runat="server" ID="tbUserName" /></td>
															<td class="search_btn_bg" width="83" rowspan="2">
																<div class="search_btn_main"><asp:Button runat="server" ID="btnSearch" Text="　検索　" OnClick="btnSearch_Click" /></div>
																<div class="search_btn_sub">
																	<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_FIXEDPURCHASECOMBINE_FIXEDPURCHASE_COMBINE %>">クリア</a>
																	<a href="javascript:Reset();">リセット</a>
																</div>
															</td>
														</tr>
														<tr>
															<td class="search_title_bg">
																<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
																次回配送日
															</td>
															<td class="search_item_bg" colspan="3">
																<uc:DateTimePickerPeriodInput id="ucNextShippingDatePeriod" runat="server" IsHideTime="True" />
															</td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
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
		<tr id="hgcCompleteMessageRow" runat="server">
			<td>
				<table class="box_border" cellspacing="3" cellpadding="3" width="784" border="0">
					<tr>
						<td class="search_box_bg">定期台帳　<%: this.FixedPurchaseCombineTargetsID %>　を定期台帳　
							<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_FIXEDPURCHASE_CONFIRM)) { %>
								<a href="javascript:open_window('<%= WebSanitizer.UrlAttrHtmlEncode(FixedPurchasePage.CreateFixedPurchaseDetailUrl(this.FixedPurchaseCombineParentID, true)) %>','ordercontact','width=828,height=600,top=110,left=380,status=NO,scrollbars=yes');"><%: this.FixedPurchaseCombineParentID %></a>　
							<% } else { %>
								<%: this.FixedPurchaseCombineParentID %>　
							<% } %>
							に同梱しました。</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr><td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
		<!--▽ 一覧 ▽-->
		<tr>
			<td><h2 class="cmn-hed-h2">定期台帳同梱対象一覧</h2></td>
		</tr>
		<tr>
			<td>
				<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
					<tr>
						<td>
							<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td>
										<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0t">
										<tr>
											<td width="675" style="height: 22px"><asp:Label ID="lbPager" runat="server" /></td>
										</tr>
										</table>
									</td>
								</tr>
								<tr>
									<td align="center">
										<div id="CombineList" runat="server">
											<asp:HiddenField ID="hfSelectedFixedPurchaseCombineParentFpId" runat="server" />
											<table class="list_table" cellspacing="1" cellpadding="3" width="95%">
											<tr class="list_title_bg">
												<td width="17">親指定</td>
												<td colspan="2" style="text-align: center">定期ID　|　注文区分　|　決済種別　|　注文日時</td>
											</tr>
											<tr id="hgcErrorMessageRow" class="list_alert" runat="server">
												<td  colspan="3"><asp:Literal id="lErrorMessage" runat="server"></asp:Literal></td>
											</tr>
											<asp:Repeater runat="server" ID="rFixedPurchaseCombineParentFixedPurchase" ItemType="w2.Domain.FixedPurchase.FixedPurchaseModel">
												<ItemTemplate>
													<tr class="list_item_bg<%#: Container.ItemIndex % 2 + 1 %>">
													<td width="17">
														<w2cm:RadioButtonGroup ID="rbgSelectedParentFixedPurchase" GroupName="rbgSelectedParentFixedPurchase" Name="rbgSelectedParentFixedPurchase"
														OnCheckedChanged="rbgSelectedParentFixedPurchase_CheckedChanged" Checked="<%# hfSelectedFixedPurchaseCombineParentFpId.Value == Item.FixedPurchaseId %>" AutoPostBack="true" runat="server" />
															<asp:HiddenField ID="hfFixedPurchaseCombineParentFpId" Value="<%# Item.FixedPurchaseId %>" runat="server" /></td>
													<td width="38"><label for='<%#: Container.FindControl("rbgSelectedParentFixedPurchase").ClientID %>'><%#: Container.ItemIndex + 1 %></label></td>
													<td>
														<table width="100%" cellspacing="0" cellpadding="3" border="1">
														<tr>
															<td colspan="2">
																<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_FIXEDPURCHASE_CONFIRM)) { %>
																	<a href="javascript:open_window('<%# WebSanitizer.UrlAttrHtmlEncode(FixedPurchasePage.CreateFixedPurchaseDetailUrl(Item.FixedPurchaseId, true)) %>','ordercontact','width=828,height=600,top=110,left=380,status=NO,scrollbars=yes');"><%#: Item.FixedPurchaseId %></a>
																<% } else { %>
																	<%#: Item.FixedPurchaseId %>
																<% } %>
																<label for='<%#: Container.FindControl("rbgSelectedParentFixedPurchase").ClientID %>'>
																	　|　<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_KBN, Item.OrderKbn) %>　|　<%#: Item.OrderPaymentKbnText %>　|　<%#: Item.FixedPurchaseDateBgn %>
																<br />
																<div style="text-align: right;">
																	-　定期購入S：<%#: Item.FixedPurchaseStatusText %>
																		　決済S：<%#: Item.PaymentStatusText %>
																</div>
																</label>
															</td>
														</tr>
														<tr>
															<td width="25%"><label for='<%#: Container.FindControl("rbgSelectedParentFixedPurchase").ClientID %>'>配送先</label></td>
															<td>
																<label for='<%#: Container.FindControl("rbgSelectedParentFixedPurchase").ClientID %>'>
																<%#: IsCountryJp(Item.Shippings[0].ShippingCountryIsoCode)
																		? "〒" + Item.Shippings[0].ShippingZip : "" %>
																	　<%#: Item.Shippings[0].ShippingAddr1 + Item.Shippings[0].ShippingAddr2 + Item.Shippings[0].ShippingAddr3 + Item.Shippings[0].ShippingAddr4 %>
																	<%#: (IsCountryJp(Item.Shippings[0].ShippingCountryIsoCode) == false) ? Item.Shippings[0].ShippingZip : "" %>
																	<%#: Item.Shippings[0].ShippingAddr5 + Item.Shippings[0].ShippingCountryName %><br />
																<%#: Item.Shippings[0].ShippingName %>
																</label>
															</td>
														</tr>
														<tr>
															<td><label for='<%#: Container.FindControl("rbgSelectedParentFixedPurchase").ClientID %>'>次回配送日｜配送周期</label></td>
															<td>
																<label for='<%#: Container.FindControl("rbgSelectedParentFixedPurchase").ClientID %>'>
																次回配送日：<%#: DateTimeUtility.ToStringForManager(Item.NextShippingDate, DateTimeUtility.FormatType.ShortDate2Letter) %>
																	　|　配送周期：<%#: OrderCommon.CreateFixedPurchaseSettingMessage(Item) %>
																</label>
															</td>
														</tr>
														<tr>
															<td><label for='<%#: Container.FindControl("rbgSelectedParentFixedPurchase").ClientID %>'>定期注文回数｜定期出荷回数</label></td>
															<td>
																<label for='<%#: Container.FindControl("rbgSelectedParentFixedPurchase").ClientID %>'>
																<%#: Item.OrderCount %>回 ｜ <%#: Item.ShippedCount %>回
																</label>
															</td>
														</tr>
														<tr>
															<td><label for='<%#: Container.FindControl("rbgSelectedParentFixedPurchase").ClientID %>'>商品明細</label></td>
															<td>
																<asp:Repeater runat="server" ID="rFixedPurchaseCombineParentFixedPurchaseProduct" DataSource="<%# ProductCommon.GetProductsInfo(Item.ShopId, Item.Shippings[0].Items.Select(m => m.ProductId).ToArray()) %>" ItemType="System.Data.DataRowView">
																	<ItemTemplate>
																		<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_PRODUCT_CONFIRM)) { %>
																			<a href="javascript:open_window('<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl((string)Item[Constants.FIELD_PRODUCT_PRODUCT_ID], true)) %>','productstockhistory_list','width=828,height=615,top=120,left=320,status=NO,scrollbars=yes');"><%#: Item[Constants.FIELD_PRODUCT_PRODUCT_ID] %></a>
																		<% } else { %>
																			<%#: Item[Constants.FIELD_PRODUCT_PRODUCT_ID] %>
																		<% } %>
																		<label for='<%# Container.Parent.Parent.FindControl("rbgSelectedParentFixedPurchase").ClientID %>'>
																			　<%#: Item[Constants.FIELD_PRODUCT_NAME] %>
																		<br />
																		</label>
																	</ItemTemplate>
																</asp:Repeater>
															</td>
														</tr>
														</table>
													</td>
													</tr>
												<asp:Repeater runat="server" ID="rFixedPurchaseCombineChildFixedPurchase" ItemType="w2.Domain.FixedPurchase.FixedPurchaseModel"
													Visible="<%# (Item == null) ? false : IsParentFixedPurchaseSelect(Item.FixedPurchaseId) %>" DataSource="<%# (Item == null) ? null : GetCombinableChildFixedPurchases(Item.FixedPurchaseId, Item.UserId) %>">
												<HeaderTemplate>
												<tr>
													<td colspan="3" style="padding: 0 0 0 0; margin: 0 0 0 0">
													<table class="list_table" cellspacing="1" cellpadding="1" width="750">
													<tr class="list_title_bg">
														<td width="17">&nbsp;</td>
														<td width="17">同梱対象</td>
														<td colspan="2" style="text-align: center">定期ID　|　注文区分　|　決済種別　|　注文日時</td>
													</tr>
												</HeaderTemplate>
												<ItemTemplate>
													<tr class="list_item_bg<%#: Container.ItemIndex % 2 + 1 %>">
														<td>&nbsp;</td>
														<td><input type="checkbox" name="FixedPurchaseCombineChildFixedPurchase" id='<%#: "FixedPurchaseCombineChildFixedPurchase" + Item.FixedPurchaseId %>' value="<%#: Item.FixedPurchaseId %>" checked="checked" /></td>
														<td><label for='<%#: "FixedPurchaseCombineChildFixedPurchase" + Item.FixedPurchaseId %>'><%#: Container.ItemIndex + 1 %></label></td>
														<td>
															<table width="100%" cellspacing="0" cellpadding="3" border="1">
															<tr>
																<td colspan="2">
																	<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_FIXEDPURCHASE_CONFIRM)) { %>
																		<a href="javascript:open_window('<%# WebSanitizer.UrlAttrHtmlEncode(FixedPurchasePage.CreateFixedPurchaseDetailUrl(Item.FixedPurchaseId, true)) %>','ordercontact','width=828,height=600,top=110,left=380,status=NO,scrollbars=yes');"><%#: Item.FixedPurchaseId %></a>
																	<% } else { %>
																		<%#: Item.FixedPurchaseId %>
																	<% } %>
																	<label for='<%#: "FixedPurchaseCombineChildFixedPurchase" + Item.FixedPurchaseId %>'>
																	　	 |　<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_KBN, Item.OrderKbn) %>　|　<%#: Item.OrderPaymentKbnText %>　|　<%#: Item.FixedPurchaseDateBgn %>
																	<br />
																	<div style="text-align: right;">
																		-　定期購入S：<%#: Item.FixedPurchaseStatusText %>
																		　	 決済S：<%#: Item.PaymentStatusText %></div>
																	</label>
																</td>
															</tr>
															<tr>
																<td width="25%"><label for='<%#: "FixedPurchaseCombineChildFixedPurchase" + Item.FixedPurchaseId %>'>配送先</label></td>
																<td>
																	<label for='<%#: "FixedPurchaseCombineChildFixedPurchase" + Item.FixedPurchaseId %>'>
																	<%#: IsCountryJp(Item.Shippings[0].ShippingCountryIsoCode)
																		? ("〒" + Item.Shippings[0].ShippingZip + Item.Shippings[0].ShippingAddr1 + Item.Shippings[0].ShippingAddr2 + Item.Shippings[0].ShippingAddr3 + Item.Shippings[0].ShippingAddr4)
																		: string.Format("{0} {1} {2} {3} {4} {5}"
																			, Item.Shippings[0].ShippingAddr2
																			, Item.Shippings[0].ShippingAddr3
																			, Item.Shippings[0].ShippingAddr4
																			, Item.Shippings[0].ShippingAddr5
																			, Item.Shippings[0].ShippingZip
																			, Item.Shippings[0].ShippingCountryName)%><br />
																	<%#: Item.Shippings[0].ShippingName %>
																	</label>
																</td>
															</tr>
															<tr>
																<td><label for='<%#: "FixedPurchaseCombineChildFixedPurchase" + Item.FixedPurchaseId %>'>次回配送日｜配送周期</label></td>
																<td>
																	<label for='<%#: "FixedPurchaseCombineChildFixedPurchase" + Item.FixedPurchaseId %>'>
																	次回配送日：<%#: DateTimeUtility.ToStringForManager(Item.NextShippingDate, DateTimeUtility.FormatType.LongDate2Letter)%>
																	　	 |　配送周期：<%#: OrderCommon.CreateFixedPurchaseSettingMessage(Item) %>
																	</label>
																</td>
															</tr>
															<tr>
																<td><label for='<%#: "FixedPurchaseCombineChildFixedPurchase" + Item.FixedPurchaseId %>'>定期注文回数｜定期出荷回数</label></td>
																<td>
																	<label for='<%#: "FixedPurchaseCombineChildFixedPurchase" + Item.FixedPurchaseId %>'>
																	<%#: Item.OrderCount %>回 ｜ <%#: Item.ShippedCount %>回
																	</label>
																</td>
															</tr>
															<tr>
																<td><label for='<%#: "FixedPurchaseCombineChildFixedPurchase" + Item.FixedPurchaseId %>'>商品明細</label></td>
																<td>
																	<asp:Repeater runat="server" ID="rFixedPurchaseCombineParentFixedPurchaseProduct"  DataSource="<%# ProductCommon.GetProductsInfo(Item.ShopId, Item.Shippings[0].Items.Select(m => m.ProductId).ToArray()) %>" ItemType="System.Data.DataRowView">
																		<ItemTemplate>
																			<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_PRODUCT_CONFIRM)) { %>
																				<a href="javascript:open_window('<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl((string)Item[Constants.FIELD_PRODUCT_PRODUCT_ID], true)) %>','productstockhistory_list','width=828,height=615,top=120,left=320,status=NO,scrollbars=yes');"><%#: Item[Constants.FIELD_PRODUCT_PRODUCT_ID] %></a>
																			<% } else { %>
																				<%#: Item[Constants.FIELD_PRODUCT_PRODUCT_ID] %>
																			<% } %>
																			<label for='<%#: "FixedPurchaseCombineChildFixedPurchase" + ((FixedPurchaseModel)((RepeaterItem)Container.Parent.Parent).DataItem).FixedPurchaseId %>'>
																			　	 <%#: Item[Constants.FIELD_PRODUCT_NAME] %>
																			<br />
																			</label>
																		</ItemTemplate>
																	</asp:Repeater>
																</td>
															</tr>
															</table>
														</td>
													</tr>
												</ItemTemplate>
												<FooterTemplate>
													<tr class="list_title_bg">
														<td colspan="4" style="text-align: right;"><asp:Button id="bCreateCombinedOrder" Text="　同梱実行　" OnClick="bCreateCombinedOrder_Click" runat="server" /></td>
													</tr>
													</table>
												</td>
												</tr>
												</FooterTemplate>
												</asp:Repeater>
												</ItemTemplate>
											</asp:Repeater>
											</table>
										</div>
									</td>
								</tr>
								<tr>
									<td>
										<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<!--△ 一覧 △-->
		<tr><td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
	</table>
<script>
	// Reset form
	function Reset()
	{
		document.getElementById('<%= ucNextShippingDatePeriod.HfStartDate.ClientID %>').value = '';
		document.getElementById('<%= ucNextShippingDatePeriod.HfStartTime.ClientID %>').value = '';
		document.getElementById('<%= ucNextShippingDatePeriod.HfEndDate.ClientID %>').value = '';
		document.getElementById('<%= ucNextShippingDatePeriod.HfEndTime.ClientID %>').value = '';
		reloadDisplayDateTimePeriod('<%= ucNextShippingDatePeriod.ClientID %>');
		this.document.<%= this.Form.ClientID %>.reset();
	}
</script>
</asp:Content>
