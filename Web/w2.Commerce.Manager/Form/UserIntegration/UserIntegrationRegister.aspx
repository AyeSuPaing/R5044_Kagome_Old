<%--
=========================================================================================================
  Module      : ユーザー統合登録ページ(UserIntegrationRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Manager" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="UserIntegrationRegister.aspx.cs" Inherits="Form_UserIntegration_UserIntegrationRegister" MaintainScrollPositionOnPostback="true" %>
<%@ Import Namespace="w2.Domain.MenuAuthority.Helper" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<style type="text/css">
.edit_table td {
	word-break:break-all;
}
</style>
<script type="text/javascript">
<!--
	// ユーザー検索アクション
	function action_user_search(user_id) {
		document.getElementById('<%= hfUserId.ClientID %>').value = user_id;
		__doPostBack('<%= btnAddUser.UniqueID %>', '');
	}

	// 代表選択アクション
	function select_representative_flg() {
		__doPostBack('<%= btnRepresentativeFlg.UniqueID %>', '');
	}
//-->
</script>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr>
		<td><h1 class="page-title">ユーザー統合</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
	</tr>
	<!--△ タイトル △-->
	<!--▽ 登録/更新 ▽-->
	<tr>
		<td>
			<h2 class="cmn-hed-h2">ユーザー統合登録</h2>
		</td>
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
												<%--▽ 完了表示 ▽--%>
												<div id="divComp" runat="server" class="action_part_top" visible="false">
													<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
														<tr class="info_item_bg">
															<td align="left">
																<asp:Literal ID="lCompMessages" runat="server" />
															</td>
														</tr>
													</table>
												</div>
												<%--△ 完了表示 △--%>
												<div class="action_part_top">
													<asp:Button ID="btnToListTop" runat="server" Text="  一覧へ戻る  " OnClick="btnToList_Click" />
													<asp:Button id="btnNoneTop" runat="server" Text="  解除する  " OnClick="btnNone_Click" />
													<asp:Button id="btnSuspendTop" runat="server" Text="  保留する  " OnClick="btnSuspend_Click" />
													<asp:Button id="btnDoneTop" runat="server" Text="  統合する  " OnClick="btnDone_Click" />
													<asp:Button id="btnExcludedTop" runat="server" Text="  除外する  " OnClick="btnExcluded_Click" />
													<span style="display:none"><asp:Button id="btnRepresentativeFlg" runat="server" Text="  代表選択用ボタン（非表示）  " OnClick="btnRepresentativeFlg_Click" /></span>
												</div>
												<%--▽ 基本情報 ▽--%>
												<div id="divDetail" runat="server" visible="false">
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="edit_title_bg" align="center" colspan="2">
															基本情報
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="25%">ユーザー統合No</td>
														<td class="edit_item_bg" align="left">
															<asp:Literal ID="lNo" runat="server"></asp:Literal>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">ユーザー統合ステータス</td>
														<td class="edit_item_bg" align="left">
															<asp:Literal ID="lStatus" runat="server"></asp:Literal>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">作成日</td>
														<td class="edit_item_bg" align="left">
															<asp:Literal ID="lDateCreated" runat="server" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">更新日</td>
														<td class="edit_item_bg" align="left">
															<asp:Label ID="lDateChanged" runat="server" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">最終更新者</td>
														<td class="edit_item_bg" align="left">
															<asp:Label ID="lLastChanged" runat="server" />
														</td>
													</tr>
												</table>
												<br />
												</div>
												<%--△ 基本情報 △--%>
												<%--▽ ユーザー情報 ▽--%>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<%--▽ エラー表示 ▽--%>
													<tr id="trUserIntegrationErrorMessagesTitle" runat="server" visible="false">
														<td class="edit_title_bg" align="center" colspan="2">エラーメッセージ</td>
													</tr>
													<tr id="trUserIntegrationErrorMessages" runat="server" visible="false">
														<td class="edit_item_bg" align="left" colspan="2">
															<asp:Label ID="lbUserIntegrationErrorMessages" runat="server" ForeColor="red" />
														</td>
													</tr>
													<%--△ エラー表示 △--%>
													<tr>
														<td class="edit_title_bg" align="center">ユーザー情報</td>
													</tr>
													<tr>
														<td class="edit_item_bg" align="center">
															<div style="float:right;margin:5px 4px 5px 0px">
															<asp:DropDownList ID="ddlSort" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSort_SelectedIndexChanged">
																<asp:ListItem Text="ユーザーID/昇順" Value="UserIdASC"></asp:ListItem>
																<asp:ListItem Text="ユーザーID/降順" Value="UserIdDESC"></asp:ListItem>
																<asp:ListItem Text="件作成日/昇順" Value="DateCreatedASC"></asp:ListItem>
																<asp:ListItem Text="作成日/降順" Value="DateCreatedDESC"></asp:ListItem>
																<asp:ListItem Text="更新日/昇順" Value="DateChangedASC"></asp:ListItem>
																<asp:ListItem Text="件更新日/降順" Value="DateChangedDESC"></asp:ListItem>
																<asp:ListItem Text="最終ログイン日時/昇順" Value="DateLastLoggedinASC"></asp:ListItem>
																<asp:ListItem Text="最終ログイン日時/降順" Value="DateLastLoggedinDESC"></asp:ListItem>
															</asp:DropDownList>
															</div>
															<asp:Repeater ID="rUserList" OnItemCommand="rUserList_ItemCommand" runat="server">
																<ItemTemplate>
																	<div style="margin-bottom:10px"></div>
																	<div style="margin-bottom:10px"></div>
																	<table class="edit_table" cellspacing="1" cellpadding="0" border="0" width="740">
																		<tr class="edit_title_bg">
																			<td width="30" align="left">
																				代表
																			</td>
																			<td colspan="4">
																				<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_USER_CONFIRM)) { %>
																					ユーザーID：<a href="javascript:open_window('<%# WebSanitizer.UrlAttrHtmlEncode(CreateUserDetailUrl(((UserIntegrationUserInput)Container.DataItem).UserId)) %>','usercontact','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');"><%# ((UserIntegrationUserInput)Container.DataItem).UserId%></a> 
																				<% } else { %>
																					ユーザーID：<%# ((UserIntegrationUserInput)Container.DataItem).UserId%> 
																				<% } %>
																				サイト: <%# CreateSiteNameOnly(((UserIntegrationUserInput)Container.DataItem))%> 顧客区分: <%# ((UserIntegrationUserInput)Container.DataItem).UserKbnText%> カード保持: <%# ((UserIntegrationUserInput)Container.DataItem).HasCreditCards ? "あり" : "なし"%>
																			</td>
																			<td width="50" align="center">
																				削除
																			</td>
																		</tr>
																		<tr>
																			<td class="edit_item_bg" align="center" rowspan="6">
																				<input type="radio" ID="rbRepresentativeFlg_<%# Container.ItemIndex %>" name="rbRepresentativeFlg" value="<%# Container.ItemIndex %>" onclick="javascript:select_representative_flg();" <%# ((UserIntegrationUserInput)Container.DataItem).IsOnRepresentativeFlg ? "checked" : "" %> <%= (this.UserIntegration.IsNoneStatus == false) ? "disabled" : "" %> />&nbsp;
																			</td>
																			<td class="edit_title_bg" width="120">
																				<%: ReplaceTag("@@User.name.name@@") %></td>
																			<td class="edit_item_bg" width="205">
																				<span class="<%# CreateMarkerCss(Container.ItemIndex, Constants.FIELD_USER_NAME1) %>"><%#: ((UserIntegrationUserInput)Container.DataItem).Name1 %></span><span class="<%# CreateMarkerCss(Container.ItemIndex, Constants.FIELD_USER_NAME2) %>"><%#: ((UserIntegrationUserInput)Container.DataItem).Name2 %></span></td>
																			<td class="edit_title_bg" width="120">
																				<%: ReplaceTag("@@User.name_kana.name@@")%></td>
																			<td class="edit_item_bg" width="205">
																				<span class="<%# CreateMarkerCss(Container.ItemIndex, Constants.FIELD_USER_NAME_KANA1) %>"><%#: ((UserIntegrationUserInput)Container.DataItem).NameKana1 %></span><span class="<%# CreateMarkerCss(Container.ItemIndex, Constants.FIELD_USER_NAME_KANA2) %>"><%#: ((UserIntegrationUserInput)Container.DataItem).NameKana2 %></span></td>
																			<td class="edit_item_bg" align="center" rowspan="6">
																				<asp:Button ID="btnDelete" runat="server" CommandName="delete" Text="  削除  " CommandArgument="<%# Container.ItemIndex %>"  Enabled="<%# (this.UserIntegration.IsNoneStatus) %>" /></td>
																		</tr>
																		<tr>
																			<td class="edit_title_bg">
																				<%: ReplaceTag("@@User.sex.name@@") %></td>
																			<td class="edit_item_bg">
																				<span class="<%# CreateMarkerCss(Container.ItemIndex, Constants.FIELD_USER_SEX) %>"><%#: ((UserIntegrationUserInput)Container.DataItem).SexText %></span></td>
																			<td class="edit_title_bg">
																				<%: ReplaceTag("@@User.birth.name@@") %></td>
																			<td class="edit_item_bg">
																				<span class="<%# CreateMarkerCss(Container.ItemIndex, Constants.FIELD_USER_BIRTH) %>"><%#: DateTimeUtility.ToStringForManager(((UserIntegrationUserInput)Container.DataItem).Birth, DateTimeUtility.FormatType.ShortDate2Letter, "-") %></span></td>
																		</tr>
																		<tr>
																			<td class="edit_title_bg">
																				住所</td>
																			<td class="edit_item_bg" colspan="3">
																				<%#: ((Constants.GLOBAL_OPTION_ENABLE == false) || ((UserIntegrationUserInput)Container.DataItem).IsAddrJp) ? "〒" : "" %>
																				<span class="<%# CreateMarkerCss(Container.ItemIndex, Constants.FIELD_USER_ZIP) %>" visible='<%# IsCountryJp(((UserIntegrationUserInput)Container.DataItem).AddrCountryIsoCode) %>' runat="server">
																					<%# WebSanitizer.HtmlEncode(((UserIntegrationUserInput)Container.DataItem).Zip) %></span>
																				 <span class="<%# CreateMarkerCss(Container.ItemIndex, Constants.FIELD_USER_ADDR1) %>"><%#: ((UserIntegrationUserInput)Container.DataItem).Addr1 %></span><span class="<%# CreateMarkerCss(Container.ItemIndex, Constants.FIELD_USER_ADDR2) %>"><%#: ((UserIntegrationUserInput)Container.DataItem).Addr2 %></span><span class="<%# CreateMarkerCss(Container.ItemIndex, Constants.FIELD_USER_ADDR3) %>"><%#: ((UserIntegrationUserInput)Container.DataItem).Addr3 %></span><span class="<%# CreateMarkerCss(Container.ItemIndex, Constants.FIELD_USER_ADDR4) %>"><%#: ((UserIntegrationUserInput)Container.DataItem).Addr4 %></span>
																				<span class="<%# CreateMarkerCss(Container.ItemIndex, Constants.FIELD_USER_ADDR5) %>">
																				<%#: ((UserIntegrationUserInput)Container.DataItem).Addr5 %></span>
																				<span class="<%# CreateMarkerCss(Container.ItemIndex, Constants.FIELD_USER_ZIP) %>" visible='<%# IsCountryJp(((UserIntegrationUserInput)Container.DataItem).AddrCountryIsoCode) %>' runat="server">
																					<%#: ((UserIntegrationUserInput)Container.DataItem).Zip %></span>
																				<span class="<%# CreateMarkerCss(Container.ItemIndex, Constants.FIELD_USER_ADDR_COUNTRY_NAME) %>"><%#: ((UserIntegrationUserInput)Container.DataItem).AddrCountryName %></span>
																			</td>
																		</tr>
																		<tr>
																			<td class="edit_title_bg">
																				<%: ReplaceTag("@@User.tel1.name@@") %></td>
																			</td>
																			<td class="edit_item_bg">
																				<span class="<%# CreateMarkerCss(Container.ItemIndex, Constants.FIELD_USER_TEL1) %>"><%#: ((UserIntegrationUserInput)Container.DataItem).Tel1 != "" ? ((UserIntegrationUserInput)Container.DataItem).Tel1 : "-" %></span></td>
																			<td class="edit_title_bg">
																				<%: ReplaceTag("@@User.tel2.name@@") %></td>
																			</td>
																			<td class="edit_item_bg">
																				<span class="<%# CreateMarkerCss(Container.ItemIndex, Constants.FIELD_USER_TEL2) %>"><%#: ((UserIntegrationUserInput)Container.DataItem).Tel2 != "" ? ((UserIntegrationUserInput)Container.DataItem).Tel2 : "-" %></span></td>
																		</tr>
																		<tr>
																			<td class="edit_title_bg">
																				<%: ReplaceTag("@@User.mail_addr.name@@") %></td>
																			<td class="edit_item_bg">
																				<span class="<%# CreateMarkerCss(Container.ItemIndex, Constants.FIELD_USER_MAIL_ADDR) %>"><%#: ((UserIntegrationUserInput)Container.DataItem).MailAddr != "" ? ((UserIntegrationUserInput)Container.DataItem).MailAddr : "-" %></span></td>
																			<td class="edit_title_bg">
																				<%: ReplaceTag("@@User.mail_addr2.name@@") %></td>
																			<td class="edit_item_bg">
																				<span class="<%# CreateMarkerCss(Container.ItemIndex, Constants.FIELD_USER_MAIL_ADDR2) %>"><%#: ((UserIntegrationUserInput)Container.DataItem).MailAddr2 != "" ? ((UserIntegrationUserInput)Container.DataItem).MailAddr2 : "-" %></span></td>
																		</tr>
																		<tr>
																			<td class="edit_title_bg">
																				作成日時
																			</td>
																			<td class="edit_item_bg">
																				<%#: DateTimeUtility.ToStringForManager(((UserIntegrationUserInput)Container.DataItem).UserDateCreated, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %>
																			</td>
																			<td class="edit_title_bg">
																				更新日時
																			</td>
																			<td class="edit_item_bg">
																				<%#: DateTimeUtility.ToStringForManager(((UserIntegrationUserInput)Container.DataItem).UserDateChanged, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %>
																			</td>
																		</tr>
																	</table>
																</ItemTemplate>
															</asp:Repeater>
															<div style="float:right;margin:5px 4px 5px 0px">
																<% if (this.UserIntegration.IsNoneStatus) { %>
																<input id="inputSearchUser" type="button" value="  ユーザー追加  " onclick="javascript:open_window('<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_USER_LIST) %>','user_search','width=850,height=610,top=120,left=320,status=NO,scrollbars=yes');" />
																<% } %>
																<asp:HiddenField id="hfUserId" runat="server" />
																<p style="display:none"><asp:Button  ID="btnAddUser" runat="server"  Text="  ユーザー追加  " onclick="btnAddUser_Click" /></p>
															</div>
														</td>
													</tr>
												</table>
												<%--△ ユーザー情報 △--%>
												<%--▽ ユーザー統合履歴一覧 ▽--%>
												<div id="divHistores" runat="server">
												<div style="margin-bottom:10px"></div>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="edit_title_bg">
														<td align="center">ユーザー統合履歴情報</td>
													</tr>
													<tr class="edit_item_bg" align="center">
														<td>
															<div style="margin-bottom:5px"></div>
															<table class="info_table" cellspacing="1" cellpadding="0" border="0" width="740">
																<tr class="info_item_bg">
																	<td align="left">
																		以下の情報が代表ユーザー（ユーザーID：<a href="javascript:open_window('<%: CreateUserDetailUrl(lUserId.Text) %>','usercontact','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');"><asp:Literal ID="lUserId" runat="server" /></a>）に統合されました。<br/>
																		<% if (Constants.W2MP_COUPON_OPTION_ENABLED){ %>
																			<span style="color:red">全てのクーポンが統合されています。重複排除が必要なクーポンについては個別で削除をお願いいたします。</span>
																		<% } %>
																	</td>
																</tr>
															</table>
															<asp:Repeater id="rHistores" ItemType="UserIntegrationUserInput" runat="server">
																<ItemTemplate>
																	<div style="margin-bottom:10px"></div>
																	<table class="edit_table" cellspacing="1" cellpadding="0" border="0" width="740">
																		<tr class="edit_title_bg">
																			<td align="left">
																				<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_USER_CONFIRM)) { %>
																					ユーザーID：<a href="javascript:open_window('<%#: CreateUserDetailUrl(Item.UserId) %>','usercontact','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');"><%#: Item.UserId %></a>
																				<% } else { %>
																					ユーザーID：<%#: Item.UserId %>
																				<% } %>
																			</td>
																		</tr>
																		<tr class="edit_item_bg">
																			<td>
																					<%--▽ 注文情報 ▽--%>
																					<div style="height:95px; width:350px; overflow-y:scroll; float: left;margin:5px;">
																					<table class="list_table" cellspacing="1" cellpadding="3" width="330" border="0">
																						<tr class="list_title_bg">
																							<td align="center" colspan="2">注文情報</td>
																						</tr>
																						<tr class="list_title_bg">
																							<td align="center" width="150">作成日時</td>
																							<td align="center" width="150">注文ID</td>
																						</tr>
																					<asp:Repeater runat="server" ItemType="UserIntegrationHistoryInput" DataSource="<%# Item.Histories.Where(h => h.TableName == Constants.TABLE_ORDER).OrderByDescending(h => h.BranchNo).ToArray() %>">
																					<ItemTemplate>
																						<tr class="list_item_bg1">
																							<td align="center"><%#: DateTime.Parse(Item.DateCreated) %></td>
																							<td align="center">
																								<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_ORDER_CONFIRM)) { %>
																									<a href="javascript:open_window('<%#: OrderPage.CreateOrderDetailUrl(Item.PrimaryKey1, true, false, "") %>','order','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');"><%#: Item.PrimaryKey1 %></a>
																								<% } else { %>
																									<%#: Item.PrimaryKey1 %>
																								<% } %>
																							</td>
																						</tr>
																					</ItemTemplate>
																					</asp:Repeater>
																						<tr class="list_item_bg1" runat="server" visible="<%# Item.Histories.Where(h => h.TableName == Constants.TABLE_ORDER).ToArray().Length == 0 %>">
																							<td align="center" colspan="2"><%= WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST) %></td>
																						</tr>
																					</table>
																					</div>
																					<%--△ 注文情報 △--%>
																					<%--▽ クレジットカード情報 ▽--%>
																					<div style="height:95px; width:350px; overflow-y:scroll; float: left;margin:5px;">
																						<table class="list_table" cellspacing="1" cellpadding="3" width="330" border="0">
																							<tr class="list_title_bg">
																								<td align="center" colspan="3">クレジットカード情報</td>
																							</tr>
																							<tr class="list_title_bg">
																								<td align="center" width="150">作成日時</td>
																								<td align="center" width="75">登録名</td>
																								<td align="center" width="75">下4桁</td>
																							</tr>
																							<asp:Repeater ID="Repeater1" runat="server" ItemType="UserIntegrationHistoryInput" DataSource="<%# Item.Histories.Where(h => h.TableName == Constants.TABLE_USERCREDITCARD).OrderByDescending(h => h.BranchNo).ToArray() %>">
																								<ItemTemplate>
																									<tr class="list_item_bg1" runat="server" visible="<%# Item.UserCreditCard.IsDisp %>">
																										<td align="center"><%#: DateTime.Parse(Item.DateCreated) %></td>
																										<td align="center"><%#: Item.UserCreditCard.CardDispName %></td>
																										<td align="center"><%#: Item.UserCreditCard.LastFourDigit %></td>
																									</tr>
																								</ItemTemplate>
																							</asp:Repeater>
																							<tr id="Tr1" class="list_item_bg1" runat="server" visible="<%# Item.Histories.Where(h => h.TableName == Constants.TABLE_USERCREDITCARD).ToArray().Length == 0 %>">
																								<td align="center" colspan="2"><%= WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST) %></td>
																							</tr>
																						</table>
																					</div>
																					<%--△ 注文情報 △--%>
																					<%--▽ 定期購入情報 ▽--%>
																					<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED){ %>
																					<div style="height:95px; width:350px; overflow-y:scroll; float: left;margin:5px;">
																					<table class="list_table" cellspacing="1" cellpadding="3" width="330" border="0">
																						<tr class="list_title_bg">
																							<td align="center" colspan="2">定期購入情報</td>
																						</tr>
																						<tr class="list_title_bg">
																							<td align="center" width="150">作成日時</td>
																							<td align="center" width="150">定期購入ID</td>
																						</tr>
																					<asp:Repeater runat="server" ItemType="UserIntegrationHistoryInput" DataSource="<%# Item.Histories.Where(h => h.TableName == Constants.TABLE_FIXEDPURCHASE).OrderByDescending(h => h.BranchNo).ToArray() %>">
																					<ItemTemplate>
																						<tr class="list_item_bg1">
																							<td align="center"><%#: DateTime.Parse(Item.DateCreated) %></td>
																							<td align="center">
																								<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_FIXEDPURCHASE_CONFIRM)) { %>
																									<a href="javascript:open_window('<%#: FixedPurchasePage.CreateFixedPurchaseDetailUrl(Item.PrimaryKey1, true) %>','fixedpurchase','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');"><%#: Item.PrimaryKey1 %></a>
																								<% } else { %>
																									<%#: Item.PrimaryKey1 %>
																								<% } %>
																							</td>
																						</tr>
																					</ItemTemplate>
																					</asp:Repeater>
																						<tr class="list_item_bg1" runat="server" visible="<%# Item.Histories.Where(h => h.TableName == Constants.TABLE_FIXEDPURCHASE).ToArray().Length == 0 %>">
																							<td align="center" colspan="2"><%= WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST) %></td>
																						</tr>
																					</table>
																					</div>
																					<% } %>
																					<%--△ 定期購入情報 △--%>
																					<%--▽ ユーザーポイント情報 ▽--%>
																					<% if (Constants.W2MP_POINT_OPTION_ENABLED){ %>
																					<div style="height:95px; width:350px; overflow-y:scroll; float: left;margin:5px;">
																					<table class="list_table" cellspacing="1" cellpadding="3" width="330" border="0">
																						<tr class="list_title_bg">
																							<td align="center" colspan="3">ユーザーポイント情報</td>
																						</tr>
																						<tr class="list_title_bg">
																							<td align="center" width="150">作成日時</td>
																							<td align="center" width="75">本/仮ポイント</td>
																							<td align="center" width="75">移行ポイント</td>
																						</tr>
																					<asp:Repeater runat="server" ItemType="UserIntegrationHistoryInput" DataSource="<%# Item.Histories.Where(h => h.TableName == Constants.TABLE_USERPOINT).OrderByDescending(h => h.BranchNo).ToArray() %>">
																					<ItemTemplate>
																						<tr class="list_item_bg1">
																							<td align="center"><%#: DateTime.Parse(Item.DateCreated) %></td>
																							<td align="center"><%#: Item.UserPointHistory.PointTypeText %></td>
																							<td align="center"><%#: Item.UserPointHistory.PointInc %></td>
																						</tr>
																					</ItemTemplate>
																					</asp:Repeater>
																						<tr class="list_item_bg1" runat="server" visible="<%# Item.Histories.Where(h => h.TableName == Constants.TABLE_USERPOINT).ToArray().Length == 0 %>">
																							<td align="center" colspan="3"><%= WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST) %></td>
																						</tr>
																					</table>
																					</div>
																					<% } %>
																					<%--△ ユーザーポイント情報 △--%>
																					<%--▽ インシデント情報 ▽--%>
																					<% if (Constants.CS_OPTION_ENABLED){ %>
																					<div style="height:95px; width:350px; overflow-y:scroll; float: left;margin:5px;">
																					<table class="list_table" cellspacing="1" cellpadding="3" width="330" border="0">
																						<tr class="list_title_bg">
																							<td align="center" colspan="2">インシデント情報</td>
																						</tr>
																						<tr class="list_title_bg">
																							<td align="center" width="150">作成日時</td>
																							<td align="center" width="150">インシデントID</td>
																						</tr>
																					<asp:Repeater runat="server" ItemType="UserIntegrationHistoryInput" DataSource="<%# Item.Histories.Where(h => h.TableName == Constants.TABLE_CSINCIDENT).OrderByDescending(h => h.BranchNo).ToArray() %>">
																					<ItemTemplate>
																						<tr class="list_item_bg1">
																							<td align="center"><%#: DateTime.Parse(Item.DateCreated) %></td>
																							<td align="center"><a href="javascript:open_window('<%#: SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Cs, HttpUtility.UrlEncode(CreateCsMessageUrl(Item.PrimaryKey2))) %>','incident','width=1200,height=770,top=110,left=380,status=NO,scrollbars=yes');"><%#: Item.PrimaryKey2 %></a></td>
																						</tr>
																					</ItemTemplate>
																					</asp:Repeater>
																						<tr class="list_item_bg1" runat="server" visible="<%# Item.Histories.Where(h => h.TableName == Constants.TABLE_CSINCIDENT).ToArray().Length == 0 %>">
																							<td align="center" colspan="2"><%= WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST) %></td>
																						</tr>
																					</table>
																					</div>
																					<% } %>
																					<%--△ インシデント情報 △--%>
																					<%--▽ クーポン情報 ▽--%>
																					<% if (Constants.W2MP_COUPON_OPTION_ENABLED){ %>
																					<div style="height:95px; width:350px; overflow-y:scroll; float: left;margin:5px;">
																					<table class="list_table" cellspacing="1" cellpadding="3" width="330" border="0">
																						<tr class="list_title_bg">
																							<td align="center" colspan="2">クーポン情報</td>
																						</tr>
																						<tr class="list_title_bg">
																							<td align="center" width="150">作成日時</td>
																							<td align="center" width="150">クーポンコード</td>
																						</tr>
																					<asp:Repeater runat="server" ItemType="UserIntegrationHistoryInput" DataSource="<%# Item.Histories.Where(h => h.TableName == Constants.TABLE_USERCOUPON).OrderByDescending(h => h.BranchNo).ToArray() %>">
																					<ItemTemplate>
																						<tr class="list_item_bg1">
																							<td align="center"><%#: DateTime.Parse(Item.DateCreated) %></td>
																							<td align="center"><%#: Item.PrimaryKey4 %></td>
																						</tr>
																					</ItemTemplate>
																					</asp:Repeater>
																						<tr class="list_item_bg1" runat="server" visible="<%# Item.Histories.Where(h => h.TableName == Constants.TABLE_USERCOUPON).ToArray().Length == 0 %>">
																							<td align="center" colspan="2"><%= WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST) %></td>
																						</tr>
																					</table>
																					</div>
																					<% } %>
																					<%--△ クーポン情報 △--%>
																					<%--▽ メール送信ログ情報 ▽--%>
																					<% if (Constants.CS_OPTION_ENABLED){ %>
																					<div style="height:95px; width:350px; overflow-y:scroll; float: left;margin:5px;">
																					<table class="list_table" cellspacing="1" cellpadding="3" width="330" border="0">
																						<tr class="list_title_bg">
																							<td align="center" colspan="2">メール送信ログ情報</td>
																						</tr>
																						<tr class="list_title_bg">
																							<td align="center" width="150">作成日時</td>
																							<td align="center" width="150">ログNo</td>
																						</tr>
																					<asp:Repeater runat="server" ItemType="UserIntegrationHistoryInput" DataSource="<%# Item.Histories.Where(h => h.TableName == Constants.TABLE_MAILSENDLOG).OrderByDescending(h => h.BranchNo).ToArray() %>">
																					<ItemTemplate>
																						<tr class="list_item_bg1">
																							<td align="center"><%#: DateTime.Parse(Item.DateCreated) %></td>
																							<td align="center"><a href="javascript:open_window('<%#: SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Cs, HttpUtility.UrlEncode(CreateCsMailSendLogConfirmUrl(Item.PrimaryKey1))) %>','mailsendlog','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');"><%#: Item.PrimaryKey1 %></a></td>
																						</tr>
																					</ItemTemplate>
																					</asp:Repeater>
																						<tr class="list_item_bg1" runat="server" visible="<%# Item.Histories.Where(h => h.TableName == Constants.TABLE_MAILSENDLOG).ToArray().Length == 0 %>">
																							<td align="center" colspan="2"><%= WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST) %></td>
																						</tr>
																					</table>
																					</div>
																					<% } %>
																					<%--△ メール送信ログ情報 △--%>
																					<%--▽ DM発送履歴情報 ▽--%>
																					<% if (Constants.DM_SHIPPING_HISTORY_OPTION_ENABLED){ %>
																					<div style="height:95px; width:350px; overflow-y:scroll; float: left;margin:5px;">
																						<table class="list_table" cellspacing="1" cellpadding="3" width="330" border="0">
																							<tr class="list_title_bg">
																								<td align="center" colspan="2">DM発送履歴</td>
																							</tr>
																							<tr class="list_title_bg">
																								<td align="center" width="150">作成日時</td>
																								<td align="center" width="150">DMコード</td>
																							</tr>
																							<asp:Repeater runat="server" ItemType="UserIntegrationHistoryInput" DataSource="<%# Item.Histories.Where(h => h.TableName == Constants.TABLE_DMSHIPPINGHISTORY).OrderByDescending(h => h.BranchNo).ToArray() %>">
																								<ItemTemplate>
																									<tr class="list_item_bg1">
																										<td align="center"><%#: DateTime.Parse(Item.DateCreated) %></td>
																										<td align="center"><%#: Item.PrimaryKey2 %></td>
																									</tr>
																								</ItemTemplate>
																							</asp:Repeater>
																							<tr class="list_item_bg1" runat="server" visible="<%# Item.Histories.Where(h => h.TableName == Constants.TABLE_DMSHIPPINGHISTORY).ToArray().Length == 0 %>">
																								<td align="center" colspan="2"><%= WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST) %></td>
																							</tr>
																						</table>
																					</div>
																					<% } %>
																					<%--△ DM発送履歴情報 △--%>
																			</td>
																		</tr>
																	</table>
																	<div style="margin-bottom:10px"></div>
																</ItemTemplate>
															</asp:Repeater>
														</td>
													</tr>
												</table>
												</div>
												<%--△ ユーザー統合履歴一覧 △--%>
												<div class="action_part_bottom">
													<asp:Button ID="btnToListBottom" runat="server" Text="  一覧へ戻る  " OnClick="btnToList_Click" />
													<asp:Button id="btnNoneBottom" runat="server" Text="  解除する  " OnClick="btnNone_Click" />
													<asp:Button id="btnSuspendBottom" runat="server" Text="  保留する  " OnClick="btnSuspend_Click" />
													<asp:Button id="btnDoneBottom" runat="server" Text="  統合する  " OnClick="btnDone_Click" />
													<asp:Button id="btnExcludedBottom" runat="server" Text="  除外する  " OnClick="btnExcluded_Click" />
												</div>
											</td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="info_item_bg">
														<td align="left">
															【備考】<br />
															・代表ユーザーと一致していない項目は<span class="marker_ps">　　　　　　　　</span>となっています。<br /><br />
															【統合の説明】<br />
															・代表ユーザーに非代表ユーザーの注文情報<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED){ %>、定期購入情報<% } %><% if (Constants.W2MP_POINT_OPTION_ENABLED){ %>、ユーザーポイント情報<% } %><% if (Constants.CS_OPTION_ENABLED){ %>、インシデント情報、メール送信ログ情報<% } %>が統合されます。<br />
															・会員ユーザーが存在する場合、ゲストユーザーを代表ユーザーにすることはできません。<br />
															・既に統合済みの非代表ユーザーを再統合することはできません。<br />
															<br />
															【統合解除の説明】<br />
															・代表ユーザーに統合された注文情報<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED){ %>、定期購入情報<% } %><% if (Constants.W2MP_POINT_OPTION_ENABLED){ %>、ユーザーポイント情報<% } %><% if (Constants.CS_OPTION_ENABLED){ %>、インシデント情報、メール送信ログ情報<% } %>が解除されます。<br />
															・統合解除は統合を行った逆の順序で行う必要があります。<br />
															&nbsp;&nbsp;[例]以下のような順で統合を実施した場合<br />
															&nbsp;&nbsp;(1)ユーザーA（代表）, ユーザーB（非代表）を統合<br />
															&nbsp;&nbsp;(2)ユーザーA（代表）, ユーザーC（非代表）を統合<br />
															&nbsp;&nbsp;「(1)」の統合解除は「(2)」の統合解除を行わないとできません。<br />
															<% if (Constants.W2MP_POINT_OPTION_ENABLED){ %>・非代表ユーザーに仮ポイントがある状態で統合し代表ユーザーで本ポイントに移行された場合、<br />
															&nbsp;&nbsp;統合解除処理で移行されたポイントの戻しは行われません。<br /><% } %>
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
	<!--△ 登録/更新 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
