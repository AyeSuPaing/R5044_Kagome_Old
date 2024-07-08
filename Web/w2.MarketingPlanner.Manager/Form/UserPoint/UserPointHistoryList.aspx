<%--
=========================================================================================================
  Module      : ユーザポイント履歴一覧ページ(UserPointHistoryList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="UserPointHistoryList.aspx.cs" Inherits="Form_UserPoint_UserPointHistoryList" %>
<%@ Import Namespace="w2.Domain.Point.Helper" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">ユーザーポイント情報</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td>
						<table class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td>
												<div class="action_part_top">
												<% if (Request[Constants.REQUEST_KEY_WINDOW_KBN] != Constants.KBN_WINDOW_POPUP){ %>
												<input type="button" onclick="Javascript:location.href='<%= CreateUserPointListUrl() %>'" value="  一覧へ戻る  " />
												<% } %>
												</div>
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="3">基本情報</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">ユーザーID</td>
														<td class="detail_item_bg" align="left" colspan="2"><%#: this.Data.UserId %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%"><%: ReplaceTag("@@User.name.name@@") %></td>
														<td class="detail_item_bg" align="left" colspan="2"><%#: this.Data.UserName %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">保有ポイント合計</td>
														<td class="detail_item_bg" align="left" colspan="2"><%#: StringUtility.ToNumeric(this.Data.PointTotal) %>pt</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">仮ポイント合計</td>
														<td class="detail_item_bg" align="left" colspan="2"><%#: StringUtility.ToNumeric(this.Data.TempPointTotal) %>pt</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">注文利用可能ポイント合計</td>
														<td class="detail_item_bg" align="left" colspan="2"><%#: StringUtility.ToNumeric(this.Data.PointUsableForOrder) %>pt</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">注文利用可能ポイント有効期限（利用可能期間）</td>
														<td class="detail_item_bg" align="left" colspan="2"><%#: DateTimeUtility.ToStringForManager(this.Data.PointExp, DateTimeUtility.FormatType.ShortDate2Letter, string.Empty) %></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr><td><br /></td></tr>
										<tr id="trUserPointDetail" runat="server">
											<td>
												<table class="detail_table">
													<tr>
														<td class="detail_title_bg" align="center" colspan="3">ポイント明細</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="center">ポイント区分・種別</td>
														<td class="detail_title_bg" align="center">ポイント数</td>
														<td class="detail_title_bg" align="center">有効期限（利用可能期間）</td>
													</tr>
													<asp:Repeater ID="rUserPointList" ItemType="w2.Domain.Point.UserPointModel" runat="server">
														<ItemTemplate>
															<tr>
																<td class="detail_item_bg" width="30%">
																	<%#: ValueText.GetValueText(Constants.TABLE_USERPOINT, Constants.FIELD_USERPOINT_POINT_TYPE, Item.PointType) %>
																	<%#: Item.IsLimitedTermPoint ? "[期間限定]" : string.Empty %>
																	<%# WebSanitizer.HtmlEncodeChangeToBr(Item.IsPointTypeTemp ? string.Format("\r\n(注文ID：{0})", Item.OrderId) : string.Empty) %>
																</td>
																<td class="detail_item_bg">
																	<%#: StringUtility.ToNumeric(Item.Point) %>pt
																</td>
																<td runat="server" class="detail_item_bg" Visible="<%# ((Item.IsPointTypeTemp && Item.IsLimitedTermPoint) == false) %>">
																	<%#: Item.IsBasePoint
																		? DateTimeUtility.ToStringForManager(Item.PointExp, DateTimeUtility.FormatType.ShortDate2Letter, "期限設定無し")
																		: string.Format("{0} ～ {1}",
																			DateTimeUtility.ToStringForManager(Item.EffectiveDate, DateTimeUtility.FormatType.ShortDate2Letter),
																			DateTimeUtility.ToStringForManager(Item.PointExp, DateTimeUtility.FormatType.ShortDate2Letter)) %>
																</td>
																<td runat="server" class="detail_item_bg" Visible="<%# (Item.IsPointTypeTemp && Item.IsLimitedTermPoint) %>">
																	本ポイントへの移行時に計算されます。
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
												</table>
											</td>
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
	<!--△ 詳細 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">ユーザーポイント情報履歴一覧</h2></td>
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
											<td>
												<!--▽ ページング ▽-->
												<table class="list_pager" cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td><asp:label id="lbPager1" Runat="server"></asp:label></td>
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
														<td class="ta-center" width="38">No</td>
														<td class="ta-center" width="38">履歴作成日</td>
														<td class="ta-center" width="130">ポイント加算区分</td>
														<td class="ta-center" width="130">ポイント種別</td>
														<td class="ta-center" width="130">ポイント数</td>
														<td class="ta-center" width="130">備考</td>
													</tr>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="8"></td>
													</tr>
													<asp:Repeater
														ID="rList"
														runat="server"
														ItemType="Form_UserPoint_UserPointHistoryList.PointHistoryForDisplay">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>"
																onmouseover="listselect_mover(this)"
																onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)"
																onmousedown="listselect_mdown(this)"
																onclick="javascript:open_window('<%# CreatePointHistoryDetailUrl(Item.UserId, Item.HistoryGroupNo) %>', 'userpointhistorydetail','width=1000,height=650,top=0,left=0,status=NO,scrollbars=yes')">
																<td class="ta-center">
																	<%#: Item.RowNumber %>
																</td>
																<td class="ta-center">
																	<%#: Item.DateCreated %>
																</td>
																<td class="ta-center">
																	<%# Item.PointIncKbn %>
																</td>
																<td class="ta-center">
																	<%#: Item.PointType %>
																</td>
																<td class="ta-center point_num <%#: Item.PointTotal.Contains("-") ? "negative_point" : "" %>">
																	<%#: Item.PointTotal %>
																</td>
																<td class="ta-center" style="padding: 0">
																	<table width="100%" height="100%">
																		<tr>
																			<td width="50%" class="ta-center item_bottom_line item_right_line">注文ID</td>
																			<td width="50%" class="ta-center item_bottom_line">定期購入ID</td>
																		</tr>
																		<tr>
																			<td class="ta-center item_bottom_line item_right_line">　<%#: Item.OrderId %>　</td>
																			<td class="ta-center item_bottom_line">　<%#: Item.FixedPurchaseId %>　</td>
																		</tr>
																	</table>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
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
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>