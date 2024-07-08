<%--
=========================================================================================================
  Module      : ポイント推移レポート一覧ページ(PointTransitionReportList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="PointTransitionReportList.aspx.cs" Inherits="Form_PointTransitionReport_PointTransitionReportList" %>
<%@ Import Namespace="w2.App.Common.Extensions" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">ポイント推移レポート</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr> <!--▽ 検索 ▽-->
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
														<td width="30%" colspan="2"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />対象年月</td>
													</tr>
													<tr class="search_item_bg">
														<td>
															<!-- ▽カレンダ▽ --><asp:label id="lblCurrentCalendar" Runat="server"></asp:label>
															<!-- △カレンダ△ -->
														</td>
														<td width="600">
															<asp:RadioButtonList id="rblReportType" Runat="server" AutoPostBack="True" CssClass="radio_button_list">
																<asp:ListItem Value="0" Selected="True">日別レポート</asp:ListItem>
																<asp:ListItem Value="1">月別レポート</asp:ListItem>
															</asp:RadioButtonList>
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
	</tr> <!--△ 検索 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr> <!--▽ 一覧 ▽-->
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
															<div id="divDaily" Visible="False" runat="server">　<%: this.CurrentMonth %>分　日別レポート</div>
															<div id="divMonthly" Visible="False" runat="server">　<%: this.CurrentYear %>年分　月別レポート</div>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<!--▽ レポート ▽-->
										<tr>
											<td>
												<div style="WIDTH: 100%">
												<table class="list_table" cellspacing="1" cellpadding="3" border="0" width="790">
													<tr class="list_title_bg">
														<td class="list_title_bg" style="white-space:nowrap" width="40" rowspan="2"></td>
														<td id="tdAddPointTitle" class="list_item_bg2" style="white-space:nowrap" align="center" width="180" colspan="2">発行ポイント</td>
														<td id="tdUsePointTitle" class="list_item_bg2" style="white-space:nowrap" align="center" width="180" colspan="2">回収ポイント</td>
														<td id="tdExpPointTitle" class="list_item_bg2" style="white-space:nowrap" align="center" width="180" colspan="2">消滅ポイント（有効期限切れ）</td>
														<td id="tdUserPointTitle" class="list_item_bg2" style="white-space:nowrap" align="center" width="90">小計ポイント</td>
														<td id="tdPointUnusedTitle" class="list_item_bg3" style="white-space:nowrap" align="center" width="90">未回収ポイント</td>
													</tr>
													<tr>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="90">ポイント</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="90">人</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="90">ポイント</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="90">人</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="90">ポイント</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="90">人</td>
														<td class="list_item_bg2" style="white-space:nowrap" align="center" width="90">ポイント</td>
														<td class="list_item_bg3" style="white-space:nowrap" align="center" width="90">ポイント</td>
													</tr>
													<asp:Repeater id="rDataList" Runat="server">
														<ItemTemplate>
															<tr>
																<td class="list_title_bg" align="center"><%# Container.ToModel<WrappedReportResult>().ViewDate%></td>
																<td class="list_item_bg1" align="right" width="90"><%# Container.ToModel<WrappedReportResult>().ViewAddPoint%></td>
																<td class="list_item_bg1" align="right" width="90"><%# Container.ToModel<WrappedReportResult>().ViewAddPointCount%></td>
																<td class="list_item_bg1" align="right" width="90"><%# Container.ToModel<WrappedReportResult>().ViewUsePoint%></td>
																<td class="list_item_bg1" align="right" width="90"><%# Container.ToModel<WrappedReportResult>().ViewUsePointCount%></td>
																<td class="list_item_bg1" align="right" width="90"><%# Container.ToModel<WrappedReportResult>().ViewExpPoint%></td>
																<td class="list_item_bg1" align="right" width="90"><%# Container.ToModel<WrappedReportResult>().ViewExpPointCount%></td>
																<td class="list_item_bg1" align="right" width="90"><%# Container.ToModel<WrappedReportResult>().ViewSubtotalPoint%></td>
																<td class="list_item_bg1" align="right" width="90"><%# Container.ToModel<WrappedReportResult>().ViewUnusedPoint%></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
												</table>
												</div>
											</td>
										</tr>
										<!--△ レポート △-->
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
									</table>
									<table class="info_table" cellspacing="1" cellpadding="3" width="700" border="0">
										<tr class="info_item_bg">
											<td align="left">備考<br />
												■発行ポイント<br />
												基本ルール設定、キャンペーン設定にもとづいてユーザーに付与されたポイントの合計です。<br />
												■回収ポイント<br />
												ユーザーによって使用されたポイントの合計です。（注文のキャンセルに伴うポイントキャンセル等を含みます）<br />
												■消滅ポイント<br />
												有効期限切れやユーザの退会によって使用不可となったポイントの合計です。<br />
												■未回収ポイント<br />
												ユーザーに付与したポイントの内、現時点で有効であり未使用のポイントの合計です。
											</td>
										</tr>
									</table>
									<br />
								</td>
							</tr>
						</table>						
					</td>
				</tr>
			</table>			
		</td>
	</tr> <!--△ 一覧 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
