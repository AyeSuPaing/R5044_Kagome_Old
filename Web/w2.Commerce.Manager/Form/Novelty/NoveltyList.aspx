<%--
=========================================================================================================
  Module      :ノベルティ設定一覧ページ(NoveltyList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.Domain.Novelty" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="NoveltyList.aspx.cs" Inherits="Form_Novelty_NoveltyList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">ノベルティ設定</h1></td>
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
												<table cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="search_title_bg" width="125">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															ノベルティID
														</td>
														<td class="search_item_bg" width="110">
															<asp:TextBox id="tbNoveltyId" runat="server" Width="100" MaxLength="30"></asp:TextBox>
														</td>
														<td class="search_title_bg" width="125">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															ノベルティ名(表示用)
														</td>
														<td class="search_item_bg" width="110">
															<asp:TextBox id="tbNoveltyDisplayName" runat="server" Width="100" MaxLength="100"></asp:TextBox>
														</td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															並び順
														</td>
														<td class="search_item_bg" width="110">
															<asp:DropDownList ID="ddlSortKbn" runat="server" Width="100"></asp:DropDownList>
														</td>
														<td class="search_btn_bg" width="83" rowspan="2">
															<div class="search_btn_main"><asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_NOVELTY_LIST %>">クリア</a>
																<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a><br />
																<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
																<asp:LinkButton id="lbExportTranslationData" Runat="server" OnClick="lbExportTranslationData_Click">翻訳設定出力</asp:LinkButton>
																<% } %>
															</div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															ノベルティ名(管理用)
														</td>
														<td class="search_item_bg">
															<asp:TextBox id="tbNoveltyName" runat="server" Width="100" MaxLength="100"></asp:TextBox>
														</td>
														<td class="search_title_bg">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															開催状態
														</td>
														<td class="search_item_bg" colspan="3">
															<asp:DropDownList ID="ddlNoveltyStatus" runat="server">
															</asp:DropDownList>
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
	<!--△ 検索 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">ノベルティ設定一覧</h2></td>
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
												<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
													<tr>
														<td width="675" style="height: 22px">
															<asp:Label ID="lbPager1" runat="server"></asp:Label>
														</td>
														<td class="action_list_sp">
															<asp:Button ID="btnInsertTop" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" />
														</td>
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
														<td align="center" width="120">ノベルティID</td>
														<td align="center" width="224">ノベルティ名(表示用)</td>
														<td align="center" width="224">ノベルティ名(管理用)</td>
														<td align="center" width="150">開始日時<br/>終了日時</td>
														<td align="center" width="70">有効フラグ</td>
														<td align="center" width="70">開催状態</td>
													</tr>
													<asp:Repeater id="rList" ItemType="w2.Domain.Novelty.NoveltyModel" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick=" listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateNoveltyRegisterUrl(((NoveltyModel)Container.DataItem).NoveltyId, Constants.ACTION_STATUS_UPDATE)) %>') ">
																<td align="center"><%# WebSanitizer.HtmlEncode(((NoveltyModel)Container.DataItem).NoveltyId) %></td>
																<td align="left" style="word-break: break-all;"><%# WebSanitizer.HtmlEncode(((NoveltyModel)Container.DataItem).NoveltyDispName) %></td>
																<td align="left" style="word-break: break-all;"><%# WebSanitizer.HtmlEncode(((NoveltyModel)Container.DataItem).NoveltyName) %></td>
																<td align="center">
																		<%#: DateTimeUtility.ToStringForManager(Item.DateBegin, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %><br />
																		<%#: DateTimeUtility.ToStringForManager(Item.DateEnd, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter, "-") %>
																</td>
																<td align="center"><%# WebSanitizer.HtmlEncode(((NoveltyModel)Container.DataItem).ValidFlgText)%></td>
																<td align="center">
																	<span visible='<%# (((NoveltyModel)Container.DataItem).Status == NoveltyModel.StatusType.Preparing) || (((NoveltyModel)Container.DataItem).Status == NoveltyModel.StatusType.Suspended) %>' runat="server">
																		<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_NOVELTY, FIELD_NOVELTY_STATUS, ((NoveltyModel)Container.DataItem).Status))%>
																	</span>
																	<span visible='<%# (((NoveltyModel)Container.DataItem).Status == NoveltyModel.StatusType.OnGoing) %>' runat="server">
																		<strong><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_NOVELTY, FIELD_NOVELTY_STATUS, ((NoveltyModel)Container.DataItem).Status))%></strong>
																	</span>
																	<span visible='<%#(((NoveltyModel)Container.DataItem).Status == NoveltyModel.StatusType.Finished) %>' runat="server">
																		<span disabled="true" runat="server"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_NOVELTY, FIELD_NOVELTY_STATUS, ((NoveltyModel)Container.DataItem).Status))%></span>
																	</span>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="6"></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
										</tr>
										<tr>
											<td class="action_list_sp">
												<asp:Button id="btnInsertBottom" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" />
											</td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
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
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
	</tr>
</table>
</asp:Content>