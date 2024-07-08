<%--
=========================================================================================================
  Module      : 注文ワークフロー設定情報一覧ページ(OrderWorkflowSettingList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OrderWorkflowSettingList.aspx.cs" Inherits="Form_OrderWorkflowSetting_OrderWorkflowSettingList" %>
<%@ Import Namespace="w2.Common.Web" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">受注ワークフロー設定</h1></td>
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
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															ワークフロー区分</td>
                                                        <td class="search_item_bg" width="130"><asp:DropDownList id="ddlWorkflowKbn" runat="server" CssClass="search_item_width">
																</asp:DropDownList></td>
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															ワークフロー名</td>
                                                        <td class="search_item_bg" width="130"><asp:TextBox id="tbWorkflowName" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															並び順</td>
														<td class="search_item_bg" width="130"><asp:DropDownList id="ddlSortKbn" runat="server" CssClass="search_item_width">
                                                                <asp:ListItem Value="0">ワークフロー区分</asp:ListItem>
																<asp:ListItem Value="1">実行順/昇順</asp:ListItem>
																<asp:ListItem Value="2">実行順/降順</asp:ListItem>
                                                            	<asp:ListItem Value="3">作成日/昇順</asp:ListItem>
																<asp:ListItem Value="4">作成日/降順</asp:ListItem>
                                                            	<asp:ListItem Value="5">更新日/昇順</asp:ListItem>
																<asp:ListItem Value="6">更新日/降順</asp:ListItem>
																</asp:DropDownList></td>
														<td class="search_btn_bg" width="83" rowspan="<%= Constants.DISPLAY_CORPORATION_ENABLED ? 10: 9 %>">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERWORKFLOWSETTING_LIST %>">クリア</a>&nbsp;
																<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a></div>
														</td>
													</tr>
                                              	    <tr>
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															実行形式</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlWorkFlowDetailKbn" runat="server" CssClass="search_item_width"></asp:DropDownList></td>
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															検索ボックス</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlAdditionalSearchFlg" runat="server" CssClass="search_item_width">
                                                                <asp:ListItem Value=""></asp:ListItem>
                                                                <asp:ListItem Value="0">検索ボックスなし</asp:ListItem>
																<asp:ListItem Value="1">検索ボックスあり</asp:ListItem>
															</asp:DropDownList></td>
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															有効フラグ</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlValidFlg" runat="server" CssClass="search_item_width">
															</asp:DropDownList></td>
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
		<td><h2 class="cmn-hed-h2">受注ワークフロー設定一覧</h2></td>
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
														<td><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
														<td style="text-align:right" class="action_list_sp">
															<% if (Constants.ORDERWORKFLOWSETTING_MAXCOUNT.HasValue) { %>
																<span style="font-size: 13px; vertical-align: middle;">現在ワークフロー設定数 <%: this.OrderWorkflowSettingItemsCount %>（最大 <%: Constants.ORDERWORKFLOWSETTING_MAXCOUNT %>）</span>
															<% } %>
															<%-- 受注ワークフロー自動実行オプションがオンの場合 --%>
															<% if (Constants.ORDERWORKFLOW_AUTOEXEC_OPTION_ENABLE){ %>
																<% btnAutoExecSettingTop.OnClientClick = "javascript:open_window('" + new UrlCreator(Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_ORDERWORKFLOW_SCENARIO_LIST).CreateUrl() + "','Import','width=1200,height=800,top=120,left=420,status=no,scrollbars=yes');return false;"; %>
																<asp:Button ID="btnAutoExecSettingTop" runat="server" Text="  シナリオ設定  " CssClass="cmn-btn-sub-action" style="margin-right: 5px"/>
															<% } %>
															<asp:Button id="btnInsertTop" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" />
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
														<td align="center" width="140">ワークフロー区分</td>
														<td align="center" width="458">ワークフロー名</td>
														<td align="center" width="80">有効フラグ</td>
														<td align="center" width="80">ワークフロー<br />実行順</td>
													</tr>
													<asp:Repeater id="rList" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateOrderWorkflowSettingEditUrl((string)Eval(Constants.REQUEST_KEY_WORKFLOW_TYPE), (string)Eval(Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN), (int)Eval(Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NO))) %>')">
																<td align="center"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN, (string)Eval(Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN))) %></td>
																<td align="left"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NAME)) %></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_VALID_FLG, (string)Eval(Constants.FIELD_ORDERWORKFLOWSETTING_VALID_FLG))) %></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_ORDER)) %></td>
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
											<td class="action_list_sp">
												<% if (Constants.ORDERWORKFLOWSETTING_MAXCOUNT.HasValue) { %>
													<span style="font-size: 13px; vertical-align: middle;">現在ワークフロー設定数 <%: this.OrderWorkflowSettingItemsCount %>（最大 <%: Constants.ORDERWORKFLOWSETTING_MAXCOUNT %>）</span>
												<% } %>
												<% if (Constants.ORDERWORKFLOW_AUTOEXEC_OPTION_ENABLE){ %>
													<% btnAutoExecSettingBottom.OnClientClick = "javascript:open_window('" + new UrlCreator(Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_ORDERWORKFLOW_SCENARIO_LIST).CreateUrl() + "','Import','width=1200,height=800,top=120,left=420,status=no,scrollbars=yes');return false;"; %>
													<asp:Button ID="btnAutoExecSettingBottom" runat="server" Text="  シナリオ設定  " CssClass="cmn-btn-sub-action" />
												<% } %>
												<asp:Button id="btnInsertBotttom" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" />
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