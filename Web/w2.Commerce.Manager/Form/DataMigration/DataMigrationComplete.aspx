<%--
=========================================================================================================
  Module      : データ移行完了(DataMigrationComplete.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="DataMigrationComplete.aspx.cs" Inherits="Form_DataMigration_DataMigrationComplete" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<table cellspacing="0" cellpadding="0" width="791" border="0">
		<!--▽ タイトル ▽-->
		<tr>
			<td>
				<h1 class="page-title"><%: ReplaceTag("@@DispText.data_migration_complete.title@@") %></h1>
			</td>
		</tr>
		<tr>
			<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
		</tr>
		<!--△ タイトル △-->
		<!--▽ 完了画面 ▽-->
		<tr>
			<td>
				<h2 class="cmn-hed-h2"><%: ReplaceTag("@@DispText.data_migration_complete.header@@") %></h2>
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
										<%= WebMessages.GetMessages(WebMessages.ERRMSG_DATA_MIGRATION_COMPLETE_MESSAGE) %>
									</td>
								</tr>
								<tr>
									<td>
										<div class="action_part_bottom">
											<asp:Button ID="btnGoToDataMigration" Runat="server" OnClick="btnGoToDataMigration_Click" />
										</div>
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
