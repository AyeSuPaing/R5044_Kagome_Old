<%--
=========================================================================================================
  Module      : メール配信文章登録ページ(MailDistTextRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MailDistTextRegister.aspx.cs" Inherits="Form_MailDistText_MailDistTextRegister" MaintainScrollPositionOnPostback="true"%>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<script type="text/javascript">
	// タグ差込
	function insertAtCaret(text, txtareaId)
	{
		var txtarea = document.getElementById(txtareaId);

		// IE10以下
		if (!window.getSelection)
		{
			var caretPos = txtarea.caretPos;
			caretPos.text = (caretPos.text.charAt(caretPos.text.length - 1) == ' ') ? text + ' ' : text;
		}
		// その他モダンブラウザ
		else if (typeof (txtarea.selectionStart) === 'number')
		{
			var scrollTop = txtarea.scrollTop;
		
			var preText = txtarea.value.slice(0, txtarea.selectionStart);
			var postText = txtarea.value.slice(txtarea.selectionEnd);

			txtarea.value = preText + text + postText;

			// キャレット位置を復元
			txtarea.selectionStart = (preText + text).length;
			txtarea.selectionEnd = txtarea.selectionStart;

			// スクロール位置を復元
			txtarea.scrollTop = scrollTop;
		}
		// 非対応ブラウザ
		else
		{
			txtarea.value = text + txtarea.value;
		}
	}

// IE用キャレット位置保存メソッド
	function storeCaret(txtareaID) 
	{
		var txtarea = document.getElementById(txtareaID)

		// IE10以下
		if (!window.getSelection)
			txtarea.caretPos = document.selection.createRange().duplicate();
	}

// IE用キャレット位置保存イベント登録
	function registerStoreCaretEvents(txtareaID)
	{
		var txtarea = document.getElementById(txtareaID);
	
		txtarea.onmouseup = function () {storeCaret(txtareaID);};
		txtarea.onkeyup = function () {storeCaret(txtareaID);};
	}

// IE用キャレット位置保存イベントを登録
	Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded( function ()
	{
		registerStoreCaretEvents('<%= tbMailtextSubjectMobile.ClientID %>');
		registerStoreCaretEvents('<%= tbMailtextMobile.ClientID %>');
	});

	$(function () {
		<%
		foreach (RepeaterItem ri in this.rSmsCarrier.Items)
		{
			var c = ri.FindControl("tbSmsText");
			if (c != null)
			{
				this.Response.Write("set_text_count('" + c.ClientID + "',70,true);");
			}
		}
	%>
	});

// 文字数警告文表示
	$(function () {
		<%
		foreach (RepeaterItem ri in rLineDirectContents.Items)
		{
			var c = ri.FindControl("tbLineText");
			if (c != null)
			{
				this.Response.Write("set_text_count_alert('" + c.ClientID + "', '-1', '4800', '※メッセージ内容はテキストの置換後に最大文字数制限を超える可能性があります');");
			}
		}
	%>
	});

</script>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr>
		<td><h1 class="page-title">メール配信文章設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ タイトル △-->
	<!--▽ 編集入力 ▽-->
	<tr>
		<% if((m_strActionStatus == Constants.ACTION_STATUS_INSERT) || (m_strActionStatus == Constants.ACTION_STATUS_COPY_INSERT)) {%>
		<td><h2 class="cmn-hed-h2">メール配信文章設定登録</h2></td>
		<%} %>
		<% if(m_strActionStatus == Constants.ACTION_STATUS_UPDATE) {%>
		<td><h2 class="cmn-hed-h2">メール配信文章設定詳細</h2></td>
		<%} %>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td>
						<table class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td>
												<div class="action_part_top"><input onclick="Javascript:history.back()" type="button" value="  戻る  " />
												<asp:Button id="btnConfirm" runat="server" Text="  確認する  " onclick="btnConfirm_Click"></asp:Button>
												</div>
												<!--▽ 基本情報 ▽-->
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="edit_title_bg" align="center" colspan="2">基本情報</td>
													</tr>
													<tr id="trMailtextId" runat="server" visible="false">
														<td class="edit_title_bg" align="left" width="30%">メール文章ID</td>
														<td class="edit_item_bg" align="left">
															<asp:Literal id="lMailtextId" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">メール文章名<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox id="tbMailtextName" runat="server" MaxLength="30" Width="300"></asp:TextBox>
															<asp:Literal id="lMailtextName" runat="server" Visible="False"></asp:Literal>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">メールFROM名</td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox id="tbMailFromName" runat="server" MaxLength="50" Width="200"></asp:TextBox></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">メールFROM<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox id="tbMailFrom" runat="server" MaxLength="256" Width="200"></asp:TextBox></td>
													</tr>
													<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">言語コード<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:DropDownList ID="ddlLanguageCode" runat="server"></asp:DropDownList>
														</td>
													</tr>
													<% } %>
												</table>
												<!--△ 基本情報 △-->
												<br />
												<!--▽ 内容 ▽-->
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="edit_title_bg" align="center" colspan="2">内容<span class="notice">*</span></td>
													</tr>												
													<tr>
														<td class="edit_title_bg" align="left" width="30%">メールタイトル</td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox id="tbMailtextSubject" runat="server" MaxLength="50" Width="300"></asp:TextBox>
															<p><input type= "button" onclick="javascript:open_window('<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MAILDIST_TEXT_TAGMANUAL + "?" + Constants.REQUEST_KEY_MOBILEPICTORIALSYMBOL_INSERTTO + "=" + tbMailtextSubjectMobile.ClientID) %>','contact','width=850,height=500,top=120,left=420,status=NO,scrollbars=yes');" value="メール配信文章タグマニュアル" /></p>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">メール文章テキスト</td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox id="tbMailtextBody" Runat="server" TextMode="MultiLine" Rows="10" Width="500"></asp:TextBox><br /></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">メール文章HTML</td>
														<td class="edit_item_bg" align="left">
															<asp:RadioButtonList id="rblSendHtmlFlg" Runat="server" RepeatDirection="Horizontal" AutoPostBack="True" CssClass="radio_button_list" onselectedindexchanged="rblSendHtmlFlg_SelectedIndexChanged">
																<asp:ListItem>有効</asp:ListItem>
																<asp:ListItem Selected="True">無効</asp:ListItem>
															</asp:RadioButtonList>
															<span id="spMailtextHtml" runat="server">
																<asp:TextBox id="tbMailtextHtml" Runat="server" TextMode="MultiLine" Rows="10" Width="500"></asp:TextBox><br />
																<%= WebSanitizer.HtmlEncode("<META http-equiv=Content-Type content=\"text/html; charset=" + Constants.PC_MAIL_DEFAULT_ENCODING_STRING +"\">") %>
																を必ず指定してください。
															</span>
														</td>
													</tr>
													<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
													<div id="divMobileSetting" runat="server">
													<tr>
														<td class="edit_title_bg_dark" align="left" width="30%">モバイルメールタイトル</td>
														<td class="edit_item_bg_dark" align="left">
															<asp:TextBox id=tbMailtextSubjectMobile runat="server" TextMode="SingleLine" MaxLength="500" Width="500" ></asp:TextBox>
															<p><input type= "button" onclick="javascript:open_window('<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MOBILEPICTORIALSYMBOL_POPUPLIST + "?" + Constants.REQUEST_KEY_MOBILEPICTORIALSYMBOL_INSERTTO + "=" + tbMailtextSubjectMobile.ClientID) %>','contact','width=828,height=500,top=120,left=420,status=NO,scrollbars=yes');" value="  絵文字入力  " /></p>	
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg_dark" align="left" width="30%">モバイルメール文章</td>
														<td class="edit_item_bg_dark" align="left">
															<asp:TextBox id="tbMailtextMobile" Runat="server" TextMode="MultiLine" Rows="10" Width="500" ></asp:TextBox>
															<p><input type= "button" onclick="javascript:open_window('<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MOBILEPICTORIALSYMBOL_POPUPLIST + "?" + Constants.REQUEST_KEY_MOBILEPICTORIALSYMBOL_INSERTTO + "=" + tbMailtextMobile.ClientID) %>','contact','width=828,height=500,top=120,left=420,status=NO,scrollbars=yes');" value="  絵文字入力  " /></p>
														</td>
													</tr>
													</div>
													<% } %>
												</table>
												<br />
												<% if (Constants.GLOBAL_SMS_OPTION_ENABLED && (this.IsSelectedLanguageCode == false)) { %>
													<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0" id="sms">
														<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">SMS情報</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">SMSを送信する</td>
															<td class="edit_item_bg" align="left"><asp:CheckBox runat="server" id="chksms" AutoPostBack="True" OnCheckedChanged="chksms_CheckedChanged"/></td>
														</tr>
														<asp:Repeater Visible="False" runat="server" ID="rSmsCarrier" ItemType="w2.Domain.GlobalSMS.GlobalSMSDistTextModel">
															<ItemTemplate>
																<tr>
																	<td class="edit_title_bg" align="left" width="20%">SMSテキスト本文(<%#Item.PhoneCarrier %>)</td>
																	<td class="edit_item_bg" align="left">
																		<asp:TextBox ID="tbSmsText" Runat="server" TextMode="MultiLine" Width="595" Rows="10" MaxLength="70" Text="<%#Item.SmsText %>" />
																		<asp:HiddenField runat="server" id="hCarrier" Value="<%#Item.PhoneCarrier %>"/>
																	</td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
														</tbody>
													</table>
													<br />
												<% } %>
												<% if (w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED && (this.IsSelectedLanguageCode == false)) { %>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0" id="lineDirect">
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">LINE情報</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">LINEを送信する</td>
															<td class="edit_item_bg" align="left"><asp:CheckBox runat="server" id="chkline" OnCheckedChanged="chkline_CheckedChanged" AutoPostBack="True"/></td>
														</tr>
														<asp:Repeater Visible="False" runat="server" ID="rLineDirectContents" OnItemCommand="rLineDirectContents_ItemCommand" ItemType="w2.Domain.MessagingAppContents.MessagingAppContentsModel">
															<ItemTemplate>
																<tr>
																	<td class="edit_title_bg" align="left" width="20%">LINE配信内容(<%#: (Container.ItemIndex + 1) %>)</td>
																	<td class="edit_item_bg" align="left">
																		<asp:TextBox ID="tbLineText" Runat="server" TextMode="MultiLine" Width="595" Rows="10" Text="<%# Item.Contents %>" /><br/>
																		<asp:Button ID="btnAddLineContent" runat="server" Text="追加" Enabled="<%# (this.LineDirectSendMessageContents.Count < w2.App.Common.Line.Constants.LINE_DIRECT_MAX_MESSAGE_COUNT) %>" CommandName="add_line_content" CommandArgument="<%# Container.ItemIndex.ToString() %>" UseSubmitBehavior="false" />
																		<asp:Button ID="btnDeleteProduct" runat="server" Text="削除" Enabled="<%# (this.LineDirectSendMessageContents.Count != 1) %>" CommandName="delete_line_content" CommandArgument="<%# Container.ItemIndex.ToString() %>" UseSubmitBehavior="false" />
																	</td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
													</tbody>
												</table>
												<br />
												<% } %>
												<!--△ 内容 △-->
												<div class="action_part_bottom"><input onclick="Javascript:history.back()" type="button" value="  戻る  " />
													<asp:Button id="btnConfirm2" runat="server" Text="  確認する  " onclick="btnConfirm_Click"></asp:Button></div>
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
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<script type="text/javascript">
	$('#<%= ddlLanguageCode.ClientID %>').change(function() {
		var r = $('option:selected').val();
		if (r != '/') {
			$('#sms').hide();
			$('#<%= chksms.ClientID %>').attr('checked', false);
			$('#lineDirect').hide();
			$('#<%= chkline.ClientID %>').attr('checked', false);
		} else {
			$('#sms').show();
			$('#lineDirect').show();
		}
	});
</script>
</asp:Content>