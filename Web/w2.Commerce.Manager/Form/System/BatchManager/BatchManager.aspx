<%--
=========================================================================================================
  Module     バッチ管理 : (BatchManager.aspx)
 ････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Common/DefaultPage_responsive.master" AutoEventWireup="true" CodeFile="BatchManager.aspx.cs" Inherits="Form_System_BatchManager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<style>
		tr {
			text-align: center;
		}

		a:link {
			text-decoration: none;
			color: #fff;
		}

		.taskList td {
			width: 10%;
			height: 30px;
		}

		.batchBtn:hover {
			background: #ad3c03;
			border: 1px solid #ad3c03;
			box-shadow: 0px 1px 5px 0px rgba(0, 0, 0, 0.4);
		}

		.batchBtn {
			background: #db6f39;
			color: #fff;
			border-radius: 100px;
			margin: 3px;
			padding: 6px 20px;
			line-height: 1.1;
			border: 1px solid #db6f39;
			outline: none;
			cursor: pointer;
			font-size: 14px;
			box-shadow: 0px 0px 3px 0px rgba(0, 0, 0, 0.2);
			-webkit-transition: 0.3s;
			-o-transition: 0.3s;
			transition: 0.3s;
			vertical-align: middle;
			width: auto !important;
			-webkit-appearance: none;
		}

			.batchBtn[disabled] {
				box-shadow: 0px 0px 3px 0px rgba(0, 0, 0, 0.2);
				background-color: #efefef !important;
				border-color: #e2e2e2 !important;
				color: #999;
				pointer-events: none;
				cursor: not-allowed;
			}

		.tdBtn {
			min-width: 120px;
		}
	</style>

	<table>
		<tr>
			<td>
				<h1 class="page-title" style="text-align: left;">バッチ一覧</h1>
				<br />
			</td>
		</tr>
		<tr>
			<td style="color: red; text-align: left;">
				<asp:Literal runat="server" ID="lErrorMessage" /><br />
			</td>
		</tr>
		<tr>
			<td>
				<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
					<tr class="list_title_bg">
						<td>タスク名
						</td>
						<td>タスク状態
						</td>
						<td>コマンドライン引数
						</td>
						<td>タスク実行状態
						</td>
						<td>前回実行時刻
						</td>
						<td>次回実行時刻
						</td>
						<th colspan="4"></th>
					</tr>
					<asp:Repeater runat="server" ID="rTaskList" OnItemDataBound="rTaskList_ItemDataBound" OnItemCommand="rTaskList_ItemCommand">
						<ItemTemplate>
							<tr class="taskList list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
								<td style="text-align: left">
									<%# ((Hashtable)Container.DataItem)["Name"] %>
								</td>
								<td>
									<%# ((Hashtable)Container.DataItem)["Enabled"] %>
								</td>
								<td style="text-align: left">
									<%# ((Hashtable)Container.DataItem)["Arguments"] %>
								</td>
								<td>
									<%# ((Hashtable)Container.DataItem)["State"] %>
								</td>
								<td>
									<%# ((Hashtable)Container.DataItem)["LastRunTime"] %>
								</td>
								<td>
									<%# ((Hashtable)Container.DataItem)["NextRunTime"] %>
								</td>
								<td class="tdBtn">
									<asp:LinkButton runat="server" Text="有効化" ID="lbActivation" CssClass="batchBtn" CommandName="active" CommandArgument='<%# ((Hashtable)Container.DataItem)["Name"] %>' />
								</td>
								<td class="tdBtn">
									<asp:LinkButton runat="server" Text="無効化" ID="lbDisabling" CssClass="batchBtn" CommandName="disable" CommandArgument='<%# ((Hashtable)Container.DataItem)["Name"] %>' />
								</td>
								<td class="tdBtn">
									<asp:LinkButton runat="server" Text="実行" ID="lbExecution" CssClass="batchBtn" CommandName="execution" CommandArgument='<%# ((Hashtable)Container.DataItem)["Name"] %>' />
								</td>
								<td style="min-width: 150px;">
									<asp:LinkButton runat="server" Text="タスクの停止" ID="lbStop" CssClass="batchBtn" CommandName="stop" CommandArgument='<%# ((Hashtable)Container.DataItem)["Name"] %>' />
								</td>
							</tr>
						</ItemTemplate>
					</asp:Repeater>
				</table>
			</td>
		</tr>
	</table>
</asp:Content>
