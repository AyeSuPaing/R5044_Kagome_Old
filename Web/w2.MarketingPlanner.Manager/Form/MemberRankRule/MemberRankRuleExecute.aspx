<%--
=========================================================================================================
  Module      : 会員ランク付与実行ページ(MemberRankRuleExecute.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="MemberRankRuleExecute.aspx.cs" Inherits="Form_MemberRankRule_MemberRankRuleExecute" %>
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
														<td class="detail_title_bg" align="left" width="30%">会員ランク変動ルールID</td>
														<td class="detail_item_bg" align="left">
															<asp:Label ID="lMemberRankRuleId" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">会員ランク変動ルール名</td>
														<td class="detail_item_bg" align="left">
															<asp:Label ID="lMemberRankRuleName" runat="server"></asp:Label>
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
															<div><asp:Literal id="lTargetStartExtract" runat="server">※「付与実行」をクリックする前に「抽出実行」をクリックして下さい。</asp:Literal></div>
														</td>
													</tr>
												</table>
												<br />
												
												<table class="edit_table" cellspacing="1" cellpadding="3" width="550" border="0">
													<tr>
														<td class="detail_title_bg" align="left" width="30%">準備ステータス</td>
														<td class="detail_item_bg" align="left">
															<asp:Label id="lbMemberRanklPrepareStatus" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">実行ステータス</td>
														<td class="detail_item_bg" align="left">
															<asp:Label id="lbMemberRankExecuteStatus" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">進捗</td>
														<td class="detail_item_bg" align="left">
															<asp:Label id="lbMemberRankProgress" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">付与実行</td>
														<td class="detail_item_bg" align="left">
															<asp:Button ID="btnMemberRankChangeStart" Text="  付与実行  " runat="server" OnClick="btnMemberRankChangeStart_Click" />
															<asp:Button ID="btnMemberRankChangeStop" Text="  付与停止  " runat="server" OnClick="btnMemberRankChangeStop_Click" />
														</td>
													</tr>
												</table>
												<br />
												<table class="info_table" cellspacing="1" cellpadding="3" width="550" border="0">
													<tr class="info_item_bg">
														<td align="left">備考<br />
														・有効フラグが無効なものでも実行可能です。<br />
														</td>
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
