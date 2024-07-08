<%--
=========================================================================================================
  Module      : メールクリックレポートページ(MailClickReportDetail.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MailClickReportDetail.aspx.cs" Inherits="Form_MailClickReport_MailClickReportDetail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">メールクリックレポート</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 詳細 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">メールクリックレポート</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td>
						<table class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td>
												<div class="action_part_top">
													<asp:Button id="btnBackTop" runat="server" Text="  一覧へ戻る  " OnClick="btnBack_Click" />
												</div>
												<table class="detail_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr>
														<td align="left" class="detail_title_bg" width="18%">メール文章ID</td>
														<td align="left" class="detail_item_bg"><asp:Label id="lbMailTextId" runat="server"></asp:Label></td>
														<td align="left" class="detail_title_bg" width="18%">メール文章名</td>
														<td align="left" class="detail_item_bg"><asp:Label id="lbMailTextName" runat="server"></asp:Label></td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg" width="18%">メール配信ID</td>
														<td align="left" class="detail_item_bg"><asp:Label id="lbMailDistId" runat="server"></asp:Label></td>
														<td align="left" class="detail_title_bg" width="18%">メール配信名</td>
														<td align="left" class="detail_item_bg"><asp:Label id="lbMailDistName" runat="server"></asp:Label></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="detail_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr>
														<td align="left" class="detail_title_bg" width="28"></td>
														<td align="left" class="detail_title_bg" width="300">メール配信開始日時</td>
														<td align="right" class="detail_title_bg" width="215">メール配信件数</td>
														<td align="right" class="detail_title_bg" width="215">総メールクリック件数</td>
													</tr>
													<asp:Repeater ID="rListActionNo" runat="server">
														<ItemTemplate>
															<tr>
																<td align="left" class="detail_item_bg">
																	<asp:RadioButton ID="rbActionNoSelect" AutoPostBack="true" runat="server" OnCheckedChanged="rbActionNoSelect_CheckedChanged" />
																	<asp:HiddenField ID="hfActonNo" Value='<%# WebSanitizer.HtmlEncode(Eval( Constants.FIELD_TASKSCHEDULE_ACTION_NO))%>' runat="server" />
																	<asp:HiddenField ID="hfSendCounts" Value='<%# WebSanitizer.HtmlEncode(Eval( "send_counts"))%>' runat="server" /></td>
																<td align="left" class="detail_item_bg"><%#: DateTimeUtility.ToStringForManager(Eval(Constants.FIELD_TASKSCHEDULE_DATE_BEGIN), DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter)%></td>
																<td align="right" class="detail_item_bg"><%# WebSanitizer.HtmlEncode(Eval("send_counts"))%></td>
																<td align="right" class="detail_item_bg"><%# WebSanitizer.HtmlEncode(Eval("total_click_counts"))%></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<asp:Repeater ID="rList" runat="server">
													<HeaderTemplate>
														<table class="detail_table" width="758" border="0" cellspacing="1" cellpadding="3">
															<tr>
																<td align="center" class="detail_title_bg" width="30"></td>
																<td align="left" class="detail_title_bg" width="438">クリックURL</td>
																<td align="center" class="detail_title_bg" width="80">クリック<br />ユーザ数</td>
																<td align="center" class="detail_title_bg" width="80">クリック<br />回数</td>
																<td align="right" class="detail_title_bg" width="70">クリック率</td>
																<td align="right" class="detail_title_bg" width="60"></td>
															</tr>
													</HeaderTemplate>
													<ItemTemplate>
														<tr>
															<td align="center" class="detail_item_bg"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_MAILCLICK_PCMOBILE_KBN)) %></td>
															<td align="left" class="detail_item_bg"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_MAILCLICK_MAILCLICK_URL)) %></td>
															<td align="right" class="detail_item_bg"><%# WebSanitizer.HtmlEncode(Eval("click_users", "{0:N0}"))%></td>
															<td align="right" class="detail_item_bg"><%# WebSanitizer.HtmlEncode(Eval("click_counts", "{0:N0}"))%></td>
															<td align="right" class="detail_item_bg"><%# WebSanitizer.HtmlEncode(Eval("click_rate", "{0:0.00}"))%>%</td>
															<td align="center" class="detail_item_bg">
																<asp:HiddenField ID="hfActionNo" Value='<%# Eval(Constants.FIELD_MAILCLICK_ACTION_NO) %>' runat="server" />
																<asp:HiddenField ID="hfMailClickId" Value='<%# Eval(Constants.FIELD_MAILCLICK_MAILCLICK_ID) %>' runat="server" />
																<asp:Button ID="btnDailyReport" Text="  詳細  " runat="server" OnClick="btnDailyReport_Click" /></td>
														</tr>
													</ItemTemplate>
													<FooterTemplate>
														</table>
													</FooterTemplate>
												</asp:Repeater>
												<div class="action_part_bottom">
													<asp:Button id="btnBackBottom" runat="server" Text="  一覧へ戻る  " OnClick="btnBack_Click" />
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
	<!--△ 詳細 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<script type="text/javascript">
	$(window).bind('pageshow',
		window.addEventListener('pageshow', function() {
			$('#<%= this.Form.ClientID %>')[0].reset();
		}));
</script>
</asp:Content>
