<%--
=========================================================================================================
  Module      : 注文関連ファイル取込出力コントローラ(BodyOrderFileImport.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BodyOrderFileImport.ascx.cs" Inherits="Form_Common_OrderFileImport" %>
<%@ Import Namespace="w2.App.Common.Order" %>
<%@ Import Namespace="w2.App.Common.LohacoCreatorWebApi.OrderInfo" %>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ 注文関連ファイル取込フォーム ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">注文関連ファイル取込</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td>
						<table class="info_box_bg" cellspacing="0" cellpadding="0" border="0" width="100%">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="list_title_bg" width="120">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />ファイル種別</td>
														<td class="list_item_bg1">
															<asp:DropDownList id="ddlOrderFile" Runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlOrderFile_SelectedIndexChanged"></asp:DropDownList>
															<%if (CanShipmentEntry()) {%>
															<asp:CheckBox id="cbExecExternalShipmentEntry" Text="出荷情報登録連携" Checked="true" runat="server"/>
															<%} %>
														</td>
													</tr>
													<tr>
														<td class="list_title_bg" width="120">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />ファイルパス<span class="notice">*</span></td>
														<td class="list_item_bg1">
															<input id="fFile" contenteditable="false" style="WIDTH: 500px; height:22px" size="90" type="file" name="fFile" runat="server" />
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
												<table class="info_table" border="0" cellspacing="1">
													<tr>
														<td class="info_item_bg">
															<div class="list_detail">
																<asp:Label ID="lbOrderFileInfo" runat="server"></asp:Label>
															</div>
														</td>
													</tr>
												</table>
												<h4 id="orderStatusTitle" visible="false" runat="server">各ステータスの対応表</h4>
												<asp:Repeater ID="rOrderStatusDescription" runat="server" ItemType="Form_Common_OrderFileImport.DisplayOrderStatus">
													<ItemTemplate>
														<span><%#: Item.StatusName %></span>
														<table  class="order-status-disp-names info_table">
															<tr class="order-status-disp-name">
																<asp:Repeater runat="server" DataSource="<%# Item.StatusDispNames %>" ItemType="System.Web.UI.WebControls.ListItem">
																	<ItemTemplate>
																		<td><%# (string)Item.Text %></td>
																	</ItemTemplate>
																</asp:Repeater>
															</tr>
															<tr>
																<asp:Repeater runat="server" DataSource="<%# Item.StatusDispNames %>" ItemType="System.Web.UI.WebControls.ListItem">
																	<ItemTemplate>
																		<td> <%#: (string)Item.Value%></td>
																	</ItemTemplate>
																</asp:Repeater>
															</tr>
														</table>
													</ItemTemplate>
												</asp:Repeater>
											</td>
										</tr>
										<tr>
											<td>
												<table cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td><div class="action_part_bottom"><asp:Button id="btnImport" Runat="server" Text=" ファイル取込 " OnClick="btnImport_Click" /></div></td>
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
			</table>
		</td>
	</tr>
		<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr id="trResultTitle" runat="server">
		<td><h2 class="cmn-hed-h2">ファイル取込結果</h2></td>
	</tr>
	<tr id="trResultDetail" runat="server">
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" border="0" width="100%">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td>
														<% if (lbResultMessage.Text != "" ){ %>
															<table class="info_table" width="758" border="0" cellspacing="1">
																<tr>
																	<td class="info_item_bg">
																		<div class="list_detail">
																			<asp:Label ID="lbResultMessage" runat="server"></asp:Label>
																			<asp:Label ID="lbMessageExcludeMessage" runat="server"></asp:Label>
																		</div>
																	</td>
																</tr>
															</table>
														<%} %>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td>
															<div id="divError" runat="server">
																<asp:Repeater ID="rErrorList" runat="server">
																	<HeaderTemplate>
																		<table class="list_table" width="758" border="0" cellspacing="1">
																	</HeaderTemplate>
																	<ItemTemplate>
																		<div runat ="server" visible="<%# (Container.ItemIndex == 0) %>">
																			<tr class="list_title_bg">
																				<td colspan="<%# ((Dictionary<string, string>)Container.DataItem).Count - 1 %>" align="center">
																					取込データ
																				</td>
																				<td rowspan="2" align="center" valign="middle">
																					エラー内容
																				</td>
																			</tr>
																			<tr class="list_title_bg" align="center">
																				<asp:Repeater runat="server" DataSource ="<%# (Dictionary<string, string>)Container.DataItem %>">
																					<ItemTemplate>
																						<td runat="server" nowrap="nowrap" visible="<%# (Container.ItemIndex < ((Dictionary<string, string>)((Repeater)Container.Parent).DataSource).Count - 1) %>">
																							<%# ((KeyValuePair<string, string>)Container.DataItem).Key %>
																						</td>
																					</ItemTemplate>
																				</asp:Repeater>
																			</tr>
																		</div>
																		<tr class="list_item_bg1">
																			<asp:Repeater runat="server" DataSource ="<%# (Dictionary<string, string>)Container.DataItem %>">
																				<ItemTemplate>
																					<!--▽ 行 ▽-->
																					<td width ="15" align="center" runat="server" nowrap="nowrap" visible="<%# (Container.ItemIndex == 0) %>">
																						<%# ((KeyValuePair<string, string>)Container.DataItem).Value %>
																					</td>
																					<!--△ 行 △-->
																					<!--▽ 抽出フィールド ▽-->
																					<td align="center" runat="server" nowrap="nowrap" visible="<%# (Container.ItemIndex != 0 && Container.ItemIndex != ((Dictionary<string, string>)((Repeater)Container.Parent).DataSource).Count - 1 && Container.ItemIndex != ((Dictionary<string, string>)((Repeater)Container.Parent).DataSource).Count - 2) %>">
																						<%# ((KeyValuePair<string, string>)Container.DataItem).Value %>
																					</td>
																					<!--△ 抽出フィールド △-->
																					<!--▽ 入金日 ▽-->
																					<td width ="70" align="center" runat="server" nowrap="nowrap" visible="<%# (Container.ItemIndex == ((Dictionary<string, string>)((Repeater)Container.Parent).DataSource).Count - 2) %>">
																						<%# ((KeyValuePair<string, string>)Container.DataItem).Value %>
																					</td>
																					<!--△ 入金日 △-->
																					<!--▽ エラー内容 ▽-->
																					<td align="left" runat="server" visible="<%# (Container.ItemIndex == ((Dictionary<string, string>)((Repeater)Container.Parent).DataSource).Count - 1) %>">
																						<%# ((KeyValuePair<string, string>)Container.DataItem).Value %>
																					</td>
																					<!--△ エラー内容 △-->
																				</ItemTemplate>
																			</asp:Repeater>
																		</tr>
																	</ItemTemplate>
																	<FooterTemplate>
																		</table>
																	</FooterTemplate>
																</asp:Repeater>
															</div>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td>
												<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
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
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ 注文関連ファイル取込フォーム △-->
</table>
