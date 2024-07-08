<%--
=========================================================================================================
  Module      : 商品タグ設定ページ処理(ProductTagSettingList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ProductTagSettingList.aspx.cs" Inherits="Form_ProductTagSetting_ProductTagSettingList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
<tr><td><h1 class="page-title">商品タグ設定</h1></td></tr>
<tr><td><img height="10" width="100" border="0" alt="" src="../../Images/Common/sp.gif" /></td></tr>
<tr><td><h2 class="cmn-hed-h2">商品タグ設定一覧</h2></td></tr>
<tr>
<td>
<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
<tr>
<td>
<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
<tr>
<td align="center">
<div>
<%-- UPDATE PANEL開始 --%>
<asp:UpdatePanel ID="upUpdatePanel" runat="server">
<ContentTemplate>
<table cellspacing="0" cellpadding="0" border="0">
	<tr>
		<td height="6" colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="6" border="0" /></td>
	</tr>
	<tr>
		<td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="4" border="0" /></td>
	</tr>
	<div id ="dvTagComplete" runat="server" visible="false">
		<tr>
			<td>
				<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
					<tr class="info_item_bg">
						<td align="left">商品タグ設定項目を登録/更新しました。</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td height="6" colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="6" border="0" /></td>
		</tr>
	</div>
	<tr>
		<td align="right">
			<asp:Button ID="btnAddTop" runat="server" Text="&nbsp;追加&nbsp;" OnClick="btnAdd_Click" />&nbsp;
			<asp:Button ID="btnAllUpdateTop" runat="server" Text="&nbsp;一括更新&nbsp;" OnClick="btnAllUpdate_Click" OnClientClick="return check_delete_fields_confirm();" />
		</td>
		<td width="5"><img alt="" src="../../Images/Common/sp.gif" width="5" height="1" border="0" /></td>
	</tr>
	<tr>
		<td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="4" border="0" /></td>
	</tr>
	<tr>
		<td valign="top">
			<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0" align="center">
					<tr class="list_title_bg">
						<td align="center" colspan="5">商品タグ設定項目&nbsp;(<a href="#note">備考</a>)</td>
					</tr>
					<tr class="list_title_bg">
						<td width="30px" align="center">表示</td>
						<td width="38px" align="center">No</td>
						<td width="240px" align="center">タグID</td>
						<td width="380px" align="center">タグ名称</td>
						<td width="70px" align="center">削除</td>
					</tr>
					<asp:Repeater id="rTagList" Runat="server">
					<ItemTemplate>
						<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
							<td align="center"><asp:CheckBox id="cbValidFlg" runat="server" Checked="<%# (string)(((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCTTAGSETTING_TAG_VALID_FLG]) != Constants.FLG_PRODUCTTAGSETTING_VALID_FLG_INVALID %>"></asp:CheckBox></td>
							<td align="center">
								<%# Container.ItemIndex + 1 %>
							</td>
							<td align="left">
								<asp:Label ID="lTagId" runat="server" Text="<%# ((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID] %>" Visible='<%# (string)((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCTTAGSETTING_TAG_NO] != "" %>' />
								<div runat="server" Visible='<%# (string)((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCTTAGSETTING_TAG_NO] == "" %>'>
									<%: TAG_ID_INITIALS %>&nbsp;+&nbsp;<asp:TextBox ID="tbTagId" runat="server" Width="150px" MaxLength="26" Text="<%# ((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID] %>" />
								</div>
							</td>
							<td align="left">
								<asp:TextBox ID="tbTagName" runat="server" Width="230" MaxLength="30" Text='<%# ((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCTTAGSETTING_TAG_NAME] %>' />
							</td>
							<td align="center">
								<asp:CheckBox id="cbDeleteFlg" runat="server" Checked="<%# (string)(((Hashtable)Container.DataItem)[FIELD_DELETE_VALIF_FLG]) == FLG_DELETE_VALID %>" Visible='<%# (string)((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCTTAGSETTING_TAG_NO] != "" %>'></asp:CheckBox>
								<asp:LinkButton runat="server" Text="&nbsp;キャンセル&nbsp;" OnClick="btnCancel_Click" CommandArgument="<%# Container.ItemIndex %>" Visible='<%# (string)((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCTTAGSETTING_TAG_NO] == "" %>' />
							</td>
						</tr>
						<asp:HiddenField ID="hdnTagNo" runat="server" Value='<%# ((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCTTAGSETTING_TAG_NO] %>' />
						<asp:HiddenField ID="hdnTagId" runat="server" Value='<%# ((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID] %>' />
						<asp:HiddenField ID="hdnTagName_old" runat="server" Value='<%# ((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCTTAGSETTING_TAG_NAME + TAG_OLD] %>' />
						<asp:HiddenField ID="hdnValidFlg_old" runat="server" Value='<%# ((Hashtable)Container.DataItem)[Constants.FIELD_PRODUCTTAGSETTING_TAG_VALID_FLG + TAG_OLD] %>' />
					</ItemTemplate>
				</asp:Repeater>
			</table>
		</td>
		<td><img alt="" src="../../Images/Common/sp.gif" width="5" height="1" border="0" /></td>
	</tr>
	<tr>
		<td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="4" border="0" /></td>
	</tr>
	<tr>
		<td align="right">
			<asp:Button ID="btnAddBottom" runat="server" Text="&nbsp;追加&nbsp;" OnClick="btnAdd_Click" />&nbsp;
			<asp:Button ID="btnAllUpdateBottom" runat="server" Text="&nbsp;一括更新&nbsp;" OnClick="btnAllUpdate_Click" OnClientClick="return check_delete_fields_confirm();" />
		</td>
		<td width="5"><img alt="" src="../../Images/Common/sp.gif" width="5" height="1" border="0" /></td>
	</tr>
	<tr>
		<td colspan="6"><img alt="" src="../../Images/Common/sp.gif" width="1" height="4" border="0" /></td>
	</tr>
</table>
</ContentTemplate>
</asp:UpdatePanel>
<%-- UPDATE PANELここまで --%>
<table id="note" class="info_table" width="758" border="0" cellspacing="1" cellpadding="3">
	<tr>
		<td align="left" class="info_item_bg" colspan="2">備考<br />
		　1.削除する場合は商品情報に紐付くタグ情報も一緒に削除される為、事前にバックアップを行ってください。<br />
		　2.新規追加は一括更新するまで登録されません。<br />
		　3.実際に登録されるタグIDについて<br />
		　　a) 入力した値の頭文字に「 tag_ 」が付きます。&nbsp;&nbsp;例）入力値が「title」の場合、実際に登録されるのは「tag_title」となります。<br />
		　　b) 大文字で入力した値は小文字に変換して登録されます。&nbsp;&nbsp;例）入力値が「PRODUCT」の場合、実際に登録されるのは「tag_product」となります。<br />
		　4.表示のチェックを外したタグIDは、商品登録時に非表示となります。<br />
		</td>
	</tr>
</table>
<br />
</div>
</td>
</tr>
</table>
</td>
</tr>
</table>
</td>
</tr>
<tr><td><img height="10" width="1" border="0" alt="" src="../../Images/Common/sp.gif" /></td></tr>
</table>
<script type="text/javascript">
<!--
//=============================================================================================
// 更新確認Confirmダイアログ生成（削除するフィールドがあれば番号を出力する）
//=============================================================================================
function check_delete_fields_confirm()
{
var strDeleteFields = "";

<%for (int iLoop = 0; iLoop < this.ProductTagSettingCount; iLoop++) { %>
strDeleteFields += 
	((document.getElementById('<%= rTagList.Items[iLoop].FindControl("cbDeleteFlg").ClientID %>') != null) && (document.getElementById('<%= rTagList.Items[iLoop].FindControl("cbDeleteFlg").ClientID %>').checked)) ? 
		(((strDeleteFields.length != 0) ? ", " : "") + '<%= iLoop+1 + ":" + ((Label)rTagList.Items[iLoop].FindControl("lTagId")).Text %>') : "";

<%} %>
	var strMessage = (strDeleteFields.length != 0)
		? "チェックしたタグIDは商品情報に紐付く実データも削除されます。" + '：' + strDeleteFields
		: "表示内容で更新します。";
	return confirm(strMessage + "\nよろしいですか？");
}
//-->
</script>
</asp:Content>

