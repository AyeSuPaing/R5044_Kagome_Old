<%--
=========================================================================================================
  Module      : インシデント・メッセージパーツユーザーコントロール(IncidentAndMessageParts.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Cs.Message" %>
<%@ Import Namespace="w2.App.Common.Cs.SummarySetting" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="IncidentAndMessageParts.ascx.cs" Inherits="Form_Top_IncidentAndMessageParts" %>
<%@ Register TagPrefix="uc" TagName="ErrorPoint" Src="~/Form/Top/ErrorPointIcon.ascx" %>

<asp:LinkButton ID="lbRefreshIncident" runat="server" OnClick="lbRefreshIncident_Click"></asp:LinkButton>
<asp:HiddenField ID="hfOnfocusMessageListOnRefreshIncident" Value="0" runat="server" />

<div id="divContents" visible="false" runat="server">
<table width="100%"  border="0" cellspacing="0" cellpadding="0">
	<tr valign="top">


		<!-- 左エリア -->
		<td width="60%">
			<asp:UpdatePanel ID="up2" UpdateMode="Conditional" ChildrenAsTriggers="false" runat="server">
			<ContentTemplate>

			<div class="tab_title">
				<span class="tab_title_left">
					<% lbMessageMainTab.CssClass = (this.LeftDispMode == LeftDispModeType.MessageMain) ? "active" : ""; %>
					<% lbMessagePropertyTab.CssClass = (this.LeftDispMode == LeftDispModeType.MessageProperty) ? "active" : ""; %>
					<ol class="toc">
						<li><asp:LinkButton ID="lbMessageMainTab" runat="server" OnClick="lbMessageMainTab_Click"><span>メッセージ</span></asp:LinkButton></li>
						<li><asp:LinkButton ID="lbMessagePropertyTab" runat="server" OnClick="lbMessagePropertyTab_Click"><span>プロパティ</span></asp:LinkButton></li>
					</ol>
				</span>

				<div class="btn-group-message-info">
					<!-- 削除ボタン -->
					<% if (this.IsTrashableMessage) { %>
						<% if (this.IsNotReadonlyOperatorAndIncidentUnlocked && (this.IsUndeletedOtherOperatorMessage == false)) { %>
							<span class="tab_title_right btn-trash">
								<asp:LinkButton ID="lbTrashMessage" runat="server" OnClientClick="return confirm('メッセージをゴミ箱に移動します。よろしいですか？');" OnClick="lbTrashMessage_Click">
									<img src="../../Images/Cs/topmenu_trash.png" alt="削除" class="button" />
								</asp:LinkButton>
							</span>
						<% } else { %>
							<span class="tab_title_right btn-trash"><img src="../../Images/Cs/topmenu_trash_off.png" alt="削除" class="button" /></span>
						<% } %>
					<% } else if (this.IsDeletableMessage) { %>
						<span class="tab_title_right btn-trash"><asp:LinkButton ID="lbDeleteMessage" runat="server" OnClientClick="return confirm('メッセージを完全に削除します。よろしいですか？');" OnClick="lbDeleteMessage_Click"><img src="../../Images/Cs/topmenu_delete.png" alt="削除" class="button" /></asp:LinkButton></span>
					<% } %>

					<!-- 編集ボタン -->
					<% if (this.IsNotReadonlyOperatorAndIncidentUnlocked && this.IsEditableUncompleteMessage) { %>
						<span class="tab_title_right"><a href='Javascript:open_window("<%= CreateMessageInputUrl(hfMessageMode.Value, Constants.KBN_MESSAGE_EDIT_MODE_EDIT_DRAFT) %>", "message","width=1200,height=740,toolbar=1,resizable=1,status=1,scrollbars=1");'><img src="../../Images/Cs/topmenu_edit.png" alt="編集" class="button" /></a></span>
					<% } else if (this.IsNotReadonlyOperatorAndIncidentUnlocked && this.IsEditableSendRequestedMessage) { %>
						<span class="tab_title_right"><a href='Javascript:open_window("<%= CreateMessageInputUrl(hfMessageMode.Value, Constants.KBN_MESSAGE_EDIT_MODE_EDIT_FOR_SEND) %>", "message","width=1200,height=740,toolbar=1,resizable=1,status=1,scrollbars=1");'><img src="../../Images/Cs/topmenu_edit.png" alt="編集" class="button" /></a></span>
					<% } else if (this.IsNotReadonlyOperatorAndIncidentUnlocked && this.IsEditableDoneMessage) { %>
						<span class="tab_title_right"><a href='Javascript:open_window("<%= CreateMessageInputUrl(hfMessageMode.Value, Constants.KBN_MESSAGE_EDIT_MODE_EDIT_DONE) %>", "message","width=1200,height=740,toolbar=1,resizable=1,status=1,scrollbars=1");'><img src="../../Images/Cs/topmenu_edit.png" alt="編集" class="button" /></a></span>
					<% } else {%>
						<span class="tab_title_right"><img src="../../Images/Cs/topmenu_edit_off.png" alt="編集" class="button" /></span>
					<% } %>

					<!-- 印刷ボタン -->
					<% if (this.Message != null) {%>
						<span class="tab_title_right"><a href='Javascript:open_window("<%= CreatePrintUrl() %>", "message","width=1200,height=740,toolbar=1,resizable=1,status=1,scrollbars=1");'><img src="../../Images/Cs/topmenu_print.png" alt="印刷" class="button" /></a></span>
					<% } else { %>
						<span class="tab_title_right"><img src="../../Images/Cs/topmenu_print_off.png" alt="印刷" class="button" /></span>
					<% } %>

					<!-- 電話返信/メール返信ボタン -->
					<% if (this.IsNotReadonlyOperatorAndIncidentUnlocked && this.IsReplyableMessage) { %>
						<span class="tab_title_right"><a href='Javascript:open_window("<%= CreateMessageInputUrl(Constants.KBN_MESSAGE_MEDIA_MODE_TEL, Constants.KBN_MESSAGE_EDIT_MODE_REPLY) %>", "inquiry","width=1200,height=740,toolbar=1,resizable=1,status=1,scrollbars=1");'><img src="../../Images/Cs/topmenu_reply_tel.png" alt="電話返信" class="button" /></a></span>
						<span class="tab_title_right"><a href='Javascript:open_window("<%= CreateMessageInputUrl(Constants.KBN_MESSAGE_MEDIA_MODE_MAIL, Constants.KBN_MESSAGE_EDIT_MODE_REPLY) %>", "inquiry","width=1200,height=740,toolbar=1,resizable=1,status=1,scrollbars=1");'><img src="../../Images/Cs/topmenu_reply_mail.png" alt="メール返信" class="button" /></a></span>
					<% } else {%>
						<span class="tab_title_right"><img src="../../Images/Cs/topmenu_reply_tel_off.png" alt="電話返信" class="button" /></span>
						<span class="tab_title_right"><img src="../../Images/Cs/topmenu_reply_mail_off.png" alt="メール返信" class="button" /></span>
					<% } %>
				</div>
			</div>

				<!-- メッセージメインエリア -->
				<div id="divMessageMain" runat="server">

				<!-- メール表示エリア -->
				<div id="divMessageMail" class="datagrid larger" runat="server">
					<table>
					<tbody>
					<tr>
						<td>
							<div id="mailheader larger">
							<p>
								<strong>差出人：</strong>
								<asp:Literal ID="lMailFrom" runat="server"></asp:Literal><uc:ErrorPoint ID="ucErrorPointFrom" runat="server" /><br />
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
								<strong>　日時：</strong>
								<asp:Literal id="lMailDate" runat="server"></asp:Literal><br />
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
					<tr id="trMailMessageStatus" class="mailalert" runat="server">
						<td valign="bottom">

							<span class="tab_title_right img_align">
								<asp:Literal ID="lMailMessageStatus" runat="server"></asp:Literal>
								<span id="spApprovalCancelButtonArea" runat="server">
									<a href='Javascript:open_window("<%= CreateMailActionUrl(MailActionType.ApprovalCancel) %>", "mailaction","width=1200,height=740,toolbar=1,resizable=1,status=1,scrollbars=1");' class="cmn-btn-sub-action">
										取り下げ
									</a>
								</span>
								<span id="spApprovalOkNGButtonArea" runat="server">
									<a href='Javascript:open_window("<%= CreateMailActionUrl(MailActionType.ApprovalOK) %>", "mailaction","width=1200,height=740,toolbar=1,resizable=1,status=1,scrollbars=1");' class="cmn-btn-sub-action">
										承認
									</a>
									<a href='Javascript:open_window("<%= CreateMailActionUrl(MailActionType.ApprovalNG) %>", "mailaction","width=1200,height=740,toolbar=1,resizable=1,status=1,scrollbars=1");' class="cmn-btn-sub-action">
										差戻し
									</a>
								</span>
								<span id="spApprovalOkSendArea" runat="server">
									<a href='Javascript:open_window("<%= CreateMailActionUrl(MailActionType.ApprovalOKSend) %>", "mailaction","width=1200,height=740,toolbar=1,resizable=1,status=1,scrollbars=1");' class="cmn-btn-sub-action">
										送信
									</a>
								</span>
								<span id="spSendCancelButtonArea" runat="server">
									<a href='Javascript:open_window("<%= CreateMailActionUrl(MailActionType.SendCancel) %>", "mailaction","width=1200,height=740,toolbar=1,resizable=1,status=1,scrollbars=1");' class="cmn-btn-sub-action">
										取り下げ
									</a>
								</span>
								<span id="spSendButtonArea" runat="server">
									<a href='Javascript:open_window("<%= CreateMailActionUrl(MailActionType.SendNG) %>", "mailaction","width=1200,height=740,toolbar=1,resizable=1,status=1,scrollbars=1");' class="cmn-btn-sub-action">
										差戻し
									</a>
								</span>
							</span>
						</td>
					</tr>
					<tr>
						<td>
							<div class="y_scrollable2" style="HEIGHT:220px; width:100%">
								<div id="maildisp larger">
									<p>
										<asp:Literal ID="lMailBody" runat="server"></asp:Literal>
									</p>
								</div>
							</div>
							<div id="win-size-grip2"><img src ="../../Images/Cs/hsizegrip.png" ></div>
						</td>
					</tr>
					</tbody>
					</table>
				</div>

				<!-- 電話メッセージ表示エリア -->
				<div id="divMessageTel" class="datagrid larger" runat="server">
					<table>
					<tbody>
						<tr>
						<td width="20%" class="alt">問合せ日時</td>
						<td>
							<asp:Literal ID="lMessageInquiryDateTime" runat="server"></asp:Literal>
						</td>
						<td width="20%" class="alt">
							回答者
						</td>
						<td><asp:Literal ID="lMessageReplyOperatorName" runat="server"></asp:Literal></td>
						</tr>
						<tr>
						<td class="alt">氏名</td>
						<td width="30%"><asp:Literal ID="lMessageUserName" runat="server"></asp:Literal></td>
						<td width="20%" class="alt"><%: ReplaceTag("@@User.name_kana.name@@") %></td>
						<td><asp:Literal ID="lMessageUserNameKana" runat="server"></asp:Literal></td>
						</tr>
						<tr>
						<td class="alt">電話番号</td>
						<td><asp:Literal ID="lMessageUserTel" runat="server"></asp:Literal></td>
						<td class="alt">メール</td>
						<td><asp:Literal ID="lMessageUserMail" runat="server"></asp:Literal><uc:ErrorPoint ID="ucErrorPoint" runat="server" /></td>
						</tr>
						<tr>
						<td class="alt">件名</td>
						<td colspan="3"><asp:Literal ID="lMessageInquiryTitle" runat="server"></asp:Literal></td>
						</tr>
						<tr id="trTelMessageStatus" class="mailalert" runat="server">
							<td valign="bottom" colspan="4">
								<span class="tab_title_right img_align">
									<asp:Literal ID="lTelMessageStatus" runat="server"></asp:Literal>
								</span>
							</td>
						</tr>
						<tr>
						<td class="alt" colspan="4">内容</td>
						</tr>
						<tr>
						<td colspan="4"><asp:Literal ID="lMessageInquiryText" runat="server"></asp:Literal></td>
						</tr>
						<tr>
						<td class="alt" colspan="4">回答</td>
						</tr>
						<tr>
						<td colspan="4"><asp:Literal ID="lMessageReplyText" runat="server"></asp:Literal></td>
						</tr>
					</tbody>
					</table>
				</div>

				</div>

				<!-- メッセージプロパティエリア -->
				<div id="divMessageProperty" runat="server">

					<div class="datagrid larger">
					<table>
						<thead>
						<tr><th colspan="5">インシデント付け替え</th></tr>
						</thead>
						<tr>
							<td width="20%" class="alt">インシデントID</td>
							<td width="80%" colspan="3">
								<asp:TextBox ID="tbMessageIncidentId" runat="server"></asp:TextBox>
								<% btnUpdateMessageIncidentId.Enabled = (this.LoginOperatorCsInfo.EX_PermitEditFlg && (this.IsIncidentLocked == false)); %>
								<asp:Button id="btnUpdateMessageIncidentId" Text="  更新  " runat="server" OnClick="btnUpdateMessageIncidentId_Click" /><br />
								※空で更新した場合は新しいインシデントが作成されます。
								<div><asp:Label ID="lUpdateMessageIncidentIdMessage" Font-Bold="true" EnableViewState="false" runat="server"></asp:Label></div>
							</td>
						</tr>
					</table>
					</div>
					<br />

					<!-- 承認依頼 -->
					<div id="divMessagePropertyRequest" class="datagrid larger" visible="false" runat="server">
						<asp:Repeater ID="rMessageRequestHistoryList" ItemType="w2.App.Common.Cs.Message.CsMessageRequestModel" runat="server">
						<ItemTemplate>
							<table>
							<thead>
							<tr><th colspan="4">依頼情報<%# WebSanitizer.HtmlEncode(((CsMessageRequestModel)Container.DataItem).RequestNo) %></th></tr>
							</thead>
							<tr>
								<td class="alt">依頼者</td>
								<td colspan="3"><%# WebSanitizer.HtmlEncode(((CsMessageRequestModel)Container.DataItem).EX_RequestOperatorName) %></td>
							</tr>
							<tr>
								<td class="alt">依頼日時</td>
								<td colspan="3"><%#: DateTimeUtility.ToStringForManager(Item.DateCreated, DateTimeUtility.FormatType.ShortDateHourMinute2Letter) %></td>
							</tr>
							<tr>
								<td class="alt">緊急度</td>
								<td colspan="3">
									<%# (((CsMessageRequestModel)Container.DataItem).UrgencyFlg == Constants.FLG_CSMESSAGEREQUEST_URGENCY_URGENT) ? "[緊急]" : "-" %>
								</td>
							</tr>
							<tr id="trApprovalType" visible="<%# (((CsMessageRequestModel)Container.DataItem).RequestType == Constants.FLG_CSMESSAGEREQUEST_REQUEST_TYPE_APPROVE) %>" runat="server">
								<td class="alt">承認方法</td>
								<td colspan="3"><%# WebSanitizer.HtmlEncode(((CsMessageRequestModel)Container.DataItem).EX_ApprovalTypeName) %></td>
							</tr>
							<tr>	
								<td class="alt">メッセージステータス</td>
								<td colspan="3">
									<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_CSMESSAGEREQUEST, Constants.FIELD_CSMESSAGEREQUEST_REQUEST_STATUS, ((CsMessageRequestModel)Container.DataItem).RequestStatus)) %>
								</td>
							</tr>
							<tr>
								<td colspan="4" class="alt">依頼コメント</td>
							</tr>
							<tr>
								<td colspan="4">
									<div style="margin:8px 0px 8px 0px">
									<%# WebSanitizer.HtmlEncodeChangeToBr(((CsMessageRequestModel)Container.DataItem).Comment) %>
									</div>
								</td>
							</tr>
							<tr>
								<td style="width:100px" class="alt">依頼先名</td>
								<td style="width:60px;text-align:center" class="alt">結果</td>
								<td style="width:100px;text-align:center" class="alt">日時</td>
								<td style="text-align:left"class="alt">結果理由</td>
							</tr>
							<asp:Repeater ID="rApprovalRequestItems" DataSource="<%# ((CsMessageRequestModel)Container.DataItem).EX_Items %>" ItemType="w2.App.Common.Cs.Message.CsMessageRequestItemModel" runat="server">
							<ItemTemplate>
							<tr>
								<td><%# WebSanitizer.HtmlEncode(((CsMessageRequestItemModel)Container.DataItem).EX_ApprOperatorName) %></td>
								<td style="text-align:center">
									<span style="<%# ((((CsMessageRequestModel)((RepeaterItem)Container.Parent.Parent).DataItem).RequestStatus != Constants.FLG_CSMESSAGEREQUEST_REQUEST_STATUS_APPROVE_REQ)
										&& ((CsMessageRequestModel)((RepeaterItem)Container.Parent.Parent).DataItem).RequestStatus != Constants.FLG_CSMESSAGEREQUEST_REQUEST_STATUS_SEND_REQ)
										&& (((CsMessageRequestItemModel)Container.DataItem).ResultStatus == Constants.FLG_CSMESSAGEREQUESTITEM_RESULT_STATUS_NONE) ? "text-decoration:line-through;" : "" %>">
										<%# WebSanitizer.HtmlEncode(((CsMessageRequestItemModel)Container.DataItem).EX_ResultStatusString) %>
									</span>
								</td>
								<td style="text-align:center">
									<%#: GetDateStatusChange(Item.DateStatusChanged) %>
								</td>
								<td><%# WebSanitizer.HtmlEncodeChangeToBr(((CsMessageRequestItemModel)Container.DataItem).Comment) %></td>
							</tr>
							</ItemTemplate>
							</asp:Repeater>
							</table>
						</ItemTemplate>
						<SeparatorTemplate><br /></SeparatorTemplate>
						</asp:Repeater>
						<br />
					</div>

					<% if ( this.LoginOperatorCsInfo.EX_PermitEditFlg == false) btnUntrashMessage.Enabled = false; %>
					<asp:Button ID="btnUntrashMessage" Text=" 元に戻す " runat="server" OnClick="btnUntrashMessage_Click" OnClientClick="return confirm('メッセージをゴミ箱から戻します。よろしいですか？')" />
					<asp:Label ID="lUntrashMessageErrorMessage" Font-Bold="true" Visible="false" runat="server">インシデントが削除されているため元に戻せません。</asp:Label>
				</div>

			<!-- メッセージ無しエラーメッセージ -->
			<div id="divMessageNone"  class="datagrid larger" runat="server">
				<table>
					<tr>
						<td width="100%" class="alt"><br/><p>メッセージが見つかりませんでした</p><br/></td>
					</tr>
				</table>
			</div>

			</ContentTemplate>
			<Triggers>
				<asp:AsyncPostBackTrigger ControlID="lbSelectMessage" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="lbMessageMainTab" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="lbMessagePropertyTab" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="btnUnlock" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="btnUntrashMessage" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="btnUpdateMessageIncidentId" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="lbRefreshIncident" EventName="Click" />
			</Triggers>
			</asp:UpdatePanel>

		</td>

		<!-- 中央余白 -->
		<td><img alt="" src="../../Images/Common/sp.gif" border="0" height="5" width="5" /></td>

		<!-- インシデント右部分 -->
		<td width="40%">

			<asp:UpdatePanel ID="up3" UpdateMode="Conditional" ChildrenAsTriggers="false" runat="server">
			<ContentTemplate>

			<!-- インシデント選択タブエリア -->
			<div class="tab_title">
				<span class="tab_title_left">
					<% lbIncidentMainTab.CssClass = (this.RightDispMode == RightDispModeType.IncidentMain) ? "active" : ""; %>
					<% lbIncidentPropertyTab.CssClass = (this.RightDispMode == RightDispModeType.IncidentProperty) ? "active" : ""; %>
					<ol class="toc">
						<li><asp:LinkButton ID="lbIncidentMainTab" runat="server" OnClick="lbIncidentMainTab_Click"><span>インシデント</span></asp:LinkButton></li>
						<li><asp:LinkButton ID="lbIncidentPropertyTab" runat="server" OnClick="lbIncidentPropertyTab_Click"><span>プロパティ</span></asp:LinkButton></li>
					</ol>
				</span>
			</div>

		<div class="tab-contents-right">
			<div style="clear:left">
				<asp:Label ID="lLockMessage" Font-Bold="true" runat="server"></asp:Label>
			</div>

			<!-- インシデントメインエリア -->
			<div id="divIncidentMain" class="datagrid larger" runat="server">
				<table>
					<tr>
						<td width="30%" class="alt">インシデントID</td>
						<td width="70%" colspan="2">
							<span class="tab_title_left">
								<asp:Literal ID="lIncidentId1" runat="server"></asp:Literal>
									<img id="imgLockBottom_<%= hfIncidentId.Value %>" style="display:none" src="../../Images/Cs/icon_lock.png" alt="ロック" width="14" height="14" />
									<% btnUnlock.Enabled = this.LoginOperatorCsInfo.EX_PermitEditFlg; %>
									<asp:Button ID="btnUnlock" Text=" ロック解除 " runat="server" OnClick="btnUnlock_Click" OnClientClick="return confirm('ロックを解除します。よろしいですか？')" />
								<asp:Literal ID="lIncidentDeletedString1" runat="server">(削除済み)</asp:Literal>
							</span>
						</td>
					</tr>
					<tr>
						<td class="alt">ユーザーID</td>
						<td colspan="2">
							<asp:Literal ID="lUserId" runat="server"></asp:Literal>
							<input style="WIDTH: 80px; display:<%= String.IsNullOrEmpty(lUserId.Text) ? "none" : "inline" %>" class="button" onclick='Javascript:open_window("<%= Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MESSAGE_USER_SEARCH + "?" + Constants.REQUEST_KEY_USER_ID + "=" + HttpUtility.UrlEncode((this.Incident != null) ? Incident.UserId : "") %>    ", "message","width=1200,height=740,toolbar=1,resizable=1,status=1,scrollbars=1");' value="顧客検索" type="button" name="sample" /></td>
					</tr>
					<tr>
						<td class="alt">タイトル</td>
						<td colspan="2"><asp:Literal ID="lIncidentTitle" runat="server"></asp:Literal></td>
					</tr>
					<tr>
						<td class="alt">カテゴリ</td>
						<td colspan="2"><asp:Literal ID="lIncidentCategory" runat="server"></asp:Literal></td>
					</tr>
					<tr>
						<td class="alt">ステータス</td>
						<td colspan="2"><asp:Literal ID="lIncidentStatus" runat="server"></asp:Literal></td>
					</tr>
					<tr>
						<td class="alt">重要度</td>
						<td colspan="2"><asp:Literal ID="lIncidentImportance" runat="server"></asp:Literal></td>
					</tr>
					<tr>
						<td rowspan="2" class="alt">担当</td>
						<td colspan="2">グループ： <asp:Literal ID="lIncidentGroupName" runat="server"></asp:Literal></td>
					</tr>
					<tr>
						<td colspan="2">オペレータ： <asp:Literal ID="lIncidentOperatorName" runat="server"></asp:Literal></td>
					</tr>
					<tr>
						<td class="alt">内部メモ</td>
						<td colspan="2">
							<asp:TextBox ID="tbIncidentComment" Rows="6" Width="90%" TextMode="MultiLine" runat="server"></asp:TextBox>
							<asp:Button ID="btnUpdateIncidentMemo" Text="  更新する  " runat="server" OnClick="btnUpdateIncident_Click" />
							<asp:Label ID="lUpdateInternalNoteMessages" Font-Bold="true" EnableViewState="false" runat="server"></asp:Label>
						</td>
					</tr>
				</table>
				<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /><br />

				<asp:LinkButton ID="lbSelectMessage" runat="server" OnClick="lbSelectMessage_Click"></asp:LinkButton>
				<asp:HiddenField ID="hfMessageNo" runat="server" />
				<asp:HiddenField ID="hfMessageMode" runat="server" />
				<asp:HiddenField ID="hfRequestNo" runat="server" />
				<asp:HiddenField ID="hfBranchNo" runat="server" />

				<div class="tab-contents-right incident-and-message">
					<div>
						<table style="height:40px; border:hidden;">
							<tr style="text-align:center;">
								<td class="alt" style="width:150px;">
									<asp:LinkButton runat="server" ID="lbDateSendOrReceive" OnClick="MessageRightListSort_Click" CommandArgument="DateSendOrReceive" Width="100%" Font-Underline="false">送受信日時
										<span style="float:right"><asp:Label runat="server" ID="lDateSendOrReceiveIconSort"></asp:Label></span>
									</asp:LinkButton>
								</td>
								<td style="width:150px; text-align:center; padding:0px;">
									<table style="border:hidden;">
										<tr style="text-align:center;">
											<td class="alt">
												<asp:LinkButton runat="server" ID="lbInquiryTitle" OnClick="MessageRightListSort_Click" CommandArgument="InquiryTitle" Width="100%" Font-Underline="false">件名
													<span style="float:right"><asp:Label runat="server" ID="lInquiryTitleIconSort"></asp:Label></span>
												</asp:LinkButton>
											</td>
										</tr>
										<tr style="text-align:center;">
											<td class="alt">
												<asp:LinkButton runat="server" ID="lbOperatorName" OnClick="MessageRightListSort_Click" CommandArgument="OperatorName" Width="100%" Font-Underline="false">作成者
													<span style="float:right"><asp:Label runat="server" ID="lOperatorNameIconSort"></asp:Label></span>
												</asp:LinkButton>
											</td>
										</tr>
									</table>
								</td>
								<td class="alt">
									<asp:LinkButton runat="server" ID="lbReplyChangedDate" OnClick="MessageRightListSort_Click" CommandArgument="ReplyChangedDate" Width="100%" Font-Underline="false">作成日
										<span style="float:right"><asp:Label runat="server" ID="lReplyChangedDateIconSort"></asp:Label></span>
									</asp:LinkButton>
								</td>
							</tr>
						</table>
					</div>
					<div class="y_scrollable" style="HEIGHT:160px; border:hidden;">
						<div class="dataresult" style="margin-right:0px;">
							<table id="bottom_message_list">
								<asp:Repeater id="rIncidentMessages" ItemType="w2.App.Common.Cs.Message.CsMessageModel" runat="server">
								<ItemTemplate>
									<tr style="text-align:center;" id="bottom_message_tr<%# ((CsMessageModel)Container.DataItem).MessageNo %>" class="<%# "oplist_item_bg" + (Container.ItemIndex % 2 + 1) %>" onmouseover="listselect_mover_csinquiry(this)" onmouseout="listselect_mout_csinquiry(this, <%# Container.ItemIndex % 2 %>)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick_message(this, <%# Container.ItemIndex % 2 %>, '<%# ((CsMessageModel)Container.DataItem).MessageNo %>')">
										<td style="white-space: nowrap; text-align:center; width:150px;">
											<a id="bottom_message_a<%# ((CsMessageModel)Container.DataItem).MessageNo %>" href="#"></a>
											<%#: DateTimeUtility.ToStringForManager(Item.EX_DateSendOrReceive, DateTimeUtility.FormatType.ShortDateHourMinute2Letter) %>
										</td>
										<td style="width:150px; padding:0px;">
											<table>
												<tr style="border-right:hidden; text-align:center;">
													<td title="<%#: ((CsMessageModel)Container.DataItem).GetMessageComment() %>">
														<span class="img_align"><img src="../../Images/Cs/<%# ((CsMessageModel)Container.DataItem).GetMessageIcon() %>" alt="mail" /></span>
														<%# WebSanitizer.HtmlEncode(((CsMessageModel)Container.DataItem).InquiryTitle) %>
														<span id="spUrgent" visible="<%# ((CsMessageModel)Container.DataItem).EX_UrgencyFlg == Constants.FLG_CSMESSAGEREQUEST_URGENCY_URGENT %>" class="notice" runat="server">*</span>
													</td>
												</tr>
												<tr style="border-right:hidden; border-bottom:hidden; text-align:center;">
													<td>
														<%# WebSanitizer.HtmlEncode(string.IsNullOrEmpty(((CsMessageModel)Container.DataItem).EX_OperatorName) ? "-" : ((CsMessageModel)Container.DataItem).EX_OperatorName) %>
													</td>
												</tr>
											</table>
										</td>
										<td style="white-space: nowrap;">
											<%#: DateTimeUtility.ToStringForManager(Item.EX_ReplyChangedDate, DateTimeUtility.FormatType.ShortDateHourMinute2Letter) %>
										</td>
									</tr>
								</ItemTemplate>
								</asp:Repeater>
							</table>
						</div>
					</div>
					<div id="win-size-grip3"><img src ="../../Images/Cs/hsizegrip.png" ></div>
				</div>
			</div>

			<!-- インシデントプロパティエリア -->
			<div id="divIncidentProperty" class="datagrid larger" runat="server">
				<table>
				<tbody>
					<tr>
						<td width="30%" class="alt">インシデントID</td>
						<td width="70%">
							<asp:Literal ID="lIncidentId2" runat="server"></asp:Literal>
							<asp:HiddenField ID="hfIncidentId"  runat="server" />
							<span class="img_align">
								<img id="imgLock2" visible="false" src="../../Images/Cs/icon_lock.png" alt="ロック" width="14" height="14" runat="server" />
							</span>
							<asp:Literal ID="lIncidentDeletedString2" runat="server">(削除済み)</asp:Literal>
						</td>
					</tr>
					<tr>
						<td class="alt">ユーザーID</td>
						<td>
							<asp:TextBox ID="tbUserId" Width="90%" runat="server"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td class="alt">タイトル <span class="notice">*</span></td>
						<td>
							<asp:TextBox ID="tbIncidentTitle" Width="90%" runat="server"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td class="alt">カテゴリ</td>
						<td>
							<asp:DropDownList ID="ddlIncidentCategory" CssClass="select2-select" Width="90%" runat="server"></asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td class="alt">ステータス <span class="notice">*</span></td>
						<td>
							<asp:DropDownList ID="ddlIncidentStatus" Width="90%" runat="server"></asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td class="alt">重要度 <span class="notice">*</span></td>
						<td>
							<asp:DropDownList ID="ddlImportance" Width="90%" runat="server"></asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td class="alt">VOC</td>
						<td>
							<asp:DropDownList CssClass="select2-select" ID="ddlVoc" runat="server"></asp:DropDownList><br />
							<asp:TextBox id="tbVocMemo" Width="90%" runat="server"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td rowspan="2" class="alt">担当</td>
						<td><span style="width:110px;display:inline-block">グループ：</span><!--
							--><asp:DropDownList ID="ddlCsGroups" CssClass="select2-select" Width="65%" AutoPostBack="true" runat="server" OnSelectedIndexChanged="ddlCsGroup_SelectedIndexChanged"></asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td><span style="width:110px;display:inline-block">オペレータ：</span><!--
							--><asp:DropDownList ID="ddlCsOperators" CssClass="select2-select" Width="65%" runat="server"></asp:DropDownList>
							<asp:HiddenField ID="hfCsOperatorBefore" runat="server" />
							<asp:Button ID="btnSetOperatorAndGroup" Text="  担当を自分にセット  " runat="server" OnClick="btnSetOperatorAndGroup_Click" />
						</td>
					</tr>
					<asp:Repeater ID="rIncidentSummary" runat="server">
					<ItemTemplate>
						<tr>
							<td class="alt"><%# WebSanitizer.HtmlEncode(((CsSummarySettingModel)Container.DataItem).SummarySettingTitle) %></td>
							<td>
								<asp:HiddenField ID="hfSummaryNo" Value="<%# ((CsSummarySettingModel)Container.DataItem).SummarySettingNo %>" runat="server" />
								<asp:HiddenField ID="hfSummarySettingType" Value="<%# ((CsSummarySettingModel)Container.DataItem).SummarySettingType %>" runat="server" />
								<asp:HiddenField ID="hfSummarySettingTitle" Value="<%# ((CsSummarySettingModel)Container.DataItem).SummarySettingTitle %>" runat="server" />
								<asp:RadioButtonList ID="rblSummaryValue" CssClass="radio_button_list" RepeatLayout="Flow" RepeatDirection="Vertical" Visible="<%# ((CsSummarySettingModel)Container.DataItem).SummarySettingType == Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_RADIO %>" DataSource="<%# ((CsSummarySettingModel)Container.DataItem).EX_ListItems.Select(p => new ListItem(WebSanitizer.HtmlEncode(p.Text), p.Value)) %>" DataTextField="Text" DataValueField="Value" runat="server"  ></asp:RadioButtonList>
								<asp:DropDownList ID="ddlSummaryValue" CssClass ="select2-select" Visible="<%# ((CsSummarySettingModel)Container.DataItem).SummarySettingType == Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_DROPDOWN %>" DataSource="<%# ((CsSummarySettingModel)Container.DataItem).EX_ListItemsWithEmptyItem %>" DataTextField="Text" DataValueField="Value" runat="server"></asp:DropDownList>
								<asp:TextBox ID="tbSummaryValue" Width="90%" MaxLength="50" Visible="<%# ((CsSummarySettingModel)Container.DataItem).SummarySettingType == Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_TEXT %>" runat="server"></asp:TextBox>
							</td>
						</tr>
					</ItemTemplate>
					</asp:Repeater>
					<tr>
						<td class="alt">最終更新者</td>
						<td><asp:Literal ID="lIncidentLastChanged" runat="server"></asp:Literal></td>
					</tr>
				</tbody>
				</table>
				<asp:Label ID="lErrorMessages" CssClass="notice"  EnableViewState="false" runat="server"></asp:Label>
				<asp:Label ID="lCompleteMessages" Font-Bold="true" EnableViewState="false" runat="server"></asp:Label>

				<div style="WIDTH:99%;float:left;margin:4px 2px 2px 2px">
				<%
					btnUpdateIncident.Enabled
							= btnTrashIncident.Enabled
							= btnCloseIncident.Enabled
							= btnUntrashIncident.Enabled
							= btnDeleteIncident.Enabled
							= (this.LoginOperatorCsInfo.EX_PermitEditFlg && (this.IsIncidentLocked == false));
				%>
				<span id="spIncidentButtonAreaUndeleted" runat="server">
					<asp:Button ID="btnUpdateIncident" Text="  更新する  " runat="server" OnClick="btnUpdateIncident_Click" />
					<asp:Button ID="btnTrashIncident" Text="  削除する  " runat="server" OnClientClick="return confirm('インシデントとそれに紐付くメッセージを削除します。よろしいですか？');" OnClick="btnTrashIncident_Click" />
					<asp:Button ID="btnCloseIncident" Text="  このインシデントをクローズ  " runat="server" OnClick="btnCloseIncident_Click" />
				</span>
				<span id="spIncidentButtonAreaDeleted" runat="server">
					<asp:Button ID="btnUntrashIncident"  Text="  元に戻す  " runat="server" OnClientClick="return confirm('インシデントをゴミ箱から戻します。よろしいですか？\r\n（メッセージは復元されませんので。個別に元に戻す必要があります）');"  OnClick="btnUntrashIncident_Click" />
					<% if (this.LoginOperatorCsInfo.EX_PermitPermanentDeleteFlg) { %>
						<asp:Button ID="btnDeleteIncident" Text="  完全削除  " runat="server" 
							OnClientClick="return confirm('インシデントとそれに紐付くメッセージ、集計情報を完全削除します。よろしいですか？');" OnClick="btnDeleteIncident_Click" />
					<% } %>
				</span>
				</div>
				</div>
			</div>

			</ContentTemplate>
			<Triggers>
				<asp:AsyncPostBackTrigger ControlID="ddlCsGroups" EventName="SelectedIndexChanged" />
				<asp:AsyncPostBackTrigger ControlID="lbIncidentMainTab" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="lbIncidentPropertyTab" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="btnUpdateIncident" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="btnUpdateIncidentMemo" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="btnDeleteIncident" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="btnCloseIncident" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="btnUnlock" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="lbRefreshIncident" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="btnUpdateMessageIncidentId" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="btnSetOperatorAndGroup" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="lbDateSendOrReceive" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="lbReplyChangedDate" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="lbInquiryTitle" EventName="Click" />
				<asp:AsyncPostBackTrigger ControlID="lbOperatorName" EventName="Click" />
			</Triggers>
			</asp:UpdatePanel>
		</td>

		<!-- 右端余白 -->
		<td><img alt="" src="../../Images/Common/sp.gif" border="0" height="10" width="10" /></td>
	</tr>

	<tr><td><img alt="" src="../../Images/Common/sp.gif" border="0" height="30" width="10" /></td></tr>

</table>
</div>

<script type="text/jscript">
	// リスト選択用JS
	var selected_tr_bottom = "";
	var selected_before_style_bottom = "";
	function listselect_mover_csinquiry(obj)
	{
		if ((single_select == false) && (obj != selected_tr_bottom)) listselect_mover(obj);
	}
	function listselect_mout_csinquiry(obj, lineIdx)
	{
		if ((single_select == false) && (obj != selected_tr_bottom)) listselect_mout(obj, lineIdx);
	}
	function listselect_mclick_message(obj, lineIdx, messageNo)
	{
		listselect_mclick_message_inner(obj, messageNo, class_bgcolouts[lineIdx]);
	}
	function listselect_mclick_message_inner(obj, messageNo, class_bgcolout)
	{
		// クリック失敗なら抜ける
		if (listselect_mclick(obj, null, noClickCheck) == false) return;
		noClickCheck = false;

		// 以前の選択列の色を戻す
		if (obj != selected_tr_bottom) {
			selected_tr_bottom.className = selected_before_style_bottom;
			selected_tr_bottom = obj;
			selected_before_style_bottom = class_bgcolout;
		}
		
		// イベント実行
		document.getElementById('<%= hfMessageNo.ClientID %>').value = messageNo;
		__doPostBack("<%= lbSelectMessage.UniqueID %>", "");
	}

	// リスト自動選択用JS
	function select_bottom_message_list()
	{
		var hfMessageNo = document.getElementById("<%= hfMessageNo.ClientID %>");
		if (hfMessageNo == null) return;

		// クリックチェック解除
		noClickCheck = true;

		// 選択
		$("table#bottom_message_list tr#bottom_message_tr" + hfMessageNo.value).trigger("click");
		// フォーカス
		var hfOnfocusMessageListOnRefreshIncident = document.getElementById("<%= hfOnfocusMessageListOnRefreshIncident.ClientID %>");
		if (hfOnfocusMessageListOnRefreshIncident.value == "1" && "<%= Request[Constants.REQUEST_KEY_CUSTOMER_TELNO]%>" == "") {
			$("table#bottom_message_list a#bottom_message_a" + hfMessageNo.value).focus();
		}
		hfOnfocusMessageListOnRefreshIncident.value = "0";
	}

	// インシデントリフレッシュ
	function refresh_incident_info()
	{
		__doPostBack("<%= lbRefreshIncident.UniqueID %>", "");
	}

	// インシデントリフレッシュ時にメッセージリストにフォーカスするか
	function set_message_list_focus_on_refresh_incident(value)
	{
		document.getElementById("<%= hfOnfocusMessageListOnRefreshIncident.ClientID %>").value = value ? "1" : "0";
	}

	// ロックアイコン更新JS
	function refresh_incident_lock_icon(incident_id, visible)
	{
		var imgLock1 = document.getElementById("imgLockTop_" + incident_id);
		if (imgLock1) imgLock1.style.display = visible ? "inline" : "none";

		var imgLock2 = document.getElementById("imgLockBottom_" + incident_id);
		if (imgLock2) imgLock2.style.display = visible ? "inline" : "none";
		
		var lockFlg = document.getElementById("lockFlg_" + incident_id);
		if (lockFlg !== null) lockFlg.innerHTML = visible ? "False" : "True";
	}
</script>
