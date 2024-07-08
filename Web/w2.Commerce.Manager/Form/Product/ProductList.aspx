<%--
=========================================================================================================
  Module      : 商品情報一覧ページ(ProductList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ProductList.aspx.cs" Inherits="Form_Product_ProductList" %>
<%-- マスタ出力コントロール --%>
<%@ Register TagPrefix="uc" TagName="MasterDownLoad" Src="~/Form/Common/MasterDownLoadPanel.ascx" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<asp:Content id="Content2" ContentPlaceHolderid="ContentPlaceHolderHead" Runat="Server">
<link rel="stylesheet" type="text/css" href="../../Css/hide-field-button-style.css">
<meta http-equiv="Pragma" content="no-cache" />
<meta http-equiv="cache-control" content="no-cache" />
<meta http-equiv="expires" content="0" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">商品情報</h1></td>
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
													tbProductId.Attributes["onkeypress"]
														= tbProductId.Attributes["onkeypress"]
														= tbName.Attributes["onkeypress"]
														= tbSupplierId.Attributes["onkeypress"]
														= tbCooperationId1.Attributes["onkeypress"]
														= tbCooperationId2.Attributes["onkeypress"]
														= tbCategoryId.Attributes["onkeypress"]
														= tbDisplayPriority.Attributes["onkeypress"]
														= "if (event.keyCode==13){__doPostBack('" + btnSearch.UniqueID + "',''); return false;}";
												%>
												<table cellspacing="1" cellpadding="2" width="768" border="0">
													<tr>
														<td class="search_title_bg" width="98">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															商品ID</td>
														<td class="search_item_bg" width="130">

															<asp:TextBox id="tbProductId" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="100">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															商品名</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbName" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="100">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															並び順</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlSortKbn" runat="server" CssClass="search_item_width">
																<asp:ListItem Value="00" Selected="True">商品ID/昇順</asp:ListItem>
																<asp:ListItem Value="01">商品ID/降順</asp:ListItem>
																<asp:ListItem Value="02">商品名/昇順</asp:ListItem>
																<asp:ListItem Value="03">商品名/降順</asp:ListItem>
																<asp:ListItem Value="04">フリガナ/昇順</asp:ListItem>
																<asp:ListItem Value="05">フリガナ/降順</asp:ListItem>
																<asp:ListItem Value="06">作成日/昇順</asp:ListItem>
																<asp:ListItem Value="07">作成日/降順</asp:ListItem>
																<asp:ListItem Value="08">更新日/昇順</asp:ListItem>
																<asp:ListItem Value="09">更新日/降順</asp:ListItem>
															</asp:DropDownList>
														</td>
														<td class="search_btn_bg" width="83" rowspan="<%= 12 + (Constants.MEMBER_RANK_OPTION_ENABLED  ? 2 : 0) + (Constants.PRODUCTBUNDLE_OPTION_ENABLED ? 1 : 0)  %>">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_LIST %>">クリア</a>
																<a href="javascript:Reset();">リセット</a></div>
															<div class="search_btn_sub">
																<asp:LinkButton id="lbExportProductDetailUrl" Runat="server" OnClick="lbExprotProductDetailUrl_Click">商品詳細ページURL出力</asp:LinkButton></div>
															<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
															<div class="search_btn_sub">
																<asp:LinkButton id="lbExportTranslationData" Runat="server" OnClick="lbExportTranslationData_Click" >翻訳設定出力</asp:LinkButton></div>
															<% } %>
														</td>
													</tr>
													<tr id="hide-field_SupplierCoop" style="display: none;">
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															サプライヤID</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbSupplierId" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															商品連携ID1</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbCooperationId1" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															商品連携ID2</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbCooperationId2" runat="server" Width="125"></asp:TextBox></td>
													</tr>
													<tr id="hide-field_CooperationIDs" style="display: none;">
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															商品連携ID3</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbCooperationId3" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															商品連携ID4</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbCooperationId4" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															商品連携ID5</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbCooperationId5" runat="server" Width="125"></asp:TextBox></td>
													</tr>
													<tr>
													<td class="search_title_bg" width="95">
														<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
														有効フラグ</td>
													<%if (Constants.PRODUCT_BRAND_ENABLED){ %>
														<td class="search_item_bg"/>
													<% }else{%>
													<td class="search_item_bg">
														<% }%>
														<asp:DropDownList id="dllValidFlg" runat="server" CssClass="search_item_width"></asp:DropDownList>
													</td>
													<td class="search_title_bg" width="95">
														<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
														販売期間</td>
													<td class="search_item_bg" width="130">
														<asp:DropDownList id="ddlSell" runat="server" CssClass="search_item_width"></asp:DropDownList>
													</td>
													<td class="search_title_bg" width="95" id="hf_ShippingTypeTitle">
														<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
														配送種別</td>
													<td class="search_item_bg" width="130" id="hf_ShippingTypeBlank">
														<asp:DropDownList id="ddlShippingType" runat="server" DataTextField="Key" DataValueField="Value" CssClass="search_item_width"></asp:DropDownList>
													</td>

													</tr>
													<tr id="hide-field_ShippingSizeKbnType" style="display: none;">
														<%if (Constants.PRODUCT_BRAND_ENABLED){ %>
															<td class="search_title_bg">
																<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
																ブランドID
															</td>
															<td class="search_item_bg">
																<asp:DropDownList id="ddlBrandId" runat="server" CssClass="search_item_width"></asp:DropDownList>
															</td>
														<% }%>
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															配送サイズ区分</td>
														<td class="search_item_bg" width="130"  colspan="<%= Constants.PRODUCT_CTEGORY_OPTION_ENABLE ? 0 : 3 %>">
															<asp:DropDownList id="ddlShippingSizeKbn" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>
														<% if (Constants.PRODUCT_CTEGORY_OPTION_ENABLE) { %>
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															カテゴリID</td>
														<td class="search_item_bg" colspan="<%= Constants.PRODUCT_BRAND_ENABLED ? 0 : 3 %>">
															<asp:TextBox id="tbCategoryId" runat="server" Width="125"></asp:TextBox></td>
														<% } %>
													</tr>

													<tr id="hide-field_DisplayKbnSell" style="display: none;">
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															表示期間</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlDisplay" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															商品表示区分</td>
														<td class="search_item_bg" width="130" colspan="3">
															<asp:DropDownList id="ddlSearchDisplayKbn" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>
													</tr>
													<%if (Constants.MEMBER_RANK_OPTION_ENABLED){ %>
													<tr id="hide-field_BuyableMemberRank" style="display: none;">
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															会員ランク<br />割引対象
														</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="cbMemberRankDiscountFlg" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															閲覧可能<br />会員ランク
														</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlDisplayMemberRank" runat="server" Width="100"></asp:DropDownList>
															以上
														</td>
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															販売可能<br />会員ランク
														</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlBuyableMemberRank" runat="server" Width="90"></asp:DropDownList>
															以上
														</td>
													</tr>
													<%} %>
													<tr id="hide-field_DisplayPrioLimitedPayment" style="display: none;">
														<td class="search_title_bg" width="98">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															表示優先順</td>
														<td class="search_item_bg" width="130">
															<asp:TextBox id="tbDisplayPriority" runat="server" Width="100"></asp:TextBox>以上</td>
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															決済利用不可</td>
														<td class="search_item_bg" colspan="3">
															<asp:DropDownList id="ddlLimitedPayment" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>
													</tr>
													<% if (Constants.PRODUCTBUNDLE_OPTION_ENABLED) { %>
													<tr id="hide-field_ProductBundleItemType" style="display: none;">
														<td class="search_title_bg" width="98">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															商品区分
														</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList ID="ddlProductType" CssClass="search_item_width" runat="server" />
														</td>
														<td class="search_title_bg" width="98">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															明細表示
														</td>
														<td class="search_item_bg" colspan="3">
															<asp:DropDownList ID="ddlBundleItemDisplayType" CssClass="search_item_width" runat="server" />
														</td>
													</tr>
													<%} %>
													<tr id="hide-field_Colour" style="display: none;">
														<td class="search_title_bg" width="98">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															カラー
														</td>
														<td class="search_item_bg" colspan="5">
															<asp:DropDownList ID="ddlColors" CssClass="search_item_width" runat="server" DataTextField="text" DataValueField="value" />
														</td>
													</tr>
													<tr id="hide-field_Aicon" style="display: none;">
													<td class="search_title_bg" width="95">
														<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
														アイコン１～１０</td>
													<td class="search_item_bg" colspan="5" valign="middle">&nbsp;
														<asp:CheckBox ID="cbIconFlg1" Runat="server" Text=" １：" TextAlign="Left"></asp:CheckBox>&nbsp;
														<asp:CheckBox ID="cbIconFlg2" Runat="server" Text=" ２：" TextAlign="Left"></asp:CheckBox>&nbsp;
														<asp:CheckBox ID="cbIconFlg3" Runat="server" Text=" ３：" TextAlign="Left"></asp:CheckBox>&nbsp;
														<asp:CheckBox ID="cbIconFlg4" Runat="server" Text=" ４：" TextAlign="Left"></asp:CheckBox>&nbsp;
														<asp:CheckBox ID="cbIconFlg5" Runat="server" Text=" ５：" TextAlign="Left"></asp:CheckBox>&nbsp;
														<asp:CheckBox ID="cbIconFlg6" Runat="server" Text=" ６：" TextAlign="Left"></asp:CheckBox>&nbsp;
														<asp:CheckBox ID="cbIconFlg7" Runat="server" Text=" ７：" TextAlign="Left"></asp:CheckBox>&nbsp;
														<asp:CheckBox ID="cbIconFlg8" Runat="server" Text=" ８：" TextAlign="Left"></asp:CheckBox>&nbsp;
														<asp:CheckBox ID="cbIconFlg9" Runat="server" Text=" ９：" TextAlign="Left"></asp:CheckBox>&nbsp;
														<asp:CheckBox ID="cbIconFlg10" Runat="server" Text=" １０：" TextAlign="Left"></asp:CheckBox>&nbsp;
													</td>
													</tr>
													<tr id="hide-field_ProductTaxCategory" style="display: none;">
														<td class="search_title_bg" width="98">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															商品税率カテゴリ
														</td>
														<td class="search_item_bg" colspan="5">
															<asp:DropDownList ID="ddlProductTaxCategory" CssClass="search_item_width" runat="server" DataTextField="text" DataValueField="value" />
														</td>
													</tr>
													<% if(Constants.SUBSCRIPTION_BOX_OPTION_ENABLED) { %>
													<tr id="hide-field_pSubscriptionBox" style="display: none;">
														<td class="search_title_bg" width="98">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															頒布会フラグ
														</td>
														<td class="search_item_bg" colspan="5">
															<asp:DropDownList ID="ddlSubscriptionBoxFlg" CssClass="search_item_width" runat="server" DataTextField="text" DataValueField="value" />
														</td>
													</tr>
													<% } %>
													<tr id="hide-field_ProductSellFromDate" style="display: none;">
														<td class="search_title_bg" width="130">
																<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
																販売開始日</td>
														<td class="search_item_bg" colspan="5">
															<div id="productSellFromDate">
																<uc:DateTimePickerPeriodInput id="ucProductSellFromDate" runat="server" IsNullStartDateTime="true" />
																<span class="search_btn_sub">(<a href="Javascript:SetToday('sellfrom');">今日</a>｜<a href="Javascript:SetThisMonth('sellfrom');">今月</a>)</span>
															</div>	
														</td>
													</tr>
													<tr id="hide-field_ProductSellToDate" style="display: none;">
														<td class="search_title_bg" width="130">
																<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
																販売終了日
														</td>
														<td class="search_item_bg" colspan="5">
															<div id="productSellToDate">
																<uc:DateTimePickerPeriodInput id="ucProductSellToDate" runat="server" IsNullStartDateTime="true" />
																<span class="search_btn_sub">(<a href="Javascript:SetToday('sellto');">今日</a>｜<a href="Javascript:SetThisMonth('sellto');">今月</a>)</span>
															</div>
														</td>
													</tr>
													<% if (Constants.MEMBER_RANK_OPTION_ENABLED) { %>
													<tr id="hide-field_DisplayBuyableFixedPurchase" style="display: none;">
														<td class="search_title_bg" width="98">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															定期会員限定フラグ（閲覧）
														</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList ID="ddDisplayFixedPurchaseMemberLimitFlg" CssClass="search_item_width" runat="server" />
														</td>
														<td class="search_title_bg" width="98">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />
															定期会員限定フラグ（購入）
														</td>
														<td class="search_item_bg" colspan="3">
															<asp:DropDownList ID="ddBuyableFixedPurchaseMemberLimitFlg" CssClass="search_item_width" runat="server" />
														</td>
													</tr>
													<% } %>
												</table>

												<div id="product-hide-search-field-slide-toggle" style="text-align: center;">
													<span id="check-toggle-text-product">全ての検索項目を表示</span>
													<span id="check-toggle-open">
														<span class="toggle-state-icon icon-arrow-down"></span>
													</span>
												</div>

											</td>
										</tr>
										<tr><td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
										<%-- マスタ出力 --%>
										<tr>
											<td class="search_table">
												<uc:MasterDownLoad runat="server" ID="uMasterDownload" DownloadType="Product" TableWidth="768" />
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
	<tr id="productListSearchResult">
		<td><h2 class="cmn-hed-h2">商品情報一覧</h2></td>
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
																		<% if (Constants.FLAPS_OPTION_ENABLE) { %>
																			<asp:Button ID="btnGetLatestInfoFromErpTop" runat="server" Text="  ERPから最新情報取得  " OnClick="btnGetLatestInfoFromErp_OnClick" />
																		<% } %>
																		<asp:Button ID="btnDefaultSettingTop" runat="server" Text="  初期設定  " OnClick="btnDefaultSetting_Click" />
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
												<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="list_title_bg">
														<td align="center" width="15%" style="height: 17px">商品ID</td>
														<td align="center" width="43%" style="height: 17px">商品名</td>
														<td align="center" width="10%" style="height: 17px">表示価格（<%: this.ProductPriceTextPrefix %>）</td>
														<% if (Constants.PRODUCT_STOCK_OPTION_ENABLE){ %>
														<td align="center" width="8%" style="height: 17px">在庫管理</td>
														<% } %>
														<td align="center" width="8%" style="height: 17px">表示期間</td>
														<td align="center" width="8%" style="height: 17px">販売期間</td>
														<td align="center" width="8%" style="height: 17px">有効フラグ</td>
													</tr>
													<asp:Repeater id="rList" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl((String)Eval(Constants.FIELD_PRODUCT_PRODUCT_ID))) %>')">
																<td align="center"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCT_PRODUCT_ID)) %>
																	<span id="Span1" class="notice" visible="<%# (string)Eval(Constants.FIELD_PRODUCT_USE_VARIATION_FLG) == Constants.FLG_PRODUCT_USE_VARIATION_FLG_USE_USE %>" runat="server">*</span></td>
																<td align="left"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCT_NAME))%></td>
																<td align="right"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_PRODUCT_DISPLAY_PRICE).ToPriceString(true)) %></td>
																<% if (Constants.PRODUCT_STOCK_OPTION_ENABLE) { %>
																<td align="center"><%# WebSanitizer.HtmlEncode((string)Eval(Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN) != Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED ? "する" : "しない") %></td>
																<% } %>
																<td align="center"><%# WebSanitizer.HtmlEncode(Eval("display")) %></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(Eval("sell")) %></td>
																<td align="center"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_VALID_FLG, Eval(Constants.FIELD_PRODUCT_VALID_FLG)))%></td>
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
											<td>
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="info_item_bg">
														<td align="left">備考
															<br />商品IDの後に<span class="notice">*</span>がある商品は、バリエーションが設定されています。
															<br />商品連携ID項目はバリエーションも参照します。
														</td>
													</tr>
												</table>
											</td>
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
																		<% if (Constants.FLAPS_OPTION_ENABLE) { %>
																			<asp:Button ID="btnGetLatestInfoFromErpBottom" runat="server" Text="  ERPから最新情報取得  " OnClick="btnGetLatestInfoFromErp_OnClick" />
																		<% } %>
																		<asp:Button ID="btnDefaultSettingBottom" runat="server" Text="  初期設定  " OnClick="btnDefaultSetting_Click" />
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
<script type="text/javascript">
<!--
	// 今日設定
	function SetToday(set_date)
	{
		if (set_date == 'sellfrom')
		{
			document.getElementById('<%= ucProductSellFromDate.HfStartDate.ClientID %>').value = '<%= DateTime.Now.ToString("yyyy/MM/dd") %>';
			document.getElementById('<%= ucProductSellFromDate.HfEndDate.ClientID %>').value = '<%= DateTime.Now.ToString("yyyy/MM/dd") %>';
			document.getElementById('<%= ucProductSellFromDate.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucProductSellFromDate.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucProductSellFromDate.ClientID %>');
		}
		else if (set_date == 'sellto')
		{
			document.getElementById('<%= ucProductSellToDate.HfStartDate.ClientID %>').value = '<%= DateTime.Now.ToString("yyyy/MM/dd") %>';
			document.getElementById('<%= ucProductSellToDate.HfEndDate.ClientID %>').value = '<%= DateTime.Now.ToString("yyyy/MM/dd") %>';
			document.getElementById('<%= ucProductSellToDate.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucProductSellToDate.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucProductSellToDate.ClientID %>');
		}
	}

	// 今月設定
	function SetThisMonth(set_date)
	{
		if (set_date == 'sellfrom') {
			document.getElementById('<%= ucProductSellFromDate.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetStartDateThisMonthString() %>';
			document.getElementById('<%= ucProductSellFromDate.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucProductSellFromDate.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetEndDateThisMonthString() %>';
			document.getElementById('<%= ucProductSellFromDate.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucProductSellFromDate.ClientID %>');
		}
		else if (set_date == 'sellto') {
			document.getElementById('<%= ucProductSellToDate.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetStartDateThisMonthString() %>';
			document.getElementById('<%= ucProductSellToDate.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucProductSellToDate.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetEndDateThisMonthString() %>';
			document.getElementById('<%= ucProductSellToDate.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucProductSellToDate.ClientID %>');
		}
	}

	// Reset
	function Reset() {
		document.getElementById('<%= ucProductSellFromDate.HfStartDate.ClientID %>').value = '';
		document.getElementById('<%= ucProductSellFromDate.HfStartTime.ClientID %>').value = '';
		document.getElementById('<%= ucProductSellFromDate.HfEndDate.ClientID %>').value = '';
		document.getElementById('<%= ucProductSellFromDate.HfEndTime.ClientID %>').value = '';
		reloadDisplayDateTimePeriod('<%= ucProductSellFromDate.ClientID %>');

		document.getElementById('<%= ucProductSellToDate.HfStartDate.ClientID %>').value = '';
		document.getElementById('<%= ucProductSellToDate.HfStartTime.ClientID %>').value = '';
		document.getElementById('<%= ucProductSellToDate.HfEndDate.ClientID %>').value = '';
		document.getElementById('<%= ucProductSellToDate.HfEndTime.ClientID %>').value = '';
		reloadDisplayDateTimePeriod('<%= ucProductSellToDate.ClientID %>');
		document.getElementById('<%= this.Form.ClientID %>').reset();
	}
//-->
</script>
<%--// 検索時の非表示--%>
<script type="text/javascript" src="<%= ResolveUrl("~/Js/hide-show_search_field.js") %>"></script>

</asp:Content>
