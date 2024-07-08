<%--
=========================================================================================================
  Module      : レコメンドレポート レポート対象選択ページ(RecommendReportTableList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="RecommendReportTableList.aspx.cs" Inherits="Form_RecommendReport_RecommendReportTableList" MaintainScrollPositionOnPostback="true" %>
<%@ Import Namespace="w2.Domain.Recommend.Helper" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">レコメンドレポート</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<td>
			<div class="tabs-wrapper">
				<a href="#" class="current">レポート対象選択</a>
				<a href="<%= WebSanitizer.UrlAttrHtmlEncode(CreateRecommendReportListUrl()) %>">詳細レポート対象選択</a>
			</div>
		</td>
	</tr>
	<tr>
		<td class="tab-contents">
			<!--▽ 検索 ▽-->
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
													<tr height="10">
														<td class="search_title_bg" width="135">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															期間指定 (<%= CONST_GET_RECOMMENDREPORT_CONDITION_DAYS%>日以内)
														</td>
														<td class="search_item_bg"  width="540" colspan="5">
															<uc:DateTimePickerPeriodInput id="ucTermPeriod" runat="server" IsNullStartDateTime="false" IsHideTime="true"/>
															<asp:Label ID="lRecommendReportDateErrorMessages" runat="server" ForeColor="red"></asp:Label><br/>
														</td>
														<td class="search_btn_bg" width="83" rowspan="3">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  表示  " OnClick="ShowRecommendReport_Click" />
															</div>
															<div class="search_btn_sub">
																<a href="<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_RECOMMEND_TABLE_REPORT) %>">クリア</a>
																<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a>
															</div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="135">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															レコメンドID
														</td>
														<td class="search_item_bg" width="110">
															<asp:TextBox id="tbRecommendId" runat="server" Width="105" MaxLength="30"></asp:TextBox>
														</td>
														<td class="search_title_bg" width="125">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															レコメンド名(管理用)
														</td>
														<td class="search_item_bg" width="110">
															<asp:TextBox id="tbRecommendName" runat="server" Width="100" MaxLength="100"></asp:TextBox>
														</td>
														<td class="search_title_bg" width="80">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															並び順
														</td>
														<td class="search_item_bg" width="120">
															<asp:DropDownList ID="ddlSortKbn" runat="server" Width="210"></asp:DropDownList>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															レコメンド区分
														</td>
														<td class="search_item_bg">
															<asp:DropDownList ID="ddlRecommendKbn" runat="server" Width="140">
															</asp:DropDownList>
														</td>
														<td class="search_title_bg">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															開催状態
														</td>
														<td class="search_item_bg">
															<asp:DropDownList ID="ddlStatus" runat="server" Width="105">
															</asp:DropDownList>
														</td>
														<td class="search_title_bg">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															有効フラグ
														</td>
														<td class="search_item_bg">
															<asp:DropDownList ID="ddlValidFlg" runat="server" Width="105">
															</asp:DropDownList>
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
			<!--△ 検索 △-->
		</td>
	</tr>
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
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="8" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
													<tr>
														<td width="587" style="height: 22px">
															<asp:Literal ID="lbPager" runat="server"></asp:Literal>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td>
												<div class="search_btn_sub_rt">
													<asp:LinkButton id="lbReportExport" Runat="server" OnClick="lbReportExport_Click">CSVダウンロード</asp:LinkButton>
												</div>
												<table class="info_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr class="info_item_bg">
														<td align="left"><asp:Literal ID="lRecommendReportTitle" runat="server" /></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="list_title_bg" align="center" width="120">レコメンドID</td>
														<td class="list_title_bg" align="center" width="150">レコメンド名(管理用)</td>
														<td class="list_title_bg" align="center" width="60">PV数</td>
														<td class="list_title_bg" align="center" width="60">CV数</td>
														<td class="list_title_bg" align="center" width="60">CV率(%)</td>	
														<td class="list_title_bg" align="center" width="100">表示ページ</td>
														<td class="list_title_bg" align="center" width="90">レコメンド区分</td>
														<td class="list_title_bg" align="center" width="70">開催状態</td>
														<td class="list_title_bg" align="center" width="70">有効フラグ</td>
													</tr>
													<tr valign="middle" height="20" id="lDataTotal" runat="server" >
														<td class="list_title_bg" align="center">合計</td>
														<td class="list_title_bg" align="center"><asp:Literal ID="lDataNameTotal" runat="server" />－</td>
														<td class="list_title_bg" align="center"><asp:Literal ID="lDataPvNumTotal" runat="server" /></td>
														<td class="list_title_bg" align="center"><asp:Literal ID="lDataCvNumTotal" runat="server" /></td>
														<td class="list_title_bg" align="center"><asp:Literal ID="lDataCvpAverager" runat="server" /></td>
														<td class="list_title_bg" align="center"><asp:Literal ID="lDataDisplayPageTotal" runat="server" />－</td>
														<td class="list_title_bg" align="center"><asp:Literal ID="lDataKbnTotal" runat="server" />－</td>
														<td class="list_title_bg" align="center"><asp:Literal ID="lDataStatusTotal" runat="server" />－</td>
														<td class="list_title_bg" align="center"><asp:Literal ID="lDataValidFlgTotal" runat="server" />－</td>
													</tr>
													<asp:Repeater id="rList" Runat="server" ItemType="w2.Domain.Recommend.Helper.RecommendListSearchResult">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" id="trItem<%# Container.ItemIndex %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)">
																<!-- レコメンドID -->
																<td align="center" class="list_title_bg" onclick="listselect_mclick(getElementById('trItem<%# Container.ItemIndex %>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateRecommendSettingUrl(Item.RecommendId)) %>')"><%# Item.RecommendId %></td>
																<!-- レコメンド名(管理用) -->
																<td align="center" onclick="listselect_mclick(getElementById('trItem<%# Container.ItemIndex %>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateRecommendSettingUrl(Item.RecommendId)) %>')"><%#: Item.RecommendName %></td>
																<!-- PV数 -->
																<td align="center" onclick="listselect_mclick(getElementById('trItem<%# Container.ItemIndex %>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateRecommendReportListUrl(Item.RecommendId, "0")) %>')"><%#: Item.PvNumber %></td>
																<!-- CV数 -->
																<td align="center" onclick="listselect_mclick(getElementById('trItem<%# Container.ItemIndex %>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateRecommendReportListUrl(Item.RecommendId, "1")) %>')"><%#: Item.CvNumber %></td>
																<!-- CV率(%) -->
																<td align="center" onclick="listselect_mclick(getElementById('trItem<%# Container.ItemIndex %>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateRecommendReportListUrl(Item.RecommendId, "2")) %>')"><%#: Item.CvPercent %></td>
																<!-- 表示ページ -->
																<td align="center" onclick="listselect_mclick(getElementById('trItem<%# Container.ItemIndex %>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateRecommendSettingUrl(Item.RecommendId)) %>')"><%# Item.RecommendDisplayPageText %></td>
																<!-- レコメンド区分 -->
																<td align="center" onclick="listselect_mclick(getElementById('trItem<%# Container.ItemIndex %>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateRecommendSettingUrl(Item.RecommendId)) %>')"><%# Item.RecommendKbnText %></td>
																<!-- 開催状態 -->
																<td align="center" onclick="listselect_mclick(getElementById('trItem<%# Container.ItemIndex %>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateRecommendSettingUrl(Item.RecommendId)) %>')">
																	<span visible="<%# (Item.Status == RecommendListSearchResult.StatusType.Preparing) || (Item.Status == RecommendListSearchResult.StatusType.Suspended) %>" runat="server">
																		<%#: ValueText.GetValueText(Constants.TABLE_RECOMMEND, Constants.REQUEST_KEY_RECOMMEND_STATUS, Item.Status) %>
																	</span>
																	<span visible="<%# Item.Status == RecommendListSearchResult.StatusType.OnGoing %>" runat="server">
																		<strong><%#: ValueText.GetValueText(Constants.TABLE_RECOMMEND, Constants.REQUEST_KEY_RECOMMEND_STATUS, Item.Status) %></strong>
																	</span>
																	<span visible="<%# Item.Status == RecommendListSearchResult.StatusType.Finished %>" runat="server">
																		<span disabled="true" runat="server"><%#: ValueText.GetValueText(Constants.TABLE_RECOMMEND, Constants.REQUEST_KEY_RECOMMEND_STATUS, Item.Status) %></span>
																	</span>
																</td>
																<!-- 有効フラグ -->
																<td align="center" onclick="listselect_mclick(getElementById('trItem<%# Container.ItemIndex %>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateRecommendSettingUrl(Item.RecommendId)) %>')"><%#: Item.ValidFlgText %></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="9"></td>
													</tr>
													<tr valign="top">
														<td class="list_item_bg5_dark" colspan="11" style="padding:0px"></td>
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
	<!--△ 一覧 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>