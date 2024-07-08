<%--
=========================================================================================================
  Module      : 外部ファイル取込完了ページ(ExternalImportComplete.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ExternalImportComplete.aspx.cs" Inherits="Form_ExternalImport_ExternalImportComplete" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr>
		<td><h1 class="page-title">外部ファイル取込</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ タイトル △-->
	<!--▽ 完了画面 ▽-->
	<tr>
		<td>
			<h1 class="cmn-hed-h2">外部ファイル取込実行</h1>
		</td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td class="search_box_bg">
						<table cellspacing="0" cellpadding="0" width="98%" border="0">
							<tr>
								<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
							</tr>
							<tr>
								<td align="center">
									外部ファイルの取込を実行しました。<br />
									終了後メールにてお知らせします。
								</td>
							</tr>
							<tr>
								<td>
									<div  class="action_part_bottom">
									<asp:Button ID="btnGoToUpload" Runat="server" Text="  外部ファイル取込ページへ  " OnClick="btnGoToUpload_Click" /></div>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<!--△ 完了画面 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>

</asp:Content>

