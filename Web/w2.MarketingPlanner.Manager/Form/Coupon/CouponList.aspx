<%--
=========================================================================================================
  Module      : クーポン設定一覧ページ(CouponList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="CouponList.aspx.cs" Inherits="Form_Coupon_CouponList" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="w2.App.Common.Option" %>
<%@ Import Namespace="w2.Domain.Coupon.Helper" %>
<%-- マスタ出力コントロール --%>
<%@ Register TagPrefix="uc" TagName="MasterDownLoad" Src="~/Form/Common/MasterDownLoadPanel.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">クーポン設定</h1></td>
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
														<td class="search_title_bg" width="120"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />クーポンコード</td>
														<td class="search_item_bg" width="110"><asp:TextBox id="tbCouponCode" runat="server" Width="100"></asp:TextBox></td>
														<td class="search_title_bg" width="120">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />クーポン名(管理用)</td>
														<td class="search_item_bg" width="110">
															<asp:TextBox id="tbCouponName" runat="server" Width="100"></asp:TextBox></td>
														<td class="search_title_bg" width="120"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />並び順</td>
														<td class="search_item_bg" width="110"><asp:DropDownList id="ddlSortKbn" runat="server">
																<asp:ListItem Value="0">作成日/昇順</asp:ListItem>
																<asp:ListItem Value="1">作成日/降順</asp:ListItem>
																<asp:ListItem Value="2">更新日/昇順</asp:ListItem>
																<asp:ListItem Value="3">更新日/降順</asp:ListItem>
															</asp:DropDownList></td>
														<td class="search_btn_bg" width="83" rowspan="5">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_COUPONT_LIST %>">クリア</a>&nbsp;
																<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a></div>
															<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
															<div class="search_btn_sub">
																<asp:LinkButton id="lbExportTranslationData" Runat="server" OnClick="lbExportTranslationData_OnClick">翻訳設定出力</asp:LinkButton></div>
															<% }  %>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />発行パターン</td>
														<td class="search_item_bg" colspan="5"><asp:DropDownList id="ddlCouponType" runat="server"></asp:DropDownList></td>
													</tr>
													<tr>
														<td class="search_title_bg">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />発行ステータス</td>
														<td class="search_item_bg" colspan="5">
															<asp:RadioButtonList ID="rblPublishDate" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
																<asp:ListItem Value="">指定なし</asp:ListItem>
															</asp:RadioButtonList>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />有効フラグ</td>
														<td class="search_item_bg" colspan="5">
															<asp:RadioButtonList ID="rblValidFlg" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
																<asp:ListItem Value="">指定なし</asp:ListItem>
															</asp:RadioButtonList>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr><td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
										<%-- マスタ出力 --%>
										<tr>
											<td class="search_table">
												<uc:MasterDownLoad runat="server" ID="uMasterDownload" DownloadType="Coupon" TableWidth="758" />
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
		<td><h2 class="cmn-hed-h2">クーポン設定一覧</h2></td>
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
											<td align="left">
												<!--▽ ページング ▽-->
												<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
													<tr>
														<td width="675"><asp:Label ID="lbPager" Runat="server"></asp:Label></td>
														<td width="83" class="action_list_sp"><asp:Button id="btnInsertTop" Runat="server" Text="  新規登録  " onclick="btnInsertTop_Click"></asp:Button></td>
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
														<td><%= WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NOT_SEARCH_DEFAULT) %></td>
													</tr>
												</table>
											</td>
										</tr>
										<% } else { %>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="list_title_bg">
														<td align="center" width="105" rowspan="2">クーポンコード</td>
														<td align="center" width="340">クーポン名(管理用)</td>
														<td align="center" width="85" rowspan="2">割引金額/率</td>
														<td align="center" width="150">発行ステータス</td>
														<td align="center" width="55" rowspan="2">利用可能<br />回数</td>
														<td align="center" width="35" rowspan="2">有効<br />フラグ</td>
													</tr>
													<tr class="list_title_bg">
														<td align="center">発行パターン</td>
														<td align="center">有効期限/期間</td>
													</tr>
													<asp:Repeater id="rList" Runat="server" ItemType="CouponListSearchResult">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateCouponDetailUrl(Item.CouponId)) %>')">
																<td align="center"><%#: Item.CouponCode %></td>
																<td align="center" style="padding:0px">
																	<table cellpadding="0" cellspacing="0" width="100%" height="100%">
																		<tr>
																			<td align="left" class="item_bottom_line"><%#: Item.CouponName %></td>
																		</tr>
																		<tr>
																			<td align="left"><%#: ValueText.GetValueText(Constants.TABLE_COUPON, Constants.FIELD_COUPON_COUPON_TYPE, Item.CouponType) %></td>
																		</tr>
																	</table>
																</td>
																<td align="center"><%# WebSanitizer.HtmlEncodeChangeToBr(DisplayDiscount(Item)) %></td>
																<td align="center" style="padding:0px">
																	<table cellpadding="0" cellspacing="0" width="100%" height="100%">
																		<tr>
																			<td align="center" class="item_bottom_line"><%#: DisplayPublishDate(Item) %></td>
																		</tr>
																		<tr>
																			<td align="center"><%#: DisplayExpire(Item) %></td>
																		</tr>
																	</table>
																</td>
																<td align="center" visible="<%# IsBlacklistCoupon(Item.CouponType) %>" runat="server">
																	<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateCouponUseUserListUrl(Item.CouponId)) %>">利用状況</a>
																</td>
																<td align="center" visible="<%# (IsBlacklistCoupon(Item.CouponType) == false) %>" runat="server">
																	<%#: DisplayCouponCount(Item) %>
																</td>
																<td align="center"><%#: ValueText.GetValueText(Constants.TABLE_COUPON, Constants.FIELD_COUPON_VALID_FLG, Item.ValidFlg) %></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" colspan="6" runat="server">
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<% } %>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td class="action_list_sp"><asp:Button id="btnInsertBottom" Runat="server" Text="  新規登録  " onclick="btnInsertTop_Click"></asp:Button></td>
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

