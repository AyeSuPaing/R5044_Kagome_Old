<%--
=========================================================================================================
  Module      : 共有情報一覧ページ(ShareInfoList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Cs.ShareInfo" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="ShareInfoList.aspx.cs" Inherits="Form_ShareInfo_ShareInfoList" Title="" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderHead" Runat="Server">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">

<table cellspacing="0" cellpadding="0" width="800" border="0">
	<tr>
	<td align="center">
	<table cellspacing="0" cellpadding="0" width="784" border="0">
	<!--▽ 検索 ▽-->
	<tr>
	<td>
	<table cellspacing="0" cellpadding="0" border="0">
		<tr>
			<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
		</tr>
		<tr>
			<td class="search_box">
				<div class="datagrid">
				<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
					<tr>
						<td class="search_title_bg alt" width="95">表示切替</td>
						<td class="search_item_bg" colspan="3">
							<asp:radiobuttonlist id="rblReadKbn" runat="server" Width="350" RepeatDirection="Horizontal" CssClass="radio_button_list" RepeatLayout="Flow">
								<asp:ListItem Value="reading">未確認/ピン止め　</asp:ListItem>
								<asp:ListItem Value="unread">未確認　</asp:ListItem>
								<asp:ListItem Value="all">全て表示　</asp:ListItem>
							</asp:radiobuttonlist>
						</td>
						<td class="search_title_bg alt" width="95">並び順</td>
						<td class="search_item_bg" width="130">
							<asp:dropdownlist id="ddlSortKbn" runat="server">
								<asp:ListItem Value="0">作成日/昇順</asp:ListItem>
								<asp:ListItem Value="1">作成日/降順</asp:ListItem>
								<asp:ListItem Value="2">重要度/昇順</asp:ListItem>
								<asp:ListItem Value="3">重要度/降順</asp:ListItem>
							</asp:dropdownlist>
						</td>
						<td class="search_btn_bg alt" width="83" rowspan="3">
							<div class="search_btn_main"><asp:button id="btnSearch" runat="server" Text="　検索　" onclick="btnSearch_Click"></asp:button></div>
							<div class="search_btn_sub"><a href="<%= Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_SHAREINFO_LIST %>">クリア</a></div>
						</td>
					</tr>																		
					<tr>
						<td class="search_title_bg alt" width="95">区分</td>
						<td class="search_item_bg" width="130"><asp:dropdownlist id="ddlInfoKbn" runat="server"></asp:dropdownlist></td>
						<td class="search_title_bg alt" width="95">共有テキスト</td> 
						<td class="search_item_bg" width="355" colspan="3"><asp:textbox id="tbInfoText" runat="server" Width="300"></asp:textbox></td>
					</tr>
					<tr>
						<td class="search_title_bg alt" width="95">重要度</td>
						<td class="search_item_bg" width="130">
							<asp:DropDownList id="ddlImportance" runat="server"></asp:DropDownList>
						</td>
						<td class="search_title_bg alt" width="95">送信元</td>
						<td class="search_item_bg" width="355" colspan="3">
							<asp:DropDownList id="ddlSender" CssClass="select2-select" Width="20%" runat="server"></asp:DropDownList>
						</td>
					</tr>
				</table>
				</div>
			</td>
		</tr>
	</table>
	</td>
	</tr>
	<!--△ 検索 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
	<td>
	<table width="100%" border="0" cellspacing="0" cellpadding="0">
		<tr>
			<td>
				<!--▽ ページング ▽-->
				<table class="list_pager" cellspacing="0" cellpadding="0" border="0">
					<tr>
						<td><asp:label id="lbPager1" Runat="server"></asp:label></td>
					</tr>
				</table>
				<!--△ ページング △-->
			</td>
		</tr>
		<tr>
			<td valign="top"><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
		</tr>
		<tr>
		<td valign="top">
		<div class="datagrid">
			<table class="list_table" border="0" cellpadding="3" cellspacing="1" width="100%">
				<thead>
				<tr class="list_title_bg">
					<th colspan="6">共有情報</th>
				</tr>
				</thead>
				<tr class="alt">
					<td align="center" width="50">ピン止め</td>
					<td align="center">区分</td>
					<td align="center" width="360">共有テキスト</td>
					<td align="center">重要度</td>
					<td align="center">送信元</td>
					<td align="center">作成日時</td>
				</tr>
				<asp:repeater id="rList" ItemType="w2.App.Common.Cs.ShareInfo.CsShareInfoModel" Runat="server">
				<ItemTemplate>
					<tr class="list_item_bg1" onmouseover="listselect_mover(this)" onmouseout="listselect_mout1(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# CreateDetailUrl(((CsShareInfoModel)Container.DataItem).InfoNo) %>')">
						<td align="center" >
							<%# ((CsShareInfoModel)Container.DataItem).EX_PinnedFlg == Constants.FLG_CSSHAREINFOREAD_PINNED_FLG_PINNED ? "<img src='../../Images/Cs/shareinfo_pin.png' width=16 height=16 />" : "" %>
						</td>
						<td width="10%" align="center" style='<%# ((CsShareInfoModel)Container.DataItem).EX_ReadFlg == Constants.FLG_CSSHAREINFOREAD_READ_FLG_READ ? "" : "font-weight: bold;" %>'>
							<%# WebSanitizer.HtmlEncode(((CsShareInfoModel)Container.DataItem).EX_InfoKbnName) %>
						</td>
						<td align="left" style='<%# ((CsShareInfoModel)Container.DataItem).EX_ReadFlg == Constants.FLG_CSSHAREINFOREAD_READ_FLG_READ ? "" : "font-weight: bold;" %>'>
							<%# (((CsShareInfoModel)Container.DataItem).InfoTextKbn == Constants.FLG_CSSHAREINFO_INFO_TEXT_KBN_TEXT) ? 
								WebSanitizer.HtmlEncode(AbbreviateString(((CsShareInfoModel)Container.DataItem).EX_InfoTextExceptHtmlTag, ((CsShareInfoModel)Container.DataItem).EX_ReadFlg == Constants.FLG_CSSHAREINFOREAD_READ_FLG_READ ? 36 : 32)) : 
								AbbreviateString(((CsShareInfoModel)Container.DataItem).EX_InfoTextExceptHtmlTag, ((CsShareInfoModel)Container.DataItem).EX_ReadFlg == Constants.FLG_CSSHAREINFOREAD_READ_FLG_READ ? 36 : 32) %>
						</td>
						<td width="10%" align="center">
							<%# WebSanitizer.HtmlEncode(((CsShareInfoModel)Container.DataItem).EX_InfoImportanceName) %>
						</td>
						<td align="center" style='<%# ((CsShareInfoModel)Container.DataItem).EX_ReadFlg == Constants.FLG_CSSHAREINFOREAD_READ_FLG_READ ? "" : "font-weight: bold;" %>'>
							<%# WebSanitizer.HtmlEncode(((CsShareInfoModel)Container.DataItem).EX_SenderName) %>
						</td>
						<td align="center" style='<%# Item.EX_ReadFlg == Constants.FLG_CSSHAREINFOREAD_READ_FLG_READ ? "" : "font-weight: bold;" %>'>
							<%#: DateTimeUtility.ToStringForManager(Item.DateCreated, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %>
						</td>
					</tr>
				</ItemTemplate>
				</asp:repeater>
				<tr id="trListError" class="list_alert" runat="server" Visible="False">
					<td id="tdErrorMessage" colspan="6" runat="server"></td>
				</tr>
			</table>
		</div>
		</td>
		</tr>
	</table>
	</td>
	</tr>
	</table>
	</td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
	<script type="text/javascript">
		$(function () {
			ddlSelect();
		});
	</script>

</asp:Content>
