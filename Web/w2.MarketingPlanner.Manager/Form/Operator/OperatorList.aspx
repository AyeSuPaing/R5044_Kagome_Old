<%--
=========================================================================================================
  Module      : オペレータ情報一覧ページ(OperatorList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OperatorList.aspx.cs" Inherits="Form_Operator_OperatorList" %>
<%@ Import Namespace="w2.Domain.ShopOperator" %>
<%@ Import Namespace="w2.App.Common.Operator" %>
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
			<table class="box_border cmn-section" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td class="search_box_bg">
						<table cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0" class="wide-content">
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td class="search_box_bg">
												<table class="search_table cmn-form" cellspacing="1" cellpadding="2" width="758" border="0">
													<tr　class="search_box_bg">
														<td class="search_title_bg" width="95"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />オペレータID</td>
														<td class="search_item_bg" width="130"><asp:TextBox id="tbOperatorId" runat="server" Width="130" /></td>
														<td class="search_title_bg" width="95"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />オペレータ名</td>
														<td class="search_item_bg" width="130"><asp:TextBox id="tbOperatorName" runat="server" Width="130" /></td>
														<td class="search_title_bg" width="95"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />並び順</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlSortKbn" runat="server" CssClass="search_item_width">
																<asp:ListItem Value="0">オペレータID/昇順</asp:ListItem>
																<asp:ListItem Value="1">オペレータID/降順</asp:ListItem>
																<asp:ListItem Value="2">オペレータ名/昇順</asp:ListItem>
																<asp:ListItem Value="3">オペレータ名/降順</asp:ListItem>
															</asp:DropDownList>
														</td>
														<td class="search_btn_bg" width="63" rowspan="2">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  "  OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_OPERATOR_LIST %>">クリア</a>&nbsp;
																<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a></div>
														</td>
													</tr>
													<tr　class="search_box_bg">
														<td class="search_title_bg" width="95"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />メニュー権限</td>
														<td class="search_item_bg"colspan="3" width="130"><asp:DropDownList id="ddlMenuAccess" runat="server" /></td>
														<td class="search_title_bg" width="95"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />有効フラグ</td>
														<td class="search_item_bg" width="130"><asp:DropDownList id="ddlValid" runat="server" /></td>
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
		</td>
	</tr>
	<!--△ 検索 △-->
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">オペレータ情報一覧</h2></td>
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
												<table cellspacing="1" cellpadding="3" width="758" class="list_table" border="0">
													<tr class="list_title_bg">
														<td align="center" width="15%">オペレータID</td>
														<td align="center" width="50%">オペレータ名</td>
														<td align="center">メニュー権限</td>
														<td align="center">有効フラグ</td>
													</tr>
													<asp:Repeater id="rOperatorList" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# WebSanitizer.HtmlEncode(Container.ItemIndex % 2 + 1) %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# WebSanitizer.HtmlEncode(Container.ItemIndex % 2 + 1) %>(this)"
																onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(OperatorUtility.CreateDetailUrl(((ShopOperatorModel)Container.DataItem).OperatorId)) %>')">
																<td align="center"><%# WebSanitizer.HtmlEncode(((ShopOperatorModel)Container.DataItem).OperatorId)%></td>
																<td align="left"><%# WebSanitizer.HtmlEncode(((ShopOperatorModel)Container.DataItem).Name) %></td>
																<td align="left">&nbsp;<%# WebSanitizer.HtmlEncode(GetMpMenuAccessLevelName(((ShopOperatorModel)Container.DataItem))) %></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(((ShopOperatorModel)Container.DataItem).ValidFlagName) %></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="4"></td>
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