<%--
=========================================================================================================
  Module      : ユーザー統合一覧ページ(UserIntegrationList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.Domain.UserIntegration.Setting" %>
<%@ Import Namespace="w2.Domain.UserIntegration.Helper" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="UserIntegrationList.aspx.cs" Inherits="Form_UserIntegration_UserIntegrationList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<style type="text/css">
.list_table td {
	word-break:break-all;
}
</style>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">ユーザー統合</h1></td>
	</tr>
	<tr>
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
														<td class="search_title_bg" width="130">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															ユーザー統合ステータス</td>
														<td class="search_item_bg" colspan="3">
															<asp:RadioButtonList id="rblStatus" runat="server" RepeatDirection="Horizontal" RepeatLayout="Table" cssclass="chkBoxList" RepeatColumns="4" /></td>
														<td class="search_title_bg">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															並び順</td>
														<td class="search_item_bg">
															<asp:DropDownList id="ddlSortKbn" runat="server" CssClass="search_item_width" Width="115"></asp:DropDownList></td>
														<td class="search_btn_bg" width="63" rowspan="5">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_USER_INTEGRATION_LIST %>">クリア</a>&nbsp;
																<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a></div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															ユーザー統合No</td>
														<td class="search_item_bg" colspan="5">
															<asp:TextBox id="tbUserIntegrationNo" runat="server" Width="110" /></td>
													</tr>
													<tr>
														<td class="search_title_bg">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															ユーザーID</td>
														<td class="search_item_bg" width="110">
															<asp:TextBox id="tbUserId" runat="server" Width="110" /></td>
														<td class="search_title_bg" width="90">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															<%: ReplaceTag("@@User.name.name@@") %></td>
														<td class="search_item_bg" width="110">
															<asp:TextBox id="tbName" runat="server" Width="110" /></td>
														<td class="search_title_bg" width="90">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															<%: ReplaceTag("@@User.name_kana.name@@") %></td>
														<td class="search_item_bg" width="110">
															<asp:TextBox id="tbNameKana" runat="server" Width="110" /></td>
													</tr>
													<tr>
														<td class="search_title_bg">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															<%: ReplaceTag("@@User.tel1.name@@") %></td>
														<td class="search_item_bg">
															<asp:TextBox id="tbTel" runat="server" Width="110"></asp:TextBox></td>
														<td class="search_title_bg">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															メールアドレス</td>
														<td class="search_item_bg" colspan="3">
															<asp:TextBox id="tbMailAddr" runat="server" Width="110"></asp:TextBox></td>
													</tr>
													<tr>
														<td class="search_title_bg" width="80">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															<%: ReplaceTag("@@User.zip.name@@") %></td>
														<td class="search_item_bg" width="120">
															<asp:TextBox id="tbZip" runat="server" Width="110"></asp:TextBox></td>
														<td class="search_title_bg" width="80">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															住所</td>
														<td class="search_item_bg" colspan="3">
															<asp:TextBox id="tbAddr" runat="server" Width="327"></asp:TextBox></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr><td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
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
		<td><h2 class="cmn-hed-h2">ユーザー統合一覧</h2></td>
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
														<td width="675" style="height: 22px">
															<asp:Label id="lbPager" Runat="server" />
														</td>
														<td width="83" class="action_list_sp">
															<asp:Button ID="btnInsertTop" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" />
														</td>
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
														<td align="center" width="50">ユーザー<br/>統合No</td>
														<td align="center">ユーザー情報</td>
													</tr>
													<asp:Repeater id="rList" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateUserIntegrationRegisterlUrl(((UserIntegrationListSearchResult)Container.DataItem).UserIntegrationNo.ToString(), Constants.ACTION_STATUS_UPDATE)) %>')">
																<td align="center"><%# ((UserIntegrationListSearchResult)Container.DataItem).UserIntegrationNo%></td>
																<td>
																	<asp:Repeater ID="rUserDataList" runat="server" DataSource="<%# ((UserIntegrationListSearchResult)Container.DataItem).Users.OrderByDescending(u => u.RepresentativeFlg).ThenByDescending(u => u.UserId) %>">
																		<ItemTemplate>
																			<div style="margin-bottom:10px"></div>
																			<table class="list_table" cellspacing="1" cellpadding="3" width="690" border="0">																			
																				<tr>
																					<td class="list_title_bg" colspan="4">
																						ユーザーID：<%# WebSanitizer.HtmlEncode(((UserIntegrationUserListSearchResult)Container.DataItem).UserId)%> サイト: <%# WebSanitizer.HtmlEncode(CreateSiteNameOnly(((UserIntegrationUserListSearchResult)Container.DataItem).MallId, ((UserIntegrationUserListSearchResult)Container.DataItem).MallName)) %> 顧客区分: <%# WebSanitizer.HtmlEncode(((UserIntegrationUserListSearchResult)Container.DataItem).UserKbnText) %> カード保持: <%# WebSanitizer.HtmlEncode(((UserIntegrationUserListSearchResult)Container.DataItem).HasCreditCards ? "あり" : "なし") %>
																					</td>
																				</tr>
																				<tr>
																					<td class="list_title_bg" width="110">
																						<%: ReplaceTag("@@User.name.name@@") %></td>
																					<td class="list_item_bg1" width="230">
																						<%# WebSanitizer.HtmlEncode(((UserIntegrationUserListSearchResult)Container.DataItem).Name) %></td>
																					<td class="list_title_bg" width="110">
																						<%: ReplaceTag("@@User.name_kana.name@@")%></td>
																					<td class="list_item_bg1" width="230">
																						<%# WebSanitizer.HtmlEncode(((UserIntegrationUserListSearchResult)Container.DataItem).NameKana) %></td>
																				</tr>
																				<tr>
																					<td class="list_title_bg">
																						<%: ReplaceTag("@@User.sex.name@@") %></td>
																					<td class="list_item_bg1">
																						<%# WebSanitizer.HtmlEncode(((UserIntegrationUserListSearchResult)Container.DataItem).SexText) %></td>
																					<td class="list_title_bg">
																						<%: ReplaceTag("@@User.birth.name@@") %></td>
																					<td class="list_item_bg1">
																						<%#: DateTimeUtility.ToStringForManager(((UserIntegrationUserListSearchResult)Container.DataItem).Birth, DateTimeUtility.FormatType.ShortDate2Letter, "-") %></td>
																				</tr>
																				<tr>
																					<td class="list_title_bg">
																						住所</td>
																					<td class="list_item_bg1" colspan="3">
																						<%# IsCountryJp(((UserIntegrationUserListSearchResult)Container.DataItem).AddrCountryIsoCode)
																							? WebSanitizer.HtmlEncode("〒" + (((UserIntegrationUserListSearchResult)Container.DataItem).Zip) + " " + (((UserIntegrationUserListSearchResult)Container.DataItem).Addr))
																							: WebSanitizer.HtmlEncode(((UserIntegrationUserListSearchResult)Container.DataItem).AddrGlobal)%>
																					</td>
																				</tr>
																				<tr>
																					<td class="list_title_bg">
																						<%: ReplaceTag("@@User.tel1.name@@") %></td>
																					<td class="list_item_bg1">
																						<%# WebSanitizer.HtmlEncode(((UserIntegrationUserListSearchResult)Container.DataItem).Tel1 != "" ?  ((UserIntegrationUserListSearchResult)Container.DataItem).Tel1 : "-") %></td>
																					<td class="list_title_bg">
																						<%: ReplaceTag("@@User.tel2.name@@") %></td>
																					<td class="list_item_bg1">
																						<%# WebSanitizer.HtmlEncode(((UserIntegrationUserListSearchResult)Container.DataItem).Tel2 != "" ?  ((UserIntegrationUserListSearchResult)Container.DataItem).Tel2 : "-") %></td>
																				</tr>
																				<tr>
																					<td class="list_title_bg">
																						<%: ReplaceTag("@@User.mail_addr.name@@") %></td>
																					<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
																					<td class="list_item_bg1">
																					<% }else{ %>
																					<td class="list_item_bg1"  colspan="3">
																					<% } %>
																						<%# WebSanitizer.HtmlEncode(((UserIntegrationUserListSearchResult)Container.DataItem).MailAddr != "" ? ((UserIntegrationUserListSearchResult)Container.DataItem).MailAddr : "-") %></td>
																					<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
																					<td class="list_title_bg">
																						<%: ReplaceTag("@@User.mail_addr2.name@@") %></td>
																					<td class="list_item_bg1">
																						<%# WebSanitizer.HtmlEncode(((UserIntegrationUserListSearchResult)Container.DataItem).MailAddr2 != "" ? ((UserIntegrationUserListSearchResult)Container.DataItem).MailAddr2 : "-") %></td>
																					<% } %>
																				</tr>
																				<tr>
																					<td class="list_title_bg">
																						作成日時</td>
																					<td class="list_item_bg1">
																						<%#: DateTimeUtility.ToStringForManager(((UserIntegrationUserListSearchResult)Container.DataItem).UserDateCreated, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
																					<td class="list_title_bg">
																						更新日時</td>
																					<td class="list_item_bg1">
																						<%#: DateTimeUtility.ToStringForManager(((UserIntegrationUserListSearchResult)Container.DataItem).UserDateCreated, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
																				</tr>
																			</table>
																		</ItemTemplate>
																	</asp:Repeater>
																	<div style="margin-bottom:10px"></div>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" colspan="2" runat="server">
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
										</tr>
										<tr>
											<td class="action_list_sp">
												<asp:Button id="btnInsertBottom" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" />
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
															【バッチ処理による自動登録について】<br />
															・以下の名寄せルールに基づき自動的にユーザー統合情報を登録します。<br />
															<%= UserIntegrationSetting.Description %><br/>
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
