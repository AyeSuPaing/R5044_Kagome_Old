<%--
=========================================================================================================
  Module      : マスタアップロードページ(MasterImportList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MasterImportList.aspx.cs" Inherits="Form_MasterImport_MasterImportInput" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr>
		<td><h1 class="page-title">マスタアップロード</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ タイトル △-->
	<!--▽  ファイルアップロードフォーム ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">マスタアップロード</h2></td>
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
															<div class="list_detail">マスタデータ(ＣＳＶファイル)のアップロードをします。ファイルは各マスタ種別に対して１個のみアップロード可能です。
															<br />
															アップロード可能なファイルの最大サイズは <b><%= MaxRequestLength %>MB</b> です。
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
															マスタ種別</td>
														<td class="list_item_bg1">
															<asp:DropDownList id="ddlMaster" Runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlMaster_SelectedIndexChanged"></asp:DropDownList></td>
													</tr>
													<tr>
														<td class="list_title_bg" width="120">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />ファイルパス<span class="notice">*</span></td>
														<td class="list_item_bg1">
															<input id="fUpload" contentEditable="false" style="WIDTH: 500px;" size="90" type="file" name="fUpload" runat="server" />
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td>
												<%if ((ddlMaster.SelectedValue == "w2_UserNotValidator")
													 || (ddlMaster.SelectedValue == "w2_User")) {%>
												<img height="7" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="info_item_bg">
														<td align="left">
															【ご注意ください】<br />
															存在しないメールアドレスを大量に登録されますと、携帯キャリアの配信制御にかかり、メール配信機能が永久停止される可能性があります。<br />
															ご登録の際には、ご注意頂けますようお願い申し上げます。
														</td>
													</tr>
												</table>
												<%} %>
												<div class="action_part_bottom"><asp:Button id="btnUpload" Runat="server" Text="  データアップロード  " OnClick="btnUpload_Click" /></div></td>
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
											<td align="center"><span class="info">現在アップロードされているファイルはありません。<br />
												ファイルのアップロードが可能です。<br />
												</span>
											</td>
										</tr>
										<tr id="trDisableUploadMessage" runat="server">
											<td align="center"><span class="info">
												別ファイルアップロードのためにはファイルの取込又は削除を行ってください。</span><br />
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
			<!--△ アップロードファイル一覧 △-->
		</td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>