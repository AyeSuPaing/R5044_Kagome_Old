<%--
=========================================================================================================
  Module      : セール設定一覧ページ(ProductSaleList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="System.Data" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ProductSaleList.aspx.cs" Inherits="Form_ProductSale_ProductSaleList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<%-- 
テキストボックス内でEnterキーを押してSubmit（一番上に配置されているTextBoxのSubmit）送信しようとすると、
IEのバグでテキストボックスが画面上に一つのみ配置されている場合にSubmit送信されない不具合の対応として、
ダミーのTextBoxを非表示で配置している。
--%>
<asp:TextBox id="tbDummy" runat="server" style="visibility:hidden;display:none;"></asp:TextBox>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">セール設定</h1></td>
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
											<td class="search_table">
												<table cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="search_title_bg" width="130"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />商品セール区分</td>
														<td class="search_item_bg" width="325" colspan="3">
															<asp:RadioButtonList id="rblProductSaleKbn" RepeatLayout="Flow" RepeatDirection="Horizontal" runat="server">
																<asp:ListItem Text="全て" Value="" Selected="True"></asp:ListItem>
																<asp:ListItem Text="闇市" Value="CM"></asp:ListItem>
																<asp:ListItem Text="タイムセール" Value="TS"></asp:ListItem>
															</asp:RadioButtonList></td>
														<td class="search_title_bg" width="100"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />並び順</td>
														<td class="search_item_bg" width="120">
															<asp:DropDownList id="ddlSortKbn" runat="server">
																<asp:ListItem Text="開始日時/昇順" Value="0"></asp:ListItem>
																<asp:ListItem Text="開始日時/降順" Value="1"></asp:ListItem>
																<asp:ListItem Text="商品セールID/昇順" Value="2"></asp:ListItem>
																<asp:ListItem Text="商品セールID/降順" Value="3"></asp:ListItem>
															</asp:DropDownList>
														</td>
														<td class="search_btn_bg" width="83" rowspan="2">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTSALE_LIST %>">クリア</a>&nbsp;
																<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a></div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="130"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />セールID/名</td>
														<td class="search_item_bg" width="110" colspan="3"><asp:TextBox id="tbProductSaleInfo" runat="server" Width="250"></asp:TextBox></td>
														<td class="search_title_bg" width="95"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />稼働状態</td>
														<td class="search_item_bg" width="110">
															<asp:DropDownList ID="ddlProductSaleOpend" runat="server">
																<asp:ListItem Text="全て" Value=""></asp:ListItem>
																<asp:ListItem Text="準備中" Value="0"></asp:ListItem>
																<asp:ListItem Text="開催中" Value="1"></asp:ListItem>
																<asp:ListItem Text="終了済" Value="2"></asp:ListItem>
																<asp:ListItem Text="一時停止" Value="3"></asp:ListItem>
															</asp:DropDownList></td>
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
		<td><h2 class="cmn-hed-h2">セール設定一覧</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<div id="divSaleList" runat="server">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td align="right">
													<!--▽ ページング ▽-->
													<table class="list_pager" cellspacing="0" cellpadding="0" border="0">
														<tr>
															<td style="height: 22px"><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
															<td class="action_list_sp"><asp:Button id="btnInsertTop" Runat="server" Text="  新規登録  " OnClick="btnInsert_Click" /></td>
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
													<div>
													<table class="list_table" cellspacing="1" cellpadding="3" width="750" border="0">
														<tr class="list_title_bg">
															<td align="center" width="120" rowspan="2">商品セールID</td>
															<td align="center" width="120" rowspan="2">商品セール区分</td>
															<td align="left" width="260" rowspan="2">商品セール名</td>
															<td align="left" width="40" rowspan="2">商品数</td>
															<td align="center" width="130">開始日時</td>
															<td align="center" width="40" rowspan="2">有効<br />フラグ</td>
															<td align="center" width="60" rowspan="2">開催状態</td>
														</tr>
														<tr class="list_title_bg">
															<td align="center" width="130">終了日時</td>
														</tr>
														<asp:Repeater id="rList" Runat="server">
															<ItemTemplate>
																<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateDetailUrl((String)Eval(Constants.FIELD_PRODUCTSALE_PRODUCTSALE_KBN), (String)Eval(Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID))) %>')">
																	<td align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID))%></td>
																	<td align="center"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTSALE, Constants.FIELD_PRODUCTSALE_PRODUCTSALE_KBN, Eval(Constants.FIELD_PRODUCTSALE_PRODUCTSALE_KBN)))%></td>
																	<td align="left"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTSALE_PRODUCTSALE_NAME))%></td>
																	<td align="center"><%# WebSanitizer.HtmlEncode(Eval("price_count","{0:d}") )%></td>
																	<td align="center"><%#: DateTimeUtility.ToStringForManager(Eval(Constants.FIELD_PRODUCTSALE_DATE_BGN), DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %><br />
																						<%#: DateTimeUtility.ToStringForManager(Eval(Constants.FIELD_PRODUCTSALE_DATE_END), DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
																	<td align="center"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTSALE, Constants.FIELD_PRODUCTSALE_VALID_FLG, Eval(Constants.FIELD_PRODUCTSALE_VALID_FLG)))%></td>
																	<td align="center">
																		<strong id="Strong1" disabled='<%# ((string)Eval("sale_opened") != "1") %>' runat="server">
																		<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTSALE, "sale_opened", Eval("sale_opened")))%>
																		</strong>
																	</td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
														<tr id="trListError" class="list_alert" runat="server" Visible="False">
															<td id="tdErrorMessage" colspan="7" runat="server"></td>
														</tr>
													</table>
													</div>
												</td>
											</tr>
											<tr>
												<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td class="action_list_sp"><asp:Button id="btnInsertBottom" Runat="server" Text="  新規登録  " OnClick="btnInsert_Click" /></td>
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
</asp:Content>