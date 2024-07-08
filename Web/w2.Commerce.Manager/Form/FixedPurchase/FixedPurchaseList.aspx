<%--
=========================================================================================================
  Module      : 定期購入情報一覧ページ(FixedPurchaseList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Order" %>
<%@ Import Namespace="w2.Domain.FixedPurchase.Helper" %>
<%@ Reference Page="~/Form/PdfOutput/PdfOutput.aspx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="FixedPurchaseList.aspx.cs" Inherits="Form_FixedPurchase_FixedPurchaseList" %>
<%-- マスタ出力コントロール --%>
<%@ Register TagPrefix="uc" TagName="MasterDownLoad" Src="~/Form/Common/MasterDownLoadPanel.ascx" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<link rel="stylesheet" type="text/css" href="../../Css/hide-field-button-style.css">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td>
			<h1 class="page-title">定期台帳</h1>
		</td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 検索 ▽-->
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
											<td class="search_table">
												<table cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="search_title_bg" width="140"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />定期購入ID</td>
														<td class="search_item_bg" width="105"><asp:TextBox id="tbFixedPurchaseId" runat="server" Width="120" /></td>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />ユーザーID</td>
														<td class="search_item_bg"><asp:TextBox id="tbUserId" runat="server" Width="120" /></td>
														<td class="search_title_bg" width="110"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />並び順</td>
														<td class="search_item_bg" width="105">
															<asp:DropDownList id="ddlSortKbn" runat="server" CssClass="search_item_width"/>
														</td>
														<td class="search_btn_bg" width="83" rowspan="<%= CalculateSearchRowSpan() %>">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_FIXEDPURCHASE_LIST %>">クリア</a>&nbsp;
																<a href="javascript:Reset();">リセット</a></div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />注文者名</td>
														<td class="search_item_bg" ><asp:TextBox id="tbUserName" runat="server" Width="120" /></td>
														<td class="search_title_bg" width="140"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />定期購入ステータス</td>
														<td class="search_item_bg" width="105">
															<asp:DropDownList id="ddlFixedPurchaseStatusKbn" runat="server" CssClass="search_item_width" />
														</td>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />決済種別</td>
														<td class="search_item_bg">
															<asp:DropDownList id="ddlOrderPaymentKbn" runat="server" CssClass="search_item_width" />
														</td>
													</tr>
													<tr id="hide-field_OrderShippedCountFrom" style="display: none;">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />購入回数<br/>　(注文基準)</td>
														<td class="search_item_bg">
															<asp:TextBox id="tbOrderCountFrom" runat="server" Width="45" />&nbsp;～&nbsp;<asp:TextBox id="tbOrderCountTo" runat="server" Width="45" />
														</td>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />購入回数<br/>　(出荷基準)</td>
														<td class="search_item_bg" colspan="3">
															<asp:TextBox id="tbShippedCountFrom" runat="server" Width="45" />&nbsp;～&nbsp;<asp:TextBox id="tbShippedCountTo" runat="server" Width="45" />
														</td>
													</tr>
													<tr id="hide-field_SubscriptionBox" style="display: none;">
														<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED) { %>
															<td class="search_title_bg">頒布会コース</td>
															<td class="search_item_bg">
																<asp:DropDownList id="ddlIsSubscriptionBox" runat="server" CssClass="search_item_width" />
															</td>
															<td class="search_title_bg">頒布会コースID</td>
															<td class="search_item_bg"><asp:TextBox id="tbSubscriptionBoxCourseId" runat="server" Width="120" /></td>
															<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />頒布会購入回数</td>
															<td class="search_item_bg">
																<asp:TextBox id="tbSubscriptionBoxOrderCountFrom" runat="server" Width="45" />&nbsp;～&nbsp;<asp:TextBox id="tbSubscriptionBoxOrderCountTo" runat="server" Width="45" />
															</td>
														<% } %>
													</tr>
													<tr id="hide-field_OrderKbnPaymentStatus" style="display: none;">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />注文区分</td>
														<td class="search_item_bg">
															<asp:DropDownList id="ddlOrderKbn" runat="server" CssClass="search_item_width" />
														</td>
														<td class="search_title_bg" ><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />決済ステータス</td>
														<td class="search_item_bg" colspan="3">
															<asp:DropDownList id="ddlPaymentStatus" runat="server" CssClass="search_item_width" />
														</td>
													</tr>
													<tr id="hide-field_FixedKbnProductId" style="display: none;">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />定期購入区分</td>
														<td class="search_item_bg">
															<asp:DropDownList id="ddlFixedPurchaseKbn" runat="server" CssClass="search_item_width" />
														</td>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />商品ID</td>
														<td class="search_item_bg" colspan="3">
															<asp:TextBox id="tbProductId" runat="server" Width="120" />
														</td>
													</tr>
													<tr id="hide-field_ShippingInfos" style="display: none;">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />配送先</td>
														<td class="search_item_bg" colspan="3">
															<asp:DropDownList id="ddlShipping" runat="server" CssClass="search_item_width" />
														</td>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />配送方法</td>
														<td align="left" class="search_item_bg">
															<asp:DropDownList ID="ddlShippingMethod" runat="server" />
														</td>
													</tr>
													<tr id="hide-field_fManagementMemo" style="display: none;">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />管理メモ</td>
														<td align="left" class="search_item_bg" colspan="5">
															<asp:DropDownList ID="ddlManagementMemoFlg" runat="server" />
															<asp:TextBox ID="tbManagementMemo" runat="server" Width="387" />
														</td>
													</tr>
													<tr id="hide-field_fShippingMemo" style="display: none;">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />配送メモ</td>
														<td align="left" class="search_item_bg" colspan="5">
															<asp:DropDownList ID="ddlShippingMemoFlg" runat="server" />
															<asp:TextBox ID="tbShippingMemo" runat="server" Width="387" />
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="130"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />日付</td>
														<td class="search_item_bg" colspan="5">
															<asp:DropDownList runat="server" ID="ddlDateType" />が
															<uc:DateTimePickerPeriodInput id="ucDatePeriod" runat="server" IsNullStartDateTime="true" IsHideTime="True" /> の間
															<span class="search_btn_sub">(<a href="Javascript:SetYesterday('datefrom');">昨日</a>｜<a href="Javascript:SetToday('datefrom');">今日</a>｜<a href="Javascript:SetThisMonth('datefrom');">今月</a>)</span>
														</td>
													</tr>
													<tr id="trExtendStatus" class="hide-field_KakuExtendStatus" runat="server" visible="false" style="display: none;">
														<td class="search_title_bg" width="130"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />拡張ステータス</td>
														<td class="search_item_bg" colspan="5">
															<asp:DropDownList id="ddlExtendStatusNo" runat="server" CssClass="search_item_width"></asp:DropDownList>が
															<asp:DropDownList id="ddlExtendStatus" runat="server"></asp:DropDownList>のステータス
														</td>
													</tr>
													<tr id="hide-field_UpdateDateExtendStatus" style="display: none;">
														<td class="search_title_bg" width="130">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />拡張ステータス更新日</td>
														<td class="search_item_bg" colspan="5">
															<asp:DropDownList ID="ddlUpdateDateExtendStatus" runat="server" CssClass="search_item_width"></asp:DropDownList>が
															<uc:DateTimePickerPeriodInput id="ucExtendStatusUpdateDatePeriod" runat="server" IsNullStartDateTime="true" /> の間
															<span class="search_btn_sub">(<a href="Javascript:SetYesterday('extendStatusUpdateDate');">昨日</a>｜<a href="Javascript:SetToday('extendStatusUpdateDate');">今日</a>｜<a href="Javascript:SetThisMonth('extendStatusUpdateDate');">今月</a>)</span>
														</td>
													</tr>
													<%--▼領収書希望有無▼--%>
													<% if (Constants.RECEIPT_OPTION_ENABLED) { %>
													<tr id="hide-field_ReceiptFlg" style="display: none;">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />領収書希望</td>
														<td class="search_item_bg" colspan="5"><asp:DropDownList id="ddlReceiptFlg" runat="server" /></td>
													</tr>
													<% } %>
													<%--▲領収書希望有無▲--%>
													<% if (Constants.ORDER_EXTEND_OPTION_ENABLED) { %>
													<tr id="hide-field_OrderExtendName" style="display: none;">
														<td class="search_title_bg" width="106px"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />注文拡張項目</td>
														<td align="left" class="search_item_bg" colspan="5">
															<asp:DropDownList id="ddlOrderExtendName" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlOrderExtend_SelectedIndexChanged"></asp:DropDownList>
															<asp:DropDownList id="ddlOrderExtendFlg" Visible="false" runat="server"></asp:DropDownList>
															<asp:TextBox ID="tbOrderExtendText" Visible="false" runat="server" Width="425px"></asp:TextBox>
															<asp:DropDownList id="ddlOrderExtendText" Visible="false" runat="server"></asp:DropDownList>
														</td>
													</tr>
													<% } %>
												</table>

													<div id="fp-hide-search-field-slide-toggle" style="text-align: center;">
														<span id="check-toggle-text-fp">全ての検索項目を表示</span>
														<span id="check-toggle-open">
															<span class="toggle-state-icon icon-arrow-down"/>
														</span>
													</div>

											</td>
										</tr>
										<tr><td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
										<%-- マスタ出力 --%>
										<tr>
											<td class="search_table">
												<uc:MasterDownLoad runat="server" ID="uMasterDownload" DownloadType="FixedPurchase" TableWidth="758" />
											</td>
										</tr>
										<tr><td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
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
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr id="fixedPurchaseListSearchResult">
		<td><h2 class="cmn-hed-h2">定期購入情報一覧</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<div>
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<% if (this.IsNotSearchDefault) { %>
											<tr>
												<td>
													<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr class="list_alert">
															<td><%= WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NOT_SEARCH_DEFAULT) %></td>
														</tr>
													</table>
												</td>
											</tr>
											<% } else { %>
											<tr>
												<td align="left">
													<!--▽ ページング ▽-->
													<table class="list_pager" cellspacing="0" cellpadding="0" border="0" style="width: 750px">
														<tr>
															<td style="height: 22px"><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
															<td class="action_list_sp" style="height: 22px;">
																<%if (Constants.MARKETINGPLANNER_TARGETLIST_OPTION_ENABLE){%>
																	<% btnImportTargetList.OnClientClick = "javascript:open_window('" + ImportTargetListUrlCreator.Create(Constants.KBN_WINDOW_POPUP) + "','Import','width=850,height=370,top=120,left=420,status=no,scrollbars=yes');return false;"; %>
																	<asp:Button ID="btnImportTargetList" runat="server" Text="  ターゲットリスト作成  " Enabled="false" EnableViewState="False" UseSubmitBehavior="False" CssClass="cmn-btn-sub-action" />
																<%} %>
															</td>
														</tr>
													</table>
													<!-- ページング-->
												</td>
											</tr>
											<tr>
												<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td>
													<div>
														<% if (rList.Items.Count > 0) { %>
														<%-- ▽ テーブルヘッダ ▽ --%>
														<div>
															<table border="0" cellpadding="0" cellspacing="0">
																<tr>
																	<td>
																		<!-- ▽ 固定ヘッダ ▽ -->	
																		<div class="div_header_left">
																			<table class="list_table tableHeaderFixed" cellspacing="1" cellpadding="3" border="0" style="height:100%">
																				<tr class="list_title_bg">
																					<td align="center" width="110" rowspan="2">定期購入ID</td>
																				</tr> 
																			</table>
																		</div>
																		<!-- △ 固定ヘッダ △ -->	
																	</td>
																	<td>
																		<!-- ▽ ヘッダ ▽ -->	
																		<div class="div_header_right">
																			<table class="list_table tableHeader" cellspacing="1" cellpadding="3" width="1550" border="0" style="height:100%"> <!-- 水平ヘッダ -->
																				<tr class="list_title_bg">
																					<td align="center" width="260" rowspan="2">注文者名</td>
																					<td align="center" width="90" rowspan="2">定期購入区分</td>
																					<td align="center" width="110" colspan="2">購入回数</td>
																					<td align="center" width="100" rowspan="2">決済種別</td>
																					<td align="center" width="75" rowspan="2">定期購入<br />ステータス</td>
																					<td align="center" width="75" rowspan="2">決済<br />ステータス</td>
																					<td align="center" width="40" rowspan="2">管理<br />メモ</td>
																					<td align="center" width="40" rowspan="2">配送<br />メモ</td>
																					<td align="center" width="80" rowspan="2">注文区分</td>
																					<td align="center" width="140" rowspan="2">次回配送日</td>
																					<td align="center" width="140" rowspan="2">次々回配送日</td>
																					<td align="center" width="140" rowspan="2">定期購入開始日時</td>
																					<td align="center" width="140" rowspan="2">最終購入日</td>
																				</tr>
																				<tr class="list_title_bg">
																					<td class="headernumberfixedpurchase" align="center" width="55">注文基準</td>
																					<td class="headernumberfixedpurchase" align="center" width="55">出荷基準</td>
																				</tr>
																			</table>
																		</div>
																		<!-- △ ヘッダ △ -->	
																	</td>
																</tr>
															</table>
														</div>
														<%-- △ テーブルヘッダ △ --%>
													
														<%-- ▽ テーブルデータ ▽ --%>
														<div class="div_data">
															<%-- ▽ 固定データ ▽ --%>
															<div class="div_data_left" style="max-height: 420px; height:<%= rList.Items.Count * 50 + 20 %>px;">
																	<table class="list_table tableDataFix" cellspacing="1" cellpadding="3" border="0"> <!-- 垂直ヘッダ -->
																		<asp:Repeater id="rTableFixColumn" Runat="server">
																			<HeaderTemplate>
																			</HeaderTemplate>
																			<ItemTemplate>
																				<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateFixedPurchaseDetailUrl(((FixedPurchaseListSearchResult)Container.DataItem).FixedPurchaseId)) %>')">
																					<td align="center" width="110"><%# WebSanitizer.HtmlEncode(((FixedPurchaseListSearchResult)Container.DataItem).FixedPurchaseId)%></td>
																				</tr>
																			</ItemTemplate>
																		</asp:Repeater>
																	</table>
															</div>
															<%-- △ 固定データ △ --%>
															<%-- ▽ データ ▽ --%>
															<div class="div_data_right">
																<table class="list_table tableData" cellspacing="1" cellpadding="3" width="1550" border="0" style="max-height: 420px; height:<%= rList.Items.Count * 50 + 20 %>px;">
																	<asp:Repeater id="rList" ItemType="w2.Domain.FixedPurchase.Helper.FixedPurchaseListSearchResult" Runat="server">
																		<HeaderTemplate>
																		</HeaderTemplate>
																		<ItemTemplate>
																			<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateFixedPurchaseDetailUrl(((FixedPurchaseListSearchResult)Container.DataItem).FixedPurchaseId)) %>')">
																				<td align="center" width="260"><%# WebSanitizer.HtmlEncode(((FixedPurchaseListSearchResult)Container.DataItem).UserName)%></td>
																				<td align="center" width="90"><%# WebSanitizer.HtmlEncode(OrderCommon.CreateFixedPurchaseSettingMessage((FixedPurchaseListSearchResult)Container.DataItem))%></td>
																				<td class="data" align="right" width="55"><%# WebSanitizer.HtmlEncode(((FixedPurchaseListSearchResult)Container.DataItem).OrderCount)%> 回</td>
																				<td class="data" align="right" width="55"><%# WebSanitizer.HtmlEncode(((FixedPurchaseListSearchResult)Container.DataItem).ShippedCount)%> 回</td>
																				<td align="center" width="100"><%# WebSanitizer.HtmlEncode(((FixedPurchaseListSearchResult)Container.DataItem).PaymentName)%></td>
																				<td align="center" width="75">
																					<span class="<%# GetFixedPurchaseStatusCssClass(((FixedPurchaseListSearchResult)Container.DataItem).FixedPurchaseStatus) %>">
																						<%# WebSanitizer.HtmlEncode(((FixedPurchaseListSearchResult)Container.DataItem).FixedPurchaseStatusText)%>
																					</span></td>
																				<td align="center" width="75">
																					<span class="<%# GetPaymentStatusCssClass(((FixedPurchaseListSearchResult)Container.DataItem).PaymentStatus) %>">
																						<%# WebSanitizer.HtmlEncode(((FixedPurchaseListSearchResult)Container.DataItem).PaymentStatusText)%>
																					</span>
																				</td>
																				<td align="center" width="40"><%# WebSanitizer.HtmlEncode(((FixedPurchaseListSearchResult)Container.DataItem).FixedPurchaseManagementMemo != "" ? "○" : "-")%></td>
																				<td align="center" width="40"><%#: Item.ShippingMemo != "" ? "○" : "-"%></td>
																				<td align="center" width="80"><%# WebSanitizer.HtmlEncode(((FixedPurchaseListSearchResult)Container.DataItem).OrderKbnText)%></td>
																				<td align="center" width="140"><%#: DateTimeUtility.ToStringForManager(Item.NextShippingDate, DateTimeUtility.FormatType.ShortDate2Letter)%></td>
																				<td align="center" width="140"><%#: DateTimeUtility.ToStringForManager(Item.NextNextShippingDate, DateTimeUtility.FormatType.ShortDate2Letter)%></td>
																				<td align="center" width="140"><%#: DateTimeUtility.ToStringForManager(Item.FixedPurchaseDateBgn,  DateTimeUtility.FormatType.ShortDate2Letter)%></td>
																				<td align="center" width="140"><%#: DateTimeUtility.ToStringForManager(Item.LastOrderDate, DateTimeUtility.FormatType.ShortDate2Letter)%></td>
																			</tr>
																		</ItemTemplate>
																	</asp:Repeater>
																</table>
															</div>
															<%-- △ データ △ --%>
														</div>
														<%-- △ テーブルデータ △ --%>
														<% } else { %>
														<%-- ▽ エラーの場合 ▽ --%>
														<div>
															<table class="list_table" cellspacing="1" cellpadding="3" width="734" border="0" style="height:100%"><!-- 水平ヘッダ -->				
																<tr id="trListError" class="list_alert" runat="server" Visible="False">
																	<td id="tdErrorMessage" colspan="13" runat="server"></td>
																</tr>
															</table>
														</div>
														<%-- △ エラーの場合 △ --%>
														<% } %>
													</div>
												</td>
											</tr>
											<% } %>
											<tr>
												<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
										</table>
									</div>
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
<script type="text/javascript">
	// 昨日設定
	function SetYesterday(set_date) {
		// 拡張ステータス更新日
		if (set_date == 'extendStatusUpdateDate') {
			document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.Date.AddDays(-1)) %>';
			document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.Date.AddDays(-1)) %>';
			document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucExtendStatusUpdateDatePeriod.ClientID %>');
		}
		// 日付
		else if (set_date == 'datefrom') {
			document.getElementById('<%= ucDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.Date.AddDays(-1)) %>';
			document.getElementById('<%= ucDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.Date.AddDays(-1)) %>';
			document.getElementById('<%= ucDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucDatePeriod.ClientID %>');
		}
	}

	// 今日設定
	function SetToday(set_date) {
		// 拡張ステータス更新日
		if (set_date == 'extendStatusUpdateDate') {
			document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
			document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
			document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucExtendStatusUpdateDatePeriod.ClientID %>');
		}
		// 日付
		else if (set_date == 'datefrom') {
			document.getElementById('<%= ucDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
			document.getElementById('<%= ucDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
			document.getElementById('<%= ucDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucDatePeriod.ClientID %>');
		}
	}

	// 今月設定
	function SetThisMonth(set_date) {
		// 拡張ステータス更新日
		if (set_date == 'extendStatusUpdateDate') {
			document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetStartDateThisMonthString() %>';
			document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetEndDateThisMonthString() %>';
			document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucExtendStatusUpdateDatePeriod.ClientID %>');
		}
		// 日付
		else if (set_date == 'datefrom') {
			document.getElementById('<%= ucDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetStartDateThisMonthString() %>';
			document.getElementById('<%= ucDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetEndDateThisMonthString() %>';
			document.getElementById('<%= ucDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucDatePeriod.ClientID %>');
		}
	}

	// テキストボックスの有効無効切り替えイベントの設定
	$(function () {
		// 管理メモのテキストボックス
		SetDisplayMemoTextBox($("#<%=ddlManagementMemoFlg.ClientID %>"), $("#<%=tbManagementMemo.ClientID %>"));
		$("#<%=ddlManagementMemoFlg.ClientID %>").change(function () { SetDisplayMemoTextBox($("#<%=ddlManagementMemoFlg.ClientID %>"), $("#<%=tbManagementMemo.ClientID %>")); });

		// 配送メモのテキストボックス
		var shippingMemoFlg = $("#<%=ddlShippingMemoFlg.ClientID %>");
		var shippingMemo = $("#<%=tbShippingMemo.ClientID %>");
		SetDisplayMemoTextBox(shippingMemoFlg, shippingMemo);
		shippingMemoFlg.change(function () { SetDisplayMemoTextBox(shippingMemoFlg, shippingMemo); });
	});

	// Form Reset
	function Reset()
	{
		document.getElementById('<%= ucDatePeriod.HfStartDate.ClientID %>').value = '';
		document.getElementById('<%= ucDatePeriod.HfStartTime.ClientID %>').value = '';
		document.getElementById('<%= ucDatePeriod.HfEndDate.ClientID %>').value = '';
		document.getElementById('<%= ucDatePeriod.HfEndTime.ClientID %>').value = '';
		reloadDisplayDateTimePeriod('<%= ucDatePeriod.ClientID %>');

		document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfStartDate.ClientID %>').value = '';
		document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfStartTime.ClientID %>').value = '';
		document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfEndDate.ClientID %>').value = '';
		document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfEndTime.ClientID %>').value = '';
		reloadDisplayDateTimePeriod('<%= ucExtendStatusUpdateDatePeriod.ClientID %>');

		this.document.<%= this.Form.ClientID %>.reset();
		SetDisplayMemoTextBox($("#<%=ddlManagementMemoFlg.ClientID %>"), $("#<%=tbManagementMemo.ClientID %>"));
		SetDisplayMemoTextBox($("#<%=ddlShippingMemoFlg.ClientID %>"), $("#<%=tbShippingMemo.ClientID %>"));
	}

	// ドロップダウンの値に応じて、テキストボックスの有効無効を切り替える。
	function SetDisplayMemoTextBox($ddlEle, $tbEle) {
		if ($ddlEle.val() != 1) {
			$tbEle.val("");
			$tbEle.attr("disabled", "disabled");
		}
		else {
			$tbEle.removeAttr("disabled");
		}
	}

</script>

<script type="text/javascript">
	$(function () {
		$(".div_header_left").height($(".div_header_right").outerHeight());
		$(".div_header_right").height($(".div_header_left").outerHeight());
		setHeightTwoTable("tableHeaderFixed", "tableHeader");

		$(".div_data_left").height($(".div_data_right").outerHeight());
		setHeightTwoTable("tableDataFix", "tableData");
		setHeightTwoTable("tableData", "tableDataFix"); // ２つテーブルを同じ高さを設定するため

		scrollLeftTwoTable("div_header_right", "div_data_right");
		scrollTopTwoTable("div_data_left", "div_data_right");

		hoverTwoTable("tableDataFix", "tableData");
		hoverTwoTable("tableData", "tableDataFix");
		$(".tableHeader").css("table-layout", "fixed");
		$(".tableData").css("table-layout", "fixed");
		$("td .data").width($("td .headernumberfixedpurchase").width());
	});

	var isMobile = getMobileOperatingSystem();
	if (isMobile) {
		$('.div_data_left').css('overflow-x', 'hidden');
		$('.div_header_right').css('overflow-y', 'hidden');
	}
	else {
		$('.div_data_left').css('overflow-x', 'scroll');
		$('.div_header_right').css('overflow-y', 'scroll');
	}

	$(window).bind('resize', function () {
		var isMobile = getMobileOperatingSystem();
		if (isMobile) {
			$('.div_data_left').css('overflow-x', 'hidden');
			$('.div_header_right').css('overflow-y', 'hidden');
		}
		else {
			$('.div_data_left').css('overflow-x', 'scroll');
			$('.div_header_right').css('overflow-y', 'scroll');
		}
	});
</script>
<%--// 検索時の非表示--%>
<script type="text/javascript" src="<%= ResolveUrl("~/Js/hide-show_search_field.js") %>"></script>
</asp:Content>
<asp:Content ID="Content2" runat="server" 
	contentplaceholderid="ContentPlaceHolderHead">
	<style type="text/css">
		.style1
		{
			height: 6px;
		}
	</style>
</asp:Content>