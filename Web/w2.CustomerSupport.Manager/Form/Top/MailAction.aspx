<%--
=========================================================================================================
  Module      : メールアクションページ(MailAction.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Cs.Message" %>
<%@ Import Namespace="w2.App.Common.Cs.SummarySetting" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="MailAction.aspx.cs" Inherits="Form_Top_MailAction" %>
<%@ Register TagPrefix="uc" TagName="ErrorPoint" Src="~/Form/Top/ErrorPointIcon.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderHead" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<div id="divFadeOutArea">
<img alt="" src="../../Images/Common/sp.gif" border="0" height="10" width="5" />
<table width="99%"  border="0" cellspacing="0" cellpadding="0">
	<tbody>
		<tr valign="top">
			<!-- メール表示エリア -->
			<td width="50%">
				<div class="datagrid">
					<table class="list_table" width="100%">
						<thead>
						<tr class="list_title_bg">
							<th>メール本文</th>
						</tr>
						</thead>
					</table>
					<table>
						<tbody>
						<tr>
							<td>
								<div id="mailheader">
								<p>
									<strong>差出人：</strong>
									<asp:Literal ID="lMailFrom" runat="server"></asp:Literal><br />
									<strong>　宛先：</strong>
									<asp:Literal id="lMailTo" runat="server"></asp:Literal><uc:ErrorPoint ID="ucErrorPointTo" runat="server" /><br />
									<span id="spMailCc" runat="server">
									<strong>　　CC：</strong>
									<asp:Literal id="lMailCc" runat="server"></asp:Literal><br />
									</span>
									<span id="spMailBcc" runat="server">
									<strong>　Bcc：</strong>
									<asp:Literal id="lMailBcc" runat="server"></asp:Literal><br />
									</span>
									<strong>　件名：</strong>
									<asp:Literal id="lMailSubject" runat="server"></asp:Literal><br />
									<asp:Repeater ID="rMailAttachmentFiles" runat="server">
									<HeaderTemplate><strong>　添付：</strong></HeaderTemplate>
									<ItemTemplate>
										<img src="../../Images/Cs/icon_clip.png" alt="添付" width="16" height="16" style="vertical-align: bottom;" /><a 
											href="<%# ((CsMessageMailAttachmentModel)Container.DataItem).EX_CreateFileDownloadUrl(Constants.REQUEST_KEY_MAIL_ID, Constants.REQUEST_KEY_FILE_NO) 
											%>"><%#((CsMessageMailAttachmentModel)Container.DataItem).FileName %></a>
									</ItemTemplate>
									<SeparatorTemplate>, </SeparatorTemplate>
									<FooterTemplate></FooterTemplate>
									</asp:Repeater>
								</p>
								</div>
							</td>
						</tr>
						<tr>
							<td>
								<div class="y_scrollable2" style="HEIGHT:276px">
									<div id="maildisp">
										<p>
											<asp:Literal ID="lMailBody" runat="server"></asp:Literal>
										</p>
									</div>
								</div>
							</td>
						</tr>
						</tbody>
					</table>
				</div>
				<div id="divApprComment" class="datagrid" runat="server">
					<table class="list_table" width="100%">
						<thead>
						<tr class="list_title_bg">
							<th>依頼コメント</th>
						</tr>
						</thead>
					</table>
					<table>
						<tbody>
						<tr>
							<td colspan="4">
								<div class="y_scrollable2" style="HEIGHT:100px; width:100%">
									<p>
										<asp:Literal ID="lRequestComment" runat="server"></asp:Literal>
									</p>
								</div>
							</td>
						</tr>
						</tbody>
					</table>
				</div>
			</td>
			<td><img alt="" src="../../Images/Common/sp.gif" border="0" height="5" width="5" /></td>
			<!-- インシデント表示エリア -->
			<td width="50%">
				<div class="datagrid">
					<table class="list_table" width="100%">
						<thead>
						<tr class="list_title_bg">
							<th>インシデント</th>
						</tr>
						</thead>
					</table>
					<table>
						<tr>
							<td width="30%" class="alt">インシデントID</td>
							<td width="70%">
								<span class="tab_title_left">
									<asp:Literal ID="lIncidentId" runat="server"></asp:Literal>
								</span>
							</td>
						</tr>
						<tr>
							<td class="alt">ユーザーID</td>
							<td><asp:Literal ID="lUserId" runat="server"></asp:Literal></td>
						</tr>
						<tr>
							<td class="alt">タイトル</td>
							<td><asp:Literal ID="lIncidentTitle" runat="server"></asp:Literal></td>
						</tr>
						<tr>
							<td class="alt">カテゴリ</td>
							<td><asp:Literal ID="lIncidentCategory" runat="server"></asp:Literal></td>
						</tr>
						<tr>
							<td class="alt">ステータス</td>
							<td><asp:Literal ID="lIncidentStatus" runat="server"></asp:Literal></td>
						</tr>
						<tr>
							<td class="alt">重要度</td>
							<td><asp:Literal ID="lIncidentImportance" runat="server"></asp:Literal></td>
						</tr>
						<tr>
							<td class="alt">VOC</td>
							<td>
								<asp:Literal ID="lIncidentVocText" runat="server"></asp:Literal>
								<div><asp:Literal ID="lIncidentVocMemo" runat="server"></asp:Literal></div>
							</td>
						</tr>
						<tr>
							<td rowspan="2" class="alt">担当</td>
							<td>グループ： <asp:Literal ID="lIncidentCsGroupName" runat="server"></asp:Literal></td>
						</tr>
						<tr>
							<td>オペレータ： <asp:Literal ID="lIncidentCsOperatorName" runat="server"></asp:Literal></td>
						</tr>
						<tr>
							<td class="alt">内部メモ</td>
							<td>
								<div class="y_scrollable2" style="HEIGHT:50px; width:100%">
									<asp:Literal ID="lIncidentComment" runat="server"></asp:Literal>
								</div>
							</td>
						</tr>
						<asp:Repeater ID="rIncidentSummary" runat="server">
						<ItemTemplate>
							<tr>
								<asp:HiddenField ID="hfSummaryNo" Value="<%# ((CsSummarySettingModel)Container.DataItem).SummarySettingNo %>" runat="server" />
								<td class="alt"><%# WebSanitizer.HtmlEncode(((CsSummarySettingModel)Container.DataItem).SummarySettingTitle) %></td>
								<td colspan="4"><asp:Literal ID="lIncidentSummaryText" runat="server"></asp:Literal></td>
							</tr>
						</ItemTemplate>
						</asp:Repeater>
						<tr>
							<td class="alt">最終更新者</td>
							<td><asp:Literal ID="lIncidentLastChanged" runat="server"></asp:Literal></td>
						</tr>

					</table>
				</div>
				<img alt="" src="../../Images/Common/sp.gif" border="0" height="5" width="5" />

				<%if ((this.MailActionType == MailActionType.ApprovalOK)
					|| (this.MailActionType == MailActionType.ApprovalNG)
					|| (this.MailActionType == MailActionType.SendNG)) {%>
				<div class="datagrid">
					<table class="list_table" width="100%">
						<thead>
						<tr class="list_title_bg">
							<th>
								<%if (this.MailActionType == MailActionType.ApprovalOK) {%>承認コメント<%} %>
								<%else if (this.MailActionType == MailActionType.ApprovalNG) {%>差し戻しコメント<%} %>
								<%else if (this.MailActionType == MailActionType.SendNG) {%>差し戻しコメント<%} %>
							</th>
						</tr>
						</thead>
					</table>
					<table>
						<tbody>
						<tr>
							<td colspan="4">
								<div>
								<p>
									<asp:TextBox ID="tbResultCommnet" TextMode="MultiLine" Width="99%" Height="94px" runat="server"></asp:TextBox>
								</p>
								</div>
							</td>
						</tr>
						</tbody>
					</table>
				</div>
				<%} %>
			</td>
			<td><img alt="" src="../../Images/Common/sp.gif" border="0" height="10" width="5" /></td>
		</tr>
	</tbody>
</table>
<img alt="" src="../../Images/Common/sp.gif" border="0" height="10" width="5" />
<div style="font-weight:bold;padding:5px">
<asp:Literal ID="lWorkingMesaage" EnableViewState="false" runat="server"></asp:Literal>
</div>
<%
	btnApprovalCancel.Enabled
		= btnApprovalOK.Enabled
		= btnApprovalOKSend.Enabled
		= btnApprovalNG.Enabled
		= btnSendCancel.Enabled
		= btnSendNG.Enabled
		= this.LoginOperatorCsInfo.EX_PermitEditFlg;	
%>
<div id="divApprovalCancelArea" visible="false" runat="server">
	<asp:Button ID="btnApprovalCancel" Text="  承認依頼取り下げ  " OnClientClick="return confirm('承認依頼を取り下げします。よろしいですか？')" runat="server" OnClick="btnApprovalCancel_Click" />
	<asp:Label ID="lApprovalCancelMessage" Visible="false" Text="承認依頼を取り下げしました。" Font-Bold="true"  runat="server"></asp:Label>
</div>
<div id="divApprovalOKArea" visible="false" runat="server">
	<asp:Button ID="btnApprovalOK" Text="  承認する  " OnClientClick="return confirm('依頼を承認します。よろしいですか？')" runat="server" OnClick="btnApprovalOK_Click" />
	<asp:Label ID="lApprovalOKMessage" Visible="false" Text="承認しました。" Font-Bold="true"  runat="server"></asp:Label>
</div>
<div id="divApprovalOKSendArea" visible="false" runat="server">
	<asp:Button ID="btnApprovalOKSend" Text="  送信する  " OnClientClick="return confirm('メールを送信します。よろしいですか？')" runat="server" OnClick="btnApprovalOKSend_Click" />
	&nbsp;
	<asp:Button ID="btnApprovalOKSendAndCloseIncident" Text="  送信してインシデントクローズ  " OnClientClick="return confirm('メールを送信してインシデントをクローズします。よろしいですか？')" runat="server" OnClick="btnApprovalOKSendAndCloseIncident_Click" />
	<asp:Label ID="lApprovalOKMessageError" Font-Bold="true" CssClass="notice" EnableViewState="false" runat="server"></asp:Label>
	<asp:Label ID="lApprovalOKSendMessage" Visible="false" Text="メールを送信しました。" Font-Bold="true"  runat="server"></asp:Label>
</div>
<div id="divApprovalNGArea" visible="false" runat="server">
	<asp:Button ID="btnApprovalNG" Text="  承認依頼差戻し  " OnClientClick="return confirm('承認依頼を差戻しします。よろしいですか？')" runat="server" OnClick="btnApprovalNG_Click" />
	<asp:Label ID="lApprovalNGMessage" Visible="false" Text="差戻ししました。" Font-Bold="true"  runat="server"></asp:Label>
</div>
<div id="divSendCancelArea" visible="false" runat="server">
	<asp:Button ID="btnSendCancel" Text="  送信依頼取り下げ  " OnClientClick="return confirm('送信依頼を取り下げします。よろしいですか？')" runat="server" OnClick="btnSendCancel_Click" />
	<asp:Label ID="lSendCancelMessage" Visible="false" Text="送信依頼を取り下げしました。" Font-Bold="true"  runat="server"></asp:Label>
</div>
<div id="divSendNGArea" visible="false" runat="server">
	<asp:Button ID="btnSendNG" Text="  送信依頼差戻し  " OnClientClick="return confirm('送信依頼を差戻しします。よろしいですか？')" runat="server" OnClick="btnSendNG_Click" />
	<asp:Label ID="lSendNGMessage" Visible="false" Text="差戻ししました。" Font-Bold="true"  runat="server"></asp:Label>
</div>
</div>

<%-- トップページ更新用スクリプト --%>
<div id="divRefreshScript" visible="false" runat="server">
<script type="text/javascript">
	function refresh_opener()
	{
		if (window.opener && (window.opener.closed == false) && window.opener.refresh) {
			window.opener.refresh();
		}
	}
</script>
</div>

<%-- フェードアウト＆クローズ --%>
<div id="divFadeoutAndCloseScript" visible="false" runat="server">
<script type="text/javascript">
	$(function ()
	{
		setTimeout(function () {
			$('#divFadeOutArea').fadeOut('fast', function () {
				refresh_opener();
				window.close();
			});
		}, 500);
	});
</script>
</div>

</asp:Content>

