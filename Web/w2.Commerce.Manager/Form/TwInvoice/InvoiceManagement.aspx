<%--
=========================================================================================================
  Module      : Invoice Management(InvoiceManagement.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="InvoiceManagement.aspx.cs" Inherits="Form_TwInvoice_InvoiceManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<%@ Import Namespace="w2.Domain.TwInvoice" %>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">電子発票管理</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<td><h2 class="cmn-hed-h2">電子発票発行状況</h2></td>
	</tr>
	<tr>
		<td>
			<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
		</td>
	</tr>
	<tr>
		<td>
			<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
		</td>
	</tr>
		<tr>
		<td>
			<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
		</td>
	</tr>
	<tr>
		<td>
			<% if (rList.Items.Count > 0) { %>
				<table class="list_table tableHeader" cellspacing="1" cellpadding="0" width="784" border="0">
					<asp:Repeater ID="rList" ItemType="TwInvoiceModel" runat="server">
						<HeaderTemplate>
						<tr class="list_title_bg">
							<td align="center" width="20%">有効期間</td>
							<td align="center" width="10%" >コード</td>
							<td align="center" width="15%" >発種別コード名</td>
							<td align="center">発番範囲</td>
							<td align="center">最終発番</td>
							<td align="center" width="10%">残数</td>
						</tr>
						</HeaderTemplate>
						<ItemTemplate>
							<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
								<td align="center"><%#: string.Format("{0:yyyy/MM/dd} ~ {1:yyyy/MM/dd}", Item.TwInvoiceDateStart, Item.TwInvoiceDateEnd)  %></td>
								<td align="center"><%#: Item.TwInvoiceTypeName %></td>
								<td align="center"><%#: Item.TwInvoiceCodeName %></td>
								<td align="center"><%#: string.Format("{0,0:00000000} ~ {1,0:00000000}", Item.TwInvoiceNoStart, Item.TwInvoiceNoEnd)  %></td>
								<td align="center"><%#: string.IsNullOrEmpty(StringUtility.ToEmpty(Item.TwInvoiceNo)) ? null : string.Format("{0,0:00000000}", Item.TwInvoiceNo) %></td>
								<td align="center"><%#: CalculateRemaining(Item) %></td>
							</tr>
						</ItemTemplate>
					</asp:Repeater>
				</table>
			<% } else { %>
				<table class="list_table" cellspacing="1" cellpadding="3" width="734" border="0" style="height:100%"> <!-- 水平ヘッダ -->
					<tr id="trListError" class="list_alert" runat="server" Visible="False">
						<td id="tdErrorMessage" colspan="31" runat="server"></td>
					</tr>
				</table>
			<% } %>
		</td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<td><h2 class="cmn-hed-h2">電子発票アラート設定</h2></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<td>
			<table class="edit_table">
				<tr>
					<td class="edit_title_bg" style="width: 14%; height: 50px;">総残数</td>
					<td class="edit_item_bg">
						<asp:Label ID="lbTotalRemainItems" runat="server" Text="0" />枚
					</td>
				</tr>
				<tr id="trInformation" runat="server">
					<td class="edit_title_bg" style="width: 14%; height: 50px;">アラートメール送信設定値</td>
					<td class="edit_item_bg">
						<asp:Label ID="lbEdit" runat="server" Text="0" />枚<asp:Button ID="btnEdit" OnClick="btnEdit_Click" runat="server" Text="編集"/>
						<br />※ 総残数がこの設定値より小さくなったら、アラートメールを送信する
					</td>
				</tr>
				<tr id="trEdit" runat="server" visible="false">
					<td class="edit_title_bg" style="width: 14%; height: 50px;">アラートメール送信設定値<span class="notice">*</span></td>
					<td class="edit_item_bg">
						<asp:TextBox ID="tbUpdate" runat="server" Width="70" MaxLength="9"/> 枚<asp:Button ID="btnUpdate" OnClick="btnUpdate_Click" runat="server" Text="更新"/>
						<br />※ 総残数がこの設定値より小さくなったら、アラートメールを送信する
						<br />
						<span class="notice"><asp:Label ID="lbErrorAlert" runat="server" /></span>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<td><h2 class="cmn-hed-h2">電子発票番号取得</h2></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<td>
			<table class="edit_table">
				<tr>
					<td class="edit_title_bg" style="width: 14%; height: 50px;">取得枚数</td>
					<td class="edit_item_bg">
						<asp:TextBox ID="tbNumber" runat="server" Width="70" />枚<asp:Button ID="btnImport" runat="server" Text="取得" OnClientClick="return GetAllInvoiceConfirm();" OnClick="btnImport_Click" />
						<br />
						※空で取得の場合は、残す分の全件を取得する
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<table id="tableResult" runat="server" visible="false">
	<tr>
		<td><h2 class="cmn-hed-h2">電子発票番号情報取得結果</h2></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr id="trError">
		<td>
			<table class="edit_table" height="75">
				<tr>
					<td class="edit_title_bg" style="color: red;">
						<asp:Label ID="lbErrorMessageGetElectronicTicketFailed" runat="server" />
						<br />
						<asp:Label ID="lbErrorMessageElectronicTicket" runat="server" />
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr id="trSuccess">
		<td>
			<table class="edit_table" height="75">
				<tr>
					<td class="edit_title_bg" style="color: red;">
						<asp:Label ID="lbMessageGetElectronicTicketSuccess" runat="server" />
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<script type="text/javascript">
	function GetAllInvoiceConfirm() {
		var amount = $('#<%= tbNumber.ClientID %>').val();

		if ((amount == "")
			|| (amount == "-1"))
		{
			return confirm('電子発票番号を全件取得します。よろしいですか？');
		}

		return true;
	}
</script>
</asp:Content>