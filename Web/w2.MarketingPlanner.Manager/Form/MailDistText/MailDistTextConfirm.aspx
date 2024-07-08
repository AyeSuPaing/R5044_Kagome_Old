<%--
=========================================================================================================
  Module      : メール配信文章確認ページ(MailDistTextConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MailDistTextConfirm.aspx.cs" Inherits="Form_MailDistText_MailDistTextConfirm" %>
<%@ Import Namespace="w2.Common.Web" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<style type="text/css">
	.dvPreview img
	{
		vertical-align : text-bottom;
		max-width : 240px;
		_width:expression(this.clientWidth > 240 ? "240px" : this.clientWidth);	/* ie6でmax-widthを実現する */
	}
	
	.dvPreview
	{
		width : 240px;
		float : left;
		
		color : black;
		background-color : White;
		border : solid 1px black ;
		
		font-size : 20px;
		font-family : 'ＭＳ ゴシック';
		line-height : normal;
		word-wrap : break-word;
	}
</style>
<script type="text/javascript">
	(function(window, $) {
		$(window).bind('load', function() {
			$('iframe.HtmlPreview').each(function() {
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
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ タイトル ▽-->
	<tr>
		<td><h1 class="page-title">メール配信文章設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--△ タイトル △-->
	<!--▽ 詳細 ▽-->
	<tr>
		<% if (m_strActionStatus == Constants.ACTION_STATUS_DETAIL){%>
		<td><h2 class="cmn-hed-h2">メール配信文章設定詳細</h2></td>
		<%} %>
		<% if ((m_strActionStatus == Constants.ACTION_STATUS_INSERT) || (m_strActionStatus == Constants.ACTION_STATUS_UPDATE)){%>
		<td><h2 class="cmn-hed-h2">メール配信文章設定確認</h2></td>
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
												<div class="action_part_top" style="float: left;">
													<asp:DropDownList ID="ddlLanguageCode" runat="server" OnSelectedIndexChanged="ddlLanguageCode_SelectedIndexChanged"  AutoPostBack="True" Visible="False"></asp:DropDownList>
												</div>
												<div class="action_part_top">
													<input type="button" onclick="Javascript:history.back();" value="  戻る  " />
													<asp:Button id="btnEditTop" runat="server" Visible="false" Text="  編集する  " onclick="btnEdit_Click"></asp:Button>
													<asp:Button id="btnCopyInsertTop" runat="server" Text="  コピー新規登録する  " Visible="False" onclick="btnCopyInsert_Click"></asp:Button>
													<% btnMailClickSettingTop.OnClientClick = "javascript:open_window('" + GetMailClickPopupUrl() + "','contact','width=828,height=900,top=120,left=420,status=NO,scrollbars=yes'); return false;"; %>
													<asp:Button ID="btnMailClickSettingTop" Text="メールクリック設定" runat="server" />
													<asp:Button ID="btnDeleteTop" Runat="server" visible="false" Text="  削除する  " onclick="btnDelete_Click" OnClientClick="return confirm('情報を削除するとレポートで名称が取得できないなどの不整合が発生する可能性があります。\n削除してもよろしいですか？');"></asp:Button>
													<asp:Button id="btnUpdateTop" runat="server" Visible="false" Text="  更新する  " onclick="btnUpdate_Click"></asp:Button>
													<asp:Button id="btnInsertTop" runat="server" Visible="false" Text="  登録する  " onclick="btnInsert_Click"></asp:Button>
													<asp:Button id="btnInsertGlobalTop" runat="server" Text="  他言語コードで登録する  " OnClick="btnInsertGlobal_Click" Visible="False" />
												</div>
												<!--▽ 基本情報 ▽-->
												<table class="detail_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr>
														<td align="center" class="detail_title_bg" colspan="2">基本情報</td>
													</tr>													
													<tr id="trMailtextId" runat="server">
														<td align="left" class="detail_title_bg" width="30%">メール文章ID</td>
														<td align="left" class="detail_item_bg">
															<asp:Literal id="lMailtextId" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg" width="30%">メール文章名</td>
														<td align="left" class="detail_item_bg">
															<asp:Literal id="lMailtextName" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg">メールFROM名</td>
														<td align="left" class="detail_item_bg">
															<asp:Literal id="lMailFromName" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg">メールFROM</td>
														<td align="left" class="detail_item_bg">
															<asp:Literal id="lMailFrom" runat="server"></asp:Literal></td>
													</tr>
													<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
													<tr>
														<td align="left" class="detail_title_bg">言語コード</td>
														<td align="left" class="detail_item_bg">
															<asp:Literal id="lLanguageCode" runat="server"></asp:Literal></td>
													</tr>
													<% } %>
													<tr >
														<td align="left" class="detail_title_bg">作成日</td>
														<td align="left" class="detail_item_bg">
															<asp:Literal id="lMailtextDateCreated" runat="server"></asp:Literal></td>
													</tr>
													<tr >
														<td align="left" class="detail_title_bg">更新日</td>
														<td align="left" class="detail_item_bg">
															<asp:Literal id="lMailtextDateChanged" runat="server"></asp:Literal></td>
													</tr>
													<tr >
														<td align="left" class="detail_title_bg">最終更新者</td>
														<td align="left" class="detail_item_bg">
															<asp:Literal id="lMailtextLastChanged" runat="server"></asp:Literal></td>
													</tr>													
												</table>
												<!--△ 基本情報 △-->
												<br />
												<!--▽ 内容 ▽-->
												<table class="detail_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr>
														<td align="center" class="detail_title_bg" colspan="2">内容</td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg" width="30%">メールタイトル</td>
														<td align="left" class="detail_item_bg">
															<asp:Literal id="lMailtextSubject" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg">メール文章テキスト</td>
														<td align="left" class="detail_item_bg">
															<asp:Literal id="lMailtextBody" runat="server"></asp:Literal>
														
															<asp:Repeater ID="rMailTextBody" runat="server">
																<ItemTemplate>
																	<asp:Literal ID="lMailTextLine" Text="<%# Container.DataItem %>" runat="server"></asp:Literal><br />
																</ItemTemplate>
															</asp:Repeater>
														</td>
													</tr>
													<tr >
														<td align="left" class="detail_title_bg">メール文章HTML</td>
														<td align="left" class="detail_item_bg">
															<iframe class="HtmlPreview" src="<%: new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_HTML_PREVIEW_FORM).AddParam(Constants.HTML_PREVIEW_NO, "1").CreateUrl() %>"
																style="width: 100%; height: 100%;" frameborder="0" scrolling="auto"></iframe>
														</td>
													</tr>
													<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
													<div id="divMobileSetting" runat="server">
													<tr>
														<td align="left" class="detail_title_bg_dark" width="30%">モバイルメールタイトル</td>
														<td align="left" class="detail_item_bg_dark">
															<div id="dvMailtextSubjectMobile" runat="server" class="dvPreview">
																<asp:Literal id="lMailtextSubjectMobile" runat="server"></asp:Literal></div></td>
													</tr>
													<tr>
														<td align="left" class="detail_title_bg_dark">モバイルメール文章</td>
														<td align="left" class="detail_item_bg_dark">
															<div id="dvMailtextMobile" runat="server" class="dvPreview">
																<asp:Literal id="lMailtextMobile" runat="server"></asp:Literal></div>
															<table id="tblMobileMailInfo" runat="server" class="info_table" width="260" border="0" cellspacing="1" cellpadding="3" style="float:left;margin-left:10px;">
																<tr class="info_item_bg">
																	<td align="left">
																		<p style="margin: 6px 0 12px 0">・確認画面で変換できない絵文字はすべて仮の画像<img src='<%= Constants.MARKETINGPLANNER_EMOJI_IMAGE_URL + "no_emoji.png" %>' />で表示されておりますが、実際にこの画像が絵文字として送信されるわけではありません。</p>
																	</td>
																</tr>
															</table>
														</td>
													</tr>
													</div>
													<% } %>
												</table>
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
																<asp:Literal runat="server" ID="lsmsUseFlg"></asp:Literal>
															</td>
														</tr>
														<asp:Repeater runat="server" ID="rSmsCarrier" ItemType="w2.Domain.GlobalSMS.GlobalSMSDistTextModel">
															<ItemTemplate>
																<tr>
																	<td class="edit_title_bg" align="left" width="20%">SMSテキスト本文(<%#Item.PhoneCarrier %>)</td>
																	<td class="edit_item_bg" align="left">
																		<%# WebSanitizer.HtmlEncodeChangeToBr(Item.SmsText) %>
																	</td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
														</tbody>
													</table>
													<br/>
												<% } %>
												<% if (w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED && (this.IsSelectedLanguageCode == false)) { %>
													<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">LINE情報</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">LINEを送信する</td>
															<td class="edit_item_bg" align="left">
																<asp:Literal runat="server" ID="llineDirectUseFlg"/>
															</td>
														</tr>
														<asp:Repeater runat="server" ID="rLineDirectContents" ItemType="w2.Domain.MessagingAppContents.MessagingAppContentsModel">
															<ItemTemplate>
																<tr>
																	<td class="edit_title_bg" align="left" width="20%">LINE送信内容(<%# (Container.ItemIndex + 1) %>)</td>
																	<td class="edit_item_bg" align="left">
																		<%# WebSanitizer.HtmlEncodeChangeToBr(Item.Contents) %>
																	</td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
														</tbody>
													</table>
													<br/>
												<% } %>
												<!--△ 内容 △-->
												<div class="action_part_bottom">
													<input type="button" onclick="Javascript:history.back();" value="  戻る  " />
													<asp:Button id="btnEditBottom" runat="server" Visible="False" Text="  編集する  " onclick="btnEdit_Click"></asp:Button>
													<asp:Button id="btnCopyInsertBottom" runat="server" Text="  コピー新規登録する  " Visible="False" onclick="btnCopyInsert_Click"></asp:Button>
													<% btnMailClickSettingBottom.OnClientClick = "javascript:open_window('" + GetMailClickPopupUrl() + "','contact','width=828,height=900,top=120,left=420,status=NO,scrollbars=yes'); return false;"; %>
													<asp:Button ID="btnMailClickSettingBottom" Text="メールクリック設定" runat="server" />
													<asp:Button ID="btnDeleteBottom" Runat="server" visible="False" Text="  削除する  " onclick="btnDelete_Click" OnClientClick="return confirm('情報を削除するとレポートで名称が取得できないなどの不整合が発生する可能性があります。\n削除してもよろしいですか？');"></asp:Button>
													<asp:Button id="btnUpdateBottom" runat="server" Visible="False" Text="  更新する  " onclick="btnUpdate_Click"></asp:Button>
													<asp:Button id="btnInsertBottom" runat="server" Visible="False" Text="  登録する  " onclick="btnInsert_Click"></asp:Button>
													<asp:Button id="btnInsertGlobalBottom" runat="server" Text="  他言語コードで登録する  " OnClick="btnInsertGlobal_Click" Visible="False" />
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
</asp:Content>
