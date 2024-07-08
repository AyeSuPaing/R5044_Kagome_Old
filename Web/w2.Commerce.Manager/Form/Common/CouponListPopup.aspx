<%--
=========================================================================================================
  Module      : クーポンリストポップアップページ(CouponListPopup.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="CouponListPopup.aspx.cs" Inherits="Form_Common_CouponListPopup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
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
														<td class="search_title_bg" width="120">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															クーポンコード
														</td>
														<td class="search_item_bg" width="110">
															<asp:TextBox id="tbCouponCode" runat="server" Width="100" />
														</td>
														<td class="search_title_bg" width="120">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															クーポン名(管理用)
														</td>
														<td class="search_item_bg" width="110">
															<asp:TextBox id="tbCouponName" runat="server" Width="100" />
														</td>
														<td class="search_title_bg" width="120">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															並び順
														</td>
														<td class="search_item_bg" width="110">
															<asp:DropDownList id="ddlSortKbn" runat="server">
																<asp:ListItem Value="0">作成日/昇順</asp:ListItem>
																<asp:ListItem Value="1">作成日/降順</asp:ListItem>
																<asp:ListItem Value="2">更新日/昇順</asp:ListItem>
																<asp:ListItem Value="3">更新日/降順</asp:ListItem>
															</asp:DropDownList>
														</td>
														<td class="search_btn_bg" width="83" rowspan="5">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" />
															</div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_COUPON_LIST_POPUP %>">クリア</a>&nbsp;
															</div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															発行ステータス
														</td>
														<td class="search_item_bg" colspan="5">
															<asp:RadioButtonList ID="rblPublishDate" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
																<asp:ListItem Value="">指定なし</asp:ListItem>
															</asp:RadioButtonList>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															有効フラグ
														</td>
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
										<% if (this.IsNotSearchDefault) { %>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="690" border="0">
													<tr class="list_alert">
														<td><%= WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NOT_SEARCH_DEFAULT) %></td>
													</tr>
												</table>
											</td>
										</tr>
										<% } else { %>
										<tr>
											<td align="left">
												<!--▽ ページング ▽-->
												<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
													<tr>
														<td width="675"><asp:Label ID="lbPager" Runat="server"></asp:Label></td>
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
														<td align="center" width="105" rowspan="2">クーポンコード</td>
														<td align="center" width="340">クーポン名(管理用)</td>
														<td align="center" width="85" rowspan="2">割引金額/率</td>
														<td align="center" width="150">発行ステータス</td>
														<td align="center" width="35" rowspan="2">有効<br />フラグ</td>
													</tr>
													<tr class="list_title_bg">
														<td align="center">発行パターン</td>
														<td align="center">有効期限/期間</td>
													</tr>
													<asp:Repeater id="rCouponList" ItemType="w2.Domain.Coupon.Helper.CouponListSearchResult" runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="set_coupon_code('<%# Item.CouponCode %>')">
																<td align="center"><%#: Item.CouponCode %></td>
																<td align="center" style="padding:0px">
																	<table cellpadding="0" cellspacing="0" width="100%" height="100%">
																		<tr>
																			<td align="left" class="item_bottom_line"><%#: Item.CouponName %></td>
																		</tr>
																		<tr>
																			<td align="left"><%#: ValueText.GetValueText(Constants.TABLE_COUPON, Constants.FIELD_COUPON_COUPON_TYPE, (string)Item.CouponType) %></td>
																		</tr>
																	</table>
																</td>
																<td align="center"><%# DisplayDiscount(Item) %></td>
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
																<td align="center"><%#: ValueText.GetValueText(Constants.TABLE_COUPON, Constants.FIELD_COUPON_VALID_FLG, (string)Item.ValidFlg) %></td>
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
<script>
	// クーポンコードをセット
	function set_coupon_code(couponCode) {
		// 親ウィンドウが存在する場合
		if (window.opener != null) {
			window.opener.set_coupon_code(couponCode);
			window.close();
		}
	}
</script>
</asp:Content>


