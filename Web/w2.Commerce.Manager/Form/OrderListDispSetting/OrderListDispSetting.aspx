<%--
=========================================================================================================
  Module      : 受注一覧表示設定(OrderListDispSetting.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OrderListDispSetting.aspx.cs" Inherits="Form_Order_OrderListDispSetting" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<table cellspacing="0" cellpadding="0" width="791" border="0">
		<tr>
			<td>
				<h1 class="page-title">受注一覧表示設定</h1>
			</td>
		</tr>
		<tr>
			<td style="width: 792px">
				<img height="10" alt="" src="../../Images/Common/sp.gif" /></td>
		</tr>
		<tr>
			<td style="width: 792px">
				<img height="10" alt="" src="../../Images/Common/sp.gif" /></td>
		</tr>
		<!--▽ 一覧 ▽-->
		<tr>
			<td>
				<h2 class="cmn-hed-h2">受注一覧表示設定一覧</h2>
			</td>
		</tr>
		<tr>
			<td style="width: 792px">
				<table class="box_border" cellspacing="1" cellpadding="0" width="300" border="0">
					<tr>
						<td>
							<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
								<%-- タブ --%>
								<div class="tabs-style">
									<asp:LinkButton ID="lbChangeToOrderList" CssClass="" Text="受注情報" CommandName="orderListTab" OnCommand="SelectTab_Onclick" runat="server"></asp:LinkButton>
									<% if (Constants.STORE_PICKUP_OPTION_ENABLED) { %>
									<asp:LinkButton
										ID="lbOrderStorePickup"
										CssClass="" Text="店舗受取注文情報"
										CommandName="orderStorePickupListTab"
										OnCommand="SelectTab_Onclick"
										runat="server" />
									<% } %>
									<asp:LinkButton ID="lbOrderWorkflow" CssClass="" Text="受注ワークフロー" CommandName="orderWorkFlowTab" OnCommand="SelectTab_Onclick" runat="server"></asp:LinkButton>
									<%-- データ更新用ボタン（常に非表示）--%>
									<div style="display: none;">
										<asp:Button ID="btnUpdateTop" OnClick="btnUpdateTop_OnClick" runat="server" />
									</div>
								</div>

								<tr>
									<td class="tab-contents">
										<table class="list_table" cellspacing="1" cellpadding="3" width="250" border="0" style="background-color: #FFE4B5">
											<%--▽スレッド部分▽--%>
											<thead>
												<tr class="list_title_bg">
													<asp:HiddenField ID="hfDispSettingKbn" Value="" runat="server" />
													<td align="center" width="50"></td>
													<td align="center" width="1010">項目名</td>
													<td align="center" width="410">表示/非表示
														<asp:CheckBox ID="cbDispTargetAll" Checked="<%# CheckedCheckBox() %>" OnCheckedChanged="btnUpdateTop_OnClick" AutoPostBack="true" runat="server" />
													</td>
												</tr>
												<%--▽注文ID(固定表示)▽--%>
												<%--ハンバーガー表示用--%>
												<tr class="fixedtr areaDesign">
													<td></td>
													<td align="center">
														<asp:Literal ID="lColmunName" runat="server" />
													</td>
													<td align="center">
														<asp:CheckBox Checked="True" Enabled="False" runat="server" />
													</td>
												</tr>
												<%--△ 注文ID(固定表示) △--%>
											</thead>
											<%--△ スレッド △--%>
											<%--▽ テーブルデータ ▽--%>
											<tbody id="sort_table_entire">
											<asp:Repeater runat="server" ID="rManagerListDispSetting" ItemType="w2.Domain.ManagerListDispSetting.ManagerListDispSettingModel">
												<ItemTemplate>
													<tr class="tr_sort">
														<td align="center" class="search_item_bg sort_handle">
															<%--▽ハンバーガー表示用▽--%>
															<div id="buns_only_hamburger">
																<div></div>
																<div></div>
															</div>
															<%--△ハンバーガー表示用△--%>
														</td>
														<input type="hidden" id="hdDispOrderNumber" class="orderNum" runat="server" />
														<td align="center">
															<input type="hidden" id="hdColmunName" runat="server" value="<%# Item.DispColmunName %>" />
															<asp:Literal Text="<%# ColumunNameConversionToLogicalName(Item.DispColmunName)%>" ID="lDispColmunName" runat="server" />
														</td>
														<td align="center">
															<div class="check_button_list">
																<asp:CheckBox ID="cbDispFlag" CssClass="checkedboxs" Checked="<%# (Item.DispFlag == Constants.FLG_MANAGERLISTDISPSETTING_DISP_FLAG_ON) %>" runat="server" OnCheckedChanged="btnUpdateTop_OnClick" AutoPostBack="true" />
															</div>
														</td>
													</tr>
												</ItemTemplate>
											</asp:Repeater>
											</tbody>
											<%--△ テーブルデータ △--%>
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
	<!--△ 一覧 △-->

	<script type="text/javascript">
		// ページロード時に表示順を取得
		window.onload = RecalculationNo();

		$(function () {
			$('#sort_table_entire').sortable({
				handle: '.sort_handle',
				axis: "y",
				opacity: 0.8,

				start: function (event, ui) {
					ui.item.css("box-shadow", "5px 5px 5px rgba(0,0,0,0.6)");
					ui.item.css("cursor", "n-resize");
				},

				stop: function (event, ui) {
					RecalculationNo();
					ui.item.css("box-shadow", "none");
					ui.item.css("cursor", "default");
					updateDispSetting();
				},

				containment: '#sort_table_entire',
				helper: fixPlaceHolderWidth
			});
		});

		// ドラッグ時のItemの表示幅を固定化
		function fixPlaceHolderWidth(event, ui) {
			ui.children().each(function () {
				$(this).width($(this).width());
			});
			return ui;
		};

		// ドロップ時に項目の表示順を計算
		function RecalculationNo() {
			var rows = $('#sort_table_entire .orderNum');
			for (var i = 0 ; i <= rows.length; ++i) {
				$($('.orderNum')[i]).attr('value', i + 1);
			}
		}

		document.getElementById("ctl00_ContentPlaceHolderBody_cbDispTargetAll").addEventListener("click", selected_target_all, false);
		// チェックボックス一括切替
		function selected_target_all() {
			var chk = document.getElementById("ctl00_ContentPlaceHolderBody_cbDispTargetAll");

			for (var i = 0; i < $('.checkedboxs input').length; i++) {
				if (chk.checked) {
					$('.checkedboxs input').prop("checked", true);
				} else {
					$('.checkedboxs input').prop("checked", false);
				}
			}
		}

		// データ更新用
		function updateDispSetting() {
			<%= this.ClientScript.GetPostBackEventReference(btnUpdateTop, string.Empty) %>
		}

	</script>

	<style type="text/css">
		.list_table td
		{
			font-size: 14px;
		}

		fixedtr,
		.areaDesign
		{
			background-color: #FFFFFF;
			border-bottom: 2px solid #d0cfcf;
		}

		#sort_table_entire
		{
			margin-top: 10px;
			margin-bottom: 10px;
		}

		.display_no_design
		{
			position: relative;
			width: 20px;
			margin-top: 2px;
		}

		.sort_handle
		{
			top: 2px;
			overflow: hidden;
		}

		#buns_only_hamburger
		{
			margin: 12px;
			float: left;
			position: relative;
			height: 20px;
			width: 20px;
			display: inline-block;
			box-sizing: border-box;
			background-color: #fff;
			border: 0 solid #888;
			border-radius: 1px;
			visibility: hidden;
		}

		.tr_sort
		{
			background-color: white;
			font-size: large;
		}

			.tr_sort:hover #buns_only_hamburger
			{
				visibility: visible;
			}

		.sort_handle:hover
		{
			cursor: n-resize;
		}

		#buns_only_hamburger div
		{
			position: absolute;
			left: 2px;
			height: 4px;
			width: 21px;
			background-color: #888;
			border-radius: 2px;
			border: 1px solid #888;
			display: inline-block;
			box-sizing: border-box;
		}

			#buns_only_hamburger div:nth-of-type(1)
			{
				top: 5px;
			}

			#buns_only_hamburger div:nth-of-type(2)
			{
				bottom: 5px;
			}
	</style>

</asp:Content>
