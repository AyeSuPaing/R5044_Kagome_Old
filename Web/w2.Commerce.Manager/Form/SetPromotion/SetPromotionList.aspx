<%--
=========================================================================================================
  Module      : セットプロモーション設定一覧ページ(SetPromotionList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="SetPromotionList.aspx.cs" Inherits="Form_SetPromotion_SetPromotionList" %>
<%@ Import Namespace="w2.Domain.SetPromotion" %>
<%@ Import Namespace="w2.Domain.SetPromotion.Helper" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0" runat="server">
	<tr>
		<td><h1 class="page-title">セットプロモーション設定</h1></td>
	</tr>
	<tr>
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 検索 ▽-->
	<tr>
		<td style="width: 792px">
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
														<td class="search_title_bg" width="110">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															セットプロモーションID</td>
														<td class="search_item_bg" width="120">
															<asp:TextBox id="tbSetPromotionId" runat="server" Width="120"></asp:TextBox>
														</td>
														<td class="search_title_bg" width="110">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															セットプロモーション名</td>
														<td class="search_item_bg" width="120">
															<asp:TextBox id="tbSetPromotionName" runat="server" Width="120"></asp:TextBox>
														</td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															並び順</td>
														<td class="search_item_bg" width="120">
															<asp:DropDownList id="ddlSortKbn" runat="server" Width="120" Font-Size="9"></asp:DropDownList>
														</td>
														<td class="search_btn_bg" width="83" rowspan="4">
															<div class="search_btn_main"><asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_SETPROMOTION_LIST %>">クリア</a>
																<a href="javascript:Reset();">リセット</a><br />
																<% if (Constants.GLOBAL_OPTION_ENABLE){ %>
																<asp:LinkButton id="lbExportTranslationData" Runat="server" OnClick="lbExportTranslationData_Click">翻訳設定出力</asp:LinkButton>
																<% } %>
															</div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															商品ID
														</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbProductId" runat="server" Width="120"></asp:TextBox>
														</td>
														<td id="tdCategoryId" visible="false" runat="server" class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															カテゴリID
														</td>
														<td id="tdCategoryIdTextBox" visible="false" runat="server" class="search_item_bg" width="130">
															<asp:TextBox id="tbCategoryId" runat="server" Width="120"></asp:TextBox>
														</td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															開催状態
														</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList ID="ddlStatus" runat="server"></asp:DropDownList>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															開始日時
														</td>
														<td class="search_item_bg" width="130" colspan="5">
															<div id="beginDate">
																<uc:DateTimePickerPeriodInput id="ucBeginDatePeriod" runat="server" IsNullStartDateTime="true"/>
																<span class="search_btn_sub">(<a href="Javascript:SetToday('begin_date');">今日</a>｜<a href="Javascript:SetThisMonth('begin_date');">今月</a>)</span>
															</div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															終了日時
														</td>
														<td class="search_item_bg" width="130" colspan="5">
															<div id="endDate">
																<uc:DateTimePickerPeriodInput id="ucEndDatePeriod" runat="server" IsNullStartDateTime="true"/>
																<span class="search_btn_sub">(<a href="Javascript:SetToday('end_date');">今日</a>｜<a href="Javascript:SetThisMonth('end_date');">今月</a>)</span>
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
			</table>
		</td>
	</tr>
	<!--△ 検索 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">セットプロモーション設定一覧</h2></td>
	</tr>
	<tr>
		<td style="width: 792px">
			<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<!--▽ ページング ▽-->
												<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
													<tr>
														<td width="675" style="height: 22px"><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
														<td class="action_list_sp"><asp:Button id="btnInsertTop" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" /></td>
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
												<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="list_title_bg">
														<td align="center" width="120">セットプロモーションID</td>
														<td align="center" width="258">セットプロモーション名(管理用)</td>
														<td align="center" width="120">開始日時</td>
														<td align="center" width="120">終了日時</td>
														<td align="center" width="70">有効フラグ</td>
														<td align="center" width="70">開催状態</td>
													</tr>
													<asp:Repeater id="rList" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateDetailUrl(((SetPromotionListSearchResult)Container.DataItem).SetpromotionId, Constants.ACTION_STATUS_UPDATE)) %>')">
																<td align="center"><%# WebSanitizer.HtmlEncode(((SetPromotionListSearchResult)Container.DataItem).SetpromotionId) %></td>
																<td align="left"><%# WebSanitizer.HtmlEncode(((SetPromotionListSearchResult)Container.DataItem).SetpromotionName) %></td>
																<td align="center"><%#: DateTimeUtility.ToStringForManager(((SetPromotionListSearchResult)Container.DataItem).BeginDate, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
																<td align="center"><%#: DateTimeUtility.ToStringForManager(((SetPromotionListSearchResult)Container.DataItem).EndDate, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter, "指定なし") %></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_SETPROMOTION, Constants.FIELD_SETPROMOTION_VALID_FLG, ((SetPromotionListSearchResult)Container.DataItem).ValidFlg))%></td>
																<td align="center">
																	<span visible='<%# (((SetPromotionListSearchResult)Container.DataItem).Status == SetPromotionModel.StatusType.Preparing) || (((SetPromotionListSearchResult)Container.DataItem).Status == SetPromotionModel.StatusType.Suspended) %>' runat="server">
																		<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_SETPROMOTION, FIELD_SETPROMOTION_STATUS, ((SetPromotionListSearchResult)Container.DataItem).Status)) %>
																	</span>
																	<span visible='<%# (((SetPromotionListSearchResult)Container.DataItem).Status == SetPromotionModel.StatusType.OnGoing) %>' runat="server">
																		<strong><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_SETPROMOTION, FIELD_SETPROMOTION_STATUS, ((SetPromotionListSearchResult)Container.DataItem).Status)) %></strong>
																	</span>
																	<span visible='<%#(((SetPromotionListSearchResult)Container.DataItem).Status == SetPromotionModel.StatusType.Finished) %>' runat="server">
																		<span disabled="true" runat="server"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_SETPROMOTION, FIELD_SETPROMOTION_STATUS, ((SetPromotionListSearchResult)Container.DataItem).Status)) %></span>
																	</span>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="6"></td>
													</tr>
												</table>
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
												</table>
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="info_item_bg">
														<td align="left">備考<br />
															・セットプロモーション名の検索は、管理用・サイト表示用すべてが検索対象となります。<br />
															・開催状態について<br />
															&nbsp;&nbsp;準備中：期間開始前のもの<br />
															&nbsp;&nbsp;開催中：期間中かつ有効のもの<br />
															&nbsp;&nbsp;一時停止：期間中かつ無効のもの<br />
															&nbsp;&nbsp;終了：期間終了後のもの
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td class="action_list_sp"><asp:Button id="btnInsertBottom" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" /></td>
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
<!--
	// 今日設定
	function SetToday(set_date) {
		// 開始日時
		if (set_date == 'begin_date') {
			document.getElementById('<%= ucBeginDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
			document.getElementById('<%= ucBeginDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucBeginDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
			document.getElementById('<%= ucBeginDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucBeginDatePeriod.ClientID %>');
		}
		// 終了日時
		else if (set_date == 'end_date') {
			document.getElementById('<%= ucEndDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
			document.getElementById('<%= ucEndDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucEndDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
			document.getElementById('<%= ucEndDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucEndDatePeriod.ClientID %>');
		}
	}

	// 今月設定
	function SetThisMonth(set_date) {
		// 開始日時
		if (set_date == 'begin_date') {
			document.getElementById('<%= ucBeginDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetStartDateThisMonthString() %>';
			document.getElementById('<%= ucBeginDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucBeginDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetEndDateThisMonthString() %>';
			document.getElementById('<%= ucBeginDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucBeginDatePeriod.ClientID %>');
		}
		// 終了日時
		else if (set_date == 'end_date') {
			document.getElementById('<%= ucEndDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetStartDateThisMonthString() %>';
			document.getElementById('<%= ucEndDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucEndDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetEndDateThisMonthString() %>';
			document.getElementById('<%= ucEndDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucEndDatePeriod.ClientID %>');
		}
	}

	// Reset
	function Reset() {
		document.getElementById('<%= ucBeginDatePeriod.HfStartDate.ClientID %>').value = '';
		document.getElementById('<%= ucBeginDatePeriod.HfStartTime.ClientID %>').value = '';
		document.getElementById('<%= ucBeginDatePeriod.HfEndDate.ClientID %>').value = '';
		document.getElementById('<%= ucBeginDatePeriod.HfEndTime.ClientID %>').value = '';
		reloadDisplayDateTimePeriod('<%= ucBeginDatePeriod.ClientID %>');

		document.getElementById('<%= ucEndDatePeriod.HfStartDate.ClientID %>').value = '';
		document.getElementById('<%= ucEndDatePeriod.HfStartTime.ClientID %>').value = '';
		document.getElementById('<%= ucEndDatePeriod.HfEndDate.ClientID %>').value = '';
		document.getElementById('<%= ucEndDatePeriod.HfEndTime.ClientID %>').value = '';
		reloadDisplayDateTimePeriod('<%= ucEndDatePeriod.ClientID %>');
		document.getElementById('<%= this.Form.ClientID %>').reset();
	}
//-->
</script>
</asp:Content>