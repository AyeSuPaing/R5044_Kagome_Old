<%--
=========================================================================================================
  Module      : バッチ監視ページ(MallWatchingLogList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MallWatchingLogList.aspx.cs" Inherits="Form_MallWatchingLog_MallWatchingLogList" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td>
			<h1 class="page-title">モール連携監視</h1>
		</td>
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
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															ログ区分</td>
														<td class="search_item_bg" width="230">
															<asp:CheckBoxList id="cblLogKbn" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList>
														</td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															モールID</td>
														<td class="search_item_bg" width="230">
															<asp:TextBox id="tbMallId" runat="server" Width="105"></asp:TextBox>
														</td>
														<td class="search_btn_bg" width="88" rowspan="5">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="MallWatchingLogSearch_Click" /></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_MALL_WATCHING_LOG_LIST %>">クリア</a>
																<a href="javascript:Reset();">リセット</a></div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															処理名</td>
														<td class="search_item_bg" colspan="3">
															<asp:CheckBoxList class="no-border" id="cblBatchList" runat="server" RepeatDirection="Horizontal" RepeatColumns="5" Width="100%" RepeatLayout="Table"></asp:CheckBoxList>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="130">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															ログ日時</td>
														<td class="search_item_bg" colspan="3">
															<uc:DateTimePickerPeriodInput id="ucMallWatchingLogDatePeriod" runat="server" IsNullStartDateTime="true" />
															の間
															<span class="search_btn_sub">(<a href="Javascript:SetToday('MallWatchingLog');">今日</a>｜<a href="Javascript:SetThisMonth('MallWatchingLog');">今月</a>)</span>
														</td>
													</tr>
													<%-- ▼ Hack: パフォーマンスの問題によりログメッセージ検索を使用不可とする。 ▼ --%>
													<!-- <tr>
														<td class="search_title_bg" width="95" rowspan>
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															ログメッセージ</td>
														<td class="search_item_bg" width="230" colspan="3">
															<asp:TextBox id="tbLogMessage" runat="server" size="80"></asp:TextBox></td>
													</tr> -->
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
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">ログ一覧</h2></td>
	</tr>
	<tr>
		<td style="width: 792px">
			<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<asp:UpdatePanel runat="server">
									<ContentTemplate>
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
													</tr>
												</table>
												<!-- ページング-->
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr><td align="center">
											<div runat="server" ID="dvLoadingImg">
												<img alt="" src="../../Images/Common/loading.gif" width="20" height="20" border="0" />
												<asp:Literal runat="server" Text="現在監視ログ一覧を取得中です.." /><br/>
											</div>
										</td></tr>
										<tr><td align="center"><asp:Literal id="lProcessMessage" runat="server" /></td></tr>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="list_title_bg">
														<td align="center" width="8%" style="height: 17px">ログNo</td>
														<td align="center" width="10%" style="height: 17px">ログ日時</td>
														<td align="center" width="8%" style="height: 17px">ログ区分</td>
														<td align="center" width="17%" style="height: 17px">処理名</td>
														<td align="center" width="10%" style="height: 17px">モールID</td>
														<td align="center" width="47%" style="height: 17px">ログメッセージ</td>
													</tr>
													<asp:Repeater id="rList" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg_<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_MALLWATCHINGLOG_LOG_KBN).ToString().ToLower()) %>" >
																<td align="center" width="8%"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_MALLWATCHINGLOG_LOG_NO))%></td>
																<td align="center" width="10%"><%#: DateTimeUtility.ToStringForManager(Eval(Constants.FIELD_MALLWATCHINGLOG_WATCHING_DATE).ToString() + " " + Eval(Constants.FIELD_MALLWATCHINGLOG_WATCHING_TIME).ToString(), DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %>
																<td align="center" width="8%"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_MALLWATCHINGLOG, Constants.FIELD_MALLWATCHINGLOG_LOG_KBN, Eval(Constants.FIELD_MALLWATCHINGLOG_LOG_KBN)))%></td>
																<td align="center" width="17%"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_MALLWATCHINGLOG, Constants.FIELD_MALLWATCHINGLOG_BATCH_ID,Eval(Constants.FIELD_MALLWATCHINGLOG_BATCH_ID)))%></td>
																<td align="center" width="10%"><%# (Eval(Constants.FIELD_MALLWATCHINGLOG_MALL_ID).ToString() == "" ? "-" : WebSanitizer.HtmlEncode(Eval(Constants.FIELD_MALLWATCHINGLOG_MALL_ID)))%></td>
																<td align="left" width="47%"><%# CreateDisplayMessage(WebSanitizer.HtmlEncode(Eval(Constants.FIELD_MALLWATCHINGLOG_LOG_MESSAGE).ToString()), 56)%>
																	<br />
																	<a href="javascript:open_window('<%# Constants.PATH_ROOT + Constants.PAGE_MANAGER_MALL_WATCHING_LOG_DISPLAY_CONTENT + '?' + Constants.REQUEST_KEY_MALLWATCHINGLOG_LOG_NO + '=' + Eval(Constants.FIELD_MALLWATCHINGLOG_LOG_NO).ToString() %>','MallWatchingLogDisplayContent','width=850,height=585,top=120,left=320,status=NO,scrollbars=yes');">詳細を表示する</a>
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
															エラーが表示されている場合は、システム管理者にご連絡ください。<br />
															機能によって、モールIDが表示されない場合があります。
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
									</table>
									</ContentTemplate>
									<Triggers>
										<asp:AsyncPostBackTrigger ControlID="tProcessTimer" EventName="Tick" />
									</Triggers>
									</asp:UpdatePanel>
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
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<script type="text/javascript">
<!--
// 今日設定
function SetToday(set_date)
{
	// ステータス更新日
	if (set_date == 'MallWatchingLog')
	{
		document.getElementById('<%= ucMallWatchingLogDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTime.Now.ToString("yyyy/MM/dd") %>';
		document.getElementById('<%= ucMallWatchingLogDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
		document.getElementById('<%= ucMallWatchingLogDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTime.Now.ToString("yyyy/MM/dd") %>';
		document.getElementById('<%= ucMallWatchingLogDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
		reloadDisplayDateTimePeriod('<%= ucMallWatchingLogDatePeriod.ClientID %>');
	}
}

// 今月設定
function SetThisMonth(set_date)
{
	// ステータス更新日
	if (set_date == 'MallWatchingLog')
	{
		document.getElementById('<%= ucMallWatchingLogDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTime.Now.ToString("yyyy/MM/01") %>';
		document.getElementById('<%= ucMallWatchingLogDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
		document.getElementById('<%= ucMallWatchingLogDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTime.Now.ToString("yyyy/MM") %>' + '/' + '<%= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) %>';
		document.getElementById('<%= ucMallWatchingLogDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
		reloadDisplayDateTimePeriod('<%= ucMallWatchingLogDatePeriod.ClientID %>');
	}
}

// Form Reset
function Reset()
{
	document.getElementById('<%= ucMallWatchingLogDatePeriod.HfStartDate.ClientID %>').value = '';
	document.getElementById('<%= ucMallWatchingLogDatePeriod.HfStartTime.ClientID %>').value = '';
	document.getElementById('<%= ucMallWatchingLogDatePeriod.HfEndDate.ClientID %>').value = '';
	document.getElementById('<%= ucMallWatchingLogDatePeriod.HfEndTime.ClientID %>').value = '';
	reloadDisplayDateTimePeriod('<%= ucMallWatchingLogDatePeriod.ClientID %>');
	this.document.<%= this.Form.ClientID %>.reset();
}
//-->
</script>
<asp:Timer id="tProcessTimer" Interval="1000" OnTick="tProcessTimer_Tick" Enabled="False" runat="server"/>
</asp:Content>