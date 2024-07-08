<%--
=========================================================================================================
  Module      : メッセージ確認（メールプレビュー）ページ(MessageConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Cs.SummarySetting" %>
<%@ Import Namespace="w2.App.Common.Cs.Message" %>
<%@ Page Title="メールプレビュー" Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="MessageConfirm.aspx.cs" Inherits="Form_Message_MessageConfirm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderHead" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<div id="divFadeOutArea">

<table border="0" cellpadding="0" cellspacing="0" width="100%">
<tr>
	<td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
</tr>
<tr>
<td colspan="3" align="center" valign="top">

	<asp:UpdatePanel ID="up1" runat="server">
	<ContentTemplate>

	<table border="0" cellpadding="0" cellspacing="0" width="99%">
	<tr>
		<td width="50%" align="center" valign="top">
			<table width="100%" border="0" cellspacing="0" cellpadding="0">
			<tr>
				<td><img alt="" src="../../Images/Common/sp.gif" border="0" height="5" width="5" /></td>
			</tr>
			<tr>
				<td>
					<div style="WIDTH:100%;float:left;margin:0px 0px 2px 0px">
					<div class="datagrid" id="sendmail">
						<table>
						<thead>
							<tr><th colspan="3">送信メール</th></tr>
						</thead>
						</table>
					</div>
					</div>

					<!-- メール本文 -->
					<div class="datagrid">
						<a name="a_replymail_title" id="a_replymail_title"></a>

						<!-- メールフォーム -->
						<table>
							<tr>
								<td width="15%" class="alt">宛先</td>
								<td width="85%" colspan="2" title="内容に誤りがないかご確認ください" class="mailConfirm">
									<asp:Literal ID="lPreviewMailTo" runat="server"></asp:Literal>
								</td>
							</tr>
							<tr id="trPreviewCc" visible="false" runat="server">
								<td class="alt">Cc</td>
								<td colspan="2">
									<asp:Literal ID="lPreviewMailCc" runat="server"></asp:Literal>
								</td>
							</tr>
							<tr id="trPreviewBcc" visible="false" runat="server">
								<td class="alt">Bcc</td>
								<td colspan="2">
									<asp:Literal ID="lPreviewMailBcc" runat="server"></asp:Literal>
								</td>
							</tr>
							<tr>
								<td class="alt">送信元</td>
								<td colspan="2">
									<asp:Literal ID="lPreviewMailFrom" runat="server"></asp:Literal>
								</td>
							</tr>
							<tr>
								<td class="alt">件名</td>
								<td title="内容に誤りがないかご確認ください" class="mailConfirm" colspan="2">
									<asp:Literal ID="lPreviewMailSubject" runat="server"></asp:Literal>
								</td>
							</tr>
							<tr height="250">
								<td class="alt">内容</td>
								<td colspan="2" valign="top">
									<br />
									<asp:Literal id="lPreviewMailBody" runat="server"></asp:Literal><br />
									<br />
								</td>
							</tr>
							<tr id="trPreviewAttachment" visible="false" runat="server">
								<td class="alt">添付</td>
								<td class="mailConfirm" title="内容に誤りがないかご確認ください" colspan="2">
									<asp:Repeater ID="rPreviewAttachmentFiles" runat="server">
									<ItemTemplate>
									<div>
										<%# WebSanitizer.HtmlEncode(((CsMessageMailAttachmentModel)Container.DataItem).FileName) %>
									</div>
									</ItemTemplate>
									</asp:Repeater>
								</td>
							</tr>
						</table>

					</div>

					<!-- 承認フォーム -->
					<% divApprovalForm.Visible = (this.MailActionType == MailActionType.ApprovalRequest); %>
					<div id="divApprovalForm" class="datagrid" runat="server">
						<table>
						<thead>
							<tr><th colspan="4">承認依頼</th></tr>
						</thead>
						<tbody>
							<tr>
								<td width="15%" class="alt">承認者 </td>
								<td width="85%" colspan="3">
									<asp:Repeater ID="rApprovalOperators" runat="server">
									<ItemTemplate>
										<%# WebSanitizer.HtmlEncode(((CsMessageRequestItemModel)Container.DataItem).EX_ApprOperatorName) %>
									</ItemTemplate>
									<SeparatorTemplate><br /></SeparatorTemplate>
									</asp:Repeater>
								</td>
							</tr>
							<tr>
								<td class="alt">緊急フラグ</td>
								<td colspan="3">
									<asp:Literal ID="lApprovalUrgencyFlg" runat="server"></asp:Literal>
								</td>
							</tr>
							<tr>
								<td class="alt">承認方法</td>
								<td colspan="3">
									<asp:Literal ID="lApprovalType" runat="server"></asp:Literal>
								</td>
							</tr>
							<tr>
								<td class="alt" style="white-space: nowrap">承認依頼コメント</td>
								<td colspan="3">
									<asp:Literal ID="lApprovalRequestComment" runat="server"></asp:Literal>&nbsp;
								</td>
							</tr>
						</tbody>
						</table>
					</div>

					<!-- 送信依頼フォーム -->
					<% divSendRequest.Visible = (this.MailActionType == MailActionType.MailSendRequest); %>
					<div id="divSendRequest" class="datagrid" runat="server">
						<table>
						<thead>
							<tr>
							<th colspan="4">送信依頼</th>
							</tr>
						</thead>
						<tbody>
							<tr>
								<td width="15%" class="alt">
									送信者&nbsp;
								</td>
								<td width="85%" colspan="3">
									<asp:Repeater ID="rMailSendableOperators" runat="server">
									<ItemTemplate>
										<%# WebSanitizer.HtmlEncode(((CsMessageRequestItemModel)Container.DataItem).EX_ApprOperatorName) %>
									</ItemTemplate>
									<SeparatorTemplate><br /></SeparatorTemplate>
									</asp:Repeater>
								</td>
							</tr>
							<tr>
								<td class="alt">緊急度</td>
								<td colspan="3">
									<asp:Literal ID="lMailSendRequestUrgencyFlg" runat="server"></asp:Literal>
								</td>
							</tr>
							<tr>
								<td class="alt">送信依頼コメント</td>
								<td colspan="3">
									<asp:Literal ID="lMailSendRequestComment" runat="server"></asp:Literal>&nbsp;
								</td>
							</tr>
						</tbody>
						</table>
					</div>

				</td>
			</tr>
			<tr>
				<td>&nbsp;</td>
			</tr>
		</table>
		</td>
		<td width="5"><img alt="" src="../../Images/Common/sp.gif" border="0" height="5" width="5" /></td>
		<td width="50%" valign="top">
			<table width="100%" border="0" cellspacing="0" cellpadding="0">
			<tr>
				<td><img alt="" src="../../Images/Common/sp.gif" border="0" height="5" width="5" /></td>
			</tr>
			<tr>
			<td valign="top">
				<!-- 顧客情報エリア -->
				<div style="WIDTH:100%;float:left;margin:0px 0px 2px 0px">
				<div class="datagrid">
					<table>
					<thead>
						<tr><th colspan="3">顧客情報</th></tr>
					</thead>
					</table>
				</div>
				</div>
				<div id="divUserInfo" class="datagrid" runat="server">
					<table>
					<tr>
						<td width="15%" class="alt">氏名</td>
						<td width="30%"><asp:Literal ID="lUserName" runat="server"></asp:Literal></td>
						<td width="15%" class="alt"><%: ReplaceTag("@@User.name_kana.name@@") %></td>
						<td width="40%"><asp:Literal ID="lUserNameKana" runat="server"></asp:Literal></td>
					</tr>
					<tr>
						<td class="alt">電話番号</td>
						<td><asp:Literal ID="lUserTel" runat="server"></asp:Literal></td>
						<td class="alt">メール</td>
						<td><asp:Literal ID="lUserMail" runat="server"></asp:Literal></td>
					</tr>
					</table>
					<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
				</div>

				<!-- インシデントエリア -->
				<div style="WIDTH:100%;float:left;margin:0px 0px 2px 0px">
				<div class="datagrid">
					<table>
					<thead>
						<tr><th colspan="3">インシデント</th></tr>
					</thead>
					</table>
				</div>
				</div>
				<div id="divIncident" class="datagrid" runat="server">
					<table>
					<tr>
						<td width="20%" class="alt">インシデントID</td>
						<td colspan="3"><asp:Literal ID="lIncidentId" runat="server"></asp:Literal></td>
					</tr>
					<tr>
						<td width="20%" class="alt">ユーザーID</td>
						<td colspan="3"><asp:Literal ID="lUserId" runat="server"></asp:Literal></td>
					</tr>
					<tr>
						<td class="alt">タイトル</td>
						<td colspan="3"><asp:Literal ID="lIncidentTitle" runat="server"></asp:Literal></td>
					</tr>
					<tr>
						<td class="alt">カテゴリ</td>
						<td colspan="3"><asp:Literal ID="lIncidentCategoryName" runat="server"></asp:Literal></td>
					</tr>
					<tr>
						<td width="20%" class="alt">ステータス</td>
						<td width="30%"><asp:Literal ID="lIncidentStatus" runat="server"></asp:Literal></td>
						<td width="20%" class="alt">重要度</td>
						<td width="30%"><asp:Literal ID="lIncidentImportance" runat="server"></asp:Literal></td>
					</tr>
					<tr>
						<td class="alt">VOC</td>
						<td colspan="3">
							<asp:Literal ID="lIncidentVoc" runat="server"></asp:Literal><br />
							<asp:Literal ID="lIncidentVocMemo" runat="server"></asp:Literal>
						</td>
					</tr>
					<tr>
						<td class="alt">担当グループ</td>
						<td><asp:Literal ID="lIncidentOperatorGroup" runat="server"></asp:Literal></td>
						<td class="alt">担当オペレータ</td>
						<td><asp:Literal ID="lIncidentOperatorName" runat="server"></asp:Literal></td>
					</tr>
					<tr>
						<td class="alt">内部メモ</td>
						<td colspan="3"><asp:Literal ID="lIncidentComment" runat="server"></asp:Literal></td>
					</tr>
					<asp:Repeater ID="rIncidentSummary" runat="server">
					<ItemTemplate>
					<tr>
						<td class="alt">
							<%# WebSanitizer.HtmlEncode(((CsSummarySettingModel)Container.DataItem).SummarySettingTitle) %>
							<asp:HiddenField ID="hfSummaryNo" Value="<%# ((CsSummarySettingModel)Container.DataItem).SummarySettingNo %>" runat="server" />
						</td>
						<td colspan="4">
							<asp:Literal ID="lIncidentSummaryText" runat="server"></asp:Literal>
						</td>
					</tr>
					</ItemTemplate>
					</asp:Repeater>
					</table>
				</div>
			</td>
			</tr>
			</table>
		</td>
	</tr>
	</table>
	<img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /><br />

	<asp:Label ID="lErrorMessages" CssClass="notice" runat="server"></asp:Label>
	<asp:Label ID="lCompleteMessages" Font-Bold="true" EnableViewState="false" runat="server"></asp:Label>

	<!-- アクションボタン -->
	<div style="WIDTH:100%;float:left;padding:5px 2px 8px 2px">
		<div style="margin: 10px 0px 15px 0px;"><asp:Label ID="lErrorPoint" runat="server" style="font-size: 9pt;" /></div>
		<%if (this.MailActionType == MailActionType.MailSend) { %>
		<asp:Button ID="btnSendMail" Text="  送信する  " runat="server" OnClick="btnSendMail_Click" />
		<%} else if (this.MailActionType == MailActionType.ApprovalRequest) { %>
		<asp:Button ID="btnApproveRequest" Text="  承認依頼する  " runat="server" OnClick="btnApproveRequest_Click" />
		<%} else if (this.MailActionType == MailActionType.MailSendRequest) { %>
		<asp:Button ID="btnSendRequest" Text="  送信依頼する  " runat="server" OnClick="btnSendRequest_Click" />
		<%} %>
		<asp:Button ID="btnBack" Text="  戻る  " runat="server" OnClick="btnBack_Click" />
	</div>

	</ContentTemplate>
	</asp:UpdatePanel>

</td>
</tr>
</table>
</div>

<script type="text/javascript">

	// ロック更新用
	if (window.opener && (window.opener.closed == false) && window.opener.refresh_incident_and_imessage_parts) {
		window.opener.refresh_incident_and_imessage_parts();
	}

	// トップページ更新用スクリプト
	function refresh_opener()
	{
		if (window.opener && (window.opener.closed == false) && window.opener.refresh) {
			window.opener.refresh();
		}
	}

	// フェードアウト＆クローズ
	function fadeout_and_close()
	{
		$('#divFadeOutArea').fadeOut('fast', function () {
			refresh_opener();
			window.close();
		});
	}

	// 閉じるボタン押下時の処理を書き換える
	OnLoadEvent = function () {
		document.getElementById('btnClose').onclick = function () {
			if (confirm('<%= WebMessages.GetMessages(WebMessages.CFMMSG_MANAGER_MESSAGE_POPUP_CLOSE_CONFIRM) %>'))
				window.close();
		}
	}
</script>
</asp:Content>
