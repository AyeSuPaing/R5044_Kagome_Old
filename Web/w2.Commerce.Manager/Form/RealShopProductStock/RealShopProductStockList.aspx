<%--
=========================================================================================================
  Module      : リアル店舗在庫情報一覧ページ(RealShopProductStockList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="RealShopProductStockList.aspx.cs" Inherits="Form_RealShopProductStock_RealShopProductStockList" MaintainScrollPositionOnPostback="true"%>
<%-- マスタ出力コントロール --%>
<%@ Register TagPrefix="uc" TagName="MasterDownLoad" Src="~/Form/Common/MasterDownLoadPanel.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
<%-- 
テキストボックス内でEnterキーを押してSubmit（一番上に配置されているTextBoxのSubmit）送信しようとすると、
IEのバグでテキストボックスが画面上に一つのみ配置されている場合にSubmit送信されない不具合の対応として、
ダミーのTextBoxを非表示で配置している。
--%>
	<asp:TextBox ID="tbDummy" runat="server" Style="visibility: hidden; display: none;"></asp:TextBox>
	<table cellspacing="0" cellpadding="0" width="791" border="0">
		<tr id="trTitleMiddle" runat="server">
			<td><h1 class="page-title">リアル店舗在庫情報</h1></td>
		</tr>
		<tr id="trTitleBottom" runat="server">
			<td>
				<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
			</td>
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
												<td>
													<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
												</td>
											</tr>
											<tr>
												<td class="search_table">
													<table cellspacing="1" cellpadding="2" width="758" border="0">
														<tr>
															<td class="search_title_bg" width="90">
																<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
																リアル店舗ID
															</td>
															<td class="search_item_bg" width="120">
																<%-- リアル店舗ID選択用（リピータで実装） --%>
																<% 
																	tbRealShopId.Attributes.Add("autocomplete", "on");
																	tbRealShopId.Attributes.Add("list", "realshopid");
																%>
																<asp:Repeater ID="rShopIdAutoComplete" runat="server">
																	<HeaderTemplate>
																		<datalist id="realshopid">
																	</HeaderTemplate>
																	<ItemTemplate>
																		<option value="<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_REALSHOP_REAL_SHOP_ID))%>" label="<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_REALSHOP_NAME))%>"></option>
																	</ItemTemplate>
																	<FooterTemplate>
																		</datalist>
																	</FooterTemplate>
																</asp:Repeater>
																<asp:TextBox ID="tbRealShopId" runat="server" Width="125"></asp:TextBox>
															</td>
															<td class="search_title_bg" width="90">
																<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
																店舗名
															</td>
															<td class="search_item_bg" width="120">
																<%-- リアル店舗名選択用（リピータで実装） --%>
																<% 
																	tbRealShopName.Attributes.Add("autocomplete", "on");
																	tbRealShopName.Attributes.Add("list", "realshopname");
																%>
																<asp:Repeater ID="rShopNameAutoComplete" runat="server">
																	<HeaderTemplate>
																		<datalist id="realshopname">
																	</HeaderTemplate>
																	<ItemTemplate>
																		<option value="<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_REALSHOP_NAME))%>" label="<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_REALSHOP_NAME))%>"></option>
																	</ItemTemplate>
																	<FooterTemplate>
																		</datalist>
																	</FooterTemplate>
																</asp:Repeater>
																<asp:TextBox ID="tbRealShopName" runat="server" Width="125"></asp:TextBox>
															</td>
															<td class="search_title_bg" width="90">
																<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
																並び順
															</td>
															<td class="search_item_bg" width="120">
																<asp:DropDownList ID="ddlSortKbn" runat="server" CssClass="search_item_width" />
															</td>
															<td class="search_btn_bg" width="83" rowspan="3">
																<div class="search_btn_main">
																	<asp:Button ID="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
																<div class="search_btn_sub">
																	<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_REALSHOPPRODUCTSTOCK_LIST %>">
																		クリア</a> <a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a>
																</div>
															</td>
														</tr>
														<tr>
															<td class="search_title_bg" width="90">
																<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
																商品ID
															</td>
															<td class="search_item_bg" width="120">
																<asp:TextBox ID="tbProductId" runat="server" Width="125"></asp:TextBox>
															</td>
															<td class="search_title_bg" width="90">
																<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
																バリエーションID
															</td>
															<td class="search_item_bg" width="120">
																<asp:TextBox ID="tbVariationId" runat="server" Width="125"></asp:TextBox>
															</td>
															<td class="search_title_bg" width="90">
																<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
																商品名
															</td>
															<td class="search_item_bg" width="120">
																<asp:TextBox ID="tbProductName" runat="server" Width="125"></asp:TextBox>
															</td>
														</tr>
														<tr>
															<td class="search_title_bg" width="90">
																<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />在庫数検索
															</td>
															<td class="search_item_bg" colspan="5">
																在庫数が
																<asp:TextBox ID="tbSearchStockCount" Width="50" runat="server">0</asp:TextBox>
																<asp:DropDownList ID="ddlSearchProductCountType" runat="server" CssClass="search_item_width">
																</asp:DropDownList>
															</td>
														</tr>
													</table>
												</td>
											</tr>
											<tr><td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
											<tr>
												<td class="search_table">
													<uc:MasterDownLoad runat="server" ID="uMasterDownload" DownloadType="RealShopProductStock" TableWidth="758" />
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
			<td>
				<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
			</td>
		</tr>
		<!--▽ 一覧 ▽-->
		<tr id="trList" runat="server">
			<td>
				<h2 class="cmn-hed-h2">リアル店舗在庫情報一覧</h2>
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
										<!--▽ 一覧編集表示 ▽-->
										<div id="divStockEdit" runat="server">
											<table cellspacing="0" cellpadding="0" border="0">
												<tr>
													<td>
														<img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
													</td>
												</tr>
												<% if (this.IsNotSearchDefault) { %>
												<tr>
													<td>
														<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
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
														<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
															<tr>
																<td width="508">
																	<asp:Label ID="lbPager1" runat="server"></asp:Label>
																</td>
																<td class="action_list_sp">
																	<div align="right">
																		<asp:Button ID="btnStockUpdateTop" runat="server" Text="  一括更新  " OnClientClick="return exec_submit()"
																			OnClick="btnStockUpdate_Click" /></div>
																</td>
															</tr>
														</table>
														<!-- ページング-->
													</td>
												</tr>
												<tr>
													<td>
														<img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
													</td>
												</tr>
												<tr>
													<td id="tdDispList" runat="server">
														<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
															<tr class="list_title_bg">
																<td align="center" width="120">
																	リアル店舗ID
																</td>
																<td align="center" width="120">
																	商品ID
																</td>
																<td align="center" width="120">
																	バリエーションID
																</td>
																<td align="center" width="253">
																	商品名
																</td>
																<td align="center" width="50">
																	在庫数
																</td>
																<td align="center" width="50">
																	＋/－
																</td>
																<td align="center" width="35">
																	削除
																	<br/>
																	<input id="deleteTargetAll" name="deleteTargetAll" type="checkbox" style="vertical-align: middle" />
																</td>
															</tr>
															<asp:Repeater ID="rList" runat="server">
																<ItemTemplate>
																	<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)"
																		onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)">
																		<td align="left">
																			<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_ID))%>
																			<asp:HiddenField ID="hfRealShopId" runat="server" Value="<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_ID))%>">
																			</asp:HiddenField>
																		</td>
																		<td align="left">
																			<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_REALSHOPPRODUCTSTOCK_PRODUCT_ID))%>
																			<asp:HiddenField ID="hfProductId" runat="server" Value="<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_REALSHOPPRODUCTSTOCK_PRODUCT_ID))%>">
																			</asp:HiddenField>
																		</td>
																		<td align="left">
																			<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_REALSHOPPRODUCTSTOCK_VARIATION_ID))%>
																			<asp:HiddenField ID="hfVariationId" runat="server" Value="<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_REALSHOPPRODUCTSTOCK_VARIATION_ID))%>">
																			</asp:HiddenField>
																		</td>
																		<td align="left">
																			<%# StringUtility.ToEmpty(Eval(Constants.FIELD_REALSHOPPRODUCTSTOCK_VARIATION_ID)) == string.Empty
																					? WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCT_NAME)) : w2.App.Common.Order.ProductCommon.CreateProductJointName(Container.DataItem)%>
																			<asp:HiddenField ID="hfProductName" runat="server" Value='<%# StringUtility.ToEmpty(Eval(Constants.FIELD_REALSHOPPRODUCTSTOCK_VARIATION_ID)) == string.Empty ? WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCT_NAME)) : WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCT_NAME)) + "(" + WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1)) + "-" + WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2)) + ")"%>'>
																			</asp:HiddenField>
																		</td>
																		<td align="center">
																			<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK))%>
																			<asp:HiddenField ID="hfRealShopStock" runat="server" Value="<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK))%>">
																			</asp:HiddenField>
																		</td>
																		<td align="center">
																			<asp:TextBox ID="tbStockNumber" runat="server" Width="50" MaxLength="5"></asp:TextBox>
																		</td>
																		<td align="center">
																			<asp:CheckBox id="cbDelete" runat="server" CssClass="deleteTarget" />
																		</td>
																	</tr>
																</ItemTemplate>
															</asp:Repeater>
															<tr id="trListError" class="list_alert" runat="server" visible="false">
																<td id="tdListErrorMessage" runat="server" colspan="7">
																</td>
															</tr>
														</table>
													</td>
												</tr>
												<tr>
													<td>
														<img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
													</td>
												</tr>
												<tr>
													<td>
														<!--▽ ページング ▽-->
														<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
															<tr>
																<td width="508">
																</td>
																<td class="action_list_sp">
																	<div align="right">
																		<asp:Button ID="btnStockUpdateBottom" runat="server" Text="  一括更新  " OnClientClick="return exec_submit()"
																			OnClick="btnStockUpdate_Click" /></div>
																</td>
															</tr>
														</table>
														<!-- ページング-->
													</td>
												</tr>
												<% } %>
												<tr>
													<td>
														<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
													</td>
												</tr>
											</table>
										</div>
										<!--△ 一覧編集表示 △-->
										<!--▽ 処理結果表示（この時、一覧編集画面＆登録画面は非表示） ▽-->
										<div id="divStockComplete" runat="server" visible="false">
											<table cellspacing="0" cellpadding="0" border="0">
												<tr>
													<td>
														<img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
													</td>
												</tr>
												<tr>
													<td>
														<div>
															<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
																<tr class="info_item_bg">
																	<td align="left">
																		リアル店舗在庫情報を更新しました。
																	</td>
																</tr>
															</table>
															<img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
														</div>
													</td>
												</tr>
												<tr>
													<td>
														<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
															<tr>
																<td width="675">
																</td>
																<td class="action_list_sp">
																	<asp:Button ID="btRedirectEditTop" runat="server" Text="  続けて処理をする  " OnClick="btRedirectEdit_Click" />
																</td>
															</tr>
														</table>
													</td>
												</tr>
												<tr>
													<td>
														<img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
													</td>
												</tr>
												<tr>
													<td>
														<asp:Repeater ID="rListComplete" runat="server">
															<HeaderTemplate>
																<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
																	<tr class="list_title_bg">
																		<td align="center" width="120" class="style1">
																			リアル店舗ID
																		</td>
																		<td align="center" width="120" class="style1">
																			商品ID
																		</td>
																		<td align="center" width="120" class="style1">
																			バリエーションID
																		</td>
																		<td align="center" width="410" class="style1">
																			処理結果
																		</td>
																	</tr>
															</HeaderTemplate>
															<ItemTemplate>
																<tr class="list_item_bg1">
																	<td align="left">
																		<%# WebSanitizer.HtmlEncode(((Hashtable)Container.DataItem)[Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_ID]) %>
																	</td>
																	<td align="left">
																		<%# WebSanitizer.HtmlEncode(((Hashtable)Container.DataItem)[Constants.FIELD_REALSHOPPRODUCTSTOCK_PRODUCT_ID]) %>
																	</td>
																	<td align="left">
																		<%# WebSanitizer.HtmlEncode(((Hashtable)Container.DataItem)[Constants.FIELD_REALSHOPPRODUCTSTOCK_VARIATION_ID]) %>
																	</td>
																	<td align="center">
																		<%# (bool)((Hashtable)Container.DataItem)["result"]  ? "○" : "×"%>
																	</td>
																</tr>
															</ItemTemplate>
															<FooterTemplate>
																</table>
															</FooterTemplate>
														</asp:Repeater>
													</td>
												</tr>
												<tr>
													<td>
														<img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
													</td>
												</tr>
												<tr>
													<td class="action_list_sp">
														<asp:Button ID="btRedirectEditBottom" runat="server" Text="  続けて処理をする  " OnClick="btRedirectEdit_Click" />
													</td>
												</tr>
												<tr>
													<td>
														<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
													</td>
												</tr>
											</table>
										</div>
										<!--▽ 処理結果表示（この時、一覧編集画面＆登録画面は非表示） ▽-->
									</td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<!--△ 一覧 △-->
		<!--▽ 登録 ▽-->
		<tbody id="tbdyRegist" runat="server">
			<tr>
				<td>
					<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
				</td>
			</tr>
			<tr>
				<td>
					<h2 class="cmn-hed-h2">リアル店舗在庫情報登録</h2>
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
											<div id="divStockAddProduct" runat="server">
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td>
															<img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
														</td>
													</tr>
													<tr>
														<td>
															<img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
														</td>
													</tr>
													<tr id="trRegistMessage" runat="server" visible="false">
														<td>
															<div id="divStockRegistComplete" runat="server" visible="false">
																<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
																	<tr class="info_item_bg">
																		<td align="left">
																			<asp:Literal ID="ltMessageRegist" Text="リアル店舗在庫情報を登録しました。" runat="server"></asp:Literal>
																		</td>
																	</tr>
																</table>
																<img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
															</div>
															<div id="divStockRegistError" runat="server" visible="false">
																<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
																	<tr class="info_item_bg">
																		<td align="left">
																			<asp:Label ID="lbMessageError" runat="server" ForeColor="Red"></asp:Label>
																		</td>
																	</tr>
																</table>
																<img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
															</div>
														</td>
													</tr>
													<tr>
														<td>
															<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
																<tr class="edit_title_bg">
																	<td align="center" width="180" rowspan="2">
																		リアル店舗ID<span class="notice">*</span>
																	</td>
																	<td align="center" width="180">
																		商品ID<span class="notice">*</span>
																	</td>
																	<td align="center" width="328" rowspan="2">
																		商品名
																	</td>
																	<td align="center" width="70" rowspan="2">
																		在庫数<span class="notice">*</span>
																	</td>
																</tr>
																<tr class="edit_title_bg">
																	<td align="center">
																		(商品ID+) バリエーションID<span class="notice">*</span>
																	</td>
																</tr>
																<tr class="detail_item_bg">
																	<td align="center" rowspan="2">
																		<% 
																			tbRealShopIdRegist.Attributes.Add("autocomplete", "on");
																			tbRealShopIdRegist.Attributes.Add("list", "realshopid");
																		%>
																		<asp:TextBox ID="tbRealShopIdRegist" Width="160" runat="server"></asp:TextBox>
																	</td>
																	<td align="left">
																		<asp:TextBox ID="tbProductIdRegist" Width="160" runat="server"></asp:TextBox>
																	</td>
																	<td align="left" rowspan="2">
																		<asp:Label ID="lbProductName" runat="server"></asp:Label>
																		<asp:HiddenField ID="hfProductName" runat="server"></asp:HiddenField>
																	</td>
																	<td align="center" rowspan="2">
																		<asp:TextBox ID="tbStockRegist" Width="50" runat="server" MaxLength="5"></asp:TextBox>
																	</td>
																</tr>
																<tr class="detail_item_bg">
																	<td align="left">
																		<asp:TextBox ID="tbVariationIdRegist" Width="120" runat="server"></asp:TextBox>
																		<input id="inputSearchProduct" type="button" value="  検索  " onclick="javascript:open_product_list('<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_SEARCH + "?" + Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN + "=" + HttpUtility.UrlEncode(Constants.KBN_PRODUCT_SEARCH_VARIATION) + "&" + Constants.REQUEST_KEY_PRODUCT_VALID_FLG + "=" + Constants.FLG_PRODUCT_VALID_FLG_VALID) %>','product_search','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes','0');"/>
																		<div style="display: none">
																			<asp:Button ID="btnGetInfo" Text="  取得  " runat="server" OnClick="btnGetInfo_Click" />
																		</div>
																	</td>
																</tr>
															</table>
														</td>
													</tr>
													<tr>
														<td>
															<img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
														</td>
													</tr>
													<tr>
														<td class="action_list_sp">
															<asp:Button ID="btnRegist" Text="  登録する  " runat="server" OnClientClick="return exec_submit()"
																OnClick="btnRegist_Click" />
														</td>
													</tr>
													<tr>
														<td>
															<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
														</td>
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
		</tbody>
		<!--△ 登録 △-->
		<tr>
			<td>
				<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
			</td>
		</tr>
	</table>
	<script type="text/javascript">
	<!--
		// 商品一覧画面表示
		function open_product_list(link_file, window_name, window_type, index) {
			// 選択商品を格納
			selected_index = index;
			// ウィンドウ表示
			open_window(link_file, window_name, window_type);
		}

		var exec_submit_flg = 0;
		function exec_submit() {
			if (exec_submit_flg == 0) {
				exec_submit_flg = 1;
				return true;
			}
			else {
				return false;
			}
		}

		// 商品一覧で選択された商品情報を設定
		function set_productinfo(product_id, supplier_id, v_id, product_name, display_price, display_special_price, product_price, sale_id, fixed_purchase_id) {
			document.getElementById("<%=tbProductIdRegist.ClientID%>").value = product_id;
			document.getElementById("<%=tbVariationIdRegist.ClientID%>").value = v_id;
			document.getElementById("<%=hfProductName.ClientID%>").value = product_name;
			__doPostBack("<%=btnGetInfo.UniqueID%>", "");
		}

		// 削除対象全選択・全解除
		$("#deleteTargetAll").click(function () {
			$('.deleteTarget input:checkbox').prop('checked', $(this).prop('checked'));
		});
	//-->
</script>
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="ContentPlaceHolderHead">
	<style type="text/css">
		.style1
		{
			height: 19px;
		}
	</style>
</asp:Content>
