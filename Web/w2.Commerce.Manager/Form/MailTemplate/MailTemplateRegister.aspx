<%--
=========================================================================================================
  Module      : メールテンプレート登録ページ(MailTemplateRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MailTemplateRegister.aspx.cs" Inherits="Form_MailTemplate_MailTemplateRegister" %>
<%@ Import Namespace="w2.Domain.MailTemplate" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
		<tr>
			<td><h1 class="page-title">メールテンプレート設定</h1></td>
		</tr>
		<tr>
			<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
		</tr>
		<!--▽ 登録 ▽-->
		<tr id="trRegister" runat="server" Visible="false">
			<td><h1 class="cmn-hed-h2">メールテンプレート設定登録</h1></td>
		</tr>
		<tr id="trEdit" runat="server" Visible="false">
			<td><h1 class="cmn-hed-h2">メールテンプレート設定編集</h1></td>
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
														<div class="action_part_top">
															<input type="button" value="  戻る  " onclick="Javascript: history.back();" />
															<asp:Button id="btnConfirmTop" OnClientClick="return validate()" runat="server" Text="  確認する  " OnClick="btnConfirm_Click" />
														</div>
														<asp:UpdatePanel runat="server" ChildrenAsTriggers="True">
															<ContentTemplate>
																<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
																	<tbody>
																		<tr>
																			<td class="edit_title_bg" align="center" colspan="2">基本情報</td>
																		</tr>
																		<tr id="trMailId" runat="server" Visible="False">
																			<td class="edit_title_bg" align="left" width="30%">メールテンプレートID</td>
																			<td class="edit_item_bg" align="left"><%# m_input.MailId %></td>
																		</tr>
																		<tr>
																			<td class="edit_title_bg" align="left" width="30%">メールテンプレート名<span class="notice">*</span></td>
																			<td class="edit_item_bg" align="left">
																				<asp:TextBox id=tbMailName runat="server" Text="<%# m_input.MailName %>" Width="300" MaxLength="50" />
																				<asp:Literal runat="server" id="lMailName" Text="<%# m_input.MailName %>" Visible="False" />
																			</td>
																		</tr>
																		<tr>
																			<td class="edit_title_bg" align="left" width="30%">送信元メールアドレス<span class="notice">*</span></td>
																			<td class="edit_item_bg" align="left"><asp:TextBox id=tbMailFrom runat="server" Text="<%# m_input.MailFrom %>" Width="500" MaxLength="256" /></td>
																		</tr>
																		<tr>
																			<td class="edit_title_bg" align="left" width="30%">送信元名</td>
																			<td class="edit_item_bg" align="left">
																				<asp:TextBox id=tbMailFromName runat="server" Text="<%# m_input.MailFromName %>" Width="500" MaxLength="50" />
																			</td>
																		</tr>
																		<tr>
																			<td class="edit_title_bg" align="left" width="30%">送信先メールアドレス</td>
																			<td class="edit_item_bg" align="left">
																				<asp:TextBox id=tbMailTo runat="server" Text="<%# m_input.MailTo %>" Width="500" MaxLength="1000" />
																			</td>
																		</tr>
																		<tr>
																			<td class="edit_title_bg" align="left" width="30%">Ccメールアドレス</td>
																			<td class="edit_item_bg" align="left">
																				<asp:TextBox id=tbMailCc runat="server" Text="<%# m_input.MailCc %>" Width="500" MaxLength="1000" />
																			</td>
																		</tr>
																		<tr>
																			<td class="edit_title_bg" align="left" width="30%">Bccメールアドレス</td>
																			<td class="edit_item_bg" align="left">
																				<asp:TextBox id=tbMailBcc runat="server" Text="<%# m_input.MailBcc %>" Width="500" MaxLength="1000"></asp:TextBox>
																			</td>
																		</tr>
																		<tr runat="server" id="trAutoSendFlg">
																			<td class="edit_title_bg" align="left" width="30%">自動送信フラグ</td>
																			<td class="edit_item_bg" align="left">
																				<asp:RadioButtonList ID="rblAutoSendFlg" RepeatDirection="Horizontal" RepeatLayout="Flow" runat="server"></asp:RadioButtonList>
																			</td>
																		</tr>
																		<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
																		<tr>
																			<td class="edit_title_bg" align="left" width="30%">言語コード</td>
																			<td class="edit_item_bg" align="left">
																				<asp:DropDownList ID="ddlLanguageCode" runat="server" />
																			</td>
																		</tr>
																		<% } %>
																		<tr>
																			<% if (MailReplacementTag.HasInnternalElements(m_input.MailId) == false) { %>
																			<td class="edit_title_bg" style="width: 30%">使用用途</td>
																			<td class="edit_item_bg">
																				<asp:DropDownList Runat="server" ID="ddlSelectTemplateTag" OnSelectedIndexChanged="ddlSelectTemplateTag_SelectedIndexChanged" AutoPostBack="True" AppendDataBoundItems="true" />
																			</td>
																			<% } %>
																		</tr>
																	</tbody>
																</table>
															</ContentTemplate>
														</asp:UpdatePanel>
														<br />

														<asp:UpdatePanel runat="server" ChildrenAsTriggers="True">
															<ContentTemplate>
																<asp:HiddenField runat="server" ID="hfTagList" />
																<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
																	<tbody>
																		<tr>
																			<td class="edit_title_bg" align="center" colspan="2">内容</td>
																		</tr>
																		<tr>
																			<td class="edit_title_bg" align="left" width="20%">メール件名</td>
																			<td class="edit_item_bg" align="left">
																				<asp:TextBox id="tbMailSubject" runat="server" Text="<%# m_input.MailSubject %>" Width="250" MaxLength="500" /><br />
																				<input type="button" id="btnInsertableReplacementTagList" value="  挿入可能置換タグ一覧  " /></td>
																		</tr>
																		<tr>
																			<td class="edit_title_bg" align="left" width="20%">メール本文</td>
																			<td class="edit_item_bg" align="left">
																				<asp:TextBox ID="tbMailBody" Runat="server" TextMode="MultiLine" Width="595" Rows="10" Text="<%# m_input.MailBody %>" />
																			</td>
																		</tr>
																		<tr>
																			<td class="edit_title_bg" align="left">メール本文HTML</td>
																			<td class="edit_item_bg" align="left">
																				<asp:RadioButtonList
																					ID="rbCheckHtml"
																					Runat="server"
																					Width="150"
																					RepeatDirection="Horizontal"
																					RepeatLayout="Flow"
																					SelectedValue="<%# this.SendHtmlFlg %>"
																					AutoPostBack="true"
																					CssClass="radio_button_list"
																					DataValueField="Value"
																					DataTextField="Text"
																					OnSelectedIndexChanged="rbCheckHtml_SelectedIndexChanged" />
																				<br/>
																				<% if (this.IsSendHtmlFlgOn) { %>
																					<input type="button" onclick="javascript:open_wysiwyg('<%= tbHtmlBody.ClientID %>', '<%= rbCheckHtml.ClientID %>');" value="  HTMLエディタ  " /><br />
																					<asp:TextBox id="tbHtmlBody" TextMode="MultiLine" Width="595" Rows="10" Text="<%# this.MailTextHtml %>" runat="server" /><br />
																					<span><%: "<META http-equiv=Content-Type content=\"text/html; charset=" + Constants.PC_MAIL_DEFAULT_ENCODING_STRING +"\">" %>を必ず指定してください。</span>
																				<% } %>
																			</td>
																		</tr>
																	</tbody>
																</table>
															</ContentTemplate>
														</asp:UpdatePanel>
														<br />
														<asp:UpdatePanel runat="server" ChildrenAsTriggers="True">
															<ContentTemplate>
																<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
																<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0" runat="server" id="tblMobileSetting" Visible="False">
																	<tbody>
																		<tr>
																			<td class="mobile_title_bg" align="center" colspan="2">内容(モバイル)</td>
																		</tr>
																		<tr>
																			<td class="mobile_title_bg" align="left" width="20%">メール件名(モバイル)</td>
																			<td class="mobile_item_bg" align="left">
																				<asp:TextBox id="tbMailSubjectMobile" runat="server" Text="<%# m_input.MailSubjectMobile %>" Width="250" MaxLength="500" />
																			</td>
																		</tr>
																		<tr>
																			<td class="mobile_title_bg" align="left" width="20%">メール本文(モバイル)</td>
																			<td class="mobile_item_bg" align="left">
																				<asp:TextBox ID="tbMailBodyMobile" Runat="server" TextMode="MultiLine" Width="595" Rows="10" Text="<%# m_input.MailBodyMobile %>" />
																			</td>
																		</tr>
																	</tbody>
																</table>
																<% } %>
															</ContentTemplate>
														</asp:UpdatePanel>
														<br />
														<asp:UpdatePanel runat="server" ChildrenAsTriggers="True">
															<ContentTemplate>
															<% if (Constants.GLOBAL_SMS_OPTION_ENABLED && (this.IsSelectedLanguageCode == false)) { %>
															<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0" id="sms">
																<tbody>
																	<tr>
																		<td class="edit_title_bg" align="center" colspan="2">SMS情報</td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="20%">SMSを送信する</td>
																		<td class="edit_item_bg" align="left"><asp:CheckBox runat="server" id="chksms" Checked="<%# m_input.SmsUseFlg == MailTemplateModel.SMS_USE_FLG_ON %>" OnCheckedChanged="chksms_CheckedChanged" AutoPostBack="True"/></td>
																	</tr>
																	<asp:Repeater Visible="False" runat="server" ID="rSmsCarrier" ItemType="w2.Domain.GlobalSMS.GlobalSMSTemplateModel">
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
															<!-- リピートPLUSからのLINE配信機能 -->
															<% if (w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED && (this.IsSelectedLanguageCode == false)) { %>
															<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0" id="lineDirect">
																<tbody>
																	<tr>
																		<td class="edit_title_bg" align="center" colspan="2">LINE情報</td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="20%">LINEを送信する</td>
																		<td class="edit_item_bg" align="left"><asp:CheckBox runat="server" id="chkline" Checked="<%# m_input.LineUseFlg == MailTemplateModel.LINE_USE_FLG_ON %>" OnCheckedChanged="chkline_CheckedChanged" AutoPostBack="True"/></td>
																	</tr>
																	<asp:Repeater Visible="False" runat="server" ID="rLineDirectContents" OnItemCommand="rLineDirectContents_ItemCommand" ItemType="w2.Domain.MessagingAppContents.MessagingAppContentsModel">
																		<ItemTemplate>
																			<tr>
																				<td class="edit_title_bg" align="left" width="20%">LINE配信内容(<%#: (Container.ItemIndex + 1) %>)</td>
																				<td class="edit_item_bg" align="left">
																					<asp:TextBox ID="tbLineText" Runat="server" TextMode="MultiLine" Width="595" Rows="10" Text="<%# Item.Contents %>" /><br/>
																					<asp:Button ID="btnAddLineContent" runat="server" Text="追加" Enabled="<%# (this.LineDirectSendMessageContents.Count < w2.App.Common.Line.Constants.LINE_DIRECT_MAX_MESSAGE_COUNT) %>" CommandName="add_line_content" CommandArgument="<%# Container.ItemIndex.ToString() %>" UseSubmitBehavior="false" />
																					<asp:Button ID="btnDeleteProduct" runat="server" Text="削除" Enabled="<%# (this.LineDirectSendMessageContents.Count != 1) %>" CommandName="delete_line_content" CommandArgument="<%# Container.ItemIndex.ToString() %>" UseSubmitBehavior="false" />
																					<asp:HiddenField runat="server" id="hMessageServiceName" Value="<%#Item.MessagingAppKbn %>"/>
																				</td>
																			</tr>
																		</ItemTemplate>
																	</asp:Repeater>
																</tbody>
															</table>
															<br />
															<!-- リピートライン機能 -->
															<% } else if (Constants.REPEATLINE_OPTION_ENABLED == Constants.RepeatLineOption.CooperationAndMessaging && this.LineDispFlg) { %>
															<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0" id="line">
																<tbody>
																	<tr>
																		<td class="edit_title_bg" align="center" colspan="2">LINE情報</td>
																	</tr>
																	<tr>
																		<td class="edit_title_bg" align="left" width="30%">LINE連携</td>
																		<td class="edit_item_bg" align="left">
																			<asp:RadioButtonList ID="rblSendLineFlg" RepeatDirection="Horizontal" RepeatLayout="Flow" runat="server"/>
																		</td>
																	</tr>
																</tbody>
															</table>
															<br />
															<% } %>
															</ContentTemplate>
														</asp:UpdatePanel>
														<div class="action_part_bottom">
															<input type="button" value="  戻る  " onclick="Javascript: history.back();" />
															<asp:Button id="btnConfirmBottom" runat="server" OnClientClick="return validate()" Text="  確認する  " OnClick="btnConfirm_Click" />
														</div>
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
			<!--△ 登録 △-->
			<tr>
				<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
			</tr>
		</table>
			<!--▽ 置換タグ一覧 ▽-->
			<div class="edit_title_bg" id="draggable_floatWindow" style="position:fixed; cursor: move; height: 200px; width: 480px;
				padding-right: 5px; padding-left: 5px; border: 1px solid #000000; border-radius: 10px; bottom: 110px; right: 40px; z-index: 100; display: none">
				<div id="resizable_floatWindow" style="height: 200px; width: 480px; padding-right: 5px; border-radius: 10px;">
					<asp:UpdatePanel runat="server" ChildrenAsTriggers="True">
						<ContentTemplate>
							<div style="text-align: left; float: left; font-size: 17px; font-weight: bold; padding-left: 5px; padding-top: 5px;">置換タグ一覧</div>
							<div style="text-align: right; padding-right: 5px; margin-left: auto;">
								<asp:TextBox runat="server" ID="tbTagSearch" placeholder="検索ワードを入力" />
								<span style="display: inline" class="replacementtag-close">×</span>
							</div>
							<div class="edit_title_bg">
								<table id="replacementtag" class="edit_table" style="table-layout: fixed;">
									<thead style="display: block;" class="edit_item_bg;">
										<tr>
											<th style="height: 20px; width: 220px">内容</th>
											<th style="height: 20px; width: 220px">置換タグ</th>
										</tr>
									</thead>
									<tbody style="display: block; overflow-y: scroll; max-height: 125px;">
										<asp:Repeater runat="server" ID="rReplacrmentTagList">
											<ItemTemplate>
												<tr onmouseover="listselect_mover(this)" onmouseout="listselect_mout1(this)" onmousedown="listselect_mdown1(this)" onclick="TableRowClick(this)">
													<td style="height: 20px; width: 220px"><%#: DataBinder.Eval(Container.DataItem,"Value") %></td>
													<td style="height: 20px; width: 220px"><%#: DataBinder.Eval(Container.DataItem,"Key") %></td>
												</tr>
											</ItemTemplate>
										</asp:Repeater>
									</tbody>
								</table>
							</div>
						</ContentTemplate>
					</asp:UpdatePanel>
				</div>
			</div>
													<!--△ 置換タグ一覧 △-->
	<script type="text/javascript">
			$(function () {
		<%
			foreach (RepeaterItem ri in this.rSmsCarrier.Items)
			{
				var c = ri.FindControl("tbSmsText");
				if (c != null)
				{
				this.Response.Write("set_text_count('" + c.ClientID + "','70',true,'文字カウント');");
				}
			}
		%>
			});

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

		function validate() {
			// Check if template tag is not specified then do not validate
			var templateTagSelected = $('#<%= ddlSelectTemplateTag.ClientID %>').val();
			if (templateTagSelected == '') return;

			var result;
			<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED){ %>
			result = MoveCheck(
				'<%= tbMailSubject.ClientID %>',
				'<%= tbMailBody.ClientID %>',
				'<%= tbMailSubjectMobile.ClientID %>',
				'<%= tbMailBodyMobile.ClientID %>',
				'<%= hfTagList.ClientID %>',
				'<%= tbHtmlBody.ClientID %>');
			<% } else{ %>
			result = MoveCheck(
				'<%= tbMailSubject.ClientID %>',
				'<%= tbMailBody.ClientID %>',
				'',
				'',
				'<%= hfTagList.ClientID %>',
				'<%= tbHtmlBody.ClientID %>');
			<% } %>
			return result;
		}

		function TableRowClick(obj) {
			var result;
			<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED){ %>
				result = listselect_mclick_Insert(
					obj,
					'<%= tbMailSubject.ClientID %>',
					'<%= tbMailBody.ClientID %>',
					'<%= tbMailSubjectMobile.ClientID %>',
					'<%= tbMailBodyMobile.ClientID %>',
					'<%= tbHtmlBody.ClientID %>');
			<% }
			else{ %>
				result = listselect_mclick_Insert(
				obj,
				'<%= tbMailSubject.ClientID %>',
				'<%= tbMailBody.ClientID %>',
				'',
				'',
				'<%= tbHtmlBody.ClientID %>');
			<% } %>
			return result;
		}

		$("#draggable_floatWindow").draggable();
		$("#resizable_floatWindow").resizable({
			minHeight: 110,
			minWidth: 400
		});

		function pageLoad() {

			$(function () {
				<%
			foreach (RepeaterItem ri in this.rSmsCarrier.Items)
			{
				var c = ri.FindControl("tbSmsText");
				if (c != null)
				{
					this.Response.Write("set_text_count('" + c.ClientID + "',70,true,'文字数')");
				}
			}
		%>
			});

			$(".replacementtag-close").click(function () {
				$("#draggable_floatWindow").hide();
				return false;
			});

			$("#btnInsertableReplacementTagList").click(function () {
				$("#draggable_floatWindow").show();
				$("#replacementtag thead th:first-child").css('width', $("#replacementtag tbody td:first-child").width() + 1);
				$("#replacementtag thead th:nth-child(2)").css('width', $("#replacementtag tbody td:nth-child(2)").width() + 1);
				return false;
			});

			//リアルタイム検索
			$('#<%= tbTagSearch.ClientID %>').keyup(function () {
				var re = new RegExp($('#<%= tbTagSearch.ClientID %>').val());
				$('#replacementtag tbody tr').each(function () {
					var txt = $(this).find("td:eq(0)").html() + $(this).find("td:eq(1)").html();
					console.log(txt);
					if (txt.match(re) != null) {
						$(this).show();
					} else {
						$(this).hide();
					}
				});
			});

			$('#<%= ddlLanguageCode.ClientID %>').change(function() {
				var r = $('option:selected').val();
				if (r != '/') {
					$('#sms').hide();
					$('#<%= chksms.ClientID %>').attr('checked', false);
					$('#lineDirect').hide();
					$('#<%= chkline.ClientID %>').attr('checked', false);
					$('#line').hide();
				} else {
					$('#sms').show();
					$('#lineDirect').show();
					$('#line').show();
				}
			});
		}

		//リサイズ
		$(function () {
			$("#resizable_floatWindow").resize(function () {
				$("#draggable_floatWindow").height($("#resizable_floatWindow").height());
				$("#draggable_floatWindow").width($("#resizable_floatWindow").width());
				$("#replacementtag tbody").css('max-height', $("#resizable_floatWindow").height() - 77);
				$("#replacementtag tbody td").css('width', ($("#resizable_floatWindow").width() - 10) / 2);
				$("#replacementtag thead th:first-child").css('width', $("#replacementtag tbody td:first-child").width() + 1);
				$("#replacementtag thead th:nth-child(2)").css('width', $("#replacementtag tbody td:nth-child(2)").width() + 1);
			});
		});

		// Wysiwygエディタを開く
		function open_wysiwyg(textAreaId, htmlTextKbn) {
			global_fullPageFlg = false;
			textAreaWysiwygBinded = document.getElementById(textAreaId);
			open_window('<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_WYSIWYG_EDITOR) %>', 'wysiwyg', 'width=900,height=740,top=120,left=420,status=NO,resizable=yes,scrollbars=yes');
			textAreaWysiwygBinded.setAttribute("disabled", "disabled");
			document.getElementById(htmlTextKbn).checked = true;
		}
	</script>
</asp:Content>
