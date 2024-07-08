<%--
=========================================================================================================
  Module      : ターゲットリストアップロードページ(TargetListUpload.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Title="" Language="C#" MasterPageFile="../../Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="TargetListUpload.aspx.cs" Inherits="Form_TargetList_TargetListUpload" %>
<%@ MasterType VirtualPath="../../Form/Common/DefaultPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderHead" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr>
		<td><h1 class="page-title">ターゲットリスト情報</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ タイトル △-->
	<tr>
		<% if(this.ActionStatus == Constants.ACTION_STATUS_UPLOAD || this.ActionStatus ==Constants.ACTION_STATUS_COMPLETE) {%>
		<td><h2 class="cmn-hed-h2">ターゲットリスト情報登録</h2></td>
		<%} %>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td>
						<table class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td>
												<div class="action_part_top" id="divActionTop" runat="server">
													<asp:Button ID="btnRegisterTop" Text="  登録する  " runat="server" Visible="false" onclick="btnRegisterTop_Click" OnClientClick="javascript:disableButton()"/>
													<asp:Button ID="btnDeleteTop" Text="  削除する  " runat="server" Visible="false" onclick="btnDeleteTop_Click"/>													
												</div>
												<table class="detail_table" width="758" border="0" cellspacing="1" cellpadding="3" id="tblTargetListName" runat="server">
													<tr>
														<td align="left" class="search_title_bg" width="150">ターゲットリスト名<span class="notice">*</span></td>
														<td align="left" class="search_item_bg"><asp:TextBox ID="tbTargetListName" Width="300" runat="server"></asp:TextBox></td>
													</tr>
												</table>
												<br />
												<table class="detail_table" width="758" border="0" cellspacing="1" cellpadding="3" id="tblUpload" runat="server" visible="false">
													<tr>
														<td align="left" class="search_title_bg" width="150">ファイルパス</td>
														<td align="left" class="search_item_bg"><input id="fUpload" contentEditable="false" style="height:22px" size="90" type="file" name="fUpload" runat="server" /></td>
													</tr>
												</table>
												<table class="detail_table" width="758" border="0" cellspacing="1" cellpadding="3" id="tblUploadConfirm" runat="server">
													<tr>
														<td align="left" class="search_title_bg" width="150">ファイル名</td>
														<td align="left" class="search_item_bg"><asp:Label ID="lblFileUploadName" runat="server" Text=""></asp:Label>
															<asp:HiddenField ID="hfFileUploadName" runat="server" />
														</td>
													</tr>
													<tr>
														<td align="left" class="search_title_bg">件数</td>
														<td align="left" class="search_item_bg"><asp:Label ID="lblFileDataCount" runat="server" Text=""></asp:Label></td>
													</tr>
												</table>
												<br />
												<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0" id="tblExplain" runat="server">
													<tbody>
														<tr class="info_item_bg">
															<td align="left">
																「ユーザーID」のみ指定したCSVファイルを取り込みます。<br />
																※ヘッダーに「user_id」を定義してください。
															</td>
														</tr>
													</tbody>
												</table>
												<table class="info_box_bg" cellspacing="0" cellpadding="0" width="758" border="0" id="tblComplete" runat="server" visible="false">
													<tr>
														<td align="center">
															ターゲットリストの登録を開始しました。<br />
															結果はメールにてご連絡いたします。<br />
															※ターゲットリストの登録が完了するまで、ターゲットリスト一覧には表示されません。
														</td>
													</tr>
													<tr>
														<td>
															<img height="25" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
														</td>
													</tr>
												</table>
												<div class="action_part_bottom" id="divActionBottom" runat="server">
													<asp:Button ID="btnBackBottom" runat="server" Text="  戻る  " onclick="btnBackBottom_Click" />
													<asp:Button ID="btnUpload" Text="  データアップロード  " runat="server" visible="false" onclick="btnUpload_Click"/>
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
	<tr>
		<td>
			<div class="action_part_bottom" style="width:784px;" id="divActionBottomComplete" runat="server" visible="false">
				<asp:Button ID="btnBackToUpload" runat="server" Text="  戻る  " 	onclick="btnBackToUpload_Click" />
			</div>
		</td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<script language="javascript" type="text/javascript">
	function disableButton() {
		document.getElementById("<%=btnRegisterTop.ClientID %>").style.display = "none";
		document.getElementById("<%=btnDeleteTop.ClientID %>").style.display = "none";
	}
</script>
</asp:Content>

