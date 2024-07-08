<%--
=========================================================================================================
  Module      : 受注ワークフローシナリオ確認ページ(OrderworkflowScenarioConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>
<%@ Reference Page="~/Form/PdfOutput/PdfOutput.aspx" %>
<%@ Page Language="C#" Title="受注ワークフローシナリオ設定" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="OrderWorkflowScenarioConfirm.aspx.cs" Inherits="Form.OrderWorkflowAutoExec.OrderWorkflowScenarioConfirm" MaintainScrollPositionOnPostback="true" %>
<%@ Import Namespace="w2.Common.Web" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">

<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr>
		<td><h1 class="page-title">受注ワークフローシナリオ設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ タイトル △-->
	<!--▽ 詳細 ▽-->
	<tr>

		<% if (this.ActionStatus == Constants.ACTION_STATUS_DETAIL) {%>
			<td><h2 class="cmn-hed-h2">シナリオ詳細</h2></td>
		<% } %>
		<% if (this.ActionStatus == Constants.ACTION_STATUS_INSERT || this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT || this.ActionStatus == Constants.ACTION_STATUS_UPDATE) {%>
			<td><h2 class="cmn-hed-h2">シナリオ確認</h2></td>
		<% } %>
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
													<asp:Button ID="btnEditTop" Text="  編集する  " runat="server" Visible="False" OnClick="btnEdit_Click" />
													<asp:Button id="btnCopyInsertTop" Text="  コピー新規登録する  " runat="server" Visible="False" Onclick="btnCopyInsert_Click"></asp:Button>
													<asp:Button ID="btnDeleteTop" Text="  削除する  " runat="server" Visible="False" OnClick="btnDelete_Click" OnClientClick="return confirm('削除してもよろしいですか？')" />
													<asp:Button ID="btnInsertTop" Text="  登録する  " runat="server" Visible="False" OnClick="btnInsert_Click" OnClientClick="return canExecInsert();" />
													<asp:Button ID="btnUpdateTop" Text="  更新する  " runat="server" Visible="False" OnClick="btnUpdate_Click" />
												</div>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" width="121">シナリオ名</td>
														<td class="search_item_bg" align="left">
															<asp:Label ID="lScenarioName" runat="server"></asp:Label>
														</td>
													</tr>
												</table>
												<br />
												<table class="detail_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr>
														<td align="center" class="detail_title_bg" width="121">実行順</td>
														<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED){ %>
															<td align="center" class="detail_title_bg">実行対象</td>
														<% } %>
														<td align="left" class="detail_title_bg">ワークフロー</td>
													</tr>
													<asp:Repeater ID="rScenario" ItemType="w2.Domain.OrderWorkflowScenarioSetting.OrderWorkflowScenarioSettingItemModel" runat="server">
														<ItemTemplate>
															<tr>
																<td align="center" class="search_item_bg">
																	<%#: Item.ScenarioNo %>
																</td>
																<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED){ %>
																	<td align="center" class="search_item_bg">
																		<%#: Item.TargetWorkflowKbn == Constants.FLG_ORDERWORKFLOWSCENARIOSETTING_TARGETKBN_NORMAL ? "受注情報" : "定期台帳" %>
																	</td>
																<% } %>
																<td align="left" class="search_item_bg">
																	<span style="font-size:17px; line-height: 30px;"><%#: Item.WorkflowName %></span>
																	<a class="showDetails" href="javascript:void(0);" onclick="ShowDetails(this);" style="font-weight: bold; margin-left: 10px;"></a>
																	<br>
																	<div class="displayWorkflowDetails" style="padding: 5px; font-size: 13px; background-color: #f0f0f0; display: none;">
																		<p style="margin-left: 15px; margin-right: 15px;">
																			<%# HtmlSanitizer.HtmlEncodeChangeToBr(Item.Desc1) %>
																		</p>
																	</div>
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
												</table>
												<br />
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
													<td class="detail_title_bg" align="center" width="121">実行タイミング</td>
													<td class="search_item_bg" align="left">
														<asp:Label ID="lExecTiming" runat="server"></asp:Label>
													</td>
													</tr>

													<tr>
														<td class="detail_title_bg" align="center">有効フラグ</td>
														<td class="search_item_bg" align="left">
															<asp:Label ID="lValidFlgt" runat="server"></asp:Label>
														</td>
													</tr>
												</table>
												<br />
												<asp:UpdatePanel runat="server">
												<ContentTemplate>
												<table id="tblManualExec" class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0" runat="server">
													<tr>
														<td colspan="2" class="detail_title_bg" align="center">手動実行</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="20%">準備ステータス</td>
														<td class="detail_item_bg" align="left">
															<asp:Label id="lbPrepareStatus" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">実行ステータス</td>
														<td class="detail_item_bg" align="left">
															<asp:Label id="lbExecuteStatus" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">進捗</td>
														<td class="detail_item_bg" align="left">
															<div id="progress" style="float: left; text-align: right;">
																<asp:Label id="lbProgress" runat="server"></asp:Label>
															</div>
															<div id="progressDescription" style="float: left; margin-left: 10px;">
																<span id="progressDescriptionForWorkflowCount" Visible="False" runat="server"> (実行数 / シナリオ内ワークフロー数)</span><br />
																<span id="progressDescriptionForActionCount" Visible="False" runat="server"> (実行数 / ワークフロー内実行合計数)</span>
															</div>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">
															今すぐ実行
														</td>
														<td class="detail_item_bg" align="left">
															<asp:Button ID="btnExec" runat="server" Text="  今すぐ実行  " OnClick="btnExec_Click" style="text-align:left" CssClass="cmn-btn-sub-action" Width="20%"/>
															<asp:Button ID="btnStop" runat="server" Text="  今すぐ実行停止  " OnClick="btnStop_Click" style="text-align:left" CssClass="cmn-btn-sub-action" Width="20%"/>
														</td>
													</tr>
												</table>
												<asp:HiddenField id="hfReload" runat="server"/>
												</ContentTemplate>
												</asp:UpdatePanel>
												<div class="action_part_bottom">
													<asp:Button ID="btnBackBottom" Text="  戻る  " runat="server" OnClick="btnBack_Click" />
													<asp:Button ID="btnEditBottom" Text="  編集する  " runat="server" Visible="False" OnClick="btnEdit_Click" />
													<asp:Button id="btnCopyInsertBottom" Text="  コピー新規登録する  " runat="server" Visible="False" Onclick="btnCopyInsert_Click"></asp:Button>
													<asp:Button ID="btnDeleteBottom" Text="  削除する  " runat="server" Visible="False" OnClick="btnDelete_Click" OnClientClick="return confirm('削除してもよろしいですか？')" />
													<asp:Button ID="btnInsertBottom" Text="  登録する  " runat="server" Visible="False" OnClick="btnInsert_Click" OnClientClick="return canExecInsert();" />
													<asp:Button ID="btnUpdateBottom" Text="  更新する  " runat="server" Visible="False" OnClick="btnUpdate_Click" />
												</div>
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

<script type="text/javascript">
	setInterval(ReloadPage, 3000);
	function ReloadPage() {
		__doPostBack('<%: hfReload.UniqueID %>', '');
	}

	window.onload = function() {
		var showDetails = document.getElementsByClassName('showDetails');
		var workflowDetails = document.getElementsByClassName('displayWorkflowDetails');
		for (var count = 0; workflowDetails.length > count; count++) {
			if (workflowDetails[count].innerText.match(/\S/g)) {
				showDetails[count].innerText = "説明";
			} else {
				showDetails[count].innerText = "説明";
				showDetails[count].style.color = "#888";
				showDetails[count].style.cursor = "default";
				showDetails[count].style.textDecoration = "none";
			}
		}
	}

	function ShowDetails(e) {

		if (e.nextElementSibling.nextElementSibling.innerText.match(/\S/g)) {
			if (e.nextElementSibling.nextElementSibling.style.display == "block") {
				e.nextElementSibling.nextElementSibling.style.display = "none";
				e.innerText = "説明";
			} else {
				e.nextElementSibling.nextElementSibling.style.display ="block";
				e.innerText = "閉じる";
			}
		}
	}

	var canInsert = true;

	// Check if can execute insert action
	function canExecInsert() {
		if (canInsert == false) {
			$('#<%= btnInsertTop.ClientID %>').attr('disabled', 'disabled');
			$('#<%= btnInsertBottom.ClientID %>').attr('disabled', 'disabled');
			return false;
		}

		canInsert = false;
		return true;
	}
</script>
</asp:Content>
