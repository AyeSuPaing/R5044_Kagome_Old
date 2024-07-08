<%--
=========================================================================================================
  Module      : メッセージページ(MessageInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Manager" %>
<%@ Import Namespace="w2.App.Common.Cs" %>
<%@ Import Namespace="w2.App.Common.Cs.Message" %>
<%@ Import Namespace="w2.App.Common.Cs.AnswerTemplate" %>
<%@ Import Namespace="w2.App.Common.Cs.UserHistory" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="MessageInput.aspx.cs" Inherits="Form_Message_MessageInput" %>
<%@ Import Namespace="w2.Common.Web" %>
<%@ Import Namespace="w2.Domain.MenuAuthority.Helper" %>
<%@ Register Src="~/Form/Message/MessageRightIncident.ascx" TagPrefix="uc1" TagName="MessageRightIncident" %>
<%@ Register Src="~/Form/Message/MessageRightTel.ascx" TagPrefix="uc1" TagName="MessageRightTel" %>
<%@ Register Src="~/Form/Message/MessageRightMail.ascx" TagPrefix="uc1" TagName="MessageRightMail" %>
<%@ Register TagPrefix="uc" TagName="ErrorPoint" Src="~/Form/Top/ErrorPointIcon.ascx" %>

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
	<table width="99%" border="0" cellspacing="0" cellpadding="0">
	<tr>
		<td align="center" valign="top">
			<table width="100%" border="0" cellspacing="0" cellpadding="0">
			<tr>
				<td><img alt="" src="../../Images/Common/sp.gif" border="0" height="5" width="5" /></td>
			</tr>
			<tr>
				<td width="40%">
						
					<asp:UpdatePanel ID="up1" UpdateMode="Conditional" ChildrenAsTriggers="false" runat="server">
					<ContentTemplate>

					<!-- タブ -->
					<div class="tab_title tab_title_left">
						<span class="tab_title_left">
							<% lbChangeToUserSearchMode.CssClass = (this.DispLeftMode == DispLeftMode.UserSearch) ? "active" : ""; %>
							<% lbChangeToUserInfoMode.CssClass = (this.DispLeftMode == DispLeftMode.UserInfo) ? "active" : ""; %>
							<% lbChangeToIncidentMode.CssClass = (this.DispLeftMode == DispLeftMode.Incident) ? "active" : ""; %>
							<% lbChangeToAnswerTemplateMode.CssClass = (this.DispLeftMode == DispLeftMode.AnswerTemplate) ? "active" : ""; %>
							<ol class="toc">
								<li><asp:LinkButton ID="lbChangeToUserSearchMode" CommandArgument="UserSearch" runat="server" OnClick="lbChangeDispMode_Click"><span>顧客検索</span></asp:LinkButton></li>
								<li><asp:LinkButton ID="lbChangeToUserInfoMode" CommandArgument="UserInfo" runat="server" OnClick="lbChangeDispMode_Click"><span>顧客情報</span></asp:LinkButton></li>
								<li><asp:LinkButton ID="lbChangeToIncidentMode" CommandArgument="Incident" runat="server" OnClick="lbChangeDispMode_Click"><span>インシデント</span></asp:LinkButton></li>
							</ol>
						</span>
						<span class="tab_title_right">
							<ol class="toc_r">
								<li><asp:LinkButton ID="lbChangeToAnswerTemplateMode" CommandArgument="AnswerTemplate" runat="server" OnClick="lbChangeDispMode_Click"><span>回答例</span></asp:LinkButton></li>
							</ol>
						</span>
					</div>

					<asp:HiddenField ID="hfSelectedUserId" runat="server" />
					<asp:HiddenField ID="hfSelectedIncidentId" runat="server" />
					<asp:HiddenField ID="hfSelectedMessageNo" runat="server" />
					<asp:HiddenField ID="hfSelectedAnswerId" runat="server" />

					<!-- 顧客検索エリア -->
					<div id="divUserSearch" class="datagrid" runat="server">
						<%
						tbSearchUserName.Attributes["onkeypress"]
							= tbSearchTel.Attributes["onkeypress"]
							= tbSearchMailAddr.Attributes["onkeypress"]
							= tbSearchUserId.Attributes["onkeypress"]
							= tbSearchOrderId.Attributes["onkeypress"]
							= "if (event.keyCode == 13) { __doPostBack('" + btnUserSearch.UniqueID + "',''); return false; }";
						%>
						<!-- 顧客検索フォーム -->
						<table>
						<tr>
						<td width="20%" class="alt">氏名</td>
						<td width="80%">
						<span class="padding-left12">
							<asp:TextBox id="tbSearchUserName" Width="120" runat="server" />
							<input value="  新規顧客登録  " onclick="javascript:open_window('<%= WebSanitizer.UrlAttrHtmlEncode(SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Ec, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_USER_REGISTER + "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT + "&" + Constants.REQUEST_KEY_WINDOW_KBN + "=" + Constants.KBN_WINDOW_POPUP + "&" + Constants.REQUEST_KEY_HIDE_BACK_BUTTON + "=" + Constants.KBN_BACK_BUTTON_HIDDEN + "&" + Constants.REQUEST_KEY_MANAGER_POPUP_CLOSE_CONFIRM + "=" + WebMessages.CFMMSG_USERREGISTER_POPUP_CLOSE_CONFIRM + "&" + Constants.REQUEST_KEY_USER_KBN + "=" + Constants.FLG_USER_USER_KBN_CS)) %>	','userregister','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');" type="button" />
						</span>
						</td>
						</tr>
						<tr>
							<td class="alt">電話番号</td>
							<td>
								<span class="padding-left12">
									<asp:TextBox id="tbSearchTel" Width="120" runat="server" />
								</span>
							</td>
						</tr>
						<tr>
							<td class="alt">メール</td>
							<td>
								<span class="padding-left12">
									<asp:TextBox id="tbSearchMailAddr" Width="120" runat="server"></asp:TextBox>
								</span>
							</td>
						</tr>
						<tr>
							<td class="alt">ユーザーID</td>
							<td>
								<span class="padding-left12">
									<asp:TextBox id="tbSearchUserId" Width="120" runat="server"></asp:TextBox>
								</span>
							</td>
						</tr>
						<tr>
							<td class="alt">注文ID</td>
							<td>
								<span class="padding-left12">
									<asp:TextBox id="tbSearchOrderId" Width="120" runat="server"></asp:TextBox>
								</span>
							</td>
						</tr>
						<tr>
							<td colspan="2">
								<asp:Button ID="btnUserSearch" Text="  顧客検索  " OnClick="btnUserSearch_Click" runat="server" />
								<asp:Literal ID="lUserSearchCount" runat="server"></asp:Literal>
							</td>
						</tr>
						</table>
						<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />

						<!-- 顧客検索結果一覧 -->
						<div id="divUserList" runat="server">
							<div class="y_scrollable" style="HEIGHT:302px; background-color:#CCC">
							<div class="dataresult">
								<asp:LinkButton ID="lbSelectUser" runat="server" OnClick="lbSelectUser_Click"></asp:LinkButton>
								<table class="list_table_min">
								<tr class="alt">
									<td width="25%">
										<asp:LinkButton runat="server" ID="lbUserIdSort" OnClick="lbMessageInputSort_Click" CommandArgument="UserId" Width="100%" Font-Underline="false">ユーザーID
											<span style="float:right"><asp:Label runat="server" ID="lUserIdIconSort"></asp:Label></span>
										</asp:LinkButton>
									</td>
									<td width="15%">
										<asp:LinkButton runat="server" ID="lbUserNameSort" OnClick="lbMessageInputSort_Click" CommandArgument="UserName" Width="100%" Font-Underline="false">氏名
											<span style="float:right"><asp:Label runat="server" ID="lUserNameIconSort"></asp:Label></span>
										</asp:LinkButton>
									</td>
									<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
										<td width="15%" nowrap="nowrap">
											<asp:LinkButton runat="server" ID="lbCompanyNameSort" OnClick="lbMessageInputSort_Click" CommandArgument="CompanyName" Width="100%" Font-Underline="false">
												<%: ReplaceTag("@@User.company_name.name@@")%>
												<span style="float:right"><asp:Label runat="server" ID="lCompanyNameIconSort"></asp:Label></span>
											</asp:LinkButton>
										</td>
										<td width="15%" nowrap="nowrap">
											<asp:LinkButton runat="server" ID="lbCompanyPostNameSort" OnClick="lbMessageInputSort_Click" CommandArgument="CompanyPostName" Width="100%" Font-Underline="false">
												<%: ReplaceTag("@@User.company_post_name.name@@")%>
												<span style="float:right"><asp:Label runat="server" ID="lCompanyPostNameIconSort"></asp:Label></span>
											</asp:LinkButton>
										</td>
									<% } %>
									<td width="30%">
										<asp:LinkButton runat="server" ID="lbMailAddressSort" OnClick="lbMessageInputSort_Click" CommandArgument="MailAddress" Width="100%" Font-Underline="false">メールアドレス
											<span style="float:right"><asp:Label runat="server" ID="lMailAddressIconSort"></asp:Label></span>
										</asp:LinkButton>
									</td>
								</tr>
								<asp:Repeater ID="rUserSearchResult" runat="server">
								<ItemTemplate>
								<tr class="<%# "oplist_item_bg" + (Container.ItemIndex % 2 + 1) %>" onmouseover="listselect_mover_cs(this)" onmouseout="listselect_mout_cs(this, <%# Container.ItemIndex % 2 %>)" onMousedown="listselect_mdown(this)" onclick="listselect_mclick_csuserlist(this, <%# Container.ItemIndex % 2 %>, '<%# Eval(Constants.FIELD_USER_USER_ID)%>')">
									<td><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USER_USER_ID)) %></td>
									<td><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USER_NAME))%></td>
									<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
										<td><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USER_COMPANY_NAME)) %></td>
										<td><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USER_COMPANY_POST_NAME)) %></td>
									<% } %>
									<td nowrap="nowrap"><%# StringUtility.ChangeToBrTag(WebSanitizer.HtmlEncode(Eval(Constants.FIELD_USER_MAIL_ADDR) + "\r\n" + Eval(Constants.FIELD_USER_MAIL_ADDR2)).Trim()) %></td>
								</tr>
								</ItemTemplate>
								</asp:Repeater>
								<tr id="trUserSearchResultAlert" class="oplist_item_bg1" runat="server">
									<td id="tdUserSearchResultAlert" style="text-align:center;padding:5px">該当データが存在しません</td>
								</tr>
								</table>
							</div>
							</div>
							<div id="win-size-grip4"><img src ="../../Images/Cs/hsizegrip.png"></div>
						</div>
					</div>

					<!-- 顧客情報エリア -->
					<div id="divUserInfo" class="datagrid" runat="server">

						<!-- 顧客詳細 -->
						<table>
						<tr>
							<td width="15%" class="alt">氏名</td>
							<td width="30%"><asp:Literal ID="lUserName" runat="server"></asp:Literal></td>
							<td width="15%" class="alt"><%: ReplaceTag("@@User.name_kana.name@@") %></td>
							<td width="40%"><asp:Literal ID="lUserNameKana" runat="server"></asp:Literal></td>
						</tr>
						<tr>
							<td class="alt">電話番号</td>
							<td>
								<div><asp:Literal ID="lUserTel1" runat="server"></asp:Literal></div>
								<div><asp:Literal ID="lUserTel2" runat="server"></asp:Literal></div>
							</td>
							<td class="alt">メール</td>
							<td>
								<div><asp:Literal ID="lUserMail" runat="server"></asp:Literal><uc:ErrorPoint ID="ucErrorPoint" runat="server" /></div>
								<div><asp:Literal ID="lUserMail2" runat="server"></asp:Literal><uc:ErrorPoint ID="ucErrorPoint2" runat="server" /></div>
							</td>
						</tr>
						</table>
						<% btnReflectUserFromUser.Enabled = (hfSelectedUserId.Value != ""); %>
						<% btnSendMailAddress.Enabled = (hfSelectedUserId.Value != ""); %>
						<div style="float:left;margin:1px">
						<asp:Button ID="btnReflectUserFromUser" Text="顧客情報差込" Width="120"  runat="server" OnClick="btnReflectUserFromUser_Click" />
							<asp:Button ID="btnSendMailAddress" Text="送信先差込" Width="120" runat="server" OnClick="btnSendMailAddress_OnClick" />
						</div>
						<div style="float:right;margin:2px">
						<input style="WIDTH: 80px" class="button" onclick='Javascript:open_window("<%= Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MESSAGE_USER_SEARCH + "?" + Constants.REQUEST_KEY_USER_ID + "=" + HttpUtility.UrlEncode(hfSelectedUserId.Value) %>	", "usersearch","width=1200,height=740,toolbar=1,resizable=1,status=1,scrollbars=1");' value="顧客検索" type="button" name="sample" />
						</div>
						<br class="clr" />
						<img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />

						<!-- インシデント一覧 -->
						<div class="y_scrollable" style="HEIGHT:387px; margin-top:3px; background-color:#CCC">
						<div class="dataresult">
							<asp:LinkButton ID="lbSelectIncident" runat="server" OnClick="lbSelectIncident_Click"></asp:LinkButton>
							<table class="list_table_min">
								<tr class="alt">
									<td width="75" rowspan="2" align="center">インシデントID</td>
									<td align="center" rowspan="2" >タイトル</td>
									<td width="110">初回問合せ日時</td>
									<td>初回問合せ件名</td>
								</tr>
								<tr class="alt">
									<td>最終問合せ日時</td>
									<td>最終問合せ件名</td>
								</tr>
								<asp:Repeater ID="rUserIncidentHisory" ItemType="w2.App.Common.Cs.UserHistory.UserHistoryIncident" runat="server">
								<ItemTemplate>
									<tr class="<%# "oplist_item_bg" + (Container.ItemIndex % 2 + 1) %>" onmouseover="listselect_mover_cs(this)" onmouseout="listselect_mout_cs(this, <%# Container.ItemIndex % 2 %>)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick_csincident(this, <%# Container.ItemIndex % 2 %>, '<%#((UserHistoryIncident)Container.DataItem).IncidentId %>', <%# ((UserHistoryIncident)Container.DataItem).FirstMessageNo %>)">
										<td rowspan="2"><%# WebSanitizer.HtmlEncode(((UserHistoryIncident)Container.DataItem).IncidentId) %></td>
										<td rowspan="2"><%#: ((UserHistoryIncident)Container.DataItem).IncidentTitle %></td>
										<td><%#: DateTimeUtility.ToStringForManager(Item.FirstMessageDateTime, DateTimeUtility.FormatType.ShortDateHourMinute2Letter) %></td>
										<td>
											<span class="img_align"><img src="../../Images/Cs/<%# CsCommon.GetMessageIcon(((UserHistoryIncident)Container.DataItem).FirstMessageStatus, ((UserHistoryIncident)Container.DataItem).FirstMessageMediaKbn, ((UserHistoryIncident)Container.DataItem).FirstMessageDirectionKbn) %>" alt="" /></span>
											<%# WebSanitizer.HtmlEncode(((UserHistoryIncident)Container.DataItem).FirstMessageTitle) %>
										</td>
									</tr>
									<tr class='<%# "oplist_item_bg" + (Container.ItemIndex % 2 + 1) %>' onmouseover="listselect_mover_cs(this)" onmouseout="listselect_mout_cs(this, <%# Container.ItemIndex % 2 %>)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick_csincident(this, <%# Container.ItemIndex % 2 %>, '<%# ((UserHistoryIncident)Container.DataItem).IncidentId %>', <%# ((UserHistoryIncident)Container.DataItem).LastMessageNo %>)">
										<td>
											<span style="<%# ((UserHistoryIncident)Container.DataItem).IsSameFirstAndLast ? "color:#aaa" : "" %>">
												<%#: DateTimeUtility.ToStringForManager(Item.LastMessageDateTime, DateTimeUtility.FormatType.ShortDateHourMinute2Letter) %>
											</span>
										</td>
										<td>
											<span style="<%# ((UserHistoryIncident)Container.DataItem).IsSameFirstAndLast ? "color:#aaa" : "" %>">
											<span class="img_align">
												<img src="../../Images/Cs/<%# CsCommon.GetMessageIcon(((UserHistoryIncident)Container.DataItem).LastMessageStatus, ((UserHistoryIncident)Container.DataItem).LastMessageMediaKbn, ((UserHistoryIncident)Container.DataItem).LastMessageDirectionKbn) %>" alt="" /></span>
												<%# WebSanitizer.HtmlEncode(((UserHistoryIncident)Container.DataItem).LastMessageTitle) %>
											</span>
										</td>
									</tr>
								</ItemTemplate>
								</asp:Repeater>
								<tr id="trUserIncidentHisoryAlert" class="oplist_item_bg1" runat="server">
									<td colspan="4" style="text-align:center;padding:5px">該当データが存在しません</td>
								</tr>
							</table>
						</div>
						</div>
					</div>

					<!-- インシデントエリア -->
					<div id="divIncident" class="datagrid" runat="server">

						<!-- インシデント情報 -->
						<table>
						<tr>
							<td width="20%" class="alt">インシデントID</td>
							<td width="30%"><asp:Literal ID="lIncidentId" runat="server"></asp:Literal>
								<span class="img_align">
									<img id="imgLockTop" src="../../Images/Cs/icon_lock.png" alt="ロック" width="14" height="14" runat="server" />
								</span>
							</td>
							<td width="20%" class="alt">担当者</td>
							<td width="30%"><asp:Literal ID="lIncidentOperatorName" runat="server"></asp:Literal></td>
						</tr>
						<tr>
							<td class="alt">タイトル</td>
							<td colspan="3"><asp:Literal ID="lIncidentTitle" runat="server"></asp:Literal></td>
						</tr>
						<tr>
							<td class="alt">内部メモ</td>
							<td colspan="3"><asp:Literal ID="lIncidentComment" runat="server"></asp:Literal></td>
						</tr>
						</table>

						<!-- 差し込みボタン -->
						<div style="float:left;margin:2px">
						<%
							btnReflectIncident.Enabled = ((hfSelectedIncidentId.Value != "")
								&& (Request[Constants.REQUEST_KEY_MESSAGE_EDIT_MODE] == Constants.KBN_MESSAGE_EDIT_MODE_NEW)
								&& string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_INCIDENT_ID])
								&& string.IsNullOrEmpty(this.NewRegisteredIncidentId));
						%>
						<asp:Button ID="btnReflectIncident" Text="インシデント情報差込" Width="170" runat="server" OnClick="btnReflectIncident_Click" />
						</div>
						<div style="float:right;margin:2px">
						<% btnReflectUserFromMessage.Enabled = (hfSelectedUserId.Value != ""); %>
						<asp:Button ID="btnReflectUserFromMessage" Text="顧客情報差込" Width="120" runat="server" OnClick="btnReflectUserFromMessage_Click" />
						</div>
						<br class="clr" />

						<!-- メッセージ履歴 -->
						<div class="y_scrollable" style="HEIGHT:147px; background-color:#CCC">
						<div class="dataresult">
							<asp:LinkButton ID="lbSelectMessage" runat="server" OnClick="lbSelectMessage_Click"></asp:LinkButton>
							<table id="message_list" class="list_table_min">
							<tr class="alt">
								<td width="25%">日時</td>
								<td width="50%">件名</td>
								<td width="25%">作成者</td>
							</tr>
							<asp:Repeater ID="rIncidentRequires" ItemType="w2.App.Common.Cs.Message.CsMessageModel" runat="server">
							<ItemTemplate>
								<tr id="message_tr<%# ((CsMessageModel)Container.DataItem).MessageNo %>" class="<%# "oplist_item_bg" + (Container.ItemIndex % 2 + 1) %>" onmouseover="listselect_mover_cs(this)" onmouseout="listselect_mout_cs(this, <%# Container.ItemIndex % 2 %>)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick_message(this, <%# Container.ItemIndex % 2 %>, '<%# ((CsMessageModel)Container.DataItem).MessageNo %>')">
									<td><%#: DateTimeUtility.ToStringForManager(Item.EX_InquiryReplyChangedDate, DateTimeUtility.FormatType.ShortDateHourMinute2Letter) %></td>
									<td>
										<span class="img_align"><img src="../../Images/Cs/<%# ((CsMessageModel)Container.DataItem).GetMessageIcon() %>" alt="" /></span>
										<%# WebSanitizer.HtmlEncode(((CsMessageModel)Container.DataItem).InquiryTitle) %>
										<span id="spUrgent" visible="<%# ((CsMessageModel)Container.DataItem).EX_UrgencyFlg == Constants.FLG_CSMESSAGEREQUEST_URGENCY_URGENT %>" class="notice" runat="server">*</span>
									</td>
									<td><%# WebSanitizer.HtmlEncode((string.IsNullOrEmpty(((CsMessageModel)Container.DataItem).EX_OperatorName)) ? "-" : ((CsMessageModel)Container.DataItem).EX_OperatorName) %></td>
								</tr>
							</ItemTemplate>
							</asp:Repeater>
							</table>
						</div>
						</div>
						<br />

						<asp:UpdatePanel ID="up2" UpdateMode="Conditional" ChildrenAsTriggers="false" runat="server">
						<ContentTemplate>

						<!-- メッセージ詳細（電話） -->
						<div id="divMessageTel" runat="server" visible="false">
						<table>
						<tr>
							<td width="20%" class="alt">問合せ 日時</td>
							<td>
								<asp:Literal ID="lMessageInquiryDateTime" runat="server"></asp:Literal><br />
							</td>
							<td width="20%" class="alt">	
								回答者
							</td>
							<td>
								<asp:Literal ID="lMessageReplyOperatorName" runat="server"></asp:Literal>
							</td>
						</tr>
						<tr>
							<td width="20%" class="alt">氏名</td>
							<td width="25%"><asp:Literal ID="lMessageUserName" runat="server"></asp:Literal></td>
							<td width="20%" class="alt"><%: ReplaceTag("@@User.name_kana.name@@") %></td>
							<td width="35%"><asp:Literal ID="lMessageUserNameKana" runat="server"></asp:Literal></td>
						</tr>
						<tr>
							<td class="alt">電話番号</td>
							<td><asp:Literal ID="lMessageUserTel" runat="server"></asp:Literal></td>
							<td class="alt">メール</td>
							<td><asp:Literal ID="lMessageUserMail" runat="server"></asp:Literal><uc:ErrorPoint ID="ucErrorPointMessage" runat="server" /></td>
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
						<tr class="alt">
							<td colspan="4">内容</td>
						</tr>
						<tr>
							<td colspan="4"><asp:Literal ID="lMessageInquiryText" runat="server"></asp:Literal></td>
						</tr>
						<tr class="alt">
							<td colspan="4">回答</td>
						</tr>
						<tr>
							<td colspan="4"><asp:Literal ID="lMessageReplyText" runat="server"></asp:Literal>	</td>
						</tr>
						</table>
						</div>

						<!-- メッセージ詳細（メール）-->
						<div id="divMessageMail" runat="server" visible="false">
						<table>
						 <tr>
							<td>
								<div id="mailheader larger">
								<p>
									<span><strong>差出人：</strong>&nbsp;<asp:Literal id="lMailFrom" runat="server"></asp:Literal><uc:ErrorPoint ID="ucErrorPointFrom" runat="server" /></span>
									<span><strong>&nbsp;&nbsp;&nbsp;&nbsp;宛先：</strong>&nbsp;<asp:Literal id="lMailTo" runat="server"></asp:Literal><uc:ErrorPoint ID="ucErrorPointTo" runat="server" /></span>
									<span id="spMailCc" runat="server"><strong>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;CC：</strong>&nbsp;<asp:Literal id="lMailCc" runat="server"></asp:Literal></span>
									<span id="spMailBcc" runat="server"><strong>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Bcc：</strong>&nbsp;<asp:Literal id="lMailBcc" runat="server"></asp:Literal></span>
									<span><strong>&nbsp;&nbsp;&nbsp;&nbsp;件名：</strong>&nbsp;<asp:Literal id="lMailSubject" runat="server"></asp:Literal></span>
									<span><strong>&nbsp;&nbsp;&nbsp;&nbsp;日時：</strong>&nbsp;<asp:Literal id="lMailDate" runat="server"></asp:Literal></span>
									<asp:Repeater ID="rMailAttachmentFiles" runat="server">
									<HeaderTemplate><span><strong>&nbsp;&nbsp;&nbsp;&nbsp;添付：</strong></HeaderTemplate>
									<ItemTemplate>
										<img src="../../Images/Cs/icon_clip.png" alt="添付" width="16" height="16" style="vertical-align: bottom;" /><a href="<%# Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MESSAGE_MAILATTACHMENTDOWNLOADER
											+ "?" + Constants.REQUEST_KEY_MAIL_ID + "=" + HttpUtility.UrlEncode(((CsMessageMailAttachmentModel)Container.DataItem).MailId)  + "&" + Constants.REQUEST_KEY_FILE_NO
											+ "=" + ((CsMessageMailAttachmentModel)Container.DataItem).FileNo %>"><%# ((CsMessageMailAttachmentModel)Container.DataItem).FileName %></a>
									</ItemTemplate>
									<SeparatorTemplate>, </SeparatorTemplate>
									<FooterTemplate></span></FooterTemplate>
									</asp:Repeater>
								</p>
								</div>
							</td>
						</tr>
						<tr id="trMailMessageStatus" class="mailalert" runat="server">
							<td valign="bottom">
								<span class="tab_title_right img_align">
									<asp:Literal ID="lMailMessageStatus" runat="server"></asp:Literal>
								</span>
							</td>
						</tr>
						<tr>
							<td>
								<div id="maildisp larger">
									<p>
										<asp:Literal id="lMailBody" runat="server"></asp:Literal>
									</p>
								</div>
							</td>
						</tr>
						</table>
						</div>

						</ContentTemplate>
						<Triggers>
							<asp:AsyncPostBackTrigger ControlID="lbChangeToUserSearchMode" EventName="Click" />
							<asp:AsyncPostBackTrigger ControlID="lbChangeToUserInfoMode" EventName="Click" />
							<asp:AsyncPostBackTrigger ControlID="lbChangeToIncidentMode" EventName="Click" />
							<asp:AsyncPostBackTrigger ControlID="lbChangeToAnswerTemplateMode" EventName="Click" />
							<asp:AsyncPostBackTrigger ControlID="btnUserSearch" EventName="Click" />
							<asp:AsyncPostBackTrigger ControlID="lbSelectUser" EventName="Click" />
							<asp:AsyncPostBackTrigger ControlID="lbSelectIncident" EventName="Click" />
							<asp:AsyncPostBackTrigger ControlID="lbSelectMessage" EventName="Click" />
						</Triggers>
						</asp:UpdatePanel>
					</div>
					<div id="divNoIncidentMessage" class="datagrid" visible="false" runat="server">
							<table class="list_table_min">
							<tr class="oplist_item_bg1" runat="server">
								<td style="text-align:center;padding:5px;font-size:11px">インシデント情報が存在しません。</td>
							</tr>
							</table>
					</div>

					<!-- 回答例エリア -->
					<div id="divAnswerTemplates" style="height:502px"  class="datagrid" runat="server">
						<asp:UpdatePanel ID="up3" UpdateMode="Conditional" ChildrenAsTriggers="false" runat="server">
						<ContentTemplate>
							<%
								tbAnswerTemplateSearchKeyword.Attributes["onkeypress"]
								= "if (event.keyCode == 13) { __doPostBack('" + btnAnswerTemplateSearch.UniqueID + "',''); return false; }";
							%>
							<table>
							<tr>
								<td width="25%" class="alt">キーワード</td>
								<td>
									<asp:TextBox id="tbAnswerTemplateSearchKeyword" Width="250px" runat="server"></asp:TextBox>
								</td>
							</tr>
							<tr>
								<td class="alt">カテゴリ</td>
								<td>
									<asp:DropDownList ID="ddlAnswerTemplateCategories" CssClass="select2-select" Width="250px" runat="server"></asp:DropDownList>
								</td>
								</tr>
							</table>
							<div style="WIDTH:100%;float:left;margin:2px 2px 2px 2px">
								<asp:Button ID="btnAnswerTemplateSearch" Text="   検索   " runat="server" OnClick="btnAnswerTemplateSearch_Click" />
							</div>
							<div id="divAnswerTempalteList" visible="false" runat="server">
							<div class="y_scrollable" style="HEIGHT:372px;margin-top:3px; background-color:#CCC">
							<div class="dataresult">
								<asp:LinkButton ID="lbSelectAnswerTemplate" runat="server" OnClick="lbSelectAnswerTemplate_Click"></asp:LinkButton>
								<table class="list_table_min">
									<tr class="alt">
										<td width="30%">回答例タイトル</td>
										<td width="30%">件名</td>
										<td width="40%">本文</td>
									</tr>
									<asp:Repeater ID="rAnswerTemplateList" runat="server">
									<ItemTemplate>
										<tr class="<%# "oplist_item_bg" + (Container.ItemIndex % 2 + 1) %>" onmouseover="listselect_mover_cs(this)" onmouseout="listselect_mout_cs(this, <%# Container.ItemIndex % 2 %>)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick_csanswertemplate(this, <%# Container.ItemIndex % 2 %>, '<%# ((CsAnswerTemplateModel)Container.DataItem).AnswerId %>')">
											<td title="<%#: ((CsAnswerTemplateModel)Container.DataItem).AnswerTitle %>">
												<%#: AbbreviateString(((CsAnswerTemplateModel)Container.DataItem).AnswerTitle, 12) %>
											</td>
											<td title="<%#: ((CsAnswerTemplateModel)Container.DataItem).AnswerMailTitle %>">
												<%#: AbbreviateString(((CsAnswerTemplateModel)Container.DataItem).AnswerMailTitle, 12) %>
											</td>
											<td title="<%#: ((CsAnswerTemplateModel)Container.DataItem).AnswerText %>">
												<%#: AbbreviateString(((CsAnswerTemplateModel)Container.DataItem).AnswerText, 12) %>
											</td>
										</tr>
									</ItemTemplate>
									</asp:Repeater>
									<tr id="trAnswerTempalteResultAlert" class="oplist_item_bg1" runat="server">
										<td colspan="2" style="text-align:center;padding:5px">該当データが存在しません</td>
									</tr>
								</table>
							</div>
							</div>
							</div>
						</ContentTemplate>
						<Triggers>
							<asp:AsyncPostBackTrigger ControlID="btnAnswerTemplateSearch" EventName="Click" />
						</Triggers>
						</asp:UpdatePanel>
					</div>

					</ContentTemplate>
					<Triggers>
						<asp:AsyncPostBackTrigger ControlID="lbChangeToUserSearchMode" EventName="Click" />
						<asp:AsyncPostBackTrigger ControlID="lbChangeToUserInfoMode" EventName="Click" />
						<asp:AsyncPostBackTrigger ControlID="lbChangeToIncidentMode" EventName="Click" />
						<asp:AsyncPostBackTrigger ControlID="lbChangeToAnswerTemplateMode" EventName="Click" />
						<asp:AsyncPostBackTrigger ControlID="btnUserSearch" EventName="Click" />
						<asp:AsyncPostBackTrigger ControlID="lbSelectUser" EventName="Click" />
						<asp:AsyncPostBackTrigger ControlID="lbSelectIncident" EventName="Click" />
						<asp:AsyncPostBackTrigger ControlID="btnRegisterTelMessage" EventName="Click" />
						<asp:AsyncPostBackTrigger ControlID="btnUpdateTel" EventName="Click" />
						<asp:AsyncPostBackTrigger ControlID="lbUserIdSort" EventName="Click" />
						<asp:AsyncPostBackTrigger ControlID="lbUserNameSort" EventName="Click" />
						<asp:AsyncPostBackTrigger ControlID="lbMailAddressSort" EventName="Click" />
						<asp:AsyncPostBackTrigger ControlID="lbCompanyNameSort" EventName="Click" />
						<asp:AsyncPostBackTrigger ControlID="lbCompanyPostNameSort" EventName="Click" />
					</Triggers>
					</asp:UpdatePanel>

				</td>
			</tr>
			<tr>
				<td>&nbsp;</td>
			</tr>
			</table>
		</td>
		<td width="5"><img alt="" src="../../Images/Common/sp.gif" border="0" height="5" width="5" /></td>
		<td width="60%" valign="top">

		
			<table width="100%" border="0" cellspacing="0" cellpadding="0">
			<tr>
			<td valign="top">
				<div style="WIDTH:100%;float:left;margin:0px 0px 2px 0px">
				<div class="datagrid" id="sendmail">
					<table border="1" cellspacing="0" cellpadding="0" style="WIDTH:100%">
					<thead>
					<tr>
					<th style="font-size:10pt">
					<%if (this.DispRightMode == DispRightMode.TelOthers) { %>
						<img src="../../Images/Cs/icon_tilte_tel.png" alt="電話その他" style="vertical-align:-0.40em;width:20px" />
						電話内容登録
					<%} %>
					<%if (this.DispRightMode == DispRightMode.Mail) { %>
						<img src="../../Images/Cs/icon_tilte_mail.png" alt="メール作成" style="vertical-align:-0.40em;width:20px" />
						メール作成
					<%} %>
				    </th>
					</tr>
					</thead>
					</table>
				</div>
				</div>

				<div id="mail_input" class="y_scrollable" style="height:480px">

					<asp:UpdatePanel ID="up4" runat="server">
					<ContentTemplate>

					<!-- 電話受発信フォーム -->
					<uc1:MessageRightTel ID="ucMessageRightTel" Visible="false" runat="server" />
					<!-- メール返信フォーム -->
					<uc1:MessageRightMail id="ucMessageRightMail" runat="server" />

					<!-- インシデントフォーム -->
					<uc1:MessageRightIncident ID="ucMessageRightIncident" runat="server" />


					<!-- 承認フォーム -->
					<div id="divApprovalForm" class="dataresult" visible="false" runat="server">

						<table>
						<thead>
							<tr><th colspan="4">承認依頼</th></tr>
						</thead>
						</table>

						<asp:Label ID="lApprRequestErrorMessages" CssClass="notice" EnableViewState="false" runat="server"></asp:Label>

						<table>
						<tbody>
							<tr>
								<td width="15%" class="alt">承認者 <span class="notice">*</span></td>
								<td width="85%" colspan="3" style="overflow: visible">
									<asp:Repeater ID="rApprovalOperators" runat="server">
									<ItemTemplate>
										<asp:DropDownList ID="ddlOperator" CssClass="select2-select" DataSource="<%# this.ApprovalOperators %>" DataTextField="Value" DataValueField="Key" Width="180" runat="server"></asp:DropDownList>
										<asp:Button ID="btnDeleteOperator" Text=" 削除 " Enabled="<%# (((string[])((Repeater)Container.Parent).DataSource).Length - 1) > 0 %>" CommandArgument="<%# Container.ItemIndex %>" runat="server" OnClick="btnDeleteOperator_Click" />
										<asp:Button ID="btnAddOperator" Text=" 追加 "  Visible="<%# (Container.ItemIndex == 0) %>" runat="server" OnClick="btnAddOperator_Click" />
										&nbsp;&nbsp;
										<asp:Button ID="btnAddOperatorAll" Text=" 全承認者をセット "  Visible="<%# (Container.ItemIndex == 0) %>" runat="server" OnClick="btnAddOperatorAll_Click" />
										<asp:Button ID="btnAddOperatorGroup" Text=" 所属グループの承認者をセット "  Visible="<%# (Container.ItemIndex == 0) %>" runat="server" OnClick="btnAddOperatorGroup_Click" />
										<br />
									</ItemTemplate>
									</asp:Repeater>
								</td>
							</tr>
							<tr>
								<td class="alt">緊急度</td>
								<td colspan="3">
									<asp:CheckBox ID="cbApprovalUrgencyFlg" Text=" 緊急" runat="server" />
								</td>
							</tr>
							<tr>
								<td class="alt">承認方法<span class="notice">*</span></td>
								<td colspan="3">
									<asp:RadioButtonList ID="rblApprovalType" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="radio_button_list" runat="server"></asp:RadioButtonList>
								</td>
							</tr>
							<tr>
								<td class="alt">承認依頼コメント</td>
								<td colspan="3">
									<asp:TextBox ID="tbApprovalRequestComment" TextMode="MultiLine" Rows="4" Width="98%" runat="server" CssClass="larger"></asp:TextBox>
								</td>
							</tr>
						</tbody>
						</table>
					</div>

					<!-- 送信依頼フォーム -->
					<div id="divSendRequest" class="dataresult" visible="false" runat="server">
						<table>
						<thead>
							<tr>
								<th colspan="4">送信依頼</th>
							</tr>
						</thead>
						</table>

						<asp:Label ID="lSendRequestErrorMessages" CssClass="notice" EnableViewState="false" runat="server"></asp:Label>

						<table>
						<tbody>
							<tr>
								<td width="15%" class="alt">
									送信者<span class="notice">*</span>&nbsp;
								</td>
								<td width="85%" colspan="3" style="overflow: visible">
									<asp:Repeater ID="rMailSendableOperators" runat="server">
									<ItemTemplate>
										<asp:DropDownList ID="ddlOperator" CssClass="select2-select" DataSource="<%# this.MailSendableOperators %>" DataTextField="Value" DataValueField="Key" Width="30%" runat="server"></asp:DropDownList>
										<br />
									</ItemTemplate>
									</asp:Repeater>
								</td>
							</tr>
							<tr>
								<td class="alt">緊急度</td>
								<td colspan="3">
									<asp:CheckBox ID="cbMailSendRequestUrgencyFlg" Text=" 緊急" runat="server" />
								</td>
							</tr>
							<tr>
								<td class="alt">送信依頼コメント</td>
								<td colspan="3">
									<asp:TextBox ID="tbMailSendRequestComment" TextMode="MultiLine" Rows="4" Width="98%" runat="server" CssClass="larger"></asp:TextBox>
								</td>
							</tr>
						</tbody>
						</table>
					</div>

					<a id="aRequestBottom" href="#" runat="server"></a>

					</ContentTemplate>
					</asp:UpdatePanel>

				</div>
				<div id="win-size-grip5"><img src ="../../Images/Cs/hsizegrip.png"></div>
			</td>
			</tr>
			<tr>
			<td>
				<asp:UpdatePanel ID="up5" runat="server">
				<ContentTemplate>

				<asp:LinkButton ID="lbRefreshActionButton" runat="server" OnClick="lbRefreshActionButton_Click"></asp:LinkButton>

				<!-- アクションボタン -->
				<div style="padding:5px 2px 8px 2px; overflow: hidden;">
					<div id="divActionButtonAera" style="float:left;" runat="server">
					<%if (this.DispRightMode == DispRightMode.TelOthers) {%>
						<%if ((this.EditMode == EditMode.EditDone) || ((this.EditMode != EditMode.EditDone) && (this.NewRegisteredIncidentId != ""))) {%>
							<%
								btnUpdateTel.Enabled = this.LoginOperatorCsInfo.EX_PermitEditFlg;
							%>
							<asp:Button ID="btnUpdateTel" Text="  更新する  " runat="server" OnClick="btnUpdateTel_Click" />
						<%} else {%>
							<%
								btnRegisterTelMessage.Enabled
									= btnRegisterTelMessageWithCloseIncident.Enabled
									= btnSaveToTelDraft.Enabled
									= this.LoginOperatorCsInfo.EX_PermitEditFlg;
							%>
							<asp:Button ID="btnRegisterTelMessage" Text="  登録する  " runat="server" OnClick="btnRegisterTelMessage_Click" />
							<asp:Button ID="btnRegisterTelMessageWithCloseIncident" Text=" 登録してインシデントクローズ " runat="server" OnClick="btnRegisterTelMessageWithCloseIncident_Click" />
							<asp:Button ID="btnSaveToTelDraft" Text="  下書き保存  " runat="server" OnClick="btnSaveToTelDraft_Click" />
						<%} %>
					<%} %>
					<%if (this.DispRightMode == DispRightMode.Mail) { %>
						<%
						  if (this.EditMode == EditMode.EditForSend)
						  {
							  rblMailAction.Items[1].Enabled = false;
							  rblMailAction.Items[2].Enabled = false;
							  btnSaveToMailDraft.Visible = false;
						  }
						%>
						<asp:RadioButtonList id="rblMailAction"  CssClass="radio_button_list" RepeatLayout="Flow" RepeatDirection="Horizontal" AutoPostBack="true" runat="server" OnSelectedIndexChanged="rblMailAction_SelectedIndexChanged">
							<asp:ListItem Text="メール送信" Value="MailSend"></asp:ListItem>
							<asp:ListItem Text="承認依頼" Value="ApprovalRequest"></asp:ListItem>
							<asp:ListItem Text="送信依頼" Value="MailSendRequest"></asp:ListItem>
						</asp:RadioButtonList><br /><br />
						<%
						  btnPreviewMail.Enabled
							  = btnSaveToMailDraft.Enabled
							  = this.LoginOperatorCsInfo.EX_PermitEditFlg;
						%>
						<div style="float:left">
						<asp:Button ID="btnPreviewMail" Text="  送信プレビュー  " runat="server" OnClick="btnPreviewMail_Click" />
						<asp:Button ID="btnSaveToMailDraft" Text="  下書き保存  " runat="server" OnClick="btnSaveToMailDraft_Click" />
						</div>
					<%} %>
					</div>
					<div style="float:right">
						<asp:Button ID="bthUnlockAndClose" Text="  ロック解除して閉じる  " runat="server" OnClick="bthUnlockAndClose_Click" OnClientClick="return btnUnlockAndCloseConfirm()" />
						<input id="btnClose" type="button" onclick="Javascript:close_popup();" value="  閉じる  " />
					</div>
				</div>
				<asp:Label ID="lLockMessage" Font-Bold="true" runat="server"></asp:Label>
				<asp:Label ID="lCompleteMessages" Font-Bold="true" EnableViewState="false" runat="server"></asp:Label>
				<br />

				<br class="clr" />

				</ContentTemplate>
				</asp:UpdatePanel>
			</td>
			</tr>
			</table>

		</td>
	</tr>
	</table>
</td>
</tr>
</table>

</div>

<script type="text/javascript">
	//----------------------------------------------------
	// リスト選択列保持用（一行選択時、該当列が選択済の場合は色を変えない）
	//----------------------------------------------------
	class_bgcolover = 'oplist_item_bg_over'; // マウスオーバー時の色
	class_bgcolout1 = 'oplist_item_bg1'; // マウスアウトしたときの色１
	class_bgcolout2 = 'oplist_item_bg2'; // マウスアウトしたときの色２
	class_bgcolout3 = 'oplist_item_bg3'; // マウスアウトしたときの色３
	var class_bgcolouts = new Array(class_bgcolout1, class_bgcolout2, class_bgcolout3);
	class_bgcolclck = 'oplist_item_bg_click'; // マウスクリックしたときの色

	var selected_tr = "";
	var selected_before_style = "";
	var single_select = false;
	function listselect_mover_cs(obj)
	{
		if ((single_select == false) && (obj != selected_tr)) listselect_mover(obj);
	}
	function listselect_mout_cs(obj, lineIdx)
	{
		if ((single_select == false) && (obj != selected_tr)) listselect_mout(obj, lineIdx);
	}

	function listselect_mclick_csuserlist(obj, lineIdx, userId)
	{
		listselect_mclick_csuserlist_inner(obj, userId, class_bgcolouts[lineIdx]);
	}
	function listselect_mclick_csuserlist_inner(obj, userId, class_bgcolout)
	{
		// クリック失敗なら抜ける
		if (listselect_mclick(obj) == false) return;

		// イベント実行
		document.getElementById('<%= hfSelectedUserId.ClientID %>').value = userId;
		__doPostBack("<%= lbSelectUser.UniqueID %>", "");
	}

	function listselect_mclick_csincident(obj, lineIdx, incidentId, messageNo)
	{
		listselect_mclick_csincident_inner(obj, incidentId, messageNo, class_bgcolouts[lineIdx]);
	}
	function listselect_mclick_csincident_inner(obj, incidentId, messageNo, class_bgcolout)
	{
		// クリック失敗なら抜ける
		if (listselect_mclick(obj) == false) return;

		// イベント実行
		document.getElementById('<%= hfSelectedIncidentId.ClientID %>').value = incidentId;
		document.getElementById('<%= hfSelectedMessageNo.ClientID %>').value = messageNo;
		__doPostBack("<%= lbSelectIncident.UniqueID %>", "");
	}

	function listselect_mclick_message(obj, lineIdx, messageNo)
	{
		listselect_mclick_message_inner(obj, messageNo, class_bgcolouts[lineIdx]);
	}
	function listselect_mclick_message_inner(obj, messageNo, class_bgcolout)
	{
		// クリック失敗なら抜ける
		if (listselect_mclick(obj) == false) return;
		// 選択列であればイベント実行しない
		if (obj == selected_tr) return;

		// 以前の選択列の色を戻す
		selected_tr.className = selected_before_style;
		selected_tr = obj;
		selected_before_style = class_bgcolout;

		// イベント実行
		document.getElementById('<%= hfSelectedMessageNo.ClientID %>').value = messageNo;
		//__doPostBack("<%= lbSelectMessage.UniqueID %>", "");

		setTimeout('__doPostBack("<%= lbSelectMessage.UniqueID %>", "");', 100);	// ポストバックを終わらせてから実行したい
	}

	function listselect_mclick_csanswertemplate(obj, lineIdx, answerId)
	{
		listselect_mclick_csanswertemplate_inner(obj, answerId, class_bgcolouts[lineIdx]);
	}
	function listselect_mclick_csanswertemplate_inner(obj, answerId, class_bgcolout)
	{
		// クリック失敗なら抜ける
		if (listselect_mclick(obj) == false) return;
		// 選択列であればイベント実行しない
		//if (obj == selected_tr) return;

		// 以前の選択列の色を戻す
		selected_tr.className = selected_before_style;
		selected_tr = obj;
		selected_before_style = class_bgcolout;

		// イベント実行
		document.getElementById('<%= hfSelectedAnswerId.ClientID %>').value = answerId;
		__doPostBack("<%= lbSelectAnswerTemplate.UniqueID %>", "");
	}

	// 呼び出し元のインシデント情報更新（ロック状態を反映する）
	function refresh_incident_info_opener(incident_id, visible)
	{
		if (window.opener && (window.opener.closed == false) && window.opener.refresh_incident_info) {
			window.opener.refresh_incident_info();
		}
	}

	// トップページ更新用スクリプト（下書き保存時に利用）
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

	// リスト自動選択用JS
	function select_message_list(messageNo)
	{
		// 選択
		$("table#message_list tr#message_tr" + messageNo).trigger("click");
	}

	// アクションボタンリフレッシュ
	function refresh_action_button()
	{
		setTimeout('__doPostBack("<%= lbRefreshActionButton.UniqueID %>", "");', 100);
	}

	// ポップアップ ウィンドウを閉じる確認します。
	function close_popup() {
		if (confirm('<%= WebMessages.GetMessages(WebMessages.CFMMSG_MANAGER_MESSAGE_POPUP_CLOSE_CONFIRM) %>'))
			window.close();
			else return;
		}

	function bodyPageLoad() {
		ddlSelect();

		if (Sys.WebForms == null) return;
		$('[id$=ddlAnswerTemplateCategories]').each(function (index, element) { $(this).width("250px") });
		$('#ctl00_ContentPlaceHolderBody_divUserList .y_scrollable').resizable2({
			handleSelector: "#win-size-grip4",
			resizeWidth: false,
			onDragStart: function (e, $el, opt) {
				$el.css({ 'height': $el.height(), 'max-height': '<%= Constants.VARIABLE_MAXIMUM_SIZE %>px' });
			},
			onDragEnd: function (e, $el, opt) {
				setCookie("<%=Constants.COOKIE_KEY_MAIL_USER_SEARCH_HEIGHT%>", $el.height(), { expires: 1000 });
			}
		});
		$('#mail_input').resizable2({
			handleSelector: "#win-size-grip5",
			resizeWidth: false,
			onDragStart: function (e, $el, opt) {
				$el.css({ 'height': $el.height(), 'max-height': '<%= Constants.VARIABLE_MAXIMUM_SIZE %>px' });
			},
			onDragEnd: function (e, $el, opt) {
				setCookie("<%=Constants.COOKIE_KEY_MESSAGE_INPUT_HEIGHT%>", $el.height(), { expires: 1000 });
			}
		});

		$('#ctl00_ContentPlaceHolderBody_divUserList .y_scrollable').css({ 'height': 'auto', 'max-height': '<%= (CookieManager.GetValue(Constants.COOKIE_KEY_MAIL_USER_SEARCH_HEIGHT) ?? Constants.MAIL_USER_SEARCH_DEFAULT_HEIGHT_SIZE) %>px' });
		$('#mail_input').css({ 'height': 'auto', 'max-height': '<%= (CookieManager.GetValue(Constants.COOKIE_KEY_MESSAGE_INPUT_HEIGHT) ?? Constants.MESSAGE_INPUT_DEFAULT_HEIGHT_SIZE) %>px' });
	}

	// ロック解除して閉じるボタン押下時のポップアップ
	function btnUnlockAndCloseConfirm() {
		return confirm('ロック解除して画面を閉じます。よろしいでしょうか？');
	}

</script>
</asp:Content>
