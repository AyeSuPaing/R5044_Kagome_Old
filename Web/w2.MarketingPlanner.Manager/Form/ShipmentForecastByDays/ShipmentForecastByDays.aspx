<%--
=========================================================================================================
  Module      : 日別出荷予測レポート一覧ページ(ShipmentForecastByDays.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ShipmentForecastByDays.aspx.cs" Inherits="Form_ShipmentForecastByDays_ShipmentForecastByDays" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.7.2/Chart.bundle.js"></script>
<script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
<script type="text/javascript" charset="UTF-8" src="<%= Constants.PATH_ROOT %>Js/ShipmentForecastByDaysChart.js?<%: Constants.QUERY_STRING_FOR_UPDATE_EXTERNAL_FILE_URLENCODED %>"></script>

<table width="791" border="0">
<tr>
	<td>
		<h1 class="page-title" style="display: inline; white-space: nowrap;">日別出荷予測レポート</h1>
		<span style="font-weight: bold; margin: 0 20px;"><%: DateTimeUtility.ToStringForManager(this.LastChangedDateTime, DateTimeUtility.FormatType.ShortDateHourMinuteNoneServerTime) %>時点</span>
		<asp:LinkButton ID="lbUpdateData" CssClass="cmn-btn-sub-action" style="margin: 0 20px;" runat="server" OnClick="lbUpdateData_Click">データ更新</asp:LinkButton>
	</td>
</tr>
<tr>
	<td>
		<img height="10" alt="" src="../../Images/Common/sp.gif" width="1" border="0"/>
	</td>
</tr> <!--▽ 検索 ▽-->
<tr>
	<td style="border: none;">
		<table class="box_border" cellpadding="0" width="784" border="0">
			<tr>
				<td class="search_box_bg">
					<table cellpadding="0" width="100%" border="0">
						<tr>
							<td align="center">
								<table cellspacing="0" cellpadding="0" border="0">
									<tr>
										<td>
											<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/>
										</td>
									</tr>
									<tr>
										<td class="search_box">
											<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
												<tr>
													<td width="30%" style="min-width:250px; border: none;">
														<h2 class="cmn-hed-h2">対象年月</h2>
													</td>
													<td width="70%" class="search_title_bg">
														<table class="trans_table" width="100%" border="0">
															<tr>
																<td style="vertical-align: middle">
																	&nbsp;
																	<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align: middle" border="0"/>
																	グラフレポート
																</td>
																<td style="text-align: right">
																	<asp:RadioButtonList id="rblChartType" Runat="server" RepeatDirection="Horizontal" AutoPostBack="True" CssClass="radio_button_list">
																		<asp:ListItem Value="bar" Selected="True">棒グラフ</asp:ListItem>
																		<asp:ListItem Value="line">折れ線グラフ</asp:ListItem>
																	</asp:RadioButtonList>
																</td>
															</tr>
														</table>
													</td>
												</tr>
												<tr class="search_item_bg" valign="top">
													<td  style="border: none;">
														<br/>
														<!-- ▽カレンダ▽ -->
														<asp:label id="lblCurrentCalendar" Runat="server"></asp:label>
														<!-- △カレンダ△ -->
														<br/>
														<table width="95%" align="center">
															<td width="30%" style="border: none;">
																&nbsp;
																<h2 class="cmn-hed-h2">
																	<%#: this.CurrentYear %>年
																	<%#: this.CurrentMonth %>月
																	&nbsp;
																	売上商品金額・出荷件数
																</h2>
																<br/>
																<table width="100%" align="center">
																	<tr class="search_title_bg">
																		<td width="50%" style="text-align: center;">
																			<span style="font-weight: bold;">月間売上商品金額</span>
																		</td>
																		<td width="50%" style="text-align: center;">
																			<span style="font-weight: bold;">月間出荷件数</span>
																		</td>
																	</tr>
																	<tr>
																		<td width="50%" style="text-align: right;">
																			<%#: this.TotalCurrentSalesPrice.ToPriceString(withSymbol:true) %>
																		</td>
																		<td width="50%" style="text-align: right;">
																			<%#: this.TotalCurrentShipped %>件
																		</td>
																	</tr>
																	<tr>
																		<td width="50%" style="text-align: right;">
																			前月比&nbsp;&nbsp;&nbsp;&nbsp;
																			<%#: this.MoMSalesGrowthRate %>%
																		</td>
																		<td width="50%" style="text-align: right;">
																			前月比&nbsp;&nbsp;&nbsp;&nbsp;
																			<%#: this.MoMShipmentRatio %>%
																		</td>
																	</tr>
																</table>
															</td>
														</table>
													</td>
													<td>
														<div style="width:90%">
															<canvas id="Chart"align="middle"></canvas>
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
<!--△ 検索 △-->
<tr>
	<td>
		<img height="10" alt="" src="../../Images/Common/sp.gif" width="1" border="0"/>
	</td>
</tr> 
<!--▽ 一覧 ▽-->
<table class="list_table" cellspacing="1" cellpadding="2" width="<%= (DateTimeUtility.GetLastDayOfMonth(this.CurrentYear, this.CurrentMonth)) * 30 %>" border="0">
	<tr class="list_title_bg">
		<td colspan="<%= DateTimeUtility.GetLastDayOfMonth(this.CurrentYear, this.CurrentMonth) %>">
			<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align: middle" border="0"/>
			<%: DateTimeUtility.ToStringForManager(new DateTime(this.CurrentYear, this.CurrentMonth, 1), DateTimeUtility.FormatType.LongYearMonth) %>
			&nbsp;
			日別出荷件数
		</td>
	</tr>
	<tr class="list_item_bg1" valign="top">
		<asp:Repeater ID="rTableHeader" runat="server">
			<ItemTemplate>
				<td width="30" class="<%# CreateTableDayClassName(this.CurrentYear + "/" + this.CurrentMonth + "/" + (Container.ItemIndex + 1)) %>">
					<div align="center">
						<asp:Literal ID="lHeader" runat="server"
							Text="<%# DateTimeUtility.ToStringForManager(new DateTime(this.CurrentYear, this.CurrentMonth, (Container.ItemIndex + 1)), DateTimeUtility.FormatType.ShortMonthDay) %>"/>
					</div>
				</td>
			</ItemTemplate>
		</asp:Repeater>
	</tr>
	<tr class="list_item_bg1" valign="top">
		<asp:Repeater ID="rTableData" runat="server">
			<ItemTemplate>
				<td width="30">
					<%-- ここの値を取得してくる --%>
					<div align="center">
						<asp:Literal ID="lData" runat="server" 
							Text="<%# ((Hashtable)Container.DataItem)[Constants.FIELD_SHIPMENTQUANTITY_SHIPMENT_ORDER_COUNT] %>"/>
					</div>
				</td>
			</ItemTemplate>
		</asp:Repeater>
	</tr>
</table>
<!--△ 一覧 △-->
<tr>
	<td>
		<img height="10" alt="" src="../../Images/Common/sp.gif" width="1" border="0"/>
	</td>
</tr>
<!-- 備考 -->
<table class="info_table" width="700" border="0" cellspacing="1" cellpadding="3" style="margin-top: 30px;">
	<tr class="info_item_bg">
		<td align="left">
			<h2>備考</h2>
			<p>■売上商品金額について</p>
			<p>・売上商品金額においては、送料・クーポン・ポイント等の割引は含んでおりません。</p>
			<p>■頒布会について</p>
			<p>・回数・期間指定なしの頒布会に関しては無限繰り返しの定期と同じ扱いで集計する。</p>
			<p>・頒布会は、1回目の商品を繰り返して計測しております。</p>
		</td>
	</tr>
</table>
</table>
<script>
	CreateShipmentForecastByDaysCharts(
		<%# this.TargetValue %>,
		'<%# this.ChartType %>',
		'<%# false.ToString() %>',
		'日付', '売上商品金額', '出荷件数')
</script>
<script>
	function checkBatchProcessStatus() {
		$.ajax({
			type: "POST",
			url: "ShipmentForecastByDays.aspx/GetBatchStatus",
			contentType: "application/json; charset=utf-8",
			dataType: "json",
			success: function (response) {
				if (response.d == 0) {
					alert("更新が完了しました。");
					$.ajax({
						type: "POST",
						url: "ShipmentForecastByDays.aspx/ResetBatchStatus",
						contentType: "application/json; charset=utf-8",
						dataType: "json",
					});
					window.location.href = "<%# Constants.PATH_ROOT + "/Form/ShipmentForecastByDays/ShipmentForecastByDays.aspx" %>";
				}
				else if (response.d == 1) {
					alert("実行中です。");
					$.ajax({
						type: "POST",
						url: "ShipmentForecastByDays.aspx/ResetBatchStatus",
						contentType: "application/json; charset=utf-8",
						dataType: "json",
					});
				}
			},
			error: function () {
				console.log("バッチ処理状態の確認中にエラーが発生しました。");
			}
		});
	}

	$(document).ready(function () {
		setInterval(checkBatchProcessStatus, 1000); 
	});
</script>
</asp:Content>
