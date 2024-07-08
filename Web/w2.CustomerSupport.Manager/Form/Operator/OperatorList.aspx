<%--
=========================================================================================================
  Module      : オペレータ情報一覧ページ(OperatorList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OperatorList.aspx.cs" Inherits="Form_Operator_OperatorList" %>
<%@ MasterType VirtualPath="~/Form/Common/DefaultPage.master" %>
<%@ Import Namespace="w2.App.Common.Cs.CsOperator" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">オペレータ情報</h1></td>
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
														<td class="search_title_bg" width="125"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />オペレータID</td>
														<td class="search_item_bg" width="100"><asp:TextBox id="tbOperatorId" runat="server" Width="100" /></td>
														<td class="search_title_bg" width="95"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />オペレータ名</td>
														<td class="search_item_bg" width="130"><asp:TextBox id="tbOperatorName" runat="server" Width="130" /></td>
														<td class="search_title_bg" width="95"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />並び順</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlSortKbn" runat="server" CssClass="search_item_width">
																<asp:ListItem Value="2">表示順/昇順</asp:ListItem>
																<asp:ListItem Value="3">表示順/降順</asp:ListItem>
																<asp:ListItem Value="8">オペレータID/昇順</asp:ListItem>
																<asp:ListItem Value="9">オペレータID/降順</asp:ListItem>
																<asp:ListItem Value="0">オペレータ名/昇順</asp:ListItem>
																<asp:ListItem Value="1">オペレータ名/降順</asp:ListItem>
															</asp:DropDownList>
														</td>
														<td class="search_btn_bg" width="83" rowspan="6">
															<div class="search_btn_main"><asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_OPERATOR_LIST %>">クリア</a>&nbsp;
																<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a>
															</div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="125"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />メニュー権限</td>
														<td class="search_item_bg" colspan="3"><asp:DropDownList id="ddlMenuAccess" runat="server" CssClass="search_item_width" /></td>
														<td class="search_title_bg" width="95"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />有効フラグ</td>
														<td class="search_item_bg" width="130"><asp:DropDownList id="ddlValid" runat="server" CssClass="search_item_width" /></td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />メールアドレス</td>
														<td class="search_item_bg" colspan="3"><asp:TextBox id="tbOperatorMailAddress" runat="server" Width="200" /></td>
														<td class="search_title_bg" width="95"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />CSオペレータ</td>
														<td class="search_item_bg" >
															<asp:DropDownList id="ddlIsCsOperator" runat="server" CssClass="search_item_width">
																<asp:ListItem Value=""></asp:ListItem>
																<asp:ListItem Value="1">○</asp:ListItem>
																<asp:ListItem Value="0">－</asp:ListItem>
															</asp:DropDownList>
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
		<td><h2 class="cmn-hed-h2">オペレータ情報一覧</h2></td>
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
											<td>
												<!--▽ ページング ▽-->
												<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
													<tr>
														<td width="675"><asp:Label ID="lbPager" Runat="server"></asp:Label></td>
														<td width="83" class="action_list_sp" style="text-align:right"><asp:Button id="btnNew" Runat="server" Text="  新規登録  " OnClick="btnNew_Click" /></td>
													</tr>
												</table>
												<!--△ ページング △-->
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<div class="x_scrollable" style="overflow-x: scroll;">
												<table cellspacing="1" cellpadding="3" width="1258" class="list_table" border="0">
													<tr class="list_title_bg">
														<td align="center" width="5%" rowspan="3">オペレータID</td>
														<td align="center" width="7%" rowspan="3">オペレータ名</td>
														<td align="center" width="10%" rowspan="3">メニュー権限</td>
														<td align="center" width="4%" rowspan="3">有効フラグ</td>
														<td align="center" colspan="6">CSオペレータ情報</td>
													</tr>
													<tr class="list_title_bg">
														<td align="center" width="8%" rowspan="2">オペレータ権限</td>
														<td align="center" width="18%" rowspan="2">メール送信元</td>
														<td align="center" width="17%" colspan="3">メール受信設定</td>
														<td align="center" width="4%" rowspan="2">表示順</td>
													</tr>
													<tr class="list_title_bg">
														<td align="center" width="4%">通知</td>
														<td align="center" width="4%">警告</td>
														<td align="center" width="9%">メールアドレス</td>
													</tr>
													<asp:Repeater id="rOperatorList" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateDetailUrl(((CsOperatorModel)Container.DataItem).OperatorId)) %>')">
																<td align="center"><%# WebSanitizer.HtmlEncode(((CsOperatorModel)Container.DataItem).OperatorId)%></td>
																<td align="left" nowrap="nowrap">&nbsp;<%# WebSanitizer.HtmlEncode(((CsOperatorModel)Container.DataItem).EX_ShopOperatorName) %>&nbsp;</td>
																<td align="left">&nbsp;<%# WebSanitizer.HtmlEncode(GetMenuAccessLevel3Name(((CsOperatorModel)Container.DataItem))) %></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(((CsOperatorModel)Container.DataItem).EX_ValidFlagName) %></td>
																<td align="left" nowrap="nowrap">&nbsp;<%# WebSanitizer.HtmlEncode(((CsOperatorModel)Container.DataItem).EX_OperatorAuthorityName ?? Constants.STRING_UNACCESSABLEUSER_NAME) %>&nbsp;</td>
																<td align="left" nowrap="nowrap">&nbsp;<%# WebSanitizer.HtmlEncode(((CsOperatorModel)Container.DataItem).EX_MailFromDisplayName) %>&nbsp;</td>
																<td align="center"><%# WebSanitizer.HtmlEncode(((CsOperatorModel)Container.DataItem).EX_NotifyInfoFlgText) %></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(((CsOperatorModel)Container.DataItem).EX_NotifyWarnFlgText) %></td>
																<td align="left">&nbsp;<%# WebSanitizer.HtmlEncode(((CsOperatorModel)Container.DataItem).EX_MailAddrText) %></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(((CsOperatorModel)Container.DataItem).EX_DisplayOrderText) %></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="11"></td>
													</tr>
												</table>
												</div>
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td class="action_list_sp"><asp:Button id="btnNew2" Runat="server" Text="  新規登録  " OnClick="btnNew_Click" /></td>
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
