<%--
=========================================================================================================
  Module      : 受注ワークフロー実行履歴詳細ページ(OrderWorkflowExecHistoryDatails.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" Title="ワークフロー実行履歴詳細" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="OrderWorkflowExecHistoryDetails.aspx.cs" Inherits="Form.OrderWorkflowExecHistory.OrderWorkflowExecHistoryDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="691" border="0">
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<td><h1 class="page-title">ワークフロー実行履歴詳細</h1></td>
	</tr>

	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="684" border="0">
				<tr>
					<div class="action_part_top">
						<asp:Button ID="btnBackTop" runat="server" Text="  一覧へ戻る  " OnClick="btnBack_Click" />
					</div>
				</tr>
				<tr>
					<td class="list_box_bg" align="center">
						<table cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="658" border="0" align="center">
													<tr align="center">
														<td class="edit_title_bg" width="6%">履歴ID</td>
														<td class="search_item_bg" width="10%"><asp:Literal ID="lExecHistoryId" runat="server" /></td>
														<td class="edit_title_bg" width="7%">開始日時</td>
														<td class="search_item_bg" width="15%"><asp:Literal ID="lDateBegin" runat="server" /></td>
														<td class="edit_title_bg" width="7%">終了日時</td>
														<td class="search_item_bg" width="15%"><asp:Literal ID="lDateEnd" runat="server" /></td>
														<td class="edit_title_bg" width="10%">実行ステータス</td>
														<td class="<%: GetCssClassToListItemBackGround() %>" width="10%"><asp:Literal ID="lExecStatus" runat="server" /></td>
														<td align="center" class="edit_title_bg" width="10%">成功件数/実行件数</td>
														<td class="search_item_bg" width="10%"><asp:Literal ID="lSuccessRate" runat="server" /></td>
													</tr>
												</table>
												<br>
												<table class="list_table" cellspacing="1" cellpadding="3" width="658" border="0" align="center">
													<tr>
														<td width="280px" class="edit_title_bg">実行起点</td>
														<td class="search_item_bg"><asp:Literal ID="lWorkFlowPlace" runat="server" /></td>
													</tr>
													<tr>
														<td class="edit_title_bg">ワークフロー名<br>(シナリオワークフロー名)</td>
														<td class="search_item_bg"><asp:Literal ID="lWorkFlowName" runat="server" /><br/><asp:Literal ID="lScenarioName" runat="server" /></td>
													</tr>
													<tr>
														<td class="edit_title_bg">ワークフロー種別</td>
														<td class="search_item_bg"><asp:Literal ID="lWorkFlowType" runat="server" /></td>
													</tr>
													<tr>
														<td class="edit_title_bg">実行タイミング</td>
														<td class="search_item_bg"><asp:Literal ID="lExecTiming" runat="server" /></td>
													</tr>
													<tr>
														<td class="edit_title_bg">履歴作成日時</td>
														<td class="search_item_bg"><asp:Literal ID="lDateCreateded" runat="server" /></td>
													</tr>
													<tr>
														<td class="edit_title_bg">メッセージ</td>
														<td class="search_item_bg"><asp:Literal ID="lMessage" runat="server" /></td>
													</tr>
													<tr>
														<td class="edit_title_bg">実行ステータスを失敗ありに変更<br />※下記の備考をよく読んでからお使いください</td>
														<td class="search_item_bg">
															<asp:Button ID="btnCancelRunningBottom" Text="  変更  " OnClick="btnChangeExecStatus_Click" Enabled="False" runat="server" OnClientClick="return confirm('ワークフローの実行ステータスを「失敗あり」に変更しますか？※「成功件数/実行件数」の情報が履歴から失われる可能性があります。詳細は下記の備考をご覧ください。')"/>
														</td>
													</tr>
												</table>
												<br>
												<table class="list_table" cellspacing="1" cellpadding="3" width="658" border="0" align="center">
													<tr>
														<td width="280px" class="edit_title_bg">実行者</td>
														<td class="search_item_bg"><asp:Literal ID="lLastChanged" runat="server" /></td>
													</tr>
												</table>
											</td>
										</tr>
									</table>
									<br />
									<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
										<tr class="info_item_bg">
											<td align="left">備考<br />
												<p>「実行ステータスを失敗ありに変更」について</p>
												<p style="margin-left: 30px">説明: 実行ステータスが、「実行中」または「保留中」の場合に「失敗あり」にすることができます。</p>
												<p style="margin-left: 30px">使い方:「シナリオ」または「受注ワークフロー」を実行している状態で、他にシナリオを実行する場合に使います。</p>
												<p style="margin-left: 30px; color: red;">※注意点: 「成功件数/実行件数」が確認できなくなります。(ワークフローの実行が正常に終了した場合、履歴が上書きされます。)</p>
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
</table>
<div class="action_part_bottom">
	<asp:Button ID="btnBackBottom" Text="  一覧へ戻る  "  OnClick="btnBack_Click" runat="server" />
</div>
</asp:Content>