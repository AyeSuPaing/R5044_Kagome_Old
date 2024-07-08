<%--
=========================================================================================================
  Module      : レコメンド設定一覧ページ(RecommendList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.Domain.Recommend.Helper" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="RecommendList.aspx.cs" Inherits="Form_Recommend_RecommendList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<script type="text/javascript">
	// レコメンドレポート、レコメンドID設定
	function setRecommend(recommendId) {
		// 親ウィンドウが存在する場合
		if (window.opener != null) {
			// 選択されたレコメンドIDを設定する
			window.opener.setRecommendId(recommendId);
			// 閉じる
			window.close();
		}
	}
</script>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">レコメンド設定</h1></td>
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
															レコメンドID
														</td>
														<td class="search_item_bg" width="110">
															<asp:TextBox id="tbRecommendId" runat="server" Width="100" MaxLength="30"></asp:TextBox>
														</td>
														<td class="search_title_bg" width="125">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															レコメンド名(管理用)
														</td>
														<td class="search_item_bg" width="110">
															<asp:TextBox id="tbRecommendName" runat="server" Width="100" MaxLength="100"></asp:TextBox>
														</td>
														<td class="search_title_bg" width="95">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															並び順
														</td>
														<td class="search_item_bg" width="110">
															<asp:DropDownList ID="ddlSortKbn" runat="server" Width="105"></asp:DropDownList>
														</td>
														<td class="search_btn_bg" width="83" rowspan="2">
															<div class="search_btn_main"><asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_RECOMMEND_LIST %>">クリア</a>
																<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a>
															</div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															レコメンド区分
														</td>
														<td class="search_item_bg">
															<asp:DropDownList ID="ddlRecommendKbn" runat="server" Width="105">
															</asp:DropDownList>
														</td>
														<td class="search_title_bg">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															開催状態
														</td>
														<td class="search_item_bg" colspan="3">
															<asp:DropDownList ID="ddlStatus" runat="server" Width="105">
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
		<td><h2 class="cmn-hed-h2">レコメンド設定一覧</h2></td>
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
										<% if (divComp.Visible == false) { %>
										<tr>
											<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<% } %>
										<tr>
											<td>
												<div id="divComp" runat="server" class="action_part_top" visible="false">
													<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
														<tr class="info_item_bg">
															<td align="left">適用優先順を更新しました。</td>
														</tr>
													</table>
												</div>
												<!--▽ ページング ▽-->
												<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
													<tr>
														<td width="587" style="height: 22px">
															<asp:Label ID="lbPager" runat="server"></asp:Label>
														</td>
														<% if (this.IsDisplayingForRecommendReport == false) { %>
														<td class="action_list_sp">
															<asp:Button id="btnUpdatePriorityTop" Runat="server" Text="  一括更新  " OnClick="btnUpdatePriority_Click" />
															<asp:Button ID="btnInsertTop" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" />
														</td>
														<% } %>
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
														<td align="center" width="120">レコメンドID</td>
														<td align="center" width="154">レコメンド名(管理用)</td>
														<td align="center" width="80">レコメンド区分</td>
														<td align="center" width="140">開始日時<br/>終了日時</td>
														<td align="center" width="70">有効フラグ</td>
														<td align="center" width="70">開催状態</td>
														<td align="center" width="70">適用優先順</td>
													</tr>
													<asp:Repeater id="rList" Runat="server" ItemType="w2.Domain.Recommend.Helper.RecommendListSearchResult">
														<ItemTemplate>
															<% if (this.IsDisplayingForRecommendReport == false) { %>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" id="trItem<%# Container.ItemIndex %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)">
																<td align="center" onclick="listselect_mclick(getElementById('trItem<%# Container.ItemIndex %>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateRecommendRegisterUrl(Item.RecommendId, Constants.ACTION_STATUS_UPDATE)) %>')"><%#: Item.RecommendId %></td>
																<td align="left"  onclick="listselect_mclick(getElementById('trItem<%# Container.ItemIndex %>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateRecommendRegisterUrl(Item.RecommendId, Constants.ACTION_STATUS_UPDATE)) %>')" style="word-break: break-all;"><%#: Item.RecommendName %></td>
																<td align="center"  onclick="listselect_mclick(getElementById('trItem<%# Container.ItemIndex %>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateRecommendRegisterUrl(Item.RecommendId, Constants.ACTION_STATUS_UPDATE)) %>')"><%#: Item.RecommendKbnText %></td>
																<td align="center"  onclick="listselect_mclick(getElementById('trItem<%# Container.ItemIndex %>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateRecommendRegisterUrl(Item.RecommendId, Constants.ACTION_STATUS_UPDATE)) %>')">
																		<%#: DateTimeUtility.ToStringForManager(Item.DateBegin, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %><br />
																		<%#: DateTimeUtility.ToStringForManager(Item.DateEnd, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter, "-") %>
																</td>
																<td align="center" onclick="listselect_mclick(getElementById('trItem<%# Container.ItemIndex %>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateRecommendRegisterUrl(Item.RecommendId, Constants.ACTION_STATUS_UPDATE)) %>')"><%#: Item.ValidFlgText %></td>
																<td align="center" onclick="listselect_mclick(getElementById('trItem<%# Container.ItemIndex %>'), '<%# WebSanitizer.UrlAttrHtmlEncode(CreateRecommendRegisterUrl(Item.RecommendId, Constants.ACTION_STATUS_UPDATE)) %>')">
																	<span visible="<%# (Item.Status == RecommendListSearchResult.StatusType.Preparing) || (Item.Status == RecommendListSearchResult.StatusType.Suspended) %>" runat="server">
																		<%#: ValueText.GetValueText(Constants.TABLE_RECOMMEND, FIELD_RECOMMEND_STATUS, Item.Status) %>
																	</span>
																	<span visible="<%# Item.Status == RecommendListSearchResult.StatusType.OnGoing %>" runat="server">
																		<strong><%#: ValueText.GetValueText(Constants.TABLE_RECOMMEND, FIELD_RECOMMEND_STATUS, Item.Status) %></strong>
																	</span>
																	<span visible="<%# Item.Status == RecommendListSearchResult.StatusType.Finished %>" runat="server">
																		<span disabled="true" runat="server"><%#: ValueText.GetValueText(Constants.TABLE_RECOMMEND, FIELD_RECOMMEND_STATUS, Item.Status) %></span>
																	</span>
																</td>
																<td align="center" onclick="javascript:void(0);">
																	<asp:TextBox ID="tbPriority" runat="server" Text="<%# Item.Priority %>" Width="50" MaxLength="7" />
																</td>
															</tr>
															<% }else { %>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" id="trItem<%# Container.ItemIndex %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="javascript:setRecommend(<%#: CreateJavaScriptSetRecommendId(Item.RecommendId) %>)">
																<td align="center"><%#: Item.RecommendId %></td>
																<td align="left" style="word-break: break-all;"><%#: Item.RecommendName %></td>
																<td align="center"><%#: Item.RecommendKbnText %></td>
																<td align="center">
																	<%#: DateTimeUtility.ToStringForManager(Item.DateBegin, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %><br />
																	<%#: DateTimeUtility.ToStringForManager(Item.DateEnd, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter, "-") %>
																</td>
																<td align="center"><%#: Item.ValidFlgText %></td>
																<td align="center">
																	<span visible="<%# (Item.Status == RecommendListSearchResult.StatusType.Preparing) || (Item.Status == RecommendListSearchResult.StatusType.Suspended) %>" runat="server">
																		<%#: ValueText.GetValueText(Constants.TABLE_RECOMMEND, FIELD_RECOMMEND_STATUS, Item.Status) %>
																	</span>
																	<span visible="<%# Item.Status == RecommendListSearchResult.StatusType.OnGoing %>" runat="server">
																		<strong><%#: ValueText.GetValueText(Constants.TABLE_RECOMMEND, FIELD_RECOMMEND_STATUS, Item.Status) %></strong>
																	</span>
																	<span visible="<%# Item.Status == RecommendListSearchResult.StatusType.Finished %>" runat="server">
																		<span disabled="true" runat="server"><%#: ValueText.GetValueText(Constants.TABLE_RECOMMEND, FIELD_RECOMMEND_STATUS, Item.Status) %></span>
																	</span>
																</td>
																<td align="center"><%# Item.Priority %></td>
															</tr>
															<% } %>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="7"></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
										</tr>
										<tr>
											<% if (this.IsDisplayingForRecommendReport == false) { %>
											<td class="action_list_sp">
												<asp:Button id="btnUpdatePriorityBottom" Runat="server" Text="  一括更新  " OnClick="btnUpdatePriority_Click" />
												<asp:Button id="btnInsertBottom" runat="server" Text="  新規登録  " OnClick="btnInsert_Click" />
											</td>
											<% } %>
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