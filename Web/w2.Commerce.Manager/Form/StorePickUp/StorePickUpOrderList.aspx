<%--
=========================================================================================================
  Module      : 店舗受取注文一覧画面(StorePickUpOrderList.aspx)
  ････････････････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Order" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="w2.Common.Web" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage_responsive.master" AutoEventWireup="true" CodeFile="StorePickUpOrderList.aspx.cs" Inherits="Form_StorePickUp_StorePickUpOrderList" %>

<%-- マスタ出力コントロール --%>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<%@ Register TagPrefix="uc" TagName="LabelOrderIdAndIconHelp" Src="~/Form/Common/LabelOrderIdAndIconHelp.ascx" %>
<asp:Content ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<link rel="stylesheet" type="text/css" href="../../Css/hide-field-button-style.css">
	<table cellspacing="0" cellpadding="0" width="791" border="0">
		<tr>
			<td><h1 class="page-title">店舗受取注文情報</h1></td>
		</tr>
		<tr>
			<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
		</tr>
		<!--▽ 検索 ▽-->
		<tr>
			<td>
				<table class="box_border cmn-section" cellspacing="1" cellpadding="3" width="784" border="0">
					<tr>
						<td class="search_box_bg">
							<table class="wide-content" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td align="center">
										<table class="wide-content" cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td class="search_table" width="758">
													<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
														<tr>
															<td class="search_title_bg" style="width: 165px">
																<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
																<uc:LabelOrderIdAndIconHelp ID="LabelOrderIdAndIconHelp1" runat="server" />
															</td>
															<td class="search_item_bg" width="130">
																<asp:TextBox ID="tbOrderId" runat="server" Width="125" />
															</td>
															<td class="search_title_bg" style="width: 165px">
																<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />注文ステータス
															</td>
															<td class="search_item_bg">
																<asp:DropDownList ID="ddlOrderStatus" runat="server" CssClass="search_item_width" />
															</td>
															<td class="search_title_bg" width="122">
																<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />並び順
															</td>
															<td class="search_item_bg">
																<asp:DropDownList ID="ddlSortKbn" runat="server" CssClass="search_item_width" />
															</td>
															<td class="search_btn_bg" width="83" rowspan="<%= CalculateSearchRowSpan() %>">
																<div class="search_btn_main">
																	<asp:Button ID="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" />
																</div>
																<div class="search_btn_sub">
																	<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_STOREPICKUP_ORDER_LIST %>">クリア</a>&nbsp;
																	<a href="javascript:Reset();">リセット</a>
																</div>
															</td>
														</tr>
														<tr>
															<td class="search_title_bg">
																<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />ユーザーID
															</td>
															<td class="search_item_bg">
																<asp:TextBox ID="tbUserId" runat="server" Width="125" />
															</td>
															<td class="search_title_bg">
																<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />注文者名
															</td>
															<td class="search_item_bg" colspan='<%= this.IsShippingCountryAvailableJp ? "1" : "3" %>'>
																<asp:TextBox ID="tbOwnerName" runat="server" Width="125" />
															</td>
															<% if (this.IsShippingCountryAvailableJp) { %>
																<td class="search_title_bg">
																	<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" /><%: ReplaceTag("@@User.name_kana.name@@") %>
																</td>
																<td class="search_item_bg">
																	<asp:TextBox ID="tbOwnerNameKana" runat="server" Width="125" />
																</td>
															<% } %>
														</tr>
														<tr>
															<td class="search_title_bg">
																<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />メールアドレス
															</td>
															<td class="search_item_bg">
																<asp:TextBox ID="tbOwnerMailAddr" runat="server" Width="125" />
															</td>
															<td class="search_title_bg">
																<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />商品ID
															</td>
															<td class="search_item_bg">
																<asp:TextBox ID="tbProductId" runat="server" Width="125" />
															</td>
															<td class="search_title_bg">
																<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />商品名
															</td>
															<td class="search_item_bg">
																<asp:TextBox ID="tbProductName" runat="server" Width="125" />
															</td>
														</tr>
														<tr>
															<td class="search_title_bg" width="130">
																<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />ステータス更新日
															</td>
															<td class="search_item_bg" colspan="3">
																<asp:DropDownList ID="ddlOrderUpdateDateStatus" runat="server" CssClass="search_item_width" />
																<div id="orderUpdate" style="display: inline-block">
																	<uc:DateTimePickerPeriodInput ID="ucOrderUpdateDatePeriod" runat="server" IsNullStartDateTime="true" IsHideTime="true" />
																	の間
																	<span class="search_btn_sub">(<a href="Javascript:SetYesterday('order');">昨日</a>｜<a href="Javascript:SetToday('order');">今日</a>｜<a href="Javascript:SetThisMonth('order');">今月</a>)</span>
																</div>
															</td>
															<td class="search_title_bg">
																<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />店舗受取ステータス
															</td>
															<td class="search_item_bg">
																<asp:DropDownList ID="ddlStorepickupStatus" runat="server" CssClass="search_item_width" />
															</td>
														</tr>
														<tr>
															<td class="search_title_bg">
																<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />注文者電話番号
															</td>
															<td class="search_item_bg" colspan='<%= (this.OperatorAuthoritys == null) ? "1" : "5" %>'>
																<asp:TextBox ID="tbOwnerTel" runat="server" Width="125" />
															</td>
															<% if (this.OperatorAuthoritys == null) { %>
																<td class="search_title_bg">
																	<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />受取店舗ID
																</td>
																<td class="search_item_bg">
																	<asp:TextBox ID="tbRealShopId" runat="server" Width="125" />
																</td>
																<td class="search_title_bg">
																	<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />受取店舗
																</td>
																<td class="search_item_bg">
																	<asp:TextBox ID="tbRealShopName" runat="server" Width="125" />
																</td>
															<% } %>
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
				</table>
			</td>
		</tr>
		<!--△ 検索 △-->
		<tr>
			<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
		</tr>
		<!--▽ 一覧 ▽-->
		<tr id="orderListSearchResult">
			<td><h2 class="cmn-hed-h2">店舗受取注文情報一覧</h2></td>
		</tr>
		<tr>
			<td align="left">
				<!--▽ ページング ▽-->
				<table class="list_pager" cellspacing="0" cellpadding="0" border="0">
					<tr>
						<td style="height: 22px; width: 70%;">
							<asp:Label ID="lbPager1" runat="server" />
						</td>
						<td class="action_list_sp" style="height: 22px;">
							<%if (Constants.MARKETINGPLANNER_TARGETLIST_OPTION_ENABLE) {%>
							<% btnImportTargetList.OnClientClick = "javascript:open_window('" + ImportTargetListUrlCreator.Create(Constants.KBN_WINDOW_POPUP) + "','Import','width=850,height=370,top=120,left=420,status=no,scrollbars=yes');return false;"; %>
							<asp:Button
								ID="btnImportTargetList"
								runat="server"
								Text="  ターゲットリスト作成  "
								Enabled="false"
								EnableViewState="False"
								UseSubmitBehavior="False"
								CssClass="cmn-btn-sub-action" />
							<%} %>
						</td>
					</tr>
				</table>
				<!-- ページング-->
			</td>
		</tr>
		<tr>
			<td>
				<table class="box_border cmn-section" cellspacing="1" cellpadding="0" width="784" border="0">
					<tr>
						<td>
							<table class="list_box_bg table_store_pickup_order_list" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td align="center">
										<div id="divOrderList" runat="server">
											<table cellspacing="0" cellpadding="0" border="0" class="wide-content table_store_pickup_order_list">
												<tr>
													<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
												</tr>
												<div>
													<tr>
														<td>
															<div>
																<% if (rList.Items.Count > 0) { %>
																<%-- ▽ テーブルヘッダ ▽ --%>
																<div>
																	<table class="table_store_pickup_order_list" border="0" cellpadding="0" cellspacing="0">
																		<tr>
																			<td>
																				<!-- ▽ 固定ヘッダ ▽ -->
																				<div class="div_header_left">
																					<table class="list_table tableHeaderFixed table_store_pickup_order_list" cellspacing="1" cellpadding="3" border="0" style="height: 100%">
																						<tr class="list_title_bg">
																							<td align="center">注文ID</td>
																						</tr>
																					</table>
																				</div>
																				<!-- △ 固定ヘッダ △ -->
																			</td>
																			<td>
																				<!-- ▽ ヘッダ ▽ -->
																				<div class="div_header_right">
																					<table class="list_table tableHeader table_store_pickup_order_list"
																						cellspacing="1"
																						cellpadding="3"
																						width="<%: this.ColumnDisplaySettings.Where(x => x.CanDisplay).Sum(x => x.ColmunWidth) %>"
																						border="0"
																						style="height: 103px">
																						<!-- 水平ヘッダ -->
																						<tr class="list_title_bg">
																							<asp:Repeater ID="rManagerListDispSetting" ItemType="w2.Domain.ManagerListDispSetting.ManagerListDispSettingModel" runat="server">
																								<ItemTemplate>
																									<td align="center"
																										width="<%#: Item.ColmunWidth %>"
																										style='<%#: "min-width: " + Item.ColmunWidth + "px" %>'
																										visible="<%# (((Item == null) == false) && Item.CanDisplay && Item.IsNotFixedColmun && IsOptionCooperation(Item.DispColmunName)) %>"
																										runat="server">
																										<%# HtmlSanitizer.HtmlEncodeChangeToBr(ConvertDispColumnNameFormat(Item.DispColmunName)) %>
																									</td>
																								</ItemTemplate>
																							</asp:Repeater>
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
																<div class="div_data" style="max-height: 420px; height: <%= rList.Items.Count * 50 + 20 %>px;">
																	<%-- ▽ 固定データ ▽ --%>
																	<div class="div_data_left">
																		<table
																			class="list_table tableDataFix table_store_pickup_order_list"
																			cellspacing="1"
																			cellpadding="3"
																			border="0"
																			style="max-height: 420px; height: <%= rList.Items.Count * 50 + 20 %>px;">
																			<!-- 垂直ヘッダ -->
																			<asp:Repeater ID="rTableFixColumn" runat="server">
																				<ItemTemplate>
																					<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>"
																						onmouseover="listselect_mover(this)"
																						onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)"
																						onmousedown="listselect_mdown(this)"
																						onclick="listselect_mclick(this, '<%#: StorePickUpOrderDetailUrl((String)Eval(Constants.FIELD_ORDER_ORDER_ID)) %>')"
																						style="height: <%#: Eval(Constants.FIELD_ORDER_ORDER_ID).ToString().Length >= 20 ? "47px" : "33px" %>">
																						<td align="left" width="110">
																							<%#: Eval(Constants.FIELD_ORDER_ORDER_ID) %>
																						</td>
																					</tr>
																				</ItemTemplate>
																			</asp:Repeater>
																		</table>
																	</div>
																	<%-- △ 固定データ △ --%>

																	<%-- ▽ データ ▽ --%>
																	<div class="div_data_right">
																		<table class="list_table tableData table_store_pickup_order_list"
																			cellspacing="1"
																			cellpadding="3"
																			width="<%= this.ColumnDisplaySettings.Where(x => x.CanDisplay).Sum(x => x.ColmunWidth) %>"
																			border="0"
																			style="max-height: 420px; height: <%= rList.Items.Count * 50 +20 %>px;">
																			<asp:Repeater ID="rList" runat="server">
																				<ItemTemplate>
																					<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>"
																						onmouseover="listselect_mover(this)"
																						onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)"
																						onmousedown="listselect_mdown(this)"
																						onclick="listselect_mclick(this, '<%#: StorePickUpOrderDetailUrl((String)Eval(Constants.FIELD_ORDER_ORDER_ID)) %>')"
																						style="height: <%#: Eval(Constants.FIELD_ORDER_ORDER_ID).ToString().Length >= 20 ? "49px" : "33px" %>">
																						<%--表示設定データの取得--%>
																						<asp:Repeater DataSource="<%# this.ColumnDisplaySettings %>" ItemType="w2.Domain.ManagerListDispSetting.ManagerListDispSettingModel" runat="server">
																							<ItemTemplate>
																								<%--表示データ--%>
																								<td align="<%#: Item.ColmunAlign %>"
																									width="<%#: Item.ColmunWidth %>"
																									runat="server"
																									style='<%#: "min-width: " + Item.ColmunWidth + "px" %>'
																									visible="<%# (((Item == null) == false) && Item.CanDisplay && Item.IsNotFixedColmun && IsOptionCooperation(Item.DispColmunName)) %>">
																									<%#: ConvertItemFormatForDisplayList(Item.DispColmunName, ((DataView)rList.DataSource)[((RepeaterItem)Container.Parent.Parent).ItemIndex][Item.DispColmunName], ((DataView)rList.DataSource)[((RepeaterItem)Container.Parent.Parent).ItemIndex][Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID].ToString()) %>
																								</td>
																							</ItemTemplate>
																						</asp:Repeater>
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
																<table class="list_table table_store_pickup_order_list" cellspacing="1" cellpadding="3" width="734" border="0" style="height: 100%">
																	<!-- 水平ヘッダ -->
																	<tr id="trListError" class="list_alert" runat="server" visible="false">
																		<td id="tdErrorMessage" colspan="31" runat="server"></td>
																	</tr>
																</table>
																<%-- △ エラーの場合 △ --%>
																<% } %>
															</div>
														</td>
													</tr>
												</div>
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
		<tr>
			<td>
				<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
					<tr class="info_item_bg">
						<td align="left">備考<br />
							<span class="info_item_bg" style="max-width:90%;">
							・氏名の後ろに「<%= Constants.USERSYMBOL_REPEATER %>」付のユーザーは注文を２回以上しているリピーターユーザー、「<%= Constants.USERSYMBOL_HAS_NOTE %>」付のユーザーは特記事項のある特記ユーザーを表します。<br />
							<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
								・定期購入商品を『含む』とした場合、「定期購入商品のみ」または「通常・定期商品が混在」注文を対象に絞り込みます。<br />
							<%} %>
							・ユーザー管理レベルを検索する際に、「除外する」にチェックを入れて検索すると、選択した管理レベル以外のユーザーを一覧に表示できます。
							</span>
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
			// ステータス更新日
			if (set_date == 'order') {
				document.getElementById('<%= ucOrderUpdateDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.Date.AddDays(-1)) %>';
				document.getElementById('<%= ucOrderUpdateDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
				document.getElementById('<%= ucOrderUpdateDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.Date.AddDays(-1)) %>';
				document.getElementById('<%= ucOrderUpdateDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
				reloadDisplayDateTimePeriod('<%= ucOrderUpdateDatePeriod.ClientID %>');
			}
		}

		// 今日設定
		function SetToday(set_date) {
			// ステータス更新日
			if (set_date == 'order') {
				document.getElementById('<%= ucOrderUpdateDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
				document.getElementById('<%= ucOrderUpdateDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
				document.getElementById('<%= ucOrderUpdateDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
				document.getElementById('<%= ucOrderUpdateDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
				reloadDisplayDateTimePeriod('<%= ucOrderUpdateDatePeriod.ClientID %>');
			}
		}

		// 今月設定
		function SetThisMonth(set_date) {
			// ステータス更新日
			if (set_date == 'order') {
				document.getElementById('<%= ucOrderUpdateDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetStartDateThisMonthString() %>';
				document.getElementById('<%= ucOrderUpdateDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
				document.getElementById('<%= ucOrderUpdateDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetEndDateThisMonthString() %>';
				document.getElementById('<%= ucOrderUpdateDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
				reloadDisplayDateTimePeriod('<%= ucOrderUpdateDatePeriod.ClientID %>');
			}
		}

		// Form Reset
		function Reset() {
			document.getElementById('<%= this.Form.ClientID %>').reset();

			document.getElementById('<%= ucOrderUpdateDatePeriod.HfStartDate.ClientID %>').value = '<%= string.Empty %>';
			document.getElementById('<%= ucOrderUpdateDatePeriod.HfStartTime.ClientID %>').value = '<%= string.Empty %>';
			document.getElementById('<%= ucOrderUpdateDatePeriod.HfEndDate.ClientID %>').value = '<%= string.Empty %>';
			document.getElementById('<%= ucOrderUpdateDatePeriod.HfEndTime.ClientID %>').value = '<%= string.Empty %>';
			reloadDisplayDateTimePeriod('<%= ucOrderUpdateDatePeriod.ClientID %>');
		}
	</script>

	<script type="text/javascript">
		$(function () {
			$(".tableHeader").css("table-layout", "auto");
			$(".tableData").css("table-layout", "fixed");

			$(".tableData").find("td").each(function () {
				$(this).attr("title", $(this).text()).css("overflow", "hidden");
			});

			setWidthTwoTable("tableData", "tableHeader");
			setHeightTwoTable("tableHeaderFixed", "tableHeader");

			$(".div_data_left").height($(".div_data_right").outerHeight());
			setHeightTwoTable("tableDataFix", "tableData");
			setHeightTwoTable("tableData", "tableDataFix"); // ２つテーブルを同じ高さを設定するため
			setWidthTwoTable("tableData", "tableHeader");
			setWidthTwoTable("tableHeader", "tableData");
			$(".div_header_left").height($(".div_header_right").outerHeight());

			scrollLeftTwoTable("div_header_right", "div_data_right");
			scrollTopTwoTable("div_data_left", "div_data_right");

			hoverTwoTable("tableDataFix", "tableData");
			hoverTwoTable("tableData", "tableDataFix");

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
		});
	</script>
</asp:Content>
