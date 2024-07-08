<%--
=========================================================================================================
  Module      : 入荷通知メール情報一覧ページ(UserProductArrivalMailList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="UserProductArrivalMailList.aspx.cs" Inherits="Form_UserProductArrivalMail_UserProductArrivalMailList" %>
<%-- マスタ出力コントロール --%>
<%@ Register TagPrefix="uc" TagName="MasterDownLoad" Src="~/Form/Common/MasterDownLoadPanel.ascx" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<asp:Content id="Content2" ContentPlaceHolderid="ContentPlaceHolderHead" Runat="Server">
<meta http-equiv="Pragma" content="no-cache" />
<meta http-equiv="cache-control" content="no-cache" />
<meta http-equiv="expires" content="0" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">

<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">入荷通知メール管理</h1></td>
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
												<%
													// 各テキストボックスのEnter押下時に検索を走らせるようにする
													tbProductId.Attributes["onkeypress"]
														= tbProductName.Attributes["onkeypress"]
														= tbProductStock.Attributes["onkeypress"]
														= "if (event.keyCode==13){__doPostBack('" + btnSearch.UniqueID + "',''); return false;}";
												%>
												<table cellspacing="1" cellpadding="2" width="768" border="0">
													<tr>
														<td class="search_title_bg" width="98">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															商品ID</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbProductId" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="100">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															商品名</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbProductName" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="100">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															並び順</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlSortKbn" runat="server">
																<asp:ListItem Value="0" Selected="True">商品ID/昇順</asp:ListItem>
																<asp:ListItem Value="1">商品ID/降順</asp:ListItem>
																<asp:ListItem Value="2">販売開始日/昇順</asp:ListItem>
																<asp:ListItem Value="3">販売開始日/降順</asp:ListItem>
																<asp:ListItem Value="4">再入荷通知件数/昇順</asp:ListItem>
																<asp:ListItem Value="5">再入荷通知件数/降順</asp:ListItem>
																<asp:ListItem Value="6">販売開始通知件数/昇順</asp:ListItem>
																<asp:ListItem Value="7">販売開始通知件数/降順</asp:ListItem>
																<asp:ListItem Value="8">再販売通知件数/昇順</asp:ListItem>
																<asp:ListItem Value="9">再販売通知件数/降順</asp:ListItem>
															</asp:DropDownList>
														</td>
														<td class="search_btn_bg" width="83" rowspan="8">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_USERPRODUCTARRIVALMAIL_LIST %>">クリア</a>
																<a href="javascript:Reset();">リセット</a></div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															在庫数</td>
														<td class="search_item_bg" colspan="5">
															<asp:TextBox id="tbProductStock" runat="server" Width="125"></asp:TextBox>&nbsp;件以上</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															販売開始日</td>
														<td class="search_item_bg" colspan="5">
															<div id="productSellFromDate">
																<uc:DateTimePickerPeriodInput id="ucProductSellFromDate" runat="server" IsNullStartDateTime="true" IsNullEndDateTime="true" />
																<span class="search_btn_sub">(<a href="Javascript:SetToday('sell_from');">今日</a>)</span>
															</div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															販売終了日</td>
														<td class="search_item_bg" colspan="5">
															<div id="productSellToDate">
																<uc:DateTimePickerPeriodInput id="ucProductSellToDate" runat="server" IsNullStartDateTime="true" IsNullEndDateTime="true" />
																<span class="search_btn_sub">(<a href="Javascript:SetToday('sell_to');">今日</a>)</span>
															</div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															販売期間</td>
														<td class="search_item_bg" colspan="5">
															<div id="productSalesPeriod">
																<uc:DateTimePickerPeriodInput id="ucProductSalesPeriod" CanShowEndDatePicker="False" IsNullStartDateTime="true" IsNullEndDateTime="true" runat="server"/>
																<span class="search_btn_sub">(<a href="Javascript:SetToday('sales_period');">今日</a>)</span>
																が含まれる
															</div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															表示期間</td>
														<td class="search_item_bg" colspan="5">
															<div id="productDisplayPeriod">
																<uc:DateTimePickerPeriodInput id="ucProductDisplayPeriod" CanShowEndDatePicker="False" IsNullStartDateTime="true" IsNullEndDateTime="true" runat="server"/>
																<span class="search_btn_sub">(<a href="Javascript:SetToday('display_period');">今日</a>)</span>
																が含まれる
															</div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															商品有効フラグ</td>
														<td class="search_item_bg" colspan="5" valign="middle">
															<asp:CheckBox ID="cbProductValidFlg" Runat="server" Text=" 有効のみ" TextAlign="Right" Checked="True"></asp:CheckBox>&nbsp;
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															入荷通知メール区分</td>
														<td class="search_item_bg" colspan="5" valign="middle">
															<asp:CheckBox ID="cbSearchArrivalMail" Runat="server" Text=" 再入荷通知" TextAlign="Right"></asp:CheckBox>&nbsp;
															<asp:CheckBox ID="cbSearchReleaseMail" Runat="server" Text=" 販売開始通知" TextAlign="Right"></asp:CheckBox>&nbsp;
															<asp:CheckBox ID="cbSearchResaleMail" Runat="server" Text=" 再販売通知" TextAlign="Right"></asp:CheckBox>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr><td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
										<tr>
											<td class="search_table">
												<uc:MasterDownLoad runat="server" ID="uMasterDownload" DownloadType="UserProductArrivalMail" TableWidth="768" />
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
		<td><h2 class="cmn-hed-h2">入荷通知メール一覧</h2></td>
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
														<td width="550" style="height: 22px"><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
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
														<td align="center" width="5%" style="height: 17px">送信<br /><input id="checkboxTargetAll" name="checkboxTargetAll" type="checkbox" onclick="javascript:selected_target_all();" /></td>
														<td align="center" width="10%" style="height: 17px">商品ID</td>
														<td align="center" width="15%" style="height: 17px">バリエーションID</td>
														<td align="center" width="30%" style="height: 17px">商品名</td>
														<td align="center" width="6%" style="height: 17px">在庫数</td>
														<td align="center" width="10%" style="height: 17px">販売開始日</td>
														<td align="center" width="8%" style="height: 17px">再入荷<br />通知件数</td>
														<td align="center" width="8%" style="height: 17px">販売開始<br />通知件数</td>
														<td align="center" width="8%" style="height: 17px">再販売<br />通知件数</td>
													</tr>
													<asp:Repeater id="rList" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" >
																<td align="center">
																	<asp:CheckBox ID="cbTarget" Runat="server"></asp:CheckBox>
																	<asp:HiddenField ID="hfShopId" Value='<%# Eval(Constants.FIELD_USERPRODUCTARRIVALMAIL_SHOP_ID) %>' runat="server" />
																	<asp:HiddenField ID="hfProductId" Value='<%# Eval(Constants.FIELD_USERPRODUCTARRIVALMAIL_PRODUCT_ID) %>' runat="server" />
																	<asp:HiddenField ID="hfVariationId" Value='<%# Eval(Constants.FIELD_USERPRODUCTARRIVALMAIL_VARIATION_ID) %>' runat="server" />
																	<asp:HiddenField ID="hfStock" Value='<%# (string)Eval(Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN) != Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED ? Eval(Constants.FIELD_PRODUCTSTOCK_STOCK) : "1" %>' runat="server" />
																</td>
																<td align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USERPRODUCTARRIVALMAIL_PRODUCT_ID)) %></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USERPRODUCTARRIVALMAIL_VARIATION_ID))%></td>
																<td align="left"><%# WebSanitizer.HtmlEncode(CreateProductAndVariationName((System.Data.DataRowView)Container.DataItem))%></td>
																<td align="center"><%# WebSanitizer.HtmlEncode((string)Eval(Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN) != Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED ? Eval(Constants.FIELD_PRODUCTSTOCK_STOCK) : "-") %></td>
																<td align="center"><%#: DateTimeUtility.ToStringForManager(Eval(Constants.FIELD_PRODUCT_SELL_FROM), DateTimeUtility.FormatType.ShortDate2Letter) %></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_COUNT)) %></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USERPRODUCTARRIVALMAIL_RELEASE_MAIL_COUNT))%></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USERPRODUCTARRIVALMAIL_RESALE_MAIL_COUNT))%></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="9"></td>
													</tr>
												</table>
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
	<!--▽ メール配信 ▽-->
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
											<td>
												<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
												<div id="divSending" runat="server">
												<table class="info_table" cellspacing="1" cellpadding="3" width="768" border="0">
													<tr class="info_item_bg">
														<td align="left">
															送信処理を開始しました。（<%= DateTime.Now %>）　送信完了後に結果メールが送信されます。<br />
														</td>
													</tr>
												</table>
												<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
												</div>
											</td>
										</tr>
										<tr>
											<td class="search_table">
												<table cellspacing="1" cellpadding="2" width="768" border="0">
													<tr>
														<td class="search_title_bg" width="135">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															再入荷通知メール</td>
														<td class="search_item_bg" width="540">
															<asp:CheckBox ID="cbSendArrivalMail" Runat="server" Text=" 送信" TextAlign="Right"></asp:CheckBox>&nbsp;
															<asp:DropDownList id="ddlMailTemplateArrival" runat="server"></asp:DropDownList>
														</td>
														<td class="search_btn_bg" width="93" rowspan="3">
															<div class="search_btn_main">
																<asp:Button id="btnSend" Text="  メール配信  " OnClientClick="return CheckStockBuyable()" runat="server" OnClick="btnSend_Click" />
															</div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="135">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															販売開始通知メール</td>
														<td class="search_item_bg" width="540">
															<asp:CheckBox ID="cbSendReleaseMail" Runat="server" Text=" 送信" TextAlign="Right"></asp:CheckBox>&nbsp;
															<asp:DropDownList id="ddlMailTemplateRelease" runat="server"></asp:DropDownList>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="135">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															再販売通知メール</td>
														<td class="search_item_bg" width="540">
															<asp:CheckBox ID="cbSendResaleMail" Runat="server" Text=" 送信" TextAlign="Right"></asp:CheckBox>&nbsp;
															<asp:DropDownList id="ddlMailTemplateResale" runat="server"></asp:DropDownList>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td>
												<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
												<table class="info_table" cellspacing="1" cellpadding="3" width="768" border="0">
													<tr class="info_item_bg">
														<td align="left">
															備考：<br />
															再入荷通知件数　　：　販売期間内在庫切れ商品に対するリクエスト件数です。<br />
															販売開始通知件数　：　販売開始前商品に対するリクエスト件数です。<br />
															再販売通知件数　　：　販売終了商品に対するリクエスト件数です。<br />
															<br />
															メール配信時は対象の商品とメール種類を選択してください。<br />
															メールテンプレートで「送信しない（リストから削除）」を選択すると、メールは送信せずにリストから削除することができます。<br />
														</td>
													</tr>
												</table>
												<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
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
	<!--△ メール配信 △-->
	<tr>
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<script type="text/javascript">
<!--
	// 今日設定
	function SetToday(set_date) {
		// 販売開始日
		if (set_date == 'sell_from') {
			document.getElementById('<%= ucProductSellFromDate.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
			document.getElementById('<%= ucProductSellFromDate.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucProductSellFromDate.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
			document.getElementById('<%= ucProductSellFromDate.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucProductSellFromDate.ClientID %>');
		}
		// 販売終了日
		else if (set_date == 'sell_to') {
			document.getElementById('<%= ucProductSellToDate.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
			document.getElementById('<%= ucProductSellToDate.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucProductSellToDate.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
			document.getElementById('<%= ucProductSellToDate.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucProductSellToDate.ClientID %>');
		}
		// 販売期間
		else if (set_date == 'sales_period') {
			document.getElementById('<%= ucProductSalesPeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
			document.getElementById('<%= ucProductSalesPeriod.HfStartTime.ClientID %>').value = '00:00:00';
			reloadDisplayDateTimePeriod('<%= ucProductSalesPeriod.ClientID %>');
		}
		// 表示期間
		else if (set_date == 'display_period') {
			document.getElementById('<%= ucProductDisplayPeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
			document.getElementById('<%= ucProductDisplayPeriod.HfStartTime.ClientID %>').value = '00:00:00';
			reloadDisplayDateTimePeriod('<%= ucProductDisplayPeriod.ClientID %>');
		}
	}

	// Reset
	function Reset() {
		document.getElementById('<%= ucProductDisplayPeriod.HfStartDate.ClientID %>').value = '';
		document.getElementById('<%= ucProductDisplayPeriod.HfStartTime.ClientID %>').value = '';
		document.getElementById('<%= ucProductDisplayPeriod.HfEndDate.ClientID %>').value = '';
		document.getElementById('<%= ucProductDisplayPeriod.HfEndTime.ClientID %>').value = '';
		reloadDisplayDateTimePeriod('<%= ucProductDisplayPeriod.ClientID %>');
	
		document.getElementById('<%= ucProductSalesPeriod.HfStartDate.ClientID %>').value = '';
		document.getElementById('<%= ucProductSalesPeriod.HfStartTime.ClientID %>').value = '';
		document.getElementById('<%= ucProductSalesPeriod.HfEndDate.ClientID %>').value = '';
		document.getElementById('<%= ucProductSalesPeriod.HfEndTime.ClientID %>').value = '';
		reloadDisplayDateTimePeriod('<%= ucProductSalesPeriod.ClientID %>');
	
		document.getElementById('<%= ucProductSellFromDate.HfStartDate.ClientID %>').value = '';
		document.getElementById('<%= ucProductSellFromDate.HfStartTime.ClientID %>').value = '';
		document.getElementById('<%= ucProductSellFromDate.HfEndDate.ClientID %>').value = '';
		document.getElementById('<%= ucProductSellFromDate.HfEndTime.ClientID %>').value = '';
		reloadDisplayDateTimePeriod('<%= ucProductSellFromDate.ClientID %>');
	
		document.getElementById('<%= ucProductSellToDate.HfStartDate.ClientID %>').value = '';
		document.getElementById('<%= ucProductSellToDate.HfStartTime.ClientID %>').value = '';
		document.getElementById('<%= ucProductSellToDate.HfEndDate.ClientID %>').value = '';
		document.getElementById('<%= ucProductSellToDate.HfEndTime.ClientID %>').value = '';
		reloadDisplayDateTimePeriod('<%= ucProductSellToDate.ClientID %>');
		document.getElementById('<%= this.Form.ClientID %>').reset();
	}

	// 商品全選択・全解除
	function selected_target_all()
	{
		var chk = document.getElementById("checkboxTargetAll").checked;
		for (i = 0; i < document.getElementsByTagName("input").length; i++)
		{
			if ((document.getElementsByTagName("input")[i].type == "checkbox")
				&& ((document.getElementsByTagName("input")[i].name).indexOf("cbTarget") != -1))
			{
				document.getElementsByTagName("input")[i].checked = chk;
			}
		}
	}

	// メール送信時在庫数チェック
	function CheckStockBuyable()
	{
	<%
		foreach (RepeaterItem ri in rList.Items)
		{
	%>
			// チェックボックスがチェックされていて在庫数が0以下だったら確認メッセージを表示
			if (document.getElementById('<%= ((CheckBox)ri.FindControl("cbTarget")).ClientID %>').checked == true
				&& (<%= ((HiddenField)ri.FindControl("hfStock")).Value %> < 1))
			{
				return confirm('在庫が０以下の商品があります。送信してよろしいですか？');
			}
	<%
		}
	%>
		return true;
	}
//-->
</script>

</asp:Content>
