<%--
=========================================================================================================
  Module      : 定期売上予測推移グラフ(FixedPurchaseForecastReportTransitiveGraph.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#"  MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="FixedPurchaseForecastReportTransitiveGraph.aspx.cs" Inherits="Form_FixedPurchaseForecastReport_FixedPurchaseForecastReportTransitiveGraph" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
	<script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.7.2/Chart.bundle.js"></script>
	<script src="../../Js/fixedPurchaseForecastChart.js"></script>
	<table width="541" border="0">
		<tr>
			<td>
				<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/>
			</td>
			<tr>
				<td><h2 class="page-title">定期売上予測推移グラフ</h2></td>
			</tr>
		</tr>
		<tr>
			<td>
				<table class="box_border" cellspacing="1" cellpadding="0" width="541" border="0">
					<tr>
						<td class="search_box_bg">
							<table cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td align="center">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td>
													<img height="8" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/>
												</td>
											</tr>
											<tr>
												<td>
													<table class="info_table" width="508" border="0" cellspacing="1" cellpadding="3">
														<tr class="info_item_bg">
															<td align="left" style="position:relative ">
																商品名(商品ID)：<asp:Literal ID="lProductName" runat="server"></asp:Literal>(<asp:Literal ID="lProductId" runat="server"></asp:Literal>)
															</td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<td>
													<img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/>
												</td>
											</tr>
											<tr>
												<td class="search_box">
													<table class="search_table" cellspacing="1" cellpadding="2" width="508" border="0">
														<tr class="search_title_bg">
															<td>
																<table class="trans_table" width="100%" border="0">
																	<tr>
																		<td style="vertical-align: middle">
																			&nbsp;
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
															<td>
																<canvas id="Chart" height="200" width="504" align="middle"></canvas>
																<div style="float: left">
																	<asp:RadioButtonList id="rblCheckNumber" runat="server" Width="160px" RepeatDirection="Horizontal" AutoPostBack="True" CssClass="radio_button_list">
																		<asp:ListItem Value="0" >数値あり</asp:ListItem>
																		<asp:ListItem Value="" Selected="true">数値なし</asp:ListItem>
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
	<script>
		CreateFixedPurchaseForecasCharts(
			<%# this.TargetValue %>,
			'<%# this.ChartType %>',
			'<%# this.IsViewNumber.ToString() %>',
			'年月', '金額', '商品数')
	</script>
</asp:Content>
