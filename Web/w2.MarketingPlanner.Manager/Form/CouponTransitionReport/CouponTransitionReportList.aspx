<%--
=========================================================================================================
  Module      : クーポン推移レポート一覧ページ(CouponTransitionReportList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="CouponTransitionReportList.aspx.cs" Inherits="Form_CouponTransitionReport_CouponTransitionReportList" %>
<%@ Import Namespace="w2.Domain.Coupon.Helper" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">クーポン推移レポート</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<%--▽ 検索 ▽--%>
	<tr>
		<td><h2 class="cmn-hed-h2">レポート対象選択</h2></td>
	</tr>
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
											<td class="search_box">
												<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
													<tr class="search_title_bg">
														<td width="250" colspan="3"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />対象年月</td>
													</tr>
													<tr class="search_item_bg">
														<td>
															<%--▽ カレンダー ▽--%>
															<asp:label id="lblCurrentCalendar" Runat="server"></asp:label>
															<%--△ カレンダー △--%>
														</td>
														<td width="130">
															<asp:RadioButtonList id="rblReportType" Runat="server" AutoPostBack="True" CssClass="radio_button_list">
																<asp:ListItem Value="0" Selected="True">日別レポート</asp:ListItem>
																<asp:ListItem Value="1">月別レポート</asp:ListItem>
															</asp:RadioButtonList>
														</td>
														
														<td width="500">
															<table  class="search_table" cellspacing="0" cellpadding="0" border="0" width="100%" >
															<tr class="search_item_bg">
																<td colspan="2">
																	<asp:RadioButton ID="rbAllCoupon" runat="server" GroupName="Coupon" Text="全てのクーポン" AutoPostBack="True" /><br />
																</td>
															</tr>
															<tr class="search_item_bg">
																<td width="150" valign="top">
																	<asp:RadioButton ID="rbTargetCoupon" runat="server" GroupName="Coupon" Text="特定のクーポン" AutoPostBack="True" />
																</td>
																<td>
																	<asp:TextBox ID="tbCouponId" Width="140px" runat="server"></asp:TextBox>
																	<asp:Button ID="btnSearchCoupon" Text="  クーポンコード検索  " style="width:108px;" runat="server" /><br />
																	<asp:DropDownList ID="ddlCoupon" runat="server" Width="260" AutoPostBack="True"></asp:DropDownList>
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
	<%--△ 検索 △--%>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<%--▽ 一覧 ▽--%>
	<tr>
		<td><h2 class="cmn-hed-h2">レポート表示</h2></td>
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
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="info_table" width="700" border="0" cellspacing="1" cellpadding="3">
													<tr class="info_item_bg">
														<td align="left">
															<div id="divDaily" Visible="False" runat="server">　<%: this.CurrentMonth %>分　日別レポート　全てのクーポン</div>
															<div id="divMonthly" Visible="False" runat="server">　<%: this.CurrentYear %>年分　月別レポート　全てのクーポン</div>
															<div id="divNoHitCoupon" Visible="False" runat="server">　特定のクーポン ： 該当なし</div>
															<div id="divOneHitCoupon" Visible="False" runat="server">　特定のクーポン ： [<%: this.HitCouponCode %>]<%: this.HitCouponName %></div>
															<div id="divMoreThanOneHitCoupon" Visible="False" runat="server">　特定のクーポン ： <%: this.HitCouponCount %> クーポンHit</div>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<%--▽ レポート ▽--%>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" border="0" width="700">
													<tr class="list_title_bg">
														<td class="list_title_bg" style="white-space:nowrap" width="40" rowspan="2"></td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="155" colspan="2">発行クーポン</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="155" colspan="2">利用クーポン</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="155" colspan="2">消滅クーポン（有効期限切れ）</td>
														<td class="list_item_bg3" style="white-space:nowrap" align="center" width="155" colspan="2">利用可能クーポン</td>
													</tr>
													<tr>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="95">金額</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="60">枚数</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="95">金額</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="60">枚数</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="95">金額</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="60">枚数</td>
														<td class="list_item_bg3" style="white-space:nowrap" align="center" width="95">金額</td>
														<td class="list_item_bg3" style="white-space:nowrap" align="center" width="60">枚数</td>
													</tr>
													<asp:Repeater id=rDataList Runat="server">
														<ItemTemplate>
															<tr>
																<td class="list_title_bg" align="center"><%# ((CouponTransitionReportResult)Container.DataItem).TimeUnit%></td>
																<td class="list_item_bg1" align="right" width="90"><%# WebSanitizer.HtmlEncode(((CouponTransitionReportResult)Container.DataItem).AddCoupon) %></td>
																<td class="list_item_bg1" align="right" width="90"><%# WebSanitizer.HtmlEncode(((CouponTransitionReportResult)Container.DataItem).AddCount) %></td>
																<td class="list_item_bg1" align="right" width="90"><%# WebSanitizer.HtmlEncode(((CouponTransitionReportResult)Container.DataItem).UseCoupon) %></td>
																<td class="list_item_bg1" align="right" width="90"><%# WebSanitizer.HtmlEncode(((CouponTransitionReportResult)Container.DataItem).UseCount) %></td>
																<td class="list_item_bg1" align="right" width="90"><%# WebSanitizer.HtmlEncode(((CouponTransitionReportResult)Container.DataItem).ExpCoupon) %></td>
																<td class="list_item_bg1" align="right" width="90"><%# WebSanitizer.HtmlEncode(((CouponTransitionReportResult)Container.DataItem).ExpCount) %></td>
																<td class="list_item_bg1" align="right" width="90"><%# WebSanitizer.HtmlEncode(((CouponTransitionReportResult)Container.DataItem).UnusedCoupon) %></td>
																<td class="list_item_bg1" align="right" width="90"><%# WebSanitizer.HtmlEncode(((CouponTransitionReportResult)Container.DataItem).UnusedCount) %></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" colspan="9" runat="server">
														</td>
													</tr>
												</table>
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
												</table>
												<table class="info_table" cellspacing="1" cellpadding="3" width="700" border="0">
													<tr>
														<td align="left" class="info_item_bg" colspan="2">備考<br/>
														■枚数について<br />
														管理者発行のクーポンについては、利用のみ枚数をカウントしています。<br />
														※管理者発行のクーポン → 無制限利用可能クーポン、回数制限付きクーポン、ブラックリスト型クーポン<br />
														■金額について<br />
														クーポン割引設定が％設定の場合には、利用しない限り、金額が確定しないため、利用以外の金額には含まれません。<br />
														■消滅クーポンについて<br />
														有効期限切れクーポン・退会ユーザのクーポンを含みます。
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<%--△ レポート △--%>
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
	<%--△ 一覧 △--%>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
