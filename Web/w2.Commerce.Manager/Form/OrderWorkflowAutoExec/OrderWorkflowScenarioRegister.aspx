<%--
=========================================================================================================
  Module      : 受注ワークフローシナリオ登録ページ(OrderWorkflowScenarioRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>
<%@ Reference Page="~/Form/PdfOutput/PdfOutput.aspx" %>
<%@ Register TagPrefix="uc" TagName="ScheduleRegisterForm" Src="~/Form/Common/ScheduleRegisterForm.ascx" %>
<%@ Page Language="C#" Title="受注ワークフローシナリオ設定" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="OrderWorkflowScenarioRegister.aspx.cs" Inherits="Form.OrderWorkflowAutoExec.OrderWorkflowScenarioSettingRegister" MaintainScrollPositionOnPostback="true" %>
<%@ MasterType VirtualPath="~/Form/Common/PopupPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">受注ワークフローシナリオ設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 詳細 ▽-->
	<tr>
		<% if ((this.ActionStatus == Constants.ACTION_STATUS_INSERT) || (this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT)) {%>
		<td><h2 class="cmn-hed-h2">シナリオ登録</h2></td>
		<%} %>
		<% if ((this.ActionStatus == Constants.ACTION_STATUS_UPDATE) || (this.ActionStatus == Constants.ACTION_STATUS_COMPLETE)) {%>
		<td><h2 class="cmn-hed-h2">シナリオ編集</h2></td>
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
									<div id="divComp" runat="server" class="action_part_top" Visible="False">
										<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
											<tr class="info_item_bg">
												<td align="left"><asp:Label ID="lMessage" runat="server"></asp:Label></td>
											</tr>
										</table>
									</div>
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td>
												<div class="action_part_top">
													<asp:Button ID="btnBackListTop" runat="server" Text="  一覧へ戻る  " Visible="True" OnClick="btnBackList_Click" />
													<asp:Button ID="btnBackTop" runat="server" Text="  戻る  " Visible="True" OnClick="btnBack_Click" />
													<asp:Button ID="btnConfirmTop" Text="  確認する  " runat="server" Visible="True" OnClick="btnConfirm_Click" />
												</div>
												<table id="tbdyOrderOwnerErrorMessages" class="detail_table" visible="False" style="margin-bottom: 10px" runat="server">
													<tr>
														<td class="edit_title_bg" align="center" colspan="4">エラーメッセージ</td>
													</tr>
													<tr>
														<td class="search_item_bg" align="left" colspan="4">
															<span style="color: red">
																<asp:Literal ID="lbErrorMessages" runat="server"></asp:Literal>
															</span>
														</td>
													</tr>
												</table>

												<table class="detail_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr>
														<td align="center" class="edit_title_bg" Width="131">シナリオ名<span class="notice">*</span></td>
														<td align="left" class="search_item_bg"><asp:TextBox ID="tbScenariotName" Width="442" runat="server"></asp:TextBox></td>
													</tr>
												</table>

												<div id="sort_table_movement_area">
												<table id="sort_table_entire" style="background-color: #FFE4B5;" class="detail_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<thead>
														<tr>
															<td align="center" class="edit_title_bg" Width="120" colspan="2">実行順</td>
															<td align="left" class="edit_title_bg">ワークフロー<span class="notice">*</span></td>
															<td align="center" class="edit_title_bg" Width="63"><asp:Button id="btnAdd" Text="  追加  " runat="server" OnClick="btnAdd_Click" /></td>
														</tr>
													</thead>
													<tbody id="sort_table">
													<asp:Repeater ID="rScenario" ItemType="w2.Domain.OrderWorkflowScenarioSetting.OrderWorkflowScenarioSettingItemModel" runat="server">
														<ItemTemplate>
															<tr class="tr_sort">
																<td align="center" class="search_item_bg sort_handle" Width="47">
																	<div id="buns_only_hamburger">
																		<div></div>
																		<div></div>
																	</div>
																</td>
																<td align="center" class="search_item_bg" Width="73">
																	<div class="display_no_design">
																		<span class="display_no">
																			<%#: Item.ScenarioNo %>
																		</span>
																	</div>
																	<input type="hidden" id="beforeNo" class="before_no" value="<%#: Item.ScenarioNo %>" runat="server" />
																	<input type="hidden" id="afterNo" class="after_no" value="<%#: Item.ScenarioNo %>" runat="server" />
																</td>
																<td align="left" class="search_item_bg">
																	<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED){ %>
																		<asp:LinkButton ID="lbTargetWorkflowByNormal" runat="server" Text="受注情報" OnClick="lbTargetWorkflowByNormal_Click" AutoPostBack="true" CssClass='<%# Item.TargetWorkflowKbn == Constants.FLG_ORDERWORKFLOWSCENARIOSETTING_TARGETKBN_NORMAL ? "selected_targetkbn" : "targetkbn" %>' />
																		<asp:LinkButton ID="lbTargetWorkflowByFixedPurchase" runat="server" Text="定期台帳" OnClick="lbTargetWorkflowByFixedPurchase_Click" AutoPostBack="true" CssClass='<%# Item.TargetWorkflowKbn == Constants.FLG_ORDERWORKFLOWSCENARIOSETTING_TARGETKBN_FIXEDPURCHASE ? "selected_targetkbn" : "targetkbn" %>' />
																	<% } %>
																	<asp:DropDownList ID="ddlWorkflowList" OnSelectedIndexChanged="ddlWorkflowList_SelectedIndexChanged" AutoPostBack="True" Width="450" runat="server" ></asp:DropDownList>
																	<a class="showDetails" href="javascript:void(0);" onclick="ShowDetails(this);" style="font-weight: bold; margin-left: 10px;"></a>
																	<div class="displayWorkflowDetails" style="margin-top: 2px; padding: 5px; font-size: 13px; background-color: #f0f0f0; display: none;">
																		<p style="margin-left: 15px; margin-right: 15px;">
																			<asp:Literal id="lWorkflowDetails" runat="server"></asp:Literal>
																		</p>
																	</div>
																</td>
																<td align="center" class="edit_title_bg">
																	<asp:Button id="btnDelete" Text="  削除  " OnClick="btnDelete_Click" Visible='True' runat="server" />
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													</tbody>
												</table>
												</div>

												<uc:ScheduleRegisterForm ID="ucScheduleRegisterForm" runat="server" />
												<br />
												<table class="detail_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr>
														<td class="edit_title_bg" align="center" Width="131">有効フラグ</td>
														<td class="search_item_bg" align="left">
															<asp:CheckBox ID="cbValidFlg" runat="server" Text="有効" />
														</td>
													</tr>
												</table>
												<br/>
												<table class="info_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr>
														<td align="left" class="info_item_bg" colspan="2">備考<br />
															■ スケジュール実行について<br />
															<table class="no-border no-padding">
																<tr>
																	<td>月単位</td>
																	<td>指定した日付で毎月実行されます。 対象月に指定した日付が存在しない場合は対象月での実行がスキップされます。</td>
																</tr>
															</table>
														</td>
													</tr>
												</table>
												<div class="action_part_bottom">
													<asp:Button ID="btnBackListBottom" Text="  一覧へ戻る  " Visible="True" OnClick="btnBackList_Click"  runat="server" />
													<asp:Button ID="btnBackBottom" Text="  戻る  " Visible="True" OnClick="btnBack_Click" runat="server" />
													<asp:Button ID="btnConfirmBottom" Text="  確認する  " Visible="True" OnClick="btnConfirm_Click" runat="server" />
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
	$(function() {
		$('#sort_table').sortable({
			handle: '.sort_handle',
			axis: "y",
			opacity: 0.75,
			start: function(event, ui) {
				ui.item.css("box-shadow", "5px 5px 5px rgba(0,0,0,0.6)");
				ui.item.css("cursor", "n-resize");
			},

			stop: function(event, ui) {
				recalculationNo();
				ui.item.css("box-shadow", "none");
				ui.item.css("cursor", "default");
			},
			containment: '#sort_table_movement_area',
			helper: fixPlaceHolderWidth,
		});

		function recalculationNo() {
			var rows = $('#sort_table .display_no');
			for (var i = 0; i <= rows.length; ++i) {
				$($('.display_no')[i]).text(i + 1);
				$($('.after_no')[i]).attr('value', i + 1);
			}
		}

		function fixPlaceHolderWidth(event, ui) {
			ui.children().each(function() {
				$(this).width($(this).width());
			});
			return ui;
		};
	});

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
		if (e.nextElementSibling.firstElementChild.innerText.match(/\S/g)) {
			if (e.nextElementSibling.style.display == "block") {
				e.nextElementSibling.style.display = "none";
				e.innerText = "説明"
			} else {
				e.nextElementSibling.style.display ="block";
				e.innerText = "閉じる"
			}
		}
	}
</script>

<style type="text/css">
	#sort_table_entire {
		margin-top: 10px;
		margin-bottom: 10px;
	}

	.display_no_design {
		position: relative;
		width: 20px;
		margin-top: 2px;
	}
	.sort_handle {
		top: 2px;
		overflow: hidden;
	}

	#buns_only_hamburger {
		margin: 12px;
		float: left;
		position: relative;
		height: 20px;
		width: 100%;
		display: inline-block;
		box-sizing: border-box;
		background-color: #fff;
		border: 0px solid #888;
		border-radius: 1px;
		visibility: hidden;
	}

	.tr_sort:hover #buns_only_hamburger {
		visibility: visible;
	}

	.sort_handle:hover {
		cursor: n-resize;
	}

	#buns_only_hamburger div {
		position: absolute;
		left: 2px;
		height: 4px;
		width: 18px;
		background-color: #888;
		border-radius: 2px;
		border: 1px solid #888;
		display: inline-block;
		box-sizing: border-box;
	}
	#buns_only_hamburger div:nth-of-type(1) {
		top: 5px;
	}
	#buns_only_hamburger div:nth-of-type(2) {
		bottom: 5px;
	}

	.targetkbn {
		background: #DDDDDD;
		color: #000 !important;
		border-radius: 100px;
		padding: 3px 7px;
		line-height: 0.8;
		outline: none;
		cursor: pointer;
		font-size: 13px;
		box-shadow: 0px 0px 3px 0px rgba(0, 0, 0, 0.2);
		-webkit-transition: 0.3s;
		-o-transition: 0.3s;
		transition: 0.3s;
		vertical-align: middle;
		width: auto !important;
		-webkit-appearance: none;
		text-decoration: none !important;
		margin: 2px;
	}

	.selected_targetkbn {
		background: #db6f39;
		color: #fff !important; 
		border-radius: 100px;
		padding: 3px 7px;
		line-height: 0.8;
		outline: none;
		cursor: pointer;
		font-size: 13px;
		box-shadow: 0px 0px 3px 0px rgba(0, 0, 0, 0.2);
		-webkit-transition: 0.3s;
		-o-transition: 0.3s;
		transition: 0.3s;
		vertical-align: middle;
		width: auto !important;
		-webkit-appearance: none;
		text-decoration: none !important;
		margin: 2px;
	}
</style>

</asp:Content>
