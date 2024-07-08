<%--
=========================================================================================================
  Module      : 売上状況レポート一覧ページ(OrderConditionReportList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OrderConditionReportList.aspx.cs" Inherits="Form_OrderConditionReport_OrderConditionReportList" MaintainScrollPositionOnPostback="true" %>
<%@ Import Namespace="w2.App.Common.Manager" %>
<%@ Import Namespace="w2.Domain.MenuAuthority.Helper" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<script type="text/javascript">
<!--
	// 商品一覧で選択された商品情報を設定
	function set_productinfo(product_id)
	{
		document.getElementById('<%= tbProductId.ClientID %>').value = product_id;
		__doPostBack('<%= tbProductId.UniqueID %>', '');
	}
-->
</script>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<tb><h1 class="page-title">売上状況レポート</h1></tb>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr> <!--▽ 検索 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">レポート対象選択</h2></td>
	</tr>
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
											<td class="search_box">
												<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
													<tr class="search_title_bg">
														<td width="30%" colspan="2"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />対象年月</td>
													</tr>
													<tr class="search_item_bg">
														<td width="200">
															<!-- ▽カレンダ▽ --><asp:label id="lblCurrentCalendar" Runat="server"></asp:label>
															<!-- △カレンダ△ -->
														</td>
														<td width="558">
															<table cellspacing="0" cellpadding="0" border="0" class="order-condition-report-list-search">
																<tr>
																	<td class="search_box" width="5"></td>
																	<td class="search_box">
																	<table cellspacing="0" cellpadding="0" border="0">
																	<tr>
																	<td class="search_box" valign="top">
																		<table class="search_table" cellspacing="1" cellpadding="2" width="100" border="0">
																			<tr class="search_title_bg">
																				<td width="30%"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />売上基準&nbsp;（<a href="#note">備考</a>）</td>
																			</tr>
																			<tr class="search_item_bg">
																				<td>
																					<asp:RadioButtonList id="rblSalesType" Runat="server" AutoPostBack="True" CssClass="radio_button_list">
																						<asp:ListItem Value="1" Selected="True">注文基準</asp:ListItem>
																						<asp:ListItem Value="0">出荷基準</asp:ListItem>
																					</asp:RadioButtonList>
																				</td>
																			</tr>
																		</table>
																	</td>
																	<td class="search_box" width="5"></td>
																	<td class="search_box" valign="top">
																		<table class="search_table" cellspacing="1" cellpadding="2" width="100" border="0">
																			<tr class="search_title_bg">
																				<td width="30%"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />レポート種別</td>
																			</tr>
																			<tr class="search_item_bg">
																				<td>
																					<asp:RadioButtonList id="rblReportType" Runat="server" AutoPostBack="True" CssClass="radio_button_list">
																						<asp:ListItem Value="0" Selected="True">日別レポート</asp:ListItem>
																						<asp:ListItem Value="1">月別レポート</asp:ListItem>
																					</asp:RadioButtonList>
																				</td>
																			</tr>
																		</table>
																	</td>
																	<td class="search_box" width="5"></td>
																	<td class="search_box" valign="top">
																		<table class="search_table" cellspacing="1" cellpadding="2" border="0" >
																			<tr class="search_title_bg">
																				<td><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />集計単位&nbsp;（<a href="#note">備考</a>）</td>
																			</tr>
																			<tr class="search_item_bg">
																				<td>
																					<asp:RadioButtonList id="rblAggregateUnit" Runat="server" AutoPostBack="True" CssClass="radio_button_list" RepeatDirection="Vertical" RepeatColumns="1" />
																				</td>
																			</tr>
																		</table>
																	</td>
																	<!-- 集計単位が注文単位の場合は非表示 -->
																	<td style="display : <%= Constants.ORDER_ITEM_DISCOUNTED_PRICE_ENABLE && (rblAggregateUnit.SelectedValue == KBN_AGGREGATE_UNIT_ORDERITEM) ? "" : "none" %>"-->
																	<table>
																	<tr>
																	<td class="search_box" width="5"></td>
																	<td class="search_box" valign="top" >
																		<table class="search_table" cellspacing="1" cellpadding="2" border="0" >
																			<tr class="search_title_bg">
																				<td><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />割引区分&nbsp;（<a href="#note">備考</a>）</td>
																			</tr>
																			<tr class="search_item_bg">
																				<td>
																					<asp:RadioButtonList id="rblDiscountType" Runat="server" AutoPostBack="True" CssClass="radio_button_list" RepeatDirection="Vertical" RepeatColumns="1" />
																				</td>
																			</tr>
																		</table>
																	</td>
																	</tr>
																	</table>
																	</td>
																	<td class="search_box" width="5"></td>
																	<td class="search_box" valign="top">
																		<table class="search_table" cellspacing="1" cellpadding="2" width="100" border="0">
																			<tr class="search_title_bg">
																				<td width="30%"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />消費税&nbsp;（<a href="#note">備考</a>）</td>
																			</tr>
																			<tr class="search_item_bg">
																				<td>
																					<asp:RadioButtonList id="rblTaxType" Runat="server" AutoPostBack="True" CssClass="radio_button_list" RepeatDirection="Vertical" RepeatColumns="1" />
																				</td>
																			</tr>
																		</table>
																	</td>
																	</tr>
																	</table>
																	</td>
																</tr>
																<tr>
																	<td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
																</tr>
																<tr style="display:<%= (rblReportType.SelectedValue == KBN_REPORT_TYPE_DAILY_REPORT) ? "" : "none" %>">
																	<td class="search_box" width="5"></td>
																	<td class="search_box">
																	<table cellspacing="0" cellpadding="0" border="0">
																	<tr>
																	<td>
																		<table class="search_table" cellspacing="1" cellpadding="2" border="0">
																			<tr>
																				<td class="search_title_bg" >
																					<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" 
																					border="0" />期間指定 (<%= CONST_GET_ORDER_CONDITION_DAYS%>日以内)</td>
																			</tr>
																			<tr>
																				<td class="search_item_bg" >
																					<uc:DateTimePickerPeriodInput ID="ucOrderConditionDatePeriod" runat="server" IsLoadPage="True" />
																					<br>
																					<asp:Label ID="lblOrderContitionError" style="color:#f00" runat="server"></asp:Label>
																				</td>
																			</tr>
																		</table>
																	</td>
																	</tr>
																	</table>
																	</td>
																</tr>
																<tr style="display:<%= (rblReportType.SelectedValue == KBN_REPORT_TYPE_DAILY_REPORT) ? "" : "none" %>">
																	<td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
																</tr>
																<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
																<%--▽ モール連携オプションが有効の場合 ▽--%>
																<tr>
																	<td class="search_box" width="5"></td>
																	<td class="search_box">
																	<table cellspacing="0" cellpadding="0" border="0">
																	<tr>
																	<td colspan="<%= (Constants.PRODUCT_BRAND_ENABLED) ? 8 : 6%>">
																		<table class="search_table" cellspacing="1" cellpadding="2" border="0">
																			<tr>
																				<td class="search_title_bg" width="130">
																					<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" 
																					border="0" />サイト</td>
																			</tr>
																			<tr>
																				<td class="search_item_bg" >
																					<asp:DropDownList id="ddlSiteName" AutoPostBack="true" runat="server">
																						<asp:ListItem Text="全体" Value="" />
																					</asp:DropDownList>
																				</td>
																			</tr>
																		</table>
																	</td>
																	</tr>
																	</table>
																	</td>
																</tr>
																<tr>
																	<td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
																</tr>
																<% } %>
																<%--△ モール連携オプションが有効の場合 △--%>
																<%--▽ 定期購入オプションが有効の場合 ▽--%>
																<tr runat="server" visible="<%# Constants.FIXEDPURCHASE_OPTION_ENABLED %>">
																	<td class="search_box" width="5"></td>
																	<td class="search_box" valign="top">
																		<table class="search_table" cellspacing="1" cellpadding="2" width="240" border="0">
																			<tr class="search_title_bg">
																				<td width="30%"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />通常注文/定期注文<span runat="server" Visible="<%# Constants.SUBSCRIPTION_BOX_OPTION_ENABLED %>">/頒布会注文</span></td>
																			</tr>
																			<tr class="search_item_bg">
																				<td>
																					<asp:CheckBoxList ID="cbOrderTypes" runat="server" RepeatColumns="3" RepeatDirection="Horizontal" AutoPostBack="True"></asp:CheckBoxList>
																				</td>
																			</tr>
																		</table>
																	</td>
																</tr>
																<tr>
																	<td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
																</tr>
																<%--△ 定期購入オプションが有効の場合 △--%>
																<tr>
																	<td class="search_box" width="5"></td>
																	<td class="search_box">
																	<table cellspacing="0" cellpadding="0" border="0">
																	<tr>
																	<td colspan="<%= (Constants.PRODUCT_BRAND_ENABLED) ? 8 : 6%>">
																		<table class="search_table" cellspacing="1" cellpadding="2" border="0">
																			<tr class="search_title_bg">
																				<td><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />購買区分</td>
																			</tr>
																			<tr class="search_item_bg">
																				<td>
																					<asp:CheckBoxList ID="cblOrderKbns" runat="server" RepeatColumns="8" AutoPostBack="True"></asp:CheckBoxList>
																				</td>																				
																			</tr>
																		</table>
																	</td>
																	</tr>
																	</table>
																	</td>
																</tr>
																<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
																<%--▽ グローバルオプションが有効の場合 ▽--%>
																<tr>
																	<td class="search_box" width="5"></td>
																	<td class="search_box">
																	<table cellspacing="0" cellpadding="0" border="0">
																	<tr>
																	<td colspan="6">
																		<table class="search_table" cellspacing="1" cellpadding="2" border="0">
																			<tr class="search_title_bg">
																				<td><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />国ISOコード</td>
																			</tr>
																			<tr class="search_item_bg">
																				<td>
																					<asp:CheckBoxList ID="cbCountryIsoCodeList" runat="server" RepeatColumns="6" AutoPostBack="True"></asp:CheckBoxList>
																				</td>
																			</tr>
																		</table>
																	</td>
																	</tr>
																	</table>
																	</td>
																</tr>
																<%--△ グローバルオプションが有効の場合 △--%>
																<% } %>
																<tr>
																	<td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
																</tr>
																<tr>
																	<td class="search_box" width="5"></td>
																	<td class="search_box">
																	<table cellspacing="0" cellpadding="0" border="0">
																	<tr>
																	<td class="search_box" valign="top">
																		<table class="search_table" cellspacing="1" cellpadding="2" width="200" border="0">
																			<tr class="search_title_bg">
																				<td width="30%"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />商品ID&nbsp;（<a href="#note">備考</a>）</td>
																			</tr>
																			<tr class="search_item_bg">
																				<td>
																					<asp:TextBox ID="tbProductId" runat="server" AutoPostBack="true" />
																					<input id="inputSelectProduct" type="button" value=" 選択 " onclick="javascript:open_window('<%: WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_PRODUCTLIST) %>','contact','width=828,height=500,top=120,left=420,status=NO,scrollbars=yes');" />
																				</td>
																			</tr>
																		</table>
																	</td>
																	<td class="search_box" width="5"></td>
																	<% if (Constants.PRODUCT_BRAND_ENABLED) {%>
																	<td class="search_box" valign="top" id="tdBrandSearch" runat="server" >
																		<table class="search_table" cellspacing="1" cellpadding="2" width="100" border="0">
																			<tr class="search_title_bg">
																				<td width="30%"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />ブランド&nbsp;（<a href="#note">備考</a>）</td>
																			</tr>
																			<tr class="search_item_bg">
																				<td>
																					<asp:DropDownList id="ddlBrandSearch" Runat="server" AutoPostBack="True" />
																				</td>
																			</tr>
																		</table>
																	</td>
																	<td class="search_box" width="53"></td>
																	<% } %>
																	</tr>
																	</table>
																	</td>
																</tr>
																<tr>
																	<td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
																</tr>
																<%--▽ タイムセールオプションが有効の場合 ▽--%>
																<tr>
																	<td class="search_box" width="5"></td>
																	<td class="search_box">
																	<table cellspacing="0" cellpadding="0" border="0">
																	<tr>
																	<td visible="<%# Constants.PRODUCT_SALE_OPTION_ENABLED %>" runat="server">
																		<table class="search_table" cellspacing="1" cellpadding="2" border="0" style="table-layout: fixed;">
																			<tr>
																				<td class="search_title_bg" width="130">
																					<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" 
																					border="0" />商品セール区分
																				</td>
																				<td class="search_title_bg" width="725">
																					<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" 
																					border="0" />商品セール名
																				</td>
																			</tr>
																			<tr>
																				<td class="search_item_bg">
																					<asp:DropDownList id="ddlProductSaleKbn" AutoPostBack="true" runat="server" Width="130px" OnTextChanged="ddlProductSaleKbn_TextChanged"></asp:DropDownList>
																				</td>
																				<td>
																					<asp:DropDownList id="ddlProductSaleName" AutoPostBack="true" runat="server" Width="255px"></asp:DropDownList>
																				</td>
																			</tr>
																		</table>
																	</td>
																	</tr>
																	</table>
																	</td>
																</tr>
																<tr>
																	<td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
																</tr>
																<%--△ タイムセールオプションが有効の場合 △--%>
																<tr>
																	<td class="search_box" width="5"></td>
																	<td class="search_box" valign="top">
																		<table class="search_table" cellspacing="1" cellpadding="2" width="450" border="0">
																			<tr class="search_title_bg">
																				<td width="30%"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />決済種別</td>
																			</tr>
																			<tr class="search_item_bg">
																				<td>
																					<asp:CheckBoxList ID="cblOrderPaymentKbns" runat="server" RepeatColumns="3" RepeatDirection="Horizontal" AutoPostBack="True"></asp:CheckBoxList>
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
	</tr> <!--△ 検索 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr> <!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">レポート表示</h2></td>
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
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table border="0" cellspacing="0" cellpadding="0">
													<tr>
														<td>
															<div class="search_btn_sub_rt" >
															&nbsp;</div>
															<table class="info_table" width="410" border="0" cellspacing="1" cellpadding="3">
																<tr class="info_item_bg">
																	<td align="left" id="tdReportInfo" runat="server">
																		<asp:Label ID="lbReportInfo" runat="server"></asp:Label><br>
																		<asp:Label ID="lbReportInfoOrderConditionDays" runat="server"></asp:Label>
																	</td>
																</tr>
															</table>
														</td>
														<td class="search_box" width="10"></td>
														<td>
															<div class="search_btn_sub_rt" >
																<asp:LinkButton id="lbReportExport" Runat="server" OnClick="lbReportExport_Click">CSVダウンロード</asp:LinkButton></div>
															<table class="info_table" width="280" border="0" cellspacing="1" cellpadding="3">
																<tr>
																	<td class="list_title_bg" align="center" width="110" id="tdDateAvgInfo" runat="server"></td>
																	<td class="info_item_bg" align="right" width="170" id="tdDateAvgPriceInfo" runat="server"></td>
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
										<!--▽ レポート ▽-->
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" border="0" width="700">
													<%--▽ 売上(注文基準) ▽--%>
													<% if (rblSalesType.SelectedValue == KBN_SALES_TYPE_ORDER_REPORT){ %>
													<tr class="list_title_bg">
														<td class="list_title_bg" style="white-space:nowrap" width="120" rowspan="2"></td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="245" colspan="3">売上(注文基準)</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="165" colspan="2">キャンセル</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="165" colspan="2">小計</td>
													</tr>
													<%--△ 売上(注文基準) △--%>
													<%--▽ 売上(出荷基準) ▽--%>
													<% }else if (rblSalesType.SelectedValue == KBN_SALES_TYPE_SHIP_REPORT){%>
													<tr class="list_title_bg">
														<td class="list_title_bg" style="white-space:nowrap" width="120" rowspan="2"></td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="245" colspan="3">売上(出荷基準)</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="165" colspan="2">返品</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="165" colspan="2">小計</td>
													</tr>
													<% } %>
													<%--△ 売上(出荷基準) △--%>
													<tr>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="98">金額</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="right" width="64"><%= (this.HasOrderAggregateUnit) ? "件数" : "個数"%></td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="83"><%= (this.HasOrderAggregateUnit) ? "平均購入単価" : "平均商品単価"%></td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="100">金額</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="right" width="65"><%= (this.HasOrderAggregateUnit) ? "件数" : "個数"%></td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="100">金額</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="right" width="65"><%= (this.HasOrderAggregateUnit) ? "件数" : "個数"%></td>
														
													</tr>
													<tr valign="top">
														<td class="list_item_bg5_dark" colspan="8" style="padding:0px"></td>
													</tr>
													<tr valign="middle"  height="20">
														<td class="list_title_bg" align="center">
															<a href="javascript:open_window('<%: CreateProductSaleRankingReportlUrlForTotal() %>','productranking','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes,resizable=yes');">合計</a>
														</td>
														<td class="list_item_bg1" align="right"><asp:Label ID="lbPriceTotal" runat="server"></asp:Label></td>
														<td class="list_item_bg1" align="right"><asp:Label ID="lbCountTotal" runat="server"></asp:Label><%= (this.HasOrderAggregateUnit) ? "件" : "個"%></td>
														<td class="list_item_bg1" align="right"><asp:Label ID="lbAvgTotal" runat="server"></asp:Label></td>
														<td class="list_item_bg1" align="right"><asp:Label ID="lbMinusPriceTotal" runat="server"></asp:Label></td>
														<td class="list_item_bg1" align="right" ID="tdTotalCancelNumber" runat="server">
															<% if (IsCancelNumberLinkDisplay) { %>
															<a href="#" onclick="javascript:open_window('<%#: SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Ec, CreateCancelOrderListUrl()) %>', 'orderlist', 'width=1200,height=850,top=5,left=600,status=NO,scrollbars=yes,resizable=yes');" title="<%# GetTitleForCancel((rblReportType.SelectedValue == KBN_REPORT_TYPE_DAILY_REPORT) ? "{0}～{1}" : "{0}年") %>のキャンセル一覧を表示">
															<% } %>
																<asp:Label ID="lbMinusCountTotal" runat="server"></asp:Label>
															<% if (IsCancelNumberLinkDisplay) { %>
															</a>
															<% } %>
															<%= (this.HasOrderAggregateUnit) ? "件" : "個"%>
														</td>
														<td class="list_item_bg1" align="right"><asp:Label ID="lbSubtotalPriceTotal" runat="server"></asp:Label></td>
														<td class="list_item_bg1" align="right"><asp:Label ID="lbSubtotalCountTotal" runat="server"></asp:Label><%= (this.HasOrderAggregateUnit) ? "件" : "個"%></td>
													</tr>
													<tr valign="top">
														<td class="list_item_bg5_dark" colspan="8" style="padding:0px"></td>
													</tr>
													<asp:Repeater id="rDataList" Runat="server" DataSource="<%# this.DisplayDataList %>">
														<ItemTemplate>
															<tr class='<%# ((Hashtable)Container.DataItem)["class"] %>'>
																<td class="list_title_bg" align="center">																
																	<%--▽ 月表示 ▽--%>
																	<% if (rblReportType.SelectedValue == KBN_REPORT_TYPE_MONTHLY_REPORT){ %>
																		<%# ((Hashtable)Container.DataItem)[FIELD_ORDERCONDITION_TARGET_DATE] + "月"%>
																		<% if (rblSalesType.SelectedValue == KBN_SALES_TYPE_ORDER_REPORT){ %>
																		<a href="javascript:open_window('<%# WebSanitizer.UrlAttrHtmlEncode(CreateMonthTimeReport(
																			((Hashtable)Container.DataItem)[Constants.REQUEST_KEY_TARGET_YEAR],
																		((Hashtable)Container.DataItem)[Constants.REQUEST_KEY_TARGET_MONTH])) %>
																		','productranking','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');">
																			<img src="../../Images/Icon/time.svg" style="width: 1vw; height: 1vw">
																		</a>
																		<% } %>
																		<a href="javascript:open_window('<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductSaleRankingReportlUrlForMonth(Container.ItemIndex + 1)) %>
																		','productranking','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');">
																		<img src="../../Images/Icon/item.svg" style="width: 1vw; height: 1vw">
																	</a>
																	<%--△ 月表示 △--%>
																	<%--▽ 日表示 ▽--%>
																	<% }else if (rblReportType.SelectedValue == KBN_REPORT_TYPE_DAILY_REPORT){%>
																	<%# ((Hashtable)Container.DataItem)[FIELD_ORDERCONDITION_TARGET_DATE]%>
																		<% if (rblSalesType.SelectedValue == KBN_SALES_TYPE_ORDER_REPORT){ %>
																		<a href="javascript:open_window('<%# CreateDayTimeReport(
																			((Hashtable)Container.DataItem)[Constants.REQUEST_KEY_TARGET_YEAR],
																			((Hashtable)Container.DataItem)[Constants.REQUEST_KEY_TARGET_MONTH],
																			((Hashtable)Container.DataItem)[Constants.REQUEST_KEY_TARGET_DAY],
																			((Hashtable)Container.DataItem)[Constants.REQUEST_KEY_REPORT_TIME_FROM],
																			((Hashtable)Container.DataItem)[Constants.REQUEST_KEY_REPORT_TIME_TO]) %>
																		','productranking','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');"> 
																			<img src="../../Images/Icon/time.svg" style="width: 1vw; height: 1vw">
																		</a>
																		<% } %>
																		<a href="javascript:open_window('<%# CreateProductSaleRankingReportlUrl(
																		((Hashtable)Container.DataItem)[Constants.REQUEST_KEY_TARGET_YEAR],
																		((Hashtable)Container.DataItem)[Constants.REQUEST_KEY_TARGET_MONTH],
																		((Hashtable)Container.DataItem)[Constants.REQUEST_KEY_TARGET_DAY],
																		((Hashtable)Container.DataItem)[Constants.REQUEST_KEY_REPORT_TIME_FROM],
																		((Hashtable)Container.DataItem)[Constants.REQUEST_KEY_REPORT_TIME_TO]) %>
																		','productranking','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');"> 
																		<img src="../../Images/Icon/item.svg" style="width: 1vw; height: 1vw">
																		</a>
																		
																	<% } %>
																	<%--△ 日表示 △--%>
																</td>
																<asp:Repeater DataSource='<%# ((Hashtable)Container.DataItem)["data"] %>' id="rLine" Runat="server">
																	<ItemTemplate>
																		<%--▽ 月表示 ▽--%>
																		<% if (rblReportType.SelectedValue == KBN_REPORT_TYPE_MONTHLY_REPORT){ %>
																			<td class="list_item_bg1" align="right">
																				<% if (IsCancelNumberLinkDisplay) { %>
																				<span Visible="<%# IsCancelOrderListLinkVisible(Container.ItemIndex) %>" runat="server"><a href="#" onclick="javascript:open_window('<%#: SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Ec, CreateCancelOrderListUrl((int)DataBinder.Eval(Container.Parent.Parent, "ItemIndex"))) %>', 'orderlist', 'width=1200,height=850,top=5,left=600,status=NO,scrollbars=yes,resizable=yes');" title="<%# GetTitleForCancel("{0}年{1}月", (int)DataBinder.Eval(Container.Parent.Parent, "ItemIndex")) %>のキャンセル一覧を表示"><%#: (Container.DataItem) %></a></span>
																				<span Visible="<%# (IsCancelOrderListLinkVisible(Container.ItemIndex) == false) %>" runat="server"><%#: (Container.DataItem) %></span>
																				<% } else { %>
																				<%#: Container.DataItem %>
																				<% } %>
																			</td>
																		<%--△ 月表示 △--%>
																		<%--▽ 日表示 ▽--%>
																		<% }else if (rblReportType.SelectedValue == KBN_REPORT_TYPE_DAILY_REPORT){%>
																			<td align="right">
																				<% if (IsCancelNumberLinkDisplay) { %>
																				<span Visible="<%# IsCancelOrderListLinkVisible(Container.ItemIndex) %>" runat="server"><a href="#" onclick="javascript:open_window('<%#: SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Ec, CreateCancelOrderListUrl((int)DataBinder.Eval(Container.Parent.Parent, "ItemIndex"))) %>', 'orderlist', 'width=1200,height=850,top=5,left=600,status=NO,scrollbars=yes,resizable=yes');" title="<%# GetTitleForCancel("{0}年{1}月", (int)DataBinder.Eval(Container.Parent.Parent, "ItemIndex")) %>のキャンセル一覧を表示"><%#: (Container.DataItem) %></a></span>
																				<span Visible="<%# (IsCancelOrderListLinkVisible(Container.ItemIndex) == false) %>" runat="server"><%#: (Container.DataItem) %></span>
																				<% } else { %>
																				<%#: Container.DataItem %>
																				<% } %>
																			</td>
																		<% } %>
																		<%--△ 日表示 △--%>
																	</ItemTemplate>
																</asp:Repeater>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
												</table>
											</td>
										</tr>
										<!--△ レポート △-->
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="info_table" width="700" border="0" cellspacing="1" cellpadding="3" id="note">
													<tr class="info_item_bg">
														<td align="left">
															備考<br />
															・売上基準を「出荷基準」に設定している場合、返品項目の金額、件数の表示対象は「返品注文」となります。<br/>
															　キャンセルされた「交換注文」についても返品として扱われますが「交換注文」は含まれません。<br />
															　なお、「交換注文」は、売上（注文基準）項目の金額、件数に含まれます。<br />
															・「商品ID」<%= (Constants.PRODUCT_BRAND_ENABLED) ? "「ブランド指定」" : ""%>は集計単位が「小計（商品単位）」の場合のみ利用可能です。<br />
															・注文単位で検索する場合、金額には注文時の手数料、割引金額を含みます。<br />
															　	※金額は、小計 + 配送料 + 手数料 + 調整金額<%= (Constants.MEMBER_RANK_OPTION_ENABLED) ? " - 会員ランク割引額 " : ""%><%= (Constants.W2MP_COUPON_OPTION_ENABLED) ? " - クーポン割引額 " : ""%><%= (Constants.W2MP_POINT_OPTION_ENABLED) ? " - 利用ポイント " : ""%>を含みます。<br />
															・商品単位の場合は注文時の手数料、割引金額を含みません。<br />
															・割引区分が「割引後」の場合は、割引
															<% if ((Constants.SETPROMOTION_OPTION_ENABLED || Constants.FIXEDPURCHASE_OPTION_ENABLED || Constants.MEMBER_RANK_OPTION_ENABLED || Constants.W2MP_COUPON_OPTION_ENABLED || Constants.W2MP_POINT_OPTION_ENABLED)) { %>
																（
																<%= (Constants.SETPROMOTION_OPTION_ENABLED) ? "「セットプロモーション割引」" : ""%>
																<%= (Constants.FIXEDPURCHASE_OPTION_ENABLED) ? "「定期購入回数割引」" : ""%>
																<%= (Constants.MEMBER_RANK_OPTION_ENABLED) ? "「会員ランク割引額」" : ""%>
																<%= (Constants.W2MP_COUPON_OPTION_ENABLED) ? "「クーポン割引額」" : ""%>
																<%= (Constants.W2MP_POINT_OPTION_ENABLED) ? "「利用ポイント」" : ""%>
																）
															<% } %>適用後の金額が表示されます。<br />
															・売上基準が「出荷基準」かつ集計単位が「小計（商品単位）」、割引区分が「割引後」の場合、税抜価格を表示できません。<br />
															・消費税が「税抜」の場合は、各合計金額欄の結果から消費税額を引いています。<br />
															<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED){ %>
															・集計単位が小計の場合、頒布会注文かつ定額機能を利用している注文は集計の対象外となります。
															<% } %>
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
	</tr> <!--△ 一覧 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<script type="text/javascript">
	$(function () {
		if ($('#<%# rblAggregateUnit.ClientID %>_0').is(':checked') === false) {
			$("#inputSelectProduct").removeAttr("disabled");
		} else {
			$("#inputSelectProduct").attr("disabled", "disabled");
		}
	})
</script>
</asp:Content>
