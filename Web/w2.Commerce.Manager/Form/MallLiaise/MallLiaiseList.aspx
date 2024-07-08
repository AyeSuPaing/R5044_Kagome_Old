<%--
=========================================================================================================
  Module      : モール連携基本設定 設定一覧ページ(MallLiaiseList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MallLiaiseList.aspx.cs" Inherits="Form_MailLiaise_MallLiaiseList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">

<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td>
			<h1 class="page-title">モール連携基本設定</h1>
		</td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 検索 ▽-->
	<tr>
		<td colspan="2">
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
														<td class="search_title_bg" width="20%"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />モールＩＤ</td>
														<td class="search_item_bg" width="25%"><asp:TextBox ID="tbMallId" runat="server" /></td>
														<td class="search_title_bg" width="20%"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />並び順</td>
														<td class="search_item_bg" width="25%">
															<asp:DropDownList ID="ddlSort" runat="server">
																<asp:ListItem Text="モールＩＤ" Value="mall_id"></asp:ListItem>
																<asp:ListItem Text="モール設定名" Value="mall_name"></asp:ListItem>
																<asp:ListItem Text="モール区分" Value="mall_kbn"></asp:ListItem>
																<asp:ListItem Text="有効フラグ" Value="valid_flg"></asp:ListItem>
															</asp:DropDownList>
														</td>
														<td class="search_title_bg" width="10%" rowspan="2">
															<div class="search_btn_main"><asp:Button ID="btnSearch" Text="  検索  " OnClick="btnSearch_Click" runat="server" /></div>
															<div class="search_btn_sub"><a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_MALL_LIAISE_LIST %>" >クリア</a></div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="20%"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />モール名</td>
														<td class="search_item_bg" width="25%"><asp:TextBox ID="tbMallName" runat="server" /></td>
														<td class="search_title_bg" width="20%"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />モール区分</td>
														<td class="search_item_bg" width="25%"><asp:DropDownList ID="ddlMallKbn" runat="server" /></td>
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
		<td><h2 class="cmn-hed-h2">モール連携基本設定一覧</h2></td>
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
														<td width="300px" style="height: 22px"><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
														<td class="action_list_sp" style="text-align:right">
															<asp:Button id="btnRegistTop" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" />
														</td>
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
												<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="list_title_bg" style="height: 17px">
														<td align="center" width="120" style="height: 17px">モールＩＤ</td>
														<td align="center" width="120" style="height: 17px">モール区分</td>
														<td align="left" width="300" style="height: 17px">モール設定名</td>
														<td align="center" width="150" style="height: 17px">モール出品設定</td>
														<td align="center" width="68" style="height: 17px">有効フラグ</td>
													</tr>
													<!-- ▽ モール連携基本設定 一覧表示 ▽ -->
													<asp:Repeater id="rMallCooperationSettingList" Runat="server">
														<ItemTemplate>
															<tr id="rMallCooperationSettingListItem<%# Container.ItemIndex%>" class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)">
																<td align="center" onclick="listselect_mclick(getElementById('rMallCooperationSettingListItem<%# Container.ItemIndex%>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateMallConfigUrl( (string)((Hashtable)Container.DataItem)[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID] ))%>')"><%# WebSanitizer.HtmlEncode((string)((Hashtable)Container.DataItem)[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID])%></td>
																<td align="center" onclick="listselect_mclick(getElementById('rMallCooperationSettingListItem<%# Container.ItemIndex%>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateMallConfigUrl( (string)((Hashtable)Container.DataItem)[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID] ))%>')"><%# WebSanitizer.HtmlEncode((string)((Hashtable)Container.DataItem)["mall_kbn_view"])%></td>
																<td align="left" onclick="listselect_mclick(getElementById('rMallCooperationSettingListItem<%# Container.ItemIndex%>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateMallConfigUrl( (string)((Hashtable)Container.DataItem)[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID] ))%>')"><%# WebSanitizer.HtmlEncode((string)((Hashtable)Container.DataItem)[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_NAME])%></td>
																<td align="center" onclick="listselect_mclick(getElementById('rMallCooperationSettingListItem<%# Container.ItemIndex%>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateMallConfigUrl( (string)((Hashtable)Container.DataItem)[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID] ))%>')"><%# WebSanitizer.HtmlEncode((string)((Hashtable)Container.DataItem)[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG])%></td>
																<td align="center" onclick="listselect_mclick(getElementById('rMallCooperationSettingListItem<%# Container.ItemIndex%>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateMallConfigUrl( (string)((Hashtable)Container.DataItem)[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID] ))%>')"><%# WebSanitizer.HtmlEncode((string)((Hashtable)Container.DataItem)[Constants.FIELD_MALLCOOPERATIONSETTING_VALID_FLG])%></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="5"></td>
													</tr>
													<!-- △ モール連携基本設定 一覧表示 △ -->
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="4px" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td class="action_list_sp" style="text-align:right">
												<asp:Button id="btnRegistBottom" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" />
											</td>
										</tr>
										<tr>
											<td><img height="10px" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
</table>
</asp:Content>
