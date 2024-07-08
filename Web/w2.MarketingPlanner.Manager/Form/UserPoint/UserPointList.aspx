<%--
=========================================================================================================
  Module      : ユーザポイント一覧ページ(UserPointList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="UserPointList.aspx.cs" Inherits="Form_UserPoint_UserPointList" %>
<%@ Import Namespace="w2.App.Common.Extensions" %>
<%@ Import Namespace="w2.Domain.MenuAuthority.Helper" %>
<%@ Import Namespace="w2.App.Common.Manager" %>
<%-- マスタ出力コントロール --%>
<%@ Register TagPrefix="uc" TagName="MasterDownLoad" Src="~/Form/Common/MasterDownLoadPanel.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">ユーザーポイント情報</h1></td>
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
																border="0" />ポイント区分</td>
														<td class="search_item_bg" colspan="5">
															<asp:RadioButtonList id="rblPointKbn" runat="server" Width="300" RepeatDirection="Horizontal" RepeatLayout="Flow">
															</asp:RadioButtonList>
														</td>
														<td class="search_btn_bg" width="83" rowspan="4">
															<div class="search_btn_main"><asp:Button id="btnSearch" runat="server" Text="  検索  " onclick="btnSearch_Click"></asp:Button></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_USERPOINT_LIST %>">クリア</a>
																<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a>
															</div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />検索項目</td>
														<td class="search_item_bg" width="130"><asp:DropDownList id="ddlSearchKey" runat="server"></asp:DropDownList></td>
														<td class="search_title_bg" width="95"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />検索値</td>
														<td class="search_item_bg" width="130"><asp:TextBox id="tbSearchWord" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="95"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />並び順</td>
														<td class="search_item_bg" width="130"><asp:DropDownList id="ddlSortKbn" runat="server">
																<asp:ListItem Value="8">ユーザID/昇順</asp:ListItem>
																<asp:ListItem Value="9" Selected="True">ユーザID/降順</asp:ListItem>
																<asp:ListItem Value="0">ポイント/昇順</asp:ListItem>
																<asp:ListItem Value="1">ポイント/降順</asp:ListItem>
																<asp:ListItem Value="2">有効期限/昇順</asp:ListItem>
																<asp:ListItem Value="3">有効期限/降順</asp:ListItem>
															</asp:DropDownList>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>

                                        <%-- マスタ出力 --%>
										<tr>
											<td class="search_table">
												<uc:MasterDownLoad runat="server" ID="uMasterDownload" DownloadType="UserPoint" TableWidth="758" />
											</td>
										</tr>

										<tr>
											<td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
	<tr id="trList" runat="server">
		<td>
			<h2 class="cmn-hed-h2">ユーザーポイント情報一覧</h2>
			<% if (Constants.CROSS_POINT_OPTION_ENABLED) { %>
			<span class="cp_annotation_message">
				※表示されているポイントは最新ではない可能性があります。
				<br/>CROSSPOINT管理画面をご確認ください。
			</span>
			<% } %>
		</td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<div id="divPointEdit" runat="server">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<% if (this.IsNotSearchDefault == false) { %>
											<tr>
												<td>
													<!--▽ ページング ▽-->
													<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
														<tr>
															<td><asp:label id="lbPager1" Runat="server"></asp:label></td>
															<td class="action_list_sp"><asp:Button id="btnPointUpdateTop" runat="server" Text="  このページの一括更新  " OnClientClick="return exec_submit()"　onclick="btnPointUpdateTop_Click"></asp:Button></td>
														</tr>
													</table>
													<!--△ ページング △-->
												</td>
											</tr>
											<tr>
												<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<% } %>
											<tr>
												<td>
													<% if (this.IsNotSearchDefault) { %>
													<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr class="list_alert">
															<td><%= WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NOT_SEARCH_DEFAULT) %></td>
														</tr>
													</table>
													<% } else { %>
													<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr class="list_title_bg">
															<td align="center" width="100" rowspan="2">ユーザーID</td>
															<td align="center" width="120" rowspan="2"><%: ReplaceTag("@@User.name.name@@") %></td>
															<td align="center" width="68" rowspan="2">ポイント<br />区分</td>
															<td align="center" width="200" colspan="2">ポイント</td>
															<% if (Constants.CROSS_POINT_OPTION_ENABLED) { %>
															<td align="center" width="270" colspan="1">有効期限</td>
															<% } else { %>
															<td align="center" width="270" colspan="2">有効期限</td>
															<% } %>
															<td align="center" width="100" rowspan="2">注文ID</td>
														</tr>
														<tr class="list_title_bg">
															<td align="center" width="80">現在<br /><span class="small_text"> (仮ポイント)</span></td>
															<td align="center" width="120">（＋－）</td>
															<% if (Constants.CROSS_POINT_OPTION_ENABLED) { %>
															<td align="center" width="270">現在</td>
															<% } else { %>
															<td align="center" width="90">現在</td>
															<td align="center" width="180">有効期限の更新</td>
															<% } %>
														</tr>
														<asp:Repeater id="rEdit" Runat="server">
															<ItemTemplate>
																<tr>
																	<td align="center" class="list_item_bg1">
																		<div id="dvUserIdLink" style="display:<%# HaveUserPointHistory(((WrappedSearchResult)Container.DataItem).UserId) ? "inline" : "none" %>">
																		<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateUserPointHistoryListUrl(((WrappedSearchResult)Container.DataItem).UserId)) %>">
																			<%# WebSanitizer.HtmlEncode(Container.ToModel<WrappedSearchResult>().UserId)%></a>
																		</div>
																		<div id="dvUserIdNoLink" style="display:<%# HaveUserPointHistory(((WrappedSearchResult)Container.DataItem).UserId) ? "none": "inline" %>">
																			<%# WebSanitizer.HtmlEncode(Container.ToModel<WrappedSearchResult>().UserId)%>
																		</div>
																	</td>
																	<td align="left" class="list_item_bg1">&nbsp;
																		<%# WebSanitizer.HtmlEncode(Container.ToModel<WrappedSearchResult>().Name)%></td>
																	<td align="center" class="list_item_bg1">
																		<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_USERPOINT, Constants.FIELD_USERPOINT_POINT_KBN, Container.ToModel<WrappedSearchResult>().PointKbn).Replace("ポイント", string.Empty))%>
																		<%# Container.ToModel<WrappedSearchResult>().ExistsRealPoint == false ? "<span class=\"notice\">*</span>" : ""%></td>
																	<td align="center" class="list_item_bg1">
																		<%# WebSanitizer.HtmlEncode(Container.ToModel<WrappedSearchResult>().ExistsRealPoint ? StringUtility.ToNumeric(Container.ToModel<WrappedSearchResult>().RealPoint) : "0")%>pt
																		<span class="small_text"><%# WebSanitizer.HtmlEncode((Container.ToModel<WrappedSearchResult>().ExistsTempPoint) ? "　（" + StringUtility.ToNumeric(Container.ToModel<WrappedSearchResult>().TempPoint) + "pt）" : "")%></span>
																	</td>
																	<td align="center" class="list_item_bg1">
																		<asp:DropDownList ID="ddlPointOperator1" Runat="server">
																			<asp:ListItem Value="0" Selected="True">＋</asp:ListItem>
																			<asp:ListItem Value="1">－</asp:ListItem>
																		</asp:DropDownList>
																		<asp:TextBox ID="tbPoint1" Runat="server" Width="40" Text="0" MaxLength="5"></asp:TextBox>pt
																	</td>
																	<td align="center" class="list_item_bg1">
																		～<%#: DateTimeUtility.ToStringForManager(Container.ToModel<WrappedSearchResult>().PointExp, DateTimeUtility.FormatType.ShortDate2Letter, DateTimeUtility.ToStringForManager(DateTime.Now.AddYears(1), DateTimeUtility.FormatType.ShortDate2Letter))%></td>
																	<% if (Constants.CROSS_POINT_OPTION_ENABLED == false) { %>
																	<td align="center" class="list_item_bg1">
																		<asp:DropDownList ID="ddlExpDateTimeOperator1" Runat="server">
																			<asp:ListItem Value="0" Selected="True">＋</asp:ListItem>
																			<asp:ListItem Value="1">－</asp:ListItem>
																		</asp:DropDownList>
																		<asp:TextBox ID="tbMonthOfExpDateTime1" Runat="server" Width="25" Text="0" MaxLength="2"></asp:TextBox>ヵ月
																		<asp:TextBox ID="tbExpDateTime1" Runat="server" Width="25" Text="0" MaxLength="2"></asp:TextBox>日
																	</td>
																	<% } %>
																	<td align="center" class="list_item_bg1">
																		<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_ORDER_CONFIRM)) { %>
																			<div style="display:<%# string.IsNullOrEmpty(((WrappedSearchResult)Container.DataItem).OrderId) ? "none" : "inline" %>">
																				<a href="#" onclick="javascript:open_window('<%#: SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Ec, CreateOrderDetailUrl(((WrappedSearchResult)Container.DataItem).OrderId))%>','userdetail','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');"))><%#: ((WrappedSearchResult)Container.DataItem).OrderId %></a>
																			</div>
																			<div style="display:<%# string.IsNullOrEmpty(((WrappedSearchResult)Container.DataItem).OrderId) ? "inline" : "none" %>">
																				－
																			</div>
																		<% } else { %>
																			<%#: string.IsNullOrEmpty(((WrappedSearchResult)Container.DataItem).OrderId) ? "－" : ((WrappedSearchResult)Container.DataItem).OrderId %>
																		<% } %>
																	</td>
																	<%-- ポイントレコードを一意に特定するため、枝番をこっそり隠しておく --%>
																	<asp:HiddenField ID="hfPointKbnNo1" Value="<%#: ((WrappedSearchResult)Container.DataItem).PointKbnNo %>" runat="server"/>
																	<%-- ポイントマスタ取得時にポイント区分が要る --%>
																	<asp:HiddenField ID="hfPointKbn1" Value="<%#: ((WrappedSearchResult)Container.DataItem).PointKbn %>" runat="server"/>
																</tr>
															</ItemTemplate>
															<AlternatingItemTemplate>
																<tr>
																	<td align="center" class="list_item_bg2">
																		<div id="dvUserIdLink" style="display:<%# HaveUserPointHistory(((WrappedSearchResult)Container.DataItem).UserId) ? "inline" : "none" %>">
																		<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateUserPointHistoryListUrl(((WrappedSearchResult)Container.DataItem).UserId)) %>">
																			<%# WebSanitizer.HtmlEncode(((WrappedSearchResult)Container.DataItem).UserId)%></a>
																		</div>
																		<div id="dvUserIdNoLink" style="display:<%# HaveUserPointHistory(((WrappedSearchResult)Container.DataItem).UserId) ? "none" : "inline" %>">
																			<%# WebSanitizer.HtmlEncode(((WrappedSearchResult)Container.DataItem).UserId)%>
																		</div>
																	</td>
																	<td align="left" class="list_item_bg2">&nbsp;
																		<%# WebSanitizer.HtmlEncode(Container.ToModel<WrappedSearchResult>().Name)%></td>
																	<td align="center" class="list_item_bg2">
																		<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_USERPOINT, Constants.FIELD_USERPOINT_POINT_KBN, Container.ToModel<WrappedSearchResult>().PointKbn).Replace("ポイント", string.Empty))%>
																		<%# (((WrappedSearchResult)Container.DataItem).ExistsRealPoint == false) ? "<span class=\"notice\">*</span>":"" %></td>
																	<td align="center" class="list_item_bg2">
																		<%# WebSanitizer.HtmlEncode(Container.ToModel<WrappedSearchResult>().ExistsRealPoint ? StringUtility.ToNumeric(Container.ToModel<WrappedSearchResult>().RealPoint) : "0")%>pt
																		<span class="small_text"><%# WebSanitizer.HtmlEncode(Container.ToModel<WrappedSearchResult>().ExistsTempPoint ? "　（" + StringUtility.ToNumeric(Container.ToModel<WrappedSearchResult>().TempPoint) + "pt）" : "")%></span>
																	</td>
																	<td align="center" class="list_item_bg2">
																		<asp:DropDownList ID="ddlPointOperator2" Runat="server">
																			<asp:ListItem Value="0" Selected="True">＋</asp:ListItem>
																			<asp:ListItem Value="1">－</asp:ListItem>
																		</asp:DropDownList>
																		<asp:TextBox ID="tbPoint2" Runat="server" Width="40" Text="0" MaxLength="5"></asp:TextBox>pt
																	</td>
																	<td align="center" class="list_item_bg2">
																		～<%#: DateTimeUtility.ToStringForManager(Container.ToModel<WrappedSearchResult>().PointExp, DateTimeUtility.FormatType.ShortDate2Letter, DateTimeUtility.ToStringForManager(DateTime.Now.AddYears(1), DateTimeUtility.FormatType.ShortDate2Letter))%></td>
																	<% if(Constants.CROSS_POINT_OPTION_ENABLED == false) { %>
																	<td align="center" class="list_item_bg2">
																		<asp:DropDownList ID="ddlExpDateTimeOperator2" Runat="server">
																			<asp:ListItem Value="0" Selected="True">＋</asp:ListItem>
																			<asp:ListItem Value="1">－</asp:ListItem>
																		</asp:DropDownList>
																		<asp:TextBox ID="tbMonthOfExpDateTime2" Runat="server" Width="25" Text="0" MaxLength="2"></asp:TextBox>ヵ月
																		<asp:TextBox ID="tbExpDateTime2" Runat="server" Width="25" Text="0" MaxLength="2"></asp:TextBox>日
																	</td>
																	<% } %>
																	<td align="center" class="list_item_bg2">
																		<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_ORDER_CONFIRM)) { %>
																			<div style="display:<%# string.IsNullOrEmpty(((WrappedSearchResult)Container.DataItem).OrderId) ? "none" : "inline" %>">
																				<a href="#" onclick="javascript:open_window('<%#: SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Ec, CreateOrderDetailUrl(((WrappedSearchResult)Container.DataItem).OrderId))%>','userdetail','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');"))><%#: ((WrappedSearchResult)Container.DataItem).OrderId %></a>
																			</div>
																			<div style="display:<%# string.IsNullOrEmpty(((WrappedSearchResult)Container.DataItem).OrderId) ? "inline" : "none" %>">
																				－
																			</div>
																		<% } else { %>
																			<%#: string.IsNullOrEmpty(((WrappedSearchResult)Container.DataItem).OrderId) ? "－" : ((WrappedSearchResult)Container.DataItem).OrderId %>
																		<% } %>
																	</td>
																	<%-- ポイントレコードを一意に特定するため、枝番をこっそり隠しておく --%>
																	<asp:HiddenField ID="hfPointKbnNo2" Value="<%#: ((WrappedSearchResult)Container.DataItem).PointKbnNo %>" runat="server"/>
																	<%-- ポイントマスタ取得時にポイント区分が要るので… --%>
																	<asp:HiddenField ID="hfPointKbn2" Value="<%#: ((WrappedSearchResult)Container.DataItem).PointKbn %>" runat="server"/>
																</tr>
															</AlternatingItemTemplate>
														</asp:Repeater>
														<tr id="trListError" class="list_alert" runat="server" Visible="false">
															<td id="tdErrorMessage" runat="server" colspan="8"></td>
														</tr>
													</table>
													<% } %>
													<table cellspacing="0" cellpadding="0" border="0">
														<tr>
															<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
														</tr>
													</table>
													<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr class="info_item_bg">
															<td align="left">備考<br />
															・<span class="notice">*</span>は、そのユーザーが通常ポイントを保有していない場合に表示され、「ポイント」「有効期限」は自動的に初期値が表示されています。<br />
															　期間限定ポイントを指定して検索した場合には表示されません。
															<% if (Constants.MARKETINGPLANNER_USE_TEMPORARY_POINT) { %>
																<br /><br />
																・期間限定ポイントの仮ポイントは、通常ポイントの仮ポイントと合算して表示されます。
															<%} %>
															<% if (Constants.ORDER_POINT_BATCH_CHANGE_TEMP_TO_COMP_ENABLED) {%>
																<br /><br />
																・仮ポイントは編集できません。注文時に発行された仮ポイントは、出荷から下記日数で本ポイントに移行されます。<br />
																<table>
																	<tr>
																		<td width="25%">通常ポイント</td>
																		<% if (Constants.MARKETINGPLANNER_USE_TEMPORARY_POINT) { %>
																			<td width="75%">出荷後<%: Constants.ORDER_POINT_BATCH_POINT_TEMP_TO_COMP_DAYS %>日</td>
																		<% } else { %>
																			<td width="75%">(仮ポイント利用なし)</td>
																		<% } %>
																	</tr>
																	<% if (Constants.CROSS_POINT_OPTION_ENABLED == false) { %>
																	<tr>
																		<td width="25%">期間限定ポイント</td>
																		<% if (Constants.MARKETINGPLANNER_USE_TEMPORARY_POINT) { %>
																			<td width="75%">出荷後<%: Constants.ORDER_POINT_BATCH_POINT_TEMP_TO_COMP_LIMITED_TERM_POINT_DAYS %>日</td>
																		<% } else { %>
																			<td width="75%">(仮ポイント利用なし)</td>
																		<% } %>
																	</tr>
																	<% } %>
																</table>
																※出荷からの経過日数は、出荷完了日からの経過日数を指します。<br/>
																　注文ステータスが「出荷完了」または「配送完了」以外の場合や、出荷完了日が登録されていない場合、本ポイントへの移行は行われません。
															<% } %>
															</td>
														</tr>
													</table>
												</td>
											</tr>
											<% if (this.IsNotSearchDefault == false) { %>
											<tr>
												<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td class="action_list_sp"><asp:Button id="btnPointUpdateBottom" runat="server" Text="  このページの一括更新  " onclick="btnPointUpdateTop_Click"></asp:Button></td>
											</tr>
											<% } %>
											<tr>
												<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
										</table>
									</div>
									<div id="divPointComplete" runat="server" Visible="False">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td>
													<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
														<tr>
															<td width="675">以下のポイント情報を更新いたしました｡</td>
															<td width="83" class="action_list_sp"><asp:Button id="btRedirectEditTop" Runat="server" Text="  続けて処理をする  " onclick="btRedirectEditTop_Click"></asp:Button></td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td>
													<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr class="list_title_bg">
															<td align="center" width="30%" rowspan="2">ユーザーID</td>
															<td align="center" width="70%" colspan="2">処理結果</td>
														</tr>
														<tr class="list_title_bg">
															<% if (Constants.CROSS_POINT_OPTION_ENABLED) { %>
															<td align="center" width="70%">ポイント</td>
															<% } else { %>
															<td align="center" width="35%">ポイント</td>
															<td align="center" width="35%">有効期限</td>
															<% } %>
														</tr>
														<asp:Repeater id="rComplete" Runat="server">
															<ItemTemplate>
																<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
																	<td align="center"><%# WebSanitizer.HtmlEncode(((Hashtable)Container.DataItem)[Constants.FIELD_USER_USER_ID])%></td>
																	<td align="center"><%# WebSanitizer.HtmlEncode(m_htUpdatePointResult[(string)((Hashtable)Container.DataItem)[Constants.FIELD_USER_USER_ID] + (string)((Hashtable)Container.DataItem)[Constants.FIELD_USERPOINT_POINT_KBN] + ((Hashtable)Container.DataItem)[Constants.FIELD_USERPOINT_POINT_KBN_NO].ToString()] == null ? "―" : (bool)m_htUpdatePointResult[(string)((Hashtable)Container.DataItem)[Constants.FIELD_USER_USER_ID] + (string)((Hashtable)Container.DataItem)[Constants.FIELD_USERPOINT_POINT_KBN] + ((Hashtable)Container.DataItem)[Constants.FIELD_USERPOINT_POINT_KBN_NO].ToString()] ? "○" : "×") %></td>
																	<% if (Constants.CROSS_POINT_OPTION_ENABLED == false) { %>
																	<td align="center"><%# WebSanitizer.HtmlEncode(m_htUpdateExpDateTimeResult[(string)((Hashtable)Container.DataItem)[Constants.FIELD_USER_USER_ID] + (string)((Hashtable)Container.DataItem)[Constants.FIELD_USERPOINT_POINT_KBN] + ((Hashtable)Container.DataItem)[Constants.FIELD_USERPOINT_POINT_KBN_NO].ToString()] == null ? "―" : (bool)m_htUpdateExpDateTimeResult[(string)((Hashtable)Container.DataItem)[Constants.FIELD_USER_USER_ID] + (string)((Hashtable)Container.DataItem)[Constants.FIELD_USERPOINT_POINT_KBN] + ((Hashtable)Container.DataItem)[Constants.FIELD_USERPOINT_POINT_KBN_NO].ToString()] ? "○" : "×") %></td>
																	<% } %>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
													</table>
												</td>
											</tr>
											<tr>
												<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td class="action_list_sp"><asp:Button id="btRedirectEditBottom" Runat="server" Text="  続けて処理をする  " onclick="btRedirectEditTop_Click"></asp:Button></td>
											</tr>
											<tr>
												<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
										</table>
									</div>
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
<script type="text/javascript">
<!--
	var exec_submit_flg = 0;
	function exec_submit() {
		if (exec_submit_flg == 0) {
			exec_submit_flg = 1;
			return true;
		} else {
			return false;
		}
	}
	//-->
</script>
</asp:Content>