<%--
=========================================================================================================
  Module      : 共有情報管理確認ページ(ShareInfoSettingConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Cs.ShareInfo" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ShareInfoSettingConfirm.aspx.cs" Inherits="Form_ShareInfoSetting_ShareInfoSettingConfirm" Title="" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">共有情報</h1></td>
	</tr>
	<tr>
	<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 詳細 ▽-->
	<tr id="trDetail" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">共有情報詳細</h2></td>
	</tr>
	<tr id="trConfirm" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">共有情報確認</h2></td>
	</tr>
	<tr>
	<td>
	<table class="box_border" cellspacing="0" cellpadding="3" width="784" border="0">
	<tr>
	<td>
	<table cellspacing="1" cellpadding="0" width="100%" border="0">
	<tr>
	<td>
	<table class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
	<td align="center">
	<table cellspacing="0" cellpadding="0" border="0">
	<tr>
	<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
	<td>
		<div class="action_part_top"><input type="button" onclick="Javascript:history.back();" value =" 　戻る 　" />
			<asp:Button id="btnEditTop" runat="server" Text="　編集する　" Visible="False" onclick="btnEdit_Click"></asp:Button>
			<asp:Button id="btnCopyInsertTop" runat="server" Text="　コピー新規登録する　" Visible="False" onclick="btnCopyInsert_Click"></asp:Button>
			<asp:Button id="btnInsertTop" runat="server" Text="　登録する　" Visible="False" onclick="btnInsert_Click"></asp:Button>
			<asp:Button id="btnUpdateTop" runat="server" Text="　更新する　" Visible="False" Width="72px" onclick="btnUpdate_Click"></asp:Button>
			<asp:Button id="btnDeleteTop" runat="server" Text="　削除する　" Visible="False" Width="72px" onclick="btnDelete_Click" OnClientClick="return confirm('情報を削除します。よろしいですか？');"></asp:Button>
		</div>
		<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
			<tbody>
				<tr id="trInfoNo" style="height: 30px;" visible="false" runat="server">
					<td class="detail_title_bg" align="left" width="120">NO</td>
					<td class="detail_item_bg" align="left" colspan="5">
						<asp:Literal id="lInfoNo" runat="server"></asp:Literal>
					</td>
				</tr>
				<tr style="height: 30px;">
					<td class="detail_title_bg" align="left" width="120">区分</td>
					<td class="detail_item_bg" align="left" width="133">
						<asp:Literal id="lInfoKbn" runat="server"></asp:Literal>
					</td>
					<td class="detail_title_bg" align="left" width="120">重要度</td>
					<td class="detail_item_bg" align="left" width="133">
						<asp:Literal id="lImportance" runat="server"></asp:Literal>
					</td>
					<td class="detail_title_bg" align="left" width="120">送信元</td>
					<td class="detail_item_bg" align="left" width="132">
						<asp:Literal id="lSenderName" runat="server"></asp:Literal>
					</td>
				</tr>
				<tr>
					<td class="detail_title_bg" align="left" width="120" rowspan="2">共有テキスト</td>
					<td class="detail_item_bg" align="left" colspan="5" valign="top">
						<asp:Literal id="lInfoTextKbn" runat="server"></asp:Literal>
					</td>
				</tr>
				<tr style="height: 210px;">
					<td class="detail_item_bg" align="left" colspan="5" valign="top">
						<asp:Literal id="lInfoText" runat="server"></asp:Literal>
					</td>
				</tr>
				<tr id="trDateCreated" style="height: 30px;" visible="false" runat="server">
					<td class="detail_title_bg" align="left" width="120">作成日時</td>
					<td class="detail_item_bg" align="left" colspan="5">
						<asp:Literal id="lDateCreated" runat="server"></asp:Literal>
					</td>
				</tr>
				<%if (this.ActionStatus == Constants.ACTION_STATUS_DETAIL) { %>
				<tr>
					<td class="detail_title_bg" align="left" width="120">確認状況</td>
					<td class="detail_item_bg" align="left" colspan="5">
						<table class="detail_table" cellspacing="1" cellpadding="3" border="0" width="100%">
							<tr>
								<td class="detail_title_bg" align="center" width="10"></td>
								<td class="detail_item_bg" align="left" colspan="3">
									<asp:Literal id="lReadRate" runat="server"></asp:Literal> %
									（<asp:Literal id="lShareCount" runat="server"></asp:Literal>人中
									<asp:Literal id="lReadCount" runat="server"></asp:Literal>人が確認済みです）
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<%} %>
			</tbody>
		</table>
	</td>
	</tr>
	<tr>
	<td>
		<div align="right">
		<img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
		<table class="detail_table" cellspacing="1" cellpadding="3" width="258" border="0">
			<tr class="detail_title_bg">
				<td align="center" width="170">対象オペレータ</td>
				<%if (this.ActionStatus == Constants.ACTION_STATUS_DETAIL) { %>
				<td align="center" width="80">確認状況</td>
				<%} %>
			</tr>
			<asp:Repeater ID="rReadList" runat="server">
			<ItemTemplate>
				<tr class="detail_item_bg">
					<td align="center">&nbsp;<%# WebSanitizer.HtmlEncode(((CsShareInfoReadModel)Container.DataItem).EX_OperatorName) %></td>
					<%if (this.ActionStatus == Constants.ACTION_STATUS_DETAIL) { %>
					<td align="center"><%# ((CsShareInfoReadModel)Container.DataItem).ReadFlg == Constants.FLG_CSSHAREINFOREAD_READ_FLG_READ ? "○" : "×" %></td>
					<%} %>
				</tr>
			</ItemTemplate>
			</asp:Repeater>																			
		</table>
		</div>
		<div class="action_part_bottom"><input type="button" onclick="Javascript:history.back();" value=" 　戻る 　" />
			<asp:Button id="btnEditBottom" runat="server" Text="　編集する　" Visible="False" onclick="btnEdit_Click"></asp:Button>
			<asp:Button id="btnCopyInsertBottom" runat="server" Text="　コピー新規登録する　" Visible="False" onclick="btnCopyInsert_Click"></asp:Button>
			<asp:Button id="btnInsertBottom" runat="server" Text="　登録する　" Visible="False" onclick="btnInsert_Click"></asp:Button>
			<asp:Button id="btnUpdateBottom" runat="server" Text="　更新する　" Visible="False" Width="72px" onclick="btnUpdate_Click"></asp:Button>
			<asp:Button id="btnDeleteBottom" runat="server" Text="　削除する　" Visible="False" Width="72px" onclick="btnDelete_Click" OnClientClick="return confirm('情報を削除します。よろしいですか？');"></asp:Button>
		</div>
	</td>
	</tr>
	<tr>
	<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
	</table>
	</td>
	</tr>
	<!--△ 詳細 △-->
	<tr>
	<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>