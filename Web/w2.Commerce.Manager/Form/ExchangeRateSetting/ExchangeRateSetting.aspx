<%--
=========================================================================================================
  Module      : 為替レート設定画面 (ExchangeRateSetting.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ExchangeRateSetting.aspx.cs" Inherits="Form_ExchangeRateSetting_ExchangeRateSetting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<table cellspacing="0" cellpadding="0" width="791" border="0">
		<tr><td><h2 class="page-title">為替レート設定画面</h2></td></tr>
		<tr>
			<td style="width: 792px">
				<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
					<tr>
						<td>
							<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td><img height="40" width="1" border="0" alt="" src="../../Images/Common/sp.gif" /></td>
									<td align="center">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td><img height="40" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td runat="server">
													<div id="divErrorMessage" runat="server" class="action_part_top" Visible="False">
														<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
															<tr class="info_item_bg">
																<td align="left">
																	<asp:Label ID="lMessage" runat="server" />
																</td>
															</tr>
														</table>
													</div>
												</td>
											</tr>
											<tr>
												<td>
													<div class="action_part_top">
														<asp:Button ID="btnGetExchangeRateTop" runat="server" Text="  為替レート取得  " OnClick="btnGetExchangeRate_Click" OnClientClick="return confirm('最新の為替データを取得します。よろしいですか？');" />
														<asp:Button ID="btnClearTop" runat="server" Text="  クリア  " OnClick="btnClear_Click" OnClientClick="return confirm('編集前の状態に戻ります。よろしいですか？');" />
														<asp:Button ID="btnUpdateAllTop" runat="server" Text="  一括更新  " OnClick="btnUpdateAll_Click" OnClientClick="return confirm('入力した内容で上書きします。よろしいですか？');" />
													</div>
												</td>
											</tr>
											<tr>
												<td><img height="15" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td>
													<div id="divDisplayList" runat="server">
														<div>
															<table>
																<tr>
																	<td>
																		<div class="div_header_left">
																			<table class="list_table tableHeaderFixed">
																				<tr class="list_title_bg">
																					<td align="center" width="100">通貨コード</td>
																				</tr>
																			</table>
																		</div>
																	</td>
																	<td>
																		<div class="div_header_right">
																			<table class="list_table tableHeader" width="<%= (rDstCurrencyCode.Items.Count * 50) %>" border="0" style="height: 103px; table-layout: fixed;">
																				<tr id="table_header">
																					<asp:Repeater ID="rDstCurrencyCode" runat="server" ItemType="System.String">
																						<ItemTemplate>
																							<td class="list_title_bg" style="max-width: 225px; width: 100px;" align="center" runat="server">
																								<%#: Item %>
																							</td>
																						</ItemTemplate>
																					</asp:Repeater>
																				</tr>
																			</table>
																		</div>
																	</td>
																</tr>
															</table>
														</div>
														<div class="div_data" style="height:<%= (rList.Items.Count * 60) %>px; max-height: 600px;">
															<div class="div_data_left" style="max-height: 600px;">
																<table class="list_table tableDataFix" style="height: <%= (rSrcCurrencyCode.Items.Count * 60) %>px; max-height: 600px;">
																	<asp:Repeater ID="rSrcCurrencyCode" runat="server" ItemType="System.String">
																		<ItemTemplate>
																			<tr class="list_title_bg">
																				<td class="table_header_item" align="left" runat="server">
																					<%#: string.Format("{0} (1 {0})", Item) %>
																				</td>
																			</tr>
																		</ItemTemplate> 
																	</asp:Repeater>
																</table>
															</div>
															<div class="div_data_right" style="height: <%= (rList.Items.Count * 60 + 18) %>px; max-height: 600px;">
																<table class="list_table tableData" border="0">
																	<asp:Repeater ID="rList" OnItemDataBound="ItemBound" runat="server" ItemType="System.String">
																			<ItemTemplate>
																				<tr>
																					<asp:Repeater ID="rExchangeRate" ItemType="ExchangeRateInput" runat="server">
																						<ItemTemplate>
																							<td class="table_data_item" style="max-width: 225px; width: 100px; min-width: 225px;" align="center">
																								<div runat="server" Visible="<%# (Item.SrcCurrencyCode != Item.DstCurrencyCode) %>">
																									<asp:TextBox
																										ID="tbExchangeRate"
																										runat="server"
																										Text="<%# Item.ExchangeRate %>"
																										MaxLength="25"
																										style="max-width: 160px; width: 70%; text-align: right; padding-right: 5px;" />
																									<span style="font-size: 14px; position: relative; top: 2px;"><%#: Item.DstCurrencyCode %></span>
																								</div>
																							</td>
																						</ItemTemplate>
																					</asp:Repeater>
																				</tr>
																			</ItemTemplate>
																	</asp:Repeater>
																</table>
															</div>
														</div>
													</div>
												</td>
											</tr>
											<div id="divDisplayMessage" runat="server">
												<tr>
													<td>
														<table class="list_table" cellspacing="0" cellpadding="0" width="758" border="0">
															<tr class="list_alert">
																<td colspan="9"><%: WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST) %></td>
															</tr>
															<tr style="height:15px" />
														</table>
													</td>
												</tr>
											</div>
											<tr>
												<td><img height="15" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td>
													<div class="action_part_bottom">
														<asp:Button ID="btnClearBottom" runat="server" Text="  クリア  " OnClick="btnClear_Click" OnClientClick="return confirm('編集前の状態に戻ります。よろしいですか？');" />
														<asp:Button ID="btnUpdateAllBottom" runat="server" Text="  一括更新  " OnClick="btnUpdateAll_Click" OnClientClick="return confirm('入力した内容で上書きします。よろしいですか？');" />
													</div>
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
	</table>
	<script type="text/javascript">
		$(function () {
			$(".tableHeader").css("table-layout", "fixed");
			$(".tableData").css("table-layout", "fixed");

			$(".tableData").find("td").each(function () {
				$(this).attr("title", $(this).text()).css("overflow", "hidden");
			});

			setWidthTwoTable("tableData", "tableHeader");
			setHeightTwoTable("tableHeaderFixed", "tableHeader");

			$(".div_data_left").height($(".div_data_right").outerHeight());
			setHeightTwoTable("tableDataFix", "tableData");
			setHeightTwoTable("tableData", "tableDataFix");
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
