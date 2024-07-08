<%--
=========================================================================================================
  Module      : メールクリックレポート詳細ページ(MailClickReportDetail2.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MailClickReportDetail2.aspx.cs" Inherits="Form_MailClickReport_MailClickReportDetail2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">メールクリックレポート</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
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
													<tr class="search_title_bg">
														<td width="30%" colspan="2"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />対象年月</td>
													</tr>
													<tr class="search_item_bg">
														<td>
															<!-- ▽カレンダ▽ -->
															<asp:label id="lblCurrentCalendar" Runat="server"></asp:label>
															<!-- △カレンダ△ -->
														</td>
														<td width="600">
															<asp:RadioButtonList id="rblReportType" Runat="server" AutoPostBack="True" CssClass="radio_button_list" OnSelectedIndexChanged="rblReportType_SelectedIndexChanged">
																<asp:ListItem Value="0">日別レポート</asp:ListItem>
																<asp:ListItem Value="1">月別レポート</asp:ListItem>
															</asp:RadioButtonList>
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
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr> <!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">メールクリックレポート詳細</h2></td>
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
											<td><img height="2" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<div class="action_part_top">
													<asp:Button id="btnBackTop" runat="server" Text="  一覧へ戻る  " OnClick="btnBack_Click" />
												</div>
												<table class="info_table" width="700" border="0" cellspacing="1" cellpadding="3">
													<tr class="info_item_bg">
														<td align="left" id="tdReportInfo" runat="server"></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<!--▽ レポート ▽-->
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" border="0" width="700">
													<tr class="list_title_bg">
														<td class="list_title_bg" style="white-space:nowrap" width="40" rowspan="2"></td>
														<td id="tdAddPointTitle" class="list_item_bg2" style="white-space:nowrap" align="center" width="660" colspan="2" runat="server">メールクリック</td>
													</tr>
													<tr>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="330">ユーザ数</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="330">クリック数</td>
													</tr>
													<asp:Repeater id=rDataList Runat="server">
														<ItemTemplate>
															<tr>
																<td class="list_title_bg" align="center"><%# WebSanitizer.HtmlEncode(Eval("Title")) %></td>
																<td class="list_item_bg1" align="right">
																	<%# WebSanitizer.HtmlEncode(Eval("ClickUsers"))%></td>
																<td class="list_item_bg1" align="right">
																	<%# WebSanitizer.HtmlEncode(Eval("ClickCounts"))%></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
												</table>
												<div class="action_part_bottom">
													<asp:Button id="btnBackBottom" runat="server" Text="  一覧へ戻る  " OnClick="btnBack_Click" />
												</div>
											</td>
										</tr>
										<!--△ レポート △-->
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
	</tr> <!--△ 一覧 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<script type="text/javascript">
	$(window).bind('pageshow',
		window.addEventListener('pageshow', function () {
			$('#<%= this.Form.ClientID %>')[0].reset();
		}));
</script>
</asp:Content>

