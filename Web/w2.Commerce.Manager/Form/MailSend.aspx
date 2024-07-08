<%--
=========================================================================================================
  Module      : メール送信ページ(MailSend.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="w2.Common.Web" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="MailSend.aspx.cs" Inherits="Form_MailSend" Title="メール送信フォーム" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" border="0">
	<tr><td><h1 class="page-title">メール送信フォーム</h1></td></tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="628" border="0">
				<tr>
					<td>
						<table class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td>
												<!--▽ エラー・メール送信完了メッセージ ▽-->
												<div id="divMessages" runat="server" visible="false">
												<br />												
												<table class="detail_table" width="604" border="0" cellspacing="1" cellpadding="3">
													<tr>
														<td class="detail_item_bg" align="left">
															<asp:Label ID="lbMessages" runat="server" ForeColor="Red"></asp:Label>
														</td>
													</tr>
												</table>
												<br />
												</div>
												<!--△ エラー・メール送信完了メッセージ △-->

												<!--▽ 編集 ▽-->
												<div id="divEdit" runat="server" visible="false">
												<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
												<div class="action_part_top" style="float: left;">
													<asp:DropDownList ID="ddlLanguageCode" runat="server" OnSelectedIndexChanged="ddlLanguageCode_OnSelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
												</div>
												<% } %>
												<div class="action_part_top">
												<asp:Button ID="btnConfirmTop" runat="server" Text="  確認する  " onclick="btnConfirm_Click" />
												</div>
												<table class="edit_table" width="604" border="0" cellspacing="1" cellpadding="3">
													<tr>
														<td class="edit_title_bg" align="center" colspan="2">メール送信情報</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">メールテンプレート名</td>
														<td class="edit_item_bg" align="left"><asp:Literal id="lMailName" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">送信元メールアドレス<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left"><asp:TextBox id="tbMailFrom" runat="server" Width="400" MaxLength="256"></asp:TextBox></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="130">送信先メールアドレス<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left"><asp:TextBox id="tbMailTo" runat="server" Width="400" MaxLength="1000"></asp:TextBox></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">Ccメールアドレス</td>
														<td class="edit_item_bg" align="left"><asp:TextBox id="tbMailCc" runat="server" Width="400" MaxLength="1000"></asp:TextBox></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">Bccメールアドレス</td>
														<td class="edit_item_bg" align="left"><asp:TextBox id="tbMailBcc" runat="server" Width="400" MaxLength="1000"></asp:TextBox></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="130">メール件名</td>
														<td class="edit_item_bg" align="left"><asp:TextBox id="tbMailSubject" runat="server" Width="450" MaxLength="500"></asp:TextBox></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">メール本文</td>
														<td class="edit_item_bg" align="left"><asp:TextBox ID="tbMailBody" Runat="server" TextMode="MultiLine" Width="460" Rows="25"></asp:TextBox></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">メール本文HTML</td>
														<td class="edit_item_bg" align="left">
															<%if (this.CanSendHtml) { %>
																<input type= "button" onclick="javascript:open_wysiwyg('<%= tbMailBodyHtml.ClientID %>')" value="  HTMLエディタ  " /><br />
															<% } %>
															<asp:TextBox id="tbMailBodyHtml" Enabled="false" TextMode="MultiLine" Width="460" Rows="10" runat="server" />
														</td>
													</tr>
													<% if (Constants.GLOBAL_SMS_OPTION_ENABLED) { %>
													<tr>
														<td class="edit_title_bg" align="left">SMS本文</td>
														<td class="edit_item_bg" align="left"><asp:TextBox ID="tbSmsText" Runat="server" TextMode="MultiLine" Width="460" Rows="5"></asp:TextBox></td>
													</tr>
													<% } %>
													<% if (w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED) { %>
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">LINE送信情報</td>
														</tr>
														<asp:Repeater runat="server" ID="rLineDirectContentsModify">
															<ItemTemplate>
																<tr>
																	<td class="detail_title_bg" align="left" width="20%">LINE配信内容(<%#: (Container.ItemIndex + 1) %>)</td>
																	<td class="detail_item_bg" align="left">
																		<asp:TextBox ID="tbLineText" Runat="server" TextMode="MultiLine" Width="460" Rows="10" Text="<%# Container.DataItem %>" /><br/>
																	</td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
													<% } %>
												</table>
												<div class="action_part_bottom">
												<asp:Button ID="btnConfirmBottom" runat="server" Text="  確認する  " onclick="btnConfirm_Click" />
												</div>
												</div>
												<!--△ 編集 △-->

												<!--▽ 確認 ▽-->
												<div id="divConfirm" runat="server" visible="false">
												<div class="action_part_top">
												<asp:Button ID="btnCloseTop" runat="server" Text="  閉じる  " OnClientClick="Javascript:window.close();" Visible="false" />
												<asp:Button ID="btnBackTop" runat="server" Text="  戻る  " onclick="btnBack_Click" />
												<asp:Button ID="btnSendMailTop" runat="server" Text="  メール送信  " onclick="btnSendMail_Click" />
												</div>
													<table class="detail_table" width="604" border="0" cellspacing="1" cellpadding="3">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">メール送信情報</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">メールテンプレート名</td>
														<td class="detail_item_bg" align="left"><asp:Literal id="lMailNameConfirm" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">送信元メールアドレス</td>
														<td class="detail_item_bg" align="left"><asp:Literal id="lMailFrom" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="120">送信先メールアドレス</td>
														<td class="detail_item_bg" align="left"><asp:Literal id="lMailTo" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">Ccメールアドレス</td>
														<td class="detail_item_bg" align="left"><asp:Literal id="lMailCc" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">Bccメールアドレス</td>
														<td class="detail_item_bg" align="left"><asp:Literal id="lMailBcc" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="120">メール件名</td>
														<td class="detail_item_bg" align="left"><asp:Literal id="lMailSubject" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">メール本文</td>
														<td class="detail_item_bg" align="left"><asp:Literal id="lMailMailBody" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">メール本文HTML</td>
														<td class="detail_item_bg" align="left">
															<iframe class="HtmlPreview" src="<%: new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_HTML_PREVIEW_FORM).AddParam(Constants.HTML_PREVIEW_NO, "1").CreateUrl() %>" style="width: 100%; height: 100%;" frameborder="0" scrolling="no"></iframe>
														</td>
													</tr>
													<% if (Constants.GLOBAL_SMS_OPTION_ENABLED) { %>
													<tr>
														<td class="detail_title_bg" align="left">SMS本文</td>
														<td class="detail_item_bg" align="left"><asp:Literal id="lSmsText" runat="server"></asp:Literal></td>
													</tr>
													<% } %>
													<% if (w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED) { %>
														<tr>
															<td class="detail_title_bg" align="center" colspan="2">LINE送信情報</td>
														</tr>
														<asp:Repeater runat="server" ID="rLineDirectContents">
															<ItemTemplate>
																<tr>
																	<td class="detail_title_bg" align="left" width="20%">LINE配信内容(<%#: (Container.ItemIndex + 1) %>)</td>
																	<td class="detail_item_bg" align="left">
																		<asp:Literal id="lLineText" runat="server" Text="<%# Container.DataItem %>"/>
																	</td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
													<% } %>
												</table>
												<div class="action_part_bottom">
												<asp:Button ID="btnBackBottom" runat="server" Text="  戻る  " onclick="btnBack_Click" />
												<asp:Button ID="btnSendMailBottom" runat="server" Text="  メール送信  " onclick="btnSendMail_Click" />
												</div>
												</div>
												<!--△ 確認 △-->
												
												<!--▽ 該当データなし ▽-->
												<div id="divNoData" runat="server" visible="false">
												<br />
												<table class="error_table" width="500" border="0" cellspacing="1" cellpadding="3">
													<tr class="error_title_bg">
														<td align="left">下記の内容についてエラーが発生しました。</td>
													</tr>
													<tr class="error_item_bg">
														<td align="left">
															<div class="error_text">
																<%= WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL) %>
															</div>
														</td>
													</tr>
												</table>
												<br />
												</div>
												<!--△ 該当データなし △-->
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
	<!--△ 詳細 △-->
	<tr>
		<td><img height="10" alt="" src="../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<script type="text/javascript">
	$(function () {
		<%
		foreach (RepeaterItem ri in rLineDirectContentsModify.Items)
		{
			var c = ri.FindControl("tbLineText");
			if (c != null)
			{
				if (((TextBox)c).Enabled)
				{
					this.Response.Write("set_text_count_alert('" + c.ClientID + "', '0', '5001', '※メッセージ内容は文字数制限により送信されない可能性があります');");
				}
			}
		}
		%>
	});

	// Wysiwygエディタを開く
	function open_wysiwyg(textAreaId) {
		global_fullPageFlg = false;
		textAreaWysiwygBinded = document.getElementById(textAreaId);
		open_window('<%: new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_WYSIWYG_EDITOR).CreateUrl() %>', 'wysiwyg', 'width=900,height=740,top=120,left=420,status=NO, resizable=yes,scrollbars=yes');
		textAreaWysiwygBinded.setAttribute("disabled", "disabled");
	}

	// HTML文書表示用フレームのウィンドウ幅調整
	(function (window, $) {
		$(window).on('load', function () {
			$('iframe.HtmlPreview').each(function () {
				var doc = $(this).get(0).contentWindow.document;
				var innerHeight = Math.max(
					doc.body.scrollHeight, doc.documentElement.scrollHeight,
					doc.body.offsetHeight, doc.documentElement.offsetHeight,
					doc.body.clientHeight, doc.documentElement.clientHeight);
				$(this).removeAttr("height").css('height', innerHeight + 'px');
			});
		});
	})(window, jQuery);
</script>
</asp:Content>
