<%--
=========================================================================================================
  Module      : 集計区分設定一覧ページ(SummarySettingList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Cs.SummarySetting" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="SummarySettingList.aspx.cs" Inherits="Form_SummarySetting_SummarySettingList" Title="" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">集計区分設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">集計区分設定一覧</h2></td>
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
					<td class="action_part_top" style="height: 33px">
						利用項目数 <%= this.TotalCount %> （最大 <%= Constants.MAX_SUMMARY_SETTING_COUNT %>）
						<asp:Button id="btnInsertTop" runat="server" Text="　新規登録　" onclick="btnInsert_Click"></asp:Button></td>
				</tr>
				<tr>
					<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
				</tr>
				<tr>
				<td>
					<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
						<tr class="list_title_bg">
							<td align="center" width="80">集計区分No</td>
							<td align="center" width="310">集計区分名</td>
							<td align="center" width="122">入力タイプ</td>
							<td align="center" width="82">登録項目数</td>
							<td align="center" width="82">表示順</td>
							<td align="center" width="82">有効フラグ</td>
						</tr>
						<asp:repeater id="rList" Runat="server">
						<ItemTemplate>
							<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# CreateDetailUrl(((CsSummarySettingModel)Container.DataItem).SummarySettingNo) %>')">
								<td align="center"><%# WebSanitizer.HtmlEncode(((CsSummarySettingModel)Container.DataItem).SummarySettingNo) %></td>
								<td align="left">&nbsp;<%# WebSanitizer.HtmlEncode(((CsSummarySettingModel)Container.DataItem).SummarySettingTitle) %></td>
								<td align="center"><%# WebSanitizer.HtmlEncode(((CsSummarySettingModel)Container.DataItem).EX_SummarySettingTypeText) %></td>
								<td align="center"><%# WebSanitizer.HtmlEncode(((CsSummarySettingModel)Container.DataItem).EX_ItemCountText) %></td>
								<td align="center"><%# WebSanitizer.HtmlEncode(((CsSummarySettingModel)Container.DataItem).DisplayOrder) %></td>
								<td align="center"><%# WebSanitizer.HtmlEncode(((CsSummarySettingModel)Container.DataItem).EX_ValidFlgText) %></td>
							</tr>
						</ItemTemplate>
						</asp:repeater>
						<tr id="trListError" class="list_alert" runat="server" Visible="False">
							<td colspan="6" runat="server">
								<asp:Literal ID="lErrorMessages" runat="server"></asp:Literal>
							</td>
						</tr>
					</table>
				</td>
				</tr>
				<tr>
					<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
				</tr>
				<tr>
				<td>
					<table class="info_table" width="758" border="0" cellspacing="1" cellpadding="3">
					<tr class="info_item_bg">
						<td align="left">備考<br />
							・集計区分は <b>最大<%= Constants.MAX_SUMMARY_SETTING_COUNT %>件まで</b> 利用可能です。<br />
							・利用しなくなったものは無効にし、再利用は行わないで下さい。
						</td>
					</tr>
					</table>
				</td>
				</tr>
				<tr>
					<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
				</tr>
				<tr>
					<td class="action_part_bottom">
						利用項目数 <%= this.TotalCount %> （最大 <%= Constants.MAX_SUMMARY_SETTING_COUNT %>）
						<asp:Button id="btnInsertBotttom" runat="server" Text="　新規登録　" onclick="btnInsert_Click"></asp:Button></td>
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

</asp:Content>