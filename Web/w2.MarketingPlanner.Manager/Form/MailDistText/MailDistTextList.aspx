<%--
=========================================================================================================
  Module      : メール配信文章一覧ページ(MailDistTextList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="System.Data" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MailDistTextList.aspx.cs" Inherits="Form_MailDistText_MailDistTextList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">メール配信文章設定</h1></td>
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
											<td class="search_box">
												<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="search_title_bg" width="95"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />検索項目</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlSearchKey" runat="server">
																<asp:ListItem Value="1" Selected="True">メール文章名</asp:ListItem>
																<asp:ListItem Value="2">メールタイトル</asp:ListItem>
															</asp:DropDownList></td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															検索値</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbSearchWord" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															並び順</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlSortKbn" runat="server">
																<asp:ListItem Value="1">メール文章ID/昇順</asp:ListItem>
																<asp:ListItem Value="2" Selected="True">メール文章ID/降順</asp:ListItem>
															</asp:DropDownList>
														</td>
														<td class="search_btn_bg" width="83">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " onclick="btnSearch_Click"></asp:Button></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MAILDIST_TEXT_LIST %>">クリア</a></div>
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
		<td><h2 class="cmn-hed-h2">メール配信文章設定一覧</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<td class="list_box_bg">
						<table cellspacing="0" cellpadding="0" width="100%" border="0">
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
														<td width="675"><asp:Label ID="lbPager" Runat="server"></asp:Label></td>
														<td width="83" class="action_list_sp"><asp:Button id="btnNew" Runat="server" Text="  新規登録  " onclick="btnNew_Click"></asp:Button></td>
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
														<td align="center" width="15%">メール文章ID</td>
														<td align="center" width="40%">メール文章名</td>
														<td align="center" >メールタイトル</td>
													</tr>
													<asp:Repeater id="rMailDistTextList" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg1" onmouseover="listselect_mover(this)" onmouseout="listselect_mout1(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateDetailUrl((String)Eval(Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID))) %>')">
																<td align="center">
																	<%# WebSanitizer.HtmlEncode(((DataRowView)Container.DataItem)[Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID]) %></td>
																<td align="left">
																	<%# WebSanitizer.HtmlEncode(((DataRowView)Container.DataItem)[Constants.FIELD_MAILDISTTEXT_MAILTEXT_NAME]) %> </td>
																<td align="left">
																	<%# WebSanitizer.HtmlEncode(((DataRowView)Container.DataItem)[Constants.FIELD_MAILDISTTEXT_MAILTEXT_SUBJECT]) %> </td>
															</tr>
														</ItemTemplate>
														<AlternatingItemTemplate>
															<tr class="list_item_bg2" onmouseover="listselect_mover(this)" onmouseout="listselect_mout2(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateDetailUrl((String)Eval(Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID))) %>')">
																<td align="center">
																	<%# WebSanitizer.HtmlEncode(((DataRowView)Container.DataItem)[Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID]) %></td>
																<td align="left">
																	<%# WebSanitizer.HtmlEncode(((DataRowView)Container.DataItem)[Constants.FIELD_MAILDISTTEXT_MAILTEXT_NAME]) %> </td>
																<td align="left">
																	<%# WebSanitizer.HtmlEncode(((DataRowView)Container.DataItem)[Constants.FIELD_MAILDISTTEXT_MAILTEXT_SUBJECT]) %> </td>
															</tr>
														</AlternatingItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="3"></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td class="action_list_sp"><asp:Button id="btnNew2" Runat="server" Text="  新規登録  " onclick="btnNew_Click"></asp:Button></td>
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

