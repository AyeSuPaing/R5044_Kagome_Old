<%--
=========================================================================================================
  Module      : 注文拡張項目設定ページ(OrderExtendSettingList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OrderExtendSettingList.aspx.cs" Inherits="Form_OrderExtendSetting_OrderExtendSettingList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
<tr><td><h1 class="page-title">注文拡張項目設定</h1></td></tr>
<tr><td><img height="10" width="100" border="0" alt="" src="../../Images/Common/sp.gif" /></td></tr>
<tr><td><h2 class="cmn-hed-h2">注文拡張項目設定一覧</h2></td></tr>
<tr>
<td>
<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
<tr><td>
<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
<tr>
<td align="center">
<div>
<asp:UpdatePanel runat="server">
<ContentTemplate>
<table cellspacing="0" cellpadding="0" border="0">
	<tr><td height="6" colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="6" border="0" /></td></tr>
	<tr><td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="4" border="0" /></td></tr>
	<tr runat="server" visible='<%# (string.IsNullOrEmpty(this.MessageHtml) == false) %>'>
		<td>
			<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
				<tr class="info_item_bg">
					<td align="left"><%# this.MessageHtml %></td>
				</tr>
			</table>
		</td>
	</tr>
	<tr><td height="6" colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="6" border="0" /></td></tr>
	<tr>
		<td align="right">
			<div align="left" style="float:left;margin-top:7px;"><a href="#note">&nbsp;備考へ&nbsp;</a></div>
			<asp:Button ID="btnAllUpdateTop" runat="server" Text="&nbsp;一括更新&nbsp;" OnClick="btnAllUpdate_Click" />
		</td>
		<td width="5"><img alt="" src="../../Images/Common/sp.gif" width="5" height="1" border="0" /></td>
	</tr>
	<tr><td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="4" border="0" /></td></tr>
	<tr>
		<td valign="top">
			<table class="list_table UserExtendSettingList" cellspacing="1" cellpadding="3" width="758" border="0" align="center">
				<%-- ▽項目設定一覧▽ --%>
				<asp:Repeater id="rOrderExtendSettingList" ItemType="OrderExtendSettingInput" Runat="server" DataSource='<%# this.OrderExtendSettingModel %>'>
				<ItemTemplate>
				<%-- ▽▽バリューフィールド▽▽(通常項目) --%>
				<tr style='background-color:<%# ((Container.ItemIndex % 2) == 1) ? "#FFE7D4" : "#e7e7e7"%>;'>
					<td colspan="3">
						<div align="left" style="float:left;">
							ID<%#: Item.SettingId %>
						</div>
						<div style="float:right;">
							<small>表示範囲&nbsp;：&nbsp;
							<asp:CheckBox runat="server" ID="cbPcDisplayFlg" Checked='<%# Item.DisplayKbn.Contains(Constants.FLG_ORDEREXTENDSETTING_DISPLAY_PC) %>' TextAlign="Right" Text="&nbsp;&nbsp;ＰＣ/スマフォ" />&nbsp;&nbsp;&nbsp;
							<asp:CheckBox runat="server" ID="cbEcDisplayFlg" Checked='<%# Item.DisplayKbn.Contains(Constants.FLG_ORDEREXTENDSETTING_DISPLAY_EC) %>' TextAlign="Right" Text="&nbsp;&nbsp;EC管理" />&nbsp;&nbsp;&nbsp;
							</small>
						</div>
					</td>
				</tr>
				<tr style='background-color:<%# ((Container.ItemIndex % 2) == 1) ? "#FCF0E7" : "#F8F8F8"%>;'>
					<%-- ▽▽▽左側フィールド▽▽▽ --%>
					<td align="center" width="3%" valign="top" style='background-color:<%# ((Container.ItemIndex % 2) == 1) ? "#FFE7D4" : "#e7e7e7"%>;'>
						<div style="writing-mode: tb-rl; height: 60px;width: 15px">表示順</div>
						<asp:TextBox ID="tbSortOrder" runat="server" Text='<%# Item.SortOrder.ToString() %>' Width="25" MaxLength="3"></asp:TextBox>
					</td>
					<td align="left" width="40%" valign="top">
						<div>
							項目名<span class="notice" runat="server" >&nbsp;*</span>&nbsp;：&nbsp;<asp:TextBox ID="tbOrderExtendSettingName" runat="server" Text='<%# Item.SettingName %>' Width="275px" MaxLength="100"></asp:TextBox>
							<div style="margin-top:7px;">
								項目説明&nbsp;：&nbsp;<asp:RadioButtonList ID="rblOutlineKbn" runat="server"  DataSource='<%# ValueText.GetValueItemList(Constants.TABLE_ORDEREXTENDSETTING, Constants.FIELD_ORDEREXTENDSETTING_OUTLINE_KBN) %>' RepeatDirection="Horizontal" RepeatLayout="Flow"  DataTextField="Text" DataValueField="Value"></asp:RadioButtonList>
								<span><input type="button" onclick="javascript:open_wysiwyg('<%# Container.FindControl("tbOrderExtendOutline").ClientID%>', '<%# Container.FindControl("rblOutlineKbn").ClientID%>');" value="  HTMLエディタ  " /></span><br />
								<asp:TextBox ID="tbOrderExtendOutline" runat="server"  TextMode="MultiLine" Text="<%# Item.Outline %>" Width="340px" Rows="7"></asp:TextBox>
							</div>
							<div align="left" style="float:left;margin-top:7px;">
								利用画面&nbsp;：&nbsp;<asp:RadioButtonList ID="rblInitOnlyType" runat="server"  DataSource='<%# ValueText.GetValueItemList(Constants.TABLE_ORDEREXTENDSETTING, Constants.FIELD_ORDEREXTENDSETTING_INIT_ONLY_FLG) %>' RepeatDirection="Horizontal" RepeatLayout="Flow"  DataTextField="Text" DataValueField="Value"></asp:RadioButtonList><br />
								必須チェック&nbsp;：&nbsp;<asp:CheckBox runat="server" ID="cbNecessary"  TextAlign="Right" Text="&nbsp;&nbsp;入力/選択を必須にする" />
							</div>
						</div>
					</td>
					<%-- ▽▽▽右側フィールド▽▽▽ --%>
					<td align="left" valign="top">
						<div>
							入力方法&nbsp;：&nbsp;<asp:DropDownList ID="ddlOrderExtendSettingInputType" runat="server"  DataSource='<%# ValueText.GetValueItemList(Constants.TABLE_ORDEREXTENDSETTING, Constants.FIELD_ORDEREXTENDSETTING_INPUT_TYPE) %>' DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlOrderExtendSettingInputType_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList><br />
							<%-- テキストボックスの場合 --%>
							<span id="dvInputPropertyAreaForTB" runat="server">
								<table border="0">
								<tr align="left">
									<td>
										初期値&nbsp;：&nbsp;<asp:TextBox ID="tbInputDefault_forTb" runat="server"  Text='<%# this.InputDefaultForText[((RepeaterItem)Container).ItemIndex] %>' Width="250"></asp:TextBox>
									</td>
								</tr>
								<tr align="left">
									<td>
										入力チェック種別&nbsp;：&nbsp;<asp:DropDownList ID="ddlCheckType" runat="server"  DataSource='<%# this.InputCheckTypeList %>' DataTextField="Text" DataValueField="Value" AutoPostBack="true" OnSelectedIndexChanged="ddlCheckType_SelectedIndexChanged"></asp:DropDownList>
									</td>
								</tr>
								<tr align="left">
									<td>
										<div id="dvLengthInputArea" runat="server" visible="false">
											文字数&nbsp;：&nbsp;<asp:RadioButtonList ID="rblFixedLength" runat="server" DataSource='<%# ValueText.GetValueItemList(Constants.TABLE_ORDEREXTENDSETTING, "fixedlength") %>' RepeatDirection="Horizontal" RepeatLayout="Flow"  DataTextField="Text" DataValueField="Value" AutoPostBack="true" OnSelectedIndexChanged="rblFixedLength_SelectedIndexChanged"></asp:RadioButtonList><br />
											入力文字数制限&nbsp;：&nbsp;
											<span id="dvFixedLengthInputArea" runat="server" visible="false">
												<asp:TextBox ID="tbFixedLength" runat="server" Width="50" MaxLength="5"></asp:TextBox>文字
											</span>
											<span id="dvMaxMinLengthInputArea" runat="server">
												<asp:TextBox ID="tbLengthMin" runat="server" Width="50" MaxLength="5"></asp:TextBox>文字以上&nbsp;～&nbsp;
												<asp:TextBox ID="tbLengthMax" runat="server" Width="50" MaxLength="5"></asp:TextBox>文字以下
											</span>
										</div>
										<div id="dvNumberRangeInputArea" runat="server" visible="false">
											数値範囲&nbsp;：&nbsp;
											<asp:TextBox ID="tbNumMin" runat="server" Width="50" MaxLength="5"></asp:TextBox>&nbsp;～&nbsp;
											<asp:TextBox ID="tbNumMax" runat="server" Width="50" MaxLength="5"></asp:TextBox>
										</div>
									</td>
								</tr>
								</table>
							</span>
							<%-- テキストボックス以外の場合 --%>
							<span id="dvInputPropertyAreaForOther" style="display: inline-block;max-width: 450px;" runat="server">
								<table border="0">
								<tr>
									<td colspan="2" align="left">
										表示名&nbsp;：&nbsp;<asp:TextBox ID="tbInputDefaultValue_ForOther" runat="server" Width="120"></asp:TextBox>&nbsp;&nbsp;&nbsp;
										値&nbsp;：&nbsp;<asp:TextBox ID="tbInputDefaultKey_ForOther" runat="server" Width="120"></asp:TextBox><br />
										リスト表示&nbsp;：&nbsp;<asp:CheckBox ID="cbDefault" runat="server" TextAlign="Right" Text="&nbsp;&nbsp;選択済にする（●）" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
										<asp:CheckBox ID="cbHide" runat="server" TextAlign="Right" Text="&nbsp;&nbsp;非表示にする（×）" />
									</td>
								</tr>
								<tr>
									<td colspan="2" align="right">
										<div style="float:left;">
											<asp:Label runat="server" id="lblKeyValueMessage" align="left" style="color:Red;" ></asp:Label>
										</div>
										<asp:Button ID="btnInputDefaultKeyValueAdd_ForOther" runat="server" Text="追加" OnClick="btnInputDefaultKeyValueAdd_ForOther_Click" />
										<asp:Button ID="btnInputDefaultKeyValueUpdate_ForOther" runat="server" Text="変更" OnClick="btnInputDefaultKeyValueUpdate_ForOther_Click" />
										<asp:Button ID="btnInputDefaultKeyValueDelete_ForOther" runat="server" Text="削除" OnClick="btnInputDefaultKeyValueDelete_ForOther_Click" />
									</td>
								</tr>
								<tr>
									<td>
										<asp:ListBox ID="lbInputDefaultList_ForOther" runat="server" Width="340" Rows="6" DataSource='<%# this.InputDefaultForListItem[((RepeaterItem)Container).ItemIndex] %>' DataTextField="Text" DataValueField="Value" AutoPostBack="true" OnSelectedIndexChanged="lbInputDefaultList_ForOther_SelectedIndexChanged"></asp:ListBox>
									</td>
									<td>
										<asp:Button ID="btnUp" runat="server" OnCommand="btnUpDown_OnCommand" CommandArgument="up" Text=" ↑ " /><br /><br /><br /><br />
										<asp:Button ID="btnDown" runat="server" OnCommand="btnUpDown_OnCommand" CommandArgument="down" Text=" ↓ " />
									</td>
								</tr>
								</table>
							</span>
						</div>
					</td>
				</tr>
				<%-- △△バリューフィールド△△(通常項目) --%>
				<asp:HiddenField ID="hdnSettingId" runat="server" Value='<%# Item.SettingId %>' />
				</ItemTemplate>
				</asp:Repeater>
				<%-- △項目設定一覧△ --%>
			</table>
		</td>
		<td><img alt="" src="../../Images/Common/sp.gif" width="5" height="1" border="0" /></td>
	</tr>
	<tr><td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="4" border="0" /></td></tr>
	<tr>
		<td align="right">
			<asp:Button ID="btnAllUpdateBottom" runat="server" Text="&nbsp;一括更新&nbsp;" OnClick="btnAllUpdate_Click" OnClientClick="return check_delete_fields_confirm();" />
		</td>
		<td width="5"><img alt="" src="../../Images/Common/sp.gif" width="5" height="1" border="0" /></td>
	</tr>
	<tr><td height="6" colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="6" border="0" /></td></tr>
	<tr runat="server" visible='<%# string.IsNullOrEmpty(this.MessageHtml) == false %>'>
		<td>
			<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
				<tr class="info_item_bg">
					<td align="left"><%# this.MessageHtml %></td>
				</tr>
			</table>
		</td>
	</tr>
	<tr><td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="4" border="0" /></td></tr>
</table>
</ContentTemplate>
</asp:UpdatePanel>
<%-- ▽備考▽ --%>
<table id="note" class="info_table" width="758" border="0" cellspacing="1" cellpadding="3">
	<tr>
		<td align="left" class="info_item_bg" colspan="2">備考<br />
			<dl style="margin-left:10px;">
				<dt><b>【入力項目の説明】</b></dt>
				<dt>表示範囲</dt>
				<dd style="margin-left:20px;">ＰＣ/スマフォ、EC管理への表示有無を設定できます。
					チェックをつけたサイトのみ表示します。</dd>
				<dt>表示順</dt>
				<dd style="margin-left:20px;">フロントサイトに表示される順序が設定できます。</dd>
				<dt>項目名</dt>
				<dd style="margin-left:20px;">入力項目の名称です。フロントサイトにも項目名として表示されます。</dd>
				<dt>項目説明</dt>
				<dd style="margin-left:20px;">説明を記述することができます。記述内容はフロントサイトに表示できます。<br /></dd>
				<dt>登録時のみ表示</dt>
				<dd style="margin-left:20px;">チェックをつけた場合、フロントサイトのマイページ > 履歴詳細にて非表示になります。<br />
					<b>※チェック有無にかかわらず、EC管理には表示します。</b></dd>
				<dt>必須（入力/選択）</dt>
				<dd style="margin-left:20px;">フロントサイトで入力または選択する際の必須有無を設定できます。<br /></dd>
			</dl><br />
			<dl style="margin-left:10px;">
				<dt><b>【入力方法について】</b></dt>
				<dt>テキストボックスの場合</dt>
				<dd style="margin-left:20px;">「初期値」及び「入力チェック」が設定できます。<br />
					<b>※チェック有無にかかわらず、EC管理の会員編集では入力チェックを行いません。</b></dd>
				<dt>テキストボックス以外の場合</dt>
				<dd style="margin-left:20px;">リストに表示する「表示名」「値」「デフォルト」が設定できます。<br />
					「表示名」はドロップダウンなどリストを表示する際の１要素の表示名です。<br />
					「値」はドロップダウンなどリストを表示する際の１要素の値です。<br />
					&nbsp;&nbsp;&nbsp;&nbsp;値は１拡張項目リストの内容で一意の必要があります。また、値が空の要素は、未選択状態用に１つのみ追加可能です。<br />
					「デフォルト」は画面表示時に選択済みにする設定ができます。</dd>
				<dt><br /><b>※各要素の追加/更新/削除や入力チェックの設定などは、「一括更新」ボタンを押すまで反映されないためご注意ください。</b></dt>
			</dl><br />
			<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
				<dl style="margin-left:10px;">
					<dt><b>【定期注文について】</b></dt>
					<dt>フロントでの定期購入の場合</dt>
					<dd style="margin-left:20px;">
						2回目以降の定期注文に入力内容が引き継がれます。マイページ > 定期台帳詳細より変更が可能です。<br />
					</dd>
				</dl><br />
			<% } %>
			<dl style="margin-left:10px;">
				<dt><b>【注文方法のデフォルト設定について】</b></dt>
				<dt>マイページ > 注文方法の設定にて配送先が設定されている場合</dt>
				<dd style="margin-left:20px;">
					項目が入力必須でかつ、ユーザーが未入力の場合は配送画面はスキップされずに表示されます。<br />
					デフォルトの入力値が設定されている場合は配送画面はスキップされます。
				</dd>
			</dl>
		</td>
	</tr>
</table>
<%-- △備考△ --%>
<br />
</div>
</td>
</tr>
</table>
</td></tr>
</table>
</td>
</tr>
<tr><td><img height="10" width="1" border="0" alt="" src="../../Images/Common/sp.gif" /></td></tr>
</table>
</asp:Content>
