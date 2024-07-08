<%--
=========================================================================================================
  Module      : 会員ランク更新履歴一覧ページ(UserMemberRankHistoryList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="UserMemberRankHistoryList.aspx.cs" Inherits="Form_UserMemberRank_UserMemberRankHistoryList" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">会員ランク更新履歴</h1></td>
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
															ユーザーID</td>
														<td class="search_item_bg" width="230" colspan="3">
															<asp:TextBox id="tbUserId" runat="server" size="40"></asp:TextBox>
														</td>
														<td class="search_btn_bg" width="88" rowspan="5">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_USER_MEMBER_RANK_HISTORY_LIST %>">クリア</a>
																<a href="javascript:Reset()">リセット</a></div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															更新前ランク</td>
														<td class="search_item_bg">
															<asp:DropDownList id="ddlBeforeRankId" runat="server"></asp:DropDownList></td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															更新後ランク</td>
														<td class="search_item_bg">
															<asp:DropDownList id="ddlAfterRankId" runat="server"></asp:DropDownList></td>
													</tr>
													<tr>
														<td class="search_title_bg" width="130">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															更新日時</td>
														<td class="search_item_bg" colspan="3">
															<div id="updateDate">
																<uc:DateTimePickerPeriodInput ID="ucUpdateDate" runat="server" IsNullStartDateTime="true"/>
																<span class="search_btn_sub">(<a href="Javascript:SetToday('UserMemberRankHis');">今日</a>｜<a href="Javascript:SetThisMonth('UserMemberRankHis');">今月</a>)</span>
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
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">会員ランク更新履歴一覧</h2></td>
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
										<% if (this.IsNotSearchDefault) { %>
											<tr>
												<td>
													<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr class="list_alert">
															<td align="center"><%= WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NOT_SEARCH_DEFAULT) %></td>
														</tr>
													</table>
													<br />
												</td>
											</tr>
										<% } else { %>
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
														<td align="center" width="20%" style="height: 17px">履歴No</td>
														<td align="center" width="20%" style="height: 17px">更新日時</td>
														<td align="center" width="20%" style="height: 17px">ユーザーID</td>
														<td align="center" width="20%" style="height: 17px">更新前ランク</td>
														<td align="center" width="20%" style="height: 17px">更新後ランク</td>
													</tr>
													<asp:Repeater id="rList" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
																<td align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USERMEMBERRANKHISTORY_HISTORY_NO))%></td>
																<td align="center"><%#: DateTimeUtility.ToStringForManager(Eval(Constants.FIELD_USERMEMBERRANKHISTORY_DATE_CREATED), DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter)%></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USERMEMBERRANKHISTORY_USER_ID))%></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(Eval("before_rank_name"))%></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(Eval("after_rank_name"))%></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="5"></td>
													</tr>
												</table>
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
												</table>
											</td>
										</tr>
										<% } %>
										<tr>
											<td>
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="info_item_bg">
														<td align="left">備考<br />
															・エラーが表示されている場合は、システム管理者にご連絡ください。<br />
															・現在会員ランク情報に設定されていない会員ランクは、更新前、更新後ランクに表示されません。
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
		if (set_date == 'UserMemberRankHis') {
			document.getElementById('<%= ucUpdateDate.HfStartDate.ClientID %>').value = '<%= DateTime.Now.ToString("yyyy/MM/dd") %>';
			document.getElementById('<%= ucUpdateDate.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucUpdateDate.HfEndDate.ClientID %>').value = '<%= DateTime.Now.ToString("yyyy/MM/dd") %>';
			document.getElementById('<%= ucUpdateDate.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucUpdateDate.ID %>');
		}
	}

	// 今月設定
	function SetThisMonth(set_date) {
		// ステータス更新日
		if (set_date == 'UserMemberRankHis') {
			document.getElementById('<%= ucUpdateDate.HfStartDate.ClientID %>').value = '<%= DateTime.Now.ToString("yyyy/MM/01") %>';
			document.getElementById('<%= ucUpdateDate.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucUpdateDate.HfEndDate.ClientID %>').value = '<%= DateTime.Now.ToString("yyyy/MM") %>' + '/' + '<%= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) %>';
			document.getElementById('<%= ucUpdateDate.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucUpdateDate.ID %>');
		}
	}

	function Reset() {
		document.getElementById('<%= ucUpdateDate.HfStartDate.ClientID %>').value = '';
		document.getElementById('<%= ucUpdateDate.HfStartTime.ClientID %>').value = '';
		document.getElementById('<%= ucUpdateDate.HfEndDate.ClientID %>').value = '';
		document.getElementById('<%= ucUpdateDate.HfEndTime.ClientID %>').value = '';
		reloadDisplayDateTimePeriod('<%= ucUpdateDate.ID %>');
		this.document.<%= this.Form.ClientID %>.reset();
	}
	//-->
</script>
</asp:Content>