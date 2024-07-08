<%--
=========================================================================================================
  Module      : 発注入庫登録ページ(StockDeliveryRegist.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="StockDeliveryRegist.aspx.cs" Inherits="Form_StockOrder_StockDeliveryRegist" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">発注入庫管理</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 登録 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">発注入庫登録</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<div id="divStockEdit" runat="server">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td>
													<!--▽ ページング ▽-->
													<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
														<tr>
															<td class="action_list_sp"><div align="right">
																<input type="button" value="  戻る  " onclick="Javascript:history.back();" />
																<asp:Button id="btnOrderTop" runat="server" Text="  入庫登録する  " OnClientClick="return exec_submit()" OnClick="btnOrder_Click" /></div></td>
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
													<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">基本情報</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" style="width: 20%">発注ID</td>
															<td class="edit_item_bg" align="left">
																<%# WebSanitizer.HtmlEncode(m_drvStockOrder[Constants.FIELD_STOCKORDER_STOCK_ORDER_ID]) %>
																<asp:HiddenField ID="hfPsOrderId" runat="server" Value='<%# m_drvStockOrder[Constants.FIELD_STOCKORDER_STOCK_ORDER_ID] %>' />
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" style="width: 20%">発注連携ID</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox ID="tbRelationId" Text='<%# WebSanitizer.HtmlEncode(m_drvStockOrder[Constants.FIELD_STOCKORDER_RELATION_ID]) %>' runat="server"></asp:TextBox>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" style="width: 20%">発注日</td>
															<td class="edit_item_bg" align="left">
																<%#: DateTimeUtility.ToStringForManager(m_drvStockOrder[Constants.FIELD_STOCKORDER_ORDER_DATE], DateTimeUtility.FormatType.ShortDate2Letter) %>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" style="width: 20%">発注手配日</td>
															<td class="edit_item_bg" align="left">
																<%#: DateTimeUtility.ToStringForManager(m_drvStockOrder[Constants.FIELD_STOCKORDER_ORDER_INPUT_DATE], DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" style="width: 20%">入庫日</td>
															<td class="edit_item_bg" align="left">
																<asp:DropDownList ID="ddlDeliveryDateYear" runat="server"></asp:DropDownList>
																/
																<asp:DropDownList ID="ddlDeliveryDateMonth" runat="server"></asp:DropDownList>
																/
																<asp:DropDownList ID="ddlDeliveryDateDay" runat="server"></asp:DropDownList>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" style="width: 20%">入庫登録日</td>
															<td class="edit_item_bg" align="left">
																<%#: DateTimeUtility.ToStringForManager(m_drvStockOrder[Constants.FIELD_STOCKORDER_DELIVERY_INPUT_DATE], DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %>
															</td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<!--▽ 登録一覧表示 ▽-->												
												<td id="divOrderInput">
													<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr class="list_title_bg">
															<td align="center" width="10%" rowspan="2">商品ID</td>
															<td align="center" width="10%" rowspan="2">バリエーションID</td>
															<td align="center" width="30%" rowspan="2">商品名</td>
															<td align="center" width="20%" colspan="2">在庫数</td>
															<td align="center" width="10%" rowspan="2">入庫Ｓ</td>
															<td align="center" width="10%" rowspan="2">発注数</td>
															<td align="center" width="10%" rowspan="2">入庫数</td>
														</tr>
														<tr class="list_title_bg">
															<td align="center" width="8%" style="height: 16px">論理在庫</td>
															<td align="center" width="12%" style="height: 16px">実在庫 [引当済]</td>
														</tr>
														<asp:Repeater id="rInput" Runat="server">
															<ItemTemplate>
																<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
																	<td align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_STOCKORDERITEM_PRODUCT_ID)) %></td>
																	<td align="center"><%# WebSanitizer.HtmlEncode(((string)Eval(Constants.FIELD_PRODUCTVARIATION_V_ID) != "") ? (string)Eval(Constants.FIELD_PRODUCTVARIATION_V_ID) : "-") %></td>
																	<td align="left"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCT_NAME)) %></td>
																	<td align="center"><%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(Constants.FIELD_PRODUCTSTOCK_STOCK))) %></td>
																	<td align="center"><%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(Constants.FIELD_PRODUCTSTOCK_REALSTOCK))) %>
																		[<%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED))) %>]</td>
																	<td align="center"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_STOCKORDER, Constants.FIELD_STOCKORDER_DELIVERY_STATUS, Eval(Constants.FIELD_STOCKORDERITEM_DELIVERY_STATUS)))%></td>
																	<td align="center"><%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(Constants.FIELD_STOCKORDERITEM_ORDER_COUNT))) %></td>
																	<td align="center">
																		<asp:TextBox id="tbDeliveryCount" Runat="server" Width="50" MaxLength="5" Text='<%# Eval(Constants.FIELD_STOCKORDERITEM_DELIVERY_COUNT) %>'></asp:TextBox>
																		<asp:HiddenField ID="hfProductId" Value="<%# Eval(Constants.FIELD_PRODUCT_PRODUCT_ID) %>" runat="server" />
																		<asp:HiddenField ID="hfVariationId" Value="<%# Eval(Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) %>" runat="server" />
																		<asp:HiddenField ID="hfOrderCount" Value="<%# Eval(Constants.FIELD_STOCKORDERITEM_ORDER_COUNT) %>" runat="server" />
																		<asp:HiddenField ID="hfDeliveryCountBefore" Value="<%# Eval(Constants.FIELD_STOCKORDERITEM_DELIVERY_COUNT) %>" runat="server" />
																		<asp:HiddenField ID="hfRealstockUnreserved" Value="<%# int.Parse(Eval(Constants.FIELD_PRODUCTSTOCK_REALSTOCK).ToString()) - int.Parse(Eval(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED).ToString()) %>" runat="server" />
																	</td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
													</table>
												</td>
												<!--△ 編集一覧表示 △-->
											</tr>
											<tr>
												<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
											<td>
													<!--▽ ページング ▽-->
													<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
														<tr>
															<td class="action_list_sp"><div align="right">
																<input type="button" value="  戻る  " onclick="Javascript:history.back();" />
																<asp:Button id="btnOrderBottom" runat="server" Text="  入庫登録する  " OnClientClick="return exec_submit()" OnClick="btnOrder_Click" /></div></td>
														</tr>
													</table>
													<!-- ページング-->
												</td>
											</tr>
											<tr>
												<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
										</table>
									</div>
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
<script language="javascript">
<!--
var exec_submit_flg = 0;
function exec_submit()
{
	if (exec_submit_flg == 0)
	{
		exec_submit_flg = 1;
		return true;
	}
	else
	{
		return false;
	}
}
//-->
</script>
</asp:Content>

