<%--
=========================================================================================================
  Module      : メールクリック一覧(MailClickReportList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MailClickReportList.aspx.cs" Inherits="Form_MailClick_MailClickReportList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">メールクリックレポート</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 検索 ▽-->
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
											<td class="search_box_bg">
												<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="search_title_bg" width="120">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															メール文章ID</td>
														<td class="search_item_bg" width="180">
															<asp:TextBox id="tbMailTextId" runat="server"></asp:TextBox></td>
														<td class="search_title_bg" width="120">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															メール文章名</td>
														<td class="search_item_bg" width="180">
															<asp:TextBox ID="tbMailTextName" runat="server"></asp:TextBox></td>
														<td class="search_btn_bg" width="83" rowspan="3">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub">
																<a href="<%= Request.Url.AbsolutePath %>">クリア</a>&nbsp;
																<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a></div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															メール配信ID</td>
														<td class="search_item_bg">
															<asp:TextBox id="tbMailDistId" runat="server"></asp:TextBox></td>
														<td class="search_title_bg">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															メール配信名</td>
														<td class="search_item_bg">
															<asp:TextBox ID="tbMailDistName" runat="server"></asp:TextBox></td>
													</tr>
													<tr>
														<td class="search_title_bg">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															メールクリックURL</td>
														<td class="search_item_bg">
															<asp:TextBox id="tbMailClickUrl" runat="server"></asp:TextBox></td>
														<td class="search_title_bg">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															メールクリックキー</td>
														<td class="search_item_bg">
															<asp:TextBox id="tbMailClickKey" runat="server"></asp:TextBox></td>
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
		<td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">メールクリック情報一覧</h2></td>
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
											<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td align="left">
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
													<asp:Repeater id="rList" Runat="server">
														<HeaderTemplate>
															<tr class="list_title_bg">
																<td align="center" width="100">メール文章ID</td>
																<td align="left" width="220">メール文章名</td>
																<td align="center" width="100">メール配信ID</td>
																<td align="left" width="220">メール配信名</td>
																<td align="center" width="118">メール配信回数</td>
															</tr>
														</HeaderTemplate>
														<ItemTemplate>
															<tr class="list_item_bg1" onmouseover="listselect_mover(this)" onmouseout="listselect_mout1(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateDetailUrl(Container.DataItem)) %>')">
																<td align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID)) %></td>
																<td align="left"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_MAILDISTTEXT_MAILTEXT_NAME))%></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_MAILDISTSETTING_MAILDIST_ID)) %></td>
																<td align="left"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_MAILDISTSETTING_MAILDIST_NAME)) %></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(Eval("count")) %></td>
															</tr>
														</ItemTemplate>
														<AlternatingItemTemplate>
															<tr class="list_item_bg2" onmouseover="listselect_mover(this)" onmouseout="listselect_mout2(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateDetailUrl(Container.DataItem)) %>')">
																<td align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID))%></td>
																<td align="left"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_MAILDISTTEXT_MAILTEXT_NAME)) %></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_MAILDISTSETTING_MAILDIST_ID)) %></td>
																<td align="left"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_MAILDISTSETTING_MAILDIST_NAME)) %></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(Eval("count"))%></td>
															</tr>
														</AlternatingItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" colspan="5" runat="server">
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
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
