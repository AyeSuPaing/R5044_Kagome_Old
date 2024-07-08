<%--
=========================================================================================================
  Module      : 共有情報登録ページ(ShareInfoRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Cs.CsOperator" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ShareInfoSettingRegister.aspx.cs" Inherits="Form_ShareInfoSetting_ShareInfoSettingRegister" Title="" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">共有情報</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 登録 ▽-->
	<tr id="trEdit" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">共有情報編集</h2></td>
	</tr>
	<tr id="trRegister" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">共有情報登録</h2></td>
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
	<div class="action_part_top">
		<input type="button" onclick="Javascript:history.back();" value =" 　戻る 　" />
		<asp:Button id="btnConfirmTop" runat="server" Text="　確認する　" Visible="False" onclick="btnConfirm_Click"></asp:Button>
	</div>
	<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
		<tr id="trInfoId" style="height: 30px;" Visible="false" runat="server">
			<td class="detail_title_bg" align="left" width="150">NO</td>
			<td class="detail_item_bg" align="left" colspan="5">
				<asp:Literal id="lInfoNo" runat="server"></asp:Literal>
			</td>
		</tr>
		<tr style="height: 30px;">
			<td class="detail_title_bg" align="left" width="150">区分<span class="notice">*</span></td>
			<td class="detail_item_bg" align="left" width="133">
				<asp:DropDownList id="ddlInfoKbn" runat="server" ></asp:DropDownList>
			</td>
			<td class="detail_title_bg" align="left" width="120">重要度<span class="notice">*</span></td>
			<td class="detail_item_bg" align="left" width="133">
				<asp:DropDownList id="ddlImportance" runat="server"></asp:DropDownList>
			</td>
			<td class="detail_title_bg" align="left" width="120">送信元</td>
			<td class="detail_item_bg" align="left" width="132">
				<asp:Literal id="lSenderName" runat="server"></asp:Literal>
			</td>
		</tr>
		<tr>
			<td class="detail_title_bg" align="left" width="150">共有テキスト<span class="notice">*</span></td>
			<td class="detail_item_bg" align="left" colspan="5">
				<asp:RadioButtonList ID="rblInfoTextKbn" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="radio_button_list" runat="server" onclick="show_hide_html();"></asp:RadioButtonList>
				<input type= "button" onclick="javascript:open_wysiwyg('<%= tbInfoText.ClientID %>', '<%= rblInfoTextKbn.ClientID %>');" value="  HTMLエディタ  " /><br />
				<asp:TextBox ID="tbInfoText" Runat="server" TextMode="MultiLine" Width="600" Rows="20"></asp:TextBox>
				<div id="divHtml" style="padding-top: 2px">
					■HTML入力例<br />
					<%= WebSanitizer.HtmlEncode("<font size=\"5\" color=\"#FF0000\">赤文字フォントサイズ5</font>") %><br />
					<%= WebSanitizer.HtmlEncode("<b>太文字</b>") %><br />
					<%= WebSanitizer.HtmlEncode("改行<br>") %><br />
				</div>
			</td>
		</tr>
		<tr id="trDateCreated" style="height: 30px;" visible="false" runat="server">
			<td class="detail_title_bg" align="left" width="150">作成日時</td>
			<td class="detail_item_bg" align="left" colspan="5">
				<asp:Literal id="lDateCreated" runat="server"></asp:Literal>
		</td>
		</tr>
		<tr>
			<td class="detail_title_bg" align="left" width="150">宛先オペレータ<span class="notice">*</span></td>
			<td class="detail_item_bg" align="left" colspan="5">
				<table class="detail_table" cellspacing="1" cellpadding="3" border="0" width="100%">
					<tr>
						<td class="detail_title_bg" align="center" width="10"></td>
						<td class="detail_item_bg" align="left" colspan="3">
							全てのオペレータ
							&nbsp;（&nbsp;<a href="javascript:;" onclick="check_all_operator(true);">全てをチェック</a>
							&nbsp;/&nbsp;<a href="javascript:;" onclick="check_all_operator(false);">全てを解除</a>&nbsp;）
						</td>
					</tr>
					<asp:Repeater id="rCsOperators" Runat="server">
					<ItemTemplate>
						<tr>
							<td class="detail_title_bg" width="10"></td>
							<td class="detail_title_bg" align="center" width="20">
								<asp:HiddenField ID="hfOperatorId" Value="<%# ((CsOperatorModel)Container.DataItem).OperatorId %>" runat="server" />
								<asp:CheckBox id="cbOperator" Runat="server"></asp:CheckBox>
							</td>
							<td class="detail_item_bg" align="left" width="525" colspan="2">
								&nbsp;<%# WebSanitizer.HtmlEncode(((CsOperatorModel)Container.DataItem).EX_ShopOperatorName) %>
								<asp:HiddenField ID="hfOperatorName" Value="<%# ((CsOperatorModel)Container.DataItem).EX_ShopOperatorName %>" runat="server" />
							</td>
						</tr>
					</ItemTemplate>
					</asp:Repeater>
					<tr>
						<td class="detail_title_bg" align="center" width="10"></td>
						<td class="detail_item_bg" align="left" colspan="3">全てのグループ&nbsp;（&nbsp;<a href="javascript:;" onclick="check_all_group(true);">全てをチェック</a>&nbsp;/&nbsp;<a href="javascript:;" onclick="check_all_group(false);">全てを解除</a>&nbsp;）
						</td>
					</tr>
					<asp:Repeater id="rCsGroups" Runat="server">
					<ItemTemplate>
						<tr>
							<td class="detail_title_bg" width="10"></td>
							<td class="detail_title_bg" align="center" width="10"></td>
							<td class="detail_item_bg" align="left" colspan="2">
								&nbsp;<%# WebSanitizer.HtmlEncode(((CsGroupModel)Container.DataItem).CsGroupName) %>
								&nbsp;（&nbsp;<a href="javascript:;" onclick="check_group(true, <%# Container.ItemIndex %>, <%# ((CsGroupModel)Container.DataItem).Ex_Operators.Length %>);">グループ内の全てをチェック</a>
								&nbsp;/&nbsp;<a href="javascript:;" onclick="check_group(false, <%# Container.ItemIndex %>, <%# ((CsGroupModel)Container.DataItem).Ex_Operators.Length %>);">グループ内の全てを解除</a>&nbsp;）
							</td>
						</tr>
						<asp:Repeater id="rCsOperators" Runat="server" DataSource="<%# ((CsGroupModel)Container.DataItem).Ex_Operators %>">
						<ItemTemplate>
							<tr>
								<td class="detail_title_bg" width="10"></td>
								<td class="detail_title_bg" width="10"></td>
								<td class="detail_title_bg" align="center" width="20">
									<asp:HiddenField ID="hfOperatorId" Value="<%# ((CsOperatorModel)Container.DataItem).OperatorId %>" runat="server" />
									<asp:CheckBox id="cbOperator" runat="server"></asp:CheckBox>
								</td>
								<td class="detail_item_bg" align="left" width="525">
									&nbsp;<%# WebSanitizer.HtmlEncode(((CsOperatorModel)Container.DataItem).EX_ShopOperatorName) %>
									<asp:HiddenField ID="hfOperatorName" Value="<%# ((CsOperatorModel)Container.DataItem).EX_ShopOperatorName %>" runat="server" />
								</td>
							</tr>
						</ItemTemplate>
						</asp:Repeater>
					</ItemTemplate>
					</asp:Repeater>
				</table>
			</td>
		</tr>
	</table>
	<div class="action_part_bottom">
		<input type="button" onclick="Javascript:history.back();" value =" 　戻る 　" />
		<asp:Button id="btnConfirmBottom" runat="server" Text="　確認する　" Visible="False" onclick="btnConfirm_Click"></asp:Button>
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
	<!--△ 登録 △-->
	<tr>
	<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<script type="text/javascript">
<!--				
// 全グループチェック・解除
function check_all_group(chk)
{
<%for (int iLoop = 0; iLoop < rCsGroups.Items.Count; iLoop++) { %>
<%	var r = (Repeater)rCsGroups.Items[iLoop].FindControl("rCsOperators"); %>
<%	for (int iLoop2 = 0; iLoop2 < r.Items.Count; iLoop2++) { %>
		document.getElementById('<% Response.Write(((CheckBox)r.Items[iLoop2].FindControl("cbOperator")).ClientID); %>').checked = chk;
<%	} %>
<%} %>
}
// グループチェック・解除
function check_group(chk, group_index, operator_count)
{
<% for (int iLoop = 0; iLoop < rCsGroups.Items.Count; iLoop++) { %>
<%	var r = (Repeater)rCsGroups.Items[iLoop].FindControl("rCsOperators"); %>
	if (group_index == <% Response.Write(iLoop); %>)
	{
<%		for (int iLoop2 = 0; iLoop2 < r.Items.Count; iLoop2++) { %>
			document.getElementById('<% Response.Write(((CheckBox)r.Items[iLoop2].FindControl("cbOperator")).ClientID); %>').checked = chk;
<%		} %>
	} 
<%} %>
}
// 全オペレータチェック・解除
function check_all_operator(chk)
{
<%for (int iLoop = 0; iLoop < rCsOperators.Items.Count; iLoop++) {%>
	document.getElementById('<% Response.Write(((CheckBox)rCsOperators.Items[iLoop].FindControl("cbOperator")).ClientID); %>').checked = chk;
<%} %>
}

// 表示または非表示の HTML 入力の例
function show_hide_html()
{
	var checkHtml = $('#<% = rblInfoTextKbn.ClientID %> input[type=radio]:checked').val() == <% = Constants.FLG_CSSHAREINFO_INFO_TEXT_KBN_HTML %>;
	document.getElementById("divHtml").style.display = checkHtml ? "block" : "none";
}

// ページを読み込み
$(document).ready(function()
{
	show_hide_html();
});
//-->
</script>
</asp:Content>