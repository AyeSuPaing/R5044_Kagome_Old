<%--
=========================================================================================================
  Module      : 商品検索ページ(ProductSearch.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="ProductSearch.aspx.cs" Inherits="Form_Common_ProductSearch" Title="商品情報一覧" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="w2.Common.Web" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="528" border="0">
	<!--▽ 検索 ▽-->
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="714" border="0">
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
												<table cellspacing="1" cellpadding="2" width="700" border="0">
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															商品ID</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbProductId" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															商品名</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbName" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_btn_bg" width="83" rowspan="3">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="ProductSearch" /></div>
															<div class="search_btn_sub">
																<a href='<%= HtmlSanitizer.UrlAttrHtmlEncode(GetClearSearchUrl()) %>'>クリア</a>
																<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a></div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															配送種別</td>
														<td id="Td1" class="search_item_bg" width="130"  runat="server" colspan="<%# Constants.FIXEDPURCHASE_OPTION_ENABLED ? 1 : 3 %>">
															<asp:DropDownList ID="ddlShippingType" runat="server" Width="130" /></td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															通常/定期購入可否</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList ID="ddlFixedPurchase" runat="server" Width="130" /></td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															有効フラグ</td>
														<td class="search_item_bg" width="130" colspan="3">
															<asp:DropDownList ID="ddlValidFlg" runat="server" Width="130" /></td>
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
		<td>
			<h1 class="cmn-hed-h2">商品情報一覧</h1>
		</td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="714" border="0">
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<% if (this.IsNotSearchDefault) { %>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="690" border="0">
													<tr class="list_alert">
														<td><%= WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NOT_SEARCH_DEFAULT) %></td>
													</tr>
												</table>
											</td>
										</tr>
										<% } else { %>
										<tr>
											<td>
												<!-- ▽ページング▽ -->
												<table class="list_pager" cellspacing="0" cellpadding="0" width="700" border="0">
													<tr>
														<td style="height: 22px"><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
													</tr>
												</table>
												<!-- △ページング△ -->
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="690" border="0">
													<% if (this.ProductSearchKbn == Constants.KBN_PRODUCT_SEARCH_PRODUCT) { %>
													<tr class="list_title_bg">
														<td align="center" width="20%" style="height: 17px">商品ID</td>
														<td align="center" width="40%" style="height: 17px">商品名</td>
														<td align="center" width="20%" style="height: 17px">表示価格</td>
														<td align="center" width="20%" style="height: 17px">特別価格</td>
													</tr>
													<% } %>
													<% if (this.ProductSearchKbn == Constants.KBN_PRODUCT_SEARCH_VARIATION) { %>
													<tr class="list_title_bg">
														<td align="center" width="20%" style="height: 17px">商品ID</td>
														<td align="center" width="15%" style="height: 17px">バリエーションID</td>
														<td align="center" width="35%" style="height: 17px">商品名</td>
														<td align="center" width="15%" style="height: 17px">表示価格</td>
														<td align="center" width="15%" style="height: 17px">特別価格</td>
													</tr>
													<% } %>
													<% if ((this.ProductSearchKbn == Constants.KBN_PRODUCT_SEARCH_ORDERPRODUCT) || (this.ProductSearchKbn == Constants.KBN_PRODUCT_SEARCH_SUBSCRIPTION_BOX)) { %>
													<tr class="list_title_bg">
														<td align="center" width="20%" style="height: 17px">商品ID</td>
														<td align="center" width="12%" style="height: 17px">バリエーションID</td>
														<td align="center" width="40%" style="height: 17px">商品名</td>
														<td align="center" width="18%" style="height: 17px">価格</td>
														<td align="center" width="10%" style="height: 17px">在庫数<br />(<a href="#anchorRemarks" style="font-size:smaller">備考</a>)</td>
													</tr>
													<% } %>
													<asp:Repeater id="rList" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="javascript:set_productinfo(<%# WebSanitizer.HtmlEncode(CreateJavaScriptSetProductInfo((DataRowView)Container.DataItem)) %>)">
																<td align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCT_PRODUCT_ID)) %></td>
																<% if (this.ProductSearchKbn != Constants.KBN_PRODUCT_SEARCH_PRODUCT) { %>
																<td align="center"><%# WebSanitizer.HtmlEncode((this.ProductSearchKbn == Constants.KBN_PRODUCT_SEARCH_PRODUCT) ? "" : Eval(Constants.FIELD_PRODUCTVARIATION_V_ID))%></td>
																<% } %>
																<td align="left"><%#: (this.ProductSearchKbn == Constants.KBN_PRODUCT_SEARCH_PRODUCT) ? Eval(Constants.FIELD_PRODUCT_NAME) : CreateProductAndVariationName((System.Data.DataRowView)Container.DataItem)%>
																	<%# (StringUtility.ToEmpty(Eval(Constants.FIELD_PRODUCT_VALID_FLG)) == Constants.FLG_PRODUCT_VALID_FLG_INVALID) ? "<br /> （無効）" : ""%>
																</td>
																<% if ((this.ProductSearchKbn == Constants.KBN_PRODUCT_SEARCH_ORDERPRODUCT) || (this.ProductSearchKbn == Constants.KBN_PRODUCT_SEARCH_SUBSCRIPTION_BOX)) { %>
																<td align="right">
																	<%-- ▽商品会員ランク価格有効▽ --%>
																	<div align="right" visible='<%# GetProductMemberRankPriceValid(Container.DataItem, true) %>' runat="server">
																	<span class="productPrice"><strike><%#: GetProductPriceNumeric(Container.DataItem, true).ToPriceString(true) %></strike><br />
																	会員ランク価格:<%#: GetProductMemberRankPrice(Container.DataItem, true).ToPriceString(true) %></span>
																	</div>
																	<%-- △商品会員ランク価格有効△ --%>
																	<%-- ▽商品セール価格有効▽ --%>
																	<div align="right" visible='<%# GetProductTimeSalesValid(Container.DataItem) %>' runat="server">
																	<span class="productPrice"><strike><%#: GetProductPriceNumeric(Container.DataItem, true).ToPriceString(true) %></strike><br />
																	タイムセール価格:<%#: GetProductTimeSalePriceNumeric(Container.DataItem).ToPriceString(true) %></span>
																	</div>
																	<%-- △商品セール価格有効△ --%>
																	<%-- ▽商品特別価格有効▽ --%>
																	<div align="right" visible='<%# GetProductSpecialPriceValid(Container.DataItem, true) %>' runat="server">
																	<span class="productPrice"><strike><%#: GetProductPriceNumeric(Container.DataItem, true).ToPriceString(true) %></strike><br />
																	特別価格:<%#: GetProductSpecialPriceNumeric(Container.DataItem, true).ToPriceString(true) %></span>
																	</div>
																	<%-- △商品特別価格有効△ --%>
																	<%-- ▽商品通常価格有効▽ --%>
																	<div align="right" visible='<%# GetProductNormalPriceValid(Container.DataItem, true) %>' runat="server">
																	<span class="productPrice"><%#: GetProductPriceNumeric(Container.DataItem, true).ToPriceString(true) %></span>
																	</div>
																	<%-- △商品通常価格有効△ --%>
																	<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) {%>
																	<div runat="server" visible='<%# (GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG).ToString() != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) %>'>
																	<p visible='<%# IsProductFixedPurchaseFirsttimePriceValid(Container.DataItem, true) %>' runat="server">
																		定期初回価格:<%#: GetProductFixedPurchaseFirsttimePrice(Container.DataItem, true).ToPriceString(true) %>
																	</p>
																	<p>
																		定期通常価格:<%#: GetProductFixedPurchasePrice(Container.DataItem, true).ToPriceString(true) %>
																	</p>
																	</div>
																	<% } %>
																</td>
																<td align="<%# ((string)(((DataRowView)Container.DataItem)[Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN]) == Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED) ? "center" : "right" %>"><%#: GetManagedStock((DataRowView)Container.DataItem) %></td>
																<% } else { %>
																<td align="right"><%#: (this.ProductSearchKbn == Constants.KBN_PRODUCT_SEARCH_PRODUCT) ? Eval(Constants.FIELD_PRODUCT_DISPLAY_PRICE).ToPriceString(true) : Eval(Constants.FIELD_PRODUCTVARIATION_PRICE).ToPriceString(true) %></td>
																<td align="right"><%#: (this.ProductSearchKbn == Constants.KBN_PRODUCT_SEARCH_PRODUCT) ? Eval(Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE).ToPriceString(true) : Eval(Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE).ToPriceString(true) %></td>
																<% } %>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="7"></td>
													</tr>
												</table>
											</td>
										</tr>
										<% } %>
										<% if ((Request[Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN] == Constants.KBN_PRODUCT_SEARCH_ORDERPRODUCT) || (this.ProductSearchKbn == Constants.KBN_PRODUCT_SEARCH_SUBSCRIPTION_BOX)) { %>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
												<table id="anchorRemarks" class="info_table" width="690" border="0" cellspacing="1" cellpadding="3">
													<tr>
														<td align="left" class="info_item_bg">
														備考<br/>
														・在庫数の表示について<br />
														　「－」 在庫管理をしていません。<br />
														　「123」 数字のみの場合は在庫数が0以下では購入できません。<br />
														　「(123)」 括弧つきの数字の場合は在庫数が0以下でも購入できます。
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<% } %>
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
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<script type="text/javascript">
<!--
	// 選択された商品情報を設定
	function set_productinfo(product_id, supplier_id, v_id, product_name, display_price, display_special_price, product_price, sale_id, fixed_purchase_id, limitedfixedpurchasekbn1setting, limitedfixedpurchasekbn3setting, limitedfixedpurchasekbn4setting, tax_rate, shipping_id) {
		// 親ウィンドウが存在する場合
		if (window.opener != null) {
			// 選択された商品情報を設定    
			window.opener.set_productinfo(product_id, supplier_id, v_id, product_name, display_price, display_special_price, product_price, sale_id, fixed_purchase_id, limitedfixedpurchasekbn1setting, limitedfixedpurchasekbn3setting, limitedfixedpurchasekbn4setting, tax_rate, shipping_id);
			// 商品一覧ウィンドウ閉じる(ユーザビリティのため)
			window.close();
		}
	}
//-->
</script>
</asp:Content>