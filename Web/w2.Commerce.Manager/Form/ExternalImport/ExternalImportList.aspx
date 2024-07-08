<%--
=========================================================================================================
  Module      : 外部ファイル取込一覧ページ(ExternalImportList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ExternalImportList.aspx.cs" Inherits="Form_ExternalImport_ExternalImportList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr>
		<td><h1 class="page-title">外部ファイルアップロード</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ タイトル △-->
	<!--▽ 外部アップロードフォーム ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">外部ファイルアップロード</h2></td>
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
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>

										<tr>
											<td>
												<table class="info_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr>
														<td class="info_item_bg">
															<div class="list_detail">外部ファイル(ＣＳＶファイル)のアップロードをします。ファイルは各種別に対して１個のみアップロード可能です。
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
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															ファイル種別</td>
														<td class="list_item_bg1">
															<asp:DropDownList
																id="ddlExternal"
																Runat="server"
																AutoPostBack="True"
																OnSelectedIndexChanged="ddlExternal_SelectedIndexChanged" />
														</td>
													</tr>
													<tr runat="server" id="trSiteInformation" visible="false">
														<td class="list_title_bg" width="120">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															サイト
															<span class="notice">*</span>
														</td>
														<td class="list_item_bg1">
															<asp:DropDownList
																id="ddlSiteInformation"
																Runat="server"
																AutoPostBack="True"
																OnSelectedIndexChanged="ddlSiteInformation_SelectedIndexChanged"
																Width="270" />
														</td>
													</tr>
													<tr>
														<td class="list_title_bg" width="120">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															ファイルパス
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
											<td><div class="action_part_bottom"><asp:Button id="btnUpload" Runat="server" Text="  データアップロード  " OnClick="btnUpload_Click" /></div></td>
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
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ アップロードファイル一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">アップロード済ファイル一覧</h2></td>
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
											<td align="center"><div class="info">現在アップロードされているファイルはありません。<br />
												ファイルのアップロードが可能です。<br />
												</div>
											</td>
										</tr>
										<tr id="trDisableUploadMessage" runat="server">
											<td align="center"><div class="info">
													別ファイルアップロードのためにはファイルの取込又は削除を行ってください。<br />
												</div>
											</td>
										</tr>
										<tr>
											<td align="center">
												<table class="list_table" cellspacing="1" cellpadding="3" width="50%" border="0">
													<asp:Repeater id="rExistFiles" runat="server" OnItemCommand="rExistFiles_ItemCommand">
													<HeaderTemplate>
														<tr class="list_title_bg">
															<td align="left" style="height: 17px">ファイル名</td>
															<td align="left" style="height: 17px">操作</td>
														</tr>
													</HeaderTemplate>
													<ItemTemplate>
														<tr class="list_item_bg1">
															<td align="left">
																<%# WebSanitizer.HtmlEncode(System.IO.Path.GetFileName((string)Container.DataItem))%>
															</td>
															<td align="center">
																<asp:Button ID="btnImport" CommandName="import" CommandArgument='<%# System.IO.Path.GetFileName((string)Container.DataItem) %>' Text="  取込実行  " runat="server" OnClientClick='return confirm("取り込んでもよろしいですか？");' />
																&nbsp;
																<asp:Button ID="btnDelete" CommandName="delete" CommandArgument='<%# System.IO.Path.GetFileName((string)Container.DataItem) %>' Text="  削除  " runat="server" OnClientClick='return confirm("削除してもよろしいですか？");' />
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
			<!--△ 外部ファイル一覧 △-->
		</td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>

</asp:Content>

