<%--
=========================================================================================================
  Module      : データ移行管理(DataMigrationManager.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="DataMigrationManager.aspx.cs" Inherits="Form_DataMigration_DataMigrationManager" %>
<%@ Import Namespace="System.IO" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr>
		<td>
			<h1 class="page-title"><%: ReplaceTag("@@DispText.data_migration_manager.title.name@@") %></h1>
		</td>
	</tr>
	<tr>
		<td>
			<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
		</td>
	</tr>
	<!--△ タイトル △-->
	<!--▽  ファイルアップロードフォーム ▽-->
	<tr>
		<td>
			<h2 class="cmn-hed-h2"><%: ReplaceTag("@@DispText.data_migration_manager.data_upload.header@@") %></h2>
		</td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td>
						<table class="info_box_bg" cellspacing="0" cellpadding="0" border="0" width="100%">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td>
												<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
											</td>
										</tr>
										<tr>
											<td>
												<table class="info_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr>
														<td class="info_item_bg">
															<div class="list_detail">
																<%= this.MessageNotesDataUpload %>
															</div>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="list_title_bg" width="120">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align: middle" border="0" />
															<%: ReplaceTag("@@DispText.data_migration_manager.data_upload.type@@") %>
														</td>
														<td class="list_item_bg1">
															<asp:DropDownList ID="ddlMaster" Runat="server" OnSelectedIndexChanged="ddlMaster_SelectedIndexChanged" AutoPostBack="True" />
														</td>
													</tr>
													<tr>
														<td class="list_title_bg" width="120">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align: middle" border="0" />
															<%: ReplaceTag("@@DispText.data_migration_manager.data_upload.file_path@@") %>
															<span class="notice">*</span>
														</td>
														<td class="list_item_bg1">
															<input id="fUpload" contentEditable="false" size="90" type="file" name="fUpload" runat="server" />
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td>
												<div class="action_part_bottom">
													<asp:Button ID="btnUpload" Runat="server" OnClick="btnUpload_Click" />
												</div>
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
	<!--△ ファイルアップロードフォーム △-->
	<tr>
		<td>
			<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
		</td>
	</tr>
	<!--▽ アップロードファイル一覧 ▽-->
	<tr>
		<td>
			<h2 class="cmn-hed-h2"><%: ReplaceTag("@@DispText.data_migration_manager.uploaded_files.header@@") %></h2>
		</td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<td align="center">
						<table class="list_box_bg" cellspacing="0" cellpadding="0" border="0" width="100%">
							<tr>
								<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
							</tr>
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" width="100%" border="0">
										<tr id="trEnableUploadMessage" runat="server">
											<td align="center">
												<span class="info">
													<%= WebMessages.GetMessages(WebMessages.ERRMSG_DATA_MIGRATION_FILE_UPLOAD_NOTICE) %>
												</span>
												<br />
											</td>
										</tr>
										<tr id="trDisableUploadMessage" runat="server">
											<td align="center">
												<span class="info">
													<%= WebMessages.GetMessages(WebMessages.ERRMSG_DATA_MIGRATION_IMPORT_OR_DELETE_FILE_NOTICE) %>
												</span>
												<br />
											</td>
										</tr>
										<tr>
											<td align="center">
												<table class="list_table" cellspacing="1" cellpadding="3" width="50%" border="0">
													<asp:Repeater ID="rExistFiles" runat="server" OnItemCommand="rExistFiles_ItemCommand">
														<HeaderTemplate>
															<tr class="list_title_bg">
																<td align="left" style="height: 17px"><%: ReplaceTag("@@DispText.data_migration_manager.uploaded_files.file_name@@") %></td>
																<td align="left" style="height: 17px"><%: ReplaceTag("@@DispText.data_migration_manager.uploaded_files.operate@@") %></td>
															</tr>
														</HeaderTemplate>
														<ItemTemplate>
															<tr class="list_item_bg1">
																<td align="left">
																	<%#: Path.GetFileName((string)Container.DataItem) %>
																</td>
																<td align="center">
																	<asp:Button
																		ID="btnImport"
																		CommandName="<%# Constants.FLG_DATA_MIGRATION_COMMAND_NAME_IMPORT %>"
																		CommandArgument='<%# Path.GetFileName((string)Container.DataItem) %>'
																		Text='<%#: ReplaceTag("@@DispText.data_migration_manager.uploaded_files.import_button_text@@") %>'
																		runat="server"
																		OnClientClick='return confirm("取り込んでもよろしいですか？");' />
																	&nbsp;
																	<asp:Button
																		ID="btnDelete"
																		CommandName="<%# Constants.FLG_DATA_MIGRATION_COMMAND_NAME_DELETE %>"
																		CommandArgument='<%# Path.GetFileName((string)Container.DataItem) %>'
																		runat="server" />
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
												</table>
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
			<!--△ アップロードファイル一覧 △-->
		</td>
	</tr>
	<tr>
		<td>
			<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
		</td>
	</tr>
</table>
<script type="text/javascript">
	if (window.history.replaceState) {
		window.history.replaceState(null, null, window.location.href);
	}
</script>
</asp:Content>
