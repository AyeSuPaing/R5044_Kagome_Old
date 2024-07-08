<%--
=========================================================================================================
  Module      : 定期売上予測レポート(FixedPurchaseForecastReport.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="FixedPurchaseForecastReport.aspx.cs" Inherits="Form_FixedPurchaseForecastReport_FixedPurchaseForecastReport" %>
<%@ Import Namespace="w2.Common.Web" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.7.2/Chart.bundle.js"></script>
<script src="../../Js/fixedPurchaseForecastChart.js"></script>
	<script>
		function open_window(product_id, window_name, window_type) {
			var link_file = '<%= Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_FIXED_PURCHASE_FORECAST_REPORT_TRANSITIVE_GRAPH %>';
			link_file += '?<%= Constants.REQUEST_KEY_FIXEDPURCHASE_FORECAST_PRODUCT_ID %>=' + encodeURIComponent(product_id);
			link_file += '&<%= Constants.REQUEST_KEY_FIXEDPURCHASE_FORECAST_DISPLAY_KBN %>=' + encodeURIComponent("<%: this.DisplayKbn %>");
			var new_win = window.open(link_file, window_name, window_type);
			new_win.focus();
		}

		function fixed_header_width() {
			$('#fixedHeader').css('width', document.getElementById("headerProductName").getBoundingClientRect().width - 11);
			
			$('.headerRowerColumn1').css('width', document.getElementById("headerRower1").getBoundingClientRect().width - 11);
			$('.headerRowerColumn2').css('width', document.getElementById("headerRower2").getBoundingClientRect().width - 11);
		}

		window.onload = function () {
			fixed_header_width();
			var table_header_positon = $('#main_contents').offset().top;
			$(window).scroll(function () {
				var value = $(this).scrollTop();
				if (value < table_header_positon) {
					$('#table_header').css('display', 'none');
				} else {
					$('#table_header').css('display', 'table');
				}
			});
		};

		$(window).resize(function () {
			fixed_header_width();
		});

	</script>
	<style>

		#table_header {
			display: none;
			position: fixed;
			top: 40px;
			z-index: 100;
			table-layout: fixed;
		}

	</style>

	<table cellspacing="0" cellpadding="0" width="791" border="0">
		<tr>
			<td><h1 class="page-title">定期売上予測レポート</h1></td>
		</tr>
		<tr>
			<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
		</tr>
		<tr>
			<table class="search_table" cellspacing="1" cellpadding="3" width="700" border="0">
				<tr runat="server" Visible="<%# (Constants.SCHEDULED_SHIPPING_DATE_OPTION_ENABLE) %>">
					<td class="search_title_bg" width="150">売上予測算出基準</td>
					<td class="search_item_bg" colspan="5">
						<asp:RadioButtonList id="rblSalesForecastCalculationCriteria" CssClass="radio_button_list" RepeatDirection="Horizontal" AutoPostBack="True" RepeatLayout="Flow" OnSelectedIndexChanged="rblDisplayType_SelectedIndexChanged" runat="server"></asp:RadioButtonList>
					</td>
				</tr>
				<tr>
					<td class="search_title_bg" width="150">レポート種別</td>
					<td class="search_item_bg" colspan="5">
						<asp:RadioButtonList id="rblDisplayType" CssClass="radio_button_list" RepeatDirection="Horizontal" AutoPostBack="True" RepeatLayout="Flow" OnSelectedIndexChanged="rblDisplayType_SelectedIndexChanged" runat="server"></asp:RadioButtonList>
					</td>
				</tr>
			</table>
		</tr>
		<tr runat="server" Visible="<%# (this.IsMonthlyDisplay == false) %>" ID="trSearchBox">
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
														<tr>
															<td class="search_title_bg" width="140" style="height: 38px">
																商品ID</td>
															<td class="search_item_bg" style="height: 38px">
																<asp:TextBox id="tbProductId" runat="server"></asp:TextBox></td>
															<td class="search_title_bg" width="140" style="height: 38px">
																商品名</td>
															<td class="search_item_bg" style="height: 38px">
																<asp:TextBox ID="tbProductName" runat="server"></asp:TextBox></td>
															<td class="search_btn_bg" width="83" rowspan="2" style="height: 38px">
																<div class="search_btn_main">
																	<asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click"/></div>
																<div class="search_btn_sub">
																	<a href="<%= new UrlCreator(Request.Url.AbsolutePath).AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_FORECAST_DISPLAY_KBN, this.DisplayKbn).CreateUrl() %>">クリア</a>&nbsp;
																	<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a></div>
															</td>
														</tr>
														<tr>
															<td class="search_title_bg" width="140" style="height: 38px">
																配送種別</td>
															<td class="search_item_bg" style="height: 38px">
																<asp:DropDownList id="ddlShippingType" runat="server"></asp:DropDownList></td>
															<td class="search_title_bg" width="140" style="height: 38px">
																カテゴリID</td>
															<td class="search_item_bg" style="height: 38px">
																<asp:TextBox ID="tbCategoryId" runat="server"></asp:TextBox></td>
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
		<tr runat="server" Visible="<%# (IsMonthlyDisplay == false) %>" ID="trProductTable">
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<td>
						<table ID="tProductList" class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0" runat="server">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table border="0" cellspacing="0" cellpadding="0" width="758px" class="list_pager">
													<tr>
													<td width="675"><asp:Literal ID="lbPager" Runat="server"></asp:Literal></td>

													</tr>
												</table>
												<div class="search_btn_sub_rt" >
													<asp:LinkButton id="lbReportExport" Runat="server" OnClick="lbReportExport_Click">CSVダウンロード</asp:LinkButton>
												</div>
												<div id="table_header">
												<table class="list_table" border="0" cellspacing="0" cellpadding="0"style="position: relative; table-layout: fixed;">
													<tr class="list_title_bg">
														<th class="tableHeader" id="fixedHeader" align="center"rowspan="2" colspan="2">商品名<br>(商品ID)</th>
														<asp:Repeater runat="server" DataSource="<%# this.DisplayMonthlyList %>">
															<ItemTemplate>
																<th align="center" colspan="2"><%#: (string)Container.DataItem %></th>
															</ItemTemplate>
														</asp:Repeater>
													</tr>
													<tr class="list_title_bg">
														<asp:Repeater runat="server" DataSource="<%# this.DisplayMonthlyList %>">
															<ItemTemplate>
																<td align="center" class="headerRowerColumn1">金額</td>
																<td align="center" class="headerRowerColumn2">数量</td>
															</ItemTemplate>
														</asp:Repeater>
													</tr>
												</table>
												</div>

												<table id="main_contents" class="list_table" border="0" cellspacing="0" cellpadding="0" style="position: relative; table-layout: fixed;">
												<asp:Repeater ID="rProductList" ItemType="w2.Domain.FixedPurchaseForecast.Helper.FixedPurchaseForecastProductListSearchResult" Runat="server" Visible="<%# (IsSerachNoHit == false) %>">
													<HeaderTemplate>
													<tr class="list_title_bg">
																<th id="headerProductName" class="tableHeader" align="center" style="width:120px;" rowspan="2" colspan="2">商品名<br>(商品ID)</th>
																<asp:Repeater runat="server" DataSource="<%# this.DisplayMonthlyList %>">
																	<ItemTemplate>
																		<th align="center" colspan="2"><%#: (string)Container.DataItem %></th>
																	</ItemTemplate>
																</asp:Repeater>
															</tr>
															<tr class="list_title_bg">
																<asp:Repeater runat="server" DataSource="<%# this.DisplayMonthlyList %>">
																	<ItemTemplate>
																		<td align="center" style="width:82px;" id="headerRower1" class="headerRowerColumn2">金額</td>
																		<td align="center" style="width:82px;" id="headerRower2" class="headerRowerColumn2">数量</td>
																	</ItemTemplate>
																</asp:Repeater>
															</tr>
																
															<tr valign="top">
																<td class="list_item_bg5_dark" colspan="2" style="padding:0"></td>
															</tr>
													</HeaderTemplate>
													<ItemTemplate>
														<tr class="list_item_bg<%#: Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="open_window('<%#: (this.DisplayKbn == KBN_DISPLAY_PRODUCT) ? Item.ProductId : Item.VariationId %>', 'fixed_purchase_forecast_report', 'width=828,height=520,top=50,left=300,status=NO,scrollbars=yes')" >
															<td align="center" class="tableTitle" colspan="2" width="10%" height="35px"><%#: Item.ProductName %><br>(<%#: (this.DisplayKbn == KBN_DISPLAY_PRODUCT) ? Item.ProductId : Item.VariationId%>)</td>
															<asp:Repeater runat="server" DataSource="<%# Item.Item %>" ItemType="w2.Domain.FixedPurchaseForecast.Helper.FixedPurchaseForecastItemSearchResult">
																<ItemTemplate>
																	<td align="right"><%#: Item.Price.ToPriceString(true) %></td>
																	<td align="right"><%#: Item.Stock %>個</td>
																</ItemTemplate>
															</asp:Repeater>
														</tr>
													</ItemTemplate>
												</asp:Repeater>
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
					<table border="0" cellspacing="0" cellpadding="0" width="758px" class="list_table">
						<tr id="trListError" class="list_alert" runat="server" Visible="false">
							<td id="tdErrorMessage" colspan="6" runat="server"></td>
						</tr>
					</table>
				</tr>
			</table>
		</td>
	</tr>
	<tr runat="server" Visible="<%# IsMonthlyDisplay %>" ID="trMonthlyTable">
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<div class="search_btn_sub_rt" style=" padding-top: 20px;" ><asp:LinkButton id="LinkButton1" Runat="server" OnClick="lbReportExport_Click">CSVダウンロード</asp:LinkButton></div>
					<td style="width: 60%;">
						<table width="" border="0">
							<tr>
								<td>
									<table class="box_border" cellspacing="1" cellpadding="0" border="0">
										<tr>
											<td class="search_box_bg">
												<table cellspacing="0" cellpadding="0" width="100%" border="0">
													<tr>
														<td align="center">
															<table cellspacing="0" cellpadding="0" border="0">
																<tr>
																	<td class="search_box">
																		<table id="search_table1" class="search_table" width="100px" cellspacing="1" cellpadding="2" border="0">
																			<tr class="search_title_bg">
																				<td>
																					<table class="trans_table" width="100%" border="0">
																						<tr>
																							<td style="vertical-align: middle">
																								&nbsp;
																								グラフレポート
																							</td>
																							<td style="text-align: right">
																								<asp:RadioButtonList id="rblChartType" Runat="server" RepeatDirection="Horizontal" AutoPostBack="True" CssClass="radio_button_list" OnSelectedIndexChanged="rblChartType_OnSelectedIndexChanged">
																									<asp:ListItem Value="bar" Selected="True">棒グラフ</asp:ListItem>
																									<asp:ListItem Value="line">折れ線グラフ</asp:ListItem>
																								</asp:RadioButtonList>
																							</td>
																						</tr>
																					</table>
																				</td>
																			</tr>
																			<tr class="search_item_bg" valign="top">
																				<td>
																					<div style="width:90%">
																						<canvas id="Chart"align="middle"></canvas>
																					</div>
																					<div style="float: left">
																						<asp:RadioButtonList id="rblCheckNumber" runat="server" Width="160px" RepeatDirection="Horizontal" AutoPostBack="True" CssClass="radio_button_list" OnSelectedIndexChanged="rblCheckNumber_OnSelectedIndexChanged">
																							<asp:ListItem Value="0" >数値あり</asp:ListItem>
																							<asp:ListItem Value="1" Selected="true">数値なし</asp:ListItem>
																						</asp:RadioButtonList>
																					</div>
																				</td>
																			</tr>
																		</table>
																	</td>
																</tr>
																<tr>
																	<td>
																		<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/>
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
								<td>
									<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/>
								</td>
							</tr>
						</table>
					</td>
					<td style="padding-left: 30px;" valign="top">
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td>
												<table class="list_table" border="0" cellspacing="0" cellpadding="0">
													<asp:Repeater ID="rMonthly" ItemType="w2.Domain.FixedPurchaseForecast.Helper.FixedPurchaseForecastItemSearchResult" Runat="server" Visible="<%# (IsSerachNoHit == false) %>">
														<HeaderTemplate>
															<tr class="list_title_bg">
																<td class="tableHeader" align="center" width="20%">年月</td>
																<td align="center" width="40%">販売予測金額</td>
																<td align="center" width="40%">販売予測数量</td>
															</tr>
															<tr valign="top">
																<td class="list_item_bg5_dark" colspan="2" style="padding:0"></td>
															</tr>
														</HeaderTemplate>
														<ItemTemplate>
															<tr class="list_item_bg<%#: Container.ItemIndex % 2 + 1 %>">
																<td align="center" class="tableTitle" height="35px"><%#: DateTimeUtility.ToStringForManager(Item.TargetMonth, DateTimeUtility.FormatType.LongYearMonth) %></td>
																<td align="right"><%#: Item.Price.ToPriceString(true) %></td>
																<td align="right"><%#: Item.Stock %>個</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
												</table>
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
	</table>
	<!-- 備考 -->
	<table class="info_table" width="700" border="0" cellspacing="1" cellpadding="3" style="margin-top: 30px;">
		<tr class="info_item_bg">
			<td align="left">
				備考
				<p>定期売上予測レポートは、定期購入ステータスが「通常」の定期台帳を対象とし、配送周期を基に予測しています。</p>
				<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED) { %>
					<p>集計には頒布会定期台帳も含まれています。（※定額機能を利用しているものは対象外）</p>
				<% } %>
			</td>
		</tr>
	</table>
	<script>CreateFixedPurchaseForecasCharts(<%# this.TargetValue %>, '<%# this.ChartType %>', '<%# this.IsViewNumber.ToString() %>', '年月', '金額', '商品数')</script>
</asp:Content>
