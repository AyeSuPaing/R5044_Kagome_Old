<%--
=========================================================================================================
  Module      : サイト設定確認画面(SiteConfigurationConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Global.Config" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="SiteConfigurationConfirm.aspx.cs" Inherits="Form_Configuration" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderHead" Runat="Server">
	<script type="text/javascript" >
		// コンフィグ読み込み確認
		function confirmLoadingConfig() {
			return confirm("<%: WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CONFIG_LOAD_ALERT) %>");
		}
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
	<table class="main" cellspacing ="0" cellpadding="0" width="791" border="0">
		<tr>
			<td><h1 class="page-title">サイトの設定<br /><br />サイト設定確認</h1></td>
		</tr>
		<br>

		<tr>
		<td>
		<div align="right">
			<asp:Button ID="btnUpdate" Text="  更新する  " runat="server" OnClick="btnUpdate_Click" OnClientClick="return confirmLoadingConfig();" />
			<input type="submit" id="backbutton" value="戻る" class="btn_main2" runat="server" onserverclick="backbutton_Click">
		</div>
		</td>
		</tr>
	</table>
	<table class="list_table" runat="server" id="tbErrorMessage" cellspacing="1" cellpadding="3" width="758" border="0">
		<tr>
			<td class="edit_title_bg" align="center">エラーメッセージ</td>
		</tr>
		<tr>
			<td class="error_message">
				<asp:Label runat="server" ID="lbErrorMessage" ForeColor="red" />
			</td>
		</tr>
	</table>
	<table cellspacing="0" cellpadding="0" border="0">
		<tr>
			<td colspan="2">
				<div id="tabs">
					<asp:Repeater ID="rReadKbn" runat="server" DataSource="<%# this.ReadKbnList %>" >
						<ItemTemplate>
							<div id="tabs-<%# Container.ItemIndex + 1 %>">
								<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
									<tbody>
									<tr>
										<td class="edit_title_bg" align="center" width="40%" rowspan="3">キー</td>
										<td class="edit_title_bg" align="center" width="60%">説明</td>
									</tr>
									<tr>
										<td class="edit_title_bg" align="center">変更前の設定値</td>
									</tr>
									<tr>
										<td class="edit_title_bg" align="center" style="color:red">変更後の設定値</td>
									</tr>
									<asp:Repeater id="rSettings" runat="server" DataSource="<%# GetConfigSettings((string)Container.DataItem) %>" ItemType="w2.App.Common.SettingNode">
										<ItemTemplate>
										<tr runat="server" visible='<%# IsDisplayReadKbn(Item.ReadKbn)%>'>
											<td class="edit_title_bg " align="left" colspan="2">
												<h2><%#: Item.ReadKbn %></h2>
											</td>
										</tr>
										<tr>
											<td class="edit_title_bg" align="left" style="word-break:break-all" rowspan="3">
												<span data-popover-message="読取区分「<%#: Item.ReadKbn %>」"><%#: Item.Key %></span>
											</td>
											<td class="edit_item_bg" align="left" style="word-break:break-all"><%#: Item.Comment %></td>
										</tr>
										<tr>
											<td class="edit_item_bg" align="left" style="word-break:break-all">
												<asp:label ID="lValue" runat="server" Text='<%#: string.IsNullOrEmpty(Item.BeforeValue) ? "（値なし）" : Item.BeforeValue %>' Width="650" />
											</td>
										</tr>
										<tr>
											<td class="edit_item_bg" align="left" style="word-break:break-all">
												<asp:label ID="Label1" runat="server" Text='<%#: string.IsNullOrEmpty(Item.Value) ? "（値なし）" : Item.Value %>' Width="650" ForeColor="red" />
											</td>
										</tr>
										</ItemTemplate>
									</asp:Repeater>
									</tbody>
								</table>
							</div>
						</ItemTemplate>
					</asp:Repeater>
				</div>
			</td>
		</tr>
	</table>
</asp:Content>

