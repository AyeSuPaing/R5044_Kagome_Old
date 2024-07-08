<%--
=========================================================================================================
  Module      : メッセージページメールフォーム出力コントローラ(MessageRightMail.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Cs.Message" %>
<%@ Control	Language="C#" AutoEventWireup="true" CodeFile="MessageRightMail.ascx.cs" Inherits="Form_Message_MessageRightMail" %>

<asp:HiddenField ID="hfMailSignatureId" runat="server" />
<asp:LinkButton ID="lSetMailSignature" runat="server" OnClick="lSetMailSignature_Click"></asp:LinkButton>

<a id="aMailTitle" href="#" runat="server"></a>

<asp:Label ID="lErrorMessages" CssClass="notice" runat="server" />
<asp:Label ID="lAlreadyUser" CssClass="notice" runat="server" />
<div class="dataresult larger" id="sendmail">
	<a name="a_replymail_title" id="a_replymail_title"></a>

	<!-- メールフォーム -->
	<asp:UpdatePanel ID="up1" runat="server">
	<ContentTemplate>
	<div id="divMailForm" runat="server">
	<table>
		<tr>
			<td width="15%" class="alt">宛先</td>
			<td width="85%" colspan="2">
				<asp:TextBox ID="tbMailTo" Width="80%" runat="server" /><asp:Button ID="btnAddUser" Text="追加" runat="server" OnClick="btnAddUserMailAddress_OnClick" />
			</td>
		</tr>
		<asp:Repeater ID="rMailAddress" runat="server" DataSource="<%# this.UserMailAddressList %>" OnItemCommand="rUserMailAddress_OnItemCommand">
			<ItemTemplate>
				<tr>
					<td width="15%" class="alt">
						<asp:DropDownList ID="Addr" runat="server">
							<asp:ListItem Value="To">宛先</asp:ListItem>
							<asp:ListItem Value="Cc">Cc</asp:ListItem>
							<asp:ListItem Value="Bcc">Bcc</asp:ListItem>
						</asp:DropDownList>
					</td>
					<td width="85%" colspan="2">
						<asp:TextBox ID="tbMailAddress" width="80%" runat="server" Text="<%# Container.DataItem.ToString() %>" />
						<asp:Button ID="btnDeleteMailAddress" Text="削除" CommandArgument="<%# Container.ItemIndex %>" runat="server" />
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
		<tr>
			<td class="alt">Cc</td>
			<td colspan="2">
				<asp:TextBox ID="tbMailCc" Width="80%" runat="server"></asp:TextBox>
			</td>
		</tr>
		<tr id="trBcc" runat="server">
			<td class="alt">Bcc</td>
			<td colspan="2">
				<asp:TextBox ID="tbMailBcc" Width="80%" runat="server" />
			</td>
		</tr>
		<tr>
			<td class="alt">送信元<span class="notice">*</span></td>
			<td colspan="2" style="overflow: visible">
				<asp:DropDownList ID="ddlMailFrom" CssClass="select2-select" Width="80%" runat="server"/>
			</td>
		</tr>
		<tr>
			<td class="alt">件名<span class="notice">*</span></td>
			<td colspan="2">
				<span id="spTilteArea">
					<asp:TextBox id="tbMailSubject" Width="80%" MaxLength="100" runat="server"></asp:TextBox>
				</span>
			</td>
		</tr>
		<tr>
		<td class="alt">内容<span class="notice">*</span></td>
		<td colspan="2">
			<asp:HiddenField ID="hfReplySubject" runat="server" />
			<asp:HiddenField ID="hfReplyBody" runat="server" />
			<input type="button" value="  署名  " onclick="Javascript:open_window('<%= Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MESSAGE_MAIL_SIGNATURE_LIST %>', 'mailsignature','width=850,height=540,toolbar=1,resizable=1,status=1,scrollbars=1');" />
			<asp:Button ID="btnClear" Text=" クリア " runat="server" OnClick="btnClear_Click" />
			<asp:Button ID="btnSetQuotation" Text=" 引用 " Enabled="false" runat="server" OnClick="btnSetQuotation_Click" />
			<br />
			<asp:TextBox id="tbMailBody" Rows="30" Width="90%" TextMode="MultiLine" runat="server" CssClass="larger"></asp:TextBox><br />
		</td>
		</tr>
		<tr>
		<td class="alt">添付</td>
		<td colspan="2">
			<asp:Repeater ID="rAttachmentFiles" runat="server">
			<HeaderTemplate>
				<div style="WIDTH:100%;float:left;margin:2px 2px 2px 2px">
			</HeaderTemplate>
			<ItemTemplate>
				<div>
					<%# WebSanitizer.HtmlEncode(((CsMessageMailAttachmentModel)Container.DataItem).FileName) %>
					<asp:Button ID="btnDeleteAttachment" Text=" 削除 " CommandArgument="<%# Container.ItemIndex %>" runat="server" OnClick="btnDeleteAttachment_Click" OnClientClick="return confirm('添付を削除してもよろしいですか？')" /><br />
				</div>
			</ItemTemplate>
			<FooterTemplate>
				</div>
			</FooterTemplate>
			</asp:Repeater>
			<input type="button" value=" 追加選択 " onclick="javascript:open_window('<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MESSAGE_MAILATTACHMENTUPLOADER) %>	','fileupload','width=800,height=500,top=120,left=420,status=NO,scrollbars=yes');" />
			<asp:LinkButton ID="lbSetAttachmentFile" runat="server" OnClick="lbSetAttachmentFile_Click"></asp:LinkButton>
			<asp:HiddenField id="hfAttachmentFileNoForSet" runat="server" />
			<asp:HiddenField id="hfAttachmentFileNameForSet" runat="server" />
		</td>
		</tr>
	</table>
	</div>
	</ContentTemplate>
	</asp:UpdatePanel>

	<script type="text/javascript">
		// アップロードファイルセット
		function set_upload_file(filename, fileNo)
		{
			document.getElementById('<%= hfAttachmentFileNameForSet.ClientID %>').value = filename;
			document.getElementById('<%= hfAttachmentFileNoForSet.ClientID %>').value = fileNo;
			__doPostBack("<%= lbSetAttachmentFile.UniqueID %>", "");
		}

		// メール署名セット
		function set_signature(signature_id)
		{
			document.getElementById('<%= hfMailSignatureId.ClientID %>').value = signature_id;
			__doPostBack("<%= lSetMailSignature.UniqueID %>", "");
		}

		// 件名カウンターセット
		function set_mailsubject_testbox_count()
		{
			set_text_count('<%= tbMailSubject.ClientID %>', '50', true, '文字数');
		}

		$(function ()
		{
			set_mailsubject_testbox_count();

			// 入力フォームのみ更新の場合（入力エラーメッセージ表示など）、件名カウンターセット実行しないことの対策
			// PageRequestManagerクラスをインスタンス化
			var mng = Sys.WebForms.PageRequestManager.getInstance();
			// 非同期ポストバックの完了後に件名カウンターセット再実行
			mng.add_endRequest(
				function (sender, args) {
					set_mailsubject_testbox_count();
				}
			);
		});
	</script>
</div>
