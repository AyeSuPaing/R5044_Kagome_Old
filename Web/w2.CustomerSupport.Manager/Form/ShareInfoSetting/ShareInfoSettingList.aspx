<%--
=========================================================================================================
  Module      : 共有情報管理一覧ページ(ShareInfoList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Cs.ShareInfo" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ShareInfoSettingList.aspx.cs" Inherits="Form_ShareInfoSetting_ShareInfoSettingList" Title="" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">共有情報</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 検索 ▽-->
	<tr>
	<td>
	<table class="box_border" cellspacing="0" cellpadding="3" width="784" border="0">
	<tr>
	<td>
	<table cellspacing="1" cellpadding="0" width="100%" border="0">
	<tr>
	<td class="search_box_bg">
	<table cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
	<td align="center">
		<table cellspacing="0" cellpadding="0" border="0">
		<tr>
			<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
		</tr>
		<tr>
			<td class="search_box">
				<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
					<tr>
						<td class="search_title_bg" width="95"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" border="0" />NO</td>
						<td class="search_item_bg" width="130"><asp:textbox id="tbInfoNo" runat="server" Width="80"></asp:textbox></td>
						<td class="search_title_bg" width="95"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" border="0" />共有テキスト</td>
						<td class="search_item_bg" width="130"><asp:textbox id="tbInfoText" runat="server" Width="120"></asp:textbox></td>
						<td class="search_title_bg" width="95"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" border="0" />並び順</td>
						<td class="search_item_bg" width="130">
							<asp:dropdownlist id="ddlSortKbn" runat="server">
								<asp:ListItem Value="0">作成日/昇順</asp:ListItem>
								<asp:ListItem Value="1">作成日/降順</asp:ListItem>
								<asp:ListItem Value="2">重要度/昇順</asp:ListItem>
								<asp:ListItem Value="3">重要度/降順</asp:ListItem>
							</asp:dropdownlist>
						</td>
						<td class="search_btn_bg" width="83" rowspan="3">
							<div class="search_btn_main"><asp:button id="btnSearch" runat="server" Text="　検索　" onclick="btnSearch_Click"></asp:button></div>
							<div class="search_btn_sub"><a href="<%= Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_SHAREINFOSETTING_LIST %>">クリア</a></div>
						</td>
					</tr>
					<tr>
						<td class="search_title_bg" width="95"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" border="0" />区分</td>
						<td class="search_item_bg" width="130"><asp:dropdownlist id="ddlInfoKbn" runat="server"></asp:dropdownlist></td>
						<td class="search_title_bg" width="95"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" border="0" />重要度</td>
						<td class="search_item_bg" width="130"><asp:dropdownlist id="ddlImportance" runat="server"></asp:dropdownlist></td>
						<td class="search_title_bg" width="95"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" border="0" />送信元</td>
						<td class="search_item_bg" width="130"><asp:dropdownlist id="ddlSenders" CssClass="select2-select" runat="server" DataValueField="Value" DataTextField="Key" AutoPostBack="False" Width="100%"></asp:dropdownlist></td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
	<!--△ 検索 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">共有情報一覧</h2></td>
	</tr>
	<tr>
	<td>
	<table class="box_border" cellspacing="0" cellpadding="0" width="784" border="0">
	<tr>
	<td>
	<table cellspacing="1" cellpadding="0" width="100%" border="0">
	<tr>
	<td>
	<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td align="center">
			<table cellspacing="0" cellpadding="0" border="0">
				<tr>
					<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
				</tr>
				<tr>
					<td>
						<!--▽ ページング ▽-->
						<table class="list_pager" cellspacing="0" cellpadding="0" border="0" width="758">
							<tr>
								<td width="675"><asp:label id="lbPager1" Runat="server"></asp:label></td>
								<td class="action_list_sp"><asp:Button id="btnInsertTop" runat="server" Text="　新規登録　" onclick="btnInsert_Click"></asp:Button></td>
							</tr>
						</table>
						<!-- ページング-->
					</td>
				</tr>
				<tr>
					<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
				</tr>
				<tr>
					<td>
						<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
							<tr class="list_title_bg">
								<td align="center" width="35">NO</td>
								<td align="center" width="70">区分</td>
								<td align="center" width="343" colspan="2">共有テキスト</td>
								<td align="center" width="50">重要度</td>
								<td align="center" width="100">送信元</td>
								<td align="center" width="80">作成日</td>
								<td align="center" width="80">確認状況</td>
							</tr>
							<asp:Repeater id="rList" ItemType="w2.App.Common.Cs.ShareInfo.CsShareInfoModel" Runat="server">
							<ItemTemplate>
								<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# CreateDetailUrl(((CsShareInfoModel)Container.DataItem).InfoNo) %>')">
									<td align="center"><%# ((CsShareInfoModel)Container.DataItem).InfoNo %></td>
									<td width="65" align="center"><%# WebSanitizer.HtmlEncode(((CsShareInfoModel)Container.DataItem).EX_InfoKbnName) %></td>
									<td align="center"><%# WebSanitizer.HtmlEncode(((CsShareInfoModel)Container.DataItem).EX_InfoTextKbnName) %></td>
									<td align="left">&nbsp;<%# ((((CsShareInfoModel)Container.DataItem).InfoTextKbn == Constants.FLG_CSSHAREINFO_INFO_TEXT_KBN_TEXT) ? WebSanitizer.HtmlEncode(AbbreviateString(((CsShareInfoModel)Container.DataItem).EX_InfoTextExceptHtmlTag, 30)) : AbbreviateString(((CsShareInfoModel)Container.DataItem).EX_InfoTextExceptHtmlTag, 30)).Trim() %></td>
									<td align="center"><%# WebSanitizer.HtmlEncode(((CsShareInfoModel)Container.DataItem).EX_InfoImportanceName) %></td>
									<td width="65" align="center"><%# WebSanitizer.HtmlEncode(((CsShareInfoModel)Container.DataItem).EX_SenderName) %></td>
									<td width="65" align="center"><%#: DateTimeUtility.ToStringForManager(Item.DateCreated, DateTimeUtility.FormatType.ShortDate2Letter) %></td>
									<td align="center"><%# ((CsShareInfoModel)Container.DataItem).EX_ReadCount %>/<%# ((CsShareInfoModel)Container.DataItem).EX_ShareCount %>&nbsp;&nbsp;(<%# ((CsShareInfoModel)Container.DataItem).EX_ReadRateString %>%)</td>
								</tr>
							</ItemTemplate>
							</asp:Repeater>
							<tr id="trListError" class="list_alert" runat="server" Visible="False">
								<td id="tdErrorMessage" colspan="8" runat="server"></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
				</tr>
				<tr>
					<td class="action_part_bottom"><asp:Button id="btnInsertBotttom" runat="server" Text="　新規登録　" onclick="btnInsert_Click"></asp:Button></td>
				</tr>
				<tr>
					<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
	<!--△ 一覧 △-->
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
