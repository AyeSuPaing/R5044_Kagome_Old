<%--
=========================================================================================================
  Module      : 共有情報確認ページ(ShareInfoConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Reference Page="~/Form/ShareInfo/ShareInfoList.aspx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="ShareInfoConfirm.aspx.cs" Inherits="Form_ShareInfo_ShareInfoConfirm" Title="" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderHead" Runat="Server">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="800" border="0">
	<tr>
	<td align="center">
	<table cellspacing="0" cellpadding="0" width="784" border="0">
	<tr>
	<td>
	<table cellspacing="1" cellpadding="0" width="100%" border="0">
	<tr>
	<td>
	<table cellspacing="0" cellpadding="0" border="0">
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
	<td>
		<asp:UpdatePanel id="updatePanel2" runat="server">
		<ContentTemplate>
		<div class="datagrid">
			<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
				<tr>
					<td class="detail_title_bg alt" align="left" width="120">確認状況</td>
					<td class="detail_item_bg" align="left" width="253" colspan="2">
						<div id="divInfoRead" runat="server">
							<img src='../../Images/Cs/shareinfo_accept.png' alt="確認済み" width="16" height="16" />
							<asp:button id="btnUnread" runat="server" Text="  未確認へ  "  onclick="btnUnread_Click"></asp:button>
						</div>
						<div id="divInfoUnread" runat="server">
							<asp:button id="btnRead" runat="server" Text="  確認済みにする  " onclick="btnRead_Click"></asp:button>
						</div>
					</td>
					<td class="detail_title_bg alt" align="left" width="120">区分</td>
					<td class="detail_item_bg" align="left" width="133">
						<asp:Literal ID="lShareInfoKbn" runat="server"></asp:Literal>
					</td>
					<td class="detail_title_bg alt" align="left" width="120">送信元</td>
					<td class="detail_item_bg" align="left" width="132">
						<asp:Literal ID="lSenderName" runat="server"></asp:Literal>
					</td>
				</tr>
				<tr>
					<td class="detail_title_bg alt" align="left" width="120">ピン止め</td>
					<td class="detail_item_bg" align="left" width="253" colspan="2">
						<div id="divInfoPinned" runat="server">
							<img src='../../Images/Cs/shareinfo_pin.png' alt="" width=16 height=16 />
							<asp:button id="btnUnpin" runat="server" Text="  はずす  " onclick="btnUnpin_Click"></asp:button>
						</div>
						<div id="divInfoNopin" runat="server">
							<asp:button id="btnPin" runat="server" Text="  ピン止めする  " onclick="btnPin_Click"></asp:button>
						</div>
					</td>
					<td class="detail_title_bg alt" align="left" width="120">重要度</td>
					<td class="detail_item_bg" align="left" width="133">
						<asp:Literal ID="lImportance" runat="server"></asp:Literal>
					</td>
					<td class="detail_title_bg alt" align="left" width="120">作成日時</td>
					<td class="detail_item_bg" align="left" width="132">
						<asp:Literal ID="lDateCreated" runat="server"></asp:Literal>
					</td>
				</tr>
				<tr>
					<td class="detail_title_bg alt" align="left" width="120">共有テキスト</td>
					<td class="detail_item_bg" align="left" colspan="6" valign="top">
						<br />
						<asp:Literal ID="lInfoText" runat="server"></asp:Literal>
						<br /><br />
					</td>
				</tr>
			</table>
		</div>
		<div class="action_part_bottom">
			<input type="button" onclick="Javascript:window.location.replace(document.referrer);" value ="　一覧へ戻る　" />
		</div>
		</ContentTemplate>
		</asp:UpdatePanel>
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
	function refresh_share_info_count()
	{
		if (window.opener && (window.opener.closed == false) && window.opener.refresh_share_info_count) {
			window.opener.refresh_share_info_count();
		}
	}
</script>
</asp:Content>

