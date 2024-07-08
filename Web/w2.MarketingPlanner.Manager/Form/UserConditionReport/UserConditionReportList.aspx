<%--
=========================================================================================================
  Module      : 顧客状況詳細レポート一覧ページ(UserConditionReportList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="UserConditionReportList.aspx.cs" Inherits="Form_UserConditionReport_UserConditionReportList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<script type="text/javascript">
<!--
// 休眠ユーザ詳細表示/非表示
function disp_unactive_detail(potrec)
{
	// 設定値決定
	var disp = "";
	var next = "";
	if (document.getElementById(potrec + "_button").value == "＋")
	{
		disp = "";
		next = "－";
	}
	else
	{
		disp = "none";
		next = "＋";
	}
	
	// 各種セル表示/非表示
	for (var iLoop = 1; iLoop <= 8; iLoop++)
	{
		document.getElementById(potrec + "_tr" + iLoop).style.display = disp;
	}
	
	// ボタン文字変更		
	document.getElementById(potrec + "_button").value = next;
}
//-->
</script>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">ユーザー状況レポート</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr> <!--▽ 検索 ▽-->
	<tr>
		<td>
			<div class="tabs-wrapper">
				<a href="#" class="current">レポート対象選択</a>
				<a href="<%# WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_USER_CONDITION_REPORT_DETAIL + "?" + Constants.REQUEST_KEY_CURRENT_YEAR + "=" + m_iCurrentYear + "&amp;" + Constants.REQUEST_KEY_CURRENT_MONTH + "=" + m_iCurrentMonth) %>">
					詳細レポート対象選択
				</a>
			</div>
		</td>
	</tr>
	<tr>
		<td class="tab-contents">
			<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td class="search_box_bg">
						<table cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td class="search_box">
												<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
													<tr class="search_title_bg">
														<td width="50%">&nbsp;
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />対象月 &nbsp;</td>
														<td width="50%">&nbsp; 
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />比較月</td>
													</tr>
													<tr class="search_item_bg">
														<td>
															<!-- ▽カレンダ（基準）▽ --><asp:label id="lblCurrentCalendar" Runat="server"></asp:label>
															<!-- △カレンダ（基準）△ -->
														<td>
															<!-- ▽カレンダ（対象）▽ --><asp:label id="lblTargetCalendar" Runat="server"></asp:label>
															<!-- △カレンダ（対象）△ -->
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
												<div class="search_btn_sub_rt" >
													<asp:LinkButton id="lbReportExport" Runat="server" OnClick="lbReportExport_Click">CSVダウンロード</asp:LinkButton></div>
												<table class="info_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr class="info_item_bg">
														<td align="left">対象月：<%#: DateTimeUtility.ToStringForManager(new DateTime(m_iCurrentYear, m_iCurrentMonth, 1), DateTimeUtility.FormatType.LongYearMonth) %>
															比較月：<%#: DateTimeUtility.ToStringForManager(new DateTime(m_iTargetYear, m_iTargetMonth, 1), DateTimeUtility.FormatType.LongYearMonth) %></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td align="left">
												<table width="700" cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td width="38%"><img height=12 alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
														<td width="10%"></td>
														<td width="32%">
															<!-- 対象月 -->
															<table class="list_table" cellspacing="1" cellpadding="0" width="100%" border="0">
																<tr class="list_item_bg1">
																	<td align="center">
																		<%#: DateTimeUtility.ToStringForManager(new DateTime(m_iCurrentYear, m_iCurrentMonth, 1), DateTimeUtility.FormatType.LongYearMonth) %>
																		<span class="notice">
																			<%# WebSanitizer.HtmlEncode((((m_drvCurrentData != null)?m_drvCurrentData["final_flg"].ToString():"") == "0")?"*":"") %></span>
																	</td>
																</tr>
															</table>
														</td>
														<td width="1%"></td>
														<td width="19%">
															<!-- 比較月 -->
															<table class="list_table" cellspacing="1" cellpadding="0" width="100%" border="0">
																<tr class="list_item_bg1_dark">
																	<td align="center">
																		<%#: DateTimeUtility.ToStringForManager(new DateTime(m_iTargetYear, m_iTargetMonth, 1), DateTimeUtility.FormatType.LongYearMonth) %>
																		<span class="notice"><%# WebSanitizer.HtmlEncode((((m_drvTargetData != null)?m_drvTargetData["final_flg"].ToString():"") == "0")?"*":"") %></span>
																	</td>
																</tr>
															</table>
														</td>
													</tr>
													<tr>
														<td colspan="5" height="5"><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
												</table>
												<table width="694" cellspacing="0" cellpadding="0" border="0" >
													<tr>
														<!-- 潜在・新規獲得・タイトル -->
														<td width="15%"><img height="12" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
														<td width="32%" class="userana_cell_tblr" valign="middle">
															潜在ユーザ：新規獲得数
														</td>
														<td width="2%"></td>
														<!-- 潜在・新規獲得・対象月 -->
														<td width="18%" class="userana_cell_tblr" align="center">
															<b><%#  WebSanitizer.HtmlEncode(DispNumeric(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_NEW + "_total", "人"))%></b>
														</td>
														<td width="1%"></td>
														<!-- 潜在・新規獲得・比率 -->
														<td width="12%"class="userana_cell_tblr" align="right">
															<%# WebSanitizer.HtmlEncode(DispIncreasingRate(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_NEW + "_total"), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_NEW + "_total")))%>
														</td>
														<td width="2%"></td>
														<!-- 潜在・新規獲得・比較月 -->
														<td width="18%" class="userana_cell_tblr_dark" align="center">
															<b><%# WebSanitizer.HtmlEncode(DispNumeric(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_NEW + "_total", "人"))%></b>
														</td>
													</tr>
													<tr>
														<td colspan="2"></td>
														<td colspan="3" align="center">↓</td>
														<td></td>
														<td></td>
														<td colspan="2" align="center">↓</td>
													</tr>
													<tr>
														<td class="userana_cell_tbl list_item_bg2" align="center">
															潜在ユーザ
														</td>
														<td>
															<!-- 潜在・タイトル -->
															<table cellspacing="0" cellpadding="0" width="100%" border="0">
																<tr class="list_item_bg2">
																	<td class="userana_cell_tbr">　</td>
																</tr>
																<tr class="list_item_bg1">
																	<td class="userana_cell_blr">アクティブユーザ</td>
																</tr>
																<tr class="list_item_bg1">
																	<td class="userana_cell_blr"><input id="pot_button" type="button" onclick="disp_unactive_detail('pot');" value="＋" style="FONT-SIZE: 6pt">&nbsp;休眠ユーザ</td>
																</tr>
																<tr id="pot_tr1" class="list_item_bg1" style="display:none">
																	<td class="userana_cell_blr">　　　休眠レベル１</td>
																</tr>
																<tr id="pot_tr2" class="list_item_bg1" style="display:none">
																	<td class="userana_cell_blr">　　　休眠レベル２</td>
																</tr>
															</table>
														</td>
														<td></td>
														<td>
															<!-- 潜在・対象月 -->
															<table cellspacing="0" cellpadding="0" width="100%" border="0">
																<tr class="list_item_bg2">
																	<td class="userana_cell_tblr" align="center" colspan="2">
																		<%# WebSanitizer.HtmlEncode(DispNumeric(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ALL, "人"))%>
																	</td>
																</tr>
																<tr class="list_item_bg1">
																	<td class="userana_cell_blr" align="right" width="62%">
																		<%# WebSanitizer.HtmlEncode(DispNumeric(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ACTIVE, "人"))%>
																	</td>
																	<td class="userana_cell_br small_text" align="right">
																		<%# WebSanitizer.HtmlEncode(DispRate(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ACTIVE), DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ALL))) %>
																	</td>
																</tr>
																<tr class="list_item_bg1">
																	<td class="userana_cell_blr" align="right">
																		<%# WebSanitizer.HtmlEncode(DispNumeric(m_drvCurrentData, "potential_unactive_total", "人"))%>
																	</td>
																	<td class="userana_cell_br small_text" align="right">
																		<%# WebSanitizer.HtmlEncode(DispRate(DispData(m_drvCurrentData, "potential_unactive_total"), DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ALL))) %>
																	</td>
																</tr>
																<tr id="pot_tr3"  class="list_item_bg1" align="right" style="display:none">
																	<td class="userana_cell_blr">
																		<%# WebSanitizer.HtmlEncode(DispNumeric(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_UNACTIVE1, "人"))%>
																	</td>
																	<td class="userana_cell_br small_text" align="right">
																		<%# WebSanitizer.HtmlEncode(DispRate(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_UNACTIVE1), DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ALL))) %>
																	</td>
																</tr>
																<tr id="pot_tr4" class="list_item_bg1" align="right" style="display:none">
																	<td class="userana_cell_blr">
																		<%# WebSanitizer.HtmlEncode(DispNumeric(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_UNACTIVE2, "人"))%>
																	</td>
																	<td class="userana_cell_br small_text" align="right">
																		<%# WebSanitizer.HtmlEncode(DispRate(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_UNACTIVE2), DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ALL))) %>
																	</td>
																</tr>
															</table>
														</td>
														<td></td>
														<td>
															<!-- 潜在・比率 -->
															<table cellspacing="0" cellpadding="0" width="100%" border="0">
																<tr class="list_item_bg2">
																	<td class="userana_cell_tblr" align="right">
																		<%# WebSanitizer.HtmlEncode(DispIncreasingRate(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ALL), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ALL))) %>
																	</td>
																</tr>
																<tr class="list_item_bg1">
																	<td class="userana_cell_blr" align="right">
																		<%# WebSanitizer.HtmlEncode(DispIncreasingRate(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ACTIVE), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ACTIVE))) %>
																	</td>
																</tr>
																<tr class="list_item_bg1">
																	<td class="userana_cell_blr" align="right">
																		<%# WebSanitizer.HtmlEncode(DispIncreasingRate(DispData(m_drvCurrentData, "potential_unactive_total"), DispData(m_drvTargetData, "potential_unactive_total"))) %>
																	</td>
																</tr>
																<tr id="pot_tr5" class="list_item_bg1" style="display:none">
																	<td class="userana_cell_blr" align="right">
																		<%# WebSanitizer.HtmlEncode(DispIncreasingRate(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_UNACTIVE1), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_UNACTIVE1))) %>
																	</td>
																</tr>
																<tr id="pot_tr6" class="list_item_bg1" style="display:none">
																	<td class="userana_cell_blr" align="right">
																		<%# WebSanitizer.HtmlEncode(DispIncreasingRate(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_UNACTIVE2), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_UNACTIVE2))) %>
																	</td>
																</tr>
															</table>
														</td>
														<td></td>
														<td>
															<!-- 潜在・比較月 -->
															<table cellspacing="0" cellpadding="0" width="100%" border="0">
																<tr class="list_item_bg2_dark">
																	<td class="userana_cell_tblr" align="center" colspan="2">
																		<%# WebSanitizer.HtmlEncode(DispNumeric(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ALL, "人"))%>
																	</td>
																</tr>
																<tr class="list_item_bg1_dark">
																	<td class="userana_cell_blr" align="right" width="62%">
																		<%# WebSanitizer.HtmlEncode(DispNumeric(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ACTIVE, "人"))%>
																	</td>
																	<td class="userana_cell_br small_text" align="right">
																		<%# WebSanitizer.HtmlEncode(DispRate(DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ACTIVE), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ALL))) %>
																	</td>
																</tr>
																<tr class="list_item_bg1_dark">
																	<td class="userana_cell_blr" align="right">
																		<%# WebSanitizer.HtmlEncode(DispNumeric(m_drvTargetData, "potential_unactive_total", "人"))%>
																	</td>
																	<td class="userana_cell_br small_text" align="right">
																		<%# WebSanitizer.HtmlEncode(DispRate(DispData(m_drvTargetData, "potential_unactive_total"), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ALL))) %>
																	</td>
																</tr>
																<tr id="pot_tr7" class="list_item_bg1_dark" align="right" style="display:none">
																	<td class="userana_cell_blr">
																		<%# WebSanitizer.HtmlEncode(DispNumeric(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_UNACTIVE1, "人"))%>
																	</td>
																	<td class="userana_cell_br small_text" align="right">
																		<%# WebSanitizer.HtmlEncode(DispRate(DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_UNACTIVE1), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ALL))) %>
																	</td>
																</tr>
																<tr id="pot_tr8" class="list_item_bg1_dark" align="right" style="display:none">
																	<td class="userana_cell_blr">
																		<%# WebSanitizer.HtmlEncode(DispNumeric(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_UNACTIVE2, "人"))%>
																	</td>
																	<td class="userana_cell_br small_text" align="right">
																		<%# WebSanitizer.HtmlEncode(DispRate(DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_UNACTIVE2), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ALL)))%>
																	</td>
																</tr>
															</table>
														</td>
													</tr>
													<tr>
														<td colspan="2"></td>
														<td colspan="3" align="center">↓</td>
														<td></td>
														<td></td>
														<td colspan="2" align="center">↓</td>
													</tr>
												</table>
												
												<table width="694" cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td width="15%"><img height="12" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
														<!-- 認知・新規獲得・タイトル -->
														<td width="32%" class="userana_cell_tblr">
															認知顧客：新規獲得数
														</td>
														<td width="2%"></td>
														<!-- 認知・新規獲得・対象月 -->
														<td width="18%" class="userana_cell_tblr" align="center">
															<b><%# WebSanitizer.HtmlEncode(DispNumeric(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_NEW + "_total", "人"))%></b>
														</td>
														<td width="1%"></td>
														<!-- 認知・新規獲得・比率 -->
														<td width="12%" class="userana_cell_tblr" align="right">
															<%# WebSanitizer.HtmlEncode(DispIncreasingRate(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_NEW + "_total"), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_NEW + "_total"))) %>
														</td>
														<td width="17">
														</td>
														<!-- 認知・新規獲得・比較月 -->
														<td width="120" class="userana_cell_tblr_dark" align="center">
															<b><%# WebSanitizer.HtmlEncode(DispNumeric(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_NEW + "_total", "人"))%></b>
														</td>
													</tr>
													<tr>
														<td colspan="2"></td>
														<td colspan="3" align="center">↓</td>
														<td></td>
														<td></td>
														<td colspan="2" align="center">↓</td>
													</tr>
													<tr>
														<td class="userana_cell_tbl list_item_bg3" align="center">
															認知顧客
														</td>
														<td>
															<!-- 認知・タイトル -->
															<table cellspacing="0" cellpadding="0" width="100%" border="0">
																<tr class="list_item_bg3">
																	<td class="userana_cell_tbr">　</td>
																</tr>
																<tr class=" list_item_bg1">
																	<td class="userana_cell_blr">アクティブユーザ</td>
																</tr>
																<tr class="list_item_bg1">
																	<td class="userana_cell_blr"><input id="rec_button" type="button" onclick="disp_unactive_detail('rec');" value="＋" style="FONT-SIZE: 6pt">&nbsp;休眠ユーザ</td>
																	
																</tr>
																<tr id="rec_tr1" class="list_item_bg1" style="display:none">
																	<td class="userana_cell_blr">　　　休眠レベル１</td>
																</tr>
																<tr id="rec_tr2" class="list_item_bg1" style="display:none">
																	<td class="userana_cell_blr">　　　休眠レベル２</td>
																</tr>
															</table>
														</td>
														<td></td>
														<td>
															<!-- 認知・対象月 -->
															<table cellspacing="0" cellpadding="0" width="100%" border="0">
																<tr class="list_item_bg3">
																	<td class="userana_cell_tblr" align="center" colspan="2">
																		<%# WebSanitizer.HtmlEncode(DispNumeric(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ALL, "人"))%>
																	</td>
																</tr>
																<tr class="list_item_bg1">
																	<td class="userana_cell_blr" align="right" width="62%">
																		<%# WebSanitizer.HtmlEncode(DispNumeric(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ACTIVE, "人"))%>
																	</td>
																	<td class="userana_cell_br small_text" align="right">
																		<%# WebSanitizer.HtmlEncode(DispRate(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ACTIVE), DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ALL))) %>
																	</td>
																</tr>
																<tr class="list_item_bg1">
																	<td class="userana_cell_blr" align="right">
																		<%# WebSanitizer.HtmlEncode(DispNumeric(m_drvCurrentData, "recognize_unactive_total", "人"))%>
																	</td>
																	<td class="userana_cell_br small_text" align="right">
																		<%# WebSanitizer.HtmlEncode(DispRate(DispData(m_drvCurrentData, "recognize_unactive_total"), DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ALL))) %>
																	</td>
																</tr>
																<tr id="rec_tr3" class="list_item_bg1" align="right" style="display:none">
																	<td class="userana_cell_blr">
																		<%# WebSanitizer.HtmlEncode(DispNumeric(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_UNACTIVE1, "人"))%>
																	</td>
																	<td class="userana_cell_br small_text" align="right">
																		<%# WebSanitizer.HtmlEncode(DispRate(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_UNACTIVE1), DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ALL))) %>
																	</td>
																</tr>
																<tr id="rec_tr4" class="list_item_bg1" align="right" style="display:none">
																	<td class="userana_cell_blr">
																		<%# WebSanitizer.HtmlEncode(DispNumeric(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_UNACTIVE2, "人"))%>
																	</td>
																	<td class="userana_cell_br small_text" align="right">
																		<%# WebSanitizer.HtmlEncode(DispRate(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_UNACTIVE2), DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ALL))) %>
																	</td>
																</tr>
															</table>
														</td>
														<td></td>
														<td>
															<!-- 認知・比率 -->
															<table cellspacing="0" cellpadding="0" width="100%" border="0">
																<tr class="list_item_bg3">
																	<td class="userana_cell_tblr" align="right">
																		<%# WebSanitizer.HtmlEncode(DispIncreasingRate(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ALL), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ALL))) %>
																	</td>
																</tr>
																<tr class="list_item_bg1">
																	<td class="userana_cell_blr" align="right">
																		<%# WebSanitizer.HtmlEncode(DispIncreasingRate(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ACTIVE), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ACTIVE))) %>
																	</td>
																</tr>
																<tr class="list_item_bg1">
																	<td class="userana_cell_blr" align="right">
																		<%# WebSanitizer.HtmlEncode(DispIncreasingRate(DispData(m_drvCurrentData, "recognize_unactive_total"), DispData(m_drvTargetData, "recognize_unactive_total"))) %>
																	</td>
																</tr>
																<tr id="rec_tr5" class="list_item_bg1" align="right" style="display:none">
																	<td class="userana_cell_blr">
																		<%# WebSanitizer.HtmlEncode(DispIncreasingRate(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_UNACTIVE1), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_UNACTIVE1))) %>
																	</td>
																</tr>
																<tr id="rec_tr6" class="list_item_bg1" align="right" style="display:none">
																	<td class="userana_cell_blr">
																		<%# WebSanitizer.HtmlEncode(DispIncreasingRate(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_UNACTIVE2), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_UNACTIVE2))) %>
																	</td>
																</tr>
															</table>
														</td>
														<td></td>
														<td>
															<!-- 認知・対象月 -->
															<table cellspacing="0" cellpadding="0" width="100%" border="0">
																<tr class="list_item_bg3_dark">
																	<td class="userana_cell_tblr" align="center" colspan="2">
																		<%# WebSanitizer.HtmlEncode(DispNumeric(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ALL, "人"))%>
																	</td>
																</tr>
																<tr class="list_item_bg1_dark">
																	<td class="userana_cell_blr" align="right" width="62%">
																		<%# WebSanitizer.HtmlEncode(DispNumeric(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ACTIVE, "人"))%>
																	</td>
																	<td class="userana_cell_br small_text" align="right">
																		<%# WebSanitizer.HtmlEncode(DispRate(DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ACTIVE), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ALL))) %>
																	</td>
																</tr>
																<tr class="list_item_bg1_dark">
																	<td class="userana_cell_blr" align="right">
																		<%# WebSanitizer.HtmlEncode(DispNumeric(m_drvTargetData, "recognize_unactive_total", "人"))%>
																	</td>
																	<td class="userana_cell_br small_text" align="right">
																		<%# WebSanitizer.HtmlEncode(DispRate(DispData(m_drvTargetData, "recognize_unactive_total"), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ALL))) %>
																	</td>
																</tr>
																<tr id="rec_tr7" class="list_item_bg1_dark" align="right" style="display:none">
																	<td class="userana_cell_blr">
																		<%# WebSanitizer.HtmlEncode(DispNumeric(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_UNACTIVE1, "人"))%>
																	</td>
																	<td class="userana_cell_br small_text" align="right">
																		<%# WebSanitizer.HtmlEncode(DispRate(DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_UNACTIVE1), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ALL))) %>
																	</td>
																</tr>
																<tr id="rec_tr8" class="list_item_bg1_dark" align="right" style="display:none">
																	<td class="userana_cell_blr">
																		<%# WebSanitizer.HtmlEncode(DispNumeric(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_UNACTIVE2, "人"))%>
																	</td>
																	<td class="userana_cell_br small_text" align="right">
																		<%# WebSanitizer.HtmlEncode(DispRate(DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_UNACTIVE2), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ALL))) %>
																	</td>
																</tr>
															</table>
														</td>
													</tr>
													<tr>
														<td colspan="2"></td>
														<td colspan="3"align="center">↓</td>
														<td></td>
														<td></td>
														<td colspan="2" align="center">↓</td>
													</tr>
												</table>
												
												<table width="694" cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td width="15%"><img height="12" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
														<!-- 当月退会者・タイトル -->
														<td width="32%" class="userana_cell_tblr">
															退会顧客：当月退会者数
														</td>
														<td width="2%"></td>
														<!-- 当月退会者・対象月 -->
														<td width="18%" class="userana_cell_tblr" align="center">
															<b><%# WebSanitizer.HtmlEncode(DispNumeric(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_LEAVE_NEW + "_total", "人"))%></b>
														</td>
														<td width="1%"></td>
														<!-- 当月退会者・比率 -->
														<td width="12%" class="userana_cell_tblr" align="right">
															<%# WebSanitizer.HtmlEncode(DispIncreasingRate(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_LEAVE_NEW + "_total"), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_LEAVE_NEW + "_total"))) %>
														</td>
														<td width="17"></td>
														<!-- 当月退会者・比較月 -->
														<td width="120" class="userana_cell_tblr_dark" align="center">
															<b><%# WebSanitizer.HtmlEncode(DispNumeric(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_LEAVE_NEW + "_total", "人"))%></b>
														</td>
													</tr>
													<tr>
														<td colspan="2"></td>
														<td colspan="3" align="center">↓</td>
														<td></td>
														<td></td>
														<td colspan="2" align="center">↓</td>
													</tr>
													<tr>
														<!-- 退会者・タイトル -->
														<td class="userana_cell_tbl list_item_bg4" align="center">
															退会顧客
														</td>
														<td>
															<table cellspacing="0" cellpadding="0" width="100%" border="0">
																<tr class="list_item_bg4">
																	<td class="userana_cell_tbr" colspan="2">　</td>
																</tr>
															</table>
														</td>
														<td></td>
														<td>
															<!--退会者・対象月 -->
															<table cellspacing="0" cellpadding="0" width="100%" border="0">
																<tr class="list_item_bg4">
																	<td class="userana_cell_tblr" align="center">
																		<%# WebSanitizer.HtmlEncode(DispNumeric(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_LEAVE_ALL, "人"))%>
																	</td>
																</tr>
															</table>
														</td>
														<td></td>
														<td>
															<!--退会者・比率 -->
															<table cellspacing="0" cellpadding="0" width="100%" border="0">
																<tr class="list_item_bg4">
																	<td class="userana_cell_tblr" align="right">
																		<%# WebSanitizer.HtmlEncode(DispIncreasingRate(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_LEAVE_ALL), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_LEAVE_ALL))) %>
																	</td>
																</tr>
															</table>
														</td>
														<td></td>
														<td>
															<!--退会者・比較月 -->
															<table cellspacing="0" cellpadding="0" width="100%" border="0">
																<tr class="list_item_bg4_dark">
																	<td class="userana_cell_tblr" align="center" colspan="2">
																		<%# WebSanitizer.HtmlEncode(DispNumeric(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_LEAVE_ALL, "人"))%>
																	</td>
																</tr>
															</table>
														</td>
													</tr>
												</table>
												<img height="30" alt="" src="../../Images/Common/sp.gif" width="100%" border="0">
												
												<!-- 備考 -->
												<table class="info_table" width="700" border="0" cellspacing="1" cellpadding="3">
													<tr class="info_item_bg">
														<td align="left">
														備考
															<table>
																<tr>
																	<td colspan="3">■対象</td>
																</tr>
																<tr>
																	<td colspan="3">本レポートはPC・スマフォサイトが対象となります。</td>
																</tr>
															</table>
															<br />
															<table>
																<tr>
																	<td colspan="3">■潜在ユーザー：新規獲得数</td>
																</tr>
																<tr>
																	<td colspan="3">指定した月の新規獲得ユーザ数及び増減率を表示します。</td>
																</tr>
															</table>
															<br />
															<table>
																<tr>
																	<td colspan="3">■潜在ユーザー</td>
																</tr>
																<tr>
																	<td>・潜在ユーザー</td>
																	<td>・・・</td>
																	<td>指定した月迄の潜在ユーザ合計数及び増減率を表示します。</td>
																</tr>
																<tr>
																	<td>・アクティブユーザー</td>
																	<td>・・・</td>
																	<td>指定した月迄で３０日以内にアクセスのあったユーザ合計数及び増減率を表示します。</td>
																</tr>

															</table>
															<br />
															<table>
																<tr>
																	<td colspan="3">■認知顧客：新規獲得数</td>
																</tr>
																<tr>
																	<td colspan="3">指定した月の潜在ユーザから認知顧客として新規獲得された件数及び増減率が表示されます。</td>
																</tr>
															</table>
															<br />
															<table>
																<tr>
																	<td colspan="3">■認知顧客</td>
																</tr>
																<tr>
																	<td>・認知顧客</td>
																	<td>・・・</td>
																	<td>指定した月迄の認知顧客合計人数及び増減率を表示します。</td>
																</tr>
																<tr>
																	<td>・アクティブユーザ</td>
																	<td>・・・</td>
																	<td>指定した月迄で、３０日以内にアクセスのあったユーザ合計数及び増減率を表示します。</td>
																</tr>
															</table>
															<br />
															<table>
																<tr>
																	<td colspan="3">■休眠ユーザー</td>
																</tr>
																<tr>
																	<td>・休眠ユーザー</td>
																	<td>・・・</td>
																	<td>30日間アクセスがないユーザー</td>
																</tr>
																<tr>
																	<td>・休眠レベル１</td>
																	<td>・・・</td>
																	<td>休眠ユーザーで、かつ60日以内にアクセスがあるユーザー</td>
																</tr>
																<tr>
																	<td>・休眠レベル２</td>
																	<td>・・・</td>
																	<td>過去60日以内にアクセスがないユーザー</td>
																</tr>
															</table>
															<br />
															<table>
																<tr>
																	<td colspan="3">■未確定データ</td>
																</tr>
																<tr>
																	<td colspan="3">・「<span class="notice">*</span>」付きの月のデータは、月末までのデータ集計が完了していない未確定データとなります。</td>
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
	</tr> <!--△ 一覧 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>