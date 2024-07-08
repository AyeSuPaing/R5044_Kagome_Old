<%--
=========================================================================================================
  Module      : ユーザクーポン履歴情報一覧ページ(UserCouponHistoryList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="w2.App.Common.Manager" %>
<%@ Import Namespace="w2.Domain.Coupon.Helper" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="UserCouponHistoryList.aspx.cs" Inherits="Form_UserCoupon_UserCouponHistoryList" %>
<%@ Import Namespace="w2.Domain.MenuAuthority.Helper" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="698" border="0">
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
											<td class="search_box_bg">
												<div class="action_part_top">
													<input type="button" onclick="Javascript:location.href='<%= CreateUserCouponListUrl() %>'" value="  編集（発行・削除）へ  " />
													<input type="button" onclick="Javascript:location.href='<%= CreateUserListUrl() %>'" value="  一覧へ戻る  " />
												</div>
												<%-- 基本情報 --%>
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">基本情報</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">ユーザーID</td>
														<td class="detail_item_bg" align="left">
															<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_USER_CONFIRM)) { %>
																<a href="#" onclick="javascript:open_window('<%= WebSanitizer.HtmlEncode(SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Ec, CreateUserDetailUrl(this.UserId))) %>','userdetail','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');"><%: this.UserId %></a>
															<% } else { %>
																<%#: this.UserId %>
															<% } %>

														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">顧客区分</td>
														<td class="detail_item_bg" align="left"><asp:Literal ID="lUserKbn" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%"><%: ReplaceTag("@@User.name.name@@") %></td>
														<td class="detail_item_bg" align="left"><asp:Literal ID="lName" runat="server"></asp:Literal></td>
													</tr>
												</table>
												<br />
												<%-- 検索 --%>
												<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="search_title_bg" width="95"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />検索項目</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlSearchKey" runat="server">
																<asp:ListItem Value="0" Selected="True">クーポンコード</asp:ListItem>
																<asp:ListItem Value="1">注文ID</asp:ListItem>
															</asp:DropDownList></td>
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
																<asp:ListItem Value="0">クーポンコード/昇順</asp:ListItem>
																<asp:ListItem Value="1">クーポンコード/降順</asp:ListItem>
																<asp:ListItem Value="2">日時/昇順</asp:ListItem>
																<asp:ListItem Value="3" Selected="True">日時/降順</asp:ListItem>
															</asp:DropDownList>
														</td>
														<td class="search_btn_bg" width="83">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " onclick="btnSearch_Click"></asp:Button></div>
															<div class="search_btn_sub">
																<a href="<%= CreateSearchUrl(this.UserId) %>">クリア</a>&nbsp;
																<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a></div>
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
	<%--△ 検索 △--%>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<%--▽ 一覧 ▽--%>
	<tr>
		<td><h2 class="cmn-hed-h2">ユーザークーポン情報履歴一覧</h2></td>
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
												<table class="list_pager" cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td><asp:label id="lbPager1" Runat="server"></asp:label></td>
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
													<tr class="list_title_bg">
														<td align="center" width="48" rowspan="2">No.</td>
														<td align="center" width="110">履歴区分</td>
														<td align="center" width="150">操作区分</td>
														<td align="center" width="260">クーポンコード</td>
														<td align="center" width="90">利用金額</td>
														<td align="center" width="110" rowspan="2">操作者</td>
														<td align="center" width="100" rowspan="2">備考</td>
													</tr>
													<tr class="list_title_bg">
														<td align="center" colspan="2">日時</td>
														<td align="center">クーポン名(管理用)</td>
														<td align="center">枚数</td>
													</tr>
													<asp:Repeater id="rList" ItemType="w2.Domain.Coupon.Helper.UserCouponHistoryListSearchResult" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
																<td align="center" rowspan="2"><%# ((UserCouponHistoryListSearchResult)Container.DataItem).RowNum %></td>
																<td align="center"><%# ValueText.GetValueText(Constants.TABLE_USERCOUPONHISTORY, Constants.FIELD_USERCOUPONHISTORY_HISTORY_KBN, Item.HistoryKbn)%></td>
																<td align="center"><%# ValueText.GetValueText(Constants.TABLE_USERCOUPONHISTORY, Constants.FIELD_USERCOUPONHISTORY_ACTION_KBN, Item.ActionKbn)%></td>
																<td align="center">
																<% if (ManagerMenuCache.Instance.HasOperatorMenuAuthority(Constants.PATH_ROOT + Constants.MENU_PATH_LARGE_COUPON)) { %>
																	<a href="<%# CreateCouponDetailUrl(Item.CouponId) %>"><%#: Item.CouponCode %></a>
																<% } else { %>
																	<%#: Item.CouponCode %>
																<% } %>
																</td>
																<td align="right"><%#: DisplayCouponPrice(Item) %></td>
																<td align="center" rowspan="2"><%#: Item.LastChanged %></td>
																<td align="center">
																	<%# ((Item.HistoryKbn == Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_FIXEDPURCHASE_USE)
																		|| (Item.HistoryKbn == Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_FIXEDPURCHASE_USE_CANCEL))
																		? "定期購入ID"
																		: (Item.HistoryKbn == Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_FIXEDPURCHASE_ADJUSTMENT)
																			? "定期購入ID<br />→注文ID"
																			: (Item.OrderId != "") ? "注文ID" : "" %>
																</td>
															</tr>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
																<td align="center" colspan="2"><%#: DateTimeUtility.ToStringForManager(Item.DateCreated, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter)%></td>
																<td align="center"><%#: Item.CouponName %></td>
																<td align="center"><%# DisplayCouponInc(Item) %></td>
																<td align="center">
																	<%# ((Item.HistoryKbn == Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_FIXEDPURCHASE_USE)
																			|| (Item.HistoryKbn == Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_FIXEDPURCHASE_USE_CANCEL))
																			? Item.FixedPurchaseId
																			: (Item.HistoryKbn == Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_FIXEDPURCHASE_ADJUSTMENT)
																				? string.Format("{0}<br />→{1}", Item.FixedPurchaseId, Item.OrderId)
																				: Item.OrderId %>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="7"></td>
													</tr>
												</table>
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
												</table>
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td align="left" class="info_item_bg" colspan="2">備考<br/>
														クーポン名(管理用)は「クーポン設定」から取得し、表示しています。<br/>
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
