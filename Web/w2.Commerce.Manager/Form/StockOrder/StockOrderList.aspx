<%--
=========================================================================================================
  Module      : 発注入庫情報一覧ページ(StockOrderList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="StockOrderList.aspx.cs" Inherits="Form_StockOrder_StockOrderList" %>
<%-- マスタ出力コントロール --%>
<%@ Register TagPrefix="uc" TagName="MasterDownLoad" Src="~/Form/Common/MasterDownLoadPanel.ascx" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<%-- 
テキストボックス内でEnterキーを押してSubmit（一番上に配置されているTextBoxのSubmit）送信しようとすると、
IEのバグでテキストボックスが画面上に一つのみ配置されている場合にSubmit送信されない不具合の対応として、
ダミーのTextBoxを非表示で配置している。
--%>
<asp:TextBox id="tbDummy" runat="server" style="visibility:hidden;display:none;"></asp:TextBox>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">発注入庫管理</h1></td>
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
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td class="search_table">
												<table cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															発注ID</td>
														<td class="search_item_bg" width="130"><asp:TextBox id="tbStockOrderId" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															発注連携ID</td>
														<td class="search_item_bg" width="130"><asp:TextBox id="tbRelationId" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															並び順</td>
														<td class="search_item_bg" width="130"><asp:DropDownList id="ddlSortKbn" runat="server"></asp:DropDownList></td>
														<td class="search_btn_bg" width="83" rowspan="6">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_STOCKORDER_LIST %>">クリア</a>&nbsp;
																<a href="javascript:Reset();">リセット</a></div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															商品ID</td>
														<td class="search_item_bg" width="130"><asp:TextBox id="tbProductId" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															発注Ｓ</td>
														<td class="search_item_bg" width="130"><asp:DropDownList ID="ddlOrderStatus" runat="server"></asp:DropDownList></td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															入庫Ｓ</td>
														<td class="search_item_bg" width="130"><asp:DropDownList ID="ddlDeliveryStatus" runat="server"></asp:DropDownList></td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
																発注日</td>
														<td class="search_item_bg" colspan="5">
															<uc:DateTimePickerPeriodInput id="ucOrderDatePeriod" runat="server" IsNullStartDateTime="true"/>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
																入庫日</td>
														<td class="search_item_bg" colspan="5">
															<uc:DateTimePickerPeriodInput id="ucDeliveryDatePeriod" runat="server" IsNullStartDateTime="true"/>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr><td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
										<tr>
											<td class="search_table">
												<uc:MasterDownLoad runat="server" ID="uMasterDownload" DownloadType="StockOrder" TableWidth="758" />
											</td>
										</tr>
										<tr><td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
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
		<td><h2 class="cmn-hed-h2">発注入庫情報一覧</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<div id="divOrderList" runat="server">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td align="left">
													<!--▽ ページング ▽-->
													<table class="list_pager" cellspacing="0" cellpadding="0" width="750" border="0">
														<tr>
															<td><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
															<td class="action_list_sp">
																<div align="right">
																	<asp:Button id="btnOrderTop" runat="server" Text="  新規発注  " OnClick="btnOrderTop_Click" /></div></td>
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
													<!--div class="x_scrollable" style="WIDTH: 750px"-->
													<table class="list_table" cellspacing="1" cellpadding="3" width="750" border="0">
														<tr class="list_title_bg">
															<td align="center" width="15%">発注ID</td>
															<td align="center" width="15%">発注連携ID</td>
															<td align="center" width="10%">発注Ｓ</td>
															<td align="center" width="10%">入庫Ｓ</td>
															<td align="center" width="10%">発注済<br />商品点数</td>
															<td align="center" width="10%">入庫済<br />商品点数</td>
															<td align="center" width="15%">発注日</td>
															<td align="center" width="15%">入庫日</td>
														</tr>
														<asp:Repeater id="rList" Runat="server">
															<ItemTemplate>
																<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateDetailUrl((String)Eval(Constants.FIELD_STOCKORDER_STOCK_ORDER_ID))) %>')">
																	<td align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_STOCKORDER_STOCK_ORDER_ID)) %></td>
																	<td align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_STOCKORDER_RELATION_ID)) %></td>
																	<td align="center"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_STOCKORDER, Constants.FIELD_STOCKORDER_ORDER_STATUS, Eval(Constants.FIELD_STOCKORDER_ORDER_STATUS))) %></td>
																	<td align="center"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_STOCKORDER, Constants.FIELD_STOCKORDER_DELIVERY_STATUS, Eval(Constants.FIELD_STOCKORDER_DELIVERY_STATUS))) %></td>
																	<td align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_STOCKORDER_ORDER_ITEM_COUNT)) %></td>
																	<td align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_STOCKORDER_DELIVERY_ITEM_COUNT)) %></td>
																	<td align="center"><%#: DateTimeUtility.ToStringForManager(Eval(Constants.FIELD_STOCKORDER_ORDER_DATE), DateTimeUtility.FormatType.ShortDate2Letter) %></td>
																	<td align="center"><%#: DateTimeUtility.ToStringForManager(Eval(Constants.FIELD_STOCKORDER_DELIVERY_DATE), DateTimeUtility.FormatType.ShortDate2Letter) %></td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
														<tr id="trListError" class="list_alert" runat="server" Visible="False">
															<td id="tdErrorMessage" colspan="8" runat="server"></td>
														</tr>
													</table>
													<!--/div-->
												</td>
											</tr>
											<tr>
												<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
	<!--△ 一覧 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<script>
	// Reset
	function Reset() {
		document.getElementById('<%= ucDeliveryDatePeriod.HfStartDate.ClientID %>').value = '';
		document.getElementById('<%= ucDeliveryDatePeriod.HfStartTime.ClientID %>').value = '';
		document.getElementById('<%= ucDeliveryDatePeriod.HfEndDate.ClientID %>').value = '';
		document.getElementById('<%= ucDeliveryDatePeriod.HfEndTime.ClientID %>').value = '';
		reloadDisplayDateTimePeriod('<%= ucDeliveryDatePeriod.ClientID %>');

		document.getElementById('<%= ucOrderDatePeriod.HfStartDate.ClientID %>').value = '';
		document.getElementById('<%= ucOrderDatePeriod.HfStartTime.ClientID %>').value = '';
		document.getElementById('<%= ucOrderDatePeriod.HfEndDate.ClientID %>').value = '';
		document.getElementById('<%= ucOrderDatePeriod.HfEndTime.ClientID %>').value = '';
		reloadDisplayDateTimePeriod('<%= ucOrderDatePeriod.ClientID %>');
		document.getElementById('<%= this.Form.ClientID %>').reset();
	}
</script>
</asp:Content>

