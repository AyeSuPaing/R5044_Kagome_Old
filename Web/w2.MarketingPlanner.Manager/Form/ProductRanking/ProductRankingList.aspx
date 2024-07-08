<%--
=========================================================================================================
  Module      : 商品ランキングレポート一覧ページ(ProductRankingList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ProductRankingList.aspx.cs" Inherits="Form_ProductRanking_ProductRankingList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<script type="text/javascript">
<!--
	// 別ウィンドウ表示
	function open_window(cur_year, cur_month, rpt, rpv, rpak, dpk, window_name, window_type)
	{
		var link_file = '<%= Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_PRODUCT_RANKING_REPORT %>';
		link_file += '?<%= Constants.REQUEST_KEY_CURRENT_YEAR %>=' + encodeURIComponent(cur_year);
		link_file += '&<%= Constants.REQUEST_KEY_CURRENT_MONTH %>=' + encodeURIComponent(cur_month);
		link_file += '&<%= Constants.REQUEST_KEY_RANKING_PRODUCT_TYPE %>=' + encodeURIComponent(rpt);
		link_file += '&<%= Constants.REQUEST_KEY_RANKING_PRODUCT_VALUE %>=' + encodeURIComponent(rpv);
		link_file += '&<%= Constants.REQUEST_KEY_RANKING_PRODUCT_ACCESS_KBN %>=' + encodeURIComponent(rpak);
		link_file += '&<%= Constants.REQUEST_KEY_TAX_DISPLAY_TYPE %>=' + encodeURIComponent(dpk);

		var new_win = window.open(link_file, window_name, window_type);
		new_win.focus();
	}
//-->
</script>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">商品ランキング</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 検索 ▽-->
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td class="search_box_bg" align="center">
						<table cellspacing="0" cellpadding="0" border="0">
							<tr>
								<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
							</tr>
							<tr>
								<td class="search_box">
									<table class="list_table" cellspacing="1" cellpadding="2" width="758" border="0">
										<tr class="search_item_bg">
											<td style="TEXT-ALIGN: center; padding:4px" width="30%" height="130" valign="top">
												<table cellpadding="0" cellspacing="0" border="0" class="list_Calendar">
													<tr>
														<td>
															<asp:LinkButton ID="lbMonthBefore" Runat="server" onclick="lbMonthBefore_Click"><img src='../../Images/Common/paging_back_01.gif' border="0" hspace="10"
																	onmouseover='JavaScript:this.src="../../Images/Common/paging_back_02.gif"'
																	onmouseout='JavaScript:this.src="../../Images/Common/paging_back_01.gif"'></asp:LinkButton></td>
														<td><asp:LinkButton ID="lbTgtMonth" Runat="server" CssClass="calendar_unselected_bg" onclick="lbTgtMonth_Click">xxxx年x月</asp:LinkButton></td>
														<td>
															<asp:LinkButton ID="lbMonthNext" Runat="server" onclick="lbMonthNext_Click"><img src='../../Images/Common/paging_next_01.gif' border="0" hspace="10"
																	onmouseover='JavaScript:this.src="../../Images/Common/paging_next_02.gif"'
																	onmouseout='JavaScript:this.src="../../Images/Common/paging_next_01.gif"'></asp:LinkButton>
														</td>
													</tr>
												</table>
												<!-- ▽カレンダ▽ -->
												<asp:Calendar ID="calTarget" Runat="server" SelectWeekText=" " PrevMonthText=" " NextMonthText=" "
													SelectMonthText=" " ShowGridLines="True" BorderColor="#ece9d8" Width="180px" ShowNextPrevMonth="False" ShowTitle="False" onselectionchanged="calTarget_SelectionChanged" DayStyle-HorizontalAlign="Center"
													OnDayRender="calTarget_DayRender" CssClass="list_Calendar">
													<DayStyle CssClass="calendar_unselected_bg" ForeColor="#0864AA"></DayStyle>
													<SelectedDayStyle ForeColor="#0864AA" BackColor="#BED2FF"></SelectedDayStyle>
													<TitleStyle BackColor="Transparent"></TitleStyle>
													<OtherMonthDayStyle ForeColor="LightGray"></OtherMonthDayStyle>
												</asp:Calendar>
												<!-- △カレンダ△ -->
											</td>
											<td align="left">
												<asp:RadioButtonList id="rblAccessKbn" Runat="server" AutoPostBack="True" CssClass="radio_button_list"
													RepeatDirection="Horizontal" onselectedindexchanged="rblRankingType_SelectedIndexChanged">
													<asp:ListItem Value="0" Selected="True">全体</asp:ListItem>
													<asp:ListItem Value="1">PC</asp:ListItem>
													<asp:ListItem Value="3">スマフォ</asp:ListItem>
													<asp:ListItem Value="2">モバイル</asp:ListItem>
												</asp:RadioButtonList>
												<br />
												<asp:RadioButtonList id="rblRankingType" Runat="server" AutoPostBack="True" CssClass="radio_button_list"
													RepeatDirection="Vertical" onselectedindexchanged="rblRankingType_SelectedIndexChanged">
													<asp:ListItem Value="1" Selected="True">商品検索ワードランキング</asp:ListItem>
													<asp:ListItem Value="2">商品販売個数ランキング</asp:ListItem>
													<asp:ListItem Value="3">商品販売金額ランキング</asp:ListItem>
												</asp:RadioButtonList>
												<div style='margin-left:20px; <%: (rblRankingType.SelectedValue == Constants.KBN_RANKING_PRODUCT_BUY_PRICE) ? "" : "display:none;"%>'>
												<asp:RadioButtonList id="rblTaxType" Runat="server" AutoPostBack="True" CssClass="radio_button_list" 
													RepeatDirection="Horizontal" onselectedindexchanged="rblTaxType_SelectedIndexChanged">
														<asp:ListItem Value="included" Selected="True">税込</asp:ListItem>
														<asp:ListItem Value="exclude">税抜</asp:ListItem>
												</asp:RadioButtonList>
												</div>
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
	<!--△ 検索 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
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
									<table cellspacing="0" cellpadding="0" border="0" width="80%">
										<tr>
											<td><img height="8" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<div class="search_btn_sub_rt" >
													<asp:LinkButton id="lbReportExport" Runat="server" OnClick="lbReportExport_Click">CSVダウンロード</asp:LinkButton></div>
												<table class="info_table" width="700" border="0" cellspacing="1" cellpadding="3">
													<tr class="info_item_bg">
														<td align="left"><asp:Label ID="lbRankingInfo" runat="server"></asp:Label></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<!--▽ ページング ▽-->
												<table class="list_pager" cellspacing="0" cellpadding="0" width="700" border="0">
													<tr>
														<td width="675"><asp:label id="lbPager1" Runat="server"></asp:label></td>
													</tr>
												</table>
												<!-- ページング-->
											</td>
										</tr>
										<!--▽ 分析結果 ▽-->
										<tr>
											<td><img height="12" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td align="center">
												<table class="list_table" cellspacing="1" cellpadding="3" width="700" border="0">
													<tr class="list_title_bg">
														<%-- ランク --%>
														<td align="center" width="10%">ランク</td>
														<%-- 商品ID(検索ワードランキング以外) --%>
														<%if (rblRankingType.SelectedValue != Constants.KBN_RANKING_PRODUCT_SEARCH_WORD) {%>
														<td align="center" width="20%"><asp:Label ID="lbExtraName1" runat="server"></asp:Label></td>
														<%} %>
														<%-- 値名称 --%>
														<td align="center"><asp:Label ID="lbValueName" runat="server"></asp:Label></td>
														<%-- ヒット数(検索ワードランキングのみ) --%>
														<%if (rblRankingType.SelectedValue == Constants.KBN_RANKING_PRODUCT_SEARCH_WORD) {%>
														<td align="center" width="35%"><asp:Label ID="lbExtraName2" runat="server"></asp:Label></td>
														<%} %>
														<%-- 単位 --%>
														<td align="center" width="15%"><asp:Label ID="lbUnitName" runat="server"></asp:Label></td>
														<%-- 構成比 --%>
														<td align="center" width="10%">構成比</td>
														<%-- 月間推移 --%>
														<td align="center" width="10%">月間推移</td>
													</tr>
													<tr id="trNoData" runat="server" class="list_item_bg1" >
														<td align="center" colspan="6">データが存在しません</td>
													</tr>
													<asp:Repeater id="rResult" Runat="server">
														<ItemTemplate>
															<tr>
																<%-- ランク --%>
																<td class="list_item_bg2" align="center"><asp:Literal ID="lRank" runat="server" Text="<%# WebSanitizer.HtmlEncode(((RankingUtility.RankingRow)Container.DataItem).Rank) %>" /></td>
																<%-- 商品ID(検索ワードランキング以外) --%>
																<%if (rblRankingType.SelectedValue != Constants.KBN_RANKING_PRODUCT_SEARCH_WORD) {%>
																<td class="list_item_bg2" align="center"><%# WebSanitizer.HtmlEncode(((RankingUtility.RankingRow)Container.DataItem).Extra)%></td>
																<%} %>
																<%-- 値名称 --%>
																<td class="list_item_bg2" align="left"><asp:Literal ID="lRowName" runat="server" Text="<%# WebSanitizer.HtmlEncode(((RankingUtility.RankingRow)Container.DataItem).RowName)%>" /></td>
																<%-- ヒット数(検索ワードランキングのみ) --%>
																<%if (rblRankingType.SelectedValue == Constants.KBN_RANKING_PRODUCT_SEARCH_WORD) {%>
																<td class="list_item_bg2" align="right"><%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(((RankingUtility.RankingRow)Container.DataItem).Extra))%></td>
																<%} %>
																<%-- 値 --%>
																<td class="list_item_bg1" align="right">
																<%if (rblRankingType.SelectedValue == Constants.KBN_RANKING_PRODUCT_BUY_PRICE) {%>
																	<%# WebSanitizer.HtmlEncode(((RankingUtility.RankingRow)Container.DataItem).Price.ToPriceString(true)) %>
																<%} else {%>
																	<%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(((RankingUtility.RankingRow)Container.DataItem).Count)) %>
																<%} %>
																</td>
																<%-- 構成比 --%>
																<td class="list_item_bg1" align="right"><asp:Literal ID="lRate" runat="server" Text="<%# WebSanitizer.HtmlEncode(KbnAnalysisUtility.GetRateString((rblRankingType.SelectedValue == Constants.KBN_RANKING_PRODUCT_BUY_PRICE) ? ((RankingUtility.RankingRow)Container.DataItem).Price : ((RankingUtility.RankingRow)Container.DataItem).Count, (rblRankingType.SelectedValue == Constants.KBN_RANKING_PRODUCT_BUY_PRICE) ? ((RankingUtility.RankingRow)Container.DataItem).TotalPrice : ((RankingUtility.RankingRow)Container.DataItem).TotalCount, 1))%>" />%</td>
																<%-- 月間推移 --%>
																<%if (rblRankingType.SelectedValue != Constants.KBN_RANKING_PRODUCT_SEARCH_WORD) {%>
																<td class="list_item_bg1" align="center"><a href="<%# WebSanitizer.HtmlEncode("javascript:open_window(" + CreateProductRankingReportUrlParam(rblRankingType.SelectedValue, (string)((RankingUtility.RankingRow)Container.DataItem).Extra) + ",'product_ranking_report','width=828,height=520,top=50,left=300,status=NO,scrollbars=yes');") %>"><img alt="月間推移" src="../../Images/Common/icon_graph.gif" border="0" /></a></td>
																<%} else {%>
																<td class="list_item_bg1" align="center"><a href="<%# WebSanitizer.HtmlEncode("javascript:open_window(" + CreateProductRankingReportUrlParam(rblRankingType.SelectedValue, (string)((RankingUtility.RankingRow)Container.DataItem).RowName) + ",'product_ranking_report','width=828,height=520,top=50,left=300,status=NO,scrollbars=yes');") %>"><img alt="月間推移" src="../../Images/Common/icon_graph.gif" border="0" /></a></td>
																<%} %>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
												</table>
											</td>
										</tr>
										<!--△ 分析結果 △-->
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<%if (rblRankingType.SelectedValue != Constants.KBN_RANKING_PRODUCT_SEARCH_WORD) {%>
										<tr>
											<td>
												<table class="info_table" width="700" border="0" cellspacing="1" cellpadding="3">
													<tr class="info_item_bg">
														<td align="left">
															 備考<br />
															・商品ID欄には商品ID+バリエーションIDが表示されます。<br />								
															・ランキングは日毎に集計されるため、日をまたいだ受注商品の変更・返品・キャンセル分の商品は考慮されません。<br />
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<%} %>
										<tr>
											<td><img height="15" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
</asp:Content>
