<%--
=========================================================================================================
  Module      : アフィリエイトレポート一覧ページ(AffiliateReportList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="AffiliateReportList.aspx.cs" Inherits="Form_AffiliateReport_AffiliateReportList" %>
<%-- マスタ出力コントロール --%>
<%@ Register TagPrefix="uc" TagName="MasterDownLoad" Src="~/Form/Common/MasterDownLoadPanel.ascx" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">アフィリエイトレポート</h1></td>
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
												<table cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="search_title_bg" width="63">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															アフィリエイト区分</td>
														<td class="search_item_bg" width="153">
															<asp:DropDownList id="ddlAffiliateKbn" runat="server"></asp:DropDownList>
														</td>
														<td class="search_title_bg" width="63">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															マスタID</td>
														<td class="search_item_bg" width="153">
															<asp:TextBox id="tbMasterId" runat="server" Width="105"></asp:TextBox>
														</td>
														<td class="search_title_bg" width="64">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															タグID</td>
														<td class="search_item_bg" width="154">
															<asp:TextBox id="tbTagId" runat="server" Width="105"></asp:TextBox>
														</td>
														<td class="search_btn_bg" width="88" rowspan="5">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_AFFILIATET_REPORT_LIST %>">クリア</a>
																<a href="javascript:Reset();">リセット</a></div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="130">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															ログ日時</td>
														<td class="search_item_bg" colspan="5">
															<div id="affiliateReportDate">
																<uc:DateTimePickerPeriodInput ID="ucAffiliateReportDatePeriod" runat="server" CanDisplayStartDateTime="true" IsNullStartDateTime="true"/>
																の間
																<span class="search_btn_sub">(<a href="Javascript:SetToday('AffiliateReport');">今日</a>｜<a href="Javascript:SetThisMonth('AffiliateReport');">今月</a>)</span>
															</div>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr><td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
										<%-- マスタ出力 --%>
										<tr>
											<td class="search_table">
												<uc:MasterDownLoad runat="server" ID="uMasterDownload" DownloadType="AffiliateCoopLog" TableWidth="758" />
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
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">アフィリエイト連携ログ一覧</h2></td>
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
														<td align="center" width="10%" style="height: 17px">ログNo</td>
														<td align="center" width="10%" style="height: 17px">ログ日時</td>
														<td align="center" width="15%" style="height: 17px">アフィリエイト区分</td>
														<td align="center" width="10%" style="height: 17px">マスタID</td>
														<td align="center" width="10%" style="height: 17px">タグID</td>
														<td align="center" width="45%" style="height: 17px">連携データ</td>
													</tr>
													<asp:Repeater id="rList" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
																<td align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_AFFILIATECOOPLOG_LOG_NO))%></td>
																<td align="center"><%#: DateTimeUtility.ToStringForManager(Eval(Constants.FIELD_AFFILIATECOOPLOG_DATE_CREATED), DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter)%>
																<td align="center"><%# WebSanitizer.HtmlEncode(CreateDisplayAffiliateKbn((string)Eval(Constants.FIELD_AFFILIATECOOPLOG_AFFILIATE_KBN)))%></td>
																<td align="center"><%# (Eval(Constants.FIELD_AFFILIATECOOPLOG_MASTER_ID).ToString() == "" ? "-" : WebSanitizer.HtmlEncode(Eval(Constants.FIELD_AFFILIATECOOPLOG_MASTER_ID)))%></td>
																<td align="center"><%# Eval(Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA11) == DBNull.Value ? "-" : WebSanitizer.HtmlEncode(Eval(Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA11))%></td>
																<td align="left"><%# CreateDisplayMessage(WebSanitizer.HtmlEncode(Eval(Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA1).ToString()), 56)%>
																	<br />
																	<a href="javascript:open_window('<%# Constants.PATH_ROOT + Constants.PAGE_MANAGER_AFFILIATET_REPORT_DISPLAY_CONTENT + '?' + Constants.REQUEST_KEY_AFFILIATET_REPORT_LOG_NO + '=' + Eval(Constants.FIELD_AFFILIATECOOPLOG_LOG_NO).ToString() %>','MallWatchingLogDisplayContent','width=828,height=585,top=120,left=320,status=NO,scrollbars=yes');">詳細を表示する</a>
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
															・エラーが表示されている場合は、システム管理者にご連絡ください。<br />
															■各項目の説明<br />
															・アフィリエイト区分 ・・・アフィリエイト区分には「リンクシェア」、「汎用アフィリエイト」があります。<br />
															・マスタID ・・・成果報告の区分が注文完了の場合は注文ID、トップページの場合はユーザーID、マイメニュー登録・解除の場合はUIDが表示されます。
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
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<script type="text/javascript">
<!--
	// 今日設定
	function SetToday(set_date) {
		// ステータス更新日
		if (set_date == 'AffiliateReport') {
			document.getElementById('<%= ucAffiliateReportDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTime.Now.ToString("yyyy/MM/dd") %>';
			document.getElementById('<%= ucAffiliateReportDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucAffiliateReportDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTime.Now.ToString("yyyy/MM/dd") %>';
			document.getElementById('<%= ucAffiliateReportDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucAffiliateReportDatePeriod.ID %>');
		}
	}

	// 今月設定
	function SetThisMonth(set_date) {
		// ステータス更新日
		if (set_date == 'AffiliateReport') {
			document.getElementById('<%= ucAffiliateReportDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTime.Now.ToString("yyyy/MM/01") %>';
			document.getElementById('<%= ucAffiliateReportDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucAffiliateReportDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTime.Now.ToString("yyyy/MM") %>' + '/' + '<%= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) %>';
			document.getElementById('<%= ucAffiliateReportDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucAffiliateReportDatePeriod.ID %>');
		}
	}

	// Form Reset
	function Reset() {
		document.getElementById('<%= ucAffiliateReportDatePeriod.HfStartDate.ClientID %>').value = '';
		document.getElementById('<%= ucAffiliateReportDatePeriod.HfStartTime.ClientID %>').value = '';
		document.getElementById('<%= ucAffiliateReportDatePeriod.HfEndDate.ClientID %>').value = '';
		document.getElementById('<%= ucAffiliateReportDatePeriod.HfEndTime.ClientID %>').value = '';
		reloadDisplayDateTimePeriod('<%= ucAffiliateReportDatePeriod.ID %>');
		this.document.<%= this.Form.ClientID %>.reset();
	}
//-->
</script>
</asp:Content>