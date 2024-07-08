<%--
=========================================================================================================
  Module      : 広告コードレポート詳細ページ(AdvertisementCodeReportDetail.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Data" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="AdvertisementCodeReportDetail.aspx.cs" Inherits="Form_AdvertisementCodeReport_AdvertisementCodeReportDetail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="628" border="0">
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
													<tr style="height: 22px">
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															広告コード</td>
														<td class="search_item_bg" width="130">
															<asp:Literal ID="lAdvertisementCode" runat="server"></asp:Literal><asp:HiddenField ID="hfDsipKbn" runat="server" /></td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															媒体名</td>
														<td class="search_item_bg" width="130">
															<asp:Literal ID="lMediaName" runat="server"></asp:Literal></td>
													</tr>
													<tr style="height: 22px">
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															集計区分</td>
														<td class="search_item_bg" width="130">
															<asp:Literal ID="lCareer" runat="server"></asp:Literal></td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															対象広告コード</td>
														<td class="search_item_bg" width="130">
															<asp:Literal ID="lAdvertisementCodeTarget" runat="server"></asp:Literal></td>

													</tr>
													<tr style="height: 22px">
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															期間</td>
														<td class="search_item_bg" width="130" colspan="3">
															<asp:Literal ID="lTerm1From" runat="server"></asp:Literal>&nbsp;～&nbsp;<asp:Literal ID="lTerm1To" runat="server"></asp:Literal></td>
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
	<tr>
		<td>
			<h2 class="cmn-hed-h2">レポート表示</h2>
			<div class="search_btn_sub_rt" style="float: right; margin-top: 8px;"><asp:LinkButton id="lbReportExport" Runat="server" OnClick="lbReportExport_Click">  CSVダウンロード  </asp:LinkButton></div>
		</td>
	</tr>
	<!--▽ 一覧 ▽-->
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
										<%--▽ 購入者詳細のみ表示 ▽--%>
										<tbody id="tbodyPager1" runat="server">
										<tr>
											<td>
												<!--▽ ページング ▽-->
												<table class="list_pager" cellspacing="0" cellpadding="0" width="604" border="0">
													<tr>
														<td width="604" style="height: 22px"><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
													</tr>
												</table>
												<!-- ページング-->
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										</tbody>
										<%--△ 購入者詳細のみ表示 △--%>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="604" border="0">
													<!--▽ 商品別売れ行きランキング ▽-->
													<asp:Repeater id="rListRank" Runat="server">
														<HeaderTemplate>
															<tr class="list_title_bg">
																<td align="center" width="34">Rank</td>
																<td align="center" width="300">注文商品名</td>
																<td align="center" width="60">注文数</td>
																<td align="center" width="60">構成比</td>
																<td align="center" width="90">注文商品金額</td>
																<td align="center" width="60">構成比</td>
															</tr>
														</HeaderTemplate>
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" style="word-break: break-all">
																<td align="center" width="34"><%# WebSanitizer.HtmlEncode(StringUtility.ToEmpty(Eval("rank"))) %></td>
																<td align="left" width="310"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_ORDERITEM_PRODUCT_NAME))%></td>
																<td align="right" width="60"><%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(Constants.FIELD_ORDERITEM_ITEM_QUANTITY)))%>個</td>
																<td align="right" width="60"><%# WebSanitizer.HtmlEncode(DispRate(Eval(Constants.FIELD_ORDERITEM_ITEM_QUANTITY), Eval(Constants.FIELD_ORDERITEM_ITEM_QUANTITY + "_total")) + "%")%></td>
																<td align="right" width="80"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_ORDERITEM_ITEM_PRICE).ToPriceString(true))%></td>
																<td align="right" width="60"><%# WebSanitizer.HtmlEncode(DispRate(Eval(Constants.FIELD_ORDERITEM_ITEM_PRICE), Eval(Constants.FIELD_ORDERITEM_ITEM_PRICE + "_total")) + "%")%></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<!--△ 商品別売れ行きランキング △-->
													<%--▽ 購入者詳細 ▽--%>
													<asp:Repeater id="rListOwner" Runat="server">
														<HeaderTemplate>
															<tr class="list_title_bg">
																<td align="center" width="34">項番</td>
																<td align="center" width="70">広告コード</td>
																<td align="center" width="75">注文日</td>
																<td align="center" width="220">注文商品名</td>
																<td align="center" width="125">注文者</td>
																<td align="center" width="90">注文商品金額</td>
															</tr>
														</HeaderTemplate>
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" style="word-break: break-all">
																<td align="center" width="34"><%# WebSanitizer.HtmlEncode(StringUtility.ToEmpty(Eval("row_num"))) %></td>
																<td align="left" width="70"><%# (StringUtility.ToValue(Request[Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE_TARGET], KBN_ADVC_ADVERTISEMENT_CODE_TARGET_ORDER).ToString() != KBN_ADVC_ADVERTISEMENT_CODE_TARGET_ORDER) ? WebSanitizer.HtmlEncode(StringUtility.ToEmpty(Eval(Constants.FIELD_ORDER_ADVCODE_FIRST))) : WebSanitizer.HtmlEncode(StringUtility.ToEmpty(Eval(Constants.FIELD_ORDER_ADVCODE_NEW))) %></td>
																<td align="left" width="75"><%#: DateTimeUtility.ToStringForManager(Eval(Constants.FIELD_ORDER_ORDER_DATE), DateTimeUtility.FormatType.ShortDate2Letter) %></td>
																<td align="left" width="240"><%# WebSanitizer.HtmlEncode(StringUtility.ToEmpty(Eval(Constants.FIELD_ORDERITEM_PRODUCT_NAME))) %></td>
																<td align="left" width="125"><%# WebSanitizer.HtmlEncode(StringUtility.ToEmpty(Eval(Constants.FIELD_ORDEROWNER_OWNER_NAME))) %></td>
																<td align="right" width="70"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_ORDERITEM_ITEM_PRICE).ToPriceString(true))%></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<%--△ 購入者詳細 △--%>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="7"></td>
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