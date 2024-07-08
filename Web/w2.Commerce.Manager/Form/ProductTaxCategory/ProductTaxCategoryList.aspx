<%--
=========================================================================================================
  Module      : 商品税率カテゴリページ(ProductTaxCategoryList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ProductTaxCategoryList.aspx.cs" Inherits="Form_TaxCategory_TaxCategoryList" %>
<%@ Import Namespace="w2.Domain.ProductTaxCategory" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
<tr><td><h1 class="page-title">商品税率カテゴリ設定</h1></td></tr>
<tr><td><img height="10" width="100" border="0" alt="" src="../../Images/Common/sp.gif" /></td></tr>
<tr><td><h2 class="cmn-hed-h2">商品税率カテゴリ設定一覧</h2></td></tr>
<tr>
<td>
<table class="box_border" cellspacing="1" cellpadding="0" width="584" border="0">
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
	<div id="dvMessage" runat="server">
		<tr>
			<td>
				<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
					<tr class="info_item_bg">
						<td align="left">
							<div id="dvUpdateComplete" runat="server" visible="false">商品税率カテゴリを登録/更新しました。</div>
							<div id="dvDeleteComplete" runat="server" visible="false">商品税率カテゴリを削除しました。</div>
							<small><asp:Label id="lAlert" runat="server" ForeColor="red"></asp:Label></small>
						</td>
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
			<asp:Button ID="btnAllUpdateTop" runat="server" Text="&nbsp;一括更新&nbsp;" OnClick="btnAllUpdate_Click" OnClientClick="return check_confirm();" />
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
						<td align="center" colspan="6">商品税率カテゴリ設定項目</td>
					</tr>
					<tr class="list_title_bg">
						<td width="38px" align="center">No</td>
						<td width="250px" align="center">商品税率カテゴリID<span class="notice">*</span></td>
						<td width="200px" align="center">商品税率カテゴリ名称<span class="notice">*</span></td>
						<td width="100px" align="center">税率(%)<span class="notice">*</span></td>
						<td width="100px" align="center">表示順<span class="notice">*</span></td>
						<td width="70px" align="center">削除</td>
					</tr>
					<asp:Repeater id="rProductTaxCategoryList" Runat="server">
					<ItemTemplate>
						<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
							<td align="center">
								<%# Container.ItemIndex + 1 %>
							</td>
							<td align="left">
								<asp:Label ID="lProductTaxCategoryId" runat="server" Text="<%# ((ProductTaxCategoryInput)Container.DataItem).TaxCategoryId %>" Visible='<%# ((ProductTaxCategoryInput)Container.DataItem).IsRegistered %>' />
								<div runat="server" Visible='<%# (((ProductTaxCategoryInput)Container.DataItem).IsRegistered == false) %>'>
									<asp:TextBox ID="tbProductTaxCategoryId" runat="server" Width="150px" MaxLength="30" Text="<%# ((ProductTaxCategoryInput)Container.DataItem).TaxCategoryId %>" />
								</div>
							</td>
							<td align="left">
								<asp:TextBox ID="tbProductTaxCategoryName" runat="server" Width="230px" MaxLength="40" Text='<%# ((ProductTaxCategoryInput)Container.DataItem).TaxCategoryName %>' />
							</td>
							<td align="left">
								<asp:TextBox ID="tbTaxRate" runat="server" Width="50px" MaxLength="6" Text='<%# ((ProductTaxCategoryInput)Container.DataItem).TaxRate %>' />
							</td>
							<td align="left">
								<asp:TextBox ID="tbDisplayOrder" runat="server" Width="50px" MaxLength="6" Text='<%# ((ProductTaxCategoryInput)Container.DataItem).DisplayOrder %>' Visible='<%# (((ProductTaxCategoryInput)Container.DataItem).TaxCategoryId) != Constants.DEFAULT_PRODUCT_TAXCATEGORY_ID %>' />
								<asp:Label ID="lDisplayOrder" runat="server" Text="-" Visible='<%# (((ProductTaxCategoryInput)Container.DataItem).TaxCategoryId) == Constants.DEFAULT_PRODUCT_TAXCATEGORY_ID %>' />
							</td>
							<td align="center">
								<asp:Button ID="btnCancel" runat="server" Text="&nbsp;キャンセル&nbsp;" OnClick="btnCancel_Click" CommandArgument="<%# Container.ItemIndex %>" Visible='<%# (((ProductTaxCategoryInput)Container.DataItem).IsRegistered == false) %>' />
								<asp:Button ID="btnDelete" runat="server" Text="&nbsp;削除&nbsp;" OnClick="btnDelete_Click" OnClientClick="return confirm('設定を削除します。よろしいですか？')" CommandArgument="<%# Container.ItemIndex %>" Visible='<%# (((ProductTaxCategoryInput)Container.DataItem).IsRegistered && (((ProductTaxCategoryInput)Container.DataItem).TaxCategoryId) != Constants.DEFAULT_PRODUCT_TAXCATEGORY_ID) %>'/>
							</td>
						</tr>
						<asp:HiddenField ID="hfRegisteredKbn" runat="server" Value='<%# ((ProductTaxCategoryInput)Container.DataItem).RegisteredKbn %>' />
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
			<asp:Button ID="btnAllUpdateBottom" runat="server" Text="&nbsp;一括更新&nbsp;" OnClick="btnAllUpdate_Click" OnClientClick="return check_confirm();" />
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
			商品税率カテゴリID「default」は削除できません。
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
// 更新確認ダイアログ生成
//=============================================================================================
function check_confirm()
{
	return confirm("表示内容で更新します。\nよろしいですか？");
}
//-->
</script>
</asp:Content>