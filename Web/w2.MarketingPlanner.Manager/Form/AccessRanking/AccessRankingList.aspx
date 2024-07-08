<%--
=========================================================================================================
  Module      : アクセスランキングレポート一覧ページ(AccessRankingList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="AccessRankingList.aspx.cs" Inherits="Form_AccessRanking_AccessRankingList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">アクセスランキング</h1></td>
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
															<asp:LinkButton ID="lbMonthBefore" Runat="server" onclick="lbMonthBefore_Click"><img src='../../Images/Common/paging_back_01.gif' alt="" border="0" hspace="10"
																	onmouseover='JavaScript:this.src="../../Images/Common/paging_back_02.gif"'
																	onmouseout='JavaScript:this.src="../../Images/Common/paging_back_01.gif"' /></asp:LinkButton></td>
														<td><asp:LinkButton ID="lbTgtMonth" Runat="server" CssClass="calendar_unselected_bg" onclick="lbTgtMonth_Click">xxxx年x月</asp:LinkButton></td>
														<td>
															<asp:LinkButton ID="lbMonthNext" Runat="server" onclick="lbMonthNext_Click"><img src='../../Images/Common/paging_next_01.gif' alt="" border="0" hspace="10"
																	onmouseover='JavaScript:this.src="../../Images/Common/paging_next_02.gif"'
																	onmouseout='JavaScript:this.src="../../Images/Common/paging_next_01.gif"' /></asp:LinkButton>
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
											<td align="left" valign="top">
												<asp:RadioButtonList ID="rblPCMobile" RepeatDirection="Horizontal" AutoPostBack="true" runat="server" OnSelectedIndexChanged="rblPCMobile_SelectedIndexChanged">
													<asp:ListItem Value="pc">ＰＣ</asp:ListItem>
													<asp:ListItem Value="smart_phone">スマフォ</asp:ListItem>
													<asp:ListItem Value="mobile">モバイル</asp:ListItem>
												</asp:RadioButtonList>
												<table border="1">
													<tr>
														<td>
															<asp:RadioButtonList id="rblRankingType" Runat="server" AutoPostBack="True" CssClass="radio_button_list" RepeatDirection="Vertical" onselectedindexchanged="rblRankingType_SelectedIndexChanged"></asp:RadioButtonList>
														</td>
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
														<td align="left"><%= WebSanitizer.HtmlEncode(GetAccessRankingInfo()) %></td>
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
														<td align="center" width="10%">ランク</td>
														<td align="center" width="60%"><asp:Literal ID="lValueName" runat="server" /></td>
														<td align="center" width="20%"><asp:Literal ID="lUnit" runat="server" /></td>
														<td align="center" width="10%">構成比</td>
													</tr>
													<tr class="list_item_bg1" id="trNoData" runat="server">
														<td align="center" width="30%" colspan="4">データが存在しません</td>
													</tr>
													<asp:Repeater id="rRankingList" Runat="server">
														<ItemTemplate>
															<tr>
																<td class="list_item_bg2" align="center"><asp:Literal ID="lRank" runat="server" Text="<%# WebSanitizer.HtmlEncode(((RankingUtility.RankingRow)Container.DataItem).Rank) %>" /></td>
																<td class="list_item_bg2" align="left"><asp:Literal ID="lRowName" runat="server" Text="<%# WebSanitizer.HtmlEncode(((RankingUtility.RankingRow)Container.DataItem).RowName)%>" /></td>
																<td class="list_item_bg1" align="right"><asp:Literal ID="lCount" runat="server" Text="<%# WebSanitizer.HtmlEncode(((RankingUtility.RankingRow)Container.DataItem).Count)%>" /></td>
																<td class="list_item_bg1" align="right"><asp:Literal ID="lTotalCount" runat="server" Text="<%# WebSanitizer.HtmlEncode(KbnAnalysisUtility.GetRateString(((RankingUtility.RankingRow)Container.DataItem).Count, ((RankingUtility.RankingRow)Container.DataItem).TotalCount, 1))%>" /></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
												</table>
											</td>
										</tr>
										<!--△ 分析結果 △-->
										<tr>
											<td><img height="20" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
