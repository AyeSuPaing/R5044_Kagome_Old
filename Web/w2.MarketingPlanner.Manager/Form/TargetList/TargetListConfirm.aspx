<%--
=========================================================================================================
  Module      : ターゲットリスト設定確認ページ(TargetListConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="TargetListConfirm.aspx.cs" Inherits="Form_TargetList_TargetListConfirm" %>
<%@ Import Namespace="w2.App.Common.TargetList" %>
<%@ Reference Page="~/Form/TargetList/TargetListRegister.aspx" %>
<%-- マスタ出力コントロール --%>
<%@ Register TagPrefix="uc" TagName="MasterDownLoad" Src="~/Form/Common/MasterDownLoadPanel.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">ターゲットリスト情報</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 詳細 ▽-->
	<tr>
		<% if (m_strActionKbn == Constants.ACTION_STATUS_DETAIL) {%>
		<td><h2 class="cmn-hed-h2">ターゲットリスト情報詳細</h2></td>
		<%} %>
		<% if ((m_strActionKbn == Constants.ACTION_STATUS_UPDATE) || (m_strActionKbn == Constants.ACTION_STATUS_INSERT) || (m_strActionKbn == Constants.ACTION_STATUS_COPY_INSERT)) {%>
		<td><h2 class="cmn-hed-h2">ターゲットリスト情報確認</h2></td>
		<%} %>
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
													<asp:Button ID="btnBackTop" Text="  戻る  " runat="server" OnClick="btnBack_Click" />
													<asp:Button ID="btnEditTop" Text="  編集する  " runat="server" OnClick="btnEdit_Click" />
													<asp:Button id="btnCopyInsertTop" runat="server" Text="  コピー新規登録する  " Visible="False" onclick="btnCopyInsert_Click"></asp:Button>
													<asp:Button ID="btnDeleteTop" Text="  削除する  " runat="server" OnClick="btnDelete_Click" OnClientClick="return confirm('削除してもよろしいですか？')" />
													<asp:Button ID="btnInsertTop" Text="  登録する  " runat="server" OnClick="btnInsert_Click" />
													<asp:Button ID="btnUpdateTop" Text="  更新する  " runat="server" OnClick="btnUpdate_Click" />
												</div>
												<table class="detail_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr id="trTargetId" runat="server">
														<td align="left" class="detail_title_bg" width="18%">ターゲットID</td>
														<td align="left" class="detail_item_bg"><asp:Label id="lbTargetId" runat="server"><%: this.TargetId %></asp:Label></td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg" width="18%">ターゲット名</td>
														<td align="left" class="detail_item_bg">
															<asp:Label id="lbTargetName" runat="server">
																<%: this.IsActionStatusDetail
																	? this.TargetList[Constants.FIELD_TARGETLIST_TARGET_NAME]
																	: this.TargetListHashTable[Constants.FIELD_TARGETLIST_TARGET_NAME]
																%>
															</asp:Label>
														</td>
													</tr>
													<tr id="trCounts" runat="server">
														<td align="left" class="detail_title_bg">件数</td>
														<td align="left" class="detail_item_bg">
															<asp:UpdatePanel ID="UpdatePanel1" runat="server" RenderMode="Inline">
																<ContentTemplate>
																	<asp:Label ID="lbDataCounts" runat="server"></asp:Label>
																</ContentTemplate>
																<Triggers>
																	<asp:AsyncPostBackTrigger ControlID="btnExtractTarget" EventName="Click" />
																</Triggers>
															</asp:UpdatePanel>
														</td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg">条件</td>
													
														<td align="left" class="detail_item_bg">
														<asp:Label ID="lbImportTypeTargetCondition" runat="server" Visible="False" />
														<asp:Repeater ID="rConditions" runat="server">
														<ItemTemplate>

															<div Visible="<%# Container.ItemIndex != 0 %>" style="padding: 3px 0px 3px 0px;" runat="server">
																<%#: (((ITargetListCondition)Container.DataItem).GetConditionType((ITargetListCondition)Container.DataItem) == TargetListCondition.CONDITION_TYPE_AND) ? "かつ" : "または" %><br />
															</div>

															<asp:Repeater DataSource="<%# ((ITargetListCondition)Container.DataItem).MakeBindData((ITargetListCondition)Container.DataItem) %>" ItemType="w2.App.Common.TargetList.TargetListCondition" runat="server">
															<ItemTemplate>

																<div class="<%# ((((List<TargetListCondition>)((Repeater)Container.Parent).DataSource).Count) != 1) ? "confirm_group_item" : "" %>">
																	<div Visible="<%# Container.ItemIndex != 0 %>" style="padding: 3px 0px 3px 0px;" runat="server">
																		<%#: (Item.GroupConditionType == TargetListCondition.CONDITION_TYPE_AND) ? "かつ" : "または" %>
																	</div>
																	<div Visible="<%# Item.DataKbn != TargetListCondition.DATAKBN_SQL_CONDITION %>" runat="server">
																		<%#: Item.DataKbnString %>
																		:
																		<%#: Item.DataFieldString %>
																		が
																		<span id="spanValue" runat="server" Visible="<%# string.IsNullOrEmpty(FixedPurchaseText(Item.FixedPurchaseKbn)) %>">
																			<%#: Item.Values[0].Name %>
																		</span>
																		<span id="spanFixedPurchaseSettingMonth" runat="server" Visible="<%# Item.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE %>">
																			<%#: FixedPurchaseText(Item.FixedPurchaseKbn) %>
																			：
																			<%#: FixedPurchasePattern(Item.Values[0].Value) %>
																			<span runat="server" Visible="<%# string.IsNullOrEmpty(FixedPurchasePattern(Item.Values[0].Value)) == false %>">ヶ月ごと</span>
																			<%#: FixedPurchasePattern(Item.Values[0].Value, 1) %>
																			<span runat="server" Visible="<%# string.IsNullOrEmpty(FixedPurchasePattern(Item.Values[0].Value, 1)) == false %>">日</span>
																			に届ける
																		</span>
																		<span id="spanFixedPurchaseSettingMonthDate" runat="server" Visible="<%# Item.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS %>">
																			<%#: FixedPurchaseText(Item.FixedPurchaseKbn) %>
																			：
																			<%#: FixedPurchasePattern(Item.Values[0].Name, 0) %>
																			日ごとに届ける
																		</span>
																		<span id="spanFixedPurchaseSettingIntervalMonth" runat="server" Visible="<%# Item.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY %>">
																			<%#: FixedPurchaseText(Item.FixedPurchaseKbn) %>
																			：
																			<%#: FixedPurchasePattern(Item.Values[0].Name) %>
																			ヶ月ごと
																			<span runat="server" Visible="<%# string.IsNullOrEmpty(FixedPurchasePattern(Item.Values[0].Value, 1)) == false %>">第</span>
																			<%#: FixedPurchasePattern(Item.Values[0].Name, 1) %>
																			<%#: NumberToDayOfWeek(Item.Values[0].Name,2) %>
																			<span runat="server" Visible="<%# string.IsNullOrEmpty(FixedPurchasePattern(Item.Values[0].Value, 1)) == false %>">曜日</span>
																			に届ける
																		</span>
																		<span id="spanFixedPurchaseKbnWeekAndDay" runat="server" Visible="<%# Item.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY %>">
																			<%#: FixedPurchaseText(Item.FixedPurchaseKbn) %>
																			：
																			<%#: FixedPurchasePattern(Item.Values[0].Name) %>
																			週間ごと
																			<%#: NumberToDayOfWeek(Item.Values[0].Name,1) %>
																			曜日に届ける
																		</span>
																		<%#: Item.EqualSignString %>
																		<%#: Item.OrderExistString %>
																		<%#: Item.FixedPurchaseOrderExistString %>
																		<%#: Item.PointExistString %>
																		<%#: Item.DmShippingHistoryExistString %>
																		<%#: Item.FavoriteExistString %>
																	</div>
																	<div Visible="<%# Item.DataKbn == TargetListCondition.DATAKBN_SQL_CONDITION %>" runat="server">
																		<%#: Item.DataKbnString %>
																		:
																		<%#: Item.Values[0].Value %>
																	</div>
																</div>
																
															</ItemTemplate>
															</asp:Repeater>
															
														</ItemTemplate>
														</asp:Repeater>

														</td>
													</tr>
													<tr id="trSchedule" runat="server">
														<td align="left" class="detail_title_bg">スケジュール</td>
														<td align="left" class="detail_item_bg"><asp:Label ID="lblSchedule" runat="server">
															<%: (this.IsActionStatusDetail)
																? GetScheduleString(this.TargetList)
																: GetScheduleStringByHashTable(this.TargetListHashTable)
														%>
														</asp:Label></td>
													</tr>
												</table>
												<asp:UpdatePanel ID="UpdatePanel2" runat="server" RenderMode="Inline">
													<ContentTemplate>
														<div id="tblTargetExtract" runat="server">
															<img height="12" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
															<table class="detail_table" width="758" border="0" cellpadding="3" cellspacing="1">
																<tr>
																	<td class="detail_title_bg" align="center" colspan="2">
																		ターゲット抽出
																	</td>
																</tr>
																<tr>
																	<td align="left" class="detail_title_bg" width="18%">
																		実行ステータス/進捗
																	</td>
																	<td align="left" class="detail_item_bg">
																		<asp:Label ID="lblExtractStatus" runat="server"></asp:Label>
																		<img
																			ID ="imgLoading"
																			alt="loading"
																			runat="server"
																			src="../../Images/Common/loading.gif"
																			visible="false"
																			height="11" />
																		<asp:Timer ID="tProcessTimer" Interval="1000" OnTick="tProcessTimer_Tick" Enabled="False" runat="server" />
																	</td>
																</tr>
																<tr>
																	<td align="left" class="detail_title_bg">
																		抽出実行
																	</td>
																	<td align="left" class="detail_item_bg">
																		<div id="UpdateMail">
																			<asp:Button runat="server" ID="btnExtractTarget" Text="  抽出実行  " OnClick="btnExtractTarget_Click"
																				OnClientClick="ShowTextSelecting();" />
																		</div>
																	</td>
																</tr>
															</table>
														</div>
													</ContentTemplate>
													<Triggers>
														<asp:AsyncPostBackTrigger ControlID="tProcessTimer" EventName="Tick" />
														<asp:AsyncPostBackTrigger ControlID="btnExtractTarget" EventName="Click" />
													</Triggers>
												</asp:UpdatePanel>
												<%-- マスタ出力 --%>
												<div id="tblMasterOutput" runat="server">
													<img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
													<table class="" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr>
														<td class="search_table">
															<uc:MasterDownLoad runat="server" ID="uMasterDownload" DownloadType="TargetListData" TableWidth="758" />
														</td>
													</tr>
													</table>
												</div>
												<img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
												<div class="action_part_bottom">
													<asp:Button ID="btnBackBottom" Text="  戻る  " runat="server" OnClick="btnBack_Click" />
													<asp:Button ID="btnEditBottom" Text="  編集する  " runat="server" OnClick="btnEdit_Click" />
													<asp:Button id="btnCopyInsertBottom" runat="server" Text="  コピー新規登録する  " Visible="False" onclick="btnCopyInsert_Click"></asp:Button>
													<asp:Button ID="btnDeleteBottom" Text="  削除する  " runat="server" OnClick="btnDelete_Click" OnClientClick="return confirm('削除してもよろしいですか？')" />
													<asp:Button ID="btnInsertBottom" Text="  登録する  " runat="server" OnClick="btnInsert_Click" />
													<asp:Button ID="btnUpdateBottom" Text="  更新する  " runat="server" OnClick="btnUpdate_Click" />												</div>
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
</table>
<script language="javascript" type="text/javascript">
	function ShowTextSelecting() {
		$("#<%= lblExtractStatus.ClientID %>").text('処理中');
	}

	function GoBack(isDetailpage) {
		if (isDetailpage) {
			window.location.replace(document.referrer);
		}
		else {
			history.back();
		}
	}
</script>
</asp:Content>
