<%--
=========================================================================================================
  Module      : 定期回数別レポートページ(OrderRepeatReport.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OrderRepeatReport.aspx.cs" Inherits="Form_OrderRepeatReport_OrderRepeatReport" MaintainScrollPositionOnPostback="true" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">

<style type="text/css">
.div_header_left {
	width:170px;
	overflow-x:hidden;
	overflow-y:hidden;
}
.div_header_right {
	width:600px; 
	height:100%;
	overflow-x:hidden;
	overflow-y:hidden;
}
.div_data {
	position: relative;
	width:100%;
}
.div_data_left {
	position: absolute;
	left:0px;
	width:150px;
	overflow-x:hidden;
	overflow-y:hidden;
}
.div_data_right {
	position: absolute;
	left:150px;
	overflow-x:scroll;
	overflow-y:scroll;
}

.tableHeader td {
	width:95px;
	word-wrap:break-word;
	text-align:center;
}

.tableData td {
	width:95px;
	text-align:right;
}

.tinyTip .tinyTip_content {
	text-align: center;
	font-size: 11px;
}
</style>

<script type="text/javascript">
<!--
	// 広告コード検索で選択されたコードを設定
	function setAdvertisementCode(advcode)
	{
		var advCodes = document.getElementById('<%= tbAdvCode.ClientID %>').value;
		document.getElementById('<%= tbAdvCode.ClientID %>').value = (advCodes == "") ? advcode : (advCodes + "," + advcode);
		__doPostBack('<%= tbAdvCode.UniqueID %>', '');
	}

	// レポート行の展開ボタンを押す処理
	function dispUnactiveDetail(pos)
	{
		// 設定値決定
		var disp = "";
		var next = "";

		// 定期2回目以降の展開ボタンがクリックされる場合
		if (parseInt(pos) === 25)
		{
			disp = "";
			next = "[－]";
			// 定期2回目以降の行を非表示
			document.getElementById("left_tr25").style.display = "none";
			document.getElementById("right_tr25").style.display = "none";
		}
		else
		{
			disp = "none";
			next = "[＋]";
			// 定期2回目以降の行を表示
			document.getElementById("left_tr25").style.display = "";
			document.getElementById("right_tr25").style.display = "";
		}

		// 定期2回目～定期20回目以降の各行を表示/非表示
		for (var iLoop = 6; iLoop <= 24; iLoop++)
		{
			document.getElementById("left_tr" + iLoop).style.display = disp;
			document.getElementById("right_tr" + iLoop).style.display = disp;
		}

		// ボタン文字変更
		document.getElementById("aExpand6").innerText = next;

		// レポート表示エリアの高さを調整
		setDivDataHeight();
	}
//-->
</script>

<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">定期回数別レポート</h1></td>
	</tr>
	<tr>
		<td><img height="3" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
				<td class="search_box">
					<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
					<tr class="search_item_bg">
					<td width="200">
						<!-- ▽カレンダ▽ --><asp:label id="lblCurrentCalendar" Runat="server"></asp:label>
						<!-- △カレンダ△ -->
					</td>
					<td width="558">
						<table cellspacing="0" cellpadding="0" border="0" class="order-condition-report-list-search">
						<tr>
							<td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
						</tr>
						<tr> <!--▽売上基準・レポート種別▽ -->
							<td class="search_box" width="5"></td>
							<td class="search_box">
								<table cellpadding="0" cellspacing="0" border="0">
								<tr>
								<td class="search_box" valign="top">
									<table class="search_table" cellspacing="1" cellpadding="2" width="100" border="0">
									<tr class="search_title_bg">
										<td width="30%">
											<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
											売上基準
										</td>
									</tr>
									<tr class="search_item_bg">
										<td>
											<asp:RadioButtonList id="rblSalesType" Runat="server" CssClass="radio_button_list" RepeatDirection="Vertical" AutoPostBack="true" OnSelectedIndexChanged="rblSalesType_SelectedIndexChanged">
												<asp:ListItem Value="1" Selected="True">注文基準</asp:ListItem>
												<asp:ListItem Value="0">出荷基準</asp:ListItem>
											</asp:RadioButtonList>
										</td>
									</tr>
									</table>
								</td>
								<td class="search_box" width="5"></td>
								<td class="search_box" valign="top">
									<table class="search_table" cellspacing="1" cellpadding="2" width="100" border="0">
									<tr class="search_title_bg">
									<td width="30%">
										<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
										レポート種別
									</td>
									</tr>
									<tr class="search_item_bg">
									<td>
										<asp:RadioButtonList id="rblReportType" Runat="server" CssClass="radio_button_list" RepeatDirection="Vertical" AutoPostBack="true" OnSelectedIndexChanged="rblReportType_SelectedIndexChanged">
										<asp:ListItem Value="0" Selected="True">全体</asp:ListItem>
										<asp:ListItem Value="1">購買区分別</asp:ListItem>
										<asp:ListItem Value="2">決済種別別</asp:ListItem>
										</asp:RadioButtonList>
									</td>
									</tr>
									</table>
								</td>
								<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED) { %>
								<td class="search_box" width="5"></td>
								<td class="search_box" valign="top">
									<table class="search_table" cellspacing="1" cellpadding="2" width="100" border="0">
									<tr class="search_title_bg">
									<td width="30%">
										<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
										定期種別
									</td>
									</tr>
									<tr class="search_item_bg">
									<td>
										<asp:RadioButtonList id="rblRegularType" Runat="server" CssClass="radio_button_list" RepeatDirection="Vertical" AutoPostBack="true" OnSelectedIndexChanged="rblRegularType_SelectedIndexChanged"></asp:RadioButtonList>
									</td>
									</tr>
									</table>
								</td>
								<%} %>
								</tr>
								</table>
							</td>
						</tr> <!--△売上基準・レポート種別△ -->
						<tr>
							<td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
						</tr>
						<tr><!--▽期間指定ト▽ -->
							<td class="search_box" width="5"></td>
							<td class="search_box">
							<table cellpadding="0" cellspacing="0" border="0">
							<tr>
							<td>
								<table class="search_table" cellspacing="1" cellpadding="2" border="0">
								<tr class="search_title_bg">
									<td width="30%">
										<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
										期間指定（<%= CONST_GET_ORDER_DAYS %>日以内）
									</td>
								</tr>
								<tr class="search_item_bg">
									<td>
										<uc:DateTimePickerPeriodInput ID="ucReportDatePeriod" runat="server" IsLoadPage="True" />
										<asp:Label id="lblConditionError" style="color:#f00" runat="server"></asp:Label>
									</td>
								</tr>
								</table>
							</td>
							</tr>
							</table>
							</td>
						</tr><!--△期間指定△ -->
						<tr>
							<td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
						</tr>
						<% if (Constants.MALLCOOPERATION_OPTION_ENABLED) { %>
						<%--▽ モール連携オプションが有効の場合 ▽--%>
						<tr><!--▽サイト▽ -->
							<td class="search_box" width="5"></td>
							<td class="search_box">
								<table cellpadding="0" cellspacing="0" border="0">
								<tr>
								<td>
									<table class="search_table" cellspacing="1" cellpadding="2" border="0">
									<tr class="search_title_bg">
										<td width="30%">
											<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
											サイト
										</td>
									</tr>
									<tr class="search_item_bg">
										<td>
											<asp:DropDownList id="ddlSiteName" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSiteName_SelectedIndexChanged">
												<asp:ListItem Text="全体" Value="" />
											</asp:DropDownList>
										</td>
									</tr>
									</table>
								</td>
								</tr>
								</table>
							</td>
						</tr><!--△サイト△ -->
						<%--△ モール連携オプションが有効の場合 △--%>
						<%} %>
						<tr>
							<td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
						</tr>
						<tr><!--▽広告コード▽ -->
						<td class="search_box" width="5"></td>
						<td class="search_box">
							<table class="search_table" cellspacing="1" cellpadding="2" width="450" border="0">
							<tr class="search_title_bg">
							<td width="30%">
								<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
								広告コード（カンマ区切り）
							</td>
							</tr>
							<tr class="search_item_bg">
							<td>
								<asp:TextBox id="tbAdvCode" runat="server" Width="380" AutoPostBack="true" OnTextChanged="tbAdvCode_TextChanged"></asp:TextBox>&nbsp;&nbsp; &nbsp; 
								<input id="inputSearchAdvCode"  type="button" value=" 選択 " onclick="javascript:open_window('<%# WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ADVERTISEMENT_CODE_SEARCH) %>	','AdvertisementCodeSearch', 'width=820,height=700,top=120,left=420,status=NO,scrollbars=yes');" />
							</td>
							</tr>
							</table>
						</td>
						</tr><!--△広告コード△ -->
						<tr>
							<td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
						</tr>
						<% if (rblRegularType.SelectedValue == REGULAR_TYPE_SUBSCRIPTION_BOX) { %>
						<tr>
							<td class="search_box" width="5"></td>
							<td class="search_box">
								<table cellpadding="0" cellspacing="0" border="0">
								<tr>
								<td>
									<table class="search_table" cellspacing="1" cellpadding="2" border="0">
									<tr class="search_title_bg">
										<td width="30%">
											<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
											頒布会コースID
										</td>
									</tr>
									<tr class="search_item_bg">
										<td>
											<asp:DropDownList id="ddlSubscriptionBoxId" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSubscriptionBoxId_SelectedIndexChanged">
												<asp:ListItem Text="" Value="" />
											</asp:DropDownList>
										</td>
									</tr>
									</table>
								</td>
								</tr>
								</table>
							</td>
						</tr>
						<%} %>
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
	</tr>	
	<!--△ 検索 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ レポート表示 ▽-->
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
								<div class="search_btn_sub_rt" ><asp:LinkButton id="lbReportExport" Runat="server" OnClick="lbReportExport_Click" OnClientClick="lbReportExport_ClientClick();return true;" >CSVダウンロード</asp:LinkButton></div>
								<table class="info_table" width="750" border="0" cellspacing="1" cellpadding="3">
								<tr class="info_item_bg">
								<td align="left" id="tdReportInfo">
									<%: rblReportType.SelectedItem.Text %>　<%: rblSalesType.SelectedItem.Text %>　（期間指定：　<%: this.DateFrom %>～<%: this.DateTo %>）<%# ddlSubscriptionBoxId.SelectedValue != null && ddlSubscriptionBoxId.SelectedValue != "" ? "頒布会コースID："+ ddlSubscriptionBoxId.SelectedValue : "" %>
									<span id="spSiteName" runat="server">　<%: ddlSiteName.SelectedItem.Text %></span>
									<span id="spAdvCode" runat="server">　広告コード（<%: tbAdvCode.Text.Trim().Replace(",", "、") %>）</span>
								</td>
								</tr>
								</table>
								<asp:HiddenField ID="hdReportInfo" runat="server"/>
							</td>
							</tr>
							</table>
						</td>
					</tr>
					<tr>
						<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
					</tr>
					<!--▽ メインエリア ▽-->
					<tr>
						<td>
							<asp:UpdatePanel runat="server">
							<ContentTemplate>
							<div style="width: 100%;">
								<%
									var rightTableWidth = (this.HasExpandedColumn ? ((95 * this.ColumnSpanCount * 7) + (this.ColumnSpanCount * 6 * 5)) : 600);
									%>
								<%-- ▽ レポートヘッダ ▽ --%>
								<div>
									<table border="0" cellspacing="0" cellpadding="0">
									<tr>
									<td>
										<!-- ▽ 固定ヘッダ ▽ -->
										<div class="div_header_left">
											<table class="list_table tableHeaderFix" cellspacing="1" cellpadding="3" border="0" style="height:100%">
											<tr class="list_item_bg3">
												<td width="150px" rowspan="<%= this.HasExpandedColumn ? 2 : 1 %>">&nbsp;</td>
											</tr>
											</table>
										</div>
										<!-- △ 固定ヘッダ △ -->
									</td>
									<td>
										<!-- ▽ ヘッダ ▽ -->
										<div class="div_header_right">
											<table class="list_table tableHeader" cellspacing="1" cellpadding="1" width="<%= rightTableWidth %>" border="0" style="height:100%">
											<tr class="list_item_bg3">
												<asp:Repeater DataSource="<%# this.ReportColumnFields %>" ItemType="System.Web.UI.WebControls.ListItem" runat="server">
												<ItemTemplate>
												<td colspan="<%# this.ColumnSpanCount %>">
													<%#: Item.Text %>
												</td>
												</ItemTemplate>
												</asp:Repeater>
											</tr>

											<% if (this.HasExpandedColumn) { %>
											<tr class="list_item_bg3">
												<asp:Repeater DataSource="<%# this.ReportColumnFields %>" runat="server">
												<ItemTemplate>
													<td>合計</td>
													<%-- 購買区分別レポートの場合 --%>
													<asp:Repeater  DataSource='<%# this.OrderDeviceTypes %>' runat="server" Visible="<%# this.IsExpandedByDeviceType %>" ItemType="System.Web.UI.WebControls.ListItem">
													<ItemTemplate>
														<td><%#: Item.Text %></td>
													</ItemTemplate>
													</asp:Repeater>
													<%-- 決済種別別レポートの場合 --%>
													<asp:Repeater DataSource='<%# this.PaymentTypes %>' runat="server" Visible='<%# this.IsExpandedByPayment %>' ItemType="System.Web.UI.WebControls.ListItem">
													<ItemTemplate>
														<td><%#: Item.Text %></td>
													</ItemTemplate>
													</asp:Repeater>
												</ItemTemplate>
												</asp:Repeater>
											</tr>
											<%} %>
											</table>
										</div>
										<!-- △ ヘッダ △ -->
									</td>
									</tr>
									</table>
								</div>
								<%-- △ レポートヘッダ △ --%>
								<%-- ▽ レポートデータ ▽ --%>
								<div class="div_data">
									<%-- ▽ 固定データ ▽ --%>
									<div class="div_data_left">
										<table class="list_table tableDataFix" cellspacing="1" cellpadding="3" border="0">
											<asp:Repeater runat="server" DataSource ="<%# this.DisplayDataList %>" ItemType="System.Collections.Hashtable">
											<ItemTemplate>
												<tr id="left_tr<%# Container.ItemIndex + 1 %>" valign="middle" height="40px"
													style="<%#: Item[CONST_DISPLAY_FIRST_TIME].ToString() %>">
												<td class="list_item_bg3" align="left" width="15px">
													<%#: Item[FIELD_REPEATORDERREPORT_ORDER_DIVISION_TEXT].ToString() %>
													<span style="<%#: Item[CONST_DISPLAY_TOOLTIP].ToString() %>">
														<img id="help_tr<%# Container.ItemIndex + 1 %>" title="<%#: Item[FIELD_REPEATORDERREPORT_ORDER_DIVISION_TITLE].ToString() %>"
															src="../../Images/Common/help_01.png" alt="" height="13" border="0" width="13">
													</span>
													<span style="<%#: Item[CONST_DISPLAY_BUTTON].ToString() %>">
														<a id="aExpand<%# Container.ItemIndex +1 %>"  href="javascript:dispUnactiveDetail('<%# Container.ItemIndex + 1 %>	');">[ + ]</a>
													</span>
												</td>
												</tr>
												<tr valign="top" style="<%# Container.ItemIndex < 4 ? "" : "display:none"%>">
													<td class="list_item_bg5_dark" style="padding:0px"></td>
												</tr>
											</ItemTemplate>
											</asp:Repeater>
										</table>
									</div>
									<%-- △ 固定データ △ --%>
									<%-- ▽ データ ▽ --%>
									<div class="div_data_right">
										<table class="list_table tableData" cellspacing="1" cellpadding="3" border="0" width="<%= rightTableWidth %>px">
											<asp:Repeater runat="server" DataSource ="<%# this.DisplayDataList %>" ItemType="System.Collections.Hashtable">
											<ItemTemplate>
												<tr id="right_tr<%# Container.ItemIndex + 1 %>" valign="middle" height="40px"
													style="<%#: Item[CONST_DISPLAY_FIRST_TIME].ToString() %>">
													<asp:Repeater runat="server" DataSource='<%# this.ReportColumnFields %>' ItemType="System.Web.UI.WebControls.ListItem">
													<ItemTemplate>
														<td class="<%#: ((Hashtable)((RepeaterItem)(Container.Parent.Parent)).DataItem)[CONST_CLASS].ToString() %>">
															<%#:  ConvertValueWithUnit(Item.Value, ((Hashtable)((RepeaterItem)Container.Parent.Parent).DataItem)[Item.Value]) %>
															<span style="<%# (((RepeaterItem)Container.Parent.Parent).ItemIndex == 0) ? "display:none" : "" %>">
																<br /> <%#: ((Hashtable)((RepeaterItem)Container.Parent.Parent).DataItem)[Item.Value + CONST_PERCENT] %>
															</span>
														</td>
														<!--注文区分別での詳細データ-->
														<asp:Repeater runat="server" DataSource="<%# this.OrderDeviceTypes %>" Visible="<%# this.IsExpandedByDeviceType %>" ItemType="System.Web.UI.WebControls.ListItem">
														<ItemTemplate>
															<td class="<%#: ((Hashtable)((RepeaterItem)(Container.Parent.Parent.Parent.Parent)).DataItem)[CONST_CLASS].ToString() %>">
																<%#: ConvertValueWithUnit(((ListItem)((RepeaterItem)Container.Parent.Parent).DataItem).Value,
																	((Hashtable)((RepeaterItem)Container.Parent.Parent.Parent.Parent).DataItem)[((ListItem)((RepeaterItem)Container.Parent.Parent).DataItem).Value + "_" + Item.Value]) %>
																<span style="<%# (((RepeaterItem)Container.Parent.Parent.Parent.Parent).ItemIndex == 0) ? "display:none" : "" %>">
																	<br /> <%#: ((Hashtable)((RepeaterItem)Container.Parent.Parent.Parent.Parent).DataItem)[((ListItem)((RepeaterItem)Container.Parent.Parent).DataItem).Value + CONST_PERCENT + "_" + Item.Value] %>
																</span>
															</td>
														</ItemTemplate>
														</asp:Repeater>
														<!--決済種別での詳細データ-->
														<asp:Repeater runat="server" DataSource="<%# this.PaymentTypes %>" Visible='<%# this.IsExpandedByPayment %>' ItemType="System.Web.UI.WebControls.ListItem">
														<ItemTemplate>
															<td class="<%#: ((Hashtable)((RepeaterItem)(Container.Parent.Parent.Parent.Parent)).DataItem)[CONST_CLASS].ToString() %>">
																<%#: ConvertValueWithUnit(((ListItem)((RepeaterItem)Container.Parent.Parent).DataItem).Value,
																	((Hashtable)((RepeaterItem)Container.Parent.Parent.Parent.Parent).DataItem)[((ListItem)((RepeaterItem)Container.Parent.Parent).DataItem).Value + "_" + Item.Value]) %>
																<span style="<%# (((RepeaterItem)Container.Parent.Parent.Parent.Parent).ItemIndex == 0) ? "display:none" : "" %>">
																	<br /> <%#: ((Hashtable)((RepeaterItem)Container.Parent.Parent.Parent.Parent).DataItem)[((ListItem)((RepeaterItem)Container.Parent.Parent).DataItem).Value + CONST_PERCENT + "_" + Item.Value] %>
																</span>
															</td>
														</ItemTemplate>
														</asp:Repeater>
													</ItemTemplate>
													</asp:Repeater>
												</tr>
												<tr valign="top" style="<%# Container.ItemIndex < 4 ? "" : "display:none"%>">
													<td class="list_item_bg5_dark" colspan="<%= 6 * this.ColumnSpanCount %>" style="padding:0px"></td>
												</tr>
											</ItemTemplate>
											</asp:Repeater>
											<!-- △レポートのデータ△ -->
										</table>
									</div>
									<%-- △ データ △ --%>
								</div>
								<%-- △ レポートデータ △ --%>
							</div>
							</ContentTemplate>
							</asp:UpdatePanel>
						</td>
					</tr>
					<!--△ メインエリア △-->
					<tr>
						<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
					</tr>
					<tr>
					<td>
						<table class="info_table" width="750" border="0" cellspacing="1" cellpadding="3" id="note" style="clear: both; display: block">
							<tr class="info_item_bg">
							<td align="left">
								備考<br />
								・交換・返品注文および解約、仮注文が集計の対象外となります。<br />
								・注文金額には注文時の手数料、割引金額を含みます。<br />
								　	※注文金額は、小計 + 配送料 + 手数料 + 調整金額<%= (Constants.MEMBER_RANK_OPTION_ENABLED) ? " - 会員ランク割引額 " : ""%><%= (Constants.W2MP_COUPON_OPTION_ENABLED) ? " - クーポン割引額 " : ""%><%= (Constants.W2MP_POINT_OPTION_ENABLED) ? " - 利用ポイント " : ""%>を含みます。<br />
								・注文種別に関しては下記の定義として注文情報を集計しております。<br />
								　　	通常【新規】：　過去に通常注文のないお客様の初めての通常注文です。<br />
								　　	通常【リピート】：　過去に通常注文履歴があるお客様の通常注文です。<br />
								　　	定期（初回）【新規】：　過去に定期注文のないお客様の1回目の定期注文です。<br />
								　　	定期（初回）【リピート】：　過去に定期注文履歴があるお客様の1回目の定期注文です。<br />
								　　	定期【回数別】：　注文の回数別の定期注文です。<br />
								　　	※通常と定期注文を分けて、新規・リピートを判断しております。
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
	<!--△ レポート表示 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>

<script type="text/javascript">
<!--
	$(function () {
		// ツールチップ設定
		for (var idx = 1; idx <= 25; idx++) {
			$("#help_tr" + idx).tinyTips('title');
		}

		// レポート表示エリアの高さを調
		setHeightTwoTable("tableDataFix", "tableData");
		setDivDataHeight();
		// スクロール上の左位置を設定
		setScrollLeftTwoTable("div_header_right", "div_data_right");
	});

	// ２つテーブルのスクロール上の左位置を設定
	function setScrollLeftTwoTable(tableTarget, tableReference) {
		$("." + tableReference).scroll(function () {
			$("." + tableTarget).scrollLeft($("." + tableReference).scrollLeft());
		});
	}

	// ２つテーブルの高さを同じく設定
	function setHeightTwoTable(tableTarget, tableReference) {
		var RowNums = $("." + tableTarget).find("tr").length;
		for (var i = 1; i < RowNums + 1; i++) {

			$("." + tableReference + " tr:nth-child(" + i + ")").css({ "height": "" });
			$("." + tableTarget + " tr:nth-child(" + i + ")").css({ "height": "" });

			var tableReference_height = $("." + tableReference + " tr:nth-child(" + i + ")").outerHeight();
			var tableTarget_height = $("." + tableTarget + " tr:nth-child(" + i + ")").outerHeight();

			var height = 0;

			if (tableReference_height < tableTarget_height) {
				height = tableTarget_height + 1;
			} else {
				height = tableReference_height + 1;
			}

			if (height <= 3) {
				height = 0;
			}

			$("." + tableReference + " tr:nth-child(" + i + ")").height(height);
			$("." + tableTarget + " tr:nth-child(" + i + ")").height(height);
		}
	}

	// レポートデータ表示エリアの高さを設定
	function setDivDataHeight() {
		$(".div_header_left").height($(".div_header_right").height());
		$(".div_data_left").height($(".div_data_right").height());
		setHeightTwoTable("tableDataFix", "tableData");
		$(".div_data").height($(".tableDataFix").height() + 16);
	};

	// csvを出力時、レポート情報を送信しておく
	function lbReportExport_ClientClick() {
		$("#<%= hdReportInfo.ClientID %>").val($("#tdReportInfo").html());
	}
//-->
</script>
</asp:Content>

