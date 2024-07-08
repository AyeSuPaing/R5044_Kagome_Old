<%--
=========================================================================================================
  Module      : 商品セット設定一覧ページ(ProductSetList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ProductSetList.aspx.cs" Inherits="Form_ProductSet_ProductSetList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">商品セット設定</h1></td>
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
												<table cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															商品セットID</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbProductSetId" runat="server" Width="120"></asp:TextBox>
														</td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															商品セット名</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbProductSetName" runat="server" Width="120"></asp:TextBox>
														</td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															並び順</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlSortKbn" runat="server">
																<asp:ListItem Value="0" Selected="True">商品セットID/昇順</asp:ListItem>
																<asp:ListItem Value="1">商品セットID/降順</asp:ListItem>
																<asp:ListItem Value="2">商品セット名/昇順</asp:ListItem>
																<asp:ListItem Value="3">商品セット名/降順</asp:ListItem>
															</asp:DropDownList>
														</td>
														<td class="search_btn_bg" width="83">
															<div class="search_btn_main"><asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub"><a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTSET_LIST %>">クリア</a></div>
															<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
															<div class="search_btn_sub"><asp:LinkButton id="lbExportTranslationData" Runat="server" OnClick="lbExportTranslationData_Click">翻訳設定出力</asp:LinkButton></div>
															<% } %>
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
	<!--△ 検索 △-->
	<tr>
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">商品セット設定一覧</h2></td>
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
														<td width="675" style="height: 22px"><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
														<td class="action_list_sp"><asp:Button id="btnInsertTop" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" /></td>
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
														<td align="center" width="120" rowspan="2">商品セットID</td>
														<td align="center" width="438" rowspan="2">商品セット名</td>
														<td align="center" width="100" colspan="2">親商品数</td>
														<td align="center" width="100" colspan="2">子商品数</td>
													</tr>
													<tr class="list_title_bg">
														<td align="center" width="50">MIN</td>
														<td align="center" width="50">MAX</td>
														<td align="center" width="50">MIN</td>
														<td align="center" width="50">MAX</td>
													</tr>
													<asp:Repeater id="rList" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateDetailUrl((string)Eval(Constants.FIELD_PRODUCTSET_PRODUCT_SET_ID))) %>')">
																<td align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTSET_PRODUCT_SET_ID))%></td>
																<td align="left"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTSET_PRODUCT_SET_NAME))%></td>
																<td align="right"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTSET_PARENT_MIN))%></td>
																<td align="right"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTSET_PARENT_MAX))%></td>
																<td align="right"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTSET_CHILD_MIN))%></td>
																<td align="right"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTSET_CHILD_MAX))%></td>
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
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td class="action_list_sp"><asp:Button id="btnInsertBottom" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" /></td>
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

