<%--
=========================================================================================================
  Module      : ユーザ情報一覧ページ(UserList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="w2.App.Common.Manager" %>
<%@ Import Namespace="w2.Domain.Coupon.Helper" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="UserList.aspx.cs" Inherits="Form_UserCoupon_UserList" %>
<%@ Import Namespace="w2.Domain.MenuAuthority.Helper" %>
<%-- マスタ出力コントロール --%>
<%@ Register TagPrefix="uc" TagName="MasterDownLoad" Src="~/Form/Common/MasterDownLoadPanel.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">ユーザークーポン情報</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<%--▽ 検索 ▽--%>
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
															<asp:DropDownList id="ddlSearchKey" runat="server"></asp:DropDownList></td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															検索値</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbSearchWord" runat="server" Width="120"></asp:TextBox></td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															並び順</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlSortKbn" runat="server">
																<asp:ListItem Value="8">ユーザID/昇順</asp:ListItem>
																<asp:ListItem Value="9" Selected="True">ユーザID/降順</asp:ListItem>
																<asp:ListItem Value="0">氏名/昇順</asp:ListItem>
																<asp:ListItem Value="1">氏名/降順</asp:ListItem>
																<asp:ListItem Value="2">氏名(かな)/昇順</asp:ListItem>
																<asp:ListItem Value="3">氏名(かな)/降順</asp:ListItem>
																<asp:ListItem Value="4">利用可能数/昇順</asp:ListItem>
																<asp:ListItem Value="5">利用可能数/降順</asp:ListItem>
																<asp:ListItem Value="6">利用済み数/昇順</asp:ListItem>
																<asp:ListItem Value="7">利用済み数/降順</asp:ListItem>
															</asp:DropDownList>
														</td>
														<td class="search_btn_bg" width="83">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " Onclick="btnSearch_Click"></asp:Button></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_USERCOUPON_USERLIST %>">クリア</a>&nbsp;
																<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a></div>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr><td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
										<%-- マスタ出力 --%>
										<tr>
											<td class="search_table">
												<uc:MasterDownLoad runat="server" ID="uMasterDownload" DownloadType="UserCoupon" TableWidth="758" />
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
	<%--△ 検索 △--%>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<%--▽ 一覧 ▽--%>
	<tr>
		<td><h2 class="cmn-hed-h2">ユーザー情報一覧</h2></td>
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
										<% if (this.IsNotSearchDefault) { %>
											<tr>
												<td>
													<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr class="list_alert">
															<td><%= WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NOT_SEARCH_DEFAULT) %></td>
														</tr>
													</table><br />
												</td>
											</tr>
										<% } else { %>
										<tr>
											<td align="left">
												<%--▽ ページング ▽--%>
												<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
													<tr>
														<td width="675"><asp:Label ID="lbPager1" Runat="server"></asp:Label></td>
													</tr>
												</table>
												<%--△ ページング △--%>
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<%--▽ 表示項目 ▽--%>
													<tr class="list_title_bg">
														<td align="center" width="110">ユーザID</td>
														<td align="center" width="120">顧客区分</td>
														<td align="center" width="218"><%: ReplaceTag("@@User.name.name@@") %></td>
														<td align="center" width="80">利用可能<br/>クーポン数</td>
														<td align="center" width="80">利用済み<br/>クーポン数</td>
														<td align="center" width="80">編集</td>
														<td align="center" width="80">履歴一覧</td>
													</tr>
													<asp:Repeater id="rList" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
																<td align="center">
																	<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_USER_CONFIRM)) { %>
																		<a href="#" onclick="javascript:open_window('<%# WebSanitizer.HtmlEncode(SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Ec, CreateUserDetailUrl(((UserListSearchResult)Container.DataItem).UserId))) %>','userdetail','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');"><%#: ((UserListSearchResult)Container.DataItem).UserId %></a>
																	<% } else { %>
																		<%#: ((UserListSearchResult)Container.DataItem).UserId %>
																	<% } %>
																</td>
																<td align="center"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_USER_KBN, ((UserListSearchResult)Container.DataItem).UserKbn)) %></td>
																<td align="left"><%# WebSanitizer.HtmlEncode(((UserListSearchResult)Container.DataItem).Name) %></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(((UserListSearchResult)Container.DataItem).UnusedCoupon))%></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(((UserListSearchResult)Container.DataItem).UsedCoupon)) %></td>
																<td align="center"><a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateUserCouponListUrl(((UserListSearchResult)Container.DataItem).UserId)) %>">発行・削除</a></td>
																<td align="center"><a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateUserCouponHistoryListUrl(((UserListSearchResult)Container.DataItem).UserId)) %>">参照</a></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<%--△ 表示項目 △--%>
													<%--▽ エラーメッセージ ▽--%>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" colspan="7" runat="server">
														</td>
													</tr>
													<%--△ エラーメッセージ △--%>
												</table>
												<br/>
											</td>
										</tr>
										<% } %>
										<tr>
											<td>
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td align="left" class="info_item_bg" colspan="2">備考<br/>
														利用可能クーポン数、利用済みクーポン数は管理者発行のクーポンを含んでいます。<br/>
														※管理者発行のクーポン → 無制限利用可能クーポン、回数制限付きクーポン、ブラックリスト型クーポン<br />
														これらクーポンの利用可能クーポン数については発行数に関わらず各ユーザー1枚として集計しています。
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
	<%--△ 一覧 △--%>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
