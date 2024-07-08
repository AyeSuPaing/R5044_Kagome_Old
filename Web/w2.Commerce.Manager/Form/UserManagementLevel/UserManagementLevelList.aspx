<%--
=========================================================================================================
  Module      : ユーザー管理レベル登録ページ(UserManagementLevelList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="UserManagementLevelList.aspx.cs" Inherits="Form_UserManagementLevel_UserManagementLevelList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
<tr><td><h1 class="page-title">ユーザー管理レベル設定</h1></td></tr>
<tr><td><img height="10" width="100" border="0" alt="" src="../../Images/Common/sp.gif" /></td></tr>
<tr><td><h2 class="cmn-hed-h2">ユーザー管理レベル設定一覧</h2></td></tr>
<tr>
<td>
<table class="box_border" cellspacing="1" cellpadding="0" width="584" border="0">
<tr>
<td>
<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
<tr>
<td align="center">
<div>
<%-- UPDATE PANEL開始 --%>
<asp:UpdatePanel ID="upUpdatePanel" runat="server">
<ContentTemplate>
<table cellspacing="0" cellpadding="0" border="0">
	<tr>
		<td height="6" colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="6" border="0" /></td>
	</tr>
	<tr>
		<td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="4" border="0" /></td>
	</tr>
	<div id="dvComplete" runat="server" visible="false">
		<tr>
			<td>
				<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
					<tr class="info_item_bg">
						<td align="left">ユーザー管理レベルを登録/更新しました。</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td height="6" colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="6" border="0" /></td>
		</tr>
	</div>
	<tr>
		<td align="right">
			<asp:Button ID="btnAddTop" runat="server" Text="&nbsp;追加&nbsp;" OnClick="btnAdd_Click" />&nbsp;
			<asp:Button ID="btnAllUpdateTop" runat="server" Text="&nbsp;一括更新&nbsp;" OnClick="btnAllUpdate_Click" OnClientClick="return check_confirm();" />
		</td>
		<td width="5"><img alt="" src="../../Images/Common/sp.gif" width="5" height="1" border="0" /></td>
	</tr>
	<tr>
		<td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="4" border="0" /></td>
	</tr>
	<tr>
		<td valign="top">
			<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0" align="center">
					<tr class="list_title_bg">
						<td align="center" colspan="5">ユーザー管理レベル設定項目</td>
					</tr>
					<tr class="list_title_bg">
						<td width="38px" align="center">No</td>
						<td width="250px" align="center">ユーザー管理レベルID</td>
						<td width="300px" align="center">ユーザー管理レベル名称</td>
						<td width="100px" align="center">表示順</td>
						<td width="70px" align="center">削除</td>
					</tr>
					<asp:Repeater id="rUserManagementLevelList" ItemType="UserManagementLevelInput" Runat="server">
					<ItemTemplate>
						<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
							<td align="center">
								<%# Container.ItemIndex + 1 %>
							</td>
							<td align="left">
								<asp:Label ID="lUserManagementLevelId" runat="server" Text="<%# Item.UserManagementLevelId %>" Visible='<%# Item.SeqNo != "" %>' />
								<div runat="server" Visible='<%# Item.SeqNo == "" %>'>
									<asp:TextBox ID="tbUserManagementLevelId" runat="server" Width="150px" MaxLength="30" Text="<%# Item.UserManagementLevelId %>" />
								</div>
							</td>
							<td align="left">
								<asp:TextBox ID="tbUserManagementLevelName" runat="server" Width="230px" MaxLength="30" Text='<%# Item.UserManagementLevelName %>' />
							</td>
							<td align="left">
								<asp:Label ID="lDisplayOrder" runat="server" Text="-" Visible='<%# IsDefaultUserManagementLevel(Item.UserManagementLevelId) %>' />
								<div runat="server" visible="<%# IsDefaultUserManagementLevel(Item.UserManagementLevelId) == false %>">
									<asp:TextBox ID="tbDisplayOrder" runat="server" Width="50px" MaxLength="3" Text='<%# Item.DisplayOrder %>' />
								</div>
							</td>
							<td align="center">
								<asp:CheckBox id="cbDeleteFlg" runat="server" Checked="<%# Item.DelFlg == UserManagementLevelInput.FLG_DELETE_VALID %>" 
											Visible='<%# ((Item.SeqNo != "") && (IsDefaultUserManagementLevel(Item.UserManagementLevelId) == false)) %>'></asp:CheckBox>
								<asp:LinkButton runat="server" Text="&nbsp;キャンセル&nbsp;" OnClick="btnCancel_Click" CommandArgument="<%# Container.ItemIndex %>" Visible='<%# Item.SeqNo== "" %>' />
							</td>
						</tr>
						<asp:HiddenField ID="hfSeqNo" runat="server" Value='<%# Item.SeqNo %>' />
						<asp:HiddenField ID="hfUserManagementLevelId" runat="server" Value='<%# Item.UserManagementLevelId %>' />
						<asp:HiddenField ID="hfUserManagementLevelName_old" runat="server" Value='<%# Item.UserManagementLevelNameOld %>' />
						<asp:HiddenField ID="hfDisplayOrder_old" runat="server" Value='<%# Item.DisplayOrderOld %>' />
						<asp:HiddenField ID="hfDelFlg_old" runat="server" Value='<%# Item.DelFlgOld %>' />
					</ItemTemplate>
				</asp:Repeater>
			</table>
		</td>
		<td><img alt="" src="../../Images/Common/sp.gif" width="5" height="1" border="0" /></td>
	</tr>
	<tr>
		<td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="4" border="0" /></td>
	</tr>
	<tr>
		<td align="right">
			<asp:Button ID="btnAddBottom" runat="server" Text="&nbsp;追加&nbsp;" OnClick="btnAdd_Click" />&nbsp;
			<asp:Button ID="btnAllUpdateBottom" runat="server" Text="&nbsp;一括更新&nbsp;" OnClick="btnAllUpdate_Click" OnClientClick="return check_confirm();" />
		</td>
		<td width="5"><img alt="" src="../../Images/Common/sp.gif" width="5" height="1" border="0" /></td>
	</tr>
	<tr>
		<td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="4" border="0" /></td>
	</tr>
</table>
</ContentTemplate>
</asp:UpdatePanel>
<%-- UPDATE PANELここまで --%>
<table id="note" class="info_table" width="758" border="0" cellspacing="1" cellpadding="3">
	<tr>
		<td align="left" class="info_item_bg" colspan="2">備考<br />
			ユーザー管理レベルID「normal」は削除できません。
		</td>
	</tr>
</table>
<br />
</div>
</td>
</tr>
</table>
</td>
</tr>
</table>
</td>
</tr>
<tr><td><img height="10" width="1" border="0" alt="" src="../../Images/Common/sp.gif" /></td></tr>
</table>
<script type="text/javascript">
<!--
//=============================================================================================
// 更新確認ダイアログ生成
//=============================================================================================
function check_confirm()
{
return confirm("表示内容で更新します。\nよろしいですか？");
}
//-->
</script>
</asp:Content>