<%--
=========================================================================================================
  Module      : メールテンプレート完了ページ(MailTemplateConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.Common.Web" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MailTemplateConfirm.aspx.cs" Inherits="Form_MailTemplate_MailTemplateConfirm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">メールテンプレート設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 詳細 ▽-->
	<tr id="trDetail" runat="server" Visible="false">
		<td><h1 class="cmn-hed-h2">メールテンプレート設定詳細</h1></td>
	</tr>
	<tr id="trConfirm" runat="server" Visible="false">
		<td><h1 class="cmn-hed-h2">メールテンプレート設定入力確認</h1></td>
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
												<div class="action_part_top" style="float: left;">
													<asp:DropDownList ID="ddlLanguageCode" runat="server" Visible="False" OnSelectedIndexChanged="ddlLanguageCode_OnSelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
												</div>
												<div class="action_part_top">
													<input type="button" id="btnGoBackTop" runat="server" Visible="False" value="  戻る  " onclick="Javascript: history.back();" />
													<asp:Button id="btnBackToListTop" runat="server" Text="  一覧へ戻る  " Visible="False" onclick="btnBackToListTop_Click"></asp:Button>
													<asp:Button id="btnEditTop" runat="server" Text="  編集する  " Visible="False" OnClick="btnEdit_Click" />
													<asp:Button id="btnCopyInsertTop" runat="server" Text="  コピー新規登録する  " Visible="False" OnClick="btnCopyInsert_Click" />
													<asp:Button id="btnDeleteTop" runat="server" Visible="False" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
													<asp:Button id="btnDeleteGlobalTop" runat="server" Visible="<%# isGlobalSetting(m_input.CreateModel()) %>" Text="  指定言語だけ削除する  " OnClick="btnDeleteGlobal_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
													<asp:Button id="btnInsertTop" runat="server" Text="  登録する  " Visible="False" OnClick="btnInsert_Click" />
													<asp:Button id="btnUpdateTop" runat="server" Text="  更新する  " Visible="False" OnClick="btnUpdate_Click" />
													<asp:Button id="btnInsertGlobalTop" runat="server" Text="  他言語コードで登録する  " OnClick="btnInsertGlobal_Click" Visible="False" />
												</div>
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">基本情報</td>
													</tr>
													<tr id="trMailId" runat="server" Visible="False">
														<td class="detail_title_bg" align="left" width="30%">メールテンプレートID</td>
														<td class="detail_item_bg" align="left"><%#: m_input.MailId %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">メールテンプレート名</td>
														<td class="detail_item_bg" align="left"><%#: m_input.MailName %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">送信元メールアドレス</td>
														<td class="detail_item_bg" align="left"><%# this.EncodedDisplayMailFrom %>
															<asp:Label ID="lbMailFromErrorMessage" runat="server" ForeColor="red" Visible="False"/>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">送信元名</td>
														<td class="detail_item_bg" align="left"><%# this.EncodedDisplaySenderName %>
															<asp:Label ID="lbMailFromNameErrorMessage" runat="server" ForeColor="red" Visible="False"/>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">送信先メールアドレス</td>
														<td class="detail_item_bg" align="left"><%#: m_input.MailTo %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">Ccメールアドレス</td>
														<td class="detail_item_bg" align="left"><%#: m_input.MailCc %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">Bccメールアドレス</td>
														<td class="detail_item_bg" align="left"><%# this.EncodedDisplayMailBcc %>
															<asp:Label ID="lbMailBccErrorMessage" runat="server" ForeColor="red" Visible="False"/>
														</td>
													</tr>
													<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">言語コード</td>
														<td class="detail_item_bg" align="left">
															<%#: (string.IsNullOrEmpty(m_input.LanguageCode) == false)
																? string.Format("{0}({1})", m_input.LanguageCode, m_input.LanguageLocaleId)
																: ValueText.GetValueText(Constants.TABLE_MAILTEMPLATE, Constants.FIELD_MAILTEMPLATE_LANGUAGE_CODE, string.Empty) %>
														</td>
													</tr>
													<% } %>
													<% if (MailReplacementTag.HasInnternalElements(m_input.MailId) == false) { %>
														<tr>
															<td class="detail_title_bg" align="left" width="30%">使用用途</td>
															<td class="detail_item_bg" align="left"><%#: GetEmailUsageCategory() %></td>
														</tr>
													<% } %>
													<tr id="trDateCreated" runat="server" Visible="False">
														<td class="detail_title_bg" align="left" width="30%">作成日</td>
														<td class="detail_item_bg" align="left"><%#: DateTimeUtility.ToStringForManager(m_input.DateCreated, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
													</TR>
													<tr id="trDateChanged" runat="server" Visible="False">
														<td class="detail_title_bg" align="left" width="30%">更新日</td>
														<td class="detail_item_bg" align="left"><%#: DateTimeUtility.ToStringForManager(m_input.DateChanged, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
													</TR>
													<tr id="trLastChanged" runat="server" Visible="False">
														<td class="detail_title_bg" align="left" width="30%">最終更新者</td>
														<td class="detail_item_bg" align="left"><%#: m_input.LastChanged %></td>
													</TR>
													<tr runat="server" id ="trAutoSendFlg" visible="true">
														<td class="detail_title_bg" align="left" width="30%">自動送信フラグ</td>
														<td class="detail_item_bg" align="left"><%#: ValueText.GetValueText(Constants.TABLE_MAILTEMPLATE, Constants.FIELD_MAILTEMPLATE_AUTO_SEND_FLG, m_input.AutoSendFlg) %></td>
													</tr>
													</table>
													<br />
													<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">内容</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">メール件名</td>
														<td class="detail_item_bg" align="left"><%#: m_input.MailSubject %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">メール本文</td>
														<td class="detail_item_bg" align="left"><pre><%# WebSanitizer.HtmlEncodeChangeToBlank(m_input.MailBody) %></pre></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">メール本文HTML</td>
														<td class="detail_item_bg" align="left">
															<iframe class="HtmlPreview" src="<%: new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_HTML_PREVIEW_FORM).AddParam(Constants.HTML_PREVIEW_NO, "1").CreateUrl() %>" style="width: 100%; height: 100%;" frameborder="0" scrolling="no"></iframe>
														</td>
													</tr>
													</table>
													<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
													<br />
													<table id="trMobileSetting" runat="server" class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="mobile_title_bg" align="center" colspan="2">内容(モバイル)</td>
													</tr>
													<tr>
														<td class="mobile_title_bg" align="left" width="30%">メール件名(モバイル)</td>
														<td class="mobile_item_bg" align="left"><%#: m_input.MailSubjectMobile %></td>
													</tr>
													<tr>
														<td class="mobile_title_bg" align="left" width="30%">メール本文(モバイル)</td>
														<td class="mobile_item_bg" align="left"><pre><%# WebSanitizer.HtmlEncodeChangeToBlank(m_input.MailBodyMobile) %></pre></td>
													</tr>
													</table>
													<% } %>
												<br/>
												<% if (Constants.GLOBAL_SMS_OPTION_ENABLED && (this.IsSelectedLanguageCode == false)) { %>
													<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">SMS情報</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">SMSを送信する</td>
															<td class="edit_item_bg" align="left">
																<%#: m_input.SmsUseFlg == w2.Domain.MailTemplate.MailTemplateModel.SMS_USE_FLG_OFF ? "送信しない" : "送信する" %>
															</td>
														</tr>
														<asp:Repeater Visible="<%# m_input.SmsUseFlg == w2.Domain.MailTemplate.MailTemplateModel.SMS_USE_FLG_ON %>" runat="server" ID="rSmsCarrier" ItemType="w2.Domain.GlobalSMS.GlobalSMSTemplateModel">
															<ItemTemplate>
																<tr>
																	<td class="edit_title_bg" align="left" width="20%">SMSテキスト本文(<%#Item.PhoneCarrier %>)</td>
																	<td class="edit_item_bg" align="left">
																		<pre><%# WebSanitizer.HtmlEncodeChangeToBlank(Item.SmsText) %></pre>
																	</td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
														</tbody>
													</table>
													<br/>
												<% } %>
												<!-- リピートPLUSからのLINE配信機能 -->
												<% if (w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED && (this.IsSelectedLanguageCode == false)) { %>
													<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tbody>
															<tr>
																<td class="edit_title_bg" align="center" colspan="2">LINE情報</td>
															</tr>
															<tr>
																<td class="edit_title_bg" align="left" width="30%">LINEを送信する</td>
																<td class="edit_item_bg" align="left">
																	<%#: m_input.LineUseFlg == w2.Domain.MailTemplate.MailTemplateModel.LINE_USE_FLG_OFF ? "送信しない" : "送信する" %>
																</td>
															</tr>
															<asp:Repeater Visible="<%# m_input.LineUseFlg == w2.Domain.MailTemplate.MailTemplateModel.LINE_USE_FLG_ON %>" runat="server" ID="rLineDirectContents" ItemType="w2.Domain.MessagingAppContents.MessagingAppContentsModel">
																<ItemTemplate>
																	<tr>
																		<td class="edit_title_bg" align="left" width="20%">LINE送信内容(<%# (Container.ItemIndex + 1) %>)</td>
																		<td class="edit_item_bg" align="left">
																			<pre><%# WebSanitizer.HtmlEncodeChangeToBlank(Item.Contents) %></pre>
																		</td>
																	</tr>
																</ItemTemplate>
															</asp:Repeater>
														</tbody>
													</table>
													<br/>
												<!-- リピートライン機能 -->
												<% } else if (Constants.REPEATLINE_OPTION_ENABLED == Constants.RepeatLineOption.CooperationAndMessaging && this.LineDispFlg) { %>
													<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">LINE情報</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">LINE連携</td>
															<td class="detail_item_bg" align="left">
																<%#: ValueText.GetValueText(Constants.TABLE_MAILTEMPLATE, Constants.FIELD_MAILTEMPLATE_LINE_USE_FLG, m_input.LineUseFlg) %>
															</td>
														</tr>
														</tbody>
													</table>
													<br/>
												<% } %>
												<div class="action_part_bottom"><input type="button" id="btnGoBackBottom" runat="server" Visible="False" value="  戻る  " onclick="Javascript:history.back();" />
													<asp:Button id="btnBackToListBottom" runat="server" Text="  一覧へ戻る  " Visible="False" onclick="btnBackToListTop_Click"></asp:Button>
													<asp:Button id="btnEditBottom" runat="server" Text="  編集する  " Visible="False" OnClick="btnEdit_Click" />
													<asp:Button id="btnCopyInsertBottom" runat="server" Text="  コピー新規登録する  " Visible="False" OnClick="btnCopyInsert_Click" />
													<asp:Button id="btnDeleteBottom" runat="server" Visible="False" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
													<asp:Button id="btnDeleteGlobalBottom" runat="server" Visible="<%# isGlobalSetting(m_input.CreateModel()) %>" Text="  指定言語だけ削除する  " OnClick="btnDeleteGlobal_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
													<asp:Button id="btnInsertBottom" runat="server" Text="  登録する  " Visible="False" OnClick="btnInsert_Click" />
													<asp:Button id="btnUpdateBottom" runat="server" Text="  更新する  " Visible="False" OnClick="btnUpdate_Click" />
													<asp:Button id="btnInsertGlobalBottom" runat="server" Text="  他言語コードで登録する  " Visible="false" OnClick="btnInsertGlobal_Click" />
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
	<!--△ 詳細 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<script type="text/javascript">
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
