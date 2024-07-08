<%--
=========================================================================================================
  Module      : 集計区分設定登録ページ(SummarySettingRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Cs.SummarySetting" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="SummarySettingRegister.aspx.cs" Inherits="Form_SummarySetting_SummarySettingRegister" Title="" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">問合せ集計区分設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 登録 ▽-->
	<tr id="trEdit" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">集計区分設定編集</h2></td>
	</tr>
	<tr id="trRegister" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">集計区分設定登録</h2></td>
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
	<tr><td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
	<tr>
	<td>
		<div class="action_part_top">
			<input onclick="Javascript:history.back();" type="button" value=" 　戻る 　" />
			<asp:button id="btnConfirmTop" runat="server" Text="　確認する　" onclick="btnConfirm_Click"></asp:button>
		</div>
		<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
		<tr>
			<td class="edit_title_bg" align="center" colspan="2">基本情報</td>
		</tr>
		<tr id="trSummarySettingNo" runat="server" Visible="False">
			<td class="edit_title_bg" align="left" width="30%">集計区分No</td>
			<td class="edit_item_bg" align="left">
				<asp:Literal ID="lSummarySettingNo" runat="server"></asp:Literal>
				<asp:HiddenField ID="hfSummarySettingNo" runat="server" />
			</td>
		</tr>
		<tr>
			<td class="edit_title_bg" align="left" width="30%">集計区分名<br />（対応画面に表示されます。）<span class="notice">*</span></td>
			<td class="edit_item_bg" align="left">
				<asp:TextBox id="tbSummarySettingTitle" runat="server" Width="240" MaxLength="30"></asp:TextBox></td>
		</tr>
		<tr>
			<td class="edit_title_bg" align="left" width="30%">表示順<span class="notice">*</span></td>
			<td class="edit_item_bg" align="left">
				<asp:DropDownList id="ddlDisplayOrder" runat="server"></asp:DropDownList></td>
		</tr>
		<tr>
			<td class="edit_title_bg" align="left" width="30%">有効フラグ</td>
			<td class="edit_item_bg" align="left">
				<asp:CheckBox id="cbValidFlg" Runat="server" Text="有効" Checked="True" /></td>
		</tr>
		<tr>
			<td class="edit_title_bg" align="left" width="30%">入力タイプ<span class="notice">*</span></td>
			<td class="edit_item_bg" align="left">
				<asp:RadioButtonList id="rblSummarySettingType" Runat="server" RepeatDirection="Horizontal" AutoPostBack="True" CssClass="radio_button_list" onselectedindexchanged="rblSummarySettingType_SelectedIndexChanged" RepeatLayout="Flow"></asp:RadioButtonList></td>
		</tr>
		</table>
	</td>
	</tr>
	<tr><td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
	<tr>
	<td>
		<div id="divSummarySettingItems" runat="server">
		<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
		<tr>
			<td class="edit_title_bg" align="center" colspan="6" style="HEIGHT: 24px">集計アイテム情報</td>
		</tr>
		<asp:repeater id="rSummarySettingItems" Runat="server">
		<HeaderTemplate>
			<tr class="edit_title_bg">
				<td align="center" width="31%"></td>
				<td align="center" width="10%">表示順<span class="notice">*</span></td>
				<td align="center" width="15%">保存する値<span class="notice">*</span></td>
				<td align="center" width="15%">表示文言<span class="notice">*</span></td>
				<td align="center" width="10%">有効フラグ</td>
				<td align="center" width="19%">
					<asp:Button id="btnAddItem" runat="server" Text="　追加　" OnClick="btnAddItem_Click"></asp:Button>
				</td>
			</tr>
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
				<td class="edit_title_bg" align="left"></td>
				<td class="edit_item_bg" align="center">
					<asp:DropDownList ID="ddlDisplayOrderItem" Runat="server" DataValueField="Value" DataTextField="Text" DataSource='<%# this.DispOrderListItems %>' SelectedValue='<%# ((CsSummarySettingItemModel)Container.DataItem).DisplayOrder %>'></asp:DropDownList>
				</td>
				<td class="edit_item_bg" align="left">
					<asp:TextBox ID="tbSummarySettingItemId" Runat="server" Text='<%# ((CsSummarySettingItemModel)Container.DataItem).SummarySettingItemId %>' Width="120" MaxLength="10"></asp:TextBox>
				</td>
				<td class="edit_item_bg" align="left">
					<asp:TextBox ID="tbSummarySettingItemText" Runat="server" Text='<%# ((CsSummarySettingItemModel)Container.DataItem).SummarySettingItemText %>' Width="120" MaxLength="10">
					</asp:TextBox>
				</td>
				<td class="edit_item_bg" align="center">
					<asp:CheckBox ID="chklValidFlg" Runat="server" Checked='<%# ((CsSummarySettingItemModel)Container.DataItem).ValidFlg == Constants.FLG_CSSUMMARYSETTINGITEM_VALID_FLG_VALID %>'></asp:CheckBox>
				</td>
				<td class="edit_item_bg" align="center">
					<asp:Button id="btnDeleteItem" runat="server" Text="　削除　" OnClick="btnDeleteItem_Click" CommandArgument="<%# Container.ItemIndex %>" Visible='<%# ((CsSummarySettingItemModel[])rSummarySettingItems.DataSource).Length > 1 %>'></asp:Button>
				</td>
			</tr>
		</ItemTemplate>
		</asp:repeater>
		</table>
		<br />
		<table class="info_table" width="758" border="0" cellspacing="1" cellpadding="3">
		<tr class="info_item_bg">
			<td align="left">■集計アイテム情報について<br />
				集計アイテム情報を編集する際に、以前のお問合せで登録された集計アイテム情報が既に情報として存在する場合、<br />
				「保存する値」が重複または再利用すると集計結果に不整合が発生する可能性があるので使い回しは行わないで下さい。<br />
				<br />
				例１）<br />
				「保存する値：1　表示文言：○○商品」をはじめ利用していたが、○○商品が販売を停止したため、<br />
				「保存する値：1　表示文言：△△商品」として変更すると、データ上○○商品は△△商品として集計されてしまいます。<br />
				例２）<br />
				はじめ利用していた値を変更した際、他の項目と重複した値を指定してしまうと、データ上それぞれの区別が出来ず集計されてしまいます。<br />
				例３）<br />
				はじめ利用していた値を削除した際、その項目についての集計は取得出来なくなってしまうので削除には十分ご注意下さい。<br />
				もう利用しなくなった場合は「有効フラグ」をオフにしておけば入力項目には表示されません。
			</td>
		</tr>
		</table>
		</div>
		<div class="action_part_bottom">
			<input onclick="Javascript:history.back();" type="button" value=" 　戻る 　" />
			<asp:button id="btnConfirmBottom" runat="server" Text="　確認する　" onclick="btnConfirm_Click"></asp:button></div>
	</td>
	</tr>
	<tr><td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
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
	<!--△ 登録 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>