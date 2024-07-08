<%--
=========================================================================================================
  Module      : 商品ランキングレポートページ(ProductRankingReport.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="ProductRankingReport.aspx.cs" Inherits=" Form_ProductRanking_ProductRankingReport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
	<script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.7.2/Chart.bundle.js"></script>
	<script src="../../Js/Manager.chart.js"></script>
	<table width="541" border="0">
		<tr>
			<td>
				<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/>
			</td>
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
																<asp:LinkButton ID="lbMonthBefore" Runat="server" onclick="lbMonthBefore_Click">
																	<asp:Image Height="17" Width="18" src="../../Images/Common/paging_back_01.gif" border="0" hspace="10" onmouseover='JavaScript:this.src = "../../Images/Common/paging_back_02.gif"' onmouseout='JavaScript:this.src = "../../Images/Common/paging_back_01.gif"' runat="server"/>
																</asp:LinkButton>
																<asp:Label ID="lbDate" runat="server"></asp:Label>
																<asp:LinkButton ID="lbMonthNext" Runat="server" onclick="lbMonthNext_Click">
																	<asp:Image  Height="17" Width="18" src="../../Images/Common/paging_next_01.gif" border="0" hspace="10" onmouseover='JavaScript:this.src = "../../Images/Common/paging_next_02.gif"' onmouseout='JavaScript:this.src = "../../Images/Common/paging_next_01.gif"' runat="server" />
																</asp:LinkButton>
																<asp:Label ID="lbRankingInfo" runat="server"></asp:Label>
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
																			<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align: middle" border="0"/>
																			グラフレポート
																		</td>
																		<td style="text-align: right">
																			<asp:RadioButtonList id="rblChartType" Runat="server" RepeatDirection="Horizontal" AutoPostBack="True" CssClass="radio_button_list">
																				<asp:ListItem Value="0" Selected="True">棒グラフ</asp:ListItem>
																				<asp:ListItem Value="1">折れ線グラフ</asp:ListItem>
																			</asp:RadioButtonList>
																		</td>
																	</tr>
																</table>
															</td>
														</tr>
														<tr class="search_item_bg" valign="top">
															<td>
																<canvas id="Chart" height="200" width="504" align="middle"></canvas>
																<asp:Literal runat="server" ID="lScriptFunction"></asp:Literal>
																<div style="float: left">
																	<asp:RadioButtonList id="rblCheckNumber" runat="server" Width="160px" RepeatDirection="Horizontal" AutoPostBack="True" CssClass="radio_button_list">
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
</asp:Content>