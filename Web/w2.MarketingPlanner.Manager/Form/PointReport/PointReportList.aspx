<%--
=========================================================================================================
  Module      : ポイント最新レポート一覧ページ(PointReportList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="PointReportList.aspx.cs" Inherits="Form_PointReport_PointReportList" MaintainScrollPositionOnPostback="true"%>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">ポイント最新レポート</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
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
														<td align="left">データ作成日時：<%: DateTimeUtility.ToStringForManager(DateTime.Now, DateTimeUtility.FormatType.LongDateHourMinute2Letter) %></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="12" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="search_table" cellspacing="1" cellpadding="3" width="700" border="0">
													<tr>
														<td class="search_title_bg" width="150">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />ポイント区分</td>
														<td class="search_item_bg">
															<asp:RadioButtonList id="rblPointKbn" RepeatLayout="Flow" RepeatDirection="Horizontal" CellSpacing="50" CellPadding="0" AutoPostBack="True" OnSelectedIndexChanged="rblPointKbn_OnSelectedIndexChanged" Width="550" runat="server"/>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="12" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="700" border="0">
													<tr class="list_title_bg">
														<td align="center" width="25%">発行ポイント</td>
														<td align="center" width="25%">回収ポイント</td>
														<td align="center" width="25%">消滅ポイント（有効期限切れ）</td>
														<td align="center" width="25%">未回収ポイント</td>
													</tr>
													<tr class="list_item_bg1">
														<td align="center" width="25%">(+)<%: StringUtility.ToNumeric((decimal)this.AmountPoint["add_point"]) %>pt</td>
														<td align="center" width="25%">(-)<%: StringUtility.ToNumeric((decimal)this.AmountPoint["use_point"] * -1) %>pt</td>
														<td align="center" width="25%">(-)<%: StringUtility.ToNumeric((decimal)this.AmountPoint["exp_point"] * -1)  %>pt</td>
														<td align="center" width="25%">(=)<%: StringUtility.ToNumeric((decimal)this.AmountPoint["user_point"]) %>pt</td>
													</tr>
												</table>
											</td>
										</tr>
										<!--▽ 分析結果 ▽-->
										<asp:Repeater ID="rResults" Runat="server">
											<ItemTemplate>
												<tr>
													<td><img height="12" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
												</tr>
												<tr>
													<td>
														<table class="list_table" cellspacing="1" cellpadding="3" width="700" border="0">
															<tr class="list_title_bg">
																<td align="center" width="30%" colspan="4"><%# WebSanitizer.HtmlEncode(((KbnAnalysisUtility.KbnAnalysisTable)Container.DataItem).Title) %></td>
															</tr>
															<asp:Repeater id="Result2" Runat="server" DataSource='<%# ((KbnAnalysisUtility.KbnAnalysisTable)Container.DataItem).Rows %>'>
																<ItemTemplate>
																	<tr>
																		<td class="list_item_bg2" align="center" width="20%">
																			<%# WebSanitizer.HtmlEncode(((KbnAnalysisUtility.KbnAnalysisRow)Container.DataItem).Name)%></td>
																		<td class="list_item_bg1" align="left" width="60%">
																			<img height="12" alt='<%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(((KbnAnalysisUtility.KbnAnalysisRow)Container.DataItem).Count)) %><%# ((KbnAnalysisUtility.KbnAnalysisTable)((RepeaterItem)(Container.Parent.Parent)).DataItem).Unit %>' src="../../Images/graph_bar01.gif" width='<%# KbnAnalysisUtility.GetRateImgWidth(((KbnAnalysisUtility.KbnAnalysisRow)Container.DataItem).Count ,((KbnAnalysisUtility.KbnAnalysisTable)((RepeaterItem)(Container.Parent.Parent)).DataItem).Total) %>%' border="0" /></td>
																		<td class="list_item_bg1" align="right" width="10%">
																			<%# WebSanitizer.HtmlEncode(KbnAnalysisUtility.GetRateString(((KbnAnalysisUtility.KbnAnalysisRow)Container.DataItem).Count ,((KbnAnalysisUtility.KbnAnalysisTable)((RepeaterItem)(Container.Parent.Parent)).DataItem).Total, 1)) %>%</td>
																		<td class="list_item_bg1" align="right" width="10%">
																			<%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(((KbnAnalysisUtility.KbnAnalysisRow)Container.DataItem).Count)) %><%# ((KbnAnalysisUtility.KbnAnalysisTable)((RepeaterItem)(Container.Parent.Parent)).DataItem).Unit %></td>
																	</tr>
																</ItemTemplate>
															</asp:Repeater>
														</table>
													</td>
												</tr>
											</ItemTemplate>
										</asp:Repeater>
										<!--△ 分析結果 △-->
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