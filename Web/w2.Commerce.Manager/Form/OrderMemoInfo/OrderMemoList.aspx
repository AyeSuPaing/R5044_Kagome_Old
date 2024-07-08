<%--
=========================================================================================================
  Module      : 注文メモ設定一覧ページ(OrderMemoList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OrderMemoList.aspx.cs" Inherits="Form_OrderMemoInfo_OrderMemoList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr>
		<td><h1 class="page-title">注文メモ設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/></td>
	</tr>
	<!--△ タイトル △-->
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
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/></td>
										</tr>
										<tr>
											<td class="search_table">
												<table cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="search_title_bg" width="130">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0"/>
															注文メモID</td>
														<td class="search_item_bg" colspan="3">
															<asp:TextBox id="tbOrderMemoId" runat="server" Width="125"></asp:TextBox>
														</td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0"/>
															表示区分</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlDispKbn" runat="server"></asp:DropDownList>
														</td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0"/>
															有効フラグ</td>
														<td class="search_item_bg" width="110">
															<asp:DropDownList id="ddlValidFlg" runat="server"></asp:DropDownList>
														</td>
														<td class="search_btn_bg" width="83" rowspan="1">
															<div class="search_btn_main"><asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub"><a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_MEMO_LIST %>">クリア</a></div>
															<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
															<div class="search_btn_sub">
																<asp:LinkButton id="lbExportTranslationData" Runat="server" OnClick="lbExportTranslationData_Click">翻訳設定出力</asp:LinkButton></div>
															<% } %>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/></td>
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
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">注文メモ設定一覧</h2></td>
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
											<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/></td>
										</tr>
										<tr>
											<td>
												<!--▽ ページング ▽-->
												<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
													<tr>
														<td width="675" style="height: 22px"><asp:Label id="lbPager" Runat="server"></asp:Label></td>
														<td class="action_list_sp"><asp:Button id="btnInsertTop" Runat="server" Text="  新規登録  " OnClick="btnInsert_Click" /></td>
													</tr>
												</table>
												<!--△ ページング △-->
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/></td>
										</tr>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="list_title_bg">
														<td align="center" width="12%">注文メモID</td>
														<td align="center" width="13%">注文メモ名称</td>
														<td align="center" width="59%">デフォルトテキスト</td>
														<td align="center" width="8%">有効フラグ</td>
														<td align="center" width="8%">表示順</td>
													</tr>
													<asp:Repeater id="rOrderMemoList" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateDetailUrl((String)Eval(Constants.FIELD_ORDERMEMOSETTING_ORDER_MEMO_ID))) %>')">
																<td ><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_ORDERMEMOSETTING_ORDER_MEMO_ID))%></td>
																<td ><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_ORDERMEMOSETTING_ORDER_MEMO_NAME))%></td>
																<td align="left"><%# WebSanitizer.HtmlEncode(AbbreviateString(((string)Eval(Constants.FIELD_ORDERMEMOSETTING_DEFAULT_TEXT)), 50))%></td>
																<td align="center" ><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_ORDERMEMOSETTING, Constants.FIELD_ORDERMEMOSETTING_VALID_FLG, Eval(Constants.FIELD_ORDERMEMOSETTING_VALID_FLG)))%></td>
																<td align="center" ><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_ORDERMEMOSETTING_DISPLAY_ORDER))%></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="6"></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/></td>
										</tr>
										<tr>
											<td class="action_list_sp"><asp:Button id="btnInsertBottom" Runat="server" Text="  新規登録  " OnClick="btnInsert_Click" /></td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/></td>
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
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"/></td>
	</tr>
</table>
</asp:Content>