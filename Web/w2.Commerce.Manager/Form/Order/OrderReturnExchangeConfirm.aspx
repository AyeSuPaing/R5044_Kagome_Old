<%--
=========================================================================================================
  Module      : 注文返品交換確認ページ(OrderReturnExchangeConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OrderReturnExchangeConfirm.aspx.cs" Inherits="Form_Order_OrderReturnExchangeConfirm" %>
<%@ Import Namespace="w2.App.Common.Option" %>
<%@ Import Namespace="w2.App.Common.Order" %>
<%@ Import Namespace="w2.Domain.Order" %>
<%@ Import Namespace="w2.App.Common.Product" %>
<%@ Import Namespace="w2.Common.Web" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr id="trTitleOrderTop" runat="server">
	</tr>
	<tr id="trTitleOrderMiddle" runat="server">
		<td><h1 class="page-title">受注情報詳細</h1></td>
	</tr>
	<tr id="trTitleOrderBottom" runat="server">
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<td><h2 class="cmn-hed-h2">受注情報返品交換確認</h2></td>
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
												<div class="action_part_top" style="display:table; width:100%">
													<div style="padding-bottom: 2px; color:red; text-align:left; vertical-align:top"><asp:Label ID="lbAllReturnNotZeroWarning" runat="server"></asp:Label></div>
													<div style="padding-bottom: 2px; color:red; text-align:left; vertical-align:top"><asp:Label ID="lbMessagePayment" runat="server"></asp:Label></div>
													<div style="display: table-cell; vertical-align:top">
													<input type="button" onclick="Javascript:history.back();" value="  戻る  " />
													<asp:Button id="btnRegistTop" runat="server" Text="  登録する  " OnClientClick="return exec_submit()" OnClick="btnRegist_Click" />
													</div>
												</div>
												<%--▽ 基本情報 ▽--%>
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="detail_title_bg" align="center" colspan="2">基本情報</td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left" width="25%">元注文ID</td>
															<td class="detail_item_bg" align="left"><asp:Literal ID="lOrderId" runat="server"></asp:Literal></td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left" width="25%">返品交換区分</td>
															<td class="detail_item_bg" align="left"><asp:Literal ID="lReturnExchangeKbn" runat="server"></asp:Literal></td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left" width="25%">返品交換受付日</td>
															<td class="detail_item_bg" align="left"><asp:Literal ID="lOrderReturnExchangeReceiptDate" runat="server"></asp:Literal></td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left" width="25%" rowspan="2">返品交換理由</td>
															<td class="detail_item_bg" align="left">
																<asp:Literal ID="lReturnExchangeReasonKbn" runat="server"></asp:Literal>
															</td>
														</tr>
														<tr>
															<td class="detail_item_bg" align="left">
																<asp:Literal ID="lReturnExchangeReasonMemo" runat="server"></asp:Literal>&nbsp;
															</td>
														</tr>
														<tr id="trOrderPaymentKbn" runat="server">
															<td class="detail_title_bg" align="left" width="25%">決済種別</td>
															<td class="detail_item_bg" align="left"><asp:Literal ID="lOrderPaymentKbn" runat="server"></asp:Literal></td>
														</tr>
													</tbody>
												</table>
												<%--△ 基本情報 △--%>
												<%--▽ 返品商品情報 ▽--%>
												<br />
												<asp:Repeater ID="rReturnOrderShipping" ItemType="w2.App.Common.Order.ReturnOrderItem" runat="server"><ItemTemplate>
												<br />
												<% if ((Constants.GIFTORDER_OPTION_ENABLED) && (this.ReturnExchangeOrderOrg.GiftFlg == Constants.FLG_ORDER_GIFT_FLG_ON)) { %>
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
															<td class="detail_title_bg" align="center" colspan="4">【　配送情報<%#: Item.OrderShippingNo %>　】</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="center" colspan="4">配送先情報</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.name.name@@") %></td>
														<td class="detail_item_bg" align="left" width="25%">
															<%#: Item.ShippingName %></td>
														<td class="detail_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.name_kana.name@@") %></td>
														<td class="detail_item_bg" align="left">
															<%#: Item.ShippingNameKana %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="25%">住所</td>
														<td class="detail_item_bg" align="left" colspan="3">
															<span visible='<%# ((string)Item.ShippingZip != "") %>' runat="server">
															<%#: (Item.IsShippingAddJp) ? "〒" + Item.ShippingZip : "" %>
															</span>
															<%#: Item.ShippingAddr1 %>
															<%#: Item.ShippingAddr2 %>
															<%#: Item.ShippingAddr3 %>
															<%#: Item.ShippingAddr4 %>
															<%#: Item.ShippingAddr5 %>
															<span id="Span1" visible='<%# ((string)Item.ShippingZip != "") %>' runat="server">
															<%#: (Item.IsShippingAddJp == false) ? Item.ShippingZip : "" %>
															</span>
															<%#: Item.ShippingCountryName %>
														</td>
													</tr>
													<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
													<tr>
														<td class="detail_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.company_name.name@@")%>・<%: ReplaceTag("@@User.company_post_name.name@@")%></td>
														<td class="detail_item_bg" align="left" colspan="3">
															<%#: Item.ShippingCompanyName %>&nbsp<%#: Item.ShippingCompanyPostName %></td>
													</tr>
													<%} %>
													<tr>
														<td class="detail_title_bg" align="left" width="25%"><%: ReplaceTag("@@User.tel1.name@@") %></td>
														<td class="detail_item_bg" align="left" colspan="3">
															<%#: Item.ShippingTel1 %></td>
													</tr>
													<%-- △配送先情報△--%>
												</table>
												<% } %>
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="detail_title_bg" align="center" colspan='<%: 6 + this.AddColumnCountForItemTable %>'>返品商品選択</td>
														</tr>
														<tr>
															<%--▽ セールOPが有効の場合 ▽--%>
															<% if (Constants.PRODUCT_SALE_OPTION_ENABLED) { %>
															<td class="detail_title_bg" align="center" rowspan="2" width="10%">セールID</td>
															<%} %>
															<%--△ セールOPが有効の場合 △--%>
															<%--▽ ノベルティOPが有効の場合 ▽--%>
															<% if (Constants.NOVELTY_OPTION_ENABLED) { %>
															<td class="detail_title_bg" align="center" rowspan="<%= Constants.RECOMMEND_OPTION_ENABLED ? 1 : 2 %>" width="10%">ノベルティID</td>
															<%} %>
															<%--△ ノベルティOPが有効の場合 △--%>
															<%--▽ レコメンド設定OPが有効の場合 ▽--%>
															<% if (Constants.RECOMMEND_OPTION_ENABLED && (Constants.NOVELTY_OPTION_ENABLED == false)) { %>
															<td class="detail_title_bg" align="center" rowspan="2" width="10%">
																レコメンドID
															</td>
															<%} %>
															<%--△ レコメンド設定OPが有効の場合 △--%>
															<%--▽ 商品同梱設定OPが有効の場合 ▽--%>
															<% if (Constants.PRODUCTBUNDLE_OPTION_ENABLED) { %>
															<td class="detail_title_bg" align="center" rowspan="2" width="10%">
																商品同梱ID
															</td>
															<%} %>
															<%--△ 商品同梱設定OPが有効の場合 △--%>
															<td class="detail_title_bg" align="center" rowspan="2" width="10%" Visible="<%# this.DisplayItemSubscriptionBoxCourseIdArea %>" runat="server">
																頒布会コースID
															</td>
															<td class="detail_title_bg" align="center" width="18%">商品ID</td>
															<td class="detail_title_bg" align="center" rowspan="2" width="<%= 20 + ((Constants.NOVELTY_OPTION_ENABLED || Constants.RECOMMEND_OPTION_ENABLED) ? 0 : 10) + (Constants.PRODUCT_SALE_OPTION_ENABLED ? 0 : 10) + (Constants.PRODUCTBUNDLE_OPTION_ENABLED ? 0 : 10) + (this.DisplayItemSubscriptionBoxCourseIdArea ? 0 : 10) %>%">
																商品名
															</td>
															<td class="detail_title_bg" align="center" rowspan="2" width="6%" Visible="<%# this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
																単価（<%: this.ProductPriceTextPrefix %>）
															</td>
															<td class="detail_title_bg" align="center" rowspan="2" width="3%">数量</td>
															<td class="detail_title_bg" align="center" rowspan="2" width="3%" Visible="<%# this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
																消費税率
															</td>
															<td class="detail_title_bg" align="center" rowspan="2" width="10%" Visible="<%# this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
																小計（<%: this.ProductPriceTextPrefix %>）
															</td>
														</tr>
														<tr>
															<%--▽ レコメンド設定OPが有効の場合 ▽--%>
															<% if (Constants.RECOMMEND_OPTION_ENABLED && Constants.NOVELTY_OPTION_ENABLED) { %>
															<td class="detail_title_bg" align="center">
																レコメンドID
															</td>
															<%} %>
															<%--△ レコメンド設定OPが有効の場合 △--%>
															<td class="detail_title_bg" align="center" width="20%">バリエーションID</td>
														</tr>
														<asp:Repeater id="rReturnOrderItem" DataSource="<%# this.lOrderItems %>" ItemType="w2.App.Common.Order.ReturnOrderItem" runat="server">
														<ItemTemplate>
															<tr class="detail_item_bg" visible="<%# (Item.OrderShippingNo == ((ReturnOrderItem)((RepeaterItem)Container.Parent.Parent).DataItem).OrderShippingNo) %>" runat="server">
																<td align="center" rowspan="2" runat="server" visible='<%# Constants.PRODUCT_SALE_OPTION_ENABLED %>'><%#: (Item.ProductSaleId != "") ? Item.ProductSaleId : "-" %></td>
																<td align="center" rowspan="<%# Constants.RECOMMEND_OPTION_ENABLED ? 1 : 2 %>" runat="server" visible='<%# Constants.NOVELTY_OPTION_ENABLED %>'>
																	<%#: (Item.NoveltyId != "") ? Item.NoveltyId : "-" %>
																</td>
																<td align="center" rowspan="2" runat="server" visible="<%# (Constants.RECOMMEND_OPTION_ENABLED && (Constants.NOVELTY_OPTION_ENABLED == false)) %>">
																	<%#: (Item.RecommendId != "") ? Item.RecommendId : "-" %>
																</td>
																<td align="center" rowspan="2" visible="<%# (Constants.PRODUCTBUNDLE_OPTION_ENABLED) %>" runat="server">
																	<%#: (string.IsNullOrEmpty(Item.ProductBundleId) == false) ? Item.ProductBundleId : "-" %>
																</td>
																<td align="center" rowspan="2" Visible="<%# this.DisplayItemSubscriptionBoxCourseIdArea %>" runat="server">
																	<%#: Item.IsSubscriptionBox ? Item.SubscriptionBoxCourseId : "-" %>
																</td>
																<td align="center"><%#: Item.ProductId %></td>
																<td align="left" rowspan="2">
																	<%#: Item.ProductName %><br />
																	<span visible='<%# (Item.OrderSetPromotionNo != "") %>' runat="server">
																		[<%#: Item.OrderSetPromotionNo %>：<%#: this.ReturnExchangeOrderOrg.GetOrderSetPromotionName(Item.OrderSetPromotionNo) %>]
																	</span>
																</td>
																<td align='<%# Item.IsSubscriptionBoxFixedAmount == false ? "right" : "center" %>' rowspan="2" Visible="<%# this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
																	<%#: Item.IsSubscriptionBoxFixedAmount == false ? Item.ProductPrice.ToPriceString(true) : "-" %>
																</td>
																<td align="center" rowspan="2">
																	<%# GetMinusNumberNoticeHtml(Item.ItemQuantity, false) %>
																</td>
																<td align="center" rowspan="2" Visible="<%# this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
																	<%#: Item.IsSubscriptionBoxFixedAmount == false ? string.Format("{0}%", TaxCalculationUtility.GetTaxRateForDIsplay(Item.ProductTaxRate)) : "-" %>
																</td>
																<td align='<%# Item.IsSubscriptionBoxFixedAmount == false ? "right" : "center" %>' rowspan="2" Visible="<%# this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
																	<div Visible="<%# Item.IsSubscriptionBoxFixedAmount == false %>" class='<%# Item.IsItemPriceMinus ? "item-notice" : string.Empty %>' runat="server">
																		<%#: Item.ItemPrice.ToPriceString(true) %>
																	</div>
																	<div Visible="<%# Item.IsSubscriptionBoxFixedAmount %>" class='<%# IsFixedAmountCourseItemAllReturns(Item.SubscriptionBoxCourseId) ? "item-notice" : string.Empty %>' runat="server">
																		<%# HtmlSanitizer.HtmlEncodeChangeToBr(string.Format(
																			    "定額\r\n({0}{1})",
																			    IsFixedAmountCourseItemAllReturns(Item.SubscriptionBoxCourseId) ? "-" : string.Empty,
																			    Item.SubscriptionBoxFixedAmount.ToPriceString(true))) %>
																	</div>
																</td>
															</tr>
															<tr class="detail_item_bg" visible="<%# (Item.OrderShippingNo == ((ReturnOrderItem)((RepeaterItem)Container.Parent.Parent).DataItem).OrderShippingNo) %>" runat="server">
																<td align="center" runat="server" visible="<%# (Constants.RECOMMEND_OPTION_ENABLED && Constants.NOVELTY_OPTION_ENABLED) %>">
																	<%#: (Item.RecommendId != "") ? Item.RecommendId : "-" %>
																</td>
																<td align="center">
																	<%#: (Item.VId != "") ? "商品ID + " + Item.VId : "-" %>
																</td>
															</tr>
															<tr visible='<%# (Item.OrderShippingNo == ((ReturnOrderItem)((RepeaterItem)Container.Parent.Parent).DataItem).OrderShippingNo)
																				&& (StringUtility.ToEmpty(Item.ProductOptionValue) != "") %>' runat="server">
																<td class="detail_title_bg" align="center">付帯情報</td>
																<td class="detail_item_bg" align="left" colspan='<%# 4 + this.AddColumnCountForItemTable %>'><%#: ProductOptionSettingHelper.GetDisplayProductOptionTexts(Item.ProductOptionValue) %></td>
															</tr>
														</ItemTemplate>
														</asp:Repeater>
													</tbody>
												</table>
												</ItemTemplate>
												</asp:Repeater>
												<br />
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="detail_title_bg" align="right">返品商品合計（<%: this.ProductPriceTextPrefix %>）</td>
															<td class="detail_item_bg" align="right" width="10%"><asp:Label ID="lbReturnOrderPriceSubTotal" runat="server" ForeColor="Red"></asp:Label></td>
														</tr>
													</tbody>
												</table>
												<%--△ 返品商品情報 △--%>
												<%--▽ 交換商品情報 ▽--%>
												<div id="divExchangeOrderItem" runat="server">
												<br />
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="detail_title_bg" align="center" colspan="<%: (this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() ? 4 : 6) + (this.DisplayItemSubscriptionBoxCourseIdArea ? -1 : 0) + this.AddColumnCountForItemTable %>">交換商品注文登録</td>
														</tr>
														<tr>
															<%--▽ セールOPが有効の場合 ▽--%>
															<% if (Constants.PRODUCT_SALE_OPTION_ENABLED){ %>
															<td class="detail_title_bg" align="center" width="10%" rowspan="2">セールID</td>
															<%} %>
															<%--△ セールOPが有効の場合 △--%>
															<%--▽ ノベルティOPが有効の場合 ▽--%>
															<% if (Constants.NOVELTY_OPTION_ENABLED){ %>
															<td class="detail_title_bg" align="center" width="10%" rowspan="<%= Constants.RECOMMEND_OPTION_ENABLED ? 1 : 2 %>">ノベルティID</td>
															<%} %>
															<%--△ ノベルティOPが有効の場合 △--%>
															<%--▽ レコメンド設定OPが有効の場合 ▽--%>
															<% if (Constants.RECOMMEND_OPTION_ENABLED && (Constants.NOVELTY_OPTION_ENABLED == false)) { %>
															<td class="detail_title_bg" align="center" rowspan="2" width="10%">
																レコメンドID
															</td>
															<%} %>
															<%--▽ 商品同梱設定OPが有効の場合 ▽--%>
															<% if (Constants.PRODUCTBUNDLE_OPTION_ENABLED) { %>
															<td class="detail_title_bg" align="center" rowspan="2" width="12%">
																商品同梱ID
															</td>
															<%} %>
															<%--△ 商品同梱設定OPが有効の場合 △--%>
															<td class="detail_title_bg" align="center" width="20%">商品ID</td>
															<td class="detail_title_bg" align="center" width="<%= 20 + ((Constants.NOVELTY_OPTION_ENABLED || Constants.RECOMMEND_OPTION_ENABLED) ? 0 : 10) + (Constants.PRODUCT_SALE_OPTION_ENABLED ? 0 : 10) + (Constants.PRODUCTBUNDLE_OPTION_ENABLED ? 0 : 12) %>%" rowspan="2">商品名</td>
															<% if (this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false) { %>
															<td class="edit_title_bg" align="center" width="8%" rowspan="2">単価（<%: this.ProductPriceTextPrefix %>）</td>
															<% } %>
															<% if (this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem()) { %>
															<td class="edit_title_bg" align="center" width="5%" rowspan="2" colspan="2">数量</td>
															<% } %>
															<% if (this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false) { %>
															<td class="edit_title_bg" align="center" width="5%" rowspan="2">数量</td>
															<td class="edit_title_bg" align="center" width="5%" rowspan="2">消費税率</td>
															<td class="edit_title_bg" align="center" width="10%" rowspan="2">小計（<%: this.ProductPriceTextPrefix %>）</td>
															<% } %>
														</tr>
														<tr>
															<%--▽ レコメンド設定OPが有効の場合 ▽--%>
															<% if (Constants.RECOMMEND_OPTION_ENABLED && Constants.NOVELTY_OPTION_ENABLED) { %>
															<td class="detail_title_bg" align="center">
																レコメンドID
															</td>
															<%} %>
															<%--△ レコメンド設定OPが有効の場合 △--%>
															<td class="detail_title_bg" align="center" width="20%">バリエーションID</td>
														</tr>
														<asp:Repeater ID="rExchangeOrderItem" runat="server">
															<ItemTemplate>
																<tr class="detail_item_bg">
																	<%--▽ セールOPが有効の場合 ▽--%>
																	<% if (Constants.PRODUCT_SALE_OPTION_ENABLED){ %>
																	<td align="center" rowspan="2"><%# WebSanitizer.HtmlEncode((((ReturnOrderItem)Container.DataItem).ProductSaleId != "") ? ((ReturnOrderItem)Container.DataItem).ProductSaleId : "-")%></td>
																	<%} %>
																	<%--△ セールOPが有効の場合 △--%>
																	<td align="center" rowspan="<%# Constants.RECOMMEND_OPTION_ENABLED ? 1 : 2 %>" runat="server" visible="<%# Constants.NOVELTY_OPTION_ENABLED %>">
																		<%# WebSanitizer.HtmlEncode((((ReturnOrderItem)Container.DataItem).NoveltyId != "") ? ((ReturnOrderItem)Container.DataItem).NoveltyId : "-")%>
																	</td>
																	<td align="center" rowspan="2" runat="server" visible="<%# (Constants.RECOMMEND_OPTION_ENABLED && (Constants.NOVELTY_OPTION_ENABLED == false)) %>">
																		<%#: (((ReturnOrderItem)Container.DataItem).RecommendId != "") ? ((ReturnOrderItem)Container.DataItem).RecommendId : "-" %>
																	</td>
																	<td align="center" rowspan="2" visible="<%# (Constants.PRODUCTBUNDLE_OPTION_ENABLED) %>" runat="server">
																		<%#: (string.IsNullOrEmpty(((ReturnOrderItem)Container.DataItem).ProductBundleId) == false) ? ((ReturnOrderItem)Container.DataItem).ProductBundleId : "-" %>
																	</td>
																	<td align="center"><%# WebSanitizer.HtmlEncode(((ReturnOrderItem)Container.DataItem).ProductId)%></td>
																	<td align="left" rowspan="2">
																		<%# WebSanitizer.HtmlEncode(((ReturnOrderItem)Container.DataItem).ProductName) %><br />
																		<span visible='<%# WebSanitizer.HtmlEncode(((ReturnOrderItem)Container.DataItem).OrderSetPromotionNo) != "" %>' runat="server">
																			[<%# WebSanitizer.HtmlEncode(((ReturnOrderItem)Container.DataItem).OrderSetPromotionNo) %>：<%# WebSanitizer.HtmlEncode(this.ReturnExchangeOrderOrg.GetOrderSetPromotionName(((ReturnOrderItem)Container.DataItem).OrderSetPromotionNo)) %>]
																		</span>
																	</td>
																	<td align='<%# ((ReturnOrderItem)Container.DataItem).IsSubscriptionBoxFixedAmount == false ? "right" : "center" %>' rowspan="2" Visible="<%# this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
																		<%#: ((ReturnOrderItem)Container.DataItem).IsSubscriptionBoxFixedAmount == false ? ((ReturnOrderItem)Container.DataItem).ProductPrice.ToPriceString(true) : "-" %>
																	</td>
																	<td align="center" rowspan="2">
																		<%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(((ReturnOrderItem)Container.DataItem).ItemQuantity)) %>
																	</td>
																	<td align="center" rowspan="2" Visible="<%# this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
																		<%#: ((ReturnOrderItem)Container.DataItem).IsSubscriptionBoxFixedAmount == false ? string.Format("{0}%", TaxCalculationUtility.GetTaxRateForDIsplay(((ReturnOrderItem)Container.DataItem).ProductTaxRate)) : "-" %>
																	</td>
																	<td align='<%# ((ReturnOrderItem)Container.DataItem).IsSubscriptionBoxFixedAmount == false ? "right" : "center" %>' rowspan="2" Visible="<%# this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
																		<div Visible="<%# ((ReturnOrderItem)Container.DataItem).IsSubscriptionBoxFixedAmount == false %>" runat="server">
																			<%#: ((ReturnOrderItem)Container.DataItem).ItemPrice.ToPriceString(true) %>
																		</div>
																		<div Visible="<%# ((ReturnOrderItem)Container.DataItem).IsSubscriptionBoxFixedAmount %>" runat="server">
																			定額(<%#: ((ReturnOrderItem)Container.DataItem).SubscriptionBoxFixedAmount.ToPriceString(true) %>)
																		</div>
																	</td>
																</tr>
																<tr class="detail_item_bg">
																	<td align="center" runat="server" visible="<%# (Constants.RECOMMEND_OPTION_ENABLED && Constants.NOVELTY_OPTION_ENABLED) %>">
																		<%#: (((ReturnOrderItem)Container.DataItem).RecommendId != "") ? ((ReturnOrderItem)Container.DataItem).RecommendId : "-" %>
																	</td>
																	<td align="center"><%# (((ReturnOrderItem)Container.DataItem).VId != "") ? WebSanitizer.HtmlEncode("商品ID + " + ((ReturnOrderItem)Container.DataItem).VId) : "-"%></td>
																</tr>
																<tr visible='<%# StringUtility.ToEmpty(((ReturnOrderItem)Container.DataItem).ProductOptionValue) != "" %>' class="detail_item_bg" runat="server">
																	<td class="detail_title_bg" align="center">付帯情報</td>
																	<td align="left" colspan="<%# (this.IsSubscriptionBoxFixedAmount ? 3 : 5) + this.AddColumnCountForItemTable %>"><%# WebSanitizer.HtmlEncode(ProductOptionSettingHelper.GetDisplayProductOptionTexts(((ReturnOrderItem)Container.DataItem).ProductOptionValue)) %></td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
														<tr>
															<td class="detail_title_bg" align="right" colspan='<%: (this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() ? 3 : 5) + (this.DisplayItemSubscriptionBoxCourseIdArea ? -1 : 0) + this.AddColumnCountForItemTable %>'>交換商品合計（<%: this.ProductPriceTextPrefix %>）</td>
															<td class="detail_item_bg" align="right"><asp:Label ID="lbExchangeOrderPriceSubTotal" runat="server"></asp:Label></td>
														</tr>
													</tbody>
												</table>
												</div>
												<%--△ 返品商品情報 △--%>
												<%--▽ 合計情報 ▽--%>
												<br />
												<table width="758" border="0" cellspacing="0" cellpadding="0">
													<tbody>
														<tr align="right" valign="top">
															<td  id ="tdOrderPointAdd" align="right" runat="server">
															<%--▽ 付与ポイント情報 ▽--%>
															<%-- ポイントオプションが有効の場合--%>
															<%if (Constants.W2MP_POINT_OPTION_ENABLED) { %>
																<table class="edit_table" cellspacing="1" cellpadding="3" width="400" border="0">
																	<tbody>
																		<tr style="height: 21px">
																			<td class="edit_title_bg" align="center" colspan="4">付与ポイント情報</td>
																		</tr>
																		<tr style="height: 21px">
																			<td class="detail_title_bg" align="left" colspan="2">付与済み<%= trOrderBasePointAddComp.Visible ? "本ポイント" : "仮ポイント" %></td>
																			<td class="detail_title_bg" align="left">調整ポイント</td>
																			<td class="detail_title_bg" align="left">調整後ポイント</td>
																		</tr>
																		<%-- ユーザポイント(仮ポイント)が存在する --%>
																		<asp:Repeater id="rOrderPointAddTemp" runat="server">
																			<ItemTemplate>
																				<asp:Repeater DataSource='<%# (List<Hashtable>)Container.DataItem %>' runat="server">
																					<ItemTemplate>
																						<tr>
																							<td class="detail_title_bg" align="left" width="130">
																								<%# ((string)((Hashtable)Container.DataItem)[Constants.FIELD_USERPOINT_POINT_KBN] == Constants.FLG_USERPOINT_POINT_KBN_BASE)
																									? "通常 付与ポイント"
																									: "期間限定 付与ポイント"%>
																								(<%#: ValueText.GetValueText(
																									Constants.TABLE_USERPOINT,
																									Constants.FIELD_USERPOINT_POINT_INC_KBN,
																									((Hashtable)Container.DataItem)[Constants.FIELD_USERPOINT_POINT_INC_KBN]) %>)
																							</td>
																							<td class="detail_item_bg" align="left" width="50"><%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(((Hashtable)Container.DataItem)[Constants.FIELD_USERPOINT_POINT  + Constants.FIELD_COMMON_BEFORE])) %>pt</td>
																							<td class="detail_item_bg" align="left"width="50"><%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(((Hashtable)Container.DataItem)[CONST_ORDER_POINT_ADD_ADJUSTMENT])) %>pt</td>
																							<td class="detail_item_bg" align="left" width="70"><%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(((Hashtable)Container.DataItem)[Constants.FIELD_USERPOINT_POINT])) %>pt</td>
																						</tr>
																					</ItemTemplate>
																				</asp:Repeater>
																			</ItemTemplate>
																		</asp:Repeater>
																		<%-- 本ポイントが存在する場合 --%>
																		<tr id="trOrderBasePointAddComp" runat="server" visible="false">
																			<td class="detail_title_bg" align="left">通常 付与ポイント</td>
																			<td class="detail_item_bg" align="left"><asp:Literal ID="lOrderBasePointAddBefore" runat="server"></asp:Literal>pt</td>
																			<td class="detail_item_bg" align="left"><asp:Literal ID="lOrderBasePointAddAdjustment" runat="server"/> pt</td>
																			<td class="detail_item_bg" align="left"><asp:Literal ID="lOrderBasePointAdd" runat="server"></asp:Literal> pt</td>
																		</tr>
                                                                        <tr id="trOrderLimitPointAddComp" runat="server" visible="false">
                                                                            <td class="detail_title_bg" align="left">期間限定 付与ポイント</td>
                                                                            <td class="detail_item_bg" align="left"><asp:Literal ID="lOrderLimitPointAddBefore" runat="server"></asp:Literal>pt</td>
                                                                            <td class="detail_item_bg" align="left"><asp:Literal ID="lOrderLimitPointAddAdjustment" runat="server"/> pt</td>
                                                                            <td class="detail_item_bg" align="left"><asp:Literal ID="lOrderLimitPointAdd" runat="server"></asp:Literal> pt</td>
                                                                        </tr>
																	</tbody>
																</table>
																<br/>
																<table class="edit_table" cellspacing="1" cellpadding="3" width="400" border="0">
																	<tbody>
																		<tr style="height: 21px">
																			<td class="edit_title_bg" align="center" colspan="3">利用ポイント情報</td>
																		</tr>
																		<tr style="height: 21px">
																			<td class="detail_title_bg" align="left">変更前最終利用ポイント</td>
																			<td class="detail_title_bg" align="left">調整ポイント</td>
																			<td class="detail_title_bg" align="left">調整後最終利用ポイント</td>
																		</tr>
																		<tr>
																			<td class="detail_item_bg" align="left"><asp:Literal ID="lLastOrderPointUseBefore" runat="server"></asp:Literal>pt</td>
																			<td class="detail_item_bg" align="left"><asp:Literal ID="lOrderPointUseAdjustment" runat="server"/> pt</td>
																			<td class="detail_item_bg" align="left"><asp:Literal ID="lLastOrderPointUse" runat="server"></asp:Literal> pt</td>
																		</tr>
																	</tbody>
																</table>
															<%} %>
															<%--△ 付与ポイント情報 △--%>
															</td>
															<%--▽ 返品交換合計情報 ▽--%>
															<td align="right">
																<table width="208" border="0" cellspacing="0" cellpadding="0">
																	<tbody>
																		<tr align="right" valign="top">
																			<td>
																				<table class="detail_table" cellspacing="1" cellpadding="3" width="250" border="0">
																					<tbody>
																						<tr>
																							<td class="detail_title_bg" align="center" colspan="2">返品交換合計情報</td>
																						</tr>
																						<tr>
																							<td class="detail_title_bg" align="right" width="61%">商品合計（<%: this.ProductPriceTextPrefix %>）
																							</td>
																							<td class="detail_item_bg" align="right"><asp:Label ID="lbReturnExchangeOrderPriceSubTotal" runat="server"></asp:Label></td>
																						</tr>
																						<%if (this.ProductIncludedTaxFlg == false) { %>
																							<tr>
																								<td class="detail_title_bg" align="right" width="61%">消費税</td>
																								<td class="detail_item_bg" align="right"><asp:Label ID="lbReturnExchangeOrderPriceTax" runat="server"></asp:Label></td>
																							</tr>
																						<%} %>
																						<asp:Repeater ID="rPriceRegulation" ItemType="w2.Domain.Order.OrderPriceByTaxRateModel" runat="server">
																							<ItemTemplate>
																						<tr>
																									<td class="edit_title_bg" align="right">
																										返品用金額補正(税率<%#: TaxCalculationUtility.GetTaxRateForDIsplay(Item.KeyTaxRate)%>%)
																									</td>
																									<td class="edit_item_bg" align="right">
																										<%# GetMinusNumberNoticeHtml(Item.ReturnPriceCorrectionByRate, true) %>
																									</td>
																								</tr>
																							</ItemTemplate>
																						</asp:Repeater>
																						<asp:Repeater ID="rReturnExchangePriceByTaxRate" ItemType="w2.Domain.Order.OrderPriceByTaxRateModel" runat="server">
																							<ItemTemplate>
																								<tr runat="server">
																									<td class="edit_title_bg" align="right">
																										最終合計金額内訳(税率<%#: TaxCalculationUtility.GetTaxRateForDIsplay(Item.KeyTaxRate)%>%)
																									</td>
																									<td class="edit_item_bg" align="right">
																										<%# GetMinusNumberNoticeHtml(Item.PriceTotalByRate, true) %>
																									</td>
																						</tr>
																							</ItemTemplate>
																						</asp:Repeater>
																						<tr>
																							<td class="detail_title_bg" align="right">最終合計金額</td>
																							<td class="detail_item_bg" align="right"><asp:Label ID="lbReturnExchangeOrderPriceTotal" runat="server"></asp:Label></td>
																						</tr>
																					</tbody>
																				</table>																			
																				<br />
																				<table class="detail_table" cellspacing="1" cellpadding="3" width="250" border="0">
																					<tbody>
																						<tr>
																							<td class="detail_title_bg" align="center" width="54%">調整金額メモ</td>
																						</tr>
																						<tr>
																							<td class="detail_item_bg" align="left"><asp:Literal ID="lRegulationMemo" runat="server"></asp:Literal>&nbsp;</td>
																						</tr>
																					</tbody>
																				</table>
																				<br />
																				<table class="detail_table" cellspacing="1" cellpadding="3" width="250" border="0">
																					<tbody>
																						<tr>
																							<td class="detail_title_bg" align="center" colspan="2">返金情報</td>
																						</tr>
																						<tr>
																							<td class="detail_title_bg" align="right" width="61%">返金金額</td>
																							<td class="detail_item_bg" align="right"><asp:Literal ID="lOrderPriceRepayment" runat="server"></asp:Literal></td>
																						</tr>
																						<tr>
																							<td class="detail_title_bg" align="center" colspan="2">返金先情報</td>
																						</tr>
																						<tr id="trRepaymentMemo" runat="server">
																							<td class="detail_item_bg" align="left" colspan="2"><asp:Literal ID="lRepaymentMemo" runat="server"></asp:Literal>&nbsp;</td>
																						</tr>
																					</tbody>
																					<tbody id="tbodyRepaymentBank" visible="false" runat="server">
																						<tr>
																							<td class="detail_title_bg" align="right" width="61%">銀行コード</td>
																							<td class="detail_item_bg" align="right"><asp:Literal ID="lRepaymentBankCode" runat="server"></asp:Literal></td>
																						</tr>
																						<tr>
																							<td class="detail_title_bg" align="right" width="61%">銀行名</td>
																							<td class="detail_item_bg" align="right"><asp:Literal ID="lRepaymentBankName" runat="server"></asp:Literal></td>
																						</tr>
																						<tr>
																							<td class="detail_title_bg" align="right" width="61%">支店名</td>
																							<td class="detail_item_bg" align="right"><asp:Literal ID="lRepaymentBankBranch" runat="server"></asp:Literal></td>
																						</tr>
																						<tr>
																							<td class="detail_title_bg" align="right" width="61%">口座番号</td>
																							<td class="detail_item_bg" align="right"><asp:Literal ID="lRepaymentBankAccountNo" runat="server"></asp:Literal></td>
																						</tr>
																						<tr>
																							<td class="detail_title_bg" align="right" width="61%">口座名</td>
																							<td class="detail_item_bg" align="right"><asp:Literal ID="lRepaymentBankAccountName" runat="server"></asp:Literal></td>
																						</tr>
																					</tbody>
																				</table>
																				<br />
																			</td>
																		</tr>
																	</tbody>
																</table>
															</td>
															<%--△ 返品交換合計情報 △--%>
														</tr>
													</tbody>
												</table>
												<%--▽ 請求金額情報 ▽--%>
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<asp:Repeater ID="rLastBilledAmountByTaxRate" ItemType="w2.Domain.Order.OrderPriceByTaxRateModel" runat="server">
															<ItemTemplate>
																<tr runat="server">
																	<td class="edit_title_bg" align="right">
																		請求金額内訳(税率<%#: TaxCalculationUtility.GetTaxRateForDIsplay(Item.KeyTaxRate) %>%)
																	</td>
																	<td class="edit_item_bg" align="right">
																		<%# GetMinusNumberNoticeHtml(Item.PriceTotalByRate, true) %>
																	</td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
														<tr style="font-weight:bold;line-height:normal;">
															<td class="detail_title_bg" align="right">今回の請求金額</td>
															<td class="detail_item_bg" align="right" width="13%"><asp:Label ID="lbReturnExchangeLastBilledAmount" runat="server"></asp:Label></td>
														</tr>
														<%if (Constants.GLOBAL_OPTION_ENABLE) { %>
														<tr style="font-weight:bold;line-height:normal;">
															<td class="detail_title_bg" align="right">今回の決済金額</td>
															<td class="detail_item_bg" align="right" width="13%"><asp:Label ID="lbReturnExchangeSettlementAmount" runat="server"></asp:Label></td>
														</tr>
														<% } %>
													</tbody>
												</table>
												<%--△ 請求金額情報 △--%>
												<%--△ 合計情報 △--%>
												<div class="action_part_bottom">
													<input type="button" onclick="Javascript:history.back();" value="  戻る  " />
													<asp:Button id="btnRegistBottom" runat="server" Text="  登録する  " OnClientClick="return exec_submit()" OnClick="btnRegist_Click" />
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
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<script type="text/javascript">
<!--
	var exec_submit_flg = 0;
	function exec_submit() {
		if (exec_submit_flg == 0) {
			exec_submit_flg = 1;
			return true;
		}
		else {
			return false;
		}
	}
//-->
</script>
</asp:Content>
