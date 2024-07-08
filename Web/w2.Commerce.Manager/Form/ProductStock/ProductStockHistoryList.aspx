<%--
=========================================================================================================
  Module      : 商品在庫履歴情報一覧ページ(ProductStockHistoryList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="System.Data" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="ProductStockHistoryList.aspx.cs" Inherits="Form_ProductStock_ProductStockHistoryList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="618" border="0">
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td>
			<h2 class="cmn-hed-h2">商品在庫履歴情報一覧</h2>
		</td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="698" border="0">				
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr id="trProduct" runat="server">
											<td>
												<table class="detail_table" cellspacing="1" cellpadding="3" width="650" border="0">
													<tr>
														<td class="detail_title_bg" align="left" width="25%">商品ID</td>
														<td class="detail_item_bg" align="left" width="25%"><asp:Label ID="lbProductId" runat="server"></asp:Label></td>
														<td class="detail_title_bg" align="left" width="25%">バリエーションID</td>
														<td class="detail_item_bg" align="left" width="25%"><asp:Label ID="lbVariationId" runat="server"></asp:Label></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">商品名</td>
														<td class="detail_item_bg" align="left" colspan="3"><asp:Label ID="lbName" runat="server"></asp:Label></td>

													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<!--▽ ページング ▽-->
												<table class="list_pager" cellspacing="0" cellpadding="0" width="584" border="0">
													<tr>
														<td width="504" style="height: 22px"><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
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
												<div class="x_scrollable" style="WIDTH: <%= Constants.REALSTOCK_OPTION_ENABLED ? 890 : 650 %>">
													<table class="list_table" cellspacing="1" cellpadding="3" width="<%= Constants.REALSTOCK_OPTION_ENABLED ? 884 : 644 %>" border="0">
														<% if (Constants.REALSTOCK_OPTION_ENABLED){ %>
														<tr class="list_title_bg">
															<td align="center" width="160" rowspan="3">作成日時</td>
															<td align="center" width="134" rowspan="3">アクション</td>
															<td align="center" width="350" colspan="5">在庫数増減</td>
															<td align="center" width="130" rowspan="3">更新メモ<br />[対象注文ID]</td>
															<td align="center" width="110" rowspan="3">最終更新者</td>
														</tr>
														<tr class="list_title_bg">
															<td align="center" width="70" rowspan="2">論理在庫</td>
															<td align="center" width="280" colspan="4">実在庫</td>
														</tr>
														<tr class="list_title_bg">
															<td align="center" width="70">A品</td>
															<td align="center" width="70">A品 引当済</td>
															<td align="center" width="70">B品</td>
															<td align="center" width="70">C品</td>
														</tr>
														<%--△ 実在庫利用が有効な場合は表示 △--%>
														<%--▽ 実在庫利用が無効な場合は表示 ▽--%>
														<%
														}else{ %>
														<tr class="list_title_bg">
															<td align="center" width="160" rowspan="1">作成日時</td>
															<td align="center" width="134" rowspan="1">アクション</td>
															<td align="center" width="100" colspan="1">論理在庫数増減</td>
															<td align="center" width="140" rowspan="1">更新メモ<br />[対象注文ID]</td>
															<td align="center" width="110" rowspan="1">最終更新者</td>
														</tr>
														<%} %>
														<asp:Repeater id="rList" Runat="server">
															<ItemTemplate>
																<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
																	<td align="center"><%#: DateTimeUtility.ToStringForManager(Eval(Constants.FIELD_PRODUCTSTOCKHISTORY_DATE_CREATED), DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
																	<td align="center"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTSTOCKHISTORY, Constants.FIELD_PRODUCTSTOCKHISTORY_ACTION_STATUS, (string)Eval(Constants.FIELD_PRODUCTSTOCKHISTORY_ACTION_STATUS)))%></td>
																	<td align="center"><%# DisplayProductStock((DataRowView)Container.DataItem, DISPLAY_PRODUCTSTOCK) %></td>
																	<%--▽ 実在庫利用が有効な場合は表示 ▽--%>
																	<% if (Constants.REALSTOCK_OPTION_ENABLED){ %>
																	<td align="center"><%# DisplayProductStock((DataRowView)Container.DataItem, DISPLAY_PRODUCTREALSTOCK) %></td>
																	<td align="center"><%# DisplayProductStock((DataRowView)Container.DataItem, DISPLAY_PRODUCTREALSTOCK_RESERVED) %></td>
																	<td align="center"><%# DisplayProductStock((DataRowView)Container.DataItem, DISPLAY_PRODUCTREALSTOCK_B) %></td>
																	<td align="center"><%# DisplayProductStock((DataRowView)Container.DataItem, DISPLAY_PRODUCTREALSTOCK_C) %></td>
																	<% } %>
																	<td align="center">
																		<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO)) %>
																		<div visible='<%# (string)Eval(Constants.FIELD_PRODUCTSTOCKHISTORY_ORDER_ID) != "" %>' runat="server">
																			[<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTSTOCKHISTORY_ORDER_ID)) %>]
																		</div>
																	</td>
																	<td align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTSTOCKHISTORY_LAST_CHANGED)) %></td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
														<tr id="trListError" class="list_alert" runat="server" Visible="false">
															<td id="tdErrorMessage" runat="server" colspan="9"></td>
														</tr>
													</table>
												</div>
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
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>