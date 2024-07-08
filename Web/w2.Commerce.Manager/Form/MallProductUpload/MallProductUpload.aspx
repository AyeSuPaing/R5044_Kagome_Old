<%--
=========================================================================================================
  Module      : モール連携 商品アップロードページ(MallProductUpload.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MallProductUpload.aspx.cs" Inherits="Form_MallProductUpload_MallProductUpload" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td>
			<h1 class="page-title">モール商品アップロード</h1>
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
															商品ID</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbProductId" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															商品名</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbName" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_btn_bg" width="83" rowspan="5">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="ProductSearch" /></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_MALL_PRODUCT_UPLOAD %>">クリア</a>
																<a href="javascript:Reset();">リセット</a></div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															登録対象日付</td>
														<td class="search_item_bg" colspan="3">
															<uc:DateTimePickerPeriodInput id="ucProductInsertDatePeriod" runat="server" IsNullStartDateTime="true" />
															<span class="search_btn_sub">(<a href="Javascript:SetToday('Insert');">今日</a>｜<a href="Javascript:SetThisMonth('Insert');">今月</a>)</span>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															更新対象日付</td>
														<td class="search_item_bg" colspan="3">
															<uc:DateTimePickerPeriodInput id="ucProductUpdateDatePeriod" runat="server" IsNullStartDateTime="true" />
															<span class="search_btn_sub">(<a href="Javascript:SetToday('Update');">今日</a>｜<a href="Javascript:SetThisMonth('Update');">今月</a>)</span>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															有効フラグ</td>
														<td class="search_item_bg" colspan="3">
															<asp:CheckBox ID="cbValidFlg" Checked="true" Text="有効な商品のみ" runat="server" />
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
	<tbody id="tbdyList" visible="false" runat="server">
	<tr>
		<td><h2 class="cmn-hed-h2">アップロード対象一覧</h2></td>
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
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="info_title_bg">
														<td align="left" colspan="2" width="90%">登録件数</td>
														<td align="left" rowspan="3">
															<asp:Button ID="btnSendInsertData" Text="  送信  " runat="server" OnClientClick='return InsertConfirm();' OnClick="btnSendInsertData_Click" />
															<div style="display: none;">
																<asp:Button ID="btnSendInsertDataDummy" Text="  送信  " runat="server" OnClick="btnSendInsertData_Click" />
															</div>
														</td>
													</tr>
													<tr class="info_item_bg">
														<td align="left" width="20%">商品マスタ</td>
														<td align="left"><asp:Label ID="lProductInsert" runat="server"></asp:Label> 件&nbsp;(<a href="javascript:open_window('<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MALL_PRODUCT_UPLOAD_PRODUCT_LIST + "?" + Constants.REQUEST_KEY_MALLPRODUCTUPLOAD_DIPLAY_KBN + "=" + Constants.KBN_MALLPRODUCTUPLOAD_DIPLAY_KBN_INSERT) %>','product_list','width=850,height=500,top=120,left=500,status=NO,scrollbars=yes');">一覧</a>)</td>
													</tr>
													<tr class="info_item_bg">
														<td align="left" width="20%">モール出品先</td>
														<td align="left"><asp:CheckBoxList id="cblProductInsertMalls" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="20" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="info_title_bg">
														<td align="left" colspan="2" width="90%">更新件数</td>
														<td align="left" rowspan="3">
															<asp:Button ID="btnSendUpdateData" Text="  送信  " runat="server" OnClientClick='return confirm("更新内容を送信します。よろしいですか？");' OnClick="btnSendUpdateData_Click" /></td>
													</tr>
													<tr class="info_item_bg">
														<td align="left" width="20%">商品マスタ</td>
														<td align="left"><asp:Label ID="lProductUpdate" runat="server"></asp:Label> 件&nbsp;(<a href="javascript:open_window('<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MALL_PRODUCT_UPLOAD_PRODUCT_LIST + "?" + Constants.REQUEST_KEY_MALLPRODUCTUPLOAD_DIPLAY_KBN + "=" + Constants.KBN_MALLPRODUCTUPLOAD_DIPLAY_KBN_UPDATE) %>','product_list','width=850,height=500,top=120,left=500,status=NO,scrollbars=yes');">一覧</a>)</td>
													</tr>
													<tr class="info_item_bg">
														<td align="left" width="20%">モール出品先</td>
														<td align="left"><asp:CheckBoxList id="cblProductUpdateMalls" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList></td>
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
	</tbody>
	<!--△ 一覧 △-->
	<tr>
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<script type="text/javascript">
// 今日設定
function SetToday(param)
{
	if (param == 'Insert')
	{
		document.getElementById('<%= ucProductInsertDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTime.Now.ToString("yyyy/MM/dd") %>';
		document.getElementById('<%= ucProductInsertDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
		document.getElementById('<%= ucProductInsertDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTime.Now.ToString("yyyy/MM/dd") %>';
		document.getElementById('<%= ucProductInsertDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
		reloadDisplayDateTimePeriod('<%= ucProductInsertDatePeriod.ClientID %>');
	}
	else
	{
		document.getElementById('<%= ucProductUpdateDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTime.Now.ToString("yyyy/MM/dd") %>';
		document.getElementById('<%= ucProductUpdateDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
		document.getElementById('<%= ucProductUpdateDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTime.Now.ToString("yyyy/MM/dd") %>';
		document.getElementById('<%= ucProductUpdateDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
		reloadDisplayDateTimePeriod('<%= ucProductUpdateDatePeriod.ClientID %>');
	}
}

// 今月設定
function SetThisMonth(param)
{
	<% var thisMonth = DateTime.Now.AddMonths(0); %>
	<% var thisMonthFrom = new DateTime(thisMonth.Year, thisMonth.Month, 1); %>
	<% var thisMonthTo = thisMonthFrom.AddMonths(1).AddDays(-1); %>
	if (param == 'Insert')
	{
		document.getElementById('<%= ucProductInsertDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTime.Now.ToString("yyyy/MM/01") %>';
		document.getElementById('<%= ucProductInsertDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
		document.getElementById('<%= ucProductInsertDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTime.Now.ToString("yyyy/MM") %>' + '/' + '<%= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) %>';
		document.getElementById('<%= ucProductInsertDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
		reloadDisplayDateTimePeriod('<%= ucProductInsertDatePeriod.ClientID %>');
	}
	else
	{
		document.getElementById('<%= ucProductUpdateDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTime.Now.ToString("yyyy/MM/01") %>';
		document.getElementById('<%= ucProductUpdateDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
		document.getElementById('<%= ucProductUpdateDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTime.Now.ToString("yyyy/MM") %>' + '/' + '<%= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) %>';
		document.getElementById('<%= ucProductUpdateDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
		reloadDisplayDateTimePeriod('<%= ucProductUpdateDatePeriod.ClientID %>');
	}
}
	// Reset form
	function Reset()
	{
		document.getElementById('<%= ucProductInsertDatePeriod.HfStartDate.ClientID %>').value = '';
		document.getElementById('<%= ucProductInsertDatePeriod.HfStartTime.ClientID %>').value = '';
		document.getElementById('<%= ucProductInsertDatePeriod.HfEndDate.ClientID %>').value = '';
		document.getElementById('<%= ucProductInsertDatePeriod.HfEndTime.ClientID %>').value = '';
		reloadDisplayDateTimePeriod('<%= ucProductInsertDatePeriod.ClientID %>');

		document.getElementById('<%= ucProductUpdateDatePeriod.HfStartDate.ClientID %>').value = '';
		document.getElementById('<%= ucProductUpdateDatePeriod.HfStartTime.ClientID %>').value = '';
		document.getElementById('<%= ucProductUpdateDatePeriod.HfEndDate.ClientID %>').value = '';
		document.getElementById('<%= ucProductUpdateDatePeriod.HfEndTime.ClientID %>').value = '';
		reloadDisplayDateTimePeriod('<%= ucProductUpdateDatePeriod.ClientID %>');

		this.document.<%= this.Form.ClientID %>.reset();
	}

	// 登録 送信時 ネクストエンジンが有効の場合はモール商品CSVダウンロードウィンドウをポップアップ
	function InsertConfirm() {
		if (confirm("登録内容を送信します。よろしいですか？")) {
			<% if (this.ValidNextEngine) { %>
			open_window('<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MALL_PRODUCT_UPLOAD_NEXT_ENGINE_CSV_DOWNLOAD) %>', 'product_list', 'width=850,height=500,top=120,left=500,status=NO,scrollbars=yes');
			return false;
			<% } else { %>
			return true;
			<% } %>
		} else {
			return false;
		}
	}

	// 登録 送信を実行 モール商品CSVダウンロードウィンドウをポップアップが表示されたタイミングで実行
	function InsertClick() {
		$('#<%= btnSendInsertDataDummy.ClientID %>').click();
	}
//-->
</script>
</asp:Content>
