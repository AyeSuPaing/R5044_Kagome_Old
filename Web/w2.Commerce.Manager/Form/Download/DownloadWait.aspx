<%--
=========================================================================================================
  Module      : ダウンロード待機ページ(DownloadWait.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="DownloadWait.aspx.cs" Inherits="Form_Download_DownloadWait" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="618" border="0">
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="698" border="0">			    
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="14" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="list_table" cellspacing="1" cellpadding="3" width="650" border="0">
													<tr class="list_title_bg">
														<td align="center" width="150" style="height: 17px">作成日時</td>
														<td align="center" width="500" style="height: 17px">ファイル</td>
													</tr>
													<asp:Repeater id="rList" Runat="server">
														<ItemTemplate>
															<span runat="server" visible='<%# (bool)((Hashtable)Container.DataItem)["complete"] %>'>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
																<td align="center"><%# WebSanitizer.HtmlEncode(((Hashtable)Container.DataItem)["creationtime"]) %></td>
																<td align="center"><a href='<%# ((Hashtable)Container.DataItem)["fileurl"] %>'  target="_blank"><%# WebSanitizer.HtmlEncode(((Hashtable)Container.DataItem)["filename"])%></a></td>
															</tr>
															</span>
															<span runat="server" visible='<%# ((bool)((Hashtable)Container.DataItem)["complete"]) == false %>'>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
																<td align="center"><img alt="" src="../../Images/Common/check_running.gif" width="20" height="20" border="0" /></td>
																<td align="center">現在ファイル作成待ちです..</td>
															</tr>
															</span>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trNoList" class="list_alert" runat="server" Visible="true">
														<td colspan="3">現在ファイル作成待ちです..</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="14" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
	<!--△ 一覧 △-->
	<tr>
		<td style="width: 792px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>

<script type="text/javascript">
<!--
function reload()
{
	document.aspnetForm.submit();
}

<% if ((bool)ViewState["isCreating"]){%>
	setTimeout("reload()", 800);
<% } %>
// -->
</script>
</asp:Content>