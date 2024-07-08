<%--
=========================================================================================================
  Module      : アフィリエイトレポート一覧 コンテンツ出力ページ(AffiliateReportDisplayContent.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="AffiliateReportDisplayContent.aspx.cs" Inherits="Form_AffiliateReport_AffiliateReportDisplayContent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="691" border="0">
	<tr>
		<td><h2 class="cmn-hed-h2">アフィリエイト連携ログ詳細</h2></td>
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
														<td>アフィリエイト区分</td>
													</tr>
													<tr class="edit_item_bg">
														<td width="658px"><pre><asp:Literal ID="lAffiliateKbn" runat="server" /></pre></td>
													</tr>
													<tr class="edit_title_bg">
														<td>マスタID</td>
													</tr>
													<tr class="edit_item_bg">
														<td width="658px"><pre><asp:Literal ID="lMasterId" runat="server" /></pre></td>
													</tr>
													<tr class="edit_title_bg">
														<td>タグID</td>
													</tr>
													<tr class="edit_item_bg">
														<td width="658px"><pre><asp:Literal ID="lTagId" runat="server" /></pre></td>
													</tr>
													<tr class="edit_title_bg">
														<td>連携データ1</td>
													</tr>
													<tr class="edit_item_bg">
														<td width="658px"><pre><asp:Literal ID="lCoopData1" runat="server" /></pre></td>
													</tr>
													<tr class="edit_title_bg">
														<td>連携データ2</td>
													</tr>
													<tr class="edit_item_bg">
														<td width="658px"><pre><asp:Literal ID="lCoopData2" runat="server" /></pre></td>
													</tr>
													<tr class="edit_title_bg">
														<td>連携データ3</td>
													</tr>
													<tr class="edit_item_bg">
														<td width="658px"><pre><asp:Literal ID="lCoopData3" runat="server" /></pre></td>
													</tr>
													<tr class="edit_title_bg">
														<td>連携データ4</td>
													</tr>
													<tr class="edit_item_bg">
														<td width="658px"><pre><asp:Literal ID="lCoopData4" runat="server" /></pre></td>
													</tr>
													<tr class="edit_title_bg">
														<td>連携データ5</td>
													</tr>
													<tr class="edit_item_bg">
														<td width="658px"><pre><asp:Literal ID="lCoopData5" runat="server" /></pre></td>
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