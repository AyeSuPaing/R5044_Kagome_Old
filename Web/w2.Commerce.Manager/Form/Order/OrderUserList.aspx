<%--
=========================================================================================================
  Module      : 注文登録用ユーザ一覧ページ(OrderUserList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="System.Data" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="OrderUserList.aspx.cs" Inherits="Form_Order_OrderUserList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<script type="text/javascript">
<!--
// 選択されたユーザ情報を設定
function set_user_info(user_id)
{
	if (window.opener != null)
	{
		window.opener.action_user_search(user_id);	   
		window.close();	   
	}
}
//-->
</script>
<table cellspacing="0" cellpadding="0" width="528" border="0">
	<!--▽ 検索 ▽-->
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="628" border="0">
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
												<table cellspacing="1" cellpadding="2" width="604" border="0">
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															ユーザID</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbUserID" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															顧客区分</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlUserKbn" runat="server"></asp:DropDownList></td>
														<td class="search_btn_bg" width="83" rowspan="5">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="UserSearch" /></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_USER_LIST %>">クリア</a>
																<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a></div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															<%: ReplaceTag("@@User.name.name@@") %></td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbUserName" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															<%: ReplaceTag("@@User.name_kana.name@@") %></td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbUserNameKana" runat="server" Width="125"></asp:TextBox></td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															<%: ReplaceTag("@@User.tel1.name@@") %></td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbUserTel1" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															メールアドレス</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbUserMailAddr" runat="server" Width="125"></asp:TextBox></td>
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
		<td><h2 class="cmn-hed-h2">ユーザー情報一覧</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="628" border="0">
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
												<table class="list_table" cellspacing="1" cellpadding="3" width="604" border="0">
													<tr class="list_alert">
														<td><%= WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NOT_SEARCH_DEFAULT) %></td>
													</tr>
												</table>
											</td>
										</tr>
										<% } else { %>
										<tr>
											<td>
												<!--▽ ページング ▽-->
												<table class="list_pager" cellspacing="0" cellpadding="0" width="604" border="0">
													<tr>
														<td width="504" style="height: 22px"><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
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
												<table class="list_table" cellspacing="1" cellpadding="3" width="604" border="0">
													<tr class="list_title_bg">
														<td align="center" width="86" style="height: 17px" >ユーザID</td>
														<td align="center" width="120" style="height: 17px"><%: ReplaceTag("@@User.name.name@@") %></td>
														<td align="center" width="150" style="height: 17px"><%: ReplaceTag("@@User.tel1.name@@") %></td>
														<td align="center" width="250" style="height: 17px" rowspan="2">住所</td>
													</tr>
													<tr class="list_title_bg">
														<td align="center" width="120" style="height: 17px">顧客区分</td>
														<td align="center" width="120" style="height: 17px"><%: ReplaceTag("@@User.name_kana.name@@") %></td>
														<td align="center" width="150" style="height: 17px">メールアドレス</td>
													</tr>
													<asp:Repeater id="rList" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="javascript:set_user_info('<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USER_USER_ID)) %>')">
																<td align="center" style="padding:0px">
																	<table cellpadding="0" cellspacing="0" width="100%">
																		<tr>
																			<td align="left" width="120" class="item_bottom_line"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USER_USER_ID))%>&nbsp;</td>
																		</tr>
																		<tr>
																			<td><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_USER_KBN, (string)Eval(Constants.FIELD_USER_USER_KBN))) %></td>
																		</tr>
																	</table>
																</td>
																<td align="center" style="padding:0px">
																	<table cellpadding="0" cellspacing="0" width="100%">
																		<tr>
																			<td align="left" width="120" class="item_bottom_line"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USER_NAME)) %>&nbsp;</td>
																		</tr>
																		<tr>
																			<td align="left" width="120"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USER_NAME_KANA)) %>&nbsp;</td>
																		</tr>
																	</table>
																</td>
																<td align="center" style="padding:0px">
																	<table cellpadding="0" cellspacing="0" width="100%">
																		<tr>
																			<td align="left" width="150" class="item_bottom_line"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USER_TEL1))%>&nbsp;</td>
																		</tr>
																		<tr>
																			<td align="left" width="150"><%# WebSanitizer.HtmlEncode(DisplayMailAddr((DataRowView)Container.DataItem)) %>&nbsp;</td>
																		</tr>
																	</table>
																</td>
																<td align="left"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USER_ADDR))%></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="7"></td>
													</tr>
												</table>
											</td>
										</tr>
										<% } %>
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

