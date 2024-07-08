<%--
=========================================================================================================
  Module      : 広告コードレポート一覧ページ(AdvertisementCodeReportList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="AdvertisementCodeReportList.aspx.cs" Inherits="Form_AdvertisementCodeReport_AdvertisementCodeReportList" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<script type="text/javascript">
<!--
	// 別ウィンドウ表示
	function open_window(link_file, window_name, window_type) {
		var new_win = window.open(link_file, window_name, window_type);
		new_win.focus();
	}
//-->
</script>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">広告コードレポート</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 検索 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">レポート対象選択</h2></td>
	</tr>
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
											<td class="search_box" align="left">
												<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="search_title_bg" width="120" rowspan="2"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />集計区分</td>
														<td class="search_item_bg">
															<asp:RadioButtonList id="rblTotalKbn" runat="server" Width="430" RepeatDirection="Horizontal" AutoPostBack="true"></asp:RadioButtonList>
														</td>
													</tr>
													<tr>
														<td class="search_item_bg">
															<asp:RadioButtonList id="rblCareer" runat="server" Width="290" RepeatDirection="Horizontal" AutoPostBack="true"></asp:RadioButtonList>
														</td>
													</tr>
												</table>
												<br />
												<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="search_title_bg" colspan="4"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />全体レポート条件</td>
														<td class="search_btn_bg" width="83" rowspan="10">
															<div class="search_btn_main"><asp:Button id="btnSearch" runat="server" Text="  検索  " onclick="btnSearch_Click"></asp:Button></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ADVERTISMENT_CODE_REPORT_LIST %>">クリア</a><br/>
																<asp:LinkButton id="lbReportExport" Runat="server" OnClick="lbReportExport_Click">  CSVダウンロード  </asp:LinkButton>
															</div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="120" ><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />広告媒体区分</td>
														<td class="search_item_bg" width="130" ><asp:DropDownList id="ddlAdvCodeMediaType" runat="server" Width="120"></asp:DropDownList></td>
														<td class="search_title_bg" width="120"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />広告コード</td>
														<td class="search_item_bg">
															<asp:RadioButton id="rbAdvertisementCodeAll" runat="server" Text="全て" GroupName="AdvertisementCode" Checked="True" />
															<asp:RadioButton id="rbAdvertisementCode" runat="server" Text="個別指定" GroupName="AdvertisementCode" />&nbsp;<asp:TextBox ID="tbAdvertisementCode" runat="server"></asp:TextBox>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="120"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />期間指定1</td>
														<td class="search_item_bg" colspan="3">
															<uc:DateTimePickerPeriodInput id="ucTerm1Period" runat="server" IsNullStartDateTime="true" IsHideTime="true"/>
															<span class="search_btn_sub">(<a href="Javascript:SetYesterday('term1period');">昨日</a>｜<a href="Javascript:SetToday('term1period');">今日</a>｜<a href="Javascript:SetThisMonth('term1period');">今月</a>)</span>
															<span class="notice" id="spErrorMessageTerm1" runat="server"></span>
														</td>
													</tr>
													<%-- ユーザ集計の場合のみ表示 --%>
													<% if(rblTotalKbn.SelectedValue == KBN_ADVC_TOTAL_KBN_USER){ %>
													<tr>
														<td class="search_title_bg" colspan="4"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />絞込み条件</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="120"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />期間指定2</td>
														<td class="search_item_bg" colspan="3">
															<uc:DateTimePickerPeriodInput id="ucTerm2Period" runat="server" IsNullStartDateTime="true" IsHideTime="true"/>
															<span class="search_btn_sub">(<a href="Javascript:SetYesterday('term2period');">昨日</a>｜<a href="Javascript:SetToday('term2period');">今日</a>｜<a href="Javascript:SetThisMonth('term2period');">今月</a>)</span>
															<span class="notice" id="spErrorMessageTerm2" runat="server"></span><br />
															※期間指定2を指定した場合の登録ユーザ数、退会ユーザ数<br />
															登録ユーザ数：期間指定1の期間に登録し、期間指定2の期間に退会していないユーザ数<br />
															退会ユーザ数：期間指定1の期間に登録し、期間指定2の期間に退会したユーザ数<br />
														</td>
													</tr>
													<% } %>
													<%-- 売上表示(出荷基準)の場合のみ表示 --%>
													<% if(rblTotalKbn.SelectedValue == KBN_ADVC_TOTAL_KBN_ORDER){ %>
													<tr>
														<td class="search_title_bg" colspan="4"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />絞込み条件</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="120"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />対象広告コード</td>
														<td class="search_item_bg" colspan="3">
															<asp:RadioButtonList id="rblAdvertisementCodeTarget" runat="server" Width="300" RepeatDirection="Horizontal" AutoPostBack="True" RepeatLayout="Flow">
																<asp:ListItem Text="注文時の広告コード" Value="0" Selected="True"></asp:ListItem>
																<asp:ListItem Text="初回の広告コード" Value="1"></asp:ListItem>
															</asp:RadioButtonList>
														</td>
													</tr>
													<% } %>
													<%-- コンバージョンレート集計の場合のみ表示 --%>
													<% if(rblTotalKbn.SelectedValue == KBN_ADVC_TOTAL_KBN_CVR){ %>
													<tr>
														<td class="search_title_bg" colspan="4"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />絞込み条件</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="120"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />並び替え</td>
														<td class="search_item_bg" colspan="3">
															<asp:DropDownList ID="ddlSortKbn" runat="server">
																<asp:ListItem Value="0" Text="広告コード/昇順" />
																<asp:ListItem Value="1" Text="広告コード/降順" />
																<asp:ListItem Value="2" Text="媒体名/昇順" />
																<asp:ListItem Value="3" Text="媒体名/降順" />
																<asp:ListItem Value="4" Text="クリック流入数(ユニークユーザ数)/昇順" />
																<asp:ListItem Value="5" Text="クリック流入数(ユニークユーザ数)/降順" />
																<asp:ListItem Value="6" Text="登録ユーザ数/昇順" />
																<asp:ListItem Value="7" Text="登録ユーザ数/降順" />
																<asp:ListItem Value="8" Text="退会ユーザ数/昇順" />
																<asp:ListItem Value="9" Text="退会ユーザ数/降順" />
																<asp:ListItem Value="10" Text="注文ユーザ数/昇順" />
																<asp:ListItem Value="11" Text="注文ユーザ数/降順" />
																<asp:ListItem Value="12" Text="注文金額/昇順" />
																<asp:ListItem Value="13" Text="注文金額/降順" />
																<asp:ListItem Value="14" Text="顧客単価/昇順" />
																<asp:ListItem Value="15" Text="顧客単価/降順" />
															</asp:DropDownList>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="120"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />集計基準</td>
														<td class="search_item_bg" colspan="3">
															<asp:RadioButtonList id="rblAdvertisementConversionRateKbn" runat="server" Width="150" RepeatDirection="Horizontal" AutoPostBack="True">
																<asp:ListItem Text="注文基準" Value="0" Selected="True"></asp:ListItem>
																<asp:ListItem Text="出荷基準" Value="1"></asp:ListItem>
															</asp:RadioButtonList>
														</td>
													</tr>
													<% } %>
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
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table border="0" cellspacing="0" cellpadding="0">
													<tr>
														<td>
															<table class="info_table" width="410" border="0" cellspacing="1" cellpadding="3">
																<tr class="info_item_bg">
																	<td align="left" runat="server">集計区分：<% = rblTotalKbn.SelectedItem.Text%>&nbsp;/&nbsp;<% = rblCareer.SelectedItem.Text %></td>
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
										<!--▽ ユーザ集計 ▽-->
										<% if(rblTotalKbn.SelectedValue == KBN_ADVC_TOTAL_KBN_USER){ %>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" border="0" width="700">
													<tr class="list_title_bg">
														<td class="list_title_bg" style="white-space:nowrap" align="left" width="80">広告媒体区分</td>
														<td class="list_title_bg" style="white-space:nowrap" align="left" width="80">広告コード</td>
														<td class="list_title_bg" style="white-space:nowrap" align="left" width="150">媒体名</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="235">登録ユーザ数</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="235">退会ユーザ数</td>
													</tr>
													<tr valign="top">
														<td class="list_item_bg5_dark" colspan="4" style="padding:0px"></td>
													</tr>
													<tr valign="middle">
														<td class="list_title_bg" align="center" colspan='3'>合計</td>
														<td class="list_item_bg1" align="right"><asp:Label ID="lbUserRegistTotal" runat="server"></asp:Label>人</td>
														<td class="list_item_bg1" align="right"><asp:Label ID="lbUserWithdrawalTotal" runat="server"></asp:Label>人</td>
													</tr>
													<tr valign="top">
														<td class="list_item_bg5_dark" colspan="3" style="padding:0px"></td>
													</tr>
													<asp:Repeater id="rUserDataList" Runat="server">
														<ItemTemplate>
															<tr>
																<td class="list_title_bg" align="left"><%# WebSanitizer.HtmlEncode(StringUtility.ToEmpty(Eval(Constants.FIELD_ADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_NAME)))%></td>
																<td class="list_title_bg" align="left"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE))%></td>
																<td class="list_title_bg" align="left">
																	<%# ((string)((System.Data.DataRowView)Container.DataItem)[Constants.FIELD_ADVCODE_MEDIA_NAME]).Length == 0 ? "" : " " + WebSanitizer.HtmlEncode(((System.Data.DataRowView)Container.DataItem)[Constants.FIELD_ADVCODE_MEDIA_NAME]) + "" %></td>
																<td class="list_item_bg1" align="right">
																	<%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(FIELD_ADVC_REGIST_COUNT)))%>人</td>
																<td class="list_item_bg1" align="right">
																	<%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(FIELD_ADVC_WITHDRAWAL_COUNT)))%>人</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trUserListError" class="list_alert" runat="server" Visible="false">
														<td id="tdUserErrorMessage" colspan='5' runat="server">
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<!--△ ユーザ集計 △-->
										<!--▽ 売上表示(出荷基準) ▽-->
										<% } else if(rblTotalKbn.SelectedValue == KBN_ADVC_TOTAL_KBN_ORDER){ %>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" border="0" width="700">
													<tr class="list_title_bg">
														<td class="list_title_bg" style="white-space:nowrap" align="left" width="80">広告媒体区分</td>
														<td class="list_title_bg" style="white-space:nowrap" align="left" width="80">広告コード</td>
														<td class="list_title_bg" style="white-space:nowrap" align="left" width="150">媒体名</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="100">注文ユーザ数</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="100">注文件数</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="140">注文金額</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="65">商品別<br/>詳細</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="65">購入者<br/>詳細</td>
													</tr>
													<tr valign="top">
														<td class="list_item_bg5_dark" colspan="7" style="padding:0px"></td>
													</tr>
													<tr valign="middle" height="20">
														<td class="list_title_bg" align="center" colspan='3'>合計</td>
														<td class="list_item_bg1" align="right"><asp:Label ID="lbUserCountTotal" runat="server"></asp:Label>人</td>
														<td class="list_item_bg1" align="right"><asp:Label ID="lbOrderCountTotal" runat="server"></asp:Label>件</td>
														<td class="list_item_bg1" align="right"><asp:Label ID="lbOrderPriceTotal" runat="server"></asp:Label></td>
														<td class="list_item_bg1" align="center"><a href="javascript:open_window('<%= WebSanitizer.UrlAttrHtmlEncode(CreateAdvertisementCodeReportDetailUrl("", Constants.KBN_ADVCODE_DISP_RANKING_PRODUCT_BUY)) %>','productstockhistory_list','width=800,height=685,top=120,left=320,status=NO,scrollbars=yes');"><img src="../../Images/Common/icon_data.gif" /></a></td>
														<td class="list_item_bg1" align="center"><a href="javascript:open_window('<%= WebSanitizer.UrlAttrHtmlEncode(CreateAdvertisementCodeReportDetailUrl("", Constants.KBN_ADVCODE_DISP_ORDER)) %>','productstockhistory_list','width=800,height=685,top=120,left=320,status=NO,scrollbars=yes');"><img src="../../Images/Common/icon_data.gif" /></a></td>
													</tr>
													<tr valign="top">
														<td class="list_item_bg5_dark" colspan="7" style="padding:0px"></td>
													</tr>
													<asp:Repeater id="rOrderDataList" Runat="server">
														<ItemTemplate>
															<tr>
																<td class="list_title_bg" align="left"><%# WebSanitizer.HtmlEncode(StringUtility.ToEmpty(Eval(Constants.FIELD_ADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_NAME)))%></td>
																<td class="list_title_bg" align="left"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE))%></td>
																<td class="list_title_bg" align="left"><%# WebSanitizer.HtmlEncode(((System.Data.DataRowView)Container.DataItem)[Constants.FIELD_ADVCODE_MEDIA_NAME]) %></td>
																<td class="list_item_bg1" align="right">
																	<%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(FIELD_ADVC_USER_COUNT)))%>人</td>
																<td class="list_item_bg1" align="right">
																	<%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(FIELD_ADVC_ORDER_COUNT)))%>件</td>
																<td class="list_item_bg1" align="right">
																	<%# WebSanitizer.HtmlEncode(Eval(FIELD_ADVC_ORDER_PRICE).ToPriceString(true))%></td>
																<td class="list_item_bg1" align="center">
																	<a href="javascript:open_window('<%# WebSanitizer.UrlAttrHtmlEncode(CreateAdvertisementCodeReportDetailUrl((string)Eval(Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE), Constants.KBN_ADVCODE_DISP_RANKING_PRODUCT_BUY)) %>','productstockhistory_list','width=800,height=685,top=120,left=320,status=NO,scrollbars=yes');"><img src="../../Images/Common/icon_data.gif" /></a>
																	</td>
																<td class="list_item_bg1" align="center">
																	<a href="javascript:open_window('<%# WebSanitizer.UrlAttrHtmlEncode(CreateAdvertisementCodeReportDetailUrl((string)Eval(Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE), Constants.KBN_ADVCODE_DISP_ORDER)) %>','productstockhistory_list','width=800,height=685,top=120,left=320,status=NO,scrollbars=yes');"><img src="../../Images/Common/icon_data.gif" /></a>
																	</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trOrderListError" class="list_alert" runat="server" Visible="false">
														<td id="tdOrderErrorMessage" colspan="8" runat="server">
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<!--△ 売上表示(出荷基準) △-->
										<!--▽ PV集計 ▽-->
										<% } else if(rblTotalKbn.SelectedValue == KBN_ADVC_TOTAL_KBN_PV){ %>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" border="0" width="700">
													<tr class="list_title_bg">
														<td class="list_title_bg" style="white-space:nowrap" align="left" width="80">広告媒体区分</td>
														<td class="list_title_bg" style="white-space:nowrap" align="left" width="80">広告コード</td>
														<td class="list_title_bg" style="white-space:nowrap" align="left" width="150">媒体名</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="235">クリック流入数</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="235">クリック流入数(ユニークユーザ数)</td>
													</tr>
													<tr valign="top">
														<td class="list_item_bg5_dark" colspan="4" style="padding:0px"></td>
													</tr>
													<tr valign="middle" height="20">
														<td class="list_title_bg" align="center" colspan='3'>合計</td>
														<td class="list_item_bg1" align="right"><asp:Label ID="lbPVClickCountTotal" runat="server"></asp:Label></td>
														<td class="list_item_bg1" align="right"><asp:Label ID="lbPVClickUniqueCountTotal" runat="server"></asp:Label></td>
													</tr>
													<tr valign="top">
														<td class="list_item_bg5_dark" colspan="4" style="padding:0px"></td>
													</tr>
													<asp:Repeater id="rPVDataList" Runat="server">
														<ItemTemplate>
															<tr>
																<td class="list_title_bg" align="left"><%# WebSanitizer.HtmlEncode(StringUtility.ToEmpty(Eval(Constants.FIELD_ADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_NAME)))%></td>
																<td class="list_title_bg" align="left"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE))%></td>
																<td class="list_title_bg" align="left">
																	<%# ((string)((System.Data.DataRowView)Container.DataItem)[Constants.FIELD_ADVCODE_MEDIA_NAME]).Length == 0 ? "" : " " + WebSanitizer.HtmlEncode(((System.Data.DataRowView)Container.DataItem)[Constants.FIELD_ADVCODE_MEDIA_NAME]) + "" %></td>
																<td class="list_item_bg1" align="right">
																	<%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(FIELD_ADVC_CLICK_COUNT)))%></td>
																<td class="list_item_bg1" align="right">
																	<%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(FIELD_ADVC_CLICK_UNIQUE_COUNT)))%></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trPVListError" class="list_alert" runat="server" Visible="false">
														<td id="tdPvErrorMessage" colspan="5" runat="server">
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<!--△ PV集計 △-->
										<!--▽ コンバージョンレート集計 ▽-->
										<% } else if(rblTotalKbn.SelectedValue == KBN_ADVC_TOTAL_KBN_CVR){ %>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" border="0">
													<tr class="list_title_bg">
														<td class="list_title_bg" style="white-space:nowrap" align="left" width="80" rowspan="2">広告媒体区分</td>
														<td class="list_title_bg" style="white-space:nowrap" align="left" width="80" rowspan="2">広告コード</td>
														<td class="list_title_bg" style="white-space:nowrap" align="left" width="150" rowspan="2">媒体名</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="80" rowspan="2">クリック流入数<br />(ユニーク)</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="100" colspan="2">登録ユーザ数</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="100" colspan="2">退会ユーザ数</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="100" colspan="2">注文ユーザ数</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="80" rowspan="2">注文金額</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="80" rowspan="2">顧客単価</td>
													</tr>
													<tr class="list_title_bg">
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="50">人数</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="50">CVR</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="50">人数</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="50">CVR</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="50">人数</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="50">CVR</td>
													</tr>
													<tr valign="top">
														<td class="list_item_bg5_dark" colspan="8" style="padding:0px"></td>
													</tr>
													<tr valign="middle" height="20">
														<td class="list_title_bg" align="center" colspan='3'>合計</td>
														<td class="list_item_bg1" align="right"><asp:Label ID="lbCRClickUniqueCountTotal" runat="server"></asp:Label></td>
														<td class="list_item_bg1" align="right"><asp:Label ID="lbCRUserRegistTotal" runat="server"></asp:Label></td>
														<td class="list_item_bg1" align="right"><asp:Label ID="lbCRUserRegistTotalCvr" runat="server"></asp:Label></td>
														<td class="list_item_bg1" align="right"><asp:Label ID="lbCRUserWithdrawalTotal" runat="server"></asp:Label></td>
														<td class="list_item_bg1" align="right"><asp:Label ID="lbCRUserWithdrawalTotalCvr" runat="server"></asp:Label></td>
														<td class="list_item_bg1" align="right"><asp:Label ID="lbCRUserCountTotal" runat="server"></asp:Label></td>
														<td class="list_item_bg1" align="right"><asp:Label ID="lbCRUserCountTotalCvr" runat="server"></asp:Label></td>
														<td class="list_item_bg1" align="right"><asp:Label ID="lbCROrderPriceTotal" runat="server"></asp:Label></td>
														<td class="list_item_bg1" align="right"><asp:Label ID="lbCROrderPriceTotalAvg" runat="server"></asp:Label></td>
													</tr>
													<tr valign="top">
														<td class="list_item_bg5_dark" colspan="10" style="padding:0px"></td>
													</tr>
													<asp:Repeater id="rConversionRateDataList" Runat="server">
														<ItemTemplate>
															<tr>
																<td class="list_title_bg" align="left"><%# WebSanitizer.HtmlEncode(StringUtility.ToEmpty(Eval(Constants.FIELD_ADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_NAME)))%></td>
																<td class="list_title_bg" align="left"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE))%></td>
																<td class="list_title_bg" align="left">
																	<%# ((string)((System.Data.DataRowView)Container.DataItem)[Constants.FIELD_ADVCODE_MEDIA_NAME]).Length == 0 ? "" : " " + WebSanitizer.HtmlEncode(((System.Data.DataRowView)Container.DataItem)[Constants.FIELD_ADVCODE_MEDIA_NAME]) + "" %></td>
																<td class="list_item_bg1" align="right">
																	<%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(FIELD_ADVC_CLICK_UNIQUE_COUNT)))%></td>
																<td class="list_item_bg1" align="right">
																	<%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(FIELD_ADVC_REGIST_COUNT)))%></td>
																<td class="list_item_bg1" align="right">
																	<%# WebSanitizer.HtmlEncode(DispRate(Eval(FIELD_ADVC_REGIST_COUNT), Eval(FIELD_ADVC_CLICK_UNIQUE_COUNT)) + "%")%></td>
																<td class="list_item_bg1" align="right">
																	<%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(FIELD_ADVC_WITHDRAWAL_COUNT)))%></td>
																<td class="list_item_bg1" align="right">
																	<%# WebSanitizer.HtmlEncode(DispRate(Eval(FIELD_ADVC_WITHDRAWAL_COUNT), Eval(FIELD_ADVC_CLICK_UNIQUE_COUNT)) + "%")%></td>
																<td class="list_item_bg1" align="right">
																	<%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(FIELD_ADVC_USER_COUNT)))%></td>
																<td class="list_item_bg1" align="right">
																	<%# WebSanitizer.HtmlEncode(DispRate(Eval(FIELD_ADVC_USER_COUNT), Eval(FIELD_ADVC_CLICK_UNIQUE_COUNT)) + "%")%></td>
																<td class="list_item_bg1" align="right">
																	<%# WebSanitizer.HtmlEncode(Eval(FIELD_ADVC_ORDER_PRICE).ToPriceString(true))%></td>
																<td class="list_item_bg1" align="right">
																	<%# WebSanitizer.HtmlEncode(Eval(FIELD_ADVC_ORDER_PRICE_AVG).ToPriceString(true))%></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trConversionListError" class="list_alert" runat="server" Visible="false">
														<td id="tdConversionErrorMessage" colspan="12" runat="server">
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<% } %>
										<!--△ コンバージョンレート集計 △-->
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="info_table" width="700" border="0" cellspacing="1" cellpadding="3">
													<tr class="info_item_bg">
														<td align="left">
															備考<br />
															・注文に関連するデータは、出荷基準で集計しています。<br />
															　また、返品などは含まれておりません。
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
	</tr>
	<!--△ 一覧 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>

<script type="text/javascript">
	// 昨日設定
	function SetYesterday(set_date) {
		// 期間指定1
		if (set_date == 'term1period') {
			document.getElementById('<%= ucTerm1Period.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.Date.AddDays(-1)) %>';
			document.getElementById('<%= ucTerm1Period.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucTerm1Period.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.Date.AddDays(-1)) %>';
			document.getElementById('<%= ucTerm1Period.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucTerm1Period.ClientID %>');
		}
		// 期間指定2
		else if (set_date == 'term2period') {
			document.getElementById('<%= ucTerm2Period.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.Date.AddDays(-1)) %>';
			document.getElementById('<%= ucTerm2Period.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucTerm2Period.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.Date.AddDays(-1)) %>';
			document.getElementById('<%= ucTerm2Period.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucTerm2Period.ClientID %>');
		}
	}
	// 今日設定
	function SetToday(set_date) {
		// 期間指定1
		if (set_date == 'term1period') {
			document.getElementById('<%= ucTerm1Period.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
			document.getElementById('<%= ucTerm1Period.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucTerm1Period.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
			document.getElementById('<%= ucTerm1Period.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucTerm1Period.ClientID %>');
		}
		// 期間指定2
		else if (set_date == 'term2period') {
			document.getElementById('<%= ucTerm2Period.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
			document.getElementById('<%= ucTerm2Period.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucTerm2Period.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
			document.getElementById('<%= ucTerm2Period.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucTerm2Period.ClientID %>');
		}
	}
	// 今月設定
	function SetThisMonth(set_date) {
		// 期間指定1
		if (set_date == 'term1period') {
			document.getElementById('<%= ucTerm1Period.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetStartDateThisMonthString() %>';
			document.getElementById('<%= ucTerm1Period.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucTerm1Period.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetEndDateThisMonthString() %>';
			document.getElementById('<%= ucTerm1Period.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucTerm1Period.ClientID %>');
		}
		// 期間指定2
		else if (set_date == 'term2period') {
			document.getElementById('<%= ucTerm2Period.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetStartDateThisMonthString() %>';
			document.getElementById('<%= ucTerm2Period.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucTerm2Period.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetEndDateThisMonthString() %>';
			document.getElementById('<%= ucTerm2Period.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucTerm2Period.ClientID %>');
		}
	}
</script>
</asp:Content>
