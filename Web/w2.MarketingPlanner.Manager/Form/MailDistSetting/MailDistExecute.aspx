<%--
=========================================================================================================
  Module      : メール配信実行ページ(MailDistExecute.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="MailDistExecute.aspx.cs" Inherits="Form_MailDistSetting_MailDistExecute" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<asp:UpdatePanel runat="server">
<ContentTemplate>
<table cellspacing="0" cellpadding="0" width="605" border="0">
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="100%" border="0">
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" border="0" width="100%">
							<tr>
								<td align="center" style="width: 600px">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="550" border="0">
													<tr>
														<td class="detail_title_bg" align="left" width="30%">メール配信設定ID</td>
														<td class="detail_item_bg" align="left">
															<asp:Label ID="lbMailDistId" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">メール配信設定名</td>
														<td class="detail_item_bg" align="left">
															<asp:Label ID="lbMailDistName" runat="server"></asp:Label>
														</td>
													</tr>
												</table>
												<br />
												
												<table class="edit_table" cellspacing="1" cellpadding="3" width="550" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">ターゲット抽出</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">実行ステータス/進捗</td>
														<td class="detail_item_bg" align="left">
															<asp:Label id="lbTargetExecuteStatus" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">抽出実行</td>
														<td class="detail_item_bg" align="left">
															<asp:Button ID="btnTargetStartExtract" Text="  抽出実行  " runat="server" OnClick="btnStartExtract_Click" />&nbsp;
														</td>
													</tr>
												</table>
												<br />
												
												<table class="edit_table" cellspacing="1" cellpadding="3" width="550" border="0">
													<tr>
														<td class="detail_title_bg" align="left" width="30%">準備ステータス</td>
														<td class="detail_item_bg" align="left">
															<asp:Label id="lbMailPrepareStatus" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">実行ステータス</td>
														<td class="detail_item_bg" align="left">
															<asp:Label id="lbMailExecuteStatus" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">エラーポイント除外数</td>
														<td class="detail_item_bg" align="left">
															<asp:Label id="lbErrorPointExceptCount" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">モバイルメール除外数</td>
														<td class="detail_item_bg" align="left">
															<asp:Label id="lbMobileMailExceptCount" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">重複配信除外件数</td>
														<td class="detail_item_bg" align="left">
															<asp:Literal id="lDuplicateExceptCount" runat="server" />
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">進捗</td>
														<td class="detail_item_bg" align="left">
															<asp:Label id="lbMailProgress" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">配信実行</td>
														<td class="detail_item_bg" align="left">
															<asp:Button ID="btnMailStartDelivery" Text="  配信実行  " runat="server" OnClick="btnStartDelivery_Click" />
															<asp:Button ID="btnMailStopDelivery" Text="  配信停止  " runat="server" OnClick="btnStopDelivery_Click" />
														</td>
													</tr>
												</table>
												<br />
												<table class="info_table" cellspacing="1" cellpadding="3" width="550" border="0">
													<tr class="info_item_bg">
														<td align="left">備考<br />
														・有効フラグが無効なものでも実行可能です。<br />
														・抽出するターゲット毎にユーザーIDで重複排除されます。<br />
														・配信時にはメールアドレスで重複排除されます。（ユーザIDの小さいもの、ＰＣユーザー優先）<br />
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="7" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<asp:HiddenField id="hfReload" runat="server"/>
</ContentTemplate>
</asp:UpdatePanel>
<script type="text/javascript">
	setInterval(ReloadPage, 3000);
	function ReloadPage() {
		__doPostBack('<%= hfReload.UniqueID %>', '');
	}
</script>
</asp:Content>

