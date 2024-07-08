<%--
=========================================================================================================
  Module      : レコメンドレポート レポート詳細対象選択ページ(RecommendReportList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="RecommendReportList.aspx.cs" Inherits="Form_RecommendReport_RecommendReportList" MaintainScrollPositionOnPostback="true" %>
<%@ Import Namespace="w2.App.Common.Manager" %>
<%@ Import Namespace="w2.Domain.MenuAuthority.Helper" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<script type="text/javascript">
	// レコメンド検索で選択されたレコメンドIDを設定
	function setRecommendId(recommendId) {
		document.getElementById('<%= tbRecommendId.ClientID %>').value = recommendId;
		__doPostBack('<%= tbRecommendId.UniqueID %>', '');
	}
</script>
<script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.7.2/Chart.bundle.js"></script>
<script type="text/javascript" src="../../Js/Manager.chart.js"></script>
<table width="791" border="0">
	<tr>
		<td><h1 class="page-title">レコメンドレポート</h1></td>
	</tr>
	<tr>
		<td>
			<img height="10" alt="" src="../../Images/Common/sp.gif" width="1" border="0"/>
		</td>
	</tr>
	<tr>
		<td>
			<div class="tabs-wrapper">
				<a href="<%= WebSanitizer.UrlAttrHtmlEncode(CreateRecommendReportTableListUrl()) %>">レポート対象選択</a>
				<a href="#" class="current">詳細レポート対象選択</a>
			</div>
		</td>
	</tr>
	<tr>
		<!-- ▽ レコメンド詳細レポート グラフ ▽ -->
		<td class="tab-contents">
			<table class="box_border" cellpadding="0" width="784" border="0">
				<tr>
					<td class="search_box_bg">
						<table cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/></td>
										</tr>
										<tr>
											<td class="search_box">
												<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
													<tr class="search_title_bg">
														<td width="25%">&nbsp;
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align: middle" border="0"/>対象期間
														</td>
														<td width="75%">
															<table class="trans_table" width="100%" border="0">
																<tr>
																	<td style="vertical-align: middle">&nbsp;
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
													<tr valign="top">
														<td>
															期間指定 (<%= CONST_GET_RECOMMENDREPORT_CONDITION_DAYS%>日以内)<br/><br/>
															<uc:DateTimePickerPeriodInput id="ucTermPeriod" runat="server" IsNullStartDateTime="false" IsHideTime="true"/><br/>
															<asp:Label ID="lRecommendReportDateErrorMessages" runat="server" ForeColor="red"></asp:Label><br/>
															<table width="95%" align="center" border="1">
																<tr>
																	<td>
																		<asp:RadioButton runat="server" ID="recommendPv" Text="PV数" AutoPostBack="True" GroupName="ReportType" Checked="True"/><br/>
																		<asp:RadioButton runat="server" ID="recommendCv" Text="CV数" AutoPostBack="True" GroupName="ReportType"/><br/>
																		<asp:RadioButton runat="server" ID="recommendCvp" Text="CV率" AutoPostBack="True" GroupName="ReportType"/><br/>
																	</td>
																</tr>
																<tr>
																	<td>レコメンドID<span class="notice">*</span></td>
																</tr>
																<tr class="search_item_bg">
																	<td height="50">
																		<asp:TextBox id="tbRecommendId" runat="server" Width="180" MaxLength="30" AutoPostBack="true"></asp:TextBox>&nbsp;&nbsp;&nbsp;
																		<input id="inputSearchRecommend" type="button" value=" 選択 " onclick="javascript:open_window('<%:SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Ec, CreateRecommendListUrl()) %>','recommendlist','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');"/>
																		<asp:Button id="btnShowRecommendReport" runat="server" Text=" 表示 " onclick="ShowRecommendReport_Click"></asp:Button><br/>
																		<asp:Label ID="lRecommendReportRecommendIdErrorMessages" runat="server" ForeColor="red"></asp:Label>
																	</td>
																</tr>
															</table>
														</td>
														<td>
															<!-- グラフ -->
															<canvas id="Chart" align="middle" style="margin-top: 20px;display: block; height: 400px; width:1000px;"></canvas>
															<asp:Literal runat="server" ID="lCreateScript"></asp:Literal>
															<div style="float: left">
																<asp:RadioButtonList id="rblCheckNumber" runat="server" Width="160px" RepeatDirection="Horizontal" AutoPostBack="True" CssClass="radio_button_list">
																	<asp:ListItem Value="0">数値あり</asp:ListItem>
																	<asp:ListItem Value="1" Selected="true">数値なし</asp:ListItem>
																</asp:RadioButtonList>
															</div>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/></td>
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
	<!--△ レコメンド詳細レポート グラフ △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="1" border="0"/></td>
	</tr>
	<!--▽ レコメンド詳細レポート 表 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">詳細レポート表示</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellpadding="0" width="784" border="0">
				<tr>
					<td class="list_box_bg">
						<table cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/></td>
										</tr>
										<tr>
											<td>
												<div class="search_btn_sub_rt">
													<asp:LinkButton id="lbReportExport" Runat="server" OnClick="lbReportExport_Click">CSVダウンロード</asp:LinkButton>
												</div>
												<div class="x_scrollable" style="display: block; height: 140px; width: 1730px;">
													<table class="list_table" cellspacing="1" cellpadding="2" width="<%= (_allTableData.Count + 1) * 30 %>" border="0">
														<tr class="list_title_bg">
															<td colspan="1730">
																<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align: middle" border="0"/>
																<asp:Literal ID="lRecommendReportTitle" runat="server" />
															</td>
														</tr>
														<tr class="list_item_bg1" valign="top">
															<asp:Repeater ID="rTableHeader" runat="server">
																<ItemTemplate>
																	<!-- 土曜日、日曜日項目の色対応 -->
																	<td width="30" class="<%# CreateTableDayClassName(((Hashtable)Container.DataItem)[TABLE_HEADER_DAY].ToString()) %>">
																		<div align="center">
																			<!-- 日付 -->
																			<asp:Literal ID="lHeader" runat="server" Text="<%# DateTimeUtility.ToStringForManager(DateTime.Parse(((Hashtable)Container.DataItem)[TABLE_HEADER_DAY].ToString()), DateTimeUtility.FormatType.ShortMonthDay) %>"/>
																		</div>
																	</td>
																</ItemTemplate>
															</asp:Repeater>
														</tr>
														<tr class="list_item_bg1" valign="top">
															<asp:Repeater ID="rTableData" runat="server">
																<ItemTemplate>
																	<td width="30">
																		<div align="center">
																			<!-- 値 -->
																			<asp:Literal ID="lData" runat="server" Text="<%# ((Hashtable)Container.DataItem)[TABLE_DATA_VALUE].ToString() %>"/>
																		</div>
																	</td>
																</ItemTemplate>
															</asp:Repeater>
														</tr>
													</table>
												</div>
												<img height="30" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/>
												<table class="info_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr class="info_item_bg">
														<td align="left">
															<table>
																<tr>
																	<td colspan="3">■項目説明</td>
																</tr>
																<tr>
																	<td>・PV数</td>
																	<td>・・・</td>
																	<td>レコメンドが表示された回数</td>
																</tr>
																<tr>
																	<td>・CV数</td>
																	<td>・・・</td>
																	<td>レコメンド商品が購入された回数</td>
																</tr>
																<tr>
																	<td>・CV率(%)</td>
																	<td>・・・</td>
																	<td>CV数/PV数の割合</td>
																</tr>
															</table>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/></td>
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
	<!--△ レコメンド詳細レポート 表 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="1" border="0"/></td>
	</tr>
</table>
</asp:Content>