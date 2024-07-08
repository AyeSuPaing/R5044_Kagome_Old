<%--
=========================================================================================================
  Module      : ユーザ拡張項目設定ページ(UserExtendSettingList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="UserExtendSettingList.aspx.cs" Inherits="Form_UserExtendSetting_UserExtendSettingList" %>
<%@ Import Namespace="w2.Domain.UserExtendSetting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
<tr><td><h1 class="page-title">ユーザー拡張項目設定</h1></td></tr>
<tr><td><img height="10" width="100" border="0" alt="" src="../../Images/Common/sp.gif" /></td></tr>
<tr><td><h2 class="cmn-hed-h2">ユーザー拡張項目設定一覧</h2></td></tr>
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
			<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
			<div align="left" style="float:left;margin-top:7px; margin-left:5px">
				<asp:LinkButton id="lbExportTranslationData" Runat="server" OnClick="lbExportTranslationData_Click">&nbsp;翻訳データ出力&nbsp;</asp:LinkButton></div>
			<% } %>
			システム利用分を除く利用項目数&nbsp;<%#: this.UserExtendSettingItemsCountWithoutSystemUsed %>&nbsp;（最大&nbsp;<%#: Constants.USEREXTENDSETTING_MAXCOUNT %>）&nbsp;
			<asp:Button ID="btnAddTop" runat="server" Text="&nbsp;追加&nbsp;" OnClick="btnAdd_Click" />&nbsp;
			<asp:Button ID="btnAllUpdateTop" runat="server" Text="&nbsp;一括更新&nbsp;" OnClick="btnAllUpdate_Click" OnClientClick="return check_delete_fields_confirm();" />
		</td>
		<td width="5"><img alt="" src="../../Images/Common/sp.gif" width="5" height="1" border="0" /></td>
	</tr>
	<tr><td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="4" border="0" /></td></tr>
	<tr>
		<td valign="top">
			<table class="list_table UserExtendSettingList" cellspacing="1" cellpadding="3" width="758" border="0" align="center">
				<%-- ▽ユーザ拡張項目設定一覧▽ --%>
				<asp:Repeater id="rUserExtendSettingList" ItemType="w2.Domain.UserExtendSetting.UserExtendSettingModel" Runat="server" DataSource='<%# this.UserExtendSettingList %>'>
				<ItemTemplate>
				<%-- ▽▽バリューフィールド▽▽(通常項目) --%>
				<tr style='background-color:<%# ((Container.ItemIndex % 2) == 1) ? "#FFE7D4" : "#e7e7e7"%>;'>
					<td colspan="3">
						<div align="left" style="float:left;">
							ID<span class="notice" runat="server" Visible="<%# (Item.IsRegisted == false) %>">&nbsp;*</span>
							&nbsp;：&nbsp;<span runat="server" Visible="<%# (Item.IsRegisted == false) %>"><%#: Constants.FLG_USEREXTENDSETTING_PREFIX_KEY %>&nbsp;+&nbsp;</span>
							<asp:TextBox ID="tbUserExtendSettingId" runat="server" Text='<%# (Item.SettingId.IndexOf(Constants.FLG_USEREXTENDSETTING_PREFIX_KEY) == 0) ? (Item.SettingId.Remove(0, Constants.FLG_USEREXTENDSETTING_PREFIX_KEY.Length)) : (Item.SettingId) %>' Width="120" Visible='<%# (Item.IsRegisted == false) %>' MaxLength="34"></asp:TextBox>
							<asp:Label ID="lbUserExtendSettingId" runat="server" Text='<%#: Item.SettingId %>'  Visible='<%# Item.IsRegisted %>'></asp:Label>
							<span runat="server" Visible="<%# (this.IsSystemUsedUserExtendSetting(Item.SettingId)) %>">&nbsp;&nbsp;※この項目はシステム利用項目のため編集できません</span>
							<span runat="server" Visible="<%# (this.IsNotDeleteUserExtendSetting(Item.SettingId)) %>">&nbsp;&nbsp;※この項目はシステム利用項目のため削除できません</span>
						</div>
						<div style="float:right;">
							<small>表示範囲&nbsp;：&nbsp;
							<asp:CheckBox runat="server" ID="cbPcDisplayFlg" Checked='<%# Item.DisplayKbn.Contains(Constants.FLG_USEREXTENDSETTING_DISPLAY_PC) %>' TextAlign="Right" Text="&nbsp;&nbsp;ＰＣ/スマフォ" />&nbsp;&nbsp;&nbsp;
							<asp:CheckBox runat="server" ID="cbEcDisplayFlg" Checked='<%# Item.DisplayKbn.Contains(Constants.FLG_USEREXTENDSETTING_DISPLAY_EC) %>' TextAlign="Right" Text="&nbsp;&nbsp;EC管理" />&nbsp;&nbsp;&nbsp;
							<asp:CheckBox runat="server" ID="cbCsDisplayFlg" Checked='<%# Item.DisplayKbn.Contains(Constants.FLG_USEREXTENDSETTING_DISPLAY_CS) %>' TextAlign="Right" Text="&nbsp;&nbsp;CS管理&nbsp;&nbsp;&nbsp;" Visible="<%# Constants.CS_OPTION_ENABLED %>" />
							&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
							｜&nbsp;<asp:CheckBox ID="cbDelete" runat="server" Enabled="<%#(this.IsSystemUsedUserExtendSetting(Item.SettingId) == false)&&(this.IsNotDeleteUserExtendSetting(Item.SettingId) == false) %>" TextAlign="Right" Text="&nbsp;&nbsp;削除" Checked='<%# Item.DeleteFlg %>' />
							</small>
						</div>
					</td>
				</tr>
				<tr style='background-color:<%# ((Container.ItemIndex % 2) == 1) ? "#FCF0E7" : "#F8F8F8"%>;'>
					<%-- ▽▽▽左側フィールド▽▽▽ --%>
					<td align="center" width="3%" valign="top" style='background-color:<%# ((Container.ItemIndex % 2) == 1) ? "#FFE7D4" : "#e7e7e7"%>;'>
						<div style="writing-mode: tb-rl; height: 60px;width: 15px">表示順</div>
						<asp:TextBox ID="tbSortOrder" runat="server" Text='<%# (Item.SortOrder > 999) ? string.Empty : Item.SortOrder.ToString() %>' Width="25" MaxLength="3"></asp:TextBox>
					</td>
					<td align="left" width="40%" valign="top">
						<div>
							項目名<span class="notice" runat="server" Visible="<%# (this.IsSystemUsedUserExtendSetting(Item.SettingId) == false) %>">&nbsp;*</span>&nbsp;：&nbsp;<asp:TextBox ID="tbUserExtendSettingName" runat="server" Enabled="<%#(this.IsSystemUsedUserExtendSetting(Item.SettingId) == false) %>" Text='<%# Item.SettingName %>' Width="275px" MaxLength="100"></asp:TextBox>
							<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
							<%-- 項目名翻訳設定情報 --%>
							<asp:Repeater ID="rTranslationSettingName" runat="server" 
								ItemType="w2.Domain.NameTranslationSetting.Helper.UserExtendSettingTranslation"
								DataSource="<%# (this.UserExtendSettingTranslationData.FirstOrDefault(d => d.MasterId1 == Item.SettingId) != null)
									? this.UserExtendSettingTranslationData.FirstOrDefault(d => (d.MasterId1 == Item.SettingId)).SettingNameTranslationList
									: null %>">
							<HeaderTemplate>
								<div>翻訳情報(項目名)</div>
							</HeaderTemplate>
							<ItemTemplate>
								<div>　<%#: Item.LanguageCode %>(<%#: Item.LanguageLocaleId %>)：<%#: Item.SettingName %></div>
							</ItemTemplate>
							</asp:Repeater>
							<% } %>
							<div style="margin-top:7px;">
								項目説明&nbsp;：&nbsp;<asp:RadioButtonList ID="rblOutlineKbn" runat="server" Enabled="<%#(this.IsSystemUsedUserExtendSetting(Item.SettingId) == false) %>" DataSource='<%# ValueText.GetValueItemList(Constants.TABLE_USEREXTENDSETTING, Constants.FIELD_USEREXTENDSETTING_OUTLINE_KBN) %>' RepeatDirection="Horizontal" RepeatLayout="Flow"  DataTextField="Text" DataValueField="Value"></asp:RadioButtonList>
								<span runat="server" visible="<%#(this.IsSystemUsedUserExtendSetting(Item.SettingId) == false) %>"><input type="button" onclick="javascript:open_wysiwyg('<%# Container.FindControl("tbUserExtendOutline").ClientID%>', '<%# Container.FindControl("rblOutlineKbn").ClientID%>');" value="  HTMLエディタ  " /></span><br />
								<asp:TextBox ID="tbUserExtendOutline" runat="server" Enabled="<%#(this.IsSystemUsedUserExtendSetting(Item.SettingId) == false) %>" TextMode="MultiLine" Text="<%# Item.Outline %>" Width="340px" Rows="7"></asp:TextBox>
								<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
									<%-- 概要翻訳設定情報 --%>
									<asp:Repeater id="rTranslationOutline" runat="server"
										ItemType="w2.Domain.NameTranslationSetting.Helper.UserExtendSettingTranslation"
										DataSource="<%# (this.UserExtendSettingTranslationData.FirstOrDefault(d => d.MasterId1 == Item.SettingId) != null)
											? this.UserExtendSettingTranslationData.FirstOrDefault(d => d.MasterId1 == Item.SettingId).OutlineTranslationList
											: null %>">
										<HeaderTemplate>
											<div>翻訳情報(項目説明)</div>
										</HeaderTemplate>
										<ItemTemplate>
											<div>　<%#: Item.LanguageCode %>(<%#: Item.LanguageLocaleId %>)：<%#: Item.Outline %></div>
										</ItemTemplate>
									</asp:Repeater>
								<% } %>
							</div>
							<div align="left" style="float:left;margin-top:7px;">
								利用画面&nbsp;：&nbsp;<asp:RadioButtonList ID="rblInitOnlyType" runat="server" Enabled="<%#(this.IsSystemUsedUserExtendSetting(Item.SettingId) == false) %>" DataSource='<%# ValueText.GetValueItemList(Constants.TABLE_USEREXTENDSETTING, Constants.FIELD_USEREXTENDSETTING_INIT_ONLY_FLG) %>' RepeatDirection="Horizontal" RepeatLayout="Flow"  DataTextField="Text" DataValueField="Value"></asp:RadioButtonList><br />
								必須チェック&nbsp;：&nbsp;<asp:CheckBox runat="server" ID="cbNecessary" Enabled="<%#(this.IsSystemUsedUserExtendSetting(Item.SettingId) == false) %>" TextAlign="Right" Text="&nbsp;&nbsp;入力/選択を必須にする" />
							</div>
						</div>
					</td>
					<%-- ▽▽▽右側フィールド▽▽▽ --%>
					<td align="left" valign="top">
						<div>
							入力方法&nbsp;：&nbsp;<asp:DropDownList ID="ddlUserExtendSettingInputType" runat="server" Enabled="<%#(this.IsSystemUsedUserExtendSetting(Item.SettingId) == false) %>" DataSource='<%# ValueText.GetValueItemList(Constants.TABLE_USEREXTENDSETTING, Constants.FIELD_USEREXTENDSETTING_INPUT_TYPE) %>' DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlUserExtendSettingInputType_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList><br />
							<%-- テキストボックスの場合 --%>
							<span id="dvInputPropertyAreaForTB" runat="server">
								<table border="0">
								<tr align="left">
									<td>
										初期値&nbsp;：&nbsp;<asp:TextBox ID="tbInputDefault_forTb" runat="server" Enabled="<%#(this.IsSystemUsedUserExtendSetting(Item.SettingId) == false) %>" Text='<%# this.InputDefaultForText[((RepeaterItem)Container).ItemIndex] %>' Width="250"></asp:TextBox>
									</td>
								</tr>
								<tr align="left">
									<td>
										入力チェック種別&nbsp;：&nbsp;<asp:DropDownList ID="ddlCheckType" runat="server" Enabled="<%#(this.IsSystemUsedUserExtendSetting(Item.SettingId) == false) %>" DataSource='<%# this.InputCheckTypeList %>' DataTextField="Text" DataValueField="Value" AutoPostBack="true" OnSelectedIndexChanged="ddlCheckType_SelectedIndexChanged"></asp:DropDownList>
									</td>
								</tr>
								<tr align="left">
									<td>
										<div id="dvLengthInputArea" runat="server" visible="false">
											文字数&nbsp;：&nbsp;<asp:RadioButtonList ID="rblFixedLength" runat="server" DataSource='<%# ValueText.GetValueItemList(Constants.TABLE_USEREXTENDSETTING, "fixedlength") %>' RepeatDirection="Horizontal" RepeatLayout="Flow"  DataTextField="Text" DataValueField="Value" AutoPostBack="true" OnSelectedIndexChanged="rblFixedLength_SelectedIndexChanged"></asp:RadioButtonList><br />
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
							<span id="dvInputPropertyAreaForOther" runat="server">
								<table border="0">
								<tr>
									<td colspan="2" align="left">
										表示名&nbsp;：&nbsp;<asp:TextBox ID="tbInputDefaultValue_ForOther" runat="server" Width="120" />&nbsp;&nbsp;&nbsp;
										値&nbsp;：&nbsp;<asp:TextBox ID="tbInputDefaultKey_ForOther" runat="server" Width="120" /><br />
										リスト表示&nbsp;：&nbsp;<asp:CheckBox ID="cbDefault" runat="server" TextAlign="Right" Text="&nbsp;&nbsp;選択済にする（●）" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
										<asp:CheckBox ID="cbHide" runat="server" TextAlign="Right" Text="&nbsp;&nbsp;非表示にする（×）" />
									</td>
								</tr>
								<tr>
									<td colspan="2" align="right" style="white-space: nowrap">
										<div style="float:left;">
											<asp:Label runat="server" id="lblKeyValueMessage" align="left" style="color:Red;" />
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
				<%-- △ユーザ拡張項目設定一覧△ --%>
			</table>
		</td>
		<td><img alt="" src="../../Images/Common/sp.gif" width="5" height="1" border="0" /></td>
	</tr>
	<tr><td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="4" border="0" /></td></tr>
	<tr>
		<td align="right">
			システム利用分を除く利用項目数&nbsp;<%#: this.UserExtendSettingItemsCountWithoutSystemUsed %>&nbsp;（最大&nbsp;<%#: Constants.USEREXTENDSETTING_MAXCOUNT %>）&nbsp;
			<asp:Button ID="btnAddBottom" runat="server" Text="&nbsp;追加&nbsp;" OnClick="btnAdd_Click" />&nbsp;
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
				<dt>ID</dt>
				<dd style="margin-left:20px;">ユーザ拡張項目設定の一意のIDです。重複しないように設定してください。</dd>
				<dt>表示範囲</dt>
				<dd style="margin-left:20px;">ＰＣ/スマフォ、EC管理への表示有無を設定できます。
					チェックをつけたサイトのみ表示します。</dd>
				<dt>削除</dt>
				<dd style="margin-left:20px;">チェックをつけて「一括更新」ボタンを押すことで、項目を削除できます。</dd>
				<dt>表示順</dt>
				<dd style="margin-left:20px;">フロントサイトに表示される順序が設定できます。</dd>
				<dt>項目名</dt>
				<dd style="margin-left:20px;">入力項目の名称です。フロントサイトにも項目名として表示されます。</dd>
				<dt>項目説明</dt>
				<dd style="margin-left:20px;">説明を記述することができます。記述内容はフロントサイトに表示できます。<br />
					<b>※EC管理の会員編集には表示されません。</b></dd>
				<dt>会員登録時のみ表示</dt>
				<dd style="margin-left:20px;">チェックをつけた場合、フロントサイトの会員情報変更の場合に非表示になります。<br />
					<b>※チェック有無にかかわらず、EC管理の会員編集には表示します。</b></dd>
				<dt>必須（入力/選択）</dt>
				<dd style="margin-left:20px;">フロントサイトで入力または選択する際の必須有無を設定できます。<br />
					<b>※チェック有無にかかわらず、EC管理の会員編集には必須はかかりません。</b></dd>
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
<script type="text/javascript">
<!--
	//------------------------------------------------------
	// ページ開く/閉じる
	//------------------------------------------------------
	function DisplayPage(tagid, hdntagid)
	{
		// TODO: 開く/閉じる更新状態保持用HIDDEN更新
	}

	//------------------------------------------------------
	// 更新確認Confirmダイアログ生成（削除するフィールドがあれば番号を出力する）
	//------------------------------------------------------
	function check_delete_fields_confirm()
	{
		// 追加更新削除の際に最終確認ダイアログを表示する処理
		var deleteIds = "";
		<%for (int index = 0; index < rUserExtendSettingList.Items.Count; index++) { %>
			deleteIds += ((document.getElementById('<%= rUserExtendSettingList.Items[index].FindControl("cbDelete").ClientID %>') != null) 
				&& (document.getElementById('<%= rUserExtendSettingList.Items[index].FindControl("cbDelete").ClientID %>').checked)) ? 
					(((deleteIds.length != 0) ? ", " : "") + '<%= ((Label)rUserExtendSettingList.Items[index].FindControl("lbUserExtendSettingId")).Text %>') : "";
		<%} %>
		return confirm((deleteIds.length != 0) ? '削除にチェックした項目ID(' + deleteIds + ')はユーザに紐付くユーザ拡張項目の情報も削除します。\n復元は行えない為、マスタ出力でデータの退避をお勧めします。\n削除してもよろしいですか？' : '表示内容で更新します。\nよろしいですか？');
	}
//-->
</script>
</asp:Content>
