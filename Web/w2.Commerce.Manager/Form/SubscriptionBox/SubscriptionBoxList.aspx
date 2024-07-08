<%--
=========================================================================================================
  Module      : 頒布会コース設定(SubscriptionBoxList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="SubscriptionBoxList.aspx.cs" Inherits="Form_SubscriptionBox_SubscriptionBoxList" %>

<asp:Content id="Content2" ContentPlaceHolderid="ContentPlaceHolderHead" Runat="Server">
<meta http-equiv="Pragma" content="no-cache" />
<meta http-equiv="cache-control" content="no-cache" />
<meta http-equiv="expires" content="0" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">頒布会コース設定</h1></td>
	</tr>
	<tr>
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 検索 ▽-->
	<tr>
		<td style="width: 792px">
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
												<%
													// 各テキストボックスのEnter押下時に検索を走らせるようにする
													tbSubscriptionBoxCourseId.Attributes["onkeypress"]
														= tbSubscriptionBoxCourseId.Attributes["onkeypress"]
														= tbProductId.Attributes["onkeypress"]
														= tbSubscriptionBoxName.Attributes["onkeypress"]
														= "if (event.keyCode==13){__doPostBack('" + btnSearch.UniqueID + "',''); return false;}";
												%>
												<table cellspacing="1" cellpadding="2" width="768" border="0">
													<tr>
														<td class="search_title_bg" width="98">
															コースID</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbSubscriptionBoxCourseId" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="95">
															商品ID</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbProductId" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="100">
															並び替え</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlSortKbn" runat="server" CssClass="search_item_width">
																<asp:ListItem Value="00" Selected="True">コースID / 昇順</asp:ListItem>
																<asp:ListItem Value="01">コースID / 降順</asp:ListItem>
																<asp:ListItem Value="02">コース名（管理用 / 昇順 </asp:ListItem>
																<asp:ListItem Value="03">コース名（管理用 / 降順 </asp:ListItem>
																<asp:ListItem Value="04">有効フラグ</asp:ListItem>
															</asp:DropDownList>
														</td>
														<td class="search_btn_bg" width="83" rowspan="<%= 12 + (Constants.MEMBER_RANK_OPTION_ENABLED  ? 2 : 0) + (Constants.PRODUCTBUNDLE_OPTION_ENABLED ? 1 : 0) %>">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_SUBSCRIPTION_BOX_LIST %>">クリア</a>
																<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a></div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="100">
															コース管理名</td>
														<td class="search_item_bg" width="130" colspan="3">
															<asp:TextBox id="tbSubscriptionBoxName" runat="server" Width="500"></asp:TextBox></td>
														<td class="search_title_bg" width="95">
															有効フラグ</td>
														<td class="search_item_bg">
															<asp:DropDownList id="dllValidFlg" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>
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
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">頒布会コース設定一覧</h2></td>
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
														<td width="550" style="height: 22px"><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
														<td align="right">
															<table width="200px">
																<tr>
																	<td style="text-align:right">
																		<asp:Button id="btnInsertTop" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" />
																	</td>
																</tr>
															</table>
														</td>
													</tr>
												</table>
												<!-- ページング-->
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<% if (this.IsNotSearchDefault) { %>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="list_alert">
														<td colspan="9"><%= WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NOT_SEARCH_DEFAULT) %></td>
													</tr>
												</table>
											</td>
										</tr>
										<% } else { %>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" border="0">
													<tr class="list_title_bg">
														<td align="center" style="height: 17px; width: 15%">コースID</td>
														<td align="center" style="height: 17px; width: 40%">コース管理名</td>
														<td align="center" style="height: 17px; width: 40%">コース表示名</td>
														<td align="center" style="height: 17px; width: 5%">有効フラグ</td>
													</tr>
													<asp:Repeater id="rList" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl((String)Eval(Constants.FIELD_SUBSCRIPTIONBOX_SUBSCRIPTION_BOX_COURSE_ID))) %>')">
																<td style="width: 15%" align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_SUBSCRIPTIONBOX_SUBSCRIPTION_BOX_COURSE_ID)) %>
																<td style="width: 15%" align="left"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_SUBSCRIPTIONBOX_MANAGEMENT_NAME))%></td>
																<td style="width: 15%"align="left"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_SUBSCRIPTIONBOX_DISPLAY_NAME))%></td>
																<td style="width: 15%" align="center"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_SUBSCRIPTIONBOX, Constants.FIELD_SUBSCRIPTIONBOX_VALID_FLG, Eval(Constants.FIELD_SUBSCRIPTIONBOX_VALID_FLG)))%></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="8"></td>
													</tr>
												</table>
											</td>
										</tr>
										<% } %>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
									
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
													<tr>
														<td width="550" style="height: 22px"></td>
														<td align="right">
															<table width="200px">
																<tr>
																	<td style="text-align:right">
																		<asp:Button id="btnInsertBottom" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" />
																	</td>
																</tr>
															</table>
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
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
