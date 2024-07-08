<%--
=========================================================================================================
  Module      : 集計区分設定確認ページ(SummarySettingConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Cs.SummarySetting" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="SummarySettingConfirm.aspx.cs" Inherits="Form_SummarySetting_SummarySettingConfirm" Title="" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">集計区分設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 詳細 ▽-->
	<tr id="trDetail" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">集計区分設定詳細</h2></td>
	</tr>
	<tr id="trConfirm" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">集計区分設定確認</h2></td>
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
					<asp:button id="btnEditTop" runat="server" Text="　編集する　" Visible="False" onclick="btnEdit_Click"></asp:button>
					<asp:button id="btnInsertTop" runat="server" Text="　登録する　" Visible="False" onclick="btnInsert_Click"></asp:button>
					<asp:button id="btnUpdateTop" runat="server" Text="　更新する　" Visible="False" Width="72px" onclick="btnUpdate_Click"></asp:button></div>
				<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
					<tr>
						<td class="detail_title_bg" align="center" colspan="2">基本情報</td>
					</tr>
					<tr id="trSummarySettingNo" runat="server">
						<td class="detail_title_bg" align="left" width="30%">集計区分No</td>
						<td class="detail_item_bg" align="left">
							<asp:Literal ID="lSummarySettingNo" runat="server"></asp:Literal>
						</td>
					</tr>
					<tr>
						<td class="detail_title_bg" align="left" width="30%">集計区分名</td>
						<td class="detail_item_bg" align="left">
							<asp:Literal ID="lSummarySettingTitle" runat="server"></asp:Literal>
							</td>
					</tr>
					<tr>
						<td class="detail_title_bg" align="left" width="30%">表示順</td>
						<td class="detail_item_bg" align="left">
							<asp:Literal ID="lSummarySettingDisplayOrder" runat="server"></asp:Literal>
						</td>
					</tr>
					<tr>
						<td class="detail_title_bg" align="left" width="30%">有効フラグ</td>
						<td class="detail_item_bg" align="left">
							<asp:Literal ID="lSummarySettingValidFlg" runat="server"></asp:Literal>
						</td>
					</tr>
					<tr>
						<td class="detail_title_bg" align="left" width="30%">入力タイプ</td>
						<td class="detail_item_bg" align="left">
							<asp:Literal ID="lSummarySettingSummarySettingType" runat="server"></asp:Literal>
						</td>
					</tr>
					<tr id="trDateCreated" runat="server" Visible="False">
						<td class="detail_title_bg" align="left" width="30%">作成日</td>
						<td class="detail_item_bg" align="left">
							<asp:Literal ID="lDateCreated" runat="server"></asp:Literal>
						</td>
					</tr>
					<tr id="trDateChanged" runat="server" Visible="False">
						<td class="detail_title_bg" align="left" width="30%">更新日</td>
						<td class="detail_item_bg" align="left">
							<asp:Literal ID="lDateChanged" runat="server"></asp:Literal>
						</td>
					</tr>
					<tr id="trLastChanged" runat="server" Visible="False">
						<td class="detail_title_bg" align="left" width="30%">最終更新者</td>
						<td class="detail_item_bg" align="left">
							<asp:Literal ID="lLastChanged" runat="server"></asp:Literal>
						</td>
					</tr>
				</table>
			</td>
			</tr>
			<tr>
			<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
			</tr>
			<tr>
			<td>
				<div id="divSummarySettingItems" runat="server">
				<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
					<tr>
						<td class="detail_title_bg" align="center" colspan="4">集計アイテム情報</td>
					</tr>
					<asp:Repeater id="rSummarySettingItems" runat="server">
					<HeaderTemplate>
						<tr>
							<td class="detail_title_bg" align="center" width="10%">表示順</td>
							<td class="detail_title_bg" align="left" width="25%">保存する値</td>
							<td class="detail_title_bg" align="left">表示文言</td>
							<td class="detail_title_bg" align="center" width="10%">有効フラグ</td>
						</tr>
					</HeaderTemplate>
					<ItemTemplate>
						<tr>
							<td class="detail_item_bg" align="center">
								<%# WebSanitizer.HtmlEncode(((CsSummarySettingItemModel)Container.DataItem).DisplayOrder) %>
							</td>
							<td class="detail_item_bg" align="left">
								<%# WebSanitizer.HtmlEncode(((CsSummarySettingItemModel)Container.DataItem).SummarySettingItemId) %>
							</td>
							<td class="detail_item_bg" align="left">
								<%# WebSanitizer.HtmlEncode(((CsSummarySettingItemModel)Container.DataItem).SummarySettingItemText) %>
							</td>																					
							<td class="detail_item_bg" align="center">
								<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_CSSUMMARYSETTINGITEM, Constants.FIELD_CSSUMMARYSETTINGITEM_VALID_FLG, ((CsSummarySettingItemModel)Container.DataItem).ValidFlg)) %>
							</td>
						</tr>
					</ItemTemplate>
					</asp:Repeater>
				</table>
				</div>
				<div class="action_part_bottom"><input type="button" onclick="Javascript:history.back();" value=" 　戻る 　" />
					<asp:button id="btnEditBottom" runat="server" Text="　編集する　" Visible="False" onclick="btnEdit_Click"></asp:button>
					<asp:button id="btnInsertBottom" runat="server" Text="　登録する　" Visible="False" onclick="btnInsert_Click"></asp:button>
					<asp:button id="btnUpdateBottom" runat="server" Text="　更新する　" Visible="False" Width="72px" onclick="btnUpdate_Click"></asp:button>
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
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>