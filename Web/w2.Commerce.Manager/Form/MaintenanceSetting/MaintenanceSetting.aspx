<%--
=========================================================================================================
  Module      : Maintenance Setting(MaintenanceSetting.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MaintenanceSetting.aspx.cs" Inherits="Form_MaintenanceSetting_MaintenanceSetting" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
	<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">メンテナンス設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<td><h2 class="cmn-hed-h2">メンテナンス設定</h2></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr id="trSaveComplete" runat="server" visible="false">
		<td>
			<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
				<tr class="info_item_bg">
					<td align="left">メンテナンスページを保存しました。</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td align="center">
						<table cellspacing="0" cellpadding="0" border="0">
							<tr>
								<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
							</tr>
							<tr>
								<td>
									<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0" colspan="3">
										<tr>
											<td class="edit_title_bg" align="left">メンテナンスページ</td>
											<td class="edit_item_bg" align="left">
												<asp:Button ID="btnStart" runat="server" Text="  開始  " OnClientClick="return confirm('メンテナンスページを開始しますか？')" OnClick="btnStart_Click"></asp:Button>
												<asp:Button ID="btnEnd" runat="server" Text="  終了  " Enabled="false" OnClientClick="return confirm('メンテナンスページを終了しますか？')" OnClick="btnEnd_Click"></asp:Button>
												<asp:Label ID="lbMaintenance" runat="server" ForeColor="red" Text="現在メンテナンスページになっています。"></asp:Label>
											</td>
										</tr>
										<tr>
											<td class="edit_title_bg" align="left" rowspan="2">編集</td>
											<td class="edit_item_bg" align="left">
												<input type= "button" onclick="OpenWysiwyg('<%= tbEdit.ClientID %>');" value="  HTMLエディタ  " /><br />
												<asp:TextBox ID="tbEdit" runat="server" TextMode="MultiLine" Width="900" Height="250"></asp:TextBox>
											</td>
										</tr>
										<tr>
											<td class="edit_item_bg" align="left">
												<asp:Button ID="btnSave" runat="server" Text="  保存する  " OnClick="btnSave_Click" OnClientClick="return Confirm();"></asp:Button>
												<input type="button" class="cmn-btn-sub-action" value="  表示する  " onclick="javascript:open_window('<%= Constants.PATH_ROOT_FRONT_PC + CONST_FILE_NAME_MAINTENANCE %>','Maintenance','width=850,height=585,top=120,left=320,status=NO,scrollbars=yes');"/>
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="4" border="0" /></td>
							</tr>
							<tr>
								<td colspan="6">
									<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
										<tr class="info_item_bg">
											<td align="left">備考<br />
												表示するボタンは今のメンテナンスページを表示します。
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
</table>
<script type="text/javascript">
	var popup;

	// Open Wysiwyg
	function OpenWysiwyg(id)
	{
		popup = OpenWysiwygAndGetWindow(id, true);
	}

	// Confirm function
	function Confirm()
	{
		var isConfirm = confirm('保存しますか？');
		if(isConfirm && (typeof (popup) != 'undefined') && (popup != null))
		{
			textAreaWysiwygBinded.removeAttribute("disabled");
			popup.close();
		}

		return isConfirm;
	}
</script>
</asp:Content>
