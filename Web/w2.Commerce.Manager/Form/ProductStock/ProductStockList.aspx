<%--
=========================================================================================================
  Module      : 商品在庫一覧ページ(ProductStockList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ProductStockList.aspx.cs" Inherits="Form_ProductStock_ProductStockList" %>
<%-- マスタ出力コントロール --%>
<%@ Register TagPrefix="uc" TagName="MasterDownLoad" Src="~/Form/Common/MasterDownLoadPanel.ascx" %>
<%@ Import Namespace="System.Data" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<%-- 
テキストボックス内でEnterキーを押してSubmit（一番上に配置されているTextBoxのSubmit）送信しようとすると、
IEのバグでテキストボックスが画面上に一つのみ配置されている場合にSubmit送信されない不具合の対応として、
ダミーのTextBoxを非表示で配置している。
--%>
<asp:TextBox id="tbDummy" runat="server" style="visibility:hidden;display:none;"></asp:TextBox>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr id="trTitleProductTop" runat="server">
	</tr>
	<tr id="trTitleProductMiddle" runat="server">
		<td><h1 class="page-title">在庫情報</h1></td>
	</tr>
	<tr id="trTitleProductBottom" runat="server">
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
														<td class="search_title_bg" width="95"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />抽出タイプ</td>
														<td class="search_item_bg" colspan="5" style="height:20px;">
															<asp:RadioButtonList id="rlStockAlert" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="radio_button_list">
																<asp:ListItem Value="0" Selected="True">在庫管理商品全て</asp:ListItem>
																<asp:ListItem Value="1">安全在庫アラート</asp:ListItem>
																<asp:ListItem Value="2">在庫切れ販売停止</asp:ListItem>
															</asp:RadioButtonList>
														</td>
														<td class="search_btn_bg" width="83" rowspan="3">
															<div class="search_btn_main"><asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub"><a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTSTOCK_LIST %>">クリア</a></div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />在庫数検索</td>
														<td class="search_item_bg" colspan="5">
															<asp:DropDownList ID="ddlSearchStockKey" runat="server" CssClass="search_item_width">
																<asp:ListItem Value=""></asp:ListItem>
																<asp:ListItem Value="stock">論理在庫</asp:ListItem>
																<%-- 以下3つはロジスティクスOptionにより、ProductStockList.aspx.cs内で切り替える --%>
																<asp:ListItem Value="realstock">実在庫 A品</asp:ListItem>
																<asp:ListItem Value="realstock_b">実在庫 B品</asp:ListItem>
																<asp:ListItem Value="realstock_c">実在庫 C品</asp:ListItem>
															</asp:DropDownList>
															が
															<asp:TextBox ID="tbSearchStockCount" Width="50" runat="server">0</asp:TextBox>
															<asp:DropDownList ID="ddlSearchProductCountType" runat="server" CssClass="search_item_width">
																<asp:ListItem Value="1">以上</asp:ListItem>
																<asp:ListItem Value="0">と等しい</asp:ListItem>
																<asp:ListItem Value="2">以下</asp:ListItem>
															</asp:DropDownList>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="95"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />検索項目</td>
														<td class="search_item_bg" width="130"><asp:DropDownList id="ddlSearchKey" runat="server" CssClass="search_item_width">
																<asp:ListItem Value="0" Selected="True">商品ID</asp:ListItem>
																<asp:ListItem Value="1">商品名</asp:ListItem>
																<asp:ListItem Value="2">フリガナ</asp:ListItem>
																<asp:ListItem Value="3">カテゴリID</asp:ListItem>
															</asp:DropDownList></td>
														<td class="search_title_bg" width="95"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />検索値</td>
														<td class="search_item_bg" width="130"><asp:TextBox id="tbSearchWord" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="95"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />並び順</td>
														<td class="search_item_bg" width="130"><asp:DropDownList id="ddlSortKbn" runat="server" CssClass="search_item_width">
																<asp:ListItem Value="0" Selected="True">商品ID/昇順</asp:ListItem>
																<asp:ListItem Value="1">商品ID/降順</asp:ListItem>
																<asp:ListItem Value="2">商品名/昇順</asp:ListItem>
																<asp:ListItem Value="3">商品名/降順</asp:ListItem>
																<asp:ListItem Value="4">フリガナ/昇順</asp:ListItem>
																<asp:ListItem Value="5">フリガナ/降順</asp:ListItem>
																<asp:ListItem Value="6">作成日/昇順</asp:ListItem>
																<asp:ListItem Value="7">作成日/降順</asp:ListItem>
																<asp:ListItem Value="8">更新日/昇順</asp:ListItem>
																<asp:ListItem Value="9">更新日/降順</asp:ListItem>
															</asp:DropDownList>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr><td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
										<% if ((this.DisplayKbn == Constants.KBN_PRODUCTSTOCK_DISPLAY_LIST) || (this.DisplayKbn == null)) { %>
										<%-- マスタ出力 --%>
										<tr>
											<td class="search_table">
												<uc:MasterDownLoad runat="server" ID="uMasterDownload" DownloadType="ProductStock" TableWidth="758" />
											</td>
										</tr>
										<tr><td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
										<%} %>
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
		<td><h2 class="cmn-hed-h2">商品在庫情報一覧</h2></td>
	</tr>
	<tr id="trEdit" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">商品在庫情報編集</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<div id="divStockEdit" runat="server">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<% if(this.IsNotSearchDefault) { %>
											<tr>
												<td>
													<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr class="list_alert">
															<td><%= WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NOT_SEARCH_DEFAULT) %></td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<%}else{%>
											<tr>
												<td>
													<!--▽ ページング ▽-->
													<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
														<tr>
															<td width="508"><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
															<td class="action_list_sp"><div align="right" class="div_button_area">
																<asp:Button id="btnEditTop" runat="server" Text="  編集する  " OnClick="btnEdit_Click" />
																<asp:Button id="btnListTop" runat="server" Text="  一覧へ戻る  " OnClick="btnList_Click" />
																<asp:Button id="btnStockUpdateTop" runat="server" Text="  一括更新  " OnClientClick="return exec_submit()" OnClick="btnStockUpdate_Click" /></div></td>
														</tr>
													</table>
													<!-- ページング-->
												</td>
											</tr>
											<tr>
												<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<!--▽ 一覧表示 ▽-->
												<td id="tdDispList" runat="server">
													<div>
														<%-- ▽ テーブルヘッダ ▽ --%>
														<% if (rList.Items.Count > 0) { %>
															<div>
																<table border="0" cellpadding="0" cellspacing="0">
																	<tr>
																		<td>
																			<!-- ▽ 固定ヘッダ ▽ -->	
																			<div class="div_header_left">        
																				<table class="list_table tableHeaderFixed" cellspacing="1" cellpadding="3" border="0" style="height:100%">
																					<tr class="list_title_bg">
																						<td align="center" width="120" rowspan="3">商品ID</td>
																						<td align="center" width="100" rowspan="3">バリエーション<br />ID</td>
																					</tr> 
																				</table>
																			</div>
																			<!-- △ 固定ヘッダ △ -->	
																		</td>
																		<td>
																			<!-- ▽ ヘッダ ▽ -->	
																			<div class="div_header_right">        
																				<table class="list_table tableHeader" cellspacing="1" cellpadding="3" width="<%# Constants.REALSTOCK_OPTION_ENABLED ? 808 : 518 %>" border="0" style="height:100%"> <!-- 水平ヘッダ -->
															<%--▽ 実在庫利用が有効な場合は表示 ▽--%>
															<% if (Constants.REALSTOCK_OPTION_ENABLED){ %>
															<tr class="list_title_bg">
																						<td align="center" width="310" rowspan="3">商品名</td>
																						<td align="center" width="350" colspan="5" >在庫数</td>
																						<td align="center" width="70" rowspan="3">安全基準値<br />(論理在庫)</td>
																<td align="center" width="40" rowspan="3">履歴<br />一覧</td>
															</tr>
															<tr class="list_title_bg">
																<td align="center" width="70" rowspan="2">論理在庫</td>
																<td align="center" width="280" colspan="4">実在庫</td>
															</tr>
															<tr class="list_title_bg">
																<td align="center" width="65">A品</td>
																<td align="center" width="65">A品 引当済</td>
																<td align="center" width="65">B品</td>
																<td align="center" width="65">C品</td>
															</tr>
															<%--△ 実在庫利用が有効な場合は表示 △--%>
															<%--▽ 実在庫利用が無効な場合は表示 ▽--%>
															<%
															}else{ %>
															<tr class="list_title_bg">
																						<td align="center" width="570">商品名</td>
																						<td align="center" width="70">在庫数<br />(論理在庫)</td>
																						<td align="center" width="70">安全基準値<br />(論理在庫)</td>
																<td align="center" width="40">履歴<br />一覧</td>
															</tr>
															<% } %>
															<%--△ 実在庫利用が無効な場合は表示 △--%>
																				</table>
																			</div>
																			<!-- △ ヘッダ △ -->	
																		</td>
																	</tr>
																</table>
															</div>
															<%-- △ テーブルヘッダ △ --%>

															<%-- ▽ テーブルデータ ▽ --%>
															<div class="div_data" style="<%= rList.Items.Count > 0 ? "" : "display:none;" %> max-height: 420px; height:<%= rList.Items.Count * 50 + 20 %>px;">
																<%-- ▽ 固定データ ▽ --%>
																<div class="div_data_left">                         
																		<table class="list_table tableDataFix" cellspacing="1" cellpadding="3" border="0"> <!-- 垂直ヘッダ -->
																			<asp:Repeater id="rTableFixColumn" Runat="server">
																				<HeaderTemplate>
																				</HeaderTemplate>
																				<ItemTemplate>
																					<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
																						<td width="120" align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCT_PRODUCT_ID)) %></td>
																						<td width="100" align="center"><%# WebSanitizer.HtmlEncode(((string)Eval(Constants.FIELD_PRODUCTVARIATION_V_ID) != "") ? (string)Eval(Constants.FIELD_PRODUCTVARIATION_V_ID) : "-") %></td>
																					</tr>
																				</ItemTemplate>
																			</asp:Repeater>
																		</table>
																</div>
																<%-- △ 固定データ △ --%>

																<%-- ▽ データ ▽ --%>
																<div class="div_data_right">
																	<table class="list_table tableData" cellspacing="1" cellpadding="3" width="<%# Constants.REALSTOCK_OPTION_ENABLED ? 808 : 518 %>" border="0" style="max-height: 420px; height:<%= rList.Items.Count * 50 + 20 %>px;">
															<asp:Repeater id="rList" Runat="server">
																<ItemTemplate>
																	<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
																		<td width="<%# (Constants.REALSTOCK_OPTION_ENABLED) ? 310 : 570 %>" align="left">
																			<%if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_PRODUCT_CONFIRM)) {%>
																				<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl((String)Eval(Constants.FIELD_PRODUCT_PRODUCT_ID))) %>"><%# WebSanitizer.HtmlEncode(CreateProductAndVariationName((DataRowView)Container.DataItem))%></a>
																			<% } else { %>
																				<%# WebSanitizer.HtmlEncode(CreateProductAndVariationName((DataRowView)Container.DataItem))%>
																			<% } %>
																		</td>
																		<td width="70" align="center"><%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(Constants.FIELD_PRODUCTSTOCK_STOCK))) %></td>
																		<%--▽ 実在庫利用が有効な場合は表示 ▽--%>
																		<% if (Constants.REALSTOCK_OPTION_ENABLED){ %>
																				<td width="65" align="center"><%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(Constants.FIELD_PRODUCTSTOCK_REALSTOCK))) %></td>
																				<td width="65" align="center"><%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED))) %></td>
																				<td width="65" align="center"><%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_B))) %></td>
																				<td width="65" align="center"><%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_C))) %></td>
																		<% } %>
																		<%--△ 実在庫利用が有効な場合は表示 △--%>
																				<td width="70" align="center"><%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT))) %></td>
																				<td width="40" align="center"><a href="javascript:open_window('<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductStockHistoryList(HttpUtility.UrlEncode((string)Eval(Constants.FIELD_PRODUCT_PRODUCT_ID)),HttpUtility.UrlEncode((string)Eval(Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)))) %>','productstockhistory_list','width=930,height=615,top=120,left=320,status=NO,scrollbars=yes');">参照</a></td>
																	</tr>
																</ItemTemplate>
															</asp:Repeater>
																</table>
																</div>
																<%-- △ データ △ --%>
															</div>
															<%-- △ テーブルデータ △ --%>
														<% } else { %>
															<%-- ▽ エラーの場合 ▽ --%>
																<div style="<%= rList.Items.Count > 0 ? "display:none" : "" %>">
																	<table class="list_table" cellspacing="1" cellpadding="3" width="734" border="0" style="height:100%"> <!-- 水平ヘッダ -->				
																		<tr id="trListError" class="list_alert" runat="server" Visible="False">
																			<td id="tdListErrorMessage" colspan="27" runat="server"></td>
																		</tr>
																	</table>
																</div>
															<%-- △ エラーの場合 △ --%>
														<% } %>
													</div>
												</td>
												<!--△ 一覧表示 △-->
												<!--▽ 編集一覧表示 ▽-->
												<td id="tdDispEditList" runat="server">
													<div style="width:100%;">
														<%-- ▽ テーブルヘッダ ▽ --%>
														<div>
															<table border="0" cellpadding="0" cellspacing="0">
																<tr>
																	<td>
																		<!-- ▽ 固定ヘッダ ▽ -->
																		<div class="div_header_left">
																			<table class="list_table tableHeaderFixed" cellspacing="1" cellpadding="3" border="0" style="height:100%">
														<tr class="list_title_bg">
															<td align="center" width="120" rowspan="3">商品ID</td>
															<td align="center" width="120" rowspan="3">バリエーション<br />ID</td>
																				</tr> 
																			</table>
																		</div>
																		<!-- △ 固定ヘッダ △ -->
																	</td>
																	<td>
																		<!-- ▽ ヘッダ ▽ -->
																		<div class="div_header_right">
																			<table class="list_table tableHeader" cellspacing="1" cellpadding="3" width="<%# Constants.REALSTOCK_OPTION_ENABLED ? 1498 : 798 %>" border="0" style="height:100%"> <!-- 水平ヘッダ -->
																				<%--▽ 実在庫利用が有効な場合は表示 ▽--%>
																				<% if (Constants.REALSTOCK_OPTION_ENABLED){ %>
																				<tr class="list_title_bg">
																					<td align="center" width="290" rowspan="3">商品名</td>
															<td align="center" width="700" colspan="10">在庫数</td>
															<td align="center" width="140" colspan="2">安全基準値(論理在庫)</td>
																					<td align="center" width="0" rowspan="3">更新メモ</td>
															<td align="center" width="40" rowspan="3">履歴<br />一覧</td>
														</tr>
														<tr class="list_title_bg">
															<td align="center" width="70" rowspan="2">論理在庫</td>
															<td align="center" width="70" rowspan="2">＋/－</td>
															<td align="center" width="560" colspan="8">実在庫</td>
															<td align="center" width="70" rowspan="2">現在</td>
															<td align="center" width="70" rowspan="2">変更後</td>
														</tr>
														<tr class="list_title_bg">
															<td align="center" width="70">A品</td>
															<td align="center" width="70">＋/－</td>
															<td align="center" width="70">A品 引当済</td>
															<td align="center" width="70">＋/－</td>
															<td align="center" width="70">B品</td>
															<td align="center" width="70">＋/－</td>
															<td align="center" width="70">C品</td>
															<td align="center" width="70">＋/－</td>
														</tr>
														<%--△ 実在庫利用が有効な場合は表示 △--%>
														<%--▽ 実在庫利用が無効な場合は表示 ▽--%>
														<%
														}else{ %>
														<tr class="list_title_bg">
																					<td align="center" width="290" rowspan="2">商品名</td>
															<td align="center" width="140" colspan="2">在庫数</td>
															<td align="center" width="140" colspan="2">安全基準値(論理在庫)</td>
																					<td align="center" width="0" rowspan="2">更新メモ</td>
															<td align="center" width="40" rowspan="2">履歴<br />一覧</td>
														</tr>
														<tr class="list_title_bg">
															<td align="center" width="70">論理在庫</td>
															<td align="center" width="70">＋/－</td>
															<td align="center" width="70">現在</td>
															<td align="center" width="70">変更後</td>
														</tr>
														<% } %>
														<%--△ 実在庫利用が無効な場合は表示 △--%>
																			</table>
																		</div>
																		<!-- △ ヘッダ △ -->
																	</td>
																</tr>
															</table>
														</div>
														<%-- △ テーブルヘッダ △ --%>

														<%-- ▽ テーブルデータ ▽ --%>
														<div class="div_data" style="<%= rEdit.Items.Count > 0 ? "" : "display:none;" %>">
															<%-- ▽ 固定データ ▽ --%>
															<div class="div_data_left">                         
																	<table class="list_table tableDataFix" cellspacing="1" cellpadding="3" border="0"> <!-- 垂直ヘッダ -->
																		<asp:Repeater id="rEditTableFixColumn" Runat="server">
																			<HeaderTemplate>
																			</HeaderTemplate>
																			<ItemTemplate>
																				<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
																					<td width="120" align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCT_PRODUCT_ID)) %></td>
																					<td width="120" align="center"><%# WebSanitizer.HtmlEncode(((string)Eval(Constants.FIELD_PRODUCTVARIATION_V_ID) != "") ? (string)Eval(Constants.FIELD_PRODUCTVARIATION_V_ID) : "-") %></td>
																				</tr>
																			</ItemTemplate>
																		</asp:Repeater>
																	</table>
															</div>
															<%-- △ 固定データ △ --%>

															<%-- ▽ データ ▽ --%>
															<div class="div_data_right">
																<table class="list_table tableData" cellspacing="1" cellpadding="3" width="<%# Constants.REALSTOCK_OPTION_ENABLED ? 1498 : 798 %>" border="0">
														<asp:Repeater id="rEdit" Runat="server">
															<ItemTemplate>
																<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
																	<td width="290" align="left">
																		<%if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_PRODUCT_CONFIRM)) {%>
																			<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl((String)Eval(Constants.FIELD_PRODUCT_PRODUCT_ID))) %>"><%# WebSanitizer.HtmlEncode(CreateProductAndVariationName((DataRowView)Container.DataItem))%></a>
																		<% } else { %>
																			<%# WebSanitizer.HtmlEncode(CreateProductAndVariationName((DataRowView)Container.DataItem))%>
																		<% } %>
																	</td>
																	<td width="70" align="center"><%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(Constants.FIELD_PRODUCTSTOCK_STOCK))) %></td>
																	<td width="70" align="center">
																		<asp:TextBox id="tbStock" Runat="server" Width="40" Text="0" MaxLength="5"></asp:TextBox></td>
																	<%--▽ 実在庫利用が有効な場合は表示 ▽--%>
																	<% if (Constants.REALSTOCK_OPTION_ENABLED){ %>
																			<td width="70" align="center"><%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(Constants.FIELD_PRODUCTSTOCK_REALSTOCK))) %></td>
																			<td width="70" align="center"><asp:TextBox id="tbRealStock" Runat="server" Width="40" Text="0" MaxLength="5"></asp:TextBox></td>
																			<td width="70" align="center"><%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED))) %></td>
																			<td width="70" align="center"><asp:TextBox id="tbRealStockReserved" Runat="server" Width="40" Text="0" MaxLength="5"></asp:TextBox></td>
																			<td width="70" align="center"><%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_B))) %></td>
																			<td width="70" align="center"><asp:TextBox id="tbRealStockB" Runat="server" Width="40" Text="0" MaxLength="5"></asp:TextBox></td>
																			<td width="70" align="center"><%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_C))) %></td>
																			<td width="70" align="center"><asp:TextBox id="tbRealStockC" Runat="server" Width="40" Text="0" MaxLength="5"></asp:TextBox></td>
																	<% } %>
																	<%--△ 実在庫利用が有効な場合は表示 △--%>
																			<td width="70" align="center"><%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT))) %></td>
																			<td width="70" align="center">
																		<asp:TextBox id="tbStockAlert" Runat="server" Width="40" Text='<%# Eval(Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT) %>' MaxLength="5"></asp:TextBox>
																	</td>
																			<td width="0" align="center">
																		<asp:TextBox id="tbUpdateMemo" Runat="server" Width="<%# Constants.REALSTOCK_OPTION_ENABLED ? 120 : 70 %>" Text='' MaxLength="200"></asp:TextBox>
																	</td>
																			<td width="40" align="center"><a href="javascript:open_window('<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductStockHistoryList(HttpUtility.UrlEncode((string)Eval(Constants.FIELD_PRODUCT_PRODUCT_ID)),HttpUtility.UrlEncode((string)Eval(Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)))) %>','productstockhistory_list','width=930,height=585,top=120,left=320,status=NO,scrollbars=yes');">参照</a></td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
															</table>
															</div>
															<%-- △ データ △ --%>
														</div>
														<%-- △ テーブルデータ △ --%>

														<%-- ▽ エラーの場合 ▽ --%>
															<div style="<%= rEdit.Items.Count > 0 ? "display:none" : "" %>">
																<table class="list_table" cellspacing="1" cellpadding="3" width="734" border="0" style="height:100%"> <!-- 水平ヘッダ -->
														<tr id="trListEditError" class="list_alert" runat="server" Visible="false">
															<td id="tdListEditErrorMessage" runat="server" colspan="17"></td>
														</tr>
													</table>
													</div>
														<%-- △ エラーの場合 △ --%>
													</div>
													
												
												</td>
												<!--△ 編集一覧表示 △-->
											</tr>
											<tr>
												<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td>
													<!--▽ ページング ▽-->
													<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
														<tr>
															<td width="508"></td>
															<td class="action_list_sp"><div align="right" class="div_button_area">
																<asp:Button id="btnEditBottom" runat="server" Text="  編集する  " OnClick="btnEdit_Click" />
																<asp:Button id="btnListBottom" runat="server" Text="  一覧へ戻る  " OnClick="btnList_Click" />
																<asp:Button id="btnStockUpdateBottom" runat="server" Text="  一括更新  " OnClientClick="return exec_submit()" OnClick="btnStockUpdate_Click" /></div></td>
														</tr>
													</table>
													<!-- ページング-->
												</td>
											</tr>
											<% }%>
											<tr>
												<td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
										</table>
									</div>
									<div id="divStockComplete" runat="server" Visible="False">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td>
													<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
														<tr>
															<td width="675">以下の商品在庫情報を更新いたしました｡</TD>
															<td width="83" class="action_list_sp"><asp:Button id="btRedirectEditTop" Runat="server" Text="  続けて処理をする  " OnClick="btRedirectEdit_Click" CssClass="cmn-btn-sub-action" />	</td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td>
													<table class="list_table" cellspacing="1" cellpadding="3" width="<%# Constants.REALSTOCK_OPTION_ENABLED ? 750 : 750 %>" border="0">
														<%--▽ 実在庫利用が有効な場合は表示 ▽--%>
														<% if (Constants.REALSTOCK_OPTION_ENABLED){ %>
														<tr class="list_title_bg">
															<td align="center" width="105" rowspan="3">商品ID</td>
															<td align="center" width="105" rowspan="3">バリエーション<br />ID</td>
															<td align="center" width="540" colspan="6">処理結果</td>
														</tr>
														<tr class="list_title_bg">
															<td align="center" width="90" rowspan="2">論理在庫<br />更新</td>
															<td align="center" width="360" colspan="4">実在庫更新</td>
															<td align="center" width="90" rowspan="2">安全基準値<br />更新</td>
														</tr>
														<tr class="list_title_bg">
															<td align="center" width="90">A品<br />更新</td>
															<td align="center" width="90">A品 引当済<br />更新</td>
															<td align="center" width="90">B品<br />更新</td>
															<td align="center" width="90">C品<br />更新</td>
														</tr>
														<%--△ 実在庫利用が有効な場合は表示 △--%>
														<%--▽ 実在庫利用が無効な場合は表示 ▽--%>
														<%
														}else{ %>
														<tr class="list_title_bg">
															<td align="center" width="105" rowspan="2">商品ID</td>
															<td align="center" width="105" rowspan="2">バリエーションID</td>
															<td align="center" width="540" colspan="2">処理結果</td>
														</tr>
														<tr class="list_title_bg">
															<td align="center" width="270">論理在庫更新</td>
															<td align="center" width="270">安全基準値更新</td>
														</tr>
														<% } %>
														<%--△ 実在庫利用が無効な場合は表示 △--%>
														<asp:Repeater id="rComplete" Runat="server">
															<ItemTemplate>
																<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
																	<td align="center"><%# WebSanitizer.HtmlEncode(((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCT_PRODUCT_ID]) %></td>
																	<td align="center"><%# WebSanitizer.HtmlEncode(((string)((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCTVARIATION_V_ID] != "") ? (string)((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCTVARIATION_V_ID] : "-")%></td>
																	<td align="center"><%# WebSanitizer.HtmlEncode(DisplayResult(m_htUpdateStockResult[(string)((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCT_PRODUCT_ID + Constants.FIELD_PRODUCTSTOCK_VARIATION_ID]])) %></td>
																	<%--▽ 実在庫利用が有効な場合は表示 ▽--%>
																	<% if (Constants.REALSTOCK_OPTION_ENABLED){ %>
																	<td align="center"><%# WebSanitizer.HtmlEncode(DisplayResult(m_htUpdateRealStockResult[(string)((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCT_PRODUCT_ID + Constants.FIELD_PRODUCTSTOCK_VARIATION_ID]])) %></td>
																	<td align="center"><%# WebSanitizer.HtmlEncode(DisplayResult(m_htUpdateRealStockReservedResult[(string)((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCT_PRODUCT_ID + Constants.FIELD_PRODUCTSTOCK_VARIATION_ID]]))%></td>
																	<td align="center"><%# WebSanitizer.HtmlEncode(DisplayResult(m_htUpdateRealStockBResult[(string)((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCT_PRODUCT_ID + Constants.FIELD_PRODUCTSTOCK_VARIATION_ID]])) %></td>
																	<td align="center"><%# WebSanitizer.HtmlEncode(DisplayResult(m_htUpdateRealStockCResult[(string)((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCT_PRODUCT_ID + Constants.FIELD_PRODUCTSTOCK_VARIATION_ID]])) %></td>
																	<% } %>
																	<%--△ 実在庫利用が無効な場合は表示 △--%>
																	<td align="center"><%# WebSanitizer.HtmlEncode(DisplayResult(m_htUpdateStockAlertResult[(string)((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCT_PRODUCT_ID + Constants.FIELD_PRODUCTSTOCK_VARIATION_ID]])) %></td>
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
												<td class="action_list_sp"><asp:Button id="btRedirectEditBottom" Runat="server" Text="  続けて処理をする  " OnClick="btRedirectEdit_Click" CssClass="cmn-btn-sub-action" /></td>
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
function exec_submit()
{
	if (exec_submit_flg == 0)
	{
		exec_submit_flg = 1;
		return true;
	}
	else
	{
		return false;
	}
}
//-->
</script>

<script type="text/javascript">
	$(function () {
		$(".div_header_left").height($(".div_header_right").outerHeight());
		setHeightTwoTable("tableHeaderFixed", "tableHeader");

		$(".div_data_left").height($(".div_data_right").outerHeight());
		setWidthTwoTable("tableDataFix", "tableHeaderFixed");
		setHeightTwoTable("tableDataFix", "tableData");
		setHeightTwoTable("tableData", "tableDataFix"); // ※IE対応　２つテーブルを同じ高さを設定する

		scrollLeftTwoTable("div_header_right", "div_data_right");
		scrollTopTwoTable("div_data_left", "div_data_right");
	});

	$(window).bind("load", function () {
		setHeightTwoTable("tableDataFix", "tableData");
		setHeightTwoTable("tableData", "tableDataFix"); // ※IE対応　２つテーブルを同じ高さを設定する
		$(".div_data_left").height($(".div_data_right").outerHeight());
		$(".div_data").height($(".div_data_right").height());

		setMaxHeightTwoTable("tableData", "tableDataFix");

		<% if ((Constants.REALSTOCK_OPTION_ENABLED == false) && (this.DisplayKbn == Constants.KBN_PRODUCTSTOCK_DISPLAY_LIST)) { %>
			setWidthTwoTable("tableData", "tableHeader");
		<% } %>
	});

	// set max height two table
	function setMaxHeightTwoTable(tableTarget, tableReference) {
		var rowNums = $("." + tableTarget).find("tr").length;
		for (var i = 1; i < rowNums + 1; i++) {
			var height = $("." + tableReference + " tr:nth-child(" + i + ")").outerHeight() + 1;
			$("." + tableReference + " tr:nth-child(" + i + ")").css('max-height', height);
			$("." + tableTarget + " tr:nth-child(" + i + ")").css('max-height', height);
		}
	}

	var isMobile = getMobileOperatingSystem();
	if (isMobile) {
		$('.div_data_left').css('overflow-x', 'hidden');
		$('.div_header_right').css('overflow-y', 'hidden');
	}
	else {
		$('.div_data_left').css('overflow-x', 'scroll');
		$('.div_header_right').css('overflow-y', 'scroll');
	}

	$(window).bind('resize', function () {
		var isMobile = getMobileOperatingSystem();
		if (isMobile) {
			$('.div_data_left').css('overflow-x', 'hidden');
			$('.div_header_right').css('overflow-y', 'hidden');
		}
		else {
			$('.div_data_left').css('overflow-x', 'scroll');
			$('.div_header_right').css('overflow-y', 'scroll');
		}
	});
</script>

</asp:Content>