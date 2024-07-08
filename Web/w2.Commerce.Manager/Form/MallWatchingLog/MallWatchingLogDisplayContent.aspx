<%--
=========================================================================================================
  Module      : モール連携監視　コンテンツ出力ページ(MallWatchingLogDisplayContent.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="MallWatchingLogDisplayContent.aspx.cs" Inherits="Form_MallWatchingLog_MallWatchingLogDisplayContent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="691" border="0">
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<td><h1 class="page-title">ログ詳細</h1></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="684" border="0">
				<tr>
					<td class="list_box_bg" align="center">
						<table cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="658" border="0">
													<tr class="edit_title_bg">
														<td>ログNo</td>
													</tr>
													<tr class="edit_item_bg">
														<td width="658px"><pre><asp:Literal ID="lLogNo" runat="server" /></pre></td>
													</tr>
													<tr class="edit_title_bg">
														<td>ログ日時</td>
													</tr>
													<tr class="edit_item_bg">
														<td width="658px"><pre><asp:Literal ID="lLogDate" runat="server" /></pre></td>
													</tr>
													<tr class="edit_title_bg">
														<td>ログ区分</td>
													</tr>
													<tr class="edit_item_bg">
														<td width="658px"><pre><asp:Literal ID="lLogKbn" runat="server" /></pre></td>
													</tr>
													<tr class="edit_title_bg">
														<td>処理名</td>
													</tr>
													<tr class="edit_item_bg">
														<td width="658px"><pre><asp:Literal ID="lBatchId" runat="server" /></pre></td>
													</tr>
													<tr class="edit_title_bg">
														<td>モールID（モール名）</td>
													</tr>
													<tr class="edit_item_bg">
														<td width="658px"><pre><asp:Literal ID="lMallIdAndName" runat="server" /></pre></td>
													</tr>
													<tr class="edit_title_bg">
														<td>ログメッセージ</td>
													</tr>
													<tr class="edit_item_bg">
														<td width="658px"><pre><asp:Literal ID="lLogMessage" runat="server" /></pre></td>
													</tr>
													<tr class="edit_title_bg">
														<td>詳細1</td>
													</tr>
													<tr class="edit_item_bg">
														<td width="658px"><pre><asp:Literal ID="lLogContent1" runat="server" /></pre></td>
													</tr>
													<tr class="edit_title_bg">
														<td>詳細2</td>
													</tr>
													<tr class="edit_item_bg">
														<td width="658px"><pre><asp:Literal ID="lLogContent2" runat="server" /></pre></td>
													</tr>
													<tr class="edit_title_bg">
														<td>詳細3</td>
													</tr>
													<tr class="edit_item_bg">
														<td width="658px"><pre><asp:Literal ID="lLogContent3" runat="server" /></pre></td>
													</tr>
													<tr class="edit_title_bg">
														<td>詳細4</td>
													</tr>
													<tr class="edit_item_bg">
														<td width="658px"><pre><asp:Literal ID="lLogContent4" runat="server" /></pre></td>
													</tr>
													<tr class="edit_title_bg">
														<td>詳細5</td>
													</tr>
													<tr class="edit_item_bg">
														<td width="658px"><pre><asp:Literal ID="lLogContent5" runat="server" /></pre></td>
													</tr>
												</table>
												<table>
													<tr>
														<td><img height="10px" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
			</table>
		</td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>